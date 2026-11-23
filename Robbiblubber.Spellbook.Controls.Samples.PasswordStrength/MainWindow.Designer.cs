namespace Robbiblubber.Spellbook.Controls.Samples.PasswordStrength;

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
		this._LabelPassword = new System.Windows.Forms.Label();
		this._TextPassword = new System.Windows.Forms.TextBox();
		this._ChkShow = new System.Windows.Forms.CheckBox();
		this._LabelSamples = new System.Windows.Forms.Label();
		this._ComboSamples = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._Indicator1 = new Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator();
		this._Indicator2 = new Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator();
		this._LabelRate = new System.Windows.Forms.Label();
		this._PanelBack = new System.Windows.Forms.Panel();
		this._GroupPassword = new System.Windows.Forms.GroupBox();
		this._LabelScheme = new System.Windows.Forms.Label();
		this._ComboScheme = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._ChkEnabled = new System.Windows.Forms.CheckBox();
		this._ChkRounded = new System.Windows.Forms.CheckBox();
		this._ChkPolicy = new System.Windows.Forms.CheckBox();
		this._ChkGerman = new System.Windows.Forms.CheckBox();
		this._ChkFont = new System.Windows.Forms.CheckBox();
		this._LabelSpacing = new System.Windows.Forms.Label();
		this._NumSpacing = new System.Windows.Forms.NumericUpDown();
		this._ChkTransparent = new System.Windows.Forms.CheckBox();
		this._GroupOptions = new System.Windows.Forms.GroupBox();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._PanelBack.SuspendLayout();
		this._GroupPassword.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._NumSpacing)).BeginInit();
		this._GroupOptions.SuspendLayout();
		this.SuspendLayout();
		//
		// _LabelPassword
		//
		this._LabelPassword.AutoSize = true;
		this._LabelPassword.Location = new System.Drawing.Point(14, 30);
		this._LabelPassword.Name = "_LabelPassword";
		this._LabelPassword.Text = "&Password:";
		this._LabelPassword.TabIndex = 0;
		//
		// _TextPassword
		//
		this._TextPassword.Location = new System.Drawing.Point(110, 26);
		this._TextPassword.Name = "_TextPassword";
		this._TextPassword.Size = new System.Drawing.Size(300, 23);
		this._TextPassword.UseSystemPasswordChar = true;
		this._TextPassword.TabIndex = 1;
		//
		// _ChkShow
		//
		this._ChkShow.AutoSize = true;
		this._ChkShow.Location = new System.Drawing.Point(420, 28);
		this._ChkShow.Name = "_ChkShow";
		this._ChkShow.Text = "S&how";
		this._ChkShow.UseVisualStyleBackColor = true;
		this._ChkShow.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkShow.TabIndex = 2;
		//
		// _LabelSamples
		//
		this._LabelSamples.AutoSize = true;
		this._LabelSamples.Location = new System.Drawing.Point(14, 64);
		this._LabelSamples.Name = "_LabelSamples";
		this._LabelSamples.Text = "Sa&mples:";
		this._LabelSamples.TabIndex = 3;
		//
		// _ComboSamples
		//
		this._ComboSamples.Location = new System.Drawing.Point(110, 60);
		this._ComboSamples.Name = "_ComboSamples";
		this._ComboSamples.Size = new System.Drawing.Size(300, 24);
		this._ComboSamples.SelectionChangeCommitted += new System.EventHandler(this._ComboSamples_SelectionChangeCommitted);
		this._ComboSamples.TabIndex = 4;
		//
		// _Indicator1
		//
		this._Indicator1.Location = new System.Drawing.Point(10, 10);
		this._Indicator1.Name = "_Indicator1";
		this._Indicator1.ShowText = true;
		this._Indicator1.Size = new System.Drawing.Size(320, 18);
		this._Indicator1.ValueChanged += new System.EventHandler(this._Indicator1_ValueChanged);
		this._Indicator1.Rating += new System.EventHandler<Robbiblubber.Spellbook.Controls.PasswordRatingEventArgs>(this._Indicator_Rating);
		this._Indicator1.ValueEvaluated += new System.EventHandler(this._Indicator1_ValueEvaluated);
		this._Indicator1.TabIndex = 5;
		//
		// _Indicator2
		//
		this._Indicator2.BoxSpacing = 2;
		this._Indicator2.Location = new System.Drawing.Point(10, 36);
		this._Indicator2.Name = "_Indicator2";
		this._Indicator2.Size = new System.Drawing.Size(220, 16);
		this._Indicator2.UseRoundedCorners = false;
		this._Indicator2.Rating += new System.EventHandler<Robbiblubber.Spellbook.Controls.PasswordRatingEventArgs>(this._Indicator_Rating);
		this._Indicator2.TabIndex = 6;
		//
		// _LabelRate
		//
		this._LabelRate.BackColor = System.Drawing.Color.Transparent;
		this._LabelRate.Location = new System.Drawing.Point(350, 10);
		this._LabelRate.Name = "_LabelRate";
		this._LabelRate.Size = new System.Drawing.Size(230, 44);
		this._LabelRate.Text = "-";
		this._LabelRate.TabIndex = 7;
		//
		// _PanelBack
		//
		this._PanelBack.Controls.Add(this._Indicator1);
		this._PanelBack.Controls.Add(this._Indicator2);
		this._PanelBack.Controls.Add(this._LabelRate);
		this._PanelBack.Location = new System.Drawing.Point(14, 96);
		this._PanelBack.Name = "_PanelBack";
		this._PanelBack.Size = new System.Drawing.Size(588, 62);
		this._PanelBack.TabIndex = 8;
		//
		// _GroupPassword
		//
		this._GroupPassword.Controls.Add(this._LabelPassword);
		this._GroupPassword.Controls.Add(this._TextPassword);
		this._GroupPassword.Controls.Add(this._ChkShow);
		this._GroupPassword.Controls.Add(this._LabelSamples);
		this._GroupPassword.Controls.Add(this._ComboSamples);
		this._GroupPassword.Controls.Add(this._PanelBack);
		this._GroupPassword.Location = new System.Drawing.Point(12, 12);
		this._GroupPassword.Name = "_GroupPassword";
		this._GroupPassword.Size = new System.Drawing.Size(616, 170);
		this._GroupPassword.TabIndex = 0;
		this._GroupPassword.TabStop = false;
		this._GroupPassword.Text = "Password";
		//
		// _LabelScheme
		//
		this._LabelScheme.AutoSize = true;
		this._LabelScheme.Location = new System.Drawing.Point(14, 28);
		this._LabelScheme.Name = "_LabelScheme";
		this._LabelScheme.Text = "Color s&cheme:";
		this._LabelScheme.TabIndex = 9;
		//
		// _ComboScheme
		//
		this._ComboScheme.Location = new System.Drawing.Point(110, 24);
		this._ComboScheme.Name = "_ComboScheme";
		this._ComboScheme.Size = new System.Drawing.Size(150, 24);
		this._ComboScheme.SelectedIndexChanged += new System.EventHandler(this._Options_Changed);
		this._ComboScheme.TabIndex = 10;
		//
		// _ChkEnabled
		//
		this._ChkEnabled.AutoSize = true;
		this._ChkEnabled.Checked = true;
		this._ChkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkEnabled.Location = new System.Drawing.Point(280, 26);
		this._ChkEnabled.Name = "_ChkEnabled";
		this._ChkEnabled.Text = "&Enabled";
		this._ChkEnabled.UseVisualStyleBackColor = true;
		this._ChkEnabled.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkEnabled.TabIndex = 11;
		//
		// _ChkRounded
		//
		this._ChkRounded.AutoSize = true;
		this._ChkRounded.Checked = true;
		this._ChkRounded.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkRounded.Location = new System.Drawing.Point(380, 26);
		this._ChkRounded.Name = "_ChkRounded";
		this._ChkRounded.Text = "&Rounded corners";
		this._ChkRounded.UseVisualStyleBackColor = true;
		this._ChkRounded.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkRounded.TabIndex = 12;
		//
		// _ChkPolicy
		//
		this._ChkPolicy.AutoSize = true;
		this._ChkPolicy.Location = new System.Drawing.Point(14, 56);
		this._ChkPolicy.Name = "_ChkPolicy";
		this._ChkPolicy.Text = "P&olicy: at least 12 characters";
		this._ChkPolicy.UseVisualStyleBackColor = true;
		this._ChkPolicy.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkPolicy.TabIndex = 13;
		//
		// _ChkGerman
		//
		this._ChkGerman.AutoSize = true;
		this._ChkGerman.Location = new System.Drawing.Point(250, 56);
		this._ChkGerman.Name = "_ChkGerman";
		this._ChkGerman.Text = "&German texts";
		this._ChkGerman.UseVisualStyleBackColor = true;
		this._ChkGerman.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkGerman.TabIndex = 14;
		//
		// _ChkFont
		//
		this._ChkFont.AutoSize = true;
		this._ChkFont.Location = new System.Drawing.Point(380, 56);
		this._ChkFont.Name = "_ChkFont";
		this._ChkFont.Text = "&Larger font";
		this._ChkFont.UseVisualStyleBackColor = true;
		this._ChkFont.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkFont.TabIndex = 15;
		//
		// _LabelSpacing
		//
		this._LabelSpacing.AutoSize = true;
		this._LabelSpacing.Location = new System.Drawing.Point(14, 86);
		this._LabelSpacing.Name = "_LabelSpacing";
		this._LabelSpacing.Text = "Box &spacing:";
		this._LabelSpacing.TabIndex = 16;
		//
		// _NumSpacing
		//
		this._NumSpacing.Location = new System.Drawing.Point(110, 82);
		this._NumSpacing.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
		this._NumSpacing.Name = "_NumSpacing";
		this._NumSpacing.Size = new System.Drawing.Size(60, 23);
		this._NumSpacing.Value = new decimal(new int[] { 4, 0, 0, 0 });
		this._NumSpacing.ValueChanged += new System.EventHandler(this._Options_Changed);
		this._NumSpacing.TabIndex = 17;
		//
		// _ChkTransparent
		//
		this._ChkTransparent.AutoSize = true;
		this._ChkTransparent.Location = new System.Drawing.Point(250, 84);
		this._ChkTransparent.Name = "_ChkTransparent";
		this._ChkTransparent.Text = "&Transparent background";
		this._ChkTransparent.UseVisualStyleBackColor = true;
		this._ChkTransparent.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkTransparent.TabIndex = 18;
		//
		// _GroupOptions
		//
		this._GroupOptions.Controls.Add(this._LabelScheme);
		this._GroupOptions.Controls.Add(this._ComboScheme);
		this._GroupOptions.Controls.Add(this._ChkEnabled);
		this._GroupOptions.Controls.Add(this._ChkRounded);
		this._GroupOptions.Controls.Add(this._ChkPolicy);
		this._GroupOptions.Controls.Add(this._ChkGerman);
		this._GroupOptions.Controls.Add(this._ChkFont);
		this._GroupOptions.Controls.Add(this._LabelSpacing);
		this._GroupOptions.Controls.Add(this._NumSpacing);
		this._GroupOptions.Controls.Add(this._ChkTransparent);
		this._GroupOptions.Location = new System.Drawing.Point(12, 190);
		this._GroupOptions.Name = "_GroupOptions";
		this._GroupOptions.Size = new System.Drawing.Size(616, 116);
		this._GroupOptions.TabIndex = 1;
		this._GroupOptions.TabStop = false;
		this._GroupOptions.Text = "Options";
		//
		// _ListLog
		//
		this._ListLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._ListLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._ListLog.HorizontalScrollbar = true;
		this._ListLog.IntegralHeight = false;
		this._ListLog.ItemHeight = 14;
		this._ListLog.Location = new System.Drawing.Point(12, 314);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(616, 134);
		this._ListLog.TabIndex = 2;
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(640, 460);
		this.Controls.Add(this._ListLog);
		this.Controls.Add(this._GroupOptions);
		this.Controls.Add(this._GroupPassword);
		this.MinimumSize = new System.Drawing.Size(656, 420);
		this.Name = "MainWindow";
		this.Text = "Spellbook Password Strength Sample";
		this._GroupOptions.ResumeLayout(false);
		this._GroupOptions.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._NumSpacing)).EndInit();
		this._GroupPassword.ResumeLayout(false);
		this._GroupPassword.PerformLayout();
		this._PanelBack.ResumeLayout(false);
		this._PanelBack.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label _LabelPassword;
	private System.Windows.Forms.TextBox _TextPassword;
	private System.Windows.Forms.CheckBox _ChkShow;
	private System.Windows.Forms.Label _LabelSamples;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboSamples;
	private Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator _Indicator1;
	private Robbiblubber.Spellbook.Controls.PasswordStrengthIndicator _Indicator2;
	private System.Windows.Forms.Label _LabelRate;
	private System.Windows.Forms.Panel _PanelBack;
	private System.Windows.Forms.GroupBox _GroupPassword;
	private System.Windows.Forms.Label _LabelScheme;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboScheme;
	private System.Windows.Forms.CheckBox _ChkEnabled;
	private System.Windows.Forms.CheckBox _ChkRounded;
	private System.Windows.Forms.CheckBox _ChkPolicy;
	private System.Windows.Forms.CheckBox _ChkGerman;
	private System.Windows.Forms.CheckBox _ChkFont;
	private System.Windows.Forms.Label _LabelSpacing;
	private System.Windows.Forms.NumericUpDown _NumSpacing;
	private System.Windows.Forms.CheckBox _ChkTransparent;
	private System.Windows.Forms.GroupBox _GroupOptions;
	private System.Windows.Forms.ListBox _ListLog;
}
