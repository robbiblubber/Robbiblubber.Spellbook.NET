namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

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
        this._LabelCountry = new System.Windows.Forms.Label();
        this._TextCountry = new System.Windows.Forms.TextBox();
        this._LabelLanguage = new System.Windows.Forms.Label();
        this._TextLanguage = new System.Windows.Forms.TextBox();
        this._ButtonDialog = new System.Windows.Forms.Button();
        this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
        this._Ilist = new System.Windows.Forms.ImageList(this.components);
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
        this._LabelIntro.Text = "Minimal setup, done in the designer. The first box uses the provider defaults (prefix matching, filtered list). The second box overrides the filter mode to SELECT: all items stay visible, the best match is selected. Ctrl+Space opens the list at any time.";
        //
        // _LabelCountry
        //
        this._LabelCountry.AutoSize = true;
        this._LabelCountry.Location = new System.Drawing.Point(12, 76);
        this._LabelCountry.Name = "_LabelCountry";
        this._LabelCountry.Size = new System.Drawing.Size(160, 15);
        this._LabelCountry.TabIndex = 1;
        this._LabelCountry.Text = "&Country (prefix, FILTER mode):";
        //
        // _TextCountry
        //
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextCountry, true);
        this._TextCountry.Location = new System.Drawing.Point(12, 96);
        this._TextCountry.Name = "_TextCountry";
        this._TextCountry.Size = new System.Drawing.Size(320, 23);
        this._TextCountry.TabIndex = 2;
        //
        // _LabelLanguage
        //
        this._LabelLanguage.AutoSize = true;
        this._LabelLanguage.Location = new System.Drawing.Point(12, 136);
        this._LabelLanguage.Name = "_LabelLanguage";
        this._LabelLanguage.Size = new System.Drawing.Size(300, 15);
        this._LabelLanguage.TabIndex = 3;
        this._LabelLanguage.Text = "Programming &languages (multiline, SELECT mode, detail, word characters \"_#+-\"):";
        //
        // _TextLanguage
        //
        this._TextLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextLanguage, true);
        this._AutoCompletion.SetFilterMode(this._TextLanguage, Robbiblubber.Spellbook.Controls.ListFilterMode.SELECT);
        this._AutoCompletion.SetWordCharacters(this._TextLanguage, "_#+-");
        this._TextLanguage.Location = new System.Drawing.Point(12, 156);
        this._TextLanguage.Multiline = true;
        this._TextLanguage.Name = "_TextLanguage";
        this._TextLanguage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this._TextLanguage.Size = new System.Drawing.Size(736, 120);
        this._TextLanguage.TabIndex = 4;
        //
        // _ButtonDialog
        //
        this._ButtonDialog.Location = new System.Drawing.Point(12, 292);
        this._ButtonDialog.Name = "_ButtonDialog";
        this._ButtonDialog.Size = new System.Drawing.Size(200, 27);
        this._ButtonDialog.TabIndex = 5;
        this._ButtonDialog.Text = "Open &dialog (Accept/Cancel)...";
        this._ButtonDialog.UseVisualStyleBackColor = true;
        this._ButtonDialog.Click += new System.EventHandler(this._ButtonDialog_Click);
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
        // BasicsControl
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this._ButtonDialog);
        this.Controls.Add(this._TextLanguage);
        this.Controls.Add(this._LabelLanguage);
        this.Controls.Add(this._TextCountry);
        this.Controls.Add(this._LabelCountry);
        this.Controls.Add(this._LabelIntro);
        this.Name = "BasicsControl";
        this.Size = new System.Drawing.Size(760, 420);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label _LabelIntro;
    private System.Windows.Forms.Label _LabelCountry;
    private System.Windows.Forms.TextBox _TextCountry;
    private System.Windows.Forms.Label _LabelLanguage;
    private System.Windows.Forms.TextBox _TextLanguage;
    private System.Windows.Forms.Button _ButtonDialog;
    private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
    private System.Windows.Forms.ImageList _Ilist;
}
