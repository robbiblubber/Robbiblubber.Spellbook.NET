namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the preview of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks>The upper half shows the new color, the lower half the color that the dialog has been opened with. Clicking the lower half
///          raises <see cref="OriginalClicked"/>, so the dialog can go back to the original color.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ColorPreview: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>New color.</summary>
	private Color _NewColor = Color.Black;

	/// <summary>Original color.</summary>
	private Color _OriginalColor = Color.Black;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public _ColorPreview(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable, false);
		TabStop = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the user has clicked the original color.</summary>
	public event EventHandler? OriginalClicked;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the area of the new color.</summary>
	private Rectangle _NewArea
	{
		get { return new Rectangle(1, 1, Width - 2, (Height - 2) / 2); }
	}


	/// <summary>Gets the area of the original color.</summary>
	private Rectangle _OriginalArea
	{
		get
		{
			Rectangle area = _NewArea;
			return new Rectangle(1, area.Bottom, Width - 2, Height - 1 - area.Bottom);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the new color.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color NewColor
	{
		get { return _NewColor; }
		set
		{
			if(_NewColor.ToArgb() == value.ToArgb()) { return; }

			_NewColor = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the original color.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color OriginalColor
	{
		get { return _OriginalColor; }
		set
		{
			_OriginalColor = value;
			Invalidate();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Theme theme = ThemeManager.GetTheme(this);
		Graphics g = e.Graphics;

		_ColorDialogWindow._FillColor(g, _NewArea, _NewColor);
		_ColorDialogWindow._FillColor(g, _OriginalArea, _OriginalColor);
		using(Pen pen = new(theme[ThemeColor.FIELD_BORDER])) { g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1); }
	}


	/// <inheritdoc/>
	protected override void OnMouseClick(MouseEventArgs e)
	{
		base.OnMouseClick(e);
		if((e.Button == MouseButtons.Left) && _OriginalArea.Contains(e.Location)) { OriginalClicked?.Invoke(this, EventArgs.Empty); }
	}
}
