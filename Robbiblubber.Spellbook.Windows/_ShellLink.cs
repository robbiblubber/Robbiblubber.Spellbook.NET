namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;



/// <summary>This class is the COM class of shell links (CLSID_ShellLink).</summary>
[SupportedOSPlatform("windows")]
[ComImport, Guid("00021401-0000-0000-C000-000000000046")]
internal class _ShellLink
{}



/// <summary>This interface is the shell link interface (IShellLinkW).</summary>
[SupportedOSPlatform("windows")]
[ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface _IShellLinkW
{
	/// <summary>Gets the target path.</summary>
	/// <param name="pszFile">Buffer.</param>
	/// <param name="cch">Buffer size.</param>
	/// <param name="pfd">Find data (not used).</param>
	/// <param name="fFlags">Flags.</param>
	void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, IntPtr pfd, uint fFlags);

	/// <summary>Gets the item ID list.</summary>
	/// <param name="ppidl">Item ID list.</param>
	void GetIDList(out IntPtr ppidl);

	/// <summary>Sets the item ID list.</summary>
	/// <param name="pidl">Item ID list.</param>
	void SetIDList(IntPtr pidl);

	/// <summary>Gets the description.</summary>
	/// <param name="pszName">Buffer.</param>
	/// <param name="cch">Buffer size.</param>
	void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);

	/// <summary>Sets the description.</summary>
	/// <param name="pszName">Description.</param>
	void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

	/// <summary>Gets the working directory.</summary>
	/// <param name="pszDir">Buffer.</param>
	/// <param name="cch">Buffer size.</param>
	void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);

	/// <summary>Sets the working directory.</summary>
	/// <param name="pszDir">Working directory.</param>
	void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

	/// <summary>Gets the arguments.</summary>
	/// <param name="pszArgs">Buffer.</param>
	/// <param name="cch">Buffer size.</param>
	void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);

	/// <summary>Sets the arguments.</summary>
	/// <param name="pszArgs">Arguments.</param>
	void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

	/// <summary>Gets the hotkey.</summary>
	/// <param name="pwHotkey">Hotkey.</param>
	void GetHotkey(out ushort pwHotkey);

	/// <summary>Sets the hotkey.</summary>
	/// <param name="wHotkey">Hotkey.</param>
	void SetHotkey(ushort wHotkey);

	/// <summary>Gets the show command.</summary>
	/// <param name="piShowCmd">Show command.</param>
	void GetShowCmd(out int piShowCmd);

	/// <summary>Sets the show command.</summary>
	/// <param name="iShowCmd">Show command.</param>
	void SetShowCmd(int iShowCmd);

	/// <summary>Gets the icon location.</summary>
	/// <param name="pszIconPath">Buffer.</param>
	/// <param name="cch">Buffer size.</param>
	/// <param name="piIcon">Icon index.</param>
	void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);

	/// <summary>Sets the icon location.</summary>
	/// <param name="pszIconPath">Icon path.</param>
	/// <param name="iIcon">Icon index.</param>
	void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

	/// <summary>Sets the relative path.</summary>
	/// <param name="pszPathRel">Relative path.</param>
	/// <param name="dwReserved">Reserved.</param>
	void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

	/// <summary>Resolves the link.</summary>
	/// <param name="hwnd">Window handle.</param>
	/// <param name="fFlags">Flags.</param>
	void Resolve(IntPtr hwnd, uint fFlags);

	/// <summary>Sets the target path.</summary>
	/// <param name="pszFile">Target path.</param>
	void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}



/// <summary>This interface gives access to the data blocks of a shell link (IShellLinkDataList).</summary>
[SupportedOSPlatform("windows")]
[ComImport, Guid("45E2B4AE-B1C3-11D0-B92F-00A0C90312E1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface _IShellLinkDataList
{
	/// <summary>Adds a data block.</summary>
	/// <param name="pDataBlock">Data block.</param>
	void AddDataBlock(IntPtr pDataBlock);

	/// <summary>Copies a data block.</summary>
	/// <param name="dwSig">Signature.</param>
	/// <param name="ppDataBlock">Data block.</param>
	void CopyDataBlock(uint dwSig, out IntPtr ppDataBlock);

	/// <summary>Removes a data block.</summary>
	/// <param name="dwSig">Signature.</param>
	void RemoveDataBlock(uint dwSig);

	/// <summary>Gets the link flags.</summary>
	/// <param name="pdwFlags">Flags.</param>
	void GetFlags(out uint pdwFlags);

	/// <summary>Sets the link flags.</summary>
	/// <param name="dwFlags">Flags.</param>
	void SetFlags(uint dwFlags);
}
