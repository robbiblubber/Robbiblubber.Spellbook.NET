namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedGroupBox"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedGroupBoxTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Tested control.</summary>
	private ThemedGroupBox _Group = null!;

	/// <summary>Text box in the group box.</summary>
	private TextBox _Text = null!;



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
		_Group = new ThemedGroupBox() { Text = "Group", Size = new Size(200, 100) };
		_Text = new TextBox() { Location = new Point(10, 30) };
		_Group.Controls.Add(_Text);
		_Form.Controls.Add(_Group);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager.Reset(_Form);
		_Text.Dispose();
		_Group.Dispose();
		_Form.Dispose();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The theme manager applies the theme to the controls in the group box.</summary>
	[Test]
	public void ThemesChildren()
	{
		ThemeManager.Apply(_Form, Theme.DARK);

		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_Group.ForeColor, Theme.DARK[ThemeColor.TEXT]);
		_AssertColor(_Text.ForeColor, Theme.DARK[ThemeColor.FIELD_TEXT]);
	}


	/// <summary>The frame is drawn in the border color, the caption interrupts it.</summary>
	[Test]
	public void DrawsFrame()
	{
		_Group.Colors.BorderColor = Color.Red;
		using Bitmap bitmap = _Render(_Group);

		_AssertColor(bitmap.GetPixel(0, _Group.Height / 2), Color.Red);
		Assert.That(bitmap.GetPixel(_Group.Width - 1, _Group.Height / 2).ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
	}


	/// <summary>System colors written for the standard group box are ignored.</summary>
	[Test]
	public void IgnoresSystemColors()
	{
		_Group.ForeColor = SystemColors.ControlText;
		Assert.That(_Group.Colors.IsDefault, Is.True);
		_AssertColor(_Group.ForeColor, Theme.LIGHT[ThemeColor.TEXT]);
	}
}
