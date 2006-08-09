using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;



namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProcessTree.
	/// </summary>
	public class ProcessTree : System.Windows.Forms.UserControl
	{
		public System.Windows.Forms.TreeView processView;
		private System.Windows.Forms.ImageList stateImages;
		private System.ComponentModel.IContainer components;

		public ProcessTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public ProcessInfoCollection Processes
		{
			get { return processes; }
			set { processes = value; UpdateProcesses(); }
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void UpdateProcesses()
		{
			processView.Nodes.Clear();
			if (processes == null)
				return;

			bool bFirst = true;

			foreach (ProcessInfo pi in processes)
			{
				TreeNode tnProcess = processView.Nodes.Add(pi.ToString());
				tnProcess.ImageIndex = 0;
				tnProcess.SelectedImageIndex = 0;
				tnProcess.Tag = pi;

				foreach (ThreadInfo ti in pi.Threads)
				{
					TreeNode tnThread = tnProcess.Nodes.Add(ti.ToString());
					tnThread.ImageIndex = 1;
					tnThread.SelectedImageIndex = 1;
					tnThread.Tag = ti;

					if (bFirst)
					{
						processView.SelectedNode = tnThread;
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
			this.processView = new System.Windows.Forms.TreeView();
			this.stateImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// _tvProcess
			// 
			this.processView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.processView.HideSelection = false;
			this.processView.ImageList = this.stateImages;
			this.processView.Location = new System.Drawing.Point(0, 0);
			this.processView.Name = "_tvProcess";
			this.processView.Size = new System.Drawing.Size(150, 150);
			this.processView.TabIndex = 0;
			this.processView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._tvProcess_AfterSelect);
			this.processView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._tvProcess_BeforeSelect);
			// 
			// _ilState
			// 
			this.stateImages.ImageSize = new System.Drawing.Size(16, 16);
			this.stateImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ProcessTree
			// 
			this.Controls.Add(this.processView);
			this.Name = "ProcessTree";
			this.Load += new System.EventHandler(this.ProcessTree_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public ThreadSelectedHandler ThreadSelected;

		private void _tvProcess_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			// Don't allow selection on this tree unless it's a ThreadInfo
			if (!(e.Node.Tag is ThreadInfo))
				e.Cancel = true;
		}

		private void _tvProcess_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (ThreadSelected != null)
				ThreadSelected((ThreadInfo)e.Node.Tag);
		}

		private void ProcessTree_Load(object sender, System.EventArgs e)
		{
			stateImages = new ImageList();
			stateImages.TransparentColor = Color.Magenta;

			stateImages.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.process.bmp")));
			stateImages.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.thread.bmp")));

			processView.ImageList = stateImages;
		}

		public delegate void ThreadSelectedHandler(ThreadInfo ti);

		private ProcessInfoCollection processes;





		public Run GetSelectedRun()
		{
			TreeNode tnSelected = processView.SelectedNode;// ; _tvProjects.SelectedNode;
			//TreeNode tnSelected = _tvProjects.SelectedNode;
			if (tnSelected == null || !(tnSelected.Tag is Run))
				return null;

			return (tnSelected.Tag as Run);
		}
		public void SelectRun(Run run)
		{
			processView.SelectedNode = FindRunNode(run);
			//_tvProjects.SelectedNode = FindRunNode(run);
		}
		private TreeNode FindRunNode(Run run)
		{
			foreach (TreeNode tnRun in processView.Nodes)
				if (tnRun.Tag == run)
					return tnRun;

			return null;
		}


		public void AddRunNode(Run run)
		{
			//_tvProjects.Invoke(new MethodInvoker(delegate()
			//{
			TreeNode node = new TreeNode(run.StartTime.ToString());
			node.ImageIndex = GetRunStateImage(run);
			node.SelectedImageIndex = GetRunStateImage(run);
			node.Tag = run;
			processView.Nodes.Add(node);
			//_tvProjects.Nodes.Add(node);
			//}));

			run.StateChanged += new Run.RunStateEventHandler(OnRunStateChanged);
		}




		private void OnRunAdded(ProjectInfo pi, RunCollection runs, Run run, int nIndex)
		{
			foreach (TreeNode tn in processView.Nodes)
			{
				if (tn.Tag == pi)
					AddRunNode(run);
				//AddRunNode(tn, run);
			}
		}

		private void OnRunRemoved(ProjectInfo pi, RunCollection runs, Run run, int nIndex)
		{
			//_tvProjects.Nodes.RemoveAt( nIndex );
		}

		private void OnRunStateChanged(Run run, Run.RunState rsOld, Run.RunState rsNew)
		{
			TreeNode tn = FindRunNode(run);
			if (tn != null) // why?
			{
				processView.Invoke(new TreeNodeSetState(OnTreeNodeSetState), new object[] { tn, run.StartTime.ToString(), GetRunStateImage(run) });
			}
			//_tvProjects.Invoke(new TreeNodeSetState(OnTreeNodeSetState), new object[] { tn, run.StartTime.ToString(), GetRunStateImage(run) });

			//TreeNode tn = FindRunNode(run);
			//_tvProjects.Invoke(new TreeNodeSetState(OnTreeNodeSetState), new object[] { tn, run.StartTime.ToString(), GetRunStateImage(run) });
		}

		private int GetRunStateImage(Run r)
		{
			switch (r.State)
			{
				case Run.RunState.Initializing:
					return 1;
				case Run.RunState.Running:
					return 2;
				case Run.RunState.Finished:
					return r.Success ? 3 : 4;
			}

			return 0;
		}

		//private void ProjectTree_Load(object sender, System.EventArgs e)
		//{
		//    //_ilState = new ImageList();
		//    //_ilState.TransparentColor = Color.Magenta;

		//    //_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.project.bmp")));
		//    //_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.initializing.bmp")));
		//    //_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.go.bmp")));
		//    //_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.stop.bmp")));
		//    //_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.stop-error.bmp")));

		//    //_tvProjects.ImageList = _ilState;
		//}

		//private void ProjectTree_DoubleClick(object sender, System.EventArgs e)
		//{
		//    Run r = GetSelectedRun();
		//    if (r != null)
		//    {
		//        if (RunDoubleClicked != null)
		//            RunDoubleClicked(r);

		//        return;
		//    }
		//}


		private void OnTreeNodeSetState(TreeNode tn, string strLabel, int nImageIndex)
		{
			tn.Text = strLabel;
			tn.ImageIndex = nImageIndex;
			tn.SelectedImageIndex = nImageIndex;
		}

		public event ProjectDoubleClickedHandler ProjectDoubleClicked;
		public event RunDoubleClickedHandler RunDoubleClicked;

		public delegate void ProjectDoubleClickedHandler(ProjectInfo proj);
		public delegate void RunDoubleClickedHandler(Run run);

		private delegate TreeNode TreeNodeAdd(TreeNodeCollection tncParent, string strLabel, int nImageIndex, object oTag);
		private delegate void TreeNodeRemove(TreeNode tnChild);
		private delegate void TreeNodeSetState(TreeNode tn, string strLabel, int nImageIndex);
		private delegate void TreeNodeUpdate(TreeNode tn);
	}
}