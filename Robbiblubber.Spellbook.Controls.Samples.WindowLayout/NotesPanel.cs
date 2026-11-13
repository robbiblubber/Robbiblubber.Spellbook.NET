using System.Runtime.Versioning;

namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;



/// <summary>This class implements a user control with its own layout provider.</summary>
/// <remarks>The splitter position is restored when the user control is loaded and saved when its form is closed.</remarks>
[SupportedOSPlatform("windows")]
public partial class NotesPanel: UserControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public NotesPanel()
	{
		InitializeComponent();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Restored event of the layout provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Layout_Restored(object? sender, EventArgs e)
	{
		SampleLog.Write($"Restored    NotesPanel: summary height {_SplitNotes.Panel2.Height}");
	}


	/// <summary>Handles the Saving event of the layout provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Layout_Saving(object? sender, EventArgs e)
	{
		SampleLog.Write($"Saving      NotesPanel: summary height {_SplitNotes.Panel2.Height}");
	}
}
