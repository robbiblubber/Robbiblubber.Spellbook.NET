namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.Versioning;



/// <summary>This class implements the basics scenario.</summary>
/// <remarks>Demonstrates the signature help on its own, with a C-like parser (strings with escapes, line and block comments), overloads,
///          a variadic parameter, nested calls and wrapping signatures. The property grid shows the provider properties.</remarks>
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

		_SignatureHelp.Source = BasicsSignatures.CreateSource();
		_SignatureHelp.Parser = new SignatureCallParser { StringDelimiters = "\"'", EscapeCharacter = '\\', LineComment = "//", BlockCommentStart = "/*", BlockCommentEnd = "*/" };
		_GridProvider.SelectedObject = _SignatureHelp;

		_TextCode.Text = string.Join(Environment.NewLine, new string[]
		{
			"// Type a function call, e.g. Max( or Round(Log(x), 2) or Format(\"{0}\", a, b)",
			"// Functions: Max, Clamp, Substring, Round, Log, Format, Now, CreateRectangle",
			string.Empty,
			"double r = Round(Clamp(value, 0, 1), 2);",
			"string s = Format(\"(a, b) = {0}, {1}\", a, b);",
			string.Empty
		});

		SampleLog.Attach(_SignatureHelp, "Basics");
	}
}
