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
		private Crownwood.Magic.Menus.MenuCommand _cmdNew;
		private Crownwood.Magic.Menus.MenuCommand _cmdOpen;
		private Crownwood.Magic.Menus.MenuCommand _cmdSave;
		private Crownwood.Magic.Menus.MenuCommand _cmdSaveAll;
		private Crownwood.Magic.Menus.MenuCommand _cmdExit;
		private Crownwood.Magic.Controls.TabControl _tcProfilers;
		private Crownwood.Magic.Menus.MenuControl _menuMain;
		private Crownwood.Magic.Menus.MenuCommand menuCommand1;
		private Crownwood.Magic.Menus.MenuCommand _menuProject;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRun;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectOptions;
		private Crownwood.Magic.Docking.DockingManager _dock;
		private Crownwood.Magic.Menus.MenuCommand _menuView;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectView;

		private Crownwood.Magic.Menus.MenuCommand _sep2;
		private Crownwood.Magic.Menus.MenuCommand _sep1;
		private Crownwood.Magic.Menus.MenuCommand _cmdHelpAbout;
		private Crownwood.Magic.Menus.MenuCommand menuCommand2;
		private Crownwood.Magic.Menus.MenuCommand menuCommand3;
		private Crownwood.Magic.Menus.MenuCommand menuCommand4;
		private System.Windows.Forms.StatusBarPanel _sbpMessage;
		private Profiler							_p;
		private ProjectTree							_pt;
		private ProjectInfoCollection				_pic;
		private ProjectInfo							_piInitialProject;
		private ProjectInfo							_piVSNetProject;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunViewMessages;
		private Crownwood.Magic.Menus.MenuCommand menuCommand5;
		private Crownwood.Magic.Menus.MenuCommand menuCommand6;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectRunCopy;
		private Crownwood.Magic.Menus.MenuCommand _cmdProjectStop;
		private Crownwood.Magic.Menus.MenuCommand _cmdClose;

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
			this._cmdNew = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdOpen = new Crownwood.Magic.Menus.MenuCommand();
			this._sep1 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdSave = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdSaveAll = new Crownwood.Magic.Menus.MenuCommand();
			this._sep2 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdExit = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdClose = new Crownwood.Magic.Menus.MenuCommand();
			this._menuEdit = new Crownwood.Magic.Menus.MenuCommand();
			this._menuProject = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRun = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectStop = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectOptions = new Crownwood.Magic.Menus.MenuCommand();
			this.menuCommand5 = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRunViewMessages = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectRunCopy = new Crownwood.Magic.Menus.MenuCommand();
			this._menuView = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdProjectView = new Crownwood.Magic.Menus.MenuCommand();
			this._menuHelp = new Crownwood.Magic.Menus.MenuCommand();
			this._cmdHelpAbout = new Crownwood.Magic.Menus.MenuCommand();
			this._tcProfilers = new Crownwood.Magic.Controls.TabControl();
			this._sbStatusBar = new System.Windows.Forms.StatusBar();
			this._sbpMessage = new System.Windows.Forms.StatusBarPanel();
			this.menuCommand1 = new Crownwood.Magic.Menus.MenuCommand();
			this.menuCommand2 = new Crownwood.Magic.Menus.MenuCommand();
			this.menuCommand3 = new Crownwood.Magic.Menus.MenuCommand();
			this.menuCommand4 = new Crownwood.Magic.Menus.MenuCommand();
			this.menuCommand6 = new Crownwood.Magic.Menus.MenuCommand();
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
																													  this._cmdNew,
																													  this._cmdOpen,
																													  this._cmdClose,
																													  this._sep1,
																													  this._cmdSave,
																													  this._cmdSaveAll,
																													  this._sep2,
																													  this._cmdExit});
			this._menuFile.Text = "&File";
			// 
			// _cmdNew
			// 
			this._cmdNew.Description = "New Profiler Project";
			this._cmdNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this._cmdNew.Text = "&New...";
			this._cmdNew.Click += new System.EventHandler(this._cmdNew_Click);
			// 
			// _cmdOpen
			// 
			this._cmdOpen.Description = "Open a profile project";
			this._cmdOpen.Enabled = false;
			this._cmdOpen.Text = "&Open";
			// 
			// _sep1
			// 
			this._sep1.Description = "MenuItem";
			this._sep1.Text = "-";
			// 
			// _cmdSave
			// 
			this._cmdSave.Description = "Save the active profiler project";
			this._cmdSave.Enabled = false;
			this._cmdSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this._cmdSave.Text = "&Save";
			// 
			// _cmdSaveAll
			// 
			this._cmdSaveAll.Description = "Save all open profiler projects";
			this._cmdSaveAll.Enabled = false;
			this._cmdSaveAll.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS;
			this._cmdSaveAll.Text = "Save A&ll";
			// 
			// _sep2
			// 
			this._sep2.Description = "MenuItem";
			this._sep2.Text = "-";
			// 
			// _cmdExit
			// 
			this._cmdExit.Description = "Exit the application";
			this._cmdExit.Shortcut = System.Windows.Forms.Shortcut.AltF4;
			this._cmdExit.Text = "E&xit";
			this._cmdExit.Click += new System.EventHandler(this._cmdExit_Click);
			// 
			// _cmdClose
			// 
			this._cmdClose.Description = "Close the project";
			this._cmdClose.Text = "&Close";
			this._cmdClose.Click += new System.EventHandler(this._cmdClose_Click);
			// 
			// _menuEdit
			// 
			this._menuEdit.Description = "Edit";
			this._menuEdit.Enabled = false;
			this._menuEdit.Text = "&Edit";
			// 
			// _menuProject
			// 
			this._menuProject.Description = "Project commands";
			this._menuProject.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
																														  this._cmdProjectRun,
																														  this._cmdProjectStop,
																														  this._cmdProjectOptions,
																														  this.menuCommand5,
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
			this._cmdProjectRun.Update += new System.EventHandler(this.UpdateProjectItems);
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
			this._cmdProjectOptions.Update += new System.EventHandler(this.UpdateProjectItems);
			// 
			// menuCommand5
			// 
			this.menuCommand5.Description = "-";
			this.menuCommand5.Text = "-";
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
																													  this._cmdProjectView});
			this._menuView.Text = "&View";
			// 
			// _cmdProjectView
			// 
			this._cmdProjectView.Description = "Show/hide the project tab";
			this._cmdProjectView.Text = "Project tab";
			this._cmdProjectView.Click += new System.EventHandler(this._cmdProjectView_Click);
			this._cmdProjectView.Update += new System.EventHandler(this._cmdProjectView_Update);
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
			this._tcProfilers.SelectionChanged += new System.EventHandler(this._tcProfilers_SelectionChanged);
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
			// menuCommand1
			// 
			this.menuCommand1.Description = "MenuItem";
			// 
			// menuCommand2
			// 
			this.menuCommand2.Break = true;
			this.menuCommand2.Description = "MenuItem";
			// 
			// menuCommand3
			// 
			this.menuCommand3.Break = true;
			this.menuCommand3.Description = "MenuItem";
			// 
			// menuCommand4
			// 
			this.menuCommand4.Description = "MenuItem";
			// 
			// menuCommand6
			// 
			this.menuCommand6.Description = "MenuItem";
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
			this.Leave += new System.EventHandler(this.ProfilerForm_Leave);
			this.Deactivate += new System.EventHandler(this.ProfilerForm_Deactivate);
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

		private void ProfilerForm_Leave(object sender, System.EventArgs e)
		{
		}

		private void ProfilerForm_Deactivate(object sender, System.EventArgs e)
		{
		}

		private void ProfilerForm_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			CheckAddBlankTab();
		}

		private void _cmdNew_Click(object sender, System.EventArgs e)
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

		private void _cmdProjectView_Click(object sender, System.EventArgs e)
		{
			if ( !_cmdProjectView.Checked )
			{
				_dock.ShowContent( _dock.Contents[ "Projects" ] );
				_cmdProjectView.Checked = true;
			}
			else
			{
				_dock.HideContent( _dock.Contents[ "Projects" ] );
				_cmdProjectView.Checked = false;
			}
		}

		private void _cmdProjectView_Update(object sender, System.EventArgs e)
		{
			_cmdProjectView.Checked = _dock.Contents[ "Projects" ].Visible;
		}

		private void _cmdExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void ProfilerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_p.Stop();
		}

		private void UpdateProjectItems(object sender, System.EventArgs e)
		{
			bool bCanRunOrEdit = _pt.GetSelectedProject() != null 
				&& _pt.GetSelectedProject().ProjectType != ProjectType.VSNet;
			Run run = _pt.GetSelectedRun();

			_cmdProjectRun.Enabled = bCanRunOrEdit;
			_cmdProjectStop.Enabled = bCanRunOrEdit && ( run != null && run.State == Run.RunState.Running );
			_cmdProjectOptions.Enabled = bCanRunOrEdit;
			_cmdClose.Enabled = bCanRunOrEdit;
			_cmdProjectRunViewMessages.Enabled = ( !IsShowingBlankTab() );
		}

		private void _cmdProjectOptions_Click(object sender, System.EventArgs e)
		{
			ProfilerProjectOptionsForm frm = new ProfilerProjectOptionsForm();
			frm.Project = GetCurrentProject();
			frm.Mode = ProfilerProjectOptionsForm.ProfilerProjectMode.ModifyProject;

			frm.ShowDialog( this );
		}

		private void _cmdHelpAbout_Click(object sender, System.EventArgs e)
		{
			AboutForm frm = new AboutForm();
			frm.ShowDialog( this );
		}

		private void _cmdProjectRunViewMessages_Click(object sender, System.EventArgs e)
		{
			Run r = ( Run )_tcProfilers.SelectedTab.Tag;
			ProfilerRunMessagesForm frm = new ProfilerRunMessagesForm();
			frm.ProfilerRun = r;
			frm.ShowDialog( this );
		}

		private void _tcProfilers_SelectionChanged(object sender, System.EventArgs e)
		{
		}

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

		private void _cmdClose_Click(object sender, System.EventArgs e)
		{
		}
	}
}
