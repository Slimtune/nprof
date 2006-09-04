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

#include "stdafx.h"
#include "profiler.h"
#include "profiler_socket.h"
#include <vector>

HRESULT __stdcall __stdcall StackWalker( 
	FunctionID funcId,
	UINT_PTR ip,
	COR_PRF_FRAME_INFO frameInfo,
	ULONG32 contextSize,
	BYTE context[  ],
	void *clientData)
{
	//timeKillEvent(profiler->timer);
	//DebugBreak();

	//if(funcId!=0)
	//{
	//	((vector<FunctionID>*)clientData)->push_back(funcId);
	//}
	cout << "in stackwalker";
	return S_OK;
}



void CALLBACK TimerFunction(UINT wTimerID, UINT msg, 
    DWORD dwUser, DWORD dw1, DWORD dw2) 
{
	Profiler* profiler=(Profiler*)dwUser;
	//cout << "timer function";
	profiler->WalkStack();
	/*timeKillEvent(profiler->timer);
	DebugBreak();*/
	//for(map< DWORD, ThreadID >::iterator i=profiler->threadMap.begin();i!=profiler->threadMap.end();i++)
	//{
	//	DWORD threadId=(*i).first;
	//	HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
	//	if(threadHandle!=NULL)
	//	{
	//		SuspendThread(threadHandle);
	//	}
	//}
	//for(map< DWORD, ThreadID >::reverse_iterator i=profiler->threadMap.rend();i!=profiler->threadMap.rbegin();i++)
	//{
	//	i++;
	//	DWORD threadId=(*i).first;
	//	HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
	//	if(threadHandle!=NULL)
	//	{
	//		if(SuspendThread(threadHandle)!=-1)
	//		{
	//			vector<FunctionID> functions;
	//			//BYTE bytes[]={0,0,0,0,0,0,0,0,0,0,0};
	//			//CONTEXT context;

	//			//FunctionID newID;
	//			//profiler->profilerInfo->GetFunctionFromIP(,&newID);
	//			HRESULT result=profiler->profilerInfo->DoStackSnapshot(
	//				threadId,
	//				StackWalker,
	//				COR_PRF_SNAPSHOT_REGISTER_CONTEXT,
	//				&functions,
	//				NULL,
	//				0);

	//			ThreadID id=i->second;
	//			//ThreadID profiler->threadMap[threadId]
	//			for(int i=0	;i<functions.size();i++)
	//			{
	//				//timeKillEvent(profiler->timer);
	//				//DebugBreak();
	//				ThreadInfo* threadInfo=profiler->threadCollection.GetThreadInfo(id);

	//				for(int y=i+1;;y++)
	//				{
	//					if(y>functions.size()-1)
	//					{
	//						FunctionID id=functions[i];
	//						threadInfo->GetFunctionInfo(id)->calls++;
	//						break;
	//					}
	//					if(functions[y]==functions[i])
	//					{
	//						break;
	//					}
	//				}
	//			}
	//			ResumeThread(threadHandle);
	//		}
	//	}
	//	break;
	//}
}
void Profiler::WalkStack()
{
	//cout << "walk stack\n";
	for(map< DWORD, ThreadID >::iterator i=threadMap.begin();i!=threadMap.end();i++)
	{
		//DebugBreak();
		i++;
		DWORD threadId=(*i).first;
		HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
		if(threadHandle!=NULL)
		{

			int suspended=SuspendThread(threadHandle);
			//cout << "suspended: "<< suspended <<"\n";
		}
	}
	for(map< DWORD, ThreadID >::reverse_iterator i=threadMap.rend();i!=threadMap.rbegin();i++)
	{
		i++;
		DWORD threadId=(*i).first;
		HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME,false,threadId);
		if(threadHandle!=NULL)
		{

			int suspended=SuspendThread(threadHandle);
			//cout << "suspended: "<< suspended <<"\n";
			if(suspended!=-1)
			{
				//cout << "suspended thread";
				vector<FunctionID> functions;
				//cout << " thread id" <<threadId <<"\n";
				//BYTE bytes[]={0,0,0,0,0,0,0,0,0,0,0};
				//CONTEXT context;
				//DWORD test;
				//profilerInfo->GetEventMask(&test);
				//timeKillEvent(timer);
				DebugBreak();

				profilerInfo->DoStackSnapshot(
					threadId,
					StackWalker,
					COR_PRF_SNAPSHOT_DEFAULT,
					&functions,
					NULL,
					NULL);

				//FunctionID newID;
				//profiler->profilerInfo->GetFunctionFromIP(,&newID);
				//HRESULT result=profilerInfo->DoStackSnapshot(
				//	threadId,
				//	StackWalker,
				//	COR_PRF_SNAPSHOT_REGISTER_CONTEXT,
				//	&functions,
				//	NULL,
				//	0);
				//timeKillEvent(timer);
				//DebugBreak();

				ThreadID id=i->second;
				//ThreadID profiler->threadMap[threadId]
				for(int i=0	;i<functions.size();i++)
				{
					//timeKillEvent(profiler->timer);
					//DebugBreak();
					ThreadInfo* threadInfo=threadCollection.GetThreadInfo(id);

					for(int y=i+1;;y++)
					{
						if(y>functions.size()-1)
						{
							FunctionID id=functions[i];
							threadInfo->GetFunctionInfo(id)->calls++;
							break;
						}
						if(functions[y]==functions[i])
						{
							break;
						}
					}
				}
				if(ResumeThread(threadHandle)!=-1)
				{
					//cout << "resumed thread";
				}
			}
		}
		break;
	}
}
		  //profiler->statisticalCompleted=false;
		  //profiler->Activate();
		 // ->profilerInfo->SetEventMask( 
			//  COR_PRF_ENABLE_STACK_SNAPSHOT
			//  |COR_PRF_MONITOR_ENTERLEAVE
			//  );
			//  //COR_PRF_MONITOR_NONE);
			////COR_PRF_MONITOR_THREADS	|
			////COR_PRF_DISABLE_INLINING |
			////COR_PRF_ENABLE_STACK_SNAPSHOT|
			////COR_PRF_MONITOR_SUSPENDS |    
			////COR_PRF_MONITOR_ENTERLEAVE |
			////COR_PRF_MONITOR_EXCEPTIONS |  
			////COR_PRF_MONITOR_APPDOMAIN_LOADS |
			////COR_PRF_MONITOR_ASSEMBLY_LOADS |
			////COR_PRF_MONITOR_CACHE_SEARCHES |
			////COR_PRF_MONITOR_JIT_COMPILATION //| 
			//////COR_PRF_MONITOR_CODE_TRANSITIONS
			////);
