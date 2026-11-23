namespace Robbiblubber.Spellbook;

using System.Security.Cryptography;



/// <summary>Classes that provide hash algorithms implement this interface.</summary>
/// <remarks>Implementations only need to provide <see cref="Encoder"/> and <see cref="Hash(byte[])"/>; the other methods have
///          default implementations. Classes usually derive from <see cref="HashBase"/>.</remarks>
public interface IHash
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the encoder that converts hashes to strings.</summary>
	public IEncoder Encoder
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hashes bytes.</summary>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashBytes(IEnumerable<byte> value)
	{
		return HashBase._HashBytes(this, value);
	}


	/// <summary>Hashes characters.</summary>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashChars(IEnumerable<char> value)
	{
		return HashBase._HashChars(this, value);
	}


	/// <summary>Hashes a string.</summary>
	/// <param name="value">String; it is converted to bytes with the encoding of the encoder.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashString(string value)
	{
		return HashBase._HashString(this, value);
	}


	/// <summary>Hashes an integer.</summary>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashInt(int value)
	{
		return HashBase._HashInt(this, value);
	}


	/// <summary>Hashes a long integer.</summary>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashLong(long value)
	{
		return HashBase._HashLong(this, value);
	}


	/// <summary>Hashes a double.</summary>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashDouble(double value)
	{
		return HashBase._HashDouble(this, value);
	}


	/// <summary>Hashes the contents of a stream.</summary>
	/// <param name="stream">Stream; it is read to its end and not closed.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashStream(Stream stream)
	{
		return HashBase._HashStream(this, stream);
	}


	/// <summary>Hashes the contents of a file.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the encoded hash.</returns>
	public string HashFile(string fileName)
	{
		return HashBase._HashFile(this, fileName);
	}


	/// <summary>Returns if a string has a hash.</summary>
	/// <param name="value">String.</param>
	/// <param name="hash">Encoded hash.</param>
	/// <returns>Returns TRUE if the hash of the string equals the hash, otherwise returns FALSE.</returns>
	/// <remarks>The comparison takes the same time for all strings of the same length, so it does not reveal how much of a hash
	///          matches.</remarks>
	public bool Verify(string value, string hash)
	{
		return HashBase._Verify(this, value, hash);
	}


	/// <summary>Hashes bytes.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the hash.</returns>
	public byte[] Hash(byte[] data);


	/// <summary>Hashes the contents of a stream.</summary>
	/// <param name="stream">Stream; it is read to its end and not closed.</param>
	/// <returns>Returns the hash.</returns>
	public byte[] Hash(Stream stream)
	{
		using(MemoryStream ms = new())
		{
			stream.CopyTo(ms);
			return Hash(ms.ToArray());
		}
	}
}
