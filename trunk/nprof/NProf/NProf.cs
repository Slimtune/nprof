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
using NProf.GUI;
using NProf;
using Genghis.Windows.Forms;
using Reflector.UserInterface;
using Crownwood.Magic.Menus;
using DotNetLib.Windows.Forms;
using System.Globalization;

namespace NProf
{
	public class ProfilerForm : Form
	{
		public ProjectInfo Project
		{
			get
			{
				return project;
			}
			set
			{
				project = value;
				runs.Nodes.Clear();
				foreach (Run run in Project.Runs)
				{
					UpdateRun(run);
				}
			}
		}
		private Profiler profiler=new Profiler();
		private ProjectInfo project;
		private TreeView runs = new TreeView();
		private MethodView methods=new MethodView();
		private MethodView callees=new MethodView();
		private MethodView callers=new MethodView();
		private TextBox findText = new TextBox();
		public static Panel methodPanel=new Panel();
		Run currentRun;
		private ProfilerForm()
		{
			runs.DoubleClick += delegate
			{
				UpdateFilters((Run)runs.SelectedNode.Tag);// this should also be done when loading a project!!
			};
			methods.SelectedItemsChanged += delegate
			{
				callees.Items.Clear();
				callers.Items.Clear();

				if (methods.SelectedItems.Count == 0)
					return;

				// somebody clicked! empty the forward stack and push this click on the "Back" stack.
				if (!isNavigating)
				{
					forward.Clear();
					if (_navCurrent != 0)
					{
						back.Push(_navCurrent);
					}
					else
					{
					}

					for (int idx = 0; idx < methods.SelectedItems.Count; ++idx)
					{
						if (methods.SelectedItems[idx].Tag != null)
						{
							_navCurrent = (methods.SelectedItems[idx].Tag as FunctionInfo).ID;
							break;
						}
					}
				}
				//Run run = (Run)runs.SelectedNode.Tag;
				UpdateCalleeList();
				UpdateCallerList(currentRun);
			};
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.app-icon.ico"));
			Text = "NProf";

			string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string dll = Path.Combine(directory, "msvcr70.dll");
			if (LoadLibrary(dll) == 0)
				throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to load msvcr70.dll");

			CommandBarManager commandBarManager = new CommandBarManager();
			CommandBar commandBar = new CommandBar(CommandBarStyle.ToolBar);
			commandBar.Items.Add(new CommandBarButton(Images.New, "New", New));
			commandBar.Items.Add(new CommandBarButton(Images.Save, "Save", Save));
			commandBar.Items.AddSeparator();
			commandBar.Items.Add(new CommandBarButton(Images.Back, "Back", Back));
			commandBar.Items.Add(new CommandBarButton(Images.Forward, "Forward", Forward));
			commandBar.Items.AddSeparator();
			commandBar.Items.Add(new CommandBarButton(Images.Run, "Run", delegate { StartRun(null, null); }));
			commandBarManager.CommandBars.Add(commandBar);

			MenuControl mainMenu = new MenuControl();
			mainMenu.Dock = DockStyle.Top;
			mainMenu.MenuCommands.AddRange(new MenuCommand[]
			{
			new Menu("File",
				new Menu("&New...","Create a new profile project",New),
				new Menu("&Open...","Open a profile project",Open),
				new Menu("-","-",null),
				new Menu("&Save","Save the active profiler project",Shortcut.CtrlS,Save),
				new Menu("Save &As...","Save the active profiler project as a specified file name",delegate {SaveProject( Project, true );}),
				new Menu("-","-",null),
				new Menu("E&xit","Exit the application",Shortcut.AltF4,
				delegate {Close();})),
			new Menu("View",
				new Menu("Back","Navigate Back",Back),
				new Menu("Forward","Navigate Forward",Forward)),
			new Menu("&Project",
				new Menu("Start","Run the current project",Shortcut.F5,StartRun),
				new Menu("Properties...","Modify the options for this project",Shortcut.F2,
					Properties)),
			new Menu("&Help",
				new Menu("About nprof...","About nprof",About))
			});

			commandBarManager.Dock = DockStyle.Top;

			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			Controls.Add(panel);

			runs.Dock = DockStyle.Left;

			Size=new Size(800,600);
			methods.Size = new Size(100, 100);

			callers.Size = new Size(100, 100);
			callers.Dock = DockStyle.Bottom;
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Bottom;
			methods.Dock = DockStyle.Fill;
			methodPanel.Size = new Size(100, 100);
			methodPanel.Dock = DockStyle.Fill;
			Splitter runSplitter = new Splitter();
			runSplitter.Dock = DockStyle.Left;

			Label findLabel = new Label();
			findLabel.Text = "Find:";
			findLabel.TextAlign = ContentAlignment.MiddleCenter;
			findLabel.AutoSize = true;
			FlowLayoutPanel findPanel = new FlowLayoutPanel();
			findPanel.WrapContents = false;
			findPanel.AutoSize = true;
			Button findNext = new Button();
			findNext.AutoSize = true;
			Button findPrevious = new Button();
			findPrevious.AutoSize = true;
			findNext.Text = "Find next";
			findNext.Click += new EventHandler(findNext_Click);
			findPrevious.Click += new EventHandler(findPrevious_Click);
			findPrevious.Text = "Find previous";


			findPanel.Controls.AddRange(new Control[] {
				findLabel,
				findText,
				findNext,
				findPrevious
			});
			findPanel.Dock = DockStyle.Top;


			methodPanel.Controls.AddRange(new Control[] {
				methods,
				Splitter(DockStyle.Bottom),
				callees,
				Splitter(DockStyle.Bottom),
				callers,
				findPanel

			});
			panel.Controls.AddRange(new Control[] {
				methodPanel,
				runSplitter,
				runs
			});
			Controls.Add(commandBarManager);
			Controls.Add(mainMenu);
		}
		public static Splitter Splitter(DockStyle dock)
		{
			Splitter splitter = new Splitter();
			splitter.Dock = dock;
			return splitter;
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int LoadLibrary(string strLibFileName);

		private delegate void HandleProfileComplete(Run run);

		//private void OnUIThreadProfileComplete(Run run)
		//{
		//    if (run.State == RunState.Finished
		//        && run.Success)
		//    {
		//        UpdateRun(run);
		//    }
		//}

		//private void OnRunStateChanged(Run run, RunState rsOld, RunState rsNew)
		//{
		//    if (rsNew == RunState.Running || rsNew == RunState.Finished)
		//    {
		//        this.BeginInvoke(new HandleProfileComplete(OnUIThreadProfileComplete), new object[] { run });
		//    }
		//}

		private void ProfilerForm_Load(object sender, System.EventArgs e)
		{
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.app-icon.ico"));
			Text = "nprof Profiling Application - v" + Profiler.Version;
		}

		private void ProfilerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			profiler.Stop();
		}

