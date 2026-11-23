namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the drop-down styles of a <see cref="ThemedComboBox"/>.</summary>
[SupportedOSPlatform("windows")]
public enum ThemedComboBoxStyle: int
{
	/// <summary>The user selects items from the list; the text cannot be edited.</summary>
	LIST = 0,

	/// <summary>The user can type text; typing searches the items or a search source.</summary>
	COMBO = 1
}
