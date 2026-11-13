namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;

using Slot = Robbiblubber.Spellbook.Controls.ThemedTextBoxColors._Slot;



/// <summary>This class draws the border of text boxes in the non-client area.</summary>
/// <remarks><para>The text box keeps its native border style, so its layout does not change, but Windows does not draw the border:
///          the class handles <c>WM_NCPAINT</c> itself and draws the border in the colors of the theme. A
///          <see cref="BorderStyle.FixedSingle"/> border is one pixel wide, a <see cref="BorderStyle.Fixed3D"/> border two pixels: the
///          outer pixel gets the border color, the inner one the back color.</para>
///          <para>Painting over the native border would flicker, because Windows draws it first and animates it when the mouse enters
///          or leaves the text box. A text box with scroll bars gets <c>WM_NCPAINT</c> with a region without the border, so Windows
///          draws only the scroll bars. Changes of the state (mouse, focus) draw the border directly, without <c>WM_NCPAINT</c>.</para>
///          </remarks>
[SupportedOSPlatform("windows")]
internal static class _FieldBorder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [delegate] _WindowProcedure                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Window procedure that handles a message, usually the window procedure of the base class.</summary>
	/// <param name="m">Message.</param>
	internal delegate void _WindowProcedure(ref Message m);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the width of a border.</summary>
	/// <param name="style">Border style.</param>
	/// <returns>Returns the width in pixels.</returns>
	private static int _GetWidth(BorderStyle style)
	{
		return style switch
		{
			BorderStyle.Fixed3D => SystemInformation.Border3DSize.Width,
			BorderStyle.FixedSingle => SystemInformation.BorderSize.Width,
			_ => 0
		};
	}


	/// <summary>Draws the border into a device context of the whole window.</summary>
	/// <param name="dc">Device context.</param>
	/// <param name="control">Control.</param>
	/// <param name="width">Border width.</param>
	/// <param name="border">Border color.</param>
	/// <param name="back">Back color.</param>
	private static void _Draw(IntPtr dc, Control control, int width, Color border, Color back)
	{
		using Graphics g = Graphics.FromHdc(dc);
		using(Pen pen = new(border)) { g.DrawRectangle(pen, 0, 0, control.Width - 1, control.Height - 1); }

		if(width < 2) { return; }
		using(Pen pen = new(back))
		{
			for(int i = 1; i < width; i++) { g.DrawRectangle(pen, i, i, control.Width - 1 - (2 * i), control.Height - 1 - (2 * i)); }
		}
	}


	/// <summary>Draws the border into the window.</summary>
	/// <param name="control">Control.</param>
	/// <param name="width">Border width.</param>
	/// <param name="border">Border color.</param>
	/// <param name="back">Back color.</param>
	private static void _DrawWindow(Control control, int width, Color border, Color back)
	{
		IntPtr dc = _NativeMethods._GetWindowDC(control.Handle);
		if(dc == IntPtr.Zero) { return; }

		try
		{
			_Draw(dc, control, width, border, back);
		}
		finally { _NativeMethods._ReleaseDC(control.Handle, dc); }
	}


	/// <summary>Lets the window procedure draw the scroll bars, but not the border.</summary>
	/// <param name="m">Message <c>WM_NCPAINT</c>.</param>
	/// <param name="width">Border width.</param>
	/// <param name="windowProcedure">Window procedure.</param>
	private static void _DrawScrollBars(ref Message m, int width, _WindowProcedure windowProcedure)
	{
		if(!_NativeMethods._GetWindowRect(m.HWnd, out _NativeMethods._Rect r)) { return; }

		IntPtr region = _NativeMethods._CreateRectRgn(r.Left + width, r.Top + width, r.Right - width, r.Bottom - width);
		if(region == IntPtr.Zero) { return; }

		IntPtr original = m.WParam;
		try
		{
			if(original.ToInt64() > 1) { _NativeMethods._CombineRgn(region, region, original, _NativeMethods._RGN_AND); }

			m.WParam = region;
			windowProcedure(ref m);
		}
		finally
		{
			m.WParam = original;
			_NativeMethods._DeleteObject(region);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the border color for the state of a text box.</summary>
	/// <param name="control">Text box.</param>
	/// <param name="colors">Color table.</param>
	/// <param name="hot">Determines if the mouse is over the text box.</param>
	/// <returns>Returns the color.</returns>
	internal static Color _GetColor(Control control, ThemedTextBoxColors colors, bool hot)
	{
		if(!control.Enabled) { return colors._Get(Slot.DISABLED_BORDER_COLOR); }
		if(control.Focused) { return colors._Get(Slot.FOCUS_COLOR); }

		return colors._Get(hot ? Slot.HOT_BORDER_COLOR : Slot.BORDER_COLOR);
	}


	/// <summary>Returns if a message paints the border.</summary>
	/// <param name="m">Message.</param>
	/// <returns>Returns <c>true</c> for <c>WM_NCPAINT</c> and <c>WM_PRINT</c>, otherwise returns <c>false</c>.</returns>
	internal static bool _IsPaintMessage(ref Message m)
	{
		return ((m.Msg == _NativeMethods._WM_NCPAINT) || (m.Msg == _NativeMethods._WM_PRINT));
	}


	/// <summary>Handles a message that paints the border.</summary>
	/// <param name="m">Message <c>WM_NCPAINT</c> or <c>WM_PRINT</c>.</param>
	/// <param name="control">Text box.</param>
	/// <param name="style">Border style.</param>
	/// <param name="border">Border color.</param>
	/// <param name="back">Back color.</param>
	/// <param name="windowProcedure">Window procedure of the text box.</param>
	/// <remarks><c>WM_NCPAINT</c> is handled without the window procedure, unless the text box shows scroll bars; <c>WM_PRINT</c> with
	///          <c>PRF_NONCLIENT</c> (used by <see cref="Control.DrawToBitmap(Bitmap, Rectangle)"/>) is passed on and the border is
	///          drawn over the result.</remarks>
	internal static void _HandlePaintMessage(ref Message m, Control control, BorderStyle style, Color border, Color back, _WindowProcedure windowProcedure)
	{
		int width = _GetWidth(style);
		if((width <= 0) || (control.Width < 2) || (control.Height < 2))
		{
			windowProcedure(ref m);
			return;
		}

		if(m.Msg == _NativeMethods._WM_NCPAINT)
		{
			if((_NativeMethods._GetWindowLong(m.HWnd, _NativeMethods._GWL_STYLE) & (_NativeMethods._WS_VSCROLL | _NativeMethods._WS_HSCROLL)) != 0)
			{
				_DrawScrollBars(ref m, width, windowProcedure);
			}
			m.Result = IntPtr.Zero;
			_DrawWindow(control, width, border, back);
			return;
		}

		windowProcedure(ref m);
		if((m.Msg == _NativeMethods._WM_PRINT) && ((m.LParam.ToInt64() & _NativeMethods._PRF_NONCLIENT) != 0) && (m.WParam != IntPtr.Zero))
		{
			_Draw(m.WParam, control, width, border, back);
		}
	}


	/// <summary>Draws the border of a text box again, for example after its state has changed.</summary>
	/// <param name="control">Text box.</param>
	/// <param name="style">Border style.</param>
	/// <param name="border">Border color.</param>
	/// <param name="back">Back color.</param>
	internal static void _Redraw(Control control, BorderStyle style, Color border, Color back)
	{
		int width = _GetWidth(style);
		if(!control.IsHandleCreated || (width <= 0) || (control.Width < 2) || (control.Height < 2)) { return; }

		_DrawWindow(control, width, border, back);
	}


	/// <summary>Lets Windows draw the non-client area of a text box again, for example after the border is no longer drawn.</summary>
	/// <param name="control">Text box.</param>
	internal static void _RedrawNative(Control control)
	{
		if(!control.IsHandleCreated) { return; }

		_NativeMethods._SendMessage(control.Handle, _NativeMethods._WM_NCPAINT, (IntPtr) 1, IntPtr.Zero);
	}
}
