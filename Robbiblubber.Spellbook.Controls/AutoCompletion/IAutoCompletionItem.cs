namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that represent auto completion items implement this interface.</summary>
/// <remarks>Auto completion items are list items (<see cref="IListItem"/>) with information on how they are inserted.</remarks>
[SupportedOSPlatform("windows")]
public interface IAutoCompletionItem: IListItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text that is inserted when the item is accepted.</summary>
	/// <remarks>If this property is NULL, <see cref="IListItem.Text"/> is inserted.</remarks>
	public string? InsertText { get; }


	/// <summary>Gets the caret position after the item has been inserted, relative to the start of the inserted text.</summary>
	/// <remarks>If this property is NULL, the caret is placed at the end of the inserted text.</remarks>
	public int? CaretPosition { get; }
}
