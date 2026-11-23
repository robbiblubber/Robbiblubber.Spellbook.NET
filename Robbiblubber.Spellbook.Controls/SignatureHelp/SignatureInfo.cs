namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents a signature (an overload) of a function or method.</summary>
/// <remarks>The displayed signature is composed of <see cref="Name"/>, <see cref="OpenText"/>, the parameter labels separated by
///          <see cref="SeparatorText"/>, <see cref="CloseText"/> and <see cref="Suffix"/>, for example
///          <c>SUBSTRING(expression, start, length) → varchar</c>. Optional properties are set with object initializers.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureInfo
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="name">Function name as displayed in the signature.</param>
	/// <param name="parameters">Parameters.</param>
	/// <exception cref="ArgumentNullException">Thrown when the name is NULL.</exception>
	public SignatureInfo(string name, params SignatureParameter[] parameters)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Parameters = (parameters ?? []).Where(m => m is not null).ToArray();
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="name">Function name as displayed in the signature.</param>
	/// <param name="parameters">Parameters.</param>
	/// <exception cref="ArgumentNullException">Thrown when the name is NULL.</exception>
	public SignatureInfo(string name, IEnumerable<SignatureParameter> parameters): this(name, parameters?.ToArray() ?? [])
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the function name as displayed in the signature.</summary>
	public string Name
	{
		get; init;
	}


	/// <summary>Gets the parameters.</summary>
	public IReadOnlyList<SignatureParameter> Parameters
	{
		get; init;
	}


	/// <summary>Gets the text displayed between the name and the first parameter.</summary>
	public string OpenText
	{
		get; init;
	} = "(";


	/// <summary>Gets the text displayed between parameters.</summary>
	public string SeparatorText
	{
		get; init;
	} = ", ";


	/// <summary>Gets the text displayed after the last parameter.</summary>
	public string CloseText
	{
		get; init;
	} = ")";


	/// <summary>Gets the text displayed after the signature, for example the return type.</summary>
	public string? Suffix
	{
		get; init;
	}


	/// <summary>Gets the description of the signature.</summary>
	public string? Description
	{
		get; init;
	}


	/// <summary>Gets a user-defined object associated with the signature.</summary>
	public object? Tag
	{
		get; init;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the parameter that receives an argument.</summary>
	/// <param name="argumentIndex">Zero-based argument index.</param>
	/// <returns>Returns the parameter index, or -1 if the signature has no parameter for the argument.</returns>
	/// <remarks>Arguments beyond the last parameter are assigned to it if it is variadic.</remarks>
	public int GetParameterIndex(int argumentIndex)
	{
		if((argumentIndex < 0) || (Parameters.Count == 0)) { return -1; }
		if(argumentIndex < Parameters.Count) { return argumentIndex; }

		return (Parameters[^1].IsVariadic ? (Parameters.Count - 1) : -1);
	}


	/// <summary>Returns if the signature accepts an argument.</summary>
	/// <param name="argumentIndex">Zero-based argument index.</param>
	/// <returns>Returns <c>true</c> if the signature has a parameter for the argument, otherwise returns <c>false</c>.</returns>
	public bool Accepts(int argumentIndex)
	{
		return ((argumentIndex == 0) || (GetParameterIndex(argumentIndex) >= 0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the displayed signature.</returns>
	public override string ToString()
	{
		return Name + OpenText + string.Join(SeparatorText, Parameters.Select(m => m.Label)) + CloseText + ((Suffix is null) ? string.Empty : (" " + Suffix));
	}
}
