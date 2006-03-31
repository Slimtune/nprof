using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;

namespace NProf.Glue.Profiler.Core
{
	/// <summary>
	/// Summary description for ProfilerSocketServer.
	/// </summary>
	public class ProfilerSocketServer
	{
		public ProfilerSocketServer( Project.Options po, Run run )
		{
			_run = run;
			_nStopFlag = 0;
			_po = po;
			_bHasStopped = false;
			_nCurrentApplicationID = 0;
			_nProfileCount = 0;
			
			_run.Messages.AddMessage( "Waiting for application..." );
		}

		public void Start()
		{
			_t = new Thread( new ThreadStart( ListenThread ) );
			_mreStarted = new ManualResetEvent( false );
			_t.Start();
			_mreStarted.WaitOne();
		}

		public void Stop()
		{
			lock ( _s )
				Interlocked.Increment( ref _nStopFlag );
			_s.Close();
		}

		public bool HasStoppedGracefully
		{
			get { return _bHasStopped; }
		}

		private void ListenThread()
		{
			Thread.CurrentThread.Name = "ProfilerSocketServer Listen Thread";
			try
			{
				_mreReceivedMessage = new ManualResetEvent( false );
				using ( _s = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp ) )
				{
					IPEndPoint ep = new IPEndPoint( IPAddress.Loopback, 0 );
					_s.Bind( ep );
					_nPort = ( ( IPEndPoint )_s.LocalEndPoint ).Port;
					_mreStarted.Set();
					_s.Listen( 100 );

					while ( true )
					{
						_mreReceivedMessage.Reset();
						lock ( _s )
							if ( _nStopFlag == 1 )
								break;
						_s.BeginAccept( new AsyncCallback( AcceptConnection ), _s );
						_mreReceivedMessage.WaitOne();
					}
				}
			}
			catch ( Exception e )
			{
				_mreStarted.Set();
			}
		}

		private string ReadLengthEncodedASCIIString( BinaryReader br )
		{
			int nLength = br.ReadInt32();
			if ( nLength > 2000 || nLength < 0 )
			{
				byte[] abNextBytes = new byte[ 8 ];
				br.Read( abNextBytes, 0, 8 );
				string strError = "Length was abnormally large or small (" + nLength.ToString( "x" ) + ").  Next bytes were ";
				foreach ( byte b in abNextBytes )
					strError += b.ToString( "x" ) + " (" + ( Char.IsControl( ( char )b ) ? '-' : ( char )b ) + ") ";

				throw new InvalidOperationException( strError );
			}

			byte[] abString = new byte[ nLength ];
			int nRead = 0;
			
			DateTime dt = DateTime.Now;

			while ( nRead < nLength )
			{
				nRead += br.Read( abString, nRead, nLength - nRead );

				// Make this loop finite (30 seconds)
				TimeSpan ts = DateTime.Now - dt;
				if ( ts.TotalSeconds > 30 )
					throw new InvalidOperationException( "Timed out while waiting for length encoded string" );
			}

			return System.Text.ASCIIEncoding.ASCII.GetString( abString, 0, nLength );
		}

