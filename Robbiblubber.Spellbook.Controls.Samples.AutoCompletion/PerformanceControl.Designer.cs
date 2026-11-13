namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

partial class PerformanceControl
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
        this._LabelCount = new System.Windows.Forms.Label();
        this._NumCount = new System.Windows.Forms.NumericUpDown();
        this._ButtonGenerate = new System.Windows.Forms.Button();
        this._LabelGenerated = new System.Windows.Forms.Label();
        this._LabelSearch = new System.Windows.Forms.Label();
        this._TextSearch = new System.Windows.Forms.TextBox();
        this._LabelKeystroke = new System.Windows.Forms.Label();
        this._LabelPopulate = new System.Windows.Forms.Label();
        this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
        this._Ilist = new System.Windows.Forms.ImageList(this.components);
        ((System.ComponentModel.ISupportInitialize)(this._NumCount)).BeginInit();
        this.SuspendLayout();
        //
        // _LabelIntro
        //
        this._LabelIntro.Dock = System.Windows.Forms.DockStyle.Top;
        this._LabelIntro.Location = new System.Drawing.Point(0, 0);
        this._LabelIntro.Name = "_LabelIntro";
        this._LabelIntro.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
        this._LabelIntro.Size = new System.Drawing.Size(760, 48);
        this._LabelIntro.TabIndex = 0;
        this._LabelIntro.Text = "Large lists with FUZZY matching and no typing delay. Generate up to 500,000 identifiers and watch the timings below while typing (try \"cuorhi\" or \"InvTax\").";
        //
        // _LabelCount
        //
        this._LabelCount.AutoSize = true;
        this._LabelCount.Location = new System.Drawing.Point(12, 60);
        this._LabelCount.Name = "_LabelCount";
        this._LabelCount.Size = new System.Drawing.Size(98, 15);
        this._LabelCount.TabIndex = 1;
        this._LabelCount.Text = "Number of items:";
        //
        // _NumCount
        //
        this._NumCount.Increment = new decimal(new int[] { 10000, 0, 0, 0 });
        this._NumCount.Location = new System.Drawing.Point(12, 80);
        this._NumCount.Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
        this._NumCount.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        this._NumCount.Name = "_NumCount";
        this._NumCount.Size = new System.Drawing.Size(120, 23);
        this._NumCount.TabIndex = 2;
        this._NumCount.ThousandsSeparator = true;
        this._NumCount.Value = new decimal(new int[] { 50000, 0, 0, 0 });
        //
        // _ButtonGenerate
        //
        this._ButtonGenerate.Location = new System.Drawing.Point(140, 78);
        this._ButtonGenerate.Name = "_ButtonGenerate";
        this._ButtonGenerate.Size = new System.Drawing.Size(100, 27);
        this._ButtonGenerate.TabIndex = 3;
        this._ButtonGenerate.Text = "&Generate";
        this._ButtonGenerate.UseVisualStyleBackColor = true;
        this._ButtonGenerate.Click += new System.EventHandler(this._ButtonGenerate_Click);
        //
        // _LabelGenerated
        //
        this._LabelGenerated.AutoSize = true;
        this._LabelGenerated.Location = new System.Drawing.Point(252, 84);
        this._LabelGenerated.Name = "_LabelGenerated";
        this._LabelGenerated.Size = new System.Drawing.Size(0, 15);
        this._LabelGenerated.TabIndex = 4;
        //
        // _LabelSearch
        //
        this._LabelSearch.AutoSize = true;
        this._LabelSearch.Location = new System.Drawing.Point(12, 120);
        this._LabelSearch.Name = "_LabelSearch";
        this._LabelSearch.Size = new System.Drawing.Size(45, 15);
        this._LabelSearch.TabIndex = 5;
        this._LabelSearch.Text = "&Search:";
        //
        // _TextSearch
        //
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextSearch, true);
        this._TextSearch.Location = new System.Drawing.Point(12, 140);
        this._TextSearch.Name = "_TextSearch";
        this._TextSearch.Size = new System.Drawing.Size(400, 23);
        this._TextSearch.TabIndex = 6;
        //
        // _LabelKeystroke
        //
        this._LabelKeystroke.AutoSize = true;
        this._LabelKeystroke.Location = new System.Drawing.Point(12, 180);
        this._LabelKeystroke.Name = "_LabelKeystroke";
        this._LabelKeystroke.Size = new System.Drawing.Size(0, 15);
        this._LabelKeystroke.TabIndex = 7;
        //
        // _LabelPopulate
        //
        this._LabelPopulate.AutoSize = true;
        this._LabelPopulate.Location = new System.Drawing.Point(12, 204);
        this._LabelPopulate.Name = "_LabelPopulate";
        this._LabelPopulate.Size = new System.Drawing.Size(0, 15);
        this._LabelPopulate.TabIndex = 8;
        //
        // _AutoCompletion
        //
        this._AutoCompletion.Delay = 0;
        this._AutoCompletion.ImageList = this._Ilist;
        this._AutoCompletion.MatchMode = Robbiblubber.Spellbook.Controls.ListMatchMode.FUZZY;
        this._AutoCompletion.Populating += new System.EventHandler<Robbiblubber.Spellbook.Controls.AutoCompletionPopulatingEventArgs>(this._AutoCompletion_Populating);
        //
        // _Ilist
        //
        this._Ilist.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
        this._Ilist.ImageSize = new System.Drawing.Size(16, 16);
        this._Ilist.TransparentColor = System.Drawing.Color.Transparent;
        //
        // PerformanceControl
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this._LabelPopulate);
        this.Controls.Add(this._LabelKeystroke);
        this.Controls.Add(this._TextSearch);
        this.Controls.Add(this._LabelSearch);
        this.Controls.Add(this._LabelGenerated);
        this.Controls.Add(this._ButtonGenerate);
        this.Controls.Add(this._NumCount);
        this.Controls.Add(this._LabelCount);
        this.Controls.Add(this._LabelIntro);
        this.Name = "PerformanceControl";
        this.Size = new System.Drawing.Size(760, 420);
        ((System.ComponentModel.ISupportInitialize)(this._NumCount)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label _LabelIntro;
    private System.Windows.Forms.Label _LabelCount;
    private System.Windows.Forms.NumericUpDown _NumCount;
    private System.Windows.Forms.Button _ButtonGenerate;
    private System.Windows.Forms.Label _LabelGenerated;
    private System.Windows.Forms.Label _LabelSearch;
    private System.Windows.Forms.TextBox _TextSearch;
    private System.Windows.Forms.Label _LabelKeystroke;
    private System.Windows.Forms.Label _LabelPopulate;
    private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
    private System.Windows.Forms.ImageList _Ilist;
}
