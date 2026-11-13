namespace Robbiblubber.Spellbook;



/// <summary>This class reads a string character by character, for example to write a small parser.</summary>
/// <remarks><para><see cref="Position"/> is the index of the current character; it is -1 before the first character has been
///          read. Read methods consume characters up to and including the new position. At the end of the string the reader
///          stays on the last character.</para>
///          <para>Characters are UTF-16 code units; a character outside the Basic Multilingual Plane (e.g. an emoji) is read
///          as two characters.</para></remarks>
public sealed class StringRdr
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Position.</summary>
	private int _Position = -1;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="s">String; NULL for an empty string.</param>
	public StringRdr(string? s = null)
	{
		String = (s ?? string.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the underlying string.</summary>
	public string String
	{
		get;
	}


	/// <summary>Gets the string length.</summary>
	public int Length
	{
		get { return String.Length; }
	}


	/// <summary>Gets or sets the position of the current character; -1 before the first character.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the position is less than -1 or not less than the length.</exception>
	public int Position
	{
		get { return _Position; }
		set
		{
			if((value < -1) || (value >= Length)) { throw new ArgumentOutOfRangeException(nameof(value), "The position must be between -1 and " + (Length - 1) + "."); }
			_Position = value;
		}
	}


	/// <summary>Gets if more characters can be read.</summary>
	public bool More
	{
		get { return (Position < (Length - 1)); }
	}


	/// <summary>Gets the current character; '\0' before the first character.</summary>
	public char Current
	{
		get { return Peek(0); }
	}


	/// <summary>Gets the character before the current character; '\0' if there is none.</summary>
	public char Preceding
	{
		get { return Peek(-1); }
	}


	/// <summary>Gets the next character; '\0' at the end of the string.</summary>
	public char Next
	{
		get { return Peek(1); }
	}


	/// <summary>Gets the part of the string from the beginning up to and including the current character.</summary>
	public string Lead
	{
		get { return String[..(Position + 1)]; }
	}


	/// <summary>Gets the part of the string after the current character.</summary>
	public string Tail
	{
		get { return String[(Position + 1)..]; }
	}


	/// <summary>Gets the delimiter found by the last call of <see cref="ReadTo(string[])"/> or <see cref="ReadTo(char)"/>;
	///          NULL if the end of the string has been reached.</summary>
	public string? Delimiter
	{
		get; private set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a character relative to the current position without reading it.</summary>
	/// <param name="offset">Offset; 1 for the next character, -1 for the preceding character.</param>
	/// <returns>Returns the character, or '\0' if the position is outside the string.</returns>
	public char Peek(int offset = 1)
	{
		long i = ((long) Position + offset);
		return (((i < 0) || (i >= Length)) ? '\0' : String[(int) i]);
	}


	/// <summary>Returns if the string continues with the given text after the current character.</summary>
	/// <param name="s">Text.</param>
	/// <param name="comparison">Comparison.</param>
	/// <returns>Returns TRUE if the next characters match the text, otherwise returns FALSE.</returns>
	public bool IsNext(string s, StringComparison comparison = StringComparison.Ordinal)
	{
		return String.AsSpan(Position + 1).StartsWith(s, comparison);
	}


	/// <summary>Reads the next character.</summary>
	/// <returns>Returns TRUE if a character has been read, FALSE at the end of the string.</returns>
	public bool Read()
	{
		if(!More) { return false; }

		Position++;
		return true;
	}


	/// <summary>Reads a number of characters.</summary>
	/// <param name="count">Number of characters.</param>
	/// <returns>Returns the characters read; fewer at the end of the string.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the count is negative.</exception>
	public string Read(int count)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count);

		int start = (Position + 1);
		int n = Math.Min(count, Length - start);
		Position += n;

		return String.Substring(start, n);
	}


	/// <summary>Reads the given text if the string continues with it.</summary>
	/// <param name="s">Text.</param>
	/// <param name="comparison">Comparison.</param>
	/// <returns>Returns TRUE if the text has been read, otherwise returns FALSE and the position is unchanged.</returns>
	public bool TryRead(string s, StringComparison comparison = StringComparison.Ordinal)
	{
		if(!IsNext(s, comparison)) { return false; }

		Position += s.Length;
		return true;
	}


	/// <summary>Reads characters as long as they match a condition.</summary>
	/// <param name="predicate">Condition.</param>
	/// <returns>Returns the characters read.</returns>
	public string ReadWhile(Func<char, bool> predicate)
	{
		int start = (Position + 1);
		while(More && predicate(String[Position + 1])) { Position++; }

		return String[start..(Position + 1)];
	}


	/// <summary>Skips white space.</summary>
	/// <returns>Returns the number of characters skipped.</returns>
	public int SkipWhiteSpace()
	{
		return ReadWhile(char.IsWhiteSpace).Length;
	}


	/// <summary>Reads up to the next occurrence of a delimiter and reads the delimiter as well.</summary>
	/// <param name="delimiter">Delimiter.</param>
	/// <returns>Returns the characters before the delimiter, or the rest of the string if the delimiter does not occur.</returns>
	/// <remarks>The delimiter is stored in <see cref="Delimiter"/>; NULL if the end has been reached.</remarks>
	public string ReadTo(char delimiter)
	{
		int start = (Position + 1);
		int i = String.IndexOf(delimiter, start);

		if(i < 0)
		{
			Delimiter = null;
			i = Length;
			Position = (Length - 1);
		}
		else
		{
			Delimiter = delimiter.ToString();
			Position = i;
		}

		return String[start..i];
	}


	/// <summary>Reads up to the next occurrence of any of the delimiters and reads the delimiter as well.</summary>
	/// <param name="delimiters">Delimiters. If several delimiters occur at the same position, the longest is used.</param>
	/// <returns>Returns the characters before the delimiter, or the rest of the string if no delimiter occurs.</returns>
	/// <remarks>The delimiter is stored in <see cref="Delimiter"/>; NULL if the end has been reached. Delimiters are compared
	///          ordinally.</remarks>
	/// <exception cref="ArgumentException">Thrown when no delimiter is given or a delimiter is empty.</exception>
	public string ReadTo(params string[] delimiters)
	{
		if(delimiters.Length == 0) { throw new ArgumentException("At least one delimiter is required.", nameof(delimiters)); }
		if(delimiters.Any(string.IsNullOrEmpty)) { throw new ArgumentException("Delimiters must not be empty.", nameof(delimiters)); }

		int start = (Position + 1);
		int pos = Length;
		string? del = null;

		foreach(string i in delimiters)
		{
			int n = String.IndexOf(i, start, StringComparison.Ordinal);
			if((n < 0) || (n > pos)) { continue; }

			if((n < pos) || (i.Length > del!.Length))
			{
				pos = n;
				del = i;
			}
		}

		Delimiter = del;
		Position = ((del is null) ? (Length - 1) : (pos + del.Length - 1));

		return String[start..pos];
	}


	/// <summary>Moves the reader before the first character.</summary>
	public void Reset()
	{
		Position = -1;
		Delimiter = null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns the underlying string.</summary>
	/// <returns>Returns the string.</returns>
	public override string ToString()
	{
		return String;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// operators                                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a string reader for a string.</summary>
	/// <param name="s">String.</param>
	public static explicit operator StringRdr(string s)
	{
		return new StringRdr(s);
	}


	/// <summary>Gets the underlying string of a string reader.</summary>
	/// <param name="re">String reader.</param>
	public static explicit operator string(StringRdr re)
	{
		return re.String;
	}
}
