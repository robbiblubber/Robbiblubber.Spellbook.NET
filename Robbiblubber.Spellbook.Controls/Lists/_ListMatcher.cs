namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides matching, scoring and highlighting for list items (auto completion, rich combo box).</summary>
/// <remarks>Scores are layered: prefix matches score 1000 and above, matches at word boundaries 600 and above, other substring
///          matches 300 and above, subsequence (fuzzy) matches below 300. A negative score indicates that the candidate does not match.</remarks>
[SupportedOSPlatform("windows")]
internal static class _ListMatcher
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Score indicating no match.</summary>
	internal const int _NO_MATCH = -1;

	/// <summary>Base score for prefix matches.</summary>
	internal const int _SCORE_PREFIX = 1000;

	/// <summary>Base score for substring matches at a word boundary.</summary>
	private const int _SCORE_BOUNDARY = 600;

	/// <summary>Base score for other substring matches.</summary>
	private const int _SCORE_CONTAINS = 300;

	/// <summary>Base score for subsequence matches.</summary>
	private const int _SCORE_FUZZY = 100;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a position in a string is the start of a word.</summary>
	/// <param name="s">String.</param>
	/// <param name="index">Index.</param>
	/// <returns>Returns <c>true</c> if the position is a word boundary, otherwise returns <c>false</c>.</returns>
	private static bool _IsBoundary(string s, int index)
	{
		if(index == 0) { return true; }
		if(!char.IsLetterOrDigit(s[index - 1])) { return true; }
		return (char.IsUpper(s[index]) && char.IsLower(s[index - 1]));
	}


	/// <summary>Compares two characters.</summary>
	/// <param name="a">First character.</param>
	/// <param name="b">Second character.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns <c>true</c> if the characters are equal, otherwise returns <c>false</c>.</returns>
	private static bool _CharEquals(char a, char b, bool caseSensitive)
	{
		if(a == b) { return true; }
		if(caseSensitive) { return false; }
		return (char.ToUpperInvariant(a) == char.ToUpperInvariant(b));
	}


	/// <summary>Returns if the rest of a pattern is a subsequence of the rest of a candidate.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="candidateIndex">Start index in the candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="patternIndex">Start index in the pattern.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns <c>true</c> if the pattern rest is a subsequence of the candidate rest, otherwise returns <c>false</c>.</returns>
	private static bool _IsSubsequence(string candidate, int candidateIndex, string pattern, int patternIndex, bool caseSensitive)
	{
		for(int i = candidateIndex; (i < candidate.Length) && (patternIndex < pattern.Length); i++)
		{
			if(_CharEquals(candidate[i], pattern[patternIndex], caseSensitive)) { patternIndex++; }
		}

		return (patternIndex == pattern.Length);
	}


	/// <summary>Finds the best position for the next pattern character.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="start">Start index in the candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="patternIndex">Index of the pattern character.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the candidate index, or -1 if the character cannot be matched.</returns>
	/// <remarks>Preference: continuing a run of matched characters, then the next word boundary that still allows the rest of the
	///          pattern to match, then the leftmost occurrence. The caller guarantees that the pattern rest is a subsequence of the
	///          candidate rest, so the leftmost occurrence always keeps the match feasible.</remarks>
	private static int _FindNext(string candidate, int start, string pattern, int patternIndex, bool caseSensitive)
	{
		char c = pattern[patternIndex];
		int rval = -1;

		if((start < candidate.Length) && _CharEquals(candidate[start], c, caseSensitive)) { return start; }

		for(int i = start; i < candidate.Length; i++)
		{
			if(!_CharEquals(candidate[i], c, caseSensitive)) { continue; }
			if(rval < 0) { rval = i; }

			if(_IsBoundary(candidate, i) && _IsSubsequence(candidate, i + 1, pattern, patternIndex + 1, caseSensitive)) { return i; }
		}

		return rval;
	}


	/// <summary>Matches a candidate as a subsequence.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <param name="positions">Receives the matched positions, may be NULL.</param>
	/// <returns>Returns the match score, or <see cref="_NO_MATCH"/> if the candidate does not match.</returns>
	/// <remarks>Pattern characters are preferably matched at word boundaries (camel humps, characters after separators), so
	///          <c>cuorhi</c> matches <c>CustomerOrderHistory</c> as Cu-O-r-H-i. Runs of consecutive characters and matched
	///          boundaries increase the score; gaps, skipped words and long candidates decrease it.</remarks>
	private static int _MatchSubsequence(string candidate, string pattern, bool caseSensitive, List<int>? positions)
	{
		int rval = _SCORE_FUZZY;
		int first = -1, last = -1, matchedBoundaries = 0, boundaries = 0;

		if(!_IsSubsequence(candidate, 0, pattern, 0, caseSensitive)) { return _NO_MATCH; }

		for(int p = 0; p < pattern.Length; p++)
		{
			int i = _FindNext(candidate, last + 1, pattern, p, caseSensitive);

			if(i < 0) { return _NO_MATCH; }

			if((last >= 0) && (i == (last + 1))) { rval += 5; }
			else if(last >= 0) { rval -= Math.Min(i - last - 1, 3); }

			if(i == 0) { rval += 10; }
			if(_IsBoundary(candidate, i))
			{
				rval += 8;
				matchedBoundaries++;
			}

			positions?.Add(i);
			if(first < 0) { first = i; }
			last = i;
		}

		for(int i = first; i <= last; i++)
		{
			if(_IsBoundary(candidate, i)) { boundaries++; }
		}

		rval -= (boundaries - matchedBoundaries) * 4;
		rval -= Math.Min(candidate.Length / 8, 8);

		return Math.Clamp(rval, 0, _SCORE_CONTAINS - 1);
	}


	/// <summary>Matches a candidate against a pattern.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the match score, or <see cref="_NO_MATCH"/> if the candidate does not match.</returns>
	internal static int _Match(string candidate, string pattern, ListMatchMode mode, bool caseSensitive)
	{
		StringComparison comparison = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

		if(pattern.Length == 0) { return 0; }
		if(pattern.Length > candidate.Length) { return _NO_MATCH; }

		if(candidate.StartsWith(pattern, comparison))
		{
			int rval = _SCORE_PREFIX;
			if(candidate.Length == pattern.Length) { rval += 100; }
			if((!caseSensitive) && candidate.StartsWith(pattern, StringComparison.Ordinal)) { rval += 10; }

			return rval;
		}

		if(mode == ListMatchMode.PREFIX) { return _NO_MATCH; }

		int index = candidate.IndexOf(pattern, comparison);
		if(index > 0)
		{
			return ((_IsBoundary(candidate, index) ? _SCORE_BOUNDARY : _SCORE_CONTAINS) + 99 - Math.Min(index, 99));
		}

		if(mode == ListMatchMode.CONTAINS) { return _NO_MATCH; }

		return _MatchSubsequence(candidate, pattern, caseSensitive, null);
	}


	/// <summary>Gets the ranges of a candidate that match a pattern.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns a list of (start, length) ranges. The list is empty if the candidate does not match.</returns>
	internal static List<(int Start, int Length)> _GetHighlights(string candidate, string pattern, ListMatchMode mode, bool caseSensitive)
	{
		List<(int Start, int Length)> rval = new();
		StringComparison comparison = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

		if((pattern.Length == 0) || (pattern.Length > candidate.Length)) { return rval; }

		if(candidate.StartsWith(pattern, comparison))
		{
			rval.Add((0, pattern.Length));
			return rval;
		}

		if(mode == ListMatchMode.PREFIX) { return rval; }

		int index = candidate.IndexOf(pattern, comparison);
		if(index >= 0)
		{
			rval.Add((index, pattern.Length));
			return rval;
		}

		if(mode == ListMatchMode.CONTAINS) { return rval; }

		List<int> positions = new();
		if(_MatchSubsequence(candidate, pattern, caseSensitive, positions) < 0) { return rval; }

		foreach(int i in positions)
		{
			if((rval.Count > 0) && ((rval[^1].Start + rval[^1].Length) == i))
			{
				rval[^1] = (rval[^1].Start, rval[^1].Length + 1);
			}
			else { rval.Add((i, 1)); }
		}

		return rval;
	}


	/// <summary>Gets the match class of a score.</summary>
	/// <param name="score">Match score.</param>
	/// <returns>Returns 4 for exact matches, 3 for prefix matches, 2 for substring matches at a word boundary, 1 for other substring
	///          matches, 0 for subsequence matches and -1 for no match.</returns>
	/// <remarks>Usage ranking only reorders items within the same match class.</remarks>
	internal static int _GetClass(int score)
	{
		if(score >= (_SCORE_PREFIX + 100)) { return 4; }
		if(score >= _SCORE_PREFIX) { return 3; }
		if(score >= _SCORE_BOUNDARY) { return 2; }
		if(score >= _SCORE_CONTAINS) { return 1; }

		return ((score >= 0) ? 0 : -1);
	}


	/// <summary>Gets the length of the common prefix of two strings.</summary>
	/// <param name="a">First string.</param>
	/// <param name="b">Second string.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <returns>Returns the length of the common prefix.</returns>
	internal static int _CommonPrefixLength(string a, string b, bool caseSensitive)
	{
		int n = Math.Min(a.Length, b.Length);

		for(int i = 0; i < n; i++)
		{
			if(!_CharEquals(a[i], b[i], caseSensitive)) { return i; }
		}

		return n;
	}


	/// <summary>Compares two sort keys.</summary>
	/// <param name="a">First sort key.</param>
	/// <param name="b">Second sort key.</param>
	/// <returns>Returns a negative value if a precedes b, a positive value if b precedes a, otherwise returns 0.</returns>
	internal static int _CompareSortKeys(string a, string b)
	{
		int rval = string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
		if(rval == 0) { rval = string.CompareOrdinal(a, b); }

		return rval;
	}
}
