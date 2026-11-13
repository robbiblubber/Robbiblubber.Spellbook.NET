namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;

using System.Runtime.Versioning;



/// <summary>This class provides sample data for the sample scenarios.</summary>
[SupportedOSPlatform("windows")]
public static class SampleData
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Word parts for generated identifiers.</summary>
	private static readonly string[] _IdentifierParts = new string[]
	{
		"Customer", "Order", "Invoice", "Product", "Category", "Supplier", "Employee", "Region", "Territory", "Shipper",
		"Payment", "Account", "Balance", "Ledger", "Journal", "Entry", "Item", "Line", "Detail", "Header",
		"History", "Archive", "Log", "Audit", "Event", "Message", "Queue", "Batch", "Job", "Task",
		"Price", "Discount", "Tax", "Currency", "Rate", "Amount", "Total", "Count", "Status", "Type"
	};



	/// <summary>Identifiers that are always part of the generated identifiers.</summary>
	private static readonly string[] _FixedIdentifiers = new string[] { "CustomerOrderHistory", "CurrencyEntryOrderHistory", "InvoiceTaxRate", "InvoiceTotalTax", "PaymentAccountBalance" };



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets a list of country names.</summary>
	public static IReadOnlyList<string> Countries
	{
		get;
	} = new string[]
	{
		"Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan",
		"Bahamas", "Bangladesh", "Belarus", "Belgium", "Bolivia", "Bosnia and Herzegovina", "Brazil", "Bulgaria", "Cambodia", "Cameroon",
		"Canada", "Chile", "China", "Colombia", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czechia", "Denmark",
		"Ecuador", "Egypt", "Estonia", "Ethiopia", "Finland", "France", "Georgia", "Germany", "Ghana", "Greece",
		"Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica",
		"Japan", "Jordan", "Kazakhstan", "Kenya", "Latvia", "Lebanon", "Liechtenstein", "Lithuania", "Luxembourg", "Malaysia",
		"Malta", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Nepal", "Netherlands", "New Zealand",
		"Nigeria", "North Macedonia", "Norway", "Pakistan", "Panama", "Peru", "Philippines", "Poland", "Portugal", "Qatar",
		"Romania", "Russia", "San Marino", "Saudi Arabia", "Serbia", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea",
		"Spain", "Sri Lanka", "Sweden", "Switzerland", "Thailand", "Tunisia", "Turkey", "Ukraine", "United Kingdom", "United States",
		"Uruguay", "Vatican City", "Venezuela", "Vietnam", "Zambia", "Zimbabwe"
	};


	/// <summary>Gets a list of cities.</summary>
	public static IReadOnlyList<string> Cities
	{
		get;
	} = new string[]
	{
		"Amsterdam", "Athens", "Barcelona", "Belgrade", "Berlin", "Bern", "Bratislava", "Brussels", "Bucharest", "Budapest",
		"Copenhagen", "Dublin", "Edinburgh", "Florence", "Frankfurt", "Geneva", "Graz", "Hamburg", "Helsinki", "Innsbruck",
		"Istanbul", "Kraków", "Linz", "Lisbon", "Ljubljana", "London", "Luxembourg", "Lyon", "Madrid", "Milan",
		"Munich", "Naples", "Oslo", "Paris", "Prague", "Reykjavík", "Riga", "Rome", "Salzburg", "Sarajevo",
		"Sofia", "Stockholm", "Tallinn", "Valletta", "Venice", "Vienna", "Vilnius", "Warsaw", "Zagreb", "Zurich"
	};


	/// <summary>Gets a list of fruits.</summary>
	public static IReadOnlyList<string> Fruits
	{
		get;
	} = new string[]
	{
		"Apple", "Apricot", "Avocado", "Banana", "Blackberry", "Blueberry", "Cherry", "Coconut", "Cranberry", "Date",
		"Fig", "Gooseberry", "Grape", "Grapefruit", "Guava", "Kiwi", "Lemon", "Lime", "Lychee", "Mango",
		"Melon", "Nectarine", "Orange", "Papaya", "Passion Fruit", "Peach", "Pear", "Pineapple", "Plum", "Pomegranate",
		"Quince", "Raspberry", "Redcurrant", "Strawberry", "Tangerine", "Watermelon"
	};



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates auto completion items for programming languages.</summary>
	/// <returns>Returns the items.</returns>
	/// <remarks>Demonstrates <see cref="ListItem.Detail"/> and <see cref="ListItem.ImageKey"/>.</remarks>
	public static List<IAutoCompletionItem> CreateLanguageItems()
	{
		(string Name, int Year)[] languages = new (string, int)[]
		{
			("Ada", 1980), ("ALGOL", 1958), ("BASIC", 1964), ("C", 1972), ("C#", 2000), ("C++", 1985), ("COBOL", 1959), ("Delphi", 1995),
			("Eiffel", 1986), ("Elixir", 2012), ("Erlang", 1986), ("F#", 2005), ("Fortran", 1957), ("Go", 2009), ("Haskell", 1990),
			("Java", 1995), ("JavaScript", 1995), ("Kotlin", 2011), ("Lisp", 1958), ("Lua", 1993), ("Modula-2", 1978), ("Oberon", 1987),
			("Pascal", 1970), ("Perl", 1987), ("PHP", 1995), ("Prolog", 1972), ("Python", 1991), ("Ruby", 1995), ("Rust", 2015),
			("Scala", 2004), ("Simula", 1967), ("Smalltalk", 1972), ("SQL", 1974), ("Swift", 2014), ("TypeScript", 2012), ("Visual Basic", 1991)
		};

		return languages.Select(m => (IAutoCompletionItem) new AutoCompletionItem(m.Name, "word") { Detail = m.Year.ToString() }).ToList();
	}


	/// <summary>Creates auto completion items for the public types of the core library.</summary>
	/// <returns>Returns the items.</returns>
	/// <remarks>Type names are CamelCase and therefore well suited to test fuzzy matching.</remarks>
	public static List<IAutoCompletionItem> CreateTypeItems()
	{
		List<IAutoCompletionItem> rval = new();
		HashSet<string> names = new();

		foreach(Type i in typeof(object).Assembly.GetExportedTypes())
		{
			string name = i.Name;
			int tick = name.IndexOf('`');

			if(tick > 0) { name = name.Substring(0, tick); }
			if(i.IsNested || (!names.Add(name))) { continue; }

			string imageKey = (i.IsInterface ? "interface" : (i.IsEnum ? "enum" : (i.IsValueType ? "struct" : "class")));
			string description = $"{imageKey} {i.FullName}" + (((i.BaseType is null) || (i.BaseType == typeof(object)) || i.IsValueType || i.IsEnum) ? string.Empty : (Environment.NewLine + "Base type: " + i.BaseType.FullName));

			rval.Add(new AutoCompletionItem(name, imageKey) { Detail = i.Namespace, Description = description, Category = imageKey, FontStyle = (i.IsAbstract && (!i.IsInterface) ? FontStyle.Italic : FontStyle.Regular) });
		}

		return rval;
	}


	/// <summary>Generates a large number of unique identifiers.</summary>
	/// <remarks>The identifiers used in the test checklist (<see cref="_FixedIdentifiers"/>) are always included.</remarks>
	/// <param name="count">Number of identifiers.</param>
	/// <param name="seed">Random seed.</param>
	/// <returns>Returns the items.</returns>
	public static List<IAutoCompletionItem> GenerateIdentifiers(int count, int seed = 42)
	{
		string[] imageKeys = new string[] { "class", "interface", "struct", "enum", "column", "table" };
		List<IAutoCompletionItem> rval = new(count);
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
		Random random = new(seed);

		foreach(string i in _FixedIdentifiers)
		{
			if((rval.Count < count) && names.Add(i)) { rval.Add(new AutoCompletionItem(i, "class") { Detail = "fixed" }); }
		}

		while(rval.Count < count)
		{
			int parts = random.Next(2, 5);
			string name = string.Empty;

			for(int i = 0; i < parts; i++) { name += _IdentifierParts[random.Next(_IdentifierParts.Length)]; }
			if(random.Next(3) == 0) { name += random.Next(1000).ToString(); }

			if(names.Add(name)) { rval.Add(new AutoCompletionItem(name, imageKeys[rval.Count % imageKeys.Length]) { Detail = "#" + rval.Count.ToString() }); }
		}

		return rval;
	}
}
