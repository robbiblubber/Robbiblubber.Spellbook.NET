namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests writing of ddn text.</summary>
[TestFixture]
public class DdnWriterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a sample document.</summary>
	/// <returns>Returns the document.</returns>
	private static DdnElement _CreateSample()
	{
		DdnElement rval = Ddn.Create();
		rval["name"] = "Sample";
		rval["window/size"] = new[] { 800, 600 };
		rval["window/state"] = DayOfWeek.Monday;
		rval["count"] = 3;
		rval["empty"].SetNull();

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default output: multi-line, ordered, values before sections, section spacing.</summary>
	[Test]
	public void ToString_Default()
	{
		Assert.That(_CreateSample().ToString(), Is.EqualTo(
			"count = 3;\r\n" +
			"empty = \\0;\r\n" +
			"name = Sample;\r\n" +
			"\r\n" +
			"window\r\n" +
			"{\r\n" +
			"    size = 800, 600;\r\n" +
			"    state = Monday;\r\n" +
			"}\r\n"));
	}


	/// <summary>Single-line output.</summary>
	[Test]
	public void ToString_SingleLine()
	{
		Assert.That(_CreateSample().ToString(Ddn.Options.SINGLE_LINE),
					Is.EqualTo("count = 3; empty = \\0; name = Sample; window { size = 800, 600; state = Monday; }"));
	}


	/// <summary>Unordered output keeps the insertion order, values first.</summary>
	[Test]
	public void ToString_Unordered()
	{
		Assert.That(_CreateSample().ToString(new Ddn.Options { Ordered = false, MultiLine = false }),
					Is.EqualTo("name = Sample; count = 3; empty = \\0; window { size = 800, 600; state = Monday; }"));
	}


	/// <summary>NULL values can be omitted.</summary>
	[Test]
	public void ToString_IgnoreNull()
	{
		Assert.That(_CreateSample().ToString(new Ddn.Options { IgnoreNullElements = true, MultiLine = false }), Does.Not.Contain("empty"));
	}


	/// <summary>Indentation, line break and section spacing can be configured.</summary>
	[Test]
	public void ToString_Formatting()
	{
		DdnElement ddn = Ddn.Create("a = 1; s { b = 2; t { c = 3; } }");

		Assert.That(ddn.ToString(new Ddn.Options { Indent = "\t", NewLine = "\n", SectionSpacing = false }),
					Is.EqualTo("a = 1;\ns\n{\n\tb = 2;\n\tt\n\t{\n\t\tc = 3;\n\t}\n}\n"));
	}


	/// <summary>Numbers in names are ordered numerically.</summary>
	[Test]
	public void ToString_NaturalOrder()
	{
		DdnElement ddn = Ddn.Create();
		foreach(string i in new[] { "[10]", "[2]", "[1]", "b", "a10", "a9", "A" }) { ddn[i] = 0; }

		Assert.That(ddn.ToString(Ddn.Options.SINGLE_LINE), Is.EqualTo("A = 0; [1] = 0; [2] = 0; [10] = 0; a9 = 0; a10 = 0; b = 0;"));
	}


	/// <summary>Empty sections are written as sections.</summary>
	[Test]
	public void ToString_EmptySection()
	{
		DdnElement ddn = Ddn.Create("s { }");

		Assert.That(ddn.ToString(), Is.EqualTo("s\r\n{\r\n}\r\n"));
		Assert.That(Ddn.Create(ddn.ToString())["s"].IsSection, Is.True);
	}


	/// <summary>Elements of a section and value statements.</summary>
	[Test]
	public void ToString_SectionAndValue()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; } v = x;");

		Assert.That(ddn["s"].ToString(Ddn.Options.SINGLE_LINE), Is.EqualTo("a = 1;"));
		Assert.That(ddn["v"].ToString(Ddn.Options.SINGLE_LINE), Is.EqualTo("v = x;"));
		Assert.That(ddn["s"].Text, Is.EqualTo("a = 1;\r\n"));
	}


	/// <summary>Names are masked.</summary>
	[Test]
	public void ToString_MaskedNames()
	{
		DdnElement ddn = Ddn.Create();
		ddn.Element(" a=b/c ").Set(1);

		Assert.That(ddn.ToString(Ddn.Options.SINGLE_LINE), Is.EqualTo(@"\_a\=b\/c\_ = 1;"));
		Assert.That(Ddn.Create(ddn.ToString()).Element(" a=b/c ").Get<int>(), Is.EqualTo(1));
	}


	/// <summary>Parsed values are written unchanged.</summary>
	[Test]
	public void ToString_ParsedValuesUnchanged()
	{
		const string TEXT = "url = https://robbiblubber.org;\r\nvals = 1.2, 1.4;\r\nwinpath = \\\\\\\\svr1\\\\folder;\r\n";

		Assert.That(Ddn.Create(TEXT).ToString(), Is.EqualTo(TEXT));
	}


	/// <summary>Writing and reading a document gives the same document.</summary>
	[Test]
	public void RoundTrip()
	{
		DdnElement ddn = _CreateSample();
		ddn["deep/a/b/c/d"] = "x;y";
		ddn["deep/a/list"] = new[] { "1,2", "3" };

		string text = ddn.ToString();
		Assert.That(Ddn.Create(text).ToString(), Is.EqualTo(text));
		Assert.That(Ddn.Create(ddn.ToString(Ddn.Options.SINGLE_LINE)).ToString(), Is.EqualTo(text));
	}
}
