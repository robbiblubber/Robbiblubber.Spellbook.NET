namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the properties and the extender provider implementation of <see cref="SignatureHelpProvider"/>.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class SignatureHelpProviderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Provider.</summary>
	private SignatureHelpProvider _Provider = null!;

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Provider = new SignatureHelpProvider();
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
		Assert.That(_Provider.TriggerCharacters, Is.EqualTo("(,"));
		Assert.That(_Provider.ShowDescriptions, Is.True);
		Assert.That(_Provider.MaximumWidth, Is.EqualTo(600));
		Assert.That(_Provider.HighlightStyle, Is.EqualTo(ListHighlightStyle.BOLD));
		Assert.That(_Provider.WindowBackColor, Is.EqualTo(Color.Empty));
		Assert.That(_Provider.Parser, Is.InstanceOf<SignatureCallParser>());
		Assert.That(_Provider.Source, Is.Null);
		Assert.That(_Provider.AutoCompletion, Is.Null);
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

		Assert.That(checkedProperties, Is.GreaterThanOrEqualTo(10));
	}


	/// <summary>Every provided property has matching Get and Set methods with a default value that matches an unconfigured control.</summary>
	[Test]
	public void ProvidedPropertiesHaveAccessors()
	{
		Type type = typeof(SignatureHelpProvider);
		int count = 0;

		foreach(ProvidePropertyAttribute i in type.GetCustomAttributes<ProvidePropertyAttribute>())
		{
			MethodInfo? getter = type.GetMethod("Get" + i.PropertyName, new[] { typeof(Control) });
			DefaultValueAttribute? defaultValue = getter?.GetCustomAttribute<DefaultValueAttribute>();

			Assert.That(getter, Is.Not.Null, $"Get{i.PropertyName} is missing.");
			Assert.That(type.GetMethod("Set" + i.PropertyName, new[] { typeof(Control), getter!.ReturnType }), Is.Not.Null, $"Set{i.PropertyName} is missing.");
			Assert.That(defaultValue, Is.Not.Null, $"Get{i.PropertyName} has no default value.");
			Assert.That(getter.Invoke(_Provider, new object[] { _Text }), Is.EqualTo(defaultValue!.Value), $"Default value of {i.PropertyName} does not match.");
			count++;
		}

		Assert.That(count, Is.EqualTo(4));
	}


	/// <summary>Extender values are stored per control.</summary>
	[Test]
	public void ExtenderValuesAreStoredPerControl()
	{
		SignatureHelpItemSource source = new();
		SignatureCallParser parser = new();

		using(TextBox other = new())
		{
			_Provider.SetSignatureHelpEnabled(_Text, true);
			_Provider.SetAutoStart(_Text, false);
			_Provider.SetShortcutKeys(_Text, Keys.F2);
			_Provider.SetTriggerCharacters(_Text, "(");
			_Provider.SetSource(_Text, source);
			_Provider.SetParser(_Text, parser);

			Assert.That(_Provider.GetSignatureHelpEnabled(_Text), Is.True);
			Assert.That(_Provider.GetAutoStart(_Text), Is.False);
			Assert.That(_Provider.GetShortcutKeys(_Text), Is.EqualTo(Keys.F2));
			Assert.That(_Provider.GetTriggerCharacters(_Text), Is.EqualTo("("));
			Assert.That(_Provider.GetSource(_Text), Is.SameAs(source));
			Assert.That(_Provider.GetParser(_Text), Is.SameAs(parser));

			Assert.That(_Provider.GetSignatureHelpEnabled(other), Is.False);
			Assert.That(_Provider.GetSource(other), Is.Null);
			Assert.That(_Provider.GetParser(other), Is.Null);
		}
	}


	/// <summary>Invalid values and controls are rejected.</summary>
	[Test]
	public void InvalidValuesAreRejected()
	{
		using(Button button = new())
		{
			Assert.That(() => { _Provider.MaximumWidth = 99; }, Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => _Provider.GetSignatureHelpEnabled(null!), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => _Provider.SetSignatureHelpEnabled(button, true), Throws.TypeOf<ArgumentException>());
			Assert.That(() => _Provider.ShowSignatureHelp(_Text), Throws.TypeOf<InvalidOperationException>());
			Assert.That(_Provider.CanExtend(_Text), Is.True);
			Assert.That(_Provider.CanExtend(button), Is.False);
		}
	}


	/// <summary>Setting the parser to NULL restores a default parser.</summary>
	[Test]
	public void NullParserRestoresDefault()
	{
		SignatureCallParser parser = new() { LineComment = "--" };

		_Provider.Parser = parser;
		Assert.That(_Provider.Parser, Is.SameAs(parser));

		_Provider.Parser = null!;
		Assert.That(_Provider.Parser, Is.InstanceOf<SignatureCallParser>());
		Assert.That(_Provider.Parser, Is.Not.SameAs(parser));
	}


	/// <summary>The link to an auto completion provider is released when that provider is disposed.</summary>
	[Test]
	public void LinkIsReleasedOnDispose()
	{
		AutoCompletionProvider completion = new();

		_Provider.AutoCompletion = completion;
		Assert.That(_Provider.AutoCompletion, Is.SameAs(completion));

		completion.Dispose();
		Assert.That(_Provider.AutoCompletion, Is.Null);
	}


	/// <summary>A disposed control is removed from the provider.</summary>
	[Test]
	public void DisposedControlIsRemoved()
	{
		TextBox text = new();

		_Provider.SetSignatureHelpEnabled(text, true);
		text.Dispose();

		Assert.That(_Provider.GetSignatureHelpEnabled(text), Is.False);
	}


	/// <summary>The provider works as a component; tracking suspensions can be disposed twice.</summary>
	[Test]
	public void ProviderWorksAsComponent()
	{
		using(Container container = new())
		{
			SignatureHelpProvider provider = new(container);
			IDisposable suspension = provider.SuspendTracking(_Text);

			provider.SetSignatureHelpEnabled(_Text, true);
			suspension.Dispose();

			Assert.That(() => suspension.Dispose(), Throws.Nothing);
			Assert.That(container.Components.Cast<IComponent>(), Does.Contain(provider));
			Assert.That(() => container.Dispose(), Throws.Nothing);
		}
	}
}
