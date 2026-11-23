namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that implement this interface store the entries of a <see cref="RecentItemList"/>.</summary>
/// <remarks><para>A store can keep several lists, identified by name. Names consist of letters, digits, underscores and hyphens.</para>
///          <para>The list loads the entries before every change and saves them right after, so several instances of an application
///          that share a store do not overwrite each other's changes. The default implementation is
///          <see cref="DdnRecentListStore"/>.</para></remarks>
[SupportedOSPlatform("windows")]
public interface IRecentListStore
{
	/// <summary>Loads a list.</summary>
	/// <param name="list">List name.</param>
	/// <returns>Returns the entries in their order, an empty list if the store does not contain the list.</returns>
	public IReadOnlyList<RecentItem> Load(string list);


	/// <summary>Saves a list.</summary>
	/// <param name="list">List name.</param>
	/// <param name="items">Entries in their order.</param>
	public void Save(string list, IReadOnlyList<RecentItem> items);
}
