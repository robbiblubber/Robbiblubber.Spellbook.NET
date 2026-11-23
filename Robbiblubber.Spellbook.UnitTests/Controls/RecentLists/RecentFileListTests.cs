namespace Robbiblubber.Spellbook.UnitTests.Controls.RecentLists;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="RecentFileList"/> component.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class RecentFileListTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List.</summary>
	private RecentFileList _List = null!;

	/// <summary>Temporary directory.</summary>
	private string _Dir = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_List = new RecentFileList() { Store = new TestRecentListStore() };
		_Dir = Path.Combine(Path.GetTempPath(), "spellbook_recent_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Dir);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_List.Dispose();
		if(Directory.Exists(_Dir)) { Directory.Delete(_Dir, true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Paths are normalized and compared case-insensitively.</summary>
	[Test]
	public void NormalizesPaths()
	{
		string file = Path.Combine(_Dir, "Report.txt");

		_List.Add(Path.Combine(_Dir, "sub", "..", "Report.txt"));
		_List.Add("\"" + file.ToUpperInvariant() + "\"");
		_List.Add(_Dir + Path.DirectorySeparatorChar);

		Assert.That(_List.Items.Count, Is.EqualTo(2));
		Assert.That(_List.Items[0].Key, Is.EqualTo(_Dir));
		Assert.That(_List.Items[1].Key, Is.EqualTo(file.ToUpperInvariant()));
		Assert.That(_List.Contains(file), Is.True);
	}


	/// <summary>Invalid paths are ignored.</summary>
	[Test]
	public void IgnoresInvalidPaths()
	{
		Assert.That(_List.Add("C:\\a\0b"), Is.Null);
		Assert.That(_List.Items, Is.Empty);
	}


	/// <summary>The menu shows file names, folder names or shortened full paths; the tool tip shows the full path.</summary>
	[Test]
	public void ShowsFileNames()
	{
		using ToolStripMenuItem menu = new();
		string file = Path.Combine(_Dir, "Report.txt");

		_List.ShowNumbers = false;
		_List.ShowClearItem = false;
		_List.Add(file);
		_List.MenuItem = menu;

		Assert.That(menu.DropDownItems[0].Text, Is.EqualTo("Report.txt"));
		Assert.That(menu.DropDownItems[0].ToolTipText, Is.EqualTo(file));

		_List.DisplayStyle = RecentFileDisplayStyle.FULL_PATH;
		_List.MaxPathLength = 1000;
		_List.Reload();
		Assert.That(menu.DropDownItems[0].Text, Is.EqualTo(file));
	}


	/// <summary>Missing files are shown grayed and reported as not available.</summary>
	[Test]
	public void GraysMissingFiles()
	{
		using ToolStripMenuItem menu = new();
		string existing = Path.Combine(_Dir, "exists.txt");
		string missing = Path.Combine(_Dir, "missing.txt");
		List<bool?> available = new();

		File.WriteAllText(existing, "x");
		_List.ShowClearItem = false;
		_List.Add(missing);
		_List.Add(existing);
		_List.MenuItem = menu;
		_List.ItemSelected += (sender, e) => { available.Add(e.Available); };

		Assert.That(menu.DropDownItems[0].ForeColor, Is.Not.EqualTo(SystemColors.GrayText));
		Assert.That(menu.DropDownItems[1].ForeColor, Is.EqualTo(SystemColors.GrayText));

		menu.DropDownItems[0].PerformClick();
		menu.DropDownItems[1].PerformClick();
		Assert.That(available, Is.EqualTo(new bool?[] { true, false }));
	}


	/// <summary>Files and folders show the shell icon of their type, also if they do not exist.</summary>
	[Test]
	public void ShowsShellIcons()
	{
		using ToolStripMenuItem menu = new();

		_List.ShowClearItem = false;
		_List.Add(Path.Combine(_Dir, "missing.txt"));
		_List.Add(_Dir);
		_List.MenuItem = menu;

		Assert.That(menu.DropDownItems[0].Image, Is.Not.Null);
		Assert.That(menu.DropDownItems[1].Image, Is.Not.Null);
		Assert.That(menu.DropDownItems[0].Image, Is.Not.SameAs(menu.DropDownItems[1].Image));
	}


	/// <summary>Long paths are shortened in the middle, keeping the root and the file name.</summary>
	[Test]
	public void CompactsPaths()
	{
		string path = @"C:\Users\someone\Documents\Projects\Spellbook\Reports\2026\October\Report.docx";

		Assert.That(RecentFileList._CompactPath(@"C:\a\b.txt", 60), Is.EqualTo(@"C:\a\b.txt"));
		Assert.That(RecentFileList._CompactPath(path, 40), Is.EqualTo("C:\\\u2026\\Reports\\2026\\October\\Report.docx"));
		Assert.That(RecentFileList._CompactPath(path, 10), Is.EqualTo("C:\\\u2026\\Report.docx"));
		Assert.That(RecentFileList._CompactPath(path, 40).Length, Is.LessThanOrEqualTo(40));
	}
}
