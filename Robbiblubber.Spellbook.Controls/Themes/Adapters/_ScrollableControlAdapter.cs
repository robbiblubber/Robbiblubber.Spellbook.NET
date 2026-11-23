namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to panels and user controls.</summary>
[ThemeAdapter(typeof(ScrollableControl))]
[SupportedOSPlatform("windows")]
internal sealed class _ScrollableControlAdapter: ThemeAdapter<ScrollableControl>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ScrollableControl>                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The colors are inherited from the parent control; the scroll bars are dark in dark themes.</remarks>
	protected override void _Apply(ScrollableControl control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, null);
		if(control.AutoScroll) { _ApplyWindowTheme(control, state); }
	}
}
