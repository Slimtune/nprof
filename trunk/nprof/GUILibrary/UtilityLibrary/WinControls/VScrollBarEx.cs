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
	public class VScrollBarEx : ScrollBarEx
	{
		#region Events
		public event ThumbHandler ThumbUp = null;
		public event ThumbHandler ThumbDown = null;
		public event EventHandler LineUp = null;
		public event EventHandler LineDown = null;
		public event EventHandler PageUp = null;
		public event EventHandler PageDown = null;
		#endregion
		
		#region Class variables
		DrawState upArrowDrawState = DrawState.Normal;
		DrawState downArrowDrawState = DrawState.Normal;
		bool ignoreMouseMove = false;
		bool draggingThumb = false;
		int oldMouseY = 0;
		ScrollBarHit currentTarget = ScrollBarHit.None;
		bool firstTick = false;
		double thumbPixelPos = 0;
		bool processingAutomaticScrolling = false;
		const int MINIMUM_THUMB_HEIGHT = 6;
		#endregion
				
		#region Constructors		
		public VScrollBarEx()
		{

		}

		public VScrollBarEx(Control parent) : base(parent)
		{

		}
		
		#endregion

		#region Properties
		public ImageList UpArrowImageList
		{
			set {  upArrowImageList = value; }
			get { return upArrowImageList; }
		}

		public ImageList DownArrowImageList
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
			if ( hit == ScrollBarHit.UpArrow )
			{
				upArrowDrawState = DrawState.Hot;
				Position -= smallChange;
				FireLineUp();
			}
			else if ( hit == ScrollBarHit.DownArrow )
			{
				downArrowDrawState = DrawState.Hot;
				Position += smallChange;
				FireLineDown();
			}
			else if ( hit == ScrollBarHit.PageUp )
			{
				Position -= largeChange;
				FirePageUp();
			}
			else if ( hit == ScrollBarHit.PageDown )
			{
				Position += largeChange;
				FirePageDown();
			}
			else if ( hit == ScrollBarHit.Thumb )
			{
				Capture = true;
				draggingThumb = true;
				thumbDrawState = DrawState.Pressed;
				oldMouseY = e.Y;
                thumbPixelPos = GetThumbPixelPosition(pos);
				Invalidate();
			}

			// Don't create reentry problems
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
			upArrowDrawState = DrawState.Normal;
			downArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;

			ScrollBarHit hit = HitTest(new Point(e.X, e.Y));
			if ( hit == ScrollBarHit.UpArrow )
			{
				upArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.DownArrow )
			{
				downArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.Thumb || draggingThumb )
			{
				if ( draggingThumb )
				{
					thumbDrawState = DrawState.Pressed;
					UpdatePosition(e.Y);
					oldMouseY = e.Y;
					if ( dragFrequency == ThumbDraggedFireFrequency.MouseMove )
					{
						if ( pos > previousPos )
							FireThumbDown((int)pos-(int)previousPos);
						else
							FireThumbUp((int)previousPos-(int)pos);
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
			upArrowDrawState = DrawState.Normal;
			downArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;
			ignoreMouseMove = true;

			if ( draggingThumb )
			{
				Capture = false;
				thumbDrawState = DrawState.Normal;
				UpdatePosition(e.Y);
				draggingThumb = false;
                
				if ( pos > previousPos )
				{
					FireThumbDown((int)pos-(int)previousPos);
				}
				else
					FireThumbUp((int)previousPos-(int)pos);

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
			if ( hit == ScrollBarHit.UpArrow )
			{
				upArrowDrawState = DrawState.Hot;
			}
			else if ( hit == ScrollBarHit.DownArrow )
			{
				downArrowDrawState = DrawState.Hot;
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
			upArrowDrawState = DrawState.Normal;
			downArrowDrawState = DrawState.Normal;
			thumbDrawState = DrawState.Normal;
			Invalidate();
		}

		protected override void SizeScrollBar()
		{
			// Resize scrollbar
			// Size scrollbar to have the standard dimensions
			// of an operating system created scrollbar
			Rectangle rcParent = parentWindow.ClientRectangle;
			// If both scrollbar are being used
			if ( usingBothScrollBars )
			{
				Bounds =  new Rectangle(rcParent.Right-VThumb-BorderGap, 
					rcParent.Top+BorderGap, VThumb, rcParent.Bottom - BorderGap*2 - HThumb);
			}
			else
			{
				Bounds =  new Rectangle(rcParent.Right-VThumb-BorderGap, 
					rcParent.Top+BorderGap, VThumb, rcParent.Bottom - BorderGap*2);
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
		void FireThumbUp(int delta)
		{
			if ( ThumbUp != null && previousPos != pos )
				ThumbUp(this, delta);
		}

		void FireThumbDown(int delta)
		{
			if ( ThumbDown != null && previousPos != pos )
				ThumbDown(this, delta);
		}

		void FireLineUp()
		{
			if ( LineUp != null && previousPos != pos )
				LineUp(this, EventArgs.Empty);
		}

		void FireLineDown()
		{
			if ( LineDown != null && previousPos != pos )
				LineDown(this, EventArgs.Empty);
		}
        
		void FirePageUp()
		{
			if ( PageUp != null && previousPos != pos )
				PageUp(this, EventArgs.Empty);
		}

		void FirePageDown()
		{
			if ( PageDown != null && previousPos != pos )
				PageDown(this, EventArgs.Empty);
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
			upArrowDrawState = DrawState.Normal;
			downArrowDrawState = DrawState.Normal;
			if ( hit == ScrollBarHit.UpArrow )
				upArrowDrawState = DrawState.Hot;
			else if ( hit == ScrollBarHit.DownArrow )
				downArrowDrawState = DrawState.Hot;
            
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
			
			if ( currentTarget == ScrollBarHit.UpArrow )
			{
				rc = GetArrowButtonRectangle(true);
				if ( rc.Contains(point) )
				{
					Position -= smallChange;
					FireLineUp();
				}
			}
			else if ( currentTarget == ScrollBarHit.DownArrow )
			{
				rc = GetArrowButtonRectangle(false);
				if ( rc.Contains(point) )
				{
					Position += smallChange;
					FireLineDown();
				}
			}
			else if ( currentTarget == ScrollBarHit.PageUp )
			{
				rc = GetPageRect(true);
				if ( rc.Contains(point) )
				{
					Position -= largeChange;
					FirePageUp();
				}
			}
			else if ( currentTarget == ScrollBarHit.PageDown )
			{
				rc = GetPageRect(false);
				if ( rc.Contains(point) )
				{
					Position += largeChange;
					FirePageDown();
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

		void UpdatePosition(int yPos)
		{
			int increment = 0;
			if ( yPos >= oldMouseY )
				increment = yPos - oldMouseY;
			else
				increment = (oldMouseY - yPos)*(-1);
		
			thumbPixelPos += increment; 
			double fPos = GetThumbLogicalPosition(thumbPixelPos);
			SetPosition(fPos);
		}

		protected virtual void DrawArrowButtons(Graphics g)
		{
			Rectangle upRect = GetArrowButtonRectangle(true); 
			Rectangle downRect = GetArrowButtonRectangle(false);
			DrawFlatArrowButton(g, upRect, ArrowGlyph.Up, upArrowDrawState);
			DrawFlatArrowButton(g, downRect, ArrowGlyph.Down, downArrowDrawState);
		}

		protected virtual void DrawThumbGripper(Graphics g, DrawState drawState)
		{
			Rectangle rc = GetThumbRect();

			// Don't draw it if it won't fit
			if ( rc.Height < MINIMUM_THUMB_WITH_GRIPPER_SIZE )
				return;

			int yMiddle = rc.Top + rc.Height/2;
			int xPos = rc.Left + (rc.Width - GRIPPER_WIDTH)/2;
            
			Color lightColor = ColorUtil.VSNetSelectionColor;
			Color darkColor = ColorUtil.VSNetPressedColor;

			// Check if the user set custom colors for the gripper
			if ( gripperLightColor != Color.Empty )
				lightColor = gripperLightColor;
			if ( gripperDarkColor != Color.Empty )
				darkColor = gripperDarkColor;
			
			Pen lightPen = new Pen(lightColor);
			Pen darkPen = new Pen(darkColor);

			Point pt1 = new Point(xPos, yMiddle-GRIPPER_HEIGHT/2);
			Point pt2 = new Point(xPos + GRIPPER_WIDTH, yMiddle-GRIPPER_HEIGHT/2);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xPos+1, yMiddle-GRIPPER_HEIGHT/2+1);
			pt2 = new Point(xPos + GRIPPER_WIDTH + 1, yMiddle-GRIPPER_HEIGHT/2+1);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xPos, yMiddle-GRIPPER_HEIGHT/2+2);
			pt2 = new Point(xPos + GRIPPER_WIDTH, yMiddle-GRIPPER_HEIGHT/2+2);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xPos+1, yMiddle-GRIPPER_HEIGHT/2+3);
			pt2 = new Point(xPos + GRIPPER_WIDTH + 1, yMiddle-GRIPPER_HEIGHT/2+3);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xPos, yMiddle-GRIPPER_HEIGHT/2+4);
			pt2 = new Point(xPos + GRIPPER_WIDTH, yMiddle-GRIPPER_HEIGHT/2+4);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xPos+1, yMiddle-GRIPPER_HEIGHT/2+5);
			pt2 = new Point(xPos + GRIPPER_WIDTH+1, yMiddle-GRIPPER_HEIGHT/2+5);
			g.DrawLine(darkPen, pt1, pt2);

			pt1 = new Point(xPos, yMiddle-GRIPPER_HEIGHT/2+6);
			pt2 = new Point(xPos + GRIPPER_WIDTH, yMiddle-GRIPPER_HEIGHT/2+6);
			g.DrawLine(lightPen, pt1, pt2);

			pt1 = new Point(xPos+1, yMiddle-GRIPPER_HEIGHT/2+7);
			pt2 = new Point(xPos + GRIPPER_WIDTH + 1, yMiddle-GRIPPER_HEIGHT/2+7);
			g.DrawLine(darkPen, pt1, pt2);

			// Cleanup 
			lightPen.Dispose();
			darkPen.Dispose();
			
		}

		Rectangle GetArrowButtonRectangle( bool upButton)
		{
			Rectangle rc = ClientRectangle;
			if ( upButton )
			{
				return new Rectangle(0, rc.Top,
					VThumb, VThumb);
			}
			else
			{
				return new Rectangle(0, rc.Bottom-VThumb,
					VThumb, VThumb);
			}
		}

		protected override Rectangle GetThumbRect()
		{
			double thumbHeight = GetThumbPixelSize();
			int drawPos = 0;

			if ( draggingThumb  ) 
			{
				// If dragging the thumb don't use
				// a position based on the scaled calculation
				// but the actual one from the OnMouseMove event
				// to avoid jerkiness from rounding off errors
				drawPos = GetSafeThumbPixelPos((int)thumbHeight);
			}
			else
			{
				drawPos = (int)GetThumbPixelPosition(pos);
			}

			// To avoid rounding off errors
			if ( pos == max-largeChange )
			{
				drawPos = ClientRectangle.Height - VThumb - (int)thumbHeight;
			}
			Rectangle rc = Rectangle.Empty;

			// If width is too small, don't let it disappear
			if ( thumbHeight < MINIMUM_THUMB_HEIGHT )
				thumbHeight = MINIMUM_THUMB_HEIGHT;
			
			// Smaller than the total width of the scrollbar
			// to make it look nicer
			rc = new Rectangle(0, drawPos, VThumb, (int)thumbHeight);
			rc.Inflate(-1, 0);
			return rc;
		}

		int GetSafeThumbPixelPos(int thumbHeight)
		{
			if ( thumbPixelPos > (ClientRectangle.Height - VThumb)-thumbHeight) 
			{
				// Position cannot be larger than
				// max-largeChange
				return (ClientRectangle.Height - VThumb)-thumbHeight;
			}
			else if ( thumbPixelPos <= VThumb )
			{
				// Negative values don't make sense
				return VThumb;
			}
			else
				return (int)thumbPixelPos;
		}

		Rectangle GetPageRect(bool up)
		{
			Rectangle rcClient = ClientRectangle;
			Rectangle rcThumb = GetThumbRect();
			Rectangle pageRect;
			if ( up )
			{
				pageRect = new Rectangle(rcClient.Left, 
					rcClient.Top+VThumb+1, rcClient.Width, rcThumb.Top-VThumb-2);
			}
			else
			{
				pageRect = new Rectangle(rcClient.Left, 
					rcThumb.Bottom+1, rcClient.Width, rcClient.Bottom-VThumb-1);
			}
			return pageRect;
		}

		double GetThumbPixelSize()
		{
			Rectangle rc =ClientRectangle;
			int height = rc.Height - VThumb*2;
			if ( largeChange == 0 || (max-min) == 0)
				return 0;
			float numOfPages = (float)(max-min)/(float)largeChange;
			return height/numOfPages;                         
		}

		double GetThumbPixelPosition(double logicalPos)
		{
			double fHeight = ClientRectangle.Height - VThumb*2;
			double fRange = (max-min);
			double fLogicalPos = logicalPos;
			return (fLogicalPos*fHeight)/fRange + VThumb;
		}

		double GetThumbLogicalPosition(double pixelPos)
		{
			double fHeight = ClientRectangle.Height - VThumb*2;
			double fRange = (max-min);
			double fpixelPos = pixelPos;
			return (fRange*(fpixelPos-VThumb)/fHeight);
		}

		ScrollBarHit HitTest(Point point)
		{
			Rectangle upArrow = GetArrowButtonRectangle(true);
			if ( upArrow.Contains(point) )
				return ScrollBarHit.UpArrow;
			
			Rectangle downArrow = GetArrowButtonRectangle(false);
			if ( downArrow.Contains(point) )
				return ScrollBarHit.DownArrow;

			Rectangle upPageRect = GetPageRect(true);
			if ( upPageRect.Contains(point) )
				return ScrollBarHit.PageUp;

			Rectangle downPageRect = GetPageRect(false);
			if ( downPageRect.Contains(point) )
				return ScrollBarHit.PageDown;

			Rectangle thumbRect = GetThumbRect();
			if ( thumbRect.Contains(point) )
				return ScrollBarHit.Thumb;
            
			return ScrollBarHit.None;
		}
		#endregion

	}
}
