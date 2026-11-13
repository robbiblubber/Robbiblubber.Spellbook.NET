namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines how a <see cref="ThemedComboBox"/> shows that it has the focus.</summary>
/// <remarks>The values can be combined.</remarks>
[Flags]
[SupportedOSPlatform("windows")]
public enum ThemedComboBoxFocusStyle: int
{
	/// <summary>The focus is not shown.</summary>
	NONE = 0,

	/// <summary>The border is drawn in the focus color.</summary>
	BORDER = 1,

	/// <summary>A dotted rectangle is drawn around the text, like in a standard combo box.</summary>
	DOTTED = 2,

	/// <summary>A line in the focus color is drawn at the bottom of the control.</summary>
	LINE = 4
}
