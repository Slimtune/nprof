using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using DotNetLib.Windows.Forms;

namespace ExtendedListTest
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.listView = new DotNetLib.Windows.Forms.ContainerListView();
			this.containerListViewColumnHeader1 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.containerListViewColumnHeader2 = new DotNetLib.Windows.Forms.ContainerListViewColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.prpGrid = new System.Windows.Forms.PropertyGrid();
			this.propertySplit = new System.Windows.Forms.Splitter();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.button4 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listView.BackgroundImage")));
			this.listView.Columns.AddRange(new DotNetLib.Windows.Forms.ContainerListViewColumnHeader[] {
																																		 this.containerListViewColumnHeader1,
																																		 this.containerListViewColumnHeader2});
			this.listView.ColumnTracking = true;
			this.listView.Location = new System.Drawing.Point(8, 40);
			this.listView.Name = "listView";
			this.listView.SelectedImageList = this.imageList1;
			this.listView.ShowPlusMinus = true;
			this.listView.ShowRootTreeLines = true;
			this.listView.ShowTreeLines = true;
			this.listView.Size = new System.Drawing.Size(632, 520);
			this.listView.SmallImageList = this.imageList1;
			this.listView.TabIndex = 0;
			this.listView.TabStop = false;
			this.listView.SelectedItemsChanged += new System.EventHandler(this.listView_SelectedItemsChanged);
			this.listView.PopItemContextMenu += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_PopItemContextMenu);
			this.listView.PopColumnHeaderContextMenu += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_PopColumnHeaderContextMenu);
			this.listView.ItemExpanding += new DotNetLib.Windows.Forms.ContainerListViewCancelEventHandler(this.listView_BeforeItemExpanded);
			this.listView.ItemCollapsed += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_AfterItemCollapsed);
			this.listView.ItemExpanded += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_AfterItemExpanded);
			this.listView.PopContextMenu += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_PopContextMenu);
			this.listView.ColumnClick += new DotNetLib.Windows.Forms.ContainerListViewEventHandler(this.listView_ColumnClick);
			this.listView.ItemCollapsing += new DotNetLib.Windows.Forms.ContainerListViewCancelEventHandler(this.listView_BeforeItemCollapsed);
			// 
			// containerListViewColumnHeader1
			// 
			this.containerListViewColumnHeader1.SortDataType = DotNetLib.Windows.Forms.SortDataType.String;
			this.containerListViewColumnHeader1.Text = "dd";
			// 
			// containerListViewColumnHeader2
			// 
			this.containerListViewColumnHeader2.DisplayIndex = 1;
			this.containerListViewColumnHeader2.SortDataType = DotNetLib.Windows.Forms.SortDataType.Integer;
			this.containerListViewColumnHeader2.Text = "ddd";
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// prpGrid
			// 
			this.prpGrid.CommandsVisibleIfAvailable = true;
			this.prpGrid.Dock = System.Windows.Forms.DockStyle.Right;
			this.prpGrid.LargeButtons = false;
			this.prpGrid.LineColor = System.Drawing.SystemColors.ActiveBorder;
			this.prpGrid.Location = new System.Drawing.Point(656, 0);
			this.prpGrid.Name = "prpGrid";
			this.prpGrid.SelectedObject = this.listView;
			this.prpGrid.Size = new System.Drawing.Size(272, 574);
			this.prpGrid.TabIndex = 0;
			this.prpGrid.Text = "PropertyGrid";
			this.prpGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.prpGrid.ViewForeColor = System.Drawing.SystemColors.ControlText;
			// 
			// propertySplit
			// 
			this.propertySplit.Dock = System.Windows.Forms.DockStyle.Right;
			this.propertySplit.Location = new System.Drawing.Point(653, 0);
			this.propertySplit.Name = "propertySplit";
			this.propertySplit.Size = new System.Drawing.Size(3, 574);
			this.propertySplit.TabIndex = 1;
			this.propertySplit.TabStop = false;
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.button4);
			this.pnlMain.Controls.Add(this.checkBox1);
			this.pnlMain.Controls.Add(this.button1);
			this.pnlMain.Controls.Add(this.listView);
			this.pnlMain.Controls.Add(this.button2);
			this.pnlMain.Controls.Add(this.button3);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(653, 574);
			this.pnlMain.TabIndex = 26;
			this.pnlMain.TabStop = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(320, 8);
			this.button4.Name = "button4";
			this.button4.TabIndex = 29;
			this.button4.Text = "button4";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(208, 16);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(14, 14);
			this.checkBox1.TabIndex = 28;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBox1_MouseDown);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(16, 8);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(104, 8);
			this.button2.Name = "button2";
			this.button2.TabIndex = 27;
			this.button2.Text = "button2";
			this.button2.Click += new System.EventHandler(this.button2_Click_1);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(232, 8);
			this.button3.Name = "button3";
			this.button3.TabIndex = 27;
			this.button3.Text = "button3";
			this.button3.Click += new System.EventHandler(this.button3_Click_1);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(928, 574);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.propertySplit);
			this.Controls.Add(this.prpGrid);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.Run(new Form1());
		}
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.PropertyGrid prpGrid;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.Splitter propertySplit;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader containerListViewColumnHeader1;
		private System.Windows.Forms.Button button1;
		private DotNetLib.Windows.Forms.ContainerListViewColumnHeader containerListViewColumnHeader2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private DotNetLib.Windows.Forms.ContainerListView listView;

		private void AddTree(int depth, int itemsPerDepth, ContainerListViewItemCollection items, string str)
		{
			if(depth == 0)
				return;

			for(int i = 0; i < itemsPerDepth; ++i)
			{
				Control ctrl = null;
				//				ProgressBar pb = new ProgressBar();
				//				pb.Maximum = 100;
				//				pb.Minimum = 0;
				//				pb.Value = (idx % 100) + 1;
				//				ctrl = pb;

				string itemText = str + " - " + i.ToString();
				ContainerListViewItem item = AddItem(items, itemText, itemText + " - 0y", itemText + " - 1", itemText + " - 2", itemText + " - 3", itemText + " - 4", ctrl);

				AddTree(depth - 1, itemsPerDepth, item.Items, item.Text);
			}
		}

		Random r = new Random();

		private ContainerListViewItem AddItem(ContainerListViewItemCollection items, string text1, string text2, string text3, string text4, string text5, string text6, Control ctrl)
		{
			ContainerListViewItem lvi = new ContainerListViewItem();
			items.Add(lvi);
			lvi.Text = text1;
			lvi.Height = 17 + r.Next(100);
			lvi.ForeColor = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
			lvi.BackColor = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));

			FontStyle style = FontStyle.Regular;

			if(r.Next(5) == 0)
				style |= FontStyle.Bold;

			if(r.Next(5) == 0)
				style |= FontStyle.Italic;

			if(r.Next(5) == 0)
				style |= FontStyle.Strikeout;

			if(r.Next(5) == 0)
				style |= FontStyle.Underline;

