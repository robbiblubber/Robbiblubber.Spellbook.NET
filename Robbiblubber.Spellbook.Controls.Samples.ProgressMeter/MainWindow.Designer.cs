namespace Robbiblubber.Spellbook.Controls.Samples.ProgressMeter;

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
		this._LabelMain = new System.Windows.Forms.Label();
		this._MeterMain = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelRange = new System.Windows.Forms.Label();
		this._MeterRange = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelMarquee = new System.Windows.Forms.Label();
		this._MeterMarquee = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelThin = new System.Windows.Forms.Label();
		this._MeterThin = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._LabelVertical = new System.Windows.Forms.Label();
		this._MeterVertical1 = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._MeterVertical2 = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._MeterVertical3 = new Robbiblubber.Spellbook.Controls.ProgressMeter();
		this._GroupMeters = new System.Windows.Forms.GroupBox();
		this._LabelValue = new System.Windows.Forms.Label();
		this._TrackValue = new System.Windows.Forms.TrackBar();
		this._LabelScheme = new System.Windows.Forms.Label();
		this._ComboScheme = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._LabelTextStyle = new System.Windows.Forms.Label();
		this._ComboTextStyle = new Robbiblubber.Spellbook.Controls.ThemedComboBox();
		this._ChkEnabled = new System.Windows.Forms.CheckBox();
		this._ChkBorder = new System.Windows.Forms.CheckBox();
		this._ChkGreen = new System.Windows.Forms.CheckBox();
		this._ChkFont = new System.Windows.Forms.CheckBox();
		this._ButtonStep = new System.Windows.Forms.Button();
		this._ButtonDecrement = new System.Windows.Forms.Button();
		this._ButtonSimulate = new System.Windows.Forms.Button();
		this._GroupOptions = new System.Windows.Forms.GroupBox();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._TimerWork = new System.Windows.Forms.Timer(this.components);
		this._GroupMeters.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._TrackValue)).BeginInit();
		this._GroupOptions.SuspendLayout();
		this.SuspendLayout();
		//
		// _LabelMain
		//
		this._LabelMain.AutoSize = true;
		this._LabelMain.Location = new System.Drawing.Point(14, 32);
		this._LabelMain.Name = "_LabelMain";
		this._LabelMain.TabIndex = 0;
		this._LabelMain.Text = "Percent:";
		//
		// _MeterMain
		//
		this._MeterMain.TextStyle = Robbiblubber.Spellbook.Controls.ProgressMeterTextStyle.PERCENT;
		this._MeterMain.ValueChanged += new System.EventHandler(this._MeterMain_ValueChanged);
		this._MeterMain.Location = new System.Drawing.Point(150, 28);
		this._MeterMain.Name = "_MeterMain";
		this._MeterMain.Size = new System.Drawing.Size(230, 24);
		this._MeterMain.TabIndex = 1;
		//
		// _LabelRange
		//
		this._LabelRange.AutoSize = true;
		this._LabelRange.Location = new System.Drawing.Point(14, 68);
		this._LabelRange.Name = "_LabelRange";
		this._LabelRange.TabIndex = 2;
		this._LabelRange.Text = "Value, range 50–150:";
		//
		// _MeterRange
		//
		this._MeterRange.Maximum = 150;
		this._MeterRange.Minimum = 50;
		this._MeterRange.TextStyle = Robbiblubber.Spellbook.Controls.ProgressMeterTextStyle.VALUE;
		this._MeterRange.Value = 50;
		this._MeterRange.Location = new System.Drawing.Point(150, 66);
		this._MeterRange.Name = "_MeterRange";
		this._MeterRange.Size = new System.Drawing.Size(230, 20);
		this._MeterRange.TabIndex = 3;
		//
		// _LabelMarquee
		//
		this._LabelMarquee.AutoSize = true;
		this._LabelMarquee.Location = new System.Drawing.Point(14, 102);
		this._LabelMarquee.Name = "_LabelMarquee";
		this._LabelMarquee.TabIndex = 4;
		this._LabelMarquee.Text = "Marquee:";
		//
		// _MeterMarquee
		//
		this._MeterMarquee.Style = Robbiblubber.Spellbook.Controls.ProgressMeterStyle.MARQUEE;
		this._MeterMarquee.Text = "Please wait...";
		this._MeterMarquee.TextStyle = Robbiblubber.Spellbook.Controls.ProgressMeterTextStyle.TEXT;
		this._MeterMarquee.Location = new System.Drawing.Point(150, 100);
		this._MeterMarquee.Name = "_MeterMarquee";
		this._MeterMarquee.Size = new System.Drawing.Size(230, 20);
		this._MeterMarquee.TabIndex = 5;
		//
		// _LabelThin
		//
		this._LabelThin.AutoSize = true;
		this._LabelThin.Location = new System.Drawing.Point(14, 134);
		this._LabelThin.Name = "_LabelThin";
		this._LabelThin.TabIndex = 6;
		this._LabelThin.Text = "Thin, no border:";
		//
		// _MeterThin
		//
		this._MeterThin.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this._MeterThin.Location = new System.Drawing.Point(150, 138);
		this._MeterThin.Name = "_MeterThin";
		this._MeterThin.Size = new System.Drawing.Size(230, 6);
		this._MeterThin.TabIndex = 7;
		//
		// _LabelVertical
		//
		this._LabelVertical.AutoSize = true;
		this._LabelVertical.Location = new System.Drawing.Point(14, 170);
		this._LabelVertical.Name = "_LabelVertical";
		this._LabelVertical.TabIndex = 8;
		this._LabelVertical.Text = "Vertical:";
		//
		// _MeterVertical1
		//
		this._MeterVertical1.Orientation = System.Windows.Forms.Orientation.Vertical;
		this._MeterVertical1.Location = new System.Drawing.Point(150, 170);
		this._MeterVertical1.Name = "_MeterVertical1";
		this._MeterVertical1.Size = new System.Drawing.Size(24, 110);
		this._MeterVertical1.TabIndex = 9;
		//
		// _MeterVertical2
		//
		this._MeterVertical2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this._MeterVertical2.Orientation = System.Windows.Forms.Orientation.Vertical;
		this._MeterVertical2.Location = new System.Drawing.Point(186, 170);
		this._MeterVertical2.Name = "_MeterVertical2";
		this._MeterVertical2.Size = new System.Drawing.Size(24, 110);
		this._MeterVertical2.TabIndex = 10;
		//
		// _MeterVertical3
		//
		this._MeterVertical3.Orientation = System.Windows.Forms.Orientation.Vertical;
		this._MeterVertical3.TextStyle = Robbiblubber.Spellbook.Controls.ProgressMeterTextStyle.PERCENT;
		this._MeterVertical3.Location = new System.Drawing.Point(222, 170);
		this._MeterVertical3.Name = "_MeterVertical3";
		this._MeterVertical3.Size = new System.Drawing.Size(40, 110);
		this._MeterVertical3.TabIndex = 11;
		//
		// _GroupMeters
		//
		this._GroupMeters.Controls.Add(this._LabelMain);
		this._GroupMeters.Controls.Add(this._MeterMain);
		this._GroupMeters.Controls.Add(this._LabelRange);
		this._GroupMeters.Controls.Add(this._MeterRange);
		this._GroupMeters.Controls.Add(this._LabelMarquee);
		this._GroupMeters.Controls.Add(this._MeterMarquee);
		this._GroupMeters.Controls.Add(this._LabelThin);
		this._GroupMeters.Controls.Add(this._MeterThin);
		this._GroupMeters.Controls.Add(this._LabelVertical);
		this._GroupMeters.Controls.Add(this._MeterVertical1);
		this._GroupMeters.Controls.Add(this._MeterVertical2);
		this._GroupMeters.Controls.Add(this._MeterVertical3);
		this._GroupMeters.Location = new System.Drawing.Point(12, 12);
		this._GroupMeters.Name = "_GroupMeters";
		this._GroupMeters.Size = new System.Drawing.Size(400, 300);
		this._GroupMeters.TabIndex = 0;
		this._GroupMeters.TabStop = false;
		this._GroupMeters.Text = "Progress meters";
		//
		// _LabelValue
		//
		this._LabelValue.AutoSize = true;
		this._LabelValue.Location = new System.Drawing.Point(12, 28);
		this._LabelValue.Name = "_LabelValue";
		this._LabelValue.Text = "&Value:";
		this._LabelValue.TabIndex = 0;
		//
		// _TrackValue
		//
		this._TrackValue.Location = new System.Drawing.Point(12, 46);
		this._TrackValue.Maximum = 100;
		this._TrackValue.Name = "_TrackValue";
		this._TrackValue.Size = new System.Drawing.Size(300, 45);
		this._TrackValue.TickFrequency = 10;
		this._TrackValue.ValueChanged += new System.EventHandler(this._TrackValue_ValueChanged);
		this._TrackValue.TabIndex = 1;
		//
		// _LabelScheme
		//
		this._LabelScheme.AutoSize = true;
		this._LabelScheme.Location = new System.Drawing.Point(12, 100);
		this._LabelScheme.Name = "_LabelScheme";
		this._LabelScheme.Text = "Color s&cheme:";
		this._LabelScheme.TabIndex = 2;
		//
		// _ComboScheme
		//
		this._ComboScheme.Location = new System.Drawing.Point(120, 96);
		this._ComboScheme.Name = "_ComboScheme";
		this._ComboScheme.Size = new System.Drawing.Size(190, 24);
		this._ComboScheme.SelectedIndexChanged += new System.EventHandler(this._ComboScheme_SelectedIndexChanged);
		this._ComboScheme.TabIndex = 3;
		//
		// _LabelTextStyle
		//
		this._LabelTextStyle.AutoSize = true;
		this._LabelTextStyle.Location = new System.Drawing.Point(12, 134);
		this._LabelTextStyle.Name = "_LabelTextStyle";
		this._LabelTextStyle.Text = "&Text style (Percent):";
		this._LabelTextStyle.TabIndex = 4;
		//
		// _ComboTextStyle
		//
		this._ComboTextStyle.Location = new System.Drawing.Point(160, 130);
		this._ComboTextStyle.Name = "_ComboTextStyle";
		this._ComboTextStyle.Size = new System.Drawing.Size(150, 24);
		this._ComboTextStyle.SelectedIndexChanged += new System.EventHandler(this._Options_Changed);
		this._ComboTextStyle.TabIndex = 5;
		//
		// _ChkEnabled
		//
		this._ChkEnabled.AutoSize = true;
		this._ChkEnabled.Checked = true;
		this._ChkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkEnabled.Location = new System.Drawing.Point(12, 166);
		this._ChkEnabled.Name = "_ChkEnabled";
		this._ChkEnabled.Text = "&Enabled";
		this._ChkEnabled.UseVisualStyleBackColor = true;
		this._ChkEnabled.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkEnabled.TabIndex = 6;
		//
		// _ChkBorder
		//
		this._ChkBorder.AutoSize = true;
		this._ChkBorder.Checked = true;
		this._ChkBorder.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkBorder.Location = new System.Drawing.Point(110, 166);
		this._ChkBorder.Name = "_ChkBorder";
		this._ChkBorder.Text = "&Border";
		this._ChkBorder.UseVisualStyleBackColor = true;
		this._ChkBorder.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkBorder.TabIndex = 7;
		//
		// _ChkGreen
		//
		this._ChkGreen.AutoSize = true;
		this._ChkGreen.Location = new System.Drawing.Point(12, 192);
		this._ChkGreen.Name = "_ChkGreen";
		this._ChkGreen.Text = "&Green bar";
		this._ChkGreen.UseVisualStyleBackColor = true;
		this._ChkGreen.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkGreen.TabIndex = 8;
		//
		// _ChkFont
		//
		this._ChkFont.AutoSize = true;
		this._ChkFont.Location = new System.Drawing.Point(110, 192);
		this._ChkFont.Name = "_ChkFont";
		this._ChkFont.Text = "&Larger font";
		this._ChkFont.UseVisualStyleBackColor = true;
		this._ChkFont.CheckedChanged += new System.EventHandler(this._Options_Changed);
		this._ChkFont.TabIndex = 9;
		//
		// _ButtonStep
		//
		this._ButtonStep.Location = new System.Drawing.Point(12, 226);
		this._ButtonStep.Name = "_ButtonStep";
		this._ButtonStep.Size = new System.Drawing.Size(145, 27);
		this._ButtonStep.Text = "&Perform step";
		this._ButtonStep.UseVisualStyleBackColor = true;
		this._ButtonStep.Click += new System.EventHandler(this._ButtonStep_Click);
		this._ButtonStep.TabIndex = 10;
		//
		// _ButtonDecrement
		//
		this._ButtonDecrement.Location = new System.Drawing.Point(165, 226);
		this._ButtonDecrement.Name = "_ButtonDecrement";
		this._ButtonDecrement.Size = new System.Drawing.Size(145, 27);
		this._ButtonDecrement.Text = "&Increment −5";
		this._ButtonDecrement.UseVisualStyleBackColor = true;
		this._ButtonDecrement.Click += new System.EventHandler(this._ButtonDecrement_Click);
		this._ButtonDecrement.TabIndex = 11;
		//
		// _ButtonSimulate
		//
		this._ButtonSimulate.Location = new System.Drawing.Point(12, 260);
		this._ButtonSimulate.Name = "_ButtonSimulate";
		this._ButtonSimulate.Size = new System.Drawing.Size(298, 27);
		this._ButtonSimulate.Text = "&Simulate work";
		this._ButtonSimulate.UseVisualStyleBackColor = true;
		this._ButtonSimulate.Click += new System.EventHandler(this._ButtonSimulate_Click);
		this._ButtonSimulate.TabIndex = 12;
		//
		// _GroupOptions
		//
		this._GroupOptions.Controls.Add(this._LabelValue);
		this._GroupOptions.Controls.Add(this._TrackValue);
		this._GroupOptions.Controls.Add(this._LabelScheme);
		this._GroupOptions.Controls.Add(this._ComboScheme);
		this._GroupOptions.Controls.Add(this._LabelTextStyle);
		this._GroupOptions.Controls.Add(this._ComboTextStyle);
		this._GroupOptions.Controls.Add(this._ChkEnabled);
		this._GroupOptions.Controls.Add(this._ChkBorder);
		this._GroupOptions.Controls.Add(this._ChkGreen);
		this._GroupOptions.Controls.Add(this._ChkFont);
		this._GroupOptions.Controls.Add(this._ButtonStep);
		this._GroupOptions.Controls.Add(this._ButtonDecrement);
		this._GroupOptions.Controls.Add(this._ButtonSimulate);
		this._GroupOptions.Location = new System.Drawing.Point(424, 12);
		this._GroupOptions.Name = "_GroupOptions";
		this._GroupOptions.Size = new System.Drawing.Size(324, 300);
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
		this._ListLog.Location = new System.Drawing.Point(12, 324);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(736, 184);
		this._ListLog.TabIndex = 2;
		//
		// _TimerWork
		//
		this._TimerWork.Interval = 30;
		this._TimerWork.Tick += new System.EventHandler(this._TimerWork_Tick);
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(760, 520);
		this.Controls.Add(this._ListLog);
		this.Controls.Add(this._GroupOptions);
		this.Controls.Add(this._GroupMeters);
		this.MinimumSize = new System.Drawing.Size(776, 480);
		this.Name = "MainWindow";
		this.Text = "Spellbook Progress Meter Sample";
		this._GroupOptions.ResumeLayout(false);
		this._GroupOptions.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._TrackValue)).EndInit();
		this._GroupMeters.ResumeLayout(false);
		this._GroupMeters.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label _LabelMain;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterMain;
	private System.Windows.Forms.Label _LabelRange;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterRange;
	private System.Windows.Forms.Label _LabelMarquee;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterMarquee;
	private System.Windows.Forms.Label _LabelThin;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterThin;
	private System.Windows.Forms.Label _LabelVertical;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterVertical1;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterVertical2;
	private Robbiblubber.Spellbook.Controls.ProgressMeter _MeterVertical3;
	private System.Windows.Forms.GroupBox _GroupMeters;
	private System.Windows.Forms.Label _LabelValue;
	private System.Windows.Forms.TrackBar _TrackValue;
	private System.Windows.Forms.Label _LabelScheme;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboScheme;
	private System.Windows.Forms.Label _LabelTextStyle;
	private Robbiblubber.Spellbook.Controls.ThemedComboBox _ComboTextStyle;
	private System.Windows.Forms.CheckBox _ChkEnabled;
	private System.Windows.Forms.CheckBox _ChkBorder;
	private System.Windows.Forms.CheckBox _ChkGreen;
	private System.Windows.Forms.CheckBox _ChkFont;
	private System.Windows.Forms.Button _ButtonStep;
	private System.Windows.Forms.Button _ButtonDecrement;
	private System.Windows.Forms.Button _ButtonSimulate;
	private System.Windows.Forms.GroupBox _GroupOptions;
	private System.Windows.Forms.ListBox _ListLog;
	private System.Windows.Forms.Timer _TimerWork;
}
