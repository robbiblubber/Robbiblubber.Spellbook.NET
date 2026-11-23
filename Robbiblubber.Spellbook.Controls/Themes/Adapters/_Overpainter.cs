namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class paints standard controls over with the control renderer.</summary>
/// <remarks>The standard control paints itself first; the handler of its <see cref="Control.Paint"/> event then draws the whole
///          control again like the themed control. The class tracks the mouse, the space key and the focus, because the standard
///          controls do not expose their interaction state.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _Overpainter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instances by control.</summary>
	private static readonly ConditionalWeakTable<Control, _Overpainter> _Instances = new();

	/// <summary>Protected property <c>ShowFocusCues</c> of <see cref="Control"/>.</summary>
	private static readonly PropertyInfo? _ShowFocusCues = typeof(Control).GetProperty("ShowFocusCues", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Protected property <c>ShowKeyboardCues</c> of <see cref="Control"/>.</summary>
	private static readonly PropertyInfo? _ShowKeyboardCues = typeof(Control).GetProperty("ShowKeyboardCues", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Protected property <c>IsDefault</c> of <see cref="ButtonBase"/>.</summary>
	private static readonly PropertyInfo? _IsDefault = typeof(ButtonBase).GetProperty("IsDefault", BindingFlags.Instance | BindingFlags.NonPublic);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Control.</summary>
	private readonly Control _Control;

	/// <summary>State of the control.</summary>
	private readonly ThemeAdapterState _State;

	/// <summary>Paint action.</summary>
	private readonly Action<Graphics, Theme, _PaintStates> _Paint;

	/// <summary>Determines if the control is drawn again when the mouse, the space key or the focus change.</summary>
	private readonly bool _Interactive;

	/// <summary>Determines if the mouse is over the control.</summary>
	private bool _Hot = false;

	/// <summary>Determines if the left mouse button has been pressed on the control.</summary>
	private bool _MousePressed = false;

	/// <summary>Determines if the space key is pressed.</summary>
	private bool _KeyPressed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	/// <param name="state">State of the control.</param>
	/// <param name="paint">Paint action.</param>
	/// <param name="interactive">Determines if the control is drawn again when the mouse, the space key or the focus change.</param>
	private _Overpainter(Control control, ThemeAdapterState state, Action<Graphics, Theme, _PaintStates> paint, bool interactive)
	{
		_Control = control;
		_State = state;
		_Paint = paint;
		_Interactive = interactive;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a protected Boolean property.</summary>
	/// <param name="property">Property, NULL if it has not been found.</param>
	/// <param name="control">Control.</param>
	/// <param name="fallback">Value if the property cannot be read.</param>
	/// <returns>Returns the value.</returns>
	private static bool _Get(PropertyInfo? property, Control control, bool fallback)
	{
		try
		{
			return ((property?.GetValue(control) as bool?) ?? fallback);
		}
		catch(Exception) { return fallback; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private properties                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the interaction states.</summary>
	private _PaintStates _States
	{
		get
		{
			_PaintStates rval = _PaintStates.NONE;

			if(_Hot) { rval |= _PaintStates.HOT; }
			if((_MousePressed && _Hot) || _KeyPressed) { rval |= _PaintStates.PRESSED; }
			if(_Control.Focused && _Get(_ShowFocusCues, _Control, true)) { rval |= _PaintStates.FOCUSED; }
			if(!_Get(_ShowKeyboardCues, _Control, false)) { rval |= _PaintStates.HIDE_PREFIX; }
			if((_Control is ButtonBase) && _Get(_IsDefault, _Control, false)) { rval |= _PaintStates.DEFAULT; }

			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Installs the event handlers.</summary>
	private void _Subscribe()
	{
		_Control.Paint += _Control_Paint;
		_Control.EnabledChanged += _Control_EnabledChanged;
		_Control.Invalidate();
		if(!_Interactive) { return; }

		_Control.MouseEnter += _Control_MouseEnter;
		_Control.MouseLeave += _Control_MouseLeave;
		_Control.MouseDown += _Control_MouseDown;
		_Control.MouseUp += _Control_MouseUp;
		_Control.KeyDown += _Control_KeyDown;
		_Control.KeyUp += _Control_KeyUp;
		_Control.GotFocus += _Control_FocusChanged;
		_Control.LostFocus += _Control_FocusChanged;
	}


	/// <summary>Removes the event handlers.</summary>
	private void _Unsubscribe()
	{
		_Control.Paint -= _Control_Paint;
		_Control.MouseEnter -= _Control_MouseEnter;
		_Control.MouseLeave -= _Control_MouseLeave;
		_Control.MouseDown -= _Control_MouseDown;
		_Control.MouseUp -= _Control_MouseUp;
		_Control.KeyDown -= _Control_KeyDown;
		_Control.KeyUp -= _Control_KeyUp;
		_Control.GotFocus -= _Control_FocusChanged;
		_Control.LostFocus -= _Control_FocusChanged;
		_Control.EnabledChanged -= _Control_EnabledChanged;
		_Instances.Remove(_Control);
		_Control.Invalidate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Paints a control over with the control renderer until it is restored.</summary>
	/// <param name="control">Control.</param>
	/// <param name="state">State of the control.</param>
	/// <param name="paint">Paint action; it gets the graphics, the theme and the interaction states.</param>
	/// <param name="interactive">Determines if the control is drawn again when the mouse, the space key or the focus change;
	///                           <c>false</c> for controls that look the same in all these states, like group boxes.</param>
	internal static void _Install(Control control, ThemeAdapterState state, Action<Graphics, Theme, _PaintStates> paint, bool interactive = true)
	{
		if(_Instances.TryGetValue(control, out _))
		{
			control.Invalidate();
			return;
		}

		_Overpainter instance = new(control, state, paint, interactive);
		state.Subscribe("overpaint", () =>
		{
			_Instances.AddOrUpdate(control, instance);
			instance._Subscribe();
		}, instance._Unsubscribe);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Paint event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_Paint(object? sender, PaintEventArgs e)
	{
		if(_State.Theme is not Theme theme) { return; }

		_Paint(e.Graphics, theme, _States);
	}


	/// <summary>Handles the MouseEnter event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_MouseEnter(object? sender, EventArgs e)
	{
		_Hot = true;
		_Control.Invalidate();
	}


	/// <summary>Handles the MouseLeave event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_MouseLeave(object? sender, EventArgs e)
	{
		_Hot = false;
		_Control.Invalidate();
	}


	/// <summary>Handles the MouseDown event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_MouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button != MouseButtons.Left) { return; }

		_MousePressed = true;
		_Control.Invalidate();
	}


	/// <summary>Handles the MouseUp event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_MouseUp(object? sender, MouseEventArgs e)
	{
		if(e.Button != MouseButtons.Left) { return; }

		_MousePressed = false;
		_Control.Invalidate();
	}


	/// <summary>Handles the KeyDown event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_KeyDown(object? sender, KeyEventArgs e)
	{
		if((e.KeyCode != Keys.Space) || _KeyPressed) { return; }

		_KeyPressed = true;
		_Control.Invalidate();
	}


	/// <summary>Handles the KeyUp event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_KeyUp(object? sender, KeyEventArgs e)
	{
		if(e.KeyCode != Keys.Space) { return; }

		_KeyPressed = false;
		_Control.Invalidate();
	}


	/// <summary>Handles the GotFocus and LostFocus events of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_FocusChanged(object? sender, EventArgs e)
	{
		if(!_Control.Focused) { _KeyPressed = _MousePressed = false; }
		_Control.Invalidate();
	}


	/// <summary>Handles the EnabledChanged event of the control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_EnabledChanged(object? sender, EventArgs e)
	{
		_Hot = _MousePressed = _KeyPressed = false;
		_Control.Invalidate();
	}
}
