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
  FunctionInfo* GetFunctionInfo( FunctionID functionId );
  void Trace( ProfilerHelper& ph );
  void Dump( ProfilerSocket& profilerSocket, ProfilerHelper& profilerHelper );
  INT64 startTime;
  INT64 endTime;
  INT64 suspendTime;

  map< FunctionID, FunctionInfo* > functionMap;

  void AddStackFrames(deque<FunctionID>* functions);

private:
  bool  isRunning;
  StackInfo* stackInfo;
};

#endif
