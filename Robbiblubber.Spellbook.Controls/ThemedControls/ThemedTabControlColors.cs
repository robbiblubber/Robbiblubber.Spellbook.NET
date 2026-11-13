namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedTabControl"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>tabs.Colors.Theme = Theme.DARK; tabs.Colors.AccentColor = Color.Orange;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedTabControlColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.TEXT,
		ThemeColor.HOT_BACK,
		ThemeColor.BACK,
		ThemeColor.SEPARATOR,
		ThemeColor.ACCENT,
		ThemeColor.ACCENT_TEXT,
		ThemeColor.BUTTON_BACK,
		ThemeColor.BUTTON_BORDER,
		ThemeColor.FIELD_BORDER_FOCUSED,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BORDER
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedTabControlColors(): base(11)
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

	/// <summary>Gets or sets the text color of the tabs.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the tabs. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of a tab while the mouse is over it.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of a tab while the mouse is over it. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBackColor
	{
		get { return _GetValue((int) _Slot.HOT_BACK_COLOR); }
		set { _SetValue((int) _Slot.HOT_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the selected tab and the page area if the tab page uses the visual style back color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the selected tab and the page area if the tab page uses the visual style back color. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color PageBackColor
	{
		get { return _GetValue((int) _Slot.PAGE_BACK_COLOR); }
		set { _SetValue((int) _Slot.PAGE_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the frame around the page area and the selected tab.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the frame around the page area and the selected tab. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BorderColor
	{
		get { return _GetValue((int) _Slot.BORDER_COLOR); }
		set { _SetValue((int) _Slot.BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the line on the selected tab and the back color of the selected button.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the line on the selected tab and the back color of the selected button. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color AccentColor
	{
		get { return _GetValue((int) _Slot.ACCENT_COLOR); }
		set { _SetValue((int) _Slot.ACCENT_COLOR, value); }
	}


	/// <summary>Gets or sets the text color of the selected button.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color of the selected button. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color AccentTextColor
	{
		get { return _GetValue((int) _Slot.ACCENT_TEXT_COLOR); }
		set { _SetValue((int) _Slot.ACCENT_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the back color of the buttons if the tabs look like buttons.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color of the buttons if the tabs look like buttons. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ButtonBackColor
	{
		get { return _GetValue((int) _Slot.BUTTON_BACK_COLOR); }
		set { _SetValue((int) _Slot.BUTTON_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the border color of the buttons if the tabs look like buttons.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color of the buttons if the tabs look like buttons. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ButtonBorderColor
	{
		get { return _GetValue((int) _Slot.BUTTON_BORDER_COLOR); }
		set { _SetValue((int) _Slot.BUTTON_BORDER_COLOR, value); }
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


	/// <summary>Gets or sets the text color when the tab control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the tab control is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledTextColor
	{
		get { return _GetValue((int) _Slot.DISABLED_TEXT_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the frame and the line when the tab control is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the frame and the line when the tab control is disabled. Empty uses the color of the theme.")]
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
		/// <summary>The text color of the tabs.</summary>
		TEXT_COLOR = 0,

		/// <summary>The back color of a tab while the mouse is over it.</summary>
		HOT_BACK_COLOR = 1,

		/// <summary>The back color of the selected tab and the page area if the tab page uses the visual style back color.</summary>
		PAGE_BACK_COLOR = 2,

		/// <summary>The color of the frame around the page area and the selected tab.</summary>
		BORDER_COLOR = 3,

		/// <summary>The color of the line on the selected tab and the back color of the selected button.</summary>
		ACCENT_COLOR = 4,

		/// <summary>The text color of the selected button.</summary>
		ACCENT_TEXT_COLOR = 5,

		/// <summary>The back color of the buttons if the tabs look like buttons.</summary>
		BUTTON_BACK_COLOR = 6,

		/// <summary>The border color of the buttons if the tabs look like buttons.</summary>
		BUTTON_BORDER_COLOR = 7,

		/// <summary>The color of the focus border.</summary>
		FOCUS_COLOR = 8,

		/// <summary>The text color when the tab control is disabled.</summary>
		DISABLED_TEXT_COLOR = 9,

		/// <summary>The color of the frame and the line when the tab control is disabled.</summary>
		DISABLED_BORDER_COLOR = 10
	}
}
