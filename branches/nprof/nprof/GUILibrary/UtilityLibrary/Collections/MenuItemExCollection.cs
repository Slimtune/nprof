using System;
using System.Collections;
using System.Windows.Forms;
using UtilityLibrary.WinControls;
using UtilityLibrary.Menus;

namespace UtilityLibrary.Collections
{
	/// <summary>
	/// Summary description for MenuItemExCollection.
	/// </summary>
	public class MenuItemExCollection : System.Collections.CollectionBase, IEnumerable
	{

		#region Events
		public event EventHandler Changed;
		#endregion
		
		#region Constructors
		public MenuItemExCollection()
		{
			
		}
		#endregion

		#region Methods
		public int Add(MenuItemEx item)
		{
			if (Contains(item)) return -1;
			int index = InnerList.Add(item);
			RaiseChanged();
			return index;
		}

		public bool Contains(MenuItemEx item)
		{
			return InnerList.Contains(item);
		}
	
		public int IndexOf(MenuItemEx item)
		{
			return InnerList.IndexOf(item);
		}
	
		public void Remove(MenuItemEx item)
		{
			InnerList.Remove(item);
			RaiseChanged();		
		}

		public void Insert(int index, MenuItemEx item)
		{
			InnerList.Insert(index, item);
			RaiseChanged();
		}

		public MenuItemEx this[int index]
		{
			get { return (MenuItemEx) InnerList[index]; }
			set {  InnerList[index] = value; }
		}
		#endregion
	
		#region Implementation
		void RaiseChanged()
		{
			if (Changed != null) Changed(this, null);
		}
		#endregion

	}
}


