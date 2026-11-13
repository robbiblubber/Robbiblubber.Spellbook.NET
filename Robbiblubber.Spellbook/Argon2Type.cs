namespace Robbiblubber.Spellbook;



/// <summary>This enumeration defines the Argon2 variants.</summary>
public enum Argon2Type: int
{
	/// <summary>Argon2d; uses data-dependent memory access. Resistant against GPU attacks, but vulnerable to side channels.</summary>
	ARGON2D = 0,
	/// <summary>Argon2i; uses data-independent memory access. Resistant against side channels.</summary>
	ARGON2I = 1,
	/// <summary>Argon2id; combines both. This is the recommended variant for password hashing.</summary>
	ARGON2ID = 2
}
