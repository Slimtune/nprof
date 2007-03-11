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
using System.Windows.Forms;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ContainerListViewItem"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ContainerListViewItemCollection</b> provides an <see cref="ArrayList"/>
	/// that is strongly typed for <see cref="ContainerListViewItem"/> elements.
	/// </remarks>
	public sealed class ContainerListViewItemCollection : IList, ICollection
	{
		#region Variables

		private ContainerListView _listView;
		private ContainerListViewItem _owningItem;
		private ArrayList _data = new ArrayList();

		#endregion

		#region Constructors

		internal ContainerListViewItemCollection(ContainerListViewItem owningItem)
		{
			_owningItem = owningItem;
		}

		#endregion

		/// <summary>
		/// Indicates the <see cref="ContainerListViewItem"/> at the specified indexed
		/// location in the collection.  In C#, this property is the indexer for the
		/// <b>ContainerListViewItemCollection</b> class.
		/// </summary>
		public ContainerListViewItem this[int index]
		{
			get
			{
				return _data[index] as ContainerListViewItem;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "ContainerListView cannot contain null ContainerListViewItems");

				if(value != _data[index])
				{
					// remove the existing item
					ContainerListViewItem item = this[index];
					item.OwnerListView = null;

					// and add the new item in place
					_data[index] = value;
					value.OwnerListView = _listView;
				}
			}
		}

		#region Add

		/// <summary>
		/// Adds an existing <see cref="ContainerListViewItem"/> object to the collection.
		/// </summary>
		/// <param name="item">The <b>ContainerListViewItem</b> object to add to the collection.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		public int Add(ContainerListViewItem item)
		{
			int idx = _data.Count;

			Insert(idx, item);

			return idx;
		}

		/// <summary>
		/// Adds an item to the collection with the specified text.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Add(string text)
		{
			return Insert(_data.Count, text);
		}

		/// <summary>
		/// Adds an item to the collection with the specified text and image index.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Add(string text, int imageIndex)
		{
			return Insert(_data.Count, text, imageIndex);
		}

		/// <summary>
		/// Adds an item to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <param name="selectedImageIndex">The index of the image to display when the item is selected.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Add(string text, int imageIndex, int selectedImageIndex)
		{
			return Insert(_data.Count, text, imageIndex, selectedImageIndex);
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts an existing <see cref="ContainerListViewItem"/> object to the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="item">The <b>ContainerListViewItem</b> object to add to the collection.</param>
		public void Insert(int index, ContainerListViewItem item)
		{
			if(_data.Count != 0)
			{
				ContainerListViewItem previousItem = _data[_data.Count - 1] as ContainerListViewItem;
				item.InternalPreviousItem = previousItem;
				previousItem.InternalNextItem = item;
			}

			lock(_data.SyncRoot)
				_data.Insert(index, item);

			item.InternalParentItem = _owningItem;
			item.OwnerListView = _listView;
		}

		/// <summary>
		/// Inserts an item to the collection with the specified text at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Insert(int index, string text)
		{
			ContainerListViewItem item = new ContainerListViewItem(text);

			Insert(index, item);

			return item;
		}

		/// <summary>
		/// Inserts an item to the collection with the specified text and image index at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Insert(int index, string text, int imageIndex)
		{
			ContainerListViewItem item = new ContainerListViewItem(text, imageIndex);

			Insert(index, item);

			return item;
		}

		/// <summary>
		/// Inserts an item to the collection with the specified properties at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <param name="selectedImageIndex">The index of the image to display when the item is selected.</param>
		/// <returns>The <see cref="ContainerListViewItem"/> that was added to the collection.</returns>
		public ContainerListViewItem Insert(int index, string text, int imageIndex, int selectedImageIndex)
		{
			ContainerListViewItem item = new ContainerListViewItem(text, imageIndex, selectedImageIndex);

			Insert(index, item);

			return item;
		}

		#endregion

		/// <summary>
		/// Removes the specified <see cref="ContainerListViewItem"/> from the collection.
		/// </summary>
		/// <param name="item">A <see cref="ContainerListViewItem"/> to remove from the collection.</param>
		public void Remove(ContainerListViewItem item)
		{
			if(item.InternalPreviousItem != null && item.InternalPreviousItem != null)
			{
				item.InternalPreviousItem.InternalNextItem = item.InternalNextItem;
				item.InternalNextItem.InternalPreviousItem = item.InternalPreviousItem;
			}
			else if(item.InternalPreviousItem != null)
				item.InternalPreviousItem.InternalNextItem = null;
			else if(item.InternalNextItem != null)
				item.InternalNextItem.InternalPreviousItem = null;

			lock(_data.SyncRoot)
				_data.Remove(item);

			item.Selected = false;
			item.Focused = false;
			item.OwnerListView = null;
			item.InternalParentItem = null;
		}

		/// <summary>
		/// Adds an array of <see cref="ContainerListViewItem"/> objects to the collection.
		/// </summary>
		/// <param name="items">An array of <see cref="ContainerListViewItem"/> objects to add to the collection.</param>
		public void AddRange(ContainerListViewItem[] items)
		{
			_listView.BeginUpdate();

			lock(_data.SyncRoot)
				for(int index = 0; index < items.Length; ++index)
					Add(items[index]);

			_listView.EndUpdate();
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
		/// Removes all items from the collection.
		/// </summary>
		public void Clear()
		{
			_listView.BeginUpdate();

			for(int index = 0; index < _data.Count; ++index)
			{
				ContainerListViewItem item = this[index];

				item.Selected = false;
				item.Focused = false;
				item.OwnerListView = null;

				for(int subIndex = 0; subIndex < item.SubItems.Count; ++subIndex)
				{
					if (item.SubItems[subIndex].ItemControl != null)
					{
						item.SubItems[subIndex].ItemControl.Parent = null;
						item.SubItems[subIndex].ItemControl.Visible = false;
						item.SubItems[subIndex].ItemControl = null;
					}
				}
			}
			_data.Clear();

			_listView.EndUpdate();
		}

		/// <summary>
		/// Sorts the elements using the specified comparer.
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer"/> to use when comparing elements.</param>
		/// <param name="recursiveSort">Whether to sort these items child items as well.</param>
		public void Sort(IComparer comparer, bool recursiveSort)
		{
			try
			{
				_data.Sort(comparer);

				ContainerListViewItem lastItem = null;
				ContainerListViewItem curItem = null;

				for(int idx = 0; idx < _data.Count; ++idx)
				{
					curItem = this[idx];

					curItem.InternalPreviousItem = lastItem;
					if(lastItem != null)
						lastItem.InternalNextItem = curItem;

					lastItem = curItem;

					if(recursiveSort && curItem.HasChildren)
						curItem.Items.Sort(comparer, recursiveSort);
				}

				if(curItem != null)
					curItem.InternalNextItem = null;
			}
			catch
			{
				// TODO: should likely refine this and determine the cause of the error and handle appropriately.
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

		/// <summary>
		/// Gets the number of items in this collection
		/// </summary>
		public int Count
		{
			get
			{
				return _data.Count;
			}
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
				this[idx] = value as ContainerListViewItem;
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

		internal ContainerListView InternalListView
		{
			set
			{
				_listView = value;
			}
		}
	}
}
