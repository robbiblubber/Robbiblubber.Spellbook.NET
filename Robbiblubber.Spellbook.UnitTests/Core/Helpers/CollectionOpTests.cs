namespace Robbiblubber.Spellbook.UnitTests.Core.Helpers;

using NUnit.Framework;



/// <summary>This class tests the collection methods.</summary>
[TestFixture]
public class CollectionOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A random element is returned; empty collections throw or return the default.</summary>
	[Test]
	public void Random_Single()
	{
		int[] data = [1, 2, 3];

		Assert.That(data, Does.Contain(data.Random()));
		Assert.That(data.Where(m => m > 1), Does.Contain(data.Where(m => m > 1).Random()));
		Assert.Throws<InvalidOperationException>(() => Array.Empty<int>().Random());
		Assert.That(Array.Empty<int>().RandomOrDefault(), Is.EqualTo(0));
		Assert.That(Array.Empty<string>().RandomOrDefault(), Is.Null);
		Assert.That(new[] { "x" }.RandomOrDefault(), Is.EqualTo("x"));
	}


	/// <summary>Random selections are fixed lists of distinct elements.</summary>
	[Test]
	public void Random_Count()
	{
		List<int> pick = Enumerable.Range(0, 100).Random(3);

		Assert.That(pick, Has.Count.EqualTo(3));
		Assert.That(pick, Is.Unique);
		Assert.That(new[] { 1, 2 }.Random(5), Has.Count.EqualTo(2));
		Assert.That(new[] { 1, 2 }.Random(0), Is.Empty);
		Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Random(-1));
	}


	/// <summary>Shuffling keeps all elements; Shuffled does not change the source.</summary>
	[Test]
	public void Shuffle()
	{
		int[] source = Enumerable.Range(0, 50).ToArray();
		List<int> shuffled = source.Shuffled();

		Assert.That(shuffled, Is.EquivalentTo(source));
		Assert.That(source, Is.Ordered);

		List<int> list = Enumerable.Range(0, 50).ToList();
		Assert.That(list.Shuffle(), Is.SameAs(list));
		Assert.That(list, Is.EquivalentTo(source));

		int[] array = Enumerable.Range(0, 50).ToArray();
		Assert.That(array.Shuffle(), Is.SameAs(array));
		Assert.That(array, Is.EquivalentTo(source));

		bool changed = false;
		for(int i = 0; (i < 10) && (!changed); i++) { changed = (!source.Shuffled().SequenceEqual(source)); }
		Assert.That(changed, Is.True);
	}


	/// <summary>Items are added.</summary>
	[Test]
	public void AddItems()
	{
		List<string> list = new() { "a" };
		list.AddItems("b", "c");

		Assert.That(list, Is.EqualTo(new[] { "a", "b", "c" }));
	}


	/// <summary>Indexes are found, including NULL.</summary>
	[Test]
	public void GetIndex()
	{
		List<string?> list = new() { "a", null, "b", "b" };

		Assert.That(list.GetIndex("b"), Is.EqualTo(2));
		Assert.That(list.GetIndex(null), Is.EqualTo(1));
		Assert.That(list.GetIndex("x"), Is.EqualTo(-1));
	}
}
