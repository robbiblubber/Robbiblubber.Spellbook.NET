namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how a <see cref="ThemedCheckBox"/> shows its check state.</summary>
[SupportedOSPlatform("windows")]
public enum ThemedCheckBoxStyle: int
{
	/// <summary>A box with a check mark.</summary>
	BOX = 0,

	/// <summary>A switch like in the Windows settings: a rounded track with a knob that is on the left when the check box is
	///          unchecked, on the right when it is checked and in the middle when its state is indeterminate.</summary>
	SWITCH = 1
}
