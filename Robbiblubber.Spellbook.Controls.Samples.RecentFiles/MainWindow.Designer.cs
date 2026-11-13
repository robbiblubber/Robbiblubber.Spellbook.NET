namespace Robbiblubber.Spellbook.Controls.Samples.RecentFiles;

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
		this._RecentFiles = new Robbiblubber.Spellbook.Controls.RecentFileList(this.components);
		this._RecentConnections = new Robbiblubber.Spellbook.Controls.Samples.RecentFiles.RecentConnectionList(this.components);
		this._MenuMain = new System.Windows.Forms.MenuStrip();
		this._MenuFile = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuFileRecent = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuFileNewWindow = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileSep3 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuDb = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuDbConnect = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuDbSep1 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuDbRecent = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuDbSep2 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuDbDropStaging = new System.Windows.Forms.ToolStripMenuItem();
		this._PanelOptions = new System.Windows.Forms.Panel();
		this._ChkFullPath = new System.Windows.Forms.CheckBox();
		this._ChkWindowsRecent = new System.Windows.Forms.CheckBox();
		this._ChkNumbers = new System.Windows.Forms.CheckBox();
		this._ChkClear = new System.Windows.Forms.CheckBox();
		this._ChkImages = new System.Windows.Forms.CheckBox();
		this._LabelMax = new System.Windows.Forms.Label();
		this._NumMax = new System.Windows.Forms.NumericUpDown();
		this._SplitMain = new System.Windows.Forms.SplitContainer();
		this._TextContent = new System.Windows.Forms.TextBox();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._MenuMain.SuspendLayout();
		this._PanelOptions.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._NumMax)).BeginInit();
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).BeginInit();
		this._SplitMain.Panel1.SuspendLayout();
		this._SplitMain.Panel2.SuspendLayout();
		this._SplitMain.SuspendLayout();
		this.SuspendLayout();
		//
		// _RecentFiles
		//
		this._RecentFiles.MenuItem = this._MenuFileRecent;
		this._RecentFiles.ItemSelected += new System.EventHandler<Robbiblubber.Spellbook.Controls.RecentItemEventArgs>(this._RecentFiles_ItemSelected);
		this._RecentFiles.Changed += new System.EventHandler(this._Recent_Changed);
		//
		// _RecentConnections
		//
		this._RecentConnections.MenuItem = this._MenuDbRecent;
		this._RecentConnections.MenuStyle = Robbiblubber.Spellbook.Controls.RecentListMenuStyle.INLINE;
		this._RecentConnections.ItemSelected += new System.EventHandler<Robbiblubber.Spellbook.Controls.RecentItemEventArgs>(this._RecentConnections_ItemSelected);
		this._RecentConnections.Changed += new System.EventHandler(this._Recent_Changed);
		//
		// _MenuMain
		//
		this._MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFile, this._MenuDb });
		this._MenuMain.Location = new System.Drawing.Point(0, 0);
		this._MenuMain.Name = "_MenuMain";
		this._MenuMain.Size = new System.Drawing.Size(900, 24);
		this._MenuMain.TabIndex = 0;
		//
		// _MenuFile
		//
		this._MenuFile.Name = "_MenuFile";
		this._MenuFile.Size = new System.Drawing.Size(240, 22);
		this._MenuFile.Text = "&File";
		this._MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFileOpen, this._MenuFileOpenFolder, this._MenuFileSep1, this._MenuFileRecent, this._MenuFileSep2, this._MenuFileNewWindow, this._MenuFileSep3, this._MenuFileExit });
		//
		// _MenuFileOpen
		//
		this._MenuFileOpen.Name = "_MenuFileOpen";
		this._MenuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
		this._MenuFileOpen.Size = new System.Drawing.Size(240, 22);
		this._MenuFileOpen.Text = "&Open File...";
		this._MenuFileOpen.Click += new System.EventHandler(this._MenuFileOpen_Click);
		//
		// _MenuFileOpenFolder
		//
		this._MenuFileOpenFolder.Name = "_MenuFileOpenFolder";
		this._MenuFileOpenFolder.Size = new System.Drawing.Size(240, 22);
		this._MenuFileOpenFolder.Text = "Open F&older...";
		this._MenuFileOpenFolder.Click += new System.EventHandler(this._MenuFileOpenFolder_Click);
		//
		// _MenuFileSep1
		//
		this._MenuFileSep1.Name = "_MenuFileSep1";
		this._MenuFileSep1.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuFileRecent
		//
		this._MenuFileRecent.Name = "_MenuFileRecent";
		this._MenuFileRecent.Size = new System.Drawing.Size(240, 22);
		this._MenuFileRecent.Text = "Recent &Files";
		//
		// _MenuFileSep2
		//
		this._MenuFileSep2.Name = "_MenuFileSep2";
		this._MenuFileSep2.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuFileNewWindow
		//
		this._MenuFileNewWindow.Name = "_MenuFileNewWindow";
		this._MenuFileNewWindow.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
		this._MenuFileNewWindow.Size = new System.Drawing.Size(240, 22);
		this._MenuFileNewWindow.Text = "&New Window";
		this._MenuFileNewWindow.Click += new System.EventHandler(this._MenuFileNewWindow_Click);
		//
		// _MenuFileSep3
		//
		this._MenuFileSep3.Name = "_MenuFileSep3";
		this._MenuFileSep3.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuFileExit
		//
		this._MenuFileExit.Name = "_MenuFileExit";
		this._MenuFileExit.Size = new System.Drawing.Size(240, 22);
		this._MenuFileExit.Text = "E&xit";
		this._MenuFileExit.Click += new System.EventHandler(this._MenuFileExit_Click);
		//
		// _MenuDb
		//
		this._MenuDb.Name = "_MenuDb";
		this._MenuDb.Size = new System.Drawing.Size(240, 22);
		this._MenuDb.Text = "&Database";
		this._MenuDb.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuDbConnect, this._MenuDbSep1, this._MenuDbRecent, this._MenuDbSep2, this._MenuDbDropStaging });
		//
		// _MenuDbConnect
		//
		this._MenuDbConnect.Name = "_MenuDbConnect";
		this._MenuDbConnect.Size = new System.Drawing.Size(240, 22);
		this._MenuDbConnect.Text = "&Connect To";
		//
		// _MenuDbSep1
		//
		this._MenuDbSep1.Name = "_MenuDbSep1";
		this._MenuDbSep1.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuDbRecent
		//
		this._MenuDbRecent.Name = "_MenuDbRecent";
		this._MenuDbRecent.Size = new System.Drawing.Size(240, 22);
		this._MenuDbRecent.Text = "(recent connections)";
		//
		// _MenuDbSep2
		//
		this._MenuDbSep2.Name = "_MenuDbSep2";
		this._MenuDbSep2.Size = new System.Drawing.Size(237, 6);
		//
		// _MenuDbDropStaging
		//
		this._MenuDbDropStaging.Name = "_MenuDbDropStaging";
		this._MenuDbDropStaging.Size = new System.Drawing.Size(240, 22);
		this._MenuDbDropStaging.Text = "&Drop Connection 'Staging'";
		this._MenuDbDropStaging.CheckOnClick = true;
		this._MenuDbDropStaging.CheckedChanged += new System.EventHandler(this._MenuDbDropStaging_CheckedChanged);
		//
		// _PanelOptions
		//
		this._PanelOptions.Controls.Add(this._ChkFullPath);
		this._PanelOptions.Controls.Add(this._ChkWindowsRecent);
		this._PanelOptions.Controls.Add(this._ChkNumbers);
		this._PanelOptions.Controls.Add(this._ChkClear);
		this._PanelOptions.Controls.Add(this._ChkImages);
		this._PanelOptions.Controls.Add(this._LabelMax);
		this._PanelOptions.Controls.Add(this._NumMax);
		this._PanelOptions.Dock = System.Windows.Forms.DockStyle.Top;
		this._PanelOptions.Location = new System.Drawing.Point(0, 24);
		this._PanelOptions.Name = "_PanelOptions";
		this._PanelOptions.Size = new System.Drawing.Size(900, 36);
		this._PanelOptions.TabIndex = 1;
		//
		// _ChkFullPath
		//
		this._ChkFullPath.AutoSize = true;
		this._ChkFullPath.Location = new System.Drawing.Point(10, 9);
		this._ChkFullPath.Name = "_ChkFullPath";
		this._ChkFullPath.TabIndex = 0;
		this._ChkFullPath.Text = "Show full &path";
		this._ChkFullPath.UseVisualStyleBackColor = true;
		this._ChkFullPath.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkWindowsRecent
		//
		this._ChkWindowsRecent.AutoSize = true;
		this._ChkWindowsRecent.Location = new System.Drawing.Point(140, 9);
		this._ChkWindowsRecent.Name = "_ChkWindowsRecent";
		this._ChkWindowsRecent.TabIndex = 0;
		this._ChkWindowsRecent.Text = "Add to &Windows recent";
		this._ChkWindowsRecent.UseVisualStyleBackColor = true;
		this._ChkWindowsRecent.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkNumbers
		//
		this._ChkNumbers.AutoSize = true;
		this._ChkNumbers.Checked = true;
		this._ChkNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkNumbers.Location = new System.Drawing.Point(310, 9);
		this._ChkNumbers.Name = "_ChkNumbers";
		this._ChkNumbers.TabIndex = 0;
		this._ChkNumbers.Text = "Show &numbers";
		this._ChkNumbers.UseVisualStyleBackColor = true;
		this._ChkNumbers.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkClear
		//
		this._ChkClear.AutoSize = true;
		this._ChkClear.Checked = true;
		this._ChkClear.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkClear.Location = new System.Drawing.Point(430, 9);
		this._ChkClear.Name = "_ChkClear";
		this._ChkClear.TabIndex = 0;
		this._ChkClear.Text = "Show c&lear item";
		this._ChkClear.UseVisualStyleBackColor = true;
		this._ChkClear.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _ChkImages
		//
		this._ChkImages.AutoSize = true;
		this._ChkImages.Checked = true;
		this._ChkImages.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkImages.Location = new System.Drawing.Point(560, 9);
		this._ChkImages.Name = "_ChkImages";
		this._ChkImages.TabIndex = 0;
		this._ChkImages.Text = "Show &images";
		this._ChkImages.UseVisualStyleBackColor = true;
		this._ChkImages.CheckedChanged += new System.EventHandler(this._Options_Changed);
		//
		// _LabelMax
		//
		this._LabelMax.AutoSize = true;
		this._LabelMax.Location = new System.Drawing.Point(680, 11);
		this._LabelMax.Name = "_LabelMax";
		this._LabelMax.TabIndex = 0;
		this._LabelMax.Text = "&Maximum:";
		//
		// _NumMax
		//
		this._NumMax.Location = new System.Drawing.Point(750, 7);
		this._NumMax.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
		this._NumMax.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
		this._NumMax.Name = "_NumMax";
		this._NumMax.Size = new System.Drawing.Size(60, 23);
		this._NumMax.TabIndex = 0;
		this._NumMax.Value = new decimal(new int[] { 10, 0, 0, 0 });
		this._NumMax.ValueChanged += new System.EventHandler(this._Options_Changed);
		//
		// _SplitMain
		//
		this._SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitMain.Location = new System.Drawing.Point(0, 60);
		this._SplitMain.Name = "_SplitMain";
		this._SplitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this._SplitMain.Panel1.Controls.Add(this._TextContent);
		this._SplitMain.Panel2.Controls.Add(this._ListLog);
		this._SplitMain.Size = new System.Drawing.Size(900, 540);
		this._SplitMain.SplitterDistance = 330;
		this._SplitMain.TabIndex = 2;
		//
		// _TextContent
		//
		this._TextContent.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextContent.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._TextContent.Location = new System.Drawing.Point(0, 0);
		this._TextContent.Multiline = true;
		this._TextContent.Name = "_TextContent";
		this._TextContent.ReadOnly = true;
		this._TextContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
		this._TextContent.Size = new System.Drawing.Size(900, 330);
		this._TextContent.TabIndex = 0;
		this._TextContent.WordWrap = false;
		//
		// _ListLog
		//
		this._ListLog.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ListLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._ListLog.HorizontalScrollbar = true;
		this._ListLog.IntegralHeight = false;
		this._ListLog.ItemHeight = 14;
		this._ListLog.Location = new System.Drawing.Point(0, 0);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(900, 206);
		this._ListLog.TabIndex = 0;
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(900, 600);
		this.Controls.Add(this._SplitMain);
		this.Controls.Add(this._PanelOptions);
		this.Controls.Add(this._MenuMain);
		this.MainMenuStrip = this._MenuMain;
		this.MinimumSize = new System.Drawing.Size(700, 400);
		this.Name = "MainWindow";
		this.Text = "Spellbook Recent Files Sample";
		this._SplitMain.Panel1.ResumeLayout(false);
		this._SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).EndInit();
		this._SplitMain.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._NumMax)).EndInit();
		this._PanelOptions.ResumeLayout(false);
		this._PanelOptions.PerformLayout();
		this._MenuMain.ResumeLayout(false);
		this._MenuMain.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.RecentFileList _RecentFiles;
	private Robbiblubber.Spellbook.Controls.Samples.RecentFiles.RecentConnectionList _RecentConnections;
	private System.Windows.Forms.MenuStrip _MenuMain;
	private System.Windows.Forms.ToolStripMenuItem _MenuFile;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileOpen;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileOpenFolder;
	private System.Windows.Forms.ToolStripSeparator _MenuFileSep1;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileRecent;
	private System.Windows.Forms.ToolStripSeparator _MenuFileSep2;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileNewWindow;
	private System.Windows.Forms.ToolStripSeparator _MenuFileSep3;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileExit;
	private System.Windows.Forms.ToolStripMenuItem _MenuDb;
	private System.Windows.Forms.ToolStripMenuItem _MenuDbConnect;
	private System.Windows.Forms.ToolStripSeparator _MenuDbSep1;
	private System.Windows.Forms.ToolStripMenuItem _MenuDbRecent;
	private System.Windows.Forms.ToolStripSeparator _MenuDbSep2;
	private System.Windows.Forms.ToolStripMenuItem _MenuDbDropStaging;
	private System.Windows.Forms.Panel _PanelOptions;
	private System.Windows.Forms.CheckBox _ChkFullPath;
	private System.Windows.Forms.CheckBox _ChkWindowsRecent;
	private System.Windows.Forms.CheckBox _ChkNumbers;
	private System.Windows.Forms.CheckBox _ChkClear;
	private System.Windows.Forms.CheckBox _ChkImages;
	private System.Windows.Forms.Label _LabelMax;
	private System.Windows.Forms.NumericUpDown _NumMax;
	private System.Windows.Forms.SplitContainer _SplitMain;
	private System.Windows.Forms.TextBox _TextContent;
	private System.Windows.Forms.ListBox _ListLog;
}
