namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="EditCommandProvider.Querying"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class EditCommandQueryEventArgs: EditCommandEventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="command">Edit command.</param>
	/// <param name="control">Control the command applies to, NULL if no control has the focus.</param>
	/// <param name="canExecute">Determines if the command can be executed by default.</param>
	public EditCommandQueryEventArgs(EditCommand command, Control? control, bool canExecute): base(command, control)
	{
		CanExecute = canExecute;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if the command can be executed.</summary>
	/// <remarks>The property is initialized with the result of the control, its <see cref="IEditable"/> implementation or its
	///          adapter.</remarks>
	public bool CanExecute
	{
		get; set;
	}
}
