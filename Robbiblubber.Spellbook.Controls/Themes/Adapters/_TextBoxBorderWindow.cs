namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class draws the border of a standard text box in the colors of a theme.</summary>
/// <remarks>The class subclasses the text box and draws the border instead of Windows like a <see cref="ThemedTextBox"/>; it tracks the
///          mouse and the focus with the events of the text box.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _TextBoxBorderWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by text box.</summary>
	private static readonly ConditionalWeakTable<TextBoxBase, _TextBoxBorderWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text box.</summary>
	private readonly TextBoxBase _TextBox;

	/// <summary>Color table.</summary>
	private readonly ThemedTextBoxColors _Colors = new();

	/// <summary>Determines if the mouse is over the text box.</summary>
	private bool _Hot = false;

	/// <summary>Window procedure that draws the border.</summary>
	private readonly _FieldBorder._WindowProcedure _BorderProcedure;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="textBox">Text box.</param>
	private _TextBoxBorderWindow(TextBoxBase textBox)
	{
		_TextBox = textBox;
		_BorderProcedure = _HandleMessage;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the border again.</summary>
	private void _Redraw()
	{
		_FieldBorder._Redraw(_TextBox, _TextBox.BorderStyle, _FieldBorder._GetColor(_TextBox, _Colors, _Hot), _TextBox.BackColor);
	}


	/// <summary>Handles a message: draws the border instead of Windows.</summary>
	/// <param name="m">Message.</param>
	private void _HandleMessage(ref Message m)
	{
		if(!_FieldBorder._IsPaintMessage(ref m))
		{
			base.WndProc(ref m);
			return;
		}

		_FieldBorder._HandlePaintMessage(ref m, _TextBox, _TextBox.BorderStyle, _FieldBorder._GetColor(_TextBox, _Colors, _Hot), _TextBox.BackColor, base.WndProc);
	}


	/// <summary>Installs the event handlers.</summary>
	private void _Subscribe()
	{
		_TextBox.MouseEnter += _TextBox_MouseEnter;
		_TextBox.MouseLeave += _TextBox_MouseLeave;
		_TextBox.GotFocus += _TextBox_Changed;
		_TextBox.LostFocus += _TextBox_Changed;
		_TextBox.EnabledChanged += _TextBox_Changed;
	}


	/// <summary>Removes the event handlers and the subclass.</summary>
	private void _Unsubscribe()
	{
		_TextBox.MouseEnter -= _TextBox_MouseEnter;
		_TextBox.MouseLeave -= _TextBox_MouseLeave;
		_TextBox.GotFocus -= _TextBox_Changed;
		_TextBox.LostFocus -= _TextBox_Changed;
		_TextBox.EnabledChanged -= _TextBox_Changed;

		if(Handle != IntPtr.Zero) { ReleaseHandle(); }
		_Windows.Remove(_TextBox);
		if(!_TextBox.IsDisposed) { _FieldBorder._RedrawNative(_TextBox); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the border of a text box in the colors of a theme until the text box is restored.</summary>
	/// <param name="textBox">Text box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the text box.</param>
	/// <remarks>Nothing is subclassed if the handle has not been created yet; the theme manager applies the theme again when the handle
	///          is created.</remarks>
	internal static void _Install(TextBoxBase textBox, Theme theme, ThemeAdapterState state)
	{
		if(!textBox.IsHandleCreated) { return; }

		_TextBoxBorderWindow window = _Windows.GetValue(textBox, m => new _TextBoxBorderWindow(m));
		window._Colors.Theme = theme;

		if(window.Handle != textBox.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(textBox.Handle);
		}
		state.Subscribe("field-border", window._Subscribe, window._Unsubscribe);
		if(ThemeManager.ThemeTextSelection) { _SysColorHook._Install(); }
		window._Redraw();
		textBox.Invalidate();
	}


	/// <summary>Gets the border color that a text box has now.</summary>
	/// <param name="textBox">Text box.</param>
	/// <returns>Returns the color, <see cref="Color.Empty"/> if the border of the text box is not drawn.</returns>
	internal static Color _GetBorderColor(TextBoxBase textBox)
	{
		if(!_Windows.TryGetValue(textBox, out _TextBoxBorderWindow? window)) { return Color.Empty; }

		return _FieldBorder._GetColor(textBox, window._Colors, window._Hot);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the MouseEnter event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextBox_MouseEnter(object? sender, EventArgs e)
	{
		_Hot = true;
		_Redraw();
	}


	/// <summary>Handles the MouseLeave event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextBox_MouseLeave(object? sender, EventArgs e)
	{
		_Hot = false;
		_Redraw();
	}


	/// <summary>Handles the GotFocus, LostFocus and EnabledChanged events of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextBox_Changed(object? sender, EventArgs e)
	{
		if(!_TextBox.Enabled) { _Hot = false; }
		_Redraw();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>With <see cref="ThemeManager.ThemeTextSelection"/>, the selection is drawn in the selection colors of the theme.</remarks>
	protected override void WndProc(ref Message m)
	{
		_SysColorHook._Run(ref m, Handle, _Colors.EffectiveTheme, _BorderProcedure);
	}
}
