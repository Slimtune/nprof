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
