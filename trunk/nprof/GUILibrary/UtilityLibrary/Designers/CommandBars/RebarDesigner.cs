using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

namespace UtilityLibrary.Designers
{
	/// <summary>
	/// Summary description for ReBarDesigner.
	/// </summary>
	public class ReBarDesigner : System.Windows.Forms.Design.ControlDesigner
	{
	
		#region Helper functions

		internal void PassMsg(ref Message m)
		{
			UtilityLibrary.CommandBars.ReBar rebar = 
				Control as UtilityLibrary.CommandBars.ReBar;
			if ( rebar != null )
				WndProc(ref m);
		}
		#endregion
	}
}


