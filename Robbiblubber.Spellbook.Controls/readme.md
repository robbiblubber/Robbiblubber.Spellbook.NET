# robbiblubber.org Spellbook Controls Library

The robbiblubber.org Spellbook Controls Library provides components, controls
and utilities for Windows Forms user interfaces. Most components are added to
a form in the designer and extend existing controls, so applications gain
auto completion, edit commands, persistent window layouts or recent file lists
without changing the controls they already use.

The library targets .NET 8 and .NET 10 on Windows. Self-drawn controls support
high DPI scaling and take their colors from themes that can be switched at run
time.

The repository can be found at https://bitbucket.org/robbiblubber/robbiblubber.spellbook.net/.
API documentation is available from https://robbiblubber.lima-city.org/.
For general information see http://robbiblubber.org/.

The robbiblubber.org Spellbook Controls Library is provided under the MIT license
and may be used without restrictions under the conditions of this license
agreement.


## Components

Extender components add functionality to existing controls:

- **`AutoCompletionProvider`** shows a completion list while the user types in
  a text box. Items come from an `IAutoCompletionSource`, which may answer
  synchronously or asynchronously; matching supports prefix, contains and fuzzy
  search with highlighting, detail texts, images and descriptions.
- **`SignatureHelpProvider`** shows the signature of the function call at the
  caret, with the current parameter highlighted.
- **`EditCommandProvider`** provides Undo, Cut, Copy, Paste, Delete and Select
  All for the focused control and keeps menu items and toolbar buttons enabled
  or disabled accordingly. Adapters cover text boxes, combo boxes, list boxes,
  list views and data grid views; further adapters can be registered or
  discovered with the `[EditAdapter]` attribute. Pasting into the row for new
  records of a data grid view appends rows. A context menu with edit commands
  moves the focus to the control it is opened for, and
  `DataGridView.SelectForContextMenu` selects the right-clicked cell or row.
- **`WindowLayoutProvider`** saves and restores window position, size and
  state as well as splitter positions, list and grid columns and selected tabs.
- **`RecentFileList`** and **`RecentItemList`** maintain a list of recently used
  files or other items in a menu, with pinning, a context menu and file type
  icons.

Controls:

- **`ThemedComboBox`** is a self-drawn combo box with images, detail texts and
  descriptions. In list mode the user selects items; in combo mode the user
  types, and typing searches the items or a separate search source. The
  control supports data binding with display, value, image and detail members.
- **`ProgressMeter`** is a flat progress bar with optional text and a marquee
  style.
- **`ThemedButton`**, **`ThemedCheckBox`**, **`ThemedRadioButton`** and
  **`ThemedGroupBox`** behave like the standard controls and draw themselves
  flat with rounded corners in the colors of the theme. They can replace the
  standard controls in existing designer files; the theme adapters draw the
  standard controls the same way. With `Style = ThemedCheckBoxStyle.SWITCH`,
  a `ThemedCheckBox` is drawn as a switch like in the Windows settings (accent
  color when on, knob in the middle when indeterminate).
- **`ThemedTextBox`** is a real text box with the field colors of the theme and
  a border that changes its color when the mouse is over it and when it has the
  focus. **`ThemedTabControl`** draws flat tabs; the selected tab has a line in
  the accent color. Standard text boxes and tab controls get the same look from
  the theme adapters.
- **`ThemedMessageBox`** has the same `Show` methods as `MessageBox` and shows
  the message box in the theme of its owner (or of the active window).
- **`ThemedColorDialog`** is used like `ColorDialog` and shows basic and custom
  colors, an area for saturation and value with a hue slider, number fields for
  HSV and RGB and the hexadecimal value (which also takes names like `teal`).
  With `AllowAlpha`, the color can be transparent. The `ThemeEditorDialog`
  uses it to change colors.
- **`ThemeEditorDialog`** lets the user select the theme of the application and
  create, copy, remove, import, export and edit custom themes with a live
  preview; Cancel undoes all changes, `SaveOnOk` saves them on OK.
- **`ThemeSelector`** is a drop-down list of the themes with color samples and
  an optional link "Customize..." that opens the `ThemeEditorDialog`, for
  settings windows. With `AutoApply` (default) the selected theme is the theme
  of the application; new, renamed and removed themes appear at once.
