namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

using System.Runtime.Versioning;



/// <summary>This class implements a note board that implements <see cref="IEditable"/> itself.</summary>
/// <remarks>The notes are shown in an inner list box. All commands, including undo and redo, are implemented by the control, so the
///          <see cref="EditCommandProvider"/> uses this implementation whenever the focus is inside the control.</remarks>
[SupportedOSPlatform("windows")]
public partial class NoteBoardControl: UserControl, IEditable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of undo steps.</summary>
	private const int _MAX_UNDO = 100;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Undo stack.</summary>
	private readonly LinkedList<string[]> _Undo = new();

	/// <summary>Redo stack.</summary>
	private readonly Stack<string[]> _Redo = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public NoteBoardControl()
	{
		InitializeComponent();
		_ListNotes.Items.AddRange(new object[] { "Buy milk", "Call Anna", "Water the plants", "Book flight to Lisbon" });
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the current notes.</summary>
	/// <returns>Returns the notes.</returns>
	private string[] _GetNotes()
	{
		return _ListNotes.Items.Cast<object>().Select(m => m.ToString() ?? string.Empty).ToArray();
	}


	/// <summary>Gets the texts of the selected notes.</summary>
	/// <returns>Returns the texts, one per line.</returns>
	private string _GetSelectedText()
	{
		return string.Join(Environment.NewLine, _ListNotes.SelectedItems.Cast<object>().Select(m => m.ToString()));
	}


	/// <summary>Saves the current notes for undo; clears the redo stack.</summary>
	private void _SaveUndo()
	{
		_Undo.AddLast(_GetNotes());
		while(_Undo.Count > _MAX_UNDO) { _Undo.RemoveFirst(); }
		_Redo.Clear();
	}


	/// <summary>Replaces the notes.</summary>
	/// <param name="notes">Notes.</param>
	/// <param name="selected">Indices of the notes to select.</param>
	private void _SetNotes(IEnumerable<string> notes, IEnumerable<int> selected)
	{
		_ListNotes.BeginUpdate();
		try
		{
			_ListNotes.Items.Clear();
			_ListNotes.Items.AddRange(notes.Cast<object>().ToArray());
			foreach(int i in selected.Where(m => (m >= 0) && (m < _ListNotes.Items.Count))) { _ListNotes.SetSelected(i, true); }
		}
		finally { _ListNotes.EndUpdate(); }

		_OnEditStateChanged(EventArgs.Empty);
	}


	/// <summary>Removes the selected notes.</summary>
	private void _RemoveSelected()
	{
		int first = _ListNotes.SelectedIndices.Cast<int>().Min();
		HashSet<int> selected = new(_ListNotes.SelectedIndices.Cast<int>());
		List<string> notes = _GetNotes().Where((m, i) => !selected.Contains(i)).ToList();

		_SaveUndo();
		_SetNotes(notes, new int[] { Math.Min(first, notes.Count - 1) });
	}


	/// <summary>Inserts the lines of the clipboard text after the selected notes.</summary>
	private void _Paste()
	{
		string[] lines = Clipboard.GetText().Split('\n').Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
		List<string> notes = _GetNotes().ToList();
		int index = ((_ListNotes.SelectedIndices.Count == 0) ? notes.Count : (_ListNotes.SelectedIndices.Cast<int>().Max() + 1));

		if(lines.Length == 0) { return; }

		_SaveUndo();
		notes.InsertRange(index, lines);
		_SetNotes(notes, Enumerable.Range(index, lines.Length));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="EditStateChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnEditStateChanged(EventArgs e)
	{
		EditStateChanged?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds a note.</summary>
	/// <param name="text">Text.</param>
	public void AddNote(string text)
	{
		List<string> notes = _GetNotes().ToList();

		_SaveUndo();
		notes.Add(text);
		_SetNotes(notes, new int[] { notes.Count - 1 });
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the SelectedIndexChanged event of the note list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListNotes_SelectedIndexChanged(object? sender, EventArgs e)
	{
		_OnEditStateChanged(EventArgs.Empty);
	}


	/// <summary>Handles the MouseDown event of the note list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListNotes_MouseDown(object? sender, MouseEventArgs e)
	{
		if((e.Button == MouseButtons.Right) && (!_ListNotes.Focused)) { _ListNotes.Focus(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditable                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public event EventHandler? EditStateChanged;


	/// <inheritdoc/>
	public bool CanExecute(EditCommand command)
	{
		bool selected = (_ListNotes.SelectedIndices.Count > 0);

		if(!Enabled) { return false; }

		return command switch
		{
			EditCommand.CUT => selected,
			EditCommand.COPY => selected,
			EditCommand.PASTE => Clipboard.ContainsText(),
			EditCommand.DELETE => selected,
			EditCommand.UNDO => (_Undo.Count > 0),
			EditCommand.REDO => (_Redo.Count > 0),
			EditCommand.SELECT_ALL => (_ListNotes.Items.Count > 0),
			EditCommand.INVERT_SELECTION => (_ListNotes.Items.Count > 0),
			EditCommand.CLEAR_SELECTION => selected,
			_ => false
		};
	}


	/// <inheritdoc/>
	public void Execute(EditCommand command)
	{
		string[] notes;

		if(!CanExecute(command)) { return; }

		switch(command)
		{
			case EditCommand.CUT:
				Clipboard.SetText(_GetSelectedText());
				_RemoveSelected();
				break;
			case EditCommand.COPY:
				Clipboard.SetText(_GetSelectedText());
				break;
			case EditCommand.PASTE:
				_Paste();
				break;
			case EditCommand.DELETE:
				_RemoveSelected();
				break;
			case EditCommand.UNDO:
				_Redo.Push(_GetNotes());
				notes = _Undo.Last!.Value;
				_Undo.RemoveLast();
				_SetNotes(notes, Array.Empty<int>());
				break;
			case EditCommand.REDO:
				_Undo.AddLast(_GetNotes());
				_SetNotes(_Redo.Pop(), Array.Empty<int>());
				break;
			case EditCommand.SELECT_ALL:
				_ListNotes.SelectAll();
				break;
			case EditCommand.INVERT_SELECTION:
				_ListNotes.InvertSelection();
				break;
			case EditCommand.CLEAR_SELECTION:
				_ListNotes.ClearSelection();
				break;
		}
	}
}
