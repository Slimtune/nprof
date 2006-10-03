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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.Globalization;
using DotNetLib.Windows.Forms;

namespace NProf
{
	public class NProf : Form
	{
		public ContainerListView runs;
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

			if (callers.SelectedItems.Count != 0)
			{
				callers.Find(findText.Text, forward, step);
			}
			//if (callees.SelectedItems.Count != 0)
			else
			{
				callees.Find(findText.Text, forward, step);
			}
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

			runs = new ContainerListView();
			ContainerListViewColumnHeader header = new ContainerListViewColumnHeader("Runs", 90);
			runs.Columns.Add(header);
			runs.SizeChanged += delegate
			{
				header.Width = runs.Size.Width-5;
			};
			runs.Dock = DockStyle.Left;
			runs.Width = 100;
			runs.DoubleClick+=delegate
			{
				if (runs.SelectedItems.Count != 0)
				{
					ShowRun((Run)runs.SelectedItems[0].Tag);
				}
			};

			callers = new MethodView("Callers");
			callers.Size = new Size(100, 200);
			callers.Dock = DockStyle.Bottom;
			callers.GotFocus += delegate
			{
				callees.SelectedItems.Clear();
			};

			callees = new MethodView("Callees");
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Fill;
			callees.GotFocus += delegate
			{
				callers.SelectedItems.Clear();
			};
			callees.SelectedItemsChanged+=delegate {
				if (callees.SelectedItems.Count != 0)
				{
					ContainerListViewItem item=callees.SelectedItems[0];
					if (item.Items.Count == 0)
					{
						foreach (FunctionInfo f in ((FunctionInfo)item.Tag).Children.Values)
						{
							callees.AddFunctionItem(item.Items, f);
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
							new MenuItem(
								"&New...",
								delegate
								{
									runs.Items.Clear();
									NProf.arguments.Text = "";
									NProf.application.Text = "";
									callees.Items.Clear();
									callers.Items.Clear();
								},
								Shortcut.CtrlN),
							new MenuItem("-"),
							new MenuItem("E&xit",delegate {Close();})
						}),
					new MenuItem(
						"Project",
						new MenuItem[]
						{
							new MenuItem(
								"Start",
								delegate{StartRun();},
								Shortcut.F5),
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
									findPanel.BorderStyle=BorderStyle.FixedSingle;
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
						runs
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
		public void AddRun(Run run)
		{
			ContainerListViewItem item = new ContainerListViewItem(DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern));
			item.Tag = run;
			runs.Items.Add(item);
			ShowRun(run);
		}
		public void ShowRun(Run run)
		{
			callees.Update(run, run.functions);
			callers.Update(run, run.callers);
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
					}
					//while (true)
					//{
					//    List<int> stackWalk = new List<int>();
					//    while (true)
					//    {
					//        int functionId = reader.ReadInt32();
					//        if (functionId == -1)
					//        {
					//            break;
					//        }
					//        else if (functionId == -2)
					//        {
					//            return;
					//        }
					//        stackWalk.Add(functionId);
					//    }
					//    run.stackWalks.Add(stackWalk);
					//}
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
	public class StackWalk
	{
		public StackWalk(int id,List<int> frames)
		{
			this.id=id;
			this.frames=frames;
		}
		public readonly int id;
		public readonly List<int> frames;
	}
	public class FunctionInfo
	{
		public const double frequency = 10.0;
		public FunctionInfo(int ID)
		{
			this.ID = ID;
		}
		public readonly int ID;
		public int Samples;
		public int lastWalk;
		private Dictionary<int, FunctionInfo> children;
		public List<StackWalk> stackWalks=new List<StackWalk>();
		public Dictionary<int, FunctionInfo> Children
		{
			get
			{
				if (children == null)
				{
					children = new Dictionary<int, FunctionInfo>();
					foreach (StackWalk walk in stackWalks)
					{
						if (walk.frames.Count != 0)
						{
							FunctionInfo callee = ProfilerSocketServer.GetFunctionInfo(children, walk.frames[walk.frames.Count-1]);
							if (callee.lastWalk != walk.id)
							{
								callee.Samples++;
							}
							callee.stackWalks.Add(new StackWalk(walk.id, walk.frames.GetRange(0, walk.frames.Count - 1)));
						}
					}
				}
				return children;
			}
		}
	}
	public class Run
	{
		public int maxSamples;
		public List<List<int>> stackWalks = new List<List<int>>();
		private void InterpreteData()
		{
			int currentWalk = 0;
			foreach (List<int> stackWalk in stackWalks)
			{
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++)
				{
					FunctionInfo function=ProfilerSocketServer.GetFunctionInfo(functions, stackWalk[stackWalk.Count-i-1]);
					if (function.lastWalk != currentWalk)
					{
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.GetRange(0, stackWalk.Count - i - 1)));
					}
					function.lastWalk = currentWalk;
				}
			}
			foreach (List<int> reversedWalk in stackWalks)
			{
				List<int> stackWalk = new List<int>(reversedWalk);
				stackWalk.Reverse();
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++)
				{
					FunctionInfo function = ProfilerSocketServer.GetFunctionInfo(callers, stackWalk[stackWalk.Count-i-1]);
					if (function.lastWalk != currentWalk)
					{
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.GetRange(0, stackWalk.Count - i - 1)));
					}
					function.lastWalk = currentWalk;
				}
			}
			maxSamples = 0;
			foreach (FunctionInfo function in functions.Values)
			{
				if (function.Samples > maxSamples)
				{
					maxSamples = function.Samples;
				}
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
		private void ReadStackWalks()
		{
			using (BinaryReader r = new BinaryReader(File.Open("C:\\test.nprof", FileMode.Open)))
			{
				while (true)
				{
					int functionId = r.ReadInt32();
					if (functionId == -1)
					{
						break;
					}
					signatures[functionId] = new FunctionSignature(
						r.ReadUInt32(),
						ReadLengthEncodedASCIIString(r),
						ReadLengthEncodedASCIIString(r),
						ReadLengthEncodedASCIIString(r),
						ReadLengthEncodedASCIIString(r)
					);
				}
				while (true)
				{
					List<int> stackWalk = new List<int>();
					while (true)
					{
						int functionId = r.ReadInt32();
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
					stackWalks.Add(stackWalk);
				}
			}
		}
		public void Complete(object sender,EventArgs e)
		{
			ReadStackWalks();
			NProf.form.BeginInvoke(new EventHandler(delegate
			{
				InterpreteData();
				NProf.form.AddRun(this);
			}));
		}
		public Dictionary<int, FunctionSignature> signatures = new Dictionary<int, FunctionSignature>();
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
	public class MethodView : ContainerListView
	{
		public void Update(Run run,Dictionary<int,FunctionInfo> functions)
		{
			currentRun = run;
			SuspendLayout();
			BeginUpdate();
			Items.Clear();
			foreach (FunctionInfo method in SortFunctions(functions.Values))
			{
				AddItem(Items, method);
			}
			UpdateView();
			EndUpdate();
			ResumeLayout();
		}
		public void Find(string text, bool forward, bool step)
		{
			if (text != "")
			{
				ContainerListViewItem item;
				if (SelectedItems.Count == 0)
				{
					if (Items.Count == 0)
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
							item = SelectedItems[0].NextVisibleItem;
						}
						else
						{
							item = SelectedItems[0].PreviousVisibleItem;
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
							item = item.NextVisibleItem;
						}
						else
						{
							item = item.PreviousVisibleItem;
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
				if (item != null)
				{
					EnsureVisible(item);
				}
			}
		}
		//public void Find(string text, bool forward, bool step)
		//{
		//    if (text != "")
		//    {
		//        ContainerListViewItem item;
		//        if (SelectedItems.Count == 0)
		//        {
		//            if (SelectedItems.Count == 0)
		//            {
		//                item = null;
		//            }
		//            else
		//            {
		//                item = Items[0];
		//            }
		//        }
		//        else
		//        {
		//            if (step)
		//            {
		//                if (forward)
		//                {
		//                    item = SelectedItems[0];
		//                }
		//                else
		//                {
		//                    item = SelectedItems[0];
		//                }
		//            }
		//            else
		//            {
		//                item = SelectedItems[0];
		//            }
		//        }
		//        ContainerListViewItem firstItem = item;
		//        while (item != null)
		//        {
		//            if (item.Text.ToLower().Contains(text.ToLower()))
		//            {
		//                SelectedItems.Clear();
		//                item.Focused = true;
		//                item.Selected = true;
		//                this.Invalidate();
		//                break;
		//            }
		//            else
		//            {
		//                if (forward)
		//                {
		//                    item = item.NextItem;
		//                }
		//                else
		//                {
		//                    item = item.PreviousItem;
		//                }
		//                if (item == null)
		//                {
		//                    if (forward)
		//                    {
		//                        item = Items[0];
		//                    }
		//                    else
		//                    {
		//                        item = Items[Items.Count - 1];
		//                    }
		//                }
		//            }
		//            if (item == firstItem)
		//            {
		//                break;
		//            }
		//        }
		//    }
		//}
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
		}
		public Run currentRun;
		public MethodView(string name)
		{
			Columns.Add(name);
			Columns.Add("Time");
			Columns[0].Width = 350;
			this.ShowPlusMinus = true;
			ShowRootTreeLines = true;
			ShowTreeLines = true;
			ColumnSortColor = Color.White;
			Font = new Font("Tahoma", 8.0f);
			this.Click += delegate
			{
				UpdateView();
			};
			this.KeyDown += delegate
			{
				UpdateView();
			};
			this.SelectedItemsChanged += delegate
			{
				UpdateView();
			};
		}
		void MakeSureComputed(ContainerListViewItem item)
		{
			MakeSureComputed(item, false);
		}
		public List<FunctionInfo> SortFunctions(IEnumerable<FunctionInfo> f)
		{
			List<FunctionInfo> functions = new List<FunctionInfo>(f);
			functions.Sort(delegate(FunctionInfo a,FunctionInfo b)
			{
				return b.Samples.CompareTo(a.Samples);
			});
			return functions;
		}
		void MakeSureComputed(ContainerListViewItem item,bool parentExpanded)
		{
			if (item.Items.Count == 0)
			{
				foreach (FunctionInfo function in SortFunctions(((FunctionInfo)item.Tag).Children.Values))
				{
					AddFunctionItem(item.Items, function);
				}
			}
			if (item.Expanded || parentExpanded)
			{
				foreach (ContainerListViewItem subItem in item.Items)
				{
					MakeSureComputed(subItem,item.Expanded);
				}
			}
		}
		void UpdateView()
		{
			SuspendLayout();
			BeginUpdate();
			foreach (ContainerListViewItem item in Items)
			{
				MakeSureComputed(item,true);
			}
			EndUpdate();
			ResumeLayout();
		}
		private ContainerListViewItem AddItem(ContainerListViewItemCollection parent, FunctionInfo function)
		{
			ContainerListViewItem item = parent.Add(currentRun.signatures[function.ID].Signature);
			item.SubItems[1].Text = ((((double)function.Samples) / (double)currentRun.maxSamples) * 100.0).ToString("0.00;-0.00;0.0");
			item.Tag = function;
			return item;
		}
		public void AddFunctionItem(ContainerListViewItemCollection parent, FunctionInfo function)
		{
			ContainerListViewItem item = AddItem(parent, function);
			foreach (FunctionInfo callee in function.Children.Values)
			{
				AddItem(item.Items, callee);
			}
		}
		public void Add(FunctionInfo function)
		{
			AddFunctionItem(Items, function);
		}
	}
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";
		public static string Version
		{
			get { return "0.10"; }
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