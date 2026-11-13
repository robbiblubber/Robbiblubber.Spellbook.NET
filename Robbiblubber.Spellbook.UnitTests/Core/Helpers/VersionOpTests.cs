namespace Robbiblubber.Spellbook.UnitTests.Core.Helpers;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests the version and stream methods.</summary>
[TestFixture]
public class VersionOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: VersionOp                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Versions are parsed; a single number is a major version.</summary>
	[Test]
	public void ToVersion()
	{
		Assert.That(VersionOp.ToVersion("1.2.3"), Is.EqualTo(new Version(1, 2, 3)));
		Assert.That(VersionOp.ToVersion(" 2 "), Is.EqualTo(new Version(2, 0)));
		Assert.That(VersionOp.ToVersion("abc"), Is.EqualTo(new Version(1, 0, 0)));
		Assert.That(VersionOp.ToVersion(null, "3"), Is.EqualTo(new Version(3, 0)));
		Assert.That(VersionOp.ToVersion("x", new Version(4, 1)), Is.EqualTo(new Version(4, 1)));
		Assert.Throws<ArgumentException>(() => VersionOp.ToVersion("abc", "x"));

		Assert.That(VersionOp.IsVersion("1.0"), Is.True);
		Assert.That(VersionOp.IsVersion("1"), Is.True);
		Assert.That(VersionOp.IsVersion("1.x"), Is.False);
		Assert.That(VersionOp.IsVersion(null), Is.False);
	}


	/// <summary>Version strings are as short as possible.</summary>
	[Test]
	public void ToVersionString()
	{
		Assert.That(new Version(1, 2).ToVersionString(), Is.EqualTo("1.2"));
		Assert.That(new Version(1, 2, 0).ToVersionString(), Is.EqualTo("1.2.0"));
		Assert.That(new Version(1, 2, 3, 0).ToVersionString(), Is.EqualTo("1.2.3"));
		Assert.That(new Version(1, 2, 3, 4).ToVersionString(), Is.EqualTo("1.2.3.4"));
		Assert.That(((Version?) null).ToVersionString(), Is.EqualTo("0.0"));
	}


	/// <summary>Assembly versions are read.</summary>
	[Test]
	public void VersionOf()
	{
		Version v = typeof(StringOp).Assembly.GetName().Version!;

		Assert.That(VersionOp.VersionOf(typeof(StringOp)), Is.EqualTo(v));
		Assert.That(VersionOp.VersionOf(new StringRdr("x")), Is.EqualTo(v));
		Assert.That(VersionOp.ApplicationVersion, Is.Not.Null);
		Assert.That(VersionOp.ApplicationCopyright, Is.Not.Null);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: StreamOp                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Bytes are read from the current position to the end.</summary>
	[Test]
	public async Task ReadAllBytes()
	{
		byte[] data = Encoding.UTF8.GetBytes("Hello Spellbook");
		MemoryStream s = new(data);
		s.Position = 6;

		Assert.That(s.ReadAllBytes(), Is.EqualTo(data[6..]));
		Assert.That(s.ReadAllBytes(), Is.Empty);

		s.Position = 0;
		Assert.That(await s.ReadAllBytesAsync(), Is.EqualTo(data));
	}
}
