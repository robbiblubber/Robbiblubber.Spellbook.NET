namespace Robbiblubber.Spellbook.TestPlugin;

using Robbiblubber.Spellbook.TestPluginDependency;



/// <summary>This class is a test plug-in that reverses bytes with a method from its own dependency.</summary>
public class ReverseEncoder: EncoderBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EncoderBase                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Encodes data.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the reversed data.</returns>
	public override byte[] Encode(byte[] data)
	{
		return Reverser.Reverse(data);
	}


	/// <summary>Decodes data.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the reversed data.</returns>
	public override byte[] Decode(byte[] data)
	{
		return Reverser.Reverse(data);
	}
}



/// <summary>This class is a test plug-in whose constructor fails.</summary>
public class FailingEncoder: ReverseEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <exception cref="InvalidOperationException">Thrown always.</exception>
	public FailingEncoder()
	{
		throw new InvalidOperationException("Plug-in failed.");
	}
}



/// <summary>This class is an abstract test plug-in base class that must not be instantiated.</summary>
public abstract class AbstractEncoder: ReverseEncoder
{}
