namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests starting, accepting and closing auto completion sessions.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionSessionTests: AutoCompletionSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Typing a word character starts a session; the context describes the typed word.</summary>
	[Test]
	public void TypingStartsSession()
	{
		_Text.Type("A");

		Assert.That(_Starting, Has.Count.EqualTo(1));
		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(AutoCompletionTrigger.TYPING));
		Assert.That(_Starting[0].Context.TriggerCharacter, Is.EqualTo('A'));
		Assert.That(_Starting[0].Context.Word, Is.EqualTo("A"));
		Assert.That(_Provider.ActiveControl, Is.SameAs(_Text));
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));
	}


	/// <summary>Typing on in the same word continues the session.</summary>
	[Test]
	public void TypingContinuesSession()
	{
		_Text.Type("Austri");

		Assert.That(_Count("Starting"), Is.EqualTo(1));
		Assert.That(_Closed, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));
	}


	/// <summary>A session starts only when the word has the minimum length.</summary>
	[Test]
	public void MinimumWordLengthDelaysStart()
	{
		_Provider.MinimumWordLength = 3;

		_Text.Type("Au");
		Assert.That(_Starting, Is.Empty);

		_Text.Type("s");
		Assert.That(_Starting, Has.Count.EqualTo(1));
		Assert.That(_Starting[0].Context.Word, Is.EqualTo("Aus"));
	}


	/// <summary>Non-word characters do not start a session and end a running session.</summary>
	[Test]
	public void NonWordCharacterEndsSession()
	{
		_Text.Type("Au ");

		Assert.That(_Events, Is.EqualTo(new[] { "Starting", "Populating", "Closed:CARET_MOVED" }));
		Assert.That(_Provider.ActiveControl, Is.Null);
		Assert.That(_Selection, Is.Null);
	}


	/// <summary>Word characters configured for the control are part of the word.</summary>
	[Test]
	public void WordCharactersExtendWord()
	{
		_SetItems("C", "C#", "C++");
		_Provider.SetWordCharacters(_Text, "#+");

		_Text.Type("C+");

		Assert.That(_Closed, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("C++"));
		Assert.That(_Text.Press(Keys.Enter), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("C++"));
	}


	/// <summary>A trigger character starts a session even if auto start is disabled; the qualifier is the preceding word chain.</summary>
	[Test]
	public void TriggerCharacterStartsSession()
	{
		_Provider.SetAutoStart(_Text, false);

		_Text.Type("dbo.Sales.");

		Assert.That(_Starting, Has.Count.EqualTo(2));
		Assert.That(_Starting[1].Context.Trigger, Is.EqualTo(AutoCompletionTrigger.TRIGGER_CHARACTER));
		Assert.That(_Starting[1].Context.TriggerCharacter, Is.EqualTo('.'));
		Assert.That(_Starting[1].Context.Qualifier, Is.EqualTo("dbo.Sales"));
		Assert.That(_Starting[1].Context.Word, Is.Empty);
	}


	/// <summary>With auto start disabled, typing does not start a session, but the shortcut keys do.</summary>
	[Test]
	public void ShortcutKeysStartSession()
	{
		_Provider.SetAutoStart(_Text, false);

		_Text.Type("Aus");
		Assert.That(_Starting, Is.Empty);

		Assert.That(_Text.Press(Keys.Control | Keys.Space), Is.True);
		Assert.That(_Starting, Has.Count.EqualTo(1));
		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(AutoCompletionTrigger.SHORTCUT));
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
	}


	/// <summary>The shortcut keys can be changed per control.</summary>
	[Test]
	public void ShortcutKeysCanBeChanged()
	{
		_Provider.SetShortcutKeys(_Text, Keys.Control | Keys.J);

		Assert.That(_Text.Press(Keys.Control | Keys.Space), Is.False);
		Assert.That(_Starting, Is.Empty);

		Assert.That(_Text.Press(Keys.Control | Keys.J), Is.True);
		Assert.That(_Starting, Has.Count.EqualTo(1));
	}


	/// <summary>An explicit start with an empty word shows all items with the first one selected.</summary>
	[Test]
	public void ExplicitStartSelectsFirstItem()
	{
		_Provider.ShowCompletion(_Text);

		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(AutoCompletionTrigger.MANUAL));
		Assert.That(_SelectedText(), Is.EqualTo("Argentina"));
	}


	/// <summary>An explicit start with a single match inserts it directly.</summary>
	[Test]
	public void ExplicitStartAcceptsSingleMatch()
	{
		_Text.SetText("I live in Austri");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Events, Is.EqualTo(new[] { "Starting", "Populating", "Accepting", "Closed:ACCEPTED", "Accepted" }));
		Assert.That(_Text.Text, Is.EqualTo("I live in Austria"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(17));
		Assert.That(_Accepted[0].InsertStart, Is.EqualTo(10));
		Assert.That(_Provider.ActiveControl, Is.Null);
	}


	/// <summary>With several matches, an explicit start shows the list.</summary>
	[Test]
	public void ExplicitStartShowsSeveralMatches()
	{
		_Text.SetText("Aus");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Accepted, Is.Empty);
		Assert.That(_Closed, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("Australia"));
	}


	/// <summary>Single matches are not inserted directly if disabled, and never while typing.</summary>
	[Test]
	public void SingleMatchIsShownIfNotAccepted()
	{
		_Text.Type("Austri");
		Assert.That(_Accepted, Is.Empty);

		_Provider.CloseCompletion();
		_Provider.AcceptSingleMatch = false;
		_Provider.ShowCompletion(_Text);

		Assert.That(_Accepted, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));
	}


	/// <summary>Enter inserts the selected item and replaces the typed word; text after the caret is kept.</summary>
	[Test]
	public void EnterAcceptsSelectedItem()
	{
		_Text.SetText(" xyz", 0);

		_Text.Type("Au");

		Assert.That(_Text.Press(Keys.Enter), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("Australia xyz"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(9));
		Assert.That(_Accepted[0].InsertText, Is.EqualTo("Australia"));
		Assert.That(_Accepted[0].InsertStart, Is.EqualTo(0));
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.ACCEPTED));
	}


	/// <summary>Tab inserts the selected item; if disabled, it closes the session and is not handled.</summary>
	[Test]
	public void TabAcceptsSelectedItem()
	{
		_Text.Type("Au");
		Assert.That(_Text.Press(Keys.Tab), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("Australia"));

		_Provider.AcceptOnTab = false;
		_Text.SetText(string.Empty);
		_Text.Type("Au");

		Assert.That(_Text.Press(Keys.Tab), Is.False);
		Assert.That(_Text.Text, Is.EqualTo("Au"));
		Assert.That(_Closed[^1].Reason, Is.EqualTo(AutoCompletionCloseReason.CANCELLED));
	}


	/// <summary>Enter with modifiers does not accept.</summary>
	[Test]
	public void EnterWithModifiersDoesNotAccept()
	{
		_Text.Type("Au");

		Assert.That(_Text.Press(Keys.Control | Keys.Enter), Is.False);
		Assert.That(_Accepted, Is.Empty);
	}


	/// <summary>Insert text and caret position of the item are applied.</summary>
	[Test]
	public void InsertTextAndCaretPositionAreApplied()
	{
		_Provider.Source = new AutoCompletionItemSource(new IAutoCompletionItem[] { new AutoCompletionItem("COUNT") { InsertText = "COUNT()", CaretPosition = 6 } });
		_Text.SetText("SELECT CO");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Text, Is.EqualTo("SELECT COUNT()"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(13));
	}


	/// <summary>The Accepting event can change the inserted text and the replaced range.</summary>
	[Test]
	public void AcceptingCanChangeInsertion()
	{
		_Provider.Accepting += (_, e) =>
		{
			e.InsertText = "[" + e.Item.Text + "]";
			e.CaretPosition = 1;
		};
		_Text.SetText("Austri");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Text, Is.EqualTo("[Austria]"));
		Assert.That(_Text.SelectionStart, Is.EqualTo(1));
		Assert.That(_Accepted[0].InsertText, Is.EqualTo("[Austria]"));
	}


	/// <summary>If the Accepting event is cancelled, the text is unchanged and the session stays open with the list shown.</summary>
	[Test]
	public void CancelledAcceptingKeepsSession()
	{
		_Provider.Accepting += (_, e) => { e.Cancel = true; };
		_Text.SetText("Austri");

		_Provider.ShowCompletion(_Text);

		Assert.That(_Text.Text, Is.EqualTo("Austri"));
		Assert.That(_Closed, Is.Empty);
		Assert.That(_SelectedText(), Is.EqualTo("Austria"));

		Assert.That(_Text.Press(Keys.Enter), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("Austri"));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Escape closes the session; typing on in the same word does not reopen it.</summary>
	[Test]
	public void EscapeSuppressesWord()
	{
		_Text.Type("Au");

		Assert.That(_Text.Press(Keys.Escape), Is.True);
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CANCELLED));

		_Text.Type("st");
		Assert.That(_Count("Starting"), Is.EqualTo(1));
	}


	/// <summary>After Escape, a new word, deleting the word or the shortcut keys start a session again.</summary>
	[Test]
	public void SuppressionEndsWithWord()
	{
		_Text.Type("Au");
		_Text.Press(Keys.Escape);

		_Text.Type(" B");
		Assert.That(_Count("Starting"), Is.EqualTo(2));

		_Text.Press(Keys.Escape);
		_Text.Type("\b\bA");
		Assert.That(_Count("Starting"), Is.EqualTo(3));

		_Text.Press(Keys.Escape);
		_Text.Press(Keys.Control | Keys.Space);
		Assert.That(_Count("Starting"), Is.EqualTo(4));
	}


	/// <summary>Escape also closes a session whose list is hidden because nothing matches.</summary>
	[Test]
	public void EscapeClosesHiddenSession()
	{
		_Text.Type("Ax");
		Assert.That(_Selection, Is.Null);
		Assert.That(_Closed, Is.Empty);

		_Text.Press(Keys.Escape);

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CANCELLED));
	}


	/// <summary>A text change that was not typed closes the session.</summary>
	[Test]
	public void ExternalTextChangeClosesSession()
	{
		_Text.Type("Au");

		_Text.Text = "Belgium";

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.TEXT_CHANGED));
	}


	/// <summary>A TextChanged event without an actual change (formatting only) does not close the session.</summary>
	[Test]
	public void FormattingChangeKeepsSession()
	{
		_Text.Type("Au");

		_Text.RaiseTextChanged();

		Assert.That(_Closed, Is.Empty);
		Assert.That(_Text.Press(Keys.Enter), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("Australia"));
	}


	/// <summary>Changes while tracking is suspended are ignored; the session continues if the caret is still in the word.</summary>
	[Test]
	public void SuspendedTrackingIgnoresChanges()
	{
		_Text.Type("Au");

		using(_Provider.SuspendTracking(_Text))
		{
			_Text.SelectedText = " xyz";
			_Text.Select(2, 0);
		}

		Assert.That(_Closed, Is.Empty);
		Assert.That(_Text.Press(Keys.Enter), Is.True);
		Assert.That(_Text.Text, Is.EqualTo("Australia xyz"));
	}


	/// <summary>After tracking resumes, the session ends if the word has moved.</summary>
	[Test]
	public void ResumedTrackingChecksCaret()
	{
		_Text.Type("Au");

		using(_Provider.SuspendTracking(_Text))
		{
			_Text.Select(0, 0);
			_Text.SelectedText = "x ";
			_Text.Select(4, 0);

			Assert.That(_Closed, Is.Empty);
		}

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CARET_MOVED));
	}


	/// <summary>Suspensions can be nested; tracking resumes with the outermost one.</summary>
	[Test]
	public void SuspensionsAreNested()
	{
		_Text.Type("Au");

		using(_Provider.SuspendTracking(_Text))
		{
			using(_Provider.SuspendTracking(_Text)) {}

			_Text.SetText("x Au");
			Assert.That(_Closed, Is.Empty);
		}

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CARET_MOVED));
	}


	/// <summary>Moving the caret within the word keeps the session; moving it out of the word closes it.</summary>
	[Test]
	public void CaretMovementIsChecked()
	{
		_Text.SetText("x ");
		_Text.Type("Aus");

		_Text.Press(Keys.Left);
		_Text.Select(4, 0);
		_DoEvents();
		Assert.That(_Closed, Is.Empty);

		_Text.Press(Keys.Left);
		_Text.Select(1, 0);
		_DoEvents();
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.CARET_MOVED));
	}


	/// <summary>CloseCompletion ends the session.</summary>
	[Test]
	public void CloseCompletionEndsSession()
	{
		_Text.Type("Au");

		_Provider.CloseCompletion();

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.MANUAL));
		Assert.That(_Closed[0].Control, Is.SameAs(_Text));
		Assert.That(_Provider.ActiveControl, Is.Null);
		Assert.That(_Selection, Is.Null);
	}


	/// <summary>Disabling auto completion ends the session and stops starting new ones.</summary>
	[Test]
	public void DisablingEndsSession()
	{
		_Text.Type("Au");

		_Provider.SetAutoCompletionEnabled(_Text, false);
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.MANUAL));

		_Text.Type("s");
		Assert.That(_Count("Starting"), Is.EqualTo(1));
	}


	/// <summary>Starting a session in another control closes the session of the first control.</summary>
	[Test]
	public void OtherControlTakesOver()
	{
		TestTextBox other = new() { Name = "Other" };

		_Window.Controls.Add(other);
		_ = other.Handle;
		_Provider.SetAutoCompletionEnabled(other, true);

		_Text.Type("Au");
		_Provider.ShowCompletion(other);

		Assert.That(_Closed[0].Control, Is.SameAs(_Text));
		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.FOCUS_LOST));
		Assert.That(_Provider.ActiveControl, Is.SameAs(other));
	}


	/// <summary>Disposing the control during a session ends the session.</summary>
	[Test]
	public void DisposingControlEndsSession()
	{
		_Text.Type("Au");

		_Text.Dispose();

		Assert.That(_Closed[0].Reason, Is.EqualTo(AutoCompletionCloseReason.MANUAL));
		Assert.That(_Provider.ActiveControl, Is.Null);
	}


	/// <summary>A cancelled Starting event does not start a session.</summary>
	[Test]
	public void CancelledStartingPreventsSession()
	{
		_Provider.Starting += (_, e) => { e.Cancel = true; };

		_Text.Type("Au");

		Assert.That(_Count("Starting"), Is.EqualTo(2));
		Assert.That(_Count("Populating"), Is.EqualTo(0));
		Assert.That(_Closed, Is.Empty);
		Assert.That(_Provider.ActiveControl, Is.Null);
	}
}
