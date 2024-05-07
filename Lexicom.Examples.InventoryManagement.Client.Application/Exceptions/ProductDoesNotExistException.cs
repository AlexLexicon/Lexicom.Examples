namespace Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
public class ProductDoesNotExistException(Guid productId) : Exception($"The product with the id '{productId}' does not exist.")
{
}
