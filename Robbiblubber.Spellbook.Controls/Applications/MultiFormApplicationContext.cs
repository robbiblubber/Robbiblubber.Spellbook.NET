namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;



/// <summary>This class implements an application context for applications that show several equal top-level windows, for example one
///          window per document.</summary>
/// <remarks><para>The application runs until the last window added with <see cref="AddForm(Form)"/> or
///          <see cref="ShowForm(Form)"/> has been closed.</para>
///          <para>With a <see cref="SingleInstance"/>, the context receives the arguments of further starts of the application and
///          raises <see cref="ArgumentsReceived"/> on the user interface thread. Further instances should pass their arguments with
///          <see cref="Forward(SingleInstance, IEnumerable{string}, int)"/>, which allows the first instance to bring its windows to
///          the foreground.</para></remarks>
/// <example><code>
/// using SingleInstance instance = new("robbiblubber.org/MyEditor");
/// if(!instance.IsFirstInstance &amp;&amp; MultiFormApplicationContext.Forward(instance, args)) { return; }
///
/// ApplicationConfiguration.Initialize();
/// MultiFormApplicationContext context = new(instance);
/// context.ArgumentsReceived += (s, e) => { foreach(string i in e.Arguments) { context.ShowForm(new DocumentWindow(e.GetFullPath(i))); } };
/// context.ShowForm(new DocumentWindow());
/// Application.Run(context);
/// </code></example>
[SupportedOSPlatform("windows")]
public class MultiFormApplicationContext: ApplicationContext
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Forms.</summary>
	private readonly List<Form> _Forms = new();

	/// <summary>Synchronization context of the user interface thread.</summary>
	private readonly SynchronizationContext _Sync;

	/// <summary>Single instance.</summary>
	private SingleInstance? _Instance;

	/// <summary>Disposed flag.</summary>
	private bool _Disposed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="instance">Single instance, NULL if the application does not receive arguments from further instances.</param>
	/// <remarks>The context has to be created on the user interface thread; it uses the synchronization context of this thread,
	///          or a <see cref="WindowsFormsSynchronizationContext"/> if there is none yet. If <paramref name="instance"/> is the
	///          first instance, the context starts listening for arguments.</remarks>
	public MultiFormApplicationContext(SingleInstance? instance = null): base()
	{
		SynchronizationContext? sync = SynchronizationContext.Current;
		_Sync = (((sync != null) && (sync.GetType() != typeof(SynchronizationContext))) ? sync : new WindowsFormsSynchronizationContext());

		if((instance != null) && instance.IsFirstInstance)
		{
			_Instance = instance;
			_Instance.ArgumentsReceived += _Instance_ArgumentsReceived;
			_Instance.StartListening();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Passes arguments to the first instance and allows it to bring its windows to the foreground.</summary>
	/// <param name="instance">Single instance; this must not be the first instance.</param>
	/// <param name="arguments">Arguments.</param>
	/// <param name="timeout">Time in milliseconds to wait for the first instance.</param>
	/// <returns>Returns TRUE if the first instance has received the arguments, otherwise returns FALSE.</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="instance"/> is the first instance.</exception>
	public static bool Forward(SingleInstance instance, IEnumerable<string> arguments, int timeout = SingleInstance.DEFAULT_TIMEOUT)
	{
		ArgumentNullException.ThrowIfNull(instance);

		_NativeMethods._AllowSetForegroundWindow(_NativeMethods._ASFW_ANY);
		return instance.Forward(arguments, timeout);
	}


	/// <summary>Restores a window if it is minimized and brings it to the foreground.</summary>
	/// <param name="form">Form.</param>
	public static void ActivateForm(Form form)
	{
		ArgumentNullException.ThrowIfNull(form);
		if(form.IsDisposed) return;

		if(!form.Visible) { form.Show(); }
		if(form.WindowState == FormWindowState.Minimized) { _NativeMethods._ShowWindow(form.Handle, _NativeMethods._SW_RESTORE); }

		form.Activate();
		_NativeMethods._SetForegroundWindow(form.Handle);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Occurs when a further instance of the application has passed its arguments.</summary>
	/// <remarks>The event is raised on the user interface thread.</remarks>
	public event EventHandler<SingleInstanceEventArgs>? ArgumentsReceived;


	/// <summary>Occurs when a form has been added.</summary>
	public event EventHandler<FormEventArgs>? FormAdded;


	/// <summary>Occurs when a form has been removed because it has been closed.</summary>
	public event EventHandler<FormEventArgs>? FormRemoved;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Gets the forms of the application in the order they have been added.</summary>
	public IReadOnlyList<Form> Forms
	{
		get { return _Forms.AsReadOnly(); }
	}


	/// <summary>Gets the single instance, NULL if the context does not receive arguments from further instances.</summary>
	public SingleInstance? Instance
	{
		get { return _Instance; }
	}


	/// <summary>Gets or sets if the application ends when the last form has been closed.</summary>
	public bool ExitOnLastFormClosed
	{
		get; set;
	} = true;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Adds a form to the application.</summary>
	/// <param name="form">Form.</param>
	/// <remarks>The form is removed when it is closed or disposed. Adding a form twice has no effect.</remarks>
	public virtual void AddForm(Form form)
	{
		ArgumentNullException.ThrowIfNull(form);
		ObjectDisposedException.ThrowIf(form.IsDisposed, form);
		if(_Forms.Contains(form)) return;

		_Forms.Add(form);
		form.FormClosed += _Form_FormClosed;
		form.Disposed += _Form_Disposed;

		_OnFormAdded(new FormEventArgs(form));
	}


	/// <summary>Adds a form to the application, shows and activates it.</summary>
	/// <param name="form">Form.</param>
	public virtual void ShowForm(Form form)
	{
		AddForm(form);
		form.Show();
		form.Activate();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Raises the <see cref="ArgumentsReceived"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnArgumentsReceived(SingleInstanceEventArgs e)
	{
		ArgumentsReceived?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="FormAdded"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnFormAdded(FormEventArgs e)
	{
		FormAdded?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="FormRemoved"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnFormRemoved(FormEventArgs e)
	{
		FormRemoved?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Removes a form.</summary>
	/// <param name="form">Form.</param>
	private void _Remove(Form form)
	{
		if(!_Forms.Remove(form)) return;

		form.FormClosed -= _Form_FormClosed;
		form.Disposed -= _Form_Disposed;
		_OnFormRemoved(new FormEventArgs(form));

		if((_Forms.Count == 0) && ExitOnLastFormClosed) { ExitThread(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] ApplicationContext                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Releases the resources used by this instance.</summary>
	/// <param name="disposing">Determines if managed resources are released.</param>
	/// <remarks>The single instance is not disposed.</remarks>
	protected override void Dispose(bool disposing)
	{
		if(disposing && (!_Disposed))
		{
			_Disposed = true;
			if(_Instance != null)
			{
				_Instance.ArgumentsReceived -= _Instance_ArgumentsReceived;
				_Instance = null;
			}
		}
		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Arguments received from a further instance.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _Instance_ArgumentsReceived(object? sender, SingleInstanceEventArgs e)
	{
		_Sync.Post(_ => { if(!_Disposed) { _OnArgumentsReceived(e); } }, null);
	}


	/// <summary>Form closed.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _Form_FormClosed(object? sender, FormClosedEventArgs e)
	{
		if(sender is Form f) { _Remove(f); }
	}


	/// <summary>Form disposed.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _Form_Disposed(object? sender, EventArgs e)
	{
		if(sender is Form f) { _Remove(f); }
	}
}
