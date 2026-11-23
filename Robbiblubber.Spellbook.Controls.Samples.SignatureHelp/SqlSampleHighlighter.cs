namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;



/// <summary>This class provides a very simple SQL keyword highlighter for a <see cref="RichTextBox"/>.</summary>
/// <remarks>The highlighter colors keywords by selecting them and setting the selection color, like many simple highlighters do. It is
///          used to test <see cref="SignatureHelpProvider.SuspendTracking"/> together with <see cref="AutoCompletionProvider.SuspendTracking"/>.
///          Selection and scroll position are restored, redrawing is suspended while highlighting.</remarks>
[SupportedOSPlatform("windows")]
public static class SqlSampleHighlighter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Message: set redraw.</summary>
	private const int _WM_SETREDRAW = 0x000B;

	/// <summary>Message: get scroll position (rich edit).</summary>
	private const int _EM_GETSCROLLPOS = 0x04DD;

	/// <summary>Message: set scroll position (rich edit).</summary>
	private const int _EM_SETSCROLLPOS = 0x04DE;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Keywords.</summary>
	private static readonly string[] _Keywords = new string[]
	{
		"SELECT", "FROM", "WHERE", "JOIN", "INNER", "LEFT", "RIGHT", "FULL", "OUTER", "CROSS", "ON", "AND", "OR", "NOT", "IN", "EXISTS",
		"BETWEEN", "LIKE", "IS", "NULL", "AS", "DISTINCT", "TOP", "ORDER", "GROUP", "BY", "HAVING", "ASC", "DESC", "INSERT", "INTO",
		"VALUES", "UPDATE", "SET", "DELETE", "CASE", "WHEN", "THEN", "ELSE", "END", "UNION", "ALL", "WITH"
	};

	/// <summary>Keyword pattern.</summary>
	private static readonly Regex _Keyword = new(@"\b(" + string.Join("|", _Keywords) + @")\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sends a message.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First parameter.</param>
	/// <param name="lParam">Second parameter.</param>
	/// <returns>Returns the message result.</returns>
	[DllImport("user32.dll", EntryPoint = "SendMessageW")]
	private static extern IntPtr _SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


	/// <summary>Sends a message with a point parameter.</summary>
	/// <param name="hWnd">Window handle.</param>
	/// <param name="msg">Message.</param>
	/// <param name="wParam">First parameter.</param>
	/// <param name="lParam">Point.</param>
	/// <returns>Returns the message result.</returns>
	[DllImport("user32.dll", EntryPoint = "SendMessageW")]
	private static extern IntPtr _SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref Point lParam);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Highlights the keywords of a rich text box.</summary>
	/// <param name="box">Rich text box.</param>
	/// <param name="keywordColor">Keyword color.</param>
	public static void Highlight(RichTextBox box, Color keywordColor)
	{
		int start = box.SelectionStart, length = box.SelectionLength;
		Point scroll = Point.Empty;

		if(!box.IsHandleCreated) { return; }

		_SendMessage(box.Handle, _EM_GETSCROLLPOS, IntPtr.Zero, ref scroll);
		_SendMessage(box.Handle, _WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

		try
		{
			box.SelectAll();
			box.SelectionColor = box.ForeColor;

			foreach(Match i in _Keyword.Matches(box.Text))
			{
				box.Select(i.Index, i.Length);
				box.SelectionColor = keywordColor;
			}

			box.Select(start, length);
			box.SelectionColor = box.ForeColor;
		}
		finally
		{
			_SendMessage(box.Handle, _EM_SETSCROLLPOS, IntPtr.Zero, ref scroll);
			_SendMessage(box.Handle, _WM_SETREDRAW, (IntPtr) 1, IntPtr.Zero);
			box.Invalidate();
		}
	}
}
