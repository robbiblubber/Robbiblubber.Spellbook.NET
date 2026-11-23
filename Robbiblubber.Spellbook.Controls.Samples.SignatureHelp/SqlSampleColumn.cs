namespace Robbiblubber.Spellbook.Controls.Samples.SignatureHelp;



/// <summary>This class represents a column of the sample database schema.</summary>
/// <param name="Name">Column name.</param>
/// <param name="Type">Column data type.</param>
/// <param name="IsKey">Determines if the column is a primary key column.</param>
public record class SqlSampleColumn(string Name, string Type, bool IsKey = false);
