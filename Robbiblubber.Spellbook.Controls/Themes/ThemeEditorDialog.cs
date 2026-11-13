namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class implements a dialog that selects the theme of the application and edits custom themes.</summary>
/// <remarks><para>The dialog lists the built-in and the custom themes. Selecting a theme makes it the theme of the application at once,
///          so the whole application shows it while the user edits it; custom themes can be created, copied, removed, imported and
///          exported, renamed, based on another theme and given their own colors. The preview shows the themed controls.</para>
///          <para>OK keeps the changes and the selected theme and, if <see cref="SaveOnOk"/> is set, saves them with
///          <see cref="ThemeManager.Save(IThemeStore?)"/>. Cancel restores the themes and the theme of the application as they were
///          when the dialog was opened.</para>
///          <para>When it is opened, the dialog takes over the changes that other applications have made to shared themes with
///          <see cref="ThemeManager.Refresh"/>.</para>
///          <para><c>using ThemeEditorDialog dialog = new() { SaveOnOk = true };
///dialog.ShowDialog(this);</c></para>
///          <para>The texts come from <see cref="ThemeEditorTexts"/>.</para></remarks>
[SupportedOSPlatform("windows")]
public partial class ThemeEditorDialog: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _SavedTheme                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class holds the state of a custom theme when the dialog has been opened.</summary>
	private sealed class _SavedTheme
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="theme">Theme.</param>
		public _SavedTheme(Theme theme)
		{
			Theme = theme;
			Name = theme.Name;
			Copy = theme.Copy(theme.Name);
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the theme.</summary>
		public Theme Theme
		{
			get;
		}


		/// <summary>Gets the name.</summary>
		public string Name
		{
			get;
		}


		/// <summary>Gets a copy with the base theme and the colors.</summary>
		public Theme Copy
		{
			get;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color groups: text and roles.</summary>
	private static readonly (Func<string> Text, ThemeColor[] Roles)[] _Groups = new (Func<string>, ThemeColor[])[]
	{
		(() => ThemeEditorTexts.GroupGeneral, new[] { ThemeColor.BACK, ThemeColor.TEXT, ThemeColor.GRAY_TEXT, ThemeColor.GLYPH, ThemeColor.SEPARATOR, ThemeColor.LINK, ThemeColor.HOT_BACK }),
		(() => ThemeEditorTexts.GroupFields, new[] { ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT, ThemeColor.FIELD_BORDER, ThemeColor.FIELD_BORDER_HOT, ThemeColor.FIELD_BORDER_FOCUSED, ThemeColor.READ_ONLY_BACK, ThemeColor.READ_ONLY_TEXT }),
		(() => ThemeEditorTexts.GroupButtons, new[] { ThemeColor.BUTTON_BACK, ThemeColor.BUTTON_TEXT, ThemeColor.BUTTON_BORDER, ThemeColor.BUTTON_BACK_HOT, ThemeColor.BUTTON_BACK_PRESSED }),
		(() => ThemeEditorTexts.GroupAccent, new[] { ThemeColor.ACCENT, ThemeColor.ACCENT_TEXT, ThemeColor.ACCENT_HOT, ThemeColor.ACCENT_PRESSED }),
		(() => ThemeEditorTexts.GroupSelection, new[] { ThemeColor.SELECTION_BACK, ThemeColor.SELECTION_TEXT, ThemeColor.INACTIVE_SELECTION_BACK, ThemeColor.INACTIVE_SELECTION_TEXT }),
		(() => ThemeEditorTexts.GroupDisabled, new[] { ThemeColor.DISABLED_BACK, ThemeColor.DISABLED_TEXT, ThemeColor.DISABLED_BORDER }),
		(() => ThemeEditorTexts.GroupLists, new[] { ThemeColor.HEADER_BACK, ThemeColor.HEADER_TEXT, ThemeColor.GRID_LINE, ThemeColor.TRACK_BACK, ThemeColor.TRACK_BORDER }),
		(() => ThemeEditorTexts.GroupMenus, new[] { ThemeColor.POPUP_BACK, ThemeColor.POPUP_TEXT, ThemeColor.POPUP_BORDER, ThemeColor.MENU_BAR_BACK, ThemeColor.MENU_BACK, ThemeColor.MENU_TEXT, ThemeColor.MENU_HIGHLIGHT_BACK, ThemeColor.MENU_HIGHLIGHT_TEXT, ThemeColor.MENU_BORDER }),
		(() => ThemeEditorTexts.GroupStatus, new[] { ThemeColor.STATUS_BACK, ThemeColor.STATUS_TEXT, ThemeColor.ERROR, ThemeColor.WARNING, ThemeColor.SUCCESS })
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme of the application when the dialog has been opened.</summary>
	private readonly Theme _OriginalTheme;

	/// <summary>Custom themes when the dialog has been opened.</summary>
	private readonly List<_SavedTheme> _OriginalThemes;

	/// <summary>Color squares of the color list.</summary>
	private readonly ImageList _Swatches = new();

	/// <summary>Color dialog, which keeps its custom colors while the editor is open.</summary>
	private readonly ThemedColorDialog _ColorDialog = new();

	/// <summary>Determines if the controls are being filled.</summary>
	private bool _Filling = false;

	/// <summary>Determines if the changes have been kept.</summary>
	private bool _Kept = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemeEditorDialog()
	{
		InitializeComponent();
		_ApplyTexts();

		ThemeManager.Refresh();
		_OriginalTheme = ThemeManager.Theme;
		_OriginalThemes = ThemeManager.CustomThemes.Select(m => new _SavedTheme(m)).ToList();

		int size = LogicalToDeviceUnits(16);
		_Swatches.ColorDepth = ColorDepth.Depth32Bit;
		_Swatches.ImageSize = new Size(size, size);
		_ListColors.SmallImageList = _Swatches;
		components!.Add(_Swatches);
		components.Add(_ColorDialog);
		_FillColors();

		_FillThemes(ThemeManager.Theme);
		ThemeManager.Apply(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a name for a new theme that is not used yet.</summary>
	/// <param name="format">Format with a placeholder for a number or a name.</param>
	/// <param name="argument">Name for the placeholder, NULL to insert a number.</param>
	/// <returns>Returns the name.</returns>
	private static string _GetUnusedName(string format, string? argument)
	{
		HashSet<string> used = new(ThemeManager.Themes.Select(m => m.Name), StringComparer.CurrentCultureIgnoreCase);

		if(argument is not null)
		{
			string rval = string.Format(format, argument);
			if(!used.Contains(rval)) { return rval; }
			format = (rval + " {0}");
		}

		for(int i = 1; ; i++)
		{
			string rval = string.Format(format, i);
			if(!used.Contains(rval)) { return rval; }
		}
	}


	/// <summary>Converts a color to a text.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the text, for example <c>#2B2B2B</c>, with alpha for transparent colors.</returns>
	private static string _ColorToText(Color color)
	{
		return ((color.A == 255) ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the texts from <see cref="ThemeEditorTexts"/>.</summary>
	private void _ApplyTexts()
	{
		Text = ThemeEditorTexts.Title;
		_LabelThemes.Text = ThemeEditorTexts.Themes;
		_ButtonNew.Text = ThemeEditorTexts.New;
		_ButtonCopy.Text = ThemeEditorTexts.Copy;
		_ButtonRemove.Text = ThemeEditorTexts.Remove;
		_ButtonImport.Text = ThemeEditorTexts.Import;
		_ButtonExport.Text = ThemeEditorTexts.Export;
		_LabelName.Text = ThemeEditorTexts.Name;
		_LabelBase.Text = ThemeEditorTexts.Base;
		_LabelColors.Text = ThemeEditorTexts.Colors;
		_ColumnRole.Text = ThemeEditorTexts.ColumnRole;
		_ColumnValue.Text = ThemeEditorTexts.ColumnValue;
		_ColumnSet.Text = ThemeEditorTexts.ColumnSet;
		_ButtonChange.Text = ThemeEditorTexts.ChangeColor;
		_ButtonResetColor.Text = ThemeEditorTexts.ResetColor;
		_ButtonResetAll.Text = ThemeEditorTexts.ResetAll;
		_LabelInfo.Text = ThemeEditorTexts.BuiltIn;
		_GroupPreview.Text = ThemeEditorTexts.Preview;
		_PreviewLabel.Text = ThemeEditorTexts.PreviewLabel;
		_PreviewText.Text = ThemeEditorTexts.PreviewTextBox;
		_PreviewCheck.Text = ThemeEditorTexts.PreviewCheckBox;
		_PreviewRadio.Text = ThemeEditorTexts.PreviewOption;
		_PreviewButton.Text = ThemeEditorTexts.PreviewButton;
		_PreviewDisabled.Text = ThemeEditorTexts.PreviewDisabled;
		_PreviewPage1.Text = ThemeEditorTexts.PreviewPage;
		_PreviewPage2.Text = ThemeEditorTexts.PreviewOtherPage;
		_PreviewDisabledText.Text = ThemeEditorTexts.PreviewDisabledText;
		_PreviewLink.Text = ThemeEditorTexts.PreviewLink;

		_PreviewCombo.Items.Clear();
		_PreviewCombo.Items.Add(new ListItem(ThemeEditorTexts.PreviewComboBox));
		_PreviewCombo.SelectedIndex = 0;

		_PreviewList.Items.Clear();
		for(int i = 1; i <= 4; i++) { _PreviewList.Items.Add((i == 2) ? ThemeEditorTexts.PreviewSelectedItem : string.Format(ThemeEditorTexts.PreviewItem, i)); }
		_PreviewList.SelectedIndex = 1;
		_ButtonOk.Text = ThemeEditorTexts.Ok;
		_ButtonCancel.Text = ThemeEditorTexts.Cancel;
	}


	/// <summary>Fills the theme list.</summary>
	/// <param name="selected">Theme to select.</param>
	private void _FillThemes(Theme selected)
	{
		_Filling = true;
		_ListThemes.BeginUpdate();
		_ListThemes.Items.Clear();
		foreach(Theme i in ThemeManager.Themes) { _ListThemes.Items.Add(i); }
		_ListThemes.EndUpdate();
		_Filling = false;

		int index = _ListThemes.Items.IndexOf(selected);
		_ListThemes.SelectedIndex = ((index >= 0) ? index : 0);
	}


	/// <summary>Creates the items of the color list.</summary>
	private void _FillColors()
	{
		_ListColors.BeginUpdate();
		foreach((Func<string> text, ThemeColor[] roles) in _Groups)
		{
			ListViewGroup group = new(text());
			_ListColors.Groups.Add(group);

			foreach(ThemeColor i in roles)
			{
				ListViewItem item = new(ThemeEditorTexts._GetRoleName(i), group) { Tag = i, ImageKey = i.ToString() };
				item.SubItems.Add(string.Empty);
				item.SubItems.Add(string.Empty);
				_ListColors.Items.Add(item);
			}
		}
		_ListColors.EndUpdate();
	}


	/// <summary>Shows the selected theme: name, base theme and colors.</summary>
	private void _ShowTheme()
	{
		Theme theme = _SelectedTheme;
		bool custom = !theme.IsBuiltIn;

		_Filling = true;
		_TextName.Text = (custom ? theme.Name : ThemeSelectorTexts.GetName(theme));
		_TextName.ReadOnly = !custom;

		_ComboBase.BeginUpdate();
		_ComboBase.Items.Clear();
		if(custom)
		{
			foreach(Theme i in ThemeManager.Themes.Where(m => !m.DependsOn(theme))) { _ComboBase.Items.Add(i); }
			_ComboBase.SelectedItem = theme.Base;
		}
		_ComboBase.EndUpdate();
		_ComboBase.Enabled = custom;
		_Filling = false;

		_ButtonRemove.Enabled = _ButtonExport.Enabled = _ButtonResetAll.Enabled = custom;
		_LabelInfo.Visible = !custom;

		_ShowColors();
	}


	/// <summary>Shows the colors of the selected theme.</summary>
	private void _ShowColors()
	{
		Theme theme = _SelectedTheme;

		_ListColors.BeginUpdate();
		foreach(ListViewItem i in _ListColors.Items)
		{
			ThemeColor role = (ThemeColor) i.Tag!;
			Color color = theme[role];

			_SetSwatch(role, color);
			i.SubItems[1].Text = _ColorToText(color);
			i.SubItems[2].Text = (theme.GetValue(role).IsEmpty ? string.Empty : ThemeEditorTexts.IsSet);
		}
		_ListColors.EndUpdate();

		_UpdateColorButtons();
	}


	/// <summary>Draws the color square of a role.</summary>
	/// <param name="role">Color role.</param>
	/// <param name="color">Color.</param>
	private void _SetSwatch(ThemeColor role, Color color)
	{
		Size size = _Swatches.ImageSize;
		Bitmap bitmap = new(size.Width, size.Height);

		using(Graphics g = Graphics.FromImage(bitmap))
		{
			using(SolidBrush brush = new(color)) { g.FillRectangle(brush, 1, 1, size.Width - 2, size.Height - 2); }
			using(Pen pen = new(Color.Gray)) { g.DrawRectangle(pen, 1, 1, size.Width - 3, size.Height - 3); }
		}

		string key = role.ToString();
		int index = _Swatches.Images.IndexOfKey(key);
		if(index >= 0) { _Swatches.Images.RemoveAt(index); }
		_Swatches.Images.Add(key, bitmap);
	}


	/// <summary>Enables the buttons for the selected color.</summary>
	private void _UpdateColorButtons()
	{
		bool custom = !_SelectedTheme.IsBuiltIn;
		ThemeColor? role = _SelectedRole;

		_ButtonChange.Enabled = (custom && role.HasValue);
		_ButtonResetColor.Enabled = (custom && (role is ThemeColor r) && !_SelectedTheme.GetValue(r).IsEmpty);
	}


	/// <summary>Lets the user choose the color of the selected role.</summary>
	private void _ChangeColor()
	{
		if(_SelectedTheme.IsBuiltIn || (_SelectedRole is not ThemeColor role)) { return; }

		_ColorDialog.Color = _SelectedTheme[role];
		_ColorDialog.Title = ThemeEditorTexts._GetRoleName(role);
		if(_ColorDialog.ShowDialog(this) != DialogResult.OK) { return; }

		_SelectedTheme.SetValue(role, _ColorDialog.Color);
		_ShowColors();
	}


	/// <summary>Resets the color of the selected role to the color of the base theme.</summary>
	private void _ResetColor()
	{
		if(_SelectedTheme.IsBuiltIn || (_SelectedRole is not ThemeColor role)) { return; }

		_SelectedTheme.SetValue(role, Color.Empty);
		_ShowColors();
	}


	/// <summary>Adds a theme, makes it the selected theme and selects its name.</summary>
	/// <param name="theme">Theme.</param>
	private void _AddTheme(Theme theme)
	{
		ThemeManager.Add(theme);
		_FillThemes(theme);
		_TextName.Focus();
		_TextName.SelectAll();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Restores the custom themes and the theme of the application as they were when the dialog has been opened.</summary>
	internal void _Restore()
	{
		foreach(_SavedTheme i in _OriginalThemes)
		{
			Theme root = i.Theme.Base!;
			while(!root.IsBuiltIn) { root = root.Base!; }
			i.Theme.Base = root;
		}
		foreach(_SavedTheme i in _OriginalThemes)
		{
			if(!ThemeManager.CustomThemes.Contains(i.Theme)) { ThemeManager.Add(i.Theme); }
		}
		foreach(_SavedTheme i in _OriginalThemes)
		{
			i.Theme.CopyFrom(i.Copy);
			i.Theme.Name = i.Name;
		}
		foreach(Theme i in ThemeManager.CustomThemes.Except(_OriginalThemes.Select(m => m.Theme)).ToArray()) { ThemeManager.Remove(i); }

		ThemeManager.Theme = _OriginalTheme;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the selected theme.</summary>
	private Theme _SelectedTheme
	{
		get { return ((_ListThemes.SelectedItem as Theme) ?? ThemeManager.Theme); }
	}


	/// <summary>Gets the selected color role, NULL if none is selected.</summary>
	private ThemeColor? _SelectedRole
	{
		get { return ((_ListColors.SelectedItems.Count > 0) ? (ThemeColor) _ListColors.SelectedItems[0].Tag! : null); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if OK saves the themes and the selected theme.</summary>
	[Category("Behavior")][Description("Determines if OK saves the themes and the selected theme with ThemeManager.Save().")][DefaultValue(false)]
	public bool SaveOnOk
	{
		get; set;
	} = false;


	/// <summary>Gets the selected theme, which is the theme of the application while the dialog is open.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Theme SelectedTheme
	{
		get { return _SelectedTheme; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Format event of the theme list and the base theme list: shows the built-in themes with the names from
	///          <see cref="ThemeSelectorTexts"/>.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Theme_Format(object? sender, ListControlConvertEventArgs e)
	{
		if(e.ListItem is Theme theme) { e.Value = ThemeSelectorTexts.GetName(theme); }
	}


	/// <summary>Handles the SelectedIndexChanged event of the theme list: makes the theme the theme of the application.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListThemes_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if(_Filling || (_ListThemes.SelectedItem is not Theme theme)) { return; }

		ThemeManager.Theme = theme;
		_ShowTheme();
	}


	/// <summary>Handles the TextChanged event of the name text box: renames the theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextName_TextChanged(object? sender, EventArgs e)
	{
		Theme theme = _SelectedTheme;
		if(_Filling || theme.IsBuiltIn || string.IsNullOrWhiteSpace(_TextName.Text)) { return; }

		theme.Name = _TextName.Text.Trim();

		_Filling = true;
		_ListThemes.Items[_ListThemes.SelectedIndex] = theme;
		_Filling = false;
	}


	/// <summary>Handles the SelectedIndexChanged event of the base theme combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboBase_SelectedIndexChanged(object? sender, EventArgs e)
	{
		Theme theme = _SelectedTheme;
		if(_Filling || theme.IsBuiltIn || (_ComboBase.SelectedItem is not Theme baseTheme)) { return; }

		theme.Base = baseTheme;
		_ShowColors();
	}


	/// <summary>Handles the SelectedIndexChanged event of the color list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListColors_SelectedIndexChanged(object? sender, EventArgs e)
	{
		_UpdateColorButtons();
	}


	/// <summary>Handles the DoubleClick event of the color list: changes the color.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListColors_DoubleClick(object? sender, EventArgs e)
	{
		_ChangeColor();
	}


	/// <summary>Handles the KeyDown event of the color list: Enter changes the color, Delete resets it.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ListColors_KeyDown(object? sender, KeyEventArgs e)
	{
		if(e.KeyCode == Keys.Delete)
		{
			_ResetColor();
			e.Handled = true;
		}
		else if((e.KeyCode == Keys.F2) || (e.KeyCode == Keys.Space))
		{
			_ChangeColor();
			e.Handled = true;
		}
	}


	/// <summary>Handles the Click event of the button "New": creates a theme based on the selected theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonNew_Click(object? sender, EventArgs e)
	{
		_AddTheme(new Theme(_GetUnusedName(ThemeEditorTexts.NewThemeName, null), _SelectedTheme));
	}


	/// <summary>Handles the Click event of the button "Copy": copies the selected theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCopy_Click(object? sender, EventArgs e)
	{
		_AddTheme(_SelectedTheme.Copy(_GetUnusedName(ThemeEditorTexts.CopyName, ThemeSelectorTexts.GetName(_SelectedTheme))));
	}


	/// <summary>Handles the Click event of the button "Remove": removes the selected theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRemove_Click(object? sender, EventArgs e)
	{
		Theme theme = _SelectedTheme;
		if(theme.IsBuiltIn) { return; }

		ThemeManager.Remove(theme);
		_FillThemes(ThemeManager.Theme);
	}


	/// <summary>Handles the Click event of the button "Import".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonImport_Click(object? sender, EventArgs e)
	{
		using OpenFileDialog dialog = new() { Filter = ThemeEditorTexts.FileFilter };
		if(dialog.ShowDialog(this) != DialogResult.OK) { return; }

		Theme theme;
		try
		{
			theme = Theme.Import(dialog.FileName);
		}
		catch(Exception ex)
		{
			ThemedMessageBox.Show(this, string.Format(ThemeEditorTexts.ImportFailed, ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		_AddTheme(theme);
	}


	/// <summary>Handles the Click event of the button "Export".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonExport_Click(object? sender, EventArgs e)
	{
		Theme theme = _SelectedTheme;
		if(theme.IsBuiltIn) { return; }

		using SaveFileDialog dialog = new() { Filter = ThemeEditorTexts.FileFilter, FileName = (theme.Name + ".theme") };
		if(dialog.ShowDialog(this) != DialogResult.OK) { return; }

		try
		{
			theme.Export(dialog.FileName);
		}
		catch(Exception ex) { ThemedMessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
	}


	/// <summary>Handles the Click event of the button "Change".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonChange_Click(object? sender, EventArgs e)
	{
		_ChangeColor();
	}


	/// <summary>Handles the Click event of the button "Reset color".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonResetColor_Click(object? sender, EventArgs e)
	{
		_ResetColor();
	}


	/// <summary>Handles the Click event of the button "Reset all": resets all colors of the theme.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonResetAll_Click(object? sender, EventArgs e)
	{
		if(_SelectedTheme.IsBuiltIn) { return; }

		_SelectedTheme.Reset();
		_ShowColors();
	}


	/// <summary>Handles the Click event of the OK button: keeps the changes and saves them if requested.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonOk_Click(object? sender, EventArgs e)
	{
		_Kept = true;
		if(SaveOnOk) { ThemeManager.Save(); }

		DialogResult = DialogResult.OK;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		if(!_Kept) { _Restore(); }
		_Kept = true;

		base.OnFormClosed(e);
	}
}
