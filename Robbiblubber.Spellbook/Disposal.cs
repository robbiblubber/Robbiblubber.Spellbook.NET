namespace Robbiblubber.Spellbook;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;



/// <summary>This class releases objects as completely as possible, for example interop objects that have a <c>Close</c>
///          method but do not implement <see cref="IDisposable"/>.</summary>
/// <remarks>All methods are best effort: an object that cannot be released does not stop the others, and errors are not
///          thrown. The <c>Try…</c> methods report if everything succeeded.</remarks>
public static class Disposal
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>COM error code for an unknown member name (DISP_E_UNKNOWNNAME).</summary>
	private const int _DISP_E_UNKNOWNNAME = unchecked((int) 0x80020006);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds a public instance method that can be called without arguments.</summary>
	/// <param name="t">Type.</param>
	/// <param name="name">Method name.</param>
	/// <returns>Returns the overload with the fewest parameters that are all optional; NULL if there is none.</returns>
	private static MethodInfo? _FindMethod(Type t, string name)
	{
		return t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => ((m.Name == name) && (!m.ContainsGenericParameters) && m.GetParameters().All(p => p.IsOptional)))
				.OrderBy(m => m.GetParameters().Length)
				.FirstOrDefault();
	}


	/// <summary>Calls a method without arguments; optional parameters get their default values.</summary>
	/// <param name="obj">Object.</param>
	/// <param name="name">Method name.</param>
	/// <param name="lateBound">Determines if COM objects are called late bound (IDispatch).</param>
	/// <returns>Returns TRUE if the method has been called or does not exist, FALSE if it failed.</returns>
	private static bool _Call(object obj, string name, bool lateBound)
	{
		try
		{
			if(lateBound && Marshal.IsComObject(obj))
			{
				obj.GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, obj, null);
				return true;
			}

			MethodInfo? m = _FindMethod(obj.GetType(), name);
			m?.Invoke(obj, m.GetParameters().Select(p => (p.HasDefaultValue ? p.DefaultValue : Type.Missing)).ToArray());

			return true;
		}
		catch(COMException ex) when(ex.HResult == _DISP_E_UNKNOWNNAME) { return true; }
		catch(MissingMethodException) { return true; }
		catch(Exception) { return false; }
	}


	/// <summary>Closes and disposes an object.</summary>
	/// <param name="obj">Object.</param>
	/// <returns>Returns TRUE if all calls succeeded, otherwise returns FALSE.</returns>
	private static bool _Dispose(object obj)
	{
		bool rval = _Call(obj, "Close", true);

		try
		{
			if(obj is IDisposable d)
			{
				d.Dispose();
			}
			else if(obj is IAsyncDisposable a)
			{
				a.DisposeAsync().AsTask().GetAwaiter().GetResult();
			}
			else { rval = (_Call(obj, "Dispose", false) && rval); }
		}
		catch(Exception) { rval = false; }

		return rval;
	}


	/// <summary>Closes and disposes an object asynchronously.</summary>
	/// <param name="obj">Object.</param>
	/// <returns>Returns TRUE if all calls succeeded, otherwise returns FALSE.</returns>
	private static async Task<bool> _DisposeAsync(object obj)
	{
		bool rval = _Call(obj, "Close", true);

		try
		{
			if(obj is IAsyncDisposable a)
			{
				await a.DisposeAsync().ConfigureAwait(false);
			}
			else if(obj is IDisposable d)
			{
				d.Dispose();
			}
			else { rval = (_Call(obj, "Dispose", false) && rval); }
		}
		catch(Exception) { rval = false; }

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Closes and disposes objects.</summary>
	/// <param name="objects">Objects; NULL entries are skipped.</param>
	/// <remarks>For each object, a <c>Close</c> method that can be called without arguments is called first (optional
	///          parameters get their default values; COM objects are called late bound). Then the object is disposed via
	///          <see cref="IDisposable"/>, <see cref="IAsyncDisposable"/> or a public <c>Dispose</c> method. Errors are
	///          ignored.</remarks>
	public static void Dispose(params object?[]? objects)
	{
		TryDispose(objects);
	}


	/// <summary>Closes and disposes objects.</summary>
	/// <param name="objects">Objects; NULL entries are skipped.</param>
	/// <returns>Returns TRUE if all objects have been released without error, otherwise returns FALSE. All objects are
	///          processed in any case.</returns>
	/// <remarks>See <see cref="Dispose(object?[])"/>.</remarks>
	public static bool TryDispose(params object?[]? objects)
	{
		bool rval = true;

		foreach(object? i in (objects ?? []))
		{
			if(i is not null) { rval = (_Dispose(i) && rval); }
		}

		return rval;
	}


	/// <summary>Closes and disposes objects asynchronously; <see cref="IAsyncDisposable"/> is preferred.</summary>
	/// <param name="objects">Objects; NULL entries are skipped.</param>
	/// <returns>Returns TRUE if all objects have been released without error, otherwise returns FALSE. All objects are
	///          processed in any case.</returns>
	public static async Task<bool> TryDisposeAsync(params object?[]? objects)
	{
		bool rval = true;

		foreach(object? i in (objects ?? []))
		{
			if(i is not null) { rval = (await _DisposeAsync(i).ConfigureAwait(false) && rval); }
		}

		return rval;
	}


	/// <summary>Closes and disposes objects asynchronously; <see cref="IAsyncDisposable"/> is preferred. Errors are
	///          ignored.</summary>
	/// <param name="objects">Objects; NULL entries are skipped.</param>
	/// <returns>Returns a task that completes when all objects have been processed.</returns>
	public static Task DisposeAsync(params object?[]? objects)
	{
		return TryDisposeAsync(objects);
	}


	/// <summary>Disposes objects and forces a full garbage collection including finalizers.</summary>
	/// <param name="objects">Objects; NULL entries are skipped.</param>
	public static void Recycle(params object?[]? objects)
	{
		Dispose(objects);
		Recycle();
	}


	/// <summary>Forces a full garbage collection including finalizers, e.g. to make COM servers release their
	///          objects.</summary>
	public static void Recycle()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}


	/// <summary>Releases COM objects completely and forces a full garbage collection afterwards.</summary>
	/// <param name="objects">Objects; NULL entries and objects that are not COM objects are skipped.</param>
	/// <returns>Returns TRUE if all COM objects have been released without error, otherwise returns FALSE.</returns>
	/// <remarks>All references of the runtime callable wrapper are released (<see cref="Marshal.FinalReleaseComObject"/>), so
	///          that COM servers like Office applications can exit. The objects must not be used afterwards.</remarks>
	[SupportedOSPlatform("windows")]
	public static bool ReleaseComObjects(params object?[]? objects)
	{
		bool rval = true;

		foreach(object? i in (objects ?? []))
		{
			if((i is null) || (!Marshal.IsComObject(i))) { continue; }

			try
			{
				Marshal.FinalReleaseComObject(i);
			}
			catch(Exception) { rval = false; }
		}

		Recycle();
		return rval;
	}
}
