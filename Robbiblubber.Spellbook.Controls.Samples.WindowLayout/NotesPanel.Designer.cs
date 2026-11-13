namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

partial class NotesPanel
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
		this._Layout = new Robbiblubber.Spellbook.Controls.WindowLayoutProvider(this.components);
		this._SplitNotes = new System.Windows.Forms.SplitContainer();
		this._TextNotes = new System.Windows.Forms.TextBox();
		this._TextSummary = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)(this._SplitNotes)).BeginInit();
		this._SplitNotes.Panel1.SuspendLayout();
		this._SplitNotes.Panel2.SuspendLayout();
		this._SplitNotes.SuspendLayout();
		this.SuspendLayout();
		//
		// _Layout
		//
		this._Layout.ContainerControl = this;
		this._Layout.SetSaveLayout(this._SplitNotes, true);
		this._Layout.Restored += new System.EventHandler(this._Layout_Restored);
		this._Layout.Saving += new System.EventHandler(this._Layout_Saving);
		//
		// _SplitNotes
		//
		this._SplitNotes.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitNotes.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
		this._SplitNotes.Location = new System.Drawing.Point(0, 0);
		this._SplitNotes.Name = "_SplitNotes";
		this._SplitNotes.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this._SplitNotes.Panel1.Controls.Add(this._TextNotes);
		this._SplitNotes.Panel2.Controls.Add(this._TextSummary);
		this._SplitNotes.Size = new System.Drawing.Size(300, 200);
		this._SplitNotes.SplitterDistance = 120;
		this._SplitNotes.TabIndex = 0;
		//
		// _TextNotes
		//
		this._TextNotes.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextNotes.Location = new System.Drawing.Point(0, 0);
		this._TextNotes.Multiline = true;
		this._TextNotes.Name = "_TextNotes";
		this._TextNotes.Size = new System.Drawing.Size(300, 120);
		this._TextNotes.TabIndex = 0;
		this._TextNotes.Text = "Notes (panel 1 grows with the window).\r\nThe user control has its own WindowLayoutProvider.";
		//
		// _TextSummary
		//
		this._TextSummary.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextSummary.Location = new System.Drawing.Point(0, 0);
		this._TextSummary.Multiline = true;
		this._TextSummary.Name = "_TextSummary";
		this._TextSummary.Size = new System.Drawing.Size(300, 76);
		this._TextSummary.TabIndex = 0;
		this._TextSummary.Text = "Summary (panel 2 has a fixed size).";
		//
		// NotesPanel
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._SplitNotes);
		this.Name = "NotesPanel";
		this.Size = new System.Drawing.Size(300, 200);
		this._SplitNotes.Panel1.ResumeLayout(false);
		this._SplitNotes.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitNotes)).EndInit();
		this._SplitNotes.ResumeLayout(false);
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.WindowLayoutProvider _Layout;
	private System.Windows.Forms.SplitContainer _SplitNotes;
	private System.Windows.Forms.TextBox _TextNotes;
	private System.Windows.Forms.TextBox _TextSummary;
}
