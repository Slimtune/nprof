/***************************************************************************
                          threadinfocollection.h  -  description
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

#ifndef THREADINFOCOLLECTION_H
#define THREADINFOCOLLECTION_H

#include "stdafx.h"
#include "threadinfo.h"
#include "profiler_helper.h"

using namespace std;

/**
  *@author Matthew Mastracci
  */

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

#endif
