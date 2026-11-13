namespace Robbiblubber.Spellbook.Controls.Samples.ThemedComboBox;



/// <summary>This class represents a product for the data binding sample.</summary>
/// <param name="Id">Product ID, the value member.</param>
/// <param name="Name">Product name, the display member.</param>
/// <param name="Origin">Country of origin, the detail member.</param>
/// <param name="ImageKey">Image key, the image member.</param>
public sealed record SampleProduct(int Id, string Name, string Origin, string? ImageKey);
