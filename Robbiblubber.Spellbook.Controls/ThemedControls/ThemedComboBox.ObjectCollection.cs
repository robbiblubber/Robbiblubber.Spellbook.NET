namespace Robbiblubber.Spellbook.Controls;

using System.Collections.ObjectModel;
using System.Runtime.Versioning;



public partial class ThemedComboBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] ObjectCollection                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the item collection of a <see cref="ThemedComboBox"/>.</summary>
	/// <remarks>Items can be any objects except NULL. Items that implement <see cref="IListItem"/> are displayed with their image,
	///          detail text, font style and color; other objects are displayed with their text (see
	///          <see cref="ThemedComboBox.GetItemText(object)"/>).</remarks>
	[SupportedOSPlatform("windows")]
	public sealed class ObjectCollection: Collection<object>
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Owner control.</summary>
		private readonly ThemedComboBox _Owner;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="owner">Owner control.</param>
		internal ObjectCollection(ThemedComboBox owner): base()
		{
			_Owner = owner;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Adds items to the collection.</summary>
		/// <param name="items">Items.</param>
		/// <exception cref="ArgumentNullException">Thrown when the items or one of the items is NULL.</exception>
		/// <remarks>The control is updated once after all items have been added.</remarks>
		public void AddRange(params object[] items)
		{
			AddRange((IEnumerable<object>) items);
		}


		/// <summary>Adds items to the collection.</summary>
		/// <param name="items">Items.</param>
		/// <exception cref="ArgumentNullException">Thrown when the items or one of the items is NULL.</exception>
		/// <remarks>The control is updated once after all items have been added.</remarks>
		public void AddRange(IEnumerable<object> items)
		{
			ArgumentNullException.ThrowIfNull(items);
			_CheckNotBound();

			object[] array = items.ToArray();
			if(array.Any(m => m is null)) { throw new ArgumentNullException(nameof(items), "The items must not contain NULL."); }
			if(array.Length == 0) { return; }

			foreach(object i in array) { Items.Add(i); }
			_Owner._OnItemsChanged();
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private methods                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Throws an exception if the items are provided by a data source.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the owner has a data source.</exception>
		private void _CheckNotBound()
		{
			if(_Owner._IsDataBound) { throw new InvalidOperationException("The items cannot be changed while a data source is set."); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// internal methods                                                                                             //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Replaces all items without changing the selection.</summary>
		/// <param name="items">New items.</param>
		/// <remarks>The owner adjusts the selection after the items have been replaced.</remarks>
		internal void _Replace(IEnumerable<object> items)
		{
			Items.Clear();
			foreach(object i in items) { Items.Add(i); }

			_Owner._OnItemsChanged();
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [override] Collection<object>                                                                                //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException">Thrown when the item is NULL.</exception>
		protected override void InsertItem(int index, object item)
		{
			ArgumentNullException.ThrowIfNull(item);
			_CheckNotBound();

			base.InsertItem(index, item);
			_Owner._OnItemInserted(index);
		}


		/// <inheritdoc/>
		protected override void RemoveItem(int index)
		{
			_CheckNotBound();
			base.RemoveItem(index);
			_Owner._OnItemRemoved(index);
		}


		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException">Thrown when the item is NULL.</exception>
		protected override void SetItem(int index, object item)
		{
			ArgumentNullException.ThrowIfNull(item);
			_CheckNotBound();

			base.SetItem(index, item);
			_Owner._OnItemSet(index);
		}


		/// <inheritdoc/>
		protected override void ClearItems()
		{
			_CheckNotBound();
			if(Count == 0) { return; }

			base.ClearItems();
			_Owner._OnItemsCleared();
		}
	}
}
