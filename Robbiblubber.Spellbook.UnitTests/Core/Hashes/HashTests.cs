namespace Robbiblubber.Spellbook.UnitTests.Core.Hashes;

using HashAlgorithmName = System.Security.Cryptography.HashAlgorithmName;
using System.Text;
using NUnit.Framework;



/// <summary>This class tests the hash algorithms.</summary>
[TestFixture]
public class HashTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: compatibility                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hashes are identical to the previous implementation, including hashes that start with a zero byte.</summary>
	/// <param name="value">Input.</param>
	/// <param name="hex">SHA-256, hexadecimal.</param>
	/// <param name="b62">SHA-256, Base62.</param>
	/// <param name="ib62">SHA-256, inverted Base62.</param>
	/// <param name="b64">SHA-256, Base64.</param>
	/// <param name="md5">MD5, Base62.</param>
	/// <param name="sha1">SHA-1, Base62.</param>
	[TestCase("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", "iDUK5mbjE0q8kxKur8d0acwb3mlMY7nzH5i6WpR5bav",
			  "Iduk5MBJe0Q8KXkUR8D0ACWB3MLmy7NZh5I6wPr5BAV", "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0", "4NjUkTaz87FZto8hbWIO6E", "OCL0sP18XM6bXxMmNhRtCAUAlXJ")]
	[TestCase("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", "rzWtdMwJelxEeMmeB0EiiEGkAYggupnSUjWEepHLxI5",
			  "RZwTDmWjELXeEmMEb0eIIegKayGGUPNsuJweEPhlXi5", "47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU", "6SFsQFoWZKg7DZ3I6vLDPK", "V8Va6wNpvtoe1O39WAnfvMaQejJ")]
	[TestCase("Passwort123!", "034c0f8ac5ab8ef39e1c25af63b4f8d40e9488ebb5ff03af3c583b4e4db2aa3d", "mTPzcu1xQPPO6Gi3W0nXLYzGC4SSHyORwn3N6KfJtd",
			  "MtpZCU1Xqppo6gI3w0NxlyZgc4sshYorWN3n6kFjTD", "A0wPisWrjvOeHCWvY7T41A6UiOu1/wOvPFg7Tk2yqj0", "o5xw6RfYUKj3cwuE4nyvH", "Al18ZGloY5WciMI9teXA1CgmUF4")]
	[TestCase("äöü€", "1dd197a44296eb8305cc81951cbf2ade302ca09d5924013631e2e42103e0f2b6", "74OgG812x8sQX8fIp5UpCZFIew0ZJI2Aj8ZhRfY562A",
			  "74oGg812X8Sqx8FiP5uPczfiEW0zji2aJ8zHrFy562a", "HdGXpEKW64MFzIGVHL8q3jAsoJ1ZJAE2MeLkIQPg8rY", "6ojuBiClwuVpNGTJbp1j1h", "72lDoyNryddlsobXRKP4ZmX6OYT")]
	[TestCase("pw142", "00dba10ee7253edc99748d5b0435e5516c52b09d263737fef1733a7f7d61446c", "Cc1csipN5OIWDk6jflFtilwBKxcxG4r4e6CtZFU5fA",
			  "cC1CSIPn5oiwdK6JFLfTILWbkXCXg4R4E6cTzfu5Fa", "ANuhDuclPtyZdI1bBDXlUWxSsJ0mNzf+8XM6f31hRGw", "2x0JqRN9EGfDZFVVkxYR3S", "3uBIH5aPl24rK58fiKB1T8kL6a")]
	[TestCase("pw280", "0070b0c577e77657071706ac8312d37648632a00408ccb3d7d9c7d5936f8b867", "6TFi6cBbf0RUlU1g8UBBM4KgP9vlOyLqzH4pdPchUt",
			  "6tfI6CbBF0ruLu1G8ubbm4kGp9VLoYlQZh4PDpCHuT", "AHCwxXfndlcHFwasgxLTdkhjKgBAjMs9fZx9WTb4uGc", "33YP6fHrCQRXUHe7rRDwoK", "EdTxLzlutO4IxFpyAo6zqSPAckH")]
	public void Compatible(string value, string hex, string b62, string ib62, string b64, string md5, string sha1)
	{
		Assert.That(SHA256.HEX.HashString(value), Is.EqualTo(hex));
		Assert.That(SHA256.BASE62.HashString(value), Is.EqualTo(b62));
		Assert.That(SHA256.INVERTED_BASE62.HashString(value), Is.EqualTo(ib62));
		Assert.That(SHA256.BASE64.HashString(value), Is.EqualTo(b64));
		Assert.That(MD5.BASE62.HashString(value), Is.EqualTo(md5));
		Assert.That(SHA1.BASE62.HashString(value), Is.EqualTo(sha1));

		Assert.That(SHA256.BASE62.Verify(value, b62), Is.True);
		Assert.That(SHA256.HEX.Verify(value, hex), Is.True);
	}


	/// <summary>SHA-512, integers and default instances are identical to the previous implementation.</summary>
	[Test]
	public void Compatible_Other()
	{
		Assert.That(SHA512.BASE62.HashString("abc"), Is.EqualTo("pTB2uLHVjZnV5fgkAz4uofSJjsROB6qP8qQxoESSwuId4xoouXhO04gZe5T1XQYXLu9yRVqEEe3fmp5Wlbvy6J"));
		Assert.That(SHA256.HEX.HashInt(42), Is.EqualTo("e8a4b2ee7ede79a3afb332b5b6cc3d952a65fd8cffb897f5d18016577c33d7cc"));
		Assert.That(SHA256.BASE62.HashInt(42), Is.EqualTo("tALa6HuT3qdyWwpSWow1x3JOo9vgJkkG0k8fmnbq2FI"));
		Assert.That(SHA256.GetHash("abc"), Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
		Assert.That(MD5.GetHash("abc"), Is.EqualTo("900150983cd24fb0d6963f7d28e17f72"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: algorithms                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>All algorithms produce the standard test vectors for "abc".</summary>
	[Test]
	public void TestVectors()
	{
		Assert.That(MD5.GetHash("abc"), Is.EqualTo("900150983cd24fb0d6963f7d28e17f72"));
		Assert.That(SHA1.GetHash("abc"), Is.EqualTo("a9993e364706816aba3e25717850c26c9cd0d89d"));
		Assert.That(SHA256.GetHash("abc"), Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
		Assert.That(SHA384.GetHash("abc"), Is.EqualTo("cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7"));
		Assert.That(SHA512.GetHash("abc"), Is.EqualTo("ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f"));
	}


	/// <summary>Bytes, characters, numbers and streams are hashed.</summary>
	[Test]
	public void Inputs()
	{
		byte[] data = Encoding.UTF8.GetBytes("abc");

		Assert.That(SHA256.GetHash(data), Is.EqualTo(SHA256.GetHash("abc")));
		Assert.That(SHA256.HEX.HashChars("abc".ToCharArray()), Is.EqualTo(SHA256.GetHash("abc")));
		Assert.That(SHA256.HEX.HashStream(new MemoryStream(data)), Is.EqualTo(SHA256.GetHash("abc")));
		Assert.That(SHA256.HEX.HashLong(42L), Is.EqualTo(SHA256.GetHash(BitConverter.GetBytes(42L))));
		Assert.That(SHA256.HEX.HashDouble(1.5), Is.EqualTo(SHA256.GetHash(BitConverter.GetBytes(1.5))));
		Assert.That(SHA256.HEX.HashLong(42L), Is.Not.EqualTo(SHA256.HEX.HashInt(42)));
	}


	/// <summary>Files are hashed as bytes or as text with normalized line breaks.</summary>
	[Test]
	public void Files()
	{
		string file = Path.Combine(Path.GetTempPath(), "spellbook_hash_" + Guid.NewGuid().ToString("N") + ".txt");

		try
		{
			File.WriteAllText(file, "line1\r\nline2", new UTF8Encoding(true));

			Assert.That(SHA256.GetFileHash(file), Is.EqualTo(SHA256.GetHash(File.ReadAllBytes(file))));
			Assert.That(SHA256.GetFileHash(file, true), Is.EqualTo(SHA256.GetHash("line1\nline2")));
			Assert.That(MD5.GetFileHash(file, true, Base62.DEFAULT), Is.EqualTo(MD5.GetHash("line1\nline2", Base62.DEFAULT)));
			Assert.That(SHA512.HEX.HashFile(file), Is.EqualTo(SHA512.GetHash(File.ReadAllBytes(file))));
		}
		finally { File.Delete(file); }
	}


	/// <summary>Verification fails for wrong values; plain text cannot be used as encoder.</summary>
	[Test]
	public void Verify_And_Errors()
	{
		Assert.That(SHA256.BASE62.Verify("abd", "iDUK5mbjE0q8kxKur8d0acwb3mlMY7nzH5i6WpR5bav"), Is.False);
		Assert.That(SHA256.BASE62.Verify("abc", "short"), Is.False);
		Assert.Throws<ArgumentException>(() => new SHA256(PlainText.DEFAULT));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: PBKDF2                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>PBKDF2 produces the published SHA-256 test vectors.</summary>
	/// <param name="iterations">Iterations.</param>
	/// <param name="hex">Expected key.</param>
	[TestCase(1, "120fb6cffcf8b32c43e7225256c4f837a86548c92ccc35480805987cb70be17b")]
	[TestCase(2, "ae4d0c95af6b46d32d0adff928f06dd02a303f8ef3c251dfd6e2d85a95474c43")]
	[TestCase(4096, "c5e478d59288c841aa530db6845c4c8d962893a001ce4e11a4963873aa98134a")]
	public void Pbkdf2_TestVectors(int iterations, string hex)
	{
		string hash = "$pbkdf2-sha256$i=" + iterations + "$" + Base64.FromString("salt") + "$" + Base64.FromBytes(Hex.ToBytes(hex));

		Assert.That(Pbkdf2.DEFAULT.Verify("password", hash), Is.True);
		Assert.That(Pbkdf2.DEFAULT.Verify("Password", hash), Is.False);
	}


	/// <summary>Password hashes are salted and verified.</summary>
	[Test]
	public void Pbkdf2_HashAndVerify()
	{
		Pbkdf2 p = new(HashAlgorithmName.SHA512, 1000);

		string h1 = p.Hash("geheim");
		string h2 = p.Hash("geheim");

		Assert.That(h1, Does.StartWith("$pbkdf2-sha512$i=1000$"));
		Assert.That(h1, Is.Not.EqualTo(h2));
		Assert.That(p.Verify("geheim", h1), Is.True);
		Assert.That(p.Verify("geheim", h2), Is.True);
		Assert.That(p.Verify("Geheim", h1), Is.False);
		Assert.That(Pbkdf2.DEFAULT.Verify("geheim", h1), Is.True);
	}


	/// <summary>Weaker or foreign hashes need a new hash; invalid hashes do not verify.</summary>
	[Test]
	public void Pbkdf2_NeedsRehash()
	{
		Pbkdf2 weak = new(null, 1000);
		Pbkdf2 strong = new(null, 2000);
		string h = weak.Hash("x");

		Assert.That(weak.NeedsRehash(h), Is.False);
		Assert.That(strong.NeedsRehash(h), Is.True);
		Assert.That(new Pbkdf2(HashAlgorithmName.SHA512, 1000).NeedsRehash(h), Is.True);
		Assert.That(strong.NeedsRehash(SHA256.GetHash("x")), Is.True);

		Assert.That(strong.Verify("x", SHA256.GetHash("x")), Is.False);
		Assert.That(strong.Verify("x", "$pbkdf2-sha256$i=abc$AAAA$AAAA"), Is.False);
		Assert.That(strong.Verify("x", "$pbkdf2-md5$i=1$AAAA$AAAA"), Is.False);
	}


	/// <summary>Defaults follow the recommendations; invalid parameters throw.</summary>
	[Test]
	public void Pbkdf2_Parameters()
	{
		Assert.That(Pbkdf2.DEFAULT.Iterations, Is.EqualTo(600_000));
		Assert.That(Pbkdf2.DEFAULT.Algorithm, Is.EqualTo(HashAlgorithmName.SHA256));
		Assert.That(new Pbkdf2(HashAlgorithmName.SHA512).Iterations, Is.EqualTo(210_000));

		Assert.Throws<ArgumentException>(() => new Pbkdf2(HashAlgorithmName.MD5));
		Assert.Throws<ArgumentException>(() => new Pbkdf2(null, -1));
		Assert.Throws<ArgumentException>(() => new Pbkdf2(null, 0, 4));
		Assert.Throws<ArgumentException>(() => new Pbkdf2(null, 0, 16, 8));
	}
}
