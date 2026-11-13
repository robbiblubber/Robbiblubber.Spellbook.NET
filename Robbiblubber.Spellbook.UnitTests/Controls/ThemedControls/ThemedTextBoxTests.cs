namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedTextBox"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedTextBoxTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Tested control.</summary>
	private ThemedTextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Asserts that two colors have the same value.</summary>
	/// <param name="actual">Actual color.</param>
	/// <param name="expected">Expected color.</param>
	private static void _AssertColor(Color actual, Color expected)
	{
		Assert.That(actual.ToArgb(), Is.EqualTo(expected.ToArgb()));
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
		_Text = new ThemedTextBox() { Text = "Text", Size = new Size(120, 23) };
		_Form.Controls.Add(_Text);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager.Reset(_Form);
		_Text.Dispose();
		_Form.Dispose();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The back and text colors follow the state of the text box.</summary>
	[Test]
	public void UsesStateColors()
	{
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);
		_AssertColor(_Text.ForeColor, Theme.LIGHT[ThemeColor.FIELD_TEXT]);

		_Text.ReadOnly = true;
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.READ_ONLY_BACK]);
		_AssertColor(_Text.ForeColor, Theme.LIGHT[ThemeColor.READ_ONLY_TEXT]);

		_Text.Enabled = false;
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.DISABLED_BACK]);
		_AssertColor(_Text.ForeColor, Theme.LIGHT[ThemeColor.DISABLED_TEXT]);

		ThemeManager.Theme = Theme.DARK;
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.DISABLED_BACK]);
	}


	/// <summary>The border color follows the state of the text box.</summary>
	[Test]
	public void UsesBorderColors()
	{
		_AssertColor(_Text._BorderColor, Theme.LIGHT[ThemeColor.FIELD_BORDER]);

		_Text.Colors.BorderColor = Color.Red;
		_AssertColor(_Text._BorderColor, Color.Red);

		_Text.Enabled = false;
		_AssertColor(_Text._BorderColor, Theme.LIGHT[ThemeColor.DISABLED_BORDER]);
	}


	/// <summary>System colors written for the standard text box are ignored; other colors are stored in the color table.</summary>
	[Test]
	public void IgnoresSystemColors()
	{
		_Text.BackColor = SystemColors.Window;
		_Text.ForeColor = SystemColors.WindowText;
		Assert.That(_Text.Colors.IsDefault, Is.True);

		_Text.BackColor = Color.LightYellow;
		Assert.That(_Text.Colors.BackColor, Is.EqualTo(Color.LightYellow));
		Assert.That(_Text.ShouldSerializeColors(), Is.True);

		_Text.ResetColors();
		Assert.That(_Text.Colors.IsDefault, Is.True);
	}


	/// <summary>The border is drawn in the border color, also into bitmaps.</summary>
	[Test]
	public void DrawsBorder()
	{
		_Text.BorderStyle = BorderStyle.FixedSingle;
		_Text.Colors.BorderColor = Color.Red;

		using Bitmap bitmap = new(_Text.Width, _Text.Height);
		_Text.DrawToBitmap(bitmap, new Rectangle(0, 0, _Text.Width, _Text.Height));

		_AssertColor(bitmap.GetPixel(0, _Text.Height / 2), Color.Red);
		_AssertColor(bitmap.GetPixel(_Text.Width / 2, _Text.Height - 1), Color.Red);
	}


	/// <summary>The text box takes the theme applied to its form.</summary>
	[Test]
	public void UsesThemeOfForm()
	{
		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_Text._BorderColor, Theme.DARK[ThemeColor.FIELD_BORDER]);

		ThemeManager.Reset(_Form);
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);
	}
}
