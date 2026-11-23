namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Diagnostics;
using System.Runtime.Versioning;



/// <summary>This class implements the performance scenario.</summary>
/// <remarks>Generates a large number of items and measures the time from a keystroke until the list has been updated. The text box
///          events are subscribed after <see cref="InitializeComponent"/>, so the measuring handlers run after the provider handlers.</remarks>
[SupportedOSPlatform("windows")]
public partial class PerformanceControl: UserControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Stopwatch for keystrokes.</summary>
	private readonly Stopwatch _KeystrokeWatch = new();

	/// <summary>Stopwatch for result processing.</summary>
	private readonly Stopwatch _PopulateWatch = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public PerformanceControl()
	{
		InitializeComponent();

		SampleImages.Fill(_Ilist);

		_TextSearch.KeyPress += _TextSearch_KeyPress;
		_TextSearch.TextChanged += _TextSearch_TextChanged;

		SampleLog.Attach(_AutoCompletion, "Performance");
		_Generate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Generates the items and sets the source.</summary>
	private void _Generate()
	{
		Stopwatch watch = Stopwatch.StartNew();
		List<IAutoCompletionItem> items = SampleData.GenerateIdentifiers((int) _NumCount.Value);

		watch.Stop();
		_AutoCompletion.SetSource(_TextSearch, new AutoCompletionItemSource(items));
		_LabelGenerated.Text = $"{items.Count:N0} items generated in {watch.Elapsed.TotalMilliseconds:N1} ms.";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Click event of the generate button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonGenerate_Click(object? sender, EventArgs e)
	{
		Cursor = Cursors.WaitCursor;
		try
		{
			_Generate();
		}
		finally { Cursor = Cursors.Default; }
	}


	/// <summary>Handles the KeyPress event of the search text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextSearch_KeyPress(object? sender, KeyPressEventArgs e)
	{
		_KeystrokeWatch.Restart();
	}


	/// <summary>Handles the TextChanged event of the search text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextSearch_TextChanged(object? sender, EventArgs e)
	{
		if(!_KeystrokeWatch.IsRunning) { return; }

		_KeystrokeWatch.Stop();
		_LabelKeystroke.Text = $"Last keystroke (local filtering + drawing setup): {_KeystrokeWatch.Elapsed.TotalMilliseconds:N2} ms";
	}


	/// <summary>Handles the Populating event of the auto completion provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AutoCompletion_Populating(object? sender, AutoCompletionPopulatingEventArgs e)
	{
		_PopulateWatch.Restart();
		BeginInvoke(new Action(() =>
		{
			_PopulateWatch.Stop();
			_LabelPopulate.Text = $"Last result ({e.Items.Count:N0} items): sorting + filtering + showing took {_PopulateWatch.Elapsed.TotalMilliseconds:N2} ms";
		}));
	}
}
