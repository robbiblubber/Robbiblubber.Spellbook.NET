namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class provides a base for tests that run auto completion sessions.</summary>
/// <remarks>Each test gets a new provider and a <see cref="TestTextBox"/> named "TestBox" on a hidden window. The typing delay is 0, so
///          sessions start synchronously. All provider events are recorded in <see cref="_Events"/>; the popup window is shown, but
///          never activated. Asynchronous continuations are queued in a <see cref="TestSynchronizationContext"/>.</remarks>
[Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public abstract class AutoCompletionSessionTestBase
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected constants                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Name of the test text box, used as usage history scope.</summary>
	protected const string _SCOPE = "TestBox";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected static members                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default items.</summary>
	protected static readonly string[] _Countries = new string[] { "Argentina", "Australia", "Austria", "Belgium", "Brazil" };



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

	/// <summary>Auto completion provider.</summary>
	protected AutoCompletionProvider _Provider = null!;

	/// <summary>Recorded events, as event names; Closed events include the reason ("Closed:ACCEPTED").</summary>
	protected readonly List<string> _Events = new();

	/// <summary>Recorded Starting event arguments.</summary>
	protected readonly List<AutoCompletionStartingEventArgs> _Starting = new();

	/// <summary>Recorded Closed event arguments.</summary>
	protected readonly List<AutoCompletionClosedEventArgs> _Closed = new();

	/// <summary>Recorded Accepted event arguments.</summary>
	protected readonly List<AutoCompletionAcceptedEventArgs> _Accepted = new();

	/// <summary>Item of the last SelectionChanged event.</summary>
	protected IAutoCompletionItem? _Selection = null;

	/// <summary>Number of SelectionChanged events.</summary>
	protected int _SelectionCount = 0;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected static methods                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates items.</summary>
	/// <param name="texts">Item texts.</param>
	/// <returns>Returns the items.</returns>
	protected static IAutoCompletionItem[] _CreateItems(params string[] texts)
	{
		return texts.Select(m => (IAutoCompletionItem) new AutoCompletionItem(m)).ToArray();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the provider source to a fixed item list.</summary>
	/// <param name="texts">Item texts.</param>
	protected void _SetItems(params string[] texts)
	{
		_Provider.Source = new AutoCompletionItemSource(texts);
	}


	/// <summary>Gets the number of recorded events with a name.</summary>
	/// <param name="name">Event name.</param>
	/// <returns>Returns the number of events.</returns>
	protected int _Count(string name)
	{
		return _Events.Count(m => (m == name));
	}


	/// <summary>Gets the text of the selected item, NULL if no item is selected.</summary>
	/// <returns>Returns the item text.</returns>
	protected string? _SelectedText()
	{
		return _Selection?.Text;
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
		_Closed.Clear();
		_Accepted.Clear();
		_Selection = null;
		_SelectionCount = 0;

		_Window = new Form();
		_Text = new TestTextBox { Name = _SCOPE };
		_Window.Controls.Add(_Text);
		_ = _Window.Handle;
		_ = _Text.Handle;

		_Provider = new AutoCompletionProvider { Delay = 0 };
		_Provider.Starting += _Provider_Starting;
		_Provider.Populating += _Provider_Populating;
		_Provider.Accepting += _Provider_Accepting;
		_Provider.Accepted += _Provider_Accepted;
		_Provider.Closed += _Provider_Closed;
		_Provider.SelectionChanged += _Provider_SelectionChanged;

		_Provider.SetAutoCompletionEnabled(_Text, true);
		_SetItems(_Countries);
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
	private void _Provider_Starting(object? sender, AutoCompletionStartingEventArgs e)
	{
		_Events.Add("Starting");
		_Starting.Add(e);
	}


	/// <summary>Handles the Populating event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Populating(object? sender, AutoCompletionPopulatingEventArgs e)
	{
		_Events.Add("Populating");
	}


	/// <summary>Handles the Accepting event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Accepting(object? sender, AutoCompletionAcceptingEventArgs e)
	{
		_Events.Add("Accepting");
	}


	/// <summary>Handles the Accepted event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Accepted(object? sender, AutoCompletionAcceptedEventArgs e)
	{
		_Events.Add("Accepted");
		_Accepted.Add(e);
	}


	/// <summary>Handles the Closed event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_Closed(object? sender, AutoCompletionClosedEventArgs e)
	{
		_Events.Add("Closed:" + e.Reason);
		_Closed.Add(e);
	}


	/// <summary>Handles the SelectionChanged event of the provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Provider_SelectionChanged(object? sender, AutoCompletionSelectionChangedEventArgs e)
	{
		_Selection = e.Item;
		_SelectionCount++;
	}
}
