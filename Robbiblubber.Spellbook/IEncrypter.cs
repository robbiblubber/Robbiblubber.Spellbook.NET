namespace Robbiblubber.Spellbook;



/// <summary>Classes that encrypt data implement this interface.</summary>
/// <remarks>As an <see cref="IEncoder"/>, an encrypter encrypts data and writes it as text with its <see cref="Encoder"/>, so
///          it can be used wherever an encoder is accepted, for example to store ddn files.</remarks>
public interface IEncrypter: IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the encoder that converts encrypted data to text.</summary>
	public IEncoder Encoder
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Encrypts data.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the encrypted data.</returns>
	public byte[] Encrypt(byte[] data);


	/// <summary>Decrypts data.</summary>
	/// <param name="data">Encrypted data.</param>
	/// <returns>Returns the decrypted data.</returns>
	/// <exception cref="System.Security.Cryptography.CryptographicException">Thrown when the data is invalid or cannot be
	///                                                                        decrypted with this instance.</exception>
	public byte[] Decrypt(byte[] data);
}
