namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests filtering, ranking, selection and keyboard navigation in auto completion sessions.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionFilteringTests: AutoCompletionSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Typing narrows the list without requesting items again.</summary>
	[Test]
	public void TypingNarrowsList()
	{
		_Text.Type("A");
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));

		_Text.Type("u");
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));

		_Text.Type("stri");
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));
		Assert.That(_Count("Populating"), Is.EqualTo(1));
	}


	/// <summary>If nothing matches, the list is hidden but the session continues; Backspace shows the list again.</summary>
	[Test]
	public void NoMatchHidesList()
	{
		_Text.Type("Aux");
		Assert.That(_Selection, Is.Null);
		Assert.That(_Closed, Is.Empty);

		_Text.Backspace();
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
		Assert.That(_Count("Starting"), Is.EqualTo(1));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Items are sorted case-insensitively by their sort text.</summary>
	[Test]
	public void ItemsAreSortedBySortText()
	{
		_Provider.Source = new AutoCompletionItemSource(new IAutoCompletionItem[]
		{
			new AutoCompletionItem("zeta"), new AutoCompletionItem("Alpha"), new AutoCompletionItem("last") { SortText = "0" }
		});

		_Provider.ShowCompletion(_Text);
		Assert.That(_SelectedText(), Is.EqualTo("last"));

		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("Alpha"));
	}


	/// <summary>Items are matched by their filter text.</summary>
	[Test]
	public void ItemsAreMatchedByFilterText()
	{
		_Provider.Source = new AutoCompletionItemSource(new IAutoCompletionItem[] { new AutoCompletionItem("Order Details") { FilterText = "OrderDetails" } });
		_Text.SetText("OrderD");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Text, Is.EqualTo("Order Details"));
	}


	/// <summary>The contains mode lists substring matches after prefix matches; earlier substrings rank higher.</summary>
	[Test]
	public void ContainsModeRanksPrefixFirst()
	{
		_SetItems("StreamReader", "TextReader", "ReaderWriterLock");
		_Provider.MatchMode = ListMatchMode.CONTAINS;

		_Text.Type("Reader");

		Assert.That(_SelectedText(), Is.EqualTo("ReaderWriterLock"));
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("TextReader"));
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("StreamReader"));
	}


	/// <summary>The fuzzy mode ranks matches at word starts first.</summary>
	[Test]
	public void FuzzyModeRanksWordStarts()
	{
		_SetItems("CurrencyEntryOrderHistory", "CustomerOrderHistory");
		_Provider.SetMatchMode(_Text, ListMatchMode.FUZZY);

		_Text.Type("cuorhi");

		Assert.That(_SelectedText(), Is.EqualTo("CustomerOrderHistory"));
	}


	/// <summary>Case-sensitive matching only matches items with the same case.</summary>
	[Test]
	public void CaseSensitiveMatchingRespectsCase()
	{
		_SetItems("String", "string");
		_Provider.SetCaseSensitive(_Text, true);

		_Text.Type("s");

		Assert.That(_SelectedText(), Is.EqualTo("string"));
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("string"));
	}


	/// <summary>Settings are resolved in the order result, control, provider.</summary>
	[Test]
	public void SettingsAreResolvedInOrder()
	{
		_SetItems("Austria");
		_Text.SetText("str");

		_Provider.ShowCompletion(_Text);
		Assert.That(_Accepted, Is.Empty);
		_Provider.CloseCompletion();

		_Provider.SetMatchMode(_Text, ListMatchMode.CONTAINS);
		_Provider.Source = new AutoCompletionDelegateSource(_ => new AutoCompletionResult(_CreateItems("Austria")) { MatchMode = ListMatchMode.PREFIX });
		_Provider.ShowCompletion(_Text);
		Assert.That(_Accepted, Is.Empty);
		_Provider.CloseCompletion();

		_SetItems("Austria");
		_Provider.ShowCompletion(_Text);
		Assert.That(_Text.Text, Is.EqualTo("Austria"));
	}


	/// <summary>The select mode keeps all items and selects the best match; Down selects the closest item if nothing matches.</summary>
	[Test]
	public void SelectModeSelectsBestMatch()
	{
		_SetItems("France", "Germany", "Greece");
		_Provider.FilterMode = ListFilterMode.SELECT;

		_Text.Type("Gr");
		Assert.That(_SelectedText(), Is.EqualTo("Greece"));

		_Text.Type("\bx");
		Assert.That(_Selection, Is.Null);
		Assert.That(_Closed, Is.Empty);

		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("Germany"));
	}


	/// <summary>Up, Down, PageUp, PageDown, Home and End move the selection.</summary>
	[Test]
	public void KeysMoveSelection()
	{
		_Provider.ShowCompletion(_Text);
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));

		Assert.That(_Text.Press(Keys.Down), Is.True);
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));

		Assert.That(_Text.Press(Keys.Up), Is.True);
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));

		Assert.That(_Text.Press(Keys.End), Is.True);
		Assert.That(_SelectedText(), Is.EqualTo("Brazil"));

		Assert.That(_Text.Press(Keys.Home), Is.True);
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));

		Assert.That(_Text.Press(Keys.PageDown), Is.True);
		Assert.That(_SelectedText(), Is.Not.EqualTo("Argentina"));

		Assert.That(_Text.Press(Keys.PageUp), Is.True);
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Home and End with modifiers are left to the text box.</summary>
	[Test]
	public void ModifiedHomeAndEndAreNotHandled()
	{
		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Press(Keys.Shift | Keys.End), Is.False);
		Assert.That(_Text.Press(Keys.Control | Keys.Home), Is.False);
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));
	}


	/// <summary>A caret key that does not change the typed text keeps the selection.</summary>
	[Test]
	public void UnchangedWordKeepsSelection()
	{
		_Text.Type("A");
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));

		_Text.Press(Keys.Right);
		_DoEvents();

		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Moving the caret within the word filters by the text before the caret.</summary>
	[Test]
	public void CaretInWordRefilters()
	{
		_Text.Type("Aus");
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));

		_Text.Press(Keys.Left);
		_Text.Select(1, 0);
		_DoEvents();

		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));
	}


	/// <summary>Frequently accepted items rank first within their match class.</summary>
	[Test]
	public void UsageRanksItems()
	{
		_SetItems("City", "Country", "CreatedAt", "CustomerId");

		_Text.Type("C");
		Assert.That(_SelectedText(), Is.EqualTo("City"));

		_Provider.CloseCompletion();
		_Provider.UsageHistory.Increment(_SCOPE, "CustomerId");
		_Text.SetText(string.Empty);
		_Text.Type("C");

		Assert.That(_SelectedText(), Is.EqualTo("CustomerId"));
	}


	/// <summary>Usage does not rank items above a better match class.</summary>
	[Test]
	public void UsageKeepsMatchClasses()
	{
		_SetItems("Cu", "Customer", "Category", "CreateAccessToken");
		_Provider.MatchMode = ListMatchMode.FUZZY;
		for(int i = 0; i < 10; i++)
		{
			_Provider.UsageHistory.Increment(_SCOPE, "Customer");
			_Provider.UsageHistory.Increment(_SCOPE, "CreateAccessToken");
		}

		_Text.Type("Cu");
		Assert.That(_SelectedText(), Is.EqualTo("Cu"));

		_Provider.CloseCompletion();
		_Text.SetText(string.Empty);
		_Text.Type("cat");
		Assert.That(_SelectedText(), Is.EqualTo("Category"));
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("CreateAccessToken"));
	}


	/// <summary>Accepted items are recorded by insert text in the scope of the control.</summary>
	[Test]
	public void AcceptedItemsAreRecorded()
	{
		_Provider.Source = new AutoCompletionItemSource(new IAutoCompletionItem[] { new AutoCompletionItem("COUNT") { InsertText = "COUNT()" }, new AutoCompletionItem("CAST") });

		_Text.Type("CO");
		_Text.Press(Keys.Enter);

		Assert.That(_Provider.UsageHistory.GetCount(_SCOPE, "COUNT()"), Is.EqualTo(1));
		Assert.That(_Provider.UsageHistory.GetCount(_SCOPE, "COUNT"), Is.EqualTo(0));
		Assert.That(_Provider.UsageHistory.GetCount("Other", "COUNT()"), Is.EqualTo(0));
	}


	/// <summary>Without usage ranking, items are neither recorded nor ranked by usage.</summary>
	[Test]
	public void UsageRankingCanBeDisabled()
	{
		_SetItems("City", "CustomerId");
		_Provider.SetUsageRanking(_Text, false);
		_Provider.UsageHistory.Increment(_SCOPE, "CustomerId");

		_Text.Type("C");
		Assert.That(_SelectedText(), Is.EqualTo("City"));

		_Text.Press(Keys.Enter);
		Assert.That(_Provider.UsageHistory.GetCount(_SCOPE, "City"), Is.EqualTo(0));
	}


	/// <summary>A commit character accepts a unique prefix match and is inserted after the item.</summary>
	[Test]
	public void CommitCharacterAcceptsUniqueMatch()
	{
		_SetItems("String", "StringBuilder", "StringComparer");
		_Provider.CommitCharacters = "(";

		_Text.Type("StringB(");

		Assert.That(_Text.Text, Is.EqualTo("StringBuilder("));
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.ACCEPTED));
	}


	/// <summary>A commit character does not accept an ambiguous match, unless the item has been selected explicitly.</summary>
	[Test]
	public void CommitCharacterRequiresUnambiguousMatch()
	{
		_SetItems("String", "StringBuilder", "StringComparer");
		_Provider.CommitCharacters = "(";

		_Text.Type("Str(");
		Assert.That(_Text.Text, Is.EqualTo("Str("));
		Assert.That(_Accepted, Is.Empty);

		_Text.SetText(string.Empty);
		_Text.Type("Str");
		_Text.Press(Keys.Down);
		_Text.Type("(");
		Assert.That(_Text.Text, Is.EqualTo("StringBuilder("));
	}


	/// <summary>An exact match is unambiguous; space accepts only if enabled.</summary>
	[Test]
	public void SpaceAcceptsIfEnabled()
	{
		_SetItems("String", "StringBuilder");

		_Text.Type("String ");
		Assert.That(_Accepted, Is.Empty);

		_Provider.AcceptOnSpace = true;
		_Text.SetText(string.Empty);
		_Text.Type("String ");
		Assert.That(_Accepted, Has.Count.EqualTo(1));
		Assert.That(_Text.Text, Is.EqualTo("String "));
	}
}
