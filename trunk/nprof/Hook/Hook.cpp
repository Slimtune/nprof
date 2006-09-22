/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003,2004,2005,2006 by Matthew Mastracci, Christian Staudenmeyer
    email                : mmastrac@canada.com
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/

#ifndef PROFILER_H
#define PROFILER_H

#include "resource.h"


#pragma once

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#ifndef WINVER				// Allow use of features specific to Windows 95 and Windows NT 4 or later.
#define WINVER 0x0400		// Change this to the appropriate value to target Windows 98 and Windows 2000 or later.
#endif

#ifndef _WIN32_WINNT		// Allow use of features specific to Windows NT 4 or later.
#define _WIN32_WINNT 0x0400	// Change this to the appropriate value to target Windows 2000 or later.
#endif						

#ifndef _WIN32_WINDOWS		// Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0410 // Change this to the appropriate value to target Windows Me or later.
#endif

#ifndef _WIN32_IE			// Allow use of features specific to IE 4.0 or later.
#define _WIN32_IE 0x0400	// Change this to the appropriate value to target IE 5.0 or later.
#endif

#define _ATL_APARTMENT_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS	// some CString constructors will be explicit

// turns off ATL's hiding of some common and often safely ignored warning messages
#define _ATL_ALL_WARNINGS

// ATL includes
#include <atlbase.h>
#include <atlcom.h>
#include <atlwin.h>
#include <atltypes.h>
#include <atlctl.h>
#include <atlhost.h>

using namespace ATL;

// STL includes
#pragma warning ( disable : 4530 )

#include <iostream>
#include <map>
#include <string>
#include <stack>
#include <utility>
#include <memory>
#include <sstream>
#include <fstream>
#include <vector>

#include "windows.h"
#include "winnt.h"
using namespace std;

// COR includes
#include "cor.h"
#include "corhdr.h"
#include "corhlpr.h"
#include "corerror.h"

//#include "corsym.h"
#include "corpub.h"
#include "corprof.h"

#define MAX_FUNCTION_LENGTH 2048


#ifdef WIN32
  UINT64 __fastcall rdtsc();
#else
  __inline__ unsigned UINT64 int rdtsc(void);
#endif



