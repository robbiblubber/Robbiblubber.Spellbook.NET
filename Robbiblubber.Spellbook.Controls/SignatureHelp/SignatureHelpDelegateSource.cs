namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements a signature source that uses a delegate to provide signatures.</summary>
/// <remarks>Usage: <c>new SignatureHelpDelegateSource(async (ctx, ct) => new SignatureHelpResult(await LoadAsync(ctx, ct)))</c>
///          or, synchronously, <c>new SignatureHelpDelegateSource(ctx => new SignatureHelpResult(Lookup(ctx.Name)))</c>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpDelegateSource: ISignatureHelpSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Asynchronous delegate.</summary>
	private readonly Func<SignatureHelpContext, CancellationToken, ValueTask<SignatureHelpResult?>>? _AsyncDelegate;

	/// <summary>Synchronous delegate.</summary>
	private readonly Func<SignatureHelpContext, SignatureHelpResult?>? _SyncDelegate;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="getSignaturesAsync">Asynchronous delegate that provides the signatures.</param>
	/// <exception cref="ArgumentNullException">Thrown when the delegate is NULL.</exception>
	public SignatureHelpDelegateSource(Func<SignatureHelpContext, CancellationToken, ValueTask<SignatureHelpResult?>> getSignaturesAsync)
	{
		_AsyncDelegate = getSignaturesAsync ?? throw new ArgumentNullException(nameof(getSignaturesAsync));
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="getSignatures">Synchronous delegate that provides the signatures.</param>
	/// <exception cref="ArgumentNullException">Thrown when the delegate is NULL.</exception>
	public SignatureHelpDelegateSource(Func<SignatureHelpContext, SignatureHelpResult?> getSignatures)
	{
		_SyncDelegate = getSignatures ?? throw new ArgumentNullException(nameof(getSignatures));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] ISignatureHelpSource                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public ValueTask<SignatureHelpResult?> GetSignaturesAsync(SignatureHelpContext context, CancellationToken cancellationToken)
	{
		if(_AsyncDelegate is not null) { return _AsyncDelegate(context, cancellationToken); }
		return new ValueTask<SignatureHelpResult?>(_SyncDelegate!(context));
	}
}
