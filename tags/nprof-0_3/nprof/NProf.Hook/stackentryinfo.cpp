/***************************************************************************
                          stackentryinfo.cpp  -  description
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
#include "stackentryinfo.h"

StackEntryInfo::StackEntryInfo( FunctionInfo* pFunctionInfo, INT64 llCycleStart )
{
  this->pFunctionInfo = pFunctionInfo;
  this->llCycleStart = llCycleStart;
}

StackEntryInfo::StackEntryInfo( const StackEntryInfo& rhs )
{
  pFunctionInfo = rhs.pFunctionInfo;
  llCycleStart = rhs.llCycleStart;
}

StackEntryInfo::~StackEntryInfo()
{
}
