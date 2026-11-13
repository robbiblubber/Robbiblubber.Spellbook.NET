namespace Robbiblubber.Spellbook.Controls.Samples.MultiForm;

partial class NoteWindow
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
		this._MenuMain = new System.Windows.Forms.MenuStrip();
		this._MenuFile = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileNew = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuFileStart = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuFileClose = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuWindow = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuWindowNone = new System.Windows.Forms.ToolStripMenuItem();
		this._TextContent = new System.Windows.Forms.TextBox();
		this._StatusMain = new System.Windows.Forms.StatusStrip();
		this._LabelProcess = new System.Windows.Forms.ToolStripStatusLabel();
		this._LabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
		this._MenuMain.SuspendLayout();
		this._StatusMain.SuspendLayout();
		this.SuspendLayout();
		//
		// _MenuMain
		//
		this._MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFile, this._MenuWindow });
		this._MenuMain.Location = new System.Drawing.Point(0, 0);
		this._MenuMain.Name = "_MenuMain";
		this._MenuMain.Size = new System.Drawing.Size(640, 24);
		this._MenuMain.TabIndex = 0;
		//
		// _MenuFile
		//
		this._MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFileNew, this._MenuFileOpen, this._MenuFileSep1, this._MenuFileStart, this._MenuFileSep2, this._MenuFileClose, this._MenuFileExit });
		this._MenuFile.Name = "_MenuFile";
		this._MenuFile.Size = new System.Drawing.Size(37, 20);
		this._MenuFile.Text = "&File";
		//
		// _MenuFileNew
		//
		this._MenuFileNew.Name = "_MenuFileNew";
		this._MenuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
		this._MenuFileNew.Size = new System.Drawing.Size(240, 22);
		this._MenuFileNew.Text = "&New Window";
		this._MenuFileNew.Click += new System.EventHandler(this._MenuFileNew_Click);
		//
		// _MenuFileOpen
		//
		this._MenuFileOpen.Name = "_MenuFileOpen";
		this._MenuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
		this._MenuFileOpen.Size = new System.Drawing.Size(240, 22);
		this._MenuFileOpen.Text = "&Open...";
		this._MenuFileOpen.Click += new System.EventHandler(this._MenuFileOpen_Click);
		//
		// _MenuFileSep1
		//
		this._MenuFileSep1.Name = "_MenuFileSep1";
		this._MenuFileSep1.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuFileStart
		//
		this._MenuFileStart.Name = "_MenuFileStart";
		this._MenuFileStart.Size = new System.Drawing.Size(240, 22);
		this._MenuFileStart.Text = "&Start Further Instance";
		this._MenuFileStart.Click += new System.EventHandler(this._MenuFileStart_Click);
		//
		// _MenuFileSep2
		//
		this._MenuFileSep2.Name = "_MenuFileSep2";
		this._MenuFileSep2.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuFileClose
		//
		this._MenuFileClose.Name = "_MenuFileClose";
		this._MenuFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W));
		this._MenuFileClose.Size = new System.Drawing.Size(240, 22);
		this._MenuFileClose.Text = "&Close";
		this._MenuFileClose.Click += new System.EventHandler(this._MenuFileClose_Click);
		//
		// _MenuFileExit
		//
		this._MenuFileExit.Name = "_MenuFileExit";
		this._MenuFileExit.Size = new System.Drawing.Size(240, 22);
		this._MenuFileExit.Text = "E&xit (all windows)";
		this._MenuFileExit.Click += new System.EventHandler(this._MenuFileExit_Click);
		//
		// _MenuWindow
		//
		this._MenuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuWindowNone });
		this._MenuWindow.Name = "_MenuWindow";
		this._MenuWindow.Size = new System.Drawing.Size(63, 20);
		this._MenuWindow.Text = "&Window";
		this._MenuWindow.DropDownOpening += new System.EventHandler(this._MenuWindow_DropDownOpening);
		//
		// _MenuWindowNone
		//
		this._MenuWindowNone.Enabled = false;
		this._MenuWindowNone.Name = "_MenuWindowNone";
		this._MenuWindowNone.Size = new System.Drawing.Size(180, 22);
		this._MenuWindowNone.Text = "(no windows)";
		//
		// _TextContent
		//
		this._TextContent.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextContent.Location = new System.Drawing.Point(0, 24);
		this._TextContent.Multiline = true;
		this._TextContent.Name = "_TextContent";
		this._TextContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
		this._TextContent.Size = new System.Drawing.Size(640, 334);
		this._TextContent.TabIndex = 1;
		this._TextContent.WordWrap = false;
		//
		// _StatusMain
		//
		this._StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._LabelProcess, this._LabelInfo });
		this._StatusMain.Location = new System.Drawing.Point(0, 358);
		this._StatusMain.Name = "_StatusMain";
		this._StatusMain.Size = new System.Drawing.Size(640, 22);
		this._StatusMain.TabIndex = 2;
		//
		// _LabelProcess
		//
		this._LabelProcess.Name = "_LabelProcess";
		this._LabelProcess.Size = new System.Drawing.Size(120, 17);
		this._LabelProcess.Text = "Process";
		//
		// _LabelInfo
		//
		this._LabelInfo.Name = "_LabelInfo";
		this._LabelInfo.Size = new System.Drawing.Size(505, 17);
		this._LabelInfo.Spring = true;
		this._LabelInfo.Text = "";
		this._LabelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		//
		// NoteWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(640, 380);
		this.Controls.Add(this._TextContent);
		this.Controls.Add(this._StatusMain);
		this.Controls.Add(this._MenuMain);
		this.MainMenuStrip = this._MenuMain;
		this.Name = "NoteWindow";
		this.Text = "Multi Form Sample";
		this.Activated += new System.EventHandler(this._NoteWindow_Activated);
		this._MenuMain.ResumeLayout(false);
		this._MenuMain.PerformLayout();
		this._StatusMain.ResumeLayout(false);
		this._StatusMain.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.MenuStrip _MenuMain;
	private System.Windows.Forms.ToolStripMenuItem _MenuFile;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileNew;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileOpen;
	private System.Windows.Forms.ToolStripSeparator _MenuFileSep1;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileStart;
	private System.Windows.Forms.ToolStripSeparator _MenuFileSep2;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileClose;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileExit;
	private System.Windows.Forms.ToolStripMenuItem _MenuWindow;
	private System.Windows.Forms.ToolStripMenuItem _MenuWindowNone;
	private System.Windows.Forms.TextBox _TextContent;
	private System.Windows.Forms.StatusStrip _StatusMain;
	private System.Windows.Forms.ToolStripStatusLabel _LabelProcess;
	private System.Windows.Forms.ToolStripStatusLabel _LabelInfo;
}
