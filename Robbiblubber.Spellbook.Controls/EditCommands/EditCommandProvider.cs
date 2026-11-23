namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.Versioning;

using Timer = System.Windows.Forms.Timer;



/// <summary>This class implements a component that provides edit commands (cut, copy, paste, ...) for the focused control of a form
///          and binds them to menu items and tool bar buttons.</summary>
/// <remarks><para>The component resolves the target of a command from the focused control upwards: the first control that implements
///          <see cref="IEditable"/>, or <see cref="IEditableBearing"/> with a non-NULL <see cref="IEditableBearing.Editable"/>, handles
///          the command. If there is none, the adapter registered for the focused control in <see cref="EditAdapters"/> is used.
///          Controls with <c>EditCommandsEnabled</c> set to <c>false</c>, and their child controls, are excluded.</para>
///          <para>Menu items and tool bar buttons with an <c>EditCommand</c> execute the command when clicked; the component enables
///          and disables them. Drop-down menus are updated when they open, everything else shortly after the application becomes
///          idle. A disabled menu item does not consume its shortcut keys, so the focused control can still handle them itself.</para>
///          <para>A context menu with bound items moves the focus to the control it is opened for (if that control can take the
///          focus), so its commands apply to the clicked control and not to the control that had the focus before.</para>
///          <para>The <see cref="Querying"/> and <see cref="Executing"/> events let the application change the availability of
///          commands or implement them, for example delete in a list.</para></remarks>
[ProvideProperty("EditCommand", typeof(ToolStripItem))]
[ProvideProperty("EditCommandsEnabled", typeof(Control))]
[ToolboxBitmap(typeof(EditCommandProvider), "Resources.editcommands.bmp")]
[SupportedOSPlatform("windows")]
public class EditCommandProvider: Component, IExtenderProvider, IEditable, IComponent, IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>All edit commands.</summary>
	private static readonly EditCommand[] _AllCommands = Enum.GetValues<EditCommand>();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Container control whose focused control is the target of the commands.</summary>
	private ContainerControl? _ContainerControl = null;

	/// <summary>Commands of bound tool strip items.</summary>
	private readonly Dictionary<ToolStripItem, EditCommand> _Items = new();

	/// <summary>Controls excluded from edit commands.</summary>
	private readonly HashSet<Control> _DisabledControls = new();

	/// <summary>Drop-downs whose Opening event is hooked.</summary>
	private readonly HashSet<ToolStripDropDown> _DropDowns = new();

	/// <summary>Timer for delayed updates after the application has become idle.</summary>
	private readonly Timer _UpdateTimer = new();

	/// <summary>Determines if the Idle event of the application is hooked.</summary>
	private bool _IdleHooked = false;

	/// <summary>Determines if the next Idle event is caused by the update itself and has to be ignored.</summary>
	private bool _IgnoreIdle = false;

	/// <summary>Determines if bound items are updated automatically.</summary>
	private bool _AutoUpdate = true;

	/// <summary>Availability of all commands at the last update, NULL before the first update.</summary>
	private bool[]? _LastState = null;

	/// <summary>Editable object whose EditStateChanged event is hooked.</summary>
	private IEditable? _ObservedEditable = null;

	/// <summary>Handlers of the <see cref="EditStateChanged"/> event.</summary>
	private EventHandler? _EditStateChanged = null;

	/// <summary>Disposed flag.</summary>
	private bool _Disposed = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public EditCommandProvider(): base()
	{
		_UpdateTimer.Interval = 100;
		_UpdateTimer.Tick += _UpdateTimer_Tick;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public EditCommandProvider(IContainer container): this()
	{
		container.Add(this);
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="containerControl">Container control (usually the form) whose focused control is the target of the commands.</param>
	public EditCommandProvider(ContainerControl containerControl): this()
	{
		_ContainerControl = containerControl;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds the form a tool strip item belongs to.</summary>
	/// <param name="item">Tool strip item.</param>
	/// <returns>Returns the form, or NULL if the item is not on a form.</returns>
	private static Form? _FindForm(ToolStripItem item)
	{
		ToolStripItem top = item;

		while(top.OwnerItem is not null) { top = top.OwnerItem; }

		if(top.Owner is ContextMenuStrip menu) { return menu.SourceControl?.FindForm(); }
		return top.Owner?.FindForm();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the container control whose focused control is the target of the commands.</summary>
	/// <returns>Returns the container control, or NULL if it cannot be determined.</returns>
	private ContainerControl? _GetRoot()
	{
		if(_ContainerControl is not null) { return _ContainerControl; }

		foreach(ToolStripItem i in _Items.Keys)
		{
			if(_FindForm(i) is Form rval) { return rval; }
		}

		return null;
	}


	/// <summary>Returns if a control is excluded from edit commands.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the control or one of its parents is excluded, otherwise returns <c>false</c>.</returns>
	private bool _IsExcluded(Control control)
	{
		for(Control? i = control; i is not null; i = i.Parent)
		{
			if(_DisabledControls.Contains(i)) { return true; }
		}

		return false;
	}


	/// <summary>Resolves the target of the commands for a control.</summary>
	/// <param name="control">Focused control.</param>
	/// <returns>Returns the editable object that handles the commands, or the adapter of the control; both are NULL if the control
	///          does not support edit commands.</returns>
	private (IEditable? Editable, IEditAdapter? Adapter) _Resolve(Control control)
	{
		for(Control? i = control; i is not null; i = i.Parent)
		{
			if((i is IEditable editable) && (!ReferenceEquals(editable, this))) { return (editable, null); }
			if((i is IEditableBearing bearing) && (bearing.Editable is IEditable borne) && (!ReferenceEquals(borne, this))) { return (borne, null); }
		}

		return (null, EditAdapters.Find(control));
	}


	/// <summary>Returns if a command can be executed for a control, without raising events.</summary>
	/// <param name="control">Focused control.</param>
	/// <param name="command">Edit command.</param>
	/// <returns>Returns <c>true</c> if the command can be executed, otherwise returns <c>false</c>.</returns>
	private bool _CanExecuteDefault(Control? control, EditCommand command)
	{
		(IEditable? Editable, IEditAdapter? Adapter) target;

		if((control is null) || _IsExcluded(control)) { return false; }

		target = _Resolve(control);
		if(target.Editable is not null) { return target.Editable.CanExecute(command); }

		return (target.Adapter?.CanExecute(control, command) ?? false);
	}


	/// <summary>Executes a command for a control, without raising events.</summary>
	/// <param name="control">Focused control.</param>
	/// <param name="command">Edit command.</param>
	private void _ExecuteDefault(Control? control, EditCommand command)
	{
		(IEditable? Editable, IEditAdapter? Adapter) target;

		if((control is null) || _IsExcluded(control)) { return; }

		target = _Resolve(control);
		if(target.Editable is not null)
		{
			target.Editable.Execute(command);
		}
		else { target.Adapter?.Execute(control, command); }
	}


	/// <summary>Hooks or unhooks the Idle event of the application as needed.</summary>
	private void _UpdateIdleHook()
	{
		bool needed = ((!_Disposed) && (!DesignMode) && _AutoUpdate && ((_Items.Count > 0) || (_EditStateChanged is not null)));

		if(needed && (!_IdleHooked))
		{
			Application.Idle += _Application_Idle;
			_IdleHooked = true;
		}
		else if((!needed) && _IdleHooked)
		{
			Application.Idle -= _Application_Idle;
			_IdleHooked = false;
			_UpdateTimer.Stop();
		}
	}


	/// <summary>Hooks the Opening event of the drop-downs containing bound items.</summary>
	private void _HookDropDowns()
	{
		foreach(ToolStripItem i in _Items.Keys)
		{
			if((i.Owner is ToolStripDropDown dropDown) && _DropDowns.Add(dropDown))
			{
				dropDown.Opening += _DropDown_Opening;
				dropDown.Disposed += _DropDown_Disposed;
			}
		}
	}


	/// <summary>Unhooks a drop-down and removes it from the hooked drop-downs.</summary>
	/// <param name="dropDown">Drop-down.</param>
	private void _UnhookDropDown(ToolStripDropDown dropDown)
	{
		if(!_DropDowns.Remove(dropDown)) { return; }

		dropDown.Opening -= _DropDown_Opening;
		dropDown.Disposed -= _DropDown_Disposed;
	}


	/// <summary>Unhooks the drop-downs that no longer contain bound items.</summary>
	/// <remarks>The owner of an item is checked instead of the owner at binding time, because a disposed item has already been
	///          removed from its drop-down.</remarks>
	private void _UnhookUnusedDropDowns()
	{
		foreach(ToolStripDropDown i in _DropDowns.ToArray())
		{
			if(!_Items.Keys.Any(m => ReferenceEquals(m.Owner, i))) { _UnhookDropDown(i); }
		}
	}


	/// <summary>Observes the EditStateChanged event of the current editable object.</summary>
	/// <param name="editable">Editable object, NULL for none.</param>
	private void _Observe(IEditable? editable)
	{
		if(ReferenceEquals(editable, _ObservedEditable)) { return; }

		if(_ObservedEditable is not null)
		{
			_ObservedEditable.EditStateChanged -= _ObservedEditable_EditStateChanged;
			if(_ObservedEditable is Control oldControl) { oldControl.Disposed -= _ObservedEditable_Disposed; }
		}

		_ObservedEditable = editable;

		if(_ObservedEditable is not null)
		{
			_ObservedEditable.EditStateChanged += _ObservedEditable_EditStateChanged;
			if(_ObservedEditable is Control newControl) { newControl.Disposed += _ObservedEditable_Disposed; }
		}
	}


	/// <summary>Removes the binding of a tool strip item.</summary>
	/// <param name="item">Tool strip item.</param>
	private void _UnbindItem(ToolStripItem item)
	{
		if(!_Items.Remove(item)) { return; }

		item.Click -= _Item_Click;
		item.Disposed -= _Item_Disposed;

		_UnhookUnusedDropDowns();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="Querying"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnQuerying(EditCommandQueryEventArgs e)
	{
		Querying?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Executing"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnExecuting(EditCommandExecuteEventArgs e)
	{
		Executing?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Executed"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnExecuted(EditCommandEventArgs e)
	{
		Executed?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="EditStateChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnEditStateChanged(EventArgs e)
	{
		_EditStateChanged?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the availability of a command is determined.</summary>
	/// <remarks>Event handlers can change <see cref="EditCommandQueryEventArgs.CanExecute"/>, for example to enable delete in a list.</remarks>
	[Category("Edit Commands")][Description("Occurs when the availability of a command is determined. The result can be changed.")]
	public event EventHandler<EditCommandQueryEventArgs>? Querying;

	/// <summary>Occurs before a command is executed.</summary>
	/// <remarks>Event handlers can implement the command and set <see cref="EditCommandExecuteEventArgs.Handled"/> to skip the default
	///          implementation.</remarks>
	[Category("Edit Commands")][Description("Occurs before a command is executed. Handlers can implement the command themselves.")]
	public event EventHandler<EditCommandExecuteEventArgs>? Executing;

	/// <summary>Occurs after a command has been executed.</summary>
	[Category("Edit Commands")][Description("Occurs after a command has been executed.")]
	public event EventHandler<EditCommandEventArgs>? Executed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the container control (usually the form) whose focused control is the target of the commands.</summary>
	/// <remarks>In the designer, this property is set to the form or user control automatically. If it is NULL, the form of the first
	///          bound menu item or tool bar button is used.</remarks>
	[Category("Edit Commands")][Description("Form or user control whose focused control is the target of the commands.")][DefaultValue(null)]
	public ContainerControl? ContainerControl
	{
		get { return _ContainerControl; }
		set { _ContainerControl = value; }
	}


	/// <summary>Gets or sets if bound menu items and tool bar buttons are enabled and disabled automatically.</summary>
	/// <remarks>If this property is <c>false</c>, the application updates them by calling <see cref="UpdateItems"/>.</remarks>
	[Category("Edit Commands")][Description("Specifies if bound menu items and tool bar buttons are enabled and disabled automatically.")][DefaultValue(true)]
	public bool AutoUpdate
	{
		get { return _AutoUpdate; }
		set
		{
			_AutoUpdate = value;
			_UpdateIdleHook();
		}
	}


	/// <summary>Gets or sets the delay in milliseconds between the application becoming idle and the update of bound items.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
	[Category("Edit Commands")][Description("Delay in milliseconds between the application becoming idle and the update of bound items.")][DefaultValue(100)]
	public int UpdateInterval
	{
		get { return _UpdateTimer.Interval; }
		set
		{
			if(value < 1) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be at least 1."); }
			_UpdateTimer.Interval = value;
		}
	}


	/// <summary>Gets the control that is the target of the commands.</summary>
	/// <remarks>This is the focused control within <see cref="ContainerControl"/>, following nested container controls. For a user
	///          control as container, it is NULL while the focus is outside of the user control.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual Control? ActiveControl
	{
		get
		{
			ContainerControl? root = _GetRoot();
			Control? rval;

			if(root is null) { return null; }
			if((root is not Form) && (!root.ContainsFocus)) { return null; }

			rval = root.ActiveControl;
			while((rval is ContainerControl container) && (container.ActiveControl is not null) && (container.ActiveControl != rval)) { rval = container.ActiveControl; }

			return rval;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the edit command of a menu item or tool bar button.</summary>
	/// <param name="item">Tool strip item.</param>
	/// <returns>Returns the edit command, or NULL if the item is not bound to an edit command.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the item is NULL.</exception>
	[Category("Edit Commands")][Description("Edit command executed by this item. The item is enabled and disabled automatically.")][DefaultValue(null)]
	public virtual EditCommand? GetEditCommand(ToolStripItem item)
	{
		if(item is null) { throw new ArgumentNullException(nameof(item)); }
		return (_Items.TryGetValue(item, out EditCommand rval) ? rval : null);
	}


	/// <summary>Sets the edit command of a menu item or tool bar button.</summary>
	/// <param name="item">Tool strip item.</param>
	/// <param name="value">Edit command, NULL to remove the binding.</param>
	/// <exception cref="ArgumentNullException">Thrown when the item is NULL.</exception>
	public virtual void SetEditCommand(ToolStripItem item, EditCommand? value)
	{
		if(item is null) { throw new ArgumentNullException(nameof(item)); }

		if(value is null)
		{
			_UnbindItem(item);
		}
		else
		{
			if(!_Items.ContainsKey(item) && (!DesignMode))
			{
				item.Click += _Item_Click;
				item.Disposed += _Item_Disposed;
			}
			_Items[item] = value.Value;
		}

		_UpdateIdleHook();
	}


	/// <summary>Gets if edit commands are enabled for a control and its child controls.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if edit commands are enabled, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	[Category("Edit Commands")][Description("Specifies if the edit commands support this control and its child controls.")][DefaultValue(true)]
	public virtual bool GetEditCommandsEnabled(Control control)
	{
		if(control is null) { throw new ArgumentNullException(nameof(control)); }
		return (!_DisabledControls.Contains(control));
	}


	/// <summary>Sets if edit commands are enabled for a control and its child controls.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	public virtual void SetEditCommandsEnabled(Control control, bool value)
	{
		if(control is null) { throw new ArgumentNullException(nameof(control)); }

		if(value)
		{
			if(_DisabledControls.Remove(control)) { control.Disposed -= _Control_Disposed; }
		}
		else if(_DisabledControls.Add(control)) { control.Disposed += _Control_Disposed; }
	}


	/// <summary>Enables and disables the bound menu items and tool bar buttons according to the availability of their commands.</summary>
	/// <remarks>This method is called automatically if <see cref="AutoUpdate"/> is <c>true</c>. It also raises
	///          <see cref="EditStateChanged"/> if the availability of a command has changed since the last update.</remarks>
	public virtual void UpdateItems()
	{
		Dictionary<EditCommand, bool> states = new();
		Control? control;

		if(_Disposed || DesignMode) { return; }

		_HookDropDowns();

		control = ActiveControl;
		_Observe(((control is null) || _IsExcluded(control)) ? null : _Resolve(control).Editable);

		// event handlers called by CanExecute may bind, unbind or dispose items
		foreach(ToolStripItem i in _Items.Keys.ToArray())
		{
			if(i.IsDisposed || (!_Items.TryGetValue(i, out EditCommand command))) { continue; }

			if(!states.TryGetValue(command, out bool enabled))
			{
				enabled = CanExecute(command);
				states.Add(command, enabled);
			}

			if(i.IsDisposed || (!_Items.ContainsKey(i))) { continue; }
			if(i.Enabled != enabled) { i.Enabled = enabled; }
		}

		if(_EditStateChanged is not null)
		{
			bool[] current = _AllCommands.Select(m => (states.TryGetValue(m, out bool enabled) ? enabled : CanExecute(m))).ToArray();

			if((_LastState is null) || (!current.SequenceEqual(_LastState)))
			{
				_LastState = current;
				_OnEditStateChanged(EventArgs.Empty);
			}
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Idle event of the application.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>Updates are delayed by <see cref="UpdateInterval"/>, so a burst of messages leads to a single update. The Idle event
	///          that follows the update timer itself is ignored; otherwise the timer would restart itself and poll endlessly.</remarks>
	private void _Application_Idle(object? sender, EventArgs e)
	{
		if(_IgnoreIdle)
		{
			_IgnoreIdle = false;
			return;
		}

		if(!_UpdateTimer.Enabled) { _UpdateTimer.Start(); }
	}


	/// <summary>Handles the Tick event of the update timer.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _UpdateTimer_Tick(object? sender, EventArgs e)
	{
		_UpdateTimer.Stop();
		UpdateItems();

		_IgnoreIdle = true;
	}


	/// <summary>Handles the Opening event of drop-downs containing bound items.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>A context menu moves the focus to the control it is opened for, so its commands apply to that control.</remarks>
	private void _DropDown_Opening(object? sender, CancelEventArgs e)
	{
		if((sender is ContextMenuStrip menu) && (menu.SourceControl is Control source) && (!source.ContainsFocus) && source.CanFocus) { source.Focus(); }
		UpdateItems();
	}


	/// <summary>Handles the Disposed event of drop-downs containing bound items.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _DropDown_Disposed(object? sender, EventArgs e)
	{
		if(sender is ToolStripDropDown dropDown) { _UnhookDropDown(dropDown); }
	}


	/// <summary>Handles the Click event of bound items.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Item_Click(object? sender, EventArgs e)
	{
		if((sender is ToolStripItem item) && _Items.TryGetValue(item, out EditCommand command)) { Execute(command); }
	}


	/// <summary>Handles the Disposed event of bound items.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Item_Disposed(object? sender, EventArgs e)
	{
		if(sender is ToolStripItem item)
		{
			_UnbindItem(item);
			_UpdateIdleHook();
		}
	}


	/// <summary>Handles the Disposed event of excluded controls.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_Disposed(object? sender, EventArgs e)
	{
		if(sender is Control control) { SetEditCommandsEnabled(control, true); }
	}


	/// <summary>Handles the EditStateChanged event of the current editable object.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ObservedEditable_EditStateChanged(object? sender, EventArgs e)
	{
		if(_AutoUpdate) { UpdateItems(); }
	}


	/// <summary>Handles the Disposed event of the current editable object, if it is a control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ObservedEditable_Disposed(object? sender, EventArgs e)
	{
		if(ReferenceEquals(sender, _ObservedEditable)) { _Observe(null); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Component                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	/// <remarks>When the component is placed in the designer, <see cref="ContainerControl"/> is set to the form or user control.</remarks>
	public override ISite? Site
	{
		get { return base.Site; }
		set
		{
			base.Site = value;

			if((_ContainerControl is null) && (value?.GetService(typeof(IDesignerHost)) is IDesignerHost host) && (host.RootComponent is ContainerControl root)) { _ContainerControl = root; }
		}
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing && (!_Disposed))
		{
			_Disposed = true;
			_UpdateIdleHook();

			_UpdateTimer.Tick -= _UpdateTimer_Tick;
			_UpdateTimer.Dispose();

			foreach(ToolStripItem i in _Items.Keys.ToArray()) { _UnbindItem(i); }
			foreach(ToolStripDropDown i in _DropDowns.ToArray()) { _UnhookDropDown(i); }
			foreach(Control i in _DisabledControls) { i.Disposed -= _Control_Disposed; }

			_DropDowns.Clear();
			_DisabledControls.Clear();
			_Observe(null);
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditable                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the availability of a command has changed.</summary>
	/// <remarks>The availability is checked with the regular updates (see <see cref="UpdateItems"/>); while the event has handlers,
	///          the updates run even if no menu items or tool bar buttons are bound.</remarks>
	[Category("Edit Commands")][Description("Occurs when the availability of a command has changed.")]
	public event EventHandler? EditStateChanged
	{
		add
		{
			_EditStateChanged += value;
			_UpdateIdleHook();
		}
		remove
		{
			_EditStateChanged -= value;
			_UpdateIdleHook();
		}
	}


	/// <inheritdoc/>
	/// <remarks>The command applies to <see cref="ActiveControl"/>. The <see cref="Querying"/> event is raised.</remarks>
	public virtual bool CanExecute(EditCommand command)
	{
		Control? control = ActiveControl;
		EditCommandQueryEventArgs e = new(command, control, _CanExecuteDefault(control, command));

		_OnQuerying(e);
		return e.CanExecute;
	}


	/// <inheritdoc/>
	/// <remarks>The command applies to <see cref="ActiveControl"/>. If it can be executed, the <see cref="Executing"/> event is raised,
	///          the command is executed unless an event handler has handled it, the <see cref="Executed"/> event is raised and the bound
	///          items are updated.</remarks>
	public virtual void Execute(EditCommand command)
	{
		Control? control = ActiveControl;
		EditCommandExecuteEventArgs e;

		if(!CanExecute(command)) { return; }

		e = new EditCommandExecuteEventArgs(command, control);
		_OnExecuting(e);

		if(!e.Handled) { _ExecuteDefault(control, command); }

		_OnExecuted(new EditCommandEventArgs(command, control));
		UpdateItems();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IExtenderProvider                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual bool CanExtend(object extendee)
	{
		return ((extendee is ToolStripItem) || ((extendee is Control) && (extendee is not Form)));
	}
}
