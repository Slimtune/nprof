/***************************************************************************\
|  Author:  Josh Carlson                                                    |
|                                                                           |
|  This work builds on code posted to CodeProject                           |
|  Jon Rista http://codeproject.com/cs/miscctrl/extendedlistviews.asp       |
|  and also updates by                                                      |
|  Bill Seddon http://codeproject.com/cs/miscctrl/Extended_List_View_2.asp  |
|                                                                           |
|  This code is provided "as is" and no warranty about                      |
|  it fitness for any specific task is expressed or                         |
|  implied.  If you choose to use this code, you do so                      |
|  at your own risk.                                                        |
\***************************************************************************/

using System;
using System.Collections;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ContainerListViewItem"/> elements
	/// that are currently selected in a <see cref="ContainerListView"/>.
	/// </summary>
	/// <remarks>
	/// <b>ContainerListViewSelectedItemCollection</b> provides an <see cref="ArrayList"/>
	/// that is strongly typed for <see cref="ContainerListViewItem"/> elements. The items
	/// always are maintained sorted by the item's position in the list.
	/// </remarks>
	public sealed class ContainerListViewSelectedItemCollection : IList, ICollection
	{
		#region Variables

		private ContainerListView _listView;
		private ArrayList _data = new ArrayList();

		#endregion

		#region Constructors

		internal ContainerListViewSelectedItemCollection(ContainerListView listView)
		{
			_listView = listView;
		}

		#endregion

		/// <summary>
		/// Indicates the <see cref="ContainerListViewItem"/> at the specified indexed
		/// location in the collection.  In C#, this property is the indexer for the
		/// <b>ContainerListViewSelectedItemCollection</b> class.
		/// </summary>
		public ContainerListViewItem this[int index]
		{
			get
			{
				return _data[index] as ContainerListViewItem;
			}
		}

		/// <summary>
		/// Selects existing <see cref="ContainerListViewItem"/> object to the list.
		/// </summary>
		/// <param name="item">The <b>ContainerListViewItem</b> object to select.</param>
		public int Add(ContainerListViewItem item)
		{
			if(item.ListView != _listView)
				throw new ArgumentException("Cannot select an item that isn't part of this ContainerListView", "item");

			return _data.Add(item);
		}

		/// <summary>
		/// Always throws <see cref="NotSupportedException"/>, use the Add method instead.
		/// </summary>
		public void Insert(int index, ContainerListViewItem item)
		{
			throw new NotSupportedException("Cannot insert an item into the selected collection at an arbitrary location.");
		}

		/// <summary>
		/// Removes a <see cref="ContainerListViewItem"/> object from the selected list.
		/// </summary>
		/// <param name="item">The <b>ContainerListViewItem</b> object you want to remove from being selected.</param>
		public void Remove(ContainerListViewItem item)
		{
			_data.Remove(item);
		}

		/// <summary>
		/// Adds an array of <see cref="ContainerListViewItem"/> objects to the selected item collection.
		/// </summary>
		/// <param name="items">An array of <see cref="ContainerListViewItem"/> objects to add to the collection.</param>
		public void AddRange(ContainerListViewItem[] items)
		{
			lock(_data.SyncRoot)
			{
				for(int idx = 0; idx < items.Length; ++idx)
					_data.Add(items[idx]);
			}
		}

		/// <summary>
		/// Returns the index within the collection of the specified item.
		/// </summary>
		/// <param name="item">A <see cref="ContainerListViewItem"/> representing the item to locate in the collection.</param>
		/// <returns>The zero-based index of the item's location in the collection.  If the item is not located in the collection the return value is negative one (-1).</returns>
		public int IndexOf(ContainerListViewItem item)
		{
			return _data.IndexOf(item);
		}

		/// <summary>
		/// Determines whether the specified item is located in the collection.
		/// </summary>
		/// <param name="item">A <see cref="ContainerListViewItem"/> representing the item to locate in the collection.</param>
		/// <returns><b>true</b> if the column is contained in the collection; otherwise, <b>false</b>.</returns>
		public bool Contains(ContainerListViewItem item)
		{
			return _data.Contains(item);
		}

		/// <summary>
		/// De-selects all the items.
		/// </summary>
		public void Clear()
		{
			_listView.ClearSelectedItems(true);
		}

		/// <summary>
		/// Gets the number of <see cref="ContainerListViewItem"/> elements in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return _data.Count;
			}
		}

		/// <summary>
		/// Copies the entire collection into an existing array at a specified location within the array.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The zero-based relative index in <em>array</em> at which copying begins.</param>
		public void CopyTo(ContainerListViewItem[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

		internal void InternalClear()
		{
			_data.Clear();
		}

		#region IList

		int IList.Add(object o)
		{
			return this.Add(o as ContainerListViewItem);
		}

		bool IList.Contains(object o)
		{
			return this.Contains(o as ContainerListViewItem);
		}

		int IList.IndexOf(object o)
		{
			return this.IndexOf(o as ContainerListViewItem);
		}

		void IList.Insert(int index, object o)
		{
			this.Insert(index, o as ContainerListViewItem);
		}

		void IList.Remove(object o)
		{
			this.Remove(o as ContainerListViewItem);
		}

		void IList.RemoveAt(int index)
		{
			_data.RemoveAt(index);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int idx]
		{
			get
			{
				return this[idx];
			}
			set
			{
				throw new NotSupportedException("Cannot insert an item at an arbitrary position.  Use Add instead.");
			}
		}

		#endregion

		#region ICollection

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			this.CopyTo((ContainerListViewItem[])array, arrayIndex);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return _data.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return _data.IsSynchronized;
			}
		}

		#endregion

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		#endregion

	}

}
