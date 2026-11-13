namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class remembers the original state of a themed control.</summary>
/// <remarks><para>Theme adapters change properties only through <see cref="ApplyColor(object, string, ThemeColor?)"/> and
///          <see cref="SetValue(object, string, object?)"/>. The first change of a property remembers its original value; when the theme
///          manager restores the control, all properties get their original values back. Properties are addressed by name, so they can
///          belong to the control or to an object of the control, like <see cref="ButtonBase.FlatAppearance"/>.</para>
///          <para>Colors that have been set explicitly (in the designer or in code) are kept, unless they are system colors; system
///          colors are replaced by the matching color of the theme (<see cref="Theme.MapSystemColor(Color)"/>). If a color is changed
///          by the application after it has been themed, the new color is kept as well.</para></remarks>
[SupportedOSPlatform("windows")]
public sealed class ThemeAdapterState
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Original                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class holds the original value of a property.</summary>
	private sealed class _Original
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="target">Object that owns the property.</param>
		/// <param name="property">Property.</param>
		public _Original(object target, PropertyDescriptor property)
		{
			Target = target;
			Property = property;
			Value = property.GetValue(target);
			Explicit = property.ShouldSerializeValue(target);
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the object that owns the property.</summary>
		public object Target
		{
			get;
		}


		/// <summary>Gets the property.</summary>
		public PropertyDescriptor Property
		{
			get;
		}


		/// <summary>Gets or sets the original value.</summary>
		public object? Value
		{
			get; set;
		}


		/// <summary>Gets or sets if the original value has been set explicitly.</summary>
		public bool Explicit
		{
			get; set;
		}


		/// <summary>Gets or sets if the value has been changed by the adapter.</summary>
		public bool Changed
		{
			get; set;
		}


		/// <summary>Gets or sets the value that the adapter has set last.</summary>
		public object? LastValue
		{
			get; set;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Sets a value.</summary>
		/// <param name="value">Value.</param>
		/// <remarks>The value is also set if it equals an inherited value, so it is set explicitly afterwards (a group box, for example,
		///          uses its fore color only if it has been set).</remarks>
		public void Set(object? value)
		{
			if(!Equals(Property.GetValue(Target), value) || !Property.ShouldSerializeValue(Target)) { Property.SetValue(Target, value); }

			Changed = true;
			LastValue = value;
		}


		/// <summary>Restores the original value.</summary>
		public void Restore()
		{
			if(!Changed) { return; }
			Changed = false;
			LastValue = null;

			if(!Explicit && Property.CanResetValue(Target))
			{
				Property.ResetValue(Target);
			}
			else if(!Equals(Property.GetValue(Target), Value)) { Property.SetValue(Target, Value); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _KeyComparer                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class compares property keys by the identity of their objects.</summary>
	private sealed class _KeyComparer: IEqualityComparer<(object Target, string Property)>
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IEqualityComparer<(object, string)>                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Returns if two keys are equal.</summary>
		/// <param name="x">First key.</param>
		/// <param name="y">Second key.</param>
		/// <returns>Returns <c>true</c> if both keys refer to the same property of the same object, otherwise returns <c>false</c>.</returns>
		public bool Equals((object Target, string Property) x, (object Target, string Property) y)
		{
			return (ReferenceEquals(x.Target, y.Target) && (x.Property == y.Property));
		}


		/// <summary>Gets the hash code of a key.</summary>
		/// <param name="obj">Key.</param>
		/// <returns>Returns the hash code.</returns>
		public int GetHashCode((object Target, string Property) obj)
		{
			return HashCode.Combine(RuntimeHelpers.GetHashCode(obj.Target), obj.Property);
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Original values by object and property.</summary>
	private readonly Dictionary<(object Target, string Property), _Original> _Originals = new(new _KeyComparer());

	/// <summary>Original values in the order of their first change.</summary>
	private readonly List<_Original> _Order = new();

	/// <summary>Actions that remove the event handlers of the adapter, by key.</summary>
	private readonly Dictionary<string, Action> _Unsubscribers = new();

	/// <summary>Additional restore actions of the adapter, by key.</summary>
	private readonly Dictionary<string, Action> _Restorers = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="control">Control.</param>
	internal ThemeAdapterState(Control control)
	{
		Control = control;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the original value of a property, remembering it if necessary.</summary>
	/// <param name="target">Object that owns the property.</param>
	/// <param name="property">Property name.</param>
	/// <returns>Returns the original value.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the object or the property name is NULL.</exception>
	/// <exception cref="ArgumentException">Thrown when the object has no such property.</exception>
	private _Original _GetOriginal(object target, string property)
	{
		ArgumentNullException.ThrowIfNull(target);
		ArgumentNullException.ThrowIfNull(property);

		if(!_Originals.TryGetValue((target, property), out _Original? rval))
		{
			PropertyDescriptor descriptor = (TypeDescriptor.GetProperties(target)[property] ??
											 throw new ArgumentException($"{target.GetType().Name} has no property {property}.", nameof(property)));
			rval = new _Original(target, descriptor);
			_Originals.Add((target, property), rval);
			_Order.Add(rval);
		}
		else if(rval.Changed && !Equals(rval.Property.GetValue(target), rval.LastValue))
		{
			rval.Value = rval.Property.GetValue(target);
			rval.Explicit = true;
			rval.Changed = false;
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the event handlers that the theme manager has installed.</summary>
	internal Action? _ManagerUnsubscriber
	{
		get; set;
	}


	/// <summary>Gets or sets the theme that applies to the control, including themes that use the system colors; NULL if the theme
	///          manager does not handle the control.</summary>
	internal Theme? _Resolved
	{
		get; set;
	}


	/// <summary>Gets the number of remembered properties.</summary>
	internal int _Count
	{
		get { return _Order.Count; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Restores the original state.</summary>
	internal void _Restore()
	{
		Theme = null;

		foreach(Action i in _Unsubscribers.Values.ToArray()) { i(); }
		_Unsubscribers.Clear();

		foreach(Action i in _Restorers.Values.ToArray()) { i(); }
		_Restorers.Clear();

		for(int i = (_Order.Count - 1); i >= 0; i--) { _Order[i].Restore(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the control.</summary>
	public Control Control
	{
		get;
	}


	/// <summary>Gets the theme that is applied, NULL if the control is in its original state.</summary>
	public Theme? Theme
	{
		get; internal set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sets a color property to a color of the theme.</summary>
	/// <param name="target">Object that owns the property, usually the control.</param>
	/// <param name="property">Name of a property of type <see cref="Color"/>.</param>
	/// <param name="role">Color role used if the color has not been set explicitly, NULL to keep such a color (for example for
	///                    ambient colors that come from the parent control).</param>
	/// <returns>Returns <c>true</c> if the property has been changed, <c>false</c> if the color has been kept.</returns>
	/// <remarks>An explicitly set color is kept unless it is a system color; a system color is replaced by the matching color of the
	///          theme.</remarks>
	/// <exception cref="InvalidOperationException">Thrown when no theme is being applied.</exception>
	/// <exception cref="ArgumentException">Thrown when the object has no such property.</exception>
	public bool ApplyColor(object target, string property, ThemeColor? role)
	{
		Theme theme = (Theme ?? throw new InvalidOperationException("No theme is being applied."));
		_Original original = _GetOriginal(target, property);

		Color color;
		if(original.Explicit)
		{
			if(original.Value is not Color value) { return false; }
			if(!value.IsSystemColor) { original.Restore(); return false; }
			color = theme.MapSystemColor(value);
		}
		else if(role is ThemeColor r) { color = theme[r]; }
		else
		{
			original.Restore();
			return false;
		}

		if((property == nameof(System.Windows.Forms.Control.BackColor)) && (color.A < 255)) { color = Color.FromArgb(255, color); }
		original.Set(color);
		return true;
	}


	/// <summary>Sets a property.</summary>
	/// <param name="target">Object that owns the property, usually the control.</param>
	/// <param name="property">Property name.</param>
	/// <param name="value">Value.</param>
	/// <remarks>The value is set in any case; the original value is restored with the control.</remarks>
	/// <exception cref="ArgumentException">Thrown when the object has no such property.</exception>
	public void SetValue(object target, string property, object? value)
	{
		_GetOriginal(target, property).Set(value);
	}


	/// <summary>Installs event handlers once.</summary>
	/// <param name="key">Key that identifies the event handlers.</param>
	/// <param name="subscribe">Action that installs the event handlers.</param>
	/// <param name="unsubscribe">Action that removes the event handlers when the control is restored.</param>
	/// <remarks>If event handlers with this key are installed already, nothing happens.</remarks>
	public void Subscribe(string key, Action subscribe, Action unsubscribe)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(subscribe);
		ArgumentNullException.ThrowIfNull(unsubscribe);
		if(_Unsubscribers.ContainsKey(key)) { return; }

		subscribe();
		_Unsubscribers.Add(key, unsubscribe);
	}


	/// <summary>Registers an action that restores a change that is not a property, for example of the native window.</summary>
	/// <param name="key">Key that identifies the action.</param>
	/// <param name="restore">Action, called once when the control is restored.</param>
	/// <remarks>If an action with this key is registered already, it is kept.</remarks>
	public void OnRestore(string key, Action restore)
	{
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(restore);

		_Restorers.TryAdd(key, restore);
	}
}
