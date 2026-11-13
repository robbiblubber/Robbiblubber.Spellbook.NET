namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class implements the FunnelCake obfuscation algorithm.</summary>
/// <remarks><para>FunnelCake combines the data with a key stream derived from a secret by XOR. It is simple, fast and easy to
///          port to other platforms, but it is not an encryption that withstands a determined attacker: the key stream is the same
///          for every message with the same secret, the data is not protected against modification, and the secret is not
///          strengthened against guessing. Use it to keep casual readers out of configuration files and similar data. For data
///          that has to be protected, use <see cref="AESGCM"/>.</para>
///          <para>The output is identical to all earlier versions.</para></remarks>
public sealed class FunnelCake: EncoderBase, IEncrypter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Seed string.</summary>
	private const string _SEED = "ZDQwZGI1YmFhNDVlNTM5ZTRmM2JiYjU4ZGQ1OGNjZDJkZDNiMjJiNThlYTBmYjE4MjdlOWNkNGFhM2U3Zjk5NQ";

	/// <summary>Default secret.</summary>
	private const string _DEFAULT_SECRET = "hUZ6wwgC20Qy";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instance using Base64 encoding and the default secret.</summary>
	public static readonly FunnelCake BASE64 = new(Base64.DEFAULT);

	/// <summary>Instance using Base62 encoding and the default secret.</summary>
	public static readonly FunnelCake BASE62 = new(Base62.DEFAULT);

	/// <summary>Instance using inverted Base62 encoding and the default secret.</summary>
	public static readonly FunnelCake INVERTED_BASE62 = new(InvertedBase62.DEFAULT);

	/// <summary>Instance using hexadecimal encoding and the default secret.</summary>
	public static readonly FunnelCake HEX = new(Hex.DEFAULT);

	/// <summary>Instance using plain text and the default secret.</summary>
	/// <remarks>Only text that results in valid characters can be written; see <see cref="Encrypt(string, string, IEncoder)"/>.</remarks>
	public static readonly FunnelCake PLAINTEXT = new(PlainText.DEFAULT);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hash that generates the key stream.</summary>
	private static readonly SHA256 _Hash = new(new BaseN(92));



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Secret.</summary>
	private readonly string _Secret;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="encoder">Encoder that converts the obfuscated data to text; NULL for plain text.</param>
	/// <param name="secret">Secret; NULL for the default secret.</param>
	public FunnelCake(IEncoder? encoder = null, string? secret = null)
	{
		Encoder = (encoder ?? PlainText.DEFAULT);
		_Secret = (secret ?? _DEFAULT_SECRET);
	}


	/// <summary>Creates a new instance of this class that writes plain text.</summary>
	/// <param name="secret">Secret.</param>
	public FunnelCake(string secret): this(null, secret)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Obfuscates data.</summary>
	/// <param name="data">Data.</param>
	/// <param name="secret">Secret.</param>
	/// <returns>Returns the obfuscated data.</returns>
	public static byte[] Encrypt(byte[] data, string secret)
	{
		byte[] rval = new byte[data.Length];
		string seed = _SEED;
		int pos = 0;

		while(pos < data.Length)
		{
			seed = _Hash.HashString(secret + seed);
			for(int i = 0; (i < seed.Length) && (pos < data.Length); i++, pos++) { rval[pos] = (byte) (data[pos] ^ seed[i]); }
		}

		return rval;
	}


	/// <summary>Obfuscates a string and writes it as plain text.</summary>
	/// <param name="s">String.</param>
	/// <param name="secret">Secret.</param>
	/// <returns>Returns the obfuscated string.</returns>
	/// <exception cref="ArgumentException">Thrown when the result contains bytes that are not valid characters.</exception>
	public static string Encrypt(string s, string secret)
	{
		return Encrypt(s, secret, PlainText.DEFAULT);
	}


	/// <summary>Obfuscates a string.</summary>
	/// <param name="s">String.</param>
	/// <param name="secret">Secret.</param>
	/// <param name="encoder">Encoder that converts the obfuscated data to text.</param>
	/// <returns>Returns the obfuscated string.</returns>
	/// <exception cref="ArgumentException">Thrown when the encoder is <see cref="PlainText"/> and the result contains bytes that
	///                                     are not valid characters. This happens for most text with non-ASCII characters; use
	///                                     another encoder for such text.</exception>
	public static string Encrypt(string s, string secret, IEncoder encoder)
	{
		byte[] data = Encrypt(encoder.Encoding.GetBytes(s), secret);
		return _Check(encoder, encoder.EncodeBytes(data), data);
	}


	/// <summary>Restores obfuscated data.</summary>
	/// <param name="data">Obfuscated data.</param>
	/// <param name="secret">Secret.</param>
	/// <returns>Returns the data.</returns>
	public static byte[] Decrypt(byte[] data, string secret)
	{
		return Encrypt(data, secret);
	}


	/// <summary>Restores an obfuscated string.</summary>
	/// <param name="s">Obfuscated string.</param>
	/// <param name="secret">Secret.</param>
	/// <param name="encoder">Encoder that has been used to convert the obfuscated data to text.</param>
	/// <returns>Returns the string.</returns>
	public static string Decrypt(string s, string secret, IEncoder encoder)
	{
		return encoder.Encoding.GetString(Decrypt(encoder.DecodeToBytes(s), secret));
	}


	/// <summary>Restores an obfuscated string that has been written as plain text.</summary>
	/// <param name="s">Obfuscated string.</param>
	/// <param name="secret">Secret.</param>
	/// <returns>Returns the string.</returns>
	public static string Decrypt(string s, string secret)
	{
		return Decrypt(s, secret, PlainText.DEFAULT);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Checks that plain text output can be restored.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="text">Text.</param>
	/// <param name="data">Obfuscated data.</param>
	/// <returns>Returns the text.</returns>
	/// <exception cref="ArgumentException">Thrown when the text cannot be restored.</exception>
	private static string _Check(IEncoder encoder, string text, byte[] data)
	{
		if((encoder is PlainText) && (!encoder.Encoding.GetBytes(text).AsSpan().SequenceEqual(data)))
		{
			throw new ArgumentException("The obfuscated data cannot be written as plain text. Use another encoder, such as Base64.");
		}

		return text;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEncrypter                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public IEncoder Encoder
	{
		get;
	}


	/// <inheritdoc/>
	public byte[] Encrypt(byte[] data)
	{
		return Encrypt(data, _Secret);
	}


	/// <inheritdoc/>
	public byte[] Decrypt(byte[] data)
	{
		return Encrypt(data, _Secret);
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
	/// <exception cref="ArgumentException">Thrown when the encoder is <see cref="PlainText"/> and the result contains bytes that
	///                                     are not valid characters.</exception>
	public override string EncodeBytes(IEnumerable<byte> value)
	{
		byte[] data = Encode(value.ToArray());
		return _Check(Encoder, Encoding.GetString(data), data);
	}


	/// <inheritdoc/>
	public override byte[] Encode(byte[] data)
	{
		return Encoder.Encode(Encrypt(data, _Secret));
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return Decrypt(Encoder.Decode(data), _Secret);
	}
}
