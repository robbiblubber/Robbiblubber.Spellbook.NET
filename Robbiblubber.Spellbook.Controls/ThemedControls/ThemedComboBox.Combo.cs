namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;

using Slot = Robbiblubber.Spellbook.Controls.ThemedComboBoxColors._Slot;



public partial class ThemedComboBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum width of the description window in logical units.</summary>
	private const int _DESCRIPTION_WIDTH = 320;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Drop-down style.</summary>
	private ThemedComboBoxStyle _DropDownStyle = ThemedComboBoxStyle.LIST;

	/// <summary>Text box of the combo mode, NULL in list mode.</summary>
	private _EditBox? _Editor = null;

	/// <summary>Determines if the text of the text box is being set by the control.</summary>
	private bool _SettingEditorText = false;

	/// <summary>Search source, NULL to search the items.</summary>
	private IAutoCompletionSource? _SearchSource = null;

	/// <summary>Match mode of the search.</summary>
	private ListMatchMode _MatchMode = ListMatchMode.FUZZY;

	/// <summary>Filter mode of the search.</summary>
	private ListFilterMode _FilterMode = ListFilterMode.FILTER;

	/// <summary>Determines if the search is case-sensitive.</summary>
	private bool _CaseSensitive = false;

	/// <summary>Highlight style of matched characters.</summary>
	private ListHighlightStyle _HighlightStyle = ListHighlightStyle.BOLD;

	/// <summary>Minimum text length that starts a search while typing.</summary>
	private int _MinimumSearchLength = 1;

	/// <summary>Determines if text that matches no item is allowed.</summary>
	private bool _AllowFreeText = true;

	/// <summary>Default image of the combo mode.</summary>
	private Image? _DefaultImage = null;

	/// <summary>Determines if the description of the selected list item is shown.</summary>
	private bool _ShowDescriptions = false;

	/// <summary>Items displayed in the search list, NULL if the list shows <see cref="Items"/>.</summary>
	private List<IListItem>? _SearchResults = null;

	/// <summary>Indices of the search results in <see cref="Items"/>, NULL if the results come from the search source.</summary>
	private List<int>? _SearchIndices = null;

	/// <summary>Items of the last result of the search source.</summary>
	private List<IListItem>? _SourceItems = null;

	/// <summary>Version of the search, incremented for each search, so obsolete asynchronous results are discarded.</summary>
	private int _SearchVersion = 0;

	/// <summary>Cancellation of the running search.</summary>
	private CancellationTokenSource? _SearchCancellation = null;

	/// <summary>Item whose text equals the text of the text box, NULL if there is none.</summary>
	private IListItem? _TextItem = null;

	/// <summary>Determines if <see cref="_TextItem"/> and <see cref="_ResolvedImage"/> are valid.</summary>
	private bool _TextItemValid = false;

	/// <summary>Image provided by the <see cref="ResolveImage"/> event.</summary>
	private Image? _ResolvedImage = null;

	/// <summary>Description window.</summary>
	private _ListDescriptionWindow? _Description = null;

	/// <summary>Delay in milliseconds between typing and searching.</summary>
	private int _SearchDelay = 0;

	/// <summary>Timer that delays the search while typing, NULL if it has not been created yet.</summary>
	private System.Windows.Forms.Timer? _SearchTimer = null;

	/// <summary>Text displayed while the search source is loading.</summary>
	private string _LoadingText = "Loading...";

	/// <summary>Handlers of the <see cref="ResolveImage"/> event.</summary>
	private EventHandler<ThemedComboBoxImageEventArgs>? _ResolveImage = null;

	/// <summary>Images resolved for items without image, by item (the wrapped object for objects that do not implement
	///          <see cref="IListItem"/>).</summary>
	private readonly Dictionary<object, Image?> _ItemImages = new(ReferenceEqualityComparer.Instance);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Ranks items by how well they match a pattern.</summary>
	/// <param name="items">Items.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the indices of the matching items, best matches first. Matches of the same quality keep their order.</returns>
	internal static List<int> _Rank(IReadOnlyList<IListItem> items, string pattern, ListMatchMode mode, bool caseSensitive)
	{
		List<(int Score, int Index)> matches = new();

		for(int i = 0; i < items.Count; i++)
		{
			int score = _ListMatcher._Match(items[i].FilterText ?? items[i].Text, pattern, mode, caseSensitive);
			if(score >= 0) { matches.Add((score, i)); }
		}

		if(pattern.Length > 0)
		{
			matches.Sort((m, n) =>
			{
				int rval = _ListMatcher._GetClass(n.Score).CompareTo(_ListMatcher._GetClass(m.Score));

				if(rval == 0) { rval = n.Score.CompareTo(m.Score); }
				if(rval == 0) { rval = m.Index.CompareTo(n.Index); }

				return rval;
			});
		}

		return matches.Select(m => m.Index).ToList();
	}


	/// <summary>Finds the item that matches a pattern best.</summary>
	/// <param name="items">Items.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the index of the best match, or -1 if no item matches or the pattern is empty.</returns>
	internal static int _FindBest(IReadOnlyList<IListItem> items, string pattern, ListMatchMode mode, bool caseSensitive)
	{
		if(pattern.Length == 0) { return -1; }

		List<int> ranked = _Rank(items, pattern, mode, caseSensitive);
		return ((ranked.Count == 0) ? -1 : ranked[0]);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if items and texts without image get an image from <see cref="ResolveImage"/> or <see cref="DefaultImage"/>.</summary>
	/// <remarks>If so, the image column is always displayed, so texts are aligned whether they have an image or not.</remarks>
	private bool _HasFallbackImages
	{
		get { return ((_DefaultImage is not null) || (_ResolveImage is not null)); }
	}


	/// <summary>Gets if the control or its text box has the focus.</summary>
	private bool _HasFocus
	{
		get { return (Focused || ((_Editor is not null) && _Editor.Focused)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the text box of the combo mode.</summary>
	private void _CreateEditor()
	{
		if(_Editor is not null) { return; }

		_Editor = new _EditBox(this) { BorderStyle = BorderStyle.None, TabStop = TabStop, TabIndex = 0, ReadOnly = _ReadOnly, Visible = Enabled };
		_Editor.TextChanged += _Editor_TextChanged;
		_Editor.GotFocus += _Editor_FocusChanged;
		_Editor.LostFocus += _Editor_FocusChanged;
		_Editor.MouseEnter += _Editor_MouseEnter;
		_Editor.MouseLeave += _Editor_MouseLeave;
		_Editor.MouseWheel += _Editor_MouseWheel;

		_SetEditorText(base.Text);
		Controls.Add(_Editor);

		SetStyle(ControlStyles.Selectable, false);
		_LayoutEditor();
	}


	/// <summary>Removes the text box of the combo mode.</summary>
	private void _RemoveEditor()
	{
		if(_Editor is null) { return; }

		bool focused = _Editor.Focused;
		_EditBox editor = _Editor;

		_Editor = null;
		editor.TextChanged -= _Editor_TextChanged;
		editor.GotFocus -= _Editor_FocusChanged;
		editor.LostFocus -= _Editor_FocusChanged;
		editor.MouseEnter -= _Editor_MouseEnter;
		editor.MouseLeave -= _Editor_MouseLeave;
		editor.MouseWheel -= _Editor_MouseWheel;
		Controls.Remove(editor);
		editor.Dispose();

		SetStyle(ControlStyles.Selectable, true);
		if(focused && CanFocus) { Focus(); }
	}


	/// <summary>Sets the text of the text box without starting a search.</summary>
	/// <param name="text">Text.</param>
	private void _SetEditorText(string text)
	{
		if(_Editor is null) { return; }

		_SettingEditorText = true;
		try
		{
			if(_Editor.Text != text) { _Editor.Text = text; }
		}
		finally { _SettingEditorText = false; }

		if(base.Text != _Editor.Text) { base.Text = _Editor.Text; }
		_InvalidateTextItem();
	}


	/// <summary>Marks the item that matches the text as outdated.</summary>
	private void _InvalidateTextItem()
	{
		if(_Editor is null) { return; }

		_TextItemValid = false;
		_LayoutEditor();
		Invalidate();
	}


	/// <summary>Determines the item whose text equals the text of the text box and the image for the text.</summary>
	private void _UpdateTextItem()
	{
		if(_TextItemValid || (_Editor is null)) { return; }

		string text = _Editor.Text;
		_TextItem = null;
		_ResolvedImage = null;
		_TextItemValid = true;

		if((_SelectedItem is not null) && string.Equals(GetItemText(_SelectedItem), text, StringComparison.CurrentCultureIgnoreCase))
		{
			List<IListItem> views = _GetViews();
			_TextItem = (((_SelectedIndex >= 0) && (_SelectedIndex < views.Count)) ? views[_SelectedIndex] : _CreateView(_SelectedItem));
		}
		else
		{
			int index = FindStringExact(text);
			if(index >= 0) { _TextItem = _GetViews()[index]; }
		}

		if(_TextItem is not null)
		{
			if(!_HasImage(_TextItem)) { _ResolvedImage = _ResolveItemImage(_TextItem, false); }
		}
		else if(_ResolveImage is not null)
		{
			ThemedComboBoxImageEventArgs e = new(text, null);

			_OnResolveImage(e);
			_ResolvedImage = e.Image;
		}
	}


	/// <summary>Gets the image for an item without image.</summary>
	/// <param name="item">Item.</param>
	/// <param name="useDefault">Determines if <see cref="DefaultImage"/> is returned when <see cref="ResolveImage"/> provides no image.</param>
	/// <returns>Returns the image, or NULL if there is none.</returns>
	/// <remarks>The results of <see cref="ResolveImage"/> are cached until the items, the default image or the event handlers change.</remarks>
	private Image? _ResolveItemImage(IListItem item, bool useDefault)
	{
		Image? rval = null;
		object key = ((item is _ObjectListItem wrapper) ? wrapper.Value : item);

		if((_ResolveImage is not null) && (!_ItemImages.TryGetValue(key, out rval)))
		{
			ThemedComboBoxImageEventArgs e = new(item.Text, item);

			_OnResolveImage(e);
			rval = e.Image;
			_ItemImages[key] = rval;
		}

		return (rval ?? (useDefault ? _DefaultImage : null));
	}


	/// <summary>Discards the cached images of items.</summary>
	private void _ClearImageCache()
	{
		_ItemImages.Clear();
		_InvalidateTextItem();
		_Popup?.Invalidate();
		Invalidate();
	}


	/// <summary>Gets the item displayed for the text of the text box.</summary>
	/// <returns>Returns the item, or NULL if the text matches no item.</returns>
	private IListItem? _GetTextItem()
	{
		_UpdateTextItem();
		return _TextItem;
	}


	/// <summary>Gets the image displayed in combo mode if the text matches no item with an image.</summary>
	/// <returns>Returns the image provided by <see cref="ResolveImage"/>, otherwise <see cref="DefaultImage"/>.</returns>
	private Image? _GetFallbackImage()
	{
		_UpdateTextItem();
		return (_ResolvedImage ?? _DefaultImage);
	}


	/// <summary>Gets the bounds of the text area.</summary>
	/// <param name="g">Graphics used to measure the detail text, NULL to create one.</param>
	/// <param name="item">Displayed item.</param>
	/// <param name="hasImage">Determines if an image column is displayed.</param>
	/// <param name="detail">Returns the bounds of the detail text, empty if no detail text is displayed.</param>
	/// <returns>Returns the bounds of the text.</returns>
	private Rectangle _GetTextBounds(Graphics? g, IListItem? item, bool hasImage, out Rectangle detail)
	{
		int textPadding = LogicalToDeviceUnits(_TEXT_PADDING);
		int x = (hasImage ? (1 + (LogicalToDeviceUnits(_IMAGE_PADDING) * 2) + _ImageSize + textPadding) : (1 + textPadding));
		int right = _ButtonBounds.Left - textPadding;

		detail = Rectangle.Empty;

		if((item is not null) && _ShowDetailInDisplay && (!string.IsNullOrEmpty(item.Detail)) && (right > x))
		{
			Font font = _GetFont(FontStyle.Regular);
			int width = ((g is null) ? TextRenderer.MeasureText(item.Detail, font, new Size(int.MaxValue, int.MaxValue), _TEXT_FLAGS) : TextRenderer.MeasureText(g, item.Detail, font, new Size(int.MaxValue, int.MaxValue), _TEXT_FLAGS)).Width;
			int dx = Math.Max(x + ((right - x) / 2), right - width);

			detail = new Rectangle(dx, 0, right - dx, Height);
			right = dx - LogicalToDeviceUnits(_DETAIL_SPACING);
		}

		return new Rectangle(x, 0, Math.Max(0, right - x), Height);
	}


	/// <summary>Gets if the image column is displayed in combo mode.</summary>
	/// <returns>Returns <c>true</c> if the image column is displayed, otherwise returns <c>false</c>.</returns>
	private bool _ComboShowsImages()
	{
		IListItem? item = _GetTextItem();
		return (_ShowsImages || _HasFallbackImages || ((item is not null) && _HasImage(item)));
	}


	/// <summary>Positions the text box and updates its colors.</summary>
	private void _LayoutEditor()
	{
		if((_Editor is null) || IsDisposed) { return; }

		Rectangle bounds = _GetTextBounds(null, _GetTextItem(), _ComboShowsImages(), out _);
		int height = _Editor.PreferredHeight;

		_Editor.Bounds = new Rectangle(bounds.X, Math.Max(1, (Height - height) / 2), Math.Max(1, bounds.Width), height);
		_UpdateEditorColors();
	}


	/// <summary>Updates the colors of the text box.</summary>
	private void _UpdateEditorColors()
	{
		if(_Editor is null) { return; }

		Color back = _GetBackColor();
		Color fore = (_ReadOnly ? _Color(Slot.READ_ONLY_FORE_COLOR) : _Color(Slot.FORE_COLOR));

		if(_Editor.BackColor != back) { _Editor.BackColor = back; }
		if(_Editor.ForeColor != fore) { _Editor.ForeColor = fore; }
	}


	/// <summary>Gets the index that the item list selects when it opens in combo mode.</summary>
	/// <returns>Returns the selected index, otherwise the index of the item whose text equals the text, otherwise -1.</returns>
	private int _GetComboListIndex()
	{
		if(_SelectedIndex >= 0) { return _SelectedIndex; }
		return ((_Editor is null) ? -1 : FindStringExact(_Editor.Text));
	}


	/// <summary>Cancels a running or pending search.</summary>
	private void _CancelSearch()
	{
		_SearchVersion++;
		_SearchTimer?.Stop();

		if(_SearchCancellation is not null)
		{
			_SearchCancellation.Cancel();
			_SearchCancellation.Dispose();
			_SearchCancellation = null;
		}
	}


	/// <summary>Starts a search for the typed text, after <see cref="SearchDelay"/> if a delay is set.</summary>
	private void _ScheduleSearch()
	{
		if(_SearchDelay <= 0)
		{
			_StartSearch(false);
			return;
		}

		_CancelSearch();

		if(_SearchTimer is null)
		{
			_SearchTimer = new System.Windows.Forms.Timer();
			_SearchTimer.Tick += _SearchTimer_Tick;
		}

		_SearchTimer.Interval = _SearchDelay;
		_SearchTimer.Start();
	}


	/// <summary>Shows the list with the loading indicator.</summary>
	private void _ShowLoading()
	{
		_ListPopupWindow<IListItem> popup = _GetPopup();
		bool images = (_ShowsImages || _HasFallbackImages);
		Point origin = PointToScreen(Point.Empty);

		_SearchResults = [];
		_SearchIndices = null;

		popup.ApplyStyle(Font, _ImageList, _GetPopupStyle(), false);
		popup.LoadingText = _LoadingText;
		popup.SetItems([], images, string.Empty, ListMatchMode.PREFIX, false, -1);
		popup.SetLoading();
		popup.ShowAt(new Point(origin.X + popup.GetTextOffset(images), origin.Y), Height, new Size(Math.Max(Width, _DropDownWidth), popup.GetWindowHeight(1)), FindForm());

		_SetDroppedDown(true);
	}


	/// <summary>Starts a search for the text of the text box.</summary>
	/// <param name="explicitStart">Determines if the user has requested the search (Ctrl+Space); then the search starts regardless of
	///                             the minimum length.</param>
	private void _StartSearch(bool explicitStart)
	{
		if((_Editor is null) || _ReadOnly || (!Enabled)) { return; }

		string text = _Editor.Text;

		_CancelSearch();
		_SourceItems = null;

		if((!explicitStart) && (text.Length < _MinimumSearchLength))
		{
			if(_DroppedDown) { _CloseDropDown(false); }
			return;
		}

		if(!_CanDropDown) { return; }

		if(_SearchSource is null)
		{
			_ShowSearchResults(_GetViews(), true, text, _MatchMode, _FilterMode, _CaseSensitive);
			return;
		}

		int version = _SearchVersion;
		AutoCompletionContext context = new(_Editor, text, text.Length, 0, string.Empty, (explicitStart ? AutoCompletionTrigger.SHORTCUT : AutoCompletionTrigger.TYPING), '\0');
		ValueTask<AutoCompletionResult?> task;

		_SearchCancellation = new CancellationTokenSource();
		task = _SearchSource.GetItemsAsync(context, _SearchCancellation.Token);

		if(task.IsCompletedSuccessfully)
		{
			_ApplySourceResult(task.Result, text);
			return;
		}

		if(!_DroppedDown) { _ShowLoading(); }
		_AwaitSourceResult(task, text, version);
	}


	/// <summary>Waits for an asynchronous result of the search source.</summary>
	/// <param name="task">Pending result.</param>
	/// <param name="text">Searched text.</param>
	/// <param name="version">Search version.</param>
	private async void _AwaitSourceResult(ValueTask<AutoCompletionResult?> task, string text, int version)
	{
		AutoCompletionResult? result;

		try
		{
			result = await task;
		}
		catch(OperationCanceledException) { return; }

		if((version != _SearchVersion) || IsDisposed || (_Editor is null)) { return; }
		_ApplySourceResult(result, text);
	}


	/// <summary>Shows the result of the search source.</summary>
	/// <param name="result">Result.</param>
	/// <param name="text">Searched text.</param>
	private void _ApplySourceResult(AutoCompletionResult? result, string text)
	{
		List<IListItem> items = ((result is null) ? new List<IListItem>() : result.Items.Cast<IListItem>().ToList());

		_SourceItems = items;
		_ShowSearchResults(items, false, text, (result?.MatchMode ?? _MatchMode), ListFilterMode.FILTER, (result?.CaseSensitive ?? _CaseSensitive));
	}


	/// <summary>Shows search results.</summary>
	/// <param name="candidates">Candidate items.</param>
	/// <param name="fromItems">Determines if the candidates are the items of the control.</param>
	/// <param name="pattern">Search pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="filterMode">Filter mode.</param>
	/// <param name="caseSensitive">Determines if the search is case-sensitive.</param>
	private void _ShowSearchResults(IReadOnlyList<IListItem> candidates, bool fromItems, string pattern, ListMatchMode mode, ListFilterMode filterMode, bool caseSensitive)
	{
		if(fromItems && (filterMode == ListFilterMode.SELECT))
		{
			int best = _FindBest(candidates, pattern, mode, caseSensitive);

			_SearchResults = null;
			_SearchIndices = null;
			_ShowPopupItems(_GetViews(), pattern, mode, caseSensitive, ((best >= 0) ? best : _GetComboListIndex()));
			_SetDroppedDown(true);
			return;
		}

		List<int> ranked = _Rank(candidates, pattern, mode, caseSensitive);

		if(ranked.Count == 0)
		{
			_SearchResults = null;
			_SearchIndices = null;
			_CloseDropDown(false);
			return;
		}

		_SearchResults = ranked.Select(m => candidates[m]).ToList();
		_SearchIndices = (fromItems ? ranked : null);
		_ShowPopupItems(_SearchResults, pattern, mode, caseSensitive, 0);
		_SetDroppedDown(true);
	}


	/// <summary>Selects a search result.</summary>
	/// <param name="index">Index in the search results.</param>
	private void _ChooseSearchResult(int index)
	{
		if((_SearchResults is null) || (index < 0) || (index >= _SearchResults.Count)) { return; }

		if(_SearchIndices is not null)
		{
			int itemIndex = _SearchIndices[index];
			if(itemIndex < _Items.Count) { _SetSelection(itemIndex, _Items[itemIndex], true); }
		}
		else { _SetSelection(-1, _SearchResults[index], true); }

		if(_Editor is not null) { _Editor.Select(_Editor.TextLength, 0); }
	}


	/// <summary>Applies the text of the text box: selects the item with this text or accepts it as free text.</summary>
	/// <param name="user">Determines if the user has entered the text.</param>
	private void _CommitText(bool user)
	{
		if(_Editor is null) { return; }

		string text = _Editor.Text;

		if((_SelectedItem is not null) && (GetItemText(_SelectedItem) == text)) { return; }

		int index = FindStringExact(text);
		if(index >= 0)
		{
			_SetSelection(index, _Items[index], user);
			return;
		}

		IListItem? sourceItem = ((text.Length == 0) ? null : _SourceItems?.FirstOrDefault(m => string.Equals(m.Text, text, StringComparison.CurrentCultureIgnoreCase)));
		if(sourceItem is not null)
		{
			_SetSelection(-1, sourceItem, user);
			return;
		}

		if(_AllowFreeText || (text.Length == 0))
		{
			_SetSelection(-1, null, user, false);
		}
		else { _UpdateText(); }
	}


	/// <summary>Processes a key of the combo mode.</summary>
	/// <param name="keyData">Key and modifiers.</param>
	/// <returns>Returns <c>true</c> if the key has been processed, otherwise returns <c>false</c>.</returns>
	private bool _ProcessComboKey(Keys keyData)
	{
		if(_ReadOnly || (_Editor is null)) { return false; }

		if((keyData == Keys.F4) || (keyData == (Keys.Alt | Keys.Down)) || (keyData == (Keys.Alt | Keys.Up)))
		{
			if(_DroppedDown)
			{
				_CloseDropDown(true);
			}
			else { _OpenDropDown(); }
			return true;
		}

		if(keyData == (Keys.Control | Keys.Space))
		{
			_StartSearch(true);
			return true;
		}

		if(_DroppedDown && (_Popup is not null))
		{
			int page = Math.Max(1, _Popup.VisibleRowCount - 1);

			switch(keyData)
			{
				case Keys.Up: _Popup.MoveSelection(-1); return true;
				case Keys.Down: _Popup.MoveSelection(1); return true;
				case Keys.PageUp: _Popup.MoveSelection(-page); return true;
				case Keys.PageDown: _Popup.MoveSelection(page); return true;
				case Keys.Enter: _CloseDropDown(true); return true;
				case Keys.Escape: _CloseDropDown(false); return true;
			}
			return false;
		}

		if(_Items.Count == 0) { return false; }

		int index = _GetComboListIndex();
		int pageSize = Math.Max(1, _MaxDropDownItems - 1);
		int target;

		switch(keyData)
		{
			case Keys.Up: target = index - 1; break;
			case Keys.Down: target = index + 1; break;
			case Keys.PageUp: target = index - pageSize; break;
			case Keys.PageDown: target = Math.Max(0, index) + pageSize; break;
			default: return false;
		}

		target = Math.Clamp(target, 0, _Items.Count - 1);
		if((target != _SelectedIndex) || (GetItemText(_Items[target]) != _Editor.Text)) { _SetSelection(target, _Items[target], true); }
		_Editor.SelectAll();

		return true;
	}


	/// <summary>Shows or hides the description of the item selected in the list.</summary>
	private void _UpdateDescription()
	{
		IListItem? item = (_DroppedDown ? _Popup?.SelectedItem : null);

		if((_Popup is null) || (item is null) || (!_ShowDescriptions) || string.IsNullOrEmpty(item.Description))
		{
			_Description?.HideWindow();
			return;
		}

		_Description ??= new _ListDescriptionWindow();
		_Description.BorderColor = _Popup.BorderColor;
		_Description.ShowFor(_Popup.Bounds, item.Description, Font, _Popup.BackColor, _Popup.ForeColor, LogicalToDeviceUnits(_DESCRIPTION_WIDTH), FindForm());
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="ResolveImage"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnResolveImage(ThemedComboBoxImageEventArgs e)
	{
		_ResolveImage?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text box of the combo mode, NULL in list mode.</summary>
	internal TextBox? _TextBox
	{
		get { return _Editor; }
	}


	/// <summary>Gets if a delayed search is pending.</summary>
	internal bool _SearchPending
	{
		get { return (_SearchTimer?.Enabled ?? false); }
	}


	/// <summary>Gets the items displayed in the search list, NULL if the list shows the items of the control.</summary>
	internal IReadOnlyList<IListItem>? _DisplayedSearchResults
	{
		get { return _SearchResults; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the image displayed in combo mode.</summary>
	/// <returns>Returns the image of the item that matches the text, otherwise the image provided by <see cref="ResolveImage"/>,
	///          otherwise <see cref="DefaultImage"/>. Returns NULL if there is no image or an item image is drawn from the image list.</returns>
	internal Image? _GetComboImage()
	{
		IListItem? item = _GetTextItem();

		if((item is not null) && _HasImage(item)) { return item.Image; }
		return _GetFallbackImage();
	}



	/// <summary>Runs a pending delayed search immediately (for tests).</summary>
	internal void _RunPendingSearch()
	{
		if(!_SearchPending) { return; }

		_SearchTimer!.Stop();
		_StartSearch(false);
	}


	/// <summary>Gets the list items of all items (for tests).</summary>
	/// <returns>Returns the list items.</returns>
	internal IReadOnlyList<IListItem> _GetViewsForTest()
	{
		return _GetViews();
	}


	/// <summary>Gets the image displayed for a list item without image (for tests).</summary>
	/// <param name="item">List item.</param>
	/// <returns>Returns the image.</returns>
	internal Image? _ResolveItemImageForTest(IListItem item)
	{
		return _ResolveItemImage(item, true);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when an item without image is displayed, or in combo mode when the text matches no item with an image; the event
	///          handler can provide an image.</summary>
	/// <remarks>If no image is provided, <see cref="DefaultImage"/> is displayed. The images of items are requested when the items are
	///          drawn and cached until the items change. If the event is handled, the image column is always displayed.</remarks>
	[Category("Behavior")][Description("Occurs when an item or the text has no image; the event handler can provide an image.")]
	public event EventHandler<ThemedComboBoxImageEventArgs>? ResolveImage
	{
		add
		{
			_ResolveImage += value;
			_ClearImageCache();
		}
		remove
		{
			_ResolveImage -= value;
			_ClearImageCache();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the drop-down style.</summary>
	/// <remarks>In list mode, the user selects items from the list. In combo mode, the user can type text; typing searches the items or
	///          the <see cref="SearchSource"/>.</remarks>
	[Category("Appearance")][Description("Determines if the user selects items from a list or can type text.")][DefaultValue(ThemedComboBoxStyle.LIST)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public ThemedComboBoxStyle DropDownStyle
	{
		get { return _DropDownStyle; }
		set
		{
			if(value == _DropDownStyle) { return; }

			_CloseDropDown(false);
			_DropDownStyle = value;

			if(value == ThemedComboBoxStyle.COMBO)
			{
				_CreateEditor();
			}
			else
			{
				_RemoveEditor();
				_UpdateText();
			}

			Invalidate();
		}
	}


	/// <summary>Gets or sets the source that is searched while typing in combo mode.</summary>
	/// <remarks>If the source is NULL, typing searches <see cref="Items"/>. The drop-down button always shows <see cref="Items"/>. Choosing
	///          a result of the source makes it the <see cref="SelectedItem"/>; <see cref="SelectedIndex"/> is -1 then.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IAutoCompletionSource? SearchSource
	{
		get { return _SearchSource; }
		set
		{
			_CancelSearch();
			_SearchSource = value;
			_SourceItems = null;
		}
	}


	/// <summary>Gets or sets how items are matched against the typed text in combo mode.</summary>
	[Category("Behavior")][Description("Determines how items are matched against the typed text in combo mode.")][DefaultValue(ListMatchMode.FUZZY)]
	public ListMatchMode MatchMode
	{
		get { return _MatchMode; }
		set { _MatchMode = value; }
	}


	/// <summary>Gets or sets how the list reacts to the typed text in combo mode.</summary>
	/// <remarks><see cref="ListFilterMode.FILTER"/> shows the matching items only, best matches first. <see cref="ListFilterMode.SELECT"/>
	///          shows all items and selects the best match; it applies when <see cref="Items"/> are searched, results of the
	///          <see cref="SearchSource"/> are always filtered.</remarks>
	[Category("Behavior")][Description("Determines how the list reacts to the typed text in combo mode.")][DefaultValue(ListFilterMode.FILTER)]
	public ListFilterMode FilterMode
	{
		get { return _FilterMode; }
		set { _FilterMode = value; }
	}


	/// <summary>Gets or sets if the search is case-sensitive.</summary>
	[Category("Behavior")][Description("Determines if the search is case-sensitive.")][DefaultValue(false)]
	public bool CaseSensitive
	{
		get { return _CaseSensitive; }
		set { _CaseSensitive = value; }
	}


	/// <summary>Gets or sets how matched characters are highlighted in the list.</summary>
	[Category("Appearance")][Description("Determines how matched characters are highlighted in the list.")][DefaultValue(ListHighlightStyle.BOLD)]
	public ListHighlightStyle HighlightStyle
	{
		get { return _HighlightStyle; }
		set { _HighlightStyle = value; }
	}


	/// <summary>Gets or sets the number of characters that have to be typed before the search list opens.</summary>
	/// <remarks>Ctrl+Space opens the search list regardless of the length.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
	[Category("Behavior")][Description("The number of characters that have to be typed before the search list opens.")][DefaultValue(1)]
	public int MinimumSearchLength
	{
		get { return _MinimumSearchLength; }
		set
		{
			if(value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_MinimumSearchLength = value;
		}
	}


	/// <summary>Gets or sets the delay in milliseconds between typing and searching in combo mode.</summary>
	/// <remarks>A delay avoids requests to a slow <see cref="SearchSource"/> for every typed character. 0 searches immediately.
	///          Ctrl+Space searches immediately regardless of the delay.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
	[Category("Behavior")][Description("The delay in milliseconds between typing and searching.")][DefaultValue(0)]
	public int SearchDelay
	{
		get { return _SearchDelay; }
		set
		{
			if(value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_SearchDelay = value;
		}
	}


	/// <summary>Gets or sets the text displayed while an asynchronous search source is loading.</summary>
	[Category("Appearance")][Description("The text displayed while the search source is loading.")][DefaultValue("Loading...")]
	public string LoadingText
	{
		get { return _LoadingText; }
		set { _LoadingText = (value ?? string.Empty); }
	}


	/// <summary>Gets or sets if the user can enter text that matches no item in combo mode.</summary>
	/// <remarks>If free text is not allowed, the text returns to the text of the selected item when the user leaves the control.</remarks>
	[Category("Behavior")][Description("Determines if the user can enter text that matches no item.")][DefaultValue(true)]
	public bool AllowFreeText
	{
		get { return _AllowFreeText; }
		set { _AllowFreeText = value; }
	}


	/// <summary>Gets or sets the image displayed for items without image and in combo mode for text that matches no item with an image.</summary>
	/// <remarks>An image provided by <see cref="ResolveImage"/> takes precedence. If a default image is set, the image column is always
	///          displayed, so texts are aligned whether they have an image or not.</remarks>
	[Category("Appearance")][Description("The image displayed for items and texts without image.")][DefaultValue(null)]
	public Image? DefaultImage
	{
		get { return _DefaultImage; }
		set
		{
			if(value == _DefaultImage) { return; }

			_DefaultImage = value;
			_ClearImageCache();
		}
	}


	/// <summary>Gets or sets if the description of the item selected in the list is shown next to the list.</summary>
	[Category("Behavior")][Description("Determines if the description of the item selected in the list is shown next to the list.")][DefaultValue(false)]
	public bool ShowDescriptions
	{
		get { return _ShowDescriptions; }
		set
		{
			_ShowDescriptions = value;
			_UpdateDescription();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the TextChanged event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Editor_TextChanged(object? sender, EventArgs e)
	{
		if(_Editor is null) { return; }

		if(base.Text != _Editor.Text) { base.Text = _Editor.Text; }
		_InvalidateTextItem();

		if(!_SettingEditorText) { _ScheduleSearch(); }
	}


	/// <summary>Handles the Tick event of the search timer.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _SearchTimer_Tick(object? sender, EventArgs e)
	{
		_SearchTimer?.Stop();
		_StartSearch(false);
	}


	/// <summary>Handles the GotFocus and LostFocus events of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Editor_FocusChanged(object? sender, EventArgs e)
	{
		Invalidate();
	}


	/// <summary>Handles the MouseEnter event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Editor_MouseEnter(object? sender, EventArgs e)
	{
		if(_Hot && (!_ButtonHot)) { return; }

		_Hot = true;
		_ButtonHot = false;
		Invalidate();
	}


	/// <summary>Handles the MouseLeave event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Editor_MouseLeave(object? sender, EventArgs e)
	{
		if(ClientRectangle.Contains(PointToClient(MousePosition))) { return; }

		_Hot = _ButtonHot = false;
		Invalidate();
	}


	/// <summary>Handles the MouseWheel event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Editor_MouseWheel(object? sender, MouseEventArgs e)
	{
		_ProcessWheel(e);
	}


	/// <summary>Handles the SelectionChanged event of the drop-down list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_SelectionChanged(object? sender, EventArgs e)
	{
		if(_DroppedDown) { _UpdateDescription(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _EditBox                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the text box of the combo mode.</summary>
	/// <remarks>The text box forwards its keys to the combo box, so key events of the combo box are raised in combo mode as well, and
	///          the combo box can process the list keys.</remarks>
	private sealed class _EditBox: TextBox
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
		public _EditBox(ThemedComboBox owner): base()
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] TextBox                                                                                           //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		/// <remarks>With <see cref="Themes.ThemeManager.ThemeTextSelection"/>, the selection is drawn in the selection colors of the
		///          theme.</remarks>
		protected override void WndProc(ref Message m)
		{
			Themes._SysColorHook._Run(ref m, Handle, _Owner.Colors.EffectiveTheme, base.WndProc);
		}


		/// <inheritdoc/>
		/// <remarks>The margins are removed, so the text is aligned with the text of the list.</remarks>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			_SendMessage(Handle, _EM_SETMARGINS, (IntPtr) (_EC_LEFTMARGIN | _EC_RIGHTMARGIN), IntPtr.Zero);
		}


		/// <inheritdoc/>
		/// <remarks>Setting the font restores the default margins, so they are removed again.</remarks>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			if(IsHandleCreated) { _SendMessage(Handle, _EM_SETMARGINS, (IntPtr) (_EC_LEFTMARGIN | _EC_RIGHTMARGIN), IntPtr.Zero); }
		}


		/// <inheritdoc/>
		protected override bool IsInputKey(Keys keyData)
		{
			if((keyData == (Keys.Alt | Keys.Down)) || (keyData == (Keys.Alt | Keys.Up))) { return true; }
			if(_Owner._DroppedDown && ((keyData == Keys.Enter) || (keyData == Keys.Escape))) { return true; }

			return base.IsInputKey(keyData);
		}


		/// <inheritdoc/>
		/// <remarks>Enter applies the text before the form processes it (accept button); Tab selects the item selected in the list.</remarks>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			Keys key = (keyData & Keys.KeyCode);

			if((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
			{
				if(_Owner._DroppedDown && (key == Keys.Tab))
				{
					_Owner._CloseDropDown(true);
				}
				else if((!_Owner._DroppedDown) && (key == Keys.Enter)) { _Owner._CommitText(true); }
			}

			return base.ProcessDialogKey(keyData);
		}


		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			_Owner.OnKeyDown(e);
			if(e.Handled) { return; }

			base.OnKeyDown(e);
		}


		/// <inheritdoc/>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			_Owner.OnKeyPress(e);
			if(e.Handled) { return; }

			base.OnKeyPress(e);
		}


		/// <inheritdoc/>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			_Owner.OnKeyUp(e);
			if(e.Handled) { return; }

			base.OnKeyUp(e);
		}
	}
}
