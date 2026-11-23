namespace Robbiblubber.Spellbook;



/// <summary>This class describes a plug-in that could not be loaded or instantiated.</summary>
public sealed class PluginError
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="path">File or directory.</param>
	/// <param name="type">Type; NULL if the error is not related to a type.</param>
	/// <param name="exception">Exception.</param>
	internal PluginError(string path, Type? type, Exception exception)
	{
		Path = path;
		Type = type;
		Exception = exception;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the file or directory.</summary>
	public string Path
	{
		get;
	}


	/// <summary>Gets the type that could not be instantiated; NULL if the error is not related to a type.</summary>
	public Type? Type
	{
		get;
	}


	/// <summary>Gets the exception.</summary>
	public Exception Exception
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the path, the type if any, and the error message.</returns>
	public override string ToString()
	{
		return Path + ((Type is null) ? string.Empty : (" (" + Type.FullName + ")")) + ": " + Exception.Message;
	}
}
