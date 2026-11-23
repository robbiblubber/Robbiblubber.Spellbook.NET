namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents an immutable snapshot of the control state a signature request is based on.</summary>
/// <remarks>Except for <see cref="Control"/>, all members can safely be accessed from background threads.</remarks>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpContext
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="text">Control text.</param>
	/// <param name="caretIndex">Caret index.</param>
	/// <param name="call">Call at the caret.</param>
	/// <param name="trigger">Trigger.</param>
	/// <param name="triggerCharacter">Trigger character, <c>'\0'</c> if the request has not been triggered by a character.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control, the text or the call is NULL.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the caret index is out of range.</exception>
	public SignatureHelpContext(TextBoxBase control, string text, int caretIndex, SignatureCall call, SignatureHelpTrigger trigger, char triggerCharacter)
	{
		Control = control ?? throw new ArgumentNullException(nameof(control));
		Text = text ?? throw new ArgumentNullException(nameof(text));
		Call = call ?? throw new ArgumentNullException(nameof(call));

		if((caretIndex < 0) || (caretIndex > text.Length)) { throw new ArgumentOutOfRangeException(nameof(caretIndex)); }

		CaretIndex = caretIndex;
		Trigger = trigger;
		TriggerCharacter = triggerCharacter;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	/// <remarks>The control must only be accessed from the UI thread.</remarks>
	public TextBoxBase Control
	{
		get;
	}


	/// <summary>Gets the complete control text.</summary>
	public string Text
	{
		get;
	}


	/// <summary>Gets the caret index.</summary>
	public int CaretIndex
	{
		get;
	}


	/// <summary>Gets the call at the caret.</summary>
	public SignatureCall Call
	{
		get;
	}


	/// <summary>Gets the function name of the call.</summary>
	public string Name
	{
		get { return Call.Name; }
	}


	/// <summary>Gets the qualifier of the call.</summary>
	public string Qualifier
	{
		get { return Call.Qualifier; }
	}


	/// <summary>Gets the zero-based index of the argument at the caret.</summary>
	public int ArgumentIndex
	{
		get { return Call.ArgumentIndex; }
	}


	/// <summary>Gets the trigger that started the request.</summary>
	public SignatureHelpTrigger Trigger
	{
		get;
	}


	/// <summary>Gets the trigger character, <c>'\0'</c> if the request has not been triggered by a character.</summary>
	public char TriggerCharacter
	{
		get;
	}
}
