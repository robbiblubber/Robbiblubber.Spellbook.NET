namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how list items (auto completion, rich combo box) are matched against the typed text.</summary>
[SupportedOSPlatform("windows")]
public enum ListMatchMode: int
{
	/// <summary>Items match if they start with the typed text.</summary>
	PREFIX = 0,

	/// <summary>Items match if they contain the typed text.</summary>
	CONTAINS = 1,

	/// <summary>Items match if they contain the characters of the typed text in the same order.</summary>
	FUZZY = 2
}