Profiler::Profiler( ICorProfilerInfo2* profilerInfo )
{
	this->profilerInfo = profilerInfo;
	this->profilerHelper.Initialize( profilerInfo );
	this->statistical = true;
	this->statisticalCompleted=false;
	

	//DebugBreak();
	TIMECAPS timeCaps;
	timeGetDevCaps(&timeCaps, sizeof(TIMECAPS));
	timer = timeSetEvent(
	  500,
	  timeCaps.wPeriodMin, 
	  TimerFunction,  
	  (DWORD_PTR)this,      
	  TIME_PERIODIC);      
}

Profiler::~Profiler()
{
}


/** No descriptions */
void Profiler::Enter( FunctionID functionId )
{
	if(statistical)
	{
		//DebugBreak();
		//WalkStack();
		//if(!statisticalCompleted)
		//{
		//	statisticalCompleted=true;
		//	Deactivate();
		//	 //HRESULT result= profilerInfo->SetEventMask( 
		//		// COR_PRF_ENABLE_STACK_SNAPSHOT);
		//		//COR_PRF_MONITOR_THREADS	|
		//		////COR_PRF_DISABLE_INLINING |
		//		////COR_PRF_ENABLE_STACK_SNAPSHOT|
		//		////COR_PRF_MONITOR_SUSPENDS |    
		//		////COR_PRF_MONITOR_EXCEPTIONS |  
		//		////COR_PRF_MONITOR_APPDOMAIN_LOADS |
		//		////COR_PRF_MONITOR_ASSEMBLY_LOADS |
		//		////COR_PRF_MONITOR_CACHE_SEARCHES |
		//		////COR_PRF_MONITOR_JIT_COMPILATION 
		//		//////| COR_PRF_MONITOR_CODE_TRANSITIONS
		//	 // //);		
		//	vector<FunctionID> functions;

			//profilerInfo->DoStackSnapshot(
			//	NULL,
			//	StackWalker,
			//	COR_PRF_SNAPSHOT_DEFAULT,
			//	&functions,
			//	NULL,
			//	0);
		//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
		//	//DebugBreak();
		//	for(int i=1;i<functions.size();i++)
		//	{
		//		for(int y=i+1;;y++)
		//		{
		//			if(y>functions.size()-1)
		//			{
		//				FunctionID id=functions[i];
		//				threadInfo->GetFunctionInfo(id)->calls++;
		//				break;
		//			}
		//			if(functions[y]==functions[i])
		//			{
		//				//DebugBreak();
		//				break;
		//			}
		//			//if(y==functions.size()-1)
		//			//{
		//			//	FunctionID id=functions[i];
		//			//	threadInfo->GetFunctionInfo(id)->calls++;
		//			//	break;
		//			//}
		//		}
		//	}
		//}
	}
	else
	{
	  ThreadInfo* threadInfo=GetCurrentThreadInfo();
	  FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
	  threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
	}
}

