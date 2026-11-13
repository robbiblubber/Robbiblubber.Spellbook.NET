namespace Robbiblubber.Spellbook.Windows;

using System.Runtime.Versioning;



/// <summary>This class describes a shortcut (.lnk file).</summary>
[SupportedOSPlatform("windows")]
public sealed class ShortcutInfo
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="targetPath">Target path.</param>
	/// <exception cref="ArgumentException">Thrown when the target path is empty.</exception>
	public ShortcutInfo(string targetPath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(targetPath);
		TargetPath = targetPath;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the target path.</summary>
	public string TargetPath
	{
		get;
	}


	/// <summary>Gets the command line arguments.</summary>
	public string? Arguments
	{
		get; init;
	}


	/// <summary>Gets the working directory, or NULL to use the directory of the target when creating a shortcut.</summary>
	public string? WorkingDirectory
	{
		get; init;
	}


	/// <summary>Gets the description (shown as tooltip).</summary>
	public string? Description
	{
		get; init;
	}


	/// <summary>Gets the icon path, or NULL to use the icon of the target.</summary>
	public string? IconPath
	{
		get; init;
	}


	/// <summary>Gets the icon index in the icon file.</summary>
	public int IconIndex
	{
		get; init;
	}


	/// <summary>Gets the window style.</summary>
	public ShortcutWindowStyle WindowStyle
	{
		get; init;
	} = ShortcutWindowStyle.NORMAL;


	/// <summary>Gets if the target is started with administrator rights ("Run as administrator").</summary>
	public bool RunAsAdministrator
	{
		get; init;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string ToString()
	{
		return (string.IsNullOrEmpty(Arguments) ? TargetPath : $"{TargetPath} {Arguments}");
	}
}
