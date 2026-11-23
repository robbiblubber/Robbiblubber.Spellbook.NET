namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the source implementations <see cref="SignatureHelpItemSource"/> and <see cref="SignatureHelpDelegateSource"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class SignatureHelpSourceTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a context for a call.</summary>
	/// <param name="name">Function name.</param>
	/// <returns>Returns the context.</returns>
	private SignatureHelpContext _Context(string name)
	{
		return new SignatureHelpContext(_Text, name + "(", name.Length + 1, new SignatureCall(name, null, 0, name.Length, 0, name.Length + 1), SignatureHelpTrigger.MANUAL, '\0');
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Text = new TextBox();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Text.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The item source returns all overloads of a name in the order they have been added, synchronously.</summary>
	[Test]
	public void ItemSourceReturnsOverloads()
	{
		SignatureHelpItemSource source = new();
		SignatureInfo first = source.Add("Max", new SignatureParameter("int a"), new SignatureParameter("int b"));
		SignatureInfo second = source.Add("Max", new SignatureParameter("double a"), new SignatureParameter("double b"));
		ValueTask<SignatureHelpResult?> task;

		source.Add("Min", new SignatureParameter("int a"));
		task = source.GetSignaturesAsync(_Context("max"), CancellationToken.None);

		Assert.That(task.IsCompletedSuccessfully, Is.True);
		Assert.That(task.Result!.Signatures, Is.EqualTo(new[] { first, second }));
	}


	/// <summary>The item source compares names case-sensitively if configured and returns NULL for unknown names.</summary>
	[Test]
	public void ItemSourceRespectsCase()
	{
		SignatureHelpItemSource source = new(new[] { new SignatureInfo("Max") }) { CaseSensitive = true };

		Assert.That(source.GetSignaturesAsync(_Context("max"), CancellationToken.None).Result, Is.Null);
		Assert.That(source.GetSignaturesAsync(_Context("Max"), CancellationToken.None).Result!.Signatures, Has.Count.EqualTo(1));
		Assert.That(source.GetSignatures("Unknown"), Is.Empty);
	}


	/// <summary>The synchronous delegate source passes the context and returns the delegate result.</summary>
	[Test]
	public void SyncDelegateSourceReturnsResult()
	{
		SignatureHelpContext? received = null;
		SignatureHelpDelegateSource source = new(m =>
		{
			received = m;
			return new SignatureHelpResult(new[] { new SignatureInfo(m.Name) });
		});
		SignatureHelpContext context = _Context("fn");

		Assert.That(source.GetSignaturesAsync(context, CancellationToken.None).Result!.Signatures[0].Name, Is.EqualTo("fn"));
		Assert.That(received, Is.SameAs(context));
	}


	/// <summary>The asynchronous delegate source passes context and cancellation token.</summary>
	[Test]
	public void AsyncDelegateSourceReturnsResult()
	{
		using(CancellationTokenSource cancellation = new())
		{
			CancellationToken received = CancellationToken.None;
			SignatureHelpDelegateSource source = new(async (m, n) =>
			{
				received = n;
				await Task.CompletedTask;
				return new SignatureHelpResult(new[] { new SignatureInfo(m.Name) }) { ActiveSignature = 0 };
			});

			ValueTask<SignatureHelpResult?> task = source.GetSignaturesAsync(_Context("fn"), cancellation.Token);
			bool completed = task.IsCompletedSuccessfully;
			SignatureHelpResult? result = task.Result;

			Assert.That(completed, Is.True);
			Assert.That(received, Is.EqualTo(cancellation.Token));
			Assert.That(result!.ActiveSignature, Is.EqualTo(0));
		}
	}


	/// <summary>The delegate source requires a delegate.</summary>
	[Test]
	public void DelegateSourceRequiresDelegate()
	{
		Assert.That(() => new SignatureHelpDelegateSource((Func<SignatureHelpContext, SignatureHelpResult?>) null!), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new SignatureHelpDelegateSource((Func<SignatureHelpContext, CancellationToken, ValueTask<SignatureHelpResult?>>) null!), Throws.TypeOf<ArgumentNullException>());
	}
}
