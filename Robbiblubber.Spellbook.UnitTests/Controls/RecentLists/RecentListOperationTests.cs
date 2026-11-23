namespace Robbiblubber.Spellbook.UnitTests.Controls.RecentLists;

using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the list operations of <see cref="RecentItemList"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class RecentListOperationTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the keys of entries.</summary>
	/// <param name="items">Entries.</param>
	/// <returns>Returns the keys, pinned keys with a leading asterisk.</returns>
	private static string[] _Keys(IEnumerable<RecentItem> items)
	{
		return items.Select(m => (m.Pinned ? "*" : string.Empty) + m.Key).ToArray();
	}


	/// <summary>Creates a list.</summary>
	/// <param name="keys">Keys, pinned keys with a leading asterisk.</param>
	/// <returns>Returns the entries.</returns>
	private static List<RecentItem> _List(params string[] keys)
	{
		return keys.Select(m => new RecentItem(m.TrimStart('*'), null, m.StartsWith('*'))).ToList();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>New entries are added on top of the unpinned entries.</summary>
	[Test]
	public void AddsOnTopOfUnpinned()
	{
		List<RecentItem> items = _List("*p", "a", "b");

		RecentItemList._Add(items, "c", null, DateTime.UtcNow, 10, StringComparer.Ordinal);

		Assert.That(_Keys(items), Is.EqualTo(new string[] { "*p", "c", "a", "b" }));
	}


	/// <summary>Existing entries move to the top and keep their text if no text is given.</summary>
	[Test]
	public void MovesExistingEntry()
	{
		List<RecentItem> items = new() { new RecentItem("a"), new RecentItem("b", "Bee") };

		RecentItemList._Add(items, "B", null, DateTime.UtcNow, 10, StringComparer.OrdinalIgnoreCase);

		Assert.That(_Keys(items), Is.EqualTo(new string[] { "B", "a" }));
		Assert.That(items[0].Text, Is.EqualTo("Bee"));
	}


	/// <summary>Pinned entries keep their position when they are used again.</summary>
	[Test]
	public void PinnedEntryKeepsPosition()
	{
		List<RecentItem> items = _List("*p", "*q", "a");
		DateTime now = new(2026, 10, 16, 8, 0, 0, DateTimeKind.Utc);

		RecentItemList._Add(items, "q", "Queue", now, 10, StringComparer.Ordinal);

		Assert.That(_Keys(items), Is.EqualTo(new string[] { "*p", "*q", "a" }));
		Assert.That(items[1].Text, Is.EqualTo("Queue"));
		Assert.That(items[1].LastUsed, Is.EqualTo(now));
	}


	/// <summary>Only unpinned entries count towards the maximum.</summary>
	[Test]
	public void TrimsUnpinnedEntries()
	{
		List<RecentItem> items = _List("*p", "*q", "a", "b");

		RecentItemList._Add(items, "c", null, DateTime.UtcNow, 2, StringComparer.Ordinal);

		Assert.That(_Keys(items), Is.EqualTo(new string[] { "*p", "*q", "c", "a" }));
	}


	/// <summary>Pinning moves an entry to the end of the pinned entries.</summary>
	[Test]
	public void PinsAtEndOfPinned()
	{
		List<RecentItem> items = _List("*p", "a", "b");

		Assert.That(RecentItemList._Pin(items, "b", StringComparer.Ordinal), Is.True);
		Assert.That(_Keys(items), Is.EqualTo(new string[] { "*p", "*b", "a" }));

		Assert.That(RecentItemList._Pin(items, "b", StringComparer.Ordinal), Is.False);
		Assert.That(RecentItemList._Pin(items, "x", StringComparer.Ordinal), Is.False);
	}


	/// <summary>Unpinning moves an entry to the top of the unpinned entries and trims the list.</summary>
	[Test]
	public void UnpinsOnTopOfUnpinned()
	{
		List<RecentItem> items = _List("*p", "*q", "a", "b");

		Assert.That(RecentItemList._Unpin(items, "p", 2, StringComparer.Ordinal), Is.True);
		Assert.That(_Keys(items), Is.EqualTo(new string[] { "*q", "p", "a" }));

		Assert.That(RecentItemList._Unpin(items, "a", 2, StringComparer.Ordinal), Is.False);
	}


	/// <summary>Loaded lists are ordered with pinned entries first, keeping the relative order.</summary>
	[Test]
	public void OrdersPinnedFirst()
	{
		Assert.That(_Keys(RecentItemList._Order(_List("a", "*p", "b", "*q"))), Is.EqualTo(new string[] { "*p", "*q", "a", "b" }));
	}


	/// <summary>Entries need a key.</summary>
	[Test]
	public void ItemNeedsKey()
	{
		Assert.Throws<ArgumentException>(() => new RecentItem(" "));
		Assert.That(new RecentItem("k", string.Empty).Text, Is.Null);
		Assert.That(new RecentItem("k", "Text").ToString(), Is.EqualTo("Text"));
	}
}
