namespace Robbiblubber.Spellbook;

using System.Security.Cryptography;



/// <summary>This class provides a base implementation for hash algorithms.</summary>
public abstract class HashBase: IHash
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoder">Encoder that converts hashes to strings; NULL for hexadecimal.</param>
	/// <exception cref="ArgumentException">Thrown when the encoder is <see cref="PlainText"/>, which cannot represent hashes.</exception>
	protected HashBase(IEncoder? encoder)
	{
		if(encoder is PlainText) { throw new ArgumentException("Plain text cannot represent hashes.", nameof(encoder)); }
		Encoder = (encoder ?? Hex.DEFAULT);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Encodes a hash.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="hash">Hash.</param>
	/// <returns>Returns the encoded hash.</returns>
	/// <remarks>BaseN encoders write hashes as numbers without leading zero digits, as earlier versions did, so stored hashes
	///          remain valid.</remarks>
	internal static string _Encode(IEncoder encoder, byte[] hash)
	{
		return ((encoder is BaseN n) ? n.EncodeNumber(hash) : encoder.EncodeBytes(hash));
	}


	/// <summary>Default implementation of <see cref="IHash.HashBytes(IEnumerable{byte})"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashBytes(IHash hasher, IEnumerable<byte> value)
	{
		return _Encode(hasher.Encoder, hasher.Hash(value.ToArray()));
	}


	/// <summary>Default implementation of <see cref="IHash.HashChars(IEnumerable{char})"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashChars(IHash hasher, IEnumerable<char> value)
	{
		return hasher.HashString(new string(value.ToArray()));
	}


	/// <summary>Default implementation of <see cref="IHash.HashString(string)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">String; it is converted to bytes with the encoding of the encoder.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashString(IHash hasher, string value)
	{
		return _Encode(hasher.Encoder, hasher.Hash(hasher.Encoder.Encoding.GetBytes(value)));
	}


	/// <summary>Default implementation of <see cref="IHash.HashInt(int)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashInt(IHash hasher, int value)
	{
		return _Encode(hasher.Encoder, hasher.Hash(BitConverter.GetBytes(value)));
	}


	/// <summary>Default implementation of <see cref="IHash.HashLong(long)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashLong(IHash hasher, long value)
	{
		return _Encode(hasher.Encoder, hasher.Hash(BitConverter.GetBytes(value)));
	}


	/// <summary>Default implementation of <see cref="IHash.HashDouble(double)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashDouble(IHash hasher, double value)
	{
		return _Encode(hasher.Encoder, hasher.Hash(BitConverter.GetBytes(value)));
	}


	/// <summary>Default implementation of <see cref="IHash.HashStream(Stream)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="stream">Stream; it is read to its end and not closed.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashStream(IHash hasher, Stream stream)
	{
		return _Encode(hasher.Encoder, hasher.Hash(stream));
	}


	/// <summary>Default implementation of <see cref="IHash.HashFile(string)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the encoded hash.</returns>
	internal static string _HashFile(IHash hasher, string fileName)
	{
		using(FileStream fs = File.OpenRead(fileName))
		{
			return hasher.HashStream(fs);
		}
	}


	/// <summary>Default implementation of <see cref="IHash.Verify(string,string)"/>.</summary>
	/// <param name="hasher">Hash algorithm.</param>
	/// <param name="value">String.</param>
	/// <param name="hash">Encoded hash.</param>
	/// <returns>Returns TRUE if the hash of the string equals the hash, otherwise returns FALSE.</returns>
	/// <remarks>The comparison takes the same time for all strings of the same length, so it does not reveal how much of a hash
	///          matches.</remarks>
	internal static bool _Verify(IHash hasher, string value, string hash)
	{
		return CryptographicOperations.FixedTimeEquals(hasher.Encoder.Encoding.GetBytes(hasher.HashString(value)), hasher.Encoder.Encoding.GetBytes(hash));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IHash                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public IEncoder Encoder
	{
		get;
	}


	/// <inheritdoc/>
	public virtual string HashBytes(IEnumerable<byte> value)
	{
		return _HashBytes(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashChars(IEnumerable<char> value)
	{
		return _HashChars(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashString(string value)
	{
		return _HashString(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashInt(int value)
	{
		return _HashInt(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashLong(long value)
	{
		return _HashLong(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashDouble(double value)
	{
		return _HashDouble(this, value);
	}


	/// <inheritdoc/>
	public virtual string HashStream(Stream stream)
	{
		return _HashStream(this, stream);
	}


	/// <inheritdoc/>
	public virtual string HashFile(string fileName)
	{
		return _HashFile(this, fileName);
	}


	/// <inheritdoc/>
	public virtual bool Verify(string value, string hash)
	{
		return _Verify(this, value, hash);
	}


	/// <inheritdoc/>
	public abstract byte[] Hash(byte[] data);


	/// <inheritdoc/>
	public abstract byte[] Hash(Stream stream);
}
