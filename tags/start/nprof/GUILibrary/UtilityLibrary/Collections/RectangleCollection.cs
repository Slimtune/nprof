using System;
using System.Collections;
using System.Drawing;

namespace UtilityLibrary.Collections 
{
	/// <summary>
	/// Summary description for RectangleCollection.
	/// </summary>
	public class RectangleCollection : IEnumerable
	{
		#region Events
		public event EventHandler Changed;
		#endregion
		
		#region Class Variables	
		ArrayList items = new ArrayList();
		#endregion
			
		#region Constructors
		public RectangleCollection()
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

		public int Add(Rectangle item)
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

		public bool Contains(Rectangle item)
		{
			return items.Contains(item);
		}

		public int IndexOf(Rectangle item)
		{
			return items.IndexOf(item);
		}

		public void Remove(Rectangle item)
		{
			items.Remove(item);
			RaiseChanged();		
		}
	
		public void RemoveAt(int index)
		{
			items.RemoveAt(index);
			RaiseChanged();
		}

		public void Insert(int index, Rectangle item)
		{
			items.Insert(index, item);
			RaiseChanged();
		}
	
		public Rectangle this[int index]
		{
			get { return (Rectangle) items[index]; }
			set {  items[index] = value; }
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


