namespace Robbiblubber.Spellbook.UnitTests.Controls.EditCommands;

using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements an edit command provider whose active control is set by the test.</summary>
/// <remarks>Tests run without visible windows, so no control has the keyboard focus.</remarks>
[SupportedOSPlatform("windows")]
public class TestEditCommandProvider: EditCommandProvider
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets the control that is the target of the commands.</summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Control? Active
	{
		get; set;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EditCommandProvider                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override Control? ActiveControl
	{
		get { return Active; }
	}
}
