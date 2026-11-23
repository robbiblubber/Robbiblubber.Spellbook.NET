namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class draws the group headers of a list view in theme colors.</summary>
/// <remarks>The visual style draws group headers in its own blue, which is hardly readable on dark backgrounds, and the header color
///          of a group cannot be set while visual styles are enabled. The class subclasses the list view and draws the headers of simple
///          groups itself in the custom draw notifications of the list view. Groups with a subtitle, a footer, a task link, a title
///          image or a collapse button are left to the visual style.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _GroupHeaderWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by list view.</summary>
	private static readonly ConditionalWeakTable<ListView, _GroupHeaderWindow> _Windows = new();

	/// <summary>Native group ID of a list view group.</summary>
	private static readonly PropertyInfo? _GroupID = typeof(ListViewGroup).GetProperty("ID", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Default group of a list view.</summary>
	private static readonly PropertyInfo? _DefaultGroup = typeof(ListView).GetProperty("DefaultGroup", BindingFlags.Instance | BindingFlags.NonPublic);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List view.</summary>
	private readonly ListView _ListView;

	/// <summary>Header text color.</summary>
	private Color _Color;

	/// <summary>Separator line color.</summary>
	private Color _LineColor;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="listView">List view.</param>
	private _GroupHeaderWindow(ListView listView)
	{
		_ListView = listView;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the group header colors of a list view.</summary>
	/// <param name="listView">List view; its handle must have been created.</param>
	/// <param name="color">Header text color.</param>
	/// <param name="lineColor">Separator line color.</param>
	internal static void _Set(ListView listView, Color color, Color lineColor)
	{
		_GroupHeaderWindow window = _Windows.GetValue(listView, m => new _GroupHeaderWindow(m));
		window._Color = color;
		window._LineColor = lineColor;

		if(window.Handle != listView.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(listView.Handle);
		}

		listView.Invalidate();
	}


	/// <summary>Restores the default group headers of a list view.</summary>
	/// <param name="listView">List view.</param>
	internal static void _Reset(ListView listView)
	{
		if(!_Windows.TryGetValue(listView, out _GroupHeaderWindow? window)) { return; }

		if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
		_Windows.Remove(listView);

		if(listView.IsHandleCreated && !listView.IsDisposed) { listView.Invalidate(); }
	}


	/// <summary>Gets the group header text color of a list view.</summary>
	/// <param name="listView">List view.</param>
	/// <returns>Returns the header text color, or <see cref="Color.Empty"/> if the list view draws the default group headers.</returns>
	internal static Color _GetColor(ListView listView)
	{
		return (_Windows.TryGetValue(listView, out _GroupHeaderWindow? window) ? window._Color : Color.Empty);
	}


	/// <summary>Returns if the header of a group can be drawn by this class.</summary>
	/// <param name="group">Group.</param>
	/// <returns>Returns <c>true</c> if the group has only a header, otherwise returns <c>false</c>.</returns>
	internal static bool _IsSimple(ListViewGroup group)
	{
		return (string.IsNullOrEmpty(group.Subtitle) && string.IsNullOrEmpty(group.Footer) && string.IsNullOrEmpty(group.TaskLink) &&
		        (group.CollapsedState == ListViewGroupCollapsedState.Default) && (group.TitleImageIndex < 0) && string.IsNullOrEmpty(group.TitleImageKey));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a group of the list view by its native ID.</summary>
	/// <param name="id">Native group ID.</param>
	/// <returns>Returns the group, or <c>NULL</c> if no group has been found.</returns>
	private ListViewGroup? _GetGroup(int id)
	{
		if(_GroupID is null) { return null; }

		foreach(ListViewGroup i in _ListView.Groups)
		{
			if(_GroupID.GetValue(i) is int rval && (rval == id)) { return i; }
		}

		if(_DefaultGroup?.GetValue(_ListView) is ListViewGroup def && _GroupID.GetValue(def) is int defID && (defID == id)) { return def; }
		return null;
	}


	/// <summary>Draws a group header.</summary>
	/// <param name="draw">Custom draw notification.</param>
	/// <returns>Returns <c>true</c> if the header has been drawn, <c>false</c> if the list view draws it.</returns>
	private bool _Draw(_NativeMethods._NMLVCUSTOMDRAW draw)
	{
		int id = (int) draw.nmcd.dwItemSpec;

		ListViewGroup? group = _GetGroup(id);
		if((group is null) || !_IsSimple(group)) { return false; }
		if(_ListView.RightToLeft == RightToLeft.Yes) { return false; }

		int[] rect = new int[] { 0, _NativeMethods._LVGGR_HEADER, 0, 0 };
		if(_NativeMethods._SendMessage(Handle, _NativeMethods._LVM_GETGROUPRECT, (IntPtr) id, rect) == IntPtr.Zero) { return false; }

		Rectangle bounds = Rectangle.FromLTRB(rect[0], rect[1], rect[2], rect[3]);
		if((bounds.Width <= 0) || (bounds.Height <= 0)) { return false; }

		using(Graphics g = Graphics.FromHdc(draw.nmcd.hdc))
		{
			using(SolidBrush back = new(_ListView.BackColor)) { g.FillRectangle(back, bounds); }

			int pad = _ListView.LogicalToDeviceUnits(10);
			Rectangle area = Rectangle.FromLTRB(bounds.Left + pad, bounds.Top, bounds.Right - pad, bounds.Bottom);
			if(area.Width <= 0) { return true; }

			TextFormatFlags flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
			flags |= group.HeaderAlignment switch
			{
				HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
				HorizontalAlignment.Right => TextFormatFlags.Right,
				_ => TextFormatFlags.Left
			};

			using Font font = new(_ListView.Font, FontStyle.Bold);
			string text = (group.Header ?? string.Empty);
			int width = Math.Min(TextRenderer.MeasureText(g, text, font, area.Size, flags).Width, area.Width);

			TextRenderer.DrawText(g, text, font, area, _Color, flags);

			int left = area.Left, right = area.Right;
			switch(group.HeaderAlignment)
			{
				case HorizontalAlignment.Center: left = area.Left + ((area.Width - width) / 2); right = left + width; break;
				case HorizontalAlignment.Right:  left = area.Right - width; break;
				default:                         right = area.Left + width; break;
			}

			int y = bounds.Top + (bounds.Height / 2);
			int gap = (pad / 2);
			using Pen pen = new(_LineColor);

			if((group.HeaderAlignment != HorizontalAlignment.Left) && (left - gap > area.Left)) { g.DrawLine(pen, area.Left, y, left - gap, y); }
			if((group.HeaderAlignment != HorizontalAlignment.Right) && (right + gap < area.Right)) { g.DrawLine(pen, right + gap, y, area.Right, y); }
		}

		return true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if((m.Msg == _NativeMethods._WM_REFLECT_NOTIFY) && (m.LParam != IntPtr.Zero) && !_ListView.OwnerDraw)
		{
			_NativeMethods._NMHDR hdr = Marshal.PtrToStructure<_NativeMethods._NMHDR>(m.LParam);

			if((hdr.code == _NativeMethods._NM_CUSTOMDRAW) && (hdr.hwndFrom == Handle))
			{
				_NativeMethods._NMLVCUSTOMDRAW draw = Marshal.PtrToStructure<_NativeMethods._NMLVCUSTOMDRAW>(m.LParam);

				if((draw.nmcd.dwDrawStage == _NativeMethods._CDDS_PREPAINT) && (draw.dwItemType == _NativeMethods._LVCDI_GROUP))
				{
					try
					{
						if(_Draw(draw))
						{
							m.Result = _NativeMethods._CDRF_SKIPDEFAULT;
							return;
						}
					}
					catch(Exception) {}
				}
			}
		}

		base.WndProc(ref m);
	}
}
