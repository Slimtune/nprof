using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

using UtilityLibrary.Win32;
using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem(false)]
	public class HScrollBarEx : UtilityLibrary.WinControls.ScrollBarEx
	{
		
		#region Events
		public event ThumbHandler ThumbLeft = null;
		public event ThumbHandler ThumbRight = null;
		public event EventHandler LineLeft = null;
		public event EventHandler LineRight = null;
		public event EventHandler PageLeft = null;
		public event EventHandler PageRight = null;
		#endregion
		
		#region Class variables
		DrawState leftArrowDrawState = DrawState.Normal;
		DrawState rightArrowDrawState = DrawState.Normal;
		bool ignoreMouseMove = false;
		bool draggingThumb = false;
		int oldMouseX = 0;
		ScrollBarHit currentTarget = ScrollBarHit.None;
		bool firstTick = false;
		double thumbPixelPos = 0;
		bool processingAutomaticScrolling = false;
		const int MINIMUM_THUMB_WIDTH = 6;
		#endregion
				
		#region Constructors		
		public HScrollBarEx()
		{

		}

		public HScrollBarEx(Control parent) : base(parent)
		{

		}
		
		#endregion

		#region Properties
		public ImageList LeftArrowImageList
		{
			set {  upArrowImageList = value; }
			get { return upArrowImageList; }
		}

		public ImageList RightArrowImageList
		{
			set {  downArrowImageList = value; }
			get { return downArrowImageList; }
		}

		#endregion

		#region Overrides
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if ( e.Button != MouseButtons.Left )
				return;

			ScrollBarHit hit = HitTest(new Point(e.X, e.Y));
			if ( hit == ScrollBarHit.LeftArrow )
			{
				leftArrowDrawState = DrawState.Hot;
				Position -= smallChange;
				FireLineLeft();
			}
			else if ( hit == ScrollBarHit.RightArrow )
			{
				rightArrowDrawState = DrawState.Hot;
				Position += smallChange;
				FireLineRight();
			}
			else if ( hit == ScrollBarHit.PageLeft )
			{
				Position -= largeChange;
				FirePageLeft();
			}
			else if ( hit == ScrollBarHit.PageRight )
			{
				Position += largeChange;
				FirePageRight();
			}
			else if ( hit == ScrollBarHit.Thumb )
			{
				Capture = true;
				draggingThumb = true;
				thumbDrawState = DrawState.Pressed;
				oldMouseX = e.X;
				thumbPixelPos = GetThumbPixelPosition(pos);
				Invalidate();
			}

			if ( !processingAutomaticScrolling )
			{
				if ( hit != ScrollBarHit.Thumb && hit != ScrollBarHit.None )
					ProcessScrolling(hit);
			}

		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			
			if ( ignoreMouseMove) 
			{
				ignoreMouseMove = false;
				return;
			}
			
			// Reset every thing to normal state
			leftArrowDrawState = DrawState.Normal;
			rightArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;

			ScrollBarHit hit = HitTest(new Point(e.X, e.Y));
			if ( hit == ScrollBarHit.LeftArrow )
			{
				leftArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.RightArrow )
			{
				rightArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.Thumb || draggingThumb )
			{
				if ( draggingThumb )
				{
					thumbDrawState = DrawState.Pressed;
					UpdatePosition(e.X);
					oldMouseX = e.X;
					if ( dragFrequency == ThumbDraggedFireFrequency.MouseMove )
					{
						if ( pos > previousPos )
							FireThumbRight((int)pos-(int)previousPos);
						else
							FireThumbLeft((int)previousPos-(int)pos);
					}
				}
				else
				{
					thumbDrawState = DrawState.Hot;
				}
			}
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if ( e.Button != MouseButtons.Left )
				return;

			// Reset drawing to normal state
			leftArrowDrawState = DrawState.Normal;
			rightArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;
			ignoreMouseMove = true;

			if ( draggingThumb )
			{
				Capture = false;
				thumbDrawState = DrawState.Normal;
				UpdatePosition(e.X);
				draggingThumb = false;
                
				if ( pos > previousPos )
				{
					FireThumbRight((int)pos-(int)previousPos);
				}
				else
					FireThumbLeft((int)previousPos-(int)pos);

				// For users who that want to know when the
				// Thumb is released
				FireThumbRelease();

			}
			Invalidate();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			// Set state to hot
			base.OnMouseEnter(e);
			Point pos = Control.MousePosition;
			pos = PointToClient(pos);

			ScrollBarHit hit = HitTest(pos);
			if ( hit == ScrollBarHit.LeftArrow )
			{
				leftArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.RightArrow )
			{
				rightArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.Thumb )
			{
				thumbDrawState = DrawState.Hot;
			}
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			// Set state to Normal
			base.OnMouseLeave(e);
			leftArrowDrawState = DrawState.Normal;
			rightArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;
			Invalidate();
		}

		protected override void SizeScrollBar()
		{
			// Resize scrollbar
			// Size scrollbar to have the standard dimensions
			// of an operating system created scrollbar
			Rectangle rcParent = parentWindow.ClientRectangle;
			if ( usingBothScrollBars )
			{
				Bounds =  new Rectangle(rcParent.Left+BorderGap, rcParent.Bottom - BorderGap*2 - HThumb,  
					rcParent.Width - BorderGap*2 - VThumb, HThumb);
			}
			else
			{
				Bounds =  new Rectangle(rcParent.Left+BorderGap, rcParent.Bottom - BorderGap*2 - HThumb,  
					rcParent.Width - BorderGap*2, HThumb);
			}
		}

		protected override void DrawScrollBar(Graphics g)
		{
			// Draw background
			DrawBackground(g);
						
			// Draw Button up arrow
			if ( Enabled )
			{
				// Up and Down buttons
				DrawArrowButtons(g);

				// Draw Thumb
				DrawThumb(g, thumbDrawState);

				// Draw Gripper
				if ( drawGripper )
					DrawThumbGripper(g, thumbDrawState);
			}
		}

		#endregion

		#region Implementation
		void FireThumbLeft(int delta)
		{
			if ( ThumbLeft != null && previousPos != pos )
				ThumbLeft(this, delta);
		}

		void FireThumbRight(int delta)
		{
			if ( ThumbRight != null && previousPos != pos )
				ThumbRight(this, delta);
		}

		void FireLineLeft()
		{
			if ( LineLeft != null && previousPos != pos )
				LineLeft(this, EventArgs.Empty);
		}

		void FireLineRight()
		{
			if ( LineRight != null && previousPos != pos )
				LineRight(this, EventArgs.Empty);
		}
        
		void FirePageLeft()
		{
			if ( PageLeft != null && previousPos != pos )
				PageLeft(this, EventArgs.Empty);
		}

		void FirePageRight()
		{
			if ( PageRight != null && previousPos != pos )
				PageRight(this, EventArgs.Empty);
		}

		void ProcessScrolling(ScrollBarHit hit)
		{
			// Capture the mouse
			stopAutomaticScrolling = false;
			Capture = true;
			firstTick = true;
					
			// Start timer
			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
			timer.Tick += new EventHandler(ScrollingTick);
			timer.Interval = TIMER_INTERVAL;
			timer.Start();
			currentTarget = hit;
                         
			while ( stopAutomaticScrolling == false ) 
			{
				// Check messages until we find a condition to break out of the loop
				Win32.MSG msg = new Win32.MSG();
				WindowsAPI.GetMessage(ref msg, 0, 0, 0);
				Point point = new Point(0,0);
				if ( msg.message == (int)Msg.WM_MOUSEMOVE 
					|| msg.message == (int)Msg.WM_LBUTTONUP || msg.message == (int)Msg.WM_LBUTTONDBLCLK )
				{
					point = WindowsAPI.GetPointFromLPARAM((int)msg.lParam);
				}

				Msg thisMessage = (Msg)msg.message;
				switch(msg.message)
				{
					case (int)Msg.WM_MOUSEMOVE:
					{
						ScrollBarHit current = HitTest(point);
						ProcessMouseMoveScrolling(current);
						Invalidate();
						break;
					}
					case (int)Msg.WM_LBUTTONUP:
					case (int)Msg.WM_LBUTTONDBLCLK:
					{
						stopAutomaticScrolling = true;
						WindowsAPI.DispatchMessage(ref msg);
						break;
					}
					case (int)Msg.WM_KEYDOWN:
					{
						if ( (int)msg.wParam == (int)VirtualKeys.VK_ESCAPE) 
							stopAutomaticScrolling = true;
						break;
					}
					default:
						WindowsAPI.DispatchMessage(ref msg);
						break;
				}
			}

			// Stop timer
			timer.Stop();
			timer.Dispose();
			Invalidate();
			// Release the capture
			Capture = false;
			if ( processingAutomaticScrolling )
			{
				processingAutomaticScrolling = false;
				FireStoppingAutomaticScrolling();
			}
		}

		void ProcessMouseMoveScrolling(ScrollBarHit hit)
		{
			leftArrowDrawState = DrawState.Normal;
			rightArrowDrawState = DrawState.Normal;
			if ( hit == ScrollBarHit.LeftArrow )
				leftArrowDrawState = DrawState.Hot;
			else if ( hit == ScrollBarHit.RightArrow )
				rightArrowDrawState = DrawState.Hot;
            
		}

		void ScrollingTick(Object timeObject, EventArgs eventArgs) 
		{
			
			processingAutomaticScrolling = true;
			FireStartingAutomaticScrolling();
			
			// Ignore the first tick since sometimes the user
			// is just clicking and the first tick happens
			// so fast that produces the effect of scrolling twice
			if ( firstTick )
			{
				firstTick = false;
				return;
			}
								
			// Get mouse coordinates
			Point point = Control.MousePosition;
			point = PointToClient(point);
			Rectangle rc = Rectangle.Empty;
			
			if ( currentTarget == ScrollBarHit.LeftArrow )
			{
				rc = GetArrowButtonRectangle(true);
				if ( rc.Contains(point) )
				{
					Position -= smallChange;
					FireLineLeft();
				}
			}
			else if ( currentTarget == ScrollBarHit.RightArrow )
			{
				rc = GetArrowButtonRectangle(false);
				if ( rc.Contains(point) )
				{
					Position += smallChange;
					FireLineRight();
				}
			}
			else if ( currentTarget == ScrollBarHit.PageLeft )
			{
				rc = GetPageRect(true);
				if ( rc.Contains(point) )
				{
					Position -= largeChange;
					FirePageLeft();
				}
			}
			else if ( currentTarget == ScrollBarHit.PageRight )
			{
				rc = GetPageRect(false);
				if ( rc.Contains(point) )
				{
					Position += largeChange;
					FirePageRight();
				}
			}
		}

		void SetPosition(double fpos)
		{
			if ( pos != fpos )
			{
				previousPos = pos;
				double newValue = fpos;

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

		void UpdatePosition(int xPos)
		{
			int increment = 0;
			if ( xPos >= oldMouseX )
				increment = xPos - oldMouseX;
			else
				increment = (oldMouseX - xPos)*(-1);
		
			thumbPixelPos += increment; 
			double fPos = GetThumbLogicalPosition(thumbPixelPos);
			SetPosition(fPos);
		}

		protected virtual void DrawArrowButtons(Graphics g)
		{
			Rectangle leftRect = GetArrowButtonRectangle(true); 
			Rectangle rightRect = GetArrowButtonRectangle(false);
			DrawFlatArrowButton(g, leftRect, ArrowGlyph.Left, leftArrowDrawState);
			DrawFlatArrowButton(g, rightRect, ArrowGlyph.Right, rightArrowDrawState);
		}


		protected virtual void DrawThumbGripper(Graphics g, DrawState drawState)
		{
			Rectangle rc = GetThumbRect();

			// Don't draw it if it won't fit
			if ( rc.Width < MINIMUM_THUMB_WITH_GRIPPER_SIZE )
				return;

			int xMiddle = rc.Left + rc.Width/2;
			int yPos = rc.Top + (rc.Height - GRIPPER_HEIGHT)/2;
            
			Color lightColor = ColorUtil.VSNetSelectionColor;
			Color darkColor = ColorUtil.VSNetPressedColor;

			// Check if the user set custom colors for the gripper
			if ( gripperLightColor != Color.Empty )
				lightColor = gripperLightColor;
			if ( gripperDarkColor != Color.Empty )
				darkColor = gripperDarkColor;

			Pen lightPen = new Pen(lightColor);
			Pen darkPen = new Pen(darkColor);

			Point pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2, yPos);
			Point pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2, yPos+GRIPPER_WIDTH);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+1, yPos+1);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+1, yPos+GRIPPER_WIDTH+1);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+2, yPos);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+2, yPos+GRIPPER_WIDTH);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+3, yPos+1);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+3, yPos+GRIPPER_WIDTH+1);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+4, yPos);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+4, yPos+GRIPPER_WIDTH);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+5, yPos+1);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+5, yPos+GRIPPER_WIDTH+1);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+6, yPos);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+6, yPos+GRIPPER_WIDTH);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xMiddle-GRIPPER_HEIGHT/2+7, yPos+1);
			pt2 = new Point(xMiddle-GRIPPER_HEIGHT/2+7, yPos+GRIPPER_WIDTH+1);
			g.DrawLine(darkPen, pt1, pt2);

			lightPen.Dispose();
			darkPen.Dispose();
			
		}

		Rectangle GetArrowButtonRectangle( bool leftButton)
		{
			Rectangle rc = ClientRectangle;
			if ( leftButton )
			{
				return new Rectangle(rc.Left, 0, HThumb, HThumb);
			}
			else
			{
				return new Rectangle(rc.Right-HThumb, 0, HThumb, HThumb);
			}
		}

		protected override Rectangle GetThumbRect()
		{
			double thumbWidth = GetThumbPixelSize();
			int drawPos = 0;

			if ( draggingThumb  ) 
			{
				// If dragging the thumb don't use
				// a position based on the scaled calculation
				// but the actual one from the OnMouseMove event
				// to avoid jerkiness from rounding off errors
				drawPos = GetSafeThumbPixelPos((int)thumbWidth);
			}
			else
			{
				drawPos = (int)GetThumbPixelPosition(pos);
			}

			// To avoid rounding off errors
			if ( pos == max-largeChange )
				drawPos = ClientRectangle.Width - HThumb - (int)thumbWidth;
			Rectangle rc = Rectangle.Empty;

			// If width is too small, don't let it disappear
			if ( thumbWidth < MINIMUM_THUMB_WIDTH )
				thumbWidth = MINIMUM_THUMB_WIDTH;
			
			// Smaller than the total width of the scrollbar
			// to make it look nicer
			rc = new Rectangle(drawPos, 0, (int)thumbWidth, HThumb);
			rc.Inflate(0, -1);
			return rc;
		}

		int GetSafeThumbPixelPos(int thumbWidth)
		{
			if ( thumbPixelPos > (ClientRectangle.Width - HThumb)- thumbWidth ) 
			{
				// Position cannot be larger than
				// max-largeChange
				return (ClientRectangle.Width - HThumb)-thumbWidth;
			}
			else if ( thumbPixelPos <= HThumb )
			{
				// Negative values don't make sense
				return HThumb;
			}
			else
				return (int)thumbPixelPos;
		}

		Rectangle GetPageRect(bool left)
		{
			Rectangle rcClient = ClientRectangle;
			Rectangle rcThumb = GetThumbRect();
			Rectangle pageRect;
			if ( left )
			{
				pageRect = new Rectangle(rcClient.Left+HThumb+1, 
					rcClient.Top, rcThumb.Left-HThumb-2, rcClient.Height);
			}
			else
			{
				pageRect = new Rectangle(rcThumb.Right+1, 
					rcClient.Top, rcClient.Right-HThumb-1, rcClient.Height);
			}
			return pageRect;
		}

		double GetThumbPixelSize()
		{
			Rectangle rc = ClientRectangle;
			int width = rc.Width - HThumb*2;
			if ( largeChange == 0 || (max-min) == 0)
				return 0;
			float numOfPages = (float)(max-min)/(float)largeChange;
			return width/numOfPages;                         
		}

		double GetThumbPixelPosition(double logicalPos)
		{
			double fWidth = ClientRectangle.Width - HThumb*2;
			double fRange = (max-min);
			double fLogicalPos = logicalPos;
			return (fLogicalPos*fWidth)/fRange + HThumb;
		}

		double GetThumbLogicalPosition(double pixelPos)
		{
			double fWidth = ClientRectangle.Width - HThumb*2;
			double fRange = (max-min);
			double fpixelPos = pixelPos;
			return (fRange*(fpixelPos-HThumb)/fWidth);
		}

		ScrollBarHit HitTest(Point point)
		{
			Rectangle leftArrow = GetArrowButtonRectangle(true);
			if ( leftArrow.Contains(point) )
				return ScrollBarHit.LeftArrow;
			
			Rectangle rightArrow = GetArrowButtonRectangle(false);
			if ( rightArrow.Contains(point) )
				return ScrollBarHit.RightArrow;

			Rectangle leftPageRect = GetPageRect(true);
			if ( leftPageRect.Contains(point) )
				return ScrollBarHit.PageLeft;

			Rectangle rightPageRect = GetPageRect(false);
			if ( rightPageRect.Contains(point) )
				return ScrollBarHit.PageRight;

			Rectangle thumbRect = GetThumbRect();
			if ( thumbRect.Contains(point) )
				return ScrollBarHit.Thumb;
            
			return ScrollBarHit.None;
		}
		#endregion

	
	}
}
