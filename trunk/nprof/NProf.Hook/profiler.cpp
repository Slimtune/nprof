/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003 by Matthew Mastracci
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

//#include "stdafx.h"
//#include "profiler.h"
//#include "profiler_socket.h"
#include "profiler.h"

Profiler::Profiler( ICorProfilerInfo* profilerInfo )
{
	this->profilerInfo = profilerInfo;
	this->profilerHelper.Initialize( profilerInfo );
}

Profiler::~Profiler()
{
}

/** No descriptions */
void Profiler::Enter( FunctionID functionId )
{
  ThreadInfo* threadInfo=GetCurrentThreadInfo();
  FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
  threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
}

/** No descriptions */
void Profiler::Leave( FunctionID functionId )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

/** No descriptions */
void Profiler::TailCall( FunctionID functionId )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

void Profiler::UnmanagedToManagedCall( FunctionID functionId )
{
  ThreadInfo* threadInfo=GetCurrentThreadInfo();
  FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
  threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
}

void Profiler::ManagedToUnmanagedCall( FunctionID functionId )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

/** No descriptions */
void Profiler::ThreadStart( ThreadID threadId )
{
  cout << "ThreadStart( " << threadId << " )" << endl;
  threadCollection.GetThreadInfo( threadId )->Start();
  ProfilerSocket ps;
  ps.SendThreadCreate( threadId );
}

void Profiler::ThreadMap( ThreadID threadId, DWORD dwOSThread )
{
  cout << "ThreadMap( " << threadId << ", " << dwOSThread << ")" << endl;
  threadMap[ dwOSThread ] = threadId;
}

/** No descriptions */
void Profiler::ThreadEnd( ThreadID threadId )
{
  threadCollection.EndThread( profilerHelper, threadId );
  cout << "ThreadEnd( " << threadId << " )" << endl;
}

/** No descriptions */
void Profiler::ThreadSuspend()
{
  //cout << "ThreadSuspend( " << GetCurrentThreadID() << " )" << endl;
  threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->SuspendFunction( rdtsc() );
}

/** No descriptions */
void Profiler::ThreadResume()
{
  //cout << "ThreadResume( " << GetCurrentThreadID() << " )" << endl;
  threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->ResumeFunction( rdtsc() );
}

void Profiler::AppDomainStart( AppDomainID appDomainId )
{
  cout << "AppDomain Created: " << appDomainId << endl;
  ProfilerSocket ps;
  ps.SendAppDomainCreate( appDomainId );
}

void Profiler::AppDomainEnd( AppDomainID appDomainId )
{
}

void Profiler::End()
{
  cout << "End()" << endl;
  threadCollection.EndAll( profilerHelper );
}

/** No descriptions */
ThreadID Profiler::GetCurrentThreadID()
{
  return profilerHelper.GetCurrentThreadID();
}

/** No descriptions */
ThreadInfo* Profiler::GetCurrentThreadInfo()
{
  return threadCollection.GetThreadInfo( GetCurrentThreadID() );
}

/** No descriptions */
void Profiler::Trace()
{
  threadCollection.Trace( profilerHelper );
}


CalleeFunctionInfo::CalleeFunctionInfo()
{
  this->cycleCount = 0;
  this->recursiveCycleCount = 0;
  this->recursiveCount = 0;
  this->calls = 0;
}

CalleeFunctionInfo::~CalleeFunctionInfo()
{
}



FunctionInfo::FunctionInfo( FunctionID functionId )
{
  this->cycleCount = -1;
  this->recursiveCycleCount = -1;
  this->suspendCycleCount = 0;
  this->calls = 0;
  this->recursiveCount = 0;
  this->functionId = functionId;
}

