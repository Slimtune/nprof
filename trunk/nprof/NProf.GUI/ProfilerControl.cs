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
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView _tvNamespaceInfo;
		private System.Windows.Forms.ComboBox _cbCurrentThread;
		private DotNetLib.Windows.Forms.ContainerListView _lvFunctionInfo;
		private DotNetLib.Windows.Forms.ContainerListView _lvChildInfo;
		private System.Windows.Forms.Splitter splitter2;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader1;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader2;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader3;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader4;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader5;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader6;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader7;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader8;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader9;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader10;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader11;
		private DotNetLib.Windows.Forms.ToggleColumnHeader toggleColumnHeader12;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionID;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionSignature;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionCalls;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionTotal;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionMethod;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionChildren;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colFunctionSuspended;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colChildID;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colChildSignature;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colChildCalls;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colChildTotal;
		private DotNetLib.Windows.Forms.ToggleColumnHeader colChildParent;
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
			this.toggleColumnHeader1 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader2 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader3 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader4 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader5 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader6 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader7 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader8 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader9 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader10 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader11 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.toggleColumnHeader12 = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this._lvFunctionInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colFunctionID = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionSignature = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionCalls = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionTotal = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionMethod = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionChildren = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colFunctionSuspended = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this._lvChildInfo = new DotNetLib.Windows.Forms.ContainerListView();
			this.colChildID = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colChildSignature = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colChildCalls = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colChildTotal = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.colChildParent = new DotNetLib.Windows.Forms.ToggleColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this._tvNamespaceInfo = new System.Windows.Forms.TreeView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this._cbCurrentThread = new System.Windows.Forms.ComboBox();
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
			this._lvFunctionInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ToggleColumnHeader[] {
																																	this.colFunctionID,
																																	this.colFunctionSignature,
																																	this.colFunctionCalls,
																																	this.colFunctionTotal,
																																	this.colFunctionMethod,
																																	this.colFunctionChildren,
																																	this.colFunctionSuspended});
			this._lvFunctionInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.ColumnTrackingColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvFunctionInfo.GridLineColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvFunctionInfo.HideSelection = false;
			this._lvFunctionInfo.Location = new System.Drawing.Point(195, 24);
			this._lvFunctionInfo.MultiSelect = true;
			this._lvFunctionInfo.Name = "_lvFunctionInfo";
			this._lvFunctionInfo.RowSelectedColor = System.Drawing.SystemColors.Highlight;
			this._lvFunctionInfo.RowTrackingColor = System.Drawing.Color.WhiteSmoke;
			this._lvFunctionInfo.Size = new System.Drawing.Size(677, 240);
			this._lvFunctionInfo.TabIndex = 14;
			this._lvFunctionInfo.HeaderMenuEvent += new DotNetLib.Windows.Forms.HeaderMenuEventHandler(this._lv_HeaderMenuEvent);
			this._lvFunctionInfo.SelectedIndexChanged += new System.EventHandler(this._lvFunctionInfo_SelectedIndexChanged);
			// 
			// colFunctionID
			// 
			this.colFunctionID.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Integer;
			this.colFunctionID.Text = "ID";
			this.colFunctionID.ToolTip = "ID Tool Tip";
			this.colFunctionID.Width = 100;
			// 
			// colFunctionSignature
			// 
			this.colFunctionSignature.DisplayIndex = 1;
			this.colFunctionSignature.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.String;
			this.colFunctionSignature.Text = "Signature";
			this.colFunctionSignature.ToolTip = "Signature Tool Tip";
			this.colFunctionSignature.Width = 350;
			// 
			// colFunctionCalls
			// 
			this.colFunctionCalls.DisplayIndex = 2;
			this.colFunctionCalls.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colFunctionCalls.Text = "# of Calls";
			this.colFunctionCalls.ToolTip = "# of Calls Tool Tip";
			this.colFunctionCalls.Width = 70;
			// 
			// colFunctionTotal
			// 
			this.colFunctionTotal.DisplayIndex = 3;
			this.colFunctionTotal.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colFunctionTotal.Text = "% of Total";
			this.colFunctionTotal.ToolTip = "% of Total Tool Tip";
			this.colFunctionTotal.Width = 70;
			// 
			// colFunctionMethod
			// 
			this.colFunctionMethod.DisplayIndex = 4;
			this.colFunctionMethod.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colFunctionMethod.Text = "% in Method";
			this.colFunctionMethod.ToolTip = "% in Method Tool Tip";
			this.colFunctionMethod.Width = 70;
			// 
			// colFunctionChildren
			// 
			this.colFunctionChildren.DisplayIndex = 5;
			this.colFunctionChildren.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colFunctionChildren.Text = "% in Children";
			this.colFunctionChildren.ToolTip = "% in Children Tool Tip";
			this.colFunctionChildren.Width = 70;
			// 
			// colFunctionSuspended
			// 
			this.colFunctionSuspended.DisplayIndex = 6;
			this.colFunctionSuspended.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colFunctionSuspended.Text = "% Suspended";
			this.colFunctionSuspended.ToolTip = "% Suspended Tool Tip";
			this.colFunctionSuspended.Width = 70;
			// 
			// _lvChildInfo
			// 
			this._lvChildInfo.AllowColumnReorder = true;
			this._lvChildInfo.BackColor = System.Drawing.SystemColors.Window;
			this._lvChildInfo.Columns.AddRange(new DotNetLib.Windows.Forms.ToggleColumnHeader[] {
																																this.colChildID,
																																this.colChildSignature,
																																this.colChildCalls,
																																this.colChildTotal,
																																this.colChildParent});
			this._lvChildInfo.ColumnSortColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.ColumnTrackingColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._lvChildInfo.GridLineColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			this._lvChildInfo.HideSelection = false;
			this._lvChildInfo.Location = new System.Drawing.Point(195, 264);
			this._lvChildInfo.Name = "_lvChildInfo";
			this._lvChildInfo.RowSelectedColor = System.Drawing.SystemColors.Highlight;
			this._lvChildInfo.RowTrackingColor = System.Drawing.Color.WhiteSmoke;
			this._lvChildInfo.Size = new System.Drawing.Size(677, 192);
			this._lvChildInfo.TabIndex = 13;
			this._lvChildInfo.DoubleClick += new System.EventHandler(this._lvChildInfo_DoubleClick);
			this._lvChildInfo.HeaderMenuEvent += new DotNetLib.Windows.Forms.HeaderMenuEventHandler(this._lv_HeaderMenuEvent);
			// 
			// colChildID
			// 
			this.colChildID.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Integer;
			this.colChildID.Text = "ID";
			this.colChildID.ToolTip = "ID Tool Tip";
			this.colChildID.Width = 100;
			// 
			// colChildSignature
			// 
			this.colChildSignature.DisplayIndex = 1;
			this.colChildSignature.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.String;
			this.colChildSignature.Text = "Signature";
			this.colChildSignature.ToolTip = "Signature Tool Tip";
			this.colChildSignature.Width = 400;
			// 
			// colChildCalls
			// 
			this.colChildCalls.DisplayIndex = 2;
			this.colChildCalls.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colChildCalls.Text = "# of Calls";
			this.colChildCalls.ToolTip = "# of Calls Tool Tip";
			this.colChildCalls.Width = 70;
			// 
			// colChildTotal
			// 
			this.colChildTotal.DisplayIndex = 3;
			this.colChildTotal.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colChildTotal.Text = "% of Total";
			this.colChildTotal.ToolTip = "% of Total Tool Tip";
			this.colChildTotal.Width = 70;
			// 
			// colChildParent
			// 
			this.colChildParent.DisplayIndex = 4;
			this.colChildParent.SortingMethod = DotNetLib.Windows.Forms.SortingMethod.Float;
			this.colChildParent.Text = "% of Parent";
			this.colChildParent.ToolTip = "% of Parent Tool Tip";
			this.colChildParent.Width = 70;
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

					ContainerListViewItem lvi = _lvFunctionInfo.Items.Add( fi.ID.ToString() );
					lvi.SubItems[ 0 ].Text = fi.Signature.Signature;
					lvi.SubItems[ 1 ].Text = fi.Calls.ToString();
					lvi.SubItems[ 2 ].Text = fi.PercentOfTotalTimeInMethodAndChildren.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 3 ].Text = fi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 4 ].Text = fi.PercentOfTotalTimeInChildren.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 5 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
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
					ContainerListViewItem lvi = _lvChildInfo.Items.Add( cfi.ID.ToString() );
					lvi.SubItems[ 0 ].Text = cfi.Signature;
					lvi.SubItems[ 1 ].Text = cfi.Calls.ToString();
					lvi.SubItems[ 2 ].Text = cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.SubItems[ 3 ].Text = cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" );
					lvi.Tag = cfi;
				}

				if ( fi.TotalSuspendedTicks > 0 )
				{
					ContainerListViewItem lviSuspend = _lvChildInfo.Items.Add( "(suspend)" );
					lviSuspend.SubItems[ 0 ].Text = "(thread suspended)";
					lviSuspend.SubItems[ 1 ].Text = "-";
					lviSuspend.SubItems[ 2 ].Text = fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" );
					lviSuspend.SubItems[ 3 ].Text = fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" );
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
			foreach( ContainerListViewItem lvi in _lvFunctionInfo.Items )
			{
				FunctionInfo fi = ( FunctionInfo )lvi.Tag;
				if ( fi.ID == nID )
				{
					if(!lvi.Selected)
					{
						_lvFunctionInfo.SelectedItems.Clear();

						lvi.Selected = true;
						lvi.Focused = true;
						_lvFunctionInfo.EnsureVisible();
						_lvFunctionInfo.Focus();
					}
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
