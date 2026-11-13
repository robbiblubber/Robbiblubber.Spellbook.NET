namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the source implementations <see cref="AutoCompletionItemSource"/> and <see cref="AutoCompletionDelegateSource"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionSourceTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;

	/// <summary>Context.</summary>
	private AutoCompletionContext _Context = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Text = new TextBox();
		_Context = new AutoCompletionContext(_Text, "Au", 2, 0, string.Empty, AutoCompletionTrigger.MANUAL, '\0');
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

	/// <summary>The item source creates items from texts and returns them synchronously.</summary>
	[Test]
	public void ItemSourceReturnsItemsSynchronously()
	{
		AutoCompletionItemSource source = new(new[] { "Austria", "Australia" });
		ValueTask<AutoCompletionResult?> task = source.GetItemsAsync(_Context, CancellationToken.None);

		Assert.That(task.IsCompletedSuccessfully, Is.True);
		Assert.That(task.Result!.Items.Select(m => m.Text), Is.EqualTo(new[] { "Austria", "Australia" }));
		Assert.That(task.Result.IsIncomplete, Is.False);
	}


	/// <summary>Items added to the item source after construction are returned by later requests.</summary>
	[Test]
	public void ItemSourceReturnsAddedItems()
	{
		AutoCompletionItemSource source = new();
		AutoCompletionItem item = source.Add("Customers", "table");

		source.Items.Add(new AutoCompletionItem("Orders"));

		Assert.That(item.ImageKey, Is.EqualTo("table"));
		Assert.That(source.GetItemsAsync(_Context, CancellationToken.None).Result!.Items.Select(m => m.Text), Is.EqualTo(new[] { "Customers", "Orders" }));
		Assert.That(new AutoCompletionItemSource(new IAutoCompletionItem[] { item }).Items, Is.EqualTo(new[] { item }));
	}


	/// <summary>The synchronous delegate source passes the context and returns the delegate result.</summary>
	[Test]
	public void SyncDelegateSourceReturnsResult()
	{
		AutoCompletionContext? received = null;
		AutoCompletionDelegateSource source = new(m =>
		{
			received = m;
			return new AutoCompletionResult(new[] { new AutoCompletionItem(m.Word + "stria") });
		});
		ValueTask<AutoCompletionResult?> task = source.GetItemsAsync(_Context, CancellationToken.None);

		Assert.That(task.IsCompletedSuccessfully, Is.True);
		Assert.That(received, Is.SameAs(_Context));
		Assert.That(task.Result!.Items[0].Text, Is.EqualTo("Austria"));
	}


	/// <summary>The asynchronous delegate source passes context and cancellation token.</summary>
	/// <remarks>The delegate completes without yielding: Windows Forms installs its synchronization context on this thread, and a
	///          blocking wait for a posted continuation would never end without a message loop.</remarks>
	[Test]
	public void AsyncDelegateSourceReturnsResult()
	{
		using(CancellationTokenSource cancellation = new())
		{
			CancellationToken received = CancellationToken.None;
			AutoCompletionDelegateSource source = new(async (m, n) =>
			{
				received = n;
				await Task.CompletedTask;
				return new AutoCompletionResult(new[] { new AutoCompletionItem(m.Word) }) { IsIncomplete = true };
			});

			ValueTask<AutoCompletionResult?> task = source.GetItemsAsync(_Context, cancellation.Token);
			bool completed = task.IsCompletedSuccessfully;
			AutoCompletionResult? result = task.Result;

			Assert.That(completed, Is.True);
			Assert.That(received, Is.EqualTo(cancellation.Token));
			Assert.That(result!.Items[0].Text, Is.EqualTo("Au"));
			Assert.That(result.IsIncomplete, Is.True);
		}
	}


	/// <summary>The delegate source may return NULL.</summary>
	[Test]
	public void DelegateSourceMayReturnNull()
	{
		AutoCompletionDelegateSource source = new(_ => null);

		Assert.That(source.GetItemsAsync(_Context, CancellationToken.None).Result, Is.Null);
	}


	/// <summary>The delegate source requires a delegate.</summary>
	[Test]
	public void DelegateSourceRequiresDelegate()
	{
		Assert.That(() => new AutoCompletionDelegateSource((Func<AutoCompletionContext, AutoCompletionResult?>) null!), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new AutoCompletionDelegateSource((Func<AutoCompletionContext, CancellationToken, ValueTask<AutoCompletionResult?>>) null!), Throws.TypeOf<ArgumentNullException>());
	}
}
