namespace Robbiblubber.Spellbook;

using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;



/// <summary>This class implements ddn serialization.</summary>
internal static class _DdnSerializer
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal constants                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Element name for type names.</summary>
	internal const string _TYPE = "$type";

	/// <summary>Element name for values serialized into a root element.</summary>
	internal const string _VALUE = "value";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Type information cache.</summary>
	private static readonly ConcurrentDictionary<Type, _TypeInfo> _Types = new();

	/// <summary>Resolved type names.</summary>
	private static readonly ConcurrentDictionary<string, Type?> _TypeNames = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static members                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom serializers.</summary>
	internal static readonly ConcurrentDictionary<Type, Ddn.ISerializer> _Serializers = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the custom serializer for a type.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the serializer, or NULL if there is none.</returns>
	private static Ddn.ISerializer? _GetSerializer(Type t)
	{
		if(_Serializers.IsEmpty) { return null; }

		for(Type? i = t; i is not null; i = i.BaseType)
		{
			if(_Serializers.TryGetValue(i, out Ddn.ISerializer? rval)) { return rval; }
		}

		return null;
	}




	/// <summary>Gets the key and value types of a dictionary type.</summary>
	/// <param name="t">Type.</param>
	/// <param name="keyType">Key type.</param>
	/// <param name="valueType">Value type.</param>
	/// <returns>Returns TRUE if the type is a dictionary, otherwise returns FALSE.</returns>
	private static bool _IsDictionary(Type t, out Type keyType, out Type valueType)
	{
		keyType = valueType = typeof(object);

		foreach(Type i in (t.IsInterface ? t.GetInterfaces().Append(t) : t.GetInterfaces()))
		{
			if(!i.IsGenericType) { continue; }

			Type def = i.GetGenericTypeDefinition();
			if((def == typeof(IDictionary<,>)) || (def == typeof(IReadOnlyDictionary<,>)))
			{
				keyType = i.GetGenericArguments()[0];
				valueType = i.GetGenericArguments()[1];
				return true;
			}
		}

		return typeof(IDictionary).IsAssignableFrom(t);
	}


	/// <summary>Gets the type name written for a type.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the type name.</returns>
	private static string _GetTypeName(Type t)
	{
		return (t.FullName + ", " + t.Assembly.GetName().Name);
	}


	/// <summary>Resolves a type name.</summary>
	/// <param name="name">Type name.</param>
	/// <returns>Returns the type, or NULL if the type has not been found.</returns>
	private static Type? _ResolveType(string name)
	{
		return _TypeNames.GetOrAdd(name, m =>
		{
			try
			{
				Type? rval = Type.GetType(m, false);
				if(rval is not null) { return rval; }
			}
			catch(Exception) {}

			string full = m.Split(',')[0].Trim();
			foreach(Assembly i in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type? rval = i.GetType(full, false);
				if(rval is not null) { return rval; }
			}

			return null;
		});
	}


	/// <summary>Gets the type information for a type.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the type information.</returns>
	private static _TypeInfo _GetTypeInfo(Type t)
	{
		return _Types.GetOrAdd(t, m => new _TypeInfo(m));
	}


	/// <summary>Creates a serialization exception.</summary>
	/// <param name="elm">Element.</param>
	/// <param name="message">Message.</param>
	/// <param name="innerException">Inner exception.</param>
	/// <returns>Returns the exception.</returns>
	private static Ddn.SerializeException _Error(DdnElement elm, string message, Exception? innerException = null)
	{
		return new Ddn.SerializeException(message + " (" + elm.Path + ")", innerException);
	}


	/// <summary>Gets the child elements of a section that represent collection items, in index order.</summary>
	/// <param name="elm">Element.</param>
	/// <returns>Returns the item elements.</returns>
	private static IEnumerable<DdnElement> _GetItems(DdnElement elm)
	{
		return elm.Elements.Where(m => m.Name != _TYPE)
						   .Select((m, n) => (Element: m, Index: (int.TryParse(m.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out int k) ? k : (int.MaxValue / 2) + n)))
						   .OrderBy(m => m.Index)
						   .Select(m => m.Element);
	}


	/// <summary>Clears a collection.</summary>
	/// <param name="col">Collection.</param>
	/// <param name="elementType">Element type.</param>
	/// <returns>Returns TRUE if the collection has been cleared, FALSE if it cannot be modified.</returns>
	private static bool _Clear(object col, Type elementType)
	{
		if(col is Array) { return false; }

		Type icol = typeof(ICollection<>).MakeGenericType(elementType);
		if(icol.IsInstanceOfType(col))
		{
			if((bool) icol.GetProperty("IsReadOnly")!.GetValue(col)!) { return false; }
			icol.GetMethod("Clear")!.Invoke(col, null);
			return true;
		}

		if((col is IList l) && (!l.IsReadOnly) && (!l.IsFixedSize)) { l.Clear(); return true; }
		if((col is IDictionary d) && (!d.IsReadOnly) && (!d.IsFixedSize)) { d.Clear(); return true; }

		return false;
	}


	/// <summary>Serializes a dictionary.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="value">Dictionary.</param>
	/// <param name="valueType">Declared value type.</param>
	/// <param name="elm">Element.</param>
	private static void _SerializeDictionary(Ddn.SerializationContext context, object value, Type valueType, DdnElement elm)
	{
		foreach(object? i in (IEnumerable) value)
		{
			object? key, val;

			if(i is DictionaryEntry e)
			{
				key = e.Key;
				val = e.Value;
			}
			else
			{
				Type t = i!.GetType();
				key = t.GetProperty("Key")!.GetValue(i);
				val = t.GetProperty("Value")!.GetValue(i);
			}

			if((key is null) || (!_DdnValue._IsScalar(key.GetType()))) { throw _Error(elm, "Dictionary keys must be scalar values."); }
			_Serialize(context, val, valueType, elm.Element(_DdnValue._Format(key)));
		}
	}


	/// <summary>Deserializes a dictionary.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="t">Dictionary type.</param>
	/// <param name="keyType">Key type.</param>
	/// <param name="valueType">Value type.</param>
	/// <param name="elm">Element.</param>
	/// <param name="existing">Existing dictionary that is filled, or NULL.</param>
	/// <returns>Returns the dictionary.</returns>
	private static object? _DeserializeDictionary(Ddn.SerializationContext context, Type t, Type keyType, Type valueType, DdnElement elm, object? existing)
	{
		object? rval = existing;

		if((rval is null) || (!_Clear(rval, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType))))
		{
			Type create = t;
			if(t.IsInterface || t.IsAbstract)
			{
				create = ((t == typeof(IDictionary)) ? typeof(Dictionary<object, object?>) : typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
				if(!t.IsAssignableFrom(create)) { throw _Error(elm, "Cannot create dictionary of type " + t.FullName + "."); }
			}

			try
			{
				rval = Activator.CreateInstance(create, true);
			}
			catch(Exception ex) { throw _Error(elm, "Cannot create dictionary of type " + t.FullName + ".", ex); }
		}

		PropertyInfo? item = rval!.GetType().GetProperty("Item", new[] { keyType });
		if((item is null) && (!(rval is IDictionary))) { throw _Error(elm, "Cannot fill dictionary of type " + t.FullName + "."); }

		foreach(DdnElement i in elm.Elements)
		{
			if(i.Name == _TYPE) { continue; }
			if(!_DdnValue._TryParse(i.Name, keyType, out object? key) || (key is null)) { throw _Error(i, "Invalid dictionary key."); }

			object? val = _Deserialize(context, valueType, i, null);
			if(item is not null)
			{
				item.SetValue(rval, val, new[] { key });
			}
			else { ((IDictionary) rval)[key] = val; }
		}

		return rval;
	}


	/// <summary>Deserializes a collection.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="t">Collection type.</param>
	/// <param name="elementType">Element type.</param>
	/// <param name="elm">Element.</param>
	/// <param name="existing">Existing collection that is filled, or NULL.</param>
	/// <returns>Returns the collection.</returns>
	private static object? _DeserializeCollection(Ddn.SerializationContext context, Type t, Type elementType, DdnElement elm, object? existing)
	{
		List<object?> items = new();

		if(elm.IsSection)
		{
			foreach(DdnElement i in _GetItems(elm)) { items.Add(_Deserialize(context, elementType, i, null)); }
		}
		else
		{
			if(!(_DdnValue._IsScalar(elementType) || (elementType == typeof(object)))) { return existing; }
			elm._GetList(((elementType == typeof(object)) ? typeof(string) : elementType), items);
		}

		if((existing is not null) && _Clear(existing, elementType))
		{
			Type icol = typeof(ICollection<>).MakeGenericType(elementType);
			if(icol.IsInstanceOfType(existing))
			{
				MethodInfo add = icol.GetMethod("Add")!;
				foreach(object? i in items) { add.Invoke(existing, new[] { i }); }
			}
			else
			{
				foreach(object? i in items) { ((IList) existing).Add(i); }
			}

			return existing;
		}

		object? rval = _DdnValue._CreateCollection(t, elementType, items);
		if(rval is null) { throw _Error(elm, "Cannot create collection of type " + t.FullName + "."); }

		return rval;
	}


	/// <summary>Creates an object for deserialization.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="info">Type information.</param>
	/// <param name="elm">Element.</param>
	/// <param name="bound">Returns the names of the members that have been passed to the constructor.</param>
	/// <returns>Returns the object.</returns>
	private static object _CreateInstance(Ddn.SerializationContext context, _TypeInfo info, DdnElement elm, out HashSet<string>? bound)
	{
		bound = null;
		Type t = info.Type;

		if(t.IsAbstract || t.IsInterface) { throw _Error(elm, "Cannot create an instance of the abstract type " + t.FullName + "; consider type names."); }

		try
		{
			if(t.IsValueType || (info.DefaultConstructor is not null))
			{
				return (info.DefaultConstructor?.Invoke(null) ?? Activator.CreateInstance(t)!);
			}

			if(info.Constructor is null) { throw _Error(elm, "Type " + t.FullName + " has no suitable constructor."); }

			ParameterInfo[] pars = info.Constructor.GetParameters();
			object?[] args = new object?[pars.Length];
			bound = new(StringComparer.Ordinal);

			for(int i = 0; i < pars.Length; i++)
			{
				_Member m = info.ConstructorMembers![i];
				DdnElement child = elm.Element(m.Name);

				if(child.Exists)
				{
					args[i] = _Deserialize(context, pars[i].ParameterType, child, null);
				}
				else if(pars[i].HasDefaultValue)
				{
					args[i] = pars[i].DefaultValue;
				}
				else { args[i] = (pars[i].ParameterType.IsValueType ? Activator.CreateInstance(pars[i].ParameterType) : null); }

				bound.Add(m.Name);
			}

			return info.Constructor.Invoke(args);
		}
		catch(Ddn.SerializeException) { throw; }
		catch(Exception ex) { throw _Error(elm, "Cannot create an instance of " + t.FullName + ".", ex); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a type is written as a ddn value (a scalar or an array of scalars).</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns TRUE if the type is a value type in ddn terms, otherwise returns FALSE.</returns>
	internal static bool _IsValue(Type t)
	{
		t = (Nullable.GetUnderlyingType(t) ?? t);
		if(_DdnValue._IsScalar(t)) { return true; }

		Type? et = _DdnValue._GetElementType(t);
		return ((et is not null) && _DdnValue._IsScalar(Nullable.GetUnderlyingType(et) ?? et));
	}


	/// <summary>Returns if an object is written as a ddn value.</summary>
	/// <param name="value">Object.</param>
	/// <returns>Returns TRUE if the object is a scalar, a collection of scalars or a collection of objects that only contains
	///          scalars and NULL values, otherwise returns FALSE.</returns>
	internal static bool _IsValueInstance(object value)
	{
		Type t = value.GetType();
		if(_IsValue(t)) { return true; }
		if((!(value is IEnumerable e)) || t.IsArray || _IsDictionary(t, out _, out _)) { return false; }

		Type? et = _DdnValue._GetElementType(t);
		if((et is not null) && (et != typeof(object))) { return false; }

		foreach(object? i in e)
		{
			if((i is not null) && (!_DdnValue._IsScalar(i.GetType()))) { return false; }
		}
		return true;
	}


	/// <summary>Serializes a value.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="value">Value.</param>
	/// <param name="declared">Declared type.</param>
	/// <param name="elm">Element.</param>
	internal static void _Serialize(Ddn.SerializationContext context, object? value, Type declared, DdnElement elm)
	{
		if(value is null) { elm.SetNull(); return; }
		if(value is DdnElement d) { elm.Set(d); return; }

		Type t = value.GetType();
		Ddn.ISerializer? custom = _GetSerializer(t);

		if((custom is null) && _IsValueInstance(value))
		{
			elm.Set(value);
			return;
		}

		if((!t.IsValueType) && (!context._Stack.Add(value))) { throw _Error(elm, "Circular reference to an object of type " + t.FullName + "."); }

		try
		{
			elm._MakeSection();

			Ddn.TypeNameHandling names = context.Options.TypeNames;
			if((names == Ddn.TypeNameHandling.ALL) || ((names == Ddn.TypeNameHandling.AUTO) && (t != (Nullable.GetUnderlyingType(declared) ?? declared))))
			{
				elm.Element(_TYPE).Set(_GetTypeName(t));
			}

			if(custom is not null)
			{
				custom.Serialize(value, elm, context);
			}
			else if(_IsDictionary(t, out _, out Type valueType))
			{
				_SerializeDictionary(context, value, valueType, elm);
			}
			else if(value is IEnumerable e)
			{
				if(t.IsArray && (t.GetArrayRank() > 1)) { throw _Error(elm, "Multi-dimensional arrays are not supported."); }

				Type et = (_DdnValue._GetElementType(t) ?? typeof(object));
				int n = 0;
				foreach(object? i in e) { _Serialize(context, i, et, elm.Element((n++).ToString(CultureInfo.InvariantCulture))); }
			}
			else { _SerializeMembers(context, value, elm); }
		}
		finally
		{
			if(!t.IsValueType) { context._Stack.Remove(value); }
		}
	}


	/// <summary>Serializes the members of an object.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="value">Object.</param>
	/// <param name="elm">Element.</param>
	internal static void _SerializeMembers(Ddn.SerializationContext context, object value, DdnElement elm)
	{
		foreach(_Member i in _GetTypeInfo(value.GetType()).Members)
		{
			if(i.Getter is null) { continue; }

			object? v;
			try
			{
				v = i.Getter(value);
			}
			catch(Exception ex) { throw _Error(elm, "Cannot read member " + i.Name + ".", ex); }

			_Serialize(context, v, i.Type, elm.Element(i.Name));
		}
	}


	/// <summary>Deserializes a value.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="t">Declared type.</param>
	/// <param name="elm">Element.</param>
	/// <param name="existing">Existing value that is returned if the element does not exist or that may be filled.</param>
	/// <returns>Returns the value.</returns>
	internal static object? _Deserialize(Ddn.SerializationContext context, Type t, DdnElement elm, object? existing)
	{
		if(!elm.Exists) { return existing; }

		Type? nt = Nullable.GetUnderlyingType(t);
		if(elm.IsNull) { return ((t.IsValueType && (nt is null)) ? Activator.CreateInstance(t) : null); }
		if(t == typeof(DdnElement)) { return elm.Clone(); }
		if(nt is not null) { t = nt; }

		if(elm.IsRoot && elm.IsSection && _IsValue(t) && elm.Element(_VALUE).Exists) { elm = elm.Element(_VALUE); }

		if(elm.IsSection && (context.Options.TypeNames != Ddn.TypeNameHandling.NONE))
		{
			string? name = elm.Element(_TYPE).Value;
			if(!string.IsNullOrWhiteSpace(name))
			{
				Type? rt = _ResolveType(name);
				if(rt is null) { throw _Error(elm, "Type " + name + " not found."); }
				if(!t.IsAssignableFrom(rt)) { throw _Error(elm, "Type " + name + " is not assignable to " + t.FullName + "."); }

				if((existing is not null) && (existing.GetType() != rt)) { existing = null; }
				t = rt;
			}
		}

		Ddn.ISerializer? custom = _GetSerializer(t);
		if(custom is not null)
		{
			try
			{
				return custom.Deserialize(elm, existing, context);
			}
			catch(Ddn.SerializeException) { throw; }
			catch(Exception ex) { throw _Error(elm, "Custom serializer for " + t.FullName + " failed.", ex); }
		}

		if(_DdnValue._IsScalar(t))
		{
			object? rval = (elm.IsSection ? null : elm.Get(t));
			return (rval ?? existing ?? (t.IsValueType ? Activator.CreateInstance(t) : null));
		}

		if(t == typeof(object)) { return (elm.IsSection ? elm.Clone() : elm.Value); }

		if(_IsDictionary(t, out Type keyType, out Type valueType))
		{
			if(!elm.IsSection) { return existing; }
			return _DeserializeDictionary(context, t, keyType, valueType, elm, existing);
		}

		Type? et = _DdnValue._GetElementType(t);
		if(et is not null) { return _DeserializeCollection(context, t, et, elm, existing); }

		if(!elm.IsSection) { return existing; }

		return _DeserializeMembers(context, existing, _GetTypeInfo(t), elm);
	}


	/// <summary>Deserializes the members of an object.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="value">Object.</param>
	/// <param name="elm">Element.</param>
	/// <returns>Returns the object.</returns>
	internal static object _DeserializeMembers(Ddn.SerializationContext context, object value, DdnElement elm)
	{
		return _DeserializeMembers(context, value, _GetTypeInfo(value.GetType()), elm);
	}


	/// <summary>Deserializes the members of an object.</summary>
	/// <param name="context">Serialization context.</param>
	/// <param name="value">Object, or NULL to create a new object.</param>
	/// <param name="info">Type information.</param>
	/// <param name="elm">Element.</param>
	/// <returns>Returns the object.</returns>
	private static object _DeserializeMembers(Ddn.SerializationContext context, object? value, _TypeInfo info, DdnElement elm)
	{
		HashSet<string>? bound = null;
		object rval = (value ?? _CreateInstance(context, info, elm, out bound));

		foreach(_Member i in info.Members)
		{
			if((i.Setter is null) && ((i.Reader is null) || i.IsValue)) { continue; }
			if(bound?.Contains(i.Name) ?? false) { continue; }

			DdnElement child = elm.Element(i.Name);
			if(!child.Exists) { continue; }

			try
			{
				object? cur = (((i.Reader is null) || ((i.Setter is not null) && i.IsCollection)) ? null : i.Reader(rval));
				object? v = _Deserialize(context, i.Type, child, cur);

				i.Setter?.Invoke(rval, v);
			}
			catch(Ddn.SerializeException) { throw; }
			catch(Exception ex) { throw _Error(child, "Cannot set member " + i.Name + ".", ex); }
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Member                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class describes a serialized member.</summary>
	private sealed class _Member
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Element name.</param>
		/// <param name="type">Member type.</param>
		/// <param name="memberName">Member name.</param>
		public _Member(string name, Type type, string memberName)
		{
			Name = name;
			Type = type;
			MemberName = memberName;
			IsValue = _IsValue(type);
			IsCollection = ((!IsValue) && (_IsDictionary(type, out _, out _) || (_DdnValue._GetElementType(type) is not null)));
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the element name.</summary>
		public string Name
		{
			get;
		}


		/// <summary>Gets the member type.</summary>
		public Type Type
		{
			get;
		}


		/// <summary>Gets the member name.</summary>
		public string MemberName
		{
			get;
		}


		/// <summary>Gets if the member type is written as a ddn value.</summary>
		public bool IsValue
		{
			get;
		}


		/// <summary>Gets if the member type is a collection or dictionary.</summary>
		public bool IsCollection
		{
			get;
		}


		/// <summary>Gets or sets the method that reads the member for serialization, or NULL.</summary>
		public Func<object, object?>? Getter
		{
			get; set;
		}


		/// <summary>Gets or sets the method that writes the member on deserialization, or NULL.</summary>
		public Action<object, object?>? Setter
		{
			get; set;
		}


		/// <summary>Gets or sets the method that reads the member to fill it on deserialization, or NULL.</summary>
		public Func<object, object?>? Reader
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _TypeInfo                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class describes the serialization of a type.</summary>
	private sealed class _TypeInfo
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="t">Type.</param>
		public _TypeInfo(Type t)
		{
			Type = t;

			Ddn.SerializableAttribute attr = (t.GetCustomAttribute<Ddn.SerializableAttribute>(true) ?? Ddn.SerializableAttribute._DEFAULT);
			Dictionary<string, _Member> members = new(StringComparer.Ordinal);
			HashSet<string> seen = new(StringComparer.Ordinal);
			List<Type> hierarchy = new();

			for(Type? i = t; (i is not null) && (i != typeof(object)) && (i != typeof(ValueType)); i = i.BaseType) { hierarchy.Insert(0, i); }

			foreach(Type i in hierarchy)
			{
				BindingFlags flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				foreach(PropertyInfo k in i.GetProperties(flags))
				{
					if(!seen.Add("P:" + k.Name)) { continue; }
					_AddProperty(members, attr, i, k);
				}

				foreach(FieldInfo k in i.GetFields(flags))
				{
					_AddField(members, attr, k);
				}

				foreach(MethodInfo k in i.GetMethods(flags))
				{
					_AddMethod(members, k);
				}
			}

			Members = members.Values.ToArray();
			_FindConstructor(t);
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private static methods                                                                                       //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Adds a member.</summary>
		/// <param name="members">Members.</param>
		/// <param name="name">Element name.</param>
		/// <param name="type">Member type.</param>
		/// <param name="memberName">Member name.</param>
		/// <param name="getter">Getter for serialization.</param>
		/// <param name="setter">Setter for deserialization.</param>
		/// <param name="reader">Reader for filling on deserialization.</param>
		private static void _Add(Dictionary<string, _Member> members, string name, Type type, string memberName,
								 Func<object, object?>? getter, Action<object, object?>? setter, Func<object, object?>? reader)
		{
			if((getter is null) && (setter is null) && (reader is null)) { return; }

			if(members.TryGetValue(name, out _Member? m))
			{
				if((m.Type != type) || ((getter is not null) && (m.Getter is not null)) || ((setter is not null) && (m.Setter is not null)))
				{
					throw new Ddn.SerializeException("Duplicate element name " + name + " in type " + type.FullName + ".");
				}
			}
			else { members.Add(name, (m = new _Member(name, type, memberName))); }

			m.Getter ??= getter;
			m.Setter ??= setter;
			m.Reader ??= reader;
		}


		/// <summary>Adds a property.</summary>
		/// <param name="members">Members.</param>
		/// <param name="attr">Serializable attribute of the type.</param>
		/// <param name="declaring">Declaring type.</param>
		/// <param name="p">Property.</param>
		private static void _AddProperty(Dictionary<string, _Member> members, Ddn.SerializableAttribute attr, Type declaring, PropertyInfo p)
		{
			if(p.GetIndexParameters().Length > 0) { return; }
			if(p.GetCustomAttribute<Ddn.IgnoreAttribute>() is not null) { return; }
			if(typeof(Delegate).IsAssignableFrom(p.PropertyType)) { return; }

			Ddn.SerializeAttribute? sattr = p.GetCustomAttribute<Ddn.SerializeAttribute>();
			MethodInfo? get = p.GetMethod;
			FieldInfo? backing = declaring.GetField("<" + p.Name + ">k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
			bool settable = ((p.SetMethod is not null) || (backing is not null));

			if(sattr is null)
			{
				if((!attr.IncludeProperties) || (get is null) || (!settable)) { return; }
				if((!get.IsPublic) && (!attr.IncludeNonPublic)) { return; }
				if(get.IsStatic) { return; }
			}

			Ddn.SerializationDirection dir = (sattr?.Direction ?? Ddn.SerializationDirection.BIDIRECTIONAL);
			Func<object, object?>? reader = ((get is null) ? null : new Func<object, object?>(m => p.GetValue(m)));
			Action<object, object?>? setter = null;

			if(p.SetMethod is not null)
			{
				setter = (m, n) => p.SetValue(m, n);
			}
			else if(backing is not null) { setter = (m, n) => backing.SetValue(m, n); }

			_Add(members, (sattr?.Name ?? p.Name), p.PropertyType, p.Name,
				 (dir.HasFlag(Ddn.SerializationDirection.OUTBOUND) ? reader : null),
				 (dir.HasFlag(Ddn.SerializationDirection.INBOUND) ? setter : null),
				 (dir.HasFlag(Ddn.SerializationDirection.INBOUND) ? reader : null));
		}


		/// <summary>Adds a field.</summary>
		/// <param name="members">Members.</param>
		/// <param name="attr">Serializable attribute of the type.</param>
		/// <param name="f">Field.</param>
		private static void _AddField(Dictionary<string, _Member> members, Ddn.SerializableAttribute attr, FieldInfo f)
		{
			if(f.IsLiteral || f.IsStatic || (f.GetCustomAttribute<CompilerGeneratedAttribute>() is not null) || f.Name.StartsWith('<')) { return; }
			if(f.GetCustomAttribute<Ddn.IgnoreAttribute>() is not null) { return; }
			if(typeof(Delegate).IsAssignableFrom(f.FieldType)) { return; }

			Ddn.SerializeAttribute? sattr = f.GetCustomAttribute<Ddn.SerializeAttribute>();
			if(sattr is null)
			{
				if(!attr.IncludeFields) { return; }
				if((!f.IsPublic) && (!attr.IncludeNonPublic)) { return; }
			}

			Ddn.SerializationDirection dir = (sattr?.Direction ?? Ddn.SerializationDirection.BIDIRECTIONAL);
			Func<object, object?> reader = (m => f.GetValue(m));

			_Add(members, (sattr?.Name ?? f.Name), f.FieldType, f.Name,
				 (dir.HasFlag(Ddn.SerializationDirection.OUTBOUND) ? reader : null),
				 (dir.HasFlag(Ddn.SerializationDirection.INBOUND) ? ((m, n) => f.SetValue(m, n)) : null),
				 (dir.HasFlag(Ddn.SerializationDirection.INBOUND) ? reader : null));
		}


		/// <summary>Adds a method.</summary>
		/// <param name="members">Members.</param>
		/// <param name="m">Method.</param>
		private static void _AddMethod(Dictionary<string, _Member> members, MethodInfo m)
		{
			Ddn.SerializeAttribute? sattr = m.GetCustomAttribute<Ddn.SerializeAttribute>();
			if(sattr is null) { return; }

			ParameterInfo[] pars = m.GetParameters();
			string name = (sattr.Name ?? m.Name);

			if((pars.Length == 0) && (m.ReturnType != typeof(void)))
			{
				if(sattr.Direction.HasFlag(Ddn.SerializationDirection.OUTBOUND)) { _Add(members, name, m.ReturnType, m.Name, (n => m.Invoke(n, null)), null, null); }
			}
			else if(pars.Length == 1)
			{
				if(sattr.Direction.HasFlag(Ddn.SerializationDirection.INBOUND)) { _Add(members, name, pars[0].ParameterType, m.Name, null, ((n, v) => m.Invoke(n, new[] { v })), null); }
			}
			else { throw new Ddn.SerializeException("Method " + m.Name + " cannot be serialized: it must either return a value without parameters or take one parameter."); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private methods                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Finds the constructor for deserialization.</summary>
		/// <param name="t">Type.</param>
		private void _FindConstructor(Type t)
		{
			BindingFlags flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			DefaultConstructor = t.GetConstructor(flags, Type.EmptyTypes);
			if((DefaultConstructor is not null) || t.IsValueType) { return; }

			foreach(ConstructorInfo i in t.GetConstructors().OrderByDescending(m => m.GetParameters().Length))
			{
				List<_Member> bound = new();
				foreach(ParameterInfo k in i.GetParameters())
				{
					_Member? m = Members.FirstOrDefault(n => (string.Equals(n.MemberName, k.Name, StringComparison.OrdinalIgnoreCase) ||
															  string.Equals(n.Name, k.Name, StringComparison.OrdinalIgnoreCase)));
					if((m is null) || ((!k.ParameterType.IsAssignableFrom(m.Type)) && (!m.Type.IsAssignableFrom(k.ParameterType)))) { break; }
					bound.Add(m);
				}

				if(bound.Count == i.GetParameters().Length)
				{
					Constructor = i;
					ConstructorMembers = bound.ToArray();
					return;
				}
			}
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the type.</summary>
		public Type Type
		{
			get;
		}


		/// <summary>Gets the serialized members.</summary>
		public _Member[] Members
		{
			get;
		}


		/// <summary>Gets the parameterless constructor, or NULL.</summary>
		public ConstructorInfo? DefaultConstructor
		{
			get; private set;
		}


		/// <summary>Gets the constructor whose parameters correspond to members, or NULL.</summary>
		public ConstructorInfo? Constructor
		{
			get; private set;
		}


		/// <summary>Gets the members that correspond to the constructor parameters, or NULL.</summary>
		public _Member[]? ConstructorMembers
		{
			get; private set;
		}
	}
}
