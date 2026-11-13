namespace Robbiblubber.Spellbook.UnitTests.Controls.RecentLists;

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="RecentItemList"/> component.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class RecentItemListTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Store.</summary>
	private TestRecentListStore _Store = null!;

	/// <summary>List.</summary>
	private RecentItemList _List = null!;

	/// <summary>Menu item.</summary>
	private ToolStripMenuItem _Menu = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Store = new TestRecentListStore();
		_List = new RecentItemList() { Store = _Store };
		_Menu = new ToolStripMenuItem("Recent");
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_List.Dispose();
		_Menu.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the texts of the menu items of a collection.</summary>
	/// <param name="items">Menu items.</param>
	/// <returns>Returns the texts; separators are returned as "-".</returns>
	private static string[] _Texts(ToolStripItemCollection items)
	{
		return items.Cast<ToolStripItem>().Select(m => ((m is ToolStripSeparator) ? "-" : (m.Text ?? string.Empty))).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new list has the documented default values.</summary>
	[Test]
	public void ListHasDefaults()
	{
		using RecentItemList list = new();
		using RecentFileList files = new();

		Assert.That(list.ListName, Is.EqualTo(RecentItemList.DEFAULT_LIST_NAME));
		Assert.That(files.ListName, Is.EqualTo(RecentFileList.DEFAULT_FILE_LIST_NAME));
		Assert.That(list.MaxCount, Is.EqualTo(10));
		Assert.That(list.MenuStyle, Is.EqualTo(RecentListMenuStyle.SUBMENU));
		Assert.That(list.ShowNumbers, Is.True);
		Assert.That(list.ShowClearItem, Is.True);
		Assert.That(list.DisableWhenEmpty, Is.True);
		Assert.That(list.ShowImages, Is.True);
		Assert.That(list.Store, Is.InstanceOf<DdnRecentListStore>());
	}


	/// <summary>The public properties declare their serialization.</summary>
	[Test]
	public void PropertiesDeclareSerialization()
	{
		foreach(Type t in new Type[] { typeof(RecentItemList), typeof(RecentFileList) })
		{
			foreach(PropertyInfo i in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => m.CanWrite))
			{
				bool declared = (i.GetCustomAttribute<DefaultValueAttribute>() is not null) || (i.GetCustomAttribute<DesignerSerializationVisibilityAttribute>() is not null) ||
								(t.GetMethod("ShouldSerialize" + i.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is not null);
				Assert.That(declared, Is.True, t.Name + "." + i.Name);
			}
		}
	}


	/// <summary>Adding saves the list; the most recent entry comes first.</summary>
	[Test]
	public void AddsEntries()
	{
		_List.Add("a");
		_List.Add("b", "Bee");

		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "b", "a" }));
		Assert.That(_List.Items[0].Text, Is.EqualTo("Bee"));
		Assert.That(_Store.Lists[RecentItemList.DEFAULT_LIST_NAME].Count, Is.EqualTo(2));
		Assert.That(_List.Contains("a"), Is.True);
		Assert.That(_List.Contains("A"), Is.False);
	}


	/// <summary>Invalid keys are ignored.</summary>
	[Test]
	public void IgnoresEmptyKeys()
	{
		Assert.That(_List.Add("  "), Is.Null);
		Assert.That(_Store.Saves, Is.EqualTo(0));
	}


	/// <summary>The list keeps at most MaxCount unpinned entries.</summary>
	[Test]
	public void KeepsMaxCount()
	{
		_List.MaxCount = 3;
		for(int i = 0; i < 5; i++) { _List.Add("k" + i); }

		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "k4", "k3", "k2" }));
		Assert.Throws<ArgumentOutOfRangeException>(() => _List.MaxCount = 0);
	}


	/// <summary>Changes made by another instance sharing the store are kept.</summary>
	[Test]
	public void SharesStoreWithOtherInstances()
	{
		using RecentItemList other = new() { Store = _Store };

		_List.Add("a");
		other.Add("b");
		_List.Add("c");

		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "c", "b", "a" }));
	}


	/// <summary>Pinning, unpinning, removing and clearing change the list and raise Changed.</summary>
	[Test]
	public void ModifiesEntries()
	{
		int changes = 0;

		_List.Add("a");
		_List.Add("b");
		_List.Add("c");
		_List.Changed += (sender, e) => { changes++; };

		Assert.That(_List.Pin("a"), Is.True);
		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "a", "c", "b" }));
		Assert.That(_List.Items[0].Pinned, Is.True);

		Assert.That(_List.Remove("c"), Is.True);
		Assert.That(_List.Remove("c"), Is.False);

		_List.Clear();
		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "a" }));

		Assert.That(_List.Unpin("a"), Is.True);
		_List.Clear(true);
		Assert.That(_List.Items, Is.Empty);

		Assert.That(changes, Is.EqualTo(5));
	}


	/// <summary>The submenu shows numbered entries, pinned entries first with an image, a separator and the clear item.</summary>
	[Test]
	public void BuildsSubmenu()
	{
		_List.Add("a");
		_List.Add("b & c");
		_List.Pin("a");
		_List.MenuItem = _Menu;

		Assert.That(_Texts(_Menu.DropDownItems), Is.EqualTo(new string[] { "&1 a", "-", "&2 b && c", "-", "&Clear List" }));
		Assert.That(_Menu.DropDownItems[0].Image, Is.Not.Null);
		Assert.That(_Menu.DropDownItems[2].Image, Is.Null);
	}


	/// <summary>This class implements a list whose entries have an image.</summary>
	private class _ImageList: RecentItemList
	{
		/// <summary>Gets the image of all entries.</summary>
		public Bitmap Image
		{
			get;
		} = new Bitmap(16, 16);


		/// <inheritdoc/>
		protected override Image? _GetImage(RecentItem item)
		{
			return Image;
		}
	}


	/// <summary>Entries show their image; pinned entries show the pin drawn over a copy of it.</summary>
	[Test]
	public void ShowsImages()
	{
		using _ImageList list = new() { Store = _Store, ShowClearItem = false };

		list.Add("a");
		list.Add("b");
		list.Pin("a");
		list.MenuItem = _Menu;

		Assert.That(_Menu.DropDownItems[2].Image, Is.SameAs(list.Image));
		Assert.That(_Menu.DropDownItems[0].Image, Is.Not.Null.And.Not.SameAs(list.Image));
		Assert.That(_Menu.DropDownItems[0].Image!.Size, Is.EqualTo(list.Image.Size));

		list.ShowImages = false;
		Assert.That(_Menu.DropDownItems[2].Image, Is.Null);
		Assert.That(_Menu.DropDownItems[0].Image, Is.Not.Null);

		list.MenuItem = null;
		Assert.That(list.Image.Width, Is.EqualTo(16));

		list.Image.Dispose();
	}


	/// <summary>An empty list disables the menu item; it shows a disabled placeholder if the menu item stays enabled.</summary>
	[Test]
	public void HandlesEmptyList()
	{
		_List.MenuItem = _Menu;
		Assert.That(_Menu.Enabled, Is.False);

		_List.DisableWhenEmpty = false;
		Assert.That(_Menu.Enabled, Is.True);
		Assert.That(_Texts(_Menu.DropDownItems), Is.EqualTo(new string[] { "(Empty)" }));
		Assert.That(_Menu.DropDownItems[0].Enabled, Is.False);

		_List.DisableWhenEmpty = true;
		_List.Add("a");
		Assert.That(_Menu.Enabled, Is.True);

		_List.Clear();
		Assert.That(_Menu.Enabled, Is.False);

		_List.MenuItem = null;
		Assert.That(_Menu.Enabled, Is.True);
	}


	/// <summary>Opening the menu that contains the menu item updates its state, for example after another instance has changed the
	///          list.</summary>
	[Test]
	public void UpdatesStateWhenParentOpens()
	{
		using ToolStripMenuItem file = new("File");
		using RecentItemList other = new() { Store = _Store };

		file.DropDownItems.Add(_Menu);
		_List.MenuItem = _Menu;
		Assert.That(_Menu.Enabled, Is.False);

		other.Add("a");
		typeof(ToolStripDropDown).GetMethod("OnOpening", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(file.DropDown, new object[] { new CancelEventArgs() });

		Assert.That(_Menu.Enabled, Is.True);
	}


	/// <summary>The menu follows changes of the list.</summary>
	[Test]
	public void RebuildsMenuOnChange()
	{
		_List.MenuItem = _Menu;
		_List.ShowNumbers = false;
		_List.ShowClearItem = false;

		_List.Add("a");
		_List.Add("b");

		Assert.That(_Texts(_Menu.DropDownItems), Is.EqualTo(new string[] { "b", "a" }));
	}


	/// <summary>In inline style, the entries replace the hidden menu item.</summary>
	[Test]
	public void BuildsInlineMenu()
	{
		using ToolStripMenuItem file = new("File");
		ToolStripMenuItem open = new("Open");
		ToolStripMenuItem exit = new("Exit");

		file.DropDownItems.AddRange(new ToolStripItem[] { open, _Menu, exit });
		_List.ShowClearItem = false;
		_List.Add("a");
		_List.Add("b");

		_List.MenuStyle = RecentListMenuStyle.INLINE;
		_List.MenuItem = _Menu;

		Assert.That(_Menu.Available, Is.False);
		Assert.That(_Texts(file.DropDownItems), Is.EqualTo(new string[] { "Open", "Recent", "&1 b", "&2 a", "Exit" }));

		_List.MenuItem = null;
		Assert.That(_Texts(file.DropDownItems), Is.EqualTo(new string[] { "Open", "Recent", "Exit" }));
		Assert.That(_Menu.Available, Is.True);
	}


	/// <summary>Clicking an entry raises ItemSelected; the handler can remove the entry.</summary>
	[Test]
	public void ClickSelectsEntry()
	{
		List<RecentItem> selected = new();

		_List.Add("a");
		_List.Add("b");
		_List.MenuItem = _Menu;
		_List.ItemSelected += (sender, e) =>
		{
			selected.Add(e.Item);
			Assert.That(e.Available, Is.Null);
			e.Remove = (e.Item.Key == "a");
		};

		_Menu.DropDownItems[0].PerformClick();
		Assert.That(selected.Select(m => m.Key), Is.EqualTo(new string[] { "b" }));
		Assert.That(_List.Contains("b"), Is.True);

		_Menu.DropDownItems[1].PerformClick();
		Assert.That(_List.Contains("a"), Is.False);
	}


	/// <summary>The clear item removes the unpinned entries.</summary>
	[Test]
	public void ClearItemClearsList()
	{
		_List.Add("a");
		_List.Add("b");
		_List.Pin("a");
		_List.MenuItem = _Menu;

		_Menu.DropDownItems.Cast<ToolStripItem>().Last().PerformClick();

		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "a" }));
	}


	/// <summary>Lists with different names share a store independently.</summary>
	[Test]
	public void SeparatesListNames()
	{
		using RecentItemList other = new() { Store = _Store, ListName = "other" };

		_List.Add("a");
		other.Add("b");

		Assert.That(_List.Items.Select(m => m.Key), Is.EqualTo(new string[] { "a" }));
		Assert.That(other.Items.Select(m => m.Key), Is.EqualTo(new string[] { "b" }));

		other.ListName = null!;
		Assert.That(other.ListName, Is.EqualTo(RecentItemList.DEFAULT_LIST_NAME));
	}
}
