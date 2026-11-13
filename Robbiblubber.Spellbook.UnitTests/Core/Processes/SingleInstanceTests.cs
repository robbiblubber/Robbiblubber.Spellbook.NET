namespace Robbiblubber.Spellbook.UnitTests.Core.Processes;

using System.Collections.Concurrent;

using NUnit.Framework;



/// <summary>This class tests the <see cref="SingleInstance"/> class.</summary>
[TestFixture]
public class SingleInstanceTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Instances created by a test.</summary>
	private readonly List<SingleInstance> _Instances = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Disposes the instances of a test.</summary>
	[TearDown]
	public void TearDown()
	{
		foreach(SingleInstance i in _Instances) { i.Dispose(); }
		_Instances.Clear();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Returns a unique application identifier.</summary>
	/// <returns>Returns the identifier.</returns>
	private static string _NewId()
	{
		return "robbiblubber.org/spellbook-tests/" + Guid.NewGuid().ToString("N");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates an instance that is disposed after the test.</summary>
	/// <param name="id">Application identifier.</param>
	/// <returns>Returns the instance.</returns>
	private SingleInstance _Create(string id)
	{
		SingleInstance rval = new(id);
		_Instances.Add(rval);
		return rval;
	}


	/// <summary>Creates a listening first instance that collects the received arguments.</summary>
	/// <param name="id">Application identifier.</param>
	/// <param name="received">Collection of received event arguments.</param>
	/// <returns>Returns the instance.</returns>
	private SingleInstance _CreateListening(string id, BlockingCollection<SingleInstanceEventArgs> received)
	{
		SingleInstance rval = _Create(id);
		rval.ArgumentsReceived += (s, e) => received.Add(e);
		rval.StartListening();
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>The first instance for an identifier is the first instance, the second is not.</summary>
	[Test]
	public void IsFirstInstance_OnlyFirst()
	{
		string id = _NewId();
		SingleInstance a = _Create(id);
		SingleInstance b = _Create(id);

		Assert.That(a.IsFirstInstance, Is.True);
		Assert.That(b.IsFirstInstance, Is.False);
		Assert.That(a.Id, Is.EqualTo(id));
	}


	/// <summary>Different identifiers do not interfere.</summary>
	[Test]
	public void IsFirstInstance_DifferentIds()
	{
		Assert.That(_Create(_NewId()).IsFirstInstance, Is.True);
		Assert.That(_Create(_NewId()).IsFirstInstance, Is.True);
	}


	/// <summary>After the first instance has been disposed, a new instance becomes the first instance.</summary>
	[Test]
	public void Dispose_ReleasesFirstInstance()
	{
		string id = _NewId();
		SingleInstance a = new(id);
		Assert.That(a.IsFirstInstance, Is.True);
		a.Dispose();

		Assert.That(_Create(id).IsFirstInstance, Is.True);
	}


	/// <summary>Empty identifiers are rejected.</summary>
	[Test]
	public void Constructor_EmptyId()
	{
		Assert.Throws<ArgumentException>(() => new SingleInstance(" "));
	}


	/// <summary>Forwarded arguments and the working directory reach the first instance.</summary>
	[Test]
	public void Forward_ArgumentsReceived()
	{
		string id = _NewId();
		using BlockingCollection<SingleInstanceEventArgs> received = new();
		SingleInstance a = _CreateListening(id, received);
		SingleInstance b = _Create(id);

		string[] args = ["a.ltab", "C:\\Daten mit Leerzeichen\\b.ltab", "\"quoted\"", "Ümlaut €", string.Empty];
		Assert.That(b.Forward(args), Is.True);
		Assert.That(received.TryTake(out SingleInstanceEventArgs? e, 5000), Is.True);

		Assert.That(e!.Arguments, Is.EqualTo(args));
		Assert.That(e.WorkingDirectory, Is.EqualTo(Environment.CurrentDirectory));
		Assert.That(a.IsListening, Is.True);
	}


	/// <summary>Forwarding without arguments raises the event with an empty array.</summary>
	[Test]
	public void Forward_NoArguments()
	{
		string id = _NewId();
		using BlockingCollection<SingleInstanceEventArgs> received = new();
		_CreateListening(id, received);

		Assert.That(_Create(id).Forward([]), Is.True);
		Assert.That(received.TryTake(out SingleInstanceEventArgs? e, 5000), Is.True);
		Assert.That(e!.Arguments, Is.Empty);
	}


	/// <summary>Several forwards in a row are all received in order.</summary>
	[Test]
	public void Forward_Several()
	{
		string id = _NewId();
		using BlockingCollection<SingleInstanceEventArgs> received = new();
		_CreateListening(id, received);

		for(int i = 0; i < 5; i++)
		{
			Assert.That(_Create(id).Forward([i.ToString()]), Is.True);
		}

		for(int i = 0; i < 5; i++)
		{
			Assert.That(received.TryTake(out SingleInstanceEventArgs? e, 5000), Is.True);
			Assert.That(e!.Arguments, Is.EqualTo(new[] { i.ToString() }));
		}
	}


	/// <summary>Forwarding fails if the first instance does not listen.</summary>
	[Test]
	public void Forward_NotListening()
	{
		string id = _NewId();
		_Create(id);

		Assert.That(_Create(id).Forward(["x"], 300), Is.False);
	}


	/// <summary>Forwarding fails after the first instance has been disposed.</summary>
	[Test]
	public void Forward_AfterDispose()
	{
		string id = _NewId();
		using BlockingCollection<SingleInstanceEventArgs> received = new();
		SingleInstance a = _CreateListening(id, received);
		SingleInstance b = _Create(id);
		a.Dispose();

		Assert.That(b.Forward(["x"], 300), Is.False);
		Assert.That(received, Is.Empty);
	}


	/// <summary>An exception in an event handler does not stop the first instance from receiving arguments.</summary>
	[Test]
	public void ArgumentsReceived_HandlerException()
	{
		string id = _NewId();
		using BlockingCollection<SingleInstanceEventArgs> received = new();
		SingleInstance a = _Create(id);
		bool thrown = false;
		a.ArgumentsReceived += (s, e) => { if(!thrown) { thrown = true; throw new InvalidOperationException(); } received.Add(e); };
		a.StartListening();

		Assert.That(_Create(id).Forward(["1"]), Is.True);
		Assert.That(_Create(id).Forward(["2"]), Is.True);
		Assert.That(received.TryTake(out SingleInstanceEventArgs? e, 5000), Is.True);
		Assert.That(e!.Arguments, Is.EqualTo(new[] { "2" }));
	}


	/// <summary>Only the first instance listens, only further instances forward.</summary>
	[Test]
	public void Roles_Enforced()
	{
		string id = _NewId();
		SingleInstance a = _Create(id);
		SingleInstance b = _Create(id);

		Assert.Throws<InvalidOperationException>(() => a.Forward(["x"]));
		Assert.Throws<InvalidOperationException>(() => b.StartListening());
	}


	/// <summary>Starting to listen twice has no effect; a disposed instance cannot listen.</summary>
	[Test]
	public void StartListening_Twice()
	{
		SingleInstance a = _Create(_NewId());
		a.StartListening();
		a.StartListening();
		Assert.That(a.IsListening, Is.True);

		a.Dispose();
		Assert.That(a.IsListening, Is.False);
		Assert.Throws<ObjectDisposedException>(() => a.StartListening());
	}


	/// <summary>Relative paths are resolved against the working directory of the forwarding instance.</summary>
	[Test]
	public void GetFullPath_Relative()
	{
		string dir = Path.GetTempPath();
		SingleInstanceEventArgs e = new(["sub/a.ltab"], dir);

		Assert.That(e.GetFullPath("sub/a.ltab"), Is.EqualTo(Path.GetFullPath(Path.Combine(dir, "sub/a.ltab"))));
		Assert.That(e.GetFullPath(Path.Combine(dir, "b.ltab")), Is.EqualTo(Path.GetFullPath(Path.Combine(dir, "b.ltab"))));
	}
}
