using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace UtilityLibrary.WinControls
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem(false)]
	public class HSkinnedSlider : UtilityLibrary.WinControls.SkinnedSlider
	{
		#region Class variables
		#endregion

		#region Constructors
		public HSkinnedSlider()
		{
			
		}

		public HSkinnedSlider(Bitmap backgroundImage, ImageList trackerImageList) 
			: base(backgroundImage, trackerImageList)
		{
		}
		#endregion

		#region Overrides
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			ResizeSkinnedSlider();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if ( e.Button != MouseButtons.Left) 
				return;

			// Process left button down
			Capture = true;
			// use property to set the new value
			// so that the event Value change can be fired 
			// if it needs to
			Rectangle rc = trackerRect;
			if ( !(rc.Contains(new Point(e.X, e.Y))) )
			{
				// Only update the value when the mouse hit outside
				// the tracker, otherwise we get the strange effect
				// of the tracker value jumping to a new value when
				// we just want to start dragging the tracker
				Value = GetValue(e.X);
				trackerPos = e.X;
			}
			else
			{
				// If we hit the tracker, keep track of the offset
				// from the mouse hit to the middle of the tracker position
				int trackerWidth = 0;
				if ( trackerImageList != null )
					trackerWidth = trackerImageList.ImageSize.Width;
				int middlePos = rc.Left + trackerWidth/2;
				offset = middlePos - e.X;
			}
			drawState = DrawState.Pressed;
			Invalidate();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			bool invalidate = false;
			Rectangle tracker = trackerRect;
			if ( !Capture )
			{
				if ( tracker.Contains(new Point(e.X, e.Y) ) )
				{
					drawState = DrawState.Hot;
				}
				else
				{
					drawState = DrawState.Normal;
				}
				invalidate = true;
			}
                        
			if ( e.Button != MouseButtons.Left) 
			{
				if ( invalidate )
					Invalidate();
				return;
			}
		
			// Process left button down
			if ( Capture )
			{
				Rectangle rc = ClientRectangle;
				int trackerWidth = 0;
				if ( trackerImageList != null )
					trackerWidth = trackerImageList.ImageSize.Width;
				if ( e.X <= rc.Left + trackerWidth/2 ) offset = 0;
				if ( e.X >= rc.Right - trackerWidth/2 ) offset = 0;

				Value = GetValue(e.X + offset);
				trackerPos = e.X + offset;
				invalidate = true;
			}
            
			if ( invalidate )
				Invalidate();

		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if ( e.Button != MouseButtons.Left) 
				return;

			Rectangle rc = ClientRectangle;
			int trackerWidth = 0;
			if ( trackerImageList != null )
                trackerWidth = trackerImageList.ImageSize.Width;
			if ( e.X <= rc.Left + trackerWidth/2 ) offset = 0;
			if ( e.X >= rc.Right - trackerWidth/2 ) offset = 0;
			
			// Process left button down
			Capture = false;
			Value = GetValue(e.X + offset);
			trackerPos = e.X + offset;
			drawState = DrawState.Normal;
			Invalidate();
		}

		protected override void DrawTracker(Graphics g)
		{
			Rectangle rc = ClientRectangle;
					
			// If we have the needed image
			if ( trackerImageList != null &&  trackerImageList.Images.Count > (int)drawState )
			{
				int trackerWidth = trackerImageList.ImageSize.Width;
				int trackerHeight = trackerImageList.ImageSize.Height;
				int x = trackerPos;
				if ( x + trackerWidth/2 > rc.Width - 1 )
				{
					// Divide and multiply by 2 to eliminate rounding error
					x = rc.Right - (trackerWidth/2*2) - 1;
				}
				else if ( x - trackerWidth/2 < 0 )
					x = 0;
				else
					x -= trackerWidth/2;
				
				Point pt = new Point(x, rc.Top + (rc.Height-trackerHeight)/2);
				trackerRect = new Rectangle(pt.X, pt.Y, trackerWidth, trackerHeight);
				g.DrawImage(trackerImageList.Images[(int)drawState], trackerRect);
			}
		}

		protected override int GetValue(int position)
		{
			int trackerWidth = 0;
			if ( trackerImageList != null )
				trackerWidth = trackerImageList.ImageSize.Width;
			
			if ( position > ClientRectangle.Right-trackerWidth/2)
				position = ClientRectangle.Right-trackerWidth/2;
			else if ( position < ClientRectangle.Left+trackerWidth/2 )
				position = ClientRectangle.Left+trackerWidth/2;
						
			return (position-trackerWidth/2)*(max-min)/(ClientRectangle.Width-trackerWidth);
		}

		protected override int GetPosition(int _value)
		{
			int trackerWidth = 0;
			if ( trackerImageList != null )
				trackerWidth = trackerImageList.ImageSize.Width;
			return _value*(ClientRectangle.Width-trackerWidth)/(max-min) + trackerWidth/2;
		}
		protected override void ResizeSkinnedSlider()
		{
			// Size control to be the size size as 
			// the background bitmap so that it draws as it were transparent
			Image bm = BackgroundImage;
			if ( bm != null )
			{
				Rectangle rc = Bounds;
				Bounds = new Rectangle(rc.Left, rc.Top, rc.Width, bm.Height);
			}
		}
		#endregion

		#region Implementation
		#endregion

	}
}
