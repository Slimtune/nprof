using System;
using System.Drawing;
using UtilityLibrary.General;

namespace UtilityLibrary.Menus
{
	/// <summary>
	/// Summary description for ColorGroup.
	/// Helper class to get VSNet IDE colors
	/// </summary>
	public class ColorGroup 
	{
		public ColorGroup(Color bgcolor, Color stripecolor, Color selectioncolor, Color bordercolor)
		{
			this.bgcolor = bgcolor;
			this.stripecolor = stripecolor;
			this.selectioncolor = selectioncolor;
			this.bordercolor = bordercolor;
		}

		Color bgcolor;
		Color stripecolor;
		Color selectioncolor;
		Color bordercolor;

		public Color bgColor
		{
			get 
			{
				return bgcolor;
			}
		}
		
		public Color stripeColor
		{
			get 
			{
				return stripecolor;
			}
		}

		public Color selectionColor
		{
			get 
			{
				return selectioncolor;
			}
		}

		public Color borderColor
		{
			get 
			{
				return bordercolor;
			}
		}

		public static ColorGroup GetColorGroup()
		{
			ColorGroup colorGroup = null;
			Color backgroundColor = ColorUtil.VSNetBackgroundColor;
			Color selectionColor = ColorUtil.VSNetSelectionColor; 
			Color stripeColor = ColorUtil.VSNetStripeColor;
			colorGroup = new ColorGroup(backgroundColor, stripeColor, selectionColor, 
				Color.FromArgb(255, SystemColors.Highlight)); 
				
			return colorGroup;
		}
		
	}

}
