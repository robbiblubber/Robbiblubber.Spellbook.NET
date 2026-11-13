namespace Robbiblubber.Spellbook;



/// <summary>This class provides stream extension methods.</summary>
public static class StreamOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads all bytes from the current position to the end of a stream.</summary>
	/// <param name="s">Stream.</param>
	/// <returns>Returns the bytes.</returns>
	public static byte[] ReadAllBytes(this Stream s)
	{
		using MemoryStream rval = new();
		s.CopyTo(rval);

		return rval.ToArray();
	}


	/// <summary>Reads all bytes from the current position to the end of a stream.</summary>
	/// <param name="s">Stream.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Returns the bytes.</returns>
	public static async Task<byte[]> ReadAllBytesAsync(this Stream s, CancellationToken cancellationToken = default)
	{
		using MemoryStream rval = new();
		await s.CopyToAsync(rval, cancellationToken).ConfigureAwait(false);

		return rval.ToArray();
	}
}
