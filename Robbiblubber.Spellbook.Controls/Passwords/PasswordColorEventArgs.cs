namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides arguments for the <see cref="PasswordStrengthIndicator.ColorRequested"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class PasswordColorEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="strength">Password strength.</param>
	/// <param name="color">Color.</param>
	public PasswordColorEventArgs(PasswordStrength strength, Color color)
	{
		Strength = strength;
		Color = color;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the strength the color is requested for.</summary>
	public PasswordStrength Strength
	{
		get;
	}


	/// <summary>Gets or sets the fill color.</summary>
	/// <remarks>The color is initialized with the color of the strength from the color table.</remarks>
	public Color Color
	{
		get; set;
	}
}
