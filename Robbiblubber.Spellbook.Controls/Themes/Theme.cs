namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.Globalization;
using System.Runtime.Versioning;
using Microsoft.Win32;



/// <summary>This class represents a color theme.</summary>
/// <remarks><para>A theme assigns a color to every <see cref="ThemeColor"/>. The built-in themes <see cref="SYSTEM"/>,
///          <see cref="AUTO"/>, <see cref="LIGHT"/> and <see cref="DARK"/> cannot be changed. A custom theme is based on another theme
///          and only sets the colors that differ; a color that is <see cref="Color.Empty"/> is taken from the base theme:</para>
///          <code>Theme green = new("Dark green", Theme.DARK);
///green.SetValue(ThemeColor.ACCENT, Color.LimeGreen);</code></remarks>
[SupportedOSPlatform("windows")]
[TypeConverter(typeof(_ThemeConverter))]
public sealed class Theme
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The system theme uses the system colors.</summary>
	/// <remarks>The system colors follow the high contrast themes and, from .NET 9, the color mode of the application
	///          (<c>Application.SetColorMode</c>).</remarks>
	public static readonly Theme SYSTEM = new(_ThemeKind.SYSTEM, "system", "System");

	/// <summary>The automatic theme follows the app mode of Windows.</summary>
	/// <remarks>The theme uses <see cref="DARK"/> when the Windows setting for apps is dark, <see cref="SYSTEM"/> while a high contrast
	///          theme is active and <see cref="LIGHT"/> otherwise. It changes when the Windows setting changes.</remarks>
	public static readonly Theme AUTO = new(_ThemeKind.AUTO, "auto", "Automatic");

	/// <summary>The light theme uses a fixed light palette.</summary>
	public static readonly Theme LIGHT = new(_ThemeKind.LIGHT, "light", "Light");

	/// <summary>The dark theme uses a fixed dark palette.</summary>
	public static readonly Theme DARK = new(_ThemeKind.DARK, "dark", "Dark");



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of colors.</summary>
	private static readonly int _ColorCount = Enum.GetValues<ThemeColor>().Length;

	/// <summary>Custom themes, notified when their base theme changes.</summary>
	private static readonly _WeakList<Theme> _CustomThemes = new();

	/// <summary>Theme that the automatic theme currently uses, NULL if it has not been determined.</summary>
	private static Theme? _AutoTarget = null;

	/// <summary>Theme that the automatic theme uses in tests, NULL for the Windows setting.</summary>
	private static Theme? _AutoTargetForTest = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Kind of theme.</summary>
	private readonly _ThemeKind _Kind;

	/// <summary>Theme name.</summary>
	private string _Name;

	/// <summary>Base theme, NULL for built-in themes.</summary>
	private Theme? _Base;

	/// <summary>Colors that are set explicitly, <see cref="Color.Empty"/> for colors of the base theme.</summary>
	private readonly Color[] _Values;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="kind">Kind of theme.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="name">Theme name.</param>
	private Theme(_ThemeKind kind, string id, string name)
	{
		_Kind = kind;
		Id = id;
		_Name = name;
		_Values = Array.Empty<Color>();
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="name">Theme name.</param>
	/// <param name="baseTheme">Base theme that provides all colors that are not set explicitly.</param>
	/// <exception cref="ArgumentNullException">Thrown when the name or the base theme is NULL.</exception>
	public Theme(string name, Theme baseTheme): this(Guid.NewGuid().ToString("N"), name, baseTheme)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="id">Identifier. The identifier is used to store the theme selection and to refer to base themes.</param>
	/// <param name="name">Theme name.</param>
	/// <param name="baseTheme">Base theme that provides all colors that are not set explicitly.</param>
	/// <exception cref="ArgumentNullException">Thrown when the name or the base theme is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the identifier is empty or the identifier of a built-in theme.</exception>
	public Theme(string id, string name, Theme baseTheme)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id);
		ArgumentNullException.ThrowIfNull(name);
		ArgumentNullException.ThrowIfNull(baseTheme);
		if(_GetBuiltIn(id) is not null) { throw new ArgumentException("The identifier is reserved for a built-in theme.", nameof(id)); }

		_Kind = _ThemeKind.CUSTOM;
		Id = id;
		_Name = name;
		_Base = baseTheme;
		_Values = new Color[_ColorCount];

		_CustomThemes._Add(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Determines the theme that the automatic theme uses.</summary>
	/// <returns>Returns the theme.</returns>
	private static Theme _DetermineAutoTarget()
	{
		if(_AutoTargetForTest is not null) { return _AutoTargetForTest; }
		if(SystemInformation.HighContrast) { return SYSTEM; }

		try
		{
			using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
			if((key?.GetValue("AppsUseLightTheme") is int light) && (light == 0)) { return DARK; }
		}
		catch(Exception) {}

		return LIGHT;
	}


	/// <summary>Gets the theme that the automatic theme uses.</summary>
	/// <returns>Returns the theme.</returns>
	private static Theme _GetAutoTarget()
	{
		return (_AutoTarget ??= _DetermineAutoTarget());
	}


	/// <summary>Gets the index of a color role.</summary>
	/// <param name="color">Color role.</param>
	/// <returns>Returns the index.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the color role is not defined.</exception>
	private static int _Index(ThemeColor color)
	{
		int rval = (int) color;
		if((rval < 0) || (rval >= _ColorCount)) { throw new ArgumentOutOfRangeException(nameof(color)); }

		return rval;
	}


	/// <summary>Converts a color to its text representation.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns <c>#RRGGBB</c> for opaque colors and <c>#AARRGGBB</c> otherwise.</returns>
	private static string _ColorToText(Color color)
	{
		return ((color.A == 255) ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");
	}


	/// <summary>Parses the text representation of a color.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the color, or <see cref="Color.Empty"/> if the text is not a valid color.</returns>
	private static Color _TextToColor(string? text)
	{
		if(string.IsNullOrWhiteSpace(text)) { return Color.Empty; }

		text = text.Trim().TrimStart('#');
		if(!uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value)) { return Color.Empty; }

		return text.Length switch
		{
			6 => Color.FromArgb(255, Color.FromArgb((int) value)),
			8 => Color.FromArgb((int) value),
			_ => Color.Empty
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the effective color of a role.</summary>
	/// <param name="index">Color index.</param>
	/// <returns>Returns the color.</returns>
	private Color _Get(int index)
	{
		return _Kind switch
		{
			_ThemeKind.SYSTEM => _ThemePalettes._System[index],
			_ThemeKind.LIGHT => _ThemePalettes._Light[index],
			_ThemeKind.DARK => _ThemePalettes._Dark[index],
			_ThemeKind.AUTO => _GetAutoTarget()._Get(index),
			_ => (_Values[index].IsEmpty ? _Base!._Get(index) : _Values[index])
		};
	}


	/// <summary>Throws an exception if the theme is a built-in theme.</summary>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	private void _ThrowIfBuiltIn()
	{
		if(_Kind != _ThemeKind.CUSTOM) { throw new InvalidOperationException("Built-in themes cannot be changed."); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a built-in theme by its identifier.</summary>
	/// <param name="id">Identifier.</param>
	/// <returns>Returns the theme, or NULL if the identifier does not belong to a built-in theme.</returns>
	internal static Theme? _GetBuiltIn(string? id)
	{
		foreach(Theme i in new[] { SYSTEM, AUTO, LIGHT, DARK })
		{
			if(string.Equals(i.Id, id, StringComparison.OrdinalIgnoreCase)) { return i; }
		}

		return null;
	}


	/// <summary>Determines the theme that the automatic theme uses again.</summary>
	/// <returns>Returns TRUE if the automatic theme now uses another theme, otherwise FALSE.</returns>
	internal static bool _RefreshAuto()
	{
		Theme? old = _AutoTarget;
		_AutoTarget = _DetermineAutoTarget();

		return (old != _AutoTarget);
	}


	/// <summary>Sets the theme that the automatic theme uses in tests.</summary>
	/// <param name="target">Theme, NULL for the Windows setting.</param>
	/// <remarks>The setting takes effect with the next <see cref="ThemeManager._OnSystemSettingsChanged"/>, like a change of the
	///          Windows setting.</remarks>
	internal static void _SetAutoTargetForTest(Theme? target)
	{
		_AutoTargetForTest = target;
	}


	/// <summary>Reads a theme from a ddn element.</summary>
	/// <param name="elm">Element.</param>
	/// <param name="baseId">Returns the identifier of the base theme.</param>
	/// <returns>Returns the theme, or NULL if the element does not contain a valid theme. The base theme is <see cref="AUTO"/> and has
	///          to be set by the caller.</returns>
	internal static Theme? _Read(DdnElement elm, out string? baseId)
	{
		string? id = elm["id"];
		string? name = elm["name"];
		baseId = elm["base"];

		if(string.IsNullOrWhiteSpace(id) || (_GetBuiltIn(id) is not null)) { return null; }

		Theme rval = new(id, (name ?? string.Empty), AUTO);
		foreach(DdnElement i in elm["colors"].Values)
		{
			if(!Enum.TryParse(i.Name, false, out ThemeColor color) || !Enum.IsDefined(color)) { continue; }
			rval._Values[(int) color] = _TextToColor(i.Value);
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="Changed"/> event for this theme and all themes based on it.</summary>
	internal void _OnChanged()
	{
		Changed?.Invoke(this, EventArgs.Empty);

		foreach(Theme i in _CustomThemes._ToList())
		{
			if(i._Base == this) { i._OnChanged(); }
		}
		ThemeManager._OnThemeChanged(this);
	}


	/// <summary>Takes over the name, the base theme and the colors of another theme without raising events.</summary>
	/// <param name="source">Theme.</param>
	/// <returns>Returns <c>true</c> if the theme has changed, otherwise returns <c>false</c>.</returns>
	/// <remarks>If this theme would become its own base, it keeps its base theme.</remarks>
	internal bool _Assign(Theme source)
	{
		Theme baseTheme = ((source._Base is null) || source._Base.DependsOn(this)) ? _Base! : source._Base;
		bool rval = ((_Name != source._Name) || (_Base != baseTheme) || !_Values.SequenceEqual(source._Values));

		_Name = source._Name;
		_Base = baseTheme;
		Array.Copy(source._Values, _Values, _Values.Length);

		return rval;
	}


	/// <summary>Writes the theme to a ddn element.</summary>
	/// <param name="elm">Element.</param>
	internal void _Write(DdnElement elm)
	{
		elm["id"] = Id;
		elm["name"] = _Name;
		elm["base"] = (_Base?.Id ?? AUTO.Id);

		for(int i = 0; i < _Values.Length; i++)
		{
			if(!_Values[i].IsEmpty) { elm["colors/" + ((ThemeColor) i).ToString()] = _ColorToText(_Values[i]); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the name, the base theme or a color of the theme has changed.</summary>
	/// <remarks>The event also occurs when the colors of the base theme change, and for <see cref="SYSTEM"/> and <see cref="AUTO"/>
	///          when the Windows settings change.</remarks>
	public event EventHandler? Changed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the identifier of the theme.</summary>
	public string Id
	{
		get;
	}


	/// <summary>Gets or sets the theme name.</summary>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	public string Name
	{
		get { return _Name; }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			_ThrowIfBuiltIn();
			if(value == _Name) { return; }

			_Name = value;
			_OnChanged();
			ThemeManager._OnThemeRenamed(this);
		}
	}


	/// <summary>Gets or sets the base theme that provides all colors that are not set explicitly.</summary>
	/// <remarks>Built-in themes do not have a base theme.</remarks>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	/// <exception cref="ArgumentException">Thrown when the theme would become its own base.</exception>
	public Theme? Base
	{
		get { return _Base; }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			_ThrowIfBuiltIn();
			if(value == _Base) { return; }
			if(value.DependsOn(this)) { throw new ArgumentException("A theme cannot be based on itself.", nameof(value)); }

			_Base = value;
			_OnChanged();
		}
	}


	/// <summary>Gets if the theme is a built-in theme.</summary>
	public bool IsBuiltIn
	{
		get { return (_Kind != _ThemeKind.CUSTOM); }
	}


	/// <summary>Gets if the theme uses the system colors.</summary>
	/// <remarks>This is the case for <see cref="SYSTEM"/> and for <see cref="AUTO"/> while it uses the system theme (high contrast).
	///          Theme adapters leave the controls in their original state for such a theme.</remarks>
	public bool IsSystem
	{
		get { return ((_Kind == _ThemeKind.SYSTEM) || ((_Kind == _ThemeKind.AUTO) && _GetAutoTarget().IsSystem)); }
	}


	/// <summary>Gets if the theme is dark.</summary>
	/// <remarks>A theme is dark when its <see cref="ThemeColor.BACK"/> color is dark. Dark themes use dark title bars, dark scroll bars
	///          and light icons.</remarks>
	public bool IsDark
	{
		get { return (_Get((int) ThemeColor.BACK).GetBrightness() < 0.5f); }
	}


	/// <summary>Gets the effective color of a role.</summary>
	/// <param name="color">Color role.</param>
	/// <returns>Returns the color that is set explicitly, otherwise the color of the base theme.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the color role is not defined.</exception>
	public Color this[ThemeColor color]
	{
		get { return _Get(_Index(color)); }
	}


	/// <summary>Gets if any color is set explicitly.</summary>
	public bool HasValues
	{
		get { return _Values.Any(m => !m.IsEmpty); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Loads a theme from a file.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the theme. The base theme is looked up in <see cref="ThemeManager.Themes"/>; if it is not found, the theme
	///          is based on <see cref="AUTO"/>. If a registered theme has the same identifier, the theme gets a new identifier.</returns>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a theme.</exception>
	public static Theme Import(string fileName)
	{
		DdnElement elm = Ddn.Open(fileName);
		Theme? rval = _Read(elm["theme"], out string? baseId);

		if(rval is null) { throw new InvalidDataException("The file does not contain a theme."); }
		if(ThemeManager.Find(rval.Id) is not null)
		{
			Theme copy = new(rval._Name, AUTO);
			Array.Copy(rval._Values, copy._Values, copy._Values.Length);
			rval = copy;
		}

		Theme? baseTheme = ThemeManager.Find(baseId ?? string.Empty);
		if((baseTheme is not null) && !baseTheme.DependsOn(rval)) { rval._Base = baseTheme; }

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color of the theme that corresponds to a system color.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the matching color of the theme if the color is a system color, for example <see cref="ThemeColor.FIELD_BACK"/>
	///          for <see cref="SystemColors.Window"/>, otherwise the color itself.</returns>
	/// <remarks>Theme adapters and the <see cref="ThemeToolStripRenderer"/> use this method for colors that have been set to system
	///          colors explicitly.</remarks>
	public Color MapSystemColor(Color color)
	{
		if(!color.IsSystemColor) { return color; }

		ThemeColor? role = color.ToKnownColor() switch
		{
			KnownColor.Control or KnownColor.ButtonFace or KnownColor.AppWorkspace or KnownColor.Desktop => ThemeColor.BACK,
			KnownColor.ControlText or KnownColor.ActiveCaptionText => ThemeColor.TEXT,
			KnownColor.Window or KnownColor.ControlLightLight or KnownColor.ButtonHighlight => ThemeColor.FIELD_BACK,
			KnownColor.WindowText => ThemeColor.FIELD_TEXT,
			KnownColor.GrayText or KnownColor.InactiveCaptionText => ThemeColor.GRAY_TEXT,
			KnownColor.ControlDark or KnownColor.ButtonShadow or KnownColor.WindowFrame or KnownColor.ActiveBorder or
			KnownColor.InactiveBorder => ThemeColor.FIELD_BORDER,
			KnownColor.ControlDarkDark => ThemeColor.FIELD_BORDER_HOT,
			KnownColor.ControlLight => ThemeColor.BUTTON_BACK_HOT,
			KnownColor.Highlight => ThemeColor.SELECTION_BACK,
			KnownColor.HighlightText => ThemeColor.SELECTION_TEXT,
			KnownColor.HotTrack => ThemeColor.LINK,
			KnownColor.Info => ThemeColor.POPUP_BACK,
			KnownColor.InfoText => ThemeColor.POPUP_TEXT,
			KnownColor.Menu => ThemeColor.MENU_BACK,
			KnownColor.MenuText => ThemeColor.MENU_TEXT,
			KnownColor.MenuBar => ThemeColor.MENU_BAR_BACK,
			KnownColor.MenuHighlight => ThemeColor.MENU_HIGHLIGHT_BACK,
			KnownColor.ActiveCaption or KnownColor.GradientActiveCaption => ThemeColor.ACCENT,
			KnownColor.InactiveCaption or KnownColor.GradientInactiveCaption => ThemeColor.INACTIVE_SELECTION_BACK,
			KnownColor.ScrollBar => ThemeColor.TRACK_BACK,
			_ => null
		};

		return ((role is ThemeColor r) ? this[r] : color);
	}


	/// <summary>Gets the color that is set explicitly for a role.</summary>
	/// <param name="color">Color role.</param>
	/// <returns>Returns the color, or <see cref="Color.Empty"/> if the color of the base theme is used. Built-in themes return
	///          <see cref="Color.Empty"/> for all roles.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the color role is not defined.</exception>
	public Color GetValue(ThemeColor color)
	{
		int index = _Index(color);
		return ((_Kind == _ThemeKind.CUSTOM) ? _Values[index] : Color.Empty);
	}


	/// <summary>Sets the color of a role.</summary>
	/// <param name="color">Color role.</param>
	/// <param name="value">Color, <see cref="Color.Empty"/> to use the color of the base theme.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the color role is not defined.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	public void SetValue(ThemeColor color, Color value)
	{
		int index = _Index(color);
		_ThrowIfBuiltIn();
		if(_Values[index] == value) { return; }

		_Values[index] = value;
		_OnChanged();
	}


	/// <summary>Resets all colors to the colors of the base theme.</summary>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	public void Reset()
	{
		_ThrowIfBuiltIn();
		if(!HasValues) { return; }

		Array.Clear(_Values);
		_OnChanged();
	}


	/// <summary>Copies the base theme and all colors from another theme.</summary>
	/// <param name="theme">Theme. If it is a built-in theme, this theme is based on it and all colors are reset.</param>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	/// <exception cref="InvalidOperationException">Thrown when this theme is a built-in theme.</exception>
	/// <exception cref="ArgumentException">Thrown when this theme would become its own base.</exception>
	/// <remarks>The name and the identifier are not copied.</remarks>
	public void CopyFrom(Theme theme)
	{
		ArgumentNullException.ThrowIfNull(theme);
		_ThrowIfBuiltIn();
		if(theme == this) { return; }

		Theme baseTheme = (theme.IsBuiltIn ? theme : theme._Base!);
		if(baseTheme.DependsOn(this)) { throw new ArgumentException("A theme cannot be based on itself.", nameof(theme)); }

		_Base = baseTheme;
		if(theme.IsBuiltIn) { Array.Clear(_Values); } else { Array.Copy(theme._Values, _Values, _Values.Length); }
		_OnChanged();
	}


	/// <summary>Creates a copy of this theme with a new identifier.</summary>
	/// <param name="name">Name of the copy.</param>
	/// <returns>Returns the copy. The copy of a built-in theme is an empty theme based on it.</returns>
	public Theme Copy(string name)
	{
		Theme rval = new(name, (IsBuiltIn ? this : _Base!));
		Array.Copy(_Values, rval._Values, _Values.Length);

		return rval;
	}


	/// <summary>Gets if this theme is the given theme or is based on it, directly or indirectly.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns TRUE if this theme takes colors from the given theme, otherwise FALSE.</returns>
	/// <remarks>The automatic theme depends on the theme that it currently uses.</remarks>
	public bool DependsOn(Theme theme)
	{
		for(Theme? i = this; i is not null; i = ((i._Kind == _ThemeKind.AUTO) ? _GetAutoTarget() : i._Base))
		{
			if(i == theme) { return true; }
		}

		return false;
	}


	/// <summary>Saves the theme to a file.</summary>
	/// <param name="fileName">File name.</param>
	/// <exception cref="InvalidOperationException">Thrown when the theme is a built-in theme.</exception>
	public void Export(string fileName)
	{
		_ThrowIfBuiltIn();

		DdnElement elm = Ddn.Create();
		_Write(elm["theme"]);
		elm.Save(fileName);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the theme name.</returns>
	public override string ToString()
	{
		return _Name;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _ThemeKind                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the kinds of themes.</summary>
	private enum _ThemeKind: int
	{
		/// <summary>Custom theme.</summary>
		CUSTOM = 0,

		/// <summary>System theme.</summary>
		SYSTEM = 1,

		/// <summary>Automatic theme.</summary>
		AUTO = 2,

		/// <summary>Light theme.</summary>
		LIGHT = 3,

		/// <summary>Dark theme.</summary>
		DARK = 4
	}
}
