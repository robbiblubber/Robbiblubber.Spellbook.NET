namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to list boxes and checked list boxes.</summary>
[ThemeAdapter(typeof(ListBox))]
[SupportedOSPlatform("windows")]
internal sealed class _ListBoxAdapter: ThemeAdapter<ListBox>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws an item of a list box in theme colors.</summary>
	/// <param name="control">List box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="e">Event arguments.</param>
	private static void _DrawItem(ListBox control, Theme theme, DrawItemEventArgs e)
	{
		if((e.Index < 0) || (e.Index >= control.Items.Count)) { return; }

		_ControlRenderer._PaintListItem(e.Graphics, control, theme, e.Bounds, e.Bounds, control.GetItemText(control.Items[e.Index]), e.State, control.UseTabStops);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ListBox>                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>List boxes get the field colors; the scroll bars are dark in dark themes. List boxes that draw their items the standard
	///          way are owner drawn while the theme is applied, so selected items get the selection colors of the theme; owner drawn
	///          list boxes keep their drawing. Checked list boxes draw their items with themed check boxes and the selection colors.</remarks>
	protected override void _Apply(ListBox control, Theme theme, ThemeAdapterState state)
	{
		if((control is not CheckedListBox) && (control.DrawMode == DrawMode.Normal))
		{
			void drawItem(object? sender, DrawItemEventArgs e)
			{
				if(state.Theme is Theme current) { _DrawItem(control, current, e); }
			}
			state.Subscribe("draw", () => control.DrawItem += drawItem, () => control.DrawItem -= drawItem);

			state.SetValue(control, nameof(ListBox.ItemHeight), control.ItemHeight);
			state.SetValue(control, nameof(ListBox.DrawMode), DrawMode.OwnerDrawFixed);
		}

		_ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT);
		_ApplyWindowTheme(control, state);
		if(control is CheckedListBox checkedList) { _CheckedListBoxWindow._Install(checkedList, theme, state); }
		else if(control.DrawMode != DrawMode.Normal) { control.Invalidate(); }
	}
}
