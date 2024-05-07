using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Notifications;
public record class ProductFieldUpdatedNotification(Guid ProductId, string Key) : INotification;
