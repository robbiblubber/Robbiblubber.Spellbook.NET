namespace Robbiblubber.Spellbook.Controls;

using System.ComponentModel;
using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements a color dialog in the colors of a theme.</summary>
/// <remarks><para>The dialog is used like <see cref="ColorDialog"/>: set <see cref="Color"/>, call <see cref="ShowDialog(IWin32Window?)"/>
///          and read <see cref="Color"/> when the result is <see cref="DialogResult.OK"/>:
///          <code>using ThemedColorDialog dialog = new() { Color = panel.BackColor };
///if(dialog.ShowDialog(this) == DialogResult.OK) { panel.BackColor = dialog.Color; }</code></para>
///          <para>The dialog shows basic colors, custom colors, an area for saturation and value with a hue slider, number fields for
///          hue, saturation, value, red, green and blue, and the hexadecimal value, which also takes color names like <c>teal</c>.
///          With <see cref="AllowAlpha"/>, the color can be transparent: there is an alpha slider and an alpha field, and the
///          hexadecimal value has eight digits. Clicking the lower half of the preview goes back to the original color.</para>
///          <para>The custom colors are kept in <see cref="CustomColors"/>; an application that wants to keep them between sessions
///          saves and restores this property. The dialog uses the theme of its owner (see <see cref="ThemeManager.GetTheme(Control)"/>),
///          or <see cref="Theme"/> if it is set. The texts come from <see cref="ThemedColorDialogTexts"/>.</para></remarks>
[SupportedOSPlatform("windows")]
[DefaultProperty(nameof(Color))]
[ToolboxBitmap(typeof(ColorDialog))]
public class ThemedColorDialog: Component
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Number of custom colors.</summary>
	public const int CUSTOM_COLOR_COUNT = _ColorDialogWindow._CUSTOM_COUNT;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Custom colors.</summary>
	private Color[] _CustomColors = new Color[CUSTOM_COLOR_COUNT];



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public ThemedColorDialog(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public ThemedColorDialog(IContainer container): this()
	{
		container.Add(this);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the color.</summary>
	[Category("Data")][Description("The color that the dialog shows and that the user has selected.")][DefaultValue(typeof(Color), "Black")]
	public Color Color
	{
		get; set;
	} = Color.Black;


	/// <summary>Gets or sets if the color can be transparent.</summary>
	/// <remarks>Without alpha, the dialog shows and returns the color without its transparency.</remarks>
	[Category("Behavior")][Description("Determines if the user can choose a transparent color.")][DefaultValue(false)]
	public bool AllowAlpha
	{
		get; set;
	} = false;


	/// <summary>Gets or sets the custom colors.</summary>
	/// <remarks>The dialog has <see cref="CUSTOM_COLOR_COUNT"/> custom colors; <see cref="Color.Empty"/> is an empty cell. Setting
	///          fewer colors leaves the other cells empty, more colors are ignored. OK returns the custom colors as the user has left
	///          them, Cancel leaves them unchanged.</remarks>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color[] CustomColors
	{
		get { return (Color[]) _CustomColors.Clone(); }
		set
		{
			_CustomColors = new Color[CUSTOM_COLOR_COUNT];
			if(value is not null) { Array.Copy(value, _CustomColors, Math.Min(value.Length, CUSTOM_COLOR_COUNT)); }
		}
	}


	/// <summary>Gets or sets the title, NULL for the title from <see cref="ThemedColorDialogTexts.Title"/>.</summary>
	[Category("Appearance")][Description("The title of the dialog; empty for the default title.")][DefaultValue(null)]
	public string? Title
	{
		get; set;
	}


	/// <summary>Gets or sets the theme, NULL for the theme of the owner.</summary>
	[Browsable(false)][DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Theme? Theme
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the window of the dialog.</summary>
	/// <param name="owner">Owner window, NULL for the active window.</param>
	/// <returns>Returns the window.</returns>
	internal _ColorDialogWindow _CreateWindow(IWin32Window? owner)
	{
		Control? control = ((owner as Control) ?? ((owner is null) ? Form.ActiveForm : Control.FromHandle(owner.Handle)));
		Theme? theme = (Theme ?? ((control is null) ? null : ThemeManager.GetTheme(control)));

		return new _ColorDialogWindow(Color, _CustomColors, AllowAlpha, (string.IsNullOrEmpty(Title) ? null : Title), theme, (control is not null));
	}


	/// <summary>Takes over the result of the window.</summary>
	/// <param name="window">Window.</param>
	internal void _TakeResult(_ColorDialogWindow window)
	{
		Color = window._Color;
		_CustomColors = window._CustomColors;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Shows the dialog with the active window as owner.</summary>
	/// <returns>Returns <see cref="DialogResult.OK"/> if the user has chosen a color, otherwise <see cref="DialogResult.Cancel"/>.</returns>
	public DialogResult ShowDialog()
	{
		return ShowDialog(null);
	}


	/// <summary>Shows the dialog.</summary>
	/// <param name="owner">Owner window, NULL for the active window.</param>
	/// <returns>Returns <see cref="DialogResult.OK"/> if the user has chosen a color, otherwise <see cref="DialogResult.Cancel"/>.</returns>
	public DialogResult ShowDialog(IWin32Window? owner)
	{
		using _ColorDialogWindow window = _CreateWindow(owner);

		DialogResult rval = window.ShowDialog(owner);
		if(rval == DialogResult.OK) { _TakeResult(window); } else { rval = DialogResult.Cancel; }

		return rval;
	}
}
