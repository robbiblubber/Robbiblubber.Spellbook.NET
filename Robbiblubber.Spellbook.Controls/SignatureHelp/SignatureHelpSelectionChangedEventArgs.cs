namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="SignatureHelpProvider.SelectionChanged"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class SignatureHelpSelectionChangedEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="signature">Displayed signature, NULL if no signature is displayed.</param>
	/// <param name="signatureIndex">Index of the displayed signature, -1 if no signature is displayed.</param>
	/// <param name="parameterIndex">Index of the active parameter, -1 if no parameter is active.</param>
	/// <param name="argumentIndex">Index of the argument at the caret, -1 if no signature is displayed.</param>
	public SignatureHelpSelectionChangedEventArgs(TextBoxBase control, SignatureInfo? signature, int signatureIndex, int parameterIndex, int argumentIndex): base()
	{
		Control = control;
		Signature = signature;
		SignatureIndex = signatureIndex;
		ParameterIndex = parameterIndex;
		ArgumentIndex = argumentIndex;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	public TextBoxBase Control
	{
		get;
	}


	/// <summary>Gets the displayed signature, NULL if no signature is displayed.</summary>
	public SignatureInfo? Signature
	{
		get;
	}


	/// <summary>Gets the index of the displayed signature, -1 if no signature is displayed.</summary>
	public int SignatureIndex
	{
		get;
	}


	/// <summary>Gets the index of the active parameter, -1 if no parameter is active.</summary>
	/// <remarks>No parameter is active if the signature has fewer parameters than the call has arguments.</remarks>
	public int ParameterIndex
	{
		get;
	}


	/// <summary>Gets the index of the argument at the caret, -1 if no signature is displayed.</summary>
	public int ArgumentIndex
	{
		get;
	}
}
