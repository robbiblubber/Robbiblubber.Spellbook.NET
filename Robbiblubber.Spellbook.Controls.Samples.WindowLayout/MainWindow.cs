namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

using System.Diagnostics;
using System.Runtime.Versioning;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>The window bounds, both splitters, the selected tab and the columns of the list view and the grid are saved by the
///          <see cref="WindowLayoutProvider"/> on the form. The sample options are stored in the same layout store through the
///          <see cref="WindowLayoutProvider.Saving"/> event.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;

	/// <summary>Layout name for the sample options.</summary>
	private const string _OPTIONS_LAYOUT = "SampleOptions";

	/// <summary>Key of the sample options.</summary>
	private const string _OPTIONS_KEY = "flags";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Open tool window, NULL if it is not open.</summary>
	private ToolWindow? _ToolWindow = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();
		SampleLog.Written += SampleLog_Written;

		_InitializeData();
		_LoadOptions();
		_UpdateStatus();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Fills the lists with sample data.</summary>
	private void _InitializeData()
	{
		string[][] files = new string[][]
		{
			new string[] { "Report.docx", "Document", "48 KB", "2026-09-14 10:12" },
			new string[] { "Budget.xlsx", "Spreadsheet", "112 KB", "2026-09-21 16:40" },
			new string[] { "Logo.png", "Image", "18 KB", "2026-08-02 09:05" },
			new string[] { "Notes.txt", "Text", "2 KB", "2026-10-01 18:23" },
			new string[] { "Archive.zip", "Archive", "3.4 MB", "2026-07-30 12:00" }
		};

		_ListFolders.Items.AddRange(new object[] { "Desktop", "Documents", "Downloads", "Pictures", "Projects" });
		foreach(string[] i in files) { _ListFiles.Items.Add(new ListViewItem(i)); }

		_Grid.Columns.Add("Name", "Name");
		_Grid.Columns.Add("Type", "Type");
		_Grid.Columns.Add("Size", "Size");
		_Grid.Columns.Add("Modified", "Modified");
		foreach(DataGridViewColumn i in _Grid.Columns) { i.Width = 140; }
		foreach(string[] i in files) { _Grid.Rows.Add(i.Cast<object>().ToArray()); }
	}


	/// <summary>Loads the sample options from the layout store and applies them to the provider.</summary>
	/// <remarks>This runs before the form is loaded, so the options apply to the restoring of this window.</remarks>
	private void _LoadOptions()
	{
		int[]? flags = _Layout.Store.Read(_OPTIONS_LAYOUT, _OPTIONS_KEY);

		if((flags is not null) && (flags.Length == 3))
		{
			_ChkPerScreen.Checked = (flags[0] != 0);
			_ChkMaximized.Checked = (flags[1] != 0);
			_ChkMinimized.Checked = (flags[2] != 0);
		}

		_ApplyOptions();
	}


	/// <summary>Applies the options to the provider.</summary>
	private void _ApplyOptions()
	{
		_Layout.PerScreenConfiguration = _ChkPerScreen.Checked;
		_Layout.RestoreMaximized = _ChkMaximized.Checked;
		_Layout.RestoreMinimized = _ChkMinimized.Checked;
	}


	/// <summary>Shows the layout file and the screen configuration in the status bar.</summary>
	private void _UpdateStatus()
	{
		string file = ((_Layout.Store is DdnWindowLayoutStore store) ? store.ConfigFile : "(custom store)");
		_LabelFile.Text = $"Layout file: {file}     Screen configuration: {WindowLayoutProvider.GetScreenConfigurationKey()}";
	}


	/// <summary>Opens the tool window or activates it, if it is already open.</summary>
	private void _OpenToolWindow()
	{
		if(_ToolWindow is not null)
		{
			_ToolWindow.Activate();
			return;
		}

		_ToolWindow = new ToolWindow();
		_ToolWindow.FormClosed += (sender, e) => { _ToolWindow = null; };
		_ToolWindow.Show(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Restored event of the layout provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Layout_Restored(object? sender, EventArgs e)
	{
		_ChkModified.Checked = _Grid.Columns["Modified"]!.Visible;
		SampleLog.Write($"Restored    MainWindow: {SampleLog.Format(Bounds)}, {WindowState}, left panel {_SplitMain.SplitterDistance}, tab {_Tabs.SelectedIndex}");
	}


	/// <summary>Handles the Saving event of the layout provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Stores the sample options together with the layout.</remarks>
	private void _Layout_Saving(object? sender, EventArgs e)
	{
		_Layout.Store.Write(_OPTIONS_LAYOUT, _OPTIONS_KEY, new int[] { (_ChkPerScreen.Checked ? 1 : 0), (_ChkMaximized.Checked ? 1 : 0), (_ChkMinimized.Checked ? 1 : 0) });
	}


	/// <summary>Handles the CheckedChanged event of the option check boxes.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkOptions_CheckedChanged(object? sender, EventArgs e)
	{
		_ApplyOptions();
	}


	/// <summary>Handles the CheckedChanged event of the Modified column check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkModified_CheckedChanged(object? sender, EventArgs e)
	{
		if(_Grid.Columns["Modified"] is DataGridViewColumn column) { column.Visible = _ChkModified.Checked; }
	}


	/// <summary>Handles the Click event of the tool window button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonTool_Click(object? sender, EventArgs e)
	{
		_OpenToolWindow();
	}


	/// <summary>Handles the Click event of the dialog button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDialog_Click(object? sender, EventArgs e)
	{
		using DialogWindow dialog = new();
		dialog.ShowDialog(this);
	}


	/// <summary>Handles the Click event of the button that simulates a missing monitor.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Stores bounds for the tool window far right of all screens, as if it had been closed on a monitor that has been
	///          disconnected since, and opens it.</remarks>
	private void _ButtonOffscreen_Click(object? sender, EventArgs e)
	{
		Rectangle all = SystemInformation.VirtualScreen;
		int x = all.Right + 1500;

		_ToolWindow?.Close();

		_Layout.Store.Write("ToolWindow", WindowLayoutProvider.WINDOW_KEY, new int[] { x, all.Top + 200, 420, 320, DeviceDpi, 0 });
		_Layout.Store.Commit();
		SampleLog.Write($"Simulated   ToolWindow saved at x = {x} (screens end at {all.Right})");

		_OpenToolWindow();
	}


	/// <summary>Handles the Click event of the button that shows the layout file.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonShowFile_Click(object? sender, EventArgs e)
	{
		if(_Layout.Store is not DdnWindowLayoutStore store) { return; }

		if(!File.Exists(store.ConfigFile))
		{
			MessageBox.Show(this, "The layout file does not exist yet. It is written when a window is closed.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}

		Process.Start(new ProcessStartInfo("notepad.exe", $"\"{store.ConfigFile}\"") { UseShellExecute = true });
	}


	/// <summary>Handles the Written event of the sample log.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Log entry.</param>
	private void SampleLog_Written(object? sender, string e)
	{
		if(IsDisposed) { return; }

		_ListLog.BeginUpdate();
		_ListLog.Items.Add(e);
		while(_ListLog.Items.Count > _MAX_LOG_ENTRIES) { _ListLog.Items.RemoveAt(0); }
		_ListLog.TopIndex = _ListLog.Items.Count - 1;
		_ListLog.EndUpdate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		SampleLog.Written -= SampleLog_Written;
		base.OnFormClosed(e);
	}
}
