namespace Robbiblubber.Spellbook.UnitTests.Core.DdnFormat;

using NUnit.Framework;



/// <summary>This class tests paths and tree operations of ddn elements.</summary>
[TestFixture]
public class DdnElementTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reading a missing path does not change the document.</summary>
	[Test]
	public void Indexer_ReadMissingPath_DoesNotCreate()
	{
		DdnElement ddn = Ddn.Create();

		int n = ddn["a/b/c"];
		DdnElement elm = ddn["x"]["y"];

		Assert.That(n, Is.EqualTo(0));
		Assert.That(elm.Exists, Is.False);
		Assert.That(elm.IsNull, Is.True);
		Assert.That(ddn.ElementCount, Is.EqualTo(0));
		Assert.That(ddn.ToString(), Is.Empty);
	}


	/// <summary>Writing to a missing path creates the path.</summary>
	[Test]
	public void Indexer_WriteMissingPath_CreatesPath()
	{
		DdnElement ddn = Ddn.Create();

		ddn["layout"]["main"]["window"] = new[] { 1, 2, 3 };
		ddn["a/b/c"] = 5;

		Assert.That(ddn.Get<int[]>("layout/main/window"), Is.EqualTo(new[] { 1, 2, 3 }));
		Assert.That(ddn.Get<int>("a/b/c"), Is.EqualTo(5));
		Assert.That(ddn["layout"].IsSection, Is.True);
		Assert.That(ddn["a/b"].Exists, Is.True);
	}


	/// <summary>Several handles to the same missing path write to the same element.</summary>
	[Test]
	public void Indexer_TwoHandlesToMissingPath_ShareElement()
	{
		DdnElement ddn = Ddn.Create();
		DdnElement a = ddn["x"];
		DdnElement b = ddn["x"];

		a["p"] = 1;
		b["q"] = 2;
		a["r"] = 3;

		Assert.That(ddn["x"].Elements.Select(m => m.Name), Is.EqualTo(new[] { "p", "q", "r" }));
		Assert.That(b.Get<int>("r"), Is.EqualTo(3));
		Assert.That(b.ElementCount, Is.EqualTo(3));
	}


	/// <summary>A handle to a missing element sees the element after it has been created elsewhere.</summary>
	[Test]
	public void Indexer_HandleSeesLaterCreatedElement()
	{
		DdnElement ddn = Ddn.Create();
		DdnElement h = ddn["s/v"];

		ddn["s/v"] = "hello";

		Assert.That(h.Exists, Is.True);
		Assert.That(h.Get<string>(), Is.EqualTo("hello"));
	}


	/// <summary>Relative paths with "." and "..".</summary>
	[Test]
	public void Indexer_DotPaths()
	{
		DdnElement ddn = Ddn.Create("a { b { c = 1; } d = 2; } e = 3;");
		DdnElement b = ddn["a/b"];

		Assert.That(b.Get<int>("./c"), Is.EqualTo(1));
		Assert.That(b.Get<int>("../d"), Is.EqualTo(2));
		Assert.That(b.Get<int>("../../e"), Is.EqualTo(3));
		Assert.That(ddn.Get<int>("a/b/../d"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("a/b/../../e"), Is.EqualTo(3));
		Assert.That(ddn.Get<int>("../e"), Is.EqualTo(3));
	}


	/// <summary>Absolute paths start at the root element.</summary>
	[Test]
	public void Indexer_AbsolutePaths()
	{
		DdnElement ddn = Ddn.Create("a { b { c = 1; } } e = 3;");
		DdnElement b = ddn["a/b"];

		Assert.That(b.Get<int>("/e"), Is.EqualTo(3));
		Assert.That(b.Get<int>("/a/b/c"), Is.EqualTo(1));
		Assert.That(b["/"], Is.SameAs(ddn));

		b["/f"] = 4;
		Assert.That(ddn.Get<int>("f"), Is.EqualTo(4));
	}


	/// <summary>Path and root of elements.</summary>
	[Test]
	public void Path_And_Root()
	{
		DdnElement ddn = Ddn.Create("a { b { c = 1; } }");
		DdnElement c = ddn["a/b/c"];

		Assert.That(c.Path, Is.EqualTo("/a/b/c"));
		Assert.That(c.Root, Is.SameAs(ddn));
		Assert.That(c.Parent!.Name, Is.EqualTo("b"));
		Assert.That(ddn.IsRoot, Is.True);
		Assert.That(c.IsRoot, Is.False);
		Assert.That(ddn.Path, Is.EqualTo("/"));
		Assert.That(ddn["x/y"].Path, Is.EqualTo("/x/y"));
	}


	/// <summary>Names with slashes are masked in paths.</summary>
	[Test]
	public void Path_MaskedNames()
	{
		DdnElement ddn = Ddn.Create();
		ddn.Element("a/b").Set(1);

		Assert.That(ddn.Element("a/b").Path, Is.EqualTo(@"/a\/b"));
		Assert.That(ddn[ddn.Element("a/b").Path].Get<int>(), Is.EqualTo(1));
		Assert.That(ddn.Contains("a"), Is.False);
	}


	/// <summary>Elements keep their order.</summary>
	[Test]
	public void Elements_KeepOrder()
	{
		DdnElement ddn = Ddn.Create();
		ddn["z"] = 1;
		ddn["a"] = 2;
		ddn["m"] = 3;

		Assert.That(ddn.Elements.Select(m => m.Name), Is.EqualTo(new[] { "z", "a", "m" }));
	}


	/// <summary>Sections and values are separated.</summary>
	[Test]
	public void SectionsAndValues()
	{
		DdnElement ddn = Ddn.Create("a = 1; s { x = 1; } b = 2; t { }");

		Assert.That(ddn.Values.Select(m => m.Name), Is.EqualTo(new[] { "a", "b" }));
		Assert.That(ddn.Sections.Select(m => m.Name), Is.EqualTo(new[] { "s", "t" }));
		Assert.That(ddn.HasElements, Is.True);
		Assert.That(ddn.ElementCount, Is.EqualTo(4));
		Assert.That(((IEnumerable<DdnElement>) ddn).Count(), Is.EqualTo(4));
	}


	/// <summary>Contains checks paths.</summary>
	[Test]
	public void Contains()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; }");

		Assert.That(ddn.Contains("a"), Is.True);
		Assert.That(ddn.Contains("a/b"), Is.True);
		Assert.That(ddn.Contains("a/c"), Is.False);
		Assert.That(ddn.Contains("x/y"), Is.False);
		Assert.That(ddn["a"].Contains("../a/b"), Is.True);
	}


	/// <summary>Removed elements become root elements and can be added again.</summary>
	[Test]
	public void Remove()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; c = 2; }");
		DdnElement b = ddn["a/b"];

		Assert.That(b.Remove(), Is.True);
		Assert.That(ddn.Contains("a/b"), Is.False);
		Assert.That(b.IsRoot, Is.True);
		Assert.That(b.Get<int>(), Is.EqualTo(1));
		Assert.That(b.Remove(), Is.False);

		Assert.That(ddn.Remove("a/c"), Is.True);
		Assert.That(ddn.Remove("a/c"), Is.False);
		Assert.That(ddn.Remove("x/y"), Is.False);
		Assert.That(ddn["a"].HasElements, Is.False);

		ddn["a/b"] = 7;
		Assert.That(ddn.Get<int>("a/b"), Is.EqualTo(7));
		Assert.That(b.Get<int>(), Is.EqualTo(1));
	}


	/// <summary>Clear removes children and sets NULL.</summary>
	[Test]
	public void Clear()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; } c = 2;");

		ddn["a"].Clear();
		ddn["c"].Clear();
		ddn["missing"].Clear();

		Assert.That(ddn["a"].HasElements, Is.False);
		Assert.That(ddn["a"].IsNull, Is.True);
		Assert.That(ddn["c"].IsNull, Is.True);
		Assert.That(ddn.Contains("missing"), Is.False);
	}


	/// <summary>Setting a value on a section removes its children.</summary>
	[Test]
	public void Set_ValueOnSection_RemovesChildren()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; }");
		DdnElement b = ddn["a/b"];

		ddn["a"] = 5;

		Assert.That(ddn["a"].IsSection, Is.False);
		Assert.That(ddn.Get<int>("a"), Is.EqualTo(5));
		Assert.That(b.IsRoot, Is.True);
	}


	/// <summary>Setting NULL on a section removes its children.</summary>
	[Test]
	public void SetNull_OnSection()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; }");
		ddn["a"].SetNull();

		Assert.That(ddn["a"].IsNull, Is.True);
		Assert.That(ddn["a"].HasElements, Is.False);
		Assert.That(ddn.ToString(), Is.EqualTo("a = \\0;\r\n"));
	}


	/// <summary>Assigning an element copies its contents.</summary>
	[Test]
	public void Set_Element_CopiesContents()
	{
		DdnElement src = Ddn.Create("s { a = 1; t { b = 2; } }");
		DdnElement ddn = Ddn.Create();

		ddn["copy"] = src["s"];
		src["s/a"] = 9;

		Assert.That(ddn.Get<int>("copy/a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("copy/t/b"), Is.EqualTo(2));
		Assert.That(src.Get<int>("s/t/b"), Is.EqualTo(2));
	}


	/// <summary>Assigning an element to one of its descendants works.</summary>
	[Test]
	public void Set_AncestorToDescendant()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; }");
		ddn["s/inner"] = ddn["s"];

		Assert.That(ddn.Get<int>("s/inner/a"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
	}


	/// <summary>Assigning an element to itself does nothing.</summary>
	[Test]
	public void Set_Itself()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; }");
		ddn["s"] = ddn["s"];

		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
	}


	/// <summary>Renaming updates the parent.</summary>
	[Test]
	public void Name_Rename()
	{
		DdnElement ddn = Ddn.Create("a = 1; b = 2;");
		ddn["a"].Name = "c";

		Assert.That(ddn.Contains("a"), Is.False);
		Assert.That(ddn.Get<int>("c"), Is.EqualTo(1));
		Assert.That(ddn.Elements.Select(m => m.Name), Is.EqualTo(new[] { "c", "b" }));
		Assert.Throws<ArgumentException>(() => ddn["c"].Name = "b");
	}


	/// <summary>Moving elements.</summary>
	[Test]
	public void MoveTo()
	{
		DdnElement ddn = Ddn.Create("a { x = 1; } b { x = 2; y = 3; }");

		ddn["b/x"].MoveTo(ddn["a"]);
		ddn["b/y"].MoveTo(ddn["c"]);

		Assert.That(ddn.Get<int>("a/x"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("c/y"), Is.EqualTo(3));
		Assert.That(ddn["b"].HasElements, Is.False);
		Assert.Throws<ArgumentException>(() => ddn["a"].MoveTo(ddn["a"]));
		Assert.Throws<ArgumentException>(() => ddn["c"].MoveTo(ddn["c/y"]));
	}


	/// <summary>Copying elements.</summary>
	[Test]
	public void CopyTo()
	{
		DdnElement ddn = Ddn.Create("a { x = 1; } b { }");

		DdnElement copy = ddn["a"].CopyTo(ddn["b"]);
		copy["x"] = 5;

		Assert.That(ddn.Get<int>("a/x"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("b/a/x"), Is.EqualTo(5));
	}


	/// <summary>Merging elements.</summary>
	[Test]
	public void Merge()
	{
		DdnElement ddn = Ddn.Create("a = 1; s { x = 1; y = 1; t { p = 1; } }");
		DdnElement defaults = Ddn.Create("a = 2; b = 2; s { y = 2; z = 2; t { q = 2; } }");

		ddn.Merge(defaults);

		Assert.That(ddn.Get<int>("a"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("b"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("s/x"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/y"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("s/z"), Is.EqualTo(2));
		Assert.That(ddn.Get<int>("s/t/p"), Is.EqualTo(1));
		Assert.That(ddn.Get<int>("s/t/q"), Is.EqualTo(2));
		Assert.That(defaults.Get<int>("s/t/q"), Is.EqualTo(2));
	}


	/// <summary>Cloning creates an independent copy.</summary>
	[Test]
	public void Clone()
	{
		DdnElement ddn = Ddn.Create("s { a = 1; t { } }");
		DdnElement copy = ddn["s"].Clone();

		copy["a"] = 2;

		Assert.That(copy.IsRoot, Is.True);
		Assert.That(copy.Name, Is.EqualTo("s"));
		Assert.That(copy["t"].IsSection, Is.True);
		Assert.That(ddn.Get<int>("s/a"), Is.EqualTo(1));
	}


	/// <summary>The value property returns the plain string.</summary>
	[Test]
	public void Value()
	{
		DdnElement ddn = Ddn.Create(@"a = x\,y; b = \0; s { }");

		Assert.That(ddn["a"].Value, Is.EqualTo("x,y"));
		Assert.That(ddn["b"].Value, Is.Null);
		Assert.That(ddn["s"].Value, Is.Null);
		Assert.That(ddn["missing"].Value, Is.Null);

		ddn["c"].Value = "a;b";
		Assert.That(ddn.ToString(), Does.Contain(@"c = a\;b;"));
	}


	/// <summary>Enumerating while modifying works on a snapshot.</summary>
	[Test]
	public void Enumerate_WhileModifying()
	{
		DdnElement ddn = Ddn.Create("a = 1; b = 2; c = 3;");

		foreach(DdnElement i in ddn) { i.Remove(); }

		Assert.That(ddn.ElementCount, Is.EqualTo(0));
	}

	/// <summary>Child elements by name.</summary>
	[Test]
	public void Element_ByName()
	{
		DdnElement ddn = Ddn.Create("a { b = 1; }");
		DdnElement x = ddn.Element("x/y");

		Assert.That(ddn["a"].Element("b").Get<int>(), Is.EqualTo(1));
		Assert.That(x.Exists, Is.False);

		x["z"] = 2;
		Assert.That(ddn.Element("x/y").Element("z").Get<int>(), Is.EqualTo(2));
		Assert.That(ddn.Contains("x"), Is.False);
	}


	/// <summary>Elements created from text are root elements.</summary>
	[Test]
	public void Constructor_Text()
	{
		DdnElement ddn = new("a = 1; s { b = 2; }");

		Assert.That(ddn.IsRoot, Is.True);
		Assert.That(ddn.Exists, Is.True);
		Assert.That(ddn.Get<int>("s/b"), Is.EqualTo(2));
		Assert.That(new DdnElement().ElementCount, Is.EqualTo(0));
	}
}
