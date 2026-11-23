namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests matching, scoring and highlighting of auto completion items.</summary>
[TestFixture]
public class AutoCompletionMatcherTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Matches a candidate.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if matching is case-sensitive.</param>
	/// <returns>Returns the score.</returns>
	private static int _Match(string candidate, string pattern, ListMatchMode mode = ListMatchMode.FUZZY, bool caseSensitive = false)
	{
		return _ListMatcher._Match(candidate, pattern, mode, caseSensitive);
	}


	/// <summary>Gets the match class of a candidate.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <param name="caseSensitive">Determines if matching is case-sensitive.</param>
	/// <returns>Returns the match class.</returns>
	private static int _Class(string candidate, string pattern, ListMatchMode mode = ListMatchMode.FUZZY, bool caseSensitive = false)
	{
		return _ListMatcher._GetClass(_Match(candidate, pattern, mode, caseSensitive));
	}


	/// <summary>Gets the highlighted ranges of a candidate.</summary>
	/// <param name="candidate">Candidate.</param>
	/// <param name="pattern">Pattern.</param>
	/// <param name="mode">Match mode.</param>
	/// <returns>Returns the ranges.</returns>
	private static (int Start, int Length)[] _Highlights(string candidate, string pattern, ListMatchMode mode = ListMatchMode.FUZZY)
	{
		return _ListMatcher._GetHighlights(candidate, pattern, mode, false).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>An empty pattern matches every candidate with score 0.</summary>
	[Test]
	public void EmptyPatternMatchesEverything()
	{
		Assert.That(_Match("Austria", string.Empty, ListMatchMode.PREFIX), Is.EqualTo(0));
		Assert.That(_Match(string.Empty, string.Empty, ListMatchMode.PREFIX), Is.EqualTo(0));
	}


	/// <summary>A pattern longer than the candidate never matches.</summary>
	[Test]
	public void LongerPatternDoesNotMatch()
	{
		Assert.That(_Match("Aus", "Austria"), Is.EqualTo(_ListMatcher._NO_MATCH));
	}


	/// <summary>Prefix matches ignore case unless matching is case-sensitive.</summary>
	[Test]
	public void PrefixMatchRespectsCaseSensitivity()
	{
		Assert.That(_Class("Austria", "aus", ListMatchMode.PREFIX), Is.EqualTo(3));
		Assert.That(_Match("Austria", "aus", ListMatchMode.PREFIX, true), Is.EqualTo(_ListMatcher._NO_MATCH));
		Assert.That(_Class("Austria", "Aus", ListMatchMode.PREFIX, true), Is.EqualTo(3));
	}


	/// <summary>A candidate equal to the pattern (ignoring case) is an exact match.</summary>
	[Test]
	public void ExactMatchHasHighestClass()
	{
		Assert.That(_Class("String", "String"), Is.EqualTo(4));
		Assert.That(_Class("Str", "str"), Is.EqualTo(4));
		Assert.That(_Match("String", "String"), Is.GreaterThan(_Match("StringBuilder", "String")));
	}


	/// <summary>A prefix with matching case scores higher than a prefix with different case.</summary>
	[Test]
	public void SameCasePrefixScoresHigher()
	{
		Assert.That(_Match("String", "Str"), Is.GreaterThan(_Match("string", "Str")));
		Assert.That(_ListMatcher._GetClass(_Match("string", "Str")), Is.EqualTo(3));
	}


	/// <summary>The prefix mode does not match substrings or subsequences.</summary>
	[Test]
	public void PrefixModeRejectsOtherMatches()
	{
		Assert.That(_Match("StreamReader", "Reader", ListMatchMode.PREFIX), Is.EqualTo(_ListMatcher._NO_MATCH));
		Assert.That(_Match("StringBuilder", "strbu", ListMatchMode.PREFIX), Is.EqualTo(_ListMatcher._NO_MATCH));
	}


	/// <summary>Substrings at word boundaries (camel humps, separators) rank above other substrings.</summary>
	[Test]
	public void ContainsModeRanksWordBoundaries()
	{
		Assert.That(_Class("StreamReader", "Reader", ListMatchMode.CONTAINS), Is.EqualTo(2));
		Assert.That(_Class("order_date", "date", ListMatchMode.CONTAINS), Is.EqualTo(2));
		Assert.That(_Class("StreamReader", "eader", ListMatchMode.CONTAINS), Is.EqualTo(1));
	}


	/// <summary>Substrings closer to the start of the candidate score higher.</summary>
	[Test]
	public void EarlierSubstringScoresHigher()
	{
		Assert.That(_Match("MyReader", "Reader", ListMatchMode.CONTAINS), Is.GreaterThan(_Match("StreamReader", "Reader", ListMatchMode.CONTAINS)));
	}


	/// <summary>The contains mode does not match subsequences.</summary>
	[Test]
	public void ContainsModeRejectsSubsequence()
	{
		Assert.That(_Match("StringBuilder", "strbu", ListMatchMode.CONTAINS), Is.EqualTo(_ListMatcher._NO_MATCH));
	}


	/// <summary>The fuzzy mode matches subsequences, always below substring matches.</summary>
	[Test]
	public void FuzzyModeMatchesSubsequence()
	{
		Assert.That(_Class("StringBuilder", "strbu"), Is.EqualTo(0));
		Assert.That(_Class("StringBuilder", "Bldr"), Is.EqualTo(0));
		Assert.That(_Class("CurrencyEntryOrderHistory", "cuorhi"), Is.EqualTo(0));
	}


	/// <summary>The fuzzy mode still ranks prefixes and substrings in their own classes.</summary>
	[Test]
	public void FuzzyModeKeepsBetterClasses()
	{
		Assert.That(_Class("StringBuilder", "Str"), Is.EqualTo(3));
		Assert.That(_Class("StringBuilder", "Builder"), Is.EqualTo(2));
	}


	/// <summary>The fuzzy mode does not match if a pattern character is missing.</summary>
	[Test]
	public void FuzzyModeRejectsMissingCharacters()
	{
		Assert.That(_Match("StringBuilder", "strbx"), Is.EqualTo(_ListMatcher._NO_MATCH));
		Assert.That(_Match("StringBuilder", "bs"), Is.EqualTo(_ListMatcher._NO_MATCH));
	}


	/// <summary>Fuzzy matches at word starts rank higher; skipped words are penalized.</summary>
	[Test]
	public void FuzzyModePrefersWordStarts()
	{
		Assert.That(_Match("CustomerOrderHistory", "cuorhi"), Is.GreaterThan(_Match("CurrencyEntryOrderHistory", "cuorhi")));
		Assert.That(_Match("InvoiceTaxRate", "InvTax"), Is.GreaterThan(_Match("InvoiceTotalTax", "InvTax")));
	}


	/// <summary>Fuzzy matching respects case sensitivity.</summary>
	[Test]
	public void FuzzyModeRespectsCaseSensitivity()
	{
		Assert.That(_Match("StringBuilder", "SB", ListMatchMode.FUZZY, true), Is.GreaterThanOrEqualTo(0));
		Assert.That(_Match("StringBuilder", "sb", ListMatchMode.FUZZY, true), Is.EqualTo(_ListMatcher._NO_MATCH));
	}


	/// <summary>Prefix matches are highlighted as one range.</summary>
	[Test]
	public void PrefixIsHighlighted()
	{
		Assert.That(_Highlights("Austria", "aus", ListMatchMode.PREFIX), Is.EqualTo(new[] { (0, 3) }));
	}


	/// <summary>Substring matches are highlighted at their position; the prefix mode does not highlight substrings.</summary>
	[Test]
	public void SubstringIsHighlighted()
	{
		Assert.That(_Highlights("StreamReader", "reader", ListMatchMode.CONTAINS), Is.EqualTo(new[] { (6, 6) }));
		Assert.That(_Highlights("StreamReader", "reader", ListMatchMode.PREFIX), Is.Empty);
	}


	/// <summary>Fuzzy matches highlight the matched characters, adjacent characters are merged into ranges.</summary>
	[Test]
	public void FuzzyMatchIsHighlightedInRanges()
	{
		Assert.That(_Highlights("CustomerOrderHistory", "cuorhi"), Is.EqualTo(new[] { (0, 2), (8, 2), (13, 2) }));
		Assert.That(_Highlights("StringBuilder", "strbu"), Is.EqualTo(new[] { (0, 3), (6, 2) }));
	}


	/// <summary>Nothing is highlighted for an empty pattern or a candidate that does not match.</summary>
	[Test]
	public void NothingIsHighlightedWithoutMatch()
	{
		Assert.That(_Highlights("Austria", string.Empty), Is.Empty);
		Assert.That(_Highlights("Austria", "xyz"), Is.Empty);
		Assert.That(_Highlights("Aus", "Austria"), Is.Empty);
	}


	/// <summary>Scores map to match classes.</summary>
	/// <param name="score">Score.</param>
	/// <param name="expected">Expected class.</param>
	[TestCase(-1, -1)]
	[TestCase(0, 0)]
	[TestCase(299, 0)]
	[TestCase(300, 1)]
	[TestCase(599, 1)]
	[TestCase(600, 2)]
	[TestCase(999, 2)]
	[TestCase(1000, 3)]
	[TestCase(1099, 3)]
	[TestCase(1100, 4)]
	public void ScoreMapsToClass(int score, int expected)
	{
		Assert.That(_ListMatcher._GetClass(score), Is.EqualTo(expected));
	}


	/// <summary>The common prefix length respects case sensitivity.</summary>
	/// <param name="a">First string.</param>
	/// <param name="b">Second string.</param>
	/// <param name="caseSensitive">Determines if the comparison is case-sensitive.</param>
	/// <param name="expected">Expected length.</param>
	[TestCase("Germany", "Greece", false, 1)]
	[TestCase("abc", "ABD", false, 2)]
	[TestCase("abc", "ABD", true, 0)]
	[TestCase("abc", "abc", true, 3)]
	[TestCase("abc", "ab", false, 2)]
	[TestCase("", "abc", false, 0)]
	public void CommonPrefixLengthIsComputed(string a, string b, bool caseSensitive, int expected)
	{
		Assert.That(_ListMatcher._CommonPrefixLength(a, b, caseSensitive), Is.EqualTo(expected));
	}


	/// <summary>Sort keys are compared case-insensitively, with an ordinal comparison as tie breaker.</summary>
	[Test]
	public void SortKeysIgnoreCaseFirst()
	{
		string[] keys = new string[] { "banana", "Apple", "apple", "Banana", "cherry" };

		Array.Sort(keys, _ListMatcher._CompareSortKeys);

		Assert.That(keys, Is.EqualTo(new[] { "Apple", "apple", "Banana", "banana", "cherry" }));
		Assert.That(_ListMatcher._CompareSortKeys("same", "same"), Is.EqualTo(0));
	}
}
