namespace Robbiblubber.Spellbook.Backpack.Samples.Packing;



partial class MainWindow
{
	/// <summary>Required designer variable.</summary>
	private System.ComponentModel.IContainer components = null;


	/// <summary>Clean up any resources being used.</summary>
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

	/// <summary>Required method for Designer support - do not modify the contents of this method with the code editor.</summary>
	private void InitializeComponent()
	{
		_GroupFiles = new GroupBox();
		_LabelCarrier = new Label();
		_TextCarrier = new TextBox();
		_ButtonBrowseCarrier = new Button();
		_LabelPayload = new Label();
		_TextPayload = new TextBox();
		_ButtonBrowsePayload = new Button();
		_LabelTarget = new Label();
		_TextTarget = new TextBox();
		_ButtonBrowseTarget = new Button();
		_LabelTag = new Label();
		_TextTag = new TextBox();
		_CheckAsync = new CheckBox();
		_PanelButtons = new Panel();
		_ButtonAppend = new Button();
		_ButtonInfo = new Button();
		_ButtonVerify = new Button();
		_ButtonExtract = new Button();
		_ButtonRemove = new Button();
		_ButtonOwn = new Button();
		_ListLog = new ListBox();
		_ButtonClear = new Button();
		_GroupFiles.SuspendLayout();
		_PanelButtons.SuspendLayout();
		SuspendLayout();
		// 
		// _GroupFiles
		// 
		_GroupFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_GroupFiles.Location = new Point(12, 12);
		_GroupFiles.Name = "_GroupFiles";
		_GroupFiles.Size = new Size(736, 154);
		_GroupFiles.TabIndex = 0;
		_GroupFiles.TabStop = false;
		_GroupFiles.Text = "Files";
		_GroupFiles.Controls.Add(_LabelCarrier);
		_GroupFiles.Controls.Add(_TextCarrier);
		_GroupFiles.Controls.Add(_ButtonBrowseCarrier);
		_GroupFiles.Controls.Add(_LabelPayload);
		_GroupFiles.Controls.Add(_TextPayload);
		_GroupFiles.Controls.Add(_ButtonBrowsePayload);
		_GroupFiles.Controls.Add(_LabelTarget);
		_GroupFiles.Controls.Add(_TextTarget);
		_GroupFiles.Controls.Add(_ButtonBrowseTarget);
		_GroupFiles.Controls.Add(_LabelTag);
		_GroupFiles.Controls.Add(_TextTag);
		_GroupFiles.Controls.Add(_CheckAsync);
		// 
		// _LabelCarrier
		// 
		_LabelCarrier.AutoSize = true;
		_LabelCarrier.Location = new Point(12, 29);
		_LabelCarrier.Name = "_LabelCarrier";
		_LabelCarrier.Size = new Size(60, 15);
		_LabelCarrier.TabIndex = 0;
		_LabelCarrier.Text = "Carrier:";
		// 
		// _TextCarrier
		// 
		_TextCarrier.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextCarrier.Location = new Point(88, 26);
		_TextCarrier.Name = "_TextCarrier";
		_TextCarrier.Size = new Size(590, 23);
		_TextCarrier.TabIndex = 1;
		// 
		// _ButtonBrowseCarrier
		// 
		_ButtonBrowseCarrier.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseCarrier.Location = new Point(684, 25);
		_ButtonBrowseCarrier.Name = "_ButtonBrowseCarrier";
		_ButtonBrowseCarrier.Size = new Size(36, 25);
		_ButtonBrowseCarrier.TabIndex = 2;
		_ButtonBrowseCarrier.Text = "...";
		_ButtonBrowseCarrier.UseVisualStyleBackColor = true;
		_ButtonBrowseCarrier.Click += _ButtonBrowseCarrier_Click;
		// 
		// _LabelPayload
		// 
		_LabelPayload.AutoSize = true;
		_LabelPayload.Location = new Point(12, 61);
		_LabelPayload.Name = "_LabelPayload";
		_LabelPayload.Size = new Size(60, 15);
		_LabelPayload.TabIndex = 3;
		_LabelPayload.Text = "Payload:";
		// 
		// _TextPayload
		// 
		_TextPayload.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextPayload.Location = new Point(88, 58);
		_TextPayload.Name = "_TextPayload";
		_TextPayload.Size = new Size(590, 23);
		_TextPayload.TabIndex = 4;
		// 
		// _ButtonBrowsePayload
		// 
		_ButtonBrowsePayload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowsePayload.Location = new Point(684, 57);
		_ButtonBrowsePayload.Name = "_ButtonBrowsePayload";
		_ButtonBrowsePayload.Size = new Size(36, 25);
		_ButtonBrowsePayload.TabIndex = 5;
		_ButtonBrowsePayload.Text = "...";
		_ButtonBrowsePayload.UseVisualStyleBackColor = true;
		_ButtonBrowsePayload.Click += _ButtonBrowsePayload_Click;
		// 
		// _LabelTarget
		// 
		_LabelTarget.AutoSize = true;
		_LabelTarget.Location = new Point(12, 93);
		_LabelTarget.Name = "_LabelTarget";
		_LabelTarget.Size = new Size(60, 15);
		_LabelTarget.TabIndex = 6;
		_LabelTarget.Text = "Target:";
		// 
		// _TextTarget
		// 
		_TextTarget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextTarget.Location = new Point(88, 90);
		_TextTarget.Name = "_TextTarget";
		_TextTarget.Size = new Size(590, 23);
		_TextTarget.TabIndex = 7;
		// 
		// _ButtonBrowseTarget
		// 
		_ButtonBrowseTarget.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseTarget.Location = new Point(684, 89);
		_ButtonBrowseTarget.Name = "_ButtonBrowseTarget";
		_ButtonBrowseTarget.Size = new Size(36, 25);
		_ButtonBrowseTarget.TabIndex = 8;
		_ButtonBrowseTarget.Text = "...";
		_ButtonBrowseTarget.UseVisualStyleBackColor = true;
		_ButtonBrowseTarget.Click += _ButtonBrowseTarget_Click;
		// 
		// _LabelTag
		// 
		_LabelTag.AutoSize = true;
		_LabelTag.Location = new Point(12, 125);
		_LabelTag.Name = "_LabelTag";
		_LabelTag.Size = new Size(28, 15);
		_LabelTag.TabIndex = 9;
		_LabelTag.Text = "Tag:";
		// 
		// _TextTag
		// 
		_TextTag.Location = new Point(88, 122);
		_TextTag.Name = "_TextTag";
		_TextTag.Size = new Size(200, 23);
		_TextTag.TabIndex = 10;
		// 
		// _CheckAsync
		// 
		_CheckAsync.AutoSize = true;
		_CheckAsync.Location = new Point(310, 124);
		_CheckAsync.Name = "_CheckAsync";
		_CheckAsync.Size = new Size(100, 19);
		_CheckAsync.TabIndex = 11;
		_CheckAsync.Text = "Asynchronous";
		_CheckAsync.UseVisualStyleBackColor = true;
		// 
		// _PanelButtons
		// 
		_PanelButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_PanelButtons.Location = new Point(12, 174);
		_PanelButtons.Name = "_PanelButtons";
		_PanelButtons.Size = new Size(736, 30);
		_PanelButtons.Controls.Add(_ButtonAppend);
		_PanelButtons.Controls.Add(_ButtonInfo);
		_PanelButtons.Controls.Add(_ButtonVerify);
		_PanelButtons.Controls.Add(_ButtonExtract);
		_PanelButtons.Controls.Add(_ButtonRemove);
		_PanelButtons.Controls.Add(_ButtonOwn);
		_PanelButtons.TabIndex = 1;
		// 
		// _ButtonAppend
		// 
		_ButtonAppend.Location = new Point(0, 0);
		_ButtonAppend.Name = "_ButtonAppend";
		_ButtonAppend.Size = new Size(110, 27);
		_ButtonAppend.TabIndex = 0;
		_ButtonAppend.Text = "Append";
		_ButtonAppend.UseVisualStyleBackColor = true;
		_ButtonAppend.Click += _ButtonAppend_Click;
		// 
		// _ButtonInfo
		// 
		_ButtonInfo.Location = new Point(116, 0);
		_ButtonInfo.Name = "_ButtonInfo";
		_ButtonInfo.Size = new Size(110, 27);
		_ButtonInfo.TabIndex = 1;
		_ButtonInfo.Text = "Info";
		_ButtonInfo.UseVisualStyleBackColor = true;
		_ButtonInfo.Click += _ButtonInfo_Click;
		// 
		// _ButtonVerify
		// 
		_ButtonVerify.Location = new Point(232, 0);
		_ButtonVerify.Name = "_ButtonVerify";
		_ButtonVerify.Size = new Size(110, 27);
		_ButtonVerify.TabIndex = 2;
		_ButtonVerify.Text = "Verify";
		_ButtonVerify.UseVisualStyleBackColor = true;
		_ButtonVerify.Click += _ButtonVerify_Click;
		// 
		// _ButtonExtract
		// 
		_ButtonExtract.Location = new Point(348, 0);
		_ButtonExtract.Name = "_ButtonExtract";
		_ButtonExtract.Size = new Size(110, 27);
		_ButtonExtract.TabIndex = 3;
		_ButtonExtract.Text = "Extract";
		_ButtonExtract.UseVisualStyleBackColor = true;
		_ButtonExtract.Click += _ButtonExtract_Click;
		// 
		// _ButtonRemove
		// 
		_ButtonRemove.Location = new Point(464, 0);
		_ButtonRemove.Name = "_ButtonRemove";
		_ButtonRemove.Size = new Size(110, 27);
		_ButtonRemove.TabIndex = 4;
		_ButtonRemove.Text = "Remove";
		_ButtonRemove.UseVisualStyleBackColor = true;
		_ButtonRemove.Click += _ButtonRemove_Click;
		// 
		// _ButtonOwn
		// 
		_ButtonOwn.Location = new Point(580, 0);
		_ButtonOwn.Name = "_ButtonOwn";
		_ButtonOwn.Size = new Size(110, 27);
		_ButtonOwn.TabIndex = 5;
		_ButtonOwn.Text = "Own payload";
		_ButtonOwn.UseVisualStyleBackColor = true;
		_ButtonOwn.Click += _ButtonOwn_Click;
		// 
		// _ListLog
		// 
		_ListLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		_ListLog.FormattingEnabled = true;
		_ListLog.HorizontalScrollbar = true;
		_ListLog.IntegralHeight = false;
		_ListLog.Location = new Point(12, 212);
		_ListLog.Name = "_ListLog";
		_ListLog.Size = new Size(736, 293);
		_ListLog.TabIndex = 2;
		// 
		// _ButtonClear
		// 
		_ButtonClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		_ButtonClear.Location = new Point(638, 513);
		_ButtonClear.Name = "_ButtonClear";
		_ButtonClear.Size = new Size(110, 27);
		_ButtonClear.TabIndex = 3;
		_ButtonClear.Text = "Clear log";
		_ButtonClear.UseVisualStyleBackColor = true;
		_ButtonClear.Click += _ButtonClear_Click;
		// 
		// MainWindow
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(760, 552);
		Controls.Add(_ButtonClear);
		Controls.Add(_ListLog);
		Controls.Add(_PanelButtons);
		Controls.Add(_GroupFiles);
		MinimumSize = new Size(600, 400);
		Name = "MainWindow";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "Backpack Sample";
		_GroupFiles.ResumeLayout(false);
		_GroupFiles.PerformLayout();
		_PanelButtons.ResumeLayout(false);
		ResumeLayout(false);
		}

