namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>Classes that store the custom themes and the selected theme implement this interface.</summary>
/// <remarks>The default implementation is <see cref="DdnThemeStore"/>.</remarks>
[SupportedOSPlatform("windows")]
public interface IThemeStore
{
	/// <summary>Loads the theme settings.</summary>
	/// <returns>Returns the settings. A store that does not contain settings returns empty settings.</returns>
	/// <remarks>The base themes of the returned themes must be built-in themes or themes contained in the settings.</remarks>
	public ThemeSettings Load();


	/// <summary>Saves the theme settings.</summary>
	/// <param name="settings">Settings.</param>
	public void Save(ThemeSettings settings);
}
