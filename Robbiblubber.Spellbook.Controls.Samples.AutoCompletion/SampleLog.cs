namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class provides a simple event log for the sample scenarios.</summary>
[SupportedOSPlatform("windows")]
public static class SampleLog
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static events                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when a log entry has been written.</summary>
	public static event EventHandler<string>? Written;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if selection changes are logged.</summary>
	public static bool LogSelection
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log entry.</summary>
	/// <param name="scenario">Scenario name.</param>
	/// <param name="message">Message.</param>
	public static void Write(string scenario, string message)
	{
		Written?.Invoke(null, $"{DateTime.Now:HH:mm:ss.fff}  [{scenario}]  {message}");
	}


	/// <summary>Logs all events of an auto completion provider.</summary>
	/// <param name="provider">Auto completion provider.</param>
	/// <param name="scenario">Scenario name.</param>
	public static void Attach(AutoCompletionProvider provider, string scenario)
	{
		provider.Starting += (sender, e) =>
		{
			Write(scenario, $"Starting    {e.Context.Control.Name}: trigger={e.Context.Trigger}, word='{e.Context.Word}', qualifier='{e.Context.Qualifier}', line='{e.Context.Line}'");
		};

		provider.Populating += (sender, e) =>
		{
			Write(scenario, $"Populating  {e.Context.Control.Name}: {e.Items.Count} items, incomplete={e.IsIncomplete}, match={e.MatchMode?.ToString() ?? "-"}, replaceStart={e.ReplaceStart?.ToString() ?? "-"}");
		};

		provider.Accepting += (sender, e) =>
		{
			Write(scenario, $"Accepting   {e.Control.Name}: '{e.Item.Text}' -> '{e.InsertText}' at {e.ReplaceStart} (replacing {e.ReplaceLength} chars)");
		};

		provider.Accepted += (sender, e) =>
		{
			Write(scenario, $"Accepted    {e.Control.Name}: '{e.InsertText}' at {e.InsertStart}");
		};

		provider.SelectionChanged += (sender, e) =>
		{
			if(LogSelection) { Write(scenario, $"Selection   {e.Control.Name}: " + ((e.Item is null) ? "(none)" : $"'{e.Item.Text}'" + ((e.Item.Category is null) ? string.Empty : $" [{e.Item.Category}]"))); }
		};

		provider.Closed += (sender, e) =>
		{
			Write(scenario, $"Closed      {e.Control.Name}: {e.Reason}" + ((e.Exception is null) ? string.Empty : $" ({e.Exception.GetType().Name}: {e.Exception.Message})"));
		};
	}
}
