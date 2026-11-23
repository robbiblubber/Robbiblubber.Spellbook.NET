namespace Robbiblubber.Spellbook;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;



/// <summary>This class provides reflection helpers: finding implementations of a type and reading or writing members by
///          name.</summary>
/// <remarks>Plug-ins from a directory are loaded with <see cref="PluginLoader"/>.</remarks>
public static class ClassOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Binding flags for declared instance members of any visibility.</summary>
	private const BindingFlags _MEMBER_FLAGS = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds a property or field in a type or its base types.</summary>
	/// <param name="t">Type.</param>
	/// <param name="name">Member name.</param>
	/// <returns>Returns the property or field; NULL if there is none.</returns>
	private static MemberInfo? _FindMember(Type t, string name)
	{
		for(Type? i = t; i is not null; i = i.BaseType)
		{
			PropertyInfo? prop = i.GetProperties(_MEMBER_FLAGS).FirstOrDefault(m => ((m.Name == name) && (m.GetIndexParameters().Length == 0)));
			if(prop is not null) { return prop; }

			FieldInfo? field = i.GetField(name, _MEMBER_FLAGS);
			if(field is not null) { return field; }
		}

		return null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the loadable types of an assembly.</summary>
	/// <param name="asm">Assembly.</param>
	/// <returns>Returns the types that could be loaded.</returns>
	internal static IEnumerable<Type> _GetTypes(Assembly asm)
	{
		try
		{
			return asm.GetTypes();
		}
		catch(ReflectionTypeLoadException ex) { return ex.Types.OfType<Type>(); }
	}


	/// <summary>Returns if a type is a public, concrete implementation of another type.</summary>
	/// <param name="baseType">Base type or interface.</param>
	/// <param name="t">Type.</param>
	/// <returns>Returns TRUE if the type can be instantiated and is assignable to the base type.</returns>
	internal static bool _IsImplementation(Type baseType, Type t)
	{
		return (t.IsClass && (!t.IsAbstract) && t.IsVisible && (!t.ContainsGenericParameters) && baseType.IsAssignableFrom(t));
	}


	/// <summary>Gets the public, concrete classes in a set of assemblies that implement or derive from a type.</summary>
	/// <param name="baseType">Base type or interface.</param>
	/// <param name="assemblies">Assemblies.</param>
	/// <returns>Returns the types.</returns>
	internal static Type[] _GetImplementations(Type baseType, IEnumerable<Assembly> assemblies)
	{
		return assemblies.Distinct().SelectMany(_GetTypes).Where(m => _IsImplementation(baseType, m)).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the public, concrete classes that implement or derive from a type.</summary>
	/// <typeparam name="T">Base type or interface.</typeparam>
	/// <param name="assemblies">Assemblies to search; if none are given, all assemblies of the application domain are
	///                          searched.</param>
	/// <returns>Returns the types. Abstract classes, interfaces and open generic types are not included.</returns>
	public static Type[] GetImplementations<T>(params Assembly[] assemblies)
	{
		return GetImplementations(typeof(T), assemblies);
	}


	/// <summary>Gets the public, concrete classes that implement or derive from a type.</summary>
	/// <param name="baseType">Base type or interface.</param>
	/// <param name="assemblies">Assemblies to search; if none are given, all assemblies of the application domain are
	///                          searched.</param>
	/// <returns>Returns the types. Abstract classes, interfaces and open generic types are not included.</returns>
	/// <remarks>Types that cannot be loaded (e.g. because of a missing dependency) are skipped.</remarks>
	public static Type[] GetImplementations(Type baseType, params Assembly[] assemblies)
	{
		ArgumentNullException.ThrowIfNull(baseType);
		if((assemblies is null) || (assemblies.Length == 0)) { assemblies = AppDomain.CurrentDomain.GetAssemblies(); }

		return _GetImplementations(baseType, assemblies);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets a property or field by name, regardless of its visibility.</summary>
	/// <typeparam name="T">Value type.</typeparam>
	/// <param name="obj">Object.</param>
	/// <param name="name">Property or field name; private members of base classes are found as well.</param>
	/// <param name="value">Value.</param>
	/// <param name="throwException">Determines if an exception is thrown when the member does not exist or cannot be
	///                              written. Default is TRUE.</param>
	/// <exception cref="InvalidOperationException">Thrown when the member does not exist or is read-only and
	///                                             <paramref name="throwException"/> is TRUE.</exception>
	/// <exception cref="ArgumentException">Thrown when the value does not match the member type.</exception>
	/// <remarks>Exceptions thrown by the property setter are passed on unchanged.</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void __Set<T>(this object obj, string name, T value, bool throwException = true)
	{
		ArgumentNullException.ThrowIfNull(obj);

		switch(_FindMember(obj.GetType(), name))
		{
			case PropertyInfo prop when prop.CanWrite:
				try
				{
					prop.SetValue(obj, value);
				}
				catch(TargetInvocationException ex) when(ex.InnerException is not null)
				{
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
				break;
			case PropertyInfo:
				if(throwException) { throw new InvalidOperationException("Property \"" + name + "\" is read-only."); }
				break;
			case FieldInfo field:
				field.SetValue(obj, value);
				break;
			default:
				if(throwException) { throw new InvalidOperationException("Property or field \"" + name + "\" does not exist."); }
				break;
		}
	}


	/// <summary>Gets a property or field by name, regardless of its visibility.</summary>
	/// <typeparam name="T">Value type.</typeparam>
	/// <param name="obj">Object.</param>
	/// <param name="name">Property or field name; private members of base classes are found as well.</param>
	/// <param name="throwException">Determines if an exception is thrown when the value cannot be retrieved. Default is
	///                              TRUE.</param>
	/// <param name="defaultValue">Value that is returned when the value cannot be retrieved and no exception is thrown.</param>
	/// <returns>Returns the value.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the member does not exist, the getter fails or the value is not
	///                                             a <typeparamref name="T"/>, and <paramref name="throwException"/> is
	///                                             TRUE.</exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static T? __Get<T>(this object obj, string name, bool throwException = true, T? defaultValue = default)
	{
		ArgumentNullException.ThrowIfNull(obj);

		MemberInfo? member = _FindMember(obj.GetType(), name);
		if((member is null) || ((member is PropertyInfo p) && (!p.CanRead)))
		{
			if(throwException) { throw new InvalidOperationException("Property or field \"" + name + "\" does not exist or cannot be read."); }
			return defaultValue;
		}

		try
		{
			object? rval = ((member is PropertyInfo prop) ? prop.GetValue(obj) : ((FieldInfo) member).GetValue(obj));
			return (T?) rval;
		}
		catch(Exception ex)
		{
			if(throwException) { throw new InvalidOperationException("Could not retrieve value of \"" + name + "\".", (ex as TargetInvocationException)?.InnerException ?? ex); }
		}

		return defaultValue;
	}
}
