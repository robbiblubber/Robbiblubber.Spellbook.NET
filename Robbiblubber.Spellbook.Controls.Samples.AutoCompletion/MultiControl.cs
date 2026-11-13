namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class implements the scenario with several controls on one provider.</summary>
/// <remarks>The first and the third box use the provider-wide source (color names), the second box has its own source (fruits).
///          The <see cref="AutoCompletionProvider.Populating"/> handler decorates the items for the third box with swatches,
///          colors and hex codes; the <see cref="AutoCompletionProvider.Accepting"/> handler appends a separator for the second box.</remarks>
[SupportedOSPlatform("windows")]
public partial class MultiControl: UserControl
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Decorated color items by name.</summary>
	private readonly Dictionary<string, IAutoCompletionItem> _DecoratedItems = new(StringComparer.OrdinalIgnoreCase);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public MultiControl()
	{
		InitializeComponent();

		string[] colors = Enum.GetValues<KnownColor>().Select(m => Color.FromKnownColor(m)).Where(m => !m.IsSystemColor).Select(m => m.Name).Distinct().ToArray();

		_AutoCompletion.Source = new AutoCompletionItemSource(colors);
		_AutoCompletion.SetSource(_TextFruits, new AutoCompletionItemSource(SampleData.Fruits));

		SampleLog.Attach(_AutoCompletion, "Multi");
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a decorated item for a color name.</summary>
	/// <param name="item">Undecorated item.</param>
	/// <returns>Returns the decorated item.</returns>
	private IAutoCompletionItem _Decorate(IAutoCompletionItem item)
	{
		if(!_DecoratedItems.TryGetValue(item.Text, out IAutoCompletionItem? rval))
		{
			Color color = Color.FromName(item.Text);
			bool dark = (color.GetBrightness() < 0.55f);

			rval = new AutoCompletionItem(item.Text, SampleImages.CreateSwatch(color)) { Detail = $"#{color.R:X2}{color.G:X2}{color.B:X2}", ForeColor = (dark ? color : Color.Empty), FontStyle = (dark ? FontStyle.Bold : FontStyle.Regular) };
			_DecoratedItems.Add(item.Text, rval);
		}

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the Click event of the show button.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _ButtonShow_Click(object? sender, EventArgs e)
	{
		_TextColors.Focus();
		_AutoCompletion.ShowCompletion(_TextColors);
	}


	/// <summary>Handles the Populating event of the auto completion provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AutoCompletion_Populating(object? sender, AutoCompletionPopulatingEventArgs e)
	{
		if(e.Context.Control != _TextDecorated) { return; }

		for(int i = 0; i < e.Items.Count; i++) { e.Items[i] = _Decorate(e.Items[i]); }
	}


	/// <summary>Handles the Accepting event of the auto completion provider.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _AutoCompletion_Accepting(object? sender, AutoCompletionAcceptingEventArgs e)
	{
		if(e.Control == _TextFruits) { e.InsertText += ", "; }
	}
}
