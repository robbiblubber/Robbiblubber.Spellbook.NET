namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the reasons for a signature help session to end.</summary>
[SupportedOSPlatform("windows")]
public enum SignatureHelpCloseReason: int
{
	/// <summary>The session has been cancelled by the user (Escape).</summary>
	CANCELLED = 0,

	/// <summary>The caret is no longer within the argument list of a call, for example because the call has been closed.</summary>
	CALL_LEFT = 1,

	/// <summary>The control or its form has lost focus.</summary>
	FOCUS_LOST = 2,

	/// <summary>No signatures were available for the call.</summary>
	NO_SIGNATURES = 3,

	/// <summary>The session has been closed by code.</summary>
	MANUAL = 4,

	/// <summary>Retrieving the signatures has failed.</summary>
	ERROR = 5,

	/// <summary>The user has clicked outside the signature window and the text area of the control.</summary>
	CLICKED_OUTSIDE = 6
}
