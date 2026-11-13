namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides a base for edit adapters of a control type.</summary>
/// <typeparam name="T">Control type.</typeparam>
/// <remarks>Derived classes implement <see cref="_CanExecute"/> and <see cref="_Execute"/> for the typed control. <see cref="Execute"/>
///          only calls <see cref="_Execute"/> if <see cref="_CanExecute"/> allows the command, so implementations can rely on it.</remarks>
[SupportedOSPlatform("windows")]
public abstract class EditAdapter<T>: IEditAdapter where T: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if an edit command can be executed for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="command">Edit command.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	protected abstract bool _CanExecute(T control, EditCommand command);


	/// <summary>Executes an edit command for a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="command">Edit command.</param>
	/// <remarks>This method is only called if <see cref="_CanExecute"/> returns <c>true</c>.</remarks>
	protected abstract void _Execute(T control, EditCommand command);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditAdapter                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual bool CanExecute(Control control, EditCommand command)
	{
		return ((control is T typed) && _CanExecute(typed, command));
	}


	/// <inheritdoc/>
	public virtual void Execute(Control control, EditCommand command)
	{
		if((control is T typed) && _CanExecute(typed, command)) { _Execute(typed, command); }
	}
}
