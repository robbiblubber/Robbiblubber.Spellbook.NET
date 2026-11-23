namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>The left group shows themed combo boxes with different item types and a standard combo box for comparison. The options on
///          the right apply to the themed combo boxes; all events are written to the log.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;

	/// <summary>Number of items of the large list.</summary>
	private const int _MANY_ITEMS = 10000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static members                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>European capitals as "city|country".</summary>
	internal static readonly string[] _Capitals = new string[]
	{
		"Amsterdam|Netherlands", "Athens|Greece", "Belgrade|Serbia", "Berlin|Germany", "Bern|Switzerland", "Bratislava|Slovakia",
		"Brussels|Belgium", "Bucharest|Romania", "Budapest|Hungary", "Copenhagen|Denmark", "Dublin|Ireland", "Helsinki|Finland",
		"Kyiv|Ukraine", "Lisbon|Portugal", "Ljubljana|Slovenia", "London|United Kingdom", "Luxembourg|Luxembourg", "Madrid|Spain",
		"Oslo|Norway", "Paris|France", "Prague|Czechia", "Reykjavik|Iceland", "Riga|Latvia", "Rome|Italy", "Sofia|Bulgaria",
		"Stockholm|Sweden", "Tallinn|Estonia", "Valletta|Malta", "Vienna|Austria", "Vilnius|Lithuania", "Warsaw|Poland", "Zagreb|Croatia"
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Themes of the color scheme combo box.</summary>
	private static readonly Theme[] _Themes = new Theme[] { Theme.SYSTEM, Theme.LIGHT, Theme.DARK };



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Larger font, NULL if it has not been created yet.</summary>
	private Font? _LargeFont = null;

	/// <summary>Number of added items.</summary>
	private int _Added = 0;

	/// <summary>Document images by file extension.</summary>
	private readonly Dictionary<string, Image> _FileImages = new(StringComparer.OrdinalIgnoreCase)
	{
		{ ".docx", SampleImages.CreateDocument(Color.FromArgb(0x2B, 0x57, 0x9A)) },
		{ ".xlsx", SampleImages.CreateDocument(Color.FromArgb(0x21, 0x73, 0x46)) },
		{ ".pdf", SampleImages.CreateDocument(Color.FromArgb(0xD3, 0x2F, 0x2F)) },
		{ ".png", SampleImages.CreateDocument(Color.FromArgb(0xF5, 0x9E, 0x0B)) }
	};

	/// <summary>Products bound to the data binding combo box.</summary>
	private readonly BindingList<SampleProduct> _Products = new()
	{
		new SampleProduct(1, "Apple", "Austria", "apple"),
		new SampleProduct(2, "Banana", "Ecuador", "banana"),
		new SampleProduct(3, "Cherry", "Italy", "cherry"),
		new SampleProduct(4, "Lemon", "Spain", "lemon"),
		new SampleProduct(5, "Fig", "Turkey", null)
	};

	/// <summary>Image for unknown file types.</summary>
	private readonly Image _UnknownFileImage = SampleImages.CreateDocument(Color.FromArgb(0xA0, 0xA0, 0xA0));



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		ImageList images = SampleImages.CreateFruitList(components ??= new Container());
		_ComboItems.ImageList = images;
		_ComboMany.ImageList = images;
		_ComboSearch.ImageList = images;
		_ComboData.ImageList = images;

		_FillItems();
		_FillMany();
		_FillObjects();
		_FillCombos();

		_ComboScheme.Items.AddRange(new ListItem("System") { Detail = "system colors" }, new ListItem("Light"), new ListItem("Dark"));
		_ComboMatchMode.Items.AddRange(Enum.GetNames<ListMatchMode>());
		_ComboFilterMode.Items.AddRange(Enum.GetNames<ListFilterMode>());
		_ComboMatchMode.SelectedIndex = (int) ListMatchMode.FUZZY;
		_ComboFilterMode.SelectedIndex = (int) ListFilterMode.FILTER;
		_ComboScheme.SelectedIndex = 0;

		_Log("Ready.");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the themed combo boxes of the left group, whose style can be switched.</summary>
	private ThemedComboBox[] _DemoCombos
	{
		get { return new ThemedComboBox[] { _ComboStrings, _ComboItems, _ComboMany, _ComboLazy, _ComboObjects }; }
	}


	/// <summary>Gets the themed combo boxes the options apply to.</summary>
	private ThemedComboBox[] _SampleCombos
	{
		get { return _DemoCombos.Concat(new ThemedComboBox[] { _ComboSearch, _ComboSource, _ComboFiles, _ComboData, _ComboAsync }).ToArray(); }
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


	/// <summary>Writes a log entry for an event of a themed combo box.</summary>
	/// <param name="sender">Sending control.</param>
	/// <param name="name">Event name.</param>
	private void _LogEvent(object? sender, string name)
	{
		if(sender is not ThemedComboBox combo) { return; }

		_Log($"{combo.Name,-16} {name,-26} SelectedIndex = {combo.SelectedIndex}, Text = \"{combo.Text}\"");
	}


	/// <summary>Creates the fruit items.</summary>
	/// <returns>Returns the items.</returns>
	private static ListItem[] _CreateFruits()
	{
		return new ListItem[]
		{
			new ListItem("Apple", "apple") { Detail = "€ 1.20", Description = "Malus domestica." + Environment.NewLine + "Keeps the doctor away." },
			new ListItem("Banana", "banana") { Detail = "€ 0.80", Description = "Musa." + Environment.NewLine + "Botanically a berry." },
			new ListItem("Cherry", "cherry") { Detail = "€ 4.50", FontStyle = FontStyle.Bold },
			new ListItem("Blueberry", "blueberry") { Detail = "€ 3.90" },
			new ListItem("Elderberry", "elderberry") { Detail = "seasonal", ForeColor = Color.MediumPurple, Description = "Sambucus nigra. Only edible when cooked." },
			new ListItem("Fig") { Detail = "no image" },
			new ListItem("Grape", SampleImages.Grape) { Detail = "Image property" },
			new ListItem("Lemon", "lemon") { Detail = "€ 0.50", FontStyle = FontStyle.Italic },
			new ListItem("Kiwi", "kiwi") { Detail = "unknown image key" }
		};
	}


	/// <summary>Fills the combo boxes of the combo mode group.</summary>
	private void _FillCombos()
	{
		_ComboSearch.Items.AddRange(_CreateFruits());

		_ComboSource.Items.AddRange(new ListItem("Vienna") { Detail = "Austria" }, new ListItem("Berlin") { Detail = "Germany" }, new ListItem("Rome") { Detail = "Italy" });
		_ComboSource.SearchSource = new AutoCompletionItemSource(_Capitals.Select(m => m.Split('|')).Select(m => new AutoCompletionItem(m[0]) { Detail = m[1], Description = $"{m[0]} is the capital of {m[1]}." }));

		_ComboData.DisplayMember = nameof(SampleProduct.Name);
		_ComboData.ValueMember = nameof(SampleProduct.Id);
		_ComboData.DetailMember = nameof(SampleProduct.Origin);
		_ComboData.ImageMember = nameof(SampleProduct.ImageKey);
		_ComboData.DataSource = _Products;

		_ComboAsync.SearchSource = new SlowCapitalSource();
		_ComboAsync.SearchDelay = 300;

		_ComboFiles.DefaultImage = _UnknownFileImage;
		_ComboFiles.Items.AddRange("report.docx", "budget.xlsx", "manual.pdf", "photo.png", "notes.txt");
	}


	/// <summary>Fills the combo box with list items.</summary>
	private void _FillItems()
	{
		_ComboItems.Items.AddRange(_CreateFruits());
		_ComboItems.SelectedIndex = 0;
	}


	/// <summary>Fills the combo box with many items.</summary>
	private void _FillMany()
	{
		Stopwatch watch = Stopwatch.StartNew();
		List<object> items = new(_MANY_ITEMS);

		for(int i = 1; i <= _MANY_ITEMS; i++)
		{
			items.Add(new ListItem($"Item {i:N0}", (((i % 10) == 0) ? "apple" : null)) { Detail = $"#{i:X4}" });
		}
		_ComboMany.Items.AddRange(items);

		_Log($"{_MANY_ITEMS:N0} items added in {watch.ElapsedMilliseconds} ms.");
	}


	/// <summary>Fills the combo box with objects that do not implement IListItem.</summary>
	private void _FillObjects()
	{
		_ComboObjects.Items.AddRange(new Country("Austria", "AT"), new Country("Germany", "DE"), new Country("Italy", "IT"), new Country("Switzerland", "CH"), new Country("Sweden", "SE"), new Country("Spain", "ES"));
	}


	/// <summary>Gets the larger font.</summary>
	/// <returns>Returns the font.</returns>
	private Font _GetLargeFont()
	{
		return (_LargeFont ??= new Font(Font.FontFamily, 12f));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the SelectedIndexChanged event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_SelectedIndexChanged(object? sender, EventArgs e)
	{
		_LogEvent(sender, "SelectedIndexChanged");
	}


	/// <summary>Handles the SelectedItemChanged event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_SelectedItemChanged(object? sender, EventArgs e)
	{
		_LogEvent(sender, "SelectedItemChanged");

		if(sender is ThemedComboBox combo)
		{
			_LabelSelected.Text = $"{combo.Name}: SelectedIndex = {combo.SelectedIndex}, SelectedItem = {combo.SelectedItem?.GetType().Name ?? "NULL"} \"{combo.Text}\"";
		}
	}


	/// <summary>Handles the SelectionChangeCommitted event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_SelectionChangeCommitted(object? sender, EventArgs e)
	{
		_LogEvent(sender, "SelectionChangeCommitted");
	}


	/// <summary>Handles the DropDown event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_DropDown(object? sender, EventArgs e)
	{
		_LogEvent(sender, "DropDown");
	}


	/// <summary>Handles the DropDownClosed event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_DropDownClosed(object? sender, EventArgs e)
	{
		_LogEvent(sender, "DropDownClosed");
	}


	/// <summary>Handles the TextChanged event of the themed combo boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Combo_TextChanged(object? sender, EventArgs e)
	{
		_LogEvent(sender, "TextChanged");
	}


	/// <summary>Handles the DropDown event of the lazy combo box: fills the items when the list opens for the first time.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboLazy_DropDown(object? sender, EventArgs e)
	{
		if(_ComboLazy.Items.Count > 0) { return; }

		Cursor = Cursors.WaitCursor;
		Thread.Sleep(300);
		for(int i = 1; i <= 20; i++) { _ComboLazy.Items.Add(new ListItem($"Loaded item {i}") { Detail = $"{DateTime.Now:HH:mm:ss}" }); }
		Cursor = Cursors.Default;

		_Log("_ComboLazy       items loaded.");
	}


	/// <summary>Handles the ResolveImage event of the file combo box: provides an image for the file extension.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboFiles_ResolveImage(object? sender, ThemedComboBoxImageEventArgs e)
	{
		string extension = Path.GetExtension(e.Text);

		if((extension.Length > 1) && _FileImages.TryGetValue(extension, out Image? image)) { e.Image = image; }
	}


	/// <summary>Handles the SelectedValueChanged event of the data binding combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboData_SelectedValueChanged(object? sender, EventArgs e)
	{
		_Log($"{_ComboData.Name,-16} {"SelectedValueChanged",-26} SelectedValue = {_ComboData.SelectedValue ?? "NULL"}");
	}


	/// <summary>Handles the Click event of the button that adds a product.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAddProduct_Click(object? sender, EventArgs e)
	{
		int id = _Products.Max(m => m.Id) + 1;

		_Products.Add(new SampleProduct(id, $"Product {id}", "Nowhere", "blueberry"));
		_Log($"_ComboData       product {id} added to the binding list.");
	}


	/// <summary>Handles the SelectedIndexChanged event of the standard combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboStandard_SelectedIndexChanged(object? sender, EventArgs e)
	{
		_Log($"{_ComboStandard.Name,-16} {"SelectedIndexChanged",-26} SelectedIndex = {_ComboStandard.SelectedIndex}, Text = \"{_ComboStandard.Text}\"");
	}


	/// <summary>Handles the SelectedIndexChanged event of the theme combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboScheme_SelectedIndexChanged(object? sender, EventArgs e)
	{
		Theme theme = _Themes[Math.Max(0, _ComboScheme.SelectedIndex)];

		foreach(ThemedComboBox i in _SampleCombos.Concat(new ThemedComboBox[] { _ComboScheme, _ComboMatchMode, _ComboFilterMode })) { i.Colors.Theme = theme; }

		(BackColor, ForeColor, _ListLog.BackColor, _ListLog.ForeColor) = (theme[ThemeColor.BACK], theme[ThemeColor.TEXT], theme[ThemeColor.FIELD_BACK], theme[ThemeColor.FIELD_TEXT]);

		_Log($"Theme: {theme.Name}");
	}


	/// <summary>Handles the changes of the options.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Options_Changed(object? sender, EventArgs e)
	{
		ThemedComboBoxFocusStyle focus = ThemedComboBoxFocusStyle.NONE;

		if(_ChkFocusBorder.Checked) { focus |= ThemedComboBoxFocusStyle.BORDER; }
		if(_ChkFocusDotted.Checked) { focus |= ThemedComboBoxFocusStyle.DOTTED; }
		if(_ChkFocusLine.Checked) { focus |= ThemedComboBoxFocusStyle.LINE; }

		foreach(ThemedComboBox i in _DemoCombos)
		{
			i.DropDownStyle = (_ChkCombo.Checked ? ThemedComboBoxStyle.COMBO : ThemedComboBoxStyle.LIST);
		}

		foreach(ThemedComboBox i in _SampleCombos)
		{
			i.AllowFreeText = _ChkFreeText.Checked;
			i.ShowDescriptions = _ChkDescriptions.Checked;
			i.MatchMode = (ListMatchMode) Math.Max(0, _ComboMatchMode.SelectedIndex);
			i.FilterMode = (ListFilterMode) Math.Max(0, _ComboFilterMode.SelectedIndex);
			i.MinimumSearchLength = (int) _NumMinLength.Value;
			i.ReadOnly = _ChkReadOnly.Checked;
			i.Enabled = _ChkEnabled.Checked;
			i.ShowDetailInDisplay = _ChkDetail.Checked;
			i.Colors.FocusColor = (_ChkFocusColor.Checked ? Color.Orange : Color.Empty);
			i.Colors.HotBackColor = (_ChkHover.Checked ? Color.FromArgb(0xE5, 0xF1, 0xFB) : Color.Empty);
			i.FocusStyle = focus;
			i.DropDownWidth = (int) _NumWidth.Value;
			i.MaxDropDownItems = (int) _NumMaxItems.Value;
			i.Font = (_ChkFont.Checked ? _GetLargeFont() : null);
		}
	}


	/// <summary>Handles the Click event of the button that selects an item outside the list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonOutside_Click(object? sender, EventArgs e)
	{
		_ComboItems.SelectedItem = new ListItem("Durian", "kiwi") { Detail = "not in the list" };
	}


	/// <summary>Handles the Click event of the add button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAdd_Click(object? sender, EventArgs e)
	{
		_ComboItems.Items.Add(new ListItem($"New item {++_Added}", "apple") { Detail = $"{DateTime.Now:HH:mm:ss}" });
		_Log($"_ComboItems      item added, {_ComboItems.Items.Count} items.");
	}


	/// <summary>Handles the Click event of the remove button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRemove_Click(object? sender, EventArgs e)
	{
		if(_ComboItems.SelectedIndex < 0)
		{
			_Log("_ComboItems      no item of the list is selected.");
			return;
		}

		_ComboItems.Items.RemoveAt(_ComboItems.SelectedIndex);
		_Log($"_ComboItems      item removed, {_ComboItems.Items.Count} items.");
	}


	/// <summary>Handles the Click event of the clear button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonClear_Click(object? sender, EventArgs e)
	{
		_ComboItems.Items.Clear();
		_Log("_ComboItems      items cleared.");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);

		_LargeFont?.Dispose();
		_LargeFont = null;

		foreach(Image i in _FileImages.Values) { i.Dispose(); }
		_FileImages.Clear();
		_UnknownFileImage.Dispose();
	}
}
