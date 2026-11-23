namespace Robbiblubber.Spellbook.Controls.Samples.PasswordStrength;

using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the main window of the sample application.</summary>
/// <remarks>Both indicators rate the text of the password box. The label shows the result of the built-in heuristic, which the
///          policy option can override through the Rating event.</remarks>
[SupportedOSPlatform("windows")]
public partial class MainWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum number of log entries.</summary>
	private const int _MAX_LOG_ENTRIES = 500;

	/// <summary>Minimum length of the sample password policy.</summary>
	private const int _POLICY_LENGTH = 12;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Themes of the color scheme combo box.</summary>
	private static readonly Theme[] _Themes = new Theme[] { Theme.SYSTEM, Theme.LIGHT, Theme.DARK };

	/// <summary>Sample passwords.</summary>
	private static readonly string[] _Samples = new string[]
	{
		"a", "password", "Password1!", "P@ssw0rd", "Sommer2024!", "qwertz123", "aaaaaaaaaaaa", "abcdefgh", "Aa1!Aa1!", "aB3$eF", "MyDog2015",
		"ilovemycat", "Xk9#mP2$", "Xk9#mP2$vL", "Tr0ub4dor&3", "Xk9#mP2$vLq7", "Xk9#mP2$vLq7!wZ4", "correct horse battery staple"
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Background image of the indicator panel.</summary>
	private readonly Bitmap _Background = new(588, 62);

	/// <summary>Larger font, NULL if it has not been created yet.</summary>
	private Font? _LargeFont = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MainWindow()
	{
		InitializeComponent();

		using(Graphics g = Graphics.FromImage(_Background))
		using(LinearGradientBrush brush = new(new Rectangle(0, 0, _Background.Width, _Background.Height), Color.FromArgb(0xB3, 0xE5, 0xFC), Color.FromArgb(0xF8, 0xBB, 0xD0), LinearGradientMode.Horizontal))
		{
			g.FillRectangle(brush, 0, 0, _Background.Width, _Background.Height);
		}
		_PanelBack.BackgroundImage = _Background;
		_PanelBack.BackgroundImageLayout = ImageLayout.Stretch;

		_Indicator1.Source = _TextPassword;
		_Indicator2.Source = _TextPassword;

		_ComboSamples.Items.AddRange(_Samples);
		_ComboScheme.Items.AddRange("System", "Light", "Dark");
		_ComboScheme.SelectedIndex = 0;

		_Log("Ready.");
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


	/// <summary>Sets the texts of an indicator.</summary>
	/// <param name="indicator">Indicator.</param>
	/// <param name="german">Determines if German texts are set.</param>
	private static void _SetTexts(PasswordStrengthIndicator indicator, bool german)
	{
		indicator.NoneText = (german ? "Kein Passwort" : string.Empty);
		indicator.VeryWeakText = (german ? "Sehr schwach" : "Very weak");
		indicator.WeakText = (german ? "Schwach" : "Weak");
		indicator.FairText = (german ? "Mittel" : "Fair");
		indicator.GoodText = (german ? "Gut" : "Good");
		indicator.StrongText = (german ? "Stark" : "Strong");
		indicator.VeryStrongText = (german ? "Sehr stark" : "Very strong");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Rating event of the indicators: applies the sample password policy.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Indicator_Rating(object? sender, PasswordRatingEventArgs e)
	{
		if(_ChkPolicy.Checked && (e.Password.Length < _POLICY_LENGTH) && (e.Strength > PasswordStrength.WEAK))
		{
			e.Strength = PasswordStrength.WEAK;
		}
	}


	/// <summary>Handles the ValueChanged event of the first indicator.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Indicator1_ValueChanged(object? sender, EventArgs e)
	{
		_Log($"ValueChanged   {_Indicator1.Value}");
	}


	/// <summary>Handles the ValueEvaluated event of the first indicator: shows the heuristic and the value.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>ValueEvaluated is raised for every evaluation, also when the value does not change, so the label is up to date from
	///          the start.</remarks>
	private void _Indicator1_ValueEvaluated(object? sender, EventArgs e)
	{
		PasswordStrength rated = PasswordStrengthIndicator.Rate(_TextPassword.Text);

		_LabelRate.Text = $"Heuristic: {rated}{Environment.NewLine}Value: {_Indicator1.Value}" + ((rated != _Indicator1.Value) ? " (policy)" : string.Empty);
	}


	/// <summary>Handles the SelectionChangeCommitted event of the sample combo box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ComboSamples_SelectionChangeCommitted(object? sender, EventArgs e)
	{
		_TextPassword.Text = _ComboSamples.Text;
	}


	/// <summary>Handles the changes of the options.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Options_Changed(object? sender, EventArgs e)
	{
		Theme theme = _Themes[Math.Max(0, _ComboScheme.SelectedIndex)];

		_TextPassword.UseSystemPasswordChar = (!_ChkShow.Checked);

		foreach(PasswordStrengthIndicator i in new PasswordStrengthIndicator[] { _Indicator1, _Indicator2 })
		{
			i.Colors.Theme = theme;
			i.Enabled = _ChkEnabled.Checked;
			i.UseRoundedCorners = (_ChkRounded.Checked && (i == _Indicator1));
			i.BoxSpacing = (int) _NumSpacing.Value;
			i.Font = (_ChkFont.Checked ? (_LargeFont ??= new Font(Font.FontFamily, 11f, FontStyle.Bold)) : null);
			i.BackColor = (_ChkTransparent.Checked ? Color.Transparent : Color.Empty);

			_SetTexts(i, _ChkGerman.Checked);
			i.Evaluate(_TextPassword.Text);
		}

		_ComboScheme.Colors.Theme = _ComboSamples.Colors.Theme = theme;
		_PanelBack.BackColor = ((theme == Theme.DARK) ? Color.FromArgb(0x20, 0x20, 0x20) : Color.Empty);
		_PanelBack.BackgroundImage = ((theme == Theme.DARK) ? null : _Background);
		_LabelRate.ForeColor = ((theme == Theme.DARK) ? Color.FromArgb(0xF0, 0xF0, 0xF0) : Color.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);

		_PanelBack.BackgroundImage = null;
		_Background.Dispose();
		_LargeFont?.Dispose();
		_LargeFont = null;
	}
}
