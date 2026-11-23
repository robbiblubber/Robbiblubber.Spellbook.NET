namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how matched characters are highlighted in item lists (auto completion, signature help,
///          rich combo box).</summary>
[SupportedOSPlatform("windows")]
public enum ListHighlightStyle: int
{
	/// <summary>Matched characters are not highlighted.</summary>
	NONE = 0,

	/// <summary>Matched characters are drawn bold (underlined for bold items).</summary>
	BOLD = 1,

	/// <summary>Matched characters are underlined.</summary>
	UNDERLINE = 2,

	/// <summary>Matched characters are drawn in the highlight color.</summary>
	COLOR = 3
}
