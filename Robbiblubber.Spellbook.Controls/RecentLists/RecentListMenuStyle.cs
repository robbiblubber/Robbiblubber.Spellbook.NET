namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how a <see cref="RecentItemList"/> shows its entries in a menu.</summary>
[SupportedOSPlatform("windows")]
public enum RecentListMenuStyle: int
{
	/// <summary>The entries are shown in the drop-down of the menu item.</summary>
	SUBMENU = 0,

	/// <summary>The entries are shown in place of the menu item, which serves as a hidden placeholder.</summary>
	INLINE = 1
}
