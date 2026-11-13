namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="EditCommandProvider"/> component.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class EditCommandProviderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Provider.</summary>
	private TestEditCommandProvider _Provider = null!;

	/// <summary>Text box with text "Hello world", nothing selected, caret at the start.</summary>
	private TextBox _Text = null!;

	/// <summary>Editable panel containing <see cref="_Inner"/>.</summary>
	private TestEditable _Editable = null!;

	/// <summary>Text box inside the editable panel.</summary>
	private TextBox _Inner = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Provider = new TestEditCommandProvider() { AutoUpdate = false };

		_Text = new TextBox() { Text = "Hello world" };
		_ = _Text.Handle;
		_Text.Select(0, 0);

		_Editable = new TestEditable();
		_Inner = new TextBox() { Text = "Inner" };
		_Editable.Controls.Add(_Inner);
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Provider.Dispose();
		_Text.Dispose();
		_Inner.Dispose();
		_Editable.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new provider has the documented default values.</summary>
	[Test]
	public void ProviderHasDefaults()
	{
		using EditCommandProvider provider = new();

		Assert.That(provider.AutoUpdate, Is.True);
		Assert.That(provider.UpdateInterval, Is.EqualTo(100));
		Assert.That(provider.ContainerControl, Is.Null);
		Assert.That(provider.ActiveControl, Is.Null);
	}


	/// <summary>The constructors set the container and the container control.</summary>
	[Test]
	public void ConstructorsSetContainer()
	{
		using Container container = new();
		using UserControl root = new();
		using EditCommandProvider contained = new(container);
		using EditCommandProvider rooted = new(root);

		Assert.That(container.Components.Cast<IComponent>(), Does.Contain(contained));
		Assert.That(rooted.ContainerControl, Is.SameAs(root));
	}


	/// <summary>A user control as container has no active control while it does not contain the focus.</summary>
	[Test]
	public void UserControlWithoutFocusHasNoActiveControl()
	{
		using UserControl root = new();
		using TextBox text = new();
		using EditCommandProvider provider = new(root);

		root.Controls.Add(text);

		Assert.That(provider.ActiveControl, Is.Null);
		Assert.That(provider.CanExecute(EditCommand.SELECT_ALL), Is.False);
	}


	/// <summary>The update interval must be positive.</summary>
	[Test]
	public void UpdateIntervalMustBePositive()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => _Provider.UpdateInterval = 0);

		_Provider.UpdateInterval = 250;
		Assert.That(_Provider.UpdateInterval, Is.EqualTo(250));
	}


	/// <summary>The provider extends tool strip items and controls, but not forms.</summary>
	[Test]
	public void CanExtendItemsAndControls()
	{
		using ToolStripMenuItem item = new();
		using Form form = new();

		Assert.That(_Provider.CanExtend(item), Is.True);
		Assert.That(_Provider.CanExtend(_Text), Is.True);
		Assert.That(_Provider.CanExtend(form), Is.False);
		Assert.That(_Provider.CanExtend("text"), Is.False);
	}


	/// <summary>The extender properties are declared with their default values.</summary>
	[Test]
	public void ExtenderPropertiesAreDeclared()
	{
		ProvidePropertyAttribute[] provided = typeof(EditCommandProvider).GetCustomAttributes<ProvidePropertyAttribute>().ToArray();

		Assert.That(provided.Select(m => m.PropertyName), Is.EquivalentTo(new string[] { "EditCommand", "EditCommandsEnabled" }));

		Assert.That(typeof(EditCommandProvider).GetMethod(nameof(EditCommandProvider.GetEditCommand))!.GetCustomAttribute<DefaultValueAttribute>()!.Value, Is.Null);
		Assert.That(typeof(EditCommandProvider).GetMethod(nameof(EditCommandProvider.GetEditCommandsEnabled))!.GetCustomAttribute<DefaultValueAttribute>()!.Value, Is.EqualTo(true));
	}


	/// <summary>Without an active control, no command is available.</summary>
	[Test]
	public void NoActiveControlNoCommands()
	{
		foreach(EditCommand i in Enum.GetValues<EditCommand>()) { Assert.That(_Provider.CanExecute(i), Is.False, i.ToString()); }
	}


	/// <summary>For standard controls, the registered adapter is used.</summary>
	[Test]
	public void UsesAdapter()
	{
		_Provider.Active = _Text;

		Assert.That(_Provider.CanExecute(EditCommand.SELECT_ALL), Is.True);
		Assert.That(_Provider.CanExecute(EditCommand.COPY), Is.False);

		_Provider.Execute(EditCommand.SELECT_ALL);

		Assert.That(_Text.SelectionLength, Is.EqualTo(_Text.TextLength));
		Assert.That(_Provider.CanExecute(EditCommand.COPY), Is.True);
	}


	/// <summary>Controls without adapter support no commands.</summary>
	[Test]
	public void UnsupportedControlNoCommands()
	{
		using Button button = new();

		_Provider.Active = button;

		Assert.That(_Provider.CanExecute(EditCommand.COPY), Is.False);
		Assert.DoesNotThrow(() => _Provider.Execute(EditCommand.COPY));
	}


	/// <summary>An editable parent handles the commands of its child controls.</summary>
	[Test]
	public void EditableParentWins()
	{
		_Provider.Active = _Inner;
		_Editable.Allowed.Add(EditCommand.PASTE);

		Assert.That(_Provider.CanExecute(EditCommand.PASTE), Is.True);
		Assert.That(_Provider.CanExecute(EditCommand.SELECT_ALL), Is.False);

		_Provider.Execute(EditCommand.PASTE);
		_Provider.Execute(EditCommand.SELECT_ALL);

		Assert.That(_Editable.Executed, Is.EqualTo(new EditCommand[] { EditCommand.PASTE }));
		Assert.That(_Inner.SelectionLength, Is.EqualTo(0));
	}


	/// <summary>An editable bearing provides its editable object; without one, the adapter is used.</summary>
	[Test]
	public void BearingProvidesEditable()
	{
		using TestEditableBearing bearing = new();
		using TextBox text = new() { Text = "abc" };
		TestEditable editable = new();

		bearing.Controls.Add(text);
		_Provider.Active = text;

		bearing.Editable = editable;
		editable.Allowed.Add(EditCommand.UNDO);
		Assert.That(_Provider.CanExecute(EditCommand.UNDO), Is.True);
		Assert.That(_Provider.CanExecute(EditCommand.SELECT_ALL), Is.False);

		bearing.Editable = null;
		Assert.That(_Provider.CanExecute(EditCommand.UNDO), Is.False);
		Assert.That(_Provider.CanExecute(EditCommand.SELECT_ALL), Is.True);

		editable.Dispose();
	}


	/// <summary>A bearing that returns the provider itself is skipped, so the provider does not call itself.</summary>
	[Test]
	public void SkipsProviderAsEditable()
	{
		using TestEditableBearing bearing = new();
		using TextBox text = new() { Text = "abc" };

		bearing.Controls.Add(text);
		bearing.Editable = _Provider;
		_Provider.Active = text;

		Assert.That(_Provider.CanExecute(EditCommand.SELECT_ALL), Is.True);
		_Provider.Execute(EditCommand.SELECT_ALL);
		Assert.That(text.SelectionLength, Is.EqualTo(3));
	}


	/// <summary>Controls with edit commands disabled, and their child controls, are excluded.</summary>
	[Test]
	public void ExcludedControlsHaveNoCommands()
	{
		_Editable.Allowed.Add(EditCommand.COPY);
		_Provider.Active = _Inner;

		Assert.That(_Provider.GetEditCommandsEnabled(_Editable), Is.True);

		_Provider.SetEditCommandsEnabled(_Editable, false);
		Assert.That(_Provider.GetEditCommandsEnabled(_Editable), Is.False);
		Assert.That(_Provider.GetEditCommandsEnabled(_Inner), Is.True);
		Assert.That(_Provider.CanExecute(EditCommand.COPY), Is.False);

		_Provider.Execute(EditCommand.COPY);
		Assert.That(_Editable.Executed, Is.Empty);

		_Provider.SetEditCommandsEnabled(_Editable, true);
		Assert.That(_Provider.CanExecute(EditCommand.COPY), Is.True);
	}


	/// <summary>Disposed controls are removed from the excluded controls.</summary>
	[Test]
	public void DisposedControlIsNoLongerExcluded()
	{
		TextBox text = new();

		_Provider.SetEditCommandsEnabled(text, false);
		text.Dispose();

		Assert.That(_Provider.GetEditCommandsEnabled(text), Is.True);
	}


	/// <summary>The Querying event can change the availability of a command.</summary>
	[Test]
	public void QueryingChangesAvailability()
	{
		List<EditCommandQueryEventArgs> queries = new();

		_Provider.Active = _Text;
		_Provider.Querying += (sender, e) =>
		{
			queries.Add(e);
			if(e.Command == EditCommand.DELETE) { e.CanExecute = false; }
			if(e.Command == EditCommand.REDO) { e.CanExecute = true; }
		};

		Assert.That(_Provider.CanExecute(EditCommand.DELETE), Is.False);
		Assert.That(_Provider.CanExecute(EditCommand.REDO), Is.True);

		Assert.That(queries[0].Command, Is.EqualTo(EditCommand.DELETE));
		Assert.That(queries[0].Control, Is.SameAs(_Text));
	}


	/// <summary>Executing, the default implementation and Executed run in this order.</summary>
	[Test]
	public void ExecuteRaisesEvents()
	{
		List<string> log = new();

		_Editable.Allowed.Add(EditCommand.CUT);
		_Provider.Active = _Inner;
		_Provider.Executing += (sender, e) => { log.Add($"Executing {e.Command} {e.Control?.Text}"); };
		_Provider.Executed += (sender, e) => { log.Add($"Executed {e.Command} {_Editable.Executed.Count}"); };

		_Provider.Execute(EditCommand.CUT);

		Assert.That(log, Is.EqualTo(new string[] { "Executing CUT Inner", "Executed CUT 1" }));
	}


	/// <summary>Commands handled by the application skip the default implementation.</summary>
	[Test]
	public void HandledSkipsDefault()
	{
		bool executed = false;

		_Editable.Allowed.Add(EditCommand.DELETE);
		_Provider.Active = _Inner;
		_Provider.Executing += (sender, e) => { e.Handled = true; };
		_Provider.Executed += (sender, e) => { executed = true; };

		_Provider.Execute(EditCommand.DELETE);

		Assert.That(_Editable.Executed, Is.Empty);
		Assert.That(executed, Is.True);
	}


	/// <summary>Commands that cannot be executed raise no events.</summary>
	[Test]
	public void UnavailableCommandRaisesNoEvents()
	{
		int count = 0;

		_Provider.Active = _Inner;
		_Provider.Executing += (sender, e) => { count++; };
		_Provider.Executed += (sender, e) => { count++; };

		_Provider.Execute(EditCommand.PASTE);

		Assert.That(count, Is.EqualTo(0));
	}


	/// <summary>Commands enabled through Querying can be implemented in Executing.</summary>
	[Test]
	public void ApplicationImplementsCommand()
	{
		using ListBox list = new();

		list.Items.AddRange(new object[] { "a", "b" });
		_ = list.Handle;
		list.SelectedIndex = 0;

		_Provider.Active = list;
		_Provider.Querying += (sender, e) => { if((e.Command == EditCommand.DELETE) && (e.Control is ListBox l)) { e.CanExecute = (l.SelectedIndex >= 0); } };
		_Provider.Executing += (sender, e) =>
		{
			if((e.Command == EditCommand.DELETE) && (e.Control is ListBox l))
			{
				l.Items.RemoveAt(l.SelectedIndex);
				e.Handled = true;
			}
		};

		Assert.That(_Provider.CanDelete(), Is.True);
		_Provider.Delete();

		Assert.That(list.Items.Cast<string>(), Is.EqualTo(new string[] { "b" }));
	}


	/// <summary>Bound items get and lose their command.</summary>
	[Test]
	public void BindsItems()
	{
		using ToolStripMenuItem item = new();

		Assert.That(_Provider.GetEditCommand(item), Is.Null);

		_Provider.SetEditCommand(item, EditCommand.COPY);
		Assert.That(_Provider.GetEditCommand(item), Is.EqualTo(EditCommand.COPY));

		_Provider.SetEditCommand(item, null);
		Assert.That(_Provider.GetEditCommand(item), Is.Null);
	}


	/// <summary>Clicking a bound item executes its command; unbound items do nothing.</summary>
	[Test]
	public void ClickExecutesCommand()
	{
		using ToolStripMenuItem item = new();

		_Editable.Allowed.Add(EditCommand.COPY);
		_Provider.Active = _Inner;
		_Provider.SetEditCommand(item, EditCommand.COPY);

		item.PerformClick();
		Assert.That(_Editable.Executed, Is.EqualTo(new EditCommand[] { EditCommand.COPY }));

		_Provider.SetEditCommand(item, null);
		item.PerformClick();
		Assert.That(_Editable.Executed.Count, Is.EqualTo(1));
	}


	/// <summary>Updating enables and disables the bound items.</summary>
	[Test]
	public void UpdateItemsEnablesItems()
	{
		using ToolStripMenuItem copy = new();
		using ToolStripMenuItem copy2 = new();
		using ToolStripButton paste = new();

		_Editable.Allowed.Add(EditCommand.COPY);
		_Provider.Active = _Inner;
		_Provider.SetEditCommand(copy, EditCommand.COPY);
		_Provider.SetEditCommand(copy2, EditCommand.COPY);
		_Provider.SetEditCommand(paste, EditCommand.PASTE);

		_Provider.UpdateItems();
		Assert.That(copy.Enabled, Is.True);
		Assert.That(copy2.Enabled, Is.True);
		Assert.That(paste.Enabled, Is.False);

		_Editable.Allowed.Clear();
		_Editable.Allowed.Add(EditCommand.PASTE);
		_Provider.UpdateItems();
		Assert.That(copy.Enabled, Is.False);
		Assert.That(paste.Enabled, Is.True);
	}


	/// <summary>EditStateChanged is raised when the availability of a command changes.</summary>
	[Test]
	public void EditStateChangedOnChange()
	{
		int count = 0;

		_Provider.Active = _Inner;
		_Provider.EditStateChanged += (sender, e) => { count++; };

		_Provider.UpdateItems();
		Assert.That(count, Is.EqualTo(1));

		_Provider.UpdateItems();
		Assert.That(count, Is.EqualTo(1));

		_Editable.Allowed.Add(EditCommand.COPY);
		_Provider.UpdateItems();
		Assert.That(count, Is.EqualTo(2));
	}


	/// <summary>The provider observes the EditStateChanged event of the current editable object.</summary>
	[Test]
	public void ObservesEditable()
	{
		using ToolStripMenuItem item = new();

		_Provider.Active = _Inner;
		_Provider.AutoUpdate = true;
		_Provider.SetEditCommand(item, EditCommand.COPY);
		_Provider.UpdateItems();

		Assert.That(item.Enabled, Is.False);
		Assert.That(_Editable.HandlerCount, Is.EqualTo(1));

		_Editable.Allowed.Add(EditCommand.COPY);
		_Editable.RaiseEditStateChanged();
		Assert.That(item.Enabled, Is.True);

		_Provider.Active = _Text;
		_Provider.UpdateItems();
		Assert.That(_Editable.HandlerCount, Is.EqualTo(0));
	}


	/// <summary>Disposing the provider releases the items and the observed editable object.</summary>
	[Test]
	public void DisposeReleasesItems()
	{
		using ToolStripMenuItem item = new();

		_Editable.Allowed.Add(EditCommand.COPY);
		_Provider.Active = _Inner;
		_Provider.SetEditCommand(item, EditCommand.COPY);
		_Provider.UpdateItems();

		_Provider.Dispose();

		item.PerformClick();
		Assert.That(_Editable.Executed, Is.Empty);
		Assert.That(_Editable.HandlerCount, Is.EqualTo(0));
	}


	/// <summary>Disposed items are released.</summary>
	[Test]
	public void DisposedItemIsReleased()
	{
		ToolStripMenuItem item = new();

		_Provider.SetEditCommand(item, EditCommand.COPY);
		item.Dispose();

		Assert.That(_Provider.GetEditCommand(item), Is.Null);
	}
}
