#include "stdafx.h"
#include "profiler_socket.h"

#define SAFE_SEND( socket, data ) \
    { \
      int nSent = send( socket, ( const char * )&data, sizeof( data ), 0 ); \
      if ( nSent != sizeof( data ) ) \
      { \
        cout << "Error: " << WSAGetLastError() << endl; \
        cout << "Expected to send " << sizeof( data ) << ", sent " << nSent << " instead." << endl; \
      } \
    }
#define SAFE_SEND_RAW( socket, data, length ) \
    int nLength = length; \
    if ( send( socket, data, length, 0 ) != length ) \
    { \
      cout << "Error: " << WSAGetLastError() << endl; \
    }

#define SAFE_READ( socket, data, result ) \
    if ( recv( socket, ( char * )data, sizeof( data ), 0 ) != sizeof( data ) ) \
    { \
      result = WSAGetLastError(); \
      cout << "Error: " << result << endl; \
    } \
    else \
    { \
      result = 0; \
    }
    
ProfilerSocket::ProfilerSocket(void)
{
    sockaddr_in sa;
    sa.sin_family = AF_INET;
    sa.sin_port = htons( atoi( getenv( "NPROF_PROFILING_SOCKET" ) ) );
    sa.sin_addr.S_un.S_addr = inet_addr( "127.0.0.1" );

    _s = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, 0);
    if ( _s == INVALID_SOCKET )
    {
      cout << "socket = " << _s << endl;
      cout << WSAGetLastError() << endl;
    }
    int n = 0;
    while ( n < 10 )
    {
      int nResult = connect( _s, ( const sockaddr * )&sa, sizeof( sa ) );
      if ( nResult == SOCKET_ERROR )
      {
        cout << "connect = " << nResult << endl;
        cout << WSAGetLastError() << endl;
      }
      else
      {
        return;
      }

      Sleep( 200 );
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
      SendUINT32( ::GetCurrentProcessId() );
      SendString( ::GetCommandLine() );
    }
    else
    {
      cout << "We weren't allowed to initialize!" << endl;
    }
  }
  else
  {
    cout << "Could not initialize!" << endl;
  }
}

void ProfilerSocket::SendShutdown()
{
  SendNetworkMessage( SHUTDOWN );
}

void ProfilerSocket::SendAppDomainCreate( AppDomainID aid )
{
  SendNetworkMessage( APPDOMAIN_CREATE );
  SendAppDomainID( aid );
}

void ProfilerSocket::SendThreadCreate( ThreadID tid )
{
  SendNetworkMessage( THREAD_CREATE );
  SendThreadID( tid );
}

void ProfilerSocket::SendThreadEnd( ThreadID tid, UINT64 llThreadStartTime, UINT64 llThreadEndTime )
{
  SendNetworkMessage( THREAD_END );
  SendThreadID( tid );
  SendUINT64( llThreadStartTime );
  SendUINT64( llThreadEndTime );
}

void ProfilerSocket::SendStartFunctionData( ThreadID tid )
{
  SendNetworkMessage( FUNCTION_DATA );
  SendThreadID( tid );
}

void ProfilerSocket::SendFunctionData( ProfilerHelper& ph, FunctionID fid )
{
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

void ProfilerSocket::SendFunctionTimingData( int nCalls, UINT64 llCycleCount, UINT64 llSuspendCycleCount )
{
  SendUINT32( nCalls );
  SendUINT64( llCycleCount );
  SendUINT64( llSuspendCycleCount );
}

void ProfilerSocket::SendCalleeFunctionData( FunctionID fid, int nCalls, UINT64 llCycleCount )
{
  SendFunctionID( fid );
  SendUINT32( nCalls );
  SendUINT64( llCycleCount );
}

void ProfilerSocket::SendEndFunctionData()
{
  SendFunctionID( 0xffffffff );
}

void ProfilerSocket::SendEndCalleeFunctionData()
{
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
  SAFE_READ( _s, &b, result );

  return result;
}

ProfilerSocket::~ProfilerSocket(void)
{
  closesocket( _s );
}
