namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that implement edit commands for a control type implement this interface.</summary>
/// <remarks>Adapters are registered for control types with <see cref="EditAdapters"/>. They add edit command support to controls
///          that do not implement <see cref="IEditable"/>, like the standard Windows Forms controls.</remarks>
[SupportedOSPlatform("windows")]
public interface IEditAdapter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if an edit command can be executed for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="command">Edit command.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public bool CanExecute(Control control, EditCommand command);


	/// <summary>Executes an edit command for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="command">Edit command.</param>
	/// <remarks>If the command cannot be executed, the method has no effect.</remarks>
	public void Execute(Control control, EditCommand command);
}
