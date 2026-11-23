namespace Robbiblubber.Spellbook;

using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;



/// <summary>This class converts values to and from their ddn representation.</summary>
/// <remarks>Scalar values are strings, characters, Booleans, numbers, enums, <see cref="DateTime"/>, <see cref="DateTimeOffset"/>,
///          <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/>, <see cref="Guid"/> and all types with a type
///          converter that converts from and to strings. Collections of scalar values are represented as ddn arrays.</remarks>
internal static class _DdnValue
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Scalar type cache.</summary>
	private static readonly ConcurrentDictionary<Type, bool> _Scalars = new();

	/// <summary>Collection element type cache.</summary>
	private static readonly ConcurrentDictionary<Type, Type?> _Elements = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the type converter for a type if it converts from and to strings.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the converter, or NULL if the type has no suitable converter.</returns>
	private static TypeConverter? _GetConverter(Type t)
	{
		try
		{
			TypeConverter rval = TypeDescriptor.GetConverter(t);
			if((rval.GetType() != typeof(TypeConverter)) && rval.CanConvertFrom(typeof(string)) && rval.CanConvertTo(typeof(string))) { return rval; }
		}
		catch(Exception) {}

		return null;
	}


	/// <summary>Parses a Boolean value.</summary>
	/// <param name="s">String.</param>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the string is a Boolean value, otherwise returns FALSE.</returns>
	private static bool _ParseBool(string s, out bool value)
	{
		value = false;
		string v = s.Trim().ToLowerInvariant();

		if(long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out long n)) { value = (n != 0); return true; }
		if(v.StartsWith('t') || v.StartsWith('y') || (v == "on")) { value = true; return true; }

		return (v.StartsWith('f') || v.StartsWith('n') || (v == "off"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if a type is represented as a single ddn value.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns TRUE if the type is a scalar type, otherwise returns FALSE.</returns>
	internal static bool _IsScalar(Type t)
	{
		t = (Nullable.GetUnderlyingType(t) ?? t);

		return _Scalars.GetOrAdd(t, m =>
		{
			if(m.IsPrimitive || m.IsEnum) { return ((m != typeof(IntPtr)) && (m != typeof(UIntPtr))); }
			if((m == typeof(string)) || (m == typeof(decimal)) || (m == typeof(DateTime)) || (m == typeof(DateTimeOffset)) ||
			   (m == typeof(DateOnly)) || (m == typeof(TimeOnly)) || (m == typeof(TimeSpan)) || (m == typeof(Guid))) { return true; }
			if(typeof(IEnumerable).IsAssignableFrom(m)) { return false; }

			return (_GetConverter(m) is not null);
		});
	}


	/// <summary>Gets the element type of a collection type that is represented as a ddn array.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the element type, or NULL if the type is not a collection.</returns>
	/// <remarks>Arrays and generic collections are supported; dictionaries and strings are not collections.</remarks>
	internal static Type? _GetElementType(Type t)
	{
		return _Elements.GetOrAdd(t, m =>
		{
			if(m == typeof(string)) { return null; }
			if(m.IsArray) { return ((m.GetArrayRank() == 1) ? m.GetElementType() : null); }
			if(typeof(IDictionary).IsAssignableFrom(m)) { return null; }

			Type? rval = null;
			foreach(Type i in (m.IsInterface ? m.GetInterfaces().Append(m) : m.GetInterfaces()))
			{
				if(!i.IsGenericType) { continue; }

				Type def = i.GetGenericTypeDefinition();
				if((def == typeof(IDictionary<,>)) || (def == typeof(IReadOnlyDictionary<,>))) { return null; }
				if(def == typeof(IEnumerable<>)) { rval = i.GetGenericArguments()[0]; }
			}

			return rval;
		});
	}


	/// <summary>Creates a collection.</summary>
	/// <param name="t">Collection type.</param>
	/// <param name="elementType">Element type.</param>
	/// <param name="items">Items.</param>
	/// <returns>Returns the collection, or NULL if the collection type cannot be created.</returns>
	internal static object? _CreateCollection(Type t, Type elementType, IList items)
	{
		if(t.IsArray)
		{
			Array rval = Array.CreateInstance(elementType, items.Count);
			for(int i = 0; i < items.Count; i++) { rval.SetValue(items[i], i); }

			return rval;
		}

		Type list = typeof(List<>).MakeGenericType(elementType);
		if(t.IsAssignableFrom(list))
		{
			IList rval = (IList) Activator.CreateInstance(list)!;
			foreach(object? i in items) { rval.Add(i); }

			return rval;
		}

		if(t.IsAbstract || t.IsInterface || (t.GetConstructor(Type.EmptyTypes) is null)) { return null; }

		object col = Activator.CreateInstance(t)!;
		Type icol = typeof(ICollection<>).MakeGenericType(elementType);

		if(icol.IsAssignableFrom(t))
		{
			System.Reflection.MethodInfo add = icol.GetMethod("Add")!;
			foreach(object? i in items) { add.Invoke(col, new[] { i }); }
		}
		else if(col is IList l)
		{
			foreach(object? i in items) { l.Add(i); }
		}
		else { return null; }

		return col;
	}


	/// <summary>Formats a scalar value.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the plain string representation.</returns>
	internal static string _Format(object value)
	{
		switch(value)
		{
			case string s:          return s;
			case bool b:            return (b ? "true" : "false");
			case char c:            return c.ToString();
			case DateTime d:        return CalendarOp.ToTimestamp(d);
			case DateTimeOffset d:  return CalendarOp.ToTimestamp(d);
			case DateOnly d:        return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			case TimeOnly d:        return d.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
			case TimeSpan d:        return d.ToString("c", CultureInfo.InvariantCulture);
			case Guid g:            return g.ToString("D");
			case Enum e:            return e.ToString();
			case IFormattable f:    return f.ToString(null, CultureInfo.InvariantCulture);
		}

		return (_GetConverter(value.GetType())?.ConvertToInvariantString(value) ?? value.ToString() ?? string.Empty);
	}


	/// <summary>Parses a scalar value.</summary>
	/// <param name="s">Plain string.</param>
	/// <param name="t">Type; nullable types are parsed as their underlying type.</param>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the string could be parsed, otherwise returns FALSE.</returns>
	internal static bool _TryParse(string s, Type t, out object? value)
	{
		t = (Nullable.GetUnderlyingType(t) ?? t);
		value = null;

		CultureInfo inv = CultureInfo.InvariantCulture;
		NumberStyles num = NumberStyles.Integer;
		NumberStyles flt = NumberStyles.Float;

		if(t == typeof(string)) { value = s; return true; }
		if(t == typeof(char))
		{
			if(s.Length != 1) { s = s.Trim(); }
			if(s.Length == 0) { return false; }

			value = s[0];
			return true;
		}

		s = s.Trim();
		if(t.IsEnum)
		{
			if(!Enum.TryParse(t, s, true, out value)) { return false; }
			return true;
		}

		switch(Type.GetTypeCode(t))
		{
			case TypeCode.Boolean:  { bool rval = _ParseBool(s, out bool v); value = v; return rval; }
			case TypeCode.SByte:    { bool rval = sbyte.TryParse(s, num, inv, out sbyte v); value = v; return rval; }
			case TypeCode.Byte:     { bool rval = byte.TryParse(s, num, inv, out byte v); value = v; return rval; }
			case TypeCode.Int16:    { bool rval = short.TryParse(s, num, inv, out short v); value = v; return rval; }
			case TypeCode.UInt16:   { bool rval = ushort.TryParse(s, num, inv, out ushort v); value = v; return rval; }
			case TypeCode.Int32:    { bool rval = int.TryParse(s, num, inv, out int v); value = v; return rval; }
			case TypeCode.UInt32:   { bool rval = uint.TryParse(s, num, inv, out uint v); value = v; return rval; }
			case TypeCode.Int64:    { bool rval = long.TryParse(s, num, inv, out long v); value = v; return rval; }
			case TypeCode.UInt64:   { bool rval = ulong.TryParse(s, num, inv, out ulong v); value = v; return rval; }
			case TypeCode.Single:   { bool rval = float.TryParse(s, flt, inv, out float v); value = v; return rval; }
			case TypeCode.Double:   { bool rval = double.TryParse(s, flt, inv, out double v); value = v; return rval; }
			case TypeCode.Decimal:  { bool rval = decimal.TryParse(s, flt, inv, out decimal v); value = v; return rval; }
			case TypeCode.DateTime:
				{
					if(!CalendarOp._TryParse(s, out DateTimeOffset v, out bool zoned)) { return false; }
					value = (zoned ? v.LocalDateTime : DateTime.SpecifyKind(v.DateTime, DateTimeKind.Local));
					return true;
				}
		}

		if(t == typeof(DateTimeOffset))
		{
			if(!CalendarOp._TryParse(s, out DateTimeOffset v, out _)) { return false; }
			value = v; return true;
		}
		if(t == typeof(DateOnly))
		{
			if(!CalendarOp._TryParse(s, out DateTimeOffset v, out _)) { return false; }
			value = DateOnly.FromDateTime(v.DateTime); return true;
		}
		if(t == typeof(TimeOnly))
		{
			if(!TimeOnly.TryParseExact(s, new[] { "H:mm:ss.FFFFFFF", "H:mm" }, inv, DateTimeStyles.None, out TimeOnly v)) { return false; }
			value = v; return true;
		}
		if(t == typeof(TimeSpan))
		{
			if(!TimeSpan.TryParse(s, inv, out TimeSpan v)) { return false; }
			value = v; return true;
		}
		if(t == typeof(Guid))
		{
			if(!Guid.TryParse(s, out Guid v)) { return false; }
			value = v; return true;
		}

		TypeConverter? cnv = _GetConverter(t);
		if(cnv is null) { return false; }

		try
		{
			value = cnv.ConvertFromInvariantString(s);
			return true;
		}
		catch(Exception) { return false; }
	}


	/// <summary>Converts a value to masked ddn text.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the masked text.</returns>
	/// <remarks>Collections are converted to ddn arrays, NULL to the NULL mask.</remarks>
	internal static string _ToText(object? value)
	{
		if(value is null) { return _DdnText._NULL; }
		if(value is string s) { return _DdnText._Mask(s); }

		if((value is IEnumerable e) && (!(value is IDictionary)) && (!_IsScalar(value.GetType())))
		{
			return string.Join(", ", e.Cast<object?>().Select(m => ((m is null) ? _DdnText._NULL : _DdnText._Mask(_Format(m)))));
		}

		return _DdnText._Mask(_Format(value));
	}


	/// <summary>Converts masked ddn text to a value.</summary>
	/// <param name="text">Masked text.</param>
	/// <param name="t">Type.</param>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the text could be converted, otherwise returns FALSE.</returns>
	/// <exception cref="InvalidCastException">Thrown when the type is neither a scalar nor a collection of scalars.</exception>
	internal static bool _TryFromText(string text, Type t, out object? value)
	{
		value = null;
		if(text == _DdnText._NULL) { return false; }

		Type? nt = Nullable.GetUnderlyingType(t);
		if(nt is not null)
		{
			if(string.IsNullOrWhiteSpace(text)) { return false; }
			t = nt;
		}

		if(_IsScalar(t)) { return _TryParse(_DdnText._Unmask(text), t, out value); }

		Type? et = _GetElementType(t);
		if((et is null) || ((!_IsScalar(et)) && (et != typeof(object))))
		{
			throw new InvalidCastException("Type " + t.FullName + " cannot be converted from a ddn value. Use Ddn.Deserialize() for complex types.");
		}

		List<object?> items = new();
		foreach(string i in _DdnText._SplitList(text)) { items.Add(_ItemFromText(i, et)); }

		value = _CreateCollection(t, et, items);
		return (value is not null);
	}


	/// <summary>Converts masked ddn text of an array item to a value.</summary>
	/// <param name="text">Masked item text.</param>
	/// <param name="t">Item type.</param>
	/// <returns>Returns the value; NULL or invalid items return NULL for nullable types and the default value otherwise.</returns>
	internal static object? _ItemFromText(string text, Type t)
	{
		if(t == typeof(object)) { return ((text == _DdnText._NULL) ? null : _DdnText._Unmask(text)); }
		if(_TryFromText(text, t, out object? rval)) { return rval; }

		return (((Nullable.GetUnderlyingType(t) is null) && t.IsValueType) ? Activator.CreateInstance(t) : null);
	}
}
