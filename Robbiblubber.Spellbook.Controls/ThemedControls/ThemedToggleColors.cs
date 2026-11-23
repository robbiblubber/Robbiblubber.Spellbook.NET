namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedCheckBox"/> or <see cref="ThemedRadioButton"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>check.Colors.Theme = Theme.DARK; check.Colors.CheckedBackColor = Color.Green;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedToggleColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.TEXT,
		ThemeColor.FIELD_BACK,
		ThemeColor.GLYPH,
		ThemeColor.HOT_BACK,
		ThemeColor.BUTTON_BACK_PRESSED,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT_HOT,
		ThemeColor.ACCENT_PRESSED,
		ThemeColor.ACCENT_TEXT,
		ThemeColor.FIELD_BORDER_FOCUSED,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BACK
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedToggleColors(): base(14)
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


	/// <summary>Gets or sets the text color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the box or circle.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the box or circle. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BoxBackColor
	{
		get { return _GetValue((int) _Slot.BOX_BACK_COLOR); }
		set { _SetValue((int) _Slot.BOX_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the box or circle.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the box or circle. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BoxBorderColor
	{
		get { return _GetValue((int) _Slot.BOX_BORDER_COLOR); }
		set { _SetValue((int) _Slot.BOX_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the box or circle while the mouse is over the control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the box or circle while the mouse is over the control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBoxBackColor
	{
		get { return _GetValue((int) _Slot.HOT_BOX_BACK_COLOR); }
		set { _SetValue((int) _Slot.HOT_BOX_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the box or circle while the control is pressed.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the box or circle while the control is pressed. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color PressedBoxBackColor
	{
		get { return _GetValue((int) _Slot.PRESSED_BOX_BACK_COLOR); }
		set { _SetValue((int) _Slot.PRESSED_BOX_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the box or circle when the control is checked.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the box or circle when the control is checked. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color CheckedBackColor
	{
		get { return _GetValue((int) _Slot.CHECKED_BACK_COLOR); }
		set { _SetValue((int) _Slot.CHECKED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the checked box or circle while the mouse is over the control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the checked box or circle while the mouse is over the control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color CheckedHotBackColor
	{
		get { return _GetValue((int) _Slot.CHECKED_HOT_BACK_COLOR); }
		set { _SetValue((int) _Slot.CHECKED_HOT_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the checked box or circle while the control is pressed.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the checked box or circle while the control is pressed. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color CheckedPressedBackColor
	{
		get { return _GetValue((int) _Slot.CHECKED_PRESSED_BACK_COLOR); }
		set { _SetValue((int) _Slot.CHECKED_PRESSED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the check mark and the dot.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the check mark and the dot. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color CheckMarkColor
	{
		get { return _GetValue((int) _Slot.CHECK_MARK_COLOR); }
		set { _SetValue((int) _Slot.CHECK_MARK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the focus border.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the focus border. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color FocusColor
	{
		get { return _GetValue((int) _Slot.FOCUS_COLOR); }
		set { _SetValue((int) _Slot.FOCUS_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the control is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledTextColor
	{
		get { return _GetValue((int) _Slot.DISABLED_TEXT_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the box or circle when the control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the box or circle when the control is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBoxBorderColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BOX_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BOX_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the checked box or circle when the control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the checked box or circle when the control is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledCheckedBackColor
	{
		get { return _GetValue((int) _Slot.DISABLED_CHECKED_BACK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_CHECKED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the check mark and the dot when the control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the check mark and the dot when the control is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledCheckMarkColor
	{
		get { return _GetValue((int) _Slot.DISABLED_CHECK_MARK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_CHECK_MARK_COLOR, value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _Slot                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the color slots.</summary>
	internal enum _Slot: int
	{
		/// <summary>The text color.</summary>
		TEXT_COLOR = 0,

		/// <summary>The back color of the box or circle.</summary>
		BOX_BACK_COLOR = 1,

		/// <summary>The border color of the box or circle.</summary>
		BOX_BORDER_COLOR = 2,

		/// <summary>The back color of the box or circle while the mouse is over the control.</summary>
		HOT_BOX_BACK_COLOR = 3,

		/// <summary>The back color of the box or circle while the control is pressed.</summary>
		PRESSED_BOX_BACK_COLOR = 4,

		/// <summary>The back color of the box or circle when the control is checked.</summary>
		CHECKED_BACK_COLOR = 5,

		/// <summary>The back color of the checked box or circle while the mouse is over the control.</summary>
		CHECKED_HOT_BACK_COLOR = 6,

		/// <summary>The back color of the checked box or circle while the control is pressed.</summary>
		CHECKED_PRESSED_BACK_COLOR = 7,

		/// <summary>The color of the check mark and the dot.</summary>
		CHECK_MARK_COLOR = 8,

		/// <summary>The color of the focus border.</summary>
		FOCUS_COLOR = 9,

		/// <summary>The text color when the control is disabled.</summary>
		DISABLED_TEXT_COLOR = 10,

		/// <summary>The border color of the box or circle when the control is disabled.</summary>
		DISABLED_BOX_BORDER_COLOR = 11,

		/// <summary>The back color of the checked box or circle when the control is disabled.</summary>
		DISABLED_CHECKED_BACK_COLOR = 12,

		/// <summary>The color of the check mark and the dot when the control is disabled.</summary>
		DISABLED_CHECK_MARK_COLOR = 13
	}
}
