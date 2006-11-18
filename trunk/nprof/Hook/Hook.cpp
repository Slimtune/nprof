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
#include <iostream>

using namespace ATL;
using namespace std;

const int MAX_FUNCTION_LENGTH=2048;
#define guid "107F578A-E019-4BAF-86A1-7128A749DB05"

class ProfilerHelper
{
public:
	void Initialize(ICorProfilerInfo2* profilerInfo) {
		this->profilerInfo = profilerInfo;
	}
	string GetClass(string className) {
		int index=className.find_first_of(".");
		if(index!=string.npos) {
			return className.substr();
		}
		else {
			return "";
		}
	}
	void GetFunctionSignature(FunctionID functionId,UINT32& methodAttributes,string& text) {
		ClassID classID;
		ModuleID moduleID;
		mdToken moduleToken;
		if(FAILED(profilerInfo->GetFunctionInfo(functionId,&classID,&moduleID,&moduleToken))) {
			DebugBreak();
		}
		IMetaDataImport *metaDataImport = 0;	
		mdToken	token;
		if(FAILED(profilerInfo->GetTokenAndMetaDataFromFunction(functionId, IID_IMetaDataImport, (IUnknown **)&metaDataImport,&token))) {
			DebugBreak();
		}
		WCHAR functionNameString[MAX_FUNCTION_LENGTH];
		if(FAILED(metaDataImport->GetMethodProps(token, 0, functionNameString, MAX_FUNCTION_LENGTH,0,0,0,0,0,0))) {
			DebugBreak();
		}
		mdTypeDef classToken = 0;
		profilerInfo->GetClassIDInfo(classID, 0, &classToken);
		if(classToken != mdTypeDefNil) {
			WCHAR classNameString[MAX_FUNCTION_LENGTH];
			if(FAILED(metaDataImport->GetTypeDefProps(classToken, classNameString, MAX_FUNCTION_LENGTH,0, 0, 0))) {
				DebugBreak();
			}
			string s=CW2A(classNameString);
			text+=GetClass(s);
			text+=".";
			text+=CW2A(functionNameString);
		}
		DWORD methodAttr = 0;
		PCCOR_SIGNATURE sigBlob = NULL;
		//HRESULT hr=metaDataImport->GetMethodProps((mdMethodDef)token,0,0,0,0,&methodAttr,&sigBlob,0,0,0);
		
		if(FAILED(metaDataImport->GetMethodProps( (mdMethodDef) token,
								NULL,
								NULL,
								0,
								NULL,
								&methodAttr,
								&sigBlob,
								NULL,
								NULL,
								NULL ))) {
			DebugBreak();
		}
		ULONG callConv;
		methodAttributes = methodAttr;
		sigBlob += CorSigUncompressData(sigBlob, &callConv);
		text+="(";
		ULONG argCount=0;
		sigBlob += CorSigUncompressData(sigBlob, &argCount);
		string returnType;
		ParseElementType(metaDataImport, &sigBlob, returnType);
		for(ULONG i = 0; (sigBlob != 0) && (i < (argCount)); i++) {
			string buffer;
			ParseElementType(metaDataImport,&sigBlob,buffer);									
			if(i!=0){
				text+=", ";
			}
			text+=buffer;
		}
		text+=")";
		metaDataImport->Release();
	}
	template<class T> string ToString(T i) {
		string s;
		stringstream stream;
		stream<<i;
		stream>>s;
		return s;
	}
	void ParseElementType(IMetaDataImport *metaDataImport,PCCOR_SIGNATURE* signature,string& text) {
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
		(*signature)++;
		CorElementType element=(CorElementType)(**signature);
		if(types.find(element)!=types.end()) {
			text+=types[element];
		}
		else {
			switch(element) {
				case ELEMENT_TYPE_CLASS:	
				case ELEMENT_TYPE_VALUETYPE:
				case ELEMENT_TYPE_CMOD_REQD:
				case ELEMENT_TYPE_CMOD_OPT: 
				{
					mdToken	token;	
					string className;//[MAX_FUNCTION_LENGTH];
					(*signature) += CorSigUncompressToken((PCCOR_SIGNATURE)(*signature),&token);
					if ( TypeFromToken( token ) != mdtTypeRef ) {
						WCHAR zName[MAX_FUNCTION_LENGTH];
						metaDataImport->GetTypeDefProps(token,zName,MAX_FUNCTION_LENGTH,0,0,0);
						className=CW2A(zName);//,MAX_FUNCTION_LENGTH);
					}
					text+=GetClass(className);
					break;	
				}
				case ELEMENT_TYPE_SZARRAY:
					ParseElementType(metaDataImport,signature,text); 
					text+="[]";
					break;		
				case ELEMENT_TYPE_ARRAY: {	
					ULONG rank;
					ParseElementType(metaDataImport,signature,text);
					rank = CorSigUncompressData((PCCOR_SIGNATURE&)*signature);
					if ( rank == 0 ) {
						text+="[?]";
					}
					else {
						ULONG *lower;
						ULONG *sizes;
						ULONG numsizes;
						ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
		                
						lower = (ULONG *)_alloca( arraysize );
						memset( lower, 0, arraysize );
						sizes = &lower[rank];

						numsizes = CorSigUncompressData((PCCOR_SIGNATURE&)*signature);
						if ( numsizes <= rank ) {
    						ULONG numlower;			                    
							for ( ULONG i = 0; i < numsizes; i++ ) {
								sizes[i] = CorSigUncompressData((PCCOR_SIGNATURE&)*signature);
							}			                    
							numlower = CorSigUncompressData((PCCOR_SIGNATURE&)*signature);	
							if ( numlower <= rank ) {
								for (ULONG i = 0; i < numlower; i++) {
									lower[i] = CorSigUncompressData((PCCOR_SIGNATURE&)*signature); 
								}
								text+="[";
								for (ULONG i = 0; i < rank; i++ ) {	
									if ( (sizes[i] != 0) && (lower[i] != 0) ) {	
										if ( lower[i] == 0 ) {
											text+=ToString(sizes[i]);
										}
										else {
											text+=ToString(lower[i]);	
											text+="...";
											if ( sizes[i] != 0 ) {
												text+=ToString((lower[i] + sizes[i] + 1) );	
											}
										}	
									}			                            	
									if ( i < (rank - 1) ) {
										text+=",";
									}
								}		                        
								text+="]";
							}
						}
					}
					break;
				}
				case ELEMENT_TYPE_PINNED:
					ParseElementType(metaDataImport,signature,text); 
					text+="pinned";
					break;
				case ELEMENT_TYPE_PTR:   
					ParseElementType(metaDataImport,signature,text); 
					text+="*";
					break;
				case ELEMENT_TYPE_BYREF:   
					text+="ref ";
					ParseElementType(metaDataImport,signature,text); 
					break;
				default:
				case ELEMENT_TYPE_END:
				case ELEMENT_TYPE_SENTINEL:
					text+="<UNKNOWN>";
					break;           	
			}
		}
	}
	CComPtr< ICorProfilerInfo2 > profilerInfo;
};
HRESULT __stdcall __stdcall StackWalker(FunctionID funcId,UINT_PTR ip,COR_PRF_FRAME_INFO frameInfo,
	ULONG32 contextSize,BYTE context[  ],void *clientData
) {
	if(funcId!=0) {
		((vector<FunctionID>*)clientData)->push_back(funcId);
	}
	return S_OK;
}
const int interval=2;
class Profiler {
public: 
	vector<vector<FunctionID>*> stackWalks;
	map< FunctionID, FunctionID> signatures;
	ofstream* file;
	string GetTemporaryFileName() {
		char path[MAX_PATH];
		path[0]=0;
		GetTempPath(MAX_PATH-1,path);
		return (string)path+guid+".nprof";
	}
	void EndAll(ProfilerHelper& profilerHelper) {
		file=new ofstream(GetTemporaryFileName().c_str(), ios::binary);
		for(map<FunctionID,FunctionID>::iterator i=signatures.begin();i!=signatures.end();i++) {
			FunctionID id=i->second;
			WriteInteger(id);
			UINT32 methodAttributes;
			string signature;
			profilerHelper.GetFunctionSignature(id, methodAttributes, signature);
			WriteInteger(methodAttributes);
			WriteString(signature);
		}
		WriteInteger(-1);
		for(vector<vector<FunctionID>*>::iterator stackWalk = stackWalks.begin(); stackWalk != stackWalks.end(); stackWalk++ ) {
			for(vector<FunctionID>::iterator stackFrame=(*stackWalk)->begin();stackFrame!=(*stackWalk)->end();stackFrame++) {
				WriteInteger(*stackFrame);
			}
			WriteInteger(-1);
		}
		WriteInteger(-1);
		WriteInteger(-2);
		file->close();
		delete file;
	}
	void WriteString(const string& signature) {
		WriteInteger((UINT32)signature.length());
		file->write(signature.c_str(),(int)signature.length());
	}
	void WriteInteger(FunctionID id) {
		file->write((char*)&id,sizeof(FunctionID));
	}
	Profiler::Profiler( ICorProfilerInfo2* profilerInfo ) {
		this->profilerInfo = profilerInfo;
		this->profilerHelper.Initialize(profilerInfo);
		profilerInfo->SetEventMask(COR_PRF_ENABLE_STACK_SNAPSHOT|COR_PRF_MONITOR_THREADS|COR_PRF_DISABLE_INLINING);
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
	void ThreadMap(ThreadID threadId, DWORD dwOSThread) {
		threadMap[dwOSThread] = threadId;
	}
	virtual void End() {
		KillTimer();
		EndAll(profilerHelper);
	}
	static void CALLBACK TimerFunction(UINT wTimerID, UINT msg, DWORD dwUser, DWORD dw1, DWORD dw2) {
		Profiler* profiler=(Profiler*)dwUser;
		__int64 totalTime=(__int64)0;
		for(map<DWORD,ThreadID>::iterator i=profiler->threadMap.begin();i!=profiler->threadMap.end();i++) {
			HANDLE thread=OpenThread(THREAD_QUERY_INFORMATION,false,i->first);
			FILETIME creationTime;
			FILETIME exitTime;
			FILETIME kernelTime;
			FILETIME userTime;
			GetThreadTimes(
				thread,
				&creationTime,
				&exitTime,
				&kernelTime,
				&userTime
			);
			__int64 l;
			memcpy(&l,&userTime,sizeof(l));
			totalTime+=l;
			__int64 m;
			memcpy(&m,&userTime,sizeof(m));
			totalTime+=m;
			CloseHandle(thread);
		}
		if(totalTime-lastTime>50) {
			lastTime=totalTime;
			profiler->WalkStack();
		}
	}
	static __int64 lastTime;
	static UINT timer;
	void WalkStack() {
		KillTimer();
		for(map<DWORD,ThreadID>::iterator i=threadMap.begin();i!=threadMap.end();i++) {
			DWORD threadId=(*i).first;
			HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
			int suspendCount=SuspendThread(threadHandle);
			vector<FunctionID>* functions=new vector<FunctionID>();
			ThreadID id=i->second;

			CONTEXT context;
			context.ContextFlags=CONTEXT_FULL;
			GetThreadContext(threadHandle,&context);
			FunctionID newID;

			bool found;
			if(profilerInfo->GetFunctionFromIP((BYTE*)context.Eip,&newID)!=S_OK && newID!=0) {
				found=true;
			}
			else {
				found=false;
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
						 &stackFrame,0,0,SymFunctionTableAccess64,SymGetModuleBase64,0)) {
						break;
					}
					if (stackFrame.AddrPC.Offset == stackFrame.AddrReturn.Offset) {
						break;
					}
					FunctionID id;
					if(profilerInfo->GetFunctionFromIP((BYTE*)stackFrame.AddrPC.Offset,&id)==S_OK && id!=0) {
						memset(&context,0,sizeof(context));
						context.Eip=stackFrame.AddrPC.Offset;
						context.Ebp=stackFrame.AddrFrame.Offset;
						context.Esp=stackFrame.AddrStack.Offset;
						found=true;
						break;
					}
				}
			}
			if(found) {
				profilerInfo->DoStackSnapshot(
					id,StackWalker,COR_PRF_SNAPSHOT_DEFAULT,functions,(BYTE*)&context,sizeof(context));

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
			CloseHandle(threadHandle);
		}
		SetTimer();
	}
	map< DWORD, ThreadID > threadMap;
protected:
	CComPtr< ICorProfilerInfo2 > profilerInfo;
	ProfilerHelper profilerHelper;
	bool statistical;
};
UINT Profiler::timer;
__int64 Profiler::lastTime;
[
	object,
	uuid("FDEDE932-9F80-4CE5-891E-3B24768CFBCB"),
	dual,helpstring("INProfCORHook Interface"),
	pointer_default(unique)
]
__interface INProfCORHook : IDispatch {
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
class ATL_NO_VTABLE CNProfCORHook : public INProfCORHook,public ICorProfilerCallback2 {
public:
	CNProfCORHook() {
		this->profiler = 0;
	}
	DECLARE_PROTECT_FINAL_CONSTRUCT()
	HRESULT FinalConstruct() {
		return S_OK;
	}
	void FinalRelease() {
		delete profiler;
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
		profiler = 0;
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
		profiler->ThreadMap(managedThreadId, osThreadId);
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
	STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[],COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]) {
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