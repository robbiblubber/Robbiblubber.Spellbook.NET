namespace Robbiblubber.Spellbook.UnitTests.Core.Reflection;

using NUnit.Framework;



/// <summary>This class tests the reflection helpers.</summary>
[TestFixture]
public class ClassOpTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test classes                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Base class with private members.</summary>
	public class Base
	{
		/// <summary>Private property of the base class.</summary>
		private string Secret { get; set; } = "base";

		/// <summary>Private field of the base class.</summary>
		private int _Counter = 1;

		/// <summary>Gets the counter.</summary>
		public int Counter => _Counter;

		/// <summary>Gets the secret.</summary>
		public string GetSecret() => Secret;
	}


	/// <summary>Derived class.</summary>
	public class Derived: Base
	{
		/// <summary>Read-only property.</summary>
		public string ReadOnly => "ro";

		/// <summary>Property whose accessors throw.</summary>
		public int Failing
		{
			get { throw new NotSupportedException("get"); }
			set { throw new NotSupportedException("set"); }
		}

		/// <summary>Indexer, which is not a named property.</summary>
		public int this[int i] => i;
	}


	/// <summary>Abstract implementation.</summary>
	public abstract class AbstractEncoder: EncoderBase
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: __Get / __Set                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Private properties and fields of base classes are read and written.</summary>
	[Test]
	public void GetSet_PrivateBaseMembers()
	{
		Derived d = new();

		Assert.That(d.__Get<string>("Secret"), Is.EqualTo("base"));
		d.__Set("Secret", "changed");
		Assert.That(d.GetSecret(), Is.EqualTo("changed"));

		Assert.That(d.__Get<int>("_Counter"), Is.EqualTo(1));
		d.__Set("_Counter", 5);
		Assert.That(d.Counter, Is.EqualTo(5));

		Assert.That(d.__Get<string>("ReadOnly"), Is.EqualTo("ro"));
	}


	/// <summary>Missing and read-only members throw unless switched off.</summary>
	[Test]
	public void GetSet_Missing()
	{
		Derived d = new();

		Assert.Throws<InvalidOperationException>(() => d.__Get<string>("Nope"));
		Assert.That(d.__Get("Nope", false, "default"), Is.EqualTo("default"));
		Assert.Throws<InvalidOperationException>(() => d.__Get<int>("Item"));

		Assert.Throws<InvalidOperationException>(() => d.__Set("Nope", 1));
		Assert.DoesNotThrow(() => d.__Set("Nope", 1, false));
		Assert.Throws<InvalidOperationException>(() => d.__Set("ReadOnly", "x"));
		Assert.DoesNotThrow(() => d.__Set("ReadOnly", "x", false));
		Assert.Throws<ArgumentNullException>(() => ((object) null!).__Get<int>("x"));
	}


	/// <summary>Accessor exceptions and type mismatches are reported.</summary>
	[Test]
	public void GetSet_Errors()
	{
		Derived d = new();

		InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => d.__Get<int>("Failing"))!;
		Assert.That(ex.InnerException, Is.InstanceOf<NotSupportedException>());
		Assert.That(d.__Get("Failing", false, -1), Is.EqualTo(-1));
		Assert.Throws<InvalidOperationException>(() => d.__Get<int>("Secret"));

		Assert.Throws<NotSupportedException>(() => d.__Set("Failing", 1));
		Assert.Throws<ArgumentException>(() => d.__Set("Secret", 42));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: implementations                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Only public, concrete classes are returned, not the type itself or abstract classes.</summary>
	[Test]
	public void GetImplementations()
	{
		Type[] types = ClassOp.GetImplementations<IEncoder>(typeof(IEncoder).Assembly, typeof(ClassOpTests).Assembly);

		Assert.That(types, Does.Contain(typeof(Base64)).And.Contain(typeof(Hex)));
		Assert.That(types, Does.Not.Contain(typeof(IEncoder)).And.Not.Contain(typeof(EncoderBase)).And.Not.Contain(typeof(AbstractEncoder)));
		Assert.That(types.Where(m => m.Assembly == typeof(IEncoder).Assembly).All(m => m.IsPublic), Is.True);

		Assert.That(ClassOp.GetImplementations(typeof(Base)), Does.Contain(typeof(Derived)).And.Contain(typeof(Base)));
	}
}
