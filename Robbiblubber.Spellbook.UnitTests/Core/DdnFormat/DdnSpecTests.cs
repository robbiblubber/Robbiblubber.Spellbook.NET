namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests the samples of the ddn specification 3.1.</summary>
[TestFixture]
public class DdnSpecTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sample from the specification.</summary>
	private const string _SAMPLE = @"
type = T;                                   // value
time = 2022-11-19 05:33:20+00:00;
winpath = \\\\svr1\\folder;
sec1                                        // section
{
	type = S;
	fab = Cotton;
	n = 20;
	sub1                                    // subsection
	{
		vals = 1.2, 1.4, 1.3;
	}
	sub2
	{
		vals = 2.3, 2.6, 1.7;
	}
}
sec2
{
	/* a multi-line comment in a single line */
	type = X;
	unused = true;
}";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The specification sample is read as documented.</summary>
	[Test]
	public void Sample()
	{
		DdnElement ddn = Ddn.Create(_SAMPLE);

		Assert.That(ddn["type"].Value, Is.EqualTo("T"));
		Assert.That(ddn.Get<DateTimeOffset>("time"), Is.EqualTo(new DateTimeOffset(2022, 11, 19, 5, 33, 20, TimeSpan.Zero)));
		Assert.That(ddn["winpath"].Value, Is.EqualTo(@"\\svr1\folder"));
		Assert.That(ddn.Get<int>("sec1/n"), Is.EqualTo(20));
		Assert.That(ddn.Get<double[]>("/sec1/sub2/vals"), Is.EqualTo(new[] { 2.3, 2.6, 1.7 }));
		Assert.That(ddn["sec1"]["../sec2/type"].Value, Is.EqualTo("X"));
		Assert.That(ddn.Get<bool>("sec2/unused"), Is.True);
		Assert.That(ddn.ElementCount, Is.EqualTo(5));
	}


	/// <summary>Slashes inside values do not start comments.</summary>
	[Test]
	public void Comments_OnlyAfterSeparators()
	{
		DdnElement ddn = Ddn.Create("url = http://robbiblubber.org; a = 1;// c\nb = 2;/* c */c = 3;");

		Assert.That(ddn["url"].Value, Is.EqualTo("http://robbiblubber.org"));
		Assert.That(ddn.Get<int>("a") + ddn.Get<int>("b") + ddn.Get<int>("c"), Is.EqualTo(6));
	}


	/// <summary>Lenient reading: missing semicolons, values without equals sign, duplicates.</summary>
	[Test]
	public void LenientReading()
	{
		DdnElement ddn = Ddn.Create("n; s { a = 1 } d = 1; d = 2; q { a = 1; } q { b = 2; } e = f");

		Assert.That(ddn["n"].Exists, Is.True);
		Assert.That(ddn["n"].IsNull, Is.True);
		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("d"), Is.EqualTo(2));
		Assert.That(ddn["q"].ElementCount, Is.EqualTo(2));
		Assert.That(ddn["e"].Value, Is.EqualTo("f"));
	}


	/// <summary>Data representations of version 3.1 are read.</summary>
	[Test]
	public void Representations()
	{
		DdnElement ddn = Ddn.Create(@"sep = \,; sp = \_; av = 6.02214076E+23; b1 = yes; b2 = OFF; day = friday; dn = 5;
									  fa = ReadOnly\, Hidden; z = 2026-10-19T10:00Z; o = 2026-10-19 10:00+0200; due = 2026-10-19;
									  start = 08:30:00; timeout = 1.12:00:00; arr = Peter, 2, \0, Mary;");

		Assert.That(ddn.Get<char>("sep"), Is.EqualTo(','));
		Assert.That(ddn.Get<char>("sp"), Is.EqualTo(' '));
		Assert.That(ddn.Get<double>("av"), Is.EqualTo(6.02214076E+23));
		Assert.That(ddn.Get<bool>("b1"), Is.True);
		Assert.That(ddn.Get<bool>("b2"), Is.False);
		Assert.That(ddn.Get<DayOfWeek>("day"), Is.EqualTo(DayOfWeek.Friday));
		Assert.That(ddn.Get<DayOfWeek>("dn"), Is.EqualTo(DayOfWeek.Friday));
		Assert.That(ddn.Get<FileAttributes>("fa"), Is.EqualTo(FileAttributes.ReadOnly | FileAttributes.Hidden));
		Assert.That(ddn.Get<DateTimeOffset>("z"), Is.EqualTo(new DateTimeOffset(2026, 10, 19, 10, 0, 0, TimeSpan.Zero)));
		Assert.That(ddn.Get<DateTimeOffset>("o").Offset, Is.EqualTo(TimeSpan.FromHours(2)));
		Assert.That(ddn.Get<DateTime>("due"), Is.EqualTo(new DateTime(2026, 10, 19)));
		Assert.That(ddn.Get<DateTime>("due").Kind, Is.EqualTo(DateTimeKind.Local));
		Assert.That(ddn.Get<TimeOnly>("start"), Is.EqualTo(new TimeOnly(8, 30)));
		Assert.That(ddn.Get<TimeSpan>("timeout"), Is.EqualTo(TimeSpan.FromHours(36)));
		Assert.That(ddn.Get<string?[]>("arr"), Is.EqualTo(new string?[] { "Peter", "2", null, "Mary" }));
	}


	/// <summary>Flags are written as a masked list of names.</summary>
	[Test]
	public void Flags_Written()
	{
		DdnElement ddn = Ddn.Create();
		ddn["fa"] = (FileAttributes.Hidden | FileAttributes.ReadOnly);

		Assert.That(ddn.ToString(), Does.Contain(@"fa = ReadOnly\, Hidden;"));
	}
}
