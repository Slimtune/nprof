using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
		private UtilityLibrary.WinControls.SortedListView listView1;
		private UtilityLibrary.WinControls.SortedListView listView2;
		private System.Windows.Forms.Splitter splitter1;
		private UtilityLibrary.WinControls.TreeViewEx treeView1;
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
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProfilerControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

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
			this.listView1 = new UtilityLibrary.WinControls.SortedListView();
			this.listView2 = new UtilityLibrary.WinControls.SortedListView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.treeView1 = new UtilityLibrary.WinControls.TreeViewEx();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalls = new System.Windows.Forms.ColumnHeader();
			this.colTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeInMethod = new System.Windows.Forms.ColumnHeader();
			this.colChildrenTime = new System.Windows.Forms.ColumnHeader();
			this.colTimeSuspended = new System.Windows.Forms.ColumnHeader();
			this.colCalleeID = new System.Windows.Forms.ColumnHeader();
			this.colCalleeSignature = new System.Windows.Forms.ColumnHeader();
			this.colCalleeCalls = new System.Windows.Forms.ColumnHeader();
			this.colCalleeTotalTime = new System.Windows.Forms.ColumnHeader();
			this.colCalleeInParent = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
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
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(872, 456);
			this.panel1.TabIndex = 10;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(195, 173);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(677, 3);
			this.splitter2.TabIndex = 10;
			this.splitter2.TabStop = false;
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
			this.listView1.Size = new System.Drawing.Size(677, 176);
			this.listView1.TabIndex = 3;
			this.listView1.View = System.Windows.Forms.View.Details;
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
			this.listView2.Location = new System.Drawing.Point(195, 176);
			this.listView2.MultiSelect = false;
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(677, 280);
			this.listView2.TabIndex = 3;
			this.listView2.View = System.Windows.Forms.View.Details;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(192, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 456);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// treeView1
			// 
			this.treeView1.CheckBoxes = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(192, 456);
			this.treeView1.TabIndex = 8;
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
			this.colTimeInMethod.Text = "% in Me...\0";
			// 
			// colChildrenTime
			// 
			this.colChildrenTime.Text = "% in Chil...\0";
			// 
			// colTimeSuspended
			// 
			this.colTimeSuspended.Text = "% Susp....\0";
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
			// ProfilerControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel1});
			this.Name = "ProfilerControl";
			this.Size = new System.Drawing.Size(872, 456);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
