using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

using UtilityLibrary.Win32;
using UtilityLibrary.General;

namespace UtilityLibrary.WinControls
{
	#region Delegates
	public delegate void ExplorerNodeChangedHandler(ExplorerTree et, TreeNodeData tnd);
	#endregion

	#region Enumerations
	[Flags]
	public enum TreeNodeFlags
	{
		ShowFiles        = 0x00001,
        ShowHiddenFiles  = 0x00003,
		ShowName         = 0x00004,
		ShowPath         = 0x00008
	}
	#endregion

	#region Helper Classes
	public class ShellIDList : ShellHandle
	{
		#region Class Variables
		IntPtr hWnd = IntPtr.Zero;
		#endregion

		#region Constructors
		public ShellIDList(IntPtr idList, IntPtr hWnd): base(idList) 
		{
			Debug.Assert(handle != IntPtr.Zero);
			this.hWnd = hWnd;
		}

		public ShellIDList(IntPtr idList) : base(idList)
		{
			Debug.Assert(idList != IntPtr.Zero);
		}
      	#endregion

		#region Overrides
		#endregion

		#region Operators
		public static implicit operator IntPtr(ShellIDList shellIDList)
		{
			return shellIDList.Handle;
		}
		#endregion

		#region Properties
		public int Length
		{
			get 
			{
				// We have a valid handle find its length
				int count = 0;
				int length = 0;
			
				// Iterate through the list to find every item length
				// and add it to the count
				int pBase = (int)handle;
				do 
				{
					// Make sure we are not trying to ready zero out memory
					if ( !IsValidPointer((IntPtr)pBase))
						break;
					
					ITEMIDLIST idl = (ITEMIDLIST)Marshal.PtrToStructure((IntPtr)pBase, 
						typeof(ITEMIDLIST));
					count = idl.mkid.cb;
					pBase = pBase + count; 
					length += count;
				}
				while ( count != 0 );
			
				return length;
			}
		}

		#endregion 

		#region Methods
		public string GetPath()
		{
			StringBuilder path = new StringBuilder(1024);
			int result = WindowsAPI.SHGetPathFromIDList(handle, path);
			if ( result == 0 )
				return string.Empty;
			return path.ToString();		
		}

		public IntPtr GetFirstChild()
		{
			return GetNextChild(this.Handle);
		}

		public IntPtr GetLastChild()
		{
			return GetLastChild(this.Handle);
		}

		public int GetIconIndex(ShellFileInfoFlags flags)
		{
			SHFILEINFO shfi = new SHFILEINFO();
                        			
			// Get small icon index
			ShellFileInfoFlags fileFlags = ShellFileInfoFlags.SHGFI_SYSICONINDEX | ShellFileInfoFlags.SHGFI_PIDL;
			fileFlags |= flags;
			WindowsAPI.SHGetFileInfo(handle, 0, out shfi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), fileFlags);
			return shfi.iIcon;
		}
		
		public int GetCount()
		{
			// Make sure we are not trying to ready zero out memory
			int pBase = (int)handle;
			if ( !IsValidPointer((IntPtr)pBase))
				return 0;

			// Bring the pointer into a managed type 
			ITEMIDLIST idList = (ITEMIDLIST)Marshal.PtrToStructure(handle, typeof(ITEMIDLIST));
			return idList.mkid.cb;
		}
		static public IntPtr GetNextChild(IntPtr parent)
		{
			// Make sure we are not trying to ready zero out memory
			if ( !IsValidPointer((IntPtr)parent))
				return IntPtr.Zero;

			// Bring the pointer into a managed type 
			ITEMIDLIST idList = (ITEMIDLIST)Marshal.PtrToStructure(parent, typeof(ITEMIDLIST));
			// Get the size of the idList
			int cb = idList.mkid.cb;
	
			// If the size is zero, it is the end of the list. 
			if ( cb == 0) 
				return IntPtr.Zero; 
	
			// Add cb to original list to obtain the next child list 
			IntPtr pBase = parent;
			IntPtr nextList = (IntPtr)((int)pBase + cb); 
			
			// Make sure we are not trying to ready zero out memory
			if ( IsValidPointer(nextList) )
			{
				// Bring the pointer into a managed type 
				idList = (ITEMIDLIST)Marshal.PtrToStructure(nextList, typeof(ITEMIDLIST));
				return nextList;
			}
			else
			{
				// Return IntPtr.Zero if we reached the terminator, or a pidl otherwise. 
				return IntPtr.Zero;
			}
		}

