namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that find the function call at the caret position implement this interface.</summary>
/// <remarks>The parser determines the language-specific syntax: strings, comments, brackets and identifiers. The default implementation
///          <see cref="SignatureCallParser"/> is configurable and fits most languages with C-like or SQL-like syntax.</remarks>
[SupportedOSPlatform("windows")]
public interface ISignatureCallParser
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds the innermost function call whose argument list contains a position.</summary>
	/// <param name="text">Text.</param>
	/// <param name="caretIndex">Caret index.</param>
	/// <returns>Returns the call, or NULL if the position is not within the argument list of a named call.</returns>
	/// <remarks>The method is called on the UI thread on every text change and caret movement during a signature help session.</remarks>
	public SignatureCall? Parse(string text, int caretIndex);
}
