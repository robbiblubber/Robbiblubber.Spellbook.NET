namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedTextBox"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>text.Colors.Theme = Theme.DARK; text.Colors.FocusColor = Color.Orange;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedTextBoxColors: ControlColorTable
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
		ThemeColor.READ_ONLY_BACK,
		ThemeColor.READ_ONLY_TEXT,
		ThemeColor.DISABLED_BACK,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BORDER
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedTextBoxColors(): base(10)
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

	/// <summary>Gets or sets the back color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color. Empty uses the color of the theme.")]
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
	public Color ForeColor
	{
		get { return _GetValue((int) _Slot.FORE_COLOR); }
		set { _SetValue((int) _Slot.FORE_COLOR, value); }
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


	/// <summary>Gets or sets the border color while the mouse is over the text box.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color while the mouse is over the text box. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color HotBorderColor
	{
		get { return _GetValue((int) _Slot.HOT_BORDER_COLOR); }
		set { _SetValue((int) _Slot.HOT_BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the border color while the text box has the focus.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color while the text box has the focus. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color FocusColor
	{
		get { return _GetValue((int) _Slot.FOCUS_COLOR); }
		set { _SetValue((int) _Slot.FOCUS_COLOR, value); }
	}


	/// <summary>Gets or sets the back color when the text box is read-only.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color when the text box is read-only. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ReadOnlyBackColor
	{
		get { return _GetValue((int) _Slot.READ_ONLY_BACK_COLOR); }
		set { _SetValue((int) _Slot.READ_ONLY_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the text box is read-only.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the text box is read-only. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color ReadOnlyForeColor
	{
		get { return _GetValue((int) _Slot.READ_ONLY_FORE_COLOR); }
		set { _SetValue((int) _Slot.READ_ONLY_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the back color when the text box is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The back color when the text box is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledBackColor
	{
		get { return _GetValue((int) _Slot.DISABLED_BACK_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_BACK_COLOR, value); }
	}


	/// <summary>Gets or sets the text color when the text box is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The text color when the text box is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledForeColor
	{
		get { return _GetValue((int) _Slot.DISABLED_FORE_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_FORE_COLOR, value); }
	}


	/// <summary>Gets or sets the border color when the text box is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The border color when the text box is disabled. Empty uses the color of the theme.")]
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
		/// <summary>The back color.</summary>
		BACK_COLOR = 0,

		/// <summary>The text color.</summary>
		FORE_COLOR = 1,

		/// <summary>The border color.</summary>
		BORDER_COLOR = 2,

		/// <summary>The border color while the mouse is over the text box.</summary>
		HOT_BORDER_COLOR = 3,

		/// <summary>The border color while the text box has the focus.</summary>
		FOCUS_COLOR = 4,

		/// <summary>The back color when the text box is read-only.</summary>
		READ_ONLY_BACK_COLOR = 5,

		/// <summary>The text color when the text box is read-only.</summary>
		READ_ONLY_FORE_COLOR = 6,

		/// <summary>The back color when the text box is disabled.</summary>
		DISABLED_BACK_COLOR = 7,

		/// <summary>The text color when the text box is disabled.</summary>
		DISABLED_FORE_COLOR = 8,

		/// <summary>The border color when the text box is disabled.</summary>
		DISABLED_BORDER_COLOR = 9
	}
}
