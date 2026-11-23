namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class sets the text color of the column header of a list view.</summary>
/// <remarks>The dark visual style of the header has a dark background but keeps the dark text. The class subclasses the list view and
///          sets the text color in the custom draw notifications of the header.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _HeaderTextWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by list view.</summary>
	private static readonly ConditionalWeakTable<ListView, _HeaderTextWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>List view.</summary>
	private readonly ListView _ListView;

	/// <summary>Text color as COLORREF.</summary>
	private int _Color;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="listView">List view.</param>
	private _HeaderTextWindow(ListView listView)
	{
		_ListView = listView;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the header text color of a list view.</summary>
	/// <param name="listView">List view; its handle must have been created.</param>
	/// <param name="color">Text color.</param>
	internal static void _Set(ListView listView, Color color)
	{
		_HeaderTextWindow window = _Windows.GetValue(listView, m => new _HeaderTextWindow(m));
		window._Color = (color.R | (color.G << 8) | (color.B << 16));

		if(window.Handle != listView.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(listView.Handle);
		}

		IntPtr header = _NativeMethods._SendMessage(listView.Handle, _NativeMethods._LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
		if(header != IntPtr.Zero) { _NativeMethods._InvalidateRect(header, IntPtr.Zero, true); }
	}


	/// <summary>Restores the header text color of a list view.</summary>
	/// <param name="listView">List view.</param>
	internal static void _Reset(ListView listView)
	{
		if(!_Windows.TryGetValue(listView, out _HeaderTextWindow? window)) { return; }

		if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
		_Windows.Remove(listView);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if((m.Msg == _NativeMethods._WM_NOTIFY) && (m.LParam != IntPtr.Zero) && !_ListView.OwnerDraw)
		{
			_NativeMethods._NMHDR hdr = Marshal.PtrToStructure<_NativeMethods._NMHDR>(m.LParam);

			if(hdr.code == _NativeMethods._NM_CUSTOMDRAW)
			{
				IntPtr header = _NativeMethods._SendMessage(Handle, _NativeMethods._LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

				if(hdr.hwndFrom == header)
				{
					_NativeMethods._NMCUSTOMDRAW draw = Marshal.PtrToStructure<_NativeMethods._NMCUSTOMDRAW>(m.LParam);

					if(draw.dwDrawStage == _NativeMethods._CDDS_PREPAINT)
					{
						m.Result = _NativeMethods._CDRF_NOTIFYITEMDRAW;
						return;
					}
					if(draw.dwDrawStage == _NativeMethods._CDDS_ITEMPREPAINT)
					{
						_NativeMethods._SetTextColor(draw.hdc, _Color);
						m.Result = _NativeMethods._CDRF_DEFAULT;
						return;
					}
				}
			}
		}

		base.WndProc(ref m);
	}
}
