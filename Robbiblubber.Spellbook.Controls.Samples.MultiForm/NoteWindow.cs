namespace Robbiblubber.Spellbook.Controls.Samples.MultiForm;

using System.Diagnostics;
using System.Runtime.Versioning;



/// <summary>This class implements a note window; every window shows one file or an empty note.</summary>
[SupportedOSPlatform("windows")]
public partial class NoteWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Number of untitled windows.</summary>
	private static int _Untitled = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="fileName">File name, NULL for an empty note.</param>
	/// <param name="info">Text shown in the status bar.</param>
	public NoteWindow(string? fileName, string info)
	{
		InitializeComponent();

		FileName = fileName;
		if(fileName == null)
		{
			Text = "Untitled " + (++_Untitled) + " - Multi Form Sample";
		}
		else
		{
			Text = Path.GetFileName(fileName) + " - Multi Form Sample";
			try
			{
				_TextContent.Text = File.ReadAllText(fileName);
			}
			catch(Exception ex) { _TextContent.Text = "The file could not be read: " + ex.Message; }
		}

		ShowInfo(info);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Gets the file name, NULL for an empty note.</summary>
	public string? FileName
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Shows a text in the status bar.</summary>
	/// <param name="info">Text.</param>
	public void ShowInfo(string info)
	{
		_LabelInfo.Text = DateTime.Now.ToString("HH:mm:ss") + "  " + info;
		_UpdateStatus();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Updates the process and window count in the status bar.</summary>
	private void _UpdateStatus()
	{
		_LabelProcess.Text = "Process " + Environment.ProcessId + ", " + Program.Context.Forms.Count + " window(s)";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Window activated.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _NoteWindow_Activated(object? sender, EventArgs e)
	{
		_UpdateStatus();
	}


	/// <summary>File > New Window.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileNew_Click(object? sender, EventArgs e)
	{
		Program.Context.ShowForm(new NoteWindow(null, "Opened with File > New Window"));
	}


	/// <summary>File > Open.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileOpen_Click(object? sender, EventArgs e)
	{
		using OpenFileDialog d = new() { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*", Multiselect = true };
		if(d.ShowDialog(this) == DialogResult.OK)
		{
			Program.Open(d.FileNames, Environment.CurrentDirectory, "Opened with File > Open");
		}
	}


	/// <summary>File > Start Further Instance.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileStart_Click(object? sender, EventArgs e)
	{
		string args = (FileName == null) ? string.Empty : ("\"" + FileName + "\"");
		Process.Start(new ProcessStartInfo(Environment.ProcessPath!, args) { UseShellExecute = false });
	}


	/// <summary>File > Close.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileClose_Click(object? sender, EventArgs e)
	{
		Close();
	}


	/// <summary>File > Exit.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuFileExit_Click(object? sender, EventArgs e)
	{
		foreach(Form i in Program.Context.Forms.ToArray()) { i.Close(); }
	}


	/// <summary>Window menu opening.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuWindow_DropDownOpening(object? sender, EventArgs e)
	{
		_MenuWindow.DropDownItems.Clear();

		int n = 1;
		foreach(Form i in Program.Context.Forms)
		{
			ToolStripMenuItem m = new("&" + n++ + " " + i.Text.Replace("&", "&&")) { Checked = (i == this), Tag = i };
			m.Click += _MenuWindowItem_Click;
			_MenuWindow.DropDownItems.Add(m);
		}
	}


	/// <summary>Window menu item clicked.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuWindowItem_Click(object? sender, EventArgs e)
	{
		if((sender as ToolStripMenuItem)?.Tag is Form f) { MultiFormApplicationContext.ActivateForm(f); }
	}
}
