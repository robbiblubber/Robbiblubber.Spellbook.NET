namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides arguments for the <see cref="PasswordStrengthIndicator.Rating"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class PasswordRatingEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="password">Rated password.</param>
	/// <param name="strength">Strength determined by the built-in heuristic.</param>
	public PasswordRatingEventArgs(string password, PasswordStrength strength): base()
	{
		Password = password;
		Strength = strength;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the rated password.</summary>
	public string Password
	{
		get;
	}


	/// <summary>Gets or sets the strength.</summary>
	/// <remarks>The strength is initialized with the result of the built-in heuristic (<see cref="PasswordStrengthIndicator.Rate"/>);
	///          event handlers can replace it, for example with the result of a password policy or of another rating library.</remarks>
	public PasswordStrength Strength
	{
		get; set;
	}
}
