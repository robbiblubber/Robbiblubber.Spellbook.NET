namespace Robbiblubber.Spellbook.UnitTests.Core.Hashes;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests BLAKE2b and Argon2.</summary>
[TestFixture]
public class Argon2Tests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a byte array filled with one value.</summary>
	/// <param name="value">Value.</param>
	/// <param name="length">Length.</param>
	/// <returns>Returns the array.</returns>
	private static byte[] _Fill(byte value, int length)
	{
		return Enumerable.Repeat(value, length).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: BLAKE2b                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>BLAKE2b produces the same hashes as Python hashlib, including inputs around the block size.</summary>
	/// <param name="length">Input length.</param>
	/// <param name="hex64">64 byte hash.</param>
	/// <param name="hex20">20 byte hash.</param>
	[TestCase(0, "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce", "3345524abf6bbe1809449224b5972c41790b6cf2")]
	[TestCase(1, "dc19b62fbd4e480a12134892649b0c86c21540830bf61aa81e7e13868e7d23ed78ed66b94f6ea8bb779261b98782745fc85ad1dfe9b07859ed77c28b94eaac85", "105ecc26ae21e421275312c6c5b50e8a006dbc3b")]
	[TestCase(127, "3dd556bc15bb8a1c16ce9bc28f245325f5bfc50e84f44c539b7172295d22d81b613a19324b278b3a06dca69a7c9f7d38b9862fa8f2230a9818fe126ef207aa17", "e47f8d9e8bfce39cb6e5926c6d0ded3c18f71c38")]
	[TestCase(128, "88134c9c2bda5481ec42b0d53611231ab7c21ccbbbcf5bc813acffea3eef47c1bb198748d222a9a0d0899631eb39b8c63d61ab1bc8128196e615dc9032ebf5bf", "e71d1237aec06bba9ffe14256ec0958a36204a72")]
	[TestCase(129, "458471021bb1880be73a9a9054316b4b5b83cd27e0ece6f76987d52eb53ccea9ea89e5f1d5a45343536d439e4a125e733c41f2aef79aa299d076a10fc3b283f2", "5843afe9a36ab8752f2c006d73fd482b1480cd13")]
	[TestCase(256, "8055742fadd993ad58a28ef85b80e63059a8944b9fc74e65ff7dbbc6a133ac5fa7ea85ff285e7e93d1cb52e1e6ec49cc6a2c56183a953b91982dfcb6aee9e0fd", "bae91822e7c7d8d4134e642ea36a8054eada4973")]
	[TestCase(1000, "82dd7f967c8cf7d791ab973f395ce10666ac0616a47b0042abc07ab7f9057733d2ccd271b1679e3074256091e733eec08744c08d86a079c8e690e887dd4810f4", "4b1a4ba0315ebe9053809eadb76774adb770d783")]
	public void Blake2b_Reference(int length, string hex64, string hex20)
	{
		byte[] data = Enumerable.Range(0, length).Select(m => (byte) (((m * 31) + 7) % 256)).ToArray();

		Assert.That(Hex.FromBytes(_Blake2b._Hash(64, data)), Is.EqualTo(hex64));
		Assert.That(Hex.FromBytes(_Blake2b._Hash(20, data)), Is.EqualTo(hex20));

		_Blake2b inc = new(64);
		for(int i = 0; i < length; i += 50) { inc._Update(data.AsSpan(i, Math.Min(50, length - i))); }
		Assert.That(Hex.FromBytes(inc._Final()), Is.EqualTo(hex64));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: Argon2                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Argon2 produces the test vectors of RFC 9106.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="hex">Expected tag.</param>
	[TestCase(Argon2Type.ARGON2D, "512b391b6f1162975371d30919734294f868e3be3984f3c1a13a4db9fabe4acb")]
	[TestCase(Argon2Type.ARGON2I, "c814d9d1dc7f37aa13f0d77f2494bda1c8de6b016dd388d29952a4c4672b6ce8")]
	[TestCase(Argon2Type.ARGON2ID, "0d640df58d78766c08c037a34a8b53c9d01ef0452d75b65eb52520e96b01e659")]
	public void Rfc9106_TestVectors(Argon2Type type, string hex)
	{
		byte[] tag = Argon2.DeriveKey(type, _Fill(1, 32), _Fill(2, 16), 32, 3, 4, 32, _Fill(3, 8), _Fill(4, 12));

		Assert.That(Hex.FromBytes(tag), Is.EqualTo(hex));
	}


	/// <summary>Argon2 produces the same keys as the reference implementation for long tags, odd lane counts and memory sizes
	///          that are not a multiple of the lane count.</summary>
	[Test]
	public void ReferenceImplementation_RawKeys()
	{
		Assert.That(Hex.FromBytes(Argon2.DeriveKey(Argon2Type.ARGON2ID, Encoding.UTF8.GetBytes("password"), Encoding.UTF8.GetBytes("somesalt"), 256, 2, 3, 100)),
					Is.EqualTo("452f4f83130e7bb2dbc1fe343f627784da5829b0f047537428666e2d1dccfe716c0546debc1890b7f504805e91ab9f3e3c2951d617cfb4162ccf" +
							   "62952356ad13a9b3aff9a2d6f2cffe577be4f8eadcea9ef618999956fdabf24cce8fe2c3e5ccc60f1f4c"));

		Assert.That(Hex.FromBytes(Argon2.DeriveKey(Argon2Type.ARGON2I, Encoding.UTF8.GetBytes("pw"), Encoding.UTF8.GetBytes("saltsalt"), 100, 1, 3, 16)),
					Is.EqualTo("e81a955bf680a3ebb88879c8f8cfbd33"));
	}


	/// <summary>Hashes created by the reference implementation (argon2-cffi) are verified.</summary>
	/// <param name="password">Password.</param>
	/// <param name="hash">Hash.</param>
	[TestCase("Spellbook äöü", "$argon2id$v=19$m=1024,t=3,p=2$MDEyMzQ1Njc4OWFiY2RlZg$IdDAQ6bTzw4c/9roK0I/cUC7gGffcbI+rvgnsdqlBMk")]
	[TestCase("Spellbook äöü", "$argon2i$v=19$m=1024,t=3,p=2$MDEyMzQ1Njc4OWFiY2RlZg$mKDv86mPkVtELoKnXI197DJQzd2gSLQN2QeaJZsYPOY")]
	[TestCase("Spellbook äöü", "$argon2d$v=19$m=1024,t=3,p=2$MDEyMzQ1Njc4OWFiY2RlZg$vvpUMZIpncfWnjsDh8j+IHzQkiqVTu+esdBAoQ4X1xM")]
	[TestCase("", "$argon2id$v=19$m=8,t=1,p=1$c2FsdHNhbHQ$R3u9vA")]
	public void ReferenceImplementation_Verify(string password, string hash)
	{
		Assert.That(Argon2.DEFAULT.Verify(password, hash), Is.True);
		Assert.That(Argon2.DEFAULT.Verify(password + "x", hash), Is.False);
	}


	/// <summary>Hashes are written in PHC format and verified.</summary>
	[Test]
	public void HashAndVerify()
	{
		Argon2 a = new(Argon2Type.ARGON2ID, 64, 1, 2);
		string h1 = a.Hash("geheim");
		string h2 = a.Hash("geheim");

		Assert.That(h1, Does.Match(@"^\$argon2id\$v=19\$m=64,t=1,p=2\$[A-Za-z0-9+/]{22}\$[A-Za-z0-9+/]{43}$"));
		Assert.That(h1, Is.Not.EqualTo(h2));
		Assert.That(a.Verify("geheim", h1), Is.True);
		Assert.That(a.Verify("Geheim", h1), Is.False);
		Assert.That(Argon2.DEFAULT.Verify("geheim", h1), Is.True);
		Assert.That(new Argon2(Argon2Type.ARGON2I, 32, 1).Hash("x"), Does.StartWith("$argon2i$v=19$m=32,t=1,p=1$"));
	}


	/// <summary>The default parameters follow the OWASP recommendation.</summary>
	[Test]
	public void Defaults()
	{
		Argon2 a = Argon2.DEFAULT;

		Assert.That(a.Type, Is.EqualTo(Argon2Type.ARGON2ID));
		Assert.That(a.Memory, Is.EqualTo(19456));
		Assert.That(a.Iterations, Is.EqualTo(2));
		Assert.That(a.Parallelism, Is.EqualTo(1));
		Assert.That(a.Hash("geheim"), Does.StartWith("$argon2id$v=19$m=19456,t=2,p=1$"));
	}


	/// <summary>Hashes with a secret can only be verified with the same secret.</summary>
	[Test]
	public void Secret()
	{
		Argon2 a = new(Argon2Type.ARGON2ID, 32, 1, 1, 16, 32, Encoding.UTF8.GetBytes("pepper"));
		string h = a.Hash("geheim");

		Assert.That(a.Verify("geheim", h), Is.True);
		Assert.That(new Argon2(Argon2Type.ARGON2ID, 32, 1).Verify("geheim", h), Is.False);
		Assert.That(new Argon2(Argon2Type.ARGON2ID, 32, 1, 1, 16, 32, Encoding.UTF8.GetBytes("salt")).Verify("geheim", h), Is.False);
	}


	/// <summary>Keys are deterministic and depend on the associated data.</summary>
	[Test]
	public void DeriveKey()
	{
		Argon2 a = new(Argon2Type.ARGON2ID, 32, 1, 1, 16, 64);
		byte[] pw = Encoding.UTF8.GetBytes("geheim");
		byte[] salt = _Fill(7, 16);

		byte[] k1 = a.DeriveKey(pw, salt);

		Assert.That(k1, Has.Length.EqualTo(64));
		Assert.That(a.DeriveKey(pw, salt), Is.EqualTo(k1));
		Assert.That(a.DeriveKey(pw, salt, [1]), Is.Not.EqualTo(k1));
		Assert.That(Argon2.DeriveKey(Argon2Type.ARGON2ID, pw, salt, 32, 1, 1, 64), Is.EqualTo(k1));
		Assert.Throws<ArgumentException>(() => a.DeriveKey(pw, [1, 2, 3]));
	}


	/// <summary>Hashes with other or weaker parameters need a rehash.</summary>
	[Test]
	public void NeedsRehash()
	{
		Argon2 weak = new(Argon2Type.ARGON2ID, 32, 1);
		Argon2 strong = new(Argon2Type.ARGON2ID, 64, 2);
		string h = weak.Hash("geheim");

		Assert.That(weak.NeedsRehash(h), Is.False);
		Assert.That(strong.NeedsRehash(h), Is.True);
		Assert.That(strong.NeedsRehash(strong.Hash("geheim")), Is.False);
		Assert.That(new Argon2(Argon2Type.ARGON2I, 32, 1).NeedsRehash(h), Is.True);
		Assert.That(new Argon2(Argon2Type.ARGON2ID, 32, 1, 2).NeedsRehash(h), Is.True);
		Assert.That(weak.NeedsRehash(Pbkdf2.DEFAULT.Hash("geheim")), Is.True);
	}


	/// <summary>Invalid parameters throw.</summary>
	[Test]
	public void InvalidParameters_Throw()
	{
		Assert.Throws<ArgumentException>(() => new Argon2(memory: 7));
		Assert.Throws<ArgumentException>(() => new Argon2(memory: 15, parallelism: 2));
		Assert.Throws<ArgumentException>(() => new Argon2(memory: Argon2.MAX_MEMORY + 1));
		Assert.Throws<ArgumentException>(() => new Argon2(iterations: 0));
		Assert.Throws<ArgumentException>(() => new Argon2(parallelism: 0));
		Assert.Throws<ArgumentException>(() => new Argon2(parallelism: 256));
		Assert.Throws<ArgumentException>(() => new Argon2(saltSize: 7));
		Assert.Throws<ArgumentException>(() => new Argon2(hashSize: 15));
		Assert.Throws<ArgumentException>(() => new Argon2((Argon2Type) 3));
		Assert.Throws<ArgumentException>(() => Argon2.DeriveKey(Argon2Type.ARGON2ID, [], _Fill(0, 8), 8, 1, 1, 3));
	}


	/// <summary>Strings that are not valid Argon2 hashes are not verified.</summary>
	/// <param name="hash">Hash.</param>
	[TestCase("")]
	[TestCase("geheim")]
	[TestCase("$argon2id$v=16$m=8,t=1,p=1$c2FsdHNhbHQ$R3u9vA")]
	[TestCase("$argon2x$v=19$m=8,t=1,p=1$c2FsdHNhbHQ$R3u9vA")]
	[TestCase("$argon2id$v=19$m=8,t=1$c2FsdHNhbHQ$R3u9vA")]
	[TestCase("$argon2id$v=19$m=7,t=1,p=1$c2FsdHNhbHQ$R3u9vA")]
	[TestCase("$argon2id$v=19$m=8,t=0,p=1$c2FsdHNhbHQ$R3u9vA")]
	[TestCase("$argon2id$v=19$m=8,t=1,p=1$c2FsdA$R3u9vA")]
	[TestCase("$argon2id$v=19$m=8,t=1,p=1$c2FsdHNhbHQ$R3u9v!")]
	[TestCase("$pbkdf2-sha256$i=1000$c2FsdHNhbHQ$R3u9vA")]
	public void InvalidHash_NotVerified(string hash)
	{
		Assert.That(Argon2.DEFAULT.Verify("", hash), Is.False);
		Assert.That(Argon2.DEFAULT.NeedsRehash(hash), Is.True);
	}
}
