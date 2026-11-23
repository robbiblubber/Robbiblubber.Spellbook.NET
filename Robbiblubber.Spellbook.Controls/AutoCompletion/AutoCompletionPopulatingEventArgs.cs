namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.Populating"/> event.</summary>
/// <remarks>The event arguments are initialized with the result of the auto completion source, if any. Event handlers may modify the
///          item list and the result settings. Changes are applied to the auto completion list after the event has been raised.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionPopulatingEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="context">Auto completion context.</param>
	/// <param name="result">Auto completion source result.</param>
	public AutoCompletionPopulatingEventArgs(AutoCompletionContext context, AutoCompletionResult? result): base()
	{
		Context = context;
		Items = (result is null) ? new List<IAutoCompletionItem>() : new List<IAutoCompletionItem>(result.Items);

		if(result is not null)
		{
			IsIncomplete = result.IsIncomplete;
			ReplaceStart = result.ReplaceStart;
			ReplaceLength = result.ReplaceLength;
			MatchMode = result.MatchMode;
			FilterMode = result.FilterMode;
			CaseSensitive = result.CaseSensitive;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the auto completion context.</summary>
	public AutoCompletionContext Context
	{
		get;
	}


	/// <summary>Gets the auto completion items.</summary>
	public List<IAutoCompletionItem> Items
	{
		get;
	}


	/// <summary>Gets or sets if the result is incomplete.</summary>
	/// <remarks>See <see cref="AutoCompletionResult.IsIncomplete"/>.</remarks>
	public bool IsIncomplete
	{
		get; set;
	}


	/// <summary>Gets or sets the start index of the text range that is replaced when an item is accepted.</summary>
	/// <remarks>See <see cref="AutoCompletionResult.ReplaceStart"/>.</remarks>
	public int? ReplaceStart
	{
		get; set;
	}


	/// <summary>Gets or sets the length of the text range that is replaced when an item is accepted.</summary>
	/// <remarks>See <see cref="AutoCompletionResult.ReplaceLength"/>.</remarks>
	public int? ReplaceLength
	{
		get; set;
	}


	/// <summary>Gets or sets the match mode.</summary>
	/// <remarks>If this property is NULL, the control or provider setting is used.</remarks>
	public ListMatchMode? MatchMode
	{
		get; set;
	}


	/// <summary>Gets or sets the filter mode.</summary>
	/// <remarks>If this property is NULL, the control or provider setting is used.</remarks>
	public ListFilterMode? FilterMode
	{
		get; set;
	}


	/// <summary>Gets or sets if matching is case-sensitive.</summary>
	/// <remarks>If this property is NULL, the control or provider setting is used.</remarks>
	public bool? CaseSensitive
	{
		get; set;
	}
}
