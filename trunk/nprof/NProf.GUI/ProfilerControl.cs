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
using SynapticEffect.Forms;

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
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView _tvNamespaceInfo;
		private System.Windows.Forms.ComboBox _cbCurrentThread;
		private SynapticEffect.Forms.ContainerListView _lvFunctionInfo;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.ColumnHeader colSignature;
		private System.Windows.Forms.ColumnHeader colCalls;
		private System.Windows.Forms.ColumnHeader colTotalTime;
		private System.Windows.Forms.ColumnHeader colTimeInMethod;
		private System.Windows.Forms.ColumnHeader colChildrenTime;
		private System.Windows.Forms.ColumnHeader colTimeSuspended;
		private SynapticEffect.Forms.ContainerListView _lvChildInfo;
		private System.Windows.Forms.Splitter splitter2;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader1 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader2 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader3 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader4 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader5 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader6 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader7 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader8 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader9 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader10 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader11 = new SynapticEffect.Forms.ToggleColumnHeader();
			SynapticEffect.Forms.ToggleColumnHeader toggleColumnHeader12 = new SynapticEffect.Forms.ToggleColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this._lvFunctionInfo = new SynapticEffect.Forms.ContainerListView();
			this._lvChildInfo = new SynapticEffect.Forms.ContainerListView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this._tvNamespaceInfo = new System.Windows.Forms.TreeView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this._cbCurrentThread = new System.Windows.Forms.ComboBox();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalls = new System.Windows.Forms.ColumnHeader();
			this.colTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeInMethod = new System.Windows.Forms.ColumnHeader();
			this.colChildrenTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeSuspended = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.splitter2);
			this.panel1.Controls.Add(this._lvFunctionInfo);
			this.panel1.Controls.Add(this._lvChildInfo);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this._tvNamespaceInfo);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(872, 456);
			this.panel1.TabIndex = 10;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(195, 261);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(677, 3);
			this.splitter2.TabIndex = 15;
			this.splitter2.TabStop = false;
			// 
			// _lvFunctionInfo
			// 
			this._lvFunctionInfo.AllowColumnReorder = true;
			this._lvFunctionInfo.BackColor = System.Drawing.SystemColors.Window;
			toggleColumnHeader1.Hovered = false;
			toggleColumnHeader1.Image = null;
			toggleColumnHeader1.Index = 0;
			toggleColumnHeader1.Pressed = false;
			toggleColumnHeader1.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader1.Selected = false;
			toggleColumnHeader1.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader1.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader1.Text = "ID";
			toggleColumnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader1.ToolTip = "ID Tip";
			toggleColumnHeader1.Visible = true;
			toggleColumnHeader1.Width = 100;
			toggleColumnHeader2.Hovered = false;
			toggleColumnHeader2.Image = null;
			toggleColumnHeader2.Index = 0;
			toggleColumnHeader2.Pressed = false;
			toggleColumnHeader2.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader2.Selected = false;
			toggleColumnHeader2.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader2.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader2.Text = "Signature";
			toggleColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader2.ToolTip = "Signature Tip";
			toggleColumnHeader2.Visible = true;
			toggleColumnHeader2.Width = 350;
			toggleColumnHeader3.Hovered = false;
			toggleColumnHeader3.Image = null;
			toggleColumnHeader3.Index = 0;
			toggleColumnHeader3.Pressed = false;
			toggleColumnHeader3.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader3.Selected = false;
			toggleColumnHeader3.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader3.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader3.Text = "# of Calls";
			toggleColumnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader3.ToolTip = "# of Calls Tip";
			toggleColumnHeader3.Visible = true;
			toggleColumnHeader3.Width = 70;
			toggleColumnHeader4.Hovered = false;
			toggleColumnHeader4.Image = null;
			toggleColumnHeader4.Index = 0;
			toggleColumnHeader4.Pressed = false;
			toggleColumnHeader4.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader4.Selected = false;
			toggleColumnHeader4.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader4.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader4.Text = "% of Total";
			toggleColumnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader4.ToolTip = "% of Total Tip";
			toggleColumnHeader4.Visible = true;
			toggleColumnHeader4.Width = 70;
			toggleColumnHeader5.Hovered = false;
			toggleColumnHeader5.Image = null;
			toggleColumnHeader5.Index = 0;
			toggleColumnHeader5.Pressed = false;
			toggleColumnHeader5.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader5.Selected = false;
			toggleColumnHeader5.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader5.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader5.Text = "% in Method";
			toggleColumnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader5.ToolTip = "% in Method Tip";
			toggleColumnHeader5.Visible = true;
			toggleColumnHeader5.Width = 70;
			toggleColumnHeader6.Hovered = false;
			toggleColumnHeader6.Image = null;
			toggleColumnHeader6.Index = 0;
			toggleColumnHeader6.Pressed = false;
			toggleColumnHeader6.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader6.Selected = false;
			toggleColumnHeader6.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader6.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader6.Text = "% in Children";
			toggleColumnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader6.ToolTip = "% in Children Tip";
			toggleColumnHeader6.Visible = true;
			toggleColumnHeader6.Width = 70;
			toggleColumnHeader7.Hovered = false;
			toggleColumnHeader7.Image = null;
			toggleColumnHeader7.Index = 0;
			toggleColumnHeader7.Pressed = false;
			toggleColumnHeader7.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader7.Selected = false;
			toggleColumnHeader7.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader7.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader7.Text = "% Suspended";
			toggleColumnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader7.ToolTip = "% Suspended Tip";
			toggleColumnHeader7.Visible = true;
			toggleColumnHeader7.Width = 70;
			this._lvFunctionInfo.Columns.AddRange(new SynapticEffect.Forms.ToggleColumnHeader[] {
																																toggleColumnHeader1,
																																toggleColumnHeader2,
																																toggleColumnHeader3,
																																toggleColumnHeader4,
																																toggleColumnHeader5,
																																toggleColumnHeader6,
																																toggleColumnHeader7});
			this._lvFunctionInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.ColumnTrackColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvFunctionInfo.GridLineColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.HeaderMenu = null;
			this._lvFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvFunctionInfo.HideSelection = false;
			this._lvFunctionInfo.ItemMenu = null;
			this._lvFunctionInfo.LabelEdit = false;
			this._lvFunctionInfo.Location = new System.Drawing.Point(195, 24);
			this._lvFunctionInfo.Name = "_lvFunctionInfo";
			this._lvFunctionInfo.RowSelectColor = System.Drawing.SystemColors.Highlight;
			this._lvFunctionInfo.RowTrackColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.RowTracking = false;
			this._lvFunctionInfo.Size = new System.Drawing.Size(677, 240);
			this._lvFunctionInfo.SmallImageList = null;
			this._lvFunctionInfo.StateImageList = null;
			this._lvFunctionInfo.TabIndex = 14;
			this._lvFunctionInfo.HeaderMenuEvent += new SynapticEffect.Forms.HeaderMenuEventHandler(this._lv_HeaderMenuEvent);
			this._lvFunctionInfo.SelectedIndexChanged += new System.EventHandler(this._lvFunctionInfo_SelectedIndexChanged);
			// 
			// _lvChildInfo
			// 
			this._lvChildInfo.AllowColumnReorder = true;
			this._lvChildInfo.BackColor = System.Drawing.SystemColors.Window;
			toggleColumnHeader8.Hovered = false;
			toggleColumnHeader8.Image = null;
			toggleColumnHeader8.Index = 0;
			toggleColumnHeader8.Pressed = false;
			toggleColumnHeader8.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader8.Selected = false;
			toggleColumnHeader8.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader8.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader8.Text = "ID";
			toggleColumnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader8.ToolTip = "ID Tip";
			toggleColumnHeader8.Visible = true;
			toggleColumnHeader8.Width = 100;
			toggleColumnHeader9.Hovered = false;
			toggleColumnHeader9.Image = null;
			toggleColumnHeader9.Index = 0;
			toggleColumnHeader9.Pressed = false;
			toggleColumnHeader9.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader9.Selected = false;
			toggleColumnHeader9.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader9.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader9.Text = "Signature";
			toggleColumnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader9.ToolTip = "Signature Tip";
			toggleColumnHeader9.Visible = true;
			toggleColumnHeader9.Width = 400;
			toggleColumnHeader10.Hovered = false;
			toggleColumnHeader10.Image = null;
			toggleColumnHeader10.Index = 0;
			toggleColumnHeader10.Pressed = false;
			toggleColumnHeader10.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader10.Selected = false;
			toggleColumnHeader10.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader10.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader10.Text = "# of Calls";
			toggleColumnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader10.ToolTip = "# of Calls Tip";
			toggleColumnHeader10.Visible = true;
			toggleColumnHeader10.Width = 70;
			toggleColumnHeader11.Hovered = false;
			toggleColumnHeader11.Image = null;
			toggleColumnHeader11.Index = 0;
			toggleColumnHeader11.Pressed = false;
			toggleColumnHeader11.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader11.Selected = false;
			toggleColumnHeader11.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader11.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader11.Text = "% of Total";
			toggleColumnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader11.ToolTip = "% of Total Tip";
			toggleColumnHeader11.Visible = true;
			toggleColumnHeader11.Width = 70;
			toggleColumnHeader12.Hovered = false;
			toggleColumnHeader12.Image = null;
			toggleColumnHeader12.Index = 0;
			toggleColumnHeader12.Pressed = false;
			toggleColumnHeader12.ScaleStyle = SynapticEffect.Forms.ColumnScaleStyle.Slide;
			toggleColumnHeader12.Selected = false;
			toggleColumnHeader12.SortingMethod = SynapticEffect.Forms.SortingMethod.None;
			toggleColumnHeader12.SortingOrder = System.Windows.Forms.SortOrder.None;
			toggleColumnHeader12.Text = "% of Parent";
			toggleColumnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			toggleColumnHeader12.ToolTip = "% of Parent Tip";
			toggleColumnHeader12.Visible = true;
			toggleColumnHeader12.Width = 70;
			this._lvChildInfo.Columns.AddRange(new SynapticEffect.Forms.ToggleColumnHeader[] {
																															toggleColumnHeader8,
																															toggleColumnHeader9,
																															toggleColumnHeader10,
																															toggleColumnHeader11,
																															toggleColumnHeader12});
			this._lvChildInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.ColumnTrackColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._lvChildInfo.GridLineColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.HeaderMenu = null;
			this._lvChildInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvChildInfo.HideSelection = false;
			this._lvChildInfo.ItemMenu = null;
			this._lvChildInfo.LabelEdit = false;
			this._lvChildInfo.Location = new System.Drawing.Point(195, 264);
			this._lvChildInfo.Name = "_lvChildInfo";
			this._lvChildInfo.RowSelectColor = System.Drawing.SystemColors.Highlight;
			this._lvChildInfo.RowTrackColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.RowTracking = false;
			this._lvChildInfo.Size = new System.Drawing.Size(677, 192);
			this._lvChildInfo.SmallImageList = null;
			this._lvChildInfo.StateImageList = null;
			this._lvChildInfo.TabIndex = 13;
			this._lvChildInfo.DoubleClick += new System.EventHandler(this._lvChildInfo_DoubleClick);
			this._lvChildInfo.HeaderMenuEvent += new SynapticEffect.Forms.HeaderMenuEventHandler(this._lv_HeaderMenuEvent);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(192, 24);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 432);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// _tvNamespaceInfo
			// 
			this._tvNamespaceInfo.CheckBoxes = true;
			this._tvNamespaceInfo.Dock = System.Windows.Forms.DockStyle.Left;
			this._tvNamespaceInfo.ImageIndex = -1;
			this._tvNamespaceInfo.Location = new System.Drawing.Point(0, 24);
			this._tvNamespaceInfo.Name = "_tvNamespaceInfo";
			this._tvNamespaceInfo.SelectedImageIndex = -1;
			this._tvNamespaceInfo.Size = new System.Drawing.Size(192, 432);
			this._tvNamespaceInfo.TabIndex = 8;
			this._tvNamespaceInfo.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this._tvNamespaceInfo_AfterCheck);
			this._tvNamespaceInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._tvNamespaceInfo_AfterSelect);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this._cbCurrentThread);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(872, 24);
			this.panel2.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 24);
			this.label1.TabIndex = 12;
			this.label1.Text = "Thread:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _cbCurrentThread
			// 
			this._cbCurrentThread.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cbCurrentThread.Location = new System.Drawing.Point(48, 0);
			this._cbCurrentThread.Name = "_cbCurrentThread";
			this._cbCurrentThread.Size = new System.Drawing.Size(136, 21);
			this._cbCurrentThread.TabIndex = 11;
			this._cbCurrentThread.SelectedIndexChanged += new System.EventHandler(this._cbCurrentThread_SelectedIndexChanged);
			// 
			// colID
			// 
			this.colID.Text = "ID";
			this.colID.Width = 100;
			// 
			// colSignature
			// 
			this.colSignature.Text = "Signature";
			this.colSignature.Width = 350;
			// 
			// colCalls
			// 
			this.colCalls.Text = "# of Calls";
			// 
			// colTotalTime
			// 
			this.colTotalTime.Text = "% of Total";
			// 
			// colTimeInMethod
			// 
			this.colTimeInMethod.Text = "% in Method";
			// 
			// colChildrenTime
			// 
			this.colChildrenTime.Text = "% in Children";
			// 
			// colTimeSuspended
			// 
			this.colTimeSuspended.Text = "% Suspended";
			// 
			// ProfilerControl
			// 
			this.Controls.Add(this.panel1);
			this.Name = "ProfilerControl";
			this.Size = new System.Drawing.Size(872, 456);
			this.Load += new System.EventHandler(this.ProfilerControl_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ProfilerControl_Layout);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public Run ProfilerRun
		{
			set 
			{ 
				_tic = value.ThreadInfoCollection; 
				RefreshData(); 
			}
		}

		private void RefreshData()
		{
			this.BeginInvoke( new EventHandler( UpdateOnUIThread ), new object[] { null, null } );
		}

		private void UpdateOnUIThread( object oSender, EventArgs ea )
		{
			_cbCurrentThread.Items.Clear();

			_cbCurrentThread.Sorted = false;
			SortedList sl = new SortedList();

			// Sort the thread info
			foreach ( ThreadInfo ti in _tic )
				sl.Add( ti.ID, ti );

			// Then add them to the combobox
			foreach ( DictionaryEntry de in sl )
				_cbCurrentThread.Items.Add( de.Value );

			if ( _cbCurrentThread.Items.Count > 0 )
			{
				_cbCurrentThread.SelectedIndex = 0;
				_cbCurrentThread.Enabled = true;
			}
			else
				_cbCurrentThread.Enabled = false;
		}

		private void _cbCurrentThread_SelectedIndexChanged(object sender, System.EventArgs e)
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

				ThreadInfo tiCurrentThread = _tic[ ( ( ThreadInfo )_cbCurrentThread.SelectedItem ).ID ];
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
			_lvChildInfo.Items.Clear();

			Hashtable htNamespaces = new Hashtable();
			foreach ( TreeNode tn in _htCheckedItems.Keys )
				htNamespaces[ tn.Tag ] = null;

			try
			{
				_lvFunctionInfo.BeginUpdate();

				ThreadInfo tiCurrentThread = _tic[ ( ( ThreadInfo )_cbCurrentThread.SelectedItem ).ID ];
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
				{
					if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
						continue;

					ContainerListViewItem lvi = new ContainerListViewItem();
					lvi.Text = fi.ID.ToString();
					lvi.SubItems.Add( fi.Signature.Signature );
					lvi.SubItems.Add( fi.Calls.ToString() );
					lvi.SubItems.Add( fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.PercentOfTotalTimeInChildren.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" ) );
					_lvFunctionInfo.Items.Add( lvi );
					lvi.Tag = fi;
				}
			}
			finally
			{
				_lvFunctionInfo.Sort(false);
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

		private void _lvFunctionInfo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( _lvFunctionInfo.SelectedItems.Count == 0 )
			{
				_lvChildInfo.Items.Clear();
				return;
			}
			FunctionInfo fi = ( FunctionInfo )_lvFunctionInfo.SelectedItems[ 0 ].Tag;
			_lvChildInfo.Items.Clear();

			try
			{
				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					ContainerListViewItem lvi = new ContainerListViewItem();
					lvi.Text = cfi.ID.ToString();
					lvi.SubItems.Add( cfi.Signature );
					lvi.SubItems.Add( cfi.Calls.ToString() );
					lvi.SubItems.Add( cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" ) );

					lvi.Tag = cfi;
					_lvChildInfo.Items.Add( lvi );
				}

				if ( fi.TotalSuspendedTicks > 0 )
				{
					ContainerListViewItem lviSuspend = new ContainerListViewItem();
					lviSuspend.Text = "(suspend)";
					lviSuspend.SubItems.Add( "(thread suspended)" );
					lviSuspend.SubItems.Add( "-" );
					lviSuspend.SubItems.Add( fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" ) );
					lviSuspend.SubItems.Add( fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" ) );
					_lvChildInfo.Items.Add( lviSuspend );
				}
			}
			finally
			{
				_lvChildInfo.Sort(false);
				_lvChildInfo.Refresh();
			}
		}

		private void ProfilerControl_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{

		}

		private void ProfilerControl_Load(object sender, System.EventArgs e)
		{
			_lvFunctionInfo.Columns[0].SortingMethod = SortingMethod.Float;
			_lvFunctionInfo.Columns[1].SortingMethod = SortingMethod.String;
			_lvFunctionInfo.Columns[2].SortingMethod = SortingMethod.Float;
			_lvFunctionInfo.Columns[3].SortingMethod = SortingMethod.Float;
			_lvFunctionInfo.Columns[4].SortingMethod = SortingMethod.Float;
			_lvFunctionInfo.Columns[5].SortingMethod = SortingMethod.Float;
			_lvFunctionInfo.Columns[6].SortingMethod = SortingMethod.Float;

			_lvFunctionInfo.Sort(1, false);

			_lvChildInfo.Columns[0].SortingMethod = SortingMethod.Float;
			_lvChildInfo.Columns[1].SortingMethod = SortingMethod.String;
			_lvChildInfo.Columns[2].SortingMethod = SortingMethod.Float;
			_lvChildInfo.Columns[3].SortingMethod = SortingMethod.Float;
			_lvChildInfo.Columns[4].SortingMethod = SortingMethod.Float;

			_lvChildInfo.Sort(1, false);
		}

		private void _lvChildInfo_DoubleClick(object sender, System.EventArgs e)
		{
			if ( _lvChildInfo.SelectedItems.Count == 0 )
				return;

			CalleeFunctionInfo cfi = ( CalleeFunctionInfo )_lvChildInfo.SelectedItems[ 0 ].Tag;
			if ( cfi == null )
				MessageBox.Show( "No information available for this item" );
			else
				JumpToID( cfi.ID );
		}

		private void JumpToID( int nID )
		{
			foreach ( ContainerListViewItem lvi in _lvFunctionInfo.Items )
			{
				FunctionInfo fi = ( FunctionInfo )lvi.Tag;
				if ( fi.ID == nID )
				{
					lvi.Selected = true;
					lvi.Focused = true;
					_lvFunctionInfo.EnsureVisible();
					_lvFunctionInfo.Focus();
					break;
				}
			}
		}

		private int GetSelectedID()
		{
			if ( _lvChildInfo.SelectedItems.Count == 0 )
				return -1;

			return ( ( FunctionInfo )_lvFunctionInfo.SelectedItems[ 0 ].Tag ).ID;
		}

		private ThreadInfoCollection _tic;
		private bool _bUpdating, _bInCheck;
		private Hashtable _htCheckedItems;

		#region "Context Menus"

		#region "Headers"

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
				ToggleColumnHeader hdr = listView.Columns[idx];
				MenuCommand sortByItem = new MenuCommand(hdr.Text);

				sortByItem.Description = string.Format("Sort By the '{1}' column from this grid", (hdr.Visible ? "Shows" : "Hides"), hdr.Text);
				sortByItem.RadioCheck = true;
				sortByItem.Checked = hdr.SortingOrder != SortOrder.None;
				sortByItem.Tag = new object[] { listView, idx };
				sortByItem.Click += new EventHandler(sortByItem_Click);

				if(sortByItem.Checked)
					isAscending = hdr.SortingOrder == SortOrder.Ascending;

				sortBy.MenuCommands.Add(sortByItem);
			}

			sortBy.MenuCommands.Add(new MenuCommand("-"));
			MenuCommand ascending = sortBy.MenuCommands.Add(new MenuCommand("&Ascending"));
			ascending.RadioCheck = true;
			ascending.Checked = isAscending;
			ascending.Tag = new object[] { listView, SortOrder.Ascending };
			ascending.Click += new EventHandler(sortOrder_Click);

			MenuCommand descending = sortBy.MenuCommands.Add(new MenuCommand("&Descending"));
			descending.RadioCheck = true;
			descending.Checked = !isAscending;
			descending.Tag = new object[] { listView, SortOrder.Descending };
			descending.Click += new EventHandler(sortOrder_Click);

			bool allShown = true;
			for(int idx = 0; idx < listView.Columns.Count; ++idx)
			{
				ToggleColumnHeader hdr = listView.Columns[idx];
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
			if(result != null && result.Tag is ToggleColumnHeader)
				(result.Tag as ToggleColumnHeader).Visible = !result.Checked;
		}

		private void sortOrder_Click(object sender, EventArgs e)
		{
			MenuCommand cmd = sender as MenuCommand;

			object[] objs = (cmd.Tag as object[]);
			ContainerListView listView = objs[0] as ContainerListView;

			listView.Sort((SortOrder)objs[1]);
		}

		private void sortByItem_Click(object sender, EventArgs e)
		{
			MenuCommand cmd = sender as MenuCommand;

			object[] objs = (cmd.Tag as object[]);
			ContainerListView listView = objs[0] as ContainerListView;

			listView.Sort((int)objs[1], false);
		}

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
