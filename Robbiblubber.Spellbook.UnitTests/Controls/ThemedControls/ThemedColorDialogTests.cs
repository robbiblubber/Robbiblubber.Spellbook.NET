namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests <see cref="ThemedColorDialog"/>; the windows are created, but not shown.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedColorDialogTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Asserts that two colors have the same value.</summary>
	/// <param name="actual">Actual color.</param>
	/// <param name="expected">Expected color.</param>
	private static void _AssertColor(Color actual, Color expected)
	{
		Assert.That(actual.ToArgb().ToString("X8"), Is.EqualTo(expected.ToArgb().ToString("X8")));
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
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemedColorDialogTexts.Red = null;
		ControlTexts.LookUp = null;
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Colors are converted to hue, saturation and value and back.</summary>
	[Test]
	public void ConvertsHsv()
	{
		_HsvColor orange = _HsvColor.FromColor(Color.FromArgb(255, 128, 0));
		Assert.That(orange.Hue, Is.EqualTo(30d).Within(0.5d));
		Assert.That(orange.Saturation, Is.EqualTo(1d));
		Assert.That(orange.Value, Is.EqualTo(1d));

		_HsvColor magenta = _HsvColor.FromColor(Color.FromArgb(128, 255, 0, 128));
		Assert.That(magenta.Hue, Is.EqualTo(330d).Within(0.5d));
		Assert.That(magenta.Alpha, Is.EqualTo(128));

		foreach(Color i in new[] { Color.FromArgb(43, 43, 43), Color.FromArgb(12, 200, 99), Color.FromArgb(77, 1, 2, 250), Color.White, Color.Black })
		{
			_AssertColor(_HsvColor.FromColor(i).ToColor(), i);
		}
		_AssertColor(new _HsvColor(360d, 1d, 1d).ToColor(), Color.FromArgb(255, 0, 0));
	}


	/// <summary>Grays keep the hue and black keeps the saturation of the reference color.</summary>
	[Test]
	public void KeepsHueOfGrays()
	{
		_HsvColor reference = new(200d, 0.6d, 0.8d);

		_HsvColor gray = _HsvColor.FromColor(Color.FromArgb(128, 128, 128), reference);
		Assert.That(gray.Hue, Is.EqualTo(200d));
		Assert.That(gray.Saturation, Is.EqualTo(0d));

		_HsvColor black = _HsvColor.FromColor(Color.Black, reference);
		Assert.That(black.Hue, Is.EqualTo(200d));
		Assert.That(black.Saturation, Is.EqualTo(0.6d));
		Assert.That(black.Value, Is.EqualTo(0d));
	}


	/// <summary>The hexadecimal value takes three, six and eight digits and color names.</summary>
	[Test]
	public void ParsesColors()
	{
		Assert.That(_ColorDialogWindow._TryParseColor("#2B2B2B", false, out Color color), Is.True);
		_AssertColor(color, Color.FromArgb(43, 43, 43));

		Assert.That(_ColorDialogWindow._TryParseColor(" f80 ", false, out color), Is.True);
		_AssertColor(color, Color.FromArgb(255, 136, 0));

		Assert.That(_ColorDialogWindow._TryParseColor("#802B2B2B", true, out color), Is.True);
		_AssertColor(color, Color.FromArgb(128, 43, 43, 43));
		Assert.That(_ColorDialogWindow._TryParseColor("#802B2B2B", false, out _), Is.False);

		Assert.That(_ColorDialogWindow._TryParseColor("Teal", false, out color), Is.True);
		_AssertColor(color, Color.Teal);
		Assert.That(color.IsNamedColor, Is.False);

		Assert.That(_ColorDialogWindow._TryParseColor("Transparent", false, out _), Is.False);
		Assert.That(_ColorDialogWindow._TryParseColor("Window", false, out _), Is.False);
		Assert.That(_ColorDialogWindow._TryParseColor("#12345", false, out _), Is.False);
		Assert.That(_ColorDialogWindow._TryParseColor("#teal", false, out _), Is.False);
		Assert.That(_ColorDialogWindow._TryParseColor(string.Empty, false, out _), Is.False);

		Assert.That(_ColorDialogWindow._ToText(Color.FromArgb(128, 43, 43, 43), true), Is.EqualTo("#802B2B2B"));
		Assert.That(_ColorDialogWindow._ToText(Color.FromArgb(43, 43, 43), true), Is.EqualTo("#2B2B2B"));
	}


	/// <summary>The window shows the color in all parts.</summary>
	[Test]
	public void ShowsColor()
	{
		using ThemedColorDialog dialog = new() { Color = Color.FromArgb(255, 128, 0) };
		using _ColorDialogWindow window = dialog._CreateWindow(null);

		Assert.That(window._NumberRed.Value, Is.EqualTo(255));
		Assert.That(window._NumberGreen.Value, Is.EqualTo(128));
		Assert.That(window._NumberBlue.Value, Is.EqualTo(0));
		Assert.That(window._NumberHue.Value, Is.EqualTo(30));
		Assert.That(window._NumberSaturation.Value, Is.EqualTo(100));
		Assert.That(window._NumberValue.Value, Is.EqualTo(100));
		Assert.That(window._TextHex.Text, Is.EqualTo("#FF8000"));
		Assert.That(window._Spectrum.Hue, Is.EqualTo(30d).Within(0.5d));
		Assert.That(window._HueSlider.Value, Is.EqualTo(30d).Within(0.5d));
		_AssertColor(window._Preview.NewColor, Color.FromArgb(255, 128, 0));
		_AssertColor(window._Preview.OriginalColor, Color.FromArgb(255, 128, 0));
		Assert.That(window._AlphaSlider, Is.Null);
		Assert.That(window._NumberAlpha, Is.Null);
		Assert.That(window.Text, Is.EqualTo("Color"));
	}


	/// <summary>A change in one part changes the others.</summary>
	[Test]
	public void FollowsChanges()
	{
		using ThemedColorDialog dialog = new() { Color = Color.Black };
		using _ColorDialogWindow window = dialog._CreateWindow(null);

		window._TextHex.Text = "#336699";
		_AssertColor(window._Color, Color.FromArgb(51, 102, 153));
		Assert.That(window._NumberBlue.Value, Is.EqualTo(153));
		Assert.That(window._NumberHue.Value, Is.EqualTo(210));
		Assert.That(window._TextHex.Text, Is.EqualTo("#336699"));

		window._NumberRed.Value = 255;
		_AssertColor(window._Color, Color.FromArgb(255, 102, 153));
		Assert.That(window._TextHex.Text, Is.EqualTo("#FF6699"));

		window._NumberSaturation.Value = 0;
		_AssertColor(window._Color, Color.FromArgb(255, 255, 255));
		Assert.That(window._NumberHue.Value, Is.Not.EqualTo(0));

		window._TextHex.Text = "#12";
		_AssertColor(window._Color, Color.FromArgb(255, 255, 255));
	}


	/// <summary>Picking a basic color and adding it to the custom colors.</summary>
	[Test]
	public void AddsCustomColors()
	{
		using ThemedColorDialog dialog = new() { CustomColors = new[] { Color.Red } };
		using _ColorDialogWindow window = dialog._CreateWindow(null);

		window._TextHex.Text = "#00FF00";
		Assert.That(window._BasicSwatches.SelectedIndex, Is.GreaterThanOrEqualTo(0));
		_AssertColor(window._BasicSwatches.SelectedColor, Color.FromArgb(0, 255, 0));

		window._AddCustomColor();
		Assert.That(window._CustomSwatches.SelectedIndex, Is.EqualTo(1));
		_AssertColor(window._CustomColors[1], Color.FromArgb(0, 255, 0));

		window._TextHex.Text = "#0000FF";
		window._AddCustomColor();
		Assert.That(window._CustomSwatches.SelectedIndex, Is.EqualTo(1));
		_AssertColor(window._CustomColors[1], Color.FromArgb(0, 0, 255));
		_AssertColor(window._CustomColors[0], Color.Red);
		Assert.That(window._CustomColors.Length, Is.EqualTo(ThemedColorDialog.CUSTOM_COLOR_COUNT));
	}


	/// <summary>OK takes over color and custom colors.</summary>
	[Test]
	public void TakesResult()
	{
		using ThemedColorDialog dialog = new() { Color = Color.Black };
		using(_ColorDialogWindow window = dialog._CreateWindow(null))
		{
			window._TextHex.Text = "#123456";
			window._AddCustomColor();
			dialog._TakeResult(window);
		}

		_AssertColor(dialog.Color, Color.FromArgb(0x12, 0x34, 0x56));
		_AssertColor(dialog.CustomColors[0], Color.FromArgb(0x12, 0x34, 0x56));
		Assert.That(dialog.CustomColors[1].IsEmpty, Is.True);
	}


	/// <summary>With alpha, the window has an alpha slider and field; without, the color is opaque.</summary>
	[Test]
	public void HandlesAlpha()
	{
		using(ThemedColorDialog dialog = new() { Color = Color.FromArgb(128, 10, 20, 30) })
		{
			using _ColorDialogWindow window = dialog._CreateWindow(null);
			_AssertColor(window._Color, Color.FromArgb(10, 20, 30));
			Assert.That(window._TextHex.Text, Is.EqualTo("#0A141E"));
		}

		using(ThemedColorDialog dialog = new() { Color = Color.FromArgb(128, 10, 20, 30), AllowAlpha = true })
		{
			using _ColorDialogWindow window = dialog._CreateWindow(null);
			Assert.That(window._AlphaSlider, Is.Not.Null);
			Assert.That(window._NumberAlpha!.Value, Is.EqualTo(128));
			Assert.That(window._TextHex.Text, Is.EqualTo("#800A141E"));

			window._NumberAlpha.Value = 255;
			_AssertColor(window._Color, Color.FromArgb(10, 20, 30));
			Assert.That(window._AlphaSlider!.Value, Is.EqualTo(255d));
		}
	}


	/// <summary>The window takes the theme of its owner, the theme of the dialog wins.</summary>
	[Test]
	public void UsesThemeOfOwner()
	{
		using Form owner = new();
		ThemeManager.Apply(owner, Theme.DARK);

		using ThemedColorDialog dialog = new();
		using(_ColorDialogWindow window = dialog._CreateWindow(owner))
		{
			Assert.That(ThemeManager.GetTheme(window), Is.SameAs(Theme.DARK));
		}

		dialog.Theme = Theme.LIGHT;
		using(_ColorDialogWindow window = dialog._CreateWindow(owner))
		{
			Assert.That(ThemeManager.GetTheme(window), Is.SameAs(Theme.LIGHT));
		}
	}


	/// <summary>The texts come from <see cref="ThemedColorDialogTexts"/> and the localization; the title can be set.</summary>
	[Test]
	public void UsesTexts()
	{
		ControlTexts.LookUp = (symbol, defaultText) => ((symbol == "spellbook:colordialog.title") ? "Farbe" : defaultText);
		ThemedColorDialogTexts.Red = "&Rot:";

		using ThemedColorDialog dialog = new();
		using(_ColorDialogWindow window = dialog._CreateWindow(null))
		{
			Assert.That(window.Text, Is.EqualTo("Farbe"));
			Assert.That(window.Controls.OfType<Label>().Select(m => m.Text), Does.Contain("&Rot:"));
		}

		dialog.Title = "Accent";
		using(_ColorDialogWindow window = dialog._CreateWindow(null))
		{
			Assert.That(window.Text, Is.EqualTo("Accent"));
		}
	}


	/// <summary>The saturation and value area shows the hue at the top right, white at the top left and black at the bottom.</summary>
	[Test]
	public void DrawsSpectrum()
	{
		using _ColorSpectrum spectrum = new() { Size = new Size(100, 100), Hue = 120d, Saturation = 0d, Value = 0d };
		using Bitmap bitmap = new(100, 100);
		spectrum.DrawToBitmap(bitmap, new Rectangle(0, 0, 100, 100));

		Color topRight = bitmap.GetPixel(96, 2);
		Assert.That(topRight.G, Is.GreaterThan(240));
		Assert.That(topRight.R, Is.LessThan(20));

		Color topLeft = bitmap.GetPixel(2, 2);
		Assert.That(Math.Min(topLeft.R, Math.Min(topLeft.G, topLeft.B)), Is.GreaterThan(235));

		Color bottom = bitmap.GetPixel(50, 97);
		Assert.That(Math.Max(bottom.R, Math.Max(bottom.G, bottom.B)), Is.LessThan(20));
	}
}
