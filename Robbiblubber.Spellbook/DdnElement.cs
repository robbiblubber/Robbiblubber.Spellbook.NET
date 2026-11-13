namespace Robbiblubber.Spellbook;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;



/// <summary>This class represents a ddn element.</summary>
/// <remarks><para>An element is either a value or a section that contains other elements. Elements are addressed by paths
///          (<c>section/subsection/value</c>); absolute paths start with a slash, <c>.</c> denotes the current element and
///          <c>..</c> the parent element.</para>
///          <para>Reading a path that does not exist returns an element that is not part of the document yet
///          (<see cref="Exists"/> is FALSE). The element and its missing parents are added to the document when a value or a
///          child element is written. Therefore <c>cfg["a"]["b"] = 5</c> creates the section <c>a</c>, while
///          <c>int n = cfg["x/y"]</c> leaves the document unchanged.</para>
///          <para>Child elements keep their order. The class is not thread-safe.</para></remarks>
public sealed partial class DdnElement: IEnumerable<DdnElement>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Element name.</summary>
	private string _Name;

	/// <summary>Parent element.</summary>
	private DdnElement? _Parent;

	/// <summary>Masked value text.</summary>
	private string _Text = _DdnText._NULL;

	/// <summary>Child elements in order.</summary>
	private List<DdnElement>? _Items = null;

	/// <summary>Child elements by name.</summary>
	private Dictionary<string, DdnElement>? _Index = null;

	/// <summary>Determines if the element is a section.</summary>
	private bool _Section = false;

	/// <summary>Determines if the element has not been added to its parent yet.</summary>
	private bool _Detached = false;

	/// <summary>Elements that have been defined more than once in parsed text.</summary>
	private List<DdnElement>? _Duplicates = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public DdnElement()
	{
		_Name = string.Empty;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">ddn text.</param>
	public DdnElement(string text): this()
	{
		Text = text;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="parent">Parent element.</param>
	/// <param name="name">Name.</param>
	/// <param name="detached">Determines if the element has not been added to its parent yet.</param>
	private DdnElement(DdnElement? parent, string name, bool detached)
	{
		_Parent = parent;
		_Name = name;
		_Detached = detached;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a detached value element.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the element.</returns>
	internal static DdnElement _FromValue(object? value)
	{
		DdnElement rval = new();
		rval._Text = _DdnValue._ToText(value);

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Compares element names, comparing digit sequences numerically.</summary>
	/// <param name="a">Name.</param>
	/// <param name="b">Name.</param>
	/// <returns>Returns a negative value if a precedes b, 0 if they are equal and a positive value if b precedes a.</returns>
	private static int _CompareNames(string a, string b)
	{
		int i = 0, k = 0;

		while((i < a.Length) && (k < b.Length))
		{
			if(char.IsAsciiDigit(a[i]) && char.IsAsciiDigit(b[k]))
			{
				int si = i, sk = k;
				while((i < a.Length) && char.IsAsciiDigit(a[i])) { i++; }
				while((k < b.Length) && char.IsAsciiDigit(b[k])) { k++; }

				string na = a[si..i].TrimStart('0'), nb = b[sk..k].TrimStart('0');
				int rval = ((na.Length != nb.Length) ? na.Length.CompareTo(nb.Length) : string.CompareOrdinal(na, nb));
				if(rval != 0) { return rval; }
			}
			else
			{
				int rval = a[i].CompareTo(b[k]);
				if(rval != 0) { return rval; }
				i++; k++;
			}
		}

		int rest = (a.Length - i).CompareTo(b.Length - k);
		return ((rest != 0) ? rest : string.CompareOrdinal(a, b));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the existing element this element represents.</summary>
	/// <remarks>Returns the element itself if it is part of the document, the element with the same path if this element has
	///          not been added yet but the path exists, or NULL.</remarks>
	private DdnElement? _Existing
	{
		get { return _GetExisting(false); }
	}


	/// <summary>Gets the existing element this element represents, adding it to the document if necessary.</summary>
	private DdnElement _Target
	{
		get { return _GetExisting(true)!; }
	}


	/// <summary>Gets the element that provides the contents of this element.</summary>
	private DdnElement _Source
	{
		get { return (_GetExisting(false) ?? this); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the existing element this element represents.</summary>
	/// <param name="create">Determines if the element is added to the document if it does not exist.</param>
	/// <returns>Returns the element, or NULL if it does not exist and has not been created.</returns>
	private DdnElement? _GetExisting(bool create)
	{
		if(!_Detached) { return this; }

		DdnElement? parent = _Parent!._GetExisting(create);
		if(parent is null) { return null; }
		if((parent._Index is not null) && parent._Index.TryGetValue(_Name, out DdnElement? rval)) { return rval; }
		if(!create) { return null; }

		_Parent = parent;
		_Detached = false;
		parent._Add(this);

		return this;
	}


	/// <summary>Adds a child element.</summary>
	/// <param name="child">Element.</param>
	/// <remarks>An existing element with the same name is replaced.</remarks>
	private void _Add(DdnElement child)
	{
		_Items ??= new();
		_Index ??= new(StringComparer.Ordinal);

		if(_Index.TryGetValue(child._Name, out DdnElement? existing))
		{
			_Items[_Items.IndexOf(existing)] = child;
			existing._Parent = null;
		}
		else { _Items.Add(child); }

		_Index[child._Name] = child;
		child._Parent = this;
		child._Detached = false;
		_Section = true;
	}


	/// <summary>Removes all child elements.</summary>
	private void _ClearItems()
	{
		if(_Items is not null)
		{
			foreach(DdnElement i in _Items) { i._Parent = null; }
		}

		_Items = null;
		_Index = null;
	}


	/// <summary>Gets a child element by name.</summary>
	/// <param name="name">Name.</param>
	/// <returns>Returns the child element; if it does not exist, a detached element is returned.</returns>
	private DdnElement _GetChild(string name)
	{
		DdnElement src = _Source;
		if((src._Index is not null) && src._Index.TryGetValue(name, out DdnElement? rval)) { return rval; }

		return new DdnElement(this, name, true);
	}


	/// <summary>Copies the contents of an element to this element.</summary>
	/// <param name="source">Source element.</param>
	private void _Assign(DdnElement source)
	{
		DdnElement copy = source.Clone();

		_ClearItems();
		_Text = copy._Text;
		_Section = copy._Section;

		if(copy._Items is not null)
		{
			foreach(DdnElement i in copy._Items.ToArray()) { _Add(i); }
		}
	}


	/// <summary>Writes the element.</summary>
	/// <param name="wr">Writer.</param>
	/// <param name="indent">Indentation level; -1 for the root element.</param>
	/// <param name="options">Options.</param>
	private void _Write(TextWriter wr, int indent, Ddn.Options options)
	{
		string ind = (options.MultiLine ? string.Concat(Enumerable.Repeat(options.Indent, Math.Max(indent, 0))) : string.Empty);
		string nl = (options.MultiLine ? options.NewLine : " ");

		if(indent >= 0)
		{
			if(!IsSection)
			{
				if(_Name.Length == 0) { return; }
				if((_Text == _DdnText._NULL) && options.IgnoreNullElements) { return; }

				wr.Write(ind);
				wr.Write(_DdnText._Mask(_Name));
				wr.Write(" = ");
				wr.Write(_Text);
				wr.Write(';');
				wr.Write(nl);
				return;
			}

			wr.Write(ind);
			wr.Write(_DdnText._Mask(_Name));
			wr.Write(options.MultiLine ? (nl + ind + "{" + nl) : " { ");
		}

		IEnumerable<DdnElement> items = (_Items ?? Enumerable.Empty<DdnElement>()).Where(m => m._Name.Length > 0);
		if(options.Ordered) { items = items.OrderBy(m => m._Name, Comparer<string>.Create(_CompareNames)); }

		bool written = false;
		foreach(DdnElement i in items.Where(m => !m.IsSection))
		{
			i._Write(wr, indent + 1, options);
			written = true;
		}

		foreach(DdnElement i in items.Where(m => m.IsSection))
		{
			if(written && options.MultiLine && options.SectionSpacing) { wr.Write(nl); }
			i._Write(wr, indent + 1, options);
			written = true;
		}

		if(indent >= 0)
		{
			wr.Write(ind);
			wr.Write("}");
			wr.Write(nl);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a child element for the parser, creating it if necessary.</summary>
	/// <param name="name">Name.</param>
	/// <param name="section">Determines if the child is a section.</param>
	/// <param name="duplicate">Returns if the element has been defined before.</param>
	/// <returns>Returns the element.</returns>
	/// <remarks>A section that has been defined before is reused and receives the new elements; a value replaces an existing
	///          element of the same name.</remarks>
	internal DdnElement _ParseChild(string name, bool section, out bool duplicate)
	{
		DdnElement? rval = null;
		duplicate = ((_Index is not null) && _Index.TryGetValue(name, out rval));

		if(rval is null)
		{
			rval = new DdnElement(this, name, false);
			_Add(rval);
		}

		if(section)
		{
			if(!rval.IsSection) { rval._Text = _DdnText._NULL; }
			rval._Section = true;
		}
		else
		{
			rval._ClearItems();
			rval._Section = false;
		}

		return rval;
	}


	/// <summary>Turns the element into an empty section.</summary>
	internal void _MakeSection()
	{
		DdnElement tar = _Target;

		tar._ClearItems();
		tar._Text = _DdnText._NULL;
		tar._Section = true;
	}


	/// <summary>Sets the masked value text of a value element.</summary>
	/// <param name="text">Masked text.</param>
	internal void _SetText(string text)
	{
		_Text = text;
	}


	/// <summary>Fills a list with the array items of the element.</summary>
	/// <param name="t">Item type.</param>
	/// <param name="list">List.</param>
	/// <returns>Returns the list.</returns>
	internal IList _GetList(Type t, IList list)
	{
		DdnElement src = _Source;
		if(src.IsNull || src.IsSection) { return list; }

		foreach(string i in _DdnText._SplitList(src._Text)) { list.Add(_DdnValue._ItemFromText(i, t)); }
		return list;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the element name.</summary>
	/// <exception cref="ArgumentException">Thrown when the parent element already contains an element with this name.</exception>
	public string Name
	{
		get { return _Name; }
		set
		{
			value ??= string.Empty;
			if(value == _Name) { return; }

			DdnElement? existing = _Existing;
			if((existing is not null) && (existing != this)) { existing.Name = value; }

			if((!_Detached) && (_Parent?._Index is not null))
			{
				if(_Parent._Index.ContainsKey(value)) { throw new ArgumentException("An element named \"" + value + "\" already exists.", nameof(value)); }

				_Parent._Index.Remove(_Name);
				_Parent._Index[value] = this;
			}

			_Name = value;
		}
	}


	/// <summary>Gets the parent element.</summary>
	public DdnElement? Parent
	{
		get { return _Parent; }
	}


	/// <summary>Gets the root element.</summary>
	public DdnElement Root
	{
		get
		{
			DdnElement rval = this;
			while(rval._Parent is not null) { rval = rval._Parent; }

			return rval;
		}
	}


	/// <summary>Gets the absolute path of the element.</summary>
	public string Path
	{
		get
		{
			List<string> rval = new();
			for(DdnElement? i = this; i?._Parent is not null; i = i._Parent) { rval.Insert(0, _DdnText._Mask(i._Name)); }

			return ("/" + string.Join('/', rval));
		}
	}


	/// <summary>Gets if the element is a root element.</summary>
	public bool IsRoot
	{
		get { return (_Parent is null); }
	}


	/// <summary>Gets if the element is part of the document.</summary>
	/// <remarks>Elements returned for paths that do not exist are added to the document when a value or child is written.</remarks>
	public bool Exists
	{
		get { return (_Existing is not null); }
	}


	/// <summary>Gets if the element is a section.</summary>
	/// <remarks>An element is a section if it contains elements or has been declared as a section in parsed text.</remarks>
	public bool IsSection
	{
		get
		{
			DdnElement src = _Source;
			return (src._Section || (src._Items?.Count > 0));
		}
	}


	/// <summary>Gets if the element has child elements.</summary>
	public bool HasElements
	{
		get { return (_Source._Items?.Count > 0); }
	}


	/// <summary>Gets the number of child elements.</summary>
	public int ElementCount
	{
		get { return (_Source._Items?.Count ?? 0); }
	}


	/// <summary>Gets the child elements.</summary>
	public IEnumerable<DdnElement> Elements
	{
		get { return (_Source._Items?.ToArray() ?? []); }
	}


	/// <summary>Gets the child elements that are sections.</summary>
	public IEnumerable<DdnElement> Sections
	{
		get { return Elements.Where(m => m.IsSection).ToArray(); }
	}


	/// <summary>Gets the child elements that are values.</summary>
	public IEnumerable<DdnElement> Values
	{
		get { return Elements.Where(m => !m.IsSection).ToArray(); }
	}


	/// <summary>Gets if the element is NULL.</summary>
	/// <remarks>Sections and elements that do not exist are NULL.</remarks>
	public bool IsNull
	{
		get
		{
			DdnElement src = _Source;
			return ((src._Text == _DdnText._NULL) && (!src.IsSection));
		}
	}


	/// <summary>Gets or sets the element value as a plain string.</summary>
	/// <remarks>NULL values and sections return NULL. Arrays are returned as they are written, with comma-separated items.</remarks>
	public string? Value
	{
		get
		{
			DdnElement src = _Source;
			return ((src.IsNull || src.IsSection) ? null : _DdnText._Unmask(src._Text));
		}
		set { Set(value); }
	}


	/// <summary>Gets or sets the ddn text of the element.</summary>
	/// <remarks>Setting the text replaces the contents of the element with the parsed elements.</remarks>
	public string Text
	{
		get { return ToString(); }
		set
		{
			DdnElement tar = _Target;

			tar._ClearItems();
			tar._Text = _DdnText._NULL;
			tar._Section = false;
			tar._Duplicates = new();

			_DdnParser._Parse(value ?? string.Empty, tar, tar._Duplicates);
		}
	}


	/// <summary>Gets or sets an element by its path.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns the element; if the path does not exist, an element that is not part of the document yet.</returns>
	/// <remarks>Setting an element copies the value or the contents of the assigned element to the element at the path; NULL
	///          sets the element to NULL.</remarks>
	[AllowNull]
	public DdnElement this[string path]
	{
		get
		{
			List<string> names = _DdnText._ParsePath(path ?? string.Empty, out bool absolute);
			DdnElement rval = (absolute ? Root : this);

			foreach(string i in names)
			{
				if(i == ".") { continue; }
				if(i == "..") { rval = (rval._Parent ?? rval); continue; }

				rval = rval._GetChild(i);
			}

			return rval;
		}
		set { this[path].Set(value); }
	}


	/// <summary>Gets the elements that have been defined more than once in the parsed text.</summary>
	/// <remarks>Duplicate values replace the earlier value, duplicate sections are merged.</remarks>
	public DdnElement[] ParsedDuplicates
	{
		get { return (_Source._Duplicates?.ToArray() ?? []); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a child element by name.</summary>
	/// <param name="name">Name; the name is not interpreted as a path.</param>
	/// <returns>Returns the element; if it does not exist, an element that is not part of the document yet.</returns>
	public DdnElement Element(string name)
	{
		return _GetChild(name ?? string.Empty);
	}


	/// <summary>Gets the element value.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns the value, or NULL if the element is NULL, does not exist or cannot be converted.</returns>
	/// <remarks>Scalar values and arrays of scalars are read from the element value. Other types are deserialized with the default
	///          serialization options, see <see cref="Ddn.Deserialize(Type, DdnElement, Ddn.SerializationOptions?)"/>.</remarks>
	/// <exception cref="Ddn.SerializeException">Thrown when the element cannot be deserialized into the type.</exception>
	public object? Get(Type t)
	{
		DdnElement src = _Source;

		if(!(_DdnValue._IsScalar(t) || ((!src.IsSection) && _DdnSerializer._IsValue(t))))
		{
			return (src.IsNull ? null : _DdnSerializer._Deserialize(new Ddn.SerializationContext(null), t, this, null));
		}

		if(src.IsSection) { return null; }
		return (_DdnValue._TryFromText(src._Text, t, out object? rval) ? rval : null);
	}


	/// <summary>Gets the value of an element.</summary>
	/// <param name="t">Type.</param>
	/// <param name="path">Path.</param>
	/// <returns>Returns the value, or NULL if the element is NULL, a section, does not exist or cannot be converted.</returns>
	public object? Get(Type t, string path)
	{
		return this[path].Get(t);
	}


	/// <summary>Tries to get the element value.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the element has a value of the type, FALSE if it is NULL, a section, does not exist or cannot be
	///          converted.</returns>
	public bool TryGet<T>(out T value)
	{
		object? rval = Get(typeof(T));
		value = ((rval is null) ? default! : (T) rval);

		return (rval is not null);
	}


	/// <summary>Gets the element value.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <returns>Returns the value, or the default value of the type if the element has no value of the type.</returns>
	public T Get<T>()
	{
		return (TryGet(out T rval) ? rval : default!);
	}


	/// <summary>Gets the value of an element.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="path">Path.</param>
	/// <returns>Returns the value, or the default value of the type if the element has no value of the type.</returns>
	public T Get<T>(string path)
	{
		return this[path].Get<T>();
	}


	/// <summary>Gets the value of an element.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="path">Path.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the element has no value of the type.</returns>
	public T Get<T>(string path, T defaultValue)
	{
		return this[path].GetWithDefault(defaultValue);
	}


	/// <summary>Gets the element value.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the element has no value of the type.</returns>
	public T GetWithDefault<T>(T defaultValue)
	{
		return (TryGet(out T rval) ? rval : defaultValue);
	}


	/// <summary>Sets the element value.</summary>
	/// <param name="value">Value; collections of scalars are written as ddn arrays, elements are copied, other objects are
	///                     serialized with the default serialization options.</param>
	/// <returns>Returns this element.</returns>
	/// <exception cref="Ddn.SerializeException">Thrown when the object cannot be serialized.</exception>
	public DdnElement Set(object? value)
	{
		DdnElement tar = _Target;

		if(value is DdnElement elm)
		{
			if(elm._Source != tar) { tar._Assign(elm._Source); }
			return this;
		}

		if((value is not null) && (!_DdnSerializer._IsValueInstance(value)))
		{
			_DdnSerializer._Serialize(new Ddn.SerializationContext(null), value, value.GetType(), tar);
			return this;
		}

		tar._ClearItems();
		tar._Section = false;
		tar._Text = _DdnValue._ToText(value);

		return this;
	}


	/// <summary>Sets the value of an element.</summary>
	/// <param name="path">Path.</param>
	/// <param name="value">Value; collections are written as ddn arrays, elements are copied.</param>
	/// <returns>Returns this element.</returns>
	public DdnElement Set(string path, object? value)
	{
		this[path].Set(value);
		return this;
	}


	/// <summary>Sets the element value to NULL.</summary>
	/// <returns>Returns this element.</returns>
	public DdnElement SetNull()
	{
		return Set(null);
	}


	/// <summary>Returns if an element exists.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns TRUE if the element exists, otherwise returns FALSE.</returns>
	public bool Contains(string path)
	{
		return this[path].Exists;
	}


	/// <summary>Removes the element from its parent.</summary>
	/// <returns>Returns TRUE if the element has been removed, FALSE if it was not part of a document.</returns>
	/// <remarks>The removed element becomes a root element.</remarks>
	public bool Remove()
	{
		DdnElement? src = _Existing;
		if(src?._Parent is null) { return false; }

		DdnElement parent = src._Parent;
		parent._Items!.Remove(src);
		parent._Index!.Remove(src._Name);
		src._Parent = null;

		if(src != this) { _Parent = null; _Detached = false; }
		return true;
	}


	/// <summary>Removes an element.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns TRUE if the element has been removed, FALSE if it did not exist.</returns>
	public bool Remove(string path)
	{
		return this[path].Remove();
	}


	/// <summary>Removes all child elements and sets the value to NULL.</summary>
	public void Clear()
	{
		DdnElement? src = _Existing;
		if(src is null) { return; }

		src._ClearItems();
		src._Section = false;
		src._Text = _DdnText._NULL;
	}


	/// <summary>Moves the element to another element.</summary>
	/// <param name="target">Target element; an existing element with the same name is replaced.</param>
	/// <exception cref="ArgumentException">Thrown when the target is the element itself or one of its descendants.</exception>
	public void MoveTo(DdnElement target)
	{
		DdnElement src = _Source;
		DdnElement tar = target._Target;

		for(DdnElement? i = tar; i is not null; i = i._Parent)
		{
			if(i == src) { throw new ArgumentException("An element cannot be moved into itself.", nameof(target)); }
		}

		src.Remove();
		tar._Add(src);
	}


	/// <summary>Copies the element to another element.</summary>
	/// <param name="target">Target element; an existing element with the same name is replaced.</param>
	/// <returns>Returns the copy.</returns>
	public DdnElement CopyTo(DdnElement target)
	{
		DdnElement rval = Clone();
		target._Target._Add(rval);

		return rval;
	}


	/// <summary>Merges the contents of another element into this element.</summary>
	/// <param name="source">Source element.</param>
	/// <remarks>Values of the source replace values of this element, sections are merged recursively. Elements that only exist in
	///          this element are kept.</remarks>
	public void Merge(DdnElement source)
	{
		DdnElement src = source._Source.Clone();
		DdnElement tar = _Target;

		if(!src.IsSection)
		{
			tar.Set(src);
			return;
		}

		if(!tar.IsSection) { tar._Text = _DdnText._NULL; }
		tar._Section = true;

		foreach(DdnElement i in src.Elements)
		{
			if(i.IsSection && (tar._Index is not null) && tar._Index.TryGetValue(i._Name, out DdnElement? existing))
			{
				existing.Merge(i);
			}
			else { tar._Add(i); }
		}
	}


	/// <summary>Creates a deep copy of the element.</summary>
	/// <returns>Returns the copy; the copy is a root element.</returns>
	public DdnElement Clone()
	{
		DdnElement src = _Source;
		DdnElement rval = new(null, src._Name, false) { _Text = src._Text, _Section = src._Section };

		if(src._Items is not null)
		{
			foreach(DdnElement i in src._Items) { rval._Add(i.Clone()); }
		}
		rval._Section = src.IsSection;

		return rval;
	}


	/// <summary>Returns the ddn text of the element.</summary>
	/// <param name="encoder">Encoder that encodes the text.</param>
	/// <returns>Returns the encoded text.</returns>
	public string ToString(IEncoder encoder)
	{
		return ToString(new Ddn.Options(encoder));
	}


	/// <summary>Returns the ddn text of the element.</summary>
	/// <param name="options">Options.</param>
	/// <returns>Returns the text.</returns>
	/// <remarks>For sections the text contains the child elements, for values the value statement (<c>name = value;</c>).</remarks>
	public string ToString(Ddn.Options options)
	{
		StringWriter rval = new();
		_Source._Write(rval, (IsSection ? -1 : 0), options);

		string text = rval.ToString();
		if(!options.MultiLine) { text = text.TrimEnd(); }

		return (((options.Encoder is null) || (options.Encoder is PlainText)) ? text : options.Encoder.EncodeString(text));
	}


	/// <summary>Saves the element to a file.</summary>
	/// <param name="fileName">File name; missing directories are created.</param>
	/// <param name="options">Options.</param>
	/// <remarks>The file is written as UTF-8 with byte order mark. Unless <see cref="Ddn.Options.Append"/> is set, the
	///          element is written to a temporary file that replaces the target file when complete, so an interrupted
	///          write does not leave a truncated file.</remarks>
	public void Save(string fileName, Ddn.Options? options = null)
	{
		options ??= Ddn.Options.DEFAULT;

		string path = System.IO.Path.GetFullPath(fileName);
		string dir = System.IO.Path.GetDirectoryName(path)!;
		Directory.CreateDirectory(dir);

		if(options.Append)
		{
			using(StreamWriter wr = new(path, true, Encoding.UTF8))
			{
				wr.Write(ToString(options));
			}
			return;
		}

		string temp = System.IO.Path.Combine(dir, $"~{Guid.NewGuid():N}.tmp");
		try
		{
			File.WriteAllText(temp, ToString(options), Encoding.UTF8);
			File.Move(temp, path, true);
		}
		finally
		{
			if(File.Exists(temp)) { File.Delete(temp); }
		}
	}


	/// <summary>Saves the element to a stream.</summary>
	/// <param name="stream">Stream; the stream is not closed.</param>
	/// <param name="options">Options.</param>
	/// <remarks>The text is written as UTF-8 without byte order mark, because the stream may already contain data.</remarks>
	public void Save(Stream stream, Ddn.Options? options = null)
	{
		options ??= Ddn.Options.DEFAULT;

		using(StreamWriter wr = new(stream, new UTF8Encoding(false), 4096, true))
		{
			wr.Write(ToString(options));
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns the ddn text of the element.</summary>
	/// <returns>Returns the text.</returns>
	public override string ToString()
	{
		return ToString(Ddn.Options.DEFAULT);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEnumerable<DdnElement>                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	IEnumerator<DdnElement> IEnumerable<DdnElement>.GetEnumerator()
	{
		return Elements.GetEnumerator();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEnumerable                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return Elements.GetEnumerator();
	}
}