		static public IntPtr GetLastChild(IntPtr parent)
		{
			IntPtr current = GetNextChild(parent);
			IntPtr last = IntPtr.Zero;
			while ( current != IntPtr.Zero)
			{
				last = current;
				current = GetNextChild(current);
			}

			return last;
		}
		static public ShellIDList Combine(IMalloc iMalloc, ShellIDList idList1, ShellIDList idList2)
		{
			// Combine PUIDL to form a new one
			IntPtr list1 = IntPtr.Zero;
			if ( idList1 != null )
				list1 = idList1.Handle;
			IntPtr list2 = IntPtr.Zero;
			if ( idList2 != null )
				list2 = idList2.Handle;

			// Get PUIDL lengths
			int length1 = 0;
			if ( idList1 != null )
			{
				length1 = idList1.Length;
			}
			int length2 = 0;
			if ( idList2 != null )
			{
				length2 = idList2.Length;
			}

			// Allocate memory for the combined handle
			IntPtr newHandle = IntPtr.Zero;
			newHandle = (IntPtr)iMalloc.Alloc((uint)(length1 + length2 + Marshal.SizeOf(typeof(ushort))));
			IntPtr pBase = newHandle;
			
			// If memory was successfully allocated
			if ( newHandle != IntPtr.Zero )
			{
				if ( length1 > 0 )
				{
					for ( int i = 0; i < length1; i++ )
					{
						byte currentByte = Marshal.ReadByte(list1, i);
						Marshal.WriteByte(pBase, currentByte);
						pBase = (IntPtr)((int)pBase + 1);
					}
				}

				if ( length2 > 0 )
				{
					for ( int i = 0; i < length2; i++ )
					{
						byte currentByte = Marshal.ReadByte(list2, i);
						Marshal.WriteByte(pBase, currentByte);
						pBase = (IntPtr)((int)pBase + 1);
					}
				}
                
				// Append terminating zero
				Marshal.WriteInt16(pBase, 0);
				return new ShellIDList(newHandle);
			}
		
			return null;
		}

		public bool IsRootList()
		{
			return Length == 0;
		}

		static bool IsValidPointer(IntPtr pBase)
		{
			// Make sure we are not trying to read into null memory
			if ( pBase == IntPtr.Zero )
				return false;

			short readValue = Marshal.ReadInt16(pBase);
			return readValue != 0;
		}
		#endregion

