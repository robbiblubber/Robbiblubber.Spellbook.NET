namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

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
		this._SplitMain = new System.Windows.Forms.SplitContainer();
		this._TabMain = new System.Windows.Forms.TabControl();
		this._PageBasics = new System.Windows.Forms.TabPage();
		this._ControlBasics = new Robbiblubber.Spellbook.Controls.Samples.SignatureHelp.BasicsControl();
		this._PageSql = new System.Windows.Forms.TabPage();
		this._ControlSql = new Robbiblubber.Spellbook.Controls.Samples.SignatureHelp.SqlControl();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._PanelLogHeader = new System.Windows.Forms.Panel();
		this._LabelLog = new System.Windows.Forms.Label();
		this._ChkLogSelection = new System.Windows.Forms.CheckBox();
		this._ButtonCopyLog = new System.Windows.Forms.Button();
		this._ButtonClearLog = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).BeginInit();
		this._SplitMain.Panel1.SuspendLayout();
		this._SplitMain.Panel2.SuspendLayout();
		this._SplitMain.SuspendLayout();
		this._TabMain.SuspendLayout();
		this._PageBasics.SuspendLayout();
		this._PageSql.SuspendLayout();
		this._PanelLogHeader.SuspendLayout();
		this.SuspendLayout();
		//
		// _SplitMain
		//
		this._SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitMain.Location = new System.Drawing.Point(0, 0);
		this._SplitMain.Name = "_SplitMain";
		this._SplitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
		//
		// _SplitMain.Panel1
		//
		this._SplitMain.Panel1.Controls.Add(this._TabMain);
		//
		// _SplitMain.Panel2
		//
		this._SplitMain.Panel2.Controls.Add(this._ListLog);
		this._SplitMain.Panel2.Controls.Add(this._PanelLogHeader);
		this._SplitMain.Size = new System.Drawing.Size(884, 711);
		this._SplitMain.SplitterDistance = 480;
		this._SplitMain.TabIndex = 0;
		//
		// _TabMain
		//
		this._TabMain.Controls.Add(this._PageBasics);
		this._TabMain.Controls.Add(this._PageSql);
		this._TabMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TabMain.Location = new System.Drawing.Point(0, 0);
		this._TabMain.Name = "_TabMain";
		this._TabMain.SelectedIndex = 0;
		this._TabMain.Size = new System.Drawing.Size(884, 480);
		this._TabMain.TabIndex = 0;
		//
		// _PageBasics
		//
		this._PageBasics.Controls.Add(this._ControlBasics);
		this._PageBasics.Location = new System.Drawing.Point(4, 24);
		this._PageBasics.Name = "_PageBasics";
		this._PageBasics.Size = new System.Drawing.Size(876, 452);
		this._PageBasics.TabIndex = 0;
		this._PageBasics.Text = "Basics";
		this._PageBasics.UseVisualStyleBackColor = true;
		//
		// _ControlBasics
		//
		this._ControlBasics.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ControlBasics.Location = new System.Drawing.Point(0, 0);
		this._ControlBasics.Name = "_ControlBasics";
		this._ControlBasics.Size = new System.Drawing.Size(876, 452);
		this._ControlBasics.TabIndex = 0;
		//
		// _PageSql
		//
		this._PageSql.Controls.Add(this._ControlSql);
		this._PageSql.Location = new System.Drawing.Point(4, 24);
		this._PageSql.Name = "_PageSql";
		this._PageSql.Size = new System.Drawing.Size(876, 452);
		this._PageSql.TabIndex = 1;
		this._PageSql.Text = "SQL";
		this._PageSql.UseVisualStyleBackColor = true;
		//
		// _ControlSql
		//
		this._ControlSql.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ControlSql.Location = new System.Drawing.Point(0, 0);
		this._ControlSql.Name = "_ControlSql";
		this._ControlSql.Size = new System.Drawing.Size(876, 452);
		this._ControlSql.TabIndex = 0;
		//
		// _ListLog
		//
		this._ListLog.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ListLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		this._ListLog.HorizontalScrollbar = true;
		this._ListLog.IntegralHeight = false;
		this._ListLog.ItemHeight = 14;
		this._ListLog.Location = new System.Drawing.Point(0, 32);
		this._ListLog.Name = "_ListLog";
		this._ListLog.Size = new System.Drawing.Size(884, 195);
		this._ListLog.TabIndex = 1;
		//
		// _PanelLogHeader
		//
		this._PanelLogHeader.Controls.Add(this._LabelLog);
		this._PanelLogHeader.Controls.Add(this._ChkLogSelection);
		this._PanelLogHeader.Controls.Add(this._ButtonCopyLog);
		this._PanelLogHeader.Controls.Add(this._ButtonClearLog);
		this._PanelLogHeader.Dock = System.Windows.Forms.DockStyle.Top;
		this._PanelLogHeader.Location = new System.Drawing.Point(0, 0);
		this._PanelLogHeader.Name = "_PanelLogHeader";
		this._PanelLogHeader.Size = new System.Drawing.Size(884, 32);
		this._PanelLogHeader.TabIndex = 0;
		//
		// _LabelLog
		//
		this._LabelLog.AutoSize = true;
		this._LabelLog.Location = new System.Drawing.Point(8, 9);
		this._LabelLog.Name = "_LabelLog";
		this._LabelLog.Size = new System.Drawing.Size(250, 15);
		this._LabelLog.TabIndex = 0;
		this._LabelLog.Text = "Event log (Starting, Populating, Selection, Closed; auto completion: accepted, closed):";
		// 
		// _ChkLogSelection
		// 
		this._ChkLogSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this._ChkLogSelection.AutoSize = true;
		this._ChkLogSelection.Checked = true;
		this._ChkLogSelection.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkLogSelection.Location = new System.Drawing.Point(590, 7);
		this._ChkLogSelection.Name = "_ChkLogSelection";
		this._ChkLogSelection.Size = new System.Drawing.Size(110, 19);
		this._ChkLogSelection.TabIndex = 3;
		this._ChkLogSelection.Text = "Log &selection";
		this._ChkLogSelection.UseVisualStyleBackColor = true;
		this._ChkLogSelection.CheckedChanged += new System.EventHandler(this._ChkLogSelection_CheckedChanged);
		// 
		// _ButtonCopyLog
		// 
		this._ButtonCopyLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this._ButtonCopyLog.Location = new System.Drawing.Point(710, 3);
		this._ButtonCopyLog.Name = "_ButtonCopyLog";
		this._ButtonCopyLog.Size = new System.Drawing.Size(80, 26);
		this._ButtonCopyLog.TabIndex = 1;
		this._ButtonCopyLog.Text = "C&opy";
		this._ButtonCopyLog.UseVisualStyleBackColor = true;
		this._ButtonCopyLog.Click += new System.EventHandler(this._ButtonCopyLog_Click);
		//
		// _ButtonClearLog
		//
		this._ButtonClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this._ButtonClearLog.Location = new System.Drawing.Point(796, 3);
		this._ButtonClearLog.Name = "_ButtonClearLog";
		this._ButtonClearLog.Size = new System.Drawing.Size(80, 26);
		this._ButtonClearLog.TabIndex = 2;
		this._ButtonClearLog.Text = "C&lear";
		this._ButtonClearLog.UseVisualStyleBackColor = true;
		this._ButtonClearLog.Click += new System.EventHandler(this._ButtonClearLog_Click);
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(884, 711);
		this.Controls.Add(this._SplitMain);
		this.MinimumSize = new System.Drawing.Size(700, 500);
		this.Name = "MainWindow";
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Spellbook Signature Help Samples";
		this._SplitMain.Panel1.ResumeLayout(false);
		this._SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).EndInit();
		this._SplitMain.ResumeLayout(false);
		this._TabMain.ResumeLayout(false);
		this._PageBasics.ResumeLayout(false);
		this._PageSql.ResumeLayout(false);
		this._PanelLogHeader.ResumeLayout(false);
		this._PanelLogHeader.PerformLayout();
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.SplitContainer _SplitMain;
	private System.Windows.Forms.TabControl _TabMain;
	private System.Windows.Forms.TabPage _PageBasics;
	private Robbiblubber.Spellbook.Controls.Samples.SignatureHelp.BasicsControl _ControlBasics;
	private System.Windows.Forms.TabPage _PageSql;
	private Robbiblubber.Spellbook.Controls.Samples.SignatureHelp.SqlControl _ControlSql;
	private System.Windows.Forms.ListBox _ListLog;
	private System.Windows.Forms.Panel _PanelLogHeader;
	private System.Windows.Forms.Label _LabelLog;
	private System.Windows.Forms.CheckBox _ChkLogSelection;
	private System.Windows.Forms.Button _ButtonCopyLog;
	private System.Windows.Forms.Button _ButtonClearLog;
}
