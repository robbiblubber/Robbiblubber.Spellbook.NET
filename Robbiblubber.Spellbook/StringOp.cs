namespace Robbiblubber.Spellbook;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;



/// <summary>This class provides string operation methods.</summary>
/// <remarks>Numbers and Boolean values are parsed with the invariant culture unless a format provider is given. Text is
///          compared ordinally.</remarks>
public static class StringOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The default password alphabet.</summary>
	public const string PASSWORD_ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!$%&#*+.:_";

	/// <summary>Digits, lower case and upper case letters; the default alphabet for random strings, number conversion and
	///          <see cref="Complies(string, string)"/>.</summary>
	public const string ALPHANUMERIC = InvertedBase62.ALPHABET;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Strings that are converted to TRUE.</summary>
	private static readonly string[] _TRUE = ["true", "t", "yes", "y", "on"];



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a random string of 24 characters from an alphabet.</summary>
	/// <param name="alphabet">Alphabet.</param>
	/// <returns>Returns the random string.</returns>
	/// <exception cref="ArgumentException">Thrown when the alphabet is empty.</exception>
	/// <remarks>The characters are chosen with a cryptographically secure random number generator.</remarks>
	public static string Random(string alphabet)
	{
		return Random(24, alphabet);
	}


	/// <summary>Returns a random string of a given length from an alphabet.</summary>
	/// <param name="length">Length.</param>
	/// <param name="alphabet">Alphabet.</param>
	/// <returns>Returns the random string.</returns>
	/// <exception cref="ArgumentException">Thrown when the alphabet is empty.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the length is negative.</exception>
	/// <remarks>The characters are chosen with a cryptographically secure random number generator; every character of the
	///          alphabet has the same probability.</remarks>
	public static string Random(int length = 24, string alphabet = ALPHANUMERIC)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length);
		if(string.IsNullOrEmpty(alphabet)) { throw new ArgumentException("The alphabet must not be empty.", nameof(alphabet)); }

		return RandomNumberGenerator.GetString(alphabet, length);
	}


	/// <summary>Returns a unique random alphanumeric string.</summary>
	/// <param name="length">Length.</param>
	/// <returns>Returns the string.</returns>
	/// <remarks>The string is random; 24 characters carry more than 140 bits, so collisions do not occur in practice. Use at
	///          least 16 characters for unique values.</remarks>
	public static string Unique(int length = 24)
	{
		return Random(length);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Validates a base.</summary>
	/// <param name="numberBase">Base.</param>
	/// <param name="alphabet">Alphabet.</param>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	private static void _CheckBase(int numberBase, string alphabet)
	{
		if((numberBase < 2) || (numberBase > alphabet.Length)) { throw new ArgumentException("The base must be between 2 and the alphabet length (" + alphabet.Length + ").", nameof(numberBase)); }
	}


	/// <summary>Gets the value of a digit.</summary>
	/// <param name="c">Character.</param>
	/// <param name="numberBase">Base.</param>
	/// <param name="alphabet">Alphabet.</param>
	/// <returns>Returns the value, or -1 if the character is not a digit of the base.</returns>
	private static int _Digit(char c, int numberBase, string alphabet)
	{
		int rval = alphabet.IndexOf(c);
		if(((rval < 0) || (rval >= numberBase)) && (numberBase <= 36))
		{
			rval = alphabet.IndexOf(char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c));
		}

		return (((rval >= 0) && (rval < numberBase)) ? rval : -1);
	}


	/// <summary>Compares two characters.</summary>
	/// <param name="a">Character a.</param>
	/// <param name="b">Character b.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns TRUE if the characters are equal, otherwise returns FALSE.</returns>
	private static bool _Equals(char a, char b, bool caseSensitive)
	{
		return ((a == b) || ((!caseSensitive) && (char.ToUpperInvariant(a) == char.ToUpperInvariant(b))));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: numbers                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts an integer to a string of a given base.</summary>
	/// <param name="value">Value.</param>
	/// <param name="toBase">Base of the result; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet; the character at index i stands for the digit i.</param>
	/// <returns>Returns the string; negative values start with '-'.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	public static string ToString(this long value, int toBase, string alphabet = ALPHANUMERIC)
	{
		_CheckBase(toBase, alphabet);

		ulong v = ((value < 0) ? (ulong) (-(value + 1)) + 1 : (ulong) value);
		Span<char> buf = stackalloc char[65];
		int pos = buf.Length;

		do
		{
			buf[--pos] = alphabet[(int) (v % (ulong) toBase)];
			v /= (ulong) toBase;
		}
		while(v > 0);

		if(value < 0) { buf[--pos] = '-'; }

		return new string(buf[pos..]);
	}


	/// <summary>Converts an integer to a string of a given base.</summary>
	/// <param name="value">Value.</param>
	/// <param name="toBase">Base of the result; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet; the character at index i stands for the digit i.</param>
	/// <returns>Returns the string; negative values start with '-'.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	public static string ToString(this int value, int toBase, string alphabet = ALPHANUMERIC)
	{
		return ToString((long) value, toBase, alphabet);
	}


	/// <summary>Converts an integer to a string of a given base.</summary>
	/// <param name="value">Value.</param>
	/// <param name="toBase">Base of the result; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet; the character at index i stands for the digit i.</param>
	/// <returns>Returns the string; negative values start with '-'.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	public static string ToString(this short value, int toBase, string alphabet = ALPHANUMERIC)
	{
		return ToString((long) value, toBase, alphabet);
	}


	/// <summary>Converts a string of a given base to an integer.</summary>
	/// <param name="value">String; may start with '-' or '+'. Leading and trailing white space is ignored.</param>
	/// <param name="fromBase">Base of the string; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet; the character at index i stands for the digit i. For bases up to 36, letters are
	///                        accepted in either case.</param>
	/// <returns>Returns the integer.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	/// <exception cref="FormatException">Thrown when the string is empty or contains invalid characters.</exception>
	/// <exception cref="OverflowException">Thrown when the value is too large.</exception>
	public static long ToInt64(this string value, int fromBase = 10, string alphabet = ALPHANUMERIC)
	{
		_CheckBase(fromBase, alphabet);

		ReadOnlySpan<char> s = value.AsSpan().Trim();
		bool neg = false;
		if((s.Length > 0) && ((s[0] == '-') || (s[0] == '+')))
		{
			neg = (s[0] == '-');
			s = s[1..];
		}
		if(s.Length == 0) { throw new FormatException("The string does not contain a number."); }

		ulong rval = 0;
		for(int i = 0; i < s.Length; i++)
		{
			int d = _Digit(s[i], fromBase, alphabet);
			if(d < 0) { throw new FormatException("Invalid digit '" + s[i] + "' for base " + fromBase + "."); }

			rval = checked((rval * (ulong) fromBase) + (ulong) d);
		}

		if(neg)
		{
			if(rval > (1UL << 63)) { throw new OverflowException("The value is too small for a long integer."); }
			return (long) (0UL - rval);
		}

		if(rval > long.MaxValue) { throw new OverflowException("The value is too large for a long integer."); }
		return (long) rval;
	}


	/// <summary>Converts a string of a given base to an integer.</summary>
	/// <param name="value">String; may start with '-' or '+'.</param>
	/// <param name="fromBase">Base of the string; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet.</param>
	/// <returns>Returns the integer.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	/// <exception cref="FormatException">Thrown when the string is empty or contains invalid characters.</exception>
	/// <exception cref="OverflowException">Thrown when the value is out of range.</exception>
	public static int ToInt32(this string value, int fromBase = 10, string alphabet = ALPHANUMERIC)
	{
		return checked((int) value.ToInt64(fromBase, alphabet));
	}


	/// <summary>Converts a string of a given base to an integer.</summary>
	/// <param name="value">String; may start with '-' or '+'.</param>
	/// <param name="fromBase">Base of the string; from 2 to the alphabet length.</param>
	/// <param name="alphabet">Alphabet.</param>
	/// <returns>Returns the integer.</returns>
	/// <exception cref="ArgumentException">Thrown when the base is invalid.</exception>
	/// <exception cref="FormatException">Thrown when the string is empty or contains invalid characters.</exception>
	/// <exception cref="OverflowException">Thrown when the value is out of range.</exception>
	public static short ToInt16(this string value, int fromBase = 10, string alphabet = ALPHANUMERIC)
	{
		return checked((short) value.ToInt64(fromBase, alphabet));
	}


	/// <summary>Converts an integer to an alphanumeric string (base 62).</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the string.</returns>
	public static string ToAlpha(this long value)
	{
		return ToString(value, 62);
	}


	/// <summary>Converts an integer to an alphanumeric string (base 62).</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the string.</returns>
	public static string ToAlpha(this int value)
	{
		return ToString((long) value, 62);
	}


	/// <summary>Converts an integer to an alphanumeric string (base 62).</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the string.</returns>
	public static string ToAlpha(this short value)
	{
		return ToString((long) value, 62);
	}


	/// <summary>Converts a string to an integer.</summary>
	/// <param name="s">String.</param>
	/// <param name="defaultValue">Value returned if the string is not an integer.</param>
	/// <param name="provider">Format provider; NULL for the invariant culture.</param>
	/// <returns>Returns the integer, or the default value.</returns>
	public static int ToInteger(this string? s, int defaultValue = 0, IFormatProvider? provider = null)
	{
		return (int.TryParse(s, NumberStyles.Integer, (provider ?? CultureInfo.InvariantCulture), out int rval) ? rval : defaultValue);
	}


	/// <summary>Converts a string to a Boolean value.</summary>
	/// <param name="s">String.</param>
	/// <returns>Returns TRUE for "true", "t", "yes", "y" and "on" (in any case) and for numbers other than 0, otherwise
	///          returns FALSE.</returns>
	public static bool ToBoolean(this string? s)
	{
		if(s is null) { return false; }

		ReadOnlySpan<char> v = s.AsSpan().Trim();
		foreach(string i in _TRUE)
		{
			if(v.Equals(i, StringComparison.OrdinalIgnoreCase)) { return true; }
		}

		return (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double n) && (n != 0));
	}


	/// <summary>Returns if a string is a number (with optional sign, decimal point and exponent, without thousands
	///          separators).</summary>
	/// <param name="s">String.</param>
	/// <param name="provider">Format provider; NULL for the invariant culture.</param>
	/// <returns>Returns TRUE if the string is a number, otherwise returns FALSE.</returns>
	public static bool IsNumeric(this string? s, IFormatProvider? provider = null)
	{
		return decimal.TryParse(s, NumberStyles.Float, (provider ?? CultureInfo.InvariantCulture), out _);
	}


	/// <summary>Returns if a string is an integer.</summary>
	/// <param name="s">String.</param>
	/// <param name="provider">Format provider; NULL for the invariant culture.</param>
	/// <returns>Returns TRUE if the string is an integer, otherwise returns FALSE.</returns>
	public static bool IsNumericInteger(this string? s, IFormatProvider? provider = null)
	{
		return long.TryParse(s, NumberStyles.Integer, (provider ?? CultureInfo.InvariantCulture), out _);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: text                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Makes the first character of a string upper case.</summary>
	/// <param name="s">String.</param>
	/// <param name="culture">Culture; NULL for the invariant culture.</param>
	/// <returns>Returns the string with the first character in upper case; an empty string for NULL.</returns>
	public static string InitCap(this string? s, CultureInfo? culture = null)
	{
		if(string.IsNullOrEmpty(s)) { return string.Empty; }

		return (char.ToUpper(s[0], (culture ?? CultureInfo.InvariantCulture)) + s[1..]);
	}


	/// <summary>Gets the number of identical characters at the start of two strings.</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="pattern">String to compare with.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the number of matching characters.</returns>
	public static int MatchesFromStart(this string expression, string pattern, bool caseSensitive = true)
	{
		int n = Math.Min(expression.Length, pattern.Length);
		int rval = 0;

		while((rval < n) && (caseSensitive ? (expression[rval] == pattern[rval]) : (char.ToUpperInvariant(expression[rval]) == char.ToUpperInvariant(pattern[rval])))) { rval++; }

		return rval;
	}


	/// <summary>Returns if an expression matches a wildcard pattern.</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="caseSensitive">Determines if the match is case-sensitive.</param>
	/// <param name="pattern">Pattern; '*' matches any number of characters, '?' matches one character.</param>
	/// <returns>Returns TRUE if the expression matches, otherwise returns FALSE.</returns>
	public static bool Matches(this string expression, bool caseSensitive, string? pattern)
	{
		if(pattern is null) { return false; }

		int e = 0, p = 0, star = -1, mark = 0;
		while(e < expression.Length)
		{
			if((p < pattern.Length) && (pattern[p] == '*'))
			{
				star = p++;
				mark = e;
			}
			else if((p < pattern.Length) && ((pattern[p] == '?') || _Equals(pattern[p], expression[e], caseSensitive)))
			{
				p++;
				e++;
			}
			else if(star >= 0)
			{
				p = (star + 1);
				e = ++mark;
			}
			else { return false; }
		}

		while((p < pattern.Length) && (pattern[p] == '*')) { p++; }
		return (p == pattern.Length);
	}


	/// <summary>Returns if an expression matches any of a number of wildcard patterns.</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="caseSensitive">Determines if the match is case-sensitive.</param>
	/// <param name="pattern">Patterns; '*' matches any number of characters, '?' matches one character.</param>
	/// <returns>Returns TRUE if the expression matches any pattern, otherwise returns FALSE.</returns>
	public static bool Matches(this string expression, bool caseSensitive, IEnumerable<string>? pattern)
	{
		return ((pattern is not null) && pattern.Any(m => expression.Matches(caseSensitive, m)));
	}


	/// <summary>Returns if an expression matches any of a number of wildcard patterns (case-sensitive).</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="pattern">Patterns; '*' matches any number of characters, '?' matches one character.</param>
	/// <returns>Returns TRUE if the expression matches any pattern, otherwise returns FALSE.</returns>
	public static bool Matches(this string expression, IEnumerable<string>? pattern)
	{
		return Matches(expression, true, pattern);
	}


	/// <summary>Returns if an expression matches any of a number of wildcard patterns.</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="caseSensitive">Determines if the match is case-sensitive.</param>
	/// <param name="pattern">Patterns; '*' matches any number of characters, '?' matches one character.</param>
	/// <returns>Returns TRUE if the expression matches any pattern, otherwise returns FALSE.</returns>
	public static bool Matches(this string expression, bool caseSensitive, params string[] pattern)
	{
		return Matches(expression, caseSensitive, (IEnumerable<string>) pattern);
	}


	/// <summary>Returns if an expression matches any of a number of wildcard patterns (case-sensitive).</summary>
	/// <param name="expression">Expression.</param>
	/// <param name="pattern">Patterns; '*' matches any number of characters, '?' matches one character.</param>
	/// <returns>Returns TRUE if the expression matches any pattern, otherwise returns FALSE.</returns>
	public static bool Matches(this string expression, params string[] pattern)
	{
		return Matches(expression, true, (IEnumerable<string>) pattern);
	}


	/// <summary>Gets the length of the longest string.</summary>
	/// <param name="strings">Strings; NULL entries count as empty.</param>
	/// <returns>Returns the maximum length; 0 if there are no strings.</returns>
	public static int MaxLength(this IEnumerable<string?> strings)
	{
		int rval = 0;
		foreach(string? i in strings) { rval = Math.Max(rval, (i?.Length ?? 0)); }

		return rval;
	}


	/// <summary>Returns if a string consists of allowed characters only.</summary>
	/// <param name="s">String.</param>
	/// <param name="allowed">Allowed characters.</param>
	/// <returns>Returns TRUE if all characters are allowed, otherwise returns FALSE.</returns>
	public static bool Complies(this string s, string allowed = ALPHANUMERIC)
	{
		return (s.AsSpan().IndexOfAnyExcept(allowed) < 0);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: parts                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the part of a string before the first occurrence of a character.</summary>
	/// <param name="s">String.</param>
	/// <param name="c">Character.</param>
	/// <returns>Returns the part, or the whole string if the character does not occur.</returns>
	public static string BeforeFirst(this string s, char c)
	{
		int i = s.IndexOf(c);
		return ((i < 0) ? s : s[..i]);
	}


	/// <summary>Gets the part of a string before the first occurrence of a separator.</summary>
	/// <param name="s">String.</param>
	/// <param name="separator">Separator.</param>
	/// <returns>Returns the part, or the whole string if the separator does not occur.</returns>
	public static string BeforeFirst(this string s, string separator)
	{
		int i = s.IndexOf(separator, StringComparison.Ordinal);
		return ((i < 0) ? s : s[..i]);
	}


	/// <summary>Gets the part of a string before the last occurrence of a character.</summary>
	/// <param name="s">String.</param>
	/// <param name="c">Character.</param>
	/// <returns>Returns the part, or the whole string if the character does not occur.</returns>
	public static string BeforeLast(this string s, char c)
	{
		int i = s.LastIndexOf(c);
		return ((i < 0) ? s : s[..i]);
	}


	/// <summary>Gets the part of a string before the last occurrence of a separator.</summary>
	/// <param name="s">String.</param>
	/// <param name="separator">Separator.</param>
	/// <returns>Returns the part, or the whole string if the separator does not occur.</returns>
	public static string BeforeLast(this string s, string separator)
	{
		int i = s.LastIndexOf(separator, StringComparison.Ordinal);
		return ((i < 0) ? s : s[..i]);
	}


	/// <summary>Gets the part of a string after the first occurrence of a character.</summary>
	/// <param name="s">String.</param>
	/// <param name="c">Character.</param>
	/// <returns>Returns the part, or the whole string if the character does not occur.</returns>
	public static string AfterFirst(this string s, char c)
	{
		int i = s.IndexOf(c);
		return ((i < 0) ? s : s[(i + 1)..]);
	}


	/// <summary>Gets the part of a string after the first occurrence of a separator.</summary>
	/// <param name="s">String.</param>
	/// <param name="separator">Separator.</param>
	/// <returns>Returns the part, or the whole string if the separator does not occur.</returns>
	public static string AfterFirst(this string s, string separator)
	{
		int i = s.IndexOf(separator, StringComparison.Ordinal);
		return ((i < 0) ? s : s[(i + separator.Length)..]);
	}


	/// <summary>Gets the part of a string after the last occurrence of a character.</summary>
	/// <param name="s">String.</param>
	/// <param name="c">Character.</param>
	/// <returns>Returns the part, or the whole string if the character does not occur.</returns>
	public static string AfterLast(this string s, char c)
	{
		int i = s.LastIndexOf(c);
		return ((i < 0) ? s : s[(i + 1)..]);
	}


	/// <summary>Gets the part of a string after the last occurrence of a separator.</summary>
	/// <param name="s">String.</param>
	/// <param name="separator">Separator.</param>
	/// <returns>Returns the part, or the whole string if the separator does not occur.</returns>
	public static string AfterLast(this string s, string separator)
	{
		int i = s.LastIndexOf(separator, StringComparison.Ordinal);
		return ((i < 0) ? s : s[(i + separator.Length)..]);
	}


	/// <summary>Returns the part of a string after the given occurrence of a space and before the next one.</summary>
	/// <param name="s">String.</param>
	/// <param name="from">Occurrence of the delimiter where the part starts; 0 for the start of the string.</param>
	/// <param name="to">Occurrence of the delimiter where the part ends; 0 for the end of the string.</param>
	/// <returns>Returns the part; an empty string if the start delimiter does not occur.</returns>
	/// <remarks>Example: <c>"a b c d".Snip(1, 3)</c> returns <c>"b c"</c>.</remarks>
	public static string Snip(this string s, int from = 1, int to = 0)
	{
		return Snip(s, from, to, ' ');
	}


	/// <summary>Returns the part of a string after the first occurrence of a delimiter.</summary>
	/// <param name="s">String.</param>
	/// <param name="delimiters">Delimiter characters.</param>
	/// <returns>Returns the part; an empty string if no delimiter occurs.</returns>
	public static string Snip(this string s, params char[] delimiters)
	{
		return Snip(s, 1, 0, delimiters);
	}


	/// <summary>Returns the part of a string after the given occurrence of a delimiter.</summary>
	/// <param name="s">String.</param>
	/// <param name="from">Occurrence of the delimiter where the part starts; 0 for the start of the string.</param>
	/// <param name="delimiters">Delimiter characters.</param>
	/// <returns>Returns the part; an empty string if the start delimiter does not occur.</returns>
	public static string Snip(this string s, int from, params char[] delimiters)
	{
		return Snip(s, from, 0, delimiters);
	}


	/// <summary>Returns the part of a string between two occurrences of a delimiter.</summary>
	/// <param name="s">String.</param>
	/// <param name="from">Occurrence of the delimiter where the part starts; 0 for the start of the string.</param>
	/// <param name="to">Occurrence of the delimiter where the part ends; 0 (or less than <paramref name="from"/>) for the end of
	///                  the string.</param>
	/// <param name="delimiters">Delimiter characters.</param>
	/// <returns>Returns the part; an empty string if the start delimiter does not occur or both occurrences are equal. If the
	///          end delimiter does not occur, the part extends to the end of the string.</returns>
	public static string Snip(this string s, int from, int to, params char[] delimiters)
	{
		int start = -1, end = -1;

		for(int i = 0; i < from; i++)
		{
			start = s.IndexOfAny(delimiters, start + 1);
			if(start < 0) { return string.Empty; }
		}

		if((to <= 0) || (to < from)) { return s[(start + 1)..]; }
		if(to == from) { return string.Empty; }

		for(int i = 0; i < to; i++)
		{
			end = s.IndexOfAny(delimiters, end + 1);
			if(end < 0) { return s[(start + 1)..]; }
		}

		return s[(start + 1)..end];
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: replace                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Replaces all occurrences of a text.</summary>
	/// <param name="s">String.</param>
	/// <param name="oldValue">Text to replace.</param>
	/// <param name="newValue">Replacement.</param>
	/// <param name="caseSensitive">Determines if the search is case-sensitive.</param>
	/// <returns>Returns the string with all replacements.</returns>
	/// <exception cref="ArgumentException">Thrown when the text to replace is empty.</exception>
	public static string ReplaceAll(this string s, string oldValue, string newValue, bool caseSensitive = true)
	{
		if(string.IsNullOrEmpty(oldValue)) { throw new ArgumentException("The text to replace must not be empty.", nameof(oldValue)); }

		return s.Replace(oldValue, newValue, (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
	}


	/// <summary>Replaces the first occurrence of a text.</summary>
	/// <param name="s">String.</param>
	/// <param name="oldValue">Text to replace.</param>
	/// <param name="newValue">Replacement.</param>
	/// <param name="caseSensitive">Determines if the search is case-sensitive.</param>
	/// <returns>Returns the string with the replacement.</returns>
	/// <exception cref="ArgumentException">Thrown when the text to replace is empty.</exception>
	public static string ReplaceFirst(this string s, string oldValue, string newValue, bool caseSensitive = true)
	{
		if(string.IsNullOrEmpty(oldValue)) { throw new ArgumentException("The text to replace must not be empty.", nameof(oldValue)); }

		int i = s.IndexOf(oldValue, (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase));
		return ((i < 0) ? s : string.Concat(s.AsSpan(0, i), newValue, s.AsSpan(i + oldValue.Length)));
	}


	/// <summary>Replaces all consecutive occurrences of a text at the start of a string.</summary>
	/// <param name="s">String.</param>
	/// <param name="find">Text to replace.</param>
	/// <param name="replace">Replacement.</param>
	/// <param name="caseSensitive">Determines if the search is case-sensitive.</param>
	/// <returns>Returns the string with the replacements; "aaab".ReplaceStart("a", "x") returns "xxxb".</returns>
	/// <exception cref="ArgumentException">Thrown when the text to replace is empty.</exception>
	public static string ReplaceStart(this string s, string find, string replace, bool caseSensitive = true)
	{
		if(string.IsNullOrEmpty(find)) { throw new ArgumentException("The text to replace must not be empty.", nameof(find)); }

		StringComparison cmp = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		int pos = 0, n = 0;
		while(s.AsSpan(pos).StartsWith(find, cmp))
		{
			pos += find.Length;
			n++;
		}
		if(n == 0) { return s; }

		StringBuilder rval = new();
		rval.Insert(0, replace, n).Append(s.AsSpan(pos));

		return rval.ToString();
	}


	/// <summary>Replaces all consecutive occurrences of a text at the end of a string.</summary>
	/// <param name="s">String.</param>
	/// <param name="find">Text to replace.</param>
	/// <param name="replace">Replacement.</param>
	/// <param name="caseSensitive">Determines if the search is case-sensitive.</param>
	/// <returns>Returns the string with the replacements.</returns>
	/// <exception cref="ArgumentException">Thrown when the text to replace is empty.</exception>
	public static string ReplaceEnd(this string s, string find, string replace, bool caseSensitive = true)
	{
		if(string.IsNullOrEmpty(find)) { throw new ArgumentException("The text to replace must not be empty.", nameof(find)); }

		StringComparison cmp = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		int len = s.Length, n = 0;
		while(s.AsSpan(0, len).EndsWith(find, cmp))
		{
			len -= find.Length;
			n++;
		}
		if(n == 0) { return s; }

		StringBuilder rval = new(s, 0, len, len + (n * replace.Length));
		rval.Insert(len, replace, n);

		return rval.ToString();
	}
}
