using System;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using NProf.GUI;
using NProf;


namespace NProf
{
	public class ProfilerSocketServer
	{
		public ProfilerSocketServer(Run run)
		{
			this.run = run;
			this.stopFlag = 0;
			this.hasStopped = false;
			this.currentApplicationID = 0;
			this.profileCount = 0;
			this.run.Messages.AddMessage("Waiting for application...");
		}
		public void Start()
		{
			thread = new Thread(new ThreadStart(ListenThread));
			resetStarted = new ManualResetEvent(false);
			thread.Start();
			resetStarted.WaitOne();
		}
		public void Stop()
		{
			lock (socket)
				Interlocked.Increment(ref stopFlag);
			socket.Close();
		}
		public bool HasStoppedGracefully
		{
			get { return hasStopped; }
		}
		private void ListenThread()
		{
			Thread.CurrentThread.Name = "ProfilerSocketServer Listen Thread";
			try
			{
				resetMessageReceived = new ManualResetEvent(false);
				using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 0);
					socket.Bind(ep);
					port = ((IPEndPoint)socket.LocalEndPoint).Port;
					resetStarted.Set();
					socket.Listen(100);

					while (true)
					{
						resetMessageReceived.Reset();
						lock (socket)
							if (stopFlag == 1)
								break;
						socket.BeginAccept(new AsyncCallback(AcceptConnection), socket);
						resetMessageReceived.WaitOne();
					}
				}
			}
			catch (Exception e)
			{
				resetStarted.Set();
			}
		}
		private string ReadLengthEncodedASCIIString(BinaryReader br)
		{
			int length = br.ReadInt32();
			if (length > 2000 || length < 0)
			{
				byte[] abNextBytes = new byte[8];
				br.Read(abNextBytes, 0, 8);
				string strError = "Length was abnormally large or small (" + length.ToString("x") + ").  Next bytes were ";
				foreach (byte b in abNextBytes)
					strError += b.ToString("x") + " (" + (Char.IsControl((char)b) ? '-' : (char)b) + ") ";

				throw new InvalidOperationException(strError);
			}

			byte[] abString = new byte[length];
			int nRead = 0;

			DateTime dt = DateTime.Now;

			while (nRead < length)
			{
				nRead += br.Read(abString, nRead, length - nRead);

				// Make this loop finite (30 seconds)
				TimeSpan ts = DateTime.Now - dt;
				if (ts.TotalSeconds > 30)
					throw new InvalidOperationException("Timed out while waiting for length encoded string");
			}

			return System.Text.ASCIIEncoding.ASCII.GetString(abString, 0, length);
		}
		ProcessInfo currentProcess = null;

		private void AcceptConnection(IAsyncResult ar)
		{
			lock (socket)
			{
				if (stopFlag == 1)
				{
					resetMessageReceived.Set();
					return;
				}
			}

			// Note that this fails if you call EndAccept on a closed socket
			Socket s = ((Socket)ar.AsyncState).EndAccept(ar);
			resetMessageReceived.Set();

			try
			{
				using (NetworkStream stream = new NetworkStream(s, true))
				{
					BinaryReader reader = new BinaryReader(stream);
					NetworkMessage message = (NetworkMessage)reader.ReadInt16();

					int appDomainID, threadId, functionId;
					//ProcessInfo currentProcess = null;

					// All socket connections send their application ID first for all messages
					// except "INITIALIZE"
					int applicationID = -1;
					if (message != NetworkMessage.INITIALIZE)
					{
						applicationID = reader.ReadInt32();

						if (currentProcess == null)
						{
							currentProcess = new ProcessInfo(applicationID);
						}
						//currentProcess = run.Processes[ applicationID ];

						if (currentProcess == null)
						{
							run.Messages.AddMessage("Invalid application ID from profilee: " + applicationID);
							run.Messages.AddMessage("Closing socket connection");
							stream.Close();

							return;
						}
					}

					switch (message)
					{
						case NetworkMessage.INITIALIZE:
							{
								if (((IPEndPoint)s.RemoteEndPoint).Address != IPAddress.Loopback)
								{
									// Prompt the user?
								}

								if (run.State == RunState.Running)
								{
									//ns.WriteByte( 0 );
								}

								int networkProtocolVersion = reader.ReadInt32();
								if (networkProtocolVersion != NETWORK_PROTOCOL_VERSION)
								{
									// Wrong version, write a negative byte
									stream.WriteByte(0);
									if (Error != null)
										Error(new InvalidOperationException("Profiler hook is wrong version: was "
											+ networkProtocolVersion + ", expected " + NETWORK_PROTOCOL_VERSION));
								}
								else
								{
									// Version was okay, write a positive byte
									if (ProfilerForm.form.Project.DebugProfiler)
									//if (run.Project.DebugProfiler)
										//if (run.Project.DebugProfiler)
									{
										stream.WriteByte(2);
									}
									else
									{
										stream.WriteByte(1);
									}

									// Set up the new application
									applicationID = currentApplicationID++;

									currentProcess = new ProcessInfo();

									run.Process = currentProcess;

									stream.WriteByte((byte)applicationID);

									currentProcess.ProcessID = (int)reader.ReadUInt32();
									int argCount = (int)reader.ReadUInt32();

									if (argCount > 0)
									{
										string strFullFilename = ReadLengthEncodedASCIIString(reader);
										strFullFilename = strFullFilename.Replace("\"", "");

										currentProcess.Name = Path.GetFileName(strFullFilename);
									}

									while (argCount > 1)
									{
										argCount--;
										ReadLengthEncodedASCIIString(reader);
									}

									profileCount++;
									run.Messages.AddMessage("Connected to " + currentProcess.Name + " with process ID " + currentProcess.ProcessID);
								}

								// We're off!
								run.State = RunState.Running;
								break;
							}

						case NetworkMessage.SHUTDOWN:
							{
								profileCount--;
								run.Messages.AddMessage("Profiling completed for " + currentProcess.Name);

								if (profileCount == 0)
								{
									hasStopped = true;
									run.Messages.AddMessage("Profiling completed.");
									if (Exited != null)
										Exited(this, EventArgs.Empty);
								}

								break;
							}

						case NetworkMessage.APPDOMAIN_CREATE:
							{
								appDomainID = reader.ReadInt32();
								run.Messages.AddMessage("AppDomain created: " + appDomainID);
								break;
							}

						case NetworkMessage.THREAD_CREATE:
							threadId = reader.ReadInt32();
							run.Messages.AddMessage("Thread created: " + threadId);
							break;

						case NetworkMessage.THREAD_END:
							threadId = reader.ReadInt32();
							currentProcess.Threads[threadId].StartTime = reader.ReadInt64();
							currentProcess.Threads[threadId].EndTime = reader.ReadInt64();
							run.Messages.AddMessage("Thread completed: " + threadId);
							break;

						case NetworkMessage.FUNCTION_DATA:
							{
								threadId = reader.ReadInt32();
								run.Messages.AddMessage("Receiving function data for thread  " + threadId + "...");

								functionId = reader.ReadInt32();
								int nIndex = 0;

								while (functionId != -1)
								{
									UInt32 uiFlags = reader.ReadUInt32();
									string returnValue = ReadLengthEncodedASCIIString(reader);
									string className = ReadLengthEncodedASCIIString(reader);
									string functionName = ReadLengthEncodedASCIIString(reader);
									string parameters = ReadLengthEncodedASCIIString(reader);

									FunctionSignature fs = new FunctionSignature(
										uiFlags,
										returnValue,
										className,
										functionName,
										parameters
									);
									currentProcess.Functions.MapSignature(functionId, fs);

									int callCount = reader.ReadInt32();
									long totalTime = reader.ReadInt64();
									long totalRecursiveTime = reader.ReadInt64();
									long totalSuspendTime = reader.ReadInt64();
									List<CalleeFunctionInfo> callees = new List<CalleeFunctionInfo>();
									int calleeFunctionId = reader.ReadInt32();

									while (calleeFunctionId != -1)
									{
										int calleeCallCount = reader.ReadInt32();
										long calleeTotalTime = reader.ReadInt64();
										long calleeRecursiveTime = reader.ReadInt64();

										callees.Add(new CalleeFunctionInfo(currentProcess.Functions, calleeFunctionId, calleeCallCount, calleeTotalTime, calleeRecursiveTime));
										calleeFunctionId = reader.ReadInt32();
									}
									//CalleeFunctionInfo[] acfi = ( CalleeFunctionInfo[] )callees.ToArray( typeof( CalleeFunctionInfo ) );

									Method function = new Method(currentProcess.Threads[threadId], functionId, fs, callCount, totalTime, totalRecursiveTime, totalSuspendTime, callees.ToArray());
									currentProcess.Threads[threadId].FunctionInfoCollection.Add(function.ID, function);
									//currentProcess.Threads[threadId].FunctionInfoCollection.Add(function);

									functionId = reader.ReadInt32();
									nIndex++;
								}

								run.Messages.AddMessage("Received " + nIndex + " item(s) for thread  " + threadId);
								break;
							}

						case NetworkMessage.PROFILER_MESSAGE:
							string text = ReadLengthEncodedASCIIString(reader);
							run.Messages.AddMessage(text);

							break;
					}
				}
			}
			catch (Exception e)
			{
				if (Error != null)
					Error(e);
			}
		}

		public int Port
		{
			get { return port; }
		}

		public event EventHandler Exited;
		public event ErrorHandler Error;
		public event MessageHandler Message;

		public delegate void ErrorHandler(Exception e);
		public delegate void MessageHandler(string strMessage);

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

		private int port;
		private int stopFlag;
		private int currentApplicationID;
		private int profileCount;
		private ManualResetEvent resetStarted;
		private ManualResetEvent resetMessageReceived;
		private Thread thread;
		private Socket socket;
		private Run run;
		private bool hasStopped;
	}
	[Serializable]
	public class CalleeFunctionInfo
	{
		public CalleeFunctionInfo()
		{
			signatures = new FunctionSignatureMap();
		}
		public CalleeFunctionInfo( FunctionSignatureMap signatures, int id, int calls, long totalTime, long totalRecursiveTime)
		{
			this.id = id;
			this.calls = calls;
			this.totalTime = totalTime;
			this.totalRecursiveTime = totalRecursiveTime;
			this.signatures = signatures;
		}
		public int ID
		{
			get { return id; }
			set { id = value; }
		}
		public string Signature
		{
			get { return signatures.GetFunctionSignature( ID ); }
		}
		public int Calls
		{
			get { return calls; }
			set { calls = value; }
		}
		[XmlIgnore]
		public long TotalTime
		{
			get { return totalTime; }
			set { totalTime = value; }
		}
		[XmlIgnore]
		public long TotalRecursiveTime
		{
			get { return totalRecursiveTime; }
			set { totalRecursiveTime = value; }
		}
		[XmlIgnore]
		public double TimeInMethod
		{
			get { return (double)totalTime / (double)function.ThreadTotalTicks; }
		}
		[XmlIgnore]
		public double TimeOfParentInMethod
		{
			get { return (double)totalTime / (double)function.TotalTicks; }
		}
		internal Method FunctionInfo
		{
			set { function = value; }
		}
		private int id;
		private int calls;
		private Method function;
		private FunctionSignatureMap signatures;
		private long totalTime;
		private long totalRecursiveTime;
	}
	[Serializable]
	public class Method
	{
		public Method()
		{
		}
		public Method( ThreadInfo ti, int nID, FunctionSignature signature, int calls, long totalTime, long totalRecursiveTime, long totalSuspendedTime, CalleeFunctionInfo[] callees )
		{
			this.thread = ti;
			this.id = nID;
			this.signature = signature;
			this.calls = calls;
			this.totalTime = totalTime;
			this.totalRecursiveTime = totalRecursiveTime;
			this.totalSuspendedTime = totalSuspendedTime;
			this.callees = callees;

			foreach ( CalleeFunctionInfo callee in callees )
				callee.FunctionInfo = this;
		}

		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public FunctionSignature Signature
		{
			get { return signature; }
			set { signature = value; }
		}

		public CalleeFunctionInfo[] CalleeInfo
		{
			get { return callees; }
			set { callees = value; }
		}
		public int Calls
		{
			get { return calls; }
		}
		[XmlIgnore]
		public long ThreadTotalTicks
		{
			get { return thread.TotalTime; }
		}
		[XmlIgnore]
		public long TotalTicks
		{
			get { return totalTime; }
		}
		[XmlIgnore]
		public long TotalRecursiveTicks
		{
			get { return totalRecursiveTime; }
		}
		[XmlIgnore]
		public long TotalSuspendedTicks
		{
			get { return totalSuspendedTime; }
		}
		[XmlIgnore]
		public long TotalChildrenTicks
		{
			get
			{
				long totalChildrenTime = 0;
				foreach ( CalleeFunctionInfo callee in callees )
					totalChildrenTime += callee.TotalTime;

				return totalChildrenTime;
			}
		}
		[XmlIgnore]
		public long TotalChildrenRecursiveTicks
		{
			get
			{
				long totalChildrenRecursiveTime = 0;
				foreach ( CalleeFunctionInfo callee in callees )
					totalChildrenRecursiveTime += callee.TotalRecursiveTime;

				return totalChildrenRecursiveTime;
			}
		}
		[XmlIgnore]
		public double PercentOfTotalTimeSuspended
		{
			get
			{
				if (ThreadTotalTicks == 0)
					return 0;

				return (
					(double)totalSuspendedTime
					/
					(double)ThreadTotalTicks);
			}
		}
		[XmlIgnore]
		public double PercentOfMethodTimeSuspended
		{
			get
			{
				if (TotalTicks == 0)
					return 0;

				return (
					(double)totalSuspendedTime
					/
					(double)TotalTicks);
			}
		}
		[XmlIgnore]
		public double PercentOfTotalTimeInMethod
		{
			get
			{
				if (ThreadTotalTicks == 0)
					return 0;

				return (
					(double)(TotalTicks + TotalRecursiveTicks - TotalChildrenTicks - TotalChildrenRecursiveTicks)
					/
					(double)ThreadTotalTicks);
			}
		}
		[XmlIgnore]
		public double PercentOfTotalTimeInMethodAndChildren
		{
			get
			{
				if (ThreadTotalTicks == 0)
					return 0;

				return (
					(double)TotalTicks
					/
					(double)ThreadTotalTicks);
			}
		}
		[XmlIgnore]
		public double TimeInChildren
		{
			get
			{
				if (TotalTicks == 0)
					return 0;

				return (
					(double)(TotalChildrenTicks)
					/
					(double)(TotalTicks + TotalRecursiveTicks));
			}
		}
		[XmlIgnore]
		public double TimeInMethod
		{
			get
			{
				if (TotalTicks == 0)
					return 0;

				return (
					(double)(TotalTicks + TotalRecursiveTicks - TotalChildrenTicks - TotalChildrenRecursiveTicks)
					/
					(double)(TotalTicks + TotalRecursiveTicks));
			}
		}
		[XmlIgnore]
		public double TotalTimeInChildren
		{
			get
			{
				if (TotalTicks == 0)
					return 0;

				long totalChildrenTime = 0;
				foreach (CalleeFunctionInfo callee in callees)
					totalChildrenTime += callee.TotalTime;

				return ((
					(double)(TotalChildrenTicks + TotalChildrenRecursiveTicks - TotalRecursiveTicks))
					/
					(double)ThreadTotalTicks);
			}
		}
		private int id;
		private int calls;
		private long totalTime;
		private long totalRecursiveTime;
		private long totalSuspendedTime;
		private FunctionSignature signature;
		private CalleeFunctionInfo[] callees;
		private ThreadInfo thread;
	}
	[Serializable]
	public class FunctionInfoCollection : Dictionary<int, Method>
	{
		public FunctionInfoCollection()
		{
		}
		public FunctionInfoCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class FunctionSignature
	{
		public FunctionSignature()
		{
		}

		public FunctionSignature( UInt32 methodAttributes, string returnType, string className, string functionName, string parameters )
		{
			CorMethodAttr cma = ( CorMethodAttr )methodAttributes;
			this.isPInvoke = ( cma & CorMethodAttr.mdPinvokeImpl ) != 0;
			this.isStatic = ( cma & CorMethodAttr.mdStatic ) != 0;
			this.isExtern = ( cma & CorMethodAttr.mdUnmanagedExport ) != 0;

			this.returnType = returnType;
			this.className = className;
			this.functionName = functionName;
			this.parameters = parameters;
		}

		public bool IsPInvoke
		{
			get { return isPInvoke; }
			set { isPInvoke = value; }
		}

		public bool IsStatic
		{
			get { return isStatic; }
			set { isStatic = value; }
		}

		public bool IsExtern
		{
			get { return isExtern; }
			set { isExtern = value; }
		}

		public string ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public string ClassName
		{
			get { return className; }
			set { className = value; }
		}

		public string FunctionName
		{
			get { return functionName; }
			set { functionName = value; }
		}

		public string Parameters
		{
			get { return parameters; }
			set { parameters = value; }
		}

		public string[] Namespace
		{
			get
			{
				string[] astrPieces = className.Split( '.' );
				string[] astrNamespace = new String[ astrPieces.Length - 1 ];
				Array.Copy( astrPieces, 0, astrNamespace, 0, astrPieces.Length - 1 );

				return astrNamespace;
			}
		}

		public string NamespaceString
		{
			get { return String.Join( ".", Namespace ); }
		}

		public string Signature
		{
			get
			{
				return className+"."+functionName+parameters;
			}
		}

		[Flags]
		enum CorMethodAttr
		{
			// member access mask - Use this mask to retrieve accessibility information.
			mdMemberAccessMask          =   0x0007,
			mdPrivateScope              =   0x0000,     // Member not referenceable.
			mdPrivate                   =   0x0001,     // Accessible only by the parent type.  
			mdFamANDAssem               =   0x0002,     // Accessible by sub-types only in this Assembly.
			mdAssem                     =   0x0003,     // Accessibly by anyone in the Assembly.
			mdFamily                    =   0x0004,     // Accessible only by type and sub-types.    
			mdFamORAssem                =   0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
			mdPublic                    =   0x0006,     // Accessibly by anyone who has visibility to this scope.    
			// end member access mask

			// method contract attributes.
			mdStatic                    =   0x0010,     // Defined on type, else per instance.
			mdFinal                     =   0x0020,     // Method may not be overridden.
			mdVirtual                   =   0x0040,     // Method virtual.
			mdHideBySig                 =   0x0080,     // Method hides by name+sig, else just by name.

			// vtable layout mask - Use this mask to retrieve vtable attributes.
			mdVtableLayoutMask          =   0x0100,
			mdReuseSlot                 =   0x0000,     // The default.
			mdNewSlot                   =   0x0100,     // Method always gets a new slot in the vtable.
			// end vtable layout mask

			// method implementation attributes.
			mdAbstract                  =   0x0400,     // Method does not provide an implementation.
			mdSpecialName               =   0x0800,     // Method is special.  Name describes how.

			// interop attributes
			mdPinvokeImpl               =   0x2000,     // Implementation is forwarded through pinvoke.
			mdUnmanagedExport           =   0x0008,     // Managed method exported via thunk to unmanaged code.

			// Reserved flags for runtime use only.
			mdReservedMask              =   0xd000,
			mdRTSpecialName             =   0x1000,     // Runtime should check name encoding.
			mdHasSecurity               =   0x4000,     // Method has security associate with it.
			mdRequireSecObject          =   0x8000,     // Method calls another method containing security code.

		} ;

		private bool isPInvoke;
		private bool isStatic;
		private bool isExtern;
		private string returnType;
		private string className;
		private string functionName;
		private string parameters;
	}
	[Serializable]
	public class FunctionSignatureMap
	{
		public FunctionSignatureMap()
		{
			signatures = new Hashtable();
		}

		public void MapSignature( int functionID, FunctionSignature signature )
		{
			lock (signatures.SyncRoot)
			{
				signatures[functionID] = signature;
			}
		}

		public string GetFunctionSignature( int functionID )
		{
			lock ( signatures.SyncRoot )
			{
				FunctionSignature signature = ( FunctionSignature )signatures[ functionID ];
				if (signature == null)
				{
					return "Unknown!";
				}

				return signature.Signature;
			}
		}

		private Hashtable signatures;
	}
	[Serializable]
	public class ProcessInfo
	{
		public ProcessInfo()
		{
			this.id = 0;
			this.threads = new ThreadInfoCollection();
			this.signatures = new FunctionSignatureMap();
		}

		public ProcessInfo( int id ) : this()
		{
			this.id = id;
		}

		public int ID
		{
			get { return id; }
		}

		[XmlIgnore]
		public int ProcessID
		{
			get { return processId; }
			set { processId = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public FunctionSignatureMap Functions
		{
			get { return signatures; }
			set { signatures = value; }
		}

		public ThreadInfoCollection Threads
		{
			get { return threads; }
			set { threads = value; }
		}

		public override string ToString()
		{
			return String.Format( "{0} ({1})", name, processId );
		}

		private int id;
		private int processId;
		private FunctionSignatureMap signatures;
		private ThreadInfoCollection threads;
		private string name;
	}
	[Serializable]
	public class ProcessInfoCollection : IEnumerable
	{
		public ProcessInfoCollection()
		{
			processes = new Dictionary<int,ProcessInfo>();
		}

		public IEnumerator GetEnumerator()
		{
			return processes.Values.GetEnumerator();
		}

		public void Add(ProcessInfo process)
		{
			processes[process.ID] = process;
		}

		public ProcessInfo this[int nProcessID]
		{
			get
			{
				lock (processes)
				{
					ProcessInfo pi = (ProcessInfo)processes[nProcessID];
					if (pi == null)
					{
						pi = new ProcessInfo(nProcessID);
						processes[nProcessID] = pi;
					}

					return pi;
				}
			}
		}
		private Dictionary<int,ProcessInfo> processes;
	}
	[Serializable]
	public class ThreadInfo
	{
		public ThreadInfo()
		{
			this.functions = new FunctionInfoCollection();
			this.id = 0;
			this.startTime = 0;
			this.endTime = 0;
		}

		public ThreadInfo( int threadId ) : this()
		{
			this.id = threadId;
		}

		[XmlIgnore]
		public int ID
		{
			get { return id; }
		}
		[XmlIgnore]
		public long StartTime
		{
			get { return startTime; }
			set { startTime = value; }
		}
		[XmlIgnore]
		public long EndTime
		{
			get { return endTime; }
			set { endTime = value; }
		}
		[XmlIgnore]
		public long TotalTime
		{
			get { return endTime - startTime; }
		}

		public FunctionInfoCollection FunctionInfoCollection
		{
			get { return functions; }
		}

		public override string ToString()
		{
			return "Thread #" + id;
		}

		private int id;
		private long startTime, endTime;
		private FunctionInfoCollection functions;
	}
	[Serializable]
	public class ThreadInfoCollection : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			return threads.Values.GetEnumerator();
		}
		public ThreadInfo this[ int threadId ]
		{
			get
			{
				lock ( threads )
				{
					ThreadInfo thread;
					if(!threads.TryGetValue(threadId,out thread))
					{
						thread = new ThreadInfo( threadId );
						threads[ threadId ] = thread;
					}
					return thread;
				}
			}
		}
		private Dictionary<int,ThreadInfo> threads=new Dictionary<int,ThreadInfo>();
	}
	[Serializable]
	public class ProjectInfo
	{
		public ProjectInfo() : this(ProjectType.File) { } // JC: added default constructor for serialization

		public ProjectInfo( ProjectType projectType )
		{
			this.name = null;
			this.runs = new RunCollection( this );
			this.projectType = projectType;
		}

		public string Name
		{
			get { return name; }
			set { name = value; Fire_ProjectInfoChanged(); }
		}

		public string ApplicationName
		{
			get { return appName; }
			set { appName = value; Fire_ProjectInfoChanged(); }
		}

		public string Arguments
		{
			get { return arguments; }
			set { arguments = value; Fire_ProjectInfoChanged(); }
		}

		public string WorkingDirectory
		{
			get { return workingDirectory; }
			set { workingDirectory = value; Fire_ProjectInfoChanged(); }
		}

		public RunCollection Runs
		{
			get { return runs; }
		}

		public Run CreateRun( Profiler p )
		{
			Run run = new Run( p, this );
			runs.Add( run );
			
			return run;
		}

		public bool DebugProfiler
		{
			get 
			{
				return debugHook;
			}
			set
			{
				debugHook = value;
			}
		}

		public ProjectType ProjectType
		{
			get { return projectType; }
		}

		private void Fire_ProjectInfoChanged()
		{
			if ( ProjectInfoChanged != null )
				ProjectInfoChanged( this );
		}
		[field:NonSerialized]
		public event ProjectEventHandler ProjectInfoChanged;

		public delegate void ProjectEventHandler( ProjectInfo project );

		private string appName;
		private string arguments;
		private string workingDirectory;
		private string name;
		private bool debugHook;
		private RunCollection runs;
		private ProjectType projectType;
	}

	[Serializable]
	public enum ProjectType
	{
		/// <summary>
		/// The project references a file on the filesystem.
		/// </summary>
		File,
		/// <summary>
		/// The project is run from VS.NET.
		/// </summary>
		VSNet,
		/// <summary>
		/// The project attaches to ASP.NET.
		/// </summary>
		AspNet,
	}
	[Serializable]
	public class Run
	{
		public Run()
		{
			this.messages = new RunMessageCollection();
		}

		public Run( Profiler p, ProjectInfo pi )
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
			this.runState = RunState.Initializing;
			this.project = pi;
			this.messages = new RunMessageCollection();
			this.isSuccess = false;
		}
		public bool Start()
		{
			start = DateTime.Now;

			return profiler.Start( project, this, new Profiler.ProcessCompletedHandler( OnProfileComplete ) );
		}

		public bool CanStop
		{
			get { return State == RunState.Initializing || 
						( project.ProjectType == ProjectType.AspNet && State != RunState.Finished ); } 
		}

		public bool Stop()
		{
			profiler.Stop();

			return true;
		}

		//public ProjectInfo Project
		//{
		//    get { return project; }
		//    set { project = value; }
		//}

		[XmlIgnore]
		public DateTime StartTime
		{
			get { return start; }
			set { start = value; }
		}
		[XmlIgnore]
		public DateTime EndTime
		{
			get { return end; }
			set { end = value; }
		}

		public RunState State
		{
			get { return runState; }
			
			set 
			{ 
				RunState rsOld = runState;
				runState = value;
				if ( StateChanged != null )
					StateChanged( this, rsOld, runState );
			}
		}

		[XmlIgnore]
		public RunMessageCollection Messages
		{
			get { return messages; }
		}

		public bool Success
		{
			get { return isSuccess; }
			set { isSuccess = value; }
		}


		public Profiler.ProcessCompletedHandler ProcessCompletedHandler
		{
			get { return new Profiler.ProcessCompletedHandler( OnProfileComplete ); }
		}

		private void OnProfileComplete( Run run )
		{
			State = RunState.Finished;
			end = DateTime.Now;
		}

		[field:NonSerialized]
		public event RunStateEventHandler StateChanged;


		private bool isSuccess;
		private DateTime start;
		private DateTime end;
		private RunState runState;
		private ProjectInfo project;
		private Profiler profiler;
		private RunMessageCollection messages;
		private ProcessInfo process;

		public ProcessInfo Process
		{
			get
			{
				return process;
			}
			set
			{
				process = value;
			}
		}
	}
	public delegate void RunStateEventHandler(Run run, RunState rsOld, RunState rsNew);

	[Serializable]
	public enum RunState
	{
		Initializing,
		Running,
		Finished,
	}
	[Serializable]
	public class RunCollection : IEnumerable
	{
		public RunCollection( ProjectInfo pi )
		{
			this.project = pi;
			items = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public int Count
		{
			get { return items.Count; }
		}

		public void Add( Run run )
		{
			items.Add( run );
			if ( RunAdded != null )
				RunAdded( project, this, run, items.Count - 1 );
		}

		public void RemoveAt( int index )
		{
			Run run = this[ index ];
			items.RemoveAt( index );
			if ( RunRemoved != null )
				RunRemoved( project, this, run, index );
		}

		public Run this[ int nIndex ]
		{
			get { return ( Run )items[ nIndex ]; }
		}
		[field:NonSerialized]
		public event RunEventHandler RunAdded;
		[field:NonSerialized]
		public event RunEventHandler RunRemoved;

		public delegate void RunEventHandler( ProjectInfo pi, RunCollection runs, Run run, int nIndex );

		private ArrayList items;
		private ProjectInfo project;
	}
	[Serializable]
	public class RunMessageCollection : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			return AllMessages.GetEnumerator();
		}
		public string[] AllMessages
		{
			get
			{
				lock ( messages )
				{
					return ( string[] )messages.ToArray( typeof( string ) );
				}
			}
		}

		public string[] StartListening( MessageHandler handler )
		{
			lock ( messages )
			{
				Message += handler;
				return AllMessages;
			}
		}

		public void StopListening( MessageHandler handler )
		{
			lock ( messages )
			{
				Message -= handler;
			}
		}

		public void Add( object oMessage )
		{
			// For XML serialization
			AddMessage( ( string )oMessage );
		}

		public void AddMessage( string message )
		{
			lock ( messages )
			{
				messages.Add( message );
				if ( Message != null )
					Message( message );
			}
		}

		private ArrayList messages=new ArrayList();

		public delegate void MessageHandler( string message );
		[field:NonSerialized]
		private event MessageHandler Message;
	}
	public class SerializationHandler
	{
		public static string DataStoreDirectory;

		private static Hashtable projectToFileName = Hashtable.Synchronized( new Hashtable() );

		static SerializationHandler() { }
		public static ProjectInfo OpenProjectInfo(string fileName)
		{
			if (!File.Exists(fileName))
				return null;

			using (Stream stream = File.Open(fileName, FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				ProjectInfo project = (ProjectInfo)formatter.Deserialize(stream);
				stream.Close();

				// add this for looking up later
				projectToFileName[project] = fileName;

				// make the file recently used and return
				//MakeRecentlyUsed(project, fileName);
				return project;
			}
		}
		public static void SaveProjectInfo( ProjectInfo info )
		{
			if( projectToFileName.Contains( info ) ) // one that has already been opened
			{
				string fileName = ( string )projectToFileName[ info ];

				SaveProjectInfo( info, fileName );
			}
		}
		public static void SaveProjectInfo(ProjectInfo info, string fileName)
		{
			using (Stream stream = File.Open(fileName,FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, info);
				// remember for later
				projectToFileName[info] = fileName;

				// make it recently used
				//MakeRecentlyUsed(info, fileName);
			}
		}
		public static string GetFilename( ProjectInfo info )
		{
			return ( ( string )projectToFileName[ info ] ) + string.Empty;
		}
	}
	[Serializable]
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";
		public Profiler()
		{
			functionMap = new Hashtable();
		}

		public static string Version
		{
			get { return "0.9-alpha"; }
		}

		public bool CheckSetup( out string message )
		{
			message = String.Empty;
			using ( RegistryKey rk = Registry.ClassesRoot.OpenSubKey( "CLSID\\" + PROFILER_GUID ) )
			{
				if ( rk == null )
				{
					message = "Unable to find the registry key for the profiler hook.  Please register the NProf.Hook.dll file.";
					return false;
				}
			}

			return true;
		}

		public bool Start( ProjectInfo pi, Run run, ProcessCompletedHandler pch )
		{
			this.start = DateTime.Now;
			this.project = pi;
			this.completed = pch;
			this.run = run;
			this.run.State = RunState.Initializing;

			socketServer = new ProfilerSocketServer(run);
			socketServer.Start();
			socketServer.Exited += new EventHandler( OnProcessExited );
			socketServer.Error += new ProfilerSocketServer.ErrorHandler( OnError );
			socketServer.Message += new ProfilerSocketServer.MessageHandler( OnMessage );

			switch ( pi.ProjectType )
			{
				case ProjectType.File:
				{
					process = new Process();
					process.StartInfo = new ProcessStartInfo( pi.ApplicationName, pi.Arguments );
					process.StartInfo.EnvironmentVariables[ "COR_ENABLE_PROFILING" ] = "0x1";
					process.StartInfo.EnvironmentVariables[ "COR_PROFILER" ] = PROFILER_GUID;
					process.StartInfo.EnvironmentVariables[ "NPROF_PROFILING_SOCKET" ] = socketServer.Port.ToString();
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.Arguments = pi.Arguments;
					process.StartInfo.WorkingDirectory = pi.WorkingDirectory;
					process.EnableRaisingEvents = true;
					//_p.Exited += new EventHandler( OnProcessExited );

					return process.Start();
				}

				case ProjectType.AspNet:
				{
					using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\W3SVC", true ) )
					{
						if ( rk != null )
							SetRegistryKeys( rk, true );
					}

					using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\IISADMIN", true ) )
					{
						if ( rk != null )
							SetRegistryKeys( rk, true );
					}

					Process p = Process.Start( "iisreset.exe", "" );
					p.WaitForExit();
					this.run.Messages.AddMessage( "Navigate to your project and ASP.NET will connect to the profiler" );
					this.run.Messages.AddMessage( "NOTE: ASP.NET must be set to run under the SYSTEM account in machine.config" );
					this.run.Messages.AddMessage( @"If ASP.NET cannot be profiled, ensure that the userName=""SYSTEM"" in the <processModel> section of machine.config." );

					return true;
				}

				case ProjectType.VSNet:
				{
					SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x1" );
					SetEnvironmentVariable( "COR_PROFILER", PROFILER_GUID );
					SetEnvironmentVariable( "NPROF_PROFILING_SOCKET", socketServer.Port.ToString() );

					return true;
				}

				default:
					throw new InvalidOperationException( "Unknown project type: " + pi.ProjectType );
			}
		}

		public void Disable()
		{
			SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x0" );
		}

		public void Stop()
		{
			Run r;

			lock ( runLock )
			{
				r = run;

				// Is there anything to stop?
				if ( run == null )
					return;

				run = null;
			}

			// Stop the profiler socket server if profilee hasn't connected
			if ( r.State == RunState.Initializing )
			{
				r.Messages.AddMessage( "Shutting down profiler..." );
				socketServer.Stop();
				r.State = RunState.Finished;
				r.Success = false;
			}

			if ( project.ProjectType == ProjectType.AspNet )
			{
				using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\W3SVC", true ) )
				{
					if ( rk != null )
						SetRegistryKeys( rk, false );
				}

				using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\IISADMIN", true ) )
				{
					if ( rk != null )
						SetRegistryKeys( rk, false );
				}

				r.Messages.AddMessage( "Terminating ASP.NET..." );
				Process.Start( "iisreset.exe", "/stop" ).WaitForExit();
			}
		}

		private void OnProcessExited( object oSender, EventArgs ea )
		{
			Run r;

			lock ( runLock )
			{
				r = run;

				// This gets called twice for file projects - FIXME
				if ( run == null )
					return;

				run = null;
			}

			if ( !socketServer.HasStoppedGracefully )
			{
				if ( r.State == RunState.Initializing )
				{
					r.Messages.AddMessage( "No connection made with profiled application." );
					r.Messages.AddMessage( "Application might not support .NET." );
				}
				else
				{
					r.Messages.AddMessage( "Application stopped before profiler information could be retrieved." );
				}

				r.Success = false;
				r.State = RunState.Finished;
				r.Messages.AddMessage( "Profiler run did not complete successfully." );
			}
			else
			{
				r.Success = true;
			}

			end = DateTime.Now;
			r.Messages.AddMessage( "Stopping profiler listener..." );
			socketServer.Stop();
//			if ( ProcessCompleted != null )
//				ProcessCompleted( _pss.ThreadInfoCollection );

			r.EndTime = end;

			completed( r );
		}

		private void OnError( Exception e )
		{
			MessageBox.Show("An internal exception occurred:\n\n" + e.ToString(), "Error");
			//if ( Error != null )
			//    Error( e );
		}

		private void OnMessage( string strMessage )
		{
			if ( Message != null )
				Message( strMessage );
		}

		public int[] GetFunctionIDs()
		{
			return ( int[] )new ArrayList( functionMap.Keys ).ToArray( typeof( int ) );
		}

		public string GetFunctionSignature( int nFunctionID )
		{
			return ( string )functionMap[ nFunctionID ];
		}

		private void SetRegistryKeys( RegistryKey key, bool isSet )
		{
			if ( key == null )
				return;
			
			if ( !isSet )
			{
				// Get rid of the environment
				key.DeleteValue( "Environment", false );
				return;
			}

			object oKeys = key.GetValue( "Environment" );
			
			// If it's not something we expected, fix it
			if ( oKeys == null || !( oKeys is string[] ) )
				oKeys = new string[ 0 ]; 

			// Save the environment the first time through
			if ( key.GetValue( "nprof Saved Environment" ) == null && ( ( string[] )oKeys ).Length > 0 )
				key.SetValue( "nprof Saved Environment", oKeys );

			Hashtable items = new Hashtable( Environment.GetEnvironmentVariables() );

			// Set the environment to be the default system environment
			using ( RegistryKey rkEnv = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment" ) )
			{
				if ( rkEnv == null )
					throw new InvalidOperationException( "Unable to locate machine environment key" );

				foreach ( string strValueName in rkEnv.GetValueNames() )
					items[ strValueName ] = rkEnv.GetValue( strValueName );
				
			}

			items.Remove( "COR_ENABLE_PROFILING" );
			items.Remove( "COR_PROFILER" );
			items.Remove( "NPROF_PROFILING_SOCKET" );

			items.Add( "COR_ENABLE_PROFILING", "0x1" );
			items.Add( "COR_PROFILER", PROFILER_GUID );
			items.Add( "NPROF_PROFILING_SOCKET", socketServer.Port.ToString() );

			ArrayList itemList = new ArrayList();
			foreach ( DictionaryEntry de in items )
				itemList.Add( String.Format( "{0}={1}", de.Key, de.Value ) );

			key.SetValue( "Environment", ( string[] )itemList.ToArray( typeof( string ) ) );
		}

		public delegate void ProcessCompletedHandler( Run run );
		[field:NonSerialized]
		public event ProcessCompletedHandler ProcessCompleted;
		public delegate void ErrorHandler( Exception e );
		//[field:NonSerialized]
		//public event ErrorHandler Error;
		public delegate void MessageHandler( string strMessage );
		[field:NonSerialized]
		public event MessageHandler Message;

		[NonSerialized]
		private ProcessCompletedHandler completed;
		private DateTime start;
		private DateTime end;
		private Run run;
		private object runLock = 0;

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto )]
		private static extern bool SetEnvironmentVariable( string strVariable, string strNewValue );

		private Hashtable functionMap;
		[NonSerialized]
		private Process process;
		private ProjectInfo project;
		[NonSerialized]
		private ProfilerSocketServer socketServer;
	}
	public class Application
	{
		[STAThread]
		static void Main( string[] args ) 
		{
			System.Windows.Forms.Application.EnableVisualStyles();

			// Always print GPL notice
			Console.WriteLine( "NProf version {0} (C) 2003 by Matthew Mastracci", Profiler.Version );
			Console.WriteLine( "http://nprof.sourceforge.net" );
			Console.WriteLine();
			Console.WriteLine( "This is free software, and you are welcome to redistribute it under certain" );
			Console.WriteLine( "conditions set out by the GNU General Public License.  A copy of the license" );
			Console.WriteLine( "is available in the distribution package and from the NProf web site." );
			Console.WriteLine();

			ProfilerForm form = ProfilerForm.form;
			if (args.Length > 0)
			{
				form.Project = CreateProjectInfo(args);
			}

			Console.Out.Flush();
			System.Threading.Thread.CurrentThread.Name = "GUI Thread";
			form.Show();
			if (form.Project == null)
			{
				PropertiesForm options = new PropertiesForm(PropertiesForm.ProfilerProjectMode.ModifyProject);
				options.ShowDialog();
				form.Project=options.Project;
			}
			try
			{
				System.Windows.Forms.Application.Run();
			}
			catch (Exception e)
			{
			}
		}
		static void PrintUsage()
		{
			Console.WriteLine( "Usage: nprof [/r:<file> [/w:<workingdir>] [/a:<args>]] | [/help] | [/v]" );
			Console.WriteLine();
			Console.WriteLine( "Options (use quotes around any arguments containing spaces)");
			Console.WriteLine( "  /r:<file>        The application to profile" );
			Console.WriteLine( "  /w:<workingdir>  Specifies the working directory for the application" );
			Console.WriteLine( "  /a:<args>        Specifies command line arguments" );
			Console.WriteLine( "  /v               Returns the version of nprof and exists" );
			Console.WriteLine();
			Console.WriteLine( @"Example: Run testapp.exe in C:\Program Files\Test App with ""-i -q"" as arguments" );
			Console.WriteLine( @"  nprof /r:testapp.exe ""/w:C:\Program Files\Test App"" ""/a:-i -q""" );
		}
		static ProjectInfo CreateProjectInfo( string[] args )
		{
			ProjectInfo project = new ProjectInfo( ProjectType.File );
			project.Arguments = project.WorkingDirectory = project.ApplicationName = String.Empty;
			foreach ( string arg in args )
			{
				string upperArg = arg.ToUpper();
				if ( upperArg.StartsWith( "/R:" ) )
				{
					project.ApplicationName = Path.GetFullPath( arg.Substring( 3 ) );
					Console.WriteLine( "Application: " + project.ApplicationName );
				} 
				else if ( upperArg.StartsWith( "/W:" ) )
				{
					project.WorkingDirectory = arg.Substring( 3 );
					Console.WriteLine( "Working Directory: " + project.WorkingDirectory );
				} 
				else if ( upperArg.StartsWith( "/A:" ) )
				{
					project.Arguments = arg.Substring( 3 );
					Console.WriteLine( "Arguments: " + project.Arguments );
				}
				else if ( upperArg.Equals( "/HELP" ) || upperArg.Equals( "/H" ) || upperArg.Equals( "/?" ) || upperArg.Equals( "-H" ) || upperArg.Equals( "-HELP" ) || upperArg.Equals( "--HELP" ) )
				{
					PrintUsage();
					return null;
				}
				else
				{
					Console.WriteLine( @"Error: Unrecognized argument ""{0}"".  Use /help for usage details.", arg );
					return null;
				}
			}
			
			// Check if the user has specified an application to run
			if ( project.ApplicationName.Length == 0 )
			{
				Console.WriteLine( "Error: You need to specify an application to run." );
				return null;
			}
			
			// Set the working directory, if not specified
			if ( project.WorkingDirectory.Length == 0 )
			{
				// Note: if the pInfo.Name is rooted, it will override the app startup path
				project.WorkingDirectory = Path.Combine( Directory.GetCurrentDirectory(), Path.GetDirectoryName( project.ApplicationName ) );
			}

			return project;
		}	
	}
}