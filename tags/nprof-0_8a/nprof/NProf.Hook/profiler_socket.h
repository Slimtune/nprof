#pragma once

#include "stdafx.h"
#include "profiler_helper.h"

enum NetworkMessage
{
	INITIALIZE = 0,
	SHUTDOWN,
	APPDOMAIN_CREATE,
	THREAD_CREATE,
	THREAD_END,
	FUNCTION_DATA,
	PROFILER_MESSAGE,
};

const int NETWORK_PROTOCOL_VERSION = 2;

class ProfilerSocket
{
public:
  ProfilerSocket(void);
  ~ProfilerSocket(void);
  static void Initialize();
  void SendShutdown();
  void SendInitialize();
  void SendAppDomainCreate( AppDomainID aid );
  void SendThreadCreate( ThreadID tid );
  void SendThreadEnd( ThreadID tid, UINT64 llThreadStartTime, UINT64 llThreadEndTime );
  void SendStartFunctionData( ThreadID tid );
  void SendFunctionData( ProfilerHelper& ph, FunctionID fid );
  void SendProfilerMessage( const string& strMessage );
  void SendFunctionTimingData( int nCalls, UINT64 llCycleCount, UINT64 llRecursiveCycleCount, UINT64 llSuspendCycleCount );
  void SendCalleeFunctionData( FunctionID fid, int nCalls, UINT64 llCycleCount, UINT64 llRecursiveCycleCount );
  void SendEndFunctionData();
  void SendEndCalleeFunctionData();
  void HandleError( const char* szCaller, int nError );
  void HandleWrongSentLength( const char* szCaller, int nExpected, int nSent );
  void HandleWrongRecvLength( const char* szCaller, int nExpected, int nSent );
  int ReadByte( BYTE& b );

private:
  inline void SendBool( bool b );
  inline void SendUINT32( UINT32 ui );
  inline void SendUINT64( UINT64 ui );
  inline void SendString( const string& strFunctionSignature );
  inline void SendNetworkMessage( NetworkMessage nm );
  inline void SendAppDomainID( AppDomainID aid );
  inline void SendThreadID( ThreadID tid );
  inline void SendFunctionID( FunctionID fid );

private:
  SOCKET _s;
  const char* _szOperation;
};
