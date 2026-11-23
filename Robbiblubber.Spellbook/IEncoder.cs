namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>Classes that provide encoding and decoding implement this interface.</summary>
/// <remarks>Implementations only need to provide <see cref="Encoding"/>, <see cref="Encode(byte[])"/> and
///          <see cref="Decode(byte[])"/>; the other methods have default implementations. Classes usually derive from
///          <see cref="EncoderBase"/>.</remarks>
public interface IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the encoding that converts between strings and bytes.</summary>
	public Encoding Encoding
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to an encoded string.</summary>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeBytes(IEnumerable<byte> value)
	{
		return EncoderBase._EncodeBytes(this, value);
	}


	/// <summary>Converts characters to an encoded string.</summary>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeChars(IEnumerable<char> value)
	{
		return EncoderBase._EncodeChars(this, value);
	}


	/// <summary>Converts a string to an encoded string.</summary>
	/// <param name="value">String.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeString(string value)
	{
		return EncoderBase._EncodeString(this, value);
	}


	/// <summary>Converts an integer to an encoded string.</summary>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeInt(int value)
	{
		return EncoderBase._EncodeInt(this, value);
	}


	/// <summary>Converts a long integer to an encoded string.</summary>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeLong(long value)
	{
		return EncoderBase._EncodeLong(this, value);
	}


	/// <summary>Converts a double to an encoded string.</summary>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeDouble(double value)
	{
		return EncoderBase._EncodeDouble(this, value);
	}


	/// <summary>Converts an encoded string to a byte array of a fixed length.</summary>
	/// <param name="s">Encoded string.</param>
	/// <param name="len">Array length.</param>
	/// <returns>Returns the decoded bytes, aligned to the end of the array.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid or the decoded data is longer than the array.</exception>
	public byte[] DecodeToBytes(string s, int len)
	{
		return EncoderBase._DecodeToBytes(this, s, len);
	}


	/// <summary>Converts an encoded string to a byte array.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded bytes.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public byte[] DecodeToBytes(string s)
	{
		return EncoderBase._DecodeToBytes(this, s);
	}


	/// <summary>Converts an encoded string to a character array.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded characters.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public char[] DecodeToChars(string s)
	{
		return EncoderBase._DecodeToChars(this, s);
	}


	/// <summary>Converts an encoded string to a string.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public string DecodeToString(string s)
	{
		return EncoderBase._DecodeToString(this, s);
	}


	/// <summary>Converts an encoded string to an integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public int DecodeToInt(string s)
	{
		return EncoderBase._DecodeToInt(this, s);
	}


	/// <summary>Converts an encoded string to a long integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded long integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public long DecodeToLong(string s)
	{
		return EncoderBase._DecodeToLong(this, s);
	}


	/// <summary>Converts an encoded string to a double.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded double.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public double DecodeToDouble(string s)
	{
		return EncoderBase._DecodeToDouble(this, s);
	}


	/// <summary>Encodes a byte array.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the encoded data.</returns>
	public byte[] Encode(byte[] data);


	/// <summary>Decodes a byte array.</summary>
	/// <param name="data">Encoded data.</param>
	/// <returns>Returns the decoded data.</returns>
	/// <exception cref="FormatException">Thrown when the data is invalid.</exception>
	public byte[] Decode(byte[] data);
}
