namespace Robbiblubber.Spellbook.Windows.Samples.Shell;

using System.Runtime.Versioning;



/// <summary>This class implements the application entry point.</summary>
[SupportedOSPlatform("windows")]
internal static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The main entry point for the application.</summary>
	/// <param name="args">Command line arguments.</param>
	[STAThread]
	private static void Main(string[] args)
	{
		ApplicationConfiguration.Initialize();
		Application.Run(new MainWindow(args));
	}
}
