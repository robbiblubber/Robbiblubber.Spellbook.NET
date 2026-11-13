namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="ListControlExtensions"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class ListControlExtensionsTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a list box.</summary>
	/// <param name="mode">Selection mode.</param>
	/// <returns>Returns the list box.</returns>
	private static ListBox _CreateListBox(SelectionMode mode)
	{
		ListBox rval = new() { SelectionMode = mode };

		rval.Items.AddRange(new object[] { "a", "b", "c" });
		_ = rval.Handle;

		return rval;
	}


	/// <summary>Creates a data grid view with three rows, two columns and the row for new records.</summary>
	/// <param name="mode">Selection mode.</param>
	/// <returns>Returns the data grid view.</returns>
	private static DataGridView _CreateGrid(DataGridViewSelectionMode mode)
	{
		DataGridView rval = new() { AllowUserToAddRows = true, MultiSelect = true };

		rval.Columns.Add("A", "A");
		rval.Columns.Add("B", "B");
		foreach(DataGridViewColumn i in rval.Columns) { i.SortMode = DataGridViewColumnSortMode.NotSortable; }
		rval.SelectionMode = mode;
		rval.Rows.Add("1", "x");
		rval.Rows.Add("2", "y");
		rval.Rows.Add("3", "z");
		_ = rval.Handle;
		rval.ClearSelection();

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Selecting all items of a single-selection list box has no effect and does not throw.</summary>
	[Test]
	public void ListBoxSingleSelectionIgnoresSelectAll()
	{
		using ListBox box = _CreateListBox(SelectionMode.One);

		box.SelectedIndex = 1;
		Assert.DoesNotThrow(() => box.SelectAll());
		Assert.DoesNotThrow(() => box.InvertSelection());

		Assert.That(box.SelectedIndex, Is.EqualTo(1));
		Assert.That(box.IsMultiSelect(), Is.False);
	}


	/// <summary>Clearing the selection of a list box without selection does not throw.</summary>
	[Test]
	public void ListBoxWithoutSelectionIgnoresClear()
	{
		using ListBox box = _CreateListBox(SelectionMode.None);

		Assert.DoesNotThrow(() => box.ClearSelection());
	}


	/// <summary>Multi-select list boxes select all and invert.</summary>
	[Test]
	public void ListBoxMultiSelection()
	{
		using ListBox box = _CreateListBox(SelectionMode.MultiSimple);

		Assert.That(box.IsMultiSelect(), Is.True);

		box.SetSelected(0, true);
		box.InvertSelection();
		Assert.That(box.SelectedIndices.Cast<int>(), Is.EqualTo(new int[] { 1, 2 }));

		box.SelectAll();
		Assert.That(box.SelectedIndices.Count, Is.EqualTo(3));

		box.ClearSelection();
		Assert.That(box.SelectedIndices.Count, Is.EqualTo(0));
	}


	/// <summary>List views in virtual mode are supported.</summary>
	[Test]
	public void ListViewVirtualMode()
	{
		using ListView view = new() { View = View.Details, MultiSelect = true, VirtualMode = true, VirtualListSize = 5 };

		view.Columns.Add("Name");
		view.RetrieveVirtualItem += (sender, e) => { e.Item = new ListViewItem("Item " + e.ItemIndex); };
		_ = view.Handle;

		Assert.That(view.GetItemCount(), Is.EqualTo(5));

		view.SelectedIndices.Add(1);
		view.InvertSelection();
		Assert.That(view.SelectedIndices.Cast<int>().OrderBy(m => m), Is.EqualTo(new int[] { 0, 2, 3, 4 }));

		view.SelectAll();
		Assert.That(view.SelectedIndices.Count, Is.EqualTo(5));

		view.ClearSelection();
		Assert.That(view.SelectedIndices.Count, Is.EqualTo(0));
	}


	/// <summary>Inverting the row selection of a grid does not select the row for new records.</summary>
	[Test]
	public void GridInvertsRows()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.FullRowSelect);

		grid.Rows[0].Selected = true;
		grid.InvertSelection();

		Assert.That(grid.Rows[0].Selected, Is.False);
		Assert.That(grid.Rows[1].Selected, Is.True);
		Assert.That(grid.Rows[2].Selected, Is.True);
		Assert.That(grid.Rows[3].IsNewRow, Is.True);
		Assert.That(grid.Rows[3].Selected, Is.False);
	}


	/// <summary>Inverting the cell selection of a grid inverts each cell.</summary>
	[Test]
	public void GridInvertsCells()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.CellSelect);

		grid.Rows[1].Cells[1].Selected = true;
		grid.InvertSelection();

		Assert.That(grid.GetCellCount(DataGridViewElementStates.Selected), Is.EqualTo(5));
		Assert.That(grid.Rows[1].Cells[1].Selected, Is.False);
		Assert.That(grid.Rows[3].Cells[0].Selected, Is.False);
	}


	/// <summary>Inverting the column selection of a grid inverts the columns.</summary>
	[Test]
	public void GridInvertsColumns()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.FullColumnSelect);

		grid.Columns[0].Selected = true;
		grid.InvertSelection();

		Assert.That(grid.Columns[0].Selected, Is.False);
		Assert.That(grid.Columns[1].Selected, Is.True);
	}


	/// <summary>Single-selection grids are not changed.</summary>
	[Test]
	public void GridSingleSelectionIgnoresInvert()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.CellSelect);

		grid.MultiSelect = false;
		grid.Rows[0].Cells[0].Selected = true;
		grid.InvertSelection();

		Assert.That(grid.GetCellCount(DataGridViewElementStates.Selected), Is.EqualTo(1));
	}

	/// <summary>A right click on a cell outside the selection selects the cell alone and makes it the current cell.</summary>
	[Test]
	public void GridContextClickSelectsCell()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.RowHeaderSelect);

		grid.Rows[0].Cells[0].Selected = true;
		grid.Rows[0].Cells[1].Selected = true;

		Assert.That(grid.SelectForContextMenu(2, 1), Is.True);
		Assert.That(grid.CurrentCell, Is.SameAs(grid.Rows[2].Cells[1]));
		Assert.That(grid.SelectedCells.Cast<DataGridViewCell>(), Is.EqualTo(new[] { grid.Rows[2].Cells[1] }));
	}


	/// <summary>A right click on a selected cell keeps the selection.</summary>
	[Test]
	public void GridContextClickKeepsSelectedCells()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.RowHeaderSelect);

		grid.Rows[0].Cells[0].Selected = true;
		grid.Rows[1].Cells[0].Selected = true;

		Assert.That(grid.SelectForContextMenu(1, 0), Is.False);
		Assert.That(grid.GetCellCount(DataGridViewElementStates.Selected), Is.EqualTo(2));
	}


	/// <summary>A right click on the header of a row outside the selection selects the row.</summary>
	[Test]
	public void GridContextClickSelectsRow()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.RowHeaderSelect);

		grid.Rows[0].Cells[1].Selected = true;

		Assert.That(grid.SelectForContextMenu(1, -1), Is.True);
		Assert.That(grid.SelectedRows.Cast<DataGridViewRow>(), Is.EqualTo(new[] { grid.Rows[1] }));
		Assert.That(grid.Rows[0].Cells[1].Selected, Is.False);
		Assert.That(grid.CurrentCell?.RowIndex, Is.EqualTo(1));
	}


	/// <summary>A right click on the header of a selected row keeps the selected rows.</summary>
	[Test]
	public void GridContextClickKeepsSelectedRows()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.RowHeaderSelect);

		grid.Rows[0].Selected = true;
		grid.Rows[2].Selected = true;

		Assert.That(grid.SelectForContextMenu(2, -1), Is.False);
		Assert.That(grid.SelectedRows.Count, Is.EqualTo(2));
	}


	/// <summary>In cell selection mode, a right click on a row header selects the cells of the row; a row whose cells are all selected is
	///          kept.</summary>
	[Test]
	public void GridContextClickSelectsRowCells()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.CellSelect);

		Assert.That(grid.SelectForContextMenu(1, -1), Is.True);
		Assert.That(grid.Rows[1].Cells[0].Selected && grid.Rows[1].Cells[1].Selected, Is.True);
		Assert.That(grid.GetCellCount(DataGridViewElementStates.Selected), Is.EqualTo(2));

		Assert.That(grid.SelectForContextMenu(1, -1), Is.False);
	}


	/// <summary>Right clicks on column headers are ignored.</summary>
	[Test]
	public void GridContextClickIgnoresColumnHeaders()
	{
		using DataGridView grid = _CreateGrid(DataGridViewSelectionMode.RowHeaderSelect);

		grid.Rows[0].Cells[0].Selected = true;

		Assert.That(grid.SelectForContextMenu(-1, 1), Is.False);
		Assert.That(grid.SelectForContextMenu(-1, -1), Is.False);
		Assert.That(grid.Rows[0].Cells[0].Selected, Is.True);
	}
}
