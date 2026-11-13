namespace Robbiblubber.Spellbook.Controls.Samples.AutoCompletion;



/// <summary>This class provides a small in-memory database schema for the SQL sample.</summary>
public static class SqlSampleSchema
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the schema name.</summary>
	public static string SchemaName
	{
		get;
	} = "dbo";


	/// <summary>Gets the tables.</summary>
	public static IReadOnlyList<SqlSampleTable> Tables
	{
		get;
	} = new SqlSampleTable[]
	{
		new("Customers", new SqlSampleColumn[]
		{
			new("CustomerId", "int", true), new("Name", "nvarchar(100)"), new("City", "nvarchar(50)"), new("Country", "nvarchar(50)"), new("CreatedAt", "datetime2")
		}),
		new("Orders", new SqlSampleColumn[]
		{
			new("OrderId", "int", true), new("CustomerId", "int"), new("EmployeeId", "int"), new("OrderDate", "datetime2"), new("Total", "decimal(18,2)"), new("Status", "tinyint")
		}),
		new("Order Details", new SqlSampleColumn[]
		{
			new("OrderId", "int", true), new("ProductId", "int", true), new("Quantity", "int"), new("UnitPrice", "decimal(18,2)"), new("Discount", "real")
		}),
		new("Products", new SqlSampleColumn[]
		{
			new("ProductId", "int", true), new("Name", "nvarchar(100)"), new("CategoryId", "int"), new("SupplierId", "int"), new("Price", "decimal(18,2)"), new("Discontinued", "bit")
		}),
		new("Categories", new SqlSampleColumn[]
		{
			new("CategoryId", "int", true), new("Name", "nvarchar(50)"), new("Description", "nvarchar(max)")
		}),
		new("Suppliers", new SqlSampleColumn[]
		{
			new("SupplierId", "int", true), new("CompanyName", "nvarchar(100)"), new("ContactName", "nvarchar(100)"), new("Country", "nvarchar(50)")
		}),
		new("Employees", new SqlSampleColumn[]
		{
			new("EmployeeId", "int", true), new("FirstName", "nvarchar(50)"), new("LastName", "nvarchar(50)"), new("HireDate", "date"), new("ManagerId", "int")
		})
	};
}
