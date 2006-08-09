using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;
using NProf.Utilities.DataStore;
using Genghis.Windows.Forms;
using Reflector.UserInterface;
using Crownwood.Magic.Menus;
using DotNetLib.Windows.Forms;


namespace NProf.GUI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ProfilerForm : System.Windows.Forms.Form
	{
		private Stack _stackBack;
		private Stack _stackForward;
		//private ProjectTree _pt;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.StatusBar _sbStatusBar;
		private Crownwood.Magic.Menus.MenuCommand _menuFile;
		//private Crownwood.Magic.Menus.MenuCommand _menuEdit;
		private Crownwood.Magic.Menus.MenuCommand _menuHelp;
		//private Crownwood.Magic.Controls.TabControl _tcProfilers;
		private Crownwood.Magic.Menus.MenuControl _menuMain;
		private Crownwood.Magic.Menus.MenuCommand _menuProject;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRun;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectOptions;
		private Crownwood.Magic.Docking.DockingManager _dock;

		private Crownwood.Magic.Menus.MenuCommand _sep2;
		private Crownwood.Magic.Menus.MenuCommand _sep1;
		private Crownwood.Magic.Menus.MenuCommand _cmdHelpAbout;
		private System.Windows.Forms.StatusBarPanel _sbpMessage;
		private Profiler							_p;
		//private ProjectTree							_pt;
		private ProjectInfo project;
		//private ProjectInfoCollection _pic;
		private ProjectInfo _piInitialProject;
		private ProjectInfo							_piVSNetProject;
		//private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunViewMessages;
		//private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunCopy;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectStop;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileNew;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileOpen;
		//private Crownwood.Magic.Menus.MenuCommand _cmdFileClose;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileSave;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileSaveAs;
		//private Crownwood.Magic.Menus.MenuCommand _cmdFileSaveAll;
		//private Crownwood.Magic.Menus.MenuCommand _cmdFileRecentProjects;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileExit;
		private Crownwood.Magic.Menus.MenuCommand _cmdViewNavBack;
		private Crownwood.Magic.Menus.MenuCommand _cmdViewNavForward;
		private Crownwood.Magic.Menus.MenuCommand _sep4;
		private Crownwood.Magic.Menus.MenuCommand _sep3;
		private Reflector.UserInterface.CommandBarManager commandBarManager1;
		private Reflector.UserInterface.CommandBar commandBar1;
		private Reflector.UserInterface.CommandBar commandBar2;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeParent;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeTotal;
		private DotNetLib.Windows.Forms.ContainerListView _lvCallersInfo;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerParent;
		private TabPage _tcCallers;
		private Splitter splitter3;
		private TreeView _tvNamespaceInfo;
		private Timer _tmrFilterThrottle;
		private Label _lblFilterSignatures;
		private Splitter splitter1;
		private Panel panel2;
		private TextBox _txtFilterBar;
		private Panel panel4;
		private ProcessTree _ptProcessTree;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionCalls;
		private Panel panel1;
		private Panel panel3;
		private DotNetLib.Windows.Forms.ContainerListView _lvFunctionInfo;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionChildren;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionSuspended;
		private Splitter splitter2;
		private TabControl _tcCalls;
		private TabPage _tpCallees;
		private DotNetLib.Windows.Forms.ContainerListView _lvCalleesInfo;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeCalls;
		private ContainerListViewColumnHeader containerListViewColumnHeader1;
		private ContainerListViewColumnHeader containerListViewColumnHeader2;
		private ContainerListViewColumnHeader containerListViewColumnHeader3;
		private ContainerListViewColumnHeader containerListViewColumnHeader4;
		private TabPage tabPage1;
		private ContainerListView containerListView1;
		private ContainerListViewColumnHeader containerListViewColumnHeader5;
		private ContainerListViewColumnHeader containerListViewColumnHeader6;
		private Splitter splitter4;
		private Splitter splitter5;
		private TreeView treeView1;
		private Timer timer1;
		private ProcessTree processTree1;
		private TextBox textBox1;
		private Panel panel5;
		private Label label1;
		private Panel panel6;
		private ContainerListViewColumnHeader containerListViewColumnHeader7;
		private ContainerListViewColumnHeader containerListViewColumnHeader8;
		private ContainerListViewColumnHeader containerListViewColumnHeader9;
		private ContainerListViewColumnHeader containerListViewColumnHeader10;
		private Panel panel7;
		private ContainerListView containerListView2;
		private ContainerListViewColumnHeader containerListViewColumnHeader11;
		private ContainerListViewColumnHeader containerListViewColumnHeader12;
		private Splitter splitter6;
		private TabControl tabControl1;
		private TabPage tabPage2;
		private ContainerListView containerListView3;
		private ContainerListViewColumnHeader containerListViewColumnHeader13;
		private ContainerListViewColumnHeader containerListViewColumnHeader14;
		private ContainerListViewColumnHeader containerListViewColumnHeader15;
		private ContainerListViewColumnHeader containerListViewColumnHeader16;
		private Panel panel8;
		private MenuCommand _menuView;
		private IContainer components;

		public ProjectInfo Project
		{
			get
			{
				return project;
			}
			set
			{
				//if (project == null)
				//{
				//    ProfilerControl control = new ProfilerControl();
				//    control.Dock = DockStyle.Top;
				//    this.Controls.Add(control);
				//    _menuMain.Dock = DockStyle.Top;
				//}
				project = value;
			}
		}
		private ProfilerControl profilerControl;
		public ProfilerForm()
		{
			profilerControl= new ProfilerControl();
			profilerControl.Dock = DockStyle.Fill;
			this.Controls.Add(profilerControl);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_stackBack = new Stack();
			_stackForward = new Stack();
			_p = new Profiler();
			//_p.ProcessCompleted += new Profiler.ProcessCompletedHandler( OnProfileComplete );
			_p.Error += new Profiler.ErrorHandler( OnError );
			_piInitialProject = null;
			
			string strDirectory = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			string strDLL = Path.Combine( strDirectory, "msvcr70.dll" );
			if ( LoadLibrary( strDLL ) == 0 )
				throw new Win32Exception( Marshal.GetLastWin32Error(), "Failed to load msvcr10.dll" );


			CommandBarItem item;
			commandBar2 = new CommandBar();
			commandBar2.Style = CommandBarStyle.ToolBar;

			item = new CommandBarButton(Images.New, "New", new EventHandler(_cmdFileNew_Click));
			_cmdFileNew.Image = Images.New;
			commandBar2.Items.Add(item);

			//item = new CommandBarButton(Images.Open, "New", new EventHandler(_cmdFileNew_Click));
			////item = new CommandBarButton(Images.Open, "Open", new EventHandler(_cmdFileOpen_Click));
			//_cmdFileOpen.Image = Images.New;
			//commandBar2.Items.Add(item);

			item = new CommandBarButton(Images.Save, "Save", new EventHandler(_cmdFileSave_Click));
			_cmdFileSave.Image = Images.Save;
			commandBar2.Items.Add(item);




			commandBar2.Items.AddSeparator();

			item = new CommandBarButton(Images.Back, "Back", new EventHandler(_cmdViewNavBack_Click));
			_cmdViewNavBack.Image = Images.Back;
			commandBar2.Items.Add(item);

			item = new CommandBarButton(Images.Forward, "Forward", new EventHandler(_cmdViewNavForward_Click));
			_cmdViewNavForward.Image = Images.Forward;
			commandBar2.Items.Add(item);

			commandBar2.Items.AddSeparator();

			commandBar2.Items.Add(new CommandBarButton(Images.Run, "Run", delegate { _cmdProjectRun_Click(null, null); }));






			commandBarManager1.CommandBars.Add(commandBar2);

			commandBarManager1.Dock = DockStyle.Top;
		}

 		[DllImport("kernel32.dll", SetLastError=true)] static extern int LoadLibrary( string strLibFileName );

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this._menuMain = new Crownwood.Magic.Menus.MenuControl();
			this._menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileOpen = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileNew = new Crownwood.Magic.Menus.MenuCommand();
			this._sep1 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileSave = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileSaveAs = new Crownwood.Magic.Menus.MenuCommand();
			this._sep2 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileExit = new Crownwood.Magic.Menus.MenuCommand();
			this._menuView = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdViewNavBack = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdViewNavForward = new Crownwood.Magic.Menus.MenuCommand();
			this._menuProject = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRun = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectStop = new Crownwood.Magic.Menus.MenuCommand();
			this._sep4 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectOptions = new Crownwood.Magic.Menus.MenuCommand();
			this._menuHelp = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdHelpAbout = new Crownwood.Magic.Menus.MenuCommand();
			this._sep3 = new Crownwood.Magic.Menus.MenuCommand();
			this._sbStatusBar = new System.Windows.Forms.StatusBar();
			this._sbpMessage = new System.Windows.Forms.StatusBarPanel();
			this.commandBarManager1 = new Reflector.UserInterface.CommandBarManager();
			this.commandBar1 = new Reflector.UserInterface.CommandBar();
			this.colCallerSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this._lvCallersInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colCallerParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this._tcCallers = new System.Windows.Forms.TabPage();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this._tvNamespaceInfo = new System.Windows.Forms.TreeView();
			this._tmrFilterThrottle = new System.Windows.Forms.Timer(this.components);
			this._lblFilterSignatures = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this._txtFilterBar = new System.Windows.Forms.TextBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.colFunctionSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this._lvFunctionInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colFunctionChildren = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionSuspended = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this._tcCalls = new System.Windows.Forms.TabControl();
			this._tpCallees = new System.Windows.Forms.TabPage();
			this._lvCalleesInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colCalleeID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader1 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader2 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader3 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader4 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.containerListView1 = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader5 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader6 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter4 = new System.Windows.Forms.Splitter();
			this.splitter5 = new System.Windows.Forms.Splitter();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel6 = new System.Windows.Forms.Panel();
			this.containerListViewColumnHeader7 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader8 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader9 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader10 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.panel7 = new System.Windows.Forms.Panel();
			this.containerListView2 = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader11 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader12 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter6 = new System.Windows.Forms.Splitter();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.containerListView3 = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader13 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader14 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader15 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader16 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.panel8 = new System.Windows.Forms.Panel();
			this._ptProcessTree = new NProf.GUI.ProcessTree();
			this.processTree1 = new NProf.GUI.ProcessTree();
			((System.ComponentModel.ISupportInitialize)(this._sbpMessage)).BeginInit();
			this._tcCallers.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this._tcCalls.SuspendLayout();
			this._tpCallees.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel7.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel8.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuItem1
			// 
			this.menuItem1.Index = -1;
			this.menuItem1.Text = "Close Tab";
			// 
			// _menuMain
			// 
			this._menuMain.AnimateStyle = Crownwood.Magic.Menus.Animation.System;
			this._menuMain.AnimateTime = 100;
			this._menuMain.Cursor = System.Windows.Forms.Cursors.Arrow;
			this._menuMain.Direction = Crownwood.Magic.Common.Direction.Horizontal;
			this._menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this._menuMain.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
			this._menuMain.HighlightTextColor = System.Drawing.SystemColors.MenuText;
			this._menuMain.Location = new System.Drawing.Point(0, 0);
			this._menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this._menuFile,
            this._menuView,
            this._menuProject,
            this._menuHelp});
			this._menuMain.Name = "_menuMain";
			this._menuMain.Size = new System.Drawing.Size(920, 25);
			this._menuMain.Style = Crownwood.Magic.Common.VisualStyle.IDE;
			this._menuMain.TabIndex = 0;
			this._menuMain.TabStop = false;
			this._menuMain.Text = "menuControl1";
			// 
			// _menuFile
			// 
			this._menuFile.Description = "File";
			this._menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
			this._cmdFileNew,
            this._cmdFileOpen,
            this._sep1,
            this._cmdFileSave,
            this._cmdFileSaveAs,
            this._sep2,
            this._cmdFileExit});
			this._menuFile.Text = "&File";
			// 
			// _cmdFileNew
			// 
			this._cmdFileNew.Description = "Create a new profile project";
			this._cmdFileNew.Text = "&New...";
			this._cmdFileNew.Click += new System.EventHandler(this._cmdFileNew_Click);

			// 
			// _cmdFileOpen
			// 
			this._cmdFileOpen.Description = "Open a profile project";
			this._cmdFileOpen.Text = "&Open...";
			this._cmdFileOpen.Click += new System.EventHandler(this._cmdFileOpen_Click);
			// 
			// _sep1
			// 
			this._sep1.Description = "MenuItem";
			this._sep1.Text = "-";
			// 
			// _cmdFileSave
			// 
			this._cmdFileSave.Description = "Save the active profiler project";
			this._cmdFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this._cmdFileSave.Text = "&Save";
			this._cmdFileSave.Click += new System.EventHandler(this._cmdFileSave_Click);
			// 
			// _cmdFileSaveAs
			// 
			this._cmdFileSaveAs.Description = "Save the active profiler project as a specified file name";
			this._cmdFileSaveAs.Text = "Save &As...";
			this._cmdFileSaveAs.Click += new System.EventHandler(this._cmdFileSaveAs_Click);
			// 
			// _sep2
			// 
			this._sep2.Description = "MenuItem";
			this._sep2.Text = "-";
			// 
			// _cmdFileExit
			// 
			this._cmdFileExit.Description = "Exit the application";
			this._cmdFileExit.Shortcut = System.Windows.Forms.Shortcut.AltF4;
			this._cmdFileExit.Text = "E&xit";
			this._cmdFileExit.Click += new System.EventHandler(this._cmdFileExit_Click);
			// 
			// _menuView
			// 
			this._menuView.Description = "MenuItem";
			this._menuView.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this._cmdViewNavBack,
            this._cmdViewNavForward});
			this._menuView.Text = "View";
			// 
			// _cmdViewNavBack
			// 
			this._cmdViewNavBack.Description = "Navigate Back";
			this._cmdViewNavBack.Text = "Back";
			this._cmdViewNavBack.Click += new System.EventHandler(this._cmdViewNavBack_Click);
			// 
			// _cmdViewNavForward
			// 
			this._cmdViewNavForward.Description = "Navigate Forward";
			this._cmdViewNavForward.Text = "Forward";
			this._cmdViewNavForward.Click += new System.EventHandler(this._cmdViewNavForward_Click);
			// 
			// _menuProject
			// 
			this._menuProject.Description = "Project commands";
			this._menuProject.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this._cmdProjectRun,
            this._cmdProjectStop,
            this._sep4,
            this._cmdProjectOptions});
			this._menuProject.Text = "&Project";
			// 
			// _cmdProjectRun
			// 
			this._cmdProjectRun.Description = "Run the current project";
			this._cmdProjectRun.Shortcut = System.Windows.Forms.Shortcut.F5;
			this._cmdProjectRun.Text = "Start";
			this._cmdProjectRun.Update += new System.EventHandler(this.UpdateMenuItems);
			this._cmdProjectRun.Click += new System.EventHandler(this._cmdProjectRun_Click);
			// 
			// _cmdProjectStop
			// 
			this._cmdProjectStop.Description = "Stop the running project";
			this._cmdProjectStop.Text = "Stop";
			// 
			// _sep4
			// 
			this._sep4.Description = "-";
			this._sep4.Text = "-";
			// 
			// _cmdProjectOptions
			// 
			this._cmdProjectOptions.Description = "Modify the options for this project";
			this._cmdProjectOptions.Shortcut = System.Windows.Forms.Shortcut.F2;
			this._cmdProjectOptions.Text = "Properties...";
			this._cmdProjectOptions.Update += new System.EventHandler(this.UpdateMenuItems);
			this._cmdProjectOptions.Click += new System.EventHandler(this._cmdProjectOptions_Click);
			// 
			// _menuHelp
			// 
			this._menuHelp.Description = "Help";
			this._menuHelp.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this._cmdHelpAbout});
			this._menuHelp.Text = "&Help";
			// 
			// _cmdHelpAbout
			// 
			this._cmdHelpAbout.Description = "About nprof";
			this._cmdHelpAbout.Text = "About nprof...";
			this._cmdHelpAbout.Click += new System.EventHandler(this._cmdHelpAbout_Click);
			// 
			// _sep3
			// 
			this._sep3.Description = "-";
			this._sep3.Text = "-";
			// 
			// _sbStatusBar
			// 
			this._sbStatusBar.Location = new System.Drawing.Point(0, 655);
			this._sbStatusBar.Name = "_sbStatusBar";
			this._sbStatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this._sbpMessage});
			this._sbStatusBar.ShowPanels = true;
			this._sbStatusBar.Size = new System.Drawing.Size(920, 22);
			this._sbStatusBar.TabIndex = 3;
			// 
			// _sbpMessage
			// 
			this._sbpMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this._sbpMessage.Name = "_sbpMessage";
			this._sbpMessage.Text = "Ready.";
			this._sbpMessage.Width = 904;
			// 
			// commandBarManager1
			// 
			this.commandBarManager1.Dock = System.Windows.Forms.DockStyle.Top;
			this.commandBarManager1.Location = new System.Drawing.Point(0, 25);
			this.commandBarManager1.Name = "commandBarManager1";
			this.commandBarManager1.Size = new System.Drawing.Size(920, 1);
			this.commandBarManager1.TabIndex = 3;
			this.commandBarManager1.TabStop = false;
			this.commandBarManager1.Text = "commandBarManager1";
			// 
			// commandBar1
			// 
			this.commandBar1.Dock = System.Windows.Forms.DockStyle.Top;
			this.commandBar1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
			this.commandBar1.Location = new System.Drawing.Point(0, 64);
			this.commandBar1.Name = "commandBar1";
			this.commandBar1.Size = new System.Drawing.Size(0, 0);
			this.commandBar1.Style = Reflector.UserInterface.CommandBarStyle.ToolBar;
			this.commandBar1.TabIndex = 4;
			this.commandBar1.TabStop = false;
			this.commandBar1.Text = "commandBar1";
			// 
			// colCallerSignature
			// 
			this.colCallerSignature.DisplayIndex = 1;
			this.colCallerSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colCallerSignature.Text = "Signature";
			this.colCallerSignature.ToolTip = "Signature Tool Tip";
			this.colCallerSignature.Width = 400;
			// 
			// colCallerID
			// 
			this.colCallerID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colCallerID.Text = "ID";
			this.colCallerID.ToolTip = "ID Tool Tip";
			this.colCallerID.Width = 100;
			// 
			// colCallerTotal
			// 
			this.colCallerTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerTotal.DisplayIndex = 3;
			this.colCallerTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerTotal.Text = "% of Total";
			this.colCallerTotal.ToolTip = "% of Total Tool Tip";
			this.colCallerTotal.Width = 70;
			// 
			// colCallerCalls
			// 
			this.colCallerCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerCalls.DisplayIndex = 2;
			this.colCallerCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerCalls.Text = "# of Calls";
			this.colCallerCalls.ToolTip = "# of Calls Tool Tip";
			this.colCallerCalls.Width = 70;
			// 
			// colCalleeParent
			// 
			this.colCalleeParent.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeParent.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeParent.DisplayIndex = 4;
			this.colCalleeParent.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeParent.Text = "% of Parent";
			this.colCalleeParent.ToolTip = "% of Parent Tool Tip";
			this.colCalleeParent.Width = 70;
			// 
			// colCalleeTotal
			// 
			this.colCalleeTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeTotal.DisplayIndex = 3;
			this.colCalleeTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeTotal.Text = "% of Total";
			this.colCalleeTotal.ToolTip = "% of Total Tool Tip";
			this.colCalleeTotal.Width = 70;
			// 
			// _lvCallersInfo
			// 
			this._lvCallersInfo.AllowColumnReorder = true;
			this._lvCallersInfo.CaptureFocusClick = false;
			this._lvCallersInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.colCallerID,
            this.colCallerSignature,
            this.colCallerCalls,
            this.colCallerTotal,
            this.colCallerParent});
			this._lvCallersInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvCallersInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvCallersInfo.HeaderHeight = 33;
			this._lvCallersInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvCallersInfo.HideSelection = false;
			this._lvCallersInfo.Location = new System.Drawing.Point(0, 0);
			this._lvCallersInfo.Name = "_lvCallersInfo";
			this._lvCallersInfo.Size = new System.Drawing.Size(709, 150);
			this._lvCallersInfo.TabIndex = 18;
			// 
			// colCallerParent
			// 
			this.colCallerParent.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerParent.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerParent.DisplayIndex = 4;
			this.colCallerParent.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerParent.Text = "% of Parent";
			this.colCallerParent.ToolTip = "% of Parent Tool Tip";
			this.colCallerParent.Width = 70;
			// 
			// _tcCallers
			// 
			this._tcCallers.Controls.Add(this._lvCallersInfo);
			this._tcCallers.Location = new System.Drawing.Point(4, 22);
			this._tcCallers.Name = "_tcCallers";
			this._tcCallers.Size = new System.Drawing.Size(709, 150);
			this._tcCallers.TabIndex = 1;
			this._tcCallers.Text = "Callers";
			// 
			// splitter3
			// 
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(0, 150);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(200, 3);
			this.splitter3.TabIndex = 15;
			this.splitter3.TabStop = false;
			// 
			// _tvNamespaceInfo
			// 
			this._tvNamespaceInfo.CheckBoxes = true;
			this._tvNamespaceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tvNamespaceInfo.FullRowSelect = true;
			this._tvNamespaceInfo.Location = new System.Drawing.Point(0, 150);
			this._tvNamespaceInfo.Name = "_tvNamespaceInfo";
			this._tvNamespaceInfo.Size = new System.Drawing.Size(200, 479);
			this._tvNamespaceInfo.TabIndex = 8;
			// 
			// _tmrFilterThrottle
			// 
			this._tmrFilterThrottle.Interval = 300;
			// 
			// _lblFilterSignatures
			// 
			this._lblFilterSignatures.Dock = System.Windows.Forms.DockStyle.Left;
			this._lblFilterSignatures.Location = new System.Drawing.Point(0, 0);
			this._lblFilterSignatures.Name = "_lblFilterSignatures";
			this._lblFilterSignatures.Size = new System.Drawing.Size(88, 24);
			this._lblFilterSignatures.TabIndex = 14;
			this._lblFilterSignatures.Text = "Filter signatures:";
			this._lblFilterSignatures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 24);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 605);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._txtFilterBar);
			this.panel2.Controls.Add(this._lblFilterSignatures);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(200, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(720, 24);
			this.panel2.TabIndex = 12;
			// 
			// _txtFilterBar
			// 
			this._txtFilterBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtFilterBar.Location = new System.Drawing.Point(88, 0);
			this._txtFilterBar.Multiline = true;
			this._txtFilterBar.Name = "_txtFilterBar";
			this._txtFilterBar.Size = new System.Drawing.Size(616, 24);
			this._txtFilterBar.TabIndex = 13;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.splitter3);
			this.panel4.Controls.Add(this._tvNamespaceInfo);
			this.panel4.Controls.Add(this._ptProcessTree);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(200, 629);
			this.panel4.TabIndex = 15;
			// 
			// colFunctionSignature
			// 
			this.colFunctionSignature.DisplayIndex = 1;
			this.colFunctionSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colFunctionSignature.Text = "Signature";
			this.colFunctionSignature.ToolTip = "Signature Tool Tip";
			this.colFunctionSignature.Width = 350;
			// 
			// colFunctionID
			// 
			this.colFunctionID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colFunctionID.Text = "ID";
			this.colFunctionID.ToolTip = "ID Tool Tip";
			this.colFunctionID.Width = 100;
			// 
			// colFunctionTotal
			// 
			this.colFunctionTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionTotal.DisplayIndex = 3;
			this.colFunctionTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionTotal.Text = "% of Total";
			this.colFunctionTotal.ToolTip = "% of Total Tool Tip";
			this.colFunctionTotal.Width = 70;
			// 
			// colFunctionCalls
			// 
			this.colFunctionCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionCalls.DisplayIndex = 2;
			this.colFunctionCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionCalls.Text = "# of Calls";
			this.colFunctionCalls.ToolTip = "# of Calls Tool Tip";
			this.colFunctionCalls.Width = 70;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.panel4);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(920, 629);
			this.panel1.TabIndex = 11;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this._lvFunctionInfo);
			this.panel3.Controls.Add(this.splitter2);
			this.panel3.Controls.Add(this._tcCalls);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(203, 24);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(717, 605);
			this.panel3.TabIndex = 13;
			// 
			// _lvFunctionInfo
			// 
			this._lvFunctionInfo.AllowColumnReorder = true;
			this._lvFunctionInfo.AllowMultiSelect = true;
			this._lvFunctionInfo.CaptureFocusClick = false;
			this._lvFunctionInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.colFunctionID,
            this.colFunctionSignature,
            this.colFunctionCalls,
            this.colFunctionTotal,
            this.colFunctionChildren,
            this.colFunctionSuspended});
			this._lvFunctionInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvFunctionInfo.HeaderHeight = 33;
			this._lvFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvFunctionInfo.HideSelection = false;
			this._lvFunctionInfo.Location = new System.Drawing.Point(0, 0);
			this._lvFunctionInfo.MultipleColumnSort = true;
			this._lvFunctionInfo.Name = "_lvFunctionInfo";
			this._lvFunctionInfo.Size = new System.Drawing.Size(717, 426);
			this._lvFunctionInfo.TabIndex = 24;
			// 
			// colFunctionChildren
			// 
			this.colFunctionChildren.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionChildren.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionChildren.DisplayIndex = 4;
			this.colFunctionChildren.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionChildren.Text = "% in\nChildren";
			this.colFunctionChildren.ToolTip = "% in Children Tool Tip";
			this.colFunctionChildren.Width = 70;
			// 
			// colFunctionSuspended
			// 
			this.colFunctionSuspended.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionSuspended.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionSuspended.DisplayIndex = 5;
			this.colFunctionSuspended.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionSuspended.Text = "% Suspended";
			this.colFunctionSuspended.ToolTip = "% Suspended Tool Tip";
			this.colFunctionSuspended.Width = 70;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 426);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(717, 3);
			this.splitter2.TabIndex = 23;
			this.splitter2.TabStop = false;
			// 
			// _tcCalls
			// 
			this._tcCalls.Controls.Add(this._tpCallees);
			this._tcCalls.Controls.Add(this._tcCallers);
			this._tcCalls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._tcCalls.Location = new System.Drawing.Point(0, 429);
			this._tcCalls.Name = "_tcCalls";
			this._tcCalls.SelectedIndex = 0;
			this._tcCalls.Size = new System.Drawing.Size(717, 176);
			this._tcCalls.TabIndex = 22;
			// 
			// _tpCallees
			// 
			this._tpCallees.Controls.Add(this._lvCalleesInfo);
			this._tpCallees.Location = new System.Drawing.Point(4, 22);
			this._tpCallees.Name = "_tpCallees";
			this._tpCallees.Size = new System.Drawing.Size(709, 150);
			this._tpCallees.TabIndex = 0;
			this._tpCallees.Text = "Callees";
			// 
			// _lvCalleesInfo
			// 
			this._lvCalleesInfo.AllowColumnReorder = true;
			this._lvCalleesInfo.CaptureFocusClick = false;
			this._lvCalleesInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.colCalleeID,
            this.colCalleeSignature,
            this.colCalleeCalls,
            this.colCalleeTotal,
            this.colCalleeParent});
			this._lvCalleesInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvCalleesInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvCalleesInfo.HeaderHeight = 33;
			this._lvCalleesInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvCalleesInfo.HideSelection = false;
			this._lvCalleesInfo.Location = new System.Drawing.Point(0, 0);
			this._lvCalleesInfo.Name = "_lvCalleesInfo";
			this._lvCalleesInfo.Size = new System.Drawing.Size(709, 150);
			this._lvCalleesInfo.TabIndex = 17;
			// 
			// colCalleeID
			// 
			this.colCalleeID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colCalleeID.Text = "ID";
			this.colCalleeID.ToolTip = "ID Tool Tip";
			this.colCalleeID.Width = 100;
			// 
			// colCalleeSignature
			// 
			this.colCalleeSignature.DisplayIndex = 1;
			this.colCalleeSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colCalleeSignature.Text = "Signature";
			this.colCalleeSignature.ToolTip = "Signature Tool Tip";
			this.colCalleeSignature.Width = 400;
			// 
			// colCalleeCalls
			// 
			this.colCalleeCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeCalls.DisplayIndex = 2;
			this.colCalleeCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeCalls.Text = "# of Calls";
			this.colCalleeCalls.ToolTip = "# of Calls Tool Tip";
			this.colCalleeCalls.Width = 70;
			// 
			// containerListViewColumnHeader1
			// 
			this.containerListViewColumnHeader1.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader1.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader1.DisplayIndex = 2;
			this.containerListViewColumnHeader1.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader1.Text = "# of Calls";
			this.containerListViewColumnHeader1.ToolTip = "# of Calls Tool Tip";
			this.containerListViewColumnHeader1.Width = 70;
			// 
			// containerListViewColumnHeader2
			// 
			this.containerListViewColumnHeader2.DisplayIndex = 1;
			this.containerListViewColumnHeader2.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader2.Text = "Signature";
			this.containerListViewColumnHeader2.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader2.Width = 400;
			// 
			// containerListViewColumnHeader3
			// 
			this.containerListViewColumnHeader3.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader3.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader3.DisplayIndex = 4;
			this.containerListViewColumnHeader3.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader3.Text = "% of Parent";
			this.containerListViewColumnHeader3.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader3.Width = 70;
			// 
			// containerListViewColumnHeader4
			// 
			this.containerListViewColumnHeader4.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader4.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader4.DisplayIndex = 3;
			this.containerListViewColumnHeader4.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader4.Text = "% of Total";
			this.containerListViewColumnHeader4.ToolTip = "% of Total Tool Tip";
			this.containerListViewColumnHeader4.Width = 70;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.containerListView1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(709, 150);
			this.tabPage1.TabIndex = 1;
			this.tabPage1.Text = "Callers";
			// 
			// containerListView1
			// 
			this.containerListView1.AllowColumnReorder = true;
			this.containerListView1.CaptureFocusClick = false;
			this.containerListView1.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader5,
            this.containerListViewColumnHeader2,
            this.containerListViewColumnHeader1,
            this.containerListViewColumnHeader4,
            this.containerListViewColumnHeader3});
			this.containerListView1.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.containerListView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerListView1.HeaderHeight = 33;
			this.containerListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.containerListView1.HideSelection = false;
			this.containerListView1.Location = new System.Drawing.Point(0, 0);
			this.containerListView1.Name = "containerListView1";
			this.containerListView1.Size = new System.Drawing.Size(709, 150);
			this.containerListView1.TabIndex = 18;
			// 
			// containerListViewColumnHeader5
			// 
			this.containerListViewColumnHeader5.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader5.Text = "ID";
			this.containerListViewColumnHeader5.ToolTip = "ID Tool Tip";
			this.containerListViewColumnHeader5.Width = 100;
			// 
			// containerListViewColumnHeader6
			// 
			this.containerListViewColumnHeader6.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader6.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader6.DisplayIndex = 4;
			this.containerListViewColumnHeader6.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader6.Text = "% of Parent";
			this.containerListViewColumnHeader6.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader6.Width = 70;
			// 
			// splitter4
			// 
			this.splitter4.Location = new System.Drawing.Point(200, 24);
			this.splitter4.Name = "splitter4";
			this.splitter4.Size = new System.Drawing.Size(3, 605);
			this.splitter4.TabIndex = 9;
			this.splitter4.TabStop = false;
			// 
			// splitter5
			// 
			this.splitter5.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter5.Location = new System.Drawing.Point(0, 150);
			this.splitter5.Name = "splitter5";
			this.splitter5.Size = new System.Drawing.Size(200, 3);
			this.splitter5.TabIndex = 15;
			this.splitter5.TabStop = false;
			// 
			// treeView1
			// 
			this.treeView1.CheckBoxes = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.FullRowSelect = true;
			this.treeView1.Location = new System.Drawing.Point(0, 150);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(200, 479);
			this.treeView1.TabIndex = 8;
			// 
			// timer1
			// 
			this.timer1.Interval = 300;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(88, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(616, 24);
			this.textBox1.TabIndex = 13;
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.textBox1);
			this.panel5.Controls.Add(this.label1);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(200, 0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(720, 24);
			this.panel5.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 24);
			this.label1.TabIndex = 14;
			this.label1.Text = "Filter signatures:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.splitter5);
			this.panel6.Controls.Add(this.treeView1);
			this.panel6.Controls.Add(this.processTree1);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel6.Location = new System.Drawing.Point(0, 0);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(200, 629);
			this.panel6.TabIndex = 15;
			// 
			// containerListViewColumnHeader7
			// 
			this.containerListViewColumnHeader7.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader7.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader7.DisplayIndex = 2;
			this.containerListViewColumnHeader7.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader7.Text = "# of Calls";
			this.containerListViewColumnHeader7.ToolTip = "# of Calls Tool Tip";
			this.containerListViewColumnHeader7.Width = 70;
			// 
			// containerListViewColumnHeader8
			// 
			this.containerListViewColumnHeader8.DisplayIndex = 1;
			this.containerListViewColumnHeader8.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader8.Text = "Signature";
			this.containerListViewColumnHeader8.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader8.Width = 350;
			// 
			// containerListViewColumnHeader9
			// 
			this.containerListViewColumnHeader9.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader9.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader9.DisplayIndex = 4;
			this.containerListViewColumnHeader9.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader9.Text = "% in\nChildren";
			this.containerListViewColumnHeader9.ToolTip = "% in Children Tool Tip";
			this.containerListViewColumnHeader9.Width = 70;
			// 
			// containerListViewColumnHeader10
			// 
			this.containerListViewColumnHeader10.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader10.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader10.DisplayIndex = 3;
			this.containerListViewColumnHeader10.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader10.Text = "% of Total";
			this.containerListViewColumnHeader10.ToolTip = "% of Total Tool Tip";
			this.containerListViewColumnHeader10.Width = 70;
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.containerListView2);
			this.panel7.Controls.Add(this.splitter6);
			this.panel7.Controls.Add(this.tabControl1);
			this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel7.Location = new System.Drawing.Point(203, 24);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(717, 605);
			this.panel7.TabIndex = 13;
			// 
			// containerListView2
			// 
			this.containerListView2.AllowColumnReorder = true;
			this.containerListView2.AllowMultiSelect = true;
			this.containerListView2.CaptureFocusClick = false;
			this.containerListView2.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader11,
            this.containerListViewColumnHeader8,
            this.containerListViewColumnHeader7,
            this.containerListViewColumnHeader10,
            this.containerListViewColumnHeader9,
            this.containerListViewColumnHeader12});
			this.containerListView2.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.containerListView2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerListView2.HeaderHeight = 33;
			this.containerListView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.containerListView2.HideSelection = false;
			this.containerListView2.Location = new System.Drawing.Point(0, 0);
			this.containerListView2.MultipleColumnSort = true;
			this.containerListView2.Name = "containerListView2";
			this.containerListView2.Size = new System.Drawing.Size(717, 426);
			this.containerListView2.TabIndex = 24;
			// 
			// containerListViewColumnHeader11
			// 
			this.containerListViewColumnHeader11.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader11.Text = "ID";
			this.containerListViewColumnHeader11.ToolTip = "ID Tool Tip";
			this.containerListViewColumnHeader11.Width = 100;
			// 
			// containerListViewColumnHeader12
			// 
			this.containerListViewColumnHeader12.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader12.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader12.DisplayIndex = 5;
			this.containerListViewColumnHeader12.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader12.Text = "% Suspended";
			this.containerListViewColumnHeader12.ToolTip = "% Suspended Tool Tip";
			this.containerListViewColumnHeader12.Width = 70;
			// 
			// splitter6
			// 
			this.splitter6.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter6.Location = new System.Drawing.Point(0, 426);
			this.splitter6.Name = "splitter6";
			this.splitter6.Size = new System.Drawing.Size(717, 3);
			this.splitter6.TabIndex = 23;
			this.splitter6.TabStop = false;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tabControl1.Location = new System.Drawing.Point(0, 429);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(717, 176);
			this.tabControl1.TabIndex = 22;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.containerListView3);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(709, 150);
			this.tabPage2.TabIndex = 0;
			this.tabPage2.Text = "Callees";
			// 
			// containerListView3
			// 
			this.containerListView3.AllowColumnReorder = true;
			this.containerListView3.CaptureFocusClick = false;
			this.containerListView3.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader13,
            this.containerListViewColumnHeader14,
            this.containerListViewColumnHeader15,
            this.containerListViewColumnHeader16,
            this.containerListViewColumnHeader6});
			this.containerListView3.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.containerListView3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerListView3.HeaderHeight = 33;
			this.containerListView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.containerListView3.HideSelection = false;
			this.containerListView3.Location = new System.Drawing.Point(0, 0);
			this.containerListView3.Name = "containerListView3";
			this.containerListView3.Size = new System.Drawing.Size(709, 150);
			this.containerListView3.TabIndex = 17;
			// 
			// containerListViewColumnHeader13
			// 
			this.containerListViewColumnHeader13.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader13.Text = "ID";
			this.containerListViewColumnHeader13.ToolTip = "ID Tool Tip";
			this.containerListViewColumnHeader13.Width = 100;
			// 
			// containerListViewColumnHeader14
			// 
			this.containerListViewColumnHeader14.DisplayIndex = 1;
			this.containerListViewColumnHeader14.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader14.Text = "Signature";
			this.containerListViewColumnHeader14.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader14.Width = 400;
			// 
			// containerListViewColumnHeader15
			// 
			this.containerListViewColumnHeader15.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader15.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader15.DisplayIndex = 2;
			this.containerListViewColumnHeader15.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader15.Text = "# of Calls";
			this.containerListViewColumnHeader15.ToolTip = "# of Calls Tool Tip";
			this.containerListViewColumnHeader15.Width = 70;
			// 
			// containerListViewColumnHeader16
			// 
			this.containerListViewColumnHeader16.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader16.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader16.DisplayIndex = 3;
			this.containerListViewColumnHeader16.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader16.Text = "% of Total";
			this.containerListViewColumnHeader16.ToolTip = "% of Total Tool Tip";
			this.containerListViewColumnHeader16.Width = 70;
			// 
			// panel8
			// 
			this.panel8.Controls.Add(this.panel7);
			this.panel8.Controls.Add(this.splitter4);
			this.panel8.Controls.Add(this.panel5);
			this.panel8.Controls.Add(this.panel6);
			this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel8.Location = new System.Drawing.Point(0, 0);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(920, 629);
			this.panel8.TabIndex = 11;
			// 
			// _ptProcessTree
			// 
			this._ptProcessTree.Dock = System.Windows.Forms.DockStyle.Top;
			this._ptProcessTree.Location = new System.Drawing.Point(0, 0);
			this._ptProcessTree.Name = "_ptProcessTree";
			this._ptProcessTree.Processes = null;
			this._ptProcessTree.Size = new System.Drawing.Size(200, 150);
			this._ptProcessTree.TabIndex = 14;
			// 
			// processTree1
			// 
			this.processTree1.Dock = System.Windows.Forms.DockStyle.Top;
			this.processTree1.Location = new System.Drawing.Point(0, 0);
			this.processTree1.Name = "processTree1";
			this.processTree1.Processes = null;
			this.processTree1.Size = new System.Drawing.Size(200, 150);
			this.processTree1.TabIndex = 14;
			// 
			// ProfilerForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(920, 677);
			this.Controls.Add(this.commandBarManager1);
			this.Controls.Add(this._menuMain);
			this.Controls.Add(this._sbStatusBar);
			this.Name = "ProfilerForm";
			this.Text = "nprof Profiling Application - Alpha v0.6";
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ProfilerForm_Layout);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ProfilerForm_Closing);
			this.Load += new System.EventHandler(this.ProfilerForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._sbpMessage)).EndInit();
			this._tcCallers.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this._tcCalls.ResumeLayout(false);
			this._tpCallees.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public ProjectInfo InitialProject
		{
			get { return _piInitialProject; }
			set { _piInitialProject = value; }
		}

		public void EnableAndStart()
		{
			if ( _piVSNetProject == null )
			{
				_piVSNetProject = new ProjectInfo( ProjectType.VSNet );
				_piVSNetProject.Name = "VS.NET Project";
				//_pic.Add( _piVSNetProject );
			}

			// Run the project
			//_pt.SelectProject( _piVSNetProject );
			_cmdProjectRun_Click( null, null );
		}

		public void Disable()
		{
			_p.Disable();
		}

		private void OnError( Exception e )
		{
			MessageBox.Show( this, "An internal exception occurred:\n\n" + e.ToString(), "Error" );
		}

		private void OnProfileStateChange( Run run )
		{
			this.BeginInvoke( new HandleProfileComplete( OnUIThreadProfileComplete ), new object[] { run } );
		}

		private delegate void HandleProfileComplete( Run run );

		private void OnUIThreadProfileComplete( Run run )
		{
			CreateTabPage( run );
		}

		private void ProfilerForm_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			CheckAddBlankTab();
		}

		private void OnRunStateChanged( Run run, Run.RunState rsOld, Run.RunState rsNew )
		{
			if ( rsNew == Run.RunState.Running )
			{
				OnProfileStateChange( run );
			}

			if ( rsNew == Run.RunState.Finished )
			{
				OnProfileStateChange( run );
			}
		}

		private void ProfilerForm_Load(object sender, System.EventArgs e)
		{
			this.Icon = new Icon( this.GetType().Assembly.GetManifestResourceStream( "NProf.GUI.Resources.app-icon.ico" ) );

			//CommandBarItem item;
			//commandBar2 = new CommandBar();
			//commandBar2.Style = CommandBarStyle.ToolBar;

			////item = new CommandBarButton(Images.New, "New", new EventHandler(_cmdFileNew_Click));
			////_cmdFileNew.Image = Images.New;
			////commandBar2.Items.Add(item);

			//item = new CommandBarButton(Images.Open, "New", new EventHandler(_cmdFileNew_Click));
			////item = new CommandBarButton(Images.Open, "Open", new EventHandler(_cmdFileOpen_Click));
			//_cmdFileOpen.Image = Images.New;
			//commandBar2.Items.Add(item);

			//item = new CommandBarButton( Images.Save, "Save", new EventHandler( _cmdFileSave_Click ) );
			//_cmdFileSave.Image = Images.Save;
			//commandBar2.Items.Add( item );




			//commandBar2.Items.AddSeparator();
			
			//item = new CommandBarButton( Images.Back, "Back", new EventHandler( _cmdViewNavBack_Click ) );
			//_cmdViewNavBack.Image = Images.Back;
			//commandBar2.Items.Add( item );
			
			//item = new CommandBarButton( Images.Forward, "Forward", new EventHandler( _cmdViewNavForward_Click ) );
			//_cmdViewNavForward.Image = Images.Forward;
			//commandBar2.Items.Add( item );

			//commandBar2.Items.AddSeparator();

			//commandBar2.Items.Add(new CommandBarButton(Images.Run, "Run", delegate { _cmdProjectRun_Click(null, null); }));






			//commandBarManager1.CommandBars.Add( commandBar2 );

			//commandBarManager1.Dock = DockStyle.Top;

			//_dock = new Crownwood.Magic.Docking.DockingManager( this, Crownwood.Magic.Common.VisualStyle.IDE );
			//_dock.OuterControl = commandBarManager1;
			//_dock.InnerControl = _tcProfilers;


			this.Text = "nprof Profiling Application - v" + Profiler.Version;
			//_tcProfilers.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;

			SerializationHandler.DataStoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NProf\\" + Profiler.Version;

			//_cmdFileRecentProjects.MenuCommands.Clear();

			int count = 1;
			//foreach(UsedFile usedFile in SerializationHandler.ProjectsHistory)
			//{
			//    if(count > 10)
			//        break;

			//    string label = (count == 10 ? "1&0" : "&" + count.ToString()) + " " + usedFile.ProjectName;

			//    Crownwood.Magic.Menus.MenuCommand pInfo = new Crownwood.Magic.Menus.MenuCommand(label);
			//    pInfo.Tag = usedFile.FileName;
			//    pInfo.Click += new EventHandler(_cmdFileRecentlyUsed_Click);
			//    pInfo.Shortcut = Shortcut.CtrlShift1 + (count == 10 ? -1 : count - 1);
			//    _cmdFileRecentProjects.MenuCommands.Add(pInfo);

			//    ++count;
			//}

			//if( count == 1 )
			//    _cmdFileRecentProjects.Enabled = false;

			if ( _piInitialProject != null )
			{
				project=_piInitialProject;
				//_pic.Add(_piInitialProject);
				//_pt.SelectProject( _piInitialProject );
				_cmdProjectRun_Click( null, null );
			}
			if (project == null)
			{
				//new ProfilerProjectOptionsForm().ShowDialog();
				//new Options().Show();
				//new Options().Show();
			}
		}

		//private void _tcProfilers_ClosePressed(object sender, System.EventArgs e)
		//{
		//    //if ( IsShowingBlankTab() )
		//    //    return;

		//    //_tcProfilers.TabPages.Remove( _tcProfilers.SelectedTab );
		//    CheckAddBlankTab();
		//}

		private void ProfilerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_p.Stop();
		}



		private void _cmdFileNew_Click(object sender, System.EventArgs e)
		{
			ProfilerProjectOptionsForm frm = new ProfilerProjectOptionsForm();
			frm.Mode = ProfilerProjectOptionsForm.ProfilerProjectMode.CreateProject;
			DialogResult dr = frm.ShowDialog( this );
			if ( dr == DialogResult.OK )
			{
				project=frm.Project;
				//_pic.Add(frm.Project);

				//foreach ( Run run in project.Runs )
				//{
				//    //_tvProjects.Invoke(new TreeNodeAdd(OnTreeNodeAdd), new object[] { tnProject.Nodes, run.StartTime.ToString(), GetRunStateImage(run), run });
				//    run.StateChanged += new Run.RunStateEventHandler(OnRunStateChanged);
				//}


				//_pt.SelectProject( frm.Project );
			}
		}

		private void _cmdFileOpen_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openDlg = new OpenFileDialog();

			openDlg.DefaultExt = "nprof";
			openDlg.Filter = "NProf projects (*.nprof)|*.nprof|All files (*.*)|*.*";
			// openDlg.InitialDirectory = TODO: store the most recently used directory somewhere and go there
			openDlg.Multiselect = true;
			openDlg.Title = "Open a saved NProf project file";

			if( openDlg.ShowDialog( this ) == DialogResult.OK )
			{
				foreach( string fileName in openDlg.FileNames )
				{
					ProjectInfo project = SerializationHandler.OpenProjectInfo( fileName );
					this.project = project;
					//_pic.Add( project );
					//_pt.SelectProject( project );
				}
			}
		}

		private void _cmdFileClose_Click(object sender, System.EventArgs e)
		{
			//_pic.Remove( GetCurrentProject() );
		}

		private void _cmdFileSave_Click(object sender, System.EventArgs e)
		{
			SaveProject( GetCurrentProject(), false );
		}

		private void _cmdFileSaveAs_Click(object sender, System.EventArgs e)
		{
			SaveProject( GetCurrentProject(), true );
		}

		private void _cmdFileSaveAll_Click(object sender, System.EventArgs e)
		{
			//foreach( ProjectInfo project in _pic )
				SaveProject( project, false );
		}

		private void _cmdFileExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void _cmdFileRecentlyUsed_Click(object sender, EventArgs e)
		{
			Crownwood.Magic.Menus.MenuCommand pInfo = (Crownwood.Magic.Menus.MenuCommand)sender;
			string fileName = (string)pInfo.Tag;

			ProjectInfo project = SerializationHandler.OpenProjectInfo( (string)pInfo.Tag );

			if( project != null )
			{
				this.project = project;
				//_pic.Add( project );
				//_pt.SelectProject( project );
			}
		}

		private void _cmdProjectRun_Click(object sender, System.EventArgs e)
		{
			string strMessage;
			bool bSuccess = _p.CheckSetup( out strMessage );
			if ( !bSuccess )
			{
				MessageBox.Show( this, strMessage, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}

			Run run = GetCurrentProject().CreateRun( _p );
			run.StateChanged += new Run.RunStateEventHandler( OnRunStateChanged );
			run.Start();

			//CreateTabPage( run );

			profilerControl._ptProcessTree.AddRunNode(run);


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

		private void _cmdViewProjectView_Click(object sender, System.EventArgs e)
		{
			//if ( !_cmdViewProjectView.Checked )
			//{
			//    _dock.ShowContent( _dock.Contents[ "Projects" ] );
			//    _cmdViewProjectView.Checked = true;
			//}
			//else
			//{
			//    _dock.HideContent( _dock.Contents[ "Projects" ] );
			//    _cmdViewProjectView.Checked = false;
			//}
		}

		private void _cmdViewProjectView_Update(object sender, System.EventArgs e)
		{
			//_cmdViewProjectView.Checked = _dock.Contents[ "Projects" ].Visible;
		}

		private void _cmdProjectOptions_Click(object sender, System.EventArgs e)
		{
			ProfilerProjectOptionsForm frm = new ProfilerProjectOptionsForm();
			frm.Project = GetCurrentProject();
			frm.Mode = ProfilerProjectOptionsForm.ProfilerProjectMode.ModifyProject;

			frm.ShowDialog( this );
		}

		private void _cmdProjectRunViewMessages_Click(object sender, System.EventArgs e)
		{
			return;
			//Run r = (Run)_tcProfilers.SelectedTab.Tag;
			//ProfilerRunMessagesForm frm = new ProfilerRunMessagesForm();
			//frm.ProfilerRun = r;
			//frm.ShowDialog(this);
			//Run r = ( Run )_tcProfilers.SelectedTab.Tag;
			//ProfilerRunMessagesForm frm = new ProfilerRunMessagesForm();
			//frm.ProfilerRun = r;
			//frm.ShowDialog( this );
		}

		private void _cmdViewNavBack_Click(object sender, System.EventArgs e)
		{
			//Crownwood.Magic.Controls.TabPage tpActive = _tcProfilers.SelectedTab;
			//if (tpActive == null)
			//    return;

			//if (tpActive.Controls.Count == 0)// || !(tpActive.Controls[0] is ProfilerControl)) 
			//    return;

			NavigateBackward();
			//((ProfilerControl) tpActive.Controls[0]).NavigateBackward();
		}

		private void _cmdViewNavForward_Click(object sender, System.EventArgs e)
		{

			//Crownwood.Magic.Controls.TabPage tpActive = _tcProfilers.SelectedTab;
			////Crownwood.Magic.Controls.TabPage tpActive = _tcProfilers.SelectedTab;

			//if (tpActive == null)
			//    return;

			//if (tpActive.Controls.Count == 0)// || !(tpActive.Controls[0] is ProfilerControl))
			//    //if (tpActive.Controls.Count == 0 || !(tpActive.Controls[0] is ProfilerControl))
			//        return;

			NavigateForward();
			//((ProfilerControl)tpActive.Controls[0]).NavigateForward();
			//((ProfilerControl)tpActive.Controls[0]).NavigateForward();

		}

		

		private void _cmdHelpAbout_Click(object sender, System.EventArgs e)
		{
			AboutForm frm = new AboutForm();
			frm.ShowDialog( this );
		}




		//private void _pic_ProjectRemoved(ProjectInfoCollection projects, ProjectInfo project, int nIndex)
		//{
		//    foreach ( Run run in project.Runs )
		//    {
		//        foreach (Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages)
		//        //foreach (Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages)
		//        {
		//            if ( tp.Tag == run )
		//            {
		//                _tcProfilers.TabPages.Remove ( tp );
		//                break;
		//            }
		//        }
		//    }

		//    CheckAddBlankTab();
		//}


		private void _pt_ProjectDoubleClicked( ProjectInfo pi )
		{
			_cmdProjectOptions_Click( null, null );
		}

		private void _pt_RunDoubleClicked( Run run )
		{
			if ( run.State != Run.RunState.Finished )
			{
				MessageBox.Show( this, "The run has not completed", "Project Run in Progress", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}

			//CreateTabPage( run );
		}

		private void _pt_ExecutableDropped( string[] fileNames )
		{
			foreach ( string fileName in fileNames )
			{
				ProfilerProjectOptionsForm frm = new ProfilerProjectOptionsForm();
				frm.Mode = ProfilerProjectOptionsForm.ProfilerProjectMode.CreateProject;

				// prepoulate from drag operation
				frm.Project.ApplicationName = fileName;
				frm.Project.WorkingDirectory = Path.GetDirectoryName(fileName);

				DialogResult dr = frm.ShowDialog( this );
				if ( dr == DialogResult.OK )
				{
					this.project = frm.Project;

					//_pic.Add( frm.Project );
					//_pt.SelectProject( frm.Project );
				}
			}
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

		private ProjectInfo GetCurrentProject()
		{
			return project;
		}

		private void CheckAddBlankTab()
		{
			return;
			//if ( _tcProfilers.TabPages.Count == 0 )
			//{
			//    Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage( "No profiler runs loaded" );
			//    _tcProfilers.TabPages.Add( tp );
			//    NoProfilerRunsLoaded np = new NoProfilerRunsLoaded();
			//    tp.Controls.Add( np );
			//    np.Dock = DockStyle.Fill;
			//}
		}

		//private void CheckRemoveBlankTab()
		//{
		//    if ( IsShowingBlankTab() )
		//        _tcProfilers.TabPages.Clear();
		//}

		//private bool IsShowingBlankTab()
		//{
		//    return ( _tcProfilers.TabPages[ 0 ].Title == "No profiler runs loaded" );
		//}

		private void CreateTabPage(Run run)
		//private Crownwood.Magic.Controls.TabPage CreateTabPage(Run run)
		{
			//CheckRemoveBlankTab();

			//Crownwood.Magic.Controls.TabPage tpActive = null;

			// Does it exist already?
			//foreach ( Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages )
			//{
			//    if ( tp.Tag == run )
			//    {
			//        _tcProfilers.SelectedTab = tp;
			//        tpActive = tp;
			//        break;
			//    }
			//}

			//if ( tpActive == null )
			//{
			//    tpActive = new Crownwood.Magic.Controls.TabPage( run.Project.Name + " [" + run.StartTime + "]" );
			//    tpActive.Tag = run;
			//    _tcProfilers.TabPages.Add( tpActive );
			//    _tcProfilers.SelectedTab = tpActive;
			//}

			if (run.State == Run.RunState.Finished
				&& run.Success)
				//&& (tpActive.Controls.Count == 0))// || !( tpActive.Controls[ 0 ] is ProfilerControl ) ) )
			{
				//tpActive.Controls.Clear();
				//ProfilerControl pc = new ProfilerControl();
				//pc.Dock = DockStyle.Fill;
				//tpActive.Controls.Add( pc );
				profilerControl.ProfilerRun = run;
			}
			ProfilerRun = run;


			//// Catch non-successful finished runs here too
			//if (((run.State == Run.RunState.Finished && !run.Success)
			//    || run.State == Run.RunState.Running
			//    || run.State == Run.RunState.Initializing))
			//    //&& (tpActive.Controls.Count == 0))// || !( tpActive.Controls[ 0 ] is ProfilerRunControl ) ) )
			//{
			//    //tpActive.Controls.Clear();
			//    //ProfilerRunControl pc = new ProfilerRunControl();
			//    //pc.Dock = DockStyle.Fill;
			//    //tpActive.Controls.Add( pc );
			//    //pc.ProfilerRun = run;
			//    ProfilerRun = run;
			//}

			//return tpActive;
		}

		//private Crownwood.Magic.Controls.TabPage CreateTabPage(Run run)
		//{
		//    //CheckRemoveBlankTab();

		//    Crownwood.Magic.Controls.TabPage tpActive = null;

		//    // Does it exist already?
		//    //foreach ( Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages )
		//    //{
		//    //    if ( tp.Tag == run )
		//    //    {
		//    //        _tcProfilers.SelectedTab = tp;
		//    //        tpActive = tp;
		//    //        break;
		//    //    }
		//    //}

		//    //if ( tpActive == null )
		//    //{
		//    //    tpActive = new Crownwood.Magic.Controls.TabPage( run.Project.Name + " [" + run.StartTime + "]" );
		//    //    tpActive.Tag = run;
		//    //    _tcProfilers.TabPages.Add( tpActive );
		//    //    _tcProfilers.SelectedTab = tpActive;
		//    //}

		//    if (run.State == Run.RunState.Finished
		//        && run.Success
		//        && (tpActive.Controls.Count == 0))// || !( tpActive.Controls[ 0 ] is ProfilerControl ) ) )
		//    {
		//        tpActive.Controls.Clear();
		//        //ProfilerControl pc = new ProfilerControl();
		//        //pc.Dock = DockStyle.Fill;
		//        //tpActive.Controls.Add( pc );
		//        //pc.ProfilerRun = run;
		//    }
		//    ProfilerRun = run;


		//    // Catch non-successful finished runs here too
		//    if (((run.State == Run.RunState.Finished && !run.Success)
		//        || run.State == Run.RunState.Running
		//        || run.State == Run.RunState.Initializing)
		//        && (tpActive.Controls.Count == 0))// || !( tpActive.Controls[ 0 ] is ProfilerRunControl ) ) )
		//    {
		//        tpActive.Controls.Clear();
		//        //ProfilerRunControl pc = new ProfilerRunControl();
		//        //pc.Dock = DockStyle.Fill;
		//        //tpActive.Controls.Add( pc );
		//        //pc.ProfilerRun = run;
		//        ProfilerRun = run;
		//    }

		//    return tpActive;
		//}

		private void UpdateMenuItems(object sender, System.EventArgs e)
		{
			//bool bCanRunOrEdit = _pt.GetSelectedProject() != null
			//    && _pt.GetSelectedProject().ProjectType != ProjectType.VSNet;
			bool bCanRunOrEdit = true;

			Run run = null;
			//Run run = _pt.GetSelectedRun();

			_cmdProjectRun.Enabled = bCanRunOrEdit;
			_cmdProjectStop.Enabled = bCanRunOrEdit && ( run != null && run.State == Run.RunState.Running );
			_cmdProjectOptions.Enabled = bCanRunOrEdit;
			//_cmdProjectRunViewMessages.Enabled = ( !IsShowingBlankTab() );
			//_cmdProjectRunCopy.Enabled = ( !IsShowingBlankTab() );

			//_cmdFileClose.Enabled = bCanRunOrEdit;
			_cmdFileSave.Enabled = bCanRunOrEdit;
			_cmdFileSaveAs.Enabled = bCanRunOrEdit;
			//_cmdFileSaveAll.Enabled = ( !IsShowingBlankTab() );

			//_cmdViewNavBack.Enabled = ( !IsShowingBlankTab() );
			//_cmdViewNavForward.Enabled = ( !IsShowingBlankTab() );
		}

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
			//public static Image Cut               { get { return images[3];  } }
			//public static Image Copy              { get { return images[4];  } }
			//public static Image Paste             { get { return images[5];  } }
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
			public static Image Run { get { return images[37]; } }
		}
		public Run ProfilerRun
		{
			set 
			{ 
				_ptProcessTree.Processes = value.Processes;
				RefreshData(); 
			}
		}

		private void RefreshData()
		{
			this.BeginInvoke( new EventHandler( UpdateOnUIThread ), new object[] { null, null } );
		}

		private void UpdateOnUIThread( object oSender, EventArgs ea )
		{
			//UpdateTree();
			//UpdateFilters();
		}

		private void UpdateTree()
		{
			_tvNamespaceInfo.Nodes.Clear();

			_bUpdating = true;
			_htCheckedItems = new Hashtable();

			try
			{
				_tvNamespaceInfo.BeginUpdate();

				TreeNode tnRoot = _tvNamespaceInfo.Nodes.Add( "All Namespaces" );
				tnRoot.Tag = "";
				tnRoot.Checked = true;

				ThreadInfo tiCurrentThread = _tiCurrent;
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
				{
					TreeNodeCollection tnc = tnRoot.Nodes;
					ArrayList alNamespace = new ArrayList();
					foreach ( string strNamespacePiece in fi.Signature.Namespace )
					{
						alNamespace.Add( strNamespacePiece );
						TreeNode tnFound = null;

						foreach ( TreeNode tn in tnc )
						{
							if ( tn.Text == strNamespacePiece )
							{
								tnFound = tn;
								break;
							}
						}

						if ( tnFound == null )
						{
							tnFound = tnc.Add( strNamespacePiece );
							tnFound.Tag = String.Join( ".", ( string[] )alNamespace.ToArray( typeof( string ) ) );
							tnFound.Checked = true;
						}

						tnc = tnFound.Nodes;
					}
				}

				tnRoot.Expand();
			}
			finally
			{
				_bUpdating = false;
				_tvNamespaceInfo.EndUpdate();
			}
		}

		private void UpdateFilters()
		{
			_lvFunctionInfo.Items.Clear();
			_lvCalleesInfo.Items.Clear();
			_lvCallersInfo.Items.Clear();

			Hashtable htNamespaces = new Hashtable();
			foreach ( TreeNode tn in _htCheckedItems.Keys )
				htNamespaces[ tn.Tag ] = null;

			try
			{
				_lvFunctionInfo.BeginUpdate();

				ThreadInfo tiCurrentThread = _tiCurrent;
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
				{
					if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
						continue;

					ContainerListViewItem lvi = _lvFunctionInfo.Items.Add( fi.ID.ToString() );
					lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					lvi.SubItems[ 2 ].Text = fi.Calls.ToString();
					lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 4 ].Text = fi.PercentOfTotalTimeInChildren.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 5 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
					lvi.Tag = fi;
				}
			}
			finally
			{
				_lvFunctionInfo.Sort();
				_lvFunctionInfo.EndUpdate();
			}
		}

		private void _tvNamespaceInfo_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			bool bInCheck = _bInCheck;

			if ( !bInCheck )
				_bInCheck = true;

			try
			{
				if ( e.Node.Checked )
					_htCheckedItems[ e.Node ] = null;
				else
					_htCheckedItems.Remove( e.Node );
				if ( _bUpdating )
					return;

				foreach ( TreeNode tnChild in e.Node.Nodes )
					tnChild.Checked = e.Node.Checked;

				if ( bInCheck )
					return;
			}
			finally
			{
				if ( !bInCheck )
					_bInCheck = false;
			}

			UpdateFilters();
		}

		private void _lvFunctionInfo_SelectedItemsChanged(object sender, System.EventArgs e)
		{
			_lvCalleesInfo.Items.Clear();
			_lvCallersInfo.Items.Clear();

			if ( _lvFunctionInfo.SelectedItems.Count == 0 )
				return;

			// somebody clicked! empty the forward stack and push this click on the "Back" stack.
			if ( !_bNavigating )
			{
				_navStackForward.Clear();
				if ( _navCurrent != null )
					_navStackBack.Push( _navCurrent );

				ArrayList lst = new ArrayList();

				for( int idx = 0; idx < _lvFunctionInfo.SelectedItems.Count; ++idx )
					if( _lvFunctionInfo.SelectedItems[idx].Tag != null )
						lst.Add( ( _lvFunctionInfo.SelectedItems[idx].Tag as FunctionInfo ).ID );

				_navCurrent = ( int[] )lst.ToArray( typeof( int ) );
			}

			UpdateCalleeList();
			UpdateCallerList();
		}

		private void UpdateCallerList()
		{
			_lvCallersInfo.BeginUpdate();

			bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
			_lvCallersInfo.ShowPlusMinus = multipleSelected;
			_lvCallersInfo.ShowRootTreeLines = multipleSelected;
			_lvCallersInfo.ShowTreeLines = multipleSelected;

			ThreadInfo tiCurrentThread = _tiCurrent;
			foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
			{
				FunctionInfo mfi = ( FunctionInfo )item.Tag;

				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
					foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
						if ( cfi.ID == mfi.ID )
						{
							ContainerListViewItem parentItem = null;

							foreach ( ContainerListViewItem pitem in _lvCallersInfo.Items )
								if ( ( pitem.Tag as FunctionInfo ).ID == fi.ID )
									parentItem = pitem;

							if ( parentItem == null ) // don't have it
							{
								parentItem = _lvCallersInfo.Items.Add( fi.ID.ToString() );
								parentItem.SubItems[ 1 ].Text = fi.Signature.Signature;
								parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
								parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
								parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
								parentItem.Tag = fi;
							}
							else // do, update totals
							{
								parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
								parentItem.SubItems[ 3 ].Text = "-";
								parentItem.SubItems[ 4 ].Text = "-";
							}

							// either way, add a child pointing back to the parent
							ContainerListViewItem lvi = parentItem.Items.Add( cfi.ID.ToString() );
							lvi.SubItems[ 1 ].Text = cfi.Signature;
							lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
							lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
							lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
							lvi.Tag = cfi;
						}
			}

			_lvCallersInfo.Sort();
			_lvCallersInfo.EndUpdate();
		}

		private void UpdateCalleeList()
		{
			_lvCalleesInfo.BeginUpdate();

			bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
			_lvCalleesInfo.ShowPlusMinus = multipleSelected;
			_lvCalleesInfo.ShowRootTreeLines = multipleSelected;
			_lvCalleesInfo.ShowTreeLines = multipleSelected;

			ContainerListViewItem lviSuspend = null;

			foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
			{
				FunctionInfo fi = ( FunctionInfo )item.Tag;

				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					ContainerListViewItem parentItem = null;

					foreach ( ContainerListViewItem pitem in _lvCalleesInfo.Items )
						if( pitem.Tag != null)
							if ( ( pitem.Tag as CalleeFunctionInfo ).ID == cfi.ID )
								parentItem = pitem;


					if ( parentItem == null ) // don't have it
					{
						parentItem = _lvCalleesInfo.Items.Add( cfi.ID.ToString() );
						parentItem.SubItems[ 1 ].Text = cfi.Signature;
						parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
						parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
						parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
						parentItem.Tag = cfi;
					}
					else // do, update totals
					{
						parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
						parentItem.SubItems[ 3 ].Text = "-";
						parentItem.SubItems[ 4 ].Text = "-";
					}

					// either way, add a child pointing back to the parent
					ContainerListViewItem lvi = parentItem.Items.Add( fi.ID.ToString() );
					lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
					lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.Tag = fi;
				}

				ContainerListViewItem inMethod = _lvCalleesInfo.Items.Add("(in method)");
				inMethod.SubItems[1].Text = "(in method)";
				inMethod.SubItems[2].Text = fi.Calls.ToString();
				inMethod.SubItems[3].Text = fi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
				inMethod.SubItems[4].Text = fi.PercentOfMethodTimeInMethod.ToString("0.00;-0.00;0");
				inMethod.Tag = fi;



				if ( fi.TotalSuspendedTicks > 0 )
				{
					if ( lviSuspend == null) // don't have it
					{
						lviSuspend = _lvCalleesInfo.Items.Add( "(suspend)" );
						lviSuspend.SubItems[ 1 ].Text = "(thread suspended)";
						lviSuspend.SubItems[ 2 ].Text = "-";
						lviSuspend.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
						lviSuspend.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
					}
					else // do, update totals
					{
						lviSuspend.SubItems[ 3 ].Text = "-";
						lviSuspend.SubItems[ 4 ].Text = "-";
					}

					// either way, add a child pointing back to the parent
					ContainerListViewItem lvi = lviSuspend.Items.Add( fi.ID.ToString() );
					lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					lvi.SubItems[ 2 ].Text = "-";
					lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
				}
			}

			_lvCalleesInfo.Sort();
			_lvCalleesInfo.EndUpdate();
		}

		private void ProfilerControl_Load(object sender, System.EventArgs e)
		{
			_ptProcessTree.ThreadSelected += new ProcessTree.ThreadSelectedHandler( _ptProcessTree_ThreadSelected );
			_lvFunctionInfo.Sort(1, true, true);
			_lvCalleesInfo.Sort(1, true, true);
		}

		private void _ptProcessTree_ThreadSelected( ThreadInfo ti )
		{
			_tiCurrent = ti;
			RefreshData();
		}

		private void _lvChildInfo_DoubleClick(object sender, System.EventArgs e)
		{
			ContainerListView ctl = sender as ContainerListView;

			if ( ctl.SelectedItems.Count == 0 )
				return;

			if ( ctl.SelectedItems[ 0 ].Tag is CalleeFunctionInfo )
			{
				CalleeFunctionInfo cfi = ( CalleeFunctionInfo )ctl.SelectedItems[ 0 ].Tag;
				if ( cfi == null )
					MessageBox.Show( "No information available for this item" );
				else
					JumpToID( cfi.ID );
			}
			else if ( ctl.SelectedItems[ 0 ].Tag is FunctionInfo )
			{
				FunctionInfo fi = ( FunctionInfo )ctl.SelectedItems[ 0 ].Tag;
				if ( fi == null )
					MessageBox.Show( "No information available for this item" );
				else
					JumpToID( fi.ID );
			}
		}

		private void _txtFilterBar_TextChanged(object sender, System.EventArgs e)
		{
			_tmrFilterThrottle.Enabled = false;
			_tmrFilterThrottle.Enabled = true;
		}

		private void _tmrFilterThrottle_Tick(object sender, System.EventArgs e)
		{
			_tmrFilterThrottle.Enabled = false;
			string text = _txtFilterBar.Text.ToLower();
			foreach (ContainerListViewItem item in _lvFunctionInfo.Items)
			{
				if (text!="" && item.SubItems[1].Text.ToLower().IndexOf(text) != -1)
				{
					item.BackColor = Color.LightSteelBlue;
				}
				else
				{
					item.BackColor = _lvFunctionInfo.BackColor;
				}
			}
			_lvFunctionInfo.BeginUpdate();
			_lvFunctionInfo.Items.Sort(new SignatureComparer(text));
			_lvFunctionInfo.EndUpdate();
			_lvFunctionInfo.Invalidate();
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
				bool aFound=a.SubItems[1].Text.ToLower().IndexOf(text)!=-1;
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

		public void  NavigateBackward()
		{
			if (_navStackBack.Count == 0)
				return;

			_navStackForward.Push(_navCurrent);
			_navCurrent = ( int[] ) _navStackBack.Pop();

			_bNavigating = true;
			JumpToID( _navCurrent );
			_bNavigating = false;
		}

		public void NavigateForward()
		{
			if ( _navStackForward.Count == 0 )
				return;

			_navStackBack.Push( _navCurrent );
			_navCurrent = ( int[] ) _navStackForward.Pop();

			_bNavigating = true;
			JumpToID( _navCurrent );
			_bNavigating = false;
		}

		private void JumpToID( int nID )
		{
			JumpToID( new int[] { nID } );
		}

		private void JumpToID( int[] ids )
		{
			_lvFunctionInfo.SelectedItems.Clear();

			foreach( int id in ids )
				foreach( ContainerListViewItem lvi in _lvFunctionInfo.Items )
				{
					FunctionInfo fi = ( FunctionInfo )lvi.Tag;

					if ( fi.ID == id )
					{
						if(!lvi.Selected)
						{
							//if(lvi.IsFiltered)
							//{
							//    if(MessageBox.Show("Cannot navigate to that function because it is being filtered.  Would you like to remove the filter and continue to that function?", "Filtered Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
							//        return;
							//    else
							//        _txtFilterBar.Text = string.Empty;
							//}

							lvi.Selected = true;
							lvi.Focused = true;
						}
						break;
					}
				}

			_lvFunctionInfo.EnsureVisible();
			_lvFunctionInfo.Focus();
		}

		private int GetSelectedID()
		{
			if ( _lvCalleesInfo.SelectedItems.Count == 0 )
				return -1;

			return ( ( FunctionInfo )_lvFunctionInfo.SelectedItems[ 0 ].Tag ).ID;
		}

		private ThreadInfo _tiCurrent;
		private bool _bUpdating, _bInCheck;
		private Hashtable _htCheckedItems;
		private Stack _navStackBack = new Stack();
		private Stack _navStackForward = new Stack();
		private int[] _navCurrent = null;
		private bool _bNavigating = false;



		private void _lv_HeaderMenuEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ContainerListView listView = sender as ContainerListView;

			PopupMenu pop = new PopupMenu();
			//pop.Selected += new CommandHandler(pop_Selected);
			//pop.Deselected += new CommandHandler(pop_Deselected);

			MenuCommand sortBy = pop.MenuCommands.Add(new MenuCommand("Sort &By"));
			pop.MenuCommands.Add(new MenuCommand("-"));
			bool isAscending = true;
			for(int idx = 0; idx < listView.Columns.Count; ++idx)
			{
				ContainerListViewColumnHeader hdr = listView.Columns[idx];
				MenuCommand sortByItem = new MenuCommand(hdr.Text);

				sortByItem.Description = string.Format("Sort By the '{1}' column from this grid", (hdr.Visible ? "Shows" : "Hides"), hdr.Text);
				sortByItem.RadioCheck = true;
				sortByItem.Checked = hdr.SortOrder != SortOrder.None;
				sortByItem.Tag = new object[] { listView, idx };

				if(sortByItem.Checked)
					isAscending = hdr.SortOrder == SortOrder.Ascending;

				sortBy.MenuCommands.Add(sortByItem);
			}

			sortBy.MenuCommands.Add(new MenuCommand("-"));
			MenuCommand ascending = sortBy.MenuCommands.Add(new MenuCommand("&Ascending"));
			ascending.RadioCheck = true;
			ascending.Checked = isAscending;
			ascending.Tag = new object[] { listView, SortOrder.Ascending };

			MenuCommand descending = sortBy.MenuCommands.Add(new MenuCommand("&Descending"));
			descending.RadioCheck = true;
			descending.Checked = !isAscending;
			descending.Tag = new object[] { listView, SortOrder.Descending };

			bool allShown = true;
			for(int idx = 0; idx < listView.Columns.Count; ++idx)
			{
				ContainerListViewColumnHeader hdr = listView.Columns[idx];
				MenuCommand checkable = new MenuCommand(hdr.Text);

				checkable.Description = string.Format("{0} the '{1}' column from this grid", (hdr.Visible ? "Shows" : "Hides"), hdr.Text);
				checkable.Checked = hdr.Visible;
				checkable.Tag = hdr;

				pop.MenuCommands.Add(checkable);
				allShown &= hdr.Visible;
			}

			pop.MenuCommands.Add(new MenuCommand("-"));
			pop.MenuCommands.Add(new MenuCommand("Show &All")).Enabled = !allShown;

			MenuCommand result = pop.TrackPopup(listView.PointToScreen(new Point(e.X, e.Y)));
			if(result != null && result.Tag is ContainerListViewColumnHeader)
				(result.Tag as ContainerListViewColumnHeader).Visible = !result.Checked;
		}
	}
}


