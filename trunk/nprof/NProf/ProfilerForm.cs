using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using Genghis.Windows.Forms;
using Reflector.UserInterface;
using Crownwood.Magic.Menus;
using DotNetLib.Windows.Forms;

using NProf.GUI;
using NProf;

namespace NProf.GUI
{
	public class ProfilerForm : System.Windows.Forms.Form
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
			}
		}
		private Profiler profiler;
		private ProjectInfo project;
		private TreeView runs;
		private MethodView methods;
		private MethodView callees;
		private MethodView callers;
		private StatusBar statusBar;
		private StatusBarPanel statusPanel;
		private TextBox findText = new TextBox();
		public static ProfilerForm form = new ProfilerForm();
		public static Panel methodPanel;
		private ProfilerForm()
		{
			methodPanel = new Panel();
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.app-icon.ico"));
			Text = "NProf";
			profiler = new Profiler();
			
			string directory = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			string dll = Path.Combine( directory, "msvcr70.dll" );
			if ( LoadLibrary( dll ) == 0 )
				throw new Win32Exception( Marshal.GetLastWin32Error(), "Failed to load msvcr10.dll" );

			CommandBarManager commandBarManager=new CommandBarManager();
			CommandBar commandBar = new CommandBar(CommandBarStyle.ToolBar);
			commandBar.Items.AddRange(
				new CommandBarItem[]
				{
					new CommandBarButton(Images.New, "New", New),
					new CommandBarButton(Images.Save, "Save", Save),

					new CommandBarButton(Images.Back, "Back", Back),
					new CommandBarButton(Images.Forward, "Forward", Forward),
					new CommandBarButton(Images.Run, "Run", delegate { StartRun(null, null); }),
				}
			);

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



			// use something like FlowLayoutPanel instead of nested panels, if possible
			statusPanel = new StatusBarPanel();
			statusPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			statusPanel.Text = "Ready.";
			statusPanel.Width = 904;

			statusBar = new StatusBar();
			statusBar.Panels.Add(statusPanel);
		    statusBar.ShowPanels = true;

			Controls.Add(statusBar);

			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			Controls.Add(panel);

			runs = new TreeView();
			runs.Dock = DockStyle.Left;

			methods = new MethodView();
			methods.Size = new Size(100, 100);

			callers = new MethodView();
			callers.Size = new Size(100, 100);
			callers.Dock = DockStyle.Bottom;
			callees = new MethodView();
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Bottom;
			methods.Dock = DockStyle.Fill;
			methodPanel.Size = new Size(100, 100);
			methodPanel.Dock = DockStyle.Fill;
			Splitter runSplitter=new Splitter();
			runSplitter.Dock=DockStyle.Left;

			FlowLayoutPanel findPanel = new FlowLayoutPanel();
			findPanel.WrapContents = false;
			findPanel.AutoSize = true;
			Button findNext=new Button();
			findNext.AutoSize = true;
			Button findPrevious = new Button();
			findPrevious.AutoSize = true;
			findNext.Text = "Find next";
			findNext.Click+=new EventHandler(findNext_Click);
			findPrevious.Click+=new EventHandler(findPrevious_Click);
			findPrevious.Text = "Find previous";


			findPanel.Controls.AddRange(new Control[] {
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


 		[DllImport("kernel32.dll", SetLastError=true)] static extern int LoadLibrary( string strLibFileName );

		private delegate void HandleProfileComplete( Run run );

		private void OnUIThreadProfileComplete( Run run )
		{
			if (run.State == RunState.Finished
				&& run.Success)
			{
				UpdateRun(run);
			}
		}
		
		private void OnRunStateChanged( Run run, RunState rsOld, RunState rsNew )
		{
			if ( rsNew == RunState.Running || rsNew==RunState.Finished)
			{
				this.BeginInvoke(new HandleProfileComplete(OnUIThreadProfileComplete), new object[] { run });
			}
		}

		private void ProfilerForm_Load(object sender, System.EventArgs e)
		{
			Icon = new Icon( this.GetType().Assembly.GetManifestResourceStream( "NProf.GUI.Resources.app-icon.ico" ) );
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
			if ( form.ShowDialog( this ) == DialogResult.OK )
			{
				project=form.Project;
			}
		}

		private void Open(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.DefaultExt = "nprof";
			dialog.Filter = "NProf projects (*.nprof)|*.nprof|All files (*.*)|*.*";
			dialog.Multiselect = true;
			dialog.Title = "Open a saved NProf project file";

			if( dialog.ShowDialog( this ) == DialogResult.OK )
			{
				foreach( string fileName in dialog.FileNames )
				{
					ProjectInfo project = SerializationHandler.OpenProjectInfo( fileName );
					this.project = project;
					//_pic.Add( project );
					//_pt.SelectProject( project );
				}
			}
		}

		private void Save(object sender, System.EventArgs e)
		{
			SaveProject( Project, false );
		}
		private void StartRun(object sender, System.EventArgs e)
		{
			string message;
			bool success = profiler.CheckSetup( out message );
			if ( !success )
			{
				MessageBox.Show( this, message, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}

			Run run = Project.CreateRun( profiler );
			run.StateChanged += new RunStateEventHandler( OnRunStateChanged );
			run.Start();

			//CreateTabPage( run );

			//processTree.AddRunNode(run);

			//profilerControl._ptProcessTree.AddRunNode(run);


			//profilerfr .AddRunNode(run);
			//_pt.AddRunNode(run);

			//TreeNode tn = (TreeNode)_tvProjects.Invoke(new TreeNodeAdd(OnTreeNodeAdd), new object[] { _tvProjects.Nodes, pi.Name, 0, pi });
			//tn.ImageIndex = 0;

			//pi.Runs.RunAdded += new RunCollection.RunEventHandler(OnRunAdded);
			//pi.Runs.RunRemoved += new RunCollection.RunEventHandler(OnRunRemoved);

			//foreach (Run run in pi.Runs)
			//{
			//    AddRunNode(tn, run);
			//}

			//_pt.SelectRun(run);
			//_pt.SelectRun(run);
		}

		private void Properties(object sender, System.EventArgs e)
		{
			PropertiesForm properties = new PropertiesForm(PropertiesForm.ProfilerProjectMode.ModifyProject);
			properties.Project = Project;
			properties.Mode = PropertiesForm.ProfilerProjectMode.ModifyProject;

			properties.ShowDialog( this );
		}
		
		private void Back(object sender, System.EventArgs e)
		{
			if (back.Count == 0)
				return;

			forward.Push(_navCurrent);
			_navCurrent = (int[])back.Pop();

			isNavigating = true;
			JumpToID(_navCurrent);
			isNavigating = false;
		}

		private void Forward(object sender, System.EventArgs e)
		{
			if (forward.Count == 0)
				return;

			back.Push(_navCurrent);
			_navCurrent = (int[])forward.Pop();

			isNavigating = true;
			JumpToID(_navCurrent);
			isNavigating = false;
		}

		private void About(object sender, System.EventArgs e)
		{
			new AboutForm().ShowDialog(this);
		}

		private bool SaveProject( ProjectInfo project, bool forceSaveDialog )
		{
			if ( project == null )
				return true;

			MessageBox.Show( this, "NOTE: You might not be able to load the data you're saving.  Please keep this in mind.", "Important Note", MessageBoxButtons.OK, MessageBoxIcon.Warning );

			string filename = SerializationHandler.GetFilename( project );

			if( forceSaveDialog || filename == string.Empty )
			{
				SaveFileDialog saveDlg = new SaveFileDialog();

				saveDlg.DefaultExt = "nprof";
				saveDlg.FileName = SerializationHandler.GetFilename( project );;
				saveDlg.Filter = "NProf projects (*.nprof)|*.nprof|All files (*.*)|*.*";
				// saveDlg.InitialDirectory = TODO: store the most recently used direcotry somewhere and go there
				saveDlg.Title = "Save a NProf project file";

				if( saveDlg.ShowDialog( this ) != DialogResult.OK )
					return false;

				project.Name = Path.GetFileNameWithoutExtension( saveDlg.FileName );
				filename = saveDlg.FileName;
			}

			SerializationHandler.SaveProjectInfo( project, filename );

			return true;
		}
		//private void UpdateMenuItems(object sender, System.EventArgs e)
		//{
		//    //bool bCanRunOrEdit = _pt.GetSelectedProject() != null
		//    //    && _pt.GetSelectedProject().ProjectType != ProjectType.VSNet;
		//    bool bCanRunOrEdit = true;

		//    Run run = null;
		//    //Run run = _pt.GetSelectedRun();

		//    runMenu.Enabled = bCanRunOrEdit;
		//    _cmdProjectStop.Enabled = bCanRunOrEdit && ( run != null && run.State == RunState.Running );
		//    optionsMenu.Enabled = bCanRunOrEdit;
		//    //_cmdProjectRunViewMessages.Enabled = ( !IsShowingBlankTab() );
		//    //_cmdProjectRunCopy.Enabled = ( !IsShowingBlankTab() );

		//    //_cmdFileClose.Enabled = bCanRunOrEdit;
		//    _cmdFileSave.Enabled = bCanRunOrEdit;
		//    _cmdFileSaveAs.Enabled = bCanRunOrEdit;
		//    //_cmdFileSaveAll.Enabled = ( !IsShowingBlankTab() );

		//    //_cmdViewNavBack.Enabled = ( !IsShowingBlankTab() );
		//    //_cmdViewNavForward.Enabled = ( !IsShowingBlankTab() );
		//}
		private class Images
		{
			private static Image[] images = null;
	
			// ImageList.Images[int index] does not preserve alpha channel.
			static Images()
			{
				// TODO alpha channel PNG loader is not working on .NET Service RC1
				Bitmap bitmap = ( Bitmap )Bitmap.FromStream( typeof( Images ).Assembly.GetManifestResourceStream( "NProf.GUI.Resources.toolbar16.png" ) );
				int count = (int) (bitmap.Width / bitmap.Height);
				images = new Image[count];
				Rectangle rectangle = new Rectangle(0, 0, bitmap.Height, bitmap.Height);
				for (int i = 0; i < count; i++)
				{
					images[i] = bitmap.Clone(rectangle, bitmap.PixelFormat);
					rectangle.X += bitmap.Height;
				}
			}	
	
			public static Image New               { get { return images[0];  } }
			public static Image Open              { get { return images[1];  } }
			public static Image Save              { get { return images[2];  } }
			public static Image Delete            { get { return images[6];  } }
			public static Image Properties        { get { return images[7];  } }
			public static Image Undo              { get { return images[8];  } }
			public static Image Redo              { get { return images[9];  } }
			public static Image Preview           { get { return images[10]; } }
			public static Image Print             { get { return images[11]; } }
			public static Image Search            { get { return images[12]; } }
			public static Image ReSearch          { get { return images[13]; } }
			public static Image Help              { get { return images[14]; } }
			public static Image ZoomIn            { get { return images[15]; } }
			public static Image ZoomOut           { get { return images[16]; } }
			public static Image Back              { get { return images[17]; } }
			public static Image Forward           { get { return images[18]; } }
			public static Image Favorites         { get { return images[19]; } }
			public static Image AddToFavorites    { get { return images[20]; } }
			public static Image Stop              { get { return images[21]; } }
			public static Image Refresh           { get { return images[22]; } }
			public static Image Home              { get { return images[23]; } }
			public static Image Edit              { get { return images[24]; } }
			public static Image Tools             { get { return images[25]; } }
			public static Image Tiles             { get { return images[26]; } }
			public static Image Icons             { get { return images[27]; } }
			public static Image List              { get { return images[28]; } }
			public static Image Details           { get { return images[29]; } }
			public static Image Pane              { get { return images[30]; } }
			public static Image Culture           { get { return images[31]; } }
			public static Image Languages         { get { return images[32]; } }
			public static Image History           { get { return images[33]; } }
			public static Image Mail              { get { return images[34]; } }
			public static Image Parent            { get { return images[35]; } }
			public static Image FolderProperties  { get { return images[36]; } }
			public static Image Run				  { get { return images[37]; } }
		}
		private Stack back = new Stack();
		private Stack forward = new Stack();

		private int[] _navCurrent = null;
		private bool isNavigating = false;

	
		public void UpdateRun(Run run)
		{

			TreeNode node = new TreeNode(run.StartTime.ToString());
			//node.ImageIndex = GetRunStateImage(run);
			//node.SelectedImageIndex = GetRunStateImage(run);
			node.Tag = run;
			runs.Nodes.Add(node);


			foreach (ThreadInfo ti in run.Process.Threads)
			{
				TreeNode threadNode = new TreeNode(ti.ToString());
				threadNode.ImageIndex = 1;
				threadNode.SelectedImageIndex = 1;
				threadNode.Tag = ti;
				node.Nodes.Add(threadNode);
			}
			runs.SelectedNode = node.Nodes[0];
			currentThread = (ThreadInfo)node.Nodes[0].Tag;

			//run.StateChanged += new RunStateEventHandler(OnRunStateChanged);
			
			UpdateFilters();
			//processTree.process = run.process;
			//_ptProcessTree.UpdateProcesses(run);
			//UpdateFilters();
		}
		private void UpdateFilters()
		{
			methods.Items.Clear();
			callees.Items.Clear();
			callers.Items.Clear();

			try
			{
				methods.BeginUpdate();

				ThreadInfo tiCurrentThread = currentThread;
				foreach (Method fi in tiCurrentThread.FunctionInfoCollection.Values)
				{
					methods.Add(fi);
				}
			}
			finally
			{
				methods.Sort();
				methods.EndUpdate();
			}
		}

		private void _lvFunctionInfo_SelectedItemsChanged(object sender, System.EventArgs e)
		{
			callees.Items.Clear();
			callers.Items.Clear();

			if (methods.SelectedItems.Count == 0)
				return;

			// somebody clicked! empty the forward stack and push this click on the "Back" stack.
			if (!isNavigating)
			{
				forward.Clear();
				if (_navCurrent != null)
					back.Push(_navCurrent);

				ArrayList lst = new ArrayList();

				for (int idx = 0; idx < methods.SelectedItems.Count; ++idx)
					if (methods.SelectedItems[idx].Tag != null)
						lst.Add((methods.SelectedItems[idx].Tag as Method).ID);

				_navCurrent = (int[])lst.ToArray(typeof(int));
			}

			UpdateCalleeList();
			UpdateCallerList();
		}
		private void UpdateCallerList()
		{
			callers.BeginUpdate();

			bool multipleSelected = (methods.SelectedItems.Count > 1);
			callers.ShowPlusMinus = multipleSelected;
			callers.ShowRootTreeLines = multipleSelected;
			callers.ShowTreeLines = multipleSelected;

			ThreadInfo tiCurrentThread = currentThread;
			foreach (ContainerListViewItem item in methods.SelectedItems)
			{
				Method mfi = (Method)item.Tag;

				foreach (Method fi in tiCurrentThread.FunctionInfoCollection.Values)
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

		private void UpdateCalleeList()
		{
			callees.BeginUpdate();

			bool multipleSelected = (methods.SelectedItems.Count > 1);
			callees.ShowPlusMinus = multipleSelected;
			callees.ShowRootTreeLines = multipleSelected;
			callees.ShowTreeLines = multipleSelected;

			ContainerListViewItem lviSuspend = null;

			foreach ( ContainerListViewItem item in methods.SelectedItems )
			{
				Method fi = ( Method )item.Tag;

				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					callees.Add(cfi);
				}

				ContainerListViewItem inMethod = callees.Items.Add("( in method )");
				inMethod.SubItems[1].Text = fi.Calls.ToString();
				inMethod.SubItems[2].Text = fi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
				inMethod.Tag = fi;



				if ( fi.TotalSuspendedTicks > 0 )
				{
					if ( lviSuspend == null) // don't have it
					{
						lviSuspend = callees.Items.Add("(thread suspended)");
						lviSuspend.SubItems[1].Text = "-";
						lviSuspend.SubItems[2].Text = fi.PercentOfTotalTimeSuspended.ToString("0.00;-0.00;0");
					}
					else // do, update totals
					{
						lviSuspend.SubItems[ 2 ].Text = "-";
					}

					// either way, add a child pointing back to the parent
					ContainerListViewItem lvi = lviSuspend.Items.Add(fi.Signature.Signature);
					lvi.SubItems[ 1 ].Text = "-";
					lvi.SubItems[ 2 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
				}
			}
			callees.Sort();
			callees.EndUpdate();
		}

		private void _lvChildInfo_DoubleClick(object sender, System.EventArgs e)
		{
			ContainerListView ctl = sender as ContainerListView;

			if (ctl.SelectedItems.Count == 0)
				return;

			if (ctl.SelectedItems[0].Tag is CalleeFunctionInfo)
			{
				CalleeFunctionInfo cfi = (CalleeFunctionInfo)ctl.SelectedItems[0].Tag;
				if (cfi == null)
					MessageBox.Show("No information available for this item");
				else
					JumpToID(cfi.ID);
			}
			else if (ctl.SelectedItems[0].Tag is Method)
			{
				Method fi = (Method)ctl.SelectedItems[0].Tag;
				if (fi == null)
					MessageBox.Show("No information available for this item");
				else
					JumpToID(fi.ID);
			}
		}
		private class SignatureComparer : IComparer
		{
			private string text;
			public SignatureComparer(string text)
			{
				this.text = text;
			}
			public int Compare(object x, object y)
			{
				ContainerListViewItem a = (ContainerListViewItem)x;
				ContainerListViewItem b = (ContainerListViewItem)y;
				bool aFound = a.SubItems[1].Text.ToLower().IndexOf(text) != -1;
				bool bFound = b.SubItems[1].Text.ToLower().IndexOf(text) != -1;
				if (aFound && !bFound)
				{
					return -1;
				}
				else if (bFound && !aFound)
				{
					return 1;
				}
				else
				{
					if (a.Text == b.Text)
					{
						return 0;
					}
					else
					{
						return -1;
					}
				}
			}
		}

		private void JumpToID(int nID)
		{
			JumpToID(new int[] { nID });
		}

		private void JumpToID(int[] ids)
		{
			methods.SelectedItems.Clear();

			foreach (int id in ids)
				foreach (ContainerListViewItem lvi in methods.Items)
				{
					Method fi = (Method)lvi.Tag;

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

			return ((Method)methods.SelectedItems[0].Tag).ID;
		}

		private ThreadInfo currentThread;		

		private void Find(bool forward)
		{
			if (findText.Text != "")
			{
				ContainerListViewItem item;
				if(methods.SelectedItems.Count==0)
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
				while (item!=null)
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
		public static Splitter Splitter(DockStyle dock)
		{
			Splitter splitter = new Splitter();
			splitter.Dock = dock;
			return splitter;
		}
	}
	public class MethodItem : ContainerListViewItem
	{
		private Method function;
		public MethodItem(Method function)
			: base(function.Signature.Signature)
		{
			this.function = function;
			this.SubItems[1].Text = function.Calls.ToString();
			this.SubItems[2].Text = function.PercentOfTotalTimeInMethod.ToString("0.0000;-0.0000;0");
			this.Tag = function;
		}
	}
	public class MethodView : ContainerListView
	{
		public MethodView()
		{
			//ProfilerForm.methodPanel.SizeChanged+= delegate
			//{
			//    Columns[0].Width = ProfilerForm.methodPanel.Width-Columns[1].Width-Columns[2].Width;
			//};
			Columns.Add("Method name");
			Columns.Add("Number of calls");
			Columns.Add("Time spent");
			Font = new Font("Tahoma", 8.0f);
		}
		public void Add(NProf.Method function)
		{
			ContainerListViewItem item = Items.Add(function.Signature.Signature);
			item.SubItems[1].Text = function.Calls.ToString();
			item.SubItems[2].Text = function.PercentOfTotalTimeInMethod.ToString("0.0000;-0.0000;0");
			item.Tag = function;
		}
		public void Add(CalleeFunctionInfo function)
		{
			ContainerListViewItem item = Items.Add(function.Signature);
			item.SubItems[1].Text = function.Calls.ToString();
			item.SubItems[2].Text = function.TimeInMethod.ToString("0.0000;-0.0000;0");
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
}
