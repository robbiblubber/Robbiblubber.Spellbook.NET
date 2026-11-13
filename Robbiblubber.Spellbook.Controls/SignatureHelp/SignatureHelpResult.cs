namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents the result of a signature request.</summary>
/// <remarks>Optional properties are set with object initializers: <c>new SignatureHelpResult(signatures) { ActiveSignature = 1 }</c>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpResult
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="signatures">Signatures (overloads) of the called function.</param>
	/// <exception cref="ArgumentNullException">Thrown when the signatures are NULL.</exception>
	public SignatureHelpResult(IEnumerable<SignatureInfo> signatures)
	{
		if(signatures is null) { throw new ArgumentNullException(nameof(signatures)); }
		Signatures = signatures.Where(m => m is not null).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the signatures (overloads) of the called function.</summary>
	public IReadOnlyList<SignatureInfo> Signatures
	{
		get;
	}


	/// <summary>Gets the index of the signature that is displayed first.</summary>
	/// <remarks>If this property is NULL, the first signature that accepts the argument at the caret is displayed, and the displayed
	///          signature follows the argument count while typing. If it is set, the signature is kept for the call, like a signature
	///          the user has selected with the Up and Down keys.</remarks>
	public int? ActiveSignature
	{
		get; init;
	}
}
