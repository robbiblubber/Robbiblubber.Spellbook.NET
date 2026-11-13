namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using static Robbiblubber.Spellbook.Controls._NativeMethods;



/// <summary>This class provides clipboard access that does not fail if the clipboard is unavailable.</summary>
/// <remarks>The clipboard can be held by another application, in that case the <see cref="Clipboard"/> methods throw an
///          <see cref="ExternalException"/>. Edit command queries and executions must not fail for this reason.</remarks>
[SupportedOSPlatform("windows")]
internal static class _Clipboard
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns if the clipboard contains text.</summary>
	/// <returns>Returns <c>true</c> if the clipboard contains text, otherwise returns <c>false</c>.</returns>
	/// <remarks>Unlike <see cref="Clipboard.ContainsText()"/>, this method does not open the clipboard.</remarks>
	internal static bool _ContainsText()
	{
		return _IsClipboardFormatAvailable(_CF_UNICODETEXT);
	}


	/// <summary>Gets the text of the clipboard.</summary>
	/// <param name="text">Receives the text, or an empty string if the clipboard is unavailable.</param>
	/// <returns>Returns <c>true</c> if the clipboard could be read, otherwise returns <c>false</c>.</returns>
	internal static bool _TryGetText(out string text)
	{
		try
		{
			text = Clipboard.GetText();
			return true;
		}
		catch(ExternalException)
		{
			text = string.Empty;
			return false;
		}
	}


	/// <summary>Puts a text on the clipboard.</summary>
	/// <param name="text">Text.</param>
	/// <returns>Returns <c>true</c> if the text has been put on the clipboard, otherwise returns <c>false</c>.</returns>
	/// <remarks>Empty texts are not put on the clipboard (see <see cref="Clipboard.SetText(string)"/>) and the clipboard is left
	///          unchanged.</remarks>
	internal static bool _TrySetText(string text)
	{
		if(string.IsNullOrEmpty(text)) { return false; }

		try
		{
			Clipboard.SetText(text);
			return true;
		}
		catch(ExternalException) { return false; }
	}


	/// <summary>Puts data on the clipboard.</summary>
	/// <param name="data">Data.</param>
	/// <param name="copy">Determines if the data remains on the clipboard after the application exits.</param>
	/// <returns>Returns <c>true</c> if the data has been put on the clipboard, otherwise returns <c>false</c>.</returns>
	internal static bool _TrySetDataObject(object data, bool copy)
	{
		try
		{
			Clipboard.SetDataObject(data, copy);
			return true;
		}
		catch(ExternalException) { return false; }
	}
}
