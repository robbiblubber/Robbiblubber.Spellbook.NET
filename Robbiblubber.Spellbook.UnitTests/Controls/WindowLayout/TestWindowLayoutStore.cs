namespace Robbiblubber.Spellbook.UnitTests.Controls.WindowLayout;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a window layout store that keeps the values in memory.</summary>
public class TestWindowLayoutStore: IWindowLayoutStore
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Values written since the last commit.</summary>
	private readonly Dictionary<(string Layout, string Key), int[]> _Pending = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the committed values.</summary>
	public Dictionary<(string Layout, string Key), int[]> Values
	{
		get;
	} = new();


	/// <summary>Gets the number of commits.</summary>
	public int Commits
	{
		get; private set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IWindowLayoutStore                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public int[]? Read(string layout, string key)
	{
		return (Values.TryGetValue((layout, key), out int[]? rval) ? rval : null);
	}


	/// <inheritdoc/>
	public void Write(string layout, string key, int[] value)
	{
		_Pending[(layout, key)] = value;
	}


	/// <inheritdoc/>
	public void Commit()
	{
		foreach(KeyValuePair<(string Layout, string Key), int[]> i in _Pending) { Values[i.Key] = i.Value; }
		_Pending.Clear();
		Commits++;
	}
}
