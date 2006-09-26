/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003,2004,2005,2006 by Matthew Mastracci, Christian Staudenmeyer
    email                : mmastrac@canada.com
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/

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
using NProf;
using Genghis.Windows.Forms;
using Reflector.UserInterface;
using Crownwood.Magic.Menus;
using System.Globalization;
using DotNetLib.Windows.Forms;

namespace NProf
{
	public class NProf : Form
	{
		public static TextBox application;
		public static TextBox arguments;
		
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();

			Console.WriteLine("NProf version {0} (C) 2003 by Matthew Mastracci", Profiler.Version);
			Console.WriteLine("http://nprof.sourceforge.net");
			Console.WriteLine();
			Console.WriteLine("This is free software, and you are welcome to redistribute it under certain");
			Console.WriteLine("conditions set out by the GNU General Public License.  A copy of the license");
			Console.WriteLine("is available in the distribution package and from the NProf web site.");
			Console.WriteLine();

			NProf form = NProf.form;

			Console.Out.Flush();
			System.Threading.Thread.CurrentThread.Name = "GUI Thread";
			form.Project = new ProjectInfo();
			form.Show();
			Application.Run(form);
		}
		public ProjectInfo Project
		{
			get
			{
				return project;
			}
			set
			{
				project = value;
			}
		}
		private Profiler profiler=new Profiler();
		private ProjectInfo project;
		public TreeView runs;
		private MethodView methods;
		private CallerView callers;
		private TextBox findText = new TextBox();
		public Run currentRun;
		public delegate void OneDelegate<T>(T t);
		public T With<T>(T t,OneDelegate<T> del)
		{
			del(t);
			return t;
		}
		private NProf()
		{
			Size = new Size(800, 600);
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = "NProf";

			string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string dll = Path.Combine(directory, "msvcr70.dll");

			runs = With(new TreeView(), delegate(TreeView tree)
			{
				tree.Dock = DockStyle.Left;
				tree.DoubleClick += delegate
				{
					UpdateFilters((Run)runs.SelectedNode.Tag);// this should also be done when loading a project!!
				};
			});
			callers = With(new CallerView("Callers"), delegate(CallerView method)
			{
				method.Size = new Size(100, 100);
				method.Dock = DockStyle.Bottom;

			});
			methods = With(new MethodView(), delegate(MethodView method)
			{
				method.Size = new Size(100, 100);
				method.Dock = DockStyle.Fill;

				method.SelectedItemsChanged+=delegate
				{
					if (methods.SelectedItems.Count == 0)
						return;
					callers.Items.Clear();
					//if (!isNavigating)
					//{
					//    forward.Clear();
					//    if (currentPosition != 0)
					//    {
					//        backward.Push(currentPosition);
					//    }
					//    currentPosition = (methods.SelectedItems[0].Tag as FunctionInfo).ID;
					//}
					UpdateCallerList(currentRun);
				};
			});
			findText.TextChanged += delegate
			{
				Find(true, false);
			};
			findText.KeyDown += delegate(object sender, KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					Find(true, true);
					e.Handled = true;
				}
			};
			Controls.AddRange(new Control[] 
			{
				With(new Panel(), delegate(Panel panel)
				{
					panel.Dock = DockStyle.Fill;
					panel.Controls.AddRange(new Control[] {
						With(new Panel(),delegate(Panel methodPanel)
					{
						methodPanel.Size = new Size(100, 100);
						methodPanel.Dock = DockStyle.Fill;

						methodPanel.Controls.AddRange(new Control[] {
							methods,
							With(new Splitter(),delegate(Splitter splitter)
							{
								splitter.Dock=DockStyle.Bottom;
							}),
							With(new Splitter(),delegate(Splitter splitter)
							{
								splitter.Dock=DockStyle.Bottom;
							}),
							callers,
							With(new FlowLayoutPanel(),delegate(FlowLayoutPanel p)
							{
							    p.WrapContents = false;
							    p.AutoSize = true;
							    p.Dock = DockStyle.Top;
							    p.Controls.AddRange(new Control[] {
							        With(new Label(),delegate(Label label)
							    {
							        label.Text = "Find:";
									label.Dock=DockStyle.Fill;
							        label.TextAlign = ContentAlignment.MiddleLeft;
							        label.AutoSize = true;
							    }),
							        findText,
							        With(new Button(),delegate(Button button)
							        {
							            button.AutoSize = true;
							            button.Text = "Find next";
							            button.Click += new EventHandler(findNext_Click);
							        }),
							        With(new Button(),delegate(Button button)
							        {
							            button.AutoSize = true;
							            button.Click += new EventHandler(findPrevious_Click);
							            button.Text = "Find previous";
							        })});
							}
							)

						});


					}),
						With(new Splitter(),delegate(Splitter splitter)
						{
							splitter.Dock = DockStyle.Left;
						}),
						runs
					});

				}),
				With(new TableLayoutPanel(),delegate(TableLayoutPanel panel)
				{
					application = With(new TextBox(),delegate(TextBox textBox)
					{
						textBox.TextChanged += delegate
						{
							Project.ApplicationName = application.Text;
						};
						textBox.Width=300;
					});
					arguments = With(new TextBox(),delegate(TextBox textBox)
					{
						textBox.TextChanged+=delegate
						{
							Project.Arguments=textBox.Text;
						};
						textBox.Width=300;
					});
					panel.Height=100;
					panel.AutoSize=true;
					panel.Dock = DockStyle.Top;
					panel.Controls.Add(With(new Label(), delegate(Label label)
					{
						label.Text = "Application:";
						label.Dock=DockStyle.Fill;
						label.TextAlign = ContentAlignment.MiddleLeft;
						label.AutoSize=true;
					}),0,0);
					panel.Controls.Add(application,1,0);

					panel.Controls.Add(With(new Button(),delegate(Button button)
					{
						button.Text = "Browse...";
						button.Focus();
						button.Click += delegate
						{
							OpenFileDialog dialog = new OpenFileDialog();
							dialog.Filter = "Executable files (*.exe)|*.exe";
							DialogResult dr = dialog.ShowDialog();
							if (dr == DialogResult.OK)
							{
								application.Text = dialog.FileName;
								application.Focus();
								application.SelectAll();
							}
						};
					}),2,0);
					panel.Controls.Add(With(new Label(),delegate(Label label)
						{
							label.Text = "Arguments:";
							label.Dock=DockStyle.Fill;
							label.TextAlign = ContentAlignment.MiddleLeft;
							label.AutoSize=true;
						}),0,1);
					panel.Controls.Add(arguments,1,1);
				}),
				With(new MenuControl(),delegate(MenuControl mainMenu)
				{
					mainMenu.Dock = DockStyle.Top;
					mainMenu.MenuCommands.AddRange(new MenuCommand[]
					{
						new Menu("File",
							new Menu("&New...","Create a new profile project",New),
							new Menu("-","-",null),
							new Menu("E&xit","Exit the application",Shortcut.AltF4,delegate {Close();})),
						new Menu("Project",
							new Menu("Start","Run the current project",Shortcut.F5,StartRun),
							new Menu("-","-",null),
							new Menu("About nprof...","About nprof",About))
					});
				})});
		}
		private delegate void HandleProfileComplete(Run run);

		private void ProfilerForm_Load(object sender, System.EventArgs e)
		{
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = "nprof Profiling Application - v" + Profiler.Version;
		}

		private void ProfilerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			profiler.Stop();
		}
		private void New(object sender, System.EventArgs e)
		{
			runs.Nodes.Clear();
			NProf.arguments.Text = "";
			NProf.application.Text = "";
			methods.Items.Clear();
			callers.Items.Clear();
		}
		private void StartRun(object sender, System.EventArgs e)
		{
			string message;
			bool success = profiler.CheckSetup(out message);
			if (!success)
			{
				MessageBox.Show(this, message, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Run run = Project.CreateRun(profiler);
			run.profiler.Completed += run.Complete;
			run.Start();
		}
		private void About(object sender, System.EventArgs e)
		{
			new AboutForm().ShowDialog(this);
		}
		private class Images
		{
			private static Image[] images = null;
			static Images()
			{
				Bitmap bitmap = (Bitmap)Bitmap.FromStream(typeof(Images).Assembly.GetManifestResourceStream("NProf.Resources.toolbar16.png"));
				int count = (int)(bitmap.Width / bitmap.Height);
				images = new Image[count];
				Rectangle rectangle = new Rectangle(0, 0, bitmap.Height, bitmap.Height);
				for (int i = 0; i < count; i++)
				{
					images[i] = bitmap.Clone(rectangle, bitmap.PixelFormat);
					rectangle.X += bitmap.Height;
				}
			}
			public static Image New { get { return images[0]; } }
			public static Image Back { get { return images[17]; } }
			public static Image Forward { get { return images[18]; } }
			public static Image Run { get { return images[37]; } }
		}
		public void UpdateRuns(Run run)
		{
			TreeNode node = new TreeNode(run.StartTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern));
			node.Tag = run;
			runs.Nodes.Add(node);
			UpdateFilters(run);
		}
		private void UpdateFilters(Run run)
		{
			methods.SuspendLayout();
			methods.BeginUpdate();

			methods.Items.Clear();
			callers.Items.Clear();

			currentRun = run;
			foreach (FunctionInfo method in run.functions)
			{
				methods.Add(method);
			}
			methods.EndUpdate();
			methods.ResumeLayout();
		}
		private void UpdateCallerList(Run run)
		{
			callers.BeginUpdate();
			ListView l;

			FunctionInfo mfi = (FunctionInfo)methods.SelectedItems[0].Tag;
			foreach (FunctionInfo fi in run.functions)
			{
				foreach (FunctionInfo cfi in fi.Callees)
				{
					if (cfi.ID == mfi.ID)
					{
						callers.Add(fi);
					}
				}
			}
			callers.Sort();
			callers.EndUpdate();
		}
		public const string timeFormat = "0.00;-0.00;0.0";
		private void JumpToID(int id)
		{
			methods.SelectedItems.Clear();
			foreach (ContainerListViewItem lvi in methods.Items)
			{
				FunctionInfo fi = (FunctionInfo)lvi.Tag;

				if (fi.ID == id)
				{
					if (!lvi.Selected)
					{
						lvi.Selected = true;
						lvi.Focused = true;
					}
					break;
				}
			}
			methods.Focus();
		}
		private int GetSelectedID()
		{
			return ((FunctionInfo)methods.SelectedItems[0].Tag).ID;
		}
		private void Find(bool forward,bool step)
		{
			if (findText.Text != "")
			{
				ContainerListViewItem item;
				if (methods.SelectedItems.Count == 0)
				{
					if (methods.SelectedItems.Count == 0)
					{
						item = null;
					}
					else
					{
						item = methods.Items[0];
					}
				}
				else
				{
					if (step)
					{
						if (forward)
						{
							item = methods.SelectedItems[0];
						}
						else
						{
							item = methods.SelectedItems[0];
						}
					}
					else
					{
						item = methods.SelectedItems[0];
					}
				}
				ContainerListViewItem firstItem = item;
				while (item != null)
				{
					if (item.Text.ToLower().Contains(findText.Text.ToLower()))
					{
						methods.SelectedItems.Clear();
						//methods.SelectedNodes.Clear();
						item.Focused = true;
						item.Selected = true;
						this.Invalidate();
						break;
					}
					else
					{
						if (forward)
						{
							item = item.NextItem;//.NextVisibleItem;
							//item = (TreeListNode)item.PreviousSibling();//.NextVisibleItem;
						}
						else
						{
							item = item.PreviousItem;//.PrevVisibleItem;
						}
						if (item == null)
						{
							if (forward)
							{
								item = methods.Items[0];
							}
							else
							{
								item = methods.Items[methods.Items.Count - 1];
							}
						}
					}
					if (item == firstItem)
					{
						break;
					}
				}
			}
		}
		private void findNext_Click(object sender, EventArgs e)
		{
			Find(true,true);
		}
		private void findPrevious_Click(object sender, EventArgs e)
		{
			Find(false,true);
		}
		public static NProf form = new NProf();
	}
	public class MethodView : ContainerListView
	{
		public MethodView()
		{
			Columns.Add("Methods");
			Columns.Add("Time");
			Columns[0].Width = 350;
			Columns[0].SortDataType = SortDataType.String;
			Columns[1].SortDataType = SortDataType.Double;
			this.ShowPlusMinus = true;
			ShowRootTreeLines = true;
			ShowTreeLines = true;

			HeaderStyle = ColumnHeaderStyle.Clickable;
			Font = new Font("Tahoma", 8.0f);
		}
		public static int counter = 0;
		public void FunctionItem(ContainerListViewItemCollection parent,FunctionInfo function)
		{
			counter++;
			ContainerListViewItem item=parent.Add(function.Signature);
			item.SubItems[1].Text = function.Time.ToString(NProf.timeFormat);
			item.Tag = function;
			foreach (FunctionInfo callee in function.Callees)
			{
				FunctionItem(item.Items,callee);
			}
		}
		public void Add(FunctionInfo function)
		{
			FunctionItem(Items, function);
		}
	}
	public class CallerView : DotNetLib.Windows.Forms.ContainerListView
	{
		public CallerView(string name)
		{
			Columns.Add(name);
			Columns.Add("Time");

			HeaderStyle = ColumnHeaderStyle.Clickable;
			Columns[0].Width = 350;
	
			ColumnSortColor = Color.White;
			Font = new Font("Tahoma", 8.0f);
		}
		public void Add(FunctionInfo function)
		{
			DotNetLib.Windows.Forms.ContainerListViewItem item = Items.Add(function.Signature);
			item.SubItems[1].Text = function.Time.ToString(NProf.timeFormat);
			item.Tag = function;
		}
	}
	public class Menu : MenuCommand
	{
		public Menu(string text, string description, EventHandler click)
			:
			this(text, description, Shortcut.None, click)
		{
		}
		public Menu(string text, string description, Shortcut shortcut, EventHandler click)
			:
			base(text, shortcut, click)
		{
			Description = description;
		}
		public Menu(string text, params MenuCommand[] commands)
			: base(text)
		{
			MenuCommands.AddRange(commands);
		}
	}
	public class ProfilerSocketServer
	{
		public ProfilerSocketServer(Run run)
		{
			this.run = run;
			this.stopFlag = 0;
			this.hasStopped = false;
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
					while (true)
					{
						int functionId = reader.ReadInt32();
						if (functionId == -1)
						{
							break;
						}
						run.signatures.MapSignature(functionId, new FunctionSignature(
							reader.ReadUInt32(),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader)
						));
					}
					List<FunctionInfo> functions = new List<FunctionInfo>();
					GetFunctions(reader, functions,null);
					StartFix(functions,null);
				}
			}
			catch (Exception e)
			{
				if (Error != null)
					Error(e);
			}
		}

		private void StartFix(List<FunctionInfo> functions,FunctionInfo parent)
		{
			FixFunctions(functions,run.functions,parent);
			foreach (FunctionInfo f in functions)
			{
				StartFix(f.Callees,f);
			}
		}
		private void FixFunctions(List<FunctionInfo> functions, List<FunctionInfo> targetFunctions,FunctionInfo parent)
		{
			foreach (FunctionInfo f in functions)
			{
				FunctionInfo function = GetFunctionInfo(targetFunctions, f.ID, run.signatures,parent);
				FunctionInfo search=f;
				//while (search.parent != null)
				//{
				//    //if(
				//    search = search.parent;
				//}
				function.calls += f.calls;
				FixFunctions(f.Callees, function.Callees,function);
			}
		}
		private static FunctionInfo GetFunctionInfo(List<FunctionInfo> functions, int id,FunctionSignatureMap signatures,FunctionInfo parent)
		{
			foreach (FunctionInfo function in functions)
			{
				if (function.ID == id)
				{
					return function;
				}
			}
			functions.Add(new FunctionInfo(id,signatures,0,parent));
			return functions[functions.Count-1];
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
		private void GetFunctions(BinaryReader reader, List<FunctionInfo> functions,FunctionInfo parent)
		{
			while (true)
			{
				int functionId = reader.ReadInt32();
				if (functionId == -1)
				{
					break;
				}
				int callCount = reader.ReadInt32();
				FunctionInfo function = new FunctionInfo(functionId, run.signatures, callCount,parent);
				GetFunctions(reader, function.Callees,function);
				functions.Add(function);
			}
		}

		public int Port
		{
			get { return port; }
		}

		public event ErrorHandler Error;

		public delegate void ErrorHandler(Exception e);
		public delegate void MessageHandler(string strMessage);


		private int port;
		private int stopFlag;
		private ManualResetEvent resetStarted;
		private ManualResetEvent resetMessageReceived;
		private Thread thread;
		private Socket socket;
		private Run run;
		private bool hasStopped;
	}
	public class FunctionInfo
	{
		private const double frequency = 10.0;
		public double Time
		{
			get
			{
				return ((double)(calls) * 10)/1000.0;///1000.0;
			}
		}
		public FunctionInfo parent;
		public FunctionInfo(int nID, FunctionSignatureMap signatures, int calls,FunctionInfo parent)
		{
			this.id = nID;
			this.calls = calls;
			this.signatures = signatures;
			this.parent = parent;
		}

		public int ID
		{
			get { return id; }
			set { id = value; }
		}
		public string Signature
		{
			get { return signatures.GetFunctionSignature(id); }
		}
		public int lastWalk = 0;

		public List<FunctionInfo> Callees
		{
			get { return callees; }
			set { callees = value; }
		}
		//public double Calls
		//{
		//    get { return calls/1000.0d; }
		//}
		private int id;
		public int calls;
		private List<FunctionInfo> callees=new List<FunctionInfo>();
		FunctionSignatureMap signatures;
	}
	public class FunctionSignature
	{
		public FunctionSignature(UInt32 methodAttributes, string returnType, string className, string functionName, string parameters)
		{
			CorMethodAttr cma = (CorMethodAttr)methodAttributes;
			this.isPInvoke = (cma & CorMethodAttr.mdPinvokeImpl) != 0;
			this.isStatic = (cma & CorMethodAttr.mdStatic) != 0;
			this.isExtern = (cma & CorMethodAttr.mdUnmanagedExport) != 0;

			this.returnType = returnType;
			this.className = className;
			this.functionName = functionName;
			this.parameters = parameters;
			this.signature=className + "." + functionName + "(" + parameters + ")";
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
				string[] astrPieces = className.Split('.');
				string[] astrNamespace = new String[astrPieces.Length - 1];
				Array.Copy(astrPieces, 0, astrNamespace, 0, astrPieces.Length - 1);

				return astrNamespace;
			}
		}

		public string NamespaceString
		{
			get { return String.Join(".", Namespace); }
		}

		private string signature;
		public string Signature
		{
			get
			{
				return signature;
				//return className + "." + functionName + "(" + parameters + ")";
			}
		}

		[Flags]
		enum CorMethodAttr
		{
			// member access mask - Use this mask to retrieve accessibility information.
			mdMemberAccessMask = 0x0007,
			mdPrivateScope = 0x0000,     // Member not referenceable.
			mdPrivate = 0x0001,     // Accessible only by the parent type.  
			mdFamANDAssem = 0x0002,     // Accessible by sub-types only in this Assembly.
			mdAssem = 0x0003,     // Accessibly by anyone in the Assembly.
			mdFamily = 0x0004,     // Accessible only by type and sub-types.    
			mdFamORAssem = 0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
			mdPublic = 0x0006,     // Accessibly by anyone who has visibility to this scope.    
			// end member access mask

			// method contract attributes.
			mdStatic = 0x0010,     // Defined on type, else per instance.
			mdFinal = 0x0020,     // Method may not be overridden.
			mdVirtual = 0x0040,     // Method virtual.
			mdHideBySig = 0x0080,     // Method hides by name+sig, else just by name.

			// vtable layout mask - Use this mask to retrieve vtable attributes.
			mdVtableLayoutMask = 0x0100,
			mdReuseSlot = 0x0000,     // The default.
			mdNewSlot = 0x0100,     // Method always gets a new slot in the vtable.
			// end vtable layout mask

			// method implementation attributes.
			mdAbstract = 0x0400,     // Method does not provide an implementation.
			mdSpecialName = 0x0800,     // Method is special.  Name describes how.

			// interop attributes
			mdPinvokeImpl = 0x2000,     // Implementation is forwarded through pinvoke.
			mdUnmanagedExport = 0x0008,     // Managed method exported via thunk to unmanaged code.

			// Reserved flags for runtime use only.
			mdReservedMask = 0xd000,
			mdRTSpecialName = 0x1000,     // Runtime should check name encoding.
			mdHasSecurity = 0x4000,     // Method has security associate with it.
			mdRequireSecObject = 0x8000,     // Method calls another method containing security code.

		} ;

		private bool isPInvoke;
		private bool isStatic;
		private bool isExtern;
		private string returnType;
		private string className;
		private string functionName;
		private string parameters;
	}
	public class FunctionSignatureMap
	{
		public FunctionSignatureMap()
		{
			signatures = new Hashtable();
		}

		public void MapSignature(int functionID, FunctionSignature signature)
		{
			lock (signatures.SyncRoot)
			{
				signatures[functionID] = signature;
			}
		}
		public string GetFunctionSignature(int functionID)
		{
			//lock (signatures.SyncRoot)
			//{
				FunctionSignature signature = (FunctionSignature)signatures[functionID];
				if (signature == null)
				{
					return "Unknown!";
				}

				return signature.Signature;
			//}
		}
		//public string GetFunctionSignature(int functionID)
		//{
		//    lock (signatures.SyncRoot)
		//    {
		//        FunctionSignature signature = (FunctionSignature)signatures[functionID];
		//        if (signature == null)
		//        {
		//            return "Unknown!";
		//        }

		//        return signature.Signature;
		//    }
		//}

		private Hashtable signatures;
	}

	public class ProjectInfo
	{
		public ProjectInfo()
		{
			this.name = null;
			this.runs = new RunCollection(this);
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

		public Run CreateRun(Profiler p)
		{
			Run run = new Run(p, this);
			runs.Add(run);

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

		private void Fire_ProjectInfoChanged()
		{
			if (ProjectInfoChanged != null)
				ProjectInfoChanged(this);
		}
		public event ProjectEventHandler ProjectInfoChanged;

		public delegate void ProjectEventHandler(ProjectInfo project);

		private string appName;
		private string arguments;
		private string workingDirectory;
		private string name;
		private bool debugHook;
		private RunCollection runs;
	}
	public class Run
	{
		public void Complete(object sender,EventArgs e)
		{
			NProf.form.BeginInvoke(new EventHandler(delegate
			{
				NProf.form.UpdateRuns(this);
				profiler.Completed -= Complete;
			}));
		}
		public FunctionSignatureMap signatures = new FunctionSignatureMap();
		public List<FunctionInfo> functions = new List<FunctionInfo>();
		//public Dictionary<int, FunctionInfo> functions = new Dictionary<int, FunctionInfo>();

		public Run(Profiler p, ProjectInfo pi)
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
			this.project = pi;
			this.isSuccess = false;
		}
		public bool Start()
		{
			start = DateTime.Now;

			return profiler.Start(project, this);
		}

		public bool Stop()
		{
			profiler.Stop();

			return true;
		}

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

		public bool Success
		{
			get { return isSuccess; }
			set { isSuccess = value; }
		}
		private bool isSuccess;
		private DateTime start;
		private DateTime end;
		private ProjectInfo project;
		public Profiler profiler;
	}
	// remove
	public class RunCollection : IEnumerable
	{
		public RunCollection(ProjectInfo pi)
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

		public void Add(Run run)
		{
			items.Add(run);
			if (RunAdded != null)
				RunAdded(project, this, run, items.Count - 1);
		}

		public void RemoveAt(int index)
		{
			Run run = this[index];
			items.RemoveAt(index);
			if (RunRemoved != null)
				RunRemoved(project, this, run, index);
		}

		public Run this[int nIndex]
		{
			get { return (Run)items[nIndex]; }
		}
		public event RunEventHandler RunAdded;
		public event RunEventHandler RunRemoved;

		public delegate void RunEventHandler(ProjectInfo pi, RunCollection runs, Run run, int nIndex);

		private ArrayList items;
		private ProjectInfo project;
	}
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
		public event EventHandler Completed;


		public bool CheckSetup(out string message)
		{
			message = String.Empty;
			using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + PROFILER_GUID))
			{
				if (rk == null)
				{
					message = "Unable to find the registry key for the profiler hook.  Please register the NProf.Hook.dll file.";
					return false;
				}
			}

			return true;
		}

		public bool Start(ProjectInfo pi, Run run)
		{
			this.start = DateTime.Now;
			this.project = pi;
			this.run = run;

			socketServer = new ProfilerSocketServer(run);
			socketServer.Start();
			//socketServer.Exited += new EventHandler(OnProcessExited);
			socketServer.Error += new ProfilerSocketServer.ErrorHandler(OnError);

			process = new Process();
			process.StartInfo = new ProcessStartInfo(pi.ApplicationName, pi.Arguments);
			process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
			process.StartInfo.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
			process.StartInfo.EnvironmentVariables["NPROF_PROFILING_SOCKET"] = socketServer.Port.ToString();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = pi.Arguments;
			process.StartInfo.WorkingDirectory = pi.WorkingDirectory;
			process.EnableRaisingEvents = true;
			process.Exited += new EventHandler(OnProcessExited);

			return process.Start();
		}
		public void Stop()
		{
			Run r;

			lock (runLock)
			{
				r = run;

				// Is there anything to stop?
				if (run == null)
					return;

				run = null;
			}
				socketServer.Stop();
				r.Success = false;


		}

		private void OnProcessExited(object oSender, EventArgs ea)
		{
			Run r;
			lock (runLock)
			{
				r = run;

				// This gets called twice for file projects - FIXME
				if (run == null)
					return;

				run = null;
			}

			if (!socketServer.HasStoppedGracefully)
			{
				r.Success = false;
			}
			else
			{
				r.Success = true;
			}

			end = DateTime.Now;
			socketServer.Stop();

			r.EndTime = end;

			if (Completed != null)
			{
				Completed(this, new EventArgs());
			}
		}

		private void OnError(Exception e)
		{
			MessageBox.Show("An internal exception occurred:\n\n" + e.ToString(), "Error");
		}

		private void OnMessage(string strMessage)
		{
			if (Message != null)
				Message(strMessage);
		}

		public int[] GetFunctionIDs()
		{
			return (int[])new ArrayList(functionMap.Keys).ToArray(typeof(int));
		}

		public string GetFunctionSignature(int nFunctionID)
		{
			return (string)functionMap[nFunctionID];
		}
		
		public delegate void ProcessCompletedHandler(Run run);
		public delegate void ErrorHandler(Exception e);
		public delegate void MessageHandler(string strMessage);
		public event MessageHandler Message;

		private DateTime start;
		private DateTime end;
		private Run run;
		private object runLock = 0;

		private Hashtable functionMap;
		private Process process;
		private ProjectInfo project;
		private ProfilerSocketServer socketServer;
	}
}