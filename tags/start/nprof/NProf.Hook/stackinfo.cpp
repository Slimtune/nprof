/***************************************************************************
                          stackinfo.cpp  -  description
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
#include "stackinfo.h"

StackInfo::StackInfo()
{
}

StackInfo::~StackInfo()
{
}

/** No descriptions */
void StackInfo::PushFunction( FunctionInfo* pFunctionInfo, INT64 llCycleCount )
{
  _sFunctionStack.push( StackEntryInfo( pFunctionInfo, llCycleCount ) );
}

/** No descriptions */
INT64 StackInfo::PopFunction( INT64 llCycleCount )
{
  INT64 llElapsed = llCycleCount - _sFunctionStack.top().llCycleStart;
  FunctionID fidCallee = _sFunctionStack.top().pFunctionInfo->fid;
  _sFunctionStack.top().pFunctionInfo->llCycleCount += llElapsed;
  _sFunctionStack.pop();

  if ( _sFunctionStack.size() > 0 )
  {
    CalleeFunctionInfo* pFunctionInfo = _sFunctionStack.top().pFunctionInfo->GetCalleeFunctionInfo( fidCallee );
    pFunctionInfo->llCycleCount += llElapsed;
    pFunctionInfo->nCalls ++;
  }
  
  return llElapsed;
}

/** No descriptions */
void StackInfo::SuspendFunction( INT64 llCycleCount )
{
  _llSuspendStart = llCycleCount;
  if ( _sFunctionStack.size() == 0 ) 
  {
    cout << "Suspend with no call stack!" << endl;
    return;
  }
  cout << "Suspended function ID: " << _sFunctionStack.top().pFunctionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::ResumeFunction( INT64 llCycleCount )
{
  INT64 llElapsed = llCycleCount - _llSuspendStart;
  // Resume with no call stack, ignore
  if ( _sFunctionStack.size() == 0 ) 
  {
    cout << "Resume with no call stack!" << endl;
    return;
  }
  _sFunctionStack.top().pFunctionInfo->llSuspendCycleCount += llElapsed;
  cout << "Resumed function ID: " << _sFunctionStack.top().pFunctionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::Trace()
{
  cout << "Stack depth = " << _sFunctionStack.size() << endl;
}
