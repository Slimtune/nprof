#include "stdafx.h"
#include "profiler_socket.h"

#define SAFE_SEND( socket, data ) \
	{ \
		int nSent = send( socket, ( const char * )&data, sizeof( data ), 0 ); \
		if ( nSent != sizeof( data ) ) \
		{ \
			HandleError( "SAFE_SEND", WSAGetLastError() ); \
			HandleWrongSentLength( "SAFE_SEND", sizeof( data ), nSent ); \
		} \
	}

#define SAFE_SEND_RAW( socket, data, length ) \
	{ \
		int nSent = send( socket, data, length, 0 ); \
		if ( nSent != length ) \
		{ \
			HandleError( "SAFE_SEND_RAW", WSAGetLastError() ); \
			HandleWrongSentLength( "SAFE_SEND_RAW", sizeof( data ), nSent ); \
		} \
	}

#define SAFE_READ( socket, data, result ) \
	{ \
		int nRecv = recv( socket, ( char * )&data, sizeof( data ), 0 ); \
		if ( nRecv != sizeof( data ) ) \
		{ \
			HandleError( "SAFE_READ", WSAGetLastError() ); \
			HandleWrongRecvLength( "SAFE_READ", sizeof( data ), nRecv ); \
			result = 1; \
		} \
		else \
		{ \
			result = 0; \
		} \
	}

bool ProfilerSocket::_bInitialized = false;
int ProfilerSocket::_nApplicationID = -1;

ProfilerSocket::ProfilerSocket()
{
	_szOperation = "ctor";
	_bSentApplicationID = false;

	sockaddr_in sa;
	sa.sin_family = AF_INET;
	sa.sin_port = htons( atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) );
	sa.sin_addr.S_un.S_addr = inet_addr( "127.0.0.1" );

	_s = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, 0);
	if ( _s == INVALID_SOCKET )
	{
		HandleError( "WSASocket", WSAGetLastError() );
	}
	else
	{
		int n = 0;
		while ( n < 10 )
		{
			int nResult = connect( _s, ( const sockaddr * )&sa, sizeof( sa ) );
			if ( nResult == SOCKET_ERROR )
			{
				HandleError( "connect", WSAGetLastError() );
			}
			else
			{
				return;
			}

			Sleep( 200 );
			n++;
		}
	}

	_s = INVALID_SOCKET;
}

void ProfilerSocket::Initialize()
{
	WSADATA wsaData;
	WSAStartup( MAKEWORD( 2, 2 ), &wsaData );
	cout << "WSAStartup: " << WSAGetLastError() << endl;
	cout << "port = " << atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) << endl;
	ProfilerSocket ps;
	ps.SendInitialize();
}

void ProfilerSocket::SendInitialize()
{
	_szOperation = "SendInitialize";

	SendNetworkMessage( INITIALIZE );
	SendUINT32( NETWORK_PROTOCOL_VERSION );
	BYTE b;
	if ( ReadByte( b ) == 0 )
	{
		if ( b == 2 )
		{
			b = 1;
			::DebugBreak();
		}

		if ( b == 1 )
		{
			cout << "Successfully initialized profiler socket!" << endl;
			ReadByte( b );
			_nApplicationID = b;

			cout << "Application ID = " << _nApplicationID << endl;

			SendUINT32( ::GetCurrentProcessId() );
			
			int nArgs;
			LPWSTR* pstrCmdLine = ::CommandLineToArgvW( ::GetCommandLineW(), &nArgs );

			SendUINT32( nArgs );

			for ( int nArg = 0; nArg < nArgs; nArg++ )
			{
				string strArg = CW2A( pstrCmdLine[ nArg ] );
				SendString( strArg );
			}
			
			::GlobalFree( pstrCmdLine );

			_bInitialized = true;
		}
		else
		{
			cout << "We weren't allowed to initialize!" << endl;
			_bInitialized = false;
		}
	}
	else
	{
		cout << "Could not initialize!" << endl;
		_bInitialized = false;
	}
}

void ProfilerSocket::SendShutdown()
{
	if ( !_bInitialized )
		return;
	_szOperation = "SendShutdown";

	SendNetworkMessage( SHUTDOWN );
}

void ProfilerSocket::SendAppDomainCreate( AppDomainID aid )
{
	_szOperation = "SendAppDomainCreate";

	SendNetworkMessage( APPDOMAIN_CREATE );
	SendAppDomainID( aid );
}

void ProfilerSocket::SendThreadCreate( ThreadID tid )
{
	_szOperation = "SendThreadCreate";

	SendNetworkMessage( THREAD_CREATE );
	SendThreadID( tid );
}

