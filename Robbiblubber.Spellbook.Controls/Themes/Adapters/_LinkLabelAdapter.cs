namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to link labels.</summary>
[ThemeAdapter(typeof(LinkLabel))]
[SupportedOSPlatform("windows")]
internal sealed class _LinkLabelAdapter: ThemeAdapter<LinkLabel>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<LinkLabel>                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void _Apply(LinkLabel control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, null);
		state.ApplyColor(control, nameof(LinkLabel.LinkColor), ThemeColor.LINK);
		state.ApplyColor(control, nameof(LinkLabel.VisitedLinkColor), ThemeColor.LINK);
		state.ApplyColor(control, nameof(LinkLabel.ActiveLinkColor), ThemeColor.ACCENT_PRESSED);
		state.ApplyColor(control, nameof(LinkLabel.DisabledLinkColor), ThemeColor.DISABLED_TEXT);
	}
}
