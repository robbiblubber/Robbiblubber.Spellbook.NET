namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;

using System.Runtime.Versioning;
using System.Text.RegularExpressions;



/// <summary>This class implements a simple context-sensitive auto completion source for SQL.</summary>
/// <remarks>The source keeps separate, precomputed item lists for each purpose (keywords, functions, tables, columns per table)
///          and only selects and combines them per request. The context analysis is deliberately simple: it looks at the last
///          clause keyword and the token before the current word, resolves table aliases from FROM and JOIN clauses and detects
///          open square brackets.</remarks>
[SupportedOSPlatform("windows")]
public class SqlSampleSource: IAutoCompletionSource
{
    //#H private static members

    /// <summary>SQL keywords.</summary>
    private static readonly string[] _Keywords = new string[]
    {
        "ALL", "AND", "AS", "ASC", "BETWEEN", "BY", "CASE", "CROSS", "DELETE", "DESC", "DISTINCT", "ELSE", "END", "EXISTS", "FROM",
        "FULL", "GROUP", "HAVING", "IN", "INNER", "INSERT", "INTO", "IS", "JOIN", "LEFT", "LIKE", "NOT", "NULL", "ON", "OR",
        "ORDER", "OUTER", "RIGHT", "SELECT", "SET", "THEN", "TOP", "UNION", "UPDATE", "VALUES", "WHEN", "WHERE", "WITH"
    };

    /// <summary>SQL functions with signature and description.</summary>
    /// <remarks>A function takes arguments if its signature does not end with "()".</remarks>
    private static readonly (string Name, string Signature, string Description)[] _Functions = new (string, string, string)[]
    {
        ("ABS", "ABS(numeric_expression)", "Returns the absolute value."),
        ("AVG", "AVG([ALL | DISTINCT] expression)", "Returns the average of the values in a group."),
        ("CAST", "CAST(expression AS data_type)", "Converts an expression to a data type."),
        ("COALESCE", "COALESCE(expression, expression [, ...])", "Returns the first non-NULL expression."),
        ("CONCAT", "CONCAT(string_value1, string_value2 [, ...])", "Concatenates two or more values."),
        ("CONVERT", "CONVERT(data_type, expression [, style])", "Converts an expression to a data type, with an optional style."),
        ("COUNT", "COUNT({* | [ALL | DISTINCT] expression})", "Returns the number of items in a group."),
        ("DATEADD", "DATEADD(datepart, number, date)", "Adds a number of dateparts to a date."),
        ("DATEDIFF", "DATEDIFF(datepart, startdate, enddate)", "Returns the number of datepart boundaries between two dates."),
        ("DAY", "DAY(date)", "Returns the day of the month."),
        ("GETDATE", "GETDATE()", "Returns the current date and time."),
        ("IIF", "IIF(boolean_expression, true_value, false_value)", "Returns one of two values, depending on a condition."),
        ("ISNULL", "ISNULL(check_expression, replacement_value)", "Replaces NULL with a replacement value."),
        ("LEN", "LEN(string_expression)", "Returns the number of characters, excluding trailing spaces."),
        ("LOWER", "LOWER(character_expression)", "Converts to lower case."),
        ("MAX", "MAX(expression)", "Returns the maximum value in a group."),
        ("MIN", "MIN(expression)", "Returns the minimum value in a group."),
        ("MONTH", "MONTH(date)", "Returns the month."),
        ("NEWID", "NEWID()", "Creates a unique value of type uniqueidentifier."),
        ("REPLACE", "REPLACE(string_expression, string_pattern, string_replacement)", "Replaces all occurrences of a substring."),
        ("ROUND", "ROUND(numeric_expression, length [, function])", "Rounds to a length or precision."),
        ("SUBSTRING", "SUBSTRING(expression, start, length)", "Returns part of a string."),
        ("SUM", "SUM([ALL | DISTINCT] expression)", "Returns the sum of the values in a group."),
        ("TRIM", "TRIM([characters FROM] string)", "Removes leading and trailing spaces or characters."),
        ("UPPER", "UPPER(character_expression)", "Converts to upper case."),
        ("YEAR", "YEAR(date)", "Returns the year.")
    };

    /// <summary>Keywords that are followed by a table name.</summary>
    private static readonly HashSet<string> _TableClauses = new(StringComparer.OrdinalIgnoreCase) { "FROM", "JOIN", "UPDATE", "INTO" };

    /// <summary>Keywords that start or continue a clause containing column expressions.</summary>
    private static readonly HashSet<string> _ColumnClauses = new(StringComparer.OrdinalIgnoreCase) { "SELECT", "WHERE", "ON", "BY", "SET", "HAVING", "AND", "OR", "WHEN", "THEN", "ELSE", "CASE" };