		private void New(object sender, System.EventArgs e)
		{
			PropertiesForm form = new PropertiesForm(PropertiesForm.ProfilerProjectMode.CreateProject);
			form.Mode = PropertiesForm.ProfilerProjectMode.CreateProject;
			if (form.ShowDialog(this) == DialogResult.OK)
			{
				project = form.Project;
			}
		}

		private void Open(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "nprof";
			dialog.Filter = "NProf projects (*.nprof)|*.nprof|All files (*.*)|*.*";
			dialog.Multiselect = true;
			dialog.Title = "Open a saved NProf project file";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				ProjectInfo project = SerializationHandler.OpenProjectInfo(dialog.FileName);
				this.Project = project;
			}
		}

		private void Save(object sender, System.EventArgs e)
		{
			SaveProject(Project, false);
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
			run.profiler.Completed += delegate
			{
				this.BeginInvoke(new EventHandler(delegate
				{
					 UpdateRun(run);
				}));
			};

			//run.StateChanged += new RunStateEventHandler(OnRunStateChanged);
			run.Start();
		}

		private void Properties(object sender, System.EventArgs e)
		{
			PropertiesForm properties = new PropertiesForm(PropertiesForm.ProfilerProjectMode.ModifyProject);
			properties.Project = Project;
			properties.Mode = PropertiesForm.ProfilerProjectMode.ModifyProject;

			properties.ShowDialog(this);
		}

		private void Back(object sender, System.EventArgs e)
		{
			if (back.Count == 0)
				return;

			forward.Push(_navCurrent);
			_navCurrent = back.Pop();

			isNavigating = true;
			JumpToID(_navCurrent);
			isNavigating = false;
		}

		private void Forward(object sender, System.EventArgs e)
		{
			if (forward.Count == 0)
				return;

			back.Push(_navCurrent);
			_navCurrent = forward.Pop();
			//_navCurrent = (int[])forward.Pop();

			isNavigating = true;
			JumpToID(_navCurrent);
			isNavigating = false;
		}

		private void About(object sender, System.EventArgs e)
		{
			new AboutForm().ShowDialog(this);
		}

