namespace Robbiblubber.Spellbook.Controls.Samples.ProgressMeter;

using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>The track bar sets the value of all meters; the buttons change the value of the first meter, which updates the track bar.
///          The options apply to all meters.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Themes of the color scheme combo box.</summary>
	private static readonly Theme[] _Themes = new Theme[] { Theme.SYSTEM, Theme.LIGHT, Theme.DARK };



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Larger font, NULL if it has not been created yet.</summary>
	private Font? _LargeFont = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		_ComboScheme.Items.AddRange("System", "Light", "Dark");
		_ComboTextStyle.Items.AddRange(Enum.GetNames<ProgressMeterTextStyle>());
		_ComboTextStyle.SelectedIndex = (int) ProgressMeterTextStyle.PERCENT;
		_ComboScheme.SelectedIndex = 0;

		_TrackValue.Value = 40;
		_Log("Ready.");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets all meters.</summary>
	private Robbiblubber.Spellbook.Controls.ProgressMeter[] _Meters
	{
		get { return new Robbiblubber.Spellbook.Controls.ProgressMeter[] { _MeterMain, _MeterRange, _MeterMarquee, _MeterThin, _MeterVertical1, _MeterVertical2, _MeterVertical3 }; }
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



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ValueChanged event of the track bar: sets the value of all meters.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TrackValue_ValueChanged(object? sender, EventArgs e)
	{
		foreach(Robbiblubber.Spellbook.Controls.ProgressMeter i in _Meters) { i.Value = _TrackValue.Value; }
		_MeterRange.Value = 50 + _TrackValue.Value;
	}


	/// <summary>Handles the ValueChanged event of the first meter.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MeterMain_ValueChanged(object? sender, EventArgs e)
	{
		_Log($"ValueChanged   Value = {_MeterMain.Value}, Fraction = {_MeterMain.Fraction:0.00}");
		if(_TrackValue.Value != _MeterMain.Value) { _TrackValue.Value = _MeterMain.Value; }
	}


	/// <summary>Handles the SelectedIndexChanged event of the theme combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboScheme_SelectedIndexChanged(object? sender, EventArgs e)
	{
		Theme theme = _Themes[Math.Max(0, _ComboScheme.SelectedIndex)];

		foreach(Robbiblubber.Spellbook.Controls.ProgressMeter i in _Meters) { i.Colors.Theme = theme; }
		_ComboScheme.Colors.Theme = _ComboTextStyle.Colors.Theme = theme;

		(BackColor, ForeColor, _ListLog.BackColor, _ListLog.ForeColor) = (theme[ThemeColor.BACK], theme[ThemeColor.TEXT], theme[ThemeColor.FIELD_BACK], theme[ThemeColor.FIELD_TEXT]);

		_Log($"Theme: {theme.Name}");
	}


	/// <summary>Handles the changes of the options.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Options_Changed(object? sender, EventArgs e)
	{
		foreach(Robbiblubber.Spellbook.Controls.ProgressMeter i in _Meters)
		{
			i.Enabled = _ChkEnabled.Checked;
			if(i != _MeterThin) { i.BorderStyle = (_ChkBorder.Checked ? BorderStyle.FixedSingle : BorderStyle.None); }
			i.Colors.BarColor = (_ChkGreen.Checked ? Color.FromArgb(0x06, 0xB0, 0x25) : Color.Empty);
			i.Font = (_ChkFont.Checked ? (_LargeFont ??= new Font(Font.FontFamily, 11f, FontStyle.Bold)) : null);
		}

		_MeterMain.TextStyle = (ProgressMeterTextStyle) Math.Max(0, _ComboTextStyle.SelectedIndex);
	}


	/// <summary>Handles the Click event of the step button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonStep_Click(object? sender, EventArgs e)
	{
		_MeterMain.PerformStep();
	}


	/// <summary>Handles the Click event of the decrement button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDecrement_Click(object? sender, EventArgs e)
	{
		_MeterMain.Increment(-5);
	}


	/// <summary>Handles the Click event of the simulate button: runs the first meter from 0 to 100.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonSimulate_Click(object? sender, EventArgs e)
	{
		_MeterMain.Value = 0;
		_TimerWork.Start();
	}


	/// <summary>Handles the Tick event of the work timer.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TimerWork_Tick(object? sender, EventArgs e)
	{
		_MeterMain.Increment(1);
		if(_MeterMain.Value >= _MeterMain.Maximum) { _TimerWork.Stop(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);

		_TimerWork.Stop();
		_LargeFont?.Dispose();
		_LargeFont = null;
	}
}
