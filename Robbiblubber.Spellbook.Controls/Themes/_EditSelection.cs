namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class draws the selection of edit controls in the selection colors of a theme.</summary>
/// <remarks><para>The edit control of Windows gets its selection colors internally, so <see cref="_SysColorHook"/> does not reach it. After
///          the edit control has handled a message that may have drawn the selection, this class draws the selected text again in the
///          colors of the theme (<see cref="_Update(ref Message, IntPtr, Theme)"/>). Rich edit controls are left out, they get their
///          colors from <see cref="_SysColorHook"/>.</para>
///          <para>The class draws the visible lines of the selection with the font of the edit control at the positions the control
///          reports (<c>EM_POSFROMCHAR</c>); tabs are filled, password characters are kept. While the edit control draws the selection
///          itself, for example while the mouse selects text, the system color may show for a moment.</para></remarks>
[SupportedOSPlatform("windows")]
internal static unsafe class _EditSelection
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Message: size changed.</summary>
	private const int _WM_SIZE = 0x0005;

	/// <summary>Message: focus received.</summary>
	private const int _WM_SETFOCUS = 0x0007;

	/// <summary>Message: focus lost.</summary>
	private const int _WM_KILLFOCUS = 0x0008;

	/// <summary>Message: enabled or disabled.</summary>
	private const int _WM_ENABLE = 0x000A;

	/// <summary>Message: set the text.</summary>
	private const int _WM_SETTEXT = 0x000C;

	/// <summary>Message: paint.</summary>
	private const int _WM_PAINT = 0x000F;

	/// <summary>Message: set the font.</summary>
	private const int _WM_SETFONT = 0x0030;

	/// <summary>Message: get the font.</summary>
	private const int _WM_GETFONT = 0x0031;

	/// <summary>Message: context menu.</summary>
	private const int _WM_CONTEXTMENU = 0x007B;

	/// <summary>Message: key down.</summary>
	private const int _WM_KEYDOWN = 0x0100;

	/// <summary>Message: key up.</summary>
	private const int _WM_KEYUP = 0x0101;

	/// <summary>Message: character.</summary>
	private const int _WM_CHAR = 0x0102;

	/// <summary>Message: system key down.</summary>
	private const int _WM_SYSKEYDOWN = 0x0104;

	/// <summary>Message: IME composition.</summary>
	private const int _WM_IME_COMPOSITION = 0x010F;

	/// <summary>Message: timer, used by the edit control to scroll while the mouse selects text.</summary>
	private const int _WM_TIMER = 0x0113;

	/// <summary>Message: horizontal scrolling.</summary>
	private const int _WM_HSCROLL = 0x0114;

	/// <summary>Message: vertical scrolling.</summary>
	private const int _WM_VSCROLL = 0x0115;

	/// <summary>Message: mouse moved.</summary>
	private const int _WM_MOUSEMOVE = 0x0200;

	/// <summary>Message: left mouse button down.</summary>
	private const int _WM_LBUTTONDOWN = 0x0201;

	/// <summary>Message: left mouse button up.</summary>
	private const int _WM_LBUTTONUP = 0x0202;

	/// <summary>Message: left mouse button double click.</summary>
	private const int _WM_LBUTTONDBLCLK = 0x0203;

	/// <summary>Message: mouse wheel.</summary>
	private const int _WM_MOUSEWHEEL = 0x020A;

	/// <summary>Message: horizontal mouse wheel.</summary>
	private const int _WM_MOUSEHWHEEL = 0x020E;

	/// <summary>Message: mouse capture lost.</summary>
	private const int _WM_CAPTURECHANGED = 0x0215;

	/// <summary>Message: cut.</summary>
	private const int _WM_CUT = 0x0300;

	/// <summary>Message: paste.</summary>
	private const int _WM_PASTE = 0x0302;

	/// <summary>Message: delete the selection.</summary>
	private const int _WM_CLEAR = 0x0303;

	/// <summary>Message: undo.</summary>
	private const int _WM_UNDO = 0x0304;

	/// <summary>Edit message: get the selection.</summary>
	private const int _EM_GETSEL = 0x00B0;

	/// <summary>Edit message: set the selection.</summary>
	private const int _EM_SETSEL = 0x00B1;

	/// <summary>Edit message: get the formatting rectangle.</summary>
	private const int _EM_GETRECT = 0x00B2;

	/// <summary>Edit message: scroll vertically.</summary>
	private const int _EM_SCROLL = 0x00B5;

	/// <summary>Edit message: scroll by lines.</summary>
	private const int _EM_LINESCROLL = 0x00B6;

	/// <summary>Edit message: scroll the caret into view.</summary>
	private const int _EM_SCROLLCARET = 0x00B7;

	/// <summary>Edit message: get the number of lines.</summary>
	private const int _EM_GETLINECOUNT = 0x00BA;

	/// <summary>Edit message: get the first character of a line.</summary>
	private const int _EM_LINEINDEX = 0x00BB;

	/// <summary>Edit message: get the length of the line of a character.</summary>
	private const int _EM_LINELENGTH = 0x00C1;

	/// <summary>Edit message: replace the selection.</summary>
	private const int _EM_REPLACESEL = 0x00C2;

	/// <summary>Edit message: undo.</summary>
	private const int _EM_UNDO = 0x00C7;

	/// <summary>Edit message: get the line of a character.</summary>
	private const int _EM_LINEFROMCHAR = 0x00C9;

	/// <summary>Edit message: get the first visible line.</summary>
	private const int _EM_GETFIRSTVISIBLELINE = 0x00CE;

	/// <summary>Edit message: set the read-only state.</summary>
	private const int _EM_SETREADONLY = 0x00CF;

	/// <summary>Edit message: get the password character.</summary>
	private const int _EM_GETPASSWORDCHAR = 0x00D2;

	/// <summary>Edit message: set the margins.</summary>
	private const int _EM_SETMARGINS = 0x00D3;

	/// <summary>Edit message: get the position of a character.</summary>
	private const int _EM_POSFROMCHAR = 0x00D6;

	/// <summary>Edit style: multiple lines.</summary>
	private const int _ES_MULTILINE = 0x0004;

	/// <summary>Edit style: the selection is shown without the focus.</summary>
	private const int _ES_NOHIDESEL = 0x0100;

	/// <summary>Mouse key state: left button down.</summary>
	private const int _MK_LBUTTON = 0x0001;

	/// <summary>ExtTextOut option: fill the rectangle with the background color.</summary>
	private const int _ETO_OPAQUE = 0x0002;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Determines if the selection is drawn on the current thread, so the messages sent while drawing are not handled.</summary>
	[ThreadStatic]
	private static bool _Drawing;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the edit control may have drawn its selection while it handled a message.</summary>
	/// <param name="m">Message.</param>
	/// <returns>Returns <c>true</c> if the selection may have been drawn, otherwise returns <c>false</c>.</returns>
	private static bool _Affects(ref Message m)
	{
		switch(m.Msg)
		{
			case _WM_MOUSEMOVE:
				return ((m.WParam.ToInt64() & _MK_LBUTTON) != 0);
			case _WM_PAINT: case _WM_SIZE: case _WM_SETFOCUS: case _WM_KILLFOCUS: case _WM_ENABLE: case _WM_SETTEXT: case _WM_SETFONT:
			case _WM_CONTEXTMENU: case _WM_KEYDOWN: case _WM_KEYUP: case _WM_CHAR: case _WM_SYSKEYDOWN: case _WM_IME_COMPOSITION:
			case _WM_TIMER: case _WM_HSCROLL: case _WM_VSCROLL: case _WM_LBUTTONDOWN: case _WM_LBUTTONUP: case _WM_LBUTTONDBLCLK:
			case _WM_MOUSEWHEEL: case _WM_MOUSEHWHEEL: case _WM_CAPTURECHANGED: case _WM_CUT: case _WM_PASTE: case _WM_CLEAR:
			case _WM_UNDO: case _EM_SETSEL: case _EM_SCROLL: case _EM_LINESCROLL: case _EM_SCROLLCARET: case _EM_REPLACESEL:
			case _EM_UNDO: case _EM_SETREADONLY: case _EM_SETMARGINS:
				return true;
		}

		return false;
	}


	/// <summary>Sends a message with integer parameters to an edit control.</summary>
	/// <param name="handle">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First parameter.</param>
	/// <returns>Returns the result as integer.</returns>
	private static int _Send(IntPtr handle, int msg, int wParam = 0)
	{
		return (int) _NativeMethods._SendMessage(handle, msg, (IntPtr) wParam, IntPtr.Zero).ToInt64();
	}


	/// <summary>Gets the position of a character in an edit control.</summary>
	/// <param name="handle">Window handle.</param>
	/// <param name="index">Character index.</param>
	/// <param name="position">Returns the position in client coordinates.</param>
	/// <returns>Returns <c>true</c> if the position has been found, otherwise returns <c>false</c>.</returns>
	private static bool _GetPosition(IntPtr handle, int index, out Point position)
	{
		int rval = _Send(handle, _EM_POSFROMCHAR, index);
		position = new Point((short) (rval & 0xFFFF), (short) ((rval >> 16) & 0xFFFF));

		return (rval != -1);
	}


	/// <summary>Gets the text of an edit control.</summary>
	/// <param name="handle">Window handle.</param>
	/// <returns>Returns the text.</returns>
	private static string _GetText(IntPtr handle)
	{
		int length = _NativeMethods._GetWindowTextLength(handle);
		if(length <= 0) { return string.Empty; }

		char[] buffer = new char[length + 1];
		fixed(char* p = buffer)
		{
			length = _NativeMethods._GetWindowText(handle, p, buffer.Length);
		}

		return new string(buffer, 0, Math.Max(0, length));
	}


	/// <summary>Draws a part of a line of the selection.</summary>
	/// <param name="hdc">Device context with the font and colors of the selection.</param>
	/// <param name="handle">Window handle.</param>
	/// <param name="text">Text of the edit control.</param>
	/// <param name="start">First character.</param>
	/// <param name="end">Character after the last character; the characters are on one line.</param>
	/// <param name="password">Password character, 0 for none.</param>
	/// <param name="height">Line height.</param>
	private static void _DrawLine(IntPtr hdc, IntPtr handle, string text, int start, int end, char password, int height)
	{
		int i = start;
		while(i < end)
		{
			if(text[i] == '\t')
			{
				if(_GetPosition(handle, i, out Point left) && ((i + 1) < text.Length) && _GetPosition(handle, i + 1, out Point right) && (right.Y == left.Y))
				{
					_NativeMethods._Rect area = new() { Left = left.X, Top = left.Y, Right = right.X, Bottom = left.Y + height };
					_NativeMethods._ExtTextOut(hdc, left.X, left.Y, _ETO_OPAQUE, ref area, null, 0, null);
				}
				i++;
				continue;
			}

			int j = i;
			while((j < end) && (text[j] != '\t')) { j++; }

			if(_GetPosition(handle, i, out Point position))
			{
				string part = ((password == '\0') ? text[i..j] : new string(password, j - i));
				fixed(char* p = part)
				{
					_NativeMethods._GetTextExtentPoint32(hdc, p, part.Length, out _NativeMethods._Size size);
					_NativeMethods._Rect area = new() { Left = position.X, Top = position.Y, Right = position.X + size.Width, Bottom = position.Y + height };
					_NativeMethods._ExtTextOut(hdc, position.X, position.Y, _ETO_OPAQUE, ref area, p, part.Length, null);
				}
			}
			i = j;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the selection of an edit control into a device context.</summary>
	/// <param name="handle">Window handle of the edit control.</param>
	/// <param name="hdc">Device context of the client area.</param>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns <c>true</c> if a selection has been drawn, otherwise returns <c>false</c>.</returns>
	/// <remarks>Without the focus, the selection is drawn in the inactive selection colors if the edit control shows it.</remarks>
	internal static bool _Draw(IntPtr handle, IntPtr hdc, Theme theme)
	{
		_NativeMethods._SendMessage(handle, _EM_GETSEL, out int start, out int end);
		if(end <= start) { return false; }

		int style = _NativeMethods._GetWindowLong(handle, _NativeMethods._GWL_STYLE);
		bool focused = (_NativeMethods._GetFocus() == handle);
		if((!focused && ((style & _ES_NOHIDESEL) == 0)) || !_NativeMethods._IsWindowEnabled(handle)) { return false; }

		string text = _GetText(handle);
		end = Math.Min(end, text.Length);
		if(end <= start) { return false; }

		bool multiline = ((style & _ES_MULTILINE) != 0);
		char password = (multiline ? '\0' : (char) _Send(handle, _EM_GETPASSWORDCHAR));
		_NativeMethods._Rect bounds = new();
		_NativeMethods._SendMessage(handle, _EM_GETRECT, IntPtr.Zero, ref bounds);

		int state = _NativeMethods._SaveDC(hdc);
		try
		{
			_NativeMethods._IntersectClipRect(hdc, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
			IntPtr font = _NativeMethods._SendMessage(handle, _WM_GETFONT, IntPtr.Zero, IntPtr.Zero);
			if(font != IntPtr.Zero) { _NativeMethods._SelectObject(hdc, font); }
			_NativeMethods._SetBkColor(hdc, ColorTranslator.ToWin32(theme[focused ? ThemeColor.SELECTION_BACK : ThemeColor.INACTIVE_SELECTION_BACK]));
			_NativeMethods._SetTextColor(hdc, ColorTranslator.ToWin32(theme[focused ? ThemeColor.SELECTION_TEXT : ThemeColor.INACTIVE_SELECTION_TEXT]));

			char x = 'X';
			_NativeMethods._GetTextExtentPoint32(hdc, &x, 1, out _NativeMethods._Size size);

			if(!multiline)
			{
				_DrawLine(hdc, handle, text, start, end, password, size.Height);
				return true;
			}

			int count = _Send(handle, _EM_GETLINECOUNT);
			for(int line = Math.Max(_Send(handle, _EM_GETFIRSTVISIBLELINE), _Send(handle, _EM_LINEFROMCHAR, start)); line < count; line++)
			{
				int lineStart = _Send(handle, _EM_LINEINDEX, line);
				if((lineStart < 0) || (lineStart >= end)) { break; }
				if(_GetPosition(handle, lineStart, out Point position) && (position.Y >= bounds.Bottom)) { break; }

				int lineEnd = Math.Min(lineStart + _Send(handle, _EM_LINELENGTH, lineStart), text.Length);
				int from = Math.Max(start, lineStart), to = Math.Min(end, lineEnd);
				if(from < to) { _DrawLine(hdc, handle, text, from, to, password, size.Height); }
			}
		}
		finally
		{
			_NativeMethods._RestoreDC(hdc, state);
		}

		return true;
	}


	/// <summary>Draws the selection of an edit control in the colors of a theme after the control has handled a message.</summary>
	/// <param name="m">Message the edit control has handled.</param>
	/// <param name="handle">Window handle of the edit control.</param>
	/// <param name="theme">Theme.</param>
	internal static void _Update(ref Message m, IntPtr handle, Theme theme)
	{
		if(_Drawing || (handle == IntPtr.Zero) || !_Affects(ref m) || !_NativeMethods._IsWindowVisible(handle) || (Control.FromHandle(handle) is RichTextBox)) { return; }
		_NativeMethods._SendMessage(handle, _EM_GETSEL, out int start, out int end);
		if(end <= start) { return; }

		_Drawing = true;
		IntPtr hdc = _NativeMethods._GetDC(handle);
		bool caret = _NativeMethods._HideCaret(handle);
		try
		{
			if(hdc != IntPtr.Zero) { _Draw(handle, hdc, theme); }
		}
		finally
		{
			if(caret) { _NativeMethods._ShowCaret(handle); }
			if(hdc != IntPtr.Zero) { _NativeMethods._ReleaseDC(handle, hdc); }
			_Drawing = false;
		}
	}
}
