namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the edit commands for <see cref="ComboBox"/> controls.</summary>
/// <remarks>For combo boxes with an edit field, the commands work on the text of the edit field; pasted text is limited to its first
///          line. Undo uses the native single-level undo of the edit field, so undoing twice restores the change; redo is not supported.
///          For <see cref="ComboBoxStyle.DropDownList"/> combo boxes, only copy is available; it copies the text of the selected
///          item.</remarks>
[SupportedOSPlatform("windows")]
public class ComboBoxEditAdapter: EditAdapter<ComboBox>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the first line of a text.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the first line.</returns>
	internal static string _FirstLine(string text)
	{
		int end = text.IndexOfAny(['\r', '\n']);
		return ((end < 0) ? text : text.Substring(0, end));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a combo box has an edit field that can be changed.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the combo box is editable, otherwise returns <c>false</c>.</returns>
	private static bool _IsEditable(ComboBox control)
	{
		return (control.Enabled && (control.DropDownStyle != ComboBoxStyle.DropDownList));
	}


	/// <summary>Gets the handle of the edit field of a combo box.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the window handle, or <see cref="IntPtr.Zero"/> if the combo box has no edit field or no handle.</returns>
	private static IntPtr _GetEditHandle(ComboBox control)
	{
		if((!control.IsHandleCreated) || (control.DropDownStyle == ComboBoxStyle.DropDownList)) { return IntPtr.Zero; }
		return _NativeMethods._FindWindowEx(control.Handle, IntPtr.Zero, "Edit", null);
	}


	/// <summary>Returns if the last change of the edit field of a combo box can be undone.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the change can be undone, otherwise returns <c>false</c>.</returns>
	private static bool _CanUndo(ComboBox control)
	{
		IntPtr edit = _GetEditHandle(control);
		return ((edit != IntPtr.Zero) && (_NativeMethods._SendMessage(edit, _NativeMethods._EM_CANUNDO, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditAdapter<ComboBox>                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override bool _CanExecute(ComboBox control, EditCommand command)
	{
		bool editable = _IsEditable(control);
		int length = control.Text.Length;

		return command switch
		{
			EditCommand.CUT => (editable && (control.SelectionLength > 0)),
			EditCommand.COPY => (editable ? (control.SelectionLength > 0) : (control.Enabled && (length > 0))),
			EditCommand.PASTE => (editable && _Clipboard._ContainsText()),
			EditCommand.DELETE => (editable && ((control.SelectionLength > 0) || (control.SelectionStart < length))),
			EditCommand.UNDO => (editable && _CanUndo(control)),
			EditCommand.SELECT_ALL => (editable && (length > 0)),
			EditCommand.INVERT_SELECTION => (editable && TextBoxEditAdapter._CanInvert(control.SelectionStart, control.SelectionLength, length)),
			EditCommand.CLEAR_SELECTION => (editable && (control.SelectionLength > 0)),
			_ => false
		};
	}


	/// <inheritdoc/>
	protected override void _Execute(ComboBox control, EditCommand command)
	{
		(int Start, int Length) inverted;
		string text;

		switch(command)
		{
			case EditCommand.CUT:
				if(_Clipboard._TrySetText(control.SelectedText)) { control.SelectedText = string.Empty; }
				break;
			case EditCommand.COPY:
				_Clipboard._TrySetText(_IsEditable(control) ? control.SelectedText : control.Text);
				break;
			case EditCommand.PASTE:
				if(_Clipboard._TryGetText(out text)) { control.SelectedText = _FirstLine(text); }
				break;
			case EditCommand.DELETE:
				if(control.SelectionLength == 0) { control.Select(control.SelectionStart, 1); }
				control.SelectedText = string.Empty;
				break;
			case EditCommand.UNDO:
				_NativeMethods._SendMessage(_GetEditHandle(control), _NativeMethods._WM_UNDO, IntPtr.Zero, IntPtr.Zero);
				break;
			case EditCommand.SELECT_ALL:
				control.SelectAll();
				break;
			case EditCommand.INVERT_SELECTION:
				inverted = TextBoxEditAdapter._Invert(control.SelectionStart, control.SelectionLength, control.Text.Length);
				control.Select(inverted.Start, inverted.Length);
				break;
			case EditCommand.CLEAR_SELECTION:
				control.Select(control.SelectionStart + control.SelectionLength, 0);
				break;
		}
	}
}
