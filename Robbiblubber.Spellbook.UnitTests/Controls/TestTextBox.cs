namespace Robbiblubber.Spellbook.UnitTests.Controls;

using System.Runtime.Versioning;
using System.Windows.Forms;



/// <summary>This class implements a text box that simulates keyboard input and focus for auto completion and signature help tests.</summary>
/// <remarks>The text box always reports to be focused, so auto completion sessions run without a visible or active window. Keystrokes
///          are simulated by raising the key events in the order of the native control and inserting the typed character, unless an
///          event handler has suppressed it.</remarks>
[SupportedOSPlatform("windows")]
public class TestTextBox: TextBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the key code for a character.</summary>
	/// <param name="c">Character.</param>
	/// <returns>Returns the key code, <see cref="Keys.None"/> for characters without a dedicated key code.</returns>
	private static Keys _GetKeyCode(char c)
	{
		char upper = char.ToUpperInvariant(c);

		if((upper >= 'A') && (upper <= 'Z')) { return Keys.A + (upper - 'A'); }
		if((c >= '0') && (c <= '9')) { return Keys.D0 + (c - '0'); }

		return c switch
		{
			' ' => Keys.Space,
			'.' => Keys.OemPeriod,
			_ => Keys.None
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Types a text at the caret position, one character at a time.</summary>
	/// <param name="text">Text. The character <c>'\b'</c> is typed as backspace.</param>
	public void Type(string text)
	{
		foreach(char i in text)
		{
			if(i == '\b')
			{
				Backspace();
				continue;
			}

			KeyEventArgs down = new(_GetKeyCode(i));
			OnKeyDown(down);
			if(down.SuppressKeyPress) { continue; }

			KeyPressEventArgs press = new(i);
			OnKeyPress(press);
			if(press.Handled) { continue; }

			SelectedText = i.ToString();
		}
	}


	/// <summary>Types a backspace, deleting the character before the caret.</summary>
	public void Backspace()
	{
		int caret = SelectionStart;

		OnKeyDown(new KeyEventArgs(Keys.Back));
		OnKeyPress(new KeyPressEventArgs('\b'));

		if(caret == 0) { return; }

		Select(caret - 1, 1);
		SelectedText = string.Empty;
	}


	/// <summary>Presses a key without typing a character.</summary>
	/// <param name="keyData">Key code and modifiers.</param>
	/// <returns>Returns <c>true</c> if the key has been handled, otherwise returns <c>false</c>.</returns>
	/// <remarks>The key only raises the KeyDown event. Caret movements have to be applied with <see cref="TextBoxBase.Select(int, int)"/>,
	///          followed by <see cref="Application.DoEvents"/>, because the auto completion checks the caret after the key has been
	///          processed.</remarks>
	public bool Press(Keys keyData)
	{
		KeyEventArgs e = new(keyData);

		OnKeyDown(e);
		return e.Handled;
	}


	/// <summary>Sets the text and the caret position without simulating keystrokes.</summary>
	/// <param name="text">Text.</param>
	/// <param name="caret">Caret position, -1 for the end of the text.</param>
	public void SetText(string text, int caret = -1)
	{
		Text = text;
		Select(((caret < 0) ? text.Length : caret), 0);
	}


	/// <summary>Raises the TextChanged event without changing the text, like a formatting-only change.</summary>
	public void RaiseTextChanged()
	{
		OnTextChanged(EventArgs.Empty);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] Control                                                                                               //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override bool Focused
	{
		get { return true; }
	}
}
