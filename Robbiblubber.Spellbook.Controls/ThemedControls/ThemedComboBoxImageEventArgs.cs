namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class provides arguments for the <see cref="ThemedComboBox.ResolveImage"/> event.</summary>
[SupportedOSPlatform("windows")]
public sealed class ThemedComboBoxImageEventArgs: EventArgs
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Text of the combo box.</param>
	/// <param name="item">Item whose text equals the text, NULL if there is none.</param>
	public ThemedComboBoxImageEventArgs(string text, IListItem? item): base()
	{
		Text = text;
		Item = item;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text of the combo box.</summary>
	public string Text
	{
		get;
	}


	/// <summary>Gets the item whose text equals the text, NULL if the text matches no item.</summary>
	/// <remarks>The event is raised for matching items without image as well.</remarks>
	public IListItem? Item
	{
		get;
	}


	/// <summary>Gets or sets the image for the text.</summary>
	/// <remarks>The combo box does not dispose the image. If no image is set, the default image of the combo box is displayed.</remarks>
	public Image? Image
	{
		get; set;
	}
}
