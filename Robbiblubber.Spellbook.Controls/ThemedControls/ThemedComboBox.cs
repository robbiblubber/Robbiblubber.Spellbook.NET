namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;

using Slot = Robbiblubber.Spellbook.Controls.ThemedComboBoxColors._Slot;



/// <summary>This class implements a self-drawn combo box with images, detail texts and customizable colors.</summary>
/// <remarks><para>The control shows the selected item and opens a list of all <see cref="Items"/> with the button, a click, F4 or
///          Alt+Down. Items that implement <see cref="IListItem"/> (for example <see cref="ListItem"/>) are displayed with image,
///          detail text, font style and color; other objects are displayed with their text.</para>
///          <para>All colors are defined by the color table <see cref="Colors"/>, which takes its colors from the theme of the
///          application or its own theme. The control can be read-only: it keeps the focus and its text can be copied, but the
///          selection cannot be changed.</para>
///          <para>In list mode (<see cref="DropDownStyle"/>), typing selects the next item that starts with the typed text. In combo
///          mode, the user types into a text box; typing searches the items or a <see cref="SearchSource"/> and shows the results in
///          the list.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(ThemedComboBox), "Resources.themedcombobox.bmp")]
[DefaultEvent(nameof(SelectedIndexChanged))][DefaultProperty(nameof(Items))]
public partial class ThemedComboBox: Control, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Horizontal text padding in logical units.</summary>
	private const int _TEXT_PADDING = 4;

	/// <summary>Image padding in logical units.</summary>
	/// <remarks>The values match the drop-down list, so the text of the control is aligned with the text of the list.</remarks>
	private const int _IMAGE_PADDING = 2;

	/// <summary>Image size in logical units.</summary>
	private const int _IMAGE_SIZE = 16;

	/// <summary>Vertical padding in logical units.</summary>
	private const int _VERTICAL_PADDING = 3;

	/// <summary>Width of the drop-down button in logical units.</summary>
	private const int _BUTTON_WIDTH = 20;

	/// <summary>Half width of the drop-down arrow in logical units.</summary>
	private const int _ARROW_SIZE = 4;

	/// <summary>Height of the focus line in logical units.</summary>
	private const int _FOCUS_LINE = 2;

	/// <summary>Spacing between text and detail text in logical units.</summary>
	private const int _DETAIL_SPACING = 12;

	/// <summary>Time in milliseconds after which typed characters start a new search.</summary>
	private const int _SEARCH_TIMEOUT = 1000;

	/// <summary>Default focus style.</summary>
	private const ThemedComboBoxFocusStyle _DEFAULT_FOCUS_STYLE = ThemedComboBoxFocusStyle.BORDER;

	/// <summary>Distance between the dotted focus rectangle and the border in logical units.</summary>
	private const int _FOCUS_INSET = 2;

	/// <summary>Default maximum number of items displayed in the drop-down list.</summary>
	private const int _DEFAULT_MAX_DROPDOWN_ITEMS = 12;

	/// <summary>Text format flags.</summary>
	private const TextFormatFlags _TEXT_FLAGS = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Items.</summary>
	private readonly ObjectCollection _Items;

	/// <summary>Color table.</summary>
	private readonly ThemedComboBoxColors _Colors = new();

	/// <summary>Displayed list items, NULL if they have to be rebuilt.</summary>
	private List<IListItem>? _Views = null;

	/// <summary>Determines if any item has an image, NULL if this has to be determined.</summary>
	private bool? _HasImages = null;

	/// <summary>Selected index.</summary>
	private int _SelectedIndex = -1;

	/// <summary>Selected item.</summary>
	private object? _SelectedItem = null;

	/// <summary>Selection version, incremented by every selection change.</summary>
	/// <remarks>Used to stop raising the events of a selection change when an event handler has changed the selection again.</remarks>
	private int _SelectionVersion = 0;

	/// <summary>Drop-down list has been closed, but the DropDownClosed event has not been raised yet.</summary>
	private bool _ClosedPending = false;

	/// <summary>Image list.</summary>
	private ImageList? _ImageList = null;

	/// <summary>Determines if the control is read-only.</summary>
	private bool _ReadOnly = false;

	/// <summary>Determines if the detail text of the selected item is displayed in the control.</summary>
	private bool _ShowDetailInDisplay = false;

	/// <summary>Focus style.</summary>
	private ThemedComboBoxFocusStyle _FocusStyle = _DEFAULT_FOCUS_STYLE;

	/// <summary>Minimum width of the drop-down list.</summary>
	private int _DropDownWidth = 0;

	/// <summary>Maximum number of items displayed in the drop-down list.</summary>
	private int _MaxDropDownItems = _DEFAULT_MAX_DROPDOWN_ITEMS;

	/// <summary>Drop-down list.</summary>
	private _ListPopupWindow<IListItem>? _Popup = null;

	/// <summary>Determines if the drop-down list is open.</summary>
	private bool _DroppedDown = false;

	/// <summary>Message filter that closes the drop-down list on clicks outside.</summary>
	private _DropDownFilter? _Filter = null;

	/// <summary>Form whose events are handled while the drop-down list is open.</summary>
	private Form? _HookedForm = null;

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;

	/// <summary>Determines if the mouse is over the drop-down button.</summary>
	private bool _ButtonHot = false;

	/// <summary>Accumulated mouse wheel delta.</summary>
	private int _WheelDelta = 0;

	/// <summary>Typed search text.</summary>
	private string _SearchText = string.Empty;

	/// <summary>Time of the last typed character.</summary>
	private long _SearchTime = 0;

	/// <summary>Font cache.</summary>
	private readonly Dictionary<FontStyle, Font> _Fonts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedComboBox(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);

		_Items = new ObjectCollection(this);
		_Colors.Changed += _Colors_Changed;

		TabStop = true;
		_UpdateHeight();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control height determined by the font.</summary>
	private int _PreferredHeight
	{
		get { return Math.Max(Font.Height, LogicalToDeviceUnits(_IMAGE_SIZE)) + (LogicalToDeviceUnits(_VERTICAL_PADDING) * 2) + 2; }
	}


	/// <summary>Gets the bounds of the drop-down button.</summary>
	private Rectangle _ButtonBounds
	{
		get
		{
			int width = Math.Min(LogicalToDeviceUnits(_BUTTON_WIDTH), Math.Max(0, Width - 2));
			return new Rectangle(Width - 1 - width, 1, width, Math.Max(0, Height - 2));
		}
	}


	/// <summary>Gets the image size.</summary>
	private int _ImageSize
	{
		get { return Math.Min(LogicalToDeviceUnits(_IMAGE_SIZE), Math.Max(1, Height - 4)); }
	}


	/// <summary>Gets if the drop-down list can be opened.</summary>
	private bool _CanDropDown
	{
		get { return ((!_ReadOnly) && Enabled && Visible && IsHandleCreated && (!IsDisposed)); }
	}


	/// <summary>Gets if an image column is displayed.</summary>
	private bool _ShowsImages
	{
		get
		{
			if(_HasImages is null)
			{
				_HasImages = _GetViews().Any(m => _HasImage(m));
			}

			return _HasImages.Value;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds the next item whose filter text starts with a text.</summary>
	/// <param name="items">Items.</param>
	/// <param name="text">Text.</param>
	/// <param name="start">Index to start the search at. The search wraps around at the end of the list.</param>
	/// <returns>Returns the index of the item, or -1 if no item matches.</returns>
	internal static int _FindPrefix(IReadOnlyList<IListItem> items, string text, int start)
	{
		if((items.Count == 0) || (text.Length == 0)) { return -1; }

		start = ((start % items.Count) + items.Count) % items.Count;

		for(int i = 0; i < items.Count; i++)
		{
			int index = (start + i) % items.Count;
			if((items[index].FilterText ?? items[index].Text).StartsWith(text, StringComparison.CurrentCultureIgnoreCase)) { return index; }
		}

		return -1;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws an image grayed.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="image">Image.</param>
	/// <param name="bounds">Bounds.</param>
	private static void _DrawDisabledImage(Graphics g, Image image, Rectangle bounds)
	{
		using ImageAttributes attributes = new();
		ColorMatrix matrix = new(new float[][]
		{
			[0.3f, 0.3f, 0.3f, 0, 0],
			[0.59f, 0.59f, 0.59f, 0, 0],
			[0.11f, 0.11f, 0.11f, 0, 0],
			[0, 0, 0, 0.5f, 0],
			[0, 0, 0, 0, 1]
		});

		attributes.SetColorMatrix(matrix);
		g.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the list items for all items.</summary>
	/// <returns>Returns the list items.</returns>
	private List<IListItem> _GetViews()
	{
		if(_Views is null)
		{
			_Views = new List<IListItem>(_Items.Count);
			foreach(object i in _Items) { _Views.Add(_CreateView(i)); }
		}

		return _Views;
	}


	/// <summary>Creates the list item for an item.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the list item.</returns>
	private IListItem _CreateView(object item)
	{
		if(item is IListItem rval) { return rval; }

		object? image = _GetMemberValue(item, _ImageMember);
		return new _ObjectListItem(item, GetItemText(item), Convert.ToString(_GetMemberValue(item, _DetailMember), CultureInfo.CurrentCulture), (image as Image), (image as string));
	}


	/// <summary>Gets the list item for the selected item.</summary>
	/// <returns>Returns the list item, or NULL if no item is selected.</returns>
	private IListItem? _GetSelectedView()
	{
		List<IListItem> views = _GetViews();

		if((_SelectedIndex >= 0) && (_SelectedIndex < views.Count)) { return views[_SelectedIndex]; }
		if(_SelectedItem is not null) { return _CreateView(_SelectedItem); }

		return null;
	}


	/// <summary>Gets if a list item has an image.</summary>
	/// <param name="item">List item.</param>
	/// <returns>Returns <c>true</c> if the item has an image, otherwise returns <c>false</c>.</returns>
	private bool _HasImage(IListItem item)
	{
		return ((item.Image is not null) || ((_ImageList is not null) && (!string.IsNullOrEmpty(item.ImageKey)) && _ImageList.Images.ContainsKey(item.ImageKey)));
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


	/// <summary>Disposes all cached fonts.</summary>
	private void _ClearFonts()
	{
		foreach(Font i in _Fonts.Values) { i.Dispose(); }
		_Fonts.Clear();
	}


	/// <summary>Adjusts the control height to the font.</summary>
	private void _UpdateHeight()
	{
		if(Height != _PreferredHeight) { Height = _PreferredHeight; }
	}


	/// <summary>Sets the selection.</summary>
	/// <param name="index">Selected index.</param>
	/// <param name="item">Selected item.</param>
	/// <param name="user">Determines if the user has changed the selection.</param>
	/// <param name="updateText">Determines if the text is set to the text of the item. In combo mode, free text keeps the text.</param>
	private void _SetSelection(int index, object? item, bool user, bool updateText = true)
	{
		bool indexChanged = (index != _SelectedIndex);
		bool itemChanged = (!Equals(item, _SelectedItem));

		// only a real change starts a new selection; a call that changes nothing must not cancel the events of an outer call
		int version = (indexChanged || itemChanged) ? ++_SelectionVersion : _SelectionVersion;

		_SelectedIndex = index;
		_SelectedItem = item;

		if(updateText) { _UpdateText(); }
		_InvalidateTextItem();
		Invalidate();

		if(indexChanged || itemChanged) { AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1); }

		_SyncPosition();
		if(version != _SelectionVersion) { return; }

		if(indexChanged)
		{
			_OnSelectedIndexChanged(EventArgs.Empty);
			if(version != _SelectionVersion) { return; }
		}
		if(itemChanged)
		{
			_OnSelectedItemChanged(EventArgs.Empty);
			if(version != _SelectionVersion) { return; }

			_OnSelectedValueChanged(EventArgs.Empty);
			if(version != _SelectionVersion) { return; }
		}
		if(user && (indexChanged || itemChanged)) { _OnSelectionChangeCommitted(EventArgs.Empty); }
	}


	/// <summary>Updates the control text to the text of the selected item.</summary>
	private void _UpdateText()
	{
		string text = GetItemText(_SelectedItem);

		if(_Editor is not null)
		{
			_SetEditorText(text);
		}
		else if(base.Text != text) { base.Text = text; }
	}


	/// <summary>Gets a color of the color table.</summary>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the color.</returns>
	private Color _Color(Slot slot)
	{
		return _Colors._Get(slot);
	}


	/// <summary>Gets the style of the drop-down list.</summary>
	/// <returns>Returns the style.</returns>
	private _ListPopupStyle _GetPopupStyle()
	{
		return new _ListPopupStyle()
		{
			BackColor = _Color(Slot.DROP_DOWN_BACK_COLOR),
			ForeColor = _Color(Slot.DROP_DOWN_FORE_COLOR),
			SelectionBackColor = _Color(Slot.SELECTION_BACK_COLOR),
			SelectionForeColor = _Color(Slot.SELECTION_FORE_COLOR),
			DetailForeColor = _Color(Slot.DETAIL_FORE_COLOR),
			HighlightStyle = _HighlightStyle,
			HighlightColor = _Color(Slot.HIGHLIGHT_COLOR),
			BorderColor = _Color(Slot.DROP_DOWN_BORDER_COLOR)
		};
	}


	/// <summary>Gets the drop-down list, creating it if necessary.</summary>
	/// <returns>Returns the drop-down list.</returns>
	private _ListPopupWindow<IListItem> _GetPopup()
	{
		if(_Popup is null)
		{
			_Popup = new _ListPopupWindow<IListItem>();
			_Popup.HotTracking = true;
			_Popup.ItemClicked += _Popup_ItemClicked;
			_Popup.SelectionChanged += _Popup_SelectionChanged;
			_Popup.ImageResolver = (m => (_HasFallbackImages ? _ResolveItemImage(m, true) : null));
		}

		return _Popup;
	}


	/// <summary>Fills and positions the drop-down list.</summary>
	/// <param name="items">Displayed items.</param>
	/// <param name="pattern">Search pattern for highlighting.</param>
	/// <param name="mode">Match mode for highlighting.</param>
	/// <param name="caseSensitive">Determines if highlighting is case-sensitive.</param>
	/// <param name="selectedIndex">Index selected in the list.</param>
	private void _ShowPopupItems(IReadOnlyList<IListItem> items, string pattern, ListMatchMode mode, bool caseSensitive, int selectedIndex)
	{
		_ListPopupWindow<IListItem> popup = _GetPopup();
		bool images = (_ShowsImages || _HasFallbackImages || ((!ReferenceEquals(items, _Views)) && items.Any(m => _HasImage(m))));
		Point origin = PointToScreen(Point.Empty);

		popup.ApplyStyle(Font, _ImageList, _GetPopupStyle(), false);
		popup.SetItems(items, images, pattern, mode, caseSensitive, selectedIndex);
		popup.ShowAt(new Point(origin.X + popup.GetTextOffset(images), origin.Y), Height, new Size(Math.Max(Width, _DropDownWidth), popup.GetWindowHeight(_MaxDropDownItems)), FindForm());

		if(_DroppedDown) { _UpdateDescription(); }
	}


	/// <summary>Fills the drop-down list with all items and positions it.</summary>
	/// <param name="selectedIndex">Index selected in the list.</param>
	private void _ShowPopup(int selectedIndex)
	{
		_SearchResults = null;
		_SearchIndices = null;
		_ShowPopupItems(_GetViews(), string.Empty, ListMatchMode.PREFIX, false, selectedIndex);
	}


	/// <summary>Marks the drop-down list as open after it has been shown.</summary>
	/// <param name="raiseEvent">Determines if the <see cref="DropDown"/> event is raised.</param>
	private void _SetDroppedDown(bool raiseEvent)
	{
		if(_DroppedDown) { return; }

		_DroppedDown = true;
		_Hook();

		Invalidate();
		AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);

		if(raiseEvent)
		{
			_RaisePendingClosed();
			_OnDropDown(EventArgs.Empty);
		}
		_UpdateDescription();
	}


	/// <summary>Opens the drop-down list with all items.</summary>
	private void _OpenDropDown()
	{
		if(_DroppedDown || (!_CanDropDown)) { return; }

		_RaisePendingClosed();
		_OnDropDown(EventArgs.Empty);
		if(_DroppedDown || (!_CanDropDown)) { return; }

		_CancelSearch();
		_SearchText = string.Empty;
		_ShowPopup((_Editor is null) ? _SelectedIndex : _GetComboListIndex());
		_SetDroppedDown(false);
	}


	/// <summary>Closes the drop-down list.</summary>
	/// <param name="commit">Determines if the item selected in the list becomes the selected item.</param>
	private void _CloseDropDown(bool commit)
	{
		if(!_DroppedDown) { return; }

		int index = (_Popup?.SelectedIndex ?? -1);
		bool search = (_SearchResults is not null);

		_DroppedDown = false;
		_SearchText = string.Empty;
		_CancelSearch();
		_Popup?.HideWindow();
		_Description?.HideWindow();
		_Unhook();

		if(IsDisposed || Disposing) { return; }

		Invalidate();
		AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);

		_ClosedPending = true;
		if(commit)
		{
			if(search)
			{
				_ChooseSearchResult(index);
			}
			else if((index >= 0) && (index < _Items.Count))
			{
				_SetSelection(index, _Items[index], true);
				if(_DroppedDown) { return; }
				_Editor?.SelectAll();
			}

			// an event handler of the selection change has reopened the drop-down list; its state belongs to the new session
			if(_DroppedDown) { return; }
		}

		_SearchResults = null;
		_SearchIndices = null;
		_RaisePendingClosed();
	}


	/// <summary>Raises the <see cref="DropDownClosed"/> event of a closed drop-down list if it has not been raised yet.</summary>
	/// <remarks>Keeps <see cref="DropDown"/> and <see cref="DropDownClosed"/> paired when an event handler reopens the list while it is being closed.</remarks>
	private void _RaisePendingClosed()
	{
		if(!_ClosedPending) { return; }

		_ClosedPending = false;
		_OnDropDownClosed(EventArgs.Empty);
	}


	/// <summary>Installs the message filter and the form event handlers.</summary>
	private void _Hook()
	{
		if(_Filter is null)
		{
			_Filter = new _DropDownFilter(this);
			Application.AddMessageFilter(_Filter);
		}

		Form? form = FindForm();
		if(form == _HookedForm) { return; }

		_UnhookForm();
		if(form is null) { return; }

		_HookedForm = form;
		form.Deactivate += _Form_Changed;
		form.Move += _Form_Changed;
		form.Resize += _Form_Changed;
	}


	/// <summary>Removes the message filter and the form event handlers.</summary>
	private void _Unhook()
	{
		if(_Filter is not null)
		{
			Application.RemoveMessageFilter(_Filter);
			_Filter = null;
		}

		_UnhookForm();
	}


	/// <summary>Removes the form event handlers.</summary>
	private void _UnhookForm()
	{
		if(_HookedForm is null) { return; }

		_HookedForm.Deactivate -= _Form_Changed;
		_HookedForm.Move -= _Form_Changed;
		_HookedForm.Resize -= _Form_Changed;
		_HookedForm = null;
	}


	/// <summary>Handles a change of the items.</summary>
	private void _ItemsChanged()
	{
		_Views = null;
		_HasImages = null;
		_ItemImages.Clear();
		_InvalidateTextItem();

		if(_DroppedDown)
		{
			if((_SearchResults is not null) && (_SearchIndices is not null))
			{
				_StartSearch(true);
			}
			else if(_SearchResults is null) { _ShowPopup(_Popup?.SelectedIndex ?? -1); }
		}

		Invalidate();
	}


	/// <summary>Moves the selection of the closed control.</summary>
	/// <param name="target">Target index.</param>
	private void _MoveSelection(int target)
	{
		if(_Items.Count == 0) { return; }

		target = Math.Clamp(target, 0, _Items.Count - 1);
		if(target != _SelectedIndex) { _SetSelection(target, _Items[target], true); }
	}


	/// <summary>Processes a typed character.</summary>
	/// <param name="c">Character.</param>
	private void _Search(char c)
	{
		List<IListItem> views = _GetViews();
		long now = Environment.TickCount64;

		if((now - _SearchTime) > _SEARCH_TIMEOUT) { _SearchText = string.Empty; }
		_SearchTime = now;
		_SearchText += c;

		if(views.Count == 0) { return; }

		int current = (_DroppedDown ? (_Popup?.SelectedIndex ?? -1) : _SelectedIndex);
		bool repeated = _SearchText.All(m => (char.ToUpperInvariant(m) == char.ToUpperInvariant(_SearchText[0])));
		int found = (repeated ? _FindPrefix(views, _SearchText.Substring(0, 1), current + 1) : _FindPrefix(views, _SearchText, Math.Max(0, current)));

		if(found < 0) { return; }

		if(_DroppedDown)
		{
			if(_Popup is not null) { _Popup.SelectedIndex = found; }
		}
		else { _MoveSelection(found); }
	}


	/// <summary>Processes a mouse wheel movement over the control or its text box.</summary>
	/// <param name="e">Event arguments.</param>
	private void _ProcessWheel(MouseEventArgs e)
	{
		if(_DroppedDown && (_Popup is not null))
		{
			_Popup.ScrollByWheel(e.Delta);
		}
		else if(_HasFocus && (!_ReadOnly) && (_Items.Count > 0))
		{
			int notches;
			int index = ((_Editor is null) ? _SelectedIndex : _GetComboListIndex());

			_WheelDelta += e.Delta;
			notches = _WheelDelta / 120;
			_WheelDelta -= notches * 120;

			if(notches != 0)
			{
				_MoveSelection((index < 0) ? 0 : (index - notches));
				_Editor?.SelectAll();
			}
		}
		else { return; }

		if(e is HandledMouseEventArgs handled) { handled.Handled = true; }
	}


	/// <summary>Copies the text of the control to the clipboard.</summary>
	private void _CopyText()
	{
		_Clipboard._TrySetText(Text);
	}


	/// <summary>Draws the image of a list item.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="item">List item.</param>
	/// <param name="bounds">Image bounds.</param>
	/// <param name="enabled">Determines if the image is drawn enabled.</param>
	private void _DrawImage(Graphics g, IListItem item, Rectangle bounds, bool enabled)
	{
		if(item.Image is not null)
		{
			if(enabled)
			{
				g.DrawImage(item.Image, bounds);
			}
			else { _DrawDisabledImage(g, item.Image, bounds); }
			return;
		}

		if((_ImageList is null) || string.IsNullOrEmpty(item.ImageKey)) { return; }

		int index = _ImageList.Images.IndexOfKey(item.ImageKey);
		if(index < 0) { return; }

		if(enabled)
		{
			_ImageList.Draw(g, bounds.X, bounds.Y, bounds.Width, bounds.Height, index);
		}
		else
		{
			using Image image = _ImageList.Images[index];
			_DrawDisabledImage(g, image, bounds);
		}
	}


	/// <summary>Gets the back color for the current state.</summary>
	/// <returns>Returns the back color.</returns>
	private Color _GetBackColor()
	{
		if(!Enabled) { return _Color(Slot.DISABLED_BACK_COLOR); }
		if(_ReadOnly) { return _Color(Slot.READ_ONLY_BACK_COLOR); }
		if(_DroppedDown) { return _Color(Slot.PRESSED_BACK_COLOR); }
		if(_Hot) { return _Color(Slot.HOT_BACK_COLOR); }

		return _Color(Slot.BACK_COLOR);
	}


	/// <summary>Draws the drop-down arrow.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="bounds">Button bounds.</param>
	/// <param name="color">Arrow color.</param>
	private void _DrawArrow(Graphics g, Rectangle bounds, Color color)
	{
		float size = LogicalToDeviceUnits(_ARROW_SIZE);
		float cx = bounds.Left + (bounds.Width / 2f);
		float cy = bounds.Top + (bounds.Height / 2f);
		SmoothingMode mode = g.SmoothingMode;

		g.SmoothingMode = SmoothingMode.AntiAlias;

		using(Pen pen = new(color, Math.Max(1f, LogicalToDeviceUnits(100) / 75f)) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
		{
			g.DrawLines(pen, new PointF[] { new(cx - size, cy - (size / 2f)), new(cx, cy + (size / 2f)), new(cx + size, cy - (size / 2f)) });
		}

		g.SmoothingMode = mode;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="SelectedIndexChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectedIndexChanged(EventArgs e)
	{
		SelectedIndexChanged?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="SelectedItemChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectedItemChanged(EventArgs e)
	{
		SelectedItemChanged?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="SelectionChangeCommitted"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectionChangeCommitted(EventArgs e)
	{
		SelectionChangeCommitted?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="DropDown"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnDropDown(EventArgs e)
	{
		DropDown?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="DropDownClosed"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnDropDownClosed(EventArgs e)
	{
		DropDownClosed?.Invoke(this, e);
	}


	/// <summary>Gets the display text of an item.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the text of an <see cref="IListItem"/>, otherwise the string representation of the item.</returns>
	protected virtual string _GetItemText(object item)
	{
		if(item is IListItem listItem) { return listItem.Text; }
		if(!string.IsNullOrEmpty(_DisplayMember)) { item = (_GetMemberValue(item, _DisplayMember) ?? string.Empty); }

		return (Convert.ToString(item, CultureInfo.CurrentCulture) ?? string.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the insertion of an item.</summary>
	/// <param name="index">Item index.</param>
	internal void _OnItemInserted(int index)
	{
		if((_SelectedIndex >= 0) && (index <= _SelectedIndex)) { _SelectedIndex++; }
		_ItemsChanged();
	}


	/// <summary>Handles the addition of several items at the end of the collection.</summary>
	internal void _OnItemsChanged()
	{
		_ItemsChanged();
	}


	/// <summary>Handles the removal of an item.</summary>
	/// <param name="index">Item index.</param>
	/// <remarks>The selection is updated before the caches are refreshed, so that no refresh sees a selected index beyond the end
	///          of the items.</remarks>
	internal void _OnItemRemoved(int index)
	{
		bool selected = (index == _SelectedIndex);

		if(index < _SelectedIndex) { _SelectedIndex--; }

		_Views = null;
		if(selected) { _SetSelection(-1, null, false); }
		_ItemsChanged();
	}


	/// <summary>Handles the replacement of an item.</summary>
	/// <param name="index">Item index.</param>
	/// <remarks>The selection is updated before the caches are refreshed, so that no refresh sees the replaced item as selected item.</remarks>
	internal void _OnItemSet(int index)
	{
		_Views = null;
		if(index == _SelectedIndex) { _SetSelection(index, _Items[index], false); }
		_ItemsChanged();
	}


	/// <summary>Handles the removal of all items.</summary>
	/// <remarks>The selection is cleared before the caches are refreshed, so that no refresh sees a selected index beyond the end
	///          of the items.</remarks>
	internal void _OnItemsCleared()
	{
		_Views = null;
		if(_SelectedIndex >= 0) { _SetSelection(-1, null, false); }
		_ItemsChanged();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the drop-down list, NULL if it has not been created yet.</summary>
	internal _ListPopupWindow<IListItem>? _DropDownList
	{
		get { return _Popup; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the selected index has changed.</summary>
	[Category("Behavior")][Description("Occurs when the selected index has changed.")]
	public event EventHandler? SelectedIndexChanged;

	/// <summary>Occurs when the selected item has changed.</summary>
	[Category("Behavior")][Description("Occurs when the selected item has changed.")]
	public event EventHandler? SelectedItemChanged;

	/// <summary>Occurs when the user has changed the selection.</summary>
	/// <remarks>Unlike <see cref="SelectedIndexChanged"/>, this event is not raised when the selection is changed by code.</remarks>
	[Category("Behavior")][Description("Occurs when the user has changed the selection.")]
	public event EventHandler? SelectionChangeCommitted;

	/// <summary>Occurs when the drop-down list opens.</summary>
	/// <remarks>For the list of all items, the event occurs before the list is filled, so event handlers can fill <see cref="Items"/> on
	///          demand. For search results in combo mode, the event occurs when the list has been shown.</remarks>
	[Category("Behavior")][Description("Occurs before the drop-down list opens.")]
	public event EventHandler? DropDown;

	/// <summary>Occurs when the drop-down list has been closed.</summary>
	[Category("Behavior")][Description("Occurs when the drop-down list has been closed.")]
	public event EventHandler? DropDownClosed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the items.</summary>
	[Category("Data")][Description("The items of the control.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public ObjectCollection Items
	{
		get { return _Items; }
	}


	/// <summary>Gets or sets the index of the selected item.</summary>
	/// <remarks>The index is -1 if no item is selected or the selected item is not in <see cref="Items"/>.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the index is less than -1 or not less than the number of items.</exception>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectedIndex
	{
		get { return _SelectedIndex; }
		set
		{
			if((value < -1) || (value >= _Items.Count)) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_SetSelection(value, ((value < 0) ? null : _Items[value]), false);
		}
	}


	/// <summary>Gets or sets the selected item.</summary>
	/// <remarks>If the item is in <see cref="Items"/>, it is selected there. Otherwise the item is displayed and
	///          <see cref="SelectedIndex"/> is -1; this way the control can show a value that is not (or no longer) available in the list.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object? SelectedItem
	{
		get { return _SelectedItem; }
		set { _SetSelection(((value is null) ? -1 : _Items.IndexOf(value)), value, false); }
	}


	/// <summary>Gets or sets the text of the control.</summary>
	/// <remarks>In list mode, the text is the display text of the selected item. Setting the text selects the first item whose text
	///          equals the value (case-insensitive), or clears the selection if there is no such item. In combo mode, the text is the
	///          text of the text box; setting it selects a matching item as well, or keeps the text as free text (see
	///          <see cref="AllowFreeText"/>).</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[AllowNull]
	public override string Text
	{
		get { return base.Text; }
		set
		{
			if(_Editor is not null)
			{
				_CloseDropDown(false);
				_SetEditorText(value ?? string.Empty);
				_CommitText(false);
				return;
			}

			int index = FindStringExact(value);
			_SetSelection(index, ((index < 0) ? null : _Items[index]), false);
		}
	}


	/// <summary>Gets the color table.</summary>
	/// <remarks>The table defines a theme and optional overrides of single colors.</remarks>
	[Category("Appearance")][Description("The colors of the control: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedComboBoxColors Colors
	{
		get { return _Colors; }
	}


	/// <summary>Gets the color table for the theme manager.</summary>
	Themes.ControlColorTable Themes.IThemedControl.ThemeColors
	{
		get { return _Colors; }
	}


	/// <summary>Gets if the theme manager applies the theme to the child controls.</summary>
	bool Themes.IThemedControl.ThemesChildren
	{
		get { return false; }
	}


	/// <summary>Gets or sets the image list.</summary>
	/// <remarks>Items that implement <see cref="IListItem"/> use the image with their <see cref="IListItem.ImageKey"/>, unless they
	///          provide an <see cref="IListItem.Image"/>.</remarks>
	[Category("Appearance")][Description("The image list that provides the item images.")][DefaultValue(null)]
	public ImageList? ImageList
	{
		get { return _ImageList; }
		set
		{
			if(value == _ImageList) { return; }

			if(_ImageList is not null) { _ImageList.Disposed -= _ImageList_Disposed; }
			_ImageList = value;
			if(_ImageList is not null) { _ImageList.Disposed += _ImageList_Disposed; }

			_ItemsChanged();
		}
	}


	/// <summary>Gets or sets if the control is read-only.</summary>
	/// <remarks>A read-only control can take the focus and its text can be copied with Ctrl+C, but the selection cannot be changed by
	///          the user.</remarks>
	[Category("Behavior")][Description("Determines if the selection can be changed by the user.")][DefaultValue(false)]
	public bool ReadOnly
	{
		get { return _ReadOnly; }
		set
		{
			if(value == _ReadOnly) { return; }

			_ReadOnly = value;
			if(value) { _CloseDropDown(false); }
			if(_Editor is not null) { _Editor.ReadOnly = value; }

			Invalidate();
			AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
		}
	}


	/// <summary>Gets or sets if the detail text of the selected item is displayed in the control.</summary>
	[Category("Appearance")][Description("Determines if the detail text of the selected item is displayed in the control.")][DefaultValue(false)]
	public bool ShowDetailInDisplay
	{
		get { return _ShowDetailInDisplay; }
		set
		{
			if(value == _ShowDetailInDisplay) { return; }

			_ShowDetailInDisplay = value;
			_LayoutEditor();
			Invalidate();
		}
	}


	/// <summary>Gets or sets how the control shows that it has the focus.</summary>
	/// <remarks>The values can be combined; the default is a border in the focus color.</remarks>
	[Category("Appearance")][Description("Determines how the control shows that it has the focus.")][DefaultValue(_DEFAULT_FOCUS_STYLE)]
	public ThemedComboBoxFocusStyle FocusStyle
	{
		get { return _FocusStyle; }
		set
		{
			if(value == _FocusStyle) { return; }

			_FocusStyle = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the minimum width of the drop-down list.</summary>
	/// <remarks>The list is never narrower than the control. 0 uses the control width.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
	[Category("Behavior")][Description("The minimum width of the drop-down list; the list is never narrower than the control.")][DefaultValue(0)]
	public int DropDownWidth
	{
		get { return _DropDownWidth; }
		set
		{
			if(value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_DropDownWidth = value;
		}
	}


	/// <summary>Gets or sets the maximum number of items displayed in the drop-down list.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
	[Category("Behavior")][Description("The maximum number of items displayed in the drop-down list.")][DefaultValue(_DEFAULT_MAX_DROPDOWN_ITEMS)]
	public int MaxDropDownItems
	{
		get { return _MaxDropDownItems; }
		set
		{
			if(value < 1) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_MaxDropDownItems = value;
		}
	}


	/// <summary>Gets or sets if the drop-down list is open.</summary>
	/// <remarks>Closing the list by setting this property keeps the selected item.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool DroppedDown
	{
		get { return _DroppedDown; }
		set
		{
			if(value)
			{
				_OpenDropDown();
			}
			else { _CloseDropDown(false); }
		}
	}


	/// <summary>Gets or sets the back color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedComboBoxColors.BackColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color BackColor
	{
		get { return _Color(Slot.BACK_COLOR); }
		set { _Colors.BackColor = value; }
	}


	/// <summary>Gets or sets the fore color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedComboBoxColors.ForeColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color ForeColor
	{
		get { return _Color(Slot.FORE_COLOR); }
		set { _Colors.ForeColor = value; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the display text of an item.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the text of an <see cref="IListItem"/>, otherwise the string representation of the item. Returns an empty
	///          string for NULL.</returns>
	public string GetItemText(object? item)
	{
		return ((item is null) ? string.Empty : _GetItemText(item));
	}


	/// <summary>Finds the first item whose text starts with a text.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the item index, or -1 if no item matches. The comparison is case-insensitive.</returns>
	public int FindString(string? text)
	{
		if(string.IsNullOrEmpty(text)) { return -1; }

		for(int i = 0; i < _Items.Count; i++)
		{
			if(GetItemText(_Items[i]).StartsWith(text, StringComparison.CurrentCultureIgnoreCase)) { return i; }
		}

		return -1;
	}


	/// <summary>Finds the first item whose text equals a text.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the item index, or -1 if no item matches. The comparison is case-insensitive.</returns>
	public int FindStringExact(string? text)
	{
		if(string.IsNullOrEmpty(text)) { return -1; }

		for(int i = 0; i < _Items.Count; i++)
		{
			if(string.Equals(GetItemText(_Items[i]), text, StringComparison.CurrentCultureIgnoreCase)) { return i; }
		}

		return -1;
	}


	/// <summary>Resets the color table.</summary>
	public void ResetColors()
	{
		_Colors.Reset();
	}


	/// <summary>Determines if the color table has to be serialized by the designer.</summary>
	/// <returns>Returns <c>true</c> if the color table differs from the default, otherwise returns <c>false</c>.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ShouldSerializeColors()
	{
		return (!_Colors.IsDefault);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Changed event of the color table.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Colors_Changed(object? sender, EventArgs e)
	{
		if(_DroppedDown) { _ShowPopup(_Popup?.SelectedIndex ?? -1); }

		Invalidate();
		OnBackColorChanged(EventArgs.Empty);
		OnForeColorChanged(EventArgs.Empty);
	}


	/// <summary>Handles the Disposed event of the image list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ImageList_Disposed(object? sender, EventArgs e)
	{
		if(Disposing || IsDisposed)
		{
			if(_ImageList is not null) { _ImageList.Disposed -= _ImageList_Disposed; }
			_ImageList = null;
			return;
		}

		ImageList = null;
	}


	/// <summary>Handles the ItemClicked event of the drop-down list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_ItemClicked(object? sender, EventArgs e)
	{
		_CloseDropDown(true);
	}


	/// <summary>Handles the Deactivate, Move and Resize events of the form.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Form_Changed(object? sender, EventArgs e)
	{
		_CloseDropDown(false);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Size DefaultSize
	{
		get { return new Size(121, 24); }
	}


	/// <inheritdoc/>
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new _AccessibleObject(this);
	}


	/// <inheritdoc/>
	/// <remarks>The height is determined by the font and cannot be changed.</remarks>
	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, _PreferredHeight, specified);
	}


	/// <inheritdoc/>
	public override Size GetPreferredSize(Size proposedSize)
	{
		return new Size(Width, _PreferredHeight);
	}


	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		_ClearFonts();
		base.OnFontChanged(e);

		_UpdateHeight();
		_LayoutEditor();
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		_LayoutEditor();
	}


	/// <inheritdoc/>
	protected override void OnTabStopChanged(EventArgs e)
	{
		base.OnTabStopChanged(e);
		if(_Editor is not null) { _Editor.TabStop = TabStop; }
	}


	/// <inheritdoc/>
	protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
	{
		base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
		_UpdateHeight();
		_LayoutEditor();
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		if(!Enabled) { _CloseDropDown(false); }
		if(_Editor is not null) { _Editor.Visible = Enabled; }

		base.OnEnabledChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnVisibleChanged(EventArgs e)
	{
		if(!Visible) { _CloseDropDown(false); }
		base.OnVisibleChanged(e);
	}


	/// <inheritdoc/>
	protected override void OnParentChanged(EventArgs e)
	{
		_CloseDropDown(false);
		base.OnParentChanged(e);
	}


	/// <inheritdoc/>
	protected override void OnHandleDestroyed(EventArgs e)
	{
		_CloseDropDown(false);
		base.OnHandleDestroyed(e);
	}


	/// <inheritdoc/>
	/// <remarks>In combo mode, the focus is passed to the text box.</remarks>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);

		if((_Editor is not null) && _Editor.CanFocus) { _Editor.Focus(); }
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		if(_Editor is null) { _CloseDropDown(false); }
		_SearchText = string.Empty;

		base.OnLostFocus(e);
		Invalidate();
	}


	/// <inheritdoc/>
	/// <remarks>When the focus leaves the control, the drop-down list closes and in combo mode the text is applied.</remarks>
	protected override void OnLeave(EventArgs e)
	{
		_CloseDropDown(false);
		_CommitText(true);

		base.OnLeave(e);
	}


	/// <inheritdoc/>
	protected override bool IsInputKey(Keys keyData)
	{
		switch(keyData & Keys.KeyCode)
		{
			case Keys.Up:
			case Keys.Down:
			case Keys.Left:
			case Keys.Right:
			case Keys.Home:
			case Keys.End:
			case Keys.PageUp:
			case Keys.PageDown:
				return true;
			case Keys.Enter:
			case Keys.Escape:
				if(_DroppedDown) { return true; }
				break;
		}

		return base.IsInputKey(keyData);
	}


	/// <inheritdoc/>
	/// <remarks>All printable characters are input characters, used for type-to-search. Otherwise the form would process them as
	///          mnemonics, because the control does not report that it wants characters (WM_GETDLGCODE), and typing <c>b</c> would
	///          activate the control with the mnemonic B. Mnemonics with Alt still work.</remarks>
	protected override bool IsInputChar(char charCode)
	{
		return ((!char.IsControl(charCode)) || base.IsInputChar(charCode));
	}


	/// <inheritdoc/>
	/// <remarks>Tab closes an open drop-down list and selects the item selected in the list.</remarks>
	protected override bool ProcessDialogKey(Keys keyData)
	{
		if(_DroppedDown && ((keyData & Keys.KeyCode) == Keys.Tab) && ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)) { _CloseDropDown(true); }

		return base.ProcessDialogKey(keyData);
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if(e.Handled) { return; }

		if(_Editor is not null)
		{
			if(_ProcessComboKey(e.KeyData))
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			return;
		}

		if((e.KeyData == (Keys.Control | Keys.C)) || (e.KeyData == (Keys.Control | Keys.Insert)))
		{
			_CopyText();
			e.Handled = true;
			return;
		}

		if((e.KeyData == Keys.F4) || (e.KeyData == (Keys.Alt | Keys.Down)) || (e.KeyData == (Keys.Alt | Keys.Up)))
		{
			if(_DroppedDown)
			{
				_CloseDropDown(true);
			}
			else { _OpenDropDown(); }

			e.Handled = true;
			return;
		}

		if(_DroppedDown && (_Popup is not null))
		{
			int page = Math.Max(1, _Popup.VisibleRowCount - 1);

			switch(e.KeyData)
			{
				case Keys.Up: _Popup.MoveSelection(-1); break;
				case Keys.Down: _Popup.MoveSelection(1); break;
				case Keys.PageUp: _Popup.MoveSelection(-page); break;
				case Keys.PageDown: _Popup.MoveSelection(page); break;
				case Keys.Home: _Popup.MoveSelection(-_Popup.ItemCount); break;
				case Keys.End: _Popup.MoveSelection(_Popup.ItemCount); break;
				case Keys.Enter: _CloseDropDown(true); e.SuppressKeyPress = true; break;
				case Keys.Escape: _CloseDropDown(false); e.SuppressKeyPress = true; break;
				default: return;
			}

			e.Handled = true;
			return;
		}

		if(_ReadOnly || (_Items.Count == 0)) { return; }

		int index = _SelectedIndex;
		int pageSize = Math.Max(1, _MaxDropDownItems - 1);

		switch(e.KeyData)
		{
			case Keys.Up:
			case Keys.Left:
				_MoveSelection(index - 1); break;
			case Keys.Down:
			case Keys.Right:
				_MoveSelection(index + 1); break;
			case Keys.PageUp: _MoveSelection(index - pageSize); break;
			case Keys.PageDown: _MoveSelection(Math.Max(0, index) + pageSize); break;
			case Keys.Home: _MoveSelection(0); break;
			case Keys.End: _MoveSelection(_Items.Count - 1); break;
			default: return;
		}

		e.Handled = true;
	}


	/// <inheritdoc/>
	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		base.OnKeyPress(e);
		if(e.Handled || (_Editor is not null) || _ReadOnly || char.IsControl(e.KeyChar)) { return; }

		_Search(e.KeyChar);
		e.Handled = true;
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if(e.Button != MouseButtons.Left) { return; }

		if(_Editor is not null)
		{
			if((!_Editor.Focused) && _Editor.CanFocus) { _Editor.Focus(); }
			if(_ReadOnly || (!_ButtonBounds.Contains(e.Location))) { return; }
		}
		else if((!Focused) && CanFocus) { Focus(); }

		if(_ReadOnly) { return; }

		if(_DroppedDown)
		{
			_CloseDropDown(false);
		}
		else { _OpenDropDown(); }
	}


	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);

		bool hot = _ButtonBounds.Contains(e.Location);
		if((hot == _ButtonHot) && _Hot) { return; }

		_ButtonHot = hot;
		_Hot = true;
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);

		if((_Editor is not null) && _Editor.Visible && _Editor.Bounds.Contains(PointToClient(MousePosition)))
		{
			_ButtonHot = false;
			Invalidate();
			return;
		}

		_Hot = _ButtonHot = false;
		Invalidate();
	}


	/// <inheritdoc/>
	/// <remarks>While the drop-down list is open, the wheel scrolls the list. Otherwise, the wheel changes the selection if the control
	///          has the focus.</remarks>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
		_ProcessWheel(e);
	}


	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs e)
	{}


	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		bool enabled = Enabled;
		bool editable = (enabled && (!_ReadOnly));
		bool focused = (enabled && (_HasFocus || _DroppedDown));
		bool combo = (_Editor is not null);
		Color back = _GetBackColor();
		Color fore = (enabled ? (_ReadOnly ? _Color(Slot.READ_ONLY_FORE_COLOR) : _Color(Slot.FORE_COLOR)) : _Color(Slot.DISABLED_FORE_COLOR));
		Color border = _Color(Slot.BORDER_COLOR);
		Rectangle button = _ButtonBounds;
		IListItem? item = (combo ? _GetTextItem() : _GetSelectedView());
		Image? fallback = (combo ? (((item is null) || (!_HasImage(item))) ? _GetFallbackImage() : null) : (((item is not null) && (!_HasImage(item))) ? _ResolveItemImage(item, true) : null));
		bool images = (combo ? _ComboShowsImages() : (_ShowsImages || _HasFallbackImages || ((item is not null) && _HasImage(item))));
		Rectangle text = _GetTextBounds(g, item, images, out Rectangle detail);

		_UpdateEditorColors();

		using(SolidBrush brush = new(back)) { g.FillRectangle(brush, ClientRectangle); }

		if(editable && (_ButtonHot || _DroppedDown))
		{
			int inset = LogicalToDeviceUnits(2);
			using SolidBrush brush = new(_Color(Slot.BUTTON_HOT_BACK_COLOR));
			g.FillRectangle(brush, Rectangle.Inflate(button, -inset, -inset));
		}
		_DrawArrow(g, button, (editable ? _Color(Slot.BUTTON_FORE_COLOR) : _Color(Slot.DISABLED_FORE_COLOR)));

		if(images)
		{
			int padding = LogicalToDeviceUnits(_IMAGE_PADDING);
			int size = _ImageSize;
			Rectangle bounds = new(1 + padding, (Height - size) / 2, size, size);

			if(fallback is not null)
			{
				if(enabled)
				{
					g.DrawImage(fallback, bounds);
				}
				else { _DrawDisabledImage(g, fallback, bounds); }
			}
			else if(item is not null) { _DrawImage(g, item, bounds, enabled); }
		}

		if((item is not null) && (!detail.IsEmpty))
		{
			TextRenderer.DrawText(g, item.Detail, _GetFont(FontStyle.Regular), detail, (enabled ? _Color(Slot.DETAIL_FORE_COLOR) : fore), _TEXT_FLAGS);
		}

		if(combo)
		{
			if((!enabled) && (text.Width > 0)) { TextRenderer.DrawText(g, base.Text, Font, text, fore, _TEXT_FLAGS); }
		}
		else if((item is not null) && (text.Width > 0))
		{
			Color color = ((editable && (!item.ForeColor.IsEmpty)) ? item.ForeColor : fore);
			TextRenderer.DrawText(g, item.Text, _GetFont(item.FontStyle), text, color, _TEXT_FLAGS);
		}

		if(!enabled)
		{
			border = _Color(Slot.DISABLED_BORDER_COLOR);
		}
		else if(focused && _FocusStyle.HasFlag(ThemedComboBoxFocusStyle.BORDER))
		{
			border = _Color(Slot.FOCUS_COLOR);
		}
		else if(_Hot || _DroppedDown) { border = _Color(Slot.HOT_BORDER_COLOR); }

		using(Pen pen = new(border)) { g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1); }

		if(!focused) { return; }

		int line = (_FocusStyle.HasFlag(ThemedComboBoxFocusStyle.LINE) ? LogicalToDeviceUnits(_FOCUS_LINE) : 0);

		if(line > 0)
		{
			using SolidBrush brush = new(_Color(Slot.FOCUS_COLOR));
			g.FillRectangle(brush, 1, Height - 1 - line, Width - 2, line);
		}

		if(_FocusStyle.HasFlag(ThemedComboBoxFocusStyle.DOTTED) && (!combo))
		{
			int inset = LogicalToDeviceUnits(_FOCUS_INSET);
			Rectangle rect = Rectangle.FromLTRB(1 + inset, 1 + inset, button.Left - inset, Height - 1 - inset - line);

			if((rect.Width > 0) && (rect.Height > 0)) { ControlPaint.DrawFocusRectangle(g, rect, fore, back); }
		}
	}


	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_DroppedDown = false;
			_Popup?.HideWindow();
			_Unhook();
			_UnbindData();

			_CancelSearch();

			if(_SearchTimer is not null)
			{
				_SearchTimer.Tick -= _SearchTimer_Tick;
				_SearchTimer.Dispose();
				_SearchTimer = null;
			}

			if(_Popup is not null)
			{
				_Popup.ItemClicked -= _Popup_ItemClicked;
				_Popup.SelectionChanged -= _Popup_SelectionChanged;
				_Popup.Dispose();
				_Popup = null;
			}

			_Description?.Dispose();
			_Description = null;

			if(_ImageList is not null)
			{
				_ImageList.Disposed -= _ImageList_Disposed;
				_ImageList = null;
			}

			_Colors.Changed -= _Colors_Changed;
			_ClearFonts();
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the type name, the number of items and the text.</returns>
	public override string ToString()
	{
		return $"{GetType().FullName}, Items.Count: {_Items.Count}, Text: {Text}";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _DropDownFilter                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the message filter that closes the drop-down list on clicks outside of the list and the control.</summary>
	private sealed class _DropDownFilter: IMessageFilter
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Owner control.</summary>
		private readonly ThemedComboBox _Owner;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="owner">Owner control.</param>
		public _DropDownFilter(ThemedComboBox owner)
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IMessageFilter                                                                                   //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public bool PreFilterMessage(ref Message m)
		{
			bool client = ((m.Msg == _WM_LBUTTONDOWN) || (m.Msg == _WM_RBUTTONDOWN) || (m.Msg == _WM_MBUTTONDOWN) || (m.Msg == _WM_XBUTTONDOWN));
			bool nonClient = ((m.Msg == _WM_NCLBUTTONDOWN) || (m.Msg == _WM_NCRBUTTONDOWN) || (m.Msg == _WM_NCMBUTTONDOWN) || (m.Msg == _WM_NCXBUTTONDOWN));
			_ListPopupWindow<IListItem>? popup = _Owner._Popup;

			if((!(client || nonClient)) || (!_Owner._DroppedDown)) { return false; }

			_ListDescriptionWindow? description = _Owner._Description;
			_EditBox? editor = _Owner._Editor;

			if((popup is not null) && popup.IsHandleCreated && (m.HWnd == popup.Handle)) { return false; }
			if((description is not null) && description.IsHandleCreated && (m.HWnd == description.Handle)) { return false; }
			if(client && _Owner.IsHandleCreated && (m.HWnd == _Owner.Handle)) { return false; }
			if(client && (editor is not null) && editor.IsHandleCreated && (m.HWnd == editor.Handle) && (_Owner._SearchResults is not null)) { return false; }

			_Owner._CloseDropDown(false);
			return false;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _AccessibleObject                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the accessible object of the control.</summary>
	private sealed class _AccessibleObject: ControlAccessibleObject
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Owner control.</summary>
		private readonly ThemedComboBox _Owner;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="owner">Owner control.</param>
		public _AccessibleObject(ThemedComboBox owner): base(owner)
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] AccessibleObject                                                                                  //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public override AccessibleRole Role
		{
			get { return ((_Owner.AccessibleRole == AccessibleRole.Default) ? AccessibleRole.ComboBox : _Owner.AccessibleRole); }
		}


		/// <inheritdoc/>
		public override string? Value
		{
			get { return _Owner.Text; }
			set {}
		}


		/// <inheritdoc/>
		public override AccessibleStates State
		{
			get
			{
				AccessibleStates rval = base.State | (_Owner._DroppedDown ? AccessibleStates.Expanded : AccessibleStates.Collapsed);
				if(_Owner._ReadOnly) { rval |= AccessibleStates.ReadOnly; }

				return rval;
			}
		}


		/// <inheritdoc/>
		public override string? DefaultAction
		{
			get { return (_Owner._DroppedDown ? "Close" : "Open"); }
		}


		/// <inheritdoc/>
		public override void DoDefaultAction()
		{
			_Owner.DroppedDown = (!_Owner._DroppedDown);
		}
	}
}
