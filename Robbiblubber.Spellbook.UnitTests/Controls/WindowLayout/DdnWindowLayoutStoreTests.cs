namespace Robbiblubber.Spellbook.UnitTests.Controls.WindowLayout;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="DdnWindowLayoutStore"/> class.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class DdnWindowLayoutStoreTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary configuration file.</summary>
	private string _File = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_File = Path.Combine(Path.GetTempPath(), "spellbook_layout_" + Guid.NewGuid().ToString("N") + ".layout");
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		if(File.Exists(_File)) { File.Delete(_File); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Relative paths are relative to the user configuration path; rooted paths are kept.</summary>
	[Test]
	public void ResolvesPaths()
	{
		Assert.That(_StoreUtility._GetFullPath(@"C:\data\my.layout", DdnWindowLayoutStore.DEFAULT_FILE_NAME), Is.EqualTo(@"C:\data\my.layout"));
		Assert.That(_StoreUtility._GetFullPath("my.layout", DdnWindowLayoutStore.DEFAULT_FILE_NAME), Is.EqualTo(Path.Combine(PathOp.UserConfigurationPath, "my.layout")));
		Assert.That(_StoreUtility._GetFullPath(null, DdnWindowLayoutStore.DEFAULT_FILE_NAME), Is.EqualTo(Path.Combine(PathOp.UserConfigurationPath, DdnWindowLayoutStore.DEFAULT_FILE_NAME)));
	}


	/// <summary>Values are written to the file on commit.</summary>
	[Test]
	public void WritesOnCommit()
	{
		DdnWindowLayoutStore store = new(_File);

		store.Write("Main", "window", new int[] { 1, 2, 3, 4, 96, 0 });
		Assert.That(new DdnWindowLayoutStore(_File).Read("Main", "window"), Is.Null);

		store.Commit();
		Assert.That(new DdnWindowLayoutStore(_File).Read("Main", "window"), Is.EqualTo(new int[] { 1, 2, 3, 4, 96, 0 }));
	}


	/// <summary>Commits keep values written by other stores in the meantime.</summary>
	[Test]
	public void CommitKeepsOtherLayouts()
	{
		DdnWindowLayoutStore a = new(_File);
		DdnWindowLayoutStore b = new(_File);

		Assert.That(a.Read("A", "window"), Is.Null);
		Assert.That(b.Read("B", "window"), Is.Null);

		a.Write("A", "window", new int[] { 1 });
		a.Commit();
		b.Write("B", "window", new int[] { 2 });
		b.Commit();

		DdnWindowLayoutStore check = new(_File);
		Assert.That(check.Read("A", "window"), Is.EqualTo(new int[] { 1 }));
		Assert.That(check.Read("B", "window"), Is.EqualTo(new int[] { 2 }));
	}


	/// <summary>Layouts saved by earlier versions are read as window values without DPI.</summary>
	[Test]
	public void ReadsLegacyLayout()
	{
		DdnElement cfg = Ddn.Open(_File, true);
		DdnElement win = cfg["layout"]["Old"];

		win["pos"] = new int[] { 10, 20 };
		win["size"] = new int[] { 300, 200 };
		win["state"] = FormWindowState.Maximized;
		cfg.Save(_File);

		Assert.That(new DdnWindowLayoutStore(_File).Read("Old", WindowLayoutProvider.WINDOW_KEY), Is.EqualTo(new int[] { 10, 20, 300, 200, 0, (int) FormWindowState.Maximized }));
	}
}
