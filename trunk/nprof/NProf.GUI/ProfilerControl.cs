using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;
using Crownwood.Magic.Menus;
using Genghis.Windows.Forms;
using DotNetLib.Windows.Forms;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProfilerControl.
	/// </summary>
	[ProgId( "NProf.ProfilerControl" ), 
	ClassInterface( ClassInterfaceType.AutoDual ),
	Guid( "D34B8507-C286-4d40-83BC-0852E19DEC89" )]
	public class ProfilerControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TreeView _tvNamespaceInfo;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionChildren;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionSuspended;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCalleeParent;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerParent;
		private System.Windows.Forms.Timer _tmrFilterThrottle;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label _lblFilterSignatures;
		private System.Windows.Forms.TextBox _txtFilterBar;
		private System.Windows.Forms.Panel panel3;
		private DotNetLib.Windows.Forms.ContainerListView _lvFunctionInfo;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TabControl _tcCalls;
		private System.Windows.Forms.TabPage _tpCallees;
		private DotNetLib.Windows.Forms.ContainerListView _lvCalleesInfo;
		private System.Windows.Forms.TabPage _tcCallers;
		private DotNetLib.Windows.Forms.ContainerListView _lvCallersInfo;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Splitter splitter3;
		private NProf.GUI.ProcessTree _ptProcessTree;
		private System.ComponentModel.IContainer components;

		public ProfilerControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			_htCheckedItems = new Hashtable();
			_bUpdating = false;
			_bInCheck = false;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this._lvFunctionInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colFunctionID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionChildren = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionSuspended = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this._tcCalls = new System.Windows.Forms.TabControl();
			this._tpCallees = new System.Windows.Forms.TabPage();
			this._lvCalleesInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colCalleeID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this._tcCallers = new System.Windows.Forms.TabPage();
			this._lvCallersInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colCallerID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this._txtFilterBar = new System.Windows.Forms.TextBox();
			this._lblFilterSignatures = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this._tvNamespaceInfo = new System.Windows.Forms.TreeView();
			this._ptProcessTree = new NProf.GUI.ProcessTree();
			this._tmrFilterThrottle = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this._tcCalls.SuspendLayout();
			this._tpCallees.SuspendLayout();
			this._tcCallers.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
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
			this.panel1.Size = new System.Drawing.Size(896, 568);
			this.panel1.TabIndex = 10;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this._lvFunctionInfo);
			this.panel3.Controls.Add(this.splitter2);
			this.panel3.Controls.Add(this._tcCalls);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(203, 24);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(693, 544);
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
			this._lvFunctionInfo.Size = new System.Drawing.Size(693, 365);
			this._lvFunctionInfo.TabIndex = 24;
			this._lvFunctionInfo.SelectedItemsChanged += new System.EventHandler(this._lvFunctionInfo_SelectedItemsChanged);
			// 
			// colFunctionID
			// 
			this.colFunctionID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colFunctionID.Text = "ID";
			this.colFunctionID.ToolTip = "ID Tool Tip";
			this.colFunctionID.Width = 100;
			// 
			// colFunctionSignature
			// 
			this.colFunctionSignature.DisplayIndex = 1;
			this.colFunctionSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colFunctionSignature.Text = "Signature";
			this.colFunctionSignature.ToolTip = "Signature Tool Tip";
			this.colFunctionSignature.Width = 350;
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
			this.splitter2.Location = new System.Drawing.Point(0, 365);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(693, 3);
			this.splitter2.TabIndex = 23;
			this.splitter2.TabStop = false;
			// 
			// _tcCalls
			// 
			this._tcCalls.Controls.Add(this._tpCallees);
			this._tcCalls.Controls.Add(this._tcCallers);
			this._tcCalls.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._tcCalls.Location = new System.Drawing.Point(0, 368);
			this._tcCalls.Name = "_tcCalls";
			this._tcCalls.SelectedIndex = 0;
			this._tcCalls.Size = new System.Drawing.Size(693, 176);
			this._tcCalls.TabIndex = 22;
			// 
			// _tpCallees
			// 
			this._tpCallees.Controls.Add(this._lvCalleesInfo);
			this._tpCallees.Location = new System.Drawing.Point(4, 22);
			this._tpCallees.Name = "_tpCallees";
			this._tpCallees.Size = new System.Drawing.Size(685, 150);
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
			this._lvCalleesInfo.Size = new System.Drawing.Size(685, 150);
			this._lvCalleesInfo.TabIndex = 17;
			this._lvCalleesInfo.DoubleClick += new System.EventHandler(this._lvChildInfo_DoubleClick);
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
			// _tcCallers
			// 
			this._tcCallers.Controls.Add(this._lvCallersInfo);
			this._tcCallers.Location = new System.Drawing.Point(4, 22);
			this._tcCallers.Name = "_tcCallers";
			this._tcCallers.Size = new System.Drawing.Size(685, 150);
			this._tcCallers.TabIndex = 1;
			this._tcCallers.Text = "Callers";
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
			this._lvCallersInfo.Size = new System.Drawing.Size(685, 150);
			this._lvCallersInfo.TabIndex = 18;
			this._lvCallersInfo.DoubleClick += new System.EventHandler(this._lvChildInfo_DoubleClick);
			// 
			// colCallerID
			// 
			this.colCallerID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colCallerID.Text = "ID";
			this.colCallerID.ToolTip = "ID Tool Tip";
			this.colCallerID.Width = 100;
			// 
			// colCallerSignature
			// 
			this.colCallerSignature.DisplayIndex = 1;
			this.colCallerSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colCallerSignature.Text = "Signature";
			this.colCallerSignature.ToolTip = "Signature Tool Tip";
			this.colCallerSignature.Width = 400;
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
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 24);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 544);
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
			this.panel2.Size = new System.Drawing.Size(696, 24);
			this.panel2.TabIndex = 12;
			// 
			// _txtFilterBar
			// 
			this._txtFilterBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtFilterBar.Location = new System.Drawing.Point(88, 0);
			this._txtFilterBar.Multiline = true;
			this._txtFilterBar.Name = "_txtFilterBar";
			this._txtFilterBar.Size = new System.Drawing.Size(592, 24);
			this._txtFilterBar.TabIndex = 13;
			this._txtFilterBar.TextChanged += new System.EventHandler(this._txtFilterBar_TextChanged);
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
			// panel4
			// 
			this.panel4.Controls.Add(this.splitter3);
			this.panel4.Controls.Add(this._tvNamespaceInfo);
			this.panel4.Controls.Add(this._ptProcessTree);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(200, 568);
			this.panel4.TabIndex = 15;
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
			this._tvNamespaceInfo.Size = new System.Drawing.Size(200, 418);
			this._tvNamespaceInfo.TabIndex = 8;
			this._tvNamespaceInfo.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this._tvNamespaceInfo_AfterCheck);
			this._tvNamespaceInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._tvNamespaceInfo_AfterSelect);
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
			// _tmrFilterThrottle
			// 
			this._tmrFilterThrottle.Interval = 300;
			this._tmrFilterThrottle.Tick += new System.EventHandler(this._tmrFilterThrottle_Tick);
			// 
			// ProfilerControl
			// 
			this.Controls.Add(this.panel1);
			this.Name = "ProfilerControl";
			this.Size = new System.Drawing.Size(896, 568);
			this.Load += new System.EventHandler(this.ProfilerControl_Load);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this._tcCalls.ResumeLayout(false);
			this._tpCallees.ResumeLayout(false);
			this._tcCallers.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

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
			UpdateTree();
			UpdateFilters();
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
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
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
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
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


		private void _tvNamespaceInfo_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateFilters();
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

				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
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

		#region Context Menus

		#region Headers

		private void _lv_HeaderMenuEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ContainerListView listView = sender as ContainerListView;

			PopupMenu pop = new PopupMenu();
			pop.Selected += new CommandHandler(pop_Selected);
			pop.Deselected += new CommandHandler(pop_Deselected);

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
				//				sortByItem.Click += new EventHandler(sortByItem_Click);

				if(sortByItem.Checked)
					isAscending = hdr.SortOrder == SortOrder.Ascending;

				sortBy.MenuCommands.Add(sortByItem);
			}

			sortBy.MenuCommands.Add(new MenuCommand("-"));
			MenuCommand ascending = sortBy.MenuCommands.Add(new MenuCommand("&Ascending"));
			ascending.RadioCheck = true;
			ascending.Checked = isAscending;
			ascending.Tag = new object[] { listView, SortOrder.Ascending };
			//			ascending.Click += new EventHandler(sortOrder_Click);

			MenuCommand descending = sortBy.MenuCommands.Add(new MenuCommand("&Descending"));
			descending.RadioCheck = true;
			descending.Checked = !isAscending;
			descending.Tag = new object[] { listView, SortOrder.Descending };
			//			descending.Click += new EventHandler(sortOrder_Click);

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

		//		private void sortOrder_Click(object sender, EventArgs e)
		//		{
		//			MenuCommand cmd = sender as MenuCommand;
		//
		//			object[] objs = (cmd.Tag as object[]);
		//			ContainerListView listView = objs[0] as ContainerListView;
		//
		//			listView.Sort((SortOrder)objs[1]);
		//		}

		//		private void sortByItem_Click(object sender, EventArgs e)
		//		{
		//			MenuCommand cmd = sender as MenuCommand;
		//
		//			object[] objs = (cmd.Tag as object[]);
		//			ContainerListView listView = objs[0] as ContainerListView;
		//
		//			listView.Sort((int)objs[1], false);
		//		}

		#endregion

		private void pop_Selected(MenuCommand item)
		{
			// TODO: Update status bar with item.Description
		}

		private void pop_Deselected(MenuCommand item)
		{
			// TODO: Update status bar with string.Empty
		}

		#endregion
	}
}
