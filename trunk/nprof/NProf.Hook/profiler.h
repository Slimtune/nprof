/***************************************************************************
                          profiler.h  -  description
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

#ifndef PROFILER_H
#define PROFILER_H

#include "stdafx.h"
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
using namespace std;

// COR includes
#include "cor.h"
#include "corhdr.h"
#include "corhlpr.h"
#include "corerror.h"

#include "corsym.h"
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
  ProfilerHelper();
  ~ProfilerHelper(void);
  void Initialize( ICorProfilerInfo* profilerInfo ); 
  
  void GetFunctionSignature(   
    FunctionID functionId, 
    UINT32& methodAttributes, 
    string& returnType, 
    string& className,
    string& functionName,
    string& parameters );
  ThreadID GetCurrentThreadID();
  string ProfilerHelper::GetCurrentThreadName();
private:
  HRESULT GetFunctionProperties( FunctionID functionID,
										    UINT32* methodAttributes,
										    ULONG* argCount,
										    WCHAR* returnType, 
										    WCHAR* parameters,
										    WCHAR* className,
                        WCHAR* functionName );
  PCCOR_SIGNATURE ParseElementType( IMetaDataImport* metaDataImport,
											    PCCOR_SIGNATURE signature, 
											    char* buffer );
  CComPtr< ICorProfilerInfo > profilerInfo;
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

class ProfilerSocket
{
public:
  ProfilerSocket();
  ~ProfilerSocket(void);
  static void Initialize();
  void SendShutdown();
  void SendInitialize();
  void SendAppDomainCreate( AppDomainID appDomainId );
  void SendThreadCreate( ThreadID threadId );
  void SendThreadEnd( ThreadID threadId, UINT64 threadStartTime, UINT64 threadEndTime );
  void SendStartFunctionData( ThreadID threadId );
  void SendFunctionData( ProfilerHelper& profilerHelper, FunctionID functionId );
  void SendProfilerMessage( const string& message );
  void SendFunctionTimingData( int calls, UINT64 cycleCount, UINT64 recursiveCycleCount, UINT64 suspendCycleCount );
  void SendCalleeFunctionData( FunctionID functionId, int calls, UINT64 cycleCount, UINT64 recursiveCycleCount );
  void SendEndFunctionData();
  void SendEndCalleeFunctionData();
  void HandleError( const char* caller, int errorNumber );
  void HandleWrongSentLength( const char* caller, int expected, int sent );
  void HandleWrongRecvLength( const char* caller, int expected, int sent );
  int ReadByte( BYTE& b );

private:
  inline void SendBool( bool boolean );
  inline void SendUINT32( UINT32 integer );
  inline void SendUINT64( UINT64 integer );
  inline void SendString( const string& functionSignature );
  inline void SendNetworkMessage( NetworkMessage networkMessage );
  inline void SendAppDomainID( AppDomainID appDomainId );
  inline void SendThreadID( ThreadID threadId );
  inline void SendFunctionID( FunctionID functionId );

private:
  SOCKET socket;
  const char* operation;
  bool isApplicationIdSent;

  static bool isInitialized;
  static int applicationId;
};

class CalleeFunctionInfo {
public: 
	CalleeFunctionInfo();
	~CalleeFunctionInfo();

  INT64 cycleCount;
  INT64 recursiveCycleCount;
  int calls;
  int recursiveCount;
};
class FunctionInfo
{
public: 
	FunctionInfo( FunctionID fid );
	~FunctionInfo();
  CalleeFunctionInfo* GetCalleeFunctionInfo( FunctionID fid );
  void Trace( ProfilerHelper& ph );
  void Dump( ProfilerSocket& ps, ProfilerHelper& ph );

  int calls;
  int recursiveCount;
  INT64 cycleCount;
  INT64 recursiveCycleCount;
  INT64 suspendCycleCount;
  FunctionID functionId;

  map< FunctionID, CalleeFunctionInfo* > calleeMap;
};

class ThreadInfo;

class StackEntryInfo {
public: 
	StackEntryInfo( FunctionInfo* functionInfo, INT64 cycleStart );
	StackEntryInfo( const StackEntryInfo& rhs );
	~StackEntryInfo();
  
  INT64 cycleStart;
  FunctionInfo* functionInfo;
};
class StackInfo {
public: 
	StackInfo( ThreadInfo* threadInfo );
	~StackInfo();
  
  INT64 PopFunction( INT64 cycleCount );
  void PushFunction( FunctionInfo* functionInfo, INT64 cycleCount );
  void Trace();

  void SuspendFunction( INT64 cycleCount );
  void ResumeFunction( INT64 cycleCount );
private:
  INT64 suspendStart;
  stack< StackEntryInfo > functionStack;
  ThreadInfo* threadInfo;
};


class ThreadInfo
{
public: 
	ThreadInfo();
	~ThreadInfo();

  void Start();
  void End();
  bool IsRunning();
  StackInfo* GetStackInfo();
  FunctionInfo* GetFunctionInfo( FunctionID functionId );
  void Trace( ProfilerHelper& ph );
  void Dump( ProfilerSocket& profilerSocket, ProfilerHelper& profilerHelper );
  INT64 startTime;
  INT64 endTime;
  INT64 suspendTime;
  
private:
  bool  isRunning;
  StackInfo* stackInfo;
  map< FunctionID, FunctionInfo* > functionMap;
};



class ThreadInfoCollection
{
public: 
	ThreadInfoCollection();
	~ThreadInfoCollection();

  ThreadInfo* GetThreadInfo( ThreadID threadId );
  /** No descriptions */
  void EndAll( ProfilerHelper& profilerHelper );
  void EndThread( ProfilerHelper& profilerHelper, ThreadID threadId );
  void DumpAll( ProfilerHelper& profilerHelper );
  void Dump( ProfilerHelper& profilerHelper, ThreadID threadId );
  void Trace( ProfilerHelper& profilerHelper );