class ProfilerHelper
{
public:
	void ProfilerHelper::Initialize(  ICorProfilerInfo2* profilerInfo )
	{
		this->profilerInfo = profilerInfo;
	}
	ThreadID ProfilerHelper::GetCurrentThreadID()
	{
		ThreadID threadId;
		profilerInfo->GetCurrentThreadID( &threadId );

		return threadId;
	}
	string ProfilerHelper::GetCurrentThreadName()
	{
		return "";
	}
	void ProfilerHelper::GetFunctionSignature( 
		FunctionID functionId,
		UINT32& methodAttributes,
		string& returnType, 
		string& className,
		string& functionName,
		string& parameters )
	{
		ULONG args;
		WCHAR returnTypeString[ MAX_FUNCTION_LENGTH ];
		WCHAR parametersString[ MAX_FUNCTION_LENGTH ];
		WCHAR functionNameString[ MAX_FUNCTION_LENGTH ];
		WCHAR classNameString[ MAX_FUNCTION_LENGTH ];
		GetFunctionProperties( functionId, &methodAttributes, &args, returnTypeString, parametersString, classNameString, functionNameString );

		returnType = CW2A( returnTypeString );
		parameters = CW2A( parametersString );
		className = CW2A( classNameString );
		functionName = CW2A( functionNameString );
	}
private:
	HRESULT ProfilerHelper::GetFunctionProperties( 
					   FunctionID functionID,
										   UINT32* methodAttributes,
										   ULONG *argCount,
										   WCHAR *returnType, 
										   WCHAR *parameters,
										   WCHAR *className,
						   WCHAR *funName )
	{
		HRESULT hr = E_FAIL; // assume success
	

		// init return values
		*argCount = 0;
		returnType[0] = NULL; 
		parameters[0] = NULL;
		funName[0] = NULL;
		className[0] = NULL;



		if ( functionID != NULL )
		{
			mdToken	token;
			ClassID classID;
			ModuleID moduleID;
			IMetaDataImport *metaDataImport = NULL;	
			mdToken moduleToken;
				    
				    
		    
			//
			// Get the classID 
			//
			try
			{
				hr = profilerInfo->GetFunctionInfo(
							functionID,
							&classID,
							&moduleID,
							&moduleToken );
			}
			catch ( ... )
			{
				hr = E_FAIL;
			}

			if ( FAILED( hr ) )
			{
				hr = S_OK;
				swprintf( funName, L"FAILED" );	
			}
			else
			{
				//
				// Get the MetadataImport interface and the metadata token 
				//
				hr = profilerInfo->GetTokenAndMetaDataFromFunction( functionID, 
		           								 				IID_IMetaDataImport, 
																(IUnknown **)&metaDataImport,
																&token );
				if ( SUCCEEDED( hr ) )
				{
					hr = metaDataImport->GetMethodProps( token,
													NULL,
													funName,
													MAX_FUNCTION_LENGTH,
													0,
													0,
													NULL,
													NULL,
													NULL, 
													NULL );
					if ( SUCCEEDED( hr ) )
					{
						mdTypeDef classToken = NULL;

						hr = profilerInfo->GetClassIDInfo( classID, 
					        						   NULL,  
													   &classToken );
						
						if SUCCEEDED( hr )
						{
			      			if ( classToken != mdTypeDefNil )
							{
			          			hr = metaDataImport->GetTypeDefProps( classToken, 
																 className, 
																 MAX_FUNCTION_LENGTH,
																 NULL, 
																 NULL, 
																 NULL ); 
							}

							DWORD methodAttr = 0;
							PCCOR_SIGNATURE sigBlob = NULL;


							hr = metaDataImport->GetMethodProps( (mdMethodDef) token,
															NULL,
															NULL,
															0,
															NULL,
															&methodAttr,
															&sigBlob,
															NULL,
															NULL,
															NULL );
							if ( SUCCEEDED( hr ) )
							{
								ULONG callConv;


								//
								// Is the method static ?
								//
				  *methodAttributes = methodAttr;

				     			//
								// Make sure we have a method signature.
								//
								char buffer[2 * MAX_FUNCTION_LENGTH];
							    
							    
								sigBlob += CorSigUncompressData( sigBlob, &callConv );
								if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
								{
									static WCHAR* callConvNames[8] = 
									{	
										L"", 
										L"unmanaged cdecl ", 
										L"unmanaged stdcall ",	
										L"unmanaged thiscall ",	
										L"unmanaged fastcall ",	
										L"vararg ",	 
										L"<error> "	 
										L"<error> "	 
									};	
									buffer[0] = '\0';
									if ( (callConv & 7) != 0 )
										sprintf( buffer, "%s ", callConvNames[callConv & 7]);	
									
									//
									// Grab the argument count
									//
									sigBlob += CorSigUncompressData( sigBlob, argCount );

									//
									// Get the return type
									//
									sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );

									//
									// if the return typ returned back empty, write void
									//
									if ( buffer[0] == '\0' )
										sprintf( buffer, "void" );

									swprintf( returnType, L"%S",buffer );
									
									//
									// Get the parameters
									//								
									for ( ULONG i = 0; 
										  (SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount))); 
										  i++ )
									{
										buffer[0] = '\0';

										sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );									
										if ( i == 0 )
											swprintf( parameters, L"%S", buffer );

										else if ( sigBlob != NULL )
											swprintf( parameters, L"%s, %S", parameters, buffer );
										
										else
											hr = E_FAIL;
									}								    								
								}
								else
								{
									//
									// Get the return type
									//
									buffer[0] = '\0';
									sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );
									swprintf( returnType, L"%s %S",returnType, buffer );
								}
							} 
						} 
					} 

					metaDataImport->Release();
				} 		
			}
		}
		//
		// This corresponds to an unmanaged frame
		//
		else
		{
			//
			// Set up return parameters
			//
			hr = S_OK;
			swprintf( funName, L"UNMANAGED FRAME" );	
		}


		return hr;
	} // BASEHELPER::GetFunctionProperties

	PCCOR_SIGNATURE ProfilerHelper::ParseElementType( IMetaDataImport *metaDataImport,
											  PCCOR_SIGNATURE signature, 
											  char *buffer )
	{
		switch ( *signature++ ) 
		{	
			case ELEMENT_TYPE_VOID:
				strcat( buffer, "void" );	
				break;					
			
		    
			case ELEMENT_TYPE_BOOLEAN:	
				strcat( buffer, "bool" );	
				break;	
			
		    
			case ELEMENT_TYPE_CHAR:
				strcat( buffer, "wchar" );	
				break;		
						
		    
			case ELEMENT_TYPE_I1:
				strcat( buffer, "int8" );	
				break;		
			
		    
			case ELEMENT_TYPE_U1:
				strcat( buffer, "unsigned int8" );	
				break;		
			
		    
			case ELEMENT_TYPE_I2:
				strcat( buffer, "int16" );	
				break;		
			
		    
			case ELEMENT_TYPE_U2:
				strcat( buffer, "unsigned int16" );	
				break;			
			
		    
			case ELEMENT_TYPE_I4:
				strcat( buffer, "int32" );	
				break;
		        
		    
			case ELEMENT_TYPE_U4:
				strcat( buffer, "unsigned int32" );	
				break;		
			
		    
			case ELEMENT_TYPE_I8:
				strcat( buffer, "int64" );	
				break;		
			
		    
			case ELEMENT_TYPE_U8:
				strcat( buffer, "unsigned int64" );	
				break;		
			
		    
			case ELEMENT_TYPE_R4:
				strcat( buffer, "float32" );	
				break;			
			
		    
			case ELEMENT_TYPE_R8:
				strcat( buffer, "float64" );	
				break;		
			
		    
			case ELEMENT_TYPE_U:
				strcat( buffer, "unsigned int" );	
				break;		 
			
		    
			case ELEMENT_TYPE_I:
				strcat( buffer, "int" );	
				break;			  
			
		    
			case ELEMENT_TYPE_OBJECT:
				strcat( buffer, "Object" );	
				break;		 
			
		    
			case ELEMENT_TYPE_STRING:
				strcat( buffer, "String" );	
				break;		 
			
		    
			case ELEMENT_TYPE_TYPEDBYREF:
				strcat( buffer, "refany" );	
				break;				       

			case ELEMENT_TYPE_CLASS:	
			case ELEMENT_TYPE_VALUETYPE:
			case ELEMENT_TYPE_CMOD_REQD:
			case ELEMENT_TYPE_CMOD_OPT:
				{	
					mdToken	token;	
					char classname[MAX_FUNCTION_LENGTH];


					classname[0] = '\0';
	   				signature += CorSigUncompressToken( signature, &token ); 
					if ( TypeFromToken( token ) != mdtTypeRef )
					{
        				HRESULT	hr;
						WCHAR zName[MAX_FUNCTION_LENGTH];
						
						
						hr = metaDataImport->GetTypeDefProps( token, 
														 zName,
														 MAX_FUNCTION_LENGTH,
														 NULL,
														 NULL,
														 NULL );
						if ( SUCCEEDED( hr ) )
							wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
					}
		                
					strcat( buffer, classname );		
				}
				break;	
			
		    
			case ELEMENT_TYPE_SZARRAY:	 
				signature = ParseElementType( metaDataImport, signature, buffer ); 
				strcat( buffer, "[]" );
				break;		
			
		    
			case ELEMENT_TYPE_ARRAY:	
				{	
					ULONG rank;
		            

					signature = ParseElementType( metaDataImport, signature, buffer );                 
					rank = CorSigUncompressData( signature );													
					if ( rank == 0 ) 
						strcat( buffer, "[?]" );

					else 
					{
						ULONG *lower;	
						ULONG *sizes; 	
						ULONG numsizes; 
						ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
		                
		                                     
						lower = (ULONG *)_alloca( arraysize );                                                        
						memset( lower, 0, arraysize ); 
						sizes = &lower[rank];

						numsizes = CorSigUncompressData( signature );	
						if ( numsizes <= rank )
						{
            				ULONG numlower;
		                    
		                    
							for ( ULONG i = 0; i < numsizes; i++ )	
								sizes[i] = CorSigUncompressData( signature );	
							
		                    
							numlower = CorSigUncompressData( signature );	
							if ( numlower <= rank )
							{
								for (ULONG i = 0; i < numlower; i++)	
									lower[i] = CorSigUncompressData( signature ); 
								
		                        
								strcat( buffer, "[" );	
								for (ULONG i = 0; i < rank; i++ )	
								{	
									if ( (sizes[i] != 0) && (lower[i] != 0) )	
									{	
										if ( lower[i] == 0 )	
											sprintf ( buffer, "%d", sizes[i] );	

										else	
										{	
											sprintf( buffer, "%d", lower[i] );	
											strcat( buffer, "..." );	
											
											if ( sizes[i] != 0 )	
												sprintf( buffer, "%d", (lower[i] + sizes[i] + 1) );	
										}	
									}
		                            	
									if ( i < (rank - 1) ) 
										strcat( buffer, "," );	
								}	
		                        
								strcat( buffer, "]" );  
							}						
						}
					}
				} 
				break;	

			
			case ELEMENT_TYPE_PINNED:
				signature = ParseElementType( metaDataImport, signature, buffer ); 
				strcat( buffer, "pinned" );	
				break;	
		     
		    
			case ELEMENT_TYPE_PTR:   
				signature = ParseElementType( metaDataImport, signature, buffer ); 
				strcat( buffer, "*" );	
				break;   
		    
		    
			case ELEMENT_TYPE_BYREF:   
				signature = ParseElementType( metaDataImport, signature, buffer ); 
				strcat( buffer, "&" );	
				break;  		    


			default:	
			case ELEMENT_TYPE_END:	
			case ELEMENT_TYPE_SENTINEL:	
				strcat( buffer, "<UNKNOWN>" );	
				break;				                      				            
		                    	
		} // switch	


		return signature;

	} // BASEHELPER::ParseElementType
	CComPtr< ICorProfilerInfo2 > profilerInfo;
};

