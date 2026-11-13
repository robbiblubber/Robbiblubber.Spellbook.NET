using System.Runtime.Versioning;

namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;



/// <summary>This class implements a signature source for SQL functions.</summary>
/// <remarks>Calls qualified with <c>dbo</c> are looked up in the user-defined functions (<c>dbo.GetCustomerTotal</c>), all other calls
///          in the built-in functions, to demonstrate qualifier-dependent signatures. A simulated latency and failure test the
///          asynchronous behavior.</remarks>
[SupportedOSPlatform("windows")]
public class SqlSampleSignatureSource: ISignatureHelpSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Built-in function signatures.</summary>
	private readonly SignatureHelpItemSource _BuiltIn = new();

	/// <summary>User-defined function signatures of the dbo schema.</summary>
	private readonly SignatureHelpItemSource _UserDefined = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public SqlSampleSignatureSource()
	{
		SignatureParameter expression = _Parameter("expression", "Any valid expression.");
		SignatureParameter numeric = _Parameter("numeric_expression", "An expression of an exact or approximate numeric data type.");
		SignatureParameter characters = _Parameter("character_expression", "An expression of character or binary data.");
		SignatureParameter datepart = _Parameter("datepart", "The part of the date: year, quarter, month, dayofyear, day, week, weekday, hour, minute, second, millisecond.");
		SignatureParameter dataType = _Parameter("data_type", "The target data type.");
		SignatureParameter length = _Parameter("length", "The precision; negative values round to the left of the decimal point.");

		_BuiltIn.Signatures.Add(_Signature("ABS", "same type", "Returns the absolute (positive) value of a numeric expression.", numeric));
		_BuiltIn.Signatures.Add(_Signature("AVG", "numeric", "Returns the average of the values in a group. NULL values are ignored.", expression));
		_BuiltIn.Signatures.Add(_Signature("SUM", "numeric", "Returns the sum of the values in a group. NULL values are ignored.", expression));
		_BuiltIn.Signatures.Add(_Signature("MIN", "same type", "Returns the minimum value in a group.", expression));
		_BuiltIn.Signatures.Add(_Signature("MAX", "same type", "Returns the maximum value in a group.", expression));
		_BuiltIn.Signatures.Add(_Signature("COUNT", "int", "Returns the number of non-NULL values in a group.", expression));
		_BuiltIn.Signatures.Add(_Signature("COUNT", "int", "Returns the number of rows in a group.", _Parameter("*", "Counts all rows, including NULL values and duplicates.")));

		_BuiltIn.Signatures.Add(_Signature("CAST", "data_type", "Converts an expression to a data type.", _Parameter("expression AS data_type", "The expression and the target data type, for example Total AS int.")));
		_BuiltIn.Signatures.Add(_Signature("CONVERT", "data_type", "Converts an expression to a data type.", dataType, expression));
		_BuiltIn.Signatures.Add(_Signature("CONVERT", "data_type", "Converts an expression to a data type, with a style for dates and numbers.", dataType, expression,
				_Parameter("style", "An integer that specifies how the expression is formatted, for example 104 for dd.mm.yyyy.")));

		_BuiltIn.Signatures.Add(_Signature("COALESCE", "first non-NULL type", "Returns the first expression that is not NULL.",
				_Parameter("expression1", "The first expression."), _Parameter("expression2, ...", "Further expressions, checked in order.", true)));
		_BuiltIn.Signatures.Add(_Signature("CONCAT", "nvarchar", "Concatenates two or more values.",
				_Parameter("string_value1", "The first value."), _Parameter("string_value2, ...", "Further values. NULL values are converted to empty strings.", true)));
		_BuiltIn.Signatures.Add(_Signature("ISNULL", "type of check_expression", "Replaces NULL with a replacement value.",
				_Parameter("check_expression", "The expression that is checked for NULL."), _Parameter("replacement_value", "The value returned if check_expression is NULL.")));
		_BuiltIn.Signatures.Add(_Signature("IIF", "type of the result", "Returns one of two values, depending on a condition.",
				_Parameter("boolean_expression", "The condition."), _Parameter("true_value", "Returned if the condition is true."), _Parameter("false_value", "Returned if the condition is false or unknown.")));

		_BuiltIn.Signatures.Add(_Signature("DATEADD", "type of date", "Adds a number of dateparts to a date.",
				datepart, _Parameter("number", "The number of dateparts to add; negative values subtract."), _Parameter("date", "The date to add to.")));
		_BuiltIn.Signatures.Add(_Signature("DATEDIFF", "int", "Returns the number of datepart boundaries crossed between two dates.",
				datepart, _Parameter("startdate", "The start date."), _Parameter("enddate", "The end date.")));
		_BuiltIn.Signatures.Add(_Signature("GETDATE", "datetime", "Returns the current date and time of the database server."));
		_BuiltIn.Signatures.Add(_Signature("YEAR", "int", "Returns the year of a date.", _Parameter("date", "A date expression.")));

		_BuiltIn.Signatures.Add(_Signature("LEN", "int", "Returns the number of characters, excluding trailing spaces.", _Parameter("string_expression", "The string to measure.")));
		_BuiltIn.Signatures.Add(_Signature("LOWER", "varchar", "Converts to lower case.", characters));
		_BuiltIn.Signatures.Add(_Signature("UPPER", "varchar", "Converts to upper case.", characters));
		_BuiltIn.Signatures.Add(_Signature("REPLACE", "varchar", "Replaces all occurrences of a substring.",
				_Parameter("string_expression", "The string to search."), _Parameter("string_pattern", "The substring to find."), _Parameter("string_replacement", "The replacement string.")));
		_BuiltIn.Signatures.Add(_Signature("SUBSTRING", "varchar", "Returns part of a character, binary, text or image expression.",
				expression, _Parameter("start", "The 1-based position of the first character."), _Parameter("length", "The number of characters.")));
		_BuiltIn.Signatures.Add(_Signature("ROUND", "same type", "Rounds a numeric value.", numeric, length));
		_BuiltIn.Signatures.Add(_Signature("ROUND", "same type", "Rounds or truncates a numeric value.", numeric, length, _Parameter("function", "0 rounds, any other value truncates.")));

		_UserDefined.Signatures.Add(_Signature("GetCustomerTotal", "decimal(18,2)", "User-defined function dbo.GetCustomerTotal: total of all orders of a customer within a date range.",
				_Parameter("@CustomerId int", "The customer."), _Parameter("@From date", "First order date (inclusive)."), _Parameter("@To date", "Last order date (inclusive).")));
	}



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
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets a simulated latency in milliseconds.</summary>
	public int Latency
	{
		get; set;
	}


	/// <summary>Gets or sets if the source throws an exception to simulate a failure.</summary>
	public bool ThrowException
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] ISignatureHelpSource                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public async ValueTask<SignatureHelpResult?> GetSignaturesAsync(SignatureHelpContext context, CancellationToken cancellationToken)
	{
		SignatureInfo[] signatures;

		if(Latency > 0) { await Task.Delay(Latency, cancellationToken); }
		if(ThrowException) { throw new InvalidOperationException("Simulated signature source failure."); }

		signatures = (context.Qualifier.Equals(SqlSampleSchema.SchemaName, StringComparison.OrdinalIgnoreCase) ? _UserDefined : _BuiltIn).GetSignatures(context.Name).ToArray();
		return ((signatures.Length == 0) ? null : new SignatureHelpResult(signatures));
	}
}
