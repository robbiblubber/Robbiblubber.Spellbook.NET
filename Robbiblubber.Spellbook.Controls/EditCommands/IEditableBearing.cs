namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>Classes that provide an <see cref="IEditable"/> implementation through a property implement this interface.</summary>
/// <remarks>This allows controls to delegate edit commands to another object, for example an editor wrapper, without implementing
///          <see cref="IEditable"/> themselves.</remarks>
[SupportedOSPlatform("windows")]
public interface IEditableBearing
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the object that handles edit commands, NULL if the object does not handle edit commands at the moment.</summary>
	public IEditable? Editable { get; }
}
