namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="SignatureHelpProvider.Starting"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpStartingEventArgs: CancelEventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="context">Signature help context.</param>
	public SignatureHelpStartingEventArgs(SignatureHelpContext context): base()
	{
		Context = context;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the signature help context.</summary>
	public SignatureHelpContext Context
	{
		get;
	}
}
