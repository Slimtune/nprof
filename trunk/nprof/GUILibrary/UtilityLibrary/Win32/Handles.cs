using System;

namespace UtilityLibrary.Win32
{
	public class ShellHandle : IDisposable
	{
		#region Class Variables
		protected IntPtr handle = IntPtr.Zero;
		#endregion
        
		#region Constructors 
		// Can only be used as base classes
		protected ShellHandle(IntPtr handle)
		{
			this.handle = handle;
		}
        
		~ShellHandle()
		{
			Dispose(false);
		}
		#endregion

		#region Properties
		public IntPtr Handle
		{
			get { return handle; }
		}
	
		#endregion

		#region Virtuals
		protected virtual void Dispose(bool disposing)
		{
			// This class encapsulate a PIDL handle that
			// it is allocated my the Shell Memory Manager
			// it needs to be deallocated by the Shell Memory Manager 
			// interface too
			
			// To avoid threads simultaneously releasing this resource
			lock (this)
			{
				
				if ( handle != IntPtr.Zero )
				{
					// If we have a valid handle
					// Release pointer that was allocated by the COM memory allocator
					WindowsAPI.SHFreeMalloc(handle);
					handle = IntPtr.Zero;
				}
			}
		}
		#endregion

		#region Methods
		// Implements the IDisposable Interface
		public void Dispose()
		{
			// Let the Garbage Collector know that it does
			// not need to call finalize for this class
			GC.SuppressFinalize(this);

			// Do the disposing
			Dispose(true);
		}
		#endregion
	
	}

	public class COMInterface : IDisposable
	{
		#region Class Variables
		protected IUnknown iUnknown = null;
		#endregion
        
		#region Constructors 
		// Can only be used as base classes
		protected COMInterface(IUnknown iUnknown)
		{
			this.iUnknown = iUnknown;
		}
        
		~COMInterface()
		{
			Dispose(false);
		}
		#endregion

		#region Properties
		#endregion

		#region Virtuals
		protected virtual void Dispose(bool disposing)
		{
			// Release the reference to this interface
			lock(this)
			{
				if ( iUnknown != null )
				{
					iUnknown.Release();
					iUnknown = null;
				}
			}
		}
		#endregion

		#region Methods
		// Implements the IDisposable Interface
		public void Dispose()
		{
			// Let the Garbage Collector know that it does
			// not need to call finalize for this class
			GC.SuppressFinalize(this);

			// Do the disposing
			Dispose(true);
		}
		#endregion
		
	}

	public class GdiHandle : IDisposable
	{
		#region Class Variables
		protected IntPtr handle = IntPtr.Zero;
		#endregion
        
		#region Constructors 
		// Can only be used as base classes
		protected GdiHandle(IntPtr handle)
		{
			this.handle = handle;
		}
        
		~GdiHandle()
		{
			Dispose(false);
		}
		#endregion

		#region Properties
		public IntPtr Handle
		{
			get { return handle; }
		}
	
		#endregion

		#region Virtuals
		protected virtual void Dispose(bool disposing)
		{
			// To avoid threads simultaneously releasing this resource
			lock (this)
			{
				
				if ( handle != IntPtr.Zero )
				{
					// If we have a valid handle
					// Destroy the handle
					WindowsAPI.DeleteObject(handle);
					handle = IntPtr.Zero;
				}
			}
		}
		#endregion

		#region Methods
		// Implements the IDisposable Interface
		public void Dispose()
		{
			// Let the Garbage Collector know that it does
			// not need to call finalize for this class
			GC.SuppressFinalize(this);

			// Do the disposing
			Dispose(true);
		}
		#endregion
	
	}



}
