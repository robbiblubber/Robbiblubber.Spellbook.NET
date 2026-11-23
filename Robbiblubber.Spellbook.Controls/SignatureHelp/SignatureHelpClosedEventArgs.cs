namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="SignatureHelpProvider.Closed"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpClosedEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="reason">Close reason.</param>
	/// <param name="exception">Exception that caused the session to end, NULL if the session has not ended because of an error.</param>
	public SignatureHelpClosedEventArgs(TextBoxBase control, SignatureHelpCloseReason reason, Exception? exception = null): base()
	{
		Control = control;
		Reason = reason;
		Exception = exception;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	public TextBoxBase Control
	{
		get;
	}


	/// <summary>Gets the close reason.</summary>
	public SignatureHelpCloseReason Reason
	{
		get;
	}


	/// <summary>Gets the exception that caused the session to end, NULL if the session has not ended because of an error.</summary>
	public Exception? Exception
	{
		get;
	}
}
