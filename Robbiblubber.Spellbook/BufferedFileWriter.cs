namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class provides a thread-safe buffered file writer. Text is collected in memory and appended to the file in
///          regular intervals while the writer is running.</summary>
/// <remarks><para>Nothing written is lost: <see cref="Stop"/>, <see cref="Flush"/> and <see cref="Dispose"/> write all pending
///          text. Text written while the writer is stopped is written on the next flush, start or dispose.</para>
///          <para>When writing fails in the background, the text stays pending and is retried in the next interval; the
///          <see cref="WriteError"/> event reports the error. <see cref="Flush"/> and <see cref="FlushAsync"/> throw the error
///          instead.</para></remarks>
public sealed class BufferedFileWriter: IDisposable, IAsyncDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Pending text.</summary>
	private readonly StringBuilder _Pending = new();

	/// <summary>Lock for the pending text.</summary>
	private readonly object _PendingLock = new();

	/// <summary>Lock for the running state.</summary>
	private readonly object _StateLock = new();

	/// <summary>Serializes file writes.</summary>
	private readonly SemaphoreSlim _WriteLock = new(1, 1);

	/// <summary>File name.</summary>
	private string _FileName;

	/// <summary>Write interval in milliseconds.</summary>
	private int _Interval;

	/// <summary>Text encoding.</summary>
	private Encoding _Encoding = new UTF8Encoding(false);

	/// <summary>Cancellation source of the write loop.</summary>
	private CancellationTokenSource? _Cancel = null;

	/// <summary>Write loop task.</summary>
	private Task? _Task = null;

	/// <summary>Disposed flag.</summary>
	private bool _Disposed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="interval">Write interval in milliseconds.</param>
	/// <exception cref="ArgumentException">Thrown when the file name is empty.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the interval is less than 1.</exception>
	public BufferedFileWriter(string fileName, int interval = 250)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
		ArgumentOutOfRangeException.ThrowIfLessThan(interval, 1);

		_FileName = fileName;
		_Interval = interval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when pending text could not be written in the background; the text stays pending.</summary>
	/// <remarks>The event is raised on a background thread. Handlers that update the user interface should use
	///          <c>BeginInvoke</c>, not <c>Invoke</c>: <see cref="Stop"/> and <see cref="Dispose"/> wait for the background
	///          loop, so a handler that waits for a UI thread calling them would deadlock.</remarks>
	public event ErrorEventHandler? WriteError;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the file name. Pending text is written to the file that is set when it is flushed.</summary>
	/// <exception cref="ArgumentException">Thrown when the file name is empty.</exception>
	public string FileName
	{
		get { return _FileName; }
		set
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(value);
			_FileName = value;
		}
	}


	/// <summary>Gets or sets the write interval in milliseconds; changes take effect after the current interval.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the interval is less than 1.</exception>
	public int Interval
	{
		get { return Volatile.Read(ref _Interval); }
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
			Volatile.Write(ref _Interval, value);
		}
	}


	/// <summary>Gets or sets the line terminator used by <see cref="WriteLine"/>; the default is
	///          <see cref="Environment.NewLine"/>.</summary>
	public string NewLine
	{
		get; set;
	} = Environment.NewLine;


	/// <summary>Gets or sets the text encoding; the default is UTF-8 without byte order mark.</summary>
	public Encoding Encoding
	{
		get { return _Encoding; }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			_Encoding = value;
		}
	}


	/// <summary>Gets or sets if the writer is running.</summary>
	public bool Running
	{
		get
		{
			lock(_StateLock) { return (_Task is not null); }
		}
		set
		{
			if(value) { Start(); } else { Stop(); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Throws an exception if the writer has been disposed.</summary>
	/// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
	private void _CheckDisposed()
	{
		ObjectDisposedException.ThrowIf(_Disposed, this);
	}


	/// <summary>Takes the pending text.</summary>
	/// <returns>Returns the pending text; an empty string if there is none.</returns>
	private string _Take()
	{
		lock(_PendingLock)
		{
			string rval = _Pending.ToString();
			_Pending.Clear();
			return rval;
		}
	}


	/// <summary>Puts text that could not be written back in front of the pending text.</summary>
	/// <param name="text">Text.</param>
	private void _Restore(string text)
	{
		lock(_PendingLock) { _Pending.Insert(0, text); }
	}


	/// <summary>Writes the pending text.</summary>
	/// <param name="throwException">Determines if a write error is thrown; otherwise <see cref="WriteError"/> is raised.</param>
	private void _Flush(bool throwException)
	{
		_WriteLock.Wait();
		try
		{
			string text = _Take();
			if(text.Length == 0) { return; }

			try
			{
				File.AppendAllText(_FileName, text, _Encoding);
			}
			catch(Exception ex)
			{
				_Restore(text);
				if(throwException) { throw; }
				WriteError?.Invoke(this, new ErrorEventArgs(ex));
			}
		}
		finally { _WriteLock.Release(); }
	}


	/// <summary>Writes the pending text asynchronously.</summary>
	/// <param name="throwException">Determines if a write error is thrown; otherwise <see cref="WriteError"/> is raised.</param>
	/// <returns>Returns a task that completes when the text has been written.</returns>
	private async Task _FlushAsync(bool throwException)
	{
		await _WriteLock.WaitAsync().ConfigureAwait(false);
		try
		{
			string text = _Take();
			if(text.Length == 0) { return; }

			try
			{
				await File.AppendAllTextAsync(_FileName, text, _Encoding).ConfigureAwait(false);
			}
			catch(Exception ex)
			{
				_Restore(text);
				if(throwException) { throw; }
				WriteError?.Invoke(this, new ErrorEventArgs(ex));
			}
		}
		finally { _WriteLock.Release(); }
	}


	/// <summary>Write loop.</summary>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Returns a task that completes when the loop has been cancelled.</returns>
	private async Task _RunAsync(CancellationToken token)
	{
		while(!token.IsCancellationRequested)
		{
			try
			{
				await Task.Delay(Interval, token).ConfigureAwait(false);
			}
			catch(OperationCanceledException) { break; }

			await _FlushAsync(false).ConfigureAwait(false);
		}
	}


	/// <summary>Stops the write loop.</summary>
	/// <returns>Returns the loop task; NULL if the writer was not running.</returns>
	private Task? _StopLoop()
	{
		lock(_StateLock)
		{
			Task? rval = _Task;
			_Cancel?.Cancel();
			_Cancel?.Dispose();
			_Cancel = null;
			_Task = null;
			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Starts the writer and writes text that is pending.</summary>
	/// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
	public void Start()
	{
		lock(_StateLock)
		{
			_CheckDisposed();
			if(_Task is not null) { return; }

			_Cancel = new CancellationTokenSource();
			CancellationToken token = _Cancel.Token;
			_Task = Task.Run(async () =>
			{
				await _FlushAsync(false).ConfigureAwait(false);
				await _RunAsync(token).ConfigureAwait(false);
			});
		}
	}


	/// <summary>Stops the writer and writes all pending text. Write errors raise <see cref="WriteError"/>.</summary>
	public void Stop()
	{
		_StopLoop()?.Wait();
		_Flush(false);
	}


	/// <summary>Writes a string.</summary>
	/// <param name="s">String; NULL is ignored.</param>
	/// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
	public void Write(string? s)
	{
		_CheckDisposed();
		if(string.IsNullOrEmpty(s)) { return; }

		lock(_PendingLock) { _Pending.Append(s); }
	}


	/// <summary>Writes a string followed by <see cref="NewLine"/>.</summary>
	/// <param name="s">String; NULL writes an empty line.</param>
	/// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
	public void WriteLine(string? s = null)
	{
		_CheckDisposed();

		lock(_PendingLock) { _Pending.Append(s).Append(NewLine); }
	}


	/// <summary>Writes all pending text to the file now.</summary>
	/// <exception cref="IOException">Thrown when the file cannot be written; the text stays pending.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown when the file cannot be accessed; the text stays pending.</exception>
	public void Flush()
	{
		_Flush(true);
	}


	/// <summary>Writes all pending text to the file now.</summary>
	/// <returns>Returns a task that completes when the text has been written.</returns>
	/// <exception cref="IOException">Thrown when the file cannot be written; the text stays pending.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown when the file cannot be accessed; the text stays pending.</exception>
	public Task FlushAsync()
	{
		return _FlushAsync(true);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IDisposable                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Stops the writer and writes all pending text.</summary>
	public void Dispose()
	{
		lock(_StateLock)
		{
			if(_Disposed) { return; }
			_Disposed = true;
		}

		Stop();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IAsyncDisposable                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Stops the writer and writes all pending text.</summary>
	/// <returns>Returns a task that completes when the writer has been disposed.</returns>
	public async ValueTask DisposeAsync()
	{
		lock(_StateLock)
		{
			if(_Disposed) { return; }
			_Disposed = true;
		}

		Task? task = _StopLoop();
		if(task is not null) { await task.ConfigureAwait(false); }
		await _FlushAsync(false).ConfigureAwait(false);
	}
}
