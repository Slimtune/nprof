using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UtilityLibrary.WinControls;
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

			_p = new ProjectInfo();
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
			this.SuspendLayout();
			// 
			// _cbDebugProfilee
			// 
			this._cbDebugProfilee.Location = new System.Drawing.Point(16, 128);
			this._cbDebugProfilee.Name = "_cbDebugProfilee";
			this._cbDebugProfilee.TabIndex = 6;
			this._cbDebugProfilee.Text = "Debug profilee";
			this._ttToolTips.SetToolTip(this._cbDebugProfilee, "Launch the debugger as soon as the profilee starts");
			// 
			// _btnBrowseApplication
			// 
			this._btnBrowseApplication.Location = new System.Drawing.Point(528, 48);
			this._btnBrowseApplication.Name = "_btnBrowseApplication";
			this._btnBrowseApplication.TabIndex = 2;
			this._btnBrowseApplication.Text = "Browse...";
			this._btnBrowseApplication.Click += new System.EventHandler(this._btnBrowseApplication_Click);
			// 
			// _txtApplicationName
			// 
			this._txtApplicationName.Location = new System.Drawing.Point(104, 48);
			this._txtApplicationName.Name = "_txtApplicationName";
			this._txtApplicationName.Size = new System.Drawing.Size(408, 20);
			this._txtApplicationName.TabIndex = 1;
			this._txtApplicationName.Text = "";
			this._ttToolTips.SetToolTip(this._txtApplicationName, "Locate the execute to profile");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 48);
			this.label1.Name = "label1";
			this.label1.TabIndex = 14;
			this.label1.Text = "Application to run:";
			// 
			// _txtArguments
			// 
			this._txtArguments.Location = new System.Drawing.Point(104, 72);
			this._txtArguments.Name = "_txtArguments";
			this._txtArguments.Size = new System.Drawing.Size(408, 20);
			this._txtArguments.TabIndex = 3;
			this._txtArguments.Text = "";
			this._ttToolTips.SetToolTip(this._txtArguments, "Enter any command line arguments to pass to the above executable");
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 72);
			this.label3.Name = "label3";
			this.label3.TabIndex = 12;
			this.label3.Text = "Arguments:";
			// 
			// _txtWorkingDirectory
			// 
			this._txtWorkingDirectory.Location = new System.Drawing.Point(104, 96);
			this._txtWorkingDirectory.Name = "_txtWorkingDirectory";
			this._txtWorkingDirectory.Size = new System.Drawing.Size(408, 20);
			this._txtWorkingDirectory.TabIndex = 4;
			this._txtWorkingDirectory.Text = "";
			this._ttToolTips.SetToolTip(this._txtWorkingDirectory, "Select the directory to start the program in");
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 96);
			this.label4.Name = "label4";
			this.label4.TabIndex = 13;
			this.label4.Text = "Working directory:";
			// 
			// _btnBrowseWorkingDirectory
			// 
			this._btnBrowseWorkingDirectory.Location = new System.Drawing.Point(528, 96);
			this._btnBrowseWorkingDirectory.Name = "_btnBrowseWorkingDirectory";
			this._btnBrowseWorkingDirectory.TabIndex = 5;
			this._btnBrowseWorkingDirectory.Text = "Browse...";
			this._btnBrowseWorkingDirectory.Click += new System.EventHandler(this._btnBrowseWorkingDirectory_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(216, 23);
			this.label2.TabIndex = 18;
			this.label2.Text = "Locate the application you wish to profile:";
			// 
			// _btnCreateProject
			// 
			this._btnCreateProject.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnCreateProject.Location = new System.Drawing.Point(172, 160);
			this._btnCreateProject.Name = "_btnCreateProject";
			this._btnCreateProject.Size = new System.Drawing.Size(96, 24);
			this._btnCreateProject.TabIndex = 7;
			this._btnCreateProject.Text = "Create Project";
			this._btnCreateProject.Click += new System.EventHandler(this._btnCreateProject_Click);
			// 
			// _btnCancel
			// 
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.Location = new System.Drawing.Point(348, 160);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(96, 24);
			this._btnCancel.TabIndex = 8;
			this._btnCancel.Text = "Cancel";
			// 
			// ProfilerProjectOptionsForm
			// 
			this.AcceptButton = this._btnCreateProject;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(616, 191);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._btnCancel,
																		  this._btnCreateProject,
																		  this.label2,
																		  this._cbDebugProfilee,
																		  this._btnBrowseApplication,
																		  this._txtApplicationName,
																		  this.label1,
																		  this._txtArguments,
																		  this.label3,
																		  this._txtWorkingDirectory,
																		  this.label4,
																		  this._btnBrowseWorkingDirectory});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfilerProjectOptionsForm";
			this.ShowInTaskbar = false;
			this.Text = "Create Profiler Project";
			this.Load += new System.EventHandler(this.ProfilerProjectOptionsForm_Load);
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
			ShellFolderBrowser fb = new ShellFolderBrowser();
			if ( _txtWorkingDirectory.Text.Trim() == String.Empty )
			{
				if ( _txtApplicationName.Text.Trim() != String.Empty )
				{
					FileInfo fi = new FileInfo( _txtApplicationName.Text );
					fb.FolderPath = fi.DirectoryName;
				}
			}
			else
			{
				fb.FolderPath = _txtWorkingDirectory.Text;
			}

			if ( fb.ShowDialog() )
			{
				_txtWorkingDirectory.Text = fb.FolderPath;
				_txtWorkingDirectory.Focus();
				_txtWorkingDirectory.SelectAll();
			}
		}

		private void ProfilerProjectOptionsForm_Load(object sender, System.EventArgs e)
		{
			if ( _ppm == ProfilerProjectMode.CreateProject )
			{
				this.Text = "Create Profiler Project";
			}
			else
			{
				this.Text = "Modify Profiler Project Options";
			}

			_txtApplicationName.Text = _p.ApplicationName;
			_txtArguments.Text = _p.Arguments;
			_txtWorkingDirectory.Text = _p.WorkingDirectory;
		}

		private void _btnCreateProject_Click(object sender, System.EventArgs e)
		{
			_p.ApplicationName = _txtApplicationName.Text;
			_p.Arguments = _txtArguments.Text;
			_p.WorkingDirectory = _txtWorkingDirectory.Text;
		}

		public enum ProfilerProjectMode
		{
			CreateProject,
			ModifyProject,
		}
	}
}
