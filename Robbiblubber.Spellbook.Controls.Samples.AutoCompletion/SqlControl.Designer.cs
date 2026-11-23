namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

partial class SqlControl
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

	#region Component Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this._LabelIntro = new System.Windows.Forms.Label();
		this._PanelOptions = new System.Windows.Forms.Panel();
		this._LabelLatency = new System.Windows.Forms.Label();
		this._TrackLatency = new System.Windows.Forms.TrackBar();
		this._ChkThrow = new System.Windows.Forms.CheckBox();
		this._ChkHighlight = new System.Windows.Forms.CheckBox();
		this._ChkSuspend = new System.Windows.Forms.CheckBox();
		this._ChkUsage = new System.Windows.Forms.CheckBox();
		this._ButtonExportUsage = new System.Windows.Forms.Button();
		this._ButtonImportUsage = new System.Windows.Forms.Button();
		this._ButtonClearUsage = new System.Windows.Forms.Button();
		this._TextQuery = new System.Windows.Forms.RichTextBox();
		this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
		this._Ilist = new System.Windows.Forms.ImageList(this.components);
		this._PanelOptions.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._TrackLatency)).BeginInit();
		this.SuspendLayout();
		//
		// _LabelIntro
		//
		this._LabelIntro.Dock = System.Windows.Forms.DockStyle.Top;
		this._LabelIntro.Location = new System.Drawing.Point(0, 0);
		this._LabelIntro.Name = "_LabelIntro";
		this._LabelIntro.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
		this._LabelIntro.Size = new System.Drawing.Size(760, 64);
		this._LabelIntro.TabIndex = 0;
		this._LabelIntro.Text = "Context-sensitive SQL: tables after FROM/JOIN, columns after \"alias.\", columns + functions + keywords in SELECT/WHERE/ON, keywords elsewhere. \"[\" lists bracketed table names (custom replace range). Functions place the caret between the parentheses. Use the latency slider to test the loading indicator and cancellation.";
		//
		// _PanelOptions
		//
		this._PanelOptions.Controls.Add(this._LabelLatency);
		this._PanelOptions.Controls.Add(this._TrackLatency);
		this._PanelOptions.Controls.Add(this._ChkThrow);
		this._PanelOptions.Controls.Add(this._ChkHighlight);
		this._PanelOptions.Controls.Add(this._ChkSuspend);
		this._PanelOptions.Controls.Add(this._ChkUsage);
		this._PanelOptions.Controls.Add(this._ButtonExportUsage);
		this._PanelOptions.Controls.Add(this._ButtonImportUsage);
		this._PanelOptions.Controls.Add(this._ButtonClearUsage);
		this._PanelOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
		this._PanelOptions.Location = new System.Drawing.Point(0, 328);
		this._PanelOptions.Name = "_PanelOptions";
		this._PanelOptions.Size = new System.Drawing.Size(760, 92);
		this._PanelOptions.TabIndex = 2;
		//
		// _LabelLatency
		//
		this._LabelLatency.AutoSize = true;
		this._LabelLatency.Location = new System.Drawing.Point(12, 4);
		this._LabelLatency.Name = "_LabelLatency";
		this._LabelLatency.Size = new System.Drawing.Size(150, 15);
		this._LabelLatency.TabIndex = 0;
		this._LabelLatency.Text = "Simulated latency: 0 ms";
		//
		// _TrackLatency
		//
		this._TrackLatency.LargeChange = 500;
		this._TrackLatency.Location = new System.Drawing.Point(12, 22);
		this._TrackLatency.Maximum = 3000;
		this._TrackLatency.Name = "_TrackLatency";
		this._TrackLatency.Size = new System.Drawing.Size(300, 45);
		this._TrackLatency.SmallChange = 100;
		this._TrackLatency.TabIndex = 1;
		this._TrackLatency.TickFrequency = 250;
		this._TrackLatency.ValueChanged += new System.EventHandler(this._TrackLatency_ValueChanged);
		//
		// _ChkThrow
		//
		this._ChkThrow.AutoSize = true;
		this._ChkThrow.Location = new System.Drawing.Point(340, 26);
		this._ChkThrow.Name = "_ChkThrow";
		this._ChkThrow.Size = new System.Drawing.Size(180, 19);
		this._ChkThrow.TabIndex = 2;
		this._ChkThrow.Text = "Source throws an &exception";
		this._ChkThrow.UseVisualStyleBackColor = true;
		this._ChkThrow.CheckedChanged += new System.EventHandler(this._ChkThrow_CheckedChanged);
		// 
		// _ChkHighlight
		// 
		this._ChkHighlight.AutoSize = true;
		this._ChkHighlight.Location = new System.Drawing.Point(12, 62);
		this._ChkHighlight.Name = "_ChkHighlight";
		this._ChkHighlight.Size = new System.Drawing.Size(140, 19);
		this._ChkHighlight.TabIndex = 3;
		this._ChkHighlight.Text = "Keyword &highlighting";
		this._ChkHighlight.UseVisualStyleBackColor = true;
		this._ChkHighlight.CheckedChanged += new System.EventHandler(this._ChkHighlight_CheckedChanged);
		// 
		// _ChkSuspend
		// 
		this._ChkSuspend.AutoSize = true;
		this._ChkSuspend.Checked = true;
		this._ChkSuspend.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkSuspend.Location = new System.Drawing.Point(160, 62);
		this._ChkSuspend.Name = "_ChkSuspend";
		this._ChkSuspend.Size = new System.Drawing.Size(140, 19);
		this._ChkSuspend.TabIndex = 4;
		this._ChkSuspend.Text = "Use &SuspendTracking";
		this._ChkSuspend.UseVisualStyleBackColor = true;
		// 
		// _ChkUsage
		// 
		this._ChkUsage.AutoSize = true;
		this._ChkUsage.Checked = true;
		this._ChkUsage.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkUsage.Location = new System.Drawing.Point(312, 62);
		this._ChkUsage.Name = "_ChkUsage";
		this._ChkUsage.Size = new System.Drawing.Size(110, 19);
		this._ChkUsage.TabIndex = 5;
		this._ChkUsage.Text = "&Usage ranking";
		this._ChkUsage.UseVisualStyleBackColor = true;
		this._ChkUsage.CheckedChanged += new System.EventHandler(this._ChkUsage_CheckedChanged);
		// 
		// _ButtonExportUsage
		// 
		this._ButtonExportUsage.Location = new System.Drawing.Point(436, 58);
		this._ButtonExportUsage.Name = "_ButtonExportUsage";
		this._ButtonExportUsage.Size = new System.Drawing.Size(100, 27);
		this._ButtonExportUsage.TabIndex = 6;
		this._ButtonExportUsage.Text = "E&xport usage";
		this._ButtonExportUsage.UseVisualStyleBackColor = true;
		this._ButtonExportUsage.Click += new System.EventHandler(this._ButtonExportUsage_Click);
		// 
		// _ButtonImportUsage
		// 
		this._ButtonImportUsage.Location = new System.Drawing.Point(542, 58);
		this._ButtonImportUsage.Name = "_ButtonImportUsage";
		this._ButtonImportUsage.Size = new System.Drawing.Size(100, 27);
		this._ButtonImportUsage.TabIndex = 7;
		this._ButtonImportUsage.Text = "I&mport usage";
		this._ButtonImportUsage.UseVisualStyleBackColor = true;
		this._ButtonImportUsage.Click += new System.EventHandler(this._ButtonImportUsage_Click);
		// 
		// _ButtonClearUsage
		// 
		this._ButtonClearUsage.Location = new System.Drawing.Point(648, 58);
		this._ButtonClearUsage.Name = "_ButtonClearUsage";
		this._ButtonClearUsage.Size = new System.Drawing.Size(100, 27);
		this._ButtonClearUsage.TabIndex = 8;
		this._ButtonClearUsage.Text = "Clea&r usage";
		this._ButtonClearUsage.UseVisualStyleBackColor = true;
		this._ButtonClearUsage.Click += new System.EventHandler(this._ButtonClearUsage_Click);
		//
		// _TextQuery
		//
		this._TextQuery.AcceptsTab = true;
		this._AutoCompletion.SetAutoCompletionEnabled(this._TextQuery, true);
		this._AutoCompletion.SetTriggerCharacters(this._TextQuery, ".[");
		this._TextQuery.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextQuery.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._TextQuery.Location = new System.Drawing.Point(0, 64);
		this._TextQuery.Name = "_TextQuery";
		this._TextQuery.Size = new System.Drawing.Size(760, 264);
		this._TextQuery.TabIndex = 1;
		this._TextQuery.Text = "";
		this._TextQuery.WordWrap = false;
		//
		// _AutoCompletion
		//
		this._AutoCompletion.ImageList = this._Ilist;
		//
		// _Ilist
		//
		this._Ilist.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		this._Ilist.ImageSize = new System.Drawing.Size(16, 16);
		this._Ilist.TransparentColor = System.Drawing.Color.Transparent;
		//
		// SqlControl
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._TextQuery);
		this.Controls.Add(this._PanelOptions);
		this.Controls.Add(this._LabelIntro);
		this.Name = "SqlControl";
		this.Size = new System.Drawing.Size(760, 420);
		this._PanelOptions.ResumeLayout(false);
		this._PanelOptions.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._TrackLatency)).EndInit();
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.Label _LabelIntro;
	private System.Windows.Forms.Panel _PanelOptions;
	private System.Windows.Forms.Label _LabelLatency;
	private System.Windows.Forms.TrackBar _TrackLatency;
	private System.Windows.Forms.CheckBox _ChkThrow;
	private System.Windows.Forms.CheckBox _ChkHighlight;
	private System.Windows.Forms.CheckBox _ChkSuspend;
	private System.Windows.Forms.CheckBox _ChkUsage;
	private System.Windows.Forms.Button _ButtonExportUsage;
	private System.Windows.Forms.Button _ButtonImportUsage;
	private System.Windows.Forms.Button _ButtonClearUsage;
	private System.Windows.Forms.RichTextBox _TextQuery;
	private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
	private System.Windows.Forms.ImageList _Ilist;
}
