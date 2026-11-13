namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the window that shows the description of the selected item of an item list.</summary>
/// <remarks>The window is placed next to the auto completion list, on the right side if there is enough space, otherwise on the left.</remarks>
[SupportedOSPlatform("windows")]
internal class _ListDescriptionWindow: _NoActivateWindowBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text padding in logical units.</summary>
	private const int _PADDING = 6;

	/// <summary>Gap between list and description in logical units.</summary>
	private const int _GAP = 2;

	/// <summary>Text format flags.</summary>
	private const TextFormatFlags _TEXT_FLAGS = TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Description text.</summary>
	private string _Description = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public _ListDescriptionWindow(): base()
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Shows a description next to the auto completion list.</summary>
	/// <param name="listBounds">Screen bounds of the auto completion list.</param>
	/// <param name="description">Description text.</param>
	/// <param name="font">Font.</param>
	/// <param name="backColor">Back color.</param>
	/// <param name="foreColor">Fore color.</param>
	/// <param name="maximumWidth">Maximum window width.</param>
	/// <param name="owner">Owner form.</param>
	public void ShowFor(Rectangle listBounds, string description, Font font, Color backColor, Color foreColor, int maximumWidth, Form? owner)
	{
		Rectangle area = Screen.FromRectangle(listBounds).WorkingArea;
		int padding = LogicalToDeviceUnits(_PADDING);
		int gap = LogicalToDeviceUnits(_GAP);
		Size text, size;
		int x, y;

		_Description = description;
		Font = font;
		BackColor = backColor;
		ForeColor = foreColor;

		text = TextRenderer.MeasureText(description, font, new Size(Math.Max(1, maximumWidth - (padding * 2)), int.MaxValue), _TEXT_FLAGS);
		size = SizeFromClientSize(new Size(text.Width + (padding * 2), text.Height + (padding * 2)));
		size.Height = Math.Min(size.Height, area.Height / 2);

		x = listBounds.Right + gap;
		if((x + size.Width) > area.Right) { x = listBounds.Left - gap - size.Width; }
		x = Math.Max(area.Left, x);

		y = listBounds.Top;
		if((y + size.Height) > area.Bottom) { y = area.Bottom - size.Height; }

		ShowNoActivate(new Rectangle(x, y, size.Width, size.Height), owner);
		Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		int padding = LogicalToDeviceUnits(_PADDING);
		Rectangle r = new(padding, padding, ClientSize.Width - (padding * 2), ClientSize.Height - (padding * 2));

		e.Graphics.Clear(BackColor);
		TextRenderer.DrawText(e.Graphics, _Description, Font, r, ForeColor, _TEXT_FLAGS | TextFormatFlags.EndEllipsis);
	}
}
