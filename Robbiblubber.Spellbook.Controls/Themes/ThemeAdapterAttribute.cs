namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This attribute marks a theme adapter for a control type.</summary>
/// <remarks>The class has to implement <see cref="IThemeAdapter"/> and to have a public parameterless constructor.
///          <see cref="ThemeManager.Discover(System.Reflection.Assembly)"/> registers all classes of an assembly with this attribute. A
///          class can be marked several times for several control types.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
[SupportedOSPlatform("windows")]
public sealed class ThemeAdapterAttribute: Attribute
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="controlType">Control type.</param>
	public ThemeAdapterAttribute(Type controlType)
	{
		ControlType = controlType;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control type.</summary>
	/// <remarks>The adapter is used for this type and all derived types that do not have an adapter of their own.</remarks>
	public Type ControlType
	{
		get;
	}
}
