namespace Robbiblubber.Spellbook.UnitTests.Backpack.Packing;

using NUnit.Framework;
using Robbiblubber.Spellbook.Backpack;



/// <summary>This class tests the packer.</summary>
[TestFixture]
public class PackerTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Footer length.</summary>
	private const int _FOOTER_LENGTH = 84;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Directory = "";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a file.</summary>
	/// <param name="name">File name.</param>
	/// <param name="data">Content.</param>
	/// <returns>Returns the full file name.</returns>
	private string _File(string name, byte[]? data = null)
	{
		string rval = Path.Combine(_Directory, name);
		if(data is not null) { File.WriteAllBytes(rval, data); }

		return rval;
	}


	/// <summary>Creates random data.</summary>
	/// <param name="length">Length.</param>
	/// <param name="seed">Seed.</param>
	/// <returns>Returns the data.</returns>
	private static byte[] _Data(int length, int seed = 1)
	{
		byte[] rval = new byte[length];
		new Random(seed).NextBytes(rval);

		return rval;
	}


	/// <summary>Overwrites a byte in a file.</summary>
	/// <param name="file">File name.</param>
	/// <param name="offset">Offset; negative values count from the end.</param>
	/// <param name="value">Value.</param>
	private static void _Poke(string file, long offset, byte value)
	{
		using FileStream fs = new(file, FileMode.Open, FileAccess.ReadWrite);
		fs.Seek(offset, ((offset < 0) ? SeekOrigin.End : SeekOrigin.Begin));
		fs.WriteByte(value);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the test directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Directory = Path.Combine(Path.GetTempPath(), "Robbiblubber.Spellbook.UnitTests.Backpack", Guid.NewGuid().ToString("N"));
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

	/// <summary>Appending and extracting files keeps carrier and payload intact.</summary>
	[Test]
	public void AppendAndExtractFile()
	{
		byte[] carrier = _Data(1000, 1), payload = _Data(500, 2);
		string c = _File("carrier.bin", carrier), p = _File("payload.bin", payload), t = _File("target.bin");

		Packer.Append(c, p);
		Packer.Extract(c, t);

		byte[] packed = File.ReadAllBytes(c);
		Assert.That(packed.Length, Is.EqualTo(1000 + 500 + _FOOTER_LENGTH));
		Assert.That(packed[..1000], Is.EqualTo(carrier));
		Assert.That(File.ReadAllBytes(t), Is.EqualTo(payload));
	}


	/// <summary>A carrier that ends with 0x04 and a payload that contains the old delimiter work (old bug).</summary>
	[Test]
	public void CarrierEndingWithDelimiterByte()
	{
		byte[] old = new byte[] { 4, 4, 4, 4, 7, 7, 7, 4, 4, 4, 4, 4, 4, 2, 4, 8, 8, 4, 4, 4, 4, 4, 24, 4 };
		byte[] carrier = new byte[] { 1, 2, 3, 4 }.Concat(old).ToArray();
		byte[] payload = old.Concat(old).ToArray();
		string c = _File("carrier.bin", carrier);

		Packer.Append(c, payload);

		Assert.That(Packer.ReadPayload(c), Is.EqualTo(payload));
		Assert.That(Packer.Remove(c), Is.True);
		Assert.That(File.ReadAllBytes(c), Is.EqualTo(carrier));
	}


	/// <summary>TryExtract returns TRUE on success and FALSE without payload (old bug: always FALSE).</summary>
	[Test]
	public void TryExtractResult()
	{
		string c = _File("carrier.bin", _Data(100)), u = _File("plain.bin", _Data(100, 3)), t = _File("target.bin");
		Packer.Append(c, _Data(10, 4));

		Assert.That(Packer.TryExtract(c, t), Is.True);
		Assert.That(File.Exists(t), Is.True);

		File.Delete(t);
		Assert.That(Packer.TryExtract(u, t), Is.False);
		Assert.That(File.Exists(t), Is.False);
		Assert.That(Directory.GetFiles(_Directory), Has.Length.EqualTo(2));
	}


	/// <summary>Extract throws when there is no payload.</summary>
	[Test]
	public void ExtractWithoutPayload()
	{
		string u = _File("plain.bin", _Data(200)), t = _File("target.bin");

		Assert.That(Packer.HasPayload(u), Is.False);
		Assert.That(Packer.GetInfo(u), Is.Null);
		Assert.Throws<InvalidDataException>(() => Packer.Extract(u, t));
		Assert.Throws<InvalidDataException>(() => Packer.ReadPayload(u));
		Assert.Throws<InvalidDataException>(() => Packer.OpenPayload(u));
		Assert.That(Packer.Verify(u), Is.False);
		Assert.That(Packer.Remove(u), Is.False);
		Assert.That(File.Exists(t), Is.False);
	}


	/// <summary>Files shorter than the footer have no payload.</summary>
	[Test]
	public void ShortFile()
	{
		string u = _File("short.bin", _Data(10));
		Assert.That(Packer.HasPayload(u), Is.False);
	}


	/// <summary>A carrier can only hold one payload.</summary>
	[Test]
	public void AppendTwice()
	{
		string c = _File("carrier.bin", _Data(100));
		Packer.Append(c, _Data(10));
		long len = new FileInfo(c).Length;

		Assert.Throws<InvalidOperationException>(() => Packer.Append(c, _Data(10)));
		Assert.That(new FileInfo(c).Length, Is.EqualTo(len));
	}


	/// <summary>A missing carrier is not created.</summary>
	[Test]
	public void MissingCarrier()
	{
		string c = _File("missing.bin"), p = _File("payload.bin", _Data(10));

		Assert.Throws<FileNotFoundException>(() => Packer.Append(c, p));
		Assert.That(File.Exists(c), Is.False);
		Assert.Throws<FileNotFoundException>(() => Packer.HasPayload(c));
	}


	/// <summary>Source and target must differ.</summary>
	[Test]
	public void SameFile()
	{
		string c = _File("carrier.bin", _Data(100));

		Assert.Throws<ArgumentException>(() => Packer.Append(c, c));
		Assert.Throws<ArgumentException>(() => Packer.Extract(c, c));
		Assert.Throws<ArgumentException>(() => Packer.Append("", c));
	}


	/// <summary>A damaged payload is detected and does not replace the target file.</summary>
	[Test]
	public void DamagedPayload()
	{
		string c = _File("carrier.bin", _Data(100)), t = _File("target.bin", new byte[] { 42 });
		Packer.Append(c, _Data(50, 5));
		_Poke(c, 120, 0);
		_Poke(c, 121, 1);

		Assert.That(Packer.HasPayload(c), Is.True);
		Assert.That(Packer.Verify(c), Is.False);
		Assert.Throws<InvalidDataException>(() => Packer.Extract(c, t));
		Assert.Throws<InvalidDataException>(() => Packer.TryExtract(c, t));
		Assert.That(File.ReadAllBytes(t), Is.EqualTo(new byte[] { 42 }));
		Assert.That(Directory.GetFiles(_Directory), Has.Length.EqualTo(2));
	}


	/// <summary>A damaged footer length throws.</summary>
	[Test]
	public void DamagedFooter()
	{
		string c = _File("carrier.bin", _Data(100));
		Packer.Append(c, _Data(50));
		_Poke(c, -_FOOTER_LENGTH + 32 + 7, 0x7F);

		Assert.Throws<InvalidDataException>(() => Packer.HasPayload(c));
		Assert.That(Packer.Verify(c), Is.False);
		Assert.Throws<InvalidDataException>(() => Packer.Append(c, _Data(10)));
	}


	/// <summary>An unknown format version throws.</summary>
	[Test]
	public void UnsupportedVersion()
	{
		string c = _File("carrier.bin", _Data(100));
		Packer.Append(c, _Data(50));
		_Poke(c, -_FOOTER_LENGTH + 72, 99);

		InvalidDataException? ex = Assert.Throws<InvalidDataException>(() => Packer.GetInfo(c));
		Assert.That(ex!.Message, Does.Contain("99"));
	}


	/// <summary>Tags are stored and filter the payload.</summary>
	[Test]
	public void Tags()
	{
		string c = _File("carrier.bin", _Data(100)), t = _File("target.bin");
		Packer.Append(c, _Data(20), "setup-dätä");

		PayloadInfo info = Packer.GetInfo(c)!;
		Assert.That(info.Tag, Is.EqualTo("setup-dätä"));
		Assert.That(info.Length, Is.EqualTo(20));
		Assert.That(info.CarrierLength, Is.EqualTo(100));
		Assert.That(info.Hash.Length, Is.EqualTo(32));

		Assert.That(Packer.HasPayload(c), Is.True);
		Assert.That(Packer.HasPayload(c, "setup-dätä"), Is.True);
		Assert.That(Packer.HasPayload(c, "other"), Is.False);
		Assert.That(Packer.TryExtract(c, t, "other"), Is.False);
		Assert.Throws<InvalidDataException>(() => Packer.ReadPayload(c, "other"));
		Assert.That(Packer.Remove(c, "other"), Is.False);
		Assert.That(Packer.Remove(c, "setup-dätä"), Is.True);
	}


	/// <summary>Empty tags mean no tag; invalid tags throw.</summary>
	[Test]
	public void TagValidation()
	{
		string c = _File("carrier.bin", _Data(100));

		Assert.Throws<ArgumentException>(() => Packer.Append(c, _Data(10), new string('x', 33)));
		Assert.Throws<ArgumentException>(() => Packer.Append(c, _Data(10), new string('ä', 17)));
		Assert.Throws<ArgumentException>(() => Packer.Append(c, _Data(10), "a\0b"));
		Assert.That(new FileInfo(c).Length, Is.EqualTo(100));

		Packer.Append(c, _Data(10), "");
		Assert.That(Packer.GetInfo(c)!.Tag, Is.Null);
		Assert.That(Packer.HasPayload(c, ""), Is.True);

		Packer.Remove(c);
		Packer.Append(c, _Data(10), new string('x', 32));
		Assert.That(Packer.GetInfo(c)!.Tag, Is.EqualTo(new string('x', 32)));
	}


	/// <summary>Remove restores the carrier.</summary>
	[Test]
	public void Remove()
	{
		byte[] carrier = _Data(300);
		string c = _File("carrier.bin", carrier);
		Packer.Append(c, _Data(30));

		Assert.That(Packer.Remove(c), Is.True);
		Assert.That(File.ReadAllBytes(c), Is.EqualTo(carrier));
		Assert.That(Packer.Remove(c), Is.False);
	}


	/// <summary>Empty payloads and empty carriers work.</summary>
	[Test]
	public void EmptyData()
	{
		string c = _File("carrier.bin", Array.Empty<byte>());
		Packer.Append(c, Array.Empty<byte>());

		Assert.That(new FileInfo(c).Length, Is.EqualTo(_FOOTER_LENGTH));
		Assert.That(Packer.HasPayload(c), Is.True);
		Assert.That(Packer.Verify(c), Is.True);
		Assert.That(Packer.ReadPayload(c), Is.Empty);
	}


	/// <summary>Payloads larger than the copy buffer work.</summary>
	[Test]
	public void LargePayload()
	{
		byte[] payload = _Data(300_000, 7);
		string c = _File("carrier.bin", _Data(200_000, 8)), p = _File("payload.bin", payload);

		Packer.Append(c, p);

		Assert.That(Packer.Verify(c), Is.True);
		Assert.That(Packer.ReadPayload(c), Is.EqualTo(payload));
	}


	/// <summary>Stream payloads are read from the current position.</summary>
	[Test]
	public void StreamPayload()
	{
		byte[] data = _Data(100);
		string c = _File("carrier.bin", _Data(10));
		MemoryStream ms = new(data);
		ms.Position = 40;

		Packer.Append(c, ms);
		MemoryStream target = new();
		Packer.Extract(c, target);

		Assert.That(target.ToArray(), Is.EqualTo(data[40..]));
	}


	/// <summary>A failing payload stream restores the carrier.</summary>
	[Test]
	public void FailedAppendRollsBack()
	{
		byte[] carrier = _Data(100);
		string c = _File("carrier.bin", carrier);

		Assert.Throws<IOException>(() => Packer.Append(c, new _FailingStream(200_000)));
		Assert.That(File.ReadAllBytes(c), Is.EqualTo(carrier));
		Assert.That(Packer.HasPayload(c), Is.False);
	}


	/// <summary>Payloads can be read while another process holds the file open for reading only (running executable).</summary>
	[Test]
	public void ReadLockedFile()
	{
		byte[] payload = _Data(64);
		string c = _File("carrier.bin", _Data(100)), t = _File("target.bin");
		Packer.Append(c, payload);

		using FileStream locked = new(c, FileMode.Open, FileAccess.Read, (FileShare.Read | FileShare.Delete));

		Assert.That(Packer.HasPayload(c), Is.True);
		Assert.That(Packer.ReadPayload(c), Is.EqualTo(payload));
		Packer.Extract(c, t);
		Assert.That(File.ReadAllBytes(t), Is.EqualTo(payload));
		using Stream s = Packer.OpenPayload(c);
		Assert.That(s.Length, Is.EqualTo(64));
	}


	/// <summary>The payload stream reads, seeks and is read-only.</summary>
	[Test]
	public void OpenPayload()
	{
		byte[] payload = _Data(1000, 9);
		string c = _File("carrier.bin", _Data(500));
		Packer.Append(c, payload);

		using(Stream s = Packer.OpenPayload(c))
		{
			Assert.That(s.CanRead, Is.True);
			Assert.That(s.CanSeek, Is.True);
			Assert.That(s.CanWrite, Is.False);
			Assert.That(s.Length, Is.EqualTo(1000));

			MemoryStream ms = new();
			s.CopyTo(ms);
			Assert.That(ms.ToArray(), Is.EqualTo(payload));
			Assert.That(s.ReadByte(), Is.EqualTo(-1));

			s.Seek(-10, SeekOrigin.End);
			byte[] buf = new byte[20];
			Assert.That(s.Read(buf, 0, 20), Is.EqualTo(10));
			Assert.That(buf[..10], Is.EqualTo(payload[990..]));

			s.Position = 3;
			Assert.That(s.ReadByte(), Is.EqualTo(payload[3]));
			Assert.Throws<NotSupportedException>(() => s.WriteByte(1));
			Assert.Throws<NotSupportedException>(() => s.SetLength(1));
			Assert.Throws<IOException>(() => s.Seek(-1, SeekOrigin.Begin));
		}

		Assert.That(Packer.Remove(c), Is.True);
	}


	/// <summary>The asynchronous methods work.</summary>
	[Test]
	public async Task Async()
	{
		byte[] payload = _Data(150_000, 10);
		string c = _File("carrier.bin", _Data(100)), p = _File("payload.bin", payload), t = _File("target.bin");

		await Packer.AppendAsync(c, p, "async");
		Assert.That(await Packer.VerifyAsync(c, "async"), Is.True);
		Assert.That(await Packer.ReadPayloadAsync(c), Is.EqualTo(payload));

		await Packer.ExtractAsync(c, t, "async");
		Assert.That(File.ReadAllBytes(t), Is.EqualTo(payload));

		MemoryStream ms = new();
		await Packer.ExtractAsync(c, ms);
		Assert.That(ms.ToArray(), Is.EqualTo(payload));

		using(Stream s = Packer.OpenPayload(c))
		{
			byte[] buf = new byte[100];
			Assert.That(await s.ReadAsync(buf), Is.EqualTo(100));
			Assert.That(buf, Is.EqualTo(payload[..100]));
		}

		Packer.Remove(c);
		await Packer.AppendAsync(c, new MemoryStream(payload));
		Assert.That(Packer.ReadPayload(c), Is.EqualTo(payload));
	}


	/// <summary>Cancelling an append restores the carrier.</summary>
	[Test]
	public void AsyncCancel()
	{
		byte[] carrier = _Data(100);
		string c = _File("carrier.bin", carrier), p = _File("payload.bin", _Data(100_000));
		using CancellationTokenSource cts = new();
		cts.Cancel();

		Assert.CatchAsync<OperationCanceledException>(async () => await Packer.AppendAsync(c, p, null, cts.Token));
		Assert.That(File.ReadAllBytes(c), Is.EqualTo(carrier));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _FailingStream                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a stream that fails after a number of bytes.</summary>
	private sealed class _FailingStream: Stream
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Bytes left before failing.</summary>
		private int _Left;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="failAfter">Number of bytes before the stream fails.</param>
		public _FailingStream(int failAfter)
		{
			_Left = failAfter;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] Stream                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public override bool CanRead { get { return true; } }


		/// <inheritdoc/>
		public override bool CanSeek { get { return false; } }


		/// <inheritdoc/>
		public override bool CanWrite { get { return false; } }


		/// <inheritdoc/>
		public override long Length { get { throw new NotSupportedException(); } }


		/// <inheritdoc/>
		public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }


		/// <inheritdoc/>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(_Left <= 0) { throw new IOException("Simulated failure."); }

			int rval = Math.Min(count, _Left);
			_Left -= rval;
			return rval;
		}


		/// <inheritdoc/>
		public override void Flush() {}


		/// <inheritdoc/>
		public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }


		/// <inheritdoc/>
		public override void SetLength(long value) { throw new NotSupportedException(); }


		/// <inheritdoc/>
		public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
	}
}