//			Font fnt = lv.Font;

//			lvi.Font = new Font(fnt.FontFamily, r.Next(50) + 1, style);

			lvi.SubItems[1].Text = text2;
			lvi.SubItems[2].Text = text3;
			lvi.SubItems[3].Text = text4;
			lvi.SubItems[4].Text = text5;

			lvi.SubItems[5].ItemControl = ctrl;
			lvi.SubItems[5].Text = text6;

			return lvi;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			timer1.Enabled = false;
//			if(lv.Items.Count > 0)
//			{
//				DateTime t = DateTime.Now;

//				ContainerListViewItem item = lv.Items[0];

//				for(int idx = 0; idx < 50 && item != null; ++idx)
//				{
//					item.Selected = true;
//					item.Focused = true;
//					lv.EnsureVisible();
//
//					if(!item.Expanded)
//						item.Expanded = true;
//					else
//						item = item.NextVisibleItem;
//
//					Application.DoEvents();
//				}
//
//				this.Text = DateTime.Now.Subtract(t).ToString();
//			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
//			lv.Items.Clear();
//
//			lv.ShowPlusMinus = true;
//			lv.ShowRootTreeLines = true;
//			lv.ShowTreeLines = true;
//
//			lv.BeginUpdate();
//			AddTree(6, 3, lv.Items, string.Empty);
//			lv.EndUpdate();
//			timer1.Enabled = true;
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
//			lv.Items.Clear();
//
//			lv.BeginUpdate();
//			for(int idx = 0; idx < 1000; ++idx)
//			{
//				string itemText = idx.ToString();
//				AddItem(lv.Items, itemText, itemText + " - 1", itemText + " - 2", itemText + " - 3", itemText + " - 4", itemText + " - 5", null);
//			}
//			lv.EndUpdate();
//			timer1.Enabled = true;
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Application.Exit();
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
//			lv.GridLineColor = Color.Black;
//
//			lv.Items.Clear();
//
//			lv.BeginUpdate();
//			for(int idx = 0; idx < 10000; ++idx)
//			{
//				string itemText = idx.ToString();
//				AddItem(lv.Items, itemText, itemText + " - 1", itemText + " - 2", itemText + " - 3", itemText + " - 4", itemText + " - 5", null);
//			}
//
//			lv.EndUpdate();
		}

		private void button2_Click_1(object sender, System.EventArgs e)
		{
			listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			ContainerListViewItem item = listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			listView.Items.Add(listView.Items.Count.ToString(), 0, 1);

			item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);

			item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item.Items.Add(item.Items.Count.ToString(), 0, 1);

			item.Height = 40;
			CheckBox bx = new CheckBox();
			bx.Size = new Size(14, 14);
			item.SubItems[1].ItemControl = bx;
			item.SubItems[1].ControlResizeBehavior = ControlResizeBehavior.None;

			int idx = 0;
			foreach(ContainerListViewItem it in item.Items)
				foreach(ContainerListViewSubItem subItem in it.SubItems)
					subItem.Text = (idx++).ToString();
		}

		private void listView_ColumnClick(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("ColumnClick " + e.ColumnHeader.Index.ToString());
		}

		private void listView_PopColumnHeaderContextMenu(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("PopColumnHeader" + e.ColumnHeader.Index.ToString());
		}

		private void listView_PopContextMenu(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("PopContextMenu");
		}

		private void listView_PopItemContextMenu(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("PopItemContextMenu " + e.Item.Text);
		}

		private void listView_SelectedItemsChanged(object sender, System.EventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("SelectedItemsChanged");
		}

		private void listView_AfterItemCollapsed(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("AfterItemCollapsed");
		}

		private void listView_AfterItemExpanded(object sender, DotNetLib.Windows.Forms.ContainerListViewEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("AfterItemExpanded");
		}

		private void listView_BeforeItemCollapsed(object sender, DotNetLib.Windows.Forms.ContainerListViewCancelEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("BeforeItemCollapsed");
			e.Cancel = checkBox1.Checked;
		}

		private void listView_BeforeItemExpanded(object sender, DotNetLib.Windows.Forms.ContainerListViewCancelEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine("BeforeItemExpanded");
			e.Cancel = checkBox1.Checked;
		}

		private void checkBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		
		}

		private void button3_Click_1(object sender, System.EventArgs e)
		{
			ContainerListViewItem item = listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
			item = item.Items.Add(item.Items.Count.ToString(), 0, 1);
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			DateTime t = DateTime.Now;
			listView.BeginUpdate();

			int size = 25000;
			ContainerListViewItem item = listView.Items.Add(listView.Items.Count.ToString(), 0, 1);
			for(int idx = 0; idx < size; ++idx)
				item.Items.Add("longer" + idx.ToString());

			item = listView.Items.Add(item.Items.Count.ToString(), 0, 1);
			for(int idx = 0; idx < size * 2; ++idx)
				item.Items.Add(idx.ToString());

			listView.EndUpdate();
			MessageBox.Show(DateTime.Now.Subtract(t).TotalSeconds.ToString());
		}
	}
}
