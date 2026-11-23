namespace Robbiblubber.Spellbook.Controls;

using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the saturation and value area of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks>The area shows the colors of one hue: the saturation grows from left to right, the value from bottom to top. The user picks
///          a color with the mouse or with the arrow keys (Shift for steps of ten percent).</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ColorSpectrum: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hue in degrees.</summary>
	private double _Hue = 0d;

	/// <summary>Saturation, 0 to 1.</summary>
	private double _Saturation = 1d;

	/// <summary>Value, 0 to 1.</summary>
	private double _Value = 1d;

	/// <summary>Determines if the left mouse button has been pressed on the control.</summary>
	private bool _Dragging = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public _ColorSpectrum(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
		TabStop = true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the user has changed the saturation or the value.</summary>
	public event EventHandler? ValueChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the area that shows the colors.</summary>
	private Rectangle _Area
	{
		get { return Rectangle.Inflate(ClientRectangle, -1, -1); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the hue in degrees.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public double Hue
	{
		get { return _Hue; }
		set
		{
			if(_Hue == value) { return; }

			_Hue = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the saturation, 0 to 1.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public double Saturation
	{
		get { return _Saturation; }
		set
		{
			value = Math.Clamp(value, 0d, 1d);
			if(_Saturation == value) { return; }

			_Saturation = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the value, 0 to 1.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public double Value
	{
		get { return _Value; }
		set
		{
			value = Math.Clamp(value, 0d, 1d);
			if(_Value == value) { return; }

			_Value = value;
			Invalidate();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets saturation and value as the user has chosen them.</summary>
	/// <param name="saturation">Saturation.</param>
	/// <param name="value">Value.</param>
	private void _SetByUser(double saturation, double value)
	{
		saturation = Math.Clamp(saturation, 0d, 1d);
		value = Math.Clamp(value, 0d, 1d);
		if((saturation == _Saturation) && (value == _Value)) { return; }

		_Saturation = saturation;
		_Value = value;
		Invalidate();
		ValueChanged?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Sets saturation and value from a point.</summary>
	/// <param name="point">Point in client coordinates.</param>
	private void _SetFromPoint(Point point)
	{
		Rectangle area = _Area;
		if((area.Width <= 1) || (area.Height <= 1)) { return; }

		_SetByUser((point.X - area.Left) / (double) (area.Width - 1), 1d - ((point.Y - area.Top) / (double) (area.Height - 1)));
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

		if((area.Width > 0) && (area.Height > 0))
		{
			Color hue = new _HsvColor(_Hue, 1d, 1d).ToColor();
			using(LinearGradientBrush brush = new(new Rectangle(area.Left - 1, area.Top, area.Width + 1, area.Height), Color.White, hue, LinearGradientMode.Horizontal))
			{
				g.FillRectangle(brush, area);
			}
			using(LinearGradientBrush brush = new(new Rectangle(area.Left, area.Top - 1, area.Width, area.Height + 1), Color.FromArgb(0, Color.Black), Color.Black, LinearGradientMode.Vertical))
			{
				g.FillRectangle(brush, area);
			}

			int x = (area.Left + (int) Math.Round(_Saturation * (area.Width - 1)));
			int y = (area.Top + (int) Math.Round((1d - _Value) * (area.Height - 1)));
			int r = LogicalToDeviceUnits(5);

			GraphicsState state = g.Save();
			g.SetClip(area);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			using(Pen pen = new(Color.Black, LogicalToDeviceUnits(3))) { g.DrawEllipse(pen, x - r, y - r, 2 * r, 2 * r); }
			using(Pen pen = new(Color.White, LogicalToDeviceUnits(1))) { g.DrawEllipse(pen, x - r, y - r, 2 * r, 2 * r); }
			g.Restore(state);
		}

		Color border = theme[(Focused && ShowFocusCues) ? ThemeColor.FIELD_BORDER_FOCUSED : ThemeColor.FIELD_BORDER];
		using(Pen pen = new(border)) { g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1); }
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
			case Keys.Left: case Keys.Right: case Keys.Up: case Keys.Down: return true;
		}

		return base.IsInputKey(keyData);
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if(e.Handled) { return; }

		double step = (e.Shift ? 0.1d : 0.01d);
		switch(e.KeyCode)
		{
			case Keys.Left: _SetByUser(_Saturation - step, _Value); break;
			case Keys.Right: _SetByUser(_Saturation + step, _Value); break;
			case Keys.Up: _SetByUser(_Saturation, _Value + step); break;
			case Keys.Down: _SetByUser(_Saturation, _Value - step); break;
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
