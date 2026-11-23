namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

partial class DialogWindow
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
		this._LabelCity = new System.Windows.Forms.Label();
		this._TextCity = new System.Windows.Forms.TextBox();
		this._ButtonOk = new System.Windows.Forms.Button();
		this._ButtonCancel = new System.Windows.Forms.Button();
		this._AutoCompletion = new Robbiblubber.Spellbook.Controls.AutoCompletionProvider(this.components);
		this.SuspendLayout();
		//
		// _LabelCity
		//
		this._LabelCity.AutoSize = true;
		this._LabelCity.Location = new System.Drawing.Point(12, 15);
		this._LabelCity.Name = "_LabelCity";
		this._LabelCity.Size = new System.Drawing.Size(250, 15);
		this._LabelCity.TabIndex = 0;
		this._LabelCity.Text = "&City (Enter/Escape: list first, then dialog):";
		//
		// _TextCity
		//
		this._AutoCompletion.SetAutoCompletionEnabled(this._TextCity, true);
		this._TextCity.Location = new System.Drawing.Point(12, 35);
		this._TextCity.Name = "_TextCity";
		this._TextCity.Size = new System.Drawing.Size(360, 23);
		this._TextCity.TabIndex = 1;
		//
		// _ButtonOk
		//
		this._ButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
		this._ButtonOk.Location = new System.Drawing.Point(196, 80);
		this._ButtonOk.Name = "_ButtonOk";
		this._ButtonOk.Size = new System.Drawing.Size(85, 27);
		this._ButtonOk.TabIndex = 2;
		this._ButtonOk.Text = "OK";
		this._ButtonOk.UseVisualStyleBackColor = true;
		//
		// _ButtonCancel
		//
		this._ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this._ButtonCancel.Location = new System.Drawing.Point(287, 80);
		this._ButtonCancel.Name = "_ButtonCancel";
		this._ButtonCancel.Size = new System.Drawing.Size(85, 27);
		this._ButtonCancel.TabIndex = 3;
		this._ButtonCancel.Text = "Cancel";
		this._ButtonCancel.UseVisualStyleBackColor = true;
		//
		// DialogWindow
		//
		this.AcceptButton = this._ButtonOk;
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.CancelButton = this._ButtonCancel;
		this.ClientSize = new System.Drawing.Size(384, 121);
		this.Controls.Add(this._ButtonCancel);
		this.Controls.Add(this._ButtonOk);
		this.Controls.Add(this._TextCity);
		this.Controls.Add(this._LabelCity);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "DialogWindow";
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Dialog Test";
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label _LabelCity;
	private System.Windows.Forms.TextBox _TextCity;
	private System.Windows.Forms.Button _ButtonOk;
	private System.Windows.Forms.Button _ButtonCancel;
	private Robbiblubber.Spellbook.Controls.AutoCompletionProvider _AutoCompletion;
}
