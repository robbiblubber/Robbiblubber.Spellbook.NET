namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests <see cref="ThemeSelector"/> and <see cref="ThemeSelectorTexts"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemeSelectorTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Selector.</summary>
	private ThemeSelector _Selector = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the texts of the items of the selector.</summary>
	/// <param name="selector">Selector.</param>
	/// <returns>Returns the texts.</returns>
	private static string[] _GetTexts(ThemeSelector selector)
	{
		return selector.ComboBox.Items.Cast<ListItem>().Select(m => m.Text).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();
		ThemeManager.Theme = Theme.LIGHT;

		_Form = new Form();
		_Selector = new ThemeSelector();
		_Form.Controls.Add(_Selector);
		_ = _Selector.Handle;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Selector.Dispose();
		_Form.Dispose();
		ThemeSelectorTexts.Dark = null;
		ThemeManager._ResetForTest();
	}


	/// <summary>The selector lists the built-in and the custom themes and shows the theme of the application.</summary>
	[Test]
	public void ListsThemes()
	{
		Assert.That(_GetTexts(_Selector), Is.EqualTo(new[] { "System", "Automatic", "Light", "Dark" }));
		Assert.That(_Selector.SelectedTheme, Is.SameAs(Theme.LIGHT));
		Assert.That(_Selector.ComboBox.SelectedItem, Is.InstanceOf<ListItem>());
		Assert.That(((ListItem) _Selector.ComboBox.SelectedItem!).Tag, Is.SameAs(Theme.LIGHT));
		Assert.That(((ListItem) _Selector.ComboBox.SelectedItem!).Image, Is.Not.Null);

		Theme custom = new("Custom", Theme.DARK);
		ThemeManager.Add(custom);
		Assert.That(_GetTexts(_Selector), Does.Contain("Custom"));

		custom.Name = "Renamed";
		Assert.That(_GetTexts(_Selector), Does.Contain("Renamed"));
	}


	/// <summary>With <see cref="ThemeSelector.AutoApply"/>, the selected theme is the theme of the application, in both
	///          directions.</summary>
	[Test]
	public void AppliesSelectedTheme()
	{
		int changes = 0;
		_Selector.SelectedThemeChanged += (sender, e) => changes++;

		_Selector.SelectedTheme = Theme.DARK;
		Assert.That(ThemeManager.Theme, Is.SameAs(Theme.DARK));
		Assert.That(((ListItem) _Selector.ComboBox.SelectedItem!).Tag, Is.SameAs(Theme.DARK));
		Assert.That(changes, Is.EqualTo(1));

		ThemeManager.Theme = Theme.SYSTEM;
		Assert.That(_Selector.SelectedTheme, Is.SameAs(Theme.SYSTEM));
		Assert.That(((ListItem) _Selector.ComboBox.SelectedItem!).Tag, Is.SameAs(Theme.SYSTEM));
		Assert.That(changes, Is.EqualTo(2));
	}


	/// <summary>Without <see cref="ThemeSelector.AutoApply"/>, the selection does not change the theme of the application, and a
	///          removed theme is replaced by the theme of the application.</summary>
	[Test]
	public void SelectsWithoutApplying()
	{
		Theme custom = new("Custom", Theme.DARK);
		ThemeManager.Add(custom);
		_Selector.AutoApply = false;

		_Selector.SelectedTheme = custom;
		Assert.That(ThemeManager.Theme, Is.SameAs(Theme.LIGHT));
		Assert.That(_Selector.SelectedTheme, Is.SameAs(custom));

		ThemeManager.Theme = Theme.DARK;
		Assert.That(_Selector.SelectedTheme, Is.SameAs(custom));

		ThemeManager.Remove(custom);
		Assert.That(_Selector.SelectedTheme, Is.SameAs(Theme.DARK));
		Assert.That(((ListItem) _Selector.ComboBox.SelectedItem!).Tag, Is.SameAs(Theme.DARK));
	}


	/// <summary>The names of the built-in themes and the link text come from <see cref="ThemeSelectorTexts"/> and the
	///          localization.</summary>
	[Test]
	public void UsesTexts()
	{
		Assert.That(ThemeSelectorTexts.GetName(Theme.AUTO), Is.EqualTo("Automatic"));
		Assert.That(ThemeSelectorTexts.Customize, Is.EqualTo("Customize..."));

		ThemeSelectorTexts.Dark = "Dunkel";
		_Selector.RefreshTexts();
		Assert.That(_GetTexts(_Selector), Does.Contain("Dunkel"));
		Assert.That(ThemeSelectorTexts.GetName(Theme.DARK), Is.EqualTo("Dunkel"));

		Theme custom = new("Mine", Theme.DARK);
		Assert.That(ThemeSelectorTexts.GetName(custom), Is.EqualTo("Mine"));

		_Selector.ShowEditorLink = false;
		Assert.That(_Selector.ShowEditorLink, Is.False);
	}


	/// <summary>The link is shown by default, also before the form is shown, and the drop-down list leaves room for it.</summary>
	[Test]
	public void ShowsEditorLink()
	{
		_Selector.Width = 300;
		_Selector.PerformLayout();

		Assert.That(_Form.Visible, Is.False);
		Assert.That(_Selector.ShowEditorLink, Is.True);
		Assert.That(_Selector.ComboBox.Width, Is.LessThan(300));

		_Selector.ShowEditorLink = false;
		Assert.That(_Selector.ComboBox.Width, Is.EqualTo(300));
	}
}
