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

FunctionInfo::FunctionInfo( FunctionID functionId )
{
  this->cycleCount = -1;
  this->recursiveCycleCount = -1;
  this->suspendCycleCount = 0;
  this->calls = 0;
  this->recursiveCount = 0;
  this->functionId = functionId;
}

FunctionInfo::~FunctionInfo()
{
}

CalleeFunctionInfo* FunctionInfo::GetCalleeFunctionInfo( FunctionID functionId )
{
  map< FunctionID, CalleeFunctionInfo* >::iterator found = calleeMap.find( functionId );
  if ( found == calleeMap.end() )
  {
    CalleeFunctionInfo* functionInfo = new CalleeFunctionInfo();
    calleeMap.insert( make_pair( functionId, functionInfo ) );
    return functionInfo;
  }
  
  return found->second;
}

/** No descriptions */
void FunctionInfo::Trace( ProfilerHelper& profilerHelper )
{
  cout << "  Calls: " << calls << endl;
  cout << "  Time: " << cycleCount << endl;
  cout << "  Avg. time: " << cycleCount / calls << endl;

  for ( map< FunctionID, CalleeFunctionInfo* >::iterator i = calleeMap.begin(); i != calleeMap.end(); i++ )
  {
    cout << "  Callee Function ID " << i->first << ":" << endl;
    cout << "    Calls: " << i->second->calls << endl;
    cout << "    Time: " << i->second->cycleCount << endl;
    cout << "    Avg. time: " << i->second->cycleCount / i->second->calls << endl;
  }
}    

void FunctionInfo::Dump( ProfilerSocket& ps, ProfilerHelper& profilerHelper )
{
  ps.SendFunctionTimingData( calls, cycleCount, recursiveCycleCount, suspendCycleCount );
  for ( map< FunctionID, CalleeFunctionInfo* >::iterator i = calleeMap.begin(); i != calleeMap.end(); i++ )
  {
    ps.SendCalleeFunctionData( i->first, i->second->calls, i->second->cycleCount, i->second->recursiveCycleCount );
  }
  ps.SendEndCalleeFunctionData();
}