enum NetworkMessage
{
	INITIALIZE = 0,
	SHUTDOWN,
	APPDOMAIN_CREATE,
	THREAD_CREATE,
	THREAD_END,
	FUNCTION_DATA,
	PROFILER_MESSAGE,
};

const int NETWORK_PROTOCOL_VERSION = 3;


#define SAFE_SEND( socket, data ) \
	{ \
		int sent = send( socket, ( const char * )&data, sizeof( data ), 0 ); \
		if ( sent != sizeof( data ) ) \
		{ \
			HandleError( "SAFE_SEND", WSAGetLastError() ); \
			HandleWrongSentLength( "SAFE_SEND", sizeof( data ), sent ); \
		} \
	}

#define SAFE_SEND_RAW( socket, data, length ) \
	{ \
		int sent = send( socket, data, length, 0 ); \
		if ( sent != length ) \
		{ \
			HandleError( "SAFE_SEND_RAW", WSAGetLastError() ); \
			HandleWrongSentLength( "SAFE_SEND_RAW", sizeof( data ), sent ); \
		} \
	}

#define SAFE_READ( socket, data, result ) \
	{ \
		int nRecv = recv( socket, ( char * )&data, sizeof( data ), 0 ); \
		if ( nRecv != sizeof( data ) ) \
		{ \
			HandleError( "SAFE_READ", WSAGetLastError() ); \
			HandleWrongRecvLength( "SAFE_READ", sizeof( data ), nRecv ); \
			result = 1; \
		} \
		else \
		{ \
			result = 0; \
		} \
	}
class ProfilerSocket
{
public:
	ProfilerSocket::ProfilerSocket()
	{
		this->operation = "ctor";
		this->isApplicationIdSent = false;

		sockaddr_in sa;
		sa.sin_family = AF_INET;
		sa.sin_port = htons( atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) );
		sa.sin_addr.S_un.S_addr = inet_addr( "127.0.0.1" );

		socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, 0);
		if ( socket == INVALID_SOCKET )
		{
			HandleError( "WSASocket", WSAGetLastError() );
		}
		else
		{
			int n = 0;
			while ( n < 10 )
			{
				int nResult = connect( socket, ( const sockaddr * )&sa, sizeof( sa ) );
				if ( nResult == SOCKET_ERROR )
				{
					HandleError( "connect", WSAGetLastError() );
				}
				else
				{
					return;
				}

				Sleep( 200 );
				n++;
			}
		}
		this->socket = INVALID_SOCKET;
	}

	static void ProfilerSocket::Initialize()
	{
		WSADATA wsaData;
		WSAStartup( MAKEWORD( 2, 2 ), &wsaData );
		cout << "WSAStartup: " << WSAGetLastError() << endl;
		cout << "port = " << atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) << endl;
		ProfilerSocket ps;
		ps.SendInitialize();
	}

	void ProfilerSocket::SendInitialize()
	{
		operation = "SendInitialize";

		SendNetworkMessage( INITIALIZE );
		SendUINT32( NETWORK_PROTOCOL_VERSION );
		BYTE b;
		if ( ReadByte( b ) == 0 )
		{
			if ( b == 2 )
			{
				b = 1;
				::DebugBreak();
			}

			if ( b == 1 )
			{
				cout << "Successfully initialized profiler socket!" << endl;
				ReadByte( b );
				applicationId = b;

				cout << "Application ID = " << applicationId << endl;

				SendUINT32( ::GetCurrentProcessId() );
				
				int nArgs;
				LPWSTR* pstrCmdLine = ::CommandLineToArgvW( ::GetCommandLineW(), &nArgs );

				SendUINT32( nArgs );

				for ( int nArg = 0; nArg < nArgs; nArg++ )
				{
					string strArg = CW2A( pstrCmdLine[ nArg ] );
					SendString( strArg );
				}
				
				::GlobalFree( pstrCmdLine );

				isInitialized = true;
			}
			else
			{
				cout << "We weren't allowed to initialize!" << endl;
				isInitialized = false;
			}
		}
		else
		{
			cout << "Could not initialize!" << endl;
			isInitialized = false;
		}
	}

	void ProfilerSocket::SendShutdown()
	{
		if ( !isInitialized )
			return;
		operation = "SendShutdown";

		SendNetworkMessage( SHUTDOWN );
	}

	void ProfilerSocket::SendAppDomainCreate( AppDomainID aid )
	{
		operation = "SendAppDomainCreate";

		SendNetworkMessage( APPDOMAIN_CREATE );
		SendAppDomainID( aid );
	}

	void ProfilerSocket::SendThreadCreate( ThreadID tid )
	{
		operation = "SendThreadCreate";

		SendNetworkMessage( THREAD_CREATE );
		SendThreadID( tid );
	}

	void ProfilerSocket::SendThreadEnd( ThreadID tid, UINT64 llThreadStartTime, UINT64 threadEndTime )
	{
		operation = "SendThreadEnd";

		SendNetworkMessage( THREAD_END );
		SendThreadID( tid );
		SendUINT64( llThreadStartTime );
		SendUINT64( threadEndTime );
	}

	void ProfilerSocket::SendStartFunctionData( ThreadID tid )
	{
		operation = "SendStartFunctionData";

		SendNetworkMessage( FUNCTION_DATA );
		SendThreadID( tid );
	}

	void ProfilerSocket::SendFunctionData( ProfilerHelper& ph, FunctionID fid )
	{
		operation = "SendFunctionData";

		SendFunctionID( fid );
		string returnType, className, functionName, parameters;
		UINT32 methodAttributes;
		ph.GetFunctionSignature( fid, methodAttributes, returnType, className, functionName, parameters );

		SendUINT32( methodAttributes );
		SendString( returnType );
		SendString( className );
		SendString( functionName );
		SendString( parameters );
	}

	void ProfilerSocket::SendProfilerMessage( const string& strMessage )
	{
		operation = "SendProfilerMessage";

		SendNetworkMessage( PROFILER_MESSAGE );
		SendString( strMessage );
	}

	void ProfilerSocket::SendFunctionTimingData( int calls, UINT64 cycleCount, UINT64 recursiveCycleCount, UINT64 suspendCycleCount )
	{
		operation = "SendFunctionTimingData";

		SendUINT32( calls );
		SendUINT64( cycleCount );
		SendUINT64( recursiveCycleCount );
		SendUINT64( suspendCycleCount );
	}

	void ProfilerSocket::SendCalleeFunctionData( FunctionID fid, int calls, UINT64 cycleCount, UINT64 recursiveCycleCount )
	{
		operation = "SendCalleeFunctionData";

		SendFunctionID( fid );
		SendUINT32( calls );
		SendUINT64( cycleCount );
		SendUINT64( recursiveCycleCount );
	}

	void ProfilerSocket::SendEndFunctionData()
	{
		operation = "SendEndFunctionData";

		SendFunctionID( 0xffffffff );
	}

	void ProfilerSocket::SendEndCalleeFunctionData()
	{
		operation = "SendEndCalleeFunctionData";

		SendFunctionID( 0xffffffff );
	}

	int ProfilerSocket::ReadByte( BYTE& b )
	{
		int result;
		SAFE_READ( socket, b, result );

		return result;
	}

	void ProfilerSocket::HandleError( const char* caller, int error )
	{
	//	std::ofstream outf;
	//	outf.open( "c:\\nprof.log", ios::app );
	//	outf << operation << " " << caller << " Error: " << nError << " on socket " << _s << endl;
	}

	void ProfilerSocket::HandleWrongSentLength( const char* caller, int expected, int sent )
	{
	//	std::ofstream outf;
	//	outf.open( "c:\\nprof.log", ios::app );
	//	outf << operation << " " << caller << " Expected to send " << expected << ", sent " << sent << " instead." << endl;
	}

	void ProfilerSocket::HandleWrongRecvLength( const char* caller, int expected, int sent )
	{
	//	std::ofstream outf;
	//	outf.open( "c:\\nprof.log", ios::app );
	//	outf << operation << " " << caller << " Expected to send " << expected << ", sent " << sent << " instead." << endl;
	}

	ProfilerSocket::~ProfilerSocket(void)
	{
		closesocket( socket );
	}


