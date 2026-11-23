namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

using Microsoft.Win32;



/// <summary>This class provides registry helper methods.</summary>
[SupportedOSPlatform("windows")]
internal static partial class _Registry
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Shell change event: file associations have changed (SHCNE_ASSOCCHANGED).</summary>
	private const uint _SHCNE_ASSOCCHANGED = 0x08000000;

	/// <summary>Shell change flag: item parameters are ID lists (SHCNF_IDLIST).</summary>
	private const uint _SHCNF_IDLIST = 0x0000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the root key for a scope.</summary>
	/// <param name="scope">Scope.</param>
	/// <returns>Returns the root key.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the scope is invalid.</exception>
	internal static RegistryKey _Root(RegistrationScope scope)
	{
		return scope switch
		{
			RegistrationScope.CURRENT_USER => Registry.CurrentUser,
			RegistrationScope.LOCAL_MACHINE => Registry.LocalMachine,
			_ => throw new ArgumentOutOfRangeException(nameof(scope), scope, "Invalid registration scope.")
		};
	}


	/// <summary>Checks a registry key name.</summary>
	/// <param name="name">Name.</param>
	/// <param name="paramName">Parameter name.</param>
	/// <exception cref="ArgumentException">Thrown when the name is empty, has leading or trailing white space or contains a
	///                                      backslash.</exception>
	internal static void _CheckName(string name, string paramName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name, paramName);

		if(name.Trim() != name) { throw new ArgumentException("The name must not have leading or trailing white space.", paramName); }
		if(name.Contains('\\')) { throw new ArgumentException("The name must not contain a backslash.", paramName); }
	}


	/// <summary>Opens a registry key for writing and creates it if it does not exist.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="path">Key path relative to the scope root.</param>
	/// <returns>Returns the key.</returns>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access to the key.</exception>
	internal static RegistryKey _Create(RegistrationScope scope, string path)
	{
		try
		{
			return _Root(scope).CreateSubKey(path, true);
		}
		catch(Exception ex) when((ex is UnauthorizedAccessException) || (ex is SecurityException))
		{
			throw _AccessDenied(scope, ex);
		}
	}


	/// <summary>Opens a registry key.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="path">Key path relative to the scope root.</param>
	/// <param name="writable">Determines if the key is opened for writing.</param>
	/// <returns>Returns the key, or NULL if it does not exist.</returns>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no access to the key.</exception>
	internal static RegistryKey? _Open(RegistrationScope scope, string path, bool writable = false)
	{
		try
		{
			return _Root(scope).OpenSubKey(path, writable);
		}
		catch(Exception ex) when((ex is UnauthorizedAccessException) || (ex is SecurityException))
		{
			throw _AccessDenied(scope, ex);
		}
	}


	/// <summary>Deletes a registry key with all subkeys.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="path">Key path relative to the scope root.</param>
	/// <returns>Returns TRUE if the key has been deleted, FALSE if it did not exist.</returns>
	/// <exception cref="UnauthorizedAccessException">Thrown when the user has no write access to the key.</exception>
	internal static bool _Delete(RegistrationScope scope, string path)
	{
		try
		{
			using(RegistryKey? k = _Root(scope).OpenSubKey(path))
			{
				if(k is null) { return false; }
			}

			_Root(scope).DeleteSubKeyTree(path, false);
			return true;
		}
		catch(Exception ex) when((ex is UnauthorizedAccessException) || (ex is SecurityException))
		{
			throw _AccessDenied(scope, ex);
		}
	}


	/// <summary>Notifies the shell that file associations have changed.</summary>
	internal static void _NotifyAssociationsChanged()
	{
		SHChangeNotify(_SHCNE_ASSOCCHANGED, _SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Notifies the system of an event that an application has performed.</summary>
	/// <param name="wEventId">Event ID.</param>
	/// <param name="uFlags">Flags.</param>
	/// <param name="dwItem1">First item.</param>
	/// <param name="dwItem2">Second item.</param>
	[LibraryImport("shell32.dll")]
	private static partial void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);


	/// <summary>Creates the exception for denied access.</summary>
	/// <param name="scope">Scope.</param>
	/// <param name="inner">Original exception.</param>
	/// <returns>Returns the exception.</returns>
	private static UnauthorizedAccessException _AccessDenied(RegistrationScope scope, Exception inner)
	{
		return new(((scope == RegistrationScope.LOCAL_MACHINE) ? "Registering for all users requires administrator rights." :
																  "Access to the registry key has been denied."), inner);
	}
}
