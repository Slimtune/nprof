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

  ThreadInfo* GetThreadInfo( ThreadID tid );
  /** No descriptions */
  void EndAll( ProfilerHelper& ph );
  void EndThread( ProfilerHelper& ph, ThreadID tid );
  void DumpAll( ProfilerHelper& ph );
  void Dump( ProfilerHelper& ph, ThreadID tid );
  void Trace( ProfilerHelper& ph );
private:
  map< ThreadID, ThreadInfo* > _mThreadInfo;
};

#endif
