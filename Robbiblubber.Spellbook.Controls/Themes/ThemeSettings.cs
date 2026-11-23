namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class contains the theme settings that an <see cref="IThemeStore"/> loads and saves.</summary>
[SupportedOSPlatform("windows")]
public sealed class ThemeSettings
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the identifier of the selected theme, NULL if no theme has been selected.</summary>
	public string? SelectedTheme
	{
		get; init;
	}


	/// <summary>Gets the custom themes.</summary>
	public IReadOnlyList<Theme> Themes
	{
		get; init;
	} = Array.Empty<Theme>();
}
