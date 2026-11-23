namespace Robbiblubber.Spellbook;



/// <summary>This class provides event arguments for the <see cref="SingleInstance.ArgumentsReceived"/> event.</summary>
public sealed class SingleInstanceEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="arguments">Arguments.</param>
	/// <param name="workingDirectory">Working directory of the instance that has passed the arguments.</param>
	public SingleInstanceEventArgs(string[] arguments, string workingDirectory): base()
	{
		Arguments = arguments;
		WorkingDirectory = workingDirectory;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Gets the arguments.</summary>
	public string[] Arguments
	{
		get;
	}


	/// <summary>Gets the working directory of the instance that has passed the arguments.</summary>
	public string WorkingDirectory
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Returns the full path of a path relative to the working directory of the instance that has passed the
	///          arguments.</summary>
	/// <param name="path">Path.</param>
	/// <returns>Returns the full path.</returns>
	public string GetFullPath(string path)
	{
		if(string.IsNullOrEmpty(WorkingDirectory) || (!Path.IsPathFullyQualified(WorkingDirectory))) { return Path.GetFullPath(path); }
		return Path.GetFullPath(path, WorkingDirectory);
	}
}
