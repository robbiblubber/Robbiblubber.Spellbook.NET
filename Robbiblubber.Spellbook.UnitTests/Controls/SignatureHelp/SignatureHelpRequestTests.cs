namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;
using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests asynchronous signature requests, errors and the interaction with a linked auto completion provider.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class SignatureHelpRequestTests: SignatureHelpSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Result of the pending request.</summary>
	private TaskCompletionSource<SignatureHelpResult?> _Pending = null!;

	/// <summary>Cancellation token of the last request.</summary>
	private CancellationToken _Token = CancellationToken.None;

	/// <summary>Auto completion provider, NULL if not created by the test.</summary>
	private AutoCompletionProvider? _Completion = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Uses an asynchronous source that completes when the test completes <see cref="_Pending"/>.</summary>
	private void _UseAsyncSource()
	{
		_Provider.Source = new SignatureHelpDelegateSource((_, n) =>
		{
			_Token = n;
			return new ValueTask<SignatureHelpResult?>(_Pending.Task);
		});
	}


	/// <summary>Completes the pending request with the signatures of a function.</summary>
	/// <param name="name">Function name.</param>
	private void _Complete(string name)
	{
		_Pending.SetResult(new SignatureHelpResult(_Source.GetSignatures(name)));
		_Context.RunPending();
	}


	/// <summary>Creates an auto completion provider for the text box.</summary>
	/// <param name="link">Determines if the provider is linked to the signature help provider.</param>
	/// <param name="first">Determines if the auto completion is enabled before the signature help, so it handles keys first.</param>
	private void _CreateCompletion(bool link, bool first = true)
	{
		_Completion = new AutoCompletionProvider { Delay = 0 };
		_Completion.Source = new AutoCompletionItemSource(new IAutoCompletionItem[]
		{
			new AutoCompletionItem("Max") { InsertText = "Max()", CaretPosition = 4 }, new AutoCompletionItem("value"), new AutoCompletionItem("variant")
		});

		if(first)
		{
			_Provider.SetSignatureHelpEnabled(_Text, false);
			_Completion.SetAutoCompletionEnabled(_Text, true);
			_Provider.SetSignatureHelpEnabled(_Text, true);
		}
		else { _Completion.SetAutoCompletionEnabled(_Text, true); }

		if(link) { _Provider.AutoCompletion = _Completion; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] SignatureHelpSessionTestBase                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override void SetUp()
	{
		base.SetUp();

		_Pending = new TaskCompletionSource<SignatureHelpResult?>();
		_Token = CancellationToken.None;
		_Completion = null;
	}


	/// <inheritdoc/>
	public override void TearDown()
	{
		_Completion?.Dispose();
		base.TearDown();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>An asynchronous result is displayed when it arrives.</summary>
	[Test]
	public void AsyncResultIsDisplayed()
	{
		_UseAsyncSource();

		_Text.Type("Max(1, ");
		Assert.That(_Provider.ActiveControl, Is.SameAs(_Text));
		Assert.That(_Selection, Is.Null);

		_Complete("Max");

		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));
		Assert.That(_Parameter(), Is.EqualTo("int b"));
	}


	/// <summary>Leaving the call during the request ends the session and discards the result.</summary>
	[Test]
	public void LeavingCallDiscardsResult()
	{
		_UseAsyncSource();

		_Text.Type("Max(1)");
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CALL_LEFT));
		Assert.That(_Token.IsCancellationRequested, Is.True);

		_Complete("Max");
		Assert.That(_Count("Populating"), Is.EqualTo(0));
	}


	/// <summary>Moving into another call during the request requests the signatures of that call.</summary>
	[Test]
	public void CallChangeDuringRequestRequestsAgain()
	{
		TaskCompletionSource<SignatureHelpResult?> first;

		_UseAsyncSource();
		_Text.Type("Round(");
		first = _Pending;
		_Pending = new TaskCompletionSource<SignatureHelpResult?>();

		_Text.Type("Log(");
		first.SetResult(new SignatureHelpResult(_Source.GetSignatures("Round")));
		_Context.RunPending();
		Assert.That(_Count("Populating"), Is.EqualTo(0));

		_Complete("Log");
		Assert.That(_Signature(), Is.EqualTo("Log(x)"));
	}


	/// <summary>Escape during the request ends the session; it is not handled because no window is visible.</summary>
	[Test]
	public void EscapeDuringRequestEndsSession()
	{
		_UseAsyncSource();

		_Text.Type("Max(");

		Assert.That(_Text.Press(Keys.Escape), Is.False);
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CANCELLED));
		Assert.That(_Token.IsCancellationRequested, Is.True);
	}


	/// <summary>A failing source ends the session with an error, synchronously and asynchronously.</summary>
	[Test]
	public void SourceExceptionEndsSession()
	{
		InvalidOperationException exception = new("Source failed.");

		_Provider.Source = new SignatureHelpDelegateSource(_ => throw exception);
		_Text.Type("Max(");
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.ERROR));
		Assert.That(_Closed[0].Exception, Is.SameAs(exception));

		_UseAsyncSource();
		_Text.Type("1, Log(");
		_Pending.SetException(exception);
		_Context.RunPending();
		Assert.That(_Closed[1].Reason, Is.EqualTo(SignatureHelpCloseReason.ERROR));
	}


	/// <summary>Without a source, nothing is displayed.</summary>
	[Test]
	public void MissingSourceShowsNothing()
	{
		_Provider.Source = null;

		_Text.Type("Max(");

		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.NO_SIGNATURES));
	}


	/// <summary>Inserting an auto completion item with a parenthesis starts the signature help if the providers are linked.</summary>
	[Test]
	public void LinkedCompletionStartsSignatureHelp()
	{
		_CreateCompletion(true);

		_Text.Type("Ma");
		Assert.That(_Text.Press(Keys.Enter), Is.True);

		Assert.That(_Text.Text, Is.EqualTo("Max()"));
		Assert.That(_Starting, Has.Count.EqualTo(1));
		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(SignatureHelpTrigger.AUTO_COMPLETION));
		Assert.That(_Parameter(), Is.EqualTo("int a"));
	}


	/// <summary>Without the link, inserting an auto completion item does not start the signature help.</summary>
	[Test]
	public void UnlinkedCompletionDoesNotStart()
	{
		_CreateCompletion(false);

		_Text.Type("Ma");
		_Text.Press(Keys.Enter);

		Assert.That(_Text.Text, Is.EqualTo("Max()"));
		Assert.That(_Starting, Is.Empty);
	}


	/// <summary>While the linked auto completion list is open, Escape and the arrow keys go to the list, regardless of the order
	///          in which the providers handle keys.</summary>
	/// <param name="completionFirst">Determines if the auto completion handles keys first.</param>
	[TestCase(true)]
	[TestCase(false)]
	public void OpenCompletionListTakesKeys(bool completionFirst)
	{
		_CreateCompletion(true, completionFirst);

		_Text.Type("Max(v");
		Assert.That(_Completion!.ActiveControl, Is.SameAs(_Text));
		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));

		_Text.Press(Keys.Down);
		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));

		_Text.Press(Keys.Escape);
		Assert.That(_Completion.ActiveControl, Is.Null);
		Assert.That(_Closed, Is.Empty);

		_Text.Press(Keys.Down);
		Assert.That(_Signature(), Is.EqualTo("Max(double a, double b)"));

		_Text.Press(Keys.Escape);
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CANCELLED));
	}


	/// <summary>The signature window inherits the colors of the linked auto completion provider unless its own colors are set.</summary>
	[Test]
	public void ColorsAreInherited()
	{
		Color inherited = Color.FromArgb(37, 37, 38), own = Color.FromArgb(1, 2, 3);
		Form? window;

		_CreateCompletion(true);
		_Completion!.ListBackColor = inherited;

		_Text.Type("Max(");
		window = Application.OpenForms.OfType<_SignatureHelpWindow>().FirstOrDefault(m => m.Visible);
		Assert.That(window, Is.Not.Null);
		Assert.That(window!.BackColor, Is.EqualTo(inherited));

		_Provider.WindowBackColor = own;
		_Text.Type("1");
		Assert.That(window.BackColor, Is.EqualTo(own));
	}
}
