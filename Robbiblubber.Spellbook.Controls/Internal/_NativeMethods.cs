namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class provides the native Win32 functions and constants used by the controls.</summary>
[SupportedOSPlatform("windows")]
internal static partial class _NativeMethods
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal constants                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Window style: popup window.</summary>
	internal const int _WS_POPUP = unchecked((int) 0x80000000);
	/// <summary>Window style: thin border.</summary>
	internal const int _WS_BORDER = 0x00800000;
	/// <summary>Extended window style: tool window.</summary>
	internal const int _WS_EX_TOOLWINDOW = 0x00000080;
	/// <summary>Extended window style: window does not become active.</summary>
	internal const int _WS_EX_NOACTIVATE = 0x08000000;
	/// <summary>Window class style: drop shadow.</summary>
	internal const int _CS_DROPSHADOW = 0x00020000;
	/// <summary>Message: mouse activation.</summary>
	internal const int _WM_MOUSEACTIVATE = 0x0021;
	/// <summary>Mouse activation result: do not activate.</summary>
	internal const int _MA_NOACTIVATE = 3;
	/// <summary>Message: non-client hit test.</summary>
	internal const int _WM_NCHITTEST = 0x0084;
	/// <summary>Message: non-client paint.</summary>
	internal const int _WM_NCPAINT = 0x0085;
	/// <summary>Message: non-client activation change.</summary>
	internal const int _WM_NCACTIVATE = 0x0086;
	/// <summary>Message: set the font of a control.</summary>
	internal const int _WM_SETFONT = 0x0030;
	/// <summary>Message: draw the window into a device context.</summary>
	internal const int _WM_PRINT = 0x0317;
	/// <summary>Flag of WM_PRINT: draw the non-client area.</summary>
	internal const int _PRF_NONCLIENT = 0x00000002;
	/// <summary>Window style: vertical scroll bar.</summary>
	internal const int _WS_VSCROLL = 0x00200000;
	/// <summary>Window style: horizontal scroll bar.</summary>
	internal const int _WS_HSCROLL = 0x00100000;
	/// <summary>Index of the window style for GetWindowLong.</summary>
	internal const int _GWL_STYLE = -16;
	/// <summary>Region combination: intersection.</summary>
	internal const int _RGN_AND = 1;
	/// <summary>Hit test result: right border.</summary>
	internal const int _HTRIGHT = 11;
	/// <summary>Hit test result: bottom border.</summary>
	internal const int _HTBOTTOM = 15;
	/// <summary>Hit test result: bottom right corner.</summary>
	internal const int _HTBOTTOMRIGHT = 17;
	/// <summary>Message: left mouse button down (client area).</summary>
	internal const int _WM_LBUTTONDOWN = 0x0201;
	/// <summary>Message: right mouse button down (client area).</summary>
	internal const int _WM_RBUTTONDOWN = 0x0204;
	/// <summary>Message: middle mouse button down (client area).</summary>
	internal const int _WM_MBUTTONDOWN = 0x0207;
	/// <summary>Message: X mouse button down (client area).</summary>
	internal const int _WM_XBUTTONDOWN = 0x020B;
	/// <summary>Message: left mouse button down (non-client area).</summary>
	internal const int _WM_NCLBUTTONDOWN = 0x00A1;
	/// <summary>Message: right mouse button down (non-client area).</summary>
	internal const int _WM_NCRBUTTONDOWN = 0x00A4;
	/// <summary>Message: middle mouse button down (non-client area).</summary>
	internal const int _WM_NCMBUTTONDOWN = 0x00A7;
	/// <summary>Message: X mouse button down (non-client area).</summary>
	internal const int _WM_NCXBUTTONDOWN = 0x00AB;
	/// <summary>Show command: hide the window.</summary>
	internal const int _SW_HIDE = 0;
	/// <summary>Show command: show the window without activating it.</summary>
	internal const int _SW_SHOWNOACTIVATE = 4;
	/// <summary>Show command: restore a minimized window to its previous state.</summary>
	internal const int _SW_RESTORE = 9;
	/// <summary>AllowSetForegroundWindow process ID: all processes.</summary>
	internal const int _ASFW_ANY = -1;
	/// <summary>Window position flag: keep size.</summary>
	internal const int _SWP_NOSIZE = 0x0001;
	/// <summary>Window position flag: keep position.</summary>
	internal const int _SWP_NOMOVE = 0x0002;
	/// <summary>Window position flag: do not activate.</summary>
	internal const int _SWP_NOACTIVATE = 0x0010;
	/// <summary>Message: returns if the last edit operation can be undone.</summary>
	internal const int _EM_CANUNDO = 0x00C6;
	/// <summary>Message: undoes the last edit operation.</summary>
	internal const int _WM_UNDO = 0x0304;
	/// <summary>SHAddToRecentDocs flag: the parameter is a Unicode path.</summary>
	internal const uint _SHARD_PATHW = 0x00000003;
	/// <summary>SHGetFileInfo flag: get the icon.</summary>
	internal const uint _SHGFI_ICON = 0x00000100;
	/// <summary>SHGetFileInfo flag: get the small icon.</summary>
	internal const uint _SHGFI_SMALLICON = 0x00000001;
	/// <summary>SHGetFileInfo flag: use the file attributes instead of accessing the file.</summary>
	internal const uint _SHGFI_USEFILEATTRIBUTES = 0x00000010;
	/// <summary>File attribute: directory.</summary>
	internal const uint _FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
	/// <summary>File attribute: normal file.</summary>
	internal const uint _FILE_ATTRIBUTE_NORMAL = 0x00000080;
	/// <summary>Message: gets the display order of the columns of a list view.</summary>
	internal const int _LVM_GETCOLUMNORDERARRAY = 0x103B;
	/// <summary>Message: sets the margins of an edit control.</summary>
	internal const int _EM_SETMARGINS = 0x00D3;
	/// <summary>Margin flag: left margin.</summary>
	internal const int _EC_LEFTMARGIN = 0x0001;
	/// <summary>Margin flag: right margin.</summary>
	internal const int _EC_RIGHTMARGIN = 0x0002;
	/// <summary>Clipboard format: Unicode text.</summary>
	internal const uint _CF_UNICODETEXT = 13;
	/// <summary>Message: gets the header control of a list view.</summary>
	internal const int _LVM_GETHEADER = 0x101F;
	/// <summary>Message: notification of a common control.</summary>
	internal const int _WM_NOTIFY = 0x004E;
	/// <summary>Notification: custom draw.</summary>
	internal const int _NM_CUSTOMDRAW = -12;
	/// <summary>Custom draw stage: before painting the control.</summary>
	internal const uint _CDDS_PREPAINT = 0x00000001;
	/// <summary>Custom draw stage: before painting an item.</summary>
	internal const uint _CDDS_ITEMPREPAINT = 0x00010001;
	/// <summary>Custom draw result: default painting.</summary>
	internal const int _CDRF_DEFAULT = 0x00000000;
	/// <summary>Custom draw result: notify before painting each item.</summary>
	internal const int _CDRF_NOTIFYITEMDRAW = 0x00000020;
	/// <summary>Custom draw result: the control does not paint the item.</summary>
	internal const int _CDRF_SKIPDEFAULT = 0x00000004;
	/// <summary>Message: notification of a common control, reflected to the control by Windows Forms.</summary>
	internal const int _WM_REFLECT_NOTIFY = 0x2000 + 0x004E;
	/// <summary>List view custom draw item type: group.</summary>
	internal const uint _LVCDI_GROUP = 0x00000001;
	/// <summary>Message: gets a rectangle of a list view group.</summary>
	internal const int _LVM_GETGROUPRECT = 0x1062;
	/// <summary>List view group rectangle: the header.</summary>
	internal const int _LVGGR_HEADER = 1;
	/// <summary>Message: paints the client area.</summary>
	internal const int _WM_PAINT = 0x000F;
	/// <summary>Message: owner drawn item, reflected to the control by Windows Forms.</summary>
	internal const int _WM_REFLECT_DRAWITEM = 0x2000 + 0x002B;
	/// <summary>Owner draw state: selected.</summary>
	internal const uint _ODS_SELECTED = 0x0001;
	/// <summary>Owner draw state: disabled.</summary>
	internal const uint _ODS_DISABLED = 0x0004;
	/// <summary>Owner draw state: focused.</summary>
	internal const uint _ODS_FOCUS = 0x0010;
	/// <summary>Owner draw state: no focus rectangle.</summary>
	internal const uint _ODS_NOFOCUSRECT = 0x0200;
	/// <summary>Custom draw stage: before painting a subitem.</summary>
	internal const uint _CDDS_SUBITEMPREPAINT = 0x00030001;
	/// <summary>Custom draw item state: selected.</summary>
	internal const uint _CDIS_SELECTED = 0x0001;
	/// <summary>Custom draw item state: focused.</summary>
	internal const uint _CDIS_FOCUS = 0x0010;
	/// <summary>Custom draw item state: under the mouse.</summary>
	internal const uint _CDIS_HOT = 0x0040;
	/// <summary>Custom draw result: the item uses the colors set in the notification.</summary>
	internal const int _CDRF_NEWFONT = 0x00000002;
	/// <summary>Message: gets the state of a list view item.</summary>
	internal const int _LVM_GETITEMSTATE = 0x102C;
	/// <summary>List view item state: selected.</summary>
	internal const int _LVIS_SELECTED = 0x0002;
	/// <summary>Custom draw stage: after an item has been drawn.</summary>
	internal const uint _CDDS_ITEMPOSTPAINT = 0x00010002;
	/// <summary>Custom draw result: send a notification after the item has been drawn.</summary>
	internal const int _CDRF_NOTIFYPOSTPAINT = 0x00000010;
	/// <summary>List view message: gets a rectangle of an item.</summary>
	internal const int _LVM_GETITEMRECT = 0x100E;
	/// <summary>List view message: gets a rectangle of a sub-item.</summary>
	internal const int _LVM_GETSUBITEMRECT = 0x1038;
	/// <summary>List view item rectangle: whole item.</summary>
	internal const int _LVIR_BOUNDS = 0;
	/// <summary>List view item rectangle: icon.</summary>
	internal const int _LVIR_ICON = 1;
	/// <summary>List view item rectangle: label.</summary>
	internal const int _LVIR_LABEL = 2;
	/// <summary>Tree view message: gets the rectangle of an item.</summary>
	internal const int _TVM_GETITEMRECT = 0x1104;
	/// <summary>Desktop window manager attribute: dark title bar (Windows 10 2004 and later).</summary>
	internal const int _DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
	/// <summary>Desktop window manager attribute: dark title bar (Windows 10 1809 to 1909).</summary>
	internal const int _DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19;
	/// <summary>Desktop window manager attribute: border color (Windows 11).</summary>
	internal const int _DWMWA_BORDER_COLOR = 34;
	/// <summary>Desktop window manager attribute: caption color (Windows 11).</summary>
	internal const int _DWMWA_CAPTION_COLOR = 35;
	/// <summary>Desktop window manager attribute: caption text color (Windows 11).</summary>
	internal const int _DWMWA_TEXT_COLOR = 36;
	/// <summary>Desktop window manager color value: default color.</summary>
	internal const int _DWMWA_COLOR_DEFAULT = unchecked((int) 0xFFFFFFFF);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the caret position in client coordinates of the window that owns the caret.</summary>
	/// <param name="point">Receives the caret position.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetCaretPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _GetCaretPos(out _Point point);


	/// <summary>Sets the show state of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="nCmdShow">Show command.</param>
	/// <returns>Returns <c>true</c> if the window was visible before, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "ShowWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _ShowWindow(IntPtr hWnd, int nCmdShow);


	/// <summary>Brings a window to the foreground and activates it.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns <c>true</c> if the window has been brought to the foreground, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SetForegroundWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _SetForegroundWindow(IntPtr hWnd);


	/// <summary>Allows a process to bring its windows to the foreground.</summary>
	/// <param name="processId">Process ID, <see cref="_ASFW_ANY"/> for all processes.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "AllowSetForegroundWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _AllowSetForegroundWindow(int processId);


	/// <summary>Returns if a window is visible.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns <c>true</c> if the window is visible, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "IsWindowVisible")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _IsWindowVisible(IntPtr hWnd);


	/// <summary>Changes the size, position and z-order of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="hWndInsertAfter">Window to precede in z-order, <see cref="IntPtr.Zero"/> for the top.</param>
	/// <param name="x">Horizontal position.</param>
	/// <param name="y">Vertical position.</param>
	/// <param name="cx">Width.</param>
	/// <param name="cy">Height.</param>
	/// <param name="flags">Position flags.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SetWindowPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);


	/// <summary>Gets the device context of a whole window, including the non-client area.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns the device context.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetWindowDC")]
	internal static partial IntPtr _GetWindowDC(IntPtr hWnd);


	/// <summary>Releases a device context.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="hDC">Device context.</param>
	/// <returns>Returns 1 if the device context has been released, otherwise returns 0.</returns>
	[LibraryImport("user32.dll", EntryPoint = "ReleaseDC")]
	internal static partial int _ReleaseDC(IntPtr hWnd, IntPtr hDC);


	/// <summary>Deletes a GDI object, for example a font.</summary>
	/// <param name="hObject">Object handle.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "DeleteObject")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _DeleteObject(IntPtr hObject);


	/// <summary>Gets a value of a window, for example its style.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="index">Index of the value.</param>
	/// <returns>Returns the value.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetWindowLongW")]
	internal static partial int _GetWindowLong(IntPtr hWnd, int index);


	/// <summary>Gets the bounds of a window in screen coordinates.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="rect">Receives the bounds.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetWindowRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _GetWindowRect(IntPtr hWnd, out _Rect rect);


	/// <summary>Creates a rectangular region.</summary>
	/// <param name="left">Left edge.</param>
	/// <param name="top">Top edge.</param>
	/// <param name="right">Right edge.</param>
	/// <param name="bottom">Bottom edge.</param>
	/// <returns>Returns the region handle; the caller deletes it with <see cref="_DeleteObject(IntPtr)"/>.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "CreateRectRgn")]
	internal static partial IntPtr _CreateRectRgn(int left, int top, int right, int bottom);


	/// <summary>Combines two regions.</summary>
	/// <param name="destination">Region that receives the result.</param>
	/// <param name="source1">First region.</param>
	/// <param name="source2">Second region.</param>
	/// <param name="mode">Combination mode.</param>
	/// <returns>Returns the type of the resulting region, 0 on error.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "CombineRgn")]
	internal static partial int _CombineRgn(IntPtr destination, IntPtr source1, IntPtr source2, int mode);


	/// <summary>Finds a child window.</summary>
	/// <param name="parent">Parent window handle.</param>
	/// <param name="childAfter">Child window to start the search after, <see cref="IntPtr.Zero"/> to start with the first child.</param>
	/// <param name="className">Window class name.</param>
	/// <param name="windowName">Window name, NULL for any name.</param>
	/// <returns>Returns the window handle, or <see cref="IntPtr.Zero"/> if no window has been found.</returns>
	[LibraryImport("user32.dll", EntryPoint = "FindWindowExW", StringMarshalling = StringMarshalling.Utf16)]
	internal static partial IntPtr _FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string? windowName);


	/// <summary>Sets an attribute of the desktop window manager for a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="attribute">Attribute.</param>
	/// <param name="value">Value.</param>
	/// <param name="size">Size of the value in bytes.</param>
	/// <returns>Returns 0 if successful, otherwise an error code.</returns>
	[LibraryImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
	internal static partial int _DwmSetWindowAttribute(IntPtr hWnd, int attribute, ref int value, int size);


	/// <summary>Sets the visual style of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="subAppName">Application name of the visual style, NULL for the default, an empty string for no visual style.</param>
	/// <param name="subIdList">Class list of the visual style, NULL for the default.</param>
	/// <returns>Returns 0 if successful, otherwise an error code.</returns>
	[LibraryImport("uxtheme.dll", EntryPoint = "SetWindowTheme", StringMarshalling = StringMarshalling.Utf16)]
	internal static partial int _SetWindowTheme(IntPtr hWnd, string? subAppName, string? subIdList);


	/// <summary>Sends a message to a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First message parameter.</param>
	/// <param name="lParam">Second message parameter.</param>
	/// <returns>Returns the result of the message.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
	internal static partial IntPtr _SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


	/// <summary>Sends a message with an integer array as second parameter to a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First message parameter.</param>
	/// <param name="lParam">Integer array.</param>
	/// <returns>Returns the result of the message.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
	internal static partial IntPtr _SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [In, Out] int[] lParam);


	/// <summary>Sends a message with a rectangle as second parameter to a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First message parameter.</param>
	/// <param name="lParam">Rectangle.</param>
	/// <returns>Returns the result of the message.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
	internal static partial IntPtr _SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref _Rect lParam);


	/// <summary>Sends a message with two integer results as parameters to a window, for example EM_GETSEL.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">Receives the first integer.</param>
	/// <param name="lParam">Receives the second integer.</param>
	/// <returns>Returns the result of the message.</returns>
	[LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
	internal static partial IntPtr _SendMessage(IntPtr hWnd, int msg, out int wParam, out int lParam);


	/// <summary>Creates a solid brush.</summary>
	/// <param name="color">Color as COLORREF (0x00BBGGRR).</param>
	/// <returns>Returns the brush handle.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "CreateSolidBrush")]
	internal static partial IntPtr _CreateSolidBrush(int color);


	/// <summary>Changes the protection of memory pages.</summary>
	/// <param name="address">Address.</param>
	/// <param name="size">Size in bytes.</param>
	/// <param name="protection">New protection.</param>
	/// <param name="oldProtection">Returns the previous protection.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise <c>false</c>.</returns>
	[LibraryImport("kernel32.dll", EntryPoint = "VirtualProtect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _VirtualProtect(IntPtr address, UIntPtr size, uint protection, out uint oldProtection);


	/// <summary>Gets the window that has the keyboard focus on the current thread.</summary>
	/// <returns>Returns the window handle, <see cref="IntPtr.Zero"/> if no window of the thread has the focus.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetFocus")]
	internal static partial IntPtr _GetFocus();


	/// <summary>Gets the device context of the client area of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns the device context; the caller releases it with <see cref="_ReleaseDC(IntPtr, IntPtr)"/>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetDC")]
	internal static partial IntPtr _GetDC(IntPtr hWnd);


	/// <summary>Saves the state of a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <returns>Returns the number of the saved state, 0 on error.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "SaveDC")]
	internal static partial int _SaveDC(IntPtr hdc);


	/// <summary>Restores a saved state of a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="state">Number of the saved state.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "RestoreDC")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _RestoreDC(IntPtr hdc, int state);


	/// <summary>Limits the clipping region of a device context to a rectangle.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="left">Left edge.</param>
	/// <param name="top">Top edge.</param>
	/// <param name="right">Right edge.</param>
	/// <param name="bottom">Bottom edge.</param>
	/// <returns>Returns the type of the clipping region, 0 on error.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "IntersectClipRect")]
	internal static partial int _IntersectClipRect(IntPtr hdc, int left, int top, int right, int bottom);


	/// <summary>Selects a GDI object, for example a font, into a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="hObject">Object handle.</param>
	/// <returns>Returns the previously selected object.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "SelectObject")]
	internal static partial IntPtr _SelectObject(IntPtr hdc, IntPtr hObject);


	/// <summary>Sets the background color of a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="color">Color as COLORREF (0x00BBGGRR).</param>
	/// <returns>Returns the previous background color.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "SetBkColor")]
	internal static partial int _SetBkColor(IntPtr hdc, int color);


	/// <summary>Draws text, optionally on a background rectangle.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="x">X coordinate.</param>
	/// <param name="y">Y coordinate.</param>
	/// <param name="options">Options.</param>
	/// <param name="rect">Rectangle for the background and clipping.</param>
	/// <param name="text">Text.</param>
	/// <param name="length">Length of the text.</param>
	/// <param name="dx">Character distances, NULL for the distances of the font.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "ExtTextOutW")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static unsafe partial bool _ExtTextOut(IntPtr hdc, int x, int y, int options, ref _Rect rect, char* text, int length, int* dx);


	/// <summary>Measures a text in the font of a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="text">Text.</param>
	/// <param name="length">Length of the text.</param>
	/// <param name="size">Receives the size.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static unsafe partial bool _GetTextExtentPoint32(IntPtr hdc, char* text, int length, out _Size size);


	/// <summary>Hides the caret of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns <c>true</c> if successful, <c>false</c> if the window has no caret.</returns>
	[LibraryImport("user32.dll", EntryPoint = "HideCaret")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _HideCaret(IntPtr hWnd);


	/// <summary>Shows the caret of a window after <see cref="_HideCaret(IntPtr)"/>.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns <c>true</c> if successful, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "ShowCaret")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _ShowCaret(IntPtr hWnd);


	/// <summary>Returns if a window is enabled.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns <c>true</c> if the window is enabled, otherwise returns <c>false</c>.</returns>
	[LibraryImport("user32.dll", EntryPoint = "IsWindowEnabled")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _IsWindowEnabled(IntPtr hWnd);


	/// <summary>Gets the length of the text of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <returns>Returns the length in characters.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetWindowTextLengthW")]
	internal static partial int _GetWindowTextLength(IntPtr hWnd);


	/// <summary>Gets the text of a window.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="text">Buffer for the text.</param>
	/// <param name="size">Size of the buffer in characters, including the terminating zero.</param>
	/// <returns>Returns the length of the text.</returns>
	[LibraryImport("user32.dll", EntryPoint = "GetWindowTextW")]
	internal static unsafe partial int _GetWindowText(IntPtr hWnd, char* text, int size);


	/// <summary>Sets the text color of a device context.</summary>
	/// <param name="hdc">Device context.</param>
	/// <param name="color">Color as COLORREF (0x00BBGGRR).</param>
	/// <returns>Returns the previous text color.</returns>
	[LibraryImport("gdi32.dll", EntryPoint = "SetTextColor")]
	internal static partial int _SetTextColor(IntPtr hdc, int color);


	/// <summary>Invalidates a window so it is painted again.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="rect">Rectangle, IntPtr.Zero for the whole window.</param>
	/// <param name="erase">Determines if the background is erased.</param>
	/// <returns>Returns <c>true</c> if successful.</returns>
	[LibraryImport("user32.dll", EntryPoint = "InvalidateRect")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _InvalidateRect(IntPtr hWnd, IntPtr rect, [MarshalAs(UnmanagedType.Bool)] bool erase);


	/// <summary>Adds a file to the recent documents of the shell (Windows recent files and jump lists).</summary>
	/// <param name="flags">Flags.</param>
	/// <param name="path">Path.</param>
	[LibraryImport("shell32.dll", EntryPoint = "SHAddToRecentDocs", StringMarshalling = StringMarshalling.Utf16)]
	internal static partial void _SHAddToRecentDocs(uint flags, string path);


	/// <summary>Gets information about a file, for example its icon.</summary>
	/// <param name="path">Path.</param>
	/// <param name="fileAttributes">File attributes (used with SHGFI_USEFILEATTRIBUTES).</param>
	/// <param name="info">Receives the information.</param>
	/// <param name="infoSize">Size of the structure.</param>
	/// <param name="flags">Flags.</param>
	/// <returns>Returns a non-zero value on success.</returns>
	[LibraryImport("shell32.dll", EntryPoint = "SHGetFileInfoW", StringMarshalling = StringMarshalling.Utf16)]
	internal static partial IntPtr _SHGetFileInfo(string path, uint fileAttributes, ref _SHFILEINFO info, uint infoSize, uint flags);


	/// <summary>Destroys an icon.</summary>
	/// <param name="hIcon">Icon handle.</param>
	/// <returns>Returns <c>true</c> on success.</returns>
	[LibraryImport("user32.dll", EntryPoint = "DestroyIcon")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _DestroyIcon(IntPtr hIcon);


	/// <summary>Returns if the clipboard contains data in a format.</summary>
	/// <param name="format">Clipboard format.</param>
	/// <returns>Returns <c>true</c> if the format is available, otherwise returns <c>false</c>.</returns>
	/// <remarks>The clipboard does not need to be opened, so this function does not fail if another application holds the clipboard.</remarks>
	[LibraryImport("user32.dll", EntryPoint = "IsClipboardFormatAvailable")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool _IsClipboardFormatAvailable(uint format);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _Point                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a Win32 POINT structure.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _Point
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>X coordinate.</summary>
		public int X;

		/// <summary>Y coordinate.</summary>
		public int Y;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _Rect                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a Win32 RECT structure.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _Rect
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Left edge.</summary>
		public int Left;

		/// <summary>Top edge.</summary>
		public int Top;

		/// <summary>Right edge.</summary>
		public int Right;

		/// <summary>Bottom edge.</summary>
		public int Bottom;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _Size                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a Win32 SIZE structure.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _Size
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Width.</summary>
		public int Width;

		/// <summary>Height.</summary>
		public int Height;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _NMHDR                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents the header of a Win32 notification (NMHDR).</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _NMHDR
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Window that sends the notification.</summary>
		public IntPtr hwndFrom;

		/// <summary>Control identifier.</summary>
		public UIntPtr idFrom;

		/// <summary>Notification code.</summary>
		public int code;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _NMCUSTOMDRAW                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a Win32 custom draw notification (NMCUSTOMDRAW).</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _NMCUSTOMDRAW
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Notification header.</summary>
		public _NMHDR hdr;

		/// <summary>Drawing stage.</summary>
		public uint dwDrawStage;

		/// <summary>Device context.</summary>
		public IntPtr hdc;

		/// <summary>Left edge of the bounds.</summary>
		public int left;

		/// <summary>Top edge of the bounds.</summary>
		public int top;

		/// <summary>Right edge of the bounds.</summary>
		public int right;

		/// <summary>Bottom edge of the bounds.</summary>
		public int bottom;

		/// <summary>Item.</summary>
		public UIntPtr dwItemSpec;

		/// <summary>Item state.</summary>
		public uint uItemState;

		/// <summary>Application data of the item.</summary>
		public IntPtr lItemlParam;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _NMLVCUSTOMDRAW                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents a Win32 list view custom draw notification (NMLVCUSTOMDRAW).</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _NMLVCUSTOMDRAW
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Custom draw notification.</summary>
		public _NMCUSTOMDRAW nmcd;

		/// <summary>Text color.</summary>
		public int clrText;

		/// <summary>Text background color.</summary>
		public int clrTextBk;

		/// <summary>Subitem.</summary>
		public int iSubItem;

		/// <summary>Item type.</summary>
		public uint dwItemType;

		/// <summary>Face color.</summary>
		public int clrFace;

		/// <summary>Icon effect.</summary>
		public int iIconEffect;

		/// <summary>Icon phase.</summary>
		public int iIconPhase;

		/// <summary>Part of the visual style.</summary>
		public int iPartId;

		/// <summary>State of the visual style.</summary>
		public int iStateId;

		/// <summary>Text bounds.</summary>
		public _Rect rcText;

		/// <summary>Alignment.</summary>
		public uint uAlign;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _DRAWITEMSTRUCT                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct represents the Win32 owner draw information of an item (DRAWITEMSTRUCT).</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct _DRAWITEMSTRUCT
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Control type.</summary>
		public uint CtlType;

		/// <summary>Control identifier.</summary>
		public uint CtlID;

		/// <summary>Item index, -1 for an empty list.</summary>
		public int itemID;

		/// <summary>Drawing action.</summary>
		public uint itemAction;

		/// <summary>Item state.</summary>
		public uint itemState;

		/// <summary>Control window.</summary>
		public IntPtr hwndItem;

		/// <summary>Device context.</summary>
		public IntPtr hDC;

		/// <summary>Item bounds.</summary>
		public _Rect rcItem;

		/// <summary>Application data of the item.</summary>
		public UIntPtr itemData;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] _SHFILEINFO                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This struct receives file information from SHGetFileInfo.</summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct _SHFILEINFO
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public members                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Icon handle.</summary>
		public IntPtr hIcon;

		/// <summary>Icon index.</summary>
		public int iIcon;

		/// <summary>Attributes.</summary>
		public uint dwAttributes;

		/// <summary>Display name.</summary>
		public fixed char szDisplayName[260];

		/// <summary>Type name.</summary>
		public fixed char szTypeName[80];
	}
}
