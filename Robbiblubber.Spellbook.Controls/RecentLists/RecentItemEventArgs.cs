namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="RecentItemList.ItemSelected"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class RecentItemEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="item">Selected entry.</param>
	/// <param name="available">Determines if the entry is available, NULL if this is unknown.</param>
	public RecentItemEventArgs(RecentItem item, bool? available): base()
	{
		Item = item;
		Available = available;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the selected entry.</summary>
	public RecentItem Item
	{
		get;
	}


	/// <summary>Gets if the entry is available (for example if the file exists), NULL if this is unknown.</summary>
	public bool? Available
	{
		get;
	}


	/// <summary>Gets or sets if the entry is removed from the list after the event.</summary>
	/// <remarks>An application can set this property, for example after the user has confirmed that a missing file is to be removed
	///          from the list.</remarks>
	public bool Remove
	{
		get; set;
	}
}
