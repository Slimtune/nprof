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
	Profiler( ICorProfilerInfo* pPrfInfo );
	~Profiler();
  void Leave( FunctionID fid );
  void Enter( FunctionID fid );
  void TailCall( FunctionID fid );
  void UnmanagedToManagedCall( FunctionID fid );
  void ManagedToUnmanagedCall( FunctionID fid );
  void ThreadStart( ThreadID tid );
  void ThreadEnd( ThreadID tid );
  void ThreadSuspend();
  void ThreadResume();
  void AppDomainStart( AppDomainID aid );
  void AppDomainEnd( AppDomainID aid );
  void End();
  void ThreadMap( ThreadID tid, DWORD dwOSThread );

  /** No descriptions */
  void Trace();
private:
  ThreadID GetCurrentThreadID();
  ThreadInfo* GetCurrentThreadInfo();

  ThreadInfoCollection _tic;
  CComPtr< ICorProfilerInfo > _pPrfInfo;
  ProfilerHelper _phHelper;
  map< DWORD, ThreadID > _mThreadMap;
};

#endif
