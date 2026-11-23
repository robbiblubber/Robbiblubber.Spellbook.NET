namespace Robbiblubber.Spellbook;



/// <summary>This class provides file and directory operations.</summary>
/// <remarks>Wildcards are '*' and '?'. File names are compared without regard to case on Windows and macOS and with regard to
///          case on other platforms.</remarks>
public static class FileOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Determines if file names are case-sensitive on this platform.</summary>
	private static readonly bool _CaseSensitive = (!(OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst()));

	/// <summary>Names that are reserved on Windows.</summary>
	private static readonly string[] _RESERVED = ["CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
												  "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"];



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the comparison for file names on this platform.</summary>
	private static StringComparison _Comparison
	{
		get { return (_CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase); }
	}


	/// <summary>Splits a path with wildcards into directory and pattern.</summary>
	/// <param name="path">Path; the file name part may contain wildcards.</param>
	/// <param name="directory">Directory; the current directory if the path has none.</param>
	/// <param name="pattern">Pattern.</param>
	private static void _Split(string path, out string directory, out string pattern)
	{
		string? dir = Path.GetDirectoryName(path);
		directory = (string.IsNullOrEmpty(dir) ? Directory.GetCurrentDirectory() : dir);
		pattern = Path.GetFileName(path);
	}


	/// <summary>Returns if a path is the same as or inside another path.</summary>
	/// <param name="path">Path.</param>
	/// <param name="parent">Parent path.</param>
	/// <returns>Returns TRUE if the path is inside the parent path, otherwise returns FALSE.</returns>
	private static bool _IsInside(string path, string parent)
	{
		string p = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path)) + Path.DirectorySeparatorChar;
		string q = Path.TrimEndingDirectorySeparator(Path.GetFullPath(parent)) + Path.DirectorySeparatorChar;

		return p.StartsWith(q, _Comparison);
	}


	/// <summary>Copies a directory.</summary>
	/// <param name="source">Source directory.</param>
	/// <param name="destination">Destination directory.</param>
	/// <param name="recursive">Determines if subdirectories are copied.</param>
	/// <param name="overwrite">Determines if existing files are overwritten.</param>
	/// <param name="errors">List that receives errors; NULL to throw on the first error.</param>
	private static void _CopyDirectory(DirectoryInfo source, string destination, bool recursive, bool overwrite, List<Exception>? errors)
	{
		try
		{
			Directory.CreateDirectory(destination);
		}
		catch(Exception ex) when(errors is not null) { errors.Add(ex); return; }

		foreach(FileInfo i in source.EnumerateFiles())
		{
			string target = Path.Combine(destination, i.Name);
			if((!overwrite) && File.Exists(target)) { continue; }

			try
			{
				i.CopyTo(target, overwrite);
			}
			catch(Exception ex) when(errors is not null) { errors.Add(ex); }
		}

		if(!recursive) { return; }
		foreach(DirectoryInfo i in source.EnumerateDirectories())
		{
			_CopyDirectory(i, Path.Combine(destination, i.Name), true, overwrite, errors);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Copies files.</summary>
	/// <param name="source">Source files; the file name may contain wildcards. Without directory the current directory is
	///                      used.</param>
	/// <param name="destination">Destination directory; it is created if necessary.</param>
	/// <param name="recursive">Determines if matching files in subdirectories are copied (into the same subdirectories of the
	///                         destination).</param>
	/// <param name="overwrite">Determines if existing files are overwritten.</param>
	public static void CopyFiles(string source, string destination, bool recursive = false, bool overwrite = true)
	{
		_Split(source, out string dir, out string pattern);

		foreach(string i in Directory.GetFiles(dir, pattern, (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)))
		{
			string target = Path.Combine(destination, Path.GetRelativePath(dir, i));
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);

			File.Copy(i, target, overwrite);
		}
	}


	/// <summary>Tries to copy a file.</summary>
	/// <param name="source">Source file.</param>
	/// <param name="destination">Destination file.</param>
	/// <param name="overwrite">Determines if an existing file is overwritten.</param>
	/// <returns>Returns TRUE if the file has been copied, otherwise returns FALSE.</returns>
	public static bool TryCopyFile(string source, string destination, bool overwrite = true)
	{
		try
		{
			File.Copy(source, destination, overwrite);
			return true;
		}
		catch(Exception) { return false; }
	}


	/// <summary>Deletes one or more files.</summary>
	/// <param name="fileName">File name; the file name part may contain wildcards. Without directory the current directory is
	///                        used.</param>
	/// <param name="recursive">Determines if matching files in subdirectories are deleted.</param>
	/// <param name="throwException">Determines if errors throw an exception; if FALSE, files that cannot be deleted are
	///                              skipped.</param>
	/// <exception cref="IOException">Thrown when a file could not be deleted.</exception>
	/// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
	public static void Delete(string fileName, bool recursive = false, bool throwException = true)
	{
		_Split(fileName, out string dir, out string pattern);

		string[] files;
		try
		{
			files = Directory.GetFiles(dir, pattern, (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
		}
		catch(Exception) when(!throwException) { return; }

		foreach(string i in files)
		{
			try
			{
				File.Delete(i);
			}
			catch(Exception) when(!throwException) {}
		}
	}


	/// <summary>Copies a directory.</summary>
	/// <param name="source">Source directory.</param>
	/// <param name="destination">Destination directory; it is created if necessary.</param>
	/// <param name="recursive">Determines if subdirectories are copied.</param>
	/// <param name="overwrite">Determines if existing files are overwritten; if FALSE, they are skipped.</param>
	/// <exception cref="DirectoryNotFoundException">Thrown when the source directory does not exist.</exception>
	/// <exception cref="IOException">Thrown when the destination is inside the source and the copy is recursive.</exception>
	public static void CopyDirectory(string source, string destination, bool recursive = true, bool overwrite = true)
	{
		DirectoryInfo src = new(source);

		if(!src.Exists) { throw new DirectoryNotFoundException("Source directory not found: " + source); }
		if(recursive && _IsInside(destination, source)) { throw new IOException("The destination directory is inside the source directory."); }

		_CopyDirectory(src, destination, recursive, overwrite, null);
	}


	/// <summary>Copies a directory as far as possible.</summary>
	/// <param name="source">Source directory.</param>
	/// <param name="destination">Destination directory; it is created if necessary.</param>
	/// <param name="recursive">Determines if subdirectories are copied.</param>
	/// <param name="overwrite">Determines if existing files are overwritten; if FALSE, they are skipped.</param>
	/// <returns>Returns TRUE if everything has been copied, otherwise returns FALSE. All files that can be copied are copied.</returns>
	public static bool TryCopyDirectory(string source, string destination, bool recursive = true, bool overwrite = true)
	{
		DirectoryInfo src = new(source);
		if((!src.Exists) || (recursive && _IsInside(destination, source))) { return false; }

		List<Exception> errors = new();
		try
		{
			_CopyDirectory(src, destination, recursive, overwrite, errors);
		}
		catch(Exception) { return false; }

		return (errors.Count == 0);
	}


	/// <summary>Deletes the contents of a directory.</summary>
	/// <param name="directoryName">Directory.</param>
	/// <param name="keep">Names of files or directories to keep; wildcards are allowed.</param>
	/// <returns>Returns the files and directories that have not been deleted.</returns>
	/// <exception cref="IOException">Thrown when files or directories could not be deleted.</exception>
	public static string[] PurgeDirectory(string directoryName, params string[] keep)
	{
		return PurgeDirectory(directoryName, false, keep);
	}


	/// <summary>Deletes the contents of a directory.</summary>
	/// <param name="directoryName">Directory.</param>
	/// <param name="ignoreErrors">Determines if errors are ignored.</param>
	/// <param name="keep">Names of files or directories to keep; wildcards are allowed.</param>
	/// <returns>Returns the files and directories that could not be deleted.</returns>
	/// <exception cref="IOException">Thrown when files or directories could not be deleted, unless errors are ignored.</exception>
	public static string[] PurgeDirectory(string directoryName, bool ignoreErrors, params string[]? keep)
	{
		keep ??= [];
		List<string> faulty = new();

		if(ignoreErrors && (!Directory.Exists(directoryName))) { return []; }

		foreach(string i in Directory.GetDirectories(directoryName))
		{
			if(Path.GetFileName(i).Matches(_CaseSensitive, keep)) { continue; }

			try
			{
				Directory.Delete(i, true);
			}
			catch(Exception) { faulty.Add(i); }
		}

		foreach(string i in Directory.GetFiles(directoryName))
		{
			if(Path.GetFileName(i).Matches(_CaseSensitive, keep)) { continue; }

			try
			{
				File.Delete(i);
			}
			catch(Exception) { faulty.Add(i); }
		}

		if(ignoreErrors || (faulty.Count == 0)) { return faulty.ToArray(); }

		throw new IOException("Failed to delete " + string.Join(", ", faulty) + ".");
	}


	/// <summary>Creates a new temporary directory in <see cref="PathOp.TempRoot"/>.</summary>
	/// <returns>Returns the directory.</returns>
	public static string CreateTempPath()
	{
		string rval;
		do { rval = Path.Combine(PathOp.TempRoot, StringOp.Random(32)); }
		while(Directory.Exists(rval) || File.Exists(rval));

		Directory.CreateDirectory(rval);
		return rval;
	}


	/// <summary>Deletes old temporary directories and files in <see cref="PathOp.TempRoot"/>.</summary>
	/// <param name="maxAge">Age from which entries are deleted; NULL for one hour. The age is measured from the last write
	///                      time, because creation times are not available on every platform.</param>
	/// <remarks>Entries that cannot be deleted (e.g. because they are in use) are skipped.</remarks>
	public static void CleanupTemporaryPaths(TimeSpan? maxAge = null)
	{
		DateTime limit = (DateTime.UtcNow - (maxAge ?? TimeSpan.FromHours(1)));

		foreach(string i in Directory.GetDirectories(PathOp.TempRoot))
		{
			try
			{
				if(Directory.GetLastWriteTimeUtc(i) < limit) { Directory.Delete(i, true); }
			}
			catch(Exception) {}
		}

		foreach(string i in Directory.GetFiles(PathOp.TempRoot))
		{
			try
			{
				if(File.GetLastWriteTimeUtc(i) < limit) { File.Delete(i); }
			}
			catch(Exception) {}
		}
	}


	/// <summary>Returns if a file name is valid on this platform.</summary>
	/// <param name="fileName">File name; may include a directory.</param>
	/// <returns>Returns TRUE if the name is valid, otherwise returns FALSE.</returns>
	/// <remarks>The path must not contain invalid characters, and the file name must not contain invalid file name characters.
	///          On Windows, reserved device names (CON, NUL, COM1, …) and names that end with a dot or a space are invalid as
	///          well. The method does not check if the directory exists or can be written.</remarks>
	public static bool ValidFileName(string? fileName)
	{
		if(string.IsNullOrWhiteSpace(fileName)) { return false; }
		if(fileName.AsSpan().IndexOfAny(Path.GetInvalidPathChars()) >= 0) { return false; }

		string name = Path.GetFileName(fileName);
		if((name.Length == 0) || (name.AsSpan().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)) { return false; }

		if(OperatingSystem.IsWindows())
		{
			if(name.EndsWith('.') || name.EndsWith(' ')) { return false; }

			string stem = name.Split('.')[0].TrimEnd();
			if(_RESERVED.Contains(stem, StringComparer.OrdinalIgnoreCase)) { return false; }
		}

		return true;
	}
}
