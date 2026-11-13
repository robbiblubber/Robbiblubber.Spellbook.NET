namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements a window layout store that keeps the layouts in a ddn configuration file.</summary>
/// <remarks><para>The values of a layout are stored in the section <c>layout/</c><i>name</i> of the file. Written values are kept in
///          memory until <see cref="Commit"/> is called; the file is then read again, so changes made by other windows in the meantime
///          are kept.</para>
///          <para>Layouts saved by earlier versions (<c>pos</c>, <c>size</c> and <c>state</c>) are read as well.</para></remarks>
[SupportedOSPlatform("windows")]
public sealed class DdnWindowLayoutStore: IWindowLayoutStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default configuration file name.</summary>
	public const string DEFAULT_FILE_NAME = "app.layout";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Configuration, NULL if it has not been loaded yet.</summary>
	private DdnElement? _Config = null;

	/// <summary>Values written since the last commit.</summary>
	private readonly List<(string Layout, string Key, int[] Value)> _Pending = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="configFile">Configuration file. A relative path is relative to the user configuration path of the application;
	///                          NULL or empty for <see cref="DEFAULT_FILE_NAME"/>.</param>
	public DdnWindowLayoutStore(string? configFile = null)
	{
		ConfigFile = _StoreUtility._GetFullPath(configFile, DEFAULT_FILE_NAME);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the configuration, loading it if necessary.</summary>
	/// <returns>Returns the configuration.</returns>
	private DdnElement _GetConfig()
	{
		return (_Config ??= Ddn.Open(ConfigFile, true));
	}


	/// <summary>Reads a layout saved by an earlier version.</summary>
	/// <param name="section">Layout section.</param>
	/// <returns>Returns the window value, or NULL if there is no layout.</returns>
	private static int[]? _ReadLegacy(DdnElement section)
	{
		int[] pos = section.Get("pos", Array.Empty<int>());
		int[] size = section.Get("size", Array.Empty<int>());

		if((pos.Length < 2) || (size.Length < 2)) { return null; }
		return new int[] { pos[0], pos[1], size[0], size[1], 0, (int) section.Get("state", FormWindowState.Normal) };
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
	// [interface] IWindowLayoutStore                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public int[]? Read(string layout, string key)
	{
		DdnElement section = _GetConfig()["layout"][layout];
		int[] rval = section.Get(key, Array.Empty<int>());

		if((rval.Length == 0) && (key == WindowLayoutProvider.WINDOW_KEY)) { return _ReadLegacy(section); }
		return ((rval.Length == 0) ? null : rval);
	}


	/// <inheritdoc/>
	public void Write(string layout, string key, int[] value)
	{
		_Pending.Add((layout, key, value));
	}


	/// <inheritdoc/>
	public void Commit()
	{
		if(_Pending.Count == 0) { return; }

		_Config = Ddn.Open(ConfigFile, true);
		foreach((string Layout, string Key, int[] Value) i in _Pending) { _Config["layout"][i.Layout][i.Key] = i.Value; }
		_Config.Save(ConfigFile);

		_Pending.Clear();
	}
}
