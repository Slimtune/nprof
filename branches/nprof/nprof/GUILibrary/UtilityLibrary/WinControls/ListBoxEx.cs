using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// ListBoxEx
	/// </summary>
	[ToolboxItem(false)]
	public class ListBoxEx : System.Windows.Forms.ListBox
	{
		#region Constructors
		public ListBoxEx()
		{
			DrawMode = DrawMode.OwnerDrawFixed;
			ItemHeight = ItemHeight + 1;
		}
		#endregion

		#region Overrides
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			DrawListBoxItem(e);
		}
		#endregion

		#region Implementation
		protected void DrawListBoxItem(DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle bounds = e.Bounds;
			bool selected = (e.State & DrawItemState.Selected) > 0;
			// Draw List box item
			if ( e.Index != -1)
			{
				if ( selected && Enabled )
				{
					Color fillColor = ColorUtil.VSNetSelectionColor;
					if ( !ContainsFocus ) fillColor = ColorUtil.VSNetControlColor;
					
					// Draw highlight rectangle
					using ( Brush b = new SolidBrush(fillColor) )
					{
						g.FillRectangle(b, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
					}
					if ( ContainsFocus )
					{
						using ( Pen p = new Pen(ColorUtil.VSNetBorderColor) )
						{
							g.DrawRectangle(p, bounds.Left, bounds.Top, bounds.Width-1, bounds.Height-1);
						}
					}
				}
				else
				{
					g.FillRectangle(SystemBrushes.Window, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
				}
				
				if ( Items.Count > 0 )
				{
					object currentObject = Items[e.Index];
					string item = currentObject as String;
					if ( item != null )
					{
						if ( Enabled )
							g.DrawString(item, SystemInformation.MenuFont, 
								SystemBrushes.ControlText, new Point(bounds.Left+2, bounds.Top));
						else
							g.DrawString(item, SystemInformation.MenuFont, 
								SystemBrushes.ControlDark, new Point(bounds.Left+2, bounds.Top));
					}
				}
				
			}
		}
		#endregion

	}
}
