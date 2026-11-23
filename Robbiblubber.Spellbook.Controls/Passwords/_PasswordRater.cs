namespace Robbiblubber.Spellbook.Controls;

using System.Runtime.Versioning;
using System.Text.RegularExpressions;



/// <summary>This class implements the built-in password strength heuristic.</summary>
/// <remarks><para>The heuristic estimates the entropy of a password: every character contributes the bits of the character pool
///          (lower case 26, upper case 26, digits 10, symbols 33, other characters 100). Predictable characters count a quarter:
///          repetitions (<c>aaaa</c>), sequences (<c>abcd</c>, <c>4321</c>), keyboard patterns (<c>qwertz</c>, <c>asdf</c>), repeated
///          blocks (<c>Aa1!Aa1!</c>) and years (<c>1987</c>, <c>2024</c>).</para>
///          <para>Common passwords are very weak, also with substitutions (<c>P@ssw0rd</c>) and with digits or symbols appended
///          (<c>Sommer2024!</c>).</para>
///          <para>Thresholds in bits: very weak below 28, weak below 40, fair below 55, good below 70, strong below 90.</para></remarks>
[SupportedOSPlatform("windows")]
internal static class _PasswordRater
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Weight of predictable characters.</summary>
	private const double _WEAK_WEIGHT = 0.25;

	/// <summary>Minimum length of a keyboard pattern.</summary>
	private const int _MIN_KEYBOARD_RUN = 4;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Upper limits of the strengths in bits.</summary>
	private static readonly double[] _Thresholds = [28, 40, 55, 70, 90];

	/// <summary>Keyboard rows.</summary>
	private static readonly string[] _KeyboardRows = ["qwertyuiop", "qwertzuiop", "asdfghjkl", "zxcvbnm", "yxcvbnm", "azertyuiop", "1234567890"];

	/// <summary>Common passwords and words, lower case.</summary>
	private static readonly HashSet<string> _Common = new(StringComparer.Ordinal)
	{
		"password", "passwort", "passwd", "pass", "kennwort", "geheim", "secret", "admin", "administrator", "root", "login", "welcome",
		"willkommen", "letmein", "master", "access", "qwerty", "qwertz", "asdf", "asdfgh", "azerty", "abc", "abcd", "abcdef", "abc123",
		"iloveyou", "ichliebedich", "love", "liebe", "schatz", "hallo", "hello", "test", "testing", "default", "changeme", "monkey",
		"dragon", "football", "fussball", "baseball", "soccer", "superman", "batman", "pokemon", "starwars", "shadow", "sunshine",
		"princess", "prinzessin", "trustno1", "freedom", "whatever", "computer", "internet", "michael", "jennifer", "thomas", "daniel",
		"andreas", "stefan", "alexander", "sabine", "martin", "hunter", "killer", "charlie", "jordan", "harley", "ranger", "buster",
		"tigger", "summer", "sommer", "winter", "spring", "fruehling", "autumn", "herbst", "january", "januar", "december", "dezember",
		"monday", "montag", "friday", "freitag", "berlin", "wien", "vienna", "hamburg", "muenchen", "bayern", "austria", "oesterreich",
		"germany", "deutschland", "schalke", "dortmund", "rapid", "mustang", "ferrari", "porsche", "matrix", "ninja", "cookie",
		"cheese", "banana", "apple", "orange", "flower", "blume", "sonne", "mond", "stern", "engel", "angel", "family", "familie"
	};

	/// <summary>Expression that finds years.</summary>
	private static readonly Regex _Year = new("(19|20)[0-9]{2}", RegexOptions.Compiled);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Replaces common character substitutions.</summary>
	/// <param name="text">Lower case text.</param>
	/// <returns>Returns the text with letters instead of substitutes.</returns>
	private static string _Unleet(string text)
	{
		char[] rval = text.ToCharArray();

		for(int i = 0; i < rval.Length; i++)
		{
			rval[i] = rval[i] switch
			{
				'0' => 'o',
				'1' => 'i',
				'3' => 'e',
				'4' => 'a',
				'5' => 's',
				'7' => 't',
				'@' => 'a',
				'$' => 's',
				'!' => 'i',
				_ => rval[i]
			};
		}

		return new string(rval);
	}


	/// <summary>Determines if a password is a common password, possibly with substitutions and appended digits or symbols.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns <c>true</c> if the password is common, otherwise returns <c>false</c>.</returns>
	internal static bool _IsCommon(string password)
	{
		string lower = password.ToLowerInvariant();
		string trimmed = lower.TrimEnd("0123456789!?.,;:-_+*#$%&/()=@ ".ToCharArray());

		if(_Common.Contains(lower) || _Common.Contains(_Unleet(lower))) { return true; }
		if(trimmed.Length == 0) { return false; }

		return (_Common.Contains(trimmed) || _Common.Contains(_Unleet(trimmed)));
	}


	/// <summary>Marks predictable characters.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns an array that is <c>true</c> for every predictable character.</returns>
	internal static bool[] _MarkPredictable(string password)
	{
		string lower = password.ToLowerInvariant();
		bool[] rval = new bool[password.Length];

		for(int i = 1; i < lower.Length; i++)
		{
			int d = lower[i] - lower[i - 1];

			if(d == 0) { rval[i] = true; }
			if((i >= 2) && (Math.Abs(d) == 1) && (d == (lower[i - 1] - lower[i - 2])) && char.IsLetterOrDigit(lower[i]) && char.IsLetterOrDigit(lower[i - 1]) && char.IsLetterOrDigit(lower[i - 2]))
			{
				rval[i] = rval[i - 1] = true;
			}
		}

		foreach(string row in _KeyboardRows)
		{
			foreach(string i in new string[] { row, new string(row.Reverse().ToArray()) })
			{
				for(int start = 0; start <= (i.Length - _MIN_KEYBOARD_RUN); start++)
				{
					string run = i.Substring(start, _MIN_KEYBOARD_RUN);
					int index = lower.IndexOf(run, StringComparison.Ordinal);

					while(index >= 0)
					{
						for(int k = index + 1; k < (index + run.Length); k++) { rval[k] = true; }
						index = lower.IndexOf(run, index + 1, StringComparison.Ordinal);
					}
				}
			}
		}

		for(int unit = 1; unit <= (password.Length / 2); unit++)
		{
			if((password.Length % unit) != 0) { continue; }

			bool repeated = true;
			for(int i = unit; (i < password.Length) && repeated; i++) { repeated = (password[i] == password[i - unit]); }

			if(repeated)
			{
				for(int i = unit; i < password.Length; i++) { rval[i] = true; }
				break;
			}
		}

		foreach(Match i in _Year.Matches(password))
		{
			for(int k = i.Index + 1; k < (i.Index + i.Length); k++) { rval[k] = true; }
		}

		return rval;
	}


	/// <summary>Gets the size of the character pool of a password.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns the number of possible characters.</returns>
	internal static int _GetPoolSize(string password)
	{
		int rval = 0;

		if(password.Any(m => (m >= 'a') && (m <= 'z'))) { rval += 26; }
		if(password.Any(m => (m >= 'A') && (m <= 'Z'))) { rval += 26; }
		if(password.Any(m => (m >= '0') && (m <= '9'))) { rval += 10; }
		if(password.Any(m => (m < 128) && (!char.IsLetterOrDigit(m)))) { rval += 33; }
		if(password.Any(m => m >= 128)) { rval += 100; }

		return Math.Max(rval, 1);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Estimates the entropy of a password.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns the estimated entropy in bits.</returns>
	internal static double _GetBits(string password)
	{
		if(password.Length == 0) { return 0; }

		double length = _MarkPredictable(password).Sum(m => (m ? _WEAK_WEIGHT : 1d));
		return length * Math.Log2(_GetPoolSize(password));
	}


	/// <summary>Rates a password.</summary>
	/// <param name="password">Password.</param>
	/// <returns>Returns the strength; <see cref="PasswordStrength.NONE"/> for an empty password.</returns>
	internal static PasswordStrength _Rate(string? password)
	{
		if(string.IsNullOrEmpty(password)) { return PasswordStrength.NONE; }
		if(_IsCommon(password)) { return PasswordStrength.VERY_WEAK; }

		double bits = _GetBits(password);

		for(int i = 0; i < _Thresholds.Length; i++)
		{
			if(bits < _Thresholds[i]) { return (PasswordStrength) i; }
		}

		return PasswordStrength.VERY_STRONG;
	}
}
