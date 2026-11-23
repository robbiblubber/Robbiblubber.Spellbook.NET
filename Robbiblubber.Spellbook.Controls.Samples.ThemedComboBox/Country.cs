namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;



/// <summary>This class represents a country. It does not implement IListItem; the combo box displays its string representation.</summary>
/// <param name="Name">Country name.</param>
/// <param name="Code">ISO country code.</param>
public sealed record Country(string Name, string Code)
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Returns a string representation of this instance.</summary>
	/// <returns>Returns the country name.</returns>
	public override string ToString()
	{
		return Name;
	}
}
