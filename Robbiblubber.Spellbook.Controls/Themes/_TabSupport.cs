namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class provides helpers for tab controls that draw themselves.</summary>
/// <remarks>A tab control with <see cref="ControlStyles.UserPaint"/> no longer passes its font to the native control, so the native
///          control would measure the tabs with the system font; <see cref="_SendFont(TabControl)"/> passes it again.</remarks>
[SupportedOSPlatform("windows")]
internal static class _TabSupport
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _FontHandleHolder                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class holds a font handle and deletes it when it is collected.</summary>
	private sealed class _FontHandleHolder
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="handle">Font handle.</param>
		public _FontHandleHolder(IntPtr handle)
		{
			Handle = handle;
		}


		/// <summary>Deletes the font handle.</summary>
		~_FontHandleHolder()
		{
			if(Handle != IntPtr.Zero) { _NativeMethods._DeleteObject(Handle); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the font handle.</summary>
		public IntPtr Handle
		{
			get;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Protected method <c>SetStyle</c> of <see cref="Control"/>.</summary>
	private static readonly MethodInfo? _SetStyle = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Protected method <c>GetStyle</c> of <see cref="Control"/>.</summary>
	private static readonly MethodInfo? _GetStyle = typeof(Control).GetMethod("GetStyle", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Internal property <c>FontHandle</c> of <see cref="Control"/>.</summary>
	private static readonly PropertyInfo? _FontHandle = typeof(Control).GetProperty("FontHandle", BindingFlags.Instance | BindingFlags.NonPublic);

	/// <summary>Font handles created for fonts, if the font handle of the control cannot be read.</summary>
	private static readonly ConditionalWeakTable<Font, _FontHandleHolder> _FontHandles = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the font handle of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the handle.</returns>
	/// <remarks>WinForms keeps the handle in an internal property, which is an <see cref="IntPtr"/> or a handle structure with a field
	///          <c>Value</c>, depending on the version; if it cannot be read, a handle is created for the font.</remarks>
	private static IntPtr _GetFontHandle(Control control)
	{
		try
		{
			object? value = _FontHandle?.GetValue(control);
			if(value is IntPtr rval) { return rval; }
			if((value?.GetType().GetField("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(value)) is IntPtr inner) { return inner; }
		}
		catch(Exception) {}

		return _FontHandles.GetValue(control.Font, m => new _FontHandleHolder(m.ToHfont())).Handle;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static properties                                                                                       //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the styles that a tab control needs to draw itself.</summary>
	internal static ControlStyles _Styles
	{
		get { return (ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the styles of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the styles of <see cref="_Styles"/> that are set, NULL if they cannot be read.</returns>
	internal static ControlStyles? _GetStyles(Control control)
	{
		if(_GetStyle is null) { return null; }

		try
		{
			ControlStyles rval = 0;
			foreach(ControlStyles i in new[] { ControlStyles.UserPaint, ControlStyles.AllPaintingInWmPaint, ControlStyles.OptimizedDoubleBuffer, ControlStyles.ResizeRedraw })
			{
				if(_GetStyle.Invoke(control, new object[] { i }) is true) { rval |= i; }
			}
			return rval;
		}
		catch(Exception) { return null; }
	}


	/// <summary>Sets the styles of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="styles">Styles of <see cref="_Styles"/> that are set; the others are cleared.</param>
	/// <returns>Returns <c>true</c> if the styles have been set, otherwise returns <c>false</c>.</returns>
	internal static bool _SetStyles(Control control, ControlStyles styles)
	{
		if(_SetStyle is null) { return false; }

		try
		{
			_SetStyle.Invoke(control, new object[] { (_Styles & styles), true });
			_SetStyle.Invoke(control, new object[] { (_Styles & ~styles), false });
			return true;
		}
		catch(Exception) { return false; }
	}


	/// <summary>Passes the font of a tab control to the native control.</summary>
	/// <param name="control">Tab control.</param>
	internal static void _SendFont(TabControl control)
	{
		if(!control.IsHandleCreated) { return; }

		IntPtr font = _GetFontHandle(control);
		if(font != IntPtr.Zero) { _NativeMethods._SendMessage(control.Handle, _NativeMethods._WM_SETFONT, font, (IntPtr) 1); }
	}


	/// <summary>Gets the tab at a point.</summary>
	/// <param name="control">Tab control.</param>
	/// <param name="point">Point in client coordinates.</param>
	/// <returns>Returns the index of the tab, -1 if there is no tab at the point.</returns>
	internal static int _HitTest(TabControl control, Point point)
	{
		if(!control.IsHandleCreated) { return -1; }

		for(int i = 0; i < control.TabCount; i++)
		{
			if(control.GetTabRect(i).Contains(point)) { return i; }
		}
		return -1;
	}
}
