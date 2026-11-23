namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ProgressMeterColors._Slot;



/// <summary>This class tests the <see cref="ThemeManager"/> class and the theme support of color tables.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemeManagerTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary file.</summary>
	private string _File = null!;

	/// <summary>Number of ThemeChanged events.</summary>
	private int _ThemeChanges = 0;

	/// <summary>Number of ThemesChanged events.</summary>
	private int _ThemesChanges = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates color tables that are not referenced afterwards.</summary>
	/// <param name="count">Number of tables.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void _CreateGarbage(int count)
	{
		for(int i = 0; i < count; i++) { _ = new ProgressMeterColors(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();
		_File = Path.Combine(Path.GetTempPath(), "spellbook_themes_" + Guid.NewGuid().ToString("N") + ".themes");

		_ThemeChanges = 0;
		_ThemesChanges = 0;
		ThemeManager.ThemeChanged += _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged += _ThemeManager_ThemesChanged;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager.ThemeChanged -= _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged -= _ThemeManager_ThemesChanged;
		ThemeManager._ResetForTest();

		if(File.Exists(_File)) { File.Delete(_File); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The application uses the automatic theme by default; the theme lists contain the built-in themes.</summary>
	[Test]
	public void DefaultsToAutomaticTheme()
	{
		Assert.That(ThemeManager.Theme, Is.SameAs(Theme.AUTO));
		Assert.That(ThemeManager.BuiltInThemes, Is.EqualTo(new[] { Theme.SYSTEM, Theme.AUTO, Theme.LIGHT, Theme.DARK }));
		Assert.That(ThemeManager.CustomThemes, Is.Empty);
		Assert.That(ThemeManager.Themes, Has.Count.EqualTo(4));
		Assert.That(ThemeManager.Find("DARK"), Is.SameAs(Theme.DARK));
		Assert.That(ThemeManager.Find("unknown"), Is.Null);
		Assert.Throws<ArgumentNullException>(() => ThemeManager.Theme = null!);
	}


	/// <summary>Color tables without a theme follow the theme of the application; tables with a theme do not.</summary>
	[Test]
	public void ColorTablesFollowApplicationTheme()
	{
		ProgressMeterColors application = new();
		ProgressMeterColors own = new() { Theme = Theme.LIGHT };
		int applicationCount = 0;
		int ownCount = 0;

		application.Changed += (sender, e) => { applicationCount++; };
		own.Changed += (sender, e) => { ownCount++; };

		ThemeManager.Theme = Theme.DARK;
		ThemeManager.Theme = Theme.DARK;

		Assert.That(_ThemeChanges, Is.EqualTo(1));
		Assert.That(applicationCount, Is.EqualTo(1));
		Assert.That(ownCount, Is.EqualTo(0));
		Assert.That(application.EffectiveTheme, Is.SameAs(Theme.DARK));
		Assert.That(application._Get(Slot.BACK_COLOR), Is.EqualTo(Theme.DARK[ThemeColor.TRACK_BACK]));
		Assert.That(own._Get(Slot.BACK_COLOR), Is.EqualTo(Theme.LIGHT[ThemeColor.TRACK_BACK]));
	}


	/// <summary>Changes of a custom theme reach the application and all color tables that use it.</summary>
	[Test]
	public void ColorTablesFollowThemeChanges()
	{
		Theme parent = new("Parent", Theme.DARK);
		Theme child = new("Child", parent);
		ProgressMeterColors application = new();
		ProgressMeterColors own = new() { Theme = child };
		ProgressMeterColors other = new() { Theme = Theme.LIGHT };
		int applicationCount = 0;
		int ownCount = 0;
		int otherCount = 0;

		ThemeManager.Add(parent);
		ThemeManager.Add(child);
		ThemeManager.Theme = parent;
		_ThemeChanges = 0;

		application.Changed += (sender, e) => { applicationCount++; };
		own.Changed += (sender, e) => { ownCount++; };
		other.Changed += (sender, e) => { otherCount++; };

		parent.SetValue(ThemeColor.ACCENT, Color.Orange);

		Assert.That(_ThemeChanges, Is.EqualTo(1));
		Assert.That(applicationCount, Is.EqualTo(1));
		Assert.That(ownCount, Is.EqualTo(1));
		Assert.That(otherCount, Is.EqualTo(0));
		Assert.That(own._Get(Slot.BAR_COLOR), Is.EqualTo(Color.Orange));

		child.SetValue(ThemeColor.ACCENT, Color.Green);
		Assert.That(_ThemeChanges, Is.EqualTo(1));
		Assert.That(applicationCount, Is.EqualTo(1));
		Assert.That(ownCount, Is.EqualTo(2));
		Assert.That(own._Get(Slot.BAR_COLOR), Is.EqualTo(Color.Green));
		Assert.That(application._Get(Slot.BAR_COLOR), Is.EqualTo(Color.Orange));
	}


	/// <summary>A change of the Windows setting reaches color tables that use the automatic theme.</summary>
	[Test]
	public void ColorTablesFollowWindowsSetting()
	{
		try
		{
			Theme._SetAutoTargetForTest(Theme.LIGHT);
			ThemeManager._OnSystemSettingsChanged();

			ProgressMeterColors colors = new() { Theme = Theme.AUTO };
			int count = 0;
			colors.Changed += (sender, e) => { count++; };

			Theme._SetAutoTargetForTest(Theme.DARK);
			ThemeManager._OnSystemSettingsChanged();

			Assert.That(count, Is.EqualTo(1));
			Assert.That(colors._Get(Slot.BACK_COLOR), Is.EqualTo(Theme.DARK[ThemeColor.TRACK_BACK]));
		}
		finally
		{
			Theme._SetAutoTargetForTest(null);
			ThemeManager._OnSystemSettingsChanged();
		}
	}


	/// <summary>The theme manager does not keep color tables alive.</summary>
	[Test]
	public void DoesNotKeepColorTablesAlive()
	{
		_CreateGarbage(1000);
		int before = ThemeManager._GetTableCountForTest();

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
		ThemeManager.Theme = Theme.DARK;

		Assert.That(ThemeManager._GetTableCountForTest(), Is.LessThan(before));
	}


	/// <summary>Custom themes are added and removed; dependent themes and the application theme fall back to the base theme.</summary>
	[Test]
	public void AddsAndRemovesThemes()
	{
		Theme parent = new("Parent", Theme.DARK);
		Theme child = new("Child", parent);
		Theme unregistered = new("Unregistered", Theme.LIGHT);

		Assert.Throws<ArgumentException>(() => ThemeManager.Add(child));
		Assert.Throws<ArgumentException>(() => ThemeManager.Add(Theme.DARK));

		ThemeManager.Add(parent);
		ThemeManager.Add(child);
		ThemeManager.Add(child);
		Assert.That(ThemeManager.CustomThemes, Is.EqualTo(new[] { parent, child }));
		Assert.That(ThemeManager.Find(child.Id), Is.SameAs(child));
		Assert.That(_ThemesChanges, Is.EqualTo(2));
		Assert.Throws<ArgumentException>(() => ThemeManager.Add(new Theme(parent.Id, "Duplicate", Theme.DARK)));

		ThemeManager.Theme = child;
		Assert.That(ThemeManager.Remove(parent), Is.True);
		Assert.That(child.Base, Is.SameAs(Theme.DARK));
		Assert.That(ThemeManager.Theme, Is.SameAs(child));

		Assert.That(ThemeManager.Remove(child), Is.True);
		Assert.That(ThemeManager.Theme, Is.SameAs(Theme.DARK));
		Assert.That(ThemeManager.Remove(unregistered), Is.False);
		Assert.That(ThemeManager.CustomThemes, Is.Empty);
		Assert.That(_ThemesChanges, Is.EqualTo(4));
	}


	/// <summary>Renaming a registered theme raises ThemesChanged, so theme lists show the new name.</summary>
	[Test]
	public void RenamingRaisesThemesChanged()
	{
		Theme theme = new("Old", Theme.DARK);
		Theme unregistered = new("Unregistered", Theme.DARK);
		ThemeManager.Add(theme);
		_ThemesChanges = 0;

		theme.Name = "New";
		Assert.That(_ThemesChanges, Is.EqualTo(1));
		theme.Name = "New";
		unregistered.Name = "Other";
		Assert.That(_ThemesChanges, Is.EqualTo(1));
	}


	/// <summary>Save and Load restore the custom themes and the selected theme.</summary>
	[Test]
	public void SavesAndLoads()
	{
		Theme parent = new("Parent", Theme.DARK);
		Theme child = new("Child", parent);
		parent.SetValue(ThemeColor.ACCENT, Color.Orange);
		child.SetValue(ThemeColor.TEXT, Color.Yellow);

		ThemeManager.Add(parent);
		ThemeManager.Add(child);
		ThemeManager.Theme = child;
		ThemeManager.Save(new DdnThemeStore(_File));

		ThemeManager._ResetForTest();
		ThemeManager.Load(new DdnThemeStore(_File));

		Assert.That(ThemeManager.CustomThemes, Has.Count.EqualTo(2));
		Theme loaded = ThemeManager.Theme;
		Assert.That(loaded.Id, Is.EqualTo(child.Id));
		Assert.That(loaded.Name, Is.EqualTo("Child"));
		Assert.That(loaded.Base!.Id, Is.EqualTo(parent.Id));
		Assert.That(loaded.Base.Base, Is.SameAs(Theme.DARK));
		Assert.That(loaded[ThemeColor.ACCENT].ToArgb(), Is.EqualTo(Color.Orange.ToArgb()));
		Assert.That(loaded[ThemeColor.TEXT].ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));
		Assert.That(ThemeManager.Store, Is.InstanceOf<DdnThemeStore>());

		ThemeManager.Theme = Theme.LIGHT;
		ThemeManager.Save();
		ThemeManager._ResetForTest();
		ThemeManager.Load(new DdnThemeStore(_File));
		Assert.That(ThemeManager.Theme, Is.SameAs(Theme.LIGHT));
	}


	/// <summary>The designer offers the built-in themes and serializes them as references to their fields.</summary>
	[Test]
	public void ConvertsThemesForTheDesigner()
	{
		TypeConverter converter = TypeDescriptor.GetConverter(typeof(Theme));

		Assert.That(converter.GetStandardValuesSupported(), Is.True);
		Assert.That(converter.GetStandardValues()!.Cast<Theme?>(), Is.EqualTo(new Theme?[] { null, Theme.SYSTEM, Theme.AUTO, Theme.LIGHT, Theme.DARK }));
		Assert.That(converter.ConvertToString(Theme.DARK), Is.EqualTo("Dark"));
		Assert.That(converter.ConvertToString(null), Is.EqualTo("(Application)"));
		Assert.That(converter.ConvertFromString("Light"), Is.SameAs(Theme.LIGHT));
		Assert.That(converter.ConvertFromString("(Application)"), Is.Null);

		InstanceDescriptor descriptor = (InstanceDescriptor) converter.ConvertTo(Theme.DARK, typeof(InstanceDescriptor))!;
		Assert.That(descriptor.MemberInfo, Is.InstanceOf<FieldInfo>());
		Assert.That(descriptor.MemberInfo!.Name, Is.EqualTo("DARK"));
		Assert.That(descriptor.Invoke(), Is.SameAs(Theme.DARK));

		Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Theme("Custom", Theme.DARK), typeof(InstanceDescriptor)));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ThemeChanged event of the theme manager.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemeChanged(object? sender, EventArgs e)
	{
		_ThemeChanges++;
	}


	/// <summary>Handles the ThemesChanged event of the theme manager.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemesChanged(object? sender, EventArgs e)
	{
		_ThemesChanges++;
	}


	/// <summary>Single-colored dark images are lightened, colored images are kept.</summary>
	[Test]
	public void LightensDarkImages()
	{
		using Bitmap dark = new(8, 8, PixelFormat.Format32bppArgb);
		using Bitmap red = new(8, 8, PixelFormat.Format32bppArgb);
		using(Graphics g = Graphics.FromImage(dark)) { g.Clear(Color.FromArgb(32, 32, 32)); }
		using(Graphics g = Graphics.FromImage(red)) { g.Clear(Color.Red); }

		Image light = ThemeManager._ResolveImage(null, dark, Theme.DARK);
		Assert.That(light, Is.Not.SameAs(dark));
		Assert.That(((Bitmap) light).GetPixel(4, 4).GetBrightness(), Is.GreaterThan(0.8f));
		Assert.That(ThemeManager._ResolveImage(null, dark, Theme.DARK), Is.SameAs(light));
		Assert.That(ThemeManager._ResolveImage(null, red, Theme.DARK), Is.SameAs(red));
		Assert.That(ThemeManager._ResolveImage(null, dark, Theme.LIGHT), Is.SameAs(dark));

		EventHandler<ThemeImageEventArgs> handler = (sender, e) => { e.Image = red; };
		ThemeManager.ResolveImage += handler;
		try
		{
			Assert.That(ThemeManager._ResolveImage(null, dark, Theme.DARK), Is.SameAs(red));
		}
		finally { ThemeManager.ResolveImage -= handler; }
	}


	/// <summary>The dark outlines of colored images are lightened in dark themes; their colors are kept.</summary>
	[Test]
	public void LightensDarkOutlines()
	{
		using Bitmap icon = new(8, 8, PixelFormat.Format32bppArgb);
		using(Graphics g = Graphics.FromImage(icon))
		{
			g.Clear(Color.Transparent);
			g.FillRectangle(Brushes.Red, 1, 1, 6, 6);
		}
		icon.SetPixel(0, 0, Color.Black);
		icon.SetPixel(1, 0, Color.FromArgb(128, 0, 0, 0));

		Bitmap light = (Bitmap) ThemeManager._ResolveImage(null, icon, Theme.DARK);

		Assert.That(light, Is.Not.SameAs(icon));
		Assert.That(light.GetPixel(0, 0).GetBrightness(), Is.GreaterThan(0.7f));
		Assert.That(light.GetPixel(1, 0).A, Is.GreaterThan(128));
		Assert.That(light.GetPixel(4, 4).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
		Assert.That(light.GetPixel(7, 7).A, Is.EqualTo(0));
		Assert.That(ThemeManager._ResolveImage(null, icon, Theme.LIGHT), Is.SameAs(icon));
	}


	/// <summary>A composed image (image with overlay, like a pinned recent entry) is composed again of its resolved parts in dark
	///          themes, so that a handler of <see cref="ThemeManager.ResolveImage"/> can replace the image below the overlay.</summary>
	[Test]
	public void ResolvesOverlayParts()
	{
		using Bitmap icon = new(8, 8, PixelFormat.Format32bppArgb);
		using Bitmap dark = new(8, 8, PixelFormat.Format32bppArgb);
		using Bitmap pin = new(8, 8, PixelFormat.Format32bppArgb);
		using Bitmap composed = new(8, 8, PixelFormat.Format32bppArgb);
		using(Graphics g = Graphics.FromImage(icon)) { g.Clear(Color.Red); }
		using(Graphics g = Graphics.FromImage(dark)) { g.Clear(Color.Lime); }
		pin.SetPixel(0, 0, Color.Blue);
		using(Graphics g = Graphics.FromImage(composed))
		{
			g.DrawImage(icon, 0, 0);
			g.DrawImage(pin, 0, 0);
		}

		EventHandler<ThemeImageEventArgs> handler = (sender, e) => { if(e.Theme.IsDark && (e.Original == icon)) { e.Image = dark; } };
		_ThemeImages._RegisterOverlay(composed, icon, pin);
		ThemeManager.ResolveImage += handler;
		try
		{
			Assert.That(ThemeManager._ResolveImage(null, composed, Theme.LIGHT), Is.SameAs(composed));

			Bitmap result = (Bitmap) ThemeManager._ResolveImage(null, composed, Theme.DARK);
			Assert.That(result, Is.Not.SameAs(composed));
			Assert.That(result.GetPixel(4, 4).ToArgb(), Is.EqualTo(Color.Lime.ToArgb()));
			Assert.That(result.GetPixel(0, 0).ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
			Assert.That(ThemeManager._ResolveImage(null, composed, Theme.DARK), Is.SameAs(result));

			_ThemeImages._RemoveOverlay(composed);
			Assert.That(((Bitmap) ThemeManager._ResolveImage(null, composed, Theme.DARK)).GetPixel(4, 4).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
		}
		finally
		{
			ThemeManager.ResolveImage -= handler;
			_ThemeImages._RemoveOverlay(composed);
		}
	}
}
