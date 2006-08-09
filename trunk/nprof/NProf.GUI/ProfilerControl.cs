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
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colFunctionTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerID;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerSignature;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerCalls;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerTotal;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader colCallerParent;
		private System.Windows.Forms.Timer _tmrFilterThrottle;
		private System.Windows.Forms.Panel panel3;
		private DotNetLib.Windows.Forms.ContainerListView _lvFunctionInfo;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel4;
		public NProf.GUI.ProcessTree _ptProcessTree;
		private TabPage tabPage1;
		private ContainerListView containerListView1;
		private ContainerListViewColumnHeader containerListViewColumnHeader1;
		private ContainerListViewColumnHeader containerListViewColumnHeader2;
		private ContainerListViewColumnHeader containerListViewColumnHeader3;
		private ContainerListViewColumnHeader containerListViewColumnHeader4;
		private ContainerListViewColumnHeader containerListViewColumnHeader5;
		private TabPage tabPage2;
		private ContainerListView containerListView2;
		private ContainerListViewColumnHeader containerListViewColumnHeader6;
		private ContainerListViewColumnHeader containerListViewColumnHeader7;
		private ContainerListViewColumnHeader containerListViewColumnHeader8;
		private ContainerListViewColumnHeader containerListViewColumnHeader9;
		private ContainerListViewColumnHeader containerListViewColumnHeader10;
		private ContainerListView callees;
		private ContainerListViewColumnHeader containerListViewColumnHeader12;
		private ContainerListViewColumnHeader containerListViewColumnHeader13;
		private ContainerListViewColumnHeader containerListViewColumnHeader14;
		private ContainerListViewColumnHeader containerListViewColumnHeader15;
		private ContainerListViewColumnHeader colCalleeParent;
		private ContainerListViewColumnHeader colCalleeTotal;
		private ContainerListViewColumnHeader colCalleeCalls;
		private ContainerListViewColumnHeader colCalleeSignature;
		private ContainerListViewColumnHeader colCalleeID;
		private ContainerListView callers;
		private ContainerListViewColumnHeader containerListViewColumnHeader17;
		private ContainerListViewColumnHeader containerListViewColumnHeader18;
		private ContainerListViewColumnHeader containerListViewColumnHeader19;
		private ContainerListViewColumnHeader containerListViewColumnHeader20;
		private Splitter splitter3;
		private Splitter splitter4;
		private System.ComponentModel.IContainer components;

		public ProfilerControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			_htCheckedItems = new Hashtable();
			_bUpdating = false;
			_bInCheck = false;



			ImageList _ilState = new ImageList();
			_ilState.TransparentColor = Color.Magenta;

			_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.project.bmp")));
			_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.initializing.bmp")));
			_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.go.bmp")));
			_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.stop.bmp")));
			_ilState.Images.Add(Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("NProf.GUI.Resources.stop-error.bmp")));

			_ptProcessTree.processView.ImageList = _ilState;

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
			this.callers = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader17 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader18 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader19 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader20 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.callees = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader12 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader13 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader14 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader15 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter4 = new System.Windows.Forms.Splitter();
			this._lvFunctionInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colFunctionSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colFunctionTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel4 = new System.Windows.Forms.Panel();
			this.colCallerID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCallerParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this._tmrFilterThrottle = new System.Windows.Forms.Timer(this.components);
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.containerListView1 = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader1 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader2 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader3 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader4 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader5 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.containerListView2 = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader6 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader7 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader8 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader9 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader10 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeParent = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeTotal = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeCalls = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeSignature = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.colCalleeID = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this._ptProcessTree = new NProf.GUI.ProcessTree();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.panel4);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(896, 568);
			this.panel1.TabIndex = 10;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.callers);
			this.panel3.Controls.Add(this.splitter3);
			this.panel3.Controls.Add(this.splitter2);
			this.panel3.Controls.Add(this.callees);
			this.panel3.Controls.Add(this.splitter4);
			this.panel3.Controls.Add(this._lvFunctionInfo);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(203, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(693, 568);
			this.panel3.TabIndex = 13;
			// 
			// callers
			// 
			this.callers.AllowColumnReorder = true;
			this.callers.CaptureFocusClick = false;
			this.callers.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader17,
            this.containerListViewColumnHeader18,
            this.containerListViewColumnHeader19,
            this.containerListViewColumnHeader20});
			this.callers.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.callers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.callers.HeaderHeight = 33;
			this.callers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.callers.HideSelection = false;
			this.callers.Location = new System.Drawing.Point(0, 396);
			this.callers.Name = "callers";
			this.callers.Size = new System.Drawing.Size(693, 169);
			this.callers.TabIndex = 26;
			// 
			// containerListViewColumnHeader17
			// 
			this.containerListViewColumnHeader17.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader17.Text = "Method";
			this.containerListViewColumnHeader17.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader17.Width = 400;
			// 
			// containerListViewColumnHeader18
			// 
			this.containerListViewColumnHeader18.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader18.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader18.DisplayIndex = 1;
			this.containerListViewColumnHeader18.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader18.Text = "Calls";
			this.containerListViewColumnHeader18.ToolTip = "# of Calls Tool Tip";
			this.containerListViewColumnHeader18.Width = 70;
			// 
			// containerListViewColumnHeader19
			// 
			this.containerListViewColumnHeader19.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader19.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader19.DisplayIndex = 2;
			this.containerListViewColumnHeader19.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader19.Text = "Time";
			this.containerListViewColumnHeader19.ToolTip = "% of Total Tool Tip";
			this.containerListViewColumnHeader19.Width = 70;
			// 
			// containerListViewColumnHeader20
			// 
			this.containerListViewColumnHeader20.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader20.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader20.DisplayIndex = 3;
			this.containerListViewColumnHeader20.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader20.Text = "% of Parent";
			this.containerListViewColumnHeader20.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader20.Width = 70;
			// 
			// splitter3
			// 
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(0, 392);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(693, 4);
			this.splitter3.TabIndex = 27;
			this.splitter3.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 565);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(693, 3);
			this.splitter2.TabIndex = 23;
			this.splitter2.TabStop = false;
			// 
			// callees
			// 
			this.callees.AllowColumnReorder = true;
			this.callees.CaptureFocusClick = false;
			this.callees.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader12,
            this.containerListViewColumnHeader13,
            this.containerListViewColumnHeader14,
            this.containerListViewColumnHeader15});
			this.callees.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.callees.Dock = System.Windows.Forms.DockStyle.Top;
			this.callees.HeaderHeight = 33;
			this.callees.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.callees.HideSelection = false;
			this.callees.Location = new System.Drawing.Point(0, 255);
			this.callees.Name = "callees";
			this.callees.Size = new System.Drawing.Size(693, 137);
			this.callees.TabIndex = 25;
			// 
			// containerListViewColumnHeader12
			// 
			this.containerListViewColumnHeader12.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader12.Text = "Method";
			this.containerListViewColumnHeader12.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader12.Width = 400;
			// 
			// containerListViewColumnHeader13
			// 
			this.containerListViewColumnHeader13.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader13.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader13.DisplayIndex = 1;
			this.containerListViewColumnHeader13.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader13.Text = "Calls";
			this.containerListViewColumnHeader13.ToolTip = "The number of times this method was called from the parent method ";
			this.containerListViewColumnHeader13.Width = 70;
			// 
			// containerListViewColumnHeader14
			// 
			this.containerListViewColumnHeader14.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader14.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader14.DisplayIndex = 2;
			this.containerListViewColumnHeader14.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader14.Text = "Time";
			this.containerListViewColumnHeader14.ToolTip = "Time spent in this method after it was called from the parent method";
			this.containerListViewColumnHeader14.Width = 70;
			// 
			// containerListViewColumnHeader15
			// 
			this.containerListViewColumnHeader15.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader15.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader15.DisplayIndex = 3;
			this.containerListViewColumnHeader15.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader15.Text = "% of Parent";
			this.containerListViewColumnHeader15.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader15.Width = 70;
			// 
			// splitter4
			// 
			this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter4.Location = new System.Drawing.Point(0, 252);
			this.splitter4.Name = "splitter4";
			this.splitter4.Size = new System.Drawing.Size(693, 3);
			this.splitter4.TabIndex = 28;
			this.splitter4.TabStop = false;
			// 
			// _lvFunctionInfo
			// 
			this._lvFunctionInfo.AllowColumnReorder = true;
			this._lvFunctionInfo.AllowMultiSelect = true;
			this._lvFunctionInfo.CaptureFocusClick = false;
			this._lvFunctionInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.colFunctionSignature,
            this.colFunctionCalls,
            this.colFunctionTotal});
			this._lvFunctionInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this._lvFunctionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lvFunctionInfo.HeaderHeight = 33;
			this._lvFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvFunctionInfo.HideSelection = false;
			this._lvFunctionInfo.Location = new System.Drawing.Point(0, 0);
			this._lvFunctionInfo.MultipleColumnSort = true;
			this._lvFunctionInfo.Name = "_lvFunctionInfo";
			this._lvFunctionInfo.Size = new System.Drawing.Size(693, 252);
			this._lvFunctionInfo.TabIndex = 24;
			this._lvFunctionInfo.SelectedItemsChanged += new System.EventHandler(this._lvFunctionInfo_SelectedItemsChanged);
			// 
			// colFunctionSignature
			// 
			this.colFunctionSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colFunctionSignature.Text = "Method";
			this.colFunctionSignature.ToolTip = "Signature Tool Tip";
			this.colFunctionSignature.Width = 350;
			// 
			// colFunctionCalls
			// 
			this.colFunctionCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionCalls.DisplayIndex = 1;
			this.colFunctionCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionCalls.Text = "Calls";
			this.colFunctionCalls.ToolTip = "The number of times this method was called";
			this.colFunctionCalls.Width = 70;
			// 
			// colFunctionTotal
			// 
			this.colFunctionTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colFunctionTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colFunctionTotal.DisplayIndex = 2;
			this.colFunctionTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colFunctionTotal.Text = "Time";
			this.colFunctionTotal.ToolTip = "The time, in percent, spent in this method";
			this.colFunctionTotal.Width = 70;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 568);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this._ptProcessTree);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(200, 568);
			this.panel4.TabIndex = 15;
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
			this.colCallerSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colCallerSignature.Text = "Method";
			this.colCallerSignature.ToolTip = "Signature Tool Tip";
			this.colCallerSignature.Width = 400;
			// 
			// colCallerCalls
			// 
			this.colCallerCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerCalls.Text = "# of Calls";
			this.colCallerCalls.ToolTip = "# of Calls Tool Tip";
			this.colCallerCalls.Width = 70;
			// 
			// colCallerTotal
			// 
			this.colCallerTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerTotal.Text = "% of Total";
			this.colCallerTotal.ToolTip = "% of Total Tool Tip";
			this.colCallerTotal.Width = 70;
			// 
			// colCallerParent
			// 
			this.colCallerParent.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCallerParent.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCallerParent.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCallerParent.Text = "% of Parent";
			this.colCallerParent.ToolTip = "% of Parent Tool Tip";
			this.colCallerParent.Width = 70;
			// 
			// _tmrFilterThrottle
			// 
			this._tmrFilterThrottle.Interval = 300;
			this._tmrFilterThrottle.Tick += new System.EventHandler(this._tmrFilterThrottle_Tick);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.containerListView1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(685, 224);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Callees";
			// 
			// containerListView1
			// 
			this.containerListView1.AllowColumnReorder = true;
			this.containerListView1.CaptureFocusClick = false;
			this.containerListView1.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader1,
            this.containerListViewColumnHeader2,
            this.containerListViewColumnHeader3,
            this.containerListViewColumnHeader4,
            this.containerListViewColumnHeader5});
			this.containerListView1.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.containerListView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerListView1.HeaderHeight = 33;
			this.containerListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.containerListView1.HideSelection = false;
			this.containerListView1.Location = new System.Drawing.Point(0, 0);
			this.containerListView1.Name = "containerListView1";
			this.containerListView1.Size = new System.Drawing.Size(685, 224);
			this.containerListView1.TabIndex = 17;
			// 
			// containerListViewColumnHeader1
			// 
			this.containerListViewColumnHeader1.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader1.Text = "ID";
			this.containerListViewColumnHeader1.ToolTip = "ID Tool Tip";
			this.containerListViewColumnHeader1.Width = 100;
			// 
			// containerListViewColumnHeader2
			// 
			this.containerListViewColumnHeader2.DisplayIndex = 1;
			this.containerListViewColumnHeader2.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader2.Text = "Method";
			this.containerListViewColumnHeader2.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader2.Width = 400;
			// 
			// containerListViewColumnHeader3
			// 
			this.containerListViewColumnHeader3.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader3.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader3.DisplayIndex = 2;
			this.containerListViewColumnHeader3.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader3.Text = "# of Calls";
			this.containerListViewColumnHeader3.ToolTip = "# of Calls Tool Tip";
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
			// containerListViewColumnHeader5
			// 
			this.containerListViewColumnHeader5.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader5.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader5.DisplayIndex = 4;
			this.containerListViewColumnHeader5.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader5.Text = "% of Parent";
			this.containerListViewColumnHeader5.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader5.Width = 70;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.containerListView2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(685, 150);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Callers";
			// 
			// containerListView2
			// 
			this.containerListView2.AllowColumnReorder = true;
			this.containerListView2.CaptureFocusClick = false;
			this.containerListView2.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
            this.containerListViewColumnHeader6,
            this.containerListViewColumnHeader7,
            this.containerListViewColumnHeader8,
            this.containerListViewColumnHeader9,
            this.containerListViewColumnHeader10});
			this.containerListView2.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this.containerListView2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.containerListView2.HeaderHeight = 33;
			this.containerListView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this.containerListView2.HideSelection = false;
			this.containerListView2.Location = new System.Drawing.Point(0, 0);
			this.containerListView2.Name = "containerListView2";
			this.containerListView2.Size = new System.Drawing.Size(685, 150);
			this.containerListView2.TabIndex = 18;
			// 
			// containerListViewColumnHeader6
			// 
			this.containerListViewColumnHeader6.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader6.Text = "ID";
			this.containerListViewColumnHeader6.ToolTip = "ID Tool Tip";
			this.containerListViewColumnHeader6.Width = 100;
			// 
			// containerListViewColumnHeader7
			// 
			this.containerListViewColumnHeader7.DisplayIndex = 1;
			this.containerListViewColumnHeader7.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader7.Text = "Method";
			this.containerListViewColumnHeader7.ToolTip = "Signature Tool Tip";
			this.containerListViewColumnHeader7.Width = 400;
			// 
			// containerListViewColumnHeader8
			// 
			this.containerListViewColumnHeader8.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader8.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader8.DisplayIndex = 2;
			this.containerListViewColumnHeader8.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader8.Text = "# of Calls";
			this.containerListViewColumnHeader8.ToolTip = "# of Calls Tool Tip";
			this.containerListViewColumnHeader8.Width = 70;
			// 
			// containerListViewColumnHeader9
			// 
			this.containerListViewColumnHeader9.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader9.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader9.DisplayIndex = 3;
			this.containerListViewColumnHeader9.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader9.Text = "% of Total";
			this.containerListViewColumnHeader9.ToolTip = "% of Total Tool Tip";
			this.containerListViewColumnHeader9.Width = 70;
			// 
			// containerListViewColumnHeader10
			// 
			this.containerListViewColumnHeader10.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.containerListViewColumnHeader10.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.containerListViewColumnHeader10.DisplayIndex = 4;
			this.containerListViewColumnHeader10.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.containerListViewColumnHeader10.Text = "% of Parent";
			this.containerListViewColumnHeader10.ToolTip = "% of Parent Tool Tip";
			this.containerListViewColumnHeader10.Width = 70;
			// 
			// colCalleeParent
			// 
			this.colCalleeParent.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeParent.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeParent.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeParent.Text = "% of Parent";
			this.colCalleeParent.ToolTip = "% of Parent Tool Tip";
			this.colCalleeParent.Width = 70;
			// 
			// colCalleeTotal
			// 
			this.colCalleeTotal.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeTotal.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeTotal.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeTotal.Text = "% of Total";
			this.colCalleeTotal.ToolTip = "% of Total Tool Tip";
			this.colCalleeTotal.Width = 70;
			// 
			// colCalleeCalls
			// 
			this.colCalleeCalls.ContentAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.colCalleeCalls.DefaultSortOrder = System.Windows.Forms.SortOrder.Descending;
			this.colCalleeCalls.SortDataType = DotNetLib.Windows.Forms.SortDataType.Double;
			this.colCalleeCalls.Text = "# of Calls";
			this.colCalleeCalls.ToolTip = "# of Calls Tool Tip";
			this.colCalleeCalls.Width = 70;
			// 
			// colCalleeSignature
			// 
			this.colCalleeSignature.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.colCalleeSignature.Text = "Method";
			this.colCalleeSignature.ToolTip = "Signature Tool Tip";
			this.colCalleeSignature.Width = 400;
			// 
			// colCalleeID
			// 
			this.colCalleeID.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.colCalleeID.Text = "ID";
			this.colCalleeID.ToolTip = "ID Tool Tip";
			this.colCalleeID.Width = 100;
			// 
			// _ptProcessTree
			// 
			this._ptProcessTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._ptProcessTree.Location = new System.Drawing.Point(0, 0);
			this._ptProcessTree.Name = "_ptProcessTree";
			this._ptProcessTree.Processes = null;
			this._ptProcessTree.Size = new System.Drawing.Size(200, 568);
			this._ptProcessTree.TabIndex = 14;
			// 
			// ProfilerControl
			// 
			this.Controls.Add(this.panel1);
			this.Name = "ProfilerControl";
			this.Size = new System.Drawing.Size(896, 568);
			this.Load += new System.EventHandler(this.ProfilerControl_Load);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
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
			//_tvNamespaceInfo.Nodes.Clear();

			_bUpdating = true;
			_htCheckedItems = new Hashtable();

			try
			{
				//_tvNamespaceInfo.BeginUpdate();

				//TreeNode tnRoot = _tvNamespaceInfo.Nodes.Add( "All Namespaces" );
				//tnRoot.Tag = "";
				//tnRoot.Checked = true;

				ThreadInfo tiCurrentThread = _tiCurrent;
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
				{
					//TreeNodeCollection tnc = tnRoot.Nodes;
					ArrayList alNamespace = new ArrayList();
					foreach ( string strNamespacePiece in fi.Signature.Namespace )
					{
						alNamespace.Add( strNamespacePiece );
						TreeNode tnFound = null;

						//foreach ( TreeNode tn in tnc )
						//{
						//    if ( tn.Text == strNamespacePiece )
						//    {
						//        tnFound = tn;
						//        break;
						//    }
						//}

						//if ( tnFound == null )
						//{
						//    tnFound = tnc.Add( strNamespacePiece );
						//    tnFound.Tag = String.Join( ".", ( string[] )alNamespace.ToArray( typeof( string ) ) );
						//    tnFound.Checked = true;
						//}

						//tnc = tnFound.Nodes;
					}
				}

				//tnRoot.Expand();
			}
			finally
			{
				_bUpdating = false;
				//_tvNamespaceInfo.EndUpdate();
			}
		}

		private void UpdateFilters()
		{
			_lvFunctionInfo.Items.Clear();
			callees.Items.Clear();
			callers.Items.Clear();

			Hashtable htNamespaces = new Hashtable();
			foreach ( TreeNode tn in _htCheckedItems.Keys )
				htNamespaces[ tn.Tag ] = null;

			try
			{
				_lvFunctionInfo.BeginUpdate();

				ThreadInfo tiCurrentThread = _tiCurrent;
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
				{
					//if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
					//    continue;
					ContainerListViewItem lvi = _lvFunctionInfo.Items.Add(fi.Signature.Signature);
					lvi.SubItems[1].Text = fi.Calls.ToString();
					//lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" );
					lvi.SubItems[2].Text = fi.PercentOfTotalTimeInChildren.ToString("0.00;-0.00;0");
					//lvi.SubItems[4].Text = fi.PercentOfTotalTimeInChildren.ToString("0.00;-0.00;0");
					//lvi.SubItems[ 5 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );

					//ContainerListViewItem lvi = _lvFunctionInfo.Items.Add( fi.ID.ToString() );
					//lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					//lvi.SubItems[ 2 ].Text = fi.Calls.ToString();
					////lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" );
					//lvi.SubItems[3].Text = fi.PercentOfTotalTimeInChildren.ToString("0.00;-0.00;0");
					////lvi.SubItems[4].Text = fi.PercentOfTotalTimeInChildren.ToString("0.00;-0.00;0");
					////lvi.SubItems[ 5 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
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
			callees.Items.Clear();
			callers.Items.Clear();

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
			callers.BeginUpdate();

			bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
			callers.ShowPlusMinus = multipleSelected;
			callers.ShowRootTreeLines = multipleSelected;
			callers.ShowTreeLines = multipleSelected;

			ThreadInfo tiCurrentThread = _tiCurrent;
			foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
			{
				FunctionInfo mfi = ( FunctionInfo )item.Tag;

				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection.Values )
					foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
						if ( cfi.ID == mfi.ID )
						{
							ContainerListViewItem parentItem = null;

							foreach ( ContainerListViewItem pitem in callers.Items )
								if ( ( pitem.Tag as FunctionInfo ).ID == fi.ID )
									parentItem = pitem;

							if ( parentItem == null ) // don't have it
							{
								parentItem = callers.Items.Add(fi.Signature.Signature);
								//parentItem.Tag = fi;
								parentItem.SubItems[1].Text = cfi.Calls.ToString();
								parentItem.SubItems[2].Text = cfi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
								parentItem.SubItems[3].Text = cfi.PercentOfParentTimeInMethod.ToString("0.00;-0.00;0");

								//parentItem = callers.Items.Add( fi.ID.ToString() );
								//parentItem.SubItems[ 1 ].Text = fi.Signature.Signature;
								//parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
								//parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
								//parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
								parentItem.Tag = fi;
							}
							else // do, update totals
							{
								parentItem.SubItems[1].Text = (int.Parse(parentItem.SubItems[2].Text) + cfi.Calls).ToString();
								parentItem.SubItems[2].Text = "-";
								parentItem.SubItems[3].Text = "-";

								//parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
								//parentItem.SubItems[ 3 ].Text = "-";
								//parentItem.SubItems[ 4 ].Text = "-";
							}

							// either way, add a child pointing back to the parent
							ContainerListViewItem lvi = parentItem.Items.Add(cfi.Signature);
							//lvi.Tag=cfi;
							lvi.SubItems[1].Text = cfi.Calls.ToString();
							lvi.SubItems[2].Text = cfi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
							lvi.SubItems[3].Text = cfi.PercentOfParentTimeInMethod.ToString("0.00;-0.00;0");
							lvi.Tag = cfi;

							//ContainerListViewItem lvi = parentItem.Items.Add( cfi.ID.ToString() );
							//lvi.SubItems[ 1 ].Text = cfi.Signature;
							//lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
							//lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
							//lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
							//lvi.Tag = cfi;
						}
			}

			callers.Sort();
			callers.EndUpdate();
		}

		private void UpdateCalleeList()
		{
			callees.BeginUpdate();

			bool multipleSelected = (_lvFunctionInfo.SelectedItems.Count > 1);
			callees.ShowPlusMinus = multipleSelected;
			callees.ShowRootTreeLines = multipleSelected;
			callees.ShowTreeLines = multipleSelected;

			ContainerListViewItem lviSuspend = null;

			foreach ( ContainerListViewItem item in _lvFunctionInfo.SelectedItems )
			{
				FunctionInfo fi = ( FunctionInfo )item.Tag;

				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					ContainerListViewItem parentItem = null;

					foreach ( ContainerListViewItem pitem in callees.Items )
						if( pitem.Tag != null)
							if ( ( pitem.Tag as CalleeFunctionInfo ).ID == cfi.ID )
								parentItem = pitem;


					if ( parentItem == null ) // don't have it
					{
						parentItem = callees.Items.Add(cfi.Signature);
						parentItem.SubItems[1].Text = cfi.Calls.ToString();
						parentItem.SubItems[2].Text = cfi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
						parentItem.SubItems[3].Text = cfi.PercentOfParentTimeInMethod.ToString("0.00;-0.00;0");
						//parentItem = callees.Items.Add( cfi.ID.ToString() );
						//parentItem.SubItems[ 1 ].Text = cfi.Signature;
						//parentItem.SubItems[ 2 ].Text = cfi.Calls.ToString();
						//parentItem.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
						//parentItem.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
						parentItem.Tag = cfi;
					}
					else // do, update totals
					{
						parentItem.SubItems[1].Text = (int.Parse(parentItem.SubItems[2].Text) + cfi.Calls).ToString();
						parentItem.SubItems[2].Text = "-";
						parentItem.SubItems[3].Text = "-";
						//parentItem.SubItems[ 2 ].Text = (int.Parse( parentItem.SubItems[ 2 ].Text ) + cfi.Calls ).ToString();
						//parentItem.SubItems[ 3 ].Text = "-";
						//parentItem.SubItems[ 4 ].Text = "-";
					}

					// either way, add a child pointing back to the parent
					ContainerListViewItem lvi = parentItem.Items.Add(fi.Signature.Signature);
					lvi.SubItems[1].Text = cfi.Calls.ToString();
					lvi.SubItems[2].Text = cfi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
					lvi.SubItems[3].Text = cfi.PercentOfParentTimeInMethod.ToString("0.00;-0.00;0");
					//ContainerListViewItem lvi = parentItem.Items.Add( fi.ID.ToString() );
					//lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					//lvi.SubItems[ 2 ].Text = cfi.Calls.ToString();
					//lvi.SubItems[ 3 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
					//lvi.SubItems[ 4 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.Tag = fi;
				}

				ContainerListViewItem inMethod = callees.Items.Add("(in method)");
				inMethod.SubItems[1].Text = fi.Calls.ToString();
				inMethod.SubItems[2].Text = fi.PercentOfTotalTimeInMethod.ToString("0.00;-0.00;0");
				inMethod.SubItems[3].Text = fi.PercentOfMethodTimeInMethod.ToString("0.00;-0.00;0");
				inMethod.Tag = fi;



				if ( fi.TotalSuspendedTicks > 0 )
				{
					if ( lviSuspend == null) // don't have it
					{
						lviSuspend = callees.Items.Add("(thread suspended)");
						lviSuspend.SubItems[1].Text = "-";
						lviSuspend.SubItems[2].Text = fi.PercentOfTotalTimeSuspended.ToString("0.00;-0.00;0");
						lviSuspend.SubItems[3].Text = fi.PercentOfMethodTimeSuspended.ToString("0.00;-0.00;0");

						//lviSuspend = callees.Items.Add( "(suspend)" );
						//lviSuspend.SubItems[ 1 ].Text = "(thread suspended)";
						//lviSuspend.SubItems[ 2 ].Text = "-";
						//lviSuspend.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
						//lviSuspend.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
					}
					else // do, update totals
					{
						lviSuspend.SubItems[ 2 ].Text = "-";
						lviSuspend.SubItems[ 3 ].Text = "-";
					}

					// either way, add a child pointing back to the parent
					ContainerListViewItem lvi = lviSuspend.Items.Add(fi.Signature.Signature);
					lvi.SubItems[ 1 ].Text = "-";
					lvi.SubItems[ 2 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 3 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
					//ContainerListViewItem lvi = lviSuspend.Items.Add( fi.ID.ToString() );
					//lvi.SubItems[ 1 ].Text = fi.Signature.Signature;
					//lvi.SubItems[ 2 ].Text = "-";
					//lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
					//lvi.SubItems[ 4 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
				}
			}

			callees.Sort();
			callees.EndUpdate();
		}

		private void ProfilerControl_Load(object sender, System.EventArgs e)
		{
			_ptProcessTree.ThreadSelected += new ProcessTree.ThreadSelectedHandler( _ptProcessTree_ThreadSelected );
			_lvFunctionInfo.Sort(1, true, true);
			callees.Sort(1, true, true);
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
			//_tmrFilterThrottle.Enabled = false;
			//string text = _txtFilterBar.Text.ToLower();
			//foreach (ContainerListViewItem item in _lvFunctionInfo.Items)
			//{
			//    if (text!="" && item.SubItems[1].Text.ToLower().IndexOf(text) != -1)
			//    {
			//        item.BackColor = Color.LightSteelBlue;
			//    }
			//    else
			//    {
			//        item.BackColor = _lvFunctionInfo.BackColor;
			//    }
			//}
			//_lvFunctionInfo.BeginUpdate();
			//_lvFunctionInfo.Items.Sort(new SignatureComparer(text));
			//_lvFunctionInfo.EndUpdate();
			//_lvFunctionInfo.Invalidate();
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
			if ( callees.SelectedItems.Count == 0 )
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
