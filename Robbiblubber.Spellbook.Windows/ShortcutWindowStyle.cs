namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.Versioning;



/// <summary>Window styles for applications started from a shortcut.</summary>
[SupportedOSPlatform("windows")]
public enum ShortcutWindowStyle: int
{
	/// <summary>The window is shown normally.</summary>
	NORMAL = 1,

	/// <summary>The window is maximized.</summary>
	MAXIMIZED = 3,

	/// <summary>The window is minimized.</summary>
	MINIMIZED = 7
}
