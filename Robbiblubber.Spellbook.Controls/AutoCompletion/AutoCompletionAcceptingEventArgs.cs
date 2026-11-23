namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.Accepting"/> event.</summary>
/// <remarks>If the event is cancelled, the auto completion session remains open.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionAcceptingEventArgs: CancelEventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="item">Accepted item.</param>
	/// <param name="insertText">Text to insert.</param>
	/// <param name="caretPosition">Caret position relative to the start of the inserted text.</param>
	/// <param name="replaceStart">Start index of the replaced text range.</param>
	/// <param name="replaceLength">Length of the replaced text range.</param>
	public AutoCompletionAcceptingEventArgs(TextBoxBase control, IAutoCompletionItem item, string insertText, int? caretPosition, int replaceStart, int replaceLength): base()
	{
		Control = control;
		Item = item;
		InsertText = insertText;
		CaretPosition = caretPosition;
		ReplaceStart = replaceStart;
		ReplaceLength = replaceLength;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	public TextBoxBase Control
	{
		get;
	}


	/// <summary>Gets the accepted item.</summary>
	public IAutoCompletionItem Item
	{
		get;
	}


	/// <summary>Gets or sets the text to insert.</summary>
	public string InsertText
	{
		get; set;
	}


	/// <summary>Gets or sets the caret position after insertion, relative to the start of the inserted text.</summary>
	/// <remarks>If this property is NULL, the caret is placed at the end of the inserted text.</remarks>
	public int? CaretPosition
	{
		get; set;
	}


	/// <summary>Gets or sets the start index of the replaced text range.</summary>
	public int ReplaceStart
	{
		get; set;
	}


	/// <summary>Gets or sets the length of the replaced text range.</summary>
	public int ReplaceLength
	{
		get; set;
	}
}
