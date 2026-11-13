namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="SharedThemeStore"/> class and <see cref="ThemeManager.Refresh"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class SharedThemeStoreTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary directory.</summary>
	private string _Directory = null!;

	/// <summary>Theme directory.</summary>
	private string _Themes = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a store for an application.</summary>
	/// <param name="application">Application name.</param>
	/// <returns>Returns the store.</returns>
	private SharedThemeStore _Store(string application)
	{
		return new SharedThemeStore(_Themes, Path.Combine(_Directory, application + ".themes"));
	}


	/// <summary>Gets a theme by its name.</summary>
	/// <param name="settings">Settings.</param>
	/// <param name="name">Name.</param>
	/// <returns>Returns the theme.</returns>
	private static Theme _Get(ThemeSettings settings, string name)
	{
		return settings.Themes.Single(m => m.Name == name);
	}


	/// <summary>Gets the names of the theme files.</summary>
	/// <returns>Returns the names of the themes in the theme files, sorted.</returns>
	private string[] _FileThemes()
	{
		return Directory.GetFiles(_Themes, "*.theme").Select(m => (string) Ddn.Open(m)["theme/name"]).Order().ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();
		_Directory = Path.Combine(Path.GetTempPath(), "spellbook_shared_" + Guid.NewGuid().ToString("N"));
		_Themes = Path.Combine(_Directory, "themes");
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager._ResetForTest();
		if(Directory.Exists(_Directory)) { Directory.Delete(_Directory, true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A missing theme directory returns empty settings; the default directory is in the roaming application data.</summary>
	[Test]
	public void LoadsEmptySettings()
	{
		ThemeSettings settings = _Store("a").Load();

		Assert.That(settings.SelectedTheme, Is.Null);
		Assert.That(settings.Themes, Is.Empty);
		Assert.That(new SharedThemeStore().ThemeDirectory, Is.EqualTo(SharedThemeStore.DefaultDirectory));
		Assert.That(SharedThemeStore.DefaultDirectory, Does.EndWith(Path.Combine("robbiblubber", "themes")));
		Assert.That(new SharedThemeStore().ConfigFile, Does.EndWith(SharedThemeStore.DEFAULT_FILE_NAME));
	}


	/// <summary>Each theme is a file in the theme directory; each application has its own selected theme.</summary>
	[Test]
	public void SharesThemesBetweenApplications()
	{
		Theme parent = new("Parent", Theme.LIGHT);
		Theme child = new("Child", parent);
		child.SetValue(ThemeColor.ACCENT, Color.Purple);

		_Store("a").Save(new ThemeSettings() { SelectedTheme = child.Id, Themes = new[] { child, parent } });
		Assert.That(_FileThemes(), Is.EqualTo(new[] { "Child", "Parent" }));

		SharedThemeStore b = _Store("b");
		ThemeSettings settings = b.Load();
		Assert.That(settings.SelectedTheme, Is.Null);
		Assert.That(_Get(settings, "Child").Base, Is.SameAs(_Get(settings, "Parent")));
		Assert.That(_Get(settings, "Child").GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.Purple.ToArgb()));

		b.Save(new ThemeSettings() { SelectedTheme = "dark", Themes = settings.Themes });
		Assert.That(_Store("a").Load().SelectedTheme, Is.EqualTo(child.Id));
		Assert.That(_Store("b").Load().SelectedTheme, Is.EqualTo("dark"));

		Theme imported = Theme.Import(Directory.GetFiles(_Themes, "*.theme").First());
		Assert.That(imported.Name, Is.AnyOf("Child", "Parent"));
	}


	/// <summary>Saving keeps the changes that other applications have saved in the meantime.</summary>
	[Test]
	public void KeepsChangesOfOtherApplications()
	{
		Theme one = new("One", Theme.DARK);
		Theme two = new("Two", Theme.DARK);
		Theme three = new("Three", Theme.DARK);
		_Store("setup").Save(new ThemeSettings() { Themes = new[] { one, two, three } });

		SharedThemeStore a = _Store("a");
		SharedThemeStore b = _Store("b");
		ThemeSettings sa = a.Load();
		ThemeSettings sb = b.Load();

		Theme four = new("Four", Theme.LIGHT);
		a.Save(new ThemeSettings() { Themes = sa.Themes.Where(m => m.Name != "Two").Append(four).ToArray() });
		Assert.That(_FileThemes(), Is.EqualTo(new[] { "Four", "One", "Three" }));

		_Get(sb, "Three").SetValue(ThemeColor.BACK, Color.Navy);
		b.Save(new ThemeSettings() { Themes = sb.Themes });
		Assert.That(_FileThemes(), Is.EqualTo(new[] { "Four", "One", "Three" }));

		a.Save(new ThemeSettings() { Themes = sa.Themes.Where(m => m.Name != "Two").Append(four).ToArray() });
		Assert.That(_Get(_Store("c").Load(), "Three").GetValue(ThemeColor.BACK).ToArgb(), Is.EqualTo(Color.Navy.ToArgb()));
	}


	/// <summary>Themes in the configuration file of the application (stored by <see cref="DdnThemeStore"/>) are taken over.</summary>
	[Test]
	public void TakesOverApplicationThemes()
	{
		string config = Path.Combine(_Directory, "old.themes");
		Directory.CreateDirectory(_Directory);
		Theme old = new("Old", Theme.DARK);
		new DdnThemeStore(config).Save(new ThemeSettings() { SelectedTheme = old.Id, Themes = new[] { old } });

		SharedThemeStore store = new(_Themes, config);
		ThemeSettings settings = store.Load();
		Assert.That(settings.Themes.Select(m => m.Name), Is.EqualTo(new[] { "Old" }));
		Assert.That(settings.SelectedTheme, Is.EqualTo(old.Id));

		store.Save(settings);
		Assert.That(_FileThemes(), Is.EqualTo(new[] { "Old" }));
		Assert.That(new DdnThemeStore(config).Load().Themes, Is.Empty);
		Assert.That(store.Load().SelectedTheme, Is.EqualTo(old.Id));
	}


	/// <summary>Refresh takes over themes that other applications have added, changed or removed.</summary>
	[Test]
	public void RefreshTakesOverChanges()
	{
		Theme one = new("One", Theme.DARK);
		Theme two = new("Two", Theme.DARK);
		Theme three = new("Three", Theme.DARK);
		Theme mine = new("Mine", Theme.DARK);
		_Store("setup").Save(new ThemeSettings() { Themes = new[] { one, two, three, mine } });

		ThemeManager.Load(_Store("a"));
		Theme local = ThemeManager.CustomThemes.Single(m => m.Name == "One");
		Theme localMine = ThemeManager.CustomThemes.Single(m => m.Name == "Mine");
		ThemeManager.Theme = local;
		localMine.SetValue(ThemeColor.TEXT, Color.Red);

		SharedThemeStore b = _Store("b");
		ThemeSettings sb = b.Load();
		_Get(sb, "One").SetValue(ThemeColor.ACCENT, Color.Orange);
		_Get(sb, "Mine").SetValue(ThemeColor.TEXT, Color.Blue);
		Theme added = new("Added", _Get(sb, "One"));
		b.Save(new ThemeSettings() { Themes = sb.Themes.Where(m => m.Name != "Two").Append(added).ToArray() });

		ThemeManager.Refresh();

		Assert.That(ThemeManager.CustomThemes.Select(m => m.Name).Order(), Is.EqualTo(new[] { "Added", "Mine", "One", "Three" }));
		Assert.That(ThemeManager.CustomThemes.Single(m => m.Name == "One"), Is.SameAs(local));
		Assert.That(local.GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.Orange.ToArgb()));
		Assert.That(ThemeManager.Theme[ThemeColor.ACCENT].ToArgb(), Is.EqualTo(Color.Orange.ToArgb()));
		Assert.That(ThemeManager.CustomThemes.Single(m => m.Name == "Added").Base, Is.SameAs(local));
		Assert.That(localMine.GetValue(ThemeColor.TEXT).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

		ThemeManager.Save();
		Assert.That(_Get(_Store("c").Load(), "Mine").GetValue(ThemeColor.TEXT).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
		Assert.That(_FileThemes(), Is.EqualTo(new[] { "Added", "Mine", "One", "Three" }));
	}


	/// <summary>If another application has removed the selected theme, the next base theme that still exists is selected.</summary>
	[Test]
	public void SelectionFallsBackToBase()
	{
		Theme root = new("Base 1", Theme.DARK);
		Theme shared = new("Shared 1", root);
		_Store("a").Save(new ThemeSettings() { SelectedTheme = shared.Id, Themes = new[] { root, shared } });

		SharedThemeStore b = _Store("b");
		b.Save(new ThemeSettings() { SelectedTheme = shared.Id, Themes = b.Load().Themes });

		SharedThemeStore a = _Store("a");
		ThemeSettings sa = a.Load();
		a.Save(new ThemeSettings() { SelectedTheme = sa.SelectedTheme, Themes = sa.Themes.Where(m => m.Name != "Shared 1").ToArray() });
		Assert.That(_Store("b").Load().SelectedTheme, Is.EqualTo(root.Id));

		_Store("setup").Save(new ThemeSettings() { Themes = Array.Empty<Theme>() });
		a = _Store("a");
		a.Save(new ThemeSettings() { Themes = Array.Empty<Theme>() });
		File.Delete(Directory.GetFiles(_Themes, "*.theme").Single());
		Assert.That(_Store("b").Load().SelectedTheme, Is.EqualTo("dark"));
	}


	/// <summary>Loading falls back to the next base theme when the theme of the application has been removed by another application.</summary>
	[Test]
	public void LoadFallsBackToBase()
	{
		Theme root = new("Base 1", Theme.DARK);
		Theme shared = new("Shared 1", root);
		_Store("setup").Save(new ThemeSettings() { Themes = new[] { root, shared } });

		ThemeManager.Load(_Store("b"));
		ThemeManager.Theme = ThemeManager.Find(shared.Id)!;

		SharedThemeStore a = _Store("a");
		ThemeSettings sa = a.Load();
		a.Save(new ThemeSettings() { Themes = sa.Themes.Where(m => m.Name != "Shared 1").ToArray() });

		ThemeManager.Load(_Store("b"));
		Assert.That(ThemeManager.CustomThemes.Select(m => m.Name), Is.EqualTo(new[] { "Base 1" }));
		Assert.That(ThemeManager.Theme, Is.SameAs(ThemeManager.Find(root.Id)));
	}


	/// <summary>Refresh does nothing for stores that do not share their themes.</summary>
	[Test]
	public void RefreshIgnoresOtherStores()
	{
		Directory.CreateDirectory(_Directory);
		string config = Path.Combine(_Directory, "app.themes");
		Theme theme = new("Theme", Theme.DARK);
		ThemeManager.Add(theme);
		ThemeManager.Store = new DdnThemeStore(config);

		ThemeManager.Refresh();
		Assert.That(ThemeManager.CustomThemes, Is.EqualTo(new[] { theme }));
	}
}
