/***************************************************************************
                          stackentryinfo.h  -  description
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

#ifndef STACKENTRYINFO_H
#define STACKENTRYINFO_H

#include "stdafx.h"
#include "functioninfo.h"

/**
  *@author Matthew Mastracci
  */

class StackEntryInfo {
public: 
	StackEntryInfo( FunctionInfo* pFunctionInfo, INT64 llCycleStart );
	StackEntryInfo( const StackEntryInfo& rhs );
	~StackEntryInfo();
  
  INT64 llCycleStart;
  FunctionInfo* pFunctionInfo;
};

#endif
