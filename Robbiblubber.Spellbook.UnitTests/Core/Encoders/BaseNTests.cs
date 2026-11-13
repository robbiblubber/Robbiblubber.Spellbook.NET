namespace Robbiblubber.Spellbook.UnitTests.Core.Encoders;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests BaseN, Base62 and inverted Base62 encoding.</summary>
[TestFixture]
public class BaseNTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets test data by name.</summary>
	/// <param name="name">Name.</param>
	/// <returns>Returns the data.</returns>
	private static byte[] _Data(string name)
	{
		return name switch
		{
			"single" => [1],
			"ff"     => [255],
			"text"   => Encoding.UTF8.GetBytes("Hello Spellbook, äöü €!"),
			"rand40" => Enumerable.Range(0, 40).Select(m => (byte) (((m * 73) + 11) % 256)).ToArray(),
			"max"    => Enumerable.Repeat((byte) 255, 17).ToArray(),
			_        => throw new ArgumentException(name)
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: compatibility                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Encoded data is identical to the previous implementation.</summary>
	/// <param name="name">Test data name.</param>
	/// <param name="b62">Base62 reference.</param>
	/// <param name="ib62">Inverted Base62 reference.</param>
	/// <param name="b92">Base92 reference.</param>
	[TestCase("single", "1", "1", "!")]
	[TestCase("ff", "47", "47", "#j")]
	[TestCase("text", "3f2Cm8GnRPWGuSLxCDDqMCb15yvZnT91ct5NhJ", "3F2cM8gNrpwgUslXcddQmcB15YVzNt91CT5nHj", @"!=K^hvgK6I;fCb+<\nzcru^Y9)?C42 dV*d")]
	[TestCase("rand40", "x3HK5i3EI0CLACUiGGqBvL1zA7RVSzsxXGcYv5UwGkAvsgzJSDzKc", "X3hk5I3ei0clacuIggQbVl1Za7rvsZSXxgCyV5uWgKaVSGZjsdZkC",
			  @"&1|Fa%KMF}rA|1od&$UWP>jKM\,@[Om,{OsAO;L8A(\,<YCB}")]
	[TestCase("max", "WAaf8lXCcWT8aQuHrZwq5R1", "waAF8LxcCwt8AqUhRzWQ5r1", "P1;W5D+?C9%C!S~j#ydJ1")]
	public void Bytes_Compatible(string name, string b62, string ib62, string b92)
	{
		byte[] data = _Data(name);

		Assert.That(Base62.FromBytes(data), Is.EqualTo(b62));
		Assert.That(InvertedBase62.FromBytes(data), Is.EqualTo(ib62));
		Assert.That(new BaseN(92).EncodeBytes(data), Is.EqualTo(b92));

		Assert.That(Base62.ToBytes(b62), Is.EqualTo(data));
		Assert.That(InvertedBase62.ToBytes(ib62), Is.EqualTo(data));
		Assert.That(new BaseN(92).DecodeToBytes(b92), Is.EqualTo(data));
	}


	/// <summary>Encoded integers are identical to the previous implementation.</summary>
	/// <param name="value">Value.</param>
	/// <param name="b62">Base62 reference.</param>
	/// <param name="ib62">Inverted Base62 reference.</param>
	[TestCase(0, "0", "0")]
	[TestCase(1, "18OWG", "18owg")]
	[TestCase(-1, "4gfFC3", "4GFfc3")]
	[TestCase(256, "H32", "h32")]
	[TestCase(65536, "48", "48")]
	[TestCase(int.MaxValue, "4gfF9z", "4GFf9Z")]
	[TestCase(int.MinValue, "24", "24")]
	[TestCase(123456789, "Okm0t", "oKM0T")]
	public void Int_Compatible(int value, string b62, string ib62)
	{
		Assert.That(Base62.FromInt(value), Is.EqualTo(b62));
		Assert.That(InvertedBase62.FromInt(value), Is.EqualTo(ib62));
		Assert.That(Base62.ToInt(b62), Is.EqualTo(value));
		Assert.That(InvertedBase62.ToInt(ib62), Is.EqualTo(value));
	}


	/// <summary>Encoded long integers are identical to the previous implementation.</summary>
	/// <param name="value">Value.</param>
	/// <param name="b62">Base62 reference.</param>
	[TestCase(0L, "0")]
	[TestCase(-1L, "LygHa16AHYF")]
	[TestCase(long.MaxValue, "LygHa16AHWB")]
	[TestCase(1234567890123456789L, "1qTzXuT1O4n")]
	public void Long_Compatible(long value, string b62)
	{
		Assert.That(Base62.FromLong(value), Is.EqualTo(b62));
		Assert.That(Base62.ToLong(b62), Is.EqualTo(value));
	}


	/// <summary>Encoded doubles are identical to the previous implementation.</summary>
	/// <param name="value">Value.</param>
	/// <param name="b62">Base62 reference.</param>
	[TestCase(0.0, "0")]
	[TestCase(3.14159, "9UJaRsSjo8m")]
	[TestCase(-2.5e-300, "43BxPj9RXPN")]
	public void Double_Compatible(double value, string b62)
	{
		Assert.That(Base62.FromDouble(value), Is.EqualTo(b62));
		Assert.That(Base62.ToDouble(b62), Is.EqualTo(value));
	}


	/// <summary>Strings are identical to the previous implementation.</summary>
	[Test]
	public void String_Compatible()
	{
		Assert.That(Base62.FromString("Hello Spellbook, äöü €!"), Is.EqualTo("3f2Cm8GnRPWGuSLxCDDqMCb15yvZnT91ct5NhJ"));
		Assert.That(Base62.Decode("3f2Cm8GnRPWGuSLxCDDqMCb15yvZnT91ct5NhJ"), Is.EqualTo("Hello Spellbook, äöü €!"));
		Assert.That("Hello".ToBase62().FromBase62(), Is.EqualTo("Hello"));
	}


	/// <summary>Integers with leading zero digits (written by the new implementation) are read as well.</summary>
	[Test]
	public void Int_LeadingZeroDigits()
	{
		Assert.That(Base62.ToInt("0H32"), Is.EqualTo(256));
		Assert.That(Base62.ToInt("0000"), Is.EqualTo(0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: round trips                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Leading zero bytes survive a round trip.</summary>
	[Test]
	public void LeadingZeros_RoundTrip()
	{
		Assert.That(Base62.FromBytes(new byte[] { 0, 0, 7 }), Is.EqualTo("007"));
		Assert.That(Base62.ToBytes("007"), Is.EqualTo(new byte[] { 0, 0, 7 }));
		Assert.That(Base62.ToBytes(Base62.FromBytes(new byte[] { 0, 0, 0 })), Is.EqualTo(new byte[] { 0, 0, 0 }));
		Assert.That(Base62.FromBytes(new byte[] { 0 }), Is.EqualTo("0"));
		Assert.That(Base62.FromBytes([]), Is.Empty);
		Assert.That(Base62.ToBytes(string.Empty), Is.Empty);
	}


	/// <summary>Random data survives a round trip in many bases, including bases above 128.</summary>
	/// <param name="b">Base.</param>
	[TestCase(2)]
	[TestCase(3)]
	[TestCase(10)]
	[TestCase(16)]
	[TestCase(58)]
	[TestCase(62)]
	[TestCase(92)]
	[TestCase(129)]
	[TestCase(200)]
	[TestCase(255)]
	[TestCase(256)]
	public void Random_RoundTrip(int b)
	{
		BaseN enc = new(b, string.Concat(Enumerable.Range(0x100, 256).Select(m => (char) m)));
		Random rnd = new(b);

		for(int t = 0; t < 200; t++)
		{
			byte[] data = new byte[rnd.Next(0, 300)];
			rnd.NextBytes(data);
			for(int z = Math.Min(rnd.Next(0, 3), data.Length); z > 0; z--) { data[z - 1] = 0; }

			Assert.That(enc.DecodeToBytes(enc.EncodeBytes(data)), Is.EqualTo(data));

			int i = rnd.Next(int.MinValue, int.MaxValue);
			Assert.That(enc.DecodeToInt(enc.EncodeInt(i)), Is.EqualTo(i));

			long l = rnd.NextInt64(long.MinValue, long.MaxValue);
			Assert.That(enc.DecodeToLong(enc.EncodeLong(l)), Is.EqualTo(l));
		}
	}


	/// <summary>Large data is converted in reasonable time.</summary>
	[Test]
	public void LargeData_RoundTrip()
	{
		byte[] data = new byte[20000];
		new Random(1).NextBytes(data);

		Assert.That(Base62.ToBytes(Base62.FromBytes(data)), Is.EqualTo(data));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: errors                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Invalid characters throw.</summary>
	[Test]
	public void InvalidCharacter_Throws()
	{
		FormatException ex = Assert.Throws<FormatException>(() => Base62.ToBytes("ab!c"))!;
		Assert.That(ex.Message, Does.Contain("position 2"));

		Assert.Throws<FormatException>(() => Base62.ToBytes("abc€"));
	}


	/// <summary>Invalid bases and alphabets throw.</summary>
	[Test]
	public void InvalidAlphabet_Throws()
	{
		Assert.Throws<ArgumentException>(() => new BaseN(1));
		Assert.Throws<ArgumentException>(() => new BaseN(257, new string('x', 300)));
		Assert.Throws<ArgumentException>(() => new BaseN(93));
		Assert.Throws<ArgumentException>(() => new BaseN(3, "aab"));
		Assert.Throws<ArgumentException>(() => new Base62("0123456789"));
	}


	/// <summary>Values that are too long for the type throw.</summary>
	[Test]
	public void TooLong_Throws()
	{
		Assert.Throws<FormatException>(() => Base62.ToInt(Base62.FromLong(long.MaxValue)));
	}


	/// <summary>Custom alphabets are used.</summary>
	[Test]
	public void CustomAlphabet()
	{
		Base62 enc = new(InvertedBase62.ALPHABET);

		Assert.That(enc.Alphabet, Is.EqualTo(InvertedBase62.ALPHABET));
		Assert.That(enc.Base, Is.EqualTo(62));
		Assert.That(enc.EncodeInt(123456789), Is.EqualTo(InvertedBase62.FromInt(123456789)));
	}
}
