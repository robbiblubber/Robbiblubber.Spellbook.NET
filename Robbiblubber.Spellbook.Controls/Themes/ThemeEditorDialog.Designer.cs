namespace Robbiblubber.Spellbook.Controls.Themes;

partial class ThemeEditorDialog
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
		this._LabelThemes = new System.Windows.Forms.Label();
		this._ListThemes = new System.Windows.Forms.ListBox();
		this._ButtonNew = new System.Windows.Forms.Button();
		this._ButtonCopy = new System.Windows.Forms.Button();
		this._ButtonRemove = new System.Windows.Forms.Button();
		this._ButtonImport = new System.Windows.Forms.Button();
		this._ButtonExport = new System.Windows.Forms.Button();
		this._LabelName = new System.Windows.Forms.Label();
		this._TextName = new Robbiblubber.Spellbook.Controls.ThemedTextBox();
		this._LabelBase = new System.Windows.Forms.Label();
		this._ComboBase = new System.Windows.Forms.ComboBox();
		this._LabelColors = new System.Windows.Forms.Label();
		this._ColumnRole = new System.Windows.Forms.ColumnHeader();
		this._ColumnValue = new System.Windows.Forms.ColumnHeader();
		this._ColumnSet = new System.Windows.Forms.ColumnHeader();
		this._ListColors = new System.Windows.Forms.ListView();
		this._ButtonChange = new System.Windows.Forms.Button();
		this._ButtonResetColor = new System.Windows.Forms.Button();
		this._ButtonResetAll = new System.Windows.Forms.Button();
		this._LabelInfo = new System.Windows.Forms.Label();
		this._GroupPreview = new Robbiblubber.Spellbook.Controls.ThemedGroupBox();
		this._PreviewLabel = new System.Windows.Forms.Label();
		this._PreviewText = new Robbiblubber.Spellbook.Controls.ThemedTextBox();
		this._PreviewCombo = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._PreviewCheck = new Robbiblubber.Spellbook.Controls.ThemedCheckBox();
		this._PreviewRadio = new Robbiblubber.Spellbook.Controls.ThemedRadioButton();
		this._PreviewButton = new Robbiblubber.Spellbook.Controls.ThemedButton();
		this._PreviewDisabled = new Robbiblubber.Spellbook.Controls.ThemedButton();
		this._PreviewMeter = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._PreviewList = new System.Windows.Forms.ListBox();
		this._PreviewTabs = new Robbiblubber.Spellbook.Controls.ThemedTabControl();
		this._PreviewPage1 = new System.Windows.Forms.TabPage();
		this._PreviewPage2 = new System.Windows.Forms.TabPage();
		this._PreviewDisabledText = new Robbiblubber.Spellbook.Controls.ThemedTextBox();
		this._PreviewLink = new System.Windows.Forms.LinkLabel();
		this._ButtonOk = new System.Windows.Forms.Button();
		this._ButtonCancel = new System.Windows.Forms.Button();
		this._GroupPreview.SuspendLayout();
		this._PreviewTabs.SuspendLayout();
		this._PreviewPage1.SuspendLayout();
		this.SuspendLayout();
		//
		// _LabelThemes
		//
		this._LabelThemes.AutoSize = true;
		this._LabelThemes.Location = new System.Drawing.Point(12, 12);
		this._LabelThemes.Size = new System.Drawing.Size(48, 15);
		this._LabelThemes.TabIndex = 0;
		this._LabelThemes.Text = "&Themes:";
		this._LabelThemes.Name = "_LabelThemes";
		//
		// _ListThemes
		//
		this._ListThemes.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ListThemes.FormattingEnabled = true;
		this._ListThemes.IntegralHeight = false;
		this._ListThemes.Location = new System.Drawing.Point(12, 32);
		this._ListThemes.Size = new System.Drawing.Size(200, 420);
		this._ListThemes.TabIndex = 1;
		this._ListThemes.SelectedIndexChanged += new System.EventHandler(this._ListThemes_SelectedIndexChanged);
		this._ListThemes.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._Theme_Format);
		this._ListThemes.Name = "_ListThemes";
		//
		// _ButtonNew
		//
		this._ButtonNew.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonNew.Location = new System.Drawing.Point(12, 460);
		this._ButtonNew.Size = new System.Drawing.Size(98, 28);
		this._ButtonNew.TabIndex = 2;
		this._ButtonNew.Text = "&New";
		this._ButtonNew.UseVisualStyleBackColor = true;
		this._ButtonNew.Click += new System.EventHandler(this._ButtonNew_Click);
		this._ButtonNew.Name = "_ButtonNew";
		//
		// _ButtonCopy
		//
		this._ButtonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonCopy.Location = new System.Drawing.Point(114, 460);
		this._ButtonCopy.Size = new System.Drawing.Size(98, 28);
		this._ButtonCopy.TabIndex = 3;
		this._ButtonCopy.Text = "Co&py";
		this._ButtonCopy.UseVisualStyleBackColor = true;
		this._ButtonCopy.Click += new System.EventHandler(this._ButtonCopy_Click);
		this._ButtonCopy.Name = "_ButtonCopy";
		//
		// _ButtonRemove
		//
		this._ButtonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonRemove.Location = new System.Drawing.Point(12, 494);
		this._ButtonRemove.Size = new System.Drawing.Size(98, 28);
		this._ButtonRemove.TabIndex = 4;
		this._ButtonRemove.Text = "&Remove";
		this._ButtonRemove.UseVisualStyleBackColor = true;
		this._ButtonRemove.Click += new System.EventHandler(this._ButtonRemove_Click);
		this._ButtonRemove.Name = "_ButtonRemove";
		//
		// _ButtonImport
		//
		this._ButtonImport.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonImport.Location = new System.Drawing.Point(114, 494);
		this._ButtonImport.Size = new System.Drawing.Size(98, 28);
		this._ButtonImport.TabIndex = 5;
		this._ButtonImport.Text = "&Import...";
		this._ButtonImport.UseVisualStyleBackColor = true;
		this._ButtonImport.Click += new System.EventHandler(this._ButtonImport_Click);
		this._ButtonImport.Name = "_ButtonImport";
		//
		// _ButtonExport
		//
		this._ButtonExport.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonExport.Location = new System.Drawing.Point(12, 528);
		this._ButtonExport.Size = new System.Drawing.Size(98, 28);
		this._ButtonExport.TabIndex = 6;
		this._ButtonExport.Text = "&Export...";
		this._ButtonExport.UseVisualStyleBackColor = true;
		this._ButtonExport.Click += new System.EventHandler(this._ButtonExport_Click);
		this._ButtonExport.Name = "_ButtonExport";
		//
		// _LabelName
		//
		this._LabelName.AutoSize = true;
		this._LabelName.Location = new System.Drawing.Point(224, 15);
		this._LabelName.Size = new System.Drawing.Size(42, 15);
		this._LabelName.TabIndex = 7;
		this._LabelName.Text = "N&ame:";
		this._LabelName.Name = "_LabelName";
		//
		// _TextName
		//
		this._TextName.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
		this._TextName.Location = new System.Drawing.Point(310, 12);
		this._TextName.Size = new System.Drawing.Size(274, 23);
		this._TextName.TabIndex = 8;
		this._TextName.TextChanged += new System.EventHandler(this._TextName_TextChanged);
		this._TextName.Name = "_TextName";
		//
		// _LabelBase
		//
		this._LabelBase.AutoSize = true;
		this._LabelBase.Location = new System.Drawing.Point(224, 47);
		this._LabelBase.Size = new System.Drawing.Size(60, 15);
		this._LabelBase.TabIndex = 9;
		this._LabelBase.Text = "&Based on:";
		this._LabelBase.Name = "_LabelBase";
		//
		// _ComboBase
		//
		this._ComboBase.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
		this._ComboBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._ComboBase.FormattingEnabled = true;
		this._ComboBase.Location = new System.Drawing.Point(310, 44);
		this._ComboBase.Size = new System.Drawing.Size(274, 23);
		this._ComboBase.TabIndex = 10;
		this._ComboBase.SelectedIndexChanged += new System.EventHandler(this._ComboBase_SelectedIndexChanged);
		this._ComboBase.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._Theme_Format);
		this._ComboBase.Name = "_ComboBase";
		//
		// _LabelColors
		//
		this._LabelColors.AutoSize = true;
		this._LabelColors.Location = new System.Drawing.Point(224, 76);
		this._LabelColors.Size = new System.Drawing.Size(44, 15);
		this._LabelColors.TabIndex = 11;
		this._LabelColors.Text = "&Colors:";
		this._LabelColors.Name = "_LabelColors";
		//
		// _ColumnRole
		//
		this._ColumnRole.Text = "Role";
		this._ColumnRole.Width = 170;
		this._ColumnRole.Name = "_ColumnRole";
		//
		// _ColumnValue
		//
		this._ColumnValue.Text = "Color";
		this._ColumnValue.Width = 100;
		this._ColumnValue.Name = "_ColumnValue";
		//
		// _ColumnSet
		//
		this._ColumnSet.Text = "Set";
		this._ColumnSet.Width = 60;
		this._ColumnSet.Name = "_ColumnSet";
		//
		// _ListColors
		//
		this._ListColors.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
		this._ListColors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this._ColumnRole, this._ColumnValue, this._ColumnSet });
		this._ListColors.FullRowSelect = true;
		this._ListColors.HideSelection = false;
		this._ListColors.Location = new System.Drawing.Point(224, 96);
		this._ListColors.MultiSelect = false;
		this._ListColors.Size = new System.Drawing.Size(360, 390);
		this._ListColors.TabIndex = 12;
		this._ListColors.UseCompatibleStateImageBehavior = false;
		this._ListColors.View = System.Windows.Forms.View.Details;
		this._ListColors.SelectedIndexChanged += new System.EventHandler(this._ListColors_SelectedIndexChanged);
		this._ListColors.DoubleClick += new System.EventHandler(this._ListColors_DoubleClick);
		this._ListColors.KeyDown += new System.Windows.Forms.KeyEventHandler(this._ListColors_KeyDown);
		this._ListColors.Name = "_ListColors";
		//
		// _ButtonChange
		//
		this._ButtonChange.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonChange.Location = new System.Drawing.Point(224, 494);
		this._ButtonChange.Size = new System.Drawing.Size(110, 28);
		this._ButtonChange.TabIndex = 13;
		this._ButtonChange.Text = "C&hange...";
		this._ButtonChange.UseVisualStyleBackColor = true;
		this._ButtonChange.Click += new System.EventHandler(this._ButtonChange_Click);
		this._ButtonChange.Name = "_ButtonChange";
		//
		// _ButtonResetColor
		//
		this._ButtonResetColor.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonResetColor.Location = new System.Drawing.Point(340, 494);
		this._ButtonResetColor.Size = new System.Drawing.Size(110, 28);
		this._ButtonResetColor.TabIndex = 14;
		this._ButtonResetColor.Text = "Reset c&olor";
		this._ButtonResetColor.UseVisualStyleBackColor = true;
		this._ButtonResetColor.Click += new System.EventHandler(this._ButtonResetColor_Click);
		this._ButtonResetColor.Name = "_ButtonResetColor";
		//
		// _ButtonResetAll
		//
		this._ButtonResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
		this._ButtonResetAll.Location = new System.Drawing.Point(456, 494);
		this._ButtonResetAll.Size = new System.Drawing.Size(110, 28);
		this._ButtonResetAll.TabIndex = 15;
		this._ButtonResetAll.Text = "Reset a&ll";
		this._ButtonResetAll.UseVisualStyleBackColor = true;
		this._ButtonResetAll.Click += new System.EventHandler(this._ButtonResetAll_Click);
		this._ButtonResetAll.Name = "_ButtonResetAll";
		//
		// _LabelInfo
		//
		this._LabelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
		this._LabelInfo.Location = new System.Drawing.Point(224, 528);
		this._LabelInfo.Size = new System.Drawing.Size(360, 28);
		this._LabelInfo.TabIndex = 16;
		this._LabelInfo.Text = "Built-in themes cannot be changed. Create a new theme based on it.";
		this._LabelInfo.Name = "_LabelInfo";
		//
		// _GroupPreview
		//
		this._GroupPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
		this._GroupPreview.Location = new System.Drawing.Point(596, 12);
		this._GroupPreview.Size = new System.Drawing.Size(252, 508);
		this._GroupPreview.TabIndex = 17;
		this._GroupPreview.TabStop = false;
		this._GroupPreview.Text = "Preview";
		this._GroupPreview.Name = "_GroupPreview";
		this._GroupPreview.Controls.Add(this._PreviewLabel);
		this._GroupPreview.Controls.Add(this._PreviewText);
		this._GroupPreview.Controls.Add(this._PreviewCombo);
		this._GroupPreview.Controls.Add(this._PreviewCheck);
		this._GroupPreview.Controls.Add(this._PreviewRadio);
		this._GroupPreview.Controls.Add(this._PreviewButton);
		this._GroupPreview.Controls.Add(this._PreviewDisabled);
		this._GroupPreview.Controls.Add(this._PreviewMeter);
		this._GroupPreview.Controls.Add(this._PreviewList);
		this._GroupPreview.Controls.Add(this._PreviewTabs);
		this._GroupPreview.Controls.Add(this._PreviewLink);
		//
		// _PreviewLabel
		//
		this._PreviewLabel.AutoSize = true;
		this._PreviewLabel.Location = new System.Drawing.Point(12, 26);
		this._PreviewLabel.Size = new System.Drawing.Size(35, 15);
		this._PreviewLabel.TabIndex = 0;
		this._PreviewLabel.Text = "Label";
		this._PreviewLabel.Name = "_PreviewLabel";
		//
		// _PreviewText
		//
		this._PreviewText.Location = new System.Drawing.Point(12, 46);
		this._PreviewText.Size = new System.Drawing.Size(228, 23);
		this._PreviewText.TabIndex = 1;
		this._PreviewText.Text = "Text box";
		this._PreviewText.Name = "_PreviewText";
		//
		// _PreviewCombo
		//
		this._PreviewCombo.Location = new System.Drawing.Point(12, 78);
		this._PreviewCombo.Size = new System.Drawing.Size(228, 24);
		this._PreviewCombo.TabIndex = 2;
		this._PreviewCombo.Name = "_PreviewCombo";
		//
		// _PreviewCheck
		//
		this._PreviewCheck.AutoSize = true;
		this._PreviewCheck.Checked = true;
		this._PreviewCheck.CheckState = System.Windows.Forms.CheckState.Checked;
		this._PreviewCheck.Location = new System.Drawing.Point(12, 112);
		this._PreviewCheck.Size = new System.Drawing.Size(80, 19);
		this._PreviewCheck.TabIndex = 3;
		this._PreviewCheck.Text = "Check box";
		this._PreviewCheck.Name = "_PreviewCheck";
		//
		// _PreviewRadio
		//
		this._PreviewRadio.AutoSize = true;
		this._PreviewRadio.Checked = true;
		this._PreviewRadio.Location = new System.Drawing.Point(130, 112);
		this._PreviewRadio.Size = new System.Drawing.Size(65, 19);
		this._PreviewRadio.TabIndex = 4;
		this._PreviewRadio.TabStop = true;
		this._PreviewRadio.Text = "Option";
		this._PreviewRadio.Name = "_PreviewRadio";
		//
		// _PreviewButton
		//
		this._PreviewButton.Location = new System.Drawing.Point(12, 142);
		this._PreviewButton.Size = new System.Drawing.Size(110, 28);
		this._PreviewButton.TabIndex = 5;
		this._PreviewButton.Text = "Button";
		this._PreviewButton.Name = "_PreviewButton";
		//
		// _PreviewDisabled
		//
		this._PreviewDisabled.Enabled = false;
		this._PreviewDisabled.Location = new System.Drawing.Point(130, 142);
		this._PreviewDisabled.Size = new System.Drawing.Size(110, 28);
		this._PreviewDisabled.TabIndex = 6;
		this._PreviewDisabled.Text = "Disabled";
		this._PreviewDisabled.Name = "_PreviewDisabled";
		//
		// _PreviewMeter
		//
		this._PreviewMeter.Location = new System.Drawing.Point(12, 180);
		this._PreviewMeter.Size = new System.Drawing.Size(228, 20);
		this._PreviewMeter.TabIndex = 7;
		this._PreviewMeter.Value = 60;
		this._PreviewMeter.Name = "_PreviewMeter";
		//
		// _PreviewList
		//
		this._PreviewList.FormattingEnabled = true;
		this._PreviewList.IntegralHeight = false;
		this._PreviewList.Items.AddRange(new object[] { "Item 1", "Selected item", "Item 3", "Item 4" });
		this._PreviewList.Location = new System.Drawing.Point(12, 210);
		this._PreviewList.Size = new System.Drawing.Size(228, 80);
		this._PreviewList.TabIndex = 8;
		this._PreviewList.Name = "_PreviewList";
		//
		// _PreviewTabs
		//
		this._PreviewTabs.Location = new System.Drawing.Point(12, 300);
		this._PreviewTabs.SelectedIndex = 0;
		this._PreviewTabs.Size = new System.Drawing.Size(228, 120);
		this._PreviewTabs.TabIndex = 9;
		this._PreviewTabs.Name = "_PreviewTabs";
		this._PreviewTabs.Controls.Add(this._PreviewPage1);
		this._PreviewTabs.Controls.Add(this._PreviewPage2);
		//
		// _PreviewPage1
		//
		this._PreviewPage1.Location = new System.Drawing.Point(4, 24);
		this._PreviewPage1.Padding = new System.Windows.Forms.Padding(3);
		this._PreviewPage1.Size = new System.Drawing.Size(220, 92);
		this._PreviewPage1.TabIndex = 0;
		this._PreviewPage1.Text = "Page";
		this._PreviewPage1.UseVisualStyleBackColor = true;
		this._PreviewPage1.Name = "_PreviewPage1";
		this._PreviewPage1.Controls.Add(this._PreviewDisabledText);
		//
		// _PreviewPage2
		//
		this._PreviewPage2.Location = new System.Drawing.Point(4, 24);
		this._PreviewPage2.Padding = new System.Windows.Forms.Padding(3);
		this._PreviewPage2.Size = new System.Drawing.Size(220, 92);
		this._PreviewPage2.TabIndex = 1;
		this._PreviewPage2.Text = "Other page";
		this._PreviewPage2.UseVisualStyleBackColor = true;
		this._PreviewPage2.Name = "_PreviewPage2";
		//
		// _PreviewDisabledText
		//
		this._PreviewDisabledText.Enabled = false;
		this._PreviewDisabledText.Location = new System.Drawing.Point(8, 8);
		this._PreviewDisabledText.Size = new System.Drawing.Size(200, 23);
		this._PreviewDisabledText.TabIndex = 0;
		this._PreviewDisabledText.Text = "Disabled text box";
		this._PreviewDisabledText.Name = "_PreviewDisabledText";
		//
		// _PreviewLink
		//
		this._PreviewLink.AutoSize = true;
		this._PreviewLink.Location = new System.Drawing.Point(12, 430);
		this._PreviewLink.Size = new System.Drawing.Size(29, 15);
		this._PreviewLink.TabIndex = 10;
		this._PreviewLink.TabStop = true;
		this._PreviewLink.Text = "Link";
		this._PreviewLink.Name = "_PreviewLink";
		//
		// _ButtonOk
		//
		this._ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
		this._ButtonOk.Location = new System.Drawing.Point(668, 560);
		this._ButtonOk.Size = new System.Drawing.Size(88, 28);
		this._ButtonOk.TabIndex = 18;
		this._ButtonOk.Text = "OK";
		this._ButtonOk.UseVisualStyleBackColor = true;
		this._ButtonOk.Click += new System.EventHandler(this._ButtonOk_Click);
		this._ButtonOk.Name = "_ButtonOk";
		//
		// _ButtonCancel
		//
		this._ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
		this._ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this._ButtonCancel.Location = new System.Drawing.Point(760, 560);
		this._ButtonCancel.Size = new System.Drawing.Size(88, 28);
		this._ButtonCancel.TabIndex = 19;
		this._ButtonCancel.Text = "Cancel";
		this._ButtonCancel.UseVisualStyleBackColor = true;
		this._ButtonCancel.Name = "_ButtonCancel";
		//
		// ThemeEditorDialog
		//
		this.AcceptButton = this._ButtonOk;
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.CancelButton = this._ButtonCancel;
		this.ClientSize = new System.Drawing.Size(860, 600);
		this.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(760, 520);
		this.Name = "ThemeEditorDialog";
		this.ShowIcon = false;
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Themes";
		this.Controls.Add(this._LabelThemes);
		this.Controls.Add(this._ListThemes);
		this.Controls.Add(this._ButtonNew);
		this.Controls.Add(this._ButtonCopy);
		this.Controls.Add(this._ButtonRemove);
		this.Controls.Add(this._ButtonImport);
		this.Controls.Add(this._ButtonExport);
		this.Controls.Add(this._LabelName);
		this.Controls.Add(this._TextName);
		this.Controls.Add(this._LabelBase);
		this.Controls.Add(this._ComboBase);
		this.Controls.Add(this._LabelColors);
		this.Controls.Add(this._ListColors);
		this.Controls.Add(this._ButtonChange);
		this.Controls.Add(this._ButtonResetColor);
		this.Controls.Add(this._ButtonResetAll);
		this.Controls.Add(this._LabelInfo);
		this.Controls.Add(this._GroupPreview);
		this.Controls.Add(this._ButtonOk);
		this.Controls.Add(this._ButtonCancel);
		this._PreviewPage1.ResumeLayout(false);
		this._PreviewPage1.PerformLayout();
		this._PreviewTabs.ResumeLayout(false);
		this._PreviewTabs.PerformLayout();
		this._GroupPreview.ResumeLayout(false);
		this._GroupPreview.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label _LabelThemes;
	private System.Windows.Forms.ListBox _ListThemes;
	private System.Windows.Forms.Button _ButtonNew;
	private System.Windows.Forms.Button _ButtonCopy;
	private System.Windows.Forms.Button _ButtonRemove;
	private System.Windows.Forms.Button _ButtonImport;
	private System.Windows.Forms.Button _ButtonExport;
	private System.Windows.Forms.Label _LabelName;
	private Robbiblubber.Spellbook.Controls.ThemedTextBox _TextName;
	private System.Windows.Forms.Label _LabelBase;
	private System.Windows.Forms.ComboBox _ComboBase;
	private System.Windows.Forms.Label _LabelColors;
	private System.Windows.Forms.ColumnHeader _ColumnRole;
	private System.Windows.Forms.ColumnHeader _ColumnValue;
	private System.Windows.Forms.ColumnHeader _ColumnSet;
	private System.Windows.Forms.ListView _ListColors;
	private System.Windows.Forms.Button _ButtonChange;
	private System.Windows.Forms.Button _ButtonResetColor;
	private System.Windows.Forms.Button _ButtonResetAll;
	private System.Windows.Forms.Label _LabelInfo;
	private Robbiblubber.Spellbook.Controls.ThemedGroupBox _GroupPreview;
	private System.Windows.Forms.Label _PreviewLabel;
	private Robbiblubber.Spellbook.Controls.ThemedTextBox _PreviewText;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _PreviewCombo;
	private Robbiblubber.Spellbook.Controls.ThemedCheckBox _PreviewCheck;
	private Robbiblubber.Spellbook.Controls.ThemedRadioButton _PreviewRadio;
	private Robbiblubber.Spellbook.Controls.ThemedButton _PreviewButton;
	private Robbiblubber.Spellbook.Controls.ThemedButton _PreviewDisabled;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _PreviewMeter;
	private System.Windows.Forms.ListBox _PreviewList;
	private Robbiblubber.Spellbook.Controls.ThemedTabControl _PreviewTabs;
	private System.Windows.Forms.TabPage _PreviewPage1;
	private System.Windows.Forms.TabPage _PreviewPage2;
	private Robbiblubber.Spellbook.Controls.ThemedTextBox _PreviewDisabledText;
	private System.Windows.Forms.LinkLabel _PreviewLink;
	private System.Windows.Forms.Button _ButtonOk;
	private System.Windows.Forms.Button _ButtonCancel;
}
