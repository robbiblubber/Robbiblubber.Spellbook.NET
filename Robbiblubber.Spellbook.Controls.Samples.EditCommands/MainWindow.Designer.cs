namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

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
		this._Provider = new Robbiblubber.Spellbook.Controls.EditCommandProvider(this.components);
		this._MenuMain = new System.Windows.Forms.MenuStrip();
		this._MenuFile = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditUndo = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditRedo = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuEditCut = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this._MenuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditInvertSelection = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuEditClearSelection = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuOptions = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuOptionsAutoUpdate = new System.Windows.Forms.ToolStripMenuItem();
		this._MenuOptionsUpdateNow = new System.Windows.Forms.ToolStripMenuItem();
		this._ToolMain = new System.Windows.Forms.ToolStrip();
		this._ToolUndo = new System.Windows.Forms.ToolStripButton();
		this._ToolRedo = new System.Windows.Forms.ToolStripButton();
		this._ToolSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this._ToolCut = new System.Windows.Forms.ToolStripButton();
		this._ToolCopy = new System.Windows.Forms.ToolStripButton();
		this._ToolPaste = new System.Windows.Forms.ToolStripButton();
		this._ToolDelete = new System.Windows.Forms.ToolStripButton();
		this._ToolSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this._ToolSelectAll = new System.Windows.Forms.ToolStripButton();
		this._StatusMain = new System.Windows.Forms.StatusStrip();
		this._LabelTarget = new System.Windows.Forms.ToolStripStatusLabel();
		this._LabelState = new System.Windows.Forms.ToolStripStatusLabel();
		this._ContextList = new System.Windows.Forms.ContextMenuStrip(this.components);
		this._ContextListCut = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListCopy = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListPaste = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListDelete = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListSeparator = new System.Windows.Forms.ToolStripSeparator();
		this._ContextListSelectAll = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListInvertSelection = new System.Windows.Forms.ToolStripMenuItem();
		this._ContextListClearSelection = new System.Windows.Forms.ToolStripMenuItem();
		this._SplitMain = new System.Windows.Forms.SplitContainer();
		this._TabMain = new System.Windows.Forms.TabControl();
		this._PageText = new System.Windows.Forms.TabPage();
		this._LabelMultiline = new System.Windows.Forms.Label();
		this._TextMultiline = new System.Windows.Forms.TextBox();
		this._LabelPassword = new System.Windows.Forms.Label();
		this._TextPassword = new System.Windows.Forms.TextBox();
		this._LabelExcluded = new System.Windows.Forms.Label();
		this._TextExcluded = new System.Windows.Forms.TextBox();
		this._LabelRich = new System.Windows.Forms.Label();
		this._RichText = new System.Windows.Forms.RichTextBox();
		this._LabelComboEditable = new System.Windows.Forms.Label();
		this._ComboEditable = new System.Windows.Forms.ComboBox();
		this._LabelComboList = new System.Windows.Forms.Label();
		this._ComboList = new System.Windows.Forms.ComboBox();
		this._LabelReadOnly = new System.Windows.Forms.Label();
		this._TextReadOnly = new System.Windows.Forms.TextBox();
		this._PageLists = new System.Windows.Forms.TabPage();
		this._LabelListView = new System.Windows.Forms.Label();
		this._ListView = new System.Windows.Forms.ListView();
		this._ColumnName = new System.Windows.Forms.ColumnHeader();
		this._ColumnColor = new System.Windows.Forms.ColumnHeader();
		this._ColumnPrice = new System.Windows.Forms.ColumnHeader();
		this._LabelListMulti = new System.Windows.Forms.Label();
		this._ListMulti = new System.Windows.Forms.ListBox();
		this._LabelListSingle = new System.Windows.Forms.Label();
		this._ListSingle = new System.Windows.Forms.ListBox();
		this._ChkHandleDelete = new System.Windows.Forms.CheckBox();
		this._ChkConfirmDelete = new System.Windows.Forms.CheckBox();
		this._LabelGrid = new System.Windows.Forms.Label();
		this._Grid = new System.Windows.Forms.DataGridView();
		this._PageCustom = new System.Windows.Forms.TabPage();
		this._LabelNotes = new System.Windows.Forms.Label();
		this._NoteBoard = new Robbiblubber.Spellbook.Controls.Samples.EditCommands.NoteBoardControl();
		this._TextNewNote = new System.Windows.Forms.TextBox();
		this._ButtonAddNote = new System.Windows.Forms.Button();
		this._LabelPad = new System.Windows.Forms.Label();
		this._Pad = new Robbiblubber.Spellbook.Controls.Samples.EditCommands.UpperCasePadControl();
		this._ChkUpperCasePaste = new System.Windows.Forms.CheckBox();
		this._ListLog = new System.Windows.Forms.ListBox();
		this._PanelLogHeader = new System.Windows.Forms.Panel();
		this._LabelLog = new System.Windows.Forms.Label();
		this._ButtonCopyLog = new System.Windows.Forms.Button();
		this._ButtonClearLog = new System.Windows.Forms.Button();
		this._MenuMain.SuspendLayout();
		this._ToolMain.SuspendLayout();
		this._StatusMain.SuspendLayout();
		this._ContextList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).BeginInit();
		this._SplitMain.Panel1.SuspendLayout();
		this._SplitMain.Panel2.SuspendLayout();
		this._SplitMain.SuspendLayout();
		this._TabMain.SuspendLayout();
		this._PageText.SuspendLayout();
		this._PageLists.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this._Grid)).BeginInit();
		this._PageCustom.SuspendLayout();
		this._PanelLogHeader.SuspendLayout();
		this.SuspendLayout();
		//
		// _Provider
		//
		this._Provider.ContainerControl = this;
		this._Provider.SetEditCommand(this._MenuEditUndo, Robbiblubber.Spellbook.Controls.EditCommand.UNDO);
		this._Provider.SetEditCommand(this._MenuEditRedo, Robbiblubber.Spellbook.Controls.EditCommand.REDO);
		this._Provider.SetEditCommand(this._MenuEditCut, Robbiblubber.Spellbook.Controls.EditCommand.CUT);
		this._Provider.SetEditCommand(this._MenuEditCopy, Robbiblubber.Spellbook.Controls.EditCommand.COPY);
		this._Provider.SetEditCommand(this._MenuEditPaste, Robbiblubber.Spellbook.Controls.EditCommand.PASTE);
		this._Provider.SetEditCommand(this._MenuEditDelete, Robbiblubber.Spellbook.Controls.EditCommand.DELETE);
		this._Provider.SetEditCommand(this._MenuEditSelectAll, Robbiblubber.Spellbook.Controls.EditCommand.SELECT_ALL);
		this._Provider.SetEditCommand(this._MenuEditInvertSelection, Robbiblubber.Spellbook.Controls.EditCommand.INVERT_SELECTION);
		this._Provider.SetEditCommand(this._MenuEditClearSelection, Robbiblubber.Spellbook.Controls.EditCommand.CLEAR_SELECTION);
		this._Provider.SetEditCommand(this._ToolUndo, Robbiblubber.Spellbook.Controls.EditCommand.UNDO);
		this._Provider.SetEditCommand(this._ToolRedo, Robbiblubber.Spellbook.Controls.EditCommand.REDO);
		this._Provider.SetEditCommand(this._ToolCut, Robbiblubber.Spellbook.Controls.EditCommand.CUT);
		this._Provider.SetEditCommand(this._ToolCopy, Robbiblubber.Spellbook.Controls.EditCommand.COPY);
		this._Provider.SetEditCommand(this._ToolPaste, Robbiblubber.Spellbook.Controls.EditCommand.PASTE);
		this._Provider.SetEditCommand(this._ToolDelete, Robbiblubber.Spellbook.Controls.EditCommand.DELETE);
		this._Provider.SetEditCommand(this._ToolSelectAll, Robbiblubber.Spellbook.Controls.EditCommand.SELECT_ALL);
		this._Provider.SetEditCommand(this._ContextListCut, Robbiblubber.Spellbook.Controls.EditCommand.CUT);
		this._Provider.SetEditCommand(this._ContextListCopy, Robbiblubber.Spellbook.Controls.EditCommand.COPY);
		this._Provider.SetEditCommand(this._ContextListPaste, Robbiblubber.Spellbook.Controls.EditCommand.PASTE);
		this._Provider.SetEditCommand(this._ContextListDelete, Robbiblubber.Spellbook.Controls.EditCommand.DELETE);
		this._Provider.SetEditCommand(this._ContextListSelectAll, Robbiblubber.Spellbook.Controls.EditCommand.SELECT_ALL);
		this._Provider.SetEditCommand(this._ContextListInvertSelection, Robbiblubber.Spellbook.Controls.EditCommand.INVERT_SELECTION);
		this._Provider.SetEditCommand(this._ContextListClearSelection, Robbiblubber.Spellbook.Controls.EditCommand.CLEAR_SELECTION);
		this._Provider.SetEditCommandsEnabled(this._TextExcluded, false);
		this._Provider.Querying += new System.EventHandler<Robbiblubber.Spellbook.Controls.EditCommandQueryEventArgs>(this._Provider_Querying);
		this._Provider.Executing += new System.EventHandler<Robbiblubber.Spellbook.Controls.EditCommandExecuteEventArgs>(this._Provider_Executing);
		this._Provider.EditStateChanged += new System.EventHandler(this._Provider_EditStateChanged);
		//
		// _MenuMain
		//
		this._MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFile, this._MenuEdit, this._MenuOptions });
		this._MenuMain.Location = new System.Drawing.Point(0, 0);
		this._MenuMain.Name = "_MenuMain";
		this._MenuMain.Size = new System.Drawing.Size(984, 24);
		this._MenuMain.TabIndex = 0;
		//
		// _MenuFile
		//
		this._MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuFileExit });
		this._MenuFile.Name = "_MenuFile";
		this._MenuFile.Size = new System.Drawing.Size(37, 20);
		this._MenuFile.Text = "&File";
		//
		// _MenuFileExit
		//
		this._MenuFileExit.Name = "_MenuFileExit";
		this._MenuFileExit.Size = new System.Drawing.Size(220, 22);
		this._MenuFileExit.Text = "E&xit";
		this._MenuFileExit.Click += new System.EventHandler(this._MenuFileExit_Click);
		//
		// _MenuEdit
		//
		this._MenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuEditUndo, this._MenuEditRedo, this._MenuEditSeparator1, this._MenuEditCut, this._MenuEditCopy, this._MenuEditPaste, this._MenuEditDelete, this._MenuEditSeparator2, this._MenuEditSelectAll, this._MenuEditInvertSelection, this._MenuEditClearSelection });
		this._MenuEdit.Name = "_MenuEdit";
		this._MenuEdit.Size = new System.Drawing.Size(39, 20);
		this._MenuEdit.Text = "&Edit";
		//
		// _MenuEditUndo
		//
		this._MenuEditUndo.Name = "_MenuEditUndo";
		this._MenuEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z));
		this._MenuEditUndo.Size = new System.Drawing.Size(220, 22);
		this._MenuEditUndo.Text = "&Undo";
		//
		// _MenuEditRedo
		//
		this._MenuEditRedo.Name = "_MenuEditRedo";
		this._MenuEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y));
		this._MenuEditRedo.Size = new System.Drawing.Size(220, 22);
		this._MenuEditRedo.Text = "&Redo";
		//
		// _MenuEditSeparator1
		//
		this._MenuEditSeparator1.Name = "_MenuEditSeparator1";
		this._MenuEditSeparator1.Size = new System.Drawing.Size(217, 6);
		//
		// _MenuEditCut
		//
		this._MenuEditCut.Name = "_MenuEditCut";
		this._MenuEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X));
		this._MenuEditCut.Size = new System.Drawing.Size(220, 22);
		this._MenuEditCut.Text = "Cu&t";
		//
		// _MenuEditCopy
		//
		this._MenuEditCopy.Name = "_MenuEditCopy";
		this._MenuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C));
		this._MenuEditCopy.Size = new System.Drawing.Size(220, 22);
		this._MenuEditCopy.Text = "&Copy";
		//
		// _MenuEditPaste
		//
		this._MenuEditPaste.Name = "_MenuEditPaste";
		this._MenuEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V));
		this._MenuEditPaste.Size = new System.Drawing.Size(220, 22);
		this._MenuEditPaste.Text = "&Paste";
		//
		// _MenuEditDelete
		//
		this._MenuEditDelete.Name = "_MenuEditDelete";
		this._MenuEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
		this._MenuEditDelete.Size = new System.Drawing.Size(220, 22);
		this._MenuEditDelete.Text = "&Delete";
		//
		// _MenuEditSeparator2
		//
		this._MenuEditSeparator2.Name = "_MenuEditSeparator2";
		this._MenuEditSeparator2.Size = new System.Drawing.Size(217, 6);
		//
		// _MenuEditSelectAll
		//
		this._MenuEditSelectAll.Name = "_MenuEditSelectAll";
		this._MenuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
		this._MenuEditSelectAll.Size = new System.Drawing.Size(220, 22);
		this._MenuEditSelectAll.Text = "Select &All";
		//
		// _MenuEditInvertSelection
		//
		this._MenuEditInvertSelection.Name = "_MenuEditInvertSelection";
		this._MenuEditInvertSelection.Size = new System.Drawing.Size(220, 22);
		this._MenuEditInvertSelection.Text = "&Invert Selection";
		//
		// _MenuEditClearSelection
		//
		this._MenuEditClearSelection.Name = "_MenuEditClearSelection";
		this._MenuEditClearSelection.Size = new System.Drawing.Size(220, 22);
		this._MenuEditClearSelection.Text = "Clear &Selection";
		//
		// _MenuOptions
		//
		this._MenuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._MenuOptionsAutoUpdate, this._MenuOptionsUpdateNow });
		this._MenuOptions.Name = "_MenuOptions";
		this._MenuOptions.Size = new System.Drawing.Size(61, 20);
		this._MenuOptions.Text = "&Options";
		//
		// _MenuOptionsAutoUpdate
		//
		this._MenuOptionsAutoUpdate.Name = "_MenuOptionsAutoUpdate";
		this._MenuOptionsAutoUpdate.Size = new System.Drawing.Size(220, 22);
		this._MenuOptionsAutoUpdate.Text = "&Automatic Update";
		this._MenuOptionsAutoUpdate.Checked = true;
		this._MenuOptionsAutoUpdate.CheckOnClick = true;
		this._MenuOptionsAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
		this._MenuOptionsAutoUpdate.CheckedChanged += new System.EventHandler(this._MenuOptionsAutoUpdate_CheckedChanged);
		//
		// _MenuOptionsUpdateNow
		//
		this._MenuOptionsUpdateNow.Name = "_MenuOptionsUpdateNow";
		this._MenuOptionsUpdateNow.ShortcutKeys = System.Windows.Forms.Keys.F5;
		this._MenuOptionsUpdateNow.Size = new System.Drawing.Size(220, 22);
		this._MenuOptionsUpdateNow.Text = "&Update Now";
		this._MenuOptionsUpdateNow.Click += new System.EventHandler(this._MenuOptionsUpdateNow_Click);
		//
		// _ToolMain
		//
		this._ToolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._ToolUndo, this._ToolRedo, this._ToolSeparator1, this._ToolCut, this._ToolCopy, this._ToolPaste, this._ToolDelete, this._ToolSeparator2, this._ToolSelectAll });
		this._ToolMain.Location = new System.Drawing.Point(0, 24);
		this._ToolMain.Name = "_ToolMain";
		this._ToolMain.Size = new System.Drawing.Size(984, 25);
		this._ToolMain.TabIndex = 1;
		//
		// _ToolUndo
		//
		this._ToolUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolUndo.Name = "_ToolUndo";
		this._ToolUndo.Size = new System.Drawing.Size(40, 22);
		this._ToolUndo.Text = "Undo";
		this._ToolUndo.ToolTipText = "Undo (Ctrl+Z)";
		//
		// _ToolRedo
		//
		this._ToolRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolRedo.Name = "_ToolRedo";
		this._ToolRedo.Size = new System.Drawing.Size(40, 22);
		this._ToolRedo.Text = "Redo";
		this._ToolRedo.ToolTipText = "Redo (Ctrl+Y)";
		//
		// _ToolSeparator1
		//
		this._ToolSeparator1.Name = "_ToolSeparator1";
		this._ToolSeparator1.Size = new System.Drawing.Size(6, 25);
		//
		// _ToolCut
		//
		this._ToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolCut.Name = "_ToolCut";
		this._ToolCut.Size = new System.Drawing.Size(40, 22);
		this._ToolCut.Text = "Cut";
		this._ToolCut.ToolTipText = "Cut (Ctrl+X)";
		//
		// _ToolCopy
		//
		this._ToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolCopy.Name = "_ToolCopy";
		this._ToolCopy.Size = new System.Drawing.Size(40, 22);
		this._ToolCopy.Text = "Copy";
		this._ToolCopy.ToolTipText = "Copy (Ctrl+C)";
		//
		// _ToolPaste
		//
		this._ToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolPaste.Name = "_ToolPaste";
		this._ToolPaste.Size = new System.Drawing.Size(40, 22);
		this._ToolPaste.Text = "Paste";
		this._ToolPaste.ToolTipText = "Paste (Ctrl+V)";
		//
		// _ToolDelete
		//
		this._ToolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolDelete.Name = "_ToolDelete";
		this._ToolDelete.Size = new System.Drawing.Size(40, 22);
		this._ToolDelete.Text = "Delete";
		this._ToolDelete.ToolTipText = "Delete (Del)";
		//
		// _ToolSeparator2
		//
		this._ToolSeparator2.Name = "_ToolSeparator2";
		this._ToolSeparator2.Size = new System.Drawing.Size(6, 25);
		//
		// _ToolSelectAll
		//
		this._ToolSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this._ToolSelectAll.Name = "_ToolSelectAll";
		this._ToolSelectAll.Size = new System.Drawing.Size(40, 22);
		this._ToolSelectAll.Text = "Select All";
		this._ToolSelectAll.ToolTipText = "Select all (Ctrl+A)";
		//
		// _StatusMain
		//
		this._StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._LabelTarget, this._LabelState });
		this._StatusMain.Location = new System.Drawing.Point(0, 739);
		this._StatusMain.Name = "_StatusMain";
		this._StatusMain.Size = new System.Drawing.Size(984, 22);
		this._StatusMain.TabIndex = 3;
		//
		// _LabelTarget
		//
		this._LabelTarget.AutoSize = false;
		this._LabelTarget.Name = "_LabelTarget";
		this._LabelTarget.Size = new System.Drawing.Size(300, 17);
		this._LabelTarget.Text = "Target: (none)";
		this._LabelTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		//
		// _LabelState
		//
		this._LabelState.Name = "_LabelState";
		this._LabelState.Size = new System.Drawing.Size(669, 17);
		this._LabelState.Spring = true;
		this._LabelState.Text = "Available: (none)";
		this._LabelState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		//
		// _ContextList
		//
		this._ContextList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._ContextListCut, this._ContextListCopy, this._ContextListPaste, this._ContextListDelete, this._ContextListSeparator, this._ContextListSelectAll, this._ContextListInvertSelection, this._ContextListClearSelection });
		this._ContextList.Name = "_ContextList";
		this._ContextList.Size = new System.Drawing.Size(181, 164);
		//
		// _ContextListCut
		//
		this._ContextListCut.Name = "_ContextListCut";
		this._ContextListCut.Size = new System.Drawing.Size(180, 22);
		this._ContextListCut.Text = "Cu&t";
		//
		// _ContextListCopy
		//
		this._ContextListCopy.Name = "_ContextListCopy";
		this._ContextListCopy.Size = new System.Drawing.Size(180, 22);
		this._ContextListCopy.Text = "&Copy";
		//
		// _ContextListPaste
		//
		this._ContextListPaste.Name = "_ContextListPaste";
		this._ContextListPaste.Size = new System.Drawing.Size(180, 22);
		this._ContextListPaste.Text = "&Paste";
		//
		// _ContextListDelete
		//
		this._ContextListDelete.Name = "_ContextListDelete";
		this._ContextListDelete.Size = new System.Drawing.Size(180, 22);
		this._ContextListDelete.Text = "&Delete";
		//
		// _ContextListSeparator
		//
		this._ContextListSeparator.Name = "_ContextListSeparator";
		this._ContextListSeparator.Size = new System.Drawing.Size(177, 6);
		//
		// _ContextListSelectAll
		//
		this._ContextListSelectAll.Name = "_ContextListSelectAll";
		this._ContextListSelectAll.Size = new System.Drawing.Size(180, 22);
		this._ContextListSelectAll.Text = "Select &All";
		//
		// _ContextListInvertSelection
		//
		this._ContextListInvertSelection.Name = "_ContextListInvertSelection";
		this._ContextListInvertSelection.Size = new System.Drawing.Size(180, 22);
		this._ContextListInvertSelection.Text = "&Invert Selection";
		//
		// _ContextListClearSelection
		//
		this._ContextListClearSelection.Name = "_ContextListClearSelection";
		this._ContextListClearSelection.Size = new System.Drawing.Size(180, 22);
		this._ContextListClearSelection.Text = "Clear &Selection";
		//
		// _SplitMain
		//
		this._SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._SplitMain.Location = new System.Drawing.Point(0, 49);
		this._SplitMain.Name = "_SplitMain";
		this._SplitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this._SplitMain.Panel1.Controls.Add(this._TabMain);
		this._SplitMain.Panel2.Controls.Add(this._ListLog);
		this._SplitMain.Panel2.Controls.Add(this._PanelLogHeader);
		this._SplitMain.Size = new System.Drawing.Size(984, 690);
		this._SplitMain.SplitterDistance = 460;
		this._SplitMain.TabIndex = 2;
		//
		// _TabMain
		//
		this._TabMain.Controls.Add(this._PageText);
		this._TabMain.Controls.Add(this._PageLists);
		this._TabMain.Controls.Add(this._PageCustom);
		this._TabMain.Dock = System.Windows.Forms.DockStyle.Fill;
		this._TabMain.Location = new System.Drawing.Point(0, 0);
		this._TabMain.Name = "_TabMain";
		this._TabMain.SelectedIndex = 0;
		this._TabMain.Size = new System.Drawing.Size(984, 460);
		this._TabMain.TabIndex = 0;
		//
		// _PageText
		//
		this._PageText.Controls.Add(this._LabelMultiline);
		this._PageText.Controls.Add(this._TextMultiline);
		this._PageText.Controls.Add(this._LabelPassword);
		this._PageText.Controls.Add(this._TextPassword);
		this._PageText.Controls.Add(this._LabelExcluded);
		this._PageText.Controls.Add(this._TextExcluded);
		this._PageText.Controls.Add(this._LabelRich);
		this._PageText.Controls.Add(this._RichText);
		this._PageText.Controls.Add(this._LabelComboEditable);
		this._PageText.Controls.Add(this._ComboEditable);
		this._PageText.Controls.Add(this._LabelComboList);
		this._PageText.Controls.Add(this._ComboList);
		this._PageText.Controls.Add(this._LabelReadOnly);
		this._PageText.Controls.Add(this._TextReadOnly);
		this._PageText.Location = new System.Drawing.Point(4, 24);
		this._PageText.Name = "_PageText";
		this._PageText.Padding = new System.Windows.Forms.Padding(3);
		this._PageText.Size = new System.Drawing.Size(976, 432);
		this._PageText.TabIndex = 0;
		this._PageText.Text = "Text";
		this._PageText.UseVisualStyleBackColor = true;
		//
		// _LabelMultiline
		//
		this._LabelMultiline.AutoSize = true;
		this._LabelMultiline.Location = new System.Drawing.Point(12, 12);
		this._LabelMultiline.Name = "_LabelMultiline";
		this._LabelMultiline.TabIndex = 0;
		this._LabelMultiline.Text = "&Multiline text box:";
		//
		// _TextMultiline
		//
		this._TextMultiline.Location = new System.Drawing.Point(12, 30);
		this._TextMultiline.Multiline = true;
		this._TextMultiline.Name = "_TextMultiline";
		this._TextMultiline.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this._TextMultiline.Size = new System.Drawing.Size(300, 150);
		this._TextMultiline.TabIndex = 1;
		this._TextMultiline.Text = "The quick brown fox jumps over the lazy dog.\r\nSecond line.\r\nThird line.";
		//
		// _LabelPassword
		//
		this._LabelPassword.AutoSize = true;
		this._LabelPassword.Location = new System.Drawing.Point(12, 192);
		this._LabelPassword.Name = "_LabelPassword";
		this._LabelPassword.TabIndex = 2;
		this._LabelPassword.Text = "&Password text box (no cut and copy):";
		//
		// _TextPassword
		//
		this._TextPassword.Location = new System.Drawing.Point(12, 210);
		this._TextPassword.Name = "_TextPassword";
		this._TextPassword.Size = new System.Drawing.Size(300, 23);
		this._TextPassword.TabIndex = 3;
		this._TextPassword.Text = "secret";
		this._TextPassword.UseSystemPasswordChar = true;
		//
		// _LabelExcluded
		//
		this._LabelExcluded.AutoSize = true;
		this._LabelExcluded.Location = new System.Drawing.Point(12, 244);
		this._LabelExcluded.Name = "_LabelExcluded";
		this._LabelExcluded.TabIndex = 4;
		this._LabelExcluded.Text = "E&xcluded text box (EditCommandsEnabled = false):";
		//
		// _TextExcluded
		//
		this._TextExcluded.Location = new System.Drawing.Point(12, 262);
		this._TextExcluded.Name = "_TextExcluded";
		this._TextExcluded.Size = new System.Drawing.Size(300, 23);
		this._TextExcluded.TabIndex = 5;
		this._TextExcluded.Text = "Menus are disabled here; native shortcuts still work.";
		//
		// _LabelRich
		//
		this._LabelRich.AutoSize = true;
		this._LabelRich.Location = new System.Drawing.Point(330, 12);
		this._LabelRich.Name = "_LabelRich";
		this._LabelRich.TabIndex = 6;
		this._LabelRich.Text = "&Rich text box (supports redo):";
		//
		// _RichText
		//
		this._RichText.Location = new System.Drawing.Point(330, 30);
		this._RichText.Name = "_RichText";
		this._RichText.Size = new System.Drawing.Size(300, 150);
		this._RichText.TabIndex = 7;
		this._RichText.Text = "Rich text box.\nType, undo and redo.";
		//
		// _LabelComboEditable
		//
		this._LabelComboEditable.AutoSize = true;
		this._LabelComboEditable.Location = new System.Drawing.Point(330, 192);
		this._LabelComboEditable.Name = "_LabelComboEditable";
		this._LabelComboEditable.TabIndex = 8;
		this._LabelComboEditable.Text = "E&ditable combo box:";
		//
		// _ComboEditable
		//
		this._ComboEditable.FormattingEnabled = true;
		this._ComboEditable.Location = new System.Drawing.Point(330, 210);
		this._ComboEditable.Name = "_ComboEditable";
		this._ComboEditable.Size = new System.Drawing.Size(300, 23);
		this._ComboEditable.TabIndex = 9;
		//
		// _LabelComboList
		//
		this._LabelComboList.AutoSize = true;
		this._LabelComboList.Location = new System.Drawing.Point(330, 244);
		this._LabelComboList.Name = "_LabelComboList";
		this._LabelComboList.TabIndex = 10;
		this._LabelComboList.Text = "Drop-down &list combo box (copy only):";
		//
		// _ComboList
		//
		this._ComboList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._ComboList.FormattingEnabled = true;
		this._ComboList.Location = new System.Drawing.Point(330, 262);
		this._ComboList.Name = "_ComboList";
		this._ComboList.Size = new System.Drawing.Size(300, 23);
		this._ComboList.TabIndex = 11;
		//
		// _LabelReadOnly
		//
		this._LabelReadOnly.AutoSize = true;
		this._LabelReadOnly.Location = new System.Drawing.Point(648, 12);
		this._LabelReadOnly.Name = "_LabelReadOnly";
		this._LabelReadOnly.TabIndex = 12;
		this._LabelReadOnly.Text = "Read-&only text box (copy and selection only):";
		//
		// _TextReadOnly
		//
		this._TextReadOnly.Location = new System.Drawing.Point(648, 30);
		this._TextReadOnly.Multiline = true;
		this._TextReadOnly.Name = "_TextReadOnly";
		this._TextReadOnly.ReadOnly = true;
		this._TextReadOnly.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this._TextReadOnly.Size = new System.Drawing.Size(300, 150);
		this._TextReadOnly.TabIndex = 13;
		this._TextReadOnly.Text = "This text is read-only.\r\nIt can be selected and copied.";
		//
		// _PageLists
		//
		this._PageLists.Controls.Add(this._ChkHandleDelete);
		this._PageLists.Controls.Add(this._ChkConfirmDelete);
		this._PageLists.Controls.Add(this._LabelListView);
		this._PageLists.Controls.Add(this._ListView);
		this._PageLists.Controls.Add(this._LabelListMulti);
		this._PageLists.Controls.Add(this._ListMulti);
		this._PageLists.Controls.Add(this._LabelListSingle);
		this._PageLists.Controls.Add(this._ListSingle);
		this._PageLists.Controls.Add(this._LabelGrid);
		this._PageLists.Controls.Add(this._Grid);
		this._PageLists.Location = new System.Drawing.Point(4, 24);
		this._PageLists.Name = "_PageLists";
		this._PageLists.Padding = new System.Windows.Forms.Padding(3);
		this._PageLists.Size = new System.Drawing.Size(976, 432);
		this._PageLists.TabIndex = 1;
		this._PageLists.Text = "Lists";
		this._PageLists.UseVisualStyleBackColor = true;
		//
		// _LabelListView
		//
		this._LabelListView.AutoSize = true;
		this._LabelListView.Location = new System.Drawing.Point(12, 40);
		this._LabelListView.Name = "_LabelListView";
		this._LabelListView.TabIndex = 0;
		this._LabelListView.Text = "List &view (multiple selection):";
		//
		// _ListView
		//
		this._ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this._ColumnName, this._ColumnColor, this._ColumnPrice });
		this._ListView.ContextMenuStrip = this._ContextList;
		this._ListView.FullRowSelect = true;
		this._ListView.HideSelection = false;
		this._ListView.Location = new System.Drawing.Point(12, 58);
		this._ListView.Name = "_ListView";
		this._ListView.Size = new System.Drawing.Size(400, 150);
		this._ListView.TabIndex = 1;
		this._ListView.UseCompatibleStateImageBehavior = false;
		this._ListView.View = System.Windows.Forms.View.Details;
		this._ListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this._List_MouseDown);
		//
		// _ColumnName
		//
		this._ColumnName.Text = "Name";
		this._ColumnName.Width = 150;
		//
		// _ColumnColor
		//
		this._ColumnColor.Text = "Color";
		this._ColumnColor.Width = 120;
		//
		// _ColumnPrice
		//
		this._ColumnPrice.Text = "Price";
		this._ColumnPrice.Width = 90;
		//
		// _LabelListMulti
		//
		this._LabelListMulti.AutoSize = true;
		this._LabelListMulti.Location = new System.Drawing.Point(430, 40);
		this._LabelListMulti.Name = "_LabelListMulti";
		this._LabelListMulti.TabIndex = 2;
		this._LabelListMulti.Text = "List &box (multiple selection):";
		//
		// _ListMulti
		//
		this._ListMulti.ContextMenuStrip = this._ContextList;
		this._ListMulti.FormattingEnabled = true;
		this._ListMulti.IntegralHeight = false;
		this._ListMulti.ItemHeight = 15;
		this._ListMulti.Location = new System.Drawing.Point(430, 58);
		this._ListMulti.Name = "_ListMulti";
		this._ListMulti.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this._ListMulti.Size = new System.Drawing.Size(200, 150);
		this._ListMulti.TabIndex = 3;
		this._ListMulti.MouseDown += new System.Windows.Forms.MouseEventHandler(this._List_MouseDown);
		//
		// _LabelListSingle
		//
		this._LabelListSingle.AutoSize = true;
		this._LabelListSingle.Location = new System.Drawing.Point(648, 40);
		this._LabelListSingle.Name = "_LabelListSingle";
		this._LabelListSingle.TabIndex = 4;
		this._LabelListSingle.Text = "List box (&single selection):";
		//
		// _ListSingle
		//
		this._ListSingle.ContextMenuStrip = this._ContextList;
		this._ListSingle.FormattingEnabled = true;
		this._ListSingle.IntegralHeight = false;
		this._ListSingle.ItemHeight = 15;
		this._ListSingle.Location = new System.Drawing.Point(648, 58);
		this._ListSingle.Name = "_ListSingle";
		this._ListSingle.Size = new System.Drawing.Size(200, 150);
		this._ListSingle.TabIndex = 5;
		this._ListSingle.MouseDown += new System.Windows.Forms.MouseEventHandler(this._List_MouseDown);
		//
		// _ChkHandleDelete
		//
		this._ChkHandleDelete.AutoSize = true;
		this._ChkHandleDelete.Location = new System.Drawing.Point(12, 12);
		this._ChkHandleDelete.Name = "_ChkHandleDelete";
		this._ChkHandleDelete.TabIndex = 6;
		this._ChkHandleDelete.Text = "&Application handles Delete in the lists";
		this._ChkHandleDelete.UseVisualStyleBackColor = true;
		//
		// _ChkConfirmDelete
		//
		this._ChkConfirmDelete.AutoSize = true;
		this._ChkConfirmDelete.Location = new System.Drawing.Point(430, 12);
		this._ChkConfirmDelete.Name = "_ChkConfirmDelete";
		this._ChkConfirmDelete.TabIndex = 7;
		this._ChkConfirmDelete.Text = "Confirm row deletion in the grid";
		this._ChkConfirmDelete.UseVisualStyleBackColor = true;
		//
		// _LabelGrid
		//
		this._LabelGrid.AutoSize = true;
		this._LabelGrid.Location = new System.Drawing.Point(12, 216);
		this._LabelGrid.Name = "_LabelGrid";
		this._LabelGrid.TabIndex = 8;
		this._LabelGrid.Text = "Data &grid view (bound to a data table; Created is read-only):";
		//
		// _Grid
		//
		this._Grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
		this._Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this._Grid.ContextMenuStrip = this._ContextList;
		this._Grid.Location = new System.Drawing.Point(12, 234);
		this._Grid.Name = "_Grid";
		this._Grid.Size = new System.Drawing.Size(952, 186);
		this._Grid.TabIndex = 9;
		this._Grid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this._Grid_UserDeletingRow);
		this._Grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._Grid_DataError);
		this._Grid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._Grid_CellMouseDown);
		//
		// _PageCustom
		//
		this._PageCustom.Controls.Add(this._LabelNotes);
		this._PageCustom.Controls.Add(this._NoteBoard);
		this._PageCustom.Controls.Add(this._TextNewNote);
		this._PageCustom.Controls.Add(this._ButtonAddNote);
		this._PageCustom.Controls.Add(this._LabelPad);
		this._PageCustom.Controls.Add(this._Pad);
		this._PageCustom.Controls.Add(this._ChkUpperCasePaste);
		this._PageCustom.Location = new System.Drawing.Point(4, 24);
		this._PageCustom.Name = "_PageCustom";
		this._PageCustom.Padding = new System.Windows.Forms.Padding(3);
		this._PageCustom.Size = new System.Drawing.Size(976, 432);
		this._PageCustom.TabIndex = 2;
		this._PageCustom.Text = "Custom";
		this._PageCustom.UseVisualStyleBackColor = true;
		//
		// _LabelNotes
		//
		this._LabelNotes.AutoSize = true;
		this._LabelNotes.Location = new System.Drawing.Point(12, 12);
		this._LabelNotes.Name = "_LabelNotes";
		this._LabelNotes.TabIndex = 0;
		this._LabelNotes.Text = "&Note board (implements IEditable):";
		//
		// _NoteBoard
		//
		this._NoteBoard.Location = new System.Drawing.Point(12, 30);
		this._NoteBoard.Name = "_NoteBoard";
		this._NoteBoard.Size = new System.Drawing.Size(300, 200);
		this._NoteBoard.TabIndex = 1;
		//
		// _TextNewNote
		//
		this._TextNewNote.Location = new System.Drawing.Point(12, 240);
		this._TextNewNote.Name = "_TextNewNote";
		this._TextNewNote.PlaceholderText = "New note";
		this._TextNewNote.Size = new System.Drawing.Size(218, 23);
		this._TextNewNote.TabIndex = 2;
		//
		// _ButtonAddNote
		//
		this._ButtonAddNote.Location = new System.Drawing.Point(236, 239);
		this._ButtonAddNote.Name = "_ButtonAddNote";
		this._ButtonAddNote.Size = new System.Drawing.Size(76, 25);
		this._ButtonAddNote.TabIndex = 3;
		this._ButtonAddNote.Text = "A&dd";
		this._ButtonAddNote.UseVisualStyleBackColor = true;
		this._ButtonAddNote.Click += new System.EventHandler(this._ButtonAddNote_Click);
		//
		// _LabelPad
		//
		this._LabelPad.AutoSize = true;
		this._LabelPad.Location = new System.Drawing.Point(330, 12);
		this._LabelPad.Name = "_LabelPad";
		this._LabelPad.TabIndex = 4;
		this._LabelPad.Text = "&Upper-case pad (implements IEditableBearing):";
		//
		// _Pad
		//
		this._Pad.Location = new System.Drawing.Point(330, 30);
		this._Pad.Name = "_Pad";
		this._Pad.Size = new System.Drawing.Size(300, 200);
		this._Pad.TabIndex = 5;
		//
		// _ChkUpperCasePaste
		//
		this._ChkUpperCasePaste.AutoSize = true;
		this._ChkUpperCasePaste.Checked = true;
		this._ChkUpperCasePaste.CheckState = System.Windows.Forms.CheckState.Checked;
		this._ChkUpperCasePaste.Location = new System.Drawing.Point(330, 242);
		this._ChkUpperCasePaste.Name = "_ChkUpperCasePaste";
		this._ChkUpperCasePaste.TabIndex = 6;
		this._ChkUpperCasePaste.Text = "Paste in &upper case (pad provides an IEditable)";
		this._ChkUpperCasePaste.UseVisualStyleBackColor = true;
		this._ChkUpperCasePaste.CheckedChanged += new System.EventHandler(this._ChkUpperCasePaste_CheckedChanged);
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
		this._ListLog.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this._ListLog.Size = new System.Drawing.Size(984, 194);
		this._ListLog.TabIndex = 1;
		//
		// _PanelLogHeader
		//
		this._PanelLogHeader.Controls.Add(this._LabelLog);
		this._PanelLogHeader.Controls.Add(this._ButtonCopyLog);
		this._PanelLogHeader.Controls.Add(this._ButtonClearLog);
		this._PanelLogHeader.Dock = System.Windows.Forms.DockStyle.Top;
		this._PanelLogHeader.Location = new System.Drawing.Point(0, 0);
		this._PanelLogHeader.Name = "_PanelLogHeader";
		this._PanelLogHeader.Size = new System.Drawing.Size(984, 32);
		this._PanelLogHeader.TabIndex = 0;
		//
		// _LabelLog
		//
		this._LabelLog.AutoSize = true;
		this._LabelLog.Location = new System.Drawing.Point(8, 9);
		this._LabelLog.Name = "_LabelLog";
		this._LabelLog.TabIndex = 0;
		this._LabelLog.Text = "Event log (Executing, Executed, EditStateChanged):";
		//
		// _ButtonCopyLog
		//
		this._ButtonCopyLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this._ButtonCopyLog.Location = new System.Drawing.Point(810, 3);
		this._ButtonCopyLog.Name = "_ButtonCopyLog";
		this._ButtonCopyLog.Size = new System.Drawing.Size(80, 26);
		this._ButtonCopyLog.TabIndex = 1;
		this._ButtonCopyLog.Text = "C&opy Log";
		this._ButtonCopyLog.UseVisualStyleBackColor = true;
		this._ButtonCopyLog.Click += new System.EventHandler(this._ButtonCopyLog_Click);
		//
		// _ButtonClearLog
		//
		this._ButtonClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this._ButtonClearLog.Location = new System.Drawing.Point(896, 3);
		this._ButtonClearLog.Name = "_ButtonClearLog";
		this._ButtonClearLog.Size = new System.Drawing.Size(80, 26);
		this._ButtonClearLog.TabIndex = 2;
		this._ButtonClearLog.Text = "C&lear Log";
		this._ButtonClearLog.UseVisualStyleBackColor = true;
		this._ButtonClearLog.Click += new System.EventHandler(this._ButtonClearLog_Click);
		//
		// MainWindow
		//
		this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(984, 761);
		this.Controls.Add(this._SplitMain);
		this.Controls.Add(this._StatusMain);
		this.Controls.Add(this._ToolMain);
		this.Controls.Add(this._MenuMain);
		this.MainMenuStrip = this._MenuMain;
		this.MinimumSize = new System.Drawing.Size(800, 600);
		this.Name = "MainWindow";
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Spellbook Edit Commands Sample";
		this._PanelLogHeader.ResumeLayout(false);
		this._PanelLogHeader.PerformLayout();
		this._PageCustom.ResumeLayout(false);
		this._PageCustom.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this._Grid)).EndInit();
		this._PageLists.ResumeLayout(false);
		this._PageLists.PerformLayout();
		this._PageText.ResumeLayout(false);
		this._PageText.PerformLayout();
		this._TabMain.ResumeLayout(false);
		this._TabMain.PerformLayout();
		this._SplitMain.Panel1.ResumeLayout(false);
		this._SplitMain.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this._SplitMain)).EndInit();
		this._SplitMain.ResumeLayout(false);
		this._ContextList.ResumeLayout(false);
		this._ContextList.PerformLayout();
		this._StatusMain.ResumeLayout(false);
		this._StatusMain.PerformLayout();
		this._ToolMain.ResumeLayout(false);
		this._ToolMain.PerformLayout();
		this._MenuMain.ResumeLayout(false);
		this._MenuMain.PerformLayout();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	#endregion

	private Robbiblubber.Spellbook.Controls.EditCommandProvider _Provider;
	private System.Windows.Forms.MenuStrip _MenuMain;
	private System.Windows.Forms.ToolStripMenuItem _MenuFile;
	private System.Windows.Forms.ToolStripMenuItem _MenuFileExit;
	private System.Windows.Forms.ToolStripMenuItem _MenuEdit;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditUndo;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditRedo;
	private System.Windows.Forms.ToolStripSeparator _MenuEditSeparator1;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditCut;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditCopy;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditPaste;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditDelete;
	private System.Windows.Forms.ToolStripSeparator _MenuEditSeparator2;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditSelectAll;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditInvertSelection;
	private System.Windows.Forms.ToolStripMenuItem _MenuEditClearSelection;
	private System.Windows.Forms.ToolStripMenuItem _MenuOptions;
	private System.Windows.Forms.ToolStripMenuItem _MenuOptionsAutoUpdate;
	private System.Windows.Forms.ToolStripMenuItem _MenuOptionsUpdateNow;
	private System.Windows.Forms.ToolStrip _ToolMain;
	private System.Windows.Forms.ToolStripButton _ToolUndo;
	private System.Windows.Forms.ToolStripButton _ToolRedo;
	private System.Windows.Forms.ToolStripSeparator _ToolSeparator1;
	private System.Windows.Forms.ToolStripButton _ToolCut;
	private System.Windows.Forms.ToolStripButton _ToolCopy;
	private System.Windows.Forms.ToolStripButton _ToolPaste;
	private System.Windows.Forms.ToolStripButton _ToolDelete;
	private System.Windows.Forms.ToolStripSeparator _ToolSeparator2;
	private System.Windows.Forms.ToolStripButton _ToolSelectAll;
	private System.Windows.Forms.StatusStrip _StatusMain;
	private System.Windows.Forms.ToolStripStatusLabel _LabelTarget;
	private System.Windows.Forms.ToolStripStatusLabel _LabelState;
	private System.Windows.Forms.ContextMenuStrip _ContextList;
	private System.Windows.Forms.ToolStripMenuItem _ContextListCut;
	private System.Windows.Forms.ToolStripMenuItem _ContextListCopy;
	private System.Windows.Forms.ToolStripMenuItem _ContextListPaste;
	private System.Windows.Forms.ToolStripMenuItem _ContextListDelete;
	private System.Windows.Forms.ToolStripSeparator _ContextListSeparator;
	private System.Windows.Forms.ToolStripMenuItem _ContextListSelectAll;
	private System.Windows.Forms.ToolStripMenuItem _ContextListInvertSelection;
	private System.Windows.Forms.ToolStripMenuItem _ContextListClearSelection;
	private System.Windows.Forms.SplitContainer _SplitMain;
	private System.Windows.Forms.TabControl _TabMain;
	private System.Windows.Forms.TabPage _PageText;
	private System.Windows.Forms.Label _LabelMultiline;
	private System.Windows.Forms.TextBox _TextMultiline;
	private System.Windows.Forms.Label _LabelPassword;
	private System.Windows.Forms.TextBox _TextPassword;
	private System.Windows.Forms.Label _LabelExcluded;
	private System.Windows.Forms.TextBox _TextExcluded;
	private System.Windows.Forms.Label _LabelRich;
	private System.Windows.Forms.RichTextBox _RichText;
	private System.Windows.Forms.Label _LabelComboEditable;
	private System.Windows.Forms.ComboBox _ComboEditable;
	private System.Windows.Forms.Label _LabelComboList;
	private System.Windows.Forms.ComboBox _ComboList;
	private System.Windows.Forms.Label _LabelReadOnly;
	private System.Windows.Forms.TextBox _TextReadOnly;
	private System.Windows.Forms.TabPage _PageLists;
	private System.Windows.Forms.Label _LabelListView;
	private System.Windows.Forms.ListView _ListView;
	private System.Windows.Forms.ColumnHeader _ColumnName;
	private System.Windows.Forms.ColumnHeader _ColumnColor;
	private System.Windows.Forms.ColumnHeader _ColumnPrice;
	private System.Windows.Forms.Label _LabelListMulti;
	private System.Windows.Forms.ListBox _ListMulti;
	private System.Windows.Forms.Label _LabelListSingle;
	private System.Windows.Forms.ListBox _ListSingle;
	private System.Windows.Forms.CheckBox _ChkHandleDelete;
	private System.Windows.Forms.CheckBox _ChkConfirmDelete;
	private System.Windows.Forms.Label _LabelGrid;
	private System.Windows.Forms.DataGridView _Grid;
	private System.Windows.Forms.TabPage _PageCustom;
	private System.Windows.Forms.Label _LabelNotes;
	private Robbiblubber.Spellbook.Controls.Samples.EditCommands.NoteBoardControl _NoteBoard;
	private System.Windows.Forms.TextBox _TextNewNote;
	private System.Windows.Forms.Button _ButtonAddNote;
	private System.Windows.Forms.Label _LabelPad;
	private Robbiblubber.Spellbook.Controls.Samples.EditCommands.UpperCasePadControl _Pad;
	private System.Windows.Forms.CheckBox _ChkUpperCasePaste;
	private System.Windows.Forms.ListBox _ListLog;
	private System.Windows.Forms.Panel _PanelLogHeader;
	private System.Windows.Forms.Label _LabelLog;
	private System.Windows.Forms.Button _ButtonCopyLog;
	private System.Windows.Forms.Button _ButtonClearLog;
}