//        public Run ProfilerRun
//        {
//            set 
//            { 
//                _ptProcessTree.Processes = value.Processes;
//                RefreshData(); 
//            }
//        }

//        private void RefreshData()
//        {
//            this.BeginInvoke( new EventHandler( UpdateOnUIThread ), new object[] { null, null } );
//        }

//        private void UpdateOnUIThread( object oSender, EventArgs ea )
//        {
//            UpdateTree();
//            UpdateFilters();
//        }

//        private void UpdateTree()
//        {
//            _tvNamespaceInfo.Nodes.Clear();

//            _bUpdating = true;
//            _htCheckedItems = new Hashtable();

//            try
//            {
//                _tvNamespaceInfo.BeginUpdate();

//                TreeNode tnRoot = _tvNamespaceInfo.Nodes.Add( "All Namespaces" );
//                tnRoot.Tag = "";
//                tnRoot.Checked = true;

//                ThreadInfo tiCurrentThread = _tiCurrent;
//                foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
//                {
//                    TreeNodeCollection tnc = tnRoot.Nodes;
//                    ArrayList alNamespace = new ArrayList();
//                    foreach ( string strNamespacePiece in fi.Signature.Namespace )
//                    {
//                        alNamespace.Add( strNamespacePiece );
//                        TreeNode tnFound = null;

