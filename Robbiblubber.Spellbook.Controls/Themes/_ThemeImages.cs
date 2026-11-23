namespace Robbiblubber.Spellbook.Controls.Themes;

using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;



/// <summary>This class creates the images for dark themes.</summary>
/// <remarks><para>Single-colored dark images are inverted. In other images, dark outlines (dark gray or black pixels and very
///          dark colors) are lightened and drawn a little more opaque, so that they stay visible on a dark background; the colors of
///          the image are kept.</para>
///          <para>An image that has been composed of an image and an overlay (see <see cref="_RegisterOverlay"/>) is not adjusted as a
///          whole: both parts are resolved for the theme on their own and composed again.</para></remarks>
[SupportedOSPlatform("windows")]
internal static class _ThemeImages
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Overlay                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class holds the parts of an image that has been composed of an image and an overlay.</summary>
	private sealed class _Overlay
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="image">Image.</param>
		/// <param name="overlay">Overlay drawn over the image.</param>
		public _Overlay(Image image, Image overlay)
		{
			Image = image;
			Overlay = overlay;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the image.</summary>
		public Image Image
		{
			get;
		}


		/// <summary>Gets the overlay.</summary>
		public Image Overlay
		{
			get;
		}


		/// <summary>Gets or sets the resolved image the cached composition has been made of.</summary>
		public Image? ResolvedImage
		{
			get; set;
		}


		/// <summary>Gets or sets the resolved overlay the cached composition has been made of.</summary>
		public Image? ResolvedOverlay
		{
			get; set;
		}


		/// <summary>Gets or sets the cached composition of the resolved parts.</summary>
		public Bitmap? Composition
		{
			get; set;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Maximum average saturation of a single-colored image.</summary>
	private const float _MAX_SATURATION = 0.2f;

	/// <summary>Maximum average brightness of a dark image.</summary>
	private const float _MAX_BRIGHTNESS = 0.45f;

	/// <summary>Maximum lightness of an outline pixel.</summary>
	private const float _OUTLINE_LIGHTNESS = 0.5f;

	/// <summary>Maximum saturation of a gray outline pixel.</summary>
	private const float _OUTLINE_SATURATION = 0.3f;

	/// <summary>Maximum lightness of a colored outline pixel.</summary>
	private const float _OUTLINE_DARK = 0.2f;

	/// <summary>Lightness of a black outline pixel in the light variant.</summary>
	private const float _OUTLINE_BASE = 0.72f;

	/// <summary>Factor of the lightness of an outline pixel that is added to its lightness in the light variant.</summary>
	private const float _OUTLINE_FACTOR = 0.3f;

	/// <summary>Factor of the opacity of an outline pixel in the light variant.</summary>
	private const float _OUTLINE_OPACITY = 1.4f;

	/// <summary>Maximum size of images that are processed.</summary>
	private const int _MAX_SIZE = 256;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Light images by original image; the original image itself if it does not need a light variant.</summary>
	private static readonly ConditionalWeakTable<Image, Image> _LightImages = new();

	/// <summary>Parts of composed images by composed image.</summary>
	private static readonly ConditionalWeakTable<Image, _Overlay> _Overlays = new();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates the light variant of an image.</summary>
	/// <param name="image">Image.</param>
	/// <returns>Returns the light variant, or the image itself if it is neither a single-colored dark image nor has dark outlines.</returns>
	private static Image _CreateLightImage(Image image)
	{
		if((image.Width > _MAX_SIZE) || (image.Height > _MAX_SIZE) || (image.Width == 0) || (image.Height == 0)) { return image; }

		try
		{
			Bitmap rval = new(image.Width, image.Height, PixelFormat.Format32bppArgb);
			using(Graphics g = Graphics.FromImage(rval)) { g.DrawImage(image, 0, 0, image.Width, image.Height); }

			float saturation = 0, brightness = 0;
			int count = 0;
			for(int y = 0; y < rval.Height; y++)
			{
				for(int x = 0; x < rval.Width; x++)
				{
					Color c = rval.GetPixel(x, y);
					if(c.A < 64) { continue; }

					saturation += c.GetSaturation();
					brightness += c.GetBrightness();
					count++;
				}
			}

			if(count == 0)
			{
				rval.Dispose();
				return image;
			}
			if(((saturation / count) > _MAX_SATURATION) || ((brightness / count) > _MAX_BRIGHTNESS))
			{
				if(_LightenOutlines(rval)) { return rval; }

				rval.Dispose();
				return image;
			}

			for(int y = 0; y < rval.Height; y++)
			{
				for(int x = 0; x < rval.Width; x++)
				{
					Color c = rval.GetPixel(x, y);
					if(c.A == 0) { continue; }

					rval.SetPixel(x, y, _Invert(c));
				}
			}
			return rval;
		}
		catch(Exception) { return image; }
	}


	/// <summary>Lightens the dark outlines of an image.</summary>
	/// <param name="image">Image, changed in place.</param>
	/// <returns>Returns TRUE if pixels have been changed, otherwise FALSE.</returns>
	private static bool _LightenOutlines(Bitmap image)
	{
		bool rval = false;

		for(int y = 0; y < image.Height; y++)
		{
			for(int x = 0; x < image.Width; x++)
			{
				Color c = image.GetPixel(x, y);
				float s = c.GetSaturation(), l = c.GetBrightness();
				if((c.A == 0) || (l >= _OUTLINE_LIGHTNESS) || ((s >= _OUTLINE_SATURATION) && (l >= _OUTLINE_DARK))) { continue; }

				int alpha = Math.Min(255, (int) Math.Round(c.A * _OUTLINE_OPACITY));
				image.SetPixel(x, y, _FromHsl(alpha, c.GetHue() / 360f, s, _OUTLINE_BASE + (_OUTLINE_FACTOR * l)));
				rval = true;
			}
		}
		return rval;
	}


	/// <summary>Creates a color from HSL values.</summary>
	/// <param name="alpha">Alpha value from 0 to 255.</param>
	/// <param name="h">Hue from 0 to 1.</param>
	/// <param name="s">Saturation from 0 to 1.</param>
	/// <param name="l">Lightness from 0 to 1.</param>
	/// <returns>Returns the color.</returns>
	private static Color _FromHsl(int alpha, float h, float s, float l)
	{
		float q = ((l < 0.5f) ? (l * (1f + s)) : (l + s - (l * s)));
		float p = (2f * l) - q;

		return Color.FromArgb(alpha, _Channel(p, q, h + (1f / 3f)), _Channel(p, q, h), _Channel(p, q, h - (1f / 3f)));
	}


	/// <summary>Gets a color channel from HSL values.</summary>
	/// <param name="p">First helper value.</param>
	/// <param name="q">Second helper value.</param>
	/// <param name="t">Hue offset.</param>
	/// <returns>Returns the channel value from 0 to 255.</returns>
	private static int _Channel(float p, float q, float t)
	{
		if(t < 0) { t += 1; }
		if(t > 1) { t -= 1; }

		float rval = p;
		if(t < (1f / 6f)) { rval = p + ((q - p) * 6f * t); }
		else if(t < 0.5f) { rval = q; }
		else if(t < (2f / 3f)) { rval = p + ((q - p) * ((2f / 3f) - t) * 6f); }

		return Math.Clamp((int) Math.Round(rval * 255f), 0, 255);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Inverts the brightness of a color and keeps its hue and saturation.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the color with inverted brightness.</returns>
	internal static Color _Invert(Color color)
	{
		return _FromHsl(color.A, color.GetHue() / 360f, color.GetSaturation(), 1f - color.GetBrightness());
	}


	/// <summary>Gets the light variant of an image for dark themes.</summary>
	/// <param name="image">Image.</param>
	/// <returns>Returns a lightened copy if the image is single-colored and dark or has dark outlines, otherwise the image itself.
	///          Copies are cached and owned by this class.</returns>
	internal static Image _GetLightImage(Image image)
	{
		lock(_LightImages)
		{
			if(_LightImages.TryGetValue(image, out Image? rval)) { return rval; }

			rval = _CreateLightImage(image);
			_LightImages.AddOrUpdate(image, rval);
			return rval;
		}
	}


	/// <summary>Registers an image that has been composed of an image and an overlay.</summary>
	/// <param name="composition">Composed image.</param>
	/// <param name="image">Image.</param>
	/// <param name="overlay">Overlay drawn over the image.</param>
	/// <remarks>When the composed image is drawn in a themed menu, both parts are resolved for the theme (including
	///          <see cref="ThemeManager.ResolveImage"/>) and composed again.</remarks>
	internal static void _RegisterOverlay(Image composition, Image image, Image overlay)
	{
		lock(_Overlays) { _Overlays.AddOrUpdate(composition, new _Overlay(image, overlay)); }
	}


	/// <summary>Removes a composed image and disposes its cached compositions.</summary>
	/// <param name="composition">Composed image.</param>
	internal static void _RemoveOverlay(Image composition)
	{
		lock(_Overlays)
		{
			if(!_Overlays.TryGetValue(composition, out _Overlay? overlay)) { return; }

			overlay.Composition?.Dispose();
			_Overlays.Remove(composition);
		}
	}


	/// <summary>Resolves a composed image for a theme.</summary>
	/// <param name="source">Object that shows the image.</param>
	/// <param name="composition">Composed image.</param>
	/// <param name="theme">Theme.</param>
	/// <param name="rval">Returns the image to show.</param>
	/// <returns>Returns TRUE if the image is a registered composed image, otherwise FALSE.</returns>
	internal static bool _TryResolveOverlay(object? source, Image composition, Theme theme, [NotNullWhen(true)] out Image? rval)
	{
		_Overlay? overlay;

		lock(_Overlays)
		{
			if(!_Overlays.TryGetValue(composition, out overlay)) { rval = null; return false; }
		}

		Image image = ThemeManager._ResolveImage(source, overlay.Image, theme);
		Image top = ThemeManager._ResolveImage(source, overlay.Overlay, theme);

		lock(_Overlays)
		{
			if((image == overlay.Image) && (top == overlay.Overlay)) { rval = composition; return true; }
			if((overlay.Composition is not null) && (image == overlay.ResolvedImage) && (top == overlay.ResolvedOverlay))
			{
				rval = overlay.Composition;
				return true;
			}

			Bitmap result = new(composition.Width, composition.Height);
			using(Graphics g = Graphics.FromImage(result))
			{
				g.DrawImage(image, 0, 0, composition.Width, composition.Height);
				g.DrawImage(top, 0, 0, composition.Width, composition.Height);
			}

			overlay.Composition?.Dispose();
			overlay.Composition = result;
			overlay.ResolvedImage = image;
			overlay.ResolvedOverlay = top;

			rval = result;
			return true;
		}
	}
}
