using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using UtilityLibrary.Win32;
using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	#region Delegates
	public delegate void SelectedDriveChangedHandler(object sender, DriveInfo driveInfo);
	#endregion

	#region Helper Classes
	// Helps to keep information about a drive in one place
	public class DriveInfo
	{
		#region Class Variables
		
		string drivePath = string.Empty; 
		string volumeName = string.Empty;
		string fileSystemName = string.Empty;
		string displayName = string.Empty;
		string typeName = string.Empty;

		uint fileSystemFlags = 0;
		uint maxFileNameLength = 0;
		uint serialNumber = 0;
		int iconIndex = -1;

		#endregion
		
		#region Constructors
		public DriveInfo(string drivePath, string volumeName)
		{
			this.drivePath = drivePath;
			this.volumeName = volumeName;
		}
		#endregion
	
		#region Properties
		public string DrivePath
		{
			get { return drivePath; }
		}
		
		public string VolumeName
		{
			get { return volumeName; }
		}

		public string DisplayName
		{
			set { displayName = value; }
			get { return displayName; }
		}

		public string TypeName
		{
			set { typeName = value; }
			get { return typeName; }
		}

		public string FileSystemName
		{
			set { fileSystemName = value; }
			get { return fileSystemName; }
		}
	
		public int IconIndex
		{
			set { iconIndex = value; }
			get { return iconIndex; }
		}

		public uint FileSystemFlags
		{
			set { fileSystemFlags = value; }
			get { return fileSystemFlags; }
		}

		public uint MaxFileNameLength
		{
			set { maxFileNameLength = value; }
			get { return maxFileNameLength; }
		}

		public uint SerialNumber
		{
			set { serialNumber = value; }
			get { return serialNumber; }
		}

		#endregion

	}
	#endregion


	/// <summary>
	/// Summary description for DriveComboBox.
	/// </summary>
	[ToolboxItem(false)]
	public class DriveComboBox : ComboBoxBase
	{
		#region Events
		public event SelectedDriveChangedHandler SelectedDriveChanged;
		#endregion
				
		#region Class Variables
		// Keeps track of all drives
		ArrayList driveList = new ArrayList();
		const int MY_COMPUTER_LEFT_PADDING = 2;
		const int DRIVES_LEFT_PADDING = 20;
		// Assume a small icon size of 16
		const int ICON_SIZE = 16;
		private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
		IntPtr hImageList = IntPtr.Zero;
		#endregion

		#region Constructors
		public DriveComboBox()
		{
			DrawMode = DrawMode.OwnerDrawVariable;
			DropDownStyle = ComboBoxStyle.DropDownList;
			
			// Override parent, we don't want to do all the painting ourselves
			// since we want to let the edit control deal with the text for editing
			// the parent class ComboBoxBase knows to do the right stuff with 
			// non-editable comboboxes as well as editable comboboxes as long
			// as we change these flags below
			SetStyle(ControlStyles.AllPaintingInWmPaint
				|ControlStyles.UserPaint|ControlStyles.Opaque, false);

			Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, 
				System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

			SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
			
			InitializeDriveComboBox();
		}

		void InitializeDriveComboBox()
		{
			GetLogicalDrives();
			int count = driveList.Count;
			for ( int i = 0; i < count; i++ )
			{
				DriveInfo di = (DriveInfo)driveList[i];
				Debug.Assert(di != null);
				Items.Add(di.DisplayName);
			}
		}
		#endregion

		#region Overrides
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
            e.ItemHeight = ICON_SIZE + 2;
			base.OnMeasureItem(e);
		}

		protected override void DrawComboBoxItem(Graphics g, Rectangle bounds, int Index, bool selected, bool editSel)
		{
			// Call base class to do the "Flat ComboBox" drawing
			base.DrawComboBoxItem(g, bounds, Index, selected, editSel);
			if ( Index != -1) 
			{
				DriveInfo driveInfo = (DriveInfo)driveList[Index];
				Debug.Assert(driveInfo != null);
				
				Brush brush;
				brush = new SolidBrush(SystemColors.MenuText);
				int gap = 0;
				if ( Index == 0 )
					// This is "My Computer" item
					gap = MY_COMPUTER_LEFT_PADDING;
				else
					gap = DRIVES_LEFT_PADDING;
				if (editSel )
					gap-=3;
				
				// Draw image
				int iconIndex = driveInfo.IconIndex;
				if ( iconIndex != -1 )	
				{
					// Use the System Image icon itself to do the drawing to avoid
					// those awful black lines on the icons
					IntPtr hDC = g.GetHdc();
					WindowsAPI.ImageList_DrawEx(hImageList, iconIndex, hDC, 
						bounds.Left+gap, bounds.Top+(bounds.Height-ICON_SIZE)/2, 0, 0,
						ImageListDrawColors.CLR_NONE, ImageListDrawColors.CLR_NONE, ImageListDrawFlags.ILD_NORMAL);
					
					// --it could not be the same size that the icons we are getting from SHGetFileInfo-- 
					gap += ICON_SIZE + 2;
					g.ReleaseHdc(hDC);
				}
				
				Size textSize = TextUtil.GetTextSize(g, driveInfo.DisplayName, Font);
				int top = bounds.Top + (bounds.Height - textSize.Height)/2;
				g.DrawString(driveInfo.DisplayName, Font, brush, new Point(bounds.Left + gap, top));
				brush.Dispose();
			}
		}

		protected override void DrawComboBoxItemEx(Graphics g, Rectangle bounds, int Index, bool selected, bool editSel)
		{
			// This "hack" is necessary to avoid a clipping bug that comes from the fact that sometimes
			// we are drawing using the Graphics object for the edit control in the combobox and sometimes
			// we are using the graphics object for the combobox itself. If we use the same function to do our custom
			// drawing it is hard to adjust for the clipping because of these limitations
			base.DrawComboBoxItemEx(g, bounds, Index, selected, editSel);
			if ( Index != -1)
			{
				DriveInfo driveInfo = (DriveInfo)driveList[Index];
				Debug.Assert(driveInfo != null);
				
				SolidBrush brush;
				brush = new SolidBrush(SystemColors.MenuText);
                		
				Rectangle rc = bounds;
				int gap = 0;
				if ( Index == 0 )
					// This is "My Computer" item
					gap = MY_COMPUTER_LEFT_PADDING;
				else
					gap = DRIVES_LEFT_PADDING;
								
				// Draw image
				int iconIndex = driveInfo.IconIndex;
				if ( iconIndex != -1 )	
				{
					// Use the System Image icon itself to do the drawing to avoid
					// those awful black lines on the icons
					IntPtr hDC = g.GetHdc();
					WindowsAPI.ImageList_DrawEx(hImageList, iconIndex, hDC, 
						bounds.Left+gap, bounds.Top+(bounds.Height-ICON_SIZE)/2, 0, 0,
						ImageListDrawColors.CLR_NONE, ImageListDrawColors.CLR_NONE, ImageListDrawFlags.ILD_NORMAL);
					
					// --it could not be the same size that the icons we are getting from SHGetFileInfo-- 
					gap += ICON_SIZE + 2;
					g.ReleaseHdc(hDC);
				}
				
				Size textSize = TextUtil.GetTextSize(g, driveInfo.DisplayName, Font);
				int top = bounds.Top + (bounds.Height - textSize.Height)/2;

				Rectangle clipRect = new Rectangle(bounds.Left + gap, top, bounds.Width - ARROW_WIDTH, top + textSize.Height);
				g.DrawString(driveInfo.DisplayName, Font, brush, clipRect);
				brush.Dispose();
			}
		}

		protected override void DrawDisableState()
		{
			// Draw the combobox state disable
			base.DrawDisableState();
			
			// Draw the specific disable state to
			// this derive class
			using ( Graphics g = CreateGraphics() )
			{
				Rectangle rc = ClientRectangle;
                int selIndex = SelectedIndex;
				int gap = 0;
				if ( selIndex == 0 )
					gap = MY_COMPUTER_LEFT_PADDING;
				else
					gap = DRIVES_LEFT_PADDING;

				DriveInfo driveInfo = (DriveInfo)driveList[selIndex];
				int iconIndex = driveInfo.IconIndex;
				using ( Brush b = new SolidBrush(SystemColors.ControlDark) )
				{
					if ( iconIndex != -1 )	
					{
						// Use the System Image icon itself to do the drawing to avoid
						// those awful black lines on the icons
						IntPtr hDC = g.GetHdc();
						WindowsAPI.ImageList_DrawEx(hImageList, iconIndex, hDC, 
							rc.Left+gap, rc.Top+(rc.Height-ICON_SIZE)/2, 0, 0,
							ImageListDrawColors.CLR_NONE, ImageListDrawColors.CLR_NONE, ImageListDrawFlags.ILD_BLEND50);
					
						// --it could not be the same size that the icons we are getting from SHGetFileInfo-- 
						gap += ICON_SIZE + 2;
						g.ReleaseHdc(hDC);
					}

					Size textSize = TextUtil.GetTextSize(g, driveInfo.DisplayName, Font);
					
					// Clipping rectangle
					int top = rc.Top + (rc.Height - textSize.Height)/2;
					Rectangle clipRect = new Rectangle(rc.Left + gap, 
						top, rc.Width  -  ARROW_WIDTH, top+textSize.Height);
					g.DrawString(driveInfo.DisplayName, Font, b, clipRect);
				}
			}
		}
		#endregion

		#region Implementation
		void GetLogicalDrives()
		{
			// Get all drives
			string[] logicalDrives = Directory.GetLogicalDrives();

			// Add the Computer item as the root of all drives
			// not really necesarry but it looks kind of cool
			DriveInfo driveInfo = new DriveInfo(string.Empty ,"My Computer");
			
			string dName;
			string tName;
			int ii;
			
			IntPtr idlPtr;
			int result = WindowsAPI.SHGetSpecialFolderLocation(IntPtr.Zero, ShellSpecialFolder.CSIDL_DRIVES, out idlPtr);
			// Get Folder information
			GetSpecialFolderShellInfo(idlPtr, out dName, out tName, out ii);
			
            // Release pointer that was allocated by the COM memory allocator
			IMalloc alloc = null;
			WindowsAPI.SHGetMalloc(out alloc);
			Debug.Assert(alloc != null);
            alloc.Free(idlPtr);
			// Free pointer memory allocator
			IUnknown iUnknown = (IUnknown)alloc;
			iUnknown.Release();
											
			driveInfo.IconIndex = ii;
			driveInfo.DisplayName = dName;
			driveInfo.FileSystemName = tName;
            driveList.Add(driveInfo);
						
			// Fill drives array with available drives
			foreach (string drivePath in logicalDrives )
			{
				uint serialNumber;
				uint maxFileNameLength;
				uint fileSystemFlags;
				string volumeName;
				string fileSystemName;

				GetDriveVolumeInfo(drivePath, out volumeName, out fileSystemName, 
					out serialNumber, out maxFileNameLength, out fileSystemFlags);
				
				// Construct DriveInfo object and add it to our list
				driveInfo = new DriveInfo(drivePath, volumeName);
				driveList.Add(driveInfo);

				// Set the rest of the properties
                driveInfo.SerialNumber = serialNumber;
				driveInfo.MaxFileNameLength = maxFileNameLength;
				driveInfo.FileSystemFlags = fileSystemFlags;
				
				// Add other information
				string displayName;
				string typeName;
				int iconIndex;
				
				GetDriveShellInfo(drivePath, out displayName, out typeName, out iconIndex);
				// Save it into the object
				driveInfo.DisplayName = displayName;
				driveInfo.TypeName = typeName;
				driveInfo.IconIndex = iconIndex;
			}
		}

		void GetDriveVolumeInfo(string drivePath, out string volumeName, out string fileSystemName, 
			out uint serialNumber, out uint maxFileNameLength, out uint fileSystemFlags)
		{
			// Make the buffer big enough
			const int BUFFER_SIZE = 1024;

			// Buffers
			StringBuilder volumeNameBuffer = new StringBuilder(BUFFER_SIZE);
			StringBuilder fileSystemNameBuffer = new StringBuilder(BUFFER_SIZE);
			
			// Get the Information
			int retval = WindowsAPI.GetVolumeInformation(drivePath, volumeNameBuffer, BUFFER_SIZE, out serialNumber,  out maxFileNameLength, 
				out fileSystemFlags, fileSystemNameBuffer, BUFFER_SIZE);

			volumeName = volumeNameBuffer.ToString();
			fileSystemName = fileSystemNameBuffer.ToString();
                    
		}
		
		void GetDriveShellInfo(string drivePath, out string displayName, out string typeName, out int iconIndex)
		{
			SHFILEINFO shfi = new SHFILEINFO();
                        			
			// Load a System Small icon image
			hImageList = WindowsAPI.SHGetFileInfo( drivePath, 0, out shfi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), 
				ShellFileInfoFlags.SHGFI_SYSICONINDEX | ShellFileInfoFlags.SHGFI_SMALLICON |
				ShellFileInfoFlags.SHGFI_DISPLAYNAME | ShellFileInfoFlags.SHGFI_TYPENAME );
			
			iconIndex = shfi.iIcon;
			displayName = shfi.szDisplayName;
			typeName = shfi.szTypeName;
      	}
		void GetSpecialFolderShellInfo(IntPtr idl, out string displayName, out string typeName, out int iconIndex)
		{
			SHFILEINFO shfi = new SHFILEINFO();
                        			
			// Load a System Small icon image
			hImageList = WindowsAPI.SHGetFileInfo(idl, 0, out shfi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), 
				ShellFileInfoFlags.SHGFI_SYSICONINDEX | ShellFileInfoFlags.SHGFI_SMALLICON |
				ShellFileInfoFlags.SHGFI_DISPLAYNAME | ShellFileInfoFlags.SHGFI_TYPENAME | ShellFileInfoFlags.SHGFI_PIDL );
			
			iconIndex = shfi.iIcon;
			displayName = shfi.szDisplayName;
			typeName = shfi.szTypeName;
		}
		
		void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			FireSelectedDriveChanged();
		}

		private void InitializeComponent()
		{
			this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			// 
			// crystalReportViewer1
			// 
			this.crystalReportViewer1.ActiveViewIndex = -1;
			this.crystalReportViewer1.Location = new System.Drawing.Point(17, 17);
			this.crystalReportViewer1.Name = "crystalReportViewer1";
			this.crystalReportViewer1.ReportSource = null;
			this.crystalReportViewer1.TabIndex = 0;

		}
		
		void FireSelectedDriveChanged()
		{
			if ( SelectedDriveChanged != null )
			{
				int index = SelectedIndex;
				if ( index != -1 )
				{
					DriveInfo di = (DriveInfo)driveList[index];
					Debug.Assert(di != null);
					SelectedDriveChanged(this, di);
				}
			}
		}

		#endregion
		
	}
}
