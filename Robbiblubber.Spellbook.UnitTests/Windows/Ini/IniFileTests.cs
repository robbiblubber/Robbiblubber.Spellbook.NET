namespace Robbiblubber.Spellbook.UnitTests.Windows.Ini;

using System.Runtime.Versioning;
using System.Text;

using NUnit.Framework;
using Robbiblubber.Spellbook.Windows;



/// <summary>This class tests ini files.</summary>
[TestFixture][SupportedOSPlatform("windows")]
public class IniFileTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Directory = "";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a file name in the test directory and optionally writes content.</summary>
	/// <param name="name">File name.</param>
	/// <param name="content">Content, or NULL to leave the file alone.</param>
	/// <param name="encoding">Encoding, or NULL for UTF-8 without byte order mark.</param>
	/// <returns>Returns the full file name.</returns>
	private string _File(string name, string? content = null, Encoding? encoding = null)
	{
		string rval = Path.Combine(_Directory, name);
		if(content is not null) { File.WriteAllText(rval, content, (encoding ?? new UTF8Encoding(false))); }

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the test directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Directory = Path.Combine(Path.GetTempPath(), "Robbiblubber.Spellbook.UnitTests.Windows", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Directory);
	}


	/// <summary>Deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		try { Directory.Delete(_Directory, true); } catch(IOException) {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Values are read with the rules of the Windows profile functions.</summary>
	[Test]
	public void ReadSyntax()
	{
		IniFile ini = new(_File("a.ini", "; comment\r\n[General]\r\n  Name =  Spellbook  \r\nquoted=\" padded \"\r\nsingle='x'\r\n" +
										 "# other comment\r\nnoequals\r\nname=second\r\n[ Other ]\r\nempty=\r\n"));

		Assert.That(ini.GetString("general", "NAME"), Is.EqualTo("Spellbook"));
		Assert.That(ini.GetString("General", "quoted"), Is.EqualTo(" padded "));
		Assert.That(ini.GetString("General", "single"), Is.EqualTo("x"));
		Assert.That(ini.GetString("Other", "empty", "default"), Is.EqualTo(""));
		Assert.That(ini.GetString("Other", "missing", "default"), Is.EqualTo("default"));
		Assert.That(ini.GetString("Missing", "name"), Is.EqualTo(""));
		Assert.That(ini.GetSections(), Is.EqualTo(new[] { "General", "Other" }));
		Assert.That(ini.GetKeys("General"), Is.EqualTo(new[] { "Name", "quoted", "single" }));
		Assert.That(ini.ContainsKey("General", "noequals"), Is.False);
		Assert.That(ini.ContainsSection("other"), Is.True);
	}


	/// <summary>Typed values are read with invariant culture; invalid values return the default.</summary>
	[Test]
	public void ReadTyped()
	{
		IniFile ini = new(_File("a.ini", "[v]\r\ni=42\r\nneg=-7\r\nbig=9000000000\r\nd=3.25\r\nbad=12abc\r\nt=yes\r\nf=Off\r\n" +
										 "one=1\r\nzero=0\r\nb64=AQID\r\nnob64=%%%\r\n"));

		Assert.That(ini.GetInt32("v", "i"), Is.EqualTo(42));
		Assert.That(ini.GetInt32("v", "neg"), Is.EqualTo(-7));
		Assert.That(ini.GetInt32("v", "bad", 5), Is.EqualTo(5));
		Assert.That(ini.GetInt32("v", "big", 5), Is.EqualTo(5));
		Assert.That(ini.GetInt64("v", "big"), Is.EqualTo(9000000000L));
		Assert.That(ini.GetInt64("v", "bad", 3), Is.EqualTo(3));
		Assert.That(ini.GetInt16("v", "i"), Is.EqualTo(42));
		Assert.That(ini.GetInt16("v", "big", 2), Is.EqualTo(2));
		Assert.That(ini.GetDouble("v", "d"), Is.EqualTo(3.25));
		Assert.That(ini.GetBoolean("v", "t"), Is.True);
		Assert.That(ini.GetBoolean("v", "f", true), Is.False);
		Assert.That(ini.GetBoolean("v", "one"), Is.True);
		Assert.That(ini.GetBoolean("v", "zero", true), Is.False);
		Assert.That(ini.GetBoolean("v", "bad", true), Is.True);
		Assert.That(ini.GetBinary("v", "b64"), Is.EqualTo(new byte[] { 1, 2, 3 }));
		Assert.That(ini.GetBinary("v", "nob64"), Is.Empty);
	}


	/// <summary>Changing values keeps comments, order and formatting of other lines.</summary>
	[Test]
	public void WritePreservesFile()
	{
		string f = _File("a.ini", "; header\r\n[A]\r\nx = 1 ; not a comment\r\n\r\n; about B\r\n[B]\r\ny=2\r\n");
		IniFile ini = new(f);

		ini.SetValue("A", "x", 10);
		ini.SetValue("A", "new", "n");
		ini.SetValue("B", "y", 2);

		Assert.That(File.ReadAllText(f), Is.EqualTo("; header\r\n[A]\r\nx=10\r\nnew=n\r\n\r\n; about B\r\n[B]\r\ny=2\r\n"));
	}


	/// <summary>New sections are appended with a blank line; values round-trip, including quotes and padding.</summary>
	[Test]
	public void WriteNewSectionsAndRoundTrip()
	{
		string f = _File("sub/dir/b.ini");
		IniFile ini = new(f);

		ini.SetValue("First", "s", " padded ");
		ini.SetValue("First", "q", "\"quoted\"");
		ini.SetValue("Second", "d", 1.5);
		ini.SetValue("Second", "b", true);
		ini.SetIntBoolean("Second", "ib", true);
		ini.SetValue("Second", "bin", new byte[] { 1, 2, 3, 4 }, 1, 2);
		ini.SetValue("Second", "l", 9000000000L);

		Assert.That(File.ReadAllText(f), Is.EqualTo("[First]\r\ns=\" padded \"\r\nq=\"\"quoted\"\"\r\n\r\n" +
													"[Second]\r\nd=1.5\r\nb=true\r\nib=1\r\nbin=AgM=\r\nl=9000000000\r\n"));

		IniFile read = new(f);
		Assert.That(read.GetString("First", "s"), Is.EqualTo(" padded "));
		Assert.That(read.GetString("First", "q"), Is.EqualTo("\"quoted\""));
		Assert.That(read.GetDouble("Second", "d"), Is.EqualTo(1.5));
		Assert.That(read.GetBoolean("Second", "b"), Is.True);
		Assert.That(read.GetBoolean("Second", "ib"), Is.True);
		Assert.That(read.GetBinary("Second", "bin"), Is.EqualTo(new byte[] { 2, 3 }));
	}


	/// <summary>Old bug: a value written as "true" was read as FALSE by GetIntBoolean; GetBoolean reads both forms.</summary>
	[Test]
	public void BooleanForms()
	{
		IniFile ini = new(_File("a.ini"));
		ini.SetValue("s", "a", true);
		ini.SetIntBoolean("s", "b", true);

		Assert.That(ini.GetBoolean("s", "a"), Is.True);
		Assert.That(ini.GetBoolean("s", "b"), Is.True);
	}


	/// <summary>Deleting keys and sections, also with NULL values.</summary>
	[Test]
	public void Delete()
	{
		string f = _File("a.ini", "[A]\r\nx=1\r\ny=2\r\nx=3\r\n[B]\r\nz=1\r\n[a]\r\nw=1\r\n");
		IniFile ini = new(f);

		Assert.That(ini.DeleteKey("A", "X"), Is.True);
		Assert.That(ini.DeleteKey("A", "x"), Is.False);
		ini.SetValue("A", "y", (string?) null);
		Assert.That(ini.DeleteSection("a"), Is.True);
		Assert.That(ini.DeleteSection("A"), Is.False);
		Assert.That(ini.DeleteKey("Missing", "x"), Is.False);

		Assert.That(File.ReadAllText(f), Is.EqualTo("[B]\r\nz=1\r\n"));
	}


	/// <summary>Old bug: GetSections returned one empty name for an empty or missing file.</summary>
	[Test]
	public void EmptyFile()
	{
		Assert.That(new IniFile(_File("missing.ini")).GetSections(), Is.Empty);
		Assert.That(new IniFile(_File("empty.ini", "")).GetSections(), Is.Empty);
		Assert.That(File.Exists(_File("missing.ini")), Is.False);
	}


	/// <summary>Old bug: relative file names were resolved against the Windows directory.</summary>
	[Test]
	public void RelativePath()
	{
		string cwd = Environment.CurrentDirectory;
		try
		{
			Environment.CurrentDirectory = _Directory;
			IniFile ini = new("relative.ini");
			ini.SetValue("s", "k", "v");

			Assert.That(ini.FileName, Is.EqualTo(Path.Combine(_Directory, "relative.ini")));
			Assert.That(File.Exists(Path.Combine(_Directory, "relative.ini")), Is.True);
		}
		finally { Environment.CurrentDirectory = cwd; }
	}


	/// <summary>Old bug: umlauts were lost (ANSI functions). UTF-8, UTF-16 and ANSI files keep their encoding.</summary>
	[Test]
	public void Encodings()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		Encoding ansi = Encoding.GetEncoding(1252);

		foreach((string name, Encoding enc) in new[] { ("utf8.ini", (Encoding) new UTF8Encoding(false)), ("utf8bom.ini", new UTF8Encoding(true)),
													   ("utf16.ini", new UnicodeEncoding(false, true)), ("ansi.ini", ansi) })
		{
			string f = _File(name, "[Größe]\r\nWert=Äpfel\r\n", enc);
			byte[] before = File.ReadAllBytes(f);

			IniFile ini = new(f);
			Assert.That(ini.GetString("Größe", "Wert"), Is.EqualTo("Äpfel"), name);

			ini.SetValue("Größe", "Neu", "Übermaß");
			byte[] after = File.ReadAllBytes(f);
			Assert.That(after[..3], Is.EqualTo(before[..3]), name);
			Assert.That(enc.GetString(after), Does.EndWith("Neu=Übermaß\r\n"), name);
			Assert.That(new IniFile(f).GetString("Größe", "Neu"), Is.EqualTo("Übermaß"), name);
		}
	}


	/// <summary>Line feeds without carriage return are preserved.</summary>
	[Test]
	public void UnixLineEndings()
	{
		string f = _File("a.ini", "[A]\nx=1\n");
		new IniFile(f).SetValue("A", "y", 2);

		Assert.That(File.ReadAllText(f), Is.EqualTo("[A]\nx=1\ny=2\n"));
	}


	/// <summary>Without automatic saving, changes are written by Save and discarded by Reload.</summary>
	[Test]
	public void ManualSave()
	{
		string f = _File("a.ini", "[A]\r\nx=1\r\n");
		IniFile ini = new(f) { AutoSave = false };

		ini.SetValue("A", "x", 2);
		Assert.That(new IniFile(f).GetInt32("A", "x"), Is.EqualTo(1));

		ini.Save();
		Assert.That(new IniFile(f).GetInt32("A", "x"), Is.EqualTo(2));

		ini.SetValue("A", "x", 3);
		ini.Reload();
		Assert.That(ini.GetInt32("A", "x"), Is.EqualTo(2));
		Assert.That(Directory.GetFiles(_Directory, "~*"), Is.Empty);
	}


	/// <summary>Invalid names and values are rejected.</summary>
	[Test]
	public void Validation()
	{
		IniFile ini = new(_File("a.ini"));

		Assert.Throws<ArgumentException>(() => new IniFile(" "));
		Assert.Throws<ArgumentException>(() => ini.SetValue("", "k", "v"));
		Assert.Throws<ArgumentException>(() => ini.SetValue("a]b", "k", "v"));
		Assert.Throws<ArgumentException>(() => ini.SetValue("s", "a=b", "v"));
		Assert.Throws<ArgumentException>(() => ini.SetValue("s", ";k", "v"));
		Assert.Throws<ArgumentException>(() => ini.SetValue("s", "[k", "v"));
		Assert.Throws<ArgumentException>(() => ini.SetValue("s", "k", "a\nb"));
		Assert.Throws<ArgumentException>(() => ini.GetString("s", ""));
		Assert.That(File.Exists(ini.FileName), Is.False);
	}
}
