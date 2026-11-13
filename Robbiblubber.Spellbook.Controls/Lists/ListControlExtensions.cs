namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides selection extensions for <see cref="ListView"/>, <see cref="ListBox"/> and <see cref="DataGridView"/>
///          controls.</summary>
/// <remarks>The methods respect the selection mode of the control: on controls that only allow a single selection, selecting all items
///          and inverting the selection have no effect. List views in virtual mode are supported.</remarks>
[SupportedOSPlatform("windows")]
public static class ListControlExtensions
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of items of a list view, including list views in virtual mode.</summary>
	/// <param name="control">List view.</param>
	/// <returns>Returns the number of items.</returns>
	public static int GetItemCount(this ListView control)
	{
		return (control.VirtualMode ? control.VirtualListSize : control.Items.Count);
	}


	/// <summary>Returns if a list box allows selecting more than one item.</summary>
	/// <param name="control">List box.</param>
	/// <returns>Returns <c>true</c> if multiple items can be selected, otherwise returns <c>false</c>.</returns>
	public static bool IsMultiSelect(this ListBox control)
	{
		return ((control.SelectionMode == SelectionMode.MultiSimple) || (control.SelectionMode == SelectionMode.MultiExtended));
	}


	/// <summary>Selects all items of a list view.</summary>
	/// <param name="control">List view.</param>
	public static void SelectAll(this ListView control)
	{
		int count = control.GetItemCount();

		if((!control.MultiSelect) || (count == 0)) { return; }

		control.BeginUpdate();
		try
		{
			if(control.VirtualMode)
			{
				for(int i = 0; i < count; i++) { control.SelectedIndices.Add(i); }
			}
			else
			{
				foreach(ListViewItem i in control.Items) { i.Selected = true; }
			}
		}
		finally { control.EndUpdate(); }
	}


	/// <summary>Selects all items of a list box.</summary>
	/// <param name="control">List box.</param>
	public static void SelectAll(this ListBox control)
	{
		if((!control.IsMultiSelect()) || (control.Items.Count == 0)) { return; }

		control.BeginUpdate();
		try
		{
			for(int i = 0; i < control.Items.Count; i++) { control.SetSelected(i, true); }
		}
		finally { control.EndUpdate(); }
	}


	/// <summary>Clears the selection of a list view.</summary>
	/// <param name="control">List view.</param>
	public static void ClearSelection(this ListView control)
	{
		control.SelectedIndices.Clear();
	}


	/// <summary>Clears the selection of a list box.</summary>
	/// <param name="control">List box.</param>
	public static void ClearSelection(this ListBox control)
	{
		if(control.SelectionMode != SelectionMode.None) { control.ClearSelected(); }
	}


	/// <summary>Inverts the selection of a list view.</summary>
	/// <param name="control">List view.</param>
	public static void InvertSelection(this ListView control)
	{
		int count = control.GetItemCount();
		HashSet<int> selected;

		if((!control.MultiSelect) || (count == 0)) { return; }

		control.BeginUpdate();
		try
		{
			if(control.VirtualMode)
			{
				selected = new HashSet<int>(control.SelectedIndices.Cast<int>());
				control.SelectedIndices.Clear();

				for(int i = 0; i < count; i++)
				{
					if(!selected.Contains(i)) { control.SelectedIndices.Add(i); }
				}
			}
			else
			{
				foreach(ListViewItem i in control.Items) { i.Selected = (!i.Selected); }
			}
		}
		finally { control.EndUpdate(); }
	}


	/// <summary>Inverts the selection of a list box.</summary>
	/// <param name="control">List box.</param>
	public static void InvertSelection(this ListBox control)
	{
		HashSet<int> selected;

		if((!control.IsMultiSelect()) || (control.Items.Count == 0)) { return; }

		selected = new HashSet<int>(control.SelectedIndices.Cast<int>());

		control.BeginUpdate();
		try
		{
			for(int i = 0; i < control.Items.Count; i++) { control.SetSelected(i, (!selected.Contains(i))); }
		}
		finally { control.EndUpdate(); }
	}


	/// <summary>Inverts the selection of a data grid view.</summary>
	/// <param name="control">Data grid view.</param>
	/// <remarks>In row selection mode, the selection of the rows is inverted; in column selection mode, the selection of the columns;
	///          otherwise the selection of the cells. The row for new records is never selected.</remarks>
	public static void InvertSelection(this DataGridView control)
	{
		if(!control.MultiSelect) { return; }

		switch(control.SelectionMode)
		{
			case DataGridViewSelectionMode.FullRowSelect:
				foreach(DataGridViewRow i in control.Rows) { i.Selected = ((!i.IsNewRow) && (!i.Selected)); }
				break;
			case DataGridViewSelectionMode.FullColumnSelect:
				foreach(DataGridViewColumn i in control.Columns) { i.Selected = (!i.Selected); }
				break;
			default:
				foreach(DataGridViewRow i in control.Rows)
				{
					foreach(DataGridViewCell j in i.Cells) { j.Selected = ((!i.IsNewRow) && (!j.Selected)); }
				}
				break;
		}
	}


	/// <summary>Selects the cell or row a context menu is opened for, unless it is part of the selection already.</summary>
	/// <param name="control">Data grid view.</param>
	/// <param name="rowIndex">Row index of the clicked cell.</param>
	/// <param name="columnIndex">Column index of the clicked cell; -1 for the row header.</param>
	/// <returns>Returns <c>true</c> if the selection has been changed, otherwise returns <c>false</c>.</returns>
	/// <remarks><para>Call the method when the right mouse button is pressed on a cell (<see cref="DataGridView.CellMouseDown"/>), so
	///          the context menu applies to what has been clicked. If the clicked cell is selected, or for a row header the clicked
	///          row, the selection is kept, so a context menu can apply to several selected cells or rows. Otherwise the clicked cell
	///          becomes the current cell and is selected alone; a row header selects its row (in cell selection modes all visible
	///          cells of the row).</para>
	///          <para>Clicks on column headers are ignored. If the current cell cannot be changed, for example because the edited value
	///          cannot be committed, the selection is not changed.</para></remarks>
	public static bool SelectForContextMenu(this DataGridView control, int rowIndex, int columnIndex)
	{
		DataGridViewRow row;
		List<DataGridViewCell> cells;
		DataGridViewCell? target;

		if((rowIndex < 0) || (rowIndex >= control.Rows.Count) || (columnIndex >= control.Columns.Count)) { return false; }

		row = control.Rows[rowIndex];
		cells = row.Cells.Cast<DataGridViewCell>().Where(m => m.Visible).OrderBy(m => m.OwningColumn!.DisplayIndex).ToList();
		if(columnIndex < 0)
		{
			if(row.Selected || ((cells.Count > 0) && cells.All(m => m.Selected))) { return false; }
			target = ((control.CurrentCell?.RowIndex == rowIndex) ? control.CurrentCell : cells.FirstOrDefault());
		}
		else
		{
			target = row.Cells[columnIndex];
			if(target.Selected || (!target.Visible)) { return false; }
		}
		if(target is null) { return false; }

		try
		{
			control.CurrentCell = target;
		}
		catch(InvalidOperationException) { return false; }
		if(control.CurrentCell != target) { return false; }

		control.ClearSelection();
		if(columnIndex >= 0)
		{
			target.Selected = true;
		}
		else if((control.SelectionMode == DataGridViewSelectionMode.FullRowSelect) || (control.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
		{
			row.Selected = true;
		}
		else
		{
			foreach(DataGridViewCell i in cells) { i.Selected = true; }
		}

		return true;
	}
}
