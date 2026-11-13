namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the ways signatures can be requested.</summary>
[SupportedOSPlatform("windows")]
public enum SignatureHelpTrigger: int
{
	/// <summary>Signature help has been started by typing a trigger character.</summary>
	TRIGGER_CHARACTER = 0,

	/// <summary>Signature help has been started by the shortcut keys.</summary>
	SHORTCUT = 1,

	/// <summary>Signature help has been started by code.</summary>
	MANUAL = 2,

	/// <summary>Signature help has been started after an auto completion item with an opening parenthesis has been inserted.</summary>
	AUTO_COMPLETION = 3,

	/// <summary>The caret has moved to another call during a session, for example after closing a nested call.</summary>
	CALL_CHANGED = 4
}
