namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

using System.Data;
using System.Runtime.Versioning;



/// <summary>This class implements the main window of the sample application.</summary>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 2000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		_InitializeText();
		_InitializeLists();
		_InitializeGrid();

		SampleLog.Written += SampleLog_Written;
		SampleLog.Attach(_Provider);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Initializes the controls of the text page.</summary>
	private void _InitializeText()
	{
		_ComboEditable.Items.AddRange(new object[] { "Vienna", "Graz", "Linz", "Salzburg", "Innsbruck" });
		_ComboEditable.Text = "Klagenfurt";

		_ComboList.Items.AddRange(new object[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" });
		_ComboList.SelectedIndex = 2;
	}


	/// <summary>Initializes the list view and list boxes.</summary>
	private void _InitializeLists()
	{
		string[][] fruits = new string[][]
		{
			new string[] { "Apple", "Red", "0.80" },
			new string[] { "Banana", "Yellow", "0.35" },
			new string[] { "Cherry", "Dark red", "4.20" },
			new string[] { "Grape", "Green", "2.90" },
			new string[] { "Kiwi", "Brown", "0.50" },
			new string[] { "Lemon", "Yellow", "0.45" },
			new string[] { "Orange", "Orange", "0.60" },
			new string[] { "Plum", "Purple", "3.10" }
		};
		string[] cities = new string[] { "Amsterdam", "Berlin", "Copenhagen", "Dublin", "Edinburgh", "Florence", "Geneva", "Helsinki" };

		foreach(string[] i in fruits) { _ListView.Items.Add(new ListViewItem(i)); }

		_ListMulti.Items.AddRange(cities);
		_ListSingle.Items.AddRange(cities);
	}


	/// <summary>Initializes the data grid view.</summary>
	private void _InitializeGrid()
	{
		DataTable table = new("Orders");
		DataGridViewComboBoxColumn category = new();

		table.Columns.Add("Product", typeof(string));
		table.Columns.Add("Quantity", typeof(int));
		table.Columns.Add("Category", typeof(string));
		table.Columns.Add("Created", typeof(DateTime));

		table.Rows.Add("Notebook", 3, "Office", new DateTime(2026, 9, 1, 9, 15, 0));
		table.Rows.Add("Pencil", 25, "Office", new DateTime(2026, 9, 2, 14, 30, 0));
		table.Rows.Add("Coffee", 2, "Kitchen", new DateTime(2026, 9, 3, 8, 0, 0));
		table.Rows.Add("Monitor", 1, "Hardware", new DateTime(2026, 9, 5, 11, 45, 0));
		table.Rows.Add("Keyboard", 2, "Hardware", new DateTime(2026, 9, 8, 16, 20, 0));
		table.Rows.Add("Tea", 4, "Kitchen", new DateTime(2026, 9, 9, 10, 5, 0));

		_Grid.AutoGenerateColumns = false;
		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "_ColumnProduct", HeaderText = "Product", DataPropertyName = "Product", Width = 200 });
		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "_ColumnQuantity", HeaderText = "Quantity (int)", DataPropertyName = "Quantity", Width = 110 });

		category.Name = "_ColumnCategory";
		category.HeaderText = "Category";
		category.DataPropertyName = "Category";
		category.Width = 150;
		category.Items.AddRange(new object[] { "Office", "Kitchen", "Hardware" });
		_Grid.Columns.Add(category);

		_Grid.Columns.Add(new DataGridViewTextBoxColumn() { Name = "_ColumnCreated", HeaderText = "Created (read-only)", DataPropertyName = "Created", ReadOnly = true, Width = 160 });

		_Grid.DataSource = table;
	}


	/// <summary>Returns if a control is one of the lists whose Delete command is implemented by the application.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the application handles Delete for the control, otherwise returns <c>false</c>.</returns>
	private bool _IsAppDeleteList(Control? control)
	{
		return (_ChkHandleDelete.Checked && ((control == _ListView) || (control == _ListMulti) || (control == _ListSingle)));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Querying event of the edit command provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Enables Delete for list views and list boxes when the application implements it.</remarks>
	private void _Provider_Querying(object? sender, EditCommandQueryEventArgs e)
	{
		if((e.Command != EditCommand.DELETE) || (!_IsAppDeleteList(e.Control))) { return; }

		e.CanExecute = e.Control switch
		{
			ListView list => (list.SelectedIndices.Count > 0),
			ListBox list => (list.SelectedIndices.Count > 0),
			_ => e.CanExecute
		};
	}


	/// <summary>Handles the Executing event of the edit command provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Implements Delete for list views and list boxes.</remarks>
	private void _Provider_Executing(object? sender, EditCommandExecuteEventArgs e)
	{
		int count = 0;

		if((e.Command != EditCommand.DELETE) || (!_IsAppDeleteList(e.Control))) { return; }

		if(e.Control is ListView view)
		{
			foreach(ListViewItem i in view.SelectedItems.Cast<ListViewItem>().ToList())
			{
				view.Items.Remove(i);
				count++;
			}
		}
		else if(e.Control is ListBox box)
		{
			foreach(int i in box.SelectedIndices.Cast<int>().OrderByDescending(m => m).ToList())
			{
				box.Items.RemoveAt(i);
				count++;
			}
		}

		e.Handled = true;
		SampleLog.Write($"Handled     {e.Control?.Name}: application removed {count} item(s)");
	}


	/// <summary>Handles the EditStateChanged event of the edit command provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_EditStateChanged(object? sender, EventArgs e)
	{
		Control? target = _Provider.ActiveControl;

		_LabelTarget.Text = "Target: " + ((target is null) ? "(none)" : $"{target.Name} ({target.GetType().Name})");
		_LabelState.Text = "Available: " + SampleLog.GetAvailableCommands(_Provider);
	}


	/// <summary>Handles the MouseDown event of the list views and list boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Focuses the list on a right click, so the context menu applies to it.</remarks>
	private void _List_MouseDown(object? sender, MouseEventArgs e)
	{
		if((e.Button == MouseButtons.Right) && (sender is Control control) && (!control.Focused)) { control.Focus(); }
	}


	/// <summary>Handles the UserDeletingRow event of the data grid view.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Asks before a row is deleted, if the confirmation check box is checked. The event is raised for the Delete key as well
	///          as for the Delete command of the provider.</remarks>
	private void _Grid_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
	{
		if(!_ChkConfirmDelete.Checked) { return; }

		e.Cancel = (MessageBox.Show(this, $"Delete the row '{e.Row?.Cells[0].Value}'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes);
		SampleLog.Write($"Deleting    _Grid: row '{e.Row?.Cells[0].Value}' " + (e.Cancel ? "kept" : "confirmed"));
	}


	/// <summary>Handles the CellMouseDown event of the data grid view.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>A right click selects the clicked cell or row for the context menu, unless it is selected already.</remarks>
	private void _Grid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
	{
		if((e.Button == MouseButtons.Right) && _Grid.SelectForContextMenu(e.RowIndex, e.ColumnIndex))
		{
			SampleLog.Write($"Context     _Grid: selected row {e.RowIndex}, column {e.ColumnIndex}");
		}
	}


	/// <summary>Handles the DataError event of the data grid view.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Grid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
	{
		SampleLog.Write($"DataError   _Grid: row {e.RowIndex}, column {e.ColumnIndex}: {e.Exception?.Message}");
		e.ThrowException = false;
	}


	/// <summary>Handles the Click event of the add note button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAddNote_Click(object? sender, EventArgs e)
	{
		if(string.IsNullOrWhiteSpace(_TextNewNote.Text)) { return; }

		_NoteBoard.AddNote(_TextNewNote.Text.Trim());
		_TextNewNote.Clear();
		_TextNewNote.Focus();
	}


	/// <summary>Handles the CheckedChanged event of the upper-case paste check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkUpperCasePaste_CheckedChanged(object? sender, EventArgs e)
	{
		_Pad.UpperCasePaste = _ChkUpperCasePaste.Checked;
	}


	/// <summary>Handles the Click event of the exit menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileExit_Click(object? sender, EventArgs e)
	{
		Close();
	}


	/// <summary>Handles the CheckedChanged event of the automatic update menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuOptionsAutoUpdate_CheckedChanged(object? sender, EventArgs e)
	{
		_Provider.AutoUpdate = _MenuOptionsAutoUpdate.Checked;
		SampleLog.Write($"Option      AutoUpdate = {_Provider.AutoUpdate}");
	}


	/// <summary>Handles the Click event of the update now menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuOptionsUpdateNow_Click(object? sender, EventArgs e)
	{
		_Provider.UpdateItems();
		SampleLog.Write("Option      UpdateItems()");
	}


	/// <summary>Handles the Written event of the sample log.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Log entry.</param>
	private void SampleLog_Written(object? sender, string e)
	{
		if(IsDisposed) { return; }

		_ListLog.BeginUpdate();
		_ListLog.Items.Add(e);
		while(_ListLog.Items.Count > _MAX_LOG_ENTRIES) { _ListLog.Items.RemoveAt(0); }
		_ListLog.TopIndex = _ListLog.Items.Count - 1;
		_ListLog.EndUpdate();
	}


	/// <summary>Handles the Click event of the copy log button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCopyLog_Click(object? sender, EventArgs e)
	{
		if(_ListLog.Items.Count == 0) { return; }
		Clipboard.SetText(string.Join(Environment.NewLine, _ListLog.Items.Cast<object>()));
	}


	/// <summary>Handles the Click event of the clear log button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonClearLog_Click(object? sender, EventArgs e)
	{
		_ListLog.Items.Clear();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		SampleLog.Written -= SampleLog_Written;
		base.OnFormClosed(e);
	}
}
