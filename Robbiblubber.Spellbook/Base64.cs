namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class implements Base64 encoding.</summary>
/// <remarks>By default the result is not padded. Both the standard and the URL-safe alphabet are read, with or without padding
///          and whitespace.</remarks>
public sealed class Base64: EncoderBase, IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Base64 alphabet; the last character is the padding character.</summary>
	public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

	/// <summary>URL-safe Base64 alphabet.</summary>
	public const string URL_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

	/// <summary>Default instance.</summary>
	public static readonly Base64 DEFAULT = new();

	/// <summary>Instance that uses the URL-safe alphabet.</summary>
	public static readonly Base64 URL = new(null, true);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public Base64(): this(null)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoding">Encoding that converts between strings and bytes; NULL for UTF-8.</param>
	/// <param name="urlSafe">Determines if the URL-safe alphabet is written.</param>
	public Base64(Encoding? encoding, bool urlSafe = false): base(encoding)
	{
		UrlSafe = urlSafe;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to a Base64 string.</summary>
	/// <param name="value">Bytes.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromBytes(IEnumerable<byte> value, bool pad = false)
	{
		return DEFAULT.EncodeBytes(value, pad);
	}


	/// <summary>Converts characters to a Base64 string.</summary>
	/// <param name="value">Characters.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromChars(IEnumerable<char> value, bool pad = false)
	{
		return DEFAULT.EncodeChars(value, pad);
	}


	/// <summary>Converts a string to a Base64 string.</summary>
	/// <param name="value">String.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromString(string value, bool pad = false)
	{
		return DEFAULT.EncodeString(value, pad);
	}


	/// <summary>Converts an integer to a Base64 string.</summary>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromInt(int value)
	{
		return DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a long integer to a Base64 string.</summary>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromLong(long value)
	{
		return DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts a double to a Base64 string.</summary>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromDouble(double value)
	{
		return DEFAULT.EncodeDouble(value);
	}


	/// <summary>Reads a file into a Base64 string.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromFile(string fileName, bool pad = false)
	{
		return DEFAULT.EncodeFile(fileName, pad);
	}


	/// <summary>Converts a Base64 string to bytes.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded bytes.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static byte[] ToBytes(string s)
	{
		return DEFAULT.DecodeToBytes(s);
	}


	/// <summary>Converts a Base64 string to characters.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded characters.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static char[] ToChars(string s)
	{
		return DEFAULT.DecodeToChars(s);
	}


	/// <summary>Converts a Base64 string to a string.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string Decode(string s)
	{
		return DEFAULT.DecodeToString(s);
	}


	/// <summary>Converts a Base64 string to an integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static int ToInt(string s)
	{
		return DEFAULT.DecodeToInt(s);
	}


	/// <summary>Converts a Base64 string to a long integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded long integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static long ToLong(string s)
	{
		return DEFAULT.DecodeToLong(s);
	}


	/// <summary>Converts a Base64 string to a double.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded double.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static double ToDouble(string s)
	{
		return DEFAULT.DecodeToDouble(s);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the URL-safe alphabet (<c>-</c> and <c>_</c> instead of <c>+</c> and <c>/</c>) is written.</summary>
	public bool UrlSafe
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to an encoded string.</summary>
	/// <param name="value">Bytes.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeBytes(IEnumerable<byte> value, bool pad)
	{
		string rval = Convert.ToBase64String(value.ToArray());

		if(UrlSafe) { rval = rval.Replace('+', '-').Replace('/', '_'); }
		return (pad ? rval : rval.TrimEnd('='));
	}


	/// <summary>Converts characters to an encoded string.</summary>
	/// <param name="value">Characters.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeChars(IEnumerable<char> value, bool pad)
	{
		return EncodeBytes(Encoding.GetBytes(value.ToArray()), pad);
	}


	/// <summary>Converts a string to an encoded string.</summary>
	/// <param name="value">String.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeString(string value, bool pad)
	{
		return EncodeBytes(Encoding.GetBytes(value), pad);
	}


	/// <summary>Reads a file into an encoded string.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeFile(string fileName, bool pad)
	{
		return EncodeBytes(File.ReadAllBytes(fileName), pad);
	}


	/// <summary>Encodes a byte array.</summary>
	/// <param name="data">Data.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded data.</returns>
	public byte[] Encode(byte[] data, bool pad)
	{
		return Encoding.GetBytes(EncodeBytes(data, pad));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EncoderBase                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string EncodeBytes(IEnumerable<byte> value)
	{
		return EncodeBytes(value, false);
	}


	/// <inheritdoc/>
	public override string EncodeChars(IEnumerable<char> value)
	{
		return EncodeChars(value, false);
	}


	/// <inheritdoc/>
	public override string EncodeString(string value)
	{
		return EncodeString(value, false);
	}


	/// <inheritdoc/>
	public override byte[] DecodeToBytes(string s)
	{
		StringBuilder b = new(s.Length + 3);
		foreach(char i in s)
		{
			if(char.IsWhiteSpace(i) || (i == '=')) { continue; }
			b.Append((i == '-') ? '+' : ((i == '_') ? '/' : i));
		}

		if((b.Length % 4) == 1) { throw new FormatException("Invalid Base64 string length."); }
		while((b.Length % 4) != 0) { b.Append('='); }

		return Convert.FromBase64String(b.ToString());
	}


	/// <inheritdoc/>
	public override byte[] Encode(byte[] data)
	{
		return Encode(data, false);
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return DecodeToBytes(Encoding.GetString(data));
	}
}
