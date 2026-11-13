namespace Robbiblubber.Spellbook.Controls.Samples.Themes;

using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements a window with standard controls, menus, a tool bar and a status bar.</summary>
/// <remarks>The window has a <see cref="ThemeProvider"/>; the tool bar combo box "Window theme" sets its theme, "(Application)" lets
///          the window follow the theme of the application. The page "Buttons" shows standard buttons, check boxes, radio buttons and
///          group boxes, which the theme adapters paint over, next to the themed controls; the page "Text and tabs" does the same for
///          text boxes and tab controls. The icons of the menus and the tool bar are black, the theme manager lightens them in dark
///          themes; the icon of "About" is colored and stays as it is.</remarks>
[SupportedOSPlatform("windows")]
public partial class ControlsWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text of the entry for the theme of the application.</summary>
	private const string _APPLICATION_THEME = "(Application)";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Icons of menus and tool bar.</summary>
	private readonly List<Bitmap> _Icons = new();

	/// <summary>Determines if the theme list is being filled.</summary>
	private bool _Filling = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ControlsWindow()
	{
		InitializeComponent();

		_MenuNew.Image = _ToolNew.Image = _CreateIcon(_DrawNew);
		_MenuOpen.Image = _ToolOpen.Image = _CreateIcon(_DrawOpen);
		_ToolSave.Image = _CreateIcon(_DrawSave);
		_MenuCut.Image = _ContextCut.Image = _CreateIcon(_DrawCut);
		_MenuCopy.Image = _ContextCopy.Image = _CreateIcon(_DrawCopy);
		_MenuPaste.Image = _ContextPaste.Image = _CreateIcon(_DrawPaste);
		_MenuAbout.Image = _CreateIcon(_DrawAbout);
		_ButtonImageStandard.Image = _ButtonImageThemed.Image = _CreateIcon(_DrawSave);
		_CheckTextSelection.Checked = ThemeManager.ThemeTextSelection;

		ImageList tabImages = new() { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(16, 16) };
		tabImages.Images.Add(_CreateIcon(_DrawSave));
		components?.Add(tabImages);
		_TabsStandard.ImageList = _TabsThemed.ImageList = tabImages;
		_PageThirdStandard.ImageIndex = _PageThirdThemed.ImageIndex = 0;
		_TextMultiStandard.Lines = _TextMultiThemed.Lines = Enumerable.Range(1, 12).Select(m => $"Line {m}").ToArray();
		_ComboTabAlignment.SelectedIndex = 0;

		_AutoCompletion.SetSource(_Text, new AutoCompletionItemSource(new[] { "Apple", "Apricot", "Avocado", "Banana", "Blackberry",
								  "Blueberry", "Cherry", "Coconut", "Grape", "Grapefruit", "Lemon", "Lime", "Mango", "Melon", "Orange",
								  "Peach", "Pear", "Pineapple", "Plum", "Raspberry", "Strawberry" }));
		_AutoCompletion.SetAutoCompletionEnabled(_Text, true);
		_AutoCompletion.SetAutoStart(_Text, true);

		_ComboList.SelectedIndex = 0;
		_ComboThemed.Items.Add(new ListItem("Berlin") { Detail = "Germany" });
		_ComboThemed.Items.Add(new ListItem("Vienna") { Detail = "Austria" });
		_ComboThemed.Items.Add(new ListItem("Bern") { Detail = "Switzerland" });
		_ComboThemed.SelectedIndex = 0;

		_TextMultiline.Lines = Enumerable.Range(1, 20).Select(m => $"Line {m}: a long line that makes the horizontal scroll bar appear").ToArray();
		_RichText.AppendText("\r\n\r\nThe rich text box gets field colors as well.");

		_FillLists();
		_FillGrid();
		_PropertyGrid.SelectedObject = _ButtonOk;

		ThemeManager.ThemeChanged += _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged += _ThemeManager_ThemesChanged;

		_FillThemes();
		_ShowTheme();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the icon "New": a sheet with a folded corner.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawNew(Graphics g, Pen pen)
	{
		g.DrawLines(pen, new Point[] { new(9, 1), new(3, 1), new(3, 14), new(13, 14), new(13, 5), new(9, 1), new(9, 5), new(13, 5) });
	}


	/// <summary>Draws the icon "Open": a folder.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawOpen(Graphics g, Pen pen)
	{
		g.DrawLines(pen, new Point[] { new(1, 13), new(1, 3), new(6, 3), new(7, 5), new(13, 5), new(13, 7) });
		g.DrawPolygon(pen, new Point[] { new(1, 13), new(4, 7), new(15, 7), new(12, 13) });
	}


	/// <summary>Draws the icon "Save": a floppy disk.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawSave(Graphics g, Pen pen)
	{
		g.DrawPolygon(pen, new Point[] { new(1, 1), new(12, 1), new(14, 3), new(14, 14), new(1, 14) });
		g.DrawRectangle(pen, 4, 1, 6, 4);
		g.DrawRectangle(pen, 4, 9, 7, 5);
	}


	/// <summary>Draws the icon "Cut": scissors.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawCut(Graphics g, Pen pen)
	{
		g.DrawLine(pen, 4, 1, 10, 10);
		g.DrawLine(pen, 11, 1, 5, 10);
		g.DrawEllipse(pen, 1, 10, 4, 4);
		g.DrawEllipse(pen, 10, 10, 4, 4);
	}


	/// <summary>Draws the icon "Copy": two sheets.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawCopy(Graphics g, Pen pen)
	{
		g.DrawLines(pen, new Point[] { new(3, 11), new(1, 11), new(1, 1), new(9, 1), new(9, 3) });
		g.DrawRectangle(pen, 5, 4, 9, 11);
	}


	/// <summary>Draws the icon "Paste": a clipboard.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawPaste(Graphics g, Pen pen)
	{
		g.DrawRectangle(pen, 2, 2, 11, 12);
		g.FillRectangle(pen.Brush, 5, 1, 5, 3);
		g.DrawLine(pen, 5, 8, 10, 8);
		g.DrawLine(pen, 5, 11, 10, 11);
	}


	/// <summary>Draws the icon "About": a blue circle with a white "i".</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="pen">Pen.</param>
	private static void _DrawAbout(Graphics g, Pen pen)
	{
		g.FillEllipse(Brushes.RoyalBlue, 0, 0, 15, 15);
		g.SmoothingMode = SmoothingMode.None;
		g.FillRectangle(Brushes.White, 7, 3, 2, 2);
		g.FillRectangle(Brushes.White, 7, 6, 2, 6);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates an icon.</summary>
	/// <param name="draw">Drawing method.</param>
	/// <returns>Returns the icon.</returns>
	private Bitmap _CreateIcon(Action<Graphics, Pen> draw)
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(Pen pen = new(Color.Black))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			draw(g, pen);
		}
		_Icons.Add(rval);

		return rval;
	}


	/// <summary>Fills the tree and the list view.</summary>
	private void _FillLists()
	{
		foreach(string i in new[] { "Documents", "Pictures", "Music" })
		{
			TreeNode node = _Tree.Nodes.Add(i);
			for(int k = 1; k <= 3; k++) { node.Nodes.Add($"{i} {k}").Nodes.Add("Item"); }
		}
		_Tree.Nodes[0].Expand();

		for(int i = 1; i <= 40; i++)
		{
			ListViewItem item = new($"File {i}");
			item.SubItems.Add($"{i * 17} KB");
			item.SubItems.Add(((i % 3) == 0) ? "Picture" : "Document");
			_ListView.Items.Add(item);
		}
	}


	/// <summary>Fills the grid.</summary>
	private void _FillGrid()
	{
		foreach(ThemeColor i in Enum.GetValues<ThemeColor>()) { _Grid.Rows.Add(i.ToString(), ((int) i).ToString()); }
	}


	/// <summary>Fills the theme list and selects the theme of the window.</summary>
	private void _FillThemes()
	{
		_Filling = true;
		try
		{
			_ToolComboTheme.Items.Clear();
			_ToolComboTheme.Items.Add(_APPLICATION_THEME);
			foreach(Theme i in ThemeManager.Themes) { _ToolComboTheme.Items.Add(i); }

			if((_ThemeProvider.Theme is not null) && !_ToolComboTheme.Items.Contains(_ThemeProvider.Theme)) { _ThemeProvider.Theme = null; }
			_ToolComboTheme.SelectedItem = ((_ThemeProvider.Theme is null) ? _APPLICATION_THEME : _ThemeProvider.Theme);
		}
		finally { _Filling = false; }
	}


	/// <summary>Shows the effective theme of the window in the status bar.</summary>
	private void _ShowTheme()
	{
		_StatusTheme.Text = $"Theme: {_ThemeProvider.EffectiveTheme.Name}{((_ThemeProvider.Theme is null) ? " (application)" : string.Empty)}";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ThemeChanged event of the theme manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemeChanged(object? sender, EventArgs e)
	{
		_ShowTheme();
	}


	/// <summary>Handles the ThemesChanged event of the theme manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemesChanged(object? sender, EventArgs e)
	{
		_FillThemes();
		_ShowTheme();
	}


	/// <summary>Handles the SelectedIndexChanged event of the theme combo box: sets the theme of the window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ToolComboTheme_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if(_Filling) { return; }

		_ThemeProvider.Theme = (_ToolComboTheme.SelectedItem as Theme);
		_ShowTheme();
	}


	/// <summary>Handles the Click event of the close menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuClose_Click(object? sender, EventArgs e)
	{
		Close();
	}


	/// <summary>Handles the SelectedIndexChanged event of the tab alignment combo box: sets the alignment of both tab controls.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboTabAlignment_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if(_ComboTabAlignment.SelectedIndex < 0) { return; }

		_TabsStandard.Alignment = _TabsThemed.Alignment = (TabAlignment) _ComboTabAlignment.SelectedIndex;
	}


	/// <summary>Handles the CheckedChanged event of the check box "Tabs as buttons": sets the appearance of both tab controls.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _CheckTabButtons_CheckedChanged(object? sender, EventArgs e)
	{
		_TabsStandard.Appearance = _TabsThemed.Appearance = (_CheckTabButtons.Checked ? TabAppearance.Buttons : TabAppearance.Normal);
	}


	/// <summary>Handles the CheckedChanged event of the check box "Text selection in theme colors": switches
	///          <see cref="ThemeManager.ThemeTextSelection"/> for the whole application.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _CheckTextSelection_CheckedChanged(object? sender, EventArgs e)
	{
		ThemeManager.ThemeTextSelection = _CheckTextSelection.Checked;
	}


	/// <summary>Handles the CheckedChanged event of the check box "Disable tab controls".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _CheckTabsDisabled_CheckedChanged(object? sender, EventArgs e)
	{
		_TabsStandard.Enabled = _TabsThemed.Enabled = (!_CheckTabsDisabled.Checked);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		ThemeManager.ThemeChanged -= _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged -= _ThemeManager_ThemesChanged;
		foreach(Bitmap i in _Icons) { i.Dispose(); }
		_Icons.Clear();

		base.OnFormClosed(e);
	}
}
