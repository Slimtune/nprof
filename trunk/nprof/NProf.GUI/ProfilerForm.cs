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
		private Stack _stackBack;
		private Stack _stackForward;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TabControl _tcProfilers;
		private System.Windows.Forms.ContextMenu _cmTabControl;
		private System.Windows.Forms.MenuItem menuItem1;

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
			_p = new Profiler();
			_p.ProcessCompleted += new Profiler.ProcessCompletedHandler( OnProfileComplete );
			_p.Error += new Profiler.ErrorHandler( OnError );
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
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this._tcProfilers = new System.Windows.Forms.TabControl();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this._cmTabControl = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
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
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this._tcProfilers,
																				 this.splitter2,
																				 this.splitter1});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 88);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(920, 589);
			this.panel1.TabIndex = 9;
			// 
			// _tcProfilers
			// 
			this._tcProfilers.ContextMenu = this._cmTabControl;
			this._tcProfilers.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tcProfilers.Location = new System.Drawing.Point(3, 0);
			this._tcProfilers.Name = "_tcProfilers";
			this._tcProfilers.SelectedIndex = 0;
			this._tcProfilers.Size = new System.Drawing.Size(917, 586);
			this._tcProfilers.TabIndex = 9;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(3, 586);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(917, 3);
			this.splitter2.TabIndex = 10;
			this.splitter2.TabStop = false;
			// 
			// splitter1
			// 
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 589);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.checkBox1,
																				 this.button2,
																				 this.textBox1,
																				 this.button1,
																				 this.label1,
																				 this.textBox2,
																				 this.label3,
																				 this.textBox3,
																				 this.label4,
																				 this.button3});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(920, 88);
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
			// _cmTabControl
			// 
			this._cmTabControl.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Close Tab";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
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
			_p.Start( textBox1.Text, textBox2.Text, textBox3.Text, po );
			this.Text = "nprof GUI - [Running]";
		}

		private void OnProfileComplete( ThreadInfoCollection tic )
		{
			this.BeginInvoke( new HandleProfileComplete( OnUIThreadProfileComplete ), new object[] { tic } );
		}

		private delegate void HandleProfileComplete( ThreadInfoCollection tic );

		private void OnUIThreadProfileComplete( ThreadInfoCollection tic )
		{
			TabPage tp = new TabPage( "Profiler [" + DateTime.Now + "]" );
			_tcProfilers.TabPages.Add( tp );
			ProfilerControl pc = new ProfilerControl();

			tp.Controls.Add( pc );
			pc.Dock = DockStyle.Fill;
			pc.ThreadInfoCollection = tic;

			this.Text = "nprof GUI";
		}

		private Profiler _p;

		private void button2_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Executable files (*.exe)|*.exe";
			DialogResult dr = ofd.ShowDialog();
			if ( dr == DialogResult.OK )
				textBox1.Text = ofd.FileName;
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

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			_tcProfilers.TabPages.Remove( _tcProfilers.SelectedTab );
		}
	}
}
