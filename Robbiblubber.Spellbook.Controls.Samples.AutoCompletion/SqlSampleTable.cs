namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;



/// <summary>This class represents a table of the sample database schema.</summary>
/// <param name="Name">Table name.</param>
/// <param name="Columns">Columns.</param>
public record class SqlSampleTable(string Name, IReadOnlyList<SqlSampleColumn> Columns);
