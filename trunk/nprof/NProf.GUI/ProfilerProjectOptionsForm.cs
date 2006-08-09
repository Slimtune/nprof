using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Genghis.Windows.Forms;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Project;
using NProf.Utilities.DataStore;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for OpenProfilerProjectForm.
	/// </summary>
	public class ProfilerProjectOptionsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox _cbDebugProfilee;
		private System.Windows.Forms.TextBox _txtApplicationName;
		private System.Windows.Forms.TextBox _txtArguments;
		private System.Windows.Forms.Button _btnCreateProject;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.ToolTip _ttToolTips;
		private System.Windows.Forms.Button _btnBrowseApplication;
		private System.ComponentModel.IContainer components;

		private ProjectInfo _p;
		private CheckBox checkBox1;
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
			this._btnCreateProject = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this._ttToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _cbDebugProfilee
			// 
			this._cbDebugProfilee.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cbDebugProfilee.Location = new System.Drawing.Point(307, 62);
			this._cbDebugProfilee.Name = "_cbDebugProfilee";
			this._cbDebugProfilee.Size = new System.Drawing.Size(128, 24);
			this._cbDebugProfilee.TabIndex = 6;
			this._cbDebugProfilee.Text = "Debug profiler hook";
			this._ttToolTips.SetToolTip(this._cbDebugProfilee, "Launch the debugger as soon as the profilee starts");
			// 
			// _btnBrowseApplication
			// 
			this._btnBrowseApplication.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnBrowseApplication.Location = new System.Drawing.Point(437, 12);
			this._btnBrowseApplication.Name = "_btnBrowseApplication";
			this._btnBrowseApplication.Size = new System.Drawing.Size(75, 23);
			this._btnBrowseApplication.TabIndex = 3;
			this._btnBrowseApplication.Text = "Browse...";
			this._btnBrowseApplication.Click += new System.EventHandler(this._btnBrowseApplication_Click);
			// 
			// _txtApplicationName
			// 
			this._txtApplicationName.Location = new System.Drawing.Point(119, 12);
			this._txtApplicationName.Name = "_txtApplicationName";
			this._txtApplicationName.Size = new System.Drawing.Size(312, 20);
			this._txtApplicationName.TabIndex = 0;
			this._ttToolTips.SetToolTip(this._txtApplicationName, "Locate the execute to profile");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Application to run:";
			// 
			// _txtArguments
			// 
			this._txtArguments.Location = new System.Drawing.Point(119, 36);
			this._txtArguments.Name = "_txtArguments";
			this._txtArguments.Size = new System.Drawing.Size(312, 20);
			this._txtArguments.TabIndex = 1;
			this._ttToolTips.SetToolTip(this._txtArguments, "Enter any command line arguments to pass to the above executable");
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(15, 36);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Arguments:";
			// 
			// _btnCreateProject
			// 
			this._btnCreateProject.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnCreateProject.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCreateProject.Location = new System.Drawing.Point(307, 114);
			this._btnCreateProject.Name = "_btnCreateProject";
			this._btnCreateProject.Size = new System.Drawing.Size(96, 24);
			this._btnCreateProject.TabIndex = 2;
			this._btnCreateProject.Text = "OK";
			this._btnCreateProject.Click += new System.EventHandler(this._btnCreateProject_Click);
			// 
			// _btnCancel
			// 
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCancel.Location = new System.Drawing.Point(416, 114);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(96, 24);
			this._btnCancel.TabIndex = 3;
			this._btnCancel.Text = "Cancel";
			// 
			// checkBox1
			// 
			this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBox1.Location = new System.Drawing.Point(18, 62);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(272, 24);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "Use stochastic profiling (faster but less accurate)";
			this._ttToolTips.SetToolTip(this.checkBox1, "Launch the debugger as soon as the profilee starts");
			// 
			// ProfilerProjectOptionsForm
			// 
			this.AcceptButton = this._btnCreateProject;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(523, 150);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._cbDebugProfilee);
			this.Controls.Add(this._txtArguments);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._txtApplicationName);
			this.Controls.Add(this._btnCreateProject);
			this.Controls.Add(this._btnBrowseApplication);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfilerProjectOptionsForm";
			this.ShowInTaskbar = false;
			this.Text = "Create Profiler Project";
			this.Load += new System.EventHandler(this.ProfilerProjectOptionsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

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

		//private void _btnBrowseWorkingDirectory_Click(object sender, System.EventArgs e)
		//{
		//    FolderNameDialog fb = new FolderNameDialog();
		//    //if ( _txtWorkingDirectory.Text.Trim() == String.Empty )
		//    //{
		//    //    if ( _txtApplicationName.Text.Trim() != String.Empty )
		//    //    {
		//    //        FileInfo fi = new FileInfo( _txtApplicationName.Text );
		//    //        //fb.StartLocation = fi.DirectoryName;
		//    //    }
		//    //}
		//    //else
		//    //{
		//    //    //fb.StartLocation = _txtWorkingDirectory.Text;
		//    //}

		//    if ( fb.ShowDialog( this ) == DialogResult.OK )
		//    {
		//        _txtWorkingDirectory.Text = fb.DirectoryPath;
		//        _txtWorkingDirectory.Focus();
		//        _txtWorkingDirectory.SelectAll();
		//    }
		//}

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
			//_txtWorkingDirectory.Text = _p.WorkingDirectory;
			_cbDebugProfilee.Checked = _p.Options.Debug;

			//switch ( _p.ProjectType )
			//{
			//    default:
			//    case ProjectType.File:
			//        _rbFile.Checked = true;
			//        break;
			//    case ProjectType.AspNet:
			//        _rbAspNet.Checked = true;
			//        break;
			//}
		}

		private void _btnCreateProject_Click(object sender, System.EventArgs e)
		{
			_p.ApplicationName = _txtApplicationName.Text;
			_p.Arguments = _txtArguments.Text;
			//_p.WorkingDirectory = _txtWorkingDirectory.Text;
			_p.Options.Debug = _cbDebugProfilee.Checked;

			//if ( _rbFile.Checked )
			//    _p.ProjectType = ProjectType.File;
			//else if ( _rbAspNet.Checked )
			//    _p.ProjectType = ProjectType.AspNet;
		}

		//private void _rbProjectType_CheckedChanged(object sender, System.EventArgs e)
		//{
		//    //_txtApplicationName.Enabled = _rbFile.Checked;
		//    //_txtArguments.Enabled = _rbFile.Checked;
		//    //_txtWorkingDirectory.Enabled = _rbFile.Checked;
		//    //_btnBrowseApplication.Enabled = _rbFile.Checked;
		//    //_btnBrowseWorkingDirectory.Enabled = _rbFile.Checked;
		//}

		public enum ProfilerProjectMode
		{
			CreateProject,
			ModifyProject,
		}
	}
}
