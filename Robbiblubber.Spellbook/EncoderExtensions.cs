namespace Robbiblubber.Spellbook;



/// <summary>This class provides extension methods for encoding and decoding.</summary>
public static class EncoderExtensions
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Decodes a string into a value of a type.</summary>
	/// <typeparam name="T">Type; strings, byte arrays, character arrays, integer types, float and double are supported.</typeparam>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	private static T _Decode<T>(IEncoder encoder, string value)
	{
		Type t = typeof(T);
		object rval;

		if(t == typeof(string))
		{
			rval = encoder.DecodeToString(value);
		}
		else if(t == typeof(byte[]))
		{
			rval = encoder.DecodeToBytes(value);
		}
		else if(t == typeof(char[]))
		{
			rval = encoder.DecodeToChars(value);
		}
		else
		{
			switch(Type.GetTypeCode(t))
			{
				case TypeCode.SByte:    rval = unchecked((sbyte) encoder.DecodeToInt(value)); break;
				case TypeCode.Byte:     rval = unchecked((byte) encoder.DecodeToInt(value)); break;
				case TypeCode.Int16:    rval = unchecked((short) encoder.DecodeToInt(value)); break;
				case TypeCode.UInt16:   rval = unchecked((ushort) encoder.DecodeToInt(value)); break;
				case TypeCode.Int32:    rval = encoder.DecodeToInt(value); break;
				case TypeCode.UInt32:   rval = unchecked((uint) encoder.DecodeToInt(value)); break;
				case TypeCode.Int64:    rval = encoder.DecodeToLong(value); break;
				case TypeCode.UInt64:   rval = unchecked((ulong) encoder.DecodeToLong(value)); break;
				case TypeCode.Single:   rval = (float) encoder.DecodeToDouble(value); break;
				case TypeCode.Double:   rval = encoder.DecodeToDouble(value); break;
				default:
					throw new ArgumentException("Cannot decode a string to type " + t.Name + ".", nameof(T));
			}
		}

		return (T) rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a string to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this string value)
	{
		return Base62.DEFAULT.EncodeString(value);
	}


	/// <summary>Converts bytes to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this byte[] value)
	{
		return Base62.DEFAULT.EncodeBytes(value);
	}


	/// <summary>Converts characters to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this char[] value)
	{
		return Base62.DEFAULT.EncodeChars(value);
	}


	/// <summary>Converts a signed byte to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this sbyte value)
	{
		return Base62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a byte to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this byte value)
	{
		return Base62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a short integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this short value)
	{
		return Base62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned short integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this ushort value)
	{
		return Base62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this int value)
	{
		return Base62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this uint value)
	{
		return Base62.DEFAULT.EncodeInt(unchecked((int) value));
	}


	/// <summary>Converts a long integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this long value)
	{
		return Base62.DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts an unsigned long integer to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this ulong value)
	{
		return Base62.DEFAULT.EncodeLong(unchecked((long) value));
	}


	/// <summary>Converts a float to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this float value)
	{
		return Base62.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a double to a Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase62(this double value)
	{
		return Base62.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a Base62 string to a string.</summary>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string FromBase62(this string value)
	{
		return Base62.DEFAULT.DecodeToString(value);
	}


	/// <summary>Converts a Base62 string to a value.</summary>
	/// <typeparam name="T">Type; strings, byte arrays, character arrays, integer types, float and double are supported.</typeparam>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static T FromBase62<T>(this string value)
	{
		return _Decode<T>(Base62.DEFAULT, value);
	}


	/// <summary>Converts a string to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this string value)
	{
		return InvertedBase62.DEFAULT.EncodeString(value);
	}


	/// <summary>Converts bytes to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this byte[] value)
	{
		return InvertedBase62.DEFAULT.EncodeBytes(value);
	}


	/// <summary>Converts characters to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this char[] value)
	{
		return InvertedBase62.DEFAULT.EncodeChars(value);
	}


	/// <summary>Converts a signed byte to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this sbyte value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a byte to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this byte value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a short integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this short value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned short integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this ushort value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this int value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this uint value)
	{
		return InvertedBase62.DEFAULT.EncodeInt(unchecked((int) value));
	}


	/// <summary>Converts a long integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this long value)
	{
		return InvertedBase62.DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts an unsigned long integer to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this ulong value)
	{
		return InvertedBase62.DEFAULT.EncodeLong(unchecked((long) value));
	}


	/// <summary>Converts a float to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this float value)
	{
		return InvertedBase62.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a double to an inverted Base62 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToInvertedBase62(this double value)
	{
		return InvertedBase62.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts an inverted Base62 string to a string.</summary>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string FromInvertedBase62(this string value)
	{
		return InvertedBase62.DEFAULT.DecodeToString(value);
	}


	/// <summary>Converts an inverted Base62 string to a value.</summary>
	/// <typeparam name="T">Type; strings, byte arrays, character arrays, integer types, float and double are supported.</typeparam>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static T FromInvertedBase62<T>(this string value)
	{
		return _Decode<T>(InvertedBase62.DEFAULT, value);
	}


	/// <summary>Converts a string to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this string value, bool pad = false)
	{
		return Base64.DEFAULT.EncodeString(value, pad);
	}


	/// <summary>Converts bytes to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this byte[] value, bool pad = false)
	{
		return Base64.DEFAULT.EncodeBytes(value, pad);
	}


	/// <summary>Converts characters to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <param name="pad">Determines if the result is padded with equals signs.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this char[] value, bool pad = false)
	{
		return Base64.DEFAULT.EncodeChars(value, pad);
	}


	/// <summary>Converts a signed byte to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this sbyte value)
	{
		return Base64.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a byte to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this byte value)
	{
		return Base64.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a short integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this short value)
	{
		return Base64.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned short integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this ushort value)
	{
		return Base64.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this int value)
	{
		return Base64.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this uint value)
	{
		return Base64.DEFAULT.EncodeInt(unchecked((int) value));
	}


	/// <summary>Converts a long integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this long value)
	{
		return Base64.DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts an unsigned long integer to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this ulong value)
	{
		return Base64.DEFAULT.EncodeLong(unchecked((long) value));
	}


	/// <summary>Converts a float to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this float value)
	{
		return Base64.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a double to a Base64 string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToBase64(this double value)
	{
		return Base64.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a Base64 string to a string.</summary>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string FromBase64(this string value)
	{
		return Base64.DEFAULT.DecodeToString(value);
	}


	/// <summary>Converts a Base64 string to a value.</summary>
	/// <typeparam name="T">Type; strings, byte arrays, character arrays, integer types, float and double are supported.</typeparam>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static T FromBase64<T>(this string value)
	{
		return _Decode<T>(Base64.DEFAULT, value);
	}


	/// <summary>Converts a string to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this string value)
	{
		return Hex.DEFAULT.EncodeString(value);
	}


	/// <summary>Converts bytes to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this byte[] value)
	{
		return Hex.DEFAULT.EncodeBytes(value);
	}


	/// <summary>Converts characters to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this char[] value)
	{
		return Hex.DEFAULT.EncodeChars(value);
	}


	/// <summary>Converts a signed byte to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this sbyte value)
	{
		return Hex.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a byte to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this byte value)
	{
		return Hex.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts a short integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this short value)
	{
		return Hex.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned short integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this ushort value)
	{
		return Hex.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this int value)
	{
		return Hex.DEFAULT.EncodeInt(value);
	}


	/// <summary>Converts an unsigned integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this uint value)
	{
		return Hex.DEFAULT.EncodeInt(unchecked((int) value));
	}


	/// <summary>Converts a long integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this long value)
	{
		return Hex.DEFAULT.EncodeLong(value);
	}


	/// <summary>Converts an unsigned long integer to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this ulong value)
	{
		return Hex.DEFAULT.EncodeLong(unchecked((long) value));
	}


	/// <summary>Converts a float to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this float value)
	{
		return Hex.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a double to a hexadecimal string.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string ToHex(this double value)
	{
		return Hex.DEFAULT.EncodeDouble(value);
	}


	/// <summary>Converts a hexadecimal string to a string.</summary>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string FromHex(this string value)
	{
		return Hex.DEFAULT.DecodeToString(value);
	}


	/// <summary>Converts a hexadecimal string to a value.</summary>
	/// <typeparam name="T">Type; strings, byte arrays, character arrays, integer types, float and double are supported.</typeparam>
	/// <param name="value">Encoded string.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static T FromHex<T>(this string value)
	{
		return _Decode<T>(Hex.DEFAULT, value);
	}


	/// <summary>Compresses a string.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the compressed string in inverted Base62 encoding.</returns>
	public static string Compress(this string text)
	{
		return GZip.Compress(text, InvertedBase62.DEFAULT);
	}


	/// <summary>Decompresses a string.</summary>
	/// <param name="data">Compressed string in inverted Base62 encoding.</param>
	/// <returns>Returns the decompressed text.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	public static string Decompress(this string data)
	{
		return GZip.Decompress(data, InvertedBase62.DEFAULT);
	}
}
