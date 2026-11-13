namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to list views.</summary>
[ThemeAdapter(typeof(ListView))]
[SupportedOSPlatform("windows")]
internal sealed class _ListViewAdapter: ThemeAdapter<ListView>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ListView>                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The list view gets the field colors; in dark themes, the scroll bars and the column header are dark, and the header
	///          text gets the header text color of the theme. Group headers are drawn in the accent color of the theme, selected items in the
	///          selection colors.</remarks>
	protected override void _Apply(ListView control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT);
		_ApplyWindowTheme(control, state);

		if(!control.IsHandleCreated) { return; }

		_GroupHeaderWindow._Set(control, theme[ThemeColor.ACCENT], theme[ThemeColor.SEPARATOR]);
		_SelectionWindow._Install(control, theme, state);
		state.OnRestore("groups", () => _GroupHeaderWindow._Reset(control));

		IntPtr header = _NativeMethods._SendMessage(control.Handle, _NativeMethods._LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
		if(header == IntPtr.Zero) { return; }

		_NativeMethods._SetWindowTheme(header, (theme.IsDark ? "DarkMode_ItemsView" : null), null);
		if(theme.IsDark) { _HeaderTextWindow._Set(control, theme[ThemeColor.HEADER_TEXT]); } else { _HeaderTextWindow._Reset(control); }

		state.OnRestore("header", () =>
		{
			_HeaderTextWindow._Reset(control);
			if(!control.IsHandleCreated || control.IsDisposed) { return; }

			IntPtr h = _NativeMethods._SendMessage(control.Handle, _NativeMethods._LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
			if(h != IntPtr.Zero) { _NativeMethods._SetWindowTheme(h, ((_GetDefaultWindowTheme() is null) ? null : "DarkMode_ItemsView"), null); }
		});
	}
}
