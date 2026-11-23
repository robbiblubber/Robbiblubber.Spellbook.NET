namespace Robbiblubber.Spellbook;

using System.Text;



/// <summary>This class provides masking and splitting of ddn text.</summary>
/// <remarks>Element values are kept in their masked form (as they appear in a ddn document). This class converts between masked
///          and plain text, splits masked lists and paths and trims masked text.</remarks>
internal static class _DdnText
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal constants                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Masked NULL value.</summary>
	internal const string _NULL = @"\0";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the character at a position is masked by a preceding backslash.</summary>
	/// <param name="text">Masked text.</param>
	/// <param name="pos">Position.</param>
	/// <returns>Returns TRUE if the character is masked, otherwise returns FALSE.</returns>
	private static bool _IsMasked(string text, int pos)
	{
		int n = 0;
		for(int i = pos - 1; ((i >= 0) && (text[i] == '\\')); i--) { n++; }

		return ((n % 2) == 1);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Masks a string for use as a ddn name or value.</summary>
	/// <param name="value">Plain text.</param>
	/// <returns>Returns the masked text.</returns>
	internal static string _Mask(string value)
	{
		string v = value.Replace("\r\n", "\n");
		StringBuilder rval = new(v.Length + 8);

		int first = 0;
		while((first < v.Length) && (v[first] == ' ')) { first++; }

		int last = v.Length;
		while((last > first) && (v[last - 1] == ' ')) { last--; }

		for(int i = 0; i < v.Length; i++)
		{
			char c = v[i];
			switch(c)
			{
				case ' ':
					rval.Append(((i < first) || (i >= last)) ? @"\_" : " "); break;
				case '\\': case '/': case '=': case ';': case ',': case '{': case '}':
					rval.Append('\\').Append(c); break;
				case '\n': case '\r':
					rval.Append(@"\n"); break;
				case '\t':
					rval.Append(@"\t"); break;
				default:
					rval.Append(c); break;
			}
		}

		return rval.ToString();
	}


	/// <summary>Converts masked text to plain text.</summary>
	/// <param name="text">Masked text.</param>
	/// <returns>Returns the plain text.</returns>
	internal static string _Unmask(string text)
	{
		if(text.IndexOf('\\') < 0) { return text; }

		StringBuilder rval = new(text.Length);
		for(int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if((c != '\\') || (i == (text.Length - 1))) { rval.Append(c); continue; }

			c = text[++i];
			switch(c)
			{
				case 'n': rval.Append(Environment.NewLine); break;
				case 't': rval.Append('\t'); break;
				case '_': rval.Append(' '); break;
				case '0': break;
				default:  rval.Append(c); break;
			}
		}

		return rval.ToString();
	}


	/// <summary>Removes unmasked leading and trailing whitespace from masked text.</summary>
	/// <param name="text">Masked text.</param>
	/// <returns>Returns the trimmed text.</returns>
	internal static string _Trim(string text)
	{
		int first = 0;
		while((first < text.Length) && char.IsWhiteSpace(text[first])) { first++; }

		int last = text.Length;
		while((last > first) && char.IsWhiteSpace(text[last - 1]) && (!_IsMasked(text, last - 1))) { last--; }

		return text[first..last];
	}


	/// <summary>Splits masked text at unmasked separators.</summary>
	/// <param name="text">Masked text.</param>
	/// <param name="separator">Separator.</param>
	/// <returns>Returns the masked parts.</returns>
	internal static List<string> _Split(string text, char separator)
	{
		List<string> rval = new();
		int start = 0;

		for(int i = 0; i < text.Length; i++)
		{
			if(text[i] == '\\') { i++; continue; }
			if(text[i] == separator)
			{
				rval.Add(text[start..i]);
				start = i + 1;
			}
		}
		rval.Add(text[start..]);

		return rval;
	}


	/// <summary>Splits a masked list value into its masked items.</summary>
	/// <param name="text">Masked value text.</param>
	/// <returns>Returns the trimmed, masked items. An empty value returns an empty list.</returns>
	internal static List<string> _SplitList(string text)
	{
		if(string.IsNullOrWhiteSpace(text)) { return new(); }

		return _Split(text, ',').ConvertAll(m => _Trim(m));
	}


	/// <summary>Parses a ddn path.</summary>
	/// <param name="path">Path.</param>
	/// <param name="absolute">Returns if the path is absolute.</param>
	/// <returns>Returns the plain names of the path; "." and ".." are returned as they are.</returns>
	internal static List<string> _ParsePath(string path, out bool absolute)
	{
		List<string> rval = new();
		string p = _Trim(path);

		absolute = p.StartsWith('/');
		foreach(string i in _Split(p, '/'))
		{
			string name = _Trim(i);
			if(name.Length == 0) { continue; }

			rval.Add(((name == ".") || (name == "..")) ? name : _Unmask(name));
		}

		return rval;
	}


	/// <summary>Returns if a comment may start at a position.</summary>
	/// <param name="text">Text.</param>
	/// <param name="pos">Position of the slash.</param>
	/// <returns>Returns TRUE if the slash starts a comment, otherwise returns FALSE.</returns>
	/// <remarks>A comment starts with <c>//</c> or <c>/*</c> at the beginning of the text, after whitespace or after a structural
	///          character (<c>;</c>, <c>{</c>, <c>}</c>, <c>=</c>).</remarks>
	internal static bool _IsCommentStart(string text, int pos)
	{
		if((pos + 1) >= text.Length) { return false; }
		if((text[pos + 1] != '/') && (text[pos + 1] != '*')) { return false; }
		if(pos == 0) { return true; }

		char prev = text[pos - 1];
		if(_IsMasked(text, pos - 1)) { return false; }

		return (char.IsWhiteSpace(prev) || (prev == ';') || (prev == '{') || (prev == '}') || (prev == '='));
	}
}
