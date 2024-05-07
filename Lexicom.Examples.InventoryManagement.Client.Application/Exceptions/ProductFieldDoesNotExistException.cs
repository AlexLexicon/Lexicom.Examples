namespace Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
public class ProductFieldDoesNotExistException(Guid productId, string? key) : Exception($"The product field with the product id '{productId}' and key '{key ?? "null"}' does not exist.")
{
}
