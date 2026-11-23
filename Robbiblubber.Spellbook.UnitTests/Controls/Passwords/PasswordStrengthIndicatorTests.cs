namespace Robbiblubber.Spellbook.UnitTests.Controls.Passwords;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="PasswordStrengthIndicator"/> control and its rating heuristic.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class PasswordStrengthIndicatorTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Tested control.</summary>
	private PasswordStrengthIndicator _Indicator = null!;

	/// <summary>Raised events.</summary>
	private readonly List<string> _Events = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Events.Clear();

		_Indicator = new PasswordStrengthIndicator();
		_Indicator.ValueChanged += (sender, e) => { _Events.Add("ValueChanged"); };
		_Indicator.ValueEvaluated += (sender, e) => { _Events.Add("ValueEvaluated"); };
		_Indicator.Rating += (sender, e) => { _Events.Add("Rating"); };
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Indicator.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The heuristic rates typical passwords as expected.</summary>
	/// <param name="password">Password.</param>
	/// <param name="expected">Expected strength.</param>
	[TestCase("", PasswordStrength.NONE)]
	[TestCase("a", PasswordStrength.VERY_WEAK)]
	[TestCase("password", PasswordStrength.VERY_WEAK)]
	[TestCase("Password1!", PasswordStrength.VERY_WEAK)]
	[TestCase("P@ssw0rd", PasswordStrength.VERY_WEAK)]
	[TestCase("Sommer2024!", PasswordStrength.VERY_WEAK)]
	[TestCase("qwertz123", PasswordStrength.VERY_WEAK)]
	[TestCase("aaaaaaaaaaaa", PasswordStrength.VERY_WEAK)]
	[TestCase("abcdefgh", PasswordStrength.VERY_WEAK)]
	[TestCase("12345678", PasswordStrength.VERY_WEAK)]
	[TestCase("Aa1!Aa1!", PasswordStrength.VERY_WEAK)]
	[TestCase("aB3$eF", PasswordStrength.WEAK)]
	[TestCase("MyDog2015", PasswordStrength.FAIR)]
	[TestCase("ilovemycat", PasswordStrength.FAIR)]
	[TestCase("Xk9#mP2$", PasswordStrength.FAIR)]
	[TestCase("Xk9#mP2$vL", PasswordStrength.GOOD)]
	[TestCase("Tr0ub4dor&3", PasswordStrength.STRONG)]
	[TestCase("Xk9#mP2$vLq7", PasswordStrength.STRONG)]
	[TestCase("Xk9#mP2$vLq7!wZ4", PasswordStrength.VERY_STRONG)]
	[TestCase("correct horse battery staple", PasswordStrength.VERY_STRONG)]
	public void RatesPasswords(string password, PasswordStrength expected)
	{
		Assert.That(PasswordStrengthIndicator.Rate(password), Is.EqualTo(expected));
	}


	/// <summary>Evaluate uses the heuristic; Rating handlers can replace the strength; empty passwords are NONE without Rating.</summary>
	[Test]
	public void EvaluatesWithRatingEvent()
	{
		_Indicator.Evaluate("Xk9#mP2$vL");
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.GOOD));
		Assert.That(_Events, Is.EqualTo(new[] { "Rating", "ValueChanged", "ValueEvaluated" }));

		_Indicator.Rating += (sender, e) =>
		{
			Assert.That(e.Strength, Is.EqualTo(PasswordStrength.GOOD));
			Assert.That(e.Password, Is.EqualTo("Xk9#mP2$vL"));
			e.Strength = PasswordStrength.WEAK;
		};
		_Indicator.Evaluate("Xk9#mP2$vL");
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.WEAK));

		_Events.Clear();
		_Indicator.Evaluate(string.Empty);
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.NONE));
		Assert.That(_Events, Is.EqualTo(new[] { "ValueChanged", "ValueEvaluated" }));
	}


	/// <summary>The source text is rated on every change; a disposed source is released.</summary>
	[Test]
	public void FollowsSource()
	{
		TextBox text = new();

		_Indicator.Source = text;
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.NONE));

		text.Text = "Tr0ub4dor&3";
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.STRONG));

		_Events.Clear();
		text.Text = "Tr0ub4dor&4";
		Assert.That(_Events, Does.Contain("ValueEvaluated"));
		Assert.That(_Events, Does.Not.Contain("ValueChanged"));

		text.Dispose();
		Assert.That(_Indicator.Source, Is.Null);
	}


	/// <summary>Values outside the range are limited; ValueChanged is raised once per change.</summary>
	[Test]
	public void LimitsValue()
	{
		_Indicator.Value = (PasswordStrength) 99;
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.VERY_STRONG));

		_Indicator.Value = (PasswordStrength) (-5);
		Assert.That(_Indicator.Value, Is.EqualTo(PasswordStrength.NONE));

		_Indicator.Value = PasswordStrength.NONE;
		Assert.That(_Events.Count(m => m == "ValueChanged"), Is.EqualTo(2));
	}


	/// <summary>The boxes fill the area completely; the spacing between them is constant.</summary>
	[Test]
	public void DistributesBoxes()
	{
		foreach(int width in new[] { 140, 141, 145, 97 })
		{
			(int Left, int Width)[] boxes = PasswordStrengthIndicator._GetBoxes(width, 4);

			Assert.That(boxes[0].Left, Is.EqualTo(0));
			Assert.That(boxes[^1].Left + boxes[^1].Width, Is.EqualTo(width), $"width {width}");

			for(int i = 1; i < boxes.Length; i++)
			{
				Assert.That(boxes[i].Left - (boxes[i - 1].Left + boxes[i - 1].Width), Is.EqualTo(4));
				Assert.That(Math.Abs(boxes[i].Width - boxes[0].Width), Is.LessThanOrEqualTo(1));
			}
		}
	}


	/// <summary>Every strength has its own fill color; FilledColor sets all of them.</summary>
	[Test]
	public void ColorsStrengths()
	{
		PasswordStrengthColors colors = _Indicator.Colors;
		colors.Theme = Theme.LIGHT;

		Assert.That(colors._GetFill(PasswordStrength.VERY_WEAK).ToArgb(), Is.EqualTo(Color.FromArgb(0xD3, 0x2F, 0x2F).ToArgb()));
		Assert.That(colors._GetFill(PasswordStrength.VERY_STRONG).ToArgb(), Is.EqualTo(Color.FromArgb(0x1B, 0x5E, 0x20).ToArgb()));
		Assert.That(colors._GetFill(PasswordStrength.NONE), Is.EqualTo(colors._GetFill(PasswordStrength.VERY_WEAK)));

		colors.GoodColor = Color.Blue;
		Assert.That(colors._GetFill(PasswordStrength.GOOD), Is.EqualTo(Color.Blue));

		_Indicator.FilledColor = Color.Navy;
		foreach(PasswordStrength i in Enum.GetValues<PasswordStrength>())
		{
			Assert.That(colors._GetFill(i), Is.EqualTo(Color.Navy));
		}

		_Indicator.ResetColors();
		Assert.That(_Indicator.ShouldSerializeColors(), Is.False);
	}


	/// <summary>The strength text follows the value and can be changed.</summary>
	[Test]
	public void ProvidesStrengthText()
	{
		Assert.That(_Indicator.StrengthText, Is.Empty);

		_Indicator.Value = PasswordStrength.FAIR;
		Assert.That(_Indicator.StrengthText, Is.EqualTo("Fair"));

		_Indicator.FairText = "Mittel";
		Assert.That(_Indicator.StrengthText, Is.EqualTo("Mittel"));

		_Indicator.FairText = null!;
		Assert.That(_Indicator.StrengthText, Is.Empty);
	}
}
