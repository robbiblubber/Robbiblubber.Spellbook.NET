namespace Robbiblubber.Spellbook.UnitTests.Core.Reflection;

using System.Runtime.Loader;
using NUnit.Framework;



/// <summary>This class tests the plug-in loader with the test plug-in (Robbiblubber.Spellbook.TestPlugin), which the build
///          copies to TestPlugins/Sample.</summary>
[TestFixture]
public class PluginLoaderTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Test directory.</summary>
	private string _Dir = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the test plug-in root directory.</summary>
	private static string _Plugins
	{
		get { return Path.Combine(AppContext.BaseDirectory, "TestPlugins"); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// setup                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a test directory.</summary>
	[SetUp]
	public void SetUp()
	{
		_Dir = Path.Combine(Path.GetTempPath(), "spellbook-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_Dir);
	}


	/// <summary>Deletes the test directory.</summary>
	[TearDown]
	public void TearDown()
	{
		try { Directory.Delete(_Dir, true); } catch(Exception) {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// tests                                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Plug-ins are found in subdirectories, with their dependencies, and are compatible with application types.</summary>
	[Test]
	public void Recursive()
	{
		PluginLoader loader = new(_Plugins);

		Assert.That(loader.Assemblies.Select(m => m.GetName().Name), Is.EqualTo(new[] { "Robbiblubber.Spellbook.TestPlugin" }));

		List<IEncoder> encoders = loader.CreateInstances<IEncoder>();
		Assert.That(encoders, Has.Count.EqualTo(1));
		Assert.That(encoders[0].GetType().Name, Is.EqualTo("ReverseEncoder"));
		Assert.That(encoders[0].Encode([1, 2, 3]), Is.EqualTo(new byte[] { 3, 2, 1 }));

		Assert.That(AssemblyLoadContext.GetLoadContext(encoders[0].GetType().Assembly), Is.Not.SameAs(AssemblyLoadContext.Default));
		Assert.That(encoders[0], Is.InstanceOf<EncoderBase>());
	}


	/// <summary>Without recursion only the directory itself is searched.</summary>
	[Test]
	public void NotRecursive()
	{
		Assert.That(new PluginLoader(_Plugins, false).Assemblies, Is.Empty);
		Assert.That(new PluginLoader(Path.Combine(_Plugins, "Sample"), false).Assemblies, Has.Count.EqualTo(1));
	}


	/// <summary>Abstract classes are skipped; failing constructors are reported.</summary>
	[Test]
	public void Types_And_Errors()
	{
		PluginLoader loader = new(_Plugins);

		Assert.That(loader.GetTypes<IEncoder>().Select(m => m.Name).Order(), Is.EqualTo(new[] { "FailingEncoder", "ReverseEncoder" }));
		Assert.That(loader.Errors, Is.Empty);

		loader.CreateInstances<IEncoder>();
		Assert.That(loader.Errors, Has.Count.EqualTo(1));
		Assert.That(loader.Errors[0].Type?.Name, Is.EqualTo("FailingEncoder"));
		Assert.That(loader.Errors[0].Exception, Is.InstanceOf<InvalidOperationException>());
		Assert.That(loader.Errors[0].ToString(), Does.Contain("Plug-in failed."));
	}


	/// <summary>In a directory without .deps.json every assembly is loaded; dependencies are found in the directory. Native
	///          libraries, satellite assemblies and copies of application assemblies are skipped.</summary>
	[Test]
	public void FlatDirectory()
	{
		string sample = Path.Combine(_Plugins, "Sample");
		foreach(string i in new[] { "Robbiblubber.Spellbook.TestPlugin.dll", "Robbiblubber.Spellbook.TestPluginDependency.dll", "Robbiblubber.Spellbook.dll" })
		{
			File.Copy(Path.Combine(sample, i), Path.Combine(_Dir, i));
		}
		File.WriteAllText(Path.Combine(_Dir, "native.dll"), "not an assembly");
		File.WriteAllText(Path.Combine(_Dir, "Plugin.resources.dll"), "not an assembly");
		Directory.CreateDirectory(Path.Combine(_Dir, "runtimes"));
		File.Copy(Path.Combine(sample, "Robbiblubber.Spellbook.TestPlugin.dll"), Path.Combine(_Dir, "runtimes", "Robbiblubber.Spellbook.TestPlugin.dll"));

		PluginLoader loader = new(_Dir);

		Assert.That(loader.Assemblies.Select(m => m.GetName().Name).Order(), Is.EqualTo(new[] { "Robbiblubber.Spellbook.TestPlugin", "Robbiblubber.Spellbook.TestPluginDependency" }));
		Assert.That(loader.Errors, Is.Empty);
		Assert.That(loader.CreateInstances<IEncoder>()[0].Decode([1, 2]), Is.EqualTo(new byte[] { 2, 1 }));
	}


	/// <summary>Two loaders load separate copies of the same plug-in.</summary>
	[Test]
	public void Isolation()
	{
		Type a = new PluginLoader(_Plugins).GetTypes<IEncoder>()[0];
		Type b = new PluginLoader(_Plugins).GetTypes<IEncoder>()[0];

		Assert.That(a.FullName, Is.EqualTo(b.FullName));
		Assert.That(a, Is.Not.SameAs(b));
	}


	/// <summary>A missing directory has no plug-ins and no errors.</summary>
	[Test]
	public void MissingDirectory()
	{
		PluginLoader loader = new(Path.Combine(_Dir, "missing"));

		Assert.That(loader.Assemblies, Is.Empty);
		Assert.That(loader.Errors, Is.Empty);
		Assert.That(loader.CreateInstances<IEncoder>(), Is.Empty);
		Assert.Throws<ArgumentException>(() => new PluginLoader(" "));
	}
}
