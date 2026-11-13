namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to controls without an adapter of their own, like labels, check boxes and group boxes.</summary>
[ThemeAdapter(typeof(Control))]
[ThemeAdapter(typeof(ThemedButton))]
[ThemeAdapter(typeof(ThemedCheckBox))]
[ThemeAdapter(typeof(ThemedRadioButton))]
[ThemeAdapter(typeof(ThemedGroupBox))]
[ThemeAdapter(typeof(ThemedTextBox))]
[ThemeAdapter(typeof(ThemedTabControl))]
[SupportedOSPlatform("windows")]
internal sealed class _ControlAdapter: ThemeAdapter<Control>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<Control>                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The back and fore colors are inherited from the parent control; only colors that have been set to system colors
	///          explicitly are replaced. The themed controls draw themselves and use this adapter as well.</remarks>
	protected override void _Apply(Control control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, null);
	}
}
