using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for NoProfilerRunsLoaded.
	/// </summary>
	public class NoProfilerRunsLoaded : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NoProfilerRunsLoaded()
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
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(504, 368);
			this.label1.TabIndex = 0;
			this.label1.Text = "No profiler runs are currently loaded.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// NoProfilerRunsLoaded
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1});
			this.Name = "NoProfilerRunsLoaded";
			this.Size = new System.Drawing.Size(504, 368);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
