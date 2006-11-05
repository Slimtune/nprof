/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003, 2004, 2005, 2006 by Matthew Mastracci, Christian Staudenmeyer
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

#pragma warning ( disable : 4530 )
#include <atlbase.h>
#include <atlcom.h>
#include <map>
#include <string>
#include <sstream>
#include <fstream>
#include <vector>
#include "cor.h"
#include "corprof.h"
#include "Dbghelp.h"
#include "resource.h"

using namespace ATL;
using namespace std;

#define MAX_FUNCTION_LENGTH 2048

class Signature {
public:
	Signature() {
	}
	Signature(string returnType,string parameters,string className,string functionName) {
		this->returnType=returnType;
		this->parameters=parameters;
		this->className=className;
		this->functionName=functionName;
	}
	string returnType;
	string parameters;
	string className;
	string functionName;
};
class ProfilerHelper
{
public:
	void ProfilerHelper::Initialize( ICorProfilerInfo2* profilerInfo) {
		this->profilerInfo = profilerInfo;
	}
	void ProfilerHelper::GetFunctionSignature( 
		FunctionID functionId,
		UINT32& methodAttributes,
		string& signature
	) {
		ULONG argCount;
		HRESULT hr = E_FAIL;
		argCount = 0;
		if ( functionId != NULL ) {
			mdToken	token;
			ClassID classID;
			ModuleID moduleID;
			IMetaDataImport *metaDataImport = NULL;	
			mdToken moduleToken;		    
			try {
				hr = profilerInfo->GetFunctionInfo(functionId,&classID,&moduleID,&moduleToken );
			}
			catch ( ... ) {
				hr = E_FAIL;
			}
			if (FAILED(hr)) {
				hr = S_OK;
				signature+="FAILED";
			}
			else {
				hr = profilerInfo->GetTokenAndMetaDataFromFunction( 
					functionId, IID_IMetaDataImport, (IUnknown **)&metaDataImport,&token );
				if(SUCCEEDED(hr)) {
					WCHAR functionNameString[ MAX_FUNCTION_LENGTH ];
					hr = metaDataImport->GetMethodProps( token, NULL, functionNameString, MAX_FUNCTION_LENGTH,
						0,0,NULL,NULL,NULL,NULL);
					if(SUCCEEDED(hr)) {
						signature+=CW2A(functionNameString);
						//signature.functionName=CW2A(functionNameString);
						mdTypeDef classToken = NULL;
						hr = profilerInfo->GetClassIDInfo(classID, NULL, &classToken);
						if(SUCCEEDED(hr)) {
			      			if(classToken != mdTypeDefNil) {
								WCHAR classNameString[ MAX_FUNCTION_LENGTH ];
			          			hr = metaDataImport->GetTypeDefProps( 
									classToken, classNameString, MAX_FUNCTION_LENGTH,NULL, NULL, NULL );
								signature+=CW2A(classNameString);
								//signature.className=CW2A(classNameString);
							}
							DWORD methodAttr = 0;
							PCCOR_SIGNATURE sigBlob = NULL;

							hr = metaDataImport->GetMethodProps( 
								(mdMethodDef) token,NULL,NULL,0,NULL,&methodAttr,&sigBlob,NULL,NULL,NULL );
							if(SUCCEEDED(hr)) {
								ULONG callConv;
								methodAttributes = methodAttr;
								//*methodAttributes = methodAttr;
								string buffer;
								sigBlob += CorSigUncompressData( sigBlob, &callConv );
								if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
								{
									static WCHAR* callConvNames[8] = {	
										L"", 
										L"unmanaged cdecl ", 
										L"unmanaged stdcall ",	
										L"unmanaged thiscall ",	
										L"unmanaged fastcall ",	
										L"vararg ",	 
										L"<error> "	 
										L"<error> "	 
									};	
									if ( (callConv & 7) != 0 ) {
										buffer+=CW2A(callConvNames[callConv & 7]);
										buffer+=" ";
									}									
									sigBlob += CorSigUncompressData( sigBlob, &argCount );
									sigBlob = ParseElementType( metaDataImport, sigBlob, buffer);									
									//if ( buffer.length() == 0 ) {
									//	buffer+="void";
									//}
									//signature==signature+buffer;		
									//signature.returnType=signature.returnType+buffer;		
									signature+="(";
									for ( ULONG i = 0; (SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (argCount))); i++ ) {
										sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );									
										if ( i == 0 ) {
											signature+=buffer;
											//signature.parameters=buffer;
										}
										else if ( sigBlob != NULL ) {
											signature+=", "+buffer;
											//signature.parameters+=(", "+buffer);
										}										
										else {
											hr = E_FAIL;
										}
									}
									signature+=")";
								}
								else {
									sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );
									signature+="("+buffer+")";
								}
							} 
						} 
					}
					metaDataImport->Release();
				} 		
			}
		}
		else {
			signature+="UNMANAGED FRAME";
		}
	}
	template<class T> string ToString(T i) {
		string s;
		stringstream stream;
		stream<<i;
		stream>>s;
		return s;
	}
	PCCOR_SIGNATURE ProfilerHelper::ParseElementType( IMetaDataImport *metaDataImport,
		PCCOR_SIGNATURE signature, string& buffer ) {
		map<CorElementType,string> types;
	
		types[ELEMENT_TYPE_VOID]="void";
		types[ELEMENT_TYPE_BOOLEAN]="bool";	
		types[ELEMENT_TYPE_CHAR]="wchar";
		types[ELEMENT_TYPE_I1]="byte";
		types[ELEMENT_TYPE_U1]="ubyte";
		types[ELEMENT_TYPE_I2]="short";
		types[ELEMENT_TYPE_U2]="ushort";
		types[ELEMENT_TYPE_I4]="int";
		types[ELEMENT_TYPE_U4]="uint";	
		types[ELEMENT_TYPE_I8]="long";	
		types[ELEMENT_TYPE_U8]="ulong";	
		types[ELEMENT_TYPE_R4]="float";	
		types[ELEMENT_TYPE_R8]="double";	
		types[ELEMENT_TYPE_U]="uint";	
		types[ELEMENT_TYPE_I]="int";	
		types[ELEMENT_TYPE_OBJECT]="object";	
		types[ELEMENT_TYPE_STRING]="string";	
		types[ELEMENT_TYPE_TYPEDBYREF]="refany";
		CorElementType element=(CorElementType)*signature++;
		if(types.find(element)!=types.end()) {
			buffer+=types[element];
		}
		else {
			switch(element) {
				case ELEMENT_TYPE_CLASS:	
				case ELEMENT_TYPE_VALUETYPE:
				case ELEMENT_TYPE_CMOD_REQD:
				case ELEMENT_TYPE_CMOD_OPT: {	
					mdToken	token;	
					char classname[MAX_FUNCTION_LENGTH];
					signature += CorSigUncompressToken( signature, &token ); 
					if ( TypeFromToken( token ) != mdtTypeRef ) {
						HRESULT	hr;
						WCHAR zName[MAX_FUNCTION_LENGTH];
						hr = metaDataImport->GetTypeDefProps( token, zName, MAX_FUNCTION_LENGTH, NULL, NULL, NULL );
						if ( SUCCEEDED( hr ) ) {
							wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
						}
					}
					char* classOnly=classname+strlen(classname);
					while(classOnly!=classname && (*(classOnly-1))!='.') {
						classOnly--;
					}
					buffer+=classOnly;
					break;	
				}
				case ELEMENT_TYPE_SZARRAY:
					signature = ParseElementType( metaDataImport, signature, buffer ); 
					buffer+="[]";
					break;		
				case ELEMENT_TYPE_ARRAY:	
					{	
						ULONG rank;
						signature = ParseElementType( metaDataImport, signature, buffer );                 
						rank = CorSigUncompressData( signature );													
						if ( rank == 0 ) {
							buffer+="[?]";
						}
						else {
							ULONG *lower;
							ULONG *sizes;
							ULONG numsizes;
							ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
			                                     
							lower = (ULONG *)_alloca( arraysize );
							memset( lower, 0, arraysize );
							sizes = &lower[rank];

							numsizes = CorSigUncompressData( signature );
							if ( numsizes <= rank ) {
        						ULONG numlower;
			                    
								for ( ULONG i = 0; i < numsizes; i++ ) {
									sizes[i] = CorSigUncompressData( signature );
								}
			                    
								numlower = CorSigUncompressData( signature );	
								if ( numlower <= rank ) {
									for (ULONG i = 0; i < numlower; i++) {
										lower[i] = CorSigUncompressData( signature ); 
									}
									buffer+="[";
									for (ULONG i = 0; i < rank; i++ ) {	
										if ( (sizes[i] != 0) && (lower[i] != 0) ) {	
											if ( lower[i] == 0 ) {
												buffer+=ToString(sizes[i]);
											}

											else	
											{	
												buffer+=ToString(lower[i]);	
												buffer+="...";
												if ( sizes[i] != 0 ) {
													buffer+=ToString((lower[i] + sizes[i] + 1) );	
												}
											}	
										}
			                            	
										if ( i < (rank - 1) ) {
											buffer+=",";
										}
									}		                        
									buffer+="]";
								}
							}
						}
					} 
					break;	
				case ELEMENT_TYPE_PINNED:
					signature = ParseElementType( metaDataImport, signature, buffer ); 
					buffer+="pinned";
					break;
				case ELEMENT_TYPE_PTR:   
					signature = ParseElementType( metaDataImport, signature, buffer ); 
					buffer+="*";
					break;
				case ELEMENT_TYPE_BYREF:   
					buffer+="ref ";
					signature = ParseElementType( metaDataImport, signature, buffer ); 
					break;
				default:
				case ELEMENT_TYPE_END:
				case ELEMENT_TYPE_SENTINEL:
					buffer+="<UNKNOWN>";
					break;           	
			}
		}
		return signature;

		//switch ( *signature++ ) {	
		//	case ELEMENT_TYPE_VOID:
		//		buffer+="void";	
		//		break;					
		//	case ELEMENT_TYPE_BOOLEAN:
		//		buffer+="bool";	
		//		break;	
		//	case ELEMENT_TYPE_CHAR:
		//		buffer+="wchar";
		//		break;		
		//	case ELEMENT_TYPE_I1:
		//		buffer+="byte";
		//		break;		
		//	case ELEMENT_TYPE_U1:
		//		buffer+="ubyte";
		//		break;		
		//	case ELEMENT_TYPE_I2:
		//		buffer+="short";
		//		break;		
		//	case ELEMENT_TYPE_U2:
		//		buffer+="ushort";
		//		break;			
		//	case ELEMENT_TYPE_I4:
		//		buffer+="int";
		//		break;
		//	case ELEMENT_TYPE_U4:
		//		buffer+="uint";	
		//		break;		
		//	case ELEMENT_TYPE_I8:
		//		buffer+="long";	
		//		break;		
		//	case ELEMENT_TYPE_U8:
		//		buffer+="ulong";	
		//		break;		
		//	case ELEMENT_TYPE_R4:
		//		buffer+="float";	
		//		break;			
		//	case ELEMENT_TYPE_R8:
		//		buffer+="double";	
		//		break;		
		//	case ELEMENT_TYPE_U:
		//		buffer+="uint";	
		//		break;		 
		//	case ELEMENT_TYPE_I:
		//		buffer+="int";	
		//		break;			  
		//	case ELEMENT_TYPE_OBJECT:
		//		buffer+="object";	
		//		break;		 
		//	case ELEMENT_TYPE_STRING:
		//		buffer+="string";	
		//		break;		 
		//	case ELEMENT_TYPE_TYPEDBYREF:
		//		buffer+="refany";	
		//		break;				       
		//	case ELEMENT_TYPE_CLASS:	
		//	case ELEMENT_TYPE_VALUETYPE:
		//	case ELEMENT_TYPE_CMOD_REQD:
		//	case ELEMENT_TYPE_CMOD_OPT: {	
		//		mdToken	token;	
		//		char classname[MAX_FUNCTION_LENGTH];
  // 				signature += CorSigUncompressToken( signature, &token ); 
		//		if ( TypeFromToken( token ) != mdtTypeRef ) {
  //  				HRESULT	hr;
		//			WCHAR zName[MAX_FUNCTION_LENGTH];
		//			hr = metaDataImport->GetTypeDefProps( token, zName, MAX_FUNCTION_LENGTH, NULL, NULL, NULL );
		//			if ( SUCCEEDED( hr ) ) {
		//				wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
		//			}
		//		}
		//		char* classOnly=classname+strlen(classname);
		//		while(classOnly!=classname && (*(classOnly-1))!='.') {
		//			classOnly--;
		//		}
		//		buffer+=classOnly;
		//		break;	
		//	}
		//	case ELEMENT_TYPE_SZARRAY:
		//		signature = ParseElementType( metaDataImport, signature, buffer ); 
		//		buffer+="[]";
		//		break;		
		//	case ELEMENT_TYPE_ARRAY:	
		//		{	
		//			ULONG rank;
		//			signature = ParseElementType( metaDataImport, signature, buffer );                 
		//			rank = CorSigUncompressData( signature );													
		//			if ( rank == 0 ) {
		//				buffer+="[?]";
		//			}
		//			else {
		//				ULONG *lower;
		//				ULONG *sizes;
		//				ULONG numsizes;
		//				ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
		//                                     
		//				lower = (ULONG *)_alloca( arraysize );
		//				memset( lower, 0, arraysize );
		//				sizes = &lower[rank];

		//				numsizes = CorSigUncompressData( signature );
		//				if ( numsizes <= rank ) {
  //          				ULONG numlower;
		//                    
		//					for ( ULONG i = 0; i < numsizes; i++ ) {
		//						sizes[i] = CorSigUncompressData( signature );
		//					}
		//                    
		//					numlower = CorSigUncompressData( signature );	
		//					if ( numlower <= rank ) {
		//						for (ULONG i = 0; i < numlower; i++) {
		//							lower[i] = CorSigUncompressData( signature ); 
		//						}
		//						buffer+="[";
		//						for (ULONG i = 0; i < rank; i++ ) {	
		//							if ( (sizes[i] != 0) && (lower[i] != 0) ) {	
		//								if ( lower[i] == 0 ) {
		//									buffer+=ToString(sizes[i]);
		//								}

		//								else	
		//								{	
		//									buffer+=ToString(lower[i]);	
		//									buffer+="...";
		//									if ( sizes[i] != 0 ) {
		//										buffer+=ToString((lower[i] + sizes[i] + 1) );	
		//									}
		//								}	
		//							}
		//                            	
		//							if ( i < (rank - 1) ) {
		//								buffer+=",";
		//							}
		//						}		                        
		//						buffer+="]";
		//					}
		//				}
		//			}
		//		} 
		//		break;	
		//	case ELEMENT_TYPE_PINNED:
		//		signature = ParseElementType( metaDataImport, signature, buffer ); 
		//		buffer+="pinned";
		//		break;
		//	case ELEMENT_TYPE_PTR:   
		//		signature = ParseElementType( metaDataImport, signature, buffer ); 
		//		buffer+="*";
		//		break;
		//	case ELEMENT_TYPE_BYREF:   
		//		buffer+="ref ";
		//		signature = ParseElementType( metaDataImport, signature, buffer ); 
		//		break;
		//	default:
		//	case ELEMENT_TYPE_END:
		//	case ELEMENT_TYPE_SENTINEL:
		//		buffer+="<UNKNOWN>";
		//		break;           				            
		//}
		//return signature;
	};
	//PCCOR_SIGNATURE ProfilerHelper::ParseElementType( IMetaDataImport *metaDataImport,
	//	PCCOR_SIGNATURE signature, string& buffer ) {
	//	switch ( *signature++ ) {	
	//		case ELEMENT_TYPE_VOID:
	//			buffer+="void";	
	//			break;					
	//		case ELEMENT_TYPE_BOOLEAN:
	//			buffer+="bool";	
	//			break;	
	//		case ELEMENT_TYPE_CHAR:
	//			buffer+="wchar";
	//			break;		
	//		case ELEMENT_TYPE_I1:
	//			buffer+="byte";
	//			break;		
	//		case ELEMENT_TYPE_U1:
	//			buffer+="ubyte";
	//			break;		
	//		case ELEMENT_TYPE_I2:
	//			buffer+="short";
	//			break;		
	//		case ELEMENT_TYPE_U2:
	//			buffer+="ushort";
	//			break;			
	//		case ELEMENT_TYPE_I4:
	//			buffer+="int";
	//			break;
	//		case ELEMENT_TYPE_U4:
	//			buffer+="uint";	
	//			break;		
	//		case ELEMENT_TYPE_I8:
	//			buffer+="long";	
	//			break;		
	//		case ELEMENT_TYPE_U8:
	//			buffer+="ulong";	
	//			break;		
	//		case ELEMENT_TYPE_R4:
	//			buffer+="float";	
	//			break;			
	//		case ELEMENT_TYPE_R8:
	//			buffer+="double";	
	//			break;		
	//		case ELEMENT_TYPE_U:
	//			buffer+="uint";	
	//			break;		 
	//		case ELEMENT_TYPE_I:
	//			buffer+="int";	
	//			break;			  
	//		case ELEMENT_TYPE_OBJECT:
	//			buffer+="object";	
	//			break;		 
	//		case ELEMENT_TYPE_STRING:
	//			buffer+="string";	
	//			break;		 
	//		case ELEMENT_TYPE_TYPEDBYREF:
	//			buffer+="refany";	
	//			break;				       
	//		case ELEMENT_TYPE_CLASS:	
	//		case ELEMENT_TYPE_VALUETYPE:
	//		case ELEMENT_TYPE_CMOD_REQD:
	//		case ELEMENT_TYPE_CMOD_OPT: {	
	//			mdToken	token;	
	//			char classname[MAX_FUNCTION_LENGTH];
 //  				signature += CorSigUncompressToken( signature, &token ); 
	//			if ( TypeFromToken( token ) != mdtTypeRef ) {
 //   				HRESULT	hr;
	//				WCHAR zName[MAX_FUNCTION_LENGTH];
	//				hr = metaDataImport->GetTypeDefProps( token, zName, MAX_FUNCTION_LENGTH, NULL, NULL, NULL );
	//				if ( SUCCEEDED( hr ) ) {
	//					wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
	//				}
	//			}
	//			char* classOnly=classname+strlen(classname);
	//			while(classOnly!=classname && (*(classOnly-1))!='.') {
	//				classOnly--;
	//			}
	//			buffer+=classOnly;
	//			break;	
	//		}
	//		case ELEMENT_TYPE_SZARRAY:
	//			signature = ParseElementType( metaDataImport, signature, buffer ); 
	//			buffer+="[]";
	//			break;		
	//		case ELEMENT_TYPE_ARRAY:	
	//			{	
	//				ULONG rank;
	//				signature = ParseElementType( metaDataImport, signature, buffer );                 
	//				rank = CorSigUncompressData( signature );													
	//				if ( rank == 0 ) {
	//					buffer+="[?]";
	//				}
	//				else {
	//					ULONG *lower;
	//					ULONG *sizes;
	//					ULONG numsizes;
	//					ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
	//	                                     
	//					lower = (ULONG *)_alloca( arraysize );
	//					memset( lower, 0, arraysize );
	//					sizes = &lower[rank];

	//					numsizes = CorSigUncompressData( signature );
	//					if ( numsizes <= rank ) {
 //           				ULONG numlower;
	//	                    
	//						for ( ULONG i = 0; i < numsizes; i++ ) {
	//							sizes[i] = CorSigUncompressData( signature );
	//						}
	//	                    
	//						numlower = CorSigUncompressData( signature );	
	//						if ( numlower <= rank ) {
	//							for (ULONG i = 0; i < numlower; i++) {
	//								lower[i] = CorSigUncompressData( signature ); 
	//							}
	//							buffer+="[";
	//							for (ULONG i = 0; i < rank; i++ ) {	
	//								if ( (sizes[i] != 0) && (lower[i] != 0) ) {	
	//									if ( lower[i] == 0 ) {
	//										buffer+=ToString(sizes[i]);
	//									}

	//									else	
	//									{	
	//										buffer+=ToString(lower[i]);	
	//										buffer+="...";
	//										if ( sizes[i] != 0 ) {
	//											buffer+=ToString((lower[i] + sizes[i] + 1) );	
	//										}
	//									}	
	//								}
	//	                            	
	//								if ( i < (rank - 1) ) {
	//									buffer+=",";
	//								}
	//							}		                        
	//							buffer+="]";
	//						}
	//					}
	//				}
	//			} 
	//			break;	
	//		case ELEMENT_TYPE_PINNED:
	//			signature = ParseElementType( metaDataImport, signature, buffer ); 
	//			buffer+="pinned";
	//			break;
	//		case ELEMENT_TYPE_PTR:   
	//			signature = ParseElementType( metaDataImport, signature, buffer ); 
	//			buffer+="*";
	//			break;
	//		case ELEMENT_TYPE_BYREF:   
	//			buffer+="ref ";
	//			signature = ParseElementType( metaDataImport, signature, buffer ); 
	//			break;
	//		default:
	//		case ELEMENT_TYPE_END:
	//		case ELEMENT_TYPE_SENTINEL:
	//			buffer+="<UNKNOWN>";
	//			break;           				            
	//	}
	//	return signature;
	//}
	CComPtr< ICorProfilerInfo2 > profilerInfo;
};

