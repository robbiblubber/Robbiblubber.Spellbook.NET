namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

using ButtonSlot = Robbiblubber.Spellbook.Controls.ThemedButtonColors._Slot;
using GroupSlot = Robbiblubber.Spellbook.Controls.ThemedGroupBoxColors._Slot;
using TabSlot = Robbiblubber.Spellbook.Controls.ThemedTabControlColors._Slot;
using ToggleSlot = Robbiblubber.Spellbook.Controls.ThemedToggleColors._Slot;



/// <summary>This class draws buttons, check boxes, radio buttons, group boxes and tab controls with theme colors.</summary>
/// <remarks>The themed controls use the renderer in their <c>OnPaint</c> method; the theme adapters use it to paint the standard
///          controls over in their <c>Paint</c> event, so both look the same.</remarks>
[SupportedOSPlatform("windows")]
internal static class _ControlRenderer
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Logical corner radius of buttons and group boxes.</summary>
	private const float _RADIUS = 4f;

	/// <summary>Logical corner radius of check boxes.</summary>
	private const float _BOX_RADIUS = 3f;

	/// <summary>Logical size of the box or circle of check boxes and radio buttons.</summary>
	private const int _GLYPH_SIZE = 13;

	/// <summary>Logical gap between the box or circle and the text.</summary>
	private const int _GLYPH_GAP = 4;

	/// <summary>Width of a switch at 96 DPI.</summary>
	private const int _SWITCH_WIDTH = 40;

	/// <summary>Height of a switch at 96 DPI.</summary>
	private const int _SWITCH_HEIGHT = 20;

	/// <summary>Logical inner padding of buttons.</summary>
	private const int _BUTTON_PADDING = 4;

	/// <summary>Logical gap between image and text.</summary>
	private const int _IMAGE_GAP = 4;

	/// <summary>Logical indent of the group box caption.</summary>
	private const int _CAPTION_INDENT = 8;

	/// <summary>Logical horizontal padding of the content of a tab.</summary>
	private const int _TAB_PADDING = 4;

	/// <summary>Logical thickness of the accent line of the selected tab.</summary>
	private const float _TAB_LINE = 2f;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts a logical value to device units.</summary>
	/// <param name="control">Control.</param>
	/// <param name="value">Logical value.</param>
	/// <returns>Returns the value in device units.</returns>
	private static float _Scale(Control control, float value)
	{
		return (value * control.DeviceDpi / 96f);
	}


	/// <summary>Creates the path of a rounded rectangle.</summary>
	/// <param name="bounds">Bounds.</param>
	/// <param name="radius">Corner radius.</param>
	/// <returns>Returns the path.</returns>
	private static GraphicsPath _RoundedRect(RectangleF bounds, float radius)
	{
		GraphicsPath rval = new();
		float d = Math.Min(radius * 2f, Math.Min(bounds.Width, bounds.Height));

		if(d <= 1f)
		{
			rval.AddRectangle(bounds);
			return rval;
		}

		rval.AddArc(bounds.Left, bounds.Top, d, d, 180f, 90f);
		rval.AddArc(bounds.Right - d, bounds.Top, d, d, 270f, 90f);
		rval.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0f, 90f);
		rval.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90f, 90f);
		rval.CloseFigure();

		return rval;
	}


	/// <summary>Mirrors the horizontal part of an alignment.</summary>
	/// <param name="align">Alignment.</param>
	/// <returns>Returns the mirrored alignment.</returns>
	private static ContentAlignment _Mirror(ContentAlignment align)
	{
		return align switch
		{
			ContentAlignment.TopLeft => ContentAlignment.TopRight,
			ContentAlignment.TopRight => ContentAlignment.TopLeft,
			ContentAlignment.MiddleLeft => ContentAlignment.MiddleRight,
			ContentAlignment.MiddleRight => ContentAlignment.MiddleLeft,
			ContentAlignment.BottomLeft => ContentAlignment.BottomRight,
			ContentAlignment.BottomRight => ContentAlignment.BottomLeft,
			_ => align
		};
	}


	/// <summary>Gets the effective alignment of a control.</summary>
	/// <param name="control">Control.</param>
	/// <param name="align">Alignment.</param>
	/// <returns>Returns the alignment, mirrored for right-to-left layout.</returns>
	private static ContentAlignment _Effective(Control control, ContentAlignment align)
	{
		return ((control.RightToLeft == RightToLeft.Yes) ? _Mirror(align) : align);
	}


	/// <summary>Returns if an alignment is on the left.</summary>
	/// <param name="align">Alignment.</param>
	/// <returns>Returns <c>true</c> for left alignments.</returns>
	private static bool _IsLeft(ContentAlignment align)
	{
		return ((align == ContentAlignment.TopLeft) || (align == ContentAlignment.MiddleLeft) || (align == ContentAlignment.BottomLeft));
	}


	/// <summary>Returns if an alignment is on the right.</summary>
	/// <param name="align">Alignment.</param>
	/// <returns>Returns <c>true</c> for right alignments.</returns>
	private static bool _IsRight(ContentAlignment align)
	{
		return ((align == ContentAlignment.TopRight) || (align == ContentAlignment.MiddleRight) || (align == ContentAlignment.BottomRight));
	}


	/// <summary>Places a size in a rectangle.</summary>
	/// <param name="size">Size.</param>
	/// <param name="bounds">Rectangle.</param>
	/// <param name="align">Alignment.</param>
	/// <returns>Returns the placed rectangle.</returns>
	private static Rectangle _Place(Size size, Rectangle bounds, ContentAlignment align)
	{
		int x = (_IsLeft(align) ? bounds.Left : (_IsRight(align) ? (bounds.Right - size.Width) : (bounds.Left + ((bounds.Width - size.Width) / 2))));
		int y = align switch
		{
			ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => bounds.Top,
			ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => (bounds.Bottom - size.Height),
			_ => (bounds.Top + ((bounds.Height - size.Height) / 2))
		};

		return new Rectangle(x, y, size.Width, size.Height);
	}


	/// <summary>Gets the text format flags for an alignment.</summary>
	/// <param name="align">Alignment.</param>
	/// <param name="wordBreak">Determines if the text is wrapped.</param>
	/// <param name="useMnemonic">Determines if ampersands mark mnemonics.</param>
	/// <param name="states">States.</param>
	/// <returns>Returns the flags.</returns>
	private static TextFormatFlags _GetFlags(ContentAlignment align, bool wordBreak, bool useMnemonic, _PaintStates states)
	{
		TextFormatFlags rval = (wordBreak ? (TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl) : TextFormatFlags.SingleLine);

		if(!useMnemonic) { rval |= TextFormatFlags.NoPrefix; }
		else if(states.HasFlag(_PaintStates.HIDE_PREFIX)) { rval |= TextFormatFlags.HidePrefix; }

		rval |= (_IsLeft(align) ? TextFormatFlags.Left : (_IsRight(align) ? TextFormatFlags.Right : TextFormatFlags.HorizontalCenter));
		rval |= align switch
		{
			ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => TextFormatFlags.Top,
			ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => TextFormatFlags.Bottom,
			_ => TextFormatFlags.VerticalCenter
		};

		return rval;
	}


	/// <summary>Fills the background of a control.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Control.</param>
	/// <param name="color">Back color; if it is transparent, the parent background is drawn.</param>
	private static void _FillBackground(Graphics g, Control control, Color color)
	{
		if((color.A < 255) && (control.Parent is not null)) { ButtonRenderer.DrawParentBackground(g, control.ClientRectangle, control); }
		if(color.A == 0) { return; }

		using SolidBrush brush = new(color);
		g.FillRectangle(brush, control.ClientRectangle);
	}


	/// <summary>Draws an image.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Control.</param>
	/// <param name="image">Image.</param>
	/// <param name="bounds">Bounds.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="back">Back color, used for disabled images.</param>
	private static void _DrawImage(Graphics g, Control control, Image image, Rectangle bounds, Theme theme, Color back)
	{
		Image themed = ThemeManager._ResolveImage(control, image, theme);

		if(control.Enabled) { g.DrawImage(themed, bounds); }
		else { ControlPaint.DrawImageDisabled(g, themed, bounds.X, bounds.Y, back); }
	}


	/// <summary>Draws the content of a button: image and text.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Button.</param>
	/// <param name="bounds">Content area.</param>
	/// <param name="textColor">Text color.</param>
	/// <param name="back">Back color.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="states">States.</param>
	private static void _DrawContent(Graphics g, ButtonBase control, Rectangle bounds, Color textColor, Color back, Theme theme, _PaintStates states)
	{
		Image? image = control.Image;
		string text = (control.Text ?? string.Empty);
		ContentAlignment textAlign = _Effective(control, control.TextAlign);
		ContentAlignment imageAlign = _Effective(control, control.ImageAlign);
		TextFormatFlags flags = _GetFlags(textAlign, true, control.UseMnemonic, states);

		if(image is null)
		{
			if(text.Length > 0) { TextRenderer.DrawText(g, text, control.Font, bounds, textColor, flags); }
			return;
		}

		Size imageSize = image.Size;
		TextImageRelation relation = control.TextImageRelation;
		if(control.RightToLeft == RightToLeft.Yes)
		{
			if(relation == TextImageRelation.ImageBeforeText) { relation = TextImageRelation.TextBeforeImage; }
			else if(relation == TextImageRelation.TextBeforeImage) { relation = TextImageRelation.ImageBeforeText; }
		}

		if((text.Length == 0) || (relation == TextImageRelation.Overlay))
		{
			_DrawImage(g, control, image, _Place(imageSize, bounds, imageAlign), theme, back);
			if(text.Length > 0) { TextRenderer.DrawText(g, text, control.Font, bounds, textColor, flags); }
			return;
		}

		int gap = (int) _Scale(control, _IMAGE_GAP);
		bool horizontal = ((relation == TextImageRelation.ImageBeforeText) || (relation == TextImageRelation.TextBeforeImage));
		Size available = (horizontal ? new Size(Math.Max(1, bounds.Width - imageSize.Width - gap), bounds.Height) : new Size(bounds.Width, Math.Max(1, bounds.Height - imageSize.Height - gap)));
		Size textSize = TextRenderer.MeasureText(g, text, control.Font, available, flags);
		textSize = new Size(Math.Min(textSize.Width, available.Width), Math.Min(textSize.Height, available.Height));

		Size block = (horizontal ? new Size(imageSize.Width + gap + textSize.Width, Math.Max(imageSize.Height, textSize.Height)) : new Size(Math.Max(imageSize.Width, textSize.Width), imageSize.Height + gap + textSize.Height));
		Rectangle area = _Place(block, bounds, textAlign);
		Rectangle imageBounds, textBounds;

		switch(relation)
		{
			case TextImageRelation.ImageBeforeText:
				imageBounds = new Rectangle(area.Left, area.Top + ((area.Height - imageSize.Height) / 2), imageSize.Width, imageSize.Height);
				textBounds = new Rectangle(imageBounds.Right + gap, area.Top, textSize.Width, area.Height);
				break;
			case TextImageRelation.TextBeforeImage:
				textBounds = new Rectangle(area.Left, area.Top, textSize.Width, area.Height);
				imageBounds = new Rectangle(textBounds.Right + gap, area.Top + ((area.Height - imageSize.Height) / 2), imageSize.Width, imageSize.Height);
				break;
			case TextImageRelation.ImageAboveText:
				imageBounds = new Rectangle(area.Left + ((area.Width - imageSize.Width) / 2), area.Top, imageSize.Width, imageSize.Height);
				textBounds = new Rectangle(area.Left, imageBounds.Bottom + gap, area.Width, textSize.Height);
				break;
			default:
				textBounds = new Rectangle(area.Left, area.Top, area.Width, textSize.Height);
				imageBounds = new Rectangle(area.Left + ((area.Width - imageSize.Width) / 2), textBounds.Bottom + gap, imageSize.Width, imageSize.Height);
				break;
		}

		_DrawImage(g, control, image, imageBounds, theme, back);
		TextRenderer.DrawText(g, text, control.Font, textBounds, textColor, (flags & ~(TextFormatFlags.Left | TextFormatFlags.Right)) | TextFormatFlags.HorizontalCenter);
	}

	/// <summary>Gets the image of a tab page.</summary>
	/// <param name="control">Tab control.</param>
	/// <param name="page">Tab page.</param>
	/// <returns>Returns the image, NULL if the page has none.</returns>
	private static Image? _GetTabImage(TabControl control, TabPage page)
	{
		if(control.ImageList is not ImageList list) { return null; }

		if((page.ImageIndex >= 0) && (page.ImageIndex < list.Images.Count)) { return list.Images[page.ImageIndex]; }
		if(!string.IsNullOrEmpty(page.ImageKey) && list.Images.ContainsKey(page.ImageKey)) { return list.Images[page.ImageKey]; }

		return null;
	}


	/// <summary>Draws the image and the text of a tab.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Tab control.</param>
	/// <param name="page">Tab page.</param>
	/// <param name="bounds">Bounds of the tab.</param>
	/// <param name="color">Text color.</param>
	/// <param name="back">Back color, used for disabled images.</param>
	/// <param name="theme">Theme, used for images.</param>
	/// <param name="states">States.</param>
	/// <remarks>Tabs on the left side are drawn from bottom to top, tabs on the right side from top to bottom.</remarks>
	private static void _DrawTabContent(Graphics g, TabControl control, TabPage page, Rectangle bounds, Color color, Color back, Theme theme, _PaintStates states)
	{
		bool vertical = ((control.Alignment == TabAlignment.Left) || (control.Alignment == TabAlignment.Right));
		GraphicsState saved = g.Save();
		Rectangle area = bounds;

		if(vertical)
		{
			if(control.Alignment == TabAlignment.Left)
			{
				g.TranslateTransform(bounds.Left, bounds.Bottom);
				g.RotateTransform(-90f);
			}
			else
			{
				g.TranslateTransform(bounds.Right, bounds.Top);
				g.RotateTransform(90f);
			}
			area = new Rectangle(0, 0, bounds.Height, bounds.Width);
		}

		int padding = (int) _Scale(control, _TAB_PADDING);
		area = Rectangle.Inflate(area, -padding, 0);

		Image? image = _GetTabImage(control, page);
		string text = (page.Text ?? string.Empty);
		TextFormatFlags flags = (TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | (states.HasFlag(_PaintStates.HIDE_PREFIX) ? TextFormatFlags.HidePrefix : TextFormatFlags.Default));
		Size textSize = ((text.Length == 0) ? Size.Empty : TextRenderer.MeasureText(g, text, control.Font, new Size(int.MaxValue, int.MaxValue), flags));
		int gap = (((image is null) || (text.Length == 0)) ? 0 : (int) _Scale(control, _IMAGE_GAP));
		int width = Math.Min(area.Width, ((image?.Width ?? 0) + gap + textSize.Width));
		int x = (area.Left + ((area.Width - width) / 2));

		if(image is not null)
		{
			_DrawImage(g, control, image, new Rectangle(x, area.Top + ((area.Height - image.Height) / 2), image.Width, image.Height), theme, back);
			x += (image.Width + gap);
		}

		if(text.Length > 0)
		{
			Rectangle textBounds = Rectangle.FromLTRB(x, area.Top, Math.Max(x, area.Right), area.Bottom);

			if(vertical)
			{
				using StringFormat format = new() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter,
													HotkeyPrefix = (states.HasFlag(_PaintStates.HIDE_PREFIX) ? System.Drawing.Text.HotkeyPrefix.Hide : System.Drawing.Text.HotkeyPrefix.Show),
													FormatFlags = StringFormatFlags.NoWrap };
				using SolidBrush brush = new(color);
				g.DrawString(text, control.Font, brush, textBounds, format);
			}
			else { TextRenderer.DrawText(g, text, control.Font, textBounds, color, flags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis); }
		}

		g.Restore(saved);
	}


	/// <summary>Gets the area of the selected tab, which reaches into the frame of the page area.</summary>
	/// <param name="control">Tab control.</param>
	/// <param name="tab">Bounds of the tab.</param>
	/// <param name="pane">Page area.</param>
	/// <returns>Returns the area.</returns>
	private static Rectangle _GetSelectedTab(TabControl control, Rectangle tab, Rectangle pane)
	{
		return control.Alignment switch
		{
			TabAlignment.Bottom => Rectangle.FromLTRB(tab.Left, pane.Bottom - 1, tab.Right, tab.Bottom),
			TabAlignment.Left => Rectangle.FromLTRB(tab.Left, tab.Top, pane.Left + 1, tab.Bottom),
			TabAlignment.Right => Rectangle.FromLTRB(pane.Right - 1, tab.Top, tab.Right, tab.Bottom),
			_ => Rectangle.FromLTRB(tab.Left, tab.Top, tab.Right, pane.Top + 1)
		};
	}


	/// <summary>Draws the frame of the selected tab: the three sides that are not open to the page area and the accent line.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Tab control.</param>
	/// <param name="tab">Area of the selected tab.</param>
	/// <param name="border">Border color.</param>
	/// <param name="accent">Color of the accent line.</param>
	private static void _DrawSelectedFrame(Graphics g, TabControl control, Rectangle tab, Color border, Color accent)
	{
		int l = tab.Left, t = tab.Top, r = (tab.Right - 1), b = (tab.Bottom - 1);
		int line = Math.Max(1, (int) Math.Round(_Scale(control, _TAB_LINE)));
		Point[] points;
		Rectangle bar;

		switch(control.Alignment)
		{
			case TabAlignment.Bottom:
				points = new Point[] { new(l, t), new(l, b), new(r, b), new(r, t) };
				bar = new Rectangle(l + 1, b - line + 1, Math.Max(0, tab.Width - 2), line);
				break;
			case TabAlignment.Left:
				points = new Point[] { new(r, t), new(l, t), new(l, b), new(r, b) };
				bar = new Rectangle(l, t + 1, line, Math.Max(0, tab.Height - 2));
				break;
			case TabAlignment.Right:
				points = new Point[] { new(l, t), new(r, t), new(r, b), new(l, b) };
				bar = new Rectangle(r - line + 1, t + 1, line, Math.Max(0, tab.Height - 2));
				break;
			default:
				points = new Point[] { new(l, b), new(l, t), new(r, t), new(r, b) };
				bar = new Rectangle(l + 1, t, Math.Max(0, tab.Width - 2), line);
				break;
		}

		using(Pen pen = new(border)) { g.DrawLines(pen, points); }
		using(SolidBrush brush = new(accent)) { g.FillRectangle(brush, bar); }
	}




	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Draws a button face: background, border, focus border, image and text.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Button or check box with button appearance.</param>
	/// <param name="back">Back color.</param>
	/// <param name="border">Border color.</param>
	/// <param name="text">Text color.</param>
	/// <param name="focus">Color of the focus border.</param>
	/// <param name="parentBack">Back color of the parent, visible in the corners.</param>
	/// <param name="theme">Theme, used for images.</param>
	/// <param name="states">States.</param>
	internal static void _PaintButtonFace(Graphics g, ButtonBase control, Color back, Color border, Color text, Color focus, Color parentBack, Theme theme, _PaintStates states)
	{
		_FillBackground(g, control, parentBack);
		if((control.Width < 2) || (control.Height < 2)) { return; }

		SmoothingMode smoothing = g.SmoothingMode;
		PixelOffsetMode offset = g.PixelOffsetMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.PixelOffsetMode = PixelOffsetMode.Half;

		float radius = _Scale(control, _RADIUS);
		RectangleF bounds = new(0.5f, 0.5f, control.Width - 1f, control.Height - 1f);
		using(GraphicsPath path = _RoundedRect(bounds, radius))
		{
			using(SolidBrush brush = new(back)) { g.FillPath(brush, path); }
			using(Pen pen = new(border)) { g.DrawPath(pen, path); }
		}

		if(states.HasFlag(_PaintStates.FOCUSED))
		{
			float inset = _Scale(control, 2f);
			RectangleF inner = RectangleF.Inflate(bounds, -inset, -inset);
			using GraphicsPath path = _RoundedRect(inner, Math.Max(1f, radius - inset));
			using Pen pen = new(focus);
			g.DrawPath(pen, path);
		}
		g.SmoothingMode = smoothing;
		g.PixelOffsetMode = offset;

		int padding = (int) _Scale(control, _BUTTON_PADDING);
		Rectangle content = new(padding + control.Padding.Left, padding + control.Padding.Top,
								control.Width - (2 * padding) - control.Padding.Horizontal, control.Height - (2 * padding) - control.Padding.Vertical);
		if((content.Width > 0) && (content.Height > 0)) { _DrawContent(g, control, content, text, back, theme, states); }
	}


	/// <summary>Draws a button with the colors of a color table.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Button.</param>
	/// <param name="colors">Color table.</param>
	/// <param name="parentBack">Back color of the parent, visible in the corners.</param>
	/// <param name="states">States.</param>
	internal static void _PaintButton(Graphics g, ButtonBase control, ThemedButtonColors colors, Color parentBack, _PaintStates states)
	{
		Color back, border, text;

		if(!control.Enabled)
		{
			back = colors._Get(ButtonSlot.DISABLED_BACK_COLOR);
			border = colors._Get(ButtonSlot.DISABLED_BORDER_COLOR);
			text = colors._Get(ButtonSlot.DISABLED_TEXT_COLOR);
			states &= ~(_PaintStates.FOCUSED | _PaintStates.HOT | _PaintStates.PRESSED);
		}
		else
		{
			back = (states.HasFlag(_PaintStates.PRESSED) ? colors._Get(ButtonSlot.PRESSED_BACK_COLOR) : (states.HasFlag(_PaintStates.HOT) ? colors._Get(ButtonSlot.HOT_BACK_COLOR) : colors._Get(ButtonSlot.BACK_COLOR)));
			border = (states.HasFlag(_PaintStates.DEFAULT) ? colors._Get(ButtonSlot.DEFAULT_BORDER_COLOR) : colors._Get(ButtonSlot.BORDER_COLOR));
			text = colors._Get(ButtonSlot.TEXT_COLOR);
		}

		_PaintButtonFace(g, control, back, border, text, colors._Get(ButtonSlot.FOCUS_COLOR), parentBack, colors.EffectiveTheme, states);
	}


	/// <summary>Draws a check box or radio button.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Check box or radio button.</param>
	/// <param name="colors">Color table.</param>
	/// <param name="check">Check state.</param>
	/// <param name="radio">Determines if a radio button is drawn.</param>
	/// <param name="appearance">Appearance.</param>
	/// <param name="checkAlign">Alignment of the box or circle.</param>
	/// <param name="states">States.</param>
	/// <param name="switchPosition">Position of the knob for a check box drawn as switch, from 0 (off) to 1 (on); NULL for a box.</param>
	internal static void _PaintToggle(Graphics g, ButtonBase control, ThemedToggleColors colors, CheckState check, bool radio, Appearance appearance, ContentAlignment checkAlign, _PaintStates states, float? switchPosition = null)
	{
		bool enabled = control.Enabled;
		bool isChecked = (check != CheckState.Unchecked);
		Theme theme = colors.EffectiveTheme;
		if(!enabled) { states &= ~(_PaintStates.FOCUSED | _PaintStates.HOT | _PaintStates.PRESSED); }

		if(appearance == Appearance.Button)
		{
			Color back, border, face;
			if(!enabled)
			{
				back = (isChecked ? colors._Get(ToggleSlot.DISABLED_CHECKED_BACK_COLOR) : theme[ThemeColor.DISABLED_BACK]);
				border = theme[ThemeColor.DISABLED_BORDER];
				face = (isChecked ? colors._Get(ToggleSlot.DISABLED_CHECK_MARK_COLOR) : colors._Get(ToggleSlot.DISABLED_TEXT_COLOR));
			}
			else if(isChecked)
			{
				back = (states.HasFlag(_PaintStates.PRESSED) ? colors._Get(ToggleSlot.CHECKED_PRESSED_BACK_COLOR) : (states.HasFlag(_PaintStates.HOT) ? colors._Get(ToggleSlot.CHECKED_HOT_BACK_COLOR) : colors._Get(ToggleSlot.CHECKED_BACK_COLOR)));
				border = back;
				face = colors._Get(ToggleSlot.CHECK_MARK_COLOR);
			}
			else
			{
				back = (states.HasFlag(_PaintStates.PRESSED) ? theme[ThemeColor.BUTTON_BACK_PRESSED] : (states.HasFlag(_PaintStates.HOT) ? theme[ThemeColor.BUTTON_BACK_HOT] : theme[ThemeColor.BUTTON_BACK]));
				border = theme[ThemeColor.BUTTON_BORDER];
				face = colors._Get(ToggleSlot.TEXT_COLOR);
			}

			_PaintButtonFace(g, control, back, border, face, colors._Get(ToggleSlot.FOCUS_COLOR), control.BackColor, theme, states);
			return;
		}

		_FillBackground(g, control, control.BackColor);

		int size = (int) Math.Round(_Scale(control, _GLYPH_SIZE));
		int gap = (int) Math.Round(_Scale(control, _GLYPH_GAP));
		Padding padding = control.Padding;
		Rectangle inner = new(padding.Left, padding.Top, Math.Max(0, control.Width - padding.Horizontal), Math.Max(0, control.Height - padding.Vertical));
		ContentAlignment align = _Effective(control, checkAlign);
		Size glyphSize = (switchPosition.HasValue ? _GetSwitchSize(control) : new Size(size, size));
		Rectangle glyph = _Place(glyphSize, inner, align);

		Rectangle textBounds = inner;
		if(_IsLeft(align)) { textBounds = Rectangle.FromLTRB(glyph.Right + gap, inner.Top, inner.Right, inner.Bottom); }
		else if(_IsRight(align)) { textBounds = Rectangle.FromLTRB(inner.Left, inner.Top, glyph.Left - gap, inner.Bottom); }

		_GetGlyphColors(colors, control.BackColor, enabled, check, states, out Color boxBack, out Color boxBorder, out Color mark);
		if(switchPosition.HasValue) { _DrawSwitch(g, control, glyph, check, ((control.RightToLeft == RightToLeft.Yes) ? (1f - switchPosition.Value) : switchPosition.Value), enabled && states.HasFlag(_PaintStates.HOT), boxBack, boxBorder, mark); }
		else { _DrawGlyph(g, control, glyph, check, radio, boxBack, boxBorder, mark); }

		Image? image = control.Image;
		if(image is not null) { _DrawImage(g, control, image, _Place(image.Size, textBounds, _Effective(control, control.ImageAlign)), theme, control.BackColor); }

		string text = (control.Text ?? string.Empty);
		if((text.Length == 0) || (textBounds.Width <= 0)) { return; }

		TextFormatFlags flags = _GetFlags(_Effective(control, control.TextAlign), !control.AutoSize, control.UseMnemonic, states) | TextFormatFlags.NoClipping;
		Color textColor = (enabled ? colors._Get(ToggleSlot.TEXT_COLOR) : colors._Get(ToggleSlot.DISABLED_TEXT_COLOR));
		TextRenderer.DrawText(g, text, control.Font, textBounds, textColor, flags);

		if(states.HasFlag(_PaintStates.FOCUSED))
		{
			Size measured = TextRenderer.MeasureText(g, text, control.Font, textBounds.Size, flags);
			Rectangle focus = _Place(new Size(Math.Min(measured.Width, textBounds.Width), Math.Min(measured.Height, textBounds.Height)), textBounds, _Effective(control, control.TextAlign));
			focus.Inflate(1, 0);

			using Pen pen = new(colors._Get(ToggleSlot.FOCUS_COLOR));
			g.DrawRectangle(pen, focus.X, focus.Y, focus.Width - 1, focus.Height - 1);
		}
	}


	/// <summary>Gets the colors of the box or circle of a check box or radio button.</summary>
	/// <param name="colors">Color table.</param>
	/// <param name="back">Back color of the control.</param>
	/// <param name="enabled">Determines if the control is enabled.</param>
	/// <param name="check">Check state.</param>
	/// <param name="states">States.</param>
	/// <param name="boxBack">Returns the back color of the box.</param>
	/// <param name="boxBorder">Returns the border color of the box.</param>
	/// <param name="mark">Returns the color of the check mark.</param>
	internal static void _GetGlyphColors(ThemedToggleColors colors, Color back, bool enabled, CheckState check, _PaintStates states, out Color boxBack, out Color boxBorder, out Color mark)
	{
		bool isChecked = (check != CheckState.Unchecked);

		if(!enabled)
		{
			boxBack = (isChecked ? colors._Get(ToggleSlot.DISABLED_CHECKED_BACK_COLOR) : back);
			boxBorder = (isChecked ? boxBack : colors._Get(ToggleSlot.DISABLED_BOX_BORDER_COLOR));
			mark = colors._Get(ToggleSlot.DISABLED_CHECK_MARK_COLOR);
		}
		else if(isChecked)
		{
			boxBack = (states.HasFlag(_PaintStates.PRESSED) ? colors._Get(ToggleSlot.CHECKED_PRESSED_BACK_COLOR) : (states.HasFlag(_PaintStates.HOT) ? colors._Get(ToggleSlot.CHECKED_HOT_BACK_COLOR) : colors._Get(ToggleSlot.CHECKED_BACK_COLOR)));
			boxBorder = boxBack;
			mark = colors._Get(ToggleSlot.CHECK_MARK_COLOR);
		}
		else
		{
			boxBack = (states.HasFlag(_PaintStates.PRESSED) ? colors._Get(ToggleSlot.PRESSED_BOX_BACK_COLOR) : (states.HasFlag(_PaintStates.HOT) ? colors._Get(ToggleSlot.HOT_BOX_BACK_COLOR) : colors._Get(ToggleSlot.BOX_BACK_COLOR)));
			boxBorder = colors._Get(ToggleSlot.BOX_BORDER_COLOR);
			mark = colors._Get(ToggleSlot.CHECK_MARK_COLOR);
		}
	}


	/// <summary>Gets the size of the box of a check box.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the size in pixels.</returns>
	internal static int _GetGlyphSize(Control control)
	{
		return (int) Math.Round(_Scale(control, _GLYPH_SIZE));
	}


	/// <summary>Gets the size of a switch.</summary>
	/// <param name="control">Control.</param>
	/// <returns>Returns the size in pixels.</returns>
	internal static Size _GetSwitchSize(Control control)
	{
		return new Size((int) Math.Round(_Scale(control, _SWITCH_WIDTH)), (int) Math.Round(_Scale(control, _SWITCH_HEIGHT)));
	}


	/// <summary>Draws a check box as switch: a rounded track with a knob.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Control.</param>
	/// <param name="bounds">Bounds of the switch.</param>
	/// <param name="check">Check state; the track has the colors of a checked box unless it is unchecked.</param>
	/// <param name="position">Position of the knob, from 0 (left) to 1 (right).</param>
	/// <param name="hot">Determines if the mouse is over the control; the knob is a little larger then.</param>
	/// <param name="trackBack">Back color of the track.</param>
	/// <param name="trackBorder">Border color of the track.</param>
	/// <param name="mark">Color of the knob of a checked switch.</param>
	internal static void _DrawSwitch(Graphics g, Control control, Rectangle bounds, CheckState check, float position, bool hot, Color trackBack, Color trackBorder, Color mark)
	{
		SmoothingMode smoothing = g.SmoothingMode;
		PixelOffsetMode offset = g.PixelOffsetMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.PixelOffsetMode = PixelOffsetMode.Half;

		RectangleF track = new(bounds.X + 0.5f, bounds.Y + 0.5f, bounds.Width - 1f, bounds.Height - 1f);
		using(GraphicsPath path = _RoundedRect(track, track.Height / 2f))
		{
			using(SolidBrush brush = new(trackBack)) { g.FillPath(brush, path); }
			using(Pen pen = new(trackBorder)) { g.DrawPath(pen, path); }
		}

		position = Math.Clamp(position, 0f, 1f);
		float knob = (track.Height * (hot ? 0.7f : 0.6f));
		float margin = ((track.Height - knob) / 2f);
		float x = track.X + margin + ((track.Width - knob - (2f * margin)) * position);
		float y = track.Y + margin;

		using(SolidBrush brush = new((check == CheckState.Unchecked) ? trackBorder : mark)) { g.FillEllipse(brush, x, y, knob, knob); }

		g.SmoothingMode = smoothing;
		g.PixelOffsetMode = offset;
	}


	/// <summary>Draws the box of a check box or the circle of a radio button.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Control.</param>
	/// <param name="glyph">Bounds of the box or circle.</param>
	/// <param name="check">Check state.</param>
	/// <param name="radio">Determines if a radio button is drawn.</param>
	/// <param name="boxBack">Back color of the box.</param>
	/// <param name="boxBorder">Border color of the box.</param>
	/// <param name="mark">Color of the check mark.</param>
	internal static void _DrawGlyph(Graphics g, Control control, Rectangle glyph, CheckState check, bool radio, Color boxBack, Color boxBorder, Color mark)
	{
		bool isChecked = (check != CheckState.Unchecked);

		SmoothingMode smoothing = g.SmoothingMode;
		PixelOffsetMode offset = g.PixelOffsetMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.PixelOffsetMode = PixelOffsetMode.Half;

		RectangleF box = new(glyph.X + 0.5f, glyph.Y + 0.5f, glyph.Width - 1f, glyph.Height - 1f);
		if(radio)
		{
			using(SolidBrush brush = new(boxBack)) { g.FillEllipse(brush, box); }
			using(Pen pen = new(boxBorder)) { g.DrawEllipse(pen, box); }

			if(isChecked)
			{
				float dot = (box.Width * 0.42f);
				using SolidBrush brush = new(mark);
				g.FillEllipse(brush, box.X + ((box.Width - dot) / 2f), box.Y + ((box.Height - dot) / 2f), dot, dot);
			}
		}
		else
		{
			using(GraphicsPath path = _RoundedRect(box, _Scale(control, _BOX_RADIUS)))
			{
				using(SolidBrush brush = new(boxBack)) { g.FillPath(brush, path); }
				using(Pen pen = new(boxBorder)) { g.DrawPath(pen, path); }
			}

			if(check == CheckState.Checked)
			{
				using Pen pen = new(mark, Math.Max(1.5f, _Scale(control, 1.6f))) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
				g.DrawLines(pen, new PointF[]
				{
					new(box.X + (box.Width * 0.24f), box.Y + (box.Height * 0.52f)),
					new(box.X + (box.Width * 0.42f), box.Y + (box.Height * 0.70f)),
					new(box.X + (box.Width * 0.76f), box.Y + (box.Height * 0.32f))
				});
			}
			else if(check == CheckState.Indeterminate)
			{
				float height = Math.Max(1.5f, _Scale(control, 1.6f));
				using SolidBrush brush = new(mark);
				g.FillRectangle(brush, box.X + (box.Width * 0.25f), box.Y + ((box.Height - height) / 2f), box.Width * 0.5f, height);
			}
		}
		g.SmoothingMode = smoothing;
		g.PixelOffsetMode = offset;
	}


	/// <summary>Draws an item of a list: background, text and focus rectangle; selected items get the selection colors of the theme.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">List control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="bounds">Bounds of the item.</param>
	/// <param name="textBounds">Bounds of the text.</param>
	/// <param name="text">Text.</param>
	/// <param name="state">State of the item.</param>
	/// <param name="tabs">Determines if tab characters are expanded.</param>
	internal static void _PaintListItem(Graphics g, Control control, Theme theme, Rectangle bounds, Rectangle textBounds, string? text, DrawItemState state, bool tabs)
	{
		bool enabled = (control.Enabled && !state.HasFlag(DrawItemState.Disabled));
		bool selected = (enabled && state.HasFlag(DrawItemState.Selected));
		Color back = (selected ? theme[ThemeColor.SELECTION_BACK] : control.BackColor);
		Color fore = (!enabled ? theme[ThemeColor.DISABLED_TEXT] : (selected ? theme[ThemeColor.SELECTION_TEXT] : control.ForeColor));

		using(SolidBrush brush = new(back)) { g.FillRectangle(brush, bounds); }

		TextFormatFlags flags = (TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
		if(control.RightToLeft == RightToLeft.Yes) { flags |= (TextFormatFlags.Right | TextFormatFlags.RightToLeft); }
		if(tabs) { flags |= TextFormatFlags.ExpandTabs; }

		TextRenderer.DrawText(g, (text ?? string.Empty), control.Font, textBounds, fore, flags);

		if(state.HasFlag(DrawItemState.Focus) && !state.HasFlag(DrawItemState.NoFocusRect)) { ControlPaint.DrawFocusRectangle(g, bounds, fore, back); }
	}


	/// <summary>Draws the buttons of an up-down control.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Button control of the up-down control.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="hot">Button under the mouse: 0 for none, 1 for up, 2 for down.</param>
	/// <param name="pressed">Pressed button: 0 for none, 1 for up, 2 for down.</param>
	internal static void _PaintSpinButtons(Graphics g, Control control, Theme theme, int hot, int pressed)
	{
		bool enabled = (control.Enabled && (control.Parent?.Enabled ?? true));
		Color field = (control.Parent?.BackColor ?? theme[ThemeColor.FIELD_BACK]);
		Color glyph = (enabled ? theme[ThemeColor.GLYPH] : theme[ThemeColor.DISABLED_TEXT]);

		int half = (control.Height / 2);
		Rectangle[] buttons = new Rectangle[] { new(0, 0, control.Width, half), new(0, half, control.Width, control.Height - half) };

		SmoothingMode smoothing = g.SmoothingMode;
		PixelOffsetMode offset = g.PixelOffsetMode;

		for(int i = 0; i < 2; i++)
		{
			Rectangle button = buttons[i];
			Color back = (!enabled ? field : ((pressed == (i + 1)) ? theme[ThemeColor.BUTTON_BACK_PRESSED] : ((hot == (i + 1)) ? theme[ThemeColor.BUTTON_BACK_HOT] : field)));
			using(SolidBrush brush = new(back)) { g.FillRectangle(brush, button); }

			float width = Math.Min(button.Width * 0.5f, _Scale(control, 8f));
			float height = (width / 2f);
			float x = button.X + ((button.Width - width) / 2f);
			float y = button.Y + ((button.Height - height) / 2f);

			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.PixelOffsetMode = PixelOffsetMode.Half;
			using Pen pen = new(glyph, Math.Max(1f, _Scale(control, 1.2f))) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
			g.DrawLines(pen, (i == 0) ? new PointF[] { new(x, y + height), new(x + (width / 2f), y), new(x + width, y + height) }
			                          : new PointF[] { new(x, y), new(x + (width / 2f), y + height), new(x + width, y) });
			g.SmoothingMode = smoothing;
			g.PixelOffsetMode = offset;
		}
	}


	/// <summary>Draws a group box: background, frame and caption.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Group box.</param>
	/// <param name="colors">Color table.</param>
	/// <param name="states">States.</param>
	internal static void _PaintGroupBox(Graphics g, GroupBox control, ThemedGroupBoxColors colors, _PaintStates states)
	{
		_FillBackground(g, control, control.BackColor);

		bool enabled = control.Enabled;
		string text = (control.Text ?? string.Empty);
		TextFormatFlags flags = (TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | (states.HasFlag(_PaintStates.HIDE_PREFIX) ? TextFormatFlags.HidePrefix : TextFormatFlags.Default));
		if(control.RightToLeft == RightToLeft.Yes) { flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft; }

		int indent = (int) _Scale(control, _CAPTION_INDENT);
		Size caption = ((text.Length == 0) ? Size.Empty : TextRenderer.MeasureText(g, text, control.Font, new Size(Math.Max(1, control.Width - (2 * indent)), int.MaxValue), flags));
		caption.Width = Math.Min(caption.Width, Math.Max(0, control.Width - (2 * indent)));
		int top = ((text.Length == 0) ? (control.Font.Height / 2) : (caption.Height / 2));

		if((control.Width > 2) && (control.Height > top + 2))
		{
			SmoothingMode smoothing = g.SmoothingMode;
			PixelOffsetMode offset = g.PixelOffsetMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.PixelOffsetMode = PixelOffsetMode.Half;

			using(GraphicsPath path = _RoundedRect(new RectangleF(0.5f, top + 0.5f, control.Width - 1f, control.Height - top - 1f), _Scale(control, _RADIUS)))
			using(Pen pen = new(enabled ? colors._Get(GroupSlot.BORDER_COLOR) : colors._Get(GroupSlot.DISABLED_BORDER_COLOR)))
			{
				g.DrawPath(pen, path);
			}
			g.SmoothingMode = smoothing;
			g.PixelOffsetMode = offset;
		}

		if(caption.Width <= 0) { return; }

		int x = ((control.RightToLeft == RightToLeft.Yes) ? (control.Width - indent - caption.Width) : indent);
		Rectangle bounds = new(x, 0, caption.Width, caption.Height);
		Rectangle gap = Rectangle.Inflate(bounds, (int) _Scale(control, 2f), 0);

		if(control.BackColor.A == 255)
		{
			using SolidBrush brush = new(control.BackColor);
			g.FillRectangle(brush, gap);
		}
		else
		{
			using Region region = new(gap);
			GraphicsState state = g.Save();
			g.SetClip(region, CombineMode.Replace);
			ButtonRenderer.DrawParentBackground(g, control.ClientRectangle, control);
			g.Restore(state);
		}

		TextRenderer.DrawText(g, text, control.Font, bounds, (enabled ? colors._Get(GroupSlot.TEXT_COLOR) : colors._Get(GroupSlot.DISABLED_TEXT_COLOR)), flags);
	}

	/// <summary>Draws a tab control: background, frame of the page area and tabs.</summary>
	/// <param name="g">Graphics.</param>
	/// <param name="control">Tab control.</param>
	/// <param name="colors">Color table.</param>
	/// <param name="parentBack">Back color of the parent, visible behind the tabs.</param>
	/// <param name="hot">Index of the tab under the mouse, -1 for none.</param>
	/// <param name="states">States; <see cref="_PaintStates.FOCUSED"/> draws a focus border on the selected tab.</param>
	/// <param name="drawItem">Action that draws the content of a tab in owner draw mode (graphics, index, bounds, selected), NULL to
	///                        draw image and text.</param>
	internal static void _PaintTabControl(Graphics g, TabControl control, ThemedTabControlColors colors, Color parentBack, int hot, _PaintStates states, Action<Graphics, int, Rectangle, bool>? drawItem = null)
	{
		_FillBackground(g, control, parentBack);

		bool enabled = control.Enabled;
		bool buttons = (control.Appearance != TabAppearance.Normal);
		int count = control.TabCount;
		int selected = control.SelectedIndex;
		Theme theme = colors.EffectiveTheme;
		TabPage? page = control.SelectedTab;
		Color pageBack = (((page is null) || page.UseVisualStyleBackColor || (page.BackColor.A < 255)) ? colors._Get(TabSlot.PAGE_BACK_COLOR) : page.BackColor);
		Color border = colors._Get(enabled ? TabSlot.BORDER_COLOR : TabSlot.DISABLED_BORDER_COLOR);
		Color text = colors._Get(enabled ? TabSlot.TEXT_COLOR : TabSlot.DISABLED_TEXT_COLOR);

		Rectangle[] tabs = new Rectangle[count];
		Rectangle band = Rectangle.Empty;
		for(int i = 0; i < count; i++)
		{
			tabs[i] = control.GetTabRect(i);
			band = ((i == 0) ? tabs[i] : Rectangle.Union(band, tabs[i]));
		}

		Rectangle client = control.ClientRectangle;
		Rectangle pane = ((count == 0) ? client : control.Alignment switch
		{
			TabAlignment.Bottom => Rectangle.FromLTRB(client.Left, client.Top, client.Right, band.Top),
			TabAlignment.Left => Rectangle.FromLTRB(band.Right, client.Top, client.Right, client.Bottom),
			TabAlignment.Right => Rectangle.FromLTRB(client.Left, client.Top, band.Left, client.Bottom),
			_ => Rectangle.FromLTRB(client.Left, band.Bottom, client.Right, client.Bottom)
		});

		if(!buttons && (pane.Width > 1) && (pane.Height > 1))
		{
			using(SolidBrush brush = new(pageBack)) { g.FillRectangle(brush, pane); }
			using(Pen pen = new(border)) { g.DrawRectangle(pen, pane.X, pane.Y, pane.Width - 1, pane.Height - 1); }
		}

		IEnumerable<int> order = Enumerable.Range(0, count).Where(m => m != selected);
		if((selected >= 0) && (selected < count)) { order = order.Append(selected); }

		foreach(int i in order)
		{
			Rectangle tab = tabs[i];
			bool isSelected = (i == selected);
			bool isHot = (enabled && (i == hot));
			Color back = parentBack;
			Color color = text;

			if(buttons)
			{
				Rectangle face = Rectangle.Inflate(tab, -1, -1);
				Color faceBorder = colors._Get(TabSlot.BUTTON_BORDER_COLOR);
				back = colors._Get(TabSlot.BUTTON_BACK_COLOR);

				if(!enabled) { faceBorder = border; }
				else if(isSelected)
				{
					back = faceBorder = colors._Get(TabSlot.ACCENT_COLOR);
					color = colors._Get(TabSlot.ACCENT_TEXT_COLOR);
				}
				else if(isHot) { back = colors._Get(TabSlot.HOT_BACK_COLOR); }

				if((control.Appearance == TabAppearance.FlatButtons) && !isSelected && !isHot) { back = faceBorder = parentBack; }

				SmoothingMode smoothing = g.SmoothingMode;
				PixelOffsetMode offset = g.PixelOffsetMode;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.PixelOffsetMode = PixelOffsetMode.Half;
				using(GraphicsPath path = _RoundedRect(new RectangleF(face.X + 0.5f, face.Y + 0.5f, face.Width - 1f, face.Height - 1f), _Scale(control, _RADIUS)))
				{
					using(SolidBrush brush = new(back)) { g.FillPath(brush, path); }
					using(Pen pen = new(faceBorder)) { g.DrawPath(pen, path); }
				}
				g.SmoothingMode = smoothing;
				g.PixelOffsetMode = offset;
			}
			else if(isSelected)
			{
				tab = _GetSelectedTab(control, tab, pane);
				back = pageBack;

				using(SolidBrush brush = new(back)) { g.FillRectangle(brush, tab); }
				_DrawSelectedFrame(g, control, tab, border, (enabled ? colors._Get(TabSlot.ACCENT_COLOR) : border));
			}
			else if(isHot)
			{
				back = colors._Get(TabSlot.HOT_BACK_COLOR);
				using SolidBrush brush = new(back);
				g.FillRectangle(brush, Rectangle.Inflate(tabs[i], -1, -1));
			}

			if(drawItem is not null) { drawItem(g, i, tabs[i], isSelected); }
			else { _DrawTabContent(g, control, control.TabPages[i], tabs[i], color, back, theme, states); }

			if(isSelected && states.HasFlag(_PaintStates.FOCUSED))
			{
				int inset = (int) _Scale(control, 3f);
				Rectangle focus = Rectangle.Inflate(tabs[i], -inset, -inset);
				if((focus.Width > 1) && (focus.Height > 1))
				{
					using Pen pen = new(colors._Get(TabSlot.FOCUS_COLOR));
					g.DrawRectangle(pen, focus.X, focus.Y, focus.Width - 1, focus.Height - 1);
				}
			}
		}
	}

}
