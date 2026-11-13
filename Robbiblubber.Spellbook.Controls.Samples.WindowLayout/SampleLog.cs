namespace Robbiblubber.Spellbook.Controls.Samples.WindowLayout;



/// <summary>This class provides a simple event log for the sample.</summary>
public static class SampleLog
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static events                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when a log entry has been written.</summary>
	public static event EventHandler<string>? Written;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Writes a log entry.</summary>
	/// <param name="message">Message.</param>
	public static void Write(string message)
	{
		Written?.Invoke(null, $"{DateTime.Now:HH:mm:ss.fff}  {message}");
	}


	/// <summary>Formats a rectangle.</summary>
	/// <param name="r">Rectangle.</param>
	/// <returns>Returns the rectangle as text.</returns>
	public static string Format(Rectangle r)
	{
		return $"{r.X}, {r.Y}, {r.Width} x {r.Height}";
	}
}
