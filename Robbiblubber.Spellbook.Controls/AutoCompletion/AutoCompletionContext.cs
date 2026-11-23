namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class represents an immutable snapshot of the control state an auto completion request is based on.</summary>
/// <remarks>Instances of this class only contain the control reference and immutable values. Except for <see cref="Control"/>,
///          all members can safely be accessed from background threads.</remarks>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionContext
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="text">Control text.</param>
	/// <param name="caretIndex">Caret index.</param>
	/// <param name="wordStart">Start index of the word at the caret.</param>
	/// <param name="qualifier">Qualifier.</param>
	/// <param name="trigger">Trigger.</param>
	/// <param name="triggerCharacter">Trigger character.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control or the text is NULL.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the caret index or the word start are out of range.</exception>
	public AutoCompletionContext(TextBoxBase control, string text, int caretIndex, int wordStart, string qualifier, AutoCompletionTrigger trigger, char triggerCharacter)
	{
		Control = control ?? throw new ArgumentNullException(nameof(control));
		Text = text ?? throw new ArgumentNullException(nameof(text));

		if((caretIndex < 0) || (caretIndex > text.Length)) { throw new ArgumentOutOfRangeException(nameof(caretIndex)); }
		if((wordStart < 0) || (wordStart > caretIndex)) { throw new ArgumentOutOfRangeException(nameof(wordStart)); }

		CaretIndex = caretIndex;
		WordStart = wordStart;
		Word = text.Substring(wordStart, caretIndex - wordStart);
		Qualifier = qualifier ?? string.Empty;
		Trigger = trigger;
		TriggerCharacter = triggerCharacter;

		int lineStart = (caretIndex == 0) ? 0 : (text.LastIndexOf('\n', caretIndex - 1) + 1);
		Line = text.Substring(lineStart, caretIndex - lineStart).TrimEnd('\r');
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


	/// <summary>Gets the start index of the word at the caret.</summary>
	public int WordStart
	{
		get;
	}


	/// <summary>Gets the part of the word at the caret that precedes the caret.</summary>
	public string Word
	{
		get;
	}


	/// <summary>Gets the text of the current line that precedes the caret.</summary>
	public string Line
	{
		get;
	}


	/// <summary>Gets the qualifier of the current word.</summary>
	/// <remarks>The qualifier is the dot-separated chain of words immediately preceding the current word, without the trailing dot.
	///          For <c>dbo.Customers.Na</c> the qualifier is <c>dbo.Customers</c>. If the word is not qualified, the qualifier is an
	///          empty string.</remarks>
	public string Qualifier
	{
		get;
	}


	/// <summary>Gets the trigger that started the request.</summary>
	public AutoCompletionTrigger Trigger
	{
		get;
	}


	/// <summary>Gets the trigger character.</summary>
	/// <remarks>For triggers other than <see cref="AutoCompletionTrigger.TRIGGER_CHARACTER"/> and <see cref="AutoCompletionTrigger.TYPING"/>,
	///          this property is <c>'\0'</c>.</remarks>
	public char TriggerCharacter
	{
		get;
	}
}
