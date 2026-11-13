namespace Robbiblubber.Spellbook;



/// <summary>Classes that hash passwords for storage implement this interface.</summary>
/// <remarks>Password hashes contain the algorithm, its parameters and a random salt, so they can be verified later even if the
///          default parameters change.</remarks>
public interface IPasswordHasher
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hashes a password.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns the password hash.</returns>
	public string Hash(string password);


	/// <summary>Returns if a password matches a password hash.</summary>
	/// <param name="password">Password.</param>
	/// <param name="hash">Password hash.</param>
	/// <returns>Returns TRUE if the password matches the hash, FALSE if it does not match or the hash has not been created by
	///          this algorithm.</returns>
	public bool Verify(string password, string hash);


	/// <summary>Returns if a password hash should be replaced by a new hash.</summary>
	/// <param name="hash">Password hash.</param>
	/// <returns>Returns TRUE if the hash uses a different algorithm or weaker parameters than this instance, otherwise
	///          returns FALSE.</returns>
	/// <remarks>Check this after a successful <see cref="Verify(string, string)"/> and store a new hash of the password.</remarks>
	public bool NeedsRehash(string hash);
}
