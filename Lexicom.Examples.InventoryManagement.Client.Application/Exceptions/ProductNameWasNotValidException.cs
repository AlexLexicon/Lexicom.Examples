namespace Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
public class ProductNameWasNotValidException(string? name) : Exception($"The product name '{name ?? "null"}' was not valid.")
{
}
