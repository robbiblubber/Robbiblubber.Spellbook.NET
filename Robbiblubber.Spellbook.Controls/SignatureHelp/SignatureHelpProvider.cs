namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class implements a component that shows the signature of the function call at the caret of text boxes.</summary>
/// <remarks>While the caret is within the argument list of a call, a window above the line shows the signature with the active parameter
///          highlighted. The call at the caret is found by an <see cref="ISignatureCallParser"/>; the signatures are provided by an
///          <see cref="ISignatureHelpSource"/>, which is asked once per call. Moving between the arguments of a call only updates the
///          active parameter. Several signatures (overloads) can be cycled with the Up and Down keys. If <see cref="AutoCompletion"/>
///          is set, an open auto completion list takes precedence for navigation keys, and inserting an auto completion item that
///          contains an opening parenthesis starts the signature help.</remarks>
[ProvideProperty("SignatureHelpEnabled", typeof(Control))]
[ProvideProperty("AutoStart", typeof(Control))]
[ProvideProperty("ShortcutKeys", typeof(Control))]
[ProvideProperty("TriggerCharacters", typeof(Control))]
[SupportedOSPlatform("windows")]
public partial class SignatureHelpProvider: Component, IExtenderProvider, IComponent, IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Bindings by control.</summary>
	private readonly Dictionary<TextBoxBase, _Binding> _Bindings = new();

	/// <summary>Signature window.</summary>
	private _SignatureHelpWindow? _Window = null;

	/// <summary>Binding with an active session.</summary>
	private _Binding? _ActiveBinding = null;

	/// <summary>Linked auto completion provider.</summary>
	private AutoCompletionProvider? _AutoCompletion = null;

	/// <summary>Call parser.</summary>
	private ISignatureCallParser _Parser = new SignatureCallParser();

	/// <summary>Maximum window width in logical units.</summary>
	private int _MaximumWidth = 600;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public SignatureHelpProvider(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public SignatureHelpProvider(IContainer container): this()
	{
		container.Add(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Validates a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the control as text box.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	private static TextBoxBase _Validate(Control control)
	{
		if(control is null) { throw new ArgumentNullException(nameof(control)); }
		if(control is TextBoxBase rval) { return rval; }

		throw new ArgumentException("Control must be a TextBoxBase.", nameof(control));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or creates the binding of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the binding.</returns>
	private _Binding _Bind(Control control)
	{
		TextBoxBase t = _Validate(control);

		if(!_Bindings.TryGetValue(t, out _Binding? rval))
		{
			rval = new _Binding(this, t);
			_Bindings.Add(t, rval);
		}

		return rval;
	}


	/// <summary>Gets the binding of a control, if it exists.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the binding, or NULL if the control is not bound.</returns>
	private _Binding? _TryGetBinding(Control control)
	{
		return (_Bindings.TryGetValue(_Validate(control), out _Binding? rval) ? rval : null);
	}


	/// <summary>Removes the binding of a control.</summary>
	/// <param name="control">Control.</param>
	private void _Unbind(TextBoxBase control)
	{
		if(_Bindings.Remove(control, out _Binding? binding)) { binding.Dispose(); }
	}


	/// <summary>Gets or creates the signature window.</summary>
	/// <returns>Returns the window.</returns>
	private _SignatureHelpWindow _GetWindow()
	{
		if((_Window is null) || _Window.IsDisposed)
		{
			_Window = new _SignatureHelpWindow();
			_Window.PreviousClicked += _Window_PreviousClicked;
			_Window.NextClicked += _Window_NextClicked;
		}

		return _Window;
	}


	/// <summary>Makes a binding the binding with the active session and ends the session of another binding.</summary>
	/// <param name="binding">Binding.</param>
	private void _Activate(_Binding binding)
	{
		if((_ActiveBinding is not null) && (_ActiveBinding != binding)) { _ActiveBinding.Close(SignatureHelpCloseReason.FOCUS_LOST); }
		_ActiveBinding = binding;
	}


	/// <summary>Returns if the linked auto completion list is visible for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the auto completion list is visible, otherwise returns <c>false</c>.</returns>
	private bool _IsCompletionVisible(TextBoxBase control)
	{
		return ((_AutoCompletion is not null) && _AutoCompletion._IsListVisible(control));
	}


	/// <summary>Returns if a window handle belongs to the linked auto completion provider.</summary>
	/// <param name="handle">Window handle.</param>
	/// <returns>Returns <c>true</c> if the handle belongs to an auto completion window, otherwise returns <c>false</c>.</returns>
	private bool _IsCompletionWindow(IntPtr handle)
	{
		return ((_AutoCompletion is not null) && _AutoCompletion._OwnsWindow(handle));
	}


	/// <summary>Resolves a color: the own color, the color of the linked auto completion provider, or a default.</summary>
	/// <param name="own">Own color.</param>
	/// <param name="linked">Color of the linked auto completion provider.</param>
	/// <param name="fallback">Default color.</param>
	/// <returns>Returns the resolved color.</returns>
	private Color _ResolveColor(Color own, Color? linked, Color fallback)
	{
		if(!own.IsEmpty) { return own; }
		if((_AutoCompletion is not null) && (linked is Color color) && (!color.IsEmpty)) { return color; }

		return fallback;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="Starting"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnStarting(SignatureHelpStartingEventArgs e)
	{
		Starting?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Populating"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnPopulating(SignatureHelpPopulatingEventArgs e)
	{
		Populating?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="SelectionChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectionChanged(SignatureHelpSelectionChangedEventArgs e)
	{
		SelectionChanged?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Closed"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnClosed(SignatureHelpClosedEventArgs e)
	{
		Closed?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when a signature help session is about to start.</summary>
	[Category("Signature Help")][Description("Occurs when a signature help session is about to start. The session can be cancelled.")]
	public event EventHandler<SignatureHelpStartingEventArgs>? Starting;

	/// <summary>Occurs when the signatures of a call have been retrieved from the source and before they are displayed.</summary>
	[Category("Signature Help")][Description("Occurs before the signatures of a call are displayed. Signatures can be added, removed or replaced.")]
	public event EventHandler<SignatureHelpPopulatingEventArgs>? Populating;

	/// <summary>Occurs when the displayed signature or the active parameter has changed.</summary>
	[Category("Signature Help")][Description("Occurs when the displayed signature or the active parameter has changed.")]
	public event EventHandler<SignatureHelpSelectionChangedEventArgs>? SelectionChanged;

	/// <summary>Occurs when a signature help session has ended.</summary>
	[Category("Signature Help")][Description("Occurs when a signature help session has ended.")]
	public event EventHandler<SignatureHelpClosedEventArgs>? Closed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the default signature source.</summary>
	/// <remarks>A source set for a control takes precedence over this source.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ISignatureHelpSource? Source
	{
		get; set;
	}


	/// <summary>Gets or sets the default call parser.</summary>
	/// <remarks>A parser set for a control takes precedence over this parser. Setting NULL restores a default <see cref="SignatureCallParser"/>.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ISignatureCallParser Parser
	{
		get { return _Parser; }
		set { _Parser = (value ?? new SignatureCallParser()); }
	}


	/// <summary>Gets or sets the linked auto completion provider.</summary>
	/// <remarks>While the auto completion list is open, it receives Escape and the navigation keys. Inserting an auto completion item
	///          that contains an opening parenthesis starts the signature help. Colors that are not set fall back to the colors of
	///          the auto completion provider.</remarks>
	[Category("Signature Help")][Description("Auto completion provider used together with this provider. Its list takes precedence for navigation keys; colors are inherited.")][DefaultValue(null)]
	public AutoCompletionProvider? AutoCompletion
	{
		get { return _AutoCompletion; }
		set
		{
			if(value == _AutoCompletion) { return; }

			if(_AutoCompletion is not null)
			{
				_AutoCompletion.Accepted -= _AutoCompletion_Accepted;
				_AutoCompletion.Disposed -= _AutoCompletion_Disposed;
			}

			_AutoCompletion = value;

			if(_AutoCompletion is not null)
			{
				_AutoCompletion.Accepted += _AutoCompletion_Accepted;
				_AutoCompletion.Disposed += _AutoCompletion_Disposed;
			}
		}
	}


	/// <summary>Gets or sets the characters that start signature help when typed within a call.</summary>
	[Category("Signature Help")][Description("Characters that start signature help when typed within a call.")][DefaultValue("(,")]
	public string TriggerCharacters
	{
		get; set;
	} = "(,";


	/// <summary>Gets or sets if the descriptions of the signature and the active parameter are displayed.</summary>
	[Category("Signature Help")][Description("Specifies if the descriptions of the signature and the active parameter are displayed.")][DefaultValue(true)]
	public bool ShowDescriptions
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the maximum window width in logical units.</summary>
	/// <remarks>Longer signatures wrap before a parameter.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 100.</exception>
	[Category("Signature Help")][Description("Maximum width of the signature window in logical units. Longer signatures wrap.")][DefaultValue(600)]
	public int MaximumWidth
	{
		get { return _MaximumWidth; }
		set
		{
			if(value < 100) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be at least 100."); }
			_MaximumWidth = value;
		}
	}


	/// <summary>Gets or sets the back color of the signature window.</summary>
	/// <remarks>If the color is empty, the list back color of the linked auto completion provider or the back color of the control is used.</remarks>
	[Category("Signature Help Colors")][Description("Back color of the signature window. Empty uses the auto completion list color or the control color.")][DefaultValue(typeof(Color), "")]
	public Color WindowBackColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the fore color of the signature window.</summary>
	/// <remarks>If the color is empty, the list fore color of the linked auto completion provider or the fore color of the control is used.</remarks>
	[Category("Signature Help Colors")][Description("Fore color of the signature window. Empty uses the auto completion list color or the control color.")][DefaultValue(typeof(Color), "")]
	public Color WindowForeColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the color of the signature counter and the suffix.</summary>
	/// <remarks>If the color is empty, the detail color of the linked auto completion provider or the gray text color of the theme is
	///          used.</remarks>
	[Category("Signature Help Colors")][Description("Color of the signature counter and the suffix. Empty uses the auto completion detail color or the gray text color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color DetailForeColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the color of the active parameter if <see cref="HighlightStyle"/> is <see cref="ListHighlightStyle.COLOR"/>.</summary>
	/// <remarks>If the color is empty, the highlight color of the linked auto completion provider or the link color of the theme is
	///          used.</remarks>
	[Category("Signature Help Colors")][Description("Color of the active parameter with highlight style COLOR. Empty uses the auto completion highlight color or the link color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color HighlightColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets how the active parameter is highlighted.</summary>
	/// <remarks><see cref="ListHighlightStyle.COLOR"/> draws the active parameter bold in the highlight color.</remarks>
	[Category("Signature Help Colors")][Description("Specifies how the active parameter is highlighted.")][DefaultValue(ListHighlightStyle.BOLD)]
	public ListHighlightStyle HighlightStyle
	{
		get; set;
	} = ListHighlightStyle.BOLD;


	/// <summary>Gets or sets the border color of the signature window.</summary>
	/// <remarks>If the color is empty, the border color of the linked auto completion provider or the popup border color of the theme
	///          is used (the system border for themes that use the system colors).</remarks>
	[Category("Signature Help Colors")][Description("Border color of the signature window. Empty uses the auto completion border color or the border color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color BorderColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets the control with an active signature help session.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TextBoxBase? ActiveControl
	{
		get { return _ActiveBinding?.Control; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if signature help is enabled for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if signature help is enabled, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Signature Help")][Description("Enables signature help for this control.")][DefaultValue(false)]
	public virtual bool GetSignatureHelpEnabled(Control control)
	{
		return _TryGetBinding(control)?.Enabled ?? false;
	}


	/// <summary>Sets if signature help is enabled for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetSignatureHelpEnabled(Control control, bool value)
	{
		_Bind(control).Enabled = value;
	}


	/// <summary>Gets if signature help starts automatically for a control when a trigger character is typed.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if signature help starts automatically, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Signature Help")][Description("Specifies if signature help starts automatically when a trigger character is typed.")][DefaultValue(true)]
	public virtual bool GetAutoStart(Control control)
	{
		return _TryGetBinding(control)?.AutoStart ?? true;
	}


	/// <summary>Sets if signature help starts automatically for a control when a trigger character is typed.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetAutoStart(Control control, bool value)
	{
		_Bind(control).AutoStart = value;
	}


	/// <summary>Gets the shortcut keys that show the signature help for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the shortcut keys.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Signature Help")][Description("Shortcut keys that show the signature help.")][DefaultValue(Keys.Control | Keys.Shift | Keys.Space)]
	public virtual Keys GetShortcutKeys(Control control)
	{
		return _TryGetBinding(control)?.ShortcutKeys ?? (Keys.Control | Keys.Shift | Keys.Space);
	}


	/// <summary>Sets the shortcut keys that show the signature help for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetShortcutKeys(Control control, Keys value)
	{
		_Bind(control).ShortcutKeys = value;
	}


	/// <summary>Gets the trigger characters of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the trigger characters, or NULL if the provider setting applies.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Signature Help")][Description("Characters that start signature help within a call. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual string? GetTriggerCharacters(Control control)
	{
		return _TryGetBinding(control)?.TriggerCharacters;
	}


	/// <summary>Sets the trigger characters of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Trigger characters, NULL to use the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetTriggerCharacters(Control control, string? value)
	{
		_Bind(control).TriggerCharacters = value;
	}


	/// <summary>Gets the signature source of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the source, or NULL if the provider source applies.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual ISignatureHelpSource? GetSource(Control control)
	{
		return _TryGetBinding(control)?.Source;
	}


	/// <summary>Sets the signature source of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Source, NULL to use the provider source.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetSource(Control control, ISignatureHelpSource? value)
	{
		_Bind(control).Source = value;
	}


	/// <summary>Gets the call parser of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the parser, or NULL if the provider parser applies.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual ISignatureCallParser? GetParser(Control control)
	{
		return _TryGetBinding(control)?.Parser;
	}


	/// <summary>Sets the call parser of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Parser, NULL to use the provider parser.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetParser(Control control, ISignatureCallParser? value)
	{
		_Bind(control).Parser = value;
	}


	/// <summary>Suspends tracking of text and caret changes of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns an object that resumes tracking when disposed.</returns>
	/// <remarks>Use this method when code changes the control text or formatting without changing the call at the caret, for example
	///          a syntax highlighter. When tracking resumes, the call at the caret is checked again. Suspensions can be nested.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual IDisposable SuspendTracking(Control control)
	{
		_Binding binding = _Bind(control);

		binding.SuspendTracking();
		return new _TrackingSuspension(binding);
	}


	/// <summary>Shows the signature help for the call at the caret of a control.</summary>
	/// <param name="control">Control.</param>
	/// <remarks>If the caret is not within the argument list of a call, nothing happens.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	/// <exception cref="InvalidOperationException">Thrown when signature help is not enabled for the control.</exception>
	public virtual void ShowSignatureHelp(Control control)
	{
		_Binding binding = _Bind(control);

		if(!binding.Enabled) { throw new InvalidOperationException("Signature help is not enabled for this control."); }
		binding.Start(SignatureHelpTrigger.MANUAL);
	}


	/// <summary>Closes the signature help.</summary>
	public virtual void CloseSignatureHelp()
	{
		_ActiveBinding?.Close(SignatureHelpCloseReason.MANUAL);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the PreviousClicked event of the signature window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Window_PreviousClicked(object? sender, EventArgs e)
	{
		_ActiveBinding?.Cycle(-1);
	}


	/// <summary>Handles the NextClicked event of the signature window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Window_NextClicked(object? sender, EventArgs e)
	{
		_ActiveBinding?.Cycle(1);
	}


	/// <summary>Handles the Accepted event of the linked auto completion provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AutoCompletion_Accepted(object? sender, AutoCompletionAcceptedEventArgs e)
	{
		if(e.InsertText.Contains('(') && _Bindings.TryGetValue(e.Control, out _Binding? binding)) { binding.StartAfterCompletion(); }
	}


	/// <summary>Handles the Disposed event of the linked auto completion provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AutoCompletion_Disposed(object? sender, EventArgs e)
	{
		AutoCompletion = null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Component                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			foreach(_Binding i in _Bindings.Values.ToArray()) { i.Dispose(); }
			_Bindings.Clear();
			_ActiveBinding = null;
			AutoCompletion = null;

			if(_Window is not null)
			{
				_Window.PreviousClicked -= _Window_PreviousClicked;
				_Window.NextClicked -= _Window_NextClicked;
				_Window.Dispose();
				_Window = null;
			}
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IExtenderProvider                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual bool CanExtend(object extendee)
	{
		return (extendee is TextBoxBase);
	}
}
