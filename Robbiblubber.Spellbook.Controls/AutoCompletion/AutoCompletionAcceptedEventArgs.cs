namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.Accepted"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionAcceptedEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="item">Accepted item.</param>
	/// <param name="insertText">Inserted text.</param>
	/// <param name="insertStart">Start index of the inserted text.</param>
	public AutoCompletionAcceptedEventArgs(TextBoxBase control, IAutoCompletionItem item, string insertText, int insertStart): base()
	{
		Control = control;
		Item = item;
		InsertText = insertText;
		InsertStart = insertStart;
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


	/// <summary>Gets the inserted text.</summary>
	public string InsertText
	{
		get;
	}


	/// <summary>Gets the start index of the inserted text.</summary>
	public int InsertStart
	{
		get;
	}
}
