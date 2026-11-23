namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements a tab control that draws its tabs with the colors of a theme.</summary>
/// <remarks><para>The tab control behaves like a <see cref="TabControl"/> (pages, keyboard navigation, multiline, alignment, images) and
///          draws its tabs flat in the colors of its color table (<see cref="Colors"/>), which takes its colors from the theme of the
///          application, the theme applied to its form or its own theme: the selected tab is joined to the page area and has a line in
///          the accent color, the tab under the mouse is highlighted. With <see cref="TabAppearance.Buttons"/> the tabs are drawn like
///          buttons, the selected one in the accent color. The theme manager applies the theme to the tab pages.</para>
///          <para>With <see cref="TabDrawMode.OwnerDrawFixed"/> the control draws the background and the frames and raises
///          <see cref="TabControl.DrawItem"/> for the content of each tab. Because the tab control draws itself, the
///          <see cref="Control.Paint"/> event does not occur.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(TabControl))]
public class ThemedTabControl: TabControl, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ThemedTabControlColors _Colors = new();

	/// <summary>Index of the tab under the mouse, -1 for none.</summary>
	private int _Hot = -1;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedTabControl(): base()
	{
		SetStyle(_TabSupport._Styles | ControlStyles.SupportsTransparentBackColor, true);
		_Colors.Changed += _Colors_Changed;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets the tab under the mouse.</summary>
	/// <param name="index">Index, -1 for none.</param>
	private void _SetHot(int index)
	{
		if(index == _Hot) { return; }

		_Hot = index;
		Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the index of the tab under the mouse, -1 for none.</summary>
	internal int _HotIndex
	{
		get { return _Hot; }
	}


	/// <summary>Gets the states for the renderer.</summary>
	internal _PaintStates _States
	{
		get
		{
			_PaintStates rval = _PaintStates.NONE;

			if(Focused && ShowFocusCues) { rval |= _PaintStates.FOCUSED; }
			if(!ShowKeyboardCues) { rval |= _PaintStates.HIDE_PREFIX; }

			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the tab control: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedTabControlColors Colors
	{
		get { return _Colors; }
	}


	/// <summary>Gets the color table for the theme manager.</summary>
	Themes.ControlColorTable Themes.IThemedControl.ThemeColors
	{
		get { return _Colors; }
	}


	/// <summary>Gets if the theme manager applies the theme to the child controls.</summary>
	bool Themes.IThemedControl.ThemesChildren
	{
		get { return true; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Resets the color table.</summary>
	public void ResetColors()
	{
		_Colors.Reset();
	}


	/// <summary>Determines if the color table has to be serialized by the designer.</summary>
	/// <returns>Returns <c>true</c> if the color table differs from the default, otherwise returns <c>false</c>.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ShouldSerializeColors()
	{
		return (!_Colors.IsDefault);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Changed event of the color table.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Colors_Changed(object? sender, EventArgs e)
	{
		Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs e)
	{}


	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Action<Graphics, int, Rectangle, bool>? drawItem = null;
		if(DrawMode == TabDrawMode.OwnerDrawFixed)
		{
			drawItem = (g, index, bounds, selected) => OnDrawItem(new DrawItemEventArgs(g, Font, bounds, index, (selected ? DrawItemState.Selected : DrawItemState.None)));
		}

		_ControlRenderer._PaintTabControl(e.Graphics, this, _Colors, (Parent?.BackColor ?? Color.Empty), _Hot, _States, drawItem);
	}


	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		_TabSupport._SendFont(this);
	}


	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		_TabSupport._SendFont(this);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		_SetHot(_TabSupport._HitTest(this, e.Location));
		base.OnMouseMove(e);
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		_SetHot(-1);
		base.OnMouseLeave(e);
	}


	/// <inheritdoc/>
	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		Invalidate();
		base.OnSelectedIndexChanged(e);
	}


	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		Invalidate();
		base.OnGotFocus(e);
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		Invalidate();
		base.OnLostFocus(e);
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		_Hot = -1;
		base.OnEnabledChanged(e);
		Invalidate();
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _Colors.Changed -= _Colors_Changed; }

		base.Dispose(disposing);
	}
}
