namespace Robbiblubber.Spellbook.UnitTests.Core.Files;

using NUnit.Framework;



/// <summary>This class tests the file operations.</summary>
[TestFixture]
[NonParallelizable]
public class FileOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Dir = string.Empty;

	/// <summary>Current directory before the test.</summary>
	private string _Current = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a test directory and uses it as temporary root.</summary>
	[SetUp]
	public void SetUp()
	{
		_Dir = Path.Combine(Path.GetTempPath(), "spellbook-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Dir);
		PathOp.TempRoot = Path.Combine(_Dir, "temp");
		_Current = Directory.GetCurrentDirectory();
	}


	/// <summary>Restores the defaults and deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		Directory.SetCurrentDirectory(_Current);
		PathOp.TempRoot = null!;
		if(Directory.Exists(_Dir)) { Directory.Delete(_Dir, true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a file with content.</summary>
	/// <param name="parts">Path parts below the test directory.</param>
	/// <returns>Returns the file name.</returns>
	private string _File(params string[] parts)
	{
		string rval = Path.Combine([_Dir, ..parts]);
		Directory.CreateDirectory(Path.GetDirectoryName(rval)!);
		File.WriteAllText(rval, parts[^1]);

		return rval;
	}


	/// <summary>Creates a source tree.</summary>
	/// <returns>Returns the source directory.</returns>
	private string _Tree()
	{
		_File("src", "a.txt");
		_File("src", "b.log");
		_File("src", "sub", "c.txt");
		_File("src", "sub", "deep", "d.txt");

		return Path.Combine(_Dir, "src");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: copy                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Files matching a pattern are copied, recursively into the same subdirectories.</summary>
	[Test]
	public void CopyFiles()
	{
		string src = _Tree();
		string dst = Path.Combine(_Dir, "dst");

		FileOp.CopyFiles(Path.Combine(src, "*.txt"), dst);
		Assert.That(File.Exists(Path.Combine(dst, "a.txt")), Is.True);
		Assert.That(File.Exists(Path.Combine(dst, "b.log")), Is.False);
		Assert.That(Directory.Exists(Path.Combine(dst, "sub")), Is.False);

		FileOp.CopyFiles(Path.Combine(src, "*.txt"), dst, true);
		Assert.That(File.ReadAllText(Path.Combine(dst, "sub", "deep", "d.txt")), Is.EqualTo("d.txt"));
	}


	/// <summary>A pattern without directory uses the current directory.</summary>
	[Test]
	public void CopyFiles_CurrentDirectory()
	{
		Directory.SetCurrentDirectory(_Tree());
		FileOp.CopyFiles("*.txt", Path.Combine(_Dir, "dst"));

		Assert.That(File.Exists(Path.Combine(_Dir, "dst", "a.txt")), Is.True);
	}


	/// <summary>Existing files are kept when overwrite is off.</summary>
	[Test]
	public void CopyFiles_NoOverwrite()
	{
		string src = _Tree();
		string target = _File("dst", "a.txt");
		File.WriteAllText(target, "old");

		Assert.Throws<IOException>(() => FileOp.CopyFiles(Path.Combine(src, "a.txt"), Path.Combine(_Dir, "dst"), false, false));
		Assert.That(File.ReadAllText(target), Is.EqualTo("old"));

		FileOp.CopyFiles(Path.Combine(src, "a.txt"), Path.Combine(_Dir, "dst"));
		Assert.That(File.ReadAllText(target), Is.EqualTo("a.txt"));
	}


	/// <summary>TryCopyFile reports success and failure.</summary>
	[Test]
	public void TryCopyFile()
	{
		string a = _File("a.txt");

		Assert.That(FileOp.TryCopyFile(a, Path.Combine(_Dir, "b.txt")), Is.True);
		Assert.That(FileOp.TryCopyFile(a, Path.Combine(_Dir, "b.txt"), false), Is.False);
		Assert.That(FileOp.TryCopyFile(Path.Combine(_Dir, "missing.txt"), Path.Combine(_Dir, "c.txt")), Is.False);
	}


	/// <summary>Directories are copied with all subdirectories.</summary>
	[Test]
	public void CopyDirectory()
	{
		string dst = Path.Combine(_Dir, "dst");
		FileOp.CopyDirectory(_Tree(), dst);

		Assert.That(File.ReadAllText(Path.Combine(dst, "b.log")), Is.EqualTo("b.log"));
		Assert.That(File.ReadAllText(Path.Combine(dst, "sub", "deep", "d.txt")), Is.EqualTo("d.txt"));

		string flat = Path.Combine(_Dir, "flat");
		FileOp.CopyDirectory(Path.Combine(_Dir, "src"), flat, false);
		Assert.That(File.Exists(Path.Combine(flat, "a.txt")), Is.True);
		Assert.That(Directory.Exists(Path.Combine(flat, "sub")), Is.False);
	}


	/// <summary>Copying a directory into itself is rejected instead of recursing endlessly.</summary>
	[Test]
	public void CopyDirectory_IntoItself()
	{
		string src = _Tree();

		Assert.Throws<IOException>(() => FileOp.CopyDirectory(src, Path.Combine(src, "copy")));
		Assert.Throws<IOException>(() => FileOp.CopyDirectory(src, src));
		Assert.That(FileOp.TryCopyDirectory(src, Path.Combine(src, "sub", "copy")), Is.False);
		Assert.That(Directory.Exists(Path.Combine(src, "copy")), Is.False);

		FileOp.CopyDirectory(src, Path.Combine(src, "copy"), false);
		Assert.That(File.Exists(Path.Combine(src, "copy", "a.txt")), Is.True);

		Assert.DoesNotThrow(() => FileOp.CopyDirectory(src, src + "2"));
	}


	/// <summary>A missing source directory throws.</summary>
	[Test]
	public void CopyDirectory_Missing()
	{
		Assert.Throws<DirectoryNotFoundException>(() => FileOp.CopyDirectory(Path.Combine(_Dir, "missing"), Path.Combine(_Dir, "dst")));
		Assert.That(FileOp.TryCopyDirectory(Path.Combine(_Dir, "missing"), Path.Combine(_Dir, "dst")), Is.False);
	}


	/// <summary>TryCopyDirectory copies everything it can after an error.</summary>
	[Test]
	public void TryCopyDirectory_ContinuesAfterError()
	{
		string src = _Tree();
		_File("src", "zz", "e.txt");
		string dst = Path.Combine(_Dir, "dst");

		// a file where the first subdirectory should be makes it fail
		Directory.CreateDirectory(dst);
		File.WriteAllText(Path.Combine(dst, "sub"), "blocker");

		Assert.That(FileOp.TryCopyDirectory(src, dst), Is.False);
		Assert.That(File.Exists(Path.Combine(dst, "a.txt")), Is.True);
		Assert.That(File.Exists(Path.Combine(dst, "zz", "e.txt")), Is.True);

		File.Delete(Path.Combine(dst, "sub"));
		Assert.That(FileOp.TryCopyDirectory(src, dst), Is.True);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: delete                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Files matching a pattern are deleted, recursively on request.</summary>
	[Test]
	public void Delete()
	{
		string src = _Tree();

		FileOp.Delete(Path.Combine(src, "*.txt"));
		Assert.That(File.Exists(Path.Combine(src, "a.txt")), Is.False);
		Assert.That(File.Exists(Path.Combine(src, "b.log")), Is.True);
		Assert.That(File.Exists(Path.Combine(src, "sub", "c.txt")), Is.True);

		FileOp.Delete(Path.Combine(src, "*.txt"), true);
		Assert.That(File.Exists(Path.Combine(src, "sub", "deep", "d.txt")), Is.False);
	}


	/// <summary>A pattern without directory uses the current directory.</summary>
	[Test]
	public void Delete_CurrentDirectory()
	{
		Directory.SetCurrentDirectory(_Tree());
		FileOp.Delete("*.log");

		Assert.That(File.Exists(Path.Combine(_Dir, "src", "b.log")), Is.False);
		Assert.That(File.Exists(Path.Combine(_Dir, "src", "a.txt")), Is.True);
	}


	/// <summary>A missing directory throws unless errors are suppressed.</summary>
	[Test]
	public void Delete_Missing()
	{
		Assert.Throws<DirectoryNotFoundException>(() => FileOp.Delete(Path.Combine(_Dir, "missing", "*.txt")));
		Assert.DoesNotThrow(() => FileOp.Delete(Path.Combine(_Dir, "missing", "*.txt"), false, false));
	}


	/// <summary>A directory is emptied except for the entries to keep.</summary>
	[Test]
	public void PurgeDirectory()
	{
		string src = _Tree();

		Assert.That(FileOp.PurgeDirectory(src, "*.LOG", "sub"), Is.Empty);

		Assert.That(Directory.GetFileSystemEntries(src).Select(Path.GetFileName).Order(), Is.EqualTo(OperatingSystem.IsLinux() ? new[] { "sub" } : ["b.log", "sub"]));
		Assert.That(File.Exists(Path.Combine(src, "sub", "deep", "d.txt")), Is.True);

		FileOp.PurgeDirectory(src);
		Assert.That(Directory.GetFileSystemEntries(src), Is.Empty);
	}


	/// <summary>A missing directory is ignored only when errors are ignored.</summary>
	[Test]
	public void PurgeDirectory_Missing()
	{
		Assert.That(FileOp.PurgeDirectory(Path.Combine(_Dir, "missing"), true), Is.Empty);
		Assert.Throws<DirectoryNotFoundException>(() => FileOp.PurgeDirectory(Path.Combine(_Dir, "missing")));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: temporary paths                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary directories are created inside the temporary root.</summary>
	[Test]
	public void CreateTempPath()
	{
		string a = FileOp.CreateTempPath();
		string b = FileOp.CreateTempPath();

		Assert.That(Path.GetDirectoryName(a), Is.EqualTo(PathOp.TempRoot));
		Assert.That(Directory.Exists(a), Is.True);
		Assert.That(a, Is.Not.EqualTo(b));
	}


	/// <summary>Old temporary directories are deleted including their content; new ones are kept.</summary>
	[Test]
	public void CleanupTemporaryPaths()
	{
		string old = FileOp.CreateTempPath();
		File.WriteAllText(Path.Combine(old, "x.txt"), "x");
		Directory.CreateDirectory(Path.Combine(old, "inner"));
		Directory.SetLastWriteTimeUtc(old, DateTime.UtcNow.AddHours(-2));

		string oldFile = PathOp.GetTempFile();
		File.WriteAllText(oldFile, "x");
		File.SetLastWriteTimeUtc(oldFile, DateTime.UtcNow.AddHours(-2));

		string fresh = FileOp.CreateTempPath();
		File.WriteAllText(Path.Combine(fresh, "y.txt"), "y");

		FileOp.CleanupTemporaryPaths();

		Assert.That(Directory.Exists(old), Is.False);
		Assert.That(File.Exists(oldFile), Is.False);
		Assert.That(Directory.Exists(fresh), Is.True);

		FileOp.CleanupTemporaryPaths(TimeSpan.Zero);
		Assert.That(Directory.Exists(fresh), Is.False);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: file names                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>File names are validated for the current platform.</summary>
	[Test]
	public void ValidFileName()
	{
		Assert.That(FileOp.ValidFileName("a.txt"), Is.True);
		Assert.That(FileOp.ValidFileName(Path.Combine(_Dir, "a.txt")), Is.True);
		Assert.That(FileOp.ValidFileName(null), Is.False);
		Assert.That(FileOp.ValidFileName(" "), Is.False);
		Assert.That(FileOp.ValidFileName("a\0b"), Is.False);
		Assert.That(FileOp.ValidFileName(_Dir + Path.DirectorySeparatorChar), Is.False);

		if(OperatingSystem.IsWindows())
		{
			Assert.That(FileOp.ValidFileName("a<b>?*|:.txt"), Is.False);
			Assert.That(FileOp.ValidFileName("CON"), Is.False);
			Assert.That(FileOp.ValidFileName("nul.txt"), Is.False);
			Assert.That(FileOp.ValidFileName("COM1 .log"), Is.False);
			Assert.That(FileOp.ValidFileName("a."), Is.False);
			Assert.That(FileOp.ValidFileName("a "), Is.False);
			Assert.That(FileOp.ValidFileName("CONSOLE.txt"), Is.True);
		}
		else
		{
			Assert.That(FileOp.ValidFileName("a<b>?*|:.txt"), Is.True);
			Assert.That(FileOp.ValidFileName("CON"), Is.True);
		}
	}
}
