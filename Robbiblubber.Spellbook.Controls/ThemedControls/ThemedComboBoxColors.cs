namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedComboBox"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme, so a control can use the dark theme and only change the focus color:
///          <c>combo.Colors.Theme = Theme.DARK; combo.Colors.FocusColor = Color.Orange;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedComboBoxColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.FIELD_BACK,
		ThemeColor.FIELD_TEXT,
		ThemeColor.FIELD_BORDER,
		ThemeColor.FIELD_BORDER_HOT,
		ThemeColor.FIELD_BORDER_FOCUSED,
		ThemeColor.GLYPH,
		ThemeColor.HOT_BACK,
		ThemeColor.GRAY_TEXT,
		ThemeColor.SELECTION_BACK,
		ThemeColor.SELECTION_TEXT,
		ThemeColor.POPUP_BACK,
		ThemeColor.POPUP_TEXT,
		ThemeColor.POPUP_BORDER,
		ThemeColor.ACCENT,
		ThemeColor.READ_ONLY_BACK,
		ThemeColor.READ_ONLY_TEXT,
		ThemeColor.DISABLED_BACK,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BORDER,
		ThemeColor.FIELD_BACK,
		ThemeColor.BUTTON_BACK_PRESSED
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedComboBoxColors(): base(21)
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

	/// <summary>Gets or sets the back color of the control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BackColor
	{
		get { return _GetValue((int) _Slot.BACK_COLOR); }
		set { _SetValue((int) _Slot.BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ForeColor
	{
		get { return _GetValue((int) _Slot.FORE_COLOR); }
		set { _SetValue((int) _Slot.FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BorderColor
	{
		get { return _GetValue((int) _Slot.BORDER_COLOR); }
		set { _SetValue((int) _Slot.BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the control while the mouse is over it.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the control while the mouse is over it. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBorderColor
	{
		get { return _GetValue((int) _Slot.HOT_BORDER_COLOR); }
		set { _SetValue((int) _Slot.HOT_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the line that marks the focused control.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the line that marks the focused control. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color FocusColor
	{
		get { return _GetValue((int) _Slot.FOCUS_COLOR); }
		set { _SetValue((int) _Slot.FOCUS_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the drop-down arrow.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the drop-down arrow. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ButtonForeColor
	{
		get { return _GetValue((int) _Slot.BUTTON_FORE_COLOR); }
		set { _SetValue((int) _Slot.BUTTON_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the drop-down button while the mouse is over it or the list is open.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the drop-down button while the mouse is over it or the list is open. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ButtonHotBackColor
	{
		get { return _GetValue((int) _Slot.BUTTON_HOT_BACK_COLOR); }
		set { _SetValue((int) _Slot.BUTTON_HOT_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of detail texts.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of detail texts. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DetailForeColor
	{
		get { return _GetValue((int) _Slot.DETAIL_FORE_COLOR); }
		set { _SetValue((int) _Slot.DETAIL_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the selected item in the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the selected item in the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color SelectionBackColor
	{
		get { return _GetValue((int) _Slot.SELECTION_BACK_COLOR); }
		set { _SetValue((int) _Slot.SELECTION_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the selected item in the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the selected item in the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color SelectionForeColor
	{
		get { return _GetValue((int) _Slot.SELECTION_FORE_COLOR); }
		set { _SetValue((int) _Slot.SELECTION_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DropDownBackColor
	{
		get { return _GetValue((int) _Slot.DROP_DOWN_BACK_COLOR); }
		set { _SetValue((int) _Slot.DROP_DOWN_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DropDownForeColor
	{
		get { return _GetValue((int) _Slot.DROP_DOWN_FORE_COLOR); }
		set { _SetValue((int) _Slot.DROP_DOWN_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DropDownBorderColor
	{
		get { return _GetValue((int) _Slot.DROP_DOWN_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DROP_DOWN_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of matched characters in the list.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of matched characters in the list. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HighlightColor
	{
		get { return _GetValue((int) _Slot.HIGHLIGHT_COLOR); }
		set { _SetValue((int) _Slot.HIGHLIGHT_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the control when it is read-only.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the control when it is read-only. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ReadOnlyBackColor
	{
		get { return _GetValue((int) _Slot.READ_ONLY_BACK_COLOR); }
		set { _SetValue((int) _Slot.READ_ONLY_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the control when it is read-only.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the control when it is read-only. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ReadOnlyForeColor
	{
		get { return _GetValue((int) _Slot.READ_ONLY_FORE_COLOR); }
		set { _SetValue((int) _Slot.READ_ONLY_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the control when it is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the control when it is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBackColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BACK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the control when it is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the control when it is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledForeColor
	{
		get { return _GetValue((int) _Slot.DISABLED_FORE_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the control when it is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the control when it is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBorderColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BORDER_COLOR, value); }
	}

	/// <summary>Gets or sets the back color of the control while the mouse is over it.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used. The built-in themes use the normal back
	///          color, so the control does not change its background on hover unless this color is set.</remarks>
	[Category("Appearance")][Description("The back color of the control while the mouse is over it. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBackColor
	{
		get { return _GetValue((int) _Slot.HOT_BACK_COLOR); }
		set { _SetValue((int) _Slot.HOT_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the control while the drop-down list is open.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the control while the drop-down list is open. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color PressedBackColor
	{
		get { return _GetValue((int) _Slot.PRESSED_BACK_COLOR); }
		set { _SetValue((int) _Slot.PRESSED_BACK_COLOR, value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _Slot                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the color slots.</summary>
	internal enum _Slot: int
	{
		/// <summary>Back color of the control.</summary>
		BACK_COLOR = 0,

		/// <summary>Text color of the control.</summary>
		FORE_COLOR = 1,

		/// <summary>Border color of the control.</summary>
		BORDER_COLOR = 2,

		/// <summary>Border color of the control while the mouse is over it.</summary>
		HOT_BORDER_COLOR = 3,

		/// <summary>Color of the line that marks the focused control.</summary>
		FOCUS_COLOR = 4,

		/// <summary>Color of the drop-down arrow.</summary>
		BUTTON_FORE_COLOR = 5,

		/// <summary>Back color of the drop-down button while the mouse is over it or the list is open.</summary>
		BUTTON_HOT_BACK_COLOR = 6,

		/// <summary>Color of detail texts.</summary>
		DETAIL_FORE_COLOR = 7,

		/// <summary>Back color of the selected item in the list.</summary>
		SELECTION_BACK_COLOR = 8,

		/// <summary>Text color of the selected item in the list.</summary>
		SELECTION_FORE_COLOR = 9,

		/// <summary>Back color of the list.</summary>
		DROP_DOWN_BACK_COLOR = 10,

		/// <summary>Text color of the list.</summary>
		DROP_DOWN_FORE_COLOR = 11,

		/// <summary>Border color of the list.</summary>
		DROP_DOWN_BORDER_COLOR = 12,

		/// <summary>Color of matched characters in the list.</summary>
		HIGHLIGHT_COLOR = 13,

		/// <summary>Back color of the control when it is read-only.</summary>
		READ_ONLY_BACK_COLOR = 14,

		/// <summary>Text color of the control when it is read-only.</summary>
		READ_ONLY_FORE_COLOR = 15,

		/// <summary>Back color of the control when it is disabled.</summary>
		DISABLED_BACK_COLOR = 16,

		/// <summary>Text color of the control when it is disabled.</summary>
		DISABLED_FORE_COLOR = 17,

		/// <summary>Border color of the control when it is disabled.</summary>
		DISABLED_BORDER_COLOR = 18,

		/// <summary>Back color of the control while the mouse is over it.</summary>
		HOT_BACK_COLOR = 19,

		/// <summary>Back color of the control while the drop-down list is open.</summary>
		PRESSED_BACK_COLOR = 20
	}
}
