namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This enumeration defines the color roles of a <see cref="Theme"/>.</summary>
/// <remarks>Controls do not use theme colors directly; their color tables map every color slot to a role. Roles are stored by
///          name, so new roles can be added without breaking saved themes.</remarks>
[SupportedOSPlatform("windows")]
public enum ThemeColor: int
{
	/// <summary>Back color of windows, panels and labels.</summary>
	BACK = 0,

	/// <summary>Text color of windows, panels and labels.</summary>
	TEXT = 1,

	/// <summary>Color of secondary texts, for example details and hints.</summary>
	GRAY_TEXT = 2,

	/// <summary>Color of glyphs like drop-down arrows and check marks on neutral backgrounds.</summary>
	GLYPH = 3,

	/// <summary>Back color of input fields, lists and grids.</summary>
	FIELD_BACK = 4,

	/// <summary>Text color of input fields, lists and grids.</summary>
	FIELD_TEXT = 5,

	/// <summary>Border color of input fields.</summary>
	FIELD_BORDER = 6,

	/// <summary>Border color of input fields while the mouse is over them.</summary>
	FIELD_BORDER_HOT = 7,

	/// <summary>Border color of focused input fields.</summary>
	FIELD_BORDER_FOCUSED = 8,

	/// <summary>Back color of read-only input fields.</summary>
	READ_ONLY_BACK = 9,

	/// <summary>Text color of read-only input fields.</summary>
	READ_ONLY_TEXT = 10,

	/// <summary>Back color of buttons.</summary>
	BUTTON_BACK = 11,

	/// <summary>Text color of buttons.</summary>
	BUTTON_TEXT = 12,

	/// <summary>Border color of buttons.</summary>
	BUTTON_BORDER = 13,

	/// <summary>Back color of buttons while the mouse is over them.</summary>
	BUTTON_BACK_HOT = 14,

	/// <summary>Back color of pressed buttons.</summary>
	BUTTON_BACK_PRESSED = 15,

	/// <summary>Accent color, used for default buttons, checked boxes, progress bars and highlighted characters.</summary>
	ACCENT = 16,

	/// <summary>Text color on the accent color.</summary>
	ACCENT_TEXT = 17,

	/// <summary>Accent color while the mouse is over the element.</summary>
	ACCENT_HOT = 18,

	/// <summary>Accent color of pressed elements.</summary>
	ACCENT_PRESSED = 19,

	/// <summary>Back color of selected items.</summary>
	SELECTION_BACK = 20,

	/// <summary>Text color of selected items.</summary>
	SELECTION_TEXT = 21,

	/// <summary>Back color of selected items when the control does not have the focus.</summary>
	INACTIVE_SELECTION_BACK = 22,

	/// <summary>Text color of selected items when the control does not have the focus.</summary>
	INACTIVE_SELECTION_TEXT = 23,

	/// <summary>Back color of items while the mouse is over them.</summary>
	HOT_BACK = 24,

	/// <summary>Back color of disabled elements.</summary>
	DISABLED_BACK = 25,

	/// <summary>Text color of disabled elements.</summary>
	DISABLED_TEXT = 26,

	/// <summary>Border color of disabled elements.</summary>
	DISABLED_BORDER = 27,

	/// <summary>Back color of tracks, for example of progress meters.</summary>
	TRACK_BACK = 28,

	/// <summary>Border color of tracks.</summary>
	TRACK_BORDER = 29,

	/// <summary>Back color of drop-down lists, tool tips and other popups.</summary>
	POPUP_BACK = 30,

	/// <summary>Text color of popups.</summary>
	POPUP_TEXT = 31,

	/// <summary>Border color of popups.</summary>
	POPUP_BORDER = 32,

	/// <summary>Back color of column and row headers.</summary>
	HEADER_BACK = 33,

	/// <summary>Text color of column and row headers.</summary>
	HEADER_TEXT = 34,

	/// <summary>Color of grid lines.</summary>
	GRID_LINE = 35,

	/// <summary>Back color of menu bars and tool bars.</summary>
	MENU_BAR_BACK = 36,

	/// <summary>Back color of drop-down and context menus.</summary>
	MENU_BACK = 37,

	/// <summary>Text color of menus.</summary>
	MENU_TEXT = 38,

	/// <summary>Back color of the highlighted menu item.</summary>
	MENU_HIGHLIGHT_BACK = 39,

	/// <summary>Text color of the highlighted menu item.</summary>
	MENU_HIGHLIGHT_TEXT = 40,

	/// <summary>Border color of drop-down and context menus.</summary>
	MENU_BORDER = 41,

	/// <summary>Color of separators.</summary>
	SEPARATOR = 42,

	/// <summary>Back color of status bars.</summary>
	STATUS_BACK = 43,

	/// <summary>Text color of status bars.</summary>
	STATUS_TEXT = 44,

	/// <summary>Color of links.</summary>
	LINK = 45,

	/// <summary>Color of errors.</summary>
	ERROR = 46,

	/// <summary>Color of warnings.</summary>
	WARNING = 47,

	/// <summary>Color of success messages.</summary>
	SUCCESS = 48
}
