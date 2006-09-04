//// NProfCORHook.h : Declaration of the CNProfCORHook
//
//#pragma once
//#include "resource.h"       // main symbols
//#include "profiler_socket.h"
//#include "profiler.h"
//
//// Pre-define these methods
//void RawEnter();
//void RawLeave();
//void RawTailCall();
//
//// INProfCORHook
//[
//	object,
//	uuid("5B94DF43-780B-42FD-AC4C-ABAB35D4A274"),
//	dual,	helpstring("INProfCORHook Interface"),
//	pointer_default(unique)
//]
//__interface INProfCORHook : IDispatch
//{
//};
//
//
//// CNProfCORHook
//
//[
//  coclass,
//  threading("apartment"),
//  vi_progid("NProf.NProfCORHook"),
//  progid("NProf.NProfCORHook.1"),
//  version(1.0),
//  uuid("791DA9FE-05A0-495E-94BF-9AD875C4DF0F"),
//  helpstring("nprof COR Profiling Hook Class")
//]
//class ATL_NO_VTABLE CNProfCORHook : 
//  public INProfCORHook,
//  public ICorProfilerCallback2
//{
//public:
//  CNProfCORHook()
//  {
//    this->profiler = NULL;
//  }
//
//  DECLARE_PROTECT_FINAL_CONSTRUCT()
//
//  HRESULT FinalConstruct()
//  {
//    return S_OK;
//  }
//
//  void FinalRelease() 
//  {
//    if ( profiler )
//      delete profiler;
//  }
//
//public:
//  static Profiler* profiler;
//
//  // ICorProfilerCallback Methods
//public:
//  static Profiler* GetProfiler()
//  {
//    return profiler;
//  }
//
//  STDMETHOD(Initialize)(LPUNKNOWN pICorProfilerInfoUnk)
//  {
//    //DebugBreak();
//    CComQIPtr< ICorProfilerInfo > profilerInfo = pICorProfilerInfoUnk;
//
//    ProfilerSocket::Initialize();
//
//    cout << "Initializing profiler hook DLL..." << endl;
//
//    if ( profilerInfo )
//    {
//      profiler = new Profiler( profilerInfo );
//      cout << "Initializing event masks..." << endl;
//      profilerInfo->SetEventMask( 
//        COR_PRF_MONITOR_THREADS	|
//        COR_PRF_DISABLE_INLINING |
//        COR_PRF_MONITOR_SUSPENDS |    
//        COR_PRF_MONITOR_ENTERLEAVE |
//        COR_PRF_MONITOR_EXCEPTIONS |  
//        COR_PRF_MONITOR_APPDOMAIN_LOADS |
//        COR_PRF_MONITOR_ASSEMBLY_LOADS |
//		COR_PRF_MONITOR_CACHE_SEARCHES |
//		COR_PRF_MONITOR_JIT_COMPILATION | 
//        COR_PRF_MONITOR_CODE_TRANSITIONS
//      );
//
//      cout << "Initializing hooks..." << endl;
//      profilerInfo->SetEnterLeaveFunctionHooks( ( FunctionEnter* )&RawEnter, ( FunctionLeave* )&RawLeave, ( FunctionTailcall* )&RawTailCall );
//      cout << "Ready!" << endl;
//    }
//
//    return S_OK;
//  }
//  STDMETHOD(Shutdown)()
//  {
//    cout << "Terminating profiler..." << endl;
//    profiler->End();
//    delete profiler;
//    profiler = NULL;
//    ProfilerSocket profilerSocket;
//    profilerSocket.SendShutdown();
//
//    return S_OK;
//  }
//  STDMETHOD(AppDomainCreationStarted)(AppDomainID appDomainId)
//  {
//    profiler->AppDomainStart( appDomainId );
//    return S_OK;
//  }
//  STDMETHOD(AppDomainCreationFinished)(AppDomainID appDomainId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(AppDomainShutdownStarted)(AppDomainID appDomainId)
//  {
//    profiler->AppDomainEnd( appDomainId );
//    return S_OK;
//  }
//  STDMETHOD(AppDomainShutdownFinished)(AppDomainID appDomainId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(AssemblyLoadStarted)(AssemblyID assemblyId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(AssemblyLoadFinished)(AssemblyID assemblyId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(AssemblyUnloadStarted)(AssemblyID assemblyId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(AssemblyUnloadFinished)(AssemblyID assemblyId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ModuleLoadStarted)(ModuleID moduleId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ModuleLoadFinished)(ModuleID moduleId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ModuleUnloadStarted)(ModuleID moduleId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ModuleUnloadFinished)(ModuleID moduleId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ModuleAttachedToAssembly)(ModuleID moduleId, AssemblyID assemblyId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ClassLoadStarted)(ClassID classId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ClassLoadFinished)(ClassID classId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ClassUnloadStarted)(ClassID classId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ClassUnloadFinished)(ClassID classId, HRESULT hrStatus)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(FunctionUnloadStarted)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITCompilationStarted)(FunctionID functionId, BOOL fIsSafeToBlock)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITCompilationFinished)(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITCachedFunctionSearchStarted)(FunctionID functionId, BOOL* pbUseCachedFunction)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITCachedFunctionSearchFinished)(FunctionID functionId, COR_PRF_JIT_CACHE result)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITFunctionPitched)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(JITInlining)(FunctionID callerId, FunctionID calleeId, BOOL* pfShouldInline)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ThreadCreated)(ThreadID threadId)
//  {
//    profiler->ThreadStart( threadId );
//    return S_OK;
//  }
//  STDMETHOD(ThreadDestroyed)(ThreadID threadId)
//  {
//    profiler->ThreadEnd( threadId );
//    return S_OK;
//  }
//  STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadId, DWORD osThreadId)
//  {
//    profiler->ThreadMap( managedThreadId, osThreadId );
//    return S_OK;
//  }
//  STDMETHOD(RemotingClientInvocationStarted)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingClientSendingMessage)(GUID * pCookie, BOOL fIsAsync)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingClientReceivingReply)(GUID * pCookie, BOOL fIsAsync)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingClientInvocationFinished)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingServerReceivingMessage)(GUID * pCookie, BOOL fIsAsync)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingServerInvocationStarted)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingServerInvocationReturned)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RemotingServerSendingReply)(GUID * pCookie, BOOL fIsAsync)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(UnmanagedToManagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
//  {
//    // Only track returns
//    if ( reason == COR_PRF_TRANSITION_RETURN )
//      profiler->UnmanagedToManagedCall( functionId );
//    return S_OK;
//  }
//  STDMETHOD(ManagedToUnmanagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
//  {
//    // Only track calls
//    if ( reason == COR_PRF_TRANSITION_CALL )
//      profiler->ManagedToUnmanagedCall( functionId );
//    return S_OK;
//  }
//  STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason)
//  {
//    profiler->ThreadSuspend();
//    return S_OK;
//  }
//  STDMETHOD(RuntimeSuspendFinished)()
//  {
//    return S_OK;
//  }
//  STDMETHOD(RuntimeSuspendAborted)()
//  {
//    return S_OK;
//  }
//  STDMETHOD(RuntimeResumeStarted)()
//  {
//    profiler->ThreadResume();
//    return S_OK;
//  }
//  STDMETHOD(RuntimeResumeFinished)()
//  {
//    return S_OK;
//  }
//  STDMETHOD(RuntimeThreadSuspended)(ThreadID threadId)
//  {
//    return S_OK;
//  }
//  STDMETHOD(RuntimeThreadResumed)(ThreadID threadId)
//  {
//    return S_OK;
//  }
//  STDMETHOD(MovedReferences)(unsigned long cMovedObjectIDRanges, ObjectID* oldObjectIDRangeStart, ObjectID* newObjectIDRangeStart, unsigned long * cObjectIDRangeLength)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ObjectAllocated)(ObjectID objectId, ClassID classId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ObjectsAllocatedByClass)(unsigned long cClassCount, ClassID* classIds, unsigned long* cObjects)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ObjectReferences)(ObjectID objectId, ClassID classId, unsigned long cObjectRefs, ObjectID* objectRefIds)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RootReferences)(unsigned long cRootRefs, ObjectID* rootRefIds)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionThrown)(ThreadID thrownObjectId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionSearchFunctionLeave)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionSearchFilterLeave)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionOSHandlerEnter)(UINT_PTR __unused)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionOSHandlerLeave)(UINT_PTR __unused)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionUnwindFunctionLeave)()
//  {
//    // Update the call stack as we leave
//    profiler->Leave( 0 );
//
//    return S_OK;
//  }
//  STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionUnwindFinallyLeave)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionCatcherEnter)(FunctionID functionId, ObjectID objectId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionCatcherLeave)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(COMClassicVTableCreated)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable, unsigned long cSlots)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(COMClassicVTableDestroyed)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionCLRCatcherFound)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ExceptionCLRCatcherExecute)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(ThreadNameChanged)(ThreadID threadId, ULONG cchName, WCHAR name[])
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(GarbageCollectionStarted)(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(SurvivingReferences) (ULONG cSurvivingObjectIDRanges,ObjectID objectIDRangeStart[],ULONG cObjectIDRangeLength[])
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(GarbageCollectionFinished)()
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(FinalizeableObjectQueued)(DWORD finalizerFlags,ObjectID objectID)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[],
//              COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[])
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(HandleCreated)(GCHandleID handleId,ObjectID initialObjectId)
//  {
//    return E_NOTIMPL;
//  }
//  STDMETHOD(HandleDestroyed)(GCHandleID handleId)
//  {
//    return E_NOTIMPL;
//  }
//};
//
//void __stdcall EnterStub( FunctionID fid )
//{
//  CNProfCORHook::GetProfiler()->Enter( fid );
//}
//
//void __stdcall LeaveStub( FunctionID fid )
//{
//  CNProfCORHook::GetProfiler()->Leave( fid );
//}
//
//void __stdcall TailCallStub( FunctionID fid )
//{
//  CNProfCORHook::GetProfiler()->TailCall( fid );
//}
//
//void __declspec( naked ) RawEnter()
//{
//    __asm
//    {
//        push eax
//        push ecx
//        push edx
//        push [esp + 16]
//        call EnterStub
//        pop edx
//        pop ecx
//        pop eax
//        ret 4
//    }   
//}
//
//void __declspec( naked ) RawLeave()
//{
//    __asm
//    {
//        push eax
//        push ecx
//        push edx
//        push [esp + 16]
//        call LeaveStub
//        pop edx
//        pop ecx
//        pop eax
//        ret 4
//    }   
//}
//
//void __declspec( naked ) RawTailCall()
//{
//    __asm
//    {
//        push eax
//        push ecx
//        push edx
//        push [esp + 16]
//        call TailCallStub
//        pop edx
//        pop ecx
//        pop eax
//        ret 4
//    }  
//}
