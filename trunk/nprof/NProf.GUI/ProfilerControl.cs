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
using Genghis.Windows.Forms;

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
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.ColumnHeader colSignature;
		private System.Windows.Forms.ColumnHeader colCalls;
		private System.Windows.Forms.ColumnHeader colTotalTime;
		private System.Windows.Forms.ColumnHeader colTimeInMethod;
		private System.Windows.Forms.ColumnHeader colChildrenTime;
		private System.Windows.Forms.ColumnHeader colTimeSuspended;
		private System.Windows.Forms.ColumnHeader colCalleeID;
		private System.Windows.Forms.ColumnHeader colCalleeSignature;
		private System.Windows.Forms.ColumnHeader colCalleeCalls;
		private System.Windows.Forms.ColumnHeader colCalleeTotalTime;
		private System.Windows.Forms.ColumnHeader colCalleeInParent;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView _lvFunctionInfo;
		private System.Windows.Forms.ListView _lvChildInfo;
		private System.Windows.Forms.TreeView _tvNamespaceInfo;
		private System.Windows.Forms.ComboBox _cbCurrentThread;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this._lvFunctionInfo = new System.Windows.Forms.ListView();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalls = new System.Windows.Forms.ColumnHeader();
			this.colTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeInMethod = new System.Windows.Forms.ColumnHeader();
			this.colChildrenTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeSuspended = new System.Windows.Forms.ColumnHeader();
			this._lvChildInfo = new System.Windows.Forms.ListView();
			this.colCalleeID = new System.Windows.Forms.ColumnHeader();
			this.colCalleeSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalleeCalls = new System.Windows.Forms.ColumnHeader();
			this.colCalleeTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colCalleeInParent = new System.Windows.Forms.ColumnHeader();
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
			this.splitter2.Location = new System.Drawing.Point(195, 229);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(677, 3);
			this.splitter2.TabIndex = 10;
			this.splitter2.TabStop = false;
			// 
			// _lvFunctionInfo
			// 
			this._lvFunctionInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.colID,
																							  this.colSignature,
																							  this.colCalls,
																							  this.colTotalTime,
																							  this.colTimeInMethod,
																							  this.colChildrenTime,
																							  this.colTimeSuspended});
			this._lvFunctionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lvFunctionInfo.FullRowSelect = true;
			this._lvFunctionInfo.HideSelection = false;
			this._lvFunctionInfo.Location = new System.Drawing.Point(195, 24);
			this._lvFunctionInfo.MultiSelect = false;
			this._lvFunctionInfo.Name = "_lvFunctionInfo";
			this._lvFunctionInfo.Size = new System.Drawing.Size(677, 208);
			this._lvFunctionInfo.TabIndex = 3;
			this._lvFunctionInfo.View = System.Windows.Forms.View.Details;
			this._lvFunctionInfo.SelectedIndexChanged += new System.EventHandler(this._lvFunctionInfo_SelectedIndexChanged);
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
			// _lvChildInfo
			// 
			this._lvChildInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.colCalleeID,
																						   this.colCalleeSignature,
																						   this.colCalleeCalls,
																						   this.colCalleeTotalTime,
																						   this.colCalleeInParent});
			this._lvChildInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._lvChildInfo.FullRowSelect = true;
			this._lvChildInfo.HideSelection = false;
			this._lvChildInfo.Location = new System.Drawing.Point(195, 232);
			this._lvChildInfo.MultiSelect = false;
			this._lvChildInfo.Name = "_lvChildInfo";
			this._lvChildInfo.Size = new System.Drawing.Size(677, 224);
			this._lvChildInfo.TabIndex = 3;
			this._lvChildInfo.View = System.Windows.Forms.View.Details;
			this._lvChildInfo.DoubleClick += new System.EventHandler(this._lvChildInfo_DoubleClick);
			// 
			// colCalleeID
			// 
			this.colCalleeID.Text = "ID";
			this.colCalleeID.Width = 100;
			// 
			// colCalleeSignature
			// 
			this.colCalleeSignature.Text = "Signature";
			this.colCalleeSignature.Width = 400;
			// 
			// colCalleeCalls
			// 
			this.colCalleeCalls.Text = "# of Calls";
			// 
			// colCalleeTotalTime
			// 
			this.colCalleeTotalTime.Text = "% of Total";
			// 
			// colCalleeInParent
			// 
			this.colCalleeInParent.Text = "% of Parent";
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
			this._cbCurrentThread.Size = new System.Drawing.Size(121, 21);
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

			foreach ( ThreadInfo ti in _tic )
				_cbCurrentThread.Items.Add( ti );

			_cbCurrentThread.SelectedIndex = 0;
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

			IComparer pOldSorter = _lvFunctionInfo.ListViewItemSorter;
			try
			{
				_lvFunctionInfo.BeginUpdate();
				_lvFunctionInfo.ListViewItemSorter = null;

				ThreadInfo tiCurrentThread = _tic[ ( ( ThreadInfo )_cbCurrentThread.SelectedItem ).ID ];
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
				{
					if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
						continue;

					ListViewItem lvi = new ListViewItem( fi.ID.ToString() );
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
				_lvFunctionInfo.ListViewItemSorter = pOldSorter;
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

			IComparer pOldSorter = _lvChildInfo.ListViewItemSorter;
			try
			{
				_lvChildInfo.BeginUpdate();
				_lvChildInfo.ListViewItemSorter = null;

				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					ListViewItem lvi = new ListViewItem( cfi.ID.ToString() );
					lvi.SubItems.Add( cfi.Signature );
					lvi.SubItems.Add( cfi.Calls.ToString() );
					lvi.SubItems.Add( cfi.PercentOfTotalTimeInMethod.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( cfi.PercentOfParentTimeInMethod.ToString( "0.00;-0.00;0" ) );

					lvi.Tag = cfi;
					_lvChildInfo.Items.Add( lvi );
				}

				if ( fi.TotalSuspendedTicks > 0 )
				{
					ListViewItem lviSuspend = new ListViewItem( "(suspend)" );
					lviSuspend.SubItems.Add( "(thread suspended)" );
					lviSuspend.SubItems.Add( "-" );
					lviSuspend.SubItems.Add( fi.PercentOfTotalTimeSuspended.ToString( "0.00;-0.00;0" ) );
					lviSuspend.SubItems.Add( fi.PercentOfMethodTimeSuspended.ToString( "0.00;-0.00;0" ) );
					_lvChildInfo.Items.Add( lviSuspend );
				}
			}
			finally
			{
				_lvChildInfo.ListViewItemSorter = pOldSorter;
				_lvChildInfo.EndUpdate();
			}
		}

		private void ProfilerControl_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{

		}

		private void ProfilerControl_Load(object sender, System.EventArgs e)
		{
			_lvsFunction = new ListViewSorter();
			_lvsFunction.SetColumns( new ListViewCompareType[] {
				 
					 ListViewCompareType.Float,
					 ListViewCompareType.String,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
			} );
			_lvsFunction.SortColumn( _lvFunctionInfo, 1 );
			_lvFunctionInfo.ListViewItemSorter = _lvsFunction;
			_lvFunctionInfo.ColumnClick += new ColumnClickEventHandler( _lvsFunction.OnListViewColumnClick );

			_lvsCalleeFunction = new ListViewSorter();
			_lvsCalleeFunction.SetColumns( new ListViewCompareType[] {
					 ListViewCompareType.Float,
					 ListViewCompareType.String,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
					 ListViewCompareType.Float,
			} );		
			_lvsCalleeFunction.SortColumn( _lvChildInfo, 1 );
			_lvChildInfo.ListViewItemSorter = _lvsCalleeFunction;
			_lvChildInfo.ColumnClick += new ColumnClickEventHandler( _lvsCalleeFunction.OnListViewColumnClick );
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
			foreach ( ListViewItem lvi in _lvFunctionInfo.Items )
			{
				FunctionInfo fi = ( FunctionInfo )lvi.Tag;
				if ( fi.ID == nID )
				{
					_lvFunctionInfo.SelectedItems.Clear();
					lvi.Selected = true;
					lvi.EnsureVisible();
					_lvFunctionInfo.Refresh();
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
		private ListViewSorter _lvsFunction;
		private ListViewSorter _lvsCalleeFunction;
	}
}
