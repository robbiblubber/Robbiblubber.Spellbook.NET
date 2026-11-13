namespace Robbiblubber.Spellbook.Backpack;

using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;



/// <summary>This class provides methods to append a payload to a carrier file and to read it back.</summary>
/// <remarks><para>A packed file consists of the unchanged carrier, the payload and a footer of 84 bytes at the end of the
///          file. The footer contains the payload length, its SHA-256 hash, an optional tag and a format version. Reading only
///          needs the end of the file, so the size of the carrier does not matter, and carrier and payload may contain any
///          bytes. A file holds at most one payload.</para>
///          <para>Files are read with <see cref="FileShare.ReadWrite"/> and <see cref="FileShare.Delete"/>, so a running
///          executable can read its own payload.</para></remarks>
/// <example><code>
/// Packer.Append("setup.exe", "data.zip", "setup-data");
///
/// using Stream data = Packer.OpenPayload(Environment.ProcessPath!, "setup-data");
/// </code></example>
public static class Packer
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Footer length.</summary>
	private const int _FOOTER_LENGTH = 84;

	/// <summary>Hash length.</summary>
	private const int _HASH_LENGTH = 32;

	/// <summary>Offset of the payload length in the footer.</summary>
	private const int _LENGTH_OFFSET = 32;

	/// <summary>Offset of the tag in the footer.</summary>
	private const int _TAG_OFFSET = 40;

	/// <summary>Maximum tag length in bytes (UTF-8).</summary>
	private const int _TAG_LENGTH = 32;

	/// <summary>Offset of the format version in the footer.</summary>
	private const int _VERSION_OFFSET = 72;

	/// <summary>Offset of the magic in the footer.</summary>
	private const int _MAGIC_OFFSET = 76;

	/// <summary>Format version.</summary>
	private const ushort _VERSION = 1;

	/// <summary>Copy buffer size.</summary>
	private const int _BUFFER_SIZE = 81920;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Footer magic.</summary>
	private static readonly byte[] _MAGIC = "RBBKPACK"u8.ToArray();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Opens a file for reading.</summary>
	/// <param name="file">File name.</param>
	/// <param name="useAsync">Determines if the file is opened for asynchronous access.</param>
	/// <param name="bufferSize">Buffer size; 0 disables buffering.</param>
	/// <returns>Returns a file stream.</returns>
	private static FileStream _OpenRead(string file, bool useAsync, int bufferSize = 0)
	{
		return new(file, FileMode.Open, FileAccess.Read, (FileShare.ReadWrite | FileShare.Delete), bufferSize, useAsync);
	}


	/// <summary>Checks a file name argument.</summary>
	/// <param name="file">File name.</param>
	/// <param name="paramName">Parameter name.</param>
	/// <exception cref="ArgumentException">Thrown when the file name is NULL or empty.</exception>
	private static void _CheckFile(string file, string paramName)
	{
		ArgumentException.ThrowIfNullOrEmpty(file, paramName);
	}


	/// <summary>Checks that two file names refer to different files.</summary>
	/// <param name="a">File name.</param>
	/// <param name="b">File name.</param>
	/// <param name="paramName">Parameter name of the second file.</param>
	/// <exception cref="ArgumentException">Thrown when both names refer to the same file.</exception>
	private static void _CheckDifferent(string a, string b, string paramName)
	{
		StringComparison cmp = (OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

		if(string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), cmp)) { throw new ArgumentException("Both file names refer to the same file.", paramName); }
	}


	/// <summary>Normalizes a tag.</summary>
	/// <param name="tag">Tag.</param>
	/// <returns>Returns the tag, or NULL for an empty tag.</returns>
	private static string? _NormalizeTag(string? tag)
	{
		return (string.IsNullOrEmpty(tag) ? null : tag);
	}


	/// <summary>Encodes a tag for the footer.</summary>
	/// <param name="tag">Tag.</param>
	/// <returns>Returns the encoded tag (32 bytes, zero padded).</returns>
	/// <exception cref="ArgumentException">Thrown when the tag is too long or contains a null character.</exception>
	private static byte[] _EncodeTag(string? tag)
	{
		byte[] rval = new byte[_TAG_LENGTH];
		tag = _NormalizeTag(tag);

		if(tag is not null)
		{
			if(tag.Contains('\0')) { throw new ArgumentException("The tag must not contain null characters.", nameof(tag)); }
			if(Encoding.UTF8.GetByteCount(tag) > _TAG_LENGTH) { throw new ArgumentException($"The tag must not exceed {_TAG_LENGTH} bytes (UTF-8).", nameof(tag)); }

			Encoding.UTF8.GetBytes(tag, rval);
		}

		return rval;
	}


	/// <summary>Decodes a tag from the footer.</summary>
	/// <param name="data">Encoded tag.</param>
	/// <returns>Returns the tag, or NULL if the payload has no tag.</returns>
	private static string? _DecodeTag(ReadOnlySpan<byte> data)
	{
		int n = data.IndexOf((byte) 0);
		if(n >= 0) { data = data[..n]; }

		return ((data.Length == 0) ? null : Encoding.UTF8.GetString(data));
	}


	/// <summary>Creates a footer.</summary>
	/// <param name="hash">Payload hash.</param>
	/// <param name="length">Payload length.</param>
	/// <param name="tag">Encoded tag.</param>
	/// <returns>Returns the footer.</returns>
	private static byte[] _CreateFooter(byte[] hash, long length, byte[] tag)
	{
		byte[] rval = new byte[_FOOTER_LENGTH];

		hash.CopyTo(rval, 0);
		BinaryPrimitives.WriteInt64LittleEndian(rval.AsSpan(_LENGTH_OFFSET), length);
		tag.CopyTo(rval, _TAG_OFFSET);
		BinaryPrimitives.WriteUInt16LittleEndian(rval.AsSpan(_VERSION_OFFSET), _VERSION);
		_MAGIC.CopyTo(rval, _MAGIC_OFFSET);

		return rval;
	}


	/// <summary>Reads the footer of a file.</summary>
	/// <param name="stream">File stream.</param>
	/// <returns>Returns the payload information, or NULL if the file has no payload.</returns>
	/// <exception cref="InvalidDataException">Thrown when the footer is damaged or has an unsupported version.</exception>
	private static PayloadInfo? _ReadFooter(FileStream stream)
	{
		long flen = stream.Length;
		if(flen < _FOOTER_LENGTH) { return null; }

		byte[] buf = new byte[_FOOTER_LENGTH];
		stream.Seek(-_FOOTER_LENGTH, SeekOrigin.End);
		stream.ReadExactly(buf);

		if(!buf.AsSpan(_MAGIC_OFFSET).SequenceEqual(_MAGIC)) { return null; }

		ushort version = BinaryPrimitives.ReadUInt16LittleEndian(buf.AsSpan(_VERSION_OFFSET));
		if(version != _VERSION) { throw new InvalidDataException($"Unsupported payload format version {version}."); }

		long len = BinaryPrimitives.ReadInt64LittleEndian(buf.AsSpan(_LENGTH_OFFSET));
		if((len < 0) || (len > (flen - _FOOTER_LENGTH))) { throw new InvalidDataException("The payload footer is damaged."); }

		return new(flen - _FOOTER_LENGTH - len, len, _DecodeTag(buf.AsSpan(_TAG_OFFSET, _TAG_LENGTH)), buf[.._HASH_LENGTH]);
	}


	/// <summary>Returns if payload information matches a tag.</summary>
	/// <param name="info">Payload information.</param>
	/// <param name="tag">Tag; NULL matches any payload.</param>
	/// <returns>Returns TRUE if there is a payload with a matching tag, otherwise returns FALSE.</returns>
	private static bool _Matches(PayloadInfo? info, string? tag)
	{
		if(info is null) { return false; }

		tag = _NormalizeTag(tag);
		return ((tag is null) || (info.Tag == tag));
	}


	/// <summary>Creates the exception for a missing payload.</summary>
	/// <param name="tag">Tag.</param>
	/// <returns>Returns the exception.</returns>
	private static InvalidDataException _NoPayload(string? tag)
	{
		tag = _NormalizeTag(tag);
		return new(((tag is null) ? "The file does not contain a payload." : $"The file does not contain a payload with the tag \"{tag}\"."));
	}


	/// <summary>Copies data from a stream to a stream.</summary>
	/// <param name="source">Source stream.</param>
	/// <param name="target">Target stream.</param>
	/// <param name="count">Number of bytes to copy; -1 copies to the end of the source.</param>
	/// <param name="hash">Hash that is computed over the copied data, or NULL.</param>
	/// <param name="useAsync">Determines if the streams are accessed asynchronously.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns the number of bytes copied.</returns>
	/// <exception cref="EndOfStreamException">Thrown when the source ends before the given number of bytes is copied.</exception>
	private static async ValueTask<long> _CopyAsync(Stream source, Stream target, long count, IncrementalHash? hash, bool useAsync, CancellationToken cancellationToken)
	{
		byte[] buf = ArrayPool<byte>.Shared.Rent(_BUFFER_SIZE);
		long rval = 0;

		try
		{
			while((count < 0) || (rval < count))
			{
				cancellationToken.ThrowIfCancellationRequested();

				int n = ((count < 0) ? buf.Length : (int) Math.Min(buf.Length, count - rval));
				int r = (useAsync ? await source.ReadAsync(buf.AsMemory(0, n), cancellationToken).ConfigureAwait(false) : source.Read(buf, 0, n));

				if(r == 0)
				{
					if(count >= 0) { throw new EndOfStreamException("The payload is incomplete."); }
					break;
				}

				hash?.AppendData(buf, 0, r);
				if(useAsync) { await target.WriteAsync(buf.AsMemory(0, r), cancellationToken).ConfigureAwait(false); } else { target.Write(buf, 0, r); }
				rval += r;
			}
		}
		finally { ArrayPool<byte>.Shared.Return(buf); }

		return rval;
	}


	/// <summary>Appends a payload to a carrier file.</summary>
	/// <param name="carrierFile">Carrier file name.</param>
	/// <param name="payload">Payload stream, read from its current position to the end.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="useAsync">Determines if the files are accessed asynchronously.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	private static async ValueTask _AppendAsync(string carrierFile, Stream payload, string? tag, bool useAsync, CancellationToken cancellationToken)
	{
		if(!payload.CanRead) { throw new ArgumentException("The payload stream is not readable.", nameof(payload)); }
		byte[] etag = _EncodeTag(tag);

		using FileStream wr = new(carrierFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 0, useAsync);
		if(_ReadFooter(wr) is not null) { throw new InvalidOperationException("The carrier file already contains a payload."); }

		long len = wr.Length;
		try
		{
			using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

			wr.Seek(0, SeekOrigin.End);
			long cnt = await _CopyAsync(payload, wr, -1, hash, useAsync, cancellationToken).ConfigureAwait(false);
			byte[] footer = _CreateFooter(hash.GetHashAndReset(), cnt, etag);

			if(useAsync)
			{
				await wr.WriteAsync(footer, cancellationToken).ConfigureAwait(false);
				await wr.FlushAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				wr.Write(footer);
				wr.Flush();
			}
		}
		catch
		{
			wr.SetLength(len);
			throw;
		}
	}


	/// <summary>Appends a payload file to a carrier file.</summary>
	/// <param name="carrierFile">Carrier file name.</param>
	/// <param name="payloadFile">Payload file name.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="useAsync">Determines if the files are accessed asynchronously.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	private static async ValueTask _AppendFileAsync(string carrierFile, string payloadFile, string? tag, bool useAsync, CancellationToken cancellationToken)
	{
		_CheckFile(carrierFile, nameof(carrierFile));
		_CheckFile(payloadFile, nameof(payloadFile));
		_CheckDifferent(carrierFile, payloadFile, nameof(payloadFile));

		using FileStream re = _OpenRead(payloadFile, useAsync);
		await _AppendAsync(carrierFile, re, tag, useAsync, cancellationToken).ConfigureAwait(false);
	}


	/// <summary>Extracts a payload to a stream.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="target">Target stream.</param>
	/// <param name="tag">Tag; NULL accepts any payload.</param>
	/// <param name="throwIfMissing">Determines if an exception is thrown when there is no matching payload.</param>
	/// <param name="useAsync">Determines if the files are accessed asynchronously.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns TRUE if the payload has been extracted, FALSE if there is no matching payload.</returns>
	/// <exception cref="InvalidDataException">Thrown when the payload is damaged, or when it is missing and
	///                                        <paramref name="throwIfMissing"/> is TRUE.</exception>
	private static async ValueTask<bool> _ExtractAsync(string sourceFile, Stream target, string? tag, bool throwIfMissing, bool useAsync, CancellationToken cancellationToken)
	{
		_CheckFile(sourceFile, nameof(sourceFile));
		ArgumentNullException.ThrowIfNull(target);

		using FileStream re = _OpenRead(sourceFile, useAsync);
		PayloadInfo? info = _ReadFooter(re);

		if(!_Matches(info, tag))
		{
			if(throwIfMissing) { throw _NoPayload(tag); }
			return false;
		}

		using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

		re.Seek(info!.CarrierLength, SeekOrigin.Begin);
		await _CopyAsync(re, target, info.Length, hash, useAsync, cancellationToken).ConfigureAwait(false);

		if(!hash.GetHashAndReset().AsSpan().SequenceEqual(info.Hash.Span)) { throw new InvalidDataException("The payload is damaged (checksum mismatch)."); }
		return true;
	}


	/// <summary>Extracts a payload to a file.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="targetFile">Target file name.</param>
	/// <param name="tag">Tag; NULL accepts any payload.</param>
	/// <param name="throwIfMissing">Determines if an exception is thrown when there is no matching payload.</param>
	/// <param name="useAsync">Determines if the files are accessed asynchronously.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns TRUE if the payload has been extracted, FALSE if there is no matching payload.</returns>
	/// <remarks>The payload is written to a temporary file that replaces the target only after it has been verified.</remarks>
	private static async ValueTask<bool> _ExtractFileAsync(string sourceFile, string targetFile, string? tag, bool throwIfMissing, bool useAsync, CancellationToken cancellationToken)
	{
		_CheckFile(sourceFile, nameof(sourceFile));
		_CheckFile(targetFile, nameof(targetFile));
		_CheckDifferent(sourceFile, targetFile, nameof(targetFile));

		string target = Path.GetFullPath(targetFile);
		string tmp = Path.Combine(Path.GetDirectoryName(target)!, "~" + Path.GetRandomFileName());
		bool rval;

		try
		{
			using(FileStream wr = new(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 0, useAsync))
			{
				rval = await _ExtractAsync(sourceFile, wr, tag, throwIfMissing, useAsync, cancellationToken).ConfigureAwait(false);
			}

			if(rval) { File.Move(tmp, target, true); }
		}
		finally
		{
			if(File.Exists(tmp)) { File.Delete(tmp); }
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Appends a payload file to a carrier file.</summary>
	/// <param name="carrierFile">Carrier file name (file to append to).</param>
	/// <param name="payloadFile">Payload file name (file to append).</param>
	/// <param name="tag">Optional tag (at most 32 bytes UTF-8) that identifies the payload.</param>
	/// <exception cref="FileNotFoundException">Thrown when the carrier or the payload file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	/// <exception cref="ArgumentException">Thrown when both names refer to the same file or the tag is invalid.</exception>
	/// <remarks>If appending fails, the carrier file is restored to its original length.</remarks>
	public static void Append(string carrierFile, string payloadFile, string? tag = null)
	{
		_AppendFileAsync(carrierFile, payloadFile, tag, false, default).GetAwaiter().GetResult();
	}


	/// <summary>Appends a payload to a carrier file.</summary>
	/// <param name="carrierFile">Carrier file name (file to append to).</param>
	/// <param name="payload">Payload stream, read from its current position to the end.</param>
	/// <param name="tag">Optional tag (at most 32 bytes UTF-8) that identifies the payload.</param>
	/// <exception cref="FileNotFoundException">Thrown when the carrier file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	/// <exception cref="ArgumentException">Thrown when the stream is not readable or the tag is invalid.</exception>
	/// <remarks>If appending fails, the carrier file is restored to its original length.</remarks>
	public static void Append(string carrierFile, Stream payload, string? tag = null)
	{
		_CheckFile(carrierFile, nameof(carrierFile));
		ArgumentNullException.ThrowIfNull(payload);

		_AppendAsync(carrierFile, payload, tag, false, default).GetAwaiter().GetResult();
	}


	/// <summary>Appends a payload to a carrier file.</summary>
	/// <param name="carrierFile">Carrier file name (file to append to).</param>
	/// <param name="payload">Payload data.</param>
	/// <param name="tag">Optional tag (at most 32 bytes UTF-8) that identifies the payload.</param>
	/// <exception cref="FileNotFoundException">Thrown when the carrier file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	/// <exception cref="ArgumentException">Thrown when the tag is invalid.</exception>
	public static void Append(string carrierFile, byte[] payload, string? tag = null)
	{
		ArgumentNullException.ThrowIfNull(payload);
		Append(carrierFile, new MemoryStream(payload, false), tag);
	}


	/// <summary>Appends a payload file to a carrier file asynchronously.</summary>
	/// <param name="carrierFile">Carrier file name (file to append to).</param>
	/// <param name="payloadFile">Payload file name (file to append).</param>
	/// <param name="tag">Optional tag (at most 32 bytes UTF-8) that identifies the payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns a task that represents the operation.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the carrier or the payload file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	/// <exception cref="ArgumentException">Thrown when both names refer to the same file or the tag is invalid.</exception>
	/// <remarks>If appending fails or is cancelled, the carrier file is restored to its original length.</remarks>
	public static async Task AppendAsync(string carrierFile, string payloadFile, string? tag = null, CancellationToken cancellationToken = default)
	{
		await _AppendFileAsync(carrierFile, payloadFile, tag, true, cancellationToken).ConfigureAwait(false);
	}


	/// <summary>Appends a payload to a carrier file asynchronously.</summary>
	/// <param name="carrierFile">Carrier file name (file to append to).</param>
	/// <param name="payload">Payload stream, read from its current position to the end.</param>
	/// <param name="tag">Optional tag (at most 32 bytes UTF-8) that identifies the payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns a task that represents the operation.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the carrier file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the carrier file already contains a payload.</exception>
	/// <exception cref="ArgumentException">Thrown when the stream is not readable or the tag is invalid.</exception>
	/// <remarks>If appending fails or is cancelled, the carrier file is restored to its original length.</remarks>
	public static async Task AppendAsync(string carrierFile, Stream payload, string? tag = null, CancellationToken cancellationToken = default)
	{
		_CheckFile(carrierFile, nameof(carrierFile));
		ArgumentNullException.ThrowIfNull(payload);

		await _AppendAsync(carrierFile, payload, tag, true, cancellationToken).ConfigureAwait(false);
	}


	/// <summary>Returns if a file contains a payload.</summary>
	/// <param name="file">File name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns TRUE if the file contains a matching payload, otherwise returns FALSE.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the payload footer is damaged.</exception>
	/// <remarks>This method only reads the footer; use <see cref="Verify(string, string?)"/> to check the payload data.</remarks>
	public static bool HasPayload(string file, string? tag = null)
	{
		return _Matches(GetInfo(file), tag);
	}


	/// <summary>Gets information on the payload of a file.</summary>
	/// <param name="file">File name.</param>
	/// <returns>Returns the payload information, or NULL if the file does not contain a payload.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the payload footer is damaged.</exception>
	public static PayloadInfo? GetInfo(string file)
	{
		_CheckFile(file, nameof(file));

		using FileStream re = _OpenRead(file, false);
		return _ReadFooter(re);
	}


	/// <summary>Verifies the payload of a file.</summary>
	/// <param name="file">File name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns TRUE if the file contains a matching and intact payload, otherwise returns FALSE.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	public static bool Verify(string file, string? tag = null)
	{
		try
		{
			return _ExtractAsync(file, Stream.Null, tag, false, false, default).GetAwaiter().GetResult();
		}
		catch(InvalidDataException) { return false; }
		catch(EndOfStreamException) { return false; }
	}


	/// <summary>Verifies the payload of a file asynchronously.</summary>
	/// <param name="file">File name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns TRUE if the file contains a matching and intact payload, otherwise returns FALSE.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	public static async Task<bool> VerifyAsync(string file, string? tag = null, CancellationToken cancellationToken = default)
	{
		try
		{
			return await _ExtractAsync(file, Stream.Null, tag, false, true, cancellationToken).ConfigureAwait(false);
		}
		catch(InvalidDataException) { return false; }
		catch(EndOfStreamException) { return false; }
	}


	/// <summary>Extracts the payload of a file to a file.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="targetFile">Target file name; an existing file is replaced.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	/// <remarks>The target file is only replaced when the payload is intact.</remarks>
	public static void Extract(string sourceFile, string targetFile, string? tag = null)
	{
		_ExtractFileAsync(sourceFile, targetFile, tag, true, false, default).GetAwaiter().GetResult();
	}


	/// <summary>Extracts the payload of a file to a stream.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="target">Target stream.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	/// <remarks>The data is written to the stream before it is verified; when the payload is damaged, the stream contains
	///          the damaged data.</remarks>
	public static void Extract(string sourceFile, Stream target, string? tag = null)
	{
		_ExtractAsync(sourceFile, target, tag, true, false, default).GetAwaiter().GetResult();
	}


	/// <summary>Extracts the payload of a file to a file, if there is one.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="targetFile">Target file name; an existing file is replaced.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns TRUE if the payload has been extracted, FALSE if the file does not contain a matching payload.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the payload is damaged.</exception>
	public static bool TryExtract(string sourceFile, string targetFile, string? tag = null)
	{
		return _ExtractFileAsync(sourceFile, targetFile, tag, false, false, default).GetAwaiter().GetResult();
	}


	/// <summary>Extracts the payload of a file to a file asynchronously.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="targetFile">Target file name; an existing file is replaced.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns a task that represents the operation.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	/// <remarks>The target file is only replaced when the payload is intact.</remarks>
	public static async Task ExtractAsync(string sourceFile, string targetFile, string? tag = null, CancellationToken cancellationToken = default)
	{
		await _ExtractFileAsync(sourceFile, targetFile, tag, true, true, cancellationToken).ConfigureAwait(false);
	}


	/// <summary>Extracts the payload of a file to a stream asynchronously.</summary>
	/// <param name="sourceFile">Packed file name.</param>
	/// <param name="target">Target stream.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns a task that represents the operation.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	/// <remarks>The data is written to the stream before it is verified; when the payload is damaged, the stream contains
	///          the damaged data.</remarks>
	public static async Task ExtractAsync(string sourceFile, Stream target, string? tag = null, CancellationToken cancellationToken = default)
	{
		await _ExtractAsync(sourceFile, target, tag, true, true, cancellationToken).ConfigureAwait(false);
	}


	/// <summary>Reads the payload of a file.</summary>
	/// <param name="file">Packed file name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns the payload data.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	public static byte[] ReadPayload(string file, string? tag = null)
	{
		MemoryStream rval = new();
		_ExtractAsync(file, rval, tag, true, false, default).GetAwaiter().GetResult();

		return rval.ToArray();
	}


	/// <summary>Reads the payload of a file asynchronously.</summary>
	/// <param name="file">Packed file name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns the payload data.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload or the payload is damaged.</exception>
	public static async Task<byte[]> ReadPayloadAsync(string file, string? tag = null, CancellationToken cancellationToken = default)
	{
		MemoryStream rval = new();
		await _ExtractAsync(file, rval, tag, true, true, cancellationToken).ConfigureAwait(false);

		return rval.ToArray();
	}


	/// <summary>Opens the payload of a file as a read-only stream.</summary>
	/// <param name="file">Packed file name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns a seekable, read-only stream over the payload. The stream must be disposed.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the file does not contain a matching payload.</exception>
	/// <remarks>The payload is not verified; call <see cref="Verify(string, string?)"/> first if that is required.</remarks>
	public static Stream OpenPayload(string file, string? tag = null)
	{
		_CheckFile(file, nameof(file));

		FileStream re = _OpenRead(file, false, 4096);
		try
		{
			PayloadInfo? info = _ReadFooter(re);
			if(!_Matches(info, tag)) { throw _NoPayload(tag); }

			return new _PayloadStream(re, info!.CarrierLength, info.Length);
		}
		catch
		{
			re.Dispose();
			throw;
		}
	}


	/// <summary>Removes the payload from a file, restoring the original carrier.</summary>
	/// <param name="file">Packed file name.</param>
	/// <param name="tag">Tag the payload must have; NULL accepts any payload.</param>
	/// <returns>Returns TRUE if a payload has been removed, FALSE if the file does not contain a matching payload.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the payload footer is damaged.</exception>
	public static bool Remove(string file, string? tag = null)
	{
		_CheckFile(file, nameof(file));

		using FileStream fs = new(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
		PayloadInfo? info = _ReadFooter(fs);
		if(!_Matches(info, tag)) { return false; }

		fs.SetLength(info!.CarrierLength);
		return true;
	}
}
