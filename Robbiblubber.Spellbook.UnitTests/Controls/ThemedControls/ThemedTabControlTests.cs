namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedControls;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests the <see cref="ThemedTabControl"/> control.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedTabControlTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form.</summary>
	private Form _Form = null!;

	/// <summary>Tested control.</summary>
	private ThemedTabControl _Tabs = null!;



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

		_Form = new Form() { BackColor = Color.Green };
		_Tabs = new ThemedTabControl() { Size = new Size(240, 140) };
		_Tabs.TabPages.Add(new TabPage("One"));
		_Tabs.TabPages.Add(new TabPage("Two"));
		_Form.Controls.Add(_Tabs);

		_ = _Form.Handle;
		_ = _Tabs.Handle;
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager.Reset(_Form);
		_Tabs.Dispose();
		_Form.Dispose();
		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The selected tab has the accent line, the page area a frame in the border color.</summary>
	[Test]
	public void DrawsSelectedTab()
	{
		_Tabs.Colors.AccentColor = Color.Red;
		_Tabs.Colors.BorderColor = Color.Blue;
		Rectangle tab = _Tabs.GetTabRect(0);

		using Bitmap bitmap = _Render(_Tabs);
		_AssertColor(bitmap.GetPixel(tab.Left + (tab.Width / 2), tab.Top), Color.Red);
		_AssertColor(bitmap.GetPixel(tab.Left, tab.Top + (tab.Height / 2)), Color.Blue);
		_AssertColor(bitmap.GetPixel(_Tabs.Width / 2, _Tabs.Height - 1), Color.Blue);
	}


	/// <summary>Tabs that are not selected show the back color of the parent; the selected tab moves with the selection.</summary>
	[Test]
	public void DrawsOtherTabs()
	{
		Rectangle tab = _Tabs.GetTabRect(1);
		using(Bitmap bitmap = _Render(_Tabs))
		{
			_AssertColor(bitmap.GetPixel(tab.Left + 2, tab.Top + 2), Color.Green);
		}

		_Tabs.Colors.AccentColor = Color.Red;
		_Tabs.SelectedIndex = 1;
		tab = _Tabs.GetTabRect(1);
		using(Bitmap bitmap = _Render(_Tabs))
		{
			_AssertColor(bitmap.GetPixel(tab.Left + (tab.Width / 2), tab.Top), Color.Red);
		}
	}


	/// <summary>In owner draw mode the control raises DrawItem for each tab.</summary>
	[Test]
	public void RaisesDrawItem()
	{
		List<int> drawn = new();
		_Tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
		_Tabs.DrawItem += (sender, e) => drawn.Add(e.Index);

		using(Bitmap bitmap = _Render(_Tabs)) {}
		Assert.That(drawn, Is.EquivalentTo(new[] { 0, 1 }));
	}


	/// <summary>The theme manager applies the theme to the tab control and its pages.</summary>
	[Test]
	public void ThemesPages()
	{
		ThemeManager.Apply(_Form, Theme.DARK);

		Assert.That(_Tabs.Colors.EffectiveTheme, Is.SameAs(Theme.DARK));
		_AssertColor(_Tabs.TabPages[0].BackColor, Theme.DARK[ThemeColor.BACK]);
		_AssertColor(_Tabs.TabPages[1].BackColor, Theme.DARK[ThemeColor.BACK]);
	}
}
