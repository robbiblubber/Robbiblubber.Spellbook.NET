namespace Robbiblubber.Spellbook.Controls;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



partial class AutoCompletionProvider
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Binding                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents the auto completion binding of a control and manages its auto completion sessions.</summary>
	/// <remarks>A session runs through the states <see cref="_SessionState.NONE"/>, <see cref="_SessionState.PENDING"/> (items are
	///          being requested) and <see cref="_SessionState.ACTIVE"/> (items are available and filtered locally). Each request is
	///          stamped with a version number; results of outdated requests are discarded.</remarks>
	private class _Binding: IMessageFilter, IDisposable
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private constants                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Delay in milliseconds before the loading indicator is shown.</summary>
		private const int _LOADING_DELAY = 150;

		/// <summary>Maximum number of items measured to determine the window width.</summary>
		private const int _AUTOSIZE_SAMPLE = 200;

		/// <summary>Maximum width of the description window in logical units.</summary>
		private const int _DESCRIPTION_WIDTH = 320;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Parent provider.</summary>
		private readonly AutoCompletionProvider _Parent;

		/// <summary>Enabled flag.</summary>
		private bool _Enabled = false;

		/// <summary>Determines if the control events are hooked.</summary>
		private bool _Hooked = false;

		/// <summary>Disposed flag.</summary>
		private bool _Disposed = false;

		/// <summary>Unsorted items of the last result, used to detect repeated results.</summary>
		private IAutoCompletionItem[]? _SortCacheInput = null;

		/// <summary>Sorted items of the last result.</summary>
		private IAutoCompletionItem[]? _SortCacheOutput = null;

		/// <summary>Pattern of the last filter run, NULL if the next run has to check all items.</summary>
		private string? _LastFilterPattern = null;

		/// <summary>Indices of the items that matched in the last filter run, in ascending order.</summary>
		private int[] _LastFilterIndices = [];

		/// <summary>Determines if the current request is waiting for the typing delay to elapse.</summary>
		private bool _Delaying = false;

		/// <summary>Determines if the binding is registered as a message filter.</summary>
		private bool _Filtering = false;

		/// <summary>Session state.</summary>
		private _SessionState _State = _SessionState.NONE;

		/// <summary>Context of the current session.</summary>
		private AutoCompletionContext? _Context = null;

		/// <summary>Trigger of the current session.</summary>
		private AutoCompletionTrigger _Trigger = AutoCompletionTrigger.TYPING;

		/// <summary>Start index of the replaced text range.</summary>
		private int _ReplaceStart = 0;

		/// <summary>Length of the replaced text range as defined by the result.</summary>
		private int? _ReplaceLength = null;

		/// <summary>Caret index at the time of the request.</summary>
		private int _RequestCaret = 0;

		/// <summary>Sorted session items.</summary>
		private IAutoCompletionItem[] _Items = [];

		/// <summary>Displayed items.</summary>
		private readonly List<IAutoCompletionItem> _Filtered = new();

		/// <summary>Determines if the session items have images.</summary>
		private bool _HasImages = false;

		/// <summary>Determines if the session result is incomplete.</summary>
		private bool _IsIncomplete = false;

		/// <summary>Effective match mode of the session.</summary>
		private ListMatchMode _SessionMatchMode = ListMatchMode.PREFIX;

		/// <summary>Effective filter mode of the session.</summary>
		private ListFilterMode _SessionFilterMode = ListFilterMode.FILTER;

		/// <summary>Effective case sensitivity of the session.</summary>
		private bool _SessionCaseSensitive = false;

		/// <summary>Current filter pattern.</summary>
		private string _Pattern = string.Empty;

		/// <summary>Number of items that start with the pattern.</summary>
		private int _PrefixMatchCount = 0;

		/// <summary>Index of the item closest to the pattern in <see cref="ListFilterMode.SELECT"/> mode.</summary>
		private int _NearestIndex = -1;

		/// <summary>Determines if the user has selected an item explicitly.</summary>
		private bool _UserNavigated = false;

		/// <summary>Request version.</summary>
		private int _Version = 0;

		/// <summary>Cancellation token source of the current request.</summary>
		private CancellationTokenSource? _Cancellation = null;

		/// <summary>Determines if a key has been typed and the next text change results from it.</summary>
		private bool _KeyPending = false;

		/// <summary>Typed key character, '\b' for deletions.</summary>
		private char _KeyChar = '\0';

		/// <summary>Determines if an item is being inserted.</summary>
		private bool _Accepting = false;

		/// <summary>Start index of the word for which auto completion has been cancelled, -1 if auto start is not suppressed.</summary>
		private int _SuppressedWordStart = -1;

		/// <summary>Caret index after the last keystroke in the suppressed word.</summary>
		private int _SuppressedCaret = -1;

		/// <summary>Form the control belongs to while a session is visible.</summary>
		private Form? _Form = null;

		/// <summary>Tracking suspension counter.</summary>
		private int _TrackingSuspended = 0;

		/// <summary>Control text after the last processed change, used to ignore formatting-only changes.</summary>
		private string? _LastText = null;

		/// <summary>Window size chosen by the user in device units, empty if the user has not resized the window.</summary>
		private Size _UserBoxSize = Size.Empty;

		/// <summary>Window width of the current session; the window only grows within a session.</summary>
		private int _SessionBoxWidth = 0;

		/// <summary>Item reported by the last selection change notification.</summary>
		private IAutoCompletionItem? _NotifiedItem = null;

		/// <summary>Determines if the next result comes from an explicit start (shortcut keys or code).</summary>
		private bool _ExplicitPending = false;

		/// <summary>Usage counts of the session items, aligned with the sorted items; NULL if usage ranking does not apply.</summary>
		private int[]? _Usage = null;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="parent">Parent provider.</param>
		/// <param name="control">Control.</param>
		public _Binding(AutoCompletionProvider parent, TextBoxBase control)
		{
			_Parent = parent;
			Control = control;

			Control.Disposed += Control_Disposed;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private properties                                                                                           //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the effective word characters.</summary>
		private string _WordCharacters
		{
			get { return (WordCharacters ?? _Parent.WordCharacters ?? string.Empty); }
		}


		/// <summary>Gets the effective trigger characters.</summary>
		private string _TriggerCharacters
		{
			get { return (TriggerCharacters ?? _Parent.TriggerCharacters ?? string.Empty); }
		}


		/// <summary>Gets the effective commit characters.</summary>
		private string _CommitCharacters
		{
			get { return (CommitCharacters ?? _Parent.CommitCharacters ?? string.Empty); }
		}


		/// <summary>Gets the effective window size.</summary>
		private Size _BoxSize
		{
			get { return (BoxSize.IsEmpty ? _Parent.BoxSize : BoxSize); }
		}


		/// <summary>Gets if the window width adjusts to the items.</summary>
		private bool _BoxAutoSize
		{
			get { return (BoxAutoSize ?? _Parent.BoxAutoSize); }
		}


		/// <summary>Gets if the window can be resized by the user.</summary>
		private bool _BoxResizable
		{
			get { return (BoxResizable ?? _Parent.BoxResizable); }
		}


		/// <summary>Gets if usage ranking is enabled.</summary>
		private bool _UsageRanking
		{
			get { return (UsageRanking ?? _Parent.UsageRanking); }
		}


		/// <summary>Gets the usage history scope of the control.</summary>
		private string _Scope
		{
			get { return (Control.Name ?? string.Empty); }
		}


		/// <summary>Gets the popup window if it is visible for this binding, otherwise NULL.</summary>
		private _ListPopupWindow<IAutoCompletionItem>? _VisiblePopup
		{
			get
			{
				if((_State == _SessionState.NONE) || (_Parent._ActiveBinding != this)) { return null; }
				if((_Parent._Popup is null) || _Parent._Popup.IsDisposed || (!_Parent._Popup.Visible)) { return null; }

				return _Parent._Popup;
			}
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private methods                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Hooks the control events.</summary>
		private void _Hook()
		{
			if(_Hooked || _Parent.DesignMode) { return; }

			Control.PreviewKeyDown += Control_PreviewKeyDown;
			Control.KeyDown += Control_KeyDown;
			Control.KeyPress += Control_KeyPress;
			Control.TextChanged += Control_TextChanged;
			Control.MouseDown += Control_MouseDown;
			Control.MouseWheel += Control_MouseWheel;
			Control.LostFocus += Control_LostFocus;

			_Hooked = true;
		}


		/// <summary>Unhooks the control events.</summary>
		private void _Unhook()
		{
			if(!_Hooked) { return; }

			Control.PreviewKeyDown -= Control_PreviewKeyDown;
			Control.KeyDown -= Control_KeyDown;
			Control.KeyPress -= Control_KeyPress;
			Control.TextChanged -= Control_TextChanged;
			Control.MouseDown -= Control_MouseDown;
			Control.MouseWheel -= Control_MouseWheel;
			Control.LostFocus -= Control_LostFocus;

			_Hooked = false;
		}


		/// <summary>Hooks the events of the form the control belongs to.</summary>
		/// <param name="form">Form.</param>
		private void _HookForm(Form? form)
		{
			if(form == _Form) { return; }

			_UnhookForm();

			if(form is not null)
			{
				form.Deactivate += _Form_Deactivate;
				form.Move += _Form_Move;
				form.Resize += _Form_Move;
				_Form = form;
			}
		}


		/// <summary>Unhooks the events of the form the control belongs to.</summary>
		private void _UnhookForm()
		{
			if(_Form is null) { return; }

			_Form.Deactivate -= _Form_Deactivate;
			_Form.Move -= _Form_Move;
			_Form.Resize -= _Form_Move;
			_Form = null;
		}


		/// <summary>Returns if a character is a word character.</summary>
		/// <param name="c">Character.</param>
		/// <returns>Returns <c>true</c> if the character is a word character, otherwise returns <c>false</c>.</returns>
		private bool _IsWordCharacter(char c)
		{
			return (char.IsLetterOrDigit(c) || _WordCharacters.Contains(c));
		}


		/// <summary>Gets the start index of the word preceding a position.</summary>
		/// <param name="text">Text.</param>
		/// <param name="caret">Position.</param>
		/// <returns>Returns the word start index.</returns>
		private int _GetWordStart(string text, int caret)
		{
			int rval = caret;

			while((rval > 0) && _IsWordCharacter(text[rval - 1])) { rval--; }
			return rval;
		}


		/// <summary>Gets the qualifier of a word.</summary>
		/// <param name="text">Text.</param>
		/// <param name="wordStart">Word start index.</param>
		/// <returns>Returns the qualifier.</returns>
		private string _GetQualifier(string text, int wordStart)
		{
			if((wordStart == 0) || (text[wordStart - 1] != '.')) { return string.Empty; }

			int end = wordStart - 1;
			int start = end;

			while((start > 0) && (_IsWordCharacter(text[start - 1]) || (text[start - 1] == '.'))) { start--; }
			return text.Substring(start, end - start);
		}


		/// <summary>Gets the caret index clamped to the text length.</summary>
		/// <param name="text">Control text.</param>
		/// <returns>Returns the caret index.</returns>
		private int _GetCaret(string text)
		{
			return Math.Min(Control.SelectionStart, text.Length);
		}


		/// <summary>Creates a context for the current control state.</summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="triggerCharacter">Trigger character.</param>
		/// <returns>Returns the context.</returns>
		private AutoCompletionContext _CreateContext(AutoCompletionTrigger trigger, char triggerCharacter)
		{
			string text = Control.Text;
			int caret = _GetCaret(text);
			int wordStart = _GetWordStart(text, caret);

			return new AutoCompletionContext(Control, text, caret, wordStart, _GetQualifier(text, wordStart), trigger, triggerCharacter);
		}


		/// <summary>Returns if the session continues after a text change.</summary>
		/// <param name="c">Typed character.</param>
		/// <param name="caret">Caret index.</param>
		/// <param name="wordStart">Word start index.</param>
		/// <returns>Returns <c>true</c> if the session continues, otherwise returns <c>false</c>.</returns>
		private bool _IsInSession(char c, int caret, int wordStart)
		{
			if(_Context is null) { return false; }
			if((c != '\b') && (!_IsWordCharacter(c))) { return false; }

			return ((wordStart == _Context.WordStart) && (caret >= _Context.WordStart));
		}


		/// <summary>Configures and positions the popup window.</summary>
		/// <param name="popup">Popup window.</param>
		/// <param name="text">Control text.</param>
		/// <remarks>Window size: the size chosen by the user, if any; otherwise the measured width (only growing within a session) if
		///          auto size is enabled; otherwise the box size. Minimum and maximum size apply to the user size and the measured size.</remarks>
		private void _Position(_ListPopupWindow<IAutoCompletionItem> popup, string text)
		{
			Form? form = Control.FindForm();
			Point position = _TextBoxUtility._GetCharacterPosition(Control, text, Math.Min(_ReplaceStart, text.Length));
			Size minimum = Control.LogicalToDeviceUnits(_Parent.MinimumBoxSize);
			Size maximum = Control.LogicalToDeviceUnits(_Parent.MaximumBoxSize);
			Size size;

			maximum = new Size(Math.Max(maximum.Width, minimum.Width), Math.Max(maximum.Height, minimum.Height));

			if(!_UserBoxSize.IsEmpty)
			{
				size = _UserBoxSize;
			}
			else if(_BoxAutoSize)
			{
				_SessionBoxWidth = Math.Max(_SessionBoxWidth, popup.MeasurePreferredWidth(_Filtered, _HasImages, _AUTOSIZE_SAMPLE));
				size = new Size(_SessionBoxWidth, maximum.Height);
			}
			else
			{
				size = Control.LogicalToDeviceUnits(_BoxSize);
				maximum = new Size(Math.Max(maximum.Width, size.Width), Math.Max(maximum.Height, size.Height));
			}

			popup.SetSizeLimits(minimum, maximum);
			popup.ShowAt(Control.PointToScreen(position), Control.Font.Height, size, form);

			_HookForm(form);
			UpdateSelection();
		}


		/// <summary>Shows or hides the description of the selected item.</summary>
		private void _UpdateDescription()
		{
			_ListPopupWindow<IAutoCompletionItem>? popup = _VisiblePopup;
			IAutoCompletionItem? item = popup?.SelectedItem;

			if((popup is null) || (item is null) || (!_Parent.ShowDescriptions) || string.IsNullOrEmpty(item.Description))
			{
				_Parent._Description?.HideWindow();
				return;
			}

			_ListDescriptionWindow description = _Parent._GetDescriptionWindow();

			description.BorderColor = popup.BorderColor;
			description.ShowFor(popup.Bounds, item.Description, Control.Font, popup.BackColor, popup.ForeColor, Control.LogicalToDeviceUnits(_DESCRIPTION_WIDTH), Control.FindForm());
		}


		/// <summary>Raises the selection changed event if the selected item has changed.</summary>
		/// <param name="item">Selected item.</param>
		private void _NotifySelection(IAutoCompletionItem? item)
		{
			if(ReferenceEquals(item, _NotifiedItem)) { return; }

			_NotifiedItem = item;
			_Parent._OnSelectionChanged(new AutoCompletionSelectionChangedEventArgs(Control, item));
		}


		/// <summary>Prepares the popup window for display.</summary>
		/// <returns>Returns the popup window.</returns>
		private _ListPopupWindow<IAutoCompletionItem> _PreparePopup()
		{
			_ListPopupWindow<IAutoCompletionItem> rval = _Parent._GetPopup();

			rval.ApplyStyle(Control.Font, _Parent.ImageList, _Parent._GetPopupStyle(Control), _BoxResizable);
			rval.LoadingText = _Parent.LoadingText;

			return rval;
		}


		/// <summary>Filters the session items and updates the popup window.</summary>
		/// <param name="text">Control text.</param>
		/// <param name="caret">Caret index.</param>
		/// <param name="acceptSingleMatch">Determines if a single matching item is accepted directly instead of being shown.</param>
		private void _Refilter(string text, int caret, bool acceptSingleMatch = false)
		{
			int start = Math.Min(_ReplaceStart, text.Length);
			int selected = -1, matchCount = 0;
			int[]? usage = _Usage;
			bool explicitStart = ((_Trigger == AutoCompletionTrigger.SHORTCUT) || (_Trigger == AutoCompletionTrigger.MANUAL));

			_Pattern = ((caret > start) ? text.Substring(start, caret - start) : string.Empty);
			_Filtered.Clear();
			_PrefixMatchCount = 0;
			_NearestIndex = -1;
			_UserNavigated = false;

			if(_SessionFilterMode == ListFilterMode.FILTER)
			{
				List<(int Score, int Index)> matches = new();
				List<int> indices = new();
				bool narrowing = ((_LastFilterPattern is not null) && (_Pattern.Length > _LastFilterPattern.Length) && _Pattern.StartsWith(_LastFilterPattern, (_SessionCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)));
				int count = (narrowing ? _LastFilterIndices.Length : _Items.Length);

				for(int j = 0; j < count; j++)
				{
					int i = (narrowing ? _LastFilterIndices[j] : j);
					int score = _ListMatcher._Match(_Items[i].FilterText ?? _Items[i].Text, _Pattern, _SessionMatchMode, _SessionCaseSensitive);

					if(score < 0) { continue; }
					if(score >= _ListMatcher._SCORE_PREFIX) { _PrefixMatchCount++; }
					matches.Add((score, i));
					indices.Add(i);
				}

				_LastFilterPattern = _Pattern;
				_LastFilterIndices = indices.ToArray();

				if(_Pattern.Length > 0)
				{
					matches.Sort((m, n) =>
					{
						int rval = _ListMatcher._GetClass(n.Score).CompareTo(_ListMatcher._GetClass(m.Score));

						if((rval == 0) && (usage is not null)) { rval = usage[n.Index].CompareTo(usage[m.Index]); }
						if(rval == 0) { rval = n.Score.CompareTo(m.Score); }
						if(rval == 0) { rval = m.Index.CompareTo(n.Index); }

						return rval;
					});
				}

				foreach((int Score, int Index) i in matches) { _Filtered.Add(_Items[i.Index]); }

				matchCount = _Filtered.Count;
				if((_Filtered.Count > 0) && ((_Pattern.Length > 0) || explicitStart)) { selected = 0; }
			}
			else
			{
				int bestScore = -1, bestPrefix = 0;

				for(int i = 0; i < _Items.Length; i++)
				{
					string filterText = (_Items[i].FilterText ?? _Items[i].Text);
					int score = _ListMatcher._Match(filterText, _Pattern, _SessionMatchMode, _SessionCaseSensitive);

					if(score >= _ListMatcher._SCORE_PREFIX) { _PrefixMatchCount++; }
					if(score >= 0) { matchCount++; }

					if(_IsBetter(score, i, bestScore, selected, usage))
					{
						bestScore = score;
						selected = i;
					}
					else if((score < 0) && (bestScore < 0))
					{
						int prefix = _ListMatcher._CommonPrefixLength(filterText, _Pattern, _SessionCaseSensitive);
						if(prefix > bestPrefix)
						{
							bestPrefix = prefix;
							_NearestIndex = i;
						}
					}
				}

				_Filtered.AddRange(_Items);

				if((_Pattern.Length == 0) && (!explicitStart)) { selected = -1; }
			}

			if(acceptSingleMatch && _Parent.AcceptSingleMatch && (matchCount == 1) && (selected >= 0))
			{
				if(_Accept(_Filtered[selected])) { return; }
			}

			_ShowFiltered(text, selected);
		}


		/// <summary>Returns if a match is better than the best match so far (SELECT mode).</summary>
		/// <param name="score">Match score.</param>
		/// <param name="index">Item index.</param>
		/// <param name="bestScore">Score of the best match so far, negative if there is none.</param>
		/// <param name="bestIndex">Item index of the best match so far.</param>
		/// <param name="usage">Usage counts, NULL if usage ranking does not apply.</param>
		/// <returns>Returns <c>true</c> if the match is better, otherwise returns <c>false</c>.</returns>
		private bool _IsBetter(int score, int index, int bestScore, int bestIndex, int[]? usage)
		{
			int classDelta;

			if(score < 0) { return false; }
			if(bestScore < 0) { return true; }

			classDelta = _ListMatcher._GetClass(score) - _ListMatcher._GetClass(bestScore);
			if(classDelta != 0) { return (classDelta > 0); }

			if((usage is not null) && (usage[index] != usage[bestIndex])) { return (usage[index] > usage[bestIndex]); }

			return (score > bestScore);
		}


		/// <summary>Shows the filtered items in the popup window, or hides the window if there are no items.</summary>
		/// <param name="text">Control text.</param>
		/// <param name="selected">Selected index.</param>
		private void _ShowFiltered(string text, int selected)
		{
			if(_Filtered.Count == 0)
			{
				if(_Parent._ActiveBinding == this)
				{
					_Parent._Popup?.HideWindow();
					_Parent._Description?.HideWindow();
				}

				_NotifySelection(null);
				return;
			}

			_ListPopupWindow<IAutoCompletionItem> popup = _PreparePopup();
			popup.SetItems(_Filtered.ToArray(), _HasImages, _Pattern, _SessionMatchMode, _SessionCaseSensitive, selected);

			// the selection notification may have closed the session
			if((_State == _SessionState.NONE) || (_Parent._ActiveBinding != this)) { return; }
			_Position(popup, text);
		}


		/// <summary>Returns if the selected item can be accepted by a commit character.</summary>
		/// <param name="item">Selected item.</param>
		/// <returns>Returns <c>true</c> if the selection is unambiguous, otherwise returns <c>false</c>.</returns>
		private bool _IsUnambiguous(IAutoCompletionItem item)
		{
			string filterText = (item.FilterText ?? item.Text);

			if(_UserNavigated) { return true; }
			if(_Pattern.Length == 0) { return false; }
			if(string.Equals(filterText, _Pattern, (_SessionCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))) { return true; }

			return ((_PrefixMatchCount == 1) && filterText.StartsWith(_Pattern, (_SessionCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)));
		}


		/// <summary>Returns if a key accepts the selected item.</summary>
		/// <param name="key">Key code.</param>
		/// <param name="modifiers">Modifier keys.</param>
		/// <returns>Returns <c>true</c> if the key accepts the selected item, otherwise returns <c>false</c>.</returns>
		private bool _AcceptsWith(Keys key, Keys modifiers)
		{
			if((_VisiblePopup?.SelectedItem is null) || (modifiers != Keys.None)) { return false; }

			return (((key == Keys.Enter) && _Parent.AcceptOnEnter) || ((key == Keys.Tab) && _Parent.AcceptOnTab));
		}


		/// <summary>Moves the selection in the popup window.</summary>
		/// <param name="popup">Popup window.</param>
		/// <param name="delta">Number of rows to move.</param>
		private void _Navigate(_ListPopupWindow<IAutoCompletionItem> popup, int delta)
		{
			if(popup.SelectedIndex < 0)
			{
				popup.SelectedIndex = (((_SessionFilterMode == ListFilterMode.SELECT) && (_NearestIndex >= 0)) ? _NearestIndex : 0);
			}
			else { popup.MoveSelection(delta); }

			_UserNavigated = true;
		}


		/// <summary>Requests items from the source.</summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="triggerCharacter">Trigger character.</param>
		/// <param name="newSession">Determines if a new session is started.</param>
		private async void _Request(AutoCompletionTrigger trigger, char triggerCharacter, bool newSession)
		{
			CancellationTokenSource cancellation;
			AutoCompletionContext context;
			int version;

			if(_Disposed) { return; }

			_Cancellation?.Cancel();
			cancellation = _Cancellation = new CancellationTokenSource();
			version = ++_Version;
			context = _CreateContext(trigger, triggerCharacter);

			if(newSession)
			{
				AutoCompletionStartingEventArgs e = new(context);

				_Parent._Activate(this);
				_Parent._OnStarting(e);
				if(e.Cancel || (version != _Version))
				{
					if((_State == _SessionState.NONE) && (_Parent._ActiveBinding == this)) { _Parent._ActiveBinding = null; }
					return;
				}

				_State = _SessionState.PENDING;
				_AddMessageFilter();
				_LastText = context.Text;
				_Context = context;
				_Trigger = trigger;
				_ReplaceStart = context.WordStart;
				_ReplaceLength = null;
				_RequestCaret = context.CaretIndex;
			}

			try
			{
				if((trigger == AutoCompletionTrigger.TYPING) && (_Parent.Delay > 0))
				{
					_Delaying = true;
					await Task.Delay(_Parent.Delay, cancellation.Token);
					if((version != _Version) || _Disposed) { return; }

					_Delaying = false;
					context = _CreateContext(trigger, triggerCharacter);
				}

				if(_State == _SessionState.PENDING) { _ShowLoadingLater(version); }

				IAutoCompletionSource? source = (Source ?? _Parent.Source);
				AutoCompletionResult? result = ((source is null) ? null : await source.GetItemsAsync(context, cancellation.Token));

				if((version != _Version) || _Disposed) { return; }
				_Apply(context, result);
			}
			catch(OperationCanceledException) {}
			catch(Exception ex)
			{
				if((version == _Version) && (!_Disposed)) { _Close(AutoCompletionCloseReason.ERROR, ex); }
			}
		}


		/// <summary>Shows the loading indicator if a request takes longer than <see cref="_LOADING_DELAY"/>.</summary>
		/// <param name="version">Request version.</param>
		private async void _ShowLoadingLater(int version)
		{
			await Task.Delay(_LOADING_DELAY);

			if((version != _Version) || (_State != _SessionState.PENDING) || _Disposed || (!Control.Focused)) { return; }

			_ListPopupWindow<IAutoCompletionItem> popup = _PreparePopup();
			popup.SetLoading();
			_Position(popup, Control.Text);
		}


		/// <summary>Applies a source result to the session.</summary>
		/// <param name="context">Request context.</param>
		/// <param name="result">Source result.</param>
		private void _Apply(AutoCompletionContext context, AutoCompletionResult? result)
		{
			string text = Control.Text;
			int caret = _GetCaret(text);

			if((caret < context.WordStart) || (_GetWordStart(text, caret) != context.WordStart))
			{
				_Close(AutoCompletionCloseReason.CARET_MOVED);
				return;
			}

			if(!Control.Focused)
			{
				_Close(AutoCompletionCloseReason.FOCUS_LOST);
				return;
			}

			AutoCompletionPopulatingEventArgs e = new(context, result);
			int version = _Version;

			_Parent._OnPopulating(e);

			if((_State == _SessionState.NONE) || (version != _Version)) { return; }

			_Context = context;
			_ReplaceStart = Math.Clamp(e.ReplaceStart ?? context.WordStart, 0, context.CaretIndex);
			_ReplaceLength = ((e.ReplaceLength is null) ? null : Math.Max(0, e.ReplaceLength.Value));
			_RequestCaret = context.CaretIndex;
			_IsIncomplete = e.IsIncomplete;
			_SessionMatchMode = (e.MatchMode ?? MatchMode ?? _Parent.MatchMode);
			_SessionFilterMode = (e.FilterMode ?? FilterMode ?? _Parent.FilterMode);
			_SessionCaseSensitive = (e.CaseSensitive ?? CaseSensitive ?? _Parent.CaseSensitive);

			_Items = _Sort(e.Items.Where(m => m is not null).ToArray());
			_LastFilterPattern = null;
			_Usage = _GetUsage();

			_HasImages = _Items.Any(m => (m.Image is not null) || ((m.ImageKey is not null) && (_Parent.ImageList is not null)));
			_State = _SessionState.ACTIVE;

			if((_Items.Length == 0) && (!_IsIncomplete))
			{
				_Close(AutoCompletionCloseReason.NO_ITEMS);
				return;
			}

			bool acceptSingleMatch = _ExplicitPending;

			_ExplicitPending = false;
			_Refilter(text, caret, acceptSingleMatch);
		}


		/// <summary>Gets the usage counts of the session items.</summary>
		/// <returns>Returns the usage counts aligned with the sorted items, or NULL if usage ranking does not apply.</returns>
		private int[]? _GetUsage()
		{
			Dictionary<string, int>? counts;
			int[] rval;

			if((!_UsageRanking) || (_Items.Length == 0)) { return null; }

			counts = _Parent.UsageHistory._GetScope(_Scope);
			if((counts is null) || (counts.Count == 0)) { return null; }

			rval = new int[_Items.Length];
			for(int i = 0; i < _Items.Length; i++)
			{
				if(counts.TryGetValue(_Items[i].InsertText ?? _Items[i].Text, out int count)) { rval[i] = count; }
			}

			return rval;
		}


		/// <summary>Continues the session after a keystroke.</summary>
		/// <param name="text">Control text.</param>
		/// <param name="caret">Caret index.</param>
		/// <remarks>While the typing delay of a pending request is running, each keystroke restarts the delay, so items are requested
		///          after the user has stopped typing. Once the source has been called, keystrokes do not restart the request; the
		///          result is filtered with the text typed in the meantime.</remarks>
		private void _Continue(string text, int caret)
		{
			if(_State == _SessionState.PENDING)
			{
				if(_Delaying) { _Request(_Trigger, _KeyChar, false); }
				return;
			}

			if(_State != _SessionState.ACTIVE) { return; }

			_Refilter(text, caret);
			if(_IsIncomplete) { _Request(AutoCompletionTrigger.TYPING, _KeyChar, false); }
		}


		/// <summary>Sorts items by their sort text.</summary>
		/// <param name="items">Items.</param>
		/// <returns>Returns the sorted items.</returns>
		/// <remarks>Sources often return the same item list for every request (for example <see cref="AutoCompletionItemSource"/>). If the
		///          items are the same instances in the same order as last time, the cached sort result is reused. The check is linear
		///          and much cheaper than sorting large lists.</remarks>
		private IAutoCompletionItem[] _Sort(IAutoCompletionItem[] items)
		{
			IAutoCompletionItem[] rval;
			string[] keys;
			bool cached = ((_SortCacheInput is not null) && (_SortCacheOutput is not null) && (_SortCacheInput.Length == items.Length));

			for(int i = 0; cached && (i < items.Length); i++)
			{
				if(!ReferenceEquals(_SortCacheInput![i], items[i])) { cached = false; }
			}
			if(cached) { return _SortCacheOutput!; }

			rval = (IAutoCompletionItem[]) items.Clone();
			keys = rval.Select(m => (m.SortText ?? m.Text)).ToArray();
			Array.Sort(keys, rval, Comparer<string>.Create(_ListMatcher._CompareSortKeys));

			_SortCacheInput = items;
			_SortCacheOutput = rval;

			return rval;
		}


		/// <summary>Registers the binding as a message filter to detect mouse clicks outside the popup window and the control.</summary>
		private void _AddMessageFilter()
		{
			if(_Filtering) { return; }

			Application.AddMessageFilter(this);
			_Filtering = true;
		}


		/// <summary>Removes the message filter registration.</summary>
		private void _RemoveMessageFilter()
		{
			if(!_Filtering) { return; }

			Application.RemoveMessageFilter(this);
			_Filtering = false;
		}


		/// <summary>Suppresses auto start for the current word until the user leaves or stops continuously typing it.</summary>
		private void _Suppress()
		{
			_LastText = Control.Text;
			_SuppressedWordStart = (_Context?.WordStart ?? -1);
			_SuppressedCaret = _GetCaret(_LastText);
		}


		/// <summary>Returns if auto start is suppressed for a keystroke and updates or ends the suppression.</summary>
		/// <param name="c">Typed character, '\b' for deletions.</param>
		/// <param name="caret">Caret index after the keystroke.</param>
		/// <param name="wordStart">Word start index after the keystroke.</param>
		/// <returns>Returns <c>true</c> if auto start is suppressed, otherwise returns <c>false</c>.</returns>
		/// <remarks>The suppression only lasts while the user continues typing or deleting in the same word, one character at a time.
		///          Any other change (caret movement, replaced selection, deleting the whole word, typing a non-word character)
		///          ends it.</remarks>
		private bool _IsSuppressed(char c, int caret, int wordStart)
		{
			if(_SuppressedWordStart < 0) { return false; }

			int expected = ((c == '\b') ? (_SuppressedCaret - 1) : (_SuppressedCaret + 1));

			if((wordStart != _SuppressedWordStart) || (caret <= wordStart) || (caret != expected))
			{
				_SuppressedWordStart = _SuppressedCaret = -1;
				return false;
			}

			_SuppressedCaret = caret;
			return true;
		}


		/// <summary>Checks if the caret is still within the session range after a caret movement.</summary>
		private void _CheckCaret()
		{
			if((_State == _SessionState.NONE) || (_Context is null) || _Disposed) { return; }

			string text = Control.Text;
			int caret = _GetCaret(text);

			if((Control.SelectionLength > 0) || (caret < _Context.WordStart) || (_GetWordStart(text, caret) != _Context.WordStart))
			{
				_Close(AutoCompletionCloseReason.CARET_MOVED);
				return;
			}

			if(_State != _SessionState.ACTIVE) { return; }

			int start = Math.Min(_ReplaceStart, text.Length);
			string pattern = ((caret > start) ? text.Substring(start, caret - start) : string.Empty);

			if(!string.Equals(pattern, _Pattern, StringComparison.Ordinal)) { _Refilter(text, caret); }
		}


		/// <summary>Checks the caret position after pending messages have been processed.</summary>
		private void _CheckCaretLater()
		{
			if((_State == _SessionState.NONE) || (!Control.IsHandleCreated)) { return; }
			Control.BeginInvoke(new Action(_CheckCaret));
		}


		/// <summary>Accepts an item.</summary>
		/// <param name="item">Item.</param>
		/// <returns>Returns <c>true</c> if the item has been inserted, <c>false</c> if the insertion has been cancelled.</returns>
		private bool _Accept(IAutoCompletionItem item)
		{
			string text = Control.Text;
			int caret = _GetCaret(text);
			int start = Math.Min(_ReplaceStart, text.Length);
			int end = ((_ReplaceLength is null) ? caret : (start + _ReplaceLength.Value + Math.Max(0, caret - _RequestCaret)));

			end = Math.Clamp(end, start, text.Length);

			AutoCompletionAcceptingEventArgs e = new(Control, item, (item.InsertText ?? item.Text), item.CaretPosition, start, end - start);
			_Parent._OnAccepting(e);

			if(e.Cancel) { return false; }

			string insert = (e.InsertText ?? string.Empty);
			int replaceStart = Math.Clamp(e.ReplaceStart, 0, text.Length);
			int replaceLength = Math.Clamp(e.ReplaceLength, 0, text.Length - replaceStart);
			int newCaret = replaceStart + Math.Clamp(e.CaretPosition ?? insert.Length, 0, insert.Length);

			_Close(AutoCompletionCloseReason.ACCEPTED);

			_Accepting = true;
			try
			{
				Control.Select(replaceStart, replaceLength);
				Control.SelectedText = insert;
				Control.Select(newCaret, 0);
			}
			finally { _Accepting = false; }

			_LastText = Control.Text;
			if(_UsageRanking) { _Parent.UsageHistory.Increment(_Scope, item.InsertText ?? item.Text); }

			_Parent._OnAccepted(new AutoCompletionAcceptedEventArgs(Control, item, insert, replaceStart));
			return true;
		}


		/// <summary>Ends the current session.</summary>
		/// <param name="reason">Close reason.</param>
		/// <param name="exception">Exception that caused the session to end.</param>
		private void _Close(AutoCompletionCloseReason reason, Exception? exception = null)
		{
			if(_State == _SessionState.NONE) { return; }

			_State = _SessionState.NONE;
			_Delaying = false;
			_Version++;
			_Cancellation?.Cancel();
			_Cancellation = null;

			if(_Parent._ActiveBinding == this)
			{
				_Parent._Popup?.HideWindow();
				_Parent._Description?.HideWindow();
				_Parent._ActiveBinding = null;
			}

			_NotifySelection(null);

			_Context = null;
			_Items = [];
			_LastFilterPattern = null;
			_LastFilterIndices = [];
			_Filtered.Clear();
			_Pattern = string.Empty;
			_IsIncomplete = false;
			_UserNavigated = false;
			_ExplicitPending = false;
			_SessionBoxWidth = 0;
			_Usage = null;
			_UnhookForm();
			_RemoveMessageFilter();

			_Parent._OnClosed(new AutoCompletionClosedEventArgs(Control, reason, exception));
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the control.</summary>
		public TextBoxBase Control
		{
			get;
		}


		/// <summary>Gets or sets if auto completion is enabled.</summary>
		public bool Enabled
		{
			get { return _Enabled; }
			set
			{
				_Enabled = value;

				if(value)
				{
					_Hook();
				}
				else
				{
					_Close(AutoCompletionCloseReason.MANUAL);
					_Unhook();
				}
			}
		}


		/// <summary>Gets or sets if auto completion starts automatically when typing.</summary>
		public bool AutoStart
		{
			get; set;
		} = true;


		/// <summary>Gets or sets the shortcut keys.</summary>
		public Keys ShortcutKeys
		{
			get; set;
		} = Keys.Control | Keys.Space;


		/// <summary>Gets or sets the source.</summary>
		public IAutoCompletionSource? Source
		{
			get; set;
		}


		/// <summary>Gets or sets the match mode.</summary>
		public ListMatchMode? MatchMode
		{
			get; set;
		}


		/// <summary>Gets or sets the filter mode.</summary>
		public ListFilterMode? FilterMode
		{
			get; set;
		}


		/// <summary>Gets or sets if matching is case-sensitive.</summary>
		public bool? CaseSensitive
		{
			get; set;
		}


		/// <summary>Gets or sets the word characters.</summary>
		public string? WordCharacters
		{
			get; set;
		}


		/// <summary>Gets or sets the trigger characters.</summary>
		public string? TriggerCharacters
		{
			get; set;
		}


		/// <summary>Gets or sets the commit characters.</summary>
		public string? CommitCharacters
		{
			get; set;
		}


		/// <summary>Gets or sets the window size.</summary>
		public Size BoxSize
		{
			get; set;
		} = Size.Empty;


		/// <summary>Gets or sets if the window width adjusts to the items.</summary>
		public bool? BoxAutoSize
		{
			get; set;
		}


		/// <summary>Gets or sets if the window can be resized by the user.</summary>
		public bool? BoxResizable
		{
			get; set;
		}


		/// <summary>Gets or sets if usage ranking is enabled.</summary>
		public bool? UsageRanking
		{
			get; set;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Starts a session.</summary>
		/// <param name="trigger">Trigger.</param>
		public void Start(AutoCompletionTrigger trigger)
		{
			_SuppressedWordStart = _SuppressedCaret = -1;
			_ExplicitPending = ((trigger == AutoCompletionTrigger.SHORTCUT) || (trigger == AutoCompletionTrigger.MANUAL));
			_Request(trigger, '\0', (_State == _SessionState.NONE));
		}


		/// <summary>Ends the current session.</summary>
		/// <param name="reason">Close reason.</param>
		public void Close(AutoCompletionCloseReason reason)
		{
			_Close(reason);
		}


		/// <summary>Accepts the selected item.</summary>
		public void AcceptSelected()
		{
			IAutoCompletionItem? item = _VisiblePopup?.SelectedItem;
			if(item is not null) { _Accept(item); }
		}


		/// <summary>Marks the current selection as explicitly made by the user.</summary>
		public void MarkNavigated()
		{
			_UserNavigated = true;
		}


		/// <summary>Updates description and selection notification after the selection of the popup window has changed.</summary>
		public void UpdateSelection()
		{
			_ListPopupWindow<IAutoCompletionItem>? popup = _VisiblePopup;

			_NotifySelection(popup?.SelectedItem);
			if(popup is not null) { _UpdateDescription(); }
		}


		/// <summary>Stores the window size chosen by the user.</summary>
		public void StoreUserSize()
		{
			_ListPopupWindow<IAutoCompletionItem>? popup = _VisiblePopup;

			if(popup is null) { return; }

			_UserBoxSize = popup.Size;
			_UpdateDescription();
		}


		/// <summary>Suspends tracking of text and selection changes.</summary>
		public void SuspendTracking()
		{
			_TrackingSuspended++;
		}


		/// <summary>Resumes tracking of text and selection changes.</summary>
		/// <remarks>When the last suspension ends, the session continues if the caret is still in the completed word, otherwise it ends.</remarks>
		public void ResumeTracking()
		{
			if(_TrackingSuspended == 0) { return; }
			if(--_TrackingSuspended > 0) { return; }

			if(_State != _SessionState.NONE)
			{
				_LastText = Control.Text;
				_CheckCaret();
			}
			else if(_SuppressedWordStart >= 0) { _LastText = Control.Text; }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// event handlers                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Handles the Disposed event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_Disposed(object? sender, EventArgs e)
		{
			_Parent._Unbind(Control);
		}


		/// <summary>Handles the PreviewKeyDown event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
		{
			if(_VisiblePopup is null) { return; }

			switch(e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Up:
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
					e.IsInputKey = true;
					break;
				case Keys.Enter:
				case Keys.Tab:
					if(_AcceptsWith(e.KeyCode, e.Modifiers)) { e.IsInputKey = true; }
					else { _Close(AutoCompletionCloseReason.CANCELLED); }
					break;
			}
		}


		/// <summary>Handles the KeyDown event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_KeyDown(object? sender, KeyEventArgs e)
		{
			_ListPopupWindow<IAutoCompletionItem>? popup;

			_KeyPending = false;

			if((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
			{
				_KeyPending = true;
				_KeyChar = '\b';
			}

			if((ShortcutKeys != Keys.None) && (e.KeyData == ShortcutKeys))
			{
				e.Handled = e.SuppressKeyPress = true;
				Start(AutoCompletionTrigger.SHORTCUT);
				return;
			}

			if((e.Modifiers == Keys.None) && ((e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End)) && ((popup = _VisiblePopup) is not null) && (popup.ItemCount > 0))
			{
				popup.SelectedIndex = ((e.KeyCode == Keys.Home) ? 0 : (popup.ItemCount - 1));
				_UserNavigated = true;

				e.Handled = e.SuppressKeyPress = true;
				return;
			}

			if((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End))
			{
				_CheckCaretLater();
				return;
			}

			if((popup = _VisiblePopup) is null)
			{
				if((e.KeyCode == Keys.Escape) && (_State != _SessionState.NONE))
				{
					_Suppress();
					_Close(AutoCompletionCloseReason.CANCELLED);
				}
				return;
			}

			switch(e.KeyCode)
			{
				case Keys.Up:
					_Navigate(popup, -1);
					e.Handled = e.SuppressKeyPress = true;
					break;
				case Keys.Down:
					_Navigate(popup, 1);
					e.Handled = e.SuppressKeyPress = true;
					break;
				case Keys.PageUp:
					_Navigate(popup, -Math.Max(1, popup.VisibleRowCount - 1));
					e.Handled = e.SuppressKeyPress = true;
					break;
				case Keys.PageDown:
					_Navigate(popup, Math.Max(1, popup.VisibleRowCount - 1));
					e.Handled = e.SuppressKeyPress = true;
					break;
				case Keys.Escape:
					_Suppress();
					_Close(AutoCompletionCloseReason.CANCELLED);
					e.Handled = e.SuppressKeyPress = true;
					break;
				case Keys.Enter:
				case Keys.Tab:
					if(_AcceptsWith(e.KeyCode, e.Modifiers))
					{
						e.Handled = e.SuppressKeyPress = true;
						_Accept(popup.SelectedItem!);
					}
					else { _Close(AutoCompletionCloseReason.CANCELLED); }
					break;
			}
		}


		/// <summary>Handles the KeyPress event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_KeyPress(object? sender, KeyPressEventArgs e)
		{
			char c = e.KeyChar;

			if(char.IsControl(c)) { return; }

			IAutoCompletionItem? item = _VisiblePopup?.SelectedItem;
			if(item is not null)
			{
				bool commit = (((c == ' ') && _Parent.AcceptOnSpace) || _CommitCharacters.Contains(c));
				if(commit && _IsUnambiguous(item)) { _Accept(item); }
			}

			_KeyPending = true;
			_KeyChar = c;
		}


		/// <summary>Handles the TextChanged event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		/// <remarks>Text changes while tracking is suspended are ignored completely; the pending keystroke is kept for the outer text
		///          change. Changes that are not caused by a keystroke end the session, unless the text has not changed at all (for
		///          example, a syntax highlighter changing colors of a <see cref="RichTextBox"/>).</remarks>
		private void Control_TextChanged(object? sender, EventArgs e)
		{
			if(_Accepting || _Disposed || (_TrackingSuspended > 0)) { return; }

			bool typed = _KeyPending;
			char c = _KeyChar;

			_KeyPending = false;

			if((!typed) || (!Control.Focused))
			{
				string current;

				if((_State == _SessionState.NONE) && (_SuppressedWordStart < 0)) { return; }

				current = Control.Text;
				if(string.Equals(current, _LastText, StringComparison.Ordinal)) { return; }

				_LastText = current;
				_SuppressedWordStart = _SuppressedCaret = -1;
				_Close(AutoCompletionCloseReason.TEXT_CHANGED);
				return;
			}

			string text = Control.Text;
			int caret = _GetCaret(text);
			int wordStart = _GetWordStart(text, caret);

			_LastText = text;

			if(_State != _SessionState.NONE)
			{
				if(_IsInSession(c, caret, wordStart))
				{
					_Continue(text, caret);
					return;
				}

				_Close(AutoCompletionCloseReason.CARET_MOVED);
			}

			bool suppressed = _IsSuppressed(c, caret, wordStart);

			if(c == '\b') { return; }

			if(_TriggerCharacters.Contains(c))
			{
				_Request(AutoCompletionTrigger.TRIGGER_CHARACTER, c, true);
			}
			else if(AutoStart && _IsWordCharacter(c) && ((caret - wordStart) >= _Parent.MinimumWordLength) && (!suppressed))
			{
				_Request(AutoCompletionTrigger.TYPING, c, true);
			}
		}


		/// <summary>Handles the MouseDown event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_MouseDown(object? sender, MouseEventArgs e)
		{
			_CheckCaretLater();
		}


		/// <summary>Handles the MouseWheel event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		/// <remarks>Mouse wheel messages are sent to the focused control, so the popup window never receives them directly. Over the
		///          popup window, the wheel scrolls the list instead of the control. Elsewhere, the control scrolls and the popup window
		///          is repositioned afterwards.</remarks>
		private void Control_MouseWheel(object? sender, MouseEventArgs e)
		{
			_ListPopupWindow<IAutoCompletionItem>? popup = _VisiblePopup;

			if(popup is null) { return; }

			if(popup.Bounds.Contains(Cursor.Position))
			{
				popup.ScrollByWheel(e.Delta);
				if(e is HandledMouseEventArgs handled) { handled.Handled = true; }
			}
			else if(Control.IsHandleCreated)
			{
				Control.BeginInvoke(new Action(() =>
				{
					_ListPopupWindow<IAutoCompletionItem>? visible = _VisiblePopup;
					if(visible is not null) { _Position(visible, Control.Text); }
				}));
			}
		}


		/// <summary>Handles the LostFocus event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_LostFocus(object? sender, EventArgs e)
		{
			_SuppressedWordStart = _SuppressedCaret = -1;
			_Close(AutoCompletionCloseReason.FOCUS_LOST);
		}


		/// <summary>Handles the Deactivate event of the form.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void _Form_Deactivate(object? sender, EventArgs e)
		{
			_Close(AutoCompletionCloseReason.FOCUS_LOST);
		}


		/// <summary>Handles the Move and Resize events of the form.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void _Form_Move(object? sender, EventArgs e)
		{
			_ListPopupWindow<IAutoCompletionItem>? popup = _VisiblePopup;
			if(popup is not null) { _Position(popup, Control.Text); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IMessageFilter                                                                                   //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Filters mouse clicks while a session is active and ends the session on clicks outside the popup window and the
		///          text area of the control.</summary>
		/// <param name="m">Message.</param>
		/// <returns>Returns <c>false</c>; messages are never consumed.</returns>
		/// <remarks>Clicks into the text area are handled by the MouseDown event of the control (the session stays open while the caret
		///          remains in the word). Clicks into the non-client area of the control, such as its scroll bars, end the session.</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			bool client = ((m.Msg == _WM_LBUTTONDOWN) || (m.Msg == _WM_RBUTTONDOWN) || (m.Msg == _WM_MBUTTONDOWN) || (m.Msg == _WM_XBUTTONDOWN));
			bool nonClient = ((m.Msg == _WM_NCLBUTTONDOWN) || (m.Msg == _WM_NCRBUTTONDOWN) || (m.Msg == _WM_NCMBUTTONDOWN) || (m.Msg == _WM_NCXBUTTONDOWN));
			_ListPopupWindow<IAutoCompletionItem>? popup = _Parent._Popup;

			if((!(client || nonClient)) || (_State == _SessionState.NONE)) { return false; }

			if((popup is not null) && popup.IsHandleCreated && (m.HWnd == popup.Handle)) { return false; }
			if((_Parent._Description is not null) && _Parent._Description.IsHandleCreated && (m.HWnd == _Parent._Description.Handle)) { return false; }
			if(client && Control.IsHandleCreated && (m.HWnd == Control.Handle)) { return false; }

			_Close(AutoCompletionCloseReason.CLICKED_OUTSIDE);
			return false;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IDisposable                                                                                      //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public void Dispose()
		{
			if(_Disposed) { return; }

			_Close(AutoCompletionCloseReason.MANUAL);
			_Disposed = true;
			_Unhook();
			_UnhookForm();
			_RemoveMessageFilter();
			Control.Disposed -= Control_Disposed;
			_SortCacheInput = _SortCacheOutput = null;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [enum] _SessionState                                                                                         //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>This enumeration defines the session states.</summary>
		private enum _SessionState: int
		{
			/// <summary>No session.</summary>
			NONE = 0,

			/// <summary>Items are being requested.</summary>
			PENDING = 1,

			/// <summary>Items are available.</summary>
			ACTIVE = 2
		}
	}
}
