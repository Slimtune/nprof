using System;
using System.Runtime.InteropServices;


namespace UtilityLibrary.Win32
{

	#region IUnknown
	[ComImport, Guid("00000000-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IUnknown
	{
		[PreserveSig]
		IntPtr QueryInterface(REFIID riid, out IntPtr pVoid);
		
		[PreserveSig]
		IntPtr AddRef();

		[PreserveSig]
		IntPtr Release();
	}
	#endregion

	#region IMalloc
	[ComImport, Guid("00000002-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMalloc
	{
		[PreserveSig]
		IntPtr Alloc(uint cb);

		[PreserveSig]
		IntPtr Realloc(IntPtr pv, uint cb);

		[PreserveSig]
		void Free(IntPtr pv);

		[PreserveSig]
		uint GetSize(IntPtr pv);

		[PreserveSig]
		int DidAlloc(IntPtr pv);

		[PreserveSig]
		void HeapMinimize();
	}
	#endregion

	#region IShellFolder
	[ComImport, Guid("000214E6-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IShellFolder
	{
		[PreserveSig]
		int ParseDisplayName(IntPtr hWnd, IntPtr bindingContext, 
			IntPtr OLEString, out uint chEaten, ref IntPtr idList, ref uint attributes);
		
		[PreserveSig]
		int EnumObjects(IntPtr hWnd, ShellEnumFlags flags,  ref IEnumIDList enumList);

		[PreserveSig]
        int BindToObject(IntPtr idList, IntPtr bindingContext, ref REFIID refiid, ref IShellFolder folder);
        
		[PreserveSig]
		int BindToStorage(ref IntPtr idList, IntPtr bindingContext, ref REFIID riid, IntPtr pVoid);

		[PreserveSig]
		int CompareIDs(int lparam, IntPtr idList1, IntPtr idList2);
        
		[PreserveSig]
		int CreateViewObject(IntPtr hWnd, REFIID riid, IntPtr pVoid);
        
		[PreserveSig]
		int GetAttributesOf(uint count, ref IntPtr idList, out GetAttributeOfFlags attributes);

		[PreserveSig]
		int GetUIObjectOf(IntPtr hWnd, uint count, ref IntPtr idList, 
			ref REFIID riid, out uint arrayInOut, ref IUnknown iUnknown);

		[PreserveSig]
		int GetDisplayNameOf(IntPtr idList, ShellGetDisplayNameOfFlags flags, out STRRET strRet);

		[PreserveSig]
		int SetNameOf(IntPtr hWnd, ref IntPtr idList,
			IntPtr pOLEString, uint flags, ref IntPtr pItemIDList);
        
	}
    #endregion

	#region IEnumIDList
	[ComImport, Guid("000214f2-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumIDList
	{
		[PreserveSig]
        int Next(uint count, ref IntPtr idList, out uint fetched);
 
		[PreserveSig]
		int Skip(uint count);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Clone(ref IEnumIDList list);
	}
	#endregion

	#region IContextMenu
	[ComImport, Guid("000214e4-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IContextMenu
	{
		[PreserveSig]
        int QueryContextMenu(IntPtr hMenu, uint indexMenu, uint idFirstCommand, uint idLastCommand, QueryContextMenuFlags flags);
    
		[PreserveSig]
		int InvokeCommand(ref CMINVOKECOMMANDINFO ici);

		[PreserveSig]
        int GetCommandString(uint idCommand, uint type, uint reserved, string commandName, uint cchMax);
	}
	#endregion

	#region IContextMenu2
	[ComImport, Guid("000214f4-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IContextMenu2
	{

		[PreserveSig]
		int HandleMenuMsg(uint message, int wParam, int lParam);
	}
	#endregion
    
}