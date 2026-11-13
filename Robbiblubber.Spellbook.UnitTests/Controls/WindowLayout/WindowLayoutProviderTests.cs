namespace Robbiblubber.Spellbook.UnitTests.Controls.WindowLayout;

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="WindowLayoutProvider"/> component.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class WindowLayoutProviderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Layout store.</summary>
	private TestWindowLayoutStore _Store = null!;

	/// <summary>Forms created by the test.</summary>
	private readonly List<Form> _Forms = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Store = new TestWindowLayoutStore();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		foreach(Form i in _Forms) { i.Dispose(); }
		_Forms.Clear();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a form with a provider.</summary>
	/// <param name="provider">Returns the provider.</param>
	/// <param name="name">Form name.</param>
	/// <returns>Returns the form.</returns>
	private Form _CreateForm(out WindowLayoutProvider provider, string name = "TestForm")
	{
		Form rval = new() { Name = name, StartPosition = FormStartPosition.CenterScreen };
		Rectangle area = Screen.PrimaryScreen!.WorkingArea;

		rval.Bounds = new Rectangle(area.X + 40, area.Y + 40, 500, 400);
		provider = new WindowLayoutProvider(rval) { Store = _Store };
		_Forms.Add(rval);

		return rval;
	}


	/// <summary>Creates a split container.</summary>
	/// <param name="fixedPanel">Fixed panel.</param>
	/// <param name="width">Width.</param>
	/// <returns>Returns the split container.</returns>
	private static SplitContainer _CreateSplit(FixedPanel fixedPanel, int width)
	{
		return new SplitContainer() { Name = "_Split", Orientation = Orientation.Vertical, FixedPanel = fixedPanel, Size = new Size(width, 200), SplitterWidth = 4 };
	}


	/// <summary>Creates a list view with three columns.</summary>
	/// <returns>Returns the list view.</returns>
	private static ListView _CreateList()
	{
		ListView rval = new() { Name = "_List", View = View.Details, AllowColumnReorder = true };

		rval.Columns.Add("A", 100);
		rval.Columns.Add("B", 100);
		rval.Columns.Add("C", 100);
		_ = rval.Handle;

		return rval;
	}


	/// <summary>Creates a data grid view with three columns.</summary>
	/// <returns>Returns the data grid view.</returns>
	private static DataGridView _CreateGrid()
	{
		DataGridView rval = new() { Name = "_Grid", AllowUserToOrderColumns = true };

		rval.Columns.Add("A", "A");
		rval.Columns.Add("B", "B");
		rval.Columns.Add("C", "C");
		foreach(DataGridViewColumn i in rval.Columns) { i.Width = 100; }

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new provider has the documented default values.</summary>
	[Test]
	public void ProviderHasDefaults()
	{
		using WindowLayoutProvider provider = new();

		Assert.That(provider.AutoRestore, Is.True);
		Assert.That(provider.AutoSave, Is.True);
		Assert.That(provider.RestoreMaximized, Is.True);
		Assert.That(provider.RestoreMinimized, Is.False);
		Assert.That(provider.PerScreenConfiguration, Is.False);
		Assert.That(provider.LayoutName, Is.Null);
		Assert.That(provider.ConfigFile, Is.Null);
		Assert.That(provider.Store, Is.InstanceOf<DdnWindowLayoutStore>());
	}


	/// <summary>The default store follows the configuration file; setting the store to NULL restores the default store.</summary>
	[Test]
	public void DefaultStoreFollowsConfigFile()
	{
		using WindowLayoutProvider provider = new() { ConfigFile = @"C:\temp\test.layout" };

		Assert.That(((DdnWindowLayoutStore) provider.Store).ConfigFile, Is.EqualTo(@"C:\temp\test.layout"));

		provider.Store = _Store;
		Assert.That(provider.Store, Is.SameAs(_Store));

		provider.Store = null;
		Assert.That(provider.Store, Is.InstanceOf<DdnWindowLayoutStore>());
	}


	/// <summary>The provider extends split containers, list views, data grid views and tab controls.</summary>
	[Test]
	public void CanExtendLayoutControls()
	{
		using WindowLayoutProvider provider = new();
		using SplitContainer split = new();
		using ListView list = new();
		using DataGridView grid = new();
		using TabControl tabs = new();
		using TextBox text = new();

		Assert.That(provider.CanExtend(split), Is.True);
		Assert.That(provider.CanExtend(list), Is.True);
		Assert.That(provider.CanExtend(grid), Is.True);
		Assert.That(provider.CanExtend(tabs), Is.True);
		Assert.That(provider.CanExtend(text), Is.False);
	}


	/// <summary>The extender property and the public properties declare their serialization.</summary>
	[Test]
	public void PropertiesDeclareSerialization()
	{
		Assert.That(typeof(WindowLayoutProvider).GetCustomAttribute<ProvidePropertyAttribute>()!.PropertyName, Is.EqualTo("SaveLayout"));
		Assert.That(typeof(WindowLayoutProvider).GetMethod(nameof(WindowLayoutProvider.GetSaveLayout))!.GetCustomAttribute<DefaultValueAttribute>()!.Value, Is.EqualTo(false));

		foreach(PropertyInfo i in typeof(WindowLayoutProvider).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => m.CanWrite && (m.Name != nameof(Component.Site))))
		{
			bool declared = (i.GetCustomAttribute<DefaultValueAttribute>() is not null) || (i.GetCustomAttribute<DesignerSerializationVisibilityAttribute>() is not null);
			Assert.That(declared, Is.True, i.Name);
		}
	}


	/// <summary>The window bounds and state are saved with the DPI.</summary>
	[Test]
	public void SavesWindow()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);

		provider.Save();

		Assert.That(_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)], Is.EqualTo(new int[] { form.Left, form.Top, 500, 400, form.DeviceDpi, (int) FormWindowState.Normal }));
		Assert.That(_Store.Commits, Is.EqualTo(1));
	}


	/// <summary>Saved bounds are restored, and the start position is set to manual.</summary>
	[Test]
	public void RestoresWindow()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Rectangle area = Screen.PrimaryScreen!.WorkingArea;

		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { area.X + 70, area.Y + 60, 450, 350, form.DeviceDpi, 0 };
		provider.Restore();

		Assert.That(form.Bounds, Is.EqualTo(new Rectangle(area.X + 70, area.Y + 60, 450, 350)));
		Assert.That(form.StartPosition, Is.EqualTo(FormStartPosition.Manual));
	}


	/// <summary>Sizes saved at a different DPI are scaled.</summary>
	[Test]
	public void ScalesSizeToDpi()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Rectangle area = Screen.PrimaryScreen!.WorkingArea;

		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { area.X + 10, area.Y + 10, 300, 200, form.DeviceDpi * 2, 0 };
		provider.Restore();

		Assert.That(form.Size, Is.EqualTo(new Size(150, 100)));
	}


	/// <summary>Windows on missing screens are moved onto a screen.</summary>
	[Test]
	public void MovesInvisibleWindow()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);

		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { -100000, -100000, 400, 300, form.DeviceDpi, 0 };
		provider.Restore();

		Assert.That(Screen.AllScreens.Any(m => m.WorkingArea.Contains(form.Bounds)), Is.True);
	}


	/// <summary>Forms with a fixed size only get their position restored.</summary>
	[Test]
	public void FixedSizeRestoresPositionOnly()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Rectangle area = Screen.PrimaryScreen!.WorkingArea;

		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.Size = new Size(320, 240);
		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { area.X + 30, area.Y + 20, 700, 600, form.DeviceDpi, 0 };
		provider.Restore();

		Assert.That(form.Location, Is.EqualTo(new Point(area.X + 30, area.Y + 20)));
		Assert.That(form.Size, Is.EqualTo(new Size(320, 240)));
	}


	/// <summary>The maximized state is restored unless disabled; the minimized state is not restored by default.</summary>
	[Test]
	public void RestoresWindowState()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Rectangle area = Screen.PrimaryScreen!.WorkingArea;

		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { area.X + 10, area.Y + 10, 400, 300, form.DeviceDpi, (int) FormWindowState.Maximized };
		provider.Restore();
		Assert.That(form.WindowState, Is.EqualTo(FormWindowState.Maximized));

		form.WindowState = FormWindowState.Normal;
		provider.RestoreMaximized = false;
		provider.Restore();
		Assert.That(form.WindowState, Is.EqualTo(FormWindowState.Normal));

		_Store.Values[("TestForm", WindowLayoutProvider.WINDOW_KEY)] = new int[] { area.X + 10, area.Y + 10, 400, 300, form.DeviceDpi, (int) FormWindowState.Minimized };
		provider.Restore();
		Assert.That(form.WindowState, Is.EqualTo(FormWindowState.Normal));
	}


	/// <summary>The layout name defaults to the form name and can be set; per screen configuration, a key is appended.</summary>
	[Test]
	public void UsesLayoutName()
	{
		_CreateForm(out WindowLayoutProvider provider, "MainForm");

		provider.Save();
		Assert.That(_Store.Values.Keys.Any(m => m.Layout == "MainForm"), Is.True);

		provider.LayoutName = "Custom";
		provider.Save();
		Assert.That(_Store.Values.Keys.Any(m => m.Layout == "Custom"), Is.True);

		provider.PerScreenConfiguration = true;
		provider.Save();
		Assert.That(_Store.Values.Keys.Any(m => m.Layout == "Custom_" + WindowLayoutProvider.GetScreenConfigurationKey()), Is.True);
	}


	/// <summary>Only controls with SaveLayout are saved; their keys contain the names of their parents.</summary>
	[Test]
	public void SavesSelectedControlsOnly()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Panel panel = new() { Name = "_Panel" };
		SplitContainer inner = _CreateSplit(FixedPanel.None, 400);
		SplitContainer other = _CreateSplit(FixedPanel.None, 400);

		other.Name = "_Other";
		panel.Controls.Add(inner);
		form.Controls.Add(panel);
		form.Controls.Add(other);
		provider.SetSaveLayout(inner, true);

		provider.Save();

		Assert.That(provider.GetSaveLayout(inner), Is.True);
		Assert.That(provider.GetSaveLayout(other), Is.False);
		Assert.That(_Store.Values.ContainsKey(("TestForm", "ctl__Panel__Split")), Is.True);
		Assert.That(_Store.Values.Keys.Count(m => m.Key.StartsWith("ctl_")), Is.EqualTo(1));
	}


	/// <summary>Controls without a name are not saved.</summary>
	[Test]
	public void SkipsUnnamedControls()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		SplitContainer split = _CreateSplit(FixedPanel.None, 400);

		split.Name = string.Empty;
		form.Controls.Add(split);
		provider.SetSaveLayout(split, true);

		provider.Save();

		Assert.That(_Store.Values.Keys.Any(m => m.Key.StartsWith("ctl_")), Is.False);
	}


	/// <summary>Parents without a name, like the panels of a split container, are skipped in the key.</summary>
	[Test]
	public void SkipsUnnamedParents()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		Panel panel = new();
		SplitContainer split = _CreateSplit(FixedPanel.None, 400);

		panel.Controls.Add(split);
		form.Controls.Add(panel);
		provider.SetSaveLayout(split, true);

		provider.Save();

		Assert.That(_Store.Values.ContainsKey(("TestForm", "ctl__Split")), Is.True);
	}


	/// <summary>A list view without a handle gets its column layout again when the handle is created.</summary>
	[Test]
	public void ReappliesListLayoutOnHandleCreated()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		ListView list = new() { Name = "_List", View = View.Details };

		list.Columns.Add("A", 100);
		list.Columns.Add("B", 100);
		list.Columns.Add("C", 100);
		form.Controls.Add(list);
		provider.SetSaveLayout(list, true);
		_Store.Values[("TestForm", "ctl__List")] = new int[] { 3, 50, 60, 70, 2, 0, 1 };

		provider.Restore();
		_ = list.Handle;

		Assert.That(list.Columns[2].Width, Is.EqualTo(70).Within(1));
		Assert.That(list.Columns[1].DisplayIndex, Is.EqualTo(0));
		Assert.That(list.Columns[0].DisplayIndex, Is.EqualTo(2));
	}


	/// <summary>Without a fixed panel, the splitter is restored proportionally.</summary>
	[Test]
	public void RestoresSplitterRatio()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		SplitContainer split = _CreateSplit(FixedPanel.None, 404);

		form.Controls.Add(split);
		provider.SetSaveLayout(split, true);
		split.SplitterDistance = 100;
		provider.Save();

		split.Width = 804;
		split.SplitterDistance = 50;
		provider.Restore();

		Assert.That(split.SplitterDistance, Is.EqualTo(200).Within(1));
	}


	/// <summary>With a fixed panel, the size of the fixed panel is restored.</summary>
	[Test]
	public void RestoresFixedPanel()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		SplitContainer split = _CreateSplit(FixedPanel.Panel2, 404);

		form.Controls.Add(split);
		provider.SetSaveLayout(split, true);
		split.SplitterDistance = 300;
		provider.Save();

		split.Width = 604;
		split.SplitterDistance = 100;
		provider.Restore();

		Assert.That(split.Width - split.SplitterWidth - split.SplitterDistance, Is.EqualTo(100).Within(1));
	}


	/// <summary>Column widths and order of list views are restored.</summary>
	[Test]
	public void RestoresListColumns()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		ListView list = _CreateList();

		form.Controls.Add(list);
		provider.SetSaveLayout(list, true);
		list.Columns[1].Width = 150;
		list.Columns[2].DisplayIndex = 0;
		provider.Save();

		list.Columns[1].Width = 80;
		list.Columns[0].DisplayIndex = 0;
		provider.Restore();

		Assert.That(list.Columns[1].Width, Is.EqualTo(150).Within(1));
		Assert.That(list.Columns[2].DisplayIndex, Is.EqualTo(0));
		Assert.That(list.Columns[0].DisplayIndex, Is.EqualTo(1));
	}


	/// <summary>Saved column layouts that do not match the columns are ignored.</summary>
	[Test]
	public void IgnoresMismatchedColumns()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		ListView list = _CreateList();

		form.Controls.Add(list);
		provider.SetSaveLayout(list, true);
		provider.Save();

		list.Columns.Add("D", 120);
		list.Columns[0].Width = 60;
		provider.Restore();

		Assert.That(list.Columns[0].Width, Is.EqualTo(60));
	}


	/// <summary>Column widths, order and visibility of data grid views are restored.</summary>
	[Test]
	public void RestoresGridColumns()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		DataGridView grid = _CreateGrid();

		form.Controls.Add(grid);
		provider.SetSaveLayout(grid, true);
		grid.Columns[0].Width = 140;
		grid.Columns[1].Visible = false;
		grid.Columns[2].DisplayIndex = 0;
		provider.Save();

		grid.Columns[0].Width = 70;
		grid.Columns[1].Visible = true;
		grid.Columns[0].DisplayIndex = 0;
		provider.Restore();

		Assert.That(grid.Columns[0].Width, Is.EqualTo(140).Within(1));
		Assert.That(grid.Columns[1].Visible, Is.False);
		Assert.That(grid.Columns[2].DisplayIndex, Is.EqualTo(0));
	}


	/// <summary>The selected tab is restored.</summary>
	[Test]
	public void RestoresSelectedTab()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		TabControl tabs = new() { Name = "_Tabs" };

		tabs.TabPages.Add("One");
		tabs.TabPages.Add("Two");
		tabs.TabPages.Add("Three");
		form.Controls.Add(tabs);
		provider.SetSaveLayout(tabs, true);

		tabs.SelectedIndex = 2;
		provider.Save();
		tabs.SelectedIndex = 0;
		provider.Restore();

		Assert.That(tabs.SelectedIndex, Is.EqualTo(2));
	}


	/// <summary>Saving raises Saving before the commit; handlers can add values. Restoring raises Restored.</summary>
	[Test]
	public void RaisesEvents()
	{
		Form form = _CreateForm(out WindowLayoutProvider provider);
		bool restored = false;

		provider.Saving += (sender, e) => { provider.Store.Write("TestForm", "extra", new int[] { 42 }); };
		provider.Restored += (sender, e) => { restored = true; };

		provider.Save();
		provider.Restore();

		Assert.That(_Store.Values[("TestForm", "extra")], Is.EqualTo(new int[] { 42 }));
		Assert.That(_Store.Commits, Is.EqualTo(1));
		Assert.That(restored, Is.True);
	}


	/// <summary>Disposed controls are no longer saved.</summary>
	[Test]
	public void DisposedControlIsRemoved()
	{
		_CreateForm(out WindowLayoutProvider provider);
		SplitContainer split = _CreateSplit(FixedPanel.None, 400);

		provider.SetSaveLayout(split, true);
		split.Dispose();

		Assert.That(provider.GetSaveLayout(split), Is.False);
	}
}
