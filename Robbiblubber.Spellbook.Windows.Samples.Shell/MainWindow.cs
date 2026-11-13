namespace Robbiblubber.Spellbook.Windows.Samples.Shell;

using System.Diagnostics;
using System.Runtime.Versioning;
using Robbiblubber.Spellbook.Windows;



/// <summary>This class implements the main window of the Windows sample.</summary>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registration name of the sample in the default apps settings.</summary>
	private const string _DEFAULT_APP_NAME = "Robbiblubber.Spellbook.Sample";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Open ini file.</summary>
	private IniFile? _Ini = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="args">Command line arguments.</param>
	public MainWindow(string[] args)
	{
		InitializeComponent();

		_ComboScope.Items.AddRange(Enum.GetValues<RegistrationScope>().Cast<object>().ToArray());
		_ComboScope.SelectedItem = RegistrationScope.CURRENT_USER;
		_ComboWindowStyle.Items.AddRange(Enum.GetValues<ShortcutWindowStyle>().Cast<object>().ToArray());
		_ComboWindowStyle.SelectedItem = ShortcutWindowStyle.NORMAL;

		_TextLinkFile.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Spellbook Sample.lnk");
		_TextTarget.Text = Environment.ProcessPath;
		_TextLinkArguments.Text = "--from-shortcut";
		_TextLinkDescription.Text = "Spellbook Windows sample";

		_TextExtension.Text = ".sbsample";
		_TextProgId.Text = "Robbiblubber.Spellbook.Sample.Document";
		_TextTypeDescription.Text = "Spellbook Sample Document";
		_TextApplication.Text = Environment.ProcessPath;

		_TextAppKey.Text = "Robbiblubber.Spellbook.Sample";
		_TextAppName.Text = "Spellbook Windows Sample";
		_TextAppVersion.Text = "4.0.1";
		_TextPublisher.Text = "robbiblubber.org";
		_TextUninstall.Text = $"\"{Environment.ProcessPath}\" --uninstall";

		_TextIniFile.Text = Path.Combine(Path.GetTempPath(), "Spellbook Sample", "sample.ini");
		_TextSection.Text = "General";
		_TextKey.Text = "Name";
		_TextValue.Text = "Spellbook";
		_CheckAutoSave.Checked = true;

		_LabelArgs.Text = "Arguments: " + ((args.Length == 0) ? "(none)" : string.Join(" ", args.Select(m => m.Contains(' ') ? $"\"{m}\"" : m)));
		_Log("Windows sample started. Executable: " + Environment.ProcessPath);
		_HandleArguments(args);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the selected scope.</summary>
	private RegistrationScope _Scope
	{
		get { return (RegistrationScope) _ComboScope.SelectedItem!; }
	}


	/// <summary>Gets the file type from the input fields.</summary>
	private FileAssociation.FileType _FileType
	{
		get
		{
			return new(_TextExtension.Text, _TextProgId.Text, _TextApplication.Text)
			{
				Description = (string.IsNullOrWhiteSpace(_TextTypeDescription.Text) ? null : _TextTypeDescription.Text),
				Verbs = (_CheckPrintVerb.Checked ? [new FileAssociation.Verb("print") { Arguments = "/print \"%1\"" }] : [])
			};
		}
	}



	/// <summary>Gets the open ini file; the file is opened if it has not been opened yet or the file name has changed.</summary>
	private IniFile _IniFile
	{
		get
		{
			if((_Ini is null) || (!string.Equals(_Ini.FileName, Path.GetFullPath(_TextIniFile.Text), StringComparison.OrdinalIgnoreCase)))
			{
				_Ini = new(_TextIniFile.Text);
				_Log($"Ini file opened: '{_Ini.FileName}' ({_Ini.Encoding.WebName}{((_Ini.Encoding.GetPreamble().Length > 0) ? " with BOM" : "")}).");
			}

			_Ini.AutoSave = _CheckAutoSave.Checked;
			return _Ini;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log line.</summary>
	/// <param name="text">Text.</param>
	private void _Log(string text)
	{
		_ListLog.Items.Add(text);
		_ListLog.TopIndex = Math.Max(0, (_ListLog.Items.Count - 1));
	}


	/// <summary>Runs an operation with error handling.</summary>
	/// <param name="action">Operation.</param>
	private void _Run(Action action)
	{
		try
		{
			action();
		}
		catch(Exception ex) { _Log($"Error: {ex.GetType().Name}: {ex.Message}"); }
	}


	/// <summary>Logs what the command line arguments request.</summary>
	/// <param name="args">Command line arguments.</param>
	private void _HandleArguments(string[] args)
	{
		if(args.Length == 0) { return; }

		switch(args[0])
		{
			case "--uninstall": _Log("Uninstall requested by Windows (--uninstall)."); return;
			case "--modify": _Log("Modify requested by Windows (--modify)."); return;
			case "--from-shortcut": _Log("Started from shortcut (--from-shortcut)."); return;
		}

		bool print = (args[0] == "/print");
		string file = (print ? (args.ElementAtOrDefault(1) ?? "") : args[0]);
		if(!File.Exists(file)) { _Log($"Unknown arguments: {string.Join(" ", args)}"); return; }

		_Log($"{(print ? "Print" : "Open")} requested for '{file}'.");
		_Log("Content: " + File.ReadAllText(file).ReplaceLineEndings(" "));
	}


	/// <summary>Shows a file dialog and puts the file name into a text box.</summary>
	/// <param name="box">Text box.</param>
	/// <param name="save">Determines if a save dialog is shown.</param>
	/// <param name="filter">File filter.</param>
	private static void _Browse(TextBox box, bool save, string filter = "All files (*.*)|*.*")
	{
		using FileDialog dlg = (save ? new SaveFileDialog() : new OpenFileDialog());
		dlg.Filter = filter;
		dlg.FileName = box.Text;
		if(dlg.ShowDialog() == DialogResult.OK) { box.Text = dlg.FileName; }
	}


	/// <summary>Opens a URI with the shell.</summary>
	/// <param name="uri">URI.</param>
	private static void _Open(string uri)
	{
		Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true })?.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Click event of the shortcut file browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseLinkFile_Click(object sender, EventArgs e)
	{
		_Browse(_TextLinkFile, true, "Shortcuts (*.lnk)|*.lnk");
	}


	/// <summary>Handles the Click event of the target browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseTarget_Click(object sender, EventArgs e)
	{
		_Browse(_TextTarget, false);
	}


	/// <summary>Handles the Click event of the icon browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseLinkIcon_Click(object sender, EventArgs e)
	{
		_Browse(_TextLinkIcon, false, "Icon files (*.ico;*.exe;*.dll)|*.ico;*.exe;*.dll|All files (*.*)|*.*");
	}


	/// <summary>Handles the Click event of the create shortcut button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCreateLink_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			Shortcut.Create(_TextLinkFile.Text, new ShortcutInfo(_TextTarget.Text)
			{
				Arguments = _TextLinkArguments.Text,
				Description = _TextLinkDescription.Text,
				IconPath = (string.IsNullOrWhiteSpace(_TextLinkIcon.Text) ? null : _TextLinkIcon.Text),
				IconIndex = (int) _NumLinkIconIndex.Value,
				WindowStyle = (ShortcutWindowStyle) _ComboWindowStyle.SelectedItem!,
				RunAsAdministrator = _CheckRunAsAdmin.Checked
			});
			_Log($"Shortcut created: '{_TextLinkFile.Text}'.");
		});
	}


	/// <summary>Handles the Click event of the read shortcut button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonReadLink_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			ShortcutInfo info = Shortcut.Read(_TextLinkFile.Text);

			_TextTarget.Text = info.TargetPath;
			_TextLinkArguments.Text = info.Arguments;
			_TextLinkDescription.Text = info.Description;
			_TextLinkIcon.Text = info.IconPath;
			_NumLinkIconIndex.Value = Math.Clamp(info.IconIndex, (int) _NumLinkIconIndex.Minimum, (int) _NumLinkIconIndex.Maximum);
			_ComboWindowStyle.SelectedItem = info.WindowStyle;
			_CheckRunAsAdmin.Checked = info.RunAsAdministrator;

			_Log($"Shortcut read: {info}");
			_Log($"  working directory: {info.WorkingDirectory ?? "(none)"}, description: {info.Description ?? "(none)"}");
			_Log($"  icon: {info.IconPath ?? "(target)"},{info.IconIndex}, window: {info.WindowStyle}, admin: {info.RunAsAdministrator}");
		});
	}


	/// <summary>Handles the Click event of the delete shortcut button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDeleteLink_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool exists = File.Exists(_TextLinkFile.Text);
			File.Delete(_TextLinkFile.Text);
			_Log(exists ? $"Shortcut deleted: '{_TextLinkFile.Text}'." : "Delete: the shortcut does not exist.");
		});
	}


	/// <summary>Handles the Click event of the desktop path button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonLinkDesktop_Click(object sender, EventArgs e)
	{
		_TextLinkFile.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Spellbook Sample.lnk");
	}


	/// <summary>Handles the Click event of the start menu path button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonLinkStartMenu_Click(object sender, EventArgs e)
	{
		_TextLinkFile.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Spellbook Sample", "Spellbook Sample.lnk");
	}


	/// <summary>Handles the Click event of the application browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseApplication_Click(object sender, EventArgs e)
	{
		_Browse(_TextApplication, false, "Programs (*.exe)|*.exe");
	}


	/// <summary>Handles the Click event of the register file type button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRegisterType_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			FileAssociation.FileType type = _FileType;
			FileAssociation.Register(type, _Scope);
			_Log($"File type registered: {type.Extension} -> {type.ProgId} ({_Scope}), verbs: open{(_CheckPrintVerb.Checked ? ", print" : "")}.");
			_Log($"  default program ID now: {FileAssociation.GetProgId(type.Extension) ?? "(none)"}");
		});
	}


	/// <summary>Handles the Click event of the unregister file type button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonUnregisterType_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool removed = FileAssociation.Unregister(_TextExtension.Text, _TextProgId.Text, _Scope);
			_Log(removed ? $"File type unregistered ({_Scope})." : $"Unregister: the file type was not registered ({_Scope}).");
		});
	}


	/// <summary>Handles the Click event of the check file type button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCheckType_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			_Log($"IsRegistered({_Scope}): {FileAssociation.IsRegistered(_TextExtension.Text, _TextProgId.Text, _Scope)}, " +
				 $"default ({_Scope}): {FileAssociation.GetProgId(_TextExtension.Text, _Scope) ?? "(none)"}, " +
				 $"default (merged): {FileAssociation.GetProgId(_TextExtension.Text) ?? "(none)"}");
		});
	}


	/// <summary>Handles the Click event of the create test file button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCreateFile_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			string dir = Path.Combine(Path.GetTempPath(), "Spellbook Sample");
			Directory.CreateDirectory(dir);

			string file = Path.Combine(dir, "Test File" + _FileType.Extension);
			File.WriteAllText(file, "Hello from the Spellbook sample.");
			Process.Start("explorer.exe", $"/select,\"{file}\"")?.Dispose();

			_Log($"Test file created: '{file}'.");
		});
	}


	/// <summary>Handles the Click event of the default apps button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDefaultApps_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			if(FileAssociation.IsApplicationRegistered(_DEFAULT_APP_NAME, _Scope))
			{
				FileAssociation.OpenDefaultAppsSettings(_DEFAULT_APP_NAME, _Scope);
				_Log($"Default apps settings opened for '{_DEFAULT_APP_NAME}'.");
			}
			else
			{
				FileAssociation.OpenDefaultAppsSettings();
				_Log("Default apps settings opened.");
			}
		});
	}


	/// <summary>Handles the Click event of the register in default apps button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRegisterDefaultApp_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			FileAssociation.FileType type = _FileType;
			FileAssociation.RegisterApplication(new FileAssociation.ApplicationCapabilities(_DEFAULT_APP_NAME, "Spellbook Windows Sample")
			{
				Description = "Sample application of the Spellbook Windows Support Library.",
				FileTypes = [type]
			}, _Scope);

			_Log($"Registered in default apps: '{_DEFAULT_APP_NAME}' with {type.Extension} -> {type.ProgId} ({_Scope}).");
		});
	}


	/// <summary>Handles the Click event of the remove from default apps button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonUnregisterDefaultApp_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool removed = FileAssociation.UnregisterApplication(_DEFAULT_APP_NAME, _Scope);
			_Log(removed ? $"Removed from default apps, file types unregistered ({_Scope})." : $"Remove: not registered in default apps ({_Scope}).");
		});
	}


	/// <summary>Handles the Click event of the register application button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRegisterApp_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			string exe = Environment.ProcessPath!;
			InstalledApplication app = new(_TextAppKey.Text, _TextAppName.Text, Version.Parse(_TextAppVersion.Text))
			{
				Publisher = _TextPublisher.Text,
				InstallLocation = Path.GetDirectoryName(exe),
				DisplayIcon = exe,
				UninstallCommand = _TextUninstall.Text,
				ModifyCommand = (_CheckModify.Checked ? $"\"{exe}\" --modify" : null),
				EstimatedSize = new FileInfo(exe).Length,
				AboutUrl = "https://robbiblubber.org/"
			};

			ApplicationRegistration.Register(app, _Scope);
			_Log($"Application registered: {app} ({_Scope}).");
		});
	}


	/// <summary>Handles the Click event of the get application button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonGetApp_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			InstalledApplication? app = ApplicationRegistration.Get(_TextAppKey.Text, _Scope);
			if(app is null) { _Log($"Get: not registered ({_Scope})."); return; }

			_Log($"Get: {app}, installed {app.InstallDate:yyyy-MM-dd}, size {app.EstimatedSize} bytes.");
			_Log($"  uninstall: {app.UninstallCommand ?? "(none)"}, modify: {app.ModifyCommand ?? "(none)"}");
		});
	}


	/// <summary>Handles the Click event of the unregister application button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonUnregisterApp_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool removed = ApplicationRegistration.Unregister(_TextAppKey.Text, _Scope);
			_Log(removed ? $"Application unregistered ({_Scope})." : $"Unregister: the application was not registered ({_Scope}).");
		});
	}


	/// <summary>Handles the Click event of the installed apps button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAppsSettings_Click(object sender, EventArgs e)
	{
		_Run(() => _Open("ms-settings:appsfeatures"));
	}


	/// <summary>Handles the Click event of the ini file browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseIniFile_Click(object sender, EventArgs e)
	{
		_Browse(_TextIniFile, false, "Ini files (*.ini)|*.ini|All files (*.*)|*.*");
	}


	/// <summary>Handles the Click event of the open ini file button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonOpenIni_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			_Ini = null;
			_ = _IniFile;
		});
	}


	/// <summary>Handles the Click event of the list button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonListIni_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			IniFile ini = _IniFile;
			string[] sections = ini.GetSections();
			_Log($"Sections: {sections.Length}");

			foreach(string i in sections)
			{
				_Log($"  [{i}] " + string.Join(", ", ini.GetKeys(i).Select(m => $"{m}={ini.GetString(i, m)}")));
			}
		});
	}


	/// <summary>Handles the Click event of the save ini file button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonSaveIni_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			_IniFile.Save();
			_Log("Ini file saved.");
		});
	}


	/// <summary>Handles the Click event of the reload ini file button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonReloadIni_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			_IniFile.Reload();
			_Log("Ini file reloaded.");
		});
	}


	/// <summary>Handles the Click event of the edit ini file button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonEditIni_Click(object sender, EventArgs e)
	{
		_Run(() => Process.Start("notepad.exe", $"\"{_IniFile.FileName}\"")?.Dispose());
	}


	/// <summary>Handles the Click event of the get value button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonGetValue_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			IniFile ini = _IniFile;
			if(ini.TryGetString(_TextSection.Text, _TextKey.Text, out string? value))
			{
				_TextValue.Text = value;
				_Log($"Get [{_TextSection.Text}] {_TextKey.Text} = '{value}' (Int32: {ini.GetInt32(_TextSection.Text, _TextKey.Text, -1)}, " +
					 $"Boolean: {ini.GetBoolean(_TextSection.Text, _TextKey.Text)}).");
			}
			else { _Log($"Get [{_TextSection.Text}] {_TextKey.Text}: not found."); }
		});
	}


	/// <summary>Handles the Click event of the set value button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonSetValue_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			_IniFile.SetValue(_TextSection.Text, _TextKey.Text, _TextValue.Text);
			_Log($"Set [{_TextSection.Text}] {_TextKey.Text} = '{_TextValue.Text}'{(_CheckAutoSave.Checked ? " (saved)" : " (not saved)")}.");
		});
	}


	/// <summary>Handles the Click event of the delete key button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDeleteKey_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool deleted = _IniFile.DeleteKey(_TextSection.Text, _TextKey.Text);
			_Log(deleted ? $"Key deleted: [{_TextSection.Text}] {_TextKey.Text}." : "Delete key: not found.");
		});
	}


	/// <summary>Handles the Click event of the delete section button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDeleteSection_Click(object sender, EventArgs e)
	{
		_Run(() =>
		{
			bool deleted = _IniFile.DeleteSection(_TextSection.Text);
			_Log(deleted ? $"Section deleted: [{_TextSection.Text}]." : "Delete section: not found.");
		});
	}


	/// <summary>Handles the Click event of the clear log button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonClear_Click(object sender, EventArgs e)
	{
		_ListLog.Items.Clear();
	}
}
