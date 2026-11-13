namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class draws the selected items of a list view or tree view in the selection colors of a theme.</summary>
/// <remarks><para>List views and tree views draw their selection with the visual style, which ignores the colors of the theme. The
///          class subclasses the control and changes the custom draw notifications that Windows Forms reflects to it: selected items
///          are drawn like unselected items, with the selection colors of the theme as text and background colors.</para>
///          <para>Because the visual style still draws the selection of list views and the text of tree views in its own colors, the
///          class draws selected items again after Windows has drawn them: the text of a tree node, and in list views in the details,
///          list and small icon views the selection background, the check box, the icon and the texts. Large icons and tiles keep the
///          selection of the visual style.</para></remarks>
[SupportedOSPlatform("windows")]
internal sealed class _SelectionWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by control.</summary>
	private static readonly ConditionalWeakTable<Control, _SelectionWindow> _Windows = new();

	/// <summary>Offset of the item state in a custom draw notification.</summary>
	private static readonly int _ItemStateOffset = (int) Marshal.OffsetOf<_NativeMethods._NMCUSTOMDRAW>(nameof(_NativeMethods._NMCUSTOMDRAW.uItemState));

	/// <summary>Offset of the text color in a list view or tree view custom draw notification.</summary>
	private static readonly int _TextOffset = (int) Marshal.OffsetOf<_NativeMethods._NMLVCUSTOMDRAW>(nameof(_NativeMethods._NMLVCUSTOMDRAW.clrText));

	/// <summary>Offset of the text background color in a list view or tree view custom draw notification.</summary>
	private static readonly int _TextBackOffset = (int) Marshal.OffsetOf<_NativeMethods._NMLVCUSTOMDRAW>(nameof(_NativeMethods._NMLVCUSTOMDRAW.clrTextBk));



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List view or tree view.</summary>
	private readonly Control _Control;

	/// <summary>Selection back color.</summary>
	private Color _Back;

	/// <summary>Selection text color.</summary>
	private Color _Text;

	/// <summary>Color table of the check boxes of list views.</summary>
	private readonly ThemedToggleColors _Toggles = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">List view or tree view.</param>
	private _SelectionWindow(Control control)
	{
		_Control = control;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a color to a COLORREF value.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the COLORREF value.</returns>
	private static int _ToColorRef(Color color)
	{
		return (color.R | (color.G << 8) | (color.B << 16));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the subclass.</summary>
	private void _Release()
	{
		if(Handle != IntPtr.Zero) { ReleaseHandle(); }
		_Windows.Remove(_Control);
		if(_Control.IsHandleCreated && !_Control.IsDisposed) { _Control.Invalidate(); }
	}


	/// <summary>Returns if the control draws itself.</summary>
	/// <returns>Returns <c>true</c> if the application draws the items, otherwise returns <c>false</c>.</returns>
	private bool _IsOwnerDrawn()
	{
		return ((_Control is ListView list) ? list.OwnerDraw : ((_Control as TreeView)?.DrawMode != TreeViewDrawMode.Normal));
	}


	/// <summary>Returns if the selection is visible.</summary>
	/// <returns>Returns <c>true</c> if the control shows its selection, otherwise returns <c>false</c>.</returns>
	private bool _ShowsSelection()
	{
		if(_Control.Focused) { return true; }
		return !((_Control as ListView)?.HideSelection ?? (_Control as TreeView)?.HideSelection ?? true);
	}


	/// <summary>Returns if an item is selected.</summary>
	/// <param name="item">Item: the index of a list view item or the handle of a tree node.</param>
	/// <returns>Returns <c>true</c> if the item is selected, otherwise returns <c>false</c>.</returns>
	private bool _IsSelected(UIntPtr item)
	{
		if(_Control is TreeView tree)
		{
			TreeNode? node = tree.SelectedNode;
			return ((node is not null) && (node.Handle == (IntPtr) (long) item.ToUInt64()));
		}

		return (_NativeMethods._SendMessage(Handle, _NativeMethods._LVM_GETITEMSTATE, (IntPtr) (long) item.ToUInt64(), (IntPtr) _NativeMethods._LVIS_SELECTED) != IntPtr.Zero);
	}


	/// <summary>Gets a rectangle of a list view item.</summary>
	/// <param name="index">Index of the item.</param>
	/// <param name="part">Part: <see cref="_NativeMethods._LVIR_BOUNDS"/>, <see cref="_NativeMethods._LVIR_ICON"/> or
	///                    <see cref="_NativeMethods._LVIR_LABEL"/>.</param>
	/// <returns>Returns the rectangle.</returns>
	private Rectangle _GetItemRect(int index, int part)
	{
		_NativeMethods._Rect rect = new() { Left = part };
		_NativeMethods._SendMessage(Handle, _NativeMethods._LVM_GETITEMRECT, (IntPtr) index, ref rect);

		return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
	}


	/// <summary>Gets the rectangle of a sub-item of a list view item.</summary>
	/// <param name="index">Index of the item.</param>
	/// <param name="subItem">Index of the sub-item, at least 1.</param>
	/// <returns>Returns the rectangle.</returns>
	private Rectangle _GetSubItemRect(int index, int subItem)
	{
		_NativeMethods._Rect rect = new() { Left = _NativeMethods._LVIR_BOUNDS, Top = subItem };
		_NativeMethods._SendMessage(Handle, _NativeMethods._LVM_GETSUBITEMRECT, (IntPtr) index, ref rect);

		return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
	}


	/// <summary>Draws a selected list view item again in the selection colors.</summary>
	/// <param name="list">List view.</param>
	/// <param name="g">Graphics.</param>
	/// <param name="index">Index of the item.</param>
	private void _PaintListItem(ListView list, Graphics g, int index)
	{
		if((list.View != View.Details) && (list.View != View.List) && (list.View != View.SmallIcon)) { return; }
		if((index < 0) || (index >= list.Items.Count)) { return; }

		ListViewItem item = list.Items[index];
		bool fullRow = ((list.View == View.Details) && list.FullRowSelect);
		Rectangle bounds = _GetItemRect(index, _NativeMethods._LVIR_BOUNDS);
		Rectangle icon = _GetItemRect(index, _NativeMethods._LVIR_ICON);
		Rectangle label = _GetItemRect(index, _NativeMethods._LVIR_LABEL);
		if(fullRow && list.GridLines) { bounds.Height -= 1; }

		Rectangle fill = (fullRow ? bounds : label);
		using(SolidBrush brush = new(_Back)) { g.FillRectangle(brush, fill); }

		if(fullRow)
		{
			if(list.CheckBoxes)
			{
				int size = _ControlRenderer._GetGlyphSize(list);
				int right = ((icon.Width > 0) ? icon.Left : label.Left);
				Rectangle glyph = new(bounds.Left + Math.Max(0, (right - bounds.Left - size) / 2), bounds.Top + ((bounds.Height - size) / 2), size, size);
				CheckState check = (item.Checked ? CheckState.Checked : CheckState.Unchecked);
				_ControlRenderer._GetGlyphColors(_Toggles, _Back, list.Enabled, check, _PaintStates.NONE, out Color boxBack, out Color boxBorder, out Color mark);
				_ControlRenderer._DrawGlyph(g, list, glyph, check, false, boxBack, boxBorder, mark);
			}

			ImageList? images = list.SmallImageList;
			int image = ((item.ImageIndex >= 0) ? item.ImageIndex : (((images is not null) && !string.IsNullOrEmpty(item.ImageKey)) ? images.Images.IndexOfKey(item.ImageKey) : -1));
			if((images is not null) && (image >= 0) && (image < images.Images.Count) && (icon.Width > 0))
			{
				Size size = images.ImageSize;
				images.Draw(g, icon.Left + ((icon.Width - size.Width) / 2), icon.Top + ((icon.Height - size.Height) / 2), image);
			}
		}

		TextFormatFlags flags = (TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
		int margin = list.LogicalToDeviceUnits(2);
		TextRenderer.DrawText(g, item.Text, item.Font, Rectangle.FromLTRB(label.Left + margin, label.Top, label.Right - margin, label.Bottom), _Text, flags);
		if(!fullRow) { return; }

		margin = list.LogicalToDeviceUnits(6);
		for(int i = 1; (i < list.Columns.Count) && (i < item.SubItems.Count); i++)
		{
			ListViewItem.ListViewSubItem sub = item.SubItems[i];
			Rectangle cell = _GetSubItemRect(index, i);
			cell = Rectangle.FromLTRB(cell.Left + margin, cell.Top, cell.Right - margin, cell.Bottom);
			if(cell.Width <= 0) { continue; }

			TextFormatFlags align = list.Columns[i].TextAlign switch
			{
				HorizontalAlignment.Right => TextFormatFlags.Right,
				HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
				_ => TextFormatFlags.Left
			};
			TextRenderer.DrawText(g, sub.Text, (item.UseItemStyleForSubItems ? item.Font : sub.Font), cell, _Text, flags | align);
		}

		if(list.Focused && item.Focused) { ControlPaint.DrawFocusRectangle(g, fill, _Text, _Back); }
	}


	/// <summary>Draws the text of the selected tree node again in the selection colors.</summary>
	/// <param name="tree">Tree view.</param>
	/// <param name="g">Graphics.</param>
	private void _PaintTreeNode(TreeView tree, Graphics g)
	{
		TreeNode? node = tree.SelectedNode;
		if(node is null) { return; }

		long item = node.Handle.ToInt64();
		_NativeMethods._Rect rect = new() { Left = unchecked((int) item), Top = (int) (item >> 32) };
		if(_NativeMethods._SendMessage(Handle, _NativeMethods._TVM_GETITEMRECT, (IntPtr) 1, ref rect) == IntPtr.Zero) { return; }

		Rectangle bounds = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
		using(SolidBrush brush = new(_Back)) { g.FillRectangle(brush, bounds); }
		TextRenderer.DrawText(g, node.Text, (node.NodeFont ?? tree.Font), bounds, _Text, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
	}


	/// <summary>Changes a custom draw notification after the control has handled it.</summary>
	/// <param name="m">Message.</param>
	private void _Change(ref Message m)
	{
		if((m.Result.ToInt64() & _NativeMethods._CDRF_SKIPDEFAULT) != 0) { return; }

		_NativeMethods._NMCUSTOMDRAW draw = Marshal.PtrToStructure<_NativeMethods._NMCUSTOMDRAW>(m.LParam);
		if(draw.dwDrawStage == _NativeMethods._CDDS_PREPAINT)
		{
			if((_Control is ListView) && (Marshal.PtrToStructure<_NativeMethods._NMLVCUSTOMDRAW>(m.LParam).dwItemType == _NativeMethods._LVCDI_GROUP)) { return; }
			m.Result = (IntPtr) (m.Result.ToInt64() | _NativeMethods._CDRF_NOTIFYITEMDRAW);
			return;
		}

		if(draw.dwDrawStage == _NativeMethods._CDDS_ITEMPOSTPAINT)
		{
			if(!_ShowsSelection() || !_IsSelected(draw.dwItemSpec)) { return; }

			using Graphics g = Graphics.FromHdc(draw.hdc);
			if(_Control is ListView list) { _PaintListItem(list, g, (int) draw.dwItemSpec.ToUInt64()); }
			else if(_Control is TreeView tree) { _PaintTreeNode(tree, g); }
			return;
		}

		if((draw.dwDrawStage != _NativeMethods._CDDS_ITEMPREPAINT) && (draw.dwDrawStage != _NativeMethods._CDDS_SUBITEMPREPAINT)) { return; }
		if(!_ShowsSelection() || !_IsSelected(draw.dwItemSpec)) { return; }

		Marshal.WriteInt32(m.LParam, _ItemStateOffset, (int) (draw.uItemState & ~(_NativeMethods._CDIS_SELECTED | _NativeMethods._CDIS_HOT)));
		Marshal.WriteInt32(m.LParam, _TextOffset, _ToColorRef(_Text));
		Marshal.WriteInt32(m.LParam, _TextBackOffset, _ToColorRef(_Back));
		long result = (m.Result.ToInt64() | _NativeMethods._CDRF_NEWFONT);
		if(draw.dwDrawStage == _NativeMethods._CDDS_ITEMPREPAINT) { result |= _NativeMethods._CDRF_NOTIFYPOSTPAINT; }
		m.Result = (IntPtr) result;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the selection of a list view or tree view in the colors of a theme until the control is restored.</summary>
	/// <param name="control">List view or tree view.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control.</param>
	/// <remarks>Nothing is subclassed if the handle has not been created yet; the theme manager applies the theme again when the handle
	///          is created.</remarks>
	internal static void _Install(Control control, Theme theme, ThemeAdapterState state)
	{
		if(!control.IsHandleCreated) { return; }

		_SelectionWindow window = _Windows.GetValue(control, m => new _SelectionWindow(m));
		window._Back = theme[ThemeColor.SELECTION_BACK];
		window._Text = theme[ThemeColor.SELECTION_TEXT];
		window._Toggles.Theme = theme;

		if(window.Handle != control.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(control.Handle);
		}
		state.OnRestore("selection", () => { if(_Windows.TryGetValue(control, out _SelectionWindow? current)) { current._Release(); } });
		control.Invalidate();
	}


	/// <summary>Gets the selection back color of a list view or tree view.</summary>
	/// <param name="control">List view or tree view.</param>
	/// <returns>Returns the color, or <see cref="Color.Empty"/> if the control draws its selection itself.</returns>
	internal static Color _GetBackColor(Control control)
	{
		return (_Windows.TryGetValue(control, out _SelectionWindow? window) ? window._Back : Color.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if((m.Msg != _NativeMethods._WM_REFLECT_NOTIFY) || (m.LParam == IntPtr.Zero) || _IsOwnerDrawn())
		{
			base.WndProc(ref m);
			return;
		}

		_NativeMethods._NMHDR hdr = Marshal.PtrToStructure<_NativeMethods._NMHDR>(m.LParam);
		base.WndProc(ref m);

		if((hdr.code != _NativeMethods._NM_CUSTOMDRAW) || (hdr.hwndFrom != Handle)) { return; }

		try { _Change(ref m); } catch(Exception) {}
	}
}
