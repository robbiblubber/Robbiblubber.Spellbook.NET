namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="SignatureHelpProvider.Populating"/> event.</summary>
/// <remarks>The event arguments are initialized with the result of the signature source, if any. Event handlers may modify the
///          signature list and the active signature. Changes are applied after the event has been raised.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpPopulatingEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="context">Signature help context.</param>
	/// <param name="result">Result of the signature source, NULL if the source has not provided a result.</param>
	public SignatureHelpPopulatingEventArgs(SignatureHelpContext context, SignatureHelpResult? result): base()
	{
		Context = context;
		Signatures = ((result is null) ? new List<SignatureInfo>() : new List<SignatureInfo>(result.Signatures));
		ActiveSignature = result?.ActiveSignature;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the signature help context.</summary>
	public SignatureHelpContext Context
	{
		get;
	}


	/// <summary>Gets the signatures that will be displayed.</summary>
	public List<SignatureInfo> Signatures
	{
		get;
	}


	/// <summary>Gets or sets the index of the signature that is displayed first.</summary>
	/// <remarks>See <see cref="SignatureHelpResult.ActiveSignature"/>.</remarks>
	public int? ActiveSignature
	{
		get; set;
	}
}