FunctionInfo::~FunctionInfo()
{
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

/** No descriptions */
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

[ module(dll, uuid = "{A461E20A-C7DC-4A89-A24E-87B5E975A96B}", 
		 name = "NProfHook", 
		 helpstring = "NProf.Hook 1.0 Type Library",
		 resource_name = "IDR_NPROFHOOK") ];


ProfilerHelper::ProfilerHelper()
{
}

ProfilerHelper::~ProfilerHelper(void)
{
}

void ProfilerHelper::Initialize(  ICorProfilerInfo* profilerInfo )
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

/* static public */
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

/* static public */
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

bool ProfilerSocket::isInitialized = false;
int ProfilerSocket::applicationId = -1;

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

void ProfilerSocket::Initialize()
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


StackEntryInfo::StackEntryInfo( FunctionInfo* functionInfo, INT64 cycleStart )
{
  this->functionInfo = functionInfo;
  this->cycleStart = cycleStart;
}

StackEntryInfo::StackEntryInfo( const StackEntryInfo& stackEntry )
{
  this->functionInfo = stackEntry.functionInfo;
  this->cycleStart = stackEntry.cycleStart;
}

StackEntryInfo::~StackEntryInfo()
{
}

StackInfo::StackInfo( ThreadInfo* threadInfo )
{
	this->threadInfo = threadInfo;
}

StackInfo::~StackInfo()
{
}

/** No descriptions */
void StackInfo::PushFunction( FunctionInfo* functionInfo, INT64 cycleCount )
{
	if ( functionStack.size() > 0 )
	{
		// Increment the recursive count of this callee function info so we don't double-book entries
		FunctionInfo* callerFunction = functionStack.top().functionInfo;
		FunctionID calleeId = functionInfo->functionId;
		
		CalleeFunctionInfo* calleeFunction = callerFunction->GetCalleeFunctionInfo( calleeId );
		calleeFunction->recursiveCount++;
		calleeFunction->calls++;
	}

	// Increment the recursive count of this function info so we don't double-book entries
	functionInfo->recursiveCount++;
	functionInfo->calls++;

	functionStack.push( StackEntryInfo( functionInfo, cycleCount ) );
}

/** No descriptions */
INT64 StackInfo::PopFunction( INT64 cycleCount )
{
	INT64 elapsed = cycleCount - functionStack.top().cycleStart;
	FunctionInfo* functionInfo = functionStack.top().functionInfo;

	FunctionID calleeId = functionInfo->functionId;

	// Only add the time if we're at the lowest call to the function on the stack
	// Prevents double-accounting of recursive functions
	functionInfo->recursiveCount--;
	if ( functionInfo->recursiveCount == 0 )
		functionInfo->cycleCount += elapsed;
	else
		functionInfo->recursiveCycleCount += elapsed;

	functionStack.pop();

	if ( functionStack.size() > 0 )
	{
		CalleeFunctionInfo* calleeFunction = functionStack.top().functionInfo->GetCalleeFunctionInfo( calleeId );

		// Only add the time if we're at the lowest call to the function on the stack
		// Prevents double-accounting of recursive functions
		calleeFunction->recursiveCount--;
		if ( calleeFunction->recursiveCount == 0 )
			calleeFunction->cycleCount += elapsed;
		else
			calleeFunction->recursiveCycleCount += elapsed;
	}

	return elapsed;
}

/** No descriptions */
void StackInfo::SuspendFunction( INT64 cycleCount )
{
	suspendStart = cycleCount;
	if ( functionStack.size() == 0 ) 
	{
		cout << "Suspend with no call stack!" << endl;
		return;
	}
	//cout << "Suspended function ID: " << functionStack.top().functionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::ResumeFunction( INT64 cycleCount )
{
	INT64 elapsed = cycleCount - suspendStart;
	// Resume with no call stack, ignore
	if ( functionStack.size() == 0 ) 
	{
		cout << "Resume with no call stack!" << endl;
		return;
	}
	functionStack.top().functionInfo->suspendCycleCount += elapsed;
	threadInfo->suspendTime += elapsed;
	//cout << "Resumed function ID: " << functionStack.top().functionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::Trace()
{
	cout << "Stack depth = " << functionStack.size() << endl;
}

ThreadInfo::ThreadInfo()
{
  this->isRunning = false;
  this->suspendTime = 0;
  this->stackInfo = new StackInfo( this );
}

ThreadInfo::~ThreadInfo()
{
	delete stackInfo;
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

StackInfo* ThreadInfo::GetStackInfo()
{
  return stackInfo;
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

/** No descriptions */
void ThreadInfo::Trace( ProfilerHelper& profilerHelper )
{
  for ( map< FunctionID, FunctionInfo* >::iterator i = functionMap.begin(); i != functionMap.end(); i++ )
  {
    cout << "Function ID " << i->first << ":" << endl;
    //cout << ph.GetFunctionSignature( it->first );
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

ThreadInfoCollection::ThreadInfoCollection()
{
}

ThreadInfoCollection::~ThreadInfoCollection()
{
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

/** No descriptions */
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

void ThreadInfoCollection::Dump( ProfilerHelper& profilerHelper, ThreadID threadId )
{
  ProfilerSocket profilerSocket;
  profilerSocket.SendStartFunctionData( threadId );
  GetThreadInfo( threadId )->Dump( profilerSocket, profilerHelper );
  profilerSocket.SendEndFunctionData();
}
Profiler* CNProfCORHook::profiler;