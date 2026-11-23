namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to menus, tool bars and status bars.</summary>
[ThemeAdapter(typeof(ToolStrip))]
[SupportedOSPlatform("windows")]
internal sealed class _ToolStripAdapter: ThemeAdapter<ToolStrip>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<ToolStrip>                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The tool strip gets a <see cref="ThemeToolStripRenderer"/>; drop-down menus of its items use the renderer as well.</remarks>
	protected override void _Apply(ToolStrip control, Theme theme, ThemeAdapterState state)
	{
		if(control.Renderer is ThemeToolStripRenderer renderer)
		{
			if(renderer.Theme != theme)
			{
				renderer.Theme = theme;
				control.Invalidate();
			}
		}
		else
		{
			ToolStripRenderMode mode = control.RenderMode;
			ToolStripRenderer original = control.Renderer;
			state.OnRestore("renderer", () =>
			{
				if(control.IsDisposed) { return; }
				if(mode == ToolStripRenderMode.Custom) { control.Renderer = original; } else { control.RenderMode = mode; }
			});

			control.Renderer = new ThemeToolStripRenderer(theme);
		}

		if(control is ToolStripDropDown) { _ApplyWindowTheme(control, state); }
	}
}
