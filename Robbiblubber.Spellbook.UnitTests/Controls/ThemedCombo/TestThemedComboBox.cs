namespace Robbiblubber.Spellbook.UnitTests.Controls.ThemedCombo;

using System.Runtime.Versioning;
using System.Windows.Forms;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class implements a rich combo box that simulates keyboard input for tests.</summary>
[SupportedOSPlatform("windows")]
public class TestThemedComboBox: ThemedComboBox
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Simulates a key press.</summary>
	/// <param name="keyData">Key and modifiers.</param>
	/// <returns>Returns <c>true</c> if the control has handled the key, otherwise returns <c>false</c>.</returns>
	public bool PressKey(Keys keyData)
	{
		KeyEventArgs e = new(keyData);

		OnKeyDown(e);
		return e.Handled;
	}


	/// <summary>Gets if a character is an input character of the control.</summary>
	/// <param name="c">Character.</param>
	/// <returns>Returns <c>true</c> if the control processes the character, <c>false</c> if the form may process it as mnemonic.</returns>
	public bool AcceptsChar(char c)
	{
		return IsInputChar(c);
	}


	/// <summary>Simulates typing a text.</summary>
	/// <param name="text">Text.</param>
	public void Type(string text)
	{
		foreach(char i in text) { OnKeyPress(new KeyPressEventArgs(i)); }
	}
}
