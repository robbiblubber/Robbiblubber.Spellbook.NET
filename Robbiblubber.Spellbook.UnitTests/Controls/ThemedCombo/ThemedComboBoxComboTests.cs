namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the combo mode of the <see cref="ThemedComboBox"/> control without drop-down list.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxComboTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

		_Combo = new TestThemedComboBox();
		_Combo.Items.AddRange(new ListItem("Apple"), new ListItem("Banana"), new ListItem("Cherry"), "Blueberry");
		_Combo.DropDownStyle = ThemedComboBoxStyle.COMBO;

		_Combo.SelectedIndexChanged += (sender, e) => { _Events.Add("SelectedIndexChanged"); };
		_Combo.SelectedItemChanged += (sender, e) => { _Events.Add("SelectedItemChanged"); };
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

	/// <summary>Combo mode creates a text box; list mode removes it.</summary>
	[Test]
	public void SwitchesStyle()
	{
		Assert.That(_Combo._TextBox, Is.Not.Null);
		Assert.That(_Combo.Controls.Contains(_Combo._TextBox!), Is.True);

		_Combo.DropDownStyle = ThemedComboBoxStyle.LIST;
		Assert.That(_Combo._TextBox, Is.Null);
		Assert.That(_Combo.Controls.Count, Is.EqualTo(0));
	}


	/// <summary>Selecting an item sets the text of the text box; clearing the selection clears the text.</summary>
	[Test]
	public void ShowsSelectedItem()
	{
		_Combo.SelectedIndex = 2;
		Assert.That(_Combo._TextBox!.Text, Is.EqualTo("Cherry"));
		Assert.That(_Combo.Text, Is.EqualTo("Cherry"));

		_Combo.SelectedIndex = -1;
		Assert.That(_Combo._TextBox.Text, Is.Empty);
	}


	/// <summary>Text that matches an item selects it and takes the spelling of the item.</summary>
	[Test]
	public void SelectsMatchingText()
	{
		_Combo.Text = "banana";

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.Text, Is.EqualTo("Banana"));
		Assert.That(_Events, Is.EqualTo(new[] { "SelectedIndexChanged", "SelectedItemChanged" }));
	}


	/// <summary>Free text clears the selection and keeps the text.</summary>
	[Test]
	public void KeepsFreeText()
	{
		_Combo.SelectedIndex = 1;
		_Combo.Text = "Durian";

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.Null);
		Assert.That(_Combo.Text, Is.EqualTo("Durian"));
	}


	/// <summary>Without free text, text that matches no item returns to the text of the selected item.</summary>
	[Test]
	public void RejectsFreeText()
	{
		_Combo.AllowFreeText = false;
		_Combo.SelectedIndex = 1;
		_Combo.Text = "Durian";

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.Text, Is.EqualTo("Banana"));
	}


	/// <summary>Up and Down select the previous and next item and set the text.</summary>
	[Test]
	public void NavigatesItems()
	{
		_Combo.Text = "Banana";

		Assert.That(_Combo.PressKey(Keys.Down), Is.True);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(2));
		Assert.That(_Combo.Text, Is.EqualTo("Cherry"));

		_Combo.PressKey(Keys.Up);
		_Combo.PressKey(Keys.Up);
		Assert.That(_Combo.Text, Is.EqualTo("Apple"));

		Assert.That(_Combo.PressKey(Keys.Home), Is.False);
	}


	/// <summary>The image comes from the matching item, otherwise from ResolveImage, otherwise from DefaultImage.</summary>
	[Test]
	public void ResolvesImage()
	{
		using Bitmap itemImage = new(16, 16);
		using Bitmap defaultImage = new(16, 16);
		using Bitmap resolved = new(16, 16);
		List<ThemedComboBoxImageEventArgs> requests = new();

		_Combo.Items.Add(new ListItem("Durian", itemImage));
		_Combo.DefaultImage = defaultImage;
		_Combo.ResolveImage += (sender, e) =>
		{
			requests.Add(e);
			if(e.Text == "Kiwi") { e.Image = resolved; }
		};
		requests.Clear();

		_Combo.Text = "durian";
		Assert.That(_Combo._GetComboImage(), Is.SameAs(itemImage));
		Assert.That(requests, Is.Empty);

		_Combo.Text = "Kiwi";
		Assert.That(_Combo._GetComboImage(), Is.SameAs(resolved));
		Assert.That(requests.Last().Item, Is.Null);

		_Combo.Text = "Mango";
		Assert.That(_Combo._GetComboImage(), Is.SameAs(defaultImage));

		_Combo.Text = "Apple";
		Assert.That(_Combo._GetComboImage(), Is.SameAs(defaultImage));
		Assert.That(requests.Last().Item, Is.Not.Null);
	}


	/// <summary>List items without image get the image from ResolveImage (requested once per item) or DefaultImage.</summary>
	[Test]
	public void ResolvesItemImages()
	{
		using Bitmap defaultImage = new(16, 16);
		using Bitmap resolved = new(16, 16);
		int count = 0;

		_Combo.ResolveImage += (sender, e) =>
		{
			if(e.Item is not null) { count++; }
			if(e.Text == "Blueberry") { e.Image = resolved; }
		};
		_Combo.DefaultImage = defaultImage;

		IListItem blueberry = _Combo._GetViewsForTest()[3];
		IListItem apple = _Combo._GetViewsForTest()[0];

		Assert.That(_Combo._ResolveItemImageForTest(blueberry), Is.SameAs(resolved));
		Assert.That(_Combo._ResolveItemImageForTest(blueberry), Is.SameAs(resolved));
		Assert.That(_Combo._ResolveItemImageForTest(apple), Is.SameAs(defaultImage));
		Assert.That(count, Is.EqualTo(2));

		_Combo.Items.Add("Fig");
		_Combo._ResolveItemImageForTest(_Combo._GetViewsForTest()[3]);
		Assert.That(count, Is.EqualTo(3));
	}


	/// <summary>Ranking puts prefix matches first and keeps the item order for matches of the same quality.</summary>
	[Test]
	public void RanksMatches()
	{
		IListItem[] items = new IListItem[] { new ListItem("Apple"), new ListItem("Banana"), new ListItem("Mango"), new ListItem("Ananas") };

		Assert.That(ThemedComboBox._Rank(items, "an", ListMatchMode.CONTAINS, false), Is.EqualTo(new[] { 3, 1, 2 }));
		Assert.That(ThemedComboBox._Rank(items, "an", ListMatchMode.PREFIX, false), Is.EqualTo(new[] { 3 }));
		Assert.That(ThemedComboBox._Rank(items, string.Empty, ListMatchMode.FUZZY, false), Is.EqualTo(new[] { 0, 1, 2, 3 }));
		Assert.That(ThemedComboBox._FindBest(items, "ma", ListMatchMode.FUZZY, false), Is.EqualTo(2));
		Assert.That(ThemedComboBox._FindBest(items, string.Empty, ListMatchMode.FUZZY, false), Is.EqualTo(-1));
	}
}
