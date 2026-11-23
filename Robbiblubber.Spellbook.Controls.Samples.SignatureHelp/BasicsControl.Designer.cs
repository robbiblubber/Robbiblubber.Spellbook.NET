namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

partial class BasicsControl
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
		this._Split = new System.Windows.Forms.SplitContainer();
		this._TextCode = new System.Windows.Forms.TextBox();
		this._TextLine = new System.Windows.Forms.TextBox();
		this._GridProvider = new System.Windows.Forms.PropertyGrid();
		this._SignatureHelp = new Robbiblubber.Spellbook.Controls.SignatureHelpProvider(this.components);
		((System.ComponentModel.ISupportInitialize)(this._Split)).BeginInit();
		this._Split.Panel1.SuspendLayout();
		this._Split.Panel2.SuspendLayout();
		this._Split.SuspendLayout();
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
		this._LabelIntro.Text = "Signature help without auto completion. \"(\" and \",\" show the signature of the call at the caret, Ctrl+Shift+Space shows it on demand. Up/Down switch between overloads, Escape closes. Strings and comments are ignored by the parser. The upper box is single-line, the lower box multi-line.";
		//
		// _Split
		//
		this._Split.Dock = System.Windows.Forms.DockStyle.Fill;
		this._Split.Location = new System.Drawing.Point(0, 64);
		this._Split.Name = "_Split";
		//
		// _Split.Panel1
		//
		this._Split.Panel1.Controls.Add(this._TextCode);
		this._Split.Panel1.Controls.Add(this._TextLine);
		this._Split.Panel1.Padding = new System.Windows.Forms.Padding(12, 8, 6, 12);
		//
		// _Split.Panel2
		//
		this._Split.Panel2.Controls.Add(this._GridProvider);
		this._Split.Panel2.Padding = new System.Windows.Forms.Padding(6, 8, 12, 12);
		this._Split.Size = new System.Drawing.Size(860, 356);
		this._Split.SplitterDistance = 560;
		this._Split.TabIndex = 1;
		//
		// _TextCode
		//
		this._TextCode.AcceptsReturn = true;
		this._TextCode.AcceptsTab = true;
		this._SignatureHelp.SetSignatureHelpEnabled(this._TextCode, true);
		this._TextCode.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextCode.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._TextCode.Location = new System.Drawing.Point(12, 39);
		this._TextCode.Multiline = true;
		this._TextCode.Name = "_TextCode";
		this._TextCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
		this._TextCode.Size = new System.Drawing.Size(542, 305);
		this._TextCode.TabIndex = 1;
		this._TextCode.WordWrap = false;
		//
		// _TextLine
		//
		this._SignatureHelp.SetSignatureHelpEnabled(this._TextLine, true);
		this._TextLine.Dock = System.Windows.Forms.DockStyle.Top;
		this._TextLine.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._TextLine.Location = new System.Drawing.Point(12, 8);
		this._TextLine.Name = "_TextLine";
		this._TextLine.Size = new System.Drawing.Size(542, 23);
		this._TextLine.TabIndex = 0;
		this._TextLine.Text = "Max(";
		//
		// _GridProvider
		//
		this._GridProvider.Dock = System.Windows.Forms.DockStyle.Fill;
		this._GridProvider.Location = new System.Drawing.Point(6, 8);
		this._GridProvider.Name = "_GridProvider";
		this._GridProvider.Size = new System.Drawing.Size(278, 336);
		this._GridProvider.TabIndex = 0;
		this._GridProvider.ToolbarVisible = false;
		//
		// BasicsControl
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._Split);
		this.Controls.Add(this._LabelIntro);
		this.Name = "BasicsControl";
		this.Size = new System.Drawing.Size(860, 420);
		this._Split.Panel1.ResumeLayout(false);
		this._Split.Panel1.PerformLayout();
		this._Split.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._Split)).EndInit();
		this._Split.ResumeLayout(false);
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.Label _LabelIntro;
	private System.Windows.Forms.SplitContainer _Split;
	private System.Windows.Forms.TextBox _TextCode;
	private System.Windows.Forms.TextBox _TextLine;
	private System.Windows.Forms.PropertyGrid _GridProvider;
	private Robbiblubber.Spellbook.Controls.SignatureHelpProvider _SignatureHelp;
}
