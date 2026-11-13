namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that implement this interface store window layouts for a <see cref="WindowLayoutProvider"/>.</summary>
/// <remarks><para>A layout is identified by its name; it consists of values that are identified by keys. All values are integer
///          arrays. Names and keys consist of letters, digits, underscores and hyphens.</para>
///          <para><see cref="Write"/> may buffer the values; <see cref="Commit"/> makes them persistent. The default implementation is
///          <see cref="DdnWindowLayoutStore"/>.</para></remarks>
[SupportedOSPlatform("windows")]
public interface IWindowLayoutStore
{
	/// <summary>Reads a value.</summary>
	/// <param name="layout">Layout name.</param>
	/// <param name="key">Key.</param>
	/// <returns>Returns the value, or NULL if the store does not contain a value for the key.</returns>
	public int[]? Read(string layout, string key);


	/// <summary>Writes a value.</summary>
	/// <param name="layout">Layout name.</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	public void Write(string layout, string key, int[] value);


	/// <summary>Makes the written values persistent.</summary>
	public void Commit();
}
