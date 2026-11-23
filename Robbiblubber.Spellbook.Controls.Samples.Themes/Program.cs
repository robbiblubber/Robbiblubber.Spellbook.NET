namespace Robbiblubber.Spellbook.Controls.Samples.Themes;

using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class provides the application entry point.</summary>
[SupportedOSPlatform("windows")]
public static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the name of the instance, NULL for the default instance.</summary>
	/// <remarks>Instances with a name keep their selected theme in their own configuration file, so several instances of the sample can
	///          act as different applications that share their themes.</remarks>
	public static string? InstanceName
	{
		get; private set;
	}


	/// <summary>Gets the theme store of the sample.</summary>
	/// <returns>Returns a shared theme store with the configuration file of the instance.</returns>
	public static SharedThemeStore CreateThemeStore()
	{
		return new SharedThemeStore(null, ((InstanceName is null) ? null : ("app." + InstanceName + ".themes")));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Runs the application.</summary>
	/// <param name="args">Command line arguments: an optional instance name.</param>
	[STAThread]
	public static void Main(string[] args)
	{
		InstanceName = ((args.Length > 0) && !string.IsNullOrWhiteSpace(args[0])) ? args[0].Trim() : null;

		ApplicationConfiguration.Initialize();
		Application.Run(new MainWindow());
	}
}
