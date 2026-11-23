namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.Versioning;



/// <summary>This class implements the SQL scenario.</summary>
/// <remarks>Demonstrates the signature help together with the auto completion: inserting a function from the auto completion list starts
///          the signature help, an open auto completion list takes precedence for Escape and the navigation keys, and the signature
///          window inherits the colors of the auto completion provider. The SQL parser recognizes strings and comments; the source
///          provides overloads, variadic parameters, a qualifier-dependent user-defined function, a simulated latency and failures.
///          The same parser instance tells the auto completion not to start in comments and strings.</remarks>
[SupportedOSPlatform("windows")]
public partial class SqlControl: UserControl
{
    //#H private members

    /// <summary>Signature source.</summary>
    private readonly SqlSampleSignatureSource _Signatures = new();

    /// <summary>SQL parser, shared by the signature help and the comment and string check of the auto completion.</summary>
    private readonly SignatureCallParser _Parser = new() { StringDelimiters = "'\"", LineComment = "--", BlockCommentStart = "/*", BlockCommentEnd = "*/" };

    /// <summary>Determines if the highlighter is running.</summary>
    private bool _Highlighting = false;



    //#H constructors

    /// <summary>Creates a new instance of this class.</summary>
    public SqlControl()
    {
        InitializeComponent();

        SampleImages.Fill(_Ilist);

        _AutoCompletion.SetSource(_TextQuery, new SqlSampleSource());
        _SignatureHelp.Source = _Signatures;
        _SignatureHelp.Parser = _Parser;
        _AutoCompletion.Starting += _AutoCompletion_Starting;

        _TextQuery.Text = string.Join(Environment.NewLine, new string[]
        {
            "-- Try: ROUND(  CONVERT(int, Total,  COALESCE(a, b, c,  dbo.GetCustomerTotal(",
            "SELECT c.Name, SUBSTRING(c.Name, 1, 3), ROUND(SUM(o.Total), 2)",
            "FROM Customers c",
            "JOIN Orders o ON o.CustomerId = c.CustomerId",
            "WHERE "
        });

        _UpdateLatencyLabel();
        _TextQuery.TextChanged += _TextQuery_TextChanged;

        SampleLog.Attach(_SignatureHelp, "SQL");
        SampleLog.Attach(_AutoCompletion, "SQL");
    }



    //#H private methods

    /// <summary>Updates the latency label.</summary>
    private void _UpdateLatencyLabel()
    {
        _LabelLatency.Text = $"Signature source latency: {_TrackLatency.Value} ms";
    }


    /// <summary>Highlights the keywords of the query text.</summary>
    /// <remarks>The highlighter suspends the tracking of both providers while it changes colors.</remarks>
    private void _Highlight()
    {
        if(_Highlighting) { return; }

        _Highlighting = true;
        try
        {
            using(_AutoCompletion.SuspendTracking(_TextQuery))
            using(_SignatureHelp.SuspendTracking(_TextQuery))
            {
                SqlSampleHighlighter.Highlight(_TextQuery, Color.RoyalBlue);
            }
        }
        finally { _Highlighting = false; }
    }



    //#H event handlers

    /// <summary>Handles the ValueChanged event of the latency track bar.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _TrackLatency_ValueChanged(object? sender, EventArgs e)
    {
        _Signatures.Latency = _TrackLatency.Value;
        _UpdateLatencyLabel();
    }


    /// <summary>Handles the CheckedChanged event of the exception check box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _ChkThrow_CheckedChanged(object? sender, EventArgs e)
    {
        _Signatures.ThrowException = _ChkThrow.Checked;
    }


    /// <summary>Handles the CheckedChanged event of the link check box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _ChkLink_CheckedChanged(object? sender, EventArgs e)
    {
        _SignatureHelp.AutoCompletion = (_ChkLink.Checked ? _AutoCompletion : null);
    }


    /// <summary>Handles the CheckedChanged event of the highlighting check box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    private void _ChkHighlight_CheckedChanged(object? sender, EventArgs e)
    {
        if(_ChkHighlight.Checked)
        {
            _Highlight();
            return;
        }

        using(_AutoCompletion.SuspendTracking(_TextQuery))
        using(_SignatureHelp.SuspendTracking(_TextQuery))
        {
            int start = _TextQuery.SelectionStart, length = _TextQuery.SelectionLength;

            _TextQuery.SelectAll();
            _TextQuery.SelectionColor = _TextQuery.ForeColor;
            _TextQuery.Select(start, length);
        }
    }


    /// <summary>Handles the CheckedChanged event of the dark theme check box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>Only the auto completion provider gets colors; the signature window inherits them while it is linked.</remarks>
    private void _ChkDark_CheckedChanged(object? sender, EventArgs e)
    {
        bool dark = _ChkDark.Checked;

        _TextQuery.BackColor = (dark ? Color.FromArgb(30, 30, 30) : SystemColors.Window);
        _TextQuery.ForeColor = (dark ? Color.FromArgb(220, 220, 220) : SystemColors.WindowText);

        _AutoCompletion.ListBackColor = (dark ? Color.FromArgb(37, 37, 38) : Color.Empty);
        _AutoCompletion.ListForeColor = (dark ? Color.FromArgb(220, 220, 220) : Color.Empty);
        _AutoCompletion.SelectionBackColor = (dark ? Color.FromArgb(4, 57, 94) : Color.Empty);
        _AutoCompletion.SelectionForeColor = (dark ? Color.White : Color.Empty);
        _AutoCompletion.DetailForeColor = (dark ? Color.FromArgb(140, 140, 140) : Color.Empty);
        _AutoCompletion.HighlightStyle = (dark ? ListHighlightStyle.COLOR : ListHighlightStyle.BOLD);
        _AutoCompletion.HighlightColor = (dark ? Color.FromArgb(24, 163, 255) : Color.Empty);
        _AutoCompletion.BorderColor = (dark ? Color.FromArgb(69, 69, 69) : Color.Empty);
        _SignatureHelp.HighlightStyle = (dark ? ListHighlightStyle.COLOR : ListHighlightStyle.BOLD);

        if(_ChkHighlight.Checked) { _Highlight(); }
    }


    /// <summary>Handles the Starting event of the auto completion provider.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>Cancelling the start is cheaper and quieter than a source that returns no items: no source request, no Closed event.</remarks>
    private void _AutoCompletion_Starting(object? sender, AutoCompletionStartingEventArgs e)
    {
        if(_ChkSyntax.Checked && (_Parser.GetSyntaxState(e.Context.Text, e.Context.CaretIndex) != SignatureCallParser.SyntaxState.CODE)) { e.Cancel = true; }
    }


    /// <summary>Handles the TextChanged event of the query text box.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>This handler is subscribed after <see cref="InitializeComponent"/>, so it runs after the handlers of both providers.</remarks>
    private void _TextQuery_TextChanged(object? sender, EventArgs e)
    {
        if(_ChkHighlight.Checked) { _Highlight(); }
    }
}
