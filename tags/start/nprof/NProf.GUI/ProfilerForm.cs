using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Info;
using UtilityLibrary.WinControls;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ProfilerForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private SortedListView listView1;
		private SortedListView listView2;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.ColumnHeader colSignature;
		private System.Windows.Forms.ColumnHeader colCalls;
		private System.Windows.Forms.ColumnHeader colTotalTime;
		private System.Windows.Forms.ColumnHeader colChildrenTime;
		private Stack _stackBack;
		private Stack _stackForward;
		private System.Windows.Forms.ColumnHeader colCalleeID;
		private System.Windows.Forms.ColumnHeader colCalleeSignature;
		private System.Windows.Forms.ColumnHeader colCalleeCalls;
		private System.Windows.Forms.ColumnHeader colCalleeInParent;
		private System.Windows.Forms.ColumnHeader colCalleeTotalTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ColumnHeader colTimeSuspended;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ColumnHeader colTimeInMethod;
		private TreeViewEx treeView1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		private Hashtable _htCheckedItems;
		private bool _bInCheck;
		private bool _bUpdating;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox checkBox1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProfilerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_stackBack = new Stack();
			_stackForward = new Stack();
			_bInCheck = false;
			_bUpdating = false;
			_p = new Profiler();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.listView1 = new UtilityLibrary.WinControls.SortedListView();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalls = new System.Windows.Forms.ColumnHeader();
			this.colTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeInMethod = new System.Windows.Forms.ColumnHeader();
			this.colChildrenTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeSuspended = new System.Windows.Forms.ColumnHeader();
			this.listView2 = new UtilityLibrary.WinControls.SortedListView();
			this.colCalleeID = new System.Windows.Forms.ColumnHeader();
			this.colCalleeSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalleeCalls = new System.Windows.Forms.ColumnHeader();
			this.colCalleeTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colCalleeInParent = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.treeView1 = new UtilityLibrary.WinControls.TreeViewEx();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(104, 8);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(408, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(616, 8);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Start";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.colID,
																						this.colSignature,
																						this.colCalls,
																						this.colTotalTime,
																						this.colTimeInMethod,
																						this.colChildrenTime,
																						this.colTimeSuspended});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderImageList = null;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(195, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(725, 293);
			this.listView1.TabIndex = 3;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.Layout += new System.Windows.Forms.LayoutEventHandler(this.listView1_Layout);
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
			this.colTimeInMethod.Text = "% in Me...";
			// 
			// colChildrenTime
			// 
			this.colChildrenTime.Text = "% in Chil...";
			// 
			// colTimeSuspended
			// 
			this.colTimeSuspended.Text = "d";
			// 
			// listView2
			// 
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.colCalleeID,
																						this.colCalleeSignature,
																						this.colCalleeCalls,
																						this.colCalleeTotalTime,
																						this.colCalleeInParent});
			this.listView2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listView2.FullRowSelect = true;
			this.listView2.HeaderImageList = null;
			this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView2.HideSelection = false;
			this.listView2.Location = new System.Drawing.Point(195, 293);
			this.listView2.MultiSelect = false;
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(725, 280);
			this.listView2.TabIndex = 3;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.DoubleClick += new System.EventHandler(this.listView2_DoubleClick);
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
			this.colCalleeInParent.Text = "% of Par...";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 5;
			this.label1.Text = "Application to run:";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(528, 8);
			this.button2.Name = "button2";
			this.button2.TabIndex = 6;
			this.button2.Text = "Browse...";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Location = new System.Drawing.Point(72, 80);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(192, 21);
			this.comboBox1.TabIndex = 7;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Thread ID:";
			// 
			// treeView1
			// 
			this.treeView1.CheckBoxes = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(192, 573);
			this.treeView1.TabIndex = 8;
			this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.splitter2,
																				 this.listView1,
																				 this.listView2,
																				 this.splitter1,
																				 this.treeView1});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 104);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(920, 573);
			this.panel1.TabIndex = 9;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(195, 290);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(725, 3);
			this.splitter2.TabIndex = 10;
			this.splitter2.TabStop = false;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(192, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 573);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.checkBox1,
																				 this.button2,
																				 this.textBox1,
																				 this.label2,
																				 this.button1,
																				 this.label1,
																				 this.comboBox1,
																				 this.textBox2,
																				 this.label3,
																				 this.textBox3,
																				 this.label4,
																				 this.button3});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(920, 104);
			this.panel2.TabIndex = 10;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(704, 8);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.TabIndex = 8;
			this.checkBox1.Text = "Debug profilee";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(104, 32);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(408, 20);
			this.textBox2.TabIndex = 0;
			this.textBox2.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 32);
			this.label3.Name = "label3";
			this.label3.TabIndex = 5;
			this.label3.Text = "Arguments:";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(104, 56);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(408, 20);
			this.textBox3.TabIndex = 0;
			this.textBox3.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Name = "label4";
			this.label4.TabIndex = 5;
			this.label4.Text = "Working directory:";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(528, 56);
			this.button3.Name = "button3";
			this.button3.TabIndex = 6;
			this.button3.Text = "Browse...";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// ProfilerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(920, 677);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel1,
																		  this.panel2});
			this.Name = "ProfilerForm";
			this.Text = "nprof GUI - Alpha v0.2";
			this.Leave += new System.EventHandler(this.ProfilerForm_Leave);
			this.Deactivate += new System.EventHandler(this.ProfilerForm_Deactivate);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void EnableAndStart()
		{
			_p.EnableAndStart( new ProfilerOptions() );
			_p.ProcessCompleted += new Profiler.ProcessCompletedHandler( OnProfileComplete );
			_p.Error += new Profiler.ErrorHandler( OnError );
			this.Text = "nprof GUI - [Running]";
			textBox1.Enabled = false;
			textBox2.Enabled = false;
			textBox3.Enabled = false;
			button1.Enabled = false;
			button2.Enabled = false;
			button3.Enabled = false;
		}

		private void OnError( Exception e )
		{
			MessageBox.Show( this, e.Message, "Error" );
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			string strMessage;
			bool bSuccess = _p.CheckSetup( out strMessage );
			if ( !bSuccess )
			{
				MessageBox.Show( this, strMessage, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			ProfilerOptions po = new ProfilerOptions();
			po.Debug = checkBox1.Checked;
			_p.ProcessCompleted += new Profiler.ProcessCompletedHandler( OnProfileComplete );
			_p.Error += new Profiler.ErrorHandler( OnError );
			_p.Start( textBox1.Text, textBox2.Text, textBox3.Text, po );
			this.Text = "nprof GUI - [Running]";
		}

		private void OnProfileComplete( ThreadInfoCollection tic )
		{
			_tic = tic;
			listView2.BeginInvoke( new UpdateUIDelegate( UpdateUI ), new object[ 0 ] );
			this.Text = "nprof GUI";
		}

		private delegate void UpdateUIDelegate();

		private void UpdateUI()
		{
			comboBox1.Items.Clear();
			foreach ( ThreadInfo ti in _tic )
				comboBox1.Items.Add( ti );
			comboBox1.SelectedIndex = 0;
		}

		private Profiler _p;
		private ThreadInfoCollection _tic;

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( listView1.SelectedItems.Count == 0 )
			{
				listView2.Items.Clear();
				return;
			}
			FunctionInfo fi = ( FunctionInfo )listView1.SelectedItems[ 0 ].Tag;
			listView2.Items.Clear();
			try
			{
				listView2.BeginUpdate();
				foreach ( CalleeFunctionInfo cfi in fi.CalleeInfo )
				{
					ListViewItem lvi = new ListViewItem( cfi.ID.ToString() );
					lvi.SubItems.Add( cfi.Signature );
					lvi.SubItems.Add( cfi.Calls.ToString() );
					lvi.SubItems.Add( cfi.TotalTime.ToString() );
					lvi.SubItems.Add( cfi.TotalTime.ToString() );

					lvi.Tag = cfi;
					listView2.Items.Add( lvi );
				}

				if ( fi.TotalSuspendedTime > 0 )
				{
					ListViewItem lviSuspend = new ListViewItem( "(suspend)" );
					lviSuspend.SubItems.Add( "(thread suspended)" );
					lviSuspend.SubItems.Add( "-" );
					lviSuspend.SubItems.Add( fi.TimeSuspended.ToString() );
					lviSuspend.SubItems.Add( fi.TimeSuspended.ToString() );
				}
			}
			finally
			{
				listView2.EndUpdate();
			}
			_stackBack.Push( fi.ID );
			_stackForward.Clear();
		}

		private void listView1_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			listView1.SetColumnSortFormat( 0, SortedListViewFormatType.Numeric );
			listView1.SetColumnSortFormat( 1, SortedListViewFormatType.String );
			listView1.SetColumnSortFormat( 2, SortedListViewFormatType.Numeric );
			listView1.SetColumnSortFormat( 3, SortedListViewFormatType.Numeric );
			listView1.SetColumnSortFormat( 4, SortedListViewFormatType.Numeric );
			listView1.SetColumnSortFormat( 5, SortedListViewFormatType.Numeric );
			listView1.SetColumnSortFormat( 6, SortedListViewFormatType.Numeric );

			listView2.SetColumnSortFormat( 0, SortedListViewFormatType.Numeric );
			listView2.SetColumnSortFormat( 1, SortedListViewFormatType.String );
			listView2.SetColumnSortFormat( 2, SortedListViewFormatType.Numeric );
			listView2.SetColumnSortFormat( 3, SortedListViewFormatType.Numeric );
			listView2.SetColumnSortFormat( 4, SortedListViewFormatType.Numeric );
		}

		private void listView2_DoubleClick(object sender, System.EventArgs e)
		{
			if ( listView2.SelectedItems.Count == 0 )
				return;

			CalleeFunctionInfo cfi = ( CalleeFunctionInfo )listView2.SelectedItems[ 0 ].Tag;
			JumpToID( cfi.ID );
		}

		private void toolBarItem1_Click(object sender, System.EventArgs e)
		{
			if ( _stackBack.Count == 1 )
				return;

			_stackForward.Push( GetSelectedID() );
			_stackBack.Pop();
			int nLastID = ( int )_stackBack.Pop();
			JumpToID( nLastID );
		}

		private void toolBarItem2_Click(object sender, System.EventArgs e)
		{
			if ( _stackForward.Count == 0 )
				return;

			_stackBack.Push( GetSelectedID() );
			int nNextID = ( int )_stackForward.Pop();
			JumpToID( nNextID );
		}

		private int GetSelectedID()
		{
			if ( listView2.SelectedItems.Count == 0 )
				return -1;

			return ( ( FunctionInfo )listView1.SelectedItems[ 0 ].Tag ).ID;
		}

		private void JumpToID( int nID )
		{
			foreach ( ListViewItem lvi in listView1.Items )
			{
				FunctionInfo fi = ( FunctionInfo )lvi.Tag;
				if ( fi.ID == nID )
				{
					listView1.SelectedItems.Clear();
					lvi.Selected = true;
					lvi.EnsureVisible();
					listView1.Refresh();
					break;
				}
			}
		}

		private void toolBarItem1_DropDown(object sender, System.EventArgs e)
		{
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Executable files (*.exe)|*.exe";
			DialogResult dr = ofd.ShowDialog();
			if ( dr == DialogResult.OK )
				textBox1.Text = ofd.FileName;
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateTree();
			UpdateFilters();
		}

		private void treeView1_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
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

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			UpdateFilters();
		}

		private void UpdateTree()
		{
			treeView1.Nodes.Clear();

			_bUpdating = true;
			_htCheckedItems = new Hashtable();

			try
			{
				treeView1.BeginUpdate();

				TreeNode tnRoot = treeView1.Nodes.Add( "All Namespaces" );
				tnRoot.Tag = "";
				tnRoot.Checked = true;

				ThreadInfo tiCurrentThread = _tic[ ( ( ThreadInfo )comboBox1.SelectedItem ).ID ];
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
				treeView1.EndUpdate();
			}
		}

		private void UpdateFilters()
		{
			listView1.Items.Clear();
			listView2.Items.Clear();

			Hashtable htNamespaces = new Hashtable();
			foreach ( TreeNode tn in _htCheckedItems.Keys )
				htNamespaces[ tn.Tag ] = null;

			IComparer pOldSorter = listView1.ListViewItemSorter;
			try
			{
				listView1.BeginUpdate();
				listView1.ListViewItemSorter = null;

				ThreadInfo tiCurrentThread = _tic[ ( ( ThreadInfo )comboBox1.SelectedItem ).ID ];
				foreach ( FunctionInfo fi in tiCurrentThread.FunctionInfoCollection )
				{
					if ( !htNamespaces.Contains( fi.Signature.NamespaceString ) )
						continue;

					ListViewItem lvi = new ListViewItem( fi.ID.ToString() );
					lvi.SubItems.Add( fi.Signature.Signature );
					lvi.SubItems.Add( fi.Calls.ToString() );
					lvi.SubItems.Add( fi.TimeInMethodAndChildren.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.TimeInMethod.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.TimeInChildren.ToString( "0.00;-0.00;0" ) );
					lvi.SubItems.Add( fi.TimeSuspended.ToString( "0.00;-0.00;0" ) );
					listView1.Items.Add( lvi );
					lvi.Tag = fi;
				}
			}
			finally
			{
				listView1.ListViewItemSorter = pOldSorter;
				listView1.EndUpdate();
			}
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			ShellFolderBrowser fb = new ShellFolderBrowser();
			if ( textBox3.Text.Trim() == String.Empty )
			{
				if ( textBox1.Text.Trim() != String.Empty )
				{
					FileInfo fi = new FileInfo( textBox1.Text );
					fb.FolderPath = fi.DirectoryName;
				}
			}
			else
			{
				fb.FolderPath = textBox3.Text;
			}

			if ( fb.ShowDialog() )
			{
				textBox3.Text = fb.FolderPath;
			}
		}

		private void ProfilerForm_Leave(object sender, System.EventArgs e)
		{
		}

		private void ProfilerForm_Deactivate(object sender, System.EventArgs e)
		{
		}
	}
}
