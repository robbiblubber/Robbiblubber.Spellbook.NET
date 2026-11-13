namespace Robbiblubber.Spellbook;



public static partial class Ddn
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] SerializableAttribute                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This attribute defines which members of a class or struct are serialized.</summary>
	/// <remarks>Without this attribute public properties with a setter (of any accessibility) and public get-only auto-properties
	///          are serialized. Members marked with <see cref="SerializeAttribute"/> are always serialized, members marked with
	///          <see cref="IgnoreAttribute"/> never.</remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public sealed class SerializableAttribute: Attribute
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// internal constants                                                                                           //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Default settings.</summary>
		internal static readonly SerializableAttribute _DEFAULT = new();



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets if properties are serialized. The default is TRUE.</summary>
		public bool IncludeProperties
		{
			get; set;
		} = true;


		/// <summary>Gets or sets if fields are serialized. The default is FALSE.</summary>
		public bool IncludeFields
		{
			get; set;
		} = false;


		/// <summary>Gets or sets if non-public properties and fields are serialized. The default is FALSE.</summary>
		public bool IncludeNonPublic
		{
			get; set;
		} = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] IgnoreAttribute                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This attribute excludes a field or property from serialization.</summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class IgnoreAttribute: Attribute
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] SerializeAttribute                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This attribute includes a field, property or method in serialization.</summary>
	/// <remarks>A method without parameters that returns a value provides the value for serialization, a method with one parameter
	///          receives the value on deserialization. Two methods with the same name form one member.</remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public sealed class SerializeAttribute: Attribute
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="direction">Serialization direction.</param>
		public SerializeAttribute(SerializationDirection direction = SerializationDirection.BIDIRECTIONAL): this(null, direction)
		{}


		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Element name; NULL for the member name.</param>
		/// <param name="direction">Serialization direction.</param>
		public SerializeAttribute(string? name, SerializationDirection direction = SerializationDirection.BIDIRECTIONAL)
		{
			Name = name;
			Direction = direction;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets the element name; NULL for the member name.</summary>
		public string? Name
		{
			get; set;
		}


		/// <summary>Gets or sets the serialization direction.</summary>
		public SerializationDirection Direction
		{
			get; set;
		} = SerializationDirection.BIDIRECTIONAL;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] CustomSerializerAttribute                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This attribute marks a custom serializer class for discovery with <see cref="Ddn.Discover(System.Reflection.Assembly)"/>.</summary>
	/// <remarks>The class must implement <see cref="ISerializer"/> and have a public parameterless constructor.</remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class CustomSerializerAttribute: Attribute
	{}
}
