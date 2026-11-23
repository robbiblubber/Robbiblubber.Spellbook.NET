namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to text boxes and rich text boxes.</summary>
[ThemeAdapter(typeof(TextBoxBase))]
[SupportedOSPlatform("windows")]
internal sealed class _TextBoxBaseAdapter: ThemeAdapter<TextBoxBase>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<TextBoxBase>                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Text boxes get the field colors, read-only text boxes the read-only colors and disabled text boxes the disabled
	///          colors; the scroll bars are dark in dark themes. The border is drawn like the border of a
	///          <see cref="ThemedTextBox"/>.</remarks>
	protected override void _Apply(TextBoxBase control, Theme theme, ThemeAdapterState state)
	{
		bool readOnly = (control.ReadOnly && (control is not RichTextBox));
		if(!control.Enabled) { _ApplyColors(control, state, ThemeColor.DISABLED_BACK, ThemeColor.DISABLED_TEXT); }
		else if(readOnly) { _ApplyColors(control, state, ThemeColor.READ_ONLY_BACK, ThemeColor.READ_ONLY_TEXT); }
		else { _ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT); }
		_ApplyWindowTheme(control, state);
		_TextBoxBorderWindow._Install(control, theme, state);

		void stateChanged(object? sender, EventArgs e)
		{
			if(state.Theme is Theme current) { _Apply(control, current, state); }
		}
		state.Subscribe("read-only", () =>
		{
			control.ReadOnlyChanged += stateChanged;
			control.EnabledChanged += stateChanged;
		}, () =>
		{
			control.ReadOnlyChanged -= stateChanged;
			control.EnabledChanged -= stateChanged;
		});
	}
}
