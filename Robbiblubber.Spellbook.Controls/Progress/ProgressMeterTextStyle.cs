namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the texts a <see cref="ProgressMeter"/> displays.</summary>
[SupportedOSPlatform("windows")]
public enum ProgressMeterTextStyle: int
{
	/// <summary>No text is displayed.</summary>
	NONE = 0,

	/// <summary>The progress is displayed in percent, for example <c>42 %</c>.</summary>
	PERCENT = 1,

	/// <summary>The value and the maximum are displayed, for example <c>17 / 40</c>.</summary>
	VALUE = 2,

	/// <summary>The <see cref="Control.Text"/> of the meter is displayed.</summary>
	TEXT = 3
}