		private void AcceptConnection( IAsyncResult ar )
		{
			lock ( _s )
			{
				if ( _nStopFlag == 1 )
				{
					_mreReceivedMessage.Set();
					return;
				}
			}

			// Note that this fails if you call EndAccept on a closed socket
			Socket s = ( ( Socket )ar.AsyncState ).EndAccept( ar );
			_mreReceivedMessage.Set();

			try
			{
				using ( NetworkStream ns = new NetworkStream( s, true ) )
				{
					BinaryReader br = new BinaryReader( ns );
					NetworkMessage nm = ( NetworkMessage )br.ReadInt16();

					int nAppDomainID, nThreadID, nFunctionID;
					ProcessInfo piCurrent = null;
					
					// All socket connections send their application ID first for all messages
					// except "INITIALIZE"
					int nApplicationID = -1;
					if ( nm != NetworkMessage.INITIALIZE )
					{
						nApplicationID = br.ReadInt32();
						piCurrent = _run.Processes[ nApplicationID ];
						
						if ( piCurrent == null )
						{
							_run.Messages.AddMessage( "Invalid application ID from profilee: " + nApplicationID );
							_run.Messages.AddMessage( "Closing socket connection" );
							ns.Close();

							return;
						}
					}

					switch ( nm )
					{
						case NetworkMessage.INITIALIZE:
						{
							if ( ( ( IPEndPoint )s.RemoteEndPoint ).Address != IPAddress.Loopback )
							{
								// Prompt the user?
							}

							if ( _run.State == Run.RunState.Running )
							{
								//ns.WriteByte( 0 );
							}

							int nNetworkProtocolVersion = br.ReadInt32();
							if ( nNetworkProtocolVersion != NETWORK_PROTOCOL_VERSION )
							{
								// Wrong version, write a negative byte
								ns.WriteByte( 0 );
								if ( Error != null )
									Error( new InvalidOperationException( "Profiler hook is wrong version: was " 
										+ nNetworkProtocolVersion + ", expected " + NETWORK_PROTOCOL_VERSION ) );
							}
							else
							{
								// Version was okay, write a positive byte
								if ( _po.Debug ) 
									ns.WriteByte( 2 );
								else
									ns.WriteByte( 1 );

								// Set up the new application
								nApplicationID = _nCurrentApplicationID++;
								piCurrent = new ProcessInfo( nApplicationID );
								_run.Processes.Add( piCurrent );

								ns.WriteByte( ( byte )nApplicationID );

								piCurrent.ProcessID = ( int )br.ReadUInt32();
								int nArgs = ( int )br.ReadUInt32();

								if ( nArgs > 0 )
								{
									string strFullFilename = ReadLengthEncodedASCIIString( br );
									strFullFilename = strFullFilename.Replace( "\"", "" );

									piCurrent.Name = Path.GetFileName( strFullFilename );
								}

								while ( nArgs > 1 )
								{
									nArgs--;
									ReadLengthEncodedASCIIString( br );
								}

								_nProfileCount++;
								_run.Messages.AddMessage( "Connected to " + piCurrent.Name + " with process ID " + piCurrent.ProcessID );
							}

							// We're off!
							_run.State = Run.RunState.Running;
							break;
						}

						case NetworkMessage.SHUTDOWN:
						{
							_nProfileCount--;
							_run.Messages.AddMessage( "Profiling completed for " + piCurrent.Name );

							if ( _nProfileCount == 0 )
							{
								_bHasStopped = true;
								_run.Messages.AddMessage( "Profiling completed." );
								if ( Exited != null )
									Exited( this, EventArgs.Empty );
							}

							break;
						}

						case NetworkMessage.APPDOMAIN_CREATE:
						{
							nAppDomainID = br.ReadInt32();
							_run.Messages.AddMessage( "AppDomain created: " + nAppDomainID );
							break;
						}

						case NetworkMessage.THREAD_CREATE:
							nThreadID = br.ReadInt32();
							_run.Messages.AddMessage( "Thread created: " + nThreadID );
							break;

						case NetworkMessage.THREAD_END:
							nThreadID = br.ReadInt32();
							piCurrent.Threads[ nThreadID ].StartTime = br.ReadInt64();
							piCurrent.Threads[ nThreadID ].EndTime = br.ReadInt64();
							_run.Messages.AddMessage( "Thread completed: " + nThreadID );
							break;

						case NetworkMessage.FUNCTION_DATA:
						{
							nThreadID = br.ReadInt32();
							_run.Messages.AddMessage( "Receiving function data for thread  " + nThreadID + "..." );

							nFunctionID = br.ReadInt32();
							int nIndex = 0;

							while ( nFunctionID != -1 )
							{
								UInt32 uiFlags = br.ReadUInt32();
								string strReturn = ReadLengthEncodedASCIIString( br );
								string strClassName = ReadLengthEncodedASCIIString( br );
								string strFnName = ReadLengthEncodedASCIIString( br );
								string strParameters = ReadLengthEncodedASCIIString( br );

								FunctionSignature fs = new FunctionSignature( 
									uiFlags,
									strReturn, 
									strClassName,
									strFnName,
									strParameters
								);
								piCurrent.Functions.MapSignature( nFunctionID, fs );

								int nCalls = br.ReadInt32();
								long lTotalTime = br.ReadInt64();
								long lTotalRecursiveTime = br.ReadInt64();
								long lTotalSuspendTime = br.ReadInt64();
								ArrayList alCallees = new ArrayList();
								int nCalleeFunctionID = br.ReadInt32();
								
								while ( nCalleeFunctionID != -1 )
								{
									int nCalleeCalls = br.ReadInt32();
									long lCalleeTotalTime = br.ReadInt64();
									long lCalleeRecursiveTime = br.ReadInt64();

									alCallees.Add( new CalleeFunctionInfo( piCurrent.Functions, nCalleeFunctionID, nCalleeCalls, lCalleeTotalTime, lCalleeRecursiveTime ) );
									nCalleeFunctionID = br.ReadInt32();
								}
								CalleeFunctionInfo[] acfi = ( CalleeFunctionInfo[] )alCallees.ToArray( typeof( CalleeFunctionInfo ) );

								FunctionInfo fi = new FunctionInfo( piCurrent.Threads[ nThreadID ], nFunctionID, fs, nCalls, lTotalTime, lTotalRecursiveTime, lTotalSuspendTime, acfi );
								piCurrent.Threads[ nThreadID ].FunctionInfoCollection.Add( fi );
								
								nFunctionID = br.ReadInt32();
								nIndex++;
							}

							_run.Messages.AddMessage( "Received " + nIndex + " item(s) for thread  " + nThreadID );
							break;
						}

						case NetworkMessage.PROFILER_MESSAGE:
							string strMessage = ReadLengthEncodedASCIIString( br );
							_run.Messages.AddMessage( strMessage );

							break;
					}
				}
			}
			catch ( Exception e )
			{
				if ( Error != null )
					Error( e );
			}
		}

		public int Port
		{
			get { return _nPort; }
		}

		public event EventHandler Exited;
		public event ErrorHandler Error;
		public event MessageHandler Message;

		public delegate void ErrorHandler( Exception e );
		public delegate void MessageHandler( string strMessage );

		// Sync with profiler_socket.h
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

		const int NETWORK_PROTOCOL_VERSION = 3;

		private int						_nPort;
		private int						_nStopFlag;
		private int						_nCurrentApplicationID;
		private int						_nProfileCount;
		private ManualResetEvent		_mreStarted;
		private ManualResetEvent		_mreReceivedMessage;
		private Thread					_t;
		private Socket					_s;
		private Project.Options			_po;
		private Run						_run;
		private bool					_bHasStopped;
	}
}
