namespace Robbiblubber.Spellbook.Controls.Samples.Themes;

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
		this._ThemeProvider = new Robbiblubber.Spellbook.Controls.Themes.ThemeProvider(this.components);
		this._LabelTheme = new System.Windows.Forms.Label();
		this._ComboTheme = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._ButtonNew = new System.Windows.Forms.Button();
		this._ButtonRemove = new System.Windows.Forms.Button();
		this._ButtonAccent = new System.Windows.Forms.Button();
		this._ButtonBack = new System.Windows.Forms.Button();
		this._ButtonResetColors = new System.Windows.Forms.Button();
		this._ButtonBase = new System.Windows.Forms.Button();
		this._ButtonExport = new System.Windows.Forms.Button();
		this._ButtonImport = new System.Windows.Forms.Button();
		this._ButtonSave = new System.Windows.Forms.Button();
		this._ButtonLoad = new System.Windows.Forms.Button();
		this._ButtonControls = new System.Windows.Forms.Button();
		this._ButtonEditor = new System.Windows.Forms.Button();
		this._ButtonMessageBox = new System.Windows.Forms.Button();
		this._LabelInfo = new System.Windows.Forms.Label();
		this._GroupTheme = new System.Windows.Forms.GroupBox();
		this._LabelCombo = new System.Windows.Forms.Label();
		this._ComboSample = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelMeter = new System.Windows.Forms.Label();
		this._MeterApp = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelMeterLight = new System.Windows.Forms.Label();
		this._MeterLight = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelPassword = new System.Windows.Forms.Label();
		this._TextPassword = new System.Windows.Forms.TextBox();
		this._IndicatorPassword = new Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator();
		this._GroupControls = new System.Windows.Forms.GroupBox();
		this._ColumnRole = new System.Windows.Forms.ColumnHeader();
		this._ColumnColor = new System.Windows.Forms.ColumnHeader();
		this._ColumnSet = new System.Windows.Forms.ColumnHeader();
		this._ListPalette = new System.Windows.Forms.ListView();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._GroupTheme.SuspendLayout();
		this._GroupControls.SuspendLayout();
		this.SuspendLayout();
		//
		// _LabelTheme
		//
		this._LabelTheme.AutoSize = true;
		this._LabelTheme.Location = new System.Drawing.Point(12, 28);
		this._LabelTheme.Name = "_LabelTheme";
		this._LabelTheme.Text = "&Theme:";
		this._LabelTheme.TabIndex = 0;
		//
		// _ComboTheme
		//
		this._ComboTheme.Location = new System.Drawing.Point(80, 24);
		this._ComboTheme.Name = "_ComboTheme";
		this._ComboTheme.Size = new System.Drawing.Size(205, 24);
		this._ComboTheme.SelectedIndexChanged += new System.EventHandler(this._ComboTheme_SelectedIndexChanged);
		this._ComboTheme.TabIndex = 1;
		//
		// _ButtonNew
		//
		this._ButtonNew.Location = new System.Drawing.Point(12, 60);
		this._ButtonNew.Name = "_ButtonNew";
		this._ButtonNew.Size = new System.Drawing.Size(132, 28);
		this._ButtonNew.Text = "&New theme";
		this._ButtonNew.UseVisualStyleBackColor = true;
		this._ButtonNew.Click += new System.EventHandler(this._ButtonNew_Click);
		this._ButtonNew.TabIndex = 2;
		//
		// _ButtonRemove
		//
		this._ButtonRemove.Location = new System.Drawing.Point(152, 60);
		this._ButtonRemove.Name = "_ButtonRemove";
		this._ButtonRemove.Size = new System.Drawing.Size(132, 28);
		this._ButtonRemove.Text = "&Remove";
		this._ButtonRemove.UseVisualStyleBackColor = true;
		this._ButtonRemove.Click += new System.EventHandler(this._ButtonRemove_Click);
		this._ButtonRemove.TabIndex = 3;
		//
		// _ButtonAccent
		//
		this._ButtonAccent.Location = new System.Drawing.Point(12, 94);
		this._ButtonAccent.Name = "_ButtonAccent";
		this._ButtonAccent.Size = new System.Drawing.Size(132, 28);
		this._ButtonAccent.Text = "&Accent color...";
		this._ButtonAccent.UseVisualStyleBackColor = true;
		this._ButtonAccent.Click += new System.EventHandler(this._ButtonAccent_Click);
		this._ButtonAccent.TabIndex = 4;
		//
		// _ButtonBack
		//
		this._ButtonBack.Location = new System.Drawing.Point(152, 94);
		this._ButtonBack.Name = "_ButtonBack";
		this._ButtonBack.Size = new System.Drawing.Size(132, 28);
		this._ButtonBack.Text = "&Back color...";
		this._ButtonBack.UseVisualStyleBackColor = true;
		this._ButtonBack.Click += new System.EventHandler(this._ButtonBack_Click);
		this._ButtonBack.TabIndex = 5;
		//
		// _ButtonResetColors
		//
		this._ButtonResetColors.Location = new System.Drawing.Point(12, 128);
		this._ButtonResetColors.Name = "_ButtonResetColors";
		this._ButtonResetColors.Size = new System.Drawing.Size(132, 28);
		this._ButtonResetColors.Text = "Reset &colors";
		this._ButtonResetColors.UseVisualStyleBackColor = true;
		this._ButtonResetColors.Click += new System.EventHandler(this._ButtonResetColors_Click);
		this._ButtonResetColors.TabIndex = 6;
		//
		// _ButtonBase
		//
		this._ButtonBase.Location = new System.Drawing.Point(152, 128);
		this._ButtonBase.Name = "_ButtonBase";
		this._ButtonBase.Size = new System.Drawing.Size(132, 28);
		this._ButtonBase.Text = "Switch ba&se";
		this._ButtonBase.UseVisualStyleBackColor = true;
		this._ButtonBase.Click += new System.EventHandler(this._ButtonBase_Click);
		this._ButtonBase.TabIndex = 7;
		//
		// _ButtonExport
		//
		this._ButtonExport.Location = new System.Drawing.Point(12, 162);
		this._ButtonExport.Name = "_ButtonExport";
		this._ButtonExport.Size = new System.Drawing.Size(132, 28);
		this._ButtonExport.Text = "E&xport...";
		this._ButtonExport.UseVisualStyleBackColor = true;
		this._ButtonExport.Click += new System.EventHandler(this._ButtonExport_Click);
		this._ButtonExport.TabIndex = 8;
		//
		// _ButtonImport
		//
		this._ButtonImport.Location = new System.Drawing.Point(152, 162);
		this._ButtonImport.Name = "_ButtonImport";
		this._ButtonImport.Size = new System.Drawing.Size(132, 28);
		this._ButtonImport.Text = "&Import...";
		this._ButtonImport.UseVisualStyleBackColor = true;
		this._ButtonImport.Click += new System.EventHandler(this._ButtonImport_Click);
		this._ButtonImport.TabIndex = 9;
		//
		// _ButtonSave
		//
		this._ButtonSave.Location = new System.Drawing.Point(12, 196);
		this._ButtonSave.Name = "_ButtonSave";
		this._ButtonSave.Size = new System.Drawing.Size(132, 28);
		this._ButtonSave.Text = "Sa&ve settings";
		this._ButtonSave.UseVisualStyleBackColor = true;
		this._ButtonSave.Click += new System.EventHandler(this._ButtonSave_Click);
		this._ButtonSave.TabIndex = 10;
		//
		// _ButtonLoad
		//
		this._ButtonLoad.Location = new System.Drawing.Point(152, 196);
		this._ButtonLoad.Name = "_ButtonLoad";
		this._ButtonLoad.Size = new System.Drawing.Size(132, 28);
		this._ButtonLoad.Text = "&Load settings";
		this._ButtonLoad.UseVisualStyleBackColor = true;
		this._ButtonLoad.Click += new System.EventHandler(this._ButtonLoad_Click);
		this._ButtonLoad.TabIndex = 11;
		//
		// _ButtonControls
		//
		this._ButtonControls.Location = new System.Drawing.Point(12, 230);
		this._ButtonControls.Name = "_ButtonControls";
		this._ButtonControls.Size = new System.Drawing.Size(272, 28);
		this._ButtonControls.Text = "Controls &window...";
		this._ButtonControls.UseVisualStyleBackColor = true;
		this._ButtonControls.Click += new System.EventHandler(this._ButtonControls_Click);
		this._ButtonControls.TabIndex = 12;
		//
		// _ButtonEditor
		//
		this._ButtonEditor.Location = new System.Drawing.Point(12, 264);
		this._ButtonEditor.Name = "_ButtonEditor";
		this._ButtonEditor.Size = new System.Drawing.Size(132, 28);
		this._ButtonEditor.Text = "Theme &editor...";
		this._ButtonEditor.UseVisualStyleBackColor = true;
		this._ButtonEditor.Click += new System.EventHandler(this._ButtonEditor_Click);
		this._ButtonEditor.TabIndex = 13;
		//
		// _ButtonMessageBox
		//
		this._ButtonMessageBox.Location = new System.Drawing.Point(152, 264);
		this._ButtonMessageBox.Name = "_ButtonMessageBox";
		this._ButtonMessageBox.Size = new System.Drawing.Size(132, 28);
		this._ButtonMessageBox.Text = "&Message box...";
		this._ButtonMessageBox.UseVisualStyleBackColor = true;
		this._ButtonMessageBox.Click += new System.EventHandler(this._ButtonMessageBox_Click);
		this._ButtonMessageBox.TabIndex = 14;
		//
		// _LabelInfo
		//
		this._LabelInfo.Location = new System.Drawing.Point(12, 302);
		this._LabelInfo.Name = "_LabelInfo";
		this._LabelInfo.Size = new System.Drawing.Size(273, 54);
		this._LabelInfo.Text = "";
		this._LabelInfo.TabIndex = 15;
		//
		// _GroupTheme
		//
		this._GroupTheme.Location = new System.Drawing.Point(12, 12);
		this._GroupTheme.Name = "_GroupTheme";
		this._GroupTheme.Size = new System.Drawing.Size(300, 370);
		this._GroupTheme.Text = "Theme of the application";
		this._GroupTheme.TabIndex = 13;
		//
		// _LabelCombo
		//
		this._LabelCombo.AutoSize = true;
		this._LabelCombo.Location = new System.Drawing.Point(12, 28);
		this._LabelCombo.Name = "_LabelCombo";
		this._LabelCombo.Text = "C&ombo box:";
		this._LabelCombo.TabIndex = 0;
		//
		// _ComboSample
		//
		this._ComboSample.Location = new System.Drawing.Point(100, 24);
		this._ComboSample.Name = "_ComboSample";
		this._ComboSample.Size = new System.Drawing.Size(185, 24);
		this._ComboSample.TabIndex = 1;
		//
		// _LabelMeter
		//
		this._LabelMeter.AutoSize = true;
		this._LabelMeter.Location = new System.Drawing.Point(12, 62);
		this._LabelMeter.Name = "_LabelMeter";
		this._LabelMeter.Text = "Meter:";
		this._LabelMeter.TabIndex = 2;
		//
		// _MeterApp
		//
		this._MeterApp.Location = new System.Drawing.Point(100, 58);
		this._MeterApp.Name = "_MeterApp";
		this._MeterApp.Size = new System.Drawing.Size(185, 22);
		this._MeterApp.Value = 60;
		this._MeterApp.TabIndex = 3;
		//
		// _LabelMeterLight
		//
		this._LabelMeterLight.AutoSize = true;
		this._LabelMeterLight.Location = new System.Drawing.Point(12, 92);
		this._LabelMeterLight.Name = "_LabelMeterLight";
		this._LabelMeterLight.Text = "Always light:";
		this._LabelMeterLight.TabIndex = 4;
		//
		// _MeterLight
		//
		this._MeterLight.Colors.Theme = Robbiblubber.Spellbook.Controls.Themes.Theme.LIGHT;
		this._MeterLight.Location = new System.Drawing.Point(100, 88);
		this._MeterLight.Name = "_MeterLight";
		this._MeterLight.Size = new System.Drawing.Size(185, 22);
		this._MeterLight.Value = 60;
		this._MeterLight.TabIndex = 5;
		//
		// _LabelPassword
		//
		this._LabelPassword.AutoSize = true;
		this._LabelPassword.Location = new System.Drawing.Point(12, 124);
		this._LabelPassword.Name = "_LabelPassword";
		this._LabelPassword.Text = "&Password:";
		this._LabelPassword.TabIndex = 6;
		//
		// _TextPassword
		//
		this._TextPassword.Location = new System.Drawing.Point(100, 120);
		this._TextPassword.Name = "_TextPassword";
		this._TextPassword.Size = new System.Drawing.Size(185, 23);
		this._TextPassword.UseSystemPasswordChar = true;
		this._TextPassword.TabIndex = 7;
		//
		// _IndicatorPassword
		//
		this._IndicatorPassword.Location = new System.Drawing.Point(100, 152);
		this._IndicatorPassword.Name = "_IndicatorPassword";
		this._IndicatorPassword.Size = new System.Drawing.Size(185, 22);
		this._IndicatorPassword.Source = this._TextPassword;
		this._IndicatorPassword.TabIndex = 8;
		//
		// _GroupControls
		//
		this._GroupControls.Location = new System.Drawing.Point(12, 390);
		this._GroupControls.Name = "_GroupControls";
		this._GroupControls.Size = new System.Drawing.Size(300, 190);
		this._GroupControls.Text = "Controls";
		this._GroupControls.TabIndex = 1;
		//
		// _ColumnRole
		//
		this._ColumnRole.Text = "Color role";
		this._ColumnRole.Width = 220;
		//
		// _ColumnColor
		//
		this._ColumnColor.Text = "Color";
		this._ColumnColor.Width = 110;
		//
		// _ColumnSet
		//
		this._ColumnSet.Text = "Set in theme";
		this._ColumnSet.Width = 110;
		//
		// _ListPalette
		//
		this._ListPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._ListPalette.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this._ColumnRole, this._ColumnColor, this._ColumnSet });
		this._ListPalette.FullRowSelect = true;
		this._ListPalette.HideSelection = false;
		this._ListPalette.Location = new System.Drawing.Point(324, 18);
		this._ListPalette.MultiSelect = false;
		this._ListPalette.Name = "_ListPalette";
		this._ListPalette.Size = new System.Drawing.Size(564, 562);
		this._ListPalette.View = System.Windows.Forms.View.Details;
		this._ListPalette.DoubleClick += new System.EventHandler(this._ListPalette_DoubleClick);
		this._ListPalette.KeyDown += new System.Windows.Forms.KeyEventHandler(this._ListPalette_KeyDown);
		this._ListPalette.TabIndex = 2;
		//
		// _ListLog
		//
		this._ListLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._ListLog.FormattingEnabled = true;
		this._ListLog.IntegralHeight = false;
		this._ListLog.Location = new System.Drawing.Point(12, 590);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(876, 112);
		this._ListLog.TabIndex = 3;
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(900, 714);
		this._GroupTheme.Controls.Add(this._LabelTheme);
		this._GroupTheme.Controls.Add(this._ComboTheme);
		this._GroupTheme.Controls.Add(this._ButtonNew);
		this._GroupTheme.Controls.Add(this._ButtonRemove);
		this._GroupTheme.Controls.Add(this._ButtonAccent);
		this._GroupTheme.Controls.Add(this._ButtonBack);
		this._GroupTheme.Controls.Add(this._ButtonResetColors);
		this._GroupTheme.Controls.Add(this._ButtonBase);
		this._GroupTheme.Controls.Add(this._ButtonExport);
		this._GroupTheme.Controls.Add(this._ButtonImport);
		this._GroupTheme.Controls.Add(this._ButtonSave);
		this._GroupTheme.Controls.Add(this._ButtonLoad);
		this._GroupTheme.Controls.Add(this._ButtonControls);
		this._GroupTheme.Controls.Add(this._ButtonEditor);
		this._GroupTheme.Controls.Add(this._ButtonMessageBox);
		this._GroupTheme.Controls.Add(this._LabelInfo);
		this._GroupControls.Controls.Add(this._LabelCombo);
		this._GroupControls.Controls.Add(this._ComboSample);
		this._GroupControls.Controls.Add(this._LabelMeter);
		this._GroupControls.Controls.Add(this._MeterApp);
		this._GroupControls.Controls.Add(this._LabelMeterLight);
		this._GroupControls.Controls.Add(this._MeterLight);
		this._GroupControls.Controls.Add(this._LabelPassword);
		this._GroupControls.Controls.Add(this._TextPassword);
		this._GroupControls.Controls.Add(this._IndicatorPassword);
		this.Controls.Add(this._ListLog);
		this.Controls.Add(this._ListPalette);
		this.Controls.Add(this._GroupControls);
		this.Controls.Add(this._GroupTheme);
		this.MinimumSize = new System.Drawing.Size(916, 753);
		this.Name = "MainWindow";
		//
		// _ThemeProvider
		//
		this._ThemeProvider.ContainerControl = this;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Spellbook Themes Sample";
		this._GroupTheme.ResumeLayout(false);
		this._GroupTheme.PerformLayout();
		this._GroupControls.ResumeLayout(false);
		this._GroupControls.PerformLayout();
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.Label _LabelTheme;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboTheme;
	private System.Windows.Forms.Button _ButtonNew;
	private System.Windows.Forms.Button _ButtonRemove;
	private System.Windows.Forms.Button _ButtonAccent;
	private System.Windows.Forms.Button _ButtonBack;
	private System.Windows.Forms.Button _ButtonResetColors;
	private System.Windows.Forms.Button _ButtonBase;
	private System.Windows.Forms.Button _ButtonExport;
	private System.Windows.Forms.Button _ButtonImport;
	private System.Windows.Forms.Button _ButtonSave;
	private System.Windows.Forms.Button _ButtonLoad;
	private System.Windows.Forms.Label _LabelInfo;
	private System.Windows.Forms.Button _ButtonControls;
	private System.Windows.Forms.Button _ButtonEditor;
	private System.Windows.Forms.Button _ButtonMessageBox;
	private Robbiblubber.Spellbook.Controls.Themes.ThemeProvider _ThemeProvider;
	private System.Windows.Forms.GroupBox _GroupTheme;
	private System.Windows.Forms.Label _LabelCombo;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboSample;
	private System.Windows.Forms.Label _LabelMeter;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterApp;
	private System.Windows.Forms.Label _LabelMeterLight;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterLight;
	private System.Windows.Forms.Label _LabelPassword;
	private System.Windows.Forms.TextBox _TextPassword;
	private Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator _IndicatorPassword;
	private System.Windows.Forms.GroupBox _GroupControls;
	private System.Windows.Forms.ColumnHeader _ColumnRole;
	private System.Windows.Forms.ColumnHeader _ColumnColor;
	private System.Windows.Forms.ColumnHeader _ColumnSet;
	private System.Windows.Forms.ListView _ListPalette;
	private System.Windows.Forms.ListBox _ListLog;
}
