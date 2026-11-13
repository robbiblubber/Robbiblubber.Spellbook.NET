namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class implements the SHA-512 hash algorithm.</summary>
/// <remarks>Hashes are too fast for storing passwords; use <see cref="Pbkdf2"/> for passwords.</remarks>
public sealed class SHA512: HashBase, IHash
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instance using Base64 encoding.</summary>
	public static readonly SHA512 BASE64 = new(Base64.DEFAULT);

	/// <summary>Instance using Base62 encoding.</summary>
	public static readonly SHA512 BASE62 = new(Base62.DEFAULT);

	/// <summary>Instance using inverted Base62 encoding.</summary>
	public static readonly SHA512 INVERTED_BASE62 = new(InvertedBase62.DEFAULT);

	/// <summary>Instance using hexadecimal encoding.</summary>
	public static readonly SHA512 HEX = new(Hex.DEFAULT);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoder">Encoder that converts hashes to strings; NULL for hexadecimal.</param>
	/// <exception cref="ArgumentException">Thrown when the encoder is <see cref="PlainText"/>.</exception>
	public SHA512(IEncoder? encoder = null): base(encoder)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hashes a string.</summary>
	/// <param name="value">String.</param>
	/// <param name="encoder">Encoder; NULL for hexadecimal.</param>
	/// <returns>Returns the encoded hash.</returns>
	public static string GetHash(string value, IEncoder? encoder = null)
	{
		return new SHA512(encoder).HashString(value);
	}


	/// <summary>Hashes bytes.</summary>
	/// <param name="value">Bytes.</param>
	/// <param name="encoder">Encoder; NULL for hexadecimal.</param>
	/// <returns>Returns the encoded hash.</returns>
	public static string GetHash(byte[] value, IEncoder? encoder = null)
	{
		return new SHA512(encoder).HashBytes(value);
	}


	/// <summary>Hashes a file.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="withUnixBreaks">Determines if the file is read as UTF-8 text and line breaks are converted to LF before
	///                              hashing, so that the hash does not depend on the platform. If FALSE, the file contents are
	///                              hashed as they are.</param>
	/// <param name="encoder">Encoder; NULL for hexadecimal.</param>
	/// <returns>Returns the encoded hash.</returns>
	public static string GetFileHash(string fileName, bool withUnixBreaks = false, IEncoder? encoder = null)
	{
		SHA512 h = new(encoder);
		if(!withUnixBreaks) { return h.HashFile(fileName); }

		return h.HashString(File.ReadAllText(fileName, Encoding.UTF8).Replace("\r\n", "\n"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] HashBase                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override byte[] Hash(byte[] data)
	{
		return System.Security.Cryptography.SHA512.HashData(data);
	}


	/// <inheritdoc/>
	public override byte[] Hash(Stream stream)
	{
		return System.Security.Cryptography.SHA512.HashData(stream);
	}
}
