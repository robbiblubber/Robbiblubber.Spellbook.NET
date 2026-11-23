namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests <see cref="ThemedMessageBox"/>; the windows are created, but not shown.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedMessageBoxTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();
		ThemeManager.Theme = Theme.LIGHT;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemedMessageBoxTexts.Yes = null;
		ControlTexts.LookUp = null;
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The buttons, the default button and the cancel button follow the arguments.</summary>
	[Test]
	public void CreatesButtons()
	{
		using _MessageBoxWindow window = ThemedMessageBox._CreateWindow(null, "Save changes?", "Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 0);

		Assert.That(window._Buttons.Select(m => m.DialogResult), Is.EqualTo(new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel }));
		Assert.That(window._Buttons.Select(m => m.Text), Is.EqualTo(new[] { "&Yes", "&No", "Cancel" }));
		Assert.That(window.AcceptButton, Is.SameAs(window._Buttons[1]));
		Assert.That(window.CancelButton, Is.SameAs(window._Buttons[2]));
		Assert.That(window._Closable, Is.True);
		Assert.That(window.Text, Is.EqualTo("Editor"));
	}


	/// <summary>Yes/No and Abort/Retry/Ignore cannot be closed without a button; OK can be closed with Esc.</summary>
	[Test]
	public void HandlesCancel()
	{
		using(_MessageBoxWindow window = ThemedMessageBox._CreateWindow(null, "Text", null, MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button3, 0))
		{
			Assert.That(window._Closable, Is.False);
			Assert.That(window.CancelButton, Is.Null);
			Assert.That(window.AcceptButton, Is.SameAs(window._Buttons[0]));
		}

		using(_MessageBoxWindow window = ThemedMessageBox._CreateWindow(null, "Text", null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0))
		{
			Assert.That(window._Closable, Is.True);
			Assert.That(window.CancelButton, Is.SameAs(window._Buttons[0]));
		}
	}


	/// <summary>The button texts come from <see cref="ThemedMessageBoxTexts"/>.</summary>
	[Test]
	public void UsesTexts()
	{
		ThemedMessageBoxTexts.Yes = "&Ja";
		using _MessageBoxWindow window = ThemedMessageBox._CreateWindow(null, "Text", null, MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);

		Assert.That(window._Buttons[0].Text, Is.EqualTo("&Ja"));
	}


	/// <summary>The texts come from the localization unless they are set explicitly.</summary>
	[Test]
	public void UsesLocalization()
	{
		List<string> symbols = new();
		ControlTexts.LookUp = (symbol, defaultText) => { symbols.Add(symbol); return (symbol == "spellbook:msgbox.no") ? "&Nein" : defaultText; };

		Assert.That(ThemedMessageBoxTexts.No, Is.EqualTo("&Nein"));
		Assert.That(ThemedMessageBoxTexts.Yes, Is.EqualTo("&Yes"));
		Assert.That(symbols, Is.EqualTo(new[] { "spellbook:msgbox.no", "spellbook:msgbox.yes" }));

		ThemedMessageBoxTexts.No = "&Non";
		Assert.That(ThemedMessageBoxTexts.No, Is.EqualTo("&Non"));

		ThemedMessageBoxTexts.No = null;
		Assert.That(ThemedMessageBoxTexts.No, Is.EqualTo("&Nein"));
	}


	/// <summary>The names of the color roles of the theme editor come from the localization.</summary>
	[Test]
	public void UsesLocalizationForRoleNames()
	{
		Assert.That(ThemeEditorTexts._GetRoleName(ThemeColor.FIELD_BORDER_HOT), Is.EqualTo("Field border hot"));

		ControlTexts.LookUp = (symbol, defaultText) => ((symbol == "spellbook:themeeditor.role.field_border_hot") ? "Feldrahmen (Maus)" : defaultText);
		Assert.That(ThemeEditorTexts._GetRoleName(ThemeColor.FIELD_BORDER_HOT), Is.EqualTo("Feldrahmen (Maus)"));
		Assert.That(ThemeEditorTexts.Title, Is.EqualTo("Themes"));
	}


	/// <summary>Ctrl+C copies the message in the format of Windows message boxes.</summary>
	[Test]
	public void CopiesText()
	{
		using _MessageBoxWindow window = ThemedMessageBox._CreateWindow(null, "Line 1", "Caption", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0);

		Assert.That(window._CopyText, Is.EqualTo("---------------------------\r\nCaption\r\n---------------------------\r\nLine 1\r\n" +
												 "---------------------------\r\nOK   Cancel   \r\n---------------------------\r\n"));
	}


	/// <summary>The message box takes the theme of its owner.</summary>
	[Test]
	public void UsesThemeOfOwner()
	{
		using Form owner = new();
		ThemeManager.Apply(owner, Theme.DARK);

		using _MessageBoxWindow window = ThemedMessageBox._CreateWindow(owner, "Text", null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
		Assert.That(ThemeManager.GetTheme(window), Is.SameAs(Theme.DARK));
		Assert.That(window._Buttons[0].Colors.EffectiveTheme, Is.SameAs(Theme.DARK));

		ThemeManager.Reset(owner);
	}
}
