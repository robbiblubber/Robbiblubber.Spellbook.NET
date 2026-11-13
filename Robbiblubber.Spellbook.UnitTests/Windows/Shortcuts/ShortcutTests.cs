namespace Robbiblubber.Spellbook.UnitTests.Windows.Shortcuts;

using System.Runtime.Versioning;

using NUnit.Framework;
using Robbiblubber.Spellbook.Windows;



/// <summary>This class tests shortcuts.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class ShortcutTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Directory = "";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a target file that exists on every Windows system.</summary>
	private static string _Notepad
	{
		get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe"); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the test directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Directory = Path.Combine(Path.GetTempPath(), "Robbiblubber.Spellbook.UnitTests.Windows", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Directory);
	}


	/// <summary>Deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		try { Directory.Delete(_Directory, true); } catch(IOException) {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Invalid arguments are rejected.</summary>
	[Test]
	public void InvalidArguments()
	{
		Assert.Throws<ArgumentException>(() => new ShortcutInfo(" "));
		Assert.Throws<ArgumentException>(() => Shortcut.Create("", _Notepad));
		Assert.Throws<ArgumentNullException>(() => Shortcut.Create("x.lnk", (ShortcutInfo) null!));
		Assert.Throws<ArgumentException>(() => Shortcut.Read(""));
	}


	/// <summary>A shortcut with all properties can be read back.</summary>
	[Test][Platform("Win")]
	public void CreateAndRead()
	{
		string lnk = Path.Combine(_Directory, "Notepad äöü.lnk");
		Shortcut.Create(lnk, new ShortcutInfo(_Notepad)
		{
			Arguments = "\"C:\\Temp\\Datei mit ß.txt\"",
			WorkingDirectory = _Directory,
			Description = "Notepad – Test",
			IconPath = @"C:\Windows\System32\shell32.dll",
			IconIndex = 3,
			WindowStyle = ShortcutWindowStyle.MAXIMIZED,
			RunAsAdministrator = true
		});

		ShortcutInfo info = Shortcut.Read(lnk);
		Assert.That(info.TargetPath, Is.EqualTo(_Notepad).IgnoreCase);
		Assert.That(info.Arguments, Is.EqualTo("\"C:\\Temp\\Datei mit ß.txt\""));
		Assert.That(info.WorkingDirectory, Is.EqualTo(_Directory).IgnoreCase);
		Assert.That(info.Description, Is.EqualTo("Notepad – Test"));
		Assert.That(info.IconPath, Is.EqualTo(@"C:\Windows\System32\shell32.dll").IgnoreCase);
		Assert.That(info.IconIndex, Is.EqualTo(3));
		Assert.That(info.WindowStyle, Is.EqualTo(ShortcutWindowStyle.MAXIMIZED));
		Assert.That(info.RunAsAdministrator, Is.True);
	}


	/// <summary>Defaults: working directory is the target directory, no icon, normal window.</summary>
	[Test][Platform("Win")]
	public void Defaults()
	{
		string lnk = Path.Combine(_Directory, "n.lnk");
		Shortcut.Create(lnk, _Notepad);

		ShortcutInfo info = Shortcut.Read(lnk);
		Assert.That(info.Arguments, Is.Null);
		Assert.That(info.Description, Is.Null);
		Assert.That(info.IconPath, Is.Null);
		Assert.That(info.WorkingDirectory, Is.EqualTo(Path.GetDirectoryName(_Notepad)).IgnoreCase);
		Assert.That(info.WindowStyle, Is.EqualTo(ShortcutWindowStyle.NORMAL));
		Assert.That(info.RunAsAdministrator, Is.False);
	}


	/// <summary>An existing shortcut is replaced; no temporary files remain; missing directories are created.</summary>
	[Test][Platform("Win")]
	public void ReplaceAndCreateDirectory()
	{
		string lnk = Path.Combine(_Directory, "sub", "dir", "n.lnk");
		Shortcut.Create(lnk, _Notepad, "a.txt");
		Shortcut.Create(lnk, _Notepad, "b.txt");

		Assert.That(Shortcut.Read(lnk).Arguments, Is.EqualTo("b.txt"));
		Assert.That(Directory.GetFiles(Path.GetDirectoryName(lnk)!), Has.Length.EqualTo(1));
	}


	/// <summary>Reading a missing file throws a FileNotFoundException.</summary>
	[Test][Platform("Win")]
	public void ReadMissing()
	{
		Assert.Throws<FileNotFoundException>(() => Shortcut.Read(Path.Combine(_Directory, "missing.lnk")));
	}
}
