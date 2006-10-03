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
		public ListView runs;
		private MethodView callees;
		private MethodView callers;
		private Profiler profiler;
		private TextBox findText;
		private FlowLayoutPanel findPanel;
		public static TextBox application;
		public static TextBox arguments;		
		public void ShowSearch()
		{
			findPanel.Visible = !findPanel.Visible;
			findText.Focus();
		}
		public void Find(bool forward, bool step)
		{
		}
		private NProf()
		{
			this.FormClosing += delegate
			{
				profiler.Stop();
			};
			Size = new Size(800, 600);
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = "nprof - v" + Profiler.Version;
			profiler = new Profiler();
			

			runs = new ListView();
			runs.Dock = DockStyle.Fill;
			runs.Width = 100;
			runs.DoubleClick += delegate
			{
				if (runs.SelectedItems.Count != 0)
				{
					callers.UpdateFilters((Run)runs.SelectedItems[0].Tag);
					//methods.UpdateFilters((Run)runs.SelectedItems[0].Tag);
				}
			};

			callers = new MethodView("Callers");
			callers.Size = new Size(100, 200);
			callers.Dock = DockStyle.Bottom;

			callees = new MethodView("Callees");
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Fill;
			callees.DoubleClick += delegate {
				if (callees.SelectedItems.Count != 0)
				{
					FunctionInfo function = (FunctionInfo)callees.SelectedItems[0].Tag;
					callers.SelectedItems.Clear();
					foreach (ContainerListViewItem item in callees.Items)
					{
						if (((FunctionInfo)item.Tag).ID == function.ID)
						{
							callers.SelectedItems.Add(item);
							break;
						}
					}
				}};
			callees.SelectedItemsChanged+=delegate {
				if (callees.SelectedItems.Count != 0)
				{
					ContainerListViewItem item=callees.SelectedItems[0];
					if (item.Items.Count == 0)
					{
						foreach (FunctionInfo f in ((FunctionInfo)item.Tag).Callees.Values)
						{
							callees.FunctionItem(item.Items, f);
						}
					}
				}};
			findText = new TextBox();
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
							new MenuItem("&New...",delegate
							{
								runs.Items.Clear();
								NProf.arguments.Text = "";
								NProf.application.Text = "";
								callees.Items.Clear();
								callers.Items.Clear();
							},
							Shortcut.CtrlN),
							new MenuItem("-"),
							new MenuItem("Find",delegate{ShowSearch();},Shortcut.CtrlF)
						})
				});
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
							callees,
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
			});
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
			run.profiler.completed=new EventHandler(run.Complete);
			run.Start();
		}
		public void UpdateRuns(Run run)
		{
			ListViewItem item = new ListViewItem(DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern));
			item.Tag = run;
			runs.Items.Add(item);
			callees.UpdateFilters(run);
			callers.UpdateFilters(run);
		}
		private void findNext_Click(object sender, EventArgs e)
		{
			Find(true,true);
		}
		private void findPrevious_Click(object sender, EventArgs e)
		{
			Find(false,true);
		}
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.Run(form);
		}
		public static NProf form = new NProf();
		public delegate void SingleArgument<T>(T t);
		public T With<T>(T t, SingleArgument<T> del)
		{
			del(t);
			return t;
		}
	}
	public class MethodView : ContainerListView
	{
		public void UpdateFilters(Run run)
		{
			currentRun = run;
			SuspendLayout();
			BeginUpdate();
			Items.Clear();
			foreach (FunctionInfo method in run.functions.Values)
			{
				Add(method);
			}
			EndUpdate();
			ResumeLayout();
		}
		private void Find(string text,bool forward, bool step)
		{
			if (text != "")
			{
				ContainerListViewItem item;
				if (SelectedItems.Count == 0)
				{
					if (SelectedItems.Count == 0)
					{
						item = null;
					}
					else
					{
						item = Items[0];
					}
				}
				else
				{
					if (step)
					{
						if (forward)
						{
							item = SelectedItems[0];
						}
						else
						{
							item = SelectedItems[0];
						}
					}
					else
					{
						item = SelectedItems[0];
					}
				}
				ContainerListViewItem firstItem = item;
				while (item != null)
				{
					if (item.Text.ToLower().Contains(text.ToLower()))
					{
						SelectedItems.Clear();
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
								item = Items[0];
							}
							else
							{
								item = Items[Items.Count - 1];
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
		private void JumpToID(int id)
		{
			SelectedItems.Clear();
			foreach (ContainerListViewItem lvi in Items)
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
			this.Focus();
			//methods.Focus();
		}
		public Run currentRun;
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
			this.Click += delegate{UpdateView();};
			this.KeyDown += delegate
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
			ContainerListViewItem item = parent.Add(currentRun.signatures[function.ID].Signature);
			//ContainerListViewItem item = parent.Add(currentRun.signatures.GetFunctionSignature(function.ID));
			item.SubItems[1].Text = ((((double)function.Calls) / (double)currentRun.maxCalls) * 100.0).ToString("0.00;-0.00;0.0");
			item.Tag = function;
			return item;
		}
		public void FunctionItem(ContainerListViewItemCollection parent,FunctionInfo function)
		{
			ContainerListViewItem item=AddItem(parent, function);
			foreach (FunctionInfo callee in function.Callees.Values)
			{
				AddItem(item.Items, callee);
			}	
		}
		public void Add(FunctionInfo function)
		{
			FunctionItem(Items, function);
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
						run.signatures[functionId]=new FunctionSignature(
							reader.ReadUInt32(),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader),
							ReadLengthEncodedASCIIString(reader)
						);

						//run.signatures.MapSignature(functionId, new FunctionSignature(
						//    reader.ReadUInt32(),
						//    ReadLengthEncodedASCIIString(reader),
						//    ReadLengthEncodedASCIIString(reader),
						//    ReadLengthEncodedASCIIString(reader),
						//    ReadLengthEncodedASCIIString(reader)
						//));
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
		public static FunctionInfo GetFunctionInfo(Dictionary<int, FunctionInfo> functions, int id)
		{
			FunctionInfo result;
			if (!functions.TryGetValue(id, out result))
			{
				result = new FunctionInfo(id);
				functions[id] = result;
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
		public const double frequency = 10.0;
		public FunctionInfo(int ID)
		{
			this.ID = ID;
		}
		public readonly int ID;
		public int Calls;
		public int lastWalk;
		public readonly Dictionary<int, FunctionInfo> Callees = new Dictionary<int, FunctionInfo>();
	}
	public class Run
	{
		public int maxCalls;
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
						FunctionInfo function = ProfilerSocketServer.GetFunctionInfo(currentMap, stackWalk[index]);
						if (function.lastWalk != currentWalk)
						{
							function.Calls++;
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
						FunctionInfo function = ProfilerSocketServer.GetFunctionInfo(currentMap, stackWalk[index]);
						if (function.lastWalk != currentWalk)
						{
							function.Calls++;
						}
						function.lastWalk = currentWalk;
						currentMap = function.Callees;
					}
				}
			}
			int max=0;
			foreach (FunctionInfo function in functions.Values)
			{
				if (function.Calls > max)
				{
					max = function.Calls;
				}
			}
			maxCalls = max;
		}
		public void Complete(object sender,EventArgs e)
		{
			NProf.form.BeginInvoke(new EventHandler(delegate
			{
				InterpreteData();
				NProf.form.UpdateRuns(this);
			}));
		}
		public Dictionary<int, FunctionSignature> signatures = new Dictionary<int, FunctionSignature>();
		//public FunctionSignatureMap signatures = new FunctionSignatureMap();
		public Dictionary<int, FunctionInfo> functions = new Dictionary<int, FunctionInfo>();
		public Dictionary<int, FunctionInfo> callers = new Dictionary<int, FunctionInfo>();
		public Run(Profiler p)
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
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
		private DateTime start;
		private DateTime end;
		public Profiler profiler;
	}
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";
		public static string Version
		{
			get { return "0.10.0"; }
		}
		public EventHandler completed;
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
			}
		}
		private void OnProcessExited(object oSender, EventArgs ea)
		{
			completed(null, null);
			socketServer.Stop();
		}
		private Run run;
		private Process process;
		private ProfilerSocketServer socketServer;
	}
	public class FunctionSignature
	{
		public FunctionSignature(UInt32 methodAttributes, string returnType, string className, string functionName, string parameters)
		{
			this.Signature = className + "." + functionName + "(" + parameters + ")";
		}
		public string Signature;
	}
}