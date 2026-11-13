namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



/// <summary>This class provides helper methods for text boxes shared by the auto completion and the signature help.</summary>
[SupportedOSPlatform("windows")]
internal static class _TextBoxUtility
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the client position of a character.</summary>
	/// <param name="control">Control.</param>
	/// <param name="text">Control text.</param>
	/// <param name="index">Character index.</param>
	/// <returns>Returns the position in client coordinates.</returns>
	/// <remarks><see cref="TextBoxBase.GetPositionFromCharIndex(int)"/> does not return a valid position for the index after the
	///          last character of a <see cref="TextBox"/>. In that case, the caret position or the position of the preceding
	///          character is used.</remarks>
	internal static Point _GetCharacterPosition(TextBoxBase control, string text, int index)
	{
		if(index < text.Length) { return control.GetPositionFromCharIndex(index); }

		if(control.Focused && (control.SelectionStart == index) && _GetCaretPos(out _Point caret)) { return new Point(caret.X, caret.Y); }
		if(index == 0) { return new Point(0, 0); }

		Point rval = control.GetPositionFromCharIndex(index - 1);
		char c = text[index - 1];

		if(c == '\n') { return new Point(control.GetPositionFromCharIndex(0).X, rval.Y + control.Font.Height); }

		rval.X += TextRenderer.MeasureText(c.ToString(), control.Font, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
		return rval;
	}
}
