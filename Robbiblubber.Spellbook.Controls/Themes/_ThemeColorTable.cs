namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class provides the colors of a theme to the <see cref="ThemeToolStripRenderer"/>.</summary>
/// <remarks>The colors are read from the theme when they are used, so the table follows changes of the theme.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ThemeColorTable: ProfessionalColorTable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="theme">Theme.</param>
	public _ThemeColorTable(Theme theme)
	{
		Theme = theme;
		UseSystemColors = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the theme.</summary>
	public Theme Theme
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ProfessionalColorTable                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override Color ButtonCheckedGradientBegin
	{
		get { return Theme[ThemeColor.INACTIVE_SELECTION_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonCheckedGradientEnd
	{
		get { return Theme[ThemeColor.INACTIVE_SELECTION_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonCheckedGradientMiddle
	{
		get { return Theme[ThemeColor.INACTIVE_SELECTION_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonCheckedHighlight
	{
		get { return Theme[ThemeColor.INACTIVE_SELECTION_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonCheckedHighlightBorder
	{
		get { return Theme[ThemeColor.ACCENT]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedBorder
	{
		get { return Theme[ThemeColor.FIELD_BORDER_HOT]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedGradientBegin
	{
		get { return Theme[ThemeColor.BUTTON_BACK_PRESSED]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedGradientEnd
	{
		get { return Theme[ThemeColor.BUTTON_BACK_PRESSED]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedGradientMiddle
	{
		get { return Theme[ThemeColor.BUTTON_BACK_PRESSED]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedHighlight
	{
		get { return Theme[ThemeColor.BUTTON_BACK_PRESSED]; }
	}


	/// <inheritdoc/>
	public override Color ButtonPressedHighlightBorder
	{
		get { return Theme[ThemeColor.FIELD_BORDER_HOT]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedBorder
	{
		get { return Theme[ThemeColor.FIELD_BORDER]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedGradientBegin
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedGradientEnd
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedGradientMiddle
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedHighlight
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ButtonSelectedHighlightBorder
	{
		get { return Theme[ThemeColor.FIELD_BORDER]; }
	}


	/// <inheritdoc/>
	public override Color CheckBackground
	{
		get { return Theme[ThemeColor.INACTIVE_SELECTION_BACK]; }
	}


	/// <inheritdoc/>
	public override Color CheckPressedBackground
	{
		get { return Theme[ThemeColor.BUTTON_BACK_PRESSED]; }
	}


	/// <inheritdoc/>
	public override Color CheckSelectedBackground
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color GripDark
	{
		get { return Theme[ThemeColor.SEPARATOR]; }
	}


	/// <inheritdoc/>
	public override Color GripLight
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginGradientBegin
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginGradientEnd
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginGradientMiddle
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginRevealedGradientBegin
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginRevealedGradientEnd
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ImageMarginRevealedGradientMiddle
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuBorder
	{
		get { return Theme[ThemeColor.MENU_BORDER]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemBorder
	{
		get { return Theme[ThemeColor.MENU_HIGHLIGHT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemPressedGradientBegin
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemPressedGradientEnd
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemPressedGradientMiddle
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemSelected
	{
		get { return Theme[ThemeColor.MENU_HIGHLIGHT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemSelectedGradientBegin
	{
		get { return Theme[ThemeColor.MENU_HIGHLIGHT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuItemSelectedGradientEnd
	{
		get { return Theme[ThemeColor.MENU_HIGHLIGHT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuStripGradientBegin
	{
		get { return Theme[ThemeColor.MENU_BAR_BACK]; }
	}


	/// <inheritdoc/>
	public override Color MenuStripGradientEnd
	{
		get { return Theme[ThemeColor.MENU_BAR_BACK]; }
	}


	/// <inheritdoc/>
	public override Color OverflowButtonGradientBegin
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color OverflowButtonGradientEnd
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color OverflowButtonGradientMiddle
	{
		get { return Theme[ThemeColor.HOT_BACK]; }
	}


	/// <inheritdoc/>
	public override Color RaftingContainerGradientBegin
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color RaftingContainerGradientEnd
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color SeparatorDark
	{
		get { return Theme[ThemeColor.SEPARATOR]; }
	}


	/// <inheritdoc/>
	public override Color SeparatorLight
	{
		get { return Theme[ThemeColor.SEPARATOR]; }
	}


	/// <inheritdoc/>
	public override Color StatusStripGradientBegin
	{
		get { return Theme[ThemeColor.STATUS_BACK]; }
	}


	/// <inheritdoc/>
	public override Color StatusStripGradientEnd
	{
		get { return Theme[ThemeColor.STATUS_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripBorder
	{
		get { return Theme[ThemeColor.SEPARATOR]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripContentPanelGradientBegin
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripContentPanelGradientEnd
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripDropDownBackground
	{
		get { return Theme[ThemeColor.MENU_BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripGradientBegin
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripGradientEnd
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripGradientMiddle
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripPanelGradientBegin
	{
		get { return Theme[ThemeColor.BACK]; }
	}


	/// <inheritdoc/>
	public override Color ToolStripPanelGradientEnd
	{
		get { return Theme[ThemeColor.BACK]; }
	}
}
