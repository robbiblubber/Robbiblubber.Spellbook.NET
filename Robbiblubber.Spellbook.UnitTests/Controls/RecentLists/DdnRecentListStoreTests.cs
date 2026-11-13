namespace Robbiblubber.Spellbook.UnitTests.Controls.RecentLists;

using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="DdnRecentListStore"/> class.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class DdnRecentListStoreTests
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
		_File = Path.Combine(Path.GetTempPath(), "spellbook_recent_" + Guid.NewGuid().ToString("N") + ".lists");
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

	/// <summary>Relative paths are relative to the user configuration path.</summary>
	[Test]
	public void ResolvesPaths()
	{
		Assert.That(new DdnRecentListStore(@"C:\data\my.lists").ConfigFile, Is.EqualTo(@"C:\data\my.lists"));
		Assert.That(new DdnRecentListStore().ConfigFile, Is.EqualTo(Path.Combine(PathOp.UserConfigurationPath, DdnRecentListStore.DEFAULT_FILE_NAME)));
	}


	/// <summary>Entries are saved and loaded with all their properties.</summary>
	[Test]
	public void RoundTrips()
	{
		DateTime used = new(2026, 10, 16, 7, 30, 15, DateTimeKind.Utc);
		DdnRecentListStore store = new(_File);

		store.Save("files", new RecentItem[] { new(@"C:\a.txt", null, true, used), new(@"C:\b.txt", "Bee", false, used) });

		IReadOnlyList<RecentItem> result = new DdnRecentListStore(_File).Load("files");
		Assert.That(result.Count, Is.EqualTo(2));
		Assert.That(result[0].Key, Is.EqualTo(@"C:\a.txt"));
		Assert.That(result[0].Pinned, Is.True);
		Assert.That(result[0].Text, Is.Null);
		Assert.That(result[0].LastUsed, Is.EqualTo(used));
		Assert.That(result[1].Text, Is.EqualTo("Bee"));
		Assert.That(new DdnRecentListStore(_File).Load("other"), Is.Empty);
	}


	/// <summary>Saving a shorter list does not keep old entries.</summary>
	[Test]
	public void SavesShorterList()
	{
		DdnRecentListStore store = new(_File);

		store.Save("files", new RecentItem[] { new("a"), new("b"), new("c") });
		store.Save("files", new RecentItem[] { new("x") });

		Assert.That(store.Load("files").Select(m => m.Key), Is.EqualTo(new string[] { "x" }));
	}


	/// <summary>Files of the earlier RecentFileList are read until the list is saved in the current format.</summary>
	[Test]
	public void ReadsLegacyFile()
	{
		DdnElement cfg = Ddn.Create();
		DdnRecentListStore store = new(_File);

		cfg["files/[0]"] = @"C:\old1.txt";
		cfg["files/[1]"] = @"C:\old2.txt";
		cfg["list/length"] = 2;
		cfg.Save(_File);

		Assert.That(store.Load("files").Select(m => m.Key), Is.EqualTo(new string[] { @"C:\old1.txt", @"C:\old2.txt" }));

		store.Save("files", Array.Empty<RecentItem>());
		Assert.That(store.Load("files"), Is.Empty);
	}
}
