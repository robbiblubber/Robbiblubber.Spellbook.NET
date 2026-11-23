namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;

partial class MainWindow
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if(disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this._LabelStrings = new System.Windows.Forms.Label();
		this._ComboStrings = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelItems = new System.Windows.Forms.Label();
		this._ComboItems = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelMany = new System.Windows.Forms.Label();
		this._ComboMany = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelLazy = new System.Windows.Forms.Label();
		this._ComboLazy = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelObjects = new System.Windows.Forms.Label();
		this._ComboObjects = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelStandard = new System.Windows.Forms.Label();
		this._ComboStandard = new System.Windows.Forms.ComboBox();
		this._LabelSelected = new System.Windows.Forms.Label();
		this._GroupDemo = new System.Windows.Forms.GroupBox();
		this._LabelSearch = new System.Windows.Forms.Label();
		this._ComboSearch = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelSource = new System.Windows.Forms.Label();
		this._ComboSource = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelFiles = new System.Windows.Forms.Label();
		this._ComboFiles = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelData = new System.Windows.Forms.Label();
		this._ComboData = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelAsync = new System.Windows.Forms.Label();
		this._ComboAsync = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._ButtonAddProduct = new System.Windows.Forms.Button();
		this._GroupCombo = new System.Windows.Forms.GroupBox();
		this._LabelScheme = new System.Windows.Forms.Label();
		this._ComboScheme = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._ChkReadOnly = new System.Windows.Forms.CheckBox();
		this._ChkEnabled = new System.Windows.Forms.CheckBox();
		this._ChkDetail = new System.Windows.Forms.CheckBox();
		this._ChkFocusColor = new System.Windows.Forms.CheckBox();
		this._ChkFont = new System.Windows.Forms.CheckBox();
		this._ChkHover = new System.Windows.Forms.CheckBox();
		this._ChkFocusBorder = new System.Windows.Forms.CheckBox();
		this._ChkFocusDotted = new System.Windows.Forms.CheckBox();
		this._ChkFocusLine = new System.Windows.Forms.CheckBox();
		this._LabelWidth = new System.Windows.Forms.Label();
		this._NumWidth = new System.Windows.Forms.NumericUpDown();
		this._LabelMaxItems = new System.Windows.Forms.Label();
		this._NumMaxItems = new System.Windows.Forms.NumericUpDown();
		this._ButtonOutside = new System.Windows.Forms.Button();
		this._ButtonAdd = new System.Windows.Forms.Button();
		this._ButtonRemove = new System.Windows.Forms.Button();
		this._ButtonClear = new System.Windows.Forms.Button();
		this._ChkCombo = new System.Windows.Forms.CheckBox();
		this._ChkFreeText = new System.Windows.Forms.CheckBox();
		this._ChkDescriptions = new System.Windows.Forms.CheckBox();
		this._LabelMatchMode = new System.Windows.Forms.Label();
		this._ComboMatchMode = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelFilterMode = new System.Windows.Forms.Label();
		this._ComboFilterMode = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelMinLength = new System.Windows.Forms.Label();
		this._NumMinLength = new System.Windows.Forms.NumericUpDown();
		this._GroupOptions = new System.Windows.Forms.GroupBox();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._GroupDemo.SuspendLayout();
		this._GroupCombo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._NumWidth)).BeginInit();
		((System.ComponentModel.ISupportInitialize)(this._NumMaxItems)).BeginInit();
		((System.ComponentModel.ISupportInitialize)(this._NumMinLength)).BeginInit();
		this._GroupOptions.SuspendLayout();
		this.SuspendLayout();
		//
		// _LabelStrings
		//
		this._LabelStrings.AutoSize = true;
		this._LabelStrings.Location = new System.Drawing.Point(14, 32);
		this._LabelStrings.Name = "_LabelStrings";
		this._LabelStrings.TabIndex = 0;
		this._LabelStrings.Text = "&Strings:";
		//
		// _ComboStrings
		//
		this._ComboStrings.Items.AddRange(new object[] { "Apple", "Banana", "Cherry", "Blueberry", "Elderberry", "Fig", "Grape", "Lemon" });
		this._ComboStrings.Location = new System.Drawing.Point(150, 28);
		this._ComboStrings.Name = "_ComboStrings";
		this._ComboStrings.Size = new System.Drawing.Size(260, 24);
		this._ComboStrings.TabIndex = 1;
		this._ComboStrings.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboStrings.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboStrings.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboStrings.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboStrings.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboStrings.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelItems
		//
		this._LabelItems.AutoSize = true;
		this._LabelItems.Location = new System.Drawing.Point(14, 68);
		this._LabelItems.Name = "_LabelItems";
		this._LabelItems.TabIndex = 2;
		this._LabelItems.Text = "&List items:";
		//
		// _ComboItems
		//
		this._ComboItems.Location = new System.Drawing.Point(150, 64);
		this._ComboItems.Name = "_ComboItems";
		this._ComboItems.Size = new System.Drawing.Size(260, 24);
		this._ComboItems.TabIndex = 3;
		this._ComboItems.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboItems.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboItems.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboItems.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboItems.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboItems.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelMany
		//
		this._LabelMany.AutoSize = true;
		this._LabelMany.Location = new System.Drawing.Point(14, 104);
		this._LabelMany.Name = "_LabelMany";
		this._LabelMany.TabIndex = 4;
		this._LabelMany.Text = "&Many items:";
		//
		// _ComboMany
		//
		this._ComboMany.Location = new System.Drawing.Point(150, 100);
		this._ComboMany.Name = "_ComboMany";
		this._ComboMany.Size = new System.Drawing.Size(260, 24);
		this._ComboMany.TabIndex = 5;
		this._ComboMany.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboMany.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboMany.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboMany.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboMany.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboMany.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelLazy
		//
		this._LabelLazy.AutoSize = true;
		this._LabelLazy.Location = new System.Drawing.Point(14, 140);
		this._LabelLazy.Name = "_LabelLazy";
		this._LabelLazy.TabIndex = 6;
		this._LabelLazy.Text = "L&azy loading:";
		//
		// _ComboLazy
		//
		this._ComboLazy.Location = new System.Drawing.Point(150, 136);
		this._ComboLazy.Name = "_ComboLazy";
		this._ComboLazy.Size = new System.Drawing.Size(260, 24);
		this._ComboLazy.TabIndex = 7;
		this._ComboLazy.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboLazy.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboLazy.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboLazy.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboLazy.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboLazy.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		this._ComboLazy.DropDown += new System.EventHandler(this._ComboLazy_DropDown);
		//
		// _LabelObjects
		//
		this._LabelObjects.AutoSize = true;
		this._LabelObjects.Location = new System.Drawing.Point(14, 176);
		this._LabelObjects.Name = "_LabelObjects";
		this._LabelObjects.TabIndex = 8;
		this._LabelObjects.Text = "O&bjects:";
		//
		// _ComboObjects
		//
		this._ComboObjects.Location = new System.Drawing.Point(150, 172);
		this._ComboObjects.Name = "_ComboObjects";
		this._ComboObjects.Size = new System.Drawing.Size(260, 24);
		this._ComboObjects.TabIndex = 9;
		this._ComboObjects.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboObjects.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboObjects.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboObjects.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboObjects.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboObjects.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelStandard
		//
		this._LabelStandard.AutoSize = true;
		this._LabelStandard.Location = new System.Drawing.Point(14, 212);
		this._LabelStandard.Name = "_LabelStandard";
		this._LabelStandard.TabIndex = 10;
		this._LabelStandard.Text = "Standard Combo&Box:";
		//
		// _ComboStandard
		//
		this._ComboStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._ComboStandard.FormattingEnabled = true;
		this._ComboStandard.Items.AddRange(new object[] { "Apple", "Banana", "Cherry", "Blueberry", "Elderberry", "Fig", "Grape", "Lemon" });
		this._ComboStandard.Location = new System.Drawing.Point(150, 208);
		this._ComboStandard.Name = "_ComboStandard";
		this._ComboStandard.Size = new System.Drawing.Size(260, 23);
		this._ComboStandard.TabIndex = 11;
		this._ComboStandard.SelectedIndexChanged += new System.EventHandler(this._ComboStandard_SelectedIndexChanged);
		//
		// _LabelSelected
		//
		this._LabelSelected.AutoEllipsis = true;
		this._LabelSelected.Location = new System.Drawing.Point(14, 250);
		this._LabelSelected.Name = "_LabelSelected";
		this._LabelSelected.Size = new System.Drawing.Size(396, 40);
		this._LabelSelected.TabIndex = 12;
		this._LabelSelected.Text = "(nothing selected)";
		//
		// _GroupDemo
		//
		this._GroupDemo.Controls.Add(this._LabelStrings);
		this._GroupDemo.Controls.Add(this._ComboStrings);
		this._GroupDemo.Controls.Add(this._LabelItems);
		this._GroupDemo.Controls.Add(this._ComboItems);
		this._GroupDemo.Controls.Add(this._LabelMany);
		this._GroupDemo.Controls.Add(this._ComboMany);
		this._GroupDemo.Controls.Add(this._LabelLazy);
		this._GroupDemo.Controls.Add(this._ComboLazy);
		this._GroupDemo.Controls.Add(this._LabelObjects);
		this._GroupDemo.Controls.Add(this._ComboObjects);
		this._GroupDemo.Controls.Add(this._LabelStandard);
		this._GroupDemo.Controls.Add(this._ComboStandard);
		this._GroupDemo.Controls.Add(this._LabelSelected);
		this._GroupDemo.Location = new System.Drawing.Point(12, 12);
		this._GroupDemo.Name = "_GroupDemo";
		this._GroupDemo.Size = new System.Drawing.Size(424, 300);
		this._GroupDemo.TabIndex = 0;
		this._GroupDemo.TabStop = false;
		this._GroupDemo.Text = "Rich combo boxes";
		//
		// _LabelSearch
		//
		this._LabelSearch.AutoSize = true;
		this._LabelSearch.Location = new System.Drawing.Point(14, 32);
		this._LabelSearch.Name = "_LabelSearch";
		this._LabelSearch.TabIndex = 0;
		this._LabelSearch.Text = "Combo (items):";
		//
		// _ComboSearch
		//
		this._ComboSearch.DropDownStyle = Robbiblubber.Spellbook.Controls.ThemedComboBoxStyle.COMBO;
		this._ComboSearch.Location = new System.Drawing.Point(150, 28);
		this._ComboSearch.Name = "_ComboSearch";
		this._ComboSearch.Size = new System.Drawing.Size(260, 24);
		this._ComboSearch.TabIndex = 1;
		this._ComboSearch.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboSearch.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboSearch.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboSearch.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboSearch.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboSearch.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelSource
		//
		this._LabelSource.AutoSize = true;
		this._LabelSource.Location = new System.Drawing.Point(14, 68);
		this._LabelSource.Name = "_LabelSource";
		this._LabelSource.TabIndex = 2;
		this._LabelSource.Text = "Combo (source):";
		//
		// _ComboSource
		//
		this._ComboSource.DropDownStyle = Robbiblubber.Spellbook.Controls.ThemedComboBoxStyle.COMBO;
		this._ComboSource.Location = new System.Drawing.Point(150, 64);
		this._ComboSource.Name = "_ComboSource";
		this._ComboSource.Size = new System.Drawing.Size(260, 24);
		this._ComboSource.TabIndex = 3;
		this._ComboSource.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboSource.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboSource.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboSource.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboSource.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboSource.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _LabelFiles
		//
		this._LabelFiles.AutoSize = true;
		this._LabelFiles.Location = new System.Drawing.Point(14, 104);
		this._LabelFiles.Name = "_LabelFiles";
		this._LabelFiles.TabIndex = 4;
		this._LabelFiles.Text = "Combo (image):";
		//
		// _ComboFiles
		//
		this._ComboFiles.DropDownStyle = Robbiblubber.Spellbook.Controls.ThemedComboBoxStyle.COMBO;
		this._ComboFiles.Location = new System.Drawing.Point(150, 100);
		this._ComboFiles.Name = "_ComboFiles";
		this._ComboFiles.Size = new System.Drawing.Size(260, 24);
		this._ComboFiles.TabIndex = 5;
		this._ComboFiles.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboFiles.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboFiles.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboFiles.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboFiles.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboFiles.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		this._ComboFiles.ResolveImage += new System.EventHandler<Robbiblubber.Spellbook.Controls.ThemedComboBoxImageEventArgs>(this._ComboFiles_ResolveImage);
		//
		// _LabelData
		//
		this._LabelData.AutoSize = true;
		this._LabelData.Location = new System.Drawing.Point(14, 140);
		this._LabelData.Name = "_LabelData";
		this._LabelData.TabIndex = 6;
		this._LabelData.Text = "Data binding:";
		//
		// _ComboData
		//
		this._ComboData.Location = new System.Drawing.Point(150, 136);
		this._ComboData.Name = "_ComboData";
		this._ComboData.Size = new System.Drawing.Size(200, 24);
		this._ComboData.TabIndex = 7;
		this._ComboData.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboData.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboData.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboData.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboData.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboData.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		this._ComboData.SelectedValueChanged += new System.EventHandler(this._ComboData_SelectedValueChanged);
		//
		// _LabelAsync
		//
		this._LabelAsync.AutoSize = true;
		this._LabelAsync.Location = new System.Drawing.Point(14, 176);
		this._LabelAsync.Name = "_LabelAsync";
		this._LabelAsync.TabIndex = 8;
		this._LabelAsync.Text = "Async source:";
		//
		// _ComboAsync
		//
		this._ComboAsync.DropDownStyle = Robbiblubber.Spellbook.Controls.ThemedComboBoxStyle.COMBO;
		this._ComboAsync.Location = new System.Drawing.Point(150, 172);
		this._ComboAsync.Name = "_ComboAsync";
		this._ComboAsync.Size = new System.Drawing.Size(260, 24);
		this._ComboAsync.TabIndex = 9;
		this._ComboAsync.SelectedIndexChanged += new System.EventHandler(this._Combo_SelectedIndexChanged);
		this._ComboAsync.SelectedItemChanged += new System.EventHandler(this._Combo_SelectedItemChanged);
		this._ComboAsync.SelectionChangeCommitted += new System.EventHandler(this._Combo_SelectionChangeCommitted);
		this._ComboAsync.DropDown += new System.EventHandler(this._Combo_DropDown);
		this._ComboAsync.DropDownClosed += new System.EventHandler(this._Combo_DropDownClosed);
		this._ComboAsync.TextChanged += new System.EventHandler(this._Combo_TextChanged);
		//
		// _ButtonAddProduct
		//
		this._ButtonAddProduct.Location = new System.Drawing.Point(356, 135);
		this._ButtonAddProduct.Name = "_ButtonAddProduct";
		this._ButtonAddProduct.Size = new System.Drawing.Size(54, 26);
		this._ButtonAddProduct.TabIndex = 20;
		this._ButtonAddProduct.Text = "Add";
		this._ButtonAddProduct.UseVisualStyleBackColor = true;
		this._ButtonAddProduct.Click += new System.EventHandler(this._ButtonAddProduct_Click);
		//
		// _GroupCombo
		//
		this._GroupCombo.Controls.Add(this._LabelSearch);
		this._GroupCombo.Controls.Add(this._ComboSearch);
		this._GroupCombo.Controls.Add(this._LabelSource);
		this._GroupCombo.Controls.Add(this._ComboSource);
		this._GroupCombo.Controls.Add(this._LabelFiles);
		this._GroupCombo.Controls.Add(this._ComboFiles);
		this._GroupCombo.Controls.Add(this._LabelData);
		this._GroupCombo.Controls.Add(this._ComboData);
		this._GroupCombo.Controls.Add(this._LabelAsync);
		this._GroupCombo.Controls.Add(this._ComboAsync);
		this._GroupCombo.Controls.Add(this._ButtonAddProduct);
		this._GroupCombo.Location = new System.Drawing.Point(12, 318);
		this._GroupCombo.Name = "_GroupCombo";
		this._GroupCombo.Size = new System.Drawing.Size(424, 212);
		this._GroupCombo.TabIndex = 1;
		this._GroupCombo.TabStop = false;
		this._GroupCombo.Text = "Combo mode";
		//
		// _LabelScheme
		//
		this._LabelScheme.AutoSize = true;
		this._LabelScheme.Location = new System.Drawing.Point(14, 32);
		this._LabelScheme.Name = "_LabelScheme";
		this._LabelScheme.TabIndex = 0;
		this._LabelScheme.Text = "Color s&cheme:";
		//
		// _ComboScheme
		//
		this._ComboScheme.Location = new System.Drawing.Point(150, 28);
		this._ComboScheme.Name = "_ComboScheme";
		this._ComboScheme.Size = new System.Drawing.Size(200, 24);
		this._ComboScheme.TabIndex = 1;
		this._ComboScheme.SelectedIndexChanged += new System.EventHandler(this._ComboScheme_SelectedIndexChanged);
		//
		// _ChkReadOnly
		//
		this._ChkReadOnly.AutoSize = true;
		this._ChkReadOnly.Location = new System.Drawing.Point(14, 66);
		this._ChkReadOnly.Name = "_ChkReadOnly";
		this._ChkReadOnly.TabIndex = 2;
		this._ChkReadOnly.Text = "&Read-only";
		this._ChkReadOnly.UseVisualStyleBackColor = true;
		this._ChkReadOnly.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkEnabled
		//
		this._ChkEnabled.AutoSize = true;
		this._ChkEnabled.Checked = true;
		this._ChkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkEnabled.Location = new System.Drawing.Point(150, 66);
		this._ChkEnabled.Name = "_ChkEnabled";
		this._ChkEnabled.TabIndex = 3;
		this._ChkEnabled.Text = "&Enabled";
		this._ChkEnabled.UseVisualStyleBackColor = true;
		this._ChkEnabled.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkDetail
		//
		this._ChkDetail.AutoSize = true;
		this._ChkDetail.Location = new System.Drawing.Point(14, 92);
		this._ChkDetail.Name = "_ChkDetail";
		this._ChkDetail.TabIndex = 4;
		this._ChkDetail.Text = "Show &detail in display";
		this._ChkDetail.UseVisualStyleBackColor = true;
		this._ChkDetail.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFocusColor
		//
		this._ChkFocusColor.AutoSize = true;
		this._ChkFocusColor.Location = new System.Drawing.Point(14, 118);
		this._ChkFocusColor.Name = "_ChkFocusColor";
		this._ChkFocusColor.TabIndex = 5;
		this._ChkFocusColor.Text = "Custom &focus color (orange)";
		this._ChkFocusColor.UseVisualStyleBackColor = true;
		this._ChkFocusColor.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFont
		//
		this._ChkFont.AutoSize = true;
		this._ChkFont.Location = new System.Drawing.Point(14, 144);
		this._ChkFont.Name = "_ChkFont";
		this._ChkFont.TabIndex = 6;
		this._ChkFont.Text = "Lar&ger font";
		this._ChkFont.UseVisualStyleBackColor = true;
		this._ChkFont.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkHover
		//
		this._ChkHover.AutoSize = true;
		this._ChkHover.Location = new System.Drawing.Point(260, 66);
		this._ChkHover.Name = "_ChkHover";
		this._ChkHover.TabIndex = 7;
		this._ChkHover.Text = "Hover color (light blue)";
		this._ChkHover.UseVisualStyleBackColor = true;
		this._ChkHover.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFocusBorder
		//
		this._ChkFocusBorder.AutoSize = true;
		this._ChkFocusBorder.Checked = true;
		this._ChkFocusBorder.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkFocusBorder.Location = new System.Drawing.Point(260, 92);
		this._ChkFocusBorder.Name = "_ChkFocusBorder";
		this._ChkFocusBorder.TabIndex = 8;
		this._ChkFocusBorder.Text = "Focus: border";
		this._ChkFocusBorder.UseVisualStyleBackColor = true;
		this._ChkFocusBorder.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFocusDotted
		//
		this._ChkFocusDotted.AutoSize = true;
		this._ChkFocusDotted.Location = new System.Drawing.Point(260, 118);
		this._ChkFocusDotted.Name = "_ChkFocusDotted";
		this._ChkFocusDotted.TabIndex = 9;
		this._ChkFocusDotted.Text = "Focus: dotted";
		this._ChkFocusDotted.UseVisualStyleBackColor = true;
		this._ChkFocusDotted.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFocusLine
		//
		this._ChkFocusLine.AutoSize = true;
		this._ChkFocusLine.Location = new System.Drawing.Point(260, 144);
		this._ChkFocusLine.Name = "_ChkFocusLine";
		this._ChkFocusLine.TabIndex = 10;
		this._ChkFocusLine.Text = "Focus: line";
		this._ChkFocusLine.UseVisualStyleBackColor = true;
		this._ChkFocusLine.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelWidth
		//
		this._LabelWidth.AutoSize = true;
		this._LabelWidth.Location = new System.Drawing.Point(14, 180);
		this._LabelWidth.Name = "_LabelWidth";
		this._LabelWidth.TabIndex = 11;
		this._LabelWidth.Text = "Drop-down &width:";
		//
		// _NumWidth
		//
		this._NumWidth.Increment = new decimal(new int[] { 50, 0, 0, 0 });
		this._NumWidth.Location = new System.Drawing.Point(150, 176);
		this._NumWidth.Maximum = new decimal(new int[] { 800, 0, 0, 0 });
		this._NumWidth.Name = "_NumWidth";
		this._NumWidth.Size = new System.Drawing.Size(80, 23);
		this._NumWidth.TabIndex = 12;
		this._NumWidth.ValueChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelMaxItems
		//
		this._LabelMaxItems.AutoSize = true;
		this._LabelMaxItems.Location = new System.Drawing.Point(14, 212);
		this._LabelMaxItems.Name = "_LabelMaxItems";
		this._LabelMaxItems.TabIndex = 13;
		this._LabelMaxItems.Text = "Ma&x. items:";
		//
		// _NumMaxItems
		//
		this._NumMaxItems.Location = new System.Drawing.Point(150, 208);
		this._NumMaxItems.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
		this._NumMaxItems.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
		this._NumMaxItems.Name = "_NumMaxItems";
		this._NumMaxItems.Size = new System.Drawing.Size(80, 23);
		this._NumMaxItems.TabIndex = 14;
		this._NumMaxItems.Value = new decimal(new int[] { 12, 0, 0, 0 });
		this._NumMaxItems.ValueChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ButtonOutside
		//
		this._ButtonOutside.Location = new System.Drawing.Point(14, 250);
		this._ButtonOutside.Name = "_ButtonOutside";
		this._ButtonOutside.Size = new System.Drawing.Size(140, 27);
		this._ButtonOutside.TabIndex = 15;
		this._ButtonOutside.Text = "Select &outside item";
		this._ButtonOutside.UseVisualStyleBackColor = true;
		this._ButtonOutside.Click += new System.EventHandler(this._ButtonOutside_Click);
		//
		// _ButtonAdd
		//
		this._ButtonAdd.Location = new System.Drawing.Point(160, 250);
		this._ButtonAdd.Name = "_ButtonAdd";
		this._ButtonAdd.Size = new System.Drawing.Size(80, 27);
		this._ButtonAdd.TabIndex = 16;
		this._ButtonAdd.Text = "Add i&tem";
		this._ButtonAdd.UseVisualStyleBackColor = true;
		this._ButtonAdd.Click += new System.EventHandler(this._ButtonAdd_Click);
		//
		// _ButtonRemove
		//
		this._ButtonRemove.Location = new System.Drawing.Point(246, 250);
		this._ButtonRemove.Name = "_ButtonRemove";
		this._ButtonRemove.Size = new System.Drawing.Size(80, 27);
		this._ButtonRemove.TabIndex = 17;
		this._ButtonRemove.Text = "Remo&ve";
		this._ButtonRemove.UseVisualStyleBackColor = true;
		this._ButtonRemove.Click += new System.EventHandler(this._ButtonRemove_Click);
		//
		// _ButtonClear
		//
		this._ButtonClear.Location = new System.Drawing.Point(332, 250);
		this._ButtonClear.Name = "_ButtonClear";
		this._ButtonClear.Size = new System.Drawing.Size(90, 27);
		this._ButtonClear.TabIndex = 18;
		this._ButtonClear.Text = "Clear &items";
		this._ButtonClear.UseVisualStyleBackColor = true;
		this._ButtonClear.Click += new System.EventHandler(this._ButtonClear_Click);
		//
		// _ChkCombo
		//
		this._ChkCombo.AutoSize = true;
		this._ChkCombo.Location = new System.Drawing.Point(14, 290);
		this._ChkCombo.Name = "_ChkCombo";
		this._ChkCombo.TabIndex = 19;
		this._ChkCombo.Text = "Combo mode (all)";
		this._ChkCombo.UseVisualStyleBackColor = true;
		this._ChkCombo.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkFreeText
		//
		this._ChkFreeText.AutoSize = true;
		this._ChkFreeText.Checked = true;
		this._ChkFreeText.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkFreeText.Location = new System.Drawing.Point(150, 290);
		this._ChkFreeText.Name = "_ChkFreeText";
		this._ChkFreeText.TabIndex = 20;
		this._ChkFreeText.Text = "Allow free text";
		this._ChkFreeText.UseVisualStyleBackColor = true;
		this._ChkFreeText.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkDescriptions
		//
		this._ChkDescriptions.AutoSize = true;
		this._ChkDescriptions.Location = new System.Drawing.Point(280, 290);
		this._ChkDescriptions.Name = "_ChkDescriptions";
		this._ChkDescriptions.TabIndex = 21;
		this._ChkDescriptions.Text = "Show descri&ptions";
		this._ChkDescriptions.UseVisualStyleBackColor = true;
		this._ChkDescriptions.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelMatchMode
		//
		this._LabelMatchMode.AutoSize = true;
		this._LabelMatchMode.Location = new System.Drawing.Point(14, 326);
		this._LabelMatchMode.Name = "_LabelMatchMode";
		this._LabelMatchMode.TabIndex = 22;
		this._LabelMatchMode.Text = "Matc&h mode:";
		//
		// _ComboMatchMode
		//
		this._ComboMatchMode.Location = new System.Drawing.Point(150, 322);
		this._ComboMatchMode.Name = "_ComboMatchMode";
		this._ComboMatchMode.Size = new System.Drawing.Size(130, 24);
		this._ComboMatchMode.TabIndex = 23;
		this._ComboMatchMode.SelectedIndexChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelFilterMode
		//
		this._LabelFilterMode.AutoSize = true;
		this._LabelFilterMode.Location = new System.Drawing.Point(14, 360);
		this._LabelFilterMode.Name = "_LabelFilterMode";
		this._LabelFilterMode.TabIndex = 24;
		this._LabelFilterMode.Text = "Filter mode:";
		//
		// _ComboFilterMode
		//
		this._ComboFilterMode.Location = new System.Drawing.Point(150, 356);
		this._ComboFilterMode.Name = "_ComboFilterMode";
		this._ComboFilterMode.Size = new System.Drawing.Size(130, 24);
		this._ComboFilterMode.TabIndex = 25;
		this._ComboFilterMode.SelectedIndexChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelMinLength
		//
		this._LabelMinLength.AutoSize = true;
		this._LabelMinLength.Location = new System.Drawing.Point(14, 394);
		this._LabelMinLength.Name = "_LabelMinLength";
		this._LabelMinLength.TabIndex = 26;
		this._LabelMinLength.Text = "Min. search length:";
		//
		// _NumMinLength
		//
		this._NumMinLength.Location = new System.Drawing.Point(150, 390);
		this._NumMinLength.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
		this._NumMinLength.Name = "_NumMinLength";
		this._NumMinLength.Size = new System.Drawing.Size(80, 23);
		this._NumMinLength.TabIndex = 27;
		this._NumMinLength.Value = new decimal(new int[] { 1, 0, 0, 0 });
		this._NumMinLength.ValueChanged += new System.EventHandler(this._Options_Changed);
		//
		// _GroupOptions
		//
		this._GroupOptions.Controls.Add(this._LabelScheme);
		this._GroupOptions.Controls.Add(this._ComboScheme);
		this._GroupOptions.Controls.Add(this._ChkReadOnly);
		this._GroupOptions.Controls.Add(this._ChkEnabled);
		this._GroupOptions.Controls.Add(this._ChkDetail);
		this._GroupOptions.Controls.Add(this._ChkFocusColor);
		this._GroupOptions.Controls.Add(this._ChkFont);
		this._GroupOptions.Controls.Add(this._ChkHover);
		this._GroupOptions.Controls.Add(this._ChkFocusBorder);
		this._GroupOptions.Controls.Add(this._ChkFocusDotted);
		this._GroupOptions.Controls.Add(this._ChkFocusLine);
		this._GroupOptions.Controls.Add(this._LabelWidth);
		this._GroupOptions.Controls.Add(this._NumWidth);
		this._GroupOptions.Controls.Add(this._LabelMaxItems);
		this._GroupOptions.Controls.Add(this._NumMaxItems);
		this._GroupOptions.Controls.Add(this._ButtonOutside);
		this._GroupOptions.Controls.Add(this._ButtonAdd);
		this._GroupOptions.Controls.Add(this._ButtonRemove);
		this._GroupOptions.Controls.Add(this._ButtonClear);
		this._GroupOptions.Controls.Add(this._ChkCombo);
		this._GroupOptions.Controls.Add(this._ChkFreeText);
		this._GroupOptions.Controls.Add(this._ChkDescriptions);
		this._GroupOptions.Controls.Add(this._LabelMatchMode);
		this._GroupOptions.Controls.Add(this._ComboMatchMode);
		this._GroupOptions.Controls.Add(this._LabelFilterMode);
		this._GroupOptions.Controls.Add(this._ComboFilterMode);
		this._GroupOptions.Controls.Add(this._LabelMinLength);
		this._GroupOptions.Controls.Add(this._NumMinLength);
		this._GroupOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._GroupOptions.Location = new System.Drawing.Point(448, 12);
		this._GroupOptions.Name = "_GroupOptions";
		this._GroupOptions.Size = new System.Drawing.Size(440, 518);
		this._GroupOptions.TabIndex = 2;
		this._GroupOptions.TabStop = false;
		this._GroupOptions.Text = "Options (apply to the themed combo boxes)";
		//
		// _ListLog
		//
		this._ListLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._ListLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._ListLog.HorizontalScrollbar = true;
		this._ListLog.IntegralHeight = false;
		this._ListLog.ItemHeight = 14;
		this._ListLog.Location = new System.Drawing.Point(12, 542);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(876, 200);
		this._ListLog.TabIndex = 3;
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(900, 754);
		this.Controls.Add(this._ListLog);
		this.Controls.Add(this._GroupOptions);
		this.Controls.Add(this._GroupCombo);
		this.Controls.Add(this._GroupDemo);
		this.MinimumSize = new System.Drawing.Size(916, 640);
		this.Name = "MainWindow";
		this.Text = "Spellbook Themed Combo Box Sample";
		this._GroupOptions.ResumeLayout(false);
		this._GroupOptions.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._NumMinLength)).EndInit();
		((System.ComponentModel.ISupportInitialize)(this._NumMaxItems)).EndInit();
		((System.ComponentModel.ISupportInitialize)(this._NumWidth)).EndInit();
		this._GroupCombo.ResumeLayout(false);
		this._GroupCombo.PerformLayout();
		this._GroupDemo.ResumeLayout(false);
		this._GroupDemo.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label _LabelStrings;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboStrings;
	private System.Windows.Forms.Label _LabelItems;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboItems;
	private System.Windows.Forms.Label _LabelMany;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboMany;
	private System.Windows.Forms.Label _LabelLazy;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboLazy;
	private System.Windows.Forms.Label _LabelObjects;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboObjects;
	private System.Windows.Forms.Label _LabelStandard;
	private System.Windows.Forms.ComboBox _ComboStandard;
	private System.Windows.Forms.Label _LabelSelected;
	private System.Windows.Forms.GroupBox _GroupDemo;
	private System.Windows.Forms.Label _LabelSearch;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboSearch;
	private System.Windows.Forms.Label _LabelSource;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboSource;
	private System.Windows.Forms.Label _LabelFiles;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboFiles;
	private System.Windows.Forms.Label _LabelData;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboData;
	private System.Windows.Forms.Label _LabelAsync;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboAsync;
	private System.Windows.Forms.Button _ButtonAddProduct;
	private System.Windows.Forms.GroupBox _GroupCombo;
	private System.Windows.Forms.Label _LabelScheme;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboScheme;
	private System.Windows.Forms.CheckBox _ChkReadOnly;
	private System.Windows.Forms.CheckBox _ChkEnabled;
	private System.Windows.Forms.CheckBox _ChkDetail;
	private System.Windows.Forms.CheckBox _ChkFocusColor;
	private System.Windows.Forms.CheckBox _ChkFont;
	private System.Windows.Forms.CheckBox _ChkHover;
	private System.Windows.Forms.CheckBox _ChkFocusBorder;
	private System.Windows.Forms.CheckBox _ChkFocusDotted;
	private System.Windows.Forms.CheckBox _ChkFocusLine;
	private System.Windows.Forms.Label _LabelWidth;
	private System.Windows.Forms.NumericUpDown _NumWidth;
	private System.Windows.Forms.Label _LabelMaxItems;
	private System.Windows.Forms.NumericUpDown _NumMaxItems;
	private System.Windows.Forms.Button _ButtonOutside;
	private System.Windows.Forms.Button _ButtonAdd;
	private System.Windows.Forms.Button _ButtonRemove;
	private System.Windows.Forms.Button _ButtonClear;
	private System.Windows.Forms.CheckBox _ChkCombo;
	private System.Windows.Forms.CheckBox _ChkFreeText;
	private System.Windows.Forms.CheckBox _ChkDescriptions;
	private System.Windows.Forms.Label _LabelMatchMode;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboMatchMode;
	private System.Windows.Forms.Label _LabelFilterMode;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboFilterMode;
	private System.Windows.Forms.Label _LabelMinLength;
	private System.Windows.Forms.NumericUpDown _NumMinLength;
	private System.Windows.Forms.GroupBox _GroupOptions;
	private System.Windows.Forms.ListBox _ListLog;
}
