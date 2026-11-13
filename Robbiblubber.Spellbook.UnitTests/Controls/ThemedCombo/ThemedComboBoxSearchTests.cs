namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the search of the <see cref="ThemedComboBox"/> control in combo mode.</summary>
/// <remarks>The tests show a window outside the visible screen area, because the list only opens for visible controls. Typing is
///          simulated by setting the text of the text box.</remarks>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxSearchTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test window.</summary>
	private Form _Window = null!;

	/// <summary>Tested control.</summary>
	private TestThemedComboBox _Combo = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the texts of the displayed search results.</summary>
	private string[] _Results
	{
		get { return _Combo._DisplayedSearchResults?.Select(m => m.Text).ToArray() ?? Array.Empty<string>(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Simulates typing a text.</summary>
	/// <param name="text">Text.</param>
	private void _Type(string text)
	{
		_Combo._TextBox!.Text = text;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Window = new Form { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false };
		_Combo = new TestThemedComboBox { DropDownStyle = ThemedComboBoxStyle.COMBO };
		_Combo.Items.AddRange("Apple", "Banana", "Cherry", "Blueberry", "Elderberry");

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

	/// <summary>Typing shows the matching items; Enter selects the first one.</summary>
	[Test]
	public void FiltersItems()
	{
		_Type("ber");

		Assert.That(_Combo.DroppedDown, Is.True);
		Assert.That(_Results, Is.EqualTo(new[] { "Blueberry", "Elderberry" }));

		_Combo.PressKey(Keys.Down);
		_Combo.PressKey(Keys.Enter);

		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(4));
		Assert.That(_Combo.Text, Is.EqualTo("Elderberry"));
	}


	/// <summary>Escape closes the list and keeps the typed text and the selection.</summary>
	[Test]
	public void KeepsTextOnEscape()
	{
		_Combo.SelectedIndex = 0;
		_Type("ber");
		_Combo.PressKey(Keys.Escape);

		Assert.That(_Combo.DroppedDown, Is.False);
		Assert.That(_Combo.Text, Is.EqualTo("ber"));
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));
	}


	/// <summary>Text that matches nothing closes the list.</summary>
	[Test]
	public void ClosesWithoutMatches()
	{
		_Type("b");
		Assert.That(_Combo.DroppedDown, Is.True);

		_Type("bxq");
		Assert.That(_Combo.DroppedDown, Is.False);
	}


	/// <summary>The list opens only from the minimum length on; Ctrl+Space opens it regardless.</summary>
	[Test]
	public void RespectsMinimumLength()
	{
		_Combo.MinimumSearchLength = 2;

		_Type("b");
		Assert.That(_Combo.DroppedDown, Is.False);

		_Combo.PressKey(Keys.Control | Keys.Space);
		Assert.That(_Combo.DroppedDown, Is.True);
		Assert.That(_Results, Has.Length.EqualTo(3));

		_Type(string.Empty);
		_Combo.PressKey(Keys.Control | Keys.Space);
		Assert.That(_Results, Has.Length.EqualTo(5));
	}


	/// <summary>In SELECT mode, the list shows all items and selects the best match.</summary>
	[Test]
	public void SelectsBestMatch()
	{
		_Combo.FilterMode = ListFilterMode.SELECT;
		_Type("che");

		Assert.That(_Combo.DroppedDown, Is.True);
		Assert.That(_Combo._DisplayedSearchResults, Is.Null);
		Assert.That(_Combo._DropDownList!.ItemCount, Is.EqualTo(5));
		Assert.That(_Combo._DropDownList.SelectedIndex, Is.EqualTo(2));

		_Combo.PressKey(Keys.Enter);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(2));
	}


	/// <summary>F4 shows all items, not the search results.</summary>
	[Test]
	public void ShowsItemsWithF4()
	{
		_Combo.Text = "Cherry";
		_Combo.PressKey(Keys.F4);

		Assert.That(_Combo._DisplayedSearchResults, Is.Null);
		Assert.That(_Combo._DropDownList!.ItemCount, Is.EqualTo(5));
		Assert.That(_Combo._DropDownList.SelectedIndex, Is.EqualTo(2));
	}


	/// <summary>A search source is searched instead of the items; a chosen result becomes the selected item with index -1.</summary>
	[Test]
	public void SearchesSource()
	{
		_Combo.SearchSource = new AutoCompletionItemSource(new[] { "Durian", "Dragonfruit", "Fig" });
		_Type("dra");

		Assert.That(_Results[0], Is.EqualTo("Dragonfruit"));
		Assert.That(_Results, Does.Not.Contain("Fig"));
		Assert.That(_Results, Does.Not.Contain("Banana"));

		_Combo.PressKey(Keys.Enter);

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.InstanceOf<IAutoCompletionItem>());
		Assert.That(_Combo.Text, Is.EqualTo("Dragonfruit"));
	}


	/// <summary>Typing the exact text of a source result and leaving the text selects the result.</summary>
	[Test]
	public void CommitsSourceText()
	{
		_Combo.SearchSource = new AutoCompletionItemSource(new[] { "Durian", "Dragonfruit" });
		_Type("durian");
		_Combo.PressKey(Keys.Escape);
		_Combo.Text = _Combo.Text;

		Assert.That(_Combo.SelectedItem, Is.InstanceOf<IAutoCompletionItem>());
		Assert.That(_Combo.Text, Is.EqualTo("Durian"));
	}
}
