namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="PasswordStrengthIndicator"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme. Every strength has its own fill color; all themes use a traffic light
///          from red to green, with lighter colors in dark themes.</remarks>
[SupportedOSPlatform("windows")]
public sealed class PasswordStrengthColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.TRACK_BACK,
		ThemeColor.TRACK_BORDER,
		ThemeColor.TEXT,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT,
		ThemeColor.DISABLED_BACK,
		ThemeColor.DISABLED_BORDER,
		ThemeColor.DISABLED_TEXT
	};

	/// <summary>Fill colors of the strengths in light themes, from very weak to very strong.</summary>
	private static readonly Color[] _LightStrengthColors = new Color[]
	{
		_Rgb(0xD32F2F),
		_Rgb(0xF57C00),
		_Rgb(0xFBC02D),
		_Rgb(0x7CB342),
		_Rgb(0x388E3C),
		_Rgb(0x1B5E20)
	};

	/// <summary>Fill colors of the strengths in dark themes, from very weak to very strong.</summary>
	private static readonly Color[] _DarkStrengthColors = new Color[]
	{
		_Rgb(0xEF5350),
		_Rgb(0xFF9800),
		_Rgb(0xFDD835),
		_Rgb(0x9CCC65),
		_Rgb(0x66BB6A),
		_Rgb(0x43A047)
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public PasswordStrengthColors(): base(12)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Color _GetThemeColor(Theme theme, int slot)
	{
		if((slot >= (int) _Slot.VERY_WEAK_COLOR) && (slot <= (int) _Slot.VERY_STRONG_COLOR))
		{
			return (theme.IsDark ? _DarkStrengthColors : _LightStrengthColors)[slot - (int) _Slot.VERY_WEAK_COLOR];
		}

		return theme[_ThemeColors[slot]];
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the effective color of a slot.</summary>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the color override if it is set, otherwise the color of the theme.</returns>
	internal Color _Get(_Slot slot)
	{
		return _GetEffective((int) slot);
	}


	/// <summary>Gets the effective fill color for a strength.</summary>
	/// <param name="strength">Password strength.</param>
	/// <returns>Returns the fill color; <see cref="PasswordStrength.NONE"/> returns the color for very weak passwords.</returns>
	internal Color _GetFill(PasswordStrength strength)
	{
		return _GetEffective((int) _Slot.VERY_WEAK_COLOR + Math.Clamp((int) strength, 0, 5));
	}


	/// <summary>Sets the fill color of all strengths.</summary>
	/// <param name="color">Color, <see cref="Color.Empty"/> for the theme colors.</param>
	internal void _SetAllFills(Color color)
	{
		for(int i = 0; i < 6; i++) { _SetValue((int) _Slot.VERY_WEAK_COLOR + i, color); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	/// <summary>Gets or sets the fill color of empty boxes.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color of empty boxes. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color EmptyColor
	{
		get { return _GetValue((int) _Slot.EMPTY_COLOR); }
		set { _SetValue((int) _Slot.EMPTY_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the boxes.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the boxes. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BorderColor
	{
		get { return _GetValue((int) _Slot.BORDER_COLOR); }
		set { _SetValue((int) _Slot.BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the strength text.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the strength text. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for very weak passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for very weak passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color VeryWeakColor
	{
		get { return _GetValue((int) _Slot.VERY_WEAK_COLOR); }
		set { _SetValue((int) _Slot.VERY_WEAK_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for weak passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for weak passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color WeakColor
	{
		get { return _GetValue((int) _Slot.WEAK_COLOR); }
		set { _SetValue((int) _Slot.WEAK_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for fair passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for fair passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color FairColor
	{
		get { return _GetValue((int) _Slot.FAIR_COLOR); }
		set { _SetValue((int) _Slot.FAIR_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for good passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for good passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color GoodColor
	{
		get { return _GetValue((int) _Slot.GOOD_COLOR); }
		set { _SetValue((int) _Slot.GOOD_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for strong passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for strong passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color StrongColor
	{
		get { return _GetValue((int) _Slot.STRONG_COLOR); }
		set { _SetValue((int) _Slot.STRONG_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color for very strong passwords.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color for very strong passwords. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color VeryStrongColor
	{
		get { return _GetValue((int) _Slot.VERY_STRONG_COLOR); }
		set { _SetValue((int) _Slot.VERY_STRONG_COLOR, value); }
	}


	/// <summary>Gets or sets the fill color of filled boxes when the indicator is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The fill color of filled boxes when the indicator is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledFillColor
	{
		get { return _GetValue((int) _Slot.DISABLED_FILL_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_FILL_COLOR, value); }
	}


	/// <summary>Gets or sets the border color when the indicator is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color when the indicator is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBorderColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the indicator is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the indicator is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledTextColor
	{
		get { return _GetValue((int) _Slot.DISABLED_TEXT_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_TEXT_COLOR, value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _Slot                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the color slots.</summary>
	/// <remarks>The fill colors of the strengths are consecutive, in the order of <see cref="PasswordStrength"/>.</remarks>
	internal enum _Slot: int
	{
		/// <summary>Fill color of empty boxes.</summary>
		EMPTY_COLOR = 0,

		/// <summary>Border color of the boxes.</summary>
		BORDER_COLOR = 1,

		/// <summary>Color of the strength text.</summary>
		TEXT_COLOR = 2,

		/// <summary>Fill color for very weak passwords.</summary>
		VERY_WEAK_COLOR = 3,

		/// <summary>Fill color for weak passwords.</summary>
		WEAK_COLOR = 4,

		/// <summary>Fill color for fair passwords.</summary>
		FAIR_COLOR = 5,

		/// <summary>Fill color for good passwords.</summary>
		GOOD_COLOR = 6,

		/// <summary>Fill color for strong passwords.</summary>
		STRONG_COLOR = 7,

		/// <summary>Fill color for very strong passwords.</summary>
		VERY_STRONG_COLOR = 8,

		/// <summary>Fill color of filled boxes when the indicator is disabled.</summary>
		DISABLED_FILL_COLOR = 9,

		/// <summary>Border color when the indicator is disabled.</summary>
		DISABLED_BORDER_COLOR = 10,

		/// <summary>Text color when the indicator is disabled.</summary>
		DISABLED_TEXT_COLOR = 11
	}
}
