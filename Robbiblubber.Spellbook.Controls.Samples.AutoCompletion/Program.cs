namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class provides the application entry point.</summary>
[SupportedOSPlatform("windows")]
public static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Runs the application.</summary>
	[STAThread]
	public static void Main()
	{
		ApplicationConfiguration.Initialize();
		Application.Run(new MainWindow());
	}
}
