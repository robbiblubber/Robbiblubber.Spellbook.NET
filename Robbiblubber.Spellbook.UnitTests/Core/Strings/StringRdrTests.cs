namespace Robbiblubber.Spellbook.UnitTests.Core.Strings;

using NUnit.Framework;



/// <summary>This class tests the string reader.</summary>
[TestFixture]
public class StringRdrTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: reading                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Characters are read one by one; the reader stays on the last character.</summary>
	[Test]
	public void Read_Characters()
	{
		StringRdr re = new("ab");

		Assert.That(re.Position, Is.EqualTo(-1));
		Assert.That(re.Current, Is.EqualTo('\0'));
		Assert.That(re.Next, Is.EqualTo('a'));
		Assert.That(re.More, Is.True);

		Assert.That(re.Read(), Is.True);
		Assert.That(re.Current, Is.EqualTo('a'));
		Assert.That(re.Preceding, Is.EqualTo('\0'));
		Assert.That(re.More, Is.True);

		Assert.That(re.Read(), Is.True);
		Assert.That(re.Current, Is.EqualTo('b'));
		Assert.That(re.Preceding, Is.EqualTo('a'));
		Assert.That(re.Next, Is.EqualTo('\0'));
		Assert.That(re.More, Is.False);

		Assert.That(re.Read(), Is.False);
		Assert.That(re.Position, Is.EqualTo(1));
		Assert.That(re.Current, Is.EqualTo('b'));
	}


	/// <summary>Empty and null strings are handled.</summary>
	[Test]
	public void Read_Empty()
	{
		StringRdr re = new(null);

		Assert.That(re.String, Is.Empty);
		Assert.That(re.More, Is.False);
		Assert.That(re.Read(), Is.False);
		Assert.That(re.Current, Is.EqualTo('\0'));
		Assert.That(re.ReadTo(","), Is.Empty);
		Assert.That(re.ReadWhile(char.IsLetter), Is.Empty);
		Assert.That(re.Read(3), Is.Empty);
		Assert.That(re.Position, Is.EqualTo(-1));
	}


	/// <summary>Several characters are read at once.</summary>
	[Test]
	public void Read_Count()
	{
		StringRdr re = new("abcde");

		Assert.That(re.Read(2), Is.EqualTo("ab"));
		Assert.That(re.Current, Is.EqualTo('b'));
		Assert.That(re.Read(10), Is.EqualTo("cde"));
		Assert.That(re.Read(1), Is.Empty);
		Assert.Throws<ArgumentOutOfRangeException>(() => re.Read(-1));
	}


	/// <summary>Peeking does not change the position.</summary>
	[Test]
	public void Peek()
	{
		StringRdr re = new("abc");
		re.Read();

		Assert.That(re.Peek(), Is.EqualTo('b'));
		Assert.That(re.Peek(2), Is.EqualTo('c'));
		Assert.That(re.Peek(3), Is.EqualTo('\0'));
		Assert.That(re.Peek(-1), Is.EqualTo('\0'));
		Assert.That(re.Peek(0), Is.EqualTo('a'));
		Assert.That(re.Peek(int.MaxValue), Is.EqualTo('\0'));
		Assert.That(re.Peek(int.MinValue), Is.EqualTo('\0'));
		Assert.That(re.Position, Is.EqualTo(0));
	}


	/// <summary>Lead and tail split the string at the current position.</summary>
	[Test]
	public void LeadAndTail()
	{
		StringRdr re = new("abc");

		Assert.That(re.Lead, Is.Empty);
		Assert.That(re.Tail, Is.EqualTo("abc"));

		re.Read();
		Assert.That(re.Lead, Is.EqualTo("a"));
		Assert.That(re.Tail, Is.EqualTo("bc"));
	}


	/// <summary>Positions outside the string throw instead of breaking later calls.</summary>
	[Test]
	public void Position_Validated()
	{
		StringRdr re = new("ab");

		re.Position = 1;
		Assert.That(re.Current, Is.EqualTo('b'));
		re.Position = -1;
		Assert.That(re.Current, Is.EqualTo('\0'));

		Assert.Throws<ArgumentOutOfRangeException>(() => re.Position = 2);
		Assert.Throws<ArgumentOutOfRangeException>(() => re.Position = -2);
		Assert.Throws<ArgumentOutOfRangeException>(() => new StringRdr().Position = 0);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: parsing helpers                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Characters are read while a condition holds.</summary>
	[Test]
	public void ReadWhile()
	{
		StringRdr re = new("abc123 x");

		Assert.That(re.ReadWhile(char.IsLetter), Is.EqualTo("abc"));
		Assert.That(re.Current, Is.EqualTo('c'));
		Assert.That(re.ReadWhile(char.IsLetter), Is.Empty);
		Assert.That(re.ReadWhile(char.IsDigit), Is.EqualTo("123"));
		Assert.That(re.SkipWhiteSpace(), Is.EqualTo(1));
		Assert.That(re.SkipWhiteSpace(), Is.EqualTo(0));
		Assert.That(re.ReadWhile(char.IsLetter), Is.EqualTo("x"));
		Assert.That(re.More, Is.False);
	}


	/// <summary>Text is matched and read.</summary>
	[Test]
	public void IsNextAndTryRead()
	{
		StringRdr re = new("SELECT * FROM t");

		Assert.That(re.IsNext("select"), Is.False);
		Assert.That(re.IsNext("select", StringComparison.OrdinalIgnoreCase), Is.True);
		Assert.That(re.TryRead("FROM"), Is.False);
		Assert.That(re.Position, Is.EqualTo(-1));
		Assert.That(re.TryRead("SELECT"), Is.True);
		Assert.That(re.Current, Is.EqualTo('T'));
		Assert.That(re.IsNext(" * FROM t and more"), Is.False);
		Assert.That(re.IsNext(string.Empty), Is.True);
	}


	/// <summary>Text is read up to a delimiter, including the last part.</summary>
	[Test]
	public void ReadTo_Char()
	{
		StringRdr re = new("a,bc,,d");

		Assert.That(re.ReadTo(','), Is.EqualTo("a"));
		Assert.That(re.Delimiter, Is.EqualTo(","));
		Assert.That(re.ReadTo(','), Is.EqualTo("bc"));
		Assert.That(re.ReadTo(','), Is.Empty);
		Assert.That(re.More, Is.True);
		Assert.That(re.ReadTo(','), Is.EqualTo("d"));
		Assert.That(re.Delimiter, Is.Null);
		Assert.That(re.More, Is.False);
	}


	/// <summary>The last character is not lost (the old StringParser reported no more data here).</summary>
	[Test]
	public void ReadTo_OneCharacterLeft()
	{
		StringRdr re = new("a b");

		Assert.That(re.ReadTo(" "), Is.EqualTo("a"));
		Assert.That(re.More, Is.True);
		Assert.That(re.ReadTo(" "), Is.EqualTo("b"));
		Assert.That(re.More, Is.False);
	}


	/// <summary>The longest of several delimiters at the same position is used; comparison is ordinal.</summary>
	[Test]
	public void ReadTo_LongestDelimiter()
	{
		StringRdr re = new("line1\r\nline2\nline3");

		Assert.That(re.ReadTo("\r", "\n", "\r\n"), Is.EqualTo("line1"));
		Assert.That(re.Delimiter, Is.EqualTo("\r\n"));
		Assert.That(re.ReadTo("\r", "\n", "\r\n"), Is.EqualTo("line2"));
		Assert.That(re.Delimiter, Is.EqualTo("\n"));
		Assert.That(re.ReadTo("\r", "\n", "\r\n"), Is.EqualTo("line3"));
		Assert.That(re.Delimiter, Is.Null);

		StringRdr crlf = new("a\r\nb");
		Assert.That(crlf.ReadTo("\n"), Is.EqualTo("a\r"));
	}


	/// <summary>Empty delimiters throw instead of looping forever.</summary>
	[Test]
	public void ReadTo_EmptyDelimiter_Throws()
	{
		StringRdr re = new("abc");

		Assert.Throws<ArgumentException>(() => re.ReadTo(string.Empty));
		Assert.Throws<ArgumentException>(() => re.ReadTo(Array.Empty<string>()));
	}


	/// <summary>A small tokenizer can be written with the helpers.</summary>
	[Test]
	public void Tokenizer()
	{
		StringRdr re = new("  sum(12, x) ");
		List<string> tokens = new();

		for(re.SkipWhiteSpace(); re.More; re.SkipWhiteSpace())
		{
			if(char.IsLetter(re.Next)) { tokens.Add(re.ReadWhile(char.IsLetterOrDigit)); }
			else if(char.IsDigit(re.Next)) { tokens.Add(re.ReadWhile(char.IsDigit)); }
			else { tokens.Add(re.Read(1)); }
		}

		Assert.That(tokens, Is.EqualTo(new[] { "sum", "(", "12", ",", "x", ")" }));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: misc                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reset moves the reader before the first character.</summary>
	[Test]
	public void Reset()
	{
		StringRdr re = new("a,b");
		re.ReadTo(',');
		re.Reset();

		Assert.That(re.Position, Is.EqualTo(-1));
		Assert.That(re.Delimiter, Is.Null);
	}


	/// <summary>Strings and readers are converted.</summary>
	[Test]
	public void Conversions()
	{
		StringRdr re = (StringRdr) "abc";

		Assert.That((string) re, Is.EqualTo("abc"));
		Assert.That(re.ToString(), Is.EqualTo("abc"));
		Assert.That(re.Length, Is.EqualTo(3));
	}
}
