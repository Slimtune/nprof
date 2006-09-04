///***************************************************************************
//                          functioninfo.h  -  description
//                             -------------------
//    begin                : Sat Jan 18 2003
//    copyright            : (C) 2003 by Matthew Mastracci
//    email                : mmastrac@canada.com
// ***************************************************************************/
//
///***************************************************************************
// *                                                                         *
// *   This program is free software; you can redistribute it and/or modify  *
// *   it under the terms of the GNU General Public License as published by  *
// *   the Free Software Foundation; either version 2 of the License, or     *
// *   (at your option) any later version.                                   *
// *                                                                         *
// ***************************************************************************/
//
//#ifndef FUNCTIONINFO_H
//#define FUNCTIONINFO_H
//
//#include "stdafx.h"
//#include "profiler_helper.h"
//#include "calleefunctioninfo.h"
//#include "profiler_socket.h"
//
///**
//  *@author Matthew Mastracci
//  */
//
//class FunctionInfo
//{
//public: 
//	FunctionInfo( FunctionID fid );
//	~FunctionInfo();
//  CalleeFunctionInfo* GetCalleeFunctionInfo( FunctionID fid );
//  /** No descriptions */
//  void Trace( ProfilerHelper& ph );
//  void Dump( ProfilerSocket& ps, ProfilerHelper& ph );
//
//  int calls;
//  int recursiveCount;
//  INT64 cycleCount;
//  INT64 recursiveCycleCount;
//  INT64 suspendCycleCount;
//  FunctionID functionId;
//
//  map< FunctionID, CalleeFunctionInfo* > calleeMap;
//};
//
//#endif
