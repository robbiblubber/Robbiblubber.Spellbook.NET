namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements an auto completion source that uses a delegate to provide items.</summary>
/// <remarks>Usage: <c>new AutoCompletionDelegateSource(async (ctx, ct) => new AutoCompletionResult(await LoadAsync(ctx, ct)))</c>
///          or, synchronously, <c>new AutoCompletionDelegateSource(ctx => new AutoCompletionResult(items))</c>.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionDelegateSource: IAutoCompletionSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Asynchronous delegate.</summary>
	private readonly Func<AutoCompletionContext, CancellationToken, ValueTask<AutoCompletionResult?>>? _AsyncDelegate;

	/// <summary>Synchronous delegate.</summary>
	private readonly Func<AutoCompletionContext, AutoCompletionResult?>? _SyncDelegate;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="getItemsAsync">Asynchronous delegate that provides the items.</param>
	/// <exception cref="ArgumentNullException">Thrown when the delegate is NULL.</exception>
	public AutoCompletionDelegateSource(Func<AutoCompletionContext, CancellationToken, ValueTask<AutoCompletionResult?>> getItemsAsync)
	{
		_AsyncDelegate = getItemsAsync ?? throw new ArgumentNullException(nameof(getItemsAsync));
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="getItems">Synchronous delegate that provides the items.</param>
	/// <exception cref="ArgumentNullException">Thrown when the delegate is NULL.</exception>
	public AutoCompletionDelegateSource(Func<AutoCompletionContext, AutoCompletionResult?> getItems)
	{
		_SyncDelegate = getItems ?? throw new ArgumentNullException(nameof(getItems));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IAutoCompletionSource                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken)
	{
		if(_AsyncDelegate is not null) { return _AsyncDelegate(context, cancellationToken); }
		return new ValueTask<AutoCompletionResult?>(_SyncDelegate!(context));
	}
}
