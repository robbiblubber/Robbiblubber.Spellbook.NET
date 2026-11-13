namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="EditCommandProvider.Executing"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class EditCommandExecuteEventArgs: EditCommandEventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="command">Edit command.</param>
	/// <param name="control">Control the command applies to, NULL if no control has the focus.</param>
	public EditCommandExecuteEventArgs(EditCommand command, Control? control): base(command, control)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if the command has been handled by the event handler.</summary>
	/// <remarks>If an event handler sets this property, the default implementation of the command is not executed.</remarks>
	public bool Handled
	{
		get; set;
	}
}
