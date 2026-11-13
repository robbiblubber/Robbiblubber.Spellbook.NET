namespace Robbiblubber.Spellbook;

using System.Reflection;
using System.Text;



/// <summary>This class provides ddn support.</summary>
public static partial class Ddn
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Parses ddn text into a new element.</summary>
	/// <param name="text">Text.</param>
	/// <param name="options">Options.</param>
	/// <returns>Returns the root element.</returns>
	private static DdnElement _Parse(string text, Options options)
	{
		if((options.Encoder is not null) && (!(options.Encoder is PlainText))) { text = options.Encoder.DecodeToString(text.Trim()); }

		return new DdnElement(text);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Opens a ddn file.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="ignoreExceptions">Determines if errors are ignored. If TRUE, an empty element is returned when the file does
	///                                not exist or cannot be read.</param>
	/// <returns>Returns the root element.</returns>
	public static DdnElement Open(string fileName, bool ignoreExceptions)
	{
		return Open(fileName, null, ignoreExceptions);
	}


	/// <summary>Opens a ddn file.</summary>
	/// <param name="fileName">File name.</param>
	/// <param name="options">Options; only <see cref="Options.Encoder"/> is used when reading.</param>
	/// <param name="ignoreExceptions">Determines if errors are ignored. If TRUE, an empty element is returned when the file does
	///                                not exist or cannot be read.</param>
	/// <returns>Returns the root element.</returns>
	public static DdnElement Open(string fileName, Options? options = null, bool ignoreExceptions = false)
	{
		try
		{
			return _Parse(File.ReadAllText(fileName), options ?? Options.DEFAULT);
		}
		catch(Exception)
		{
			if(!ignoreExceptions) { throw; }
			return new DdnElement();
		}
	}


	/// <summary>Opens ddn data from a stream.</summary>
	/// <param name="stream">Stream; the stream is read to its end and not closed.</param>
	/// <param name="ignoreExceptions">Determines if errors are ignored. If TRUE, an empty element is returned when the stream
	///                                cannot be read.</param>
	/// <returns>Returns the root element.</returns>
	public static DdnElement Open(Stream stream, bool ignoreExceptions)
	{
		return Open(stream, null, ignoreExceptions);
	}


	/// <summary>Opens ddn data from a stream.</summary>
	/// <param name="stream">Stream; the stream is read to its end and not closed.</param>
	/// <param name="options">Options; only <see cref="Options.Encoder"/> is used when reading.</param>
	/// <param name="ignoreExceptions">Determines if errors are ignored. If TRUE, an empty element is returned when the stream
	///                                cannot be read.</param>
	/// <returns>Returns the root element.</returns>
	public static DdnElement Open(Stream stream, Options? options = null, bool ignoreExceptions = false)
	{
		try
		{
			using(StreamReader re = new(stream, Encoding.UTF8, true, 4096, true))
			{
				return _Parse(re.ReadToEnd(), options ?? Options.DEFAULT);
			}
		}
		catch(Exception)
		{
			if(!ignoreExceptions) { throw; }
			return new DdnElement();
		}
	}


	/// <summary>Creates an empty ddn root element.</summary>
	/// <returns>Returns the element.</returns>
	public static DdnElement Create()
	{
		return new DdnElement();
	}


	/// <summary>Creates a ddn root element from text.</summary>
	/// <param name="text">ddn text.</param>
	/// <returns>Returns the element.</returns>
	public static DdnElement Create(string text)
	{
		return new DdnElement(text);
	}


	/// <summary>Serializes an object.</summary>
	/// <param name="obj">Object.</param>
	/// <param name="options">Serialization options.</param>
	/// <returns>Returns the root element.</returns>
	/// <exception cref="SerializeException">Thrown when the object cannot be serialized.</exception>
	public static DdnElement Serialize(object? obj, SerializationOptions? options = null)
	{
		DdnElement rval = new();
		Serialize(obj, rval, options);

		return rval;
	}


	/// <summary>Serializes an object into an element.</summary>
	/// <param name="obj">Object.</param>
	/// <param name="elm">Element; existing contents are replaced.</param>
	/// <param name="options">Serialization options.</param>
	/// <remarks>Scalar values and arrays of scalars cannot be stored in a root element; they are written to its child element
	///          <c>value</c>.</remarks>
	/// <exception cref="SerializeException">Thrown when the object cannot be serialized.</exception>
	public static void Serialize(object? obj, DdnElement elm, SerializationOptions? options = null)
	{
		SerializationContext context = new(options);

		if(elm.IsRoot && ((obj is null) || ((!(obj is DdnElement)) && _DdnSerializer._IsValueInstance(obj))))
		{
			elm.Clear();
			_DdnSerializer._Serialize(context, obj, (obj?.GetType() ?? typeof(object)), elm.Element(_DdnSerializer._VALUE));
		}
		else { _DdnSerializer._Serialize(context, obj, (obj?.GetType() ?? typeof(object)), elm); }
	}


	/// <summary>Deserializes an object from an element.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="elm">Element.</param>
	/// <param name="options">Serialization options.</param>
	/// <returns>Returns the object, or the default value of the type if the element does not exist.</returns>
	/// <exception cref="SerializeException">Thrown when the element cannot be deserialized.</exception>
	public static T? Deserialize<T>(DdnElement elm, SerializationOptions? options = null)
	{
		T? rval = default;
		return Deserialize(ref rval, elm, options);
	}


	/// <summary>Deserializes an element into an existing object.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="obj">Object; existing objects and collections are filled, members without element keep their values.
	///                   Receives the result.</param>
	/// <param name="elm">Element.</param>
	/// <param name="options">Serialization options.</param>
	/// <returns>Returns the object.</returns>
	/// <exception cref="SerializeException">Thrown when the element cannot be deserialized.</exception>
	public static T? Deserialize<T>(ref T? obj, DdnElement elm, SerializationOptions? options = null)
	{
		return (obj = (T?) _DdnSerializer._Deserialize(new SerializationContext(options), typeof(T), elm, obj));
	}


	/// <summary>Deserializes an object from an element.</summary>
	/// <param name="t">Type.</param>
	/// <param name="elm">Element.</param>
	/// <param name="options">Serialization options.</param>
	/// <returns>Returns the object, or NULL if the element does not exist.</returns>
	/// <exception cref="SerializeException">Thrown when the element cannot be deserialized.</exception>
	public static object? Deserialize(Type t, DdnElement elm, SerializationOptions? options = null)
	{
		return _DdnSerializer._Deserialize(new SerializationContext(options), t, elm, null);
	}


	/// <summary>Registers a custom serializer.</summary>
	/// <param name="serializer">Serializer; replaces a serializer registered for the same type.</param>
	public static void RegisterSerializer(ISerializer serializer)
	{
		_DdnSerializer._Serializers[serializer.Type] = serializer;
	}


	/// <summary>Registers a custom serializer.</summary>
	/// <typeparam name="T">Type.</typeparam>
	/// <param name="serialize">Serialization method.</param>
	/// <param name="deserialize">Deserialization method; receives the element, the existing object or NULL, and the context.</param>
	public static void RegisterSerializer<T>(Action<T, DdnElement, SerializationContext> serialize, Func<DdnElement, T?, SerializationContext, T?> deserialize)
	{
		RegisterSerializer(new _DelegateSerializer<T>(serialize, deserialize));
	}


	/// <summary>Removes the custom serializer for a type.</summary>
	/// <param name="t">Type.</param>
	/// <returns>Returns TRUE if a serializer has been removed, otherwise returns FALSE.</returns>
	public static bool UnregisterSerializer(Type t)
	{
		return _DdnSerializer._Serializers.TryRemove(t, out _);
	}


	/// <summary>Registers all custom serializers of an assembly.</summary>
	/// <param name="assembly">Assembly.</param>
	/// <returns>Returns the number of registered serializers.</returns>
	/// <remarks>Classes marked with <see cref="CustomSerializerAttribute"/> that implement <see cref="ISerializer"/> and have a
	///          public parameterless constructor are registered. Types that cannot be loaded are skipped.</remarks>
	public static int Discover(Assembly assembly)
	{
		Type?[] types;

		try
		{
			types = assembly.GetTypes();
		}
		catch(ReflectionTypeLoadException ex) { types = ex.Types; }

		return Discover(types);
	}


	/// <summary>Registers custom serializers.</summary>
	/// <param name="types">Types; types that are not marked with <see cref="CustomSerializerAttribute"/> are ignored.</param>
	/// <returns>Returns the number of registered serializers.</returns>
	/// <remarks>Abstract classes, classes without a public parameterless constructor and serializers whose constructor
	///          throws an exception are skipped.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the types are NULL.</exception>
	public static int Discover(IEnumerable<Type?> types)
	{
		int rval = 0;

		ArgumentNullException.ThrowIfNull(types);

		foreach(Type? i in types.Distinct())
		{
			ISerializer serializer;

			if((i is null) || i.IsAbstract || i.IsInterface || i.ContainsGenericParameters) { continue; }
			if(i.GetCustomAttribute<CustomSerializerAttribute>(false) is null) { continue; }
			if((!typeof(ISerializer).IsAssignableFrom(i)) || (i.GetConstructor(Type.EmptyTypes) is null)) { continue; }

			try
			{
				serializer = (ISerializer) Activator.CreateInstance(i)!;
			}
			catch(Exception ex) when ((ex is TargetInvocationException) || (ex is MemberAccessException) || (ex is TypeLoadException)) { continue; }

			RegisterSerializer(serializer);
			rval++;
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Options                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class provides options for reading and writing ddn text.</summary>
	public sealed class Options
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public constants                                                                                             //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Default options.</summary>
		public static readonly Options DEFAULT = new();

		/// <summary>Options for single-line text.</summary>
		public static readonly Options SINGLE_LINE = new() { MultiLine = false, SectionSpacing = false };



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		public Options()
		{}


		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="encoder">Encoder that encodes the whole text.</param>
		public Options(IEncoder? encoder)
		{
			Encoder = encoder;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets if the text is written in multiple lines. The default is TRUE.</summary>
		public bool MultiLine
		{
			get; init;
		} = true;


		/// <summary>Gets if an empty line is written before sections. The default is TRUE.</summary>
		public bool SectionSpacing
		{
			get; init;
		} = true;


		/// <summary>Gets if NULL values are omitted. The default is FALSE.</summary>
		public bool IgnoreNullElements
		{
			get; init;
		} = false;


		/// <summary>Gets if elements are written in alphabetical order. The default is TRUE.</summary>
		/// <remarks>Values are always written before sections. Numbers in names are compared numerically, so <c>[2]</c> precedes
		///          <c>[10]</c>. If FALSE, elements are written in the order they have been added.</remarks>
		public bool Ordered
		{
			get; init;
		} = true;


		/// <summary>Gets if text is appended to an existing file. The default is FALSE.</summary>
		public bool Append
		{
			get; init;
		} = false;


		/// <summary>Gets the encoder that encodes the whole text, or NULL for plain text.</summary>
		public IEncoder? Encoder
		{
			get; init;
		} = null;


		/// <summary>Gets the indentation for one level. The default is four spaces.</summary>
		public string Indent
		{
			get; init;
		} = "    ";


		/// <summary>Gets the line break. The default is CR LF.</summary>
		public string NewLine
		{
			get; init;
		} = "\r\n";
	}
}
