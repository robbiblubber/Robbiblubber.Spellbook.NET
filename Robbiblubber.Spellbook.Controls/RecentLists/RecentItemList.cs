namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements a component that keeps a list of recently used entries and shows it in a menu.</summary>
/// <remarks><para>Entries are identified by their key (for example a file name or a connection ID). The most recently used entry comes
///          first; the list keeps at most <see cref="MaxCount"/> entries. Pinned entries are shown above the others, in the order they
///          have been pinned; they do not count towards <see cref="MaxCount"/> and never drop out of the list.</para>
///          <para>If <see cref="MenuItem"/> is set, the entries are shown in its drop-down or in its place (see
///          <see cref="MenuStyle"/>). Clicking an entry raises <see cref="ItemSelected"/>; right-clicking it shows a context menu to
///          open, pin, unpin or remove the entry.</para>
///          <para>The list is loaded from the store before every change and saved right after, so several instances of an
///          application can share it. Derived classes adapt the list to a kind of entries by overriding <see cref="KeyComparer"/>,
///          <see cref="_Normalize"/>, <see cref="_GetText"/>, <see cref="_GetToolTip"/>, <see cref="_GetImage"/> and
///          <see cref="_IsAvailable"/>;
///          <see cref="RecentFileList"/> is the implementation for files and folders.</para>
///          <para>Pinned entries show the pin over their image. In themed menus, the image and the pin are resolved for the theme on
///          their own (see <see cref="Themes.ThemeManager.ResolveImage"/>), so a dark variant of the image is shown below the pin as
///          well.</para></remarks>
[ToolboxBitmap(typeof(RecentItemList), "Resources.recentlist.bmp")]
[DefaultEvent("ItemSelected")]
[SupportedOSPlatform("windows")]
public class RecentItemList: Component, IComponent, IDisposable
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default list name.</summary>
	public const string DEFAULT_LIST_NAME = "recent";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Pin image, NULL if it has not been loaded yet.</summary>
	private static Image? _PinImage = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Entries, NULL if they have not been loaded yet.</summary>
	private List<RecentItem>? _Items = null;

	/// <summary>Layout store, NULL for the default store.</summary>
	private IRecentListStore? _Store = null;

	/// <summary>Default store, NULL if it has not been created yet.</summary>
	private DdnRecentListStore? _DefaultStore = null;

	/// <summary>Configuration file.</summary>
	private string? _ConfigFile = null;

	/// <summary>List name.</summary>
	private string _ListName;

	/// <summary>Maximum number of entries that are not pinned.</summary>
	private int _MaxCount = 10;

	/// <summary>Menu item.</summary>
	private ToolStripMenuItem? _MenuItem = null;

	/// <summary>Menu style.</summary>
	private RecentListMenuStyle _MenuStyle = RecentListMenuStyle.SUBMENU;

	/// <summary>Drop-down whose Opening event is hooked for the inline style.</summary>
	private ToolStripDropDown? _HookedOwner = null;

	/// <summary>Menu items created by the list.</summary>
	private readonly List<ToolStripItem> _Generated = new();

	/// <summary>Images created for the menu items (pinned entries with an image).</summary>
	private readonly List<Image> _GeneratedImages = new();

	/// <summary>Context menu, NULL if it has not been created yet.</summary>
	private ContextMenuStrip? _Context = null;

	/// <summary>Menu item the context menu has been opened for.</summary>
	private ToolStripMenuItem? _ContextSource = null;

	/// <summary>Drop-downs whose automatic closing has been suspended while the context menu is open.</summary>
	private readonly List<ToolStripDropDown> _SuspendedDropDowns = new();

	/// <summary>Determines if the menu shows images for the entries.</summary>
	private bool _ShowImages = true;

	/// <summary>Determines if the menu item is disabled while the list is empty.</summary>
	private bool _DisableWhenEmpty = true;

	/// <summary>Determines if the next click on an entry is ignored, because it has opened the context menu.</summary>
	private bool _IgnoreClick = false;

	/// <summary>Determines if the Click event of a menu item created by the list is being handled.</summary>
	/// <remarks>While it is set, the menu is not rebuilt, because that would dispose the clicked menu item (see
	///          <see cref="_RebuildMenu"/>).</remarks>
	private bool _InClick = false;

	/// <summary>Determines if a rebuild of the menu has been deferred because a click was being handled.</summary>
	private bool _RebuildPending = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public RecentItemList(): base()
	{
		_ListName = _DefaultListName;
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public RecentItemList(IContainer container): this()
	{
		container.Add(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the index of an entry.</summary>
	/// <param name="items">Entries.</param>
	/// <param name="key">Normalized key.</param>
	/// <param name="comparer">Key comparer.</param>
	/// <returns>Returns the index, or -1 if the list does not contain the key.</returns>
	internal static int _IndexOf(List<RecentItem> items, string key, IEqualityComparer<string> comparer)
	{
		return items.FindIndex(m => comparer.Equals(m.Key, key));
	}


	/// <summary>Puts pinned entries first, keeping the order within pinned and unpinned entries.</summary>
	/// <param name="items">Entries.</param>
	/// <returns>Returns the ordered entries.</returns>
	internal static List<RecentItem> _Order(IEnumerable<RecentItem> items)
	{
		List<RecentItem> all = items.ToList();
		return all.Where(m => m.Pinned).Concat(all.Where(m => !m.Pinned)).ToList();
	}


	/// <summary>Removes unpinned entries beyond a maximum number.</summary>
	/// <param name="items">Entries, pinned entries first.</param>
	/// <param name="maxCount">Maximum number of unpinned entries.</param>
	/// <returns>Returns <c>true</c> if entries have been removed, otherwise returns <c>false</c>.</returns>
	internal static bool _Trim(List<RecentItem> items, int maxCount)
	{
		int pinned = items.Count(m => m.Pinned);
		int excess = items.Count - pinned - maxCount;

		if(excess <= 0) { return false; }

		items.RemoveRange(items.Count - excess, excess);
		return true;
	}


	/// <summary>Adds an entry or moves it to the top of the unpinned entries.</summary>
	/// <param name="items">Entries, pinned entries first.</param>
	/// <param name="key">Normalized key.</param>
	/// <param name="text">Display text, NULL to keep the text of an existing entry.</param>
	/// <param name="now">Current time.</param>
	/// <param name="maxCount">Maximum number of unpinned entries.</param>
	/// <param name="comparer">Key comparer.</param>
	/// <remarks>A pinned entry keeps its position; only its time and text are updated.</remarks>
	internal static void _Add(List<RecentItem> items, string key, string? text, DateTime now, int maxCount, IEqualityComparer<string> comparer)
	{
		int index = _IndexOf(items, key, comparer);
		RecentItem? existing = ((index < 0) ? null : items[index]);

		if((existing is not null) && existing.Pinned)
		{
			items[index] = new RecentItem(key, (text ?? existing.Text), true, now);
			return;
		}

		if(index >= 0) { items.RemoveAt(index); }
		items.Insert(items.Count(m => m.Pinned), new RecentItem(key, (text ?? existing?.Text), false, now));
		_Trim(items, maxCount);
	}


	/// <summary>Pins an entry; it is moved to the end of the pinned entries.</summary>
	/// <param name="items">Entries, pinned entries first.</param>
	/// <param name="key">Normalized key.</param>
	/// <param name="comparer">Key comparer.</param>
	/// <returns>Returns <c>true</c> if the entry has been pinned, otherwise returns <c>false</c>.</returns>
	internal static bool _Pin(List<RecentItem> items, string key, IEqualityComparer<string> comparer)
	{
		int index = _IndexOf(items, key, comparer);
		RecentItem item;

		if((index < 0) || items[index].Pinned) { return false; }

		item = items[index];
		items.RemoveAt(index);
		items.Insert(items.Count(m => m.Pinned), new RecentItem(item.Key, item.Text, true, item.LastUsed));

		return true;
	}


	/// <summary>Unpins an entry; it is moved to the top of the unpinned entries.</summary>
	/// <param name="items">Entries, pinned entries first.</param>
	/// <param name="key">Normalized key.</param>
	/// <param name="maxCount">Maximum number of unpinned entries.</param>
	/// <param name="comparer">Key comparer.</param>
	/// <returns>Returns <c>true</c> if the entry has been unpinned, otherwise returns <c>false</c>.</returns>
	internal static bool _Unpin(List<RecentItem> items, string key, int maxCount, IEqualityComparer<string> comparer)
	{
		int index = _IndexOf(items, key, comparer);
		RecentItem item;

		if((index < 0) || (!items[index].Pinned)) { return false; }

		item = items[index];
		items.RemoveAt(index);
		items.Insert(items.Count(m => m.Pinned), new RecentItem(item.Key, item.Text, false, item.LastUsed));
		_Trim(items, maxCount);

		return true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the pin image.</summary>
	/// <returns>Returns the image.</returns>
	private static Image _GetPinImage()
	{
		return (_PinImage ??= new Bitmap(typeof(RecentItemList), "Resources.pin.png"));
	}


	/// <summary>Escapes ampersands, so a text is shown literally in a menu.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns the escaped text.</returns>
	private static string _Escape(string text)
	{
		return text.Replace("&", "&&");
	}


	/// <summary>Gets the drop-downs that contain a menu item, innermost first.</summary>
	/// <param name="item">Menu item.</param>
	/// <returns>Returns the drop-downs.</returns>
	private static List<ToolStripDropDown> _GetDropDowns(ToolStripItem item)
	{
		List<ToolStripDropDown> rval = new();

		for(ToolStripDropDown? i = item.Owner as ToolStripDropDown; i is not null; i = i.OwnerItem?.Owner as ToolStripDropDown) { rval.Add(i); }
		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Changes the list.</summary>
	/// <param name="change">Change; returns <c>true</c> if the list has been changed.</param>
	/// <remarks>The list is loaded from the store, changed, saved and shown again.</remarks>
	private void _Modify(Func<List<RecentItem>, bool> change)
	{
		List<RecentItem> items = _Order(Store.Load(_ListName));
		bool changed = change(items);

		if(changed) { Store.Save(_ListName, items); }
		_Items = items;

		_RebuildMenu();
		if(changed) { _OnChanged(EventArgs.Empty); }
	}


	/// <summary>Hooks the menu events.</summary>
	/// <param name="hook">Determines if the events are hooked; <c>false</c> unhooks them.</param>
	private void _HookMenu(bool hook)
	{
		if(_MenuItem is null) { return; }

		_MenuItem.DropDownOpening -= _MenuItem_DropDownOpening;
		_MenuItem.OwnerChanged -= _MenuItem_OwnerChanged;
		_HookOwner(null);

		if((!hook) || DesignMode) { return; }

		if(_MenuStyle == RecentListMenuStyle.SUBMENU) { _MenuItem.DropDownOpening += _MenuItem_DropDownOpening; }

		_MenuItem.OwnerChanged += _MenuItem_OwnerChanged;
		_HookOwner(_MenuItem.Owner as ToolStripDropDown);
	}


	/// <summary>Hooks the Opening event of the drop-down that contains the menu item.</summary>
	/// <remarks>For the inline style, the entries are created then; for the submenu style, the state of the menu item is updated.</remarks>
	/// <param name="owner">Drop-down, NULL to unhook only.</param>
	private void _HookOwner(ToolStripDropDown? owner)
	{
		if(_HookedOwner is not null) { _HookedOwner.Opening -= _Owner_Opening; }

		_HookedOwner = owner;
		if(_HookedOwner is not null) { _HookedOwner.Opening += _Owner_Opening; }
	}


	/// <summary>Restores the visibility and state of the menu item when it is no longer used by the list.</summary>
	private void _RestoreMenuItem()
	{
		if(_MenuItem is null) { return; }

		_MenuItem.Available = true;
		_MenuItem.Enabled = true;
	}


	/// <summary>Removes the menu items created by the list.</summary>
	private void _ClearMenu()
	{
		foreach(ToolStripItem i in _Generated)
		{
			i.Owner?.Items.Remove(i);
			i.Image = null;
			i.Dispose();
		}
		_Generated.Clear();

		foreach(Image i in _GeneratedImages)
		{
			_ThemeImages._RemoveOverlay(i);
			i.Dispose();
		}
		_GeneratedImages.Clear();
	}


	/// <summary>Gets the image of a menu item.</summary>
	/// <param name="item">Entry.</param>
	/// <returns>Returns the image of the entry (if <see cref="ShowImages"/> is <c>true</c>), with the pin drawn over it for pinned
	///          entries; the pin alone for pinned entries without image; NULL for other entries without image.</returns>
	private Image? _GetMenuImage(RecentItem item)
	{
		Image? image = (ShowImages ? _GetImage(item) : null);
		Bitmap rval;

		if(!item.Pinned) { return image; }
		if(image is null) { return _GetPinImage(); }

		rval = new Bitmap(image.Width, image.Height);
		using(Graphics g = Graphics.FromImage(rval))
		{
			g.DrawImage(image, 0, 0, image.Width, image.Height);
			g.DrawImage(_GetPinImage(), 0, 0, image.Width, image.Height);
		}
		_GeneratedImages.Add(rval);
		_ThemeImages._RegisterOverlay(rval, image, _GetPinImage());

		return rval;
	}


	/// <summary>Creates the menu items for the entries.</summary>
	/// <returns>Returns the menu items.</returns>
	private List<ToolStripItem> _CreateMenuItems()
	{
		List<ToolStripItem> rval = new();
		IReadOnlyList<RecentItem> items = Items;
		int number = 0;
		bool pinned = false;

		foreach(RecentItem i in items)
		{
			bool? available = _IsAvailable(i);
			string text = _Escape(_GetText(i));
			ToolStripMenuItem menuItem;

			if(pinned && (!i.Pinned)) { rval.Add(new ToolStripSeparator()); }
			pinned = i.Pinned;

			number++;
			if(ShowNumbers && (number <= 10)) { text = ((number == 10) ? "1&0 " : ("&" + number.ToString() + " ")) + text; }

			menuItem = new ToolStripMenuItem(text) { Tag = i, ToolTipText = _GetToolTip(i) };
			menuItem.Image = _GetMenuImage(i);
			if(available == false) { menuItem.ForeColor = SystemColors.GrayText; }

			menuItem.Click += _Entry_Click;
			menuItem.MouseUp += _Entry_MouseUp;
			rval.Add(menuItem);
		}

		if(ShowClearItem && items.Any(m => !m.Pinned))
		{
			ToolStripMenuItem clear = new(ClearText);

			clear.Click += _Clear_Click;
			rval.Add(new ToolStripSeparator());
			rval.Add(clear);
		}

		return rval;
	}


	/// <summary>Shows the entries in the menu.</summary>
	/// <remarks>While a click on a menu item created by the list is handled, the menu items are kept and only the state of the menu
	///          item is updated; the rebuild is deferred until the click has been handled (see <see cref="_HandleClick"/>).</remarks>
	private void _RebuildMenu()
	{
		List<ToolStripItem> items;
		int index;

		if((_MenuItem is null) || DesignMode) { return; }

		if(_InClick)
		{
			_RebuildPending = true;
			if(_MenuStyle == RecentListMenuStyle.SUBMENU) { _MenuItem.Enabled = ((!DisableWhenEmpty) || (Items.Count > 0)); }
			return;
		}
		_RebuildPending = false;

		_ClearMenu();
		items = _CreateMenuItems();

		if(_MenuStyle == RecentListMenuStyle.SUBMENU)
		{
			_MenuItem.Available = true;
			_MenuItem.Enabled = ((!DisableWhenEmpty) || (items.Count > 0));
			if(items.Count == 0) { items.Add(new ToolStripMenuItem(EmptyText) { Enabled = false }); }

			_MenuItem.DropDownItems.AddRange(items.ToArray());
		}
		else
		{
			_MenuItem.Available = false;
			if(_MenuItem.Owner is not ToolStrip owner) { return; }

			index = owner.Items.IndexOf(_MenuItem);
			for(int i = 0; i < items.Count; i++) { owner.Items.Insert(index + 1 + i, items[i]); }
		}

		_Generated.AddRange(items);
	}


	/// <summary>Handles a click on a menu item created by the list.</summary>
	/// <param name="action">Click action.</param>
	/// <remarks>Changes of the list made by the action (for example by an <see cref="ItemSelected"/> handler that adds the selected
	///          entry again) do not rebuild the menu while the clicked menu item is still processing its Click event. The rebuild is
	///          posted to the tool strip of the menu item instead. If it cannot be posted, or the message is lost, the menu is rebuilt
	///          the next time it opens, as the menu is reloaded then anyway.</remarks>
	private void _HandleClick(Action action)
	{
		bool outer = _InClick;

		_InClick = true;
		try
		{
			action();
		}
		finally
		{
			_InClick = outer;
			if((!outer) && _RebuildPending) { _RebuildLater(); }
		}
	}


	/// <summary>Posts a deferred rebuild of the menu to the tool strip of the menu item.</summary>
	private void _RebuildLater()
	{
		ToolStrip? owner = _MenuItem?.Owner;

		if((owner is null) || owner.IsDisposed || (!owner.IsHandleCreated)) { return; }

		try
		{
			owner.BeginInvoke(new Action(_RebuildIfPending));
		}
		catch(InvalidOperationException) {}
	}


	/// <summary>Performs a deferred rebuild of the menu, unless it has been rebuilt in the meantime.</summary>
	private void _RebuildIfPending()
	{
		if(_RebuildPending && (!_InClick)) { _RebuildMenu(); }
	}


	/// <summary>Restores the automatic closing of drop-downs suspended while the context menu was open.</summary>
	private void _ResumeDropDowns()
	{
		foreach(ToolStripDropDown i in _SuspendedDropDowns) { i.AutoClose = true; }
		_SuspendedDropDowns.Clear();
	}


	/// <summary>Selects an entry: raises <see cref="ItemSelected"/> and removes the entry if requested.</summary>
	/// <param name="item">Entry.</param>
	private void _Select(RecentItem item)
	{
		RecentItemEventArgs e = new(item, _IsAvailable(item));

		_OnItemSelected(e);
		if(e.Remove) { Remove(item.Key); }
	}


	/// <summary>Creates the context menu.</summary>
	/// <returns>Returns the context menu.</returns>
	private ContextMenuStrip _GetContext()
	{
		if(_Context is not null) { return _Context; }

		_Context = new ContextMenuStrip();
		_Context.Items.Add(new ToolStripMenuItem(OpenText, null, _ContextOpen_Click) { Name = "open" });
		_Context.Items.Add(new ToolStripSeparator());
		_Context.Items.Add(new ToolStripMenuItem(PinText, null, _ContextPin_Click) { Name = "pin" });
		_Context.Items.Add(new ToolStripMenuItem(RemoveText, null, _ContextRemove_Click) { Name = "remove" });
		_Context.Closed += _Context_Closed;

		return _Context;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected properties                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the list name that is used if <see cref="ListName"/> is not set.</summary>
	protected virtual string _DefaultListName
	{
		get { return DEFAULT_LIST_NAME; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Normalizes a key.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns the normalized key, or NULL if the key is invalid.</returns>
	/// <remarks>The default implementation trims the key.</remarks>
	protected virtual string? _Normalize(string key)
	{
		key = key.Trim();
		return ((key.Length == 0) ? null : key);
	}


	/// <summary>Gets the menu text of an entry.</summary>
	/// <param name="item">Entry.</param>
	/// <returns>Returns the text; the default implementation returns the text of the entry or its key.</returns>
	protected virtual string _GetText(RecentItem item)
	{
		return (item.Text ?? item.Key);
	}


	/// <summary>Gets the tool tip of an entry.</summary>
	/// <param name="item">Entry.</param>
	/// <returns>Returns the tool tip, NULL for none; the default implementation returns the key if the entry has a text.</returns>
	protected virtual string? _GetToolTip(RecentItem item)
	{
		return ((item.Text is null) ? null : item.Key);
	}


	/// <summary>Gets the image of an entry.</summary>
	/// <param name="item">Entry.</param>
	/// <returns>Returns the image, NULL for none; the default implementation returns NULL.</returns>
	/// <remarks>The list does not dispose the image, so implementations can cache images. For pinned entries, the list draws the pin
	///          over a copy of the image. This method is called when the menu opens, so it must be fast.</remarks>
	protected virtual Image? _GetImage(RecentItem item)
	{
		return null;
	}


	/// <summary>Returns if an entry is available.</summary>
	/// <param name="item">Entry.</param>
	/// <returns>Returns <c>true</c> if the entry is available, <c>false</c> if it is not (it is shown grayed), NULL if this is
	///          unknown. The default implementation returns NULL.</returns>
	/// <remarks>This method is called when the menu opens, so it must be fast.</remarks>
	protected virtual bool? _IsAvailable(RecentItem item)
	{
		return null;
	}


	/// <summary>Called after an entry has been added.</summary>
	/// <param name="item">Entry.</param>
	protected virtual void _OnAdded(RecentItem item)
	{}


	/// <summary>Raises the <see cref="ItemSelected"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnItemSelected(RecentItemEventArgs e)
	{
		ItemSelected?.Invoke(this, e);
	}


	/// <summary>Raises the <see cref="Changed"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnChanged(EventArgs e)
	{
		Changed?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when an entry has been selected in the menu.</summary>
	[Category("Recent List")][Description("Occurs when an entry has been selected in the menu.")]
	public event EventHandler<RecentItemEventArgs>? ItemSelected;

	/// <summary>Occurs when the list has been changed.</summary>
	[Category("Recent List")][Description("Occurs when the list has been changed.")]
	public event EventHandler? Changed;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the comparer for keys.</summary>
	/// <remarks>The default comparer is case-sensitive.</remarks>
	[Browsable(false)]
	public virtual IEqualityComparer<string> KeyComparer
	{
		get { return StringComparer.Ordinal; }
	}


	/// <summary>Gets the entries: pinned entries first in the order they have been pinned, then the other entries, most recent
	///          first.</summary>
	[Browsable(false)]
	public IReadOnlyList<RecentItem> Items
	{
		get
		{
			if(DesignMode) { return []; }

			_Items ??= _Order(Store.Load(_ListName));
			return _Items.Where(m => m.Pinned).Concat(_Items.Where(m => !m.Pinned).Take(_MaxCount)).ToList();
		}
	}


	/// <summary>Gets or sets the list name.</summary>
	/// <remarks>Several lists with different names can share a store. NULL or empty restores the default name.</remarks>
	[Category("Recent List")][Description("Name of the list in the store.")]
	public string ListName
	{
		get { return _ListName; }
		set
		{
			_ListName = (string.IsNullOrWhiteSpace(value) ? _DefaultListName : value.Trim());
			_Items = null;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets the configuration file of the default store.</summary>
	/// <remarks>A relative path is relative to the user configuration path of the application; NULL or empty for
	///          <see cref="DdnRecentListStore.DEFAULT_FILE_NAME"/>. The property has no effect if <see cref="Store"/> has been
	///          set.</remarks>
	[Category("Recent List")][Description("Configuration file. If empty, recent.lists in the user configuration path is used.")][DefaultValue(null)]
	public string? ConfigFile
	{
		get { return _ConfigFile; }
		set
		{
			_ConfigFile = value;
			_DefaultStore = null;
			_Items = null;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets the store.</summary>
	/// <remarks>Setting this property to NULL restores the default store, a <see cref="DdnRecentListStore"/> for
	///          <see cref="ConfigFile"/>.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[AllowNull]
	public IRecentListStore Store
	{
		get { return (_Store ?? (_DefaultStore ??= new DdnRecentListStore(_ConfigFile))); }
		set
		{
			_Store = value;
			_Items = null;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets the maximum number of entries that are not pinned.</summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
	[Category("Recent List")][Description("Maximum number of entries that are not pinned.")][DefaultValue(10)]
	public int MaxCount
	{
		get { return _MaxCount; }
		set
		{
			if(value < 1) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be at least 1."); }

			_MaxCount = value;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets the menu item that shows the entries.</summary>
	[Category("Recent List")][Description("Menu item that shows the entries, in its drop-down or in its place (see MenuStyle).")][DefaultValue(null)]
	public ToolStripMenuItem? MenuItem
	{
		get { return _MenuItem; }
		set
		{
			_ClearMenu();
			_HookMenu(false);
			_RestoreMenuItem();

			_MenuItem = value;
			_HookMenu(true);
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets how the entries are shown in the menu.</summary>
	[Category("Recent List")][Description("Specifies if the entries are shown in the drop-down of the menu item or in its place.")][DefaultValue(RecentListMenuStyle.SUBMENU)]
	public RecentListMenuStyle MenuStyle
	{
		get { return _MenuStyle; }
		set
		{
			_ClearMenu();
			_HookMenu(false);
			_RestoreMenuItem();

			_MenuStyle = value;
			_HookMenu(true);
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets if the menu item is disabled while the list is empty (submenu style).</summary>
	/// <remarks>If this property is <c>false</c>, the drop-down of an empty list shows <see cref="EmptyText"/>. The list sets the
	///          <c>Enabled</c> property of the menu item; it is updated when the menu containing the item opens.</remarks>
	[Category("Recent List")][Description("Specifies if the menu item is disabled while the list is empty (submenu style).")][DefaultValue(true)]
	public bool DisableWhenEmpty
	{
		get { return _DisableWhenEmpty; }
		set
		{
			_DisableWhenEmpty = value;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets if the menu shows images for the entries (for example file icons).</summary>
	/// <remarks>Pinned entries always show the pin; if the entry has an image, the pin is drawn over it.</remarks>
	[Category("Recent List")][Description("Specifies if the menu shows images for the entries (for example file icons).")][DefaultValue(true)]
	public bool ShowImages
	{
		get { return _ShowImages; }
		set
		{
			_ShowImages = value;
			_RebuildMenu();
		}
	}


	/// <summary>Gets or sets if the entries are numbered (with mnemonics 1 to 9 and 0).</summary>
	[Category("Recent List")][Description("Specifies if the entries are numbered.")][DefaultValue(true)]
	public bool ShowNumbers
	{
		get; set;
	} = true;


	/// <summary>Gets or sets if the menu shows an item that clears the list.</summary>
	/// <remarks>The item removes the entries that are not pinned.</remarks>
	[Category("Recent List")][Description("Specifies if the menu shows an item that clears the list (pinned entries are kept).")][DefaultValue(true)]
	public bool ShowClearItem
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the text of the menu item that clears the list.</summary>
	[Category("Recent List Texts")][Description("Text of the menu item that clears the list.")][DefaultValue("&Clear List")]
	public string ClearText
	{
		get; set;
	} = "&Clear List";


	/// <summary>Gets or sets the text that is shown when the list is empty (submenu style).</summary>
	[Category("Recent List Texts")][Description("Text that is shown when the list is empty.")][DefaultValue("(Empty)")]
	public string EmptyText
	{
		get; set;
	} = "(Empty)";


	/// <summary>Gets or sets the text of the context menu item that opens an entry.</summary>
	[Category("Recent List Texts")][Description("Text of the context menu item that opens an entry.")][DefaultValue("&Open")]
	public string OpenText
	{
		get; set;
	} = "&Open";


	/// <summary>Gets or sets the text of the context menu item that pins an entry.</summary>
	[Category("Recent List Texts")][Description("Text of the context menu item that pins an entry.")][DefaultValue("&Pin to List")]
	public string PinText
	{
		get; set;
	} = "&Pin to List";


	/// <summary>Gets or sets the text of the context menu item that unpins an entry.</summary>
	[Category("Recent List Texts")][Description("Text of the context menu item that unpins an entry.")][DefaultValue("&Unpin from List")]
	public string UnpinText
	{
		get; set;
	} = "&Unpin from List";


	/// <summary>Gets or sets the text of the context menu item that removes an entry.</summary>
	[Category("Recent List Texts")][Description("Text of the context menu item that removes an entry.")][DefaultValue("&Remove from List")]
	public string RemoveText
	{
		get; set;
	} = "&Remove from List";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds an entry or moves it to the top of the list.</summary>
	/// <param name="key">Key.</param>
	/// <param name="text">Display text, NULL to keep the text of an existing entry or to create it from the key.</param>
	/// <returns>Returns the entry, or NULL if the key is invalid.</returns>
	public virtual RecentItem? Add(string key, string? text = null)
	{
		string? normalized = _Normalize(key ?? string.Empty);
		RecentItem? rval = null;

		if(normalized is null) { return null; }

		_Modify(m =>
		{
			_Add(m, normalized, text, DateTime.UtcNow, _MaxCount, KeyComparer);
			rval = m[_IndexOf(m, normalized, KeyComparer)];
			return true;
		});

		_OnAdded(rval!);
		return rval;
	}


	/// <summary>Removes an entry.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns <c>true</c> if the entry has been removed, otherwise returns <c>false</c>.</returns>
	public virtual bool Remove(string key)
	{
		string? normalized = _Normalize(key ?? string.Empty);
		bool rval = false;

		if(normalized is null) { return false; }

		_Modify(m =>
		{
			int index = _IndexOf(m, normalized, KeyComparer);
			if(index >= 0) { m.RemoveAt(index); }
			return (rval = (index >= 0));
		});

		return rval;
	}


	/// <summary>Pins an entry.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns <c>true</c> if the entry has been pinned, otherwise returns <c>false</c>.</returns>
	public virtual bool Pin(string key)
	{
		string? normalized = _Normalize(key ?? string.Empty);
		bool rval = false;

		if(normalized is null) { return false; }

		_Modify(m => (rval = _Pin(m, normalized, KeyComparer)));
		return rval;
	}


	/// <summary>Unpins an entry.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns <c>true</c> if the entry has been unpinned, otherwise returns <c>false</c>.</returns>
	public virtual bool Unpin(string key)
	{
		string? normalized = _Normalize(key ?? string.Empty);
		bool rval = false;

		if(normalized is null) { return false; }

		_Modify(m => (rval = _Unpin(m, normalized, _MaxCount, KeyComparer)));
		return rval;
	}


	/// <summary>Removes all entries.</summary>
	/// <param name="includePinned">Determines if pinned entries are removed as well.</param>
	public virtual void Clear(bool includePinned = false)
	{
		_Modify(m => (m.RemoveAll(n => includePinned || (!n.Pinned)) > 0));
	}


	/// <summary>Returns if the list contains an entry.</summary>
	/// <param name="key">Key.</param>
	/// <returns>Returns <c>true</c> if the list contains the entry, otherwise returns <c>false</c>.</returns>
	public virtual bool Contains(string key)
	{
		string? normalized = _Normalize(key ?? string.Empty);
		return ((normalized is not null) && Items.Any(m => KeyComparer.Equals(m.Key, normalized)));
	}


	/// <summary>Loads the list from the store again, for example to show changes made by another instance.</summary>
	public virtual void Reload()
	{
		_Items = null;
		_RebuildMenu();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// designer support                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the list name has to be serialized by the designer.</summary>
	/// <returns>Returns <c>true</c> if the list name differs from the default name, otherwise returns <c>false</c>.</returns>
	private bool ShouldSerializeListName()
	{
		return (_ListName != _DefaultListName);
	}


	/// <summary>Resets the list name to the default name.</summary>
	private void ResetListName()
	{
		ListName = _DefaultListName;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the DropDownOpening event of the menu item (submenu style).</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuItem_DropDownOpening(object? sender, EventArgs e)
	{
		Reload();
	}


	/// <summary>Handles the Opening event of the drop-down containing the menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Owner_Opening(object? sender, CancelEventArgs e)
	{
		Reload();
	}


	/// <summary>Handles the OwnerChanged event of the menu item.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _MenuItem_OwnerChanged(object? sender, EventArgs e)
	{
		_HookOwner(_MenuItem?.Owner as ToolStripDropDown);
	}


	/// <summary>Handles the Click event of entries.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Entry_Click(object? sender, EventArgs e)
	{
		if(_IgnoreClick)
		{
			_IgnoreClick = false;
			return;
		}

		if((sender is ToolStripItem menuItem) && (menuItem.Tag is RecentItem item)) { _HandleClick(() => _Select(item)); }
	}


	/// <summary>Handles the MouseUp event of entries: the right mouse button shows the context menu.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Entry_MouseUp(object? sender, MouseEventArgs e)
	{
		ContextMenuStrip context;

		if((e.Button != MouseButtons.Right) || (sender is not ToolStripMenuItem menuItem) || (menuItem.Tag is not RecentItem item)) { return; }

		context = _GetContext();
		context.Items["pin"]!.Text = (item.Pinned ? UnpinText : PinText);

		_ResumeDropDowns();
		_SuspendedDropDowns.AddRange(_GetDropDowns(menuItem));
		foreach(ToolStripDropDown i in _SuspendedDropDowns) { i.AutoClose = false; }

		_ContextSource = menuItem;
		_IgnoreClick = true;
		context.Show(Cursor.Position);
	}


	/// <summary>Handles the Closed event of the context menu.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Context_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
	{
		_IgnoreClick = false;
		if(e.CloseReason != ToolStripDropDownCloseReason.ItemClicked) { _ResumeDropDowns(); }
	}


	/// <summary>Handles the Click event of the context menu item that opens an entry.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ContextOpen_Click(object? sender, EventArgs e)
	{
		List<ToolStripDropDown> dropDowns = _SuspendedDropDowns.ToList();

		_ResumeDropDowns();
		foreach(ToolStripDropDown i in dropDowns) { i.Close(ToolStripDropDownCloseReason.ItemClicked); }

		if(_ContextSource?.Tag is RecentItem item) { _HandleClick(() => _Select(item)); }
	}


	/// <summary>Handles the Click event of the context menu item that pins or unpins an entry.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ContextPin_Click(object? sender, EventArgs e)
	{
		_ResumeDropDowns();
		if(_ContextSource?.Tag is not RecentItem item) { return; }

		if(item.Pinned)
		{
			Unpin(item.Key);
		}
		else { Pin(item.Key); }
	}


	/// <summary>Handles the Click event of the context menu item that removes an entry.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ContextRemove_Click(object? sender, EventArgs e)
	{
		_ResumeDropDowns();
		if(_ContextSource?.Tag is RecentItem item) { Remove(item.Key); }
	}


	/// <summary>Handles the Click event of the menu item that clears the list.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Clear_Click(object? sender, EventArgs e)
	{
		_HandleClick(() => Clear(false));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Component                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_RebuildPending = false;
			_ResumeDropDowns();
			_ClearMenu();
			_HookMenu(false);
			_RestoreMenuItem();

			if(_Context is not null)
			{
				_Context.Closed -= _Context_Closed;
				_Context.Dispose();
				_Context = null;
			}
		}

		base.Dispose(disposing);
	}
}
