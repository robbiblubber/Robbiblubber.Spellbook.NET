namespace Robbiblubber.Spellbook.UnitTests.Windows.AppRegistration;

using System.Runtime.Versioning;

using Microsoft.Win32;
using NUnit.Framework;
using Robbiblubber.Spellbook.Windows;



/// <summary>This class tests application registration (current user only).</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ApplicationRegistrationTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test key.</summary>
	private const string _KEY = "Robbiblubber.Spellbook.UnitTest";

	/// <summary>Uninstall key path.</summary>
	private const string _PATH = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + _KEY;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the test key before each test.</summary>
	[SetUp]
	public void SetUp()
	{
		if(OperatingSystem.IsWindows()) { Registry.CurrentUser.DeleteSubKeyTree(_PATH, false); }
	}


	/// <summary>Removes the test key after each test.</summary>
	[TearDown]
	public void TearDown()
	{
		if(OperatingSystem.IsWindows()) { Registry.CurrentUser.DeleteSubKeyTree(_PATH, false); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Invalid keys are rejected (old bug: an empty key deleted the whole uninstall key).</summary>
	[Test]
	public void Validation()
	{
		foreach(string i in new[] { "", " ", " a", "a ", @"a\b", @"..\x" })
		{
			Assert.Throws<ArgumentException>(() => ApplicationRegistration.Unregister(i), i);
			Assert.Throws<ArgumentException>(() => new InstalledApplication(i, "Name", new Version(1, 0)), i);
		}
		Assert.Throws<ArgumentException>(() => new InstalledApplication(_KEY, "", new Version(1, 0)));
		Assert.Throws<ArgumentNullException>(() => new InstalledApplication(_KEY, "Name", null!));
	}


	/// <summary>Version strings omit missing parts and a revision of 0.</summary>
	[Test]
	public void VersionString()
	{
		Assert.That(new InstalledApplication(_KEY, "Name", new Version(1, 2)).ToString(), Is.EqualTo("Name 1.2"));
		Assert.That(new InstalledApplication(_KEY, "Name", new Version(1, 2, 3)).ToString(), Is.EqualTo("Name 1.2.3"));
		Assert.That(new InstalledApplication(_KEY, "Name", new Version(1, 2, 3, 0)).ToString(), Is.EqualTo("Name 1.2.3"));
		Assert.That(new InstalledApplication(_KEY, "Name", new Version(1, 2, 3, 4)) { Publisher = "P" }.ToString(), Is.EqualTo("Name 1.2.3.4 (P)"));
	}


	/// <summary>A registration can be read back with all values.</summary>
	[Test][Platform("Win")]
	public void RegisterAndGet()
	{
		ApplicationRegistration.Register(new InstalledApplication(_KEY, "Spellbook Test", new Version(4, 1, 2))
		{
			Publisher = "robbiblubber.org",
			InstallLocation = @"C:\Test",
			DisplayIcon = @"C:\Test\test.exe,0",
			UninstallCommand = @"""C:\Test\uninstall.exe""",
			QuietUninstallCommand = @"""C:\Test\uninstall.exe"" /quiet",
			EstimatedSize = 5 * 1024 * 1024,
			InstallDate = new DateTime(2026, 10, 20),
			HelpUrl = "https://robbiblubber.org/help",
			AboutUrl = "https://robbiblubber.org/"
		});

		Assert.That(ApplicationRegistration.IsRegistered(_KEY), Is.True);

		InstalledApplication app = ApplicationRegistration.Get(_KEY)!;
		Assert.That(app.DisplayName, Is.EqualTo("Spellbook Test"));
		Assert.That(app.Version, Is.EqualTo(new Version(4, 1, 2)));
		Assert.That(app.Publisher, Is.EqualTo("robbiblubber.org"));
		Assert.That(app.InstallLocation, Is.EqualTo(@"C:\Test"));
		Assert.That(app.DisplayIcon, Is.EqualTo(@"C:\Test\test.exe,0"));
		Assert.That(app.UninstallCommand, Is.EqualTo(@"""C:\Test\uninstall.exe"""));
		Assert.That(app.QuietUninstallCommand, Is.EqualTo(@"""C:\Test\uninstall.exe"" /quiet"));
		Assert.That(app.ModifyCommand, Is.Null);
		Assert.That(app.EstimatedSize, Is.EqualTo(5 * 1024 * 1024));
		Assert.That(app.InstallDate, Is.EqualTo(new DateTime(2026, 10, 20)));
		Assert.That(app.HelpUrl, Is.EqualTo("https://robbiblubber.org/help"));
		Assert.That(app.AboutUrl, Is.EqualTo("https://robbiblubber.org/"));

		using RegistryKey k = Registry.CurrentUser.OpenSubKey(_PATH)!;
		Assert.That(k.GetValue("VersionMajor"), Is.EqualTo(4));
		Assert.That(k.GetValue("VersionMinor"), Is.EqualTo(1));
		Assert.That(k.GetValue("NoModify"), Is.EqualTo(1));
		Assert.That(k.GetValue("NoRepair"), Is.EqualTo(1));
		Assert.That(k.GetValue("InstallDate"), Is.EqualTo("20261020"));
	}


	/// <summary>With a modify command, NoModify is not set; registering again replaces all values.</summary>
	[Test][Platform("Win")]
	public void RegisterReplaces()
	{
		ApplicationRegistration.Register(new InstalledApplication(_KEY, "A", new Version(1, 0)) { Publisher = "P", ModifyCommand = "modify.exe" });
		using(RegistryKey k = Registry.CurrentUser.OpenSubKey(_PATH)!) { Assert.That(k.GetValue("NoModify"), Is.Null); }

		ApplicationRegistration.Register(new InstalledApplication(_KEY, "B", new Version(2, 0)));

		InstalledApplication app = ApplicationRegistration.Get(_KEY)!;
		Assert.That(app.DisplayName, Is.EqualTo("B"));
		Assert.That(app.Version, Is.EqualTo(new Version(2, 0)));
		Assert.That(app.Publisher, Is.Null);
		Assert.That(app.ModifyCommand, Is.Null);
		Assert.That(app.InstallDate, Is.EqualTo(DateTime.Today));
	}


	/// <summary>Unregistering removes the entry and returns FALSE when there is nothing to remove.</summary>
	[Test][Platform("Win")]
	public void Unregister()
	{
		ApplicationRegistration.Register(new InstalledApplication(_KEY, "A", new Version(1, 0)));

		Assert.That(ApplicationRegistration.Unregister(_KEY), Is.True);
		Assert.That(ApplicationRegistration.IsRegistered(_KEY), Is.False);
		Assert.That(ApplicationRegistration.Get(_KEY), Is.Null);
		Assert.That(ApplicationRegistration.Unregister(_KEY), Is.False);
	}
}
