namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

partial class ToolWindow
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
		this._Notes = new Robbiblubber.Spellbook.Controls.Samples.WindowLayout.NotesPanel();
		this.SuspendLayout();
		//
		// _Layout
		//
		this._Layout.ContainerControl = this;
		this._Layout.Restored += new System.EventHandler(this._Layout_Restored);
		//
		// _Notes
		//
		this._Notes.Dock = System.Windows.Forms.DockStyle.Fill;
		this._Notes.Location = new System.Drawing.Point(0, 0);
		this._Notes.Name = "_Notes";
		this._Notes.Size = new System.Drawing.Size(420, 320);
		this._Notes.TabIndex = 0;
		//
		// ToolWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(420, 320);
		this.Controls.Add(this._Notes);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
		this.Name = "ToolWindow";
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Tool Window";
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.WindowLayoutProvider _Layout;
	private Robbiblubber.Spellbook.Controls.Samples.WindowLayout.NotesPanel _Notes;
}
