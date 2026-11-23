namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.SelectionChanged"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionSelectionChangedEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="item">Selected item.</param>
	public AutoCompletionSelectionChangedEventArgs(TextBoxBase control, IAutoCompletionItem? item): base()
	{
		Control = control;
		Item = item;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	public TextBoxBase Control
	{
		get;
	}


	/// <summary>Gets the selected item.</summary>
	/// <remarks>This property is NULL if no item is selected, or if the auto completion list has been closed or hidden.</remarks>
	public IAutoCompletionItem? Item
	{
		get;
	}
}
