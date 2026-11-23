namespace Robbiblubber.Spellbook.UnitTests.Controls.AutoCompletion;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the <see cref="AutoCompletionContext"/> class.</summary>
[TestFixture][Apartment(ApartmentState.STA)]
[SupportedOSPlatform("windows")]
public class AutoCompletionContextTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Text box.</summary>
	private TextBox _Text = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a context.</summary>
	/// <param name="text">Text.</param>
	/// <param name="caret">Caret index.</param>
	/// <param name="wordStart">Word start index.</param>
	/// <param name="qualifier">Qualifier.</param>
	/// <returns>Returns the context.</returns>
	private AutoCompletionContext _Create(string text, int caret, int wordStart, string qualifier = "")
	{
		return new AutoCompletionContext(_Text, text, caret, wordStart, qualifier, AutoCompletionTrigger.TYPING, 'x');
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Prepares a test.</summary>
	[SetUp]
	public void SetUp()
	{
		_Text = new TextBox();
	}


	/// <summary>Cleans up after a test.</summary>
	[TearDown]
	public void TearDown()
	{
		_Text.Dispose();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The word is the text between word start and caret.</summary>
	[Test]
	public void WordIsTextBeforeCaret()
	{
		AutoCompletionContext context = _Create("SELECT Customer", 11, 7);

		Assert.That(context.Word, Is.EqualTo("Cust"));
		Assert.That(context.CaretIndex, Is.EqualTo(11));
		Assert.That(context.WordStart, Is.EqualTo(7));
		Assert.That(context.Text, Is.EqualTo("SELECT Customer"));
		Assert.That(context.Control, Is.SameAs(_Text));
		Assert.That(context.Trigger, Is.EqualTo(AutoCompletionTrigger.TYPING));
		Assert.That(context.TriggerCharacter, Is.EqualTo('x'));
	}


	/// <summary>The line is the text of the current line before the caret, without line break characters.</summary>
	[Test]
	public void LineIsCurrentLineBeforeCaret()
	{
		Assert.That(_Create("SELECT *\r\nFROM Cu", 17, 15).Line, Is.EqualTo("FROM Cu"));
		Assert.That(_Create("SELECT *\r\nFROM Cu", 4, 0).Line, Is.EqualTo("SELE"));
		Assert.That(_Create("ab\r\n", 4, 4).Line, Is.Empty);
		Assert.That(_Create("ab\r\nc", 3, 3).Line, Is.EqualTo("ab"));
		Assert.That(_Create("one\ntwo", 7, 4).Line, Is.EqualTo("two"));
	}


	/// <summary>A caret at the start of the text yields an empty word and line.</summary>
	[Test]
	public void CaretAtStartYieldsEmptyWord()
	{
		AutoCompletionContext context = _Create("abc", 0, 0);

		Assert.That(context.Word, Is.Empty);
		Assert.That(context.Line, Is.Empty);
	}


	/// <summary>A NULL qualifier is replaced by an empty string.</summary>
	[Test]
	public void QualifierIsNeverNull()
	{
		Assert.That(_Create("x", 1, 0, null!).Qualifier, Is.Empty);
		Assert.That(_Create("dbo.Cu", 6, 4, "dbo").Qualifier, Is.EqualTo("dbo"));
	}


	/// <summary>Invalid arguments are rejected.</summary>
	[Test]
	public void InvalidArgumentsAreRejected()
	{
		Assert.That(() => new AutoCompletionContext(null!, "abc", 0, 0, string.Empty, AutoCompletionTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new AutoCompletionContext(_Text, null!, 0, 0, string.Empty, AutoCompletionTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => _Create("abc", 4, 0), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => _Create("abc", -1, 0), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => _Create("abc", 2, 3), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => _Create("abc", 2, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
	}
}
