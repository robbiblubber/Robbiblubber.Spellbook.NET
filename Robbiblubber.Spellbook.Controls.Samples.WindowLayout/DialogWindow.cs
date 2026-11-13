namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

using System.Runtime.Versioning;



/// <summary>This class implements a dialog with a fixed size; only its position is saved.</summary>
[SupportedOSPlatform("windows")]
public partial class DialogWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public DialogWindow()
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
		SampleLog.Write($"Restored    DialogWindow: {SampleLog.Format(Bounds)}");
	}
}