private:

	void ProfilerSocket::SendBool( bool boolean )
	{
		SAFE_SEND( socket, boolean );
	}

	void ProfilerSocket::SendUINT32( UINT32 integer )
	{
		SAFE_SEND( socket, integer );
	}

	void ProfilerSocket::SendUINT64( UINT64 integer)
	{
		SAFE_SEND( socket, integer);
	}

	void ProfilerSocket::SendString( const string& signature )
	{
		SendUINT32( ( UINT32 )signature.length() );
		SAFE_SEND_RAW( socket, signature.c_str(), ( int )signature.length() );
	}

	void ProfilerSocket::SendNetworkMessage( NetworkMessage networkMessage )
	{
		UINT16 mess = networkMessage;
		SAFE_SEND( socket, mess );

		if ( !isApplicationIdSent)
		{
			if ( networkMessage != INITIALIZE )
				SendUINT32( applicationId );

			isApplicationIdSent = true;
		}
	}

	void ProfilerSocket::SendAppDomainID( AppDomainID appDomainId )
	{
		SAFE_SEND( socket, appDomainId );
	}

	void ProfilerSocket::SendThreadID( ThreadID threadId )
	{
		SAFE_SEND( socket, threadId );
	}

	void ProfilerSocket::SendFunctionID( FunctionID functionId )
	{
		SAFE_SEND( socket, functionId );
	}
	SOCKET socket;
	const char* operation;
	bool isApplicationIdSent;

	static bool isInitialized;
	static int applicationId;
};

class CalleeFunctionInfo {
public: 
	CalleeFunctionInfo::CalleeFunctionInfo()
	{
		this->cycleCount = 0;
		this->recursiveCycleCount = 0;
		this->recursiveCount = 0;
		this->calls = 0;
	}
	INT64 cycleCount;
	INT64 recursiveCycleCount;
	int calls;
	int recursiveCount;
};
class FunctionInfo
{
public: 

	FunctionInfo::FunctionInfo( FunctionID functionId )
	{
		this->cycleCount = -1;
		this->recursiveCycleCount = -1;
		this->suspendCycleCount = 0;
		this->calls = 0;
		this->recursiveCount = 0;
		this->functionId = functionId;
	}

	CalleeFunctionInfo* FunctionInfo::GetCalleeFunctionInfo( FunctionID functionId )
	{
		map< FunctionID, CalleeFunctionInfo* >::iterator found = calleeMap.find( functionId );
		if ( found == calleeMap.end() )
		{
			CalleeFunctionInfo* functionInfo = new CalleeFunctionInfo();
			calleeMap.insert( make_pair( functionId, functionInfo ) );
			return functionInfo;
		}
		return found->second;
	}

	void FunctionInfo::Trace( ProfilerHelper& profilerHelper )
	{
		cout << "  Calls: " << calls << endl;
		cout << "  Time: " << cycleCount << endl;
		cout << "  Avg. time: " << cycleCount / calls << endl;

		for ( map< FunctionID, CalleeFunctionInfo* >::iterator i = calleeMap.begin(); i != calleeMap.end(); i++ )
		{
			cout << "  Callee Function ID " << i->first << ":" << endl;
			cout << "    Calls: " << i->second->calls << endl;
			cout << "    Time: " << i->second->cycleCount << endl;
			cout << "    Avg. time: " << i->second->cycleCount / i->second->calls << endl;
		}
	}    

	void FunctionInfo::Dump( ProfilerSocket& ps, ProfilerHelper& profilerHelper )
	{
		ps.SendFunctionTimingData( calls, cycleCount, recursiveCycleCount, suspendCycleCount );
		for ( map< FunctionID, CalleeFunctionInfo* >::iterator i = calleeMap.begin(); i != calleeMap.end(); i++ )
		{
			ps.SendCalleeFunctionData( i->first, i->second->calls, i->second->cycleCount, i->second->recursiveCycleCount );
		}
		ps.SendEndCalleeFunctionData();
	}
	int calls;
	int recursiveCount;
	INT64 cycleCount;
	INT64 recursiveCycleCount;
	INT64 suspendCycleCount;
	FunctionID functionId;
	map< FunctionID, CalleeFunctionInfo* > calleeMap;
};

class ThreadInfo
{
public: 
	ThreadInfo::ThreadInfo()
	{
		this->isRunning = false;
		this->suspendTime = 0;
	}

	void ThreadInfo::Start()
	{
		startTime = rdtsc();
		isRunning = true;
	}

	void ThreadInfo::End()
	{
		endTime = rdtsc();
		isRunning = false;
	}

	bool ThreadInfo::IsRunning()
	{
		return isRunning;
	}

	FunctionInfo* ThreadInfo::GetFunctionInfo( FunctionID functionId )
	{
		map< FunctionID, FunctionInfo* >::iterator found = functionMap.find( functionId );
		if ( found == functionMap.end() )
		{
			FunctionInfo* functionInfo = new FunctionInfo( functionId );
			functionMap.insert( make_pair( functionId, functionInfo ) );
			return functionInfo;
		}

		return found->second;
	}

	void ThreadInfo::Trace( ProfilerHelper& profilerHelper )
	{
		for ( map< FunctionID, FunctionInfo* >::iterator i = functionMap.begin(); i != functionMap.end(); i++ )
		{
			cout << "Function ID " << i->first << ":" << endl;
			i->second->Trace( profilerHelper );
		}
	}

	void ThreadInfo::Dump( ProfilerSocket& ps, ProfilerHelper& profilerHelper )
	{
		for ( map< FunctionID, FunctionInfo* >::iterator i = functionMap.begin(); i != functionMap.end(); i++ )
		{
			ps.SendFunctionData( profilerHelper, i->first );
			i->second->Dump( ps, profilerHelper );
		}
	}
	INT64 startTime;
	INT64 endTime;
	INT64 suspendTime;
  
private:
	bool  isRunning;
	map< FunctionID, FunctionInfo* > functionMap;
};

