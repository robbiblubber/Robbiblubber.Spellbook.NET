namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.Win32;



/// <summary>This class manages the theme of the application and the custom themes.</summary>
/// <remarks><para>All color tables that do not have their own <see cref="ControlColorTable.Theme"/> use <see cref="Theme"/>. Changing
///          the theme takes effect immediately; controls repaint themselves.</para>
///          <para>Custom themes are registered with <see cref="Add(Theme)"/>. <see cref="Load(IThemeStore?)"/> and
///          <see cref="Save(IThemeStore?)"/> read and write the custom themes and the selected theme, by default with a
///          <see cref="DdnThemeStore"/>:</para>
///          <code>ThemeManager.Load();
///ThemeManager.Theme = Theme.DARK;
///ThemeManager.Save();</code>
///          <para><see cref="Apply(Control, Theme?)"/> applies the theme to standard controls, for example to a form and all its child
///          controls, with the theme adapters (<see cref="IThemeAdapter"/>); <see cref="ThemeProvider"/> does this from the
///          designer.</para>
///          <para>The theme manager and the themes are used on the user interface thread; changes of the Windows settings are passed to
///          that thread.</para></remarks>
[SupportedOSPlatform("windows")]
public static class ThemeManager
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme of the application.</summary>
	private static Theme _Theme = Theme.AUTO;

	/// <summary>Custom themes.</summary>
	private static readonly List<Theme> _CustomThemes = new();

	/// <summary>Built-in themes.</summary>
	private static readonly Theme[] _BuiltInThemes = new[] { Theme.SYSTEM, Theme.AUTO, Theme.LIGHT, Theme.DARK };

	/// <summary>Color tables, notified when their theme changes.</summary>
	private static readonly _WeakList<ControlColorTable> _Tables = new();

	/// <summary>Synchronization context of the user interface thread, NULL if it is not known yet.</summary>
	private static SynchronizationContext? _Context = null;

	/// <summary>Determines if the handler for Windows setting changes has been installed.</summary>
	private static bool _SystemEventsHooked = false;

	/// <summary>Registered theme adapters by control type, NULL if the built-in adapters have not been registered yet.</summary>
	private static Dictionary<Type, IThemeAdapter?>? _Adapters = null;

	/// <summary>Theme adapters by control type, including derived types.</summary>
	private static readonly Dictionary<Type, IThemeAdapter?> _AdapterCache = new();

	/// <summary>States of themed controls.</summary>
	private static readonly ConditionalWeakTable<Control, ThemeAdapterState> _States = new();

	/// <summary>Themes of the roots, NULL for the inherited theme.</summary>
	private static readonly ConditionalWeakTable<Control, StrongBox<Theme?>> _RootThemes = new();

	/// <summary>Controls that the theme has been applied to with <see cref="Apply(Control, Theme?)"/>.</summary>
	private static readonly _WeakList<Control> _Roots = new();

	/// <summary>Determines if the theme manager sets the color mode of the application.</summary>
	private static bool _SetsColorMode = false;

	/// <summary>Determines if text boxes draw their selection in the selection colors of the theme.</summary>
	private static bool _ThemeTextSelection = true;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Installs the handler for Windows setting changes, remembers the user interface thread and sets the initial color mode
	///          of the application.</summary>
	private static void _HookSystemEvents()
	{
		if((_Context is null) && (SynchronizationContext.Current is WindowsFormsSynchronizationContext context)) { _Context = context; }

		if(_SystemEventsHooked) { return; }
		_SystemEventsHooked = true;
		_UpdateColorMode();

		try
		{
			SystemEvents.UserPreferenceChanged += _SystemEvents_UserPreferenceChanged;
		}
		catch(Exception) {}
	}


	/// <summary>Raises the notifications for a theme change.</summary>
	/// <param name="theme">Theme whose colors have changed, NULL if the theme of the application has been replaced.</param>
	private static void _Notify(Theme? theme)
	{
		if((theme is null) || (theme == _Theme)) { _UpdateColorMode(); }

		foreach(Control i in _Roots._ToList())
		{
			if(i.IsDisposed) { _Roots._Remove(i); continue; }

			Theme rootTheme = GetTheme(i);
			if((theme is null) || (rootTheme == theme)) { _ApplyTree(i, rootTheme); }
		}

		foreach(ControlColorTable i in _Tables._ToList())
		{
			if((theme is null) ? ((i.Theme is null) && (i._AmbientTheme is null)) : (i.EffectiveTheme == theme)) { i._OnThemeChanged(); }
		}

		if((theme is null) || (theme == _Theme)) { ThemeChanged?.Invoke(null, EventArgs.Empty); }
	}


	/// <summary>Sets the color mode of the application according to its theme.</summary>
	/// <remarks>Only available for .NET 9 and later; the system theme uses the Windows setting, other themes the dark or the classic
	///          mode.</remarks>
	private static void _UpdateColorMode()
	{
#if NET9_0_OR_GREATER
		if(!_SetsColorMode) { return; }

		SystemColorMode mode = (_Theme.IsSystem ? SystemColorMode.System : (_Theme.IsDark ? SystemColorMode.Dark : SystemColorMode.Classic));
		if(Application.ColorMode == mode) { return; }

		try
		{
			Application.SetColorMode(mode);
		}
		catch(Exception) {}
#endif
	}


	/// <summary>Registers the built-in theme adapters if necessary.</summary>
	/// <returns>Returns the registered theme adapters.</returns>
	private static Dictionary<Type, IThemeAdapter?> _GetAdapters()
	{
		if(_Adapters is null)
		{
			_Adapters = new();
			Discover(typeof(ThemeManager).Assembly);
		}

		return _Adapters;
	}


	/// <summary>Checks if an adapter can theme a control type.</summary>
	/// <param name="controlType">Control type.</param>
	/// <param name="adapter">Theme adapter.</param>
	/// <exception cref="ArgumentException">Thrown when the adapter is derived from <see cref="ThemeAdapter{T}"/> for a control type
	///                                      that the control type is not derived from.</exception>
	private static void _CheckAdapter(Type controlType, IThemeAdapter adapter)
	{
		for(Type? i = adapter.GetType(); i is not null; i = i.BaseType)
		{
			if(!i.IsGenericType || (i.GetGenericTypeDefinition() != typeof(ThemeAdapter<>))) { continue; }

			Type supported = i.GetGenericArguments()[0];
			if(!supported.IsAssignableFrom(controlType))
			{
				throw new ArgumentException($"The adapter {adapter.GetType().Name} supports {supported.Name} and cannot theme {controlType.Name}.");
			}
			return;
		}
	}


	/// <summary>Applies a theme to a control and its child controls.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Theme.</param>
	private static void _ApplyTree(Control control, Theme theme)
	{
		if(control.IsDisposed) { return; }
		if(_RootThemes.TryGetValue(control, out StrongBox<Theme?>? root) && (root.Value is not null)) { theme = root.Value; }

		IThemeAdapter? adapter = GetAdapter(control.GetType());
		_ApplyControl(control, theme, adapter);

		if((adapter?.IncludesChildren(control) ?? true) && ((control as IThemedControl)?.ThemesChildren ?? true))
		{
			foreach(Control i in control.Controls.Cast<Control>().ToArray()) { _ApplyTree(i, theme); }
		}
		if(control.ContextMenuStrip is ContextMenuStrip menu) { _ApplyTree(menu, theme); }
	}


	/// <summary>Applies a theme to a single control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="adapter">Theme adapter of the control, NULL if there is none.</param>
	private static void _ApplyControl(Control control, Theme theme, IThemeAdapter? adapter)
	{
		ThemeAdapterState state = _States.GetValue(control, m => new ThemeAdapterState(m));
		_HookControl(control, state);
		state._Resolved = theme;

		if(control is IThemedControl themed) { themed.ThemeColors._AmbientTheme = theme; }

		if(theme.IsSystem)
		{
			state._Restore();
			return;
		}

		state.Theme = theme;
		adapter?.Apply(control, theme, state);
	}


	/// <summary>Installs the event handlers of the theme manager for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="state">State.</param>
	private static void _HookControl(Control control, ThemeAdapterState state)
	{
		if(state._ManagerUnsubscriber is not null) { return; }

		void handleCreated(object? sender, EventArgs e)
		{
			if(state.Theme is Theme theme) { GetAdapter(control.GetType())?.Apply(control, theme, state); }
		}

		void controlAdded(object? sender, ControlEventArgs e)
		{
			if((state._Resolved is not Theme theme) || (e.Control is null)) { return; }
			if(!((control as IThemedControl)?.ThemesChildren ?? true) || !(GetAdapter(control.GetType())?.IncludesChildren(control) ?? true)) { return; }

			_ApplyTree(e.Control, theme);
		}

		control.HandleCreated += handleCreated;
		control.ControlAdded += controlAdded;
		state._ManagerUnsubscriber = () =>
		{
			control.HandleCreated -= handleCreated;
			control.ControlAdded -= controlAdded;
		};
	}


	/// <summary>Restores a control and its child controls.</summary>
	/// <param name="control">Control.</param>
	private static void _ResetTree(Control control)
	{
		if(_States.TryGetValue(control, out ThemeAdapterState? state))
		{
			state._Restore();
			state._ManagerUnsubscriber?.Invoke();
			state._ManagerUnsubscriber = null;
			state._Resolved = null;
			_States.Remove(control);
		}
		if(control is IThemedControl themed) { themed.ThemeColors._AmbientTheme = null; }

		foreach(Control i in control.Controls.Cast<Control>().ToArray()) { _ResetTree(i); }
		if(control.ContextMenuStrip is ContextMenuStrip menu) { _ResetTree(menu); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registers a color table for theme notifications.</summary>
	/// <param name="table">Color table.</param>
	internal static void _Register(ControlColorTable table)
	{
		_Tables._Add(table);
		_HookSystemEvents();
	}


	/// <summary>Notifies the application and the color tables that the colors of a theme have changed.</summary>
	/// <param name="theme">Theme.</param>
	internal static void _OnThemeChanged(Theme theme)
	{
		_Notify(theme);
	}


	/// <summary>Notifies the application that a theme has been renamed.</summary>
	/// <param name="theme">Theme.</param>
	internal static void _OnThemeRenamed(Theme theme)
	{
		if(_CustomThemes.Contains(theme)) { ThemesChanged?.Invoke(null, EventArgs.Empty); }
	}


	/// <summary>Processes a change of the Windows settings.</summary>
	internal static void _OnSystemSettingsChanged()
	{
		bool autoChanged = Theme._RefreshAuto();

		Theme.SYSTEM._OnChanged();
		if(autoChanged || Theme.AUTO.DependsOn(Theme.SYSTEM)) { Theme.AUTO._OnChanged(); }
	}


	/// <summary>Gets the number of registered color tables, including collected ones.</summary>
	/// <returns>Returns the number of entries.</returns>
	internal static int _GetTableCountForTest()
	{
		return _Tables._Count;
	}


	/// <summary>Removes all custom themes and selects the default theme; the color mode of the application is not changed.</summary>
	internal static void _ResetForTest()
	{
		_SetsColorMode = false;
		_ThemeTextSelection = true;
		_Adapters = null;
		_AdapterCache.Clear();
		_CustomThemes.Clear();
		Store = null;
		Theme = Theme.AUTO;
	}


	/// <summary>Gets the state of a themed control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the state, or NULL if no theme has been applied to the control.</returns>
	internal static ThemeAdapterState? _GetStateForTest(Control control)
	{
		return (_States.TryGetValue(control, out ThemeAdapterState? rval) ? rval : null);
	}


	/// <summary>Raises the <see cref="ResolveImage"/> event.</summary>
	/// <param name="source">Object that shows the image.</param>
	/// <param name="image">Image.</param>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns the image to show: for dark themes, single-colored dark images and dark outlines are lightened, unless an
	///          event handler replaces the image. A composed image (for example a pinned recent entry) is composed again of its
	///          resolved parts.</returns>
	internal static Image _ResolveImage(object? source, Image image, Theme theme)
	{
		if(_ThemeImages._TryResolveOverlay(source, image, theme, out Image? composed)) { return composed; }

		Image themed = (theme.IsDark ? _ThemeImages._GetLightImage(image) : image);
		if(ResolveImage is null) { return themed; }

		ThemeImageEventArgs e = new(source, image, theme, themed);
		ResolveImage(null, e);
		return (e.Image ?? image);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static events                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the theme of the application has been replaced or its colors have changed.</summary>
	public static event EventHandler? ThemeChanged;

	/// <summary>Occurs when a custom theme has been added, removed or renamed.</summary>
	public static event EventHandler? ThemesChanged;

	/// <summary>Occurs when an image of a themed menu or tool bar is drawn.</summary>
	/// <remarks>The event allows to replace an image for a theme, for example with a light variant for dark themes. By default,
	///          single-colored dark images are lightened in dark themes, and so are the dark outlines (dark gray or black pixels) of
	///          colored images; their colors are kept.</remarks>
	public static event EventHandler<ThemeImageEventArgs>? ResolveImage;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the theme of the application.</summary>
	/// <remarks>The default is <see cref="Theme.AUTO"/>, which follows the light or dark app mode of Windows.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	public static Theme Theme
	{
		get { return _Theme; }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			if(value == _Theme) { return; }

			_Theme = value;
			_HookSystemEvents();
			_Notify(null);
		}
	}


	/// <summary>Gets the built-in themes.</summary>
	public static IReadOnlyList<Theme> BuiltInThemes
	{
		get { return _BuiltInThemes; }
	}


	/// <summary>Gets the registered custom themes.</summary>
	public static IReadOnlyList<Theme> CustomThemes
	{
		get { return _CustomThemes.ToArray(); }
	}


	/// <summary>Gets all themes: the built-in themes followed by the custom themes.</summary>
	public static IReadOnlyList<Theme> Themes
	{
		get { return _BuiltInThemes.Concat(_CustomThemes).ToArray(); }
	}


	/// <summary>Gets or sets if the theme manager sets the color mode of the application.</summary>
	/// <remarks><para>With .NET 9 and later, the theme manager then calls <c>Application.SetColorMode()</c>: dark themes
	///          use the dark mode, light themes the classic mode and the system theme the Windows setting. The color mode affects the
	///          system colors and the controls that Windows Forms draws itself in dark mode. With .NET 8, this property has no
	///          effect.</para>
	///          <para>Windows Forms does not support changing the color mode while windows are open: controls keep parts of the old
	///          mode, and button texts can become unreadable. Only enable this property if the application sets or loads its theme
	///          before <see cref="Application.Run(Form)"/> and does not switch between dark and light themes afterwards. The default
	///          is <c>false</c>.</para></remarks>
	public static bool SetsColorMode
	{
		get { return _SetsColorMode; }
		set
		{
			if(value == _SetsColorMode) { return; }

			_SetsColorMode = value;
			_UpdateColorMode();
		}
	}


	/// <summary>Gets or sets if text boxes draw their selection in the selection colors of the theme.</summary>
	/// <remarks><para>Text boxes, rich text boxes and the text of editable combo boxes take their selection colors from the system
	///          colors, which cannot be set per control. With this property, the theme manager replaces the functions <c>GetSysColor</c>
	///          and <c>GetSysColorBrush</c> in the import tables of the libraries that implement these controls in this process
	///          (comctl32, msftedit, riched20); the replacements return the selection colors of the theme while a themed text box draws,
	///          and the system colors otherwise. The edit control of text boxes, combo boxes and up-down controls gets its selection
	///          colors internally, so its selected text is drawn again in the theme colors after the control has drawn it; while the
	///          mouse selects text, the system color may show for a moment. A text box without the focus gets the inactive selection
	///          colors.</para>
	///          <para>This changes code that does not belong to the application, so it may stop working with a future version of Windows,
	///          and security software may report it. Once enabled, the import tables stay changed until the process ends; switching the
	///          property off only makes the replacements pass the calls on. Without visual styles the property has no effect. The
	///          default is <c>true</c>.</para></remarks>
	public static bool ThemeTextSelection
	{
		get { return _ThemeTextSelection; }
		set
		{
			if(value == _ThemeTextSelection) { return; }

			_ThemeTextSelection = value;
			_Notify(null);
		}
	}


	/// <summary>Gets or sets the store that <see cref="Save(IThemeStore?)"/> uses when no store is passed.</summary>
	/// <remarks><see cref="Load(IThemeStore?)"/> sets the store it has used.</remarks>
	public static IThemeStore? Store
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds a theme by its identifier.</summary>
	/// <param name="id">Identifier.</param>
	/// <returns>Returns the built-in or registered theme, or NULL if no theme has this identifier.</returns>
	public static Theme? Find(string id)
	{
		return (Theme._GetBuiltIn(id) ?? _CustomThemes.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase)));
	}


	/// <summary>Registers a custom theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the theme is a built-in theme, when another theme has the same identifier or
	///                                      when its base theme is not registered.</exception>
	public static void Add(Theme theme)
	{
		ArgumentNullException.ThrowIfNull(theme);
		if(theme.IsBuiltIn) { throw new ArgumentException("Built-in themes cannot be added.", nameof(theme)); }
		if(_CustomThemes.Contains(theme)) { return; }
		if(Find(theme.Id) is not null) { throw new ArgumentException("A theme with this identifier is already registered.", nameof(theme)); }
		if(Find(theme.Base!.Id) != theme.Base) { throw new ArgumentException("The base theme must be registered.", nameof(theme)); }

		_CustomThemes.Add(theme);
		ThemesChanged?.Invoke(null, EventArgs.Empty);
	}


	/// <summary>Removes a custom theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns TRUE if the theme has been removed, FALSE if it was not registered.</returns>
	/// <remarks>Themes based on the removed theme are based on its base theme afterwards. If the removed theme is the theme of the
	///          application, its base theme becomes the theme of the application.</remarks>
	public static bool Remove(Theme theme)
	{
		ArgumentNullException.ThrowIfNull(theme);
		if(!_CustomThemes.Remove(theme)) { return false; }

		foreach(Theme i in _CustomThemes)
		{
			if(i.Base == theme) { i.Base = theme.Base; }
		}
		if(_Theme.DependsOn(theme))
		{
			Theme next = theme.Base!;
			while(!next.IsBuiltIn && !_CustomThemes.Contains(next)) { next = next.Base!; }
			Theme = ((_Theme == theme) ? next : _Theme);
		}

		ThemesChanged?.Invoke(null, EventArgs.Empty);
		return true;
	}


	/// <summary>Loads the custom themes and the selected theme.</summary>
	/// <param name="store">Store, NULL for a <see cref="DdnThemeStore"/> with the default file.</param>
	/// <remarks>The loaded themes replace the registered custom themes. If the selected theme does not exist, the theme of the
	///          application stays, or, if it is a custom theme that has not been loaded, its next base theme that exists becomes the theme
	///          of the application.</remarks>
	public static void Load(IThemeStore? store = null)
	{
		store ??= new DdnThemeStore();
		ThemeSettings settings = store.Load();

		_CustomThemes.Clear();
		foreach(Theme i in settings.Themes)
		{
			if(!i.IsBuiltIn && (Find(i.Id) is null)) { _CustomThemes.Add(i); }
		}
		Store = store;
		ThemesChanged?.Invoke(null, EventArgs.Empty);

		Theme? selected = (string.IsNullOrWhiteSpace(settings.SelectedTheme) ? null : Find(settings.SelectedTheme));
		for(Theme? i = _Theme; (selected is null) && (i is not null); i = i.Base) { selected = Find(i.Id); }
		if(selected is not null) { Theme = selected; }
	}


	/// <summary>Takes over the changes that other applications have made to the stored themes.</summary>
	/// <remarks><para>Only stores that share their themes with other applications support this, like the <see cref="SharedThemeStore"/>;
	///          for other stores and without <see cref="Store"/>, nothing happens. Added themes are registered, changed themes are
	///          updated in place and removed themes are removed. Themes that have been changed or removed in this application as well keep
	///          the state of this application.</para>
	///          <para>The <see cref="ThemeEditorDialog"/> calls this method when it is opened.</para></remarks>
	public static void Refresh()
	{
		if(Store is not SharedThemeStore store) { return; }

		store._Refresh(_CustomThemes, out List<Theme> changed, out List<Theme> removed);
		if((changed.Count == 0) && (removed.Count == 0)) { return; }

		List<Theme> updated = new();
		foreach(Theme i in changed)
		{
			Theme? existing = Find(i.Id);
			if(existing is null) { _CustomThemes.Add(i); }
			else if(existing._Assign(i)) { updated.Add(existing); }
		}
		foreach(Theme i in _CustomThemes)
		{
			if(i.Base!.IsBuiltIn || (Find(i.Base.Id) == i.Base)) { continue; }

			Theme baseTheme = Find(i.Base.Id) ?? Theme.AUTO;
			i.Base = (baseTheme.DependsOn(i) ? Theme.AUTO : baseTheme);
		}

		foreach(Theme i in removed) { Remove(i); }
		foreach(Theme i in updated) { i._OnChanged(); }

		ThemesChanged?.Invoke(null, EventArgs.Empty);
	}


	/// <summary>Saves the custom themes and the selected theme.</summary>
	/// <param name="store">Store, NULL for <see cref="Store"/> or, if it is not set, a <see cref="DdnThemeStore"/> with the default
	///                     file.</param>
	public static void Save(IThemeStore? store = null)
	{
		store ??= (Store ??= new DdnThemeStore());
		store.Save(new ThemeSettings() { SelectedTheme = _Theme.Id, Themes = _CustomThemes.ToArray() });
	}


	/// <summary>Applies a theme to a control and its child controls.</summary>
	/// <param name="control">Control, usually a form.</param>
	/// <param name="theme">Theme, NULL for the inherited theme: the theme of the next parent control that has a theme of its own, or
	///                     the theme of the application.</param>
	/// <remarks><para>The control keeps its theme: it follows changes of the theme (and, without a theme of its own, the theme of the
	///          application); controls that are added later are themed as well. The control is referenced weakly.</para>
	///          <para>Colors that have been set explicitly are kept, unless they are system colors. For themes that use the system colors
	///          (<see cref="Theme.IsSystem"/>), the controls are restored to their original state.</para></remarks>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	public static void Apply(Control control, Theme? theme = null)
	{
		ArgumentNullException.ThrowIfNull(control);
		_HookSystemEvents();

		if(_RootThemes.TryGetValue(control, out StrongBox<Theme?>? root))
		{
			root.Value = theme;
		}
		else
		{
			_RootThemes.Add(control, new StrongBox<Theme?>(theme));
			_Roots._Add(control);
		}

		_ApplyTree(control, GetTheme(control));
	}


	/// <summary>Restores a control and its child controls to their original state.</summary>
	/// <param name="control">Control.</param>
	/// <remarks>The control does not follow theme changes any more.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	public static void Reset(Control control)
	{
		ArgumentNullException.ThrowIfNull(control);

		if(_RootThemes.Remove(control)) { _Roots._Remove(control); }
		_ResetTree(control);
	}


	/// <summary>Gets the theme that applies to a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the theme of the next control (the control itself or a parent) that has a theme of its own, otherwise the theme
	///          of the application.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	public static Theme GetTheme(Control control)
	{
		ArgumentNullException.ThrowIfNull(control);

		for(Control? i = control; i is not null; i = i.Parent)
		{
			if(_RootThemes.TryGetValue(i, out StrongBox<Theme?>? root) && (root.Value is Theme rval)) { return rval; }
		}

		return _Theme;
	}


	/// <summary>Registers a theme adapter for a control type.</summary>
	/// <param name="controlType">Control type.</param>
	/// <param name="adapter">Theme adapter, NULL to theme controls of this type without adapter (only their child controls).</param>
	/// <remarks>The adapter replaces a registered adapter for the same type, including a built-in adapter. Controls of derived types
	///          use the adapter unless their type has an adapter of its own.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the control type is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the type is not derived from <see cref="Control"/>, or when the adapter is
	///                                      derived from <see cref="ThemeAdapter{T}"/> for a type that is not a base of the control
	///                                      type.</exception>
	public static void RegisterAdapter(Type controlType, IThemeAdapter? adapter)
	{
		ArgumentNullException.ThrowIfNull(controlType);
		if(!typeof(Control).IsAssignableFrom(controlType)) { throw new ArgumentException("The type must be derived from Control.", nameof(controlType)); }
		if(adapter is not null) { _CheckAdapter(controlType, adapter); }

		_GetAdapters()[controlType] = adapter;
		_AdapterCache.Clear();
	}


	/// <summary>Registers all theme adapters of an assembly.</summary>
	/// <param name="assembly">Assembly.</param>
	/// <remarks>All classes with <see cref="ThemeAdapterAttribute"/> are registered; see <see cref="Discover(IEnumerable{Type})"/>.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the assembly is NULL.</exception>
	public static void Discover(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);
		Discover(assembly.GetTypes());
	}


	/// <summary>Registers the theme adapters among a set of types.</summary>
	/// <param name="types">Types.</param>
	/// <remarks>Types that implement <see cref="IThemeAdapter"/>, are not abstract, have a public parameterless constructor and have a
	///          <see cref="ThemeAdapterAttribute"/> are registered for the control types of their attributes. Other types are
	///          ignored.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the types are NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when an adapter derived from <see cref="ThemeAdapter{T}"/> has an attribute for a
	///                                      control type that is not derived from its type argument.</exception>
	public static void Discover(IEnumerable<Type> types)
	{
		ArgumentNullException.ThrowIfNull(types);
		Dictionary<Type, IThemeAdapter?> adapters = _GetAdapters();

		foreach(Type i in types)
		{
			if(i.IsAbstract || i.IsInterface || !typeof(IThemeAdapter).IsAssignableFrom(i) || (i.GetConstructor(Type.EmptyTypes) is null)) { continue; }

			ThemeAdapterAttribute[] attributes = i.GetCustomAttributes<ThemeAdapterAttribute>(false).ToArray();
			if(attributes.Length == 0) { continue; }

			IThemeAdapter adapter = (IThemeAdapter) Activator.CreateInstance(i)!;
			foreach(ThemeAdapterAttribute j in attributes)
			{
				if(!typeof(Control).IsAssignableFrom(j.ControlType)) { continue; }

				_CheckAdapter(j.ControlType, adapter);
				adapters[j.ControlType] = adapter;
			}
		}
		_AdapterCache.Clear();
	}


	/// <summary>Gets the theme adapter for a control type.</summary>
	/// <param name="controlType">Control type.</param>
	/// <returns>Returns the adapter registered for the type or its nearest base type, or NULL if there is none.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control type is NULL.</exception>
	public static IThemeAdapter? GetAdapter(Type controlType)
	{
		ArgumentNullException.ThrowIfNull(controlType);
		if(_AdapterCache.TryGetValue(controlType, out IThemeAdapter? rval)) { return rval; }

		Dictionary<Type, IThemeAdapter?> adapters = _GetAdapters();
		for(Type? i = controlType; i is not null; i = i.BaseType)
		{
			if(adapters.TryGetValue(i, out rval)) { break; }
		}

		_AdapterCache[controlType] = rval;
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the UserPreferenceChanged event of the system events.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private static void _SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		if((e.Category != UserPreferenceCategory.Color) && (e.Category != UserPreferenceCategory.General) &&
		   (e.Category != UserPreferenceCategory.Accessibility) && (e.Category != UserPreferenceCategory.VisualStyle)) { return; }

		if((_Context is not null) && (SynchronizationContext.Current != _Context))
		{
			_Context.Post(_ => _OnSystemSettingsChanged(), null);
			return;
		}

		Form? form = ((Application.OpenForms.Count > 0) ? Application.OpenForms[0] : null);
		if((form is not null) && form.InvokeRequired) { form.BeginInvoke(_OnSystemSettingsChanged); } else { _OnSystemSettingsChanged(); }
	}
}
