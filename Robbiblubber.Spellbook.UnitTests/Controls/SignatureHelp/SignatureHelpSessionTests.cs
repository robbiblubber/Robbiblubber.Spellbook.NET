namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;
using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests starting, updating and closing signature help sessions.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class SignatureHelpSessionTests: SignatureHelpSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>An opening parenthesis after a known function starts a session with the first parameter active.</summary>
	[Test]
	public void ParenthesisStartsSession()
	{
		_Text.Type("x = Max(");

		Assert.That(_Starting, Has.Count.EqualTo(1));
		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(SignatureHelpTrigger.TRIGGER_CHARACTER));
		Assert.That(_Starting[0].Context.TriggerCharacter, Is.EqualTo('('));
		Assert.That(_Starting[0].Context.Name, Is.EqualTo("Max"));
		Assert.That(_Provider.ActiveControl, Is.SameAs(_Text));
		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));
		Assert.That(_Parameter(), Is.EqualTo("int a"));
		Assert.That(_Selection!.SignatureIndex, Is.EqualTo(0));
		Assert.That(_Selection.ArgumentIndex, Is.EqualTo(0));
	}


	/// <summary>A separator moves to the next parameter without requesting the signatures again.</summary>
	[Test]
	public void SeparatorMovesParameter()
	{
		_Text.Type("Max(1, ");

		Assert.That(_Parameter(), Is.EqualTo("int b"));
		Assert.That(_Selection!.ArgumentIndex, Is.EqualTo(1));
		Assert.That(_Count("Populating"), Is.EqualTo(1));
	}


	/// <summary>The closing parenthesis ends the session.</summary>
	[Test]
	public void ClosingParenthesisEndsSession()
	{
		_Text.Type("Max(1, 2)");

		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CALL_LEFT));
		Assert.That(_Provider.ActiveControl, Is.Null);
		Assert.That(_Selection!.Signature, Is.Null);
	}


	/// <summary>A function without signatures does not show anything.</summary>
	[Test]
	public void UnknownFunctionEndsSession()
	{
		_Text.Type("Foo(");

		Assert.That(_Events, Is.EqualTo(new[] { "Starting", "Populating", "Closed:NO_SIGNATURES" }));
		Assert.That(_Provider.ActiveControl, Is.Null);
	}


	/// <summary>Trigger characters outside a named call do not start a session.</summary>
	[Test]
	public void TriggerOutsideCallDoesNothing()
	{
		_Text.Type("a, (1 + 2, x");

		Assert.That(_Events, Is.Empty);
	}


	/// <summary>A separator typed within a call starts a session.</summary>
	[Test]
	public void SeparatorStartsSession()
	{
		_Text.SetText("Max(1");

		_Text.Type(",");

		Assert.That(_Starting[0].Context.TriggerCharacter, Is.EqualTo(','));
		Assert.That(_Parameter(), Is.EqualTo("int b"));
	}


	/// <summary>Trigger characters can be overridden for a control.</summary>
	[Test]
	public void TriggerCharactersCanBeOverridden()
	{
		_Provider.SetTriggerCharacters(_Text, "(");
		_Text.SetText("Max(1");

		_Text.Type(",");
		Assert.That(_Starting, Is.Empty);

		_Text.Type(" Log(");
		Assert.That(_Starting, Has.Count.EqualTo(1));
	}


	/// <summary>Without auto start, only the shortcut keys start a session; outside a call they do nothing.</summary>
	[Test]
	public void ShortcutKeysStartSession()
	{
		_Provider.SetAutoStart(_Text, false);

		_Text.Type("Max(");
		Assert.That(_Starting, Is.Empty);

		Assert.That(_Text.Press(Keys.Control | Keys.Shift | Keys.Space), Is.True);
		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(SignatureHelpTrigger.SHORTCUT));

		_Provider.CloseSignatureHelp();
		_Text.SetText("abc");
		Assert.That(_Text.Press(Keys.Control | Keys.Shift | Keys.Space), Is.True);
		Assert.That(_Starting, Has.Count.EqualTo(1));
	}


	/// <summary>ShowSignatureHelp starts a session with the trigger MANUAL.</summary>
	[Test]
	public void ShowSignatureHelpStartsSession()
	{
		_Text.SetText("Log(x, 2", 6);

		_Provider.ShowSignatureHelp(_Text);

		Assert.That(_Starting[0].Context.Trigger, Is.EqualTo(SignatureHelpTrigger.MANUAL));
		Assert.That(_Parameter(), Is.EqualTo("base"));
	}


	/// <summary>Escape ends the session; typing in the same argument does not restart it, a separator does.</summary>
	[Test]
	public void EscapeEndsSession()
	{
		_Text.Type("Max(1");

		Assert.That(_Text.Press(Keys.Escape), Is.True);
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CANCELLED));

		_Text.Type("2");
		Assert.That(_Starting, Has.Count.EqualTo(1));

		_Text.Type(",");
		Assert.That(_Starting, Has.Count.EqualTo(2));
	}


	/// <summary>Escape is not handled without a session.</summary>
	[Test]
	public void EscapeWithoutSessionIsNotHandled()
	{
		Assert.That(_Text.Press(Keys.Escape), Is.False);
	}


	/// <summary>Up and Down switch between overloads and wrap around.</summary>
	[Test]
	public void ArrowKeysSwitchOverloads()
	{
		_Text.Type("Max(");

		Assert.That(_Text.Press(Keys.Down), Is.True);
		Assert.That(_Signature(), Is.EqualTo("Max(double a, double b)"));

		Assert.That(_Text.Press(Keys.Down), Is.True);
		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));

		Assert.That(_Text.Press(Keys.Up), Is.True);
		Assert.That(_Selection!.SignatureIndex, Is.EqualTo(1));
	}


	/// <summary>Up and Down are not handled with a single signature.</summary>
	[Test]
	public void ArrowKeysIgnoredForSingleSignature()
	{
		_Text.Type("Format(");

		Assert.That(_Text.Press(Keys.Down), Is.False);
	}


	/// <summary>The first signature that accepts the current argument is displayed automatically.</summary>
	[Test]
	public void OverloadFollowsArgumentCount()
	{
		_Text.Type("Round(x");
		Assert.That(_Signature(), Is.EqualTo("Round(value)"));

		_Text.Type(", ");
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits)"));

		_Text.Type("2, ");
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits, mode)"));
		Assert.That(_Parameter(), Is.EqualTo("mode"));

		_Text.Type("m, ");
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits, mode)"));
		Assert.That(_Parameter(), Is.Null);
	}


	/// <summary>A signature chosen with the arrow keys is kept when the argument changes.</summary>
	[Test]
	public void ChosenOverloadIsKept()
	{
		_Text.Type("Round(x");
		_Text.Press(Keys.Down);
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits)"));

		_Text.Type(", 2, m");
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits)"));
		Assert.That(_Parameter(), Is.Null);
	}


	/// <summary>A variadic parameter stays active for all further arguments.</summary>
	[Test]
	public void VariadicParameterStaysActive()
	{
		_Text.Type("Format(f, a, b, c");

		Assert.That(_Parameter(), Is.EqualTo("args"));
		Assert.That(_Selection!.ArgumentIndex, Is.EqualTo(3));
	}


	/// <summary>Nested calls show the innermost call; closing it returns to the outer call without a new session.</summary>
	[Test]
	public void NestedCallsAreFollowed()
	{
		_Text.Type("Round(Log(x, ");
		Assert.That(_Signature(), Is.EqualTo("Log(x, base)"));
		Assert.That(_Parameter(), Is.EqualTo("base"));

		_Text.Type("2), ");
		Assert.That(_Signature(), Is.EqualTo("Round(value, digits)"));
		Assert.That(_Parameter(), Is.EqualTo("digits"));

		Assert.That(_Count("Starting"), Is.EqualTo(1));
		Assert.That(_Populating.Select(m => m.Context.Name), Is.EqualTo(new[] { "Round", "Log", "Round" }));
		Assert.That(_Populating[1].Context.Trigger, Is.EqualTo(SignatureHelpTrigger.CALL_CHANGED));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>An unknown nested call hides the signature without ending the session.</summary>
	[Test]
	public void UnknownNestedCallKeepsSession()
	{
		_Text.Type("Max(Foo(");
		Assert.That(_Selection!.Signature, Is.Null);
		Assert.That(_Closed, Is.Empty);

		_Text.Type("1)");
		Assert.That(_Signature(), Is.EqualTo("Max(int a, int b)"));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Separators and parentheses in strings do not change the argument.</summary>
	[Test]
	public void StringsAreIgnored()
	{
		_Text.Type("Max(\"a, (b\", ");

		Assert.That(_Parameter(), Is.EqualTo("int b"));
		Assert.That(_Closed, Is.Empty);
	}


	/// <summary>Moving the caret between arguments updates the parameter; moving it out of the call ends the session.</summary>
	[Test]
	public void CaretMovementIsFollowed()
	{
		_Text.Type("Max(1, 2");
		Assert.That(_Parameter(), Is.EqualTo("int b"));

		_Text.Press(Keys.Left);
		_Text.Select(5, 0);
		_DoEvents();
		Assert.That(_Parameter(), Is.EqualTo("int a"));

		_Text.Press(Keys.Home);
		_Text.Select(0, 0);
		_DoEvents();
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CALL_LEFT));
	}


	/// <summary>Text changes that leave the call end the session.</summary>
	[Test]
	public void ExternalTextChangeIsFollowed()
	{
		_Text.Type("Max(1");

		_Text.Text = "abc";

		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.CALL_LEFT));
	}


	/// <summary>Changes while tracking is suspended are ignored until tracking resumes.</summary>
	[Test]
	public void SuspendedTrackingDefersUpdate()
	{
		_Text.Type("Max(1");

		using(_Provider.SuspendTracking(_Text))
		{
			_Text.SelectedText = ", ";
			Assert.That(_Parameter(), Is.EqualTo("int a"));
		}

		Assert.That(_Parameter(), Is.EqualTo("int b"));
	}


	/// <summary>A cancelled Starting event prevents the session.</summary>
	[Test]
	public void CancelledStartingPreventsSession()
	{
		_Provider.Starting += (_, e) => { e.Cancel = true; };

		_Text.Type("Max(");

		Assert.That(_Count("Populating"), Is.EqualTo(0));
		Assert.That(_Provider.ActiveControl, Is.Null);
	}


	/// <summary>The Populating event can add signatures and choose the displayed one, which is then kept.</summary>
	[Test]
	public void PopulatingCanChangeSignatures()
	{
		_Provider.Populating += (_, e) =>
		{
			e.Signatures.Add(new SignatureInfo("Max", new SignatureParameter("long a"), new SignatureParameter("long b"), new SignatureParameter("long c")));
			e.ActiveSignature = 2;
		};

		_Text.Type("Max(1, 2, ");

		Assert.That(_Signature(), Is.EqualTo("Max(long a, long b, long c)"));
		Assert.That(_Parameter(), Is.EqualTo("long c"));
	}


	/// <summary>If the Populating event removes all signatures, the session ends.</summary>
	[Test]
	public void PopulatingCanRemoveSignatures()
	{
		_Provider.Populating += (_, e) => { e.Signatures.Clear(); };

		_Text.Type("Max(");

		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.NO_SIGNATURES));
	}


	/// <summary>Source and parser can be set per control.</summary>
	[Test]
	public void SourceAndParserCanBeSetPerControl()
	{
		SignatureHelpItemSource source = new();

		source.Add("Max", new SignatureParameter("x"));
		_Provider.SetSource(_Text, source);
		_Provider.SetParser(_Text, new SignatureCallParser { ArgumentSeparator = ';' });
		_Provider.SetTriggerCharacters(_Text, "(;");

		_Text.Type("Max(1, 2; ");

		Assert.That(_Signature(), Is.EqualTo("Max(x)"));
		Assert.That(_Selection!.ArgumentIndex, Is.EqualTo(1));
	}


	/// <summary>A failing parser ends a running session with an error and prevents new sessions.</summary>
	[Test]
	public void ParserExceptionIsHandled()
	{
		InvalidOperationException exception = new("Parser failed.");
		_FailingParser parser = new();

		_Provider.Parser = parser;
		_Text.Type("Max(");
		Assert.That(_Starting, Has.Count.EqualTo(1));

		parser.Exception = exception;
		_Text.Type("1");
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.ERROR));
		Assert.That(_Closed[0].Exception, Is.SameAs(exception));

		_Text.Type(",");
		Assert.That(_Starting, Has.Count.EqualTo(1));
	}


	/// <summary>Starting a session in another control ends the session of the first control.</summary>
	[Test]
	public void OtherControlTakesOver()
	{
		TestTextBox other = new() { Name = "Other" };

		_Window.Controls.Add(other);
		_ = other.Handle;
		_Provider.SetSignatureHelpEnabled(other, true);

		_Text.Type("Max(");
		other.Type("Log(");

		Assert.That(_Closed[0].Control, Is.SameAs(_Text));
		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.FOCUS_LOST));
		Assert.That(_Provider.ActiveControl, Is.SameAs(other));
	}


	/// <summary>Disabling the signature help ends the session.</summary>
	[Test]
	public void DisablingEndsSession()
	{
		_Text.Type("Max(");

		_Provider.SetSignatureHelpEnabled(_Text, false);

		Assert.That(_Closed[0].Reason, Is.EqualTo(SignatureHelpCloseReason.MANUAL));
		_Text.Type("Log(");
		Assert.That(_Starting, Has.Count.EqualTo(1));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _FailingParser                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a parser that can be made to fail.</summary>
	private sealed class _FailingParser: ISignatureCallParser
	{
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // private members		                                                                                        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>The inner parser used when not failing.</summary>
        private readonly SignatureCallParser _Inner = new();



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>Gets or sets the exception associated with the parser failure.</summary>
		public Exception? Exception
		{
			get; set;
		}




		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] ISignatureCallParser                                                                             //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
        /// <inheritdoc/>
        public SignatureCall? Parse(string text, int caretIndex)
		{
			if(Exception is not null) { throw Exception; }
			return _Inner.Parse(text, caretIndex);
		}
	}
}
