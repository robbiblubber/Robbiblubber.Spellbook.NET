namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;



/// <summary>This class provides the texts of <see cref="ThemeEditorDialog"/>.</summary>
/// <remarks>The texts are English by default. An application either sets them before the dialog is created, or connects a
///          localization through <see cref="ControlTexts.LookUp"/>; the symbols are <c>spellbook:themeeditor.</c> and the lower case
///          name of the property, for example <c>spellbook:themeeditor.title</c>. An explicitly set text wins over the localization;
///          setting a text to NULL uses the localization or the default text again.
///          An ampersand marks the mnemonic. The names of the color roles have the symbols <c>spellbook:themeeditor.role.</c> and the
///          lower case role (<c>spellbook:themeeditor.role.field_border_hot</c>); they are derived from <see cref="ThemeColor"/> if
///          neither <see cref="RoleName"/> nor the localization returns a name.</remarks>
[SupportedOSPlatform("windows")]
public static class ThemeEditorTexts
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Symbol prefix.</summary>
	private const string _PREFIX = "spellbook:themeeditor";

	/// <summary>Explicitly set texts.</summary>
	private static readonly Dictionary<string, string> _Texts = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the window title.</summary>
	[AllowNull]
	public static string Title
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Title), "Themes"); }
		set { ControlTexts._Set(_Texts, nameof(Title), value); }
	}


	/// <summary>Gets or sets the label of the theme list.</summary>
	[AllowNull]
	public static string Themes
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Themes), "&Themes:"); }
		set { ControlTexts._Set(_Texts, nameof(Themes), value); }
	}


	/// <summary>Gets or sets the button that creates a theme.</summary>
	[AllowNull]
	public static string New
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(New), "&New"); }
		set { ControlTexts._Set(_Texts, nameof(New), value); }
	}


	/// <summary>Gets or sets the button that copies a theme.</summary>
	[AllowNull]
	public static string Copy
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Copy), "Co&py"); }
		set { ControlTexts._Set(_Texts, nameof(Copy), value); }
	}


	/// <summary>Gets or sets the button that removes a theme.</summary>
	[AllowNull]
	public static string Remove
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Remove), "&Remove"); }
		set { ControlTexts._Set(_Texts, nameof(Remove), value); }
	}


	/// <summary>Gets or sets the import button.</summary>
	[AllowNull]
	public static string Import
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Import), "&Import..."); }
		set { ControlTexts._Set(_Texts, nameof(Import), value); }
	}


	/// <summary>Gets or sets the export button.</summary>
	[AllowNull]
	public static string Export
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Export), "&Export..."); }
		set { ControlTexts._Set(_Texts, nameof(Export), value); }
	}


	/// <summary>Gets or sets the label of the name.</summary>
	[AllowNull]
	public static string Name
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Name), "N&ame:"); }
		set { ControlTexts._Set(_Texts, nameof(Name), value); }
	}


	/// <summary>Gets or sets the label of the base theme.</summary>
	[AllowNull]
	public static string Base
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Base), "&Based on:"); }
		set { ControlTexts._Set(_Texts, nameof(Base), value); }
	}


	/// <summary>Gets or sets the label of the color list.</summary>
	[AllowNull]
	public static string Colors
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Colors), "&Colors:"); }
		set { ControlTexts._Set(_Texts, nameof(Colors), value); }
	}


	/// <summary>Gets or sets the column header of the roles.</summary>
	[AllowNull]
	public static string ColumnRole
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ColumnRole), "Role"); }
		set { ControlTexts._Set(_Texts, nameof(ColumnRole), value); }
	}


	/// <summary>Gets or sets the column header of the colors.</summary>
	[AllowNull]
	public static string ColumnValue
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ColumnValue), "Color"); }
		set { ControlTexts._Set(_Texts, nameof(ColumnValue), value); }
	}


	/// <summary>Gets or sets the column header that shows if a color is set in the theme.</summary>
	[AllowNull]
	public static string ColumnSet
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ColumnSet), "Set"); }
		set { ControlTexts._Set(_Texts, nameof(ColumnSet), value); }
	}


	/// <summary>Gets or sets the text that marks a color as set in the theme.</summary>
	[AllowNull]
	public static string IsSet
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(IsSet), "yes"); }
		set { ControlTexts._Set(_Texts, nameof(IsSet), value); }
	}


	/// <summary>Gets or sets the button that changes a color.</summary>
	[AllowNull]
	public static string ChangeColor
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ChangeColor), "C&hange..."); }
		set { ControlTexts._Set(_Texts, nameof(ChangeColor), value); }
	}


	/// <summary>Gets or sets the button that resets a color.</summary>
	[AllowNull]
	public static string ResetColor
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ResetColor), "Reset c&olor"); }
		set { ControlTexts._Set(_Texts, nameof(ResetColor), value); }
	}


	/// <summary>Gets or sets the button that resets all colors.</summary>
	[AllowNull]
	public static string ResetAll
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ResetAll), "Reset a&ll"); }
		set { ControlTexts._Set(_Texts, nameof(ResetAll), value); }
	}


	/// <summary>Gets or sets the hint shown for built-in themes.</summary>
	[AllowNull]
	public static string BuiltIn
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(BuiltIn), "Built-in themes cannot be changed. Create a new theme based on it."); }
		set { ControlTexts._Set(_Texts, nameof(BuiltIn), value); }
	}


	/// <summary>Gets or sets the caption of the preview.</summary>
	[AllowNull]
	public static string Preview
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Preview), "Preview"); }
		set { ControlTexts._Set(_Texts, nameof(Preview), value); }
	}


	/// <summary>Gets or sets the text of the preview label.</summary>
	[AllowNull]
	public static string PreviewLabel
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewLabel), "Label"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewLabel), value); }
	}


	/// <summary>Gets or sets the text of the preview text box.</summary>
	[AllowNull]
	public static string PreviewTextBox
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewTextBox), "Text box"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewTextBox), value); }
	}


	/// <summary>Gets or sets the text of the preview combo box.</summary>
	[AllowNull]
	public static string PreviewComboBox
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewComboBox), "Combo box"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewComboBox), value); }
	}


	/// <summary>Gets or sets the text of the preview check box.</summary>
	[AllowNull]
	public static string PreviewCheckBox
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewCheckBox), "Check box"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewCheckBox), value); }
	}


	/// <summary>Gets or sets the text of the preview radio button.</summary>
	[AllowNull]
	public static string PreviewOption
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewOption), "Option"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewOption), value); }
	}


	/// <summary>Gets or sets the text of the preview button.</summary>
	[AllowNull]
	public static string PreviewButton
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewButton), "Button"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewButton), value); }
	}


	/// <summary>Gets or sets the text of the preview disabled button.</summary>
	[AllowNull]
	public static string PreviewDisabled
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewDisabled), "Disabled"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewDisabled), value); }
	}


	/// <summary>Gets or sets the text of the preview list items; {0} is the number of the item.</summary>
	[AllowNull]
	public static string PreviewItem
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewItem), "Item {0}"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewItem), value); }
	}


	/// <summary>Gets or sets the text of the preview selected list item.</summary>
	[AllowNull]
	public static string PreviewSelectedItem
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewSelectedItem), "Selected item"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewSelectedItem), value); }
	}


	/// <summary>Gets or sets the text of the preview first tab page.</summary>
	[AllowNull]
	public static string PreviewPage
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewPage), "Page"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewPage), value); }
	}


	/// <summary>Gets or sets the text of the preview second tab page.</summary>
	[AllowNull]
	public static string PreviewOtherPage
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewOtherPage), "Other page"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewOtherPage), value); }
	}


	/// <summary>Gets or sets the text of the preview disabled text box.</summary>
	[AllowNull]
	public static string PreviewDisabledText
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewDisabledText), "Disabled text box"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewDisabledText), value); }
	}


	/// <summary>Gets or sets the text of the preview link.</summary>
	[AllowNull]
	public static string PreviewLink
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(PreviewLink), "Link"); }
		set { ControlTexts._Set(_Texts, nameof(PreviewLink), value); }
	}


	/// <summary>Gets or sets the OK button.</summary>
	[AllowNull]
	public static string Ok
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Ok), "OK"); }
		set { ControlTexts._Set(_Texts, nameof(Ok), value); }
	}


	/// <summary>Gets or sets the Cancel button.</summary>
	[AllowNull]
	public static string Cancel
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(Cancel), "Cancel"); }
		set { ControlTexts._Set(_Texts, nameof(Cancel), value); }
	}


	/// <summary>Gets or sets the name of a new theme; {0} is a number.</summary>
	[AllowNull]
	public static string NewThemeName
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(NewThemeName), "New theme {0}"); }
		set { ControlTexts._Set(_Texts, nameof(NewThemeName), value); }
	}


	/// <summary>Gets or sets the name of a copied theme; {0} is the name of the original.</summary>
	[AllowNull]
	public static string CopyName
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(CopyName), "{0} (copy)"); }
		set { ControlTexts._Set(_Texts, nameof(CopyName), value); }
	}


	/// <summary>Gets or sets the file filter of the import and export dialogs.</summary>
	[AllowNull]
	public static string FileFilter
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(FileFilter), "Theme files (*.theme)|*.theme|All files (*.*)|*.*"); }
		set { ControlTexts._Set(_Texts, nameof(FileFilter), value); }
	}


	/// <summary>Gets or sets the message when an import fails; {0} is the error message.</summary>
	[AllowNull]
	public static string ImportFailed
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(ImportFailed), "The theme could not be imported.\r\n\r\n{0}"); }
		set { ControlTexts._Set(_Texts, nameof(ImportFailed), value); }
	}


	/// <summary>Gets or sets the color group of general colors.</summary>
	[AllowNull]
	public static string GroupGeneral
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupGeneral), "General"); }
		set { ControlTexts._Set(_Texts, nameof(GroupGeneral), value); }
	}


	/// <summary>Gets or sets the color group of input fields.</summary>
	[AllowNull]
	public static string GroupFields
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupFields), "Input fields"); }
		set { ControlTexts._Set(_Texts, nameof(GroupFields), value); }
	}


	/// <summary>Gets or sets the color group of buttons.</summary>
	[AllowNull]
	public static string GroupButtons
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupButtons), "Buttons"); }
		set { ControlTexts._Set(_Texts, nameof(GroupButtons), value); }
	}


	/// <summary>Gets or sets the color group of the accent color.</summary>
	[AllowNull]
	public static string GroupAccent
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupAccent), "Accent"); }
		set { ControlTexts._Set(_Texts, nameof(GroupAccent), value); }
	}


	/// <summary>Gets or sets the color group of selections.</summary>
	[AllowNull]
	public static string GroupSelection
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupSelection), "Selection"); }
		set { ControlTexts._Set(_Texts, nameof(GroupSelection), value); }
	}


	/// <summary>Gets or sets the color group of disabled controls.</summary>
	[AllowNull]
	public static string GroupDisabled
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupDisabled), "Disabled controls"); }
		set { ControlTexts._Set(_Texts, nameof(GroupDisabled), value); }
	}


	/// <summary>Gets or sets the color group of lists and grids.</summary>
	[AllowNull]
	public static string GroupLists
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupLists), "Lists and grids"); }
		set { ControlTexts._Set(_Texts, nameof(GroupLists), value); }
	}


	/// <summary>Gets or sets the color group of popups and menus.</summary>
	[AllowNull]
	public static string GroupMenus
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupMenus), "Popups and menus"); }
		set { ControlTexts._Set(_Texts, nameof(GroupMenus), value); }
	}


	/// <summary>Gets or sets the color group of the status bar and status colors.</summary>
	[AllowNull]
	public static string GroupStatus
	{
		get { return ControlTexts._Get(_Texts, _PREFIX, nameof(GroupStatus), "Status"); }
		set { ControlTexts._Set(_Texts, nameof(GroupStatus), value); }
	}


	/// <summary>Gets or sets a function that returns the display name of a color role, NULL for names derived from the role
	///          (<c>FIELD_BORDER_HOT</c> is shown as "Field border hot").</summary>
	public static Func<ThemeColor, string>? RoleName
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the display name of a color role.</summary>
	/// <param name="color">Color role.</param>
	/// <returns>Returns the name.</returns>
	internal static string _GetRoleName(ThemeColor color)
	{
		if(RoleName is not null) { return RoleName(color); }

		string rval = color.ToString().Replace('_', ' ').ToLowerInvariant();
		rval = (char.ToUpperInvariant(rval[0]) + rval[1..]);

		return ControlTexts._Get(_Texts, _PREFIX, "role." + color.ToString(), rval);
	}
}