		#region Implementation
		#endregion
		
    }

	public class ShellIFolder : COMInterface
	{
		#region Class Variables
		IShellFolder iShellFolder = null;
	    #endregion

		#region Constructors
		public ShellIFolder(IShellFolder iShellFolder): base((IUnknown)iShellFolder) 
		{
            this.iShellFolder = iShellFolder;
		}

		public ShellIFolder(ShellIFolder shellIFolder) : base((IUnknown)shellIFolder.Interface)
		{
			IShellFolder iShellFolder = shellIFolder.Interface;
			this.iShellFolder = iShellFolder;
		}

		public static ShellIFolder CreateShellIFolder(ShellIFolder shellIFolder, ShellIDList idList)
		{
			IShellFolder iShellFolder = shellIFolder.BindToObject(idList);
			Debug.Assert(iShellFolder != null);
			return new ShellIFolder(iShellFolder);
		}

		~ShellIFolder()
		{
			Dispose(false);
		}
		#endregion

		#region Properties
		public IShellFolder Interface
		{
			get { return iShellFolder; }
		}
		#endregion

		#region Methods
		public string GetDisplayNameOf(IntPtr idList, ShellGetDisplayNameOfFlags flags)
		{
			// Use the native handle this class is wrapping to obtain the Display Name of the node
			STRRET strRet = new STRRET();
            iShellFolder.GetDisplayNameOf(idList, flags, out strRet);
			if ( strRet.uType == STRRETFlags.STRRET_WSTR )
			{
				// Get the OLE string into a managed string
                IntPtr pOLEString = (IntPtr)strRet.pOleStr;
                string displayName = Marshal.PtrToStringAuto(pOLEString);
				
				// Release memory
				WindowsAPI.SHFreeMalloc(pOLEString);
                
				return displayName;
			}
                        			
			return string.Empty;
		}
		
		public void GetAttributesOf(uint count, ref IntPtr idList, out GetAttributeOfFlags attributes)
		{
			iShellFolder.GetAttributesOf(count, ref idList, out attributes);
		}
		
		public IShellFolder BindToObject(IntPtr idList)
		{
			IShellFolder iFolder = null;
			REFIID refiid =  new REFIID("000214E6-0000-0000-c000-000000000046");
			int result = iShellFolder.BindToObject(idList, IntPtr.Zero, ref refiid, ref iFolder);
			Debug.Assert(iFolder !=null);
			return iFolder;
		}
		
		public IEnumIDList EnumObjects(IntPtr hWnd, ShellEnumFlags flags)
		{
			IEnumIDList iEnumIDList = null;
			iShellFolder.EnumObjects(hWnd, flags, ref iEnumIDList);
			// iEnumIDList could be null if we are enumerating a removable media drive
			return iEnumIDList;
		}

		public int CompareIDs(int lparam, ShellIDList shellIDList1, ShellIDList shellIDList2)
		{
            IntPtr list1 = shellIDList1.GetLastChild();
			IntPtr list2 = shellIDList2.GetLastChild();
            int result = iShellFolder.CompareIDs(lparam, list1, list2);
            
			if ( WindowsAPI.FAILED(result) )
				return 0;

			short ret = WindowsAPI.HRESULT_CODE(result);
			if ( ret < 0 ) return -1;
			else if ( ret > 0) return 1;
            
			return 0;            
		}

		public int GetUIObjectOf(IntPtr hWnd, uint count, ref IntPtr idList, REFIID riid, ref IUnknown iUnknown)
		{
			uint arrayInOut = 0;
			return iShellFolder.GetUIObjectOf(hWnd, count, ref idList, ref riid,  out arrayInOut, ref iUnknown);
		}

		#endregion

		#region Implementation
		#endregion

	}

	public class ShellIEnumIDList : COMInterface
	{
		#region Class Variables
		IEnumIDList iEnumIDList = null;
		#endregion

		#region Constructors

		public ShellIEnumIDList(IEnumIDList iEnumIDList) : base((IUnknown)iEnumIDList)
		{
			this.iEnumIDList = iEnumIDList;
		}

		public static ShellIEnumIDList CreateShellIEnumIDList(IntPtr hWnd, ShellIFolder shellIFolder, ShellEnumFlags flags)
		{
			// Interface could be null if we are trying to
			// enumerate a removable media
			IEnumIDList iEnumIDList = shellIFolder.EnumObjects(hWnd, flags);
			// Don't let the object to succed if we could get the interface
			if ( iEnumIDList == null )
				throw new Exception("Failed to obtain IEnumIDList interface.");
			return new ShellIEnumIDList(iEnumIDList);
		}
		#endregion

		#region Properties
		public IEnumIDList Interface
		{
			get { return iEnumIDList; }
		}
		#endregion

		#region Methods
		public int Next(uint count, ref IntPtr idListPtr, out uint fetched)
		{
			return iEnumIDList.Next(count, ref idListPtr, out fetched);

		}
		#endregion

	}

	[ToolboxItem(false)]
	public class ShellPopupMenu : ContextMenu 
	{
		#region Class Variables
		IContextMenu iContextMenu = null;
		const int FIRST_COMMAND = 20000;
		const int LAST_COMMAND = FIRST_COMMAND + 1000;
		int lastCommand = 0;
		IntPtr hOwner = IntPtr.Zero;
		IntPtr hOldWinProc = IntPtr.Zero;
		#endregion

		#region Constructors
		public ShellPopupMenu()
		{

		}
		public void Initialize(ShellIFolder folder, ShellIDList idList)
		{
			// Dispose of any resources we have before
			Dispose();

			// Clear any previous menu items
			MenuItems.Clear();
			
			IntPtr childIDList = idList.GetLastChild();
			// If there is not child, use the parent
			if ( childIDList == IntPtr.Zero )
			{
				childIDList = idList.Handle;
				Debug.Assert(childIDList!=IntPtr.Zero);
			}
			REFIID riid = new REFIID("000214e4-0000-0000-c000-000000000046");
			IUnknown iUnknown = null;
			folder.GetUIObjectOf(IntPtr.Zero, 1, ref childIDList, riid, ref iUnknown);
			Debug.Assert(iUnknown!=null);
			iContextMenu = (IContextMenu)iUnknown;
            
			// Get the menu items of the context menu
			QueryContextMenu();
       }

		~ShellPopupMenu()
		{
			Dispose(false);;
		}

		#endregion

		#region Overrides
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			// Relese previous reference to interface
			Release();
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		public void InvokeCommand(int commandID)
		{
			
			if ( iContextMenu != null )
			{
				CMINVOKECOMMANDINFO ici = new CMINVOKECOMMANDINFO();
				ici.cbSize = (uint)Marshal.SizeOf(typeof(CMINVOKECOMMANDINFO));
				ici.hwnd = hOwner;
				ici.nShow = (int)ShowWindowStyles.SW_SHOWNORMAL;
				ici.lpVerb = (IntPtr)WindowsAPI.MAKEINTRESOURCE(commandID - FIRST_COMMAND);
				WinErrors error = (WinErrors)iContextMenu.InvokeCommand(ref ici);
			}
		}

		public new void Dispose()
		{
			// Let the Garbage Collector know that it does
			// not need to call finalize for this class
			GC.SuppressFinalize(this);

			// Do the disposing
			Dispose(true);
		}
		#endregion

		#region Implementation
		void QueryContextMenu()
		{
			Debug.Assert(iContextMenu!=null);
			QueryContextMenuFlags flags = QueryContextMenuFlags.CMF_NODEFAULT| QueryContextMenuFlags.CMF_EXPLORE;
			int hr = iContextMenu.QueryContextMenu(Handle, 0, FIRST_COMMAND, LAST_COMMAND, flags);
			Debug.Assert(WindowsAPI.SUCCEEDED(hr));

			// Get last command index that the shell assigned us
			lastCommand = FIRST_COMMAND + WindowsAPI.HRESULT_CODE(hr) -1;

		}
		
		void Release()
		{
			// Release interface
			if ( iContextMenu!=null )
			{
				IUnknown iUnknown = (IUnknown)iContextMenu;
				Debug.Assert(iUnknown!=null);
				iUnknown.Release();
				iContextMenu = null;
			}
		}
		
		#endregion
        
	}
	
	public class TreeNodeData
	{
		#region Class Variables
		ShellIDList idList = null;
		ShellIFolder folder = null;
		TreeNodeFlags flags = TreeNodeFlags.ShowName;
		IntPtr handle = IntPtr.Zero;
		bool expanded = false;
		#endregion

		#region Constructors
		#endregion
                
		#region Properties
		public ShellIDList IDList
		{
			set { idList = value; }
			get { return idList; }
		}

		public ShellIFolder Folder
		{
			set { folder = value; }
			get { return folder; }
		}

		public TreeNodeFlags Flags
		{
			set { flags = value; }
			get { return flags; }
		}

		public IntPtr Handle
		{
			set { handle = value; }
			get { return handle; }
		}

		#endregion

		#region Methods
		public bool IsValid()
		{
			return idList != null && folder != null;
		}
		#endregion

		#region Implementation
		
		// Properties
		internal bool Expanded 
		{
			set { expanded = value; }
			get { return expanded; }
		}
		

		// Functions
        
		#endregion

	}
    #endregion


	/// <summary>
	/// Explorer like implementation of a Tree Control that show the resources in the Computer
	/// </summary>
	[ToolboxItem(false)]
	public class ExplorerTree : UtilityLibrary.WinControls.TreeViewEx
	{
		#region Class Variables
		
		// Property backers
		IntPtr hSystemImageList = IntPtr.Zero;
		bool optimizeMemoryUsage = false;
		TreeNodeFlags treeNodeFlags = TreeNodeFlags.ShowName;

		// Keep an instance of the shell memory allocator since we are going
		// to use it a lot
		IMalloc iMalloc = null;

		// Other helpers
		ShellIFolder desktopFolder = null;
		Hashtable hashTable = new Hashtable();
		TreeNode rootTreeNode = null;
		ShellPopupMenu contextMenu = new ShellPopupMenu();
		
		#endregion

		#region Constructors
		public ExplorerTree()
		{
			Initialize();
		}

		public ExplorerTree(bool optimizeMemoryUsage)
		{
			Initialize();
			this.optimizeMemoryUsage = optimizeMemoryUsage;
		}

		void Initialize()
		{
			// Obtain IShellFolder interface for shell root folder
			IShellFolder folder;
			WindowsAPI.SHGetDesktopFolder(out folder);
			Debug.Assert(folder != null);
			desktopFolder = new ShellIFolder(folder);
			// Get a pointer to the Shell Memory allocator
			WindowsAPI.SHGetMalloc(out iMalloc);
			Debug.Assert(iMalloc!=null);
		}

		void InitializeImageList()
		{
			// Get System Image List
			SHFILEINFO shfi = new SHFILEINFO();
			
			// Initialie idList by obtaining it from the shell 
            IntPtr idHandle = IntPtr.Zero;
			WindowsAPI.SHGetSpecialFolderLocation(Handle, ShellSpecialFolder.CSIDL_DESKTOP, out idHandle);
			
			ShellIDList idList = new ShellIDList(idHandle, Handle);
			hSystemImageList = WindowsAPI.SHGetFileInfo(idList, 0, out shfi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), 
			ShellFileInfoFlags.SHGFI_SYSICONINDEX | ShellFileInfoFlags.SHGFI_SMALLICON );
			Debug.Assert(hSystemImageList != IntPtr.Zero);
			
			// Don't let the Garbage collector to dispose of these resource
			// when calling the Finalize methods. The garbage collector runs on a different
			// thread and calling the SHGetMalloc does seem to like this
			idList.Dispose();
			
		}

		~ExplorerTree()
		{
			// Make sure we relese the shell memory allocator
			if ( iMalloc!=null )
			{
				IUnknown iUnknown = (IUnknown)iMalloc;
				iUnknown.Release();
			}

		}
		#endregion

		#region Overrides
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			// Tree control has been created
			InitializeImageList();

			// Associate the System image list with the tree control
			WindowsAPI.SendMessage(Handle, (int)TreeViewMessages.TVM_SETIMAGELIST, 
				(int)TreeViewImageListFlags.TVSIL_NORMAL, (int)hSystemImageList);

		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			// Check for right click
			if (e.Button == MouseButtons.Right)
			{
				OnContextMenu(e);
			}
		}
						
		protected override void OnTreeViewItemExpanding(ref Message m)
		{
			Cursor = Cursors.WaitCursor;
			NM_TREEVIEW nmtv = (NM_TREEVIEW) m.GetLParam(typeof(NM_TREEVIEW));
			IntPtr currentHandle = nmtv.itemNew.hItem;

			// Get corresponding TreeNode object
			TreeNode tn = (TreeNode)hashTable[currentHandle];

			// Process any valid node except the root node
			if ( tn != null && tn.Parent != null )
			{
				TreeNodeData tnd = (TreeNodeData)tn.Tag;
				if ( optimizeMemoryUsage || tnd.Expanded == false || IsRemovableMedia(tnd.IDList) )
				{
					Debug.Assert(tnd!=null);
					if ( nmtv.action == (int)TreeViewItemExpansion.TVE_EXPAND )
					{
						EnumerateChildrenNodes(tn, tnd.IDList, tnd.Flags);
						tnd.Expanded = true;
					}
				}
			}
			
			// So that expansion takes place
			m.Result = IntPtr.Zero;
			Cursor = Cursors.Default;
		}

		protected override void OnTreeViewItemExpanded(ref Message m)
		{
			Cursor = Cursors.WaitCursor;
			NM_TREEVIEW nmtv = (NM_TREEVIEW) m.GetLParam(typeof(NM_TREEVIEW));
			IntPtr currentHandle = nmtv.itemNew.hItem;

			// Get corresponding TreeNode object
			TreeNode tn = (TreeNode)hashTable[currentHandle];
			TreeNodeData tnd = null;
			if ( tn != null )
			{
				tnd = (TreeNodeData)tn.Tag;
				Debug.Assert(tnd!=null);
			}

			// Process any valid node except the root node
			if ( (tn != null && tn.Parent != null && optimizeMemoryUsage) || IsRemovableMedia(tnd.IDList) )
			{
				if ( nmtv.action == (int)TreeViewItemExpansion.TVE_COLLAPSE )
				{
					DeleteChildren(tn);
				}
			}
			
			// So that expansion takes place
			m.Result = IntPtr.Zero;
			Cursor = Cursors.Default;
			            			
		}
		protected override void OnTreeViewSelectionChanging(ref Message m)
		{
			// Fire event that contains the TreeNodeData class
			NM_TREEVIEW nmtv = (NM_TREEVIEW) m.GetLParam(typeof(NM_TREEVIEW));
			IntPtr currentHandle = nmtv.itemNew.hItem;

			// Get associated TreeNodeData object
			TreeNode treeNode = (TreeNode)hashTable[currentHandle];
			// We should always get a valid object
			TreeNodeData tnd = (TreeNodeData)treeNode.Tag;
			Debug.Assert(tnd!=null);

			// Fire event containing a copy of the object
			FireExplorerNodeChanged(tnd);
      
		}

		protected override  void WndProc(ref Message message)
		{
			switch (message.Msg)
			{
				case (int)Msg.WM_COMMAND:
					InvokeCommand(message.WParam.ToInt32());
					break;
				case (int)Msg.WM_DESTROY:
					OnDestroyTree();
					break;
				default:
					break;
			}

			base.WndProc(ref message);
		}
		#endregion

		#region Properties
		public bool OptimizeMemoryUsage
		{
			get { return optimizeMemoryUsage; }
		}

		public IntPtr SystemImageList
		{
			get { return hSystemImageList; }
		}

		public TreeNodeFlags TreeNodeFlags
		{
			set { treeNodeFlags = value; }
			get { return treeNodeFlags; }
		}
		#endregion

		#region Methods
		public void InsertRootFolder(bool expand)
		{
			// If there is already a tree node delete it
			RemoveRootFolder();

			// Get Desktop IDList
			IntPtr idHandle = IntPtr.Zero;
			WindowsAPI.SHGetSpecialFolderLocation(Handle, ShellSpecialFolder.CSIDL_DESKTOP, out idHandle);
			ShellIDList idList = new ShellIDList(idHandle, Handle);
			
			// Insert Root Node
			TreeNode tn = InsertNode(null, desktopFolder, null, idList, treeNodeFlags);
		            
			// Enumerate Root Node children
			if ( expand )
			{
				// Insert children
				EnumerateChildrenNodes(tn, idList, treeNodeFlags);
				// Expand the tree
				tn.Expand();
			}

			// Release in this thread not the Garbage collector thread
			idList.Dispose();

			// Save the root node for later use
			rootTreeNode = tn;

		}
		public void RemoveRootFolder()
		{
			// Remove the root
			if ( rootTreeNode != null )
			{
				DeleteNode(rootTreeNode);
				rootTreeNode = null;
			}
		}
		#endregion

		#region Events
		public event ExplorerNodeChangedHandler ExplorerNodeChanged;
		#endregion

		#region Implementation
		TreeNode InsertNode( TreeNode parentNode, ShellIFolder parentFolder, 
			ShellIDList parentList, ShellIDList idList, TreeNodeFlags flags)
		{
			// Tree Node item to insert
			TVITEM item = new TVITEM();
			
			// Prepare helper object for this node
			TreeNodeData tnd = new TreeNodeData();
			tnd.Folder = parentFolder;
			tnd.Flags = flags;
			Debug.Assert(iMalloc!=null);
			tnd.IDList = ShellIDList.Combine(iMalloc, parentList, idList);
			Debug.Assert(tnd.IsValid());
						
			// Setup which fields we are going to use
			TreeViewItemFlags itemFlags = TreeViewItemFlags.TVIF_TEXT | TreeViewItemFlags.TVIF_IMAGE
				| TreeViewItemFlags.TVIF_SELECTEDIMAGE | TreeViewItemFlags.TVIF_CHILDREN | TreeViewItemFlags.TVIF_PARAM;
			item.mask = (uint)(itemFlags);
			string nodeText = string.Empty;
			SetupTreeViewItem(ref item, tnd, out nodeText);

            // Insert item using the Nodes collection
			TreeNode treeNode = null;
			if ( parentNode == null )
			{
				// ROOT node
				treeNode = Nodes.Add(nodeText);
			}
			else
			{
				// This is a child node, add it to the parent
               treeNode = parentNode.Nodes.Add(nodeText);
 			}
			
			IntPtr handle = treeNode.Handle;
			tnd.Handle = handle;
			// Save item data in the Tag bucket
			treeNode.Tag = tnd;
                        
			// Set some flags for the node we are inserting
			item.hItem = handle;
			// We are going to use the lParam when sorting
			item.lParam = handle;
			SetItem(ref item);
						
			// Release previously allocated memory if necessary
			if ( item.pszText != IntPtr.Zero )
				Marshal.FreeHGlobal(item.pszText);

			// Add it to the hash table for easy mapping and retrieval
			hashTable.Add(handle, treeNode);

			return treeNode;
		}

		void SetupTreeViewItem(ref TVITEM item, TreeNodeData tnd, out string path)
		{
			// Initialize out parameter
			path = string.Empty;

			// Get TreeNodeData
			Debug.Assert(tnd!=null && tnd.IsValid());
			// Relative pIDList
			IntPtr idRel = tnd.IDList.GetLastChild();
			if ( idRel == IntPtr.Zero )
			{
				// If the parent ID List has not children
				// set the relative ID List to the parent list
				idRel = tnd.IDList.Handle;
			}

			// If we need to show the node name
			if ( (item.mask & (uint)TreeViewItemFlags.TVIF_TEXT) != 0)
			{
				if ( (tnd.Flags & TreeNodeFlags.ShowPath) != 0 )
				{
					// Get full path
					path = tnd.IDList.GetPath();
					if ( path != string.Empty && (tnd.Flags & TreeNodeFlags.ShowName) != 0 )
					{
						// Get just the name of the node
						int slashIndex = path.LastIndexOf('\\');
						path = path.Substring(slashIndex+1);
					}
				}

				// If we could not get a path, try getting a global name
				if ( path == string.Empty )
				{
					ShellGetDisplayNameOfFlags flags = ShellGetDisplayNameOfFlags.SHGDN_INFOLDER;
					if ( (tnd.Flags & TreeNodeFlags.ShowName) != 0 )
						flags = ShellGetDisplayNameOfFlags.SHGDN_NORMAL;
					flags |= ShellGetDisplayNameOfFlags.SHGDN_INCLUDE_NONFILESYS;
					path = tnd.Folder.GetDisplayNameOf(idRel, flags);
				}

				// Set the Node text
				if ( path != string.Empty )
					item.pszText = Marshal.StringToHGlobalAuto(path);
			}

			// If we need to show node image
			if ( (item.mask & (uint)(TreeViewItemFlags.TVIF_IMAGE |
				TreeViewItemFlags.TVIF_SELECTEDIMAGE)) !=  0)
			{
				GetAttributeOfFlags attributes = GetAttributeOfFlags.SFGAO_FOLDER | GetAttributeOfFlags.SFGAO_LINK 
					| GetAttributeOfFlags.SFGAO_SHARE | GetAttributeOfFlags.SFGAO_GHOSTED;
				tnd.Folder.GetAttributesOf(1, ref idRel, out attributes);

				// set correct icon
				if ( (attributes & GetAttributeOfFlags.SFGAO_GHOSTED) != 0)
				{
					item.mask  |= (uint)ListViewItemFlags.LVIF_STATE;
					item.stateMask |= (uint)ListViewItemState.LVIS_CUT;
					item.state |= (uint)ListViewItemState.LVIS_CUT;
				}
				
				if ( (attributes & GetAttributeOfFlags.SFGAO_SHARE) != 0 )
				{
					item.mask |= (uint)ListViewItemFlags.LVIF_STATE;
					item.state &= ~(uint)ListViewItemState.LVIS_OVERLAYMASK;
					item.state |= WindowsAPI.INDEXTOOVERLAYMASK(1);
					item.stateMask |= (uint)ListViewItemState.LVIS_OVERLAYMASK;
				}
				else if ( (attributes & GetAttributeOfFlags.SFGAO_LINK) != 0)
				{
					item.mask |= (uint)ListViewItemFlags.LVIF_STATE;
					item.state &= ~(uint)ListViewItemState.LVIS_OVERLAYMASK;
					item.state |= WindowsAPI.INDEXTOOVERLAYMASK(2);
					item.stateMask |= (uint)ListViewItemState.LVIS_OVERLAYMASK;
				}

				if ( (item.mask & (uint)(TreeViewItemFlags.TVIF_IMAGE)) != 0 )
				{
					item.iImage = tnd.IDList.GetIconIndex(ShellFileInfoFlags.SHGFI_SMALLICON);
					item.iSelectedImage = item.iImage;
				}
				
				if ( ((item.mask & (uint)TreeViewItemFlags.TVIF_SELECTEDIMAGE) != 0)
					&& ((attributes & GetAttributeOfFlags.SFGAO_FOLDER) != 0) )
				{
					item.iSelectedImage = tnd.IDList.GetIconIndex(ShellFileInfoFlags.SHGFI_SMALLICON
						| ShellFileInfoFlags.SHGFI_OPENICON);
				}
			}

			// If the node has children and is a folder
			if ( (item.mask & (uint)TreeViewItemFlags.TVIF_CHILDREN) != 0 )
			{
				GetAttributeOfFlags attributes = GetAttributeOfFlags.SFGAO_FOLDER;
				tnd.Folder.GetAttributesOf(1, ref idRel, out attributes);

				// Get children
				item.cChildren = 0;
				if ( (attributes & GetAttributeOfFlags.SFGAO_FOLDER) != 0 )
				{
					if ( (tnd.Flags & TreeNodeFlags.ShowFiles) != 0 )
						item.cChildren = 1;
					else if ( (attributes & GetAttributeOfFlags.SFGAO_REMOVABLE) != 0 )
						item.cChildren = 1;
					else
					{
						attributes = GetAttributeOfFlags.SFGAO_HASSUBFOLDER;
						tnd.Folder.GetAttributesOf(1,  ref idRel, out attributes);
						item.cChildren = ((attributes & GetAttributeOfFlags.SFGAO_HASSUBFOLDER) != 0) ? 1 : 0;
					}
				}
			}
		}

		void SetItem(ref TVITEM item)
		{
			// Only Windows NT based systems support for the moment
			WindowsAPI.SendMessage(Handle, TreeViewMessages.TVM_SETITEMW, 0, ref item);
		}

		void GetItem(ref TVITEM item)
		{
			// Only Windows NT based systems support for the moment
			WindowsAPI.SendMessage(Handle, TreeViewMessages.TVM_GETITEMW, 0, ref item);
		}

		void EnumerateChildrenNodes(TreeNode parentNode, ShellIDList idListParent, TreeNodeFlags flags)
		{
			ShellIFolder shellFolder = null;
			if ( idListParent.IsRootList() )
			{
				// Root object, just increase the reference count for the interface
				shellFolder = new ShellIFolder(desktopFolder);
			}
			else
			{
				// Not a root item, get a new IShellFolder interface bound to the idList
				shellFolder = ShellIFolder.CreateShellIFolder(desktopFolder, idListParent);
			}

			Debug.Assert(shellFolder!=null);
		
			// Get enumeration interface
			ShellEnumFlags enumFlags = ShellEnumFlags.SHCONTF_FOLDERS |
				( ((flags & TreeNodeFlags.ShowFiles) != 0) ? ShellEnumFlags.SHCONTF_NONFOLDERS : 0) |
				( ((flags & TreeNodeFlags.ShowHiddenFiles) != 0) ? ShellEnumFlags.SHCONTF_INCLUDEHIDDEN : 0);

			ShellIEnumIDList shellEnumIDList = null;
			try 
			{
				shellEnumIDList = ShellIEnumIDList.CreateShellIEnumIDList(Handle, shellFolder, enumFlags);
			}
			catch(Exception e)
			{
				// We are going to get an exception if we tried to enumerate a removable
				// media and the user did not insert a CD or other valid media
				Debug.WriteLine(e.Message);
				return;
			}
			
			// Do the insertion
			IntPtr idListPtr = IntPtr.Zero;
			uint fetched = 0;
			while ( (int)WinErrors.NOERROR == shellEnumIDList.Next(1, ref idListPtr, out fetched) )
			{
				ShellIDList idList = new ShellIDList(idListPtr);
				InsertNode(parentNode, shellFolder, idListParent, idList, flags);
				idList.Dispose();
			}

			// Sort the recently inserted children
			SortChildren(parentNode);
            			          
		}
		void DeleteChildren(TreeNode parentNode)
		{
			int count = parentNode.Nodes.Count;
			while ( count != 0 )
			{
				// Remove children nodes
				// Remove from hash table first
				TreeNode childNode = (TreeNode)parentNode.Nodes[0];

				// Actively release the shell Memory allocated here
				// -- Letting the garbage collector do it from its thread 
				// causes an exception --
				TreeNodeData tnd = (TreeNodeData)childNode.Tag;
				Debug.Assert(tnd!=null);
				tnd.IDList.Dispose();

				// Recursively delete the children of
				// the child node
				DeleteChildren(childNode);
				hashTable.Remove(childNode.Handle);
				
				// Remove from the collection
				parentNode.Nodes.RemoveAt(0);
				count = parentNode.Nodes.Count;

			}
			
			ResetChildrenCountFlag(parentNode.Handle);
		}

		void DeleteNode(TreeNode treeNode)
		{
			bool hasChildren = false;
			IntPtr handle = treeNode.Handle;
			if ( treeNode.Nodes.Count != 0 )
			{
				hasChildren = true;
				DeleteChildren(treeNode);
			}
			// Now delete this node from the collection
			Nodes.Remove(treeNode);

			// Actively release the shell Memory allocated here
			// -- Letting the garbage collector do it from its thread 
			// causes an exception --
			TreeNodeData tnd = (TreeNodeData)treeNode.Tag;
			Debug.Assert(tnd!=null);
			tnd.IDList.Dispose();
			hashTable.Remove(handle);
			
			if ( hasChildren )
				ResetChildrenCountFlag(handle);
            
		}


		void ResetChildrenCountFlag(IntPtr hItem)
		{
			// Since we are wiping out all children
			// we need to reset the style of the node
			// so that it shows the plus sign 
			TVITEM item = new TVITEM();
			item.hItem = hItem;
			item.mask |= (uint)(TreeViewItemFlags.TVIF_CHILDREN | TreeViewItemFlags.TVIF_HANDLE);
			item.cChildren = 1;
			SetItem(ref item);
		}

		void SortChildren(TreeNode parentNode)
		{
			TVSORTCB tvscb = new TVSORTCB();
            tvscb.hParent = parentNode.Handle;
			tvscb.lpfnCompare = new WindowsAPI.CompareFunc(OnCompare);

			// Pin the delegate object so that it can be passed
			// to unmanaged code without risk of the garbage collector free its memory
			GCHandle compareHandle = GCHandle.Alloc(tvscb.lpfnCompare);
			
			// Now do the sorting
			WindowsAPI.SendMessage(Handle, TreeViewMessages.TVM_SORTCHILDRENCB, 0, ref tvscb);

			// Free our handle
			compareHandle.Free();
            			
		}

		int OnCompare(IntPtr param1, IntPtr param2, IntPtr sortParam)
		{
			// Do the comparison
            TreeNode tn1 = (TreeNode)hashTable[param1];
            TreeNode tn2 = (TreeNode)hashTable[param2];
			Debug.Assert(tn1!=null && tn2!=null);

			TreeNodeData tnd1 = (TreeNodeData)tn1.Tag;
			TreeNodeData tnd2 = (TreeNodeData)tn2.Tag;
			Debug.Assert(tnd1!=null && tnd2!=null);

			// Sort items by comparing using the parent
			// folder to compare their IDs
			ShellIFolder folder = tnd1.Folder;
			Debug.Assert(folder!=null);

            return folder.CompareIDs(0, tnd1.IDList, tnd2.IDList);            
		}

		bool IsRemovableMedia(ShellIDList idList)
		{
			StringBuilder path = new StringBuilder(1024);
            int result = WindowsAPI.SHGetPathFromIDList(idList, path);
			if ( result != 0 )
			{
				string drivePath = path.ToString();
				drivePath = drivePath.Substring(0,2);
				DriveType driveType = (DriveType)WindowsAPI.GetDriveType(drivePath);
				if ( driveType == DriveType.DRIVE_CDROM || driveType == DriveType.DRIVE_REMOVABLE )
					return true;
			}

			return false;
            
		}
		void OnContextMenu(MouseEventArgs e)
		{
			// Display shell context menu for the item that was clicked
			TreeViewHitTestFlags htFlags;
			IntPtr hItem = HitTest(new Point(e.X, e.Y), out htFlags);
			
			// If we did not click on an item, just return
			if ( (htFlags & TreeViewHitTestFlags.TVHT_ONITEM) == 0 )
				return;
            
			// Should have a valid item
			Debug.Assert(hItem!=IntPtr.Zero);
			TreeNode tn = (TreeNode)hashTable[hItem];
			Debug.Assert(tn!=null);
			// Select this node
			SelectedNode = tn;

			// Get item shellInformation
			TreeNodeData tnd = (TreeNodeData)tn.Tag;
			Debug.Assert(tnd!=null);
            
            // Get clicked item context menu
			contextMenu.Initialize(tnd.Folder, tnd.IDList);
			// Now display the menu
			contextMenu.Show(this, new Point(e.X, e.Y));
					
		}

		void InvokeCommand(int commandID)
		{
			contextMenu.InvokeCommand(commandID);
		}

		void FireExplorerNodeChanged(TreeNodeData tnd)
		{
			if ( ExplorerNodeChanged != null )
			{
				ExplorerNodeChanged(this, tnd);
			}
		}
		void OnDestroyTree()
		{
			// Delete all the nodes here so that
			// we dispose of the memory allocated by IMalloc here
			// in the main UI thread instead of letting the Garbage collector
			// to call the dispose method itself since it seems to be 
			// a problem with COM thread initialization
			if ( rootTreeNode != null )
			{
				DeleteChildren(rootTreeNode);
			}
		}
		#endregion

	}
}
