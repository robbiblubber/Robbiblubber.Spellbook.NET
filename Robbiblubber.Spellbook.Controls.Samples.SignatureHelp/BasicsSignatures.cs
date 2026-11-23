namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.Versioning;



/// <summary>This class provides the signatures of the basics scenario.</summary>
/// <remarks>The functions cover overloads with the same and with different parameter counts, a variadic parameter, a function without
///          parameters and long signatures and descriptions that wrap.</remarks>
[SupportedOSPlatform("windows")]
public static class BasicsSignatures
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a parameter.</summary>
	/// <param name="label">Label.</param>
	/// <param name="description">Description.</param>
	/// <param name="variadic">Determines if the parameter is variadic.</param>
	/// <returns>Returns the parameter.</returns>
	private static SignatureParameter _Parameter(string label, string description, bool variadic = false)
	{
		return new SignatureParameter(label, description) { IsVariadic = variadic };
	}


	/// <summary>Creates a signature.</summary>
	/// <param name="name">Function name.</param>
	/// <param name="suffix">Return type.</param>
	/// <param name="description">Description.</param>
	/// <param name="parameters">Parameters.</param>
	/// <returns>Returns the signature.</returns>
	private static SignatureInfo _Signature(string name, string suffix, string description, params SignatureParameter[] parameters)
	{
		return new SignatureInfo(name, parameters) { Suffix = "→ " + suffix, Description = description };
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the signature source.</summary>
	/// <returns>Returns the source.</returns>
	public static SignatureHelpItemSource CreateSource()
	{
		SignatureHelpItemSource rval = new() { CaseSensitive = true };
		SignatureParameter value = _Parameter("double value", "The number to round.");
		SignatureParameter digits = _Parameter("int digits", "Number of fractional digits.");
		SignatureParameter x = _Parameter("double x", "A positive number.");
		SignatureParameter start = _Parameter("int start", "Zero-based start position.");

		rval.Signatures.Add(_Signature("Max", "int", "Returns the larger of two integers.", _Parameter("int a", "First value."), _Parameter("int b", "Second value.")));
		rval.Signatures.Add(_Signature("Max", "double", "Returns the larger of two double-precision numbers.", _Parameter("double a", "First value."), _Parameter("double b", "Second value.")));

		rval.Signatures.Add(_Signature("Clamp", "double", "Returns value clamped to the inclusive range of min and max.",
				_Parameter("double value", "The value to clamp."), _Parameter("double min", "Lower bound."), _Parameter("double max", "Upper bound.")));

		rval.Signatures.Add(_Signature("Substring", "string", "Returns the rest of the string from a start position.", start));
		rval.Signatures.Add(_Signature("Substring", "string", "Returns a part of the string.", start, _Parameter("int length", "Number of characters.")));

		rval.Signatures.Add(_Signature("Round", "double", "Rounds to the nearest integral value.", value));
		rval.Signatures.Add(_Signature("Round", "double", "Rounds to a number of fractional digits.", value, digits));
		rval.Signatures.Add(_Signature("Round", "double", "Rounds to a number of fractional digits, using a midpoint rounding mode.", value, digits,
				_Parameter("MidpointRounding mode", "How to round values that are exactly halfway between two numbers: ToEven (banker's rounding) or AwayFromZero.")));

		rval.Signatures.Add(_Signature("Log", "double", "Returns the natural logarithm.", x));
		rval.Signatures.Add(_Signature("Log", "double", "Returns the logarithm in a base.", x, _Parameter("double base", "The base of the logarithm.")));

		rval.Signatures.Add(_Signature("Format", "string", "Replaces the format items in a string with the string representations of the arguments. Any number of arguments can follow the format string; all of them belong to the last parameter.",
				_Parameter("string format", "A composite format string with placeholders like {0}."), _Parameter("params object[] args", "The values to format.", true)));

		rval.Signatures.Add(_Signature("Now", "DateTime", "Returns the current local date and time."));

		rval.Signatures.Add(_Signature("CreateRectangle", "Rectangle", "A function with many parameters, so the signature wraps in the window.",
				_Parameter("int x", "Left edge."), _Parameter("int y", "Top edge."), _Parameter("int width", "Width."), _Parameter("int height", "Height."),
				_Parameter("Color fill", "Fill color."), _Parameter("Color border", "Border color."), _Parameter("int borderWidth", "Border width in pixels."),
				_Parameter("bool rounded", "Rounded corners.")));

		return rval;
	}
}
