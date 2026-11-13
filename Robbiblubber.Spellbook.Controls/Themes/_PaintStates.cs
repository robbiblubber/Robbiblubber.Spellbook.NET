namespace Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This enumeration defines the interaction states that the control renderer draws.</summary>
[Flags]
internal enum _PaintStates: int
{
	/// <summary>Normal state.</summary>
	NONE = 0,

	/// <summary>The mouse is over the control.</summary>
	HOT = 1,

	/// <summary>The control is pressed.</summary>
	PRESSED = 2,

	/// <summary>The control has the focus and shows focus cues.</summary>
	FOCUSED = 4,

	/// <summary>The control is the default button of its form.</summary>
	DEFAULT = 8,

	/// <summary>Mnemonic underlines are hidden.</summary>
	HIDE_PREFIX = 16
}
