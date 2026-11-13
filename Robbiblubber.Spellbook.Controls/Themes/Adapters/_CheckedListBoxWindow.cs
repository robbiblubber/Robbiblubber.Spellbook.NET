namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class draws the items of a checked list box in the colors of a theme.</summary>
/// <remarks>A checked list box draws its items itself and always uses the system selection color, so its drawing mode cannot be
///          changed like the one of a list box. The class subclasses the checked list box and draws the items in the owner draw message
///          that Windows Forms reflects to it: the check box like a themed check box, selected items in the selection colors.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _CheckedListBoxWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by checked list box.</summary>
	private static readonly ConditionalWeakTable<CheckedListBox, _CheckedListBoxWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Checked list box.</summary>
	private readonly CheckedListBox _ListBox;

	/// <summary>Color table of the check boxes.</summary>
	private readonly ThemedToggleColors _Colors = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="listBox">Checked list box.</param>
	private _CheckedListBoxWindow(CheckedListBox listBox)
	{
		_ListBox = listBox;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the subclass.</summary>
	private void _Release()
	{
		if(Handle != IntPtr.Zero) { ReleaseHandle(); }
		_Windows.Remove(_ListBox);
		if(_ListBox.IsHandleCreated && !_ListBox.IsDisposed) { _ListBox.Invalidate(); }
	}


	/// <summary>Draws an item.</summary>
	/// <param name="draw">Owner draw information.</param>
	private void _Draw(_NativeMethods._DRAWITEMSTRUCT draw)
	{
		Theme theme = _Colors.EffectiveTheme;
		Rectangle bounds = Rectangle.FromLTRB(draw.rcItem.Left, draw.rcItem.Top, draw.rcItem.Right, draw.rcItem.Bottom);

		using Graphics g = Graphics.FromHdc(draw.hDC);
		using(SolidBrush brush = new(_ListBox.BackColor)) { g.FillRectangle(brush, bounds); }
		if((draw.itemID < 0) || (draw.itemID >= _ListBox.Items.Count)) { return; }

		DrawItemState state = DrawItemState.None;
		if((draw.itemState & _NativeMethods._ODS_SELECTED) != 0) { state |= DrawItemState.Selected; }
		if((draw.itemState & _NativeMethods._ODS_DISABLED) != 0) { state |= DrawItemState.Disabled; }
		if((draw.itemState & _NativeMethods._ODS_FOCUS) != 0) { state |= DrawItemState.Focus; }
		if((draw.itemState & _NativeMethods._ODS_NOFOCUSRECT) != 0) { state |= DrawItemState.NoFocusRect; }
		if(_ListBox.SelectionMode == SelectionMode.None) { state &= ~DrawItemState.Selected; }

		int size = _ControlRenderer._GetGlyphSize(_ListBox);
		int gap = _ListBox.LogicalToDeviceUnits(2);
		Rectangle glyph = new(bounds.X + gap, bounds.Y + ((bounds.Height - size) / 2), size, size);
		Rectangle text = Rectangle.FromLTRB(glyph.Right + gap, bounds.Top, bounds.Right, bounds.Bottom);

		CheckState check = _ListBox.GetItemCheckState(draw.itemID);
		bool enabled = (_ListBox.Enabled && !state.HasFlag(DrawItemState.Disabled));
		_ControlRenderer._GetGlyphColors(_Colors, _ListBox.BackColor, enabled, check, _PaintStates.NONE, out Color boxBack, out Color boxBorder, out Color mark);
		_ControlRenderer._DrawGlyph(g, _ListBox, glyph, check, false, boxBack, boxBorder, mark);

		_ControlRenderer._PaintListItem(g, _ListBox, theme, text, text, _ListBox.GetItemText(_ListBox.Items[draw.itemID]), state, _ListBox.UseTabStops);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the items of a checked list box in the colors of a theme until the checked list box is restored.</summary>
	/// <param name="listBox">Checked list box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the checked list box.</param>
	/// <remarks>Nothing is subclassed if the handle has not been created yet; the theme manager applies the theme again when the handle
	///          is created.</remarks>
	internal static void _Install(CheckedListBox listBox, Theme theme, ThemeAdapterState state)
	{
		if(!listBox.IsHandleCreated) { return; }

		_CheckedListBoxWindow window = _Windows.GetValue(listBox, m => new _CheckedListBoxWindow(m));
		window._Colors.Theme = theme;

		if(window.Handle != listBox.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(listBox.Handle);
		}
		state.OnRestore("checked-items", () => { if(_Windows.TryGetValue(listBox, out _CheckedListBoxWindow? current)) { current._Release(); } });
		listBox.Invalidate();
	}


	/// <summary>Returns if the items of a checked list box are drawn in theme colors.</summary>
	/// <param name="listBox">Checked list box.</param>
	/// <returns>Returns <c>true</c> if the items are drawn by this class, otherwise returns <c>false</c>.</returns>
	internal static bool _IsInstalled(CheckedListBox listBox)
	{
		return _Windows.TryGetValue(listBox, out _);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if((m.Msg == _NativeMethods._WM_REFLECT_DRAWITEM) && (m.LParam != IntPtr.Zero) && (_ListBox.RightToLeft != RightToLeft.Yes))
		{
			try
			{
				_Draw(Marshal.PtrToStructure<_NativeMethods._DRAWITEMSTRUCT>(m.LParam));
				m.Result = (IntPtr) 1;
				return;
			}
			catch(Exception) {}
		}

		base.WndProc(ref m);
	}
}
