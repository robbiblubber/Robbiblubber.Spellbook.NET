namespace Robbiblubber.Spellbook.Backpack;



/// <summary>This class provides information on the payload of a packed file.</summary>
public sealed class PayloadInfo
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="carrierLength">Carrier length.</param>
	/// <param name="length">Payload length.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="hash">SHA-256 hash.</param>
	internal PayloadInfo(long carrierLength, long length, string? tag, byte[] hash)
	{
		CarrierLength = carrierLength;
		Length = length;
		Tag = tag;
		Hash = hash;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the length of the carrier in bytes, which is also the offset of the payload in the file.</summary>
	public long CarrierLength
	{
		get;
	}


	/// <summary>Gets the payload length in bytes.</summary>
	public long Length
	{
		get;
	}


	/// <summary>Gets the tag of the payload, or NULL if the payload has no tag.</summary>
	public string? Tag
	{
		get;
	}


	/// <summary>Gets the SHA-256 hash of the payload as stored in the footer.</summary>
	public ReadOnlyMemory<byte> Hash
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string ToString()
	{
		return ($"{Length} bytes at {CarrierLength}" + ((Tag is null) ? "" : $", tag \"{Tag}\""));
	}
}
