namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.Versioning;



/// <summary>This class implements a component that applies a theme to a form or user control.</summary>
/// <remarks><para>Placed on a form, the component applies the theme with <see cref="ThemeManager.Apply(Control, Theme?)"/> when the
///          handle of the form is created, before the form is shown. The form follows theme changes, and controls that are added later
///          are themed as well.</para>
///          <para>Without a <see cref="Theme"/> of its own, the form uses the theme of the application
///          (<see cref="ThemeManager.Theme"/>).</para></remarks>
[ToolboxBitmap(typeof(ThemeProvider), "Resources.themeprovider.bmp")]
[SupportedOSPlatform("windows")]
public class ThemeProvider: Component
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Form or user control that the theme is applied to.</summary>
	private ContainerControl? _ContainerControl = null;

	/// <summary>Hooked container control.</summary>
	private ContainerControl? _HookedRoot = null;

	/// <summary>Theme, NULL for the theme of the application.</summary>
	private Theme? _Theme = null;

	/// <summary>Determines if the theme has been applied.</summary>
	private bool _Applied = false;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemeProvider(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public ThemeProvider(IContainer container): this()
	{
		container.Add(this);
	}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="containerControl">Form or user control that the theme is applied to.</param>
	public ThemeProvider(ContainerControl containerControl): this()
	{
		ContainerControl = containerControl;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hooks a container control.</summary>
	/// <param name="root">Container control, NULL to remove the hook.</param>
	private void _Hook(ContainerControl? root)
	{
		if(_HookedRoot is not null) { _HookedRoot.HandleCreated -= _Root_HandleCreated; }

		_HookedRoot = null;
		_Applied = false;
		if((root is null) || DesignMode) { return; }

		_HookedRoot = root;
		if(root.IsHandleCreated) { Apply(); } else { root.HandleCreated += _Root_HandleCreated; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the form or user control that the theme is applied to.</summary>
	/// <remarks>In the designer, this property is set to the form or user control automatically.</remarks>
	[Category("Behavior")][Description("Form or user control that the theme is applied to.")][DefaultValue(null)]
	public ContainerControl? ContainerControl
	{
		get { return _ContainerControl; }
		set
		{
			if(value == _ContainerControl) { return; }

			_ContainerControl = value;
			_Hook(value);
		}
	}


	/// <summary>Gets or sets the theme.</summary>
	/// <remarks>NULL uses the theme of the application. Changing the theme takes effect immediately.</remarks>
	[Category("Appearance")][Description("The theme of the form; (Application) for the application theme.")][DefaultValue(null)]
	public Theme? Theme
	{
		get { return _Theme; }
		set
		{
			if(value == _Theme) { return; }

			_Theme = value;
			if(_Applied) { Apply(); }
		}
	}


	/// <summary>Gets the theme that applies to the form or user control.</summary>
	[Browsable(false)]
	public Theme EffectiveTheme
	{
		get { return ((_ContainerControl is null) ? (_Theme ?? ThemeManager.Theme) : (_Theme ?? ThemeManager.GetTheme(_ContainerControl))); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Applies the theme to the form or user control now.</summary>
	/// <remarks>The theme is applied automatically when the handle of the form or user control is created; this method is only
	///          needed to apply it earlier.</remarks>
	public void Apply()
	{
		if((_ContainerControl is null) || DesignMode) { return; }

		if(_HookedRoot is not null) { _HookedRoot.HandleCreated -= _Root_HandleCreated; }
		ThemeManager.Apply(_ContainerControl, _Theme);
		_Applied = true;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the HandleCreated event of the container control.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private void _Root_HandleCreated(object? sender, EventArgs e)
	{
		Apply();
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
		if(disposing) { _Hook(null); }

		base.Dispose(disposing);
	}
}
