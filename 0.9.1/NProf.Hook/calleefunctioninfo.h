/***************************************************************************
                          calleefunctioninfo.h  -  description
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

#ifndef CALLEEFUNCTIONINFO_H
#define CALLEEFUNCTIONINFO_H

#include "stdafx.h"

/**
  *@author Matthew Mastracci
  */

class CalleeFunctionInfo {
public: 
	CalleeFunctionInfo();
	~CalleeFunctionInfo();

  INT64 llCycleCount;
  INT64 llRecursiveCycleCount;
  int nCalls;
  int nRecursiveCount;
};

#endif
