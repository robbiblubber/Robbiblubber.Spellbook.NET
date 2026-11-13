namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="ListBoxEditAdapter"/> and <see cref="ListViewEditAdapter"/> classes.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class ListEditAdapterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List box adapter.</summary>
	private readonly ListBoxEditAdapter _BoxAdapter = new();

	/// <summary>List view adapter.</summary>
	private readonly ListViewEditAdapter _ViewAdapter = new();

	/// <summary>List box.</summary>
	private ListBox _Box = null!;

	/// <summary>List view.</summary>
	private ListView _View = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Box = new ListBox() { SelectionMode = SelectionMode.MultiExtended };
		_Box.Items.AddRange(new object[] { "a", "b", "c", "d" });
		_ = _Box.Handle;

		_View = new ListView() { View = View.Details, MultiSelect = true };
		_View.Columns.Add("Name");
		_View.Columns.Add("Value");
		_View.Items.Add(new ListViewItem(new string[] { "a", "1" }));
		_View.Items.Add(new ListViewItem(new string[] { "b", "2" }));
		_View.Items.Add(new ListViewItem(new string[] { "c", "3" }));
		_ = _View.Handle;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Box.Dispose();
		_View.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List boxes do not support commands that change the items.</summary>
	[Test]
	public void ListBoxDoesNotChangeItems()
	{
		_Box.SetSelected(1, true);

		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.CUT), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.PASTE), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.DELETE), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.UNDO), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.COPY), Is.True);
	}


	/// <summary>Copy and clear selection need a selection.</summary>
	[Test]
	public void ListBoxCopyNeedsSelection()
	{
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.COPY), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.CLEAR_SELECTION), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.SELECT_ALL), Is.True);
	}


	/// <summary>Select all, invert and clear selection work for multi-select list boxes.</summary>
	[Test]
	public void ListBoxSelectionCommands()
	{
		_BoxAdapter.Execute(_Box, EditCommand.SELECT_ALL);
		Assert.That(_Box.SelectedIndices.Count, Is.EqualTo(4));

		_Box.SetSelected(0, false);
		_BoxAdapter.Execute(_Box, EditCommand.INVERT_SELECTION);
		Assert.That(_Box.SelectedIndices.Cast<int>(), Is.EqualTo(new int[] { 0 }));

		_BoxAdapter.Execute(_Box, EditCommand.CLEAR_SELECTION);
		Assert.That(_Box.SelectedIndices.Count, Is.EqualTo(0));
	}


	/// <summary>Single-selection list boxes do not select all or invert.</summary>
	[Test]
	public void SingleSelectionListBox()
	{
		_Box.SelectionMode = SelectionMode.One;
		_Box.SelectedIndex = 2;

		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.SELECT_ALL), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.INVERT_SELECTION), Is.False);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.COPY), Is.True);
		Assert.That(_BoxAdapter.CanExecute(_Box, EditCommand.CLEAR_SELECTION), Is.True);
	}


	/// <summary>List views support copy and selection commands.</summary>
	[Test]
	public void ListViewSelectionCommands()
	{
		Assert.That(_ViewAdapter.CanExecute(_View, EditCommand.COPY), Is.False);
		Assert.That(_ViewAdapter.CanExecute(_View, EditCommand.DELETE), Is.False);

		_ViewAdapter.Execute(_View, EditCommand.SELECT_ALL);
		Assert.That(_View.SelectedIndices.Count, Is.EqualTo(3));
		Assert.That(_ViewAdapter.CanExecute(_View, EditCommand.COPY), Is.True);

		_View.Items[1].Selected = false;
		_ViewAdapter.Execute(_View, EditCommand.INVERT_SELECTION);
		Assert.That(_View.SelectedIndices.Cast<int>(), Is.EqualTo(new int[] { 1 }));

		_ViewAdapter.Execute(_View, EditCommand.CLEAR_SELECTION);
		Assert.That(_View.SelectedIndices.Count, Is.EqualTo(0));
	}


	/// <summary>Single-selection list views do not select all.</summary>
	[Test]
	public void SingleSelectionListView()
	{
		_View.MultiSelect = false;

		Assert.That(_ViewAdapter.CanExecute(_View, EditCommand.SELECT_ALL), Is.False);
		Assert.That(_ViewAdapter.CanExecute(_View, EditCommand.INVERT_SELECTION), Is.False);
	}


	/// <summary>The text of a list view item contains its sub-items, separated by tabs.</summary>
	[Test]
	public void ListViewItemTextContainsSubItems()
	{
		Assert.That(ListViewEditAdapter._GetItemText(_View.Items[1]), Is.EqualTo("b\t2"));
	}


	/// <summary>Copy copies the selected items, one per line.</summary>
	[Test][Category("Clipboard")]
	public void CopyCopiesSelectedItems()
	{
		_Box.SetSelected(1, true);
		_Box.SetSelected(3, true);
		_BoxAdapter.Execute(_Box, EditCommand.COPY);
		Assert.That(Clipboard.GetText(), Is.EqualTo("b" + Environment.NewLine + "d"));

		_View.Items[0].Selected = true;
		_View.Items[2].Selected = true;
		_ViewAdapter.Execute(_View, EditCommand.COPY);
		Assert.That(Clipboard.GetText(), Is.EqualTo("a\t1" + Environment.NewLine + "c\t3"));
	}
}
