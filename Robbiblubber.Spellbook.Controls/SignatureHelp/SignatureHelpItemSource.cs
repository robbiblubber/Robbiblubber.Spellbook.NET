namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements a signature source that provides signatures from a fixed list.</summary>
/// <remarks>The signatures of a call are the signatures whose <see cref="SignatureInfo.Name"/> equals the name of the call; the
///          qualifier of the call is ignored. For qualifier-dependent signatures, use <see cref="SignatureHelpDelegateSource"/> or
///          implement <see cref="ISignatureHelpSource"/>.</remarks>
[SupportedOSPlatform("windows")]
public class SignatureHelpItemSource: ISignatureHelpSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public SignatureHelpItemSource()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="signatures">Signatures.</param>
	public SignatureHelpItemSource(IEnumerable<SignatureInfo> signatures)
	{
		Signatures.AddRange(signatures);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the signatures.</summary>
	public List<SignatureInfo> Signatures
	{
		get;
	} = new();


	/// <summary>Gets or sets if function names are compared case-sensitively.</summary>
	public bool CaseSensitive
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds a signature.</summary>
	/// <param name="name">Function name.</param>
	/// <param name="parameters">Parameters.</param>
	/// <returns>Returns the added signature.</returns>
	public SignatureInfo Add(string name, params SignatureParameter[] parameters)
	{
		SignatureInfo rval = new(name, parameters);
		Signatures.Add(rval);

		return rval;
	}


	/// <summary>Gets the signatures for a function name.</summary>
	/// <param name="name">Function name.</param>
	/// <returns>Returns the signatures in the order they have been added.</returns>
	public IEnumerable<SignatureInfo> GetSignatures(string name)
	{
		StringComparison comparison = (CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		return Signatures.Where(m => string.Equals(m.Name, name, comparison));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] ISignatureHelpSource                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual ValueTask<SignatureHelpResult?> GetSignaturesAsync(SignatureHelpContext context, CancellationToken cancellationToken)
	{
		SignatureInfo[] signatures = GetSignatures(context.Name).ToArray();
		return new ValueTask<SignatureHelpResult?>((signatures.Length == 0) ? null : new SignatureHelpResult(signatures));
	}
}
