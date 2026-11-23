namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.Versioning;



/// <summary>This class implements the main window of the sample application.</summary>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 2000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();
		SampleLog.Written += SampleLog_Written;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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


	/// <summary>Handles the Click event of the copy log button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonCopyLog_Click(object? sender, EventArgs e)
	{
		if(_ListLog.Items.Count == 0) { return; }
		Clipboard.SetText(string.Join(Environment.NewLine, _ListLog.Items.Cast<object>()));
	}


	/// <summary>Handles the CheckedChanged event of the log selection check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkLogSelection_CheckedChanged(object? sender, EventArgs e)
	{
		SampleLog.LogSelection = _ChkLogSelection.Checked;
	}


	/// <summary>Handles the Click event of the clear log button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonClearLog_Click(object? sender, EventArgs e)
	{
		_ListLog.Items.Clear();
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
