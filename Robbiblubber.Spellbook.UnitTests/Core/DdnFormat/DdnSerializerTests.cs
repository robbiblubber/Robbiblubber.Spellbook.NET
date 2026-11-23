namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests ddn serialization.</summary>
[TestFixture]
[NonParallelizable]
public class DdnSerializerTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Address                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class.</summary>
	public class Address
	{
		/// <summary>Gets or sets the street.</summary>
		public string Street
		{
			get; set;
		} = string.Empty;


		/// <summary>Gets or sets the city.</summary>
		public string? City
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Person                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class.</summary>
	public class Person
	{
		/// <summary>Gets or sets the name.</summary>
		public string Name
		{
			get; set;
		} = string.Empty;


		/// <summary>Gets or sets the age.</summary>
		public int Age
		{
			get; set;
		}


		/// <summary>Gets or sets the salary.</summary>
		public decimal Salary
		{
			get; set;
		}


		/// <summary>Gets or sets the birthday.</summary>
		public DateTime? Birthday
		{
			get; set;
		}


		/// <summary>Gets or sets the day off.</summary>
		public DayOfWeek DayOff
		{
			get; set;
		}


		/// <summary>Gets or sets the tags.</summary>
		public List<string> Tags
		{
			get; set;
		} = new();


		/// <summary>Gets or sets the address.</summary>
		public Address? Home
		{
			get; set;
		}


		/// <summary>Gets or sets other addresses.</summary>
		public List<Address> Addresses
		{
			get; set;
		} = new();


		/// <summary>Gets or sets scores.</summary>
		public Dictionary<string, int> Scores
		{
			get; set;
		} = new();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Defaults                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class.</summary>
	public class Defaults
	{
		/// <summary>Gets or sets a value.</summary>
		public int A
		{
			get; set;
		} = 17;


		/// <summary>Gets or sets a value.</summary>
		public string B
		{
			get; set;
		} = "default";
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [record] Point                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test record.</summary>
	/// <param name="X">X.</param>
	/// <param name="Y">Y.</param>
	public record Point(int X, int Y);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Immutable                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with get-only properties and a constructor.</summary>
	public class Immutable
	{
		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Name.</param>
		/// <param name="count">Count.</param>
		public Immutable(string name, int count = 5)
		{
			Name = name;
			Count = count;
		}


		/// <summary>Gets the name.</summary>
		public string Name
		{
			get;
		}


		/// <summary>Gets the count.</summary>
		public int Count
		{
			get;
		}


		/// <summary>Gets a computed value that is not serialized.</summary>
		public string Computed
		{
			get { return Name + Count; }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Accessors                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class.</summary>
	public class Accessors
	{
		/// <summary>Gets or privately sets a value.</summary>
		public int PrivateSet
		{
			get; private set;
		}


		/// <summary>Gets a get-only auto-property.</summary>
		public int GetOnly
		{
			get;
		}


		/// <summary>Gets a list without setter that is filled on deserialization.</summary>
		public List<int> Filled
		{
			get;
		} = new();


		/// <summary>Gets an object without setter that is filled on deserialization.</summary>
		public Address Inner
		{
			get;
		} = new();


		/// <summary>Gets or sets an ignored value.</summary>
		[Ddn.Ignore]
		public int Ignored
		{
			get; set;
		} = 1;


		/// <summary>Gets or sets a renamed value.</summary>
		[Ddn.Serialize("renamed")]
		public int Original
		{
			get; set;
		}


		/// <summary>Gets or sets a value that is only written.</summary>
		[Ddn.Serialize(Ddn.SerializationDirection.OUTBOUND)]
		public int WriteOnly
		{
			get; set;
		}


		/// <summary>Gets or sets an internal value.</summary>
		internal int Internal
		{
			get; set;
		}


		/// <summary>Gets an indexed value.</summary>
		/// <param name="i">Index.</param>
		/// <returns>Returns the value.</returns>
		public int this[int i]
		{
			get { return i; }
			set {}
		}


		/// <summary>Gets or sets a delegate.</summary>
		public Action? Callback
		{
			get; set;
		}


		/// <summary>Public field.</summary>
		public int Field = 3;


		/// <summary>Marked private field.</summary>
		[Ddn.Serialize("secret")]
		private int _Secret = 0;


		/// <summary>Value set by method.</summary>
		internal string _Stored = string.Empty;


		/// <summary>Sets the private value.</summary>
		/// <param name="value">Value.</param>
		public void SetPrivate(int value)
		{
			PrivateSet = value;
		}


		/// <summary>Gets the secret.</summary>
		/// <returns>Returns the secret.</returns>
		public int GetSecret()
		{
			return _Secret;
		}


		/// <summary>Sets the secret.</summary>
		/// <param name="value">Value.</param>
		public void SetSecret(int value)
		{
			_Secret = value;
		}


		/// <summary>Provides a value for serialization.</summary>
		/// <returns>Returns the value.</returns>
		[Ddn.Serialize("stored")]
		public string ProvideStored()
		{
			return _Stored.ToUpperInvariant();
		}


		/// <summary>Receives a value on deserialization.</summary>
		/// <param name="value">Value.</param>
		[Ddn.Serialize("stored")]
		public void ReceiveStored(string value)
		{
			_Stored = value.ToLowerInvariant();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] WithFields                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class that serializes fields.</summary>
	[Ddn.Serializable(IncludeFields = true, IncludeNonPublic = true, IncludeProperties = false)]
	public class WithFields
	{
		/// <summary>Public field.</summary>
		public int A;


		/// <summary>Private field.</summary>
		private string _B = string.Empty;


		/// <summary>Gets or sets a property that is not serialized.</summary>
		public int P
		{
			get; set;
		}


		/// <summary>Gets or sets the private field.</summary>
		public string B
		{
			get { return _B; }
			set { _B = value; }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Animal                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test base class.</summary>
	public abstract class Animal
	{
		/// <summary>Gets or sets the name.</summary>
		public string Name
		{
			get; set;
		} = string.Empty;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Dog                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test derived class.</summary>
	public class Dog: Animal
	{
		/// <summary>Gets or sets if the dog barks.</summary>
		public bool Barks
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Cat                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test derived class.</summary>
	public class Cat: Animal
	{
		/// <summary>Gets or sets the lives.</summary>
		public int Lives
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Zoo                                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with polymorphic members.</summary>
	public class Zoo
	{
		/// <summary>Gets or sets the animals.</summary>
		public List<Animal> Animals
		{
			get; set;
		} = new();


		/// <summary>Gets or sets the star.</summary>
		public Animal? Star
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Node                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class for cycles.</summary>
	public class Node
	{
		/// <summary>Gets or sets the next node.</summary>
		public Node? Next
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [struct] Size                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test struct.</summary>
	public struct Size
	{
		/// <summary>Gets or sets the width.</summary>
		public int Width
		{
			get; set;
		}


		/// <summary>Gets or sets the height.</summary>
		public int Height
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Shape                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with a struct member and a document member.</summary>
	public class Shape
	{
		/// <summary>Gets or sets the size.</summary>
		public Size Size
		{
			get; set;
		}


		/// <summary>Gets or sets the optional size.</summary>
		public Size? MaxSize
		{
			get; set;
		}


		/// <summary>Gets or sets free data.</summary>
		public DdnElement? Data
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Collections                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with collections.</summary>
	public class Collections
	{
		/// <summary>Gets or sets an array.</summary>
		public int[] Array
		{
			get; set;
		} = [];


		/// <summary>Gets or sets nullable items.</summary>
		public List<int?> Nullables
		{
			get; set;
		} = new();


		/// <summary>Gets or sets nested lists.</summary>
		public List<List<int>> Nested
		{
			get; set;
		} = new();


		/// <summary>Gets or sets objects with NULL items.</summary>
		public List<Address?> Objects
		{
			get; set;
		} = new();


		/// <summary>Gets or sets a set.</summary>
		public HashSet<string> Set
		{
			get; set;
		} = new();


		/// <summary>Gets or sets a read-only list.</summary>
		public IReadOnlyList<string> ReadOnly
		{
			get; set;
		} = [];


		/// <summary>Gets or sets a dictionary with integer keys.</summary>
		public Dictionary<int, Address> ByNumber
		{
			get; set;
		} = new();


		/// <summary>Gets or sets a dictionary interface.</summary>
		public IDictionary<string, string> Map
		{
			get; set;
		} = new Dictionary<string, string>();


		/// <summary>Gets or sets a dictionary of lists.</summary>
		public Dictionary<string, List<int>> Lists
		{
			get; set;
		} = new();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Color                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with a custom serializer.</summary>
	public class Color
	{
		/// <summary>Gets or sets the red part.</summary>
		public byte R
		{
			get; set;
		}


		/// <summary>Gets or sets the green part.</summary>
		public byte G
		{
			get; set;
		}


		/// <summary>Gets or sets the blue part.</summary>
		public byte B
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] ColorSerializer                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom serializer that writes colors as hex values.</summary>
	[Ddn.CustomSerializer]
	public class ColorSerializer: Ddn.Serializer<Color>
	{
		/// <inheritdoc/>
		public override void Serialize(Color value, DdnElement elm, Ddn.SerializationContext context)
		{
			elm.Set("#" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2"));
		}


		/// <inheritdoc/>
		public override Color? Deserialize(DdnElement elm, Color? existing, Ddn.SerializationContext context)
		{
			string s = elm.Get<string>().TrimStart('#');
			return new Color() { R = Convert.ToByte(s[0..2], 16), G = Convert.ToByte(s[2..4], 16), B = Convert.ToByte(s[4..6], 16) };
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Theme                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test class with custom serialized members.</summary>
	public class Theme
	{
		/// <summary>Gets or sets the foreground.</summary>
		public Color Fore
		{
			get; set;
		} = new();


		/// <summary>Gets or sets the palette.</summary>
		public List<Color> Palette
		{
			get; set;
		} = new();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Removes custom serializers.</summary>
	[TearDown]
	public void TearDown()
	{
		Ddn.UnregisterSerializer(typeof(Color));
		Ddn.UnregisterSerializer(typeof(Size));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Serializes an object to text and back.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="obj">Object.</param>
	/// <param name="options">Options.</param>
	/// <returns>Returns the deserialized object.</returns>
	private static T? _RoundTrip<T>(T obj, Ddn.SerializationOptions? options = null)
	{
		string text = Ddn.Serialize(obj, options).ToString();
		return Ddn.Deserialize<T>(Ddn.Create(text), options);
	}


	/// <summary>Creates a test person.</summary>
	/// <returns>Returns the person.</returns>
	private static Person _CreatePerson()
	{
		return new Person()
		{
			Name = "Debbie / \"Deb\"; {x} = y",
			Age = 37,
			Salary = 1234.56m,
			Birthday = new DateTime(1988, 4, 3, 0, 0, 0, DateTimeKind.Local),
			DayOff = DayOfWeek.Friday,
			Tags = ["a", "b, c", ""],
			Home = new Address() { Street = "Main Street 1", City = "Vienna" },
			Addresses = [new Address() { Street = "One" }, new Address() { Street = "Two", City = "Graz" }],
			Scores = new() { ["math"] = 1, ["a/b"] = 2, ["x y"] = 3 }
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: basics                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>A class with values, collections, nested objects and dictionaries survives a text round trip.</summary>
	[Test]
	public void Poco_RoundTrip()
	{
		Person p = _CreatePerson();
		Person r = _RoundTrip(p)!;

		Assert.That(r.Name, Is.EqualTo(p.Name));
		Assert.That(r.Age, Is.EqualTo(37));
		Assert.That(r.Salary, Is.EqualTo(1234.56m));
		Assert.That(r.Birthday, Is.EqualTo(p.Birthday));
		Assert.That(r.DayOff, Is.EqualTo(DayOfWeek.Friday));
		Assert.That(r.Tags, Is.EqualTo(p.Tags));
		Assert.That(r.Home!.Street, Is.EqualTo("Main Street 1"));
		Assert.That(r.Home.City, Is.EqualTo("Vienna"));
		Assert.That(r.Addresses.Select(m => m.Street), Is.EqualTo(new[] { "One", "Two" }));
		Assert.That(r.Addresses[0].City, Is.Null);
		Assert.That(r.Scores, Is.EquivalentTo(p.Scores));
	}


	/// <summary>The written text uses sections for objects, index names for lists and keys for dictionaries.</summary>
	[Test]
	public void Poco_Structure()
	{
		DdnElement ddn = Ddn.Serialize(_CreatePerson());

		Assert.That(ddn["Age"].Value, Is.EqualTo("37"));
		Assert.That(ddn["DayOff"].Value, Is.EqualTo("Friday"));
		Assert.That(ddn["Home"].IsSection, Is.True);
		Assert.That(ddn["Home/City"].Value, Is.EqualTo("Vienna"));
		Assert.That(ddn["Addresses"].Element("1").Get<string>("Street"), Is.EqualTo("Two"));
		Assert.That(ddn["Scores"].Element("a/b").Get<int>(), Is.EqualTo(2));
		Assert.That(ddn["Tags"].IsSection, Is.False);
		Assert.That(ddn.Contains("$type"), Is.False);
	}


	/// <summary>Elements that are missing keep the default values of the object.</summary>
	[Test]
	public void MissingElements_KeepDefaults()
	{
		Defaults d = Ddn.Deserialize<Defaults>(Ddn.Create("B = changed;"))!;

		Assert.That(d.A, Is.EqualTo(17));
		Assert.That(d.B, Is.EqualTo("changed"));
	}


	/// <summary>Deserializing into an existing object only changes members that are present.</summary>
	[Test]
	public void ExistingObject_IsFilled()
	{
		Person? p = new() { Name = "Old", Age = 5, Tags = ["x"] };
		Person? orig = p;

		Ddn.Deserialize(ref p, Ddn.Create("Age = 6;"));

		Assert.That(p, Is.SameAs(orig));
		Assert.That(p!.Name, Is.EqualTo("Old"));
		Assert.That(p.Age, Is.EqualTo(6));
		Assert.That(p.Tags, Is.EqualTo(new[] { "x" }));
	}


	/// <summary>Unknown elements are ignored.</summary>
	[Test]
	public void UnknownElements_Ignored()
	{
		Defaults d = Ddn.Deserialize<Defaults>(Ddn.Create("X = 1; Y { Z = 2; } A = 4;"))!;
		Assert.That(d.A, Is.EqualTo(4));
	}


	/// <summary>NULL values are written as NULL and read as NULL.</summary>
	[Test]
	public void Null_Members()
	{
		Person p = new() { Home = null, Birthday = null };
		DdnElement ddn = Ddn.Serialize(p);

		Assert.That(ddn["Home"].IsNull, Is.True);
		Assert.That(ddn["Birthday"].IsNull, Is.True);

		Person? r = new() { Home = new Address(), Birthday = DateTime.Now };
		Ddn.Deserialize(ref r, Ddn.Create(ddn.ToString()));

		Assert.That(r!.Home, Is.Null);
		Assert.That(r.Birthday, Is.Null);
	}


	/// <summary>Invalid values do not overwrite existing values.</summary>
	[Test]
	public void InvalidValue_KeepsExisting()
	{
		Defaults d = Ddn.Deserialize<Defaults>(Ddn.Create("A = abc;"))!;
		Assert.That(d.A, Is.EqualTo(17));
	}


	/// <summary>Enumerations are read from names and numbers.</summary>
	[Test]
	public void Enum_NamesAndNumbers()
	{
		Assert.That(Ddn.Deserialize<Person>(Ddn.Create("DayOff = 3;"))!.DayOff, Is.EqualTo(DayOfWeek.Wednesday));
		Assert.That(Ddn.Deserialize<Person>(Ddn.Create("DayOff = monday;"))!.DayOff, Is.EqualTo(DayOfWeek.Monday));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: root values                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Scalars are written to the element "value" of a root element.</summary>
	[Test]
	public void RootScalar_UsesValueElement()
	{
		DdnElement ddn = Ddn.Serialize(42);

		Assert.That(ddn["value"].Get<int>(), Is.EqualTo(42));
		Assert.That(Ddn.Deserialize<int>(Ddn.Create(ddn.ToString())), Is.EqualTo(42));
		Assert.That(Ddn.Deserialize<string>(Ddn.Serialize("hello")), Is.EqualTo("hello"));
	}


	/// <summary>Arrays of scalars are written to the element "value" of a root element.</summary>
	[Test]
	public void RootArray_UsesValueElement()
	{
		DdnElement ddn = Ddn.Serialize(new[] { 1, 2, 3 });

		Assert.That(ddn["value"].Value, Is.EqualTo("1, 2, 3"));
		Assert.That(Ddn.Deserialize<List<int>>(ddn), Is.EqualTo(new[] { 1, 2, 3 }));
	}


	/// <summary>Non-root elements receive values directly.</summary>
	[Test]
	public void NonRoot_Scalar()
	{
		DdnElement ddn = Ddn.Create();
		Ddn.Serialize(5, ddn["a/b"]);

		Assert.That(ddn["a/b"].Get<int>(), Is.EqualTo(5));
		Assert.That(Ddn.Deserialize<int>(ddn["a/b"]), Is.EqualTo(5));
	}


	/// <summary>Lists of objects and dictionaries can be root objects.</summary>
	[Test]
	public void RootCollections()
	{
		List<Address> list = [new Address() { Street = "A" }, new Address() { Street = "B" }];
		Assert.That(_RoundTrip(list)!.Select(m => m.Street), Is.EqualTo(new[] { "A", "B" }));

		Dictionary<string, double> dict = new() { ["pi"] = 3.14, ["e"] = 2.72 };
		Assert.That(_RoundTrip(dict), Is.EquivalentTo(dict));
	}


	/// <summary>Missing elements return the default value.</summary>
	[Test]
	public void Missing_ReturnsDefault()
	{
		DdnElement ddn = Ddn.Create();

		Assert.That(Ddn.Deserialize<Person>(ddn["nothing"]), Is.Null);
		Assert.That(Ddn.Deserialize<int>(ddn["nothing"]), Is.EqualTo(0));
		Assert.That(Ddn.Deserialize(typeof(Person), ddn["nothing"]), Is.Null);
	}


	/// <summary>Serializing replaces existing contents.</summary>
	[Test]
	public void Serialize_ReplacesContents()
	{
		DdnElement ddn = Ddn.Create("old = 1; s { x = 2; }");
		Ddn.Serialize(new Defaults(), ddn["s"]);

		Assert.That(ddn["s/x"].Exists, Is.False);
		Assert.That(ddn["s/A"].Get<int>(), Is.EqualTo(17));
		Assert.That(ddn["old"].Get<int>(), Is.EqualTo(1));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: construction                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Records are created with their constructor.</summary>
	[Test]
	public void Record_RoundTrip()
	{
		Point p = _RoundTrip(new Point(3, -4))!;
		Assert.That(p, Is.EqualTo(new Point(3, -4)));

		List<Point> list = _RoundTrip(new List<Point>() { new(1, 2), new(3, 4) })!;
		Assert.That(list, Is.EqualTo(new[] { new Point(1, 2), new Point(3, 4) }));
	}


	/// <summary>Constructor parameters without elements receive their default values.</summary>
	[Test]
	public void Constructor_DefaultParameters()
	{
		Immutable i = Ddn.Deserialize<Immutable>(Ddn.Create("Name = n;"))!;

		Assert.That(i.Name, Is.EqualTo("n"));
		Assert.That(i.Count, Is.EqualTo(5));
	}


	/// <summary>Computed properties are not serialized.</summary>
	[Test]
	public void ComputedProperties_NotSerialized()
	{
		DdnElement ddn = Ddn.Serialize(new Immutable("a", 1));

		Assert.That(ddn.Contains("Computed"), Is.False);
		Assert.That(_RoundTrip(new Immutable("a", 1))!.Computed, Is.EqualTo("a1"));
	}


	/// <summary>Structs are supported as members and nullable members.</summary>
	[Test]
	public void Structs()
	{
		Shape s = _RoundTrip(new Shape() { Size = new Size() { Width = 3, Height = 4 }, MaxSize = new Size() { Width = 10 } })!;

		Assert.That(s.Size.Width, Is.EqualTo(3));
		Assert.That(s.Size.Height, Is.EqualTo(4));
		Assert.That(s.MaxSize!.Value.Width, Is.EqualTo(10));
		Assert.That(_RoundTrip(new Shape())!.MaxSize, Is.Null);
	}


	/// <summary>Members of type DdnElement are copied.</summary>
	[Test]
	public void DdnElementMember()
	{
		Shape s = new() { Data = Ddn.Create("a = 1; b { c = 2; }") };
		DdnElement ddn = Ddn.Serialize(s);

		Assert.That(ddn["Data/b/c"].Get<int>(), Is.EqualTo(2));

		Shape r = Ddn.Deserialize<Shape>(Ddn.Create(ddn.ToString()))!;
		Assert.That(r.Data!["b/c"].Get<int>(), Is.EqualTo(2));
		Assert.That(r.Data.IsRoot, Is.True);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: member selection                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Accessibility, attributes, methods and fields are handled as documented.</summary>
	[Test]
	public void MemberSelection()
	{
		Accessors a = new() { Original = 7, WriteOnly = 8, Internal = 9, _Stored = "Abc" };
		a.SetPrivate(4);
		a.SetSecret(6);
		a.Filled.Add(1);
		a.Inner.Street = "S";

		DdnElement ddn = Ddn.Serialize(a);

		Assert.That(ddn["PrivateSet"].Get<int>(), Is.EqualTo(4));
		Assert.That(ddn["GetOnly"].Exists, Is.True);
		Assert.That(ddn["Filled"].Value, Is.EqualTo("1"));
		Assert.That(ddn["Inner/Street"].Value, Is.EqualTo("S"));
		Assert.That(ddn.Contains("Ignored"), Is.False);
		Assert.That(ddn["renamed"].Get<int>(), Is.EqualTo(7));
		Assert.That(ddn.Contains("Original"), Is.False);
		Assert.That(ddn["WriteOnly"].Get<int>(), Is.EqualTo(8));
		Assert.That(ddn.Contains("Internal"), Is.False);
		Assert.That(ddn.Contains("Item"), Is.False);
		Assert.That(ddn.Contains("Callback"), Is.False);
		Assert.That(ddn.Contains("Field"), Is.False);
		Assert.That(ddn["secret"].Get<int>(), Is.EqualTo(6));
		Assert.That(ddn["stored"].Value, Is.EqualTo("ABC"));
	}


	/// <summary>Private setters, get-only auto-properties, fields, methods and fill-only members are deserialized.</summary>
	[Test]
	public void MemberSelection_Deserialize()
	{
		Accessors a = Ddn.Deserialize<Accessors>(Ddn.Create("PrivateSet = 1; GetOnly = 2; Filled = 3, 4; Inner { Street = s; } " +
														   "Ignored = 5; renamed = 6; WriteOnly = 7; Internal = 8; secret = 9; stored = XyZ;"))!;

		Assert.That(a.PrivateSet, Is.EqualTo(1));
		Assert.That(a.GetOnly, Is.EqualTo(2));
		Assert.That(a.Filled, Is.EqualTo(new[] { 3, 4 }));
		Assert.That(a.Inner.Street, Is.EqualTo("s"));
		Assert.That(a.Ignored, Is.EqualTo(1));
		Assert.That(a.Original, Is.EqualTo(6));
		Assert.That(a.WriteOnly, Is.EqualTo(0));
		Assert.That(a.Internal, Is.EqualTo(0));
		Assert.That(a.GetSecret(), Is.EqualTo(9));
		Assert.That(a._Stored, Is.EqualTo("xyz"));
	}


	/// <summary>Fill-only collections are replaced, not appended.</summary>
	[Test]
	public void FillOnlyCollection_Cleared()
	{
		Accessors? a = new();
		a.Filled.AddRange([1, 2, 3]);

		Ddn.Deserialize(ref a, Ddn.Create("Filled = 9;"));
		Assert.That(a!.Filled, Is.EqualTo(new[] { 9 }));
	}


	/// <summary>The serializable attribute selects fields and non-public members.</summary>
	[Test]
	public void SerializableAttribute_Fields()
	{
		WithFields w = new() { A = 1, B = "b", P = 2 };
		DdnElement ddn = Ddn.Serialize(w);

		Assert.That(ddn["A"].Get<int>(), Is.EqualTo(1));
		Assert.That(ddn["_B"].Value, Is.EqualTo("b"));
		Assert.That(ddn.Contains("P"), Is.False);
		Assert.That(ddn.Contains("B"), Is.False);

		WithFields r = Ddn.Deserialize<WithFields>(Ddn.Create(ddn.ToString()))!;
		Assert.That(r.A, Is.EqualTo(1));
		Assert.That(r.B, Is.EqualTo("b"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: collections                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Collections of different kinds survive a round trip.</summary>
	[Test]
	public void Collections_RoundTrip()
	{
		Collections c = new()
		{
			Array = [1, 2, 3],
			Nullables = [1, null, 3],
			Nested = [[1, 2], [], [3]],
			Objects = [new Address() { Street = "a" }, null, new Address() { Street = "c" }],
			Set = ["x", "y"],
			ReadOnly = ["r1", "r2"],
			ByNumber = new() { [10] = new Address() { Street = "ten" }, [2] = new Address() { Street = "two" } },
			Map = new Dictionary<string, string>() { ["k"] = "v", ["{odd} = key;"] = "w" },
			Lists = new() { ["a"] = [1, 2], ["b"] = [] }
		};

		Collections r = _RoundTrip(c)!;

		Assert.That(r.Array, Is.EqualTo(c.Array));
		Assert.That(r.Nullables, Is.EqualTo(c.Nullables));
		Assert.That(r.Nested, Is.EqualTo(c.Nested));
		Assert.That(r.Objects, Has.Count.EqualTo(3));
		Assert.That(r.Objects[1], Is.Null);
		Assert.That(r.Objects[2]!.Street, Is.EqualTo("c"));
		Assert.That(r.Set, Is.EquivalentTo(c.Set));
		Assert.That(r.ReadOnly, Is.EqualTo(c.ReadOnly));
		Assert.That(r.ByNumber[10].Street, Is.EqualTo("ten"));
		Assert.That(r.ByNumber[2].Street, Is.EqualTo("two"));
		Assert.That(r.Map, Is.EquivalentTo(c.Map));
		Assert.That(r.Lists["a"], Is.EqualTo(new[] { 1, 2 }));
		Assert.That(r.Lists["b"], Is.Empty);
	}


	/// <summary>List items are ordered by index, not by order of appearance.</summary>
	[Test]
	public void ListItems_OrderedByIndex()
	{
		List<Address> list = Ddn.Deserialize<List<Address>>(Ddn.Create("10 { Street = c; } 2 { Street = b; } 1 { Street = a; }"))!;
		Assert.That(list.Select(m => m.Street), Is.EqualTo(new[] { "a", "b", "c" }));
	}


	/// <summary>Lists of scalars can also be read from sections.</summary>
	[Test]
	public void ScalarList_FromSection()
	{
		Assert.That(Ddn.Deserialize<int[]>(Ddn.Create("x { 0 = 5; 1 = 6; }")["x"]), Is.EqualTo(new[] { 5, 6 }));
	}


	/// <summary>Lists of objects with scalars are written as values.</summary>
	[Test]
	public void ObjectList_OfScalars()
	{
		DdnElement ddn = Ddn.Create();
		ddn["x"].Set(new List<object?>() { 1, "a", null });

		Assert.That(ddn.ToString(), Does.Contain(@"x = 1, a, \0;"));
		Assert.That(ddn["x"].Get<List<object?>>(), Is.EqualTo(new object?[] { "1", "a", null }));
	}


	/// <summary>Invalid dictionary keys throw.</summary>
	[Test]
	public void InvalidKey_Throws()
	{
		Assert.Throws<Ddn.SerializeException>(() => Ddn.Deserialize<Dictionary<int, int>>(Ddn.Create("abc = 1;")));
		Assert.Throws<Ddn.SerializeException>(() => Ddn.Serialize(new Dictionary<Address, int>() { [new Address()] = 1 }));
	}


	/// <summary>Multi-dimensional arrays are not supported.</summary>
	[Test]
	public void MultiDimensionalArray_Throws()
	{
		Assert.Throws<Ddn.SerializeException>(() => Ddn.Serialize(new Address[2, 2]));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: type names                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Type names are not written by default; abstract types cannot be created.</summary>
	[Test]
	public void TypeNames_None()
	{
		Zoo z = new() { Star = new Dog() { Name = "Rex" } };
		DdnElement ddn = Ddn.Serialize(z);

		Assert.That(ddn.Contains("Star/$type"), Is.False);
		Assert.Throws<Ddn.SerializeException>(() => Ddn.Deserialize<Zoo>(ddn));
	}


	/// <summary>Automatic type names are written for derived types only.</summary>
	[Test]
	public void TypeNames_Auto()
	{
		Ddn.SerializationOptions opt = new() { TypeNames = Ddn.TypeNameHandling.AUTO };
		Zoo z = new() { Star = new Cat() { Name = "Tom", Lives = 9 }, Animals = [new Dog() { Name = "Rex", Barks = true }, new Cat() { Name = "Kit" }] };

		DdnElement ddn = Ddn.Serialize(z, opt);
		Assert.That(ddn.Contains("$type"), Is.False);
		Assert.That(ddn["Star/$type"].Value, Does.StartWith(typeof(Cat).FullName!));

		Zoo r = Ddn.Deserialize<Zoo>(Ddn.Create(ddn.ToString()), opt)!;
		Assert.That(r.Star, Is.TypeOf<Cat>());
		Assert.That(((Cat) r.Star!).Lives, Is.EqualTo(9));
		Assert.That(r.Animals[0], Is.TypeOf<Dog>());
		Assert.That(((Dog) r.Animals[0]).Barks, Is.True);
		Assert.That(r.Animals[1], Is.TypeOf<Cat>());
	}


	/// <summary>All type names are written with handling ALL.</summary>
	[Test]
	public void TypeNames_All()
	{
		Ddn.SerializationOptions opt = new() { TypeNames = Ddn.TypeNameHandling.ALL };
		DdnElement ddn = Ddn.Serialize(new Person() { Home = new Address() }, opt);

		Assert.That(ddn.Contains("$type"), Is.True);
		Assert.That(ddn.Contains("Home/$type"), Is.True);
		Assert.That(ddn["Age"].IsSection, Is.False);
		Assert.That(Ddn.Deserialize<Person>(ddn, opt)!.Home, Is.Not.Null);
	}


	/// <summary>Type names must be assignable to the declared type.</summary>
	[Test]
	public void TypeNames_NotAssignable_Throws()
	{
		Ddn.SerializationOptions opt = new() { TypeNames = Ddn.TypeNameHandling.AUTO };
		DdnElement ddn = Ddn.Create();
		ddn["Star/$type"] = (typeof(Address).FullName + ", " + typeof(Address).Assembly.GetName().Name);

		Assert.Throws<Ddn.SerializeException>(() => Ddn.Deserialize<Zoo>(ddn, opt));
	}


	/// <summary>Unknown type names throw.</summary>
	[Test]
	public void TypeNames_Unknown_Throws()
	{
		Ddn.SerializationOptions opt = new() { TypeNames = Ddn.TypeNameHandling.AUTO };
		Assert.Throws<Ddn.SerializeException>(() => Ddn.Deserialize<Zoo>(Ddn.Create("Star { $type = No.Such.Type; }"), opt));
	}


	/// <summary>Type names are ignored when reading with handling NONE.</summary>
	[Test]
	public void TypeNames_IgnoredWithNone()
	{
		DdnElement ddn = Ddn.Serialize(new Address() { Street = "s" }, new Ddn.SerializationOptions() { TypeNames = Ddn.TypeNameHandling.ALL });
		Address a = Ddn.Deserialize<Address>(ddn)!;

		Assert.That(a.Street, Is.EqualTo("s"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: errors                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Circular references throw.</summary>
	[Test]
	public void CircularReference_Throws()
	{
		Node n = new();
		n.Next = new Node() { Next = n };

		Ddn.SerializeException ex = Assert.Throws<Ddn.SerializeException>(() => Ddn.Serialize(n))!;
		Assert.That(ex.Message, Does.Contain("Next/Next"));
	}


	/// <summary>The same object may appear more than once if there is no cycle.</summary>
	[Test]
	public void SharedReference_Allowed()
	{
		Address a = new() { Street = "same" };
		DdnElement ddn = Ddn.Serialize(new List<Address>() { a, a });

		Assert.That(ddn["1/Street"].Value, Is.EqualTo("same"));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: custom serializers                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom serializers can write values.</summary>
	[Test]
	public void CustomSerializer_Class()
	{
		Ddn.RegisterSerializer(new ColorSerializer());

		Theme t = new() { Fore = new Color() { R = 255, G = 16, B = 1 }, Palette = [new Color() { B = 255 }] };
		DdnElement ddn = Ddn.Serialize(t);

		Assert.That(ddn["Fore"].Value, Is.EqualTo("#FF1001"));
		Assert.That(ddn["Palette/0"].Value, Is.EqualTo("#0000FF"));

		Theme r = Ddn.Deserialize<Theme>(Ddn.Create(ddn.ToString()))!;
		Assert.That(r.Fore.R, Is.EqualTo(255));
		Assert.That(r.Fore.G, Is.EqualTo(16));
		Assert.That(r.Palette[0].B, Is.EqualTo(255));
	}


	/// <summary>Custom serializers can be registered with delegates and use the context.</summary>
	[Test]
	public void CustomSerializer_Delegates()
	{
		Ddn.RegisterSerializer<Size>((v, e, c) => e.Set(v.Width + "x" + v.Height),
									 (e, x, c) => { string[] p = e.Get<string>().Split('x'); return new Size() { Width = int.Parse(p[0]), Height = int.Parse(p[1]) }; });

		DdnElement ddn = Ddn.Serialize(new Shape() { Size = new Size() { Width = 2, Height = 3 } });
		Assert.That(ddn["Size"].Value, Is.EqualTo("2x3"));
		Assert.That(Ddn.Deserialize<Shape>(ddn)!.Size.Height, Is.EqualTo(3));

		Assert.That(Ddn.UnregisterSerializer(typeof(Size)), Is.True);
		Assert.That(Ddn.Serialize(new Shape() { Size = new Size() { Width = 2 } })["Size/Width"].Get<int>(), Is.EqualTo(2));
	}


	/// <summary>Custom serializers can extend the default member serialization.</summary>
	[Test]
	public void CustomSerializer_SerializeMembers()
	{
		Ddn.RegisterSerializer<Size>((v, e, c) => { c.SerializeMembers(v, e); e["Area"] = (v.Width * v.Height); },
									 (e, x, c) => (Size) c.DeserializeMembers(new Size(), e));

		DdnElement ddn = Ddn.Serialize(new Shape() { Size = new Size() { Width = 2, Height = 3 } });
		Assert.That(ddn["Size/Area"].Get<int>(), Is.EqualTo(6));
		Assert.That(Ddn.Deserialize<Shape>(ddn)!.Size.Width, Is.EqualTo(2));
	}


	/// <summary>Discovery registers marked serializers.</summary>
	[Test]
	public void Discover()
	{
		Assert.That(Ddn.Discover(new[] { typeof(ColorSerializer), typeof(Person), null }), Is.EqualTo(1));
		Assert.That(Ddn.Serialize(new Theme())["Fore"].Value, Is.EqualTo("#000000"));
		Assert.That(Ddn.Discover(typeof(DdnSerializerTests).Assembly), Is.GreaterThanOrEqualTo(1));
	}


	/// <summary>Exceptions in custom serializers are wrapped.</summary>
	[Test]
	public void CustomSerializer_ExceptionWrapped()
	{
		Ddn.RegisterSerializer<Color>((v, e, c) => e.Set("bad"), (e, x, c) => throw new FormatException());
		DdnElement ddn = Ddn.Serialize(new Theme());

		Ddn.SerializeException ex = Assert.Throws<Ddn.SerializeException>(() => Ddn.Deserialize<Theme>(ddn))!;
		Assert.That(ex.InnerException, Is.TypeOf<FormatException>());
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests: element integration                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Element Get and Set serialize complex types.</summary>
	[Test]
	public void Element_GetSet_Complex()
	{
		DdnElement ddn = Ddn.Create();
		ddn["people/debbie"].Set(_CreatePerson());
		ddn.Set("config/scores", new Dictionary<string, int>() { ["a"] = 1 });

		Assert.That(ddn["people/debbie/Home/City"].Value, Is.EqualTo("Vienna"));
		Assert.That(ddn.Get<Person>("people/debbie").Age, Is.EqualTo(37));
		Assert.That(ddn["config/scores"].Get<Dictionary<string, int>>()["a"], Is.EqualTo(1));
		Assert.That(ddn["people/nobody"].Get<Person>(), Is.Null);
		Assert.That(ddn.Get("people/x", new Defaults()).A, Is.EqualTo(17));
		Assert.That(ddn["people/debbie"].TryGet(out Address _), Is.True);
	}


	/// <summary>Scalar lists still are values and scalar reads of sections return NULL.</summary>
	[Test]
	public void Element_Scalars_Unchanged()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; }");
		ddn["l"].Set(new List<int>() { 1, 2 });

		Assert.That(ddn["l"].IsSection, Is.False);
		Assert.That(ddn.Get<int?>("s"), Is.Null);
		Assert.That(ddn.Get<string>("s"), Is.Null);
	}
}
