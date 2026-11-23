namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the edit commands.</summary>
[SupportedOSPlatform("windows")]
public enum EditCommand: int
{
	/// <summary>Cuts the selection to the clipboard.</summary>
	CUT = 0,

	/// <summary>Copies the selection to the clipboard.</summary>
	COPY = 1,

	/// <summary>Pastes the clipboard contents.</summary>
	PASTE = 2,

	/// <summary>Deletes the selection.</summary>
	DELETE = 3,

	/// <summary>Undoes the last operation.</summary>
	UNDO = 4,

	/// <summary>Redoes the last undone operation.</summary>
	REDO = 5,

	/// <summary>Selects all elements.</summary>
	SELECT_ALL = 6,

	/// <summary>Inverts the selection.</summary>
	INVERT_SELECTION = 7,

	/// <summary>Clears the selection.</summary>
	CLEAR_SELECTION = 8
}
