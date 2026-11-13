namespace Robbiblubber.Spellbook.UnitTests.Core.Encryption;

using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;



/// <summary>This class tests AES-GCM encryption.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class AESGCMTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of iterations for fast tests.</summary>
	private const int _FAST = 1000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: key                                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Data encrypted with a key is decrypted; the format is header, nonce, ciphertext and tag.</summary>
	/// <param name="size">Key size.</param>
	[TestCase(16)]
	[TestCase(24)]
	[TestCase(32)]
	public void Key_RoundTrip(int size)
	{
		byte[] key = AESGCM.GenerateKey(size);
		byte[] data = Encoding.UTF8.GetBytes("Hello Spellbook, äöü €!");

		byte[] enc = AESGCM.Encrypt(data, key);

		Assert.That(enc, Has.Length.EqualTo(1 + 12 + data.Length + 16));
		Assert.That(enc[0], Is.EqualTo(1));
		Assert.That(AESGCM.Decrypt(enc, key), Is.EqualTo(data));
		Assert.That(AESGCM.Decrypt(AESGCM.Encrypt([], key), key), Is.Empty);
	}


	/// <summary>The same data results in different output each time.</summary>
	[Test]
	public void Key_RandomNonce()
	{
		byte[] key = AESGCM.GenerateKey();
		byte[] data = [1, 2, 3];

		byte[] first = AESGCM.Encrypt(data, key);
		byte[] second = AESGCM.Encrypt(data, key);
		Assert.That(first, Is.Not.EqualTo(second));
	}


	/// <summary>Data that can be read by a standard AES-GCM implementation.</summary>
	[Test]
	public void Key_StandardFormat()
	{
		byte[] key = AESGCM.GenerateKey();
		byte[] data = Encoding.UTF8.GetBytes("interop");
		byte[] enc = AESGCM.Encrypt(data, key);

		byte[] dec = new byte[data.Length];
		using AesGcm aes = new(key, 16);
		aes.Decrypt(enc.AsSpan(1, 12), enc.AsSpan(13, data.Length), enc.AsSpan(13 + data.Length), dec, enc.AsSpan(0, 1));

		Assert.That(dec, Is.EqualTo(data));
	}


	/// <summary>Modified data, wrong keys and invalid data throw.</summary>
	[Test]
	public void Key_Tampered_Throws()
	{
		byte[] key = AESGCM.GenerateKey();
		byte[] enc = AESGCM.Encrypt(Encoding.UTF8.GetBytes("secret data"), key);

		for(int i = 0; i < enc.Length; i++)
		{
			byte[] t = (byte[]) enc.Clone();
			t[i] ^= 1;
			Assert.Throws(Is.InstanceOf<CryptographicException>(), () => AESGCM.Decrypt(t, key));
		}

		Assert.Throws(Is.InstanceOf<CryptographicException>(), () => AESGCM.Decrypt(enc, AESGCM.GenerateKey()));
		Assert.Throws(Is.InstanceOf<CryptographicException>(), () => AESGCM.Decrypt(enc[..^1], key));
		Assert.Throws(Is.InstanceOf<CryptographicException>(), () => AESGCM.Decrypt([], key));
		Assert.Throws(Is.InstanceOf<CryptographicException>(), () => AESGCM.Decrypt(new byte[20], key));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: password                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Data encrypted with a password is decrypted by another instance with the same password.</summary>
	[Test]
	public void Password_RoundTrip()
	{
		AESGCM a = new("geheim", null, _FAST);
		AESGCM b = new("geheim", null, _FAST);
		string enc = a.EncodeString("Hello Spellbook, äöü €!");

		Assert.That(a.DecodeToString(enc), Is.EqualTo("Hello Spellbook, äöü €!"));
		Assert.That(b.DecodeToString(enc), Is.EqualTo("Hello Spellbook, äöü €!"));
		
		string x1 = a.EncodeString("x");
		string x2 = a.EncodeString("x");
		Assert.That(x1, Is.Not.EqualTo(x2));

		Assert.That(a.DecodeToInt(a.EncodeInt(-42)), Is.EqualTo(-42));
		Assert.That(a.Iterations, Is.EqualTo(_FAST));
	}


	/// <summary>The key is derived with PBKDF2-SHA256 as documented.</summary>
	[Test]
	public void Password_StandardFormat()
	{
		AESGCM a = new("geheim", null, _FAST);
		byte[] data = Encoding.UTF8.GetBytes("interop");
		byte[] enc = a.Encrypt(data);

		Assert.That(enc[0], Is.EqualTo(2));
		Assert.That(enc[1..5], Is.EqualTo(new byte[] { 0, 0, 0x03, 0xe8 }));

		byte[] key = Rfc2898DeriveBytes.Pbkdf2("geheim", enc.AsSpan(5, 16), _FAST, HashAlgorithmName.SHA256, 32);
		byte[] dec = new byte[data.Length];
		using AesGcm aes = new(key, 16);
		aes.Decrypt(enc.AsSpan(21, 12), enc.AsSpan(33, data.Length), enc.AsSpan(33 + data.Length), dec, enc.AsSpan(0, 21));

		Assert.That(dec, Is.EqualTo(data));
	}


	/// <summary>A wrong password throws.</summary>
	[Test]
	public void Password_Wrong_Throws()
	{
		string enc = new AESGCM("geheim", null, _FAST).EncodeString("data");

		Assert.Throws(Is.InstanceOf<CryptographicException>(), () => new AESGCM("Geheim", null, _FAST).DecodeToString(enc));
	}


	/// <summary>Key and password formats are not mixed up.</summary>
	[Test]
	public void Formats_NotMixed()
	{
		byte[] key = AESGCM.GenerateKey();
		AESGCM k = new(key);
		AESGCM p = new("geheim", null, _FAST);

		Assert.Throws<CryptographicException>(() => p.Decrypt(k.Encrypt([1])));
		Assert.Throws<CryptographicException>(() => k.Decrypt(p.Encrypt([1])));
	}


	/// <summary>The static string methods use the default iterations and Base64.</summary>
	[Test]
	public void Static_Strings()
	{
		string enc = AESGCM.Encrypt("Hello", "geheim");

		Assert.That(Base64.ToBytes(enc)[0], Is.EqualTo(2));
		Assert.That(Base64.ToBytes(enc)[1..5], Is.EqualTo(new byte[] { 0x00, 0x09, 0x27, 0xc0 }));
		Assert.That(AESGCM.Decrypt(enc, "geheim"), Is.EqualTo("Hello"));
		Assert.That(AESGCM.Decrypt(AESGCM.Encrypt("Hello", "geheim", Base62.DEFAULT), "geheim", Base62.DEFAULT), Is.EqualTo("Hello"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: API                                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances can be used as encoder to store ddn files.</summary>
	[Test]
	public void Ddn_Encrypted()
	{
		AESGCM c = new(AESGCM.GenerateKey(), Base62.DEFAULT);
		DdnElement ddn = Ddn.Create("a = 1; s { b = text; }");
		string enc = ddn.ToString(c);

		Assert.That(enc, Does.Not.Contain("text"));
		Assert.That(Ddn.Open(new MemoryStream(Encoding.UTF8.GetBytes(enc)), new Ddn.Options(c))["s/b"].Value, Is.EqualTo("text"));
	}


	/// <summary>Invalid parameters throw.</summary>
	[Test]
	public void InvalidParameters_Throw()
	{
		Assert.Throws<ArgumentException>(() => new AESGCM(new byte[15]));
		Assert.Throws<ArgumentException>(() => new AESGCM(AESGCM.GenerateKey(), PlainText.DEFAULT));
		Assert.Throws<ArgumentException>(() => new AESGCM("x", PlainText.DEFAULT));
		Assert.Throws<ArgumentException>(() => new AESGCM("x", null, 0));
		Assert.Throws<ArgumentException>(() => new AESGCM("x", null, AESGCM.MAX_ITERATIONS + 1));
		Assert.Throws<ArgumentException>(() => AESGCM.GenerateKey(20));
	}


	/// <summary>Instances are thread-safe.</summary>
	[Test]
	public void Parallel_RoundTrip()
	{
		AESGCM c = new("geheim", null, _FAST);

		Parallel.For(0, 50, i => Assert.That(c.DecodeToString(c.EncodeString("v" + i)), Is.EqualTo("v" + i)));
	}
}
