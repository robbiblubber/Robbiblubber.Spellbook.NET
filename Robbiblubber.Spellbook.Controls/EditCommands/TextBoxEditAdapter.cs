namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the edit commands for <see cref="TextBox"/> and <see cref="RichTextBox"/> controls.</summary>
/// <remarks>Commands that change the text are not available for read-only or disabled controls. Cut and copy are not available for
///          password text boxes. Redo is only available for <see cref="RichTextBox"/>. Delete without a selection deletes the character
///          after the caret, like the Delete key; deleting can be undone. Invert selection works for selections at the start or the end
///          of the text, which are the only ones whose inverse is a single range.</remarks>
[SupportedOSPlatform("windows")]
public class TextBoxEditAdapter: EditAdapter<TextBoxBase>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the selection of a text box can be inverted.</summary>
	/// <param name="start">Selection start.</param>
	/// <param name="length">Selection length.</param>
	/// <param name="textLength">Text length.</param>
	/// <returns>Returns <c>true</c> if the selection can be inverted, otherwise returns <c>false</c>.</returns>
	internal static bool _CanInvert(int start, int length, int textLength)
	{
		return ((textLength > 0) && ((length == 0) || (start == 0) || ((start + length) == textLength)));
	}


	/// <summary>Gets the inverted selection.</summary>
	/// <param name="start">Selection start.</param>
	/// <param name="length">Selection length.</param>
	/// <param name="textLength">Text length.</param>
	/// <returns>Returns start and length of the inverted selection.</returns>
	internal static (int Start, int Length) _Invert(int start, int length, int textLength)
	{
		int end = start + length;

		if(length == 0) { return (0, textLength); }
		if((start == 0) && (end == textLength)) { return (end, 0); }
		if(start == 0) { return (end, textLength - end); }

		return (0, start);
	}


	/// <summary>Gets the length of the text range that is deleted after a position, if nothing is selected.</summary>
	/// <param name="text">Text.</param>
	/// <param name="start">Position.</param>
	/// <returns>Returns 2 for a line break (CR LF), otherwise 1.</returns>
	internal static int _GetDeleteLength(string text, int start)
	{
		return ((((start + 1) < text.Length) && (text[start] == '\r') && (text[start + 1] == '\n')) ? 2 : 1);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a text box hides its text.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the control is a password text box, otherwise returns <c>false</c>.</returns>
	private static bool _IsPassword(TextBoxBase control)
	{
		return ((control is TextBox t) && ((t.PasswordChar != '\0') || t.UseSystemPasswordChar));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditAdapter<TextBoxBase>                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override bool _CanExecute(TextBoxBase control, EditCommand command)
	{
		bool writable = (control.Enabled && (!control.ReadOnly));
		int length = control.TextLength;

		return command switch
		{
			EditCommand.CUT => (writable && (control.SelectionLength > 0) && (!_IsPassword(control))),
			EditCommand.COPY => ((control.SelectionLength > 0) && (!_IsPassword(control))),
			EditCommand.PASTE => (writable && _Clipboard._ContainsText()),
			EditCommand.DELETE => (writable && ((control.SelectionLength > 0) || (control.SelectionStart < length))),
			EditCommand.UNDO => (writable && control.CanUndo),
			EditCommand.REDO => (writable && (control is RichTextBox rich) && rich.CanRedo),
			EditCommand.SELECT_ALL => (control.Enabled && (length > 0)),
			EditCommand.INVERT_SELECTION => (control.Enabled && _CanInvert(control.SelectionStart, control.SelectionLength, length)),
			EditCommand.CLEAR_SELECTION => (control.SelectionLength > 0),
			_ => false
		};
	}


	/// <inheritdoc/>
	protected override void _Execute(TextBoxBase control, EditCommand command)
	{
		(int Start, int Length) inverted;

		switch(command)
		{
			case EditCommand.CUT:
				control.Cut();
				break;
			case EditCommand.COPY:
				control.Copy();
				break;
			case EditCommand.PASTE:
				control.Paste();
				break;
			case EditCommand.DELETE:
				if(control.SelectionLength == 0) { control.Select(control.SelectionStart, _GetDeleteLength(control.Text, control.SelectionStart)); }
				if(control is TextBox text)
				{
					text.Paste(string.Empty);
				}
				else { control.SelectedText = string.Empty; }
				break;
			case EditCommand.UNDO:
				control.Undo();
				break;
			case EditCommand.REDO:
				((RichTextBox) control).Redo();
				break;
			case EditCommand.SELECT_ALL:
				control.SelectAll();
				break;
			case EditCommand.INVERT_SELECTION:
				inverted = _Invert(control.SelectionStart, control.SelectionLength, control.TextLength);
				control.Select(inverted.Start, inverted.Length);
				break;
			case EditCommand.CLEAR_SELECTION:
				control.Select(control.SelectionStart + control.SelectionLength, 0);
				break;
		}
	}
}
