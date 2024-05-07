namespace Lexicom.Examples.InventoryManagement.Client.Application.Models;
public class ProductField
{
    public required Guid ProductId { get; init; }
    public required string Key { get; init; }
    public required string? Value { get; set; }
}