class ThreadInfoCollection
{
public: 
	void Dump( ProfilerHelper& profilerHelper, ThreadID threadId )
	{
	  ProfilerSocket profilerSocket;
	  profilerSocket.SendStartFunctionData( threadId );
	  GetThreadInfo( threadId )->Dump( profilerSocket, profilerHelper );
	  profilerSocket.SendEndFunctionData();
	}

	void ThreadInfoCollection::EndAll( ProfilerHelper& profilerHelper )
	{
		for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
		if ( i->second->IsRunning() )
		EndThread( profilerHelper, i->first );
	}

	void ThreadInfoCollection::EndThread( ProfilerHelper& profilerHelper, ThreadID threadId )
	{
		ThreadInfo* threadInfo = GetThreadInfo( threadId );
		threadInfo->End();
		ProfilerSocket profilerSocket;
		profilerSocket.SendThreadEnd( threadId, threadInfo->startTime, threadInfo->endTime );
		Dump( profilerHelper, threadId );
	}

	ThreadInfo* ThreadInfoCollection::GetThreadInfo( ThreadID threadId )
	{
		map< ThreadID, ThreadInfo* >::iterator found = threadMap.find( threadId );
		if ( found == threadMap.end() )
		{
			ThreadInfo* threadInfo = new ThreadInfo();
			threadMap.insert( make_pair( threadId, threadInfo ) );
			return threadInfo;
		}
		return found->second;
	}

	void ThreadInfoCollection::Trace( ProfilerHelper& profilerHelper )
	{
		for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
		{
			cout << "Thread ID " << i->first << ":" << endl;
			i->second->Trace( profilerHelper );
		}
	}

	void ThreadInfoCollection::DumpAll( ProfilerHelper& profilerHelper )
	{
		for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
		{
			if ( i->second->IsRunning() )
			{
				ProfilerSocket profilerSocket;
				profilerSocket.SendStartFunctionData( i->first );
				i->second->Dump( profilerSocket, profilerHelper );
				profilerSocket.SendEndFunctionData();
			}
		}
	}
private:
	map< ThreadID, ThreadInfo* > threadMap;
};

HRESULT __stdcall __stdcall StackWalker( 
	FunctionID funcId,
	UINT_PTR ip,
	COR_PRF_FRAME_INFO frameInfo,
	ULONG32 contextSize,
	BYTE context[  ],
	void *clientData)
{
	//cout << funcId;// << "\n";
	if(funcId!=0)
	{
		((vector<FunctionID>*)clientData)->push_back(funcId);
	}
	return S_OK;
}


class Profiler
{
public: 
	Profiler::Profiler( ICorProfilerInfo2* profilerInfo )
	{
		this->profilerInfo = profilerInfo;
		this->profilerHelper.Initialize( profilerInfo );
		profilerInfo->SetEventMask( 
			COR_PRF_ENABLE_STACK_SNAPSHOT|
			COR_PRF_MONITOR_THREADS	|
			COR_PRF_DISABLE_INLINING |
			COR_PRF_MONITOR_SUSPENDS |    
			COR_PRF_MONITOR_EXCEPTIONS |  
			COR_PRF_MONITOR_APPDOMAIN_LOADS |
			COR_PRF_MONITOR_ASSEMBLY_LOADS |
			COR_PRF_MONITOR_CACHE_SEARCHES |
			COR_PRF_MONITOR_JIT_COMPILATION);

		TIMECAPS timeCaps;
		timeGetDevCaps(&timeCaps, sizeof(TIMECAPS));
		timer = timeSetEvent(
			10,
			timeCaps.wPeriodMin, 
			TimerFunction, 
			(DWORD_PTR)this,     
			TIME_PERIODIC);      
	}
	virtual void Leave( FunctionID functionId ){};
	virtual void Enter( FunctionID functionId ){};
	virtual void TailCall( FunctionID functionId ){};
	virtual void UnmanagedToManagedCall( FunctionID functionId ){};
	virtual void AppDomainEnd( AppDomainID appDomainId ){};
	
	void ManagedToUnmanagedCall( FunctionID functionId )
	{
	  //GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
	};

	void ThreadStart( ThreadID threadId )
	{
	  threadCollection.GetThreadInfo( threadId )->Start();
	  ProfilerSocket ps;
	  ps.SendThreadCreate( threadId );
	};

	void ThreadMap( ThreadID threadId, DWORD dwOSThread )
	{
		cout << "ThreadMap( " << threadId << ", " << dwOSThread << ")" << endl;
		threadMap[ dwOSThread ] = threadId;
	};

	void ThreadEnd( ThreadID threadId )
	{
	  threadCollection.EndThread( profilerHelper, threadId );
	  cout << "ThreadEnd( " << threadId << " )" << endl;
	};

	void ThreadSuspend()
	{
	  //threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->SuspendFunction( rdtsc() );
	};

	void ThreadResume()
	{
	  //threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->ResumeFunction( rdtsc() );
	};

	void AppDomainStart( AppDomainID appDomainId )
	{
		cout << "AppDomain Created: " << appDomainId << endl;
		ProfilerSocket ps;
		ps.SendAppDomainCreate( appDomainId );
	};

