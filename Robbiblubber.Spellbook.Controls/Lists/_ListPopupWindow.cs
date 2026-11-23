namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



/// <summary>This class implements the popup window that shows an item list (auto completion, rich combo box).</summary>
/// <typeparam name="T">Item type.</typeparam>
/// <remarks>The item list is owner-drawn and virtual: only visible rows are rendered. The scroll bar is drawn by the window itself, so
///          it follows the list colors and leaves room for the footer, which spans the full width. If the window is resizable, the
///          footer shows the item count and a size grip; the window can be resized at the grip and at its right and bottom edges.
///          Resizing is implemented with mouse capture instead of the system size loop, because the system size loop activates the
///          window and the edited control would lose the focus.</remarks>
[SupportedOSPlatform("windows")]
internal class _ListPopupWindow<T>: _NoActivateWindowBase where T: class, IListItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text padding in logical units.</summary>
	private const int _TEXT_PADDING = 4;

	/// <summary>Image padding in logical units.</summary>
	private const int _IMAGE_PADDING = 2;

	/// <summary>Spacing between item text and detail text in logical units.</summary>
	private const int _DETAIL_SPACING = 12;

	/// <summary>Width of the resize edges in logical units.</summary>
	private const int _RESIZE_EDGE = 4;

	/// <summary>Scroll bar width in logical units.</summary>
	private const int _SCROLLBAR_WIDTH = 12;

	/// <summary>Minimum scroll bar thumb height in logical units.</summary>
	private const int _MINIMUM_THUMB = 16;

	/// <summary>Delay before a held page scroll repeats, in milliseconds.</summary>
	private const int _REPEAT_DELAY = 400;

	/// <summary>Interval of a repeating page scroll, in milliseconds.</summary>
	private const int _REPEAT_INTERVAL = 60;

	/// <summary>Text format flags.</summary>
	/// <remarks><see cref="TextFormatFlags.PreserveGraphicsClipping"/> is required: without it, <see cref="TextRenderer"/> ignores the
	///          clip region of the graphics, and partially visible rows would be drawn into the footer.</remarks>
	private const TextFormatFlags _TEXT_FLAGS = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Displayed items.</summary>
	private IReadOnlyList<T> _Items = [];

	/// <summary>Filter pattern used for highlighting.</summary>
	private string _Pattern = string.Empty;

	/// <summary>Match mode used for highlighting.</summary>
	private ListMatchMode _MatchMode = ListMatchMode.PREFIX;

	/// <summary>Case sensitivity used for highlighting.</summary>
	private bool _CaseSensitive = false;

	/// <summary>Selected index.</summary>
	private int _SelectedIndex = -1;

	/// <summary>Index of the first visible row.</summary>
	private int _TopIndex = 0;

	/// <summary>Row height.</summary>
	private int _RowHeight = 18;

	/// <summary>Image size.</summary>
	private int _ImageSize = 16;

	/// <summary>Footer height, 0 if no footer is shown.</summary>
	private int _FooterHeight = 0;

	/// <summary>Determines if an image column is displayed.</summary>
	private bool _HasImages = false;

	/// <summary>Determines if the loading indicator is displayed.</summary>
	private bool _Loading = false;

	/// <summary>Determines if the window can be resized by the user.</summary>
	private bool _Resizable = false;

	/// <summary>Determines if the selection follows the mouse.</summary>
	private bool _HotTracking = false;

	/// <summary>Last mouse position processed by hot tracking.</summary>
	/// <remarks>Windows also sends mouse move messages when the window is shown or scrolled under a resting mouse; hot tracking only
	///          follows actual mouse movements, so the keyboard selection is not overwritten.</remarks>
	private Point _LastMouse = new(-1, -1);

	/// <summary>Accumulated mouse wheel delta that has not yet been scrolled (high-resolution wheels and touchpads).</summary>
	private int _WheelDelta = 0;

	/// <summary>Hit test code of the active resize operation, 0 if the window is not being resized.</summary>
	private int _ResizeMode = 0;

	/// <summary>Mouse position at the start of the resize operation, in screen coordinates.</summary>
	private Point _ResizeStart = Point.Empty;

	/// <summary>Window size at the start of the resize operation.</summary>
	private Size _ResizeStartSize = Size.Empty;

	/// <summary>Minimum window size.</summary>
	/// <remarks>The size limits are kept by the window itself instead of <see cref="Form.MinimumSize"/> and <see cref="Form.MaximumSize"/>,
	///          whose internal handling for top-level forms interferes with showing and hiding the popup.</remarks>
	private Size _MinimumWindowSize = Size.Empty;

	/// <summary>Maximum window size, empty for no limit.</summary>
	private Size _MaximumWindowSize = Size.Empty;

	/// <summary>Determines if the scroll bar thumb is being dragged.</summary>
	private bool _ThumbDragging = false;

	/// <summary>Vertical offset of the mouse within the thumb while dragging.</summary>
	private int _ThumbDragOffset = 0;

	/// <summary>Determines if the mouse is over the scroll bar.</summary>
	private bool _ScrollBarHot = false;

	/// <summary>Direction of a held page scroll: -1 up, 1 down, 0 none.</summary>
	private int _RepeatDirection = 0;

	/// <summary>Timer for held page scrolls.</summary>
	private readonly System.Windows.Forms.Timer _RepeatTimer = new();

	/// <summary>Selection back color.</summary>
	private Color _SelectionBackColor = SystemColors.Highlight;

	/// <summary>Selection fore color.</summary>
	private Color _SelectionForeColor = SystemColors.HighlightText;

	/// <summary>Detail fore color.</summary>
	private Color _DetailForeColor = SystemColors.GrayText;

	/// <summary>Highlight style.</summary>
	private ListHighlightStyle _HighlightStyle = ListHighlightStyle.BOLD;

	/// <summary>Highlight color.</summary>
	private Color _HighlightColor = SystemColors.HotTrack;

	/// <summary>Font cache.</summary>
	private readonly Dictionary<FontStyle, Font> _Fonts = new();

	/// <summary>Image list.</summary>
	private ImageList? _ImageList = null;

	/// <summary>Function that provides images for items without image.</summary>
	private Func<T, Image?>? _ImageResolver = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public _ListPopupWindow(): base()
	{
		_RepeatTimer.Tick += _RepeatTimer_Tick;
		_UpdateMetrics();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the left offset of the item text within a row.</summary>
	private int _TextLeft
	{
		get { return _GetTextLeft(_HasImages); }
	}


	/// <summary>Gets the height of the row area.</summary>
	private int _RowAreaHeight
	{
		get { return Math.Max(0, ClientSize.Height - _FooterHeight); }
	}


	/// <summary>Gets the width of the row area.</summary>
	private int _RowAreaWidth
	{
		get { return Math.Max(0, ClientSize.Width - (_ScrollBarVisible ? _ScrollBarWidth : 0)); }
	}


	/// <summary>Gets the scroll bar width.</summary>
	private int _ScrollBarWidth
	{
		get { return LogicalToDeviceUnits(_SCROLLBAR_WIDTH); }
	}


	/// <summary>Gets if the scroll bar is visible.</summary>
	private bool _ScrollBarVisible
	{
		get { return (_Items.Count > VisibleRowCount); }
	}


	/// <summary>Gets the scroll bar track bounds.</summary>
	private Rectangle _TrackBounds
	{
		get { return new Rectangle(ClientSize.Width - _ScrollBarWidth, 0, _ScrollBarWidth, _RowAreaHeight); }
	}


	/// <summary>Gets the scroll bar thumb bounds.</summary>
	private Rectangle _ThumbBounds
	{
		get
		{
			Rectangle track = _TrackBounds;
			int visible = VisibleRowCount;
			int range = Math.Max(1, _Items.Count - visible);
			int height = Math.Min(track.Height, Math.Max(LogicalToDeviceUnits(_MINIMUM_THUMB), (int) ((long) track.Height * visible / Math.Max(1, _Items.Count))));
			int top = track.Top + (int) ((long) (track.Height - height) * Math.Min(_TopIndex, range) / range);

			return new Rectangle(track.Left, top, track.Width, height);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws a text segment.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="text">Text.</param>
	/// <param name="font">Font.</param>
	/// <param name="x">Horizontal position.</param>
	/// <param name="bounds">Text bounds.</param>
	/// <param name="color">Text color.</param>
	/// <returns>Returns the horizontal position after the segment.</returns>
	private static int _DrawSegment(Graphics g, string text, Font font, int x, Rectangle bounds, Color color)
	{
		if((text.Length == 0) || (x >= bounds.Right)) { return x; }

		TextRenderer.DrawText(g, text, font, new Rectangle(x, bounds.Y, bounds.Right - x, bounds.Height), color, _TEXT_FLAGS);
		return x + TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), _TEXT_FLAGS).Width;
	}


	/// <summary>Blends two colors.</summary>
	/// <param name="a">First color.</param>
	/// <param name="b">Second color.</param>
	/// <param name="amount">Amount of the second color, between 0 and 1.</param>
	/// <returns>Returns the blended color.</returns>
	private static Color _Blend(Color a, Color b, float amount)
	{
		return Color.FromArgb((int) (a.R + ((b.R - a.R) * amount)), (int) (a.G + ((b.G - a.G) * amount)), (int) (a.B + ((b.B - a.B) * amount)));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the left offset of the item text within a row.</summary>
	/// <param name="hasImages">Determines if an image column is displayed.</param>
	/// <returns>Returns the offset.</returns>
	private int _GetTextLeft(bool hasImages)
	{
		return (hasImages ? (LogicalToDeviceUnits(_IMAGE_PADDING * 2) + _ImageSize) : 0) + LogicalToDeviceUnits(_TEXT_PADDING);
	}


	/// <summary>Gets a font for a font style.</summary>
	/// <param name="style">Font style.</param>
	/// <returns>Returns the font.</returns>
	private Font _GetFont(FontStyle style)
	{
		if(style == Font.Style) { return Font; }

		if(!_Fonts.TryGetValue(style, out Font? rval))
		{
			rval = new Font(Font, style);
			_Fonts.Add(style, rval);
		}

		return rval;
	}


	/// <summary>Gets the font for highlighted characters.</summary>
	/// <param name="style">Item font style.</param>
	/// <returns>Returns the font.</returns>
	private Font _GetHighlightFont(FontStyle style)
	{
		return _HighlightStyle switch
		{
			ListHighlightStyle.BOLD => _GetFont(style | (style.HasFlag(FontStyle.Bold) ? FontStyle.Underline : FontStyle.Bold)),
			ListHighlightStyle.UNDERLINE => _GetFont(style | FontStyle.Underline),
			_ => _GetFont(style)
		};
	}


	/// <summary>Disposes all cached fonts.</summary>
	private void _ClearFonts()
	{
		foreach(Font i in _Fonts.Values) { i.Dispose(); }
		_Fonts.Clear();
	}


	/// <summary>Updates row height, image size and footer height.</summary>
	private void _UpdateMetrics()
	{
		_RowHeight = Font.Height + LogicalToDeviceUnits(4);
		_ImageSize = Math.Min(_RowHeight - 2, LogicalToDeviceUnits(16));
		_FooterHeight = (_Resizable ? Math.Max(Font.Height + LogicalToDeviceUnits(2), LogicalToDeviceUnits(14)) : 0);
	}


	/// <summary>Limits a size to the minimum and maximum window size.</summary>
	/// <param name="size">Size.</param>
	/// <returns>Returns the limited size.</returns>
	private Size _LimitSize(Size size)
	{
		int width = Math.Max(size.Width, _MinimumWindowSize.Width);
		int height = Math.Max(size.Height, Math.Max(_MinimumWindowSize.Height, MinimumWindowHeight));

		if(_MaximumWindowSize.Width > 0) { width = Math.Min(width, Math.Max(_MaximumWindowSize.Width, _MinimumWindowSize.Width)); }
		if(_MaximumWindowSize.Height > 0) { height = Math.Min(height, Math.Max(_MaximumWindowSize.Height, MinimumWindowHeight)); }

		return new Size(width, height);
	}


	/// <summary>Sets the index of the first visible row.</summary>
	/// <param name="index">Index.</param>
	private void _SetTopIndex(int index)
	{
		index = Math.Max(0, Math.Min(index, _Items.Count - VisibleRowCount));
		if(index == _TopIndex) { return; }

		_TopIndex = index;
		Invalidate();
	}


	/// <summary>Scrolls the list so that an item is visible.</summary>
	/// <param name="index">Item index.</param>
	private void _EnsureVisible(int index)
	{
		if(index < _TopIndex)
		{
			_SetTopIndex(index);
		}
		else if(index >= (_TopIndex + VisibleRowCount))
		{
			_SetTopIndex(index - VisibleRowCount + 1);
		}
	}


	/// <summary>Scrolls the list so that the thumb is at a position.</summary>
	/// <param name="thumbTop">Top of the thumb in client coordinates.</param>
	private void _ScrollToThumb(int thumbTop)
	{
		Rectangle track = _TrackBounds;
		Rectangle thumb = _ThumbBounds;
		int free = track.Height - thumb.Height;
		int range = Math.Max(0, _Items.Count - VisibleRowCount);

		if(free <= 0) { return; }
		_SetTopIndex((int) Math.Round((double) (Math.Clamp(thumbTop, track.Top, track.Top + free) - track.Top) * range / free));
	}


	/// <summary>Scrolls one page towards the held mouse position.</summary>
	/// <returns>Returns <c>true</c> if the list has been scrolled, otherwise returns <c>false</c>.</returns>
	private bool _RepeatPage()
	{
		Point mouse = PointToClient(Cursor.Position);
		Rectangle thumb = _ThumbBounds;
		int page = Math.Max(1, VisibleRowCount - 1);

		if((_RepeatDirection < 0) && (mouse.Y < thumb.Top))
		{
			_SetTopIndex(_TopIndex - page);
			return true;
		}

		if((_RepeatDirection > 0) && (mouse.Y >= thumb.Bottom))
		{
			_SetTopIndex(_TopIndex + page);
			return true;
		}

		return false;
	}


	/// <summary>Ends dragging the thumb and held page scrolls.</summary>
	private void _EndScrolling()
	{
		_ThumbDragging = false;
		_RepeatDirection = 0;
		_RepeatTimer.Stop();
		Invalidate();
	}


	/// <summary>Gets the hit test result for the resize areas.</summary>
	/// <param name="screen">Mouse position in screen coordinates.</param>
	/// <returns>Returns the hit test result, or 0 if the position is not in a resize area.</returns>
	/// <remarks>Next to the scroll bar, only the window border is a resize area, so the scroll bar can be used over its full width.</remarks>
	private int _HitTestResize(Point screen)
	{
		Point client = PointToClient(screen);
		int x = screen.X - Left, y = screen.Y - Top;
		int edge = LogicalToDeviceUnits(_RESIZE_EDGE);
		int right = ((_ScrollBarVisible && (client.Y < _RowAreaHeight)) ? (Width - ClientSize.Width) : edge);
		Rectangle grip = new(ClientSize.Width - _FooterHeight, ClientSize.Height - _FooterHeight, _FooterHeight, _FooterHeight);

		if(grip.Contains(client) || ((x >= (Width - edge)) && (y >= (Height - edge)))) { return _HTBOTTOMRIGHT; }
		if(x >= (Width - right)) { return _HTRIGHT; }
		if(y >= (Height - edge)) { return _HTBOTTOM; }

		return 0;
	}


	/// <summary>Ends the active resize operation.</summary>
	private void _EndResize()
	{
		if(_ResizeMode == 0) { return; }

		_ResizeMode = 0;
		if(Capture) { Capture = false; }

		UserResized?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Raises the <see cref="SelectionChanged"/> event.</summary>
	private void _NotifySelection()
	{
		SelectionChanged?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Draws the item text with highlighted matches.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="item">Item.</param>
	/// <param name="bounds">Text bounds.</param>
	/// <param name="color">Text color.</param>
	/// <param name="selected">Determines if the item is selected.</param>
	private void _DrawItemText(Graphics g, T item, Rectangle bounds, Color color, bool selected)
	{
		Font regular = _GetFont(item.FontStyle);
		List<(int Start, int Length)> highlights = ((_HighlightStyle == ListHighlightStyle.NONE) ? new() : _ListMatcher._GetHighlights(item.Text, _Pattern, _MatchMode, _CaseSensitive));

		if(highlights.Count == 0)
		{
			TextRenderer.DrawText(g, item.Text, regular, bounds, color, _TEXT_FLAGS | TextFormatFlags.EndEllipsis);
			return;
		}

		Font highlighted = _GetHighlightFont(item.FontStyle);
		Color highlightColor = (((_HighlightStyle == ListHighlightStyle.COLOR) && (!selected)) ? _HighlightColor : color);
		GraphicsState state = g.Save();
		int x = bounds.X, pos = 0;

		g.SetClip(bounds, CombineMode.Intersect);

		foreach((int Start, int Length) i in highlights)
		{
			x = _DrawSegment(g, item.Text.Substring(pos, i.Start - pos), regular, x, bounds, color);
			x = _DrawSegment(g, item.Text.Substring(i.Start, i.Length), highlighted, x, bounds, highlightColor);
			pos = i.Start + i.Length;
		}
		_DrawSegment(g, item.Text.Substring(pos), regular, x, bounds, color);

		g.Restore(state);
	}


	/// <summary>Draws the item image.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="item">Item.</param>
	/// <param name="bounds">Image bounds.</param>
	private void _DrawImage(Graphics g, T item, Rectangle bounds)
	{
		if(item.Image is not null)
		{
			g.DrawImage(item.Image, bounds);
			return;
		}

		if((_ImageList is not null) && (!string.IsNullOrEmpty(item.ImageKey)))
		{
			int index = _ImageList.Images.IndexOfKey(item.ImageKey);
			if(index >= 0)
			{
				_ImageList.Draw(g, bounds.X, bounds.Y, bounds.Width, bounds.Height, index);
				return;
			}
		}

		Image? fallback = _ImageResolver?.Invoke(item);
		if(fallback is not null) { g.DrawImage(fallback, bounds); }
	}


	/// <summary>Draws a row.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="index">Item index.</param>
	/// <param name="bounds">Row bounds.</param>
	private void _DrawRow(Graphics g, int index, Rectangle bounds)
	{
		T item = _Items[index];
		bool selected = (index == _SelectedIndex);
		Color fore = (selected ? _SelectionForeColor : (item.ForeColor.IsEmpty ? ForeColor : item.ForeColor));
		int x = bounds.X + _TextLeft;
		int right = bounds.Right - LogicalToDeviceUnits(_TEXT_PADDING);

		if(selected)
		{
			using(SolidBrush brush = new(_SelectionBackColor)) { g.FillRectangle(brush, bounds); }
		}

		if(_HasImages)
		{
			int pad = LogicalToDeviceUnits(_IMAGE_PADDING);
			_DrawImage(g, item, new Rectangle(bounds.X + pad, bounds.Y + ((bounds.Height - _ImageSize) / 2), _ImageSize, _ImageSize));
		}

		if(!string.IsNullOrEmpty(item.Detail))
		{
			Font font = _GetFont(FontStyle.Regular);
			int width = TextRenderer.MeasureText(g, item.Detail, font, new Size(int.MaxValue, int.MaxValue), _TEXT_FLAGS).Width;
			int dx = Math.Max(x + ((right - x) / 2), right - width);

			TextRenderer.DrawText(g, item.Detail, font, new Rectangle(dx, bounds.Y, right - dx, bounds.Height), (selected ? _SelectionForeColor : _DetailForeColor), _TEXT_FLAGS | TextFormatFlags.EndEllipsis);
			right = dx - LogicalToDeviceUnits(_DETAIL_SPACING);
		}

		if(right > x) { _DrawItemText(g, item, new Rectangle(x, bounds.Y, right - x, bounds.Height), fore, selected); }
	}


	/// <summary>Draws the scroll bar.</summary>
	/// <param name="g">Graphics.</param>
	private void _DrawScrollBar(Graphics g)
	{
		Rectangle track = _TrackBounds;
		Rectangle thumb = _ThumbBounds;
		int inset = Math.Max(1, LogicalToDeviceUnits(3));
		Rectangle bar = new(thumb.Left + inset, thumb.Top + inset, Math.Max(1, thumb.Width - (inset * 2)), Math.Max(1, thumb.Height - (inset * 2)));
		Color thumbColor = _Blend(BackColor, ForeColor, ((_ThumbDragging || _ScrollBarHot) ? 0.55f : 0.3f));

		using(SolidBrush brush = new(_Blend(BackColor, ForeColor, 0.05f))) { g.FillRectangle(brush, track); }

		using(GraphicsPath path = new())
		using(SolidBrush brush = new(thumbColor))
		{
			int radius = Math.Min(bar.Width, bar.Height);

			path.AddArc(bar.Left, bar.Top, radius, radius, 180, 180);
			path.AddArc(bar.Left, bar.Bottom - radius, radius, radius, 0, 180);
			path.CloseFigure();

			SmoothingMode mode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillPath(brush, path);
			g.SmoothingMode = mode;
		}
	}


	/// <summary>Draws the footer.</summary>
	/// <param name="g">Graphics.</param>
	private void _DrawFooter(Graphics g)
	{
		Rectangle footer = new(0, ClientSize.Height - _FooterHeight, ClientSize.Width, _FooterHeight);
		Rectangle grip = new(footer.Right - _FooterHeight, footer.Top, _FooterHeight, _FooterHeight);
		int padding = LogicalToDeviceUnits(_TEXT_PADDING);

		using(SolidBrush brush = new(BackColor)) { g.FillRectangle(brush, footer); }
		using(Pen pen = new(_Blend(BackColor, ForeColor, 0.2f))) { g.DrawLine(pen, footer.Left, footer.Top, footer.Right, footer.Top); }

		if(!_Loading)
		{
			Rectangle text = new(footer.Left + padding, footer.Top + 1, Math.Max(0, grip.Left - footer.Left - padding), footer.Height - 1);
			TextRenderer.DrawText(g, ((_Items.Count == 1) ? "1 item" : $"{_Items.Count:N0} items"), _GetFont(FontStyle.Regular), text, _DetailForeColor, _TEXT_FLAGS | TextFormatFlags.EndEllipsis);
		}

		ControlPaint.DrawSizeGrip(g, BackColor, grip);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when an item has been clicked.</summary>
	public event EventHandler? ItemClicked;

	/// <summary>Occurs when an item has been double-clicked.</summary>
	public event EventHandler? ItemActivated;

	/// <summary>Occurs when the selection or the items may have changed.</summary>
	public event EventHandler? SelectionChanged;

	/// <summary>Occurs when the user has finished resizing the window.</summary>
	public event EventHandler? UserResized;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the selected index.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectedIndex
	{
		get { return _SelectedIndex; }
		set
		{
			int index = ((_Items.Count == 0) ? -1 : Math.Clamp(value, -1, _Items.Count - 1));
			if(index == _SelectedIndex) { return; }

			_SelectedIndex = index;
			if(index >= 0) { _EnsureVisible(index); }
			Invalidate();

			_NotifySelection();
		}
	}


	/// <summary>Gets the selected item.</summary>
	public T? SelectedItem
	{
		get { return ((_SelectedIndex >= 0) ? _Items[_SelectedIndex] : null); }
	}


	/// <summary>Gets if the loading indicator is displayed.</summary>
	public bool IsLoading
	{
		get { return _Loading; }
	}


	/// <summary>Gets the number of displayed items.</summary>
	public int ItemCount
	{
		get { return _Items.Count; }
	}


	/// <summary>Gets the number of fully visible rows.</summary>
	public int VisibleRowCount
	{
		get { return Math.Max(1, _RowAreaHeight / _RowHeight); }
	}


	/// <summary>Gets the minimum window height (one row and the footer).</summary>
	public int MinimumWindowHeight
	{
		get { return SizeFromClientSize(new Size(1, _RowHeight + _FooterHeight)).Height; }
	}


	/// <summary>Gets or sets if the selection follows the mouse.</summary>
	/// <remarks>Drop-down lists of combo boxes select the item under the mouse; the auto completion list does not, so moving the mouse
	///          over the list does not change the item that is inserted with the keyboard.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HotTracking
	{
		get { return _HotTracking; }
		set { _HotTracking = value; }
	}


	/// <summary>Gets or sets a function that provides images for items without image.</summary>
	/// <remarks>The function is called for visible items that have neither an image nor an image key found in the image list.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Func<T, Image?>? ImageResolver
	{
		get { return _ImageResolver; }
		set
		{
			_ImageResolver = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the text displayed while items are loading.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string LoadingText
	{
		get; set;
	} = "Loading...";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies font, colors and options.</summary>
	/// <param name="font">Font.</param>
	/// <param name="imageList">Image list.</param>
	/// <param name="style">Colors and highlight style.</param>
	/// <param name="resizable">Determines if the window can be resized by the user.</param>
	public void ApplyStyle(Font font, ImageList? imageList, _ListPopupStyle style, bool resizable)
	{
		if(!font.Equals(Font))
		{
			_ClearFonts();
			Font = font;
		}

		BackColor = style.BackColor;
		ForeColor = style.ForeColor;
		_SelectionBackColor = style.SelectionBackColor;
		_SelectionForeColor = style.SelectionForeColor;
		_DetailForeColor = style.DetailForeColor;
		_HighlightStyle = style.HighlightStyle;
		_HighlightColor = style.HighlightColor;
		BorderColor = style.BorderColor;

		_ImageList = imageList;
		_Resizable = resizable;
		_UpdateMetrics();
	}


	/// <summary>Sets the displayed items.</summary>
	/// <param name="items">Items.</param>
	/// <param name="hasImages">Determines if an image column is displayed.</param>
	/// <param name="pattern">Filter pattern.</param>
	/// <param name="matchMode">Match mode.</param>
	/// <param name="caseSensitive">Determines if matching is case-sensitive.</param>
	/// <param name="selectedIndex">Selected index.</param>
	public void SetItems(IReadOnlyList<T> items, bool hasImages, string pattern, ListMatchMode matchMode, bool caseSensitive, int selectedIndex)
	{
		_EndScrolling();

		_Items = items;
		_HasImages = hasImages;
		_Pattern = pattern;
		_MatchMode = matchMode;
		_CaseSensitive = caseSensitive;
		_Loading = false;
		_TopIndex = 0;
		_SelectedIndex = ((items.Count == 0) ? -1 : Math.Clamp(selectedIndex, -1, items.Count - 1));

		if(_SelectedIndex >= 0) { _EnsureVisible(_SelectedIndex); }
		Invalidate();

		_NotifySelection();
	}


	/// <summary>Clears the items and shows the loading indicator.</summary>
	public void SetLoading()
	{
		SetItems([], _HasImages, string.Empty, _MatchMode, _CaseSensitive, -1);
		_Loading = true;
	}


	/// <summary>Moves the selection.</summary>
	/// <param name="delta">Number of rows to move.</param>
	public void MoveSelection(int delta)
	{
		if(_Items.Count == 0) { return; }
		SelectedIndex = Math.Clamp(_SelectedIndex + delta, 0, _Items.Count - 1);
	}


	/// <summary>Sets the minimum and maximum window size.</summary>
	/// <param name="minimum">Minimum size.</param>
	/// <param name="maximum">Maximum size, empty for no limit.</param>
	public void SetSizeLimits(Size minimum, Size maximum)
	{
		_MinimumWindowSize = minimum;
		_MaximumWindowSize = maximum;
	}


	/// <summary>Gets the window height needed to display a number of rows.</summary>
	/// <param name="rows">Number of rows.</param>
	/// <returns>Returns the window height, including border and footer.</returns>
	public int GetWindowHeight(int rows)
	{
		return SizeFromClientSize(new Size(1, (Math.Max(1, rows) * _RowHeight) + _FooterHeight)).Height;
	}


	/// <summary>Gets the horizontal distance between the window edge and the item text.</summary>
	/// <param name="hasImages">Determines if an image column is displayed.</param>
	/// <returns>Returns the text offset.</returns>
	public int GetTextOffset(bool hasImages)
	{
		return _GetTextLeft(hasImages) + SystemInformation.BorderSize.Width;
	}


	/// <summary>Measures the window width needed to display items without truncation.</summary>
	/// <param name="items">Items.</param>
	/// <param name="hasImages">Determines if an image column is displayed.</param>
	/// <param name="sampleSize">Maximum number of items to measure.</param>
	/// <returns>Returns the window width, including border and scroll bar.</returns>
	/// <remarks>Only the first <paramref name="sampleSize"/> items are measured, so large lists do not slow down the display.</remarks>
	public int MeasurePreferredWidth(IReadOnlyList<T> items, bool hasImages, int sampleSize)
	{
		Size unlimited = new(int.MaxValue, int.MaxValue);
		Font regular = _GetFont(FontStyle.Regular);
		int content = 0;
		int count = Math.Min(items.Count, sampleSize);

		for(int i = 0; i < count; i++)
		{
			T item = items[i];
			int width = TextRenderer.MeasureText(item.Text, _GetHighlightFont(item.FontStyle), unlimited, _TEXT_FLAGS).Width;

			if(!string.IsNullOrEmpty(item.Detail)) { width += LogicalToDeviceUnits(_DETAIL_SPACING) + TextRenderer.MeasureText(item.Detail, regular, unlimited, _TEXT_FLAGS).Width; }
			content = Math.Max(content, width);
		}

		return SizeFromClientSize(new Size(_GetTextLeft(hasImages) + content + LogicalToDeviceUnits(_TEXT_PADDING) + _ScrollBarWidth, 1)).Width;
	}


	/// <summary>Shows the window at an anchor position.</summary>
	/// <param name="anchor">Screen position of the first character of the completed text.</param>
	/// <param name="lineHeight">Line height.</param>
	/// <param name="size">Window width and maximum window height.</param>
	/// <param name="owner">Owner form.</param>
	/// <remarks>The window height is reduced to the number of items, but not below the minimum size (see <see cref="SetSizeLimits"/>).</remarks>
	public void ShowAt(Point anchor, int lineHeight, Size size, Form? owner)
	{
		int chrome = SizeFromClientSize(Size.Empty).Height;
		int rows = Math.Max(1, Math.Min(_Items.Count, (size.Height - chrome - _FooterHeight) / _RowHeight));
		Size limited = _LimitSize(new Size(size.Width, SizeFromClientSize(new Size(size.Width, (rows * _RowHeight) + _FooterHeight)).Height));
		int width = limited.Width, height = limited.Height;
		Rectangle area = Screen.FromPoint(anchor).WorkingArea;
		int x = anchor.X - GetTextOffset(_HasImages);
		int y = anchor.Y + lineHeight;

		if(((y + height) > area.Bottom) && ((anchor.Y - height) >= area.Top)) { y = anchor.Y - height; }
		x = Math.Max(area.Left, Math.Min(x, area.Right - width));

		ShowNoActivate(new Rectangle(x, y, width, height), owner);
		_LastMouse = PointToClient(Cursor.Position);

		if(_SelectedIndex >= 0) { _EnsureVisible(_SelectedIndex); }
		Invalidate();
	}


	/// <summary>Scrolls the list by a mouse wheel delta.</summary>
	/// <param name="delta">Mouse wheel delta, a multiple of 120 for standard wheels.</param>
	public void ScrollByWheel(int delta)
	{
		int lines = SystemInformation.MouseWheelScrollLines;
		int notches;

		if(lines <= 0) { lines = VisibleRowCount; }

		_WheelDelta += delta;
		notches = _WheelDelta / 120;
		_WheelDelta -= notches * 120;

		if(notches != 0) { _SetTopIndex(_TopIndex - (notches * lines)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Tick event of the repeat timer.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _RepeatTimer_Tick(object? sender, EventArgs e)
	{
		_RepeatTimer.Interval = _REPEAT_INTERVAL;
		if(!_RepeatPage()) { _RepeatTimer.Stop(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if((m.Msg == _WM_NCLBUTTONDOWN) && _Resizable)
		{
			int hit = (int) m.WParam.ToInt64();

			if((hit == _HTRIGHT) || (hit == _HTBOTTOM) || (hit == _HTBOTTOMRIGHT))
			{
				_ResizeMode = hit;
				_ResizeStart = Cursor.Position;
				_ResizeStartSize = Size;
				Capture = true;

				m.Result = IntPtr.Zero;
				return;
			}
		}

		if((m.Msg == _WM_NCHITTEST) && _Resizable && (_ResizeMode == 0) && (!_ThumbDragging))
		{
			long lParam = m.LParam.ToInt64();
			int hit = _HitTestResize(new Point(unchecked((short) (lParam & 0xFFFF)), unchecked((short) ((lParam >> 16) & 0xFFFF))));

			if(hit != 0)
			{
				m.Result = (IntPtr) hit;
				return;
			}
		}

		base.WndProc(ref m);
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_RepeatTimer.Tick -= _RepeatTimer_Tick;
			_RepeatTimer.Dispose();
			_ClearFonts();
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		int width = _RowAreaWidth;
		int rowArea = _RowAreaHeight;

		g.Clear(BackColor);

		if(_Loading && (_Items.Count == 0))
		{
			Rectangle r = new(LogicalToDeviceUnits(_TEXT_PADDING), 0, width - LogicalToDeviceUnits(_TEXT_PADDING * 2), _RowHeight);
			TextRenderer.DrawText(g, LoadingText, _GetFont(FontStyle.Italic), r, _DetailForeColor, _TEXT_FLAGS | TextFormatFlags.EndEllipsis);
		}
		else
		{
			GraphicsState state = g.Save();
			g.SetClip(new Rectangle(0, 0, width, rowArea), CombineMode.Intersect);

			for(int i = _TopIndex; i < _Items.Count; i++)
			{
				Rectangle row = new(0, (i - _TopIndex) * _RowHeight, width, _RowHeight);

				if(row.Top >= rowArea) { break; }
				if(row.IntersectsWith(e.ClipRectangle)) { _DrawRow(g, i, row); }
			}

			g.Restore(state);

			if(_ScrollBarVisible) { _DrawScrollBar(g); }
		}

		if(_FooterHeight > 0) { _DrawFooter(g); }
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);

		if((e.Button != MouseButtons.Left) || (e.Y >= _RowAreaHeight)) { return; }

		if(_ScrollBarVisible && _TrackBounds.Contains(e.Location))
		{
			Rectangle thumb = _ThumbBounds;

			Capture = true;

			if(thumb.Contains(e.Location))
			{
				_ThumbDragging = true;
				_ThumbDragOffset = e.Y - thumb.Top;
				Invalidate();
			}
			else
			{
				_RepeatDirection = ((e.Y < thumb.Top) ? -1 : 1);
				_RepeatPage();
				_RepeatTimer.Interval = _REPEAT_DELAY;
				_RepeatTimer.Start();
			}
			return;
		}

		int index = _TopIndex + (e.Y / _RowHeight);
		if(index < _Items.Count)
		{
			SelectedIndex = index;
			ItemClicked?.Invoke(this, EventArgs.Empty);
		}
	}


	/// <inheritdoc/>
	/// <remarks>While the window is being resized, the size follows the mouse. While the thumb is dragged, the list follows the mouse.</remarks>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);

		if(_ResizeMode != 0)
		{
			Point position = Cursor.Position;
			int width = _ResizeStartSize.Width + ((_ResizeMode != _HTBOTTOM) ? (position.X - _ResizeStart.X) : 0);
			int height = _ResizeStartSize.Height + ((_ResizeMode != _HTRIGHT) ? (position.Y - _ResizeStart.Y) : 0);

			Cursor.Current = _ResizeMode switch
			{
				_HTRIGHT => Cursors.SizeWE,
				_HTBOTTOM => Cursors.SizeNS,
				_ => Cursors.SizeNWSE
			};

			Size = _LimitSize(new Size(width, height));
			return;
		}

		if(_ThumbDragging)
		{
			_ScrollToThumb(e.Y - _ThumbDragOffset);
			return;
		}

		bool hot = (_ScrollBarVisible && _TrackBounds.Contains(e.Location));
		if(hot != _ScrollBarHot)
		{
			_ScrollBarHot = hot;
			Invalidate(_TrackBounds);
		}

		if(_HotTracking && (e.Location != _LastMouse) && (!hot) && (e.X >= 0) && (e.X < _RowAreaWidth) && (e.Y >= 0) && (e.Y < (VisibleRowCount * _RowHeight)))
		{
			int index = _TopIndex + (e.Y / _RowHeight);
			if(index < _Items.Count) { SelectedIndex = index; }
		}
		_LastMouse = e.Location;
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);

		if(_ScrollBarHot)
		{
			_ScrollBarHot = false;
			Invalidate(_TrackBounds);
		}
	}


	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);

		if(_ThumbDragging || (_RepeatDirection != 0))
		{
			_EndScrolling();
			if(Capture) { Capture = false; }
		}

		_EndResize();
	}


	/// <inheritdoc/>
	protected override void OnMouseCaptureChanged(EventArgs e)
	{
		base.OnMouseCaptureChanged(e);

		if(!Capture)
		{
			if(_ThumbDragging || (_RepeatDirection != 0)) { _EndScrolling(); }
			_EndResize();
		}
	}


	/// <inheritdoc/>
	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		base.OnMouseDoubleClick(e);

		if((e.Button == MouseButtons.Left) && (e.Y < _RowAreaHeight) && (e.X < _RowAreaWidth) && (_SelectedIndex >= 0)) { ItemActivated?.Invoke(this, EventArgs.Empty); }
	}


	/// <inheritdoc/>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		ScrollByWheel(e.Delta);
	}


	/// <inheritdoc/>
	protected override void OnClientSizeChanged(EventArgs e)
	{
		base.OnClientSizeChanged(e);

		_SetTopIndex(_TopIndex);
		if(_SelectedIndex >= 0) { _EnsureVisible(_SelectedIndex); }
		Invalidate();
	}
}
