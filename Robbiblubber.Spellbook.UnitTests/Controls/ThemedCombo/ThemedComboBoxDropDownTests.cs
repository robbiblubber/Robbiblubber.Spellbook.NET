namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the drop-down list of the <see cref="ThemedComboBox"/> control.</summary>
/// <remarks>The tests show a window outside the visible screen area, because the list only opens for visible controls.</remarks>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxDropDownTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test window.</summary>
	private Form _Window = null!;

	/// <summary>Tested control.</summary>
	private TestThemedComboBox _Combo = null!;

	/// <summary>Raised events.</summary>
	private readonly List<string> _Events = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Events.Clear();

		_Window = new Form { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false };
		_Combo = new TestThemedComboBox();
		_Combo.Items.AddRange("Apple", "Banana", "Cherry", "Blueberry");
		_Combo.SelectedIndex = 1;

		_Combo.SelectionChangeCommitted += (sender, e) => { _Events.Add("SelectionChangeCommitted"); };
		_Combo.DropDown += (sender, e) => { _Events.Add("DropDown"); };
		_Combo.DropDownClosed += (sender, e) => { _Events.Add("DropDownClosed"); };

		_Window.Controls.Add(_Combo);
		_Window.Show();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Combo.Dispose();
		_Window.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>F4 opens the list with the selected item selected; Escape closes it without changing the selection.</summary>
	[Test]
	public void CancelsWithEscape()
	{
		Assert.That(_Combo.PressKey(Keys.F4), Is.True);
		Assert.That(_Combo.DroppedDown, Is.True);
		Assert.That(_Combo._DropDownList!.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo._DropDownList.ItemCount, Is.EqualTo(4));

		_Combo.PressKey(Keys.Down);
		Assert.That(_Combo._DropDownList.SelectedIndex, Is.EqualTo(2));
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));

		_Combo.PressKey(Keys.Escape);
		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Events, Is.EqualTo(new[] { "DropDown", "DropDownClosed" }));
	}


	/// <summary>Enter closes the list and selects the item selected in the list.</summary>
	[Test]
	public void CommitsWithEnter()
	{
		_Combo.PressKey(Keys.Alt | Keys.Down);
		_Combo.PressKey(Keys.End);
		_Combo.PressKey(Keys.Enter);

		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(3));
		Assert.That(_Events, Is.EqualTo(new[] { "DropDown", "SelectionChangeCommitted", "DropDownClosed" }));
	}


	/// <summary>F4 closes an open list and keeps the item selected in the list.</summary>
	[Test]
	public void CommitsWithF4()
	{
		_Combo.PressKey(Keys.F4);
		_Combo.PressKey(Keys.Home);
		_Combo.PressKey(Keys.F4);

		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));
	}


	/// <summary>Typing in the open list moves the list selection only.</summary>
	[Test]
	public void SearchesInList()
	{
		_Combo.DroppedDown = true;
		_Combo.Type("c");

		Assert.That(_Combo._DropDownList!.SelectedIndex, Is.EqualTo(2));
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
	}


	/// <summary>Items changed while the list is open are shown in the list.</summary>
	[Test]
	public void UpdatesOpenList()
	{
		_Combo.DroppedDown = true;
		_Combo.Items.Add("Date");

		Assert.That(_Combo._DropDownList!.ItemCount, Is.EqualTo(5));
	}


	/// <summary>The DropDown event can fill the items on demand.</summary>
	[Test]
	public void FillsItemsOnDropDown()
	{
		_Combo.DropDown += (sender, e) => { _Combo.Items.Add("Elderberry"); };
		_Combo.DroppedDown = true;

		Assert.That(_Combo._DropDownList!.ItemCount, Is.EqualTo(5));
	}


	/// <summary>Disabling the control or making it read-only closes the list.</summary>
	[Test]
	public void ClosesWhenDisabledOrReadOnly()
	{
		_Combo.DroppedDown = true;
		_Combo.Enabled = false;
		Assert.That(_Combo.DroppedDown, Is.False);

		_Combo.Enabled = true;
		_Combo.DroppedDown = true;
		_Combo.ReadOnly = true;
		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
	}


	/// <summary>The list is at least as wide as the control and as DropDownWidth.</summary>
	[Test]
	public void SizesList()
	{
		_Combo.Width = 150;
		_Combo.DroppedDown = true;
		Assert.That(_Combo._DropDownList!.Width, Is.EqualTo(150));
		_Combo.DroppedDown = false;

		_Combo.DropDownWidth = 300;
		_Combo.DroppedDown = true;
		Assert.That(_Combo._DropDownList.Width, Is.EqualTo(300));
	}
}
