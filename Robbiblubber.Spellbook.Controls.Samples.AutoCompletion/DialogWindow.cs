namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class implements a dialog with accept and cancel buttons to test Enter and Escape handling.</summary>
/// <remarks>While the auto completion list is visible, Enter and Escape must be handled by the list. Otherwise, they must trigger the
///          accept and cancel buttons of the dialog.</remarks>
[SupportedOSPlatform("windows")]
public partial class DialogWindow: Form
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public DialogWindow()
	{
		InitializeComponent();

		_AutoCompletion.SetSource(_TextCity, new AutoCompletionItemSource(SampleData.Cities));
		SampleLog.Attach(_AutoCompletion, "Dialog");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the entered city.</summary>
	public string City
	{
		get { return _TextCity.Text; }
	}
}
