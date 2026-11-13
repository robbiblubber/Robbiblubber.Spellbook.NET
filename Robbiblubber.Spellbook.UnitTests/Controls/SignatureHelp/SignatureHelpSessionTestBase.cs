namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;
using Robbiblubber.Spellbook.Controls;



/// <summary>This class provides a base for tests that run signature help sessions.</summary>
/// <remarks>Each test gets a new provider and a <see cref="TestTextBox"/> named "TestBox" on a hidden window. The source knows
///          <c>Max</c> (two overloads with two parameters), <c>Round</c> (overloads with one, two and three parameters), <c>Log</c>
///          (one and two parameters), <c>Format</c> (variadic) and <c>Now</c> (no parameters). All provider events are recorded; the
///          signature window is shown, but never activated. Asynchronous continuations are queued in a
///          <see cref="TestSynchronizationContext"/>.</remarks>
[Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public abstract class SignatureHelpSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected members                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Synchronization context.</summary>
	protected TestSynchronizationContext _Context = null!;

	/// <summary>Previous synchronization context.</summary>
	protected SynchronizationContext? _PreviousContext = null;

	/// <summary>Hidden window that hosts the text box.</summary>
	protected Form _Window = null!;

	/// <summary>Text box.</summary>
	protected TestTextBox _Text = null!;

	/// <summary>Signature help provider.</summary>
	protected SignatureHelpProvider _Provider = null!;

	/// <summary>Signature source.</summary>
	protected SignatureHelpItemSource _Source = null!;

	/// <summary>Recorded events, as event names; Closed events include the reason ("Closed:CALL_LEFT").</summary>
	protected readonly List<string> _Events = new();

	/// <summary>Recorded Starting event arguments.</summary>
	protected readonly List<SignatureHelpStartingEventArgs> _Starting = new();

	/// <summary>Recorded Populating event arguments.</summary>
	protected readonly List<SignatureHelpPopulatingEventArgs> _Populating = new();

	/// <summary>Recorded Closed event arguments.</summary>
	protected readonly List<SignatureHelpClosedEventArgs> _Closed = new();

	/// <summary>Arguments of the last SelectionChanged event.</summary>
	protected SignatureHelpSelectionChangedEventArgs? _Selection = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the number of recorded events with a name.</summary>
	/// <param name="name">Event name.</param>
	/// <returns>Returns the number of events.</returns>
	protected int _Count(string name)
	{
		return _Events.Count(m => (m == name));
	}


	/// <summary>Gets the displayed signature as text, NULL if no signature is displayed.</summary>
	/// <returns>Returns the signature text.</returns>
	protected string? _Signature()
	{
		return _Selection?.Signature?.ToString();
	}


	/// <summary>Gets the label of the active parameter, NULL if no parameter is active.</summary>
	/// <returns>Returns the label.</returns>
	protected string? _Parameter()
	{
		if((_Selection?.Signature is null) || (_Selection.ParameterIndex < 0)) { return null; }
		return _Selection.Signature.Parameters[_Selection.ParameterIndex].Label;
	}


	/// <summary>Processes pending window messages, such as deferred caret checks.</summary>
	protected void _DoEvents()
	{
		Application.DoEvents();
		_Context.RunPending();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public virtual void SetUp()
	{
		_PreviousContext = SynchronizationContext.Current;
		WindowsFormsSynchronizationContext.AutoInstall = false;
		_Context = new TestSynchronizationContext();
		SynchronizationContext.SetSynchronizationContext(_Context);

		_Events.Clear();
		_Starting.Clear();
		_Populating.Clear();
		_Closed.Clear();
		_Selection = null;

		_Window = new Form();
		_Text = new TestTextBox { Name = "TestBox" };
		_Window.Controls.Add(_Text);
		_ = _Window.Handle;
		_ = _Text.Handle;

		_Source = new SignatureHelpItemSource();
		_Source.Add("Max", new SignatureParameter("int a"), new SignatureParameter("int b"));
		_Source.Add("Max", new SignatureParameter("double a"), new SignatureParameter("double b"));
		_Source.Add("Round", new SignatureParameter("value"));
		_Source.Add("Round", new SignatureParameter("value"), new SignatureParameter("digits"));
		_Source.Add("Round", new SignatureParameter("value"), new SignatureParameter("digits"), new SignatureParameter("mode"));
		_Source.Add("Log", new SignatureParameter("x"));
		_Source.Add("Log", new SignatureParameter("x"), new SignatureParameter("base"));
		_Source.Add("Format", new SignatureParameter("format"), new SignatureParameter("args") { IsVariadic = true });
		_Source.Add("Now");

		_Provider = new SignatureHelpProvider { Source = _Source };
		_Provider.Starting += _Provider_Starting;
		_Provider.Populating += _Provider_Populating;
		_Provider.SelectionChanged += _Provider_SelectionChanged;
		_Provider.Closed += _Provider_Closed;
		_Provider.SetSignatureHelpEnabled(_Text, true);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public virtual void TearDown()
	{
		_Provider.Dispose();
		_Text.Dispose();
		_Window.Dispose();

		SynchronizationContext.SetSynchronizationContext(_PreviousContext);
		WindowsFormsSynchronizationContext.AutoInstall = true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Starting event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Starting(object? sender, SignatureHelpStartingEventArgs e)
	{
		_Events.Add("Starting");
		_Starting.Add(e);
	}


	/// <summary>Handles the Populating event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Populating(object? sender, SignatureHelpPopulatingEventArgs e)
	{
		_Events.Add("Populating");
		_Populating.Add(e);
	}


	/// <summary>Handles the SelectionChanged event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_SelectionChanged(object? sender, SignatureHelpSelectionChangedEventArgs e)
	{
		_Selection = e;
	}


	/// <summary>Handles the Closed event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Closed(object? sender, SignatureHelpClosedEventArgs e)
	{
		_Events.Add("Closed:" + e.Reason);
		_Closed.Add(e);
	}
}
