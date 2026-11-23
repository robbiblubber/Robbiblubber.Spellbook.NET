namespace Robbiblubber.Spellbook.UnitTests.Core.Files;

using NUnit.Framework;



/// <summary>This class tests the path operations.</summary>
[TestFixture]
[NonParallelizable]
public class PathOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Dir = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a test directory and uses it as temporary root.</summary>
	[SetUp]
	public void SetUp()
	{
		_Dir = Path.Combine(Path.GetTempPath(), "spellbook-tests", Guid.NewGuid().ToString("N"));
		PathOp.TempRoot = Path.Combine(_Dir, "temp");
	}


	/// <summary>Restores the defaults and deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		PathOp.TempRoot = null!;
		PathOp.ApplicationPart = null;
		if(Directory.Exists(_Dir)) { Directory.Delete(_Dir, true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The default temporary root is in the system temporary directory and exists.</summary>
	[Test]
	public void TempRoot_Default()
	{
		PathOp.TempRoot = null!;

		Assert.That(PathOp.TempRoot, Is.EqualTo(Path.Combine(Path.GetTempPath(), "robbiblubber.org")));
		Assert.That(Directory.Exists(PathOp.TempRoot), Is.True);
	}


	/// <summary>A custom temporary root is created on access.</summary>
	[Test]
	public void TempRoot_Custom()
	{
		Assert.That(Directory.Exists(Path.Combine(_Dir, "temp")), Is.False);
		Assert.That(PathOp.TempRoot, Is.EqualTo(Path.Combine(_Dir, "temp")));
		Assert.That(Directory.Exists(Path.Combine(_Dir, "temp")), Is.True);
	}


	/// <summary>Temporary files are unique names inside the temporary root.</summary>
	[Test]
	public void GetTempFile()
	{
		string a = PathOp.GetTempFile();
		string b = PathOp.GetTempFile(".log");
		string c = PathOp.GetTempFile("txt");

		Assert.That(Path.GetDirectoryName(a), Is.EqualTo(PathOp.TempRoot));
		Assert.That(a, Does.EndWith(".tmp"));
		Assert.That(b, Does.EndWith(".log").And.Not.EndWith("..log"));
		Assert.That(c, Does.EndWith(".txt"));
		Assert.That(a, Is.Not.EqualTo(PathOp.GetTempFile()));
		Assert.That(File.Exists(a), Is.False);
	}


	/// <summary>The application part accepts both separators and builds the path with the platform separator.</summary>
	[Test]
	public void ApplicationPart()
	{
		PathOp.Initialize(@"robbiblubber.org\Tests/ Spellbook /");

		Assert.That(PathOp.ApplicationPart, Is.EqualTo("robbiblubber.org/Tests/Spellbook"));
		Assert.That(PathOp.UserConfigurationPath, Is.EqualTo(Path.Combine(PathOp.SystemUserConfigurationPath, "robbiblubber.org", "Tests", "Spellbook")));
		Assert.That(Directory.Exists(PathOp.UserConfigurationPath), Is.True);
		Assert.That(PathOp.UserSettingsFile, Is.EqualTo(Path.Combine(PathOp.UserConfigurationPath, "application.settings")));

		Directory.Delete(PathOp.UserConfigurationPath);

		PathOp.ApplicationPart = "";
		Assert.That(PathOp.ApplicationPart, Is.Null);
		Assert.That(PathOp.UserConfigurationPath, Is.EqualTo(PathOp.SystemUserConfigurationPath));
	}


	/// <summary>Setting the application part does not create directories.</summary>
	[Test]
	public void ApplicationPart_CreatesNothing()
	{
		string part = "spellbook-test-" + Guid.NewGuid().ToString("N");
		PathOp.ApplicationPart = part;

		Assert.That(Directory.Exists(Path.Combine(PathOp.SystemUserConfigurationPath, part)), Is.False);
		Assert.That(Directory.Exists(Path.Combine(PathOp.SystemLocalConfigurationPath, part)), Is.False);
	}


	/// <summary>Application directory and executable are available.</summary>
	[Test]
	public void ApplicationDirectory()
	{
		Assert.That(PathOp.ApplicationDirectory, Is.EqualTo(Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory)));
		Assert.That(Directory.Exists(PathOp.ApplicationDirectory), Is.True);
		Assert.That(File.Exists(PathOp.ApplicationExecutable), Is.True);
		Assert.That(File.Exists(Path.Combine(PathOp.LibraryDirectory, "Robbiblubber.Spellbook.dll")), Is.True);
	}
}
