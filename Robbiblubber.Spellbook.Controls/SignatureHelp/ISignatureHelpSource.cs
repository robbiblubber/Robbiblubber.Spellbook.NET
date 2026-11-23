namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that provide signatures for function calls implement this interface.</summary>
/// <remarks><see cref="GetSignaturesAsync"/> is called on the UI thread once per call; moving between the arguments of the same call
///          does not request the signatures again. Results that are available immediately should be returned synchronously as a
///          completed <see cref="ValueTask{TResult}"/>.</remarks>
[SupportedOSPlatform("windows")]
public interface ISignatureHelpSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the signatures for a call.</summary>
	/// <param name="context">Signature help context.</param>
	/// <param name="cancellationToken">Cancellation token. The token is cancelled when the request becomes obsolete.</param>
	/// <returns>Returns the result, or NULL if no signatures are available.</returns>
	public ValueTask<SignatureHelpResult?> GetSignaturesAsync(SignatureHelpContext context, CancellationToken cancellationToken);
}
