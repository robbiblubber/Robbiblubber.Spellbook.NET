namespace Robbiblubber.Spellbook.UnitTests.Controls;

using System.Collections.Concurrent;



/// <summary>This class implements a synchronization context that queues posted callbacks until a test runs them.</summary>
/// <remarks>Asynchronous continuations of the providers (source results, delays) are posted to the UI thread. In tests, there is
///          no message loop, so this context queues them and the test decides when they run by calling <see cref="RunPending"/>.
///          Callbacks that are never run, like the delayed loading indicator, are discarded with the context.</remarks>
public class TestSynchronizationContext: SynchronizationContext
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Posted callbacks.</summary>
	private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _Queue = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Runs all posted callbacks on the calling thread, including callbacks posted while running.</summary>
	/// <returns>Returns the number of callbacks that have been run.</returns>
	public int RunPending()
	{
		int rval = 0;

		while(_Queue.TryDequeue(out (SendOrPostCallback Callback, object? State) item))
		{
			item.Callback(item.State);
			rval++;
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] SynchronizationContext                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override void Post(SendOrPostCallback d, object? state)
	{
		_Queue.Enqueue((d, state));
	}


	/// <inheritdoc/>
	public override void Send(SendOrPostCallback d, object? state)
	{
		d(state);
	}


	/// <inheritdoc/>
	public override SynchronizationContext CreateCopy()
	{
		return this;
	}
}
