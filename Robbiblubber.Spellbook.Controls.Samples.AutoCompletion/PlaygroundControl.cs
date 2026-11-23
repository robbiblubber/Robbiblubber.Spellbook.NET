namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class implements the playground scenario.</summary>
/// <remarks>The property grid shows the provider settings, which can be changed while typing. The combo boxes set the per-control
///          overrides, which take precedence over the provider settings.</remarks>
[SupportedOSPlatform("windows")]
public partial class PlaygroundControl: UserControl
{
    //#H private constants

    /// <summary>Text for the "use provider setting" option.</summary>
    private const string _INHERIT = "(Provider)";



    //#H constructors

    /// <summary>Creates a new instance of this class.</summary>
    public PlaygroundControl()
    {
        InitializeComponent();

        SampleImages.Fill(_Ilist);

        _CboMatchMode.Items.AddRange(new object[] { _INHERIT, ListMatchMode.PREFIX, ListMatchMode.CONTAINS, ListMatchMode.FUZZY });
        _CboFilterMode.Items.AddRange(new object[] { _INHERIT, ListFilterMode.FILTER, ListFilterMode.SELECT });
        _CboCaseSensitive.Items.AddRange(new object[] { _INHERIT, true, false });
        _CboMatchMode.SelectedIndex = _CboFilterMode.SelectedIndex = _CboCaseSensitive.SelectedIndex = 0;

        _AutoCompletion.SetSource(_TextEditor, new AutoCompletionItemSource(SampleData.CreateTypeItems()));
        _GridProvider.SelectedObject = _AutoCompletion;

        SampleLog.Attach(_AutoCompletion, "Playground");
    }



    //#H event handlers

    /// <summary>Handles the SelectedIndexChanged event of the match mode combo box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _CboMatchMode_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _AutoCompletion.SetMatchMode(_TextEditor, (_CboMatchMode.SelectedItem is ListMatchMode m) ? m : null);
    }


    /// <summary>Handles the SelectedIndexChanged event of the filter mode combo box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _CboFilterMode_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _AutoCompletion.SetFilterMode(_TextEditor, (_CboFilterMode.SelectedItem is ListFilterMode m) ? m : null);
    }


    /// <summary>Handles the CheckedChanged event of the dark theme check box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>Demonstrates the color properties of the provider together with a dark text box.</remarks>
    private void _ChkDark_CheckedChanged(object? sender, EventArgs e)
    {
        bool dark = _ChkDark.Checked;

        _TextEditor.BackColor = (dark ? Color.FromArgb(30, 30, 30) : SystemColors.Window);
        _TextEditor.ForeColor = (dark ? Color.FromArgb(220, 220, 220) : SystemColors.WindowText);

        _AutoCompletion.ListBackColor = (dark ? Color.FromArgb(37, 37, 38) : Color.Empty);
        _AutoCompletion.ListForeColor = (dark ? Color.FromArgb(220, 220, 220) : Color.Empty);
        _AutoCompletion.SelectionBackColor = (dark ? Color.FromArgb(4, 57, 94) : Color.Empty);
        _AutoCompletion.SelectionForeColor = (dark ? Color.White : Color.Empty);
        _AutoCompletion.DetailForeColor = (dark ? Color.FromArgb(140, 140, 140) : Color.Empty);
        _AutoCompletion.HighlightStyle = (dark ? ListHighlightStyle.COLOR : ListHighlightStyle.BOLD);
        _AutoCompletion.HighlightColor = (dark ? Color.FromArgb(24, 163, 255) : Color.Empty);
        _AutoCompletion.BorderColor = (dark ? Color.FromArgb(69, 69, 69) : Color.Empty);

        _GridProvider.Refresh();
    }


    /// <summary>Handles the SelectedIndexChanged event of the case sensitivity combo box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _CboCaseSensitive_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _AutoCompletion.SetCaseSensitive(_TextEditor, (_CboCaseSensitive.SelectedItem is bool m) ? m : null);
    }
}
