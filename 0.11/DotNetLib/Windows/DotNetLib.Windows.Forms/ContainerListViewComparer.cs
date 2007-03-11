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
using System.Globalization;
using System.Windows.Forms;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Exposes a method that can filter objects
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		/// Filters an object and returns a value indicating whether or not it should be included in the filter
		/// </summary>
		/// <param name="o">The object to filter.</param>
		/// <returns><b>True</b> if the item belongs in the filter, <b>false</b> otherwise.</returns>
		bool Belongs(object o);
	}

	/// <summary>
	/// Represents a filter that always returns the same value
	/// </summary>
	public class FilterStub : IFilter
	{
		/// <summary>
		/// Represents an <see cref="IFilter"/> that will include all items.
		/// </summary>
		public static readonly FilterStub AllFilter = new FilterStub(true);

		/// <summary>
		/// Represents an <see cref="IFilter"/> that will include none of the items.
		/// </summary>
		public static readonly FilterStub NoneFilter = new FilterStub(false);

		private bool _value;

		/// <summary>
		/// Creates a new FilterStub that always returns this value
		/// </summary>
		/// <param name="value">The value to return in the filter</param>
		private FilterStub(bool value)
		{
			_value = value;
		}

		/// <summary>
		/// Always returns the value passed in through the constructor
		/// </summary>
		/// <param name="o">The object to filter, completely ignore in this filter</param>
		/// <returns>The value that was passed into the constructor.</returns>
		public bool Belongs(object o)
		{
			return _value;
		}
	}

	/// <summary>
	/// Represents a basic string comparison for a single <see cref="ContainerListViewColumnHeader"/> in a <see cref="ContainerListView"/>.
	/// </summary>
	public class ContainerListViewItemFilter : IFilter
	{
		private int _columnIndex;
		private string _string;

		/// <summary>
		/// Creates a new filter with the specified text and column index
		/// </summary>
		/// <param name="columnIndex">The index of the column this filter should work on.</param>
		/// <param name="filterText">The text to filter for, it is case insensitive.</param>
		public ContainerListViewItemFilter(int columnIndex, string filterText)
		{
			_columnIndex = columnIndex;
			_string = filterText.ToLower(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Performs the filtering for the passed in <see cref="ContainerListViewItem"/>.
		/// </summary>
		/// <param name="o">An object that should represent a <see cref="ContainerListViewItem"/>.</param>
		/// <returns></returns>
		public bool Belongs(object o)
		{
			ContainerListViewItem item = o as ContainerListViewItem;

			if(o == null)
				return false;

			string actual = item.SubItems[_columnIndex].Text.ToLower(CultureInfo.CurrentCulture);

			if(actual.IndexOf(_string) != -1)
				return true;
			else
				return false;
		}
	}

	/// <summary>
	/// A fairly optimized IComparer for sorting strings based on different data types
	/// </summary>
	public class ContainerListViewComparer : IComparer
	{
		private CompareInfo _compareInfo;
		private ContainerListViewColumnHeader[] _sortColumns;
		private int[] _sortColumnIndices;

		/// <summary>
		/// Creates a new comparer for a <see cref="ContainerListView"/>, will use current sort settings and
		/// cache them locally.
		/// </summary>
		/// <param name="listView">The <see cref="ContainerListView"/> that this comparer is for.</param>
		public ContainerListViewComparer(ContainerListView listView)
		{
			_sortColumns = listView.SortColumns;
			_sortColumnIndices = new int[_sortColumns.Length];

			for(int idx = 0; idx < _sortColumns.Length; ++idx)
				_sortColumnIndices[idx] = _sortColumns[idx].Index;

			_compareInfo = CultureInfo.CurrentCulture.CompareInfo;
		}

		/// <summary>
		/// Compares the two incoming <see cref="ContainerListViewItem"/> elements.
		/// </summary>
		/// <param name="item1">The first item to compare.</param>
		/// <param name="item2">The second item to compare.</param>
		/// <returns>Zero if equal, -1 if item1 &lt; item2, 1 if item1 &gt; item2</returns>
		public int Compare(ContainerListViewItem item1, ContainerListViewItem item2)
		{
			for(int idx = 0; idx < _sortColumns.Length; ++idx)
			{
				// cache these values so as to make all of this a little bit faster
				ContainerListViewColumnHeader sortColumn = _sortColumns[idx];
				int sortColumnIndex = _sortColumnIndices[idx];
				SortOrder sortOrder = sortColumn.SortOrder;
				SortDataType sortDataType = sortColumn.SortDataType;

				if(item1 == null || item2 == null || sortOrder == SortOrder.None || sortDataType == SortDataType.None)
					return 0; // We don't know type, the order, or the data - so both objects are same for us

				int n = 0;

				ContainerListViewSubItem subItem1 = sortOrder == SortOrder.Ascending ? item1.SubItems[sortColumnIndex] : item2.SubItems[sortColumnIndex];
				ContainerListViewSubItem subItem2 = sortOrder == SortOrder.Ascending ? item2.SubItems[sortColumnIndex] : item1.SubItems[sortColumnIndex];

				if(sortDataType == SortDataType.Custom)
					n = sortColumn.CustomSortComparer.Compare(subItem1, subItem2);
				else
					n = CompareItems(subItem1.Text, subItem2.Text, sortDataType);

				if(n != 0)
					return n;
			}

			return 0;
		}

		private int CompareItems(string item1, string item2, SortDataType sortDataType)
		{
			if(item1.Length == 0)
			{
				if(item2.Length == 0)
					return 0;
				else
					return -1;
			}
			else if(item2.Length == 0)
				return 1;

			switch (sortDataType)
			{
				case SortDataType.Integer:
				{
					int n1, n2;

					if(TryParseInt32(item1, out n1) && TryParseInt32(item2, out n2))
						return n1.CompareTo(n2);
					else
						goto case SortDataType.String;
				}
				case SortDataType.Double:
				{
					double n1, n2;

					if(Double.TryParse(item1, NumberStyles.Float, null, out n1) && Double.TryParse(item2, NumberStyles.Float, null, out n2))
						return n1.CompareTo(n2);
					else
						goto case SortDataType.String;
				}
				case SortDataType.Date:
				{
					try
					{
						DateTime dt1 = DateTime.Parse(item1, null, DateTimeStyles.None);
						DateTime dt2 = DateTime.Parse(item2, null, DateTimeStyles.None);

						return DateTime.Compare(dt1, dt2);
					}
					catch(FormatException)
					{
						goto case SortDataType.String;
					}
				}
				case SortDataType.String:
					return _compareInfo.Compare(item1, item2, CompareOptions.None);
				default:
					return 0;
			}
		}

		private bool TryParseInt32(string s, out int result)
		{
			bool negative = false;
			result = 0;

			if(s.Length == 0)
				return false;

			int idx = 0;
			if(s[0] == '+')
				++idx;
			else if(s[0] == '-')
			{
				negative = true;
				++idx;
			}

			for(; idx < s.Length; ++idx)
			{
				char ch = s[idx];

				if(ch >= '0' && ch <= '9')
				{
					result *= 10;
					result += (ch - '0');
				}
				else
				{
					result = 0;
					return false;
				}
			}

			result *= (negative ? -1 : 1);
			return true;
		}

		int IComparer.Compare(object item1, object item2)
		{
			return Compare(item1 as ContainerListViewItem, item2 as ContainerListViewItem);
		}
	}
}
