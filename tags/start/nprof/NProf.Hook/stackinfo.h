/***************************************************************************
                          stackinfo.h  -  description
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

#ifndef STACKINFO_H
#define STACKINFO_H

#include "stdafx.h"
#include "functioninfo.h"
#include "stackentryinfo.h"
 
using namespace std;

/**
  *@author Matthew Mastracci
  */

class StackInfo {
public: 
	StackInfo();
	~StackInfo();
  
  /** No descriptions */
  INT64 PopFunction( INT64 llCycleCount );
  /** No descriptions */
  void PushFunction( FunctionInfo* pFunctionInfo, INT64 llCycleCount );
  /** No descriptions */
  void Trace();

  void SuspendFunction( INT64 llCycleCount );
  void ResumeFunction( INT64 llCycleCount );
private:
  INT64 _llSuspendStart;
  stack< StackEntryInfo > _sFunctionStack;
};

#endif
