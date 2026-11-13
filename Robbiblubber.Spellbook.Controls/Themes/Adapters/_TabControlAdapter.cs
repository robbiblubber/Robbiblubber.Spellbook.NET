namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class applies themes to tab controls.</summary>
[ThemeAdapter(typeof(TabControl))]
[SupportedOSPlatform("windows")]
internal sealed class _TabControlAdapter: ThemeAdapter<TabControl>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _TabState                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class holds the drawing state of a tab control.</summary>
	private sealed class _TabState
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the color table.</summary>
		public ThemedTabControlColors Colors
		{
			get;
		} = new();


		/// <summary>Gets or sets the index of the tab under the mouse, -1 for none.</summary>
		public int Hot
		{
			get; set;
		} = -1;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Drawing states of the tab controls.</summary>
	private static readonly ConditionalWeakTable<TabControl, _TabState> _States = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the index of the tab under the mouse that the adapter uses for a tab control.</summary>
	/// <param name="control">Tab control.</param>
	/// <returns>Returns the index, -1 for none.</returns>
	internal static int _GetHot(TabControl control)
	{
		return (_States.TryGetValue(control, out _TabState? rval) ? rval.Hot : -1);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<TabControl>                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>Tab controls are drawn like a <see cref="ThemedTabControl"/>: the adapter switches the control to
	///          <see cref="ControlStyles.UserPaint"/> and draws it in the <see cref="Control.Paint"/> event. Tab controls with
	///          <see cref="TabDrawMode.OwnerDrawFixed"/> keep their look, because the application draws the tabs.</remarks>
	protected override void _Apply(TabControl control, Theme theme, ThemeAdapterState state)
	{
		if(control.DrawMode == TabDrawMode.OwnerDrawFixed) { return; }

		ControlStyles? original = _TabSupport._GetStyles(control);
		if((original is not ControlStyles styles) || !_TabSupport._SetStyles(control, _TabSupport._Styles)) { return; }

		state.OnRestore("user-paint", () =>
		{
			if(control.IsDisposed) { return; }

			_TabSupport._SetStyles(control, styles);
			_TabSupport._SendFont(control);
			control.Invalidate();
		});
		_TabSupport._SendFont(control);

		_TabState tabs = _States.GetValue(control, m => new _TabState());

		void mouseMove(object? sender, MouseEventArgs e)
		{
			int hot = _TabSupport._HitTest(control, e.Location);
			if(hot == tabs.Hot) { return; }

			tabs.Hot = hot;
			control.Invalidate();
		}

		void mouseLeave(object? sender, EventArgs e)
		{
			tabs.Hot = -1;
			control.Invalidate();
		}

		void invalidate(object? sender, EventArgs e)
		{
			control.Invalidate();
		}

		void fontChanged(object? sender, EventArgs e)
		{
			_TabSupport._SendFont(control);
			control.Invalidate();
		}

		state.Subscribe("tabs", () =>
		{
			control.MouseMove += mouseMove;
			control.MouseLeave += mouseLeave;
			control.SelectedIndexChanged += invalidate;
			control.FontChanged += fontChanged;
		}, () =>
		{
			control.MouseMove -= mouseMove;
			control.MouseLeave -= mouseLeave;
			control.SelectedIndexChanged -= invalidate;
			control.FontChanged -= fontChanged;
			tabs.Hot = -1;
		});

		_Overpainter._Install(control, state, (g, current, paintStates) =>
		{
			tabs.Colors.Theme = current;
			_ControlRenderer._PaintTabControl(g, control, tabs.Colors, (control.Parent?.BackColor ?? Color.Empty), tabs.Hot, paintStates & ~(_PaintStates.HOT | _PaintStates.PRESSED | _PaintStates.DEFAULT));
		});
	}
}
