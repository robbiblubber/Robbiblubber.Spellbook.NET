namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to tree views.</summary>
[ThemeAdapter(typeof(TreeView))]
[SupportedOSPlatform("windows")]
internal sealed class _TreeViewAdapter: ThemeAdapter<TreeView>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<TreeView>                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The tree view gets the field colors; the selected node gets the selection colors of the theme.</remarks>
	protected override void _Apply(TreeView control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT);
		state.ApplyColor(control, nameof(TreeView.LineColor), ThemeColor.GRAY_TEXT);
		_ApplyWindowTheme(control, state);
		_SelectionWindow._Install(control, theme, state);
	}
}
