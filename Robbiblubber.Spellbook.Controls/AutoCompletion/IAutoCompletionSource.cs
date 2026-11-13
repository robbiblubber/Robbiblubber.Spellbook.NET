namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that provide auto completion items implement this interface.</summary>
/// <remarks><see cref="GetItemsAsync"/> is called on the UI thread. Implementations that perform expensive work should do so
///          asynchronously or offload it to a background thread. Results that are available immediately (for example from a cache)
///          should be returned synchronously as a completed <see cref="ValueTask{TResult}"/>.</remarks>
[SupportedOSPlatform("windows")]
public interface IAutoCompletionSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the auto completion items for a context.</summary>
	/// <param name="context">Auto completion context.</param>
	/// <param name="cancellationToken">Cancellation token. The token is cancelled when the request becomes obsolete.</param>
	/// <returns>Returns the auto completion result, or NULL if no items are available.</returns>
	public ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken);
}
