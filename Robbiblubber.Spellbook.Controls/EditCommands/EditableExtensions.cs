namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides named shortcuts for the edit commands of <see cref="IEditable"/> objects.</summary>
[SupportedOSPlatform("windows")]
public static class EditableExtensions
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the cut command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanCut(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.CUT);
	}


	/// <summary>Executes the cut command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Cut(this IEditable editable)
	{
		editable.Execute(EditCommand.CUT);
	}


	/// <summary>Returns if the copy command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanCopy(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.COPY);
	}


	/// <summary>Executes the copy command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Copy(this IEditable editable)
	{
		editable.Execute(EditCommand.COPY);
	}


	/// <summary>Returns if the paste command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanPaste(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.PASTE);
	}


	/// <summary>Executes the paste command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Paste(this IEditable editable)
	{
		editable.Execute(EditCommand.PASTE);
	}


	/// <summary>Returns if the delete command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanDelete(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.DELETE);
	}


	/// <summary>Executes the delete command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Delete(this IEditable editable)
	{
		editable.Execute(EditCommand.DELETE);
	}


	/// <summary>Returns if the undo command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanUndo(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.UNDO);
	}


	/// <summary>Executes the undo command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Undo(this IEditable editable)
	{
		editable.Execute(EditCommand.UNDO);
	}


	/// <summary>Returns if the redo command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanRedo(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.REDO);
	}


	/// <summary>Executes the redo command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void Redo(this IEditable editable)
	{
		editable.Execute(EditCommand.REDO);
	}


	/// <summary>Returns if the select all command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanSelectAll(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.SELECT_ALL);
	}


	/// <summary>Executes the select all command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void SelectAll(this IEditable editable)
	{
		editable.Execute(EditCommand.SELECT_ALL);
	}


	/// <summary>Returns if the invert selection command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanInvertSelection(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.INVERT_SELECTION);
	}


	/// <summary>Executes the invert selection command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void InvertSelection(this IEditable editable)
	{
		editable.Execute(EditCommand.INVERT_SELECTION);
	}


	/// <summary>Returns if the clear selection command can be executed.</summary>
	/// <param name="editable">Editable object.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	public static bool CanClearSelection(this IEditable editable)
	{
		return editable.CanExecute(EditCommand.CLEAR_SELECTION);
	}


	/// <summary>Executes the clear selection command.</summary>
	/// <param name="editable">Editable object.</param>
	public static void ClearSelection(this IEditable editable)
	{
		editable.Execute(EditCommand.CLEAR_SELECTION);
	}
}
