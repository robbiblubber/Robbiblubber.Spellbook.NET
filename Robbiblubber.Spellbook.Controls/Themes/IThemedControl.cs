namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This interface is implemented by self-drawn controls that take their colors from a <see cref="ControlColorTable"/>.</summary>
/// <remarks><para>When a theme is applied to a parent control (<see cref="ThemeManager.Apply(Control, Theme?)"/>, a
///          <see cref="ThemeProvider"/>), the theme manager passes it to the color table, which uses it for the colors as long as its
///          <see cref="ControlColorTable.Theme"/> is not set; <see cref="ControlColorTable.EffectiveTheme"/> returns it. The table raises
///          <see cref="ControlColorTable.Changed"/>, so the control repaints. If <see cref="ThemesChildren"/> is <c>false</c>, the
///          theme manager leaves the child controls to the control.</para>
///          <para>Controls of other libraries implement the interface explicitly, so it does not show up in their public members:
///          <code>ControlColorTable IThemedControl.ThemeColors => Colors;
///bool IThemedControl.ThemesChildren => false;</code></para></remarks>
[SupportedOSPlatform("windows")]
public interface IThemedControl
{
	/// <summary>Gets the color table.</summary>
	ControlColorTable ThemeColors
	{
		get;
	}


	/// <summary>Gets if the theme manager applies the theme to the child controls.</summary>
	bool ThemesChildren
	{
		get;
	}
}
