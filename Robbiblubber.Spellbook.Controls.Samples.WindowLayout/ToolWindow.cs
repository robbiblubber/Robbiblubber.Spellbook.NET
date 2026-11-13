namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;

using System.Runtime.Versioning;



/// <summary>This class implements a sizable tool window that contains a user control with its own layout provider.</summary>
[SupportedOSPlatform("windows")]
public partial class ToolWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ToolWindow()
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
		SampleLog.Write($"Restored    ToolWindow: {SampleLog.Format(Bounds)}, {WindowState}");
	}
}
