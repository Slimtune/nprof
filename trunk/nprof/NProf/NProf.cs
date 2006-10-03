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
			Application.Run(form);
		}
		private Profiler profiler=new Profiler();
		public ListView runs;
		private MethodView methods;
		private MethodView callers;
		private TextBox findText = new TextBox();
		public delegate void SingleArgument<T>(T t);
		private FlowLayoutPanel findPanel;
		public T With<T>(T t,SingleArgument<T> del)
		{
			del(t);
			return t;
		}
		public void ShowSearch()
		{
			findPanel.Visible = !findPanel.Visible;
			findText.Focus();
		}
		private NProf()
		{
			Size = new Size(800, 600);
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = "NProf";
			runs = With(new ListView(), delegate(ListView tree)
			{
				tree.Dock = DockStyle.Fill;
				tree.DoubleClick += delegate
				{
					if (runs.SelectedItems.Count != 0)
					{
						UpdateFilters((Run)runs.SelectedItems[0].Tag);
					}
				};
			});
			callers = With(new MethodView("Callers"), delegate(MethodView method)
			{
				method.Size = new Size(100, 200);
				method.Dock = DockStyle.Bottom;
			});
			methods = With(new MethodView("Callees"), delegate(MethodView method)
			{
				method.Size = new Size(100, 100);
				method.Dock = DockStyle.Fill;
				method.DoubleClick += delegate
				{
					if (method.SelectedItems.Count != 0)
					{
						FunctionInfo function = (FunctionInfo)method.SelectedItems[0].Tag;
						callers.SelectedItems.Clear();
						foreach (ContainerListViewItem item in method.Items)
						{
							if (((FunctionInfo)item.Tag).ID == function.ID)
							{
								callers.SelectedItems.Add(item);
								break;
							}
						}
					}
				};
				method.SelectedItemsChanged+=delegate
				{
					if (method.SelectedItems.Count != 0)
					{
						ContainerListViewItem item=method.SelectedItems[0];
						if (item.Items.Count == 0)
						{
							foreach (FunctionInfo f in ((FunctionInfo)item.Tag).Callees.Values)
							{
								method.FunctionItem(item.Items, f);
							}
						}
					}
					//if (methods.SelectedItems.Count == 0)
					//    return;
					//callers.Items.Clear();
					//UpdateCallerList(currentRun);
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
			Menu = new MainMenu(new MenuItem[]
				{
					new MenuItem(
						"File",
						new MenuItem[] 
						{
							new MenuItem("About nprof...",delegate{new AboutForm().ShowDialog(this);}),
							new MenuItem("-"),
							new MenuItem("E&xit",delegate {Close();})
						}),
					new MenuItem(
						"Project",
						new MenuItem[]
						{
							new MenuItem("Start",delegate{StartRun();},Shortcut.F5),
							new MenuItem("-"),
							new MenuItem("&New...",New,Shortcut.CtrlN),
							new MenuItem("-"),
							new MenuItem("Find",delegate{ShowSearch();},Shortcut.CtrlF)
						})					
				});

			Controls.AddRange(new Control[] 
			{
				With(new Panel(), delegate(Panel panel)
				{
					//panel.BorderStyle=BorderStyle.Fixed3D;

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
							callers,
							With(new FlowLayoutPanel(),delegate(FlowLayoutPanel p)
							{
								findPanel=p;
								p.Visible=false;
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
						With(new Panel(),delegate(Panel p)
						{
							p.Controls.Add(runs);
							p.Dock=DockStyle.Left;
							p.Controls.Add(With(new Label(),delegate(Label l)
							{
								l.Text="Completed runs:";
						        l.TextAlign = ContentAlignment.MiddleLeft;
						        l.AutoSize = true;
								l.Dock=DockStyle.Top;
							}));
						})
					});

				}),
				With(new Splitter(),delegate(Splitter splitter)
				{
					splitter.Dock = DockStyle.Top;
				}),
				With(new TableLayoutPanel(),delegate(TableLayoutPanel panel)
				{
					application = With(new TextBox(),delegate(TextBox textBox)
					{
						textBox.Width=300;
					});
					arguments = With(new TextBox(),delegate(TextBox textBox)
					{
						textBox.Width=300;
					});
					//panel.BorderStyle=BorderStyle.Fixed3D;
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
					//panel.Controls.Add(With(new Button(),delegate(Button button)
					//{
					//    button.Text="Start";
					//    button.Click+=delegate
					//    {
					//        StartRun();
					//    };
					//}),3,0);
					panel.Controls.Add(With(new Label(),delegate(Label label)
						{
							label.Text = "Arguments:";
							label.Dock=DockStyle.Fill;
							label.TextAlign = ContentAlignment.MiddleLeft;
							label.AutoSize=true;
						}),0,1);
					panel.Controls.Add(arguments,1,1);
				}),
			});
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
			runs.Items.Clear();
			NProf.arguments.Text = "";
			NProf.application.Text = "";
			methods.Items.Clear();
			callers.Items.Clear();
		}
		private void StartRun()
		{
			string message;
			bool success = profiler.CheckSetup(out message);
			if (!success)
			{
				MessageBox.Show(this, message, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Run run = new Run(profiler);
			run.profiler.Completed += run.Complete;
			run.Start();
		}
		public void UpdateRuns(Run run)
		{
			ListViewItem item = new ListViewItem(DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern));
			item.Tag = run;
			runs.Items.Add(item);
			UpdateFilters(run);
		}
		private void UpdateFilters(Run run)
		{
			methods.SuspendLayout();
			methods.BeginUpdate();
			callers.BeginUpdate();

			methods.Items.Clear();
			callers.Items.Clear();

			foreach (FunctionInfo method in run.functions.Values)
			{
				methods.Add(method);
			}
			foreach (FunctionInfo method in run.callers.Values)
			{
				callers.Add(method);
			}
			callers.EndUpdate();
			methods.EndUpdate();
			methods.ResumeLayout();
		}
		private void UpdateCallerList(Run run)
		{
			callers.BeginUpdate();

			FunctionInfo mfi = (FunctionInfo)methods.SelectedItems[0].Tag;
			foreach (FunctionInfo fi in run.functions.Values)
			{
				foreach (FunctionInfo cfi in fi.Callees.Values)
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
						item.Focused = true;
						item.Selected = true;
						this.Invalidate();
						break;
					}
					else
					{
						if (forward)
						{
							item = item.NextItem;
						}
						else
						{
							item = item.PreviousItem;
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
		public MethodView(string name)
		{
			Columns.Add(name);
			Columns.Add("Time");
			Columns[0].Width = 350;
			Columns[0].SortDataType = SortDataType.String;
			Columns[1].SortDataType = SortDataType.Double;
			this.ShowPlusMinus = true;
			ShowRootTreeLines = true;
			ShowTreeLines = true;
			ColumnSortColor = Color.White;

			HeaderStyle = ColumnHeaderStyle.Clickable;
			Font = new Font("Tahoma", 8.0f);
			this.Click += delegate
			{
				//ContainerListViewItem item=this.GetItemAt(this.PointToClient(Control.MousePosition).Y);
				//if (item != null)
				//{
				UpdateView();
				//}
			};
			this.KeyDown += delegate(object sender,KeyEventArgs e)
			{
				if (SelectedItems.Count != 0)
				{
					UpdateView();
				}
			};
		}
		void UpdateView()
		{
			SuspendLayout();
			BeginUpdate();
			ContainerListViewItem item = this.BottomItemPartiallyVisible;
			while (item != null)
			{
				//foreach (ContainerListViewItem subItem in item.Items)
				//{
				//item.Items.Clear();
				//if (item.Items.Count == 0)
				//{
				//    foreach (FunctionInfo function in ((FunctionInfo)item.Tag).Callees.Values)
				//    {
				//        FunctionItem(item.Items, function);
				//    }
				//    foreach (ContainerListViewItem i in item.Items)
				//    {
				//        if (i.Expanded)
				//        {
				//            FunctionItem(i.Items, (FunctionInfo)i.Tag);
				//        }
				//    }
				//    //foreach (ContainerListViewItem i in item.Items)
				//    //{
				//    //    FunctionItem(i.Items, (FunctionInfo)i.Tag);
				//    //}

				//    //if (item.Items.Count == 0 && ((FunctionInfo)item.Tag).Callees.Values.Count != 0)
				//    //{
				//    //}
				//}

					//if (subItem.Items.Count == 0)
					//{
					//    foreach (FunctionInfo function in ((FunctionInfo)subItem.Tag).Callees.Values)
					//    {
					//        FunctionItem(subItem.Items, function);
					//    }
					//}
				//}
				item = item.PreviousVisibleItem;
			}
			EndUpdate();
			ResumeLayout();
		}
		private ContainerListViewItem AddItem(ContainerListViewItemCollection parent, FunctionInfo function)
		{
			ContainerListViewItem item = parent.Add(function.Signature);
			item.SubItems[1].Text = function.Time.ToString(NProf.timeFormat);
			item.Tag = function;
			return item;
		}
		public void FunctionItem(ContainerListViewItemCollection parent,FunctionInfo function)
		{
			ContainerListViewItem item=AddItem(parent, function);
			foreach (FunctionInfo callee in function.Callees.Values)
			{
				AddItem(item.Items, callee);

				////FunctionItem(item.Items, callee);

				////ContainerListViewItem subItem = AddItem(item.Items, callee);
				////foreach (FunctionInfo subCallee in callee.Callees.Values)
				////{
				////    AddItem(subItem.Items, subCallee);
				////}
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
					while (true)
					{
						List<int> stackWalk = new List<int>();
						while (true)
						{
							int functionId = reader.ReadInt32();
							if (functionId == -1)
							{
								break;
							}
							else if (functionId == -2)
							{
								return;
							}
							stackWalk.Add(functionId);
						}
						run.stackWalks.Add(stackWalk);
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("An internal exception occurred:\n\n" + e.ToString(), "Error");
			}
		}
		public static FunctionInfo GetFunctionInfo(Dictionary<int, FunctionInfo> functions, int id, FunctionSignatureMap signatures)
		{
			FunctionInfo result;
			if (!functions.TryGetValue(id,out result))
			{
				result =new FunctionInfo(id, signatures, 0);
				functions[id]=result;
			}
			return result;
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

		public int Port
		{
			get { return port; }
		}
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
		public FunctionInfo(int nID, FunctionSignatureMap signatures, int calls)
		{
			this.id = nID;
			this.calls = calls;
			this.signatures = signatures;
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

		public Dictionary<int,FunctionInfo> Callees
		{
			get { return callees; }
			set { callees = value; }
		}
		private int id;
		public int calls;
		private Dictionary<int, FunctionInfo> callees = new Dictionary<int, FunctionInfo>();
		//private List<FunctionInfo> callees = new List<FunctionInfo>();
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
			signatures[functionID] = signature;
		}
		public string GetFunctionSignature(int functionID)
		{
			FunctionSignature signature = (FunctionSignature)signatures[functionID];
			if (signature == null)
			{
				return "Unknown!";
			}

			return signature.Signature;
		}
		private Hashtable signatures;
	}
	public class Run
	{
		public List<List<int>> stackWalks = new List<List<int>>();

		private void InterpreteData()
		{
			int currentWalk = 0;
			foreach (List<int> stackWalk in stackWalks)
			{
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++)
				{
					Dictionary<int, FunctionInfo> currentMap = functions;
					for (int index = stackWalk.Count - 1 - i; index >= 0; index--)
					{
						FunctionInfo function = ProfilerSocketServer.GetFunctionInfo(currentMap, stackWalk[index], signatures);
						if (function.lastWalk != currentWalk)
						{
							function.calls++;
						}
						function.lastWalk = currentWalk;
						currentMap = function.Callees;
					}
				}
			}
			foreach (List<int> stackWalk in stackWalks)
			{
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++)
				{
					Dictionary<int, FunctionInfo> currentMap = callers;
					for (int index = i;index<stackWalk.Count; index++)
					{
						FunctionInfo function = ProfilerSocketServer.GetFunctionInfo(currentMap, stackWalk[index], signatures);
						if (function.lastWalk != currentWalk)
						{
							function.calls++;
						}
						function.lastWalk = currentWalk;
						currentMap = function.Callees;
					}
				}
			}
			int asdf = 0;
		}
		public void Complete(object sender,EventArgs e)
		{
			NProf.form.BeginInvoke(new EventHandler(delegate
			{
				InterpreteData();
				NProf.form.UpdateRuns(this);
				profiler.Completed -= Complete;
			}));
		}
		public FunctionSignatureMap signatures = new FunctionSignatureMap();
		public Dictionary<int, FunctionInfo> functions = new Dictionary<int, FunctionInfo>();
		public Dictionary<int, FunctionInfo> callers = new Dictionary<int, FunctionInfo>();

		public Run(Profiler p)
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
			this.isSuccess = false;
		}
		public bool Start()
		{
			start = DateTime.Now;

			return profiler.Start(this);
		}

		public bool Stop()
		{
			profiler.Stop();

			return true;
		}


		public bool Success
		{
			get { return isSuccess; }
			set { isSuccess = value; }
		}
		private bool isSuccess;
		private DateTime start;
		private DateTime end;
		public Profiler profiler;
	}
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";

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

		public bool Start(Run run)
		{
			this.run = run;
			socketServer = new ProfilerSocketServer(run);
			socketServer.Start();
			//socketServer.Error += new ProfilerSocketServer.ErrorHandler(OnError);
			process = new Process();
			process.StartInfo = new ProcessStartInfo(NProf.application.Text,NProf.arguments.Text);
			process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
			process.StartInfo.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
			process.StartInfo.EnvironmentVariables["NPROF_PROFILING_SOCKET"] = socketServer.Port.ToString();
			process.StartInfo.UseShellExecute = false;
			process.EnableRaisingEvents = true;
			process.Exited += new EventHandler(OnProcessExited);
			return process.Start();
		}
		public void Stop()
		{
			if (run != null)
			{
				socketServer.Stop();
				run.Success = false;
			}
		}

		private void OnProcessExited(object oSender, EventArgs ea)
		{
			if (!socketServer.HasStoppedGracefully)
			{
				run.Success = false;
			}
			else
			{
				run.Success = true;
			}

			socketServer.Stop();

			if (Completed != null)
			{
				Completed(this, new EventArgs());
			}
		}
		private Run run;
		private Process process;
		private ProfilerSocketServer socketServer;
	}
}