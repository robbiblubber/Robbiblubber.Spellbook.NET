namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class implements a theme store that shares the custom themes between applications.</summary>
/// <remarks><para>Each custom theme is a file in a theme directory that all applications of the user use, by default
///          <see cref="DefaultDirectory"/>; the files have the format of <see cref="Theme.Export(string)"/>. The selected theme is
///          stored for each application in its own configuration file, so every application can show another theme. With it, the
///          base themes of the selected theme are stored, so the application falls back to the next base theme that still exists if
///          another application has removed the selected theme.</para>
///          <para>Saving only writes the themes that have been changed in this application and only deletes the themes that have been
///          removed in it, so themes that other applications have added or changed in the meantime are kept. Themes that an older
///          version has stored in the configuration file of the application are taken over into the theme directory with the next
///          save.</para>
///          <para><see cref="ThemeManager.Refresh"/> takes over the changes of other applications; the
///          <see cref="ThemeEditorDialog"/> does this when it is opened.</para>
///          <para><c>ThemeManager.Load(new SharedThemeStore());</c></para></remarks>
[SupportedOSPlatform("windows")]
public sealed class SharedThemeStore: IThemeStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default configuration file name.</summary>
	public const string DEFAULT_FILE_NAME = "app.themes";

	/// <summary>File extension of theme files.</summary>
	public const string THEME_FILE_EXTENSION = ".theme";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Files and contents of the themes as they have been loaded or saved last, by theme identifier.</summary>
	private readonly Dictionary<string, (string File, string Text)> _Known = new(StringComparer.OrdinalIgnoreCase);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="themeDirectory">Directory of the theme files, NULL or empty for <see cref="DefaultDirectory"/>.</param>
	/// <param name="configFile">Configuration file of the application that keeps the selected theme. A relative path is relative to the
	///                          user configuration path of the application; NULL or empty for <see cref="DEFAULT_FILE_NAME"/>.</param>
	public SharedThemeStore(string? themeDirectory = null, string? configFile = null)
	{
		ThemeDirectory = (string.IsNullOrWhiteSpace(themeDirectory) ? DefaultDirectory : Path.GetFullPath(themeDirectory));
		ConfigFile = _StoreUtility._GetFullPath(configFile, DEFAULT_FILE_NAME);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the default theme directory, <c>%APPDATA%\robbiblubber\themes</c>.</summary>
	public static string DefaultDirectory
	{
		get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "robbiblubber", "themes"); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the file content of a theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns the ddn text.</returns>
	private static string _ToText(Theme theme)
	{
		DdnElement elm = Ddn.Create();
		theme._Write(elm["theme"]);

		return elm.ToString();
	}


	/// <summary>Reads a theme file.</summary>
	/// <param name="file">File name.</param>
	/// <param name="baseId">Returns the identifier of the base theme.</param>
	/// <returns>Returns the theme, or NULL if the file does not contain a valid theme.</returns>
	private static Theme? _ReadFile(string file, out string? baseId)
	{
		baseId = null;
		try
		{
			return Theme._Read(Ddn.Open(file)["theme"], out baseId);
		}
		catch(Exception) { return null; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the full path of the theme directory.</summary>
	public string ThemeDirectory
	{
		get;
	}


	/// <summary>Gets the full path of the configuration file of the application.</summary>
	public string ConfigFile
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads all theme files of the theme directory.</summary>
	/// <returns>Returns the themes with the identifiers of their base themes and their files.</returns>
	private List<(Theme Theme, string? BaseId, string File)> _ReadDirectory()
	{
		List<(Theme Theme, string? BaseId, string File)> rval = new();
		if(!Directory.Exists(ThemeDirectory)) { return rval; }

		string[] files;
		try { files = Directory.GetFiles(ThemeDirectory, "*" + THEME_FILE_EXTENSION); } catch(Exception) { return rval; }
		Array.Sort(files, StringComparer.OrdinalIgnoreCase);

		foreach(string i in files)
		{
			Theme? theme = _ReadFile(i, out string? baseId);
			if((theme is null) || rval.Any(m => string.Equals(m.Theme.Id, theme.Id, StringComparison.OrdinalIgnoreCase))) { continue; }

			rval.Add((theme, baseId, i));
		}

		return rval;
	}


	/// <summary>Gets a new file name for a theme.</summary>
	/// <param name="id">Theme identifier.</param>
	/// <returns>Returns the full path of a file that does not exist or that belongs to the same theme.</returns>
	private string _NewFile(string id)
	{
		char[] invalid = Path.GetInvalidFileNameChars();
		string name = new(id.Select(m => (invalid.Contains(m) ? '_' : m)).ToArray());

		string rval = Path.Combine(ThemeDirectory, name + THEME_FILE_EXTENSION);
		for(int i = 2; File.Exists(rval); i++)
		{
			if(string.Equals(_ReadFile(rval, out _)?.Id, id, StringComparison.OrdinalIgnoreCase)) { break; }
			rval = Path.Combine(ThemeDirectory, name + "-" + i.ToString() + THEME_FILE_EXTENSION);
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the changes that other applications have made since the last load or save.</summary>
	/// <param name="current">Registered custom themes.</param>
	/// <param name="changed">Returns themes that have been added or changed by other applications. Their base themes are built-in themes,
	///                       registered themes or other returned themes.</param>
	/// <param name="removed">Returns registered themes that have been removed by other applications.</param>
	/// <remarks>Themes that have been changed or removed in this application as well keep the state of this application, so the next
	///          save stores it.</remarks>
	internal void _Refresh(IReadOnlyList<Theme> current, out List<Theme> changed, out List<Theme> removed)
	{
		changed = new();
		removed = new();

		List<(Theme Theme, string? BaseId, string File)> files = _ReadDirectory();
		Theme? find(string? id)
		{
			if(string.IsNullOrWhiteSpace(id)) { return null; }
			return (Theme._GetBuiltIn(id) ?? current.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase)) ??
			        files.Select(m => m.Theme).FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase)));
		}

		foreach((Theme theme, string? baseId, string file) in files)
		{
			Theme? baseTheme = find(baseId);
			if((baseTheme is not null) && !baseTheme.DependsOn(theme)) { theme.Base = baseTheme; }
		}

		foreach((Theme theme, string? baseId, string file) in files)
		{
			string text = _ToText(theme);
			bool known = _Known.TryGetValue(theme.Id, out (string File, string Text) last);
			if(known && (last.Text == text)) { continue; }

			Theme? local = current.FirstOrDefault(m => string.Equals(m.Id, theme.Id, StringComparison.OrdinalIgnoreCase));
			if(local is null)
			{
				if(known) { continue; }
			}
			else if(known && (_ToText(local) != last.Text)) { continue; }

			changed.Add(theme);
			_Known[theme.Id] = (file, text);
		}

		foreach(string i in _Known.Keys.ToArray())
		{
			if(files.Any(m => string.Equals(m.Theme.Id, i, StringComparison.OrdinalIgnoreCase))) { continue; }

			Theme? local = current.FirstOrDefault(m => string.Equals(m.Id, i, StringComparison.OrdinalIgnoreCase));
			if((local is not null) && (_ToText(local) == _Known[i].Text)) { removed.Add(local); }
			_Known.Remove(i);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IThemeStore                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Base themes that cannot be found are replaced by <see cref="Theme.AUTO"/>.</remarks>
	public ThemeSettings Load()
	{
		_Known.Clear();

		List<(Theme Theme, string? BaseId, string File)> files = _ReadDirectory();
		List<Theme> themes = files.Select(m => m.Theme).ToList();
		List<string?> baseIds = files.Select(m => m.BaseId).ToList();

		DdnElement cfg = Ddn.Open(ConfigFile, true);
		int length = cfg["themes/length"];
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

		for(int i = 0; i < files.Count; i++) { _Known[files[i].Theme.Id] = (files[i].File, _ToText(files[i].Theme)); }

		string? selected = cfg["themes/selected"];
		if(string.IsNullOrWhiteSpace(selected)) { selected = null; }

		int fallbacks = cfg["themes/fallback/length"];
		for(int i = 0; (selected is not null) && (Theme._GetBuiltIn(selected) is null) && !themes.Any(m => string.Equals(m.Id, selected, StringComparison.OrdinalIgnoreCase)) && (i < fallbacks); i++)
		{
			selected = cfg["themes/fallback/[" + i.ToString() + "]"];
		}

		return new ThemeSettings() { SelectedTheme = selected, Themes = themes };
	}


	/// <inheritdoc/>
	/// <remarks>Only the themes that differ from the last load or save are written, and only the files of themes that have been loaded or
	///          saved before and are missing in the settings are deleted.</remarks>
	public void Save(ThemeSettings settings)
	{
		ArgumentNullException.ThrowIfNull(settings);

		Directory.CreateDirectory(ThemeDirectory);
		HashSet<string> ids = new(StringComparer.OrdinalIgnoreCase);

		foreach(Theme i in settings.Themes)
		{
			if(i.IsBuiltIn || !ids.Add(i.Id)) { continue; }

			string text = _ToText(i);
			bool known = _Known.TryGetValue(i.Id, out (string File, string Text) last);
			if(known && (last.Text == text)) { continue; }

			string file = (known ? last.File : _NewFile(i.Id));
			Ddn.Create(text).Save(file);
			_Known[i.Id] = (file, text);
		}

		foreach(string i in _Known.Keys.Where(m => !ids.Contains(m)).ToArray())
		{
			string file = _Known[i].File;
			if(File.Exists(file) && string.Equals(_ReadFile(file, out _)?.Id, i, StringComparison.OrdinalIgnoreCase)) { File.Delete(file); }
			_Known.Remove(i);
		}

		DdnElement cfg = Ddn.Open(ConfigFile, true);
		cfg.Remove("themes");
		cfg["themes/selected"] = (settings.SelectedTheme ?? string.Empty);

		List<string> fallback = new();
		Theme? selected = settings.Themes.FirstOrDefault(m => string.Equals(m.Id, settings.SelectedTheme, StringComparison.OrdinalIgnoreCase));
		for(Theme? i = selected?.Base; i is not null; i = i.Base) { fallback.Add(i.Id); }
		for(int i = 0; i < fallback.Count; i++) { cfg["themes/fallback/[" + i.ToString() + "]"] = fallback[i]; }
		cfg["themes/fallback/length"] = fallback.Count;

		cfg.Save(ConfigFile);
	}
}