//                        foreach ( TreeNode tn in tnc )
//                        {
//                            if ( tn.Text == strNamespacePiece )
//                            {
//                                tnFound = tn;
//                                break;
//                            }
//                        }

//                        if ( tnFound == null )
//                        {
//                            tnFound = tnc.Add( strNamespacePiece );
//                            tnFound.Tag = String.Join( ".", ( string[] )alNamespace.ToArray( typeof( string ) ) );
//                            tnFound.Checked = true;
//                        }

//                        tnc = tnFound.Nodes;
//                    }
//                }

//                tnRoot.Expand();
//            }
//            finally
//            {
//                _bUpdating = false;
//                _tvNamespaceInfo.EndUpdate();
//            }
//        }

//        private void UpdateFilters()
//        {
//            _lvFunctionInfo.Items.Clear();
//            _lvCalleesInfo.Items.Clear();
//            _lvCallersInfo.Items.Clear();

//            Hashtable htNamespaces = new Hashtable();
//            foreach ( TreeNode tn in _htCheckedItems.Keys )
//                htNamespaces[ tn.Tag ] = null;

//            try
//            {
//                _lvFunctionInfo.BeginUpdate();

//                ThreadInfo tiCurrentThread = _tiCurrent;
//                foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
//                {
//                    if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
//                        continue;

