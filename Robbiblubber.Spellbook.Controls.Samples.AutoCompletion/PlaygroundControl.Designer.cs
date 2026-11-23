namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

partial class PlaygroundControl
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
		this._TextEditor = new System.Windows.Forms.RichTextBox();
		this._PanelOverrides = new System.Windows.Forms.Panel();
		this._LabelMatchMode = new System.Windows.Forms.Label();
		this._CboMatchMode = new System.Windows.Forms.ComboBox();
		this._LabelFilterMode = new System.Windows.Forms.Label();
		this._CboFilterMode = new System.Windows.Forms.ComboBox();
		this._LabelCaseSensitive = new System.Windows.Forms.Label();
		this._CboCaseSensitive = new System.Windows.Forms.ComboBox();
		this._ChkDark = new System.Windows.Forms.CheckBox();
		this._GridProvider = new System.Windows.Forms.PropertyGrid();
		this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
		this._Ilist = new System.Windows.Forms.ImageList(this.components);
		((System.ComponentModel.ISupportInitialize)(this._Split)).BeginInit();
		this._Split.Panel1.SuspendLayout();
		this._Split.Panel2.SuspendLayout();
		this._Split.SuspendLayout();
		this._PanelOverrides.SuspendLayout();
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
		this._LabelIntro.Text = "Type .NET type names (try \"strbu\" with FUZZY, \"Reader\" with CONTAINS). Provider settings on the right apply immediately; the combo boxes below set the per-control overrides. The list adjusts its width and can be resized at the grip.";
		//
		// _Split
		//
		this._Split.Dock = System.Windows.Forms.DockStyle.Fill;
		this._Split.Location = new System.Drawing.Point(0, 48);
		this._Split.Name = "_Split";
		//
		// _Split.Panel1
		//
		this._Split.Panel1.Controls.Add(this._TextEditor);
		this._Split.Panel1.Controls.Add(this._PanelOverrides);
		//
		// _Split.Panel2
		//
		this._Split.Panel2.Controls.Add(this._GridProvider);
		this._Split.Size = new System.Drawing.Size(760, 372);
		this._Split.SplitterDistance = 460;
		this._Split.TabIndex = 1;
		//
		// _TextEditor
		//
		this._TextEditor.AcceptsTab = true;
		this._AutoCompletion.SetAutoCompletionEnabled(this._TextEditor, true);
		this._TextEditor.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextEditor.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._TextEditor.Location = new System.Drawing.Point(0, 0);
		this._TextEditor.Name = "_TextEditor";
		this._TextEditor.Size = new System.Drawing.Size(460, 276);
		this._TextEditor.TabIndex = 0;
		this._TextEditor.Text = "";
		//
		// _PanelOverrides
		//
		this._PanelOverrides.Controls.Add(this._LabelMatchMode);
		this._PanelOverrides.Controls.Add(this._CboMatchMode);
		this._PanelOverrides.Controls.Add(this._LabelFilterMode);
		this._PanelOverrides.Controls.Add(this._CboFilterMode);
		this._PanelOverrides.Controls.Add(this._LabelCaseSensitive);
		this._PanelOverrides.Controls.Add(this._CboCaseSensitive);
		this._PanelOverrides.Controls.Add(this._ChkDark);
		this._PanelOverrides.Dock = System.Windows.Forms.DockStyle.Bottom;
		this._PanelOverrides.Location = new System.Drawing.Point(0, 276);
		this._PanelOverrides.Name = "_PanelOverrides";
		this._PanelOverrides.Size = new System.Drawing.Size(460, 96);
		this._PanelOverrides.TabIndex = 1;
		//
		// _LabelMatchMode
		//
		this._LabelMatchMode.AutoSize = true;
		this._LabelMatchMode.Location = new System.Drawing.Point(8, 10);
		this._LabelMatchMode.Name = "_LabelMatchMode";
		this._LabelMatchMode.Size = new System.Drawing.Size(74, 15);
		this._LabelMatchMode.TabIndex = 0;
		this._LabelMatchMode.Text = "Match mode:";
		//
		// _CboMatchMode
		//
		this._CboMatchMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._CboMatchMode.Location = new System.Drawing.Point(8, 30);
		this._CboMatchMode.Name = "_CboMatchMode";
		this._CboMatchMode.Size = new System.Drawing.Size(130, 23);
		this._CboMatchMode.TabIndex = 1;
		this._CboMatchMode.SelectedIndexChanged += new System.EventHandler(this._CboMatchMode_SelectedIndexChanged);
		//
		// _LabelFilterMode
		//
		this._LabelFilterMode.AutoSize = true;
		this._LabelFilterMode.Location = new System.Drawing.Point(150, 10);
		this._LabelFilterMode.Name = "_LabelFilterMode";
		this._LabelFilterMode.Size = new System.Drawing.Size(70, 15);
		this._LabelFilterMode.TabIndex = 2;
		this._LabelFilterMode.Text = "Filter mode:";
		//
		// _CboFilterMode
		//
		this._CboFilterMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._CboFilterMode.Location = new System.Drawing.Point(150, 30);
		this._CboFilterMode.Name = "_CboFilterMode";
		this._CboFilterMode.Size = new System.Drawing.Size(130, 23);
		this._CboFilterMode.TabIndex = 3;
		this._CboFilterMode.SelectedIndexChanged += new System.EventHandler(this._CboFilterMode_SelectedIndexChanged);
		//
		// _LabelCaseSensitive
		//
		this._LabelCaseSensitive.AutoSize = true;
		this._LabelCaseSensitive.Location = new System.Drawing.Point(292, 10);
		this._LabelCaseSensitive.Name = "_LabelCaseSensitive";
		this._LabelCaseSensitive.Size = new System.Drawing.Size(84, 15);
		this._LabelCaseSensitive.TabIndex = 4;
		this._LabelCaseSensitive.Text = "Case sensitive:";
		//
		// _CboCaseSensitive
		//
		this._CboCaseSensitive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._CboCaseSensitive.Location = new System.Drawing.Point(292, 30);
		this._CboCaseSensitive.Name = "_CboCaseSensitive";
		this._CboCaseSensitive.Size = new System.Drawing.Size(130, 23);
		this._CboCaseSensitive.TabIndex = 5;
		this._CboCaseSensitive.SelectedIndexChanged += new System.EventHandler(this._CboCaseSensitive_SelectedIndexChanged);
		// 
		// _ChkDark
		// 
		this._ChkDark.AutoSize = true;
		this._ChkDark.Location = new System.Drawing.Point(8, 66);
		this._ChkDark.Name = "_ChkDark";
		this._ChkDark.Size = new System.Drawing.Size(84, 19);
		this._ChkDark.TabIndex = 6;
		this._ChkDark.Text = "&Dark theme";
		this._ChkDark.UseVisualStyleBackColor = true;
		this._ChkDark.CheckedChanged += new System.EventHandler(this._ChkDark_CheckedChanged);
		//
		// _GridProvider
		//
		this._GridProvider.Dock = System.Windows.Forms.DockStyle.Fill;
		this._GridProvider.Location = new System.Drawing.Point(0, 0);
		this._GridProvider.Name = "_GridProvider";
		this._GridProvider.Size = new System.Drawing.Size(296, 372);
		this._GridProvider.TabIndex = 0;
		this._GridProvider.ToolbarVisible = false;
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
		// PlaygroundControl
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._Split);
		this.Controls.Add(this._LabelIntro);
		this.Name = "PlaygroundControl";
		this.Size = new System.Drawing.Size(760, 420);
		this._Split.Panel1.ResumeLayout(false);
		this._Split.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._Split)).EndInit();
		this._Split.ResumeLayout(false);
		this._PanelOverrides.ResumeLayout(false);
		this._PanelOverrides.PerformLayout();
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.Label _LabelIntro;
	private System.Windows.Forms.SplitContainer _Split;
	private System.Windows.Forms.RichTextBox _TextEditor;
	private System.Windows.Forms.Panel _PanelOverrides;
	private System.Windows.Forms.Label _LabelMatchMode;
	private System.Windows.Forms.ComboBox _CboMatchMode;
	private System.Windows.Forms.Label _LabelFilterMode;
	private System.Windows.Forms.ComboBox _CboFilterMode;
	private System.Windows.Forms.Label _LabelCaseSensitive;
	private System.Windows.Forms.ComboBox _CboCaseSensitive;
	private System.Windows.Forms.CheckBox _ChkDark;
	private System.Windows.Forms.PropertyGrid _GridProvider;
	private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
	private System.Windows.Forms.ImageList _Ilist;
}
