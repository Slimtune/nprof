using System;
using System.Collections;
using System.Drawing;

namespace UtilityLibrary.Collections
{
	/// <summary>
	/// Summary description for BitmapCollection.
	/// </summary>
	public class BitmapCollection : IEnumerable
	{
		
		#region Class Variables
		public event EventHandler Changed;
		ArrayList items = new ArrayList();
		#endregion
			
		#region Constructor
		public BitmapCollection()
		{
		
		}
		
		#endregion

		#region Properties
		public int Count
		{
			get { return items.Count; }
		}	
	
		#endregion

		#region Methods
		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();		
		}

		public int Add(Bitmap item)
		{
			if (Contains(item)) return -1;
			int index = items.Add(item);
			RaiseChanged();
			return index;
		}

		public void Clear()
		{
			while (Count > 0) RemoveAt(0);
		}

		public bool Contains(Bitmap item)
		{
			return items.Contains(item);
		}

		public int IndexOf(Bitmap item)
		{
			return items.IndexOf(item);
		}

		public void Remove(Bitmap item)
		{
			items.Remove(item);
			RaiseChanged();		
		}
	
		public void RemoveAt(int index)
		{
			items.RemoveAt(index);
			RaiseChanged();
		}

		public void Insert(int index, Bitmap item)
		{
			items.Insert(index, item);
			RaiseChanged();
		}
	
		public Bitmap this[int index]
		{
			get { return (Bitmap) items[index]; }
			set {  items[index] = value; }
		}

		#endregion 
		
		#region Implementatioin
		void RaiseChanged()
		{
			if (Changed != null) Changed(this, null);
		}
		#endregion

	}
}


