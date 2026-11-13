namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class provides a base for theme adapters of a control type.</summary>
/// <typeparam name="T">Control type.</typeparam>
/// <remarks>A derived class implements <see cref="_Apply(T, Theme, ThemeAdapterState)"/> and is registered with
///          <see cref="ThemeAdapterAttribute"/>:
///          <code>[ThemeAdapter(typeof(MyControl))]
///public class MyControlAdapter: ThemeAdapter&lt;MyControl&gt;
///{
///    protected override void _Apply(MyControl control, Theme theme, ThemeAdapterState state)
///    {
///        _ApplyColors(control, state, ThemeColor.FIELD_BACK, ThemeColor.FIELD_TEXT);
///        state.ApplyColor(control, nameof(MyControl.MarkerColor), ThemeColor.ACCENT);
///    }
///}</code></remarks>
[SupportedOSPlatform("windows")]
public abstract class ThemeAdapter<T>: IThemeAdapter where T: Control
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected static methods                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies the theme to the back and fore color of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="state">State.</param>
	/// <param name="back">Role of the back color, NULL to keep the inherited back color.</param>
	/// <param name="fore">Role of the fore color, NULL to keep the inherited fore color.</param>
	protected static void _ApplyColors(Control control, ThemeAdapterState state, ThemeColor? back, ThemeColor? fore)
	{
		state.ApplyColor(control, nameof(Control.BackColor), back);
		state.ApplyColor(control, nameof(Control.ForeColor), fore);
	}


	/// <summary>Sets the visual style of the native window, for example to get dark scroll bars.</summary>
	/// <param name="control">Control.</param>
	/// <param name="state">State.</param>
	/// <param name="dark">Visual style for dark themes, for example <c>DarkMode_Explorer</c>.</param>
	/// <param name="light">Visual style for light themes, NULL for the default style.</param>
	/// <remarks>Nothing happens if the handle of the control has not been created yet; the theme manager applies the theme again when
	///          the handle is created.</remarks>
	protected static void _ApplyWindowTheme(Control control, ThemeAdapterState state, string dark = "DarkMode_Explorer", string? light = null)
	{
		if(!control.IsHandleCreated) { return; }

		_NativeMethods._SetWindowTheme(control.Handle, ((state.Theme?.IsDark ?? false) ? dark : light), null);
		state.OnRestore("window-theme", () => { if(control.IsHandleCreated && !control.IsDisposed) { _NativeMethods._SetWindowTheme(control.Handle, _GetDefaultWindowTheme(), null); } });
	}


	/// <summary>Gets the visual style that WinForms uses for native windows.</summary>
	/// <returns>Returns <c>DarkMode_Explorer</c> if the application runs in dark mode, otherwise NULL.</returns>
	protected static string? _GetDefaultWindowTheme()
	{
#if NET9_0_OR_GREATER
		if(Application.IsDarkModeEnabled) { return "DarkMode_Explorer"; }
#endif
		return null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies a theme to a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control that remembers the original values.</param>
	/// <remarks>The method is called again when the theme or its colors change and when the handle of the control has been created
	///          again, so it has to set all values every time.</remarks>
	protected abstract void _Apply(T control, Theme theme, ThemeAdapterState state);


	/// <summary>Returns if the theme manager applies the theme to the child controls of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the child controls are themed, <c>false</c> if the adapter takes care of them.</returns>
	protected virtual bool _IncludesChildren(T control)
	{
		return true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IThemeAdapter                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies a theme to a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control that remembers the original values.</param>
	public void Apply(Control control, Theme theme, ThemeAdapterState state)
	{
		_Apply((T) control, theme, state);
	}


	/// <summary>Returns if the theme manager applies the theme to the child controls of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the child controls are themed, <c>false</c> if the adapter takes care of them.</returns>
	public bool IncludesChildren(Control control)
	{
		return _IncludesChildren((T) control);
	}
}
