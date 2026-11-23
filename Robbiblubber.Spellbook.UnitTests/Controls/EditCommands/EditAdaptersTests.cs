namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="EditAdapters"/> registry.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class EditAdaptersTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private classes                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a list box subclass.</summary>
	private class _CustomListBox: ListBox
	{}


	/// <summary>This class implements an adapter that supports every command.</summary>
	private class _CustomAdapter: EditAdapter<ListBox>
	{
		/// <inheritdoc/>
		protected override bool _CanExecute(ListBox control, EditCommand command)
		{
			return true;
		}


		/// <inheritdoc/>
		protected override void _Execute(ListBox control, EditCommand command)
		{}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Restores the default registrations after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		EditAdapters.Reset();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The default adapters are registered.</summary>
	[Test]
	public void DefaultAdaptersAreRegistered()
	{
		Assert.That(EditAdapters.Find(typeof(TextBox)), Is.InstanceOf<TextBoxEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(ComboBox)), Is.InstanceOf<ComboBoxEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(ListBox)), Is.InstanceOf<ListBoxEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(ListView)), Is.InstanceOf<ListViewEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(DataGridView)), Is.InstanceOf<DataGridViewEditAdapter>());
	}


	/// <summary>Derived control types use the adapter of the nearest registered base type.</summary>
	[Test]
	public void FindsBaseTypeAdapter()
	{
		Assert.That(EditAdapters.Find(typeof(RichTextBox)), Is.InstanceOf<TextBoxEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(MaskedTextBox)), Is.InstanceOf<TextBoxEditAdapter>());
		Assert.That(EditAdapters.Find(typeof(_CustomListBox)), Is.InstanceOf<ListBoxEditAdapter>());
	}


	/// <summary>Controls without a registered adapter return NULL.</summary>
	[Test]
	public void UnknownControlHasNoAdapter()
	{
		using Button button = new();

		Assert.That(EditAdapters.Find(button), Is.Null);
		Assert.That(EditAdapters.Find((Control?) null), Is.Null);
	}


	/// <summary>The most derived registration wins.</summary>
	[Test]
	public void MostDerivedRegistrationWins()
	{
		_CustomAdapter adapter = new();
		using _CustomListBox box = new();

		EditAdapters.Register<_CustomListBox>(adapter);

		Assert.That(EditAdapters.Find(box), Is.SameAs(adapter));
		Assert.That(EditAdapters.Find(typeof(ListBox)), Is.InstanceOf<ListBoxEditAdapter>());
	}


	/// <summary>Registrations can be replaced, removed and reset.</summary>
	[Test]
	public void RegisterUnregisterReset()
	{
		_CustomAdapter adapter = new();

		EditAdapters.Register(typeof(ListBox), adapter);
		Assert.That(EditAdapters.Find(typeof(ListBox)), Is.SameAs(adapter));

		Assert.That(EditAdapters.Unregister(typeof(ListBox)), Is.True);
		Assert.That(EditAdapters.Unregister(typeof(ListBox)), Is.False);
		Assert.That(EditAdapters.Find(typeof(ListBox)), Is.Null);

		EditAdapters.Reset();
		Assert.That(EditAdapters.Find(typeof(ListBox)), Is.InstanceOf<ListBoxEditAdapter>());
	}


	/// <summary>Only control types can be registered.</summary>
	[Test]
	public void RegisterValidatesArguments()
	{
		Assert.Throws<ArgumentException>(() => EditAdapters.Register(typeof(string), new _CustomAdapter()));
		Assert.Throws<ArgumentNullException>(() => EditAdapters.Register(typeof(ListBox), null!));
	}
}
