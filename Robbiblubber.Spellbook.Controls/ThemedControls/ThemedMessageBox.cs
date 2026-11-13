namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class displays message boxes in the colors of a theme.</summary>
/// <remarks><para>The class has the same <c>Show</c> methods as <see cref="MessageBox"/>, so a call can be changed by replacing the class
///          name: <c>ThemedMessageBox.Show(this, "Save changes?", "Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)</c>.
///          The message box uses the theme of its owner (see <see cref="ThemeManager.GetTheme(Control)"/>), without owner the theme
///          of the active window or of the application. It behaves like a Windows message box: Esc and the close button cancel if
///          the buttons have a cancel or OK button, Ctrl+C copies caption, text and buttons, the system sound of the icon is played.
///          The button texts come from <see cref="ThemedMessageBoxTexts"/>.</para>
///          <para>With <see cref="MessageBoxOptions.DefaultDesktopOnly"/> or <see cref="MessageBoxOptions.ServiceNotification"/>, the
///          standard Windows message box is shown, because only Windows can show it on the default desktop.</para></remarks>
[SupportedOSPlatform("windows")]
public static class ThemedMessageBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Displays a message box.</summary>
	/// <param name="owner">Owner window, NULL for the active window.</param>
	/// <param name="text">Text.</param>
	/// <param name="caption">Caption.</param>
	/// <param name="buttons">Buttons.</param>
	/// <param name="icon">Icon.</param>
	/// <param name="defaultButton">Default button.</param>
	/// <param name="options">Options.</param>
	/// <returns>Returns the result.</returns>
	private static DialogResult _Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		if(options.HasFlag(MessageBoxOptions.DefaultDesktopOnly) || options.HasFlag(MessageBoxOptions.ServiceNotification))
		{
			return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
		}

		using _MessageBoxWindow window = _CreateWindow(owner, text, caption, buttons, icon, defaultButton, options);
		return window.ShowDialog(owner);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the window of a message box.</summary>
	/// <param name="owner">Owner window, NULL for the active window.</param>
	/// <param name="text">Text.</param>
	/// <param name="caption">Caption.</param>
	/// <param name="buttons">Buttons.</param>
	/// <param name="icon">Icon.</param>
	/// <param name="defaultButton">Default button.</param>
	/// <param name="options">Options.</param>
	/// <returns>Returns the window.</returns>
	internal static _MessageBoxWindow _CreateWindow(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		Control? control = ((owner as Control) ?? ((owner is null) ? Form.ActiveForm : Control.FromHandle(owner.Handle)));
		Theme? theme = ((control is null) ? null : ThemeManager.GetTheme(control));

		return new _MessageBoxWindow((text ?? string.Empty), (caption ?? string.Empty), buttons, icon, defaultButton, options, theme, (control is not null));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text)
	{
		return _Show(null, text, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text, string? caption)
	{
		return _Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons)
	{
		return _Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		return _Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <param name="defaultButton">The default button of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		return _Show(null, text, caption, buttons, icon, defaultButton, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <param name="defaultButton">The default button of the message box.</param>
	/// <param name="options">Options of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		return _Show(null, text, caption, buttons, icon, defaultButton, options);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text)
	{
		return _Show(owner, text, null, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text, string? caption)
	{
		return _Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons)
	{
		return _Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		return _Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <param name="defaultButton">The default button of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		return _Show(owner, text, caption, buttons, icon, defaultButton, 0);
	}


	/// <summary>Displays a message box in the colors of the theme.</summary>
	/// <param name="owner">The window that owns the message box; its theme is used.</param>
	/// <param name="text">The text to display in the message box.</param>
	/// <param name="caption">The text to display in the title bar of the message box.</param>
	/// <param name="buttons">The buttons to display in the message box.</param>
	/// <param name="icon">The icon to display in the message box.</param>
	/// <param name="defaultButton">The default button of the message box.</param>
	/// <param name="options">Options of the message box.</param>
	/// <returns>Returns the result of the button that the user has clicked.</returns>
	public static DialogResult Show(IWin32Window? owner, string? text, string? caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		return _Show(owner, text, caption, buttons, icon, defaultButton, options);
	}
}
