namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedButton"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>button.Colors.Theme = Theme.DARK; button.Colors.BackColor = Color.DarkRed;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedButtonColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.BUTTON_BACK,
		ThemeColor.BUTTON_TEXT,
		ThemeColor.BUTTON_BORDER,
		ThemeColor.BUTTON_BACK_HOT,
		ThemeColor.BUTTON_BACK_PRESSED,
		ThemeColor.ACCENT,
		ThemeColor.FIELD_BORDER_FOCUSED,
		ThemeColor.DISABLED_BACK,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BORDER
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedButtonColors(): base(10)
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


	/// <summary>Gets or sets the back color of the button.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the button. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BackColor
	{
		get { return _GetValue((int) _Slot.BACK_COLOR); }
		set { _SetValue((int) _Slot.BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
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


	/// <summary>Gets or sets the back color while the mouse is over the button.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color while the mouse is over the button. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBackColor
	{
		get { return _GetValue((int) _Slot.HOT_BACK_COLOR); }
		set { _SetValue((int) _Slot.HOT_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color while the button is pressed.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color while the button is pressed. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color PressedBackColor
	{
		get { return _GetValue((int) _Slot.PRESSED_BACK_COLOR); }
		set { _SetValue((int) _Slot.PRESSED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the default button.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the default button. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DefaultBorderColor
	{
		get { return _GetValue((int) _Slot.DEFAULT_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DEFAULT_BORDER_COLOR, value); }
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


	/// <summary>Gets or sets the back color when the button is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color when the button is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBackColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BACK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the button is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the button is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledTextColor
	{
		get { return _GetValue((int) _Slot.DISABLED_TEXT_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the border color when the button is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color when the button is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBorderColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BORDER_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BORDER_COLOR, value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _Slot                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the color slots.</summary>
	internal enum _Slot: int
	{
		/// <summary>The back color of the button.</summary>
		BACK_COLOR = 0,

		/// <summary>The text color.</summary>
		TEXT_COLOR = 1,

		/// <summary>The border color.</summary>
		BORDER_COLOR = 2,

		/// <summary>The back color while the mouse is over the button.</summary>
		HOT_BACK_COLOR = 3,

		/// <summary>The back color while the button is pressed.</summary>
		PRESSED_BACK_COLOR = 4,

		/// <summary>The border color of the default button.</summary>
		DEFAULT_BORDER_COLOR = 5,

		/// <summary>The color of the focus border.</summary>
		FOCUS_COLOR = 6,

		/// <summary>The back color when the button is disabled.</summary>
		DISABLED_BACK_COLOR = 7,

		/// <summary>The text color when the button is disabled.</summary>
		DISABLED_TEXT_COLOR = 8,

		/// <summary>The border color when the button is disabled.</summary>
		DISABLED_BORDER_COLOR = 9
	}
}
