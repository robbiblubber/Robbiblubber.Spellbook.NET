namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class applies themes to group boxes.</summary>
[ThemeAdapter(typeof(GroupBox))]
[SupportedOSPlatform("windows")]
internal sealed class _GroupBoxAdapter: ThemeAdapter<GroupBox>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color tables of the group boxes.</summary>
	private static readonly ConditionalWeakTable<GroupBox, ThemedGroupBoxColors> _Tables = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<GroupBox>                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Group boxes are drawn like a <see cref="ThemedGroupBox"/>. The fore color is set to the text color of the theme, so the
	///          controls in the group box inherit it. Group boxes with <see cref="FlatStyle.System"/> are switched to
	///          <see cref="FlatStyle.Standard"/>, because Windows draws them otherwise. Group boxes are double buffered while the theme
	///          is applied, so the standard drawing under the theme drawing is not visible.</remarks>
	protected override void _Apply(GroupBox control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, null, ThemeColor.TEXT);
		if(control.FlatStyle == FlatStyle.System) { state.SetValue(control, nameof(GroupBox.FlatStyle), FlatStyle.Standard); }

		if((_TabSupport._GetStyles(control) is ControlStyles original) &&
		   _TabSupport._SetStyles(control, original | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer))
		{
			state.OnRestore("double-buffer", () =>
			{
				if(!control.IsDisposed) { _TabSupport._SetStyles(control, original); }
			});
		}

		ThemedGroupBoxColors colors = _Tables.GetValue(control, m => new ThemedGroupBoxColors());
		_Overpainter._Install(control, state, (g, current, states) =>
		{
			colors.Theme = current;
			colors.TextColor = control.ForeColor;

			_ControlRenderer._PaintGroupBox(g, control, colors, states);
		}, false);
	}
}
