namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.Versioning;



/// <summary>This class describes an application entry in the installed apps list of Windows ("Apps &amp; features").</summary>
/// <example><code>
/// ApplicationRegistration.Register(new InstalledApplication("Robbiblubber.Rqoon", "rqoon", new Version(4, 0, 1))
/// {
///     Publisher = "robbiblubber.org",
///     InstallLocation = AppContext.BaseDirectory,
///     DisplayIcon = Environment.ProcessPath,
///     UninstallCommand = $"\"{Path.Combine(AppContext.BaseDirectory, "uninstall.exe")}\""
/// });
/// </code></example>
[SupportedOSPlatform("windows")]
public sealed class InstalledApplication
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="key">Registry key name that identifies the application.</param>
	/// <param name="displayName">Display name.</param>
	/// <param name="version">Version.</param>
	/// <exception cref="ArgumentException">Thrown when the key is empty, has leading or trailing white space or contains a
	///                                      backslash, or when the display name is empty.</exception>
	public InstalledApplication(string key, string displayName, Version version)
	{
		_Registry._CheckName(key, nameof(key));
		ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
		ArgumentNullException.ThrowIfNull(version);

		Key = key;
		DisplayName = displayName;
		Version = version;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the registry key name that identifies the application.</summary>
	public string Key
	{
		get;
	}


	/// <summary>Gets the display name.</summary>
	public string DisplayName
	{
		get;
	}


	/// <summary>Gets the version.</summary>
	public Version Version
	{
		get;
	}


	/// <summary>Gets the publisher name.</summary>
	public string? Publisher
	{
		get; init;
	}


	/// <summary>Gets the installation directory.</summary>
	public string? InstallLocation
	{
		get; init;
	}


	/// <summary>Gets the display icon, a file path optionally followed by a comma and an icon index.</summary>
	public string? DisplayIcon
	{
		get; init;
	}


	/// <summary>Gets the command line that uninstalls the application. Windows may hide entries without uninstall command.</summary>
	public string? UninstallCommand
	{
		get; init;
	}


	/// <summary>Gets the command line that uninstalls the application without user interaction.</summary>
	public string? QuietUninstallCommand
	{
		get; init;
	}


	/// <summary>Gets the command line that modifies the installation, or NULL if the installation cannot be modified.</summary>
	public string? ModifyCommand
	{
		get; init;
	}


	/// <summary>Gets the estimated size of the installation in bytes, or NULL if unknown.</summary>
	public long? EstimatedSize
	{
		get; init;
	}


	/// <summary>Gets the installation date, or NULL to use the current date when registering.</summary>
	public DateTime? InstallDate
	{
		get; init;
	}


	/// <summary>Gets the URL of the support page.</summary>
	public string? HelpUrl
	{
		get; init;
	}


	/// <summary>Gets the URL of the product page.</summary>
	public string? AboutUrl
	{
		get; init;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string ToString()
	{
		return ($"{DisplayName} {ApplicationRegistration._ToVersionString(Version)}" + ((Publisher is null) ? "" : $" ({Publisher})"));
	}
}
