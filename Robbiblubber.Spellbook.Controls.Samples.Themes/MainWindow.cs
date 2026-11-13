namespace Robbiblubber.Spellbook.Controls.Samples.Themes;

using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>The combo box selects the theme of the application; all controls without an own theme follow it immediately. The
///          buttons create, change, export and import custom themes. The list shows the effective colors of the theme; double-click a
///          role to set its color in a custom theme, press Delete to reset it. The window has a <see cref="ThemeProvider"/> without an own
///          theme, so the theme adapters apply the theme of the application to all its controls. "Controls window" opens a window
///          with standard controls, menus and a tool bar that can have its own theme.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;

	/// <summary>File filter for theme files.</summary>
	private const string _THEME_FILTER = "Theme files (*.theme)|*.theme|All files (*.*)|*.*";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Message box samples, shown one after the other.</summary>
	private static readonly (string Text, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefaultButton)[] _MESSAGES =
	{
		("The theme has been saved.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1),
		("Do you want to save the changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1),
		("Delete the theme?\r\n\r\nThis cannot be undone.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2),
		("The file could not be read.", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2),
		("The connection was lost.", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2),
		("The device is not ready.", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1),
		(string.Join(" ", Enumerable.Repeat("This is a long message that wraps over several lines.", 12)), MessageBoxButtons.OKCancel, MessageBoxIcon.None,
		 MessageBoxDefaultButton.Button1),
		(string.Join("\r\n", Enumerable.Range(1, 80).Select(m => $"Line {m}")), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Images of the palette list.</summary>
	private readonly ImageList _PaletteImages = new() { ColorDepth = ColorDepth.Depth32Bit };

	/// <summary>Images in the palette image list; the image list needs them until its handle is created.</summary>
	private readonly List<Bitmap> _Swatches = new();

	/// <summary>Number of created themes, used for their names.</summary>
	private int _Created = 0;

	/// <summary>Determines if the theme list is being filled.</summary>
	private bool _Filling = false;

	/// <summary>Index of the next message box sample.</summary>
	private int _NextMessage = 0;

	/// <summary>Color dialog, which keeps its custom colors while the sample runs.</summary>
	private readonly ThemedColorDialog _ColorDialog = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		_PaletteImages.ImageSize = new Size(LogicalToDeviceUnits(16), LogicalToDeviceUnits(16));
		_ListPalette.SmallImageList = _PaletteImages;

		_ComboSample.Items.Add(new ListItem("Berlin") { Detail = "Germany" });
		_ComboSample.Items.Add(new ListItem("Vienna") { Detail = "Austria" });
		_ComboSample.Items.Add(new ListItem("Bern") { Detail = "Switzerland" });
		_ComboSample.SelectedIndex = 0;

		ThemeManager.Store ??= Program.CreateThemeStore();
		if(Program.InstanceName is not null) { Text += " - " + Program.InstanceName; }
		ThemeManager.ThemeChanged += _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged += _ThemeManager_ThemesChanged;

		_FillThemes();
		_ShowTheme();
		_Log("Ready.");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the theme of the application if it is a custom theme.</summary>
	private Theme? _CustomTheme
	{
		get { return (ThemeManager.Theme.IsBuiltIn ? null : ThemeManager.Theme); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Disposes the images of the palette list.</summary>
	private void _DisposeSwatches()
	{
		foreach(Bitmap i in _Swatches) { i.Dispose(); }
		_Swatches.Clear();
	}


	/// <summary>Creates the image of a palette entry.</summary>
	/// <param name="color">Color.</param>
	/// <param name="size">Image size.</param>
	/// <returns>Returns the image.</returns>
	private static Bitmap _CreateSwatch(Color color, Size size)
	{
		Bitmap rval = new(size.Width, size.Height);

		using Graphics g = Graphics.FromImage(rval);
		using SolidBrush brush = new(color);
		g.FillRectangle(brush, 1, 1, (size.Width - 2), (size.Height - 2));
		g.DrawRectangle(Pens.Gray, 0, 0, (size.Width - 1), (size.Height - 1));

		return rval;
	}


	/// <summary>Gets the text representation of a color.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the color as <c>#RRGGBB</c> or <c>#AARRGGBB</c>.</returns>
	private static string _ColorText(Color color)
	{
		return ((color.A == 255) ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log entry.</summary>
	/// <param name="message">Message.</param>
	private void _Log(string message)
	{
		_ListLog.BeginUpdate();
		_ListLog.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {message}");
		while(_ListLog.Items.Count > _MAX_LOG_ENTRIES) { _ListLog.Items.RemoveAt(0); }
		_ListLog.TopIndex = _ListLog.Items.Count - 1;
		_ListLog.EndUpdate();
	}


	/// <summary>Fills the theme list and selects the theme of the application.</summary>
	private void _FillThemes()
	{
		_Filling = true;
		try
		{
			_ComboTheme.Items.Clear();
			foreach(Theme i in ThemeManager.Themes) { _ComboTheme.Items.Add(i); }
			_ComboTheme.SelectedItem = ThemeManager.Theme;
		}
		finally { _Filling = false; }
	}


	/// <summary>Shows the theme of the application: palette, information and buttons.</summary>
	private void _ShowTheme()
	{
		Theme theme = ThemeManager.Theme;

		_ShowPalette();

		_LabelInfo.Text = $"Id: {theme.Id}\r\nBase: {(theme.Base?.Name ?? "-")}\r\nDark: {(theme.IsDark ? "yes" : "no")}, " +
						  $"built-in: {(theme.IsBuiltIn ? "yes" : "no")}";

		bool custom = (_CustomTheme is not null);
		_ButtonRemove.Enabled = _ButtonAccent.Enabled = _ButtonBack.Enabled = _ButtonResetColors.Enabled = custom;
		_ButtonBase.Enabled = _ButtonExport.Enabled = custom;

		if(!_Filling && (_ComboTheme.SelectedItem != theme))
		{
			_Filling = true;
			try { _ComboTheme.SelectedItem = theme; } finally { _Filling = false; }
		}
	}


	/// <summary>Shows the effective colors of the theme of the application.</summary>
	private void _ShowPalette()
	{
		Theme theme = ThemeManager.Theme;
		int selected = ((_ListPalette.SelectedIndices.Count > 0) ? _ListPalette.SelectedIndices[0] : -1);

		_ListPalette.BeginUpdate();
		try
		{
			_ListPalette.Items.Clear();
			_PaletteImages.Images.Clear();
			_DisposeSwatches();

			foreach(ThemeColor i in Enum.GetValues<ThemeColor>())
			{
				Color color = theme[i];
				Bitmap swatch = _CreateSwatch(color, _PaletteImages.ImageSize);
				_Swatches.Add(swatch);
				_PaletteImages.Images.Add(swatch);

				ListViewItem item = new(i.ToString(), (_PaletteImages.Images.Count - 1)) { Tag = i };
				item.SubItems.Add(_ColorText(color));
				item.SubItems.Add(theme.GetValue(i).IsEmpty ? string.Empty : "yes");
				_ListPalette.Items.Add(item);
			}

			if((selected >= 0) && (selected < _ListPalette.Items.Count))
			{
				_ListPalette.Items[selected].Selected = true;
				_ListPalette.Items[selected].Focused = true;
			}
		}
		finally { _ListPalette.EndUpdate(); }
	}


	/// <summary>Lets the user choose a color for a role of the custom theme.</summary>
	/// <param name="color">Color role.</param>
	/// <remarks>With Shift, the color dialog allows transparent colors.</remarks>
	private void _ChooseColor(ThemeColor color)
	{
		Theme? theme = _CustomTheme;
		if(theme is null) { _Log("Built-in themes cannot be changed. Create a custom theme first."); return; }

		_ColorDialog.Color = theme[color];
		_ColorDialog.AllowAlpha = ((ModifierKeys & Keys.Shift) == Keys.Shift);
		if(_ColorDialog.ShowDialog(this) != DialogResult.OK) { return; }

		theme.SetValue(color, _ColorDialog.Color);
		_Log($"SetValue  {color} = {_ColorText(_ColorDialog.Color)}");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ThemeChanged event of the theme manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemeChanged(object? sender, EventArgs e)
	{
		_Log($"ThemeChanged   Theme = {ThemeManager.Theme.Name}, IsDark = {ThemeManager.Theme.IsDark}");
		_ShowTheme();
	}


	/// <summary>Handles the ThemesChanged event of the theme manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemesChanged(object? sender, EventArgs e)
	{
		_Log($"ThemesChanged  {ThemeManager.CustomThemes.Count} custom theme(s)");
		_FillThemes();
	}


	/// <summary>Handles the SelectedIndexChanged event of the theme combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboTheme_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if(_Filling || (_ComboTheme.SelectedItem is not Theme theme)) { return; }

		ThemeManager.Theme = theme;
	}


	/// <summary>Handles the Click event of the new theme button: creates a theme based on the current theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonNew_Click(object? sender, EventArgs e)
	{
		Theme theme = new($"Custom {++_Created}", ThemeManager.Theme);

		ThemeManager.Add(theme);
		ThemeManager.Theme = theme;
		_Log($"Created \"{theme.Name}\" based on \"{theme.Base!.Name}\".");
	}


	/// <summary>Handles the Click event of the remove button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRemove_Click(object? sender, EventArgs e)
	{
		Theme? theme = _CustomTheme;
		if(theme is null) { return; }

		ThemeManager.Remove(theme);
		_Log($"Removed \"{theme.Name}\".");
	}


	/// <summary>Handles the Click event of the accent color button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAccent_Click(object? sender, EventArgs e)
	{
		_ChooseColor(ThemeColor.ACCENT);
	}


	/// <summary>Handles the Click event of the back color button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBack_Click(object? sender, EventArgs e)
	{
		_ChooseColor(ThemeColor.BACK);
	}


	/// <summary>Handles the Click event of the reset colors button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonResetColors_Click(object? sender, EventArgs e)
	{
		_CustomTheme?.Reset();
		_Log("Reset");
	}


	/// <summary>Handles the Click event of the switch base button: switches the base theme between light and dark.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBase_Click(object? sender, EventArgs e)
	{
		Theme? theme = _CustomTheme;
		if(theme is null) { return; }

		theme.Base = ((theme.Base == Theme.DARK) ? Theme.LIGHT : Theme.DARK);
		_Log($"Base = {theme.Base.Name}");
	}


	/// <summary>Handles the Click event of the export button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonExport_Click(object? sender, EventArgs e)
	{
		Theme? theme = _CustomTheme;
		if(theme is null) { return; }

		using SaveFileDialog dialog = new() { Filter = _THEME_FILTER, FileName = theme.Name + ".theme" };
		if(dialog.ShowDialog(this) != DialogResult.OK) { return; }

		theme.Export(dialog.FileName);
		_Log($"Exported to {dialog.FileName}");
	}


	/// <summary>Handles the Click event of the import button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonImport_Click(object? sender, EventArgs e)
	{
		using OpenFileDialog dialog = new() { Filter = _THEME_FILTER };
		if(dialog.ShowDialog(this) != DialogResult.OK) { return; }

		try
		{
			Theme theme = Theme.Import(dialog.FileName);
			ThemeManager.Add(theme);
			ThemeManager.Theme = theme;
			_Log($"Imported \"{theme.Name}\" (Id {theme.Id}) based on \"{theme.Base!.Name}\".");
		}
		catch(Exception ex) { _Log($"Import failed: {ex.GetType().Name}: {ex.Message}"); }
	}


	/// <summary>Handles the Click event of the save button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonSave_Click(object? sender, EventArgs e)
	{
		ThemeManager.Save();
		if(ThemeManager.Store is SharedThemeStore store) { _Log($"Saved themes to {store.ThemeDirectory}, selection to {store.ConfigFile}"); }
	}


	/// <summary>Handles the Click event of the load button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonLoad_Click(object? sender, EventArgs e)
	{
		ThemeManager.Load(Program.CreateThemeStore());
		_Log($"Loaded {ThemeManager.CustomThemes.Count} custom theme(s), theme = {ThemeManager.Theme.Name}");
		_ShowTheme();
	}


	/// <summary>Handles the Click event of the controls window button: opens a window with standard controls.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonControls_Click(object? sender, EventArgs e)
	{
		new ControlsWindow().Show(this);
	}


	/// <summary>Handles the Click event of the theme editor button: opens the theme editor.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonEditor_Click(object? sender, EventArgs e)
	{
		using ThemeEditorDialog dialog = new();
		DialogResult result = dialog.ShowDialog(this);

		_Log($"ThemeEditorDialog  {result}, theme {ThemeManager.Theme.Name}");
	}


	/// <summary>Handles the Click event of the message box button: shows the next message box sample.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonMessageBox_Click(object? sender, EventArgs e)
	{
		(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) = _MESSAGES[_NextMessage];
		_NextMessage = ((_NextMessage + 1) % _MESSAGES.Length);

		DialogResult result = ThemedMessageBox.Show(this, text, $"ThemedMessageBox ({buttons})", buttons, icon, defaultButton);
		_Log($"ThemedMessageBox  {buttons}, {icon}: {result}");
	}


	/// <summary>Handles the DoubleClick event of the palette list: sets the color of the selected role.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListPalette_DoubleClick(object? sender, EventArgs e)
	{
		if((_ListPalette.SelectedItems.Count == 0) || (_ListPalette.SelectedItems[0].Tag is not ThemeColor color)) { return; }

		_ChooseColor(color);
	}


	/// <summary>Handles the KeyDown event of the palette list: Delete resets the color of the selected role.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListPalette_KeyDown(object? sender, KeyEventArgs e)
	{
		if((e.KeyCode != Keys.Delete) || (_CustomTheme is null)) { return; }
		if((_ListPalette.SelectedItems.Count == 0) || (_ListPalette.SelectedItems[0].Tag is not ThemeColor color)) { return; }

		_CustomTheme.SetValue(color, Color.Empty);
		_Log($"SetValue  {color} = Empty");
		e.Handled = true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		ThemeManager.ThemeChanged -= _ThemeManager_ThemeChanged;
		ThemeManager.ThemesChanged -= _ThemeManager_ThemesChanged;
		_PaletteImages.Dispose();
		_DisposeSwatches();

		base.OnFormClosed(e);
	}
}
