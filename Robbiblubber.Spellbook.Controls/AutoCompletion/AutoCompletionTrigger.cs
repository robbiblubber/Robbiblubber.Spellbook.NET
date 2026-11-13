namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the ways an auto completion session can be started.</summary>
[SupportedOSPlatform("windows")]
public enum AutoCompletionTrigger: int
{
	/// <summary>Auto completion has been started by typing a word character.</summary>
	TYPING = 0,

	/// <summary>Auto completion has been started by typing a trigger character.</summary>
	TRIGGER_CHARACTER = 1,

	/// <summary>Auto completion has been started by the shortcut keys.</summary>
	SHORTCUT = 2,

	/// <summary>Auto completion has been started by code.</summary>
	MANUAL = 3
}