    /// <summary>Keyword lookup.</summary>
    private static readonly HashSet<string> _KeywordSet = new(_Keywords, StringComparer.OrdinalIgnoreCase);

    /// <summary>Table reference pattern: keyword, optional schema, table name, optional alias.</summary>
    private static readonly Regex _TableReference = new(@"\b(?:FROM|JOIN|UPDATE|INTO)\s+(?:dbo\.)?(\[[^\]]+\]|\w+)(?:\s+(?:AS\s+)?(\w+))?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Token pattern.</summary>
    private static readonly Regex _Token = new(@"\[[^\]]*\]|\w+|[^\s\w]", RegexOptions.Compiled);



    //#H private members

    /// <summary>Keyword items.</summary>
    private readonly List<IAutoCompletionItem> _KeywordItems = new();

    /// <summary>Function items.</summary>
    private readonly List<IAutoCompletionItem> _FunctionItems = new();

    /// <summary>Table items.</summary>
    private readonly List<IAutoCompletionItem> _TableItems = new();

    /// <summary>Table items including the schema.</summary>
    private readonly List<IAutoCompletionItem> _TableAndSchemaItems = new();

    /// <summary>Table items for bracketed names.</summary>
    private readonly List<IAutoCompletionItem> _BracketTableItems = new();

    /// <summary>Column items by table, for qualified access.</summary>
    private readonly Dictionary<string, List<IAutoCompletionItem>> _ColumnItems = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Column items by table, with the table name as detail, for unqualified access.</summary>
    private readonly Dictionary<string, List<IAutoCompletionItem>> _UnqualifiedColumnItems = new(StringComparer.OrdinalIgnoreCase);



    //#H constructors

    /// <summary>Creates a new instance of this class.</summary>
    public SqlSampleSource()
    {
        foreach(string i in _Keywords)
        {
            _KeywordItems.Add(new AutoCompletionItem(i, "keyword") { SortText = "2" + i, ForeColor = Color.RoyalBlue, Category = "Keyword" });
        }

        foreach((string Name, string Signature, string Description) i in _Functions)
        {
            bool hasArguments = (!i.Signature.EndsWith("()"));

            _FunctionItems.Add(new AutoCompletionItem(i.Name, "function")
            {
                InsertText = i.Name + "()", CaretPosition = (hasArguments ? (i.Name.Length + 1) : null), Detail = "function", SortText = "1" + i.Name,
                Description = i.Signature + Environment.NewLine + Environment.NewLine + i.Description, Category = "Function"
            });
        }

        foreach(SqlSampleTable i in SqlSampleSchema.Tables)
        {
            bool quoted = i.Name.Contains(' ');
            List<IAutoCompletionItem> columns = new();
            List<IAutoCompletionItem> unqualified = new();

            string tableDescription = $"{SqlSampleSchema.SchemaName}.{i.Name}" + Environment.NewLine + string.Join(", ", i.Columns.Select(m => m.Name));

            _TableItems.Add(new AutoCompletionItem(i.Name, "table") { InsertText = (quoted ? $"[{i.Name}]" : null), Detail = $"{i.Columns.Count} columns", Description = tableDescription, Category = "Table" });
            _BracketTableItems.Add(new AutoCompletionItem(i.Name, "table") { InsertText = $"[{i.Name}]", FilterText = $"[{i.Name}]", Detail = $"{i.Columns.Count} columns", Description = tableDescription, Category = "Table" });

            foreach(SqlSampleColumn j in i.Columns)
            {
                FontStyle style = (j.IsKey ? FontStyle.Bold : FontStyle.Regular);
                string columnDescription = $"{i.Name}.{j.Name}" + Environment.NewLine + j.Type + (j.IsKey ? ", primary key" : string.Empty);

                columns.Add(new AutoCompletionItem(j.Name, "column") { Detail = j.Type, FontStyle = style, Description = columnDescription, Category = "Column" });
                unqualified.Add(new AutoCompletionItem(j.Name, "column") { Detail = $"{i.Name} · {j.Type}", SortText = "0" + j.Name, FontStyle = style, Description = columnDescription, Category = "Column" });
            }

            _ColumnItems.Add(i.Name, columns);
            _UnqualifiedColumnItems.Add(i.Name, unqualified);
        }

        _TableAndSchemaItems.AddRange(_TableItems);
        _TableAndSchemaItems.Add(new AutoCompletionItem(SqlSampleSchema.SchemaName, "schema") { Detail = "schema", Category = "Schema" });
    }



    //#H private methods

    /// <summary>Finds an unclosed square bracket on the current line before the caret.</summary>
    /// <param name="text">Text.</param>
    /// <param name="caret">Caret index.</param>
    /// <returns>Returns the index of the bracket, or NULL if there is no unclosed bracket.</returns>
    private int? _FindOpenBracket(string text, int caret)
    {
        for(int i = caret - 1; i >= 0; i--)
        {
            if((text[i] == ']') || (text[i] == '\n')) { return null; }
            if(text[i] == '[') { return i; }
        }

        return null;
    }


    /// <summary>Gets the tables referenced in FROM, JOIN, UPDATE and INTO clauses.</summary>
    /// <param name="text">Text.</param>
    /// <returns>Returns a list of (table, alias) tuples. The alias is NULL if the table has no alias.</returns>
    private List<(string Table, string? Alias)> _GetTableReferences(string text)
    {
        List<(string Table, string? Alias)> rval = new();

        foreach(Match i in _TableReference.Matches(text))
        {
            string table = i.Groups[1].Value.Trim('[', ']');
            string? alias = (i.Groups[2].Success ? i.Groups[2].Value : null);

            if((alias is not null) && _KeywordSet.Contains(alias)) { alias = null; }
            if(_ColumnItems.ContainsKey(table)) { rval.Add((table, alias)); }
        }

        return rval;
    }


    /// <summary>Gets the items for a qualified word.</summary>
    /// <param name="context">Auto completion context.</param>
    /// <returns>Returns the result.</returns>
    private AutoCompletionResult? _GetQualifiedItems(AutoCompletionContext context)
    {
        string name = context.Qualifier.Split('.')[^1];

        if(name.Equals(SqlSampleSchema.SchemaName, StringComparison.OrdinalIgnoreCase))
        {
            return new AutoCompletionResult(_TableItems) { MatchMode = ListMatchMode.CONTAINS };
        }

        foreach((string Table, string? Alias) i in _GetTableReferences(context.Text))
        {
            if(name.Equals(i.Alias, StringComparison.OrdinalIgnoreCase) || name.Equals(i.Table, StringComparison.OrdinalIgnoreCase))
            {
                return new AutoCompletionResult(_ColumnItems[i.Table]) { MatchMode = ListMatchMode.FUZZY };
            }
        }

        return null;
    }


    /// <summary>Gets the items for a context.</summary>
    /// <param name="context">Auto completion context.</param>
    /// <returns>Returns the result.</returns>
    private AutoCompletionResult? _GetItems(AutoCompletionContext context)
    {
        int? bracket = _FindOpenBracket(context.Text, context.CaretIndex);
        if(bracket is not null)
        {
            return new AutoCompletionResult(_BracketTableItems) { ReplaceStart = bracket.Value };
        }

        if(context.Qualifier.Length > 0) { return _GetQualifiedItems(context); }

        MatchCollection tokens = _Token.Matches(context.Text.Substring(0, context.WordStart));
        string previous = ((tokens.Count > 0) ? tokens[tokens.Count - 1].Value : string.Empty);
        string? clause = null;

        for(int i = tokens.Count - 1; i >= 0; i--)
        {
            string token = tokens[i].Value;

            if(token == ";") { break; }
            if(_TableClauses.Contains(token) || _ColumnClauses.Contains(token))
            {
                clause = token.ToUpperInvariant();
                break;
            }
        }

        if(_TableClauses.Contains(previous) || ((previous == ",") && (clause == "FROM")))
        {
            return new AutoCompletionResult(_TableAndSchemaItems) { MatchMode = ListMatchMode.CONTAINS };
        }

        if((clause is not null) && _ColumnClauses.Contains(clause))
        {
            List<(string Table, string? Alias)> tables = _GetTableReferences(context.Text);
            List<IAutoCompletionItem> items = new();

            if(tables.Count == 0)
            {
                foreach(List<IAutoCompletionItem> i in _UnqualifiedColumnItems.Values) { items.AddRange(i); }
            }
            else
            {
                foreach(string i in tables.Select(m => m.Table).Distinct(StringComparer.OrdinalIgnoreCase)) { items.AddRange(_UnqualifiedColumnItems[i]); }
            }

            items.AddRange(_FunctionItems);
            items.AddRange(_KeywordItems);

            return new AutoCompletionResult(items) { MatchMode = ListMatchMode.FUZZY };
        }

        return new AutoCompletionResult(_KeywordItems);
    }



    //#H public properties

    /// <summary>Gets or sets a simulated latency in milliseconds.</summary>
    public int Latency
    {
        get; set;
    }


    /// <summary>Gets or sets if the source throws an exception to simulate a failure.</summary>
    public bool ThrowException
    {
        get; set;
    }



    //#H [interface] IAutoCompletionSource

    /// <inheritdoc/>
    public async ValueTask<AutoCompletionResult?> GetItemsAsync(AutoCompletionContext context, CancellationToken cancellationToken)
    {
        if(Latency > 0) { await Task.Delay(Latency, cancellationToken); }
        if(ThrowException) { throw new InvalidOperationException("Simulated source failure."); }

        return _GetItems(context);
    }
}
