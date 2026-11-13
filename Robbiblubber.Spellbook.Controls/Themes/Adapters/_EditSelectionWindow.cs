namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class draws the selection of the edit control inside a control, like an editable combo box or an up-down control, in
///          the selection colors of a theme.</summary>
/// <remarks>The class subclasses the edit control and handles its messages with <see cref="_SysColorHook"/>; it is only installed with
///          <see cref="ThemeManager.ThemeTextSelection"/>.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _EditSelectionWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by control.</summary>
	private static readonly ConditionalWeakTable<Control, _EditSelectionWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme.</summary>
	private Theme _Theme = Theme.SYSTEM;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the subclass of the edit control of a control.</summary>
	/// <param name="control">Control.</param>
	private static void _Release(Control control)
	{
		if(!_Windows.TryGetValue(control, out _EditSelectionWindow? window)) { return; }

		if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
		_Windows.Remove(control);
		if(control.IsHandleCreated && !control.IsDisposed) { control.Invalidate(true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the selection of the edit control of a control in the colors of a theme until the control is restored, or
	///          removes the subclass if <see cref="ThemeManager.ThemeTextSelection"/> is off.</summary>
	/// <param name="control">Control.</param>
	/// <param name="edit">Window handle of the edit control, <see cref="IntPtr.Zero"/> if there is none.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control.</param>
	internal static void _Update(Control control, IntPtr edit, Theme theme, ThemeAdapterState state)
	{
		if(!ThemeManager.ThemeTextSelection || (edit == IntPtr.Zero))
		{
			_Release(control);
			return;
		}

		_EditSelectionWindow window = _Windows.GetValue(control, m => new _EditSelectionWindow());
		window._Theme = theme;
		if(window.Handle != edit)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(edit);
		}
		state.OnRestore("text-selection", () => _Release(control));
		control.Invalidate(true);
	}


	/// <summary>Returns if the edit control of a control is subclassed.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the edit control is subclassed, otherwise returns <c>false</c>.</returns>
	internal static bool _IsInstalled(Control control)
	{
		return _Windows.TryGetValue(control, out _);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		_SysColorHook._Run(ref m, Handle, _Theme, base.WndProc);
	}
}
