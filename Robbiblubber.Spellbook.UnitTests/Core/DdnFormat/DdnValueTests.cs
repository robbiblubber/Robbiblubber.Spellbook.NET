namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using System.Globalization;
using NUnit.Framework;



/// <summary>This class tests value conversion of ddn elements.</summary>
[TestFixture]
[NonParallelizable]
public class DdnValueTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Culture before the test.</summary>
	private CultureInfo _Culture = CultureInfo.CurrentCulture;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
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


	/// <summary>Numbers are written and read independent of the culture.</summary>
	[Test]
	public void Numbers_Invariant()
	{
		DdnElement ddn = Ddn.Create();
		ddn["i"] = -42;
		ddn["l"] = long.MinValue;
		ddn["ul"] = ulong.MaxValue;
		ddn["f"] = -1.5f;
		ddn["d"] = 3.1415926536;
		ddn["m"] = -1234567.891m;
		ddn["b"] = (byte) 255;
		ddn["sb"] = (sbyte) -128;
		ddn["s"] = (short) -32768;
		ddn["us"] = (ushort) 65535;
		ddn["ui"] = uint.MaxValue;

		string text = ddn.ToString();
		Assert.That(text, Does.Contain("i = -42;"));
		Assert.That(text, Does.Contain("d = 3.1415926536;"));
		Assert.That(text, Does.Contain("m = -1234567.891;"));

		DdnElement read = Ddn.Create(text);
		Assert.That(read.Get<int>("i"), Is.EqualTo(-42));
		Assert.That(read.Get<long>("l"), Is.EqualTo(long.MinValue));
		Assert.That(read.Get<ulong>("ul"), Is.EqualTo(ulong.MaxValue));
		Assert.That(read.Get<float>("f"), Is.EqualTo(-1.5f));
		Assert.That(read.Get<double>("d"), Is.EqualTo(3.1415926536));
		Assert.That(read.Get<decimal>("m"), Is.EqualTo(-1234567.891m));
		Assert.That(read.Get<byte>("b"), Is.EqualTo(255));
		Assert.That(read.Get<sbyte>("sb"), Is.EqualTo(-128));
		Assert.That(read.Get<short>("s"), Is.EqualTo(-32768));
		Assert.That(read.Get<ushort>("us"), Is.EqualTo(65535));
		Assert.That(read.Get<uint>("ui"), Is.EqualTo(uint.MaxValue));
	}


	/// <summary>Doubles are written so that they can be read back exactly.</summary>
	[Test]
	public void Double_RoundTrip()
	{
		double[] values = { 0.1, 1.0 / 3.0, double.MaxValue, double.Epsilon, -0.0, 1e-300 };
		DdnElement ddn = Ddn.Create();
		ddn["v"] = values;

		Assert.That(Ddn.Create(ddn.ToString()).Get<double[]>("v"), Is.EqualTo(values));
	}


	/// <summary>Floating point values with leading decimal point are read.</summary>
	[Test]
	public void Float_LeadingDecimalPoint()
	{
		DdnElement ddn = Ddn.Create("a = .5; b = -.25; c = 1e3;");

		Assert.That(ddn.Get<double>("a"), Is.EqualTo(0.5));
		Assert.That(ddn.Get<decimal>("b"), Is.EqualTo(-0.25m));
		Assert.That(ddn.Get<float>("c"), Is.EqualTo(1000f));
	}


	/// <summary>Invalid values return the default value or NULL.</summary>
	[Test]
	public void InvalidValues()
	{
		DdnElement ddn = Ddn.Create("a = abc; b = 1,5; c = 99999999999; d = ;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(0));
		Assert.That(ddn.Get<int?>("a"), Is.Null);
		Assert.That(ddn.Get("a", 7), Is.EqualTo(7));
		Assert.That(ddn["a"].TryGet(out int _), Is.False);
		Assert.That(ddn.Get<double>("b"), Is.EqualTo(0));
		Assert.That(ddn.Get<int?>("c"), Is.Null);
		Assert.That(ddn.Get<int?>("d"), Is.Null);
		Assert.That(ddn.Get<string>("d"), Is.EqualTo(string.Empty));
	}


	/// <summary>Booleans.</summary>
	[Test]
	public void Booleans()
	{
		DdnElement ddn = Ddn.Create("a = true; b = False; c = 1; d = 0; e = yes; f = no; g = maybe; h = -1;");

		Assert.That(ddn.Get<bool>("a"), Is.True);
		Assert.That(ddn.Get<bool>("b"), Is.False);
		Assert.That(ddn.Get<bool>("c"), Is.True);
		Assert.That(ddn.Get<bool>("d"), Is.False);
		Assert.That(ddn.Get<bool>("e"), Is.True);
		Assert.That(ddn.Get<bool?>("f"), Is.False);
		Assert.That(ddn.Get<bool?>("g"), Is.Null);
		Assert.That(ddn.Get<bool>("h"), Is.True);

		ddn["x"] = true;
		ddn["y"] = false;
		Assert.That(ddn.ToString(), Does.Contain("x = true;").And.Contain("y = false;"));
	}


	/// <summary>Strings with special characters are masked and read back.</summary>
	[Test]
	public void Strings_RoundTrip()
	{
		string[] values = { "plain", "  padded  ", "a=b;c,d{e}f/g\\h", "line1\r\nline2", "tab\there", "", " ", "//comment?", "/*x*/", @"\0", "ä€😀" };
		DdnElement ddn = Ddn.Create();

		for(int i = 0; i < values.Length; i++) { ddn["s" + i] = values[i]; }
		DdnElement read = Ddn.Create(ddn.ToString());

		for(int i = 0; i < values.Length; i++)
		{
			Assert.That(read.Get<string>("s" + i), Is.EqualTo(values[i].Replace("\r\n", Environment.NewLine)), "value " + i);
		}
	}


	/// <summary>Characters.</summary>
	[Test]
	public void Chars()
	{
		DdnElement ddn = Ddn.Create();
		ddn["a"] = 'x';
		ddn["b"] = ';';
		ddn["c"] = ' ';

		DdnElement read = Ddn.Create(ddn.ToString());
		Assert.That(read.Get<char>("a"), Is.EqualTo('x'));
		Assert.That(read.Get<char>("b"), Is.EqualTo(';'));
		Assert.That(read.Get<char>("c"), Is.EqualTo(' '));
		Assert.That(read.Get<char?>("missing"), Is.Null);
	}


	/// <summary>Timestamps are written with time zone and read as local times.</summary>
	[Test]
	public void DateTime_RoundTrip()
	{
		DateTime local = new(2026, 10, 19, 8, 15, 30, 123, DateTimeKind.Local);
		DateTime utc = new(2026, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);
		DdnElement ddn = Ddn.Create();

		ddn["local"] = local;
		ddn["utc"] = utc;

		Assert.That(ddn.ToString(), Does.Contain("utc = 2026-01-02 03:04:05.006+00:00;"));

		DdnElement read = Ddn.Create(ddn.ToString());
		Assert.That(read.Get<DateTime>("local"), Is.EqualTo(local));
		Assert.That(read.Get<DateTime>("local").Kind, Is.EqualTo(DateTimeKind.Local));
		Assert.That(read.Get<DateTime>("utc").ToUniversalTime(), Is.EqualTo(utc));
	}


	/// <summary>Timestamp parts may be omitted, except the date.</summary>
	[Test]
	public void DateTime_PartialTimestamps()
	{
		DdnElement ddn = Ddn.Create("""
			d1 = 1977-08-14;
			d2 = 1977-08-14 15:27;
			d3 = 1977-08-14 15:27:22;
			d4 = 1977-08-14 15:27:22.926;
			d5 = 1977-08-14 15:27:22.926+02:00;
			d6 = 1977-08-14 15:27:22.926-05:30;
			d7 = 1977-08-14T15:27:22Z;
			d8 = 1977-08-14 15:27:22+0200;
			d9 = 1977-08-14 15:27:22.1234567+00:00;
			x1 = 14.08.1977;
			x2 = 1977-13-01;
			""");

		Assert.That(ddn.Get<DateTime>("d1"), Is.EqualTo(new DateTime(1977, 8, 14)));
		Assert.That(ddn.Get<DateTime>("d2"), Is.EqualTo(new DateTime(1977, 8, 14, 15, 27, 0)));
		Assert.That(ddn.Get<DateTime>("d3"), Is.EqualTo(new DateTime(1977, 8, 14, 15, 27, 22)));
		Assert.That(ddn.Get<DateTime>("d4"), Is.EqualTo(new DateTime(1977, 8, 14, 15, 27, 22, 926)));
		Assert.That(ddn.Get<DateTime>("d5").ToUniversalTime(), Is.EqualTo(new DateTime(1977, 8, 14, 13, 27, 22, 926)));
		Assert.That(ddn.Get<DateTime>("d6").ToUniversalTime(), Is.EqualTo(new DateTime(1977, 8, 14, 20, 57, 22, 926)));
		Assert.That(ddn.Get<DateTime>("d7").ToUniversalTime(), Is.EqualTo(new DateTime(1977, 8, 14, 15, 27, 22)));
		Assert.That(ddn.Get<DateTime>("d8").ToUniversalTime(), Is.EqualTo(new DateTime(1977, 8, 14, 13, 27, 22)));
		Assert.That(ddn.Get<DateTime>("d9").ToUniversalTime().Ticks % TimeSpan.TicksPerSecond, Is.EqualTo(1234567));
		Assert.That(ddn.Get<DateTime?>("x1"), Is.Null);
		Assert.That(ddn.Get<DateTime?>("x2"), Is.Null);
	}


	/// <summary>Other date and time types.</summary>
	[Test]
	public void DateTimeTypes_RoundTrip()
	{
		DateTimeOffset dto = new(2026, 10, 19, 8, 15, 30, 500, TimeSpan.FromHours(-3.5));
		DateOnly date = new(1977, 8, 14);
		TimeOnly time = new(15, 27, 22, 926);
		TimeSpan span = new(3, 4, 5, 6, 7);
		DdnElement ddn = Ddn.Create();

		ddn["dto"] = dto;
		ddn["date"] = date;
		ddn["time"] = time;
		ddn["span"] = span;
		ddn["neg"] = -span;

		string text = ddn.ToString();
		Assert.That(text, Does.Contain("dto = 2026-10-19 08:15:30.500-03:30;"));
		Assert.That(text, Does.Contain("date = 1977-08-14;"));
		Assert.That(text, Does.Contain("time = 15:27:22.926;"));

		DdnElement read = Ddn.Create(text);
		Assert.That(read.Get<DateTimeOffset>("dto"), Is.EqualTo(dto));
		Assert.That(read.Get<DateTimeOffset>("dto").Offset, Is.EqualTo(dto.Offset));
		Assert.That(read.Get<DateOnly>("date"), Is.EqualTo(date));
		Assert.That(read.Get<TimeOnly>("time"), Is.EqualTo(time));
		Assert.That(read.Get<TimeSpan>("span"), Is.EqualTo(span));
		Assert.That(read.Get<TimeSpan>("neg"), Is.EqualTo(-span));
		Assert.That(Ddn.Create("t = 7:05;").Get<TimeOnly>("t"), Is.EqualTo(new TimeOnly(7, 5)));
	}


	/// <summary>GUIDs and types with type converters.</summary>
	[Test]
	public void Guid_And_TypeConverters()
	{
		Guid g = Guid.NewGuid();
		DdnElement ddn = Ddn.Create();

		ddn["g"] = g;
		ddn.Set("v", new Version(1, 2, 3, 4));
		ddn.Set("u", new Uri("https://robbiblubber.org/a?b=c"));

		DdnElement read = Ddn.Create(ddn.ToString());
		Assert.That(read.Get<Guid>("g"), Is.EqualTo(g));
		Assert.That(read.Get<Version>("v"), Is.EqualTo(new Version(1, 2, 3, 4)));
		Assert.That(read.Get<Uri>("u"), Is.EqualTo(new Uri("https://robbiblubber.org/a?b=c")));
	}


	/// <summary>Enums are written as names and read from names and numbers.</summary>
	[Test]
	public void Enums()
	{
		DdnElement ddn = Ddn.Create();
		ddn["a"] = DayOfWeek.Friday;
		ddn["b"] = (int) DayOfWeek.Friday;
		ddn["f"] = FileAttributes.ReadOnly | FileAttributes.Hidden;
		ddn["u"] = _UShortEnum.SECOND;
		ddn["l"].Set(new[] { DayOfWeek.Monday, DayOfWeek.Sunday });

		string text = ddn.ToString();
		Assert.That(text, Does.Contain("a = Friday;"));
		Assert.That(text, Does.Contain("b = 5;"));
		Assert.That(text, Does.Contain(@"f = ReadOnly\, Hidden;"));
		Assert.That(text, Does.Contain("l = Monday, Sunday;"));

		DdnElement read = Ddn.Create(text + "c = friday; d = 99; e = Nonsense; g = 2, Saturday;");
		Assert.That(read.Get<DayOfWeek>("a"), Is.EqualTo(DayOfWeek.Friday));
		Assert.That(read.Get<DayOfWeek>("b"), Is.EqualTo(DayOfWeek.Friday));
		Assert.That(read.Get<DayOfWeek>("c"), Is.EqualTo(DayOfWeek.Friday));
		Assert.That(read.Get<DayOfWeek>("d"), Is.EqualTo((DayOfWeek) 99));
		Assert.That(read.Get<DayOfWeek?>("e"), Is.Null);
		Assert.That(read.Get<FileAttributes>("f"), Is.EqualTo(FileAttributes.ReadOnly | FileAttributes.Hidden));
		Assert.That(read.Get<_UShortEnum>("u"), Is.EqualTo(_UShortEnum.SECOND));
		Assert.That(read.Get<DayOfWeek[]>("l"), Is.EqualTo(new[] { DayOfWeek.Monday, DayOfWeek.Sunday }));
		Assert.That(read.Get<List<DayOfWeek>>("g"), Is.EqualTo(new[] { DayOfWeek.Tuesday, DayOfWeek.Saturday }));
	}


	/// <summary>Arrays of all supported types.</summary>
	[Test]
	public void Arrays_RoundTrip()
	{
		DdnElement ddn = Ddn.Create();
		ddn["s"] = new[] { "Peter", "Paul, Mary", " x ", "" };
		ddn["sn"] = new string?[] { "a", null, "b" };
		ddn["i"] = new[] { 1, -2, 3 };
		ddn["in"] = new int?[] { 1, null, 3 };
		ddn["l"] = new[] { long.MaxValue, long.MinValue };
		ddn["ln"] = new long?[] { null, 5 };
		ddn["b"] = new[] { true, false };
		ddn["by"] = new byte[] { 0, 128, 255 };
		ddn["dt"] = new[] { new DateTime(2000, 1, 1), new DateTime(2001, 2, 3, 4, 5, 6) };
		ddn["e"] = Array.Empty<int>();

		DdnElement read = Ddn.Create(ddn.ToString());
		Assert.That(read.Get<string[]>("s"), Is.EqualTo(new[] { "Peter", "Paul, Mary", " x ", "" }));
		Assert.That(read.Get<string?[]>("sn"), Is.EqualTo(new string?[] { "a", null, "b" }));
		Assert.That(read.Get<int[]>("i"), Is.EqualTo(new[] { 1, -2, 3 }));
		Assert.That(read.Get<int?[]>("in"), Is.EqualTo(new int?[] { 1, null, 3 }));
		Assert.That(read.Get<int[]>("in"), Is.EqualTo(new[] { 1, 0, 3 }));
		Assert.That(read.Get<long[]>("l"), Is.EqualTo(new[] { long.MaxValue, long.MinValue }));
		Assert.That(read.Get<long?[]>("ln"), Is.EqualTo(new long?[] { null, 5 }));
		Assert.That(read.Get<bool[]>("b"), Is.EqualTo(new[] { true, false }));
		Assert.That(read.Get<byte[]>("by"), Is.EqualTo(new byte[] { 0, 128, 255 }));
		Assert.That(read.Get<DateTime[]>("dt"), Is.EqualTo(new[] { new DateTime(2000, 1, 1), new DateTime(2001, 2, 3, 4, 5, 6) }));
		Assert.That(read.Get<int[]>("e"), Is.Empty);
		Assert.That(read.Get<int[]>("missing"), Is.Null);
	}


	/// <summary>Collections of various types.</summary>
	[Test]
	public void Collections()
	{
		DdnElement ddn = Ddn.Create("v = 3, 1, 2, 1;");

		Assert.That(ddn.Get<List<int>>("v"), Is.EqualTo(new[] { 3, 1, 2, 1 }));
		Assert.That(ddn.Get<IList<int>>("v"), Is.EqualTo(new[] { 3, 1, 2, 1 }));
		Assert.That(ddn.Get<IEnumerable<int>>("v"), Is.EqualTo(new[] { 3, 1, 2, 1 }));
		Assert.That(ddn.Get<IReadOnlyList<long>>("v"), Is.EqualTo(new[] { 3L, 1, 2, 1 }));
		Assert.That(ddn.Get<HashSet<int>>("v"), Is.EquivalentTo(new[] { 1, 2, 3 }));
		Assert.That(ddn.Get<SortedSet<int>>("v"), Is.EqualTo(new[] { 1, 2, 3 }));
		Assert.That(ddn.Get<System.Collections.ObjectModel.Collection<int>>("v"), Is.EqualTo(new[] { 3, 1, 2, 1 }));

		ddn["h"].Set(new HashSet<string> { "x" });
		Assert.That(ddn.Get<string[]>("h"), Is.EqualTo(new[] { "x" }));
	}


	/// <summary>A single value can be read as array with one item and an array can be read as string.</summary>
	[Test]
	public void ArraysAndStrings()
	{
		DdnElement ddn = Ddn.Create(@"a = 5; b = Peter, Paul\, Mary;");

		Assert.That(ddn.Get<int[]>("a"), Is.EqualTo(new[] { 5 }));
		Assert.That(ddn.Get<string>("b"), Is.EqualTo("Peter, Paul, Mary"));
		Assert.That(ddn.Get<string[]>("b"), Is.EqualTo(new[] { "Peter", "Paul, Mary" }));
	}


	/// <summary>Complex types cannot be read from a value element.</summary>
	[Test]
	public void ComplexType_FromValue_ReturnsDefault()
	{
		DdnElement ddn = Ddn.Create("a = 1;");

		Assert.That(ddn["a"].Get<Dictionary<string, int>>(), Is.Null);
		Assert.That(ddn["a"].Get<DdnValueTests>(), Is.Null);
	}


	/// <summary>Sections and NULL return no value.</summary>
	[Test]
	public void SectionsAndNull()
	{
		DdnElement ddn = Ddn.Create(@"s { a = 1; } n = \0;");

		Assert.That(ddn.Get<int?>("s"), Is.Null);
		Assert.That(ddn.Get<string>("s"), Is.Null);
		Assert.That(ddn.Get<string>("n"), Is.Null);
		Assert.That(ddn.Get("n", "default"), Is.EqualTo("default"));
		Assert.That(ddn.Get<int[]>("n"), Is.Null);
		Assert.That(ddn["n"].GetWithDefault(3), Is.EqualTo(3));
		Assert.That(ddn["s"].Get(typeof(int)), Is.Null);
		Assert.That(ddn.Get(typeof(int), "s/a"), Is.EqualTo(1));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] _UShortEnum                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Enum with ushort as underlying type.</summary>
	private enum _UShortEnum: ushort
	{
		/// <summary>First value.</summary>
		FIRST = 1,
		/// <summary>Second value.</summary>
		SECOND = 2
	}
}
