namespace Robbiblubber.Spellbook.UnitTests.Core.Helpers;

using System.Net.Sockets;
using NUnit.Framework;



/// <summary>This class tests the disposal helpers.</summary>
[TestFixture]
public class DisposalTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test classes                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Records calls in a log.</summary>
	public class Logged
	{
		/// <summary>Gets the call log.</summary>
		public List<string> Log { get; } = new();
	}


	/// <summary>Explicit IDisposable with Close.</summary>
	public class Explicit: Logged, IDisposable
	{
		/// <summary>Closes the object.</summary>
		public void Close() { Log.Add("Close"); }

		/// <summary>Disposes the object.</summary>
		void IDisposable.Dispose() { Log.Add("Dispose"); }
	}


	/// <summary>Asynchronous disposal only.</summary>
	public class AsyncOnly: Logged, IAsyncDisposable
	{
		/// <summary>Disposes the object.</summary>
		public async ValueTask DisposeAsync() { await Task.Yield(); Log.Add("DisposeAsync"); }
	}


	/// <summary>Interop-like object: Close with optional parameters, Dispose without IDisposable, overloads.</summary>
	public class InteropLike: Logged
	{
		/// <summary>Closes the object.</summary>
		public void Close(bool save, string file) { Log.Add("Close(" + save + ")"); }

		/// <summary>Closes the object.</summary>
		public void Close(bool save = true, string? file = null, int route = 7) { Log.Add("Close(" + save + "," + file + "," + route + ")"); }

		/// <summary>Disposes the object.</summary>
		public void Dispose(bool disposing) { Log.Add("Dispose(bool)"); }

		/// <summary>Disposes the object.</summary>
		public void Dispose() { Log.Add("Dispose"); }
	}


	/// <summary>Object whose Close fails.</summary>
	public class Failing: Logged, IDisposable
	{
		/// <summary>Closes the object.</summary>
		public void Close() { throw new InvalidOperationException(); }

		/// <summary>Disposes the object.</summary>
		public void Dispose() { Log.Add("Dispose"); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Overloaded Close does not cause an AmbiguousMatchException.</summary>
	[Test]
	public void Socket()
	{
		Socket s = new(SocketType.Stream, ProtocolType.Tcp);

		Assert.That(Disposal.TryDispose(s), Is.True);
		Assert.Throws<ObjectDisposedException>(() => _ = s.Available);
	}


	/// <summary>Close is called first, then the explicit IDisposable implementation.</summary>
	[Test]
	public void Explicit_CloseThenDispose()
	{
		Explicit e = new();
		Disposal.Dispose(e);

		Assert.That(e.Log, Is.EqualTo(new[] { "Close", "Dispose" }));
	}


	/// <summary>IAsyncDisposable is used synchronously and asynchronously.</summary>
	[Test]
	public async Task AsyncDisposable()
	{
		AsyncOnly a = new();
		Disposal.Dispose(a);
		Assert.That(a.Log, Is.EqualTo(new[] { "DisposeAsync" }));

		AsyncOnly b = new();
		Assert.That(await Disposal.TryDisposeAsync(b, null), Is.True);
		Assert.That(b.Log, Is.EqualTo(new[] { "DisposeAsync" }));
	}


	/// <summary>Close with optional parameters gets the defaults; the parameterless Dispose is chosen.</summary>
	[Test]
	public void OptionalParameters()
	{
		InteropLike o = new();
		Disposal.Dispose(o);

		Assert.That(o.Log, Is.EqualTo(new[] { "Close(True,,7)", "Dispose" }));
	}


	/// <summary>A failing object does not stop the others and is reported.</summary>
	[Test]
	public void Errors()
	{
		Failing f = new();
		Explicit e = new();

		Assert.That(Disposal.TryDispose(null, f, e), Is.False);
		Assert.That(f.Log, Is.EqualTo(new[] { "Dispose" }));
		Assert.That(e.Log, Is.EqualTo(new[] { "Close", "Dispose" }));

		Assert.DoesNotThrow(() => Disposal.Dispose(new Failing()));
		Assert.DoesNotThrow(() => Disposal.Dispose(null));
		Assert.That(Disposal.TryDispose(new object(), "text", 42), Is.True);
	}


	/// <summary>Recycle disposes the objects and collects them.</summary>
	[Test]
	public void Recycle()
	{
		Explicit e = new();
		Disposal.Recycle(e);
		Assert.That(e.Log, Is.EqualTo(new[] { "Close", "Dispose" }));

		WeakReference w = _Garbage();
		Disposal.Recycle();
		Assert.That(w.IsAlive, Is.False);
	}


	/// <summary>Non-COM objects are skipped by ReleaseComObjects.</summary>
	[Test]
	public void ReleaseComObjects()
	{
		if(OperatingSystem.IsWindows())
		{
			Assert.That(Disposal.ReleaseComObjects(null, new object(), "x"), Is.True);
		}
		else { Assert.Ignore("Windows only."); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates an object that is only weakly referenced.</summary>
	/// <returns>Returns the weak reference.</returns>
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
	private static WeakReference _Garbage()
	{
		return new WeakReference(new object());
	}
}
