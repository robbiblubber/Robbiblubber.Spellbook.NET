namespace Robbiblubber.Spellbook;

using System.Reflection;
using System.Runtime.Loader;



/// <summary>This class implements the load context for the plug-ins of one directory.</summary>
/// <remarks>Assemblies the application provides itself (e.g. the assembly that defines the plug-in interface) are shared with
///          the default context, so that plug-in types are compatible with the application's types. All other dependencies
///          are loaded from the plug-in directory.</remarks>
internal sealed class _PluginContext: AssemblyLoadContext
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Names of the assemblies the application provides.</summary>
	private static readonly HashSet<string> _APPLICATION_ASSEMBLIES = new(((AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string) ?? string.Empty)
																		  .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
																		  .Select(Path.GetFileNameWithoutExtension).OfType<string>(), StringComparer.OrdinalIgnoreCase);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Plug-in directory.</summary>
	private readonly string _Directory;

	/// <summary>Dependency resolver; NULL if none could be created.</summary>
	private readonly AssemblyDependencyResolver? _Resolver = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="directory">Plug-in directory.</param>
	/// <param name="mainAssembly">Main assembly of the directory; its .deps.json file is used to resolve dependencies.</param>
	public _PluginContext(string directory, string mainAssembly): base("Plugins: " + directory)
	{
		_Directory = directory;

		try
		{
			_Resolver = new AssemblyDependencyResolver(mainAssembly);
		}
		catch(Exception) {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if an assembly is provided by the application and therefore shared with the plug-ins.</summary>
	/// <param name="name">Assembly name.</param>
	/// <returns>Returns TRUE if the assembly is shared, otherwise returns FALSE.</returns>
	internal static bool _IsShared(AssemblyName name)
	{
		if(string.IsNullOrEmpty(name.Name)) { return false; }
		if(_APPLICATION_ASSEMBLIES.Contains(name.Name)) { return true; }

		return Default.Assemblies.Any(m => string.Equals(m.GetName().Name, name.Name, StringComparison.OrdinalIgnoreCase));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] AssemblyLoadContext                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Resolves an assembly.</summary>
	/// <param name="assemblyName">Assembly name.</param>
	/// <returns>Returns the assembly; NULL to use the default context.</returns>
	protected override Assembly? Load(AssemblyName assemblyName)
	{
		if(_IsShared(assemblyName)) { return null; }

		string? path = _Resolver?.ResolveAssemblyToPath(assemblyName);
		if((path is null) && (!string.IsNullOrEmpty(assemblyName.Name)))
		{
			string probe = Path.Combine(_Directory, assemblyName.Name + ".dll");
			if(File.Exists(probe)) { path = probe; }
		}

		return ((path is null) ? null : LoadFromAssemblyPath(path));
	}


	/// <summary>Resolves a native library.</summary>
	/// <param name="unmanagedDllName">Library name.</param>
	/// <returns>Returns the library handle; <see cref="IntPtr.Zero"/> to use the default resolution.</returns>
	protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
	{
		string? path = _Resolver?.ResolveUnmanagedDllToPath(unmanagedDllName);
		return ((path is null) ? IntPtr.Zero : LoadUnmanagedDllFromPath(path));
	}
}