	virtual void End()
	{
		cout << "End()" << endl;
		threadCollection.EndAll( profilerHelper );
	};

protected:
	CComPtr< ICorProfilerInfo2 > profilerInfo;
	ThreadID GetCurrentThreadID()
	{
		return profilerHelper.GetCurrentThreadID();
	}
	ThreadInfo* GetCurrentThreadInfo()
	{
	  return threadCollection.GetThreadInfo( GetCurrentThreadID() );
	};
	ThreadInfoCollection threadCollection;
	ProfilerHelper profilerHelper;
	map< DWORD, ThreadID > threadMap;
	bool statistical;
public:
	static void CALLBACK TimerFunction(UINT wTimerID, UINT msg, 
		DWORD dwUser, DWORD dw1, DWORD dw2) 
	{
		Profiler* profiler=(Profiler*)dwUser;
		profiler->WalkStack();
	}
	static UINT timer;

	void WalkStack()
	{
		for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++)
		{
			DWORD threadId=(*i).first;
			HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
			if(threadHandle!=NULL)
			{

				int suspended=SuspendThread(threadHandle);
			}
		}
		for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++)
		{
			DWORD threadId=(*i).first;
			HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
			if(threadHandle!=NULL)
			{
				vector<FunctionID> functions;
				ThreadID id=i->second;

				profilerInfo->DoStackSnapshot(
					id,
					StackWalker,
					COR_PRF_SNAPSHOT_DEFAULT,
					&functions,
					NULL,
					NULL);
				
				ThreadInfo* threadInfo=threadCollection.GetThreadInfo(id);

				for(int index=0	;index<functions.size();index++)
				{
					for(int y=index+1;;y++)
					{
						if(y>functions.size()-1)
						{
							FunctionID id=functions[index];
							FunctionInfo* function=threadInfo->GetFunctionInfo(id);
							function->calls++;
							if(index<functions.size()-1)
							{
								function->GetCalleeFunctionInfo(functions[index+1])->calls++;
							}
							break;
						}
						if(functions[y]==functions[index])
						{
							break;
						}
					}
				}
				ResumeThread(threadHandle);
			}
		}
	}
};

UINT Profiler::timer;

void RawEnter();
void RawLeave();
void RawTailCall();

[
	object,
	uuid("5B94DF43-780B-42FD-AC4C-ABAB35D4A274"),
	dual,	helpstring("INProfCORHook Interface"),
	pointer_default(unique)
]
__interface INProfCORHook : IDispatch
{
};