//                    ContainerListViewItem lvi = _lvFunctionInfo.Items.Add( fi.ID.ToString() );
//                    lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
//                    lvi.SubItems[ 2 ].Text = fi.Calls.ToString();
//                    lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" );
//                    lvi.SubItems[ 4 ].Text = fi.PercentOfTotalTimeInChildren.ToString( "0.00;-0.00;0" );
//                    lvi.SubItems[ 5 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
//                    lvi.Tag = fi;
//                }
//            }
//            finally
//            {
//                _lvFunctionInfo.Sort();
//                _lvFunctionInfo.EndUpdate();
//            }
//        }


//        private void _tvNamespaceInfo_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
//        {
//            UpdateFilters();
//        }

//        private void _tvNamespaceInfo_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
//        {
//            bool bInCheck = _bInCheck;

//            if ( !bInCheck )
//                _bInCheck = true;

//            try
//            {
//                if ( e.Node.Checked )
//                    _htCheckedItems[ e.Node ] = null;
//                else
//                    _htCheckedItems.Remove( e.Node );
//                if ( _bUpdating )
//                    return;

//                foreach ( TreeNode tnChild in e.Node.Nodes )
//                    tnChild.Checked = e.Node.Checked;

//                if ( bInCheck )
//                    return;
//            }
//            finally
//            {
//                if ( !bInCheck )
//                    _bInCheck = false;
//            }

