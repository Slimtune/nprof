using System;
using System.Runtime.InteropServices;

using UtilityLibrary.Win32;

namespace UtilityLibrary.General
{
	/// <summary>
	/// Summary description for ImageListUtil.
	/// </summary>
	public class ImageListUtil : IDisposable
	{
		#region Class Variables
		IntPtr handle = IntPtr.Zero;
		const int IMAGELIST_GROW = 10;
		#endregion
        				
		#region Constructors
		public ImageListUtil(int width, int height, uint flags, int count)
		{
			// Initialize CommonLibrary 
			WindowsAPI.InitCommonControls();

			// Create imageList
            handle = WindowsAPI.ImageList_Create(width, height, flags, count, IMAGELIST_GROW);                        
		}
			
		public void Dispose()
		{
			// Destroy image list
			if ( handle != IntPtr.Zero )
			{
				WindowsAPI.ImageList_Destroy(handle);
			}
            
		}
		#endregion

		#region Methods
		public void AddImage(IntPtr hBitmap, IntPtr hMask)
		{
			int rc = WindowsAPI.ImageList_Add(handle, hBitmap, hMask);
		}
        
		public void RemoveImage(int index)
		{
			WindowsAPI.ImageList_Remove(handle, index);
		}

		public void BeginDrag(int imageIndex, int xHotSpot, int yHotSpot)
		{
            WindowsAPI.ImageList_BeginDrag(handle, imageIndex, xHotSpot, yHotSpot);
		}
        
		public void DragEnter(IntPtr hWndLock, int x, int y)
		{
			WindowsAPI.ImageList_DragEnter(hWndLock, x, y);
		}

		public void DragMove(int x, int y)
		{
			WindowsAPI.ImageList_DragMove(x, y);
		}

		public void DragLeave(IntPtr hWndLock)
		{
			WindowsAPI.ImageList_DragLeave(hWndLock);
		}
		
		public void EndDrag()
		{
			WindowsAPI.ImageList_EndDrag();
		}
        #endregion

	}
}