[
  coclass,
  threading("apartment"),
  vi_progid("NProf.NProfCORHook"),
  progid("NProf.NProfCORHook.1"),
  version(1.0),
  uuid("791DA9FE-05A0-495E-94BF-9AD875C4DF0F"),
  helpstring("nprof COR Profiling Hook Class")
]
class ATL_NO_VTABLE CNProfCORHook : 
  public INProfCORHook,
  public ICorProfilerCallback2
{
public:
  CNProfCORHook()
  {
    this->profiler = NULL;
  }

  DECLARE_PROTECT_FINAL_CONSTRUCT()

  HRESULT FinalConstruct()
  {
    return S_OK;
  }

  void FinalRelease() 
  {
    if ( profiler )
      delete profiler;
  }

public:
	static Profiler* profiler;
	CRITICAL_SECTION criticalSection;
public:
	static Profiler* GetProfiler()
	{
		return profiler;
	}

	STDMETHOD(Initialize)(LPUNKNOWN pICorProfilerInfoUnk)
	{
		CComQIPtr< ICorProfilerInfo2 > profilerInfo = pICorProfilerInfoUnk;
		InitializeCriticalSection(&criticalSection);

		ProfilerSocket::Initialize();

		cout << "Initializing profiler hook DLL..." << endl;

		if ( profilerInfo )
		{
			profiler = new Profiler( profilerInfo );
			//profiler = new InstrumentationProfiler( profilerInfo );
			cout << "Initializing hooks..." << endl;
			profilerInfo->SetEnterLeaveFunctionHooks( ( FunctionEnter* )&RawEnter, ( FunctionLeave* )&RawLeave, ( FunctionTailcall* )&RawTailCall );
			cout << "Ready!" << endl;
		}

		return S_OK;
	}
	STDMETHOD(Shutdown)()
	{
		EnterCriticalSection(&criticalSection);
		cout << "Terminating profiler..." << endl;
		profiler->End();
		delete profiler;
		profiler = NULL;
		ProfilerSocket profilerSocket;
		profilerSocket.SendShutdown();
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(AppDomainCreationStarted)(AppDomainID appDomainId)
	{
		EnterCriticalSection(&criticalSection);
		profiler->AppDomainStart( appDomainId );
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(AppDomainCreationFinished)(AppDomainID appDomainId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainShutdownStarted)(AppDomainID appDomainId)
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);
		//profiler->AppDomainEnd( appDomainId );
		//LeaveCriticalSection(&criticalSection);
		//return S_OK;
	}
	STDMETHOD(AppDomainShutdownFinished)(AppDomainID appDomainId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadStarted)(AssemblyID assemblyId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadFinished)(AssemblyID assemblyId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadStarted)(AssemblyID assemblyId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadFinished)(AssemblyID assemblyId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadStarted)(ModuleID moduleId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadFinished)(ModuleID moduleId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadStarted)(ModuleID moduleId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadFinished)(ModuleID moduleId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleAttachedToAssembly)(ModuleID moduleId, AssemblyID assemblyId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadStarted)(ClassID classId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadFinished)(ClassID classId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadStarted)(ClassID classId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadFinished)(ClassID classId, HRESULT hrStatus)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(FunctionUnloadStarted)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationStarted)(FunctionID functionId, BOOL fIsSafeToBlock)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationFinished)(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchStarted)(FunctionID functionId, BOOL* pbUseCachedFunction)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchFinished)(FunctionID functionId, COR_PRF_JIT_CACHE result)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITFunctionPitched)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(JITInlining)(FunctionID callerId, FunctionID calleeId, BOOL* pfShouldInline)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadCreated)(ThreadID threadId)
	{
		EnterCriticalSection(&criticalSection);
		profiler->ThreadStart( threadId );
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(ThreadDestroyed)(ThreadID threadId)
	{
		EnterCriticalSection(&criticalSection);
		profiler->ThreadEnd( threadId );
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadId, DWORD osThreadId)
	{
		EnterCriticalSection(&criticalSection);
		profiler->ThreadMap( managedThreadId, osThreadId );
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(RemotingClientInvocationStarted)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientSendingMessage)(GUID * pCookie, BOOL fIsAsync)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientReceivingReply)(GUID * pCookie, BOOL fIsAsync)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientInvocationFinished)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerReceivingMessage)(GUID * pCookie, BOOL fIsAsync)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationStarted)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationReturned)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerSendingReply)(GUID * pCookie, BOOL fIsAsync)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(UnmanagedToManagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);
		//// Only track returns
		//if ( reason == COR_PRF_TRANSITION_RETURN )
		//  profiler->UnmanagedToManagedCall( functionId );
		//LeaveCriticalSection(&criticalSection);
		//return S_OK;
	}
	STDMETHOD(ManagedToUnmanagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);
		//// Only track calls
		//if ( reason == COR_PRF_TRANSITION_CALL )
		//  profiler->ManagedToUnmanagedCall( functionId );
		//LeaveCriticalSection(&criticalSection);
		//return S_OK;
	}
	STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason)
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);
		//profiler->ThreadSuspend();
		//LeaveCriticalSection(&criticalSection);
		//return S_OK;
	}
	STDMETHOD(RuntimeSuspendFinished)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendAborted)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeResumeStarted)()
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);
		//profiler->ThreadResume();
		//LeaveCriticalSection(&criticalSection);
		//return S_OK;
	}
	STDMETHOD(RuntimeResumeFinished)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadSuspended)(ThreadID threadId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadResumed)(ThreadID threadId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(MovedReferences)(unsigned long cMovedObjectIDRanges, ObjectID* oldObjectIDRangeStart, ObjectID* newObjectIDRangeStart, unsigned long * cObjectIDRangeLength)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectAllocated)(ObjectID objectId, ClassID classId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectsAllocatedByClass)(unsigned long cClassCount, ClassID* classIds, unsigned long* cObjects)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectReferences)(ObjectID objectId, ClassID classId, unsigned long cObjectRefs, ObjectID* objectRefIds)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences)(unsigned long cRootRefs, ObjectID* rootRefIds)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionThrown)(ThreadID thrownObjectId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionLeave)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterLeave)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerEnter)(UINT_PTR __unused)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerLeave)(UINT_PTR __unused)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionLeave)()
	{
		return E_NOTIMPL;
		//EnterCriticalSection(&criticalSection);

		//// Update the call stack as we leave
		//profiler->Leave( 0 );
		//LeaveCriticalSection(&criticalSection);

		//return S_OK;
	}
	STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFinallyLeave)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherEnter)(FunctionID functionId, ObjectID objectId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherLeave)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableCreated)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable, unsigned long cSlots)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableDestroyed)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherFound)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherExecute)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadNameChanged)(ThreadID threadId, ULONG cchName, WCHAR name[])
	{
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionStarted)(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(SurvivingReferences) (ULONG cSurvivingObjectIDRanges,ObjectID objectIDRangeStart[],ULONG cObjectIDRangeLength[])
	{
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionFinished)()
	{
		return E_NOTIMPL;
	}
	STDMETHOD(FinalizeableObjectQueued)(DWORD finalizerFlags,ObjectID objectID)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[],
			  COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[])
	{
		return E_NOTIMPL;
	}
	STDMETHOD(HandleCreated)(GCHandleID handleId,ObjectID initialObjectId)
	{
		return E_NOTIMPL;
	}
	STDMETHOD(HandleDestroyed)(GCHandleID handleId)
	{
		return E_NOTIMPL;
	}
};

void __stdcall EnterStub( FunctionID fid )
{
  CNProfCORHook::GetProfiler()->Enter( fid );
}

void __stdcall LeaveStub( FunctionID fid )
{
  CNProfCORHook::GetProfiler()->Leave( fid );
}

void __stdcall TailCallStub( FunctionID fid )
{
  CNProfCORHook::GetProfiler()->TailCall( fid );
}

void __declspec( naked ) RawEnter()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call EnterStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }   
}

void __declspec( naked ) RawLeave()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call LeaveStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }   
}

void __declspec( naked ) RawTailCall()
{
    __asm
    {
        push eax
        push ecx
        push edx
        push [esp + 16]
        call TailCallStub
        pop edx
        pop ecx
        pop eax
        ret 4
    }  
}
#ifdef WIN32
  __declspec(naked) UINT64 __fastcall rdtsc()
  {
    __asm
    {
      rdtsc;
      ret;
    }
  }
#else
  __inline__ unsigned UINT64 int rdtsc(void)
  {
    unsigned long long int x;
    __asm__ volatile (".byte 0x0f, 0x31" : "=A" (x));
    return x;
  }
#endif
#endif



[ module(dll, uuid = "{A461E20A-C7DC-4A89-A24E-87B5E975A96B}", 
		 name = "NProfHook", 
		 helpstring = "NProf.Hook 1.0 Type Library",
		 resource_name = "IDR_NPROFHOOK") ];





bool ProfilerSocket::isInitialized = false;
int ProfilerSocket::applicationId = -1;
Profiler* CNProfCORHook::profiler;