//            UpdateFilters();
//        }

//        private void _lvFunctionInfo_SelectedItemsChanged(object sender, System.EventArgs e)
//        {
//            _lvCalleesInfo.Items.Clear();
//            _lvCallersInfo.Items.Clear();

//            if ( _lvFunctionInfo.SelectedItems.Count == 0 )
//                return;

//            // somebody clicked! empty the forward stack and push this click on the "Back" stack.
//            if ( !_bNavigating )
//            {
//                _navStackForward.Clear();
//                if ( _navCurrent != null )
//                    _navStackBack.Push( _navCurrent );

//                ArrayList lst = new ArrayList();

//                for( int idx = 0; idx < _lvFunctionInfo.SelectedItems.Count; ++idx )
//                    if( _lvFunctionInfo.SelectedItems[idx].Tag != null )
//                        lst.Add( ( _lvFunctionInfo.SelectedItems[idx].Tag as FunctionInfo ).ID );

//                _navCurrent = ( int[] )lst.ToArray( typeof( int ) );
//            }

//            UpdateCalleeList();
//            UpdateCallerList();
//        }

//        private void UpdateCallerList()
//        {
//            _lvCallersInfo.BeginUpdate();

//            bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
//            _lvCallersInfo.ShowPlusMinus = multipleSelected;
//            _lvCallersInfo.ShowRootTreeLines = multipleSelected;
//            _lvCallersInfo.ShowTreeLines = multipleSelected;

