namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the window that displays a signature.</summary>
/// <remarks>The window shows the signature with the active parameter highlighted, the description of the signature and the description
///          of the active parameter. If the function has several signatures, an indicator (▲ 2/3 ▼) allows switching between them.
///          Long signatures wrap before a parameter. The window is placed above the line of the call, or below if there is no space
///          above.</remarks>
[SupportedOSPlatform("windows")]
internal class _SignatureHelpWindow: _NoActivateWindowBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Padding in logical units.</summary>
	private const int _PADDING = 5;

	/// <summary>Vertical gap between sections in logical units.</summary>
	private const int _SECTION_GAP = 4;

	/// <summary>Gap between window and text line in logical units.</summary>
	private const int _LINE_GAP = 2;

	/// <summary>Indentation of wrapped signature lines in logical units.</summary>
	private const int _WRAP_INDENT = 16;

	/// <summary>Text format flags for single runs.</summary>
	private const TextFormatFlags _RUN_FLAGS = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

	/// <summary>Text format flags for wrapped text.</summary>
	private const TextFormatFlags _WRAP_FLAGS = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Signature text runs.</summary>
	private readonly List<_Run> _Runs = new();

	/// <summary>Signature description.</summary>
	private string? _Description = null;

	/// <summary>Bounds of the signature description.</summary>
	private Rectangle _DescriptionBounds = Rectangle.Empty;

	/// <summary>Label of the active parameter.</summary>
	private string? _ParameterLabel = null;

	/// <summary>Bounds of the active parameter label.</summary>
	private Rectangle _ParameterLabelBounds = Rectangle.Empty;

	/// <summary>Description of the active parameter.</summary>
	private string? _ParameterDescription = null;

	/// <summary>Bounds of the active parameter description.</summary>
	private Rectangle _ParameterDescriptionBounds = Rectangle.Empty;

	/// <summary>Bounds of the "previous signature" arrow.</summary>
	private Rectangle _PreviousBounds = Rectangle.Empty;

	/// <summary>Bounds of the "next signature" arrow.</summary>
	private Rectangle _NextBounds = Rectangle.Empty;

	/// <summary>Detail fore color.</summary>
	private Color _DetailForeColor = SystemColors.GrayText;

	/// <summary>Highlight color.</summary>
	private Color _HighlightColor = SystemColors.HotTrack;

	/// <summary>Highlight style.</summary>
	private ListHighlightStyle _HighlightStyle = ListHighlightStyle.BOLD;

	/// <summary>Font cache.</summary>
	private readonly Dictionary<FontStyle, Font> _Fonts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public _SignatureHelpWindow(): base()
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Measures the width of a single-line text.</summary>
	/// <param name="text">Text.</param>
	/// <param name="font">Font.</param>
	/// <returns>Returns the width.</returns>
	private static int _Measure(string text, Font font)
	{
		return ((text.Length == 0) ? 0 : TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), _RUN_FLAGS).Width);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a font for a font style.</summary>
	/// <param name="style">Font style.</param>
	/// <returns>Returns the font.</returns>
	private Font _GetFont(FontStyle style)
	{
		if(style == Font.Style) { return Font; }

		if(!_Fonts.TryGetValue(style, out Font? rval))
		{
			rval = new Font(Font, style);
			_Fonts.Add(style, rval);
		}

		return rval;
	}


	/// <summary>Disposes all cached fonts.</summary>
	private void _ClearFonts()
	{
		foreach(Font i in _Fonts.Values) { i.Dispose(); }
		_Fonts.Clear();
	}


	/// <summary>Gets font and color of the active parameter.</summary>
	/// <returns>Returns font and color.</returns>
	private (Font Font, Color Color) _GetActiveStyle()
	{
		return _HighlightStyle switch
		{
			ListHighlightStyle.BOLD => (_GetFont(FontStyle.Bold), ForeColor),
			ListHighlightStyle.UNDERLINE => (_GetFont(FontStyle.Underline), ForeColor),
			ListHighlightStyle.COLOR => (_GetFont(FontStyle.Bold), _HighlightColor),
			_ => (Font, ForeColor)
		};
	}


	/// <summary>Creates the wrapping units of a signature.</summary>
	/// <param name="signature">Signature.</param>
	/// <param name="parameterIndex">Index of the active parameter, -1 for none.</param>
	/// <returns>Returns the units; each unit is a list of text pieces that is kept on one line.</returns>
	private List<List<(string Text, Font Font, Color Color)>> _CreateUnits(SignatureInfo signature, int parameterIndex)
	{
		List<List<(string Text, Font Font, Color Color)>> rval = new();
		(Font Font, Color Color) active = _GetActiveStyle();
		List<(string Text, Font Font, Color Color)> current = new() { (signature.Name + signature.OpenText, Font, ForeColor) };

		for(int i = 0; i < signature.Parameters.Count; i++)
		{
			if(i > 0)
			{
				rval.Add(current);
				current = [];
			}

			current.Add((signature.Parameters[i].Label, ((i == parameterIndex) ? active.Font : Font), ((i == parameterIndex) ? active.Color : ForeColor)));
			current.Add((((i + 1) < signature.Parameters.Count) ? signature.SeparatorText : signature.CloseText, Font, ForeColor));
		}

		if(signature.Parameters.Count == 0) { current.Add((signature.CloseText, Font, ForeColor)); }
		rval.Add(current);

		if(!string.IsNullOrEmpty(signature.Suffix)) { rval.Add(new List<(string Text, Font Font, Color Color)> { (" " + signature.Suffix, Font, _DetailForeColor) }); }

		return rval;
	}


	/// <summary>Lays out the window content.</summary>
	/// <param name="signature">Signature.</param>
	/// <param name="signatureIndex">Index of the signature.</param>
	/// <param name="signatureCount">Number of signatures.</param>
	/// <param name="parameterIndex">Index of the active parameter, -1 for none.</param>
	/// <param name="showDescriptions">Determines if descriptions are displayed.</param>
	/// <param name="maximumWidth">Maximum client width.</param>
	/// <returns>Returns the client size.</returns>
	private Size _Layout(SignatureInfo signature, int signatureIndex, int signatureCount, int parameterIndex, bool showDescriptions, int maximumWidth)
	{
		List<List<(string Text, Font Font, Color Color)>> units = _CreateUnits(signature, parameterIndex);
		int padding = LogicalToDeviceUnits(_PADDING);
		int gap = LogicalToDeviceUnits(_SECTION_GAP);
		int lineHeight = Font.Height;
		int left = padding, x, y = padding, width = 0, contentWidth;
		SignatureParameter? parameter = (((parameterIndex >= 0) && (parameterIndex < signature.Parameters.Count)) ? signature.Parameters[parameterIndex] : null);

		_Runs.Clear();
		_PreviousBounds = _NextBounds = Rectangle.Empty;

		if(signatureCount > 1)
		{
			string counter = $" {signatureIndex + 1}/{signatureCount} ";
			int arrow = _Measure("▲", Font);

			_PreviousBounds = new Rectangle(left, y, arrow, lineHeight);
			_Runs.Add(new _Run("▲", Font, _DetailForeColor, new Point(left, y)));
			left += arrow;

			_Runs.Add(new _Run(counter, Font, _DetailForeColor, new Point(left, y)));
			left += _Measure(counter, Font);

			_NextBounds = new Rectangle(left, y, arrow, lineHeight);
			_Runs.Add(new _Run("▼", Font, _DetailForeColor, new Point(left, y)));
			left += arrow + LogicalToDeviceUnits(_PADDING);
		}

		contentWidth = Math.Max(left + LogicalToDeviceUnits(_WRAP_INDENT) * 4, maximumWidth - padding);
		x = left;

		foreach(List<(string Text, Font Font, Color Color)> i in units)
		{
			int unitWidth = i.Sum(m => _Measure(m.Text, m.Font));

			if(((x + unitWidth) > contentWidth) && (x > (left + LogicalToDeviceUnits(_WRAP_INDENT))))
			{
				x = left + LogicalToDeviceUnits(_WRAP_INDENT);
				y += lineHeight;
			}

			foreach((string Text, Font Font, Color Color) j in i)
			{
				_Runs.Add(new _Run(j.Text, j.Font, j.Color, new Point(x, y)));
				x += _Measure(j.Text, j.Font);
			}

			width = Math.Max(width, x);
		}

		y += lineHeight;
		width = Math.Min(Math.Max(width, padding), contentWidth);

		_Description = ((showDescriptions && (!string.IsNullOrWhiteSpace(signature.Description))) ? signature.Description : null);
		_ParameterLabel = _ParameterDescription = null;
		_DescriptionBounds = _ParameterLabelBounds = _ParameterDescriptionBounds = Rectangle.Empty;

		if(_Description is not null)
		{
			int natural = _Measure(_Description.Replace("\r", string.Empty).Split('\n').MaxBy(m => m.Length) ?? string.Empty, Font);
			width = Math.Min(contentWidth, Math.Max(width, padding + natural));
		}

		if(showDescriptions && (parameter is not null) && (!string.IsNullOrWhiteSpace(parameter.Description)))
		{
			_ParameterLabel = parameter.Label + ": ";
			_ParameterDescription = parameter.Description;
			width = Math.Min(contentWidth, Math.Max(width, padding + _Measure(_ParameterLabel, _GetFont(FontStyle.Bold)) + _Measure(_ParameterDescription, Font)));
		}

		if(_Description is not null)
		{
			Size size = TextRenderer.MeasureText(_Description, Font, new Size(Math.Max(1, width - padding), int.MaxValue), _WRAP_FLAGS);

			y += gap;
			_DescriptionBounds = new Rectangle(padding, y, width - padding, size.Height);
			y += size.Height;
		}

		if(_ParameterLabel is not null)
		{
			Font bold = _GetFont(FontStyle.Bold);
			int labelWidth = _Measure(_ParameterLabel, bold);
			int textLeft = (((labelWidth * 2) < (width - padding)) ? (padding + labelWidth) : padding);
			int textTop = ((textLeft == padding) ? (y + gap + lineHeight) : (y + gap));
			Size size = TextRenderer.MeasureText(_ParameterDescription, Font, new Size(Math.Max(1, width - textLeft), int.MaxValue), _WRAP_FLAGS);

			y += gap;
			_ParameterLabelBounds = new Rectangle(padding, y, labelWidth, lineHeight);
			_ParameterDescriptionBounds = new Rectangle(textLeft, textTop, width - textLeft, size.Height);
			y = textTop + size.Height;
		}

		return new Size(width + padding, y + padding);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the user has clicked the "previous signature" arrow.</summary>
	public event EventHandler? PreviousClicked;

	/// <summary>Occurs when the user has clicked the "next signature" arrow.</summary>
	public event EventHandler? NextClicked;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies font and colors.</summary>
	/// <param name="font">Font.</param>
	/// <param name="backColor">Back color.</param>
	/// <param name="foreColor">Fore color.</param>
	/// <param name="detailForeColor">Fore color of counter and suffix.</param>
	/// <param name="highlightColor">Highlight color.</param>
	/// <param name="highlightStyle">Highlight style of the active parameter.</param>
	/// <param name="borderColor">Border color, empty for the system border.</param>
	public void ApplyStyle(Font font, Color backColor, Color foreColor, Color detailForeColor, Color highlightColor, ListHighlightStyle highlightStyle, Color borderColor)
	{
		if(!font.Equals(Font))
		{
			_ClearFonts();
			Font = font;
		}

		BackColor = backColor;
		ForeColor = foreColor;
		_DetailForeColor = detailForeColor;
		_HighlightColor = highlightColor;
		_HighlightStyle = highlightStyle;
		BorderColor = borderColor;
	}


	/// <summary>Shows a signature.</summary>
	/// <param name="anchor">Screen position of the top left corner of the anchor character.</param>
	/// <param name="lineHeight">Line height.</param>
	/// <param name="signature">Signature.</param>
	/// <param name="signatureIndex">Index of the signature.</param>
	/// <param name="signatureCount">Number of signatures.</param>
	/// <param name="parameterIndex">Index of the active parameter, -1 for none.</param>
	/// <param name="showDescriptions">Determines if descriptions are displayed.</param>
	/// <param name="maximumWidth">Maximum window width.</param>
	/// <param name="owner">Owner form.</param>
	public void ShowSignature(Point anchor, int lineHeight, SignatureInfo signature, int signatureIndex, int signatureCount, int parameterIndex, bool showDescriptions, int maximumWidth, Form? owner)
	{
		Rectangle area = Screen.FromPoint(anchor).WorkingArea;
		int chrome = SizeFromClientSize(Size.Empty).Width;
		Size size = SizeFromClientSize(_Layout(signature, signatureIndex, signatureCount, parameterIndex, showDescriptions, Math.Min(maximumWidth, area.Width) - chrome));
		int gap = LogicalToDeviceUnits(_LINE_GAP);
		int x = anchor.X - LogicalToDeviceUnits(_PADDING) - (chrome / 2);
		int y = anchor.Y - gap - size.Height;

		if(y < area.Top) { y = anchor.Y + lineHeight + gap; }
		x = Math.Max(area.Left, Math.Min(x, area.Right - size.Width));

		ShowNoActivate(new Rectangle(x, y, size.Width, size.Height), owner);
		Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _ClearFonts(); }
		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;

		g.Clear(BackColor);

		foreach(_Run i in _Runs) { TextRenderer.DrawText(g, i.Text, i.Font, i.Location, i.Color, _RUN_FLAGS); }

		if(_Description is not null) { TextRenderer.DrawText(g, _Description, Font, _DescriptionBounds, ForeColor, _WRAP_FLAGS); }

		if(_ParameterLabel is not null)
		{
			TextRenderer.DrawText(g, _ParameterLabel, _GetFont(FontStyle.Bold), _ParameterLabelBounds, ForeColor, _RUN_FLAGS);
			TextRenderer.DrawText(g, _ParameterDescription, Font, _ParameterDescriptionBounds, ForeColor, _WRAP_FLAGS);
		}
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);

		if(e.Button != MouseButtons.Left) { return; }

		if(_PreviousBounds.Contains(e.Location)) { PreviousClicked?.Invoke(this, EventArgs.Empty); }
		else if(_NextBounds.Contains(e.Location)) { NextClicked?.Invoke(this, EventArgs.Empty); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _Run                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a text run of the signature line.</summary>
	/// <param name="Text">Text.</param>
	/// <param name="Font">Font.</param>
	/// <param name="Color">Color.</param>
	/// <param name="Location">Location in client coordinates.</param>
	private readonly record struct _Run(string Text, Font Font, Color Color, Point Location);
}
