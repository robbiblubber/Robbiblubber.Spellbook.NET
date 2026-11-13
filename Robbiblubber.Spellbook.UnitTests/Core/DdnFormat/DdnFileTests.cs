namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests reading and writing ddn files and streams.</summary>
[TestFixture]
public class DdnFileTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Temporary directory.</summary>
	private string _Dir = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the temporary directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Dir = Path.Combine(Path.GetTempPath(), "ddn-tests-" + Guid.NewGuid().ToString("N"));
	}


	/// <summary>Deletes the temporary directory.</summary>
	[TearDown]
	public void TearDown()
	{
		if(Directory.Exists(_Dir)) { Directory.Delete(_Dir, true); }
	}


	/// <summary>Saving creates missing directories, opening reads the file.</summary>
	[Test]
	public void SaveAndOpen_File()
	{
		string file = Path.Combine(_Dir, "sub", "test.ddn");
		DdnElement ddn = Ddn.Create();
		ddn["a/b"] = "ä€";

		ddn.Save(file);

		Assert.That(Ddn.Open(file).Get<string>("a/b"), Is.EqualTo("ä€"));
		Assert.That(File.ReadAllBytes(file).Take(3), Is.EqualTo(Encoding.UTF8.GetPreamble()));
	}


	/// <summary>Saving replaces an existing file and leaves no temporary file behind.</summary>
	[Test]
	public void Save_ReplacesFile()
	{
		string file = Path.Combine(_Dir, "replace.ddn");
		Ddn.Create("a = 1; b = 2;").Save(file);

		Ddn.Create("c = 3;").Save(file);

		DdnElement ddn = Ddn.Open(file);
		Assert.That(ddn.Contains("a"), Is.False);
		Assert.That(ddn.Get<int>("c"), Is.EqualTo(3));
		Assert.That(Directory.GetFiles(_Dir), Is.EqualTo(new[] { file }));
	}


	/// <summary>Saving to a file name without directory works.</summary>
	[Test]
	public void Save_FileNameWithoutDirectory()
	{
		Directory.CreateDirectory(_Dir);
		string cur = Environment.CurrentDirectory;

		try
		{
			Environment.CurrentDirectory = _Dir;
			Ddn.Create("a = 1;").Save("plain.ddn");

			Assert.That(Ddn.Open(Path.Combine(_Dir, "plain.ddn")).Get<int>("a"), Is.EqualTo(1));
		}
		finally { Environment.CurrentDirectory = cur; }
	}


	/// <summary>Opening a missing file throws unless errors are ignored.</summary>
	[Test]
	public void Open_MissingFile()
	{
		string file = Path.Combine(_Dir, "missing.ddn");

		Assert.Throws<DirectoryNotFoundException>(() => Ddn.Open(file));
		Assert.That(Ddn.Open(file, true).ElementCount, Is.EqualTo(0));
	}


	/// <summary>Appending to a file.</summary>
	[Test]
	public void Save_Append()
	{
		string file = Path.Combine(_Dir, "append.ddn");

		Ddn.Create("a = 1;").Save(file);
		Ddn.Create("b = 2;").Save(file, new Ddn.Options { Append = true });

		DdnElement ddn = Ddn.Open(file);
		Assert.That(ddn.Get<int>("a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
	}


	/// <summary>Streams are read and written without closing them.</summary>
	[Test]
	public void SaveAndOpen_Stream()
	{
		using MemoryStream stream = new();
		Ddn.Create("a = 1; s { b = 2; }").Save(stream);

		Assert.That(stream.CanWrite, Is.True);
		stream.Position = 0;

		DdnElement ddn = Ddn.Open(stream);
		Assert.That(stream.CanRead, Is.True);
		Assert.That(ddn.Get<int>("s/b"), Is.EqualTo(2));
	}


	/// <summary>Files with encoders.</summary>
	[Test]
	public void Encoders()
	{
		foreach(IEncoder i in new IEncoder[] { Base64.DEFAULT, Hex.DEFAULT, GZip.BASE64, new FunnelCake(Base64.DEFAULT, "secret") })
		{
			string file = Path.Combine(_Dir, i.GetType().Name + ".ddn");
			Ddn.Options options = new(i);
			DdnElement ddn = Ddn.Create("a = 1; s { b = Text; }");

			ddn.Save(file, options);

			Assert.That(File.ReadAllText(file), Does.Not.Contain("Text"), i.GetType().Name);
			Assert.That(Ddn.Open(file, options).Get<string>("s/b"), Is.EqualTo("Text"), i.GetType().Name);
		}
	}


	/// <summary>Plain text encoder writes plain text.</summary>
	[Test]
	public void Encoder_PlainText()
	{
		string file = Path.Combine(_Dir, "plain.ddn");
		Ddn.Create("a = 1;").Save(file, new Ddn.Options(PlainText.DEFAULT));

		Assert.That(File.ReadAllText(file), Is.EqualTo("a = 1;\r\n"));
	}

	/// <summary>Duplicates in files are reported.</summary>
	[Test]
	public void Open_Duplicates()
	{
		string file = Path.Combine(_Dir, "dup.ddn");
		Directory.CreateDirectory(_Dir);
		File.WriteAllText(file, "a = 1; a = 2;");

		DdnElement ddn = Ddn.Open(file);
		Assert.That(ddn.Get<int>("a"), Is.EqualTo(2));
		Assert.That(ddn.ParsedDuplicates, Has.Length.EqualTo(1));
	}
}
