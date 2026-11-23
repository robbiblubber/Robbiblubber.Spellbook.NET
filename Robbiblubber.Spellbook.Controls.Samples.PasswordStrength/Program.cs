namespace Robbiblubber.Spellbook.Controls.Samples.PasswordStrength;

using System.Runtime.Versioning;



/// <summary>This class provides the application entry point.</summary>
public static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Runs the application.</summary>
	[STAThread][SupportedOSPlatform("windows")]
	public static void Main()
	{
		ApplicationConfiguration.Initialize();
		Application.Run(new MainWindow());
	}
}
