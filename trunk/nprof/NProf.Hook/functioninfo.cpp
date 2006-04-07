/***************************************************************************
                          functioninfo.cpp  -  description
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
#include "functioninfo.h"

FunctionInfo::FunctionInfo( FunctionID fid )
{
  llCycleCount = -1;
  llRecursiveCycleCount = -1;
  llSuspendCycleCount = 0;
  nCalls = 0;
  nRecursiveCount = 0;
  this->fid = fid;
}

FunctionInfo::~FunctionInfo()
{
}

CalleeFunctionInfo* FunctionInfo::GetCalleeFunctionInfo( FunctionID fid )
{
  map< FunctionID, CalleeFunctionInfo* >::iterator found = _mCalleeInfo.find( fid );
  if ( found == _mCalleeInfo.end() )
  {
    CalleeFunctionInfo* pFunctionInfo = new CalleeFunctionInfo();
    _mCalleeInfo.insert( make_pair( fid, pFunctionInfo ) );
    return pFunctionInfo;
  }
  
  return found->second;
}

/** No descriptions */
void FunctionInfo::Trace( ProfilerHelper& ph )
{
  cout << "  Calls: " << nCalls << endl;
  cout << "  Time: " << llCycleCount << endl;
  cout << "  Avg. time: " << llCycleCount / nCalls << endl;

  for ( map< FunctionID, CalleeFunctionInfo* >::iterator it = _mCalleeInfo.begin(); it != _mCalleeInfo.end(); it++ )
  {
    cout << "  Callee Function ID " << it->first << ":" << endl;
    //cout << "  " << ph.GetFunctionSignature( it->first ) << endl;
    cout << "    Calls: " << it->second->nCalls << endl;
    cout << "    Time: " << it->second->llCycleCount << endl;
    cout << "    Avg. time: " << it->second->llCycleCount / it->second->nCalls << endl;
  }
}    

void FunctionInfo::Dump( ProfilerSocket& ps, ProfilerHelper& ph )
{
  ps.SendFunctionTimingData( nCalls, llCycleCount, llRecursiveCycleCount, llSuspendCycleCount );
  for ( map< FunctionID, CalleeFunctionInfo* >::iterator it = _mCalleeInfo.begin(); it != _mCalleeInfo.end(); it++ )
  {
    ps.SendCalleeFunctionData( it->first, it->second->nCalls, it->second->llCycleCount, it->second->llRecursiveCycleCount );
  }
  ps.SendEndCalleeFunctionData();
}
