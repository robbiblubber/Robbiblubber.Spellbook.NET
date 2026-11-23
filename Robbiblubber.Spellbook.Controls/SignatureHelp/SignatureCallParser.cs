namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements a configurable parser that finds the function call at the caret position.</summary>
/// <remarks>The parser scans the text from the start to the caret and tracks strings, comments and brackets, so separators and brackets
///          inside strings, comments and nested brackets are ignored. A call is an opening parenthesis preceded by a name, optionally
///          qualified with dots (<c>dbo.fn(</c>). Parentheses without a name (grouping) are skipped, so the enclosing call is found.
///          If the caret is inside a string, the call is still found; inside a comment, no call is found. The defaults fit SQL and
///          most C-like languages; comments are not recognized unless configured:
///          <c>new SignatureCallParser { LineComment = "--", BlockCommentStart = "/*", BlockCommentEnd = "*/" }</c>.
///          <see cref="GetSyntaxState"/> applies the same rules to tell code, strings and comments apart, for example for an auto
///          completion source that should not offer items in comments.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureCallParser: ISignatureCallParser
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public SignatureCallParser()
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a text contains a token at a position.</summary>
	/// <param name="text">Text.</param>
	/// <param name="index">Position.</param>
	/// <param name="token">Token, NULL or empty for none.</param>
	/// <returns>Returns <c>true</c> if the token is found at the position, otherwise returns <c>false</c>.</returns>
	private static bool _IsAt(string text, int index, string? token)
	{
		return ((!string.IsNullOrEmpty(token)) && ((index + token.Length) <= text.Length) && (string.CompareOrdinal(text, index, token, 0, token.Length) == 0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a character is part of a name.</summary>
	/// <param name="c">Character.</param>
	/// <returns>Returns <c>true</c> if the character is a name character, otherwise returns <c>false</c>.</returns>
	private bool _IsNameCharacter(char c)
	{
		return (char.IsLetterOrDigit(c) || NameCharacters.Contains(c));
	}


	/// <summary>Gets the closing bracket for an opening bracket.</summary>
	/// <param name="open">Opening bracket.</param>
	/// <returns>Returns the closing bracket, or <c>'\0'</c> if the character is not an opening bracket.</returns>
	private char _GetClosing(char open)
	{
		if(open == '(') { return ')'; }

		for(int i = 0; (i + 1) < NestingBrackets.Length; i += 2)
		{
			if(NestingBrackets[i] == open) { return NestingBrackets[i + 1]; }
		}

		return '\0';
	}


	/// <summary>Returns if a character is a closing bracket.</summary>
	/// <param name="c">Character.</param>
	/// <returns>Returns <c>true</c> if the character is a closing bracket, otherwise returns <c>false</c>.</returns>
	private bool _IsClosing(char c)
	{
		if(c == ')') { return true; }

		for(int i = 1; i < NestingBrackets.Length; i += 2)
		{
			if(NestingBrackets[i] == c) { return true; }
		}

		return false;
	}


	/// <summary>Scans a text up to a position, tracking strings, comments and brackets.</summary>
	/// <param name="text">Text.</param>
	/// <param name="caretIndex">Position; the text before it is scanned.</param>
	/// <param name="stack">Receives the open brackets at the position, NULL if brackets are not needed.</param>
	/// <returns>Returns the syntax state at the position.</returns>
	private SyntaxState _Scan(string text, int caretIndex, List<_Frame>? stack)
	{
		char quote = '\0';
		bool lineComment = false, blockComment = false;

		caretIndex = Math.Clamp(caretIndex, 0, text.Length);

		for(int i = 0; i < caretIndex; i++)
		{
			char c = text[i];

			if(quote != '\0')
			{
				if((c == EscapeCharacter) && (EscapeCharacter != '\0')) { i++; }
				else if(c == quote) { quote = '\0'; }
				continue;
			}

			if(lineComment)
			{
				if(c == '\n') { lineComment = false; }
				continue;
			}

			if(blockComment)
			{
				if(_IsAt(text, i, BlockCommentEnd))
				{
					blockComment = false;
					i += BlockCommentEnd!.Length - 1;
				}
				continue;
			}

			if(_IsAt(text, i, LineComment))
			{
				lineComment = true;
				i += LineComment!.Length - 1;
				continue;
			}

			if(_IsAt(text, i, BlockCommentStart) && (!string.IsNullOrEmpty(BlockCommentEnd)))
			{
				blockComment = true;
				i += BlockCommentStart!.Length - 1;
				continue;
			}

			if(StringDelimiters.Contains(c))
			{
				quote = c;
			}
			else if(stack is null)
			{
				continue;
			}
			else if(_GetClosing(c) != '\0')
			{
				stack.Add(new _Frame(c, i, 0, i + 1));
			}
			else if(_IsClosing(c))
			{
				for(int j = stack.Count - 1; j >= 0; j--)
				{
					if(_GetClosing(stack[j].Open) != c) { continue; }

					stack.RemoveRange(j, stack.Count - j);
					break;
				}
			}
			else if((c == ArgumentSeparator) && (stack.Count > 0))
			{
				_Frame top = stack[^1];
				stack[^1] = new _Frame(top.Open, top.Index, top.ArgumentIndex + 1, i + 1);
			}
		}

		if(lineComment || blockComment) { return SyntaxState.COMMENT; }
		return ((quote == '\0') ? SyntaxState.CODE : SyntaxState.STRING);
	}


	/// <summary>Creates a call for an opening parenthesis, if it is preceded by a name.</summary>
	/// <param name="text">Text.</param>
	/// <param name="frame">Bracket frame of the parenthesis.</param>
	/// <returns>Returns the call, or NULL if the parenthesis is not preceded by a name.</returns>
	private SignatureCall? _CreateCall(string text, _Frame frame)
	{
		int end = frame.Index;
		int start, qualifierStart;

		while(AllowWhitespaceBeforeParenthesis && (end > 0) && ((text[end - 1] == ' ') || (text[end - 1] == '\t'))) { end--; }

		start = end;
		while((start > 0) && _IsNameCharacter(text[start - 1])) { start--; }

		if((start == end) || char.IsDigit(text[start])) { return null; }

		qualifierStart = start;
		while((qualifierStart > 0) && (text[qualifierStart - 1] == '.'))
		{
			int previous = qualifierStart - 1;

			while((previous > 0) && _IsNameCharacter(text[previous - 1])) { previous--; }
			if(previous == (qualifierStart - 1)) { break; }

			qualifierStart = previous;
		}

		return new SignatureCall(text.Substring(start, end - start), ((qualifierStart < start) ? text.Substring(qualifierStart, start - qualifierStart - 1) : string.Empty), qualifierStart, frame.Index, frame.ArgumentIndex, frame.ArgumentStart);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the characters that delimit strings.</summary>
	/// <remarks>A string ends with the same character it started with. Doubled delimiters (SQL <c>'it''s'</c>) work without further
	///          configuration.</remarks>
	public string StringDelimiters
	{
		get; set;
	} = "'\"";


	/// <summary>Gets or sets the escape character within strings, <c>'\0'</c> for none.</summary>
	/// <remarks>The character following the escape character never ends a string (C-like <c>"a\"b"</c>).</remarks>
	public char EscapeCharacter
	{
		get; set;
	} = '\0';


	/// <summary>Gets or sets the token that starts a comment running to the end of the line, NULL for none.</summary>
	public string? LineComment
	{
		get; set;
	}


	/// <summary>Gets or sets the token that starts a block comment, NULL for none.</summary>
	public string? BlockCommentStart
	{
		get; set;
	}


	/// <summary>Gets or sets the token that ends a block comment, NULL for none.</summary>
	public string? BlockCommentEnd
	{
		get; set;
	}


	/// <summary>Gets or sets the characters that are part of names in addition to letters and digits.</summary>
	public string NameCharacters
	{
		get; set;
	} = "_";


	/// <summary>Gets or sets the separator between arguments.</summary>
	public char ArgumentSeparator
	{
		get; set;
	} = ',';


	/// <summary>Gets or sets pairs of brackets that nest within arguments, opening and closing bracket alternating.</summary>
	/// <remarks>Separators within nested brackets do not count as argument separators (<c>fn(a[1, 2], b)</c>). Parentheses are always
	///          recognized.</remarks>
	public string NestingBrackets
	{
		get; set;
	} = "[]{}";


	/// <summary>Gets or sets if spaces and tabs are allowed between the name and the opening parenthesis (<c>COUNT (*)</c>).</summary>
	public bool AllowWhitespaceBeforeParenthesis
	{
		get; set;
	} = true;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the syntax state at a position: code, string or comment.</summary>
	/// <param name="text">Text.</param>
	/// <param name="caretIndex">Position.</param>
	/// <returns>Returns the syntax state.</returns>
	/// <remarks>The method uses the same rules for strings and comments as <see cref="Parse"/>. Auto completion sources can use it to
	///          offer no items in comments and strings, with the same parser instance that is used for the signature help.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the text is NULL.</exception>
	public SyntaxState GetSyntaxState(string text, int caretIndex)
	{
		if(text is null) { throw new ArgumentNullException(nameof(text)); }
		return _Scan(text, caretIndex, null);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] ISignatureCallParser                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public SignatureCall? Parse(string text, int caretIndex)
	{
		List<_Frame> stack = new();

		if(text is null) { throw new ArgumentNullException(nameof(text)); }
		if(_Scan(text, caretIndex, stack) == SyntaxState.COMMENT) { return null; }

		for(int i = stack.Count - 1; i >= 0; i--)
		{
			if(stack[i].Open != '(') { continue; }

			SignatureCall? rval = _CreateCall(text, stack[i]);
			if(rval is not null) { return rval; }
		}

		return null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] SyntaxState                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines the syntax states of a text position, as determined by <see cref="GetSyntaxState"/>.</summary>
	public enum SyntaxState: int
	{
		/// <summary>The position is in code, outside strings and comments.</summary>
		CODE = 0,

		/// <summary>The position is within a string.</summary>
		STRING = 1,

		/// <summary>The position is within a comment.</summary>
		COMMENT = 2
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _Frame                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents an open bracket during parsing.</summary>
	/// <param name="Open">Opening bracket.</param>
	/// <param name="Index">Index of the opening bracket.</param>
	/// <param name="ArgumentIndex">Number of separators found within the bracket.</param>
	/// <param name="ArgumentStart">Start index of the current argument.</param>
	private readonly record struct _Frame(char Open, int Index, int ArgumentIndex, int ArgumentStart);
}
