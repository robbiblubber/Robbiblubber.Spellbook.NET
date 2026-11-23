namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the styles of a <see cref="ProgressMeter"/>.</summary>
[SupportedOSPlatform("windows")]
public enum ProgressMeterStyle: int
{
	/// <summary>The bar shows the value between minimum and maximum.</summary>
	CONTINUOUS = 0,

	/// <summary>A bar segment moves through the meter continuously, for progress of unknown duration.</summary>
	MARQUEE = 1
}
