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

StackEntryInfo::StackEntryInfo( FunctionInfo* functionInfo, INT64 cycleStart )
{
  this->functionInfo = functionInfo;
  this->cycleStart = cycleStart;
}

StackEntryInfo::StackEntryInfo( const StackEntryInfo& stackEntry )
{
  this->functionInfo = stackEntry.functionInfo;
  this->cycleStart = stackEntry.cycleStart;
}

StackEntryInfo::~StackEntryInfo()
{
}
