namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class connects the texts of the dialogs of this library to a localization.</summary>
/// <remarks><para>Every text of <see cref="ThemedMessageBoxTexts"/>, <see cref="ThemedColorDialogTexts"/>,
///          <see cref="Themes.ThemeEditorTexts"/> and <see cref="Themes.ThemeSelectorTexts"/> has a fixed symbol, for example
///          <c>spellbook:msgbox.yes</c>, <c>spellbook:colordialog.red</c>, <c>spellbook:themeeditor.title</c> or
///          <c>spellbook:themeselector.dark</c>. A text that has been set explicitly is
///          used as it is; otherwise the dialog asks <see cref="LookUp"/> for the symbol each time it is shown, and uses the English
///          default text if no function is set.</para>
///          <para>The library itself does not depend on a localization library. To use Robbiblubber.Localization, for example:
///          <code>ControlTexts.LookUp = (symbol, defaultText) => Locale.LookUp(symbol, defaultText);</code></para></remarks>
[SupportedOSPlatform("windows")]
public static class ControlTexts
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets a function that returns the text for a symbol, NULL for the English default texts.</summary>
	/// <remarks>The function gets the symbol and the default text and returns the text to show.</remarks>
	public static Func<string, string, string>? LookUp
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a text.</summary>
	/// <param name="texts">Explicitly set texts.</param>
	/// <param name="prefix">Symbol prefix.</param>
	/// <param name="name">Name of the text.</param>
	/// <param name="defaultText">Default text.</param>
	/// <returns>Returns the text.</returns>
	internal static string _Get(Dictionary<string, string> texts, string prefix, string name, string defaultText)
	{
		if(texts.TryGetValue(name, out string? rval)) { return rval; }
		if(LookUp is null) { return defaultText; }

		try
		{
			rval = LookUp(prefix + "." + name.ToLowerInvariant(), defaultText);
		}
		catch(Exception) {}

		return (string.IsNullOrEmpty(rval) ? defaultText : rval);
	}


	/// <summary>Sets a text explicitly.</summary>
	/// <param name="texts">Explicitly set texts.</param>
	/// <param name="name">Name of the text.</param>
	/// <param name="value">Text, NULL to use the localization or the default text again.</param>
	internal static void _Set(Dictionary<string, string> texts, string name, string? value)
	{
		if(value is null) { texts.Remove(name); } else { texts[name] = value; }
	}
}
