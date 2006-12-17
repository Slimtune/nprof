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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Represents a single item (row) in a <see cref="ContainerListView"/> control.
	/// </summary>
	[
	DefaultProperty("Text"),
	DesignTimeVisible(false),
	TypeConverter("DotNetLib.Windows.Forms.ContainerListViewItemConverter")
	]
	public class ContainerListViewItem : ICloneable
	{
		#region Variables

		private Color _backColor = Color.Empty;
		private Color _foreColor = Color.Empty;

		private Font _font = null;

		private int _imageIndex = -1;
		private int _selectedImageIndex = -1;

		private ContainerListView _listView;
		private ContainerListViewSubItemCollection _subItems;

		private ContainerListViewItem _parentItem;
		private ContainerListViewItem _previousItem;
		private ContainerListViewItem _nextItem;
		private ContainerListViewItemCollection _items;
		private ContainerListViewItemCollection _container;

		private int _y;
		private bool _expanded = false;
		private NullableBoolean _hasChildren = NullableBoolean.NotSet;
		private int _height = -1;

		private object _tag = null;

		private IFilter _filter = FilterStub.AllFilter;

		private Rectangle _glyph;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewItem"/> class.
		/// </summary>
		public ContainerListViewItem()
		{
			_subItems = new ContainerListViewSubItemCollection(this);
			_listView = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewItem"/> class with the specified text for the first sub item.
		/// </summary>
		/// <param name="text">The text you want to appear for the first sub item.</param>
		public ContainerListViewItem(string text) : this()
		{
			Text = text;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewItem"/> class with the specified text for the first sub item and the specified image index.
		/// </summary>
		/// <param name="text">The text you want to appear for the first sub item.</param>
		/// <param name="imageIndex">The index of the image you want displayed for the item.</param>
		public ContainerListViewItem(string text, int imageIndex) : this()
		{
			Text = text;
			_imageIndex = imageIndex;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewItem"/> class with the specified properties set.
		/// </summary>
		/// <param name="text">The text you want to appear for the first sub item.</param>
		/// <param name="imageIndex">The index of the image you want displayed for the item.</param>
		/// <param name="selectedImageIndex">The index of the image you want displayed for the item when it is in the selected state.</param>
		public ContainerListViewItem(string text, int imageIndex, int selectedImageIndex) : this()
		{
			Text = text;
			_imageIndex = imageIndex;
			_selectedImageIndex = selectedImageIndex;
		}

		#endregion

		#region Properties

		#region Appearance

		/// <summary>
		/// Gets or sets the background color for the item.
		/// </summary>
		[
		Category("Appearance"),
		Description("The color to use to paint the back color of the item."),
		DefaultValue(typeof(Color), "Empty")
		]
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				if(_backColor != value)
				{
					_backColor = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> used to paint the item's text.
		/// </summary>
		[
		Category("Appearance"),
		Description("The color to use to paint the text of the item."),
		DefaultValue(typeof(Color), "Empty")
		]
		public Color ForeColor
		{
			get
			{
				return _foreColor;
			}
			set
			{
				if(_foreColor != value)
				{
					_foreColor = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Font"/> used to paint the item's text.  If left null it will use the font associated with the <see cref="ContainerListView"/> control.
		/// </summary>
		[
		Category("Appearance"),
		Description("The font to use to draw the text of the column header."),
		AmbientValue(null)
		]
		public Font Font
		{
			get
			{
				if(_font == null)
				{
					if(_listView != null)
						return _listView.Font;
					else
						return Control.DefaultFont;
				}
				else
					return _font;
			}
			set
			{
				if(_font != value)
				{
					_font = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the text of the first sub item for this item.
		/// </summary>
		[
		Category("Appearance"),
		Description("The text of the first sub item of this item."),
		DefaultValue("")
		]
		public string Text
		{
			get
			{
				if(_subItems.Count > 0)
					return _subItems[0].Text;
				else
					return string.Empty;
			}
			set
			{
				if(_subItems.Count == 0 && _listView == null)
					_subItems.Add(new ContainerListViewSubItem(this));

				_subItems[0].Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of this item.  Set to -1 to use the <see cref="ContainerListView"/> DefaultItemHeight property.
		/// </summary>
		[
		Category("Appearance"),
		Description("The height of this item."),
		AmbientValue(-1)
		]
		public int Height
		{
			get
			{
				if(_height == -1)
				{
					if(_listView != null)
						return _listView.DefaultItemHeight;
					else
						return 17;
				}
				else
					return _height;
			}
			set
			{
				if(_height != value)
				{
					_height = value;
					Refresh();
				}
			}
		}

		#endregion

		#region Behavior

		/// <summary>
		/// Gets or sets the items expanded state.
		/// </summary>
		[
		Category("Behavior"),
		Description("Whether this item is currently expanded."),
		DefaultValue(false)
		]
		public bool Expanded
		{
			get
			{
				return _expanded;
			}
			set
			{
				if(value)
					Expand();
				else
					Collapse();
			}
		}

		/// <summary>
		/// Gets or sets the index into the image list for this item's image.
		/// </summary>
		[
		Category("Behavior"),
		Description("The index into the image list for this item's image."),
		DefaultValue(-1)
		]
		public int ImageIndex
		{
			get
			{
				return _imageIndex;
			}
			set
			{
				if(_imageIndex != value)
				{
					_imageIndex = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the index into the image list for this items' image while selected.
		/// </summary>
		[
		Category("Behavior"),
		Description("The index into the image list for this item's image when it is selected."),
		DefaultValue(-1)
		]
		public int SelectedImageIndex
		{
			get
			{
				return _selectedImageIndex;
			}
			set
			{
				if(_selectedImageIndex != value)
				{
					_selectedImageIndex = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets the collection of <see cref="ContainerListViewSubItem"/> this item is the owner of.  If this item is attached to a list view, this collection cannot be modified with add/insert/remove/clear.
		/// </summary>
		[
		Category("Behavior"),
		Description("The items collection of sub controls."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CollectionEditor), typeof(UITypeEditor))		 
		]
		public ContainerListViewSubItemCollection SubItems
		{
			get
			{
				return _subItems;
			}
		}

		#endregion

		/// <summary>
		/// Gets the collection of <see cref="ContainerListViewItem"/> this item is the parent of.
		/// </summary>
		[
		Category("Data"),
		Description("The child items contained in this item."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CollectionEditor), typeof(UITypeEditor))
		]
		public ContainerListViewItemCollection Items
		{
			get
			{
				if(_items == null)
				{
					_items = new ContainerListViewItemCollection(this);
					_items.InternalListView = _listView;
				}

				return _items;
			}
		}

		/// <summary>
		/// Gets or sets the user-defined data to associate with this item.
		/// </summary>
		[
		Category("Data"),
		Description("User defined data associated with the item.")
		]
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this control is currently part of the selected collection.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool Selected
		{
			get
			{
				if(_listView != null)
					return _listView.GetItemSelected(this);
				else
					return false;
			}
			set
			{
				if(_listView != null)
					_listView.SetItemSelected(this, value, true, true);
			}
		}

		/// <summary>
		/// Gets the previous visible item.
		/// </summary>
		/// <remarks>
		/// The <b>PreviousVisibleItem</b> can be a child, sibling, or a tree node from another branch.  If there is not previous tree node, the <b>PrevVisibleNode</b> property returns a null reference (<b>Nothing</b> in Visual Basic).
		/// </remarks>
		[
		Browsable(false)
		]
		public ContainerListViewItem PreviousVisibleItem
		{
			get
			{
				ContainerListViewItem item = PreviousItem; // go for a sibling first

				if(item == null) // no siblings, look to our parent(s)
					item = _parentItem;
				else if(item.Expanded && item.HasChildren) // sibling, but it has a child we want to select
					item = item.VeryLastItem;

				if(item != null && item.ParentItem != null)
				{
					if(_filter.Belongs(item))
						return item;
					else
						return item.PreviousVisibleItem;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the previous sibling tree node.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem PreviousItem
		{
			get
			{
				if(_container == null || _previousItem == null)
					return null;

				if(_filter.Belongs(_previousItem))
					return _previousItem;
				else
					return _previousItem.PreviousItem;
			}
		}

		/// <summary>
		/// Gets the next visible item.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem NextVisibleItem
		{
			get
			{
				ContainerListViewItem item = null;

				if(_expanded && _items != null && _items.Count > 0) // go for a child
					item = FirstItem;
				else // no children, try our siblings
					item = NextItem;

				if(item == null) // no visible children or siblings, look to our parent(s)
				{
					ContainerListViewItem currentParent = _parentItem;

					while(currentParent != null && item == null)
					{
						item = currentParent.NextItem;
						currentParent = currentParent.ParentItem;
					}
				}

				if(item != null && item.ParentItem != null)
				{
					if(_filter.Belongs(item))
						return item;
					else
						return item.NextVisibleItem;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the next sibling item.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem NextItem
		{
			get
			{
				if(_container == null || _nextItem == null)
					return null;

				if(_filter.Belongs(_nextItem))
					return _nextItem;
				else
					return _nextItem.NextItem;
			}
		}

		/// <summary>
		/// Gets the first child item.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem FirstItem
		{
			get
			{
				if(_items != null)
				{
					for(int idx = 0; idx < _items.Count; ++idx)
					{
						ContainerListViewItem item = _items[idx];
						if(_filter.Belongs(item))
							return item;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the last child item.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem LastItem
		{
			get
			{
				if(_items != null)
				{
					for(int idx = _items.Count - 1; idx >= 0; --idx)
					{
						ContainerListViewItem item = _items[idx];
						if(_filter.Belongs(item))
							return item;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the recursively last child item of its last child item.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem VeryLastItem
		{
			get
			{
				ContainerListViewItem item = LastItem;

				if(item != null)
				{
					if(item.Expanded)
						return item.VeryLastItem;
					else
						return item;
				}

				if(_parentItem != null)
					return this;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets or sets whether the mouse is currently hovering over this item.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool Hovered
		{
			get
			{
				if(_listView != null)
					return _listView.GetItemHovered(this);
				else
					return false;
			}
			set
			{
				if(_listView != null)
					_listView.SetItemHovered(this, value);
			}
		}

		/// <summary>
		/// Gets the virtual y-axis position of this item.  The first item in the control will always return zero (0).
		/// </summary>
		[
		Browsable(false)
		]
		public int Y
		{
			get
			{
				return _y;
			}
		}

		/// <summary>
		/// Gets the <see cref="ContainerListView"/> that this item is currently attached to.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListView ListView
		{
			get
			{
				return _listView;
			}
		}

		/// <summary>
		/// Gets the <see cref="ContainerListViewItem"/> that this item is currently a child to.
		/// </summary>
		[
		Browsable(false)
		]
		public ContainerListViewItem ParentItem
		{
			get
			{
				return _parentItem;
			}
		}

		/// <summary>
		/// Gets one-based depth of this item.  Root nodes return a one (1), their children return a two(2), and so on.
		/// </summary>
		[
		Browsable(false)
		]
		public int Depth
		{
			get
			{
				if(_parentItem == null)
					return 0;
				else
					return _parentItem.Depth + 1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item is currently being filtered out.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool IsFiltered
		{
			get
			{
				if(_filter.Belongs(this))
				{
					if(_parentItem != null)
						return _parentItem.IsFiltered;
					else
						return false;
				}
				else
					return true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this item has the focus.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool Focused
		{
			get
			{
				if(_listView != null)
					return _listView.GetItemFocused(this);
				else
					return false;
			}
			set
			{
				if(_listView != null)
					_listView.SetItemFocused(this, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item has children.  Use <see cref="ForceHasChildren"/> to hide children without having to remove them.
		/// </summary>
		public bool HasChildren
		{
			get
			{
				if(_hasChildren == NullableBoolean.NotSet)
					return (_items != null && _items.Count != 0);
				else if(_hasChildren == NullableBoolean.True)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets or sets a value to determine whether the control should acknowledge its children or not.
		/// </summary>
		public NullableBoolean ForceHasChildren
		{
			get
			{
				return _hasChildren;
			}
			set
			{
				if(_hasChildren != value)
				{
					_hasChildren = value;
					Refresh();
				}
			}
		}

		internal Rectangle Glyph
		{
			get
			{
				return _glyph;
			}
			set
			{
				_glyph = value;
			}
		}

		internal ContainerListView OwnerListView
		{
			set
			{
				_listView = value;

				if(_items != null)
					_items.InternalListView = value;

				if(_listView != null)
				{
					_subItems.AdjustSize(_listView.Columns.Count);

					if(!_listView.InUpdateTransaction)
						_listView.RecalculateItemPositions(PreviousVisibleItem);
				}
			}
		}

		internal ContainerListViewItem InternalParentItem
		{
			set
			{
				_parentItem = value;
				_container = _parentItem.Items;
			}
		}

		internal int InternalY
		{
			set
			{
				_y = value;
			}
		}

		internal bool InternalIsVisible
		{
			get
			{
				if(_parentItem == null)
					return true;
				else if(_parentItem.HasChildren && _parentItem.Expanded)
					return _parentItem.InternalIsVisible;
				else
					return false;
			}
		}

		internal ContainerListViewItem InternalPreviousItem
		{
			get
			{
				return _previousItem;
			}
			set
			{
				_previousItem = value;
			}
		}

		internal ContainerListViewItem InternalNextItem
		{
			get
			{
				return _nextItem;
			}
			set
			{
				_nextItem = value;
			}
		}

		#endregion

		#region Methods

		internal ContainerListViewItem GetItemAt(int y)
		{
			ContainerListViewItem lastItem = null;

			if(_items != null)
			{
				for(int idx = 0; idx < _items.Count; ++idx)
				{
					ContainerListViewItem item = _items[idx];

					if(y >= item.Y && y < (item.Y + item.Height) && item._filter.Belongs(item)) // base case if it is this node
						return item;

					if(y < item.Y && lastItem != null && lastItem.Expanded) // must be a child of the last item
						return lastItem.GetItemAt(y);

					lastItem = item;
				}

				if(lastItem != null && lastItem.Expanded)
					return lastItem.GetItemAt(y);
			}

			return null;
		}

		/// <summary>
		/// Expands the item to display any child items.
		/// </summary>
		public void Expand()
		{
			if(_expanded)
				return;


			if(_listView != null)
			{
				ContainerListViewCancelEventArgs e = new ContainerListViewCancelEventArgs(null, this);

				
				_listView.OnItemExpanding(e);

				if(e.Cancel)
					return;
			}

			_expanded = true;

			if(_items != null && _items.Count > 0)
				_listView.RecalculateItemPositions(this);
			else
				_listView.Invalidate();

			if(_listView != null)
				_listView.OnItemExpanded(new ContainerListViewEventArgs(null, this));
		}

		/// <summary>
		/// Collapses the item to hide any child items.
		/// </summary>
		public void Collapse()
		{
			if(!_expanded)
				return;

			if(_listView != null)
			{
				ContainerListViewCancelEventArgs e = new ContainerListViewCancelEventArgs(null, this);

				_listView.OnItemCollapsing(e);

				if(e.Cancel)
					return;
			}

			_expanded = false;

			if(_items != null && _items.Count > 0)
				_listView.RecalculateItemPositions(this);
			else
				_listView.Invalidate();

			if(_listView != null)
				_listView.OnItemCollapsed(new ContainerListViewEventArgs(null, this));
		}

		/// <summary>
		/// Sets the <see cref="IFilter"/> this item should use to determine whether it is drawn.
		/// </summary>
		/// <param name="filter">The <see cref="IFilter"/> that will determine the visibility.</param>
		/// <param name="recursive">Whether to distribute this <see cref="IFilter"/> to all of its children (recursively).</param>
		public void SetFilter(IFilter filter, bool recursive)
		{
			if(filter == null)
				filter = FilterStub.AllFilter;

			_filter = filter;

			if(recursive && _items != null)
				for(int idx = 0; idx < _items.Count; ++idx)
					_items[idx].SetFilter(_filter, recursive);

			_listView.RecalculateItemPositions(this);
		}

		public void Refresh()
		{
			if(_listView != null)
				_listView.Invalidate(); // TODO: optimize this call to simply invalidate this item (and items below it if necessary) will likely need another overload that indicates whether other items are affected (ala changing the height property)
		}

		internal void Refresh(ContainerListViewSubItem subItem)
		{
			if(_listView != null)
				_listView.Invalidate(); // TODO: optimize this call to simply invalidate this sub-item
		}

		/// <summary>
		/// Creates a copy of this <see cref="ContainerListViewItem"/> that is not attached to a list.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ContainerListViewItem lvi = new ContainerListViewItem();

			lvi._backColor = _backColor;
			lvi._font = _font;
			lvi._foreColor = _foreColor;
			lvi._imageIndex = _imageIndex;
			lvi._selectedImageIndex = _selectedImageIndex;
			lvi._listView = _listView;
			lvi._tag = _tag;

			for(int idx = 0; idx < _subItems.Count; ++idx)
				lvi._subItems[idx] = _subItems[idx].Clone();

			return lvi;
		}

		#endregion
	}
}
