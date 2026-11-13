namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

partial class NoteBoardControl
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
		this._ListNotes = new System.Windows.Forms.ListBox();
		this.SuspendLayout();
		//
		// _ListNotes
		//
		this._ListNotes.Dock = System.Windows.Forms.DockStyle.Fill;
		this._ListNotes.FormattingEnabled = true;
		this._ListNotes.IntegralHeight = false;
		this._ListNotes.ItemHeight = 15;
		this._ListNotes.Location = new System.Drawing.Point(0, 0);
		this._ListNotes.Name = "_ListNotes";
		this._ListNotes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this._ListNotes.Size = new System.Drawing.Size(300, 200);
		this._ListNotes.TabIndex = 0;
		this._ListNotes.SelectedIndexChanged += new System.EventHandler(this._ListNotes_SelectedIndexChanged);
		this._ListNotes.MouseDown += new System.Windows.Forms.MouseEventHandler(this._ListNotes_MouseDown);
		//
		// NoteBoardControl
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._ListNotes);
		this.Name = "NoteBoardControl";
		this.Size = new System.Drawing.Size(300, 200);
		this.ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.ListBox _ListNotes;
}
