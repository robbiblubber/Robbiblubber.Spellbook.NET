namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class provides the palettes of the built-in themes.</summary>
/// <remarks>The arrays are indexed by <see cref="ThemeColor"/>.</remarks>
[SupportedOSPlatform("windows")]
internal static class _ThemePalettes
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static members                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Palette of the system theme.</summary>
	/// <remarks>System colors are resolved when they are used, so the palette follows changes of the system colors and the color
	///          mode of the application.</remarks>
	internal static readonly Color[] _System = new Color[]
	{
		SystemColors.Control,
		SystemColors.ControlText,
		SystemColors.GrayText,
		SystemColors.ControlText,
		SystemColors.Window,
		SystemColors.WindowText,
		SystemColors.ControlDark,
		SystemColors.ControlDarkDark,
		SystemColors.Highlight,
		SystemColors.Control,
		SystemColors.WindowText,
		SystemColors.Control,
		SystemColors.ControlText,
		SystemColors.ControlDark,
		SystemColors.ControlLight,
		SystemColors.Control,
		SystemColors.Highlight,
		SystemColors.HighlightText,
		SystemColors.Highlight,
		SystemColors.Highlight,
		SystemColors.Highlight,
		SystemColors.HighlightText,
		SystemColors.Control,
		SystemColors.ControlText,
		SystemColors.Control,
		SystemColors.Control,
		SystemColors.GrayText,
		SystemColors.ControlDark,
		SystemColors.Control,
		SystemColors.ControlDarkDark,
		SystemColors.Window,
		SystemColors.WindowText,
		SystemColors.ControlDark,
		SystemColors.Control,
		SystemColors.ControlText,
		SystemColors.ControlDark,
		SystemColors.MenuBar,
		SystemColors.Menu,
		SystemColors.MenuText,
		SystemColors.MenuHighlight,
		SystemColors.HighlightText,
		SystemColors.ControlDark,
		SystemColors.ControlDark,
		SystemColors.Control,
		SystemColors.ControlText,
		SystemColors.HotTrack,
		_Rgb(0xC42B1C),
		_Rgb(0x9D5D00),
		_Rgb(0x0F7B0F)
	};


	/// <summary>Palette of the light theme.</summary>
	internal static readonly Color[] _Light = new Color[]
	{
		_Rgb(0xF3F3F3),
		_Rgb(0x1A1A1A),
		_Rgb(0x6E6E6E),
		_Rgb(0x5D5D5D),
		_Rgb(0xFFFFFF),
		_Rgb(0x1A1A1A),
		_Rgb(0xA0A0A0),
		_Rgb(0x707070),
		_Rgb(0x0067C0),
		_Rgb(0xF3F3F3),
		_Rgb(0x1A1A1A),
		_Rgb(0xFBFBFB),
		_Rgb(0x1A1A1A),
		_Rgb(0xD0D0D0),
		_Rgb(0xF5F5F5),
		_Rgb(0xEBEBEB),
		_Rgb(0x0067C0),
		_Rgb(0xFFFFFF),
		_Rgb(0x1975C5),
		_Rgb(0x3183CA),
		_Rgb(0xCCE4F7),
		_Rgb(0x1A1A1A),
		_Rgb(0xE5E5E5),
		_Rgb(0x1A1A1A),
		_Rgb(0xE8E8E8),
		_Rgb(0xF5F5F5),
		_Rgb(0xA0A0A0),
		_Rgb(0xD0D0D0),
		_Rgb(0xE6E6E6),
		_Rgb(0xA0A0A0),
		_Rgb(0xFFFFFF),
		_Rgb(0x1A1A1A),
		_Rgb(0xA0A0A0),
		_Rgb(0xF3F3F3),
		_Rgb(0x1A1A1A),
		_Rgb(0xE0E0E0),
		_Rgb(0xF3F3F3),
		_Rgb(0xF9F9F9),
		_Rgb(0x1A1A1A),
		_Rgb(0xE0EEF9),
		_Rgb(0x1A1A1A),
		_Rgb(0xCCCCCC),
		_Rgb(0xD0D0D0),
		_Rgb(0xF3F3F3),
		_Rgb(0x1A1A1A),
		_Rgb(0x0067C0),
		_Rgb(0xC42B1C),
		_Rgb(0x9D5D00),
		_Rgb(0x0F7B0F)
	};


	/// <summary>Palette of the dark theme.</summary>
	internal static readonly Color[] _Dark = new Color[]
	{
		_Rgb(0x202020),
		_Rgb(0xF0F0F0),
		_Rgb(0x9A9A9A),
		_Rgb(0xC8C8C8),
		_Rgb(0x2B2B2B),
		_Rgb(0xF0F0F0),
		_Rgb(0x5A5A5A),
		_Rgb(0x808080),
		_Rgb(0x4CC2FF),
		_Rgb(0x202020),
		_Rgb(0xD0D0D0),
		_Rgb(0x2D2D2D),
		_Rgb(0xF0F0F0),
		_Rgb(0x454545),
		_Rgb(0x323232),
		_Rgb(0x3A3A3A),
		_Rgb(0x4CC2FF),
		_Rgb(0x000000),
		_Rgb(0x47B1E8),
		_Rgb(0x42A1D2),
		_Rgb(0x094771),
		_Rgb(0xFFFFFF),
		_Rgb(0x3A3A3A),
		_Rgb(0xF0F0F0),
		_Rgb(0x3C3C3C),
		_Rgb(0x252525),
		_Rgb(0x6E6E6E),
		_Rgb(0x3A3A3A),
		_Rgb(0x2B2B2B),
		_Rgb(0x5A5A5A),
		_Rgb(0x252526),
		_Rgb(0xF0F0F0),
		_Rgb(0x454545),
		_Rgb(0x2B2B2B),
		_Rgb(0xF0F0F0),
		_Rgb(0x3A3A3A),
		_Rgb(0x202020),
		_Rgb(0x2B2B2B),
		_Rgb(0xF0F0F0),
		_Rgb(0x3D3D3D),
		_Rgb(0xFFFFFF),
		_Rgb(0x454545),
		_Rgb(0x454545),
		_Rgb(0x202020),
		_Rgb(0xF0F0F0),
		_Rgb(0x4CC2FF),
		_Rgb(0xFF99A4),
		_Rgb(0xFCE100),
		_Rgb(0x6CCB5F)
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates an opaque color from an RGB value.</summary>
	/// <param name="rgb">RGB value, for example <c>0x1A1A1A</c>.</param>
	/// <returns>Returns the color.</returns>
	private static Color _Rgb(int rgb)
	{
		return Color.FromArgb(255, Color.FromArgb(rgb));
	}
}