//            ThreadInfo tiCurrentThread = _tiCurrent;
//            foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
//            {
//                FunctionInfo mfi = ( FunctionInfo )item.Tag;

//                foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
//                    foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
//                        if ( cfi.ID == mfi.ID )
//                        {
//                            ContainerListViewItem parentItem = null;

//                            foreach ( ContainerListViewItem pitem in _lvCallersInfo.Items )
//                                if ( ( pitem.Tag as FunctionInfo ).ID == fi.ID )
//                                    parentItem = pitem;

//                            if ( parentItem == null ) // don't have it
//                            {
//                                parentItem = _lvCallersInfo.Items.Add( fi.ID.ToString() );
//                                parentItem.SubItems[ 1 ].Text = fi.Signature.Signature;
//                                parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
//                                parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
//                                parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
//                                parentItem.Tag = fi;
//                            }
//                            else // do, update totals
//                            {
//                                parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
//                                parentItem.SubItems[ 3 ].Text = "-";
//                                parentItem.SubItems[ 4 ].Text = "-";
//                            }

//                            // either way, add a child pointing back to the parent
//                            ContainerListViewItem lvi = parentItem.Items.Add( cfi.ID.ToString() );
//                            lvi.SubItems[ 1 ].Text = cfi.Signature;
//                            lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
//                            lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
//                            lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
//                            lvi.Tag = cfi;
//                        }
//            }

