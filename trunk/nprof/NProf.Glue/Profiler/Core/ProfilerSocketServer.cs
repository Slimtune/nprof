using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using NProf.Glue.Profiler.Info;

namespace NProf.Glue.Profiler.Core
{
	/// <summary>
	/// Summary description for ProfilerSocketServer.
	/// </summary>
	public class ProfilerSocketServer
	{
		public ProfilerSocketServer( Project.Options po )
		{
			_fsm = new FunctionSignatureMap();
			_tic = new ThreadInfoCollection();
			_nStopFlag = 0;
			_po = po;
			_alMessages = new ArrayList();
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

		public ThreadInfoCollection ThreadInfoCollection
		{
			get { return _tic; }
		}

		public string[] Messages
		{
			get { return ( string[] )_alMessages.ToArray( typeof( string ) ); }
		}

		public FunctionSignatureMap FunctionSignatureMap
		{
			get { return _fsm; }
		}

		private void ListenThread()
		{
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
					strError += b.ToString( "x" ) + " (" + ( char )b + ") ";

				throw new InvalidOperationException( strError );
			}

			byte[] abString = new byte[ nLength ];
			br.Read( abString, 0, nLength );
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

					switch ( nm )
					{
						case NetworkMessage.INITIALIZE:
						{
							if ( ( ( IPEndPoint )s.RemoteEndPoint ).Address != IPAddress.Loopback )
							{
								// Prompt the user?
							}

							int nNetworkProtocolVersion = br.ReadInt32();
							if ( nNetworkProtocolVersion != NETWORK_PROTOCOL_VERSION )
							{
								// Wrong version, write a negative byte
								ns.WriteByte( 0 );
								if ( Error != null )
									Error( new InvalidOperationException( "Profiler hook is wrong version" ) );
							}
							else
							{
								// Version was okay, write a positive byte
								if ( _po.Debug ) 
									ns.WriteByte( 2 );
								else
									ns.WriteByte( 1 );
								UInt32 nProcessID = br.ReadUInt32();
								string strProcessName = ReadLengthEncodedASCIIString( br );
							}
							break;
						}

						case NetworkMessage.SHUTDOWN:
						{
							if ( Exited != null )
								Exited( this, EventArgs.Empty );
							break;
						}

						case NetworkMessage.APPDOMAIN_CREATE:
						{
							nAppDomainID = br.ReadInt32();
							break;
						}

						case NetworkMessage.THREAD_CREATE:
							nThreadID = br.ReadInt32();
							break;

						case NetworkMessage.THREAD_END:
							nThreadID = br.ReadInt32();
							_tic[ nThreadID ].StartTime = br.ReadInt64();
							_tic[ nThreadID ].EndTime = br.ReadInt64();
							break;

						case NetworkMessage.FUNCTION_DATA:
						{
							nThreadID = br.ReadInt32();
							nFunctionID = br.ReadInt32();
							while ( nFunctionID != -1 )
							{
								FunctionSignature fs = new FunctionSignature( 
									br.ReadUInt32(),
									ReadLengthEncodedASCIIString( br ), 
									ReadLengthEncodedASCIIString( br ), 
									ReadLengthEncodedASCIIString( br ), 
									ReadLengthEncodedASCIIString( br ) 
									);
								_fsm.MapSignature( nFunctionID, fs );

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

									alCallees.Add( new CalleeFunctionInfo( _fsm, nCalleeFunctionID, nCalleeCalls, lCalleeTotalTime, lCalleeRecursiveTime ) );
									nCalleeFunctionID = br.ReadInt32();
								}
								CalleeFunctionInfo[] acfi = ( CalleeFunctionInfo[] )alCallees.ToArray( typeof( CalleeFunctionInfo ) );

								FunctionInfo fi = new FunctionInfo( _tic[ nThreadID ], nFunctionID, fs, nCalls, lTotalTime, lTotalRecursiveTime, lTotalSuspendTime, acfi );
								_tic[ nThreadID ].FunctionInfoCollection.Add( fi );
								nFunctionID = br.ReadInt32();
							}
							break;
						}

						case NetworkMessage.PROFILER_MESSAGE:
							string strMessage = ReadLengthEncodedASCIIString( br );
							_alMessages.Add( strMessage );
							if ( Message != null )
								Message( strMessage );

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

		const int NETWORK_PROTOCOL_VERSION = 2;

		private int						_nPort;
		private int						_nStopFlag;
		private ManualResetEvent		_mreStarted;
		private ManualResetEvent		_mreReceivedMessage;
		private FunctionSignatureMap	_fsm;
		private ThreadInfoCollection	_tic;
		private Thread					_t;
		private Socket					_s;
		private Project.Options			_po;
		private ArrayList				_alMessages;
	}
}
