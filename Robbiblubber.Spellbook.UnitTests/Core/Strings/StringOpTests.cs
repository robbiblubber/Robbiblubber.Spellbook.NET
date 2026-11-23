namespace Robbiblubber.Spellbook.UnitTests.Core.Strings;

using System.Globalization;
using NUnit.Framework;



/// <summary>This class tests the string operations.</summary>
[TestFixture]
[NonParallelizable]
public class StringOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Culture before the test.</summary>
	private CultureInfo _Culture = CultureInfo.CurrentCulture;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Switches to German culture.</summary>
	[SetUp]
	public void SetUp()
	{
		_Culture = CultureInfo.CurrentCulture;
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
	}


	/// <summary>Restores the culture.</summary>
	[TearDown]
	public void TearDown()
	{
		CultureInfo.CurrentCulture = _Culture;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: random                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Random strings use every character of the alphabet.</summary>
	[Test]
	public void Random_UsesWholeAlphabet()
	{
		string r = StringOp.Random(2000, "ab");

		Assert.That(r, Has.Length.EqualTo(2000));
		Assert.That(r, Does.Contain("a").And.Contain("b"));
		Assert.That(StringOp.Random(5000), Does.Contain("Z").And.Contain("0"));
		Assert.That(StringOp.Random(StringOp.PASSWORD_ALPHABET), Has.Length.EqualTo(24));
		Assert.That(StringOp.Random(0), Is.Empty);
	}


	/// <summary>Invalid arguments throw.</summary>
	[Test]
	public void Random_Invalid_Throws()
	{
		Assert.Throws<ArgumentException>(() => StringOp.Random(5, string.Empty));
		Assert.Throws<ArgumentOutOfRangeException>(() => StringOp.Random(-1));
	}


	/// <summary>Unique strings are alphanumeric and do not repeat.</summary>
	[Test]
	public void Unique()
	{
		HashSet<string> set = new();
		for(int i = 0; i < 10000; i++) { Assert.That(set.Add(StringOp.Unique()), Is.True); }

		Assert.That(StringOp.Unique(), Has.Length.EqualTo(24));
		Assert.That(StringOp.Unique(8), Has.Length.EqualTo(8));
		Assert.That(StringOp.Unique().Complies(), Is.True);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: numbers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number conversion is identical to the previous implementation.</summary>
	/// <param name="value">Value.</param>
	/// <param name="alpha">Base 62.</param>
	/// <param name="bin">Base 2.</param>
	/// <param name="hex">Base 16.</param>
	/// <param name="b36">Base 36.</param>
	[TestCase(123456789L, "8m0Kx", "111010110111100110100010101", "75bcd15", "21i3v9")]
	[TestCase(638650000000000000L, "Lb1too3RBe", "100011011100111100001100101010010001110011011010000000000000", "8dcf0ca91cda000", "4uoej3ohnchs")]
	[TestCase(long.MaxValue, "aZl8N0y58M7", "111111111111111111111111111111111111111111111111111111111111111", "7fffffffffffffff", "1y2p0ij32e8e7")]
	[TestCase(-42L, "-G", "-101010", "-2a", "-16")]
	[TestCase(-1L, "-1", "-1", "-1", "-1")]
	public void Base_Compatible(long value, string alpha, string bin, string hex, string b36)
	{
		Assert.That(value.ToAlpha(), Is.EqualTo(alpha));
		Assert.That(value.ToString(2), Is.EqualTo(bin));
		Assert.That(value.ToString(16), Is.EqualTo(hex));
		Assert.That(value.ToString(36), Is.EqualTo(b36));

		Assert.That(alpha.ToInt64(62), Is.EqualTo(value));
		Assert.That(bin.ToInt64(2), Is.EqualTo(value));
		Assert.That(hex.ToInt64(16), Is.EqualTo(value));
		Assert.That(b36.ToInt64(36), Is.EqualTo(value));
	}


	/// <summary>Zero and the minimum value are converted.</summary>
	[Test]
	public void Base_ZeroAndMinimum()
	{
		Assert.That(0L.ToString(16), Is.EqualTo("0"));
		Assert.That(0.ToAlpha(), Is.EqualTo("0"));
		Assert.That(long.MinValue.ToString(16), Is.EqualTo("-8000000000000000"));
		Assert.That("-8000000000000000".ToInt64(16), Is.EqualTo(long.MinValue));
		Assert.That(((short) -300).ToString(10), Is.EqualTo("-300"));
		Assert.That(int.MinValue.ToString(62).ToInt32(62), Is.EqualTo(int.MinValue));
	}


	/// <summary>Upper case letters are accepted for bases up to 36; sign and white space are handled.</summary>
	[Test]
	public void Base_Parse()
	{
		Assert.That("FF".ToInt64(16), Is.EqualTo(255));
		Assert.That("ZZ".ToInt64(36), Is.EqualTo(1295));
		Assert.That(" +12 ".ToInt64(), Is.EqualTo(12));
		Assert.That("-12".ToInt32(), Is.EqualTo(-12));
		Assert.That("12".ToInt16(), Is.TypeOf<short>().And.EqualTo(12));
		Assert.That("Z".ToInt64(62), Is.EqualTo(61));
	}


	/// <summary>Invalid strings and overflows throw instead of returning wrong values.</summary>
	[Test]
	public void Base_Invalid_Throws()
	{
		Assert.Throws<FormatException>(() => "1x".ToInt64());
		Assert.Throws<FormatException>(() => "12".ToInt64(2));
		Assert.Throws<FormatException>(() => "".ToInt64());
		Assert.Throws<FormatException>(() => "-".ToInt64());
		Assert.Throws<FormatException>(() => "1 2".ToInt64());
		Assert.Throws<OverflowException>(() => "99999999999999999999".ToInt64());
		Assert.Throws<OverflowException>(() => "9223372036854775808".ToInt64());
		Assert.Throws<OverflowException>(() => "-8000000000000001".ToInt64(16));
		Assert.Throws<OverflowException>(() => "70000".ToInt16());
		Assert.Throws<OverflowException>(() => "3000000000".ToInt32());
		Assert.Throws<ArgumentException>(() => 5L.ToString(1));
		Assert.Throws<ArgumentException>(() => 5L.ToString(63));
	}


	/// <summary>Numbers are parsed with the invariant culture unless a provider is given.</summary>
	[Test]
	public void Numbers_Culture()
	{
		Assert.That("1.5".IsNumeric(), Is.True);
		Assert.That("1,5".IsNumeric(), Is.False);
		Assert.That("1e3".IsNumeric(), Is.True);
		Assert.That("1,5".IsNumeric(CultureInfo.CurrentCulture), Is.True);
		Assert.That("x".IsNumeric(), Is.False);
		Assert.That(((string?) null).IsNumeric(), Is.False);
		Assert.That("-15".IsNumericInteger(), Is.True);
		Assert.That("1.5".IsNumericInteger(), Is.False);
		Assert.That("42".ToInteger(), Is.EqualTo(42));
		Assert.That("x".ToInteger(7), Is.EqualTo(7));
		Assert.That("1.000".ToInteger(), Is.EqualTo(0));
	}


	/// <summary>Boolean values are recognized case-insensitively.</summary>
	/// <param name="s">String.</param>
	/// <param name="expected">Expected value.</param>
	[TestCase("true", true)]
	[TestCase("TRUE", true)]
	[TestCase("T", true)]
	[TestCase("yes", true)]
	[TestCase("Y", true)]
	[TestCase("On", true)]
	[TestCase(" yes ", true)]
	[TestCase("1", true)]
	[TestCase("-3", true)]
	[TestCase("1.5", true)]
	[TestCase("0", false)]
	[TestCase("0.0", false)]
	[TestCase("false", false)]
	[TestCase("no", false)]
	[TestCase("off", false)]
	[TestCase("Tschüss", false)]
	[TestCase("", false)]
	[TestCase(null, false)]
	public void ToBoolean(string? s, bool expected)
	{
		Assert.That(s.ToBoolean(), Is.EqualTo(expected));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: text                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The first character is made upper case.</summary>
	[Test]
	public void InitCap()
	{
		Assert.That("ärger".InitCap(), Is.EqualTo("Ärger"));
		Assert.That("i".InitCap(), Is.EqualTo("I"));
		Assert.That("i".InitCap(CultureInfo.GetCultureInfo("tr-TR")), Is.EqualTo("İ"));
		Assert.That(string.Empty.InitCap(), Is.Empty);
		Assert.That(((string?) null).InitCap(), Is.Empty);
	}


	/// <summary>Matching characters at the start are counted.</summary>
	[Test]
	public void MatchesFromStart()
	{
		Assert.That("abc".MatchesFromStart("abd"), Is.EqualTo(2));
		Assert.That("ABC".MatchesFromStart("abd"), Is.EqualTo(0));
		Assert.That("ABC".MatchesFromStart("abd", false), Is.EqualTo(2));
		Assert.That("ab".MatchesFromStart("abc"), Is.EqualTo(2));
		Assert.That("Straße".MatchesFromStart("STRASSE", false), Is.EqualTo(4));
	}


	/// <summary>Wildcard patterns are matched; regular expression characters are literal.</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="expected">Expected result.</param>
	[TestCase("file.txt", "*.txt", true)]
	[TestCase("file.txt", "*.TXT", false)]
	[TestCase("file.txt", "file.???", true)]
	[TestCase("file.txt", "file.??", false)]
	[TestCase("file.txt", "*", true)]
	[TestCase("", "*", true)]
	[TestCase("", "", true)]
	[TestCase("a", "", false)]
	[TestCase("a+b", "a+b", true)]
	[TestCase("aab", "a+b", false)]
	[TestCase("a(b)[c]", "a(*)[c]", true)]
	[TestCase(@"C:\temp\x.log", @"C:\temp\*.log", true)]
	[TestCase("abcabcabd", "*abd", true)]
	[TestCase("abcabcabd", "a*c*d", true)]
	[TestCase("abcabcabd", "a*c*e", false)]
	[TestCase("mississippi", "m*iss*ppi", true)]
	public void Matches(string expression, string pattern, bool expected)
	{
		Assert.That(expression.Matches(pattern), Is.EqualTo(expected));
	}


	/// <summary>Matching can be case-insensitive and use several patterns.</summary>
	[Test]
	public void Matches_Options()
	{
		Assert.That("file.txt".Matches(false, "*.TXT"), Is.True);
		Assert.That("file.txt".Matches("*.doc", "*.txt"), Is.True);
		Assert.That("file.txt".Matches(new List<string> { "*.doc" }), Is.False);
		Assert.That("file.txt".Matches(false, (IEnumerable<string>?) null), Is.False);
		Assert.That("file.txt".Matches(true, (string?) null), Is.False);
		Assert.That("file.txt".Matches(Array.Empty<string>()), Is.False);
	}


	/// <summary>The maximum length is determined.</summary>
	[Test]
	public void MaxLength()
	{
		Assert.That(new[] { "a", "abc", null, "ab" }.MaxLength(), Is.EqualTo(3));
		Assert.That(Array.Empty<string>().MaxLength(), Is.EqualTo(0));
	}


	/// <summary>Allowed characters are checked.</summary>
	[Test]
	public void Complies()
	{
		Assert.That("abc123XYZ".Complies(), Is.True);
		Assert.That("abc-123".Complies(), Is.False);
		Assert.That("abc-123".Complies("abc123-"), Is.True);
		Assert.That(string.Empty.Complies(), Is.True);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: parts                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parts before and after separators are returned; the whole string if the separator is missing.</summary>
	[Test]
	public void BeforeAndAfter()
	{
		string s = "a.b.c";

		Assert.That(s.BeforeFirst('.'), Is.EqualTo("a"));
		Assert.That(s.BeforeLast('.'), Is.EqualTo("a.b"));
		Assert.That(s.AfterFirst('.'), Is.EqualTo("b.c"));
		Assert.That(s.AfterLast('.'), Is.EqualTo("c"));
		Assert.That(s.BeforeFirst(".b"), Is.EqualTo("a"));
		Assert.That(s.BeforeLast("."), Is.EqualTo("a.b"));
		Assert.That(s.AfterFirst("a."), Is.EqualTo("b.c"));
		Assert.That(s.AfterLast(".c"), Is.Empty);

		Assert.That(s.BeforeFirst('x'), Is.EqualTo(s));
		Assert.That(s.BeforeLast("x"), Is.EqualTo(s));
		Assert.That(s.AfterFirst('x'), Is.EqualTo(s));
		Assert.That(s.AfterLast("x"), Is.EqualTo(s));
	}


	/// <summary>Snipping is identical to the previous implementation.</summary>
	/// <param name="from">Start occurrence.</param>
	/// <param name="to">End occurrence.</param>
	/// <param name="expected">Expected result.</param>
	[TestCase(1, 3, "b c")]
	[TestCase(0, 2, "a b")]
	[TestCase(2, 0, "c d")]
	[TestCase(3, 2, "d")]
	[TestCase(5, 0, "")]
	[TestCase(2, 9, "c d")]
	[TestCase(2, 2, "")]
	public void Snip_Compatible(int from, int to, string expected)
	{
		Assert.That("a b c d".Snip(from, to), Is.EqualTo(expected));
	}


	/// <summary>A delimiter at the start of the string is found as end delimiter.</summary>
	[Test]
	public void Snip_DelimiterAtStart()
	{
		Assert.That(" a b".Snip(0, 1), Is.Empty);
		Assert.That("a,b;c".Snip(',', ';'), Is.EqualTo("b;c"));
		Assert.That("a,b;c".Snip(1, 2, ',', ';'), Is.EqualTo("b"));
		Assert.That("a,b;c".Snip(2, ',', ';'), Is.EqualTo("c"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: replace                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Case-insensitive replacement terminates and replaces every occurrence once.</summary>
	[Test]
	public void ReplaceAll()
	{
		Assert.That("aaa".ReplaceAll("a", "A", false), Is.EqualTo("AAA"));
		Assert.That("ab".ReplaceAll("A", "aa", false), Is.EqualTo("aab"));
		Assert.That("Hello hello".ReplaceAll("hello", "bye"), Is.EqualTo("Hello bye"));
		Assert.That("Hello hello".ReplaceAll("HELLO", "bye", false), Is.EqualTo("bye bye"));
		Assert.That("İx".ReplaceAll("X", "y", false), Is.EqualTo("İy"));
		Assert.Throws<ArgumentException>(() => "ab".ReplaceAll(string.Empty, "x"));
	}


	/// <summary>Only the first occurrence is replaced.</summary>
	[Test]
	public void ReplaceFirst()
	{
		Assert.That("a.b.c".ReplaceFirst(".", "-"), Is.EqualTo("a-b.c"));
		Assert.That("A.a".ReplaceFirst("a", "x", false), Is.EqualTo("x.a"));
		Assert.That("abc".ReplaceFirst("x", "y"), Is.EqualTo("abc"));
		Assert.Throws<ArgumentException>(() => "ab".ReplaceFirst(string.Empty, "x"));
	}


	/// <summary>Repeated occurrences at the start and end are replaced as before; empty texts throw.</summary>
	[Test]
	public void ReplaceStartAndEnd()
	{
		Assert.That("xxab".ReplaceStart("x", "yz"), Is.EqualTo("yzyzab"));
		Assert.That("abXXx".ReplaceEnd("x", "-", false), Is.EqualTo("ab---"));
		Assert.That("abXXx".ReplaceEnd("x", "-"), Is.EqualTo("abXX-"));
		Assert.That("ab".ReplaceStart("x", "y"), Is.EqualTo("ab"));
		Assert.That("xx".ReplaceStart("x", ""), Is.Empty);
		Assert.Throws<ArgumentException>(() => "ab".ReplaceStart(string.Empty, "x"));
		Assert.Throws<ArgumentException>(() => "ab".ReplaceEnd(string.Empty, "x"));
	}
}
