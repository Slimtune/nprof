using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// Summary description for Scrollable Picture
	/// </summary>
	[ToolboxItem(false)]
	public class ScrollablePicture : System.Windows.Forms.Control
	{
		#region Class Variables
		Bitmap image = null;
		IntPtr hImage = IntPtr.Zero;
		VScrollBarEx vScrollBar = null;
		HScrollBarEx hScrollBar = null;
				
		// Initial Picture Position
		int xPos = 0;
		int yPos = 0;
				     
		#endregion
		
		#region Constructors
		public ScrollablePicture()
		{
			// We are going to do all of the painting so better 
			// setup the control to use double buffering
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.ResizeRedraw|
				ControlStyles.Opaque|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true);
			TabStop = false;

			// Construct scrollbars controls
			vScrollBar = new VScrollBarEx(this);
			hScrollBar = new HScrollBarEx(this);

			// Setup event listeners
			vScrollBar.LineUp += new EventHandler(OnYPosChange);
			vScrollBar.LineDown += new EventHandler(OnYPosChange);
			vScrollBar.ThumbUp += new ThumbHandler(OnYThumbChange);
			vScrollBar.ThumbDown += new ThumbHandler(OnYThumbChange);
			vScrollBar.PageUp += new EventHandler(OnYPosChange);
			vScrollBar.PageDown += new EventHandler(OnYPosChange);

			hScrollBar.LineLeft += new EventHandler(OnXPosChange);
			hScrollBar.LineRight += new EventHandler(OnXPosChange);
			hScrollBar.ThumbLeft += new ThumbHandler(OnXThumbChange);
			hScrollBar.ThumbRight += new ThumbHandler(OnXThumbChange);
			hScrollBar.PageLeft += new EventHandler(OnXPosChange);
			hScrollBar.PageRight += new EventHandler(OnXPosChange);
		}
		#endregion

		#region Overrides
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Graphics g = pe.Graphics;
			if ( image != null )
			{
				Rectangle rc = ClientRectangle;
				// Fill out background wiht Control color
				g.FillRectangle(SystemBrushes.Control, rc);

				// Take into account whether scrollbars are
				// displayed or not
				int hThumb = hScrollBar.HThumb;
				int vThumb = hScrollBar.VThumb;
				if ( !vScrollBar.Visible ) hThumb = 0;
				if ( !hScrollBar.Visible ) vThumb = 0;	

                // GDI+ is too slow for my taste, do the painting using regular GDI
				GDIUtil.BlitBitmap(g, new Rectangle(0, 0, rc.Width-hThumb, rc.Height-vThumb), xPos, yPos, hImage);
				
			}

		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			// Add scrollBars to this control
			Controls.AddRange(new System.Windows.Forms.Control[] {hScrollBar, vScrollBar});
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			// Size changed, recalculate dimensions
			SetupScrollBars();
			base.OnSizeChanged(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if ( Visible )
			{
				// Wait until parent window is being displayed
				// to correctly calculate the scrollbars dimensions
				SetupScrollBars();
			}
			
		}
		#endregion

		#region Properties
		public VScrollBarEx VScrollBar
		{
			set 
			{
				vScrollBar = value; 
				vScrollBar.Invalidate();
			}
			get { return vScrollBar; }
		}

		public HScrollBarEx HScrollBar
		{
			set 
			{
				hScrollBar = value; 
				hScrollBar.Invalidate();
			}
			get { return hScrollBar; }
		}

		public Bitmap Image
		{
			set 
			{
				image = value;
				hImage = image.GetHbitmap();
				SetupScrollBars();
				Invalidate();
			}

			get { return image; }
		}

		#endregion

		#region Implementation
		void SetupScrollBars()
		{
			if ( image != null && ClientRectangle.Width != 0 && ClientRectangle.Height != 0)
			{
				// Check if we need to show the scrollbars first
				if ( image.Width <= ClientRectangle.Width )
				{
					hScrollBar.Visible = false;
				}
				else
				{
					// If it is not visible make it so
					hScrollBar.Visible = true;
				}
	
				if ( image.Height <= ClientRectangle.Height )
				{
					vScrollBar.Visible = false;
				}
				else
				{
					// If it is not visible make it so
					vScrollBar.Visible = true;
				}
			
				// Check if we need to leave the righ bottom empty square
				// when using both scrollbars
				if ( vScrollBar.Visible && hScrollBar.Visible)
				{
					hScrollBar.UsingBothScrollBars = true;
					vScrollBar.UsingBothScrollBars = true;
				}
				else 
				{
					hScrollBar.UsingBothScrollBars = false;
					vScrollBar.UsingBothScrollBars = false;
				}

				// Now dimension the scrollbars settings
				if ( hScrollBar.Visible )
				{
					int vThumb = hScrollBar.VThumb;
					if ( !vScrollBar.Visible ) vThumb = 0;

					int workingWidth = (ClientRectangle.Width - vThumb);
					hScrollBar.Position = 0;
					hScrollBar.Minimum = 0;
					hScrollBar.Maximum = image.Width;
					hScrollBar.LargeChange = workingWidth;
					hScrollBar.SmallChange = workingWidth/10;
					hScrollBar.Invalidate();
				}

				if ( vScrollBar.Visible )
				{
					int hThumb = vScrollBar.HThumb;
					if ( !hScrollBar.Visible ) hThumb = 0;
					
					int workingHeight = (ClientRectangle.Height - hThumb);
					vScrollBar.Position = 0;
					vScrollBar.Minimum = 0;
					vScrollBar.Maximum = image.Height;
					vScrollBar.LargeChange = workingHeight;
					vScrollBar.SmallChange = workingHeight/10;
					vScrollBar.Invalidate();
				}

				// Reset Picture position
				xPos = 0;
				yPos = 0;
			}
		}


		void OnYPosChange(object sender, EventArgs e)
		{
			yPos = vScrollBar.Position;
			Invalidate();

		}

		void OnYThumbChange(object sender, int delta)
		{
			yPos = vScrollBar.Position;
			Invalidate();
		}

		void OnXPosChange(object sender, EventArgs e)
		{
			xPos = hScrollBar.Position;
			Invalidate();

		}

		void OnXThumbChange(object sender, int delta)
		{
			xPos = hScrollBar.Position;
			Invalidate();
		}


		#endregion
	}
}
