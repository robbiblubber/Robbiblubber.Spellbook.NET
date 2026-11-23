namespace Robbiblubber.Spellbook.Controls;

using System.Reflection;
using System.Runtime.Versioning;



/// <summary>This class manages the edit adapters for control types.</summary>
/// <remarks><para>The adapter of a control is the adapter registered for its type or, if there is none, for its nearest base type. By default,
///          adapters are registered for <see cref="TextBoxBase"/>, <see cref="ComboBox"/>, <see cref="DataGridView"/>,
///          <see cref="ListView"/> and <see cref="ListBox"/>. Registrations apply to the whole application.</para>
///          <para>Adapters can be registered explicitly (<see cref="Register(Type, IEditAdapter)"/>) or discovered among a set of
///          types (<see cref="Discover(IEnumerable{Type})"/>), for example the types found by <c>ClassOp.GetSubtypesOf</c>.</para></remarks>
[SupportedOSPlatform("windows")]
public static class EditAdapters
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adapters by control type.</summary>
	private static readonly Dictionary<Type, IEditAdapter> _Adapters = new();

	/// <summary>Lock object.</summary>
	private static readonly object _Lock = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Initializes the class.</summary>
	static EditAdapters()
	{
		Reset();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the type argument of <see cref="EditAdapter{T}"/> for an adapter type.</summary>
	/// <param name="adapterType">Adapter type.</param>
	/// <returns>Returns the control type, or NULL if the type is not derived from <see cref="EditAdapter{T}"/>.</returns>
	internal static Type? _GetGenericControlType(Type adapterType)
	{
		for(Type? i = adapterType; i is not null; i = i.BaseType)
		{
			if(i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(EditAdapter<>))) { return i.GetGenericArguments()[0]; }
		}

		return null;
	}


	/// <summary>Gets the control types a type is a discoverable adapter for.</summary>
	/// <param name="type">Type.</param>
	/// <returns>Returns the control types; an empty list if the type is not a discoverable adapter.</returns>
	/// <remarks>A discoverable adapter is a concrete class that implements <see cref="IEditAdapter"/>, has a public parameterless
	///          constructor and at least one <see cref="EditAdapterAttribute"/>.</remarks>
	internal static List<Type> _GetDiscoverableControlTypes(Type type)
	{
		List<Type> rval = new();

		if((!type.IsClass) || type.IsAbstract || type.ContainsGenericParameters || (!typeof(IEditAdapter).IsAssignableFrom(type))) { return rval; }
		if(type.GetConstructor(Type.EmptyTypes) is null) { return rval; }

		foreach(EditAdapterAttribute i in type.GetCustomAttributes<EditAdapterAttribute>(false))
		{
			Type? controlType = (i.ControlType ?? _GetGenericControlType(type));
			if((controlType is not null) && typeof(Control).IsAssignableFrom(controlType) && (!rval.Contains(controlType))) { rval.Add(controlType); }
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Registers the adapters among a set of types.</summary>
	/// <param name="types">Types, for example all types of an assembly or the result of <c>ClassOp.GetSubtypesOf&lt;IEditAdapter&gt;()</c>.</param>
	/// <returns>Returns the number of registrations (an adapter for several control types counts several times).</returns>
	/// <remarks>Only types marked with <see cref="EditAdapterAttribute"/> are registered; other types, abstract classes and classes
	///          without a public parameterless constructor are skipped, as are adapters whose constructor throws an exception.
	///          Discovered adapters replace adapters registered before for the same control type, including the default
	///          adapters.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the types are NULL.</exception>
	public static int Discover(IEnumerable<Type?> types)
	{
		int rval = 0;

		if(types is null) { throw new ArgumentNullException(nameof(types)); }

		foreach(Type? i in types.Distinct())
		{
			List<Type> controlTypes = ((i is null) ? new List<Type>() : _GetDiscoverableControlTypes(i));
			IEditAdapter adapter;

			if(controlTypes.Count == 0) { continue; }

			try
			{
				adapter = (IEditAdapter) Activator.CreateInstance(i!)!;
			}
			catch(Exception ex) when ((ex is TargetInvocationException) || (ex is MemberAccessException) || (ex is TypeLoadException)) { continue; }

			foreach(Type j in controlTypes)
			{
				Register(j, adapter);
				rval++;
			}
		}

		return rval;
	}


	/// <summary>Registers the adapters of an assembly.</summary>
	/// <param name="assembly">Assembly.</param>
	/// <returns>Returns the number of registrations.</returns>
	/// <remarks>See <see cref="Discover(IEnumerable{Type})"/>. Types that cannot be loaded are skipped.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the assembly is NULL.</exception>
	public static int Discover(Assembly assembly)
	{
		Type?[] types;

		if(assembly is null) { throw new ArgumentNullException(nameof(assembly)); }

		try
		{
			types = assembly.GetTypes();
		}
		catch(ReflectionTypeLoadException ex) { types = ex.Types; }

		return Discover(types);
	}



	/// <summary>Registers an adapter for a control type, replacing an adapter registered before.</summary>
	/// <param name="controlType">Control type.</param>
	/// <param name="adapter">Adapter.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control type or the adapter is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the type is not a control type.</exception>
	public static void Register(Type controlType, IEditAdapter adapter)
	{
		if(controlType is null) { throw new ArgumentNullException(nameof(controlType)); }
		if(adapter is null) { throw new ArgumentNullException(nameof(adapter)); }
		if(!typeof(Control).IsAssignableFrom(controlType)) { throw new ArgumentException("Type must be a control type.", nameof(controlType)); }

		lock(_Lock) { _Adapters[controlType] = adapter; }
	}


	/// <summary>Registers an adapter for a control type, replacing an adapter registered before.</summary>
	/// <typeparam name="T">Control type.</typeparam>
	/// <param name="adapter">Adapter.</param>
	/// <exception cref="ArgumentNullException">Thrown when the adapter is NULL.</exception>
	public static void Register<T>(IEditAdapter adapter) where T: Control
	{
		Register(typeof(T), adapter);
	}


	/// <summary>Removes the adapter registered for a control type.</summary>
	/// <param name="controlType">Control type.</param>
	/// <returns>Returns <c>true</c> if an adapter has been removed, otherwise returns <c>false</c>.</returns>
	public static bool Unregister(Type controlType)
	{
		lock(_Lock) { return _Adapters.Remove(controlType); }
	}


	/// <summary>Restores the default registrations.</summary>
	public static void Reset()
	{
		lock(_Lock)
		{
			_Adapters.Clear();
			_Adapters[typeof(TextBoxBase)] = new TextBoxEditAdapter();
			_Adapters[typeof(ComboBox)] = new ComboBoxEditAdapter();
			_Adapters[typeof(DataGridView)] = new DataGridViewEditAdapter();
			_Adapters[typeof(ListView)] = new ListViewEditAdapter();
			_Adapters[typeof(ListBox)] = new ListBoxEditAdapter();
		}
	}


	/// <summary>Gets the adapter registered for a control type or its nearest base type.</summary>
	/// <param name="controlType">Control type.</param>
	/// <returns>Returns the adapter, or NULL if no adapter applies.</returns>
	public static IEditAdapter? Find(Type controlType)
	{
		lock(_Lock)
		{
			for(Type? i = controlType; i is not null; i = i.BaseType)
			{
				if(_Adapters.TryGetValue(i, out IEditAdapter? rval)) { return rval; }
			}
		}

		return null;
	}


	/// <summary>Gets the adapter for a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the adapter, or NULL if the control is NULL or no adapter applies.</returns>
	public static IEditAdapter? Find(Control? control)
	{
		return ((control is null) ? null : Find(control.GetType()));
	}
}
