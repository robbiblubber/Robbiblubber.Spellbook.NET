namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class provides a base implementation for encoders.</summary>
public abstract class EncoderBase: IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoding">Encoding that converts between strings and bytes; NULL for UTF-8.</param>
	protected EncoderBase(Encoding? encoding = null)
	{
		if(encoding is not null) { Encoding = encoding; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Fits decoded data into an array of a fixed length.</summary>
	/// <param name="data">Data.</param>
	/// <param name="len">Array length.</param>
	/// <returns>Returns the data aligned to the end of an array of the given length.</returns>
	/// <exception cref="FormatException">Thrown when the data is longer than the array.</exception>
	internal static byte[] _Fit(byte[] data, int len)
	{
		int extra = (data.Length - len);
		for(int i = 0; i < extra; i++)
		{
			if(data[i] != 0) { throw new FormatException("The decoded value is longer than " + len + " bytes."); }
		}

		if(extra == 0) { return data; }

		byte[] rval = new byte[len];
		if(extra > 0)
		{
			Array.Copy(data, extra, rval, 0, len);
		}
		else { Array.Copy(data, 0, rval, -extra, data.Length); }

		return rval;
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeBytes(IEnumerable{byte})"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Bytes.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeBytes(IEncoder encoder, IEnumerable<byte> value)
	{
		return encoder.Encoding.GetString(encoder.Encode(value.ToArray()));
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeChars(IEnumerable{char})"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Characters.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeChars(IEncoder encoder, IEnumerable<char> value)
	{
		return encoder.EncodeBytes(encoder.Encoding.GetBytes(value.ToArray()));
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeString(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">String.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeString(IEncoder encoder, string value)
	{
		return encoder.EncodeBytes(encoder.Encoding.GetBytes(value));
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeInt(int)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Integer.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeInt(IEncoder encoder, int value)
	{
		return encoder.EncodeBytes(BitConverter.GetBytes(value));
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeLong(long)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Long integer.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeLong(IEncoder encoder, long value)
	{
		return encoder.EncodeBytes(BitConverter.GetBytes(value));
	}


	/// <summary>Default implementation of <see cref="IEncoder.EncodeDouble(double)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="value">Double.</param>
	/// <returns>Returns the encoded string.</returns>
	internal static string _EncodeDouble(IEncoder encoder, double value)
	{
		return encoder.EncodeBytes(BitConverter.GetBytes(value));
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToBytes(string,int)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <param name="len">Array length.</param>
	/// <returns>Returns the decoded bytes, aligned to the end of the array.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid or the decoded data is longer than the array.</exception>
	internal static byte[] _DecodeToBytes(IEncoder encoder, string s, int len)
	{
		return _Fit(encoder.DecodeToBytes(s), len);
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToBytes(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded bytes.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static byte[] _DecodeToBytes(IEncoder encoder, string s)
	{
		return encoder.Decode(encoder.Encoding.GetBytes(s));
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToChars(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded characters.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static char[] _DecodeToChars(IEncoder encoder, string s)
	{
		return encoder.DecodeToString(s).ToCharArray();
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToString(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded string.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static string _DecodeToString(IEncoder encoder, string s)
	{
		return encoder.Encoding.GetString(encoder.DecodeToBytes(s));
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToInt(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static int _DecodeToInt(IEncoder encoder, string s)
	{
		return BitConverter.ToInt32(encoder.DecodeToBytes(s, 4), 0);
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToLong(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded long integer.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static long _DecodeToLong(IEncoder encoder, string s)
	{
		return BitConverter.ToInt64(encoder.DecodeToBytes(s, 8), 0);
	}


	/// <summary>Default implementation of <see cref="IEncoder.DecodeToDouble(string)"/>.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="s">Encoded string.</param>
	/// <returns>Returns the decoded double.</returns>
	/// <exception cref="FormatException">Thrown when the string is invalid.</exception>
	internal static double _DecodeToDouble(IEncoder encoder, string s)
	{
		return BitConverter.ToDouble(encoder.DecodeToBytes(s, 8), 0);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads a file into an encoded string.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Returns the encoded string.</returns>
	public string EncodeFile(string fileName)
	{
		return EncodeBytes(File.ReadAllBytes(fileName));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEncoder                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual Encoding Encoding
	{
		get; protected set;
	} = Encoding.UTF8;


	/// <inheritdoc/>
	public virtual string EncodeBytes(IEnumerable<byte> value)
	{
		return _EncodeBytes(this, value);
	}


	/// <inheritdoc/>
	public virtual string EncodeChars(IEnumerable<char> value)
	{
		return _EncodeChars(this, value);
	}


	/// <inheritdoc/>
	public virtual string EncodeString(string value)
	{
		return _EncodeString(this, value);
	}


	/// <inheritdoc/>
	public virtual string EncodeInt(int value)
	{
		return _EncodeInt(this, value);
	}


	/// <inheritdoc/>
	public virtual string EncodeLong(long value)
	{
		return _EncodeLong(this, value);
	}


	/// <inheritdoc/>
	public virtual string EncodeDouble(double value)
	{
		return _EncodeDouble(this, value);
	}


	/// <inheritdoc/>
	public virtual byte[] DecodeToBytes(string s, int len)
	{
		return _DecodeToBytes(this, s, len);
	}


	/// <inheritdoc/>
	public virtual byte[] DecodeToBytes(string s)
	{
		return _DecodeToBytes(this, s);
	}


	/// <inheritdoc/>
	public virtual char[] DecodeToChars(string s)
	{
		return _DecodeToChars(this, s);
	}


	/// <inheritdoc/>
	public virtual string DecodeToString(string s)
	{
		return _DecodeToString(this, s);
	}


	/// <inheritdoc/>
	public virtual int DecodeToInt(string s)
	{
		return _DecodeToInt(this, s);
	}


	/// <inheritdoc/>
	public virtual long DecodeToLong(string s)
	{
		return _DecodeToLong(this, s);
	}


	/// <inheritdoc/>
	public virtual double DecodeToDouble(string s)
	{
		return _DecodeToDouble(this, s);
	}


	/// <inheritdoc/>
	public abstract byte[] Encode(byte[] data);


	/// <inheritdoc/>
	public abstract byte[] Decode(byte[] data);
}