- **`PasswordStrengthIndicator`** rates a password with a built-in heuristic
  and shows the strength as a row of colored boxes. Applications can replace
  the rating, for example to apply a password policy.

`ListControlExtensions` provides selection helpers for list boxes and list
views.


## Themes

A theme (`Theme`, namespace `Robbiblubber.Spellbook.Controls.Themes`) assigns
a color to every color role (`ThemeColor`), for example `FIELD_BACK`,
`ACCENT` or `MENU_HIGHLIGHT_BACK`. The library provides four built-in themes:

- **`SYSTEM`** uses the system colors and follows high contrast themes and the
  color mode of the application.
- **`AUTO`** follows the app mode of Windows: dark or light, and the system
  colors while a high contrast theme is active. This is the default theme of the
  application.
- **`LIGHT`** and **`DARK`** use fixed palettes independent of the Windows
  settings.

A custom theme is based on another theme and sets only the colors that differ.
`ThemeManager` holds the theme of the application and the registered custom
themes; changing `ThemeManager.Theme` repaints all controls immediately.
`ThemeManager.Load()` and `Save()` keep the custom themes and the selection in
a Ddn file (`DdnThemeStore`, behind the interface `IThemeStore`); single themes
can be exported to and imported from files.

```csharp
using Robbiblubber.Spellbook.Controls.Themes;

ThemeManager.Load();

Theme green = new("Dark green", Theme.DARK);
green.SetValue(ThemeColor.ACCENT, Color.LimeGreen);
ThemeManager.Add(green);
ThemeManager.Theme = green;

ThemeManager.Save();
```

To share the custom themes between applications, use a `SharedThemeStore`.
Every custom theme is then a file (`<id>.theme`, the export format) in
`%APPDATA%\robbiblubber\themes`, while each application keeps only its
selected theme in its own configuration file, so every application can show
another theme and all of them offer the same themes. Saving writes only the
themes changed in this application and deletes only the ones removed in it, so
changes of other applications are kept. `ThemeManager.Refresh()` takes over
the changes of other applications; the `ThemeEditorDialog` calls it when it
opens. Themes that an application has kept in its own file so far are moved to
the shared directory with the next save.

```csharp
ThemeManager.Load(new SharedThemeStore());
```

The self-drawn controls take their colors from a color table (`Colors`
property). A table uses the theme of the application unless it has its own
`Theme`, and single colors can be overridden. `BackColor`, `ForeColor` and
similar properties of earlier versions are mapped to the color table. Own
controls can use the same pattern by deriving from `ControlColorTable`: the
table maps every color slot to a role of the theme, and its `Changed` event
also occurs when the colors of the theme change.

Standard Windows Forms controls get their colors from theme adapters. Add a
**`ThemeProvider`** to a form, or call `ThemeManager.Apply(form)`, and the
theme is applied to the form and all its controls, including controls added
later, context menus, menus and tool bars (`ThemeToolStripRenderer`), the
title bar and the scroll bars. A provider or `Apply(control, theme)` can give
a window or a panel its own theme; without one it follows the theme of the
application. Colors that the application has set explicitly are kept, system
colors such as `SystemColors.Info` are mapped to the matching role, and the
original colors return when the system theme is selected or
`ThemeManager.Reset(control)` is called. Adapters exist for forms, text boxes,
list and combo boxes, buttons, check boxes, radio buttons, group boxes, link
labels, tree and list views, data grid views, property grids, up-down controls,
tab controls, tab pages and tool strips. Lists, combo boxes, tree and list views
show their selection in the selection colors of the theme, and up-down controls
get the border of a themed text box and flat buttons. Own
adapters implement `IThemeAdapter` (or derive from `ThemeAdapter<T>`) and are
registered with `ThemeManager.RegisterAdapter()` or discovered with the
`[ThemeAdapter]` attribute (`ThemeManager.Discover()`). Self-drawn controls of
other libraries implement `IThemedControl` with a `ControlColorTable` of their
own: the theme applied to their form then reaches the color table, which raises
`Changed` so the control repaints.

