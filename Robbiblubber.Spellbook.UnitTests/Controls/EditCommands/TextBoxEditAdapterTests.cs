namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="TextBoxEditAdapter"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class TextBoxEditAdapterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adapter.</summary>
	private readonly TextBoxEditAdapter _Adapter = new();

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Text = new TextBox() { Multiline = true, Text = "Hello world" };
		_ = _Text.Handle;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Text.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A selection can be inverted when it is empty or touches the start or the end of the text.</summary>
	[Test]
	public void CanInvertForSingleRanges()
	{
		Assert.That(TextBoxEditAdapter._CanInvert(3, 0, 10), Is.True);
		Assert.That(TextBoxEditAdapter._CanInvert(0, 4, 10), Is.True);
		Assert.That(TextBoxEditAdapter._CanInvert(6, 4, 10), Is.True);
		Assert.That(TextBoxEditAdapter._CanInvert(0, 10, 10), Is.True);
		Assert.That(TextBoxEditAdapter._CanInvert(3, 4, 10), Is.False);
		Assert.That(TextBoxEditAdapter._CanInvert(0, 0, 0), Is.False);
	}


	/// <summary>The inverted selection covers the rest of the text.</summary>
	[Test]
	public void InvertsSelection()
	{
		Assert.That(TextBoxEditAdapter._Invert(3, 0, 10), Is.EqualTo((0, 10)));
		Assert.That(TextBoxEditAdapter._Invert(0, 10, 10), Is.EqualTo((10, 0)));
		Assert.That(TextBoxEditAdapter._Invert(0, 4, 10), Is.EqualTo((4, 6)));
		Assert.That(TextBoxEditAdapter._Invert(6, 4, 10), Is.EqualTo((0, 6)));
	}


	/// <summary>A line break is deleted in one step.</summary>
	[Test]
	public void DeleteLengthCoversLineBreak()
	{
		Assert.That(TextBoxEditAdapter._GetDeleteLength("a\r\nb", 1), Is.EqualTo(2));
		Assert.That(TextBoxEditAdapter._GetDeleteLength("a\r\nb", 0), Is.EqualTo(1));
		Assert.That(TextBoxEditAdapter._GetDeleteLength("a\r", 1), Is.EqualTo(1));
	}


	/// <summary>Cut and copy need a selection.</summary>
	[Test]
	public void CutAndCopyNeedSelection()
	{
		_Text.Select(0, 0);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CUT), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.COPY), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CLEAR_SELECTION), Is.False);

		_Text.Select(0, 5);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CUT), Is.True);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.COPY), Is.True);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CLEAR_SELECTION), Is.True);
	}


	/// <summary>Password text boxes do not cut or copy.</summary>
	[Test]
	public void PasswordBlocksCutAndCopy()
	{
		_Text.Multiline = false;
		_Text.UseSystemPasswordChar = true;
		_Text.Select(0, 5);

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CUT), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.COPY), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.DELETE), Is.True);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.SELECT_ALL), Is.True);
	}


	/// <summary>Read-only text boxes allow copying and selecting, but no changes.</summary>
	[Test]
	public void ReadOnlyAllowsCopyOnly()
	{
		_Text.ReadOnly = true;
		_Text.Select(0, 5);

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.COPY), Is.True);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.SELECT_ALL), Is.True);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CUT), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.PASTE), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.DELETE), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.UNDO), Is.False);
	}


	/// <summary>Commands that cannot be executed are ignored.</summary>
	[Test]
	public void ExecuteIgnoresUnavailableCommand()
	{
		_Text.ReadOnly = true;
		_Text.Select(0, 5);

		_Adapter.Execute(_Text, EditCommand.DELETE);

		Assert.That(_Text.Text, Is.EqualTo("Hello world"));
	}


	/// <summary>Disabled text boxes support no commands that change the text or the selection.</summary>
	[Test]
	public void DisabledBlocksCommands()
	{
		_Text.Enabled = false;
		_Text.Select(0, 5);

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.CUT), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.DELETE), Is.False);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.SELECT_ALL), Is.False);
	}


	/// <summary>Delete without a selection deletes the character after the caret.</summary>
	[Test]
	public void DeleteRemovesNextCharacter()
	{
		_Text.Select(5, 0);
		_Adapter.Execute(_Text, EditCommand.DELETE);

		Assert.That(_Text.Text, Is.EqualTo("Helloworld"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(5));
	}


	/// <summary>Delete removes a line break in one step.</summary>
	[Test]
	public void DeleteRemovesLineBreak()
	{
		_Text.Text = "ab\r\ncd";
		_Text.Select(2, 0);
		_Adapter.Execute(_Text, EditCommand.DELETE);

		Assert.That(_Text.Text, Is.EqualTo("abcd"));
	}


	/// <summary>Delete removes the selection.</summary>
	[Test]
	public void DeleteRemovesSelection()
	{
		_Text.Select(0, 6);
		_Adapter.Execute(_Text, EditCommand.DELETE);

		Assert.That(_Text.Text, Is.EqualTo("world"));
	}


	/// <summary>Delete is not available at the end of the text.</summary>
	[Test]
	public void DeleteUnavailableAtEnd()
	{
		_Text.Select(_Text.TextLength, 0);

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.DELETE), Is.False);
	}


	/// <summary>Deleting can be undone.</summary>
	[Test]
	public void DeleteCanBeUndone()
	{
		_Text.ClearUndo();
		_Text.Select(0, 6);
		_Adapter.Execute(_Text, EditCommand.DELETE);

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.UNDO), Is.True);

		_Adapter.Execute(_Text, EditCommand.UNDO);
		Assert.That(_Text.Text, Is.EqualTo("Hello world"));
	}


	/// <summary>Select all selects the whole text; it is not available for an empty text.</summary>
	[Test]
	public void SelectAllSelectsText()
	{
		_Adapter.Execute(_Text, EditCommand.SELECT_ALL);

		Assert.That(_Text.SelectionStart, Is.EqualTo(0));
		Assert.That(_Text.SelectionLength, Is.EqualTo(_Text.TextLength));

		_Text.Text = string.Empty;
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.SELECT_ALL), Is.False);
	}


	/// <summary>Invert selection selects the rest of the text.</summary>
	[Test]
	public void InvertSelectionSelectsRest()
	{
		_Text.Select(0, 6);
		_Adapter.Execute(_Text, EditCommand.INVERT_SELECTION);

		Assert.That(_Text.SelectedText, Is.EqualTo("world"));

		_Text.Select(2, 3);
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.INVERT_SELECTION), Is.False);
	}


	/// <summary>Clear selection puts the caret at the end of the selection.</summary>
	[Test]
	public void ClearSelectionKeepsCaretAtEnd()
	{
		_Text.Select(2, 3);
		_Adapter.Execute(_Text, EditCommand.CLEAR_SELECTION);

		Assert.That(_Text.SelectionStart, Is.EqualTo(5));
		Assert.That(_Text.SelectionLength, Is.EqualTo(0));
	}


	/// <summary>Redo is only available for rich text boxes.</summary>
	[Test]
	public void RedoOnlyForRichTextBox()
	{
		using RichTextBox rich = new();
		_ = rich.Handle;

		rich.Text = "abc";
		rich.Select(3, 0);
		rich.SelectedText = " def";

		Assert.That(_Adapter.CanExecute(rich, EditCommand.REDO), Is.False);
		_Adapter.Execute(rich, EditCommand.UNDO);
		Assert.That(_Adapter.CanExecute(rich, EditCommand.REDO), Is.True);

		_Text.Select(0, 5);
		_Text.Paste("Bye");
		_Text.Undo();
		Assert.That(_Adapter.CanExecute(_Text, EditCommand.REDO), Is.False);
	}


	/// <summary>The adapter only handles text boxes.</summary>
	[Test]
	public void IgnoresOtherControls()
	{
		using Button button = new();

		Assert.That(_Adapter.CanExecute(button, EditCommand.COPY), Is.False);
		Assert.DoesNotThrow(() => _Adapter.Execute(button, EditCommand.COPY));
	}


	/// <summary>Cut, copy and paste use the clipboard.</summary>
	[Test][Category("Clipboard")]
	public void CutCopyPasteUseClipboard()
	{
		_Text.Select(0, 5);
		_Adapter.Execute(_Text, EditCommand.COPY);
		Assert.That(Clipboard.GetText(), Is.EqualTo("Hello"));

		_Text.Select(6, 5);
		_Adapter.Execute(_Text, EditCommand.CUT);
		Assert.That(Clipboard.GetText(), Is.EqualTo("world"));
		Assert.That(_Text.Text, Is.EqualTo("Hello "));

		Assert.That(_Adapter.CanExecute(_Text, EditCommand.PASTE), Is.True);
		_Adapter.Execute(_Text, EditCommand.PASTE);
		Assert.That(_Text.Text, Is.EqualTo("Hello world"));
	}
}
