namespace Robbiblubber.Spellbook.Controls;

using Robbiblubber.Spellbook.Controls.Themes;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



partial class SignatureHelpProvider
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Binding                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents the signature help binding of a control and manages its sessions.</summary>
	/// <remarks>A session runs through the states <see cref="_SessionState.NONE"/>, <see cref="_SessionState.PENDING"/> (signatures are
	///          being requested) and <see cref="_SessionState.ACTIVE"/> (signatures are available). Every text change and caret movement
	///          parses the call at the caret again: within the same call, only the active parameter changes; another call requests its
	///          signatures; leaving all calls ends the session. If another call within a session has no signatures, the session stays
	///          active with the window hidden, so it continues when the caret returns to a known call. Each request is stamped with a
	///          version number; results of outdated requests are discarded.</remarks>
	private class _Binding: IMessageFilter, IDisposable
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Parent provider.</summary>
		private readonly SignatureHelpProvider _Parent;

		/// <summary>Enabled flag.</summary>
		private bool _Enabled = false;

		/// <summary>Determines if the control events are hooked.</summary>
		private bool _Hooked = false;

		/// <summary>Disposed flag.</summary>
		private bool _Disposed = false;

		/// <summary>Determines if the binding is registered as a message filter.</summary>
		private bool _Filtering = false;

		/// <summary>Session state.</summary>
		private _SessionState _State = _SessionState.NONE;

		/// <summary>Call of the current session.</summary>
		private SignatureCall? _Call = null;

		/// <summary>Signatures of the current call.</summary>
		private SignatureInfo[] _Signatures = [];

		/// <summary>Index of the displayed signature.</summary>
		private int _SignatureIndex = 0;

		/// <summary>Determines if the displayed signature has been chosen explicitly and is kept when the argument changes.</summary>
		private bool _SignatureFixed = false;

		/// <summary>Request version.</summary>
		private int _Version = 0;

		/// <summary>Cancellation token source of the current request.</summary>
		private CancellationTokenSource? _Cancellation = null;

		/// <summary>Determines if a character has been typed and the next text change may result from it.</summary>
		private bool _KeyPending = false;

		/// <summary>Typed character.</summary>
		private char _KeyChar = '\0';

		/// <summary>Tracking suspension counter.</summary>
		private int _TrackingSuspended = 0;

		/// <summary>Form the control belongs to while a session is active.</summary>
		private Form? _Form = null;

		/// <summary>Selection reported by the last selection change notification.</summary>
		private (SignatureInfo? Signature, int SignatureIndex, int ParameterIndex, int ArgumentIndex) _Notified = (null, -1, -1, -1);



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="parent">Parent provider.</param>
		/// <param name="control">Control.</param>
		public _Binding(SignatureHelpProvider parent, TextBoxBase control)
		{
			_Parent = parent;
			Control = control;

			Control.Disposed += Control_Disposed;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private properties                                                                                           //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the effective trigger characters.</summary>
		private string _TriggerCharacters
		{
			get { return (TriggerCharacters ?? _Parent.TriggerCharacters ?? string.Empty); }
		}


		/// <summary>Gets the effective parser.</summary>
		private ISignatureCallParser _Parser
		{
			get { return (Parser ?? _Parent.Parser); }
		}


		/// <summary>Gets if the signature window is visible for this binding.</summary>
		private bool _WindowVisible
		{
			get
			{
				if((_State != _SessionState.ACTIVE) || (_Parent._ActiveBinding != this) || (_Signatures.Length == 0)) { return false; }
				return ((_Parent._Window is not null) && (!_Parent._Window.IsDisposed) && _Parent._Window.Visible);
			}
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private static methods                                                                                       //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Returns if a key moves the caret.</summary>
		/// <param name="key">Key code.</param>
		/// <returns>Returns <c>true</c> if the key moves the caret, otherwise returns <c>false</c>.</returns>
		private static bool _IsCaretKey(Keys key)
		{
			return ((key == Keys.Left) || (key == Keys.Right) || (key == Keys.Up) || (key == Keys.Down) || (key == Keys.Home) || (key == Keys.End) || (key == Keys.PageUp) || (key == Keys.PageDown));
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


		/// <summary>Registers the binding as a message filter to detect mouse clicks outside the window and the control.</summary>
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


		/// <summary>Gets the caret index clamped to the text length.</summary>
		/// <param name="text">Control text.</param>
		/// <returns>Returns the caret index.</returns>
		private int _GetCaret(string text)
		{
			return Math.Min(Control.SelectionStart, text.Length);
		}


		/// <summary>Gets the index of the active parameter of the displayed signature.</summary>
		/// <returns>Returns the parameter index, or -1 if no parameter is active.</returns>
		private int _GetParameterIndex()
		{
			if((_Call is null) || (_Signatures.Length == 0)) { return -1; }
			return _Signatures[_SignatureIndex].GetParameterIndex(_Call.ArgumentIndex);
		}


		/// <summary>Selects the first signature that accepts the current argument, unless a signature has been chosen explicitly.</summary>
		private void _SelectSignature()
		{
			if(_SignatureFixed || (_Call is null)) { return; }

			for(int i = 0; i < _Signatures.Length; i++)
			{
				if(!_Signatures[i].Accepts(_Call.ArgumentIndex)) { continue; }

				_SignatureIndex = i;
				return;
			}
		}


		/// <summary>Raises the selection changed event if the displayed signature or the active parameter has changed.</summary>
		private void _NotifySelection()
		{
			(SignatureInfo? Signature, int SignatureIndex, int ParameterIndex, int ArgumentIndex) current = (null, -1, -1, -1);

			if((_State == _SessionState.ACTIVE) && (_Signatures.Length > 0) && (_Call is not null))
			{
				current = (_Signatures[_SignatureIndex], _SignatureIndex, _GetParameterIndex(), _Call.ArgumentIndex);
			}

			if(ReferenceEquals(current.Signature, _Notified.Signature) && (current.SignatureIndex == _Notified.SignatureIndex) && (current.ParameterIndex == _Notified.ParameterIndex) && (current.ArgumentIndex == _Notified.ArgumentIndex)) { return; }

			_Notified = current;
			_Parent._OnSelectionChanged(new SignatureHelpSelectionChangedEventArgs(Control, current.Signature, current.SignatureIndex, current.ParameterIndex, current.ArgumentIndex));
		}


		/// <summary>Shows the displayed signature, or hides the window if the call has no signatures.</summary>
		private void _Show()
		{
			if((_Call is null) || (_Signatures.Length == 0))
			{
				if(_Parent._ActiveBinding == this) { _Parent._Window?.HideWindow(); }
				_NotifySelection();
				return;
			}

			_SignatureHelpWindow window = _Parent._GetWindow();
			AutoCompletionProvider? linked = _Parent._AutoCompletion;
			string text = Control.Text;
			int caret = _GetCaret(text);
			Point position = _TextBoxUtility._GetCharacterPosition(Control, text, Math.Min(_Call.NameStart, text.Length));
			Point caretPosition = _TextBoxUtility._GetCharacterPosition(Control, text, caret);
			Form? form = Control.FindForm();

			if(position.Y != caretPosition.Y)
			{
				int lineStart = ((caret == 0) ? 0 : (text.LastIndexOf('\n', caret - 1) + 1));
				position = new Point(_TextBoxUtility._GetCharacterPosition(Control, text, lineStart).X, caretPosition.Y);
			}

			Theme theme = ThemeManager.GetTheme(Control);
			Color backColor = _Parent._ResolveColor(_Parent.WindowBackColor, linked?.ListBackColor, theme.MapSystemColor(Control.BackColor));
			Color foreColor = _Parent._ResolveColor(_Parent.WindowForeColor, linked?.ListForeColor, theme.MapSystemColor(Control.ForeColor));
			Color detailColor = _Parent._ResolveColor(_Parent.DetailForeColor, linked?.DetailForeColor, theme[ThemeColor.GRAY_TEXT]);
			Color highlightColor = _Parent._ResolveColor(_Parent.HighlightColor, linked?.HighlightColor, theme[ThemeColor.LINK]);
			Color borderColor = _Parent._ResolveColor(_Parent.BorderColor, linked?.BorderColor, (theme.IsSystem ? Color.Empty : theme[ThemeColor.POPUP_BORDER]));

			int maximumWidth = Control.LogicalToDeviceUnits(_Parent.MaximumWidth);

			window.ApplyStyle(Control.Font, backColor, foreColor, detailColor, highlightColor, _Parent.HighlightStyle, borderColor);
			window.ShowSignature(Control.PointToScreen(position), Control.Font.Height, _Signatures[_SignatureIndex], _SignatureIndex, _Signatures.Length, _GetParameterIndex(), _Parent.ShowDescriptions, maximumWidth, form);

			_HookForm(form);
			_NotifySelection();
		}


		/// <summary>Finds the call at the caret.</summary>
		/// <returns>Returns the call, or NULL if the caret is not within a call.</returns>
		private SignatureCall? _Parse()
		{
			string text = Control.Text;
			return _Parser.Parse(text, _GetCaret(text));
		}


		/// <summary>Starts a session if the caret is within a call.</summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="triggerCharacter">Trigger character.</param>
		/// <remarks>If the parser fails, no session is started; within a session, parser exceptions end the session with
		///          <see cref="SignatureHelpCloseReason.ERROR"/>.</remarks>
		private void _TryStart(SignatureHelpTrigger trigger, char triggerCharacter)
		{
			SignatureCall? call;

			try
			{
				call = _Parse();
			}
			catch(Exception) { return; }

			if(call is not null) { _Request(trigger, triggerCharacter, call, true); }
		}


		/// <summary>Parses the call at the caret again and updates the session.</summary>
		private void _Update()
		{
			SignatureCall? call;

			if((_State == _SessionState.NONE) || _Disposed) { return; }

			try
			{
				call = _Parse();
			}
			catch(Exception ex)
			{
				_Close(SignatureHelpCloseReason.ERROR, ex);
				return;
			}

			if(call is null)
			{
				_Close(SignatureHelpCloseReason.CALL_LEFT);
				return;
			}

			if(!call.IsSameCall(_Call))
			{
				_Request(SignatureHelpTrigger.CALL_CHANGED, '\0', call, false);
				return;
			}

			_Call = call;

			if(_State == _SessionState.ACTIVE)
			{
				_SelectSignature();
				_Show();
			}
		}


		/// <summary>Parses the call at the caret after pending messages have been processed.</summary>
		private void _UpdateLater()
		{
			if((_State == _SessionState.NONE) || (!Control.IsHandleCreated)) { return; }
			Control.BeginInvoke(new Action(_Update));
		}


		/// <summary>Requests the signatures of a call.</summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="triggerCharacter">Trigger character.</param>
		/// <param name="call">Call.</param>
		/// <param name="newSession">Determines if a new session is started.</param>
		private async void _Request(SignatureHelpTrigger trigger, char triggerCharacter, SignatureCall call, bool newSession)
		{
			CancellationTokenSource cancellation;
			SignatureHelpContext context;
			int version;

			if(_Disposed) { return; }

			_Cancellation?.Cancel();
			cancellation = _Cancellation = new CancellationTokenSource();
			version = ++_Version;
			context = new SignatureHelpContext(Control, Control.Text, _GetCaret(Control.Text), call, trigger, triggerCharacter);

			if(newSession)
			{
				SignatureHelpStartingEventArgs e = new(context);

				_Parent._Activate(this);
				_Parent._OnStarting(e);
				if(e.Cancel || (version != _Version))
				{
					if((_State == _SessionState.NONE) && (_Parent._ActiveBinding == this)) { _Parent._ActiveBinding = null; }
					return;
				}

				_AddMessageFilter();
			}

			_State = _SessionState.PENDING;
			_Call = call;
			_SignatureFixed = false;

			try
			{
				ISignatureHelpSource? source = (Source ?? _Parent.Source);
				ValueTask<SignatureHelpResult?> task = ((source is null) ? new ValueTask<SignatureHelpResult?>((SignatureHelpResult?) null) : source.GetSignaturesAsync(context, cancellation.Token));
				SignatureHelpResult? result;

				if((!task.IsCompleted) && (_Parent._ActiveBinding == this)) { _Parent._Window?.HideWindow(); }

				result = await task;
				if((version != _Version) || _Disposed) { return; }

				_Apply(context, result);
			}
			catch(OperationCanceledException) {}
			catch(Exception ex)
			{
				if((version == _Version) && (!_Disposed)) { _Close(SignatureHelpCloseReason.ERROR, ex); }
			}
		}


		/// <summary>Applies a source result to the session.</summary>
		/// <param name="context">Request context.</param>
		/// <param name="result">Source result.</param>
		private void _Apply(SignatureHelpContext context, SignatureHelpResult? result)
		{
			SignatureCall? call = _Parse();
			SignatureHelpPopulatingEventArgs e;

			if(call is null)
			{
				_Close(SignatureHelpCloseReason.CALL_LEFT);
				return;
			}

			if(!call.IsSameCall(context.Call))
			{
				_Request(SignatureHelpTrigger.CALL_CHANGED, '\0', call, false);
				return;
			}

			if(!Control.Focused)
			{
				_Close(SignatureHelpCloseReason.FOCUS_LOST);
				return;
			}

			e = new SignatureHelpPopulatingEventArgs(context, result);
			_Parent._OnPopulating(e);

			if(_State == _SessionState.NONE) { return; }

			_Call = call;
			_Signatures = e.Signatures.Where(m => m is not null).ToArray();

			if((_Signatures.Length == 0) && (context.Trigger != SignatureHelpTrigger.CALL_CHANGED))
			{
				_Close(SignatureHelpCloseReason.NO_SIGNATURES);
				return;
			}

			_State = _SessionState.ACTIVE;
			_SignatureFixed = ((e.ActiveSignature is not null) && (_Signatures.Length > 0));
			_SignatureIndex = ((_Signatures.Length == 0) ? 0 : Math.Clamp(e.ActiveSignature ?? 0, 0, _Signatures.Length - 1));

			_SelectSignature();
			_Show();
		}


		/// <summary>Ends the current session.</summary>
		/// <param name="reason">Close reason.</param>
		/// <param name="exception">Exception that caused the session to end.</param>
		private void _Close(SignatureHelpCloseReason reason, Exception? exception = null)
		{
			if(_State == _SessionState.NONE) { return; }

			_State = _SessionState.NONE;
			_Version++;
			_Cancellation?.Cancel();
			_Cancellation = null;

			if(_Parent._ActiveBinding == this)
			{
				_Parent._Window?.HideWindow();
				_Parent._ActiveBinding = null;
			}

			_NotifySelection();

			_Call = null;
			_Signatures = [];
			_SignatureIndex = 0;
			_SignatureFixed = false;
			_UnhookForm();
			_RemoveMessageFilter();

			_Parent._OnClosed(new SignatureHelpClosedEventArgs(Control, reason, exception));
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the control.</summary>
		public TextBoxBase Control
		{
			get;
		}


		/// <summary>Gets or sets if signature help is enabled.</summary>
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
					_Close(SignatureHelpCloseReason.MANUAL);
					_Unhook();
				}
			}
		}


		/// <summary>Gets or sets if signature help starts automatically when a trigger character is typed.</summary>
		public bool AutoStart
		{
			get; set;
		} = true;


		/// <summary>Gets or sets the shortcut keys.</summary>
		public Keys ShortcutKeys
		{
			get; set;
		} = Keys.Control | Keys.Shift | Keys.Space;


		/// <summary>Gets or sets the trigger characters, NULL for the provider setting.</summary>
		public string? TriggerCharacters
		{
			get; set;
		}


		/// <summary>Gets or sets the source, NULL for the provider source.</summary>
		public ISignatureHelpSource? Source
		{
			get; set;
		}


		/// <summary>Gets or sets the parser, NULL for the provider parser.</summary>
		public ISignatureCallParser? Parser
		{
			get; set;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Starts a session, or updates the running session.</summary>
		/// <param name="trigger">Trigger.</param>
		public void Start(SignatureHelpTrigger trigger)
		{
			if(_State != _SessionState.NONE)
			{
				_Update();
				return;
			}

			_TryStart(trigger, '\0');
		}


		/// <summary>Starts or updates a session after an auto completion item with an opening parenthesis has been inserted.</summary>
		public void StartAfterCompletion()
		{
			if((!_Enabled) || (!AutoStart) || _Disposed) { return; }

			if(_State != _SessionState.NONE)
			{
				_Update();
				return;
			}

			_TryStart(SignatureHelpTrigger.AUTO_COMPLETION, '\0');
		}


		/// <summary>Ends the current session.</summary>
		/// <param name="reason">Close reason.</param>
		public void Close(SignatureHelpCloseReason reason)
		{
			_Close(reason);
		}


		/// <summary>Displays another signature.</summary>
		/// <param name="delta">Number of signatures to move, negative to move backwards; the selection wraps around.</param>
		public void Cycle(int delta)
		{
			int count = _Signatures.Length;

			if((_State != _SessionState.ACTIVE) || (count < 2)) { return; }

			_SignatureIndex = (((_SignatureIndex + delta) % count) + count) % count;
			_SignatureFixed = true;
			_Show();
		}


		/// <summary>Suspends tracking of text and caret changes.</summary>
		public void SuspendTracking()
		{
			_TrackingSuspended++;
		}


		/// <summary>Resumes tracking of text and caret changes.</summary>
		/// <remarks>When the last suspension ends, the call at the caret is checked again.</remarks>
		public void ResumeTracking()
		{
			if(_TrackingSuspended == 0) { return; }
			if(--_TrackingSuspended > 0) { return; }

			_Update();
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
			if((!_WindowVisible) || _Parent._IsCompletionVisible(Control) || (e.Modifiers != Keys.None)) { return; }

			if((e.KeyCode == Keys.Escape) || (((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)) && (_Signatures.Length > 1))) { e.IsInputKey = true; }
		}


		/// <summary>Handles the KeyDown event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		/// <remarks>Keys that have already been handled, for example by an open auto completion list, are ignored, and so are Escape and
		///          the navigation keys while the linked auto completion list is visible.</remarks>
		private void Control_KeyDown(object? sender, KeyEventArgs e)
		{
			bool free;

			_KeyPending = false;

			if((ShortcutKeys != Keys.None) && (e.KeyData == ShortcutKeys))
			{
				e.Handled = e.SuppressKeyPress = true;
				Start(SignatureHelpTrigger.SHORTCUT);
				return;
			}

			if(_State == _SessionState.NONE) { return; }

			free = ((!e.Handled) && (!_Parent._IsCompletionVisible(Control)) && (e.Modifiers == Keys.None));

			if(free && (e.KeyCode == Keys.Escape))
			{
				if(_WindowVisible) { e.Handled = e.SuppressKeyPress = true; }
				_Close(SignatureHelpCloseReason.CANCELLED);
				return;
			}

			if(free && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)) && _WindowVisible && (_Signatures.Length > 1))
			{
				Cycle((e.KeyCode == Keys.Up) ? -1 : 1);
				e.Handled = e.SuppressKeyPress = true;
				return;
			}

			if((!e.Handled) && _IsCaretKey(e.KeyCode)) { _UpdateLater(); }
		}


		/// <summary>Handles the KeyPress event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_KeyPress(object? sender, KeyPressEventArgs e)
		{
			if(char.IsControl(e.KeyChar)) { return; }

			_KeyPending = true;
			_KeyChar = e.KeyChar;
		}


		/// <summary>Handles the TextChanged event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		/// <remarks>A text change counts as typed if a character has been typed and precedes the caret. Otherwise the change has been
		///          caused by something else (for example an auto completion inserting text before the typed character is inserted),
		///          and the typed character is kept for the next change.</remarks>
		private void Control_TextChanged(object? sender, EventArgs e)
		{
			string text;
			int caret;
			bool typed;

			if(_Disposed || (_TrackingSuspended > 0)) { return; }

			text = Control.Text;
			caret = _GetCaret(text);
			typed = (_KeyPending && (caret > 0) && (text[caret - 1] == _KeyChar));

			if(typed) { _KeyPending = false; }

			if(_State != _SessionState.NONE)
			{
				_Update();
				return;
			}

			if(typed && AutoStart && _TriggerCharacters.Contains(_KeyChar)) { _TryStart(SignatureHelpTrigger.TRIGGER_CHARACTER, _KeyChar); }
		}


		/// <summary>Handles the MouseDown event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_MouseDown(object? sender, MouseEventArgs e)
		{
			_UpdateLater();
		}


		/// <summary>Handles the LostFocus event of the control.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void Control_LostFocus(object? sender, EventArgs e)
		{
			_Close(SignatureHelpCloseReason.FOCUS_LOST);
		}


		/// <summary>Handles the Deactivate event of the form.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void _Form_Deactivate(object? sender, EventArgs e)
		{
			_Close(SignatureHelpCloseReason.FOCUS_LOST);
		}


		/// <summary>Handles the Move and Resize events of the form.</summary>
		/// <param name="sender">Sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void _Form_Move(object? sender, EventArgs e)
		{
			if(_WindowVisible) { _Show(); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IMessageFilter                                                                                   //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Filters mouse clicks while a session is active and ends the session on clicks outside the signature window, the
		///          text area of the control and the windows of the linked auto completion provider.</summary>
		/// <param name="m">Message.</param>
		/// <returns>Returns <c>false</c>; messages are never consumed.</returns>
		public bool PreFilterMessage(ref Message m)
		{
			bool client = ((m.Msg == _WM_LBUTTONDOWN) || (m.Msg == _WM_RBUTTONDOWN) || (m.Msg == _WM_MBUTTONDOWN) || (m.Msg == _WM_XBUTTONDOWN));
			bool nonClient = ((m.Msg == _WM_NCLBUTTONDOWN) || (m.Msg == _WM_NCRBUTTONDOWN) || (m.Msg == _WM_NCMBUTTONDOWN) || (m.Msg == _WM_NCXBUTTONDOWN));
			_SignatureHelpWindow? window = _Parent._Window;

			if((!(client || nonClient)) || (_State == _SessionState.NONE)) { return false; }

			if((window is not null) && window.IsHandleCreated && (m.HWnd == window.Handle)) { return false; }
			if(client && Control.IsHandleCreated && (m.HWnd == Control.Handle)) { return false; }
			if(_Parent._IsCompletionWindow(m.HWnd)) { return false; }

			_Close(SignatureHelpCloseReason.CLICKED_OUTSIDE);
			return false;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IDisposable                                                                                      //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public void Dispose()
		{
			if(_Disposed) { return; }

			_Close(SignatureHelpCloseReason.MANUAL);
			_Disposed = true;
			_Unhook();
			_UnhookForm();
			_RemoveMessageFilter();
			Control.Disposed -= Control_Disposed;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [enum] _SessionState                                                                                         //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>This enumeration defines the session states.</summary>
		private enum _SessionState: int
		{
			/// <summary>No session.</summary>
			NONE = 0,

			/// <summary>Signatures are being requested.</summary>
			PENDING = 1,

			/// <summary>Signatures are available.</summary>
			ACTIVE = 2
		}
	}
}
