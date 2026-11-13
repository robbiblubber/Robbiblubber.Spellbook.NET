namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements a list of weak references.</summary>
/// <typeparam name="T">Item type.</typeparam>
/// <remarks>The list keeps listeners of static notifications without keeping them alive. Items that have been collected are removed
///          when the list is read or grows.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _WeakList<T> where T: class
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Items.</summary>
	private readonly List<WeakReference<T>> _Items = new();

	/// <summary>Number of items at which the next addition removes collected items.</summary>
	private int _PurgeAt = 64;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the collected items.</summary>
	private void _Purge()
	{
		_Items.RemoveAll(m => !m.TryGetTarget(out _));
		_PurgeAt = Math.Max(64, (_Items.Count * 2));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of entries including collected items.</summary>
	internal int _Count
	{
		get { lock(_Items) { return _Items.Count; } }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds an item.</summary>
	/// <param name="item">Item.</param>
	internal void _Add(T item)
	{
		lock(_Items)
		{
			if(_Items.Count >= _PurgeAt) { _Purge(); }
			_Items.Add(new WeakReference<T>(item));
		}
	}


	/// <summary>Removes an item.</summary>
	/// <param name="item">Item.</param>
	internal void _Remove(T item)
	{
		lock(_Items)
		{
			_Items.RemoveAll(m => (!m.TryGetTarget(out T? target) || ReferenceEquals(target, item)));
		}
	}


	/// <summary>Gets the items that are still alive.</summary>
	/// <returns>Returns a snapshot of the items, so the list can be changed while the snapshot is processed.</returns>
	internal List<T> _ToList()
	{
		lock(_Items)
		{
			List<T> rval = new(_Items.Count);

			foreach(WeakReference<T> i in _Items)
			{
				if(i.TryGetTarget(out T? target)) { rval.Add(target); }
			}
			if(rval.Count < _Items.Count) { _Purge(); }

			return rval;
		}
	}
}
