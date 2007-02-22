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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DotNetLib.Windows.Forms
{
	/// <summary>
	/// Represents a single column header in a <see cref="ContainerListView"/> control.
	/// </summary>
	[
	DefaultProperty("Text"),
	DesignTimeVisible(false),
	ToolboxItem(false),
	TypeConverter("DotNetLib.Windows.Forms.ContainerListViewColumnHeaderConverter")
	]
	public class ContainerListViewColumnHeader : Component, ICloneable
	{
		#region Variables

		private ContainerListView _listView;

		private string _text = string.Empty;
		private Image _image = null;
		private ContentAlignment _contentAlign = ContentAlignment.MiddleLeft;
		private SolidBrush _foregroundBrush = SystemBrushes.ControlText as SolidBrush;
		private Font _font = null;

		private int _width = 90;
		private int _maxWidth = 0;
		private int _minWidth = 0;
		private bool _visible = true;

		private SortDataType _sortDataType = SortDataType.None;
		private SortOrder _defaultSortOrder = SortOrder.Ascending;
		private SortOrder _sortOrder = SortOrder.None;
		private IComparer _customSort = null;
		private string _toolTip = string.Empty;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewColumnHeader"/> class.
		/// </summary>
		public ContainerListViewColumnHeader()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewColumnHeader"/> class with the specified text.
		/// </summary>
		/// <param name="text">The text to use in the column header.</param>
		public ContainerListViewColumnHeader(string text)
		{
			_text = text;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewColumnHeader"/> class with the specified text and width.
		/// </summary>
		/// <param name="text">The text to use in the column header.</param>
		/// <param name="width">The initial width of the colun header.</param>
		public ContainerListViewColumnHeader(string text, int width)
		{
			_text = text;
			_width = width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewColumnHeader"/> class with the specified properties.
		/// </summary>
		/// <param name="text">The text to use in the column header.</param>
		/// <param name="width">The initial width of the colun header.</param>
		/// <param name="horizontalAlign">The horizontal alignment of the column header content, will default vertical alignment to middle</param>
		public ContainerListViewColumnHeader(string text, int width, HorizontalAlignment horizontalAlign)
		{
			_text = text;
			_width = width;

			switch(horizontalAlign)
			{
				case HorizontalAlignment.Left:
					_contentAlign = ContentAlignment.MiddleLeft;
					break;
				case HorizontalAlignment.Right:
					_contentAlign = ContentAlignment.MiddleRight;
					break;
				case HorizontalAlignment.Center:
					_contentAlign = ContentAlignment.MiddleCenter;
					break;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerListViewColumnHeader"/> class with the specified properties.
		/// </summary>
		/// <param name="text">The text to use in the column header.</param>
		/// <param name="width">The initial width of the colun header.</param>
		/// <param name="contentAlign">The content alignment of the column header content</param>
		public ContainerListViewColumnHeader(string text, int width, ContentAlignment contentAlign)
		{
			_text = text;
			_width = width;
			_contentAlign = contentAlign;
		}

		#endregion

		#region Properties

		#region Property Helpers

		private bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		#endregion

		#region Appearance

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Image"/> used to paint on the column header.
		/// </summary>
		[
		Category("Appearance"),
		Description("The image to display in this header."),
		DefaultValue(null)
		]
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				if(_image != value)
				{
					_image = value;

					if(_listView != null)
						_listView.ColumnInvalidated(this, false, false);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Color used to paint the column header text.
		/// </summary>
		[
		Category("Appearance"),
		Description("The color to use to paint the text of the column header."),
		DefaultValue(typeof(Color), "ControlText")
		]
		public Color ForeColor
		{
			get
			{
				return _foregroundBrush.Color;
			}
			set
			{
				if(_foregroundBrush.Color != value)
				{
					_foregroundBrush.Color = value;

					if(_listView != null)
						_listView.ColumnInvalidated(this, false, false);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Font used to paint the column header text.  If left null it will use the <see cref="ContainerListView"/>'s Font property.
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

					if(_listView != null)
						_listView.ColumnInvalidated(this, false, false);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text of the column header.
		/// </summary>
		[
		Category("Appearance"),
		Description("The title of this column header."),
		DefaultValue("")
		]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if(value == null)
					value = string.Empty;

				if(_text != value)
				{
					_text = value;

					if(_listView != null)
						_listView.ColumnInvalidated(this, false, false);
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignment of the content for the column header.
		/// </summary>
		[
		Category("Appearance"),
		Description("The alignment of the column headers content (text, image, sort icon)."),
		DefaultValue(ContentAlignment.MiddleLeft)
		]
		public ContentAlignment ContentAlign
		{
			get
			{
				return _contentAlign;
			}
			set
			{
				if(_contentAlign != value)
				{
					_contentAlign = value;

					if(_listView != null)
						_listView.ColumnInvalidated(this, false, true);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text of the tool tip for this column header.
		/// </summary>
		[
		Category("Appearance"),
		Description("The tooltip of the column"),
		DefaultValue("")
		]
		public string ToolTip
		{
			get
			{
				return _toolTip;
			}
			set
			{
				_toolTip = value + string.Empty;
			}
		}

		#endregion

		/// <summary>
		/// The width of the column header.
		/// </summary>
		[
		Category("Behavior"),
		Description("The width in pixels of this column header."),
		DefaultValue(90)
		]
		public int Width
		{
			get
			{
				return _width;
			}
			set 
			{
				if(value < _minWidth)
					value = _minWidth;

				if(_maxWidth != 0 && value > _maxWidth)
					value = _maxWidth;

				if(value != _width || value < 0)
				{
					if(value < 0)
						AutoSizeWidth(value == -1); // no need to invalidate, will call back in to this property
					else
					{
						_width = value; 

						if(_listView != null)
							_listView.ColumnInvalidated(this, true, true);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum width a column is allowed to be.
		/// </summary>
		[
		Category("Behavior"),
		Description("The minimum width in pixels this column is allowed to be."),
		DefaultValue(0)
		]
		public int MinimumWidth
		{
			get
			{
				return _minWidth;
			}
			set
			{
				_minWidth = value;

				if(_width < _minWidth)
					Width = _minWidth;
			}
		}

		/// <summary>
		/// Gets or sets the maximum width a column is allowed to be.
		/// </summary>
		[
		Category("Behavior"),
		Description("The maximum width in pixels this column is allowed to be. (0 = no max)"),
		DefaultValue(0)
		]
		public int MaximumWidth
		{
			get
			{
				return _maxWidth;
			}
			set
			{
				_maxWidth = value;

				if(_width > _maxWidth)
					Width = _maxWidth;
			}
		}

		/// <summary>
		/// Gets or sets a value to indicate whether or not the column is being drawn actively or not.  Is not affected by the scrolling area.
		/// </summary>
		[
		Category("Behavior"),
		Description("Determines whether the column is visible or hidden."),
		DefaultValue(true)
		]
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if(value != _visible)
				{
					_visible = value;

					if(_listView != null)
						_listView.ColumnInvalidated(this, true, true);
				}
			}
		}

		/// <summary>
		/// Gets or sets the data type used for comparisons when sorting this column.
		/// </summary>
		[
		Category("Sorting"),
		Description("What comparisons to use when sorting this column"),
		DefaultValue(SortDataType.None)
		]
		public SortDataType SortDataType
		{
			get
			{
				return _sortDataType;
			}
			set
			{
				_sortDataType = value;

				if(_sortDataType == SortDataType.None)
					_sortOrder = SortOrder.None;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="SortOrder"/> that is used as the default when this column is first sorted.
		/// </summary>
		[
		Category("Sorting"),
		Description("What sort order this column will use when first sorted"),
		DefaultValue(SortOrder.Ascending)
		]
		public SortOrder DefaultSortOrder
		{
			get
			{
				return _defaultSortOrder;
			}
			set
			{
				_defaultSortOrder = value;
			}
		}

		/// <summary>
		/// Gets or sets the custom IComparer, SortDataType must be set to <b>Custom</b> for this to be used
		/// </summary>
		[
		Browsable(false),
		DefaultValue(null)
		]
		public IComparer CustomSortComparer
		{
			get
			{
				return _customSort;
			}
			set
			{
				_customSort = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="ContainerListView"/> that this header is attached to.
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
		/// Gets the <see cref="SortOrder"/> that this column is current being sorted by.  If value is <b>SortOrder.None</b> then it currently isn't being sorted by.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(SortOrder.None)
		]
		public SortOrder SortOrder
		{
			get
			{
				return _sortOrder;
			}
		}

		/// <summary>
		/// Gets the index of this header in the containing <see cref="ContainerListViewColumnHeaderCollection"/>.
		/// </summary>
		[
		Browsable(false)
		]
		public int Index
		{
			get
			{
				if(_listView != null)
					return _listView.GetColumnIndex(this);
				else
					return -1;
			}
		}

		/// <summary>
		/// Gets or sets the display index of this header.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(0)
		]
		public int DisplayIndex
		{
			get
			{
				if(_listView != null)
					return _listView.GetColumnDisplayIndex(this);
				else
					return 0;
			}
			set
			{
				if(_listView != null)
					_listView.SetColumnDisplayIndex(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether or not this column is currently being tracked.
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
					return _listView.GetColumnHovered(this);
				else
					return false;
			}
			set
			{
				if(_listView != null)
					_listView.SetColumnHovered(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether or not this column is currently being pressed.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool Pressed
		{
			get
			{
				if(_listView != null)
					return _listView.GetColumnPressed(this);
				else
					return false;
			}
			set
			{
				if(_listView != null)
					_listView.SetColumnPressed(this, value);
			}
		}

		internal ContainerListView OwnerListView
		{
			set
			{
				if(_listView != value)
				{
					if(_listView != null) 
						_listView.ColumnInvalidated(null, true, true);

					_listView = value;

					if(value != null)
					{
						Width = _width; // ugly, but it's what I want to do
						_listView.ColumnInvalidated(this, true, true);
					}
				}
			}
		}

		internal SortOrder InternalSortOrder
		{
			set
			{
				_sortOrder = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Will automatically size the width of the column to fit the text
		/// </summary>
		/// <param name="includeItemWidths">If <b>true</b> will use the sizes the SubItems as well.  If <b>false</b> it will just use the text on the header.</param>
		public void AutoSizeWidth(bool includeItemWidths)
		{
			int mwid = 0;
			int twid = 0;

			if(includeItemWidths && _listView != null)
				mwid = AutoSizeItems(_listView.Items);

			twid = GetStringWidth(_text, Font);

			if(twid > mwid)
				mwid = twid;

			mwid += 5;

			Width = mwid;
		}

		private int AutoSizeItems(ContainerListViewItemCollection items)
		{
			int colIndex = this.Index;
			int colDisplayIndex = this.DisplayIndex;
			int twid = 0;
			int mwid = 0;
			int baseIndent = 0;

			if(colDisplayIndex == 0)
				if(_listView.ShowPlusMinus || (_listView.ShowTreeLines && _listView.ShowRootTreeLines))
					baseIndent += 16;

			for(int idx = 0; idx < items.Count; ++idx)
			{
				twid = 0;
				ContainerListViewItem item = items[idx];

				Font fnt = item.Font;
				if(fnt == null)
					fnt = Font;

				if(item.ImageIndex > -1 || item.SelectedImageIndex > -1)
					twid += 16;

				if(colDisplayIndex == 0)
					twid += baseIndent + (16 * (item.Depth - 1));

				twid += GetStringWidth(item.SubItems[colIndex].Text, fnt) + 10;

				twid += 5;
				if(twid > mwid)
					mwid = twid;

				if(item.HasChildren && item.Expanded)
				{
					twid = AutoSizeItems(item.Items);
					if(twid > mwid)
						mwid = twid;
				}
			}

			return mwid;
		}

		private int GetStringWidth(string text, Font fnt)
		{
			Graphics g = Graphics.FromImage(new Bitmap(32, 32));
			SizeF strSize = g.MeasureString(text, fnt);
			return (int)strSize.Width;
		}

		#endregion

		#region IClonable interface implementation

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates an identity copy of the current <see cref="ContainerListViewColumnHeader" /> that is not attached to any <see cref="ContainerListView"/>.
		/// </summary>
		/// <returns>An object representing a copy of this <see cref="ContainerListViewColumnHeader"/> object.</returns>
		public ContainerListViewColumnHeader Clone()
		{
			ContainerListViewColumnHeader ch = new ContainerListViewColumnHeader();

			ch._text = _text;
			ch._contentAlign = _contentAlign;
			ch._width = _width;
			ch._visible = _visible;
			ch._sortDataType = _sortDataType;
			ch._defaultSortOrder = _defaultSortOrder;
			ch._toolTip = _toolTip;

			if(_image != null)
				ch._image = _image.Clone() as Image;

			return ch;
		}

		#endregion
	}
}
