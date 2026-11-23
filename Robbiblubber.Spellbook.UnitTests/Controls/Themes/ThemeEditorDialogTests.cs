namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemeEditorDialog"/>; the dialog is created, but not shown.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemeEditorDialogTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom theme based on the dark theme.</summary>
	private Theme _First = null!;

	/// <summary>Custom theme based on the first theme.</summary>
	private Theme _Second = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a control and all its child controls.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the controls.</returns>
	private static IEnumerable<Control> _GetControls(Control control)
	{
		return new[] { control }.Concat(control.Controls.Cast<Control>().SelectMany(_GetControls));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();

		_First = new Theme("First", Theme.DARK);
		_First.SetValue(ThemeColor.ACCENT, Color.Orange);
		_Second = new Theme("Second", _First);
		ThemeManager.Add(_First);
		ThemeManager.Add(_Second);
		ThemeManager.Theme = _Second;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The dialog selects the theme of the application.</summary>
	[Test]
	public void SelectsThemeOfApplication()
	{
		using ThemeEditorDialog dialog = new();

		Assert.That(dialog.SelectedTheme, Is.SameAs(_Second));
		Assert.That(dialog.SaveOnOk, Is.False);
	}


	/// <summary>Restoring brings back names, colors, base themes, removed themes and the theme of the application.</summary>
	[Test]
	public void RestoresThemes()
	{
		using ThemeEditorDialog dialog = new();

		_First.Name = "Renamed";
		_First.SetValue(ThemeColor.ACCENT, Color.Red);
		_Second.Base = Theme.LIGHT;
		_First.Base = _Second;
		Theme added = new("Added", Theme.LIGHT);
		ThemeManager.Add(added);
		ThemeManager.Theme = added;

		dialog._Restore();

		Assert.That(_First.Name, Is.EqualTo("First"));
		Assert.That(_First.GetValue(ThemeColor.ACCENT), Is.EqualTo(Color.Orange));
		Assert.That(_First.Base, Is.SameAs(Theme.DARK));
		Assert.That(_Second.Base, Is.SameAs(_First));
		Assert.That(ThemeManager.CustomThemes, Is.EquivalentTo(new[] { _First, _Second }));
		Assert.That(ThemeManager.Theme, Is.SameAs(_Second));
	}


	/// <summary>A removed theme is registered again, and the themes based on it get it back as their base.</summary>
	[Test]
	public void RestoresRemovedThemes()
	{
		using ThemeEditorDialog dialog = new();

		ThemeManager.Remove(_First);
		Assert.That(_Second.Base, Is.SameAs(Theme.DARK));

		dialog._Restore();

		Assert.That(ThemeManager.CustomThemes, Does.Contain(_First));
		Assert.That(_Second.Base, Is.SameAs(_First));
	}


	/// <summary>The theme list shows the built-in themes with the names from <see cref="ThemeSelectorTexts"/>.</summary>
	[Test]
	public void ShowsTranslatedNames()
	{
		ThemeSelectorTexts.Dark = "Dunkel";
		try
		{
			using ThemeEditorDialog dialog = new();
			ListBox list = _GetControls(dialog).OfType<ListBox>().First(m => m.Items.Contains(Theme.DARK));

			Assert.That(list.GetItemText(Theme.DARK), Is.EqualTo("Dunkel"));
			Assert.That(list.GetItemText(Theme.LIGHT), Is.EqualTo("Light"));
			Assert.That(list.GetItemText(_First), Is.EqualTo("First"));
		}
		finally { ThemeSelectorTexts.Dark = null; }
	}
}
