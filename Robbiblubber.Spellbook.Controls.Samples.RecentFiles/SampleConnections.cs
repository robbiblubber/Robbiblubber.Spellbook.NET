namespace Robbiblubber.Spellbook.Controls.Samples.RecentFiles;



/// <summary>This class provides the sample database connections.</summary>
public static class SampleConnections
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Connection names by ID.</summary>
	private static readonly Dictionary<string, string> _Names = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "dev", "Development (localhost)" },
		{ "staging", "Staging" },
		{ "prod", "Production" },
		{ "reporting", "Reporting Replica" }
	};

	/// <summary>IDs of dropped connections.</summary>
	private static readonly HashSet<string> _Dropped = new(StringComparer.OrdinalIgnoreCase);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the IDs of all connections.</summary>
	public static IEnumerable<string> IDs
	{
		get { return _Names.Keys; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the name of a connection.</summary>
	/// <param name="id">Connection ID.</param>
	/// <returns>Returns the name, or NULL if the connection is unknown.</returns>
	public static string? GetName(string id)
	{
		return (_Names.TryGetValue(id, out string? rval) ? rval : null);
	}


	/// <summary>Returns if a connection exists.</summary>
	/// <param name="id">Connection ID.</param>
	/// <returns>Returns <c>true</c> if the connection exists and has not been dropped, otherwise returns <c>false</c>.</returns>
	public static bool Exists(string id)
	{
		return (_Names.ContainsKey(id) && (!_Dropped.Contains(id)));
	}


	/// <summary>Drops or restores a connection.</summary>
	/// <param name="id">Connection ID.</param>
	/// <param name="dropped">Determines if the connection is dropped.</param>
	public static void SetDropped(string id, bool dropped)
	{
		if(dropped)
		{
			_Dropped.Add(id);
		}
		else { _Dropped.Remove(id); }
	}
}
