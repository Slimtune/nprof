using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NProf.Glue.Profiler.Project;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProfilerRunControl.
	/// </summary>
	public class ProfilerRunControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _btnStop;
		private System.Windows.Forms.ColumnHeader _chMessage;
		private System.Windows.Forms.ListView _lvMessages;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProfilerRunControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
			this.label1 = new System.Windows.Forms.Label();
			this._lvMessages = new System.Windows.Forms.ListView();
			this._chMessage = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this._btnStop = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(168, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current state:";
			// 
			// _lvMessages
			// 
			this._lvMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._lvMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this._chMessage});
			this._lvMessages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this._lvMessages.Location = new System.Drawing.Point(8, 48);
			this._lvMessages.Name = "_lvMessages";
			this._lvMessages.Size = new System.Drawing.Size(824, 400);
			this._lvMessages.TabIndex = 1;
			this._lvMessages.View = System.Windows.Forms.View.Details;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Messages:";
			// 
			// _btnStop
			// 
			this._btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnStop.Location = new System.Drawing.Point(760, 8);
			this._btnStop.Name = "_btnStop";
			this._btnStop.TabIndex = 3;
			this._btnStop.Text = "Stop Run";
			// 
			// ProfilerRunControl
			// 
			this.Controls.Add(this._btnStop);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._lvMessages);
			this.Controls.Add(this.label1);
			this.Name = "ProfilerRunControl";
			this.Size = new System.Drawing.Size(840, 456);
			this.VisibleChanged += new System.EventHandler(this.ProfilerRunControl_VisibleChanged);
			this.Resize += new System.EventHandler(this.ProfilerRunControl_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		private Run _r;

		private void OnMessage( string strMessage )
		{
			_lvMessages.Invoke( new RunMessageCollection.MessageHandler( OnMessageUIThread ), new object[] { strMessage } );
		}

		private void OnMessageUIThread( string strMessage )
		{
			lock ( _lvMessages )
				_lvMessages.Items.Add( strMessage );
		}

		private void ProfilerRunControl_VisibleChanged(object sender, System.EventArgs e)
		{
			if ( !this.Visible )
			{
				_r.Messages.StopListening( new RunMessageCollection.MessageHandler( OnMessage ) );
			}
		}

		public Run ProfilerRun
		{
			set 
			{ 
				_r = value; 

				lock ( _lvMessages )
				{
					string[] astrMessages = _r.Messages.StartListening( new RunMessageCollection.MessageHandler( OnMessage ) );
					foreach ( string strMessage in astrMessages )
						OnMessageUIThread( strMessage );
				}
			}
		}

		private void ProfilerRunControl_Resize(object sender, System.EventArgs e)
		{
			_lvMessages.Columns[ 0 ].Width = -2;
		}
	}
}
