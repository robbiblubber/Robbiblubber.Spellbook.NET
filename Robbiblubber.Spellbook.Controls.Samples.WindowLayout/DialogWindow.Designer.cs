namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

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
		this._Layout = new Robbiblubber.Spellbook.Controls.WindowLayoutProvider(this.components);
		this._LabelInfo = new System.Windows.Forms.Label();
		this._ButtonOk = new System.Windows.Forms.Button();
		this.SuspendLayout();
		//
		// _Layout
		//
		this._Layout.ContainerControl = this;
		this._Layout.Restored += new System.EventHandler(this._Layout_Restored);
		//
		// _LabelInfo
		//
		this._LabelInfo.Location = new System.Drawing.Point(12, 12);
		this._LabelInfo.Name = "_LabelInfo";
		this._LabelInfo.Size = new System.Drawing.Size(336, 70);
		this._LabelInfo.TabIndex = 0;
		this._LabelInfo.Text = "This dialog has a fixed size, so only its position is saved. Move it, close it and open it again.";
		//
		// _ButtonOk
		//
		this._ButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
		this._ButtonOk.Location = new System.Drawing.Point(272, 100);
		this._ButtonOk.Name = "_ButtonOk";
		this._ButtonOk.Size = new System.Drawing.Size(76, 26);
		this._ButtonOk.TabIndex = 1;
		this._ButtonOk.Text = "OK";
		this._ButtonOk.UseVisualStyleBackColor = true;
		//
		// DialogWindow
		//
		this.AcceptButton = this._ButtonOk;
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(360, 140);
		this.Controls.Add(this._LabelInfo);
		this.Controls.Add(this._ButtonOk);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "DialogWindow";
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Fixed Dialog";
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.WindowLayoutProvider _Layout;
	private System.Windows.Forms.Label _LabelInfo;
	private System.Windows.Forms.Button _ButtonOk;
}
