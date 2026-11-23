namespace Robbiblubber.Spellbook;

using System.Globalization;
using System.Security.Cryptography;



/// <summary>This class hashes passwords with PBKDF2.</summary>
/// <remarks>Hashes are written in the PHC string format <c>$pbkdf2-sha256$i=600000$salt$hash</c> with Base64 salt and hash.
///          The default parameters follow the OWASP recommendations: SHA-256 with 600,000 iterations, a 16 byte salt and a
///          32 byte hash.</remarks>
public sealed class Pbkdf2: IPasswordHasher
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instance with default parameters.</summary>
	public static readonly Pbkdf2 DEFAULT = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="algorithm">Hash algorithm; SHA-256, SHA-384 or SHA-512. NULL for SHA-256.</param>
	/// <param name="iterations">Number of iterations; 0 for the recommended number of the algorithm (600,000 for SHA-256,
	///                          210,000 for SHA-384 and SHA-512).</param>
	/// <param name="saltSize">Salt size in bytes; at least 8.</param>
	/// <param name="hashSize">Hash size in bytes; at least 16.</param>
	/// <exception cref="ArgumentException">Thrown when a parameter is invalid.</exception>
	public Pbkdf2(HashAlgorithmName? algorithm = null, int iterations = 0, int saltSize = 16, int hashSize = 32)
	{
		Algorithm = (algorithm ?? HashAlgorithmName.SHA256);
		if(_GetName(Algorithm) is null) { throw new ArgumentException("Unsupported hash algorithm " + Algorithm.Name + ".", nameof(algorithm)); }

		if(iterations < 0) { throw new ArgumentException("The number of iterations must not be negative.", nameof(iterations)); }
		if(saltSize < 8) { throw new ArgumentException("The salt must have at least 8 bytes.", nameof(saltSize)); }
		if(hashSize < 16) { throw new ArgumentException("The hash must have at least 16 bytes.", nameof(hashSize)); }

		Iterations = ((iterations > 0) ? iterations : ((Algorithm == HashAlgorithmName.SHA256) ? 600_000 : 210_000));
		SaltSize = saltSize;
		HashSize = hashSize;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the hash algorithm.</summary>
	public HashAlgorithmName Algorithm
	{
		get;
	}


	/// <summary>Gets the number of iterations.</summary>
	public int Iterations
	{
		get;
	}


	/// <summary>Gets the salt size in bytes.</summary>
	public int SaltSize
	{
		get;
	}


	/// <summary>Gets the hash size in bytes.</summary>
	public int HashSize
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the PHC name of an algorithm.</summary>
	/// <param name="algorithm">Algorithm.</param>
	/// <returns>Returns the name, or NULL if the algorithm is not supported.</returns>
	private static string? _GetName(HashAlgorithmName algorithm)
	{
		if(algorithm == HashAlgorithmName.SHA256) { return "pbkdf2-sha256"; }
		if(algorithm == HashAlgorithmName.SHA384) { return "pbkdf2-sha384"; }
		if(algorithm == HashAlgorithmName.SHA512) { return "pbkdf2-sha512"; }

		return null;
	}


	/// <summary>Parses a password hash.</summary>
	/// <param name="hash">Password hash.</param>
	/// <param name="algorithm">Algorithm.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <param name="salt">Salt.</param>
	/// <param name="key">Derived key.</param>
	/// <returns>Returns TRUE if the hash is a valid PBKDF2 hash, otherwise returns FALSE.</returns>
	private static bool _TryParse(string hash, out HashAlgorithmName algorithm, out int iterations, out byte[] salt, out byte[] key)
	{
		algorithm = default;
		iterations = 0;
		salt = key = [];

		string[] parts = hash.Split('$');
		if((parts.Length != 5) || (parts[0].Length != 0)) { return false; }

		switch(parts[1])
		{
			case "pbkdf2-sha256": algorithm = HashAlgorithmName.SHA256; break;
			case "pbkdf2-sha384": algorithm = HashAlgorithmName.SHA384; break;
			case "pbkdf2-sha512": algorithm = HashAlgorithmName.SHA512; break;
			default: return false;
		}

		if((!parts[2].StartsWith("i=", StringComparison.Ordinal)) ||
		   (!int.TryParse(parts[2][2..], NumberStyles.None, CultureInfo.InvariantCulture, out iterations)) || (iterations < 1)) { return false; }

		try
		{
			salt = Base64.ToBytes(parts[3]);
			key = Base64.ToBytes(parts[4]);
		}
		catch(FormatException) { return false; }

		return ((salt.Length > 0) && (key.Length > 0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IPasswordHasher                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public string Hash(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
		byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

		return ("$" + _GetName(Algorithm) + "$i=" + Iterations.ToString(CultureInfo.InvariantCulture) + "$" + Base64.FromBytes(salt) + "$" + Base64.FromBytes(key));
	}


	/// <inheritdoc/>
	public bool Verify(string password, string hash)
	{
		if(!_TryParse(hash, out HashAlgorithmName algorithm, out int iterations, out byte[] salt, out byte[] key)) { return false; }

		return CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithm, key.Length), key);
	}


	/// <inheritdoc/>
	public bool NeedsRehash(string hash)
	{
		if(!_TryParse(hash, out HashAlgorithmName algorithm, out int iterations, out byte[] salt, out byte[] key)) { return true; }

		return ((algorithm != Algorithm) || (iterations < Iterations) || (salt.Length < SaltSize) || (key.Length < HashSize));
	}
}
