using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// Summary description for VSNetButton.
	/// </summary>
	[ToolboxItem(false)]
	public class ButtonEx : System.Windows.Forms.Button
	{

		#region Class Variables
		bool gotFocus = false;
		bool mouseDown = false;
		bool mouseEnter = false;
		string hotKey = string.Empty;
		#endregion
		
		#region Constructors
		public ButtonEx()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true);
		}

		#endregion

		#region Overrides
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Graphics g = pe.Graphics;

			if ( mouseDown )
			{
				DrawButtonState(g, DrawState.Pressed);
				return;
			}

			if ( gotFocus || mouseEnter) 
			{
				DrawButtonState(g, DrawState.Hot);
				return;
			}
			
			if ( Enabled )
				DrawButtonState(pe.Graphics, DrawState.Normal);
			else
				DrawButtonState(pe.Graphics, DrawState.Disable);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			mouseEnter = true;
			base.OnMouseEnter(e);
			Invalidate();
		
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			mouseEnter = false;
			base.OnMouseLeave(e);
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
           	base.OnMouseDown(e);
			mouseDown = true;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			mouseDown = false;
			Invalidate();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			gotFocus = true;
			Invalidate();
		}
        
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			gotFocus = false;
			Invalidate();
		}

		#endregion
		
		#region Implementation
		void DrawButtonState(Graphics g, DrawState state)
		{
			DrawBackground(g, state);
			Rectangle rc = ClientRectangle;
			
			bool hasText = false;
			bool hasImage = Image != null;
			Size textSize = new Size(0,0);
			if ( Text != string.Empty && Text != "" )
			{
				hasText = true;
				textSize = TextUtil.GetTextSize(g, Text, Font);
			}
			
			int imageWidth = 0;
			int imageHeight = 0;
			if ( hasImage )
			{
				SizeF sizeF = Image.PhysicalDimension;
				imageWidth = (int)sizeF.Width;
				imageHeight = (int)sizeF.Height;
				// We are assuming that the button image is smaller than
				// the button itself
				if ( imageWidth > rc.Width || imageHeight > rc.Height)
				{
					Debug.WriteLine("Image dimensions need to be smaller that button's dimension...");
					return;
				}
			}

			int x, y;
			if ( hasText && !hasImage )
			{
				// Text only drawing
				x = (Width - textSize.Width)/2;
				y = (Height - textSize.Height)/2;
				DrawText(g, Text, state, x, y);
			}
			else if ( hasImage && !hasText )
			{
				// Image only drawing
				x = (Width - imageWidth)/2;
				y = (Height - imageHeight)/2;
				DrawImage(g, state, Image, x, y);
			}
			else 
			{
				// Text and Image drawing
				x = (Width - textSize.Width - imageWidth -2)/2;
				y = (Height - imageHeight)/2;
				if ( hasImage )
				{
					// In case it does not have text
					// make sure we test if it has an image
					DrawImage(g, state, Image, x, y);
					x += imageWidth + 2;
				}
				y = (Height - textSize.Height)/2;
				DrawText(g, Text, state, x, y);
			}
			
		}

		void DrawBackground(Graphics g, DrawState state)
		{
			Rectangle rc = ClientRectangle;
			// Draw background
			if ( state == DrawState.Normal || state == DrawState.Disable )
			{
				g.FillRectangle(new SolidBrush(ColorUtil.VSNetControlColor), rc);
				SolidBrush rcBrush;
				if ( state == DrawState.Disable )
				{
					rcBrush = new SolidBrush(ControlPaint.Dark(ColorUtil.VSNetControlColor));
				}
				else
					rcBrush = new SolidBrush(SystemColors.ControlDark);
				
				// Draw border rectangle
				g.DrawRectangle(new Pen(rcBrush), rc.Left, rc.Top, rc.Width-1, rc.Height-1);

			}
			else if ( state == DrawState.Hot || state == DrawState.Pressed  )
			{
				// Erase whaterver that was there before
				if ( state == DrawState.Hot )
					g.FillRectangle(new SolidBrush(ColorUtil.VSNetSelectionColor), rc);
				else
					g.FillRectangle(new SolidBrush(ColorUtil.VSNetPressedColor), rc);
				// Draw border rectangle
				using ( Pen p = new Pen(ColorUtil.VSNetBorderColor) )
				{
					g.DrawRectangle(p, rc.Left, rc.Top, rc.Width-1, rc.Height-1);
				}
			}
		}

		void DrawImage(Graphics g, DrawState state, Image image, int x, int y)
		{
			SizeF sizeF = Image.PhysicalDimension;
			int imageWidth = (int)sizeF.Width;
			int imageHeight = (int)sizeF.Height;
			
			if ( state == DrawState.Normal )
			{
				g.DrawImage(Image, x, y, imageWidth, imageHeight);
			}
			else if ( state == DrawState.Disable )
			{
				ControlPaint.DrawImageDisabled(g, Image, x, y, SystemColors.Control);
			}
			else if ( state == DrawState.Pressed || state == DrawState.Hot )
			{
				ControlPaint.DrawImageDisabled(g, Image, x+1, y, SystemColors.Control);
				g.DrawImage(Image, x, y-1, imageWidth, imageHeight);                 
			}
		}

		protected void DrawText(Graphics g, string Text, DrawState state, int x, int y)
		{ 
			if ( Text.IndexOf("&",0) != -1 ) 
			{
				// Hot key defined
				Font underlineFont = new Font(Font,System.Drawing.FontStyle.Underline);

				string pretext = Text.Substring(0,Text.IndexOf("&",0));
				string posttext = Text.Substring(Text.IndexOf("&",0)+ 2);
				hotKey = Text.Substring(Text.IndexOf("&",0)+1, 1);

				System.Drawing.Size szpre = new Size(0,0);
				System.Drawing.Size szkey = new Size(0,0);
				System.Drawing.Size szpost = new Size(0,0);

				szpre = TextUtil.GetTextSize(g, pretext,Font);
				szkey = TextUtil.GetTextSize(g, hotKey, underlineFont);
				szpost = TextUtil.GetTextSize(g,posttext, Font);
				
				System.Drawing.Brush brush;
				if ( state == DrawState.Disable ) 
					brush = SystemBrushes.ControlDark;
				else
					brush = SystemBrushes.ControlText;

				if ( pretext.Length > 0 ) 
				{
					g.DrawString(pretext, Font, brush, new Point(x, y));
					x += szpre.Width;
				}

				if ( hotKey.Length > 0 ) 
				{
					g.DrawString(hotKey, underlineFont, brush, new Point(x, y));
					x += szkey.Width;
				}

				if ( posttext.Length > 0 ) 
				{
					g.DrawString(posttext, Font, brush, new Point(x, y));
					x += szpost.Width;
				}
			}
			else 
			{
				// Normal Text
				if ( state == DrawState.Disable )
					g.DrawString(Text, Font, SystemBrushes.ControlDark, new Point(x, y));
				else
					g.DrawString(Text, Font, SystemBrushes.ControlText, new Point(x, y));
			}
		}
		#endregion

	}

}
