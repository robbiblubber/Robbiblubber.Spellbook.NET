# robbiblubber.org Spellbook Windows Support Library

The robbiblubber.org Spellbook Windows Support Library provides classes for
Windows-specific operations: creating and reading shortcuts, registering file
types, registering applications in the installed apps list and accessing ini
files.

The library targets .NET 8 and .NET 10 on Windows (`net8.0-windows`,
`net10.0-windows`) and builds with the .NET SDK (`dotnet build`); Visual Studio
is not required.

The repository can be found at https://bitbucket.org/robbiblubber/robbiblubber.spellbook.net/.
API documentation is available from https://robbiblubber.lima-city.org/.
For general information see http://robbiblubber.org/.

The robbiblubber.org Spellbook Windows Support Library is provided under the
MIT license and may be used without restrictions under the conditions of this
license agreement.


## Shortcuts

```csharp
using Robbiblubber.Spellbook.Windows;

string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
Shortcut.Create(Path.Combine(desktop, "rqoon.lnk"), new ShortcutInfo(Environment.ProcessPath!)
{
    Arguments = "--start",
    Description = "rqoon",
    WindowStyle = ShortcutWindowStyle.MAXIMIZED
});

ShortcutInfo info = Shortcut.Read(Path.Combine(desktop, "rqoon.lnk"));
```

`ShortcutInfo` covers target, arguments, working directory (default: folder of
the target), description, icon, window style and "Run as administrator".
`Create()` creates missing folders and replaces an existing shortcut only after
the new one has been written.


## File types

```csharp
FileAssociation.Register(new FileAssociation.FileType(".rqn", "Robbiblubber.Rqoon.Document", Environment.ProcessPath!)
{
    Description = "rqoon Document",
    Verbs = [new FileAssociation.Verb("print") { Arguments = "/print \"%1\"" }]
});

FileAssociation.Unregister(".rqn", "Robbiblubber.Rqoon.Document");
```

- `Register()` writes the program ID with icon and verbs and adds it to the
  "Open with" list of the extension. It becomes the default for the extension
  only if there is no default yet, the default is the same program ID or a
  program ID that no longer exists.
- `Unregister()` removes only the program ID and its references; other
  applications registered for the extension are not affected.
- `IsRegistered()` and `GetProgId()` query registrations.
- Since Windows 8 the user's choice of the default app always wins and cannot
  be changed by applications. `OpenDefaultAppsSettings()` opens the settings
  where the user chooses.

To list an application with its file types in the default apps settings, where
the user can make it the default for all its file types at once:

```csharp
FileAssociation.RegisterApplication(new FileAssociation.ApplicationCapabilities("Robbiblubber.Rqoon", "rqoon")
{
    Description = "rqoon document editor",
    FileTypes = [new FileAssociation.FileType(".rqn", "Robbiblubber.Rqoon.Document", Environment.ProcessPath!) { Description = "rqoon Document" }]
});

FileAssociation.OpenDefaultAppsSettings("Robbiblubber.Rqoon");    // opens the page of the application
FileAssociation.UnregisterApplication("Robbiblubber.Rqoon");      // also unregisters the file types
```

The capabilities are stored under `Software\<name>\Capabilities` and listed under
`Software\RegisteredApplications`.


## Installed applications

```csharp
ApplicationRegistration.Register(new InstalledApplication("Robbiblubber.Rqoon", "rqoon", new Version(4, 0, 1))
{
    Publisher = "robbiblubber.org",
    InstallLocation = AppContext.BaseDirectory,
    DisplayIcon = Environment.ProcessPath,
    UninstallCommand = $"\"{Path.Combine(AppContext.BaseDirectory, "uninstall.exe")}\""
});

InstalledApplication? app = ApplicationRegistration.Get("Robbiblubber.Rqoon");
```

`Register()` replaces an existing entry completely. Without `ModifyCommand`, the
"Modify" command is hidden. `Unregister()`, `IsRegistered()` and `Get()` work
with entries of other installers as well.


## Scopes

`FileAssociation` and `ApplicationRegistration` take a `RegistrationScope`:
`CURRENT_USER` (default, no administrator rights required) or `LOCAL_MACHINE`
(all users, requires administrator rights; otherwise an
`UnauthorizedAccessException` is thrown). Names of keys, program IDs and
extensions are checked, so an empty or malformed name never affects other
registry keys.


## Ini files

```csharp
IniFile ini = new("settings.ini");                 // relative to the current directory
int width = ini.GetInt32("window", "width", 800);
ini.SetValue("window", "width", 1024);             // written immediately (AutoSave)

IniFile batch = new("settings.ini") { AutoSave = false };
batch.SetValue("general", "name", "rqoon");
batch.SetValue("general", "started", true);
batch.Save();
```

`IniFile` is a managed implementation that follows the syntax of the Windows
profile functions: names are not case-sensitive, the first occurrence wins,
white space around names and values is ignored, quotes around values are
removed and lines starting with `;` or `#` are comments. The file is read once;
comments, order and formatting of unchanged lines are kept when it is written.

- `GetString()`, `TryGetString()`, `GetInt16()`, `GetInt32()`, `GetInt64()`,
  `GetDouble()`, `GetBoolean()` and `GetBinary()` (Base64) return the default
  value if a key is missing or invalid. Numbers use the invariant culture.
  `GetBoolean()` accepts `true`/`false`, `yes`/`no`, `on`/`off` and numbers.
- `SetValue()` for strings, numbers, booleans (`true`/`false`) and byte arrays;
  `SetIntBoolean()` writes `1`/`0`. A NULL string deletes the key.
- `GetSections()`, `GetKeys()`, `ContainsSection()`, `ContainsKey()`,
  `DeleteKey()`, `DeleteSection()`, `Save()` and `Reload()`.
- The encoding is detected (UTF-8 with or without BOM, UTF-16, otherwise the
  ANSI code page) and kept when saving; new files are UTF-8 without BOM.
