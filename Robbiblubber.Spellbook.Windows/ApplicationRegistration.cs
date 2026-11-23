namespace Robbiblubber.Spellbook.Windows;

using System.Globalization;
using System.Runtime.Versioning;

using Microsoft.Win32;



/// <summary>This class registers applications in the installed apps list of Windows ("Apps &amp; features").</summary>
/// <remarks>Registrations for the current user do not require administrator rights; Windows shows them in the same list as
///          registrations for all users.</remarks>
[SupportedOSPlatform("windows")]
public static class ApplicationRegistration
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registry path of the uninstall key.</summary>
	private const string _UNINSTALL = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\";

	/// <summary>Format of the installation date.</summary>
	private const string _DATE_FORMAT = "yyyyMMdd";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a version string with two to four parts.</summary>
	/// <param name="version">Version.</param>
	/// <returns>Returns the version string; a revision of 0 is omitted.</returns>
	internal static string _ToVersionString(Version version)
	{
		if(version.Build < 0) { return version.ToString(2); }
		return version.ToString((version.Revision > 0) ? 4 : 3);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets a string value if it is not empty.</summary>
	/// <param name="key">Registry key.</param>
	/// <param name="name">Value name.</param>
	/// <param name="value">Value.</param>
	private static void _Set(RegistryKey key, string name, string? value)
	{
		if(!string.IsNullOrWhiteSpace(value)) { key.SetValue(name, value, RegistryValueKind.String); }
	}


	/// <summary>Reads a string value.</summary>
	/// <param name="key">Registry key.</param>
	/// <param name="name">Value name.</param>
	/// <returns>Returns the value, or NULL if it does not exist or is empty.</returns>
	private static string? _Get(RegistryKey key, string name)
	{
		string? rval = key.GetValue(name)?.ToString();
		return (string.IsNullOrWhiteSpace(rval) ? null : rval);
	}


	/// <summary>Reads the version of an entry.</summary>
	/// <param name="key">Registry key.</param>
	/// <returns>Returns the version.</returns>
	private static Version _GetVersion(RegistryKey key)
	{
		if(Version.TryParse(_Get(key, "DisplayVersion"), out Version? rval)) { return rval; }

		int major = ((key.GetValue("VersionMajor") is int a) ? a : 0);
		int minor = ((key.GetValue("VersionMinor") is int b) ? b : 0);
		return new(major, minor);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registers an application.</summary>
	/// <param name="application">Application.</param>
	/// <param name="scope">Scope.</param>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access, for example when registering for
	///                                                all users without administrator rights.</exception>
	/// <remarks>An existing entry with the same key is replaced completely.</remarks>
	public static void Register(InstalledApplication application, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		ArgumentNullException.ThrowIfNull(application);

		_Registry._Delete(scope, _UNINSTALL + application.Key);
		using RegistryKey k = _Registry._Create(scope, _UNINSTALL + application.Key);

		k.SetValue("DisplayName", application.DisplayName, RegistryValueKind.String);
		k.SetValue("DisplayVersion", _ToVersionString(application.Version), RegistryValueKind.String);
		k.SetValue("VersionMajor", application.Version.Major, RegistryValueKind.DWord);
		k.SetValue("VersionMinor", application.Version.Minor, RegistryValueKind.DWord);
		_Set(k, "Publisher", application.Publisher);
		_Set(k, "InstallLocation", application.InstallLocation);
		_Set(k, "DisplayIcon", application.DisplayIcon);
		_Set(k, "UninstallString", application.UninstallCommand);
		_Set(k, "QuietUninstallString", application.QuietUninstallCommand);
		_Set(k, "ModifyPath", application.ModifyCommand);
		_Set(k, "HelpLink", application.HelpUrl);
		_Set(k, "URLInfoAbout", application.AboutUrl);
		k.SetValue("InstallDate", (application.InstallDate ?? DateTime.Today).ToString(_DATE_FORMAT, CultureInfo.InvariantCulture), RegistryValueKind.String);

		if(application.EstimatedSize is long size)
		{
			k.SetValue("EstimatedSize", (int) Math.Clamp((size + 1023) / 1024, 0, int.MaxValue), RegistryValueKind.DWord);
		}

		if(string.IsNullOrWhiteSpace(application.ModifyCommand)) { k.SetValue("NoModify", 1, RegistryValueKind.DWord); }
		k.SetValue("NoRepair", 1, RegistryValueKind.DWord);
	}


	/// <summary>Removes an application entry.</summary>
	/// <param name="key">Registry key name that identifies the application.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns TRUE if the entry has been removed, FALSE if it did not exist.</returns>
	/// <exception cref="ArgumentException">Thrown when the key is empty, has leading or trailing white space or contains a
	///                                      backslash.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access.</exception>
	public static bool Unregister(string key, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		_Registry._CheckName(key, nameof(key));
		return _Registry._Delete(scope, _UNINSTALL + key);
	}


	/// <summary>Determines if an application entry exists.</summary>
	/// <param name="key">Registry key name that identifies the application.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns TRUE if the entry exists, otherwise returns FALSE.</returns>
	/// <exception cref="ArgumentException">Thrown when the key is invalid.</exception>
	public static bool IsRegistered(string key, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		_Registry._CheckName(key, nameof(key));

		using RegistryKey? k = _Registry._Open(scope, _UNINSTALL + key);
		return (k is not null);
	}


	/// <summary>Reads an application entry.</summary>
	/// <param name="key">Registry key name that identifies the application.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns the application, or NULL if the entry does not exist or has no display name.</returns>
	/// <exception cref="ArgumentException">Thrown when the key is invalid.</exception>
	/// <remarks>Entries written by other installers can be read as well.</remarks>
	public static InstalledApplication? Get(string key, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		_Registry._CheckName(key, nameof(key));

		using RegistryKey? k = _Registry._Open(scope, _UNINSTALL + key);
		string? name = ((k is null) ? null : _Get(k, "DisplayName"));
		if((k is null) || (name is null)) { return null; }

		DateTime? date = null;
		if(DateTime.TryParseExact(_Get(k, "InstallDate"), _DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) { date = d; }

		return new(key, name, _GetVersion(k))
		{
			Publisher = _Get(k, "Publisher"),
			InstallLocation = _Get(k, "InstallLocation"),
			DisplayIcon = _Get(k, "DisplayIcon"),
			UninstallCommand = _Get(k, "UninstallString"),
			QuietUninstallCommand = _Get(k, "QuietUninstallString"),
			ModifyCommand = _Get(k, "ModifyPath"),
			EstimatedSize = ((k.GetValue("EstimatedSize") is int size) ? (size * 1024L) : null),
			InstallDate = date,
			HelpUrl = _Get(k, "HelpLink"),
			AboutUrl = _Get(k, "URLInfoAbout")
		};
	}
}
