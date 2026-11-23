namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that support edit commands implement this interface.</summary>
/// <remarks>Controls that implement this interface handle the edit commands of an <see cref="EditCommandProvider"/> while they or one of
///          their child controls have the focus. The extension methods of <see cref="EditableExtensions"/> provide named shortcuts
///          like <c>Cut()</c> and <c>CanCut()</c>.</remarks>
[SupportedOSPlatform("windows")]
public interface IEditable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the availability of edit commands may have changed.</summary>
	/// <remarks>Raising this event is optional; it lets consumers update menus and tool bars immediately instead of on their next
	///          regular update.</remarks>
	public event EventHandler? EditStateChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if an edit command can be executed.</summary>
	/// <param name="command">Edit command.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public bool CanExecute(EditCommand command);


	/// <summary>Executes an edit command.</summary>
	/// <param name="command">Edit command.</param>
	/// <remarks>If the command cannot be executed, the method has no effect.</remarks>
	public void Execute(EditCommand command);
}
