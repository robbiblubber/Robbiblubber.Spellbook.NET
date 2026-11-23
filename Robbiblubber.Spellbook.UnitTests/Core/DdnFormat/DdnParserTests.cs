namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests parsing of ddn text.</summary>
[TestFixture]
public class DdnParserTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sample from the ddn specification (with the missing semicolon added).</summary>
	private const string _SPEC_SAMPLE = """
		type = T;                                  // value
		time = 2022-11-19 05:33:20+00:00;
		winpath = \\\\svr1\\folder;
		sec1                                       // section
		{
		   type = S;
		   fab = Cotton;
		   n = 20;
		   sub1                                    // subsection
		   {
			  vals = 1.2, 1.4, 1.3;
		   }
		   sub2
		   {
			  vals = 2.3, 2.6, 1.7;
		   }
		}
		sec2
		{
		   /* a multi line comment in a single line */
		   type = X;
		   unused = true;
		}
		""";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses the sample of the specification.</summary>
	[Test]
	public void Parse_SpecificationSample()
	{
		DdnElement ddn = Ddn.Create(_SPEC_SAMPLE);

		Assert.That(ddn.Get<string>("type"), Is.EqualTo("T"));
		Assert.That(ddn.Get<DateTime>("time"), Is.EqualTo(new DateTime(2022, 11, 19, 5, 33, 20, DateTimeKind.Utc).ToLocalTime()));
		Assert.That(ddn.Get<string>("winpath"), Is.EqualTo(@"\\svr1\folder"));
		Assert.That(ddn.Get<string>("sec1/fab"), Is.EqualTo("Cotton"));
		Assert.That(ddn.Get<int>("sec1/n"), Is.EqualTo(20));
		Assert.That(ddn.Get<double[]>("sec1/sub1/vals"), Is.EqualTo(new[] { 1.2, 1.4, 1.3 }));
		Assert.That(ddn.Get<double[]>("/sec1/sub2/vals"), Is.EqualTo(new[] { 2.3, 2.6, 1.7 }));
		Assert.That(ddn.Get<bool>("sec2/unused"), Is.True);
		Assert.That(ddn.Get<string>("sec2/type"), Is.EqualTo("X"));
		Assert.That(ddn.ElementCount, Is.EqualTo(5));
		Assert.That(ddn["sec1"].Sections.Select(m => m.Name), Is.EqualTo(new[] { "sub1", "sub2" }));
	}


	/// <summary>Parses masked characters.</summary>
	[Test]
	public void Parse_MaskedCharacters()
	{
		DdnElement ddn = Ddn.Create(@"a = x\=y\;z\,w\{v\}u\/t\\s; b = \_padded\_; c = line1\nline2\ttab; d = \0; e = ;");

		Assert.That(ddn.Get<string>("a"), Is.EqualTo(@"x=y;z,w{v}u/t\s"));
		Assert.That(ddn.Get<string>("b"), Is.EqualTo(" padded "));
		Assert.That(ddn.Get<string>("c"), Is.EqualTo("line1" + Environment.NewLine + "line2\ttab"));
		Assert.That(ddn["d"].IsNull, Is.True);
		Assert.That(ddn.Get<string>("d"), Is.Null);
		Assert.That(ddn["e"].IsNull, Is.False);
		Assert.That(ddn.Get<string>("e"), Is.EqualTo(string.Empty));
	}


	/// <summary>Parses masked names.</summary>
	[Test]
	public void Parse_MaskedNames()
	{
		DdnElement ddn = Ddn.Create(@"a\/b = 1; \_x\_ = 2; s\=t { [0] = 3; }");

		Assert.That(ddn.Element("a/b").Get<int>(), Is.EqualTo(1));
		Assert.That(ddn[@"a\/b"].Get<int>(), Is.EqualTo(1));
		Assert.That(ddn.Element(" x ").Get<int>(), Is.EqualTo(2));
		Assert.That(ddn["s=t/[0]"].Get<int>(), Is.EqualTo(3));
	}


	/// <summary>Parses comments.</summary>
	[Test]
	public void Parse_Comments()
	{
		DdnElement ddn = Ddn.Create("""
			// comment at start
			a = 1; // trailing comment
			b = 2;// directly after semicolon
			/* multi
			   line */ c = 3;
			d = x /* inside */ y;
			e = 5;/* after semicolon */
			""");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("c"), Is.EqualTo(3));
		Assert.That(ddn.Get<string>("d"), Is.EqualTo("x   y"));
		Assert.That(ddn.Get<int>("e"), Is.EqualTo(5));
		Assert.That(ddn.ElementCount, Is.EqualTo(5));
	}


	/// <summary>Double slashes inside values are not comments.</summary>
	[Test]
	public void Parse_SlashesInValues()
	{
		DdnElement ddn = Ddn.Create("url = https://robbiblubber.org/path; file = c:/temp//x; next = 1;");

		Assert.That(ddn.Get<string>("url"), Is.EqualTo("https://robbiblubber.org/path"));
		Assert.That(ddn.Get<string>("file"), Is.EqualTo("c:/temp//x"));
		Assert.That(ddn.Get<int>("next"), Is.EqualTo(1));
	}


	/// <summary>A comment after an equals sign makes the value empty.</summary>
	[Test]
	public void Parse_CommentAfterEquals()
	{
		DdnElement ddn = Ddn.Create("a =// comment\n5; b = 6;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(5));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(6));
	}


	/// <summary>The parser accepts missing semicolons before closing brackets and at the end.</summary>
	[Test]
	public void Parse_MissingSemicolons()
	{
		DdnElement ddn = Ddn.Create("s { a = 1 } b = 2");

		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
	}


	/// <summary>Statements without equals sign are NULL values.</summary>
	[Test]
	public void Parse_StatementWithoutEquals()
	{
		DdnElement ddn = Ddn.Create("flag; a = 1;");

		Assert.That(ddn.Contains("flag"), Is.True);
		Assert.That(ddn["flag"].IsNull, Is.True);
		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
	}


	/// <summary>The obsolete equals sign in section declarations is accepted.</summary>
	[Test]
	public void Parse_ObsoleteSectionEquals()
	{
		DdnElement ddn = Ddn.Create("s = { a = 1; }");

		Assert.That(ddn["s"].IsSection, Is.True);
		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
	}


	/// <summary>Unbalanced brackets do not throw.</summary>
	[Test]
	public void Parse_UnbalancedBrackets()
	{
		DdnElement ddn = Ddn.Create("} a = 1; s { b = 2;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/b"), Is.EqualTo(2));
	}


	/// <summary>An unterminated multi-line comment ends the text.</summary>
	[Test]
	public void Parse_UnterminatedComment()
	{
		DdnElement ddn = Ddn.Create("a = 1; /* b = 2;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
		Assert.That(ddn.Contains("b"), Is.False);
	}


	/// <summary>Empty sections are sections.</summary>
	[Test]
	public void Parse_EmptySection()
	{
		DdnElement ddn = Ddn.Create("s { }");

		Assert.That(ddn["s"].IsSection, Is.True);
		Assert.That(ddn["s"].HasElements, Is.False);
		Assert.That(ddn["s"].IsNull, Is.False);
	}


	/// <summary>Duplicate values replace earlier values and are reported.</summary>
	[Test]
	public void Parse_DuplicateValues()
	{
		DdnElement ddn = Ddn.Create("a = 1; b = 2; a = 3;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(3));
		Assert.That(ddn.Elements.Select(m => m.Name), Is.EqualTo(new[] { "a", "b" }));
		Assert.That(ddn.ParsedDuplicates.Select(m => m.Path), Is.EqualTo(new[] { "/a" }));
	}


	/// <summary>Duplicate sections are merged and reported.</summary>
	[Test]
	public void Parse_DuplicateSections()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; t { x = 1; } } s { b = 2; t { y = 2; } }");

		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/b"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("s/t/x"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/t/y"), Is.EqualTo(2));
		Assert.That(ddn.ParsedDuplicates.Select(m => m.Path), Is.EquivalentTo(new[] { "/s", "/s/t" }));
	}


	/// <summary>Sections without name add their elements to the enclosing section.</summary>
	[Test]
	public void Parse_AnonymousSection()
	{
		DdnElement ddn = Ddn.Create("{ a = 1; } b = 2;");

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
	}


	/// <summary>Whitespace around names and values is ignored, whitespace inside is kept.</summary>
	[Test]
	public void Parse_Whitespace()
	{
		DdnElement ddn = Ddn.Create("\t my name \t=\r\n  some  value \t;");

		Assert.That(ddn.Get<string>("my name"), Is.EqualTo("some  value"));
	}


	/// <summary>Multi-line values keep their line breaks.</summary>
	[Test]
	public void Parse_ValueAcrossLines()
	{
		DdnElement ddn = Ddn.Create("vals = 1,\r\n       2,\r\n       3;");

		Assert.That(ddn.Get<int[]>("vals"), Is.EqualTo(new[] { 1, 2, 3 }));
	}


	/// <summary>Setting the text replaces the contents.</summary>
	[Test]
	public void Text_ReplacesContents()
	{
		DdnElement ddn = Ddn.Create("a = 1;");
		ddn.Text = "b = 2;";

		Assert.That(ddn.Contains("a"), Is.False);
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
	}
}
