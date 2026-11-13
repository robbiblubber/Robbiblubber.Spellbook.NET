namespace Robbiblubber.Spellbook;

using System.IO.Compression;
using System.Text;



/// <summary>This class implements GZip compression combined with an encoder.</summary>
/// <remarks>Byte data is compressed and then encoded with the encoder. Strings are compressed and encoded into text; with
///          <see cref="PlainText"/>, which cannot represent compressed data as text, strings are encoded in Base64.</remarks>
public sealed class GZip: EncoderBase, IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instance using Base64 encoding.</summary>
	public static readonly GZip BASE64 = new(Base64.DEFAULT);

	/// <summary>Instance using Base62 encoding.</summary>
	public static readonly GZip BASE62 = new(Base62.DEFAULT);

	/// <summary>Instance using inverted Base62 encoding.</summary>
	public static readonly GZip INVERTED_BASE62 = new(InvertedBase62.DEFAULT);

	/// <summary>Instance using hexadecimal encoding.</summary>
	public static readonly GZip HEX = new(Hex.DEFAULT);

	/// <summary>Instance without encoding; strings are encoded in Base64.</summary>
	public static readonly GZip PLAINTEXT = new(PlainText.DEFAULT);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoder">Encoder for the compressed data; NULL for no encoding.</param>
	public GZip(IEncoder? encoder = null)
	{
		Encoder = (encoder ?? PlainText.DEFAULT);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Compresses data.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the compressed data.</returns>
	public static byte[] Compress(byte[] data)
	{
		using(MemoryStream rval = new())
		{
			using(GZipStream gz = new(rval, CompressionMode.Compress, true))
			{
				gz.Write(data, 0, data.Length);
			}
			return rval.ToArray();
		}
	}


	/// <summary>Decompresses data.</summary>
	/// <param name="data">Compressed data.</param>
	/// <returns>Returns the decompressed data.</returns>
	/// <exception cref="InvalidDataException">Thrown when the data is not valid GZip data.</exception>
	public static byte[] Decompress(byte[] data)
	{
		using(MemoryStream rval = new())
		using(MemoryStream src = new(data))
		using(GZipStream gz = new(src, CompressionMode.Decompress))
		{
			gz.CopyTo(rval);
			return rval.ToArray();
		}
	}


	/// <summary>Compresses and encodes data.</summary>
	/// <param name="data">Data.</param>
	/// <param name="encoder">Encoder.</param>
	/// <returns>Returns the encoded data.</returns>
	public static byte[] Compress(byte[] data, IEncoder encoder)
	{
		return new GZip(encoder).Encode(data);
	}


	/// <summary>Decodes and decompresses data.</summary>
	/// <param name="data">Encoded data.</param>
	/// <param name="encoder">Encoder.</param>
	/// <returns>Returns the decompressed data.</returns>
	public static byte[] Decompress(byte[] data, IEncoder encoder)
	{
		return new GZip(encoder).Decode(data);
	}


	/// <summary>Compresses a string.</summary>
	/// <param name="s">String.</param>
	/// <param name="encoder">Encoder; NULL or <see cref="PlainText"/> for Base64.</param>
	/// <returns>Returns the encoded string.</returns>
	public static string Compress(string s, IEncoder? encoder = null)
	{
		return new GZip(encoder).EncodeString(s);
	}


	/// <summary>Decompresses a string.</summary>
	/// <param name="s">Encoded string.</param>
	/// <param name="encoder">Encoder; NULL or <see cref="PlainText"/> for Base64.</param>
	/// <returns>Returns the decompressed string.</returns>
	public static string Decompress(string s, IEncoder? encoder = null)
	{
		return new GZip(encoder).DecodeToString(s);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the encoder for the compressed data.</summary>
	public IEncoder Encoder
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the encoder that converts compressed data to text.</summary>
	private IEncoder _TextEncoder
	{
		get { return ((Encoder is PlainText) ? Base64.DEFAULT : Encoder); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EncoderBase                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override Encoding Encoding
	{
		get { return Encoder.Encoding; }
	}


	/// <inheritdoc/>
	public override string EncodeBytes(IEnumerable<byte> value)
	{
		return _TextEncoder.EncodeBytes(Compress(value.ToArray()));
	}


	/// <inheritdoc/>
	public override byte[] DecodeToBytes(string s)
	{
		return Decompress(_TextEncoder.DecodeToBytes(s));
	}


	/// <inheritdoc/>
	public override byte[] Encode(byte[] data)
	{
		return Encoder.Encode(Compress(data));
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return Decompress(Encoder.Decode(data));
	}
}
