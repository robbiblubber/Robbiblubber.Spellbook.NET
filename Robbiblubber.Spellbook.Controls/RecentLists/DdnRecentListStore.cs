namespace Robbiblubber.Spellbook.Controls;

using System.Globalization;
using System.Runtime.Versioning;



/// <summary>This class implements a recent list store that keeps the lists in a ddn configuration file.</summary>
/// <remarks><para>A list is stored in the section <c>recent/</c><i>name</i> of the file. Every load reads the file and every save
///          writes it, so the store does not cache anything.</para>
///          <para>Files written by the <see cref="RecentFileList"/> of earlier versions (<c>list/length</c>, <c>files/[n]</c>) are read
///          as well, as long as the list has not been saved in the current format.</para></remarks>
[SupportedOSPlatform("windows")]
public sealed class DdnRecentListStore: IRecentListStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default configuration file name.</summary>
	public const string DEFAULT_FILE_NAME = "recent.lists";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="configFile">Configuration file. A relative path is relative to the user configuration path of the application;
	///                          NULL or empty for <see cref="DEFAULT_FILE_NAME"/>.</param>
	public DdnRecentListStore(string? configFile = null)
	{
		ConfigFile = _StoreUtility._GetFullPath(configFile, DEFAULT_FILE_NAME);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads a list saved by an earlier version.</summary>
	/// <param name="cfg">Configuration.</param>
	/// <returns>Returns the entries.</returns>
	private static List<RecentItem> _ReadLegacy(DdnElement cfg)
	{
		List<RecentItem> rval = new();
		int length = cfg["list/length"];
		DateTime now = DateTime.UtcNow;

		for(int i = 0; i < length; i++)
		{
			string? key = cfg["files/[" + i.ToString() + "]"];
			if(!string.IsNullOrWhiteSpace(key)) { rval.Add(new RecentItem(key, null, false, now.AddSeconds(-i))); }
		}

		return rval;
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
	// [interface] IRecentListStore                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public IReadOnlyList<RecentItem> Load(string list)
	{
		DdnElement cfg = Ddn.Open(ConfigFile, true);
		string section = "recent/" + list + "/";
		List<RecentItem> rval = new();
		int length = cfg[section + "length"];
		int format = cfg[section + "format"];

		if(format == 0) { return _ReadLegacy(cfg); }

		for(int i = 0; i < length; i++)
		{
			string entry = section + "[" + i.ToString() + "]/";
			string? key = cfg[entry + "key"];
			string? text = cfg[entry + "text"];
			string? used = cfg[entry + "used"];
			int pinned = cfg[entry + "pinned"];

			if(string.IsNullOrWhiteSpace(key)) { continue; }
			if(!DateTime.TryParse(used, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime lastUsed)) { lastUsed = DateTime.MinValue; }

			rval.Add(new RecentItem(key, text, (pinned != 0), lastUsed));
		}

		return rval;
	}


	/// <inheritdoc/>
	public void Save(string list, IReadOnlyList<RecentItem> items)
	{
		DdnElement cfg = Ddn.Open(ConfigFile, true);
		string section = "recent/" + list + "/";

		for(int i = 0; i < items.Count; i++)
		{
			string entry = section + "[" + i.ToString() + "]/";

			cfg[entry + "key"] = items[i].Key;
			cfg[entry + "text"] = (items[i].Text ?? string.Empty);
			cfg[entry + "pinned"] = (items[i].Pinned ? 1 : 0);
			cfg[entry + "used"] = items[i].LastUsed.ToString("o", CultureInfo.InvariantCulture);
		}
		cfg[section + "length"] = items.Count;
		cfg[section + "format"] = 2;

		cfg.Save(ConfigFile);
	}
}
