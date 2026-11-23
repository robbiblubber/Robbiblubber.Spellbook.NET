namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the discovery of edit adapters by <see cref="EditAdapters.Discover(IEnumerable{Type})"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")][NonParallelizable]
public class EditAdapterDiscoveryTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private classes                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a control for discovery tests.</summary>
	private class DiscoveryControl: Control
	{}


	/// <summary>This class implements a marked adapter whose control type is taken from its base class.</summary>
	[EditAdapter]
	private class GenericAdapter: EditAdapter<DiscoveryControl>
	{
		/// <inheritdoc/>
		protected override bool _CanExecute(DiscoveryControl control, EditCommand command)
		{
			return true;
		}


		/// <inheritdoc/>
		protected override void _Execute(DiscoveryControl control, EditCommand command)
		{}
	}


	/// <summary>This class implements a marked adapter for two control types.</summary>
	[EditAdapter(typeof(Label))]
	[EditAdapter(typeof(LinkLabel))]
	private class MultiAdapter: IEditAdapter
	{
		/// <inheritdoc/>
		public bool CanExecute(Control control, EditCommand command)
		{
			return false;
		}


		/// <inheritdoc/>
		public void Execute(Control control, EditCommand command)
		{}
	}


	/// <summary>This class implements an adapter without attribute.</summary>
	private class UnmarkedAdapter: EditAdapter<Button>
	{
		/// <inheritdoc/>
		protected override bool _CanExecute(Button control, EditCommand command)
		{
			return false;
		}


		/// <inheritdoc/>
		protected override void _Execute(Button control, EditCommand command)
		{}
	}


	/// <summary>This class implements a marked adapter without a parameterless constructor.</summary>
	[EditAdapter(typeof(CheckBox))]
	private class ParameterAdapter: MultiAdapter
	{
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="value">Value.</param>
		public ParameterAdapter(int value): base()
		{}
	}


	/// <summary>This class implements a marked adapter whose constructor fails.</summary>
	[EditAdapter(typeof(RadioButton))]
	private class FailingAdapter: MultiAdapter
	{
		/// <summary>Creates a new instance of this class.</summary>
		public FailingAdapter(): base()
		{
			throw new InvalidOperationException("Fails.");
		}
	}


	/// <summary>This class implements a marked abstract adapter.</summary>
	[EditAdapter(typeof(GroupBox))]
	private abstract class AbstractAdapter: MultiAdapter
	{}


	/// <summary>This class implements an adapter marked for a type that is not a control.</summary>
	[EditAdapter(typeof(string))]
	private class InvalidTypeAdapter: MultiAdapter
	{}


	/// <summary>This class implements an adapter for text boxes, replacing the default adapter.</summary>
	[EditAdapter(typeof(TextBoxBase))]
	private class TextAdapter: MultiAdapter
	{}



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

	/// <summary>The control type is taken from EditAdapter&lt;T&gt; if the attribute does not specify it.</summary>
	[Test]
	public void DiscoversGenericAdapter()
	{
		Assert.That(EditAdapters.Discover(new Type[] { typeof(GenericAdapter) }), Is.EqualTo(1));
		Assert.That(EditAdapters.Find(typeof(DiscoveryControl)), Is.InstanceOf<GenericAdapter>());
	}


	/// <summary>An adapter with several attributes is registered for several control types, as one instance.</summary>
	[Test]
	public void DiscoversMultipleControlTypes()
	{
		Assert.That(EditAdapters.Discover(new Type[] { typeof(MultiAdapter) }), Is.EqualTo(2));
		Assert.That(EditAdapters.Find(typeof(Label)), Is.InstanceOf<MultiAdapter>());
		Assert.That(EditAdapters.Find(typeof(LinkLabel)), Is.SameAs(EditAdapters.Find(typeof(Label))));
	}


	/// <summary>Unmarked, abstract, failing and invalid adapters as well as other types are skipped.</summary>
	[Test]
	public void SkipsInvalidTypes()
	{
		Type?[] types = new Type?[] { typeof(UnmarkedAdapter), typeof(ParameterAdapter), typeof(FailingAdapter), typeof(AbstractAdapter), typeof(InvalidTypeAdapter), typeof(string), null };

		Assert.That(EditAdapters.Discover(types), Is.EqualTo(0));
		Assert.That(EditAdapters.Find(typeof(Button)), Is.Null);
		Assert.That(EditAdapters.Find(typeof(CheckBox)), Is.Null);
		Assert.That(EditAdapters.Find(typeof(RadioButton)), Is.Null);
		Assert.That(EditAdapters.Find(typeof(GroupBox)), Is.Null);
	}


	/// <summary>Discovered adapters replace the default adapters; Reset restores them.</summary>
	[Test]
	public void ReplacesDefaultAdapters()
	{
		EditAdapters.Discover(new Type[] { typeof(TextAdapter) });
		Assert.That(EditAdapters.Find(typeof(TextBox)), Is.InstanceOf<TextAdapter>());

		EditAdapters.Reset();
		Assert.That(EditAdapters.Find(typeof(TextBox)), Is.InstanceOf<TextBoxEditAdapter>());
	}


	/// <summary>Discovering an assembly registers all marked adapters it contains.</summary>
	[Test]
	public void DiscoversAssembly()
	{
		int count = EditAdapters.Discover(typeof(EditAdapterDiscoveryTests).Assembly);

		Assert.That(count, Is.GreaterThanOrEqualTo(4));
		Assert.That(EditAdapters.Find(typeof(DiscoveryControl)), Is.InstanceOf<GenericAdapter>());
		Assert.That(EditAdapters.Find(typeof(LinkLabel)), Is.Not.Null);
		Assert.That(EditAdapters.Find(typeof(Button)), Is.Null);
	}


	/// <summary>NULL arguments are rejected.</summary>
	[Test]
	public void RejectsNull()
	{
		Assert.Throws<ArgumentNullException>(() => EditAdapters.Discover((IEnumerable<Type?>) null!));
		Assert.Throws<ArgumentNullException>(() => EditAdapters.Discover((System.Reflection.Assembly) null!));
	}
}
