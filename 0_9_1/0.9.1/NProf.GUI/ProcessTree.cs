using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NProf.Glue.Profiler.Info;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProcessTree.
	/// </summary>
	public class ProcessTree : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView _tvProcess;
		private System.Windows.Forms.ImageList _ilState;
		private System.ComponentModel.IContainer components;

		public ProcessTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public ProcessInfoCollection Processes
		{
			get { return _pic; }
			set { _pic = value; UpdateProcesses(); }
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

		private void UpdateProcesses()
		{
			_tvProcess.Nodes.Clear();
			if ( _pic == null )
				return;

			bool bFirst = true;

			foreach ( ProcessInfo pi in _pic )
			{
				TreeNode tnProcess = _tvProcess.Nodes.Add( pi.ToString() );
				tnProcess.ImageIndex = 0;
				tnProcess.SelectedImageIndex = 0;
				tnProcess.Tag = pi;

				foreach ( ThreadInfo ti in pi.Threads )
				{
					TreeNode tnThread = tnProcess.Nodes.Add( ti.ToString() );
					tnThread.ImageIndex = 1;
					tnThread.SelectedImageIndex = 1;
					tnThread.Tag = ti;

					if ( bFirst )
					{
						_tvProcess.SelectedNode = tnThread;
						bFirst = false;
					}
				}
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._tvProcess = new System.Windows.Forms.TreeView();
			this._ilState = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// _tvProcess
			// 
			this._tvProcess.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tvProcess.HideSelection = false;
			this._tvProcess.ImageList = this._ilState;
			this._tvProcess.Location = new System.Drawing.Point(0, 0);
			this._tvProcess.Name = "_tvProcess";
			this._tvProcess.Size = new System.Drawing.Size(150, 150);
			this._tvProcess.TabIndex = 0;
			this._tvProcess.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._tvProcess_AfterSelect);
			this._tvProcess.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._tvProcess_BeforeSelect);
			// 
			// _ilState
			// 
			this._ilState.ImageSize = new System.Drawing.Size(16, 16);
			this._ilState.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ProcessTree
			// 
			this.Controls.Add(this._tvProcess);
			this.Name = "ProcessTree";
			this.Load += new System.EventHandler(this.ProcessTree_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public ThreadSelectedHandler ThreadSelected;

		private void _tvProcess_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			// Don't allow selection on this tree unless it's a ThreadInfo
			if ( !( e.Node.Tag is ThreadInfo ) )
				e.Cancel = true;
		}

		private void _tvProcess_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if ( ThreadSelected != null )
				ThreadSelected( ( ThreadInfo )e.Node.Tag );
		}

		private void ProcessTree_Load(object sender, System.EventArgs e)
		{
			_ilState = new ImageList();
			_ilState.TransparentColor = Color.Magenta;
			
			_ilState.Images.Add( Image.FromStream( this.GetType().Assembly.GetManifestResourceStream( "NProf.GUI.Resources.process.bmp" ) ) );
			_ilState.Images.Add( Image.FromStream( this.GetType().Assembly.GetManifestResourceStream( "NProf.GUI.Resources.thread.bmp" ) ) );

			_tvProcess.ImageList = _ilState;
		}

		public delegate void ThreadSelectedHandler( ThreadInfo ti );

		private ProcessInfoCollection _pic;
	}
}
