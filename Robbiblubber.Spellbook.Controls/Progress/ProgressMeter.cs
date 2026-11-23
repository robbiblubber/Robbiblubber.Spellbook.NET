namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Versioning;

using Slot = Robbiblubber.Spellbook.Controls.ProgressMeterColors._Slot;



/// <summary>This class implements a flat, self-drawn progress meter with customizable colors, optional text and a marquee style.</summary>
/// <remarks>Unlike <see cref="ProgressBar"/>, the meter is drawn with the colors of its color table (<see cref="Colors"/>), which
///          takes its colors from the theme of the application or its own theme. Values outside the range are limited to
///          <see cref="Minimum"/> and <see cref="Maximum"/>.</remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(ProgressMeter), "Resources.progressmeter.bmp")]
[DefaultEvent(nameof(ValueChanged))][DefaultProperty(nameof(Value))]
public class ProgressMeter: Control, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Interval of the marquee animation in milliseconds.</summary>
	private const int _MARQUEE_INTERVAL = 30;

	/// <summary>Length of the marquee segment relative to the meter length.</summary>
	private const float _MARQUEE_LENGTH = 0.25f;

	/// <summary>Default duration of a marquee cycle in milliseconds.</summary>
	private const int _DEFAULT_MARQUEE_DURATION = 2000;

	/// <summary>Text format flags.</summary>
	private const TextFormatFlags _TEXT_FLAGS = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ProgressMeterColors _Colors = new();

	/// <summary>Minimum value.</summary>
	private int _Minimum = 0;

	/// <summary>Maximum value.</summary>
	private int _Maximum = 100;

	/// <summary>Current value.</summary>
	private int _Value = 0;

	/// <summary>Step for <see cref="PerformStep"/>.</summary>
	private int _Step = 10;

	/// <summary>Border style.</summary>
	private BorderStyle _BorderStyle = BorderStyle.FixedSingle;

	/// <summary>Orientation.</summary>
	private Orientation _Orientation = Orientation.Horizontal;

	/// <summary>Style.</summary>
	private ProgressMeterStyle _Style = ProgressMeterStyle.CONTINUOUS;

	/// <summary>Text style.</summary>
	private ProgressMeterTextStyle _TextStyle = ProgressMeterTextStyle.NONE;

	/// <summary>Duration of a marquee cycle in milliseconds.</summary>
	private int _MarqueeDuration = _DEFAULT_MARQUEE_DURATION;

	/// <summary>Position of the marquee segment, from 0 to 1.</summary>
	private float _MarqueePhase = 0f;

	/// <summary>Timer of the marquee animation, NULL if it has not been created yet.</summary>
	private System.Windows.Forms.Timer? _MarqueeTimer = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ProgressMeter(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable, false);

		_Colors.Changed += _Colors_Changed;
		TabStop = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the marquee animation should run.</summary>
	private bool _MarqueeActive
	{
		get { return ((_Style == ProgressMeterStyle.MARQUEE) && Visible && Enabled && IsHandleCreated && (!IsDisposed)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a color of the color table.</summary>
	/// <param name="slot">Color slot.</param>
	/// <returns>Returns the color.</returns>
	private Color _Color(Slot slot)
	{
		return _Colors._Get(slot);
	}


	/// <summary>Sets the value, limited to the range, and raises <see cref="ValueChanged"/> if it has changed.</summary>
	/// <param name="value">Value.</param>
	private void _SetValue(int value)
	{
		value = Math.Clamp(value, _Minimum, _Maximum);
		if(value == _Value) { return; }

		_Value = value;
		Invalidate();
		AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

		_OnValueChanged(EventArgs.Empty);
	}


	/// <summary>Starts or stops the marquee animation.</summary>
	private void _UpdateMarquee()
	{
		if(_MarqueeActive)
		{
			if(_MarqueeTimer is null)
			{
				_MarqueeTimer = new System.Windows.Forms.Timer { Interval = _MARQUEE_INTERVAL };
				_MarqueeTimer.Tick += _MarqueeTimer_Tick;
			}
			_MarqueeTimer.Start();
		}
		else { _MarqueeTimer?.Stop(); }
	}


	/// <summary>Gets the area inside the border.</summary>
	/// <returns>Returns the area.</returns>
	private Rectangle _GetInnerBounds()
	{
		Rectangle rval = ClientRectangle;

		if(_BorderStyle != BorderStyle.None) { rval.Inflate(-1, -1); }
		return rval;
	}


	/// <summary>Gets the bounds of the bar.</summary>
	/// <param name="inner">Area inside the border.</param>
	/// <returns>Returns the bounds, empty if no bar is displayed.</returns>
	private Rectangle _GetBarBounds(Rectangle inner)
	{
		bool vertical = (_Orientation == Orientation.Vertical);
		int length = (vertical ? inner.Height : inner.Width);
		int start, size;

		if(_Style == ProgressMeterStyle.MARQUEE)
		{
			int segment = Math.Max(1, (int) (length * _MARQUEE_LENGTH));

			start = (int) (((length + segment) * _MarqueePhase) - segment);
			size = segment;

			if(start < 0)
			{
				size += start;
				start = 0;
			}
			size = Math.Min(size, length - start);
		}
		else
		{
			start = 0;
			size = (int) Math.Round(length * Fraction);
		}

		if(size <= 0) { return Rectangle.Empty; }

		return (vertical ? new Rectangle(inner.Left, inner.Bottom - start - size, inner.Width, size) : new Rectangle(inner.Left + start, inner.Top, size, inner.Height));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="ValueChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnValueChanged(EventArgs e)
	{
		ValueChanged?.Invoke(this, e);
	}


	/// <summary>Gets the displayed text.</summary>
	/// <returns>Returns the text for the <see cref="TextStyle"/>, or an empty string if no text is displayed.</returns>
	/// <remarks>Derived classes can override this method to display their own text.</remarks>
	protected virtual string _GetDisplayText()
	{
		if((_Style == ProgressMeterStyle.MARQUEE) && (_TextStyle != ProgressMeterTextStyle.TEXT)) { return string.Empty; }

		return _TextStyle switch
		{
			ProgressMeterTextStyle.PERCENT => Fraction.ToString("P0", CultureInfo.CurrentCulture),
			ProgressMeterTextStyle.VALUE => string.Format(CultureInfo.CurrentCulture, "{0:N0} / {1:N0}", _Value, _Maximum),
			ProgressMeterTextStyle.TEXT => Text,
			_ => string.Empty
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the marquee timer is running.</summary>
	internal bool _MarqueeRunning
	{
		get { return (_MarqueeTimer?.Enabled ?? false); }
	}


	/// <summary>Gets the displayed text (for tests).</summary>
	internal string _DisplayText
	{
		get { return _GetDisplayText(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the value has changed.</summary>
	[Category("Behavior")][Description("Occurs when the value has changed.")]
	public event EventHandler? ValueChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the meter: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ProgressMeterColors Colors
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


	/// <summary>Gets or sets the minimum value.</summary>
	/// <remarks>If the minimum is greater than the maximum, the maximum is raised to the minimum. The value is limited to the new range.</remarks>
	[Category("Behavior")][Description("The minimum value.")][DefaultValue(0)][RefreshProperties(RefreshProperties.Repaint)]
	public int Minimum
	{
		get { return _Minimum; }
		set
		{
			if(value == _Minimum) { return; }

			_Minimum = value;
			if(_Maximum < value) { _Maximum = value; }

			Invalidate();
			_SetValue(_Value);
		}
	}


	/// <summary>Gets or sets the maximum value.</summary>
	/// <remarks>If the maximum is less than the minimum, the minimum is lowered to the maximum. The value is limited to the new range.</remarks>
	[Category("Behavior")][Description("The maximum value.")][DefaultValue(100)][RefreshProperties(RefreshProperties.Repaint)]
	public int Maximum
	{
		get { return _Maximum; }
		set
		{
			if(value == _Maximum) { return; }

			_Maximum = value;
			if(_Minimum > value) { _Minimum = value; }

			Invalidate();
			_SetValue(_Value);
		}
	}


	/// <summary>Gets or sets the value.</summary>
	/// <remarks>Values outside the range are limited to <see cref="Minimum"/> and <see cref="Maximum"/>.</remarks>
	[Category("Behavior")][Description("The current value.")][DefaultValue(0)][Bindable(true)]
	public int Value
	{
		get { return _Value; }
		set { _SetValue(value); }
	}


	/// <summary>Gets the progress as a fraction between 0 and 1.</summary>
	[Browsable(false)]
	public double Fraction
	{
		get { return ((_Maximum > _Minimum) ? ((double) (_Value - _Minimum) / (_Maximum - _Minimum)) : 0d); }
	}


	/// <summary>Gets or sets the amount by which <see cref="PerformStep"/> increases the value.</summary>
	[Category("Behavior")][Description("The amount by which PerformStep increases the value.")][DefaultValue(10)]
	public int Step
	{
		get { return _Step; }
		set { _Step = value; }
	}


	/// <summary>Gets or sets the border style.</summary>
	/// <remarks><see cref="BorderStyle.Fixed3D"/> is drawn like <see cref="BorderStyle.FixedSingle"/>.</remarks>
	[Category("Appearance")][Description("The border style.")][DefaultValue(BorderStyle.FixedSingle)]
	public BorderStyle BorderStyle
	{
		get { return _BorderStyle; }
		set
		{
			if(value == _BorderStyle) { return; }

			_BorderStyle = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the orientation.</summary>
	/// <remarks>A horizontal meter fills from left to right, a vertical meter from bottom to top.</remarks>
	[Category("Appearance")][Description("The orientation.")][DefaultValue(Orientation.Horizontal)]
	public Orientation Orientation
	{
		get { return _Orientation; }
		set
		{
			if(value == _Orientation) { return; }

			_Orientation = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the style.</summary>
	[Category("Behavior")][Description("Determines if the meter shows the value or a moving segment for progress of unknown duration.")]
	[DefaultValue(ProgressMeterStyle.CONTINUOUS)]
	public ProgressMeterStyle Style
	{
		get { return _Style; }
		set
		{
			if(value == _Style) { return; }

			_Style = value;
			_MarqueePhase = 0f;
			_UpdateMarquee();
			Invalidate();
		}
	}


	/// <summary>Gets or sets the displayed text.</summary>
	/// <remarks>In marquee style, only <see cref="ProgressMeterTextStyle.TEXT"/> is displayed.</remarks>
	[Category("Appearance")][Description("The text displayed in the meter.")][DefaultValue(ProgressMeterTextStyle.NONE)]
	public ProgressMeterTextStyle TextStyle
	{
		get { return _TextStyle; }
		set
		{
			if(value == _TextStyle) { return; }

			_TextStyle = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the duration of a marquee cycle in milliseconds.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 100.</exception>
	[Category("Behavior")][Description("The duration of a marquee cycle in milliseconds.")][DefaultValue(_DEFAULT_MARQUEE_DURATION)]
	public int MarqueeDuration
	{
		get { return _MarqueeDuration; }
		set
		{
			if(value < 100) { throw new ArgumentOutOfRangeException(nameof(value)); }
			_MarqueeDuration = value;
		}
	}


	/// <summary>Gets or sets the text, displayed with <see cref="ProgressMeterTextStyle.TEXT"/>.</summary>
	[Browsable(true)][EditorBrowsable(EditorBrowsableState.Always)][DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Category("Appearance")][Description("The text displayed with the TEXT text style.")]
	[AllowNull]
	public override string Text
	{
		get { return base.Text; }
		set { base.Text = value; }
	}


	/// <summary>Gets or sets the back color.</summary>
	/// <remarks>This property gets and sets <see cref="ProgressMeterColors.BackColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color BackColor
	{
		get { return _Color(Slot.BACK_COLOR); }
		set { _Colors.BackColor = value; }
	}


	/// <summary>Gets or sets the bar color.</summary>
	/// <remarks>This property gets and sets <see cref="ProgressMeterColors.BarColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color ForeColor
	{
		get { return _Color(Slot.BAR_COLOR); }
		set { _Colors.BarColor = value; }
	}


	/// <summary>Gets or sets the border color.</summary>
	/// <remarks>This property gets and sets <see cref="ProgressMeterColors.BorderColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color BorderColor
	{
		get { return _Color(Slot.BORDER_COLOR); }
		set { _Colors.BorderColor = value; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Increases the value by <see cref="Step"/>.</summary>
	public void PerformStep()
	{
		Increment(_Step);
	}


	/// <summary>Increases the value.</summary>
	/// <param name="amount">Amount, negative to decrease the value.</param>
	public void Increment(int amount)
	{
		_SetValue((int) Math.Clamp((long) _Value + amount, int.MinValue, int.MaxValue));
	}


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
		OnBackColorChanged(EventArgs.Empty);
		OnForeColorChanged(EventArgs.Empty);
	}


	/// <summary>Handles the Tick event of the marquee timer.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MarqueeTimer_Tick(object? sender, EventArgs e)
	{
		_MarqueePhase += (float) _MARQUEE_INTERVAL / _MarqueeDuration;
		if(_MarqueePhase > 1f) { _MarqueePhase -= 1f; }

		Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Size DefaultSize
	{
		get { return new Size(100, 16); }
	}


	/// <inheritdoc/>
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new _AccessibleObject(this);
	}


	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		_UpdateMarquee();
	}


	/// <inheritdoc/>
	protected override void OnHandleDestroyed(EventArgs e)
	{
		_MarqueeTimer?.Stop();
		base.OnHandleDestroyed(e);
	}


	/// <inheritdoc/>
	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		_UpdateMarquee();
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);

		_UpdateMarquee();
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		if(_TextStyle == ProgressMeterTextStyle.TEXT) { Invalidate(); }
	}


	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs e)
	{}


	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		bool enabled = Enabled;
		Rectangle inner = _GetInnerBounds();
		Rectangle bar = _GetBarBounds(inner);
		string text = _GetDisplayText();

		using(SolidBrush brush = new(enabled ? _Color(Slot.BACK_COLOR) : _Color(Slot.DISABLED_BACK_COLOR))) { g.FillRectangle(brush, ClientRectangle); }

		if(!bar.IsEmpty)
		{
			using SolidBrush brush = new(enabled ? _Color(Slot.BAR_COLOR) : _Color(Slot.DISABLED_BAR_COLOR));
			g.FillRectangle(brush, bar);
		}

		if(text.Length > 0)
		{
			Color textColor = (enabled ? _Color(Slot.TEXT_COLOR) : _Color(Slot.DISABLED_TEXT_COLOR));

			TextRenderer.DrawText(g, text, Font, inner, textColor, _TEXT_FLAGS);

			if(enabled && (!bar.IsEmpty))
			{
				GraphicsState state = g.Save();

				g.SetClip(bar, CombineMode.Intersect);
				TextRenderer.DrawText(g, text, Font, inner, _Color(Slot.BAR_TEXT_COLOR), _TEXT_FLAGS);
				g.Restore(state);
			}
		}

		if(_BorderStyle != BorderStyle.None)
		{
			using Pen pen = new(enabled ? _Color(Slot.BORDER_COLOR) : _Color(Slot.DISABLED_BORDER_COLOR));
			g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
		}

		base.OnPaint(e);
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_MarqueeTimer is not null)
			{
				_MarqueeTimer.Tick -= _MarqueeTimer_Tick;
				_MarqueeTimer.Dispose();
				_MarqueeTimer = null;
			}

			_Colors.Changed -= _Colors_Changed;
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _AccessibleObject                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the accessible object of the meter.</summary>
	private sealed class _AccessibleObject: ControlAccessibleObject
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Owner control.</summary>
		private readonly ProgressMeter _Owner;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="owner">Owner control.</param>
		public _AccessibleObject(ProgressMeter owner): base(owner)
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] AccessibleObject                                                                                  //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public override AccessibleRole Role
		{
			get { return ((_Owner.AccessibleRole == AccessibleRole.Default) ? AccessibleRole.ProgressBar : _Owner.AccessibleRole); }
		}


		/// <inheritdoc/>
		public override string? Value
		{
			get { return ((_Owner._Style == ProgressMeterStyle.MARQUEE) ? string.Empty : _Owner.Fraction.ToString("P0", CultureInfo.CurrentCulture)); }
			set {}
		}


		/// <inheritdoc/>
		public override AccessibleStates State
		{
			get { return (base.State | AccessibleStates.ReadOnly); }
		}
	}
}
