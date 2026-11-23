namespace Robbiblubber.Spellbook.Controls;

using System.Globalization;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the window of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks><para>The window shows the basic colors and the custom colors on the left, the saturation and value area with the hue
///          slider (and the alpha slider) in the middle, and the preview with the number fields and the hexadecimal value on the
///          right. All parts follow each other: changing one changes the others.</para>
///          <para>The window keeps the color as hue, saturation and value, so the hue survives grays and the saturation survives
///          black.</para></remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ColorDialogWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Logical margin.</summary>
	private const int _MARGIN = 12;

	/// <summary>Logical gap between the columns.</summary>
	private const int _COLUMN_GAP = 16;

	/// <summary>Number of columns of the color grids.</summary>
	private const int _COLUMNS = 12;

	/// <summary>Logical height of a label above a part.</summary>
	private const int _LABEL_HEIGHT = 18;

	/// <summary>Logical size of the saturation and value area.</summary>
	private const int _SPECTRUM_SIZE = 220;

	/// <summary>Logical width of a slider.</summary>
	private const int _SLIDER_WIDTH = 22;

	/// <summary>Logical gap before a slider.</summary>
	private const int _SLIDER_GAP = 6;

	/// <summary>Logical height of the preview.</summary>
	private const int _PREVIEW_HEIGHT = 56;

	/// <summary>Logical height of a row of number fields.</summary>
	private const int _ROW_HEIGHT = 28;

	/// <summary>Logical width of the label of a number field.</summary>
	private const int _FIELD_LABEL_WIDTH = 48;

	/// <summary>Logical width of a number field.</summary>
	private const int _FIELD_WIDTH = 56;

	/// <summary>Logical gap between the two columns of number fields.</summary>
	private const int _FIELD_GAP = 8;

	/// <summary>Logical width of a button.</summary>
	private const int _BUTTON_WIDTH = 88;

	/// <summary>Logical height of a button.</summary>
	private const int _BUTTON_HEIGHT = 28;

	/// <summary>Logical gap between buttons.</summary>
	private const int _BUTTON_GAP = 8;

	/// <summary>Logical padding of the button bar.</summary>
	private const int _FOOTER_PADDING = 12;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal constants                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of custom colors.</summary>
	internal const int _CUSTOM_COUNT = 24;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Basic colors.</summary>
	private static readonly Color[] _BasicColors = _CreateBasicColors();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Determines if the color can be transparent.</summary>
	private readonly bool _AllowAlpha;

	/// <summary>Color that the dialog has been opened with.</summary>
	private readonly Color _Original;

	/// <summary>Number fields.</summary>
	private readonly List<NumericUpDown> _Numbers = new();

	/// <summary>Button bar.</summary>
	private readonly Panel _Footer;

	/// <summary>Color.</summary>
	private _HsvColor _Hsv;

	/// <summary>Determines if the controls are being updated.</summary>
	private bool _Updating = false;

	/// <summary>Custom color that "Add to custom colors" sets if no custom color is selected and none is empty.</summary>
	private int _NextCustom = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal members                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Basic colors.</summary>
	internal readonly _ColorSwatches _BasicSwatches;

	/// <summary>Custom colors.</summary>
	internal readonly _ColorSwatches _CustomSwatches;

	/// <summary>Button "Add to custom colors".</summary>
	internal readonly ThemedButton _ButtonAdd;

	/// <summary>Saturation and value area.</summary>
	internal readonly _ColorSpectrum _Spectrum;

	/// <summary>Hue slider.</summary>
	internal readonly _ColorSlider _HueSlider;

	/// <summary>Alpha slider, NULL if the color cannot be transparent.</summary>
	internal readonly _ColorSlider? _AlphaSlider;

	/// <summary>Preview.</summary>
	internal readonly _ColorPreview _Preview;

	/// <summary>Hue field.</summary>
	internal readonly NumericUpDown _NumberHue;

	/// <summary>Saturation field.</summary>
	internal readonly NumericUpDown _NumberSaturation;

	/// <summary>Value field.</summary>
	internal readonly NumericUpDown _NumberValue;

	/// <summary>Red field.</summary>
	internal readonly NumericUpDown _NumberRed;

	/// <summary>Green field.</summary>
	internal readonly NumericUpDown _NumberGreen;

	/// <summary>Blue field.</summary>
	internal readonly NumericUpDown _NumberBlue;

	/// <summary>Alpha field, NULL if the color cannot be transparent.</summary>
	internal readonly NumericUpDown? _NumberAlpha;

	/// <summary>Hexadecimal value.</summary>
	internal readonly TextBox _TextHex;

	/// <summary>OK button.</summary>
	internal readonly ThemedButton _ButtonOk;

	/// <summary>Cancel button.</summary>
	internal readonly ThemedButton _ButtonCancel;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="color">Color.</param>
	/// <param name="customColors">Custom colors, <see cref="Color.Empty"/> for empty cells.</param>
	/// <param name="allowAlpha">Determines if the color can be transparent.</param>
	/// <param name="title">Title, NULL for the default title.</param>
	/// <param name="theme">Theme, NULL for the theme of the application.</param>
	/// <param name="hasOwner">Determines if the window has an owner.</param>
	internal _ColorDialogWindow(Color color, Color[] customColors, bool allowAlpha, string? title, Theme? theme, bool hasOwner)
	{
		_AllowAlpha = allowAlpha;
		_Original = (allowAlpha ? color : Color.FromArgb(255, color));
		_Hsv = _HsvColor.FromColor(_Original);

		SuspendLayout();
		AutoScaleMode = AutoScaleMode.None;
		Font = (SystemFonts.MessageBoxFont ?? Control.DefaultFont);
		Text = (title ?? ThemedColorDialogTexts.Title);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MinimizeBox = MaximizeBox = false;
		ShowIcon = false;
		ShowInTaskbar = !hasOwner;
		StartPosition = (hasOwner ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen);

		float scale = (DeviceDpi / 96f);
		int s(int value) => (int) Math.Round(value * scale);
		int tab = 0;

		Label addLabel(string text, Rectangle bounds, ContentAlignment align)
		{
			Label rval = new() { AutoSize = false, AutoEllipsis = true, Text = text, Bounds = bounds, TextAlign = align, TabIndex = tab++ };
			Controls.Add(rval);
			return rval;
		}

		T add<T>(T control, Rectangle bounds) where T: Control
		{
			control.Bounds = bounds;
			control.TabIndex = tab++;
			Controls.Add(control);
			return control;
		}

		int x = s(_MARGIN), y = s(_MARGIN);

		_BasicSwatches = new _ColorSwatches(_BasicColors, _COLUMNS);
		Size grid = _BasicSwatches.GridSize;
		addLabel(ThemedColorDialogTexts.BasicColors, new Rectangle(x, y, grid.Width, s(_LABEL_HEIGHT)), ContentAlignment.MiddleLeft);
		y += s(_LABEL_HEIGHT);
		add(_BasicSwatches, new Rectangle(new Point(x, y), grid));
		y += (grid.Height + s(_MARGIN));

		Color[] custom = new Color[_CUSTOM_COUNT];
		Array.Copy(customColors, custom, Math.Min(customColors.Length, _CUSTOM_COUNT));
		_CustomSwatches = new _ColorSwatches(custom, _COLUMNS);
		addLabel(ThemedColorDialogTexts.CustomColors, new Rectangle(x, y, grid.Width, s(_LABEL_HEIGHT)), ContentAlignment.MiddleLeft);
		y += s(_LABEL_HEIGHT);
		add(_CustomSwatches, new Rectangle(new Point(x, y), _CustomSwatches.GridSize));
		y += (_CustomSwatches.GridSize.Height + s(_BUTTON_GAP));

		_ButtonAdd = add(new ThemedButton() { Text = ThemedColorDialogTexts.AddCustomColor }, new Rectangle(x, y, grid.Width, s(_BUTTON_HEIGHT)));
		int bottom = (y + s(_BUTTON_HEIGHT));

		x += (grid.Width + s(_COLUMN_GAP));
		y = (s(_MARGIN) + s(_LABEL_HEIGHT));
		_Spectrum = add(new _ColorSpectrum(), new Rectangle(x, y, s(_SPECTRUM_SIZE), s(_SPECTRUM_SIZE)));
		x += (s(_SPECTRUM_SIZE) + s(_SLIDER_GAP));
		_HueSlider = add(new _ColorSlider(false), new Rectangle(x, y - s(4), s(_SLIDER_WIDTH), s(_SPECTRUM_SIZE) + s(8)));
		x += s(_SLIDER_WIDTH);
		if(allowAlpha)
		{
			x += s(_SLIDER_GAP);
			_AlphaSlider = add(new _ColorSlider(true), new Rectangle(x, y - s(4), s(_SLIDER_WIDTH), s(_SPECTRUM_SIZE) + s(8)));
			x += s(_SLIDER_WIDTH);
		}
		bottom = Math.Max(bottom, y + s(_SPECTRUM_SIZE));

		x += s(_COLUMN_GAP);
		int column = (s(_FIELD_LABEL_WIDTH) + s(_FIELD_WIDTH));
		int right = (x + column + s(_FIELD_GAP));
		_Preview = add(new _ColorPreview(), new Rectangle(x, y, (2 * column) + s(_FIELD_GAP), s(_PREVIEW_HEIGHT)));
		y += (s(_PREVIEW_HEIGHT) + s(_MARGIN));

		NumericUpDown addNumber(string text, int maximum, int left, int top)
		{
			addLabel(text, new Rectangle(left, top, s(_FIELD_LABEL_WIDTH), s(_ROW_HEIGHT) - s(4)), ContentAlignment.MiddleLeft);
			NumericUpDown rval = add(new NumericUpDown() { Minimum = 0, Maximum = maximum }, new Rectangle(left + s(_FIELD_LABEL_WIDTH), top, s(_FIELD_WIDTH), s(_ROW_HEIGHT) - s(4)));
			rval.TextChanged += _Number_TextChanged;
			_Numbers.Add(rval);
			return rval;
		}

		_NumberHue = addNumber(ThemedColorDialogTexts.Hue, 360, x, y);
		_NumberSaturation = addNumber(ThemedColorDialogTexts.Saturation, 100, x, y + s(_ROW_HEIGHT));
		_NumberValue = addNumber(ThemedColorDialogTexts.Value, 100, x, y + (2 * s(_ROW_HEIGHT)));
		_NumberRed = addNumber(ThemedColorDialogTexts.Red, 255, right, y);
		_NumberGreen = addNumber(ThemedColorDialogTexts.Green, 255, right, y + s(_ROW_HEIGHT));
		_NumberBlue = addNumber(ThemedColorDialogTexts.Blue, 255, right, y + (2 * s(_ROW_HEIGHT)));
		y += (3 * s(_ROW_HEIGHT));
		if(allowAlpha)
		{
			_NumberAlpha = addNumber(ThemedColorDialogTexts.Alpha, 255, right, y);
			y += s(_ROW_HEIGHT);
		}

		addLabel(ThemedColorDialogTexts.Hex, new Rectangle(x, y, s(_FIELD_LABEL_WIDTH), s(_ROW_HEIGHT) - s(4)), ContentAlignment.MiddleLeft);
		_TextHex = add(new TextBox() { MaxLength = 32 }, new Rectangle(x + s(_FIELD_LABEL_WIDTH), y, column + s(_FIELD_GAP) + s(_FIELD_WIDTH), s(_ROW_HEIGHT) - s(4)));
		y += s(_ROW_HEIGHT);
		bottom = Math.Max(bottom, y);

		int width = (x + (2 * column) + s(_FIELD_GAP) + s(_MARGIN));
		int footerHeight = (s(_BUTTON_HEIGHT) + (2 * s(_FOOTER_PADDING)));
		ClientSize = new Size(width, bottom + s(_MARGIN) + footerHeight);

		_Footer = new Panel() { Dock = DockStyle.Bottom, Height = footerHeight, TabIndex = tab++ };
		_Footer.Paint += _Footer_Paint;
		Size buttonSize = new(s(_BUTTON_WIDTH), s(_BUTTON_HEIGHT));
		int buttonX = (width - s(_FOOTER_PADDING) - (2 * buttonSize.Width) - s(_BUTTON_GAP));
		_ButtonOk = new ThemedButton() { Text = ThemedColorDialogTexts.Ok, Bounds = new Rectangle(new Point(buttonX, s(_FOOTER_PADDING)), buttonSize), TabIndex = 0 };
		_ButtonCancel = new ThemedButton() { Text = ThemedColorDialogTexts.Cancel, DialogResult = DialogResult.Cancel, Bounds = new Rectangle(new Point(buttonX + buttonSize.Width + s(_BUTTON_GAP), s(_FOOTER_PADDING)), buttonSize), TabIndex = 1 };
		_Footer.Controls.Add(_ButtonOk);
		_Footer.Controls.Add(_ButtonCancel);
		Controls.Add(_Footer);
		AcceptButton = _ButtonOk;
		CancelButton = _ButtonCancel;

		_Preview.OriginalColor = _Original;
		_Update(null);

		_BasicSwatches.ColorPicked += _Swatches_ColorPicked;
		_CustomSwatches.ColorPicked += _Swatches_ColorPicked;
		_ButtonAdd.Click += _ButtonAdd_Click;
		_Spectrum.ValueChanged += _Spectrum_ValueChanged;
		_HueSlider.ValueChanged += _HueSlider_ValueChanged;
		if(_AlphaSlider is not null) { _AlphaSlider.ValueChanged += _AlphaSlider_ValueChanged; }
		_Preview.OriginalClicked += _Preview_OriginalClicked;
		_TextHex.TextChanged += _TextHex_TextChanged;
		_TextHex.Leave += _TextHex_Leave;
		_ButtonOk.Click += _ButtonOk_Click;

		ActiveControl = _Spectrum;
		ResumeLayout(false);
		ThemeManager.Apply(this, theme);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the basic colors.</summary>
	/// <returns>Returns twelve hues in six shades and twelve grays.</returns>
	private static Color[] _CreateBasicColors()
	{
		(double Saturation, double Value)[] shades = { (0.25d, 1d), (0.5d, 1d), (1d, 1d), (1d, 0.75d), (1d, 0.5d), (1d, 0.25d) };
		List<Color> rval = new();

		foreach((double saturation, double value) in shades)
		{
			for(int i = 0; i < _COLUMNS; i++) { rval.Add(new _HsvColor(i * (360d / _COLUMNS), saturation, value).ToColor()); }
		}
		for(int i = 0; i < _COLUMNS; i++) { rval.Add(new _HsvColor(0d, 0d, 1d - (i / (double) (_COLUMNS - 1))).ToColor()); }

		return rval.ToArray();
	}


	/// <summary>Gets the value of a number field from its text.</summary>
	/// <param name="number">Number field.</param>
	/// <returns>Returns the value, NULL if the text is not a valid value.</returns>
	private static int? _GetTypedValue(NumericUpDown number)
	{
		if(!int.TryParse(number.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int rval)) { return null; }
		if((rval < number.Minimum) || (rval > number.Maximum)) { return null; }

		return rval;
	}


	/// <summary>Sets the value of a number field.</summary>
	/// <param name="number">Number field.</param>
	/// <param name="value">Value.</param>
	private static void _SetNumber(NumericUpDown number, double value)
	{
		number.Value = Math.Clamp((decimal) Math.Round(value), number.Minimum, number.Maximum);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a color to its hexadecimal value.</summary>
	/// <param name="color">Color.</param>
	/// <param name="allowAlpha">Determines if the alpha is written for transparent colors.</param>
	/// <returns>Returns the text, for example <c>#2B2B2B</c> or <c>#802B2B2B</c>.</returns>
	internal static string _ToText(Color color, bool allowAlpha)
	{
		return ((allowAlpha && (color.A < 255)) ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}" : $"#{color.R:X2}{color.G:X2}{color.B:X2}");
	}


	/// <summary>Converts a text to a color.</summary>
	/// <param name="text">Text: a hexadecimal value with or without #, with three, six or (with alpha) eight digits, or the name of a
	///                    color like <c>teal</c>.</param>
	/// <param name="allowAlpha">Determines if the color can be transparent.</param>
	/// <param name="color">Returns the color.</param>
	/// <returns>Returns <c>true</c> if the text is a color, otherwise <c>false</c>.</returns>
	internal static bool _TryParseColor(string? text, bool allowAlpha, out Color color)
	{
		color = Color.Empty;
		text = text?.Trim();
		if(string.IsNullOrEmpty(text)) { return false; }

		string digits = (text.StartsWith('#') ? text.Substring(1) : text);
		if((digits.Length is 3 or 6 or 8) && digits.All(Uri.IsHexDigit))
		{
			if((digits.Length == 8) && !allowAlpha) { return false; }
			if(digits.Length == 3) { digits = string.Concat(digits.Select(m => new string(m, 2))); }

			uint value = uint.Parse(digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			if(digits.Length == 6) { value |= 0xFF000000; }

			color = Color.FromArgb(unchecked((int) value));
			return true;
		}

		if(text.StartsWith('#')) { return false; }

		Color named = Color.FromName(text);
		if(!named.IsKnownColor || named.IsSystemColor) { return false; }
		if(!allowAlpha && (named.A < 255)) { return false; }

		color = Color.FromArgb(named.ToArgb());
		return true;
	}


	/// <summary>Fills an area with a color; transparent colors are drawn on a checkerboard.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="bounds">Area.</param>
	/// <param name="color">Color.</param>
	internal static void _FillColor(Graphics g, Rectangle bounds, Color color)
	{
		if(color.A < 255) { _DrawChecker(g, bounds); }
		using SolidBrush brush = new(color);
		g.FillRectangle(brush, bounds);
	}


	/// <summary>Draws a checkerboard that shows transparency.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="bounds">Area.</param>
	internal static void _DrawChecker(Graphics g, Rectangle bounds)
	{
		int size = Math.Max(3, (int) Math.Round(4 * g.DpiX / 96f));

		g.FillRectangle(Brushes.White, bounds);
		Region clip = g.Clip;
		g.SetClip(bounds, System.Drawing.Drawing2D.CombineMode.Intersect);
		using(SolidBrush brush = new(Color.FromArgb(204, 204, 204)))
		{
			for(int y = 0; (bounds.Top + (y * size)) < bounds.Bottom; y++)
			{
				for(int x = (y % 2); (bounds.Left + (x * size)) < bounds.Right; x += 2) { g.FillRectangle(brush, bounds.Left + (x * size), bounds.Top + (y * size), size, size); }
			}
		}
		g.Clip = clip;
		clip.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the color.</summary>
	/// <param name="color">Color.</param>
	/// <param name="source">Control that the color comes from, NULL for none.</param>
	private void _SetColor(Color color, object? source)
	{
		if(!_AllowAlpha) { color = Color.FromArgb(255, color); }

		_Hsv = _HsvColor.FromColor(color, _Hsv);
		_Update(source);
	}


	/// <summary>Shows the color in all controls except the one that it comes from.</summary>
	/// <param name="source">Control that the color comes from, NULL for none.</param>
	private void _Update(object? source)
	{
		_Updating = true;
		try
		{
			Color color = _Color;

			_Spectrum.Hue = _Hsv.Hue;
			_Spectrum.Saturation = _Hsv.Saturation;
			_Spectrum.Value = _Hsv.Value;
			_HueSlider.Value = _Hsv.Hue;
			if(_AlphaSlider is not null)
			{
				_AlphaSlider.Value = _Hsv.Alpha;
				_AlphaSlider.Color = color;
			}
			_Preview.NewColor = color;

			if(source != _NumberHue) { _SetNumber(_NumberHue, _Hsv.Hue); }
			if(source != _NumberSaturation) { _SetNumber(_NumberSaturation, _Hsv.Saturation * 100d); }
			if(source != _NumberValue) { _SetNumber(_NumberValue, _Hsv.Value * 100d); }
			if(source != _NumberRed) { _SetNumber(_NumberRed, color.R); }
			if(source != _NumberGreen) { _SetNumber(_NumberGreen, color.G); }
			if(source != _NumberBlue) { _SetNumber(_NumberBlue, color.B); }
			if((_NumberAlpha is not null) && (source != _NumberAlpha)) { _SetNumber(_NumberAlpha, color.A); }
			if(source != _TextHex) { _TextHex.Text = _ToText(color, _AllowAlpha); }

			if(source != _BasicSwatches) { _BasicSwatches.SelectedIndex = _BasicSwatches.IndexOf(color); }
		}
		finally { _Updating = false; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds the color to the custom colors: sets the selected custom color, otherwise the first empty one, otherwise the
	///          next one.</summary>
	internal void _AddCustomColor()
	{
		int index = _CustomSwatches.SelectedIndex;
		if(index < 0) { index = Array.FindIndex(_CustomSwatches.Colors, m => m.IsEmpty); }
		if(index < 0) { index = _NextCustom; }

		_CustomSwatches.SetColor(index, _Color);
		_CustomSwatches.SelectedIndex = index;
		_NextCustom = ((index + 1) % _CustomSwatches.Count);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color.</summary>
	internal Color _Color
	{
		get
		{
			Color rval = _Hsv.ToColor();
			return (_AllowAlpha ? rval : Color.FromArgb(255, rval));
		}
	}


	/// <summary>Gets the custom colors.</summary>
	internal Color[] _CustomColors
	{
		get { return _CustomSwatches.Colors; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ColorPicked event of the color grids.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Swatches_ColorPicked(object? sender, EventArgs e)
	{
		if(sender is not _ColorSwatches swatches) { return; }

		if(swatches == _BasicSwatches) { _CustomSwatches.SelectedIndex = -1; }
		_SetColor(swatches.SelectedColor, swatches);
	}


	/// <summary>Handles the Click event of the button "Add to custom colors".</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonAdd_Click(object? sender, EventArgs e)
	{
		_AddCustomColor();
	}


	/// <summary>Handles the ValueChanged event of the saturation and value area.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Spectrum_ValueChanged(object? sender, EventArgs e)
	{
		if(_Updating) { return; }

		_Hsv = new _HsvColor(_Hsv.Hue, _Spectrum.Saturation, _Spectrum.Value, _Hsv.Alpha);
		_Update(_Spectrum);
	}


	/// <summary>Handles the ValueChanged event of the hue slider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _HueSlider_ValueChanged(object? sender, EventArgs e)
	{
		if(_Updating) { return; }

		_Hsv = new _HsvColor(_HueSlider.Value, _Hsv.Saturation, _Hsv.Value, _Hsv.Alpha);
		_Update(_HueSlider);
	}


	/// <summary>Handles the ValueChanged event of the alpha slider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AlphaSlider_ValueChanged(object? sender, EventArgs e)
	{
		if(_Updating || (_AlphaSlider is null)) { return; }

		_Hsv = new _HsvColor(_Hsv.Hue, _Hsv.Saturation, _Hsv.Value, (int) Math.Round(_AlphaSlider.Value));
		_Update(_AlphaSlider);
	}


	/// <summary>Handles the TextChanged event of the number fields: typed values take effect at once.</summary>
	/// <remarks>The text also changes when the value changes, but only after the ValueChanged event.</remarks>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Number_TextChanged(object? sender, EventArgs e)
	{
		if(_Updating || (sender is not NumericUpDown number) || (_GetTypedValue(number) is not int value)) { return; }

		Color color = _Color;
		if(number == _NumberHue) { _Hsv = new _HsvColor(value, _Hsv.Saturation, _Hsv.Value, _Hsv.Alpha); }
		else if(number == _NumberSaturation) { _Hsv = new _HsvColor(_Hsv.Hue, value / 100d, _Hsv.Value, _Hsv.Alpha); }
		else if(number == _NumberValue) { _Hsv = new _HsvColor(_Hsv.Hue, _Hsv.Saturation, value / 100d, _Hsv.Alpha); }
		else if(number == _NumberRed) { _Hsv = _HsvColor.FromColor(Color.FromArgb(color.A, value, color.G, color.B), _Hsv); }
		else if(number == _NumberGreen) { _Hsv = _HsvColor.FromColor(Color.FromArgb(color.A, color.R, value, color.B), _Hsv); }
		else if(number == _NumberBlue) { _Hsv = _HsvColor.FromColor(Color.FromArgb(color.A, color.R, color.G, value), _Hsv); }
		else if(number == _NumberAlpha) { _Hsv = new _HsvColor(_Hsv.Hue, _Hsv.Saturation, _Hsv.Value, value); }

		_Update(number);
	}


	/// <summary>Handles the TextChanged event of the hexadecimal value: a valid value takes effect at once.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextHex_TextChanged(object? sender, EventArgs e)
	{
		if(_Updating || !_TryParseColor(_TextHex.Text, _AllowAlpha, out Color color)) { return; }

		_SetColor(color, _TextHex);
	}


	/// <summary>Handles the Leave event of the hexadecimal value: shows the value of the color.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextHex_Leave(object? sender, EventArgs e)
	{
		_Updating = true;
		_TextHex.Text = _ToText(_Color, _AllowAlpha);
		_Updating = false;
	}


	/// <summary>Handles the OriginalClicked event of the preview: goes back to the original color.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Preview_OriginalClicked(object? sender, EventArgs e)
	{
		_SetColor(_Original, null);
	}


	/// <summary>Handles the Click event of the OK button: takes over typed values and closes the window.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonOk_Click(object? sender, EventArgs e)
	{
		foreach(NumericUpDown i in _Numbers) { _ = i.Value; }

		DialogResult = DialogResult.OK;
	}


	/// <summary>Handles the Paint event of the button bar: draws a separator line at the top.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Footer_Paint(object? sender, PaintEventArgs e)
	{
		using Pen pen = new(ThemeManager.GetTheme(this)[ThemeColor.SEPARATOR]);
		e.Graphics.DrawLine(pen, 0, 0, _Footer.Width, 0);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _Footer.Paint -= _Footer_Paint; }

		base.Dispose(disposing);
	}
}
