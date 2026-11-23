namespace Robbiblubber.Spellbook.Controls.Samples.RecentFiles;

using System.ComponentModel;
using System.Runtime.Versioning;



/// <summary>This class implements a recent list of database connections.</summary>
/// <remarks>This is an example of a list for entries that are not files: the key is the connection ID, the menu shows the current
///          name of the connection, and dropped connections are shown grayed.</remarks>
[SupportedOSPlatform("windows")]
public class RecentConnectionList: RecentItemList
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Database image, NULL if it has not been created yet.</summary>
	private static Image? _DatabaseImage = null;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public RecentConnectionList(): base()
	{}


	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="container">Container.</param>
	public RecentConnectionList(IContainer container): base(container)
	{}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a database image (a small cylinder), drawn once.</summary>
	/// <returns>Returns the image.</returns>
	private static Image _GetDatabaseImage()
	{
		Bitmap rval;

		if(_DatabaseImage is not null) { return _DatabaseImage; }

		rval = new Bitmap(16, 16);
		using(Graphics g = Graphics.FromImage(rval))
		using(SolidBrush body = new(Color.FromArgb(69, 123, 157)))
		using(SolidBrush top = new(Color.FromArgb(140, 180, 205)))
		using(Pen edge = new(Color.FromArgb(40, 80, 110)))
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.FillRectangle(body, 3, 4, 10, 9);
			g.FillEllipse(body, 3, 10, 10, 5);
			g.FillEllipse(top, 3, 2, 10, 5);
			g.DrawEllipse(edge, 3, 2, 10, 5);
			g.DrawLine(edge, 3, 4, 3, 12);
			g.DrawLine(edge, 13, 4, 13, 12);
			g.DrawArc(edge, 3, 10, 10, 5, 0, 180);
		}

		return (_DatabaseImage = rval);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// protected properties                                                                                             //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override string _DefaultListName
	{
		get { return "connections"; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	[Browsable(false)]
	public override IEqualityComparer<string> KeyComparer
	{
		get { return StringComparer.OrdinalIgnoreCase; }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] RecentItemList                                                                                        //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	protected override string _GetText(RecentItem item)
	{
		return (SampleConnections.GetName(item.Key) ?? item.Text ?? item.Key);
	}


	/// <inheritdoc/>
	protected override string? _GetToolTip(RecentItem item)
	{
		return ("Connection ID: " + item.Key);
	}


	/// <inheritdoc/>
	protected override Image? _GetImage(RecentItem item)
	{
		return _GetDatabaseImage();
	}


	/// <inheritdoc/>
	protected override bool? _IsAvailable(RecentItem item)
	{
		return SampleConnections.Exists(item.Key);
	}
}
