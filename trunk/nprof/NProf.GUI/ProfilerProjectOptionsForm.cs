using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Genghis.Windows.Forms;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Project;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for OpenProfilerProjectForm.
	/// </summary>
	public class ProfilerProjectOptionsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox _cbDebugProfilee;
		private System.Windows.Forms.TextBox _txtApplicationName;
		private System.Windows.Forms.TextBox _txtArguments;
		private System.Windows.Forms.TextBox _txtWorkingDirectory;
		private System.Windows.Forms.Button _btnCreateProject;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.ToolTip _ttToolTips;
		private System.Windows.Forms.Button _btnBrowseApplication;
		private System.Windows.Forms.Button _btnBrowseWorkingDirectory;
		private System.ComponentModel.IContainer components;

		private ProjectInfo _p;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.RadioButton _rbAspNet;
		private System.Windows.Forms.RadioButton _rbFile;
		private System.Windows.Forms.RadioButton _rbRemote;
		private System.Windows.Forms.Label _lblAspNet;
		private System.Windows.Forms.Label label6;
		private ProfilerProjectMode _ppm;

		public ProfilerProjectOptionsForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_ppm = ProfilerProjectMode.CreateProject;

			_p = new ProjectInfo( ProjectType.File );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this._cbDebugProfilee = new System.Windows.Forms.CheckBox();
			this._btnBrowseApplication = new System.Windows.Forms.Button();
			this._txtApplicationName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._txtArguments = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._txtWorkingDirectory = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._btnBrowseWorkingDirectory = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this._btnCreateProject = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this._ttToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.label5 = new System.Windows.Forms.Label();
			this._rbAspNet = new System.Windows.Forms.RadioButton();
			this._rbFile = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this._rbRemote = new System.Windows.Forms.RadioButton();
			this._lblAspNet = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cbDebugProfilee
			// 
			this._cbDebugProfilee.Location = new System.Drawing.Point(8, 16);
			this._cbDebugProfilee.Name = "_cbDebugProfilee";
			this._cbDebugProfilee.Size = new System.Drawing.Size(128, 24);
			this._cbDebugProfilee.TabIndex = 6;
			this._cbDebugProfilee.Text = "Debug profiler hook";
			this._ttToolTips.SetToolTip(this._cbDebugProfilee, "Launch the debugger as soon as the profilee starts");
			// 
			// _btnBrowseApplication
			// 
			this._btnBrowseApplication.Location = new System.Drawing.Point(520, 88);
			this._btnBrowseApplication.Name = "_btnBrowseApplication";
			this._btnBrowseApplication.TabIndex = 2;
			this._btnBrowseApplication.Text = "Browse...";
			this._btnBrowseApplication.Click += new System.EventHandler(this._btnBrowseApplication_Click);
			// 
			// _txtApplicationName
			// 
			this._txtApplicationName.Location = new System.Drawing.Point(120, 88);
			this._txtApplicationName.Name = "_txtApplicationName";
			this._txtApplicationName.Size = new System.Drawing.Size(392, 20);
			this._txtApplicationName.TabIndex = 1;
			this._txtApplicationName.Text = "";
			this._ttToolTips.SetToolTip(this._txtApplicationName, "Locate the execute to profile");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 88);
			this.label1.Name = "label1";
			this.label1.TabIndex = 14;
			this.label1.Text = "Application to run:";
			// 
			// _txtArguments
			// 
			this._txtArguments.Location = new System.Drawing.Point(120, 112);
			this._txtArguments.Name = "_txtArguments";
			this._txtArguments.Size = new System.Drawing.Size(392, 20);
			this._txtArguments.TabIndex = 3;
			this._txtArguments.Text = "";
			this._ttToolTips.SetToolTip(this._txtArguments, "Enter any command line arguments to pass to the above executable");
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 112);
			this.label3.Name = "label3";
			this.label3.TabIndex = 12;
			this.label3.Text = "Arguments:";
			// 
			// _txtWorkingDirectory
			// 
			this._txtWorkingDirectory.Location = new System.Drawing.Point(120, 136);
			this._txtWorkingDirectory.Name = "_txtWorkingDirectory";
			this._txtWorkingDirectory.Size = new System.Drawing.Size(392, 20);
			this._txtWorkingDirectory.TabIndex = 4;
			this._txtWorkingDirectory.Text = "";
			this._ttToolTips.SetToolTip(this._txtWorkingDirectory, "Select the directory to start the program in");
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 136);
			this.label4.Name = "label4";
			this.label4.TabIndex = 13;
			this.label4.Text = "Working directory:";
			// 
			// _btnBrowseWorkingDirectory
			// 
			this._btnBrowseWorkingDirectory.Location = new System.Drawing.Point(520, 136);
			this._btnBrowseWorkingDirectory.Name = "_btnBrowseWorkingDirectory";
			this._btnBrowseWorkingDirectory.TabIndex = 5;
			this._btnBrowseWorkingDirectory.Text = "Browse...";
			this._btnBrowseWorkingDirectory.Click += new System.EventHandler(this._btnBrowseWorkingDirectory_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(216, 23);
			this.label2.TabIndex = 18;
			this.label2.Text = "Locate the application you wish to profile:";
			// 
			// _btnCreateProject
			// 
			this._btnCreateProject.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnCreateProject.Location = new System.Drawing.Point(196, 368);
			this._btnCreateProject.Name = "_btnCreateProject";
			this._btnCreateProject.Size = new System.Drawing.Size(96, 24);
			this._btnCreateProject.TabIndex = 7;
			this._btnCreateProject.Text = "Create Project";
			this._btnCreateProject.Click += new System.EventHandler(this._btnCreateProject_Click);
			// 
			// _btnCancel
			// 
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.Location = new System.Drawing.Point(324, 368);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(96, 24);
			this._btnCancel.TabIndex = 8;
			this._btnCancel.Text = "Cancel";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(184, 24);
			this.label5.TabIndex = 19;
			this.label5.Text = "Select the type of project to profile:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _rbAspNet
			// 
			this._rbAspNet.Location = new System.Drawing.Point(8, 176);
			this._rbAspNet.Name = "_rbAspNet";
			this._rbAspNet.TabIndex = 21;
			this._rbAspNet.Text = "ASP.NET";
			this._rbAspNet.CheckedChanged += new System.EventHandler(this._rbProjectType_CheckedChanged);
			// 
			// _rbFile
			// 
			this._rbFile.Checked = true;
			this._rbFile.Location = new System.Drawing.Point(8, 32);
			this._rbFile.Name = "_rbFile";
			this._rbFile.TabIndex = 23;
			this._rbFile.TabStop = true;
			this._rbFile.Text = "File";
			this._rbFile.CheckedChanged += new System.EventHandler(this._rbProjectType_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this._cbDebugProfilee);
			this.groupBox1.Location = new System.Drawing.Point(8, 288);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(600, 72);
			this.groupBox1.TabIndex = 24;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Common Options";
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(8, 40);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(168, 24);
			this.checkBox1.TabIndex = 7;
			this.checkBox1.Text = "Start profiling immediately";
			// 
			// _rbRemote
			// 
			this._rbRemote.Enabled = false;
			this._rbRemote.Location = new System.Drawing.Point(8, 232);
			this._rbRemote.Name = "_rbRemote";
			this._rbRemote.Size = new System.Drawing.Size(136, 24);
			this._rbRemote.TabIndex = 21;
			this._rbRemote.Text = "Remote Connection";
			this._rbRemote.CheckedChanged += new System.EventHandler(this._rbProjectType_CheckedChanged);
			// 
			// _lblAspNet
			// 
			this._lblAspNet.Location = new System.Drawing.Point(24, 200);
			this._lblAspNet.Name = "_lblAspNet";
			this._lblAspNet.Size = new System.Drawing.Size(264, 23);
			this._lblAspNet.TabIndex = 25;
			this._lblAspNet.Text = "ASP.NET will be profiled when this project is run.";
			// 
			// label6
			// 
			this.label6.Enabled = false;
			this.label6.Location = new System.Drawing.Point(24, 256);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(152, 23);
			this.label6.TabIndex = 25;
			this.label6.Text = "Profile a remote application.";
			// 
			// ProfilerProjectOptionsForm
			// 
			this.AcceptButton = this._btnCreateProject;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(616, 399);
			this.Controls.Add(this._lblAspNet);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._rbFile);
			this.Controls.Add(this._rbAspNet);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnCreateProject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._txtArguments);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._btnBrowseWorkingDirectory);
			this.Controls.Add(this._txtWorkingDirectory);
			this.Controls.Add(this._btnBrowseApplication);
			this.Controls.Add(this._txtApplicationName);
			this.Controls.Add(this._rbRemote);
			this.Controls.Add(this.label6);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfilerProjectOptionsForm";
			this.ShowInTaskbar = false;
			this.Text = "Create Profiler Project";
			this.Load += new System.EventHandler(this.ProfilerProjectOptionsForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public ProfilerProjectMode Mode
		{
			get { return _ppm; }
			set { _ppm = value; }
		}

		public ProjectInfo Project
		{
			get { return _p; }
			set { _p = value; }
		}

		private void _btnBrowseApplication_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Executable files (*.exe)|*.exe";
			DialogResult dr = ofd.ShowDialog();
			if ( dr == DialogResult.OK )
			{
				_txtApplicationName.Text = ofd.FileName;
				_txtApplicationName.Focus();
				_txtApplicationName.SelectAll();
			}
		}

		private void _btnBrowseWorkingDirectory_Click(object sender, System.EventArgs e)
		{
			FolderNameDialog fb = new FolderNameDialog();
			if ( _txtWorkingDirectory.Text.Trim() == String.Empty )
			{
				if ( _txtApplicationName.Text.Trim() != String.Empty )
				{
					FileInfo fi = new FileInfo( _txtApplicationName.Text );
					//fb.StartLocation = fi.DirectoryName;
				}
			}
			else
			{
				//fb.StartLocation = _txtWorkingDirectory.Text;
			}

			if ( fb.ShowDialog( this ) == DialogResult.OK )
			{
				_txtWorkingDirectory.Text = fb.DirectoryPath;
				_txtWorkingDirectory.Focus();
				_txtWorkingDirectory.SelectAll();
			}
		}

		private void ProfilerProjectOptionsForm_Load(object sender, System.EventArgs e)
		{
			if ( _ppm == ProfilerProjectMode.CreateProject )
			{
				this.Text = "Create Profiler Project";
				_btnCreateProject.Text = "Create Project";
			}
			else
			{
				this.Text = "Modify Profiler Project Options";
				_btnCreateProject.Text = "Save Project";
			}

			_txtApplicationName.Text = _p.ApplicationName;
			_txtArguments.Text = _p.Arguments;
			_txtWorkingDirectory.Text = _p.WorkingDirectory;

			switch ( _p.ProjectType )
			{
				default:
				case ProjectType.File:
					_rbFile.Checked = true;
					break;
				case ProjectType.AspNet:
					_rbAspNet.Checked = true;
					break;
			}
		}

		private void _btnCreateProject_Click(object sender, System.EventArgs e)
		{
			_p.ApplicationName = _txtApplicationName.Text;
			_p.Arguments = _txtArguments.Text;
			_p.WorkingDirectory = _txtWorkingDirectory.Text;

			if ( _rbFile.Checked )
				_p.ProjectType = ProjectType.File;
			else if ( _rbAspNet.Checked )
				_p.ProjectType = ProjectType.AspNet;
		}

		private void _rbProjectType_CheckedChanged(object sender, System.EventArgs e)
		{
			_txtApplicationName.Enabled = _rbFile.Checked;
			_txtArguments.Enabled = _rbFile.Checked;
			_txtWorkingDirectory.Enabled = _rbFile.Checked;
			_btnBrowseApplication.Enabled = _rbFile.Checked;
			_btnBrowseWorkingDirectory.Enabled = _rbFile.Checked;
		}

		public enum ProfilerProjectMode
		{
			CreateProject,
			ModifyProject,
		}
	}
}
