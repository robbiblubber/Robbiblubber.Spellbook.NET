namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a panel that implements <see cref="IEditable"/> and records the executed commands.</summary>
[SupportedOSPlatform("windows")]
public class TestEditable: Panel, IEditable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the commands that can be executed.</summary>
	public HashSet<EditCommand> Allowed
	{
		get;
	} = new();


	/// <summary>Gets the executed commands.</summary>
	public List<EditCommand> Executed
	{
		get;
	} = new();


	/// <summary>Gets the number of handlers of the <see cref="EditStateChanged"/> event.</summary>
	public int HandlerCount
	{
		get { return (EditStateChanged?.GetInvocationList().Length ?? 0); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="EditStateChanged"/> event.</summary>
	public void RaiseEditStateChanged()
	{
		EditStateChanged?.Invoke(this, EventArgs.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditable                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public event EventHandler? EditStateChanged;


	/// <inheritdoc/>
	public bool CanExecute(EditCommand command)
	{
		return Allowed.Contains(command);
	}


	/// <inheritdoc/>
	public void Execute(EditCommand command)
	{
		Executed.Add(command);
	}
}
