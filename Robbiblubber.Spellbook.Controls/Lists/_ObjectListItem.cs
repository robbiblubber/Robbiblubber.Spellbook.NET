namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class wraps an object that does not implement <see cref="IListItem"/>, so it can be displayed in an item list.</summary>
[SupportedOSPlatform("windows")]
internal sealed class _ObjectListItem: IListItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="value">Wrapped object.</param>
	/// <param name="text">Display text.</param>
	public _ObjectListItem(object value, string text)
	{
		Value = value;
		Text = text;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="value">Wrapped object.</param>
	/// <param name="text">Display text.</param>
	/// <param name="detail">Detail text.</param>
	/// <param name="image">Image.</param>
	/// <param name="imageKey">Image key.</param>
	public _ObjectListItem(object value, string text, string? detail, Image? image, string? imageKey): this(value, text)
	{
		Detail = detail;
		Image = image;
		ImageKey = imageKey;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the wrapped object.</summary>
	public object Value
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the display text.</returns>
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
		get;
	}


	/// <inheritdoc/>
	public string? FilterText
	{
		get { return null; }
	}


	/// <inheritdoc/>
	public string? SortText
	{
		get { return null; }
	}


	/// <inheritdoc/>
	public string? Detail
	{
		get;
	}


	/// <inheritdoc/>
	public string? Description
	{
		get { return null; }
	}


	/// <inheritdoc/>
	public string? Category
	{
		get { return null; }
	}


	/// <inheritdoc/>
	public Image? Image
	{
		get;
	}


	/// <inheritdoc/>
	public string? ImageKey
	{
		get;
	}


	/// <inheritdoc/>
	public FontStyle FontStyle
	{
		get { return FontStyle.Regular; }
	}


	/// <inheritdoc/>
	public Color ForeColor
	{
		get { return Color.Empty; }
	}


	/// <inheritdoc/>
	public object? Tag
	{
		get { return Value; }
	}
}
