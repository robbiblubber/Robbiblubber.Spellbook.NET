namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.Versioning;



/// <summary>Scopes for registrations with Windows.</summary>
[SupportedOSPlatform("windows")]
public enum RegistrationScope: int
{
	/// <summary>The registration applies to the current user (HKEY_CURRENT_USER) and does not require administrator rights.</summary>
	CURRENT_USER = 0,

	/// <summary>The registration applies to all users (HKEY_LOCAL_MACHINE) and requires administrator rights.</summary>
	LOCAL_MACHINE = 1
}
