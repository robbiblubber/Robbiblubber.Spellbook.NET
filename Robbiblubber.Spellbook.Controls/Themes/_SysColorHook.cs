namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class lets text boxes draw their selection in the selection colors of a theme.</summary>
/// <remarks><para>Edit and rich edit controls draw the selection in the system colors <c>COLOR_HIGHLIGHT</c> and
///          <c>COLOR_HIGHLIGHTTEXT</c>, which they get from <c>GetSysColor</c> and <c>GetSysColorBrush</c>, or with a visual style from
///          <c>GetThemeSysColor</c>, <c>GetThemeSysColorBrush</c> and <c>GetThemeColor</c>; there is no setting per control. The class
///          replaces these functions in the import tables of the libraries that implement the controls in this process (comctl32,
///          uxtheme, msftedit, riched20). The replacements return the colors of the theme while a themed text box handles a message on
///          the current thread (<see cref="_Run"/>), and the original values otherwise.</para>
///          <para>The import tables stay patched until the process ends; with <see cref="ThemeManager.ThemeTextSelection"/> off, the
///          replacements only pass the calls on. Without visual styles, the edit control of user32 is used, which is not
///          affected.</para>
///          <para>The edit control of comctl32 gets the selection colors internally; <see cref="_EditSelection"/> draws its selection
///          again.</para></remarks>
[SupportedOSPlatform("windows")]
internal static unsafe class _SysColorHook
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>System color index: selection background.</summary>
	private const int _COLOR_HIGHLIGHT = 13;

	/// <summary>System color index: selection text.</summary>
	private const int _COLOR_HIGHLIGHTTEXT = 14;

	/// <summary>Theme property: selection background.</summary>
	private const int _TMT_HIGHLIGHT = 1614;

	/// <summary>Theme property: selection text.</summary>
	private const int _TMT_HIGHLIGHTTEXT = 1615;

	/// <summary>Memory protection: read and write.</summary>
	private const uint _PAGE_READWRITE = 0x04;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Names of the libraries whose import tables are patched.</summary>
	private static readonly HashSet<string> _ModuleNames = new(StringComparer.OrdinalIgnoreCase) { "comctl32.dll", "uxtheme.dll", "msftedit.dll", "riched20.dll", "riched32.dll" };

	/// <summary>Base addresses of the patched libraries.</summary>
	private static readonly HashSet<IntPtr> _Patched = new();

	/// <summary>Brushes for the selection colors by COLORREF value; they are kept until the process ends.</summary>
	private static readonly Dictionary<int, IntPtr> _Brushes = new();

	/// <summary>Lock object.</summary>
	private static readonly object _Lock = new();

	/// <summary>Original GetSysColor function.</summary>
	private static delegate* unmanaged[Stdcall]<int, int> _GetSysColor = null;

	/// <summary>Original GetSysColorBrush function.</summary>
	private static delegate* unmanaged[Stdcall]<int, IntPtr> _GetSysColorBrush = null;

	/// <summary>Original GetThemeSysColor function.</summary>
	private static delegate* unmanaged[Stdcall]<IntPtr, int, int> _GetThemeSysColor = null;

	/// <summary>Original GetThemeSysColorBrush function.</summary>
	private static delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr> _GetThemeSysColorBrush = null;

	/// <summary>Original GetThemeColor function.</summary>
	private static delegate* unmanaged[Stdcall]<IntPtr, int, int, int, int*, int> _GetThemeColor = null;

	/// <summary>Replacements: name of the function, original address and address of the replacement.</summary>
	private static (string Name, IntPtr Original, IntPtr Replacement)[] _Replacements = Array.Empty<(string, IntPtr, IntPtr)>();

	/// <summary>Determines if patching has failed, so it is not tried again.</summary>
	private static bool _Failed = false;

	/// <summary>Determines if the current thread is handling a message of a themed text box.</summary>
	[ThreadStatic]
	private static bool _Active;

	/// <summary>Selection back color of the text box as COLORREF value.</summary>
	[ThreadStatic]
	private static int _Back;

	/// <summary>Selection text color of the text box as COLORREF value.</summary>
	[ThreadStatic]
	private static int _Text;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a color to a COLORREF value.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the COLORREF value.</returns>
	private static int _ToColorRef(Color color)
	{
		return (color.R | (color.G << 8) | (color.B << 16));
	}


	/// <summary>Replaces GetSysColor.</summary>
	/// <param name="index">System color index.</param>
	/// <returns>Returns the color as COLORREF value.</returns>
	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
	private static int _HookGetSysColor(int index)
	{
		return _GetColor(index);
	}


	/// <summary>Replaces GetSysColorBrush.</summary>
	/// <param name="index">System color index.</param>
	/// <returns>Returns the brush.</returns>
	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
	private static IntPtr _HookGetSysColorBrush(int index)
	{
		if(_Active && ((index == _COLOR_HIGHLIGHT) || (index == _COLOR_HIGHLIGHTTEXT)))
		{
			try
			{
				return _GetBrush((index == _COLOR_HIGHLIGHT) ? _Back : _Text);
			}
			catch(Exception) {}
		}

		return _GetSysColorBrush(index);
	}


	/// <summary>Gets the selection color for a system color index or a theme color property.</summary>
	/// <param name="index">System color index or theme property.</param>
	/// <param name="color">Returns the color as COLORREF value.</param>
	/// <returns>Returns <c>true</c> if a themed text box handles a message on the current thread and the index is a selection color,
	///          otherwise <c>false</c>.</returns>
	private static bool _TryGetSelectionColor(int index, out int color)
	{
		color = 0;
		if(!_Active) { return false; }

		if((index == _COLOR_HIGHLIGHT) || (index == _TMT_HIGHLIGHT)) { color = _Back; return true; }
		if((index == _COLOR_HIGHLIGHTTEXT) || (index == _TMT_HIGHLIGHTTEXT)) { color = _Text; return true; }

		return false;
	}


	/// <summary>Replaces GetThemeSysColor.</summary>
	/// <param name="theme">Theme handle.</param>
	/// <param name="index">System color index.</param>
	/// <returns>Returns the color as COLORREF value.</returns>
	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
	private static int _HookGetThemeSysColor(IntPtr theme, int index)
	{
		return (_TryGetSelectionColor(index, out int color) ? color : _GetThemeSysColor(theme, index));
	}


	/// <summary>Replaces GetThemeSysColorBrush.</summary>
	/// <param name="theme">Theme handle.</param>
	/// <param name="index">System color index.</param>
	/// <returns>Returns the brush.</returns>
	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
	private static IntPtr _HookGetThemeSysColorBrush(IntPtr theme, int index)
	{
		if(_TryGetSelectionColor(index, out int color))
		{
			try
			{
				return _GetBrush(color);
			}
			catch(Exception) {}
		}

		return _GetThemeSysColorBrush(theme, index);
	}


	/// <summary>Replaces GetThemeColor.</summary>
	/// <param name="theme">Theme handle.</param>
	/// <param name="part">Part.</param>
	/// <param name="state">State.</param>
	/// <param name="property">Property.</param>
	/// <param name="color">Returns the color as COLORREF value.</param>
	/// <returns>Returns an HRESULT.</returns>
	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
	private static int _HookGetThemeColor(IntPtr theme, int part, int state, int property, int* color)
	{
		if((color != null) && _TryGetSelectionColor(property, out int value))
		{
			*color = value;
			return 0;
		}

		return _GetThemeColor(theme, part, state, property, color);
	}


	/// <summary>Gets a brush for a color.</summary>
	/// <param name="color">Color as COLORREF value.</param>
	/// <returns>Returns the brush.</returns>
	private static IntPtr _GetBrush(int color)
	{
		lock(_Lock)
		{
			if(!_Brushes.TryGetValue(color, out IntPtr rval))
			{
				rval = _NativeMethods._CreateSolidBrush(color);
				_Brushes[color] = rval;
			}

			return rval;
		}
	}


	/// <summary>Patches the import table of a library.</summary>
	/// <param name="module">Base address of the library.</param>
	private static void _PatchModule(IntPtr module)
	{
		byte* image = (byte*) module;
		int header = *(int*) (image + 0x3C);
		if(*(uint*) (image + header) != 0x00004550) { return; }

		byte* optional = (image + header + 24);
		int directories = ((*(ushort*) optional == 0x020B) ? 112 : 96);

		uint imports = *(uint*) (optional + directories + 8);
		if(imports != 0)
		{
			for(byte* i = (image + imports); *(uint*) (i + 12) != 0; i += 20) { _PatchThunks(image, *(uint*) i, *(uint*) (i + 16)); }
		}

		uint delayed = *(uint*) (optional + directories + (13 * 8));
		if(delayed != 0)
		{
			for(byte* i = (image + delayed); *(uint*) (i + 4) != 0; i += 32)
			{
				if((*(uint*) i & 1) != 0) { _PatchThunks(image, *(uint*) (i + 16), *(uint*) (i + 12)); }
			}
		}
	}


	/// <summary>Patches the entries of one imported library in an import table.</summary>
	/// <param name="image">Base address of the library.</param>
	/// <param name="names">Address of the name table relative to the base address, 0 if there is none.</param>
	/// <param name="addresses">Address of the address table relative to the base address.</param>
	private static void _PatchThunks(byte* image, uint names, uint addresses)
	{
		IntPtr* slot = (IntPtr*) (image + addresses);
		IntPtr* name = ((names != 0) ? (IntPtr*) (image + names) : null);
		for(int i = 0; slot[i] != IntPtr.Zero; i++)
		{
			string? function = null;
			if(name is not null)
			{
				long value = name[i].ToInt64();
				if(value > 0) { function = Marshal.PtrToStringAnsi((IntPtr) (image + (uint) value + 2)); }
			}

			IntPtr replacement = IntPtr.Zero;
			foreach((string Name, IntPtr Original, IntPtr Replacement) j in _Replacements)
			{
				if((slot[i] == j.Original) || (function == j.Name)) { replacement = j.Replacement; break; }
			}
			if((replacement == IntPtr.Zero) || (slot[i] == replacement)) { continue; }

			if(!_NativeMethods._VirtualProtect((IntPtr) (slot + i), (UIntPtr) IntPtr.Size, _PAGE_READWRITE, out uint protection)) { continue; }
			slot[i] = replacement;
			_NativeMethods._VirtualProtect((IntPtr) (slot + i), (UIntPtr) IntPtr.Size, protection, out _);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Patches the libraries that are loaded and have not been patched yet.</summary>
	/// <returns>Returns <c>true</c> if the replacements are installed, <c>false</c> if patching has failed.</returns>
	/// <remarks>Libraries that are loaded later, like msftedit for the first rich text box, are patched by the next call.</remarks>
	internal static bool _Install()
	{
		lock(_Lock)
		{
			if(_Failed) { return false; }

			try
			{
				if(_GetSysColor == null)
				{
					IntPtr user32 = NativeLibrary.Load("user32.dll");
					_GetSysColor = (delegate* unmanaged[Stdcall]<int, int>) NativeLibrary.GetExport(user32, "GetSysColor");
					_GetSysColorBrush = (delegate* unmanaged[Stdcall]<int, IntPtr>) NativeLibrary.GetExport(user32, "GetSysColorBrush");

					IntPtr uxtheme = NativeLibrary.Load("uxtheme.dll");
					_GetThemeSysColor = (delegate* unmanaged[Stdcall]<IntPtr, int, int>) NativeLibrary.GetExport(uxtheme, "GetThemeSysColor");
					_GetThemeSysColorBrush = (delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr>) NativeLibrary.GetExport(uxtheme, "GetThemeSysColorBrush");
					_GetThemeColor = (delegate* unmanaged[Stdcall]<IntPtr, int, int, int, int*, int>) NativeLibrary.GetExport(uxtheme, "GetThemeColor");

					_Replacements = new (string, IntPtr, IntPtr)[]
					{
						("GetSysColor", (IntPtr) _GetSysColor, (IntPtr) (delegate* unmanaged[Stdcall]<int, int>) &_HookGetSysColor),
						("GetSysColorBrush", (IntPtr) _GetSysColorBrush, (IntPtr) (delegate* unmanaged[Stdcall]<int, IntPtr>) &_HookGetSysColorBrush),
						("GetThemeSysColor", (IntPtr) _GetThemeSysColor, (IntPtr) (delegate* unmanaged[Stdcall]<IntPtr, int, int>) &_HookGetThemeSysColor),
						("GetThemeSysColorBrush", (IntPtr) _GetThemeSysColorBrush, (IntPtr) (delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr>) &_HookGetThemeSysColorBrush),
						("GetThemeColor", (IntPtr) _GetThemeColor, (IntPtr) (delegate* unmanaged[Stdcall]<IntPtr, int, int, int, int*, int>) &_HookGetThemeColor)
					};
				}

				using Process process = Process.GetCurrentProcess();
				foreach(ProcessModule i in process.Modules)
				{
					if(!_ModuleNames.Contains(i.ModuleName) || _Patched.Contains(i.BaseAddress)) { continue; }

					_PatchModule(i.BaseAddress);
					_Patched.Add(i.BaseAddress);
				}
			}
			catch(Exception)
			{
				_Failed = true;
				return false;
			}

			return true;
		}
	}


	/// <summary>Gets a system color as the replacement of GetSysColor returns it.</summary>
	/// <param name="index">System color index.</param>
	/// <returns>Returns the color as COLORREF value: the selection colors of the theme while a themed text box handles a message on
	///          the current thread, otherwise the system color.</returns>
	internal static int _GetColor(int index)
	{
		if(_Active)
		{
			if(index == _COLOR_HIGHLIGHT) { return _Back; }
			if(index == _COLOR_HIGHLIGHTTEXT) { return _Text; }
		}

		return _GetSysColor(index);
	}


	/// <summary>Handles a message of a text box with the selection colors of a theme.</summary>
	/// <param name="m">Message.</param>
	/// <param name="handle">Window handle of the text box.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="windowProcedure">Window procedure that handles the message.</param>
	/// <remarks>Without <see cref="ThemeManager.ThemeTextSelection"/>, the message is handled as it is. A text box without the focus
	///          gets the inactive selection colors. The selection of edit controls, which do not use the system colors of this class, is
	///          drawn again by <see cref="_EditSelection"/>.</remarks>
	internal static void _Run(ref Message m, IntPtr handle, Theme theme, _FieldBorder._WindowProcedure windowProcedure)
	{
		if(!ThemeManager.ThemeTextSelection)
		{
			windowProcedure(ref m);
			return;
		}
		if(((_GetSysColor == null) && !_Install()) || _Failed)
		{
			windowProcedure(ref m);
			_EditSelection._Update(ref m, handle, theme);
			return;
		}

		bool focused = (_NativeMethods._GetFocus() == handle);
		bool active = _Active;
		int back = _Back, text = _Text;

		_Back = _ToColorRef(theme[focused ? ThemeColor.SELECTION_BACK : ThemeColor.INACTIVE_SELECTION_BACK]);
		_Text = _ToColorRef(theme[focused ? ThemeColor.SELECTION_TEXT : ThemeColor.INACTIVE_SELECTION_TEXT]);
		_Active = true;
		try
		{
			windowProcedure(ref m);
		}
		finally
		{
			_Active = active;
			_Back = back;
			_Text = text;
		}
		_EditSelection._Update(ref m, handle, theme);
	}
}
