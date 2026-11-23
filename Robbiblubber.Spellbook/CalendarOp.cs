namespace Robbiblubber.Spellbook;

using System.Globalization;
using System.Text.RegularExpressions;



/// <summary>This class provides date and time extension methods.</summary>
/// <remarks>Timestamps are written as <c>yyyy-MM-dd HH:mm:ss.fff+hh:mm</c>. When reading, the date may stand alone, the time may
///          be separated by a space or 'T', seconds may have any number of decimal places, and the time zone may be 'Z',
///          <c>±hh:mm</c>, <c>±hhmm</c> or <c>±hh</c>. Timestamps without time zone are local times.</remarks>
public static class CalendarOp
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Timestamp expression.</summary>
	private static readonly Regex _Timestamp = new(@"^(\d{1,4})-(\d{1,2})-(\d{1,2})(?:[ T]+(\d{1,2})(?::(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?)?)?" +
												   @"\s*(?:(Z)|([+-])(\d{1,2})(?::?(\d{2}))?)?$", RegexOptions.CultureInvariant);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses a timestamp.</summary>
	/// <param name="s">String.</param>
	/// <param name="value">Timestamp; without time zone the local time zone is applied.</param>
	/// <param name="zoned">Returns if the string contains a time zone.</param>
	/// <returns>Returns TRUE if the string is a valid timestamp, otherwise returns FALSE.</returns>
	internal static bool _TryParse(string? s, out DateTimeOffset value, out bool zoned)
	{
		value = default;
		zoned = false;
		if(s is null) { return false; }

		Match m = _Timestamp.Match(s.Trim());
		if(!m.Success) { return false; }

		try
		{
			int _Int(int group) { return (m.Groups[group].Success ? int.Parse(m.Groups[group].Value, CultureInfo.InvariantCulture) : 0); }

			DateTime t = new(_Int(1), _Int(2), _Int(3), _Int(4), _Int(5), _Int(6), DateTimeKind.Unspecified);
			if(m.Groups[7].Success)
			{
				string fraction = m.Groups[7].Value.PadRight(7, '0')[..7];
				t = t.AddTicks(long.Parse(fraction, CultureInfo.InvariantCulture));
			}

			if(m.Groups[8].Success)
			{
				zoned = true;
				value = new(t, TimeSpan.Zero);
			}
			else if(m.Groups[9].Success)
			{
				zoned = true;
				TimeSpan offset = new(_Int(10), _Int(11), 0);
				value = new(t, ((m.Groups[9].Value == "-") ? offset.Negate() : offset));
			}
			else { value = new(t, TimeZoneInfo.Local.GetUtcOffset(t)); }

			return true;
		}
		catch(ArgumentException) { return false; }
	}


	/// <summary>Formats a time offset.</summary>
	/// <param name="offset">Offset.</param>
	/// <returns>Returns the offset string.</returns>
	private static string _FormatOffset(TimeSpan offset)
	{
		return (((offset < TimeSpan.Zero) ? "-" : "+") + offset.Duration().ToString(@"hh\:mm", CultureInfo.InvariantCulture));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: timestamps                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a date and time to a timestamp string.</summary>
	/// <param name="value">Date and time; unspecified times are local times.</param>
	/// <returns>Returns the timestamp string, e.g. <c>2024-05-01 12:00:00.500+02:00</c>.</returns>
	public static string ToTimestamp(this DateTime value)
	{
		TimeSpan offset = ((value.Kind == DateTimeKind.Utc) ? TimeSpan.Zero : TimeZoneInfo.Local.GetUtcOffset(value));
		return (value.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + _FormatOffset(offset));
	}


	/// <summary>Converts a date and time to a timestamp string.</summary>
	/// <param name="value">Date and time.</param>
	/// <returns>Returns the timestamp string, e.g. <c>2024-05-01 12:00:00.500+02:00</c>.</returns>
	public static string ToTimestamp(this DateTimeOffset value)
	{
		return (value.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + _FormatOffset(value.Offset));
	}


	/// <summary>Parses a timestamp string.</summary>
	/// <param name="s">String.</param>
	/// <param name="value">Date and time with offset; without time zone the local offset is applied.</param>
	/// <returns>Returns TRUE if the string is a valid timestamp, otherwise returns FALSE.</returns>
	public static bool TryParseTimestamp(this string? s, out DateTimeOffset value)
	{
		return _TryParse(s, out value, out _);
	}


	/// <summary>Parses a timestamp string.</summary>
	/// <param name="s">String.</param>
	/// <param name="value">Date and time as local time, or as UTC.</param>
	/// <param name="utc">Determines if the result is UTC.</param>
	/// <returns>Returns TRUE if the string is a valid timestamp, otherwise returns FALSE.</returns>
	public static bool TryParseTimestamp(this string? s, out DateTime value, bool utc = false)
	{
		value = default;
		if(!_TryParse(s, out DateTimeOffset v, out bool zoned)) { return false; }

		if(utc) { value = v.UtcDateTime; }
		else { value = (zoned ? v.LocalDateTime : DateTime.SpecifyKind(v.DateTime, DateTimeKind.Local)); }

		return true;
	}


	/// <summary>Parses a timestamp string.</summary>
	/// <param name="s">String.</param>
	/// <param name="utc">Determines if the result is UTC.</param>
	/// <returns>Returns the date and time as local time, or as UTC.</returns>
	/// <exception cref="FormatException">Thrown when the string is not a valid timestamp.</exception>
	public static DateTime ParseTimestamp(this string s, bool utc = false)
	{
		if(!TryParseTimestamp(s, out DateTime rval, utc)) { throw new FormatException("Invalid timestamp \"" + s + "\"."); }

		return rval;
	}


	/// <summary>Parses a timestamp string.</summary>
	/// <param name="s">String.</param>
	/// <param name="defaultValue">Value returned if the string is not a valid timestamp.</param>
	/// <param name="utc">Determines if the result is UTC.</param>
	/// <returns>Returns the date and time as local time, or as UTC.</returns>
	public static DateTime ParseTimestamp(this string? s, DateTime defaultValue, bool utc = false)
	{
		if(TryParseTimestamp(s, out DateTime rval, utc)) { return rval; }

		return (utc ? defaultValue.ToUniversalTime() : defaultValue.ToLocalTime());
	}


	/// <summary>Parses a string formatted <c>yyyyMMdd</c> or <c>yyyyMMddHHmmss</c>.</summary>
	/// <param name="s">String.</param>
	/// <returns>Returns the date and time.</returns>
	/// <exception cref="FormatException">Thrown when the string does not have one of the formats.</exception>
	public static DateTime ParseYYYYMMDD(this string s)
	{
		return DateTime.ParseExact(s.Trim(), ["yyyyMMdd", "yyyyMMddHHmmss"], CultureInfo.InvariantCulture, DateTimeStyles.None);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extension methods: Unix time                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a Unix timestamp (seconds since 1970-01-01 UTC) to a local date and time.</summary>
	/// <param name="u">Unix timestamp.</param>
	/// <returns>Returns the local date and time.</returns>
	public static DateTime FromUnixTimestamp(this long u)
	{
		return DateTimeOffset.FromUnixTimeSeconds(u).LocalDateTime;
	}


	/// <summary>Converts a Unix timestamp (seconds since 1970-01-01 UTC) to a local date and time.</summary>
	/// <param name="u">Unix timestamp.</param>
	/// <returns>Returns the local date and time.</returns>
	public static DateTime FromUnixTimestamp(this int u)
	{
		return FromUnixTimestamp((long) u);
	}


	/// <summary>Converts a Unix timestamp in milliseconds to a local date and time.</summary>
	/// <param name="u">Unix timestamp in milliseconds.</param>
	/// <returns>Returns the local date and time.</returns>
	public static DateTime FromUnixTimestampMilliseconds(this long u)
	{
		return DateTimeOffset.FromUnixTimeMilliseconds(u).LocalDateTime;
	}


	/// <summary>Converts a date and time to a Unix timestamp (seconds since 1970-01-01 UTC).</summary>
	/// <param name="d">Date and time; unspecified times are local times.</param>
	/// <returns>Returns the Unix timestamp; fractions of a second are cut off.</returns>
	public static long ToUnixTimestamp(this DateTime d)
	{
		return new DateTimeOffset(d.ToUniversalTime()).ToUnixTimeSeconds();
	}


	/// <summary>Converts a date and time to a Unix timestamp in milliseconds.</summary>
	/// <param name="d">Date and time; unspecified times are local times.</param>
	/// <returns>Returns the Unix timestamp in milliseconds; fractions are cut off.</returns>
	public static long ToUnixTimestampMilliseconds(this DateTime d)
	{
		return new DateTimeOffset(d.ToUniversalTime()).ToUnixTimeMilliseconds();
	}
}
