namespace Robbiblubber.Spellbook;

using System.Buffers.Binary;
using System.IO.Pipes;
using System.Text;



/// <summary>This class makes sure that only one instance of an application runs for the current user and passes the arguments of
///          further starts to this instance.</summary>
/// <remarks><para>The first instance that is created for an identifier becomes the first instance (<see cref="IsFirstInstance"/>)
///          and receives arguments with <see cref="StartListening"/> and the <see cref="ArgumentsReceived"/> event. Further
///          instances pass their arguments to the first instance with <see cref="Forward(IEnumerable{string}, int)"/> and usually
///          end afterwards.</para>
///          <para>The instances are found by a named mutex and a named pipe whose names are derived from the identifier and the
///          user name. The pipe only accepts connections from the current user.</para>
///          <para>The <see cref="ArgumentsReceived"/> event is raised on a thread pool thread; user interface applications have to
///          marshal it to their user interface thread.</para></remarks>
/// <example><code>
/// using SingleInstance instance = new("robbiblubber.org/MyTool");
/// if(!instance.IsFirstInstance &amp;&amp; instance.Forward(args)) { return; }
///
/// instance.ArgumentsReceived += (s, e) => Open(e.Arguments.Select(e.GetFullPath));
/// instance.StartListening();
/// </code></example>
public class SingleInstance: IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constants                                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Default timeout for forwarding arguments in milliseconds.</summary>
	public const int DEFAULT_TIMEOUT = 5000;

	/// <summary>Message signature.</summary>
	private const int _SIGNATURE = 0x49534252;

	/// <summary>Protocol version.</summary>
	private const int _VERSION = 1;

	/// <summary>Acknowledgement byte.</summary>
	private const byte _ACK = 0x06;

	/// <summary>Maximum message size in bytes.</summary>
	private const int _MAX_MESSAGE_SIZE = 1048576;

	/// <summary>Time in milliseconds the first instance waits for a complete message.</summary>
	private const int _RECEIVE_TIMEOUT = 5000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Mutex names held by instances in this process.</summary>
	private static readonly HashSet<string> _Held = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Mutex.</summary>
	private Mutex? _Mutex;

	/// <summary>Managed thread ID of the thread that has acquired the mutex.</summary>
	private readonly int _MutexThread;

	/// <summary>Mutex name.</summary>
	private readonly string _MutexName;

	/// <summary>Pipe name.</summary>
	private readonly string _PipeName;

	/// <summary>Cancellation source of the listener.</summary>
	private CancellationTokenSource? _Cancel = null;

	/// <summary>Listener task.</summary>
	private Task? _Listener = null;

	/// <summary>Disposed flag.</summary>
	private bool _Disposed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="id">Application identifier, for example <c>"robbiblubber.org/MyTool"</c>.</param>
	/// <exception cref="ArgumentException">Thrown when the identifier is empty.</exception>
	public SingleInstance(string id)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id);

		Id = id;
		byte[] key = Encoding.UTF8.GetBytes(id + "\n" + Environment.UserDomainName + "\\" + Environment.UserName);
		string hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(key))[..32].ToLowerInvariant();
		_MutexName = @"Local\sbsi-" + hash;
		_PipeName = "sbsi-" + hash;

		lock(_Held)
		{
			if(_Held.Contains(_MutexName)) { return; }

			Mutex m = new(false, _MutexName);
			bool owned;
			try
			{
				owned = m.WaitOne(0);
			}
			catch(AbandonedMutexException) { owned = true; }

			if(owned)
			{
				_Mutex = m;
				_MutexThread = Environment.CurrentManagedThreadId;
				_Held.Add(_MutexName);
				IsFirstInstance = true;
			}
			else { m.Dispose(); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Occurs when another instance has forwarded its arguments to this instance.</summary>
	/// <remarks>The event is raised on a thread pool thread. Exceptions thrown by event handlers are ignored.</remarks>
	public event EventHandler<SingleInstanceEventArgs>? ArgumentsReceived;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Gets the application identifier.</summary>
	public string Id
	{
		get;
	}


	/// <summary>Gets if this is the first instance.</summary>
	public bool IsFirstInstance
	{
		get;
	}


	/// <summary>Gets if the instance receives arguments from other instances.</summary>
	public bool IsListening
	{
		get { return (_Listener != null); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Starts receiving arguments from other instances.</summary>
	/// <remarks>Calling the method again has no effect.</remarks>
	/// <exception cref="InvalidOperationException">Thrown when this is not the first instance.</exception>
	/// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
	public void StartListening()
	{
		ObjectDisposedException.ThrowIf(_Disposed, this);
		if(!IsFirstInstance) { throw new InvalidOperationException("Only the first instance can receive arguments."); }
		if(_Listener != null) return;

		_Cancel = new();
		CancellationToken token = _Cancel.Token;
		_Listener = Task.Run(() => _ListenAsync(token));
	}


	/// <summary>Passes arguments to the first instance.</summary>
	/// <param name="arguments">Arguments.</param>
	/// <param name="timeout">Time in milliseconds to wait for the first instance.</param>
	/// <returns>Returns TRUE if the first instance has received the arguments, otherwise returns FALSE.</returns>
	/// <remarks>The current directory is passed along, so that the first instance can resolve relative paths with
	///          <see cref="SingleInstanceEventArgs.GetFullPath(string)"/>. The method returns FALSE if the first instance does not
	///          listen or does not answer in time, for example because it is closing.</remarks>
	/// <exception cref="InvalidOperationException">Thrown when this is the first instance.</exception>
	/// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
	public bool Forward(IEnumerable<string> arguments, int timeout = DEFAULT_TIMEOUT)
	{
		ArgumentNullException.ThrowIfNull(arguments);
		ObjectDisposedException.ThrowIf(_Disposed, this);
		if(IsFirstInstance) { throw new InvalidOperationException("The first instance cannot forward arguments."); }

		byte[] message = _Pack(arguments.ToArray(), Environment.CurrentDirectory);
		return Task.Run(() => _SendAsync(message, timeout)).GetAwaiter().GetResult();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Raises the <see cref="ArgumentsReceived"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnArgumentsReceived(SingleInstanceEventArgs e)
	{
		ArgumentsReceived?.Invoke(this, e);
	}


	/// <summary>Releases the resources used by this instance.</summary>
	/// <param name="disposing">Determines if managed resources are released.</param>
	protected virtual void _Dispose(bool disposing)
	{
		if(_Disposed) return;
		_Disposed = true;

		if(!disposing) return;

		if(_Cancel != null)
		{
			_Cancel.Cancel();
			try { _Listener?.Wait(1000); } catch(Exception) {}
			_Cancel.Dispose();
			_Cancel = null;
		}
		_Listener = null;

		if(_Mutex != null)
		{
			lock(_Held) { _Held.Remove(_MutexName); }
			if(Environment.CurrentManagedThreadId == _MutexThread)
			{
				try { _Mutex.ReleaseMutex(); } catch(ApplicationException) {}
			}
			_Mutex.Dispose();
			_Mutex = null;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a message.</summary>
	/// <param name="arguments">Arguments.</param>
	/// <param name="workingDirectory">Working directory.</param>
	/// <returns>Returns the message without length prefix.</returns>
	private static byte[] _Pack(string[] arguments, string workingDirectory)
	{
		using MemoryStream rval = new();
		using(BinaryWriter w = new(rval, Encoding.UTF8, true))
		{
			w.Write(_SIGNATURE);
			w.Write(_VERSION);
			w.Write(workingDirectory);
			w.Write(arguments.Length);
			foreach(string i in arguments) { w.Write(i ?? string.Empty); }
		}
		if(rval.Length > _MAX_MESSAGE_SIZE) { throw new ArgumentException("The arguments are too long.", nameof(arguments)); }
		return rval.ToArray();
	}


	/// <summary>Reads a message.</summary>
	/// <param name="message">Message without length prefix.</param>
	/// <returns>Returns the event arguments, or NULL if the message is invalid.</returns>
	private static SingleInstanceEventArgs? _Unpack(byte[] message)
	{
		try
		{
			using BinaryReader r = new(new MemoryStream(message), Encoding.UTF8);
			if(r.ReadInt32() != _SIGNATURE) { return null; }
			if(r.ReadInt32() != _VERSION) { return null; }

			string dir = r.ReadString();
			int n = r.ReadInt32();
			if((n < 0) || (n > message.Length)) { return null; }

			string[] args = new string[n];
			for(int i = 0; i < n; i++) { args[i] = r.ReadString(); }

			return new SingleInstanceEventArgs(args, dir);
		}
		catch(Exception ex) when ((ex is EndOfStreamException) || (ex is IOException) || (ex is FormatException)) { return null; }
	}


	/// <summary>Reads a number of bytes from a stream.</summary>
	/// <param name="stream">Stream.</param>
	/// <param name="count">Number of bytes.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Returns the bytes, or NULL if the stream has ended before.</returns>
	private static async Task<byte[]?> _ReadAsync(Stream stream, int count, CancellationToken token)
	{
		byte[] rval = new byte[count];
		int m = 0;
		while(m < count)
		{
			int n = await stream.ReadAsync(rval.AsMemory(m, count - m), token).ConfigureAwait(false);
			if(n == 0) { return null; }
			m += n;
		}
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Sends a message to the first instance.</summary>
	/// <param name="message">Message.</param>
	/// <param name="timeout">Timeout in milliseconds.</param>
	/// <returns>Returns TRUE if the first instance has acknowledged the message, otherwise returns FALSE.</returns>
	private async Task<bool> _SendAsync(byte[] message, int timeout)
	{
		try
		{
			using CancellationTokenSource cancel = new(timeout);
			using NamedPipeClientStream client = new(".", _PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
			await client.ConnectAsync(cancel.Token).ConfigureAwait(false);

			byte[] length = new byte[4];
			BinaryPrimitives.WriteInt32LittleEndian(length, message.Length);
			await client.WriteAsync(length, cancel.Token).ConfigureAwait(false);
			await client.WriteAsync(message, cancel.Token).ConfigureAwait(false);
			await client.FlushAsync(cancel.Token).ConfigureAwait(false);

			byte[]? ack = await _ReadAsync(client, 1, cancel.Token).ConfigureAwait(false);
			return ((ack != null) && (ack[0] == _ACK));
		}
		catch(Exception ex) when ((ex is OperationCanceledException) || (ex is TimeoutException) || (ex is IOException) || (ex is UnauthorizedAccessException))
		{
			return false;
		}
	}


	/// <summary>Receives messages until the listener is cancelled.</summary>
	/// <param name="token">Cancellation token.</param>
	private async Task _ListenAsync(CancellationToken token)
	{
		while(!token.IsCancellationRequested)
		{
			SingleInstanceEventArgs? e = null;
			try
			{
				using NamedPipeServerStream server = new(_PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
				await server.WaitForConnectionAsync(token).ConfigureAwait(false);

				using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
				timeout.CancelAfter(_RECEIVE_TIMEOUT);

				byte[]? length = await _ReadAsync(server, 4, timeout.Token).ConfigureAwait(false);
				if(length == null) continue;

				int n = BinaryPrimitives.ReadInt32LittleEndian(length);
				if((n <= 0) || (n > _MAX_MESSAGE_SIZE)) continue;

				byte[]? message = await _ReadAsync(server, n, timeout.Token).ConfigureAwait(false);
				if(message == null) continue;

				e = _Unpack(message);
				if(e == null) continue;

				await server.WriteAsync(new byte[] { _ACK }, timeout.Token).ConfigureAwait(false);
				await server.FlushAsync(timeout.Token).ConfigureAwait(false);
			}
			catch(OperationCanceledException)
			{
				if(token.IsCancellationRequested) { break; }
				continue;
			}
			catch(Exception)
			{
				try { await Task.Delay(100, token).ConfigureAwait(false); } catch(OperationCanceledException) { break; }
				continue;
			}

			try
			{
				_OnArgumentsReceived(e);
			}
			catch(Exception) {}
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IDisposable                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Stops listening and releases the instance, so that another instance can become the first instance.</summary>
	public void Dispose()
	{
		_Dispose(true);
		GC.SuppressFinalize(this);
	}
}
