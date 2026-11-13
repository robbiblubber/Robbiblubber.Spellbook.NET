namespace Robbiblubber.Spellbook.Controls;

using System.Media;
using System.Runtime.Versioning;
using System.Text;

using Robbiblubber.Spellbook.Controls.Themes;



/// <summary>This class implements the window of <see cref="ThemedMessageBox"/>.</summary>
/// <remarks>The window is laid out like a Windows message box: icon and text at the top, the buttons right-aligned at the bottom. It
///          applies the theme with the theme manager, so it gets a dark title bar in dark themes.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _MessageBoxWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Window class style: no close button.</summary>
	private const int _CS_NOCLOSE = 0x0200;

	/// <summary>Logical maximum width of the text.</summary>
	private const int _MAX_TEXT_WIDTH = 460;

	/// <summary>Logical minimum width of the client area.</summary>
	private const int _MIN_WIDTH = 220;

	/// <summary>Logical size of the icon.</summary>
	private const int _ICON_SIZE = 32;

	/// <summary>Logical margin around icon and text.</summary>
	private const int _MARGIN = 20;

	/// <summary>Logical gap between icon and text.</summary>
	private const int _ICON_GAP = 12;

	/// <summary>Logical width of a button.</summary>
	private const int _BUTTON_WIDTH = 88;

	/// <summary>Logical height of a button.</summary>
	private const int _BUTTON_HEIGHT = 28;

	/// <summary>Logical gap between buttons.</summary>
	private const int _BUTTON_GAP = 8;

	/// <summary>Logical padding of the button bar.</summary>
	private const int _FOOTER_PADDING = 12;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Message text.</summary>
	private readonly string _Message;

	/// <summary>Icon.</summary>
	private readonly MessageBoxIcon _MessageIcon;

	/// <summary>Determines if the window can be closed without a button (Esc, close button).</summary>
	private readonly bool _CanClose;

	/// <summary>Buttons.</summary>
	private readonly List<ThemedButton> _ButtonList = new();

	/// <summary>Button bar.</summary>
	private readonly Panel _Footer;

	/// <summary>Icon image, NULL if there is no icon.</summary>
	private readonly Bitmap? _IconImage;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="text">Message text.</param>
	/// <param name="caption">Caption.</param>
	/// <param name="buttons">Buttons.</param>
	/// <param name="icon">Icon.</param>
	/// <param name="defaultButton">Default button.</param>
	/// <param name="options">Options.</param>
	/// <param name="theme">Theme, NULL for the theme of the application.</param>
	/// <param name="hasOwner">Determines if the window has an owner.</param>
	internal _MessageBoxWindow(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, Theme? theme, bool hasOwner)
	{
		_Message = text;
		_MessageIcon = icon;
		_CanClose = _GetCancelResult(buttons).HasValue;

		SuspendLayout();
		AutoScaleMode = AutoScaleMode.None;
		Font = (SystemFonts.MessageBoxFont ?? Control.DefaultFont);
		Text = caption;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MinimizeBox = MaximizeBox = false;
		ShowIcon = false;
		ShowInTaskbar = !hasOwner;
		StartPosition = (hasOwner ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen);
		KeyPreview = true;
		if(options.HasFlag(MessageBoxOptions.RtlReading))
		{
			RightToLeft = RightToLeft.Yes;
			RightToLeftLayout = true;
		}

		float scale = (DeviceDpi / 96f);
		int s(int value) => (int) Math.Round(value * scale);

		int iconSize = 0;
		StockIconId? stock = _GetStockIcon(icon);
		if(stock is StockIconId id)
		{
			iconSize = s(_ICON_SIZE);
			try
			{
				using Icon stockIcon = SystemIcons.GetStockIcon(id, iconSize);
				_IconImage = stockIcon.ToBitmap();
			}
			catch(Exception) { iconSize = 0; }
		}

		TextFormatFlags flags = (TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPrefix);
		Size textSize = TextRenderer.MeasureText(text, Font, new Size(s(_MAX_TEXT_WIDTH), int.MaxValue), flags);
		int maxTextHeight = (Screen.FromPoint(Cursor.Position).WorkingArea.Height * 2 / 3);

		DialogResult[] results = _GetResults(buttons);
		Size buttonSize = new(s(_BUTTON_WIDTH), s(_BUTTON_HEIGHT));
		int buttonsWidth = ((results.Length * buttonSize.Width) + ((results.Length - 1) * s(_BUTTON_GAP)));
		int footerHeight = (buttonSize.Height + (2 * s(_FOOTER_PADDING)));

		int iconGap = ((iconSize > 0) ? s(_ICON_GAP) : 0);
		int textWidth = Math.Min(textSize.Width, s(_MAX_TEXT_WIDTH));
		int textHeight = Math.Min(textSize.Height, maxTextHeight);
		int contentHeight = Math.Max(iconSize, textHeight);
		int width = Math.Max(s(_MIN_WIDTH), Math.Max(iconSize + iconGap + textWidth + (2 * s(_MARGIN)), buttonsWidth + (2 * s(_FOOTER_PADDING))));
		ClientSize = new Size(width, contentHeight + (2 * s(_MARGIN)) + footerHeight);

		if(_IconImage is not null)
		{
			PictureBox picture = new() { Image = _IconImage, SizeMode = PictureBoxSizeMode.Zoom, Bounds = new Rectangle(s(_MARGIN), s(_MARGIN), iconSize, iconSize), TabStop = false };
			Controls.Add(picture);
		}

		Panel textPanel = new()
		{
			Bounds = new Rectangle(s(_MARGIN) + iconSize + iconGap, s(_MARGIN) + Math.Max(0, (iconSize - textHeight) / 2), width - (2 * s(_MARGIN)) - iconSize - iconGap, textHeight),
			AutoScroll = (textSize.Height > textHeight)
		};
		Label label = new()
		{
			AutoSize = false,
			UseMnemonic = false,
			Text = text,
			Size = new Size(textPanel.Width - (textPanel.AutoScroll ? SystemInformation.VerticalScrollBarWidth : 0), textSize.Height),
			TextAlign = (options.HasFlag(MessageBoxOptions.RightAlign) ? ContentAlignment.TopRight : ContentAlignment.TopLeft)
		};
		textPanel.Controls.Add(label);
		Controls.Add(textPanel);

		_Footer = new Panel() { Dock = DockStyle.Bottom, Height = footerHeight };
		_Footer.Paint += _Footer_Paint;
		int x = (width - s(_FOOTER_PADDING) - buttonsWidth);
		foreach(DialogResult i in results)
		{
			ThemedButton button = new()
			{
				Text = ThemedMessageBoxTexts._Get(i),
				DialogResult = i,
				Bounds = new Rectangle(x, s(_FOOTER_PADDING), buttonSize.Width, buttonSize.Height),
				TabIndex = _ButtonList.Count
			};
			_ButtonList.Add(button);
			_Footer.Controls.Add(button);
			x += (buttonSize.Width + s(_BUTTON_GAP));
		}
		Controls.Add(_Footer);

		int index = defaultButton switch
		{
			MessageBoxDefaultButton.Button2 => 1,
			MessageBoxDefaultButton.Button3 => 2,
			_ => 0
		};
		AcceptButton = _ButtonList[(index < _ButtonList.Count) ? index : 0];
		if(_GetCancelResult(buttons) is DialogResult cancel) { CancelButton = _ButtonList.First(m => m.DialogResult == cancel); }
		ActiveControl = (Control) AcceptButton;

		ResumeLayout(false);
		ThemeManager.Apply(this, theme);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the dialog results of the buttons.</summary>
	/// <param name="buttons">Buttons.</param>
	/// <returns>Returns the results in the order of the buttons.</returns>
	private static DialogResult[] _GetResults(MessageBoxButtons buttons)
	{
		return buttons switch
		{
			MessageBoxButtons.OKCancel => new[] { DialogResult.OK, DialogResult.Cancel },
			MessageBoxButtons.AbortRetryIgnore => new[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore },
			MessageBoxButtons.YesNoCancel => new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel },
			MessageBoxButtons.YesNo => new[] { DialogResult.Yes, DialogResult.No },
			MessageBoxButtons.RetryCancel => new[] { DialogResult.Retry, DialogResult.Cancel },
			MessageBoxButtons.CancelTryContinue => new[] { DialogResult.Cancel, DialogResult.TryAgain, DialogResult.Continue },
			_ => new[] { DialogResult.OK }
		};
	}


	/// <summary>Gets the result when the window is closed with Esc or the close button.</summary>
	/// <param name="buttons">Buttons.</param>
	/// <returns>Returns the result, NULL if the window cannot be closed without a button.</returns>
	private static DialogResult? _GetCancelResult(MessageBoxButtons buttons)
	{
		return buttons switch
		{
			MessageBoxButtons.OK => DialogResult.OK,
			MessageBoxButtons.YesNo or MessageBoxButtons.AbortRetryIgnore => null,
			_ => DialogResult.Cancel
		};
	}


	/// <summary>Gets the stock icon for a message box icon.</summary>
	/// <param name="icon">Message box icon.</param>
	/// <returns>Returns the stock icon, NULL for none.</returns>
	private static StockIconId? _GetStockIcon(MessageBoxIcon icon)
	{
		return icon switch
		{
			MessageBoxIcon.Error => StockIconId.Error,
			MessageBoxIcon.Question => StockIconId.Help,
			MessageBoxIcon.Warning => StockIconId.Warning,
			MessageBoxIcon.Information => StockIconId.Info,
			_ => null
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal properties                                                                                              //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the buttons.</summary>
	internal IReadOnlyList<ThemedButton> _Buttons
	{
		get { return _ButtonList; }
	}


	/// <summary>Gets if the window can be closed without a button.</summary>
	internal bool _Closable
	{
		get { return _CanClose; }
	}


	/// <summary>Gets the text that Ctrl+C copies, in the format of Windows message boxes.</summary>
	internal string _CopyText
	{
		get
		{
			const string LINE = "---------------------------";
			StringBuilder rval = new();

			rval.Append(LINE).Append("\r\n").Append(Text).Append("\r\n").Append(LINE).Append("\r\n");
			rval.Append(_Message).Append("\r\n").Append(LINE).Append("\r\n");
			rval.Append(string.Join("   ", _ButtonList.Select(m => m.Text.Replace("&", string.Empty)))).Append("   \r\n").Append(LINE).Append("\r\n");

			return rval.ToString();
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Paint event of the button bar: draws a separator line at the top.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _Footer_Paint(object? sender, PaintEventArgs e)
	{
		using Pen pen = new(ThemeManager.GetTheme(this)[ThemeColor.SEPARATOR]);
		e.Graphics.DrawLine(pen, 0, 0, _Footer.Width, 0);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Form                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams rval = base.CreateParams;
			if(!_CanClose) { rval.ClassStyle |= _CS_NOCLOSE; }

			return rval;
		}
	}


	/// <inheritdoc/>
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if((keyData == (Keys.Control | Keys.C)) || (keyData == (Keys.Control | Keys.Insert)))
		{
			try
			{
				Clipboard.SetText(_CopyText);
			}
			catch(Exception) {}
			return true;
		}

		return base.ProcessCmdKey(ref msg, keyData);
	}


	/// <inheritdoc/>
	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);

		SystemSound sound = _MessageIcon switch
		{
			MessageBoxIcon.Error => SystemSounds.Hand,
			MessageBoxIcon.Question => SystemSounds.Question,
			MessageBoxIcon.Warning => SystemSounds.Exclamation,
			MessageBoxIcon.Information => SystemSounds.Asterisk,
			_ => SystemSounds.Beep
		};
		sound.Play();
	}


	/// <inheritdoc/>
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if(!_CanClose && (DialogResult == DialogResult.None) && (e.CloseReason == CloseReason.UserClosing)) { e.Cancel = true; }

		base.OnFormClosing(e);
	}


	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_Footer.Paint -= _Footer_Paint;
			_IconImage?.Dispose();
		}

		base.Dispose(disposing);
	}
}
