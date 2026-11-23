namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This attribute marks an edit adapter for <see cref="EditAdapters.Discover(IEnumerable{Type})"/>.</summary>
/// <remarks><para>Only classes with this attribute are registered by discovery. The control type is taken from the attribute or, if
///          it is not specified, from the type argument of <see cref="EditAdapter{T}"/>. The attribute can be applied several times to
///          register one adapter for several control types.</para>
///          <para>Discovered adapters need a public parameterless constructor.</para></remarks>
/// <example><code>
/// [EditAdapter]
/// public class MyGridEditAdapter: EditAdapter&lt;MyGrid&gt;
/// { ... }
///
/// EditAdapters.Discover(ClassOp.GetSubtypesOf&lt;IEditAdapter&gt;());
/// </code></example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
[SupportedOSPlatform("windows")]
public sealed class EditAdapterAttribute: Attribute
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <remarks>The control type is taken from the type argument of <see cref="EditAdapter{T}"/>.</remarks>
	public EditAdapterAttribute(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="controlType">Control type the adapter is registered for.</param>
	public EditAdapterAttribute(Type controlType): base()
	{
		ControlType = controlType;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control type the adapter is registered for, NULL for the type argument of <see cref="EditAdapter{T}"/>.</summary>
	public Type? ControlType
	{
		get;
	}
}
