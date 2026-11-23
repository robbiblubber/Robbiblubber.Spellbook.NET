namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a panel that provides an editable object through <see cref="IEditableBearing"/>.</summary>
[SupportedOSPlatform("windows")]
public class TestEditableBearing: Panel, IEditableBearing
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditableBearing                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IEditable? Editable
	{
		get; set;
	}
}
