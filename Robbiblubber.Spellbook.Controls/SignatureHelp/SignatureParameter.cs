namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents a parameter of a signature.</summary>
/// <remarks>Optional properties are set with object initializers: <c>new SignatureParameter("start") { Description = "1-based start position." }</c>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureParameter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="label">Parameter label as displayed in the signature, for example <c>start</c> or <c>int start</c>.</param>
	/// <exception cref="ArgumentNullException">Thrown when the label is NULL.</exception>
	public SignatureParameter(string label)
	{
		Label = label ?? throw new ArgumentNullException(nameof(label));
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="label">Parameter label as displayed in the signature.</param>
	/// <param name="description">Parameter description.</param>
	/// <exception cref="ArgumentNullException">Thrown when the label is NULL.</exception>
	public SignatureParameter(string label, string? description): this(label)
	{
		Description = description;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the parameter label as displayed in the signature.</summary>
	public string Label
	{
		get; init;
	}


	/// <summary>Gets the parameter description.</summary>
	/// <remarks>The description of the active parameter is displayed below the signature.</remarks>
	public string? Description
	{
		get; init;
	}


	/// <summary>Gets if the parameter accepts any number of arguments.</summary>
	/// <remarks>Only the last parameter of a signature should be variadic. Arguments beyond the last parameter are assigned to it;
	///          the label usually indicates the repetition, for example <c>value, ...</c>.</remarks>
	public bool IsVariadic
	{
		get; init;
	}


	/// <summary>Gets a user-defined object associated with the parameter.</summary>
	public object? Tag
	{
		get; init;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the label.</returns>
	public override string ToString()
	{
		return Label;
	}
}
