namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that represent items of item lists (auto completion, rich combo box) implement this interface.</summary>
[SupportedOSPlatform("windows")]
public interface IListItem
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text displayed in the list.</summary>
	public string Text { get; }


	/// <summary>Gets the text the typed text is matched against.</summary>
	/// <remarks>If this property is NULL, <see cref="Text"/> is used.</remarks>
	public string? FilterText { get; }


	/// <summary>Gets the text that determines the item order.</summary>
	/// <remarks>If this property is NULL, <see cref="Text"/> is used. Items are sorted ordinally and case-insensitive.</remarks>
	public string? SortText { get; }


	/// <summary>Gets an additional text displayed right-aligned and grayed next to the item text.</summary>
	public string? Detail { get; }


	/// <summary>Gets a description for the item.</summary>
	/// <remarks>The description of the selected item can be shown next to the list. Line breaks are allowed.</remarks>
	public string? Description { get; }


	/// <summary>Gets the item category.</summary>
	/// <remarks>Categories group items of the same kind (for example tables, columns, keywords). The category is currently not
	///          evaluated by the lists; it is available for sources, event handlers and future filtering.</remarks>
	public string? Category { get; }


	/// <summary>Gets the item image.</summary>
	/// <remarks>If this property is set, it takes precedence over <see cref="ImageKey"/>.</remarks>
	public Image? Image { get; }


	/// <summary>Gets the key of the item image in the image list of the component that shows the list.</summary>
	public string? ImageKey { get; }


	/// <summary>Gets the item font style.</summary>
	public FontStyle FontStyle { get; }


	/// <summary>Gets the item text color.</summary>
	/// <remarks>If this property is <see cref="Color.Empty"/>, the fore color of the list is used.</remarks>
	public Color ForeColor { get; }


	/// <summary>Gets a user-defined object associated with the item.</summary>
	public object? Tag { get; }
}
