namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



/// <summary>This class provides a base for popup windows that never become active.</summary>
/// <remarks>Windows derived from this class never take the focus, so keyboard input stays with the edited control. The class handles
///          the window styles, showing without activation (including the z-order), the owner handling while hidden and an optional
///          custom border color.</remarks>
[SupportedOSPlatform("windows")]
internal abstract class _NoActivateWindowBase: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Border color.</summary>
	private Color _BorderColor = Color.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	protected _NoActivateWindowBase(): base()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable, false);

		AutoScaleMode = AutoScaleMode.None;
		FormBorderStyle = FormBorderStyle.None;
		ControlBox = MinimizeBox = MaximizeBox = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.Manual;
		Text = string.Empty;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws the custom border.</summary>
	private void _DrawBorder()
	{
		IntPtr dc;

		if(_BorderColor.IsEmpty || (!IsHandleCreated)) { return; }

		dc = _GetWindowDC(Handle);
		if(dc == IntPtr.Zero) { return; }

		try
		{
			using(Graphics g = Graphics.FromHdc(dc))
			using(Pen pen = new(_BorderColor))
			{
				g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
			}
		}
		finally { _ReleaseDC(Handle, dc); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the border color.</summary>
	/// <remarks>If the color is <see cref="Color.Empty"/>, the system border is used.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color BorderColor
	{
		get { return _BorderColor; }
		set
		{
			if(value == _BorderColor) { return; }

			_BorderColor = value;
			_DrawBorder();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Shows the window without activating it.</summary>
	/// <param name="bounds">Window bounds in screen coordinates.</param>
	/// <param name="owner">Owner form.</param>
	/// <remarks>A hidden window without owner keeps its old z-order position, so the window is explicitly moved to the top of the
	///          z-order when it is shown. The actual window state is checked with the Win32 API, so the window is shown even if the
	///          visibility state of Windows Forms and the actual window state ever differ.</remarks>
	public void ShowNoActivate(Rectangle bounds, Form? owner)
	{
		Bounds = bounds;

		if(Visible && IsHandleCreated && _IsWindowVisible(Handle)) { return; }

		if(Owner != owner) { Owner = owner; }

		if(Visible)
		{
			_ShowWindow(Handle, _SW_SHOWNOACTIVATE);
		}
		else { Show(); }

		_SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, _SWP_NOMOVE | _SWP_NOSIZE | _SWP_NOACTIVATE);
	}


	/// <summary>Hides the window.</summary>
	/// <remarks>The owner is released, so the window is not closed and disposed together with its owner while it is hidden. The window
	///          is additionally hidden with the Win32 API, so it cannot stay on the screen if the visibility state of Windows Forms and
	///          the actual window state ever differ.</remarks>
	public void HideWindow()
	{
		if(Visible) { Hide(); }
		if(IsHandleCreated) { _ShowWindow(Handle, _SW_HIDE); }

		Owner = null;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams rval = base.CreateParams;

			rval.Style |= (_WS_POPUP | _WS_BORDER);
			rval.ExStyle |= (_WS_EX_NOACTIVATE | _WS_EX_TOOLWINDOW);
			rval.ClassStyle |= _CS_DROPSHADOW;

			return rval;
		}
	}


	/// <inheritdoc/>
	protected override bool ShowWithoutActivation
	{
		get { return true; }
	}


	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if(m.Msg == _WM_MOUSEACTIVATE)
		{
			m.Result = (IntPtr) _MA_NOACTIVATE;
			return;
		}

		base.WndProc(ref m);

		if((m.Msg == _WM_NCPAINT) || (m.Msg == _WM_NCACTIVATE)) { _DrawBorder(); }
	}
}
