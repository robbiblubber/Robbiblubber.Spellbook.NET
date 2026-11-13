namespace Robbiblubber.Spellbook.Windows;

using System.Diagnostics;
using System.Runtime.Versioning;

using Microsoft.Win32;



/// <summary>This class registers file types with Windows.</summary>
/// <remarks><para>A registration adds the program ID with its icon and verbs and lists it in the "Open with" menu of the
///          extension. It becomes the default for the extension only if the extension has no default yet, already points to the
///          same program ID or points to a program ID that no longer exists.</para>
///          <para>Since Windows 8, the user's choice in "Open with" or in the default apps settings always wins and cannot be
///          changed by applications. <see cref="OpenDefaultAppsSettings()"/> opens the settings so the user can choose.
///          <see cref="RegisterApplication"/> lists an application with its file types in the default apps settings.</para></remarks>
[SupportedOSPlatform("windows")]
public static class FileAssociation
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registry path of the classes key.</summary>
	private const string _CLASSES = @"Software\Classes\";

	/// <summary>Name of the open with list key.</summary>
	private const string _OPEN_WITH = "OpenWithProgids";

	/// <summary>Registry path of the registered applications list.</summary>
	private const string _REGISTERED_APPS = @"Software\RegisteredApplications";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Normalizes and checks a file extension.</summary>
	/// <param name="extension">Extension, with or without leading dot.</param>
	/// <param name="paramName">Parameter name.</param>
	/// <returns>Returns the extension with leading dot.</returns>
	/// <exception cref="ArgumentException">Thrown when the extension is invalid.</exception>
	internal static string _NormalizeExtension(string extension, string paramName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(extension, paramName);

		string rval = (extension.StartsWith('.') ? extension : ('.' + extension));
		if((rval.Length < 2) || rval[1..].Contains('.') || rval.Any(m => char.IsWhiteSpace(m) || (m == '\\') || (m == '/') || (m == '*') || (m == '?')))
		{
			throw new ArgumentException($"\"{extension}\" is not a valid file extension.", paramName);
		}

		return rval;
	}


	/// <summary>Checks a program ID.</summary>
	/// <param name="progId">Program ID.</param>
	/// <param name="paramName">Parameter name.</param>
	/// <exception cref="ArgumentException">Thrown when the program ID is invalid.</exception>
	internal static void _CheckProgId(string progId, string paramName)
	{
		_Registry._CheckName(progId, paramName);
		if(progId.StartsWith('.')) { throw new ArgumentException("The program ID must not start with a dot.", paramName); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the registry path of the application key that contains the capabilities.</summary>
	/// <param name="name">Registration name.</param>
	/// <returns>Returns the path relative to the scope root.</returns>
	private static string _AppPath(string name)
	{
		return @"Software\" + name;
	}


	/// <summary>Gets the registry path of the capabilities key of an application.</summary>
	/// <param name="name">Registration name.</param>
	/// <returns>Returns the path relative to the scope root.</returns>
	private static string _CapabilitiesPath(string name)
	{
		return _AppPath(name) + @"\Capabilities";
	}


	/// <summary>Returns the command line for an application.</summary>
	/// <param name="application">Application path.</param>
	/// <param name="arguments">Arguments.</param>
	/// <returns>Returns the command line.</returns>
	private static string _Command(string application, string arguments)
	{
		return (string.IsNullOrWhiteSpace(arguments) ? $"\"{application}\"" : $"\"{application}\" {arguments}");
	}


	/// <summary>Determines if a program ID exists in any scope.</summary>
	/// <param name="progId">Program ID.</param>
	/// <returns>Returns TRUE if the program ID exists, otherwise returns FALSE.</returns>
	private static bool _ProgIdExists(string progId)
	{
		if(string.IsNullOrWhiteSpace(progId) || progId.Contains('\\')) { return false; }

		using RegistryKey? k = Registry.ClassesRoot.OpenSubKey(progId);
		return (k is not null);
	}


	/// <summary>Determines if a registry key is empty.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns TRUE if the key has no values and no subkeys, otherwise returns FALSE.</returns>
	private static bool _IsEmpty(RegistryKey key)
	{
		return ((key.ValueCount == 0) && (key.SubKeyCount == 0));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registers a file type.</summary>
	/// <param name="type">File type.</param>
	/// <param name="scope">Scope.</param>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access, for example when registering for
	///                                                all users without administrator rights.</exception>
	/// <remarks>An existing registration of the program ID is replaced. The program ID becomes the default for the extension if
	///          Windows (both scopes merged) has no default for it yet, the default is the same program ID or a program ID that no
	///          longer exists.</remarks>
	public static void Register(FileType type, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		ArgumentNullException.ThrowIfNull(type);

		_Registry._Delete(scope, _CLASSES + type.ProgId);
		using(RegistryKey prog = _Registry._Create(scope, _CLASSES + type.ProgId))
		{
			if(type.Description is not null) { prog.SetValue(string.Empty, type.Description); }

			using(RegistryKey icon = prog.CreateSubKey("DefaultIcon"))
			{
				icon.SetValue(string.Empty, $"\"{(type.IconPath ?? type.ApplicationPath)}\",{type.IconIndex}");
			}

			using RegistryKey shell = prog.CreateSubKey("shell");
			shell.SetValue(string.Empty, "open");
			using(RegistryKey cmd = shell.CreateSubKey(@"open\command"))
			{
				cmd.SetValue(string.Empty, _Command(type.ApplicationPath, type.Arguments));
			}

			foreach(Verb i in type.Verbs)
			{
				using RegistryKey verb = shell.CreateSubKey(i.Name);
				if(i.Text is not null) { verb.SetValue(string.Empty, i.Text); }

				using RegistryKey cmd = verb.CreateSubKey("command");
				cmd.SetValue(string.Empty, _Command((i.ApplicationPath ?? type.ApplicationPath), i.Arguments));
			}
		}

		using(RegistryKey ext = _Registry._Create(scope, _CLASSES + type.Extension))
		{
			string? current = GetProgId(type.Extension);
			if((current is null) || string.Equals(current, type.ProgId, StringComparison.OrdinalIgnoreCase) || (!_ProgIdExists(current)))
			{
				ext.SetValue(string.Empty, type.ProgId);
			}

			using RegistryKey openWith = ext.CreateSubKey(_OPEN_WITH);
			openWith.SetValue(type.ProgId, Array.Empty<byte>(), RegistryValueKind.None);
		}

		_Registry._NotifyAssociationsChanged();
	}


	/// <summary>Removes a file type registration.</summary>
	/// <param name="extension">File extension, with or without leading dot.</param>
	/// <param name="progId">Program ID.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns TRUE if anything has been removed, FALSE if the file type was not registered.</returns>
	/// <exception cref="ArgumentException">Thrown when the extension or the program ID is invalid.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access.</exception>
	/// <remarks>Only the program ID and its references are removed; registrations of other applications for the same extension
	///          remain.</remarks>
	public static bool Unregister(string extension, string progId, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		extension = _NormalizeExtension(extension, nameof(extension));
		_CheckProgId(progId, nameof(progId));

		bool rval = _Registry._Delete(scope, _CLASSES + progId);

		using(RegistryKey? ext = _Registry._Open(scope, _CLASSES + extension, true))
		{
			if(ext is not null)
			{
				if(string.Equals((ext.GetValue(string.Empty) as string), progId, StringComparison.OrdinalIgnoreCase))
				{
					ext.DeleteValue(string.Empty, false);
					rval = true;
				}

				using(RegistryKey? openWith = ext.OpenSubKey(_OPEN_WITH, true))
				{
					if((openWith is not null) && openWith.GetValueNames().Contains(progId, StringComparer.OrdinalIgnoreCase))
					{
						openWith.DeleteValue(progId, false);
						rval = true;
					}
					if((openWith is not null) && _IsEmpty(openWith)) { ext.DeleteSubKey(_OPEN_WITH, false); }
				}
			}
		}

		using(RegistryKey? ext = _Registry._Open(scope, _CLASSES + extension))
		{
			if((ext is not null) && _IsEmpty(ext)) { _Registry._Delete(scope, _CLASSES + extension); }
		}

		if(rval) { _Registry._NotifyAssociationsChanged(); }
		return rval;
	}


	/// <summary>Determines if a file type is registered.</summary>
	/// <param name="extension">File extension, with or without leading dot.</param>
	/// <param name="progId">Program ID.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns TRUE if the program ID exists and is registered for the extension, otherwise returns FALSE.</returns>
	/// <exception cref="ArgumentException">Thrown when the extension or the program ID is invalid.</exception>
	public static bool IsRegistered(string extension, string progId, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		extension = _NormalizeExtension(extension, nameof(extension));
		_CheckProgId(progId, nameof(progId));

		using(RegistryKey? prog = _Registry._Open(scope, _CLASSES + progId))
		{
			if(prog is null) { return false; }
		}

		using RegistryKey? ext = _Registry._Open(scope, _CLASSES + extension);
		if(ext is null) { return false; }
		if(string.Equals((ext.GetValue(string.Empty) as string), progId, StringComparison.OrdinalIgnoreCase)) { return true; }

		using RegistryKey? openWith = ext.OpenSubKey(_OPEN_WITH);
		return ((openWith is not null) && openWith.GetValueNames().Contains(progId, StringComparer.OrdinalIgnoreCase));
	}


	/// <summary>Gets the program ID that is registered as default for an extension.</summary>
	/// <param name="extension">File extension, with or without leading dot.</param>
	/// <param name="scope">Scope, or NULL for the merged view of both scopes as Windows sees it (without the user's choice in
	///                     the default apps settings).</param>
	/// <returns>Returns the program ID, or NULL if no default is registered.</returns>
	/// <exception cref="ArgumentException">Thrown when the extension is invalid.</exception>
	public static string? GetProgId(string extension, RegistrationScope? scope = null)
	{
		extension = _NormalizeExtension(extension, nameof(extension));

		using RegistryKey? ext = ((scope is null) ? Registry.ClassesRoot.OpenSubKey(extension) : _Registry._Open(scope.Value, _CLASSES + extension));
		string? rval = (ext?.GetValue(string.Empty) as string);

		return (string.IsNullOrEmpty(rval) ? null : rval);
	}


	/// <summary>Registers an application with its file types for the default apps settings.</summary>
	/// <param name="application">Application.</param>
	/// <param name="scope">Scope.</param>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access, for example when registering for
	///                                                all users without administrator rights.</exception>
	/// <remarks>All file types are registered with <see cref="Register(FileType, RegistrationScope)"/>. The application then
	///          appears in the default apps settings with its file types, where the user can make it the default. The
	///          capabilities are stored under Software\&lt;name&gt;\Capabilities and replaced completely.</remarks>
	public static void RegisterApplication(ApplicationCapabilities application, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		ArgumentNullException.ThrowIfNull(application);

		foreach(FileType i in application.FileTypes) { Register(i, scope); }

		_Registry._Delete(scope, _CapabilitiesPath(application.Name));
		using(RegistryKey caps = _Registry._Create(scope, _CapabilitiesPath(application.Name)))
		{
			caps.SetValue("ApplicationName", application.DisplayName);
			if(application.Description is not null) { caps.SetValue("ApplicationDescription", application.Description); }

			string? icon = (application.IconPath ?? application.FileTypes.FirstOrDefault()?.ApplicationPath);
			if(icon is not null) { caps.SetValue("ApplicationIcon", $"\"{icon}\",{application.IconIndex}"); }

			using RegistryKey assoc = caps.CreateSubKey("FileAssociations");
			foreach(FileType i in application.FileTypes) { assoc.SetValue(i.Extension, i.ProgId); }
		}

		using(RegistryKey apps = _Registry._Create(scope, _REGISTERED_APPS))
		{
			apps.SetValue(application.Name, _CapabilitiesPath(application.Name));
		}

		_Registry._NotifyAssociationsChanged();
	}


	/// <summary>Removes an application from the default apps settings.</summary>
	/// <param name="name">Registration name.</param>
	/// <param name="scope">Scope.</param>
	/// <param name="fileTypes">Determines if the file types of the application are unregistered as well.</param>
	/// <returns>Returns TRUE if anything has been removed, FALSE if the application was not registered.</returns>
	/// <exception cref="ArgumentException">Thrown when the name is invalid.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access.</exception>
	public static bool UnregisterApplication(string name, RegistrationScope scope = RegistrationScope.CURRENT_USER, bool fileTypes = true)
	{
		_Registry._CheckName(name, nameof(name));
		bool rval = false;

		if(fileTypes)
		{
			using RegistryKey? assoc = _Registry._Open(scope, _CapabilitiesPath(name) + @"\FileAssociations");
			foreach(string i in (assoc?.GetValueNames() ?? []))
			{
				if((assoc!.GetValue(i) is string progId) && (!string.IsNullOrWhiteSpace(progId)) && (!progId.Contains('\\')))
				{
					try { rval |= Unregister(i, progId, scope); } catch(ArgumentException) {}
				}
			}
		}

		rval |= _Registry._Delete(scope, _CapabilitiesPath(name));
		using(RegistryKey? app = _Registry._Open(scope, _AppPath(name)))
		{
			if((app is not null) && _IsEmpty(app)) { _Registry._Delete(scope, _AppPath(name)); }
		}

		using(RegistryKey? apps = _Registry._Open(scope, _REGISTERED_APPS, true))
		{
			if((apps is not null) && apps.GetValueNames().Contains(name, StringComparer.OrdinalIgnoreCase))
			{
				apps.DeleteValue(name, false);
				rval = true;
			}
		}

		if(rval) { _Registry._NotifyAssociationsChanged(); }
		return rval;
	}


	/// <summary>Determines if an application is registered for the default apps settings.</summary>
	/// <param name="name">Registration name.</param>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns TRUE if the application is listed and its capabilities exist, otherwise returns FALSE.</returns>
	/// <exception cref="ArgumentException">Thrown when the name is invalid.</exception>
	public static bool IsApplicationRegistered(string name, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		_Registry._CheckName(name, nameof(name));

		using(RegistryKey? apps = _Registry._Open(scope, _REGISTERED_APPS))
		{
			if((apps is null) || (!apps.GetValueNames().Contains(name, StringComparer.OrdinalIgnoreCase))) { return false; }
		}

		using RegistryKey? caps = _Registry._Open(scope, _CapabilitiesPath(name));
		return (caps is not null);
	}


	/// <summary>Opens the default apps settings, where the user chooses the default application for a file type.</summary>
	public static void OpenDefaultAppsSettings()
	{
		Process.Start(new ProcessStartInfo("ms-settings:defaultapps") { UseShellExecute = true })?.Dispose();
	}


	/// <summary>Opens the page of an application in the default apps settings.</summary>
	/// <param name="name">Registration name of the application.</param>
	/// <param name="scope">Scope the application has been registered in.</param>
	/// <exception cref="ArgumentException">Thrown when the name is invalid.</exception>
	/// <remarks>Windows versions that do not support application pages open the overview.</remarks>
	public static void OpenDefaultAppsSettings(string name, RegistrationScope scope = RegistrationScope.CURRENT_USER)
	{
		_Registry._CheckName(name, nameof(name));

		string param = ((scope == RegistrationScope.LOCAL_MACHINE) ? "registeredAppMachine" : "registeredAppUser");
		Process.Start(new ProcessStartInfo($"ms-settings:defaultapps?{param}={Uri.EscapeDataString(name)}") { UseShellExecute = true })?.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] FileType                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class describes a file type for a file association.</summary>
	/// <example><code>
	/// FileAssociation.Register(new FileAssociation.FileType(".rqn", "Robbiblubber.Rqoon.Document", Environment.ProcessPath!)
	/// {
	///     Description = "rqoon Document",
	///     Verbs = [new FileAssociation.Verb("print") { Arguments = "/print \"%1\"" }]
	/// });
	/// </code></example>
	public sealed class FileType
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="extension">File extension, with or without leading dot.</param>
		/// <param name="progId">Program ID (registry key name of the file type), for example "Company.Application.Document".</param>
		/// <param name="applicationPath">Path of the application that opens the files.</param>
		/// <exception cref="ArgumentException">Thrown when an argument is empty or invalid.</exception>
		public FileType(string extension, string progId, string applicationPath)
		{
			Extension = FileAssociation._NormalizeExtension(extension, nameof(extension));
			FileAssociation._CheckProgId(progId, nameof(progId));
			ArgumentException.ThrowIfNullOrWhiteSpace(applicationPath);

			ProgId = progId;
			ApplicationPath = applicationPath;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the file extension including the leading dot.</summary>
		public string Extension
		{
			get;
		}


		/// <summary>Gets the program ID.</summary>
		public string ProgId
		{
			get;
		}


		/// <summary>Gets the path of the application that opens the files.</summary>
		public string ApplicationPath
		{
			get;
		}


		/// <summary>Gets the file type description shown in Explorer, or NULL to let Windows derive one from the extension.</summary>
		public string? Description
		{
			get; init;
		}


		/// <summary>Gets the icon path, or NULL to use the application icon.</summary>
		public string? IconPath
		{
			get; init;
		}


		/// <summary>Gets the icon index in the icon file.</summary>
		public int IconIndex
		{
			get; init;
		}


		/// <summary>Gets the command line arguments of the open verb; "%1" is replaced by the file name.</summary>
		public string Arguments
		{
			get; init;
		} = "\"%1\"";


		/// <summary>Gets additional verbs.</summary>
		public IReadOnlyList<Verb> Verbs
		{
			get; init;
		} = [];
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Verb                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class describes an additional shell verb of a file type (an entry in the context menu of a file).</summary>
	public sealed class Verb
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Verb name, for example "edit" or "print". Standard verbs get their menu text from Windows.</param>
		/// <exception cref="ArgumentException">Thrown when the name is empty, has leading or trailing white space or contains a
		///                                      backslash.</exception>
		public Verb(string name)
		{
			_Registry._CheckName(name, nameof(name));
			Name = name;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the verb name.</summary>
		public string Name
		{
			get;
		}


		/// <summary>Gets the menu text, or NULL to use the verb name (standard verbs like "edit" or "print" are translated by
		///          Windows).</summary>
		public string? Text
		{
			get; init;
		}


		/// <summary>Gets the application path, or NULL to use the application of the file type.</summary>
		public string? ApplicationPath
		{
			get; init;
		}


		/// <summary>Gets the command line arguments; "%1" is replaced by the file name.</summary>
		public string Arguments
		{
			get; init;
		} = "\"%1\"";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] ApplicationCapabilities                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class describes an application with the file types it can open, for the default apps settings of
	///          Windows.</summary>
	/// <example><code>
	/// FileAssociation.RegisterApplication(new FileAssociation.ApplicationCapabilities("Robbiblubber.Rqoon", "rqoon")
	/// {
	///     Description = "rqoon document editor",
	///     FileTypes = [new FileAssociation.FileType(".rqn", "Robbiblubber.Rqoon.Document", Environment.ProcessPath!) { Description = "rqoon Document" }]
	/// });
	/// </code></example>
	public sealed class ApplicationCapabilities
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Registration name that identifies the application, for example "Company.Application".</param>
		/// <param name="displayName">Display name shown in the default apps settings.</param>
		/// <exception cref="ArgumentException">Thrown when the name is empty, has leading or trailing white space or contains a
		///                                      backslash, or when the display name is empty.</exception>
		public ApplicationCapabilities(string name, string displayName)
		{
			_Registry._CheckName(name, nameof(name));
			ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

			Name = name;
			DisplayName = displayName;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the registration name that identifies the application.</summary>
		public string Name
		{
			get;
		}


		/// <summary>Gets the display name.</summary>
		public string DisplayName
		{
			get;
		}


		/// <summary>Gets the description shown in the default apps settings.</summary>
		public string? Description
		{
			get; init;
		}


		/// <summary>Gets the icon path, or NULL to use the application of the first file type.</summary>
		public string? IconPath
		{
			get; init;
		}


		/// <summary>Gets the icon index in the icon file.</summary>
		public int IconIndex
		{
			get; init;
		}


		/// <summary>Gets the file types the application can open.</summary>
		public IReadOnlyList<FileType> FileTypes
		{
			get; init;
		} = [];
	}
}
