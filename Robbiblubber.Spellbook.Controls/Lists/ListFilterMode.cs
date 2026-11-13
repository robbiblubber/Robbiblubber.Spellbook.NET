namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how an item list (auto completion, rich combo box) reacts to the typed text.</summary>
[SupportedOSPlatform("windows")]
public enum ListFilterMode: int
{
	/// <summary>Only matching items are shown, ordered by match quality.</summary>
	FILTER = 0,

	/// <summary>All items are shown, the best matching item is selected.</summary>
	SELECT = 1
}
