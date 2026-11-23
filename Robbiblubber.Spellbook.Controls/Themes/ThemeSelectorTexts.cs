namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;



/// <summary>This class provides the texts of <see cref="ThemeSelector"/>.</summary>
/// <remarks>The texts are English by default. An application either sets them before the selector is shown, or connects a
///          localization through <see cref="ControlTexts.LookUp"/>; the symbols are <c>spellbook:themeselector.</c> and the lower case
///          name of the property, for example <c>spellbook:themeselector.dark</c>. An explicitly set text wins over the localization;
///          setting a text to NULL uses the localization or the default text again. The names of custom themes are shown as they
///          are.</remarks>
[SupportedOSPlatform("windows")]
public static class ThemeSelectorTexts
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Symbol prefix.</summary>
	private const string _PREFIX = "spellbook:themeselector";

	/// <summary>Explicitly set texts.</summary>
	private static readonly Dictionary<string, string> _Texts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the name of <see cref="Theme.AUTO"/>.</summary>
	[AllowNull]
	public static string Automatic
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Automatic), "Automatic"); }
		set { ControlTexts._Set(_Texts, nameof(Automatic), value); }
	}


	/// <summary>Gets or sets the name of <see cref="Theme.SYSTEM"/>.</summary>
	[AllowNull]
	public static string System
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(System), "System"); }
		set { ControlTexts._Set(_Texts, nameof(System), value); }
	}


	/// <summary>Gets or sets the name of <see cref="Theme.LIGHT"/>.</summary>
	[AllowNull]
	public static string Light
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Light), "Light"); }
		set { ControlTexts._Set(_Texts, nameof(Light), value); }
	}


	/// <summary>Gets or sets the name of <see cref="Theme.DARK"/>.</summary>
	[AllowNull]
	public static string Dark
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Dark), "Dark"); }
		set { ControlTexts._Set(_Texts, nameof(Dark), value); }
	}


	/// <summary>Gets or sets the text of the link that opens the theme editor.</summary>
	[AllowNull]
	public static string Customize
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Customize), "Customize..."); }
		set { ControlTexts._Set(_Texts, nameof(Customize), value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the display name of a theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns the localized name of a built-in theme, or the name of a custom theme.</returns>
	public static string GetName(Theme theme)
	{
		ArgumentNullException.ThrowIfNull(theme);

		if(theme == Theme.AUTO) { return Automatic; }
		if(theme == Theme.SYSTEM) { return System; }
		if(theme == Theme.LIGHT) { return Light; }
		if(theme == Theme.DARK) { return Dark; }

		return theme.Name;
	}
}
