namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class applies themes to buttons.</summary>
[ThemeAdapter(typeof(Button))]
[SupportedOSPlatform("windows")]
internal sealed class _ButtonAdapter: ThemeAdapter<Button>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color tables of the buttons.</summary>
	private static readonly ConditionalWeakTable<Button, ThemedButtonColors> _Tables = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<Button>                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Buttons are drawn like a <see cref="ThemedButton"/>: flat with rounded corners in the button colors of the theme; the
	///          default button of the form has an accent border. Back and text colors that the application has set explicitly are
	///          kept.</remarks>
	protected override void _Apply(Button control, Theme theme, ThemeAdapterState state)
	{
		state.SetValue(control, nameof(Button.FlatStyle), FlatStyle.Flat);
		state.SetValue(control, nameof(Button.UseVisualStyleBackColor), false);
		_ApplyColors(control, state, ThemeColor.BUTTON_BACK, ThemeColor.BUTTON_TEXT);

		bool isDefault = (control.FindForm()?.AcceptButton == control);
		state.ApplyColor(control.FlatAppearance, nameof(FlatButtonAppearance.BorderColor), (isDefault ? ThemeColor.ACCENT : ThemeColor.BUTTON_BORDER));
		state.ApplyColor(control.FlatAppearance, nameof(FlatButtonAppearance.MouseOverBackColor), ThemeColor.BUTTON_BACK_HOT);
		state.ApplyColor(control.FlatAppearance, nameof(FlatButtonAppearance.MouseDownBackColor), ThemeColor.BUTTON_BACK_PRESSED);

		ThemedButtonColors colors = _Tables.GetValue(control, m => new ThemedButtonColors());
		_Overpainter._Install(control, state, (g, current, states) =>
		{
			colors.Theme = current;
			colors.BackColor = control.BackColor;
			colors.TextColor = control.ForeColor;
			colors.HotBackColor = control.FlatAppearance.MouseOverBackColor;
			colors.PressedBackColor = control.FlatAppearance.MouseDownBackColor;

			_ControlRenderer._PaintButton(g, control, colors, (control.Parent?.BackColor ?? Color.Empty), states);
		});
	}
}
