namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

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
	} = true;



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


	/// <summary>Logs all events of a signature help provider.</summary>
	/// <param name="provider">Signature help provider.</param>
	/// <param name="scenario">Scenario name.</param>
	public static void Attach(SignatureHelpProvider provider, string scenario)
	{
		provider.Starting += (sender, e) =>
		{
			Write(scenario, $"Starting    {e.Context.Control.Name}: trigger={e.Context.Trigger}, call={e.Context.Call}, caret={e.Context.CaretIndex}");
		};

		provider.Populating += (sender, e) =>
		{
			Write(scenario, $"Populating  {e.Context.Control.Name}: {e.Context.Call} -> {e.Signatures.Count} signatures, active={e.ActiveSignature?.ToString() ?? "-"}");
		};

		provider.SelectionChanged += (sender, e) =>
		{
			string parameter;

			if(!LogSelection) { return; }

			if(e.Signature is null)
			{
				Write(scenario, $"Selection   {e.Control.Name}: (none)");
				return;
			}

			parameter = ((e.ParameterIndex < 0) ? "(none)" : $"'{e.Signature.Parameters[e.ParameterIndex].Label}'");
			Write(scenario, $"Selection   {e.Control.Name}: #{e.SignatureIndex} {e.Signature.Name}, argument {e.ArgumentIndex}, parameter {parameter}");
		};

		provider.Closed += (sender, e) =>
		{
			Write(scenario, $"Closed      {e.Control.Name}: {e.Reason}" + ((e.Exception is null) ? string.Empty : $" ({e.Exception.GetType().Name}: {e.Exception.Message})"));
		};
	}


	/// <summary>Logs the session start and end of an auto completion provider.</summary>
	/// <param name="provider">Auto completion provider.</param>
	/// <param name="scenario">Scenario name.</param>
	public static void Attach(AutoCompletionProvider provider, string scenario)
	{
		provider.Accepted += (sender, e) =>
		{
			Write(scenario, $"Completion  {e.Control.Name}: accepted '{e.InsertText}' at {e.InsertStart}");
		};

		provider.Closed += (sender, e) =>
		{
			Write(scenario, $"Completion  {e.Control.Name}: closed ({e.Reason})");
		};
	}
}
