namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedButton"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedButtonTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Tested control.</summary>
	private ThemedButton _Button = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws a control into a bitmap.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the bitmap.</returns>
	private static Bitmap _Render(Control control)
	{
		Bitmap rval = new(control.Width, control.Height);
		control.DrawToBitmap(rval, new Rectangle(0, 0, control.Width, control.Height));

		return rval;
	}


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
		_Button = new ThemedButton() { Text = "OK", Size = new Size(80, 30) };
		_Form.Controls.Add(_Button);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Button.Dispose();
		_Form.Dispose();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Without own colors, the button uses the button colors of the theme.</summary>
	[Test]
	public void UsesThemeColors()
	{
		Assert.That(_Button.Colors.IsDefault, Is.True);
		Assert.That(_Button.ShouldSerializeColors(), Is.False);
		_AssertColor(_Button.BackColor, Theme.LIGHT[ThemeColor.BUTTON_BACK]);
		_AssertColor(_Button.ForeColor, Theme.LIGHT[ThemeColor.BUTTON_TEXT]);

		ThemeManager.Theme = Theme.DARK;
		_AssertColor(_Button.BackColor, Theme.DARK[ThemeColor.BUTTON_BACK]);
	}


	/// <summary>System colors written for the standard button are ignored; other colors are stored in the color table.</summary>
	[Test]
	public void IgnoresSystemColors()
	{
		_Button.BackColor = SystemColors.Control;
		_Button.ForeColor = SystemColors.ControlText;
		Assert.That(_Button.Colors.IsDefault, Is.True);

		_Button.BackColor = Color.DarkRed;
		Assert.That(_Button.Colors.BackColor, Is.EqualTo(Color.DarkRed));
		Assert.That(_Button.ShouldSerializeColors(), Is.True);

		_Button.ResetColors();
		Assert.That(_Button.Colors.IsDefault, Is.True);
	}


	/// <summary>The flat style cannot be changed, so the button always draws itself.</summary>
	[Test]
	public void KeepsFlatStyle()
	{
		_Button.FlatStyle = FlatStyle.System;
		Assert.That(_Button.FlatStyle, Is.EqualTo(FlatStyle.Standard));
	}


	/// <summary>The button is drawn with the back color of its theme; the default button gets the default state.</summary>
	[Test]
	public void DrawsThemeColors()
	{
		_Button.Colors.Theme = Theme.DARK;
		using(Bitmap bitmap = _Render(_Button))
		{
			_AssertColor(bitmap.GetPixel(_Button.Width / 2, 6), Theme.DARK[ThemeColor.BUTTON_BACK]);
		}

		Assert.That(_Button._States.HasFlag(_PaintStates.DEFAULT), Is.False);
		_Form.AcceptButton = _Button;
		Assert.That(_Button._States.HasFlag(_PaintStates.DEFAULT), Is.True);
	}


	/// <summary>The theme applied to the form is used, the button gets no adapter of its own.</summary>
	[Test]
	public void FollowsFormTheme()
	{
		Assert.That(ThemeManager.GetAdapter(typeof(ThemedButton)), Is.SameAs(ThemeManager.GetAdapter(typeof(Control))));

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(_Button.BackColor, Theme.DARK[ThemeColor.BUTTON_BACK]);
		Assert.That(_Button.Colors.Theme, Is.Null);

		ThemeManager.Reset(_Form);
		_AssertColor(_Button.BackColor, Theme.LIGHT[ThemeColor.BUTTON_BACK]);
	}
}
