using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;
using NProf.Utilities.DataStore;
using Genghis.Windows.Forms;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ProfilerForm : System.Windows.Forms.Form
	{
		private Stack _stackBack;
		private Stack _stackForward;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.StatusBar _sbStatusBar;
		private Crownwood.Magic.Menus.MenuCommand _menuFile;
		private Crownwood.Magic.Menus.MenuCommand _menuEdit;
		private Crownwood.Magic.Menus.MenuCommand _menuHelp;
		private Crownwood.Magic.Controls.TabControl _tcProfilers;
		private Crownwood.Magic.Menus.MenuControl _menuMain;
		private Crownwood.Magic.Menus.MenuCommand _menuProject;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRun;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectOptions;
		private Crownwood.Magic.Docking.DockingManager _dock;
		private Crownwood.Magic.Menus.MenuCommand _menuView;

		private Crownwood.Magic.Menus.MenuCommand _sep2;
		private Crownwood.Magic.Menus.MenuCommand _sep1;
		private Crownwood.Magic.Menus.MenuCommand _cmdHelpAbout;
		private System.Windows.Forms.StatusBarPanel _sbpMessage;
		private Profiler							_p;
		private ProjectTree							_pt;
		private ProjectInfoCollection				_pic;
		private ProjectInfo							_piInitialProject;
		private ProjectInfo							_piVSNetProject;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunViewMessages;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunCopy;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectStop;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileNew;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileOpen;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileClose;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileSave;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileSaveAs;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileSaveAll;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileRecentProjects;
		private Crownwood.Magic.Menus.MenuCommand _cmdFileExit;
		private Crownwood.Magic.Menus.MenuCommand _cmdViewProjectView;
		private Crownwood.Magic.Menus.MenuCommand _cmdViewNavBack;
		private Crownwood.Magic.Menus.MenuCommand _cmdViewNavForward;
		private Crownwood.Magic.Menus.MenuCommand _sep4;
		private Crownwood.Magic.Menus.MenuCommand _sep3;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProfilerForm()
		{
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
		}

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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this._menuMain = new Crownwood.Magic.Menus.MenuControl();
			this._menuFile = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileNew = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileOpen = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileClose = new Crownwood.Magic.Menus.MenuCommand();
			this._sep1 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileSave = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileSaveAs = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileSaveAll = new Crownwood.Magic.Menus.MenuCommand();
			this._sep2 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileRecentProjects = new Crownwood.Magic.Menus.MenuCommand();
			this._sep3 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdFileExit = new Crownwood.Magic.Menus.MenuCommand();
			this._menuEdit = new Crownwood.Magic.Menus.MenuCommand();
			this._menuProject = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRun = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectStop = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectOptions = new Crownwood.Magic.Menus.MenuCommand();
			this._sep4 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRunViewMessages = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRunCopy = new Crownwood.Magic.Menus.MenuCommand();
			this._menuView = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdViewProjectView = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdViewNavBack = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdViewNavForward = new Crownwood.Magic.Menus.MenuCommand();
			this._menuHelp = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdHelpAbout = new Crownwood.Magic.Menus.MenuCommand();
			this._tcProfilers = new Crownwood.Magic.Controls.TabControl();
			this._sbStatusBar = new System.Windows.Forms.StatusBar();
			this._sbpMessage = new System.Windows.Forms.StatusBarPanel();
			((System.ComponentModel.ISupportInitialize)(this._sbpMessage)).BeginInit();
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
			this._menuMain.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this._menuMain.HighlightTextColor = System.Drawing.SystemColors.MenuText;
			this._menuMain.Location = new System.Drawing.Point(0, 0);
			this._menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																													  this._menuFile,
																													  this._menuEdit,
																													  this._menuProject,
																													  this._menuView,
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
																													  this._cmdFileClose,
																													  this._sep1,
																													  this._cmdFileSave,
																													  this._cmdFileSaveAs,
																													  this._cmdFileSaveAll,
																													  this._sep2,
																													  this._cmdFileRecentProjects,
																													  this._sep3,
																													  this._cmdFileExit});
			this._menuFile.Text = "&File";
			// 
			// _cmdFileNew
			// 
			this._cmdFileNew.Description = "New Profiler Project";
			this._cmdFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this._cmdFileNew.Text = "&New...";
			this._cmdFileNew.Click += new System.EventHandler(this._cmdFileNew_Click);
			this._cmdFileNew.Update += new System.EventHandler(this.UpdateMenuItems);
			// 
			// _cmdFileOpen
			// 
			this._cmdFileOpen.Description = "Open a profile project";
			this._cmdFileOpen.Text = "&Open...";
			this._cmdFileOpen.Click += new System.EventHandler(this._cmdFileOpen_Click);
			// 
			// _cmdFileClose
			// 
			this._cmdFileClose.Description = "Close the project";
			this._cmdFileClose.Text = "&Close";
			this._cmdFileClose.Click += new System.EventHandler(this._cmdFileClose_Click);
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
			// _cmdFileSaveAll
			// 
			this._cmdFileSaveAll.Description = "Save all open profiler projects";
			this._cmdFileSaveAll.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS;
			this._cmdFileSaveAll.Text = "Save A&ll";
			this._cmdFileSaveAll.Click += new System.EventHandler(this._cmdFileSaveAll_Click);
			// 
			// _sep2
			// 
			this._sep2.Description = "MenuItem";
			this._sep2.Text = "-";
			// 
			// _cmdFileRecentProjects
			// 
			this._cmdFileRecentProjects.Description = "Provides a list of the 10 most recently opened projects for easy access";
			this._cmdFileRecentProjects.Text = "Recent Projects";
			// 
			// _sep3
			// 
			this._sep3.Description = "-";
			this._sep3.Text = "-";
			// 
			// _cmdFileExit
			// 
			this._cmdFileExit.Description = "Exit the application";
			this._cmdFileExit.Shortcut = System.Windows.Forms.Shortcut.AltF4;
			this._cmdFileExit.Text = "E&xit";
			this._cmdFileExit.Click += new System.EventHandler(this._cmdFileExit_Click);
			// 
			// _menuEdit
			// 
			this._menuEdit.Description = "Edit";
			this._menuEdit.Text = "&Edit";
			// 
			// _menuProject
			// 
			this._menuProject.Description = "Project commands";
			this._menuProject.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																														  this._cmdProjectRun,
																														  this._cmdProjectStop,
																														  this._cmdProjectOptions,
																														  this._sep4,
																														  this._cmdProjectRunViewMessages,
																														  this._cmdProjectRunCopy});
			this._menuProject.Text = "&Project";
			// 
			// _cmdProjectRun
			// 
			this._cmdProjectRun.Description = "Run the current project";
			this._cmdProjectRun.Shortcut = System.Windows.Forms.Shortcut.F5;
			this._cmdProjectRun.Text = "Start project run";
			this._cmdProjectRun.Click += new System.EventHandler(this._cmdProjectRun_Click);
			this._cmdProjectRun.Update += new System.EventHandler(this.UpdateMenuItems);
			// 
			// _cmdProjectStop
			// 
			this._cmdProjectStop.Description = "Stop the running project";
			this._cmdProjectStop.Text = "Stop project run";
			// 
			// _cmdProjectOptions
			// 
			this._cmdProjectOptions.Description = "Modify the options for this project";
			this._cmdProjectOptions.Shortcut = System.Windows.Forms.Shortcut.F2;
			this._cmdProjectOptions.Text = "Options...";
			this._cmdProjectOptions.Click += new System.EventHandler(this._cmdProjectOptions_Click);
			this._cmdProjectOptions.Update += new System.EventHandler(this.UpdateMenuItems);
			// 
			// _sep4
			// 
			this._sep4.Description = "-";
			this._sep4.Text = "-";
			// 
			// _cmdProjectRunViewMessages
			// 
			this._cmdProjectRunViewMessages.Description = "View the Messages from the current profiler run...";
			this._cmdProjectRunViewMessages.Text = "View Run Messages...";
			this._cmdProjectRunViewMessages.Click += new System.EventHandler(this._cmdProjectRunViewMessages_Click);
			// 
			// _cmdProjectRunCopy
			// 
			this._cmdProjectRunCopy.Description = "Copy the project run data to the clipboard";
			this._cmdProjectRunCopy.Text = "Copy Project Run Data";
			// 
			// _menuView
			// 
			this._menuView.Description = "View";
			this._menuView.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																													  this._cmdViewProjectView,
																													  this._cmdViewNavBack,
																													  this._cmdViewNavForward});
			this._menuView.Text = "&View";
			// 
			// _cmdViewProjectView
			// 
			this._cmdViewProjectView.Description = "Show/hide the project tab";
			this._cmdViewProjectView.Text = "Project tab";
			this._cmdViewProjectView.Click += new System.EventHandler(this._cmdViewProjectView_Click);
			this._cmdViewProjectView.Update += new System.EventHandler(this._cmdViewProjectView_Update);
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
			// _tcProfilers
			// 
			this._tcProfilers.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tcProfilers.IDEPixelArea = true;
			this._tcProfilers.Location = new System.Drawing.Point(0, 25);
			this._tcProfilers.Name = "_tcProfilers";
			this._tcProfilers.Size = new System.Drawing.Size(920, 630);
			this._tcProfilers.TabIndex = 2;
			this._tcProfilers.ClosePressed += new System.EventHandler(this._tcProfilers_ClosePressed);
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
			this._sbpMessage.Text = "Ready.";
			this._sbpMessage.Width = 904;
			// 
			// ProfilerForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(920, 677);
			this.Controls.Add(this._tcProfilers);
			this.Controls.Add(this._menuMain);
			this.Controls.Add(this._sbStatusBar);
			this.Name = "ProfilerForm";
			this.Text = "nprof Profiling Application - Alpha v0.6";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ProfilerForm_Closing);
			this.Load += new System.EventHandler(this.ProfilerForm_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ProfilerForm_Layout);
			((System.ComponentModel.ISupportInitialize)(this._sbpMessage)).EndInit();
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
				_pic.Add( _piVSNetProject );
			}

			// Run the project
			_pt.SelectProject( _piVSNetProject );
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

			_pic = new ProjectInfoCollection();
			_pt = new ProjectTree();

			_pic.ProjectRemoved +=new NProf.Glue.Profiler.Project.ProjectInfoCollection.ProjectEventHandler(_pic_ProjectRemoved);

			_pt.Projects = _pic;
			_pt.ProjectDoubleClicked += new ProjectTree.ProjectDoubleClickedHandler( _pt_ProjectDoubleClicked );
			_pt.RunDoubleClicked += new ProjectTree.RunDoubleClickedHandler( _pt_RunDoubleClicked );
			_pt.ExecutablesDropped +=new ProjectTree.ExecutablesDroppedHandler( _pt_ExecutableDropped );

			_dock = new Crownwood.Magic.Docking.DockingManager( this, Crownwood.Magic.Common.VisualStyle.IDE );
			_dock.OuterControl = _menuMain;
			_dock.InnerControl = _tcProfilers;
			Crownwood.Magic.Docking.Content c = _dock.Contents.Add( _pt, "Projects" );
			Crownwood.Magic.Docking.WindowContent wc = _dock.AddContentWithState( c, Crownwood.Magic.Docking.State.DockLeft );

			this.Text = "nprof Profiling Application - v" + Profiler.Version;
			_tcProfilers.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;

			SerializationHandler.DataStoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NProf\\" + Profiler.Version;

			_cmdFileRecentProjects.MenuCommands.Clear();

			int count = 1;
			foreach(UsedFile usedFile in SerializationHandler.ProjectsHistory)
			{
				if(count > 10)
					break;

				string label = (count == 10 ? "1&0" : "&" + count.ToString()) + " " + usedFile.ProjectName;

				Crownwood.Magic.Menus.MenuCommand pInfo = new Crownwood.Magic.Menus.MenuCommand(label);
				pInfo.Tag = usedFile.FileName;
				pInfo.Click += new EventHandler(_cmdFileRecentlyUsed_Click);
				pInfo.Shortcut = Shortcut.CtrlShift1 + (count == 10 ? -1 : count - 1);
				_cmdFileRecentProjects.MenuCommands.Add(pInfo);

				++count;
			}

			if( count == 1 )
				_cmdFileRecentProjects.Enabled = false;

			if ( _piInitialProject != null )
			{
				_pic.Add( _piInitialProject );
				_pt.SelectProject( _piInitialProject );
				_cmdProjectRun_Click( null, null );
			}
		}

		private void _tcProfilers_ClosePressed(object sender, System.EventArgs e)
		{
			if ( IsShowingBlankTab() )
				return;

			_tcProfilers.TabPages.Remove( _tcProfilers.SelectedTab );
			CheckAddBlankTab();
		}

		private void ProfilerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_p.Stop();
		}

		#region Menu event handlers

		#region File

		private void _cmdFileNew_Click(object sender, System.EventArgs e)
		{
			ProfilerProjectOptionsForm frm = new ProfilerProjectOptionsForm();
			frm.Mode = ProfilerProjectOptionsForm.ProfilerProjectMode.CreateProject;
			DialogResult dr = frm.ShowDialog( this );
			if ( dr == DialogResult.OK )
			{
				_pic.Add( frm.Project );
				_pt.SelectProject( frm.Project );
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
					_pic.Add( project );
					_pt.SelectProject( project );
				}
			}
		}

		private void _cmdFileClose_Click(object sender, System.EventArgs e)
		{
			_pic.Remove( GetCurrentProject() );
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
			foreach( ProjectInfo project in _pic )
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
				_pic.Add( project );
				_pt.SelectProject( project );
			}
		}

		#endregion

		#region Project

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

			CreateTabPage( run );

			_pt.SelectRun( run );
		}

		private void _cmdViewProjectView_Click(object sender, System.EventArgs e)
		{
			if ( !_cmdViewProjectView.Checked )
			{
				_dock.ShowContent( _dock.Contents[ "Projects" ] );
				_cmdViewProjectView.Checked = true;
			}
			else
			{
				_dock.HideContent( _dock.Contents[ "Projects" ] );
				_cmdViewProjectView.Checked = false;
			}
		}

		private void _cmdViewProjectView_Update(object sender, System.EventArgs e)
		{
			_cmdViewProjectView.Checked = _dock.Contents[ "Projects" ].Visible;
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
			Run r = ( Run )_tcProfilers.SelectedTab.Tag;
			ProfilerRunMessagesForm frm = new ProfilerRunMessagesForm();
			frm.ProfilerRun = r;
			frm.ShowDialog( this );
		}

		#endregion

		#region View

		private void _cmdViewNavBack_Click(object sender, System.EventArgs e)
		{
			Crownwood.Magic.Controls.TabPage tpActive = _tcProfilers.SelectedTab;
			if (tpActive == null)
				return;

			if (tpActive.Controls.Count == 0 || !(tpActive.Controls[0] is ProfilerControl)) 
				return;

			((ProfilerControl) tpActive.Controls[0]).NavigateBackward();
		}

		private void _cmdViewNavForward_Click(object sender, System.EventArgs e)
		{

			Crownwood.Magic.Controls.TabPage tpActive = _tcProfilers.SelectedTab;

			if (tpActive == null)
				return;

			if (tpActive.Controls.Count == 0 || !(tpActive.Controls[0] is ProfilerControl)) 
				return;

			((ProfilerControl) tpActive.Controls[0]).NavigateForward();

		}

		#endregion
		
		#region Help

		private void _cmdHelpAbout_Click(object sender, System.EventArgs e)
		{
			AboutForm frm = new AboutForm();
			frm.ShowDialog( this );
		}

		#endregion

		#endregion

		#region ProjectInfoCollection event handlers

		private void _pic_ProjectRemoved(ProjectInfoCollection projects, ProjectInfo project, int nIndex)
		{
			foreach ( Run run in project.Runs )
			{
				foreach ( Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages )
				{
					if ( tp.Tag == run )
					{
						_tcProfilers.TabPages.Remove ( tp );
						break;
					}
				}
			}

			CheckAddBlankTab();
		}

		#endregion

		#region ProjectTree event handlers

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

			CreateTabPage( run );
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
					_pic.Add( frm.Project );
					_pt.SelectProject( frm.Project );
				}
			}
		}

		#endregion

		private bool SaveProject( ProjectInfo project, bool forceSaveDialog )
		{
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

				filename = saveDlg.FileName;
			}

			SerializationHandler.SaveProjectInfo( project, filename );

			return true;
		}

		private ProjectInfo GetCurrentProject()
		{
			return _pt.GetSelectedProject();
		}

		private void CheckAddBlankTab()
		{
			if ( _tcProfilers.TabPages.Count == 0 )
			{
				Crownwood.Magic.Controls.TabPage tp = new Crownwood.Magic.Controls.TabPage( "No profiler runs loaded" );
				_tcProfilers.TabPages.Add( tp );
				NoProfilerRunsLoaded np = new NoProfilerRunsLoaded();
				tp.Controls.Add( np );
				np.Dock = DockStyle.Fill;
			}
		}

		private void CheckRemoveBlankTab()
		{
			if ( IsShowingBlankTab() )
				_tcProfilers.TabPages.Clear();
		}

		private bool IsShowingBlankTab()
		{
			return ( _tcProfilers.TabPages[ 0 ].Title == "No profiler runs loaded" );
		}

		private Crownwood.Magic.Controls.TabPage CreateTabPage( Run run )
		{
			CheckRemoveBlankTab();

			Crownwood.Magic.Controls.TabPage tpActive = null;

			// Does it exist already?
			foreach ( Crownwood.Magic.Controls.TabPage tp in _tcProfilers.TabPages )
			{
				if ( tp.Tag == run )
				{
					_tcProfilers.SelectedTab = tp;
					tpActive = tp;
					break;
				}
			}
	
			if ( tpActive == null )
			{
				tpActive = new Crownwood.Magic.Controls.TabPage( run.Project.Name + " [" + run.StartTime + "]" );
				tpActive.Tag = run;
				_tcProfilers.TabPages.Add( tpActive );
				_tcProfilers.SelectedTab = tpActive;
			}

			if ( run.State == Run.RunState.Finished
				&& run.Success 
				&& ( tpActive.Controls.Count == 0 || !( tpActive.Controls[ 0 ] is ProfilerControl ) ) )
			{
				tpActive.Controls.Clear();
				ProfilerControl pc = new ProfilerControl();
				pc.Dock = DockStyle.Fill;
				tpActive.Controls.Add( pc );
				pc.ProfilerRun = run;
			}

			// Catch non-successful finished runs here too
			if ( ( ( run.State == Run.RunState.Finished && !run.Success ) 
				|| run.State == Run.RunState.Running 
				|| run.State == Run.RunState.Initializing )
				&& ( tpActive.Controls.Count == 0 || !( tpActive.Controls[ 0 ] is ProfilerRunControl ) ) )
			{
				tpActive.Controls.Clear();
				ProfilerRunControl pc = new ProfilerRunControl();
				pc.Dock = DockStyle.Fill;
				tpActive.Controls.Add( pc );
				pc.ProfilerRun = run;
			}

			return tpActive;
		}

		private void UpdateMenuItems(object sender, System.EventArgs e)
		{
			bool bCanRunOrEdit = _pt.GetSelectedProject() != null 
				&& _pt.GetSelectedProject().ProjectType != ProjectType.VSNet;
			Run run = _pt.GetSelectedRun();

			_cmdProjectRun.Enabled = bCanRunOrEdit;
			_cmdProjectStop.Enabled = bCanRunOrEdit && ( run != null && run.State == Run.RunState.Running );
			_cmdProjectOptions.Enabled = bCanRunOrEdit;
			_cmdProjectRunViewMessages.Enabled = ( !IsShowingBlankTab() );
			_cmdProjectRunCopy.Enabled = ( !IsShowingBlankTab() );

			_cmdFileClose.Enabled = bCanRunOrEdit;
			_cmdFileSave.Enabled = bCanRunOrEdit;
			_cmdFileSaveAs.Enabled = bCanRunOrEdit;
			_cmdFileSaveAll.Enabled = ( !IsShowingBlankTab() );

			_cmdViewNavBack.Enabled = ( !IsShowingBlankTab() );
			_cmdViewNavForward.Enabled = ( !IsShowingBlankTab() );
		}
	}
}
