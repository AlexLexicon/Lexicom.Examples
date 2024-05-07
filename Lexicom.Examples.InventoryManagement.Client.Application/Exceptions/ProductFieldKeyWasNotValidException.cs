namespace Lexicom.Examples.InventoryManagement.Client.Application.Exceptions;
public class ProductFieldKeyWasNotValidException(string? key) : Exception($"The product field key '{key ?? "null"}' was not valid.")
{
}
