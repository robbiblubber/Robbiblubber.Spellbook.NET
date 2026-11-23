namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

using Slot = Robbiblubber.Spellbook.Controls.PasswordStrengthColors._Slot;



/// <summary>This control displays the strength of a password as a row of boxes and can evaluate the text of a source control.</summary>
/// <remarks><para>The indicator has six boxes, one for each strength from <see cref="PasswordStrength.VERY_WEAK"/> to
///          <see cref="PasswordStrength.VERY_STRONG"/>; the filled boxes take the color of the strength (a traffic light from red to
///          green by default). <see cref="PasswordStrength.NONE"/> fills no box.</para>
///          <para>If a <see cref="Source"/> is set, its text is rated whenever it changes: the built-in heuristic (<see cref="Rate"/>)
///          provides a strength, which handlers of the <see cref="Rating"/> event can replace.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(PasswordStrengthIndicator), "Resources.passwordstrength.bmp")]
[DefaultProperty(nameof(Value))][DefaultEvent(nameof(ValueChanged))]
public class PasswordStrengthIndicator: Control, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of boxes.</summary>
	private const int _BOX_COUNT = 6;

	/// <summary>Default spacing between the boxes in logical units.</summary>
	private const int _DEFAULT_BOX_SPACING = 4;

	/// <summary>Default corner radius in logical units.</summary>
	private const int _DEFAULT_CORNER_RADIUS = 3;

	/// <summary>Spacing between the boxes and the text in logical units.</summary>
	private const int _TEXT_SPACING = 8;

	/// <summary>Text format flags.</summary>
	private const TextFormatFlags _TEXT_FLAGS = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly PasswordStrengthColors _Colors = new();

	/// <summary>Value.</summary>
	private PasswordStrength _Value = PasswordStrength.NONE;

	/// <summary>Spacing between the boxes in logical units.</summary>
	private int _BoxSpacing = _DEFAULT_BOX_SPACING;

	/// <summary>Determines if the boxes have rounded corners.</summary>
	private bool _UseRoundedCorners = true;

	/// <summary>Corner radius in logical units.</summary>
	private int _CornerRadius = _DEFAULT_CORNER_RADIUS;

	/// <summary>Determines if the strength is displayed as text.</summary>
	private bool _ShowText = false;

	/// <summary>Texts of the strengths, from <see cref="PasswordStrength.NONE"/> to <see cref="PasswordStrength.VERY_STRONG"/>.</summary>
	private readonly string[] _Texts = [string.Empty, "Very weak", "Weak", "Fair", "Good", "Strong", "Very strong"];

	/// <summary>Source control.</summary>
	private Control? _Source = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public PasswordStrengthIndicator(): base()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
		SetStyle(ControlStyles.Selectable, false);

		_Colors.Changed += _Colors_Changed;
		TabStop = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Rates a password with the built-in heuristic.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns the estimated strength; <see cref="PasswordStrength.NONE"/> for an empty password.</returns>
	/// <remarks><para>The heuristic estimates the entropy of the password from its length and character classes. Repetitions,
	///          sequences, keyboard patterns, repeated blocks and years count only a quarter; common passwords (also with
	///          substitutions like <c>P@ssw0rd</c> or appended digits like <c>Sommer2024!</c>) are very weak.</para>
	///          <para>The heuristic is a guide for the user, not a security check. Use the <see cref="Rating"/> event to apply a password
	///          policy or another rating.</para></remarks>
	public static PasswordStrength Rate(string? password)
	{
		return _PasswordRater._Rate(password);
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


	/// <summary>Gets the text index of a strength.</summary>
	/// <param name="strength">Strength.</param>
	/// <returns>Returns the index in the text array.</returns>
	private static int _TextIndex(PasswordStrength strength)
	{
		return (Math.Clamp((int) strength, -1, 5) + 1);
	}


	/// <summary>Sets the text of a strength.</summary>
	/// <param name="strength">Strength.</param>
	/// <param name="value">Text.</param>
	private void _SetText(PasswordStrength strength, string? value)
	{
		_Texts[_TextIndex(strength)] = (value ?? string.Empty);
		Invalidate();
	}


	/// <summary>Attaches the source control.</summary>
	/// <param name="control">Control.</param>
	private void _AttachSource(Control? control)
	{
		if(control is null) { return; }

		control.TextChanged += _Source_TextChanged;
		control.Disposed += _Source_Disposed;
	}


	/// <summary>Detaches the source control.</summary>
	/// <param name="control">Control.</param>
	private void _DetachSource(Control? control)
	{
		if(control is null) { return; }

		control.TextChanged -= _Source_TextChanged;
		control.Disposed -= _Source_Disposed;
	}


	/// <summary>Gets the width of the text column.</summary>
	/// <param name="g">Graphics.</param>
	/// <returns>Returns the width including the spacing, or 0 if no text is displayed.</returns>
	private int _GetTextWidth(Graphics g)
	{
		if(!_ShowText) { return 0; }

		int rval = 0;
		foreach(string i in _Texts) { rval = Math.Max(rval, TextRenderer.MeasureText(g, i, Font, new Size(int.MaxValue, int.MaxValue), _TEXT_FLAGS).Width); }

		return ((rval == 0) ? 0 : (rval + LogicalToDeviceUnits(_TEXT_SPACING)));
	}


	/// <summary>Creates the path of a rectangle with rounded corners.</summary>
	/// <param name="rect">Rectangle.</param>
	/// <param name="radius">Radius.</param>
	/// <returns>Returns the path.</returns>
	private static GraphicsPath _CreateRoundedRectanglePath(Rectangle rect, int radius)
	{
		GraphicsPath rval = new();

		if(radius <= 0)
		{
			rval.AddRectangle(rect);
			return rval;
		}

		int diameter = radius * 2;
		Rectangle arc = new(rect.Location, new Size(diameter, diameter));

		rval.AddArc(arc, 180, 90);
		arc.X = rect.Right - diameter;
		rval.AddArc(arc, 270, 90);
		arc.Y = rect.Bottom - diameter;
		rval.AddArc(arc, 0, 90);
		arc.X = rect.Left;
		rval.AddArc(arc, 90, 90);
		rval.CloseFigure();

		return rval;
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


	/// <summary>Raises the <see cref="ValueEvaluated"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnValueEvaluated(EventArgs e)
	{
		ValueEvaluated?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Rating"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnRating(PasswordRatingEventArgs e)
	{
		Rating?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="ColorRequested"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnColorRequested(PasswordColorEventArgs e)
	{
		ColorRequested?.Invoke(this, e);
	}


	/// <summary>Gets the fill color of the filled boxes.</summary>
	/// <returns>Returns the color of the strength from the color table, possibly changed by <see cref="ColorRequested"/>.</returns>
	protected virtual Color _GetFillColor()
	{
		PasswordColorEventArgs e = new(_Value, _Colors._GetFill(_Value));

		_OnColorRequested(e);
		return e.Color;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the bounds of the boxes (for tests).</summary>
	/// <param name="width">Width of the box area.</param>
	/// <param name="spacing">Spacing between the boxes.</param>
	/// <returns>Returns the left position and width of every box.</returns>
	/// <remarks>The remaining pixels are distributed over the boxes, so the last box ends at the right edge of the area.</remarks>
	internal static (int Left, int Width)[] _GetBoxes(int width, int spacing)
	{
		(int Left, int Width)[] rval = new (int, int)[_BOX_COUNT];

		for(int i = 0; i < _BOX_COUNT; i++)
		{
			int left = (int) Math.Round((double) i * (width + spacing) / _BOX_COUNT);
			int right = (int) Math.Round((double) (i + 1) * (width + spacing) / _BOX_COUNT) - spacing;

			rval[i] = (left, Math.Max(0, right - left));
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the <see cref="Value"/> has changed.</summary>
	[Category("Property Changed")][Description("Occurs when the value has changed.")]
	public event EventHandler? ValueChanged;

	/// <summary>Occurs when the text of the source control has been evaluated, also if the value has not changed.</summary>
	[Category("Action")][Description("Occurs when the text of the source control has been evaluated.")]
	public event EventHandler? ValueEvaluated;

	/// <summary>Occurs when a password is rated; event handlers can replace the strength determined by the built-in heuristic.</summary>
	[Category("Action")][Description("Occurs when a password is rated; event handlers can replace the strength.")]
	public event EventHandler<PasswordRatingEventArgs>? Rating;

	/// <summary>Occurs when the fill color of the boxes is requested; event handlers can replace the color.</summary>
	[Category("Appearance")][Description("Occurs when the fill color of the boxes is requested.")]
	public event EventHandler<PasswordColorEventArgs>? ColorRequested;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the password strength.</summary>
	[Category("Behavior")][Description("The password strength.")][DefaultValue(PasswordStrength.NONE)]
	public PasswordStrength Value
	{
		get { return _Value; }
		set
		{
			value = (PasswordStrength) Math.Clamp((int) value, (int) PasswordStrength.NONE, (int) PasswordStrength.VERY_STRONG);
			if(value == _Value) { return; }

			_Value = value;
			Invalidate();
			AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);

			_OnValueChanged(EventArgs.Empty);
		}
	}


	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the indicator: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public PasswordStrengthColors Colors
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


	/// <summary>Gets or sets the spacing between the boxes in logical units (pixels at 96 DPI).</summary>
	[Category("Layout")][Description("The spacing between the boxes.")][DefaultValue(_DEFAULT_BOX_SPACING)]
	public int BoxSpacing
	{
		get { return _BoxSpacing; }
		set
		{
			value = Math.Max(0, value);
			if(value == _BoxSpacing) { return; }

			_BoxSpacing = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets if the boxes have rounded corners.</summary>
	[Category("Appearance")][Description("Determines if the boxes have rounded corners.")][DefaultValue(true)]
	public bool UseRoundedCorners
	{
		get { return _UseRoundedCorners; }
		set
		{
			if(value == _UseRoundedCorners) { return; }

			_UseRoundedCorners = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the corner radius in logical units (pixels at 96 DPI).</summary>
	[Category("Appearance")][Description("The corner radius of the boxes.")][DefaultValue(_DEFAULT_CORNER_RADIUS)]
	public int CornerRadius
	{
		get { return _CornerRadius; }
		set
		{
			value = Math.Max(0, value);
			if(value == _CornerRadius) { return; }

			_CornerRadius = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets if the strength is displayed as text to the right of the boxes.</summary>
	[Category("Appearance")][Description("Determines if the strength is displayed as text to the right of the boxes.")][DefaultValue(false)]
	public bool ShowText
	{
		get { return _ShowText; }
		set
		{
			if(value == _ShowText) { return; }

			_ShowText = value;
			Invalidate();
		}
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.NONE"/>.</summary>
	[Category("Texts")][Description("The text displayed when no password has been entered.")][DefaultValue("")]
	public string NoneText
	{
		get { return _Texts[0]; }
		set { _SetText(PasswordStrength.NONE, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.VERY_WEAK"/>.</summary>
	[Category("Texts")][Description("The text displayed for very weak passwords.")][DefaultValue("Very weak")]
	public string VeryWeakText
	{
		get { return _Texts[1]; }
		set { _SetText(PasswordStrength.VERY_WEAK, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.WEAK"/>.</summary>
	[Category("Texts")][Description("The text displayed for weak passwords.")][DefaultValue("Weak")]
	public string WeakText
	{
		get { return _Texts[2]; }
		set { _SetText(PasswordStrength.WEAK, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.FAIR"/>.</summary>
	[Category("Texts")][Description("The text displayed for fair passwords.")][DefaultValue("Fair")]
	public string FairText
	{
		get { return _Texts[3]; }
		set { _SetText(PasswordStrength.FAIR, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.GOOD"/>.</summary>
	[Category("Texts")][Description("The text displayed for good passwords.")][DefaultValue("Good")]
	public string GoodText
	{
		get { return _Texts[4]; }
		set { _SetText(PasswordStrength.GOOD, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.STRONG"/>.</summary>
	[Category("Texts")][Description("The text displayed for strong passwords.")][DefaultValue("Strong")]
	public string StrongText
	{
		get { return _Texts[5]; }
		set { _SetText(PasswordStrength.STRONG, value); }
	}


	/// <summary>Gets or sets the text for <see cref="PasswordStrength.VERY_STRONG"/>.</summary>
	[Category("Texts")][Description("The text displayed for very strong passwords.")][DefaultValue("Very strong")]
	public string VeryStrongText
	{
		get { return _Texts[6]; }
		set { _SetText(PasswordStrength.VERY_STRONG, value); }
	}


	/// <summary>Gets the text of the current strength.</summary>
	[Browsable(false)]
	public string StrengthText
	{
		get { return _Texts[_TextIndex(_Value)]; }
	}


	/// <summary>Gets or sets the source control whose text is rated automatically.</summary>
	[Category("Behavior")][Description("The control whose text is rated automatically.")][DefaultValue(null)]
	public Control? Source
	{
		get { return _Source; }
		set
		{
			if(ReferenceEquals(_Source, value)) { return; }

			_DetachSource(_Source);
			_Source = value;
			_AttachSource(_Source);

			Evaluate(_Source?.Text);
		}
	}


	/// <summary>Gets or sets the fill color of all strengths.</summary>
	/// <remarks>This property exists for compatibility: setting it sets the fill colors of all strengths in the color table; getting it
	///          returns the fill color of the current strength.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color FilledColor
	{
		get { return _Colors._GetFill(_Value); }
		set { _Colors._SetAllFills(value); }
	}


	/// <summary>Gets or sets the fill color of empty boxes.</summary>
	/// <remarks>This property gets and sets <see cref="PasswordStrengthColors.EmptyColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color EmptyColor
	{
		get { return _Color(Slot.EMPTY_COLOR); }
		set { _Colors.EmptyColor = value; }
	}


	/// <summary>Gets or sets the border color of the boxes.</summary>
	/// <remarks>This property gets and sets <see cref="PasswordStrengthColors.BorderColor"/> of the color table.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color BorderColor
	{
		get { return _Color(Slot.BORDER_COLOR); }
		set { _Colors.BorderColor = value; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Rates a password and sets the <see cref="Value"/>.</summary>
	/// <param name="password">Password.</param>
	/// <remarks>The built-in heuristic (<see cref="Rate"/>) determines the strength; handlers of the <see cref="Rating"/> event can
	///          replace it. An empty password is <see cref="PasswordStrength.NONE"/> without raising <see cref="Rating"/>. Raises
	///          <see cref="ValueEvaluated"/>.</remarks>
	public void Evaluate(string? password)
	{
		PasswordStrength strength = Rate(password);

		if(!string.IsNullOrEmpty(password))
		{
			PasswordRatingEventArgs e = new(password, strength);

			_OnRating(e);
			strength = e.Strength;
		}

		Value = strength;
		_OnValueEvaluated(EventArgs.Empty);
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
	}


	/// <summary>Handles the TextChanged event of the source control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Source_TextChanged(object? sender, EventArgs e)
	{
		Evaluate(_Source?.Text);
	}


	/// <summary>Handles the Disposed event of the source control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Source_Disposed(object? sender, EventArgs e)
	{
		if(!ReferenceEquals(_Source, sender)) { return; }

		_DetachSource(_Source);
		_Source = null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override Size DefaultSize
	{
		get { return new Size(140, 16); }
	}


	/// <inheritdoc/>
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new _AccessibleObject(this);
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	/// <remarks>The background is painted by the base class, so a transparent back color shows the parent.</remarks>
	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		Rectangle area = ClientRectangle;
		bool enabled = Enabled;
		int textWidth = _GetTextWidth(g);
		int boxArea = area.Width - textWidth;

		base.OnPaint(e);
		if((boxArea <= _BOX_COUNT) || (area.Height <= 1)) { return; }

		int filled = (int) _Value + 1;
		int spacing = LogicalToDeviceUnits(_BoxSpacing);
		int radius = (_UseRoundedCorners ? LogicalToDeviceUnits(_CornerRadius) : 0);
		SmoothingMode mode = g.SmoothingMode;

		using SolidBrush emptyBrush = new(_Color(Slot.EMPTY_COLOR));
		using SolidBrush fillBrush = new(enabled ? _GetFillColor() : _Color(Slot.DISABLED_FILL_COLOR));
		using Pen borderPen = new(enabled ? _Color(Slot.BORDER_COLOR) : _Color(Slot.DISABLED_BORDER_COLOR));

		g.SmoothingMode = ((radius > 0) ? SmoothingMode.AntiAlias : SmoothingMode.None);

		(int Left, int Width)[] boxes = _GetBoxes(boxArea, spacing);
		for(int i = 0; i < _BOX_COUNT; i++)
		{
			Rectangle rect = new(area.Left + boxes[i].Left, area.Top, boxes[i].Width - 1, area.Height - 1);
			if((rect.Width <= 0) || (rect.Height <= 0)) { continue; }

			using GraphicsPath path = _CreateRoundedRectanglePath(rect, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
			g.FillPath(((i < filled) ? fillBrush : emptyBrush), path);
			g.DrawPath(borderPen, path);
		}

		g.SmoothingMode = mode;

		if(textWidth > 0)
		{
			Rectangle text = new(area.Left + boxArea + LogicalToDeviceUnits(_TEXT_SPACING), area.Top, textWidth - LogicalToDeviceUnits(_TEXT_SPACING), area.Height);
			TextRenderer.DrawText(g, StrengthText, Font, text, (enabled ? _Color(Slot.TEXT_COLOR) : _Color(Slot.DISABLED_TEXT_COLOR)), _TEXT_FLAGS);
		}
	}


	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_DetachSource(_Source);
			_Source = null;
			_Colors.Changed -= _Colors_Changed;
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _AccessibleObject                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the accessible object of the indicator.</summary>
	private sealed class _AccessibleObject: ControlAccessibleObject
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Owner control.</summary>
		private readonly PasswordStrengthIndicator _Owner;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="owner">Owner control.</param>
		public _AccessibleObject(PasswordStrengthIndicator owner): base(owner)
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] AccessibleObject                                                                                  //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public override AccessibleRole Role
		{
			get { return ((_Owner.AccessibleRole == AccessibleRole.Default) ? AccessibleRole.Indicator : _Owner.AccessibleRole); }
		}


		/// <inheritdoc/>
		public override string? Value
		{
			get
			{
				string text = _Owner.StrengthText;
				PasswordStrength v = _Owner.Value;
				return ((text.Length > 0) ? text : v.ToString());
			}
			set {}
		}


		/// <inheritdoc/>
		public override AccessibleStates State
		{
			get { return (base.State | AccessibleStates.ReadOnly); }
		}
	}
}
