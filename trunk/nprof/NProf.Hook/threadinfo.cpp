/***************************************************************************
                          threadinfo.cpp  -  description
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
#include "threadinfo.h"
#include "profiler_socket.h"

ThreadInfo::ThreadInfo()
{
  _bRunning = false;
}

ThreadInfo::~ThreadInfo()
{
}

void ThreadInfo::Start()
{
  _llStartTime = rdtsc();
  _bRunning = true;
}

void ThreadInfo::End()
{
  _llEndTime = rdtsc();
  _bRunning = false;
}

bool ThreadInfo::IsRunning()
{
  return _bRunning;
}

StackInfo* ThreadInfo::GetStackInfo()
{
  return &_si;
}

FunctionInfo* ThreadInfo::GetFunctionInfo( FunctionID fid )
{
  if ( _mFunctionInfo.find( fid ) == _mFunctionInfo.end() )
  {
    FunctionInfo* pFunctionInfo = new FunctionInfo( fid );
    _mFunctionInfo.insert( make_pair( fid, pFunctionInfo ) );
    return pFunctionInfo;
  }

  return _mFunctionInfo[ fid ];
}

/** No descriptions */
void ThreadInfo::Trace( ProfilerHelper& ph )
{
  for ( map< FunctionID, FunctionInfo* >::iterator it = _mFunctionInfo.begin(); it != _mFunctionInfo.end(); it++ )
  {
    cout << "Function ID " << it->first << ":" << endl;
    //cout << ph.GetFunctionSignature( it->first );
    it->second->Trace( ph );
  }
}

void ThreadInfo::Dump( ProfilerSocket& ps, ProfilerHelper& ph )
{
  for ( map< FunctionID, FunctionInfo* >::iterator it = _mFunctionInfo.begin(); it != _mFunctionInfo.end(); it++ )
  {
    ps.SendFunctionData( ph, it->first );
    it->second->Dump( ps, ph );
  }
}