HRESULT __stdcall __stdcall StackWalker( 
	FunctionID funcId,
	UINT_PTR ip,
	COR_PRF_FRAME_INFO frameInfo,
	ULONG32 contextSize,
	BYTE context[  ],
	void *clientData) {
	if(funcId!=0) {
		((vector<FunctionID>*)clientData)->push_back(funcId);
	}
	return S_OK;
}
#define guid "029C3A01-70C1-46D2-92B7-24B157DF55CE"

const int interval=5;

class Profiler
{
public: 
	vector<vector<FunctionID>*> stackWalks;
	map< FunctionID, FunctionID> signatures;
	ofstream* file;
	string GetTemporaryFileName() {
		char path[MAX_PATH];
		memset(path,0,sizeof(path));
		GetTempPath(MAX_PATH-1,path);
		string temp(path);
		return temp+guid+".nprof";
	}
	void EndAll(ProfilerHelper& profilerHelper) {
		file=new ofstream(GetTemporaryFileName().c_str(), ios::binary);
		for(map< FunctionID, FunctionID >::iterator i = signatures.begin(); i != signatures.end(); i++ ) {
			FunctionID id=i->second;
			WriteUINT32(id);

			UINT32 methodAttributes;
			string signature;
			profilerHelper.GetFunctionSignature(id, methodAttributes, signature);
			WriteUINT32(methodAttributes);
			WriteString(signature);
		}
		WriteUINT32(-1);

		for(vector<vector<FunctionID>*>::iterator stackWalk = stackWalks.begin(); stackWalk != stackWalks.end(); stackWalk++ ) {
			for(vector<FunctionID>::iterator stackFrame=(*stackWalk)->begin();stackFrame!=(*stackWalk)->end();stackFrame++) {
				WriteUINT32(*stackFrame);
			}
			WriteUINT32(-1);
		}
		WriteUINT32(-1);
		WriteUINT32(-2);
		file->close();
		delete file;
	}
	void WriteString(const string& signature) {
		WriteUINT32(( UINT32 )signature.length());
		file->write(signature.c_str(),(int)signature.length());
	}
	void WriteUINT32(FunctionID id) {
		file->write((char*)&id,sizeof(FunctionID));
	}
	Profiler::Profiler( ICorProfilerInfo2* profilerInfo ) {
		this->profilerInfo = profilerInfo;
		this->profilerHelper.Initialize( profilerInfo );
		profilerInfo->SetEventMask(COR_PRF_ENABLE_STACK_SNAPSHOT|COR_PRF_MONITOR_THREADS|
			COR_PRF_DISABLE_INLINING|COR_PRF_MONITOR_SUSPENDS|COR_PRF_MONITOR_EXCEPTIONS|
			COR_PRF_MONITOR_APPDOMAIN_LOADS|COR_PRF_MONITOR_ASSEMBLY_LOADS|
			COR_PRF_MONITOR_CACHE_SEARCHES|COR_PRF_MONITOR_JIT_COMPILATION);
		SetTimer();
	}
	void KillTimer() {
		timeKillEvent(timer);
	}
	void SetTimer() {
		TIMECAPS timeCaps;
		timeGetDevCaps(&timeCaps, sizeof(TIMECAPS));
		timer = timeSetEvent(interval,timeCaps.wPeriodMin,TimerFunction,(DWORD_PTR)this,TIME_PERIODIC);
	}
	void ThreadMap( ThreadID threadId, DWORD dwOSThread ) {
		threadMap[ dwOSThread ] = threadId;
	}
	virtual void End() {
		KillTimer();
		EndAll( profilerHelper );
	}
protected:
	CComPtr< ICorProfilerInfo2 > profilerInfo;
	ProfilerHelper profilerHelper;
	map< DWORD, ThreadID > threadMap;
	bool statistical;
public:
	static void CALLBACK TimerFunction(UINT wTimerID, UINT msg, DWORD dwUser, DWORD dw1, DWORD dw2) {
		Profiler* profiler=(Profiler*)dwUser;
		profiler->WalkStack();
	}
	static UINT timer;
	void WalkStack() {
		KillTimer();
		for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++) {
			DWORD threadId=(*i).first;
			HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
			if(threadHandle!=NULL) {
				int suspendCount=SuspendThread(threadHandle);
				vector<FunctionID>* functions=new vector<FunctionID>();
				ThreadID id=i->second;

				CONTEXT context;
				context.ContextFlags=CONTEXT_FULL;
				GetThreadContext(threadHandle,&context);
				FunctionID newID;
				HRESULT result=profilerInfo->GetFunctionFromIP((BYTE*)context.Eip,&newID);

				bool found=false;
				if(result!=S_OK || newID!=0) {
					STACKFRAME64 stackFrame;
					memset(&stackFrame, 0, sizeof(stackFrame));
					stackFrame.AddrPC.Offset = context.Eip;
					stackFrame.AddrPC.Mode = AddrModeFlat;
					stackFrame.AddrFrame.Offset = context.Ebp;
					stackFrame.AddrFrame.Mode = AddrModeFlat;
					stackFrame.AddrStack.Offset = context.Esp;
					stackFrame.AddrStack.Mode = AddrModeFlat;
					while(true) {
						if(!StackWalk64(IMAGE_FILE_MACHINE_I386,GetCurrentProcess(),threadHandle,
							 &stackFrame,NULL,NULL,SymFunctionTableAccess64,SymGetModuleBase64,NULL)) {
							break;
						}
						if (stackFrame.AddrPC.Offset == stackFrame.AddrReturn.Offset) {
							break;
						}

						FunctionID id;
						HRESULT result=profilerInfo->GetFunctionFromIP((BYTE*)stackFrame.AddrPC.Offset,&id);
						if(result==S_OK && id!=0) {
							memset(&context,0,sizeof(context));
							context.Eip=stackFrame.AddrPC.Offset;
							context.Ebp=stackFrame.AddrFrame.Offset;
							context.Esp=stackFrame.AddrStack.Offset;
							found=true;
							break;
						}
					}
				}
				else {
					found=true;
				}
				if(found) {
					profilerInfo->DoStackSnapshot(id,StackWalker,COR_PRF_SNAPSHOT_DEFAULT,functions,
						(BYTE*)&context,sizeof(context));

					for(int index=0;index<functions->size();index++) {
						FunctionID id=functions->at(index);
						map<FunctionID,FunctionID>::iterator found = signatures.find(id);
						if(found == signatures.end()){
							signatures.insert(make_pair(id,id));
						}
					}
				}
				stackWalks.push_back(functions);
				ResumeThread(threadHandle);
			}
		}
		SetTimer();
	}
};
UINT Profiler::timer;
[
	object,
	uuid("FDEDE932-9F80-4CE5-891E-3B24768CFBCB"),
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
  uuid(guid),
  helpstring("nprof COR Profiling Hook Class")
]
class ATL_NO_VTABLE CNProfCORHook : public INProfCORHook,public ICorProfilerCallback2{
public:
  CNProfCORHook() {
    this->profiler = NULL;
  }
  DECLARE_PROTECT_FINAL_CONSTRUCT()
  HRESULT FinalConstruct() {
    return S_OK;
  }
  void FinalRelease() {
	  if ( profiler ) {
		delete profiler;
	  }
  }
public:
	static Profiler* profiler;
	CRITICAL_SECTION criticalSection;
public:
	static Profiler* GetProfiler() {
		return profiler;
	}
	STDMETHOD(Initialize)(LPUNKNOWN pICorProfilerInfoUnk) {
		CComQIPtr< ICorProfilerInfo2 > profilerInfo = pICorProfilerInfoUnk;
		InitializeCriticalSection(&criticalSection);
		profiler = new Profiler( profilerInfo );
		return S_OK;
	}
	STDMETHOD(Shutdown)() {
		EnterCriticalSection(&criticalSection);
		profiler->End();
		delete profiler;
		profiler = NULL;
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(AppDomainCreationStarted)(AppDomainID appDomainId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainCreationFinished)(AppDomainID appDomainId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainShutdownStarted)(AppDomainID appDomainId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainShutdownFinished)(AppDomainID appDomainId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadStarted)(AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadFinished)(AssemblyID assemblyId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadStarted)(AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadFinished)(AssemblyID assemblyId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadStarted)(ModuleID moduleId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadFinished)(ModuleID moduleId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadStarted)(ModuleID moduleId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadFinished)(ModuleID moduleId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleAttachedToAssembly)(ModuleID moduleId, AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadStarted)(ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadFinished)(ClassID classId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadStarted)(ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadFinished)(ClassID classId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(FunctionUnloadStarted)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationStarted)(FunctionID functionId, BOOL fIsSafeToBlock) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationFinished)(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchStarted)(FunctionID functionId, BOOL* pbUseCachedFunction) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchFinished)(FunctionID functionId, COR_PRF_JIT_CACHE result) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITFunctionPitched)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITInlining)(FunctionID callerId, FunctionID calleeId, BOOL* pfShouldInline) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadCreated)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadDestroyed)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadId, DWORD osThreadId) {
		EnterCriticalSection(&criticalSection);
		profiler->ThreadMap( managedThreadId, osThreadId );
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(RemotingClientInvocationStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientSendingMessage)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientReceivingReply)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientInvocationFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerReceivingMessage)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationReturned)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerSendingReply)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(UnmanagedToManagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(ManagedToUnmanagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendAborted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeResumeStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeResumeFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadSuspended)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadResumed)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(MovedReferences)(unsigned long cMovedObjectIDRanges, ObjectID* oldObjectIDRangeStart, ObjectID* newObjectIDRangeStart, unsigned long * cObjectIDRangeLength) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectAllocated)(ObjectID objectId, ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectsAllocatedByClass)(unsigned long cClassCount, ClassID* classIds, unsigned long* cObjects) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectReferences)(ObjectID objectId, ClassID classId, unsigned long cObjectRefs, ObjectID* objectRefIds) {
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences)(unsigned long cRootRefs, ObjectID* rootRefIds) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionThrown)(ThreadID thrownObjectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerEnter)(UINT_PTR __unused) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerLeave)(UINT_PTR __unused) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFinallyLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherEnter)(FunctionID functionId, ObjectID objectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableCreated)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable, unsigned long cSlots) {
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableDestroyed)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherFound)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherExecute)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadNameChanged)(ThreadID threadId, ULONG cchName, WCHAR name[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionStarted)(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(SurvivingReferences) (ULONG cSurvivingObjectIDRanges,ObjectID objectIDRangeStart[],ULONG cObjectIDRangeLength[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(FinalizeableObjectQueued)(DWORD finalizerFlags,ObjectID objectID) {
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[],
			  COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(HandleCreated)(GCHandleID handleId,ObjectID initialObjectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(HandleDestroyed)(GCHandleID handleId) {
		return E_NOTIMPL;
	}
};
[ 
	module(dll, uuid = "{A461E20A-C7DC-4A89-A24E-87B5E975A96B}", 
	name = "NProfHook", 
	helpstring = "NProf.Hook 1.0 Type Library",
	resource_name = "IDR_NPROFHOOK")
];
Profiler* CNProfCORHook::profiler;
