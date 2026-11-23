namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class implements Base62 encoding with lower case letters before upper case letters.</summary>
public sealed class InvertedBase62: BaseN, IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Inverted Base62 alphabet.</summary>
	public const string ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

	/// <summary>Default instance.</summary>
	public static readonly InvertedBase62 DEFAULT = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public InvertedBase62(): base(62, ALPHABET)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoding">Encoding that converts between strings and bytes.</param>
	public InvertedBase62(Encoding encoding): base(62, ALPHABET, encoding)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to an inverted Base62 string.</summary>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromBytes(IEnumerable<byte> value)
	{
		return DEFAULT.EncodeBytes(value);
	}


	/// <summary>Converts characters to an inverted Base62 string.</summary>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromChars(IEnumerable<char> value)
	{
		return DEFAULT.EncodeChars(value);
	}


	/// <summary>Converts a string to an inverted Base62 string.</summary>
	/// <param name="value">String.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromString(string value)
	{
		return DEFAULT.EncodeString(value);
	}


	/// <summary>Converts an integer to an inverted Base62 string.</summary>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromInt(int value)
	{
		return DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a long integer to an inverted Base62 string.</summary>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromLong(long value)
	{
		return DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts a double to an inverted Base62 string.</summary>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromDouble(double value)
	{
		return DEFAULT.EncodeDouble(value);
	}


	/// <summary>Reads a file into an inverted Base62 string.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromFile(string fileName)
	{
		return DEFAULT.EncodeFile(fileName);
	}


	/// <summary>Converts an inverted Base62 string to bytes.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded bytes.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static byte[] ToBytes(string s)
	{
		return DEFAULT.DecodeToBytes(s);
	}


	/// <summary>Converts an inverted Base62 string to characters.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded characters.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static char[] ToChars(string s)
	{
		return DEFAULT.DecodeToChars(s);
	}


	/// <summary>Converts an inverted Base62 string to a string.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string Decode(string s)
	{
		return DEFAULT.DecodeToString(s);
	}


	/// <summary>Converts an inverted Base62 string to an integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static int ToInt(string s)
	{
		return DEFAULT.DecodeToInt(s);
	}


	/// <summary>Converts an inverted Base62 string to a long integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded long integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static long ToLong(string s)
	{
		return DEFAULT.DecodeToLong(s);
	}


	/// <summary>Converts an inverted Base62 string to a double.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded double.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static double ToDouble(string s)
	{
		return DEFAULT.DecodeToDouble(s);
	}
}
