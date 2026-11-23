namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines password strengths.</summary>
[SupportedOSPlatform("windows")]
public enum PasswordStrength: int
{
	/// <summary>No password has been entered.</summary>
	NONE = -1,

	/// <summary>Very weak password.</summary>
	VERY_WEAK = 0,

	/// <summary>Weak password.</summary>
	WEAK = 1,

	/// <summary>Fair password.</summary>
	FAIR = 2,

	/// <summary>Good password.</summary>
	GOOD = 3,

	/// <summary>Strong password.</summary>
	STRONG = 4,

	/// <summary>Very strong password.</summary>
	VERY_STRONG = 5
}
