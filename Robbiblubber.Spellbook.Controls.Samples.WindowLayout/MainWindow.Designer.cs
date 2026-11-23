namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

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
		this._Layout = new Robbiblubber.Spellbook.Controls.WindowLayoutProvider(this.components);
		this._PanelTop = new System.Windows.Forms.Panel();
		this._ButtonTool = new System.Windows.Forms.Button();
		this._ButtonDialog = new System.Windows.Forms.Button();
		this._ButtonOffscreen = new System.Windows.Forms.Button();
		this._ButtonShowFile = new System.Windows.Forms.Button();
		this._ChkPerScreen = new System.Windows.Forms.CheckBox();
		this._ChkMaximized = new System.Windows.Forms.CheckBox();
		this._ChkMinimized = new System.Windows.Forms.CheckBox();
		this._ChkModified = new System.Windows.Forms.CheckBox();
		this._SplitMain = new System.Windows.Forms.SplitContainer();
		this._ListFolders = new System.Windows.Forms.ListBox();
		this._SplitRight = new System.Windows.Forms.SplitContainer();
		this._Tabs = new System.Windows.Forms.TabControl();
		this._PageFiles = new System.Windows.Forms.TabPage();
		this._PageData = new System.Windows.Forms.TabPage();
		this._ListFiles = new System.Windows.Forms.ListView();
		this._ColumnName = new System.Windows.Forms.ColumnHeader();
		this._ColumnType = new System.Windows.Forms.ColumnHeader();
		this._ColumnSize = new System.Windows.Forms.ColumnHeader();
		this._ColumnModified = new System.Windows.Forms.ColumnHeader();
		this._Grid = new System.Windows.Forms.DataGridView();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._StatusMain = new System.Windows.Forms.StatusStrip();
		this._LabelFile = new System.Windows.Forms.ToolStripStatusLabel();
		this._PanelTop.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).BeginInit();
		this._SplitMain.Panel1.SuspendLayout();
		this._SplitMain.Panel2.SuspendLayout();
		this._SplitMain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._SplitRight)).BeginInit();
		this._SplitRight.Panel1.SuspendLayout();
		this._SplitRight.Panel2.SuspendLayout();
		this._SplitRight.SuspendLayout();
		this._Tabs.SuspendLayout();
		this._PageFiles.SuspendLayout();
		this._PageData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._Grid)).BeginInit();
		this._StatusMain.SuspendLayout();
		this.SuspendLayout();
		//
		// _Layout
		//
		this._Layout.ContainerControl = this;
		this._Layout.SetSaveLayout(this._SplitMain, true);
		this._Layout.SetSaveLayout(this._SplitRight, true);
		this._Layout.SetSaveLayout(this._Tabs, true);
		this._Layout.SetSaveLayout(this._ListFiles, true);
		this._Layout.SetSaveLayout(this._Grid, true);
		this._Layout.Restored += new System.EventHandler(this._Layout_Restored);
		this._Layout.Saving += new System.EventHandler(this._Layout_Saving);
		//
		// _PanelTop
		//
		this._PanelTop.Controls.Add(this._ButtonTool);
		this._PanelTop.Controls.Add(this._ButtonDialog);
		this._PanelTop.Controls.Add(this._ButtonOffscreen);
		this._PanelTop.Controls.Add(this._ButtonShowFile);
		this._PanelTop.Controls.Add(this._ChkPerScreen);
		this._PanelTop.Controls.Add(this._ChkMaximized);
		this._PanelTop.Controls.Add(this._ChkMinimized);
		this._PanelTop.Controls.Add(this._ChkModified);
		this._PanelTop.Dock = System.Windows.Forms.DockStyle.Top;
		this._PanelTop.Location = new System.Drawing.Point(0, 0);
		this._PanelTop.Name = "_PanelTop";
		this._PanelTop.Size = new System.Drawing.Size(1000, 66);
		this._PanelTop.TabIndex = 0;
		//
		// _ButtonTool
		//
		this._ButtonTool.Location = new System.Drawing.Point(8, 6);
		this._ButtonTool.Name = "_ButtonTool";
		this._ButtonTool.Size = new System.Drawing.Size(150, 26);
		this._ButtonTool.TabIndex = 0;
		this._ButtonTool.Text = "Open &tool window";
		this._ButtonTool.UseVisualStyleBackColor = true;
		this._ButtonTool.Click += new System.EventHandler(this._ButtonTool_Click);
		//
		// _ButtonDialog
		//
		this._ButtonDialog.Location = new System.Drawing.Point(164, 6);
		this._ButtonDialog.Name = "_ButtonDialog";
		this._ButtonDialog.Size = new System.Drawing.Size(120, 26);
		this._ButtonDialog.TabIndex = 0;
		this._ButtonDialog.Text = "Open &dialog";
		this._ButtonDialog.UseVisualStyleBackColor = true;
		this._ButtonDialog.Click += new System.EventHandler(this._ButtonDialog_Click);
		//
		// _ButtonOffscreen
		//
		this._ButtonOffscreen.Location = new System.Drawing.Point(290, 6);
		this._ButtonOffscreen.Name = "_ButtonOffscreen";
		this._ButtonOffscreen.Size = new System.Drawing.Size(190, 26);
		this._ButtonOffscreen.TabIndex = 0;
		this._ButtonOffscreen.Text = "&Simulate missing monitor";
		this._ButtonOffscreen.UseVisualStyleBackColor = true;
		this._ButtonOffscreen.Click += new System.EventHandler(this._ButtonOffscreen_Click);
		//
		// _ButtonShowFile
		//
		this._ButtonShowFile.Location = new System.Drawing.Point(486, 6);
		this._ButtonShowFile.Name = "_ButtonShowFile";
		this._ButtonShowFile.Size = new System.Drawing.Size(140, 26);
		this._ButtonShowFile.TabIndex = 0;
		this._ButtonShowFile.Text = "Show layout &file";
		this._ButtonShowFile.UseVisualStyleBackColor = true;
		this._ButtonShowFile.Click += new System.EventHandler(this._ButtonShowFile_Click);
		//
		// _ChkPerScreen
		//
		this._ChkPerScreen.AutoSize = true;
		this._ChkPerScreen.Location = new System.Drawing.Point(10, 40);
		this._ChkPerScreen.Name = "_ChkPerScreen";
		this._ChkPerScreen.TabIndex = 0;
		this._ChkPerScreen.Text = "&Per screen configuration";
		this._ChkPerScreen.UseVisualStyleBackColor = true;
		this._ChkPerScreen.CheckedChanged += new System.EventHandler(this._ChkOptions_CheckedChanged);
		//
		// _ChkMaximized
		//
		this._ChkMaximized.AutoSize = true;
		this._ChkMaximized.Checked = true;
		this._ChkMaximized.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkMaximized.Location = new System.Drawing.Point(210, 40);
		this._ChkMaximized.Name = "_ChkMaximized";
		this._ChkMaximized.TabIndex = 0;
		this._ChkMaximized.Text = "Restore &maximized";
		this._ChkMaximized.UseVisualStyleBackColor = true;
		this._ChkMaximized.CheckedChanged += new System.EventHandler(this._ChkOptions_CheckedChanged);
		//
		// _ChkMinimized
		//
		this._ChkMinimized.AutoSize = true;
		this._ChkMinimized.Location = new System.Drawing.Point(370, 40);
		this._ChkMinimized.Name = "_ChkMinimized";
		this._ChkMinimized.TabIndex = 0;
		this._ChkMinimized.Text = "Restore mi&nimized";
		this._ChkMinimized.UseVisualStyleBackColor = true;
		this._ChkMinimized.CheckedChanged += new System.EventHandler(this._ChkOptions_CheckedChanged);
		//
		// _ChkModified
		//
		this._ChkModified.AutoSize = true;
		this._ChkModified.Checked = true;
		this._ChkModified.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkModified.Location = new System.Drawing.Point(530, 40);
		this._ChkModified.Name = "_ChkModified";
		this._ChkModified.TabIndex = 0;
		this._ChkModified.Text = "Show 'Modified' &column in the grid";
		this._ChkModified.UseVisualStyleBackColor = true;
		this._ChkModified.CheckedChanged += new System.EventHandler(this._ChkModified_CheckedChanged);
		//
		// _SplitMain
		//
		this._SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
		this._SplitMain.Location = new System.Drawing.Point(0, 66);
		this._SplitMain.Name = "_SplitMain";
		this._SplitMain.Panel1.Controls.Add(this._ListFolders);
		this._SplitMain.Panel2.Controls.Add(this._SplitRight);
		this._SplitMain.Size = new System.Drawing.Size(1000, 592);
		this._SplitMain.SplitterDistance = 220;
		this._SplitMain.TabIndex = 1;
		//
		// _ListFolders
		//
		this._ListFolders.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ListFolders.FormattingEnabled = true;
		this._ListFolders.IntegralHeight = false;
		this._ListFolders.ItemHeight = 15;
		this._ListFolders.Location = new System.Drawing.Point(0, 0);
		this._ListFolders.Name = "_ListFolders";
		this._ListFolders.Size = new System.Drawing.Size(220, 592);
		this._ListFolders.TabIndex = 0;
		//
		// _SplitRight
		//
		this._SplitRight.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitRight.Location = new System.Drawing.Point(0, 0);
		this._SplitRight.Name = "_SplitRight";
		this._SplitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this._SplitRight.Panel1.Controls.Add(this._Tabs);
		this._SplitRight.Panel2.Controls.Add(this._ListLog);
		this._SplitRight.Size = new System.Drawing.Size(776, 592);
		this._SplitRight.SplitterDistance = 400;
		this._SplitRight.TabIndex = 0;
		//
		// _Tabs
		//
		this._Tabs.Controls.Add(this._PageFiles);
		this._Tabs.Controls.Add(this._PageData);
		this._Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
		this._Tabs.Location = new System.Drawing.Point(0, 0);
		this._Tabs.Name = "_Tabs";
		this._Tabs.SelectedIndex = 0;
		this._Tabs.Size = new System.Drawing.Size(776, 400);
		this._Tabs.TabIndex = 0;
		//
		// _PageFiles
		//
		this._PageFiles.Controls.Add(this._ListFiles);
		this._PageFiles.Location = new System.Drawing.Point(4, 24);
		this._PageFiles.Name = "_PageFiles";
		this._PageFiles.Size = new System.Drawing.Size(768, 372);
		this._PageFiles.TabIndex = 0;
		this._PageFiles.Text = "Files";
		this._PageFiles.UseVisualStyleBackColor = true;
		//
		// _PageData
		//
		this._PageData.Controls.Add(this._Grid);
		this._PageData.Location = new System.Drawing.Point(4, 24);
		this._PageData.Name = "_PageData";
		this._PageData.Size = new System.Drawing.Size(768, 372);
		this._PageData.TabIndex = 1;
		this._PageData.Text = "Data";
		this._PageData.UseVisualStyleBackColor = true;
		//
		// _ListFiles
		//
		this._ListFiles.AllowColumnReorder = true;
		this._ListFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this._ColumnName, this._ColumnType, this._ColumnSize, this._ColumnModified });
		this._ListFiles.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ListFiles.FullRowSelect = true;
		this._ListFiles.Location = new System.Drawing.Point(0, 0);
		this._ListFiles.Name = "_ListFiles";
		this._ListFiles.Size = new System.Drawing.Size(768, 372);
		this._ListFiles.TabIndex = 0;
		this._ListFiles.UseCompatibleStateImageBehavior = false;
		this._ListFiles.View = System.Windows.Forms.View.Details;
		//
		// _ColumnName
		//
		this._ColumnName.Text = "Name";
		this._ColumnName.Width = 220;
		//
		// _ColumnType
		//
		this._ColumnType.Text = "Type";
		this._ColumnType.Width = 120;
		//
		// _ColumnSize
		//
		this._ColumnSize.Text = "Size";
		this._ColumnSize.Width = 90;
		//
		// _ColumnModified
		//
		this._ColumnModified.Text = "Modified";
		this._ColumnModified.Width = 150;
		//
		// _Grid
		//
		this._Grid.AllowUserToAddRows = false;
		this._Grid.AllowUserToOrderColumns = true;
		this._Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this._Grid.Dock = System.Windows.Forms.DockStyle.Fill;
		this._Grid.Location = new System.Drawing.Point(0, 0);
		this._Grid.Name = "_Grid";
		this._Grid.ReadOnly = true;
		this._Grid.Size = new System.Drawing.Size(768, 372);
		this._Grid.TabIndex = 0;
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
		this._ListLog.Size = new System.Drawing.Size(776, 188);
		this._ListLog.TabIndex = 0;
		//
		// _StatusMain
		//
		this._StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._LabelFile });
		this._StatusMain.Location = new System.Drawing.Point(0, 658);
		this._StatusMain.Name = "_StatusMain";
		this._StatusMain.Size = new System.Drawing.Size(1000, 22);
		this._StatusMain.TabIndex = 2;
		//
		// _LabelFile
		//
		this._LabelFile.Name = "_LabelFile";
		this._LabelFile.Size = new System.Drawing.Size(985, 17);
		this._LabelFile.Spring = true;
		this._LabelFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(1000, 680);
		this.Controls.Add(this._SplitMain);
		this.Controls.Add(this._PanelTop);
		this.Controls.Add(this._StatusMain);
		this.MinimumSize = new System.Drawing.Size(700, 450);
		this.Name = "MainWindow";
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Spellbook Window Layout Sample";
		this._StatusMain.ResumeLayout(false);
		this._StatusMain.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._Grid)).EndInit();
		this._PageData.ResumeLayout(false);
		this._PageData.PerformLayout();
		this._PageFiles.ResumeLayout(false);
		this._PageFiles.PerformLayout();
		this._Tabs.ResumeLayout(false);
		this._Tabs.PerformLayout();
		this._SplitRight.Panel1.ResumeLayout(false);
		this._SplitRight.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitRight)).EndInit();
		this._SplitRight.ResumeLayout(false);
		this._SplitMain.Panel1.ResumeLayout(false);
		this._SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).EndInit();
		this._SplitMain.ResumeLayout(false);
		this._PanelTop.ResumeLayout(false);
		this._PanelTop.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.WindowLayoutProvider _Layout;
	private System.Windows.Forms.Panel _PanelTop;
	private System.Windows.Forms.Button _ButtonTool;
	private System.Windows.Forms.Button _ButtonDialog;
	private System.Windows.Forms.Button _ButtonOffscreen;
	private System.Windows.Forms.Button _ButtonShowFile;
	private System.Windows.Forms.CheckBox _ChkPerScreen;
	private System.Windows.Forms.CheckBox _ChkMaximized;
	private System.Windows.Forms.CheckBox _ChkMinimized;
	private System.Windows.Forms.CheckBox _ChkModified;
	private System.Windows.Forms.SplitContainer _SplitMain;
	private System.Windows.Forms.ListBox _ListFolders;
	private System.Windows.Forms.SplitContainer _SplitRight;
	private System.Windows.Forms.TabControl _Tabs;
	private System.Windows.Forms.TabPage _PageFiles;
	private System.Windows.Forms.TabPage _PageData;
	private System.Windows.Forms.ListView _ListFiles;
	private System.Windows.Forms.ColumnHeader _ColumnName;
	private System.Windows.Forms.ColumnHeader _ColumnType;
	private System.Windows.Forms.ColumnHeader _ColumnSize;
	private System.Windows.Forms.ColumnHeader _ColumnModified;
	private System.Windows.Forms.DataGridView _Grid;
	private System.Windows.Forms.ListBox _ListLog;
	private System.Windows.Forms.StatusStrip _StatusMain;
	private System.Windows.Forms.ToolStripStatusLabel _LabelFile;
}
