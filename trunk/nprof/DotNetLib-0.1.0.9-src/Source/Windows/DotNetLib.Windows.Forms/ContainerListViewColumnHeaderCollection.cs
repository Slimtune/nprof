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
using System.Drawing;
using System.Windows.Forms;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ContainerListViewColumnHeader"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ContainerListViewColumnHeaderCollection</b> provides an <see cref="ArrayList"/> 
	/// that is strongly typed for <see cref="ContainerListViewColumnHeader"/> elements.
	/// </remarks>    
	public sealed class ContainerListViewColumnHeaderCollection : IList, ICollection
	{
		#region Variables

		private ContainerListView _listView;
		private ArrayList _data = new ArrayList();

		#endregion

		#region Constructors

		internal ContainerListViewColumnHeaderCollection(ContainerListView listView)
		{
			_listView = listView;
		}

		#endregion

		/// <summary>
		/// Indicates the <see cref="ContainerListViewColumnHeader"/> at the specified indexed
		/// location in the collection.  In C#, this property is the indexer for the
		/// <b>ContainerListViewColumnHeaderCollection</b> class.
		/// </summary>
		public ContainerListViewColumnHeader this[int index]
		{
			get
			{ 
				return _data[index] as ContainerListViewColumnHeader;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "ContainerListView cannot contain null ContainerListViewColumnHeaders");

				if(value != _data[index])
				{
					// remove the existing column from the list
					ContainerListViewColumnHeader cur = this[index];
					int displayIndex = cur.DisplayIndex;
					cur.OwnerListView = null;

					// and add the new column in place
					_data[index] = value;
					value.OwnerListView = _listView;
					value.DisplayIndex = displayIndex;
				}
			}
		}

		#region Add

		/// <summary>
		/// Adds an existing <see cref="ContainerListViewColumnHeader"/> object to the collection.
		/// </summary>
		/// <param name="column">The <b>ContainerListViewColumnHeader</b> object to add to the collection.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		public int Add(ContainerListViewColumnHeader column)
		{
			int index = _data.Count;

			Insert(index, column);

			return index;
		}

		/// <summary>
		/// Adds a column to the collection with the specified text.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was added to the collection.</returns>
		public ContainerListViewColumnHeader Add(string text)
		{
			return Insert(_data.Count, text);
		}

		/// <summary>
		/// Adds a column to the collection with the specified text and width.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was added to the collection.</returns>
		public ContainerListViewColumnHeader Add(string text, int width)
		{
			return Insert(_data.Count, text, width);
		}

		/// <summary>
		/// Adds a column to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="horizontalAlign">The horizontal alignment, will default vertical alignment to middle.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was added to the collection.</returns>
		public ContainerListViewColumnHeader Add(string text, int width, HorizontalAlignment horizontalAlign)
		{
			return Insert(_data.Count, text, width, horizontalAlign);
		}

		/// <summary>
		/// Adds a column to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="contentAlign">The content alignment.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was added to the collection.</returns>
		public ContainerListViewColumnHeader Add(string text, int width, ContentAlignment contentAlign)
		{
			return Insert(_data.Count, text, width, contentAlign);
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts an existing <see cref="ContainerListViewColumnHeader"/> into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="column">The <see cref="ContainerListViewColumnHeader"/> that represents the column to insert.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was inserted into the collection.</returns>
		public ContainerListViewColumnHeader Insert(int index, ContainerListViewColumnHeader column)
		{
			lock(_data.SyncRoot)
				_data.Insert(index, column);

			column.OwnerListView = _listView;

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was inserted into the collection.</returns>
		public ContainerListViewColumnHeader Insert(int index, string text)
		{
			ContainerListViewColumnHeader column = new ContainerListViewColumnHeader(text);

			Insert(index, column);

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was inserted into the collection.</returns>
		public ContainerListViewColumnHeader Insert(int index, string text, int width)
		{
			ContainerListViewColumnHeader column = new ContainerListViewColumnHeader(text, width);

			Insert(index, column);

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="horizontalAlign">The horizontal alignment of the text, will default vertical alignment to middle.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was inserted into the collection.</returns>
		public ContainerListViewColumnHeader Insert(int index, string text, int width, HorizontalAlignment horizontalAlign)
		{
			ContainerListViewColumnHeader column = new ContainerListViewColumnHeader(text, width, horizontalAlign);

			Insert(index, column);

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="contentAlign">The content alignment.</param>
		/// <returns>The <see cref="ContainerListViewColumnHeader"/> that was inserted into the collection.</returns>
		public ContainerListViewColumnHeader Insert(int index, string text, int width, ContentAlignment contentAlign)
		{
			ContainerListViewColumnHeader column = new ContainerListViewColumnHeader(text, width, contentAlign);

			Insert(index, column);

			return column;
		}

		#endregion

		/// <summary>
		/// Removes the specified <see cref="ContainerListViewColumnHeader"/> from the collection.
		/// </summary>
		/// <param name="column">A <see cref="ContainerListViewColumnHeader"/> to remove from the collection.</param>
		public void Remove(ContainerListViewColumnHeader column)
		{
			lock(_data.SyncRoot)
				_data.Remove(column);

			column.OwnerListView = null;
		}

		/// <summary>
		/// Removes the column at the specified location.
		/// </summary>
		/// <param name="index">The zero-based index of the column you want to remove.</param>
		public void RemoveAt(int index)
		{
			lock(_data.SyncRoot)
				_data.RemoveAt(index);
		}

		/// <summary>
		/// Adds an array of <see cref="ContainerListViewColumnHeader"/> objects to the collection.
		/// </summary>
		/// <param name="columns">An array of <see cref="ContainerListViewColumnHeader"/> objects to add to the collection.</param>
		public void AddRange(ContainerListViewColumnHeader[] columns)
		{
			_listView.BeginUpdate();

			lock(_data.SyncRoot)
				for(int index = 0; index < columns.Length; ++index)
					Add(columns[index]);

			_listView.EndUpdate();
		}

		/// <summary>
		/// Returns the index within the collection of the specified column.
		/// </summary>
		/// <param name="column">A <see cref="ContainerListViewColumnHeader"/> representing the column to locate in the collection.</param>
		/// <returns>The zero-based index of the column's location in the collection.  If the column is not located in the collection the return value is negative one (-1).</returns>
		public int IndexOf(ContainerListViewColumnHeader column)
		{
			return _data.IndexOf(column);
		}

		/// <summary>
		/// Determines whether the specified column is located in the collection.
		/// </summary>
		/// <param name="column">A <see cref="ContainerListViewColumnHeader"/> representing the column to locate in the collection.</param>
		/// <returns><b>true</b> if the column is contained in the collection; otherwise, <b>false</b>.</returns>
		public bool Contains(ContainerListViewColumnHeader column)
		{
			return _data.Contains(column);
		}

		/// <summary>
		/// Removes all columns from the collection.
		/// </summary>
		public void Clear()
		{
			_listView.BeginUpdate();

			for(int index = 0; index < _data.Count; ++index)
				this[index].OwnerListView = null;

			_data.Clear();

			_listView.EndUpdate();
		}

		/// <summary>
		/// Gets the number of <see cref="ContainerListViewColumnHeader"/> in this collection.
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

		#region IList

		int IList.Add(object o)
		{
			return this.Add(o as ContainerListViewColumnHeader);
		}

		bool IList.Contains(object o)
		{
			return this.Contains(o as ContainerListViewColumnHeader);
		}

		int IList.IndexOf(object o)
		{
			return this.IndexOf(o as ContainerListViewColumnHeader);
		}

		void IList.Insert(int index, object o)
		{
			this.Insert(index, o as ContainerListViewColumnHeader);
		}

		void IList.Remove(object o)
		{
			this.Remove(o as ContainerListViewColumnHeader);
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
				this[idx] = value as ContainerListViewColumnHeader;
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
