namespace Robbiblubber.Spellbook.UnitTests.Controls.SignatureHelp;

using System.Runtime.Versioning;
using System.Windows.Forms;

using NUnit.Framework;

using Robbiblubber.Spellbook.Controls;



/// <summary>This class tests the model classes of the signature help: <see cref="SignatureInfo"/>, <see cref="SignatureParameter"/>,
///          <see cref="SignatureCall"/>, <see cref="SignatureHelpResult"/>, <see cref="SignatureHelpContext"/> and the event arguments.</summary>
[TestFixture][Apartment(ApartmentState.STA)][SupportedOSPlatform("windows")]
public class SignatureModelTests
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a signature.</summary>
	/// <param name="variadic">Determines if the last parameter is variadic.</param>
	/// <param name="labels">Parameter labels.</param>
	/// <returns>Returns the signature.</returns>
	private static SignatureInfo _Create(bool variadic, params string[] labels)
	{
		return new SignatureInfo("fn", labels.Select((m, n) => new SignatureParameter(m) { IsVariadic = (variadic && (n == (labels.Length - 1))) }));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// test methods                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>The displayed signature is composed of name, parameters, separators and suffix.</summary>
	[Test]
	public void SignatureIsComposed()
	{
		SignatureInfo signature = new("SUBSTRING", new SignatureParameter("expression"), new SignatureParameter("start"), new SignatureParameter("length")) { Suffix = "→ varchar" };

		Assert.That(signature.ToString(), Is.EqualTo("SUBSTRING(expression, start, length) → varchar"));
		Assert.That(new SignatureInfo("Now").ToString(), Is.EqualTo("Now()"));
		Assert.That(new SignatureInfo("Item", new SignatureParameter("int index")) { OpenText = "[", CloseText = "]", SeparatorText = "; " }.ToString(), Is.EqualTo("Item[int index]"));
	}


	/// <summary>Arguments map to parameters; arguments beyond the last parameter map to it only if it is variadic.</summary>
	[Test]
	public void ArgumentsMapToParameters()
	{
		SignatureInfo fixedSignature = _Create(false, "a", "b");
		SignatureInfo variadicSignature = _Create(true, "format", "args");

		Assert.That(fixedSignature.GetParameterIndex(0), Is.EqualTo(0));
		Assert.That(fixedSignature.GetParameterIndex(1), Is.EqualTo(1));
		Assert.That(fixedSignature.GetParameterIndex(2), Is.EqualTo(-1));
		Assert.That(fixedSignature.GetParameterIndex(-1), Is.EqualTo(-1));
		Assert.That(variadicSignature.GetParameterIndex(5), Is.EqualTo(1));
		Assert.That(new SignatureInfo("Now").GetParameterIndex(0), Is.EqualTo(-1));
	}


	/// <summary>A signature accepts an argument if it has a parameter for it; the first argument is always accepted.</summary>
	[Test]
	public void SignatureAcceptsArguments()
	{
		Assert.That(_Create(false, "a", "b").Accepts(1), Is.True);
		Assert.That(_Create(false, "a", "b").Accepts(2), Is.False);
		Assert.That(_Create(true, "a", "b").Accepts(9), Is.True);
		Assert.That(new SignatureInfo("Now").Accepts(0), Is.True);
		Assert.That(new SignatureInfo("Now").Accepts(1), Is.False);
	}


	/// <summary>NULL parameters are removed; NULL names and labels are rejected.</summary>
	[Test]
	public void SignatureValidatesArguments()
	{
		Assert.That(new SignatureInfo("fn", new SignatureParameter("a"), null!).Parameters, Has.Count.EqualTo(1));
		Assert.That(new SignatureInfo("fn", (SignatureParameter[]) null!).Parameters, Is.Empty);
		Assert.That(() => new SignatureInfo(null!), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new SignatureParameter(null!), Throws.TypeOf<ArgumentNullException>());
		Assert.That(new SignatureParameter("start", "Start position.").Description, Is.EqualTo("Start position."));
		Assert.That(new SignatureParameter("start").ToString(), Is.EqualTo("start"));
	}


	/// <summary>Calls are the same call if position, name and qualifier match; the argument does not matter.</summary>
	[Test]
	public void CallsAreCompared()
	{
		SignatureCall call = new("fn", "dbo", 0, 6, 0, 7);

		Assert.That(call.IsSameCall(new SignatureCall("fn", "dbo", 0, 6, 2, 12)), Is.True);
		Assert.That(call.IsSameCall(new SignatureCall("fn", "dbo", 1, 7, 0, 8)), Is.False);
		Assert.That(call.IsSameCall(new SignatureCall("FN", "dbo", 0, 6, 0, 7)), Is.False);
		Assert.That(call.IsSameCall(new SignatureCall("fn", null, 4, 6, 0, 7)), Is.False);
		Assert.That(call.IsSameCall(null), Is.False);
		Assert.That(call.ToString(), Is.EqualTo("dbo.fn(#0)"));
	}


	/// <summary>Invalid call positions are rejected.</summary>
	[Test]
	public void CallValidatesPositions()
	{
		Assert.That(() => new SignatureCall(null!, null, 0, 2, 0, 3), Throws.TypeOf<ArgumentNullException>());
		Assert.That(() => new SignatureCall("fn", null, -1, 2, 0, 3), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => new SignatureCall("fn", null, 3, 2, 0, 3), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => new SignatureCall("fn", null, 0, 2, -1, 3), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(() => new SignatureCall("fn", null, 0, 2, 0, 2), Throws.TypeOf<ArgumentOutOfRangeException>());
		Assert.That(new SignatureCall("fn", null, 0, 2, 0, 3).Qualifier, Is.Empty);
	}


	/// <summary>A result takes a snapshot of the signatures without NULL entries.</summary>
	[Test]
	public void ResultCopiesSignatures()
	{
		List<SignatureInfo> signatures = new() { new SignatureInfo("a"), null! };
		SignatureHelpResult result = new(signatures);

		signatures.Add(new SignatureInfo("b"));

		Assert.That(result.Signatures, Has.Count.EqualTo(1));
		Assert.That(result.ActiveSignature, Is.Null);
		Assert.That(() => new SignatureHelpResult(null!), Throws.TypeOf<ArgumentNullException>());
	}


	/// <summary>The context exposes the call and validates its arguments.</summary>
	[Test]
	public void ContextExposesCall()
	{
		using(TextBox text = new())
		{
			SignatureCall call = new("fn", "dbo", 0, 6, 1, 9);
			SignatureHelpContext context = new(text, "dbo.fn(a, ", 10, call, SignatureHelpTrigger.TRIGGER_CHARACTER, ',');

			Assert.That(context.Name, Is.EqualTo("fn"));
			Assert.That(context.Qualifier, Is.EqualTo("dbo"));
			Assert.That(context.ArgumentIndex, Is.EqualTo(1));
			Assert.That(context.Call, Is.SameAs(call));
			Assert.That(context.TriggerCharacter, Is.EqualTo(','));

			Assert.That(() => new SignatureHelpContext(null!, "x", 0, call, SignatureHelpTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => new SignatureHelpContext(text, null!, 0, call, SignatureHelpTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => new SignatureHelpContext(text, "x", 0, null!, SignatureHelpTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => new SignatureHelpContext(text, "x", 2, call, SignatureHelpTrigger.MANUAL, '\0'), Throws.TypeOf<ArgumentOutOfRangeException>());
		}
	}


	/// <summary>The populating event arguments copy the result; the list can be changed without changing the result.</summary>
	[Test]
	public void PopulatingArgumentsCopyResult()
	{
		using(TextBox text = new())
		{
			SignatureHelpContext context = new(text, "fn(", 3, new SignatureCall("fn", null, 0, 2, 0, 3), SignatureHelpTrigger.MANUAL, '\0');
			SignatureHelpResult result = new(new[] { new SignatureInfo("fn") }) { ActiveSignature = 0 };
			SignatureHelpPopulatingEventArgs e = new(context, result);
			SignatureHelpPopulatingEventArgs empty = new(context, null);

			e.Signatures.Add(new SignatureInfo("fn"));

			Assert.That(e.Signatures, Has.Count.EqualTo(2));
			Assert.That(result.Signatures, Has.Count.EqualTo(1));
			Assert.That(e.ActiveSignature, Is.EqualTo(0));
			Assert.That(empty.Signatures, Is.Empty);
			Assert.That(empty.ActiveSignature, Is.Null);
		}
	}
}
