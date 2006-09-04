//// stdafx.cpp : source file that includes just the standard includes
//// NProf.Hook.pch will be the pre-compiled header
//// stdafx.obj will contain the pre-compiled type information
//
//#include "stdafx.h"
//#ifdef WIN32
//  __declspec(naked) UINT64 __fastcall rdtsc()
//  {
//    __asm
//    {
//      rdtsc;
//      ret;
//    }
//  }
//#else
//  __inline__ unsigned UINT64 int rdtsc(void)
//  {
//    unsigned long long int x;
//    __asm__ volatile (".byte 0x0f, 0x31" : "=A" (x));
//    return x;
//  }
//#endif
//
