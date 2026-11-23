namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;



/// <summary>This control lets the user select a theme.</summary>
/// <remarks><para>The selector is a drop-down list of the built-in and the custom themes, each with a small color sample, and an optional
///          link that opens the <see cref="ThemeEditorDialog"/>. It follows <see cref="ThemeManager.ThemesChanged"/>, so new, renamed
///          and removed themes appear at once. With <see cref="AutoApply"/> (the default), the selected theme is the theme of the
///          application: selecting a theme sets <see cref="ThemeManager.Theme"/>, and the selector follows when the theme of the
///          application changes elsewhere. Without <see cref="AutoApply"/>, the application reads <see cref="SelectedTheme"/>, for
///          example when a settings dialog is closed with OK.</para>
///          <para>The names of the built-in themes and the link text come from <see cref="ThemeSelectorTexts"/>.</para></remarks>
[SupportedOSPlatform("windows")]
[DefaultEvent(nameof(SelectedThemeChanged))]
[DefaultProperty(nameof(SelectedTheme))]
[ToolboxBitmap(typeof(ThemeProvider), "Resources.themeprovider.bmp")]
public class ThemeSelector: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Size of the color samples in logical pixels.</summary>
	private const int _SAMPLE_SIZE = 16;

	/// <summary>Gap between the list and the link in logical pixels.</summary>
	private const int _LINK_GAP = 8;

	/// <summary>Default width in logical pixels.</summary>
	private const int _DEFAULT_WIDTH = 260;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Drop-down list.</summary>
	private readonly ThemedComboBox _Combo;

	/// <summary>Link that opens the theme editor.</summary>
	private readonly LinkLabel _Link;

	/// <summary>Color samples owned by the control.</summary>
	private readonly List<Image> _Samples = new();

	/// <summary>Selected theme, NULL for the theme of the application.</summary>
	private Theme? _SelectedTheme = null;

	/// <summary>Theme that has been reported as selected theme last.</summary>
	private Theme? _Shown = null;

	/// <summary>Link text, NULL for the text from <see cref="ThemeSelectorTexts.Customize"/>.</summary>
	private string? _EditorLinkText = null;

	/// <summary>Determines if the list is being filled.</summary>
	private bool _Filling = false;

	/// <summary>Determines if the selector sets the theme of the application.</summary>
	private bool _Applying = false;

	/// <summary>Determines if the control listens to the theme manager.</summary>
	private bool _Subscribed = false;

	/// <summary>Determines if the link that opens the theme editor is shown.</summary>
	private bool _ShowEditorLink = true;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemeSelector(): base()
	{
		SetStyle(ControlStyles.ContainerControl | ControlStyles.SupportsTransparentBackColor, true);
		SetStyle(ControlStyles.Selectable, false);

		_Combo = new ThemedComboBox() { TabIndex = 0 };
		_Combo.SelectionChangeCommitted += _Combo_SelectionChangeCommitted;
		_Combo.SizeChanged += _Combo_SizeChanged;

		_Link = new LinkLabel() { AutoSize = true, TabIndex = 1, TabStop = true, Text = ThemeSelectorTexts.Customize };
		_Link.LinkClicked += _Link_LinkClicked;

		Controls.Add(_Combo);
		Controls.Add(_Link);
		Size = new Size(_DEFAULT_WIDTH, _Combo.Height);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the color sample of a theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns the image.</returns>
	/// <remarks>The sample shows the window background with a field, framed in the field border color, and a bar in the accent
	///          color.</remarks>
	private Image _CreateSample(Theme theme)
	{
		int size = LogicalToDeviceUnits(_SAMPLE_SIZE);
		Bitmap rval = new(size, size);

		using Graphics g = Graphics.FromImage(rval);
		g.SmoothingMode = SmoothingMode.None;

		Rectangle bounds = new(0, 0, size - 1, size - 1);
		using(SolidBrush brush = new(theme[ThemeColor.BACK])) { g.FillRectangle(brush, bounds); }

		int inset = Math.Max(2, size / 5);
		Rectangle field = new(inset, inset, size - (2 * inset), (size / 2) - inset);
		using(SolidBrush brush = new(theme[ThemeColor.FIELD_BACK])) { g.FillRectangle(brush, field); }

		Rectangle accent = new(inset, (size / 2) + 1, size - (2 * inset), Math.Max(2, (size / 2) - inset - 1));
		using(SolidBrush brush = new(theme[ThemeColor.ACCENT])) { g.FillRectangle(brush, accent); }

		using(Pen pen = new(theme[ThemeColor.FIELD_BORDER])) { g.DrawRectangle(pen, bounds); }

		return rval;
	}


	/// <summary>Returns the theme that the selector shows.</summary>
	/// <returns>Returns the selected theme, or the theme of the application if no theme has been selected.</returns>
	private Theme _GetShownTheme()
	{
		if((_SelectedTheme is not null) && ThemeManager.Themes.Contains(_SelectedTheme)) { return _SelectedTheme; }

		return ThemeManager.Theme;
	}


	/// <summary>Fills the list with the themes and selects the shown theme.</summary>
	private void _Fill()
	{
		_Filling = true;
		try
		{
			Theme shown = _GetShownTheme();
			_Combo.Items.Clear();
			foreach(Image i in _Samples) { i.Dispose(); }
			_Samples.Clear();

			foreach(Theme i in ThemeManager.Themes)
			{
				Image sample = _CreateSample(i);
				_Samples.Add(sample);
				_Combo.Items.Add(new ListItem(ThemeSelectorTexts.GetName(i), sample) { Tag = i });
			}
			_Select(shown);
		}
		finally
		{
			_Filling = false;
		}
	}


	/// <summary>Selects the item of a theme in the list.</summary>
	/// <param name="theme">Theme.</param>
	private void _Select(Theme theme)
	{
		for(int i = 0; i < _Combo.Items.Count; i++)
		{
			if((_Combo.Items[i] as ListItem)?.Tag == theme)
			{
				if(_Combo.SelectedIndex != i) { _Combo.SelectedIndex = i; }
				return;
			}
		}
		_Combo.SelectedIndex = -1;
	}


	/// <summary>Sets the selected theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <param name="apply">Determines if the theme is applied with <see cref="AutoApply"/>.</param>
	private void _SetSelectedTheme(Theme theme, bool apply)
	{
		_SelectedTheme = theme;
		if(apply && AutoApply && !DesignMode)
		{
			_Applying = true;
			try
			{
				ThemeManager.Theme = theme;
			}
			finally
			{
				_Applying = false;
			}
		}
		if(!_Filling) { _Select(theme); }

		_ReportChange();
	}


	/// <summary>Raises <see cref="SelectedThemeChanged"/> if the shown theme is not the theme that has been reported last.</summary>
	private void _ReportChange()
	{
		Theme shown = _GetShownTheme();
		if(shown == _Shown) { return; }

		bool first = (_Shown is null);
		_Shown = shown;
		if(!first) { OnSelectedThemeChanged(EventArgs.Empty); }
	}


	/// <summary>Starts to listen to the theme manager.</summary>
	private void _Subscribe()
	{
		if(_Subscribed) { return; }

		ThemeManager.ThemesChanged += _ThemeManager_ThemesChanged;
		ThemeManager.ThemeChanged += _ThemeManager_ThemeChanged;
		_Subscribed = true;
	}


	/// <summary>Stops listening to the theme manager.</summary>
	private void _Unsubscribe()
	{
		if(!_Subscribed) { return; }

		ThemeManager.ThemesChanged -= _ThemeManager_ThemesChanged;
		ThemeManager.ThemeChanged -= _ThemeManager_ThemeChanged;
		_Subscribed = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="SelectedThemeChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSelectedThemeChanged(EventArgs e)
	{
		SelectedThemeChanged?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the selected theme has changed.</summary>
	[Category("Behavior")][Description("Occurs when the selected theme has changed.")]
	public event EventHandler? SelectedThemeChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if the selected theme is the theme of the application.</summary>
	/// <remarks>If this property is set, selecting a theme sets <see cref="ThemeManager.Theme"/>, and the selector follows when the
	///          theme of the application changes. The default is <c>true</c>.</remarks>
	[Category("Behavior")][Description("Determines if the selected theme is applied as theme of the application.")][DefaultValue(true)]
	public bool AutoApply
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the selected theme.</summary>
	/// <remarks>As long as no theme has been selected, this is the theme of the application. A theme that is removed from the theme
	///          manager is replaced by the theme of the application. With <see cref="AutoApply"/>, setting this property sets
	///          <see cref="ThemeManager.Theme"/>.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Theme SelectedTheme
	{
		get { return _GetShownTheme(); }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			_SetSelectedTheme(value, true);
		}
	}


	/// <summary>Gets or sets if the link that opens the theme editor is shown.</summary>
	[Category("Appearance")][Description("Determines if the link that opens the theme editor is shown.")][DefaultValue(true)]
	public bool ShowEditorLink
	{
		get { return _ShowEditorLink; }
		set
		{
			if(value == _ShowEditorLink) { return; }

			_ShowEditorLink = _Link.Visible = value;
			PerformLayout();
		}
	}


	/// <summary>Gets or sets the text of the link that opens the theme editor, NULL or empty for the text from
	///          <see cref="ThemeSelectorTexts.Customize"/>.</summary>
	[Category("Appearance")][Description("The text of the link that opens the theme editor; empty for the default text.")][DefaultValue(null)]
	[Localizable(true)]
	public string? EditorLinkText
	{
		get { return _EditorLinkText; }
		set
		{
			_EditorLinkText = (string.IsNullOrEmpty(value) ? null : value);
			_Link.Text = (_EditorLinkText ?? ThemeSelectorTexts.Customize);
			PerformLayout();
		}
	}


	/// <summary>Gets or sets if the theme editor saves the themes when it is closed with OK.</summary>
	/// <remarks>See <see cref="ThemeEditorDialog.SaveOnOk"/>. The default is <c>false</c>.</remarks>
	[Category("Behavior")][Description("Determines if the theme editor saves the themes with ThemeManager.Save() when it is closed with OK.")][DefaultValue(false)]
	public bool SaveOnEditorOk
	{
		get; set;
	} = false;


	/// <summary>Gets the drop-down list.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ThemedComboBox ComboBox
	{
		get { return _Combo; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads the names of the built-in themes and the link text again from <see cref="ThemeSelectorTexts"/>, for example
	///          after the language of the user interface has changed.</summary>
	public void RefreshTexts()
	{
		_Link.Text = (_EditorLinkText ?? ThemeSelectorTexts.Customize);
		if(IsHandleCreated) { _Fill(); }
		PerformLayout();
	}


	/// <summary>Opens the theme editor and selects the theme chosen in it.</summary>
	/// <returns>Returns <see cref="DialogResult.OK"/> if the editor has been closed with OK, otherwise <see cref="DialogResult.Cancel"/>.</returns>
	/// <remarks>The editor makes the theme selected in it the theme of the application while it is open, and restores the previous
	///          theme on Cancel. On OK, the selector selects the theme chosen in the editor; without <see cref="AutoApply"/>, the theme
	///          of the application is set back to the theme it had before.</remarks>
	public DialogResult ShowEditor()
	{
		Theme previous = ThemeManager.Theme;

		using ThemeEditorDialog dialog = new() { SaveOnOk = SaveOnEditorOk };
		if(!AutoApply) { ThemeManager.Theme = SelectedTheme; }

		DialogResult rval = dialog.ShowDialog(FindForm());
		if(rval == DialogResult.OK) { _SetSelectedTheme(dialog.SelectedTheme, false); } else { rval = DialogResult.Cancel; }
		if(!AutoApply) { ThemeManager.Theme = (ThemeManager.Themes.Contains(previous) ? previous : Theme.AUTO); }

		_Fill();
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Size DefaultSize
	{
		get { return new Size(_DEFAULT_WIDTH, 23); }
	}


	/// <inheritdoc/>
	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, (((_Combo is null) || (_Link is null)) ? height : Math.Max(_Combo.Height, _Link.PreferredHeight)), specified);
	}


	/// <inheritdoc/>
	protected override void OnLayout(LayoutEventArgs levent)
	{
		base.OnLayout(levent);

		int linkWidth = (_ShowEditorLink ? (_Link.PreferredWidth + LogicalToDeviceUnits(_LINK_GAP)) : 0);
		_Combo.SetBounds(0, 0, Math.Max(LogicalToDeviceUnits(40), ClientSize.Width - linkWidth), _Combo.Height);
		if(_ShowEditorLink)
		{
			Size size = _Link.PreferredSize;
			_Link.SetBounds(ClientSize.Width - size.Width, Math.Max(0, (ClientSize.Height - size.Height) / 2), size.Width, size.Height);
		}
	}


	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		_Subscribe();
		RefreshTexts();
		_ReportChange();
	}


	/// <inheritdoc/>
	protected override void OnHandleDestroyed(EventArgs e)
	{
		if(!RecreatingHandle) { _Unsubscribe(); }
		base.OnHandleDestroyed(e);
	}


	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		Height = Math.Max(_Combo.Height, _Link.PreferredHeight);
		PerformLayout();
	}


	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		base.OnDpiChangedAfterParent(e);
		if(IsHandleCreated) { _Fill(); }
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_Unsubscribe();
			foreach(Image i in _Samples) { i.Dispose(); }
			_Samples.Clear();
		}
		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the SelectionChangeCommitted event of the drop-down list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_SelectionChangeCommitted(object? sender, EventArgs e)
	{
		if(_Filling || ((_Combo.SelectedItem as ListItem)?.Tag is not Theme theme)) { return; }

		_SetSelectedTheme(theme, true);
	}


	/// <summary>Handles the SizeChanged event of the drop-down list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_SizeChanged(object? sender, EventArgs e)
	{
		if(Height != _Combo.Height) { Height = Math.Max(_Combo.Height, _Link.PreferredHeight); }
	}


	/// <summary>Handles the LinkClicked event of the link.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Link_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		if(!DesignMode) { ShowEditor(); }
	}


	/// <summary>Handles the ThemesChanged event of the theme manager: lists new, renamed and removed themes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemesChanged(object? sender, EventArgs e)
	{
		if((_SelectedTheme is not null) && !ThemeManager.Themes.Contains(_SelectedTheme)) { _SelectedTheme = null; }
		_Fill();
		_ReportChange();
	}


	/// <summary>Handles the ThemeChanged event of the theme manager: follows the theme of the application with
	///          <see cref="AutoApply"/>, and shows changed colors in the samples.</summary>
	/// <remarks>When the selector sets the theme itself, the list is not filled again, so the items stay while the drop-down list
	///          closes.</remarks>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ThemeManager_ThemeChanged(object? sender, EventArgs e)
	{
		if(AutoApply) { _SelectedTheme = null; }
		if(!_Applying) { _Fill(); }
		_ReportChange();
	}
}
