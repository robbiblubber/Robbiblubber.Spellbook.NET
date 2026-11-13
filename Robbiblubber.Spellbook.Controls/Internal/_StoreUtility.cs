namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides helper methods shared by the ddn stores.</summary>
[SupportedOSPlatform("windows")]
internal static class _StoreUtility
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the full path of a configuration file.</summary>
	/// <param name="configFile">Configuration file. A relative path is relative to the user configuration path of the application;
	///                          NULL or empty for the default file name.</param>
	/// <param name="defaultFileName">Default file name.</param>
	/// <returns>Returns the full path.</returns>
	internal static string _GetFullPath(string? configFile, string defaultFileName)
	{
		if(string.IsNullOrWhiteSpace(configFile)) { configFile = defaultFileName; }
		if(Path.IsPathRooted(configFile)) { return configFile; }

		return Path.Combine(PathOp.UserConfigurationPath, configFile.TrimStart(' ', '\\', '/'));
	}
}
