using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace UtilityLibrary.WinControls
{
	#region Enumerations
	enum TabControlStyle
	{
		Standard,
		Document,
		HighContrast,
		Skinned
	}
	#endregion
	
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem(false)] 
	public class TabControlEx : System.Windows.Forms.Control
	{
		#region Class Variables
		
		// Property backing variables
		TabControlStyle style = TabControlStyle.Standard;

        #endregion

		#region Constructors
		public TabControlEx()
		{

			
		}
		#endregion

		#region Overrides
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Graphics g = pe.Graphics;
			DrawTabControl(g);
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		#region Implementation
		void DrawTabControl(Graphics g)
		{
			DrawBackground(g);
		}

		void DrawBackground(Graphics g)
		{
			Rectangle rc = ClientRectangle;
			
			if ( style == TabControlStyle.Standard )
			{

			}
			else
			{

			}
		}

		#endregion


	}
}
