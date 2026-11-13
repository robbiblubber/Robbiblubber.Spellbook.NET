namespace Robbiblubber.Spellbook.UnitTests.Controls.Progress;

using System.Drawing;
using System.Globalization;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ProgressMeter"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ProgressMeterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Tested control.</summary>
	private ProgressMeter _Meter = null!;

	/// <summary>Number of ValueChanged events.</summary>
	private int _Changes = 0;

	/// <summary>Culture before the test.</summary>
	private CultureInfo _Culture = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Culture = CultureInfo.CurrentCulture;
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

		_Changes = 0;
		_Meter = new ProgressMeter();
		_Meter.ValueChanged += (sender, e) => { _Changes++; };
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Meter.Dispose();
		CultureInfo.CurrentCulture = _Culture;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Values outside the range are limited; ValueChanged is raised only when the value changes.</summary>
	[Test]
	public void LimitsValue()
	{
		_Meter.Value = 150;
		Assert.That(_Meter.Value, Is.EqualTo(100));

		_Meter.Value = 100;
		_Meter.Value = -5;
		Assert.That(_Meter.Value, Is.EqualTo(0));
		Assert.That(_Changes, Is.EqualTo(2));
	}


	/// <summary>The fraction is computed relative to the range, also for a minimum greater than 0.</summary>
	[Test]
	public void ComputesFraction()
	{
		_Meter.Minimum = 50;
		_Meter.Value = 75;
		Assert.That(_Meter.Fraction, Is.EqualTo(0.5).Within(0.0001));

		_Meter.Minimum = -100;
		_Meter.Maximum = 100;
		_Meter.Value = 0;
		Assert.That(_Meter.Fraction, Is.EqualTo(0.5).Within(0.0001));

		_Meter.Maximum = -100;
		Assert.That(_Meter.Fraction, Is.EqualTo(0));
	}


	/// <summary>Changing the range adjusts the other limit and the value.</summary>
	[Test]
	public void AdjustsRange()
	{
		_Meter.Value = 80;

		_Meter.Maximum = 50;
		Assert.That(_Meter.Value, Is.EqualTo(50));

		_Meter.Minimum = 70;
		Assert.That(_Meter.Maximum, Is.EqualTo(70));
		Assert.That(_Meter.Value, Is.EqualTo(70));

		_Meter.Maximum = 10;
		Assert.That(_Meter.Minimum, Is.EqualTo(10));
		Assert.That(_Meter.Value, Is.EqualTo(10));
	}


	/// <summary>PerformStep and Increment change the value within the range.</summary>
	[Test]
	public void Steps()
	{
		Assert.That(_Meter.Step, Is.EqualTo(10));

		_Meter.PerformStep();
		_Meter.PerformStep();
		Assert.That(_Meter.Value, Is.EqualTo(20));

		_Meter.Increment(-5);
		Assert.That(_Meter.Value, Is.EqualTo(15));

		_Meter.Increment(int.MaxValue);
		Assert.That(_Meter.Value, Is.EqualTo(100));
	}


	/// <summary>The text styles display the percentage, the value or the text.</summary>
	[Test]
	public void FormatsText()
	{
		_Meter.Maximum = 40;
		_Meter.Value = 17;
		Assert.That(_Meter._DisplayText, Is.Empty);

		_Meter.TextStyle = ProgressMeterTextStyle.PERCENT;
		Assert.That(_Meter._DisplayText, Is.EqualTo((17.0 / 40).ToString("P0", CultureInfo.CurrentCulture)));

		_Meter.TextStyle = ProgressMeterTextStyle.VALUE;
		Assert.That(_Meter._DisplayText, Is.EqualTo("17 / 40"));

		_Meter.Text = "Copying";
		_Meter.TextStyle = ProgressMeterTextStyle.TEXT;
		Assert.That(_Meter._DisplayText, Is.EqualTo("Copying"));
	}


	/// <summary>In marquee style, only the text style TEXT is displayed.</summary>
	[Test]
	public void HidesValueTextInMarquee()
	{
		_Meter.Style = ProgressMeterStyle.MARQUEE;
		_Meter.TextStyle = ProgressMeterTextStyle.PERCENT;
		Assert.That(_Meter._DisplayText, Is.Empty);

		_Meter.Text = "Waiting";
		_Meter.TextStyle = ProgressMeterTextStyle.TEXT;
		Assert.That(_Meter._DisplayText, Is.EqualTo("Waiting"));
	}


	/// <summary>BackColor, ForeColor and BorderColor are mapped to the color table.</summary>
	[Test]
	public void MapsColorsToTable()
	{
		_Meter.ForeColor = Color.Green;
		_Meter.BorderColor = Color.Red;

		Assert.That(_Meter.Colors.BarColor, Is.EqualTo(Color.Green));
		Assert.That(_Meter.Colors.BorderColor, Is.EqualTo(Color.Red));
		Assert.That(_Meter.ShouldSerializeColors(), Is.True);

		_Meter.ResetColors();
		Assert.That(_Meter.ForeColor, Is.EqualTo(ThemeManager.Theme[ThemeColor.ACCENT]));

		_Meter.Colors.Theme = Theme.DARK;
		Assert.That(_Meter.BackColor.ToArgb(), Is.EqualTo(Color.FromArgb(0x2B, 0x2B, 0x2B).ToArgb()));
	}


	/// <summary>The marquee timer runs only while the marquee style is set and the meter is visible and enabled.</summary>
	[Test]
	public void RunsMarquee()
	{
		using Form form = new() { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false };

		form.Controls.Add(_Meter);
		form.Show();
		Assert.That(_Meter._MarqueeRunning, Is.False);

		_Meter.Style = ProgressMeterStyle.MARQUEE;
		Assert.That(_Meter._MarqueeRunning, Is.True);

		_Meter.Enabled = false;
		Assert.That(_Meter._MarqueeRunning, Is.False);

		_Meter.Enabled = true;
		_Meter.Visible = false;
		Assert.That(_Meter._MarqueeRunning, Is.False);

		_Meter.Visible = true;
		_Meter.Style = ProgressMeterStyle.CONTINUOUS;
		Assert.That(_Meter._MarqueeRunning, Is.False);

		form.Controls.Remove(_Meter);
	}


	/// <summary>Color tables can only copy tables of the same type.</summary>
	[Test]
	public void CopiesColorTablesOfSameType()
	{
		ProgressMeterColors colors = new() { Theme = Theme.LIGHT, BarColor = Color.Green };

		_Meter.Colors.CopyFrom(colors);
		Assert.That(_Meter.Colors.BarColor, Is.EqualTo(Color.Green));
		Assert.That(_Meter.Colors.Theme, Is.SameAs(Theme.LIGHT));

		Assert.Throws<ArgumentException>(() => _Meter.Colors.CopyFrom(new ThemedComboBoxColors()));
	}
}
