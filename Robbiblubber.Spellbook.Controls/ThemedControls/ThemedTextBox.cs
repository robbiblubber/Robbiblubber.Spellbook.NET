namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;

using Slot = Robbiblubber.Spellbook.Controls.ThemedTextBoxColors._Slot;



/// <summary>This class implements a text box that uses the colors of a theme.</summary>
/// <remarks><para>The text box is a real <see cref="TextBox"/> (editing, selection, undo, auto completion and the context menu stay
///          native). It takes its back and text colors from its color table (<see cref="Colors"/>), which takes its colors from the
///          theme of the application, the theme applied to its form or its own theme, and draws its border in the border color of
///          the theme: lighter while the mouse is over it, in the focus color while it has the focus. Read-only and disabled text boxes
///          use the read-only and disabled colors; the scroll bars of a multiline text box are dark in dark themes.</para>
///          <para>The border is drawn for <see cref="BorderStyle.FixedSingle"/> and <see cref="BorderStyle.Fixed3D"/>. Windows draws
///          the text of a disabled text box in the gray text color of the system.</para></remarks>
[SupportedOSPlatform("windows")]
[ToolboxBitmap(typeof(TextBox))]
public class ThemedTextBox: TextBox, Themes.IThemedControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Color table.</summary>
	private readonly ThemedTextBoxColors _Colors = new();

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedTextBox(): base()
	{
		_Colors.Changed += _Colors_Changed;
		_SetBaseBackColor();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Updates the colors after the state of the text box or the color table has changed.</summary>
	private void _Update()
	{
		_SetBaseBackColor();
		OnBackColorChanged(EventArgs.Empty);
		OnForeColorChanged(EventArgs.Empty);
		_ApplyWindowTheme();
		Invalidate();
		_RedrawBorder();
	}


	/// <summary>Draws the border again.</summary>
	private void _RedrawBorder()
	{
		_FieldBorder._Redraw(this, BorderStyle, _BorderColor, BackColor);
	}


	/// <summary>Stores the current back color in the base control as well.</summary>
	/// <remarks>Windows Forms lets the visual style paint the background of read-only and disabled text boxes unless the back color
	///          has been set explicitly; the color table alone is not enough.</remarks>
	private void _SetBaseBackColor()
	{
		base.BackColor = BackColor;
	}


	/// <summary>Sets the visual style of the native window, so the scroll bars are dark in dark themes.</summary>
	private void _ApplyWindowTheme()
	{
		if(!IsHandleCreated) { return; }

		_NativeMethods._SetWindowTheme(Handle, (_Colors.EffectiveTheme.IsDark ? "DarkMode_Explorer" : null), null);
	}


	/// <summary>Handles a message: draws the border instead of Windows.</summary>
	/// <param name="m">Message.</param>
	private void _HandleMessage(ref Message m)
	{
		if(_FieldBorder._IsPaintMessage(ref m))
		{
			_FieldBorder._HandlePaintMessage(ref m, this, BorderStyle, _BorderColor, BackColor, base.WndProc);
			return;
		}

		base.WndProc(ref m);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the current border color.</summary>
	internal Color _BorderColor
	{
		get { return _FieldBorder._GetColor(this, _Colors, _Hot); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the color table.</summary>
	[Category("Appearance")][Description("The colors of the text box: a theme and optional overrides of single colors.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ThemedTextBoxColors Colors
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
		get { return false; }
	}


	/// <summary>Gets or sets the back color.</summary>
	/// <remarks>The getter returns the back color for the current state (normal, read-only or disabled); the setter sets
	///          <see cref="ThemedTextBoxColors.BackColor"/> of the color table. System colors, which the designer may have written for
	///          the standard control, are ignored, so the color of the theme is used.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color BackColor
	{
		get { return _Colors._Get((!Enabled) ? Slot.DISABLED_BACK_COLOR : (ReadOnly ? Slot.READ_ONLY_BACK_COLOR : Slot.BACK_COLOR)); }
		set { _Colors.BackColor = (value.IsSystemColor ? Color.Empty : value); }
	}


	/// <summary>Gets or sets the text color.</summary>
	/// <remarks>The getter returns the text color for the current state (normal, read-only or disabled); the setter sets
	///          <see cref="ThemedTextBoxColors.ForeColor"/> of the color table. System colors, which the designer may have written for
	///          the standard control, are ignored, so the color of the theme is used.</remarks>
	[Browsable(false)][EditorBrowsable(EditorBrowsableState.Never)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Color ForeColor
	{
		get { return _Colors._Get((!Enabled) ? Slot.DISABLED_FORE_COLOR : (ReadOnly ? Slot.READ_ONLY_FORE_COLOR : Slot.FORE_COLOR)); }
		set { _Colors.ForeColor = (value.IsSystemColor ? Color.Empty : value); }
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
		_Update();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>With <see cref="ThemeManager.ThemeTextSelection"/>, the selection is drawn in the selection colors of the theme.</remarks>
	protected override void WndProc(ref Message m)
	{
		_SysColorHook._Run(ref m, Handle, _Colors.EffectiveTheme, _HandleMessage);
	}


	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		_ApplyWindowTheme();
	}


	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		_Hot = true;
		_RedrawBorder();
		base.OnMouseEnter(e);
	}


	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		_Hot = false;
		_RedrawBorder();
		base.OnMouseLeave(e);
	}


	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		_RedrawBorder();
	}


	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		_RedrawBorder();
	}


	/// <inheritdoc/>
	protected override void OnEnabledChanged(EventArgs e)
	{
		_Hot = false;
		base.OnEnabledChanged(e);
		_Update();
	}


	/// <inheritdoc/>
	protected override void OnReadOnlyChanged(EventArgs e)
	{
		base.OnReadOnlyChanged(e);
		_Update();
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing) { _Colors.Changed -= _Colors_Changed; }

		base.Dispose(disposing);
	}
}