private:
  map< ThreadID, ThreadInfo* > threadMap;
};
class Profiler
{
public: 
	Profiler( ICorProfilerInfo* profilerInfo );
	~Profiler();
  void Leave( FunctionID functionId );
  void Enter( FunctionID functionId );
  void TailCall( FunctionID functionId );
  void UnmanagedToManagedCall( FunctionID functionId );
  void ManagedToUnmanagedCall( FunctionID functionId );
  void ThreadStart( ThreadID threadId );
  void ThreadEnd( ThreadID threadId );
  void ThreadSuspend();
  void ThreadResume();
  void AppDomainStart( AppDomainID appDomainId );
  void AppDomainEnd( AppDomainID appDomainId );
  void End();
  void ThreadMap( ThreadID threadId, DWORD osThread );

  void Trace();
  CComPtr< ICorProfilerInfo > profilerInfo;

private:
  ThreadID GetCurrentThreadID();
  ThreadInfo* GetCurrentThreadInfo();

  ThreadInfoCollection threadCollection;
  ProfilerHelper profilerHelper;
  map< DWORD, ThreadID > threadMap;
};


// Pre-define these methods
void RawEnter();
void RawLeave();
void RawTailCall();

// INProfCORHook
[
	object,
	uuid("5B94DF43-780B-42FD-AC4C-ABAB35D4A274"),
	dual,	helpstring("INProfCORHook Interface"),
	pointer_default(unique)
]
__interface INProfCORHook : IDispatch
{
};


// CNProfCORHook

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

  // ICorProfilerCallback Methods
public:
  static Profiler* GetProfiler()
  {
    return profiler;
  }

  STDMETHOD(Initialize)(LPUNKNOWN pICorProfilerInfoUnk)
  {
    //DebugBreak();
    CComQIPtr< ICorProfilerInfo > profilerInfo = pICorProfilerInfoUnk;

    ProfilerSocket::Initialize();

    cout << "Initializing profiler hook DLL..." << endl;

    if ( profilerInfo )
    {
      profiler = new Profiler( profilerInfo );
      cout << "Initializing event masks..." << endl;
      profilerInfo->SetEventMask( 
        COR_PRF_MONITOR_THREADS	|
        COR_PRF_DISABLE_INLINING |
        COR_PRF_MONITOR_SUSPENDS |    
        COR_PRF_MONITOR_ENTERLEAVE |
        COR_PRF_MONITOR_EXCEPTIONS |  
        COR_PRF_MONITOR_APPDOMAIN_LOADS |
        COR_PRF_MONITOR_ASSEMBLY_LOADS |
		COR_PRF_MONITOR_CACHE_SEARCHES |
		COR_PRF_MONITOR_JIT_COMPILATION | 
        COR_PRF_MONITOR_CODE_TRANSITIONS
      );

      cout << "Initializing hooks..." << endl;
      profilerInfo->SetEnterLeaveFunctionHooks( ( FunctionEnter* )&RawEnter, ( FunctionLeave* )&RawLeave, ( FunctionTailcall* )&RawTailCall );
      cout << "Ready!" << endl;
    }

    return S_OK;
  }
  STDMETHOD(Shutdown)()
  {
    cout << "Terminating profiler..." << endl;
    profiler->End();
    delete profiler;
    profiler = NULL;
    ProfilerSocket profilerSocket;
    profilerSocket.SendShutdown();

    return S_OK;
  }
  STDMETHOD(AppDomainCreationStarted)(AppDomainID appDomainId)
  {
    profiler->AppDomainStart( appDomainId );
    return S_OK;
  }
  STDMETHOD(AppDomainCreationFinished)(AppDomainID appDomainId, HRESULT hrStatus)
  {
    return E_NOTIMPL;
  }
  STDMETHOD(AppDomainShutdownStarted)(AppDomainID appDomainId)
  {
    profiler->AppDomainEnd( appDomainId );
    return S_OK;
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
    profiler->ThreadStart( threadId );
    return S_OK;
  }
  STDMETHOD(ThreadDestroyed)(ThreadID threadId)
  {
    profiler->ThreadEnd( threadId );
    return S_OK;
  }
  STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadId, DWORD osThreadId)
  {
    profiler->ThreadMap( managedThreadId, osThreadId );
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
    // Only track returns
    if ( reason == COR_PRF_TRANSITION_RETURN )
      profiler->UnmanagedToManagedCall( functionId );
    return S_OK;
  }
  STDMETHOD(ManagedToUnmanagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason)
  {
    // Only track calls
    if ( reason == COR_PRF_TRANSITION_CALL )
      profiler->ManagedToUnmanagedCall( functionId );
    return S_OK;
  }
  STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason)
  {
    profiler->ThreadSuspend();
    return S_OK;
  }
  STDMETHOD(RuntimeSuspendFinished)()
  {
    return S_OK;
  }
  STDMETHOD(RuntimeSuspendAborted)()
  {
    return S_OK;
  }
  STDMETHOD(RuntimeResumeStarted)()
  {
    profiler->ThreadResume();
    return S_OK;
  }
  STDMETHOD(RuntimeResumeFinished)()
  {
    return S_OK;
  }
  STDMETHOD(RuntimeThreadSuspended)(ThreadID threadId)
  {
    return S_OK;
  }
  STDMETHOD(RuntimeThreadResumed)(ThreadID threadId)
  {
    return S_OK;
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
    // Update the call stack as we leave
    profiler->Leave( 0 );

    return S_OK;
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
