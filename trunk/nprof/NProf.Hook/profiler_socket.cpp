////#include "stdafx.h"
////#include "profiler_socket.h"
//#include "profiler.h"
//
//#define SAFE_SEND( socket, data ) \
//	{ \
//		int sent = send( socket, ( const char * )&data, sizeof( data ), 0 ); \
//		if ( sent != sizeof( data ) ) \
//		{ \
//			HandleError( "SAFE_SEND", WSAGetLastError() ); \
//			HandleWrongSentLength( "SAFE_SEND", sizeof( data ), sent ); \
//		} \
//	}
//
//#define SAFE_SEND_RAW( socket, data, length ) \
//	{ \
//		int sent = send( socket, data, length, 0 ); \
//		if ( sent != length ) \
//		{ \
//			HandleError( "SAFE_SEND_RAW", WSAGetLastError() ); \
//			HandleWrongSentLength( "SAFE_SEND_RAW", sizeof( data ), sent ); \
//		} \
//	}
//
//#define SAFE_READ( socket, data, result ) \
//	{ \
//		int nRecv = recv( socket, ( char * )&data, sizeof( data ), 0 ); \
//		if ( nRecv != sizeof( data ) ) \
//		{ \
//			HandleError( "SAFE_READ", WSAGetLastError() ); \
//			HandleWrongRecvLength( "SAFE_READ", sizeof( data ), nRecv ); \
//			result = 1; \
//		} \
//		else \
//		{ \
//			result = 0; \
//		} \
//	}
//
//bool ProfilerSocket::isInitialized = false;
//int ProfilerSocket::applicationId = -1;
//
//ProfilerSocket::ProfilerSocket()
//{
//	this->operation = "ctor";
//	this->isApplicationIdSent = false;
//
//	sockaddr_in sa;
//	sa.sin_family = AF_INET;
//	sa.sin_port = htons( atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) );
//	sa.sin_addr.S_un.S_addr = inet_addr( "127.0.0.1" );
//
//	socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, 0);
//	if ( socket == INVALID_SOCKET )
//	{
//		HandleError( "WSASocket", WSAGetLastError() );
//	}
//	else
//	{
//		int n = 0;
//		while ( n < 10 )
//		{
//			int nResult = connect( socket, ( const sockaddr * )&sa, sizeof( sa ) );
//			if ( nResult == SOCKET_ERROR )
//			{
//				HandleError( "connect", WSAGetLastError() );
//			}
//			else
//			{
//				return;
//			}
//
//			Sleep( 200 );
//			n++;
//		}
//	}
//	this->socket = INVALID_SOCKET;
//}
//
//void ProfilerSocket::Initialize()
//{
//	WSADATA wsaData;
//	WSAStartup( MAKEWORD( 2, 2 ), &wsaData );
//	cout << "WSAStartup: " << WSAGetLastError() << endl;
//	cout << "port = " << atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) << endl;
//	ProfilerSocket ps;
//	ps.SendInitialize();
//}
//
//void ProfilerSocket::SendInitialize()
//{
//	operation = "SendInitialize";
//
//	SendNetworkMessage( INITIALIZE );
//	SendUINT32( NETWORK_PROTOCOL_VERSION );
//	BYTE b;
//	if ( ReadByte( b ) == 0 )
//	{
//		if ( b == 2 )
//		{
//			b = 1;
//			::DebugBreak();
//		}
//
//		if ( b == 1 )
//		{
//			cout << "Successfully initialized profiler socket!" << endl;
//			ReadByte( b );
//			applicationId = b;
//
//			cout << "Application ID = " << applicationId << endl;
//
//			SendUINT32( ::GetCurrentProcessId() );
//			
//			int nArgs;
//			LPWSTR* pstrCmdLine = ::CommandLineToArgvW( ::GetCommandLineW(), &nArgs );
//
//			SendUINT32( nArgs );
//
//			for ( int nArg = 0; nArg < nArgs; nArg++ )
//			{
//				string strArg = CW2A( pstrCmdLine[ nArg ] );
//				SendString( strArg );
//			}
//			
//			::GlobalFree( pstrCmdLine );
//
//			isInitialized = true;
//		}
//		else
//		{
//			cout << "We weren't allowed to initialize!" << endl;
//			isInitialized = false;
//		}
//	}
//	else
//	{
//		cout << "Could not initialize!" << endl;
//		isInitialized = false;
//	}
//}
//
//void ProfilerSocket::SendShutdown()
//{
//	if ( !isInitialized )
//		return;
//	operation = "SendShutdown";
//
//	SendNetworkMessage( SHUTDOWN );
//}
//
//void ProfilerSocket::SendAppDomainCreate( AppDomainID aid )
//{
//	operation = "SendAppDomainCreate";
//
//	SendNetworkMessage( APPDOMAIN_CREATE );
//	SendAppDomainID( aid );
//}
//
//void ProfilerSocket::SendThreadCreate( ThreadID tid )
//{
//	operation = "SendThreadCreate";
//
//	SendNetworkMessage( THREAD_CREATE );
//	SendThreadID( tid );
//}
//
//void ProfilerSocket::SendThreadEnd( ThreadID tid, UINT64 llThreadStartTime, UINT64 threadEndTime )
//{
//	operation = "SendThreadEnd";
//
//	SendNetworkMessage( THREAD_END );
//	SendThreadID( tid );
//	SendUINT64( llThreadStartTime );
//	SendUINT64( threadEndTime );
//}
//
//void ProfilerSocket::SendStartFunctionData( ThreadID tid )
//{
//	operation = "SendStartFunctionData";
//
//	SendNetworkMessage( FUNCTION_DATA );
//	SendThreadID( tid );
//}
//
//void ProfilerSocket::SendFunctionData( ProfilerHelper& ph, FunctionID fid )
//{
//	operation = "SendFunctionData";
//
//	SendFunctionID( fid );
//	string returnType, className, functionName, parameters;
//	UINT32 methodAttributes;
//	ph.GetFunctionSignature( fid, methodAttributes, returnType, className, functionName, parameters );
//
//	SendUINT32( methodAttributes );
//	SendString( returnType );
//	SendString( className );
//	SendString( functionName );
//	SendString( parameters );
//}
//
//void ProfilerSocket::SendProfilerMessage( const string& strMessage )
//{
//	operation = "SendProfilerMessage";
//
//	SendNetworkMessage( PROFILER_MESSAGE );
//	SendString( strMessage );
//}
//
//void ProfilerSocket::SendFunctionTimingData( int calls, UINT64 cycleCount, UINT64 recursiveCycleCount, UINT64 suspendCycleCount )
//{
//	operation = "SendFunctionTimingData";
//
//	SendUINT32( calls );
//	SendUINT64( cycleCount );
//	SendUINT64( recursiveCycleCount );
//	SendUINT64( suspendCycleCount );
//}
//
//void ProfilerSocket::SendCalleeFunctionData( FunctionID fid, int calls, UINT64 cycleCount, UINT64 recursiveCycleCount )
//{
//	operation = "SendCalleeFunctionData";
//
//	SendFunctionID( fid );
//	SendUINT32( calls );
//	SendUINT64( cycleCount );
//	SendUINT64( recursiveCycleCount );
//}
//
//void ProfilerSocket::SendEndFunctionData()
//{
//	operation = "SendEndFunctionData";
//
//	SendFunctionID( 0xffffffff );
//}
//
//void ProfilerSocket::SendEndCalleeFunctionData()
//{
//	operation = "SendEndCalleeFunctionData";
//
//	SendFunctionID( 0xffffffff );
//}
//
//void ProfilerSocket::SendBool( bool boolean )
//{
//	SAFE_SEND( socket, boolean );
//}
//
//void ProfilerSocket::SendUINT32( UINT32 integer )
//{
//	SAFE_SEND( socket, integer );
//}
//
//void ProfilerSocket::SendUINT64( UINT64 integer)
//{
//	SAFE_SEND( socket, integer);
//}
//
//void ProfilerSocket::SendString( const string& signature )
//{
//	SendUINT32( ( UINT32 )signature.length() );
//	SAFE_SEND_RAW( socket, signature.c_str(), ( int )signature.length() );
//}
//
//void ProfilerSocket::SendNetworkMessage( NetworkMessage networkMessage )
//{
//	UINT16 mess = networkMessage;
//	SAFE_SEND( socket, mess );
//
//	if ( !isApplicationIdSent)
//	{
//		if ( networkMessage != INITIALIZE )
//			SendUINT32( applicationId );
//
//		isApplicationIdSent = true;
//	}
//}
//
//void ProfilerSocket::SendAppDomainID( AppDomainID appDomainId )
//{
//	SAFE_SEND( socket, appDomainId );
//}
//
//void ProfilerSocket::SendThreadID( ThreadID threadId )
//{
//	SAFE_SEND( socket, threadId );
//}
//
//void ProfilerSocket::SendFunctionID( FunctionID functionId )
//{
//	SAFE_SEND( socket, functionId );
//}
//
//int ProfilerSocket::ReadByte( BYTE& b )
//{
//	int result;
//	SAFE_READ( socket, b, result );
//
//	return result;
//}
//
//void ProfilerSocket::HandleError( const char* caller, int error )
//{
////	std::ofstream outf;
////	outf.open( "c:\\nprof.log", ios::app );
////	outf << operation << " " << caller << " Error: " << nError << " on socket " << _s << endl;
//}
//
//void ProfilerSocket::HandleWrongSentLength( const char* caller, int expected, int sent )
//{
////	std::ofstream outf;
////	outf.open( "c:\\nprof.log", ios::app );
////	outf << operation << " " << caller << " Expected to send " << expected << ", sent " << sent << " instead." << endl;
//}
//
//void ProfilerSocket::HandleWrongRecvLength( const char* caller, int expected, int sent )
//{
////	std::ofstream outf;
////	outf.open( "c:\\nprof.log", ios::app );
////	outf << operation << " " << caller << " Expected to send " << expected << ", sent " << sent << " instead." << endl;
//}
//
//ProfilerSocket::~ProfilerSocket(void)
//{
//	closesocket( socket );
//}
