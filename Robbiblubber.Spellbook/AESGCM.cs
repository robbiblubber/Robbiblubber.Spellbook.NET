namespace Robbiblubber.Spellbook;

using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;



/// <summary>This class encrypts data with AES-GCM.</summary>
/// <remarks><para>Every message gets a random nonce, so the same data never results in the same output, and an authentication
///          tag, so modified data or a wrong key is detected on decryption. The key is either given directly (16, 24 or 32 bytes)
///          or derived from a password with PBKDF2-SHA256 and a random salt.</para>
///          <para>Binary format, all integers big-endian:</para>
///          <list type="bullet">
///             <item>Key: <c>0x01 | nonce (12) | ciphertext | tag (16)</c>.</item>
///             <item>Password: <c>0x02 | iterations (4) | salt (16) | nonce (12) | ciphertext | tag (16)</c>; the key is
///                   PBKDF2-SHA256(UTF-8 password, salt, iterations, 32 bytes).</item>
///          </list>
///          <para>The bytes before the nonce are passed to AES-GCM as associated data. The format can be read with any AES-GCM
///          implementation, e.g. WebCrypto, OpenSSL or Java.</para></remarks>
public sealed class AESGCM: EncoderBase, IEncrypter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default number of PBKDF2 iterations for passwords (OWASP recommendation).</summary>
	public const int DEFAULT_ITERATIONS = 600_000;

	/// <summary>Maximum number of PBKDF2 iterations accepted when decrypting.</summary>
	public const int MAX_ITERATIONS = 10_000_000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Format identifier for data encrypted with a key.</summary>
	private const byte _FORMAT_KEY = 1;

	/// <summary>Format identifier for data encrypted with a password.</summary>
	private const byte _FORMAT_PASSWORD = 2;

	/// <summary>Nonce size in bytes.</summary>
	private const int _NONCE_SIZE = 12;

	/// <summary>Tag size in bytes.</summary>
	private const int _TAG_SIZE = 16;

	/// <summary>Salt size in bytes.</summary>
	private const int _SALT_SIZE = 16;

	/// <summary>Maximum number of derived keys kept for decryption.</summary>
	private const int _MAX_CACHED_KEYS = 32;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Key used for encryption.</summary>
	private readonly byte[] _Key;

	/// <summary>Header written before the nonce.</summary>
	private readonly byte[] _Header;

	/// <summary>Password; NULL if the instance uses a key.</summary>
	private readonly string? _Password;

	/// <summary>Keys derived from the password, by header.</summary>
	private readonly ConcurrentDictionary<string, byte[]> _Keys = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class that uses a key.</summary>
	/// <param name="key">Key; 16, 24 or 32 bytes. Use <see cref="GenerateKey(int)"/> to create one.</param>
	/// <param name="encoder">Encoder that converts encrypted data to text; NULL for Base64.</param>
	/// <exception cref="ArgumentException">Thrown when the key size is invalid or the encoder is <see cref="PlainText"/>.</exception>
	public AESGCM(byte[] key, IEncoder? encoder = null)
	{
		if((key.Length != 16) && (key.Length != 24) && (key.Length != 32)) { throw new ArgumentException("The key must have 16, 24 or 32 bytes.", nameof(key)); }

		Encoder = _GetEncoder(encoder);
		_Key = (byte[]) key.Clone();
		_Header = [_FORMAT_KEY];
	}


	/// <summary>Creates a new instance of this class that uses a password.</summary>
	/// <param name="password">Password.</param>
	/// <param name="encoder">Encoder that converts encrypted data to text; NULL for Base64.</param>
	/// <param name="iterations">Number of PBKDF2 iterations; from 1 to <see cref="MAX_ITERATIONS"/>.</param>
	/// <exception cref="ArgumentException">Thrown when the number of iterations is invalid or the encoder is
	///                                     <see cref="PlainText"/>.</exception>
	/// <remarks>The key is derived once with a random salt; all data encrypted by this instance uses that salt. Data with
	///          other salts is decrypted as well, the keys are derived as needed and cached.</remarks>
	public AESGCM(string password, IEncoder? encoder = null, int iterations = DEFAULT_ITERATIONS)
	{
		if((iterations < 1) || (iterations > MAX_ITERATIONS)) { throw new ArgumentException("The number of iterations must be between 1 and " + MAX_ITERATIONS + ".", nameof(iterations)); }

		Encoder = _GetEncoder(encoder);
		Iterations = iterations;
		_Password = password;

		_Header = new byte[5 + _SALT_SIZE];
		_Header[0] = _FORMAT_PASSWORD;
		BinaryPrimitives.WriteInt32BigEndian(_Header.AsSpan(1), iterations);
		RandomNumberGenerator.Fill(_Header.AsSpan(5));

		_Key = Rfc2898DeriveBytes.Pbkdf2(password, _Header.AsSpan(5), iterations, HashAlgorithmName.SHA256, 32);
		_Keys[Convert.ToBase64String(_Header)] = _Key;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of PBKDF2 iterations; 0 if the instance uses a key.</summary>
	public int Iterations
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Generates a random key.</summary>
	/// <param name="size">Key size in bytes; 16, 24 or 32.</param>
	/// <returns>Returns the key.</returns>
	/// <exception cref="ArgumentException">Thrown when the key size is invalid.</exception>
	public static byte[] GenerateKey(int size = 32)
	{
		if((size != 16) && (size != 24) && (size != 32)) { throw new ArgumentException("The key must have 16, 24 or 32 bytes.", nameof(size)); }

		return RandomNumberGenerator.GetBytes(size);
	}


	/// <summary>Encrypts data with a key.</summary>
	/// <param name="data">Data.</param>
	/// <param name="key">Key; 16, 24 or 32 bytes.</param>
	/// <returns>Returns the encrypted data.</returns>
	/// <exception cref="ArgumentException">Thrown when the key size is invalid.</exception>
	public static byte[] Encrypt(byte[] data, byte[] key)
	{
		return new AESGCM(key).Encrypt(data);
	}


	/// <summary>Decrypts data with a key.</summary>
	/// <param name="data">Encrypted data.</param>
	/// <param name="key">Key; 16, 24 or 32 bytes.</param>
	/// <returns>Returns the data.</returns>
	/// <exception cref="ArgumentException">Thrown when the key size is invalid.</exception>
	/// <exception cref="CryptographicException">Thrown when the data is invalid, has been modified or the key is wrong.</exception>
	public static byte[] Decrypt(byte[] data, byte[] key)
	{
		return new AESGCM(key).Decrypt(data);
	}


	/// <summary>Encrypts a string with a password.</summary>
	/// <param name="s">String.</param>
	/// <param name="password">Password.</param>
	/// <param name="encoder">Encoder that converts encrypted data to text; NULL for Base64.</param>
	/// <returns>Returns the encrypted string.</returns>
	/// <remarks>The key is derived for every call, which takes some time. Use an instance to encrypt several strings.</remarks>
	public static string Encrypt(string s, string password, IEncoder? encoder = null)
	{
		return new AESGCM(password, encoder).EncodeString(s);
	}


	/// <summary>Decrypts a string with a password.</summary>
	/// <param name="s">Encrypted string.</param>
	/// <param name="password">Password.</param>
	/// <param name="encoder">Encoder that has been used to convert encrypted data to text; NULL for Base64.</param>
	/// <returns>Returns the string.</returns>
	/// <exception cref="CryptographicException">Thrown when the data is invalid, has been modified or the password is wrong.</exception>
	public static string Decrypt(string s, string password, IEncoder? encoder = null)
	{
		AESGCM c = new(_GetEncoder(encoder), password);
		return c.DecodeToString(s);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constructors                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class for decryption only, without deriving a key.</summary>
	/// <param name="encoder">Encoder.</param>
	/// <param name="password">Password.</param>
	private AESGCM(IEncoder encoder, string password)
	{
		Encoder = encoder;
		Iterations = DEFAULT_ITERATIONS;
		_Password = password;
		_Key = [];
		_Header = [];
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Validates the encoder.</summary>
	/// <param name="encoder">Encoder; NULL for Base64.</param>
	/// <returns>Returns the encoder.</returns>
	/// <exception cref="ArgumentException">Thrown when the encoder is <see cref="PlainText"/>.</exception>
	private static IEncoder _GetEncoder(IEncoder? encoder)
	{
		if(encoder is PlainText) { throw new ArgumentException("Encrypted data cannot be written as plain text.", nameof(encoder)); }

		return (encoder ?? Base64.DEFAULT);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the key for a password header.</summary>
	/// <param name="header">Header.</param>
	/// <returns>Returns the key.</returns>
	/// <exception cref="CryptographicException">Thrown when the header is invalid.</exception>
	private byte[] _GetPasswordKey(ReadOnlySpan<byte> header)
	{
		string id = Convert.ToBase64String(header);
		if(_Keys.TryGetValue(id, out byte[]? rval)) { return rval; }

		int iterations = BinaryPrimitives.ReadInt32BigEndian(header[1..]);
		if((iterations < 1) || (iterations > MAX_ITERATIONS)) { throw new CryptographicException("The data is invalid."); }

		rval = Rfc2898DeriveBytes.Pbkdf2(_Password!, header[5..], iterations, HashAlgorithmName.SHA256, 32);

		if(_Keys.Count >= _MAX_CACHED_KEYS) { _Keys.Clear(); }
		_Keys[id] = rval;

		return rval;
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
		if(_Key.Length == 0) { throw new InvalidOperationException("This instance cannot encrypt."); }

		int h = _Header.Length;
		byte[] rval = new byte[h + _NONCE_SIZE + data.Length + _TAG_SIZE];

		_Header.CopyTo(rval, 0);
		Span<byte> nonce = rval.AsSpan(h, _NONCE_SIZE);
		RandomNumberGenerator.Fill(nonce);

		using AesGcm aes = new(_Key, _TAG_SIZE);
		aes.Encrypt(nonce, data, rval.AsSpan(h + _NONCE_SIZE, data.Length), rval.AsSpan(h + _NONCE_SIZE + data.Length, _TAG_SIZE), rval.AsSpan(0, h));

		return rval;
	}


	/// <inheritdoc/>
	public byte[] Decrypt(byte[] data)
	{
		int h;
		byte[] key;

		if((data.Length > 0) && (data[0] == _FORMAT_KEY) && (_Password is null))
		{
			h = 1;
			key = _Key;
		}
		else if((data.Length >= (5 + _SALT_SIZE)) && (data[0] == _FORMAT_PASSWORD) && (_Password is not null))
		{
			h = (5 + _SALT_SIZE);
			key = _GetPasswordKey(data.AsSpan(0, h));
		}
		else { throw new CryptographicException("The data is invalid or has not been encrypted with " + ((_Password is null) ? "a key." : "a password.")); }

		if(data.Length < (h + _NONCE_SIZE + _TAG_SIZE)) { throw new CryptographicException("The data is invalid."); }

		int len = (data.Length - h - _NONCE_SIZE - _TAG_SIZE);
		byte[] rval = new byte[len];

		using AesGcm aes = new(key, _TAG_SIZE);
		aes.Decrypt(data.AsSpan(h, _NONCE_SIZE), data.AsSpan(h + _NONCE_SIZE, len), data.AsSpan(h + _NONCE_SIZE + len, _TAG_SIZE), rval, data.AsSpan(0, h));

		return rval;
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
	public override byte[] Encode(byte[] data)
	{
		return Encoder.Encode(Encrypt(data));
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return Decrypt(Encoder.Decode(data));
	}
}