//            _lvCallersInfo.Sort();
//            _lvCallersInfo.EndUpdate();
//        }

//        private void UpdateCalleeList()
//        {
//            _lvCalleesInfo.BeginUpdate();

//            bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
//            _lvCalleesInfo.ShowPlusMinus = multipleSelected;
//            _lvCalleesInfo.ShowRootTreeLines = multipleSelected;
//            _lvCalleesInfo.ShowTreeLines = multipleSelected;

//            ContainerListViewItem lviSuspend = null;

//            foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
//            {
//                FunctionInfo fi = ( FunctionInfo )item.Tag;

//                foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
//                {
//                    ContainerListViewItem parentItem = null;

//                    foreach ( ContainerListViewItem pitem in _lvCalleesInfo.Items )
//                        if( pitem.Tag != null)
//                            if ( ( pitem.Tag as CalleeFunctionInfo ).ID == cfi.ID )
//                                parentItem = pitem;


//                    if ( parentItem == null ) // don't have it
//                    {
//                        parentItem = _lvCalleesInfo.Items.Add( cfi.ID.ToString() );
//                        parentItem.SubItems[ 1 ].Text = cfi.Signature;
//                        parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
//                        parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
//                        parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
//                        parentItem.Tag = cfi;
//                    }
//                    else // do, update totals
//                    {
//                        parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
//                        parentItem.SubItems[ 3 ].Text = "-";
//                        parentItem.SubItems[ 4 ].Text = "-";
//                    }

