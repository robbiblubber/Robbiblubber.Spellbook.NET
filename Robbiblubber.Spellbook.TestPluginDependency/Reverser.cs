namespace Robbiblubber.Spellbook.TestPluginDependency;



/// <summary>This class is a dependency of the test plug-in; the application does not reference it.</summary>
public static class Reverser
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a reversed copy of a byte array.</summary>
	/// <param name="data">Data.</param>
	/// <returns>Returns the reversed data.</returns>
	public static byte[] Reverse(byte[] data)
	{
		byte[] rval = (byte[]) data.Clone();
		Array.Reverse(rval);

		return rval;
	}
}
