using System;
using System.Collections;
using System.Windows.Forms;
using UtilityLibrary.WinControls;

namespace UtilityLibrary.Collections
{
	/// <summary>
	/// Summary description for OutlookBarItemCollection.
	/// </summary>
	public class OutlookBarItemCollection : System.Collections.CollectionBase, IEnumerable
	{

		#region Events
		public event EventHandler Changed;
		#endregion
		
		#region Constructors
		public OutlookBarItemCollection()
		{
			
		}
		#endregion

		#region Methods
		
		public int Add(OutlookBarItem item)
		{
			if (Contains(item)) return -1;
			int index = InnerList.Add(item);
			RaiseChanged();
			return index;
		}

		public bool Contains(OutlookBarItem item)
		{
			return InnerList.Contains(item);
		}
	
		public int IndexOf(OutlookBarItem item)
		{
			return InnerList.IndexOf(item);
		}
	
		public void Remove(OutlookBarItem item)
		{
			InnerList.Remove(item);
			RaiseChanged();		
		}
		
		public void Insert(int index, OutlookBarItem item)
		{
			InnerList.Insert(index, item);
			RaiseChanged();
		}
		
		public OutlookBarItem this[int index]
		{
			get { return (OutlookBarItem) InnerList[index]; }
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


