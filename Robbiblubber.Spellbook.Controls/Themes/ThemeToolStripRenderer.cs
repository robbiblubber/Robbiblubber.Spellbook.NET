namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class implements a renderer for menus, tool bars and status bars that uses the colors of a theme.</summary>
/// <remarks><para>The theme adapters set this renderer for all tool strips of a themed form. It can also be used on its own:</para>
///          <code>menuStrip.Renderer = new ThemeToolStripRenderer(Theme.DARK);</code>
///          <para>Item texts that use system colors get the matching colors of the theme; other explicitly set colors are kept. In dark
///          themes, single-colored dark images are lightened, and so are the dark outlines of colored images;
///          <see cref="ThemeManager.ResolveImage"/> allows to replace images.</para></remarks>
[SupportedOSPlatform("windows")]
public class ThemeToolStripRenderer: ToolStripProfessionalRenderer
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="theme">Theme.</param>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	public ThemeToolStripRenderer(Theme theme): base(new _ThemeColorTable(theme ?? throw new ArgumentNullException(nameof(theme))))
	{
		RoundedEdges = false;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the text color of an item.</summary>
	/// <param name="item">Item.</param>
	/// <param name="color">Text color of the item.</param>
	/// <returns>Returns the color to draw the text with.</returns>
	private Color _GetTextColor(ToolStripItem item, Color color)
	{
		Theme theme = Theme;

		if(!item.Enabled) { return theme[ThemeColor.DISABLED_TEXT]; }
		if(!color.IsSystemColor) { return color; }
		if(color.ToKnownColor() == KnownColor.GrayText) { return theme[ThemeColor.GRAY_TEXT]; }

		if((item.Selected || item.Pressed) && ((item.Owner is ToolStripDropDown) || (item.Owner is MenuStrip))) { return theme[ThemeColor.MENU_HIGHLIGHT_TEXT]; }
		if(item.Owner is StatusStrip) { return theme[ThemeColor.STATUS_TEXT]; }
		if((item.Owner is ToolStripDropDown) || (item.Owner is MenuStrip)) { return theme[ThemeColor.MENU_TEXT]; }

		return theme[ThemeColor.TEXT];
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the theme.</summary>
	/// <remarks>The tool strips that use the renderer have to be repainted after the theme has been changed.</remarks>
	/// <exception cref="ArgumentNullException">Thrown when the theme is NULL.</exception>
	public Theme Theme
	{
		get { return ((_ThemeColorTable) ColorTable).Theme; }
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			((_ThemeColorTable) ColorTable).Theme = value;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ToolStripProfessionalRenderer                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		e.TextColor = _GetTextColor(e.Item, e.TextColor);

		if((e.TextDirection == ToolStripTextDirection.Horizontal) && !e.Item.Enabled)
		{
			TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, e.TextColor, e.TextFormat);
			return;
		}
		base.OnRenderItemText(e);
	}


	/// <inheritdoc/>
	protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		if(e.Item is not null) { e.ArrowColor = _GetTextColor(e.Item, SystemColors.ControlText); }
		base.OnRenderArrow(e);
	}


	/// <inheritdoc/>
	protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		if(e.Image is null) { base.OnRenderItemImage(e); return; }

		base.OnRenderItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, ThemeManager._ResolveImage(e.Item, e.Image, Theme), e.ImageRectangle));
	}


	/// <inheritdoc/>
	protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		if(e.Image is null) { base.OnRenderItemCheck(e); return; }

		base.OnRenderItemCheck(new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, ThemeManager._ResolveImage(e.Item, e.Image, Theme), e.ImageRectangle));
	}


	/// <inheritdoc/>
	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		if(e.ToolStrip is StatusStrip)
		{
			using Pen pen = new(Theme[ThemeColor.SEPARATOR]);
			e.Graphics.DrawLine(pen, 0, 0, e.ToolStrip.Width, 0);
			return;
		}
		base.OnRenderToolStripBorder(e);
	}
}
