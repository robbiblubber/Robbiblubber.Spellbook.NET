namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ThemedGroupBoxColors._Slot;



/// <summary>This class implements a group box that draws itself with the colors of a theme.</summary>
/// <remarks><para>The group box draws its frame with rounded corners and its caption in the colors of its color table
///          (<see cref="Colors"/>), which takes its colors from the theme of the application, the theme applied to its form or its own
///          theme. The theme manager applies the theme to the controls in the group box as well.</para>
///          <para>Because the group box draws itself completely, the <see cref="Control.Paint"/> event does not occur.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(GroupBox))]
public class ThemedGroupBox: GroupBox, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ThemedGroupBoxColors _Colors = new();


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedGroupBox(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
		_Colors.Changed += _Colors_Changed;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the states for the renderer.</summary>
	internal _PaintStates _States
	{
		get { return (ShowKeyboardCues ? _PaintStates.NONE : _PaintStates.HIDE_PREFIX); }
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the group box: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedGroupBoxColors Colors
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


	/// <summary>Gets or sets the caption color.</summary>
	/// <remarks>This property gets and sets <see cref="ThemedGroupBoxColors.TextColor"/> of the color table. System colors, which the designer may have
	///          written for the standard control, are ignored, so the color of the theme is used.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color ForeColor
	{
		get { return _Colors._Get(Slot.TEXT_COLOR); }
		set { _Colors.TextColor = (value.IsSystemColor ? Color.Empty : value); }
	}


	/// <summary>Gets or sets the flat style.</summary>
	/// <remarks>The control always draws itself; this property has no effect and is kept for compatibility with the standard
	///          control.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new FlatStyle FlatStyle
	{
		get { return base.FlatStyle; }
		set {}
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
		OnForeColorChanged(EventArgs.Empty);
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
		_ControlRenderer._PaintGroupBox(e.Graphics, this, _Colors, _States);
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
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
