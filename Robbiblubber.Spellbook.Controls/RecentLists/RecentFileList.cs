namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class implements a component that keeps a list of recently used files and folders and shows it in a menu.</summary>
/// <remarks><para>Paths are normalized (full path, no trailing separator) and compared case-insensitively. Files and folders that do
///          not exist are kept in the list and shown grayed; the application can remove them when they are selected (see
///          <see cref="RecentItemEventArgs.Remove"/>). Paths on network drives are not checked, so opening the menu never blocks.
///          The menu shows the shell icon of the file type or of a folder.</para>
///          <para>Optionally, added files are also passed to the recent documents of Windows and the jump list of the application
///          (see <see cref="AddToWindowsRecent"/>).</para></remarks>
[ToolboxBitmap(typeof(RecentFileList), "Resources.recentlist.bmp")]
[DefaultEvent("ItemSelected")]
[SupportedOSPlatform("windows")]
public class RecentFileList: RecentItemList
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default list name.</summary>
	public const string DEFAULT_FILE_LIST_NAME = "files";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Icons by file type ("&lt;folder&gt;" for folders, otherwise the lower-case extension).</summary>
	private static readonly Dictionary<string, Image?> _Icons = new();

	/// <summary>Lock object for the icon cache.</summary>
	private static readonly object _IconLock = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public RecentFileList(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public RecentFileList(IContainer container): base(container)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Shortens a path in the middle, keeping its root and its last parts.</summary>
	/// <param name="path">Path.</param>
	/// <param name="maxLength">Maximum length.</param>
	/// <returns>Returns the path if it is short enough, otherwise the root, an ellipsis and as many of the last parts as fit (at least
	///          the file name).</returns>
	internal static string _CompactPath(string path, int maxLength)
	{
		string root;
		string[] parts;
		string rval;

		if(path.Length <= maxLength) { return path; }

		root = (Path.GetPathRoot(path) ?? string.Empty);
		parts = path.Substring(root.Length).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
		if(parts.Length == 0) { return path; }

		rval = parts[^1];
		for(int i = parts.Length - 2; i >= 0; i--)
		{
			string candidate = parts[i] + Path.DirectorySeparatorChar + rval;
			if((root.Length + 2 + candidate.Length) > maxLength) { break; }
			rval = candidate;
		}

		return (root + "\u2026" + Path.DirectorySeparatorChar + rval);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a path is on a network drive or a UNC path.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns <c>true</c> for network paths, NULL if this cannot be determined, otherwise <c>false</c>.</returns>
	private static bool? _IsNetworkPath(string path)
	{
		string? root = Path.GetPathRoot(path);

		try
		{
			if(string.IsNullOrEmpty(root)) { return null; }
			if(root.StartsWith(@"\\", StringComparison.Ordinal)) { return true; }

			return (new DriveInfo(root).DriveType == DriveType.Network);
		}
		catch(Exception ex) when ((ex is ArgumentException) || (ex is IOException) || (ex is UnauthorizedAccessException)) { return null; }
	}


	/// <summary>Returns if a path is a folder, without accessing network paths.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns <c>true</c> if the path is a folder.</returns>
	/// <remarks>Local paths are checked; for network paths and paths that do not exist, a path without extension is considered to be
	///          a folder.</remarks>
	private static bool _IsFolder(string path)
	{
		if((_IsNetworkPath(path) == false) && Directory.Exists(path)) { return true; }
		if((_IsNetworkPath(path) == false) && File.Exists(path)) { return false; }

		return (!Path.HasExtension(path));
	}


	/// <summary>Gets the shell icon of a file type.</summary>
	/// <param name="folder">Determines if the icon of a folder is returned.</param>
	/// <param name="extension">Extension including the dot, empty for files without extension.</param>
	/// <returns>Returns the small icon, or NULL if the shell does not provide one.</returns>
	/// <remarks>Icons are retrieved by type, without accessing a file, and cached for the lifetime of the application.</remarks>
	private static Image? _GetShellIcon(bool folder, string extension)
	{
		string key = (folder ? "<folder>" : extension.ToLowerInvariant());
		_NativeMethods._SHFILEINFO info = new();
		Image? rval = null;

		lock(_IconLock)
		{
			if(_Icons.TryGetValue(key, out rval)) { return rval; }

			try
			{
				IntPtr result = _NativeMethods._SHGetFileInfo((folder ? "folder" : ("file" + extension)), (folder ? _NativeMethods._FILE_ATTRIBUTE_DIRECTORY : _NativeMethods._FILE_ATTRIBUTE_NORMAL),
																	ref info, (uint) Marshal.SizeOf<_NativeMethods._SHFILEINFO>(),
																	_NativeMethods._SHGFI_ICON | _NativeMethods._SHGFI_SMALLICON | _NativeMethods._SHGFI_USEFILEATTRIBUTES);

				if((result != IntPtr.Zero) && (info.hIcon != IntPtr.Zero))
				{
					using(Icon icon = Icon.FromHandle(info.hIcon)) { rval = icon.ToBitmap(); }
					_NativeMethods._DestroyIcon(info.hIcon);
				}
			}
			catch(Exception ex) when ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException) || (ex is ArgumentException) || (ex is ExternalException)) { rval = null; }

			_Icons[key] = rval;
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected properties                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override string _DefaultListName
	{
		get { return DEFAULT_FILE_LIST_NAME; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Paths are compared case-insensitively.</remarks>
	[Browsable(false)]
	public override IEqualityComparer<string> KeyComparer
	{
		get { return StringComparer.OrdinalIgnoreCase; }
	}


	/// <summary>Gets or sets how file names are shown in the menu.</summary>
	[Category("Recent List")][Description("Specifies if the file name or the full (shortened) path is shown.")][DefaultValue(RecentFileDisplayStyle.FILE_NAME)]
	public RecentFileDisplayStyle DisplayStyle
	{
		get; set;
	} = RecentFileDisplayStyle.FILE_NAME;


	/// <summary>Gets or sets the maximum length of paths shown with <see cref="RecentFileDisplayStyle.FULL_PATH"/>.</summary>
	[Category("Recent List")][Description("Maximum length of paths shown as full path; longer paths are shortened in the middle.")][DefaultValue(60)]
	public int MaxPathLength
	{
		get; set;
	} = 60;


	/// <summary>Gets or sets if added files are passed to the recent documents of Windows and the jump list of the application.</summary>
	/// <remarks>Windows only shows files whose type is registered for the application in the jump list.</remarks>
	[Category("Recent List")][Description("Specifies if added files are passed to the recent documents of Windows and the jump list.")][DefaultValue(false)]
	public bool AddToWindowsRecent
	{
		get; set;
	} = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] RecentItemList                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Returns the full path without a trailing separator, or NULL if the path is invalid.</remarks>
	protected override string? _Normalize(string key)
	{
		string rval;

		key = key.Trim().Trim('"');
		if(key.Length == 0) { return null; }

		try
		{
			rval = Path.GetFullPath(key);
		}
		catch(Exception ex) when ((ex is ArgumentException) || (ex is NotSupportedException) || (ex is PathTooLongException) || (ex is System.Security.SecurityException)) { return null; }

		if((rval.Length > 1) && (rval != Path.GetPathRoot(rval))) { rval = rval.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
		return rval;
	}


	/// <inheritdoc/>
	protected override string _GetText(RecentItem item)
	{
		string name;

		if(item.Text is not null) { return item.Text; }
		if(DisplayStyle == RecentFileDisplayStyle.FULL_PATH) { return _CompactPath(item.Key, MaxPathLength); }

		name = Path.GetFileName(item.Key);
		return ((name.Length == 0) ? item.Key : name);
	}


	/// <inheritdoc/>
	protected override string? _GetToolTip(RecentItem item)
	{
		return item.Key;
	}


	/// <inheritdoc/>
	/// <remarks>Paths on network drives and UNC paths are not checked (NULL); for other paths, the method returns if the file or
	///          folder exists.</remarks>
	protected override bool? _IsAvailable(RecentItem item)
	{
		if(_IsNetworkPath(item.Key) != false) { return null; }

		try
		{
			return (File.Exists(item.Key) || Directory.Exists(item.Key));
		}
		catch(Exception ex) when ((ex is IOException) || (ex is UnauthorizedAccessException)) { return null; }
	}


	/// <inheritdoc/>
	/// <remarks>Returns the shell icon of the file type or of a folder; the file itself is not accessed.</remarks>
	protected override Image? _GetImage(RecentItem item)
	{
		bool folder = _IsFolder(item.Key);
		return _GetShellIcon(folder, (folder ? string.Empty : Path.GetExtension(item.Key)));
	}


	/// <inheritdoc/>
	protected override void _OnAdded(RecentItem item)
	{
		if(AddToWindowsRecent && File.Exists(item.Key))
		{
			try
			{
				_NativeMethods._SHAddToRecentDocs(_NativeMethods._SHARD_PATHW, item.Key);
			}
			catch(Exception ex) when ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException)) {}
		}
	}
}
