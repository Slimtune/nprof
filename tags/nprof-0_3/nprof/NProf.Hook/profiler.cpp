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

Profiler::Profiler( ICorProfilerInfo* pPrfInfo )
{
  _pPrfInfo = pPrfInfo;
  _phHelper.Initialize( _pPrfInfo );
}

Profiler::~Profiler()
{
}

/** No descriptions */
void Profiler::Enter( FunctionID fid )
{
  FunctionInfo* pFunctionInfo = GetCurrentThreadInfo()->GetFunctionInfo( fid );
  pFunctionInfo->nCalls++;
  
  GetCurrentThreadInfo()->GetStackInfo()->PushFunction( pFunctionInfo, rdtsc() );
}

/** No descriptions */
void Profiler::Leave( FunctionID fid )
{
  INT64 ll = GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

/** No descriptions */
void Profiler::TailCall( FunctionID fid )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

void Profiler::UnmanagedToManagedCall( FunctionID fid )
{
  FunctionInfo* pFunctionInfo = GetCurrentThreadInfo()->GetFunctionInfo( fid );
  pFunctionInfo->nCalls++;
  GetCurrentThreadInfo()->GetStackInfo()->PushFunction( pFunctionInfo, rdtsc() );
}

void Profiler::ManagedToUnmanagedCall( FunctionID fid )
{
  GetCurrentThreadInfo()->GetStackInfo()->PopFunction( rdtsc() );
}

/** No descriptions */
void Profiler::ThreadStart( ThreadID tid )
{
  cout << "ThreadStart( " << tid << " )" << endl;
  _tic.GetThreadInfo( tid )->Start();
  ProfilerSocket ps;
  ps.SendThreadCreate( tid );
}

void Profiler::ThreadMap( ThreadID tid, DWORD dwOSThread )
{
  cout << "ThreadMap( " << tid << ", " << dwOSThread << ")" << endl;
  _mThreadMap[ dwOSThread ] = tid;
}

/** No descriptions */
void Profiler::ThreadEnd( ThreadID tid )
{
  _tic.EndThread( _phHelper, tid );
  cout << "ThreadEnd( " << tid << " )" << endl;
}

/** No descriptions */
void Profiler::ThreadSuspend()
{
  cout << "ThreadSuspend( " << GetCurrentThreadID() << " )" << endl;
  _tic.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->SuspendFunction( rdtsc() );
}

/** No descriptions */
void Profiler::ThreadResume()
{
  cout << "ThreadResume( " << GetCurrentThreadID() << " )" << endl;
  _tic.GetThreadInfo( GetCurrentThreadID() )->GetStackInfo()->ResumeFunction( rdtsc() );
}

void Profiler::AppDomainStart( AppDomainID aid )
{
  cout << "AppDomain Created: " << aid << endl;
  ProfilerSocket ps;
  ps.SendAppDomainCreate( aid );
}

void Profiler::AppDomainEnd( AppDomainID aid )
{
}

void Profiler::End()
{
  cout << "End()" << endl;
  _tic.EndAll( _phHelper );
}

/** No descriptions */
ThreadID Profiler::GetCurrentThreadID()
{
  return _phHelper.GetCurrentThreadID();
}

/** No descriptions */
ThreadInfo* Profiler::GetCurrentThreadInfo()
{
  return _tic.GetThreadInfo( GetCurrentThreadID() );
}

/** No descriptions */
void Profiler::Trace()
{
  _tic.Trace( _phHelper );
}
