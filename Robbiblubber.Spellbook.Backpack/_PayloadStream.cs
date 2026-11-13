namespace Robbiblubber.Spellbook.Backpack;



/// <summary>This class implements a read-only stream over the payload section of a file.</summary>
internal sealed class _PayloadStream: Stream
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>File stream.</summary>
	private readonly FileStream _Stream;

	/// <summary>Payload offset in the file.</summary>
	private readonly long _Offset;

	/// <summary>Payload length.</summary>
	private readonly long _Length;

	/// <summary>Current position in the payload.</summary>
	private long _Position = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="stream">File stream; it is disposed with this stream.</param>
	/// <param name="offset">Payload offset.</param>
	/// <param name="length">Payload length.</param>
	internal _PayloadStream(FileStream stream, long offset, long length)
	{
		_Stream = stream;
		_Offset = offset;
		_Length = length;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of bytes to read and positions the file.</summary>
	/// <param name="count">Requested number of bytes.</param>
	/// <returns>Returns the number of bytes that can be read.</returns>
	private int _Prepare(int count)
	{
		ObjectDisposedException.ThrowIf(!_Stream.CanRead, this);

		if(_Position >= _Length) { return 0; }

		_Stream.Position = (_Offset + _Position);
		return (int) Math.Min(count, _Length - _Position);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Stream                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override bool CanRead
	{
		get { return _Stream.CanRead; }
	}


	/// <inheritdoc/>
	public override bool CanSeek
	{
		get { return _Stream.CanSeek; }
	}


	/// <inheritdoc/>
	public override bool CanWrite
	{
		get { return false; }
	}


	/// <inheritdoc/>
	public override long Length
	{
		get { return _Length; }
	}


	/// <inheritdoc/>
	public override long Position
	{
		get { return _Position; }
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value);
			_Position = value;
		}
	}


	/// <inheritdoc/>
	public override int Read(byte[] buffer, int offset, int count)
	{
		ValidateBufferArguments(buffer, offset, count);
		return Read(buffer.AsSpan(offset, count));
	}


	/// <inheritdoc/>
	public override int Read(Span<byte> buffer)
	{
		int n = _Prepare(buffer.Length);
		if(n == 0) { return 0; }

		int rval = _Stream.Read(buffer[..n]);
		_Position += rval;

		return rval;
	}


	/// <inheritdoc/>
	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		ValidateBufferArguments(buffer, offset, count);
		return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
	}


	/// <inheritdoc/>
	public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
	{
		int n = _Prepare(buffer.Length);
		if(n == 0) { return 0; }

		int rval = await _Stream.ReadAsync(buffer[..n], cancellationToken).ConfigureAwait(false);
		_Position += rval;

		return rval;
	}


	/// <inheritdoc/>
	public override long Seek(long offset, SeekOrigin origin)
	{
		long pos = origin switch
		{
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => (_Position + offset),
			SeekOrigin.End => (_Length + offset),
			_ => throw new ArgumentException("Invalid seek origin.", nameof(origin))
		};

		if(pos < 0) { throw new IOException("An attempt was made to move the position before the beginning of the stream."); }
		return (_Position = pos);
	}


	/// <inheritdoc/>
	public override void Flush()
	{}


	/// <inheritdoc/>
	public override void SetLength(long value)
	{
		throw new NotSupportedException("The payload stream is read-only.");
	}


	/// <inheritdoc/>
	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("The payload stream is read-only.");
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _Stream.Dispose(); }
		base.Dispose(disposing);
	}
}
