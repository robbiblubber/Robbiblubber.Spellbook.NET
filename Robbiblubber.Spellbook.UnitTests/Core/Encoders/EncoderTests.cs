namespace Robbiblubber.Spellbook.UnitTests.Core.Encoders;

using System.Globalization;
using System.Text;
using NUnit.Framework;



/// <summary>This class tests Base64, hexadecimal, plain text and GZip encoding and the extension methods.</summary>
[TestFixture]
[NonParallelizable]
public class EncoderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] XorEncoder                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Minimal encoder that only implements the required members.</summary>
	private sealed class XorEncoder: IEncoder
	{
		/// <inheritdoc/>
		public Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}


		/// <inheritdoc/>
		public byte[] Encode(byte[] data)
		{
			return Encoding.UTF8.GetBytes(Convert.ToHexString(data.Select(m => (byte) (m ^ 0x5a)).ToArray()));
		}


		/// <inheritdoc/>
		public byte[] Decode(byte[] data)
		{
			return Convert.FromHexString(Encoding.UTF8.GetString(data)).Select(m => (byte) (m ^ 0x5a)).ToArray();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Culture before the test.</summary>
	private CultureInfo _Culture = CultureInfo.CurrentCulture;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Switches to a culture with comma decimal separator and a non-ASCII minus sign.</summary>
	[SetUp]
	public void SetUp()
	{
		_Culture = CultureInfo.CurrentCulture;

		CultureInfo culture = (CultureInfo) CultureInfo.GetCultureInfo("de-DE").Clone();
		culture.NumberFormat.NegativeSign = "−";
		CultureInfo.CurrentCulture = culture;
	}


	/// <summary>Restores the culture.</summary>
	[TearDown]
	public void TearDown()
	{
		CultureInfo.CurrentCulture = _Culture;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: Base64                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Base64 is compatible with the previous implementation.</summary>
	[Test]
	public void Base64_Compatible()
	{
		Assert.That(Base64.FromString("Hello Spellbook, äöü €!"), Is.EqualTo("SGVsbG8gU3BlbGxib29rLCDDpMO2w7wg4oKsIQ"));
		Assert.That(Base64.FromInt(123456789), Is.EqualTo("Fc1bBw"));
		Assert.That(Base64.FromInt(-1), Is.EqualTo("/////w"));
		Assert.That(Base64.ToInt("Fc1bBw"), Is.EqualTo(123456789));
	}


	/// <summary>Padding is written on request.</summary>
	[Test]
	public void Base64_Padding()
	{
		Assert.That("ab".ToBase64(), Is.EqualTo("YWI"));
		Assert.That("ab".ToBase64(true), Is.EqualTo("YWI="));
		Assert.That(Base64.FromString("ab", true), Is.EqualTo("YWI="));
		Assert.That(new byte[] { 1 }.ToBase64(true), Is.EqualTo("AQ=="));
		Assert.That(Base64.Decode("YWI="), Is.EqualTo("ab"));
		Assert.That(Base64.Decode("YWI"), Is.EqualTo("ab"));
	}


	/// <summary>The URL-safe alphabet is written by the URL instance; both alphabets and whitespace are read.</summary>
	[Test]
	public void Base64_UrlSafe()
	{
		byte[] data = [251, 255, 191];

		Assert.That(Base64.FromBytes(data), Is.EqualTo("+/+/"));
		Assert.That(Base64.URL.EncodeBytes(data), Is.EqualTo("-_-_"));
		Assert.That(Base64.URL.UrlSafe, Is.True);
		Assert.That(Base64.ToBytes("-_-_"), Is.EqualTo(data));
		Assert.That(Base64.ToBytes("+/ +/\r\n"), Is.EqualTo(data));
	}


	/// <summary>Invalid Base64 strings throw.</summary>
	[Test]
	public void Base64_Invalid_Throws()
	{
		Assert.Throws<FormatException>(() => Base64.ToBytes("abcde"));
		Assert.Throws<FormatException>(() => Base64.ToBytes("ab$d"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: Hex                                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hexadecimal encoding is compatible and supports upper case.</summary>
	[Test]
	public void Hex_Encoding()
	{
		Assert.That(Hex.FromInt(123456789), Is.EqualTo("15cd5b07"));
		Assert.That(Hex.FromString("Hi!"), Is.EqualTo("486921"));
		Assert.That(Hex.UPPER.EncodeBytes(new byte[] { 171, 205 }), Is.EqualTo("ABCD"));
		Assert.That(Hex.ToBytes("abCD"), Is.EqualTo(new byte[] { 171, 205 }));
		Assert.That(Hex.ToInt("15CD5B07"), Is.EqualTo(123456789));
	}


	/// <summary>Single bytes and characters are converted.</summary>
	[Test]
	public void Hex_BytesAndChars()
	{
		Assert.That(Hex.DEFAULT.EncodeByte(10), Is.EqualTo("0a"));
		Assert.That(Hex.DEFAULT.EncodeByte(10, false), Is.EqualTo("a"));
		Assert.That(Hex.DEFAULT.EncodeChar('A'), Is.EqualTo("41"));
		Assert.That(Hex.DEFAULT.EncodeChar('€'), Is.EqualTo("20ac"));
		Assert.That(Hex.DEFAULT.DecodeToByte("ff"), Is.EqualTo(255));
		Assert.That(Hex.DEFAULT.DecodeToChar("20AC"), Is.EqualTo('€'));
		Assert.Throws<FormatException>(() => Hex.DEFAULT.DecodeToByte("1ff"));
	}


	/// <summary>Invalid hexadecimal strings throw.</summary>
	[Test]
	public void Hex_Invalid_Throws()
	{
		Assert.Throws<FormatException>(() => Hex.ToBytes("abc"));
		Assert.Throws<FormatException>(() => Hex.ToBytes("zz"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: PlainText                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Plain text numbers are independent of the culture.</summary>
	[Test]
	public void PlainText_Invariant()
	{
		PlainText p = PlainText.DEFAULT;

		Assert.That(p.EncodeInt(-5), Is.EqualTo("-5"));
		Assert.That(p.EncodeLong(-5000000000L), Is.EqualTo("-5000000000"));
		Assert.That(p.EncodeDouble(-1.5), Is.EqualTo("-1.5"));
		Assert.That(p.DecodeToDouble("-1.5"), Is.EqualTo(-1.5));
		Assert.That(p.DecodeToInt("-5"), Is.EqualTo(-5));
		Assert.That(p.EncodeString("x"), Is.EqualTo("x"));
		Assert.That(p.DecodeToString("x"), Is.EqualTo("x"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: GZip                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>All GZip instances compress and decompress strings and bytes.</summary>
	[Test]
	public void GZip_RoundTrip()
	{
		string text = string.Concat(Enumerable.Repeat("Hello Spellbook, äöü €! ", 50));
		byte[] data = Encoding.UTF8.GetBytes(text);

		foreach(GZip i in new[] { GZip.BASE64, GZip.BASE62, GZip.INVERTED_BASE62, GZip.HEX, GZip.PLAINTEXT, new GZip() })
		{
			string enc = i.EncodeString(text);

			Assert.That(enc.Length, Is.LessThan(text.Length));
			Assert.That(i.DecodeToString(enc), Is.EqualTo(text));
			Assert.That(i.Decode(i.Encode(data)), Is.EqualTo(data));
			Assert.That(i.DecodeToInt(i.EncodeInt(-17)), Is.EqualTo(-17));
		}
	}


	/// <summary>Static compression without encoder produces text.</summary>
	[Test]
	public void GZip_Static()
	{
		string text = string.Concat(Enumerable.Repeat("abc", 100));

		Assert.That(GZip.Decompress(GZip.Compress(text)), Is.EqualTo(text));
		Assert.That(GZip.Decompress(GZip.Compress(text, Base62.DEFAULT), Base62.DEFAULT), Is.EqualTo(text));
		Assert.That(GZip.Decompress(GZip.Compress(new byte[] { 1, 2, 3 })), Is.EqualTo(new byte[] { 1, 2, 3 }));
		Assert.That(text.Compress().Decompress(), Is.EqualTo(text));
	}


	/// <summary>ddn text can be stored with GZip.</summary>
	[Test]
	public void GZip_Ddn()
	{
		DdnElement ddn = Ddn.Create("a = 1; s { b = text; }");
		string enc = ddn.ToString(GZip.PLAINTEXT);

		Assert.That(Ddn.Open(new MemoryStream(Encoding.UTF8.GetBytes(enc)), new Ddn.Options(GZip.PLAINTEXT))["s/b"].Value, Is.EqualTo("text"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: extensions and defaults                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Generic decoding supports all integer types, floating point numbers, strings and arrays.</summary>
	[Test]
	public void Extensions_GenericDecode()
	{
		Assert.That(((short) -5).ToBase62().FromBase62<short>(), Is.EqualTo(-5));
		Assert.That(((byte) 200).ToBase62().FromBase62<byte>(), Is.EqualTo(200));
		Assert.That(uint.MaxValue.ToBase62().FromBase62<uint>(), Is.EqualTo(uint.MaxValue));
		Assert.That(ulong.MaxValue.ToHex().FromHex<ulong>(), Is.EqualTo(ulong.MaxValue));
		Assert.That(1.5f.ToBase64().FromBase64<float>(), Is.EqualTo(1.5f));
		Assert.That(2.5.ToInvertedBase62().FromInvertedBase62<double>(), Is.EqualTo(2.5));
		Assert.That("abc".ToHex().FromHex<string>(), Is.EqualTo("abc"));
		Assert.That(new byte[] { 0, 1 }.ToBase62().FromBase62<byte[]>(), Is.EqualTo(new byte[] { 0, 1 }));
		Assert.That("xy".ToCharArray().ToBase64().FromBase64<char[]>(), Is.EqualTo("xy".ToCharArray()));
		Assert.Throws<ArgumentException>(() => "1".FromBase62<decimal>());
	}


	/// <summary>An encoder that only implements the required members gets the default implementations.</summary>
	[Test]
	public void Interface_Defaults()
	{
		IEncoder enc = new XorEncoder();

		Assert.That(enc.DecodeToString(enc.EncodeString("Hello")), Is.EqualTo("Hello"));
		Assert.That(enc.DecodeToInt(enc.EncodeInt(-42)), Is.EqualTo(-42));
		Assert.That(enc.DecodeToLong(enc.EncodeLong(long.MinValue)), Is.EqualTo(long.MinValue));
		Assert.That(enc.DecodeToDouble(enc.EncodeDouble(0.1)), Is.EqualTo(0.1));
		Assert.That(enc.DecodeToChars(enc.EncodeChars("ab".ToCharArray())), Is.EqualTo("ab".ToCharArray()));
		Assert.That(enc.DecodeToBytes(enc.EncodeBytes(new byte[] { 1, 2 }), 4), Is.EqualTo(new byte[] { 0, 0, 1, 2 }));
	}
}
