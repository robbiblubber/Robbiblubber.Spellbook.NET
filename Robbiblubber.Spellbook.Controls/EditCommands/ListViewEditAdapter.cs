namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements the edit commands for <see cref="ListView"/> controls.</summary>
/// <remarks>Copy copies the selected items, one per line, with the texts of their sub-items separated by tabs. Select all and invert
///          selection are available for list views that allow multiple selection. Commands that change the items (cut, paste, delete)
///          are application-specific; they can be implemented with the <see cref="EditCommandProvider.Querying"/> and
///          <see cref="EditCommandProvider.Executing"/> events.</remarks>
[SupportedOSPlatform("windows")]
public class ListViewEditAdapter: EditAdapter<ListView>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text of a list view item including its sub-items.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the texts of the item and its sub-items, separated by tabs.</returns>
	internal static string _GetItemText(ListViewItem item)
	{
		return string.Join("\t", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(m => m.Text));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditAdapter<ListView>                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override bool _CanExecute(ListView control, EditCommand command)
	{
		if(!control.Enabled) { return false; }

		return command switch
		{
			EditCommand.COPY => (control.SelectedIndices.Count > 0),
			EditCommand.SELECT_ALL => (control.MultiSelect && (control.GetItemCount() > 0)),
			EditCommand.INVERT_SELECTION => (control.MultiSelect && (control.GetItemCount() > 0)),
			EditCommand.CLEAR_SELECTION => (control.SelectedIndices.Count > 0),
			_ => false
		};
	}


	/// <inheritdoc/>
	protected override void _Execute(ListView control, EditCommand command)
	{
		string text;

		switch(command)
		{
			case EditCommand.COPY:
				text = string.Join(Environment.NewLine, control.SelectedIndices.Cast<int>().Select(m => _GetItemText(control.Items[m])));
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
