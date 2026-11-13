namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class implements the basics scenario.</summary>
/// <remarks>Everything except the sources is set up in the designer: provider, image list and the extender properties of the text boxes.</remarks>
[SupportedOSPlatform("windows")]
public partial class BasicsControl: UserControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public BasicsControl()
	{
		InitializeComponent();

		SampleImages.Fill(_Ilist);

		_AutoCompletion.SetSource(_TextCountry, new AutoCompletionItemSource(SampleData.Countries));
		_AutoCompletion.SetSource(_TextLanguage, new AutoCompletionItemSource(SampleData.CreateLanguageItems()));

		SampleLog.Attach(_AutoCompletion, "Basics");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Click event of the dialog button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonDialog_Click(object? sender, EventArgs e)
	{
		using(DialogWindow dialog = new())
		{
			DialogResult result = dialog.ShowDialog(FindForm());
			SampleLog.Write("Dialog", $"Dialog closed: {result}, city='{dialog.City}'");
		}
	}
}
