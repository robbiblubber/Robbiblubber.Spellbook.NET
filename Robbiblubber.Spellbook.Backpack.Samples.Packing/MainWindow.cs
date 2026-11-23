namespace Robbiblubber.Spellbook.Backpack.Samples.Packing;

using System.Runtime.Versioning;
using System.Text;
using Robbiblubber.Spellbook.Backpack;



/// <summary>This class implements the main window of the Backpack sample.</summary>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();
		_Log("Backpack sample started. Executable: " + Environment.ProcessPath);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the tag, or NULL if the tag box is empty.</summary>
	private string? _Tag
	{
		get { return (string.IsNullOrEmpty(_TextTag.Text) ? null : _TextTag.Text); }
	}


	/// <summary>Gets a description of the tag for the log.</summary>
	private string _TagText
	{
		get { return ((_Tag is null) ? "tag: none" : $"tag: \"{_Tag}\""); }
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
	private async void _Run(Func<bool, Task> action)
	{
		_PanelButtons.Enabled = false;
		try
		{
			await action(_CheckAsync.Checked);
		}
		catch(Exception ex) { _Log($"Error: {ex.GetType().Name}: {ex.Message}"); }
		finally { _PanelButtons.Enabled = true; }
	}


	/// <summary>Shows a file dialog and puts the file name into a text box.</summary>
	/// <param name="box">Text box.</param>
	/// <param name="save">Determines if a save dialog is shown.</param>
	private static void _Browse(TextBox box, bool save)
	{
		using FileDialog dlg = (save ? new SaveFileDialog() : new OpenFileDialog());
		dlg.FileName = box.Text;
		if(dlg.ShowDialog() == DialogResult.OK) { box.Text = dlg.FileName; }
	}


	/// <summary>Returns a short preview of payload data.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the preview text.</returns>
	private static string _Preview(byte[] data)
	{
		string rval = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 80)).ReplaceLineEndings(" ");
		return ((data.Length > 80) ? (rval + "...") : rval);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Click event of the carrier browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseCarrier_Click(object sender, EventArgs e)
	{
		_Browse(_TextCarrier, false);
	}


	/// <summary>Handles the Click event of the payload browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowsePayload_Click(object sender, EventArgs e)
	{
		_Browse(_TextPayload, false);
	}


	/// <summary>Handles the Click event of the target browse button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonBrowseTarget_Click(object sender, EventArgs e)
	{
		_Browse(_TextTarget, true);
	}


	/// <summary>Handles the Click event of the append button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAppend_Click(object sender, EventArgs e)
	{
		_Run(async m =>
		{
			if(m) { await Packer.AppendAsync(_TextCarrier.Text, _TextPayload.Text, _Tag); } else { Packer.Append(_TextCarrier.Text, _TextPayload.Text, _Tag); }
			_Log($"Appended '{Path.GetFileName(_TextPayload.Text)}' to '{Path.GetFileName(_TextCarrier.Text)}' ({new FileInfo(_TextPayload.Text).Length} bytes, {_TagText}).");
		});
	}


	/// <summary>Handles the Click event of the info button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonInfo_Click(object sender, EventArgs e)
	{
		_Run(m =>
		{
			PayloadInfo? info = Packer.GetInfo(_TextCarrier.Text);
			_Log((info is null) ? "Info: no payload." : $"Info: {info}, file {new FileInfo(_TextCarrier.Text).Length} bytes.");
			if(_Tag is not null) { _Log($"HasPayload({_TagText}): {Packer.HasPayload(_TextCarrier.Text, _Tag)}"); }

			return Task.CompletedTask;
		});
	}


	/// <summary>Handles the Click event of the verify button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonVerify_Click(object sender, EventArgs e)
	{
		_Run(async m =>
		{
			bool ok = (m ? await Packer.VerifyAsync(_TextCarrier.Text, _Tag) : Packer.Verify(_TextCarrier.Text, _Tag));
			_Log(ok ? "Verify: payload intact." : "Verify: no matching payload or payload damaged.");
		});
	}


	/// <summary>Handles the Click event of the extract button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonExtract_Click(object sender, EventArgs e)
	{
		_Run(async m =>
		{
			if(m) { await Packer.ExtractAsync(_TextCarrier.Text, _TextTarget.Text, _Tag); } else { Packer.Extract(_TextCarrier.Text, _TextTarget.Text, _Tag); }
			_Log($"Extracted to '{Path.GetFileName(_TextTarget.Text)}' ({new FileInfo(_TextTarget.Text).Length} bytes).");
		});
	}


	/// <summary>Handles the Click event of the remove button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonRemove_Click(object sender, EventArgs e)
	{
		_Run(m =>
		{
			bool removed = Packer.Remove(_TextCarrier.Text, _Tag);
			_Log(removed ? $"Payload removed, file {new FileInfo(_TextCarrier.Text).Length} bytes." : "Remove: no matching payload.");

			return Task.CompletedTask;
		});
	}


	/// <summary>Handles the Click event of the own payload button.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonOwn_Click(object sender, EventArgs e)
	{
		_Run(async m =>
		{
			string exe = Environment.ProcessPath!;
			if(!Packer.HasPayload(exe, _Tag)) { _Log($"Own payload: none ({_TagText})."); return; }

			using Stream s = Packer.OpenPayload(exe, _Tag);
			byte[] data = new byte[s.Length];
			if(m) { await s.ReadExactlyAsync(data); } else { s.ReadExactly(data); }

			_Log($"Own payload: {data.Length} bytes, verified: {Packer.Verify(exe, _Tag)}.");
			_Log("Content: " + _Preview(data));
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
