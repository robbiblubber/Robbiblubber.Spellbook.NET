namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to data grid views.</summary>
[ThemeAdapter(typeof(DataGridView))]
[SupportedOSPlatform("windows")]
internal sealed class _DataGridViewAdapter: ThemeAdapter<DataGridView>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<DataGridView>                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Cells get the field and selection colors, headers the header colors; header visual styles are switched off, because
	///          they ignore the colors. Cell styles of columns, rows and cells are kept.</remarks>
	protected override void _Apply(DataGridView control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, null);
		state.ApplyColor(control, nameof(DataGridView.BackgroundColor), ThemeColor.FIELD_BACK);
		state.ApplyColor(control, nameof(DataGridView.GridColor), ThemeColor.GRID_LINE);

		DataGridViewCellStyle cells = control.DefaultCellStyle;
		state.ApplyColor(cells, nameof(DataGridViewCellStyle.BackColor), ThemeColor.FIELD_BACK);
		state.ApplyColor(cells, nameof(DataGridViewCellStyle.ForeColor), ThemeColor.FIELD_TEXT);
		state.ApplyColor(cells, nameof(DataGridViewCellStyle.SelectionBackColor), ThemeColor.SELECTION_BACK);
		state.ApplyColor(cells, nameof(DataGridViewCellStyle.SelectionForeColor), ThemeColor.SELECTION_TEXT);

		state.SetValue(control, nameof(DataGridView.EnableHeadersVisualStyles), false);
		foreach(DataGridViewCellStyle i in new[] { control.ColumnHeadersDefaultCellStyle, control.RowHeadersDefaultCellStyle })
		{
			state.SetValue(i, nameof(DataGridViewCellStyle.BackColor), theme[ThemeColor.HEADER_BACK]);
			state.SetValue(i, nameof(DataGridViewCellStyle.ForeColor), theme[ThemeColor.HEADER_TEXT]);
			state.SetValue(i, nameof(DataGridViewCellStyle.SelectionBackColor), theme[ThemeColor.SELECTION_BACK]);
			state.SetValue(i, nameof(DataGridViewCellStyle.SelectionForeColor), theme[ThemeColor.SELECTION_TEXT]);
		}
	}
}
