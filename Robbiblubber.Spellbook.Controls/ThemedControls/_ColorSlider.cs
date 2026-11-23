namespace Robbiblubber.Spellbook.Controls;

using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the vertical hue and alpha sliders of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks>The hue slider shows the hues from 0 at the top to 360 at the bottom; the alpha slider shows its color from opaque at
///          the top to transparent at the bottom. The user moves the slider with the mouse or with the arrow keys (Shift and
///          Page Up/Down for steps of ten).</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ColorSlider: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Value.</summary>
	private double _Value = 0d;

	/// <summary>Color of the alpha slider.</summary>
	private Color _Color = Color.Black;

	/// <summary>Determines if the left mouse button has been pressed on the control.</summary>
	private bool _Dragging = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="alpha">Determines if the slider is an alpha slider, otherwise a hue slider.</param>
	public _ColorSlider(bool alpha): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
		TabStop = true;
		IsAlpha = alpha;
		_Value = (alpha ? 255d : 0d);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the user has changed the value.</summary>
	public event EventHandler? ValueChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the value as the user has chosen it.</summary>
	/// <param name="value">Value.</param>
	private void _SetByUser(double value)
	{
		value = Math.Clamp(value, 0d, Maximum);
		if(value == _Value) { return; }

		_Value = value;
		Invalidate();
		ValueChanged?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Sets the value from a point.</summary>
	/// <param name="point">Point in client coordinates.</param>
	private void _SetFromPoint(Point point)
	{
		Rectangle area = _Area;
		if(area.Height <= 1) { return; }

		double position = Math.Clamp((point.Y - area.Top) / (double) (area.Height - 1), 0d, 1d);
		_SetByUser((IsAlpha ? (1d - position) : position) * Maximum);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the area that shows the colors.</summary>
	private Rectangle _Area
	{
		get
		{
			int inset = LogicalToDeviceUnits(4);
			return new Rectangle(inset + 1, inset + 1, Width - (2 * inset) - 2, Height - (2 * inset) - 2);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the slider is an alpha slider.</summary>
	public bool IsAlpha
	{
		get;
	}


	/// <summary>Gets the maximum value: 360 for the hue, 255 for the alpha.</summary>
	public double Maximum
	{
		get { return (IsAlpha ? 255d : 360d); }
	}


	/// <summary>Gets or sets the value.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public double Value
	{
		get { return _Value; }
		set
		{
			value = Math.Clamp(value, 0d, Maximum);
			if(_Value == value) { return; }

			_Value = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the color of the alpha slider.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color Color
	{
		get { return _Color; }
		set
		{
			if(_Color.ToArgb() == value.ToArgb()) { return; }

			_Color = value;
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
		Rectangle area = _Area;

		using(SolidBrush brush = new(BackColor)) { g.FillRectangle(brush, ClientRectangle); }
		if((area.Width <= 0) || (area.Height <= 0)) { return; }

		Rectangle gradient = new(area.Left, area.Top - 1, area.Width, area.Height + 1);
		if(IsAlpha)
		{
			_ColorDialogWindow._DrawChecker(g, area);
			Color opaque = Color.FromArgb(255, _Color);
			using LinearGradientBrush brush = new(gradient, opaque, Color.FromArgb(0, opaque), LinearGradientMode.Vertical);
			g.FillRectangle(brush, area);
		}
		else
		{
			using LinearGradientBrush brush = new(gradient, Color.Red, Color.Red, LinearGradientMode.Vertical);
			ColorBlend blend = new(7);
			for(int i = 0; i < 7; i++)
			{
				blend.Colors[i] = new _HsvColor(i * 60d, 1d, 1d).ToColor();
				blend.Positions[i] = (i / 6f);
			}
			brush.InterpolationColors = blend;
			g.FillRectangle(brush, area);
		}

		using(Pen pen = new(theme[ThemeColor.FIELD_BORDER])) { g.DrawRectangle(pen, area.Left - 1, area.Top - 1, area.Width + 1, area.Height + 1); }

		double position = (_Value / Maximum);
		if(IsAlpha) { position = (1d - position); }
		int y = (area.Top + (int) Math.Round(position * (area.Height - 1)));
		int h = LogicalToDeviceUnits(3);

		Color marker = theme[(Focused && ShowFocusCues) ? ThemeColor.ACCENT : ThemeColor.TEXT];
		using(Pen pen = new(marker, LogicalToDeviceUnits(2)))
		{
			g.DrawRectangle(pen, 1, y - h, Width - 3, 2 * h);
		}
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if(e.Button != MouseButtons.Left) { return; }

		Focus();
		_Dragging = true;
		_SetFromPoint(e.Location);
	}


	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if(_Dragging) { _SetFromPoint(e.Location); }
	}


	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		if(e.Button == MouseButtons.Left) { _Dragging = false; }
	}


	/// <inheritdoc/>
	protected override void OnMouseCaptureChanged(EventArgs e)
	{
		_Dragging = false;
		base.OnMouseCaptureChanged(e);
	}


	/// <inheritdoc/>
	protected override bool IsInputKey(Keys keyData)
	{
		switch(keyData & Keys.KeyCode)
		{
			case Keys.Up: case Keys.Down: case Keys.PageUp: case Keys.PageDown: case Keys.Home: case Keys.End: return true;
		}

		return base.IsInputKey(keyData);
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if(e.Handled) { return; }

		double step = (e.Shift ? 10d : 1d);
		double down = (IsAlpha ? -1d : 1d);
		switch(e.KeyCode)
		{
			case Keys.Up: _SetByUser(Math.Round(_Value) - (down * step)); break;
			case Keys.Down: _SetByUser(Math.Round(_Value) + (down * step)); break;
			case Keys.PageUp: _SetByUser(Math.Round(_Value) - (down * 10d)); break;
			case Keys.PageDown: _SetByUser(Math.Round(_Value) + (down * 10d)); break;
			case Keys.Home: _SetByUser(IsAlpha ? Maximum : 0d); break;
			case Keys.End: _SetByUser(IsAlpha ? 0d : Maximum); break;
			default: return;
		}
		e.Handled = true;
	}


	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		Invalidate();
	}
}
