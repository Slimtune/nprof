/***************************************************************************
                          threadinfo.h  -  description
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

#ifndef THREADINFO_H
#define THREADINFO_H

#include "stdafx.h"
#include "profiler_helper.h"
#include "stackinfo.h"

using namespace std;

/**
  *@author Matthew Mastracci
  */

class ThreadInfo
{
public: 
	ThreadInfo();
	~ThreadInfo();

  void Start();
  void End();
  bool IsRunning();
  StackInfo* GetStackInfo();
  FunctionInfo* GetFunctionInfo( FunctionID fid );
  void Trace( ProfilerHelper& ph );
  void Dump( ProfilerSocket& ps, ProfilerHelper& ph );
  INT64 _llStartTime;
  INT64 _llEndTime;
  
private:
  bool  _bRunning;
  StackInfo _si;
  map< FunctionID, FunctionInfo* > _mFunctionInfo;
};

#endif
