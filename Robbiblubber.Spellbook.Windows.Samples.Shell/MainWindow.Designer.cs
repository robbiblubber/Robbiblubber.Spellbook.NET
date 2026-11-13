namespace Robbiblubber.Spellbook.Windows.Samples.Shell;



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
		_LabelScope = new Label();
		_ComboScope = new ComboBox();
		_LabelArgs = new Label();
		_Tabs = new TabControl();
		_TabShortcut = new TabPage();
		_TabFileType = new TabPage();
		_TabApplication = new TabPage();
		_TabIni = new TabPage();
		_LabelLinkFile = new Label();
		_TextLinkFile = new TextBox();
		_ButtonBrowseLinkFile = new Button();
		_LabelTarget = new Label();
		_TextTarget = new TextBox();
		_ButtonBrowseTarget = new Button();
		_LabelLinkArguments = new Label();
		_TextLinkArguments = new TextBox();
		_LabelLinkDescription = new Label();
		_TextLinkDescription = new TextBox();
		_LabelLinkIcon = new Label();
		_TextLinkIcon = new TextBox();
		_ButtonBrowseLinkIcon = new Button();
		_LabelLinkIconIndex = new Label();
		_NumLinkIconIndex = new NumericUpDown();
		_LabelWindowStyle = new Label();
		_ComboWindowStyle = new ComboBox();
		_CheckRunAsAdmin = new CheckBox();
		_ButtonCreateLink = new Button();
		_ButtonReadLink = new Button();
		_ButtonDeleteLink = new Button();
		_ButtonLinkDesktop = new Button();
		_ButtonLinkStartMenu = new Button();
		_LabelExtension = new Label();
		_TextExtension = new TextBox();
		_LabelProgId = new Label();
		_TextProgId = new TextBox();
		_LabelTypeDescription = new Label();
		_TextTypeDescription = new TextBox();
		_LabelApplication = new Label();
		_TextApplication = new TextBox();
		_ButtonBrowseApplication = new Button();
		_CheckPrintVerb = new CheckBox();
		_ButtonRegisterType = new Button();
		_ButtonUnregisterType = new Button();
		_ButtonCheckType = new Button();
		_ButtonCreateFile = new Button();
		_ButtonDefaultApps = new Button();
		_ButtonRegisterDefaultApp = new Button();
		_ButtonUnregisterDefaultApp = new Button();
		_LabelAppKey = new Label();
		_TextAppKey = new TextBox();
		_LabelAppName = new Label();
		_TextAppName = new TextBox();
		_LabelAppVersion = new Label();
		_TextAppVersion = new TextBox();
		_LabelPublisher = new Label();
		_TextPublisher = new TextBox();
		_LabelUninstall = new Label();
		_TextUninstall = new TextBox();
		_CheckModify = new CheckBox();
		_ButtonRegisterApp = new Button();
		_ButtonGetApp = new Button();
		_ButtonUnregisterApp = new Button();
		_ButtonAppsSettings = new Button();
		_LabelIniFile = new Label();
		_TextIniFile = new TextBox();
		_ButtonBrowseIniFile = new Button();
		_LabelSection = new Label();
		_TextSection = new TextBox();
		_LabelKey = new Label();
		_TextKey = new TextBox();
		_LabelValue = new Label();
		_TextValue = new TextBox();
		_CheckAutoSave = new CheckBox();
		_ButtonOpenIni = new Button();
		_ButtonListIni = new Button();
		_ButtonSaveIni = new Button();
		_ButtonReloadIni = new Button();
		_ButtonEditIni = new Button();
		_ButtonGetValue = new Button();
		_ButtonSetValue = new Button();
		_ButtonDeleteKey = new Button();
		_ButtonDeleteSection = new Button();
		_ListLog = new ListBox();
		_ButtonClear = new Button();
		_Tabs.SuspendLayout();
		_TabShortcut.SuspendLayout();
		_TabFileType.SuspendLayout();
		_TabApplication.SuspendLayout();
		_TabIni.SuspendLayout();
		((System.ComponentModel.ISupportInitialize) _NumLinkIconIndex).BeginInit();
		SuspendLayout();
		//
		// _LabelScope
		//
		_LabelScope.AutoSize = true;
		_LabelScope.Location = new Point(12, 15);
		_LabelScope.Size = new Size(39, 15);
		_LabelScope.Text = "Scope:";
		_LabelScope.Name = "_LabelScope";
		_LabelScope.TabIndex = 0;
		//
		// _ComboScope
		//
		_ComboScope.DropDownStyle = ComboBoxStyle.DropDownList;
		_ComboScope.Location = new Point(88, 12);
		_ComboScope.Size = new Size(200, 23);
		_ComboScope.Name = "_ComboScope";
		_ComboScope.TabIndex = 1;
		//
		// _LabelArgs
		//
		_LabelArgs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_LabelArgs.Location = new Point(300, 15);
		_LabelArgs.Size = new Size(476, 15);
		_LabelArgs.TextAlign = ContentAlignment.TopRight;
		_LabelArgs.Text = "Arguments: (none)";
		_LabelArgs.Name = "_LabelArgs";
		_LabelArgs.TabIndex = 2;
		//
		// _Tabs
		//
		_Tabs.Controls.Add(_TabShortcut);
		_Tabs.Controls.Add(_TabFileType);
		_Tabs.Controls.Add(_TabApplication);
		_Tabs.Controls.Add(_TabIni);
		_Tabs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_Tabs.Location = new Point(12, 44);
		_Tabs.Size = new Size(776, 300);
		_Tabs.Name = "_Tabs";
		_Tabs.TabIndex = 3;
		//
		// _TabShortcut
		//
		_TabShortcut.Controls.Add(_LabelLinkFile);
		_TabShortcut.Controls.Add(_TextLinkFile);
		_TabShortcut.Controls.Add(_ButtonBrowseLinkFile);
		_TabShortcut.Controls.Add(_LabelTarget);
		_TabShortcut.Controls.Add(_TextTarget);
		_TabShortcut.Controls.Add(_ButtonBrowseTarget);
		_TabShortcut.Controls.Add(_LabelLinkArguments);
		_TabShortcut.Controls.Add(_TextLinkArguments);
		_TabShortcut.Controls.Add(_LabelLinkDescription);
		_TabShortcut.Controls.Add(_TextLinkDescription);
		_TabShortcut.Controls.Add(_LabelLinkIcon);
		_TabShortcut.Controls.Add(_TextLinkIcon);
		_TabShortcut.Controls.Add(_ButtonBrowseLinkIcon);
		_TabShortcut.Controls.Add(_LabelLinkIconIndex);
		_TabShortcut.Controls.Add(_NumLinkIconIndex);
		_TabShortcut.Controls.Add(_LabelWindowStyle);
		_TabShortcut.Controls.Add(_ComboWindowStyle);
		_TabShortcut.Controls.Add(_CheckRunAsAdmin);
		_TabShortcut.Controls.Add(_ButtonCreateLink);
		_TabShortcut.Controls.Add(_ButtonReadLink);
		_TabShortcut.Controls.Add(_ButtonDeleteLink);
		_TabShortcut.Controls.Add(_ButtonLinkDesktop);
		_TabShortcut.Controls.Add(_ButtonLinkStartMenu);
		_TabShortcut.Location = new Point(4, 24);
		_TabShortcut.Padding = new Padding(3);
		_TabShortcut.Size = new Size(768, 272);
		_TabShortcut.Text = "Shortcut";
		_TabShortcut.UseVisualStyleBackColor = true;
		_TabShortcut.Name = "_TabShortcut";
		//
		// _TabFileType
		//
		_TabFileType.Controls.Add(_LabelExtension);
		_TabFileType.Controls.Add(_TextExtension);
		_TabFileType.Controls.Add(_LabelProgId);
		_TabFileType.Controls.Add(_TextProgId);
		_TabFileType.Controls.Add(_LabelTypeDescription);
		_TabFileType.Controls.Add(_TextTypeDescription);
		_TabFileType.Controls.Add(_LabelApplication);
		_TabFileType.Controls.Add(_TextApplication);
		_TabFileType.Controls.Add(_ButtonBrowseApplication);
		_TabFileType.Controls.Add(_CheckPrintVerb);
		_TabFileType.Controls.Add(_ButtonRegisterType);
		_TabFileType.Controls.Add(_ButtonUnregisterType);
		_TabFileType.Controls.Add(_ButtonCheckType);
		_TabFileType.Controls.Add(_ButtonCreateFile);
		_TabFileType.Controls.Add(_ButtonDefaultApps);
		_TabFileType.Controls.Add(_ButtonRegisterDefaultApp);
		_TabFileType.Controls.Add(_ButtonUnregisterDefaultApp);
		_TabFileType.Location = new Point(4, 24);
		_TabFileType.Padding = new Padding(3);
		_TabFileType.Size = new Size(768, 272);
		_TabFileType.Text = "File type";
		_TabFileType.UseVisualStyleBackColor = true;
		_TabFileType.Name = "_TabFileType";
		//
		// _TabApplication
		//
		_TabApplication.Controls.Add(_LabelAppKey);
		_TabApplication.Controls.Add(_TextAppKey);
		_TabApplication.Controls.Add(_LabelAppName);
		_TabApplication.Controls.Add(_TextAppName);
		_TabApplication.Controls.Add(_LabelAppVersion);
		_TabApplication.Controls.Add(_TextAppVersion);
		_TabApplication.Controls.Add(_LabelPublisher);
		_TabApplication.Controls.Add(_TextPublisher);
		_TabApplication.Controls.Add(_LabelUninstall);
		_TabApplication.Controls.Add(_TextUninstall);
		_TabApplication.Controls.Add(_CheckModify);
		_TabApplication.Controls.Add(_ButtonRegisterApp);
		_TabApplication.Controls.Add(_ButtonGetApp);
		_TabApplication.Controls.Add(_ButtonUnregisterApp);
		_TabApplication.Controls.Add(_ButtonAppsSettings);
		_TabApplication.Location = new Point(4, 24);
		_TabApplication.Padding = new Padding(3);
		_TabApplication.Size = new Size(768, 272);
		_TabApplication.Text = "Application";
		_TabApplication.UseVisualStyleBackColor = true;
		_TabApplication.Name = "_TabApplication";
		//
		// _TabIni
		//
		_TabIni.Controls.Add(_LabelIniFile);
		_TabIni.Controls.Add(_TextIniFile);
		_TabIni.Controls.Add(_ButtonBrowseIniFile);
		_TabIni.Controls.Add(_LabelSection);
		_TabIni.Controls.Add(_TextSection);
		_TabIni.Controls.Add(_LabelKey);
		_TabIni.Controls.Add(_TextKey);
		_TabIni.Controls.Add(_LabelValue);
		_TabIni.Controls.Add(_TextValue);
		_TabIni.Controls.Add(_CheckAutoSave);
		_TabIni.Controls.Add(_ButtonOpenIni);
		_TabIni.Controls.Add(_ButtonListIni);
		_TabIni.Controls.Add(_ButtonSaveIni);
		_TabIni.Controls.Add(_ButtonReloadIni);
		_TabIni.Controls.Add(_ButtonEditIni);
		_TabIni.Controls.Add(_ButtonGetValue);
		_TabIni.Controls.Add(_ButtonSetValue);
		_TabIni.Controls.Add(_ButtonDeleteKey);
		_TabIni.Controls.Add(_ButtonDeleteSection);
		_TabIni.Location = new Point(4, 24);
		_TabIni.Padding = new Padding(3);
		_TabIni.Size = new Size(768, 272);
		_TabIni.Text = "Ini file";
		_TabIni.UseVisualStyleBackColor = true;
		_TabIni.Name = "_TabIni";
		//
		// _LabelLinkFile
		//
		_LabelLinkFile.AutoSize = true;
		_LabelLinkFile.Location = new Point(12, 15);
		_LabelLinkFile.Text = "Shortcut file:";
		_LabelLinkFile.Name = "_LabelLinkFile";
		_LabelLinkFile.TabIndex = 0;
		//
		// _TextLinkFile
		//
		_TextLinkFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextLinkFile.Location = new Point(130, 12);
		_TextLinkFile.Size = new Size(580, 23);
		_TextLinkFile.Name = "_TextLinkFile";
		_TextLinkFile.TabIndex = 1;
		//
		// _ButtonBrowseLinkFile
		//
		_ButtonBrowseLinkFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseLinkFile.Location = new Point(720, 11);
		_ButtonBrowseLinkFile.Size = new Size(36, 25);
		_ButtonBrowseLinkFile.Text = "...";
		_ButtonBrowseLinkFile.UseVisualStyleBackColor = true;
		_ButtonBrowseLinkFile.Name = "_ButtonBrowseLinkFile";
		_ButtonBrowseLinkFile.TabIndex = 2;
		_ButtonBrowseLinkFile.Click += _ButtonBrowseLinkFile_Click;
		//
		// _LabelTarget
		//
		_LabelTarget.AutoSize = true;
		_LabelTarget.Location = new Point(12, 44);
		_LabelTarget.Text = "Target:";
		_LabelTarget.Name = "_LabelTarget";
		_LabelTarget.TabIndex = 3;
		//
		// _TextTarget
		//
		_TextTarget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextTarget.Location = new Point(130, 41);
		_TextTarget.Size = new Size(580, 23);
		_TextTarget.Name = "_TextTarget";
		_TextTarget.TabIndex = 4;
		//
		// _ButtonBrowseTarget
		//
		_ButtonBrowseTarget.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseTarget.Location = new Point(720, 40);
		_ButtonBrowseTarget.Size = new Size(36, 25);
		_ButtonBrowseTarget.Text = "...";
		_ButtonBrowseTarget.UseVisualStyleBackColor = true;
		_ButtonBrowseTarget.Name = "_ButtonBrowseTarget";
		_ButtonBrowseTarget.TabIndex = 5;
		_ButtonBrowseTarget.Click += _ButtonBrowseTarget_Click;
		//
		// _LabelLinkArguments
		//
		_LabelLinkArguments.AutoSize = true;
		_LabelLinkArguments.Location = new Point(12, 73);
		_LabelLinkArguments.Text = "Arguments:";
		_LabelLinkArguments.Name = "_LabelLinkArguments";
		_LabelLinkArguments.TabIndex = 6;
		//
		// _TextLinkArguments
		//
		_TextLinkArguments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextLinkArguments.Location = new Point(130, 70);
		_TextLinkArguments.Size = new Size(616, 23);
		_TextLinkArguments.Name = "_TextLinkArguments";
		_TextLinkArguments.TabIndex = 7;
		//
		// _LabelLinkDescription
		//
		_LabelLinkDescription.AutoSize = true;
		_LabelLinkDescription.Location = new Point(12, 102);
		_LabelLinkDescription.Text = "Description:";
		_LabelLinkDescription.Name = "_LabelLinkDescription";
		_LabelLinkDescription.TabIndex = 8;
		//
		// _TextLinkDescription
		//
		_TextLinkDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextLinkDescription.Location = new Point(130, 99);
		_TextLinkDescription.Size = new Size(616, 23);
		_TextLinkDescription.Name = "_TextLinkDescription";
		_TextLinkDescription.TabIndex = 9;
		//
		// _LabelLinkIcon
		//
		_LabelLinkIcon.AutoSize = true;
		_LabelLinkIcon.Location = new Point(12, 131);
		_LabelLinkIcon.Text = "Icon file:";
		_LabelLinkIcon.Name = "_LabelLinkIcon";
		_LabelLinkIcon.TabIndex = 10;
		//
		// _TextLinkIcon
		//
		_TextLinkIcon.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextLinkIcon.Location = new Point(130, 128);
		_TextLinkIcon.Size = new Size(580, 23);
		_TextLinkIcon.Name = "_TextLinkIcon";
		_TextLinkIcon.TabIndex = 11;
		//
		// _ButtonBrowseLinkIcon
		//
		_ButtonBrowseLinkIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseLinkIcon.Location = new Point(720, 127);
		_ButtonBrowseLinkIcon.Size = new Size(36, 25);
		_ButtonBrowseLinkIcon.Text = "...";
		_ButtonBrowseLinkIcon.UseVisualStyleBackColor = true;
		_ButtonBrowseLinkIcon.Name = "_ButtonBrowseLinkIcon";
		_ButtonBrowseLinkIcon.TabIndex = 12;
		_ButtonBrowseLinkIcon.Click += _ButtonBrowseLinkIcon_Click;
		//
		// _LabelLinkIconIndex
		//
		_LabelLinkIconIndex.AutoSize = true;
		_LabelLinkIconIndex.Location = new Point(12, 160);
		_LabelLinkIconIndex.Text = "Icon index:";
		_LabelLinkIconIndex.Name = "_LabelLinkIconIndex";
		_LabelLinkIconIndex.TabIndex = 13;
		//
		// _NumLinkIconIndex
		//
		_NumLinkIconIndex.Location = new Point(130, 157);
		_NumLinkIconIndex.Size = new Size(80, 23);
		_NumLinkIconIndex.Name = "_NumLinkIconIndex";
		_NumLinkIconIndex.TabIndex = 14;
		//
		// _LabelWindowStyle
		//
		_LabelWindowStyle.AutoSize = true;
		_LabelWindowStyle.Location = new Point(12, 189);
		_LabelWindowStyle.Text = "Window style:";
		_LabelWindowStyle.Name = "_LabelWindowStyle";
		_LabelWindowStyle.TabIndex = 15;
		//
		// _ComboWindowStyle
		//
		_ComboWindowStyle.DropDownStyle = ComboBoxStyle.DropDownList;
		_ComboWindowStyle.Location = new Point(130, 186);
		_ComboWindowStyle.Size = new Size(150, 23);
		_ComboWindowStyle.Name = "_ComboWindowStyle";
		_ComboWindowStyle.TabIndex = 16;
		//
		// _CheckRunAsAdmin
		//
		_CheckRunAsAdmin.AutoSize = true;
		_CheckRunAsAdmin.Location = new Point(300, 188);
		_CheckRunAsAdmin.Text = "Run as administrator";
		_CheckRunAsAdmin.UseVisualStyleBackColor = true;
		_CheckRunAsAdmin.Name = "_CheckRunAsAdmin";
		_CheckRunAsAdmin.TabIndex = 17;
		//
		// _ButtonCreateLink
		//
		_ButtonCreateLink.Location = new Point(130, 222);
		_ButtonCreateLink.Size = new Size(100, 27);
		_ButtonCreateLink.Text = "Create";
		_ButtonCreateLink.UseVisualStyleBackColor = true;
		_ButtonCreateLink.Name = "_ButtonCreateLink";
		_ButtonCreateLink.TabIndex = 18;
		_ButtonCreateLink.Click += _ButtonCreateLink_Click;
		//
		// _ButtonReadLink
		//
		_ButtonReadLink.Location = new Point(238, 222);
		_ButtonReadLink.Size = new Size(100, 27);
		_ButtonReadLink.Text = "Read";
		_ButtonReadLink.UseVisualStyleBackColor = true;
		_ButtonReadLink.Name = "_ButtonReadLink";
		_ButtonReadLink.TabIndex = 19;
		_ButtonReadLink.Click += _ButtonReadLink_Click;
		//
		// _ButtonDeleteLink
		//
		_ButtonDeleteLink.Location = new Point(346, 222);
		_ButtonDeleteLink.Size = new Size(100, 27);
		_ButtonDeleteLink.Text = "Delete";
		_ButtonDeleteLink.UseVisualStyleBackColor = true;
		_ButtonDeleteLink.Name = "_ButtonDeleteLink";
		_ButtonDeleteLink.TabIndex = 20;
		_ButtonDeleteLink.Click += _ButtonDeleteLink_Click;
		//
		// _ButtonLinkDesktop
		//
		_ButtonLinkDesktop.Location = new Point(454, 222);
		_ButtonLinkDesktop.Size = new Size(110, 27);
		_ButtonLinkDesktop.Text = "Desktop path";
		_ButtonLinkDesktop.UseVisualStyleBackColor = true;
		_ButtonLinkDesktop.Name = "_ButtonLinkDesktop";
		_ButtonLinkDesktop.TabIndex = 21;
		_ButtonLinkDesktop.Click += _ButtonLinkDesktop_Click;
		//
		// _ButtonLinkStartMenu
		//
		_ButtonLinkStartMenu.Location = new Point(572, 222);
		_ButtonLinkStartMenu.Size = new Size(120, 27);
		_ButtonLinkStartMenu.Text = "Start menu path";
		_ButtonLinkStartMenu.UseVisualStyleBackColor = true;
		_ButtonLinkStartMenu.Name = "_ButtonLinkStartMenu";
		_ButtonLinkStartMenu.TabIndex = 22;
		_ButtonLinkStartMenu.Click += _ButtonLinkStartMenu_Click;
		//
		// _LabelExtension
		//
		_LabelExtension.AutoSize = true;
		_LabelExtension.Location = new Point(12, 15);
		_LabelExtension.Text = "Extension:";
		_LabelExtension.Name = "_LabelExtension";
		_LabelExtension.TabIndex = 0;
		//
		// _TextExtension
		//
		_TextExtension.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextExtension.Location = new Point(130, 12);
		_TextExtension.Size = new Size(616, 23);
		_TextExtension.Name = "_TextExtension";
		_TextExtension.TabIndex = 1;
		//
		// _LabelProgId
		//
		_LabelProgId.AutoSize = true;
		_LabelProgId.Location = new Point(12, 44);
		_LabelProgId.Text = "Program ID:";
		_LabelProgId.Name = "_LabelProgId";
		_LabelProgId.TabIndex = 2;
		//
		// _TextProgId
		//
		_TextProgId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextProgId.Location = new Point(130, 41);
		_TextProgId.Size = new Size(616, 23);
		_TextProgId.Name = "_TextProgId";
		_TextProgId.TabIndex = 3;
		//
		// _LabelTypeDescription
		//
		_LabelTypeDescription.AutoSize = true;
		_LabelTypeDescription.Location = new Point(12, 73);
		_LabelTypeDescription.Text = "Description:";
		_LabelTypeDescription.Name = "_LabelTypeDescription";
		_LabelTypeDescription.TabIndex = 4;
		//
		// _TextTypeDescription
		//
		_TextTypeDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextTypeDescription.Location = new Point(130, 70);
		_TextTypeDescription.Size = new Size(616, 23);
		_TextTypeDescription.Name = "_TextTypeDescription";
		_TextTypeDescription.TabIndex = 5;
		//
		// _LabelApplication
		//
		_LabelApplication.AutoSize = true;
		_LabelApplication.Location = new Point(12, 102);
		_LabelApplication.Text = "Application:";
		_LabelApplication.Name = "_LabelApplication";
		_LabelApplication.TabIndex = 6;
		//
		// _TextApplication
		//
		_TextApplication.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextApplication.Location = new Point(130, 99);
		_TextApplication.Size = new Size(580, 23);
		_TextApplication.Name = "_TextApplication";
		_TextApplication.TabIndex = 7;
		//
		// _ButtonBrowseApplication
		//
		_ButtonBrowseApplication.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseApplication.Location = new Point(720, 98);
		_ButtonBrowseApplication.Size = new Size(36, 25);
		_ButtonBrowseApplication.Text = "...";
		_ButtonBrowseApplication.UseVisualStyleBackColor = true;
		_ButtonBrowseApplication.Name = "_ButtonBrowseApplication";
		_ButtonBrowseApplication.TabIndex = 8;
		_ButtonBrowseApplication.Click += _ButtonBrowseApplication_Click;
		//
		// _CheckPrintVerb
		//
		_CheckPrintVerb.AutoSize = true;
		_CheckPrintVerb.Location = new Point(130, 130);
		_CheckPrintVerb.Text = "Add verb \"print\" (/print \"%1\")";
		_CheckPrintVerb.UseVisualStyleBackColor = true;
		_CheckPrintVerb.Name = "_CheckPrintVerb";
		_CheckPrintVerb.TabIndex = 9;
		//
		// _ButtonRegisterType
		//
		_ButtonRegisterType.Location = new Point(130, 160);
		_ButtonRegisterType.Size = new Size(100, 27);
		_ButtonRegisterType.Text = "Register";
		_ButtonRegisterType.UseVisualStyleBackColor = true;
		_ButtonRegisterType.Name = "_ButtonRegisterType";
		_ButtonRegisterType.TabIndex = 10;
		_ButtonRegisterType.Click += _ButtonRegisterType_Click;
		//
		// _ButtonUnregisterType
		//
		_ButtonUnregisterType.Location = new Point(238, 160);
		_ButtonUnregisterType.Size = new Size(100, 27);
		_ButtonUnregisterType.Text = "Unregister";
		_ButtonUnregisterType.UseVisualStyleBackColor = true;
		_ButtonUnregisterType.Name = "_ButtonUnregisterType";
		_ButtonUnregisterType.TabIndex = 11;
		_ButtonUnregisterType.Click += _ButtonUnregisterType_Click;
		//
		// _ButtonCheckType
		//
		_ButtonCheckType.Location = new Point(346, 160);
		_ButtonCheckType.Size = new Size(100, 27);
		_ButtonCheckType.Text = "Check";
		_ButtonCheckType.UseVisualStyleBackColor = true;
		_ButtonCheckType.Name = "_ButtonCheckType";
		_ButtonCheckType.TabIndex = 12;
		_ButtonCheckType.Click += _ButtonCheckType_Click;
		//
		// _ButtonCreateFile
		//
		_ButtonCreateFile.Location = new Point(130, 195);
		_ButtonCreateFile.Size = new Size(208, 27);
		_ButtonCreateFile.Text = "Create test file and show it";
		_ButtonCreateFile.UseVisualStyleBackColor = true;
		_ButtonCreateFile.Name = "_ButtonCreateFile";
		_ButtonCreateFile.TabIndex = 13;
		_ButtonCreateFile.Click += _ButtonCreateFile_Click;
		//
		// _ButtonDefaultApps
		//
		_ButtonDefaultApps.Location = new Point(346, 195);
		_ButtonDefaultApps.Size = new Size(160, 27);
		_ButtonDefaultApps.Text = "Default apps settings";
		_ButtonDefaultApps.UseVisualStyleBackColor = true;
		_ButtonDefaultApps.Name = "_ButtonDefaultApps";
		_ButtonDefaultApps.TabIndex = 14;
		_ButtonDefaultApps.Click += _ButtonDefaultApps_Click;
		//
		// _ButtonRegisterDefaultApp
		//
		_ButtonRegisterDefaultApp.Location = new Point(130, 230);
		_ButtonRegisterDefaultApp.Size = new Size(208, 27);
		_ButtonRegisterDefaultApp.Text = "Register in default apps";
		_ButtonRegisterDefaultApp.UseVisualStyleBackColor = true;
		_ButtonRegisterDefaultApp.Name = "_ButtonRegisterDefaultApp";
		_ButtonRegisterDefaultApp.TabIndex = 15;
		_ButtonRegisterDefaultApp.Click += _ButtonRegisterDefaultApp_Click;
		//
		// _ButtonUnregisterDefaultApp
		//
		_ButtonUnregisterDefaultApp.Location = new Point(346, 230);
		_ButtonUnregisterDefaultApp.Size = new Size(208, 27);
		_ButtonUnregisterDefaultApp.Text = "Remove from default apps";
		_ButtonUnregisterDefaultApp.UseVisualStyleBackColor = true;
		_ButtonUnregisterDefaultApp.Name = "_ButtonUnregisterDefaultApp";
		_ButtonUnregisterDefaultApp.TabIndex = 16;
		_ButtonUnregisterDefaultApp.Click += _ButtonUnregisterDefaultApp_Click;
		//
		// _LabelAppKey
		//
		_LabelAppKey.AutoSize = true;
		_LabelAppKey.Location = new Point(12, 15);
		_LabelAppKey.Text = "Key:";
		_LabelAppKey.Name = "_LabelAppKey";
		_LabelAppKey.TabIndex = 0;
		//
		// _TextAppKey
		//
		_TextAppKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextAppKey.Location = new Point(130, 12);
		_TextAppKey.Size = new Size(616, 23);
		_TextAppKey.Name = "_TextAppKey";
		_TextAppKey.TabIndex = 1;
		//
		// _LabelAppName
		//
		_LabelAppName.AutoSize = true;
		_LabelAppName.Location = new Point(12, 44);
		_LabelAppName.Text = "Display name:";
		_LabelAppName.Name = "_LabelAppName";
		_LabelAppName.TabIndex = 2;
		//
		// _TextAppName
		//
		_TextAppName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextAppName.Location = new Point(130, 41);
		_TextAppName.Size = new Size(616, 23);
		_TextAppName.Name = "_TextAppName";
		_TextAppName.TabIndex = 3;
		//
		// _LabelAppVersion
		//
		_LabelAppVersion.AutoSize = true;
		_LabelAppVersion.Location = new Point(12, 73);
		_LabelAppVersion.Text = "Version:";
		_LabelAppVersion.Name = "_LabelAppVersion";
		_LabelAppVersion.TabIndex = 4;
		//
		// _TextAppVersion
		//
		_TextAppVersion.Location = new Point(130, 70);
		_TextAppVersion.Size = new Size(150, 23);
		_TextAppVersion.Name = "_TextAppVersion";
		_TextAppVersion.TabIndex = 5;
		//
		// _LabelPublisher
		//
		_LabelPublisher.AutoSize = true;
		_LabelPublisher.Location = new Point(12, 102);
		_LabelPublisher.Text = "Publisher:";
		_LabelPublisher.Name = "_LabelPublisher";
		_LabelPublisher.TabIndex = 6;
		//
		// _TextPublisher
		//
		_TextPublisher.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextPublisher.Location = new Point(130, 99);
		_TextPublisher.Size = new Size(616, 23);
		_TextPublisher.Name = "_TextPublisher";
		_TextPublisher.TabIndex = 7;
		//
		// _LabelUninstall
		//
		_LabelUninstall.AutoSize = true;
		_LabelUninstall.Location = new Point(12, 131);
		_LabelUninstall.Text = "Uninstall command:";
		_LabelUninstall.Name = "_LabelUninstall";
		_LabelUninstall.TabIndex = 8;
		//
		// _TextUninstall
		//
		_TextUninstall.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextUninstall.Location = new Point(130, 128);
		_TextUninstall.Size = new Size(616, 23);
		_TextUninstall.Name = "_TextUninstall";
		_TextUninstall.TabIndex = 9;
		//
		// _CheckModify
		//
		_CheckModify.AutoSize = true;
		_CheckModify.Location = new Point(130, 159);
		_CheckModify.Text = "Modify command (same with --modify)";
		_CheckModify.UseVisualStyleBackColor = true;
		_CheckModify.Name = "_CheckModify";
		_CheckModify.TabIndex = 10;
		//
		// _ButtonRegisterApp
		//
		_ButtonRegisterApp.Location = new Point(130, 189);
		_ButtonRegisterApp.Size = new Size(100, 27);
		_ButtonRegisterApp.Text = "Register";
		_ButtonRegisterApp.UseVisualStyleBackColor = true;
		_ButtonRegisterApp.Name = "_ButtonRegisterApp";
		_ButtonRegisterApp.TabIndex = 11;
		_ButtonRegisterApp.Click += _ButtonRegisterApp_Click;
		//
		// _ButtonGetApp
		//
		_ButtonGetApp.Location = new Point(238, 189);
		_ButtonGetApp.Size = new Size(100, 27);
		_ButtonGetApp.Text = "Get";
		_ButtonGetApp.UseVisualStyleBackColor = true;
		_ButtonGetApp.Name = "_ButtonGetApp";
		_ButtonGetApp.TabIndex = 12;
		_ButtonGetApp.Click += _ButtonGetApp_Click;
		//
		// _ButtonUnregisterApp
		//
		_ButtonUnregisterApp.Location = new Point(346, 189);
		_ButtonUnregisterApp.Size = new Size(100, 27);
		_ButtonUnregisterApp.Text = "Unregister";
		_ButtonUnregisterApp.UseVisualStyleBackColor = true;
		_ButtonUnregisterApp.Name = "_ButtonUnregisterApp";
		_ButtonUnregisterApp.TabIndex = 13;
		_ButtonUnregisterApp.Click += _ButtonUnregisterApp_Click;
		//
		// _ButtonAppsSettings
		//
		_ButtonAppsSettings.Location = new Point(454, 189);
		_ButtonAppsSettings.Size = new Size(120, 27);
		_ButtonAppsSettings.Text = "Installed apps";
		_ButtonAppsSettings.UseVisualStyleBackColor = true;
		_ButtonAppsSettings.Name = "_ButtonAppsSettings";
		_ButtonAppsSettings.TabIndex = 14;
		_ButtonAppsSettings.Click += _ButtonAppsSettings_Click;
		//
		// _LabelIniFile
		//
		_LabelIniFile.AutoSize = true;
		_LabelIniFile.Location = new Point(12, 15);
		_LabelIniFile.Text = "Ini file:";
		_LabelIniFile.Name = "_LabelIniFile";
		_LabelIniFile.TabIndex = 0;
		//
		// _TextIniFile
		//
		_TextIniFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextIniFile.Location = new Point(130, 12);
		_TextIniFile.Size = new Size(580, 23);
		_TextIniFile.Name = "_TextIniFile";
		_TextIniFile.TabIndex = 1;
		//
		// _ButtonBrowseIniFile
		//
		_ButtonBrowseIniFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		_ButtonBrowseIniFile.Location = new Point(720, 11);
		_ButtonBrowseIniFile.Size = new Size(36, 25);
		_ButtonBrowseIniFile.Text = "...";
		_ButtonBrowseIniFile.UseVisualStyleBackColor = true;
		_ButtonBrowseIniFile.Name = "_ButtonBrowseIniFile";
		_ButtonBrowseIniFile.TabIndex = 2;
		_ButtonBrowseIniFile.Click += _ButtonBrowseIniFile_Click;
		//
		// _LabelSection
		//
		_LabelSection.AutoSize = true;
		_LabelSection.Location = new Point(12, 44);
		_LabelSection.Text = "Section:";
		_LabelSection.Name = "_LabelSection";
		_LabelSection.TabIndex = 3;
		//
		// _TextSection
		//
		_TextSection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextSection.Location = new Point(130, 41);
		_TextSection.Size = new Size(616, 23);
		_TextSection.Name = "_TextSection";
		_TextSection.TabIndex = 4;
		//
		// _LabelKey
		//
		_LabelKey.AutoSize = true;
		_LabelKey.Location = new Point(12, 73);
		_LabelKey.Text = "Key:";
		_LabelKey.Name = "_LabelKey";
		_LabelKey.TabIndex = 5;
		//
		// _TextKey
		//
		_TextKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextKey.Location = new Point(130, 70);
		_TextKey.Size = new Size(616, 23);
		_TextKey.Name = "_TextKey";
		_TextKey.TabIndex = 6;
		//
		// _LabelValue
		//
		_LabelValue.AutoSize = true;
		_LabelValue.Location = new Point(12, 102);
		_LabelValue.Text = "Value:";
		_LabelValue.Name = "_LabelValue";
		_LabelValue.TabIndex = 7;
		//
		// _TextValue
		//
		_TextValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_TextValue.Location = new Point(130, 99);
		_TextValue.Size = new Size(616, 23);
		_TextValue.Name = "_TextValue";
		_TextValue.TabIndex = 8;
		//
		// _CheckAutoSave
		//
		_CheckAutoSave.AutoSize = true;
		_CheckAutoSave.Location = new Point(130, 130);
		_CheckAutoSave.Text = "Save automatically";
		_CheckAutoSave.UseVisualStyleBackColor = true;
		_CheckAutoSave.Name = "_CheckAutoSave";
		_CheckAutoSave.TabIndex = 9;
		//
		// _ButtonOpenIni
		//
		_ButtonOpenIni.Location = new Point(130, 160);
		_ButtonOpenIni.Size = new Size(100, 27);
		_ButtonOpenIni.Text = "Open";
		_ButtonOpenIni.UseVisualStyleBackColor = true;
		_ButtonOpenIni.Name = "_ButtonOpenIni";
		_ButtonOpenIni.TabIndex = 10;
		_ButtonOpenIni.Click += _ButtonOpenIni_Click;
		//
		// _ButtonListIni
		//
		_ButtonListIni.Location = new Point(238, 160);
		_ButtonListIni.Size = new Size(100, 27);
		_ButtonListIni.Text = "List";
		_ButtonListIni.UseVisualStyleBackColor = true;
		_ButtonListIni.Name = "_ButtonListIni";
		_ButtonListIni.TabIndex = 11;
		_ButtonListIni.Click += _ButtonListIni_Click;
		//
		// _ButtonSaveIni
		//
		_ButtonSaveIni.Location = new Point(346, 160);
		_ButtonSaveIni.Size = new Size(100, 27);
		_ButtonSaveIni.Text = "Save";
		_ButtonSaveIni.UseVisualStyleBackColor = true;
		_ButtonSaveIni.Name = "_ButtonSaveIni";
		_ButtonSaveIni.TabIndex = 12;
		_ButtonSaveIni.Click += _ButtonSaveIni_Click;
		//
		// _ButtonReloadIni
		//
		_ButtonReloadIni.Location = new Point(454, 160);
		_ButtonReloadIni.Size = new Size(100, 27);
		_ButtonReloadIni.Text = "Reload";
		_ButtonReloadIni.UseVisualStyleBackColor = true;
		_ButtonReloadIni.Name = "_ButtonReloadIni";
		_ButtonReloadIni.TabIndex = 13;
		_ButtonReloadIni.Click += _ButtonReloadIni_Click;
		//
		// _ButtonEditIni
		//
		_ButtonEditIni.Location = new Point(562, 160);
		_ButtonEditIni.Size = new Size(130, 27);
		_ButtonEditIni.Text = "Edit in Notepad";
		_ButtonEditIni.UseVisualStyleBackColor = true;
		_ButtonEditIni.Name = "_ButtonEditIni";
		_ButtonEditIni.TabIndex = 14;
		_ButtonEditIni.Click += _ButtonEditIni_Click;
		//
		// _ButtonGetValue
		//
		_ButtonGetValue.Location = new Point(130, 195);
		_ButtonGetValue.Size = new Size(100, 27);
		_ButtonGetValue.Text = "Get";
		_ButtonGetValue.UseVisualStyleBackColor = true;
		_ButtonGetValue.Name = "_ButtonGetValue";
		_ButtonGetValue.TabIndex = 15;
		_ButtonGetValue.Click += _ButtonGetValue_Click;
		//
		// _ButtonSetValue
		//
		_ButtonSetValue.Location = new Point(238, 195);
		_ButtonSetValue.Size = new Size(100, 27);
		_ButtonSetValue.Text = "Set";
		_ButtonSetValue.UseVisualStyleBackColor = true;
		_ButtonSetValue.Name = "_ButtonSetValue";
		_ButtonSetValue.TabIndex = 16;
		_ButtonSetValue.Click += _ButtonSetValue_Click;
		//
		// _ButtonDeleteKey
		//
		_ButtonDeleteKey.Location = new Point(346, 195);
		_ButtonDeleteKey.Size = new Size(100, 27);
		_ButtonDeleteKey.Text = "Delete key";
		_ButtonDeleteKey.UseVisualStyleBackColor = true;
		_ButtonDeleteKey.Name = "_ButtonDeleteKey";
		_ButtonDeleteKey.TabIndex = 17;
		_ButtonDeleteKey.Click += _ButtonDeleteKey_Click;
		//
		// _ButtonDeleteSection
		//
		_ButtonDeleteSection.Location = new Point(454, 195);
		_ButtonDeleteSection.Size = new Size(130, 27);
		_ButtonDeleteSection.Text = "Delete section";
		_ButtonDeleteSection.UseVisualStyleBackColor = true;
		_ButtonDeleteSection.Name = "_ButtonDeleteSection";
		_ButtonDeleteSection.TabIndex = 18;
		_ButtonDeleteSection.Click += _ButtonDeleteSection_Click;
		//
		// _ListLog
		//
		_ListLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		_ListLog.HorizontalScrollbar = true;
		_ListLog.IntegralHeight = false;
		_ListLog.Location = new Point(12, 352);
		_ListLog.Size = new Size(776, 216);
		_ListLog.Name = "_ListLog";
		_ListLog.TabIndex = 4;
		//
		// _ButtonClear
		//
		_ButtonClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		_ButtonClear.Location = new Point(688, 576);
		_ButtonClear.Size = new Size(100, 27);
		_ButtonClear.Text = "Clear log";
		_ButtonClear.UseVisualStyleBackColor = true;
		_ButtonClear.Name = "_ButtonClear";
		_ButtonClear.TabIndex = 5;
		_ButtonClear.Click += _ButtonClear_Click;
		//
		// MainWindow
		//
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 615);
		Controls.Add(_LabelScope);
		Controls.Add(_ComboScope);
		Controls.Add(_LabelArgs);
		Controls.Add(_Tabs);
		Controls.Add(_ListLog);
		Controls.Add(_ButtonClear);
		MinimumSize = new Size(700, 560);
		Name = "MainWindow";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "Spellbook Windows Sample";
		((System.ComponentModel.ISupportInitialize) _NumLinkIconIndex).EndInit();
		_TabIni.ResumeLayout(false);
		_TabIni.PerformLayout();
		_TabApplication.ResumeLayout(false);
		_TabApplication.PerformLayout();
		_TabFileType.ResumeLayout(false);
		_TabFileType.PerformLayout();
		_TabShortcut.ResumeLayout(false);
		_TabShortcut.PerformLayout();
		_Tabs.ResumeLayout(false);
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion


	private Label _LabelScope;

	private ComboBox _ComboScope;

	private Label _LabelArgs;

	private TabControl _Tabs;

	private TabPage _TabShortcut;

	private TabPage _TabFileType;

	private TabPage _TabApplication;

	private TabPage _TabIni;

	private Label _LabelLinkFile;

	private TextBox _TextLinkFile;

	private Button _ButtonBrowseLinkFile;

	private Label _LabelTarget;

	private TextBox _TextTarget;

	private Button _ButtonBrowseTarget;

	private Label _LabelLinkArguments;

	private TextBox _TextLinkArguments;

	private Label _LabelLinkDescription;

	private TextBox _TextLinkDescription;

	private Label _LabelLinkIcon;

	private TextBox _TextLinkIcon;

	private Button _ButtonBrowseLinkIcon;

	private Label _LabelLinkIconIndex;

	private NumericUpDown _NumLinkIconIndex;

	private Label _LabelWindowStyle;

	private ComboBox _ComboWindowStyle;

	private CheckBox _CheckRunAsAdmin;

	private Button _ButtonCreateLink;

	private Button _ButtonReadLink;

	private Button _ButtonDeleteLink;

	private Button _ButtonLinkDesktop;

	private Button _ButtonLinkStartMenu;

	private Label _LabelExtension;

	private TextBox _TextExtension;

	private Label _LabelProgId;

	private TextBox _TextProgId;

	private Label _LabelTypeDescription;

	private TextBox _TextTypeDescription;

	private Label _LabelApplication;

	private TextBox _TextApplication;

	private Button _ButtonBrowseApplication;

	private CheckBox _CheckPrintVerb;

	private Button _ButtonRegisterType;

	private Button _ButtonUnregisterType;

	private Button _ButtonCheckType;

	private Button _ButtonCreateFile;

	private Button _ButtonDefaultApps;

	private Button _ButtonRegisterDefaultApp;

	private Button _ButtonUnregisterDefaultApp;

	private Label _LabelAppKey;

	private TextBox _TextAppKey;

	private Label _LabelAppName;

	private TextBox _TextAppName;

	private Label _LabelAppVersion;

	private TextBox _TextAppVersion;

	private Label _LabelPublisher;

	private TextBox _TextPublisher;

	private Label _LabelUninstall;

	private TextBox _TextUninstall;

	private CheckBox _CheckModify;

	private Button _ButtonRegisterApp;

	private Button _ButtonGetApp;

	private Button _ButtonUnregisterApp;

	private Button _ButtonAppsSettings;

	private Label _LabelIniFile;

	private TextBox _TextIniFile;

	private Button _ButtonBrowseIniFile;

	private Label _LabelSection;

	private TextBox _TextSection;

	private Label _LabelKey;

	private TextBox _TextKey;

	private Label _LabelValue;

	private TextBox _TextValue;

	private CheckBox _CheckAutoSave;

	private Button _ButtonOpenIni;

	private Button _ButtonListIni;

	private Button _ButtonSaveIni;

	private Button _ButtonReloadIni;

	private Button _ButtonEditIni;

	private Button _ButtonGetValue;

	private Button _ButtonSetValue;

	private Button _ButtonDeleteKey;

	private Button _ButtonDeleteSection;

	private ListBox _ListLog;

	private Button _ButtonClear;
}
