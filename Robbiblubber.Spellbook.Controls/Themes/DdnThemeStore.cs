namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class implements a theme store that keeps the settings in a ddn configuration file.</summary>
/// <remarks>The settings are stored in the section <c>themes</c> of the file. Every load reads the file and every save writes it,
///          so the store does not cache anything. Base themes that cannot be found are replaced by <see cref="Theme.AUTO"/>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class DdnThemeStore: IThemeStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default configuration file name.</summary>
	public const string DEFAULT_FILE_NAME = "app.themes";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="configFile">Configuration file. A relative path is relative to the user configuration path of the application;
	///                          NULL or empty for <see cref="DEFAULT_FILE_NAME"/>.</param>
	public DdnThemeStore(string? configFile = null)
	{
		ConfigFile = _StoreUtility._GetFullPath(configFile, DEFAULT_FILE_NAME);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the full path of the configuration file.</summary>
	public string ConfigFile
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IThemeStore                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public ThemeSettings Load()
	{
		DdnElement cfg = Ddn.Open(ConfigFile, true);
		int length = cfg["themes/length"];
		List<Theme> themes = new();
		List<string?> baseIds = new();

		for(int i = 0; i < length; i++)
		{
			Theme? theme = Theme._Read(cfg["themes/[" + i.ToString() + "]"], out string? baseId);

			if((theme is null) || themes.Any(m => string.Equals(m.Id, theme.Id, StringComparison.OrdinalIgnoreCase))) { continue; }
			themes.Add(theme);
			baseIds.Add(baseId);
		}

		for(int i = 0; i < themes.Count; i++)
		{
			Theme? baseTheme = (Theme._GetBuiltIn(baseIds[i]) ??
								themes.FirstOrDefault(m => string.Equals(m.Id, baseIds[i], StringComparison.OrdinalIgnoreCase)));
			if((baseTheme is not null) && !baseTheme.DependsOn(themes[i])) { themes[i].Base = baseTheme; }
		}

		string? selected = cfg["themes/selected"];
		return new ThemeSettings() { SelectedTheme = (string.IsNullOrWhiteSpace(selected) ? null : selected), Themes = themes };
	}


	/// <inheritdoc/>
	public void Save(ThemeSettings settings)
	{
		ArgumentNullException.ThrowIfNull(settings);

		DdnElement cfg = Ddn.Open(ConfigFile, true);
		cfg.Remove("themes");

		for(int i = 0; i < settings.Themes.Count; i++)
		{
			settings.Themes[i]._Write(cfg["themes/[" + i.ToString() + "]"]);
		}
		cfg["themes/length"] = settings.Themes.Count;
		cfg["themes/selected"] = (settings.SelectedTheme ?? string.Empty);

		cfg.Save(ConfigFile);
	}
}
