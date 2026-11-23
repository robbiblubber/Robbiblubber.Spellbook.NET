namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedCheckBox"/> and <see cref="ThemedRadioButton"/> controls.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedToggleTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Check box.</summary>
	private ThemedCheckBox _Check = null!;

	/// <summary>Radio button.</summary>
	private ThemedRadioButton _Radio = null!;



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



	/// <summary>Gets the bounds of the box or circle.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the bounds.</returns>
	private static Rectangle _Glyph(Control control)
	{
		int size = (int) Math.Round(13f * control.DeviceDpi / 96f);
		return new Rectangle(0, (control.Height - size) / 2, size, size);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		ThemeManager._ResetForTest();
		ThemeManager.Theme = Theme.DARK;

		_Form = new Form() { BackColor = Theme.DARK[ThemeColor.BACK] };
		_Check = new ThemedCheckBox() { Text = "Check", AutoSize = false, Size = new Size(120, 24) };
		_Radio = new ThemedRadioButton() { Text = "Radio", AutoSize = false, Size = new Size(120, 24), Top = 30 };
		_Form.Controls.AddRange(new Control[] { _Check, _Radio });
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Check.Dispose();
		_Radio.Dispose();
		_Form.Dispose();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The box is filled with the field color when unchecked and with the accent color when checked.</summary>
	[Test]
	public void DrawsCheckBox()
	{
		Rectangle glyph = _Glyph(_Check);
		Point probe = new(glyph.Left + (glyph.Width / 2), glyph.Top + (glyph.Height / 4));

		using(Bitmap bitmap = _Render(_Check)) { _AssertColor(bitmap.GetPixel(probe.X, probe.Y), Theme.DARK[ThemeColor.FIELD_BACK]); }

		_Check.Checked = true;
		using(Bitmap bitmap = _Render(_Check)) { _AssertColor(bitmap.GetPixel(probe.X, probe.Y), Theme.DARK[ThemeColor.ACCENT]); }

		_Check.Enabled = false;
		using(Bitmap bitmap = _Render(_Check)) { _AssertColor(bitmap.GetPixel(probe.X, probe.Y), Theme.DARK[ThemeColor.DISABLED_TEXT]); }
	}


	/// <summary>As switch, the knob is on the left when unchecked, on the right when checked and in the middle when indeterminate; the
	///          track has the accent color unless the check box is unchecked.</summary>
	[Test]
	public void DrawsSwitch()
	{
		float scale = (_Check.DeviceDpi / 96f);
		_Check.Style = ThemedCheckBoxStyle.SWITCH;
		_Check.ThreeState = true;
		int y = (_Check.Height / 2);
		int left = (int) Math.Round(10f * scale), middle = (int) Math.Round(20f * scale), right = (int) Math.Round(30f * scale);

		using(Bitmap bitmap = _Render(_Check)) { _AssertColor(bitmap.GetPixel(right, y), Theme.DARK[ThemeColor.FIELD_BACK]); }

		_Check.CheckState = CheckState.Checked;
		using(Bitmap bitmap = _Render(_Check))
		{
			_AssertColor(bitmap.GetPixel(left, y), Theme.DARK[ThemeColor.ACCENT]);
			_AssertColor(bitmap.GetPixel(right, y), Theme.DARK[ThemeColor.ACCENT_TEXT]);
		}

		_Check.CheckState = CheckState.Indeterminate;
		using(Bitmap bitmap = _Render(_Check))
		{
			_AssertColor(bitmap.GetPixel(left, y), Theme.DARK[ThemeColor.ACCENT]);
			_AssertColor(bitmap.GetPixel(middle, y), Theme.DARK[ThemeColor.ACCENT_TEXT]);
		}
	}


	/// <summary>An automatically sized switch is wider than a check box with a box.</summary>
	[Test]
	public void SizesSwitch()
	{
		ThemedCheckBox check = new() { Text = "Check", AutoSize = true };
		_Form.Controls.Add(check);
		int width = check.PreferredSize.Width;

		check.Style = ThemedCheckBoxStyle.SWITCH;
		Assert.That(check.PreferredSize.Width, Is.GreaterThanOrEqualTo(width + (int) (20f * check.DeviceDpi / 96f)));
		Assert.That(check.Width, Is.EqualTo(check.PreferredSize.Width));
		Assert.That(new ThemedCheckBox().Style, Is.EqualTo(ThemedCheckBoxStyle.BOX));
	}


	/// <summary>A checked radio button has the accent color around a dot in the check mark color.</summary>
	[Test]
	public void DrawsRadioButton()
	{
		Rectangle glyph = _Glyph(_Radio);
		Point center = new(glyph.Left + (glyph.Width / 2), glyph.Top + (glyph.Height / 2));

		_Radio.Checked = true;
		using Bitmap bitmap = _Render(_Radio);
		_AssertColor(bitmap.GetPixel(center.X, center.Y), Theme.DARK[ThemeColor.ACCENT_TEXT]);
		_AssertColor(bitmap.GetPixel(glyph.Left + 3, center.Y), Theme.DARK[ThemeColor.ACCENT]);
	}


	/// <summary>The text color comes from the theme; system colors are ignored.</summary>
	[Test]
	public void UsesThemeTextColor()
	{
		_AssertColor(_Check.ForeColor, Theme.DARK[ThemeColor.TEXT]);

		_Check.ForeColor = SystemColors.ControlText;
		Assert.That(_Check.Colors.IsDefault, Is.True);

		_Radio.ForeColor = Color.Orange;
		Assert.That(_Radio.Colors.TextColor, Is.EqualTo(Color.Orange));
		Assert.That(_Radio.ShouldSerializeColors(), Is.True);
	}


	/// <summary>The controls keep the behavior of the standard controls.</summary>
	[Test]
	public void BehavesLikeStandardControls()
	{
		_Check.ThreeState = true;
		_Check.CheckState = CheckState.Indeterminate;
		Assert.That(_Check.Checked, Is.True);

		ThemedRadioButton other = new() { Checked = true };
		_Form.Controls.Add(other);
		_Radio.Checked = true;
		Assert.That(other.Checked, Is.False);
	}
}
