namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the edit commands for <see cref="ListBox"/> controls.</summary>
/// <remarks>Copy copies the texts of the selected items, one per line. Select all and invert selection are available for list boxes that
///          allow multiple selection. Commands that change the items (cut, paste, delete) are application-specific; they can be
///          implemented with the <see cref="EditCommandProvider.Querying"/> and <see cref="EditCommandProvider.Executing"/> events.</remarks>
[SupportedOSPlatform("windows")]
public class ListBoxEditAdapter: EditAdapter<ListBox>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditAdapter<ListBox>                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override bool _CanExecute(ListBox control, EditCommand command)
	{
		if(!control.Enabled) { return false; }

		return command switch
		{
			EditCommand.COPY => (control.SelectedIndices.Count > 0),
			EditCommand.SELECT_ALL => (control.IsMultiSelect() && (control.Items.Count > 0)),
			EditCommand.INVERT_SELECTION => (control.IsMultiSelect() && (control.Items.Count > 0)),
			EditCommand.CLEAR_SELECTION => ((control.SelectionMode != SelectionMode.None) && (control.SelectedIndices.Count > 0)),
			_ => false
		};
	}


	/// <inheritdoc/>
	protected override void _Execute(ListBox control, EditCommand command)
	{
		string text;

		switch(command)
		{
			case EditCommand.COPY:
				text = string.Join(Environment.NewLine, control.SelectedItems.Cast<object>().Select(m => control.GetItemText(m)));
				_Clipboard._TrySetText(text);
				break;
			case EditCommand.SELECT_ALL:
				control.SelectAll();
				break;
			case EditCommand.INVERT_SELECTION:
				control.InvertSelection();
				break;
			case EditCommand.CLEAR_SELECTION:
				control.ClearSelection();
				break;
		}
	}
}
