using System.Runtime.Versioning;

namespace Robbiblubber.Spellbook.Backpack.Samples.Packing;



/// <summary>This class implements the application entry point.</summary>
[SupportedOSPlatform("windows")]
internal static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The main entry point for the application.</summary>
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();
		Application.Run(new MainWindow());
	}
}
