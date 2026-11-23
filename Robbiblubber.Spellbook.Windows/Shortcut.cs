namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using System.Text;



/// <summary>This class creates and reads shortcuts (.lnk files).</summary>
/// <example><code>
/// string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
/// Shortcut.Create(Path.Combine(desktop, "rqoon.lnk"), new ShortcutInfo(Environment.ProcessPath!) { Description = "rqoon" });
/// </code></example>
[SupportedOSPlatform("windows")]
public static class Shortcut
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Link flag: run as administrator (SLDF_RUNAS_USER).</summary>
	private const uint _SLDF_RUNAS_USER = 0x00002000;

	/// <summary>Storage mode: read access (STGM_READ).</summary>
	private const int _STGM_READ = 0;

	/// <summary>Buffer size for paths.</summary>
	private const int _PATH_SIZE = 32768;

	/// <summary>Buffer size for descriptions (INFOTIPSIZE).</summary>
	private const int _DESCRIPTION_SIZE = 1024;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a shell link object, performs an action and releases the object.</summary>
	/// <typeparam name="T">Result type.</typeparam>
	/// <param name="action">Action.</param>
	/// <returns>Returns the result of the action.</returns>
	private static T _Use<T>(Func<_IShellLinkW, T> action)
	{
		object link = new _ShellLink();
		try
		{
			return action((_IShellLinkW) link);
		}
		finally { Marshal.FinalReleaseComObject(link); }
	}


	/// <summary>Saves a shortcut file.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="info">Shortcut information.</param>
	private static void _Save(string fileName, ShortcutInfo info)
	{
		_Use(link =>
		{
			link.SetPath(info.TargetPath);
			link.SetArguments(info.Arguments ?? string.Empty);
			link.SetDescription(info.Description ?? string.Empty);
			link.SetShowCmd((int) info.WindowStyle);

			string? dir = (info.WorkingDirectory ?? (Path.IsPathRooted(info.TargetPath) ? Path.GetDirectoryName(info.TargetPath) : null));
			if(!string.IsNullOrEmpty(dir)) { link.SetWorkingDirectory(dir); }
			if(!string.IsNullOrEmpty(info.IconPath)) { link.SetIconLocation(info.IconPath, info.IconIndex); }

			if(info.RunAsAdministrator)
			{
				_IShellLinkDataList data = (_IShellLinkDataList) link;
				data.GetFlags(out uint flags);
				data.SetFlags(flags | _SLDF_RUNAS_USER);
			}

			((IPersistFile) link).Save(fileName, true);
			return true;
		});
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a shortcut.</summary>
	/// <param name="linkFile">Shortcut file name, usually with the extension ".lnk".</param>
	/// <param name="targetPath">Target path.</param>
	/// <param name="arguments">Command line arguments.</param>
	/// <exception cref="ArgumentException">Thrown when the file name or the target path is empty.</exception>
	public static void Create(string linkFile, string targetPath, string? arguments = null)
	{
		Create(linkFile, new ShortcutInfo(targetPath) { Arguments = arguments });
	}


	/// <summary>Creates a shortcut.</summary>
	/// <param name="linkFile">Shortcut file name, usually with the extension ".lnk".</param>
	/// <param name="info">Shortcut information.</param>
	/// <exception cref="ArgumentException">Thrown when the file name is empty.</exception>
	/// <remarks>The directory is created if it does not exist. An existing shortcut is replaced only after the new one has been
	///          written successfully.</remarks>
	public static void Create(string linkFile, ShortcutInfo info)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(linkFile);
		ArgumentNullException.ThrowIfNull(info);

		linkFile = Path.GetFullPath(linkFile);
		string dir = Path.GetDirectoryName(linkFile)!;
		Directory.CreateDirectory(dir);

		string temp = Path.Combine(dir, $"~{Guid.NewGuid():N}.lnk");
		try
		{
			_Save(temp, info);
			File.Move(temp, linkFile, true);
		}
		finally
		{
			if(File.Exists(temp)) { File.Delete(temp); }
		}
	}


	/// <summary>Reads a shortcut.</summary>
	/// <param name="linkFile">Shortcut file name.</param>
	/// <returns>Returns the shortcut information.</returns>
	/// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
	/// <exception cref="InvalidDataException">Thrown when the shortcut has no file system target (for example a shortcut to
	///                                         a control panel item).</exception>
	/// <remarks>Environment variables in paths are returned as stored (not expanded).</remarks>
	public static ShortcutInfo Read(string linkFile)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(linkFile);
		linkFile = Path.GetFullPath(linkFile);
		if(!File.Exists(linkFile)) { throw new FileNotFoundException("The shortcut file does not exist.", linkFile); }

		return _Use(link =>
		{
			((IPersistFile) link).Load(linkFile, _STGM_READ);

			StringBuilder s = new(_PATH_SIZE);
			link.GetPath(s, s.Capacity, IntPtr.Zero, 0);
			string target = s.ToString();
			if(string.IsNullOrWhiteSpace(target)) { throw new InvalidDataException("The shortcut has no file system target."); }

			s.Clear();
			link.GetArguments(s, s.Capacity);
			string args = s.ToString();

			s.Clear();
			link.GetWorkingDirectory(s, s.Capacity);
			string dir = s.ToString();

			s.Clear();
			link.GetIconLocation(s, s.Capacity, out int iconIndex);
			string icon = s.ToString();

			StringBuilder d = new(_DESCRIPTION_SIZE);
			link.GetDescription(d, d.Capacity);
			string desc = d.ToString();

			link.GetShowCmd(out int show);
			((_IShellLinkDataList) link).GetFlags(out uint flags);

			return new ShortcutInfo(target)
			{
				Arguments = ((args.Length == 0) ? null : args),
				WorkingDirectory = ((dir.Length == 0) ? null : dir),
				Description = ((desc.Length == 0) ? null : desc),
				IconPath = ((icon.Length == 0) ? null : icon),
				IconIndex = iconIndex,
				WindowStyle = (Enum.IsDefined((ShortcutWindowStyle) show) ? (ShortcutWindowStyle) show : ShortcutWindowStyle.NORMAL),
				RunAsAdministrator = ((flags & _SLDF_RUNAS_USER) != 0)
			};
		});
	}
}