		private bool SaveProject(ProjectInfo project, bool forceSaveDialog)
		{
			if (project == null)
				return true;

			string filename = SerializationHandler.GetFilename(project);

			if (forceSaveDialog || filename == string.Empty)
			{
				SaveFileDialog saveDlg = new SaveFileDialog();

				saveDlg.DefaultExt = "nprof";
				saveDlg.FileName = SerializationHandler.GetFilename(project); ;
				saveDlg.Filter = "NProf projects (*.nprof)|*.nprof|All files (*.*)|*.*";
				// saveDlg.InitialDirectory = TODO: store the most recently used direcotry somewhere and go there
				saveDlg.Title = "Save a NProf project file";

				if (saveDlg.ShowDialog(this) != DialogResult.OK)
					return false;

				project.Name = Path.GetFileNameWithoutExtension(saveDlg.FileName);
				filename = saveDlg.FileName;
			}
			SerializationHandler.SaveProjectInfo(project, filename);

			return true;
		}
		private class Images
		{
			private static Image[] images = null;
			static Images()
			{
				Bitmap bitmap = (Bitmap)Bitmap.FromStream(typeof(Images).Assembly.GetManifestResourceStream("NProf.GUI.Resources.toolbar16.png"));
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
			public static Image Open { get { return images[1]; } }
			public static Image Save { get { return images[2]; } }
			public static Image Delete { get { return images[6]; } }
			public static Image Properties { get { return images[7]; } }
			public static Image Undo { get { return images[8]; } }
			public static Image Redo { get { return images[9]; } }
			public static Image Preview { get { return images[10]; } }
			public static Image Print { get { return images[11]; } }
			public static Image Search { get { return images[12]; } }
			public static Image ReSearch { get { return images[13]; } }
			public static Image Help { get { return images[14]; } }
			public static Image ZoomIn { get { return images[15]; } }
			public static Image ZoomOut { get { return images[16]; } }
			public static Image Back { get { return images[17]; } }
			public static Image Forward { get { return images[18]; } }
			public static Image Favorites { get { return images[19]; } }
			public static Image AddToFavorites { get { return images[20]; } }
			public static Image Stop { get { return images[21]; } }
			public static Image Refresh { get { return images[22]; } }
			public static Image Home { get { return images[23]; } }
			public static Image Edit { get { return images[24]; } }
			public static Image Tools { get { return images[25]; } }
			public static Image Tiles { get { return images[26]; } }
			public static Image Icons { get { return images[27]; } }
			public static Image List { get { return images[28]; } }
			public static Image Details { get { return images[29]; } }
			public static Image Pane { get { return images[30]; } }
			public static Image Culture { get { return images[31]; } }
			public static Image Languages { get { return images[32]; } }
			public static Image History { get { return images[33]; } }
			public static Image Mail { get { return images[34]; } }
			public static Image Parent { get { return images[35]; } }
			public static Image FolderProperties { get { return images[36]; } }
			public static Image Run { get { return images[37]; } }
		}
		private Stack<int> back = new Stack<int>();
		private Stack<int> forward = new Stack<int>();

		private int _navCurrent = 0;
		//private int[] _navCurrent = null;
		// remove
		private bool isNavigating = false;

