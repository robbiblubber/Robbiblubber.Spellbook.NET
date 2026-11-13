namespace Robbiblubber.Spellbook.UnitTests.Core.Files;

using System.Text;
using NUnit.Framework;



/// <summary>This class tests the buffered file writer.</summary>
[TestFixture]
[NonParallelizable]
public class BufferedFileWriterTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Dir = string.Empty;

	/// <summary>Test file.</summary>
	private string _FileName = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a test directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Dir = Path.Combine(Path.GetTempPath(), "spellbook-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Dir);
		_FileName = Path.Combine(_Dir, "test.log");
	}


	/// <summary>Deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		if(Directory.Exists(_Dir)) { Directory.Delete(_Dir, true); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads the test file.</summary>
	/// <param name="fileName">File name, NULL for the test file.</param>
	/// <returns>Returns the file content; NULL if the file does not exist or cannot be read at the moment.</returns>
	/// <remarks>The writer may be appending to the file in the background. The file is opened with full sharing, and if it is
	///          locked anyway, NULL is returned, so a caller waiting with <see cref="_WaitFor(Func{bool})"/> simply reads
	///          again.</remarks>
	private string? _Read(string? fileName = null)
	{
		try
		{
			using FileStream s = new((fileName ?? _FileName), FileMode.Open, FileAccess.Read, (FileShare.ReadWrite | FileShare.Delete));
			using StreamReader r = new(s);
			return r.ReadToEnd();
		}
		catch(IOException) { return null; }
	}


	/// <summary>Waits until a condition is met.</summary>
	/// <param name="condition">Condition.</param>
	/// <returns>Returns TRUE if the condition has been met within five seconds.</returns>
	private static bool _WaitFor(Func<bool> condition)
	{
		DateTime limit = DateTime.UtcNow.AddSeconds(5);
		while(DateTime.UtcNow < limit)
		{
			if(condition()) return true;
			Thread.Sleep(10);
		}
		return condition();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text is written in the background while running.</summary>
	[Test]
	public void Running_WritesInBackground()
	{
		using BufferedFileWriter w = new(_FileName, 20);
		w.Start();
		Assert.That(w.Running, Is.True);

		w.Write("a");
		w.WriteLine("b");

		Assert.That(_WaitFor(() => (_Read() == ("ab" + Environment.NewLine))), Is.True);
	}


	/// <summary>The interval property takes effect.</summary>
	[Test]
	public void Interval()
	{
		using BufferedFileWriter w = new(_FileName, 60000);
		Assert.Throws<ArgumentOutOfRangeException>(() => w.Interval = 0);
		Assert.Throws<ArgumentOutOfRangeException>(() => new BufferedFileWriter(_FileName, 0));

		w.Start();
		Thread.Sleep(100);
		w.Write("x");
		Thread.Sleep(200);

		Assert.That(_Read(), Is.Null);  // first interval of 60 s is still running

		w.Stop();
		w.Interval = 20;
		w.Start();
		w.Write("y");

		Assert.That(_WaitFor(() => (_Read() == "xy")), Is.True);
	}


	/// <summary>Stop writes all pending text; text written after stop is written on the next flush.</summary>
	[Test]
	public void Stop_WritesEverything()
	{
		BufferedFileWriter w = new(_FileName, 60000);
		w.Start();
		for(int i = 0; i < 1000; i++) { w.WriteLine(i.ToString()); }
		w.Stop();

		Assert.That(w.Running, Is.False);
		Assert.That(File.ReadAllLines(_FileName), Has.Length.EqualTo(1000));

		w.Write("after");
		Assert.That(_Read(), Does.Not.EndWith("after"));
		w.Flush();
		Assert.That(_Read(), Does.EndWith("after"));
	}


	/// <summary>Dispose writes pending text; writing afterwards throws.</summary>
	[Test]
	public void Dispose_WritesPending()
	{
		BufferedFileWriter w = new(_FileName);
		w.Write("pending");
		w.Dispose();
		w.Dispose();

		Assert.That(_Read(), Is.EqualTo("pending"));
		Assert.Throws<ObjectDisposedException>(() => w.Write("x"));
		Assert.Throws<ObjectDisposedException>(() => w.Start());
	}


	/// <summary>DisposeAsync writes pending text.</summary>
	[Test]
	public async Task DisposeAsync_WritesPending()
	{
		await using(BufferedFileWriter w = new(_FileName, 60000))
		{
			w.Start();
			w.Write("async");
		}

		Assert.That(_Read(), Is.EqualTo("async"));
	}


	/// <summary>Write errors keep the text pending: Flush throws, the background raises WriteError.</summary>
	[Test]
	public void WriteError_KeepsText()
	{
		string bad = Path.Combine(_Dir, "missing", "test.log");
		using BufferedFileWriter w = new(bad, 20);
		List<Exception> errors = new();
		w.WriteError += (_, e) => { lock(errors) { errors.Add(e.GetException()); } };

		w.Write("keep");
		Assert.Throws<DirectoryNotFoundException>(() => w.Flush());

		w.Start();
		Assert.That(_WaitFor(() => { lock(errors) { return (errors.Count > 0); } }), Is.True);
		Assert.That(errors[0], Is.InstanceOf<DirectoryNotFoundException>());

		Directory.CreateDirectory(Path.GetDirectoryName(bad)!);
		Assert.That(_WaitFor(() => (_Read(bad) == "keep")), Is.True);
	}


	/// <summary>FlushAsync writes pending text.</summary>
	[Test]
	public async Task FlushAsync()
	{
		using BufferedFileWriter w = new(_FileName);
		w.WriteLine();
		w.WriteLine(null);
		await w.FlushAsync();

		Assert.That(_Read(), Is.EqualTo(Environment.NewLine + Environment.NewLine));
	}


	/// <summary>Line terminator and encoding can be changed.</summary>
	[Test]
	public void NewLineAndEncoding()
	{
		using(BufferedFileWriter w = new(_FileName) { NewLine = "\n", Encoding = Encoding.Latin1 })
		{
			w.WriteLine("ä");
		}

		Assert.That(File.ReadAllBytes(_FileName), Is.EqualTo(new byte[] { 0xE4, 0x0A }));

		using(BufferedFileWriter w = new(_FileName))
		{
			w.Write("ö");
		}
		Assert.That(File.ReadAllBytes(_FileName).Skip(2), Is.EqualTo(new byte[] { 0xC3, 0xB6 }));
	}


	/// <summary>Concurrent writes are all written.</summary>
	[Test]
	public void ConcurrentWrites()
	{
		using(BufferedFileWriter w = new(_FileName, 5))
		{
			w.Start();
			Parallel.For(0, 4000, i => w.WriteLine(i.ToString()));
		}

		Assert.That(File.ReadAllLines(_FileName).Select(int.Parse).Order(), Is.EqualTo(Enumerable.Range(0, 4000)));
	}
}
