namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class draws the selection of the text of an editable combo box in the selection colors of a theme.</summary>
/// <remarks>The class subclasses the native edit control of the combo box and handles its messages with
///          <see cref="_SysColorHook"/>; it is only installed with <see cref="ThemeManager.ThemeTextSelection"/>.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ComboEditWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by combo box.</summary>
	private static readonly ConditionalWeakTable<ComboBox, _ComboEditWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Theme.</summary>
	private Theme _Theme = Theme.SYSTEM;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes the subclass of a combo box.</summary>
	/// <param name="comboBox">Combo box.</param>
	private static void _Release(ComboBox comboBox)
	{
		if(!_Windows.TryGetValue(comboBox, out _ComboEditWindow? window)) { return; }

		if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
		_Windows.Remove(comboBox);
		if(comboBox.IsHandleCreated && !comboBox.IsDisposed) { comboBox.Invalidate(true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the selection of the text of an editable combo box in the colors of a theme until the combo box is restored, or
	///          removes the subclass if <see cref="ThemeManager.ThemeTextSelection"/> is off.</summary>
	/// <param name="comboBox">Combo box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the combo box.</param>
	internal static void _Update(ComboBox comboBox, Theme theme, ThemeAdapterState state)
	{
		IntPtr edit = (comboBox.IsHandleCreated ? _NativeMethods._FindWindowEx(comboBox.Handle, IntPtr.Zero, "Edit", null) : IntPtr.Zero);
		if(!ThemeManager.ThemeTextSelection || (edit == IntPtr.Zero))
		{
			_Release(comboBox);
			return;
		}

		_ComboEditWindow window = _Windows.GetValue(comboBox, m => new _ComboEditWindow());
		window._Theme = theme;
		if(window.Handle != edit)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(edit);
		}
		state.OnRestore("text-selection", () => _Release(comboBox));
		comboBox.Invalidate(true);
	}


	/// <summary>Returns if the edit control of a combo box is subclassed.</summary>
	/// <param name="comboBox">Combo box.</param>
	/// <returns>Returns <c>true</c> if the edit control is subclassed, otherwise returns <c>false</c>.</returns>
	internal static bool _IsInstalled(ComboBox comboBox)
	{
		return _Windows.TryGetValue(comboBox, out _);
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
