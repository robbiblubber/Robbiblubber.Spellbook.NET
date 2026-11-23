namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text;



/// <summary>This class implements a component that saves and restores the layout of a form or user control.</summary>
/// <remarks><para>For a form, the component restores the window bounds and state when the form is loaded and saves them when it is
///          closed. Windows whose saved position is no longer visible (for example because a monitor has been disconnected) are moved
///          onto the nearest screen; sizes are adjusted to the DPI of the monitor.</para>
///          <para>For split containers, list views, data grid views and tab controls with <c>SaveLayout</c> set to <c>true</c>, the
///          splitter position, the column widths, order and visibility, or the selected tab are saved as well. This also works with a
///          user control as container; its layout is restored when it is loaded and saved when its form is closed or the user control
///          is disposed.</para>
///          <para>The layouts are kept in a <see cref="IWindowLayoutStore"/>, by default a <see cref="DdnWindowLayoutStore"/> for
///          <see cref="ConfigFile"/>.</para></remarks>
[ProvideProperty("SaveLayout", typeof(Control))]
[ToolboxBitmap(typeof(WindowLayoutProvider), "Resources.windowlayout.bmp")]
[SupportedOSPlatform("windows")]
public class WindowLayoutProvider: Component, IExtenderProvider, IComponent, IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Key of the window bounds and state.</summary>
	public const string WINDOW_KEY = "window";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>DPI that logical sizes refer to.</summary>
	private const int _LOGICAL_DPI = 96;

	/// <summary>Scale of splitter ratios.</summary>
	private const int _RATIO_SCALE = 100000;

	/// <summary>Height of the window strip that has to be visible on a screen.</summary>
	private const int _CAPTION_HEIGHT = 30;

	/// <summary>Minimum width of the visible part of the window strip.</summary>
	private const int _MIN_VISIBLE_WIDTH = 100;

	/// <summary>Splitter mode: ratio.</summary>
	private const int _SPLIT_RATIO = 0;

	/// <summary>Splitter mode: fixed size of panel 1.</summary>
	private const int _SPLIT_PANEL1 = 1;

	/// <summary>Splitter mode: fixed size of panel 2.</summary>
	private const int _SPLIT_PANEL2 = 2;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Container control whose layout is saved.</summary>
	private ContainerControl? _ContainerControl = null;

	/// <summary>Hooked container control.</summary>
	private ContainerControl? _HookedRoot = null;

	/// <summary>Hooked form of a user control.</summary>
	private Form? _HookedForm = null;

	/// <summary>Controls whose layout is saved.</summary>
	private readonly HashSet<Control> _Controls = new();

	/// <summary>Layout store, NULL for the default store.</summary>
	private IWindowLayoutStore? _Store = null;

	/// <summary>Default layout store, NULL if it has not been created yet.</summary>
	private DdnWindowLayoutStore? _DefaultStore = null;

	/// <summary>Configuration file.</summary>
	private string? _ConfigFile = null;

	/// <summary>Layouts to apply again when the handle of a control is created.</summary>
	private readonly Dictionary<Control, int[]> _PendingLayouts = new();

	/// <summary>Determines if the layout has been saved since it has been restored.</summary>
	private bool _Saved = false;

	/// <summary>Determines if the window bounds are checked again when the form is shown.</summary>
	private bool _CheckOnShown = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public WindowLayoutProvider(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public WindowLayoutProvider(IContainer container): this()
	{
		container.Add(this);
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="containerControl">Form or user control whose layout is saved.</param>
	public WindowLayoutProvider(ContainerControl containerControl): this()
	{
		ContainerControl = containerControl;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a key that identifies the current screen configuration.</summary>
	/// <returns>Returns the key.</returns>
	public static string GetScreenConfigurationKey()
	{
		return _GetScreenConfigurationKey(Screen.AllScreens.Select(m => m.Bounds));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a key that identifies a screen configuration.</summary>
	/// <param name="screens">Bounds of the screens.</param>
	/// <returns>Returns an eight-digit hexadecimal key that only depends on the bounds of the screens.</returns>
	internal static string _GetScreenConfigurationKey(IEnumerable<Rectangle> screens)
	{
		StringBuilder text = new();
		uint hash = 2166136261;

		foreach(Rectangle i in screens.OrderBy(m => m.X).ThenBy(m => m.Y)) { text.Append($"{i.X},{i.Y},{i.Width},{i.Height};"); }
		foreach(char i in text.ToString())
		{
			hash ^= i;
			hash *= 16777619;
		}

		return hash.ToString("x8");
	}


	/// <summary>Moves and shrinks window bounds so that the window is visible.</summary>
	/// <param name="bounds">Window bounds.</param>
	/// <param name="workingAreas">Working areas of the screens.</param>
	/// <returns>Returns the bounds unchanged if the top strip of the window (where the caption is) is visible on a screen; otherwise
	///          the bounds moved onto, and if necessary shrunk to, the working area of the nearest screen.</returns>
	internal static Rectangle _FitToScreen(Rectangle bounds, IReadOnlyList<Rectangle> workingAreas)
	{
		Rectangle caption = new(bounds.X, bounds.Y, bounds.Width, Math.Min(bounds.Height, _CAPTION_HEIGHT));
		Rectangle target;
		int width;
		int height;

		if(workingAreas.Count == 0) { return bounds; }

		foreach(Rectangle i in workingAreas)
		{
			Rectangle visible = Rectangle.Intersect(caption, i);
			if((visible.Width >= Math.Min(_MIN_VISIBLE_WIDTH, bounds.Width)) && (visible.Height >= Math.Min(caption.Height, _CAPTION_HEIGHT / 2))) { return bounds; }
		}

		target = workingAreas.OrderByDescending(m => _Area(Rectangle.Intersect(bounds, m))).ThenBy(m => _Distance(bounds, m)).First();
		width = Math.Min(bounds.Width, target.Width);
		height = Math.Min(bounds.Height, target.Height);

		return new Rectangle(Math.Clamp(bounds.X, target.Left, target.Right - width), Math.Clamp(bounds.Y, target.Top, target.Bottom - height), width, height);
	}


	/// <summary>Converts a value from logical pixels (96 DPI) to device pixels.</summary>
	/// <param name="value">Value.</param>
	/// <param name="dpi">Device DPI.</param>
	/// <returns>Returns the value in device pixels.</returns>
	internal static int _FromLogical(int value, int dpi)
	{
		return (int) Math.Round(value * (double) dpi / _LOGICAL_DPI);
	}


	/// <summary>Converts a value from device pixels to logical pixels (96 DPI).</summary>
	/// <param name="value">Value.</param>
	/// <param name="dpi">Device DPI.</param>
	/// <returns>Returns the value in logical pixels.</returns>
	internal static int _ToLogical(int value, int dpi)
	{
		return (int) Math.Round(value * (double) _LOGICAL_DPI / ((dpi > 0) ? dpi : _LOGICAL_DPI));
	}


	/// <summary>Returns if an array is a permutation of the numbers 0 to n - 1.</summary>
	/// <param name="values">Values.</param>
	/// <returns>Returns <c>true</c> if the values are a permutation, otherwise returns <c>false</c>.</returns>
	internal static bool _IsPermutation(IReadOnlyCollection<int> values)
	{
		return values.OrderBy(m => m).SequenceEqual(Enumerable.Range(0, values.Count));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the area of a rectangle.</summary>
	/// <param name="r">Rectangle.</param>
	/// <returns>Returns the area.</returns>
	private static long _Area(Rectangle r)
	{
		return ((long) r.Width * r.Height);
	}


	/// <summary>Gets the distance between the centers of two rectangles.</summary>
	/// <param name="a">First rectangle.</param>
	/// <param name="b">Second rectangle.</param>
	/// <returns>Returns the squared distance.</returns>
	private static long _Distance(Rectangle a, Rectangle b)
	{
		long dx = (a.X + (a.Width / 2)) - (b.X + (b.Width / 2));
		long dy = (a.Y + (a.Height / 2)) - (b.Y + (b.Height / 2));

		return ((dx * dx) + (dy * dy));
	}


	/// <summary>Gets the nesting depth of a control.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the number of parents.</returns>
	private static int _GetDepth(Control control)
	{
		int rval = 0;

		for(Control? i = control.Parent; i is not null; i = i.Parent) { rval++; }
		return rval;
	}


	/// <summary>Gets the key of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="root">Container control.</param>
	/// <returns>Returns the key, or NULL if the control has no name.</returns>
	/// <remarks>The key consists of the names of the control and its named parents up to the container control. Parents without a
	///          name (for example the panels of a split container) are skipped.</remarks>
	private static string? _GetControlKey(Control control, Control root)
	{
		List<string> names = new();

		if(string.IsNullOrWhiteSpace(control.Name)) { return null; }

		for(Control? i = control; (i is not null) && (i != root); i = i.Parent)
		{
			if(!string.IsNullOrWhiteSpace(i.Name)) { names.Insert(0, i.Name); }
		}

		return ("ctl_" + string.Join("_", names));
	}


	/// <summary>Gets the space available for the panels of a split container.</summary>
	/// <param name="split">Split container.</param>
	/// <returns>Returns the space in pixels.</returns>
	private static int _GetSplitSpace(SplitContainer split)
	{
		return (((split.Orientation == Orientation.Vertical) ? split.Width : split.Height) - split.SplitterWidth);
	}


	/// <summary>Gets the layout of a split container.</summary>
	/// <param name="split">Split container.</param>
	/// <returns>Returns mode and value.</returns>
	private static int[]? _GetSplitLayout(SplitContainer split)
	{
		int space = _GetSplitSpace(split);

		if(space <= 0) { return null; }

		return split.FixedPanel switch
		{
			FixedPanel.Panel1 => [_SPLIT_PANEL1, _ToLogical(split.SplitterDistance, split.DeviceDpi)],
			FixedPanel.Panel2 => [_SPLIT_PANEL2, _ToLogical(space - split.SplitterDistance, split.DeviceDpi)],
			_ => [_SPLIT_RATIO, (int) ((long) split.SplitterDistance * _RATIO_SCALE / space)]
		};
	}


	/// <summary>Restores the layout of a split container.</summary>
	/// <param name="split">Split container.</param>
	/// <param name="value">Saved layout.</param>
	private static void _SetSplitLayout(SplitContainer split, int[] value)
	{
		int space = _GetSplitSpace(split);
		int distance;

		if((value.Length != 2) || (space <= 0)) { return; }

		distance = value[0] switch
		{
			_SPLIT_PANEL1 => _FromLogical(value[1], split.DeviceDpi),
			_SPLIT_PANEL2 => space - _FromLogical(value[1], split.DeviceDpi),
			_ => (int) ((long) value[1] * space / _RATIO_SCALE)
		};
		distance = Math.Max(split.Panel1MinSize, Math.Min(distance, space - split.Panel2MinSize));

		try
		{
			if(distance > 0) { split.SplitterDistance = distance; }
		}
		catch(Exception ex) when ((ex is ArgumentException) || (ex is InvalidOperationException)) {}
	}


	/// <summary>Gets the layout of a list view.</summary>
	/// <param name="list">List view.</param>
	/// <returns>Returns the number of columns, their widths and display indices.</returns>
	private static int[] _GetListLayout(ListView list)
	{
		List<int> rval = new() { list.Columns.Count };

		rval.AddRange(list.Columns.Cast<ColumnHeader>().Select(m => _ToLogical(m.Width, list.DeviceDpi)));
		rval.AddRange(_GetColumnOrder(list));

		return rval.ToArray();
	}


	/// <summary>Gets the display indices of the columns of a list view.</summary>
	/// <param name="list">List view.</param>
	/// <returns>Returns the display index of each column.</returns>
	/// <remarks>If the list view has a handle, the order is read from the native control, because it is the reliable source after the
	///          user has dragged columns.</remarks>
	private static int[] _GetColumnOrder(ListView list)
	{
		int n = list.Columns.Count;
		int[] order = new int[n];
		int[] rval = new int[n];

		if((!list.IsHandleCreated) || (n == 0) || (_NativeMethods._SendMessage(list.Handle, _NativeMethods._LVM_GETCOLUMNORDERARRAY, (IntPtr) n, order) == IntPtr.Zero) || (!_IsPermutation(order)))
		{
			return list.Columns.Cast<ColumnHeader>().Select(m => m.DisplayIndex).ToArray();
		}

		for(int i = 0; i < n; i++) { rval[order[i]] = i; }
		return rval;
	}


	/// <summary>Restores the layout of a list view.</summary>
	/// <param name="list">List view.</param>
	/// <param name="value">Saved layout.</param>
	private static void _SetListLayout(ListView list, int[] value)
	{
		int n = list.Columns.Count;

		if((value.Length != ((2 * n) + 1)) || (value[0] != n) || (!_IsPermutation(value.Skip(n + 1).ToArray()))) { return; }

		for(int i = 0; i < n; i++) { list.Columns[i].Width = Math.Max(0, _FromLogical(value[i + 1], list.DeviceDpi)); }
		foreach(int i in Enumerable.Range(0, n).OrderBy(m => value[n + 1 + m])) { list.Columns[i].DisplayIndex = value[n + 1 + i]; }
	}


	/// <summary>Gets the layout of a data grid view.</summary>
	/// <param name="grid">Data grid view.</param>
	/// <returns>Returns the number of columns, their widths (-1 for automatically sized columns), display indices and visibility.</returns>
	private static int[] _GetGridLayout(DataGridView grid)
	{
		List<DataGridViewColumn> columns = grid.Columns.Cast<DataGridViewColumn>().ToList();
		List<int> rval = new() { columns.Count };

		rval.AddRange(columns.Select(m => ((m.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.None) ? _ToLogical(m.Width, grid.DeviceDpi) : -1)));
		rval.AddRange(columns.Select(m => m.DisplayIndex));
		rval.AddRange(columns.Select(m => (m.Visible ? 1 : 0)));

		return rval.ToArray();
	}


	/// <summary>Restores the layout of a data grid view.</summary>
	/// <param name="grid">Data grid view.</param>
	/// <param name="value">Saved layout.</param>
	private static void _SetGridLayout(DataGridView grid, int[] value)
	{
		int n = grid.Columns.Count;

		if((value.Length != ((3 * n) + 1)) || (value[0] != n) || (!_IsPermutation(value.Skip(n + 1).Take(n).ToArray()))) { return; }

		for(int i = 0; i < n; i++)
		{
			DataGridViewColumn column = grid.Columns[i];

			if((value[i + 1] >= 0) && (column.InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.None))
			{
				column.Width = Math.Max(column.MinimumWidth, _FromLogical(value[i + 1], grid.DeviceDpi));
			}
			column.Visible = (value[(2 * n) + 1 + i] != 0);
		}

		foreach(int i in Enumerable.Range(0, n).OrderBy(m => value[n + 1 + m])) { grid.Columns[i].DisplayIndex = value[n + 1 + i]; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the effective layout name.</summary>
	/// <param name="root">Container control.</param>
	/// <returns>Returns the layout name.</returns>
	private string _GetLayoutName(ContainerControl root)
	{
		string rval = (string.IsNullOrWhiteSpace(LayoutName) ? (string.IsNullOrWhiteSpace(root.Name) ? root.GetType().Name : root.Name) : LayoutName);

		if(PerScreenConfiguration) { rval += "_" + GetScreenConfigurationKey(); }
		return rval;
	}


	/// <summary>Restores the window bounds and state of a form.</summary>
	/// <param name="form">Form.</param>
	/// <param name="layout">Layout name.</param>
	private void _RestoreWindow(Form form, string layout)
	{
		int[]? value = Store.Read(layout, WINDOW_KEY);
		Rectangle[] areas = Screen.AllScreens.Select(m => m.WorkingArea).ToArray();
		bool sizable = ((form.FormBorderStyle == FormBorderStyle.Sizable) || (form.FormBorderStyle == FormBorderStyle.SizableToolWindow));
		Rectangle bounds;
		FormWindowState state;

		if((value is null) || (value.Length < 6)) { return; }

		bounds = new Rectangle(value[0], value[1], (sizable ? value[2] : form.Width), (sizable ? value[3] : form.Height));
		bounds = _FitToScreen(bounds, areas);

		form.StartPosition = FormStartPosition.Manual;
		form.Location = bounds.Location;
		if(sizable)
		{
			form.Size = ((value[4] > 0) ? new Size((int) Math.Round(bounds.Width * (double) form.DeviceDpi / value[4]), (int) Math.Round(bounds.Height * (double) form.DeviceDpi / value[4])) : bounds.Size);
		}
		form.Bounds = _FitToScreen(form.Bounds, areas);
		_CheckOnShown = true;

		state = (FormWindowState) value[5];
		if(((state == FormWindowState.Maximized) && RestoreMaximized) || ((state == FormWindowState.Minimized) && RestoreMinimized)) { form.WindowState = state; }
	}


	/// <summary>Saves the window bounds and state of a form.</summary>
	/// <param name="form">Form.</param>
	/// <param name="layout">Layout name.</param>
	private void _SaveWindow(Form form, string layout)
	{
		Rectangle bounds = ((form.WindowState == FormWindowState.Normal) ? form.Bounds : form.RestoreBounds);

		if((bounds.Width <= 0) || (bounds.Height <= 0)) { return; }
		Store.Write(layout, WINDOW_KEY, [bounds.X, bounds.Y, bounds.Width, bounds.Height, form.DeviceDpi, (int) form.WindowState]);
	}


	/// <summary>Applies a column layout again when the handle of the control is created.</summary>
	/// <param name="control">List view or data grid view.</param>
	/// <param name="value">Saved layout.</param>
	/// <remarks>Controls on hidden tab pages get their handle only when they are shown. Creating the handle can reset the column
	///          order, so the layout is applied again then.</remarks>
	private void _ReapplyOnHandleCreated(Control control, int[] value)
	{
		if(control.IsHandleCreated) { return; }

		if(!_PendingLayouts.ContainsKey(control)) { control.HandleCreated += _Control_HandleCreated; }
		_PendingLayouts[control] = value;
	}


	/// <summary>Hooks the events of the container control.</summary>
	/// <param name="root">Container control, NULL to unhook only.</param>
	private void _Hook(ContainerControl? root)
	{
		if(_HookedRoot is Form oldForm)
		{
			oldForm.Load -= _Root_Load;
			oldForm.Shown -= _Form_Shown;
			oldForm.FormClosed -= _Form_FormClosed;
		}
		else if(_HookedRoot is UserControl oldUser) { oldUser.Load -= _Root_Load; }
		_HookParentForm(null);

		_HookedRoot = null;
		if((root is null) || DesignMode) { return; }

		if(root is Form form)
		{
			form.Load += _Root_Load;
			form.Shown += _Form_Shown;
			form.FormClosed += _Form_FormClosed;
		}
		else if(root is UserControl user) { user.Load += _Root_Load; }

		_HookedRoot = root;
	}


	/// <summary>Hooks the FormClosed event of the form of a user control.</summary>
	/// <param name="form">Form, NULL to unhook only.</param>
	private void _HookParentForm(Form? form)
	{
		if(_HookedForm is not null) { _HookedForm.FormClosed -= _Form_FormClosed; }

		_HookedForm = form;
		if(_HookedForm is not null) { _HookedForm.FormClosed += _Form_FormClosed; }
	}


	/// <summary>Restores the layout automatically.</summary>
	/// <remarks>The automatic restore runs in the Load event; a layout file that cannot be read must not prevent the form or user
	///          control from loading, so I/O errors are ignored. <see cref="Restore"/> itself throws them.</remarks>
	private void _AutoRestore()
	{
		try
		{
			Restore();
		}
		catch(IOException) {}
		catch(UnauthorizedAccessException) {}
	}


	/// <summary>Saves the layout automatically.</summary>
	/// <remarks>The automatic save runs when the form is closed or the component is disposed; a layout file that cannot be written
	///          must not fail closing or disposing, so I/O errors are ignored. <see cref="Save"/> itself throws them.</remarks>
	private void _AutoSave()
	{
		try
		{
			Save();
		}
		catch(IOException) {}
		catch(UnauthorizedAccessException) {}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="Restored"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnRestored(EventArgs e)
	{
		Restored?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Saving"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSaving(EventArgs e)
	{
		Saving?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs after the layout has been restored.</summary>
	[Category("Layout")][Description("Occurs after the layout has been restored.")]
	public event EventHandler? Restored;

	/// <summary>Occurs before the layout is saved.</summary>
	/// <remarks>Event handlers can write additional values to <see cref="Store"/>; they are committed together with the layout.</remarks>
	[Category("Layout")][Description("Occurs before the layout is saved.")]
	public event EventHandler? Saving;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the form or user control whose layout is saved.</summary>
	/// <remarks>In the designer, this property is set to the form or user control automatically.</remarks>
	[Category("Layout")][Description("Form or user control whose layout is saved.")][DefaultValue(null)]
	public ContainerControl? ContainerControl
	{
		get { return _ContainerControl; }
		set
		{
			_ContainerControl = value;
			_Hook(value);
		}
	}


	/// <summary>Gets or sets the layout name.</summary>
	/// <remarks>If this property is NULL or empty, the name of the container control is used. Names consist of letters, digits,
	///          underscores and hyphens.</remarks>
	[Category("Layout")][Description("Layout name. If empty, the name of the form or user control is used.")][DefaultValue(null)]
	public string? LayoutName
	{
		get; set;
	} = null;


	/// <summary>Gets or sets the configuration file of the default store.</summary>
	/// <remarks>A relative path is relative to the user configuration path of the application; NULL or empty for
	///          <see cref="DdnWindowLayoutStore.DEFAULT_FILE_NAME"/>. The property has no effect if <see cref="Store"/> has been
	///          set.</remarks>
	[Category("Layout")][Description("Configuration file. If empty, app.layout in the user configuration path is used.")][DefaultValue(null)]
	public string? ConfigFile
	{
		get { return _ConfigFile; }
		set
		{
			_ConfigFile = value;
			_DefaultStore = null;
		}
	}


	/// <summary>Gets or sets the layout store.</summary>
	/// <remarks>Setting this property to NULL restores the default store, a <see cref="DdnWindowLayoutStore"/> for
	///          <see cref="ConfigFile"/>.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[AllowNull]
	public IWindowLayoutStore Store
	{
		get { return (_Store ?? (_DefaultStore ??= new DdnWindowLayoutStore(_ConfigFile))); }
		set { _Store = value; }
	}


	/// <summary>Gets or sets if the layout is restored automatically when the container control is loaded.</summary>
	[Category("Layout")][Description("Specifies if the layout is restored automatically when the form or user control is loaded.")][DefaultValue(true)]
	public bool AutoRestore
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if the layout is saved automatically when the form is closed.</summary>
	[Category("Layout")][Description("Specifies if the layout is saved automatically when the form is closed.")][DefaultValue(true)]
	public bool AutoSave
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if a maximized window is restored maximized.</summary>
	[Category("Layout")][Description("Specifies if a maximized window is restored maximized.")][DefaultValue(true)]
	public bool RestoreMaximized
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if a minimized window is restored minimized.</summary>
	[Category("Layout")][Description("Specifies if a minimized window is restored minimized.")][DefaultValue(false)]
	public bool RestoreMinimized
	{
		get; set;
	} = false;


	/// <summary>Gets or sets if separate layouts are kept for each screen configuration.</summary>
	/// <remarks>If this property is <c>true</c>, a key of the screen configuration (see <see cref="GetScreenConfigurationKey"/>) is
	///          appended to the layout name, so for example a laptop keeps different layouts with and without external monitors.</remarks>
	[Category("Layout")][Description("Specifies if separate layouts are kept for each screen configuration.")][DefaultValue(false)]
	public bool PerScreenConfiguration
	{
		get; set;
	} = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the layout of a control is saved.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns <c>true</c> if the layout is saved, otherwise returns <c>false</c>.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	[Category("Layout")][Description("Specifies if the layout of this control (splitter, columns or selected tab) is saved.")][DefaultValue(false)]
	public virtual bool GetSaveLayout(Control control)
	{
		if(control is null) { throw new ArgumentNullException(nameof(control)); }
		return _Controls.Contains(control);
	}


	/// <summary>Sets if the layout of a control is saved.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Value.</param>
	/// <exception cref="ArgumentNullException">Thrown when the control is NULL.</exception>
	public virtual void SetSaveLayout(Control control, bool value)
	{
		if(control is null) { throw new ArgumentNullException(nameof(control)); }

		if(value)
		{
			if(_Controls.Add(control)) { control.Disposed += _Control_Disposed; }
		}
		else if(_Controls.Remove(control)) { control.Disposed -= _Control_Disposed; }
	}


	/// <summary>Restores the layout.</summary>
	/// <remarks>For a form, the window bounds and state are restored first, then the layouts of the controls, outer controls first.
	///          Values that do not match the controls (for example because columns have been added) are ignored.</remarks>
	public virtual void Restore()
	{
		ContainerControl? root = _ContainerControl;
		string layout;

		if(root is null) { return; }
		layout = _GetLayoutName(root);

		if(root is Form form) { _RestoreWindow(form, layout); }

		foreach(Control i in _Controls.OrderBy(m => _GetDepth(m)).ToList())
		{
			string? key = _GetControlKey(i, root);
			int[]? value = ((key is null) ? null : Store.Read(layout, key));

			if(value is null) { continue; }

			switch(i)
			{
				case SplitContainer split:
					_SetSplitLayout(split, value);
					break;
				case ListView list:
					_SetListLayout(list, value);
					_ReapplyOnHandleCreated(list, value);
					break;
				case DataGridView grid:
					_SetGridLayout(grid, value);
					_ReapplyOnHandleCreated(grid, value);
					break;
				case TabControl tabs:
					if((value.Length == 1) && (value[0] >= 0) && (value[0] < tabs.TabCount)) { tabs.SelectedIndex = value[0]; }
					break;
			}
		}

		_Saved = false;
		_OnRestored(EventArgs.Empty);
	}


	/// <summary>Saves the layout.</summary>
	public virtual void Save()
	{
		ContainerControl? root = _ContainerControl;
		string layout;

		if(root is null) { return; }
		layout = _GetLayoutName(root);

		if(root is Form form) { _SaveWindow(form, layout); }

		foreach(Control i in _Controls)
		{
			string? key = _GetControlKey(i, root);
			int[]? value = i switch
			{
				SplitContainer split => _GetSplitLayout(split),
				ListView list => _GetListLayout(list),
				DataGridView grid => _GetGridLayout(grid),
				TabControl tabs => [tabs.SelectedIndex],
				_ => null
			};

			if((key is not null) && (value is not null)) { Store.Write(layout, key, value); }
		}

		_OnSaving(EventArgs.Empty);
		Store.Commit();
		_Saved = true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Load event of the container control.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Root_Load(object? sender, EventArgs e)
	{
		if(_HookedRoot is UserControl user) { _HookParentForm(user.ParentForm); }
		if(AutoRestore) { _AutoRestore(); }
	}


	/// <summary>Handles the Shown event of the form.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	/// <remarks>The bounds are checked again, because the form may have been scaled to the DPI of its monitor when it was shown.</remarks>
	private void _Form_Shown(object? sender, EventArgs e)
	{
		if((!_CheckOnShown) || (sender is not Form form) || (form.WindowState != FormWindowState.Normal)) { return; }

		_CheckOnShown = false;
		form.Bounds = _FitToScreen(form.Bounds, Screen.AllScreens.Select(m => m.WorkingArea).ToArray());
	}


	/// <summary>Handles the FormClosed event of the form.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Form_FormClosed(object? sender, FormClosedEventArgs e)
	{
		if(AutoSave) { _AutoSave(); }
	}


	/// <summary>Handles the HandleCreated event of controls whose layout has been restored before they had a handle.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_HandleCreated(object? sender, EventArgs e)
	{
		if(sender is not Control control) { return; }

		control.HandleCreated -= _Control_HandleCreated;
		if(!_PendingLayouts.Remove(control, out int[]? value)) { return; }

		if(control is ListView list)
		{
			_SetListLayout(list, value);
		}
		else if(control is DataGridView grid) { _SetGridLayout(grid, value); }
	}


	/// <summary>Handles the Disposed event of controls whose layout is saved.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Control_Disposed(object? sender, EventArgs e)
	{
		if(sender is not Control control) { return; }

		if(_PendingLayouts.Remove(control)) { control.HandleCreated -= _Control_HandleCreated; }
		SetSaveLayout(control, false);
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
	/// <remarks>The layout of a user control that is disposed before its form has been closed is saved here.</remarks>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(AutoSave && (!_Saved) && (_HookedRoot is UserControl) && (!DesignMode)) { _AutoSave(); }

			_Hook(null);
			foreach(Control i in _PendingLayouts.Keys) { i.HandleCreated -= _Control_HandleCreated; }
			foreach(Control i in _Controls) { i.Disposed -= _Control_Disposed; }
			_PendingLayouts.Clear();
			_Controls.Clear();
		}

		base.Dispose(disposing);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IExtenderProvider                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public virtual bool CanExtend(object extendee)
	{
		return ((extendee is SplitContainer) || (extendee is ListView) || (extendee is DataGridView) || (extendee is TabControl));
	}
}
