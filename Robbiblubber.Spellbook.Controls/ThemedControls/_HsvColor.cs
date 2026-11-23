namespace Robbiblubber.Spellbook.Controls;



/// <summary>This structure holds a color as hue, saturation, value and alpha.</summary>
/// <remarks>The color dialog keeps its color in this form, so the hue survives grays and the saturation survives black.</remarks>
internal readonly struct _HsvColor
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this structure.</summary>
	/// <param name="hue">Hue in degrees, 0 to 360; 0 and 360 are both red.</param>
	/// <param name="saturation">Saturation, 0 to 1.</param>
	/// <param name="value">Value, 0 to 1.</param>
	/// <param name="alpha">Alpha, 0 to 255.</param>
	public _HsvColor(double hue, double saturation, double value, int alpha = 255)
	{
		Hue = Math.Clamp(hue, 0d, 360d);
		Saturation = Math.Clamp(saturation, 0d, 1d);
		Value = Math.Clamp(value, 0d, 1d);
		Alpha = Math.Clamp(alpha, 0, 255);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a color.</summary>
	/// <param name="color">Color.</param>
	/// <param name="reference">Color that gives the hue for grays and the saturation for black.</param>
	/// <returns>Returns the color.</returns>
	public static _HsvColor FromColor(Color color, _HsvColor reference = default)
	{
		double r = (color.R / 255d), g = (color.G / 255d), b = (color.B / 255d);
		double max = Math.Max(r, Math.Max(g, b));
		double min = Math.Min(r, Math.Min(g, b));
		double delta = (max - min);

		double hue = reference.Hue;
		if(delta > 0)
		{
			if(max == r) { hue = (60d * (((g - b) / delta) % 6d)); }
			else if(max == g) { hue = (60d * (((b - r) / delta) + 2d)); }
			else { hue = (60d * (((r - g) / delta) + 4d)); }
			if(hue < 0) { hue += 360d; }
		}

		double saturation = ((max > 0) ? (delta / max) : reference.Saturation);

		return new _HsvColor(hue, saturation, max, color.A);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the hue in degrees, 0 to 360.</summary>
	public double Hue
	{
		get;
	}


	/// <summary>Gets the saturation, 0 to 1.</summary>
	public double Saturation
	{
		get;
	}


	/// <summary>Gets the value, 0 to 1.</summary>
	public double Value
	{
		get;
	}


	/// <summary>Gets the alpha, 0 to 255.</summary>
	public int Alpha
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts this color.</summary>
	/// <returns>Returns the color.</returns>
	public Color ToColor()
	{
		double c = (Value * Saturation);
		double h = (Hue / 60d);
		double x = (c * (1d - Math.Abs((h % 2d) - 1d)));
		double m = (Value - c);

		(double r, double g, double b) = ((int) h) switch
		{
			0 => (c, x, 0d),
			1 => (x, c, 0d),
			2 => (0d, c, x),
			3 => (0d, x, c),
			4 => (x, 0d, c),
			_ => (c, 0d, x)
		};

		static int toByte(double value) => (int) Math.Round(Math.Clamp(value, 0d, 1d) * 255d);
		return Color.FromArgb(Alpha, toByte(r + m), toByte(g + m), toByte(b + m));
	}
}
