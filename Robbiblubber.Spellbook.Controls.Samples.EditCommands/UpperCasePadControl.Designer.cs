namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

partial class UpperCasePadControl
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
		this._TextPad = new System.Windows.Forms.TextBox();
		this.SuspendLayout();
		//
		// _TextPad
		//
		this._TextPad.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TextPad.Location = new System.Drawing.Point(0, 0);
		this._TextPad.Name = "_TextPad";
		this._TextPad.Multiline = true;
		this._TextPad.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this._TextPad.Size = new System.Drawing.Size(300, 200);
		this._TextPad.TabIndex = 0;
		this._TextPad.Text = "Paste text here.";
		this._TextPad.TextChanged += new System.EventHandler(this._TextPad_TextChanged);
		//
		// UpperCasePadControl
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this._TextPad);
		this.Name = "UpperCasePadControl";
		this.Size = new System.Drawing.Size(300, 200);
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.TextBox _TextPad;
}
