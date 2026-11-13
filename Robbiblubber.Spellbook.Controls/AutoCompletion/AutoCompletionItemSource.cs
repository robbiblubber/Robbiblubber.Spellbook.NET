namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements an auto completion source that provides a fixed list of items.</summary>
[SupportedOSPlatform("windows")]
public class AutoCompletionItemSource: IAutoCompletionSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public AutoCompletionItemSource()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="items">Items.</param>
	public AutoCompletionItemSource(IEnumerable<IAutoCompletionItem> items)
	{
		Items.AddRange(items);
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="texts">Item texts.</param>
	public AutoCompletionItemSource(IEnumerable<string> texts)
	{
		Items.AddRange(texts.Select(m => new AutoCompletionItem(m)));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the items.</summary>
	public List<IAutoCompletionItem> Items
	{
		get;
	} = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds an item.</summary>
	/// <param name="text">Item text.</param>
	/// <param name="imageKey">Image key.</param>
	/// <returns>Returns the added item.</returns>
	public AutoCompletionItem Add(string text, string? imageKey = null)
	{
		AutoCompletionItem rval = new(text, imageKey);
		Items.Add(rval);

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IAutoCompletionSource                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken)
	{
		return new ValueTask<AutoCompletionResult?>(new AutoCompletionResult(Items));
	}
}
