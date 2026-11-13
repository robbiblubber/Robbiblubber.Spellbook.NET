namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class provides event arguments for the <see cref="AutoCompletionProvider.Starting"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class AutoCompletionStartingEventArgs: CancelEventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="context">Auto completion context.</param>
	public AutoCompletionStartingEventArgs(AutoCompletionContext context): base()
	{
		Context = context;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the auto completion context.</summary>
	public AutoCompletionContext Context
	{
		get;
	}
}
