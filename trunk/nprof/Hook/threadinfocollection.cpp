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

void ThreadInfoCollection::EndAll( ProfilerHelper& profilerHelper )
{
  for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
    if ( i->second->IsRunning() )
      EndThread( profilerHelper, i->first );
}

void ThreadInfoCollection::EndThread( ProfilerHelper& profilerHelper, ThreadID threadId )
{
  ThreadInfo* threadInfo = GetThreadInfo( threadId );
  threadInfo->End();
  ProfilerSocket profilerSocket;
  profilerSocket.SendThreadEnd( threadId, threadInfo->startTime, threadInfo->endTime );
  Dump( profilerHelper, threadId );
}

ThreadInfo* ThreadInfoCollection::GetThreadInfo( ThreadID threadId )
{
  map< ThreadID, ThreadInfo* >::iterator found = threadMap.find( threadId );
  if ( found == threadMap.end() )
  {
    ThreadInfo* threadInfo = new ThreadInfo();
    threadMap.insert( make_pair( threadId, threadInfo ) );
    return threadInfo;
  }
  
  return found->second;
}

/** No descriptions */
void ThreadInfoCollection::Trace( ProfilerHelper& profilerHelper )
{
  for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
  {
    cout << "Thread ID " << i->first << ":" << endl;
    i->second->Trace( profilerHelper );
  }
}

void ThreadInfoCollection::DumpAll( ProfilerHelper& profilerHelper )
{
  for ( map< ThreadID, ThreadInfo* >::iterator i = threadMap.begin(); i != threadMap.end(); i++ )
  {
    if ( i->second->IsRunning() )
    {
      ProfilerSocket profilerSocket;
      profilerSocket.SendStartFunctionData( i->first );
      i->second->Dump( profilerSocket, profilerHelper );
      profilerSocket.SendEndFunctionData();
    }
  }
}

void ThreadInfoCollection::Dump( ProfilerHelper& profilerHelper, ThreadID threadId )
{
  ProfilerSocket profilerSocket;
  profilerSocket.SendStartFunctionData( threadId );
  GetThreadInfo( threadId )->Dump( profilerSocket, profilerHelper );
  profilerSocket.SendEndFunctionData();
}
