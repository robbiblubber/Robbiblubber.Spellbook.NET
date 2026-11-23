namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ThemedButtonColors._Slot;



/// <summary>This class implements a button that draws itself with the colors of a theme.</summary>
/// <remarks><para>The button behaves like a <see cref="Button"/> (dialog result, default and cancel button, images, mnemonics) and is
///          drawn flat with rounded corners in the colors of its color table (<see cref="Colors"/>), which takes its colors from the
///          theme of the application, the theme applied to its form or its own theme. The default button of a form has a border in the
///          accent color.</para>
///          <para>Because the button draws itself completely, the <see cref="Control.Paint"/> event does not occur.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(Button))]
public class ThemedButton: Button, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ThemedButtonColors _Colors = new();

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;

	/// <summary>Determines if the left mouse button has been pressed on the control.</summary>
	private bool _MousePressed = false;

	/// <summary>Determines if the space key is pressed.</summary>
	private bool _KeyPressed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedButton(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
		_Colors.Changed += _Colors_Changed;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the interaction states for the renderer.</summary>
	internal _PaintStates _States
	{
		get
		{
			_PaintStates rval = _PaintStates.NONE;

			if(_Hot) { rval |= _PaintStates.HOT; }
			if((_MousePressed && _Hot) || _KeyPressed) { rval |= _PaintStates.PRESSED; }
			if(Focused && ShowFocusCues) { rval |= _PaintStates.FOCUSED; }
			if(!ShowKeyboardCues) { rval |= _PaintStates.HIDE_PREFIX; }
			if(IsDefault) { rval |= _PaintStates.DEFAULT; }

			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the button: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedButtonColors Colors
	{
		get { return _Colors; }
	}


	/// <summary>Gets the color table for the theme manager.</summary>
	Themes.ControlColorTable Themes.IThemedControl.ThemeColors
	{
		get { return _Colors; }
	}


	/// <summary>Gets if the theme manager applies the theme to the child controls.</summary>
	bool Themes.IThemedControl.ThemesChildren
	{
		get { return false; }
	}


	/// <summary>Gets or sets the back color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedButtonColors.BackColor"/> of the color table. System colors, which the designer may have
	///          written for the standard control, are ignored, so the color of the theme is used.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color BackColor
	{
		get { return _Colors._Get(Slot.BACK_COLOR); }
		set { _Colors.BackColor = (value.IsSystemColor ? Color.Empty : value); }
	}


	/// <summary>Gets or sets the text color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedButtonColors.TextColor"/> of the color table. System colors, which the designer may have
	///          written for the standard control, are ignored, so the color of the theme is used.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color ForeColor
	{
		get { return _Colors._Get(Slot.TEXT_COLOR); }
		set { _Colors.TextColor = (value.IsSystemColor ? Color.Empty : value); }
	}


	/// <summary>Gets or sets the flat style.</summary>
	/// <remarks>The control always draws itself; this property has no effect and is kept for compatibility with the standard
	///          control.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new FlatStyle FlatStyle
	{
		get { return base.FlatStyle; }
		set {}
	}


	/// <summary>Gets the appearance of the flat style.</summary>
	/// <remarks>The control always draws itself with the colors of <see cref="Colors"/>; this property has no effect.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new FlatButtonAppearance FlatAppearance
	{
		get { return base.FlatAppearance; }
	}


	/// <summary>Gets or sets if the back color of the visual style is used.</summary>
	/// <remarks>The control always draws itself with the colors of <see cref="Colors"/>; this property has no effect.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new bool UseVisualStyleBackColor
	{
		get { return base.UseVisualStyleBackColor; }
		set {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Resets the color table.</summary>
	public void ResetColors()
	{
		_Colors.Reset();
	}


	/// <summary>Determines if the color table has to be serialized by the designer.</summary>
	/// <returns>Returns <c>true</c> if the color table differs from the default, otherwise returns <c>false</c>.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ShouldSerializeColors()
	{
		return (!_Colors.IsDefault);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Changed event of the color table.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Colors_Changed(object? sender, EventArgs e)
	{
		Invalidate();
		OnForeColorChanged(EventArgs.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs e)
	{}


	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		_ControlRenderer._PaintButton(e.Graphics, this, _Colors, (Parent?.BackColor ?? Color.Empty), _States);
	}
	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		_Hot = true;
		Invalidate();
		base.OnMouseEnter(e);
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		_Hot = false;
		Invalidate();
		base.OnMouseLeave(e);
	}


	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			_MousePressed = true;
			Invalidate();
		}
		base.OnMouseDown(e);
	}


	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			_MousePressed = false;
			Invalidate();
		}
		base.OnMouseUp(e);
	}


	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		if((e.KeyCode == Keys.Space) && !_KeyPressed)
		{
			_KeyPressed = true;
			Invalidate();
		}
		base.OnKeyDown(e);
	}


	/// <inheritdoc/>
	protected override void OnKeyUp(KeyEventArgs e)
	{
		if(e.KeyCode == Keys.Space)
		{
			_KeyPressed = false;
			Invalidate();
		}
		base.OnKeyUp(e);
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		_KeyPressed = false;
		_MousePressed = false;
		Invalidate();
		base.OnLostFocus(e);
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		_Hot = _MousePressed = _KeyPressed = false;
		base.OnEnabledChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		Invalidate();
		base.OnGotFocus(e);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _Colors.Changed -= _Colors_Changed; }

		base.Dispose(disposing);
	}
}
