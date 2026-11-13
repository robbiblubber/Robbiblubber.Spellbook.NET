namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="Theme"/> class.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemeTests
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
		ThemeManager._ResetForTest();
		_File = Path.Combine(Path.GetTempPath(), "spellbook_theme_" + Guid.NewGuid().ToString("N") + ".theme");
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		if(File.Exists(_File)) { File.Delete(_File); }

		Theme._SetAutoTargetForTest(null);
		ThemeManager._OnSystemSettingsChanged();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Every built-in theme defines every color.</summary>
	[Test]
	public void BuiltInThemesDefineAllColors()
	{
		foreach(Theme i in ThemeManager.BuiltInThemes)
		{
			foreach(ThemeColor j in Enum.GetValues<ThemeColor>())
			{
				Assert.That(i[j].IsEmpty, Is.False, $"{i}/{j}");
			}
		}
	}


	/// <summary>The light theme is light, the dark theme is dark, and their back colors are the documented ones.</summary>
	[Test]
	public void DetectsDarkThemes()
	{
		Assert.That(Theme.LIGHT.IsDark, Is.False);
		Assert.That(Theme.DARK.IsDark, Is.True);
		Assert.That(Theme.LIGHT[ThemeColor.FIELD_BACK].ToArgb(), Is.EqualTo(Color.White.ToArgb()));
		Assert.That(Theme.DARK[ThemeColor.BACK].ToArgb(), Is.EqualTo(Color.FromArgb(0x20, 0x20, 0x20).ToArgb()));
		Assert.That(Theme.SYSTEM[ThemeColor.FIELD_BACK], Is.EqualTo(SystemColors.Window));
	}


	/// <summary>Built-in themes cannot be changed and have no base theme.</summary>
	[Test]
	public void BuiltInThemesAreReadOnly()
	{
		foreach(Theme i in ThemeManager.BuiltInThemes)
		{
			Assert.That(i.IsBuiltIn, Is.True);
			Assert.That(i.Base, Is.Null);
			Assert.That(i.HasValues, Is.False);
			Assert.That(i.GetValue(ThemeColor.ACCENT), Is.EqualTo(Color.Empty));
			Assert.Throws<InvalidOperationException>(() => i.SetValue(ThemeColor.ACCENT, Color.Red));
			Assert.Throws<InvalidOperationException>(() => i.Name = "Other");
			Assert.Throws<InvalidOperationException>(() => i.Base = Theme.DARK);
			Assert.Throws<InvalidOperationException>(() => i.Reset());
			Assert.Throws<InvalidOperationException>(() => i.Export(_File));
		}

		Assert.That(Theme.DARK.Id, Is.EqualTo("dark"));
		Assert.That(Theme.DARK.ToString(), Is.EqualTo("Dark"));
	}


	/// <summary>A custom theme takes all colors that are not set from its base theme.</summary>
	[Test]
	public void InheritsFromBaseTheme()
	{
		Theme theme = new("Green", Theme.DARK);

		Assert.That(theme.IsBuiltIn, Is.False);
		Assert.That(theme.Base, Is.SameAs(Theme.DARK));
		Assert.That(theme.Id, Has.Length.EqualTo(32));
		Assert.That(theme[ThemeColor.BACK], Is.EqualTo(Theme.DARK[ThemeColor.BACK]));
		Assert.That(theme.IsDark, Is.True);

		theme.SetValue(ThemeColor.ACCENT, Color.LimeGreen);
		Assert.That(theme[ThemeColor.ACCENT], Is.EqualTo(Color.LimeGreen));
		Assert.That(theme.GetValue(ThemeColor.ACCENT), Is.EqualTo(Color.LimeGreen));
		Assert.That(theme.GetValue(ThemeColor.BACK), Is.EqualTo(Color.Empty));
		Assert.That(theme.HasValues, Is.True);

		theme.SetValue(ThemeColor.BACK, Color.White);
		Assert.That(theme.IsDark, Is.False);

		theme.Reset();
		Assert.That(theme.HasValues, Is.False);
		Assert.That(theme[ThemeColor.ACCENT], Is.EqualTo(Theme.DARK[ThemeColor.ACCENT]));
	}


	/// <summary>Changed occurs only on real changes, and themes based on a changed theme are notified as well.</summary>
	[Test]
	public void RaisesChanged()
	{
		Theme parent = new("Parent", Theme.DARK);
		Theme child = new("Child", parent);
		int parentCount = 0;
		int childCount = 0;

		parent.Changed += (sender, e) => { parentCount++; };
		child.Changed += (sender, e) => { childCount++; };

		parent.SetValue(ThemeColor.TEXT, Color.Red);
		parent.SetValue(ThemeColor.TEXT, Color.Red);
		Assert.That(parentCount, Is.EqualTo(1));
		Assert.That(childCount, Is.EqualTo(1));
		Assert.That(child[ThemeColor.TEXT], Is.EqualTo(Color.Red));

		child.Name = "Child 2";
		child.Name = "Child 2";
		Assert.That(parentCount, Is.EqualTo(1));
		Assert.That(childCount, Is.EqualTo(2));

		parent.Base = Theme.LIGHT;
		Assert.That(childCount, Is.EqualTo(3));
		Assert.That(child[ThemeColor.BACK], Is.EqualTo(Theme.LIGHT[ThemeColor.BACK]));
	}


	/// <summary>A theme cannot become its own base theme, directly or indirectly.</summary>
	[Test]
	public void RejectsCycles()
	{
		Theme a = new("A", Theme.DARK);
		Theme b = new("B", a);

		Assert.Throws<ArgumentException>(() => a.Base = a);
		Assert.Throws<ArgumentException>(() => a.Base = b);
		Assert.Throws<ArgumentException>(() => a.CopyFrom(b));
		Assert.Throws<ArgumentNullException>(() => a.Base = null);
		Assert.That(a.Base, Is.SameAs(Theme.DARK));

		Assert.That(b.DependsOn(a), Is.True);
		Assert.That(b.DependsOn(Theme.DARK), Is.True);
		Assert.That(b.DependsOn(Theme.LIGHT), Is.False);
		Assert.That(a.DependsOn(b), Is.False);
	}


	/// <summary>The identifiers of built-in themes are reserved; undefined colors are rejected.</summary>
	[Test]
	public void ValidatesArguments()
	{
		Assert.Throws<ArgumentException>(() => new Theme("dark", "My dark", Theme.DARK));
		Assert.Throws<ArgumentException>(() => new Theme("SYSTEM", "My system", Theme.DARK));
		Assert.Throws<ArgumentException>(() => new Theme(" ", "Empty", Theme.DARK));
		Assert.Throws<ArgumentNullException>(() => new Theme("Name", null!));
		Assert.Throws<ArgumentOutOfRangeException>(() => _ = Theme.DARK[(ThemeColor) 999]);
		Assert.Throws<ArgumentOutOfRangeException>(() => new Theme("A", Theme.DARK).SetValue((ThemeColor) (-1), Color.Red));
	}


	/// <summary>The automatic theme uses the theme of the Windows setting and follows its changes.</summary>
	[Test]
	public void AutoFollowsWindowsSetting()
	{
		Theme custom = new("Custom", Theme.AUTO);
		int count = 0;

		custom.Changed += (sender, e) => { count++; };

		Theme._SetAutoTargetForTest(Theme.DARK);
		ThemeManager._OnSystemSettingsChanged();
		int afterDark = count;

		Assert.That(Theme.AUTO[ThemeColor.BACK], Is.EqualTo(Theme.DARK[ThemeColor.BACK]));
		Assert.That(Theme.AUTO.IsDark, Is.True);
		Assert.That(custom.IsDark, Is.True);
		Assert.That(Theme.AUTO.DependsOn(Theme.DARK), Is.True);

		Theme._SetAutoTargetForTest(Theme.LIGHT);
		ThemeManager._OnSystemSettingsChanged();

		Assert.That(Theme.AUTO[ThemeColor.BACK], Is.EqualTo(Theme.LIGHT[ThemeColor.BACK]));
		Assert.That(custom.IsDark, Is.False);
		Assert.That(count, Is.EqualTo(afterDark + 1));
	}


	/// <summary>Copy creates a new theme with the same colors; CopyFrom copies base and colors but not the name.</summary>
	[Test]
	public void CopiesThemes()
	{
		Theme source = new("Source", Theme.DARK);
		source.SetValue(ThemeColor.ACCENT, Color.Orange);

		Theme copy = source.Copy("Copy");
		Assert.That(copy.Id, Is.Not.EqualTo(source.Id));
		Assert.That(copy.Name, Is.EqualTo("Copy"));
		Assert.That(copy.Base, Is.SameAs(Theme.DARK));
		Assert.That(copy.GetValue(ThemeColor.ACCENT), Is.EqualTo(Color.Orange));

		Theme target = new("Target", Theme.LIGHT);
		target.SetValue(ThemeColor.TEXT, Color.Red);
		target.CopyFrom(source);
		Assert.That(target.Name, Is.EqualTo("Target"));
		Assert.That(target.Base, Is.SameAs(Theme.DARK));
		Assert.That(target.GetValue(ThemeColor.ACCENT), Is.EqualTo(Color.Orange));
		Assert.That(target.GetValue(ThemeColor.TEXT), Is.EqualTo(Color.Empty));

		target.CopyFrom(Theme.LIGHT);
		Assert.That(target.Base, Is.SameAs(Theme.LIGHT));
		Assert.That(target.HasValues, Is.False);

		Theme empty = Theme.DARK.Copy("Empty");
		Assert.That(empty.Base, Is.SameAs(Theme.DARK));
		Assert.That(empty.HasValues, Is.False);
	}


	/// <summary>Export and import keep name, base theme and colors, including transparency.</summary>
	[Test]
	public void ExportsAndImports()
	{
		Theme parent = new("Parent", Theme.DARK);
		Theme theme = new("Exported", parent);
		ThemeManager.Add(parent);

		theme.SetValue(ThemeColor.ACCENT, Color.FromArgb(255, 0x12, 0x34, 0x56));
		theme.SetValue(ThemeColor.SELECTION_BACK, Color.FromArgb(0x80, 0x10, 0x20, 0x30));
		theme.Export(_File);

		Theme imported = Theme.Import(_File);
		Assert.That(imported.Id, Is.EqualTo(theme.Id));
		Assert.That(imported.Name, Is.EqualTo("Exported"));
		Assert.That(imported.Base, Is.SameAs(parent));
		Assert.That(imported.GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.FromArgb(255, 0x12, 0x34, 0x56).ToArgb()));
		Assert.That(imported.GetValue(ThemeColor.SELECTION_BACK).ToArgb(), Is.EqualTo(Color.FromArgb(0x80, 0x10, 0x20, 0x30).ToArgb()));
		Assert.That(imported.GetValue(ThemeColor.TEXT), Is.EqualTo(Color.Empty));

		ThemeManager.Add(imported);
		Theme second = Theme.Import(_File);
		Assert.That(second.Id, Is.Not.EqualTo(theme.Id));
		Assert.That(second.Name, Is.EqualTo("Exported"));
		Assert.That(second.GetValue(ThemeColor.ACCENT).ToArgb(), Is.EqualTo(Color.FromArgb(255, 0x12, 0x34, 0x56).ToArgb()));

		ThemeManager.Remove(parent);
		ThemeManager.Remove(imported);
		Theme orphan = Theme.Import(_File);
		Assert.That(orphan.Base, Is.SameAs(Theme.AUTO));
	}


	/// <summary>Import rejects files without a theme.</summary>
	[Test]
	public void ImportRejectsInvalidFiles()
	{
		File.WriteAllText(_File, "other { value = 1; }");
		Assert.Throws<InvalidDataException>(() => Theme.Import(_File));
	}


	/// <summary>System colors are mapped to theme colors.</summary>
	[Test]
	public void MapsSystemColors()
	{
		Assert.That(Theme.DARK.MapSystemColor(SystemColors.Window), Is.EqualTo(Theme.DARK[ThemeColor.FIELD_BACK]));
		Assert.That(Theme.DARK.MapSystemColor(SystemColors.Highlight), Is.EqualTo(Theme.DARK[ThemeColor.SELECTION_BACK]));
		Assert.That(Theme.DARK.MapSystemColor(Color.Red), Is.EqualTo(Color.Red));
		Assert.That(Theme.SYSTEM.IsSystem, Is.True);
		Assert.That(Theme.DARK.IsSystem, Is.False);
		Assert.That(new Theme("Custom", Theme.SYSTEM).IsSystem, Is.False);
	}


	/// <summary>Lightening icons inverts the brightness and keeps hue, saturation and alpha.</summary>
	[Test]
	public void InvertsBrightness()
	{
		Color gray = _ThemeImages._Invert(Color.FromArgb(128, 32, 32, 32));
		Assert.That(gray.A, Is.EqualTo(128));
		Assert.That(gray.GetBrightness(), Is.EqualTo(1f - Color.FromArgb(32, 32, 32).GetBrightness()).Within(0.01f));
		Assert.That(gray.R, Is.EqualTo(gray.G));

		Color blue = Color.FromArgb(0, 0, 139);
		Color light = _ThemeImages._Invert(blue);
		Assert.That(light.GetHue(), Is.EqualTo(blue.GetHue()).Within(1f));
		Assert.That(light.GetBrightness(), Is.EqualTo(1f - blue.GetBrightness()).Within(0.01f));
	}
}
