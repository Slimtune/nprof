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

StackInfo::StackInfo( ThreadInfo* threadInfo )
{
	this->threadInfo = threadInfo;
}

StackInfo::~StackInfo()
{
}

/** No descriptions */
void StackInfo::PushFunction( FunctionInfo* functionInfo, INT64 cycleCount )
{
	if ( functionStack.size() > 0 )
	{
		// Increment the recursive count of this callee function info so we don't double-book entries
		FunctionInfo* callerFunction = functionStack.top().functionInfo;
		FunctionID calleeId = functionInfo->functionId;
		
		CalleeFunctionInfo* calleeFunction = callerFunction->GetCalleeFunctionInfo( calleeId );
		calleeFunction->recursiveCount++;
		calleeFunction->calls++;
	}

	// Increment the recursive count of this function info so we don't double-book entries
	functionInfo->recursiveCount++;
	functionInfo->calls++;

	functionStack.push( StackEntryInfo( functionInfo, cycleCount ) );
}

/** No descriptions */
INT64 StackInfo::PopFunction( INT64 cycleCount )
{
	INT64 elapsed = cycleCount - functionStack.top().cycleStart;
	FunctionInfo* functionInfo = functionStack.top().functionInfo;

	FunctionID calleeId = functionInfo->functionId;

	// Only add the time if we're at the lowest call to the function on the stack
	// Prevents double-accounting of recursive functions
	functionInfo->recursiveCount--;
	if ( functionInfo->recursiveCount == 0 )
		functionInfo->cycleCount += elapsed;
	else
		functionInfo->recursiveCycleCount += elapsed;

	functionStack.pop();

	if ( functionStack.size() > 0 )
	{
		CalleeFunctionInfo* calleeFunction = functionStack.top().functionInfo->GetCalleeFunctionInfo( calleeId );

		// Only add the time if we're at the lowest call to the function on the stack
		// Prevents double-accounting of recursive functions
		calleeFunction->recursiveCount--;
		if ( calleeFunction->recursiveCount == 0 )
			calleeFunction->cycleCount += elapsed;
		else
			calleeFunction->recursiveCycleCount += elapsed;
	}

	return elapsed;
}

/** No descriptions */
void StackInfo::SuspendFunction( INT64 cycleCount )
{
	suspendStart = cycleCount;
	if ( functionStack.size() == 0 ) 
	{
		cout << "Suspend with no call stack!" << endl;
		return;
	}
	//cout << "Suspended function ID: " << functionStack.top().functionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::ResumeFunction( INT64 cycleCount )
{
	INT64 elapsed = cycleCount - suspendStart;
	// Resume with no call stack, ignore
	if ( functionStack.size() == 0 ) 
	{
		cout << "Resume with no call stack!" << endl;
		return;
	}
	functionStack.top().functionInfo->suspendCycleCount += elapsed;
	threadInfo->suspendTime += elapsed;
	//cout << "Resumed function ID: " << functionStack.top().functionInfo->fid << endl;
}

/** No descriptions */
void StackInfo::Trace()
{
	cout << "Stack depth = " << functionStack.size() << endl;
}
