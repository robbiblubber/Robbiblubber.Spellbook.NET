namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.Closed"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionClosedEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="reason">Close reason.</param>
	/// <param name="exception">Exception that caused the session to end.</param>
	public AutoCompletionClosedEventArgs(TextBoxBase control, AutoCompletionCloseReason reason, Exception? exception = null): base()
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


	/// <summary>Gets the reason the session has ended.</summary>
	public AutoCompletionCloseReason Reason
	{
		get;
	}


	/// <summary>Gets the exception that caused the session to end.</summary>
	/// <remarks>This property is only set if <see cref="Reason"/> is <see cref="AutoCompletionCloseReason.ERROR"/>.</remarks>
	public Exception? Exception
	{
		get;
	}
}
