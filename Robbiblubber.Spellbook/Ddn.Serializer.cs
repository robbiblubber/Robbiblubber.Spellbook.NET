namespace Robbiblubber.Spellbook;



public static partial class Ddn
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] ISerializer                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom serializers implement this interface.</summary>
	public interface ISerializer
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the type this serializer handles.</summary>
		/// <remarks>The serializer is also used for derived types that have no serializer of their own.</remarks>
		public Type Type
		{
			get;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Serializes an object.</summary>
		/// <param name="value">Object; never NULL.</param>
		/// <param name="elm">Element that receives the object; the element is empty.</param>
		/// <param name="context">Serialization context.</param>
		public void Serialize(object value, DdnElement elm, SerializationContext context);


		/// <summary>Deserializes an object.</summary>
		/// <param name="elm">Element; the element exists and is not NULL.</param>
		/// <param name="existing">Existing object that may be filled, or NULL.</param>
		/// <param name="context">Serialization context.</param>
		/// <returns>Returns the object.</returns>
		public object? Deserialize(DdnElement elm, object? existing, SerializationContext context);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] Serializer<T>                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class is a base class for custom serializers.</summary>
	/// <typeparam name="T">Type.</typeparam>
	public abstract class Serializer<T>: ISerializer
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Serializes an object.</summary>
		/// <param name="value">Object; never NULL.</param>
		/// <param name="elm">Element that receives the object; the element is empty.</param>
		/// <param name="context">Serialization context.</param>
		public abstract void Serialize(T value, DdnElement elm, SerializationContext context);


		/// <summary>Deserializes an object.</summary>
		/// <param name="elm">Element; the element exists and is not NULL.</param>
		/// <param name="existing">Existing object that may be filled, or NULL.</param>
		/// <param name="context">Serialization context.</param>
		/// <returns>Returns the object.</returns>
		public abstract T? Deserialize(DdnElement elm, T? existing, SerializationContext context);



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] ISerializer                                                                                      //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		Type ISerializer.Type
		{
			get { return typeof(T); }
		}


		/// <inheritdoc/>
		void ISerializer.Serialize(object value, DdnElement elm, SerializationContext context)
		{
			Serialize((T) value, elm, context);
		}


		/// <inheritdoc/>
		object? ISerializer.Deserialize(DdnElement elm, object? existing, SerializationContext context)
		{
			return Deserialize(elm, ((existing is T e) ? e : default), context);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _DelegateSerializer<T>                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements a custom serializer with delegates.</summary>
	/// <typeparam name="T">Type.</typeparam>
	private sealed class _DelegateSerializer<T>: Serializer<T>
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Serialization method.</summary>
		private readonly Action<T, DdnElement, SerializationContext> _Serialize;

		/// <summary>Deserialization method.</summary>
		private readonly Func<DdnElement, T?, SerializationContext, T?> _Deserialize;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="serialize">Serialization method.</param>
		/// <param name="deserialize">Deserialization method.</param>
		public _DelegateSerializer(Action<T, DdnElement, SerializationContext> serialize, Func<DdnElement, T?, SerializationContext, T?> deserialize)
		{
			_Serialize = serialize;
			_Deserialize = deserialize;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] Serializer<T>                                                                                     //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public override void Serialize(T value, DdnElement elm, SerializationContext context)
		{
			_Serialize(value, elm, context);
		}


		/// <inheritdoc/>
		public override T? Deserialize(DdnElement elm, T? existing, SerializationContext context)
		{
			return _Deserialize(elm, existing, context);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] SerializationOptions                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class provides options for serialization.</summary>
	public sealed class SerializationOptions
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public constants                                                                                             //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Default options.</summary>
		public static readonly SerializationOptions DEFAULT = new();



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets when type names are written and if they are read. The default is <see cref="TypeNameHandling.NONE"/>.</summary>
		public TypeNameHandling TypeNames
		{
			get; init;
		} = TypeNameHandling.NONE;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] SerializationContext                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class provides the state of a serialization or deserialization.</summary>
	/// <remarks>Custom serializers use the context to serialize nested values and to apply the default member serialization.</remarks>
	public sealed class SerializationContext
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="options">Options.</param>
		internal SerializationContext(SerializationOptions? options)
		{
			Options = (options ?? SerializationOptions.DEFAULT);
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// internal properties                                                                                          //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the objects that are being serialized.</summary>
		internal HashSet<object> _Stack
		{
			get;
		} = new(ReferenceEqualityComparer.Instance);



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the options.</summary>
		public SerializationOptions Options
		{
			get;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Serializes a value.</summary>
		/// <param name="value">Value.</param>
		/// <param name="elm">Element.</param>
		public void Serialize(object? value, DdnElement elm)
		{
			_DdnSerializer._Serialize(this, value, (value?.GetType() ?? typeof(object)), elm);
		}


		/// <summary>Serializes a value.</summary>
		/// <param name="value">Value.</param>
		/// <param name="declaredType">Declared type; type names are written when the value has a different type.</param>
		/// <param name="elm">Element.</param>
		public void Serialize(object? value, Type declaredType, DdnElement elm)
		{
			_DdnSerializer._Serialize(this, value, declaredType, elm);
		}


		/// <summary>Serializes the members of an object as the default serializer does.</summary>
		/// <param name="value">Object.</param>
		/// <param name="elm">Element.</param>
		public void SerializeMembers(object value, DdnElement elm)
		{
			_DdnSerializer._SerializeMembers(this, value, elm);
		}


		/// <summary>Deserializes a value.</summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="elm">Element.</param>
		/// <param name="existing">Existing value that is returned if the element does not exist or that may be filled.</param>
		/// <returns>Returns the value.</returns>
		public T? Deserialize<T>(DdnElement elm, T? existing = default)
		{
			return (T?) _DdnSerializer._Deserialize(this, typeof(T), elm, existing);
		}


		/// <summary>Deserializes a value.</summary>
		/// <param name="t">Type.</param>
		/// <param name="elm">Element.</param>
		/// <param name="existing">Existing value that is returned if the element does not exist or that may be filled.</param>
		/// <returns>Returns the value.</returns>
		public object? Deserialize(Type t, DdnElement elm, object? existing = null)
		{
			return _DdnSerializer._Deserialize(this, t, elm, existing);
		}


		/// <summary>Deserializes the members of an object as the default serializer does.</summary>
		/// <param name="value">Object.</param>
		/// <param name="elm">Element.</param>
		/// <returns>Returns the object; for structs a modified copy.</returns>
		public object DeserializeMembers(object value, DdnElement elm)
		{
			return _DdnSerializer._DeserializeMembers(this, value, elm);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] SerializeException                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This exception is thrown when an object cannot be serialized or deserialized.</summary>
	public sealed class SerializeException: Exception
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		public SerializeException(): base()
		{}


		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="message">Message.</param>
		/// <param name="innerException">Inner exception.</param>
		public SerializeException(string message, Exception? innerException = null): base(message, innerException)
		{}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] SerializationDirection                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines serialization directions.</summary>
	[Flags]
	public enum SerializationDirection: int
	{
		/// <summary>The member is deserialized only.</summary>
		INBOUND = 1,
		/// <summary>The member is serialized only.</summary>
		OUTBOUND = 2,
		/// <summary>The member is serialized and deserialized.</summary>
		BIDIRECTIONAL = (INBOUND | OUTBOUND)
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [enum] TypeNameHandling                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This enumeration defines when type names are written.</summary>
	/// <remarks>Type names are stored in the element <c>$type</c> of a section. They are read unless the handling is
	///          <see cref="NONE"/>; the type must be assignable to the declared type.</remarks>
	public enum TypeNameHandling: int
	{
		/// <summary>Type names are neither written nor read.</summary>
		NONE = 0,
		/// <summary>Type names are written when the type of an object differs from the declared type.</summary>
		AUTO = 1,
		/// <summary>Type names are written for all objects.</summary>
		ALL = 2
	}
}
