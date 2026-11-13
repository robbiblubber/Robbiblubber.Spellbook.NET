namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements an auto completion item.</summary>
/// <remarks>Optional properties are set with object initializers: <c>new AutoCompletionItem("Customers", "table") { Detail = "dbo" }</c>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionItem: ListItem, IAutoCompletionItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public AutoCompletionItem(string text): base(text)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <param name="imageKey">Image key.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public AutoCompletionItem(string text, string? imageKey): base(text, imageKey)
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <param name="image">Image.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public AutoCompletionItem(string text, Image? image): base(text, image)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IAutoCompletionItem                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public string? InsertText
	{
		get; init;
	}


	/// <inheritdoc/>
	public int? CaretPosition
	{
		get; init;
	}
}
