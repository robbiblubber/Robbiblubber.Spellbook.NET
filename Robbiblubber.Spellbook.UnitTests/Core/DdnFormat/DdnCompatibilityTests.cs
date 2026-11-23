namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests compatibility with files written by earlier versions and usage patterns of other libraries.</summary>
[TestFixture]
public class DdnCompatibilityTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Files written by version 3.1 are read.</summary>
	[Test]
	public void Read_Version31File()
	{
		DdnElement ddn = Ddn.Create("""
			state = 2;
			when = 2024-05-06 07:08:09.010+02:00;
			empty = \0;
			path = c:\\temp\/x;

			layout
			{
				main
				{
					pos = 10, 20;
					size = 800, 600;
					state = 2;
				}
			}
			""");

		Assert.That(ddn.Get<FileAccess>("state"), Is.EqualTo(FileAccess.Write));
		Assert.That(ddn.Get<DateTime>("when").ToUniversalTime(), Is.EqualTo(new DateTime(2024, 5, 6, 5, 8, 9, 10)));
		Assert.That(ddn["empty"].IsNull, Is.True);
		Assert.That(ddn.Get<string>("path"), Is.EqualTo(@"c:\temp/x"));
		Assert.That(ddn["layout"]["main"].Get("pos", Array.Empty<int>()), Is.EqualTo(new[] { 10, 20 }));
	}


	/// <summary>Usage pattern of the recent list store in Spellbook Controls.</summary>
	[Test]
	public void Pattern_RecentListStore()
	{
		DdnElement cfg = Ddn.Create();
		string section = "recent/files/";

		for(int i = 0; i < 2; i++)
		{
			string entry = section + "[" + i.ToString() + "]/";
			cfg[entry + "key"] = "file" + i;
			cfg[entry + "pinned"] = ((i == 0) ? 1 : 0);
		}
		cfg[section + "length"] = 2;

		DdnElement read = Ddn.Create(cfg.ToString());
		int length = read[section + "length"];
		string? key = read[section + "[1]/key"];
		int pinned = read[section + "[0]/pinned"];
		int format = read[section + "format"];

		Assert.That(length, Is.EqualTo(2));
		Assert.That(key, Is.EqualTo("file1"));
		Assert.That(pinned, Is.EqualTo(1));
		Assert.That(format, Is.EqualTo(0));
	}


	/// <summary>Usage pattern of the window layout store in Spellbook Controls.</summary>
	[Test]
	public void Pattern_WindowLayoutStore()
	{
		DdnElement cfg = Ddn.Create();

		cfg["layout"]["main"]["window"] = new[] { 1, 2, 3, 4, 0, 2 };
		cfg["layout"]["main"]["split"] = new[] { 120 };
		cfg["layout"]["other"]["window"] = new[] { 5 };

		DdnElement section = Ddn.Create(cfg.ToString())["layout"]["main"];
		Assert.That(section.Get("window", Array.Empty<int>()), Is.EqualTo(new[] { 1, 2, 3, 4, 0, 2 }));
		Assert.That(section.Get("missing", Array.Empty<int>()), Is.Empty);
		Assert.That(cfg["layout"].ElementCount, Is.EqualTo(2));
	}


	/// <summary>Enum values written by earlier versions as numbers are read; new values are written as names.</summary>
	[Test]
	public void Enums_NumbersAndNames()
	{
		DdnElement cfg = Ddn.Create("state = 2;");
		cfg["state2"] = FileAccess.Write;

		Assert.That(cfg.Get("state", FileAccess.Read), Is.EqualTo(FileAccess.Write));
		Assert.That(cfg.ToString(), Does.Contain("state2 = Write;"));
	}
}
