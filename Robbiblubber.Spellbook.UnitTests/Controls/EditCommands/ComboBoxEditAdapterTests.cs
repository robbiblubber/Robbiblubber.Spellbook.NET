namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="ComboBoxEditAdapter"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class ComboBoxEditAdapterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adapter.</summary>
	private readonly ComboBoxEditAdapter _Adapter = new();

	/// <summary>Combo box.</summary>
	private ComboBox _Combo = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Combo = new ComboBox();
		_Combo.Items.AddRange(new object[] { "Monday", "Tuesday" });
		_ = _Combo.Handle;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Combo.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Only the first line of a text is pasted.</summary>
	[Test]
	public void FirstLineIsUsed()
	{
		Assert.That(ComboBoxEditAdapter._FirstLine("abc"), Is.EqualTo("abc"));
		Assert.That(ComboBoxEditAdapter._FirstLine("abc\r\ndef"), Is.EqualTo("abc"));
		Assert.That(ComboBoxEditAdapter._FirstLine("abc\ndef"), Is.EqualTo("abc"));
		Assert.That(ComboBoxEditAdapter._FirstLine("\ndef"), Is.Empty);
	}


	/// <summary>A drop-down list only supports copying the selected item.</summary>
	[Test]
	public void DropDownListOnlyCopies()
	{
		_Combo.DropDownStyle = ComboBoxStyle.DropDownList;
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.COPY), Is.False);

		_Combo.SelectedIndex = 1;

		foreach(EditCommand i in Enum.GetValues<EditCommand>())
		{
			Assert.That(_Adapter.CanExecute(_Combo, i), Is.EqualTo(i == EditCommand.COPY), i.ToString());
		}
	}


	/// <summary>Editable combo boxes work on the text; changes can be undone, redo is not supported.</summary>
	[Test]
	public void EditableWorksOnText()
	{
		_Combo.Text = "Wednesday";
		_Combo.Select(0, 3);

		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.CUT), Is.True);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.COPY), Is.True);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.DELETE), Is.True);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.UNDO), Is.False);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.REDO), Is.False);

		_Adapter.Execute(_Combo, EditCommand.DELETE);
		Assert.That(_Combo.Text, Is.EqualTo("nesday"));

		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.UNDO), Is.True);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.REDO), Is.False);
		_Adapter.Execute(_Combo, EditCommand.UNDO);
		Assert.That(_Combo.Text, Is.EqualTo("Wednesday"));
	}


	/// <summary>Select all and clear selection work on the text.</summary>
	[Test]
	public void SelectsText()
	{
		_Combo.Text = "Friday";

		_Adapter.Execute(_Combo, EditCommand.SELECT_ALL);
		Assert.That(_Combo.SelectionLength, Is.EqualTo(6));

		_Adapter.Execute(_Combo, EditCommand.CLEAR_SELECTION);
		Assert.That(_Combo.SelectionLength, Is.EqualTo(0));
		Assert.That(_Combo.SelectionStart, Is.EqualTo(6));
	}


	/// <summary>Disabled combo boxes support no commands.</summary>
	[Test]
	public void DisabledBlocksCommands()
	{
		_Combo.Text = "Friday";
		_Combo.Select(0, 3);
		_Combo.Enabled = false;

		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.CUT), Is.False);
		Assert.That(_Adapter.CanExecute(_Combo, EditCommand.SELECT_ALL), Is.False);
	}


	/// <summary>Paste inserts the first line of the clipboard text.</summary>
	[Test][Category("Clipboard")]
	public void PastesFirstLine()
	{
		Clipboard.SetText("Saturday\r\nSunday");
		_Combo.Text = string.Empty;

		_Adapter.Execute(_Combo, EditCommand.PASTE);

		Assert.That(_Combo.Text, Is.EqualTo("Saturday"));
	}
}
