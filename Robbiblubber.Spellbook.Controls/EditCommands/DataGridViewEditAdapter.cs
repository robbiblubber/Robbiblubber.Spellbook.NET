namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Versioning;



/// <summary>This class implements the edit commands for <see cref="DataGridView"/> controls.</summary>
/// <remarks><para>While a cell is being edited, the commands are passed to the adapter of the editing control (for example the text box of
///          a text cell).</para>
///          <para>Otherwise, the commands work on the selected cells. Copy uses the clipboard format of the grid, as defined by
///          <see cref="DataGridView.ClipboardCopyMode"/>.</para>
///          <para>Paste writes tab-separated text, line by line, into the cells starting at the top left cell of the selection (or the
///          current cell, if nothing is selected), in display order. A single value is written into all selected cells. If the selected
///          rows are a multiple of the pasted lines (for example one copied row and two selected rows), the lines are repeated for all
///          selected rows; the same applies to the selected columns and the values of a line. Header texts copied by the grid are
///          skipped, read-only and hidden cells are skipped, and values that cannot be converted to the cell type are ignored.</para>
///          <para>When the text reaches the row for new records, rows are added for the remaining lines, so text can be pasted into the
///          row for new records to append rows. This requires that the grid allows adding rows and is not read-only; a bound grid
///          needs a list that implements <see cref="IBindingList"/> and allows new items, an unbound grid must not be in virtual
///          mode.</para>
///          <para>Delete removes the selected rows if whole rows are selected and the grid allows deleting rows. Rows are deleted like with
///          the Delete key, so <see cref="DataGridView.UserDeletingRow"/> (which can cancel the deletion) and
///          <see cref="DataGridView.UserDeletedRow"/> are raised. Otherwise, Delete clears the values of the selected cells.</para>
///          <para>Read-only cells are never changed. Select all and invert selection do not select the row for new records (except in
///          column selection mode, which selects whole columns).</para></remarks>
[SupportedOSPlatform("windows")]
public class DataGridViewEditAdapter: EditAdapter<DataGridView>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The protected method <c>DataGridView.ProcessDeleteKey</c>, which deletes the selected rows like the Delete key.</summary>
	private static readonly MethodInfo? _ProcessDeleteKey = typeof(DataGridView).GetMethod("ProcessDeleteKey", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Keys) }, null);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Splits clipboard text into rows and values.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the values by row.</returns>
	/// <remarks>A trailing line break is ignored, so text copied from a grid or a spreadsheet does not paste an empty extra row.</remarks>
	internal static string[][] _SplitText(string text)
	{
		string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');

		if(normalized.EndsWith('\n')) { normalized = normalized.Substring(0, normalized.Length - 1); }
		return normalized.Split('\n').Select(m => m.Split('\t')).ToArray();
	}


	/// <summary>Removes header texts that a data grid view has put on the clipboard together with the values.</summary>
	/// <param name="rows">Values by row.</param>
	/// <param name="headers">Header texts of the visible columns, in display order.</param>
	/// <returns>Returns the values without header texts.</returns>
	/// <remarks><para>Depending on <see cref="DataGridView.ClipboardCopyMode"/>, copied whole rows start with the (empty) text of the
	///          row header. They are recognized by an empty first value in every line and one value more than the grid has visible
	///          columns; the first value is then removed.</para>
	///          <para>A first line that only consists of column header texts is removed if more lines follow.</para></remarks>
	internal static string[][] _StripHeaders(string[][] rows, IList<string> headers)
	{
		string[][] rval = rows;

		if((rval.Length > 0) && rval.All(m => (m.Length == (headers.Count + 1)) && (m[0].Length == 0)))
		{
			rval = rval.Select(m => m.Skip(1).ToArray()).ToArray();
		}

		if((rval.Length > 1) && rval[0].All(m => headers.Contains(m))) { rval = rval.Skip(1).ToArray(); }

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the selected cells that can be changed.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns the cells.</returns>
	private static IEnumerable<DataGridViewCell> _GetWritableCells(DataGridView control)
	{
		return control.SelectedCells.Cast<DataGridViewCell>().Where(m => (!m.ReadOnly) && (!m.OwningRow!.IsNewRow));
	}


	/// <summary>Gets the selected rows that can be removed.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns the rows, an empty list if rows cannot be removed.</returns>
	private static List<DataGridViewRow> _GetRemovableRows(DataGridView control)
	{
		if(control.ReadOnly || (!control.AllowUserToDeleteRows)) { return new List<DataGridViewRow>(); }
		return control.SelectedRows.Cast<DataGridViewRow>().Where(m => !m.IsNewRow).ToList();
	}


	/// <summary>Gets the edit adapter of the editing control, if a cell is being edited.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns the editing control and its adapter, or NULL if no cell is being edited.</returns>
	private static (Control Control, IEditAdapter Adapter)? _GetEditingAdapter(DataGridView control)
	{
		if(control.IsCurrentCellInEditMode && (control.EditingControl is Control editing) && (EditAdapters.Find(editing) is IEditAdapter adapter)) { return (editing, adapter); }
		return null;
	}


	/// <summary>Returns if a data grid view has a target for pasting.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns <c>true</c> if a selected cell or the current cell can be changed, or if the current cell is in the row for new
	///          records and rows can be added; otherwise returns <c>false</c>.</returns>
	private static bool _HasPasteTarget(DataGridView control)
	{
		if(control.ReadOnly) { return false; }
		if(_GetWritableCells(control).Any()) { return true; }
		if((control.CurrentCell is null) || control.CurrentCell.ReadOnly) { return false; }

		return ((control.CurrentCell.OwningRow?.IsNewRow == false) || _CanAddRows(control));
	}


	/// <summary>Gets the list a data grid view is bound to.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns the list, or NULL if the grid is not bound to an <see cref="IBindingList"/>.</returns>
	private static IBindingList? _GetBindingList(DataGridView control)
	{
		if(control.DataSource is null) { return null; }
		return (ListBindingHelper.GetList(control.DataSource, control.DataMember) as IBindingList);
	}


	/// <summary>Returns if rows can be added to a data grid view by pasting.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns <c>true</c> if the grid has a row for new records and rows can be added to the grid or to its list, otherwise
	///          returns <c>false</c>.</returns>
	private static bool _CanAddRows(DataGridView control)
	{
		if(control.ReadOnly || (!control.AllowUserToAddRows) || (control.NewRowIndex < 0)) { return false; }
		if(control.DataSource is null) { return (!control.VirtualMode); }

		return (_GetBindingList(control)?.AllowNew ?? false);
	}


	/// <summary>Adds a row before the row for new records.</summary>
	/// <param name="control">Data grid view.</param>
	/// <returns>Returns the index of the new row, or -1 if no row could be added.</returns>
	/// <remarks>For a bound grid, a new item is added to the list and committed at once.</remarks>
	private static int _AddRow(DataGridView control)
	{
		IBindingList? list;
		object? item;
		int rval;

		if(control.DataSource is null) { return control.Rows.Add(); }

		list = _GetBindingList(control);
		if(list is null) { return -1; }

		try
		{
			item = list.AddNew();
			rval = list.IndexOf(item);
			if(list is ICancelAddNew pending) { pending.EndNew(rval); }
		}
		catch(Exception ex) when ((ex is InvalidOperationException) || (ex is NotSupportedException) || (ex is MissingMethodException)) { return -1; }

		return ((rval < control.Rows.Count) ? rval : -1);
	}


	/// <summary>Deletes the selected rows like the Delete key.</summary>
	/// <param name="control">Data grid view.</param>
	/// <remarks>The rows are deleted by the grid itself, so <see cref="DataGridView.UserDeletingRow"/> and
	///          <see cref="DataGridView.UserDeletedRow"/> are raised and handlers can cancel the deletion. If the method of the grid is not
	///          available, the rows are removed directly, without these events.</remarks>
	private static void _DeleteRows(DataGridView control)
	{
		if(_ProcessDeleteKey is not null)
		{
			_ProcessDeleteKey.Invoke(control, [Keys.Delete]);
			return;
		}

		try
		{
			foreach(DataGridViewRow i in _GetRemovableRows(control)) { control.Rows.Remove(i); }
		}
		catch(InvalidOperationException) {}
	}


	/// <summary>Selects all cells except the row for new records.</summary>
	/// <param name="control">Data grid view.</param>
	/// <remarks>In column selection mode, whole columns are selected, including the row for new records.</remarks>
	private static void _SelectAll(DataGridView control)
	{
		DataGridViewRow row;

		control.SelectAll();
		if((control.NewRowIndex < 0) || (control.SelectionMode == DataGridViewSelectionMode.FullColumnSelect) || (control.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)) { return; }

		row = control.Rows[control.NewRowIndex];
		if(row.Selected) { row.Selected = false; }
		foreach(DataGridViewCell i in row.Cells)
		{
			if(i.Selected) { i.Selected = false; }
		}
	}


	/// <summary>Clears the values of the selected cells that can be changed.</summary>
	/// <param name="control">Data grid view.</param>
	private static void _ClearCells(DataGridView control)
	{
		foreach(DataGridViewCell i in _GetWritableCells(control).ToList()) { i.Value = null; }
	}


	/// <summary>Writes a text into a cell, converted to the type of the cell.</summary>
	/// <param name="cell">Cell.</param>
	/// <param name="text">Text.</param>
	/// <remarks>Read-only cells are not changed; texts that cannot be converted are ignored.</remarks>
	private static void _SetText(DataGridViewCell cell, string text)
	{
		if(cell.ReadOnly) { return; }

		try
		{
			cell.Value = cell.ParseFormattedValue(text, cell.InheritedStyle, null, null);
		}
		catch(Exception ex) when ((ex is FormatException) || (ex is ArgumentException) || (ex is InvalidCastException)) {}
	}


	/// <summary>Pastes text repeatedly into the selection, if the selection is a multiple of the text.</summary>
	/// <param name="control">Data grid view.</param>
	/// <param name="text">Tab-separated text.</param>
	/// <returns>Returns <c>true</c> if the text has been pasted, <c>false</c> if it has to be pasted by <see cref="_Paste"/>.</returns>
	/// <remarks><para>A single value is written into all selected cells. Otherwise, the lines of the text are repeated if the number of
	///          selected rows is a multiple of the number of lines and greater than it; the values of a line are repeated if the number
	///          of selected columns is a multiple of the number of values and greater than it. Lines or values that are not repeated are
	///          pasted into the rows or columns following the top left selected cell.</para>
	///          <para>Only selected rows and columns are filled, also if the selection is not contiguous. The row for new records is not
	///          part of a repeated paste.</para></remarks>
	private static bool _PasteRepeated(DataGridView control, string text)
	{
		List<DataGridViewColumn> columns = control.Columns.Cast<DataGridViewColumn>().Where(m => m.Visible).OrderBy(m => m.DisplayIndex).ToList();
		List<DataGridViewCell> selected = control.SelectedCells.Cast<DataGridViewCell>().Where(m => (!m.OwningRow!.IsNewRow) && m.OwningColumn!.Visible).ToList();
		string[][] values = _StripHeaders(_SplitText(text), columns.Select(m => m.HeaderText).ToList());
		List<int> rows;
		List<int> positions;
		int width;
		bool repeatRows;
		bool repeatColumns;

		if(selected.Count < 2) { return false; }

		if((values.Length == 1) && (values[0].Length == 1))
		{
			foreach(DataGridViewCell i in selected) { _SetText(i, values[0][0]); }
			return true;
		}

		rows = selected.Select(m => m.RowIndex).Distinct().Order().ToList();
		positions = selected.Select(m => columns.IndexOf(m.OwningColumn!)).Distinct().Order().ToList();
		width = values.Max(m => m.Length);
		repeatRows = ((rows.Count > values.Length) && ((rows.Count % values.Length) == 0));
		repeatColumns = ((positions.Count > width) && ((positions.Count % width) == 0));
		if((!repeatRows) && (!repeatColumns)) { return false; }

		if(!repeatRows) { rows = Enumerable.Range(rows[0], values.Length).TakeWhile(m => (m < control.Rows.Count) && (!control.Rows[m].IsNewRow)).ToList(); }
		if(!repeatColumns) { positions = Enumerable.Range(positions[0], width).Where(m => m < columns.Count).ToList(); }

		for(int i = 0; i < rows.Count; i++)
		{
			string[] line = values[i % values.Length];
			for(int j = 0; j < positions.Count; j++)
			{
				if((j % width) < line.Length) { _SetText(control.Rows[rows[i]].Cells[columns[positions[j]].Index], line[j % width]); }
			}
		}

		return true;
	}


	/// <summary>Pastes text into the cells.</summary>
	/// <param name="control">Data grid view.</param>
	/// <param name="text">Tab-separated text.</param>
	/// <remarks><para>If the selection is a multiple of the text, it is filled by <see cref="_PasteRepeated"/>. Otherwise, the text is
	///          pasted starting at the top left cell of the selection, or at the current cell if nothing is selected. Header texts copied by the grid are skipped (see
	///          <see cref="_StripHeaders"/>).</para>
	///          <para>When the text reaches the row for new records, rows are added if possible (see <see cref="_CanAddRows"/>). If the
	///          current cell is in the row for new records, it is moved to the first pasted cell; this is done after the rows have been
	///          added, because the grid may set the current cell on its own while rows are added.</para></remarks>
	private static void _Paste(DataGridView control, string text)
	{
		if(_PasteRepeated(control, text)) { return; }

		List<DataGridViewColumn> columns = control.Columns.Cast<DataGridViewColumn>().Where(m => m.Visible).OrderBy(m => m.DisplayIndex).ToList();
		List<DataGridViewCell> selected = control.SelectedCells.Cast<DataGridViewCell>().Where(m => m.OwningColumn!.Visible).ToList();
		string[][] values = _StripHeaders(_SplitText(text), columns.Select(m => m.HeaderText).ToList());
		bool canAdd = _CanAddRows(control);
		bool moveCurrent;
		int firstColumn;
		int firstRow;
		int row;

		if(selected.Count > 0)
		{
			firstColumn = selected.Min(m => columns.IndexOf(m.OwningColumn!));
			firstRow = selected.Min(m => m.RowIndex);
		}
		else
		{
			firstColumn = columns.IndexOf(control.CurrentCell!.OwningColumn!);
			firstRow = control.CurrentCell.RowIndex;
		}

		moveCurrent = (canAdd && (control.CurrentCell?.OwningRow?.IsNewRow == true));
		if(moveCurrent) { control.CurrentCell = null; }

		row = firstRow;
		foreach(string[] i in values)
		{
			if((row >= control.Rows.Count) || control.Rows[row].IsNewRow)
			{
				if(!canAdd) { break; }
				if((row = _AddRow(control)) < 0) { break; }
			}

			for(int j = 0; (j < i.Length) && ((firstColumn + j) < columns.Count); j++)
			{
				_SetText(control.Rows[row].Cells[columns[firstColumn + j].Index], i[j]);
			}

			row++;
		}

		if(moveCurrent && (firstRow < control.Rows.Count) && (!control.Rows[firstRow].IsNewRow))
		{
			control.CurrentCell = control.Rows[firstRow].Cells[columns[firstColumn].Index];
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditAdapter<DataGridView>                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override bool CanExecute(Control control, EditCommand command)
	{
		if((control is DataGridView grid) && (_GetEditingAdapter(grid) is { } editing)) { return editing.Adapter.CanExecute(editing.Control, command); }
		return base.CanExecute(control, command);
	}


	/// <inheritdoc/>
	public override void Execute(Control control, EditCommand command)
	{
		if((control is DataGridView grid) && (_GetEditingAdapter(grid) is { } editing))
		{
			editing.Adapter.Execute(editing.Control, command);
			return;
		}

		base.Execute(control, command);
	}


	/// <inheritdoc/>
	protected override bool _CanExecute(DataGridView control, EditCommand command)
	{
		int selected;
		bool copyable;

		if(!control.Enabled) { return false; }

		selected = control.GetCellCount(DataGridViewElementStates.Selected);
		copyable = ((selected > 0) && (control.ClipboardCopyMode != DataGridViewClipboardCopyMode.Disable));

		return command switch
		{
			EditCommand.CUT => (copyable && (_GetWritableCells(control).Count() == selected)),
			EditCommand.COPY => copyable,
			EditCommand.PASTE => (_HasPasteTarget(control) && _Clipboard._ContainsText()),
			EditCommand.DELETE => ((_GetRemovableRows(control).Count > 0) || _GetWritableCells(control).Any()),
			EditCommand.SELECT_ALL => (control.MultiSelect && (control.RowCount > 0) && (control.ColumnCount > 0)),
			EditCommand.INVERT_SELECTION => (control.MultiSelect && (control.RowCount > 0) && (control.ColumnCount > 0)),
			EditCommand.CLEAR_SELECTION => (selected > 0),
			_ => false
		};
	}


	/// <inheritdoc/>
	protected override void _Execute(DataGridView control, EditCommand command)
	{
		DataObject? data;
		string text;

		switch(command)
		{
			case EditCommand.CUT:
			case EditCommand.COPY:
				data = control.GetClipboardContent();
				if((data is not null) && (!_Clipboard._TrySetDataObject(data, true))) { break; }
				if(command == EditCommand.CUT) { _ClearCells(control); }
				break;
			case EditCommand.PASTE:
				if(_Clipboard._TryGetText(out text)) { _Paste(control, text); }
				break;
			case EditCommand.DELETE:
				if(_GetRemovableRows(control).Count > 0)
				{
					_DeleteRows(control);
				}
				else { _ClearCells(control); }
				break;
			case EditCommand.SELECT_ALL:
				_SelectAll(control);
				break;
			case EditCommand.INVERT_SELECTION:
				control.InvertSelection();
				break;
			case EditCommand.CLEAR_SELECTION:
				control.ClearSelection();
				break;
		}
	}
}
