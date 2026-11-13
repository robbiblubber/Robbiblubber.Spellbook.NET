namespace Robbiblubber.Spellbook;

using System.Reflection;



/// <summary>This class provides version methods.</summary>
public static class VersionOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the version of the application (the entry assembly); 0.0.0.0 if there is none.</summary>
	public static Version ApplicationVersion
	{
		get { return (Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0, 0, 0)); }
	}


	/// <summary>Gets the copyright of the application (the entry assembly); an empty string if there is none.</summary>
	public static string ApplicationCopyright
	{
		get { return (Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses a version.</summary>
	/// <param name="version">Version string; a single number is read as major version.</param>
	/// <param name="value">Version.</param>
	/// <returns>Returns TRUE if the string is a valid version, otherwise returns FALSE.</returns>
	private static bool _TryParse(string? version, out Version value)
	{
		value = new Version(0, 0);
		if(version is null) { return false; }

		string v = version.Trim();
		if(int.TryParse(v, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int major))
		{
			value = new Version(major, 0);
			return true;
		}

		if(Version.TryParse(v, out Version? rval))
		{
			value = rval;
			return true;
		}

		return false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a string to a version.</summary>
	/// <param name="version">Version string; a single number is read as major version.</param>
	/// <param name="defaultValue">Value returned if the string is not a valid version.</param>
	/// <returns>Returns the version, or the default value.</returns>
	public static Version ToVersion(string? version, Version defaultValue)
	{
		return (_TryParse(version, out Version rval) ? rval : defaultValue);
	}


	/// <summary>Converts a string to a version.</summary>
	/// <param name="version">Version string; a single number is read as major version.</param>
	/// <param name="defaultValue">Value returned if the string is not a valid version.</param>
	/// <returns>Returns the version, or the default value.</returns>
	/// <exception cref="ArgumentException">Thrown when the default value is not a valid version.</exception>
	public static Version ToVersion(string? version, string defaultValue = "1.0.0")
	{
		if(_TryParse(version, out Version rval)) { return rval; }
		if(_TryParse(defaultValue, out rval)) { return rval; }

		throw new ArgumentException("The default value is not a valid version.", nameof(defaultValue));
	}


	/// <summary>Returns if a string is a valid version.</summary>
	/// <param name="version">Version string; a single number is read as major version.</param>
	/// <returns>Returns TRUE if the string is a valid version, otherwise returns FALSE.</returns>
	public static bool IsVersion(string? version)
	{
		return _TryParse(version, out _);
	}


	/// <summary>Gets the version of the assembly that defines a type or the type of an object.</summary>
	/// <param name="obj">Type or object.</param>
	/// <returns>Returns the assembly version; 0.0.0.0 if it has none.</returns>
	public static Version VersionOf(object obj)
	{
		return (((obj as Type) ?? obj.GetType()).Assembly.GetName().Version ?? new Version(0, 0, 0, 0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a short string representation of a version.</summary>
	/// <param name="version">Version.</param>
	/// <returns>Returns the version with two or three parts, or four if the revision is greater than 0; "0.0" for NULL.</returns>
	public static string ToVersionString(this Version? version)
	{
		if(version is null) { return "0.0"; }
		if(version.Build < 0) { return version.ToString(2); }

		return version.ToString((version.Revision > 0) ? 4 : 3);
	}
}
