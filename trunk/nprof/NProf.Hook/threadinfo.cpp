///***************************************************************************
//                          threadinfo.cpp  -  description
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
////#include "stdafx.h"
////#include "threadinfo.h"
////#include "profiler_socket.h"
//#include "profiler.h"
//
//ThreadInfo::ThreadInfo()
//{
//  this->isRunning = false;
//  this->suspendTime = 0;
//  this->stackInfo = new StackInfo( this );
//}
//
//ThreadInfo::~ThreadInfo()
//{
//	delete stackInfo;
//}
//
//void ThreadInfo::Start()
//{
//  startTime = rdtsc();
//  isRunning = true;
//}
//
//void ThreadInfo::End()
//{
//  endTime = rdtsc();
//  isRunning = false;
//}
//
//bool ThreadInfo::IsRunning()
//{
//  return isRunning;
//}
//
//StackInfo* ThreadInfo::GetStackInfo()
//{
//  return stackInfo;
//}
//
//FunctionInfo* ThreadInfo::GetFunctionInfo( FunctionID functionId )
//{
//  map< FunctionID, FunctionInfo* >::iterator found = functionMap.find( functionId );
//  if ( found == functionMap.end() )
//  {
//    FunctionInfo* functionInfo = new FunctionInfo( functionId );
//    functionMap.insert( make_pair( functionId, functionInfo ) );
//    return functionInfo;
//  }
//
//  return found->second;
//}
//
///** No descriptions */
//void ThreadInfo::Trace( ProfilerHelper& profilerHelper )
//{
//  for ( map< FunctionID, FunctionInfo* >::iterator i = functionMap.begin(); i != functionMap.end(); i++ )
//  {
//    cout << "Function ID " << i->first << ":" << endl;
//    //cout << ph.GetFunctionSignature( it->first );
//    i->second->Trace( profilerHelper );
//  }
//}
//
//void ThreadInfo::Dump( ProfilerSocket& ps, ProfilerHelper& profilerHelper )
//{
//  for ( map< FunctionID, FunctionInfo* >::iterator i = functionMap.begin(); i != functionMap.end(); i++ )
//  {
//    ps.SendFunctionData( profilerHelper, i->first );
//    i->second->Dump( ps, profilerHelper );
//  }
//}
