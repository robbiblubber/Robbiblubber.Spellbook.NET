namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This component provides auto completion capabilities to text boxes.</summary>
/// <remarks>Items are provided by an <see cref="IAutoCompletionSource"/> (per control or for all controls) and can be modified or
///          provided in the <see cref="Populating"/> event. The source is called asynchronously, the event is raised on the UI thread
///          after the source has returned. Settings resolve in the order result, control, provider.</remarks>
[ProvideProperty("AutoCompletionEnabled", typeof(Control))]
[ProvideProperty("AutoStart", typeof(Control))]
[ProvideProperty("ShortcutKeys", typeof(Control))]
[ProvideProperty("MatchMode", typeof(Control))]
[ProvideProperty("FilterMode", typeof(Control))]
[ProvideProperty("CaseSensitive", typeof(Control))]
[ProvideProperty("WordCharacters", typeof(Control))]
[ProvideProperty("TriggerCharacters", typeof(Control))]
[ProvideProperty("CommitCharacters", typeof(Control))]
[ProvideProperty("BoxSize", typeof(Control))]
[ProvideProperty("BoxAutoSize", typeof(Control))]
[ProvideProperty("BoxResizable", typeof(Control))]
[ProvideProperty("UsageRanking", typeof(Control))]
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(AutoCompletionProvider), "Resources.autocompletion.bmp")]
public partial class AutoCompletionProvider: Component, IExtenderProvider, IComponent, IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Control bindings.</summary>
	private readonly Dictionary<TextBoxBase, _Binding> _Bindings = new();

	/// <summary>Popup window.</summary>
	private _ListPopupWindow<IAutoCompletionItem>? _Popup = null;

	/// <summary>Binding with an active auto completion session.</summary>
	private _Binding? _ActiveBinding = null;

	/// <summary>Typing delay.</summary>
	private int _Delay = 50;

	/// <summary>Minimum word length.</summary>
	private int _MinimumWordLength = 1;

	/// <summary>Description window.</summary>
	private _ListDescriptionWindow? _Description = null;

	/// <summary>Usage history.</summary>
	private AutoCompletionUsageHistory _UsageHistory = new();

	/// <summary>Image list.</summary>
	private ImageList? _ImageList = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public AutoCompletionProvider(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public AutoCompletionProvider(IContainer container): this()
	{
		container.Add(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Validates a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the control as a <see cref="TextBoxBase"/>.</returns>
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

	/// <summary>Gets the binding for a control, creating it if necessary.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the binding.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
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


	/// <summary>Gets the binding for a control if it exists.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the binding, or NULL if the control is not bound.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
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


	/// <summary>Gets the popup window, creating it if necessary.</summary>
	/// <returns>Returns the popup window.</returns>
	private _ListPopupWindow<IAutoCompletionItem> _GetPopup()
	{
		if((_Popup is null) || _Popup.IsDisposed)
		{
			_Popup = new _ListPopupWindow<IAutoCompletionItem>();
			_Popup.ItemClicked += _Popup_ItemClicked;
			_Popup.ItemActivated += _Popup_ItemActivated;
			_Popup.SelectionChanged += _Popup_SelectionChanged;
			_Popup.UserResized += _Popup_UserResized;
		}

		return _Popup;
	}


	/// <summary>Gets the colors and highlight style of the popup window.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the style; colors that are not set fall back to the colors of the control and its theme.</returns>
	private _ListPopupStyle _GetPopupStyle(Control control)
	{
		Theme theme = ThemeManager.GetTheme(control);

		return new _ListPopupStyle()
		{
			BackColor = (ListBackColor.IsEmpty ? theme.MapSystemColor(control.BackColor) : ListBackColor),
			ForeColor = (ListForeColor.IsEmpty ? theme.MapSystemColor(control.ForeColor) : ListForeColor),
			SelectionBackColor = (SelectionBackColor.IsEmpty ? theme[ThemeColor.SELECTION_BACK] : SelectionBackColor),
			SelectionForeColor = (SelectionForeColor.IsEmpty ? theme[ThemeColor.SELECTION_TEXT] : SelectionForeColor),
			DetailForeColor = (DetailForeColor.IsEmpty ? theme[ThemeColor.GRAY_TEXT] : DetailForeColor),
			HighlightStyle = HighlightStyle,
			HighlightColor = (HighlightColor.IsEmpty ? theme[ThemeColor.LINK] : HighlightColor),
			BorderColor = ((BorderColor.IsEmpty && !theme.IsSystem) ? theme[ThemeColor.POPUP_BORDER] : BorderColor)
		};
	}


	/// <summary>Gets the description window, creating it if necessary.</summary>
	/// <returns>Returns the description window.</returns>
	private _ListDescriptionWindow _GetDescriptionWindow()
	{
		if((_Description is null) || _Description.IsDisposed) { _Description = new _ListDescriptionWindow(); }
		return _Description;
	}


	/// <summary>Makes a binding the active binding, closing the session of the previously active binding.</summary>
	/// <param name="binding">Binding.</param>
	private void _Activate(_Binding binding)
	{
		if((_ActiveBinding is not null) && (_ActiveBinding != binding)) { _ActiveBinding.Close(AutoCompletionCloseReason.FOCUS_LOST); }
		_ActiveBinding = binding;
	}


	/// <summary>Returns if the auto completion list is visible for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the list is visible for the control, otherwise returns <c>false</c>.</returns>
	/// <remarks>Used by <see cref="SignatureHelpProvider"/> to leave navigation keys to an open auto completion list.</remarks>
	internal bool _IsListVisible(TextBoxBase control)
	{
		return ((_ActiveBinding is not null) && (_ActiveBinding.Control == control) && (_Popup is not null) && (!_Popup.IsDisposed) && _Popup.Visible);
	}


	/// <summary>Returns if a window handle belongs to one of the auto completion windows.</summary>
	/// <param name="handle">Window handle.</param>
	/// <returns>Returns <c>true</c> if the handle belongs to the auto completion list or its description, otherwise returns <c>false</c>.</returns>
	/// <remarks>Used by <see cref="SignatureHelpProvider"/>, so clicks into the auto completion list do not close the signature help.</remarks>
	internal bool _OwnsWindow(IntPtr handle)
	{
		if((_Popup is not null) && _Popup.IsHandleCreated && (_Popup.Handle == handle)) { return true; }
		return ((_Description is not null) && _Description.IsHandleCreated && (_Description.Handle == handle));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="Starting"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnStarting(AutoCompletionStartingEventArgs e)
	{
		Starting?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Populating"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnPopulating(AutoCompletionPopulatingEventArgs e)
	{
		Populating?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Accepting"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnAccepting(AutoCompletionAcceptingEventArgs e)
	{
		Accepting?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Accepted"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnAccepted(AutoCompletionAcceptedEventArgs e)
	{
		Accepted?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="SelectionChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectionChanged(AutoCompletionSelectionChangedEventArgs e)
	{
		SelectionChanged?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Closed"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnClosed(AutoCompletionClosedEventArgs e)
	{
		Closed?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when an auto completion session is about to start.</summary>
	[Category("Auto Completion")][Description("Occurs when an auto completion session is about to start. The session can be cancelled.")]
	public event EventHandler<AutoCompletionStartingEventArgs>? Starting;


	/// <summary>Occurs when the auto completion items have been retrieved from the source and before they are displayed.</summary>
	[Category("Auto Completion")][Description("Occurs before the auto completion items are displayed. Items can be added, removed or replaced.")]
	public event EventHandler<AutoCompletionPopulatingEventArgs>? Populating;


	/// <summary>Occurs when an item is about to be accepted.</summary>
	[Category("Auto Completion")][Description("Occurs when an item is about to be inserted. The inserted text can be modified and the insertion can be cancelled.")]
	public event EventHandler<AutoCompletionAcceptingEventArgs>? Accepting;


	/// <summary>Occurs when an item has been accepted.</summary>
	[Category("Auto Completion")][Description("Occurs when an item has been inserted.")]
	public event EventHandler<AutoCompletionAcceptedEventArgs>? Accepted;


	/// <summary>Occurs when an auto completion session has ended.</summary>
	[Category("Auto Completion")][Description("Occurs when an auto completion session has ended.")]
	public event EventHandler<AutoCompletionClosedEventArgs>? Closed;


	/// <summary>Occurs when the selected item of the auto completion list has changed.</summary>
	[Category("Auto Completion")][Description("Occurs when the selected item of the auto completion list has changed.")]
	public event EventHandler<AutoCompletionSelectionChangedEventArgs>? SelectionChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the image list used for item images.</summary>
	[Category("Auto Completion")][Description("ImageList used to draw item images referenced by ImageKey.")]
	[DefaultValue(null)][RefreshProperties(RefreshProperties.Repaint)][TypeConverter(typeof(ReferenceConverter))]
	public ImageList? ImageList
	{
		get { return _ImageList; }
		set
		{
			if(value == _ImageList) { return; }

			if(_ImageList is not null) { _ImageList.Disposed -= _ImageList_Disposed; }
			_ImageList = value;
			if(_ImageList is not null) { _ImageList.Disposed += _ImageList_Disposed; }
		}
	}


	/// <summary>Gets or sets the default auto completion source for all controls.</summary>
	/// <remarks>A source set for a control takes precedence over this source.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IAutoCompletionSource? Source
	{
		get; set;
	}


	/// <summary>Gets or sets the default match mode.</summary>
	[Category("Auto Completion")][Description("Default match mode.")][DefaultValue(ListMatchMode.PREFIX)]
	public ListMatchMode MatchMode
	{
		get; set;
	} = ListMatchMode.PREFIX;


	/// <summary>Gets or sets the default filter mode.</summary>
	[Category("Auto Completion")][Description("Default filter mode.")][DefaultValue(ListFilterMode.FILTER)]
	public ListFilterMode FilterMode
	{
		get; set;
	} = ListFilterMode.FILTER;


	/// <summary>Gets or sets if matching is case-sensitive by default.</summary>
	[Category("Auto Completion")][Description("Specifies if matching is case-sensitive by default.")][DefaultValue(false)]
	public bool CaseSensitive
	{
		get; set;
	}


	/// <summary>Gets or sets the characters that are part of a word in addition to letters and digits.</summary>
	[Category("Auto Completion")][Description("Characters that are part of a word in addition to letters and digits.")][DefaultValue("_")]
	public string WordCharacters
	{
		get; set;
	} = "_";


	/// <summary>Gets or sets the characters that start auto completion when typed.</summary>
	[Category("Auto Completion")][Description("Characters that start auto completion when typed.")][DefaultValue(".")]
	public string TriggerCharacters
	{
		get; set;
	} = ".";


	/// <summary>Gets or sets the characters that accept the selected item when typed.</summary>
	/// <remarks>Commit characters only accept an item if the selection is unambiguous: the user has selected the item explicitly,
	///          the item matches the typed text exactly, or it is the only item that starts with the typed text.</remarks>
	[Category("Auto Completion")][Description("Characters that accept the selected item when typed, if the selection is unambiguous.")][DefaultValue("")]
	public string CommitCharacters
	{
		get; set;
	} = string.Empty;


	/// <summary>Gets or sets if Space accepts the selected item.</summary>
	/// <remarks>Space follows the same rules as <see cref="CommitCharacters"/>.</remarks>
	[Category("Auto Completion")][Description("Specifies if Space accepts the selected item, if the selection is unambiguous.")][DefaultValue(false)]
	public bool AcceptOnSpace
	{
		get; set;
	}


	/// <summary>Gets or sets if Tab accepts the selected item.</summary>
	[Category("Auto Completion")][Description("Specifies if Tab accepts the selected item.")][DefaultValue(true)]
	public bool AcceptOnTab
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if Enter accepts the selected item.</summary>
	[Category("Auto Completion")][Description("Specifies if Enter accepts the selected item.")][DefaultValue(true)]
	public bool AcceptOnEnter
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the delay in milliseconds before items are requested when typing.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
	[Category("Auto Completion")][Description("Delay in milliseconds before items are requested when typing.")][DefaultValue(50)]
	public int Delay
	{
		get { return _Delay; }
		set
		{
			if(value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative."); }
			_Delay = value;
		}
	}


	/// <summary>Gets or sets the minimum number of word characters that have to be typed before auto completion starts automatically.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
	[Category("Auto Completion")][Description("Minimum number of characters typed before auto completion starts automatically.")][DefaultValue(1)]
	public int MinimumWordLength
	{
		get { return _MinimumWordLength; }
		set
		{
			if(value < 1) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be at least 1."); }
			_MinimumWordLength = value;
		}
	}


	/// <summary>Gets or sets the default size of the auto completion window in logical units (96 dpi).</summary>
	/// <remarks>The size is used if <see cref="BoxAutoSize"/> is false. The height is the maximum height; the window shrinks to the
	///          number of items.</remarks>
	[Category("Auto Completion")][Description("Default size of the auto completion window.")][DefaultValue(typeof(Size), "300, 200")]
	public Size BoxSize
	{
		get; set;
	} = new Size(300, 200);


	/// <summary>Gets or sets if the auto completion window adjusts its width to the items by default.</summary>
	/// <remarks>The width is limited by <see cref="MinimumBoxSize"/> and <see cref="MaximumBoxSize"/>. Within a session, the window only
	///          grows, so it does not jump while typing. If this property is false, <see cref="BoxSize"/> is used.</remarks>
	[Category("Auto Completion")][Description("Specifies if the auto completion window adjusts its width to the items by default.")][DefaultValue(true)]
	public bool BoxAutoSize
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the minimum size of the auto completion window in logical units (96 dpi).</summary>
	/// <remarks>A height of 0 lets the window shrink to the number of items.</remarks>
	[Category("Auto Completion")][Description("Minimum size of the auto completion window. A height of 0 lets the window shrink to the number of items.")][DefaultValue(typeof(Size), "160, 0")]
	public Size MinimumBoxSize
	{
		get; set;
	} = new Size(160, 0);


	/// <summary>Gets or sets the maximum size of the auto completion window in logical units (96 dpi).</summary>
	[Category("Auto Completion")][Description("Maximum size of the auto completion window.")][DefaultValue(typeof(Size), "600, 320")]
	public Size MaximumBoxSize
	{
		get; set;
	} = new Size(600, 320);


	/// <summary>Gets or sets if the auto completion window can be resized by the user by default.</summary>
	/// <remarks>A resizable window shows a footer with the item count and a size grip; it can also be resized at its right and bottom
	///          edges. The size chosen by the user is kept for the control until the application ends.</remarks>
	[Category("Auto Completion")][Description("Specifies if the auto completion window can be resized by the user by default.")][DefaultValue(true)]
	public bool BoxResizable
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if the description of the selected item is shown next to the auto completion list.</summary>
	[Category("Auto Completion")][Description("Specifies if the description of the selected item is shown next to the auto completion list.")][DefaultValue(true)]
	public bool ShowDescriptions
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if a single matching item is inserted directly when auto completion is started explicitly.</summary>
	/// <remarks>This only applies to the shortcut keys and <see cref="ShowCompletion"/>, never to typing.</remarks>
	[Category("Auto Completion")][Description("Specifies if a single matching item is inserted directly when auto completion is started with the shortcut keys or by code.")][DefaultValue(true)]
	public bool AcceptSingleMatch
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if frequently accepted items are ranked higher by default.</summary>
	/// <remarks>Usage only ranks items within the same match class: a frequently used fuzzy match never precedes a prefix match.</remarks>
	[Category("Auto Completion")][Description("Specifies if frequently accepted items are ranked higher by default.")][DefaultValue(true)]
	public bool UsageRanking
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the usage history.</summary>
	/// <remarks>The history can be shared between providers and persisted with <see cref="AutoCompletionUsageHistory.Export"/> and
	///          <see cref="AutoCompletionUsageHistory.Import"/>. Usage is recorded per control name: controls without a name share
	///          one history, and renaming a control starts a new one.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the value is NULL.</exception>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public AutoCompletionUsageHistory UsageHistory
	{
		get { return _UsageHistory; }
		set { _UsageHistory = (value ?? throw new ArgumentNullException(nameof(value))); }
	}


	/// <summary>Gets or sets the back color of the auto completion list.</summary>
	/// <remarks>If the color is empty, the back color of the control is used; system colors are replaced by the matching color of the
	///          theme of the control (<see cref="ThemeManager.GetTheme(Control)"/>).</remarks>
	[Category("Auto Completion Colors")][Description("Back color of the auto completion list. Empty uses the back color of the control.")][DefaultValue(typeof(Color), "")]
	public Color ListBackColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the fore color of the auto completion list.</summary>
	/// <remarks>If the color is empty, the fore color of the control is used; system colors are replaced by the matching color of the
	///          theme of the control.</remarks>
	[Category("Auto Completion Colors")][Description("Fore color of the auto completion list. Empty uses the fore color of the control.")][DefaultValue(typeof(Color), "")]
	public Color ListForeColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the back color of the selected item.</summary>
	/// <remarks>If the color is empty, the selection color of the theme (<see cref="ThemeColor.SELECTION_BACK"/>) is used.</remarks>
	[Category("Auto Completion Colors")][Description("Back color of the selected item. Empty uses the selection color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color SelectionBackColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the fore color of the selected item.</summary>
	/// <remarks>If the color is empty, the selection text color of the theme (<see cref="ThemeColor.SELECTION_TEXT"/>) is used.</remarks>
	[Category("Auto Completion Colors")][Description("Fore color of the selected item. Empty uses the selection text color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color SelectionForeColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the color of detail texts, the footer and the loading indicator.</summary>
	/// <remarks>If the color is empty, the gray text color of the theme (<see cref="ThemeColor.GRAY_TEXT"/>) is used.</remarks>
	[Category("Auto Completion Colors")][Description("Color of detail texts, the footer and the loading indicator. Empty uses the gray text color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color DetailForeColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets how matched characters are highlighted.</summary>
	[Category("Auto Completion Colors")][Description("Specifies how matched characters are highlighted.")][DefaultValue(ListHighlightStyle.BOLD)]
	public ListHighlightStyle HighlightStyle
	{
		get; set;
	} = ListHighlightStyle.BOLD;


	/// <summary>Gets or sets the color of matched characters if <see cref="HighlightStyle"/> is <see cref="ListHighlightStyle.COLOR"/>.</summary>
	/// <remarks>If the color is empty, the link color of the theme (<see cref="ThemeColor.LINK"/>) is used.</remarks>
	[Category("Auto Completion Colors")][Description("Color of matched characters for highlight style COLOR. Empty uses the link color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color HighlightColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the border color of the auto completion windows.</summary>
	/// <remarks>If the color is empty, the popup border color of the theme (<see cref="ThemeColor.POPUP_BORDER"/>) is used, or the
	///          system border for themes that use the system colors.</remarks>
	[Category("Auto Completion Colors")][Description("Border color of the auto completion windows. Empty uses the border color of the theme.")][DefaultValue(typeof(Color), "")]
	public Color BorderColor
	{
		get; set;
	} = Color.Empty;


	/// <summary>Gets or sets the text displayed while items are loading.</summary>
	[Category("Auto Completion")][Description("Text displayed while items are loading.")][DefaultValue("Loading...")]
	public string LoadingText
	{
		get; set;
	} = "Loading...";


	/// <summary>Gets the control with an active auto completion session.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TextBoxBase? ActiveControl
	{
		get { return _ActiveBinding?.Control; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if auto completion is enabled for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if auto completion is enabled, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if auto completion is enabled for the control.")][DefaultValue(false)]
	public virtual bool GetAutoCompletionEnabled(Control control)
	{
		return _TryGetBinding(control)?.Enabled ?? false;
	}


	/// <summary>Sets if auto completion is enabled for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetAutoCompletionEnabled(Control control, bool value)
	{
		_Bind(control).Enabled = value;
	}


	/// <summary>Gets if auto completion starts automatically when typing in a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if auto completion starts automatically, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if auto completion starts automatically when typing.")][DefaultValue(true)]
	public virtual bool GetAutoStart(Control control)
	{
		return _TryGetBinding(control)?.AutoStart ?? true;
	}


	/// <summary>Sets if auto completion starts automatically when typing in a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetAutoStart(Control control, bool value)
	{
		_Bind(control).AutoStart = value;
	}


	/// <summary>Gets the shortcut keys that start auto completion for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the shortcut keys.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Shortcut keys that start auto completion. None disables the shortcut.")][DefaultValue(Keys.Control | Keys.Space)]
	public virtual Keys GetShortcutKeys(Control control)
	{
		return _TryGetBinding(control)?.ShortcutKeys ?? (Keys.Control | Keys.Space);
	}


	/// <summary>Sets the shortcut keys that start auto completion for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetShortcutKeys(Control control, Keys value)
	{
		_Bind(control).ShortcutKeys = value;
	}


	/// <summary>Gets the match mode for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the match mode, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Match mode for the control. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual ListMatchMode? GetMatchMode(Control control)
	{
		return _TryGetBinding(control)?.MatchMode;
	}


	/// <summary>Sets the match mode for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetMatchMode(Control control, ListMatchMode? value)
	{
		_Bind(control).MatchMode = value;
	}


	/// <summary>Gets the filter mode for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the filter mode, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Filter mode for the control. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual ListFilterMode? GetFilterMode(Control control)
	{
		return _TryGetBinding(control)?.FilterMode;
	}


	/// <summary>Sets the filter mode for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetFilterMode(Control control, ListFilterMode? value)
	{
		_Bind(control).FilterMode = value;
	}


	/// <summary>Gets if matching is case-sensitive for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the case sensitivity, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if matching is case-sensitive for the control. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual bool? GetCaseSensitive(Control control)
	{
		return _TryGetBinding(control)?.CaseSensitive;
	}


	/// <summary>Sets if matching is case-sensitive for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetCaseSensitive(Control control, bool? value)
	{
		_Bind(control).CaseSensitive = value;
	}


	/// <summary>Gets the word characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the word characters, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Word characters for the control. NULL uses the provider setting.")][DefaultValue(null)]
	public virtual string? GetWordCharacters(Control control)
	{
		return _TryGetBinding(control)?.WordCharacters;
	}


	/// <summary>Sets the word characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetWordCharacters(Control control, string? value)
	{
		_Bind(control).WordCharacters = value;
	}


	/// <summary>Gets the trigger characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the trigger characters, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Trigger characters for the control. NULL uses the provider setting.")][DefaultValue(null)]
	public virtual string? GetTriggerCharacters(Control control)
	{
		return _TryGetBinding(control)?.TriggerCharacters;
	}


	/// <summary>Sets the trigger characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetTriggerCharacters(Control control, string? value)
	{
		_Bind(control).TriggerCharacters = value;
	}


	/// <summary>Gets the commit characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the commit characters, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Commit characters for the control. NULL uses the provider setting.")][DefaultValue(null)]
	public virtual string? GetCommitCharacters(Control control)
	{
		return _TryGetBinding(control)?.CommitCharacters;
	}


	/// <summary>Sets the commit characters for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetCommitCharacters(Control control, string? value)
	{
		_Bind(control).CommitCharacters = value;
	}


	/// <summary>Gets the auto completion window size for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the window size, or <see cref="Size.Empty"/> if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Auto completion window size for the control. 0, 0 uses the provider setting.")][DefaultValue(typeof(Size), "0, 0")]
	public virtual Size GetBoxSize(Control control)
	{
		return _TryGetBinding(control)?.BoxSize ?? Size.Empty;
	}


	/// <summary>Sets the auto completion window size for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. <see cref="Size.Empty"/> uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetBoxSize(Control control, Size value)
	{
		_Bind(control).BoxSize = value;
	}


	/// <summary>Gets if the auto completion window adjusts its width to the items for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the setting, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if the auto completion window adjusts its width to the items. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual bool? GetBoxAutoSize(Control control)
	{
		return _TryGetBinding(control)?.BoxAutoSize;
	}


	/// <summary>Sets if the auto completion window adjusts its width to the items for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetBoxAutoSize(Control control, bool? value)
	{
		_Bind(control).BoxAutoSize = value;
	}


	/// <summary>Gets if the auto completion window can be resized by the user for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the setting, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if the auto completion window can be resized by the user. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual bool? GetBoxResizable(Control control)
	{
		return _TryGetBinding(control)?.BoxResizable;
	}


	/// <summary>Sets if the auto completion window can be resized by the user for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetBoxResizable(Control control, bool? value)
	{
		_Bind(control).BoxResizable = value;
	}


	/// <summary>Gets if frequently accepted items are ranked higher for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the setting, or NULL if the provider setting is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	[Category("Auto Completion")][Description("Specifies if frequently accepted items are ranked higher. Empty uses the provider setting.")][DefaultValue(null)]
	public virtual bool? GetUsageRanking(Control control)
	{
		return _TryGetBinding(control)?.UsageRanking;
	}


	/// <summary>Sets if frequently accepted items are ranked higher for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider setting.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetUsageRanking(Control control, bool? value)
	{
		_Bind(control).UsageRanking = value;
	}


	/// <summary>Suspends the tracking of text and selection changes of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns an object that resumes tracking when it is disposed.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	/// <remarks>Use this method for programmatic changes that should not end an auto completion session, for example syntax
	///          highlighting or automatic formatting: <c>using(provider.SuspendTracking(textBox)) { ... }</c>. Calls can be nested.
	///          When tracking is resumed, the session continues if the caret is still in the completed word, otherwise it ends.
	///          Changes that only affect formatting (the text stays the same) are ignored automatically, even without suspending.</remarks>
	public virtual IDisposable SuspendTracking(Control control)
	{
		_Binding binding = _Bind(control);

		binding.SuspendTracking();
		return new _TrackingSuspension(binding);
	}


	/// <summary>Gets the auto completion source for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the source, or NULL if the provider source is used.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual IAutoCompletionSource? GetSource(Control control)
	{
		return _TryGetBinding(control)?.Source;
	}


	/// <summary>Sets the auto completion source for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value. NULL uses the provider source.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	public virtual void SetSource(Control control, IAutoCompletionSource? value)
	{
		_Bind(control).Source = value;
	}


	/// <summary>Starts auto completion for a control.</summary>
	/// <param name="control">Control.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the control is not a <see cref="TextBoxBase"/>.</exception>
	/// <exception cref="InvalidOperationException">Thrown when auto completion is not enabled for the control.</exception>
	public virtual void ShowCompletion(Control control)
	{
		_Binding binding = _Bind(control);

		if(!binding.Enabled) { throw new InvalidOperationException("Auto completion is not enabled for this control."); }
		binding.Start(AutoCompletionTrigger.MANUAL);
	}


	/// <summary>Closes the active auto completion session.</summary>
	public virtual void CloseCompletion()
	{
		_ActiveBinding?.Close(AutoCompletionCloseReason.MANUAL);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Disposed event of the image list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ImageList_Disposed(object? sender, EventArgs e)
	{
		ImageList = null;
	}


	/// <summary>Handles the ItemClicked event of the popup window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_ItemClicked(object? sender, EventArgs e)
	{
		_ActiveBinding?.MarkNavigated();
	}


	/// <summary>Handles the SelectionChanged event of the popup window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_SelectionChanged(object? sender, EventArgs e)
	{
		_ActiveBinding?.UpdateSelection();
	}


	/// <summary>Handles the UserResized event of the popup window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_UserResized(object? sender, EventArgs e)
	{
		_ActiveBinding?.StoreUserSize();
	}


	/// <summary>Handles the ItemActivated event of the popup window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Popup_ItemActivated(object? sender, EventArgs e)
	{
		_ActiveBinding?.AcceptSelected();
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

			if(_Popup is not null)
			{
				_Popup.ItemClicked -= _Popup_ItemClicked;
				_Popup.ItemActivated -= _Popup_ItemActivated;
				_Popup.SelectionChanged -= _Popup_SelectionChanged;
				_Popup.UserResized -= _Popup_UserResized;
				_Popup.Dispose();
				_Popup = null;
			}

			_Description?.Dispose();
			_Description = null;

			if(_ImageList is not null) { _ImageList.Disposed -= _ImageList_Disposed; }
			_ImageList = null;
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
