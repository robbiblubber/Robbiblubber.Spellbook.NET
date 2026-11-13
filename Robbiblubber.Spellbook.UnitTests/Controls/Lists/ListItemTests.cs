namespace Robbiblubber.Spellbook.UnitTests.Controls.Lists;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="ListItem"/> class and its relation to <see cref="AutoCompletionItem"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class ListItemTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The constructors set text, image key and image; the defaults are empty.</summary>
	[Test]
	public void ConstructorsSetProperties()
	{
		using Bitmap image = new(16, 16);
		ListItem plain = new("Customers");
		ListItem keyed = new("Orders", "table");
		ListItem imaged = new("Products", image);

		Assert.That(plain.Text, Is.EqualTo("Customers"));
		Assert.That(plain.ToString(), Is.EqualTo("Customers"));
		Assert.That(plain.Image, Is.Null);
		Assert.That(plain.ImageKey, Is.Null);
		Assert.That(plain.FontStyle, Is.EqualTo(FontStyle.Regular));
		Assert.That(plain.ForeColor, Is.EqualTo(Color.Empty));
		Assert.That(keyed.ImageKey, Is.EqualTo("table"));
		Assert.That(imaged.Image, Is.SameAs(image));

		Assert.Throws<ArgumentNullException>(() => new ListItem(null!));
	}


	/// <summary>Optional properties are set with object initializers.</summary>
	[Test]
	public void InitializersSetProperties()
	{
		ListItem item = new("Customers") { Detail = "dbo", Description = "All customers", Category = "table", FilterText = "cust", SortText = "1", Tag = 42 };

		Assert.That(item.Detail, Is.EqualTo("dbo"));
		Assert.That(item.Description, Is.EqualTo("All customers"));
		Assert.That(item.Category, Is.EqualTo("table"));
		Assert.That(item.FilterText, Is.EqualTo("cust"));
		Assert.That(item.SortText, Is.EqualTo("1"));
		Assert.That(item.Tag, Is.EqualTo(42));
	}


	/// <summary>Auto completion items are list items with insert information.</summary>
	[Test]
	public void AutoCompletionItemIsListItem()
	{
		AutoCompletionItem item = new("COUNT", "function") { InsertText = "COUNT()", CaretPosition = 6, Detail = "aggregate" };
		IListItem listItem = item;

		Assert.That(item, Is.InstanceOf<ListItem>());
		Assert.That(listItem.Text, Is.EqualTo("COUNT"));
		Assert.That(listItem.Detail, Is.EqualTo("aggregate"));
		Assert.That(listItem.ImageKey, Is.EqualTo("function"));
		Assert.That(((IAutoCompletionItem) item).InsertText, Is.EqualTo("COUNT()"));
		Assert.That(((IAutoCompletionItem) item).CaretPosition, Is.EqualTo(6));
	}
}
