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
  //ProfilerHelper();
  //~ProfilerHelper(void);
  void Initialize( ICorProfilerInfo2* profilerInfo ); 
  
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
  
  INT64 cycleStart;
  FunctionInfo* functionInfo;
};
class StackInfo {
public: 
	StackInfo( ThreadInfo* threadInfo );
  
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

  ThreadInfo* GetThreadInfo( ThreadID threadId );

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
	Profiler( ICorProfilerInfo2* profilerInfo );
	void WalkStack();
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
	CComPtr< ICorProfilerInfo2 > profilerInfo;
	bool statisticalCompleted;
	static UINT timer;
private:
	ThreadID GetCurrentThreadID();
	ThreadInfo* GetCurrentThreadInfo();

	ThreadInfoCollection threadCollection;
	ProfilerHelper profilerHelper;
	map< DWORD, ThreadID > threadMap;
	bool statistical;
};
UINT Profiler::timer;


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

int profileCount=0;
ThreadID globalId=0;
DWORD globalNativeId=0;

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
    CComQIPtr< ICorProfilerInfo2 > profilerInfo = pICorProfilerInfoUnk;

    ProfilerSocket::Initialize();

    cout << "Initializing profiler hook DLL..." << endl;

    if ( profilerInfo )
    {
      profiler = new Profiler( profilerInfo );
      cout << "Initializing event masks..." << endl;
      profilerInfo->SetEventMask( 
		COR_PRF_ENABLE_STACK_SNAPSHOT|
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
	  if(globalNativeId==0)
	  {
		globalNativeId=osThreadId;
	  }
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


Profiler::Profiler( ICorProfilerInfo2* profilerInfo )
{
	this->profilerInfo = profilerInfo;
	this->profilerHelper.Initialize( profilerInfo );
}

void Profiler::Enter( FunctionID functionId )
{
  ThreadInfo* threadInfo=GetCurrentThreadInfo();
  FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
  threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
}

void Profiler::Leave( FunctionID functionId )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

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

void Profiler::ThreadEnd( ThreadID threadId )
{
  threadCollection.EndThread( profilerHelper, threadId );
  cout << "ThreadEnd( " << threadId << " )" << endl;
}

void Profiler::ThreadSuspend()
{
  //cout << "ThreadSuspend( " << GetCurrentThreadID() << " )" << endl;
  threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->SuspendFunction( rdtsc() );
}

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

ThreadID Profiler::GetCurrentThreadID()
{
  return profilerHelper.GetCurrentThreadID();
}

ThreadInfo* Profiler::GetCurrentThreadInfo()
{
  return threadCollection.GetThreadInfo( GetCurrentThreadID() );
}

void Profiler::Trace()
{
  threadCollection.Trace( profilerHelper );
}



////HRESULT __stdcall __stdcall StackWalker( 
////	FunctionID funcId,
////	UINT_PTR ip,
////	COR_PRF_FRAME_INFO frameInfo,
////	ULONG32 contextSize,
////	BYTE context[  ],
////	void *clientData)
////{
////	//cout << "StackWalker\n" << funcId << "\n";
////	//if(funcId!=0)
////	//{
////	//	((vector<FunctionID>*)clientData)->push_back(funcId);
////	//}
////	//cout << "in stackwalker";
////	return S_OK;
////}
////
////
////
////void CALLBACK TimerFunction(UINT wTimerID, UINT msg, 
////    DWORD dwUser, DWORD dw1, DWORD dw2) 
////{
////	Profiler* profiler=(Profiler*)dwUser;
////	profiler->WalkStack();
////}
////
//////DWORD globalThread;
////void Profiler::WalkStack()
////{
////	//cout << "walk stack\n";
////	//cout << "profileCount:"<<profileCount<<"\n";
////	profileCount++;
////	//threadMap.
////	DWORD threadId=globalNativeId;
////	ThreadID id=globalId;
////
////	//DWORD threadId=threadMap._Myfirstiter->first;
////	//ThreadID id=threadMap._Myfirstiter->second;
////
////	//HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
////	HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
////	int suspended=SuspendThread(threadHandle);
////	timeKillEvent(timer);
////	DebugBreak();
////	HRESULT result=profilerInfo->SetEventMask(0);
////	profilerInfo->DoStackSnapshot(
////		//NULL,
////		id,
////		//threadId,
////		StackWalker,
////		COR_PRF_SNAPSHOT_DEFAULT,
////		NULL,
////		//&functions,
////		NULL,
////		NULL);
////		//(BYTE*)(void*)pContext,
////		//sizeof(CONTEXT));
////	ResumeThread(threadHandle);
////}
//////void Profiler::WalkStack()
//////{
//////
//////	//cout << "walk stack\n";
//////	//cout << "profileCount:"<<profileCount<<"\n";
//////	profileCount++;
//////
//////	for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++)
//////	{
//////		i++;
//////		DWORD threadId=(*i).first;
//////		HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
//////		if(threadHandle!=NULL)
//////		{
//////
//////			int suspended=SuspendThread(threadHandle);
//////			//cout << "suspended: "<< suspended <<"\n";
//////		}
//////	}
//////	//for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++)
//////	for(map< DWORD, ThreadID >::reverse_iterator i=threadMap.rend();i!=threadMap.rbegin();i++)
//////	{
//////		i++;
//////		DWORD threadId=(*i).first;
//////		//cout<<"threadId "<<threadId<<"\n";
//////		//cout<<"managed thread "<<i->second<<"\n";
//////
//////		HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
//////		//HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
//////		if(threadHandle!=NULL)
//////		{
//////			//cout <<"opened thread";
//////			vector<FunctionID> functions;
//////			//cout << " thread id" <<threadId <<"\n";
//////			//BYTE bytes[]={0,0,0,0,0,0,0,0,0,0,0};
//////			//CONTEXT context;
//////			//DWORD test;
//////			//profilerInfo->GetEventMask(&test);
//////
//////			//cout << "DoStackSnapshot";
//////			//cout << 
//////			//profilerInfo->GetFunctionFromIP(,&newID);
//////			//align dword;
//////			CONTEXT context;
//////
//////			
//////			//context.ContextFlags=CONTEXT_DEBUG_REGISTERS;
//////			context.ContextFlags=CONTEXT_FULL;
//////
//////			GetThreadContext(threadHandle,&context);
//////			//cout << "threadContext" << GetThreadContext(threadHandle,&context) <<"\n";
//////			//cout << "lastError" << GetLastError() <<"\n";
//////
//////			CONTEXT* pContext=&context;
//////			//try
//////			//{
//////			ThreadID id=i->second;
//////
//////
//////				//cout << "DoStackSnapshot" << 
//////			profilerInfo->DoStackSnapshot(
//////				//NULL,
//////				threadId,
//////				StackWalker,
//////				COR_PRF_SNAPSHOT_DEFAULT,
//////				&functions,
//////				NULL,
//////				NULL);
//////				//(BYTE*)(void*)pContext,
//////				//sizeof(CONTEXT));
//////			ResumeThread(threadHandle);
//////			for(int i=0	;i<functions.size();i++)
//////			{
//////				ThreadInfo* threadInfo=threadCollection.GetThreadInfo(id);
//////				for(int y=i+1;;y++)
//////				{
//////					if(y>functions.size()-1)
//////					{
//////						FunctionID id=functions[i];
//////						threadInfo->GetFunctionInfo(id)->calls++;
//////						break;
//////					}
//////					if(functions[y]==functions[i])
//////					{
//////						break;
//////					}
//////				}
//////			}
//////			//if(ResumeThread(threadHandle)!=-1)
//////			//{
//////			//	//cout << "resumed thread";
//////			//}
//////		}
//////		else
//////		{
//////			//cout <<"could not open thread";
//////		}
//////		break;
//////	}
//////}
////Profiler::Profiler( ICorProfilerInfo2* profilerInfo )
////{
////	this->profilerInfo = profilerInfo;
////	this->profilerHelper.Initialize( profilerInfo );
////	this->statistical = true;
////	this->statisticalCompleted=false;
////	
////
////	TIMECAPS timeCaps;
////	timeGetDevCaps(&timeCaps, sizeof(TIMECAPS));
////	timer = timeSetEvent(
////	  500,
////	  timeCaps.wPeriodMin, 
////	  TimerFunction,  
////	  (DWORD_PTR)this,      
////	  TIME_PERIODIC);      
////}
////void Profiler::Enter( FunctionID functionId )
////{
////	if(statistical)
////	{
////		//	vector<FunctionID> functions;
////
////			//profilerInfo->DoStackSnapshot(
////			//	NULL,
////			//	StackWalker,
////			//	COR_PRF_SNAPSHOT_DEFAULT,
////			//	&functions,
////			//	NULL,
////			//	0);
////		//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
////		//	for(int i=1;i<functions.size();i++)
////		//	{
////		//		for(int y=i+1;;y++)
////		//		{
////		//			if(y>functions.size()-1)
////		//			{
////		//				FunctionID id=functions[i];
////		//				threadInfo->GetFunctionInfo(id)->calls++;
////		//				break;
////		//			}
////		//			if(functions[y]==functions[i])
////		//			{
////		//				break;
////		//			}
////		//			//if(y==functions.size()-1)
////		//			//{
////		//			//	FunctionID id=functions[i];
////		//			//	threadInfo->GetFunctionInfo(id)->calls++;
////		//			//	break;
////		//			//}
////		//		}
////		//	}
////		//}
////	}
////	else
////	{
////	  ThreadInfo* threadInfo=GetCurrentThreadInfo();
////	  FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
////	  threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
////	}
////}
////
////void Profiler::Leave( FunctionID functionId )
////{
////	if(statistical)
////	{
////		//if(!statisticalCompleted)
////		//{
////		//	statisticalCompleted=true;
////		//	Deactivate();
////		//	 //HRESULT result= profilerInfo->SetEventMask( 
////		//		// COR_PRF_ENABLE_STACK_SNAPSHOT
////		//		//// COR_PRF_MONITOR_NONE
////		//		////COR_PRF_MONITOR_THREADS	|
////		//		////COR_PRF_DISABLE_INLINING |
////		//		////COR_PRF_ENABLE_STACK_SNAPSHOT|
////		//		////COR_PRF_MONITOR_SUSPENDS |    
////		//		////COR_PRF_MONITOR_EXCEPTIONS |  
////		//		////COR_PRF_MONITOR_APPDOMAIN_LOADS |
////		//		////COR_PRF_MONITOR_ASSEMBLY_LOADS |
////		//		////COR_PRF_MONITOR_CACHE_SEARCHES |
////		//		////COR_PRF_MONITOR_JIT_COMPILATION 
////		//		////| COR_PRF_MONITOR_CODE_TRANSITIONS
////		//	 // );		
////		//	vector<FunctionID> functions;
////
////		//	functions.push_back(functionId);
////		//	profilerInfo->DoStackSnapshot(
////		//		NULL,
////		//		StackWalker,
////		//		COR_PRF_SNAPSHOT_DEFAULT,
////		//		&functions,
////		//		NULL,
////		//		0);
////		//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
////		//	for(int i=0;i<functions.size();i++)
////		//	{
////		//		for(int y=i+1;;y++)
////		//		{
////		//			if(y>functions.size()-1)
////		//			{
////		//				FunctionID id=functions[i];
////		//				threadInfo->GetFunctionInfo(id)->calls++;
////		//				break;
////		//			}
////		//			if(functions[y]==functions[i])
////		//			{
////		//				break;
////		//			}
////		//			//if(y==functions.size()-1)
////		//			//{
////		//			//	FunctionID id=functions[i];
////		//			//	threadInfo->GetFunctionInfo(id)->calls++;
////		//			//	break;
////		//			//}
////		//		}
////		//		//}
////		//		//FunctionID id=functions[i];
////		//		//threadInfo->GetFunctionInfo(id)->calls++;
////		//	}
////		//}
////	}
////	else
////	{
////		GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
////	}
////}
////
////void Profiler::TailCall( FunctionID functionId )
////{
////	//Leave(functionId);
////	if(statistical)
////	{
////		Enter(functionId);
////	}
////	else
////	{
////		Leave(functionId);
////		//GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
////	}
////}
////
////void Profiler::UnmanagedToManagedCall( FunctionID functionId )
////{
////	Enter(functionId);
////	//if(statistical)
////	//{
////	//	//this->Enter(functionId);
////	//}
////	//else
////	//{
////	//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
////	//	FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
////	//	threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
////	//}
////}
////
////void Profiler::ManagedToUnmanagedCall( FunctionID functionId )
////{
////	Leave(functionId);
////	//if(statistical)
////	//{
////	//	Leave(functionId);
////	//}
////	//else
////	//{
////	//	GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
////	//}
////}
////void Profiler::ThreadStart( ThreadID threadId )
////{
////  //cout << "ThreadStart( " << threadId << " )" << endl;
////	if(globalId==0)
////	{
////		globalId=threadId;
////	}
////  threadCollection.GetThreadInfo( threadId )->Start();
////  ProfilerSocket ps;
////  ps.SendThreadCreate( threadId );
////}
////
////void Profiler::ThreadMap( ThreadID threadId, DWORD dwOSThread )
////{
////  //cout << "ThreadMap( " << threadId << ", " << dwOSThread << ")" << endl;
////  threadMap[ dwOSThread ] = threadId;
////}
////
////void Profiler::ThreadEnd( ThreadID threadId )
////{
////	//Sleep(1000000);
////  threadCollection.EndThread( profilerHelper, threadId );
////  //cout << "ThreadEnd( " << threadId << " )" << endl;
////}
////
////void Profiler::ThreadSuspend()
////{
////  //cout << "ThreadSuspend( " << GetCurrentThreadID() << " )" << endl;
////  threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->SuspendFunction( rdtsc() );
////}
////
////void Profiler::ThreadResume()
////{
////  //cout << "ThreadResume( " << GetCurrentThreadID() << " )" << endl;
////  threadCollection.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->ResumeFunction( rdtsc() );
////}
////
////void Profiler::AppDomainStart( AppDomainID appDomainId )
////{
////  cout << "AppDomain Created: " << appDomainId << endl;
////  ProfilerSocket ps;
////  ps.SendAppDomainCreate( appDomainId );
////}
////
////void Profiler::AppDomainEnd( AppDomainID appDomainId )
////{
////}
////
////void Profiler::End()
////{
////	cout << "End()" << endl;
////	timeKillEvent(timer);
////
////  threadCollection.EndAll( profilerHelper );
////}
////
////ThreadID Profiler::GetCurrentThreadID()
////{
////  return profilerHelper.GetCurrentThreadID();
////}
////
////ThreadInfo* Profiler::GetCurrentThreadInfo()
////{
////  return threadCollection.GetThreadInfo( GetCurrentThreadID() );
////}
////
////void Profiler::Trace()
////{
////  threadCollection.Trace( profilerHelper );
////}
//void Profiler::Activate()
//{
//	 profilerInfo->SetEventMask(
//		 COR_PRF_MONITOR_THREADS
//		 |COR_PRF_ENABLE_STACK_SNAPSHOT
//		 //|COR_PRF_MONITOR_ENTERLEAVE
//		 );
//	//profilerInfo->SetEventMask(
//	//	COR_PRF_MONITOR_THREADS	
//	//	|COR_PRF_DISABLE_INLINING
//	//	|COR_PRF_ENABLE_STACK_SNAPSHOT
//	//	|COR_PRF_MONITOR_SUSPENDS
//	//	|COR_PRF_MONITOR_ENTERLEAVE
//	//	|COR_PRF_MONITOR_EXCEPTIONS
//	//	|COR_PRF_MONITOR_APPDOMAIN_LOADS
//	//	|COR_PRF_MONITOR_ASSEMBLY_LOADS
//	//	|COR_PRF_MONITOR_CACHE_SEARCHES
//	//	|COR_PRF_MONITOR_JIT_COMPILATION
//	//	|COR_PRF_MONITOR_CODE_TRANSITIONS
//	//);
//}
//void Profiler::Deactivate()
//{
//	 //profilerInfo->SetEventMask(
//		//COR_PRF_MONITOR_THREADS
//	 //|COR_PRF_ENABLE_STACK_SNAPSHOT
//	 //|COR_PRF_MONITOR_ENTERLEAVE
//	 //);
//	//profilerInfo->SetEventMask(
//	//	COR_PRF_MONITOR_THREADS	
//	//	|COR_PRF_DISABLE_INLINING
//	//	|COR_PRF_ENABLE_STACK_SNAPSHOT
//	//	|COR_PRF_MONITOR_SUSPENDS
//	//	//|COR_PRF_MONITOR_ENTERLEAVE
//	//	|COR_PRF_MONITOR_EXCEPTIONS
//	//	|COR_PRF_MONITOR_APPDOMAIN_LOADS
//	//	|COR_PRF_MONITOR_ASSEMBLY_LOADS
//	//	|COR_PRF_MONITOR_CACHE_SEARCHES
//	//	|COR_PRF_MONITOR_JIT_COMPILATION
//	//	|COR_PRF_MONITOR_CODE_TRANSITIONS
//	//);
//
//	//profilerInfo->SetEventMask(
//	//COR_PRF_MONITOR_THREADS	|
//	//COR_PRF_DISABLE_INLINING |
//	//COR_PRF_ENABLE_STACK_SNAPSHOT//|
//	//COR_PRF_MONITOR_SUSPENDS 
//	//|    
//	//COR_PRF_MONITOR_ENTERLEAVE |
//	//COR_PRF_MONITOR_EXCEPTIONS |  
//	//COR_PRF_MONITOR_APPDOMAIN_LOADS |
//	//COR_PRF_MONITOR_ASSEMBLY_LOADS |
//	//COR_PRF_MONITOR_CACHE_SEARCHES |
//	//COR_PRF_MONITOR_JIT_COMPILATION //| 
//	//COR_PRF_MONITOR_CODE_TRANSITIONS
//	//);
//}

CalleeFunctionInfo::CalleeFunctionInfo()
{
  this->cycleCount = 0;
  this->recursiveCycleCount = 0;
  this->recursiveCount = 0;
  this->calls = 0;
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

[ module(dll, uuid = "{A461E20A-C7DC-4A89-A24E-87B5E975A96B}", 
		 name = "NProfHook", 
		 helpstring = "NProf.Hook 1.0 Type Library",
		 resource_name = "IDR_NPROFHOOK") ];


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


StackInfo::StackInfo( ThreadInfo* threadInfo )
{
	this->threadInfo = threadInfo;
}

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

void StackInfo::SuspendFunction( INT64 cycleCount )
{
	suspendStart = cycleCount;
	if ( functionStack.size() == 0 ) 
	{
		//cout << "Suspend with no call stack!" << endl;
		return;
	}
	//cout << "Suspended function ID: " << functionStack.top().functionInfo->fid << endl;
}

void StackInfo::ResumeFunction( INT64 cycleCount )
{
	INT64 elapsed = cycleCount - suspendStart;
	// Resume with no call stack, ignore
	if ( functionStack.size() == 0 ) 
	{
		//cout << "Resume with no call stack!" << endl;
		return;
	}
	functionStack.top().functionInfo->suspendCycleCount += elapsed;
	threadInfo->suspendTime += elapsed;
	//cout << "Resumed function ID: " << functionStack.top().functionInfo->fid << endl;
}

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

void ThreadInfoCollection::Dump( ProfilerHelper& profilerHelper, ThreadID threadId )
{
  ProfilerSocket profilerSocket;
  profilerSocket.SendStartFunctionData( threadId );
  GetThreadInfo( threadId )->Dump( profilerSocket, profilerHelper );
  profilerSocket.SendEndFunctionData();
}
Profiler* CNProfCORHook::profiler;