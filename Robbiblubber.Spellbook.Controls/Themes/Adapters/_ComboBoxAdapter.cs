namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to standard combo boxes.</summary>
[ThemeAdapter(typeof(ComboBox))]
[SupportedOSPlatform("windows")]
internal sealed class _ComboBoxAdapter: ThemeAdapter<ComboBox>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws an item of a combo box in theme colors.</summary>
	/// <param name="control">Combo box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="e">Event arguments.</param>
	private static void _DrawItem(ComboBox control, Theme theme, DrawItemEventArgs e)
	{
		string? text = (((e.Index >= 0) && (e.Index < control.Items.Count)) ? control.GetItemText(control.Items[e.Index]) : string.Empty);
		_ControlRenderer._PaintListItem(e.Graphics, control, theme, e.Bounds, e.Bounds, text, e.State, false);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ComboBox>                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The combo box is drawn flat, because the system style ignores the back color of drop-down lists. Combo boxes that
	///          draw their items the standard way are owner drawn while the theme is applied, so selected items get the selection colors
	///          of the theme; owner drawn combo boxes keep their drawing.</remarks>
	protected override void _Apply(ComboBox control, Theme theme, ThemeAdapterState state)
	{
		if(control.DrawMode == DrawMode.Normal)
		{
			void drawItem(object? sender, DrawItemEventArgs e)
			{
				if(state.Theme is Theme current) { _DrawItem(control, current, e); }
			}
			state.Subscribe("draw", () => control.DrawItem += drawItem, () => control.DrawItem -= drawItem);

			state.SetValue(control, nameof(ComboBox.ItemHeight), control.ItemHeight);
			state.SetValue(control, nameof(ComboBox.DrawMode), DrawMode.OwnerDrawFixed);
		}

		state.SetValue(control, nameof(ComboBox.FlatStyle), FlatStyle.Flat);
		_ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT);
		_ApplyWindowTheme(control, state, "DarkMode_CFD");
		IntPtr edit = (control.IsHandleCreated ? _NativeMethods._FindWindowEx(control.Handle, IntPtr.Zero, "Edit", null) : IntPtr.Zero);
		_EditSelectionWindow._Update(control, edit, theme, state);
	}


	/// <inheritdoc/>
	protected override bool _IncludesChildren(ComboBox control)
	{
		return false;
	}
}
