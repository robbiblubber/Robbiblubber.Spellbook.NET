namespace Robbiblubber.Spellbook.UnitTests.Windows.FileAssociations;

using System.Runtime.Versioning;

using Microsoft.Win32;
using NUnit.Framework;
using Robbiblubber.Spellbook.Windows;



/// <summary>This class tests file associations (current user only).</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class FileAssociationTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test extension.</summary>
	private const string _EXT = ".spellbooktest";

	/// <summary>Test program ID.</summary>
	private const string _PROG_ID = "Robbiblubber.Spellbook.UnitTest.Document";

	/// <summary>Program ID of another application.</summary>
	private const string _OTHER_ID = "Robbiblubber.Spellbook.UnitTest.Other";

	/// <summary>Test application name.</summary>
	private const string _APP_NAME = "Robbiblubber.Spellbook.UnitTest.App";

	/// <summary>Application path.</summary>
	private const string _APP = @"C:\Program Files\Spellbook Test\test.exe";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Opens a key under HKCU\Software\Classes.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns the key, or NULL if it does not exist.</returns>
	private static RegistryKey? _Open(string path)
	{
		return Registry.CurrentUser.OpenSubKey(@"Software\Classes\" + path);
	}


	/// <summary>Removes all test keys.</summary>
	private static void _Clean()
	{
		foreach(string i in new[] { _EXT, _PROG_ID, _OTHER_ID })
		{
			Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\" + i, false);
		}
		Registry.CurrentUser.DeleteSubKeyTree(@"Software\" + _APP_NAME, false);
		using RegistryKey? apps = Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications", true);
		apps?.DeleteValue(_APP_NAME, false);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the test keys before each test.</summary>
	[SetUp]
	public void SetUp()
	{
		if(OperatingSystem.IsWindows()) { _Clean(); }
	}


	/// <summary>Removes the test keys after each test.</summary>
	[TearDown]
	public void TearDown()
	{
		if(OperatingSystem.IsWindows()) { _Clean(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Extensions are normalized; invalid extensions and program IDs are rejected (old bug: an empty program ID deleted
	///          HKCU\Software\Classes).</summary>
	[Test]
	public void Validation()
	{
		Assert.That(new FileAssociation.FileType("txt2", "A.B", _APP).Extension, Is.EqualTo(".txt2"));
		Assert.That(new FileAssociation.FileType(".txt2", "A.B", _APP).Extension, Is.EqualTo(".txt2"));

		foreach(string i in new[] { "", " ", ".", "..x", ".a b", @".a\b", ".a.b", ".a*" })
		{
			Assert.Throws<ArgumentException>(() => new FileAssociation.FileType(i, "A.B", _APP), i);
		}
		foreach(string i in new[] { "", " ", " A", @"A\B", ".A" })
		{
			Assert.Throws<ArgumentException>(() => new FileAssociation.FileType(".x", i, _APP), i);
			Assert.Throws<ArgumentException>(() => FileAssociation.Unregister(".x", i), i);
		}
		Assert.Throws<ArgumentException>(() => FileAssociation.Unregister("", "A.B"));
		Assert.Throws<ArgumentException>(() => new FileAssociation.FileType(".x", "A.B", ""));
		Assert.Throws<ArgumentException>(() => new FileAssociation.Verb(""));
		Assert.Throws<ArgumentException>(() => new FileAssociation.ApplicationCapabilities("", "Name"));
		Assert.Throws<ArgumentException>(() => new FileAssociation.ApplicationCapabilities(@"a\b", "Name"));
		Assert.Throws<ArgumentException>(() => new FileAssociation.ApplicationCapabilities("A.B", " "));
		Assert.Throws<ArgumentException>(() => FileAssociation.UnregisterApplication(""));
	}


	/// <summary>Registering writes program ID, icon, verbs, default and open with list.</summary>
	[Test][Platform("Win")]
	public void Register()
	{
		FileAssociation.Register(new FileAssociation.FileType("spellbooktest", _PROG_ID, _APP)
		{
			Description = "Spellbook Test Document",
			Verbs = [new FileAssociation.Verb("print") { Arguments = "/p \"%1\"" }, new FileAssociation.Verb("convert") { Text = "Convert", ApplicationPath = @"C:\conv.exe" }]
		});

		using(RegistryKey prog = _Open(_PROG_ID)!)
		{
			Assert.That(prog.GetValue(""), Is.EqualTo("Spellbook Test Document"));
			Assert.That(prog.OpenSubKey("DefaultIcon")!.GetValue(""), Is.EqualTo($"\"{_APP}\",0"));
			Assert.That(prog.OpenSubKey(@"shell\open\command")!.GetValue(""), Is.EqualTo($"\"{_APP}\" \"%1\""));
			Assert.That(prog.OpenSubKey(@"shell\print\command")!.GetValue(""), Is.EqualTo($"\"{_APP}\" /p \"%1\""));
			Assert.That(prog.OpenSubKey(@"shell\convert")!.GetValue(""), Is.EqualTo("Convert"));
			Assert.That(prog.OpenSubKey(@"shell\convert\command")!.GetValue(""), Is.EqualTo("\"C:\\conv.exe\" \"%1\""));
		}

		using(RegistryKey ext = _Open(_EXT)!)
		{
			Assert.That(ext.GetValue(""), Is.EqualTo(_PROG_ID));
			Assert.That(ext.OpenSubKey("OpenWithProgids")!.GetValueNames(), Does.Contain(_PROG_ID));
		}

		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.True);
		Assert.That(FileAssociation.GetProgId(_EXT, RegistrationScope.CURRENT_USER), Is.EqualTo(_PROG_ID));
		Assert.That(FileAssociation.GetProgId(_EXT), Is.EqualTo(_PROG_ID));
	}


	/// <summary>Registering again replaces the program ID completely.</summary>
	[Test][Platform("Win")]
	public void RegisterReplaces()
	{
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP) { Verbs = [new FileAssociation.Verb("print")] });
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP));

		using RegistryKey prog = _Open(_PROG_ID)!;
		Assert.That(prog.OpenSubKey(@"shell\print"), Is.Null);
	}


	/// <summary>An existing default of another application is kept; the program ID is only added to the open with list.</summary>
	[Test][Platform("Win")]
	public void KeepsOtherDefault()
	{
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _OTHER_ID, _APP));
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP));

		Assert.That(FileAssociation.GetProgId(_EXT, RegistrationScope.CURRENT_USER), Is.EqualTo(_OTHER_ID));
		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.True);
	}


	/// <summary>A default that points to a program ID that no longer exists is replaced.</summary>
	[Test][Platform("Win")]
	public void ReplacesStaleDefault()
	{
		using(RegistryKey ext = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + _EXT)) { ext.SetValue("", "Robbiblubber.Spellbook.UnitTest.Gone"); }

		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP));

		Assert.That(FileAssociation.GetProgId(_EXT, RegistrationScope.CURRENT_USER), Is.EqualTo(_PROG_ID));
	}


	/// <summary>Unregistering removes only the own program ID and keeps other registrations.</summary>
	[Test][Platform("Win")]
	public void UnregisterKeepsOthers()
	{
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _OTHER_ID, _APP));
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP));

		Assert.That(FileAssociation.Unregister(_EXT, _PROG_ID), Is.True);

		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.False);
		Assert.That(FileAssociation.IsRegistered(_EXT, _OTHER_ID), Is.True);
		Assert.That(FileAssociation.GetProgId(_EXT, RegistrationScope.CURRENT_USER), Is.EqualTo(_OTHER_ID));
		using RegistryKey? prog = _Open(_PROG_ID);
		Assert.That(prog, Is.Null);
	}


	/// <summary>Unregistering the only registration removes the extension key; unregistering again returns FALSE (old bug:
	///          exception).</summary>
	[Test][Platform("Win")]
	public void UnregisterAll()
	{
		FileAssociation.Register(new FileAssociation.FileType(_EXT, _PROG_ID, _APP));

		Assert.That(FileAssociation.Unregister(_EXT, _PROG_ID), Is.True);
		using(RegistryKey? ext = _Open(_EXT)) { Assert.That(ext, Is.Null); }

		Assert.That(FileAssociation.Unregister(_EXT, _PROG_ID), Is.False);
		Assert.That(FileAssociation.GetProgId(_EXT, RegistrationScope.CURRENT_USER), Is.Null);
	}


	/// <summary>Registering an application writes its file types, capabilities and the registered applications entry.</summary>
	[Test][Platform("Win")]
	public void RegisterApplication()
	{
		FileAssociation.RegisterApplication(new FileAssociation.ApplicationCapabilities(_APP_NAME, "Spellbook Test")
		{
			Description = "Test application",
			FileTypes = [new FileAssociation.FileType(_EXT, _PROG_ID, _APP)]
		});

		Assert.That(FileAssociation.IsApplicationRegistered(_APP_NAME), Is.True);
		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.True);

		using(RegistryKey apps = Registry.CurrentUser.OpenSubKey(@"Software\RegisteredApplications")!)
		{
			Assert.That(apps.GetValue(_APP_NAME), Is.EqualTo($@"Software\{_APP_NAME}\Capabilities"));
		}

		using RegistryKey caps = Registry.CurrentUser.OpenSubKey($@"Software\{_APP_NAME}\Capabilities")!;
		Assert.That(caps.GetValue("ApplicationName"), Is.EqualTo("Spellbook Test"));
		Assert.That(caps.GetValue("ApplicationDescription"), Is.EqualTo("Test application"));
		Assert.That(caps.GetValue("ApplicationIcon"), Is.EqualTo($"\"{_APP}\",0"));
		Assert.That(caps.OpenSubKey("FileAssociations")!.GetValue(_EXT), Is.EqualTo(_PROG_ID));
	}


	/// <summary>Unregistering an application removes everything, optionally keeping the file types.</summary>
	[Test][Platform("Win")]
	public void UnregisterApplication()
	{
		FileAssociation.ApplicationCapabilities app = new(_APP_NAME, "Spellbook Test") { FileTypes = [new FileAssociation.FileType(_EXT, _PROG_ID, _APP)] };

		FileAssociation.RegisterApplication(app);
		Assert.That(FileAssociation.UnregisterApplication(_APP_NAME, fileTypes: false), Is.True);
		Assert.That(FileAssociation.IsApplicationRegistered(_APP_NAME), Is.False);
		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.True);

		FileAssociation.RegisterApplication(app);
		Assert.That(FileAssociation.UnregisterApplication(_APP_NAME), Is.True);
		Assert.That(FileAssociation.IsApplicationRegistered(_APP_NAME), Is.False);
		Assert.That(FileAssociation.IsRegistered(_EXT, _PROG_ID), Is.False);
		using(RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\" + _APP_NAME)) { Assert.That(key, Is.Null); }

		Assert.That(FileAssociation.UnregisterApplication(_APP_NAME), Is.False);
	}
}
