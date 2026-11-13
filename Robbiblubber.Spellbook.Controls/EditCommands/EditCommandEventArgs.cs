namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for edit command events.</summary>
[SupportedOSPlatform("windows")]
public class EditCommandEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="command">Edit command.</param>
	/// <param name="control">Control the command applies to, NULL if no control has the focus.</param>
	public EditCommandEventArgs(EditCommand command, Control? control): base()
	{
		Command = command;
		Control = control;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the edit command.</summary>
	public EditCommand Command
	{
		get; protected init;
	}


	/// <summary>Gets the control the command applies to, NULL if no control has the focus.</summary>
	public Control? Control
	{
		get; protected init;
	}
}
