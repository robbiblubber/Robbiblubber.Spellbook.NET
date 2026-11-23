namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ThemedComboBoxColors._Slot;



/// <summary>This class tests the <see cref="ThemedComboBoxColors"/> class.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxColorsTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new color table uses the theme of the application and has no overrides.</summary>
	[Test]
	public void DefaultsToApplicationTheme()
	{
		ThemedComboBoxColors colors = new();

		Assert.That(colors.IsDefault, Is.True);
		Assert.That(colors.Theme, Is.Null);
		Assert.That(colors.EffectiveTheme, Is.SameAs(ThemeManager.Theme));
		Assert.That(colors.BackColor, Is.EqualTo(Color.Empty));
		Assert.That(colors._Get(Slot.BACK_COLOR), Is.EqualTo(ThemeManager.Theme[ThemeColor.FIELD_BACK]));
		Assert.That(colors._Get(Slot.SELECTION_BACK_COLOR), Is.EqualTo(ThemeManager.Theme[ThemeColor.SELECTION_BACK]));
		Assert.That(colors.ToString(), Is.EqualTo("Application"));
	}


	/// <summary>Every built-in theme defines every color.</summary>
	[Test]
	public void ThemesDefineAllColors()
	{
		ThemedComboBoxColors colors = new();

		foreach(Theme i in ThemeManager.BuiltInThemes)
		{
			colors.Theme = i;
			foreach(Slot j in Enum.GetValues<Slot>())
			{
				Assert.That(colors._Get(j).IsEmpty, Is.False, $"{i}/{j}");
			}
		}
	}


	/// <summary>The built-in themes do not change the background on hover: the hot back color equals the back color.</summary>
	[Test]
	public void HotBackColorDefaultsToBackColor()
	{
		ThemedComboBoxColors colors = new();

		foreach(Theme i in ThemeManager.BuiltInThemes)
		{
			colors.Theme = i;
			Assert.That(colors._Get(Slot.HOT_BACK_COLOR).ToArgb(), Is.EqualTo(colors._Get(Slot.BACK_COLOR).ToArgb()), i.ToString());
		}
	}


	/// <summary>Overrides take precedence over the theme; the other colors still come from the theme.</summary>
	[Test]
	public void OverridesTakePrecedence()
	{
		ThemedComboBoxColors colors = new() { Theme = Theme.DARK };
		Color dark = colors._Get(Slot.BACK_COLOR);

		colors.FocusColor = Color.Orange;

		Assert.That(dark.ToArgb(), Is.EqualTo(Color.FromArgb(0x2B, 0x2B, 0x2B).ToArgb()));
		Assert.That(colors._Get(Slot.FOCUS_COLOR), Is.EqualTo(Color.Orange));
		Assert.That(colors._Get(Slot.BACK_COLOR), Is.EqualTo(dark));
		Assert.That(colors.IsDefault, Is.False);
		Assert.That(colors.ToString(), Is.EqualTo("Dark, 1 custom"));

		colors.Theme = Theme.LIGHT;
		Assert.That(colors._Get(Slot.FOCUS_COLOR), Is.EqualTo(Color.Orange));
		Assert.That(colors._Get(Slot.BACK_COLOR).ToArgb(), Is.EqualTo(Color.White.ToArgb()));
	}


	/// <summary>Changed is raised when the theme or a color changes, but not when a value is set again.</summary>
	[Test]
	public void RaisesChanged()
	{
		ThemedComboBoxColors colors = new();
		int count = 0;

		colors.Changed += (sender, e) => { count++; };

		colors.Theme = Theme.DARK;
		colors.Theme = Theme.DARK;
		colors.BorderColor = Color.Red;
		colors.BorderColor = Color.Red;
		colors.BorderColor = Color.Empty;

		Assert.That(count, Is.EqualTo(3));
	}


	/// <summary>Reset restores the defaults; CopyFrom copies theme and overrides.</summary>
	[Test]
	public void ResetsAndCopies()
	{
		ThemedComboBoxColors source = new() { Theme = Theme.LIGHT, DetailForeColor = Color.Green };
		ThemedComboBoxColors target = new();
		int count = 0;

		target.Changed += (sender, e) => { count++; };
		target.CopyFrom(source);

		Assert.That(target.Theme, Is.SameAs(Theme.LIGHT));
		Assert.That(target.DetailForeColor, Is.EqualTo(Color.Green));
		Assert.That(count, Is.EqualTo(1));

		target.Reset();
		Assert.That(target.IsDefault, Is.True);
		Assert.That(target.DetailForeColor, Is.EqualTo(Color.Empty));
		Assert.That(count, Is.EqualTo(2));

		target.Reset();
		Assert.That(count, Is.EqualTo(2));

		Assert.Throws<ArgumentNullException>(() => target.CopyFrom(null!));
	}
}
