namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class provides a base for the color tables of self-drawn controls.</summary>
/// <remarks><para>A color table consists of a <see cref="Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme; without a theme of its own, the table uses the theme of the application
///          (<see cref="ThemeManager.Theme"/>). Derived classes define the colors as slots, map every slot to a color of the theme and
///          expose the slots as properties.</para>
///          <para><see cref="Changed"/> occurs when a color is set and when the colors of the effective theme change, so the owning
///          control only has to repaint itself.</para></remarks>
[SupportedOSPlatform("windows")]
[TypeConverter(typeof(ExpandableObjectConverter))]
public abstract class ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme, NULL for the theme of the application.</summary>
	private Theme? _Theme = null;

	/// <summary>Theme of the themed parent controls, NULL for the theme of the application.</summary>
	private Theme? _Ambient = null;

	/// <summary>Color overrides, <see cref="Color.Empty"/> for the theme color.</summary>
	private readonly Color[] _Values;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="slotCount">Number of color slots.</param>
	protected ControlColorTable(int slotCount)
	{
		_Values = new Color[slotCount];
		ThemeManager._Register(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected static methods                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates an opaque color from an RGB value.</summary>
	/// <param name="rgb">RGB value, for example <c>0x1A1A1A</c>.</param>
	/// <returns>Returns the color.</returns>
	protected static Color _Rgb(int rgb)
	{
		return Color.FromArgb(255, Color.FromArgb(rgb));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color of a slot in a theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the color, usually a color of the theme like <c>theme[ThemeColor.FIELD_BACK]</c>. Colors that are not part of
	///          the theme can depend on <see cref="Theme.IsDark"/>.</returns>
	protected abstract Color _GetThemeColor(Theme theme, int slot);


	/// <summary>Gets the color override of a slot.</summary>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the override, or <see cref="Color.Empty"/> if the theme color is used.</returns>
	protected Color _GetValue(int slot)
	{
		return _Values[slot];
	}


	/// <summary>Sets the color override of a slot.</summary>
	/// <param name="slot">Color slot.</param>
	/// <param name="value">Color, <see cref="Color.Empty"/> to use the theme color.</param>
	protected void _SetValue(int slot, Color value)
	{
		if(_Values[slot] == value) { return; }

		_Values[slot] = value;
		Changed?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Gets the effective color of a slot.</summary>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the color override if it is set, otherwise the color of the effective theme.</returns>
	protected Color _GetEffective(int slot)
	{
		Color rval = _Values[slot];
		return (rval.IsEmpty ? _GetThemeColor(EffectiveTheme, slot) : rval);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the theme that the theme manager has applied to the control, NULL for the theme of the application.</summary>
	/// <remarks>The theme is used when <see cref="Theme"/> is not set.</remarks>
	internal Theme? _AmbientTheme
	{
		get { return _Ambient; }
		set
		{
			if(value == _Ambient) { return; }

			Theme old = EffectiveTheme;
			_Ambient = value;
			if(EffectiveTheme != old) { Changed?.Invoke(this, EventArgs.Empty); }
		}
	}


	/// <summary>Raises the <see cref="Changed"/> event after the colors of the effective theme have changed.</summary>
	internal void _OnThemeChanged()
	{
		Changed?.Invoke(this, EventArgs.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the theme, a color or the colors of the effective theme have changed.</summary>
	public event EventHandler? Changed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the theme.</summary>
	/// <remarks>The theme provides all colors that are not set explicitly. NULL uses the theme of the application.</remarks>
	[Category("Appearance")][Description("The theme that provides all colors that are not set explicitly; (Application) for the application theme.")]
	[DefaultValue(null)][NotifyParentProperty(true)][RefreshProperties(RefreshProperties.Repaint)]
	public Theme? Theme
	{
		get { return _Theme; }
		set
		{
			if(value == _Theme) { return; }

			_Theme = value;
			Changed?.Invoke(this, EventArgs.Empty);
		}
	}


	/// <summary>Gets the theme that provides the colors.</summary>
	/// <remarks>This is <see cref="Theme"/>; if no theme is set, the theme that has been applied to the control with
	///          <see cref="ThemeManager.Apply(Control, Theme?)"/> or a <see cref="ThemeProvider"/>, otherwise the theme of the
	///          application.</remarks>
	[Browsable(false)]
	public Theme EffectiveTheme
	{
		get { return (_Theme ?? _Ambient ?? ThemeManager.Theme); }
	}


	/// <summary>Gets if no theme and no color is set explicitly.</summary>
	[Browsable(false)]
	public bool IsDefault
	{
		get { return ((_Theme is null) && _Values.All(m => m.IsEmpty)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Resets the theme and all colors.</summary>
	public void Reset()
	{
		if(IsDefault) { return; }

		_Theme = null;
		Array.Clear(_Values);
		Changed?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Copies the theme and all colors from another color table of the same type.</summary>
	/// <param name="colors">Color table.</param>
	/// <exception cref="ArgumentNullException">Thrown when the color table is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the color table is of another type.</exception>
	public void CopyFrom(ControlColorTable colors)
	{
		ArgumentNullException.ThrowIfNull(colors);
		if(ReferenceEquals(colors, this)) { return; }
		if(colors.GetType() != GetType()) { throw new ArgumentException("The color table must be of the same type.", nameof(colors)); }

		_Theme = colors._Theme;
		Array.Copy(colors._Values, _Values, _Values.Length);
		Changed?.Invoke(this, EventArgs.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the theme name and the number of explicitly set colors.</returns>
	public override string ToString()
	{
		string theme = (_Theme?.Name ?? "Application");
		int count = _Values.Count(m => !m.IsEmpty);

		return ((count == 0) ? theme : $"{theme}, {count} custom");
	}
}
