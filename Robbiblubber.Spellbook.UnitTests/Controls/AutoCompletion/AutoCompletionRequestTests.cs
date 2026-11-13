namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests how sessions request, receive and apply source results: asynchronous results, cancellation, errors,
///          the Populating event, incomplete results and replace ranges.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionRequestTests: AutoCompletionSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Result of the pending request.</summary>
	private TaskCompletionSource<AutoCompletionResult?> _Pending = null!;

	/// <summary>Cancellation token of the last request.</summary>
	private CancellationToken _Token = CancellationToken.None;

	/// <summary>Number of requests.</summary>
	private int _Requests = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Uses an asynchronous source that completes when the test completes <see cref="_Pending"/>.</summary>
	private void _UseAsyncSource()
	{
		_Provider.Source = new AutoCompletionDelegateSource((_, n) =>
		{
			_Requests++;
			_Token = n;
			return new ValueTask<AutoCompletionResult?>(_Pending.Task);
		});
	}


	/// <summary>Completes the pending request.</summary>
	/// <param name="texts">Item texts.</param>
	private void _Complete(params string[] texts)
	{
		_Pending.SetResult(new AutoCompletionResult(_CreateItems(texts)));
		_Context.RunPending();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] AutoCompletionSessionTestBase                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override void SetUp()
	{
		base.SetUp();

		_Pending = new TaskCompletionSource<AutoCompletionResult?>();
		_Token = CancellationToken.None;
		_Requests = 0;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>An asynchronous result is applied when it arrives.</summary>
	[Test]
	public void AsyncResultIsApplied()
	{
		_UseAsyncSource();

		_Text.Type("Au");
		Assert.That(_Provider.ActiveControl, Is.SameAs(_Text));
		Assert.That(_Count("Populating"), Is.EqualTo(0));

		_Complete(_Countries);

		Assert.That(_Count("Populating"), Is.EqualTo(1));
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
		Assert.That(_Requests, Is.EqualTo(1));
	}


	/// <summary>Text typed while the request is pending filters the result; the source is not asked again.</summary>
	[Test]
	public void TextTypedWhilePendingFiltersResult()
	{
		_UseAsyncSource();

		_Text.Type("A");
		_Text.Type("ustri");
		_Complete(_Countries);

		Assert.That(_Requests, Is.EqualTo(1));
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));
		Assert.That(_Accepted, Is.Empty);
	}


	/// <summary>Closing a pending session cancels the request and discards its result.</summary>
	[Test]
	public void ClosingCancelsRequest()
	{
		_UseAsyncSource();

		_Text.Type("Au");
		_Provider.CloseCompletion();

		Assert.That(_Token.IsCancellationRequested, Is.True);
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.MANUAL));

		_Complete(_Countries);

		Assert.That(_Count("Populating"), Is.EqualTo(0));
		Assert.That(_Selection, Is.Null);
	}


	/// <summary>Escape during a pending request cancels it and suppresses the word.</summary>
	[Test]
	public void EscapeCancelsRequest()
	{
		_UseAsyncSource();

		_Text.Type("Au");
		_Text.Press(Keys.Escape);
		_Complete(_Countries);

		Assert.That(_Token.IsCancellationRequested, Is.True);
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CANCELLED));
		Assert.That(_Count("Populating"), Is.EqualTo(0));
	}


	/// <summary>A result is discarded if the caret has left the word in the meantime.</summary>
	[Test]
	public void ResultIsDiscardedAfterCaretMoved()
	{
		_UseAsyncSource();
		_Text.SetText("x ");

		_Text.Type("Au");
		_Text.Select(0, 0);
		_Complete(_Countries);

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CARET_MOVED));
		Assert.That(_Count("Populating"), Is.EqualTo(0));
	}


	/// <summary>A space typed during a pending request ends the session; the result is discarded.</summary>
	[Test]
	public void SpaceDuringRequestEndsSession()
	{
		_UseAsyncSource();

		_Text.Type("Au ");
		_Complete(_Countries);

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CARET_MOVED));
		Assert.That(_Token.IsCancellationRequested, Is.True);
		Assert.That(_Count("Populating"), Is.EqualTo(0));
	}


	/// <summary>An exception thrown by the source ends the session with an error.</summary>
	[Test]
	public void SourceExceptionEndsSession()
	{
		InvalidOperationException exception = new("Source failed.");

		_Provider.Source = new AutoCompletionDelegateSource(_ => throw exception);

		_Text.Type("A");

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.ERROR));
		Assert.That(_Closed[0].Exception, Is.SameAs(exception));
	}


	/// <summary>An exception of an asynchronous source ends the session with an error.</summary>
	[Test]
	public void AsyncSourceExceptionEndsSession()
	{
		InvalidOperationException exception = new("Source failed.");

		_UseAsyncSource();
		_Text.Type("A");

		_Pending.SetException(exception);
		_Context.RunPending();

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.ERROR));
		Assert.That(_Closed[0].Exception, Is.SameAs(exception));
	}


	/// <summary>An empty or missing result ends the session.</summary>
	[Test]
	public void EmptyResultEndsSession()
	{
		_SetItems();
		_Text.Type("A");
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.NO_ITEMS));

		_Provider.Source = new AutoCompletionDelegateSource(_ => null);
		_Text.Type(" B");
		Assert.That(_Closed[1].Reason, Is.EqualTo(AutoCompletionCloseReason.NO_ITEMS));

		_Provider.Source = null;
		_Text.Type(" C");
		Assert.That(_Closed[2].Reason, Is.EqualTo(AutoCompletionCloseReason.NO_ITEMS));
	}


	/// <summary>A source set for the control takes precedence over the provider source.</summary>
	[Test]
	public void ControlSourceTakesPrecedence()
	{
		_Provider.SetSource(_Text, new AutoCompletionItemSource(new[] { "Alpha", "Beta" }));

		_Text.Type("A");

		Assert.That(_SelectedText(), Is.EqualTo("Alpha"));
	}


	/// <summary>The Populating event can change items and settings.</summary>
	[Test]
	public void PopulatingCanChangeItems()
	{
		_Provider.Populating += (_, e) =>
		{
			e.Items.Clear();
			e.Items.Add(new AutoCompletionItem("Beta"));
			e.Items.Add(new AutoCompletionItem("Alphabet"));
			e.MatchMode = ListMatchMode.CONTAINS;
		};
		_Text.SetText("bet");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Closed, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("Beta"));
		_Text.Press(Keys.Down);
		Assert.That(_SelectedText(), Is.EqualTo("Alphabet"));
	}


	/// <summary>The Populating event receives the request context.</summary>
	[Test]
	public void PopulatingReceivesContext()
	{
		AutoCompletionContext? context = null;

		_Provider.Populating += (_, e) => { context = e.Context; };
		_Text.SetText("SELECT c.Na");

		_Provider.ShowCompletion(_Text);

		Assert.That(context, Is.Not.Null);
		Assert.That(context!.Word, Is.EqualTo("Na"));
		Assert.That(context.Qualifier, Is.EqualTo("c"));
		Assert.That(context.Line, Is.EqualTo("SELECT c.Na"));
		Assert.That(context.Trigger, Is.EqualTo(AutoCompletionTrigger.MANUAL));
	}


	/// <summary>A complete result is filtered locally; an incomplete result is requested again on every keystroke.</summary>
	[Test]
	public void IncompleteResultIsRequestedAgain()
	{
		bool incomplete = false;

		_Provider.Source = new AutoCompletionDelegateSource(_ =>
		{
			_Requests++;
			return new AutoCompletionResult(_CreateItems(_Countries)) { IsIncomplete = incomplete };
		});

		_Text.Type("Aus");
		Assert.That(_Requests, Is.EqualTo(1));

		_Provider.CloseCompletion();
		incomplete = true;
		_Requests = 0;
		_Text.SetText(string.Empty);
		_Text.Type("Aus");

		Assert.That(_Requests, Is.EqualTo(3));
		Assert.That(_Count("Starting"), Is.EqualTo(2));
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
	}


	/// <summary>An incomplete empty result keeps the session open.</summary>
	[Test]
	public void IncompleteEmptyResultKeepsSession()
	{
		_Provider.Source = new AutoCompletionDelegateSource(m => new AutoCompletionResult((m.Word.Length < 2) ? _CreateItems() : _CreateItems("Austria")) { IsIncomplete = true });

		_Text.Type("A");
		Assert.That(_Closed, Is.Empty);

		_Text.Type("u");
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));
	}


	/// <summary>The replace start of a result extends the replaced range and the filter text before the word.</summary>
	[Test]
	public void ReplaceStartExtendsRange()
	{
		_Provider.Source = new AutoCompletionDelegateSource(m => new AutoCompletionResult(_CreateItems("[Customers]", "[Order Details]")) { ReplaceStart = m.WordStart - 1 });
		_Text.SetText("FROM [Ord");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Text, Is.EqualTo("FROM [Order Details]"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(20));
	}


	/// <summary>The replace length of a result replaces text after the caret.</summary>
	[Test]
	public void ReplaceLengthReplacesTextAfterCaret()
	{
		_Provider.Source = new AutoCompletionDelegateSource(_ => new AutoCompletionResult(_CreateItems("Name", "Number")) { ReplaceLength = 3 });
		_Text.SetText("x.Nme + 1", 3);

		_Provider.ShowCompletion(_Text);
		Assert.That(_SelectedText(), Is.EqualTo("Name"));

		_Text.Press(Keys.Enter);

		Assert.That(_Text.Text, Is.EqualTo("x.Name + 1"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(6));
	}
}
