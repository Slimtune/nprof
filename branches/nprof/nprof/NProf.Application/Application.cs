using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace NProf.Application
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Application
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new NProf.GUI.ProfilerForm());
		}
	}
}
