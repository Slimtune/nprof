/***************************************************************************
                          threadinfocollection.cpp  -  description
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
#include "threadinfocollection.h"

ThreadInfoCollection::ThreadInfoCollection()
{
}

ThreadInfoCollection::~ThreadInfoCollection()
{
}

void ThreadInfoCollection::EndAll( ProfilerHelper& ph )
{
  for ( map< ThreadID, ThreadInfo* >::iterator it = _mThreadInfo.begin(); it != _mThreadInfo.end(); it++ )
    if ( it->second->IsRunning() )
      EndThread( ph, it->first );
}

void ThreadInfoCollection::EndThread( ProfilerHelper& ph, ThreadID tid )
{
  ThreadInfo* pThreadInfo = GetThreadInfo( tid );
  pThreadInfo->End();
  ProfilerSocket ps;
  ps.SendThreadEnd( tid, pThreadInfo->_llStartTime, pThreadInfo->_llEndTime );
  Dump( ph, tid );
}

ThreadInfo* ThreadInfoCollection::GetThreadInfo( ThreadID tid )
{
  map< ThreadID, ThreadInfo* >::iterator found = _mThreadInfo.find( tid );
  if ( found == _mThreadInfo.end() )
  {
    ThreadInfo* pThreadInfo = new ThreadInfo();
    _mThreadInfo.insert( make_pair( tid, pThreadInfo ) );
    return pThreadInfo;
  }
  
  return found->second;
}

/** No descriptions */
void ThreadInfoCollection::Trace( ProfilerHelper& ph )
{
  for ( map< ThreadID, ThreadInfo* >::iterator it = _mThreadInfo.begin(); it != _mThreadInfo.end(); it++ )
  {
    cout << "Thread ID " << it->first << ":" << endl;
    it->second->Trace( ph );
  }
}

void ThreadInfoCollection::DumpAll( ProfilerHelper& ph )
{
  for ( map< ThreadID, ThreadInfo* >::iterator it = _mThreadInfo.begin(); it != _mThreadInfo.end(); it++ )
  {
    if ( it->second->IsRunning() )
    {
      ProfilerSocket ps;
      ps.SendStartFunctionData( it->first );
      it->second->Dump( ps, ph );
      ps.SendEndFunctionData();
    }
  }
}

void ThreadInfoCollection::Dump( ProfilerHelper& ph, ThreadID tid )
{
  ProfilerSocket ps;
  ps.SendStartFunctionData( tid );
  GetThreadInfo( tid )->Dump( ps, ph );
  ps.SendEndFunctionData();
}
