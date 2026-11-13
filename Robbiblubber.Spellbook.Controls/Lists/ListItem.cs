namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements an item of an item list (auto completion, rich combo box).</summary>
/// <remarks>Optional properties are set with object initializers: <c>new ListItem("Customers", "table") { Detail = "dbo" }</c>.</remarks>
[SupportedOSPlatform("windows")]
public class ListItem: IListItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public ListItem(string text)
	{
		Text = text ?? throw new ArgumentNullException(nameof(text));
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <param name="imageKey">Image key.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public ListItem(string text, string? imageKey): this(text)
	{
		ImageKey = imageKey;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Item text.</param>
	/// <param name="image">Image.</param>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public ListItem(string text, Image? image): this(text)
	{
		Image = image;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the item text.</returns>
	public override string ToString()
	{
		return Text;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IListItem                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public string Text
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? FilterText
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? SortText
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? Detail
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? Description
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? Category
	{
		get; init;
	}


	/// <inheritdoc/>
	public Image? Image
	{
		get; init;
	}


	/// <inheritdoc/>
	public string? ImageKey
	{
		get; init;
	}


	/// <inheritdoc/>
	public FontStyle FontStyle
	{
		get; init;
	} = FontStyle.Regular;


	/// <inheritdoc/>
	public Color ForeColor
	{
		get; init;
	} = Color.Empty;


	/// <inheritdoc/>
	public object? Tag
	{
		get; init;
	}
}