Single-colored dark icons in menus and tool bars are lightened in dark themes,
and so are the dark outlines of colored icons (their colors are kept);
the event `ThemeManager.ResolveImage` lets the application supply its own
images. Pinned entries of recent lists resolve their icon and the pin
separately, so an image supplied for the icon is used below the pin as well. With .NET 9 and later, `ThemeManager.SetsColorMode` lets the theme
manager also set the color mode of the application (`Application.SetColorMode`).
Windows Forms does not support changing the color mode while windows are open,
so this is off by default; enable it only if the theme is set before the first
window is shown and does not change between dark and light afterwards. Set or
load the theme before the first window is shown in any case.

```csharp
ThemeManager.Load();
Application.Run(new MainWindow());      // MainWindow has a ThemeProvider
```


Text boxes, rich text boxes, up-down controls and the text of editable combo
boxes take their selection colors from Windows, which cannot be set per
control. `ThemeManager.ThemeTextSelection` changes `GetSysColor` in the import
tables of comctl32 and msftedit in the process, so rich text boxes get the
selection colors of the theme. Text boxes, up-down controls and the text of
combo boxes get their selection colors internally; their selected text is drawn
again in the theme colors after the control has drawn it. It is on by default;
an application that does not want code that does not belong to it changed (it
may stop working with a future Windows) switches it off before the first form
is shown.


## Texts and localization

The texts of `ThemedMessageBox`, `ThemedColorDialog`, `ThemeEditorDialog` and
`ThemeSelector` are English by default. They are set in `ThemedMessageBoxTexts`,
`ThemedColorDialogTexts`, `ThemeEditorTexts` and `ThemeSelectorTexts`, or taken
from a localization through `ControlTexts.LookUp`, a function that gets a symbol
(`spellbook:msgbox.yes`, `spellbook:colordialog.red`,
`spellbook:themeeditor.title`, `spellbook:themeselector.dark`, …) and the
default text.
Explicitly set texts win; setting a text to `null` uses the localization or the
default again. The library does not depend on a localization library;
`SpellbookLocalization.Enable()` from Robbiblubber.Localization.Controls
connects the dialogs to Robbiblubber.Localization and brings language tables
for German, English, French and Indonesian.


## Persistence

`WindowLayoutProvider` and the recent lists store their data through the
interfaces `IWindowLayoutStore` and `IRecentListStore`. The default stores save
to a Ddn configuration file in the user configuration path of the application
(`PathOp.UserConfigurationPath`, e.g. `%LOCALAPPDATA%\robbiblubber.org\MyApp`),
so every user has their own lists and layouts; other storage locations are
supported by implementing these interfaces. Several windows or application
instances can share a recent list: every change is loaded, applied and saved,
so concurrent changes are not lost.


## Applications with several windows

`MultiFormApplicationContext` runs an application with several equal
top-level windows, for example one window per document, until the last window
is closed. Together with `SingleInstance` from the core library, further
starts of the application pass their arguments to the running instance, which
receives them on the user interface thread and may bring a window to the
foreground:

```csharp
using SingleInstance instance = new("robbiblubber.org/MyEditor");
if(!instance.IsFirstInstance && MultiFormApplicationContext.Forward(instance, args)) { return; }

ApplicationConfiguration.Initialize();
MultiFormApplicationContext context = new(instance);
context.ArgumentsReceived += (s, e) =>
{
    foreach(string i in e.Arguments) { context.ShowForm(new DocumentWindow(e.GetFullPath(i))); }
};
context.ShowForm(new DocumentWindow());
Application.Run(context);
```


## Example

```csharp
using Robbiblubber.Spellbook.Controls;
using Robbiblubber.Spellbook.Controls.Themes;

// A combo box in combo mode: the button shows the favorites, typing searches all cities.
ThemedComboBox city = new() { DropDownStyle = ThemedComboBoxStyle.COMBO, MatchMode = ListMatchMode.FUZZY };

city.Items.AddRange(new ListItem("Vienna") { Detail = "Austria" }, new ListItem("Berlin") { Detail = "Germany" });
city.SearchSource = new AutoCompletionItemSource(new[] { "Amsterdam", "Bern", "Budapest", "Rome", "Vienna" });
city.Colors.Theme = Theme.DARK;

city.SelectedItemChanged += (sender, e) => Console.WriteLine(city.Text);
```


## Samples

The repository contains a sample application for every component. Each sample
comes with a `TESTING.md` checklist that describes the expected behavior and
serves as a manual test plan.
