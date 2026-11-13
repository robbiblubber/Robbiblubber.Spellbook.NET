namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class parses ddn text.</summary>
/// <remarks><para>The parser is lenient: a missing semicolon before a closing bracket or at the end of the text, a statement without
///          equals sign (NULL value), unbalanced brackets and the obsolete equals sign in section declarations are accepted.</para>
///          <para>Values are kept in their masked form; comments inside values are replaced by a space.</para></remarks>
internal sealed class _DdnParser
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text.</summary>
	private readonly string _Text;

	/// <summary>Duplicate element list.</summary>
	private readonly List<DdnElement>? _Duplicates;

	/// <summary>Statement buffer.</summary>
	private readonly StringBuilder _Statement = new();

	/// <summary>Position of the first unmasked equals sign in the statement buffer.</summary>
	private int _Equals = -1;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Text.</param>
	/// <param name="duplicates">List that receives elements that are defined more than once, or NULL.</param>
	private _DdnParser(string text, List<DdnElement>? duplicates)
	{
		_Text = text;
		_Duplicates = duplicates;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Clears the statement buffer.</summary>
	private void _Reset()
	{
		_Statement.Clear();
		_Equals = -1;
	}


	/// <summary>Adds the value statement in the buffer to a section.</summary>
	/// <param name="section">Section.</param>
	private void _AddValue(DdnElement section)
	{
		string stmt = _Statement.ToString();
		int eq = _Equals;
		_Reset();

		if(string.IsNullOrWhiteSpace(stmt)) { return; }

		string name = _DdnText._Unmask(_DdnText._Trim((eq < 0) ? stmt : stmt[..eq]));
		string text = ((eq < 0) ? _DdnText._NULL : _DdnText._Trim(stmt[(eq + 1)..]));

		if(name.Length == 0) { return; }

		DdnElement elm = section._ParseChild(name, false, out bool duplicate);
		elm._SetText(text);

		if(duplicate) { _Duplicates?.Add(elm); }
	}


	/// <summary>Opens a section for the section declaration in the buffer.</summary>
	/// <param name="section">Parent section.</param>
	/// <returns>Returns the new section; a section without name returns the parent section, its elements are added there.</returns>
	private DdnElement _OpenSection(DdnElement section)
	{
		string stmt = _Statement.ToString();
		if((_Equals >= 0) && string.IsNullOrWhiteSpace(stmt[(_Equals + 1)..])) { stmt = stmt[.._Equals]; }
		_Reset();

		string name = _DdnText._Unmask(_DdnText._Trim(stmt));
		if(name.Length == 0) { return section; }

		DdnElement rval = section._ParseChild(name, true, out bool duplicate);
		if(duplicate) { _Duplicates?.Add(rval); }

		return rval;
	}


	/// <summary>Parses the text into an element.</summary>
	/// <param name="root">Root element.</param>
	private void _Parse(DdnElement root)
	{
		Stack<DdnElement> sections = new();
		DdnElement cur = root;
		string t = _Text;

		for(int i = 0; i < t.Length; i++)
		{
			char c = t[i];

			if(c == '\\')
			{
				_Statement.Append(c);
				if((i + 1) < t.Length) { _Statement.Append(t[++i]); }
				continue;
			}

			if((c == '/') && _DdnText._IsCommentStart(t, i))
			{
				if(t[i + 1] == '/')
				{
					while(((i + 1) < t.Length) && (t[i + 1] != '\n')) { i++; }
				}
				else
				{
					int end = t.IndexOf("*/", i + 2, StringComparison.Ordinal);
					i = ((end < 0) ? t.Length : (end + 1));
				}
				_Statement.Append(' ');
				continue;
			}

			switch(c)
			{
				case '=':
					if(_Equals < 0) { _Equals = _Statement.Length; }
					_Statement.Append(c);
					break;
				case ';':
					_AddValue(cur);
					break;
				case '{':
					sections.Push(cur);
					cur = _OpenSection(cur);
					break;
				case '}':
					_AddValue(cur);
					if(sections.Count > 0) { cur = sections.Pop(); }
					break;
				default:
					_Statement.Append(c);
					break;
			}
		}

		_AddValue(cur);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses ddn text into an element.</summary>
	/// <param name="text">Text.</param>
	/// <param name="root">Element that receives the parsed elements.</param>
	/// <param name="duplicates">List that receives elements that are defined more than once, or NULL.</param>
	internal static void _Parse(string text, DdnElement root, List<DdnElement>? duplicates)
	{
		new _DdnParser(text, duplicates)._Parse(root);
	}
}
