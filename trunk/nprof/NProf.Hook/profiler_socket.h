//#pragma once
//
//#include "stdafx.h"
//#include "profiler_helper.h"
//
//enum NetworkMessage
//{
//	INITIALIZE = 0,
//	SHUTDOWN,
//	APPDOMAIN_CREATE,
//	THREAD_CREATE,
//	THREAD_END,
//	FUNCTION_DATA,
//	PROFILER_MESSAGE,
//};
//
//const int NETWORK_PROTOCOL_VERSION = 3;
//
//class ProfilerSocket
//{
//public:
//  ProfilerSocket();
//  ~ProfilerSocket(void);
//  static void Initialize();
//  void SendShutdown();
//  void SendInitialize();
//  void SendAppDomainCreate( AppDomainID appDomainId );
//  void SendThreadCreate( ThreadID threadId );
//  void SendThreadEnd( ThreadID threadId, UINT64 threadStartTime, UINT64 threadEndTime );
//  void SendStartFunctionData( ThreadID threadId );
//  void SendFunctionData( ProfilerHelper& profilerHelper, FunctionID functionId );
//  void SendProfilerMessage( const string& message );
//  void SendFunctionTimingData( int calls, UINT64 cycleCount, UINT64 recursiveCycleCount, UINT64 suspendCycleCount );
//  void SendCalleeFunctionData( FunctionID functionId, int calls, UINT64 cycleCount, UINT64 recursiveCycleCount );
//  void SendEndFunctionData();
//  void SendEndCalleeFunctionData();
//  void HandleError( const char* caller, int errorNumber );
//  void HandleWrongSentLength( const char* caller, int expected, int sent );
//  void HandleWrongRecvLength( const char* caller, int expected, int sent );
//  int ReadByte( BYTE& b );
//
//private:
//  inline void SendBool( bool boolean );
//  inline void SendUINT32( UINT32 integer );
//  inline void SendUINT64( UINT64 integer );
//  inline void SendString( const string& functionSignature );
//  inline void SendNetworkMessage( NetworkMessage networkMessage );
//  inline void SendAppDomainID( AppDomainID appDomainId );
//  inline void SendThreadID( ThreadID threadId );
//  inline void SendFunctionID( FunctionID functionId );
//
//private:
//  SOCKET socket;
//  const char* operation;
//  bool isApplicationIdSent;
//
//  static bool isInitialized;
//  static int applicationId;
//};
