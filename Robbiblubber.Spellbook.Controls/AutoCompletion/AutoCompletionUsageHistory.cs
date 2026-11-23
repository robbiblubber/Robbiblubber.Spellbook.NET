namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;
using System.Text;



/// <summary>This class records how often auto completion items have been accepted.</summary>
/// <remarks>Counts are kept per scope and per item key. The provider uses the <see cref="Control.Name"/> of the control as scope,
///          so controls without a name share one scope, and renaming a control in the designer starts a new history; as item key
///          it uses the insert text or the text of the item. The history is kept in memory; <see cref="Export"/> and
///          <see cref="Import"/> allow the application to persist it. One instance can be shared by several providers. The class
///          is not thread-safe and is meant to be used on the UI thread.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionUsageHistory
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Field separator of the export format.</summary>
	private const char _SEPARATOR = '\t';



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Usage counts by scope and key.</summary>
	private readonly Dictionary<string, Dictionary<string, int>> _Scopes = new(StringComparer.Ordinal);

	/// <summary>Maximum number of entries per scope.</summary>
	private int _MaximumEntries = 1000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public AutoCompletionUsageHistory()
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Escapes a value for the export format.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the escaped value.</returns>
	private static string _Escape(string value)
	{
		return value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
	}


	/// <summary>Unescapes a value of the export format.</summary>
	/// <param name="value">Escaped value.</param>
	/// <returns>Returns the value.</returns>
	private static string _Unescape(string value)
	{
		StringBuilder rval = new(value.Length);

		for(int i = 0; i < value.Length; i++)
		{
			if((value[i] != '\\') || (i == (value.Length - 1)))
			{
				rval.Append(value[i]);
				continue;
			}

			i++;
			rval.Append(value[i] switch { 't' => '\t', 'r' => '\r', 'n' => '\n', _ => value[i] });
		}

		return rval.ToString();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the least used entries of a scope until it does not exceed the maximum number of entries.</summary>
	/// <param name="counts">Scope counts.</param>
	/// <param name="keep">Key that must not be removed, NULL if all entries may be removed.</param>
	/// <returns>Returns TRUE if entries have been removed, otherwise returns FALSE.</returns>
	/// <remarks>The key that has just been used is kept, otherwise a new item could never enter a full history whose entries have all
	///          been used more than once.</remarks>
	private bool _Trim(Dictionary<string, int> counts, string? keep = null)
	{
		bool rval = false;

		while(counts.Count > _MaximumEntries)
		{
			counts.Remove(counts.Where(m => !string.Equals(m.Key, keep, StringComparison.Ordinal)).MinBy(m => m.Value).Key);
			rval = true;
		}

		return rval;
	}


	/// <summary>Gets the counts of a scope.</summary>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns the counts, or NULL if the scope has no entries.</returns>
	internal Dictionary<string, int>? _GetScope(string scope)
	{
		return (_Scopes.TryGetValue(scope, out Dictionary<string, int>? rval) ? rval : null);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the history has changed.</summary>
	public event EventHandler? Changed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the maximum number of entries per scope.</summary>
	/// <remarks>When a scope exceeds the maximum, the least used entries are removed and <see cref="Changed"/> is raised.</remarks>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
	public int MaximumEntries
	{
		get { return _MaximumEntries; }
		set
		{
			if(value < 1) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be at least 1."); }

			bool changed = false;

			_MaximumEntries = value;
			foreach(Dictionary<string, int> i in _Scopes.Values) { changed |= _Trim(i); }

			if(changed) { Changed?.Invoke(this, EventArgs.Empty); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the usage count of an item.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="key">Item key.</param>
	/// <returns>Returns the usage count.</returns>
	public int GetCount(string scope, string key)
	{
		return (((_GetScope(scope) is Dictionary<string, int> counts) && counts.TryGetValue(key, out int rval)) ? rval : 0);
	}


	/// <summary>Increments the usage count of an item.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="key">Item key.</param>
	public void Increment(string scope, string key)
	{
		if(!_Scopes.TryGetValue(scope, out Dictionary<string, int>? counts))
		{
			counts = new Dictionary<string, int>(StringComparer.Ordinal);
			_Scopes.Add(scope, counts);
		}

		counts[key] = (counts.TryGetValue(key, out int count) ? ((count == int.MaxValue) ? count : (count + 1)) : 1);
		_Trim(counts, key);

		Changed?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Removes all entries.</summary>
	public void Clear()
	{
		_Scopes.Clear();
		Changed?.Invoke(this, EventArgs.Empty);
	}


	/// <summary>Removes all entries of a scope.</summary>
	/// <param name="scope">Scope.</param>
	public void Clear(string scope)
	{
		if(_Scopes.Remove(scope)) { Changed?.Invoke(this, EventArgs.Empty); }
	}


	/// <summary>Exports the history to a string.</summary>
	/// <returns>Returns the history as text, one entry per line (scope, key and count, separated by tabs).</returns>
	public string Export()
	{
		StringBuilder rval = new();

		foreach(KeyValuePair<string, Dictionary<string, int>> i in _Scopes)
		{
			foreach(KeyValuePair<string, int> j in i.Value)
			{
				rval.Append(_Escape(i.Key)).Append(_SEPARATOR).Append(_Escape(j.Key)).Append(_SEPARATOR).Append(j.Value).Append('\n');
			}
		}

		return rval.ToString();
	}


	/// <summary>Imports a history exported by <see cref="Export"/>.</summary>
	/// <param name="data">Exported history.</param>
	/// <param name="merge">Determines if the imported counts are added to the existing history. Otherwise, the history is replaced.</param>
	/// <remarks>Invalid lines are ignored.</remarks>
	public void Import(string data, bool merge = false)
	{
		if(!merge) { _Scopes.Clear(); }

		foreach(string i in data.Split('\n'))
		{
			string[] fields = i.TrimEnd('\r').Split(_SEPARATOR);

			if((fields.Length != 3) || (!int.TryParse(fields[2], out int count)) || (count < 1)) { continue; }

			string scope = _Unescape(fields[0]);
			string key = _Unescape(fields[1]);

			if(!_Scopes.TryGetValue(scope, out Dictionary<string, int>? counts))
			{
				counts = new Dictionary<string, int>(StringComparer.Ordinal);
				_Scopes.Add(scope, counts);
			}

			counts[key] = (counts.TryGetValue(key, out int existing) ? (int) Math.Min((long) existing + count, int.MaxValue) : count);
		}

		foreach(Dictionary<string, int> i in _Scopes.Values) { _Trim(i); }
		Changed?.Invoke(this, EventArgs.Empty);
	}
}