/** No descriptions */
void Profiler::Leave( FunctionID functionId )
{
	if(statistical)
	{
		//if(!statisticalCompleted)
		//{
		//	statisticalCompleted=true;
		//	Deactivate();
		//	 //HRESULT result= profilerInfo->SetEventMask( 
		//		// COR_PRF_ENABLE_STACK_SNAPSHOT
		//		//// COR_PRF_MONITOR_NONE
		//		////COR_PRF_MONITOR_THREADS	|
		//		////COR_PRF_DISABLE_INLINING |
		//		////COR_PRF_ENABLE_STACK_SNAPSHOT|
		//		////COR_PRF_MONITOR_SUSPENDS |    
		//		////COR_PRF_MONITOR_EXCEPTIONS |  
		//		////COR_PRF_MONITOR_APPDOMAIN_LOADS |
		//		////COR_PRF_MONITOR_ASSEMBLY_LOADS |
		//		////COR_PRF_MONITOR_CACHE_SEARCHES |
		//		////COR_PRF_MONITOR_JIT_COMPILATION 
		//		////| COR_PRF_MONITOR_CODE_TRANSITIONS
		//	 // );		
		//	vector<FunctionID> functions;

		//	functions.push_back(functionId);
		//	profilerInfo->DoStackSnapshot(
		//		NULL,
		//		StackWalker,
		//		COR_PRF_SNAPSHOT_DEFAULT,
		//		&functions,
		//		NULL,
		//		0);
		//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
		//	//DebugBreak();
		//	for(int i=0;i<functions.size();i++)
		//	{
		//		for(int y=i+1;;y++)
		//		{
		//			if(y>functions.size()-1)
		//			{
		//				FunctionID id=functions[i];
		//				threadInfo->GetFunctionInfo(id)->calls++;
		//				break;
		//			}
		//			if(functions[y]==functions[i])
		//			{
		//				//DebugBreak();
		//				break;
		//			}
		//			//if(y==functions.size()-1)
		//			//{
		//			//	FunctionID id=functions[i];
		//			//	threadInfo->GetFunctionInfo(id)->calls++;
		//			//	break;
		//			//}
		//		}
		//		//}
		//		//FunctionID id=functions[i];
		//		//threadInfo->GetFunctionInfo(id)->calls++;
		//	}
		//}
	}
	else
	{
		GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
	}
}

/** No descriptions */
void Profiler::TailCall( FunctionID functionId )
{
	//Leave(functionId);
	if(statistical)
	{
		Enter(functionId);
	}
	else
	{
		Leave(functionId);
		//GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
	}
}

void Profiler::UnmanagedToManagedCall( FunctionID functionId )
{
	Enter(functionId);
	//if(statistical)
	//{
	//	//this->Enter(functionId);
	//}
	//else
	//{
	//	ThreadInfo* threadInfo=GetCurrentThreadInfo();
	//	FunctionInfo* functionInfo = threadInfo->GetFunctionInfo( functionId );
	//	threadInfo->GetStackInfo()->PushFunction( functionInfo, rdtsc() );
	//}
}

