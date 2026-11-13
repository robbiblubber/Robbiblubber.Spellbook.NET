namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class applies themes to check boxes and radio buttons.</summary>
[ThemeAdapter(typeof(CheckBox))]
[ThemeAdapter(typeof(RadioButton))]
[SupportedOSPlatform("windows")]
internal sealed class _CheckBoxAdapter: ThemeAdapter<ButtonBase>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color tables of the controls.</summary>
	private static readonly ConditionalWeakTable<ButtonBase, ThemedToggleColors> _Tables = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ButtonBase>                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Check boxes and radio buttons are drawn like a <see cref="ThemedCheckBox"/> or <see cref="ThemedRadioButton"/>. The
	///          back and text colors are inherited from the parent control unless the application has set them explicitly. Controls
	///          with <see cref="FlatStyle.System"/> are switched to <see cref="FlatStyle.Standard"/>, because Windows draws them
	///          otherwise.</remarks>
	protected override void _Apply(ButtonBase control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, null);
		if(control.FlatStyle == FlatStyle.System) { state.SetValue(control, nameof(ButtonBase.FlatStyle), FlatStyle.Standard); }

		ThemedToggleColors colors = _Tables.GetValue(control, m => new ThemedToggleColors());
		_Overpainter._Install(control, state, (g, current, states) =>
		{
			colors.Theme = current;
			colors.TextColor = control.ForeColor;

			if(control is CheckBox check)
			{
				_ControlRenderer._PaintToggle(g, control, colors, check.CheckState, false, check.Appearance, check.CheckAlign, states);
			}
			else if(control is RadioButton radio)
			{
				_ControlRenderer._PaintToggle(g, control, colors, (radio.Checked ? CheckState.Checked : CheckState.Unchecked), true, radio.Appearance, radio.CheckAlign, states);
			}
		});
	}
}
