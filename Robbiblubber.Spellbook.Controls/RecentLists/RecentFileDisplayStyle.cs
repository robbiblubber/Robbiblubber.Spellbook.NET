namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how a <see cref="RecentFileList"/> shows file names in a menu.</summary>
[SupportedOSPlatform("windows")]
public enum RecentFileDisplayStyle: int
{
	/// <summary>The file name without its path is shown; the full path is shown as tool tip.</summary>
	FILE_NAME = 0,

	/// <summary>The full path is shown, shortened in the middle if it is too long.</summary>
	FULL_PATH = 1
}
