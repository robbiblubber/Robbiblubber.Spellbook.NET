namespace Robbiblubber.Spellbook.UnitTests.Controls.Applications;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="MultiFormApplicationContext"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class MultiFormApplicationContextTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Synchronization context.</summary>
	private TestSynchronizationContext _Sync = null!;

	/// <summary>Previous synchronization context.</summary>
	private SynchronizationContext? _PreviousSync;

	/// <summary>Forms created by a test.</summary>
	private readonly List<Form> _Forms = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Installs the test synchronization context.</summary>
	[SetUp]
	public void SetUp()
	{
		_PreviousSync = SynchronizationContext.Current;
		_Sync = new TestSynchronizationContext();
		SynchronizationContext.SetSynchronizationContext(_Sync);
	}


	/// <summary>Disposes the forms and restores the synchronization context.</summary>
	[TearDown]
	public void TearDown()
	{
		foreach(Form i in _Forms) { i.Dispose(); }
		_Forms.Clear();
		SynchronizationContext.SetSynchronizationContext(_PreviousSync);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a form that is disposed after the test.</summary>
	/// <returns>Returns the form.</returns>
	private Form _NewForm()
	{
		Form rval = new() { ShowInTaskbar = false, StartPosition = FormStartPosition.Manual, Location = new(-2000, -2000) };
		_Forms.Add(rval);
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Added forms are listed once and raise FormAdded.</summary>
	[Test]
	public void AddForm_Listed()
	{
		using MultiFormApplicationContext c = new();
		List<Form> added = new();
		c.FormAdded += (s, e) => added.Add(e.Form);

		Form a = _NewForm(), b = _NewForm();
		c.AddForm(a);
		c.AddForm(b);
		c.AddForm(a);

		Assert.That(c.Forms, Is.EqualTo(new[] { a, b }));
		Assert.That(added, Is.EqualTo(new[] { a, b }));
		Assert.That(c.Instance, Is.Null);
	}


	/// <summary>A closed form is removed; the application ends with the last form.</summary>
	[Test]
	public void Close_LastFormExits()
	{
		using MultiFormApplicationContext c = new();
		int exits = 0;
		List<Form> removed = new();
		c.ThreadExit += (s, e) => exits++;
		c.FormRemoved += (s, e) => removed.Add(e.Form);

		Form a = _NewForm(), b = _NewForm();
		c.ShowForm(a);
		c.ShowForm(b);

		a.Close();
		Assert.That(c.Forms, Is.EqualTo(new[] { b }));
		Assert.That(exits, Is.Zero);

		b.Close();
		Assert.That(c.Forms, Is.Empty);
		Assert.That(removed, Is.EqualTo(new[] { a, b }));
		Assert.That(exits, Is.EqualTo(1));
	}


	/// <summary>Disposing a form without closing it removes it as well.</summary>
	[Test]
	public void Dispose_FormRemoved()
	{
		using MultiFormApplicationContext c = new();
		Form a = _NewForm();
		c.AddForm(a);
		a.Dispose();

		Assert.That(c.Forms, Is.Empty);
	}


	/// <summary>With ExitOnLastFormClosed set to false, the application keeps running.</summary>
	[Test]
	public void ExitOnLastFormClosed_False()
	{
		using MultiFormApplicationContext c = new() { ExitOnLastFormClosed = false };
		int exits = 0;
		c.ThreadExit += (s, e) => exits++;

		Form a = _NewForm();
		c.ShowForm(a);
		a.Close();

		Assert.That(c.Forms, Is.Empty);
		Assert.That(exits, Is.Zero);
	}


	/// <summary>Disposed forms cannot be added.</summary>
	[Test]
	public void AddForm_Disposed()
	{
		using MultiFormApplicationContext c = new();
		Form a = _NewForm();
		a.Dispose();

		Assert.Throws<ObjectDisposedException>(() => c.AddForm(a));
	}


	/// <summary>Arguments of further instances are raised on the synchronization context of the context.</summary>
	[Test]
	public void ArgumentsReceived_Marshaled()
	{
		string id = "robbiblubber.org/spellbook-tests/" + Guid.NewGuid().ToString("N");
		using SingleInstance first = new(id);
		using MultiFormApplicationContext c = new(first);
		List<string[]> received = new();
		c.ArgumentsReceived += (s, e) => received.Add(e.Arguments);

		Assert.That(c.Instance, Is.SameAs(first));
		Assert.That(first.IsListening, Is.True);

		using SingleInstance second = new(id);
		Assert.That(MultiFormApplicationContext.Forward(second, ["a.ltab", "b.ltab"]), Is.True);

		DateTime end = DateTime.Now.AddSeconds(5);
		while((received.Count == 0) && (DateTime.Now < end))
		{
			_Sync.RunPending();
			Thread.Sleep(20);
		}
		Assert.That(received, Has.Count.EqualTo(1));
		Assert.That(received[0], Is.EqualTo(new[] { "a.ltab", "b.ltab" }));
	}


	/// <summary>A context with a further instance does not listen.</summary>
	[Test]
	public void Constructor_NotFirstInstance()
	{
		string id = "robbiblubber.org/spellbook-tests/" + Guid.NewGuid().ToString("N");
		using SingleInstance first = new(id);
		using SingleInstance second = new(id);
		using MultiFormApplicationContext c = new(second);

		Assert.That(c.Instance, Is.Null);
		Assert.That(second.IsListening, Is.False);
	}


	/// <summary>Activating a minimized form restores it.</summary>
	[Test]
	public void ActivateForm_Restores()
	{
		Form a = _NewForm();
		a.Show();
		a.WindowState = FormWindowState.Minimized;

		MultiFormApplicationContext.ActivateForm(a);
		Assert.That(a.WindowState, Is.EqualTo(FormWindowState.Normal));
	}
}
