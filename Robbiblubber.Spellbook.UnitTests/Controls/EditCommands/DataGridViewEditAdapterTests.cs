namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="DataGridViewEditAdapter"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class DataGridViewEditAdapterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Item                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents an item of a bound grid.</summary>
	private sealed class _Item
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets the name.</summary>
		public string? Name
		{
			get; set;
		}


		/// <summary>Gets or sets the count.</summary>
		public int Count
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adapter.</summary>
	private readonly DataGridViewEditAdapter _Adapter = new();

	/// <summary>Form hosting the grid.</summary>
	private Form _Form = null!;

	/// <summary>Data grid view with the columns Name, Count (int) and Fixed (read-only).</summary>
	private DataGridView _Grid = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Form = new Form();
		_Grid = new DataGridView() { AllowUserToAddRows = true, AllowUserToDeleteRows = true, MultiSelect = true };
		_Form.Controls.Add(_Grid);

		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Name", HeaderText = "Name", ValueType = typeof(string) });
		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Count", HeaderText = "Count", ValueType = typeof(int) });
		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Fixed", HeaderText = "Fixed", ValueType = typeof(string), ReadOnly = true });
		_Grid.Rows.Add("a", 1, "x");
		_Grid.Rows.Add("b", 2, "y");
		_Grid.Rows.Add("c", 3, "z");

		_ = _Form.Handle;
		_ = _Grid.Handle;
		_Grid.ClearSelection();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Grid.Dispose();
		_Form.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Selects cells.</summary>
	/// <param name="cells">Row and column indices.</param>
	private void _Select(params (int Row, int Column)[] cells)
	{
		_Grid.ClearSelection();
		foreach((int Row, int Column) i in cells) { _Grid.Rows[i.Row].Cells[i.Column].Selected = true; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Clipboard text is split into rows and values; a trailing line break is ignored.</summary>
	[Test]
	public void SplitsText()
	{
		string[][] result = DataGridViewEditAdapter._SplitText("a\tb\r\nc\td\r\n");

		Assert.That(result.Length, Is.EqualTo(2));
		Assert.That(result[0], Is.EqualTo(new string[] { "a", "b" }));
		Assert.That(result[1], Is.EqualTo(new string[] { "c", "d" }));

		Assert.That(DataGridViewEditAdapter._SplitText("x\ry").Length, Is.EqualTo(2));
		Assert.That(DataGridViewEditAdapter._SplitText("single"), Is.EqualTo(new string[][] { new string[] { "single" } }));
	}


	/// <summary>The row header text of copied whole rows is removed.</summary>
	[Test]
	public void StripsRowHeader()
	{
		string[] headers = new string[] { "Name", "Count", "Fixed" };
		string[][] result = DataGridViewEditAdapter._StripHeaders(new string[][] { new string[] { "", "a", "1", "x" }, new string[] { "", "b", "2", "y" } }, headers);

		Assert.That(result[0], Is.EqualTo(new string[] { "a", "1", "x" }));
		Assert.That(result[1], Is.EqualTo(new string[] { "b", "2", "y" }));
	}


	/// <summary>An empty first value is kept if the line does not have the shape of a whole copied row.</summary>
	[Test]
	public void KeepsEmptyFirstValue()
	{
		string[] headers = new string[] { "Name", "Count", "Fixed" };
		string[][] result = DataGridViewEditAdapter._StripHeaders(new string[][] { new string[] { "", "1" } }, headers);

		Assert.That(result[0], Is.EqualTo(new string[] { "", "1" }));
	}


	/// <summary>A first line of column header texts is removed if values follow.</summary>
	[Test]
	public void StripsColumnHeaderLine()
	{
		string[] headers = new string[] { "Name", "Count", "Fixed" };

		string[][] result = DataGridViewEditAdapter._StripHeaders(new string[][] { new string[] { "", "Name", "Count", "Fixed" }, new string[] { "", "a", "1", "x" } }, headers);
		Assert.That(result.Length, Is.EqualTo(1));
		Assert.That(result[0], Is.EqualTo(new string[] { "a", "1", "x" }));

		result = DataGridViewEditAdapter._StripHeaders(new string[][] { new string[] { "Count" } }, headers);
		Assert.That(result[0], Is.EqualTo(new string[] { "Count" }));
	}


	/// <summary>With whole rows selected, paste starts at the first column of the topmost selected row.</summary>
	[Test][Category("Clipboard")]
	public void PastesWholeRow()
	{
		_Grid.CurrentCell = _Grid.Rows[0].Cells[2];
		_Grid.ClearSelection();
		_Grid.Rows[1].Selected = true;
		Clipboard.SetText("\tp\t7\tq\r\n");

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.PASTE), Is.True);
		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows[1].Cells[0].Value, Is.EqualTo("p"));
		Assert.That(_Grid.Rows[1].Cells[1].Value, Is.EqualTo(7));
		Assert.That(_Grid.Rows[1].Cells[2].Value, Is.EqualTo("y"));
		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.EqualTo("a"));
	}


	/// <summary>Copy and clear selection need selected cells.</summary>
	[Test]
	public void CopyNeedsSelection()
	{
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.COPY), Is.False);
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.CLEAR_SELECTION), Is.False);

		_Select((0, 0));
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.COPY), Is.True);
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.CLEAR_SELECTION), Is.True);
	}


	/// <summary>Copy is not available if the grid does not copy to the clipboard.</summary>
	[Test]
	public void CopyRespectsClipboardCopyMode()
	{
		_Select((0, 0));
		_Grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.COPY), Is.False);
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.CUT), Is.False);
	}


	/// <summary>Cut is not available if a read-only cell is selected.</summary>
	[Test]
	public void CutNeedsWritableCells()
	{
		_Select((0, 0), (0, 1));
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.CUT), Is.True);

		_Select((0, 0), (0, 2));
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.CUT), Is.False);
	}


	/// <summary>Delete clears the values of writable cells and keeps read-only cells.</summary>
	[Test]
	public void DeleteClearsCells()
	{
		_Select((1, 0), (1, 2));

		_Adapter.Execute(_Grid, EditCommand.DELETE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(4));
		Assert.That(_Grid.Rows[1].Cells[0].Value, Is.Null);
		Assert.That(_Grid.Rows[1].Cells[2].Value, Is.EqualTo("y"));
	}


	/// <summary>Delete removes selected rows.</summary>
	[Test]
	public void DeleteRemovesRows()
	{
		_Grid.Rows[0].Selected = true;
		_Grid.Rows[2].Selected = true;

		_Adapter.Execute(_Grid, EditCommand.DELETE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(2));
		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.EqualTo("b"));
	}


	/// <summary>Rows are deleted like with the Delete key: UserDeletingRow can cancel, UserDeletedRow is raised.</summary>
	[Test]
	public void DeleteRaisesUserEvents()
	{
		List<object?> deleted = new();

		_Grid.UserDeletingRow += (sender, e) => { if(Equals(e.Row!.Cells[0].Value, "a")) { e.Cancel = true; } };
		_Grid.UserDeletedRow += (sender, e) => { deleted.Add(e.Row!.Cells[0].Value); };
		_Grid.Rows[0].Selected = true;
		_Grid.Rows[2].Selected = true;

		_Adapter.Execute(_Grid, EditCommand.DELETE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(3));
		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.EqualTo("a"));
		Assert.That(_Grid.Rows[1].Cells[0].Value, Is.EqualTo("b"));
		Assert.That(deleted, Is.EqualTo(new object?[] { "c" }));
	}


	/// <summary>Delete clears cells instead of removing rows if the grid does not allow deleting rows.</summary>
	[Test]
	public void DeleteKeepsRowsIfNotAllowed()
	{
		_Grid.AllowUserToDeleteRows = false;
		_Grid.Rows[0].Selected = true;

		_Adapter.Execute(_Grid, EditCommand.DELETE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(4));
		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.Null);
		Assert.That(_Grid.Rows[0].Cells[2].Value, Is.EqualTo("x"));
	}


	/// <summary>Nothing can be deleted if only read-only cells are selected.</summary>
	[Test]
	public void DeleteNeedsWritableCells()
	{
		_Select((0, 2));

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.DELETE), Is.False);
	}


	/// <summary>Select all and invert selection need multiple selection; select all skips the row for new records.</summary>
	[Test]
	public void SelectAllNeedsMultiSelect()
	{
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.SELECT_ALL), Is.True);

		_Adapter.Execute(_Grid, EditCommand.SELECT_ALL);
		Assert.That(_Grid.GetCellCount(DataGridViewElementStates.Selected), Is.EqualTo(9));
		Assert.That(_Grid.Rows[_Grid.NewRowIndex].Cells.Cast<DataGridViewCell>().Any(m => m.Selected), Is.False);

		_Grid.MultiSelect = false;
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.SELECT_ALL), Is.False);
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.INVERT_SELECTION), Is.False);
	}


	/// <summary>The grid does not support undo and redo.</summary>
	[Test]
	public void NoUndo()
	{
		_Select((0, 0));

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.UNDO), Is.False);
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.REDO), Is.False);
	}


	/// <summary>Paste writes values starting at the current cell, skips read-only cells and ignores invalid values.</summary>
	[Test][Category("Clipboard")]
	public void PastesValues()
	{
		_Grid.CurrentCell = _Grid.Rows[0].Cells[0];
		Clipboard.SetText("p\t7\tq\r\nr\tabc\r\n");

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.PASTE), Is.True);
		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.EqualTo("p"));
		Assert.That(_Grid.Rows[0].Cells[1].Value, Is.EqualTo(7));
		Assert.That(_Grid.Rows[0].Cells[2].Value, Is.EqualTo("x"));
		Assert.That(_Grid.Rows[1].Cells[0].Value, Is.EqualTo("r"));
		Assert.That(_Grid.Rows[1].Cells[1].Value, Is.EqualTo(2));
		Assert.That(_Grid.Rows.Count, Is.EqualTo(4));
	}


	/// <summary>Paste starts at the top left cell of the selection, not at the current cell.</summary>
	/// <remarks>Three selected rows are not a multiple of two lines, so the text is pasted once.</remarks>
	[Test][Category("Clipboard")]
	public void PastesAtTopLeftOfSelection()
	{
		_Grid.CurrentCell = _Grid.Rows[1].Cells[1];
		_Select((1, 1), (2, 1), (0, 0), (0, 1), (1, 0), (2, 0));
		Clipboard.SetText("p\t7\r\nq\t8");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That((_Grid.Rows[0].Cells[0].Value, _Grid.Rows[0].Cells[1].Value), Is.EqualTo(("p", 7)));
		Assert.That((_Grid.Rows[1].Cells[0].Value, _Grid.Rows[1].Cells[1].Value), Is.EqualTo(("q", 8)));
		Assert.That((_Grid.Rows[2].Cells[0].Value, _Grid.Rows[2].Cells[1].Value), Is.EqualTo(("c", 3)));
	}


	/// <summary>A single value is written into all selected cells; read-only cells are skipped.</summary>
	[Test][Category("Clipboard")]
	public void PasteFillsSelection()
	{
		_Select((0, 0), (1, 0), (2, 0), (1, 2));
		Clipboard.SetText("z");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows[0].Cells[0].Value, Is.EqualTo("z"));
		Assert.That(_Grid.Rows[1].Cells[0].Value, Is.EqualTo("z"));
		Assert.That(_Grid.Rows[2].Cells[0].Value, Is.EqualTo("z"));
		Assert.That(_Grid.Rows[1].Cells[2].Value, Is.EqualTo("y"));
	}


	/// <summary>A copied row is pasted into every selected row.</summary>
	[Test][Category("Clipboard")]
	public void PasteRepeatsRow()
	{
		_Grid.Rows[0].Selected = true;
		_Grid.Rows[2].Selected = true;
		Clipboard.SetText("\tp\t7\tq\r\n");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That((_Grid.Rows[0].Cells[0].Value, _Grid.Rows[0].Cells[1].Value, _Grid.Rows[0].Cells[2].Value), Is.EqualTo(("p", 7, "x")));
		Assert.That((_Grid.Rows[1].Cells[0].Value, _Grid.Rows[1].Cells[1].Value), Is.EqualTo(("b", 2)));
		Assert.That((_Grid.Rows[2].Cells[0].Value, _Grid.Rows[2].Cells[1].Value, _Grid.Rows[2].Cells[2].Value), Is.EqualTo(("p", 7, "z")));
		Assert.That(_Grid.Rows.Count, Is.EqualTo(4));
	}


	/// <summary>Two copied lines are repeated for four selected rows; a selection that is not a multiple is pasted once.</summary>
	[Test][Category("Clipboard")]
	public void PasteRepeatsLinesOnlyForMultiples()
	{
		_Grid.Rows.Add("d", 4, "w");
		_Select((0, 0), (1, 0), (2, 0), (3, 0));
		Clipboard.SetText("u\r\nv");

		_Adapter.Execute(_Grid, EditCommand.PASTE);
		Assert.That(Enumerable.Range(0, 4).Select(m => _Grid.Rows[m].Cells[0].Value), Is.EqualTo(new object[] { "u", "v", "u", "v" }));

		_Select((0, 0), (1, 0), (2, 0));
		Clipboard.SetText("s\r\nt");

		_Adapter.Execute(_Grid, EditCommand.PASTE);
		Assert.That(Enumerable.Range(0, 4).Select(m => _Grid.Rows[m].Cells[0].Value), Is.EqualTo(new object[] { "s", "t", "u", "v" }));
	}


	/// <summary>A copied column is pasted into every selected column.</summary>
	[Test][Category("Clipboard")]
	public void PasteRepeatsColumn()
	{
		_Select((0, 0), (0, 1), (1, 0), (1, 1));
		Clipboard.SetText("5\r\n6");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That((_Grid.Rows[0].Cells[0].Value, _Grid.Rows[0].Cells[1].Value), Is.EqualTo(("5", 5)));
		Assert.That((_Grid.Rows[1].Cells[0].Value, _Grid.Rows[1].Cells[1].Value), Is.EqualTo(("6", 6)));
	}


	/// <summary>Paste adds rows when the text reaches the row for new records.</summary>
	[Test][Category("Clipboard")]
	public void PasteAddsRows()
	{
		_Grid.CurrentCell = _Grid.Rows[2].Cells[0];
		Clipboard.SetText("u\r\nv\t8\r\nw");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(6));
		Assert.That(_Grid.Rows[2].Cells[0].Value, Is.EqualTo("u"));
		Assert.That(_Grid.Rows[3].Cells[0].Value, Is.EqualTo("v"));
		Assert.That(_Grid.Rows[3].Cells[1].Value, Is.EqualTo(8));
		Assert.That(_Grid.Rows[4].Cells[0].Value, Is.EqualTo("w"));
		Assert.That(_Grid.Rows[5].IsNewRow, Is.True);
	}


	/// <summary>Paste into the row for new records appends rows; the current cell moves to the first pasted cell.</summary>
	[Test][Category("Clipboard")]
	public void PastesIntoNewRow()
	{
		_Grid.CurrentCell = _Grid.Rows[_Grid.NewRowIndex].Cells[0];
		Clipboard.SetText("p\t7\r\nq\t9\r\n");

		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.PASTE), Is.True);
		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows.Count, Is.EqualTo(6));
		Assert.That(_Grid.Rows[3].Cells[0].Value, Is.EqualTo("p"));
		Assert.That(_Grid.Rows[3].Cells[1].Value, Is.EqualTo(7));
		Assert.That(_Grid.Rows[4].Cells[0].Value, Is.EqualTo("q"));
		Assert.That(_Grid.Rows[4].Cells[1].Value, Is.EqualTo(9));
		Assert.That(_Grid.CurrentCell?.RowIndex, Is.EqualTo(3));
	}


	/// <summary>Paste does not add rows if the grid does not allow adding rows.</summary>
	[Test][Category("Clipboard")]
	public void PasteDoesNotAddRowsIfNotAllowed()
	{
		_Grid.AllowUserToAddRows = false;
		_Grid.CurrentCell = _Grid.Rows[2].Cells[0];
		Clipboard.SetText("u\r\nv\r\nw");

		_Adapter.Execute(_Grid, EditCommand.PASTE);

		Assert.That(_Grid.Rows[2].Cells[0].Value, Is.EqualTo("u"));
		Assert.That(_Grid.Rows.Count, Is.EqualTo(3));
	}


	/// <summary>Paste into the row for new records is only available if rows can be added.</summary>
	[Test][Category("Clipboard")]
	public void PasteIntoNewRowNeedsAddRows()
	{
		Clipboard.SetText("p");
		_Grid.CurrentCell = _Grid.Rows[_Grid.NewRowIndex].Cells[0];
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.PASTE), Is.True);

		_Grid.ReadOnly = true;
		Assert.That(_Adapter.CanExecute(_Grid, EditCommand.PASTE), Is.False);
	}


	/// <summary>Paste adds items to the list of a bound grid.</summary>
	[Test][Category("Clipboard")]
	public void PasteAddsItemsToBoundList()
	{
		System.ComponentModel.BindingList<_Item> list = new() { new _Item() { Name = "a", Count = 1 } };
		using DataGridView grid = new() { AllowUserToAddRows = true, DataSource = list };
		_Form.Controls.Add(grid);
		_ = grid.Handle;

		grid.CurrentCell = grid.Rows[grid.NewRowIndex].Cells[0];
		Clipboard.SetText("b\t2\r\nc\t3");

		Assert.That(_Adapter.CanExecute(grid, EditCommand.PASTE), Is.True);
		_Adapter.Execute(grid, EditCommand.PASTE);

		Assert.That(list.Select(m => (m.Name, m.Count)), Is.EqualTo(new[] { ("a", 1), ("b", 2), ("c", 3) }));
		Assert.That(grid.Rows.Count, Is.EqualTo(4));
		Assert.That(grid.Rows[3].IsNewRow, Is.True);
	}


	/// <summary>Copy puts the selected cells on the clipboard.</summary>
	[Test][Category("Clipboard")]
	public void CopiesCells()
	{
		_Grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
		_Select((1, 0), (1, 1));

		_Adapter.Execute(_Grid, EditCommand.COPY);

		Assert.That(Clipboard.GetText().TrimEnd(), Is.EqualTo("b\t2"));
	}
}
