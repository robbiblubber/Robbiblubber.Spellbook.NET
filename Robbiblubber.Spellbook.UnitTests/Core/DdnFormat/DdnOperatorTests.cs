namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests the conversion operators of ddn elements.</summary>
[TestFixture]
public class DdnOperatorTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Scalar operators.</summary>
	[Test]
	public void Scalars()
	{
		DdnElement ddn = Ddn.Create();
		Guid g = Guid.NewGuid();
		DateTime now = new(2026, 10, 19, 9, 0, 0, 250, DateTimeKind.Local);

		ddn["s"] = "text";
		ddn["b"] = true;
		ddn["i"] = 1;
		ddn["l"] = 2L;
		ddn["sh"] = (short) 3;
		ddn["by"] = (byte) 4;
		ddn["ui"] = 5u;
		ddn["ul"] = 6ul;
		ddn["us"] = (ushort) 7;
		ddn["sb"] = (sbyte) -8;
		ddn["f"] = 9.5f;
		ddn["d"] = 10.5;
		ddn["m"] = 11.5m;
		ddn["c"] = 'c';
		ddn["dt"] = now;
		ddn["dto"] = new DateTimeOffset(now);
		ddn["do"] = new DateOnly(2026, 10, 19);
		ddn["to"] = new TimeOnly(9, 30);
		ddn["ts"] = TimeSpan.FromMinutes(90);
		ddn["g"] = g;
		ddn["e"] = DayOfWeek.Sunday;

		DdnElement r = Ddn.Create(ddn.ToString());
		string s = r["s"]; bool b = r["b"]; int i = r["i"]; long l = r["l"]; short sh = r["sh"]; byte by = r["by"];
		uint ui = r["ui"]; ulong ul = r["ul"]; ushort us = r["us"]; sbyte sb = r["sb"]; float f = r["f"]; double d = r["d"];
		decimal m = r["m"]; char c = r["c"]; DateTime dt = r["dt"]; DateTimeOffset dto = r["dto"]; DateOnly dd = r["do"];
		TimeOnly to = r["to"]; TimeSpan ts = r["ts"]; Guid gg = r["g"];

		Assert.That(s, Is.EqualTo("text"));
		Assert.That(b, Is.True);
		Assert.That(new long[] { i, l, sh, by, ui, (long) ul, us, sb }, Is.EqualTo(new long[] { 1, 2, 3, 4, 5, 6, 7, -8 }));
		Assert.That(new double[] { f, d, (double) m }, Is.EqualTo(new[] { 9.5, 10.5, 11.5 }));
		Assert.That(c, Is.EqualTo('c'));
		Assert.That(dt, Is.EqualTo(now));
		Assert.That(dto, Is.EqualTo(new DateTimeOffset(now)));
		Assert.That(dd, Is.EqualTo(new DateOnly(2026, 10, 19)));
		Assert.That(to, Is.EqualTo(new TimeOnly(9, 30)));
		Assert.That(ts, Is.EqualTo(TimeSpan.FromMinutes(90)));
		Assert.That(gg, Is.EqualTo(g));
		Assert.That(r.Get<DayOfWeek>("e"), Is.EqualTo(DayOfWeek.Sunday));
	}


	/// <summary>Nullable operators return NULL for NULL and missing values.</summary>
	[Test]
	public void Nullables()
	{
		DdnElement ddn = Ddn.Create(@"n = \0; v = 5; t = true;");

		int? a = ddn["n"]; int? b = ddn["missing"]; int? c = ddn["v"];
		bool? d = ddn["n"]; bool? e = ddn["t"];
		DateTime? f = ddn["n"]; double? g = ddn["v"];
		string s = ddn["n"];

		Assert.That(a, Is.Null);
		Assert.That(b, Is.Null);
		Assert.That(c, Is.EqualTo(5));
		Assert.That(d, Is.Null);
		Assert.That(e, Is.True);
		Assert.That(f, Is.Null);
		Assert.That(g, Is.EqualTo(5.0));
		Assert.That(s, Is.EqualTo(string.Empty));

		ddn["x"] = (int?) null;
		ddn["y"] = (int?) 3;
		ddn["z"] = (string?) null;
		ddn["w"] = null;
		Assert.That(ddn["x"].IsNull, Is.True);
		Assert.That(ddn.Get<int>("y"), Is.EqualTo(3));
		Assert.That(ddn["z"].IsNull, Is.True);
		Assert.That(ddn["w"].IsNull, Is.True);
	}


	/// <summary>Array and list operators.</summary>
	[Test]
	public void ArraysAndLists()
	{
		DdnElement ddn = Ddn.Create();
		ddn["s"] = new[] { "a", "b" };
		ddn["sl"] = new List<string> { "c", "d" };
		ddn["i"] = new[] { 1, 2 };
		ddn["in"] = new int?[] { null, 2 };
		ddn["il"] = new List<int> { 3, 4 };
		ddn["l"] = new[] { 5L, 6L };
		ddn["ln"] = new long?[] { 7L, null };
		ddn["b"] = new[] { true, false };
		ddn["dt"] = new[] { new DateTime(2020, 1, 1) };
		ddn["d"] = new List<double> { 1.5 };

		DdnElement r = Ddn.Create(ddn.ToString());
		string[] s = r["s"]; List<string> sl = r["sl"]; int[] i = r["i"]; int?[] inn = r["in"]; List<int> il = r["il"];
		long[] l = r["l"]; long?[] ln = r["ln"]; bool[] b = r["b"]; DateTime[] dt = r["dt"]; List<double> d = r["d"];

		Assert.That(s, Is.EqualTo(new[] { "a", "b" }));
		Assert.That(sl, Is.EqualTo(new[] { "c", "d" }));
		Assert.That(i, Is.EqualTo(new[] { 1, 2 }));
		Assert.That(inn, Is.EqualTo(new int?[] { null, 2 }));
		Assert.That(il, Is.EqualTo(new[] { 3, 4 }));
		Assert.That(l, Is.EqualTo(new[] { 5L, 6L }));
		Assert.That(ln, Is.EqualTo(new long?[] { 7L, null }));
		Assert.That(b, Is.EqualTo(new[] { true, false }));
		Assert.That(dt, Is.EqualTo(new[] { new DateTime(2020, 1, 1) }));
		Assert.That(d, Is.EqualTo(new[] { 1.5 }));
	}


	/// <summary>Array and list operators return empty collections for missing values.</summary>
	[Test]
	public void ArraysAndLists_Missing()
	{
		DdnElement ddn = Ddn.Create();
		int[] a = ddn["x"]; string[] b = ddn["x"]; List<int> c = ddn["x"]; List<string> d = ddn["x"]; byte?[] e = ddn["x"];

		Assert.That(a, Is.Empty);
		Assert.That(b, Is.Empty);
		Assert.That(c, Is.Empty);
		Assert.That(d, Is.Empty);
		Assert.That(e, Is.Empty);
	}


	/// <summary>Operators on a NULL element reference.</summary>
	[Test]
	public void NullReference()
	{
		DdnElement? elm = null;
		int a = elm; int? b = elm; string c = elm; int[] d = elm;

		Assert.That(a, Is.EqualTo(0));
		Assert.That(b, Is.Null);
		Assert.That(c, Is.Empty);
		Assert.That(d, Is.Empty);
	}


	/// <summary>The Boolean operator works in conditions.</summary>
	[Test]
	public void BooleanCondition()
	{
		DdnElement ddn = Ddn.Create("on = true; off = false;");

		if(ddn["on"]) { Assert.Pass(); }
		Assert.Fail();
	}
}
