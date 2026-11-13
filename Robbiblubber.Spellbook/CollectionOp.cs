namespace Robbiblubber.Spellbook;



/// <summary>This class provides collection extension methods.</summary>
/// <remarks>Random selection and shuffling use <see cref="System.Random.Shared"/> and are thread-safe.</remarks>
public static class CollectionOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a random element.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="c">Collection.</param>
	/// <returns>Returns a random element.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the collection is empty.</exception>
	public static T Random<T>(this IEnumerable<T> c)
	{
		IList<T> list = ((c as IList<T>) ?? c.ToList());
		if(list.Count == 0) { throw new InvalidOperationException("The collection is empty."); }

		return list[System.Random.Shared.Next(list.Count)];
	}


	/// <summary>Gets a random element, or the default value if the collection is empty.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="c">Collection.</param>
	/// <returns>Returns a random element, or the default value.</returns>
	public static T? RandomOrDefault<T>(this IEnumerable<T> c)
	{
		IList<T> list = ((c as IList<T>) ?? c.ToList());

		return ((list.Count == 0) ? default : list[System.Random.Shared.Next(list.Count)]);
	}


	/// <summary>Gets a number of distinct random elements.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="c">Collection.</param>
	/// <param name="count">Number of elements; fewer if the collection is smaller.</param>
	/// <returns>Returns a new list with the elements in random order.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the count is negative.</exception>
	public static List<T> Random<T>(this IEnumerable<T> c, int count)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count);

		List<T> rval = c.Shuffled();
		if(count < rval.Count) { rval.RemoveRange(count, rval.Count - count); }

		return rval;
	}


	/// <summary>Returns the elements of a collection in random order.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="source">Collection.</param>
	/// <returns>Returns a new list with the elements in random order; the collection is not changed.</returns>
	public static List<T> Shuffled<T>(this IEnumerable<T> source)
	{
		List<T> rval = source.ToList();
		rval.Shuffle();

		return rval;
	}


	/// <summary>Shuffles a list in place.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="list">List.</param>
	/// <returns>Returns the list.</returns>
	public static IList<T> Shuffle<T>(this IList<T> list)
	{
		for(int n = (list.Count - 1); n > 0; n--)
		{
			int k = System.Random.Shared.Next(n + 1);
			(list[k], list[n]) = (list[n], list[k]);
		}

		return list;
	}


	/// <summary>Shuffles an array in place.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="array">Array.</param>
	/// <returns>Returns the array.</returns>
	public static T[] Shuffle<T>(this T[] array)
	{
		System.Random.Shared.Shuffle(array);
		return array;
	}


	/// <summary>Adds items to a collection.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="collection">Collection.</param>
	/// <param name="items">Items.</param>
	public static void AddItems<T>(this ICollection<T> collection, params T[] items)
	{
		foreach(T i in items) { collection.Add(i); }
	}


	/// <summary>Gets the index of the first occurrence of an item.</summary>
	/// <typeparam name="T">Element type.</typeparam>
	/// <param name="c">Collection.</param>
	/// <param name="item">Item; may be NULL.</param>
	/// <returns>Returns the index, or -1 if the item does not occur.</returns>
	public static int GetIndex<T>(this IEnumerable<T> c, T item)
	{
		EqualityComparer<T> cmp = EqualityComparer<T>.Default;
		int rval = 0;

		foreach(T i in c)
		{
			if(cmp.Equals(i, item)) { return rval; }
			rval++;
		}

		return -1;
	}
}
