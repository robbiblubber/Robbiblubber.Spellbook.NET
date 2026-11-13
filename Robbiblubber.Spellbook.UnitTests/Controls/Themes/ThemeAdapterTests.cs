namespace Robbiblubber.Spellbook.UnitTests.Controls.Themes;

using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class tests applying themes to controls: <see cref="ThemeManager.Apply(Control, Theme?)"/>, the theme adapters and
///          the <see cref="ThemeProvider"/>.</summary>
[TestFixture][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemeAdapterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] TestControl                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a control for custom adapters.</summary>
	public class TestControl: Control
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets the marker color.</summary>
		[DefaultValue(typeof(Color), "")]
		public Color MarkerColor
		{
			get; set;
		} = Color.Empty;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] TestAdapter                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a custom adapter.</summary>
	[ThemeAdapter(typeof(TestControl))]
	public class TestAdapter: ThemeAdapter<TestControl>
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] ThemeAdapter<TestControl>                                                                         //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		protected override void _Apply(TestControl control, Theme theme, ThemeAdapterState state)
		{
			_ApplyColors(control, state, ThemeColor.FIELD_BACK, null);
			state.ApplyColor(control, nameof(TestControl.MarkerColor), ThemeColor.ACCENT);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] TestTextAdapter                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a custom adapter for text boxes.</summary>
	public class TestTextAdapter: ThemeAdapter<TextBox>
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] ThemeAdapter<TextBox>                                                                             //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		protected override void _Apply(TextBox control, Theme theme, ThemeAdapterState state)
		{
			_ApplyColors(control, state, ThemeColor.ACCENT, null);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test form.</summary>
	private Form _Form = null!;

	/// <summary>Text box on the form.</summary>
	private TextBox _Text = null!;

	/// <summary>Label on the form.</summary>
	private Label _Label = null!;

	/// <summary>Panel on the form.</summary>
	private Panel _Panel = null!;

	/// <summary>Text box in the panel.</summary>
	private TextBox _PanelText = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a property is set explicitly.</summary>
	/// <param name="target">Object.</param>
	/// <param name="property">Property name.</param>
	/// <returns>Returns <c>true</c> if the property would be serialized.</returns>
	private static bool _IsSet(object target, string property)
	{
		return TypeDescriptor.GetProperties(target)[property]!.ShouldSerializeValue(target);
	}


	/// <summary>Asserts that two colors have the same value.</summary>
	/// <param name="actual">Actual color.</param>
	/// <param name="expected">Expected color.</param>
	private static void _AssertColor(Color actual, Color expected)
	{
		Assert.That(actual.ToArgb(), Is.EqualTo(expected.ToArgb()));
	}


	/// <summary>Creates the handles of controls, which a form does not create for its child controls while it is not shown.</summary>
	/// <param name="controls">Controls; the handles of their parents are created with them.</param>
	private static void _CreateHandles(params Control[] controls)
	{
		foreach(Control i in controls) { _ = i.Handle; }
	}


	/// <summary>Draws the selection of a text box into a bitmap.</summary>
	/// <param name="textBox">Text box.</param>
	/// <param name="bitmap">Bitmap.</param>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns <c>true</c> if a selection has been drawn, otherwise returns <c>false</c>.</returns>
	private static bool _Draw(TextBox textBox, Bitmap bitmap, Theme theme)
	{
		using Graphics g = Graphics.FromImage(bitmap);
		IntPtr hdc = g.GetHdc();
		try
		{
			return _EditSelection._Draw(textBox.Handle, hdc, theme);
		}
		finally
		{
			g.ReleaseHdc(hdc);
		}
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

		_Form = new Form();
		_Text = new TextBox();
		_Label = new Label();
		_Panel = new Panel();
		_PanelText = new TextBox();

		_Panel.Controls.Add(_PanelText);
		_Form.Controls.AddRange(new Control[] { _Text, _Label, _Panel });
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		ThemeManager.Reset(_Form);
		_PanelText.Dispose();
		_Panel.Dispose();
		_Label.Dispose();
		_Text.Dispose();
		_Form.Dispose();

		ThemeManager._ResetForTest();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The adapter of the nearest registered base type is used.</summary>
	[Test]
	public void FindsAdapters()
	{
		Assert.That(ThemeManager.GetAdapter(typeof(TextBox)), Is.Not.Null);
		Assert.That(ThemeManager.GetAdapter(typeof(RichTextBox)), Is.SameAs(ThemeManager.GetAdapter(typeof(TextBox))));
		Assert.That(ThemeManager.GetAdapter(typeof(Label)), Is.SameAs(ThemeManager.GetAdapter(typeof(Control))));
		Assert.That(ThemeManager.GetAdapter(typeof(MenuStrip)), Is.SameAs(ThemeManager.GetAdapter(typeof(ToolStrip))));
		Assert.That(ThemeManager.GetAdapter(typeof(RadioButton)), Is.SameAs(ThemeManager.GetAdapter(typeof(CheckBox))));
		Assert.That(ThemeManager.GetAdapter(typeof(CheckBox)), Is.Not.SameAs(ThemeManager.GetAdapter(typeof(Control))));
		Assert.That(ThemeManager.GetAdapter(typeof(ThemedCheckBox)), Is.SameAs(ThemeManager.GetAdapter(typeof(Control))));
		Assert.That(ThemeManager.GetAdapter(typeof(ThemedGroupBox)), Is.SameAs(ThemeManager.GetAdapter(typeof(Control))));
		Assert.That(ThemeManager.GetAdapter(typeof(Form)), Is.Not.SameAs(ThemeManager.GetAdapter(typeof(Panel))));
		Assert.Throws<ArgumentException>(() => ThemeManager.RegisterAdapter(typeof(string), null));
	}


	/// <summary>Custom adapters are discovered by their attribute and replace built-in adapters; an adapter for another control type
	///          is refused.</summary>
	[Test]
	public void RegistersCustomAdapters()
	{
		ThemeManager.Discover(new[] { typeof(TestAdapter), typeof(string) });
		Assert.That(ThemeManager.GetAdapter(typeof(TestControl)), Is.InstanceOf<TestAdapter>());

		TestTextAdapter adapter = new();
		IThemeAdapter? builtIn = ThemeManager.GetAdapter(typeof(RichTextBox));
		ThemeManager.RegisterAdapter(typeof(TextBox), adapter);
		Assert.That(ThemeManager.GetAdapter(typeof(TextBox)), Is.SameAs(adapter));
		Assert.That(ThemeManager.GetAdapter(typeof(RichTextBox)), Is.SameAs(builtIn));
		Assert.Throws<ArgumentException>(() => ThemeManager.RegisterAdapter(typeof(Label), new TestAdapter()));
		Assert.That(ThemeManager.GetAdapter(typeof(Label)), Is.SameAs(ThemeManager.GetAdapter(typeof(Control))));

		TestControl control = new();
		_Form.Controls.Add(control);
		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(control.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(control.MarkerColor, Theme.DARK[ThemeColor.ACCENT]);
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.ACCENT]);

		ThemeManager.Reset(_Form);
		Assert.That(control.MarkerColor, Is.EqualTo(Color.Empty));
	}


	/// <summary>Applying a theme colors the form and its controls; resetting restores the original state.</summary>
	[Test]
	public void AppliesAndRestoresColors()
	{
		ThemeManager.Apply(_Form, Theme.DARK);

		_AssertColor(_Form.BackColor, Theme.DARK[ThemeColor.BACK]);
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_Text.ForeColor, Theme.DARK[ThemeColor.FIELD_TEXT]);
		_AssertColor(_PanelText.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_Label.ForeColor, Theme.DARK[ThemeColor.TEXT]);
		Assert.That(_IsSet(_Label, nameof(Label.ForeColor)), Is.False);

		ThemeManager.Reset(_Form);

		Assert.That(_Form.BackColor, Is.EqualTo(SystemColors.Control));
		Assert.That(_Text.BackColor, Is.EqualTo(SystemColors.Window));
		Assert.That(_IsSet(_Form, nameof(Form.BackColor)), Is.False);
		Assert.That(_IsSet(_Text, nameof(TextBox.BackColor)), Is.False);
		Assert.That(ThemeManager._GetStateForTest(_Text), Is.Null);
	}


	/// <summary>Explicit colors are kept, explicit system colors are mapped to the theme.</summary>
	[Test]
	public void KeepsExplicitColors()
	{
		_Text.BackColor = Color.Red;
		_Label.ForeColor = SystemColors.ControlText;

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_Text.BackColor, Is.EqualTo(Color.Red));
		_AssertColor(_Label.ForeColor, Theme.DARK[ThemeColor.TEXT]);

		ThemeManager.Reset(_Form);
		Assert.That(_Text.BackColor, Is.EqualTo(Color.Red));
		Assert.That(_Label.ForeColor, Is.EqualTo(SystemColors.ControlText));
		Assert.That(_IsSet(_Label, nameof(Label.ForeColor)), Is.True);
	}


	/// <summary>Colors that the application changes after the theme has been applied are kept.</summary>
	[Test]
	public void KeepsApplicationChanges()
	{
		ThemeManager.Apply(_Form);
		_Text.BackColor = Color.Yellow;

		ThemeManager.Theme = Theme.DARK;
		Assert.That(_Text.BackColor, Is.EqualTo(Color.Yellow));
		_AssertColor(_PanelText.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
	}


	/// <summary>Without a theme of its own, the form follows the theme of the application; the system theme restores the controls.</summary>
	[Test]
	public void FollowsApplicationTheme()
	{
		ThemeManager.Apply(_Form);
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);
		Assert.That(ThemeManager.GetTheme(_PanelText), Is.SameAs(Theme.LIGHT));

		ThemeManager.Theme = Theme.DARK;
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_Form.BackColor, Theme.DARK[ThemeColor.BACK]);

		ThemeManager.Theme = Theme.SYSTEM;
		Assert.That(_IsSet(_Text, nameof(TextBox.BackColor)), Is.False);
		Assert.That(_IsSet(_Form, nameof(Form.BackColor)), Is.False);
		Assert.That(ThemeManager._GetStateForTest(_Text)!.Theme, Is.Null);
	}


	/// <summary>Changes of a custom theme are applied immediately.</summary>
	[Test]
	public void FollowsThemeChanges()
	{
		Theme custom = new("Custom", Theme.DARK);
		ThemeManager.Apply(_Form, custom);

		custom.SetValue(ThemeColor.FIELD_BACK, Color.Navy);
		_AssertColor(_Text.BackColor, Color.Navy);
	}


	/// <summary>Controls that are added later are themed as well.</summary>
	[Test]
	public void ThemesAddedControls()
	{
		ThemeManager.Apply(_Form, Theme.DARK);

		TextBox text = new();
		Panel panel = new();
		panel.Controls.Add(text);
		_Panel.Controls.Add(panel);

		_AssertColor(text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
	}


	/// <summary>A control with a theme of its own keeps it inside a themed form.</summary>
	[Test]
	public void SupportsNestedThemes()
	{
		ThemeManager.Apply(_Panel, Theme.LIGHT);
		ThemeManager.Apply(_Form, Theme.DARK);

		Assert.That(ThemeManager.GetTheme(_Text), Is.SameAs(Theme.DARK));
		Assert.That(ThemeManager.GetTheme(_PanelText), Is.SameAs(Theme.LIGHT));
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(_PanelText.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);

		ThemeManager.Theme = Theme.SYSTEM;
		_AssertColor(_PanelText.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);
	}


	/// <summary>Self-drawn controls use the theme of their form unless their color table has a theme of its own.</summary>
	[Test]
	public void PassesThemeToColorTables()
	{
		ProgressMeter meter = new();
		ProgressMeter own = new();
		own.Colors.Theme = Theme.LIGHT;
		_Panel.Controls.Add(meter);
		_Panel.Controls.Add(own);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(meter.Colors.EffectiveTheme, Is.SameAs(Theme.DARK));
		Assert.That(own.Colors.EffectiveTheme, Is.SameAs(Theme.LIGHT));
		Assert.That(meter.Colors.Theme, Is.Null);

		ThemeManager.Reset(_Form);
		Assert.That(meter.Colors.EffectiveTheme, Is.SameAs(Theme.LIGHT));
	}


	/// <summary>Buttons are drawn flat with the theme colors and restored afterwards.</summary>
	[Test]
	public void ThemesButtons()
	{
		Button button = new();
		Button accept = new();
		_Form.Controls.Add(button);
		_Form.Controls.Add(accept);
		_Form.AcceptButton = accept;

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(button.FlatStyle, Is.EqualTo(FlatStyle.Flat));
		Assert.That(button.UseVisualStyleBackColor, Is.False);
		_AssertColor(button.BackColor, Theme.DARK[ThemeColor.BUTTON_BACK]);
		_AssertColor(button.FlatAppearance.BorderColor, Theme.DARK[ThemeColor.BUTTON_BORDER]);
		_AssertColor(accept.FlatAppearance.BorderColor, Theme.DARK[ThemeColor.ACCENT]);

		ThemeManager.Reset(_Form);
		Assert.That(button.FlatStyle, Is.EqualTo(FlatStyle.Standard));
		Assert.That(button.UseVisualStyleBackColor, Is.True);
		Assert.That(button.FlatAppearance.BorderColor, Is.EqualTo(Color.Empty));
	}


	/// <summary>Standard check boxes are painted over like themed check boxes; the system flat style is switched while themed.</summary>
	[Test]
	public void PaintsCheckBoxes()
	{
		CheckBox check = new() { Text = "Check", AutoSize = false, Size = new Size(120, 24), Checked = true, FlatStyle = FlatStyle.System };
		_Form.Controls.Add(check);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(check.FlatStyle, Is.EqualTo(FlatStyle.Standard));

		int size = (int) Math.Round(13f * check.DeviceDpi / 96f);
		Point probe = new(size / 2, ((check.Height - size) / 2) + (size / 4));
		using(Bitmap bitmap = new(check.Width, check.Height))
		{
			check.DrawToBitmap(bitmap, new Rectangle(0, 0, check.Width, check.Height));
			_AssertColor(bitmap.GetPixel(probe.X, probe.Y), Theme.DARK[ThemeColor.ACCENT]);
		}

		ThemeManager.Reset(_Form);
		Assert.That(check.FlatStyle, Is.EqualTo(FlatStyle.System));
	}


	/// <summary>Group boxes get the text color explicitly, check boxes inherit it.</summary>
	[Test]
	public void ThemesGroupBoxes()
	{
		GroupBox group = new();
		CheckBox check = new() { Enabled = false };
		group.Controls.Add(check);
		_Form.Controls.Add(group);

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(group.ForeColor, Theme.DARK[ThemeColor.TEXT]);
		Assert.That(_IsSet(group, nameof(GroupBox.ForeColor)), Is.True);
		_AssertColor(check.ForeColor, Theme.DARK[ThemeColor.TEXT]);
		Assert.That(_IsSet(check, nameof(CheckBox.ForeColor)), Is.False);

		ThemeManager.Reset(_Form);
		Assert.That(_IsSet(group, nameof(GroupBox.ForeColor)), Is.False);
	}


	/// <summary>Group boxes are double buffered while the theme is applied and get their original styles back.</summary>
	[Test]
	public void DoubleBuffersGroupBoxes()
	{
		GroupBox group = new();
		_Form.Controls.Add(group);

		ControlStyles original = (_TabSupport._GetStyles(group) ?? 0);
		Assert.That(original.HasFlag(ControlStyles.OptimizedDoubleBuffer), Is.False);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_TabSupport._GetStyles(group)?.HasFlag(ControlStyles.OptimizedDoubleBuffer), Is.True);
		ThemeManager.Apply(_Form, Theme.LIGHT);

		ThemeManager.Reset(_Form);
		Assert.That(_TabSupport._GetStyles(group), Is.EqualTo(original));
	}

	/// <summary>Standard text boxes get the border of a themed text box while the theme is applied.</summary>
	[Test]
	public void DrawsTextBoxBorders()
	{
		_ = _Form.Handle;
		_ = _Text.Handle;
		Assert.That(_TextBoxBorderWindow._GetBorderColor(_Text), Is.EqualTo(Color.Empty));

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(_TextBoxBorderWindow._GetBorderColor(_Text), Theme.DARK[ThemeColor.FIELD_BORDER]);

		_Text.Enabled = false;
		_AssertColor(_TextBoxBorderWindow._GetBorderColor(_Text), Theme.DARK[ThemeColor.DISABLED_BORDER]);
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.DISABLED_BACK]);

		ThemeManager.Reset(_Form);
		Assert.That(_TextBoxBorderWindow._GetBorderColor(_Text), Is.EqualTo(Color.Empty));
	}


	/// <summary>Standard list boxes are owner drawn while the theme is applied, so selected items get the theme colors.</summary>
	[Test]
	public void DrawsListBoxSelection()
	{
		ListBox list = new();
		list.Items.AddRange(new object[] { "One", "Two", "Three" });
		ListBox owner = new() { DrawMode = DrawMode.OwnerDrawVariable };
		CheckedListBox check = new();
		_Form.Controls.AddRange(new Control[] { list, owner, check });
		_ = _Form.Handle;
		list.SelectedIndex = 1;
		int height = list.ItemHeight;

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(list.DrawMode, Is.EqualTo(DrawMode.OwnerDrawFixed));
		Assert.That(list.ItemHeight, Is.EqualTo(height));
		Assert.That(list.SelectedIndex, Is.EqualTo(1));
		Assert.That(owner.DrawMode, Is.EqualTo(DrawMode.OwnerDrawVariable));
		Assert.That(check.DrawMode, Is.EqualTo(DrawMode.Normal));

		ThemeManager.Apply(_Form, Theme.LIGHT);
		Assert.That(list.DrawMode, Is.EqualTo(DrawMode.OwnerDrawFixed));

		ThemeManager.Reset(_Form);
		Assert.That(list.DrawMode, Is.EqualTo(DrawMode.Normal));
		Assert.That(list.SelectedIndex, Is.EqualTo(1));
		Assert.That(owner.DrawMode, Is.EqualTo(DrawMode.OwnerDrawVariable));
	}


	/// <summary>Standard combo boxes are owner drawn while the theme is applied, so selected items get the theme colors.</summary>
	[Test]
	public void DrawsComboBoxSelection()
	{
		ComboBox combo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
		combo.Items.AddRange(new object[] { "One", "Two" });
		ComboBox owner = new() { DrawMode = DrawMode.OwnerDrawVariable };
		_Form.Controls.AddRange(new Control[] { combo, owner });
		_ = _Form.Handle;
		combo.SelectedIndex = 1;
		int height = combo.ItemHeight;

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(combo.DrawMode, Is.EqualTo(DrawMode.OwnerDrawFixed));
		Assert.That(combo.ItemHeight, Is.EqualTo(height));
		Assert.That(combo.SelectedIndex, Is.EqualTo(1));
		Assert.That(owner.DrawMode, Is.EqualTo(DrawMode.OwnerDrawVariable));

		ThemeManager.Reset(_Form);
		Assert.That(combo.DrawMode, Is.EqualTo(DrawMode.Normal));
		Assert.That(combo.SelectedIndex, Is.EqualTo(1));
	}


	/// <summary>Checked list boxes, list views and tree views draw their selection in theme colors while the theme is applied.</summary>
	[Test]
	public void DrawsSelectionOfOtherLists()
	{
		CheckedListBox check = new();
		check.Items.AddRange(new object[] { "One", "Two" });
		ListView list = new() { View = View.Details };
		TreeView tree = new();
		_Form.Controls.AddRange(new Control[] { check, list, tree });
		_CreateHandles(check, list, tree);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_CheckedListBoxWindow._IsInstalled(check), Is.True);
		_AssertColor(_SelectionWindow._GetBackColor(list), Theme.DARK[ThemeColor.SELECTION_BACK]);
		_AssertColor(_SelectionWindow._GetBackColor(tree), Theme.DARK[ThemeColor.SELECTION_BACK]);

		ThemeManager.Apply(_Form, Theme.LIGHT);
		_AssertColor(_SelectionWindow._GetBackColor(tree), Theme.LIGHT[ThemeColor.SELECTION_BACK]);

		ThemeManager.Reset(_Form);
		Assert.That(_CheckedListBoxWindow._IsInstalled(check), Is.False);
		Assert.That(_SelectionWindow._GetBackColor(list), Is.EqualTo(Color.Empty));
		Assert.That(_SelectionWindow._GetBackColor(tree), Is.EqualTo(Color.Empty));
	}


	/// <summary>A selected list view row is drawn in the selection back color of the theme, not in the color of the visual style.</summary>
	[Test]
	public void DrawsListViewSelection()
	{
		Theme theme = new("Red selection", Theme.DARK);
		theme.SetValue(ThemeColor.SELECTION_BACK, Color.FromArgb(200, 0, 0));

		ListView list = new() { View = View.Details, FullRowSelect = true, HideSelection = false, Bounds = new Rectangle(0, 0, 300, 120) };
		list.Columns.Add("Name", 60);
		list.Columns.Add("Empty", 160);
		list.Items.Add(new ListViewItem(new[] { "A", string.Empty }));
		_Form.Controls.Add(list);
		_ = _Form.Handle;

		ThemeManager.Apply(_Form, theme);
		list.Items[0].Selected = true;

		Rectangle row = list.Items[0].Bounds;
		using Bitmap bitmap = new(list.Width, list.Height);
		list.DrawToBitmap(bitmap, new Rectangle(0, 0, list.Width, list.Height));

		Point client = new(140, row.Top + (row.Height / 2));
		Point inBitmap = (list.PointToScreen(client) - (Size) list.Parent!.PointToScreen(list.Location));
		_AssertColor(bitmap.GetPixel(inBitmap.X, inBitmap.Y), Color.FromArgb(200, 0, 0));
	}


	/// <summary>With <see cref="ThemeManager.ThemeTextSelection"/>, which is on by default, the system colors of the selection are the
	///          theme colors while a themed text box handles a message, and the edit controls of an editable combo box and of an up-down
	///          control are subclassed.</summary>
	[Test]
	public void ThemesTextSelection()
	{
		Assert.That(ThemeManager.ThemeTextSelection, Is.True);
		ThemeManager.ThemeTextSelection = false;

		Theme theme = new("Red selection", Theme.DARK);
		theme.SetValue(ThemeColor.SELECTION_BACK, Color.FromArgb(200, 0, 0));
		theme.SetValue(ThemeColor.INACTIVE_SELECTION_BACK, Color.FromArgb(200, 0, 0));
		ComboBox combo = new() { DropDownStyle = ComboBoxStyle.DropDown };
		NumericUpDown number = new();
		_Form.Controls.Add(combo);
		_Form.Controls.Add(number);
		_CreateHandles(combo, number, number.Controls.OfType<TextBoxBase>().Single());

		ThemeManager.Apply(_Form, theme);
		Assert.That(_EditSelectionWindow._IsInstalled(combo), Is.False);
		Assert.That(_EditSelectionWindow._IsInstalled(number), Is.False);

		ThemeManager.ThemeTextSelection = true;
		Assert.That(_SysColorHook._Install(), Is.True);
		Assert.That(_EditSelectionWindow._IsInstalled(combo), Is.True);
		Assert.That(_EditSelectionWindow._IsInstalled(number), Is.True);

		int color = -1;
		Message m = Message.Create(IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
		_SysColorHook._Run(ref m, IntPtr.Zero, theme, (ref Message _) => color = _SysColorHook._GetColor(13));
		Assert.That(color, Is.EqualTo(200));
		Assert.That(_SysColorHook._GetColor(13), Is.EqualTo(SystemColors.Highlight.R | (SystemColors.Highlight.G << 8) | (SystemColors.Highlight.B << 16)));

		ThemeManager.ThemeTextSelection = false;
		Assert.That(_EditSelectionWindow._IsInstalled(combo), Is.False);
		Assert.That(_EditSelectionWindow._IsInstalled(number), Is.False);
	}


	/// <summary>The edit control of an up-down control is subclassed when its handle is created after the theme has been applied.</summary>
	[Test]
	public void ThemesUpDownSelectionAfterHandleCreation()
	{
		NumericUpDown number = new();
		_Form.Controls.Add(number);
		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_EditSelectionWindow._IsInstalled(number), Is.False);

		_ = number.Handle;
		_ = number.Controls.OfType<TextBoxBase>().Single().Handle;
		Assert.That(_EditSelectionWindow._IsInstalled(number), Is.True);
	}


	/// <summary>The selection of an edit control is drawn again in the selection colors of the theme.</summary>
	[Test]
	public void DrawsEditSelection()
	{
		Theme theme = new("Red selection", Theme.DARK);
		theme.SetValue(ThemeColor.SELECTION_BACK, Color.FromArgb(200, 0, 0));
		theme.SetValue(ThemeColor.INACTIVE_SELECTION_BACK, Color.FromArgb(200, 0, 0));
		TextBox textBox = new() { Text = "a      b", HideSelection = false, Bounds = new Rectangle(0, 0, 200, 30) };
		_Form.Controls.Add(textBox);
		_ = _Form.Handle;

		using Bitmap bitmap = new(textBox.ClientSize.Width, textBox.ClientSize.Height);
		Assert.That(_Draw(textBox, bitmap, theme), Is.False);

		textBox.Select(1, 6);
		Assert.That(_Draw(textBox, bitmap, theme), Is.True);

		Point position = textBox.GetPositionFromCharIndex(3);
		_AssertColor(bitmap.GetPixel(position.X + 1, position.Y + (textBox.Font.Height / 2)), Color.FromArgb(200, 0, 0));
	}


	/// <summary>Up-down controls get the border of a themed text box and the disabled colors while the theme is applied.</summary>
	[Test]
	public void DrawsUpDownBorders()
	{
		NumericUpDown number = new();
		_Form.Controls.Add(number);
		_CreateHandles(number);
		Color back = number.BackColor;

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(_UpDownBorderWindow._GetBorderColor(number), Theme.DARK[ThemeColor.FIELD_BORDER]);
		_AssertColor(number.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);

		number.Enabled = false;
		_AssertColor(_UpDownBorderWindow._GetBorderColor(number), Theme.DARK[ThemeColor.DISABLED_BORDER]);
		_AssertColor(number.BackColor, Theme.DARK[ThemeColor.DISABLED_BACK]);
		number.Enabled = true;
		_AssertColor(number.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);

		ThemeManager.Reset(_Form);
		Assert.That(_UpDownBorderWindow._GetBorderColor(number), Is.EqualTo(Color.Empty));
		Assert.That(number.BackColor, Is.EqualTo(back));
	}


	/// <summary>List views draw their group headers in theme colors while the theme is applied.</summary>
	[Test]
	public void DrawsGroupHeaders()
	{
		ListView list = new() { View = View.Details };
		list.Groups.Add(new ListViewGroup("General"));
		_Form.Controls.Add(list);
		_ = _Form.Handle;
		_ = list.Handle;
		Assert.That(_GroupHeaderWindow._GetColor(list), Is.EqualTo(Color.Empty));

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(_GroupHeaderWindow._GetColor(list), Theme.DARK[ThemeColor.ACCENT]);

		ThemeManager.Apply(_Form, Theme.LIGHT);
		_AssertColor(_GroupHeaderWindow._GetColor(list), Theme.LIGHT[ThemeColor.ACCENT]);

		ThemeManager.Reset(_Form);
		Assert.That(_GroupHeaderWindow._GetColor(list), Is.EqualTo(Color.Empty));
	}


	/// <summary>Only groups with nothing but a header are drawn in theme colors.</summary>
	[Test]
	public void DrawsSimpleGroupsOnly()
	{
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General")), Is.True);
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General") { Subtitle = "Sub" }), Is.False);
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General") { Footer = "Foot" }), Is.False);
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General") { TaskLink = "Task" }), Is.False);
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General") { CollapsedState = ListViewGroupCollapsedState.Expanded }), Is.False);
		Assert.That(_GroupHeaderWindow._IsSimple(new ListViewGroup("General") { TitleImageIndex = 0 }), Is.False);
	}


	/// <summary>Standard tab controls draw themselves while the theme is applied and get their original styles back.</summary>
	[Test]
	public void PaintsTabControls()
	{
		TabControl tabs = new() { Size = new Size(200, 120) };
		tabs.TabPages.Add(new TabPage("One"));
		tabs.TabPages.Add(new TabPage("Two"));
		_Form.Controls.Add(tabs);
		_ = _Form.Handle;
		_ = tabs.Handle;

		ControlStyles original = (_TabSupport._GetStyles(tabs) ?? 0);
		Assert.That(original.HasFlag(ControlStyles.UserPaint), Is.False);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_TabSupport._GetStyles(tabs)?.HasFlag(ControlStyles.UserPaint), Is.True);
		_AssertColor(tabs.TabPages[1].BackColor, Theme.DARK[ThemeColor.BACK]);

		Rectangle tab = tabs.GetTabRect(0);
		using(Bitmap bitmap = new(tabs.Width, tabs.Height))
		{
			tabs.DrawToBitmap(bitmap, new Rectangle(0, 0, tabs.Width, tabs.Height));
			_AssertColor(bitmap.GetPixel(tab.Left + (tab.Width / 2), tab.Top), Theme.DARK[ThemeColor.ACCENT]);
			_AssertColor(bitmap.GetPixel(tabs.Width / 2, tabs.Height - 1), Theme.DARK[ThemeColor.SEPARATOR]);
		}

		ThemeManager.Reset(_Form);
		Assert.That(_TabSupport._GetStyles(tabs), Is.EqualTo(original));
	}


	/// <summary>Owner drawn tab controls keep their look.</summary>
	[Test]
	public void KeepsOwnerDrawnTabControls()
	{
		TabControl tabs = new() { DrawMode = TabDrawMode.OwnerDrawFixed };
		tabs.TabPages.Add(new TabPage("One"));
		_Form.Controls.Add(tabs);

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(_TabSupport._GetStyles(tabs)?.HasFlag(ControlStyles.UserPaint), Is.False);
	}


	/// <summary>Tool strips and context menus get the theme renderer.</summary>
	[Test]
	public void ThemesToolStrips()
	{
		MenuStrip menu = new();
		ContextMenuStrip context = new();
		_Form.Controls.Add(menu);
		_Text.ContextMenuStrip = context;

		ThemeManager.Apply(_Form, Theme.DARK);
		Assert.That(menu.Renderer, Is.InstanceOf<ThemeToolStripRenderer>());
		Assert.That(((ThemeToolStripRenderer) context.Renderer).Theme, Is.SameAs(Theme.DARK));

		ThemeManager.Theme = Theme.DARK;
		ThemeManager.Apply(_Form, Theme.LIGHT);
		Assert.That(((ThemeToolStripRenderer) menu.Renderer).Theme, Is.SameAs(Theme.LIGHT));

		ThemeManager.Reset(_Form);
		Assert.That(menu.Renderer, Is.Not.InstanceOf<ThemeToolStripRenderer>());
		Assert.That(menu.RenderMode, Is.EqualTo(ToolStripRenderMode.ManagerRenderMode));
		context.Dispose();
	}


	/// <summary>Data grid views get field, selection and header colors.</summary>
	[Test]
	public void ThemesDataGridViews()
	{
		DataGridView grid = new();
		_Form.Controls.Add(grid);

		ThemeManager.Apply(_Form, Theme.DARK);
		_AssertColor(grid.DefaultCellStyle.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		_AssertColor(grid.DefaultCellStyle.SelectionBackColor, Theme.DARK[ThemeColor.SELECTION_BACK]);
		_AssertColor(grid.ColumnHeadersDefaultCellStyle.BackColor, Theme.DARK[ThemeColor.HEADER_BACK]);
		Assert.That(grid.EnableHeadersVisualStyles, Is.False);

		ThemeManager.Reset(_Form);
		Assert.That(grid.DefaultCellStyle.BackColor, Is.EqualTo(SystemColors.Window));
		Assert.That(grid.ColumnHeadersDefaultCellStyle.BackColor, Is.EqualTo(SystemColors.Control));
		Assert.That(grid.EnableHeadersVisualStyles, Is.True);
	}


	/// <summary>Read-only text boxes get the read-only colors, also when the property changes later.</summary>
	[Test]
	public void FollowsReadOnly()
	{
		ThemeManager.Apply(_Form, Theme.DARK);

		_Text.ReadOnly = true;
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.READ_ONLY_BACK]);

		_Text.ReadOnly = false;
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
	}


	/// <summary>The theme provider applies its theme to its form.</summary>
	[Test]
	public void ProviderAppliesTheme()
	{
		using ThemeProvider provider = new() { Theme = Theme.DARK };
		provider.ContainerControl = _Form;
		provider.Apply();
		_AssertColor(_Text.BackColor, Theme.DARK[ThemeColor.FIELD_BACK]);
		Assert.That(provider.EffectiveTheme, Is.SameAs(Theme.DARK));

		provider.Theme = null;
		_AssertColor(_Text.BackColor, Theme.LIGHT[ThemeColor.FIELD_BACK]);
		Assert.That(provider.EffectiveTheme, Is.SameAs(Theme.LIGHT));
	}
}
