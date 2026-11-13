namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.ComponentModel;
using System.Data;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the data binding of the <see cref="ThemedComboBox"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxDataTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public classes                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents a product for data binding tests.</summary>
	public sealed class Product
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets the ID.</summary>
		public int Id { get; set; }

		/// <summary>Gets or sets the name.</summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>Gets or sets the origin.</summary>
		public string Origin { get; set; } = string.Empty;

		/// <summary>Gets or sets the image key.</summary>
		public string? ImageKey { get; set; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Tested control.</summary>
	private TestThemedComboBox _Combo = null!;

	/// <summary>Raised events.</summary>
	private readonly List<string> _Events = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the sample products.</summary>
	/// <returns>Returns the products.</returns>
	private static List<Product> _CreateProducts()
	{
		return new List<Product>()
		{
			new Product { Id = 1, Name = "Apple", Origin = "Austria", ImageKey = "apple" },
			new Product { Id = 2, Name = "Banana", Origin = "Ecuador", ImageKey = "banana" },
			new Product { Id = 3, Name = "Cherry", Origin = "Italy" }
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the members for the product class.</summary>
	private void _SetMembers()
	{
		_Combo.DisplayMember = "Name";
		_Combo.ValueMember = "Id";
		_Combo.DetailMember = "Origin";
		_Combo.ImageMember = "ImageKey";
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
		_Combo.SelectedValueChanged += (sender, e) => { _Events.Add("SelectedValueChanged"); };
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

	/// <summary>A list fills the items; the members provide text, detail and image.</summary>
	[Test]
	public void BindsList()
	{
		_SetMembers();
		_Combo.DataSource = _CreateProducts();

		Assert.That(_Combo.Items, Has.Count.EqualTo(3));
		Assert.That(_Combo.GetItemText(_Combo.Items[1]), Is.EqualTo("Banana"));
		Assert.That(_Combo._GetViewsForTest()[0].Detail, Is.EqualTo("Austria"));
		Assert.That(_Combo._GetViewsForTest()[0].ImageKey, Is.EqualTo("apple"));
		Assert.That(_Combo._GetViewsForTest()[2].ImageKey, Is.Null);
		Assert.That(_Combo.GetItemValue(_Combo.Items[2]), Is.EqualTo(3));
	}


	/// <summary>SelectedValue selects by value member and raises SelectedValueChanged.</summary>
	[Test]
	public void SelectsByValue()
	{
		_SetMembers();
		_Combo.DataSource = _CreateProducts();
		_Events.Clear();

		_Combo.SelectedValue = 2;
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.SelectedValue, Is.EqualTo(2));
		Assert.That(_Combo.Text, Is.EqualTo("Banana"));
		Assert.That(_Events, Does.Contain("SelectedValueChanged"));

		_Combo.SelectedValue = 99;
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedValue, Is.Null);
	}


	/// <summary>Without value member, the value is the item itself.</summary>
	[Test]
	public void UsesItemAsValue()
	{
		_Combo.Items.AddRange("Apple", "Banana");
		_Combo.SelectedValue = "Banana";

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(_Combo.SelectedValue, Is.EqualTo("Banana"));
	}


	/// <summary>While a data source is set, the items cannot be changed; removing the data source clears them.</summary>
	[Test]
	public void ProtectsBoundItems()
	{
		_Combo.DataSource = _CreateProducts();

		Assert.Throws<InvalidOperationException>(() => _Combo.Items.Add("Durian"));
		Assert.Throws<InvalidOperationException>(() => _Combo.Items.RemoveAt(0));
		Assert.Throws<InvalidOperationException>(() => _Combo.Items.Clear());

		_Combo.DataSource = null;
		Assert.That(_Combo.Items, Is.Empty);

		_Combo.Items.Add("Durian");
		Assert.That(_Combo.Items, Has.Count.EqualTo(1));
	}


	/// <summary>Changes of a binding list update the items and keep the selection.</summary>
	[Test]
	public void FollowsBindingList()
	{
		BindingList<Product> products = new(_CreateProducts());

		_SetMembers();
		_Combo.DataSource = products;
		_Combo.SelectedIndex = 1;
		object selected = _Combo.SelectedItem!;
		_Events.Clear();

		products.Insert(0, new Product { Id = 4, Name = "Avocado" });
		Assert.That(_Combo.Items, Has.Count.EqualTo(4));
		Assert.That(_Combo.SelectedItem, Is.SameAs(selected));
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(2));

		products.Remove((Product) selected);
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(-1));
		Assert.That(_Combo.SelectedItem, Is.Null);
	}


	/// <summary>A data table is bound with its columns as members.</summary>
	[Test]
	public void BindsDataTable()
	{
		using DataTable table = new();

		table.Columns.Add("Id", typeof(int));
		table.Columns.Add("Name", typeof(string));
		table.Rows.Add(1, "Apple");
		table.Rows.Add(2, "Banana");
		table.Rows.Add(3, "Cherry");

		_Combo.DisplayMember = "Name";
		_Combo.ValueMember = "Id";
		_Combo.DataSource = table;

		Assert.That(_Combo.Items, Has.Count.EqualTo(3));
		Assert.That(_Combo.GetItemText(_Combo.Items[1]), Is.EqualTo("Banana"));

		_Combo.SelectedValue = 3;
		Assert.That(_Combo.Text, Is.EqualTo("Cherry"));
	}


	/// <summary>With a binding context, the selection follows the position of the currency manager and vice versa.</summary>
	[Test]
	public void SynchronizesPosition()
	{
		using Form form = new();
		List<Product> products = _CreateProducts();

		form.Controls.Add(_Combo);
		_SetMembers();
		_Combo.DataSource = products;

		CurrencyManager manager = (CurrencyManager) form.BindingContext![products];

		Assert.That(_Combo.SelectedIndex, Is.EqualTo(0));

		_Combo.SelectedIndex = 2;
		Assert.That(manager.Position, Is.EqualTo(2));

		manager.Position = 1;
		Assert.That(_Combo.SelectedIndex, Is.EqualTo(1));

		form.Controls.Remove(_Combo);
	}


	/// <summary>Other data sources than lists are rejected.</summary>
	[Test]
	public void RejectsInvalidDataSource()
	{
		Assert.Throws<ArgumentException>(() => _Combo.DataSource = 42);
	}
}
