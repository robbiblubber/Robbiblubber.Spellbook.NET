namespace Robbiblubber.Spellbook;

using System.Reflection;



/// <summary>This class loads plug-ins from a directory.</summary>
/// <remarks><para>Each plug-in directory gets its own load context, so plug-ins find their dependencies in their own directory
///          and different plug-ins can use different versions of the same library. Assemblies the application provides itself
///          (e.g. the assembly that defines the plug-in interface) are shared, so plug-in types are compatible with the
///          application's types.</para>
///          <para>A directory that contains a <c>.deps.json</c> file (the normal build output of a plug-in project) is one
///          plug-in: its main assembly is loaded, and the directory is not searched further. In other directories every
///          assembly is loaded. Native libraries, satellite assemblies (<c>*.resources.dll</c>), <c>runtimes</c> directories
///          and copies of application assemblies are skipped.</para>
///          <para>Plug-ins are loaded on first access. Errors do not stop loading; they are collected in
///          <see cref="Errors"/>.</para></remarks>
/// <example><code>
/// PluginLoader plugins = new(Path.Combine(PathOp.ApplicationDirectory, "plugins"));
/// List&lt;IExporter&gt; exporters = plugins.CreateInstances&lt;IExporter&gt;();
/// foreach(PluginError i in plugins.Errors) { Console.Error.WriteLine(i); }
/// </code></example>
public sealed class PluginLoader
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Lock object.</summary>
	private readonly object _Lock = new();

	/// <summary>Loaded assemblies; NULL before loading.</summary>
	private List<Assembly>? _Assemblies = null;

	/// <summary>Errors.</summary>
	private readonly List<PluginError> _Errors = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="directory">Plug-in directory.</param>
	/// <param name="recursive">Determines if subdirectories are searched.</param>
	/// <exception cref="ArgumentException">Thrown when the directory is empty.</exception>
	public PluginLoader(string directory, bool recursive = true)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(directory);

		Directory = Path.GetFullPath(directory);
		Recursive = recursive;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the plug-in directory.</summary>
	public string Directory
	{
		get;
	}


	/// <summary>Gets if subdirectories are searched.</summary>
	public bool Recursive
	{
		get;
	}


	/// <summary>Gets the plug-in assemblies; an empty list if the directory does not exist.</summary>
	public IReadOnlyList<Assembly> Assemblies
	{
		get
		{
			lock(_Lock) { return _Load().ToArray(); }
		}
	}


	/// <summary>Gets the errors that occurred while loading assemblies and creating instances.</summary>
	public IReadOnlyList<PluginError> Errors
	{
		get
		{
			lock(_Lock)
			{
				_Load();
				return _Errors.ToArray();
			}
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Loads the plug-ins if they have not been loaded yet.</summary>
	/// <returns>Returns the assemblies.</returns>
	private List<Assembly> _Load()
	{
		if(_Assemblies is null)
		{
			_Assemblies = new();
			if(System.IO.Directory.Exists(Directory)) { _Scan(Directory); }
		}

		return _Assemblies;
	}


	/// <summary>Loads the plug-ins of a directory and, if recursive, of its subdirectories.</summary>
	/// <param name="directory">Directory.</param>
	private void _Scan(string directory)
	{
		string[] deps, entries, subdirectories;

		try
		{
			deps = System.IO.Directory.GetFiles(directory, "*.deps.json");
			if(deps.Length > 0)
			{
				entries = deps.Select(m => Path.Combine(directory, Path.GetFileName(m)[..^".deps.json".Length] + ".dll")).Where(File.Exists).ToArray();
			}
			else { entries = System.IO.Directory.GetFiles(directory, "*.dll").Where(m => !m.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase)).ToArray(); }

			subdirectories = ((Recursive && (deps.Length == 0)) ? System.IO.Directory.GetDirectories(directory) : []);
		}
		catch(Exception ex)
		{
			_Errors.Add(new PluginError(directory, null, ex));
			return;
		}

		Array.Sort(entries, StringComparer.Ordinal);
		Array.Sort(subdirectories, StringComparer.Ordinal);

		_PluginContext? context = null;
		foreach(string i in entries)
		{
			AssemblyName name;
			try
			{
				name = AssemblyName.GetAssemblyName(i);
			}
			catch(BadImageFormatException) { continue; }
			catch(Exception ex)
			{
				_Errors.Add(new PluginError(i, null, ex));
				continue;
			}

			if(_PluginContext._IsShared(name)) { continue; }

			try
			{
				context ??= new _PluginContext(directory, i);

				Assembly? asm = context.Assemblies.FirstOrDefault(m => AssemblyName.ReferenceMatchesDefinition(m.GetName(), name));
				_Assemblies!.Add(asm ?? context.LoadFromAssemblyPath(i));
			}
			catch(Exception ex) { _Errors.Add(new PluginError(i, null, ex)); }
		}

		foreach(string i in subdirectories)
		{
			if(!string.Equals(Path.GetFileName(i), "runtimes", StringComparison.OrdinalIgnoreCase)) { _Scan(i); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the public, concrete plug-in classes that implement or derive from a type.</summary>
	/// <typeparam name="T">Base type or interface.</typeparam>
	/// <returns>Returns the types.</returns>
	public Type[] GetTypes<T>()
	{
		return GetTypes(typeof(T));
	}


	/// <summary>Gets the public, concrete plug-in classes that implement or derive from a type.</summary>
	/// <param name="baseType">Base type or interface.</param>
	/// <returns>Returns the types. Abstract classes, interfaces and open generic types are not included; types that cannot
	///          be loaded are skipped.</returns>
	public Type[] GetTypes(Type baseType)
	{
		ArgumentNullException.ThrowIfNull(baseType);

		return ClassOp._GetImplementations(baseType, Assemblies);
	}


	/// <summary>Creates an instance of every plug-in class that implements or derives from a type.</summary>
	/// <typeparam name="T">Base type or interface.</typeparam>
	/// <param name="args">Constructor arguments.</param>
	/// <returns>Returns the instances. Classes that cannot be instantiated are skipped and reported in
	///          <see cref="Errors"/>.</returns>
	public List<T> CreateInstances<T>(params object?[] args)
	{
		return CreateInstances(typeof(T), args).Cast<T>().ToList();
	}


	/// <summary>Creates an instance of every plug-in class that implements or derives from a type.</summary>
	/// <param name="baseType">Base type or interface.</param>
	/// <param name="args">Constructor arguments.</param>
	/// <returns>Returns the instances. Classes that cannot be instantiated are skipped and reported in
	///          <see cref="Errors"/>.</returns>
	public List<object> CreateInstances(Type baseType, params object?[] args)
	{
		List<object> rval = new();

		foreach(Type i in GetTypes(baseType))
		{
			try
			{
				object? obj = Activator.CreateInstance(i, args);
				if(obj is not null) { rval.Add(obj); }
			}
			catch(Exception ex)
			{
				lock(_Lock) { _Errors.Add(new PluginError(i.Assembly.Location, i, (ex as TargetInvocationException)?.InnerException ?? ex)); }
			}
		}

		return rval;
	}
}
