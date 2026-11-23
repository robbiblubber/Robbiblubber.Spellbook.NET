namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ThemedToggleColors._Slot;



/// <summary>This class implements a check box that draws itself with the colors of a theme.</summary>
/// <remarks><para>The control behaves like a <see cref="CheckBox"/> and draws its box in the colors of its color table
///          (<see cref="Colors"/>), which takes its colors from the theme of the application, the theme applied to its form or its own
///          theme. With <see cref="Appearance.Button"/>, it is drawn as a button that has the accent color when it is checked. With
///          <see cref="Style"/> set to <see cref="ThemedCheckBoxStyle.SWITCH"/>, it is drawn as a switch like in the Windows
///          settings.</para>
///          <para>Because the control draws itself completely, the <see cref="Control.Paint"/> event does not occur.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(CheckBox))]
public class ThemedCheckBox: CheckBox, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Duration of the movement of the switch knob in milliseconds.</summary>
	private const double _SWITCH_DURATION = 120d;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ThemedToggleColors _Colors = new();

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;

	/// <summary>Determines if the left mouse button has been pressed on the control.</summary>
	private bool _MousePressed = false;

	/// <summary>Determines if the space key is pressed.</summary>
	private bool _KeyPressed = false;

	/// <summary>Style.</summary>
	private ThemedCheckBoxStyle _Style = ThemedCheckBoxStyle.BOX;

	/// <summary>Current position of the switch knob, from 0 (off) to 1 (on).</summary>
	private float _SwitchPosition = 0f;

	/// <summary>Timer that moves the switch knob.</summary>
	private System.Windows.Forms.Timer? _SwitchTimer = null;

	/// <summary>Time when the switch knob has started to move.</summary>
	private DateTime _SwitchStart;

	/// <summary>Position of the switch knob when it has started to move.</summary>
	private float _SwitchFrom = 0f;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedCheckBox(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
		_Colors.Changed += _Colors_Changed;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the position of the switch knob for a check state.</summary>
	/// <param name="state">Check state.</param>
	/// <returns>Returns 0 for unchecked, 1 for checked and 0.5 for indeterminate.</returns>
	private static float _GetSwitchTarget(CheckState state)
	{
		return state switch
		{
			CheckState.Checked => 1f,
			CheckState.Indeterminate => 0.5f,
			_ => 0f
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the control is drawn as switch.</summary>
	private bool _IsSwitch
	{
		get { return ((_Style == ThemedCheckBoxStyle.SWITCH) && (Appearance == Appearance.Normal)); }
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

			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the check box: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedToggleColors Colors
	{
		get { return _Colors; }
	}


	/// <summary>Gets or sets how the check box shows its check state: as box or as switch.</summary>
	/// <remarks>The style has an effect only with <see cref="Appearance.Normal"/>.</remarks>
	[Category("Appearance")][Description("Determines if the check box is drawn as box or as switch.")][DefaultValue(ThemedCheckBoxStyle.BOX)]
	public ThemedCheckBoxStyle Style
	{
		get { return _Style; }
		set
		{
			if(value == _Style) { return; }

			_Style = value;
			_StopSwitch();
			if(AutoSize) { Size = PreferredSize; }
			Parent?.PerformLayout(this, nameof(Style));
			Invalidate();
		}
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


	/// <summary>Gets or sets the text color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedToggleColors.TextColor"/> of the color table. System colors, which the designer may have
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
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Stops the movement of the switch knob and puts it to the position of the check state.</summary>
	private void _StopSwitch()
	{
		_SwitchTimer?.Stop();
		_SwitchPosition = _GetSwitchTarget(CheckState);
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


	/// <summary>Handles the Tick event of the switch timer: moves the knob.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _SwitchTimer_Tick(object? sender, EventArgs e)
	{
		float target = _GetSwitchTarget(CheckState);
		float progress = (float) Math.Min(1d, (DateTime.UtcNow - _SwitchStart).TotalMilliseconds / _SWITCH_DURATION);

		_SwitchPosition = (_SwitchFrom + ((target - _SwitchFrom) * progress));
		if(progress >= 1f) { _StopSwitch(); }
		Invalidate();
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
		_ControlRenderer._PaintToggle(e.Graphics, this, _Colors, CheckState, false, Appearance, CheckAlign, _States, (_IsSwitch ? _SwitchPosition : null));
	}


	/// <inheritdoc/>
	protected override void OnCheckStateChanged(EventArgs e)
	{
		if(_IsSwitch && IsHandleCreated && Visible && SystemInformation.UIEffectsEnabled)
		{
			_SwitchFrom = _SwitchPosition;
			_SwitchStart = DateTime.UtcNow;
			if(_SwitchTimer is null)
			{
				_SwitchTimer = new() { Interval = 15 };
				_SwitchTimer.Tick += _SwitchTimer_Tick;
			}
			_SwitchTimer.Start();
		}
		else { _StopSwitch(); }

		base.OnCheckStateChanged(e);
	}


	/// <inheritdoc/>
	public override Size GetPreferredSize(Size proposedSize)
	{
		Size rval = base.GetPreferredSize(proposedSize);
		if(!_IsSwitch) { return rval; }

		Size box = new(_ControlRenderer._GetGlyphSize(this), _ControlRenderer._GetGlyphSize(this));
		Size track = _ControlRenderer._GetSwitchSize(this);
		return new Size(rval.Width + (track.Width - box.Width), Math.Max(rval.Height, track.Height + Padding.Vertical + 2));
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
		if(disposing)
		{
			_Colors.Changed -= _Colors_Changed;
			_SwitchTimer?.Dispose();
			_SwitchTimer = null;
		}

		base.Dispose(disposing);
	}
}
