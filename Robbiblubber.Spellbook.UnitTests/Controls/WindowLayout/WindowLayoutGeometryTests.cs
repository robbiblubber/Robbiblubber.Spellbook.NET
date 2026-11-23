namespace Robbiblubber.Spellbook.UnitTests.Controls.WindowLayout;

using System.Drawing;
using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the geometry helpers of <see cref="WindowLayoutProvider"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class WindowLayoutGeometryTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Two screens side by side, the right one with a task bar at the bottom.</summary>
	private static readonly Rectangle[] _TwoScreens = new Rectangle[] { new(0, 0, 1920, 1040), new(1920, 0, 2560, 1400) };



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A window whose caption is visible is not changed.</summary>
	[Test]
	public void VisibleWindowIsKept()
	{
		Rectangle bounds = new(100, 100, 800, 600);

		Assert.That(WindowLayoutProvider._FitToScreen(bounds, _TwoScreens), Is.EqualTo(bounds));
	}


	/// <summary>A window spanning two screens is kept.</summary>
	[Test]
	public void WindowAcrossScreensIsKept()
	{
		Rectangle bounds = new(1500, 200, 1000, 600);

		Assert.That(WindowLayoutProvider._FitToScreen(bounds, _TwoScreens), Is.EqualTo(bounds));
	}


	/// <summary>A window that is mostly outside of the screens is kept as long as enough of its caption is visible.</summary>
	[Test]
	public void PartlyVisibleWindowIsKept()
	{
		Rectangle bounds = new(-600, 100, 800, 600);

		Assert.That(WindowLayoutProvider._FitToScreen(bounds, _TwoScreens), Is.EqualTo(bounds));
	}


	/// <summary>A window on a disconnected screen is moved onto the nearest screen.</summary>
	[Test]
	public void WindowOnMissingScreenIsMoved()
	{
		Rectangle bounds = new(5000, 300, 800, 600);
		Rectangle result = WindowLayoutProvider._FitToScreen(bounds, _TwoScreens);

		Assert.That(result.Size, Is.EqualTo(bounds.Size));
		Assert.That(_TwoScreens[1].Contains(result), Is.True);
		Assert.That(result.Y, Is.EqualTo(300));
	}


	/// <summary>A window whose caption is above the screen is moved down.</summary>
	[Test]
	public void CaptionAboveScreenIsMoved()
	{
		Rectangle result = WindowLayoutProvider._FitToScreen(new Rectangle(100, -500, 800, 600), _TwoScreens);

		Assert.That(result, Is.EqualTo(new Rectangle(100, 0, 800, 600)));
	}


	/// <summary>A window that is larger than the nearest screen is shrunk.</summary>
	[Test]
	public void LargeWindowIsShrunk()
	{
		Rectangle[] small = new Rectangle[] { new(0, 0, 1280, 720) };
		Rectangle result = WindowLayoutProvider._FitToScreen(new Rectangle(3000, 3000, 2000, 1500), small);

		Assert.That(result, Is.EqualTo(new Rectangle(0, 0, 1280, 720)));
	}


	/// <summary>Without screens, the bounds are not changed.</summary>
	[Test]
	public void NoScreensKeepsBounds()
	{
		Rectangle bounds = new(5000, 5000, 100, 100);

		Assert.That(WindowLayoutProvider._FitToScreen(bounds, Array.Empty<Rectangle>()), Is.EqualTo(bounds));
	}


	/// <summary>The screen configuration key depends on the screen bounds only, not on their order.</summary>
	[Test]
	public void ScreenConfigurationKey()
	{
		string a = WindowLayoutProvider._GetScreenConfigurationKey(_TwoScreens);
		string b = WindowLayoutProvider._GetScreenConfigurationKey(Enumerable.Reverse(_TwoScreens));
		string c = WindowLayoutProvider._GetScreenConfigurationKey(new Rectangle[] { _TwoScreens[0] });

		Assert.That(a, Has.Length.EqualTo(8));
		Assert.That(a, Is.EqualTo(b));
		Assert.That(a, Is.Not.EqualTo(c));
	}


	/// <summary>Logical and device pixels are converted with the DPI.</summary>
	[Test]
	public void ConvertsLogicalPixels()
	{
		Assert.That(WindowLayoutProvider._FromLogical(100, 144), Is.EqualTo(150));
		Assert.That(WindowLayoutProvider._ToLogical(150, 144), Is.EqualTo(100));
		Assert.That(WindowLayoutProvider._ToLogical(150, 0), Is.EqualTo(150));
	}


	/// <summary>Permutations are recognized.</summary>
	[Test]
	public void RecognizesPermutations()
	{
		Assert.That(WindowLayoutProvider._IsPermutation(new int[] { 2, 0, 1 }), Is.True);
		Assert.That(WindowLayoutProvider._IsPermutation(Array.Empty<int>()), Is.True);
		Assert.That(WindowLayoutProvider._IsPermutation(new int[] { 0, 0, 1 }), Is.False);
		Assert.That(WindowLayoutProvider._IsPermutation(new int[] { 1, 2 }), Is.False);
	}
}
