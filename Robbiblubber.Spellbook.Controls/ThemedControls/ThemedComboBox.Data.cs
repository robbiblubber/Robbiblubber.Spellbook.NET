namespace Robbiblubber.Spellbook.Controls;

using System.Collections;
using System.ComponentModel;
using System.Runtime.Versioning;



public partial class ThemedComboBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Data source.</summary>
	private object? _DataSource = null;

	/// <summary>Currency manager of the data source, NULL if the control has no binding context.</summary>
	private CurrencyManager? _DataManager = null;

	/// <summary>Bound list that is observed without currency manager.</summary>
	private IBindingList? _BindingList = null;

	/// <summary>Determines if the items are being filled from the data source.</summary>
	private bool _FillingItems = false;

	/// <summary>Determines if the selection and the position of the currency manager are being synchronized.</summary>
	private bool _SyncingPosition = false;

	/// <summary>Display member.</summary>
	private string _DisplayMember = string.Empty;

	/// <summary>Value member.</summary>
	private string _ValueMember = string.Empty;

	/// <summary>Image member.</summary>
	private string _ImageMember = string.Empty;

	/// <summary>Detail member.</summary>
	private string _DetailMember = string.Empty;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the value of a member of an item.</summary>
	/// <param name="item">Item.</param>
	/// <param name="member">Member name; a path like <c>Address.City</c> is not supported.</param>
	/// <returns>Returns the member value, or NULL if the member is empty or does not exist.</returns>
	private object? _GetMemberValue(object item, string member)
	{
		if(string.IsNullOrEmpty(member)) { return null; }

		return TypeDescriptor.GetProperties(item).Find(member, true)?.GetValue(item);
	}


	/// <summary>Gets the list of a data source.</summary>
	/// <param name="dataSource">Data source.</param>
	/// <returns>Returns the list, or NULL if the data source is NULL.</returns>
	private static IList? _GetList(object? dataSource)
	{
		return dataSource switch
		{
			IListSource source => source.GetList(),
			IList list => list,
			_ => null
		};
	}


	/// <summary>Gets the value of an item.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the value of the <see cref="ValueMember"/>, or the item itself if no value member is set.</returns>
	private object? _GetItemValue(object? item)
	{
		if(item is null) { return null; }
		return (string.IsNullOrEmpty(_ValueMember) ? item : _GetMemberValue(item, _ValueMember));
	}


	/// <summary>Connects the control to the data source.</summary>
	private void _BindData()
	{
		_UnbindData();
		if(_DataSource is null) { return; }

		IList? list;

		if(BindingContext is not null)
		{
			_DataManager = (CurrencyManager) BindingContext[_DataSource];
			_DataManager.ListChanged += _DataManager_ListChanged;
			_DataManager.PositionChanged += _DataManager_PositionChanged;
			list = _DataManager.List;
		}
		else
		{
			list = _GetList(_DataSource);
			_BindingList = (list as IBindingList);
			if(_BindingList is not null) { _BindingList.ListChanged += _BindingList_ListChanged; }
		}

		_FillFromList(list);
	}


	/// <summary>Disconnects the control from the data source.</summary>
	private void _UnbindData()
	{
		if(_DataManager is not null)
		{
			_DataManager.ListChanged -= _DataManager_ListChanged;
			_DataManager.PositionChanged -= _DataManager_PositionChanged;
			_DataManager = null;
		}

		if(_BindingList is not null)
		{
			_BindingList.ListChanged -= _BindingList_ListChanged;
			_BindingList = null;
		}
	}


	/// <summary>Fills the items from a list and restores the selection.</summary>
	/// <param name="list">List, NULL to clear the items.</param>
	private void _FillFromList(IList? list)
	{
		int oldIndex = _SelectedIndex;
		object? oldItem = _SelectedItem;
		int index = -1;

		_FillingItems = true;
		try
		{
			_SelectedIndex = -1;
			_Items._Replace((list is null) ? [] : list.Cast<object?>().Where(m => m is not null).Cast<object>());
		}
		finally { _FillingItems = false; }

		if((_DataManager is not null) && (_DataManager.Position >= 0) && (_DataManager.Position < _Items.Count))
		{
			index = _DataManager.Position;
		}
		else if(oldItem is not null) { index = _Items.IndexOf(oldItem); }

		if(index >= 0)
		{
			_SelectedIndex = oldIndex;
			_SetSelection(index, _Items[index], false);
		}
		else if(oldIndex >= 0)
		{
			_SelectedIndex = oldIndex;
			_SetSelection(-1, null, false);
		}
	}


	/// <summary>Handles a change of a member.</summary>
	private void _MembersChanged()
	{
		_ItemsChanged();
		_UpdateText();
	}


	/// <summary>Sets the position of the currency manager to the selected index.</summary>
	private void _SyncPosition()
	{
		if((_DataManager is null) || _SyncingPosition || _FillingItems) { return; }
		if((_SelectedIndex < 0) || (_SelectedIndex >= _DataManager.Count) || (_DataManager.Position == _SelectedIndex)) { return; }

		_SyncingPosition = true;
		try
		{
			_DataManager.Position = _SelectedIndex;
		}
		finally { _SyncingPosition = false; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected methods                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Raises the <see cref="SelectedValueChanged"/> event.</summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void _OnSelectedValueChanged(EventArgs e)
	{
		SelectedValueChanged?.Invoke(this, e);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets if the items are filled from a data source and cannot be changed directly.</summary>
	internal bool _IsDataBound
	{
		get { return ((_DataSource is not null) && (!_FillingItems)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public events                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Occurs when the selected value has changed.</summary>
	[Category("Behavior")][Description("Occurs when the selected value has changed.")]
	public event EventHandler? SelectedValueChanged;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the data source that provides the items.</summary>
	/// <remarks><para>The data source is an <see cref="IList"/> (for example a <see cref="BindingList{T}"/>) or an
	///          <see cref="IListSource"/> (for example a <see cref="System.Data.DataTable"/>). Changes of an <see cref="IBindingList"/>
	///          are applied to the items. While a data source is set, <see cref="Items"/> cannot be changed directly.</para>
	///          <para>If the control has a binding context, the selection is synchronized with the position of the currency manager.</para></remarks>
	/// <exception cref="ArgumentException">Thrown when the value is neither an <see cref="IList"/> nor an <see cref="IListSource"/>.</exception>
	[Category("Data")][Description("The data source that provides the items.")][DefaultValue(null)]
	[AttributeProvider(typeof(IListSource))][RefreshProperties(RefreshProperties.Repaint)]
	public object? DataSource
	{
		get { return _DataSource; }
		set
		{
			if((value is not null) && (value is not IList) && (value is not IListSource)) { throw new ArgumentException("The data source must be an IList or an IListSource.", nameof(value)); }
			if(ReferenceEquals(value, _DataSource)) { return; }

			_CloseDropDown(false);
			_UnbindData();
			_DataSource = value;

			if(value is null)
			{
				_FillFromList(null);
			}
			else { _BindData(); }
		}
	}


	/// <summary>Gets or sets the property that provides the display text of items that do not implement <see cref="IListItem"/>.</summary>
	[Category("Data")][Description("The property that provides the display text of the items.")][DefaultValue("")]
	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public string DisplayMember
	{
		get { return _DisplayMember; }
		set
		{
			_DisplayMember = (value ?? string.Empty);
			_MembersChanged();
		}
	}


	/// <summary>Gets or sets the property that provides the value of the items (see <see cref="SelectedValue"/>).</summary>
	[Category("Data")][Description("The property that provides the value of the items.")][DefaultValue("")]
	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public string ValueMember
	{
		get { return _ValueMember; }
		set
		{
			_ValueMember = (value ?? string.Empty);
			_OnSelectedValueChanged(EventArgs.Empty);
		}
	}


	/// <summary>Gets or sets the property that provides the image of items that do not implement <see cref="IListItem"/>.</summary>
	/// <remarks>The property value can be an <see cref="System.Drawing.Image"/> or the key of an image in <see cref="ImageList"/>.</remarks>
	[Category("Data")][Description("The property that provides the image (Image or image key) of the items.")][DefaultValue("")]
	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public string ImageMember
	{
		get { return _ImageMember; }
		set
		{
			_ImageMember = (value ?? string.Empty);
			_MembersChanged();
		}
	}


	/// <summary>Gets or sets the property that provides the detail text of items that do not implement <see cref="IListItem"/>.</summary>
	[Category("Data")][Description("The property that provides the detail text of the items.")][DefaultValue("")]
	[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public string DetailMember
	{
		get { return _DetailMember; }
		set
		{
			_DetailMember = (value ?? string.Empty);
			_MembersChanged();
		}
	}


	/// <summary>Gets or sets the value of the selected item.</summary>
	/// <remarks>The value is provided by <see cref="ValueMember"/>; without value member it is the item itself. Setting the value
	///          selects the first item with this value, or clears the selection if there is no such item.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)][Bindable(true)]
	public object? SelectedValue
	{
		get { return _GetItemValue(_SelectedItem); }
		set
		{
			int index = -1;

			if(value is not null)
			{
				for(int i = 0; i < _Items.Count; i++)
				{
					if(Equals(_GetItemValue(_Items[i]), value))
					{
						index = i;
						break;
					}
				}
			}

			_SetSelection(index, ((index < 0) ? null : _Items[index]), false);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the value of an item.</summary>
	/// <param name="item">Item.</param>
	/// <returns>Returns the value of the <see cref="ValueMember"/>, or the item itself if no value member is set.</returns>
	public object? GetItemValue(object? item)
	{
		return _GetItemValue(item);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the ListChanged event of the currency manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _DataManager_ListChanged(object? sender, ListChangedEventArgs e)
	{
		if(_DataManager is null) { return; }
		_FillFromList(_DataManager.List);
	}


	/// <summary>Handles the PositionChanged event of the currency manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _DataManager_PositionChanged(object? sender, EventArgs e)
	{
		if((_DataManager is null) || _SyncingPosition || _FillingItems) { return; }

		int position = _DataManager.Position;
		if((position < 0) || (position >= _Items.Count) || (position == _SelectedIndex)) { return; }

		_SyncingPosition = true;
		try
		{
			_SetSelection(position, _Items[position], false);
		}
		finally { _SyncingPosition = false; }
	}


	/// <summary>Handles the ListChanged event of a bound list without currency manager.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _BindingList_ListChanged(object? sender, ListChangedEventArgs e)
	{
		_FillFromList(_BindingList);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override void OnBindingContextChanged(EventArgs e)
	{
		if(_DataSource is not null) { _BindData(); }
		base.OnBindingContextChanged(e);
	}
}
