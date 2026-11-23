namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="DdnThemeStore"/> class.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class DdnThemeStoreTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary file.</summary>
	private string _File = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_File = Path.Combine(Path.GetTempPath(), "spellbook_themes_" + Guid.NewGuid().ToString("N") + ".themes");
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		if(File.Exists(_File)) { File.Delete(_File); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A missing file returns empty settings; relative paths are relative to the user configuration path.</summary>
	[Test]
	public void LoadsEmptySettings()
	{
		ThemeSettings settings = new DdnThemeStore(_File).Load();

		Assert.That(settings.SelectedTheme, Is.Null);
		Assert.That(settings.Themes, Is.Empty);
		Assert.That(new DdnThemeStore().ConfigFile, Does.EndWith(DdnThemeStore.DEFAULT_FILE_NAME));
		Assert.That(new DdnThemeStore(_File).ConfigFile, Is.EqualTo(_File));
	}


	/// <summary>Themes are saved with their base themes and colors, in any order.</summary>
	[Test]
	public void SavesAndLoadsThemes()
	{
		Theme parent = new("Parent", Theme.LIGHT);
		Theme child = new("Child", parent);
		parent.SetValue(ThemeColor.BACK, Color.Beige);
		child.SetValue(ThemeColor.ACCENT, Color.Purple);

		DdnThemeStore store = new(_File);
		store.Save(new ThemeSettings() { SelectedTheme = child.Id, Themes = new[] { child, parent } });
		ThemeSettings settings = store.Load();

		Assert.That(settings.SelectedTheme, Is.EqualTo(child.Id));
		Assert.That(settings.Themes, Has.Count.EqualTo(2));

		Theme loadedChild = settings.Themes[0];
		Theme loadedParent = settings.Themes[1];
		Assert.That(loadedChild.Name, Is.EqualTo("Child"));
		Assert.That(loadedChild.Base, Is.SameAs(loadedParent));
		Assert.That(loadedParent.Base, Is.SameAs(Theme.LIGHT));
		Assert.That(loadedParent.GetValue(ThemeColor.BACK).ToArgb(), Is.EqualTo(Color.Beige.ToArgb()));
		Assert.That(loadedChild[ThemeColor.BACK].ToArgb(), Is.EqualTo(Color.Beige.ToArgb()));
		Assert.That(loadedChild.GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.Purple.ToArgb()));
		Assert.That(loadedChild.GetValue(ThemeColor.TEXT), Is.EqualTo(Color.Empty));

		store.Save(new ThemeSettings() { SelectedTheme = "dark", Themes = new[] { parent } });
		settings = store.Load();
		Assert.That(settings.SelectedTheme, Is.EqualTo("dark"));
		Assert.That(settings.Themes, Has.Count.EqualTo(1));
	}


	/// <summary>Unknown base themes, invalid entries and invalid colors are tolerated.</summary>
	[Test]
	public void ToleratesInvalidData()
	{
		DdnElement cfg = Ddn.Create();
		cfg["themes/length"] = 5;
		cfg["themes/selected"] = "a";
		cfg["themes/[0]/id"] = "a";
		cfg["themes/[0]/name"] = "A";
		cfg["themes/[0]/base"] = "unknown";
		cfg["themes/[0]/colors/ACCENT"] = "#FF0000";
		cfg["themes/[0]/colors/TEXT"] = "red";
		cfg["themes/[0]/colors/NOPE"] = "#00FF00";
		cfg["themes/[1]/id"] = "dark";
		cfg["themes/[1]/name"] = "Reserved";
		cfg["themes/[2]/name"] = "No id";
		cfg["themes/[3]/id"] = "A";
		cfg["themes/[3]/name"] = "Duplicate";
		cfg["themes/[4]/id"] = "b";
		cfg["themes/[4]/name"] = "B";
		cfg["themes/[4]/base"] = "b";
		cfg.Save(_File);

		ThemeSettings settings = new DdnThemeStore(_File).Load();

		Assert.That(settings.SelectedTheme, Is.EqualTo("a"));
		Assert.That(settings.Themes.Select(m => m.Id), Is.EqualTo(new[] { "a", "b" }));
		Assert.That(settings.Themes[0].Base, Is.SameAs(Theme.AUTO));
		Assert.That(settings.Themes[0].GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
		Assert.That(settings.Themes[0].GetValue(ThemeColor.TEXT), Is.EqualTo(Color.Empty));
		Assert.That(settings.Themes[1].Base, Is.SameAs(Theme.AUTO));
	}
}
