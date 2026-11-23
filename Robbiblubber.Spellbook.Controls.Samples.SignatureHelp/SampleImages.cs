namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;



/// <summary>This class provides images for the sample scenarios.</summary>
/// <remarks>Images are drawn at runtime, so the sample does not need resource files.</remarks>
[SupportedOSPlatform("windows")]
public static class SampleImages
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a 16x16 badge image with a letter.</summary>
	/// <param name="color">Badge color.</param>
	/// <param name="letter">Letter.</param>
	/// <returns>Returns the image.</returns>
	public static Bitmap CreateBadge(Color color, string letter)
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush brush = new(color))
		using(Font font = new("Segoe UI", 7.5f, FontStyle.Bold, GraphicsUnit.Point))
		using(StringFormat format = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

			g.FillEllipse(brush, 1, 1, 14, 14);
			g.DrawString(letter, font, Brushes.White, new RectangleF(0, 0, 16, 16), format);
		}

		return rval;
	}


	/// <summary>Creates a 16x16 color swatch image.</summary>
	/// <param name="color">Color.</param>
	/// <returns>Returns the image.</returns>
	public static Bitmap CreateSwatch(Color color)
	{
		Bitmap rval = new(16, 16);

		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush brush = new(color))
		{
			g.FillRectangle(brush, 2, 2, 12, 12);
			g.DrawRectangle(Pens.Gray, 2, 2, 11, 11);
		}

		return rval;
	}


	/// <summary>Adds the sample images to an image list.</summary>
	/// <param name="imageList">Image list.</param>
	/// <remarks>Available keys: keyword, function, table, column, schema, class, interface, struct, enum, word.</remarks>
	public static void Fill(ImageList imageList)
	{
		imageList.Images.Add("keyword", CreateBadge(Color.RoyalBlue, "K"));
		imageList.Images.Add("function", CreateBadge(Color.MediumPurple, "f"));
		imageList.Images.Add("table", CreateBadge(Color.DarkOrange, "T"));
		imageList.Images.Add("column", CreateBadge(Color.SeaGreen, "c"));
		imageList.Images.Add("schema", CreateBadge(Color.SlateGray, "S"));
		imageList.Images.Add("class", CreateBadge(Color.SteelBlue, "C"));
		imageList.Images.Add("interface", CreateBadge(Color.Teal, "I"));
		imageList.Images.Add("struct", CreateBadge(Color.IndianRed, "V"));
		imageList.Images.Add("enum", CreateBadge(Color.Goldenrod, "E"));
		imageList.Images.Add("word", CreateBadge(Color.Gray, "W"));
	}
}
