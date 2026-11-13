namespace Robbiblubber.Spellbook.UnitTests.Core.Helpers;

using NUnit.Framework;



/// <summary>This class tests the date and time methods.</summary>
[TestFixture]
public class CalendarOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: timestamps                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Timestamps with time zone are read correctly.</summary>
	/// <param name="s">Timestamp.</param>
	/// <param name="utc">Expected UTC time (round-trip format).</param>
	[TestCase("2024-05-01 12:00:00.000+02:00", "2024-05-01T10:00:00.0000000Z")]
	[TestCase("2024-05-01 12:00:00-02:00", "2024-05-01T14:00:00.0000000Z")]
	[TestCase("2024-05-01T12:00:00Z", "2024-05-01T12:00:00.0000000Z")]
	[TestCase("2024-05-01T12:00:00.123Z", "2024-05-01T12:00:00.1230000Z")]
	[TestCase("2024-05-01 12:00:00.1234567+00:00", "2024-05-01T12:00:00.1234567Z")]
	[TestCase("2024-05-01 12:00:00.5+0530", "2024-05-01T06:30:00.5000000Z")]
	[TestCase("2024-05-01 12:00+01", "2024-05-01T11:00:00.0000000Z")]
	[TestCase(" 2024-5-1 8:05:09 Z ", "2024-05-01T08:05:09.0000000Z")]
	public void ParseTimestamp_Zoned(string s, string utc)
	{
		Assert.That(s.ParseTimestamp(true).ToString("o"), Is.EqualTo(utc));
		Assert.That(s.ParseTimestamp(true).Kind, Is.EqualTo(DateTimeKind.Utc));
		Assert.That(s.ParseTimestamp().Kind, Is.EqualTo(DateTimeKind.Local));
		Assert.That(s.ParseTimestamp().ToUniversalTime().ToString("o"), Is.EqualTo(utc));

		Assert.That(s.TryParseTimestamp(out DateTimeOffset o), Is.True);
		Assert.That(o.UtcDateTime.ToString("o"), Is.EqualTo(utc));
	}


	/// <summary>Timestamps without time zone and dates are local times.</summary>
	[Test]
	public void ParseTimestamp_Local()
	{
		DateTime t = "2024-05-01 12:00:00".ParseTimestamp();
		Assert.That(t, Is.EqualTo(new DateTime(2024, 5, 1, 12, 0, 0)));
		Assert.That(t.Kind, Is.EqualTo(DateTimeKind.Local));

		DateTime d = "2024-05-01".ParseTimestamp();
		Assert.That(d, Is.EqualTo(new DateTime(2024, 5, 1)));
		Assert.That("2024-05-01".ParseTimestamp(true), Is.EqualTo(new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime()));

		Assert.That("2024-05-01 12:00".TryParseTimestamp(out DateTimeOffset o), Is.True);
		Assert.That(o.Offset, Is.EqualTo(TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 5, 1, 12, 0, 0))));
	}


	/// <summary>Invalid timestamps fail instead of silently returning 1970.</summary>
	/// <param name="s">String.</param>
	[TestCase("hello")]
	[TestCase("")]
	[TestCase("2024-13-01")]
	[TestCase("2024-02-30 12:00")]
	[TestCase("2024-05-01 25:00")]
	[TestCase("2024-05-01 12:00:00+")]
	public void ParseTimestamp_Invalid(string s)
	{
		Assert.That(s.TryParseTimestamp(out DateTime _), Is.False);
		Assert.Throws<FormatException>(() => s.ParseTimestamp());

		DateTime def = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		Assert.That(s.ParseTimestamp(def, true), Is.EqualTo(def));
		Assert.That(s.ParseTimestamp(def), Is.EqualTo(def.ToLocalTime()));
		Assert.That(((string?) null).TryParseTimestamp(out DateTimeOffset _), Is.False);
	}


	/// <summary>Timestamps are written in the previous format and read back.</summary>
	[Test]
	public void ToTimestamp_RoundTrip()
	{
		Assert.That(new DateTime(2024, 5, 1, 12, 0, 0, 500, DateTimeKind.Utc).ToTimestamp(), Is.EqualTo("2024-05-01 12:00:00.500+00:00"));
		Assert.That(new DateTimeOffset(2024, 5, 1, 12, 0, 0, 7, TimeSpan.FromMinutes(-210)).ToTimestamp(), Is.EqualTo("2024-05-01 12:00:00.007-03:30"));

		DateTime local = new(2024, 5, 1, 12, 0, 0, 500, DateTimeKind.Local);
		TimeSpan off = TimeZoneInfo.Local.GetUtcOffset(local);
		string s = local.ToTimestamp();

		Assert.That(s, Does.StartWith("2024-05-01 12:00:00.500" + ((off < TimeSpan.Zero) ? "-" : "+")));
		Assert.That(s.ParseTimestamp(), Is.EqualTo(local));
	}


	/// <summary>Compact dates are parsed; other lengths throw.</summary>
	[Test]
	public void ParseYYYYMMDD()
	{
		Assert.That("20240501".ParseYYYYMMDD(), Is.EqualTo(new DateTime(2024, 5, 1)));
		Assert.That("20240501123456".ParseYYYYMMDD(), Is.EqualTo(new DateTime(2024, 5, 1, 12, 34, 56)));
		Assert.Throws<FormatException>(() => "2024050112".ParseYYYYMMDD());
		Assert.Throws<FormatException>(() => "20241301".ParseYYYYMMDD());
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: Unix time                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Unix timestamps work after 2038 and cut off fractions.</summary>
	[Test]
	public void UnixTimestamp()
	{
		Assert.That(new DateTime(2040, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp(), Is.EqualTo(2208988800L));
		Assert.That(new DateTime(1970, 1, 1, 0, 0, 1, 600, DateTimeKind.Utc).ToUnixTimestamp(), Is.EqualTo(1L));
		Assert.That(new DateTime(1969, 12, 31, 23, 59, 59, 500, DateTimeKind.Utc).ToUnixTimestamp(), Is.EqualTo(-1L));
		Assert.That(new DateTime(1970, 1, 1, 0, 0, 1, 600, DateTimeKind.Utc).ToUnixTimestampMilliseconds(), Is.EqualTo(1600L));

		Assert.That(2208988800L.FromUnixTimestamp(), Is.EqualTo(new DateTime(2040, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()));
		Assert.That(2208988800L.FromUnixTimestamp().Kind, Is.EqualTo(DateTimeKind.Local));
		Assert.That(86400.FromUnixTimestamp().ToUniversalTime(), Is.EqualTo(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc)));
		Assert.That(1600L.FromUnixTimestampMilliseconds().ToUniversalTime(), Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 1, 600, DateTimeKind.Utc)));

		DateTime now = DateTime.Now;
		Assert.That(now.ToUnixTimestamp().FromUnixTimestamp(), Is.EqualTo(now.AddTicks(-(now.Ticks % TimeSpan.TicksPerSecond))));
	}
}
