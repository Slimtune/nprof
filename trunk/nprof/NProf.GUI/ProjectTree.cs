using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NProf.Glue.Profiler.Project;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProjectTree.
	/// </summary>
	public class ProjectTree : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView _tvProjects;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList _ilState;

		private ProjectInfoCollection _pic;

		public ProjectTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			_pic = null;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProjectTree));
			this._tvProjects = new System.Windows.Forms.TreeView();
			this._ilState = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// _tvProjects
			// 
			this._tvProjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tvProjects.HideSelection = false;
			this._tvProjects.ImageList = this._ilState;
			this._tvProjects.Name = "_tvProjects";
			this._tvProjects.Size = new System.Drawing.Size(344, 464);
			this._tvProjects.TabIndex = 0;
			// 
			// _ilState
			// 
			this._ilState.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._ilState.ImageSize = new System.Drawing.Size(16, 16);
			this._ilState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_ilState.ImageStream")));
			this._ilState.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// ProjectTree
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._tvProjects});
			this.Name = "ProjectTree";
			this.Size = new System.Drawing.Size(344, 464);
			this.ResumeLayout(false);

		}
		#endregion

		public ProjectInfoCollection Projects
		{
			get { return _pic; }
			set { _pic = value; UpdateProjects(); }
		}

		public ProjectInfo GetSelectedProject()
		{
			TreeNode tnSelected = _tvProjects.SelectedNode;
			if ( tnSelected == null )
				return null;

			if ( tnSelected.Tag is ProjectInfo )
				return ( ProjectInfo )tnSelected.Tag;

			return ( tnSelected.Parent.Tag as ProjectInfo );
		}

		public void SelectProject( ProjectInfo pi )
		{
			_tvProjects.SelectedNode = FindProjectNode( pi );
		}

		public void SelectRun( Run run )
		{
			_tvProjects.SelectedNode = FindRunNode( run );
		}

		private TreeNode FindProjectNode( ProjectInfo pi )
		{
			foreach ( TreeNode tn in _tvProjects.Nodes )
				if ( tn.Tag == pi )
					return tn;

			return null;
		}

		private TreeNode FindRunNode( Run run )
		{
			TreeNode tnProject = FindProjectNode( run.Project );
			if ( tnProject == null )
				return null;

			foreach ( TreeNode tnRun in tnProject.Nodes )
				if ( tnRun.Tag == run )
					return tnRun;

			return null;
		}

		private void UpdateProjects()
		{
			// Clear the tree
			_tvProjects.Nodes.Clear();

			foreach ( ProjectInfo pi in _pic )
			{
				AddProjectNode( pi );
			}

			_pic.ProjectAdded += new ProjectInfoCollection.ProjectEventHandler( OnProjectAdded );
			_pic.ProjectRemoved += new ProjectInfoCollection.ProjectEventHandler( OnProjectAdded );
		}

		private void AddProjectNode( ProjectInfo pi )
		{
			TreeNode tn = ( TreeNode )_tvProjects.Invoke( new TreeNodeAdd( OnTreeNodeAdd ), new object[]{ _tvProjects.Nodes, pi.Name, 0, pi } );

			pi.Runs.RunAdded += new RunCollection.RunEventHandler( OnRunAdded );
			pi.Runs.RunRemoved += new RunCollection.RunEventHandler( OnRunRemoved );

			foreach ( Run run in pi.Runs )
			{
				AddRunNode( tn, run );
			}
		}

		private void AddRunNode( TreeNode tnProject, Run run )
		{
			_tvProjects.Invoke( new TreeNodeAdd( OnTreeNodeAdd ), new object[]{ tnProject.Nodes, run.StartTime.ToString(), GetRunStateImage( run.State ), run } );

			run.StateChanged += new Run.RunStateEventHandler( OnRunStateChanged );
		}

		private TreeNode OnTreeNodeAdd( TreeNodeCollection tncParent, string strLabel, int nImageIndex, object oTag )
		{
			TreeNode tnChild = new TreeNode( strLabel );
			tnChild.ImageIndex = nImageIndex;
			tnChild.SelectedImageIndex = nImageIndex;
			tnChild.Tag = oTag;
			tncParent.Add( tnChild );

			return tnChild;
		}

		private void OnTreeNodeRemove( TreeNode tnChild )
		{
			tnChild.Remove();
		}

		private void OnTreeNodeSetState( TreeNode tn, string strLabel, int nImageIndex )
		{
			tn.Text = strLabel;
			tn.ImageIndex = nImageIndex;
			tn.SelectedImageIndex = nImageIndex;
		}

		private void OnProjectAdded( ProjectInfoCollection projects, ProjectInfo pi, int nIndex )
		{
			AddProjectNode( pi );
		}

		private void OnProjectRemoved( ProjectInfoCollection projects, ProjectInfo pi, int nIndex )
		{
			_tvProjects.Invoke( new TreeNodeRemove( OnTreeNodeRemove ), new object[]{ _tvProjects.Nodes[ nIndex ] } );
		}

		private void OnRunAdded( ProjectInfo pi, RunCollection runs, Run run, int nIndex )
		{
			foreach ( TreeNode tn in _tvProjects.Nodes )
			{
				if ( tn.Tag == pi )
					AddRunNode( tn, run );
			}
		}

		private void OnRunRemoved( ProjectInfo pi, RunCollection runs, Run run, int nIndex )
		{
			//_tvProjects.Nodes.RemoveAt( nIndex );
		}

		private void OnRunStateChanged( Run run, Run.RunState rsOld, Run.RunState rsNew )
		{
			TreeNode tn = FindRunNode( run );
			_tvProjects.Invoke( new TreeNodeSetState( OnTreeNodeSetState ), new object[]{ tn, run.StartTime.ToString(), GetRunStateImage( run.State ) } );
		}

		private int GetRunStateImage( Run.RunState rs )
		{
			switch ( rs )
			{
				case Run.RunState.Running:
					return 2;
				case Run.RunState.Finished:
					return 1;
			}

			return 0;
		}

		private delegate TreeNode TreeNodeAdd( TreeNodeCollection tncParent, string strLabel, int nImageIndex, object oTag );
		private delegate void TreeNodeRemove( TreeNode tnChild );
		private delegate void TreeNodeSetState( TreeNode tn, string strLabel, int nImageIndex );
	}
}