namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class defines the colors and the highlight style of a <see cref="_ListPopupWindow{T}"/>.</summary>
/// <remarks>All colors are resolved: the component that shows the list replaces unset colors by its fallbacks before it applies the
///          style.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ListPopupStyle
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the back color of the list.</summary>
	public Color BackColor
	{
		get; init;
	} = SystemColors.Window;


	/// <summary>Gets the fore color of the list.</summary>
	public Color ForeColor
	{
		get; init;
	} = SystemColors.WindowText;


	/// <summary>Gets the back color of the selected item.</summary>
	public Color SelectionBackColor
	{
		get; init;
	} = SystemColors.Highlight;


	/// <summary>Gets the fore color of the selected item.</summary>
	public Color SelectionForeColor
	{
		get; init;
	} = SystemColors.HighlightText;


	/// <summary>Gets the color of the detail text.</summary>
	public Color DetailForeColor
	{
		get; init;
	} = SystemColors.GrayText;


	/// <summary>Gets the highlight style of matched characters.</summary>
	public ListHighlightStyle HighlightStyle
	{
		get; init;
	} = ListHighlightStyle.BOLD;


	/// <summary>Gets the color of matched characters for <see cref="ListHighlightStyle.COLOR"/>.</summary>
	public Color HighlightColor
	{
		get; init;
	} = SystemColors.HotTrack;


	/// <summary>Gets the border color, <see cref="Color.Empty"/> for the default border.</summary>
	public Color BorderColor
	{
		get; init;
	} = Color.Empty;
}
