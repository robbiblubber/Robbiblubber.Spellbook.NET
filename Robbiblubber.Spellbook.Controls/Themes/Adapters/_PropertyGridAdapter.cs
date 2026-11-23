namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to property grids.</summary>
[ThemeAdapter(typeof(PropertyGrid))]
[SupportedOSPlatform("windows")]
internal sealed class _PropertyGridAdapter: ThemeAdapter<PropertyGrid>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the visual style of the native windows of the inner controls.</summary>
	/// <param name="control">Control.</param>
	/// <param name="theme">Visual style.</param>
	private static void _SetInnerWindowTheme(Control control, string? theme)
	{
		foreach(Control i in control.Controls)
		{
			if(i.IsHandleCreated) { _NativeMethods._SetWindowTheme(i.Handle, theme, null); }
			_SetInnerWindowTheme(i, theme);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<PropertyGrid>                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void _Apply(PropertyGrid control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, ThemeColor.BACK, ThemeColor.TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.ViewBackColor), ThemeColor.FIELD_BACK);
		state.ApplyColor(control, nameof(PropertyGrid.ViewForeColor), ThemeColor.FIELD_TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.ViewBorderColor), ThemeColor.FIELD_BORDER);
		state.ApplyColor(control, nameof(PropertyGrid.LineColor), ThemeColor.HEADER_BACK);
		state.ApplyColor(control, nameof(PropertyGrid.CategoryForeColor), ThemeColor.HEADER_TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.CategorySplitterColor), ThemeColor.GRID_LINE);
		state.ApplyColor(control, nameof(PropertyGrid.SelectedItemWithFocusBackColor), ThemeColor.SELECTION_BACK);
		state.ApplyColor(control, nameof(PropertyGrid.SelectedItemWithFocusForeColor), ThemeColor.SELECTION_TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.DisabledItemForeColor), ThemeColor.DISABLED_TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.HelpBackColor), ThemeColor.BACK);
		state.ApplyColor(control, nameof(PropertyGrid.HelpForeColor), ThemeColor.TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.HelpBorderColor), ThemeColor.FIELD_BORDER);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsBackColor), ThemeColor.BACK);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsForeColor), ThemeColor.TEXT);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsBorderColor), ThemeColor.FIELD_BORDER);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsLinkColor), ThemeColor.LINK);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsActiveLinkColor), ThemeColor.ACCENT_PRESSED);
		state.ApplyColor(control, nameof(PropertyGrid.CommandsDisabledLinkColor), ThemeColor.DISABLED_TEXT);

		foreach(ToolStrip i in control.Controls.OfType<ToolStrip>())
		{
			if(i.Renderer is ThemeToolStripRenderer renderer)
			{
				renderer.Theme = theme;
				i.Invalidate();
			}
			else
			{
				ToolStripRenderer original = i.Renderer;
				state.OnRestore("tool-bar", () => { if(!i.IsDisposed) { i.Renderer = original; } });
				i.Renderer = new ThemeToolStripRenderer(theme);
			}
		}

		_SetInnerWindowTheme(control, (theme.IsDark ? "DarkMode_Explorer" : null));
		state.OnRestore("inner-window-theme", () => { if(!control.IsDisposed) { _SetInnerWindowTheme(control, _GetDefaultWindowTheme()); } });
	}


	/// <inheritdoc/>
	protected override bool _IncludesChildren(PropertyGrid control)
	{
		return false;
	}
}
