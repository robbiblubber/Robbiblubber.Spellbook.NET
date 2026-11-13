namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class implements the SQL scenario.</summary>
/// <remarks>Demonstrates a context-sensitive source (<see cref="SqlSampleSource"/>), trigger characters ('.' and '['), custom replace
///          ranges, caret positioning after insertion, result-level match modes and the behavior with slow or failing sources.</remarks>
[SupportedOSPlatform("windows")]
public partial class SqlControl: UserControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>SQL auto completion source.</summary>
	private readonly SqlSampleSource _Source = new();

	/// <summary>Determines if the highlighter is running.</summary>
	private bool _Highlighting = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public SqlControl()
	{
		InitializeComponent();

		SampleImages.Fill(_Ilist);

		_AutoCompletion.SetSource(_TextQuery, _Source);
		_TextQuery.Text = "SELECT c.Name, o.Total" + Environment.NewLine + "FROM Customers c" + Environment.NewLine + "JOIN Orders o ON o.CustomerId = c.CustomerId" + Environment.NewLine + "WHERE ";

		_UpdateLatencyLabel();
		_TextQuery.TextChanged += _TextQuery_TextChanged;

		SampleLog.Attach(_AutoCompletion, "SQL");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Updates the latency label.</summary>
	private void _UpdateLatencyLabel()
	{
		_LabelLatency.Text = $"Simulated latency: {_TrackLatency.Value} ms";
	}


	/// <summary>Highlights the keywords of the query text.</summary>
	/// <remarks>With "Use SuspendTracking", the highlighter suspends the tracking of the auto completion provider. Without it, the
	///          provider has to recognize the formatting changes by itself.</remarks>
	private void _Highlight()
	{
		if(_Highlighting) { return; }

		_Highlighting = true;
		try
		{
			using(IDisposable? suspension = (_ChkSuspend.Checked ? _AutoCompletion.SuspendTracking(_TextQuery) : null))
			{
				SqlSampleHighlighter.Highlight(_TextQuery, Color.RoyalBlue);
			}
		}
		finally { _Highlighting = false; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ValueChanged event of the latency track bar.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TrackLatency_ValueChanged(object? sender, EventArgs e)
	{
		_Source.Latency = _TrackLatency.Value;
		_UpdateLatencyLabel();
	}


	/// <summary>Handles the CheckedChanged event of the exception check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkThrow_CheckedChanged(object? sender, EventArgs e)
	{
		_Source.ThrowException = _ChkThrow.Checked;
	}


	/// <summary>Handles the CheckedChanged event of the highlighting check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkHighlight_CheckedChanged(object? sender, EventArgs e)
	{
		if(_ChkHighlight.Checked)
		{
			_Highlight();
		}
		else
		{
			using(_AutoCompletion.SuspendTracking(_TextQuery))
			{
				int start = _TextQuery.SelectionStart, length = _TextQuery.SelectionLength;

				_TextQuery.SelectAll();
				_TextQuery.SelectionColor = _TextQuery.ForeColor;
				_TextQuery.Select(start, length);
			}
		}
	}


	/// <summary>Handles the CheckedChanged event of the usage ranking check box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ChkUsage_CheckedChanged(object? sender, EventArgs e)
	{
		_AutoCompletion.UsageRanking = _ChkUsage.Checked;
	}


	/// <summary>Handles the Click event of the export usage button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonExportUsage_Click(object? sender, EventArgs e)
	{
		string data = _AutoCompletion.UsageHistory.Export();

		if(data.Length == 0)
		{
			SampleLog.Write("SQL", "Usage history is empty.");
			return;
		}

		Clipboard.SetText(data);
		SampleLog.Write("SQL", $"Usage history exported to the clipboard ({data.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length} entries):");
		foreach(string i in data.Split('\n', StringSplitOptions.RemoveEmptyEntries)) { SampleLog.Write("SQL", "    " + i.Replace('\t', ' ')); }
	}


	/// <summary>Handles the Click event of the import usage button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonImportUsage_Click(object? sender, EventArgs e)
	{
		if(!Clipboard.ContainsText())
		{
			SampleLog.Write("SQL", "The clipboard does not contain text.");
			return;
		}

		_AutoCompletion.UsageHistory.Import(Clipboard.GetText());
		SampleLog.Write("SQL", "Usage history imported from the clipboard.");
	}


	/// <summary>Handles the Click event of the clear usage button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonClearUsage_Click(object? sender, EventArgs e)
	{
		_AutoCompletion.UsageHistory.Clear();
		SampleLog.Write("SQL", "Usage history cleared.");
	}


	/// <summary>Handles the TextChanged event of the query text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>This handler is subscribed after <see cref="InitializeComponent"/>, so it runs after the handler of the auto completion
	///          provider, like a highlighter added later in an application.</remarks>
	private void _TextQuery_TextChanged(object? sender, EventArgs e)
	{
		if(_ChkHighlight.Checked) { _Highlight(); }
	}
}