		public void UpdateRun(Run run)
		{
			TreeNode node = new TreeNode(run.StartTime.ToString());
			node.Tag = run;
			runs.Nodes.Add(node);
			UpdateFilters(run);
		}
		private void UpdateFilters(Run run)
		{
			methods.Items.Clear();
			callees.Items.Clear();
			callers.Items.Clear();

			currentRun = run;
			methods.BeginUpdate();
			foreach (ThreadInfo thread in run.Process.Threads)
			{
				foreach (FunctionInfo method in thread.FunctionInfoCollection.Values)
				{
					methods.Add(method);
				}
			}
			methods.EndUpdate();
		}
		private void UpdateCallerList(Run run)
		{
			callers.BeginUpdate();

			bool multipleSelected = (methods.SelectedItems.Count > 1);
			callers.ShowPlusMinus = multipleSelected;
			callers.ShowRootTreeLines = multipleSelected;
			callers.ShowTreeLines = multipleSelected;

			FunctionInfo mfi = (FunctionInfo)methods.SelectedItems[0].Tag;
			foreach (ThreadInfo thread in run.Process.Threads)
			{
				foreach (FunctionInfo fi in thread.FunctionInfoCollection.Values)
				{
					foreach (CalleeFunctionInfo cfi in fi.CalleeInfo)
					{
						if (cfi.ID == mfi.ID)
						{
							callers.Add(fi);
						}
					}
				}
			}
			callers.Sort();
			callers.EndUpdate();
		}
		public const string timeFormat = ".00;-.00;.0";
		private void UpdateCalleeList()
		{
			callees.Items.Clear();
			callees.BeginUpdate();

			bool multipleSelected = (methods.SelectedItems.Count > 1);
			callees.ShowPlusMinus = multipleSelected;
			callees.ShowRootTreeLines = multipleSelected;
			callees.ShowTreeLines = multipleSelected;

			foreach (ContainerListViewItem item in methods.SelectedItems)
			{
				FunctionInfo fi = (FunctionInfo)item.Tag;

				foreach (CalleeFunctionInfo cfi in fi.CalleeInfo)
				{
					callees.Add(cfi);
				}

				ContainerListViewItem inMethod = callees.Items.Add("(in method)");
				inMethod.SubItems[1].Text = fi.Calls.ToString();
				inMethod.SubItems[2].Text = fi.TimeInMethod.ToString(timeFormat);
				inMethod.Tag = fi;
			}
			callees.Sort();
			callees.EndUpdate();
		}
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
			methods.EnsureVisible();
			methods.Focus();
		}
		private int GetSelectedID()
		{
			if (callees.SelectedItems.Count == 0)
				return -1;

			return ((FunctionInfo)methods.SelectedItems[0].Tag).ID;
		}
		private void Find(bool forward)
		{
			if (findText.Text != "")
			{
				ContainerListViewItem item;
				if (methods.SelectedItems.Count == 0)
				{
					if (methods.Items.Count == 0)
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
					if (forward)
					{
						item = methods.SelectedItems[0].NextItem;
					}
					else
					{
						item = methods.SelectedItems[0].PreviousItem;
					}
				}
				while (item != null)
				{
					if (item.Text.ToLower().Contains(findText.Text.ToLower()))
					{
						methods.SelectedItems.Clear();
						item.Focused = true;
						item.Selected = true;
						methods.EnsureVisible();
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
					}
					if (item == null)
					{
						break;
					}
				}
			}
		}
		private void findNext_Click(object sender, EventArgs e)
		{
			Find(true);
		}
		private void findPrevious_Click(object sender, EventArgs e)
		{
			Find(false);
		}
		public static ProfilerForm form = new ProfilerForm();
	}
	public class MethodItem : ContainerListViewItem
	{
		private FunctionInfo function;
		public MethodItem(FunctionInfo function)
			: base(function.Signature.Signature)
		{
			this.function = function;
			this.SubItems[1].Text = function.Calls.ToString();
			this.SubItems[2].Text = function.TimeInMethod.ToString(ProfilerForm.timeFormat);
			this.Tag = function;
		}
	}
	public class MethodView : ContainerListView
	{
		public MethodView()
		{
			Columns.Add("Method name");
			Columns.Add("Number of calls");
			Columns.Add("Time spent");
			HeaderStyle = ColumnHeaderStyle.Clickable;
			Columns[0].Width = 350;
			this.ColumnSortColor = Color.White;
			Columns[0].SortDataType = SortDataType.String;
			Columns[1].SortDataType = SortDataType.Integer;
			Columns[2].SortDataType = SortDataType.String;
			this.ClientSizeChanged += new EventHandler(MethodView_ClientSizeChanged);
			Font = new Font("Tahoma", 8.0f);

			Sort(2, SortOrder.Descending, true);
		}

		void MethodView_ClientSizeChanged(object sender, EventArgs e)
		{
		}
		public void Add(FunctionInfo function)
		{
			ContainerListViewItem item = Items.Add(function.Signature.Signature);
			item.SubItems[1].Text = function.Calls.ToString();
			item.SubItems[2].Text = function.TimeInMethod.ToString(ProfilerForm.timeFormat);
			item.Tag = function;
		}
		public void Add(CalleeFunctionInfo function)
		{
			ContainerListViewItem item = Items.Add(function.Signature);
			item.SubItems[1].Text = function.Calls.ToString();
			item.SubItems[2].Text = function.TimeInMethod.ToString(ProfilerForm.timeFormat);
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

								//if (run.State == RunState.Running)
								//{
								//    //ns.WriteByte( 0 );
								//}

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
								//run.State = RunState.Running;
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

									FunctionInfo function = new FunctionInfo(currentProcess.Threads[threadId], functionId, fs, callCount, totalTime, totalRecursiveTime, totalSuspendTime, callees.ToArray());
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
		public CalleeFunctionInfo(FunctionSignatureMap signatures, int id, int calls, long totalTime, long totalRecursiveTime)
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
			get { return signatures.GetFunctionSignature(ID); }
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
		internal FunctionInfo FunctionInfo
		{
			set { function = value; }
		}
		private int id;
		private int calls;
		private FunctionInfo function;
		private FunctionSignatureMap signatures;
		private long totalTime;
		private long totalRecursiveTime;
	}
	// rename?
	[Serializable]
	public class FunctionInfo
	{
		public FunctionInfo()
		{
		}
		public FunctionInfo(ThreadInfo ti, int nID, FunctionSignature signature, int calls, long totalTime, long totalRecursiveTime, long totalSuspendedTime, CalleeFunctionInfo[] callees)
		{
			this.thread = ti;
			this.id = nID;
			this.signature = signature;
			this.calls = calls;
			this.totalTime = totalTime;
			this.totalRecursiveTime = totalRecursiveTime;
			this.totalSuspendedTime = totalSuspendedTime;
			this.callees = callees;

			foreach (CalleeFunctionInfo callee in callees)
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
				foreach (CalleeFunctionInfo callee in callees)
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
				foreach (CalleeFunctionInfo callee in callees)
					totalChildrenRecursiveTime += callee.TotalRecursiveTime;

				return totalChildrenRecursiveTime;
			}
		}
		[XmlIgnore]
		public double TimeInMethod
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
	public class FunctionInfoCollection : Dictionary<int, FunctionInfo>
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

		public string Signature
		{
			get
			{
				return className + "." + functionName + "(" + parameters + ")";
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
	[Serializable]
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
			lock (signatures.SyncRoot)
			{
				FunctionSignature signature = (FunctionSignature)signatures[functionID];
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

		public ProcessInfo(int id)
			: this()
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
			return String.Format("{0} ({1})", name, processId);
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
			processes = new Dictionary<int, ProcessInfo>();
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
		private Dictionary<int, ProcessInfo> processes;
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

		public ThreadInfo(int threadId)
			: this()
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
		public ThreadInfo this[int threadId]
		{
			get
			{
				lock (threads)
				{
					ThreadInfo thread;
					if (!threads.TryGetValue(threadId, out thread))
					{
						thread = new ThreadInfo(threadId);
						threads[threadId] = thread;
					}
					return thread;
				}
			}
		}
		private Dictionary<int, ThreadInfo> threads = new Dictionary<int, ThreadInfo>();
	}
	[Serializable]
	public class ProjectInfo
	{
		public ProjectInfo() : this(ProjectType.File) { } // JC: added default constructor for serialization

		public ProjectInfo(ProjectType projectType)
		{
			this.name = null;
			this.runs = new RunCollection(this);
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

		public ProjectType ProjectType
		{
			get { return projectType; }
		}

		private void Fire_ProjectInfoChanged()
		{
			if (ProjectInfoChanged != null)
				ProjectInfoChanged(this);
		}
		[field: NonSerialized]
		public event ProjectEventHandler ProjectInfoChanged;

		public delegate void ProjectEventHandler(ProjectInfo project);

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

		public Run(Profiler p, ProjectInfo pi)
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
			//this.runState = RunState.Initializing;
			this.project = pi;
			this.messages = new RunMessageCollection();
			this.isSuccess = false;
		}
		public bool Start()
		{
			start = DateTime.Now;

			return profiler.Start(project, this);
		}

		//public bool CanStop
		//{
		//    get
		//    {
		//        return State == RunState.Initializing ||
		//              (project.ProjectType == ProjectType.AspNet && State != RunState.Finished);
		//    }
		//}

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

		//public RunState State
		//{
		//    get { return runState; }

		//    set
		//    {
		//        RunState rsOld = runState;
		//        runState = value;
		//        if (StateChanged != null)
		//            StateChanged(this, rsOld, runState);
		//    }
		//}

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


		//public Profiler.ProcessCompletedHandler ProcessCompletedHandler
		//{
		//    get { return new Profiler.ProcessCompletedHandler(OnProfileComplete); }
		//}

		//private void OnProfileComplete(Run run)
		//{
		//    State = RunState.Finished;
		//    end = DateTime.Now;
		//}

		//[field: NonSerialized]
		//public event RunStateEventHandler StateChanged;



		private bool isSuccess;
		private DateTime start;
		private DateTime end;
		//private RunState runState;
		private ProjectInfo project;
		public Profiler profiler;
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
	//public delegate void RunStateEventHandler(Run run, RunState rsOld, RunState rsNew);

	//[Serializable]
	//public enum RunState
	//{
	//    Initializing,
	//    Running,
	//    Finished,
	//}
	// remove
	[Serializable]
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
		[field: NonSerialized]
		public event RunEventHandler RunAdded;
		[field: NonSerialized]
		public event RunEventHandler RunRemoved;

		public delegate void RunEventHandler(ProjectInfo pi, RunCollection runs, Run run, int nIndex);

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
				lock (messages)
				{
					return (string[])messages.ToArray(typeof(string));
				}
			}
		}

		public string[] StartListening(MessageHandler handler)
		{
			lock (messages)
			{
				Message += handler;
				return AllMessages;
			}
		}

		public void StopListening(MessageHandler handler)
		{
			lock (messages)
			{
				Message -= handler;
			}
		}

		public void Add(object oMessage)
		{
			// For XML serialization
			AddMessage((string)oMessage);
		}

		public void AddMessage(string message)
		{
			lock (messages)
			{
				messages.Add(message);
				if (Message != null)
					Message(message);
			}
		}

		private ArrayList messages = new ArrayList();

		public delegate void MessageHandler(string message);
		[field: NonSerialized]
		private event MessageHandler Message;
	}
	public class SerializationHandler
	{
		public static string DataStoreDirectory;

		private static Hashtable projectToFileName = Hashtable.Synchronized(new Hashtable());

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
				MakeRecentlyUsed(fileName);

				return project;
			}
		}
		private static RegistryKey RecentProjectsKey()
		{
			return Registry.CurrentUser.CreateSubKey("Software\\NProf\\RecentProjects");
		}
		public static void MakeRecentlyUsed(string fileName)
		{
			using (RegistryKey key = RecentProjectsKey())
			{
				key.SetValue(fileName, DateTime.Now.ToString(CultureInfo.InvariantCulture));
				//key.Flush();
			}
			//{
			//    Queue<string> files=GetRecentlyUsed();
			//    files.Enqueue(fileName);
			//    int index=1;
			//    foreach(string file in files)
			//    {
			//        key.SetValue(index.ToString(),file);
			//        index++;
			//    }
			//}
		}
		public static List<string> GetRecentlyUsed()
		{
			using(RegistryKey key=RecentProjectsKey())
			{
				Dictionary<string,DateTime> data=new Dictionary<string,DateTime>();
				foreach(string name in key.GetValueNames())
				{
					data[name]=Convert.ToDateTime(key.GetValue(name),CultureInfo.InvariantCulture);
				}
				List<string> result=new List<string>(data.Keys);
				result.Sort(delegate(string a, string b)
				{
					return data[a].CompareTo(data[b]);
				});
				return result;
			}
		}
		public static void SaveProjectInfo(ProjectInfo info)
		{
			if (projectToFileName.Contains(info)) // one that has already been opened
			{
				string fileName = (string)projectToFileName[info];
				MakeRecentlyUsed(fileName);
				SaveProjectInfo(info, fileName);
			}
		}
		public static void SaveProjectInfo(ProjectInfo info, string fileName)
		{
			using (Stream stream = File.Open(fileName, FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, info);
				// remember for later
				projectToFileName[info] = fileName;

				// make it recently used
				MakeRecentlyUsed(fileName);
				//MakeRecentlyUsed(info, fileName);
			}
		}
		public static string GetFilename(ProjectInfo info)
		{
			return ((string)projectToFileName[info]) + string.Empty;
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
		[field: NonSerialized]
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
			//this.completed = pch;
			this.run = run;
			//this.run.State = RunState.Initializing;

			socketServer = new ProfilerSocketServer(run);
			socketServer.Start();
			socketServer.Exited += new EventHandler(OnProcessExited);
			socketServer.Error += new ProfilerSocketServer.ErrorHandler(OnError);
			socketServer.Message += new ProfilerSocketServer.MessageHandler(OnMessage);

			switch (pi.ProjectType)
			{
				case ProjectType.File:
					{
						process = new Process();
						process.StartInfo = new ProcessStartInfo(pi.ApplicationName, pi.Arguments);
						process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
						process.StartInfo.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
						process.StartInfo.EnvironmentVariables["NPROF_PROFILING_SOCKET"] = socketServer.Port.ToString();
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.Arguments = pi.Arguments;
						process.StartInfo.WorkingDirectory = pi.WorkingDirectory;
						process.EnableRaisingEvents = true;
						//_p.Exited += new EventHandler( OnProcessExited );

						return process.Start();
					}

				case ProjectType.AspNet:
					{
						using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\W3SVC", true))
						{
							if (rk != null)
								SetRegistryKeys(rk, true);
						}

						using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\IISADMIN", true))
						{
							if (rk != null)
								SetRegistryKeys(rk, true);
						}

						Process p = Process.Start("iisreset.exe", "");
						p.WaitForExit();
						this.run.Messages.AddMessage("Navigate to your project and ASP.NET will connect to the profiler");
						this.run.Messages.AddMessage("NOTE: ASP.NET must be set to run under the SYSTEM account in machine.config");
						this.run.Messages.AddMessage(@"If ASP.NET cannot be profiled, ensure that the userName=""SYSTEM"" in the <processModel> section of machine.config.");

						return true;
					}

				case ProjectType.VSNet:
					{
						SetEnvironmentVariable("COR_ENABLE_PROFILING", "0x1");
						SetEnvironmentVariable("COR_PROFILER", PROFILER_GUID);
						SetEnvironmentVariable("NPROF_PROFILING_SOCKET", socketServer.Port.ToString());

						return true;
					}

				default:
					throw new InvalidOperationException("Unknown project type: " + pi.ProjectType);
			}
		}
		//public bool Start(ProjectInfo pi, Run run, ProcessCompletedHandler pch)
		//{
		//    this.start = DateTime.Now;
		//    this.project = pi;
		//    this.completed = pch;
		//    this.run = run;
		//    this.run.State = RunState.Initializing;

		//    socketServer = new ProfilerSocketServer(run);
		//    socketServer.Start();
		//    socketServer.Exited += new EventHandler(OnProcessExited);
		//    socketServer.Error += new ProfilerSocketServer.ErrorHandler(OnError);
		//    socketServer.Message += new ProfilerSocketServer.MessageHandler(OnMessage);

		//    switch (pi.ProjectType)
		//    {
		//        case ProjectType.File:
		//            {
		//                process = new Process();
		//                process.StartInfo = new ProcessStartInfo(pi.ApplicationName, pi.Arguments);
		//                process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
		//                process.StartInfo.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
		//                process.StartInfo.EnvironmentVariables["NPROF_PROFILING_SOCKET"] = socketServer.Port.ToString();
		//                process.StartInfo.UseShellExecute = false;
		//                process.StartInfo.Arguments = pi.Arguments;
		//                process.StartInfo.WorkingDirectory = pi.WorkingDirectory;
		//                process.EnableRaisingEvents = true;
		//                //_p.Exited += new EventHandler( OnProcessExited );

		//                return process.Start();
		//            }

		//        case ProjectType.AspNet:
		//            {
		//                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\W3SVC", true))
		//                {
		//                    if (rk != null)
		//                        SetRegistryKeys(rk, true);
		//                }

		//                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\IISADMIN", true))
		//                {
		//                    if (rk != null)
		//                        SetRegistryKeys(rk, true);
		//                }

		//                Process p = Process.Start("iisreset.exe", "");
		//                p.WaitForExit();
		//                this.run.Messages.AddMessage("Navigate to your project and ASP.NET will connect to the profiler");
		//                this.run.Messages.AddMessage("NOTE: ASP.NET must be set to run under the SYSTEM account in machine.config");
		//                this.run.Messages.AddMessage(@"If ASP.NET cannot be profiled, ensure that the userName=""SYSTEM"" in the <processModel> section of machine.config.");

		//                return true;
		//            }

		//        case ProjectType.VSNet:
		//            {
		//                SetEnvironmentVariable("COR_ENABLE_PROFILING", "0x1");
		//                SetEnvironmentVariable("COR_PROFILER", PROFILER_GUID);
		//                SetEnvironmentVariable("NPROF_PROFILING_SOCKET", socketServer.Port.ToString());

		//                return true;
		//            }

		//        default:
		//            throw new InvalidOperationException("Unknown project type: " + pi.ProjectType);
		//    }
		//}

		public void Disable()
		{
			SetEnvironmentVariable("COR_ENABLE_PROFILING", "0x0");
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

			// Stop the profiler socket server if profilee hasn't connected
			//if (r.State == RunState.Initializing)
			//{
				r.Messages.AddMessage("Shutting down profiler...");
				socketServer.Stop();
				//r.State = RunState.Finished;
				r.Success = false;
			//}

			if (project.ProjectType == ProjectType.AspNet)
			{
				using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\W3SVC", true))
				{
					if (rk != null)
						SetRegistryKeys(rk, false);
				}

				using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\IISADMIN", true))
				{
					if (rk != null)
						SetRegistryKeys(rk, false);
				}

				r.Messages.AddMessage("Terminating ASP.NET...");
				Process.Start("iisreset.exe", "/stop").WaitForExit();
			}
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
				//if (r.State == RunState.Initializing)
				//{
				//    r.Messages.AddMessage("No connection made with profiled application.");
				//    r.Messages.AddMessage("Application might not support .NET.");
				//}
				//else
				//{
				//    r.Messages.AddMessage("Application stopped before profiler information could be retrieved.");
				//}

				r.Success = false;
				//r.State = RunState.Finished;
				r.Messages.AddMessage("Profiler run did not complete successfully.");
			}
			else
			{
				r.Success = true;
			}

			end = DateTime.Now;
			r.Messages.AddMessage("Stopping profiler listener...");
			socketServer.Stop();
			//			if ( ProcessCompleted != null )
			//				ProcessCompleted( _pss.ThreadInfoCollection );

			r.EndTime = end;

			if (Completed != null)
			{
				Completed(this, new EventArgs());
			}
			//completed(r);
		}

		private void OnError(Exception e)
		{
			MessageBox.Show("An internal exception occurred:\n\n" + e.ToString(), "Error");
			//if ( Error != null )
			//    Error( e );
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

		private void SetRegistryKeys(RegistryKey key, bool isSet)
		{
			if (key == null)
				return;

			if (!isSet)
			{
				// Get rid of the environment
				key.DeleteValue("Environment", false);
				return;
			}

			object oKeys = key.GetValue("Environment");

			// If it's not something we expected, fix it
			if (oKeys == null || !(oKeys is string[]))
				oKeys = new string[0];

			// Save the environment the first time through
			if (key.GetValue("nprof Saved Environment") == null && ((string[])oKeys).Length > 0)
				key.SetValue("nprof Saved Environment", oKeys);

			Hashtable items = new Hashtable(Environment.GetEnvironmentVariables());

			// Set the environment to be the default system environment
			using (RegistryKey rkEnv = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"))
			{
				if (rkEnv == null)
					throw new InvalidOperationException("Unable to locate machine environment key");

				foreach (string strValueName in rkEnv.GetValueNames())
					items[strValueName] = rkEnv.GetValue(strValueName);

			}

			items.Remove("COR_ENABLE_PROFILING");
			items.Remove("COR_PROFILER");
			items.Remove("NPROF_PROFILING_SOCKET");

			items.Add("COR_ENABLE_PROFILING", "0x1");
			items.Add("COR_PROFILER", PROFILER_GUID);
			items.Add("NPROF_PROFILING_SOCKET", socketServer.Port.ToString());

			ArrayList itemList = new ArrayList();
			foreach (DictionaryEntry de in items)
				itemList.Add(String.Format("{0}={1}", de.Key, de.Value));

			key.SetValue("Environment", (string[])itemList.ToArray(typeof(string)));
		}

		public delegate void ProcessCompletedHandler(Run run);
		[field: NonSerialized]
		public event ProcessCompletedHandler ProcessCompleted;
		public delegate void ErrorHandler(Exception e);
		public delegate void MessageHandler(string strMessage);
		[field: NonSerialized]
		public event MessageHandler Message;

		//[NonSerialized]
		//private ProcessCompletedHandler completed;
		private DateTime start;
		private DateTime end;
		private Run run;
		private object runLock = 0;

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool SetEnvironmentVariable(string strVariable, string strNewValue);

		private Hashtable functionMap;
		[NonSerialized]
		private Process process;
		private ProjectInfo project;
		[NonSerialized]
		private ProfilerSocketServer socketServer;
	}
	public class NProf
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();

			// Always print GPL notice
			Console.WriteLine("NProf version {0} (C) 2003 by Matthew Mastracci", Profiler.Version);
			Console.WriteLine("http://nprof.sourceforge.net");
			Console.WriteLine();
			Console.WriteLine("This is free software, and you are welcome to redistribute it under certain");
			Console.WriteLine("conditions set out by the GNU General Public License.  A copy of the license");
			Console.WriteLine("is available in the distribution package and from the NProf web site.");
			Console.WriteLine();

			ProfilerForm form = ProfilerForm.form;
			if (args.Length > 0)
			{
				form.Project =  SerializationHandler.OpenProjectInfo(args[0].Trim('"'));
			}

			Console.Out.Flush();
			System.Threading.Thread.CurrentThread.Name = "GUI Thread";
			form.Show();
			if (form.Project == null)
			{
				PropertiesForm options = new PropertiesForm(PropertiesForm.ProfilerProjectMode.CreateProject);
				options.ShowDialog();
				if (form.Project == null)
				{
					form.Project = options.Project;
				}
			}
			Application.Run(form);
		}
	}
}
