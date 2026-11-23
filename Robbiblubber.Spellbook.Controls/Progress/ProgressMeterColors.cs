namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ProgressMeter"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>meter.Colors.Theme = Theme.DARK; meter.Colors.BarColor = Color.Green;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ProgressMeterColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.TRACK_BACK,
		ThemeColor.ACCENT,
		ThemeColor.TRACK_BORDER,
		ThemeColor.TEXT,
		ThemeColor.ACCENT_TEXT,
		ThemeColor.DISABLED_BACK,
		ThemeColor.DISABLED_BORDER,
		ThemeColor.DISABLED_BORDER,
		ThemeColor.DISABLED_TEXT
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ProgressMeterColors(): base(9)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Color _GetThemeColor(Theme theme, int slot)
	{
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



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	/// <summary>Gets or sets the back color of the meter.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the meter. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BackColor
	{
		get { return _GetValue((int) _Slot.BACK_COLOR); }
		set { _SetValue((int) _Slot.BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the bar.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the bar. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BarColor
	{
		get { return _GetValue((int) _Slot.BAR_COLOR); }
		set { _SetValue((int) _Slot.BAR_COLOR, value); }
	}


	/// <summary>Gets or sets the border color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BorderColor
	{
		get { return _GetValue((int) _Slot.BORDER_COLOR); }
		set { _SetValue((int) _Slot.BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the text outside the bar.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the text outside the bar. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the text on the bar.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the text on the bar. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BarTextColor
	{
		get { return _GetValue((int) _Slot.BAR_TEXT_COLOR); }
		set { _SetValue((int) _Slot.BAR_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the meter when it is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the meter when it is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBackColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BACK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the bar when the meter is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the bar when the meter is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBarColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BAR_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BAR_COLOR, value); }
	}


	/// <summary>Gets or sets the border color when the meter is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color when the meter is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBorderColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the meter is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the meter is disabled. Empty uses the color of the theme.")]
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
	internal enum _Slot: int
	{
		/// <summary>Back color of the meter.</summary>
		BACK_COLOR = 0,

		/// <summary>Color of the bar.</summary>
		BAR_COLOR = 1,

		/// <summary>Border color.</summary>
		BORDER_COLOR = 2,

		/// <summary>Color of the text outside the bar.</summary>
		TEXT_COLOR = 3,

		/// <summary>Color of the text on the bar.</summary>
		BAR_TEXT_COLOR = 4,

		/// <summary>Back color of the meter when it is disabled.</summary>
		DISABLED_BACK_COLOR = 5,

		/// <summary>Color of the bar when the meter is disabled.</summary>
		DISABLED_BAR_COLOR = 6,

		/// <summary>Border color when the meter is disabled.</summary>
		DISABLED_BORDER_COLOR = 7,

		/// <summary>Text color when the meter is disabled.</summary>
		DISABLED_TEXT_COLOR = 8
	}
}
