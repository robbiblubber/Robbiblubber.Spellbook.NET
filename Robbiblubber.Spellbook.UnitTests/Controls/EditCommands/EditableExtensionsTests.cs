namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="EditableExtensions"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class EditableExtensionsTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The named methods query and execute the corresponding commands.</summary>
	[Test]
	public void NamedMethodsMapToCommands()
	{
		using TestEditable editable = new();
		(Func<bool> Can, Action Execute, EditCommand Command)[] methods = new (Func<bool>, Action, EditCommand)[]
		{
			(editable.CanCut, editable.Cut, EditCommand.CUT),
			(editable.CanCopy, editable.Copy, EditCommand.COPY),
			(editable.CanPaste, editable.Paste, EditCommand.PASTE),
			(editable.CanDelete, editable.Delete, EditCommand.DELETE),
			(editable.CanUndo, editable.Undo, EditCommand.UNDO),
			(editable.CanRedo, editable.Redo, EditCommand.REDO),
			(editable.CanSelectAll, editable.SelectAll, EditCommand.SELECT_ALL),
			(editable.CanInvertSelection, editable.InvertSelection, EditCommand.INVERT_SELECTION),
			(editable.CanClearSelection, editable.ClearSelection, EditCommand.CLEAR_SELECTION)
		};

		foreach((Func<bool> Can, Action Execute, EditCommand Command) i in methods)
		{
			editable.Allowed.Clear();
			editable.Executed.Clear();

			Assert.That(i.Can(), Is.False, i.Command.ToString());
			editable.Allowed.Add(i.Command);
			Assert.That(i.Can(), Is.True, i.Command.ToString());

			i.Execute();
			Assert.That(editable.Executed, Is.EqualTo(new EditCommand[] { i.Command }), i.Command.ToString());
		}
	}
}
