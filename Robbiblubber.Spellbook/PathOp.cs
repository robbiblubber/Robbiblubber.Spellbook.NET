namespace Robbiblubber.Spellbook;



/// <summary>This class provides application, configuration and temporary paths.</summary>
/// <remarks><para>The local configuration path is shared by all users of the machine (Windows: <c>C:\ProgramData</c>), the user
///          configuration path belongs to the current user (Windows: <c>%LOCALAPPDATA%</c>). Both are extended by the
///          <see cref="ApplicationPart"/>. Their directories are created on first access.</para>
///          <para>All paths are combined with <see cref="Path.Combine(string[])"/> and work on every platform.</para></remarks>
public static class PathOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Application part segments.</summary>
	private static string[] _ApplicationPart = [];

	/// <summary>Temporary files root directory; NULL for the default.</summary>
	private static string? _TempRoot = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the system local (machine-wide) configuration path.</summary>
	public static string SystemLocalConfigurationPath
	{
		get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); }
	}


	/// <summary>Gets the system user configuration path.</summary>
	public static string SystemUserConfigurationPath
	{
		get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); }
	}


	/// <summary>Gets the application-specific local (machine-wide) configuration path; the directory is created if it does
	///          not exist.</summary>
	/// <exception cref="UnauthorizedAccessException">Thrown when the directory cannot be created due to missing permissions.</exception>
	public static string LocalConfigurationPath
	{
		get { return _Ensure(Path.Combine([SystemLocalConfigurationPath, .._ApplicationPart])); }
	}


	/// <summary>Gets the application-specific user configuration path; the directory is created if it does not exist.</summary>
	public static string UserConfigurationPath
	{
		get { return _Ensure(Path.Combine([SystemUserConfigurationPath, .._ApplicationPart])); }
	}


	/// <summary>Gets or sets the application part of the configuration paths, e.g. "robbiblubber.org/MyApp".</summary>
	/// <remarks>Both '/' and '\' separate directories. NULL or an empty string use the system paths directly.</remarks>
	public static string? ApplicationPart
	{
		get { return ((_ApplicationPart.Length == 0) ? null : string.Join('/', _ApplicationPart)); }
		set { _ApplicationPart = (value ?? string.Empty).Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); }
	}


	/// <summary>Gets the application directory (without trailing separator).</summary>
	public static string ApplicationDirectory
	{
		get { return Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory); }
	}


	/// <summary>Gets the path of the application executable; an empty string if it is unknown.</summary>
	public static string ApplicationExecutable
	{
		get { return (Environment.ProcessPath ?? string.Empty); }
	}


	/// <summary>Gets the directory of the Spellbook library; the application directory if the library is embedded in a single
	///          file application.</summary>
	public static string LibraryDirectory
	{
		get
		{
			string? rval = Path.GetDirectoryName(typeof(PathOp).Assembly.Location);
			return (string.IsNullOrEmpty(rval) ? ApplicationDirectory : rval);
		}
	}


	/// <summary>Gets the user settings file.</summary>
	public static string UserSettingsFile
	{
		get { return Path.Combine(UserConfigurationPath, "application.settings"); }
	}


	/// <summary>Gets or sets the temporary files root directory; the directory is created if it does not exist.</summary>
	/// <remarks>The default is the directory "robbiblubber.org" in the system temporary directory. NULL restores the
	///          default.</remarks>
	public static string TempRoot
	{
		get { return _Ensure(_TempRoot ?? Path.Combine(Path.GetTempPath(), "robbiblubber.org")); }
		set { _TempRoot = value; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a directory if it does not exist.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns the path.</returns>
	private static string _Ensure(string path)
	{
		Directory.CreateDirectory(path);
		return path;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the application part of the configuration paths.</summary>
	/// <param name="applicationPart">Application part, e.g. "robbiblubber.org/MyApp".</param>
	public static void Initialize(string applicationPart)
	{
		ApplicationPart = applicationPart;
	}


	/// <summary>Gets a unique temporary file name in <see cref="TempRoot"/>; the file is not created.</summary>
	/// <param name="extension">File extension; NULL for ".tmp".</param>
	/// <returns>Returns the file name.</returns>
	public static string GetTempFile(string? extension = null)
	{
		return Path.Combine(TempRoot, StringOp.Unique() + "." + (extension ?? "tmp").Trim().TrimStart('.'));
	}
}
