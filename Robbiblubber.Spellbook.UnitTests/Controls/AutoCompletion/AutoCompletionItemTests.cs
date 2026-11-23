namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the item and result classes: <see cref="AutoCompletionItem"/>, <see cref="AutoCompletionResult"/> and
///          <see cref="AutoCompletionPopulatingEventArgs"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionItemTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new item has the given text and default values for all optional properties.</summary>
	[Test]
	public void ItemHasDefaults()
	{
		AutoCompletionItem item = new("Customers");

		Assert.That(item.Text, Is.EqualTo("Customers"));
		Assert.That(item.ToString(), Is.EqualTo("Customers"));
		Assert.That(item.InsertText, Is.Null);
		Assert.That(item.FilterText, Is.Null);
		Assert.That(item.SortText, Is.Null);
		Assert.That(item.Detail, Is.Null);
		Assert.That(item.Description, Is.Null);
		Assert.That(item.Category, Is.Null);
		Assert.That(item.Image, Is.Null);
		Assert.That(item.ImageKey, Is.Null);
		Assert.That(item.FontStyle, Is.EqualTo(FontStyle.Regular));
		Assert.That(item.ForeColor, Is.EqualTo(Color.Empty));
		Assert.That(item.CaretPosition, Is.Null);
		Assert.That(item.Tag, Is.Null);
	}


	/// <summary>The constructors set image key and image.</summary>
	[Test]
	public void ItemConstructorsSetImage()
	{
		using(Bitmap image = new(16, 16))
		{
			Assert.That(new AutoCompletionItem("t", "table").ImageKey, Is.EqualTo("table"));
			Assert.That(new AutoCompletionItem("t", image).Image, Is.SameAs(image));
		}
	}


	/// <summary>An item without text is rejected.</summary>
	[Test]
	public void ItemRequiresText()
	{
		Assert.That(() => new AutoCompletionItem(null!), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new AutoCompletionItem(null!, "key"), Throws.TypeOf<ArgumentNullException>());
	}


	/// <summary>A result takes a snapshot of the items and has no overrides by default.</summary>
	[Test]
	public void ResultCopiesItems()
	{
		List<IAutoCompletionItem> items = new() { new AutoCompletionItem("a") };
		AutoCompletionResult result = new(items);

		items.Add(new AutoCompletionItem("b"));

		Assert.That(result.Items, Has.Count.EqualTo(1));
		Assert.That(result.IsIncomplete, Is.False);
		Assert.That(result.ReplaceStart, Is.Null);
		Assert.That(result.ReplaceLength, Is.Null);
		Assert.That(result.MatchMode, Is.Null);
		Assert.That(result.FilterMode, Is.Null);
		Assert.That(result.CaseSensitive, Is.Null);
		Assert.That(() => new AutoCompletionResult(null!), Throws.TypeOf<ArgumentNullException>());
	}


	/// <summary>The populating event arguments are initialized from the result; the item list is a copy.</summary>
	[Test]
	public void PopulatingArgumentsCopyResult()
	{
		using(TextBox text = new())
		{
			AutoCompletionContext context = new(text, "x", 1, 0, string.Empty, AutoCompletionTrigger.MANUAL, '\0');
			AutoCompletionResult result = new(new[] { new AutoCompletionItem("a") })
			{
				IsIncomplete = true, ReplaceStart = 0, ReplaceLength = 1, MatchMode = ListMatchMode.FUZZY, FilterMode = ListFilterMode.SELECT, CaseSensitive = true
			};
			AutoCompletionPopulatingEventArgs e = new(context, result);

			e.Items.Add(new AutoCompletionItem("b"));

			Assert.That(e.Context, Is.SameAs(context));
			Assert.That(e.Items, Has.Count.EqualTo(2));
			Assert.That(result.Items, Has.Count.EqualTo(1));
			Assert.That(e.IsIncomplete, Is.True);
			Assert.That(e.ReplaceStart, Is.EqualTo(0));
			Assert.That(e.ReplaceLength, Is.EqualTo(1));
			Assert.That(e.MatchMode, Is.EqualTo(ListMatchMode.FUZZY));
			Assert.That(e.FilterMode, Is.EqualTo(ListFilterMode.SELECT));
			Assert.That(e.CaseSensitive, Is.True);
		}
	}


	/// <summary>Without a result, the populating event arguments have an empty item list and no overrides.</summary>
	[Test]
	public void PopulatingArgumentsWithoutResult()
	{
		using(TextBox text = new())
		{
			AutoCompletionPopulatingEventArgs e = new(new AutoCompletionContext(text, string.Empty, 0, 0, string.Empty, AutoCompletionTrigger.MANUAL, '\0'), null);

			Assert.That(e.Items, Is.Empty);
			Assert.That(e.IsIncomplete, Is.False);
			Assert.That(e.MatchMode, Is.Null);
		}
	}
}
