namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents the result of an auto completion request.</summary>
/// <remarks>Optional properties are set with object initializers: <c>new AutoCompletionResult(items) { MatchMode = ListMatchMode.FUZZY }</c>.
///          Properties that are NULL fall back to the control settings, and then to the provider settings.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionResult
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="items">Items.</param>
	/// <exception cref="ArgumentNullException">Thrown when the items are NULL.</exception>
	public AutoCompletionResult(IEnumerable<IAutoCompletionItem> items)
	{
		if(items is null) { throw new ArgumentNullException(nameof(items)); }
		Items = items.ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the auto completion items.</summary>
	public IReadOnlyList<IAutoCompletionItem> Items
	{
		get;
	}


	/// <summary>Gets if the result is incomplete.</summary>
	/// <remarks>If a result is incomplete, the source is requested again on every keystroke. Otherwise, the result is reused and
	///          filtered locally until the caret leaves the current word.</remarks>
	public bool IsIncomplete
	{
		get; init;
	}


	/// <summary>Gets the start index of the text range that is replaced when an item is accepted.</summary>
	/// <remarks>The index refers to <see cref="AutoCompletionContext.Text"/>. If this property is NULL, the current word is replaced.
	///          The text between this index and the caret is used as the filter text.</remarks>
	public int? ReplaceStart
	{
		get; init;
	}


	/// <summary>Gets the length of the text range that is replaced when an item is accepted.</summary>
	/// <remarks>The length refers to <see cref="AutoCompletionContext.Text"/> and is extended by the characters typed after the
	///          request. If this property is NULL, the text from <see cref="ReplaceStart"/> up to the caret is replaced.</remarks>
	public int? ReplaceLength
	{
		get; init;
	}


	/// <summary>Gets the match mode for this result.</summary>
	public ListMatchMode? MatchMode
	{
		get; init;
	}


	/// <summary>Gets the filter mode for this result.</summary>
	public ListFilterMode? FilterMode
	{
		get; init;
	}


	/// <summary>Gets if matching is case-sensitive for this result.</summary>
	public bool? CaseSensitive
	{
		get; init;
	}
}
