namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

using System.Runtime.Versioning;



/// <summary>This class provides a simple event log for the sample.</summary>
[SupportedOSPlatform("windows")]
public static class SampleLog
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static events                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when a log entry has been written.</summary>
	public static event EventHandler<string>? Written;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a display name for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the control name and type.</returns>
	private static string _Name(Control? control)
	{
		return ((control is null) ? "(none)" : $"{control.Name} ({control.GetType().Name})");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log entry.</summary>
	/// <param name="message">Message.</param>
	public static void Write(string message)
	{
		Written?.Invoke(null, $"{DateTime.Now:HH:mm:ss.fff}  {message}");
	}


	/// <summary>Gets the names of the commands that are currently available.</summary>
	/// <param name="provider">Edit command provider.</param>
	/// <returns>Returns a comma-separated list of command names, or "(none)".</returns>
	public static string GetAvailableCommands(EditCommandProvider provider)
	{
		List<string> rval = Enum.GetValues<EditCommand>().Where(m => provider.CanExecute(m)).Select(m => m.ToString()).ToList();
		return ((rval.Count == 0) ? "(none)" : string.Join(", ", rval));
	}


	/// <summary>Logs the events of an edit command provider.</summary>
	/// <param name="provider">Edit command provider.</param>
	public static void Attach(EditCommandProvider provider)
	{
		provider.Executing += (sender, e) =>
		{
			Write($"Executing   {e.Command} on {_Name(e.Control)}");
		};

		provider.Executed += (sender, e) =>
		{
			Write($"Executed    {e.Command} on {_Name(e.Control)}");
		};

		provider.EditStateChanged += (sender, e) =>
		{
			Write($"State       {_Name(provider.ActiveControl)}: {GetAvailableCommands(provider)}");
		};
	}
}
