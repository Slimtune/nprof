using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NProf.Glue.Profiler.Project;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for ProfilerRunMessagesForm.
	/// </summary>
	public class ProfilerRunMessagesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader _chMessage;
		private System.Windows.Forms.ListView _lvMessages;
		private System.Windows.Forms.Button _btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProfilerRunMessagesForm()
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
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public Run ProfilerRun
		{
			set 
			{
				foreach ( string strMessage in value.Messages )
					_lvMessages.Items.Add( strMessage );

				_lvMessages.Columns[ 0 ].Width = -2;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lvMessages = new System.Windows.Forms.ListView();
			this.label1 = new System.Windows.Forms.Label();
			this._btnOK = new System.Windows.Forms.Button();
			this._chMessage = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// _lvMessages
			// 
			this._lvMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._lvMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this._chMessage});
			this._lvMessages.FullRowSelect = true;
			this._lvMessages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this._lvMessages.Location = new System.Drawing.Point(8, 32);
			this._lvMessages.Name = "_lvMessages";
			this._lvMessages.Size = new System.Drawing.Size(720, 360);
			this._lvMessages.TabIndex = 0;
			this._lvMessages.View = System.Windows.Forms.View.Details;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "Messages:";
			// 
			// _btnOK
			// 
			this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnOK.Location = new System.Drawing.Point(640, 408);
			this._btnOK.Name = "_btnOK";
			this._btnOK.TabIndex = 2;
			this._btnOK.Text = "OK";
			// 
			// _chMessage
			// 
			this._chMessage.Text = "Message";
			this._chMessage.Width = 672;
			// 
			// ProfilerRunMessagesForm
			// 
			this.AcceptButton = this._btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._btnOK;
			this.ClientSize = new System.Drawing.Size(736, 437);
			this.Controls.Add(this._btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._lvMessages);
			this.Name = "ProfilerRunMessagesForm";
			this.Text = "Profiler Run Messages";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
