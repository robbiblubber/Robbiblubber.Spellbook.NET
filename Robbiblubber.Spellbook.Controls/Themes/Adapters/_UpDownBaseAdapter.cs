namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Reflection;
using System.Runtime.Versioning;



/// <summary>This class applies themes to numeric and domain up-down controls.</summary>
[ThemeAdapter(typeof(UpDownBase))]
[SupportedOSPlatform("windows")]
internal sealed class _UpDownBaseAdapter: ThemeAdapter<UpDownBase>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the value of a private enumeration field of the button control as integer.</summary>
	/// <param name="buttons">Button control.</param>
	/// <param name="name">Field name.</param>
	/// <returns>Returns the value: 0 for none, 1 for the up button, 2 for the down button.</returns>
	private static int _GetButton(Control buttons, string name)
	{
		try
		{
			object? value = buttons.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(buttons);
			return ((value is null) ? 0 : Convert.ToInt32(value));
		}
		catch(Exception) { return 0; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<UpDownBase>                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Up-down controls get the field colors, read-only ones the read-only colors and disabled ones the disabled colors. The
	///          border is drawn like the border of a <see cref="ThemedTextBox"/>, the buttons are drawn flat with the glyph color of the
	///          theme. With <see cref="ThemeManager.ThemeTextSelection"/>, the selection of the text has the selection colors; the
	///          edit control is handled when its handle is created, which happens after the handle of the up-down control.</remarks>
	protected override void _Apply(UpDownBase control, Theme theme, ThemeAdapterState state)
	{
		if(!control.Enabled) { _ApplyColors(control, state, ThemeColor.DISABLED_BACK, ThemeColor.DISABLED_TEXT); }
		else if(control.ReadOnly) { _ApplyColors(control, state, ThemeColor.READ_ONLY_BACK, ThemeColor.READ_ONLY_TEXT); }
		else { _ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT); }

		void stateChanged(object? sender, EventArgs e)
		{
			if(state.Theme is Theme current) { _Apply(control, current, state); }
		}
		state.Subscribe("read-only", () => control.EnabledChanged += stateChanged, () => control.EnabledChanged -= stateChanged);

		_UpDownBorderWindow._Install(control, theme, state);

		TextBoxBase? edit = control.Controls.OfType<TextBoxBase>().FirstOrDefault();
		if(edit is not null)
		{
			void editHandleCreated(object? sender, EventArgs e)
			{
				if(state.Theme is Theme current) { _EditSelectionWindow._Update(control, edit.Handle, current, state); }
			}
			state.Subscribe("edit-handle", () => edit.HandleCreated += editHandleCreated, () => edit.HandleCreated -= editHandleCreated);
		}
		_EditSelectionWindow._Update(control, (((edit is not null) && edit.IsHandleCreated) ? edit.Handle : IntPtr.Zero), theme, state);

		Control? buttons = control.Controls.Cast<Control>().FirstOrDefault(m => m is not TextBoxBase);
		if(buttons is not null)
		{
			_Overpainter._Install(buttons, state, (g, current, states) =>
			{
				_ControlRenderer._PaintSpinButtons(g, buttons, current, _GetButton(buttons, "_mouseOver"), _GetButton(buttons, "_pushed"));
			}, false);
		}
	}


	/// <inheritdoc/>
	protected override bool _IncludesChildren(UpDownBase control)
	{
		return false;
	}
}
