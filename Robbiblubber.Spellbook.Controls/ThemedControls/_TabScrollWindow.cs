namespace Robbiblubber.Spellbook.Controls;

using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using TabSlot = Robbiblubber.Spellbook.Controls.ThemedTabControlColors._Slot;



/// <summary>This class draws the scroll buttons of a <see cref="ThemedTabControl"/> in the colors of its color table.</summary>
/// <remarks><para>When the tabs do not fit in one row, the tab control shows an up-down control (<c>msctls_updown32</c>) with two
///          arrows. The class subclasses it and paints both buttons flat like the tabs: the background of the tab strip, a frame in
///          the border color, the button under the mouse highlighted, the pressed button framed in the accent color, and the arrow
///          in the text color or, if the tabs cannot scroll further in its direction, in the disabled color.</para>
///          <para>The up-down control keeps working as before; only its painting is replaced.</para></remarks>
[SupportedOSPlatform("windows")]
internal sealed class _TabScrollWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Tab control.</summary>
	private readonly ThemedTabControl _Owner;

	/// <summary>Button under the mouse: 0 for the left one, 1 for the right one, -1 for none.</summary>
	private int _Hot = -1;

	/// <summary>Button pressed: 0 for the left one, 1 for the right one, -1 for none.</summary>
	private int _Pressed = -1;

	/// <summary>Determines if the window reports when the mouse leaves it.</summary>
	private bool _Tracking = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class and subclasses the up-down control.</summary>
	/// <param name="owner">Tab control.</param>
	/// <param name="handle">Handle of the up-down control.</param>
	internal _TabScrollWindow(ThemedTabControl owner, IntPtr handle)
	{
		_Owner = owner;
		AssignHandle(handle);
		_Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Paints the buttons again.</summary>
	internal void _Invalidate()
	{
		if(Handle != IntPtr.Zero) { _NativeMethods._InvalidateRect(Handle, IntPtr.Zero, false); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the button at a position.</summary>
	/// <param name="lParam">Position as in a mouse message.</param>
	/// <param name="width">Width of the control.</param>
	/// <returns>Returns 0 for the left button, 1 for the right one.</returns>
	private static int _GetButton(IntPtr lParam, int width)
	{
		int x = unchecked((short) (lParam.ToInt64() & 0xFFFF));
		return ((x < (width / 2)) ? 0 : 1);
	}


	/// <summary>Gets if the tabs can scroll in the direction of a button.</summary>
	/// <param name="button">Button: 0 for the left one, 1 for the right one.</param>
	/// <returns>Returns TRUE if they can, otherwise returns FALSE.</returns>
	private bool _CanScroll(int button)
	{
		_NativeMethods._SendMessage(Handle, _NativeMethods._UDM_GETRANGE32, out int low, out int high);
		int position = (int) _NativeMethods._SendMessage(Handle, _NativeMethods._UDM_GETPOS32, IntPtr.Zero, IntPtr.Zero);
		return ((button == 0) ? (position > Math.Min(low, high)) : (position < Math.Max(low, high)));
	}


	/// <summary>Gets the width of the client area.</summary>
	/// <returns>Returns the width.</returns>
	private int _GetWidth()
	{
		return (_NativeMethods._GetClientRect(Handle, out _NativeMethods._Rect r) ? (r.Right - r.Left) : 0);
	}


	/// <summary>Paints the buttons.</summary>
	private void _Paint()
	{
		if(!_NativeMethods._GetClientRect(Handle, out _NativeMethods._Rect r)) return;
		Rectangle bounds = Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
		if((bounds.Width < 4) || (bounds.Height < 4)) return;

		ThemedTabControlColors colors = _Owner._ColorTable;
		bool enabled = _Owner.Enabled;
		Color back = (_Owner.Parent?.BackColor ?? Color.Empty);
		if(back.A < 255) { back = colors._Get(TabSlot.PAGE_BACK_COLOR); }

		using Graphics g = Graphics.FromHwnd(Handle);
		using(SolidBrush brush = new(back)) { g.FillRectangle(brush, bounds); }

		int half = (bounds.Width / 2);
		for(int i = 0; i < 2; i++)
		{
			Rectangle button = ((i == 0) ? new Rectangle(bounds.X, bounds.Y, half, bounds.Height) : Rectangle.FromLTRB(bounds.X + half, bounds.Y, bounds.Right, bounds.Bottom));
			button = Rectangle.FromLTRB(button.Left + ((i == 0) ? 0 : 1), button.Top, button.Right - ((i == 0) ? 1 : 0), button.Bottom);
			bool can = (enabled && _CanScroll(i));

			Color face = back;
			Color frame = colors._Get(enabled ? TabSlot.BORDER_COLOR : TabSlot.DISABLED_BORDER_COLOR);
			if(can && (i == _Pressed)) { frame = colors._Get(TabSlot.ACCENT_COLOR); }
			if(can && ((i == _Hot) || (i == _Pressed))) { face = colors._Get(TabSlot.HOT_BACK_COLOR); }

			using(SolidBrush brush = new(face)) { g.FillRectangle(brush, button); }
			using(Pen pen = new(frame)) { g.DrawRectangle(pen, button.X, button.Y, (button.Width - 1), (button.Height - 1)); }

			int size = Math.Max(2, (Math.Min(button.Width, button.Height) / 4));
			int cx = (button.X + (button.Width / 2));
			int cy = (button.Y + (button.Height / 2));
			Point[] arrow = ((i == 0) ? [ new Point((cx + (size / 2)), (cy - size)), new Point((cx + (size / 2)), (cy + size)), new Point((cx - (size / 2) - 1), cy) ]
									  : [ new Point((cx - (size / 2)), (cy - size)), new Point((cx - (size / 2)), (cy + size)), new Point((cx + (size / 2) + 1), cy) ]);

			g.SmoothingMode = SmoothingMode.AntiAlias;
			using(SolidBrush brush = new(colors._Get(can ? TabSlot.TEXT_COLOR : TabSlot.DISABLED_TEXT_COLOR))) { g.FillPolygon(brush, arrow); }
			g.SmoothingMode = SmoothingMode.Default;
		}
	}


	/// <summary>Asks for <see cref="_NativeMethods._WM_MOUSELEAVE"/> when the mouse leaves the control.</summary>
	private void _Track()
	{
		if(_Tracking) return;

		_NativeMethods._TrackMouseEventInfo info = new()
		{
			cbSize = Marshal.SizeOf<_NativeMethods._TrackMouseEventInfo>(),
			dwFlags = _NativeMethods._TME_LEAVE,
			hwndTrack = Handle
		};
		_Tracking = _NativeMethods._TrackMouseEvent(ref info);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		switch(m.Msg)
		{
			case _NativeMethods._WM_PAINT:
				_Paint();
				_NativeMethods._ValidateRect(Handle, IntPtr.Zero);
				return;
			case _NativeMethods._WM_ERASEBKGND:
				m.Result = (IntPtr) 1;
				return;
			case _NativeMethods._WM_MOUSEMOVE:
				_Track();
				int hot = _GetButton(m.LParam, _GetWidth());
				if(hot != _Hot)
				{
					_Hot = hot;
					_Invalidate();
				}
				break;
			case _NativeMethods._WM_MOUSELEAVE:
				_Tracking = false;
				_Hot = -1;
				_Invalidate();
				break;
			case _NativeMethods._WM_LBUTTONDOWN:
				_Pressed = _GetButton(m.LParam, _GetWidth());
				_Invalidate();
				break;
			case _NativeMethods._WM_LBUTTONUP:
				_Pressed = -1;
				_Invalidate();
				break;
		}

		base.WndProc(ref m);

		switch(m.Msg)
		{
			case _NativeMethods._UDM_SETPOS:
			case _NativeMethods._UDM_SETPOS32:
			case _NativeMethods._WM_LBUTTONDOWN:
			case _NativeMethods._WM_LBUTTONUP:
				_Invalidate();
				break;
		}
	}
}
