namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="AutoCompletionUsageHistory"/> class.</summary>
[TestFixture]
public class AutoCompletionUsageHistoryTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Counts start at 0 and are incremented per scope and key.</summary>
	[Test]
	public void IncrementCountsPerScopeAndKey()
	{
		AutoCompletionUsageHistory history = new();

		history.Increment("a", "x");
		history.Increment("a", "x");
		history.Increment("b", "x");

		Assert.That(history.GetCount("a", "x"), Is.EqualTo(2));
		Assert.That(history.GetCount("b", "x"), Is.EqualTo(1));
		Assert.That(history.GetCount("a", "y"), Is.EqualTo(0));
		Assert.That(history.GetCount("c", "x"), Is.EqualTo(0));
	}


	/// <summary>Keys and scopes are case-sensitive.</summary>
	[Test]
	public void KeysAreCaseSensitive()
	{
		AutoCompletionUsageHistory history = new();

		history.Increment("a", "Name");

		Assert.That(history.GetCount("a", "name"), Is.EqualTo(0));
		Assert.That(history.GetCount("A", "Name"), Is.EqualTo(0));
	}


	/// <summary>Clear removes all entries, Clear with a scope only the entries of that scope.</summary>
	[Test]
	public void ClearRemovesEntries()
	{
		AutoCompletionUsageHistory history = new();

		history.Increment("a", "x");
		history.Increment("b", "x");

		history.Clear("a");
		Assert.That(history.GetCount("a", "x"), Is.EqualTo(0));
		Assert.That(history.GetCount("b", "x"), Is.EqualTo(1));

		history.Clear();
		Assert.That(history.GetCount("b", "x"), Is.EqualTo(0));
		Assert.That(history.Export(), Is.Empty);
	}


	/// <summary>The Changed event is raised for every change, but not for clearing a scope that does not exist.</summary>
	[Test]
	public void ChangedIsRaisedOnChanges()
	{
		AutoCompletionUsageHistory history = new();
		int changes = 0;

		history.Changed += (_, __) => { changes++; };

		history.Increment("a", "x");
		Assert.That(changes, Is.EqualTo(1));

		history.Clear("unknown");
		Assert.That(changes, Is.EqualTo(1));

		history.Clear("a");
		Assert.That(changes, Is.EqualTo(2));

		history.Import("a\tx\t1");
		Assert.That(changes, Is.EqualTo(3));

		history.Clear();
		Assert.That(changes, Is.EqualTo(4));
	}


	/// <summary>When a scope exceeds the maximum number of entries, the least used entries are removed.</summary>
	[Test]
	public void LeastUsedEntriesAreRemoved()
	{
		AutoCompletionUsageHistory history = new() { MaximumEntries = 3 };

		for(int i = 0; i < 3; i++) { history.Increment("a", "often"); }
		history.Increment("a", "rare");
		history.Increment("a", "medium");
		history.Increment("a", "medium");
		history.Increment("a", "new");

		Assert.That(history.GetCount("a", "rare"), Is.EqualTo(0));
		Assert.That(history.GetCount("a", "often"), Is.EqualTo(3));
		Assert.That(history.GetCount("a", "medium"), Is.EqualTo(2));
		Assert.That(history.GetCount("a", "new"), Is.EqualTo(1));
	}


	/// <summary>A newly used item enters a full history, even if all other entries have been used more often.</summary>
	[Test]
	public void NewEntryEntersFullHistory()
	{
		AutoCompletionUsageHistory history = new() { MaximumEntries = 2 };

		for(int i = 0; i < 5; i++)
		{
			history.Increment("a", "x");
			history.Increment("a", "y");
		}
		history.Increment("a", "z");

		Assert.That(history.GetCount("a", "z"), Is.EqualTo(1));
		Assert.That(history.GetCount("a", "x") + history.GetCount("a", "y"), Is.EqualTo(5));
	}


	/// <summary>Reducing the maximum number of entries trims existing scopes; values below 1 are rejected.</summary>
	[Test]
	public void MaximumEntriesTrimsAndValidates()
	{
		AutoCompletionUsageHistory history = new();

		history.Increment("a", "x");
		history.Increment("a", "x");
		history.Increment("a", "y");

		history.MaximumEntries = 1;

		Assert.That(history.GetCount("a", "x"), Is.EqualTo(2));
		Assert.That(history.GetCount("a", "y"), Is.EqualTo(0));
		Assert.That(() => { history.MaximumEntries = 0; }, Throws.TypeOf<ArgumentOutOfRangeException>());
	}


	/// <summary>Export and import restore the history, including special characters in scopes and keys.</summary>
	[Test]
	public void ExportAndImportRoundTrip()
	{
		AutoCompletionUsageHistory history = new();
		AutoCompletionUsageHistory restored = new();

		history.Increment("Scope\tWith\tTabs", "Key\r\nWith\\Breaks");
		history.Increment("Scope\tWith\tTabs", "Key\r\nWith\\Breaks");
		history.Increment("plain", "Name");

		restored.Import(history.Export());

		Assert.That(restored.GetCount("Scope\tWith\tTabs", "Key\r\nWith\\Breaks"), Is.EqualTo(2));
		Assert.That(restored.GetCount("plain", "Name"), Is.EqualTo(1));
		Assert.That(restored.Export(), Is.EqualTo(history.Export()));
	}


	/// <summary>Import replaces the history unless merging is requested; merged counts are added.</summary>
	[Test]
	public void ImportReplacesOrMerges()
	{
		AutoCompletionUsageHistory history = new();

		history.Increment("a", "x");
		history.Increment("a", "old");

		history.Import("a\tx\t2", true);
		Assert.That(history.GetCount("a", "x"), Is.EqualTo(3));
		Assert.That(history.GetCount("a", "old"), Is.EqualTo(1));

		history.Import("a\tx\t2");
		Assert.That(history.GetCount("a", "x"), Is.EqualTo(2));
		Assert.That(history.GetCount("a", "old"), Is.EqualTo(0));
	}


	/// <summary>Invalid lines are ignored; Windows line breaks are accepted.</summary>
	[Test]
	public void ImportIgnoresInvalidLines()
	{
		AutoCompletionUsageHistory history = new();

		history.Import("a\tvalid\t4\r\n" + "a\tmissing\r\n" + "a\tnumber\tabc\r\n" + "a\tzero\t0\r\n" + "a\tnegative\t-3\r\n" + "a\textra\t1\t2\r\n" + "\r\n");

		Assert.That(history.GetCount("a", "valid"), Is.EqualTo(4));
		Assert.That(history.Export(), Is.EqualTo("a\tvalid\t4\n"));
	}


	/// <summary>Merged counts do not overflow.</summary>
	[Test]
	public void MergedCountsDoNotOverflow()
	{
		AutoCompletionUsageHistory history = new();

		history.Import($"a\tx\t{int.MaxValue}");
		history.Import("a\tx\t10", true);

		Assert.That(history.GetCount("a", "x"), Is.EqualTo(int.MaxValue));
	}
}
