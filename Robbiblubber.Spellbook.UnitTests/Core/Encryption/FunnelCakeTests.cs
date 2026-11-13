namespace Robbiblubber.Spellbook.UnitTests.Core.Encryption;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests FunnelCake.</summary>
[TestFixture]
public class FunnelCakeTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Long test text; several key stream blocks.</summary>
	private static readonly string _LONG = string.Concat(Enumerable.Repeat("Lorem ipsum dolor sit amet, 0123456789. ", 12));



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: compatibility                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Output is identical to the previous implementation.</summary>
	/// <param name="s">Input.</param>
	/// <param name="b62">BASE62.</param>
	/// <param name="b64">BASE64.</param>
	/// <param name="hex">HEX.</param>
	/// <param name="ib62">INVERTED_BASE62.</param>
	/// <param name="secret62">Base62 with secret "geheim".</param>
	/// <param name="static64">Static Base64 with secret "geheim".</param>
	[TestCase("Hello", "45aOwIE", "NhdSOTI", "3617523932", "45AoWie", "7x2wKcS", "aSs8WFA")]
	[TestCase("Passwort123!", "IXo33VLPA1nSKnDR", "LhNNJipeP1sMWHxF", "2e134d262a5e3f5b0c587c45", "ixO33vlpa1NskNdr", "jYUyHOnNBCwq2Q5x", "cS8jR0hOJy8QBAxJ")]
	[TestCase("äöü€ Spellbook", "6YjFoTCQnWLbTp02dU6FOt9KSu", "vdb9456Nr62RShwURUxcVgFPOA", "bdd6fde39e8dafad914a1c14454c5c56014f38", "6yJfOtcqNwlBtP02Du6foT9ksU",
			  "7qAHwiZWyxre0W47IqZnjrrH8C", "4uqTgvydt9mNFmwYHR5CCVM/CA")]
	public void Compatible(string s, string b62, string b64, string hex, string ib62, string secret62, string static64)
	{
		Assert.That(FunnelCake.BASE62.EncodeString(s), Is.EqualTo(b62));
		Assert.That(FunnelCake.BASE64.EncodeString(s), Is.EqualTo(b64));
		Assert.That(FunnelCake.HEX.EncodeString(s), Is.EqualTo(hex));
		Assert.That(FunnelCake.INVERTED_BASE62.EncodeString(s), Is.EqualTo(ib62));
		Assert.That(new FunnelCake(Base62.DEFAULT, "geheim").EncodeString(s), Is.EqualTo(secret62));
		Assert.That(FunnelCake.Encrypt(s, "geheim", Base64.DEFAULT), Is.EqualTo(static64));

		Assert.That(FunnelCake.BASE62.DecodeToString(b62), Is.EqualTo(s));
		Assert.That(FunnelCake.BASE64.DecodeToString(b64), Is.EqualTo(s));
		Assert.That(FunnelCake.HEX.DecodeToString(hex), Is.EqualTo(s));
		Assert.That(FunnelCake.INVERTED_BASE62.DecodeToString(ib62), Is.EqualTo(s));
		Assert.That(new FunnelCake(Base62.DEFAULT, "geheim").DecodeToString(secret62), Is.EqualTo(s));
		Assert.That(FunnelCake.Decrypt(static64, "geheim", Base64.DEFAULT), Is.EqualTo(s));
	}


	/// <summary>Long text spanning several key stream blocks is identical to the previous implementation.</summary>
	[Test]
	public void Compatible_LongText()
	{
		Assert.That(SHA256.GetHash(FunnelCake.BASE62.EncodeString(_LONG)), Is.EqualTo("7577af534c5e20cc29ea3ab7539d69918b9934f6e1effbf44741845864ee4769"));
		Assert.That(SHA256.GetHash(FunnelCake.BASE64.EncodeString(_LONG)), Is.EqualTo("10fdab2b03d6bc3ac0d2634ded7759928a48260878893ca566223e354bd04d4d"));
		Assert.That(SHA256.GetHash(FunnelCake.HEX.EncodeString(_LONG)), Is.EqualTo("483b243a2634945b9be677e0b1bb4ea376a5ca805b1a4c9d670b9481142f6950"));
		Assert.That(SHA256.GetHash(FunnelCake.INVERTED_BASE62.EncodeString(_LONG)), Is.EqualTo("716dd22b6b761c38a41d285c2391ff3f7b552b17e5c82366ff99378221def7da"));
		Assert.That(SHA256.GetHash(new FunnelCake(Base62.DEFAULT, "geheim").EncodeString(_LONG)), Is.EqualTo("fd1fafaac19af1c603d36aa6b636454f33acbc477a0c969f4e0b1df19b7a833e"));
		Assert.That(SHA256.GetHash(FunnelCake.Encrypt(_LONG, "geheim", Base64.DEFAULT)), Is.EqualTo("ce0282e78fb86b8487dffc548aaa450201c742be7789d05a9d814238a064b1af"));

		Assert.That(FunnelCake.BASE62.DecodeToString(FunnelCake.BASE62.EncodeString(_LONG)), Is.EqualTo(_LONG));
	}


	/// <summary>Bytes, numbers and the key stream are identical to the previous implementation.</summary>
	[Test]
	public void Compatible_BytesAndNumbers()
	{
		byte[] data = Enumerable.Range(0, 300).Select(m => (byte) m).ToArray();

		Assert.That(SHA256.GetHash(Hex.FromBytes(FunnelCake.Encrypt(data, "k"))), Is.EqualTo("8b9b8dbd686c3299188d6e63c520fd489478153b22c7a9469b34523242c768e7"));
		Assert.That(FunnelCake.Decrypt(FunnelCake.Encrypt(data, "k"), "k"), Is.EqualTo(data));
		Assert.That(FunnelCake.BASE62.EncodeInt(123456789), Is.EqualTo("1yKwl8"));
		Assert.That(FunnelCake.HEX.EncodeLong(-5), Is.EqualTo("858dc1aaa2ceb2d0"));
		Assert.That(FunnelCake.BASE62.DecodeToInt("1yKwl8"), Is.EqualTo(123456789));
		Assert.That(Encoding.ASCII.GetString(FunnelCake.Encrypt(new byte[60], "hUZ6wwgC20Qy")), Is.EqualTo(@"~r>U]1M/=jOd  04n SOklvt[;fPNFmr[9jRF/PEr)bq)C!iE\TN=y4viKVD"));
	}


	/// <summary>ASCII text written as plain text is identical to the previous implementation.</summary>
	[Test]
	public void Compatible_PlainText()
	{
		Assert.That(Hex.UPPER.EncodeBytes(Encoding.UTF8.GetBytes(FunnelCake.Encrypt("Hello ASCII", "geheim"))), Is.EqualTo("692B3C5850011408627F76"));
		Assert.That(Hex.UPPER.EncodeBytes(Encoding.UTF8.GetBytes(FunnelCake.PLAINTEXT.EncodeString("Hello ASCII"))), Is.EqualTo("3617523932110C7C7E2306"));
		Assert.That(FunnelCake.Decrypt(FunnelCake.Encrypt("Hello ASCII", "geheim"), "geheim"), Is.EqualTo("Hello ASCII"));
		Assert.That(FunnelCake.PLAINTEXT.DecodeToString(FunnelCake.PLAINTEXT.EncodeString("Hello ASCII")), Is.EqualTo("Hello ASCII"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: fixes                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text that cannot be written as plain text throws instead of being corrupted.</summary>
	[Test]
	public void PlainText_NonAscii_Throws()
	{
		Assert.Throws<ArgumentException>(() => FunnelCake.Encrypt("äöü€ Spellbook", "geheim"));
		Assert.Throws<ArgumentException>(() => FunnelCake.PLAINTEXT.EncodeString("äöü€ Spellbook"));
		Assert.Throws<ArgumentException>(() => new FunnelCake("geheim").EncodeString("äöü€ Spellbook"));
	}


	/// <summary>Data whose first obfuscated byte is zero survives a round trip with Base62.</summary>
	[Test]
	public void LeadingZero_RoundTrip()
	{
		Assert.That(FunnelCake.Encrypt(Encoding.ASCII.GetBytes("~abc"), "hUZ6wwgC20Qy")[0], Is.EqualTo(0));
		Assert.That(FunnelCake.BASE62.DecodeToString(FunnelCake.BASE62.EncodeString("~abc")), Is.EqualTo("~abc"));
		Assert.That(FunnelCake.INVERTED_BASE62.DecodeToString(FunnelCake.INVERTED_BASE62.EncodeString("~abc")), Is.EqualTo("~abc"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: API                                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances implement IEncrypter and can be used to store ddn files.</summary>
	[Test]
	public void Encrypter()
	{
		IEncrypter e = new FunnelCake(Base64.DEFAULT, "geheim");
		byte[] data = [1, 2, 3, 250];

		Assert.That(e.Encoder, Is.SameAs(Base64.DEFAULT));
		Assert.That(e.Decrypt(e.Encrypt(data)), Is.EqualTo(data));
		Assert.That(e.Encrypt(data), Is.EqualTo(FunnelCake.Encrypt(data, "geheim")));
		Assert.That(new FunnelCake().Encoder, Is.SameAs(PlainText.DEFAULT));

		DdnElement ddn = Ddn.Create("a = 1; s { b = text; }");
		string enc = ddn.ToString(e);
		Assert.That(enc, Does.Not.Contain("text"));
		Assert.That(Ddn.Open(new MemoryStream(Encoding.UTF8.GetBytes(enc)), new Ddn.Options(e))["s/b"].Value, Is.EqualTo("text"));
	}


	/// <summary>Instances are thread-safe.</summary>
	[Test]
	public void Parallel_SameResult()
	{
		string expected = FunnelCake.BASE62.EncodeString(_LONG);

		Parallel.For(0, 50, i => Assert.That(FunnelCake.BASE62.EncodeString(_LONG), Is.EqualTo(expected)));
	}
}
