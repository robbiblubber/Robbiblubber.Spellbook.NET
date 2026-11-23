namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents an entry of a <see cref="RecentItemList"/>.</summary>
/// <remarks>Entries are immutable; the list replaces them when they change.</remarks>
[SupportedOSPlatform("windows")]
public sealed class RecentItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="key">Key, for example a file name. It identifies the entry and is what the application needs to open it.</param>
	/// <param name="text">Display text, NULL to let the list create the text from the key.</param>
	/// <param name="pinned">Determines if the entry is pinned.</param>
	/// <param name="lastUsed">Time of the last use (UTC), NULL for now.</param>
	/// <exception cref="ArgumentException">Thrown when the key is NULL or empty.</exception>
	public RecentItem(string key, string? text = null, bool pinned = false, DateTime? lastUsed = null)
	{
		if(string.IsNullOrWhiteSpace(key)) { throw new ArgumentException("Key must not be empty.", nameof(key)); }

		Key = key;
		Text = (string.IsNullOrEmpty(text) ? null : text);
		Pinned = pinned;
		LastUsed = (lastUsed ?? DateTime.UtcNow);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the key.</summary>
	public string Key
	{
		get;
	}


	/// <summary>Gets the display text, NULL if the list creates the text from the key.</summary>
	public string? Text
	{
		get;
	}


	/// <summary>Gets if the entry is pinned.</summary>
	public bool Pinned
	{
		get;
	}


	/// <summary>Gets the time of the last use (UTC).</summary>
	public DateTime LastUsed
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string ToString()
	{
		return (Text ?? Key);
	}
}
