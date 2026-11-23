namespace Robbiblubber.Spellbook.UnitTests.Controls.RecentLists;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a recent list store that keeps the lists in memory.</summary>
public class TestRecentListStore: IRecentListStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the lists.</summary>
	public Dictionary<string, List<RecentItem>> Lists
	{
		get;
	} = new();


	/// <summary>Gets the number of saves.</summary>
	public int Saves
	{
		get; private set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IRecentListStore                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public IReadOnlyList<RecentItem> Load(string list)
	{
		return (Lists.TryGetValue(list, out List<RecentItem>? rval) ? rval.ToList() : new List<RecentItem>());
	}


	/// <inheritdoc/>
	public void Save(string list, IReadOnlyList<RecentItem> items)
	{
		Lists[list] = items.ToList();
		Saves++;
	}
}
