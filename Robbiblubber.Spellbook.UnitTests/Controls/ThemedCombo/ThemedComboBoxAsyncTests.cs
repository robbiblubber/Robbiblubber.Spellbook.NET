namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests asynchronous search sources and the search delay of the <see cref="ThemedComboBox"/> control.</summary>
/// <remarks>The tests show a window outside the visible screen area, because the list only opens for visible controls.</remarks>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class ThemedComboBoxAsyncTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private classes                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a search source whose results are completed by the test.</summary>
	private sealed class PendingSource: IAutoCompletionSource
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the pending requests.</summary>
		public List<TaskCompletionSource<AutoCompletionResult?>> Requests
		{
			get;
		} = new();



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Completes a request.</summary>
		/// <param name="index">Request index.</param>
		/// <param name="texts">Result texts.</param>
		public void Complete(int index, params string[] texts)
		{
			Requests[index].SetResult(new AutoCompletionResult(texts.Select(m => new AutoCompletionItem(m))));
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IAutoCompletionSource                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken)
		{
			TaskCompletionSource<AutoCompletionResult?> rval = new();

			Requests.Add(rval);
			return new ValueTask<AutoCompletionResult?>(rval.Task);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test window.</summary>
	private Form _Window = null!;

	/// <summary>Tested control.</summary>
	private TestThemedComboBox _Combo = null!;

	/// <summary>Search source.</summary>
	private PendingSource _Source = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the texts of the displayed search results.</summary>
	private string[] _Results
	{
		get { return _Combo._DisplayedSearchResults?.Select(m => m.Text).ToArray() ?? Array.Empty<string>(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Processes pending messages until a condition is met.</summary>
	/// <param name="condition">Condition.</param>
	private static void _PumpUntil(Func<bool> condition)
	{
		for(int i = 0; (i < 100) && (!condition()); i++)
		{
			Application.DoEvents();
			Thread.Sleep(5);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Window = new Form { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false };
		_Source = new PendingSource();
		_Combo = new TestThemedComboBox { DropDownStyle = ThemedComboBoxStyle.COMBO, SearchSource = _Source };

		_Window.Controls.Add(_Combo);
		_Window.Show();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Combo.Dispose();
		_Window.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>While the source is loading, the list shows the loading indicator; then it shows the results.</summary>
	[Test]
	public void ShowsLoading()
	{
		_Combo._TextBox!.Text = "be";

		Assert.That(_Combo.DroppedDown, Is.True);
		Assert.That(_Combo._DropDownList!.IsLoading, Is.True);

		_Source.Complete(0, "Berlin", "Bern", "Vienna");
		_PumpUntil(() => !_Combo._DropDownList.IsLoading);

		Assert.That(_Combo._DropDownList.IsLoading, Is.False);
		Assert.That(_Results, Is.EqualTo(new[] { "Berlin", "Bern" }));
	}


	/// <summary>Results of obsolete requests are discarded.</summary>
	[Test]
	public void DiscardsObsoleteResults()
	{
		_Combo._TextBox!.Text = "b";
		_Combo._TextBox.Text = "bern";

		_Source.Complete(1, "Bern");
		_PumpUntil(() => _Results.Length > 0);
		Assert.That(_Results, Is.EqualTo(new[] { "Bern" }));

		_Source.Complete(0, "Berlin", "Bern", "Bratislava");
		_PumpUntil(() => false);
		Assert.That(_Results, Is.EqualTo(new[] { "Bern" }));
	}


	/// <summary>Escape cancels a loading search.</summary>
	[Test]
	public void CancelsLoadingSearch()
	{
		_Combo._TextBox!.Text = "be";
		_Combo.PressKey(Keys.Escape);

		Assert.That(_Combo.DroppedDown, Is.False);

		_Source.Complete(0, "Berlin");
		_PumpUntil(() => false);
		Assert.That(_Combo.DroppedDown, Is.False);
	}


	/// <summary>With a search delay, typing does not search immediately.</summary>
	[Test]
	public void DelaysSearch()
	{
		_Combo.SearchDelay = 5000;
		_Combo._TextBox!.Text = "be";

		Assert.That(_Combo._SearchPending, Is.True);
		Assert.That(_Source.Requests, Is.Empty);

		_Combo._TextBox.Text = "ber";
		_Combo._RunPendingSearch();

		Assert.That(_Source.Requests, Has.Count.EqualTo(1));
		Assert.That(_Combo.DroppedDown, Is.True);
	}
}
