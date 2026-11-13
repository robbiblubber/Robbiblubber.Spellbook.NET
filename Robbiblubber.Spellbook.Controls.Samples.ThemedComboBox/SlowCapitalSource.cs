namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;

using System.Runtime.Versioning;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a search source that answers after a delay, like a database or a web service.</summary>
[SupportedOSPlatform("windows")]
public sealed class SlowCapitalSource: IAutoCompletionSource
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Response time in milliseconds.</summary>
	private const int _RESPONSE_TIME = 800;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IAutoCompletionSource                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public async ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken)
	{
		await Task.Delay(_RESPONSE_TIME, cancellationToken);

		return new AutoCompletionResult(MainWindow._Capitals.Select(m => m.Split('|')).Where(m => m[0].Contains(context.Word, StringComparison.OrdinalIgnoreCase)).Select(m => new AutoCompletionItem(m[0]) { Detail = m[1] }));
	}
}