void ProfilerSocket::SendThreadEnd( ThreadID tid, UINT64 llThreadStartTime, UINT64 llThreadEndTime )
{
	_szOperation = "SendThreadEnd";

	SendNetworkMessage( THREAD_END );
	SendThreadID( tid );
	SendUINT64( llThreadStartTime );
	SendUINT64( llThreadEndTime );
}

void ProfilerSocket::SendStartFunctionData( ThreadID tid )
{
	_szOperation = "SendStartFunctionData";

	SendNetworkMessage( FUNCTION_DATA );
	SendThreadID( tid );
}

void ProfilerSocket::SendFunctionData( ProfilerHelper& ph, FunctionID fid )
{
	_szOperation = "SendFunctionData";

	SendFunctionID( fid );
	string strRet, strClassName, strFnName, strParameters;
	UINT32 uiMethodAttributes;
	ph.GetFunctionSignature( fid, uiMethodAttributes, strRet, strClassName, strFnName, strParameters );

	SendUINT32( uiMethodAttributes );
	SendString( strRet );
	SendString( strClassName );
	SendString( strFnName );
	SendString( strParameters );
}

void ProfilerSocket::SendProfilerMessage( const string& strMessage )
{
	_szOperation = "SendProfilerMessage";

	SendNetworkMessage( PROFILER_MESSAGE );
	SendString( strMessage );
}

void ProfilerSocket::SendFunctionTimingData( int nCalls, UINT64 llCycleCount, UINT64 llRecursiveCycleCount, UINT64 llSuspendCycleCount )
{
	_szOperation = "SendFunctionTimingData";

	SendUINT32( nCalls );
	SendUINT64( llCycleCount );
	SendUINT64( llRecursiveCycleCount );
	SendUINT64( llSuspendCycleCount );
}

void ProfilerSocket::SendCalleeFunctionData( FunctionID fid, int nCalls, UINT64 llCycleCount, UINT64 llRecursiveCycleCount )
{
	_szOperation = "SendCalleeFunctionData";

	SendFunctionID( fid );
	SendUINT32( nCalls );
	SendUINT64( llCycleCount );
	SendUINT64( llRecursiveCycleCount );
}

void ProfilerSocket::SendEndFunctionData()
{
	_szOperation = "SendEndFunctionData";

	SendFunctionID( 0xffffffff );
}

void ProfilerSocket::SendEndCalleeFunctionData()
{
	_szOperation = "SendEndCalleeFunctionData";

	SendFunctionID( 0xffffffff );
}

void ProfilerSocket::SendBool( bool b )
{
	SAFE_SEND( _s, b );
}

void ProfilerSocket::SendUINT32( UINT32 ui )
{
	SAFE_SEND( _s, ui );
}

void ProfilerSocket::SendUINT64( UINT64 ui )
{
	SAFE_SEND( _s, ui );
}

void ProfilerSocket::SendString( const string& strFunctionSignature )
{
	SendUINT32( ( UINT32 )strFunctionSignature.length() );
	SAFE_SEND_RAW( _s, strFunctionSignature.c_str(), ( int )strFunctionSignature.length() );
}

void ProfilerSocket::SendNetworkMessage( NetworkMessage nm )
{
	UINT16 mess = nm;
	SAFE_SEND( _s, mess );

	if ( !_bSentApplicationID )
	{
		if ( nm != INITIALIZE )
			SendUINT32( _nApplicationID );

		_bSentApplicationID = true;
	}
}

void ProfilerSocket::SendAppDomainID( AppDomainID aid )
{
	SAFE_SEND( _s, aid );
}

void ProfilerSocket::SendThreadID( ThreadID tid )
{
	SAFE_SEND( _s, tid );
}

void ProfilerSocket::SendFunctionID( FunctionID fid )
{
	SAFE_SEND( _s, fid );
}

int ProfilerSocket::ReadByte( BYTE& b )
{
	int result;
	SAFE_READ( _s, b, result );

	return result;
}

void ProfilerSocket::HandleError( const char* szCaller, int nError )
{
//	std::ofstream outf;
//	outf.open( "c:\\nprof.log", ios::app );
//	outf << _szOperation << " " << szCaller << " Error: " << nError << " on socket " << _s << endl;
}

void ProfilerSocket::HandleWrongSentLength( const char* szCaller, int nExpected, int nSent )
{
//	std::ofstream outf;
//	outf.open( "c:\\nprof.log", ios::app );
//	outf << _szOperation << " " << szCaller << " Expected to send " << nExpected << ", sent " << nSent << " instead." << endl;
}

void ProfilerSocket::HandleWrongRecvLength( const char* szCaller, int nExpected, int nSent )
{
//	std::ofstream outf;
//	outf.open( "c:\\nprof.log", ios::app );
//	outf << _szOperation << " " << szCaller << " Expected to send " << nExpected << ", sent " << nSent << " instead." << endl;
}

ProfilerSocket::~ProfilerSocket(void)
{
	closesocket( _s );
}
