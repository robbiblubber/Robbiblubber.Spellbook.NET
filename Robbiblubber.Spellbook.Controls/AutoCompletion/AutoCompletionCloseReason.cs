namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the reasons for an auto completion session to end.</summary>
[SupportedOSPlatform("windows")]
public enum AutoCompletionCloseReason: int
{
	/// <summary>The session has been cancelled by the user (Escape).</summary>
	CANCELLED = 0,

	/// <summary>An item has been accepted.</summary>
	ACCEPTED = 1,

	/// <summary>The caret has left the word that is being completed, or a non-word character has been typed.</summary>
	CARET_MOVED = 2,

	/// <summary>The control or its form has lost focus.</summary>
	FOCUS_LOST = 3,

	/// <summary>The text has been changed by other means than typing.</summary>
	TEXT_CHANGED = 4,

	/// <summary>No items were available.</summary>
	NO_ITEMS = 5,

	/// <summary>The session has been closed by code.</summary>
	MANUAL = 6,

	/// <summary>Retrieving the items has failed.</summary>
	ERROR = 7,

	/// <summary>The user has clicked outside the auto completion list and the text area of the control.</summary>
	CLICKED_OUTSIDE = 8
}
