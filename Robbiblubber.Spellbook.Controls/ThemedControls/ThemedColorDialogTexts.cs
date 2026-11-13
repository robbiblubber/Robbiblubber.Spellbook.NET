namespace Robbiblubber.Spellbook.Controls;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;



/// <summary>This class provides the texts of <see cref="ThemedColorDialog"/>.</summary>
/// <remarks>The texts are English by default. An application either sets them once, for example when it loads its language:
///          <code>ThemedColorDialogTexts.Title = "Farbe";
///ThemedColorDialogTexts.Red = "&amp;Rot:";</code>
///          or connects a localization through <see cref="ControlTexts.LookUp"/>; the symbols are <c>spellbook:colordialog.</c> and
///          the lower case name of the property, for example <c>spellbook:colordialog.red</c>. An explicitly set text wins over the
///          localization; setting a text to NULL uses the localization or the default text again. An ampersand marks the mnemonic.</remarks>
[SupportedOSPlatform("windows")]
public static class ThemedColorDialogTexts
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Symbol prefix.</summary>
	private const string _PREFIX = "spellbook:colordialog";

	/// <summary>Explicitly set texts.</summary>
	private static readonly Dictionary<string, string> _Texts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the text of the title of the dialog.</summary>
	[AllowNull]
	public static string Title
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Title), "Color"); }
		set { ControlTexts._Set(_Texts, nameof(Title), value); }
	}


	/// <summary>Gets or sets the text of the label of the basic colors.</summary>
	[AllowNull]
	public static string BasicColors
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(BasicColors), "&Basic colors:"); }
		set { ControlTexts._Set(_Texts, nameof(BasicColors), value); }
	}


	/// <summary>Gets or sets the text of the label of the custom colors.</summary>
	[AllowNull]
	public static string CustomColors
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(CustomColors), "&Custom colors:"); }
		set { ControlTexts._Set(_Texts, nameof(CustomColors), value); }
	}


	/// <summary>Gets or sets the text of the button that adds the color to the custom colors.</summary>
	[AllowNull]
	public static string AddCustomColor
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(AddCustomColor), "&Add to custom colors"); }
		set { ControlTexts._Set(_Texts, nameof(AddCustomColor), value); }
	}


	/// <summary>Gets or sets the text of the label of the hue.</summary>
	[AllowNull]
	public static string Hue
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Hue), "Hu&e:"); }
		set { ControlTexts._Set(_Texts, nameof(Hue), value); }
	}


	/// <summary>Gets or sets the text of the label of the saturation.</summary>
	[AllowNull]
	public static string Saturation
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Saturation), "&Sat:"); }
		set { ControlTexts._Set(_Texts, nameof(Saturation), value); }
	}


	/// <summary>Gets or sets the text of the label of the value.</summary>
	[AllowNull]
	public static string Value
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Value), "&Val:"); }
		set { ControlTexts._Set(_Texts, nameof(Value), value); }
	}


	/// <summary>Gets or sets the text of the label of the red component.</summary>
	[AllowNull]
	public static string Red
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Red), "&Red:"); }
		set { ControlTexts._Set(_Texts, nameof(Red), value); }
	}


	/// <summary>Gets or sets the text of the label of the green component.</summary>
	[AllowNull]
	public static string Green
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Green), "&Green:"); }
		set { ControlTexts._Set(_Texts, nameof(Green), value); }
	}


	/// <summary>Gets or sets the text of the label of the blue component.</summary>
	[AllowNull]
	public static string Blue
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Blue), "Bl&ue:"); }
		set { ControlTexts._Set(_Texts, nameof(Blue), value); }
	}


	/// <summary>Gets or sets the text of the label of the alpha component.</summary>
	[AllowNull]
	public static string Alpha
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Alpha), "Al&pha:"); }
		set { ControlTexts._Set(_Texts, nameof(Alpha), value); }
	}


	/// <summary>Gets or sets the text of the label of the hexadecimal value.</summary>
	[AllowNull]
	public static string Hex
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Hex), "He&x:"); }
		set { ControlTexts._Set(_Texts, nameof(Hex), value); }
	}


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
}