void Profiler::ManagedToUnmanagedCall( FunctionID functionId )
{
	Leave(functionId);
	//if(statistical)
	//{
	//	Leave(functionId);
	//}
	//else
	//{
	//	GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
	//}
}

/** No descriptions */
void Profiler::ThreadStart( ThreadID threadId )
{
  //cout << "ThreadStart( " << threadId << " )" << endl;
  threadCollection.GetThreadInfo( threadId )->Start();
  ProfilerSocket ps;
  ps.SendThreadCreate( threadId );
}

void Profiler::ThreadMap( ThreadID threadId, DWORD dwOSThread )
{
  //cout << "ThreadMap( " << threadId << ", " << dwOSThread << ")" << endl;
  threadMap[ dwOSThread ] = threadId;
}

/** No descriptions */
void Profiler::ThreadEnd( ThreadID threadId )
{
	DebugBreak();
	Sleep(1000000);
  threadCollection.EndThread( profilerHelper, threadId );
  //cout << "ThreadEnd( " << threadId << " )" << endl;
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
	//DebugBreak();
  cout << "End()" << endl;
	timeKillEvent(timer);

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
void Profiler::Activate()
{
	 profilerInfo->SetEventMask(
		 COR_PRF_MONITOR_THREADS
		 |COR_PRF_ENABLE_STACK_SNAPSHOT
		 //|COR_PRF_MONITOR_ENTERLEAVE
		 );
	//profilerInfo->SetEventMask(
	//	COR_PRF_MONITOR_THREADS	
	//	|COR_PRF_DISABLE_INLINING
	//	|COR_PRF_ENABLE_STACK_SNAPSHOT
	//	|COR_PRF_MONITOR_SUSPENDS
	//	|COR_PRF_MONITOR_ENTERLEAVE
	//	|COR_PRF_MONITOR_EXCEPTIONS
	//	|COR_PRF_MONITOR_APPDOMAIN_LOADS
	//	|COR_PRF_MONITOR_ASSEMBLY_LOADS
	//	|COR_PRF_MONITOR_CACHE_SEARCHES
	//	|COR_PRF_MONITOR_JIT_COMPILATION
	//	|COR_PRF_MONITOR_CODE_TRANSITIONS
	//);
}
void Profiler::Deactivate()
{
	 //profilerInfo->SetEventMask(
		//COR_PRF_MONITOR_THREADS
	 //|COR_PRF_ENABLE_STACK_SNAPSHOT
	 //|COR_PRF_MONITOR_ENTERLEAVE
	 //);
	//profilerInfo->SetEventMask(
	//	COR_PRF_MONITOR_THREADS	
	//	|COR_PRF_DISABLE_INLINING
	//	|COR_PRF_ENABLE_STACK_SNAPSHOT
	//	|COR_PRF_MONITOR_SUSPENDS
	//	//|COR_PRF_MONITOR_ENTERLEAVE
	//	|COR_PRF_MONITOR_EXCEPTIONS
	//	|COR_PRF_MONITOR_APPDOMAIN_LOADS
	//	|COR_PRF_MONITOR_ASSEMBLY_LOADS
	//	|COR_PRF_MONITOR_CACHE_SEARCHES
	//	|COR_PRF_MONITOR_JIT_COMPILATION
	//	|COR_PRF_MONITOR_CODE_TRANSITIONS
	//);

	//profilerInfo->SetEventMask(
	//COR_PRF_MONITOR_THREADS	|
	//COR_PRF_DISABLE_INLINING |
	//COR_PRF_ENABLE_STACK_SNAPSHOT//|
	//COR_PRF_MONITOR_SUSPENDS 
	//|    
	//COR_PRF_MONITOR_ENTERLEAVE |
	//COR_PRF_MONITOR_EXCEPTIONS |  
	//COR_PRF_MONITOR_APPDOMAIN_LOADS |
	//COR_PRF_MONITOR_ASSEMBLY_LOADS |
	//COR_PRF_MONITOR_CACHE_SEARCHES |
	//COR_PRF_MONITOR_JIT_COMPILATION //| 
	//COR_PRF_MONITOR_CODE_TRANSITIONS
	//);
}