	#endregion


	/// <summary>GroupFiles control.</summary>
	private GroupBox _GroupFiles;
	/// <summary>LabelCarrier control.</summary>
	private Label _LabelCarrier;
	/// <summary>TextCarrier control.</summary>
	private TextBox _TextCarrier;
	/// <summary>ButtonBrowseCarrier control.</summary>
	private Button _ButtonBrowseCarrier;
	/// <summary>LabelPayload control.</summary>
	private Label _LabelPayload;
	/// <summary>TextPayload control.</summary>
	private TextBox _TextPayload;
	/// <summary>ButtonBrowsePayload control.</summary>
	private Button _ButtonBrowsePayload;
	/// <summary>LabelTarget control.</summary>
	private Label _LabelTarget;
	/// <summary>TextTarget control.</summary>
	private TextBox _TextTarget;
	/// <summary>ButtonBrowseTarget control.</summary>
	private Button _ButtonBrowseTarget;
	/// <summary>LabelTag control.</summary>
	private Label _LabelTag;
	/// <summary>TextTag control.</summary>
	private TextBox _TextTag;
	/// <summary>CheckAsync control.</summary>
	private CheckBox _CheckAsync;
	/// <summary>PanelButtons control.</summary>
	private Panel _PanelButtons;
	/// <summary>ButtonAppend control.</summary>
	private Button _ButtonAppend;
	/// <summary>ButtonInfo control.</summary>
	private Button _ButtonInfo;
	/// <summary>ButtonVerify control.</summary>
	private Button _ButtonVerify;
	/// <summary>ButtonExtract control.</summary>
	private Button _ButtonExtract;
	/// <summary>ButtonRemove control.</summary>
	private Button _ButtonRemove;
	/// <summary>ButtonOwn control.</summary>
	private Button _ButtonOwn;
	/// <summary>ListLog control.</summary>
	private ListBox _ListLog;
	/// <summary>ButtonClear control.</summary>
	private Button _ButtonClear;
}
