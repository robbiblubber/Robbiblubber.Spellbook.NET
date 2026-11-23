namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to scroll bars.</summary>
[ThemeAdapter(typeof(ScrollBar))]
[SupportedOSPlatform("windows")]
internal sealed class _ScrollBarAdapter: ThemeAdapter<ScrollBar>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ScrollBar>                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void _Apply(ScrollBar control, Theme theme, ThemeAdapterState state)
	{
		_ApplyWindowTheme(control, state);
	}
}
