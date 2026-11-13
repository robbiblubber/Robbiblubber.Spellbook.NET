namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedComboBox"/> control in list mode.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Tested control.</summary>
	private TestThemedComboBox _Combo = null!;

	/// <summary>Raised events.</summary>
	private readonly List<string> _Events = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds the sample items.</summary>
	private void _AddItems()
	{
		_Combo.Items.AddRange(new ListItem("Apple", "fruit") { Detail = "1.20" }, new ListItem("Banana"), new ListItem("Cherry") { FilterText = "Kirsche" }, "Blueberry", 42);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Events.Clear();

		_Combo = new TestThemedComboBox();
		_Combo.SelectedIndexChanged += (sender, e) => { _Events.Add("SelectedIndexChanged"); };
		_Combo.SelectedItemChanged += (sender, e) => { _Events.Add("SelectedItemChanged"); };
		_Combo.SelectionChangeCommitted += (sender, e) => { _Events.Add("SelectionChangeCommitted"); };
		_Combo.TextChanged += (sender, e) => { _Events.Add("TextChanged"); };
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

	/// <summary>The display text is the text of list items and the string representation of other objects.</summary>
	[Test]
	public void GetsItemText()
	{
		Assert.That(_Combo.GetItemText(new ListItem("Apple")), Is.EqualTo("Apple"));
		Assert.That(_Combo.GetItemText("Banana"), Is.EqualTo("Banana"));
		Assert.That(_Combo.GetItemText(42), Is.EqualTo("42"));
		Assert.That(_Combo.GetItemText(null), Is.EqualTo(string.Empty));
	}


	/// <summary>Setting the selected index updates item and text and raises the events, but not SelectionChangeCommitted.</summary>
	[Test]
	public void SelectsByIndex()
	{
		_AddItems();
		_Combo.SelectedIndex = 1;

		Assert.That(_Combo.SelectedItem, Is.SameAs(_Combo.Items[1]));
		Assert.That(_Combo.Text, Is.EqualTo("Banana"));
		Assert.That(_Events, Is.EqualTo(new[] { "TextChanged", "SelectedIndexChanged", "SelectedItemChanged" }));

		_Events.Clear();
		_Combo.SelectedIndex = 1;
		Assert.That(_Events, Is.Empty);

		_Combo.SelectedIndex = -1;
		Assert.That(_Combo.SelectedItem, Is.Null);
		Assert.That(_Combo.Text, Is.Empty);

		Assert.Throws<ArgumentOutOfRangeException>(() => _Combo.SelectedIndex = 5);
		Assert.Throws<ArgumentOutOfRangeException>(() => _Combo.SelectedIndex = -2);
	}


	/// <summary>Setting the text selects the first item with that text, ignoring case; an unknown text clears the selection.</summary>
	[Test]
	public void SelectsByText()
	{
		_AddItems();

		_Combo.Text = "blueberry";
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(3));
		Assert.That(_Combo.Text, Is.EqualTo("Blueberry"));

		_Combo.Text = "Durian";
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.Text, Is.Empty);
	}


	/// <summary>An item that is not in the list can be selected; the index is -1.</summary>
	[Test]
	public void SelectsItemOutsideList()
	{
		ListItem outside = new("Durian") { Detail = "rare" };

		_AddItems();
		_Combo.SelectedItem = outside;

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.SameAs(outside));
		Assert.That(_Combo.Text, Is.EqualTo("Durian"));

		_Combo.SelectedItem = 42;
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(4));
	}


	/// <summary>The selected index follows insertions and removals; removing the selected item clears the selection.</summary>
	[Test]
	public void KeepsSelectionOnListChanges()
	{
		_AddItems();
		_Combo.SelectedIndex = 2;
		object selected = _Combo.SelectedItem!;
		_Events.Clear();

		_Combo.Items.Insert(0, "Avocado");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(3));
		Assert.That(_Combo.SelectedItem, Is.SameAs(selected));

		_Combo.Items.RemoveAt(0);
		_Combo.Items.RemoveAt(0);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.SelectedItem, Is.SameAs(selected));
		Assert.That(_Events, Is.Empty);

		_Combo.Items.Add("Date");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));

		_Combo.Items.RemoveAt(1);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.Null);
		Assert.That(_Events, Is.EqualTo(new[] { "TextChanged", "SelectedIndexChanged", "SelectedItemChanged" }));
	}


	/// <summary>Replacing the selected item keeps the index and updates item and text.</summary>
	[Test]
	public void UpdatesReplacedItem()
	{
		_AddItems();
		_Combo.SelectedIndex = 1;
		_Events.Clear();

		_Combo.Items[1] = "Banana split";

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.SelectedItem, Is.EqualTo("Banana split"));
		Assert.That(_Combo.Text, Is.EqualTo("Banana split"));
		Assert.That(_Events, Is.EqualTo(new[] { "TextChanged", "SelectedItemChanged" }));
	}


	/// <summary>Clearing the items clears the selection; an item outside the list stays selected.</summary>
	[Test]
	public void ClearsSelectionOnClear()
	{
		ListItem outside = new("Durian");

		_AddItems();
		_Combo.SelectedIndex = 0;
		_Combo.Items.Clear();

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.Null);

		_AddItems();
		_Combo.SelectedItem = outside;
		_Combo.Items.Clear();
		Assert.That(_Combo.SelectedItem, Is.SameAs(outside));
	}


	/// <summary>NULL items are rejected; AddRange adds nothing if one of the items is NULL.</summary>
	[Test]
	public void RejectsNullItems()
	{
		Assert.Throws<ArgumentNullException>(() => _Combo.Items.Add(null!));
		Assert.Throws<ArgumentNullException>(() => _Combo.Items.AddRange("a", null!, "b"));
		Assert.Throws<ArgumentNullException>(() => _Combo.Items.AddRange((IEnumerable<object>) null!));
		Assert.That(_Combo.Items, Is.Empty);

		_Combo.Items.Add("a");
		Assert.Throws<ArgumentNullException>(() => _Combo.Items[0] = null!);
	}


	/// <summary>FindString and FindStringExact compare case-insensitive.</summary>
	[Test]
	public void FindsStrings()
	{
		_AddItems();

		Assert.That(_Combo.FindString("b"), Is.EqualTo(1));
		Assert.That(_Combo.FindString("BLU"), Is.EqualTo(3));
		Assert.That(_Combo.FindString("x"), Is.EqualTo(-1));
		Assert.That(_Combo.FindString(string.Empty), Is.EqualTo(-1));
		Assert.That(_Combo.FindStringExact("banana"), Is.EqualTo(1));
		Assert.That(_Combo.FindStringExact("Ban"), Is.EqualTo(-1));
		Assert.That(_Combo.FindStringExact("42"), Is.EqualTo(4));
	}


	/// <summary>The prefix search uses the filter text, starts at the given index and wraps around.</summary>
	[Test]
	public void FindsPrefix()
	{
		IListItem[] items = new IListItem[] { new ListItem("Apple"), new ListItem("Banana"), new ListItem("Cherry") { FilterText = "Kirsche" }, new ListItem("Blueberry") };

		Assert.That(ThemedComboBox._FindPrefix(items, "b", 0), Is.EqualTo(1));
		Assert.That(ThemedComboBox._FindPrefix(items, "b", 2), Is.EqualTo(3));
		Assert.That(ThemedComboBox._FindPrefix(items, "b", 4), Is.EqualTo(1));
		Assert.That(ThemedComboBox._FindPrefix(items, "ki", 0), Is.EqualTo(2));
		Assert.That(ThemedComboBox._FindPrefix(items, "ch", 0), Is.EqualTo(-1));
		Assert.That(ThemedComboBox._FindPrefix(items, string.Empty, 0), Is.EqualTo(-1));
		Assert.That(ThemedComboBox._FindPrefix(Array.Empty<IListItem>(), "a", 0), Is.EqualTo(-1));
	}


	/// <summary>The arrow keys, Home, End and the page keys change the selection and raise SelectionChangeCommitted.</summary>
	[Test]
	public void NavigatesWithKeys()
	{
		_AddItems();

		Assert.That(_Combo.PressKey(Keys.Down), Is.True);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));
		Assert.That(_Events, Does.Contain("SelectionChangeCommitted"));

		_Combo.PressKey(Keys.Down);
		_Combo.PressKey(Keys.Right);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(2));

		_Combo.PressKey(Keys.Up);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));

		_Combo.PressKey(Keys.End);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(4));

		_Combo.PressKey(Keys.Down);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(4));

		_Combo.PressKey(Keys.Home);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));

		_Combo.PressKey(Keys.PageDown);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(4));

		_Combo.PressKey(Keys.PageUp);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));

		Assert.That(_Combo.PressKey(Keys.A), Is.False);
	}


	/// <summary>Typing selects the next matching item; repeating the first character cycles through the matches.</summary>
	[Test]
	public void SearchesTypedText()
	{
		_AddItems();

		_Combo.Type("b");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Events, Does.Contain("SelectionChangeCommitted"));

		_Combo.Type("b");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(3));

		_Combo.Type("b");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
	}


	/// <summary>Printable characters are input characters, so the form does not process them as mnemonics.</summary>
	[Test]
	public void AcceptsTypedCharacters()
	{
		Assert.That(_Combo.AcceptsChar('b'), Is.True);
		Assert.That(_Combo.AcceptsChar('Ä'), Is.True);
		Assert.That(_Combo.AcceptsChar(' '), Is.True);
		Assert.That(_Combo.AcceptsChar('\t'), Is.False);
	}


	/// <summary>Typing several characters selects the first item that starts with them.</summary>
	[Test]
	public void SearchesPrefix()
	{
		_AddItems();

		_Combo.Type("blu");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(3));

		_Combo.Text = string.Empty;
		_Combo.Type("x");
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
	}


	/// <summary>A read-only control does not change the selection with the keyboard and does not open the list.</summary>
	[Test]
	public void IgnoresInputWhenReadOnly()
	{
		_AddItems();
		_Combo.SelectedIndex = 1;
		_Combo.ReadOnly = true;
		_Events.Clear();

		_Combo.PressKey(Keys.Down);
		_Combo.Type("c");
		_Combo.PressKey(Keys.F4);
		_Combo.DroppedDown = true;

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Events, Is.Empty);
	}


	/// <summary>The height is determined by the font and cannot be set.</summary>
	[Test]
	public void KeepsHeight()
	{
		int height = _Combo.Height;

		_Combo.Height = height + 40;
		Assert.That(_Combo.Height, Is.EqualTo(height));

		_Combo.Font = new Font(_Combo.Font.FontFamily, _Combo.Font.Size * 2);
		Assert.That(_Combo.Height, Is.GreaterThan(height));
	}


	/// <summary>The default focus style is a colored border; the values can be combined.</summary>
	[Test]
	public void DefaultsFocusStyle()
	{
		Assert.That(_Combo.FocusStyle, Is.EqualTo(ThemedComboBoxFocusStyle.BORDER));

		_Combo.FocusStyle = ThemedComboBoxFocusStyle.LINE | ThemedComboBoxFocusStyle.DOTTED;
		Assert.That(_Combo.FocusStyle.HasFlag(ThemedComboBoxFocusStyle.DOTTED), Is.True);
		Assert.That(_Combo.FocusStyle.HasFlag(ThemedComboBoxFocusStyle.BORDER), Is.False);

		_Combo.FocusStyle = ThemedComboBoxFocusStyle.NONE;
		Assert.That(_Combo.FocusStyle, Is.EqualTo(ThemedComboBoxFocusStyle.NONE));
	}


	/// <summary>BackColor and ForeColor are mapped to the color table.</summary>
	[Test]
	public void MapsColorsToTable()
	{
		int count = 0;

		_Combo.BackColorChanged += (sender, e) => { count++; };
		_Combo.BackColor = Color.Navy;

		Assert.That(_Combo.Colors.BackColor, Is.EqualTo(Color.Navy));
		Assert.That(_Combo.BackColor, Is.EqualTo(Color.Navy));
		Assert.That(count, Is.EqualTo(1));
		Assert.That(_Combo.ShouldSerializeColors(), Is.True);

		_Combo.ResetColors();
		Assert.That(_Combo.ShouldSerializeColors(), Is.False);
		Assert.That(_Combo.BackColor, Is.EqualTo(ThemeManager.Theme[ThemeColor.FIELD_BACK]));
	}


	/// <summary>Disposing the image list removes it from the control.</summary>
	[Test]
	public void ReleasesDisposedImageList()
	{
		ImageList images = new();

		_Combo.ImageList = images;
		images.Dispose();

		Assert.That(_Combo.ImageList, Is.Null);
	}
}
