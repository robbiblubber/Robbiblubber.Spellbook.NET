namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

partial class MultiControl
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
        this._LabelColors = new System.Windows.Forms.Label();
        this._TextColors = new System.Windows.Forms.TextBox();
        this._ButtonShow = new System.Windows.Forms.Button();
        this._LabelFruits = new System.Windows.Forms.Label();
        this._TextFruits = new System.Windows.Forms.TextBox();
        this._LabelDecorated = new System.Windows.Forms.Label();
        this._TextDecorated = new System.Windows.Forms.RichTextBox();
        this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
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
        this._LabelIntro.Text = "One provider, three controls. Switch focus while the list is open: only one session can be active. The fruit box appends \", \" on accept (Accepting event), the third box decorates items in the Populating event.";
        //
        // _LabelColors
        //
        this._LabelColors.AutoSize = true;
        this._LabelColors.Location = new System.Drawing.Point(12, 60);
        this._LabelColors.Name = "_LabelColors";
        this._LabelColors.Size = new System.Drawing.Size(200, 15);
        this._LabelColors.TabIndex = 1;
        this._LabelColors.Text = "C&olors (provider-wide source, CONTAINS):";
        //
        // _TextColors
        //
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextColors, true);
        this._AutoCompletion.SetMatchMode(this._TextColors, Robbiblubber.Spellbook.Controls.ListMatchMode.CONTAINS);
        this._TextColors.Location = new System.Drawing.Point(12, 80);
        this._TextColors.Name = "_TextColors";
        this._TextColors.Size = new System.Drawing.Size(320, 23);
        this._TextColors.TabIndex = 2;
        //
        // _ButtonShow
        //
        this._ButtonShow.Location = new System.Drawing.Point(340, 78);
        this._ButtonShow.Name = "_ButtonShow";
        this._ButtonShow.Size = new System.Drawing.Size(160, 27);
        this._ButtonShow.TabIndex = 3;
        this._ButtonShow.Text = "Show&Completion()";
        this._ButtonShow.UseVisualStyleBackColor = true;
        this._ButtonShow.Click += new System.EventHandler(this._ButtonShow_Click);
        //
        // _LabelFruits
        //
        this._LabelFruits.AutoSize = true;
        this._LabelFruits.Location = new System.Drawing.Point(12, 120);
        this._LabelFruits.Name = "_LabelFruits";
        this._LabelFruits.Size = new System.Drawing.Size(220, 15);
        this._LabelFruits.TabIndex = 4;
        this._LabelFruits.Text = "&Fruits (own source, Accepting appends \", \"):";
        //
        // _TextFruits
        //
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextFruits, true);
        this._TextFruits.Location = new System.Drawing.Point(12, 140);
        this._TextFruits.Name = "_TextFruits";
        this._TextFruits.Size = new System.Drawing.Size(488, 23);
        this._TextFruits.TabIndex = 5;
        //
        // _LabelDecorated
        //
        this._LabelDecorated.AutoSize = true;
        this._LabelDecorated.Location = new System.Drawing.Point(12, 180);
        this._LabelDecorated.Name = "_LabelDecorated";
        this._LabelDecorated.Size = new System.Drawing.Size(280, 15);
        this._LabelDecorated.TabIndex = 6;
        this._LabelDecorated.Text = "&Decorated colors (Populating sets image, color, detail):";
        //
        // _TextDecorated
        //
        this._TextDecorated.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        this._AutoCompletion.SetAutoCompletionEnabled(this._TextDecorated, true);
        this._AutoCompletion.SetBoxSize(this._TextDecorated, new System.Drawing.Size(320, 240));
        this._TextDecorated.Location = new System.Drawing.Point(12, 200);
        this._TextDecorated.Name = "_TextDecorated";
        this._TextDecorated.Size = new System.Drawing.Size(736, 208);
        this._TextDecorated.TabIndex = 7;
        this._TextDecorated.Text = "";
        //
        // _AutoCompletion
        //
        this._AutoCompletion.Accepting += new System.EventHandler<Robbiblubber.Spellbook.Controls.AutoCompletionAcceptingEventArgs>(this._AutoCompletion_Accepting);
        this._AutoCompletion.Populating += new System.EventHandler<Robbiblubber.Spellbook.Controls.AutoCompletionPopulatingEventArgs>(this._AutoCompletion_Populating);
        //
        // MultiControl
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this._TextDecorated);
        this.Controls.Add(this._LabelDecorated);
        this.Controls.Add(this._TextFruits);
        this.Controls.Add(this._LabelFruits);
        this.Controls.Add(this._ButtonShow);
        this.Controls.Add(this._TextColors);
        this.Controls.Add(this._LabelColors);
        this.Controls.Add(this._LabelIntro);
        this.Name = "MultiControl";
        this.Size = new System.Drawing.Size(760, 420);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label _LabelIntro;
    private System.Windows.Forms.Label _LabelColors;
    private System.Windows.Forms.TextBox _TextColors;
    private System.Windows.Forms.Button _ButtonShow;
    private System.Windows.Forms.Label _LabelFruits;
    private System.Windows.Forms.TextBox _TextFruits;
    private System.Windows.Forms.Label _LabelDecorated;
    private System.Windows.Forms.RichTextBox _TextDecorated;
    private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
}
