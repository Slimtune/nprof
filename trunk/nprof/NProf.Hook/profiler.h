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
#include "threadinfocollection.h"
#include "profiler_helper.h"

using namespace std;

/**
  *@author Matthew Mastracci
  */

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

  /** No descriptions */
  void Trace();
  CComPtr< ICorProfilerInfo > profilerInfo;

private:
  ThreadID GetCurrentThreadID();
  ThreadInfo* GetCurrentThreadInfo();

  ThreadInfoCollection threadCollection;
  ProfilerHelper profilerHelper;
  map< DWORD, ThreadID > threadMap;
};

#endif
