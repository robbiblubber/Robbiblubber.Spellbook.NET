namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the color grids of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks>The grid shows its colors in rows; <see cref="Color.Empty"/> is an empty cell. The user picks a color with the mouse or
///          moves through the grid with the arrow keys, Home and End, which picks the color as well.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ColorSwatches: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal constants                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Logical size of a cell.</summary>
	internal const int _CELL_SIZE = 18;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Colors.</summary>
	private readonly Color[] _Colors;

	/// <summary>Index of the selected cell, -1 if no cell is selected.</summary>
	private int _Selected = -1;

	/// <summary>Index of the cell under the mouse, -1 if there is none.</summary>
	private int _Hot = -1;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="colors">Colors.</param>
	/// <param name="columns">Number of columns.</param>
	public _ColorSwatches(Color[] colors, int columns): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
		TabStop = true;

		_Colors = (Color[]) colors.Clone();
		Columns = columns;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the user has picked a color.</summary>
	public event EventHandler? ColorPicked;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the bounds of a cell.</summary>
	/// <param name="index">Index.</param>
	/// <returns>Returns the bounds.</returns>
	private Rectangle _GetCellBounds(int index)
	{
		int size = CellSize;
		int column = (index % Columns);
		if(RightToLeft == RightToLeft.Yes) { column = (Columns - 1 - column); }

		return new Rectangle(column * size, (index / Columns) * size, size, size);
	}


	/// <summary>Gets the cell at a point.</summary>
	/// <param name="point">Point in client coordinates.</param>
	/// <returns>Returns the index, -1 if there is no cell.</returns>
	private int _GetCellAt(Point point)
	{
		int size = CellSize;
		if((point.X < 0) || (point.Y < 0) || (point.X >= (Columns * size))) { return -1; }

		int column = (point.X / size);
		if(RightToLeft == RightToLeft.Yes) { column = (Columns - 1 - column); }
		int rval = (((point.Y / size) * Columns) + column);

		return ((rval < _Colors.Length) ? rval : -1);
	}


	/// <summary>Picks a cell as the user has chosen it.</summary>
	/// <param name="index">Index.</param>
	private void _Pick(int index)
	{
		if((index < 0) || (index >= _Colors.Length)) { return; }

		SelectedIndex = index;
		if(!_Colors[index].IsEmpty) { ColorPicked?.Invoke(this, EventArgs.Empty); }
	}


	/// <summary>Sets the cell under the mouse.</summary>
	/// <param name="index">Index, -1 for none.</param>
	private void _SetHot(int index)
	{
		if(index == _Hot) { return; }

		if(_Hot >= 0) { Invalidate(_GetCellBounds(_Hot)); }
		_Hot = index;
		if(_Hot >= 0) { Invalidate(_GetCellBounds(_Hot)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of columns.</summary>
	public int Columns
	{
		get;
	}


	/// <summary>Gets the number of cells.</summary>
	public int Count
	{
		get { return _Colors.Length; }
	}


	/// <summary>Gets the size of a cell in pixels.</summary>
	public int CellSize
	{
		get { return LogicalToDeviceUnits(_CELL_SIZE); }
	}


	/// <summary>Gets the size the grid needs for its cells.</summary>
	public Size GridSize
	{
		get { return new Size(Columns * CellSize, ((_Colors.Length + Columns - 1) / Columns) * CellSize); }
	}


	/// <summary>Gets or sets the index of the selected cell, -1 if no cell is selected.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectedIndex
	{
		get { return _Selected; }
		set
		{
			if((value < -1) || (value >= _Colors.Length)) { value = -1; }
			if(value == _Selected) { return; }

			if(_Selected >= 0) { Invalidate(_GetCellBounds(_Selected)); }
			_Selected = value;
			if(_Selected >= 0) { Invalidate(_GetCellBounds(_Selected)); }
		}
	}


	/// <summary>Gets the color of the selected cell, <see cref="Color.Empty"/> if no cell or an empty cell is selected.</summary>
	public Color SelectedColor
	{
		get { return ((_Selected >= 0) ? _Colors[_Selected] : Color.Empty); }
	}


	/// <summary>Gets a copy of the colors.</summary>
	public Color[] Colors
	{
		get { return (Color[]) _Colors.Clone(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the color of a cell.</summary>
	/// <param name="index">Index.</param>
	/// <param name="color">Color.</param>
	public void SetColor(int index, Color color)
	{
		_Colors[index] = color;
		Invalidate(_GetCellBounds(index));
	}


	/// <summary>Finds a color.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the index of the first cell with the color, -1 if there is none.</returns>
	public int IndexOf(Color color)
	{
		return Array.FindIndex(_Colors, m => (!m.IsEmpty && (m.ToArgb() == color.ToArgb())));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Theme theme = ThemeManager.GetTheme(this);
		Graphics g = e.Graphics;
		int gap = LogicalToDeviceUnits(2);

		using(SolidBrush brush = new(BackColor)) { g.FillRectangle(brush, ClientRectangle); }

		using Pen border = new(theme[ThemeColor.FIELD_BORDER]);
		for(int i = 0; i < _Colors.Length; i++)
		{
			Rectangle cell = _GetCellBounds(i);
			if(!e.ClipRectangle.IntersectsWith(cell)) { continue; }

			Rectangle swatch = Rectangle.Inflate(cell, -gap, -gap);
			if(!_Colors[i].IsEmpty) { _ColorDialogWindow._FillColor(g, swatch, _Colors[i]); }
			g.DrawRectangle(border, swatch.Left, swatch.Top, swatch.Width - 1, swatch.Height - 1);

			Color? frame = null;
			if(i == _Selected) { frame = theme[(Focused && ShowFocusCues) ? ThemeColor.ACCENT : ThemeColor.TEXT]; }
			else if(i == _Hot) { frame = theme[ThemeColor.GLYPH]; }

			if(frame is Color c)
			{
				using Pen pen = new(c, (i == _Selected) ? gap : 1);
				Rectangle outer = Rectangle.Inflate(cell, -(gap / 2), -(gap / 2));
				g.DrawRectangle(pen, outer.Left, outer.Top, outer.Width - 1, outer.Height - 1);
			}
		}
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if(e.Button != MouseButtons.Left) { return; }

		Focus();
		_Pick(_GetCellAt(e.Location));
	}


	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		_SetHot(_GetCellAt(e.Location));
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		_SetHot(-1);
	}


	/// <inheritdoc/>
	protected override bool IsInputKey(Keys keyData)
	{
		switch(keyData & Keys.KeyCode)
		{
			case Keys.Left: case Keys.Right: case Keys.Up: case Keys.Down: case Keys.Home: case Keys.End: return true;
		}

		return base.IsInputKey(keyData);
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if(e.Handled) { return; }

		int current = Math.Max(_Selected, 0);
		int forward = ((RightToLeft == RightToLeft.Yes) ? -1 : 1);
		int index;
		switch(e.KeyCode)
		{
			case Keys.Left: index = (current - forward); break;
			case Keys.Right: index = (current + forward); break;
			case Keys.Up: index = (current - Columns); break;
			case Keys.Down: index = (current + Columns); break;
			case Keys.Home: index = 0; break;
			case Keys.End: index = (_Colors.Length - 1); break;
			case Keys.Space: index = current; break;
			default: return;
		}

		if(_Selected < 0) { index = current; }
		_Pick(Math.Clamp(index, 0, _Colors.Length - 1));
		e.Handled = true;
	}


	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		if(_Selected >= 0) { Invalidate(_GetCellBounds(_Selected)); }
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		if(_Selected >= 0) { Invalidate(_GetCellBounds(_Selected)); }
	}
}
