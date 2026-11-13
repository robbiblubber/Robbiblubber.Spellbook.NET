namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents a function call found at the caret position.</summary>
/// <remarks>Instances are created by an <see cref="ISignatureCallParser"/> and describe the innermost call whose argument list contains
///          the caret. Two calls are the same call if <see cref="OpenIndex"/>, <see cref="Name"/> and <see cref="Qualifier"/> are equal.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureCall
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="name">Function name.</param>
	/// <param name="qualifier">Qualifier, the dot-separated chain preceding the name without the trailing dot.</param>
	/// <param name="nameStart">Start index of the function name (including the qualifier).</param>
	/// <param name="openIndex">Index of the opening parenthesis.</param>
	/// <param name="argumentIndex">Zero-based index of the argument at the caret.</param>
	/// <param name="argumentStart">Start index of the argument at the caret.</param>
	/// <exception cref="ArgumentNullException">Thrown when the name is NULL.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when an index is negative or the indexes are not in ascending order.</exception>
	public SignatureCall(string name, string? qualifier, int nameStart, int openIndex, int argumentIndex, int argumentStart)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));

		if(nameStart < 0) { throw new ArgumentOutOfRangeException(nameof(nameStart)); }
		if(openIndex < nameStart) { throw new ArgumentOutOfRangeException(nameof(openIndex)); }
		if(argumentIndex < 0) { throw new ArgumentOutOfRangeException(nameof(argumentIndex)); }
		if(argumentStart <= openIndex) { throw new ArgumentOutOfRangeException(nameof(argumentStart)); }

		Qualifier = qualifier ?? string.Empty;
		NameStart = nameStart;
		OpenIndex = openIndex;
		ArgumentIndex = argumentIndex;
		ArgumentStart = argumentStart;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the function name.</summary>
	public string Name
	{
		get;
	}


	/// <summary>Gets the qualifier of the function name.</summary>
	/// <remarks>For <c>dbo.Sales.fn(</c> the qualifier is <c>dbo.Sales</c>. If the name is not qualified, the qualifier is an empty string.</remarks>
	public string Qualifier
	{
		get;
	}


	/// <summary>Gets the start index of the function name, including the qualifier.</summary>
	public int NameStart
	{
		get;
	}


	/// <summary>Gets the index of the opening parenthesis.</summary>
	public int OpenIndex
	{
		get;
	}


	/// <summary>Gets the zero-based index of the argument at the caret.</summary>
	public int ArgumentIndex
	{
		get;
	}


	/// <summary>Gets the start index of the argument at the caret (the index after the opening parenthesis or the preceding separator).</summary>
	public int ArgumentStart
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if another call is the same call, possibly at a different argument.</summary>
	/// <param name="other">Other call.</param>
	/// <returns>Returns <c>true</c> if both calls have the same opening parenthesis, name and qualifier, otherwise returns <c>false</c>.</returns>
	public bool IsSameCall(SignatureCall? other)
	{
		return ((other is not null) && (other.OpenIndex == OpenIndex) && string.Equals(other.Name, Name, StringComparison.Ordinal) && string.Equals(other.Qualifier, Qualifier, StringComparison.Ordinal));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the qualified name and the argument index.</returns>
	public override string ToString()
	{
		return ((Qualifier.Length > 0) ? (Qualifier + ".") : string.Empty) + Name + $"(#{ArgumentIndex})";
	}
}
