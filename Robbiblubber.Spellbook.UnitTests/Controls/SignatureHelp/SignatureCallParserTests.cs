namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using NUnit.Framework;
using Robbiblubber.Spellbook.Controls;

using SyntaxState = Spellbook.Controls.SignatureCallParser.SyntaxState;



/// <summary>This class tests the <see cref="SignatureCallParser"/> class.</summary>
[TestFixture]
public class SignatureCallParserTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses a text with the caret at the end.</summary>
	/// <param name="text">Text.</param>
	/// <param name="parser">Parser, NULL for a default parser.</param>
	/// <returns>Returns the call.</returns>
	private static SignatureCall? _Parse(string text, SignatureCallParser? parser = null)
	{
		return (parser ?? new SignatureCallParser()).Parse(text, text.Length);
	}


	/// <summary>Creates a parser with SQL comments.</summary>
	/// <returns>Returns the parser.</returns>
	private static SignatureCallParser _SqlParser()
	{
		return new SignatureCallParser { LineComment = "--", BlockCommentStart = "/*", BlockCommentEnd = "*/" };
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>An opening parenthesis after a name is a call; all positions are reported.</summary>
	[Test]
	public void FindsCall()
	{
		SignatureCall? call = _Parse("x = Max(");

		Assert.That(call, Is.Not.Null);
		Assert.That(call!.Name, Is.EqualTo("Max"));
		Assert.That(call.Qualifier, Is.Empty);
		Assert.That(call.NameStart, Is.EqualTo(4));
		Assert.That(call.OpenIndex, Is.EqualTo(7));
		Assert.That(call.ArgumentIndex, Is.EqualTo(0));
		Assert.That(call.ArgumentStart, Is.EqualTo(8));
	}


	/// <summary>Separators count the arguments.</summary>
	[Test]
	public void CountsArguments()
	{
		SignatureCall? call = _Parse("Max(1, 2, ");

		Assert.That(call!.ArgumentIndex, Is.EqualTo(2));
		Assert.That(call.ArgumentStart, Is.EqualTo(9));
	}


	/// <summary>Only the text before the caret counts.</summary>
	[Test]
	public void UsesCaretPosition()
	{
		SignatureCallParser parser = new();

		Assert.That(parser.Parse("Max(1, 2)", 5)!.ArgumentIndex, Is.EqualTo(0));
		Assert.That(parser.Parse("Max(1, 2)", 7)!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(parser.Parse("Max(1, 2)", 3), Is.Null);
		Assert.That(parser.Parse("Max(1, 2)", 9), Is.Null);
		Assert.That(parser.Parse("Max(1", 100)!.Name, Is.EqualTo("Max"));
		Assert.That(parser.Parse("Max(1", -5), Is.Null);
	}


	/// <summary>Texts without an open call yield no call.</summary>
	/// <param name="text">Text.</param>
	[TestCase("")]
	[TestCase("Max")]
	[TestCase("Max(1)")]
	[TestCase("(")]
	[TestCase("1 + (2, ")]
	[TestCase("3(")]
	public void FindsNoCall(string text)
	{
		Assert.That(_Parse(text), Is.Null);
	}


	/// <summary>The innermost call is found; after it is closed, the enclosing call is found again.</summary>
	[Test]
	public void FindsInnermostCall()
	{
		SignatureCall? inner = _Parse("Round(Log(x, ");
		SignatureCall? outer = _Parse("Round(Log(x), ");

		Assert.That(inner!.Name, Is.EqualTo("Log"));
		Assert.That(inner.ArgumentIndex, Is.EqualTo(1));
		Assert.That(outer!.Name, Is.EqualTo("Round"));
		Assert.That(outer.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>Parentheses without a name are skipped, so the enclosing call is found.</summary>
	[Test]
	public void SkipsGroupingParentheses()
	{
		Assert.That(_Parse("Max((1 + 2), ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("Max((1, ")!.Name, Is.EqualTo("Max"));
		Assert.That(_Parse("Max((1, ")!.ArgumentIndex, Is.EqualTo(0));
	}


	/// <summary>Separators within nested brackets do not count.</summary>
	[Test]
	public void IgnoresSeparatorsInNestedBrackets()
	{
		Assert.That(_Parse("fn(a[1, 2], ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn({1, 2}, b")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(a[1, ")!.ArgumentIndex, Is.EqualTo(0));
		Assert.That(_Parse("fn(a[1, ", new SignatureCallParser { NestingBrackets = string.Empty })!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>A closing bracket closes all brackets opened after its opening bracket; unmatched closing brackets are ignored.</summary>
	[Test]
	public void HandlesMismatchedBrackets()
	{
		Assert.That(_Parse("fn(a], ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("g(fn(a[b), ")!.Name, Is.EqualTo("g"));
		Assert.That(_Parse("g(fn(a[b), ")!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>Separators and brackets in strings are ignored; the call is found while the caret is in a string.</summary>
	[Test]
	public void IgnoresStrings()
	{
		Assert.That(_Parse("fn('a, (b', ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(\"a, )\", ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn('it''s, x', ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(a, 'b, ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(a, \"x\", ", new SignatureCallParser { StringDelimiters = "'" })!.ArgumentIndex, Is.EqualTo(2));
	}


	/// <summary>The escape character keeps a string open.</summary>
	[Test]
	public void RespectsEscapeCharacter()
	{
		Assert.That(_Parse("fn(\"a\\\", ", new SignatureCallParser { EscapeCharacter = '\\' })!.ArgumentIndex, Is.EqualTo(0));
		Assert.That(_Parse("fn(\"a\\\", ")!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>Configured comments are ignored; within a comment, no call is found.</summary>
	[Test]
	public void IgnoresComments()
	{
		Assert.That(_Parse("fn(a, -- x, (y\n b, ", _SqlParser())!.ArgumentIndex, Is.EqualTo(2));
		Assert.That(_Parse("fn(a /* , ( */, ", _SqlParser())!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(a -- (", _SqlParser()), Is.Null);
		Assert.That(_Parse("fn(a /* x, ", _SqlParser()), Is.Null);
		Assert.That(_Parse("-- fn(", _SqlParser()), Is.Null);
		Assert.That(_Parse("'-- not a comment', fn(", _SqlParser())!.Name, Is.EqualTo("fn"));
	}


	/// <summary>Comments are not recognized unless configured.</summary>
	[Test]
	public void CommentsRequireConfiguration()
	{
		Assert.That(_Parse("fn(a -- x, ")!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Parse("fn(a /* x, ", new SignatureCallParser { BlockCommentStart = "/*" })!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>Qualified names report the qualifier; the name start includes the qualifier.</summary>
	[Test]
	public void FindsQualifier()
	{
		SignatureCall? call = _Parse("SELECT dbo.Sales.fn(");

		Assert.That(call!.Name, Is.EqualTo("fn"));
		Assert.That(call.Qualifier, Is.EqualTo("dbo.Sales"));
		Assert.That(call.NameStart, Is.EqualTo(7));
		Assert.That(_Parse("x.fn(")!.Qualifier, Is.EqualTo("x"));
		Assert.That(_Parse(".fn(")!.Qualifier, Is.Empty);
	}


	/// <summary>Whitespace between name and parenthesis is allowed unless disabled.</summary>
	[Test]
	public void AllowsWhitespaceBeforeParenthesis()
	{
		Assert.That(_Parse("COUNT \t(*")!.Name, Is.EqualTo("COUNT"));
		Assert.That(_Parse("COUNT (*", new SignatureCallParser { AllowWhitespaceBeforeParenthesis = false }), Is.Null);
		Assert.That(_Parse("COUNT\n(*"), Is.Null);
	}


	/// <summary>Name characters are configurable.</summary>
	[Test]
	public void RespectsNameCharacters()
	{
		Assert.That(_Parse("my_fn(")!.Name, Is.EqualTo("my_fn"));
		Assert.That(_Parse("@fn(")!.Name, Is.EqualTo("fn"));
		Assert.That(_Parse("@fn(", new SignatureCallParser { NameCharacters = "_@" })!.Name, Is.EqualTo("@fn"));
		Assert.That(_Parse("Größe(")!.Name, Is.EqualTo("Größe"));
	}


	/// <summary>The argument separator is configurable.</summary>
	[Test]
	public void RespectsArgumentSeparator()
	{
		Assert.That(_Parse("fn(a; b; ", new SignatureCallParser { ArgumentSeparator = ';' })!.ArgumentIndex, Is.EqualTo(2));
		Assert.That(_Parse("fn(a; b, ", new SignatureCallParser { ArgumentSeparator = ';' })!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>Calls spanning several lines are found.</summary>
	[Test]
	public void FindsMultilineCall()
	{
		SignatureCall? call = _Parse("fn(a,\r\n   b,\r\n   ");

		Assert.That(call!.ArgumentIndex, Is.EqualTo(2));
		Assert.That(call.ArgumentStart, Is.EqualTo(12));
	}


	/// <summary>The syntax state tells code, strings and comments apart with the configured rules.</summary>
	/// <param name="text">Text; the caret is at the end.</param>
	/// <param name="expected">Expected state.</param>
	[TestCase("", SyntaxState.CODE)]
	[TestCase("SELECT Na", SyntaxState.CODE)]
	[TestCase("SELECT 'abc", SyntaxState.STRING)]
	[TestCase("SELECT 'abc' + x", SyntaxState.CODE)]
	[TestCase("SELECT 'it''s", SyntaxState.STRING)]
	[TestCase("SELECT \"Order", SyntaxState.STRING)]
	[TestCase("SELECT x -- comment", SyntaxState.COMMENT)]
	[TestCase("SELECT x -- comment\nFROM", SyntaxState.CODE)]
	[TestCase("SELECT /* comment", SyntaxState.COMMENT)]
	[TestCase("SELECT /* comment */ x", SyntaxState.CODE)]
	[TestCase("SELECT '-- no comment", SyntaxState.STRING)]
	[TestCase("SELECT '/*' x", SyntaxState.CODE)]
	public void GetsSyntaxState(string text, SyntaxState expected)
	{
		Assert.That(_SqlParser().GetSyntaxState(text, text.Length), Is.EqualTo(expected));
	}


	/// <summary>The syntax state respects the caret position, the escape character and unconfigured comments.</summary>
	[Test]
	public void SyntaxStateRespectsSettings()
	{
		SignatureCallParser parser = new();

		Assert.That(parser.GetSyntaxState("x -- y", 6), Is.EqualTo(SyntaxState.CODE));
		Assert.That(_SqlParser().GetSyntaxState("'abc' x", 3), Is.EqualTo(SyntaxState.STRING));
		Assert.That(_SqlParser().GetSyntaxState("'abc' x", 5), Is.EqualTo(SyntaxState.CODE));
		Assert.That(_SqlParser().GetSyntaxState("x -- y", 100), Is.EqualTo(SyntaxState.COMMENT));
		Assert.That(new SignatureCallParser { EscapeCharacter = '\\' }.GetSyntaxState("\"a\\\" b", 6), Is.EqualTo(SyntaxState.STRING));
		Assert.That(parser.GetSyntaxState("\"a\\\" b", 6), Is.EqualTo(SyntaxState.CODE));
		Assert.That(() => parser.GetSyntaxState(null!, 0), Throws.TypeOf<ArgumentNullException>());
	}


	/// <summary>NULL text is rejected.</summary>
	[Test]
	public void RejectsNullText()
	{
		Assert.That(() => new SignatureCallParser().Parse(null!, 0), Throws.TypeOf<ArgumentNullException>());
	}
}
