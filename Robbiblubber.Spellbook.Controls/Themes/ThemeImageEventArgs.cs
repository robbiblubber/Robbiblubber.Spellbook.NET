namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class provides the arguments of the <see cref="ThemeManager.ResolveImage"/> event.</summary>
[SupportedOSPlatform("windows")]
public class ThemeImageEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="source">Object that shows the image, for example a <see cref="ToolStripItem"/>.</param>
	/// <param name="original">Original image.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="image">Image to show.</param>
	public ThemeImageEventArgs(object? source, Image original, Theme theme, Image image)
	{
		Source = source;
		Original = original;
		Theme = theme;
		Image = image;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the object that shows the image, for example a <see cref="ToolStripItem"/>.</summary>
	public object? Source
	{
		get;
	}


	/// <summary>Gets the original image.</summary>
	public Image Original
	{
		get;
	}


	/// <summary>Gets the theme.</summary>
	public Theme Theme
	{
		get;
	}


	/// <summary>Gets or sets the image to show.</summary>
	/// <remarks>Initially the original image or, in dark themes, a lightened copy of a single-colored dark image or of an image with
	///          dark outlines. NULL shows the original image. The event handler keeps ownership of an image it sets.</remarks>
	public Image? Image
	{
		get; set;
	}
}
