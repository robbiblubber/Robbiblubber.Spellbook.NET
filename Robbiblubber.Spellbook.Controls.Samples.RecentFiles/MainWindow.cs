namespace Robbiblubber.Spellbook.Controls.Samples.RecentFiles;

using System.Runtime.Versioning;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>The File menu has a <see cref="RecentFileList"/> in a submenu; the Database menu has a <see cref="RecentConnectionList"/>
///          shown inline between two separators. Several windows (File › New Window) share the lists.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;

	/// <summary>Maximum number of characters shown of a file.</summary>
	private const int _MAX_PREVIEW = 4000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of windows opened so far.</summary>
	private static int _WindowCount = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		_WindowCount++;
		Text += $" #{_WindowCount}";

		foreach(string i in SampleConnections.IDs)
		{
			string id = i;
			_MenuDbConnect.DropDownItems.Add(new ToolStripMenuItem(SampleConnections.GetName(id), null, (sender, e) => { _Connect(id); }));
		}

		_Log($"Lists stored in {((_RecentFiles.Store is DdnRecentListStore store) ? store.ConfigFile : "(custom store)")}");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log entry.</summary>
	/// <param name="message">Message.</param>
	private void _Log(string message)
	{
		_ListLog.BeginUpdate();
		_ListLog.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {message}");
		while(_ListLog.Items.Count > _MAX_LOG_ENTRIES) { _ListLog.Items.RemoveAt(0); }
		_ListLog.TopIndex = _ListLog.Items.Count - 1;
		_ListLog.EndUpdate();
	}


	/// <summary>Opens a file: shows its beginning and adds it to the recent files.</summary>
	/// <param name="path">Path.</param>
	private void _OpenFile(string path)
	{
		try
		{
			using StreamReader reader = new(path);
			char[] buffer = new char[_MAX_PREVIEW];
			int length = reader.ReadBlock(buffer, 0, buffer.Length);

			_TextContent.Text = $"File: {path}{Environment.NewLine}{Environment.NewLine}" + new string(buffer, 0, length).Replace("\0", "\u2400");
		}
		catch(Exception ex) when ((ex is IOException) || (ex is UnauthorizedAccessException))
		{
			_TextContent.Text = $"File: {path}{Environment.NewLine}{Environment.NewLine}Cannot read the file: {ex.Message}";
		}

		_RecentFiles.Add(path);
		_Log($"Opened      {path}");
	}


	/// <summary>Opens a folder: lists its content and adds it to the recent files.</summary>
	/// <param name="path">Path.</param>
	private void _OpenFolder(string path)
	{
		try
		{
			_TextContent.Text = $"Folder: {path}{Environment.NewLine}{Environment.NewLine}" + string.Join(Environment.NewLine, Directory.EnumerateFileSystemEntries(path).Take(200).Select(m => Path.GetFileName(m)));
		}
		catch(Exception ex) when ((ex is IOException) || (ex is UnauthorizedAccessException))
		{
			_TextContent.Text = $"Folder: {path}{Environment.NewLine}{Environment.NewLine}Cannot read the folder: {ex.Message}";
		}

		_RecentFiles.Add(path);
		_Log($"Opened      {path}");
	}


	/// <summary>Connects to a sample database and adds the connection to the recent connections.</summary>
	/// <param name="id">Connection ID.</param>
	private void _Connect(string id)
	{
		if(!SampleConnections.Exists(id))
		{
			MessageBox.Show(this, $"The connection '{id}' does not exist.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		_TextContent.Text = $"Connected to {SampleConnections.GetName(id)} (ID: {id}).";
		_RecentConnections.Add(id, SampleConnections.GetName(id));
		_Log($"Connected   {id}");
	}


	/// <summary>Asks if a missing entry is to be removed from the list.</summary>
	/// <param name="e">Event arguments.</param>
	/// <param name="what">Description of the entry.</param>
	private void _AskRemove(RecentItemEventArgs e, string what)
	{
		e.Remove = (MessageBox.Show(this, $"{what} does not exist any more.{Environment.NewLine}{Environment.NewLine}Remove it from the list?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
		_Log($"Missing     {e.Item.Key}: " + (e.Remove ? "removed" : "kept"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ItemSelected event of the recent files.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _RecentFiles_ItemSelected(object? sender, RecentItemEventArgs e)
	{
		_Log($"Selected    {e.Item.Key} (available: {e.Available?.ToString() ?? "unknown"}, pinned: {e.Item.Pinned})");

		if(e.Available == false)
		{
			_AskRemove(e, $"'{e.Item.Key}'");
			return;
		}

		if(Directory.Exists(e.Item.Key))
		{
			_OpenFolder(e.Item.Key);
		}
		else { _OpenFile(e.Item.Key); }
	}


	/// <summary>Handles the ItemSelected event of the recent connections.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _RecentConnections_ItemSelected(object? sender, RecentItemEventArgs e)
	{
		_Log($"Selected    connection {e.Item.Key} (available: {e.Available?.ToString() ?? "unknown"})");

		if(e.Available == false)
		{
			_AskRemove(e, $"The connection '{e.Item.Text ?? e.Item.Key}'");
			return;
		}

		_Connect(e.Item.Key);
	}


	/// <summary>Handles the Changed event of the recent lists.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Recent_Changed(object? sender, EventArgs e)
	{
		if(sender is RecentItemList list) { _Log($"Changed     {list.ListName}: " + string.Join(", ", list.Items.Select(m => (m.Pinned ? "*" : string.Empty) + m.Key))); }
	}


	/// <summary>Handles the changes of the options.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Options_Changed(object? sender, EventArgs e)
	{
		int max = Math.Max(1, (int) _NumMax.Value);

		_RecentFiles.DisplayStyle = (_ChkFullPath.Checked ? RecentFileDisplayStyle.FULL_PATH : RecentFileDisplayStyle.FILE_NAME);
		_RecentFiles.AddToWindowsRecent = _ChkWindowsRecent.Checked;

		foreach(RecentItemList i in new RecentItemList[] { _RecentFiles, _RecentConnections })
		{
			i.ShowNumbers = _ChkNumbers.Checked;
			i.ShowClearItem = _ChkClear.Checked;
			i.ShowImages = _ChkImages.Checked;
			i.MaxCount = max;
			i.Reload();
		}
	}


	/// <summary>Handles the Click event of the open file menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileOpen_Click(object? sender, EventArgs e)
	{
		using OpenFileDialog dialog = new() { Title = "Open File", Filter = "All files (*.*)|*.*" };

		if(dialog.ShowDialog(this) == DialogResult.OK) { _OpenFile(dialog.FileName); }
	}


	/// <summary>Handles the Click event of the open folder menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileOpenFolder_Click(object? sender, EventArgs e)
	{
		using FolderBrowserDialog dialog = new() { Description = "Open Folder", UseDescriptionForTitle = true };

		if(dialog.ShowDialog(this) == DialogResult.OK) { _OpenFolder(dialog.SelectedPath); }
	}


	/// <summary>Handles the Click event of the new window menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileNewWindow_Click(object? sender, EventArgs e)
	{
		new MainWindow().Show();
	}


	/// <summary>Handles the Click event of the exit menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileExit_Click(object? sender, EventArgs e)
	{
		Close();
	}


	/// <summary>Handles the CheckedChanged event of the menu item that drops the staging connection.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuDbDropStaging_CheckedChanged(object? sender, EventArgs e)
	{
		SampleConnections.SetDropped("staging", _MenuDbDropStaging.Checked);
		_Log("Connection  staging " + (_MenuDbDropStaging.Checked ? "dropped" : "restored"));
	}
}
