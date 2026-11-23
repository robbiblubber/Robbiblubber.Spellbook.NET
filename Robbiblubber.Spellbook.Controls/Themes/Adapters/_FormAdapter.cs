namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Runtime.Versioning;



/// <summary>This class applies themes to forms.</summary>
[ThemeAdapter(typeof(Form))]
[SupportedOSPlatform("windows")]
internal sealed class _FormAdapter: ThemeAdapter<Form>
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>First Windows build that supports caption colors (Windows 11).</summary>
	private const int _CAPTION_COLOR_BUILD = 22000;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a color to a native color value.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the color as 0x00BBGGRR.</returns>
	private static int _ToColorRef(Color color)
	{
		return (color.R | (color.G << 8) | (color.B << 16));
	}


	/// <summary>Sets the title bar attributes of a window.</summary>
	/// <param name="handle">Window handle.</param>
	/// <param name="dark">Determines if the title bar is dark.</param>
	/// <param name="caption">Caption color, <see cref="_NativeMethods._DWMWA_COLOR_DEFAULT"/> for the default.</param>
	/// <param name="text">Caption text color, <see cref="_NativeMethods._DWMWA_COLOR_DEFAULT"/> for the default.</param>
	private static void _SetTitleBar(IntPtr handle, bool dark, int caption, int text)
	{
		try
		{
			int value = (dark ? 1 : 0);
			if(_NativeMethods._DwmSetWindowAttribute(handle, _NativeMethods._DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int)) != 0)
			{
				_NativeMethods._DwmSetWindowAttribute(handle, _NativeMethods._DWMWA_USE_IMMERSIVE_DARK_MODE_OLD, ref value, sizeof(int));
			}

			if(Environment.OSVersion.Version.Build >= _CAPTION_COLOR_BUILD)
			{
				_NativeMethods._DwmSetWindowAttribute(handle, _NativeMethods._DWMWA_CAPTION_COLOR, ref caption, sizeof(int));
				_NativeMethods._DwmSetWindowAttribute(handle, _NativeMethods._DWMWA_TEXT_COLOR, ref text, sizeof(int));
			}
		}
		catch(Exception) {}
	}


	/// <summary>Gets if the application runs in dark mode.</summary>
	/// <returns>Returns <c>true</c> for the dark mode of .NET 9 and later, otherwise returns <c>false</c>.</returns>
	private static bool _IsDarkModeEnabled()
	{
#if NET9_0_OR_GREATER
		return Application.IsDarkModeEnabled;
#else
		return false;
#endif
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ThemeAdapter<Form>                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>The form gets the back and text color of the theme and a dark title bar in dark themes. On Windows 11, custom themes
	///          also color the title bar.</remarks>
	protected override void _Apply(Form control, Theme theme, ThemeAdapterState state)
	{
		_ApplyColors(control, state, ThemeColor.BACK, ThemeColor.TEXT);
		if(control.AutoScroll) { _ApplyWindowTheme(control, state); }

		if(!control.IsHandleCreated || !control.TopLevel) { return; }

		int caption = (theme.IsBuiltIn ? _NativeMethods._DWMWA_COLOR_DEFAULT : _ToColorRef(theme[ThemeColor.BACK]));
		int text = (theme.IsBuiltIn ? _NativeMethods._DWMWA_COLOR_DEFAULT : _ToColorRef(theme[ThemeColor.TEXT]));
		_SetTitleBar(control.Handle, theme.IsDark, caption, text);

		state.OnRestore("title-bar", () =>
		{
			if(control.IsHandleCreated && !control.IsDisposed) { _SetTitleBar(control.Handle, _IsDarkModeEnabled(), _NativeMethods._DWMWA_COLOR_DEFAULT, _NativeMethods._DWMWA_COLOR_DEFAULT); }
		});
	}
}
