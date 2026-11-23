namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class draws the border of an up-down control in the colors of a theme.</summary>
/// <remarks>With visual styles, an up-down control draws its border in its <c>OnPaint</c> method after the <c>Paint</c> event. The
///          class subclasses the control and draws the border like the one of a themed text box after the control has painted itself;
///          it tracks the mouse over the control and its parts and the focus of its text box.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _UpDownBorderWindow: NativeWindow
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by up-down control.</summary>
	private static readonly ConditionalWeakTable<UpDownBase, _UpDownBorderWindow> _Windows = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Up-down control.</summary>
	private readonly UpDownBase _Control;

	/// <summary>Theme.</summary>
	private Theme _Theme = Theme.AUTO;

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;

	/// <summary>Determines if a part of the control has the focus.</summary>
	private bool _Focused = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Up-down control.</param>
	private _UpDownBorderWindow(UpDownBase control)
	{
		_Control = control;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the border color for the state of the control.</summary>
	/// <returns>Returns the color.</returns>
	private Color _GetColor()
	{
		if(!_Control.Enabled) { return _Theme[ThemeColor.DISABLED_BORDER]; }
		if(_Focused) { return _Theme[ThemeColor.FIELD_BORDER_FOCUSED]; }

		return _Theme[_Hot ? ThemeColor.FIELD_BORDER_HOT : ThemeColor.FIELD_BORDER];
	}


	/// <summary>Draws the border.</summary>
	private void _Draw()
	{
		if(!_Control.IsHandleCreated || (_Control.BorderStyle == BorderStyle.None) || (_Control.Width < 2) || (_Control.Height < 2)) { return; }

		using Graphics g = _Control.CreateGraphics();
		Rectangle bounds = _Control.ClientRectangle;

		using(SolidBrush brush = new(_Control.BackColor)) { g.FillRectangle(brush, bounds); }
		using(Pen pen = new(_GetColor())) { g.DrawRectangle(pen, 0, 0, bounds.Width - 1, bounds.Height - 1); }
	}


	/// <summary>Updates the mouse state and draws the border again.</summary>
	private void _Update()
	{
		_Hot = (_Control.Enabled && _Control.IsHandleCreated && _Control.ClientRectangle.Contains(_Control.PointToClient(Control.MousePosition)));
		_Draw();
	}


	/// <summary>Installs the event handlers.</summary>
	private void _Subscribe()
	{
		foreach(Control i in _Control.Controls.Cast<Control>().Prepend(_Control))
		{
			i.MouseEnter += _Control_Changed;
			i.MouseLeave += _Control_Changed;
			i.GotFocus += _Control_GotFocus;
			i.LostFocus += _Control_LostFocus;
		}
		_Control.EnabledChanged += _Control_Changed;
		_Focused = _Control.ContainsFocus;
	}


	/// <summary>Removes the event handlers and the subclass.</summary>
	private void _Unsubscribe()
	{
		foreach(Control i in _Control.Controls.Cast<Control>().Prepend(_Control))
		{
			i.MouseEnter -= _Control_Changed;
			i.MouseLeave -= _Control_Changed;
			i.GotFocus -= _Control_GotFocus;
			i.LostFocus -= _Control_LostFocus;
		}
		_Control.EnabledChanged -= _Control_Changed;

		if(Handle != IntPtr.Zero) { ReleaseHandle(); }
		_Windows.Remove(_Control);
		if(!_Control.IsDisposed) { _Control.Invalidate(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the border of an up-down control in the colors of a theme until the control is restored.</summary>
	/// <param name="control">Up-down control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="state">State of the control.</param>
	/// <remarks>Nothing is subclassed if the handle has not been created yet; the theme manager applies the theme again when the handle
	///          is created.</remarks>
	internal static void _Install(UpDownBase control, Theme theme, ThemeAdapterState state)
	{
		if(!control.IsHandleCreated) { return; }

		_UpDownBorderWindow window = _Windows.GetValue(control, m => new _UpDownBorderWindow(m));
		window._Theme = theme;

		if(window.Handle != control.Handle)
		{
			if(window.Handle != IntPtr.Zero) { window.ReleaseHandle(); }
			window.AssignHandle(control.Handle);
		}
		state.Subscribe("updown-border", window._Subscribe, window._Unsubscribe);
		control.Invalidate();
	}


	/// <summary>Gets the border color that an up-down control has now.</summary>
	/// <param name="control">Up-down control.</param>
	/// <returns>Returns the color, <see cref="Color.Empty"/> if the border of the control is not drawn.</returns>
	internal static Color _GetBorderColor(UpDownBase control)
	{
		return (_Windows.TryGetValue(control, out _UpDownBorderWindow? window) ? window._GetColor() : Color.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the GotFocus event of the control and its parts.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_GotFocus(object? sender, EventArgs e)
	{
		_Focused = true;
		_Update();
	}


	/// <summary>Handles the LostFocus event of the control and its parts.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_LostFocus(object? sender, EventArgs e)
	{
		_Focused = false;
		_Update();
	}


	/// <summary>Handles the mouse and enabled events of the control and its parts.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_Changed(object? sender, EventArgs e)
	{
		_Update();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] NativeWindow                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		if(m.Msg == _NativeMethods._WM_PAINT) { _Draw(); }
	}
}
