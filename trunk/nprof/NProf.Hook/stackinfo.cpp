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
#include "threadinfo.h"

StackInfo::StackInfo( ThreadInfo* pThreadInfo )
{
	_pThreadInfo = pThreadInfo;
}

StackInfo::~StackInfo()
{
}

/** No descriptions */
void StackInfo::PushFunction( FunctionInfo* pFunctionInfo, INT64 llCycleCount )
{
	if ( _sFunctionStack.size() > 0 )
	{
		// Increment the recursive count of this callee function info so we don't double-book entries
		FunctionInfo* pCallerFunctionInfo = _sFunctionStack.top().pFunctionInfo;
		FunctionID fidCallee = pFunctionInfo->fid;
		
		CalleeFunctionInfo* pCalleeFunctionInfo = pCallerFunctionInfo->GetCalleeFunctionInfo( fidCallee );
		pCalleeFunctionInfo->nRecursiveCount++;
		pCalleeFunctionInfo->nCalls++;
	}

	// Increment the recursive count of this function info so we don't double-book entries
	pFunctionInfo->nRecursiveCount++;
	pFunctionInfo->nCalls++;

	_sFunctionStack.push( StackEntryInfo( pFunctionInfo, llCycleCount ) );
}

/** No descriptions */
INT64 StackInfo::PopFunction( INT64 llCycleCount )
{
	INT64 llElapsed = llCycleCount - _sFunctionStack.top().llCycleStart;
	FunctionInfo* pFunctionInfo = _sFunctionStack.top().pFunctionInfo;

	FunctionID fidCallee = pFunctionInfo->fid;

	// Only add the time if we're at the lowest call to the function on the stack
	// Prevents double-accounting of recursive functions
	pFunctionInfo->nRecursiveCount--;
	if ( pFunctionInfo->nRecursiveCount == 0 )
		pFunctionInfo->llCycleCount += llElapsed;
	else
		pFunctionInfo->llRecursiveCycleCount += llElapsed;

	_sFunctionStack.pop();

	if ( _sFunctionStack.size() > 0 )
	{
		CalleeFunctionInfo* pCalleeFunctionInfo = _sFunctionStack.top().pFunctionInfo->GetCalleeFunctionInfo( fidCallee );

		// Only add the time if we're at the lowest call to the function on the stack
		// Prevents double-accounting of recursive functions
		pCalleeFunctionInfo->nRecursiveCount--;
		if ( pCalleeFunctionInfo->nRecursiveCount == 0 )
			pCalleeFunctionInfo->llCycleCount += llElapsed;
		else
			pCalleeFunctionInfo->llRecursiveCycleCount += llElapsed;
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
	//cout << "Suspended function ID: " << _sFunctionStack.top().pFunctionInfo->fid << endl;
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
	_pThreadInfo->_llSuspendTime += llElapsed;
	//cout << "Resumed function ID: " << _sFunctionStack.top().pFunctionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::Trace()
{
	cout << "Stack depth = " << _sFunctionStack.size() << endl;
}
