namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>Theme adapters apply a theme to controls of a certain type.</summary>
/// <remarks><para>An adapter is registered for a control type with <see cref="ThemeAdapterAttribute"/> and
///          <see cref="ThemeManager.Discover(System.Reflection.Assembly)"/>, or with
///          <see cref="ThemeManager.RegisterAdapter(Type, IThemeAdapter)"/>. The adapter of the most derived registered type is used.</para>
///          <para>Adapters change controls only through the <see cref="ThemeAdapterState"/>, which remembers the original values. For
///          themes that use the system colors (<see cref="Theme.IsSystem"/>), the theme manager restores the original values instead of
///          calling the adapter.</para>
///          <para>Derive from <see cref="ThemeAdapter{T}"/> rather than implementing this interface directly.</para></remarks>
[SupportedOSPlatform("windows")]
public interface IThemeAdapter
{
	/// <summary>Applies a theme to a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control that remembers the original values.</param>
	/// <remarks>The method is called again when the theme or its colors change and when the handle of the control has been created
	///          again, so it has to set all values every time.</remarks>
	void Apply(Control control, Theme theme, ThemeAdapterState state);


	/// <summary>Returns if the theme manager applies the theme to the child controls of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the child controls are themed, <c>false</c> if the adapter takes care of them.</returns>
	bool IncludesChildren(Control control)
	{
		return true;
	}
}
