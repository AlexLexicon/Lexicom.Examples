namespace Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
public class ProductFieldAlreadyExistsException(Guid productId, string? key) : Exception($"The product with the id '{productId}' already has a product field with the key '{key ?? "null"}'.")
{
}
