namespace Robbiblubber.Spellbook.Controls;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;



/// <summary>This class provides the button texts of <see cref="ThemedMessageBox"/>.</summary>
/// <remarks>The texts are English by default. An application either sets them once, for example when it loads its language:
///          <code>ThemedMessageBoxTexts.Yes = "&amp;Ja";
///ThemedMessageBoxTexts.No = "&amp;Nein";</code>
///          or connects a localization through <see cref="ControlTexts.LookUp"/>; the symbols are <c>spellbook:msgbox.</c> and the
///          lower case name of the property, for example <c>spellbook:msgbox.yes</c>. An explicitly set text wins over the
///          localization; setting a text to NULL uses the localization or the default text again. An ampersand marks the mnemonic.</remarks>
[SupportedOSPlatform("windows")]
public static class ThemedMessageBoxTexts
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Symbol prefix.</summary>
	private const string _PREFIX = "spellbook:msgbox";

	/// <summary>Explicitly set texts.</summary>
	private static readonly Dictionary<string, string> _Texts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the text of the OK button.</summary>
	[AllowNull]
	public static string Ok
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Ok), "OK"); }
		set { ControlTexts._Set(_Texts, nameof(Ok), value); }
	}


	/// <summary>Gets or sets the text of the Cancel button.</summary>
	[AllowNull]
	public static string Cancel
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Cancel), "Cancel"); }
		set { ControlTexts._Set(_Texts, nameof(Cancel), value); }
	}


	/// <summary>Gets or sets the text of the Abort button.</summary>
	[AllowNull]
	public static string Abort
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Abort), "&Abort"); }
		set { ControlTexts._Set(_Texts, nameof(Abort), value); }
	}


	/// <summary>Gets or sets the text of the Retry button.</summary>
	[AllowNull]
	public static string Retry
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Retry), "&Retry"); }
		set { ControlTexts._Set(_Texts, nameof(Retry), value); }
	}


	/// <summary>Gets or sets the text of the Ignore button.</summary>
	[AllowNull]
	public static string Ignore
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Ignore), "&Ignore"); }
		set { ControlTexts._Set(_Texts, nameof(Ignore), value); }
	}


	/// <summary>Gets or sets the text of the Yes button.</summary>
	[AllowNull]
	public static string Yes
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Yes), "&Yes"); }
		set { ControlTexts._Set(_Texts, nameof(Yes), value); }
	}


	/// <summary>Gets or sets the text of the No button.</summary>
	[AllowNull]
	public static string No
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(No), "&No"); }
		set { ControlTexts._Set(_Texts, nameof(No), value); }
	}


	/// <summary>Gets or sets the text of the Try Again button.</summary>
	[AllowNull]
	public static string TryAgain
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(TryAgain), "&Try Again"); }
		set { ControlTexts._Set(_Texts, nameof(TryAgain), value); }
	}


	/// <summary>Gets or sets the text of the Continue button.</summary>
	[AllowNull]
	public static string Continue
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Continue), "&Continue"); }
		set { ControlTexts._Set(_Texts, nameof(Continue), value); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text of a button.</summary>
	/// <param name="result">Dialog result of the button.</param>
	/// <returns>Returns the text.</returns>
	internal static string _Get(DialogResult result)
	{
		return result switch
		{
			DialogResult.Cancel => Cancel,
			DialogResult.Abort => Abort,
			DialogResult.Retry => Retry,
			DialogResult.Ignore => Ignore,
			DialogResult.Yes => Yes,
			DialogResult.No => No,
			DialogResult.TryAgain => TryAgain,
			DialogResult.Continue => Continue,
			_ => Ok
		};
	}
}
