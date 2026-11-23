namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to tab pages.</summary>
[ThemeAdapter(typeof(TabPage))]
[SupportedOSPlatform("windows")]
internal sealed class _TabPageAdapter: ThemeAdapter<TabPage>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<TabPage>                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The page gets the back color of the theme; the tab control draws the tabs (see <see cref="ThemedTabControl"/>).</remarks>
	protected override void _Apply(TabPage control, Theme theme, ThemeAdapterState state)
	{
		state.SetValue(control, nameof(TabPage.UseVisualStyleBackColor), false);
		_ApplyColors(control, state, ThemeColor.BACK, null);
		if(control.AutoScroll) { _ApplyWindowTheme(control, state); }
	}
}
