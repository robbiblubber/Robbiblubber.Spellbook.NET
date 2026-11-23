namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color table of a <see cref="ThemedGroupBox"/>.</summary>
/// <remarks>The table consists of a <see cref="ControlColorTable.Theme"/> and optional overrides of single colors. A color that is
///          <see cref="Color.Empty"/> is taken from the theme: <c>group.Colors.Theme = Theme.DARK; group.Colors.BorderColor = Color.Gray;</c></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemedGroupBoxColors: ControlColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme colors of the slots.</summary>
	private static readonly ThemeColor[] _ThemeColors = new ThemeColor[]
	{
		ThemeColor.TEXT,
		ThemeColor.SEPARATOR,
		ThemeColor.DISABLED_TEXT,
		ThemeColor.DISABLED_BORDER
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedGroupBoxColors(): base(4)
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


	/// <summary>Gets or sets the color of the caption.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the caption. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color TextColor
	{
		get { return _GetValue((int) _Slot.TEXT_COLOR); }
		set { _SetValue((int) _Slot.TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the frame.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the frame. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color BorderColor
	{
		get { return _GetValue((int) _Slot.BORDER_COLOR); }
		set { _SetValue((int) _Slot.BORDER_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the caption when the group box is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the caption when the group box is disabled. Empty uses the color of the theme.")]
	[DefaultValue(typeof(Color), "")][NotifyParentProperty(true)]
	public Color DisabledTextColor
	{
		get { return _GetValue((int) _Slot.DISABLED_TEXT_COLOR); }
		set { _SetValue((int) _Slot.DISABLED_TEXT_COLOR, value); }
	}


	/// <summary>Gets or sets the color of the frame when the group box is disabled.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the color of the <see cref="ControlColorTable.Theme"/> is used.</remarks>
	[Category("Appearance")][Description("The color of the frame when the group box is disabled. Empty uses the color of the theme.")]
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
		/// <summary>The color of the caption.</summary>
		TEXT_COLOR = 0,

		/// <summary>The color of the frame.</summary>
		BORDER_COLOR = 1,

		/// <summary>The color of the caption when the group box is disabled.</summary>
		DISABLED_TEXT_COLOR = 2,

		/// <summary>The color of the frame when the group box is disabled.</summary>
		DISABLED_BORDER_COLOR = 3
	}
}
