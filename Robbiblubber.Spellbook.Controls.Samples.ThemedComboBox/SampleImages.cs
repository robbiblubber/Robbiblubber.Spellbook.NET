namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;

using System.Drawing.Drawing2D;
using System.Runtime.Versioning;



/// <summary>This class creates the sample images.</summary>
[SupportedOSPlatform("windows")]
public static class SampleImages
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Grape image.</summary>
	private static Bitmap? _Grape = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets an image that is assigned with the Image property of an item instead of an image key.</summary>
	public static Image Grape
	{
		get { return _Grape ??= CreateBunch(Color.FromArgb(0x8E, 0x24, 0xAA)); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a fruit image.</summary>
	/// <param name="color">Fruit color.</param>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateFruit(Color color)
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush fill = new(color))
		using(Pen border = new(ControlPaint.Dark(color, 0.2f)))
		using(SolidBrush shine = new(Color.FromArgb(160, Color.White)))
		using(Pen stem = new(Color.FromArgb(0x55, 0x8B, 0x2F), 1.5f))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillEllipse(fill, 1.5f, 3.5f, 12.5f, 11.5f);
			g.DrawEllipse(border, 1.5f, 3.5f, 12.5f, 11.5f);
			g.FillEllipse(shine, 4.5f, 6f, 3.5f, 3f);
			g.DrawLine(stem, 8f, 4f, 10f, 0.5f);
		}

		return rval;
	}


	/// <summary>Creates a banana image.</summary>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateBanana()
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(Pen outline = new(Color.FromArgb(0xC7, 0x8F, 0x00), 5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
		using(Pen fill = new(Color.FromArgb(0xFB, 0xC0, 0x2D), 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
		using(SolidBrush tip = new(Color.FromArgb(0x5D, 0x40, 0x37)))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawArc(outline, 2.5f, -4.5f, 11f, 16f, 20f, 140f);
			g.DrawArc(fill, 2.5f, -4.5f, 11f, 16f, 20f, 140f);
			g.FillEllipse(tip, 12f, 5f, 2.5f, 2.5f);
		}

		return rval;
	}


	/// <summary>Creates a cherry image (two cherries on stems).</summary>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateCherries()
	{
		Bitmap rval = new(16, 16);
		Color color = Color.FromArgb(0xB7, 0x1C, 0x1C);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush fill = new(color))
		using(Pen border = new(ControlPaint.Dark(color, 0.2f)))
		using(SolidBrush shine = new(Color.FromArgb(160, Color.White)))
		using(Pen stem = new(Color.FromArgb(0x55, 0x8B, 0x2F), 1.2f))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawBezier(stem, 4f, 9f, 5f, 4f, 8f, 1.5f, 11f, 0.8f);
			g.DrawBezier(stem, 11.5f, 10f, 11.5f, 5f, 11f, 2f, 11f, 0.8f);
			foreach(RectangleF i in new RectangleF[] { new(0.5f, 7.5f, 7.5f, 7.5f), new(8f, 8f, 7.5f, 7.5f) })
			{
				g.FillEllipse(fill, i);
				g.DrawEllipse(border, i);
				g.FillEllipse(shine, i.X + 1.8f, i.Y + 1.8f, 2f, 2f);
			}
		}

		return rval;
	}


	/// <summary>Creates a lemon image.</summary>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateLemon()
	{
		Bitmap rval = new(16, 16);
		Color color = Color.FromArgb(0xFF, 0xE0, 0x3B);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush fill = new(color))
		using(Pen border = new(Color.FromArgb(0xC0, 0xA0, 0x00)))
		using(SolidBrush shine = new(Color.FromArgb(160, Color.White)))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillEllipse(fill, 0.5f, 7f, 3f, 2f);
			g.FillEllipse(fill, 12.5f, 7f, 3f, 2f);
			g.FillEllipse(fill, 1.5f, 3.5f, 13f, 9f);
			g.DrawEllipse(border, 1.5f, 3.5f, 13f, 9f);
			g.FillEllipse(shine, 4.5f, 5.5f, 3.5f, 2f);
		}

		return rval;
	}


	/// <summary>Creates a grape image (a bunch of berries).</summary>
	/// <param name="color">Berry color.</param>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateBunch(Color color)
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush fill = new(color))
		using(Pen border = new(ControlPaint.Dark(color, 0.2f)))
		using(Pen stem = new(Color.FromArgb(0x55, 0x8B, 0x2F), 1.5f))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawLine(stem, 8f, 3f, 9.5f, 0.5f);
			foreach(PointF i in new PointF[] { new(2f, 2.5f), new(6f, 2.5f), new(10f, 2.5f), new(4f, 6.5f), new(8f, 6.5f), new(6f, 10.5f) })
			{
				g.FillEllipse(fill, i.X, i.Y, 4.5f, 4.5f);
				g.DrawEllipse(border, i.X, i.Y, 4.5f, 4.5f);
			}
		}

		return rval;
	}


	/// <summary>Creates a document image (a page with a folded corner).</summary>
	/// <param name="color">Color of the stripe at the bottom of the page.</param>
	/// <returns>Returns a 16 × 16 image.</returns>
	public static Bitmap CreateDocument(Color color)
	{
		Bitmap rval = new(16, 16);
		PointF[] page = new PointF[] { new(2.5f, 0.5f), new(10f, 0.5f), new(13.5f, 4f), new(13.5f, 15.5f), new(2.5f, 15.5f) };

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush paper = new(Color.White))
		using(SolidBrush stripe = new(color))
		using(Pen border = new(Color.FromArgb(0x80, 0x80, 0x80)))
		using(Pen lines = new(Color.FromArgb(0xB0, 0xB0, 0xB0)))
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillPolygon(paper, page);
			g.FillRectangle(stripe, 3f, 10.5f, 10f, 4.5f);
			g.DrawLine(lines, 4.5f, 4.5f, 9f, 4.5f);
			g.DrawLine(lines, 4.5f, 7f, 11.5f, 7f);
			g.DrawPolygon(border, page);
			g.DrawLines(border, new PointF[] { new(10f, 0.5f), new(10f, 4f), new(13.5f, 4f) });
		}

		return rval;
	}


	/// <summary>Creates the image list with the fruit images.</summary>
	/// <param name="container">Container that disposes the image list.</param>
	/// <returns>Returns the image list.</returns>
	public static ImageList CreateFruitList(System.ComponentModel.IContainer container)
	{
		ImageList rval = new(container) { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(16, 16) };

		rval.Images.Add("apple", CreateFruit(Color.FromArgb(0xD3, 0x2F, 0x2F)));
		rval.Images.Add("banana", CreateBanana());
		rval.Images.Add("cherry", CreateCherries());
		rval.Images.Add("blueberry", CreateFruit(Color.FromArgb(0x39, 0x49, 0xAB)));
		rval.Images.Add("elderberry", CreateBunch(Color.FromArgb(0x4A, 0x14, 0x8C)));
		rval.Images.Add("lemon", CreateLemon());

		return rval;
	}
}
