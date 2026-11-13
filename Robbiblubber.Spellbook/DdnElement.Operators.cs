namespace Robbiblubber.Spellbook;



public sealed partial class DdnElement
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// operators                                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts an element to a string; NULL values return an empty string.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator string(DdnElement? value)
	{
		return (value?.Get<string>() ?? string.Empty);
	}

	/// <summary>Converts a string to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(string? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>bool</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator bool(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<bool>());
	}

	/// <summary>Converts an element to <c>bool?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator bool?(DdnElement? value)
	{
		return value?.Get<bool?>();
	}

	/// <summary>Converts <c>bool</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(bool value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>bool?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(bool? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>int</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator int(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<int>());
	}

	/// <summary>Converts an element to <c>int?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator int?(DdnElement? value)
	{
		return value?.Get<int?>();
	}

	/// <summary>Converts <c>int</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(int value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>int?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(int? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>long</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator long(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<long>());
	}

	/// <summary>Converts an element to <c>long?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator long?(DdnElement? value)
	{
		return value?.Get<long?>();
	}

	/// <summary>Converts <c>long</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(long value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>long?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(long? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>short</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator short(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<short>());
	}

	/// <summary>Converts an element to <c>short?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator short?(DdnElement? value)
	{
		return value?.Get<short?>();
	}

	/// <summary>Converts <c>short</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(short value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>short?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(short? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>byte</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator byte(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<byte>());
	}

	/// <summary>Converts an element to <c>byte?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator byte?(DdnElement? value)
	{
		return value?.Get<byte?>();
	}

	/// <summary>Converts <c>byte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(byte value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>byte?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(byte? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>uint</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator uint(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<uint>());
	}

	/// <summary>Converts an element to <c>uint?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator uint?(DdnElement? value)
	{
		return value?.Get<uint?>();
	}

	/// <summary>Converts <c>uint</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(uint value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>uint?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(uint? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>ulong</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ulong(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<ulong>());
	}

	/// <summary>Converts an element to <c>ulong?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ulong?(DdnElement? value)
	{
		return value?.Get<ulong?>();
	}

	/// <summary>Converts <c>ulong</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ulong value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>ulong?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ulong? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>ushort</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ushort(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<ushort>());
	}

	/// <summary>Converts an element to <c>ushort?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ushort?(DdnElement? value)
	{
		return value?.Get<ushort?>();
	}

	/// <summary>Converts <c>ushort</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ushort value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>ushort?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ushort? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>sbyte</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator sbyte(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<sbyte>());
	}

	/// <summary>Converts an element to <c>sbyte?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator sbyte?(DdnElement? value)
	{
		return value?.Get<sbyte?>();
	}

	/// <summary>Converts <c>sbyte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(sbyte value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>sbyte?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(sbyte? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>float</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator float(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<float>());
	}

	/// <summary>Converts an element to <c>float?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator float?(DdnElement? value)
	{
		return value?.Get<float?>();
	}

	/// <summary>Converts <c>float</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(float value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>float?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(float? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>double</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator double(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<double>());
	}

	/// <summary>Converts an element to <c>double?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator double?(DdnElement? value)
	{
		return value?.Get<double?>();
	}

	/// <summary>Converts <c>double</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(double value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>double?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(double? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>decimal</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator decimal(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<decimal>());
	}

	/// <summary>Converts an element to <c>decimal?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator decimal?(DdnElement? value)
	{
		return value?.Get<decimal?>();
	}

	/// <summary>Converts <c>decimal</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(decimal value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>decimal?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(decimal? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <c>char</c>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator char(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<char>());
	}

	/// <summary>Converts an element to <c>char?</c>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator char?(DdnElement? value)
	{
		return value?.Get<char?>();
	}

	/// <summary>Converts <c>char</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(char value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts <c>char?</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(char? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="DateTime"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTime(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<DateTime>());
	}

	/// <summary>Converts an element to a nullable <see cref="DateTime"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTime?(DdnElement? value)
	{
		return value?.Get<DateTime?>();
	}

	/// <summary>Converts <see cref="DateTime"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTime value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="DateTime"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTime? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="DateTimeOffset"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTimeOffset(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<DateTimeOffset>());
	}

	/// <summary>Converts an element to a nullable <see cref="DateTimeOffset"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTimeOffset?(DdnElement? value)
	{
		return value?.Get<DateTimeOffset?>();
	}

	/// <summary>Converts <see cref="DateTimeOffset"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTimeOffset value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="DateTimeOffset"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTimeOffset? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="DateOnly"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateOnly(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<DateOnly>());
	}

	/// <summary>Converts an element to a nullable <see cref="DateOnly"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateOnly?(DdnElement? value)
	{
		return value?.Get<DateOnly?>();
	}

	/// <summary>Converts <see cref="DateOnly"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateOnly value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="DateOnly"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateOnly? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="TimeOnly"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator TimeOnly(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<TimeOnly>());
	}

	/// <summary>Converts an element to a nullable <see cref="TimeOnly"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator TimeOnly?(DdnElement? value)
	{
		return value?.Get<TimeOnly?>();
	}

	/// <summary>Converts <see cref="TimeOnly"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(TimeOnly value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="TimeOnly"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(TimeOnly? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="TimeSpan"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator TimeSpan(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<TimeSpan>());
	}

	/// <summary>Converts an element to a nullable <see cref="TimeSpan"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator TimeSpan?(DdnElement? value)
	{
		return value?.Get<TimeSpan?>();
	}

	/// <summary>Converts <see cref="TimeSpan"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(TimeSpan value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="TimeSpan"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(TimeSpan? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to <see cref="Guid"/>. Missing or invalid values return the default value.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator Guid(DdnElement? value)
	{
		return ((value is null) ? default : value.Get<Guid>());
	}

	/// <summary>Converts an element to a nullable <see cref="Guid"/>. Missing or invalid values return NULL.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator Guid?(DdnElement? value)
	{
		return value?.Get<Guid?>();
	}

	/// <summary>Converts <see cref="Guid"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(Guid value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a nullable <see cref="Guid"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(Guid? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an enum value to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(Enum? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to a string array; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator string[](DdnElement? value)
	{
		return (value?.Get<string?[]>() ?? [])!;
	}

	/// <summary>Converts a string array to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(string?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to a string list; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<string>(DdnElement? value)
	{
		return (value?.Get<List<string>>() ?? new());
	}

	/// <summary>Converts a string list to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<string>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>bool</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator bool[](DdnElement? value)
	{
		return (value?.Get<bool[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>bool</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator bool?[](DdnElement? value)
	{
		return (value?.Get<bool?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>bool</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<bool>(DdnElement? value)
	{
		return (value?.Get<List<bool>>() ?? new());
	}

	/// <summary>Converts an array of <c>bool</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(bool[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>bool</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(bool?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>bool</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<bool>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>int</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator int[](DdnElement? value)
	{
		return (value?.Get<int[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>int</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator int?[](DdnElement? value)
	{
		return (value?.Get<int?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>int</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<int>(DdnElement? value)
	{
		return (value?.Get<List<int>>() ?? new());
	}

	/// <summary>Converts an array of <c>int</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(int[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>int</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(int?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>int</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<int>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>long</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator long[](DdnElement? value)
	{
		return (value?.Get<long[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>long</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator long?[](DdnElement? value)
	{
		return (value?.Get<long?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>long</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<long>(DdnElement? value)
	{
		return (value?.Get<List<long>>() ?? new());
	}

	/// <summary>Converts an array of <c>long</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(long[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>long</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(long?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>long</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<long>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>short</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator short[](DdnElement? value)
	{
		return (value?.Get<short[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>short</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator short?[](DdnElement? value)
	{
		return (value?.Get<short?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>short</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<short>(DdnElement? value)
	{
		return (value?.Get<List<short>>() ?? new());
	}

	/// <summary>Converts an array of <c>short</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(short[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>short</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(short?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>short</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<short>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>byte</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator byte[](DdnElement? value)
	{
		return (value?.Get<byte[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>byte</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator byte?[](DdnElement? value)
	{
		return (value?.Get<byte?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>byte</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<byte>(DdnElement? value)
	{
		return (value?.Get<List<byte>>() ?? new());
	}

	/// <summary>Converts an array of <c>byte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(byte[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>byte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(byte?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>byte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<byte>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>uint</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator uint[](DdnElement? value)
	{
		return (value?.Get<uint[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>uint</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator uint?[](DdnElement? value)
	{
		return (value?.Get<uint?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>uint</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<uint>(DdnElement? value)
	{
		return (value?.Get<List<uint>>() ?? new());
	}

	/// <summary>Converts an array of <c>uint</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(uint[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>uint</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(uint?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>uint</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<uint>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>ulong</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ulong[](DdnElement? value)
	{
		return (value?.Get<ulong[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>ulong</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ulong?[](DdnElement? value)
	{
		return (value?.Get<ulong?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>ulong</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<ulong>(DdnElement? value)
	{
		return (value?.Get<List<ulong>>() ?? new());
	}

	/// <summary>Converts an array of <c>ulong</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ulong[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>ulong</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ulong?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>ulong</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<ulong>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>ushort</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ushort[](DdnElement? value)
	{
		return (value?.Get<ushort[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>ushort</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator ushort?[](DdnElement? value)
	{
		return (value?.Get<ushort?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>ushort</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<ushort>(DdnElement? value)
	{
		return (value?.Get<List<ushort>>() ?? new());
	}

	/// <summary>Converts an array of <c>ushort</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ushort[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>ushort</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(ushort?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>ushort</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<ushort>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>sbyte</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator sbyte[](DdnElement? value)
	{
		return (value?.Get<sbyte[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>sbyte</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator sbyte?[](DdnElement? value)
	{
		return (value?.Get<sbyte?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>sbyte</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<sbyte>(DdnElement? value)
	{
		return (value?.Get<List<sbyte>>() ?? new());
	}

	/// <summary>Converts an array of <c>sbyte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(sbyte[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>sbyte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(sbyte?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>sbyte</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<sbyte>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>float</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator float[](DdnElement? value)
	{
		return (value?.Get<float[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>float</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator float?[](DdnElement? value)
	{
		return (value?.Get<float?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>float</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<float>(DdnElement? value)
	{
		return (value?.Get<List<float>>() ?? new());
	}

	/// <summary>Converts an array of <c>float</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(float[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>float</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(float?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>float</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<float>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>double</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator double[](DdnElement? value)
	{
		return (value?.Get<double[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>double</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator double?[](DdnElement? value)
	{
		return (value?.Get<double?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>double</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<double>(DdnElement? value)
	{
		return (value?.Get<List<double>>() ?? new());
	}

	/// <summary>Converts an array of <c>double</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(double[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>double</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(double?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>double</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<double>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <c>decimal</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator decimal[](DdnElement? value)
	{
		return (value?.Get<decimal[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <c>decimal</c>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator decimal?[](DdnElement? value)
	{
		return (value?.Get<decimal?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <c>decimal</c>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<decimal>(DdnElement? value)
	{
		return (value?.Get<List<decimal>>() ?? new());
	}

	/// <summary>Converts an array of <c>decimal</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(decimal[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <c>decimal</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(decimal?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <c>decimal</c> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<decimal>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an element to an array of <see cref="DateTime"/>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTime[](DdnElement? value)
	{
		return (value?.Get<DateTime[]>() ?? []);
	}

	/// <summary>Converts an element to an array of nullable <see cref="DateTime"/>; missing values return an empty array.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DateTime?[](DdnElement? value)
	{
		return (value?.Get<DateTime?[]>() ?? []);
	}

	/// <summary>Converts an element to a list of <see cref="DateTime"/>; missing values return an empty list.</summary>
	/// <param name="value">Element.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator List<DateTime>(DdnElement? value)
	{
		return (value?.Get<List<DateTime>>() ?? new());
	}

	/// <summary>Converts an array of <see cref="DateTime"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTime[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of nullable <see cref="DateTime"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(DateTime?[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of <see cref="DateTime"/> to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<DateTime>? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts an array of enum values to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(Enum[]? value)
	{
		return _FromValue(value);
	}

	/// <summary>Converts a list of enum values to a detached value element that can be assigned to an element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the converted value.</returns>
	public static implicit operator DdnElement(List<Enum>? value)
	{
		return _FromValue(value);
	}
}
