namespace Robbiblubber.Spellbook;

using System.Globalization;
using System.Text;



/// <summary>This class implements an encoder that does not change text.</summary>
/// <remarks>Strings are returned as they are, numbers are written in invariant culture. Bytes are converted with the encoding,
///          so binary data that is not valid text does not survive a round trip.</remarks>
public sealed class PlainText: IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default instance.</summary>
	public static readonly PlainText DEFAULT = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public PlainText()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoding">Encoding that converts between strings and bytes.</param>
	public PlainText(Encoding encoding)
	{
		Encoding = encoding;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEncoder                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public Encoding Encoding
	{
		get;
	} = Encoding.UTF8;


	/// <inheritdoc/>
	public string EncodeBytes(IEnumerable<byte> value)
	{
		return Encoding.GetString(value.ToArray());
	}


	/// <inheritdoc/>
	public string EncodeChars(IEnumerable<char> value)
	{
		return new string(value.ToArray());
	}


	/// <inheritdoc/>
	public string EncodeString(string value)
	{
		return value;
	}


	/// <inheritdoc/>
	public string EncodeInt(int value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public string EncodeLong(long value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public string EncodeDouble(double value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public byte[] DecodeToBytes(string s, int len)
	{
		return EncoderBase._Fit(DecodeToBytes(s), len);
	}


	/// <inheritdoc/>
	public byte[] DecodeToBytes(string s)
	{
		return Encoding.GetBytes(s);
	}


	/// <inheritdoc/>
	public char[] DecodeToChars(string s)
	{
		return s.ToCharArray();
	}


	/// <inheritdoc/>
	public string DecodeToString(string s)
	{
		return s;
	}


	/// <inheritdoc/>
	public int DecodeToInt(string s)
	{
		return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public long DecodeToLong(string s)
	{
		return long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public double DecodeToDouble(string s)
	{
		return double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
	}


	/// <inheritdoc/>
	public byte[] Encode(byte[] data)
	{
		return data;
	}


	/// <inheritdoc/>
	public byte[] Decode(byte[] data)
	{
		return data;
	}
}
