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
	public class VSkinnedSlider : UtilityLibrary.WinControls.SkinnedSlider
	{
		#region Class Variables
		#endregion
		
		#region Constructors
		public VSkinnedSlider()
		{
		}

		public VSkinnedSlider(Bitmap backgroundImage, ImageList trackerImageList) 
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
				Value = GetValue(e.Y);
				trackerPos = e.Y;
			}
			else
			{
				// If we hit the tracker, keep track of the offset
				// from the mouse hit to the middle of the tracker position
				int trackerHeight = 0;
				if ( trackerImageList != null )
					trackerHeight = trackerImageList.ImageSize.Height;
				int middlePos = rc.Top + trackerHeight/2;
				offset = middlePos - e.Y;
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
				int trackerHeight = 0;
				if ( trackerImageList != null )
					trackerHeight = trackerImageList.ImageSize.Height;
				if ( e.Y <= rc.Top + trackerHeight/2 ) offset = 0;
				if ( e.Y >= rc.Bottom - trackerHeight/2 ) offset = 0;

				Value = GetValue(e.Y + offset);
				trackerPos = e.Y + offset;
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
			int trackerHeight = 0;
			if ( trackerImageList != null )
				trackerHeight = trackerImageList.ImageSize.Height;
			if ( e.Y <= rc.Top + trackerHeight/2 ) offset = 0;
			if ( e.Y >= rc.Bottom - trackerHeight/2 ) offset = 0;
			
			// Process left button down
			Capture = false;
			Value = GetValue(e.Y + offset);
			trackerPos = e.Y + offset;
			drawState = DrawState.Normal;
			Invalidate();
		}

		protected override void DrawTracker(Graphics g)
		{
			Rectangle rc = ClientRectangle;
					
			// If we have all the needed bitmaps
			if ( trackerImageList != null &&  trackerImageList.Images.Count > (int)drawState )
			{
				int trackerWidth = trackerImageList.ImageSize.Width;
				int trackerHeight = trackerImageList.ImageSize.Height;
				int y = trackerPos;
				if ( y + trackerHeight/2 > rc.Height - 1 )
				{
					// Divide and multiply by 2 to eliminate rounding error
					y = rc.Bottom - (trackerHeight/2*2) - 1;
				}
				else if ( y - trackerHeight/2 < 0 )
					y = 0;
				else
					y -= trackerHeight/2;
				
				Point pt = new Point(rc.Left + (rc.Width-trackerWidth)/2, y );
				trackerRect = new Rectangle(pt.X, pt.Y, trackerWidth, trackerHeight);
				g.DrawImage(trackerImageList.Images[(int)drawState], trackerRect);
			}
		}

		protected override int GetValue(int position)
		{
			int trackerHeight = 0;
			if ( trackerImageList != null )
				trackerHeight = trackerImageList.ImageSize.Height;
			if ( position > ClientRectangle.Bottom-trackerHeight/2)
				position = ClientRectangle.Bottom-trackerHeight/2;
			else if ( position < ClientRectangle.Top+trackerHeight/2 )
				position = ClientRectangle.Top+trackerHeight/2;
						
			return (position-trackerHeight/2)*(max-min)/(ClientRectangle.Height-trackerHeight);
		}

		protected override int GetPosition(int _value)
		{
			int trackerHeight = 0;
			if ( trackerImageList != null )
				trackerHeight = trackerImageList.ImageSize.Height;
			return _value*(ClientRectangle.Height-trackerHeight)/(max-min) + trackerHeight/2;
		}
		protected override void ResizeSkinnedSlider()
		{
			// Size control to be the size size as 
			// the background bitmap so that it draws as it were transparent
			Image bm = BackgroundImage;
			if ( bm != null )
			{
				Rectangle rc = Bounds;
				Bounds = new Rectangle(rc.Left, rc.Top, bm.Width, rc.Height);
			}
		}
		#endregion

		#region Implementation
        #endregion

	}
}
