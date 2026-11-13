namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

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
		this._ChkLink = new System.Windows.Forms.CheckBox();
		this._ChkHighlight = new System.Windows.Forms.CheckBox();
		this._ChkDark = new System.Windows.Forms.CheckBox();
		this._ChkSyntax = new System.Windows.Forms.CheckBox();
		this._TextQuery = new System.Windows.Forms.RichTextBox();
		this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
		this._SignatureHelp = new Robbiblubber.Spellbook.Controls.SignatureHelpProvider(this.components);
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
		this._LabelIntro.Size = new System.Drawing.Size(860, 64);
		this._LabelIntro.TabIndex = 0;
		this._LabelIntro.Text = "Signature help together with auto completion. Insert a function from the list (e.g. type \"rou\" + Enter): the signature appears. While the list is open, Escape and Up/Down go to the list. Strings and comments (--, /* */) are ignored. dbo.GetCustomerTotal( uses a qualifier-dependent source.";
		//
		// _PanelOptions
		//
		this._PanelOptions.Controls.Add(this._LabelLatency);
		this._PanelOptions.Controls.Add(this._TrackLatency);
		this._PanelOptions.Controls.Add(this._ChkThrow);
		this._PanelOptions.Controls.Add(this._ChkLink);
		this._PanelOptions.Controls.Add(this._ChkHighlight);
		this._PanelOptions.Controls.Add(this._ChkDark);
		this._PanelOptions.Controls.Add(this._ChkSyntax);
		this._PanelOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
		this._PanelOptions.Location = new System.Drawing.Point(0, 328);
		this._PanelOptions.Name = "_PanelOptions";
		this._PanelOptions.Size = new System.Drawing.Size(860, 92);
		this._PanelOptions.TabIndex = 2;
		//
		// _LabelLatency
		//
		this._LabelLatency.AutoSize = true;
		this._LabelLatency.Location = new System.Drawing.Point(12, 4);
		this._LabelLatency.Name = "_LabelLatency";
		this._LabelLatency.Size = new System.Drawing.Size(190, 15);
		this._LabelLatency.TabIndex = 0;
		this._LabelLatency.Text = "Signature source latency: 0 ms";
		//
		// _TrackLatency
		//
		this._TrackLatency.LargeChange = 250;
		this._TrackLatency.Location = new System.Drawing.Point(12, 22);
		this._TrackLatency.Maximum = 2000;
		this._TrackLatency.Name = "_TrackLatency";
		this._TrackLatency.Size = new System.Drawing.Size(300, 45);
		this._TrackLatency.SmallChange = 50;
		this._TrackLatency.TabIndex = 1;
		this._TrackLatency.TickFrequency = 250;
		this._TrackLatency.ValueChanged += new System.EventHandler(this._TrackLatency_ValueChanged);
		//
		// _ChkThrow
		//
		this._ChkThrow.AutoSize = true;
		this._ChkThrow.Location = new System.Drawing.Point(340, 26);
		this._ChkThrow.Name = "_ChkThrow";
		this._ChkThrow.Size = new System.Drawing.Size(200, 19);
		this._ChkThrow.TabIndex = 2;
		this._ChkThrow.Text = "Signature source throws an &exception";
		this._ChkThrow.UseVisualStyleBackColor = true;
		this._ChkThrow.CheckedChanged += new System.EventHandler(this._ChkThrow_CheckedChanged);
		//
		// _ChkLink
		//
		this._ChkLink.AutoSize = true;
		this._ChkLink.Checked = true;
		this._ChkLink.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkLink.Location = new System.Drawing.Point(12, 62);
		this._ChkLink.Name = "_ChkLink";
		this._ChkLink.Size = new System.Drawing.Size(150, 19);
		this._ChkLink.TabIndex = 3;
		this._ChkLink.Text = "&Link auto completion";
		this._ChkLink.UseVisualStyleBackColor = true;
		this._ChkLink.CheckedChanged += new System.EventHandler(this._ChkLink_CheckedChanged);
		//
		// _ChkHighlight
		//
		this._ChkHighlight.AutoSize = true;
		this._ChkHighlight.Location = new System.Drawing.Point(180, 62);
		this._ChkHighlight.Name = "_ChkHighlight";
		this._ChkHighlight.Size = new System.Drawing.Size(140, 19);
		this._ChkHighlight.TabIndex = 4;
		this._ChkHighlight.Text = "Keyword &highlighting";
		this._ChkHighlight.UseVisualStyleBackColor = true;
		this._ChkHighlight.CheckedChanged += new System.EventHandler(this._ChkHighlight_CheckedChanged);
		//
		// _ChkDark
		//
		this._ChkDark.AutoSize = true;
		this._ChkDark.Location = new System.Drawing.Point(340, 62);
		this._ChkDark.Name = "_ChkDark";
		this._ChkDark.Size = new System.Drawing.Size(90, 19);
		this._ChkDark.TabIndex = 5;
		this._ChkDark.Text = "&Dark theme";
		this._ChkDark.UseVisualStyleBackColor = true;
		this._ChkDark.CheckedChanged += new System.EventHandler(this._ChkDark_CheckedChanged);
		//
		// _ChkSyntax
		//
		this._ChkSyntax.AutoSize = true;
		this._ChkSyntax.Checked = true;
		this._ChkSyntax.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkSyntax.Location = new System.Drawing.Point(450, 62);
		this._ChkSyntax.Name = "_ChkSyntax";
		this._ChkSyntax.Size = new System.Drawing.Size(250, 19);
		this._ChkSyntax.TabIndex = 6;
		this._ChkSyntax.Text = "No auto &completion in comments and strings";
		this._ChkSyntax.UseVisualStyleBackColor = true;
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
		this._SignatureHelp.SetSignatureHelpEnabled(this._TextQuery, true);
		this._TextQuery.Size = new System.Drawing.Size(860, 264);
		this._TextQuery.TabIndex = 1;
		this._TextQuery.Text = "";
		this._TextQuery.WordWrap = false;
		//
		// _AutoCompletion
		//
		this._AutoCompletion.ImageList = this._Ilist;
		//
		// _SignatureHelp
		//
		this._SignatureHelp.AutoCompletion = this._AutoCompletion;
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
		this.Size = new System.Drawing.Size(860, 420);
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
	private System.Windows.Forms.CheckBox _ChkLink;
	private System.Windows.Forms.CheckBox _ChkHighlight;
	private System.Windows.Forms.CheckBox _ChkDark;
	private System.Windows.Forms.CheckBox _ChkSyntax;
	private System.Windows.Forms.RichTextBox _TextQuery;
	private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
	private Robbiblubber.Spellbook.Controls.SignatureHelpProvider _SignatureHelp;
	private System.Windows.Forms.ImageList _Ilist;
}
