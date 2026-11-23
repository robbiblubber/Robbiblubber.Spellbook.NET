namespace Robbiblubber.Spellbook;

using System.Globalization;
using System.Text;



/// <summary>This class implements hexadecimal encoding.</summary>
public sealed class Hex: EncoderBase, IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hexadecimal alphabet.</summary>
	public const string ALPHABET = "0123456789abcdef";

	/// <summary>Default instance.</summary>
	public static readonly Hex DEFAULT = new();

	/// <summary>Instance that writes upper case letters.</summary>
	public static readonly Hex UPPER = new(null, true);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public Hex(): this(null)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoding">Encoding that converts between strings and bytes; NULL for UTF-8.</param>
	/// <param name="upperCase">Determines if upper case letters are written.</param>
	public Hex(Encoding? encoding, bool upperCase = false): base(encoding)
	{
		UpperCase = upperCase;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to a hexadecimal string.</summary>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromBytes(IEnumerable<byte> value)
	{
		return DEFAULT.EncodeBytes(value);
	}


	/// <summary>Converts characters to a hexadecimal string.</summary>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromChars(IEnumerable<char> value)
	{
		return DEFAULT.EncodeChars(value);
	}


	/// <summary>Converts a string to a hexadecimal string.</summary>
	/// <param name="value">String.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromString(string value)
	{
		return DEFAULT.EncodeString(value);
	}


	/// <summary>Converts an integer to a hexadecimal string.</summary>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromInt(int value)
	{
		return DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a long integer to a hexadecimal string.</summary>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromLong(long value)
	{
		return DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts a double to a hexadecimal string.</summary>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromDouble(double value)
	{
		return DEFAULT.EncodeDouble(value);
	}


	/// <summary>Reads a file into a hexadecimal string.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string FromFile(string fileName)
	{
		return DEFAULT.EncodeFile(fileName);
	}


	/// <summary>Converts a hexadecimal string to bytes.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded bytes.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static byte[] ToBytes(string s)
	{
		return DEFAULT.DecodeToBytes(s);
	}


	/// <summary>Converts a hexadecimal string to characters.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded characters.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static char[] ToChars(string s)
	{
		return DEFAULT.DecodeToChars(s);
	}


	/// <summary>Converts a hexadecimal string to a string.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string Decode(string s)
	{
		return DEFAULT.DecodeToString(s);
	}


	/// <summary>Converts a hexadecimal string to an integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static int ToInt(string s)
	{
		return DEFAULT.DecodeToInt(s);
	}


	/// <summary>Converts a hexadecimal string to a long integer.</summary>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded long integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static long ToLong(string s)
	{
		return DEFAULT.DecodeToLong(s);
	}


	/// <summary>Converts a hexadecimal string to a double.</summary>
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

	/// <summary>Gets if upper case letters are written. Both cases are read.</summary>
	public bool UpperCase
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a byte to a hexadecimal string.</summary>
	/// <param name="value">Byte.</param>
	/// <param name="pad">Determines if the result is padded to two digits.</param>
	/// <returns>Returns the hexadecimal string.</returns>
	public string EncodeByte(byte value, bool pad = true)
	{
		return value.ToString((UpperCase ? "X" : "x") + (pad ? "2" : ""), CultureInfo.InvariantCulture);
	}


	/// <summary>Converts a character to a hexadecimal string.</summary>
	/// <param name="value">Character.</param>
	/// <param name="pad">Determines if the result is padded to two digits, or four digits for characters above 0xFF.</param>
	/// <returns>Returns the hexadecimal string.</returns>
	public string EncodeChar(char value, bool pad = true)
	{
		return ((ushort) value).ToString((UpperCase ? "X" : "x") + (pad ? ((value > 0xff) ? "4" : "2") : ""), CultureInfo.InvariantCulture);
	}


	/// <summary>Converts a hexadecimal string to a byte.</summary>
	/// <param name="s">Hexadecimal string of up to two digits.</param>
	/// <returns>Returns the byte.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public byte DecodeToByte(string s)
	{
		if(!byte.TryParse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out byte rval)) { throw new FormatException("Invalid hexadecimal byte '" + s + "'."); }
		return rval;
	}


	/// <summary>Converts a hexadecimal string to a character.</summary>
	/// <param name="s">Hexadecimal string of up to four digits.</param>
	/// <returns>Returns the character.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public char DecodeToChar(string s)
	{
		if(!ushort.TryParse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort rval)) { throw new FormatException("Invalid hexadecimal character '" + s + "'."); }
		return (char) rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EncoderBase                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string EncodeBytes(IEnumerable<byte> value)
	{
		string rval = Convert.ToHexString(value.ToArray());
		return (UpperCase ? rval : rval.ToLowerInvariant());
	}


	/// <inheritdoc/>
	public override byte[] DecodeToBytes(string s)
	{
		if((s.Length % 2) != 0) { throw new FormatException("A hexadecimal string must have an even number of digits."); }
		return Convert.FromHexString(s);
	}


	/// <inheritdoc/>
	public override byte[] Encode(byte[] data)
	{
		return Encoding.GetBytes(EncodeBytes(data));
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return DecodeToBytes(Encoding.GetString(data));
	}
}
