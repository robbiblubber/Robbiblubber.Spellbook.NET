namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the properties and the extender provider implementation of <see cref="AutoCompletionProvider"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class AutoCompletionProviderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Provider.</summary>
	private AutoCompletionProvider _Provider = null!;

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Provider = new AutoCompletionProvider();
		_Text = new TextBox();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Provider.Dispose();
		_Text.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A new provider has the documented default values.</summary>
	[Test]
	public void ProviderHasDefaults()
	{
		Assert.That(_Provider.MatchMode, Is.EqualTo(ListMatchMode.PREFIX));
		Assert.That(_Provider.FilterMode, Is.EqualTo(ListFilterMode.FILTER));
		Assert.That(_Provider.CaseSensitive, Is.False);
		Assert.That(_Provider.WordCharacters, Is.EqualTo("_"));
		Assert.That(_Provider.TriggerCharacters, Is.EqualTo("."));
		Assert.That(_Provider.CommitCharacters, Is.Empty);
		Assert.That(_Provider.AcceptOnSpace, Is.False);
		Assert.That(_Provider.AcceptOnTab, Is.True);
		Assert.That(_Provider.AcceptOnEnter, Is.True);
		Assert.That(_Provider.Delay, Is.EqualTo(50));
		Assert.That(_Provider.MinimumWordLength, Is.EqualTo(1));
		Assert.That(_Provider.BoxAutoSize, Is.True);
		Assert.That(_Provider.BoxResizable, Is.True);
		Assert.That(_Provider.ShowDescriptions, Is.True);
		Assert.That(_Provider.AcceptSingleMatch, Is.True);
		Assert.That(_Provider.UsageRanking, Is.True);
		Assert.That(_Provider.UsageHistory, Is.Not.Null);
		Assert.That(_Provider.HighlightStyle, Is.EqualTo(ListHighlightStyle.BOLD));
		Assert.That(_Provider.Source, Is.Null);
		Assert.That(_Provider.ImageList, Is.Null);
		Assert.That(_Provider.ActiveControl, Is.Null);
	}


	/// <summary>The default values declared for the designer match the actual default values, so defaults are not serialized.</summary>
	[Test]
	public void DefaultValueAttributesMatchDefaults()
	{
		int checkedProperties = 0;

		foreach(PropertyDescriptor i in TypeDescriptor.GetProperties(_Provider))
		{
			if(i.Attributes[typeof(DefaultValueAttribute)] is null) { continue; }

			Assert.That(i.ShouldSerializeValue(_Provider), Is.False, $"Property {i.Name} would be serialized with its default value.");
			checkedProperties++;
		}

		Assert.That(checkedProperties, Is.GreaterThan(20));
	}


	/// <summary>Every provided property has matching Get and Set methods, as required by the designer.</summary>
	[Test]
	public void ProvidedPropertiesHaveAccessors()
	{
		Type type = typeof(AutoCompletionProvider);

		foreach(ProvidePropertyAttribute i in type.GetCustomAttributes<ProvidePropertyAttribute>())
		{
			MethodInfo? getter = type.GetMethod("Get" + i.PropertyName, new[] { typeof(Control) });

			Assert.That(getter, Is.Not.Null, $"Get{i.PropertyName} is missing.");
			Assert.That(type.GetMethod("Set" + i.PropertyName, new[] { typeof(Control), getter!.ReturnType }), Is.Not.Null, $"Set{i.PropertyName} is missing.");
		}
	}


	/// <summary>The default values declared for the designer on the extender getters match the values of a control without settings.</summary>
	[Test]
	public void ExtenderDefaultValueAttributesMatchDefaults()
	{
		Type type = typeof(AutoCompletionProvider);

		foreach(ProvidePropertyAttribute i in type.GetCustomAttributes<ProvidePropertyAttribute>())
		{
			MethodInfo getter = type.GetMethod("Get" + i.PropertyName, new[] { typeof(Control) })!;
			DefaultValueAttribute? defaultValue = getter.GetCustomAttribute<DefaultValueAttribute>();

			Assert.That(defaultValue, Is.Not.Null, $"Get{i.PropertyName} has no default value.");
			Assert.That(getter.Invoke(_Provider, new object[] { _Text }), Is.EqualTo(defaultValue!.Value), $"Default value of {i.PropertyName} does not match.");
		}
	}


	/// <summary>Invalid property values are rejected.</summary>
	[Test]
	public void InvalidValuesAreRejected()
	{
		Assert.That(() => { _Provider.Delay = -1; }, Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => { _Provider.MinimumWordLength = 0; }, Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => { _Provider.UsageHistory = null!; }, Throws.TypeOf<ArgumentNullException>());

		_Provider.Delay = 0;
		Assert.That(_Provider.Delay, Is.EqualTo(0));
	}


	/// <summary>The usage history can be replaced, for example to share it between providers.</summary>
	[Test]
	public void UsageHistoryCanBeShared()
	{
		AutoCompletionUsageHistory history = new();

		_Provider.UsageHistory = history;
		Assert.That(_Provider.UsageHistory, Is.SameAs(history));
	}


	/// <summary>The provider extends text boxes and rich text boxes only.</summary>
	[Test]
	public void ProviderExtendsTextBoxes()
	{
		using(RichTextBox rich = new())
		using(Button button = new())
		{
			Assert.That(_Provider.CanExtend(_Text), Is.True);
			Assert.That(_Provider.CanExtend(rich), Is.True);
			Assert.That(_Provider.CanExtend(button), Is.False);
			Assert.That(_Provider.CanExtend("text"), Is.False);
		}
	}


	/// <summary>A control without settings returns the default extender values.</summary>
	[Test]
	public void UnboundControlHasDefaults()
	{
		Assert.That(_Provider.GetAutoCompletionEnabled(_Text), Is.False);
		Assert.That(_Provider.GetAutoStart(_Text), Is.True);
		Assert.That(_Provider.GetShortcutKeys(_Text), Is.EqualTo(Keys.Control | Keys.Space));
		Assert.That(_Provider.GetMatchMode(_Text), Is.Null);
		Assert.That(_Provider.GetFilterMode(_Text), Is.Null);
		Assert.That(_Provider.GetCaseSensitive(_Text), Is.Null);
		Assert.That(_Provider.GetWordCharacters(_Text), Is.Null);
		Assert.That(_Provider.GetTriggerCharacters(_Text), Is.Null);
		Assert.That(_Provider.GetCommitCharacters(_Text), Is.Null);
		Assert.That(_Provider.GetBoxSize(_Text), Is.EqualTo(Size.Empty));
		Assert.That(_Provider.GetBoxAutoSize(_Text), Is.Null);
		Assert.That(_Provider.GetBoxResizable(_Text), Is.Null);
		Assert.That(_Provider.GetUsageRanking(_Text), Is.Null);
		Assert.That(_Provider.GetSource(_Text), Is.Null);
	}


	/// <summary>Extender values are stored per control.</summary>
	[Test]
	public void ExtenderValuesAreStoredPerControl()
	{
		AutoCompletionItemSource source = new();

		using(TextBox other = new())
		{
			_Provider.SetAutoCompletionEnabled(_Text, true);
			_Provider.SetAutoStart(_Text, false);
			_Provider.SetShortcutKeys(_Text, Keys.Control | Keys.J);
			_Provider.SetMatchMode(_Text, ListMatchMode.CONTAINS);
			_Provider.SetFilterMode(_Text, ListFilterMode.SELECT);
			_Provider.SetCaseSensitive(_Text, true);
			_Provider.SetWordCharacters(_Text, "_#");
			_Provider.SetTriggerCharacters(_Text, ".[");
			_Provider.SetCommitCharacters(_Text, "(");
			_Provider.SetBoxSize(_Text, new Size(400, 300));
			_Provider.SetBoxAutoSize(_Text, false);
			_Provider.SetBoxResizable(_Text, false);
			_Provider.SetUsageRanking(_Text, false);
			_Provider.SetSource(_Text, source);

			Assert.That(_Provider.GetAutoCompletionEnabled(_Text), Is.True);
			Assert.That(_Provider.GetAutoStart(_Text), Is.False);
			Assert.That(_Provider.GetShortcutKeys(_Text), Is.EqualTo(Keys.Control | Keys.J));
			Assert.That(_Provider.GetMatchMode(_Text), Is.EqualTo(ListMatchMode.CONTAINS));
			Assert.That(_Provider.GetFilterMode(_Text), Is.EqualTo(ListFilterMode.SELECT));
			Assert.That(_Provider.GetCaseSensitive(_Text), Is.True);
			Assert.That(_Provider.GetWordCharacters(_Text), Is.EqualTo("_#"));
			Assert.That(_Provider.GetTriggerCharacters(_Text), Is.EqualTo(".["));
			Assert.That(_Provider.GetCommitCharacters(_Text), Is.EqualTo("("));
			Assert.That(_Provider.GetBoxSize(_Text), Is.EqualTo(new Size(400, 300)));
			Assert.That(_Provider.GetBoxAutoSize(_Text), Is.False);
			Assert.That(_Provider.GetBoxResizable(_Text), Is.False);
			Assert.That(_Provider.GetUsageRanking(_Text), Is.False);
			Assert.That(_Provider.GetSource(_Text), Is.SameAs(source));

			Assert.That(_Provider.GetAutoCompletionEnabled(other), Is.False);
			Assert.That(_Provider.GetMatchMode(other), Is.Null);

			_Provider.SetMatchMode(_Text, null);
			Assert.That(_Provider.GetMatchMode(_Text), Is.Null);
		}
	}


	/// <summary>Extender methods reject NULL and controls that are not text boxes.</summary>
	[Test]
	public void ExtenderMethodsValidateControl()
	{
		using(Button button = new())
		{
			Assert.That(() => _Provider.GetAutoCompletionEnabled(null!), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => _Provider.SetAutoCompletionEnabled(null!, true), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => _Provider.GetMatchMode(button), Throws.TypeOf<ArgumentException>());
			Assert.That(() => _Provider.SetMatchMode(button, ListMatchMode.FUZZY), Throws.TypeOf<ArgumentException>());
		}
	}


	/// <summary>A completion cannot be shown for a control without auto completion.</summary>
	[Test]
	public void ShowCompletionRequiresEnabledControl()
	{
		Assert.That(() => _Provider.ShowCompletion(_Text), Throws.TypeOf<InvalidOperationException>());
	}


	/// <summary>Closing without a session has no effect.</summary>
	[Test]
	public void CloseCompletionWithoutSessionDoesNothing()
	{
		int closed = 0;

		_Provider.Closed += (_, __) => { closed++; };
		_Provider.SetAutoCompletionEnabled(_Text, true);
		_Provider.CloseCompletion();

		Assert.That(closed, Is.EqualTo(0));
	}


	/// <summary>A disposed control is removed from the provider.</summary>
	[Test]
	public void DisposedControlIsRemoved()
	{
		TextBox text = new();

		_Provider.SetAutoCompletionEnabled(text, true);
		_Provider.SetMatchMode(text, ListMatchMode.FUZZY);
		text.Dispose();

		Assert.That(_Provider.GetAutoCompletionEnabled(text), Is.False);
		Assert.That(_Provider.GetMatchMode(text), Is.Null);
	}


	/// <summary>Tracking suspensions can be disposed more than once.</summary>
	[Test]
	public void TrackingSuspensionCanBeDisposedTwice()
	{
		IDisposable suspension = _Provider.SuspendTracking(_Text);

		suspension.Dispose();
		Assert.That(() => suspension.Dispose(), Throws.Nothing);
	}


	/// <summary>The container constructor adds the provider to the container; disposing a provider with bound controls succeeds.</summary>
	[Test]
	public void ProviderWorksAsComponent()
	{
		using(Container container = new())
		{
			AutoCompletionProvider provider = new(container);

			provider.SetAutoCompletionEnabled(_Text, true);

			Assert.That(container.Components.Cast<IComponent>(), Does.Contain(provider));
			Assert.That(() => container.Dispose(), Throws.Nothing);
		}
	}
}