//                    // either way, add a child pointing back to the parent
//                    ContainerListViewItem lvi = parentItem.Items.Add( fi.ID.ToString() );
//                    lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
//                    lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
//                    lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
//                    lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
//                    lvi.Tag = fi;
//                }

//                ContainerListViewItem inMethod = _lvCalleesInfo.Items.Add("(in method)");
//                inMethod.SubItems[1].Text = "(in method)";
//                inMethod.SubItems[2].Text = fi.Calls.ToString();
//                inMethod.SubItems[3].Text = fi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
//                inMethod.SubItems[4].Text = fi.PercentOfMethodTimeInMethod.ToString("0.00;-0.00;0");
//                inMethod.Tag = fi;



//                if ( fi.TotalSuspendedTicks > 0 )
//                {
//                    if ( lviSuspend == null) // don't have it
//                    {
//                        lviSuspend = _lvCalleesInfo.Items.Add( "(suspend)" );
//                        lviSuspend.SubItems[ 1 ].Text = "(thread suspended)";
//                        lviSuspend.SubItems[ 2 ].Text = "-";
//                        lviSuspend.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
//                        lviSuspend.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
//                    }
//                    else // do, update totals
//                    {
//                        lviSuspend.SubItems[ 3 ].Text = "-";
//                        lviSuspend.SubItems[ 4 ].Text = "-";
//                    }

//                    // either way, add a child pointing back to the parent
//                    ContainerListViewItem lvi = lviSuspend.Items.Add( fi.ID.ToString() );
//                    lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
//                    lvi.SubItems[ 2 ].Text = "-";
//                    lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
//                    lvi.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
//                }
//            }

//            _lvCalleesInfo.Sort();
//            _lvCalleesInfo.EndUpdate();
//        }

//        private void ProfilerControl_Load(object sender, System.EventArgs e)
//        {
//            _ptProcessTree.ThreadSelected += new ProcessTree.ThreadSelectedHandler( _ptProcessTree_ThreadSelected );
//            _lvFunctionInfo.Sort(1, true, true);
//            _lvCalleesInfo.Sort(1, true, true);
//        }

//        private void _ptProcessTree_ThreadSelected( ThreadInfo ti )
//        {
//            _tiCurrent = ti;
//            RefreshData();
//        }

//        private void _lvChildInfo_DoubleClick(object sender, System.EventArgs e)
//        {
//            ContainerListView ctl = sender as ContainerListView;

//            if ( ctl.SelectedItems.Count == 0 )
//                return;

//            if ( ctl.SelectedItems[ 0 ].Tag is CalleeFunctionInfo )
//            {
//                CalleeFunctionInfo cfi = ( CalleeFunctionInfo )ctl.SelectedItems[ 0 ].Tag;
//                if ( cfi == null )
//                    MessageBox.Show( "No information available for this item" );
//                else
//                    JumpToID( cfi.ID );
//            }
//            else if ( ctl.SelectedItems[ 0 ].Tag is FunctionInfo )
//            {
//                FunctionInfo fi = ( FunctionInfo )ctl.SelectedItems[ 0 ].Tag;
//                if ( fi == null )
//                    MessageBox.Show( "No information available for this item" );
//                else
//                    JumpToID( fi.ID );
//            }
//        }

//        private void _txtFilterBar_TextChanged(object sender, System.EventArgs e)
//        {
//            _tmrFilterThrottle.Enabled = false;
//            _tmrFilterThrottle.Enabled = true;
//        }

//        private void _tmrFilterThrottle_Tick(object sender, System.EventArgs e)
//        {
//            _tmrFilterThrottle.Enabled = false;
//            string text = _txtFilterBar.Text.ToLower();
//            foreach (ContainerListViewItem item in _lvFunctionInfo.Items)
//            {
//                if (text!="" && item.SubItems[1].Text.ToLower().IndexOf(text) != -1)
//                {
//                    item.BackColor = Color.LightSteelBlue;
//                }
//                else
//                {
//                    item.BackColor = _lvFunctionInfo.BackColor;
//                }
//            }
//            _lvFunctionInfo.BeginUpdate();
//            _lvFunctionInfo.Items.Sort(new SignatureComparer(text));
//            _lvFunctionInfo.EndUpdate();
//            _lvFunctionInfo.Invalidate();
//        }
//        private class SignatureComparer : IComparer
//        {
//            private string text;
//            public SignatureComparer(string text)
//            {
//                this.text = text;
//            }
//            public int Compare(object x, object y)
//            {
//                ContainerListViewItem a = (ContainerListViewItem)x;
//                ContainerListViewItem b = (ContainerListViewItem)y;
//                bool aFound=a.SubItems[1].Text.ToLower().IndexOf(text)!=-1;
//                bool bFound = b.SubItems[1].Text.ToLower().IndexOf(text) != -1;
//                if (aFound && !bFound)
//                {
//                    return -1;
//                }
//                else if (bFound && !aFound)
//                {
//                    return 1;
//                }
//                else
//                {
//                    if (a.Text == b.Text)
//                    {
//                        return 0;
//                    }
//                    else
//                    {
//                        return -1;
//                    }
//                }
//            }
//        }

//        public void  NavigateBackward()
//        {
//            if (_navStackBack.Count == 0)
//                return;

//            _navStackForward.Push(_navCurrent);
//            _navCurrent = ( int[] ) _navStackBack.Pop();

//            _bNavigating = true;
//            JumpToID( _navCurrent );
//            _bNavigating = false;
//        }

//        public void NavigateForward()
//        {
//            if ( _navStackForward.Count == 0 )
//                return;

//            _navStackBack.Push( _navCurrent );
//            _navCurrent = ( int[] ) _navStackForward.Pop();

//            _bNavigating = true;
//            JumpToID( _navCurrent );
//            _bNavigating = false;
//        }

//        private void JumpToID( int nID )
//        {
//            JumpToID( new int[] { nID } );
//        }

//        private void JumpToID( int[] ids )
//        {
//            _lvFunctionInfo.SelectedItems.Clear();

//            foreach( int id in ids )
//                foreach( ContainerListViewItem lvi in _lvFunctionInfo.Items )
//                {
//                    FunctionInfo fi = ( FunctionInfo )lvi.Tag;

//                    if ( fi.ID == id )
//                    {
//                        if(!lvi.Selected)
//                        {
//                            //if(lvi.IsFiltered)
//                            //{
//                            //    if(MessageBox.Show("Cannot navigate to that function because it is being filtered.  Would you like to remove the filter and continue to that function?", "Filtered Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
//                            //        return;
//                            //    else
//                            //        _txtFilterBar.Text = string.Empty;
//                            //}

//                            lvi.Selected = true;
//                            lvi.Focused = true;
//                        }
//                        break;
//                    }
//                }

//            _lvFunctionInfo.EnsureVisible();
//            _lvFunctionInfo.Focus();
//        }

//        private int GetSelectedID()
//        {
//            if ( _lvCalleesInfo.SelectedItems.Count == 0 )
//                return -1;

//            return ( ( FunctionInfo )_lvFunctionInfo.SelectedItems[ 0 ].Tag ).ID;
//        }

//        private ThreadInfo _tiCurrent;
//        private bool _bUpdating, _bInCheck;
//        private Hashtable _htCheckedItems;
//        private Stack _navStackBack = new Stack();
//        private Stack _navStackForward = new Stack();
//        private int[] _navCurrent = null;
//        private bool _bNavigating = false;

//        #region Context Menus

//        #region Headers

//        private void _lv_HeaderMenuEvent(object sender, System.Windows.Forms.MouseEventArgs e)
//        {
//            ContainerListView listView = sender as ContainerListView;

//            PopupMenu pop = new PopupMenu();
//            pop.Selected += new CommandHandler(pop_Selected);
//            pop.Deselected += new CommandHandler(pop_Deselected);

//            MenuCommand sortBy = pop.MenuCommands.Add(new MenuCommand("Sort &By"));
//            pop.MenuCommands.Add(new MenuCommand("-"));
//            bool isAscending = true;
//            for(int idx = 0; idx < listView.Columns.Count; ++idx)
//            {
//                ContainerListViewColumnHeader hdr = listView.Columns[idx];
//                MenuCommand sortByItem = new MenuCommand(hdr.Text);

//                sortByItem.Description = string.Format("Sort By the '{1}' column from this grid", (hdr.Visible ? "Shows" : "Hides"), hdr.Text);
//                sortByItem.RadioCheck = true;
//                sortByItem.Checked = hdr.SortOrder != SortOrder.None;
//                sortByItem.Tag = new object[] { listView, idx };
//                //				sortByItem.Click += new EventHandler(sortByItem_Click);

//                if(sortByItem.Checked)
//                    isAscending = hdr.SortOrder == SortOrder.Ascending;

//                sortBy.MenuCommands.Add(sortByItem);
//            }

//            sortBy.MenuCommands.Add(new MenuCommand("-"));
//            MenuCommand ascending = sortBy.MenuCommands.Add(new MenuCommand("&Ascending"));
//            ascending.RadioCheck = true;
//            ascending.Checked = isAscending;
//            ascending.Tag = new object[] { listView, SortOrder.Ascending };
//            //			ascending.Click += new EventHandler(sortOrder_Click);

//            MenuCommand descending = sortBy.MenuCommands.Add(new MenuCommand("&Descending"));
//            descending.RadioCheck = true;
//            descending.Checked = !isAscending;
//            descending.Tag = new object[] { listView, SortOrder.Descending };
//            //			descending.Click += new EventHandler(sortOrder_Click);

//            bool allShown = true;
//            for(int idx = 0; idx < listView.Columns.Count; ++idx)
//            {
//                ContainerListViewColumnHeader hdr = listView.Columns[idx];
//                MenuCommand checkable = new MenuCommand(hdr.Text);

//                checkable.Description = string.Format("{0} the '{1}' column from this grid", (hdr.Visible ? "Shows" : "Hides"), hdr.Text);
//                checkable.Checked = hdr.Visible;
//                checkable.Tag = hdr;

//                pop.MenuCommands.Add(checkable);
//                allShown &= hdr.Visible;
//            }

//            pop.MenuCommands.Add(new MenuCommand("-"));
//            pop.MenuCommands.Add(new MenuCommand("Show &All")).Enabled = !allShown;

//            MenuCommand result = pop.TrackPopup(listView.PointToScreen(new Point(e.X, e.Y)));
//            if(result != null && result.Tag is ContainerListViewColumnHeader)
//                (result.Tag as ContainerListViewColumnHeader).Visible = !result.Checked;
//        }

//        //		private void sortOrder_Click(object sender, EventArgs e)
//        //		{
//        //			MenuCommand cmd = sender as MenuCommand;
//        //
//        //			object[] objs = (cmd.Tag as object[]);
//        //			ContainerListView listView = objs[0] as ContainerListView;
//        //
//        //			listView.Sort((SortOrder)objs[1]);
//        //		}

//        //		private void sortByItem_Click(object sender, EventArgs e)
//        //		{
//        //			MenuCommand cmd = sender as MenuCommand;
//        //
//        //			object[] objs = (cmd.Tag as object[]);
//        //			ContainerListView listView = objs[0] as ContainerListView;
//        //
//        //			listView.Sort((int)objs[1], false);
//        //		}

//        #endregion

//        private void pop_Selected(MenuCommand item)
//        {
//            // TODO: Update status bar with item.Description
//        }

//        private void pop_Deselected(MenuCommand item)
//        {
//            // TODO: Update status bar with string.Empty
//        }

//        #endregion
//    }
//}

























