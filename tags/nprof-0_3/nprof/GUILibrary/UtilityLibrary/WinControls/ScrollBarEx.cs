using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

using UtilityLibrary.Win32;
using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{

	#region Enumerations
	public enum ThumbDraggedFireFrequency
	{
		MouseMove,
		MouseUp
	}

	#endregion

	#region Delegates
	public delegate void ThumbHandler(object sender, int delta);
	#endregion

	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem(false)]
	// I wanted to create this class as an abstract base class
	// for the VScroolBarEx and the HScrollBarEx but the designer
	// has problems creating derived classes that have as their base
	// classes anything other than public classes
	// --It is frustrating that you cannot fully apply the idioms of the languages
	// because the IDE Designer and the reflection implementation fail to
	// honor those idioms--
	public class ScrollBarEx : System.Windows.Forms.Control
	{
		#region Events
		public event EventHandler StartingAutomaticScrolling = null;
		public event EventHandler StoppingAutomaticScrolling = null;
		public event EventHandler ThumbReleased = null;
		#endregion

		#region Class Variables
        
		// Constants
		protected const int GRIPPER_WIDTH = 8;
		protected const int GRIPPER_HEIGHT = 8;
		protected const int MINIMUM_THUMB_WITH_GRIPPER_SIZE = 12;
		protected const int TIMER_INTERVAL = 200;
        		
		// Parent control for the scrollbar
		protected Control parentWindow;

		// property backing variables
		int hThumb = -1;
		int vThumb = -1;
		int borderGap = 0;
		protected int min = 0;
		protected int max = 100;
		protected double pos = 0;
		protected double previousPos = 0;
		protected int smallChange = 10;
		protected int largeChange = 20;
		protected ThumbDraggedFireFrequency dragFrequency = ThumbDraggedFireFrequency.MouseMove;

		// Standard scrollbars properties
		protected Color backgroundColor = Color.Empty;
		protected Color foregroundColor = Color.Empty;
		protected Color borderColor = Color.Empty;
		protected Color arrowColor = Color.Empty;
		protected Color gripperLightColor = Color.Empty;
		protected Color gripperDarkColor = Color.Empty;
		protected Color hoverColor = Color.Empty;
		protected Color pressedColor = Color.Empty;
		protected Color gradientStartBackgroundColor = Color.Empty;
		protected Color gradientEndBackgroundColor = Color.Empty;
		protected Color gradientStartForegroundColor = Color.Empty;
		protected Color gradientEndForegroundColor = Color.Empty;

		// Skin support
		protected ImageList thumbImageList = null;
		protected Image scrollShaftImage = null;
		protected ImageList upArrowImageList = null;
		protected ImageList downArrowImageList = null;
                				
		// Keep track of the UI element state
		protected DrawState thumbDrawState = DrawState.Normal;

		// Other helper variables
		protected bool stopAutomaticScrolling = false;
		
		// Keeps track of the ScrollBar objects constructed
		// so that we can transparently--to the user--check if
		// an horizontal and vertical bars are both being used
		// on the same parent window to be able to leave the 
		// lower right corner empty space that avoids the two
		// scrollbars overlapping on the arrow button
		static ArrayList scrollBarList = new ArrayList();
		protected bool usingBothScrollBars = false;

		// Miscellaneous
		protected bool drawGripper = true;
                
		#endregion
		
		#region Constructors
		public ScrollBarEx()
		{

		}

		public ScrollBarEx(Control parentControl)
		{
			// We are going to do all of the painting so better 
			// setup the control to use double buffering
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.ResizeRedraw|
				ControlStyles.Opaque|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true);
			TabStop = false;

			hThumb = WindowsAPI.GetSystemMetrics(SystemMetricsCodes.SM_CXHTHUMB);
			vThumb = WindowsAPI.GetSystemMetrics(SystemMetricsCodes.SM_CYVTHUMB);
		
			parentWindow = parentControl;
			parentWindow.SizeChanged += new EventHandler(ParentSizeChanged);

			// Need to check setting of flag
			CheckForUsingBothScrollBarsFlag(this);
			scrollBarList.Add(this);
		}
		#endregion

		#region Overrides
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		
			// Set the parent for the scrollBar
			WindowsAPI.SetParent(Handle, parentWindow.Handle);
			SizeScrollBar();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Graphics g = pe.Graphics;
			DrawScrollBar(g);
		}

		protected override  void WndProc(ref Message message)
		{
			base.WndProc(ref message);

			switch (message.Msg)
			{	
				case (int)Msg.WM_LBUTTONUP:
					stopAutomaticScrolling = true;
					break;
				default:
					break;
			}
		}

		#endregion

		#region Virtuals
		// Need to be implemented by a derived class
		// This shoud have been abstract methods but 
		// because of the comments at the class declaration
		// above, this could not be
		protected virtual void SizeScrollBar()
		{
			
            			
		}
		protected virtual void DrawScrollBar(Graphics g)
		{
		}
		protected virtual Rectangle GetThumbRect()
		{
			return Rectangle.Empty;
		}
		protected virtual void DrawBackground(Graphics g)
		{
			Rectangle rc = ClientRectangle;
			Color backColor = SystemColors.ControlLight;
			if ( backgroundColor != Color.Empty )
				backColor = BackgroundColor;

			if ( scrollShaftImage != null )
			{
				// Draw background bitmap
				g.DrawImage(scrollShaftImage, rc);

			}
			else if ( gradientStartBackgroundColor != Color.Empty && gradientEndBackgroundColor != Color.Empty )
			{

				LinearGradientMode mode = LinearGradientMode.Horizontal;
				HScrollBarEx hs = this as HScrollBarEx;
				if ( hs != null )
					mode = LinearGradientMode.Vertical;
				
				using ( LinearGradientBrush b = new LinearGradientBrush( rc, gradientStartBackgroundColor, 
							gradientEndBackgroundColor, mode) )
				{
					g.FillRectangle(b, rc);
				}
			}
			else
			{
				using ( Brush b = new SolidBrush(backColor) )
				{
					// Fill background;
					g.FillRectangle(b, rc);
				}
			}
		}

		protected virtual void DrawFlatArrowButton(Graphics g, Rectangle rc, ArrowGlyph arrowGlyph, DrawState drawState)
		{
			// Make rectangle 1 pixel smaller
			// makes it look nicer
			rc.Inflate(-1, -1);
		
			Color border = Color.Empty;
			Color background = Color.Empty;

			if ( drawState == DrawState.Normal )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetControlColor;
				if ( backgroundColor != Color.Empty )
					background = foregroundColor;
			}
			else if ( drawState == DrawState.Hot )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetSelectionColor;
				if ( hoverColor != Color.Empty )
					background = hoverColor;
			}
			else if ( drawState == DrawState.Pressed )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetPressedColor;
				if ( pressedColor != Color.Empty )
					background = pressedColor;
			}

			// Which arrow gyph we need to draw
			bool upArrow = (arrowGlyph == ArrowGlyph.Up || arrowGlyph == ArrowGlyph.Left);
			bool paintBorder = true;
            
			if ( (upArrow && upArrowImageList != null && upArrowImageList.Images.Count > (int)drawState)
				|| (!upArrow && downArrowImageList != null && downArrowImageList.Images.Count > (int)drawState) )
			{
				if ( upArrow )
					g.DrawImage(upArrowImageList.Images[(int)drawState], rc);
				else
					g.DrawImage(downArrowImageList.Images[(int)drawState], rc);
				paintBorder = false;
			}
			else if ( gradientStartForegroundColor != Color.Empty && gradientEndForegroundColor != Color.Empty 
				&& drawState == DrawState.Normal )
			{
				using ( LinearGradientBrush b = new LinearGradientBrush( rc, gradientStartForegroundColor, 
							gradientEndForegroundColor, LinearGradientMode.Horizontal) )
				{
					g.FillRectangle(b, rc);
				}
			}
			else
			{
				using ( Brush b = new SolidBrush(background) )
				{
					// Fill background;
					g.FillRectangle(b, rc);
				}
			}
			
			// Check if the user set custom colors
			if ( borderColor != Color.Empty )
				border = borderColor;
			using ( Pen p = new Pen(border) )
			{
				if ( paintBorder )
                    g.DrawRectangle(p, rc.Left, rc.Top, rc.Width-1, rc.Height-1);
				Color arrow = Color.Black;
				if ( arrowColor != Color.Empty )
					arrow = arrowColor;
				using ( Brush b = new SolidBrush(arrow) ) 
				{
					GDIUtil.DrawArrowGlyph(g, rc, 7, 4, arrowGlyph, b);
				}
			}
		}

		protected virtual void DrawThumb(Graphics g, DrawState drawState)
		{
			Rectangle rc = GetThumbRect();
			Color border = Color.Empty;
			Color background = Color.Empty;

			if ( drawState == DrawState.Normal )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetControlColor;
				if ( backgroundColor != Color.Empty )
					background = foregroundColor;
			}
			else if ( drawState == DrawState.Hot )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetSelectionColor;
				if ( hoverColor != Color.Empty )
					background = hoverColor;
			}
			else if ( drawState == DrawState.Pressed )
			{
				border = ColorUtil.VSNetBorderColor;
				background = ColorUtil.VSNetPressedColor;
				if ( pressedColor != Color.Empty )
					background = pressedColor;
			}

			// Paint border by default
			bool paintBorder = true;
            
			if ( thumbImageList != null && thumbImageList.Images.Count > (int)drawState )
			{
				
				VScrollBarEx vScrollBarEx = this as VScrollBarEx;
				bool verticalScrollBar = ( vScrollBarEx != null);
				Image image = thumbImageList.Images[(int)drawState];
				
				// We need to draw the thumb in chunks to avoid distorting the thumb image
				// We are assuming that the corners of the image require at most 10 pixel
				// on both size, the rest will be the area we strech
				if ( !verticalScrollBar )
				{
					// Draw start corner
					if ( rc.Width > GRIPPER_WIDTH*2 )
					{
						g.DrawImage(image, new Rectangle(rc.Left, rc.Top, 10, rc.Height), 
							0, 0, 10, rc.Height, GraphicsUnit.Pixel);
						// Draw middle part
						g.DrawImage(image, new Rectangle(rc.Left+10, rc.Top, rc.Width-20, rc.Height), 
							10, 0, image.Width-20, rc.Height, GraphicsUnit.Pixel);
						// Draw end corner
						g.DrawImage(image, new Rectangle(rc.Right-10, rc.Top, 10, rc.Height), 
							image.Width-10, 0, 10, rc.Height, GraphicsUnit.Pixel);
					}
					else
					{
						// Width is too small, just draw the image
                        g.DrawImage(image, rc);
					}

				}
				else
				{
					if ( rc.Height > GRIPPER_HEIGHT*2 )
					{
						// Draw start corner
						g.DrawImage(image, new Rectangle(rc.Left, rc.Top, rc.Width, 10), 
							0, 0, rc.Width, 10, GraphicsUnit.Pixel);
						// Draw middle part
						g.DrawImage(image, new Rectangle(rc.Left, rc.Top+10, rc.Width, rc.Height-20), 
							0, 10, rc.Width, image.Height - 20, GraphicsUnit.Pixel);
						// Draw end corner
						g.DrawImage(image, new Rectangle(rc.Left, rc.Bottom-10, rc.Width, 10), 
							0, image.Height-10, rc.Width, 10, GraphicsUnit.Pixel);
					}
					else
					{
						// Height is too small, just draw the image
						g.DrawImage(image, rc);
					}

				}
               	paintBorder = false;
			}
			else if ( gradientStartForegroundColor != Color.Empty && gradientEndForegroundColor != Color.Empty )
			{
				Color startColor = gradientStartForegroundColor;
				if ( drawState == DrawState.Hot || drawState == DrawState.Pressed )
					startColor = background;
				
				LinearGradientMode mode = LinearGradientMode.Horizontal;
				HScrollBarEx hs = this as HScrollBarEx;
				if ( hs != null )
					mode = LinearGradientMode.Vertical;
				
				using ( LinearGradientBrush b = new LinearGradientBrush( rc, startColor, 
							gradientEndForegroundColor, mode) )
				{
					g.FillRectangle(b, rc);
				}
			}
			else
			{
				using ( Brush b = new SolidBrush(background) )
				{
					// Fill background;
					g.FillRectangle(b, rc);
				}
			}
			
			if ( paintBorder )
			{
				// If border color was set by the user
				if ( borderColor != Color.Empty )
					border = borderColor;
				using ( Pen p = new Pen(border) )
				{
					g.DrawRectangle(p, rc.Left, rc.Top, rc.Width-1, rc.Height-1);
				}
			}
		}

		#endregion

		#region Properties 
		public int Position
		{
			set
			{
				if ( pos != value )
				{
					previousPos = pos;
					int newValue = value;

					if ( newValue >= max-largeChange ) 
					{
						// Position cannot be larger than
						// max-largeChange
						pos = max-largeChange;
					}
					else if ( newValue < 0 )
					{
						// Negative values don't make sense
						pos = 0;
					}
					else
						pos = newValue;
					
					if ( previousPos != pos )
						Invalidate();
				}
				else
					previousPos = pos;
			}
			get 
			{
				return (int)pos;
			}
		}

		public int VThumb
		{
			get { return vThumb;}
		}

		public int HThumb
		{
			get { return hThumb;}
		}
		
		public int BorderGap
		{
			set { borderGap = value;}
			get { return borderGap; }
		}

		public int SmallChange
		{
			set 
			{
				if ( value < min || value > max)
					smallChange = 1;
				smallChange = value;				
			}
			get { return smallChange;}
		}

		public int LargeChange
		{
			set 
			{
				if ( value < min || value > max )
					largeChange = 1;
				largeChange = value;				
			}
			get { return largeChange;}
		}

		public int Minimum
		{
			set { min = value;}
			get { return min; }
		}

		public int Maximum
		{
			set { max = value;}
			get { return max; }
		}

		public int PreviousPosition
		{
			get { return (int)previousPos; }
		}

		public ThumbDraggedFireFrequency ThumbDraggedFireFrequency
		{
			set { dragFrequency = value; }
			get { return dragFrequency; }
		}

		public Color BackgroundColor
		{
			set { backgroundColor = value;}
			get { return backgroundColor; }
		}

		public Color ForegroundColor
		{
			set { foregroundColor = value;}
			get { return foregroundColor; }
		}

		public Color BorderColor
		{
			set { borderColor = value;}
			get { return borderColor; }
		}

		public Color PressedColor
		{
			set { pressedColor = value;}
			get { return pressedColor; }
		}

		public Color HoverColor
		{
			set { hoverColor = value;}
			get { return hoverColor; }
		}

		public Color ArrowColor
		{
			set { arrowColor = value;}
			get { return arrowColor; }
		}

		public Color GripperLightColor
		{
			set { gripperLightColor = value;}
			get { return gripperLightColor; }
		}

		public Color GripperDarkColor
		{
			set { gripperDarkColor = value;}
			get { return gripperDarkColor; }
		}
                
		public bool UsingBothScrollBars
		{
			set { usingBothScrollBars = value; }
			get { return usingBothScrollBars; }
		}

		public bool DrawGripper
		{
			set { 
				drawGripper = value;
				Invalidate();
			}
			get { return drawGripper; }
		}

		public Color GradientStartBackgroundColor
		{
			set { gradientStartBackgroundColor = value;}
			get { return gradientStartBackgroundColor; }
		}

		public Color GradientEndBackgroundColor
		{
			set { gradientEndBackgroundColor = value;}
			get { return gradientEndBackgroundColor; }
		}

		public Color GradientStartForegroundColor
		{
			set { gradientStartForegroundColor = value;}
			get { return gradientStartForegroundColor; }
		}

		public Color GradientEndForegroundColor
		{
			set { gradientEndForegroundColor = value;}
			get { return gradientEndForegroundColor; }
		}

		public ImageList ThumbImageList
		{
			set { thumbImageList = value; }
			get { return thumbImageList; }
		}

		public Image ScrollShaftImage
		{
			set { scrollShaftImage = value; }
			get { return scrollShaftImage; }
		}

		#endregion
		
		#region Implementation
		void CheckForUsingBothScrollBarsFlag(ScrollBarEx scrollBar)
		{
			for ( int i = 0; i < scrollBarList.Count; i++ )
			{
				ScrollBarEx currentScrollBar = (ScrollBarEx)scrollBarList[i];
				Debug.Assert(currentScrollBar != null);
				if ( currentScrollBar.parentWindow == scrollBar.parentWindow )
				{
					if ( currentScrollBar.GetType() != scrollBar.GetType() )
					{
						currentScrollBar.usingBothScrollBars = true;
						scrollBar.usingBothScrollBars = true;
					}
				}
			}
		}
		
		void ParentSizeChanged(object sender, EventArgs e)
		{
			SizeScrollBar();
		}

		protected void FireStartingAutomaticScrolling()
		{
			if ( StartingAutomaticScrolling != null )
				StartingAutomaticScrolling(this, EventArgs.Empty);
		}

		protected void FireStoppingAutomaticScrolling()
		{
			if ( StoppingAutomaticScrolling != null )
				StoppingAutomaticScrolling(this, EventArgs.Empty);
		}

		protected void FireThumbRelease()
		{
			if ( ThumbReleased != null )
				ThumbReleased(this, EventArgs.Empty);
		}

		#endregion

	}
}
